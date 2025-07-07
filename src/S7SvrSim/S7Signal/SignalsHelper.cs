using S7SvrSim.Project;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.Services.Settings;
using S7SvrSim.Shared;
using S7SvrSim.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.S7Signal
{
    public class SignalsHelper
    {
        private readonly IS7BlockFactory blockFactory;
        private readonly IMemCache<Type[]> signalTypes;
        private readonly ISignalAddressUesdCollection signalAddressUsed;

        private bool ForbidIndexHasOddNumber { get; set; }
        private bool AllowBoolIndexHasOddNumber { get; set; }
        private bool AllowByteIndexHAsOddNumber { get; set; }

        public SignalsHelper(ISignalAddressUesdCollection signalAddressUsed, IS7BlockFactory blockFactory, ISetting<UpdateAddressOptions> setting, IMemCache<Type[]> signalTypes)
        {
            this.signalAddressUsed = signalAddressUsed;
            this.blockFactory = blockFactory;
            this.signalTypes = signalTypes;
            setting.Value.Subscribe(options =>
            {
                ForbidIndexHasOddNumber = options.ForbidIndexHasOddNumber;
                AllowBoolIndexHasOddNumber = options.AllowBoolIndexHasOddNumber;
                AllowByteIndexHAsOddNumber = options.AllowByteIndexHAsOddNumber;
            });
        }

        public SignalEditObj ItemToEditObj(SignalItem item)
        {
            var signalType = signalTypes.Value.First(ty => ty.Name == item.Type);
            var signal = (SignalBase)Activator.CreateInstance(signalType);
            signal.CopyFromSignalItem(item);

            return new SignalEditObj(signalType)
            {
                Value = signal
            };
        }

        public void RefreshValue(IEnumerable<SignalBase> signals)
        {
            signals.Where(s => s.Address == null).Each(s => s.Value = null);
            foreach (var blockSignals in signals.Where(s => s.Address != null).GroupBy(s => s.Address.AreaKind))
            {
                object preValue = null;
                SignalBase preSignal = null;
                foreach (var signal in blockSignals.OrderBy(s => s.Address))
                {
                    if (signal.Address.DbIndex < 0 || signal.Address.Index < 0 || (!signal.Address.HideOffset && signal.Address.Offset < 0))
                    {
                        signal.Value = null;
                        continue;
                    }

                    var block = signal.Address.AreaKind == S7Svr.Simulator.ViewModels.AreaKind.DB ? blockFactory.GetDataBlockService(signal.Address.DbIndex) : blockFactory.GetMemoryBlockService();
                    try
                    {
                        if (signal is Bool boolSignal)
                        {
                            if (preSignal is Bool && preSignal.Address.DbIndex == signal.Address.DbIndex && preSignal.Address.Index == signal.Address.Index)
                            {
                                preValue ??= block.ReadByte(signal.Address.Index);
                            }
                            else
                            {
                                preValue = block.ReadByte(signal.Address.Index);
                            }

                            boolSignal.Refresh((byte)preValue);
                        }
                        else
                        {
                            signal.Refresh(block);
                            preValue = signal;
                        }
                        preSignal = signal;
                    }
                    catch (Exception e) when (e is ArgumentException || e is IndexOutOfRangeException || e is InvalidOperationException)
                    {
                        signal.Value = null;
                    }
                }
            }
        }

        public void UpdateAddress(IEnumerable<SignalBase> signals)
        {
            if (signals.Count() <= 1)
            {
                return;
            }

            var preSignal = signals.First();
            var preAddress = preSignal.Address;
            if (preAddress == null)
            {
                return;
            }

            IAddressUsed preUsed = null;

            try
            {
                preUsed = signalAddressUsed.GetAddressUsed(preSignal);
            }
            catch (Exception e) when (e is KeyNotFoundException || e is NotSupportedException)
            {

            }

            if (preUsed == null)
            {
                return;
            }


            foreach (var signal in signals.Skip(1))
            {
                if (signalAddressUsed.TryGetAddressUsed(signal, out var used))
                {
                    var dbIndex = preAddress.DbIndex;
                    int index;
                    byte offset = 0;

                    if (signal is Holding)
                    {
                        index = preAddress.Index + preUsed.IndexSize;
                    }
                    else
                    {
                        if (preUsed.IndexSize == 0 && used.IndexSize == 0 && preSignal is Bool && signal is Bool)
                        {
                            if (preAddress.Offset >= 7)
                            {
                                index = preAddress.Index + 1;
                                offset = 0;
                            }
                            else
                            {
                                index = preAddress.Index;
                                offset = (byte)(preAddress.Offset + preUsed.OffsetSize);
                            }
                        }
                        else
                        {
                            index = preAddress.Index + (preUsed.IndexSize == 0 ? 1 : preUsed.IndexSize);
                        }

                        if (index % 2 == 1 && (signal is not Bool || !AllowBoolIndexHasOddNumber) && (signal is not Byte || !AllowByteIndexHAsOddNumber) && ForbidIndexHasOddNumber)
                        {
                            index += 1;
                        }
                    }

                    var newAddress = new SignalAddress(dbIndex, index, offset)
                    {
                        HideOffset = used.IndexSize != 0 || signal is Holding,
                        AreaKind = preAddress.AreaKind
                    };

                    if (newAddress != signal.Address)
                    {
                        var command = new ValueChangedCommand<SignalAddress>(address =>
                        {
                            signal.Address = address;
                        }, signal.Address, newAddress);
                        UndoRedoManager.Run(command);
                    }

                    preSignal = signal;
                    preAddress = newAddress;
                    preUsed = used;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
