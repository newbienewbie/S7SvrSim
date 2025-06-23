using S7SvrSim.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.S7Signal
{
    public static class SignalsExtensions
    {
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
    }
}
