using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
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

        public int? MaxLen { get; set; }

        public string Remark { get; set; }

        public SignalItem()
        {

        }

        public SignalItem(SignalBase signal)
        {
            Name = signal.Name;
            FormatAddress = signal.FormatAddress;
            Type = signal.GetType().Name;
            Remark = signal.Remark;
            if (signal is S7Signal.String str)
            {
                MaxLen = str.MaxLen;
            }
        }

        public SignalItem(SignalEditObj signal)
        {
            Name = signal.Value.Name;
            FormatAddress = signal.Value.FormatAddress;
            Type = signal.Other.Name;
            Remark = signal.Value.Remark;
            if (signal.Value is S7Signal.String str)
            {
                MaxLen = str.MaxLen;
            }
        }
    }
}
