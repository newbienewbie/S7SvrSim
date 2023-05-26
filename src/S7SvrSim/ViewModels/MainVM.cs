using Reactive.Bindings;
using S7Server.Simulator.ViewModels;
using S7SvrSim.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    public class MainVM 
    {
        private readonly IS7DataBlockService _s7ServerService;

        public MainVM(IS7DataBlockService s7ServerService, MsgLoggerVM loggerVM, ConfigPyEngineVM configPyEngineVM , RunningSnap7ServerVM runningVM, OperationVM operationVM, ConfigSnap7ServerVM configVM)
        {
            this._s7ServerService = s7ServerService;
            LoggerVM = loggerVM;
            ConfigPyEngineVM = configPyEngineVM;
            this.RunningVM = runningVM;
            this.OperationVM = operationVM;
            this.ConfigVM = configVM;

            this.CmdStartServer = this.RunningVM.RunningStatus.Select(i => !i)
                .ToReactiveCommand()
                .WithSubscribe(async i => {
                    if (this.ConfigVM.AreaConfigs.Count == 0)
                    {
                        MessageBox.Show("当前未指定DB配置！");
                        return;
                    }
                    this.RunningVM.IpAddress.Value = this.ConfigVM.IpAddress.Value;
                    this.RunningVM.AreaConfigs.Clear();
                    foreach (var config in this.ConfigVM.AreaConfigs)
                    {
                        this.RunningVM.AreaConfigs.Add(config);
                    }
                    await this._s7ServerService.StartServerAsync();
                    this.RunningVM.RunningStatus.Value = true;
                });

            this.CmdStopServer = this.RunningVM.RunningStatus
                .ToReactiveCommand()
                .WithSubscribe(async i => {
                    this.RunningVM.IpAddress.Value = string.Empty;
                    this.RunningVM.AreaConfigs.Clear();
                    await this._s7ServerService.StopServerAsync();
                    this.RunningVM.RunningStatus.Value = false;
                });




        }
        public ReactiveCommand<object> CmdStartServer { get; }
        public ReactiveCommand<object> CmdStopServer { get; }

        public ConfigSnap7ServerVM ConfigVM { get; }
        public MsgLoggerVM LoggerVM { get; }
        public ConfigPyEngineVM ConfigPyEngineVM { get; }
        public RunningSnap7ServerVM RunningVM { get; }

        public OperationVM OperationVM { get; }



    }
}
