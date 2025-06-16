using S7Svr.Simulator.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace S7SvrSim.Project
{
    [XmlRoot("Project")]
    public class ProjectFile
    {
        public string IpAddress { get; set; }

        [XmlElement("AreaConfig")]
        public List<AreaConfig> AreaConfigs { get; set; } = new List<AreaConfig>();

        [XmlElement("ScriptItem")]
        public List<ScriptItem> ScriptItems { get; set; } = new List<ScriptItem>();

        [XmlElement("SearchPath")]
        public List<string> SearchPaths { get; set; } = new();

        [XmlElement("Signal")]
        public List<SignalItem> Signals { get; set; } = new();

        public void DefaultInit()
        {
            IpAddress = "127.0.0.1";
            SearchPaths.Add("$DEFAULT");
            ScriptItems.Add(new ScriptItem()
            {
                Path = "Scripts\\",
                Recursive = true,
            });
        }

        public void Save(string path)
        {
            var xml = new XmlSerializer(typeof(ProjectFile));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            using var stream = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            xml.Serialize(stream, this, ns);
        }

        public static ProjectFile Load(string path)
        {
            var xml = new XmlSerializer(typeof(ProjectFile));
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (ProjectFile)xml.Deserialize(stream);
        }
    }
}
