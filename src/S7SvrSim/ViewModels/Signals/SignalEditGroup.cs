using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace S7SvrSim.ViewModels.Signals
{
    public class SignalEditGroup : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; }

        public ObservableCollection<SignalEditObj> Signals { get; }

        public SignalEditGroup(string name, IEnumerable<SignalEditObj> signals)
        {
            Name = name;
            Signals = new ObservableCollection<SignalEditObj>(signals);
        }
    }
}
