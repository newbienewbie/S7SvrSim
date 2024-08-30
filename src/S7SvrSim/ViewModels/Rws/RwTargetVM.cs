using Microsoft.Win32;
using ReactiveUI.Fody.Helpers;
using S7SvrSim.Services;
using System;
using System.Reactive.Linq;
using System.Windows;

namespace S7SvrSim.ViewModels.Rw
{
    public class RwTargetVM : ReactiveObject
    {
        private readonly PyScriptRunner _scriptRunner;

        public RwTargetVM(PyScriptRunner scriptRunner)
        {
            this._scriptRunner = scriptRunner;

            this.CmdRunScript = ReactiveCommand.Create(CmdRunScript_Impl);
        }

        /// <summary>
        /// 目标DB号
        /// </summary>
        [Reactive]
        public int TargetDBNumber { get; set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        [Reactive]
        public int TargetPos { get; set; }

        /// <summary>
        /// 运行脚本
        /// </summary>
        public ReactiveCommand<Unit, Unit> CmdRunScript { get; }
        public void CmdRunScript_Impl()
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                var result = fileDialog.ShowDialog();
                if (result != true)
                {
                    return;
                }

                Observable.Create<Unit>(observer =>
                    {
                        var filename = fileDialog.FileName;
                        this._scriptRunner.RunFile(filename);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    })
                    .SubscribeOn(RxApp.TaskpoolScheduler)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        e => {
                            MessageBox.Show("脚本执行完成！");
                        }, 
                        err => {
                            MessageBox.Show(err.Message);
                        }
                    );

            }
            catch (Exception ex)
            {
                MessageBox.Show($"执行脚本出错！{ex.Message}");
            }
        }
    }
}
