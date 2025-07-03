namespace S7SvrSim.S7Signal
{
    public class UpdateAddressOptions(bool updateAddressByDbIndex, bool forbidIndexHasOddNumber, bool allowBoolIndexHasOddNumber, bool allowByteIndexHAsOddNumber, bool stringUseTenCeiling)
    {
        public bool UpdateAddressByDbIndex { get; } = updateAddressByDbIndex;
        public bool ForbidIndexHasOddNumber { get; } = forbidIndexHasOddNumber;
        public bool AllowBoolIndexHasOddNumber { get; } = allowBoolIndexHasOddNumber;
        public bool AllowByteIndexHAsOddNumber { get; } = allowByteIndexHAsOddNumber;
        public bool StringUseTenCeiling { get; } = stringUseTenCeiling;
    }
}
