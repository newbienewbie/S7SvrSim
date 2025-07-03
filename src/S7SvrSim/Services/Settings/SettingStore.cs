using DynamicData.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace S7SvrSim.Services.Settings
{
    public class SettingStore : ISettingStore
    {
        [Serializable]
        public class Option
        {
            [XmlAttribute]
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private string Location { get; }
        private Dictionary<string, string> Options { get; }

        public SettingStore()
        {
            Location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "S7SvrSim", "Settings.xml");
            var locationDir = Path.GetDirectoryName(Location);
            if (!Directory.Exists(locationDir)) Directory.CreateDirectory(locationDir);

            if (File.Exists(Location))
            {
                var serializer = new XmlSerializer(typeof(List<Option>));
                if (serializer.Deserialize(File.OpenRead(Location)) is List<Option> options)
                {
                    Options = options.ToDictionary(op => op.Key, op => op.Value);
                }
            }

            Options ??= [];
        }

        public Optional<string> Load(string key)
        {
            if (Options.TryGetValue(key, out var value))
                return value;
            return Optional.None<string>();
        }

        public void Save(string key, string value)
        {
            if (Options.ContainsKey(key))
            {
                Options[key] = value;
            }
            else
            {
                Options.Add(key, value);
            }

            var serializer = new XmlSerializer(typeof(List<Option>));
            serializer.Serialize(File.Create(Location), Options.Select(item => new Option()
            {
                Key = item.Key,
                Value = item.Value
            }).ToList());
        }
    }
}
