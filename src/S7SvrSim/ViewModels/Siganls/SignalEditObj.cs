using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.ViewModels
{
    public partial class SignalEditObj : ObservableObject
    {
        private readonly IMemCache<Type[]> signalTypes;
        private bool isInit = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SignaleType))]
        public Type other;

        [ObservableProperty]
        private SignalBase value;

        public string SignaleType
        {
            get => (Other.IsSubclassOf(typeof(SignalWithLengthBase)) && Value is SignalWithLengthBase lengthSignal) ? $"{Other.Name}[{lengthSignal?.Length}]" : Other.Name;
            set
            {
                if (!UndoRedoManager.IsInUndoRedo)
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        return;
                    }

                    if (SignaleType.Equals(value, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    var tyStr = value.Split('[')[0];
                    var len = GetLength(value);

                    var typeQuery = signalTypes.Value.Where(ty => ty.Name.Equals(tyStr, StringComparison.OrdinalIgnoreCase));
                    if (typeQuery.Any())
                    {
                        UndoRedoManager.StartTransaction();
                        Other = typeQuery.First();
                        if (Value is SignalWithLengthBase lengthValue)
                        {
                            lengthValue.Length = len;
                        }
                        UndoRedoManager.EndTransaction();
                    }
                }

                OnPropertyChanged();
            }
        }

        private int GetLength(string value)
        {
            var leftBaket = value.IndexOf('[');
            var rightBaket = value.IndexOf(']');

            if (leftBaket != -1 && rightBaket != -1)
            {
                if (int.TryParse(value[(leftBaket + 1)..rightBaket], out var res))
                {
                    return res;
                }
            }
            return default;
        }

        public SignalEditObj(Type type)
        {
            Other = type;

            signalTypes = ((App)App.Current).ServiceProvider.GetRequiredService<IMemCache<Type[]>>();

            isInit = true;
        }

        partial void OnValueChanged(SignalBase value)
        {
            Value.NameChanged += OnNameChanged;
            Value.RemarkChanged += OnRemarkChanged;
            Value.FormatAddressChanged += OnFormtAddressChanged;
            if (Value is SignalWithLengthBase lengthValue)
            {
                lengthValue.LengthChanged += OnSignalLengthChanged;
                lengthValue.NotifyLengthChanged();
            }
        }

        void ReleaseBind()
        {
            Value.NameChanged -= OnNameChanged;
            Value.RemarkChanged -= OnRemarkChanged;
            Value.FormatAddressChanged -= OnFormtAddressChanged;
            if (Value is SignalWithLengthBase lengthValue)
            {
                lengthValue.LengthChanged -= OnSignalLengthChanged;
                lengthValue.NotifyLengthChanged();
            }
        }

        partial void OnOtherChanged(Type value)
        {
            if (UndoRedoManager.IsInUndoRedo)
            {
                return;
            }

            var newVal = (SignalBase)Activator.CreateInstance(value);
            var preValue = CloneValue();

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

            this.value = newVal;
            OnPropertyChanged(nameof(Value));
            OnValueChanged(Value);

            if (isInit)
            {
                var command = new ValueChangedCommand<SignalEditObj>(signal =>
                {
                    Other = signal.Other;
                    ReleaseBind();
                    this.value = signal.Value;
                    OnPropertyChanged(nameof(Value));
                    OnPropertyChanged(nameof(SignaleType));
                }, new SignalEditObj(preValue.GetType()) { Value = preValue }, CloneCurrent());
                command.AfterExecute += CommandEventHandle;
                command.AfterUndo += CommandEventHandle;
                UndoRedoManager.Regist(command);
            }
        }

        private void OnSignalLengthChanged(int oldValue, int newValue)
        {
            RegistValueChangedCommand(val =>
            {
                if (Value is SignalWithLengthBase lengthSignal)
                {
                    lengthSignal.Length = val;
                    OnPropertyChanged(nameof(SignaleType));
                }
            }, oldValue, newValue);

        }

        private void OnFormtAddressChanged(string oldValue, string newValue)
        {
            RegistValueChangedCommand(val => Value.FormatAddress = val, oldValue, newValue);
        }

        private void OnRemarkChanged(string oldValue, string newValue)
        {
            RegistValueChangedCommand(val => Value.Remark = val, oldValue, newValue);
        }

        private void OnNameChanged(string oldValue, string newValue)
        {
            RegistValueChangedCommand(val => Value.Name = val, oldValue, newValue);
        }

        private void RegistValueChangedCommand<T>(Action<T> update, T oldValue, T newValue)
        {
            if (UndoRedoManager.IsInUndoRedo || EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                return;
            }

            var command = new ValueChangedCommand<T>(val =>
            {
                update?.Invoke(val);
            }, oldValue, newValue);
            command.AfterExecute += CommandEventHandle;
            command.AfterUndo += CommandEventHandle;
            UndoRedoManager.Regist(command);
        }

        private SignalBase CloneValue()
        {
            if (Value == null)
            {
                return null;
            }

            var value = (SignalBase)Activator.CreateInstance(Value.GetType());
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

        private SignalEditObj CloneCurrent()
        {
            return new SignalEditObj(Other)
            {
                Other = Other,
                Value = CloneValue()
            };
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
        }
    }
}
