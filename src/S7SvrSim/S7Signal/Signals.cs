using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.S7Signal
{
    public class Bool : SignalBase
    {
        public override string FormatAddress
        {
            get => base.FormatAddress;
            set => Address = new SignalAddress(value) { HideOffset = false };
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
    }

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
    }

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
    }

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
    }

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
    }

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
    }

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
    }

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
    }

    public class String : SignalBase
    {
        public int MaxLen { get; set; }

        public override void Refresh(IS7DataBlockService db)
        {
            if (Address == null)
            {
                Value = null;
                return;
            }
            Value = db.ReadString(Address.DbIndex, Address.Index);
        }
    }
}
