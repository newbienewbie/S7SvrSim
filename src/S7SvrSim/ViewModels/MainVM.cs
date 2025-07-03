using S7Server.Simulator.ViewModels;
using S7SvrSim.Services.Project;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

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
        public SignalPageVM SignalPageVM { get; }
        public ProjectVM ProjectVM { get; }

        public MainVM(IS7ServerService server, IProjectFactory projectFactory)
        {
            this._server = server;

            this.LoggerVM = Locator.Current.GetRequiredService<MsgLoggerVM>();
            this.ConfigPyEngineVM = Locator.Current.GetRequiredService<ConfigPyEngineVM>();
            this.RunningVM = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            this.OperationVM = Locator.Current.GetRequiredService<OperationVM>();
            this.ConfigVM = Locator.Current.GetRequiredService<ConfigSnap7ServerVM>();
            this.SignalPageVM = Locator.Current.GetRequiredService<SignalPageVM>();
            this.ProjectVM = Locator.Current.GetRequiredService<ProjectVM>();

            var watchRunningStatus = this.WhenAnyValue(vm => vm.RunningVM.RunningStatus);
            this.CmdStartServer = ReactiveCommand.CreateFromTask(CmdStartServer_Impl, watchRunningStatus.Select(r => !r));
            this.CmdStopServer = ReactiveCommand.CreateFromTask(CmdStopServer_Impl, watchRunningStatus);
            this.CmdRestartServer = ReactiveCommand.CreateFromTask(CmdRestartServer_Impl, watchRunningStatus);
        }

        #region 启停服务器
        /// <summary>
        /// 启动服务器
        /// </summary>
        public ReactiveCommand<Unit, Unit> CmdStartServer { get; }
        private async Task CmdStartServer_Impl()
        {
            if (RunningVM.RunningStatus)
            {
                return;
            }
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
        public ReactiveCommand<Unit, Unit> CmdStopServer { get; }
        private async Task CmdStopServer_Impl()
        {
            if (!RunningVM.RunningStatus)
            {
                return;
            }
            this.RunningVM.IpAddress = string.Empty;
            this.RunningVM.AreaConfigs.Clear();
            await this._server.StopServerAsync();
            this.RunningVM.RunningStatus = false;
        }

        /// <summary>
        /// 停止服务器
        /// </summary>
        public ReactiveCommand<Unit, Unit> CmdRestartServer { get; }
        private async Task CmdRestartServer_Impl()
        {
            await CmdStopServer_Impl();
            await CmdStartServer_Impl();
        }
        #endregion
    }
}
