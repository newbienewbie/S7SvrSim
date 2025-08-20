using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using S7SvrSim.Services.S7Blocks;
using System;
using System.Reflection;
using System.Windows;

namespace S7SvrSim.ViewModels
{
    public partial class SetSignalValueVM : ViewModelBase
    {
        private readonly IS7BlockProvider blockFactory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ValueType))]
        private SignalEditObj selectedSignal;

        public Type ValueType => SelectedSignal?.Value?.GetType().GetCustomAttribute<SignalVaueTypeAttribute>()?.ValueType;

        [ObservableProperty]
        private string value;

        [ObservableProperty]
        private bool hasValidationError;

        public event Action AfterSetValue;

        public SetSignalValueVM()
        {
            blockFactory = ((App)Application.Current).ServiceProvider.GetRequiredService<IS7BlockProvider>();
        }

        partial void OnSelectedSignalChanged(SignalEditObj value)
        {
            Value = (string)Convert.ChangeType(value.Value.Value, typeof(string));
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
                    Value = (string)Convert.ChangeType(Activator.CreateInstance(valueType), typeof(string));
                }
                else
                {
                    Value = "";
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
            if (SelectedSignal != null && SelectedSignal.Value.Address != null && SelectedSignal.Value.Address.IsValid())
            {
                var block = SelectedSignal.Value.Address.AreaKind == AreaKind.DB ? blockFactory.GetDataBlockService(SelectedSignal.Value.Address.DbIndex) : blockFactory.GetMemoryBlockService();
                SelectedSignal.Value.SetValue(block, Convert.ChangeType(Value, ValueType));
            }
            AfterSetValue?.Invoke();
        }
    }
}
