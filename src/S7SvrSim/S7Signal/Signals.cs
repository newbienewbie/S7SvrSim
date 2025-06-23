using S7SvrSim.Services;
using System.ComponentModel;

namespace S7SvrSim.S7Signal
{
    [AddressUsed(OffsetSize = 1)]
    [SignalVaueType(typeof(bool))]
    public class Bool : SignalBase
    {
        public override string FormatAddress
        {
            get => base.FormatAddress;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Address = null;
                }
                else
                {
                    Address = new SignalAddress(value) { HideOffset = false };
                }
            }
        }

        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadBit(Address.Index, Address.Offset);
        }

        public void Refresh(byte flags)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }

            Value = (flags & (1 << Address.Offset)) != 0;
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is bool boolValue)
            {
                block.WriteBit(Address.Index, Address.Offset, boolValue);
                Value = boolValue;
            }
        }
    }

    [AddressUsed(IndexSize = 1)]
    [SignalVaueType(typeof(byte))]
    public class Byte : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadByte(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is byte byteValue)
            {
                block.WriteByte(Address.Index, byteValue);
                Value = byteValue;
            }
            else if (value is string stringValue)
            {
                byte val = byte.Parse(stringValue);
                block.WriteByte(Address.Index, val);
                Value = val;
            }
        }
    }

    [AddressUsed(IndexSize = 2)]
    [SignalVaueType(typeof(short))]
    public class Int : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadShort(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is short intValue)
            {
                block.WriteShort(Address.Index, intValue);
                Value = intValue;
            }
        }
    }

    [AddressUsed(IndexSize = 4)]
    [SignalVaueType(typeof(int))]
    public class DInt : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadInt(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is int dintValue)
            {
                block.WriteInt(Address.Index, dintValue);
                Value = dintValue;
            }
        }
    }

    [AddressUsed(IndexSize = 4)]
    [SignalVaueType(typeof(uint))]
    public class UDInt : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadUInt32(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is uint udintValue)
            {
                block.WriteUInt32(Address.Index, udintValue);
                Value = udintValue;
            }
        }
    }

    [AddressUsed(IndexSize = 8)]
    [SignalVaueType(typeof(ulong))]
    public class ULong : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadULong(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is ulong ulongValue)
            {
                block.WriteULong(Address.Index, ulongValue);
                Value = ulongValue;
            }
        }
    }

    [AddressUsed(IndexSize = 4)]
    [SignalVaueType(typeof(float))]
    public class Real : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadReal(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is float realValue)
            {
                block.WriteReal(Address.Index, realValue);
                Value = realValue;
            }
        }
    }

    [AddressUsed(IndexSize = 8)]
    [SignalVaueType(typeof(double))]
    public class LReal : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadLReal(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is double lrealValue)
            {
                block.WriteLReal(Address.Index, lrealValue);
                Value = lrealValue;
            }
        }
    }

    [AddressUsed(CalcMethod = nameof(AddressUse))]
    [SignalVaueType(typeof(string))]
    public partial class String : SignalWithLengthBase
    {
        public static bool UseTenCeiling = false;

        public AddressUsedItem AddressUse()
        {
            if (UseTenCeiling)
            {
                var remain = (Length + 2) % 10;
                var number = (Length + 2) - remain;
                return new AddressUsedItem()
                {
                    IndexSize = (number < 0 ? 0 : number) + (remain != 0 ? 10 : 0),
                };
            }
            else
            {
                return new AddressUsedItem()
                {
                    IndexSize = Length + 2,
                };
            }
        }

        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadString(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is string stringValue)
            {
                block.WriteString(Address.Index, Length, stringValue);
                Value = stringValue;
            }
        }
    }

    /// <summary>
    /// 地址占用
    /// </summary>
    [AddressUsed(CalcMethod = nameof(AddressUse))]
    [SignalVaueType(typeof(string))]
    [Description("Holding 是一个特殊类型，用于占用对应长度的字节")]
    public class Holding : SignalWithLengthBase
    {
        public AddressUsedItem AddressUse()
        {
            return new AddressUsedItem()
            {
                IndexSize = Length
            };
        }

        public override void Refresh(IS7Block block)
        {

        }
    }
}
