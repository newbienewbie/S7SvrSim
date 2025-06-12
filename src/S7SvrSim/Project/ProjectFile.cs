using S7Svr.Simulator.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace S7SvrSim.Project
{
    [XmlRoot("Project")]
    public class ProjectFile
    {
        [XmlAttribute]
        public string Name { get; set; }

        public string IpAddress { get; set; }

        [XmlElement("AreaConfig")]
        public List<AreaConfig> AreaConfigs { get; set; } = new List<AreaConfig>();

        [XmlElement("ScriptItem")]
        public List<ScriptItem> ScriptItems { get; set; } = new List<ScriptItem>();

        [XmlElement("SearchPath")]
        public List<string> SearchPaths { get; set; } = new();

        public void Save(string path)
        {
            var xml = new XmlSerializer(typeof(ProjectFile));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
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
