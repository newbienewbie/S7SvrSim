using FutureTech.Snap7;
using MediatR;
using Microsoft.Extensions.Logging;
using S7Svr.Simulator.Messages;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Linq;

namespace S7Svr.Simulator.ViewModels
{
    public class S7DataBlockService : IS7DataBlockService
    {
        private readonly RunningSnap7ServerVM _runningVM;
        private readonly MsgLoggerVM _loggerVM;
        private readonly ILogger<IS7DataBlockService> _logger;
        protected virtual IMediator _mediator { get; set; }
        protected virtual FutureTech.Snap7.S7Server S7Server { get; set; }

        public S7DataBlockService(IMediator mediator, ILogger<IS7DataBlockService> logger)
        {
            this._mediator = mediator;
            this._runningVM = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            this._loggerVM = Locator.Current.GetRequiredService<MsgLoggerVM>();
            this._logger = logger;
        }

        #region Byte
        public byte ReadByte(int dbNumber, int pos)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }

            var buffer = config.Bytes;

            var val = S7.GetByteAt(buffer, pos);
            return val;
        }


        public void WriteByte(int dbNumber, int pos, byte value)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;

            S7.SetByteAt(buffer, pos, value);
        }
        #endregion

        #region Short
        public short ReadShort(int dbNumber, int pos)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }

            var buffer = config.Bytes;

            var val = S7.GetIntAt(buffer, pos);
            return (short)val;
        }


        public void WriteShort(int dbNumber, int pos, short value)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;

            S7.SetIntAt(buffer, pos, value);
        }
        #endregion

        #region Bit
        public bool ReadBit(int dbNumber, int offset, byte bit)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }

            var buffer = config.Bytes;

            var x = S7.GetBitAt(buffer, offset, bit);
            return x;
        }


        public void WriteBit(int dbNumber, int offset, byte bit, bool flag)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;

            S7.SetBitAt(ref buffer, offset, bit, flag);
        }
        #endregion

        #region String
        public void WriteString(int dbNumber, int offset, int maxlen, string str)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;
            S7.SetStringAt(buffer, offset, maxlen, str);
        }

        public string ReadString(int dbNumber, int offset)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;
            var str = S7.GetStringAt(buffer, offset);
            return str;
        }
        #endregion

        #region Real
        public void WriteReal(int dbNumber, int pos, float real)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;
            S7.SetRealAt(buffer, pos, real);
        }

        public float ReadReal(int dbNumber, int pos)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;
            var real = S7.GetRealAt(buffer, pos);
            return real;
        }

        #endregion

        #region LReal
        public void WriteLReal(int dbNumber, int pos, double real)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;
            S7.SetLRealAt(buffer, pos, real);
        }

        public double ReadLReal(int dbNumber, int pos)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;
            var real = S7.GetLRealAt(buffer, pos);
            return real;
        }

        #endregion

        #region ulong
        public ulong ReadULong(int dbNumber, int pos)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }

            var buffer = config.Bytes;

            var val = S7.GetULIntAt(buffer, pos);
            return val;
        }


        public void WriteULong(int dbNumber, int pos, ulong value)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;

            S7.SetULintAt(buffer, pos, value);
        }
        #endregion

        #region uint32
        public uint ReadUInt32(int dbNumber, int pos)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }

            var buffer = config.Bytes;

            var val = S7.GetUDIntAt(buffer, pos);
            return val;
        }

        public void WriteUInt32(int dbNumber, int pos, uint value)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;

            S7.SetUDIntAt(buffer, pos, value);
        }
        #endregion

        #region int32(DInt)
        public int ReadInt(int dbNumber, int pos)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }

            var buffer = config.Bytes;

            var val = S7.GetDIntAt(buffer, pos);
            return val;
        }

        public void WriteInt(int dbNumber, int pos, int value)
        {
            var config = _runningVM.RunningsItems.Where(i => i.AreaKind == AreaKind.DB && i.BlockNumber == dbNumber).FirstOrDefault();
            if (config == null)
            {
                throw new ArgumentException($"DBNumber={dbNumber} 不存在！", nameof(dbNumber));
            }
            var buffer = config.Bytes;

            S7.SetDIntAt(buffer, pos, value);
        }
        #endregion
    }
}
