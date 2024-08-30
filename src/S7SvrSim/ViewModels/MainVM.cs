using ReactiveUI;
using S7Server.Simulator.ViewModels;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    public class MainVM : ReactiveObject
    {
        private readonly IS7ServerService _server;

        public ConfigSnap7ServerVM ConfigVM { get; }
        public MsgLoggerVM LoggerVM { get; }
        public ConfigPyEngineVM ConfigPyEngineVM { get; }
        public RunningSnap7ServerVM RunningVM { get; }
        public OperationVM OperationVM { get; }

        public MainVM(IS7ServerService server)
        {
            this._server = server;

            this.LoggerVM = Locator.Current.GetRequiredService<MsgLoggerVM>();
            this.ConfigPyEngineVM = Locator.Current.GetRequiredService<ConfigPyEngineVM>();
            this.RunningVM = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            this.OperationVM = Locator.Current.GetRequiredService<OperationVM>();
            this.ConfigVM = Locator.Current.GetRequiredService<ConfigSnap7ServerVM>();


            var watchRunningStatus = this.WhenAnyValue(x => x.RunningVM.RunningStatus);
            this.CmdStartServer = ReactiveCommand.CreateFromTask(CmdStartServer_Impl, watchRunningStatus.Select(i => !i));
            this.CmdStopServer = ReactiveCommand.CreateFromTask(CmdStopServer_Impl, watchRunningStatus);
        }


        #region 启停服务器

        /// <summary>
        /// 启动服务器
        /// </summary>
        public ReactiveCommand<Unit, Unit> CmdStartServer { get; }
        private async Task CmdStartServer_Impl()
        {
            if (this.ConfigVM.AreaConfigs.Count == 0)
            {
                MessageBox.Show("当前未指定DB配置！");
                return;
            }
            this.RunningVM.IpAddress = this.ConfigVM.IpAddress;
            this.RunningVM.AreaConfigs.Clear();
            foreach (var config in this.ConfigVM.AreaConfigs)
            {
                this.RunningVM.AreaConfigs.Add(config);
            }
            await this._server.StartServerAsync();
            this.RunningVM.RunningStatus = true;
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public ReactiveCommand<Unit,Unit> CmdStopServer { get; }
        private async Task CmdStopServer_Impl()
        {
            this.RunningVM.IpAddress = string.Empty;
            this.RunningVM.AreaConfigs.Clear();
            await this._server.StopServerAsync();
            this.RunningVM.RunningStatus = false;
        }
        #endregion

    }
}
