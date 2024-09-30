using Microsoft.Extensions.Logging;
using S7Svr.Simulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace S7SvrSim.ViewModels
{
    public class MsgLoggerVM : INotifyPropertyChanged
    {
        #region 日志消息
        /// <summary>
        /// 屏幕上的最大日志数量
        /// </summary>
        private const int LOGS_MAX = 800;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<LogMessage> Logs { get; } = new ObservableCollection<LogMessage>();

        public bool _isLogsChanged;
        public bool IsLogsChanged { 
            get { return _isLogsChanged; } 
            set { 
                this._isLogsChanged = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsLogsChanged)));
            } 
        }
        #endregion


        public IDisposable AddLogMsg(LogMessage s)
        {
            var obs = Observable.Create<Unit>((observer) => {
                var logcount = this.Logs.Count;
                if (logcount >= LOGS_MAX)
                {
                    this.Logs.RemoveAt(0);
                }
                this.Logs.Add(s);
                this.IsLogsChanged = true;
                observer.OnCompleted();
                return Disposable.Empty;
            });

            return obs.SubscribeOn(RxApp.MainThreadScheduler)
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Subscribe(
                        e => {
                            MessageBox.Show("脚本执行完成！");
                        },
                        err => {
                            MessageBox.Show(err.Message);
                        }
                );
        }

        public IDisposable LogInfo(string content)
        {
            var msg = new LogMessage(DateTime.Now, LogLevel.Information, content);
            return this.AddLogMsg(msg);
        }

        public IDisposable LogError(string content)
        {
            var msg = new LogMessage(DateTime.Now, LogLevel.Error, content);
            return this.AddLogMsg(msg);
        }

        public MsgLoggerVM()
        {
        }
    }
}
