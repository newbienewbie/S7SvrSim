using S7SvrSim.Services.S7Blocks;
using System.ComponentModel;

namespace S7SvrSim.S7Signal
{
    [AddressUsed(OffsetSize = 1)]
    [SignalVaueType(typeof(bool))]
    public class BoolSignal : SignalBase
    {
        public override string FormatAddress
        {
            get => base.FormatAddress;
            set
            {
                if (Address?.ToString() == value)
                {
                    return;
                }

                var oldAddress = Address?.ToString();

                if (string.IsNullOrEmpty(value))
                {
                    Address = null;
                }
                else
                {
                    Address = new SignalAddress(value) { HideOffset = false };
                }
                this.RaisePropertyChanged();
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
    public class ByteSignal : SignalBase
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
    public class IntSignal : SignalBase
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
    public class DIntSignal : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadDInt(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is int dintValue)
            {
                block.WriteDInt(Address.Index, dintValue);
                Value = dintValue;
            }
        }
    }

    [AddressUsed(IndexSize = 2)]
    [SignalVaueType(typeof(ushort))]
    public class UIntSignal : SignalBase
    {
        public override void Refresh(IS7Block block)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = block.ReadUShort(Address.Index);
        }

        public override void SetValue(IS7Block block, object value)
        {
            if (value is ushort ushortValue)
            {
                block.WriteUShort(Address.Index, ushortValue);
                Value = ushortValue;
            }
        }
    }

    [AddressUsed(IndexSize = 4)]
    [SignalVaueType(typeof(uint))]
    public class UDIntSignal : SignalBase
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
    public class ULongSignal : SignalBase
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
    public class RealSignal : SignalBase
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
    public class LRealSignal : SignalBase
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

    [SignalVaueType(typeof(string))]
    public partial class StringSignal : SignalWithLengthBase
    {
        public static bool UseTenCeiling = false;

        public AddressUsed AddressUse()
        {
            if (UseTenCeiling)
            {
                var remain = (Length + 2) % 10;
                var number = (Length + 2) - remain;
                return new AddressUsed()
                {
                    IndexSize = (number < 0 ? 0 : number) + (remain != 0 ? 10 : 0),
                };
            }
            else
            {
                return new AddressUsed()
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
    [Description("Holding 是一个特殊类型，用于占用对应长度的字节")]
    public class HoldingSignal : SignalWithLengthBase
    {
        public AddressUsed AddressUse()
        {
            return new AddressUsed()
            {
                IndexSize = Length
            };
        }

        public override void Refresh(IS7Block block)
        {

        }
    }
}
