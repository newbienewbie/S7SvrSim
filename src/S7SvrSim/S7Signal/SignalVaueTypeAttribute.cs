using System;

namespace S7SvrSim.S7Signal
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SignalVaueTypeAttribute : Attribute
    {
        public Type ValueType { get; }
        public SignalVaueTypeAttribute(Type valueType)
        {
            ValueType = valueType;
        }
    }
}
