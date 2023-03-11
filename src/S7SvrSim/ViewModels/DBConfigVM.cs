using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace S7Svr.Simulator.ViewModels
{
    public struct DBConfig
    { 
        public int DBNumber { get; set; }
        public int DBSize { get; set; }
    }


    public class DBConfigVM :IEditableObject, INotifyPropertyChanged
    {
        private DBConfig _currentConfig = new DBConfig();
        private DBConfig _bakeup = default;

        public DBConfigVM()
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

        public int DBNumber { 
            get => this._currentConfig.DBNumber;
            set
            {
                if (this._currentConfig.DBNumber != value)
                { 
                    this._currentConfig.DBNumber = value;
                    this.OnPropertyChanged(nameof(DBNumber));
                }
            }
        }
        public int DBSize { 
            get => this._currentConfig.DBSize;
            set
            {
                if (this._currentConfig.DBSize!= value)
                { 
                    this._currentConfig.DBSize = value;
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
