using System.Xml.Serialization;

namespace S7SvrSim.Project
{
    [XmlRoot("Script")]
    public class ScriptItem
    {
        [XmlAttribute]
        public string Name { get; set; }
    }
}
