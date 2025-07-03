namespace S7SvrSim.Services.Settings
{
    public interface IConverter<T>
    {
        string Convert(T value);
        T Parse(string value);
        T GetDefaultValue();
    }
}
