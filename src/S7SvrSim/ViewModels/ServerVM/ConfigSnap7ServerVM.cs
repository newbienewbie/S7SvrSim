using MediatR;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator.Messages;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    /// <summary>
    /// S7 Server 的配置
    /// </summary>
    public class ConfigSnap7ServerVM : ReactiveObject
    {
        /// <summary>
        /// IP Address
        /// </summary>
        [Reactive]
        public virtual string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// DB Configs
        /// </summary>
        public virtual ObservableCollection<AreaConfigVM> AreaConfigs { get; } = new ObservableCollection<AreaConfigVM>();
    }


}
