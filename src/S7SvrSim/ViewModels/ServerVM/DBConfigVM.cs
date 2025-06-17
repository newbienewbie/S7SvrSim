using ReactiveUI.Fody.Helpers;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace S7Svr.Simulator.ViewModels
{
    /// <summary>
    /// 区域种类
    /// </summary>
    public enum AreaKind
    {
        DB = 0,
        MB = 1,
    }

    /// <summary>
    /// 区域配置
    /// </summary>
    public class AreaConfig : ReactiveObject, ICloneable
    {
        [Reactive]
        public AreaKind AreaKind { get; set; }

        [Reactive]
        public int BlockNumber { get; set; }

        [Reactive]
        public int BlockSize { get; set; }

        public object Clone()
        {
            return new AreaConfig()
            {
                AreaKind = this.AreaKind,
                BlockNumber = this.BlockNumber,
                BlockSize = this.BlockSize
            };
        }
    }


    public class AreaConfigVM : IEditableObject, INotifyPropertyChanged
    {
        private AreaConfig _currentConfig = new AreaConfig();
        private AreaConfig _bakeup = default;

        public AreaConfigVM()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public AreaKind AreaKind
        {
            get => this._currentConfig.AreaKind;
            set
            {
                if (this._currentConfig.AreaKind != value)
                {
                    this._currentConfig.AreaKind = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public int DBNumber
        {
            get => this._currentConfig.BlockNumber;
            set
            {
                if (this._currentConfig.BlockNumber != value)
                {
                    this._currentConfig.BlockNumber = value;
                    this.OnPropertyChanged();
                }
            }
        }
        public int DBSize
        {
            get => this._currentConfig.BlockSize;
            set
            {
                if (this._currentConfig.BlockSize != value)
                {
                    this._currentConfig.BlockSize = value;
                    this.OnPropertyChanged();
                }
            }
        }


        public void BeginEdit()
        {
            this._bakeup = new AreaConfig()
            {
                AreaKind = _currentConfig.AreaKind,
                BlockNumber = _currentConfig.BlockNumber,
                BlockSize = _currentConfig.BlockSize
            };
        }

        public void CancelEdit()
        {
            this._currentConfig = this._bakeup;
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(0);
        }

        public void EndEdit()
        {
            if (this._bakeup != null && (_bakeup.AreaKind != _currentConfig.AreaKind || _bakeup.BlockNumber != _currentConfig.BlockNumber || _bakeup.BlockSize != _currentConfig.BlockSize))
            {
                var command = new ValueChangedCommand<AreaConfig>(config =>
                {
                    this.AreaKind = config.AreaKind;
                    this.DBNumber = config.BlockNumber;
                    this.DBSize = config.BlockSize;
                }, (AreaConfig)this._bakeup.Clone(), (AreaConfig)this._currentConfig.Clone());
                command.AfterExecute += CommandEventHandle;
                command.AfterUndo += CommandEventHandle;
                UndoRedoManager.Regist(command);
            }

            this._bakeup = default;
        }
    }
}
