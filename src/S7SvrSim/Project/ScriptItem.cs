using System.Xml.Serialization;

namespace S7SvrSim.Project
{
    [XmlRoot("Script")]
    public class ScriptItem
    {
        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public bool Recursive { get; set; }

        [XmlAttribute]
        public bool Remove { get; set; }

        public string AbsolutePath(string path)
        {
            return System.IO.Path.GetFullPath(Path, path);
        }
    }
}
