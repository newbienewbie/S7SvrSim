namespace S7SvrSim.S7Signal
{
    public record UpdateAddressOptions(bool UpdateAddressByDbIndex
        , bool ForbidIndexHasOddNumber
        , bool AllowBoolIndexHasOddNumber
        , bool AllowByteIndexHAsOddNumber
        , bool StringUseTenCeiling);
}
