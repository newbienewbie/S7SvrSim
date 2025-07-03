using System;

namespace S7SvrSim.Services.Settings
{
    public interface ISetting<T>
    {
        IObservable<T> Value { get; }

        void Write(T value);
    }
}
