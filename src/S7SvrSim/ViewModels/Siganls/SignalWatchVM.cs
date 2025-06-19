using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using S7Svr.Simulator;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.Shared;
using S7SvrSim.UserControls.Signals;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace S7SvrSim.ViewModels
{
    public partial class SignalWatchVM : ViewModelBase
    {
        private readonly IS7DataBlockService db;
        private readonly IMediator mediator;

        CancellationTokenSource cancelSource;

        public Type[] SignalTypes { get; }

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
        private Dictionary<Type, AddressUsed> SignalAddressUsed { get; }

        public ObservableCollection<SignalEditObj> Signals { get; } = [];

        public MultiSelector Selector { get; set; }

        [ObservableProperty]
        private int scanSpan = 50;

        [ObservableProperty]
        private SignalEditObj selectedEditObj;

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

        #region Signal Edit
        public void OpenValueSet()
        {
            if (SelectedEditObj == null || !IsInWatch)
            {
                return;
            }
            var setWindow = new SetSignalValueWindow();
            setWindow.viewModel.SelectedSignal = SelectedEditObj;

            setWindow.ShowDialog();
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

        public void ReplaceSignal(SignalEditObj oldItem, SignalEditObj newItem)
        {
            int oldIndex = Signals.IndexOf(oldItem);
            int newIndex = Signals.IndexOf(newItem);

            if (oldIndex != newIndex)
            {
                var command = ListChangedCommand.Replace(Signals, [(oldItem, newItem), (newItem, oldItem)]);
                RegistCommandEventHandle(command);
                UndoRedoManager.Run(command);
            }
        }
        #endregion

        #region Quick Cal Address
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
        #endregion

        #region ScanSpan For UndoRedo
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
        #endregion

        #region Watch Method
        Task watchTask;
        public bool IsInWatch => watchTask != null && cancelSource != null && !cancelSource.IsCancellationRequested;
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

        private void StartWatch()
        {
            if (cancelSource != null && !cancelSource.IsCancellationRequested)
            {
                EndWatch();
            }

            cancelSource = new CancellationTokenSource();
            watchTask = WatchTask(cancelSource.Token);
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
                finally
                {
                    watchTask = null;
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
                await Task.Delay(TimeSpan.FromMilliseconds(ScanSpan >= 0 ? ScanSpan : 50), token);
            }
        }
        #endregion
    }
}
