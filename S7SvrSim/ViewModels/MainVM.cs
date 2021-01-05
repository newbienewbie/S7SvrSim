using FutureTech.Mvvm;
using S7Server.Simulator.ViewModels;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    public class MainVM : ViewModelBase
    {
        private readonly S7ServerService _s7ServerService;

        public MainVM(S7ServerService s7ServerService, RunningSnap7ServerVM runningVM, OperationVM operationVM, ConfigSnap7ServerVM configVM)
        {
            this._s7ServerService = s7ServerService;
            this.RunningVM = runningVM;
            this.OperationVM = operationVM;
            this.ConfigVM = configVM;

            this.CmdStartServer = new AsyncRelayCommand<object>(
                async o => {
                    this.RunningVM.IpAddress = this.ConfigVM.IpAddress;
                    this.RunningVM.DBConfigs.Clear();
                    foreach (var config in this.ConfigVM.DBConfigs)
                    { 
                        this.RunningVM.DBConfigs.Add(config);
                    }
                    await this._s7ServerService.StartServerAsync();
                    this.RunningVM.RunningStatus = true;
                },
                o => true
            );

            this.CmdStopServer = new AsyncRelayCommand<object>(
                async o => {
                    this.RunningVM.IpAddress = string.Empty;
                    this.RunningVM.DBConfigs.Clear();
                    await this._s7ServerService.StopServerAsync();
                    this.RunningVM.RunningStatus = false;
                },
                o => true
            );
        }

        public ICommand CmdStartServer { get; }
        public ICommand CmdStopServer { get; }

        public ConfigSnap7ServerVM ConfigVM { get; }

        public RunningSnap7ServerVM RunningVM { get; }

        public OperationVM OperationVM { get; }
    }
}
