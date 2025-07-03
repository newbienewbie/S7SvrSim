using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace S7SvrSim.Project
{
    [Serializable]
    public class SignalGroup
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlElement("Signal")]
        public List<SignalItem> SignalItems { get; set; } = new List<SignalItem>();
    }
}
