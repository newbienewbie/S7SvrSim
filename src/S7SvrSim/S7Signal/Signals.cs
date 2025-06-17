using CommunityToolkit.Mvvm.ComponentModel;
using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.S7Signal
{
    [AddressUsed(OffsetSize = 1)]
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

    [AddressUsed(IndexSize = 1)]
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

    [AddressUsed(IndexSize = 2)]
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

    [AddressUsed(IndexSize = 4)]
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

    [AddressUsed(IndexSize = 4)]
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

    [AddressUsed(IndexSize = 8)]
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

    [AddressUsed(IndexSize = 4)]
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

    [AddressUsed(IndexSize = 8)]
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

    [AddressUsed(CalcMethod = nameof(AddressUse))]
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
    }
}
