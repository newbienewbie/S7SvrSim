using ReactiveUI.Fody.Helpers;
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
    public class AreaConfig : ReactiveObject
    {
        [Reactive]
        public AreaKind AreaKind { get; set; }

        [Reactive]
        public int BlockNumber { get; set; }

        [Reactive]
        public int BlockSize { get; set; }
    }


    public class AreaConfigVM :IEditableObject, INotifyPropertyChanged
    {
        private AreaConfig _currentConfig = new AreaConfig();
        private AreaConfig _bakeup = default;

        public AreaConfigVM()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(propName)));
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
                    this.OnPropertyChanged(nameof(AreaKind));
                }
            }
        }

        public int DBNumber { 
            get => this._currentConfig.BlockNumber;
            set
            {
                if (this._currentConfig.BlockNumber != value)
                { 
                    this._currentConfig.BlockNumber = value;
                    this.OnPropertyChanged(nameof(DBNumber));
                }
            }
        }
        public int DBSize { 
            get => this._currentConfig.BlockSize;
            set
            {
                if (this._currentConfig.BlockSize!= value)
                { 
                    this._currentConfig.BlockSize = value;
                    this.OnPropertyChanged(nameof(DBNumber));
                }
            }
        }


        public void BeginEdit()
        {
            this._bakeup = _currentConfig;
        }

        public void CancelEdit()
        {
            this._currentConfig = this._bakeup;
        }

        public void EndEdit()
        {
            this._bakeup = default;
        }
    }
}
