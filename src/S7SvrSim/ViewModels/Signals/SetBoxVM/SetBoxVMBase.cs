using Microsoft.Extensions.DependencyInjection;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using S7SvrSim.Services.S7Blocks;
using System;
using System.Windows;

namespace S7SvrSim.ViewModels.Signals.SetBoxVM
{
    public abstract class SetBoxVMBase : ViewModelBase
    {
        [Reactive]
        public virtual SignalBase Signal { get; set; }

        public Func<bool> HasValidationError { get; set; }

        public abstract ReactiveCommand<Unit, Unit> SetValueCmd { get; }
        public event Action AfterSetValue;
        protected abstract void SetSignalValue();

        protected void InvokeAfterSetValue()
        {
            AfterSetValue?.Invoke();
        }
    }

    public class SetBoxVMBase<T> : SetBoxVMBase
    {
        [Reactive]
        public T Value { get; set; }

        public override ReactiveCommand<Unit, Unit> SetValueCmd { get; }

        private readonly IS7BlockProvider blockFactory;

        public SetBoxVMBase()
        {
            SetValueCmd = ReactiveCommand.Create(SetSignalValue);
            blockFactory = ((App)Application.Current).ServiceProvider.GetRequiredService<IS7BlockProvider>();

            this.WhenAnyValue(vm => vm.Signal).Subscribe(s =>
            {
                if (s != null)
                {
                    this.Value = (T)s.Value;
                }
            });
        }

        protected override void SetSignalValue()
        {
            if (HasValidationError != null && HasValidationError())
            {
                return;
            }

            try
            {
                if (Signal != null && Signal.Address != null && Signal.Address.IsValid())
                {
                    var block = Signal.Address.AreaKind == AreaKind.DB ? blockFactory.GetDataBlockService(Signal.Address.DbIndex) : blockFactory.GetMemoryBlockService();
                    Signal.SetValue(block, Value);
                }
                InvokeAfterSetValue();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "设置值时发生错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
