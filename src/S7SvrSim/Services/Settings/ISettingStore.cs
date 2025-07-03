using DynamicData.Kernel;

namespace S7SvrSim.Services.Settings
{
    public interface ISettingStore
    {
        void Save(string key, string value);
        Optional<string> Load(string key);
    }
}
