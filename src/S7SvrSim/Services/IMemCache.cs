namespace S7SvrSim.Services
{
    public interface IMemCache<T>
    {
        T Value { get; }
    }
}
