using FutureTech.Mvvm;
using S7Server.Simulator.ViewModels;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    public class MainVM : ViewModelBase
    {
        private readonly S7ServerService _s7ServerService;

        public MainVM(S7ServerService s7ServerService)
        {
            this._s7ServerService = s7ServerService;
            this.ConfigVM = new ConfigSnap7ServerVM();
            this.RunningVM = new RunningSnap7ServerVM();
            this.OperationVM = new OperationVM(this.RunningVM, this._s7ServerService);

            this.CmdStartServer = new AsyncRelayCommand<object>(
                async o => {
                    this.RunningVM.IpAddress = this.ConfigVM.IpAddress;
                    this.RunningVM.DBConfigs.Clear();
                    foreach (var config in this.ConfigVM.DBConfigs)
                    { 
                        this.RunningVM.DBConfigs.Add(config);
                    }
                    await this._s7ServerService.StartServerAsync(this.RunningVM);
                },
                o => true
            );

            this.CmdStopServer = new AsyncRelayCommand<object>(
                async o => {
                    this.RunningVM.IpAddress = string.Empty;
                    this.RunningVM.DBConfigs.Clear();
                    await this._s7ServerService.StopServerAsync();
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
