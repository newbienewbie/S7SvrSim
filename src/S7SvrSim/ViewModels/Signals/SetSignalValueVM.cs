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
        private object value;

        [ObservableProperty]
        private bool hasValidationError;

        public event Action AfterSetValue;

        public SetSignalValueVM()
        {
            blockFactory = ((App)Application.Current).ServiceProvider.GetRequiredService<IS7BlockProvider>();
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
            if (SelectedSignal != null && SelectedSignal.Value.Address != null && SelectedSignal.Value.Address.IsValid())
            {
                var block = SelectedSignal.Value.Address.AreaKind == AreaKind.DB ? blockFactory.GetDataBlockService(SelectedSignal.Value.Address.DbIndex) : blockFactory.GetMemoryBlockService();
                SelectedSignal.Value.SetValue(block, Value);
            }
            AfterSetValue?.Invoke();
        }
    }
}
