using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using S7SvrSim.Shared;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S7SvrSim.ViewModels
{
    public partial class SignalWatchVM : ViewModelBase
    {
        private readonly IS7DataBlockService db;
        private readonly IMediator mediator;

        CancellationTokenSource cancelSource;

        public ObservableCollection<Type> SignalTypes { get; }
        public ObservableCollection<ObjectWith<ISignal, Type>> Signals { get; } = new ObservableCollection<ObjectWith<ISignal, Type>>();

        [ObservableProperty]
        private int scanSpan = 50;

        public SignalWatchVM(IS7DataBlockService db, IMediator mediator)
        {
            var runningModel = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            runningModel.PropertyChanged += RunningModel_PropertyChanged;

            SignalTypes = new ObservableCollection<Type>(typeof(SignalWatchVM).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.GetInterface(typeof(ISignal).Name) != null));
            this.db = db;
            this.mediator = mediator;
        }

        private ObjectWith<ISignal, Type> InitSignal(Type signalType)
        {
            var signal = (ISignal)Activator.CreateInstance(signalType);
            signal.Name = signalType.Name;
            var result = new ObjectWith<ISignal, Type>() { Value = signal, Other = signalType };

            result.OtherChanged += ty =>
            {
                string name = result.Value.Name;
                var address = (string)result.Value.FormatAddress?.Clone();
                result.Value = (ISignal)Activator.CreateInstance(ty);
                result.Value.Name = name;
                result.Value.FormatAddress = address;
            };

            return result;
        }

        [RelayCommand]
        private void NewSignal(Type signalType)
        {
            Signals.Add(InitSignal(signalType));
        }

        private void RunningModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is RunningSnap7ServerVM runningModel && e.PropertyName == nameof(RunningSnap7ServerVM.RunningStatus))
            {
                if (runningModel.RunningStatus)
                {
                    StartWatch();
                }
                else
                {
                    EndWatch();
                }
            }
        }

        Task task;
        private void StartWatch()
        {
            if (cancelSource != null && !cancelSource.IsCancellationRequested)
            {
                EndWatch();
            }

            cancelSource = new CancellationTokenSource();
            task = WatchTask(cancelSource.Token);
        }

        private void EndWatch()
        {
            if (cancelSource != null)
            {
                try
                {
                    cancelSource.Cancel();
                }
                catch (Exception)
                {

                }
            }
        }

        private async Task WatchTask(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Signals.Each(s =>
                {
                    if (s.Value is SignalBase bs)
                    {
                        try
                        {
                            bs.Refresh(db);
                        }
                        catch (Exception e) when (e is ArgumentException || e is IndexOutOfRangeException)
                        {

                        }
                    }
                });
                this.RaisePropertyChanged(nameof(Signals));
                await Task.Delay(TimeSpan.FromMilliseconds(ScanSpan >= 0 ? ScanSpan : 50), token);
            }
        }
    }
}
