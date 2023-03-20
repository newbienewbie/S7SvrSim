using Reactive.Bindings;
using S7Server.Simulator.ViewModels;
using S7SvrSim.ViewModels;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    public class MainVM 
    {
        private readonly IS7ServerService _s7ServerService;

        public MainVM(IS7ServerService s7ServerService, ConfigPyEngineVM configPyEngineVM , RunningSnap7ServerVM runningVM, OperationVM operationVM, ConfigSnap7ServerVM configVM)
        {
            this._s7ServerService = s7ServerService;
            ConfigPyEngineVM = configPyEngineVM;
            this.RunningVM = runningVM;
            this.OperationVM = operationVM;
            this.ConfigVM = configVM;

            this.CmdStartServer = this.RunningVM.RunningStatus.Select(i => !i)
                .ToReactiveCommand()
                .WithSubscribe(async i => {
                    if (this.ConfigVM.DBConfigs.Count == 0)
                    {
                        MessageBox.Show("当前未指定DB配置！");
                        return;
                    }
                    this.RunningVM.IpAddress.Value = this.ConfigVM.IpAddress.Value;
                    this.RunningVM.DBConfigs.Clear();
                    foreach (var config in this.ConfigVM.DBConfigs)
                    {
                        this.RunningVM.DBConfigs.Add(config);
                    }
                    await this._s7ServerService.StartServerAsync();
                    this.RunningVM.RunningStatus.Value = true;
                });

            this.CmdStopServer = this.RunningVM.RunningStatus
                .ToReactiveCommand()
                .WithSubscribe(async i => {
                    this.RunningVM.IpAddress.Value = string.Empty;
                    this.RunningVM.DBConfigs.Clear();
                    await this._s7ServerService.StopServerAsync();
                    this.RunningVM.RunningStatus.Value = false;
                });
        }
        public ReactiveCommand<object> CmdStartServer { get; }
        public ReactiveCommand<object> CmdStopServer { get; }

        public ConfigSnap7ServerVM ConfigVM { get; }

        public ConfigPyEngineVM ConfigPyEngineVM { get; }
        public RunningSnap7ServerVM RunningVM { get; }

        public OperationVM OperationVM { get; }
    }
}
