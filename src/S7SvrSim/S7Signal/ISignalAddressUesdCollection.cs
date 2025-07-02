namespace S7SvrSim.S7Signal
{
    public interface ISignalAddressUesdCollection
    {
        IAddressUsed GetAddressUsed(SignalBase signal);
        bool TryGetAddressUsed(SignalBase signal, out IAddressUsed addressUsed);
    }
}
