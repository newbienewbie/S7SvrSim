using FutureTech.Mvvm;
using MediatR;
using S7Svr.Simulator.Messages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{

    public class ConfigSnap7ServerVM : ViewModelBase
    {
        private string _ipAddress = "127.0.0.1";

        public ConfigSnap7ServerVM()
        {
            this.DBConfigs = new ObservableCollection<DBConfigVM>();
        }

        /// <summary>
        /// IP Address
        /// </summary>
        public virtual string IpAddress
        {
            get => _ipAddress;
            set
            {
                if (this._ipAddress != value)
                {
                    this._ipAddress = value;
                    this.OnPropertyChanged(nameof(IpAddress));
                }
            }
        }

        /// <summary>
        /// DB Configs
        /// </summary>
        public virtual ObservableCollection<DBConfigVM> DBConfigs { get; } 
    }


    public class RunningSnap7ServerVM : ConfigSnap7ServerVM
    {
        public class RunningServerItem : ViewModelBase
        {
            private int dBNumber;
            private int _dBSize;
            private byte[] _bytes;

            public int DBNumber { 
                get => dBNumber;
                set
                {
                    if (this.dBNumber != value)
                    { 
                        dBNumber = value;
                        this.OnPropertyChanged(nameof(DBNumber));
                    }
                }
            }

            public int DBSize { 
                get => _dBSize;
                set
                {
                    if (this._dBSize != value)
                    { 
                        _dBSize = value;
                        this.OnPropertyChanged(nameof(DBSize));
                    }
                }
            }

            public byte[] Bytes { 
                get => _bytes;
                set
                {
                    _bytes = value;
                    this.OnPropertyChanged(nameof(Bytes));
                }
            }
        }

        #region 运行状态
        private bool _runningStatus = false;
        public bool RunningStatus
        {
            get => _runningStatus;
            set
            {
                if (this._runningStatus != value)
                {
                    this._runningStatus = value;
                    this.OnPropertyChanged(nameof(RunningStatus));
                    this.OnPropertyChanged(nameof(IsNotRunning));
                }
            }
        }

        public bool IsNotRunning => !_runningStatus;
        #endregion


        /// <summary>
        /// 正在运行的
        /// </summary>
        public ObservableCollection<RunningServerItem> RunningsItems { get; } = new ObservableCollection<RunningServerItem>();
    }
}
