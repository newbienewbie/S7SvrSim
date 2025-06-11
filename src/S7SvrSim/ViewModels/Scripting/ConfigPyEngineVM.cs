using DynamicData;
using Microsoft.Scripting.Hosting;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace S7SvrSim.ViewModels
{
    public class ConfigPyEngineVM : ReactiveObject
    {
        private readonly static string[] DefaultPath = [".", Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "lib"), Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "DLLs"), Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), "predefined")];
        private readonly PyScriptRunner _pyRunner;

        public ConfigPyEngineVM(PyScriptRunner pyRunner)
        {
            this._pyRunner = pyRunner;
            var searchpathes = this._pyRunner.PyEngine.GetSearchPaths();

            if (searchpathes != null)
            {
                if (!DefaultPath.Except(searchpathes).Any())
                {
                    this.PyEngineSearchPaths.Add("$DEFAULT");
                    this.PyEngineSearchPaths.AddRange(searchpathes.Except(DefaultPath));

                }
                else
                {
                    this.PyEngineSearchPaths.AddRange(searchpathes);
                }
            }

            this.CmdSelectModulePath = ReactiveCommand.Create(() =>
            {
                var dialog = new FolderBrowserDialog()
                {
                    Description = "选择Python模块路径",
                    UseDescriptionForTitle = true,
                    ShowNewFolderButton = true
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.SelectedModulePath = dialog.SelectedPath;
                }
            });

            var canSubmitSelectPath = this.WhenAnyValue(x => x.SelectedModulePath)
                .Select(p => !string.IsNullOrEmpty(p));

            this.CmdSubmitSelectPath = ReactiveCommand.Create(CmdSubmitSelectPath_Impl, canSubmitSelectPath);
            this.CmdDeletePath = ReactiveCommand.Create<string>(path =>
            {
                var command = new IListChangedCommand<string>(PyEngineSearchPaths, Services.Command.ChangedType.Remove, path);
                CommandEventRegist(command);
                UndoRedoManager.Run(command);
            });
        }

        internal void SetSearchPaths(ScriptEngine engine)
        {
            engine.SetSearchPaths(ActualSearchPaths.ToArray());
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            SetSearchPaths(_pyRunner.PyEngine);
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
        }

        private void CommandEventRegist(ICommand command)
        {
            command.AfterExecute += CommandEventHandle;
            command.AfterUndo += CommandEventHandle;
        }

        public ObservableCollection<string> PyEngineSearchPaths { get; } = new ObservableCollection<string>();
        private IEnumerable<string> ActualSearchPaths => PyEngineSearchPaths.Select(s =>
        {
            if (s == "$DEFAULT")
            {
                return DefaultPath;
            }
            return [s];
        }).Merge();

        #region 选择路径
        [Reactive]
        public string SelectedModulePath { get; set; }

        /// <summary>
        /// 选择路径
        /// </summary>
        public ReactiveCommand<Unit, Unit> CmdSelectModulePath { get; }

        /// <summary>
        /// 提交所选择的路径
        /// </summary>
        public ReactiveCommand<Unit, Unit> CmdSubmitSelectPath { get; }
        private void CmdSubmitSelectPath_Impl()
        {
            if (ActualSearchPaths.Contains(this.SelectedModulePath))
            {
                MessageBox.Show($"当前所选择的路径已经在检索路径中！无需重复添加");
                return;
            }
            var command = new IListChangedCommand<string>(PyEngineSearchPaths, Services.Command.ChangedType.Add, this.SelectedModulePath);
            CommandEventRegist(command);
            UndoRedoManager.Run(command);
        }
        #endregion
        /// <summary>
        /// 删除路径
        /// </summary>
        public ReactiveCommand<string, Unit> CmdDeletePath { get; }
    }
}
