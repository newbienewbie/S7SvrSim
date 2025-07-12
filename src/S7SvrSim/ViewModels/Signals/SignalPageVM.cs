using S7SvrSim.Services;
using S7SvrSim.ViewModels.Signals;
using Splat;
using System;

namespace S7SvrSim.ViewModels
{
    public class SignalPageVM
    {
        public SignalPageVM(IMemCache<SignalType[]> signalTypes)
        {
            Signals = Locator.Current.GetRequiredService<SignalsCollection>();
            WatchVM = Locator.Current.GetRequiredService<SignalWatchVM>();
            DragSignalsVM = Locator.Current.GetRequiredService<DragSignalsVM>();
            UpdateAddressOptionsVM = Locator.Current.GetRequiredService<UpdateAddressOptionsVM>();
            SignalExcelVM = Locator.Current.GetRequiredService<SignalExcelVM>();
            SignalTypes = signalTypes.Value;
        }

        public SignalsCollection Signals { get; }
        public SignalWatchVM WatchVM { get; }
        public DragSignalsVM DragSignalsVM { get; }
        public UpdateAddressOptionsVM UpdateAddressOptionsVM { get; }
        public SignalExcelVM SignalExcelVM { get; }
        public SignalType[] SignalTypes { get; }
    }
}
