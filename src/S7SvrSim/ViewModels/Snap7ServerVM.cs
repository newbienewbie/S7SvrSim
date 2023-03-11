using MediatR;
using Reactive.Bindings;
using S7Svr.Simulator.Messages;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{

    public class ConfigSnap7ServerVM 
    {
        private string _ipAddress = "127.0.0.1";

        public ConfigSnap7ServerVM()
        {
            this.DBConfigs = new ObservableCollection<DBConfigVM>();

            this.IpAddress = new ReactiveProperty<string>(_ipAddress);
        }

        /// <summary>
        /// IP Address
        /// </summary>
        public virtual ReactiveProperty<string> IpAddress { get; }

        /// <summary>
        /// DB Configs
        /// </summary>
        public virtual ObservableCollection<DBConfigVM> DBConfigs { get; } 
    }


    public class RunningServerItem
    {
        public int DBNumber { get; set; }

        public int DBSize { get; set; }

        public byte[] Bytes { get; set; }
    }

    public class RunningSnap7ServerVM : ConfigSnap7ServerVM
    {
        #region 运行状态
        public RunningSnap7ServerVM()
        {
            this.RunningStatus = new ReactiveProperty<bool>(false);
        }

        public ReactiveProperty<bool> RunningStatus { get; }

        #endregion


        /// <summary>
        /// 正在运行的——一旦开始，就不在变化。停止后Clear、再重建
        /// </summary>
        public IList<RunningServerItem> RunningsItems { get; } = new List<RunningServerItem>();
    }
}
