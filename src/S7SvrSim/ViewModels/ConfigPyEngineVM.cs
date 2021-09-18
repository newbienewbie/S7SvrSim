using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reactive.Linq;
using S7SvrSim.Services;

namespace S7SvrSim.ViewModels
{
    public class ConfigPyEngineVM
    {
        private readonly PyScriptRunner _pyRunner;

        public ConfigPyEngineVM(PyScriptRunner pyRunner)
        {
            this._pyRunner = pyRunner;
            var searchpathes = this._pyRunner.PyEngine.GetSearchPaths();
            this.PyEngineSearchPaths = new ReactiveCollection<string>();
            if (searchpathes != null)
            { 
                this.PyEngineSearchPaths.AddRangeOnScheduler(searchpathes);
            }

            this.SelectedModulePath = new ReactiveProperty<string>();

            this.CmdSelectModulePath = new ReactiveCommand();
            this.CmdSelectModulePath.Subscribe(() => {
                var dialog = new FolderBrowserDialog() {
                    Description = "选择Python模块路径",
                    UseDescriptionForTitle = true,
                    ShowNewFolderButton = true
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.SelectedModulePath.Value = dialog.SelectedPath;
                }
            });


            this.CmdSubmitSelectPath = new ReactiveCommand(
                this.SelectedModulePath.Select(s => !string.IsNullOrEmpty(s)),
                false
                );
            this.CmdSubmitSelectPath.Subscribe(()=> {
                var list = this._pyRunner.PyEngine.GetSearchPaths();
                if (list.Contains(this.SelectedModulePath.Value))
                {
                    MessageBox.Show($"当前所选择的路径已经在检索路径中！无需重复添加");
                    return;
                }
                list.Add(this.SelectedModulePath.Value);
                this._pyRunner.PyEngine.SetSearchPaths(list);
                this.PyEngineSearchPaths.AddOnScheduler(this.SelectedModulePath.Value);
            });


        }

        public ReactiveCollection<string> PyEngineSearchPaths { get; }
        public ReactiveProperty<string> SelectedModulePath { get; }

        public ReactiveCommand CmdSelectModulePath { get; }

        public ReactiveCommand CmdSubmitSelectPath { get; }
    }
}
