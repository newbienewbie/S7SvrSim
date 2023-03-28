using Microsoft.Extensions.Logging;
using Reactive.Bindings;
using S7Svr.Simulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

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


        public void AddLogMsg(LogMessage s)
        {
            try
            {
                var logcount = this.Logs.Count;
                if (logcount >= LOGS_MAX)
                {
                    this.Logs.RemoveAt(0);
                }
                this.Logs.Add(s);
                this.IsLogsChanged = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void LogInfo(string content)
        {
            var msg = new LogMessage(DateTime.Now, LogLevel.Information, content);
            this.AddLogMsg(msg);
        }

        public void LogError(string content)
        {
            var msg = new LogMessage(DateTime.Now, LogLevel.Error, content);
            this.AddLogMsg(msg);
        }

        public MsgLoggerVM()
        {
        }
    }
}
