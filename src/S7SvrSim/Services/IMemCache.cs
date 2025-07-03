namespace S7SvrSim.Services
{
    public interface IMemCache<T>
    {
        T Value { get; }

        void Write(T value);
    }
}
