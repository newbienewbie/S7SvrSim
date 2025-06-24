using CommunityToolkit.Mvvm.ComponentModel;
using S7Svr.Simulator;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using System;
using System.ComponentModel;
using SignalWithType = S7SvrSim.Shared.ObjectWith<S7SvrSim.S7Signal.SignalBase, System.Type>;

namespace S7SvrSim.ViewModels
{
    public partial class SignalEditObj : ObservableObject, IEditableObject
    {
        private SignalWithType _bakup;

        [ObservableProperty]
        private Type other;

        [ObservableProperty]
        private SignalBase value;

        public SignalEditObj(Type type)
        {
            Other = type;
        }

        partial void OnOtherChanged(Type value)
        {
            var newVal = (SignalBase)Activator.CreateInstance(value);

            if (Value != null)
            {
                newVal.Name = Value.Name;
            }
            else
            {
                newVal.Name = value.Name;
            }

            newVal.FormatAddress = (string)Value?.FormatAddress?.Clone();
            newVal.Remark = Value?.Remark;
            Value = newVal;
        }

        private SignalBase CloneValue()
        {
            var value = (SignalBase)Activator.CreateInstance(Other);
            value.Value = Value.Value;
            value.Address = Value.Address ?? null;
            value.Name = Value.Name;
            value.Remark = Value.Remark;

            if (value is S7Signal.SignalWithLengthBase lenSignal && Value is S7Signal.SignalWithLengthBase curLenSignal)
            {
                lenSignal.Length = curLenSignal.Length;
            }

            return value;
        }

        private SignalWithType CloneCurrent()
        {
            return new SignalWithType()
            {
                Other = Other,
                Value = CloneValue()
            };
        }

        public void BeginEdit()
        {
            _bakup = CloneCurrent();
        }

        public void CancelEdit()
        {
            Other = _bakup.Other;
            Value = _bakup.Value;
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
        }

        private bool IsChanged()
        {
            if (_bakup == null)
            {
                return false;
            }

            return Other != _bakup.Other
                || Value != _bakup.Value
                || (Value is S7Signal.SignalWithLengthBase valLen && _bakup.Value is S7Signal.SignalWithLengthBase bakLen && valLen != bakLen);
        }

        public void EndEdit()
        {
            if (IsChanged())
            {
                var command = new ValueChangedCommand<SignalWithType>(signal =>
                {
                    Other = signal.Other;
                    Value = signal.Value;
                }, _bakup, CloneCurrent());
                command.AfterExecute += CommandEventHandle;
                command.AfterUndo += CommandEventHandle;
                UndoRedoManager.Regist(command);
            }
            _bakup = default;
        }

        public static implicit operator SignalEditObj(SignalWithType signal)
        {
            return new SignalEditObj(signal.Other)
            {
                Value = signal.Value
            };
        }
    }
}
