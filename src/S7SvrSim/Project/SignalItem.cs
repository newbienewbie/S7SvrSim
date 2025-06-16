using S7SvrSim.S7Signal;
using System.Xml.Serialization;

namespace S7SvrSim.Project
{
    public class SignalItem : ISignal
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlIgnore]
        public SignalAddress Address { get; set; }
        [XmlAttribute]
        public string FormatAddress
        {
            get => Address?.ToString();
            set
            {
                if (value != null)
                {
                    Address = new SignalAddress(value);
                }
                else
                {
                    Address = null;
                }
            }
        }

        public SignalItem()
        {

        }

        public SignalItem(ISignal signal)
        {
            Name = signal.Name;
            FormatAddress = signal.FormatAddress;
        }
    }
}
