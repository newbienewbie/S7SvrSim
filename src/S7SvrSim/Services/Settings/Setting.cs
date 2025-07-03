using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace S7SvrSim.Services.Settings
{
    public class Setting<T> : ISetting<T>
    {
        private readonly string _key;
        private readonly IConverter<T> _converter;
        private readonly ISettingStore _settingStore;
        private readonly ILogger<ISetting<T>> _logger;
        private string _rawValue;

        private readonly ISubject<T> _changed = new ReplaySubject<T>(1);
        private T _value;

        public IObservable<T> Value => _changed.AsObservable();

        public Setting(IConverter<T> converter, ISettingStore settingStore, ILogger<ISetting<T>> logger, string key)
        {
            _key = key;
            _converter = converter;
            _settingStore = settingStore;
            _logger = logger;

            try
            {
                var loaded = _settingStore.Load(key);
                if (loaded.HasValue)
                {
                    var converted = _converter.Parse(loaded.Value);
                    _rawValue = loaded.Value;
                    _value = converted;
                }
                else
                {
                    _value = converter.GetDefaultValue();
                    _rawValue = converter.Convert(_value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"读取设置 {key} 发生错误 {ex.Message}");
                _value = converter.GetDefaultValue();
                _rawValue = converter.Convert(_value);
            }
            _changed.OnNext(_value);
        }

        public void Write(T value)
        {
            _rawValue = _converter.Convert(value);
            _value = value;

            try
            {
                _settingStore.Save(_key, _rawValue);
            }
            catch (Exception ex)
            {
                _logger.LogError($"保存设置 {_key} 发生错误 {ex.Message}");
            }

            _changed.OnNext(value);
        }
    }
}
