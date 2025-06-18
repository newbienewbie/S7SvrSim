using CommunityToolkit.Mvvm.ComponentModel;
using S7Svr.Simulator.ViewModels;

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
                if (value == null)
                {
                    Address = null;
                }
                else
                {
                    Address = new SignalAddress(value) { HideOffset = false };
                }
            }
        }

        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadBit(Address.DbIndex, Address.Index, Address.Offset);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is bool boolValue)
            {
                db.WriteBit(Address.DbIndex, Address.Index, Address.Offset, boolValue);
                Value = boolValue;
            }
        }
    }

    [AddressUsed(IndexSize = 1)]
    [SignalVaueType(typeof(byte))]
    public class Byte : SignalBase
    {
        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadByte(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is byte byteValue)
            {
                db.WriteByte(Address.DbIndex, Address.Index, byteValue);
                Value = byteValue;
            }
            else if (value is string stringValue)
            {
                byte val = byte.Parse(stringValue);

            }

                
        }
    }

    [AddressUsed(IndexSize = 2)]
    [SignalVaueType(typeof(short))]
    public class Int : SignalBase
    {
        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadShort(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is short intValue)
            {
                db.WriteShort(Address.DbIndex, Address.Index, intValue);
                Value = intValue;
            }
        }
    }

    [AddressUsed(IndexSize = 4)]
    [SignalVaueType(typeof(int))]
    public class DInt : SignalBase
    {
        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadInt(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is int dintValue)
            {
                db.WriteInt(Address.DbIndex, Address.Index, dintValue);
                Value = dintValue;
            }
        }
    }

    [AddressUsed(IndexSize = 4)]
    [SignalVaueType(typeof(uint))]
    public class UDInt : SignalBase
    {
        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadUInt32(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is uint udintValue)
            {
                db.WriteUInt32(Address.DbIndex, Address.Index, udintValue);
                Value = udintValue;
            }
        }
    }

    [AddressUsed(IndexSize = 8)]
    [SignalVaueType(typeof(ulong))]
    public class ULong : SignalBase
    {
        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadULong(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is ulong ulongValue)
            {
                db.WriteULong(Address.DbIndex, Address.Index, ulongValue);
                Value = ulongValue;
            }
        }
    }

    [AddressUsed(IndexSize = 4)]
    [SignalVaueType(typeof(float))]
    public class Real : SignalBase
    {
        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadReal(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is float realValue)
            {
                db.WriteReal(Address.DbIndex, Address.Index, realValue);
                Value = realValue;
            }
        }
    }

    [AddressUsed(IndexSize = 8)]
    [SignalVaueType(typeof(double))]
    public class LReal : SignalBase
    {
        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadLReal(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is double lrealValue)
            {
                db.WriteLReal(Address.DbIndex, Address.Index, lrealValue);
                Value = lrealValue;
            }
        }
    }

    [AddressUsed(CalcMethod = nameof(AddressUse))]
    [SignalVaueType(typeof(string))]
    public partial class String : SignalBase
    {
        [ObservableProperty]
        private int maxLen;

        public AddressUsedItem AddressUse()
        {
            var remain = (MaxLen + 2) % 10;
            var number = (MaxLen + 2) - remain;
            return new AddressUsedItem()
            {
                IndexSize = (number < 0 ? 0 : number) + (remain != 0 ? 10 : 0),
            };
        }

        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadString(Address.DbIndex, Address.Index);
        }

        public override void SetValue(IS7DataBlockService db, object value)
        {
            if (value is string stringValue)
            {
                db.WriteString(Address.DbIndex, Address.Index, MaxLen, stringValue);
                Value = stringValue;
            }
        }
    }
}
