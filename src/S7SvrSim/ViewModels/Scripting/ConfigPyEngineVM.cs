using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reactive.Linq;
using S7SvrSim.Services;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using DynamicData;
using System.Diagnostics;
using S7SvrSim.Shared;
using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.ViewModels
{
    public class ConfigPyEngineVM : ReactiveObject
    {
        private const string PYENGINE_SEARCH_PATHS_FILE = "py-search-path.txt";
        private readonly PyScriptRunner _pyRunner;
        private string SavedFileName
        {
            get
            {
                var processPath = Path.GetDirectoryName(Environment.ProcessPath);
                if (processPath != null)
                {
                    return Path.Combine(processPath, PYENGINE_SEARCH_PATHS_FILE);
                }
                else
                {
                    return PYENGINE_SEARCH_PATHS_FILE;
                }
            }
        }

        public ConfigPyEngineVM(PyScriptRunner pyRunner)
        {
            this._pyRunner = pyRunner;
            var searchpathes = this._pyRunner.PyEngine.GetSearchPaths();

            if (searchpathes != null)
            { 
                this.PyEngineSearchPaths.AddRange(searchpathes);
            }

            this.CmdSelectModulePath = ReactiveCommand.Create(() => {
                var dialog = new FolderBrowserDialog() {
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
                if (PyEngineSearchPaths.Remove(path))
                {
                    this._pyRunner.PyEngine.SetSearchPaths(PyEngineSearchPaths);
                }
            });

            LoadSearchPath();
            PyEngineSearchPaths.CollectionChanged += PyEngineSearchPaths_CollectionChanged;
        }

        public ObservableCollection<string> PyEngineSearchPaths { get; } = new ObservableCollection<string>();

        private void PyEngineSearchPaths_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SaveSearchPath();
        }

        private void LoadSearchPath(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = SavedFileName;
            }

            if (File.Exists(path))
            {
                var fileContent = File.ReadAllLines(path);
                PyEngineSearchPaths.Clear();
                foreach (var line in fileContent)
                {
                    PyEngineSearchPaths.Add(line);
                }
                this._pyRunner.PyEngine.SetSearchPaths(PyEngineSearchPaths);
            }
        }

        private void SaveSearchPath(string path = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = SavedFileName;
            }

            using var fileStream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            fileStream.WriteString(string.Join(Environment.NewLine, PyEngineSearchPaths));
        }

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
            var list = this._pyRunner.PyEngine.GetSearchPaths();
            if (list.Contains(this.SelectedModulePath))
            {
                MessageBox.Show($"当前所选择的路径已经在检索路径中！无需重复添加");
                return;
            }
            list.Add(this.SelectedModulePath);
            this._pyRunner.PyEngine.SetSearchPaths(list);
            this.PyEngineSearchPaths.Add(this.SelectedModulePath);
        }
        #endregion
        /// <summary>
        /// 删除路径
        /// </summary>
        public ReactiveCommand<string, Unit> CmdDeletePath { get; }
    }
}
