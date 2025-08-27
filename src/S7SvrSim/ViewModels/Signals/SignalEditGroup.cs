using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator;
using S7SvrSim.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace S7SvrSim.ViewModels.Signals
{
    public class SignalEditGroup : ReactiveObject
    {
        [Reactive]
        public string Name { get; set; }

        public ObservableCollection<SignalEditObj> Signals { get; }

        private readonly ISaveNotifier saveNotifier;
        private readonly IDisposable nameDisposable;
        public SignalEditGroup(string name, IEnumerable<SignalEditObj> signals)
        {
            Name = name;
            Signals = new ObservableCollection<SignalEditObj>(signals);

            saveNotifier = ((App)App.Current).ServiceProvider.GetRequiredService<ISaveNotifier>();

            nameDisposable = this.WhenAnyValue(x => x.Name).Subscribe(_ => saveNotifier.NotifyNeedSave(true));

            Signals.CollectionChanged += Signals_CollectionChanged;
        }

        private void Signals_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            saveNotifier.NotifyNeedSave(true);
        }

        ~SignalEditGroup()
        {
            try
            {
                nameDisposable?.Dispose();
                Signals.CollectionChanged -= Signals_CollectionChanged;
            }
            catch (Exception)
            {

            }
        }
    }
}
