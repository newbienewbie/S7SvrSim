using CommunityToolkit.Mvvm.ComponentModel;
using S7Svr.Simulator.ViewModels;

namespace S7SvrSim.S7Signal
{
    public interface ISignal
    {
        string Name { get; set; }
        SignalAddress Address { get; set; }
        string FormatAddress { get; set; }
    }

    public abstract partial class SignalBase : ObservableObject, ISignal
    {
        [ObservableProperty]
        private object value;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormatAddress))]
        private SignalAddress address;

        public virtual string FormatAddress
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

        public abstract void Refresh(IS7DataBlockService db);
        public virtual void SetValue(IS7DataBlockService db, object value) { }
    }
}
