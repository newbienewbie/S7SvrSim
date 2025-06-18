using System;

namespace S7SvrSim.S7Signal
{
    public class SignalAddress
    {
        public SignalAddress()
        {

        }
        public SignalAddress(int dbIndex, int index, byte offset)
        {
            DbIndex = dbIndex;
            Index = index;
            Offset = offset;
        }

        public SignalAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return;
            }

            var splits = address.Split('.');
            if (splits.Length >= 1)
            {
                DbIndex = string.IsNullOrWhiteSpace(splits[0]) ? 0 : int.Parse(splits[0]);
            }
            if (splits.Length >= 2)
            {
                Index = string.IsNullOrWhiteSpace(splits[1]) ? 0 : int.Parse(splits[1]);
            }
            if (splits.Length >= 3)
            {
                Offset = string.IsNullOrWhiteSpace(splits[2]) ? (byte)0 : byte.Parse(splits[2]);
            }
        }

        public SignalAddress(SignalAddress other)
        {
            if (other == null)
            {
                return;
            }
            DbIndex = other.DbIndex;
            Index = other.Index;
            Offset = other.Offset;
            HideOffset = other.HideOffset;
        }

        public int DbIndex { get; set; }
        public int Index { get; set; }
        public byte Offset { get; set; }
        public bool HideOffset { get; set; } = true;

        public static SignalAddress Parse(string address)
        {
            var signalAddress = new SignalAddress();
            if (string.IsNullOrEmpty(address))
            {
                return signalAddress;
            }

            var splits = address.Split('.');
            if (splits.Length >= 1)
            {
                signalAddress.DbIndex = string.IsNullOrWhiteSpace(splits[0]) ? 0 : int.Parse(splits[0]);
            }
            if (splits.Length >= 2)
            {
                signalAddress.Index = string.IsNullOrWhiteSpace(splits[1]) ? 0 : int.Parse(splits[1]);
            }
            if (splits.Length >= 3)
            {
                signalAddress.Offset = string.IsNullOrWhiteSpace(splits[2]) ? (byte)0 : byte.Parse(splits[2]);
                signalAddress.HideOffset = false;
            }

            return signalAddress;
        }

        public override string ToString()
        {
            if (HideOffset)
            {
                return $"{DbIndex}.{Index}";
            }
            else
            {
                return $"{DbIndex}.{Index}.{Offset}";
            }
        }

        public override bool Equals(object obj)
        {
            return obj is SignalAddress address &&
                   DbIndex == address.DbIndex &&
                   Index == address.Index &&
                   Offset == address.Offset &&
                   HideOffset == address.HideOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DbIndex, Index, Offset, HideOffset);
        }

        public static bool operator ==(SignalAddress address1, SignalAddress address2)
        {
            if (address1 is null || address2 is null)
            {
                return address1 is null && address2 is null;
            }
            return address1.DbIndex == address2.DbIndex && address1.Index == address2.Index && address1.Offset == address2.Offset;
        }

        public static bool operator !=(SignalAddress address1, SignalAddress address2)
        {
            if (address1 is null || address2 is null)
            {
                return (address1 is null && address2 is not null) || (address1 is not null && address2 is null);
            }
            return address1.DbIndex != address2.DbIndex || address1.Index != address2.Index || address1.Offset != address2.Offset;
        }
    }
}
