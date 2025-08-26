using CommunityToolkit.Mvvm.ComponentModel;
using S7SvrSim.ViewModels;
using System.Collections.ObjectModel;

namespace S7Svr.Simulator.ViewModels
{
    /// <summary>
    /// S7 Server 的配置
    /// </summary>
    public partial class ConfigSnap7ServerVM : ViewModelBase
    {
        protected bool registCommand = true;

        /// <summary>
        /// IP Address
        /// </summary>
        [ObservableProperty]
        private string ipAddress = "127.0.0.1";

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
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            ipAddress = (string)value;
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            OnPropertyChanged(nameof(IpAddress));
        }
    }
}
