namespace S7SvrSim.Services.Settings
{
    public interface ISettingFactory
    {
        ISetting<T> Create<T>(IConverter<T> converter, string key);
    }
}
