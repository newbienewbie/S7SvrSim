using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace S7SvrSim.S7Signal
{
    public static class SignalsExtensions
    {
        private class AddressUsed
        {
            public AddressUsedAttribute Attribute { get; }
            public MethodInfo CalcMethod { get; }
            public AddressUsed(Type ty)
            {
                Attribute = ty.GetCustomAttribute<AddressUsedAttribute>();
                if (Attribute != null && !string.IsNullOrEmpty(Attribute.CalcMethod))
                {
                    CalcMethod = ty.GetMethod(Attribute.CalcMethod);
                }
            }
        }

        private static Dictionary<Type, AddressUsed> SignalAddressUsed { get; }

        static SignalsExtensions()
        {
            var signalTypes = typeof(SignalWatchVM).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SignalBase)));
            SignalAddressUsed = signalTypes.Select(ty => (Type: ty, Attr: new AddressUsed(ty))).Where(it => it.Attr.Attribute != null).ToDictionary(it => it.Type, it => it.Attr);
        }

        public static void RefreshValue(this IEnumerable<SignalBase> signals, IS7BlockFactory blockFactory)
        {
            foreach (var blockSignals in signals.Where(s => s.Address != null).GroupBy(s => s.Address.AreaKind))
            {
                object preValue = null;
                SignalBase preSignal = null;
                foreach (var signal in blockSignals.OrderBy(s => s.Address))
                {
                    var block = signal.Address.AreaKind == S7Svr.Simulator.ViewModels.AreaKind.DB ? blockFactory.GetDataBlockService(signal.Address.DbIndex) : blockFactory.GetMemoryBlockService();
                    try
                    {
                        if (signal is Bool boolSignal)
                        {
                            if (preSignal is Bool && preSignal.Address.Index == signal.Address.Index)
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
                            preValue = signal.Value;
                        }
                        preSignal = signal;
                    }
                    catch (Exception e) when (e is ArgumentException || e is IndexOutOfRangeException || e is InvalidOperationException)
                    {

                    }
                }
            }
        }

        public static void UpdateAddress(this IEnumerable<SignalBase> signals, UpdateAddressOptions options = null)
        {
            if (signals.Count() <= 1)
            {
                return;
            }

            options ??= new UpdateAddressOptions();

            var preSignal = signals.First();
            var preAddress = preSignal.Address;
            if (preAddress == null)
            {
                return;
            }

            AddressUsed preUsed = null;

            try
            {
                preUsed = SignalAddressUsed[preSignal.GetType()];
            }
            catch (KeyNotFoundException)
            {

            }

            if (preUsed == null)
            {
                return;
            }


            foreach (var signal in signals.Skip(1))
            {
                if (SignalAddressUsed.TryGetValue(signal.GetType(), out var used))
                {
                    var preUsedItem = GetAddressUsedItem(preUsed, preSignal);
                    var usedItem = GetAddressUsedItem(used, signal);

                    var dbIndex = preAddress.DbIndex;
                    int index;
                    byte offset = 0;

                    if (signal.Value is Holding)
                    {
                        index = preAddress.Index + preUsedItem.IndexSize;
                    }
                    else
                    {
                        if (preUsedItem.IndexSize == 0 && usedItem.IndexSize == 0 && preSignal.Value is Bool && signal.Value is Bool)
                        {
                            if (preAddress.Offset >= 7)
                            {
                                index = preAddress.Index + 1;
                                offset = 0;
                            }
                            else
                            {
                                index = preAddress.Index;
                                offset = (byte)(preAddress.Offset + preUsedItem.OffsetSize);
                            }
                        }
                        else
                        {
                            index = preAddress.Index + (preUsedItem.IndexSize == 0 ? 1 : preUsedItem.IndexSize);
                        }

                        if (index % 2 == 1 && (signal.Value is not Bool || !options.AllowBoolIndexHasOddNumber) && options.ForbidIndexHasOddNumber)
                        {
                            index += 1;
                        }
                    }

                    var newAddress = new SignalAddress(dbIndex, index, offset)
                    {
                        HideOffset = usedItem.IndexSize != 0 || signal.Value is Holding,
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

        private static AddressUsedItem GetAddressUsedItem(AddressUsed used, SignalBase signal)
        {
            if (used.CalcMethod == null)
            {
                return new AddressUsedItem()
                {
                    IndexSize = used.Attribute.IndexSize,
                    OffsetSize = used.Attribute.OffsetSize
                };
            }
            else
            {
                return (AddressUsedItem)used.CalcMethod.Invoke(signal, []);
            }
        }
    }
}
