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
                DbIndex = int.Parse(splits[0]);
            }
            if (splits.Length >= 2)
            {
                Index = int.Parse(splits[1]);
            }
            if (splits.Length >= 3)
            {
                Offset = byte.Parse(splits[2]);
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
    }
}
