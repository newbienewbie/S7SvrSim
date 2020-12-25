using FutureTech.Mvvm;
using FutureTech.Snap7;
using MediatR;
using Microsoft.Extensions.Logging;
using S7Svr.Simulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S7Svr.Simulator.ViewModels
{
    public class S7ServerService 
    {

        private readonly ILogger<S7ServerService> _logger;
        protected virtual IMediator _mediator { get; set; }
        protected virtual FutureTech.Snap7.S7Server S7Server { get; set; } = new FutureTech.Snap7.S7Server();

        public S7ServerService(IMediator mediator, ILogger<S7ServerService> logger)
        {
            this._mediator = mediator;
            this._logger = logger;
        }


        public async Task StartServerAsync(RunningSnap7ServerVM vm)
        {
            try
            {
                if (vm.DBConfigs == null)
                {
                    var msg = new MessageNotification() { Message = "当前 DBConfigs 为 NULL !" };
                    await this._mediator.Publish(msg);
                    return;
                }

                vm.RunningsItems.Clear();

                foreach (var db in vm.DBConfigs)
                {
                    var buffer = new byte[db.DBSize];
                    vm.RunningsItems.Add(new RunningSnap7ServerVM.RunningServerItem
                    {
                        DBNumber = db.DBNumber,
                        DBSize = db.DBSize,
                        Bytes  = buffer,
                    });
                    this.S7Server.RegisterArea(FutureTech.Snap7.S7Server.srvAreaDB, db.DBNumber, ref buffer, db.DBSize);
                }
                this.S7Server.StartTo(vm.IpAddress);
            }
            catch (Exception ex)
            {
                var msg = $"启动服务器出错：{ex.Message}";
                this._logger.LogError(msg);
                await this._mediator.Publish(new MessageNotification
                {
                    Message = msg
                });
            }
        }

        public async Task StopServerAsync() 
        {
            try
            {
                this.S7Server.Stop();
            }
            catch (Exception ex)
            {
                var msg = $"停止服务器出错：{ex.Message}";
                this._logger.LogError(msg);
                await this._mediator.Publish(new MessageNotification { Message = msg });
            }
        }

        #region Byte
        public byte ReadByte(RunningSnap7ServerVM vm, int dbNumber, int pos)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！" });
                return default;
            }

            var buffer = config.Bytes;

            var val = S7.GetByteAt(buffer, pos);
            return val;
        }


        public void WriteByte(RunningSnap7ServerVM vm, int dbNumber, int pos, byte value)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！" });
                return;
            }
            var buffer = config.Bytes;

            S7.SetByteAt(buffer, pos, value);
        }
        #endregion

        #region Short
        public short ReadShort(RunningSnap7ServerVM vm, int dbNumber, int pos)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！" });
                return default;
            }

            var buffer = config.Bytes;

            var val = S7.GetIntAt(buffer, pos);
            return (short) val;
        }


        public void WriteShort(RunningSnap7ServerVM vm, int dbNumber, int pos, short value)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！" });
                return;
            }
            var buffer = config.Bytes;

            S7.SetIntAt(buffer, pos, value);
        }
        #endregion


        #region Bit
        public bool ReadBit(RunningSnap7ServerVM vm, int dbNumber, int offset, byte bit)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！"});
                return default;
            }

            var buffer = config.Bytes;

            var x = S7.GetBitAt(buffer, offset, bit);
            return x;
        }


        public void WriteBit(RunningSnap7ServerVM vm, int dbNumber, int offset, byte bit, bool flag)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！"});
                return;
            }
            var buffer = config.Bytes;

            S7.SetBitAt(ref buffer, offset, bit, flag);
        }
        #endregion

        #region String
        public void WriteString(RunningSnap7ServerVM vm, int dbNumber, int offset, string str)
        { 
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！"});
                return;
            }
            var buffer = config.Bytes;
            S7.SetStringAt(buffer, offset, 256, str);
        }

        public string ReadString(RunningSnap7ServerVM vm, int dbNumber, int offset)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！" });
                return String.Empty;
            }
            var buffer = config.Bytes;
            var str = S7.GetStringAt(buffer, offset);
            return str;
        }
        #endregion

        #region Real
        public void WriteReal(RunningSnap7ServerVM vm, int dbNumber, int pos, float real)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！" });
                return;
            }
            var buffer = config.Bytes;
            S7.SetRealAt(buffer, pos, real);
        }

        public float ReadReal(RunningSnap7ServerVM vm, int dbNumber, int pos)
        {
            var config = vm.RunningsItems.Where(i => i.DBNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"DBNumber={dbNumber} 不存在！" });
                return default;
            }
            var buffer = config.Bytes;
            var real = S7.GetRealAt(buffer, pos);
            return real;
        }
        #endregion

    }

}
