using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using System;
using System.Reflection;
using System.Windows;

namespace S7SvrSim.ViewModels
{
    public partial class SetSignalValueVM : ViewModelBase
    {
        private readonly IS7DataBlockService db;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ValueType))]
        private SignalEditObj selectedSignal;

        public Type ValueType => SelectedSignal?.Value?.GetType().GetCustomAttribute<SignalVaueTypeAttribute>()?.ValueType;

        [ObservableProperty]
        private object value;

        [ObservableProperty]
        private bool hasValidationError;

        public event Action AfterSetValue;

        public SetSignalValueVM()
        {
            db = ((App)Application.Current).ServiceProvider.GetRequiredService<IS7DataBlockService>();
        }

        partial void OnSelectedSignalChanged(SignalEditObj value)
        {
            Value = value.Value.Value;
            if (Value != null)
            {
                return;
            }
            var valueType = ValueType;
            if (valueType == typeof(string))
            {
                Value = "";
            }
            else
            {
                if (valueType.IsPrimitive || valueType.GetConstructor(Type.EmptyTypes) != null)
                {
                    Value = Activator.CreateInstance(valueType);
                }
                else
                {
                    Value = null;
                }
            }
        }

        [RelayCommand]
        private void SetValue()
        {
            if (HasValidationError == true)
            {
                return;
            }
            SelectedSignal?.Value.SetValue(db, Value);
            AfterSetValue?.Invoke();
        }
    }
}
