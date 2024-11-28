using MediatR;
using Microsoft.Win32;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator.Messages;
using S7SvrSim.Services;
using S7SvrSim.UserControls;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Unit = System.Reactive.Unit;

namespace S7SvrSim.ViewModels.Rw
{
    public class RwTargetVM : ReactiveObject
    {
        private readonly PyScriptRunner _scriptRunner;
        private readonly IMediator _mediator;

        public RwTargetVM(PyScriptRunner scriptRunner,IMediator mediator)
        {
            this._scriptRunner = scriptRunner;
            _mediator = mediator;
            this.CmdRunScript = ReactiveCommand.Create(CmdRunScript_Impl);
            this.CmdTaskList = ReactiveCommand.Create(() =>
            {
                var win = new ScriptTaskWindow ();
                win.ViewModel = TaskViewModle;
                win.ShowDialog();
            });
        }
        public ScriptTaskWindowVM TaskViewModle = Locator.Current.GetRequiredService<ScriptTaskWindowVM>();
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
        /// <summary>
        /// 任务管理
        /// </summary>
        public ReactiveCommand<Unit, Unit> CmdTaskList { get; }
        public async void CmdRunScript_Impl()
        {
            try
            {
                var fileDialog = new OpenFileDialog();
                var result = fileDialog.ShowDialog();
                if (result != true)
                {
                    return;
                }
                var filename = fileDialog.FileName;
                var tokensorce = new CancellationTokenSource();
                var tasktoken = tokensorce.Token;
               
                var a= Observable.Create<Unit>(observer =>
                    {
                        this._scriptRunner.RunFile(filename, tasktoken);
                        observer.OnCompleted();
                        return Disposable.Empty;
                    }).SubscribeOn(RxApp.TaskpoolScheduler)
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(
                        e =>
                        {
                            MessageBox.Show("脚本执行完成！");
                        },
                        err =>
                        {
                            MessageBox.Show(err.Message);
                        }
                 );
                await _mediator.Publish(new MessageScriptTaskNotification() { FilePath = filename, TaskDisposable = tokensorce });

            }
            catch (Exception ex)
            {
                MessageBox.Show($"执行脚本出错！{ex.Message}");
            }
        }
    }
}
