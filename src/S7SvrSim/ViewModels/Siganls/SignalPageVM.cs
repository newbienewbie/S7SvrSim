using S7SvrSim.Services;
using S7SvrSim.ViewModels.Siganls;
using Splat;
using System;

namespace S7SvrSim.ViewModels
{
    public class SignalPageVM
    {
        public SignalPageVM(IMemCache<Type[]> signalTypes)
        {
            Signals = Locator.Current.GetRequiredService<SignalsCollection>();
            WatchVM = Locator.Current.GetRequiredService<SignalWatchVM>();
            DragSignalsVM = Locator.Current.GetRequiredService<DragSignalsVM>();
            UpdateAddressOptionsVM = Locator.Current.GetRequiredService<UpdateAddressOptionsVM>();
            SignalTypes = signalTypes.Value;
        }

        public SignalsCollection Signals { get; }
        public SignalWatchVM WatchVM { get; }
        public DragSignalsVM DragSignalsVM { get; }
        public UpdateAddressOptionsVM UpdateAddressOptionsVM { get; }
        public Type[] SignalTypes { get; }
    }
}
