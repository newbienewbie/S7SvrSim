using Microsoft.Win32;
using ReactiveUI.Fody.Helpers;
using S7Server.Simulator.ViewModels;
using S7SvrSim.Services;
using S7SvrSim.Services.Project;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace S7Svr.Simulator.ViewModels
{
    public class MainVM : ReactiveObject
    {
        private readonly IS7ServerService _server;
        private readonly IProjectFactory projectFactory;
        private IProject currentProject;

        public ConfigSnap7ServerVM ConfigVM { get; }
        public MsgLoggerVM LoggerVM { get; }
        public ConfigPyEngineVM ConfigPyEngineVM { get; }
        public RunningSnap7ServerVM RunningVM { get; }
        public OperationVM OperationVM { get; }
        public SignalWatchVM SignalWatchVM { get; }

        [Reactive]
        public bool NeedSave { get; set; }

        [Reactive]
        public int UndoCount { get; set; }

        [Reactive]
        public int RedoCount { get; set; }

        public string ProjectTitle => Path.GetFileName(currentProject.Path);

        public MainVM(IS7ServerService server, IProjectFactory projectFactory)
        {
            this._server = server;
            this.projectFactory = projectFactory;

            currentProject = projectFactory.GetOrCreateProject(null);

            this.LoggerVM = Locator.Current.GetRequiredService<MsgLoggerVM>();
            this.ConfigPyEngineVM = Locator.Current.GetRequiredService<ConfigPyEngineVM>();
            this.RunningVM = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            this.OperationVM = Locator.Current.GetRequiredService<OperationVM>();
            this.ConfigVM = Locator.Current.GetRequiredService<ConfigSnap7ServerVM>();
            this.SignalWatchVM = Locator.Current.GetRequiredService<SignalWatchVM>();

            var watchRunningStatus = this.WhenAnyValue(vm => vm.RunningVM.RunningStatus);
            this.CmdStartServer = ReactiveCommand.CreateFromTask(CmdStartServer_Impl, watchRunningStatus.Select(r => !r));
            this.CmdStopServer = ReactiveCommand.CreateFromTask(CmdStopServer_Impl, watchRunningStatus);
            this.CmdRestartServer = ReactiveCommand.CreateFromTask(CmdRestartServer_Impl, watchRunningStatus);

            UndoRedoManager.UndoRedoChanged += () =>
            {
                UndoCount = UndoRedoManager.UndoCount;
                RedoCount = UndoRedoManager.RedoCount;
            };
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

        private void CallbackNeedSave()
        {
            NeedSave = true;
            NeedSave = false;
        }

        public MessageBoxResult? NotifyIfSave()
        {
            if (NeedSave)
            {
                var result = MessageBox.Show("当前项目未保存，是否保存？", "未保存项目", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProject();
                }
                return result;
            }
            return null;
        }

        public void OpenProjectFolder()
        {
            Process.Start("explorer.exe", $"/select,{currentProject.Path}");
        }

        public void NewProject()
        {
            if (NotifyIfSave() == MessageBoxResult.Cancel)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "选择新项目保存路径",
                Filter = $"S7模拟项目|*{ProjectConst.FILE_EXTENSION}",
                FileName = Path.GetFileName(currentProject.Path),
                RestoreDirectory = true,
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            currentProject = projectFactory.CreateProject(saveFileDialog.FileName);
            UndoRedoManager.Reset();

            CallbackNeedSave();
        }

        public void LoadProject()
        {
            if (NotifyIfSave() == MessageBoxResult.Cancel)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "选择项目文件",
                Filter = $"S7模拟项目|*{ProjectConst.FILE_EXTENSION}",
                Multiselect = false,
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            IProject project;

            try
            {
                project = projectFactory.GetProject(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "打开项目文件失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            currentProject = project;
            UndoRedoManager.Reset();

            CallbackNeedSave();
        }

        public void SaveProject()
        {
            currentProject.Save();
            NeedSave = false;
        }

        public void SaveProjectAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "选择新项目保存路径",
                Filter = $"S7模拟项目|*{ProjectConst.FILE_EXTENSION}",
                FileName = Path.GetFileName(currentProject.Path),
                RestoreDirectory = true,
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            currentProject.SaveAs(saveFileDialog.FileName);
            currentProject = projectFactory.GetProject(saveFileDialog.FileName);

            CallbackNeedSave();
        }
    }
}
