using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace S7SvrSim.Shared
{
    public delegate void PropertyChanged<T>(T oldValue, T newValue);

    public partial class ObjectWithBool<T> : ObservableObject
    {
        [ObservableProperty]
        private T value;
        [ObservableProperty]
        private bool boolean;

        public event Action<bool> BooleanChanged;
        partial void OnBooleanChanged(bool value)
        {
            BooleanChanged?.Invoke(value);
        }
    }

    public partial class ObjectWith<T, S> : ObservableObject
    {
        [ObservableProperty]
        private T value;
        [ObservableProperty]
        private S other;

        public event PropertyChanged<T> ValueChanged;
        public event PropertyChanged<S> OtherChanged;

        partial void OnValueChanged(T oldValue, T newValue)
        {
            ValueChanged?.Invoke(oldValue, newValue);
        }

        partial void OnOtherChanged(S oldValue, S newValue)
        {
            OtherChanged?.Invoke(oldValue, newValue);
        }
    }
}
