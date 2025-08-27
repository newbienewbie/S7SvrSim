using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.ViewModels.Signals;
using Splat;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace S7SvrSim.ViewModels
{
    public partial class SignalWatchVM : ReactiveObject
    {
        private readonly SignalsHelper signalsHelper;
        private readonly SignalsCollection signals;
        private readonly RunningSnap7ServerVM runningVM;
        private readonly IMemCache<WatchState> watchStateCache;
        private readonly ISaveNotifier saveNotifier;
        private readonly ISignalAddressUesdCollection signalAddressUesd;

        [Reactive]
        public int ScanSpan { get; set; } = 50;

        public SignalWatchVM(SignalsHelper signalsHelper, IMemCache<WatchState> watchStateCache, ISaveNotifier saveNotifier, ISignalAddressUesdCollection signalAddressUesd)
        {
            this.signalsHelper = signalsHelper;
            this.signals = Locator.Current.GetRequiredService<SignalsCollection>();
            runningVM = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            this.watchStateCache = watchStateCache;
            this.saveNotifier = saveNotifier;
            this.signalAddressUesd = signalAddressUesd;
            runningVM.WhenAnyValue(rm => rm.RunningStatus).Subscribe(RunningStatusChanged);

            this.WhenAnyValue(vm => vm.ScanSpan).Subscribe(_ => saveNotifier.NotifyNeedSave(true));
        }

        internal void SetScanSpan(int scanSpan)
        {
            this.ScanSpan = scanSpan;
            this.RaisePropertyChanged(nameof(ScanSpan));
        }

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
                signalsHelper.RefreshValue(signals.Signals.Where(s =>
                {
                    if (signalAddressUesd.TryGetAddressUsed(s.Value, out var addressUsed))
                    {
                        var config = runningVM.AreaConfigs.FirstOrDefault(ac => (ac.AreaKind == AreaKind.MB && s.Value.Address?.AreaKind == AreaKind.MB) || (ac.AreaKind == AreaKind.DB && s.Value.Address?.AreaKind == AreaKind.DB && ac.BlockNumber == s.Value.Address?.DbIndex));
                        return config != null && config.BlockSize > (s.Value.Address?.Index + addressUsed.IndexSize);
                    }
                    return false;
                }).Select(s => s.Value));
                await Task.Delay(TimeSpan.FromMilliseconds(ScanSpan >= 0 ? ScanSpan : 50), token);
            }
        }
        #endregion
    }
}
