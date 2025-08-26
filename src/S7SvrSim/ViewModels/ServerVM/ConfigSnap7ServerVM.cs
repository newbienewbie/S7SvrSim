using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace S7Svr.Simulator.ViewModels
{
    /// <summary>
    /// S7 Server 的配置
    /// </summary>
    public partial class ConfigSnap7ServerVM : ReactiveObject
    {
        protected bool registCommand = true;

        /// <summary>
        /// IP Address
        /// </summary>
        [Reactive]
        public string IpAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// DB Configs
        /// </summary>
        public virtual ObservableCollection<AreaConfigVM> AreaConfigs { get; } = new ObservableCollection<AreaConfigVM>();

        public ReactiveCommand<Unit, Unit> CmdAddArea { get; }
        public ReactiveCommand<AreaConfigVM, Unit> CmdRemoveArea { get; }

        public ConfigSnap7ServerVM()
        {
            this.CmdAddArea = ReactiveCommand.Create<Unit>(_ => AreaConfigs.Add(new AreaConfigVM()));
            this.CmdRemoveArea = ReactiveCommand.Create<AreaConfigVM>(area => AreaConfigs.Remove(area));
        }

        internal void SetIpAddress(object value)
        {
            IpAddress = (string)value;
            this.RaisePropertyChanged(nameof(IpAddress));
        }
    }
}
