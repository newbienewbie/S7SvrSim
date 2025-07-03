using Microsoft.Extensions.DependencyInjection;

namespace S7SvrSim.Services.Settings
{
    public static class SettingsEx
    {
        public static IServiceCollection AddSetting<T>(this IServiceCollection services, IConverter<T> converter, string key)
        {
            services.AddSingleton<ISetting<T>>(servicesProvider =>
            {
                var factory = servicesProvider.GetRequiredService<ISettingFactory>();
                return factory.Create(converter, key);
            });
            return services;
        }
    }
}
