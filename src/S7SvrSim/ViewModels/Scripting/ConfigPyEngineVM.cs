using DynamicData;
using Microsoft.Scripting.Hosting;
using ReactiveUI.Fody.Helpers;
using S7SvrSim.Services;
using S7SvrSim.Services.Project;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace S7SvrSim.ViewModels
{
    public class ConfigPyEngineVM : ReactiveObject
    {

        private readonly PyScriptRunner _pyRunner;
        private readonly IPyPathService pyPathService;

        public ConfigPyEngineVM(PyScriptRunner pyRunner, IPyPathService pyPathService, ISaveNotifier saveNotifier)
        {
            this._pyRunner = pyRunner;
            this.pyPathService = pyPathService;
            var searchpathes = this._pyRunner.PyEngine.GetSearchPaths();

            if (searchpathes != null)
            {
                this.PyEngineSearchPaths.AddRange(searchpathes);
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
                PyEngineSearchPaths.Remove(path);
            });

            PyEngineSearchPaths.CollectionChanged += (_, _) =>
            {
                SetSearchPaths(_pyRunner.PyEngine);
                saveNotifier.NotifyNeedSave();
            };
        }

        internal void SetSearchPaths(ScriptEngine engine)
        {
            engine.SetSearchPaths(PyEngineSearchPaths.Select(pyPathService.ReplaceEnv).Distinct().ToArray());
        }

        public ObservableCollection<string> PyEngineSearchPaths { get; } = new ObservableCollection<string>();

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
            if (PyEngineSearchPaths.Contains(this.SelectedModulePath))
            {
                MessageBox.Show($"当前所选择的路径已经在检索路径中！无需重复添加");
                return;
            }
            PyEngineSearchPaths.Add(SelectedModulePath);
        }
        #endregion

        /// <summary>
        /// 删除路径
        /// </summary>
        public ReactiveCommand<string, Unit> CmdDeletePath { get; }
    }
}
