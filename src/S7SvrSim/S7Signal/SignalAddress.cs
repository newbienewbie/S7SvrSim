using S7Svr.Simulator.ViewModels;
using System;

namespace S7SvrSim.S7Signal
{
    public class SignalAddress : IComparable<SignalAddress>, IEquatable<SignalAddress>
    {
        public SignalAddress()
        {

        }

        public SignalAddress(int index, byte offset)
        {
            AreaKind = AreaKind.MB;
            Index = index;
            Offset = offset;
        }

        public SignalAddress(int dbIndex, int index, byte offset)
        {
            AreaKind = AreaKind.DB;
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
                var dbStr = splits[0];
                if (dbStr.ToUpper() == "MB")
                {
                    AreaKind = AreaKind.MB;
                }
                else
                {
                    AreaKind = AreaKind.DB;
                    DbIndex = string.IsNullOrWhiteSpace(dbStr) ? 0 : (dbStr.StartsWith("DB", StringComparison.OrdinalIgnoreCase) ? int.Parse(dbStr.Substring(2)) : int.Parse(dbStr));
                }
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
            AreaKind = other.AreaKind;
            DbIndex = other.DbIndex;
            Index = other.Index;
            Offset = other.Offset;
            HideOffset = other.HideOffset;
        }
        public AreaKind AreaKind { get; set; }
        public int DbIndex { get; set; }
        public int Index { get; set; }
        public byte Offset { get; set; }
        public bool HideOffset { get; set; } = true;

        /// <summary>
        /// 和 <see cref="SignalAddress.SignalAddress(string)"/> 的区别在于，该方法会区别比特位
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static SignalAddress Parse(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                return new SignalAddress();
            }

            var splits = address.Split('.');
            var signalAddress = new SignalAddress(address);

            if (splits.Length >= 3)
            {
                signalAddress.HideOffset = false;
            }

            return signalAddress;
        }

        public bool IsValid()
        {
            return DbIndex >= 0 && Index >= 0 && (HideOffset || Offset >= 0);
        }

        public override string ToString()
        {
            if (HideOffset)
            {
                return $"{(AreaKind == AreaKind.DB ? $"DB{DbIndex}" : "MB")}.{Index}";
            }
            else
            {
                return $"{(AreaKind == AreaKind.DB ? $"DB{DbIndex}" : "MB")}.{Index}.{Offset}";
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(AreaKind, DbIndex, Index, Offset, HideOffset);
        }

        public int CompareTo(SignalAddress other)
        {
            if (other == null)
            {
                return 1;
            }

            if (AreaKind != other.AreaKind)
            {
                return AreaKind == AreaKind.DB ? -1 : 1;
            }

            if (AreaKind == AreaKind.DB)
            {
                if (DbIndex != other.DbIndex)
                {
                    return DbIndex - other.DbIndex;
                }
            }

            if (Index != other.Index)
            {
                return Index - other.Index;
            }

            if (HideOffset && !other.HideOffset)
            {
                return 1;
            }

            if (!HideOffset && !other.HideOffset)
            {
                return Offset - other.Offset;
            }

            return 0;
        }

        public bool Equals(SignalAddress other)
        {
            return this == other;
        }

        public static bool operator ==(SignalAddress address1, SignalAddress address2)
        {
            if (address1 is null || address2 is null)
            {
                return address1 is null && address2 is null;
            }
            return (address1.AreaKind == address2.AreaKind && (address1.AreaKind == AreaKind.MB || address1.DbIndex == address2.DbIndex)) && address1.Index == address2.Index && address1.Offset == address2.Offset;
        }

        public static bool operator !=(SignalAddress address1, SignalAddress address2)
        {
            return !(address1 == address2);
        }
    }
}
