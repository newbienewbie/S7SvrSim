using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
using System;
using System.Xml.Serialization;

namespace S7SvrSim.Project
{
    [Serializable]
    public class SignalItem : ISignal
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlIgnore]
        public SignalAddress Address { get; set; }
        [XmlAttribute(AttributeName = "Address")]
        public string FormatAddress
        {
            get => Address?.ToString();
            set
            {
                if (value != null)
                {
                    Address = SignalAddress.Parse(value);
                }
                else
                {
                    Address = null;
                }
            }
        }
        [XmlAttribute]
        public string Type { get; set; }

        [XmlIgnore]
        public int? Length { get; set; }

        [XmlAttribute(AttributeName = "Length")]
        public string LengthAttr
        {
            get => Length?.ToString();
            set
            {
                if (value == null)
                {
                    Length = null;
                }
                else
                {
                    Length = int.TryParse(value, out var result) ? result : null;
                }
            }
        }

        public string Remark { get; set; }
    }
}
