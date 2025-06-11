using ReactiveUI.Fody.Helpers;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using System;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;

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

        public ReactiveCommand<Unit, Unit> CmdAddArea { get; }
        public ReactiveCommand<AreaConfigVM, Unit> CmdRemoveArea { get; }

        public ConfigSnap7ServerVM()
        {
            this.CmdAddArea = ReactiveCommand.Create<Unit>(_ =>
            {
                var command = new CollectionChangedCommand<AreaConfigVM>(AreaConfigs, ChangedType.Add, new AreaConfigVM());
                CommandEventRegist(command);
                UndoRedoManager.Run(command);
            });
            this.CmdRemoveArea = ReactiveCommand.Create<AreaConfigVM>(area =>
            {
                var command = new CollectionChangedCommand<AreaConfigVM>(AreaConfigs, ChangedType.Remove, area);
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
    }
}
