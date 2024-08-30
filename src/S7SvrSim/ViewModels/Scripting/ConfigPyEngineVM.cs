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

namespace S7SvrSim.ViewModels
{
    public class ConfigPyEngineVM : ReactiveObject
    {
        private readonly PyScriptRunner _pyRunner;

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



    }
}
