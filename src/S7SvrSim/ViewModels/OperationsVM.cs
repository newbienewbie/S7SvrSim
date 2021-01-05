using FutureTech.Mvvm;
using Microsoft.Win32;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace S7Server.Simulator.ViewModels
{
    public class OperationVM : ViewModelBase
    {
        private S7ServerService _s7ServerService;
        private readonly PyScriptRunner _scriptRunner;

        public OperationVM(S7ServerService s7ServerService, PyScriptRunner scriptRunner)
        {
            this._s7ServerService = s7ServerService;
            this._scriptRunner = scriptRunner;

            this.CmdRunScript = new AsyncRelayCommand<object>(
                o => {
                    try
                    {
                        var fileDialog = new OpenFileDialog();
                        var result = fileDialog.ShowDialog();
                        if (result == true)
                        {
                            var filename = fileDialog.FileName;
                            this._scriptRunner.RunFile(filename);
                        }
                        MessageBox.Show("脚本执行完成！");
                    }
                    catch(Exception ex) {
                        MessageBox.Show($"执行脚本出错！{ex.Message}");
                    }
                    return Task.CompletedTask;
                },
                o => true
            );


            #region Byte
            this.CmdWriteByte = new AsyncRelayCommand<object>(
                o =>
                {
                    this._s7ServerService.WriteByte(this.TargetDBNumber, this.TargetPos, this.ByteToBeWritten);
                    return Task.CompletedTask;
                },
                o => true
            );
            this.CmdReadByte = new AsyncRelayCommand<object>(
                o => {
                    var val = this._s7ServerService.ReadByte(this.TargetDBNumber, this.TargetPos);
                    this.ByteRead= val;
                    return Task.CompletedTask;
                },
                o => true
            );
            #endregion

            #region Short
            this.CmdWriteShort = new AsyncRelayCommand<object>(
                o =>
                {
                    this._s7ServerService.WriteShort(this.TargetDBNumber, this.TargetPos, this.ShortToBeWritten);
                    return Task.CompletedTask;
                },
                o => true
            );
            this.CmdReadShort = new AsyncRelayCommand<object>(
                o => {
                    var val = this._s7ServerService.ReadShort(this.TargetDBNumber, this.TargetPos);
                    this.ShortRead = val;
                    return Task.CompletedTask;
                },
                o => true
            );
            #endregion

            #region Bit
            this.CmdWriteBit = new AsyncRelayCommand<object>(
                o =>
                {
                    this._s7ServerService.WriteBit(this.TargetDBNumber, this.TargetPos, this.TargetBitPos, this.BitToBeWritten);
                    return Task.CompletedTask;
                },
                o => true
            );
            this.CmdReadBit = new AsyncRelayCommand<object>(
                o => {
                    var val = this._s7ServerService.ReadBit(this.TargetDBNumber, this.TargetPos, this.TargetBitPos);
                    this.BitRead= val;
                    return Task.CompletedTask;
                },
                o => true
            );
            #endregion

            #region String
            this.CmdWriteString = new AsyncRelayCommand<object>(
                o =>
                {
                    this._s7ServerService.WriteString(this.TargetDBNumber, this.TargetPos, this.StrToBeWritten);
                    return Task.CompletedTask;
                },
                o => true
            );
            this.CmdReadString = new AsyncRelayCommand<object>(
                o => {
                    var str = this._s7ServerService.ReadString(this.TargetDBNumber, this.TargetPos);
                    this.StrRead = str;
                    return Task.CompletedTask;
                },
                o => true
            );
            #endregion

            #region Real
            this.CmdWriteReal = new AsyncRelayCommand<object>(
                o =>
                {
                    this._s7ServerService.WriteReal(this.TargetDBNumber, this.TargetPos, this.RealToBeWritten);
                    return Task.CompletedTask;
                },
                o => true
            );
            this.CmdReadReal = new AsyncRelayCommand<object>(
                o => {
                    var real = this._s7ServerService.ReadReal(this.TargetDBNumber, this.TargetPos);
                    this.RealRead= real;
                    return Task.CompletedTask;
                },
                o => true
            );
            #endregion
        }

        private int _targetDBNumber;
        private int _targetPos;

        public int TargetDBNumber { 
            get => _targetDBNumber;
            set
            {
                if (_targetDBNumber != value)
                { 
                    _targetDBNumber = value;
                    this.OnPropertyChanged(nameof(TargetDBNumber));
                }
            }
        }

        public int TargetPos { 
            get => _targetPos;
            set
            {
                if (this._targetPos != value)
                { 
                    _targetPos = value;
                    this.OnPropertyChanged(nameof(TargetPos));
                }
            }
        }

        public ICommand CmdRunScript { get; }

        #region Short Read And Write
        private short _shortToBeWritten;
        public short ShortToBeWritten
        {
            get => _shortToBeWritten;
            set
            {
                if (this._shortToBeWritten != value)
                {
                    _shortToBeWritten = value;
                    this.OnPropertyChanged(nameof(ShortToBeWritten));
                }
            }
        }


        private short _shortRead;
        public short ShortRead
        {
            get => _shortRead;
            set
            {
                if (this._shortRead != value)
                {
                    this._shortRead = value;
                    this.OnPropertyChanged(nameof(ShortRead));
                }
            }
        }

        public ICommand CmdWriteShort{ get; }
        public ICommand CmdReadShort{ get; }
        #endregion



        #region Byte Read And Write
        private byte _byteToBeWritten;
        public byte ByteToBeWritten
        {
            get => _byteToBeWritten;
            set
            {
                if (this._byteToBeWritten != value)
                {
                    _byteToBeWritten = value;
                    this.OnPropertyChanged(nameof(ByteToBeWritten));
                }
            }
        }


        private byte _byteRead;
        public byte ByteRead
        {
            get => _byteRead;
            set
            {
                if (this._byteRead != value)
                {
                    this._byteRead = value;
                    this.OnPropertyChanged(nameof(ByteRead));
                }
            }
        }

        public ICommand CmdWriteByte{ get; }
        public ICommand CmdReadByte{ get; }
        #endregion


        #region Bit Read And Write


        private byte _targetBitPos;

        /// <summary>
        /// 标识字节的第N位：取值 0 -7
        /// </summary>
        public byte TargetBitPos
        {
            get => _targetBitPos;
            set
            {
                if (this._targetBitPos != value)
                {
                    _targetBitPos = value;
                    this.OnPropertyChanged(nameof(TargetBitPos));
                }
            }
        }

        private bool _bitToBeWritten;
        public bool BitToBeWritten
        {
            get => _bitToBeWritten;
            set
            {
                if (this._bitToBeWritten != value)
                {
                    _bitToBeWritten = value;
                    this.OnPropertyChanged(nameof(BitToBeWritten));
                }
            }
        }

        private bool _bitRead;
        public bool BitRead
        {
            get => _bitRead;
            set
            {
                if (this._bitRead != value)
                {
                    this._bitRead = value;
                    this.OnPropertyChanged(nameof(BitRead));
                }
            }
        }

        public ICommand CmdWriteBit{ get; }
        public ICommand CmdReadBit{ get; }
        #endregion


        #region String Read And Write
        private string _strToBeWritten;
        public string StrToBeWritten
        {
            get => _strToBeWritten;
            set
            {
                if (this._strToBeWritten != value)
                {
                    _strToBeWritten = value;
                    this.OnPropertyChanged(nameof(StrToBeWritten));
                }
            }
        }


        private string _strRead;
        public string StrRead { 
            get => _strRead;
            set {
                if (this._strRead != value)
                { 
                    this._strRead = value;
                    this.OnPropertyChanged(nameof(StrRead));
                }
            }
        }

        public ICommand CmdWriteString { get; }
        public ICommand CmdReadString { get; }
        #endregion


        #region Real Read And Write
        private float _realToBeWritten;
        public float RealToBeWritten
        {
            get => _realToBeWritten;
            set
            {
                if (this._realToBeWritten != value)
                {
                    _realToBeWritten = value;
                    this.OnPropertyChanged(nameof(RealToBeWritten));
                }
            }
        }


        private float _realRead;
        public float RealRead
        {
            get => _realRead;
            set
            {
                if (this._realRead != value)
                {
                    this._realRead = value;
                    this.OnPropertyChanged(nameof(RealRead));
                }
            }
        }

        public ICommand CmdWriteReal{ get; }
        public ICommand CmdReadReal{ get; }
        #endregion

    }
}
