using CommunityToolkit.Mvvm.ComponentModel;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.ViewModels;
using System;
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
            this.CmdAddArea = ReactiveCommand.Create<Unit>(_ =>
            {
                var command = ListChangedCommand<AreaConfigVM>.Add(AreaConfigs, [new AreaConfigVM()]);
                CommandEventRegist(command);
                UndoRedoManager.Run(command);
            });
            this.CmdRemoveArea = ReactiveCommand.Create<AreaConfigVM>(area =>
            {
                var command = ListChangedCommand<AreaConfigVM>.Remove(AreaConfigs, [area]);
                CommandEventRegist(command);
                UndoRedoManager.Run(command);
            });
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(0);
        }

        internal void CommandEventRegist(ICommand command)
        {
            command.AfterExecute += CommandEventHandle;
            command.AfterUndo += CommandEventHandle;
        }

        internal void SetIpAddress(object value)
        {
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            ipAddress = (string)value;
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            OnPropertyChanged(nameof(IpAddress));
        }

        partial void OnIpAddressChanged(string oldValue, string newValue)
        {
            if (registCommand)
            {
                var command = new ValueChangedCommand(SetIpAddress, oldValue, newValue);
                CommandEventRegist(command);
                UndoRedoManager.Regist(command);
            }
        }
    }
}
