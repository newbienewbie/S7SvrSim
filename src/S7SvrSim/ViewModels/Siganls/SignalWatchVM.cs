using CommunityToolkit.Mvvm.ComponentModel;
using S7Svr.Simulator;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.ViewModels.Siganls;
using Splat;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S7SvrSim.ViewModels
{
    public partial class SignalWatchVM : ViewModelBase
    {
        private readonly SignalsHelper signalsHelper;
        private readonly SignalsCollection signals;
        private readonly IMemCache<WatchState> watchStateCache;

        [ObservableProperty]
        private int scanSpan = 50;

        public SignalWatchVM(SignalsHelper signalsHelper, IMemCache<WatchState> watchStateCache)
        {
            this.signalsHelper = signalsHelper;
            this.signals = Locator.Current.GetRequiredService<SignalsCollection>();
            this.watchStateCache = watchStateCache;

            var runningModel = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            runningModel.WhenAnyValue(rm => rm.RunningStatus).Subscribe(RunningStatusChanged);
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
        }

        private void RegistCommandEventHandle(IHistoryCommand command)
        {
            command.AfterExecute += CommandEventHandle;
            command.AfterUndo += CommandEventHandle;
        }

        #region ScanSpan For UndoRedo
        internal void SetScanSpan(int scanSpan)
        {
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            this.scanSpan = scanSpan;
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            OnPropertyChanged(nameof(ScanSpan));
        }

        partial void OnScanSpanChanged(int oldValue, int newValue)
        {
            var command = new ValueChangedCommand<int>(SetScanSpan, oldValue, newValue);
            RegistCommandEventHandle(command);
            UndoRedoManager.Run(command);
        }
        #endregion

        #region Watch Method
        CancellationTokenSource watchCancelSource;
        Task watchTask;

        private void RunningStatusChanged(bool runningStatus)
        {
            if (runningStatus)
            {
                StartWatch();
                watchStateCache.Write(new WatchState(true));
            }
            else
            {
                EndWatch();
                watchStateCache.Write(new WatchState(false));
            }
        }

        private void StartWatch()
        {
            if (watchCancelSource != null && !watchCancelSource.IsCancellationRequested)
            {
                EndWatch();
            }

            watchCancelSource = new CancellationTokenSource();
            watchTask = WatchTask(watchCancelSource.Token);
        }

        private void EndWatch()
        {
            if (watchCancelSource != null)
            {
                try
                {
                    watchCancelSource.Cancel();
                }
                catch (Exception)
                {

                }
                finally
                {
                    watchTask = null;
                }
            }
        }

        private async Task WatchTask(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                signalsHelper.RefreshValue(signals.Signals.Select(s => s.Value));
                await Task.Delay(TimeSpan.FromMilliseconds(ScanSpan >= 0 ? ScanSpan : 50), token);
            }
        }
        #endregion
    }
}
