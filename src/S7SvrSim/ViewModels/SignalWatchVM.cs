using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using S7Svr.Simulator;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.Shared;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace S7SvrSim.ViewModels
{
    using SignalWithType = ObjectWith<SignalBase, Type>;
    public partial class SignalWatchVM : ViewModelBase
    {
        private readonly IS7DataBlockService db;
        private readonly IMediator mediator;

        CancellationTokenSource cancelSource;

        private class AddressUsed
        {
            public AddressUsedAttribute Attribute { get; }
            public MethodInfo CalcMethod { get; }
            public AddressUsed(Type ty)
            {
                Attribute = ty.GetCustomAttribute<AddressUsedAttribute>();
                if (Attribute != null && !string.IsNullOrEmpty(Attribute.CalcMethod))
                {
                    CalcMethod = ty.GetMethod(Attribute.CalcMethod);
                }
            }
        }

        public Type[] SignalTypes { get; }
        private Dictionary<Type, AddressUsed> SignalAddressUsed { get; }
        public ObservableCollection<SignalEditObj> Signals { get; } = [];
        public MultiSelector Selector { get; set; }

        [ObservableProperty]
        private int scanSpan = 50;

        public SignalWatchVM(IS7DataBlockService db, IMediator mediator)
        {
            var runningModel = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            runningModel.PropertyChanged += RunningModel_PropertyChanged;

            SignalTypes = [.. typeof(SignalWatchVM).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SignalBase)))];
            SignalAddressUsed = SignalTypes.Select(ty => (Type: ty, Attr: new AddressUsed(ty))).Where(it => it.Attr.Attribute != null).ToDictionary(it => it.Type, it => it.Attr);
            this.db = db;
            this.mediator = mediator;
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
        }

        private void RegistCommandEventHandle(ICommand command)
        {
            command.AfterExecute += CommandEventHandle;
            command.AfterUndo += CommandEventHandle;
        }

        [RelayCommand]
        private void NewSignal(Type signalType)
        {
            var command = ListChangedCommand.Add(Signals, [new SignalEditObj(signalType)]);
            RegistCommandEventHandle(command);
            UndoRedoManager.Run(command);
        }

        [RelayCommand]
        private void RemoveSignal(SignalEditObj signal)
        {
            var command = ListChangedCommand.Remove(Signals, [signal]);
            RegistCommandEventHandle(command);
            UndoRedoManager.Run(command);
        }

        private void UpdateAddress(IEnumerable<SignalEditObj> signals)
        {
            if (signals.Count() <= 1)
            {
                return;
            }

            var preSignal = signals.First();
            var preAddress = preSignal.Value.Address;
            if (preAddress == null)
            {
                return;
            }

            AddressUsed preUsed = null;

            try
            {
                preUsed = SignalAddressUsed[preSignal.Other];
            }
            catch (KeyNotFoundException)
            {

            }

            if (preUsed == null)
            {
                return;
            }

            UndoRedoManager.StartTransaction();
            try
            {
                foreach (var signal in signals.Skip(1))
                {
                    if (SignalAddressUsed.TryGetValue(signal.Other, out var used))
                    {
                        var preUsedItem = GetAddressUsedItem(preUsed, preSignal);
                        var usedItem = GetAddressUsedItem(used, signal);

                        var dbIndex = preAddress.DbIndex;
                        int index;
                        byte offset = 0;

                        if (preUsedItem.IndexSize == 0 && usedItem.IndexSize == 0)
                        {
                            if (preAddress.Offset >= 7)
                            {
                                index = preAddress.Index + 1;
                                offset = 0;
                            }
                            else
                            {
                                index = preAddress.Index;
                                offset = (byte)(preAddress.Offset + preUsedItem.OffsetSize);
                            }
                        }
                        else
                        {
                            index = preAddress.Index + (preUsedItem.IndexSize == 0 ? 1 : preUsedItem.IndexSize);
                        }

                        if (index % 2 == 1)
                        {
                            index += 1;
                        }

                        var newAddress = new SignalAddress(dbIndex, index, offset)
                        {
                            HideOffset = usedItem.IndexSize != 0
                        };

                        if (newAddress != signal.Value.Address)
                        {
                            var command = new ValueChangedCommand<SignalAddress>(address =>
                            {
                                signal.Value.Address = address;
                            }, signal.Value.Address, newAddress);
                            UndoRedoManager.Run(command);
                        }

                        preSignal = signal;
                        preAddress = newAddress;
                        preUsed = used;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                UndoRedoManager.EndTransaction();
            }
        }

        private AddressUsedItem GetAddressUsedItem(AddressUsed used, SignalEditObj signal)
        {
            if (used.CalcMethod == null)
            {
                return new AddressUsedItem()
                {
                    IndexSize = used.Attribute.IndexSize,
                    OffsetSize = used.Attribute.OffsetSize
                };
            }
            else
            {
                return (AddressUsedItem)used.CalcMethod.Invoke(signal.Value, []);
            }
        }


        [RelayCommand]
        private void UpdateAddressFromFirst()
        {
            UpdateAddress(Signals);
        }

        [RelayCommand]
        private void UpdateAddressFromFirtSelected()
        {
            if (Selector.SelectedItems.Count == 0)
            {
                return;
            }

            UpdateAddress(Signals.Skip(Signals.IndexOf(Selector.SelectedItems.Cast<SignalEditObj>().First())));
        }

        [RelayCommand]
        private void UpdateAddressFromSelectedItems()
        {
            UpdateAddress(Selector.SelectedItems.Cast<SignalEditObj>());
        }

        internal void SetScanSpan(int scanSpan)
        {
#pragma warning disable MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            this.scanSpan = scanSpan;
#pragma warning restore MVVMTK0034 // Direct field reference to [ObservableProperty] backing field
            OnPropertyChanged(nameof(ScanSpan));
        }

        partial void OnScanSpanChanged(int oldValue, int newValue)
        {
            var command = new ValueChangedCommand<int>(SetScanSpan, oldValue, newValue);
            RegistCommandEventHandle(command);
            UndoRedoManager.Run(command);
        }

        #region Watch Method
        private void RunningModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (sender is RunningSnap7ServerVM runningModel && e.PropertyName == nameof(RunningSnap7ServerVM.RunningStatus))
            {
                if (runningModel.RunningStatus)
                {
                    StartWatch();
                }
                else
                {
                    EndWatch();
                }
            }
        }

        Task task;
        private void StartWatch()
        {
            if (cancelSource != null && !cancelSource.IsCancellationRequested)
            {
                EndWatch();
            }

            cancelSource = new CancellationTokenSource();
            task = WatchTask(cancelSource.Token);
        }

        private void EndWatch()
        {
            if (cancelSource != null)
            {
                try
                {
                    cancelSource.Cancel();
                }
                catch (Exception)
                {

                }
            }
        }

        private async Task WatchTask(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Signals.Each(s =>
                {
                    if (s.Value is SignalBase bs)
                    {
                        try
                        {
                            bs.Refresh(db);
                        }
                        catch (Exception e) when (e is ArgumentException || e is IndexOutOfRangeException)
                        {

                        }
                    }
                });
                this.RaisePropertyChanged(nameof(Signals));
                await Task.Delay(TimeSpan.FromMilliseconds(ScanSpan >= 0 ? ScanSpan : 50), token);
            }
        }
        #endregion
    }

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

            Value = newVal;
        }

        private SignalBase CloneValue()
        {
            var value = (SignalBase)Activator.CreateInstance(Other);
            value.Value = Value.Value;
            value.Address = Value.Address == null ? null : new SignalAddress(Value.FormatAddress);
            value.Name = Value.Name;

            if (value is S7Signal.String strSignal && Value is S7Signal.String curStrSignal)
            {
                strSignal.MaxLen = curStrSignal.MaxLen;
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

        public void EndEdit()
        {
            if (_bakup != null && (Other != _bakup.Other || Value.FormatAddress != _bakup.Value.FormatAddress || Value.Name != _bakup.Value.Name || (_bakup.Value is S7Signal.String bakStr && Value is S7Signal.String strSignal && bakStr.MaxLen != strSignal.MaxLen)))
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
