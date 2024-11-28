using FutureTech.Snap7;
using MediatR;
using Microsoft.Extensions.Logging;
using S7Svr.Simulator.Messages;
using S7SvrSim.ViewModels;
using Splat;
using System.Linq;

namespace S7Svr.Simulator.ViewModels
{
    public class S7MBService : IS7MBService
    {
        private readonly RunningSnap7ServerVM _runningVM;
        private readonly MsgLoggerVM _loggerVM;
        private readonly ILogger<S7DataBlockService> _logger;
        protected virtual IMediator _mediator { get; set; }
        protected virtual FutureTech.Snap7.S7Server S7Server { get; set; }

        public S7MBService(IMediator mediator, ILogger<S7DataBlockService> logger)
        {
            this._mediator = mediator;
            this._runningVM = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            this._loggerVM = Locator.Current.GetRequiredService<MsgLoggerVM>();
            this._logger = logger;
        }


        public byte[] GetBuffer()
        {
            var config = _runningVM.RunningsItems.FirstOrDefault(i => i.AreaKind == AreaKind.MB);
            if (config == null)
            {
                this._mediator.Publish(new MessageNotification { Message = $"AreaKind.MB 不存在！" });
                return default;
            }
            var buffer = config.Bytes;
            return buffer;
        }

        #region Byte
        public byte ReadByte(int pos)
        {
            var buffer = GetBuffer();
            var val = S7.GetByteAt(buffer, pos);
            return val;
        }


        public void WriteByte(int pos, byte value)
        {
            var buffer = GetBuffer();
            S7.SetByteAt(buffer, pos, value);
        }
        #endregion

        #region Short
        public short ReadShort(int pos)
        {
            var buffer = GetBuffer();

            var val = S7.GetIntAt(buffer, pos);
            return (short)val;
        }


        public void WriteShort( int pos, short value)
        {
            var buffer = GetBuffer();

            S7.SetIntAt(buffer, pos, value);
        }
        #endregion


        #region Bit
        public bool ReadBit(int offset, byte bit)
        {
            var buffer = GetBuffer();

            var x = S7.GetBitAt(buffer, offset, bit);
            return x;
        }


        public void WriteBit(int offset, byte bit, bool flag)
        {
            var buffer = GetBuffer();

            S7.SetBitAt(ref buffer, offset, bit, flag);
        }
        #endregion

        #region String
        public void WriteString(int offset, int maxlen, string str)
        {
            var buffer = GetBuffer();
            S7.SetStringAt(buffer, offset, maxlen, str);
        }

        public string ReadString(int offset)
        {
            var buffer = GetBuffer();
            var str = S7.GetStringAt(buffer, offset);
            return str;
        }
        #endregion

        #region Real
        public void WriteReal(int pos, float real)
        {
            var buffer = GetBuffer();
            S7.SetRealAt(buffer, pos, real);
        }

        public float ReadReal(int pos)
        {
            var buffer = GetBuffer();
            var real = S7.GetRealAt(buffer, pos);
            return real;
        }

        #endregion

        #region ulong
        public ulong ReadULong(int pos)
        {
            var buffer = GetBuffer();

            var val = S7.GetULIntAt(buffer, pos);
            return val;
        }


        public void WriteULong(int pos, ulong value)
        {
            var buffer = GetBuffer();

            S7.SetULintAt(buffer, pos, value);
        }
        #endregion

        #region uint32
        public uint ReadUInt32( int pos)
        {
            var buffer = GetBuffer();

            var val = S7.GetUDIntAt(buffer, pos);
            return val;
        }

        public void WriteUInt32( int pos, uint value)
        {
            var buffer = GetBuffer();

            S7.SetUDIntAt(buffer, pos, value);
        }
        #endregion
    }

}
