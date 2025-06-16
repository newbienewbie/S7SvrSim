using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace S7SvrSim.Shared
{
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

        public event Action<T> ValueChanged;
        public event Action<S> OtherChanged;

        partial void OnValueChanged(T value)
        {
            ValueChanged?.Invoke(value);
        }

        partial void OnOtherChanged(S value)
        {
            OtherChanged?.Invoke(value);
        }
    }
}
