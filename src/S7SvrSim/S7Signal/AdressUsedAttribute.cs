using System;

namespace S7SvrSim.S7Signal
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class AddressUsedAttribute : Attribute
    {
        public int IndexSize { get; set; }
        public byte OffsetSize { get; set; }
    }

    public class AddressUsed : IAddressUsed
    {
        public int IndexSize { get; set; }
        public byte OffsetSize { get; set; }
    }

    public interface IAddressUsed
    {
        int IndexSize { get; }
        byte OffsetSize { get; }
    }

    public interface IAddressUsedCalc<T>
        where T : SignalBase
    {
        IAddressUsed CalcAddressUsed(T signal);
    }
}
