using Microsoft.Win32;
using Reactive.Bindings;
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
    public class OperationVM 
    {
        private IS7DataBlockService _s7ServerService;
        private readonly PyScriptRunner _scriptRunner;

        public OperationVM(IS7DataBlockService s7ServerService, PyScriptRunner scriptRunner)
        {
            this._s7ServerService = s7ServerService;
            this._scriptRunner = scriptRunner;

            this.CmdRunScript = new ReactiveCommand().WithSubscribe(
                o => {
                    try
                    {
                        var fileDialog = new OpenFileDialog();
                        var result = fileDialog.ShowDialog();
                        if (result == true)
                        {
                            var filename = fileDialog.FileName;
                            this._scriptRunner.RunFile(filename);
                            MessageBox.Show("脚本执行完成！");
                        }
                    }
                    catch(Exception ex) {
                        MessageBox.Show($"执行脚本出错！{ex.Message}");
                    }
                }
            );


            #region Byte
            this.CmdWriteByte = new ReactiveCommand().WithSubscribe(
                o =>
                {
                    this._s7ServerService.WriteByte(this.TargetDBNumber.Value, this.TargetPos.Value, this.ByteToBeWritten.Value);
                }
            );
            this.CmdReadByte = new ReactiveCommand().WithSubscribe(
                o => {
                    var val = this._s7ServerService.ReadByte(this.TargetDBNumber.Value, this.TargetPos.Value);
                    this.ByteRead.Value = val;
                }
            );
            #endregion

            #region Short
            this.CmdWriteShort = new ReactiveCommand().WithSubscribe(
                o =>
                {
                    this._s7ServerService.WriteShort(this.TargetDBNumber.Value, this.TargetPos.Value, this.ShortToBeWritten.Value);
                }
            );
            this.CmdReadShort = new ReactiveCommand().WithSubscribe(
                o => {
                    var val = this._s7ServerService.ReadShort(this.TargetDBNumber.Value, this.TargetPos.Value);
                    this.ShortRead.Value = val;
                }
            );
            #endregion

            #region Bit
            this.CmdWriteBit = new ReactiveCommand().WithSubscribe(
                o =>
                {
                    if (this.TargetBitPos.Value > 7 || this.TargetBitPos.Value < 0)
                    {
                        MessageBox.Show($"位数的取值必须落于范围[0,7]之间，当前={this.TargetBitPos}");
                        return;
                    }
                    this._s7ServerService.WriteBit(this.TargetDBNumber.Value, this.TargetPos.Value, this.TargetBitPos.Value, this.BitToBeWritten.Value);
                }
            );
            this.CmdReadBit = new ReactiveCommand().WithSubscribe(
                o => {
                    if (this.TargetBitPos.Value > 7 || this.TargetBitPos.Value < 0)
                    {
                        MessageBox.Show($"位数的取值必须落于范围[0,7]之间，当前={this.TargetBitPos}");
                        return;
                    }
                    var val = this._s7ServerService.ReadBit(this.TargetDBNumber.Value,this.TargetPos.Value, this.TargetBitPos.Value);
                    this.BitRead.Value = val;
                }
            );
            #endregion

            #region String
            this.CmdWriteString = new ReactiveCommand().WithSubscribe(
                o =>
                {
                    this._s7ServerService.WriteString(this.TargetDBNumber.Value, this.TargetPos.Value, this.StringArrayMaxLength.Value, this.StrToBeWritten.Value);
                }
            );
            this.CmdReadString = new ReactiveCommand().WithSubscribe(
                o => {
                    var str = this._s7ServerService.ReadString(this.TargetDBNumber.Value, this.TargetPos.Value);
                    this.StrRead.Value = str;
                }
            );
            #endregion

            #region Real
            this.CmdWriteReal = new ReactiveCommand().WithSubscribe(
                o =>
                {
                    this._s7ServerService.WriteReal(this.TargetDBNumber.Value, this.TargetPos.Value, this.RealToBeWritten.Value);
                }
            );
            this.CmdReadReal = new ReactiveCommand().WithSubscribe(
                o => {
                    var real = this._s7ServerService.ReadReal(this.TargetDBNumber.Value, this.TargetPos.Value);
                    this.RealRead.Value = real;
                }
            );
            #endregion

            #region UInt
            this.UIntRead = new ReactiveProperty<uint>();
            this.UIntToBeWritten = new ReactiveProperty<uint>();

            this.CmdReadUInt = new ReactiveCommand();
            this.CmdReadUInt.Subscribe(i => {
                var s = this._s7ServerService.ReadUInt32(this.TargetDBNumber.Value, this.TargetPos.Value);
                this.UIntRead.Value = s;
            });

            this.CmdWriteUInt = new ReactiveCommand();
            this.CmdWriteUInt.Subscribe(i => {
                this._s7ServerService.WriteUInt32(this.TargetDBNumber.Value, this.TargetPos.Value, this.UIntToBeWritten.Value);
            });
            #endregion

            #region ULong
            this.ULongRead = new ReactiveProperty<ulong>();
            this.ULongToBeWritten = new ReactiveProperty<ulong>();

            this.CmdReadULong = new ReactiveCommand();
            this.CmdReadULong.Subscribe(i => {
                var s = this._s7ServerService.ReadULong(this.TargetDBNumber.Value, this.TargetPos.Value);
                this.ULongRead.Value = s;
            });

            this.CmdWriteULong= new ReactiveCommand();
            this.CmdWriteULong.Subscribe(i => {
                this._s7ServerService.WriteULong(this.TargetDBNumber.Value, this.TargetPos.Value, this.ULongToBeWritten.Value);
            });
            #endregion
        }

        public ReactiveProperty<int> TargetDBNumber { get; } = new();
        public ReactiveProperty<int> TargetPos { get; } = new();
        public ICommand CmdRunScript { get; }

        #region Short Read And Write
        public ReactiveProperty<short> ShortToBeWritten { get; } = new();

        public ReactiveProperty<short> ShortRead { get; } = new();

        public ICommand CmdWriteShort{ get; }
        public ICommand CmdReadShort{ get; }
        #endregion



        #region Byte Read And Write
        public ReactiveProperty<byte> ByteToBeWritten { get; } = new();

        public ReactiveProperty<byte> ByteRead { get; } = new();

        public ICommand CmdWriteByte{ get; }
        public ICommand CmdReadByte{ get; }
        #endregion


        #region Bit Read And Write



        /// <summary>
        /// 标识字节的第N位：取值 0 -7
        /// </summary>
        public ReactiveProperty<byte> TargetBitPos { get; } = new();

        public ReactiveProperty<bool> BitToBeWritten { get; } = new();

        public ReactiveProperty<bool> BitRead { get; } = new();
        public ICommand CmdWriteBit{ get; }
        public ICommand CmdReadBit{ get; }
        #endregion


        #region String Read And Write
        public ReactiveProperty<string> StrToBeWritten { get; } = new ();

        public ReactiveProperty<string> StrRead { get; } = new();

        public ReactiveProperty<int> StringArrayMaxLength { get; } = new(256);
        public ICommand CmdWriteString { get; }
        public ICommand CmdReadString { get; }
        #endregion


        #region Real Read And Write
        public ReactiveProperty<float> RealToBeWritten { get; } = new();

        public ReactiveProperty<float> RealRead { get; } = new();

        public ICommand CmdWriteReal{ get; }
        public ICommand CmdReadReal{ get; }
        #endregion

        #region UInt
        public ReactiveProperty<uint> UIntToBeWritten { get; } = new();
        public ReactiveProperty<uint> UIntRead { get; } = new();
        public ReactiveCommand CmdReadUInt { get; }
        public ReactiveCommand CmdWriteUInt { get; }
        #endregion

        #region ULong
        public ReactiveProperty<ulong> ULongToBeWritten { get; } = new ();
        public ReactiveProperty<ulong> ULongRead { get; }= new ();
        public ReactiveCommand CmdReadULong{ get; }
        public ReactiveCommand CmdWriteULong{ get; }
        #endregion
    }
}
