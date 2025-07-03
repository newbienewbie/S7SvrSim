using Microsoft.Extensions.Logging;

namespace S7SvrSim.Services.Settings
{
    public class SettingFactory : ISettingFactory
    {
        private readonly ISettingStore settingStore;
        private readonly ILoggerFactory loggerFactory;

        public SettingFactory(ISettingStore settingStore, ILoggerFactory loggerFactory)
        {
            this.settingStore = settingStore;
            this.loggerFactory = loggerFactory;
        }

        public ISetting<T> Create<T>(IConverter<T> converter, string key)
        {
            return new Setting<T>(converter, settingStore, loggerFactory.CreateLogger<ISetting<T>>(), key);
        }
    }
}
