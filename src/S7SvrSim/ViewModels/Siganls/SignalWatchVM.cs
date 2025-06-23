using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;
using ReactiveUI.Fody.Helpers;
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
using System.Windows;
using System.Windows.Controls;

namespace S7SvrSim.ViewModels
{
    public partial class SignalWatchVM : ViewModelBase
    {
        #region DI
        private readonly IS7DataBlockService db;
        private readonly IMediator mediator;
        #endregion

        public DataGrid Grid { get; set; }

        public Type[] SignalTypes { get; }

        public ObservableCollection<SignalEditObj> Signals { get; } = [];

        [ObservableProperty]
        private int scanSpan = 50;

        [ObservableProperty]
        private SignalEditObj selectedEditObj;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanMoveAfter))]
        [NotifyPropertyChangedFor(nameof(CanMoveBefore))]
        private SignalEditObj dragTargetSignal;

        [Reactive]
        public bool AllowBoolIndexHasOddNumber { get; set; } = true;

        [Reactive]
        public bool ForbidIndexHasOddNumber { get; set; } = true;

        [Reactive]
        public bool UpdateAddressByDbIndex { get; set; } = false;

        [Reactive]
        public bool StringUseTenCeiling { get; set; } = S7Signal.String.UseTenCeiling;

        [Reactive]
        public bool IsDragSignals { get; set; }
        public ObservableCollection<SignalEditObj> DragSignals { get; } = [];

        public event Action<IEnumerable<SignalEditObj>> AfterDragEvent;

        public SignalWatchVM(IS7DataBlockService db, IMediator mediator)
        {
            this.db = db;
            this.mediator = mediator;

            var runningModel = Locator.Current.GetRequiredService<RunningSnap7ServerVM>();
            runningModel.PropertyChanged += RunningModel_PropertyChanged;

            SignalTypes = [.. typeof(SignalWatchVM).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(SignalBase)))];
            SignalAddressUsed = SignalTypes.Select(ty => (Type: ty, Attr: new AddressUsed(ty))).Where(it => it.Attr.Attribute != null).ToDictionary(it => it.Type, it => it.Attr);
            DragSignals.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(DragSignalsIsOne));
                OnPropertyChanged(nameof(CanMoveAfter));
                OnPropertyChanged(nameof(CanMoveBefore));
            };
            this.WhenAnyValue(vm => vm.StringUseTenCeiling).Subscribe(u => S7Signal.String.UseTenCeiling = u);
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

        private void SetGridSelectedItems(IEnumerable<SignalEditObj> signals)
        {
            Grid.UnselectAll();
            foreach (var item in signals)
            {
                Grid.SelectedItems.Add(item);
            }
            if (signals.Any())
            {
                Grid.ScrollIntoView(signals.First());
            }
        }

        #region Signal Edit
        public void OpenValueSet()
        {
            if (SelectedEditObj == null || !IsInWatch || SelectedEditObj.Value is Holding)
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
            Grid.CommitEdit(DataGridEditingUnit.Row, true);
            var newSignal = new SignalEditObj(signalType);
            var command = ListChangedCommand.Add(Signals, [newSignal]);
            RegistCommandEventHandle(command);
            command.AfterExecute += (_, _) => SetGridSelectedItems([newSignal]);
            UndoRedoManager.Run(command);
        }

        [RelayCommand]
        private void InsertSignal(Type signalType)
        {
            Grid.CommitEdit(DataGridEditingUnit.Row, true);
            var newSignal = new SignalEditObj(signalType);
            var command = ListChangedCommand.Insert(Signals, Grid.SelectedItems.Count == 0 ? -1 : Signals.IndexOf(Grid.SelectedItems.Cast<SignalEditObj>().First()), [newSignal]);
            RegistCommandEventHandle(command);
            command.AfterExecute += (_, _) => SetGridSelectedItems([newSignal]);
            UndoRedoManager.Run(command);
        }

        [RelayCommand]
        private void RemoveSignal(SignalEditObj signal)
        {
            var command = ListChangedCommand.Remove(Signals, [signal]);
            RegistCommandEventHandle(command);
            command.AfterUndo += (_, _) => SetGridSelectedItems([signal]);
            UndoRedoManager.Run(command);
        }

        [RelayCommand]
        private void RemoveSelectedSignals()
        {
            if (Signals.Count == 0 || Grid.SelectedItems.Count == 0)
            {
                return;
            }
            var removed = Grid.SelectedItems.Cast<SignalEditObj>().ToArray();
            var command = ListChangedCommand.Remove(Signals, removed);
            RegistCommandEventHandle(command);
            command.AfterUndo += (_, _) => SetGridSelectedItems(removed);
            UndoRedoManager.Run(command);
        }

        [RelayCommand]
        private void ClearSignals()
        {
            if (Signals.Count == 0)
            {
                return;
            }

            if (MessageBox.Show("确认要删除所有信号吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
            {
                return;
            }

            var command = ListChangedCommand.Clear(Signals);
            RegistCommandEventHandle(command);
            UndoRedoManager.Run(command);
        }

        public bool DragSignalsIsOne => DragSignals.Count == 1;

        [RelayCommand(CanExecute = nameof(DragSignalsIsOne))]
        private void ReplaceSignal()
        {
            if (DragSignals.Count != 1)
            {
                return;
            }

            var oldItem = DragTargetSignal;
            var newItem = DragSignals[0];

            ReplaceSignal(oldItem, newItem);
        }

        public void ReplaceSignal(SignalEditObj oldItem, SignalEditObj newItem)
        {
            int oldIndex = Signals.IndexOf(oldItem);
            int newIndex = Signals.IndexOf(newItem);

            if (oldIndex != newIndex)
            {
                var command = ListChangedCommand.Replace(Signals, [(oldItem, newItem), (newItem, oldItem)]);
                RegistCommandEventHandle(command);
                if (AfterDragEvent != null)
                {
                    command.AfterExecute += (_, _) =>
                    {
                        AfterDragEvent?.Invoke([newItem]);
                    };
                    command.AfterUndo += (_, _) =>
                    {
                        AfterDragEvent?.Invoke([newItem]);
                    };
                }
                UndoRedoManager.Run(command);
            }
        }

        /// <summary>
        /// 判断拖拽的项在原列表中是否是连续的
        /// </summary>
        /// <returns></returns>
        private bool IsDragsContinuous()
        {
            if (Signals.Count == 0)
            {
                return false;
            }
            var query = Signals.Select((signal, index) => (Signal: signal, Index: index)).IntersectBy(DragSignals, s => s.Signal).Select(s => s.Index).OrderBy(i => i);
            if (!query.Any())
            {
                return false;
            }

            var preIndex = query.First();
            var skipQuery = query.Skip(1);
            return !skipQuery.Any() || skipQuery.All(i =>
            {
                var result = Math.Abs(i - preIndex) == 1;
                preIndex = i;
                return result;
            });
        }

        public bool CanMoveBefore
        {
            get
            {
                if (DragTargetSignal == null || DragSignals.Count == 0)
                {
                    return false;
                }

                return !IsDragsContinuous() || (Signals.IndexOf(DragTargetSignal) - Signals.IndexOf(DragSignals.OrderBy(Signals.IndexOf).Last())) != 1;
            }
        }

        [RelayCommand(CanExecute = nameof(CanMoveBefore))]
        private void MoveSignlsBefore()
        {
            MoveSignals(DragTargetSignal, DragSignals.ToArray());
        }

        public bool CanMoveAfter
        {
            get
            {
                if (DragTargetSignal == null || DragSignals.Count == 0)
                {
                    return false;
                }

                return !IsDragsContinuous() || (Signals.IndexOf(DragSignals.OrderBy(Signals.IndexOf).First()) - Signals.IndexOf(DragTargetSignal)) != 1;
            }
        }

        [RelayCommand(CanExecute = nameof(CanMoveAfter))]
        private void MoveSignalsAfter()
        {
            var dragItems = DragSignals.ToArray();
            if (Signals.Last() == DragTargetSignal)
            {
                MoveSignals(null, dragItems);
            }
            else
            {
                MoveSignals(Signals[Signals.IndexOf(DragTargetSignal) + 1], dragItems);
            }
        }

        public void MoveSignals(SignalEditObj signal, IEnumerable<SignalEditObj> moved)
        {
            if (moved.Contains(signal))
            {
                return;
            }

            var command = ListChangedCommand.Move(Signals, signal, moved);
            RegistCommandEventHandle(command);
            if (AfterDragEvent != null)
            {
                command.AfterExecute += (_, _) =>
                {
                    AfterDragEvent?.Invoke(moved);
                };
                command.AfterUndo += (_, _) =>
                {
                    AfterDragEvent?.Invoke(moved);
                };
            }
            UndoRedoManager.Run(command);
        }

        private SignalSortBy? lastSignalSoryBy;

        [RelayCommand]
        private void OrderBy(SignalSortBy sortBy)
        {
            ICommand command;
            switch (sortBy)
            {
                case SignalSortBy.Name:
                    command = lastSignalSoryBy == SignalSortBy.Name ? ListChangedCommand.OrderByDescending(Signals, s => s.Value.Name) : ListChangedCommand.OrderBy(Signals, s => s.Value.Name);
                    break;
                case SignalSortBy.Address:
                    command = lastSignalSoryBy == SignalSortBy.Address ? ListChangedCommand.OrderByDescending(Signals, s => s.Value.Address) : ListChangedCommand.OrderBy(Signals, s => s.Value.Address);
                    break;
                case SignalSortBy.Type:
                    command = lastSignalSoryBy == SignalSortBy.Type ? ListChangedCommand.OrderByDescending(Signals, s => s.Other.Name) : ListChangedCommand.OrderBy(Signals, s => s.Other.Name);
                    break;
                default:
                    return;
            }
            RegistCommandEventHandle(command);
            var lastSignalSoryByCp = lastSignalSoryBy;
            command.AfterExecute += (_, _) =>
            {
                switch (lastSignalSoryBy)
                {
                    case SignalSortBy.Name:
                    case SignalSortBy.Address:
                    case SignalSortBy.Type:
                        lastSignalSoryBy = null;
                        break;
                    case null:
                        lastSignalSoryBy = sortBy;
                        break;
                }
            };
            command.AfterUndo += (_, _) =>
            {
                lastSignalSoryBy = lastSignalSoryByCp;
            };
            UndoRedoManager.Run(command);
        }
        #endregion

        #region Quick Cal Address
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


            foreach (var signal in signals.Skip(1))
            {
                if (SignalAddressUsed.TryGetValue(signal.Other, out var used))
                {
                    var preUsedItem = GetAddressUsedItem(preUsed, preSignal);
                    var usedItem = GetAddressUsedItem(used, signal);

                    var dbIndex = preAddress.DbIndex;
                    int index;
                    byte offset = 0;

                    if (signal.Value is Holding)
                    {
                        index = preAddress.Index + preUsedItem.IndexSize;
                    }
                    else
                    {
                        if (preUsedItem.IndexSize == 0 && usedItem.IndexSize == 0 && preSignal.Value is Bool && signal.Value is Bool)
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

                        if (index % 2 == 1 && (signal.Value is not Bool || !AllowBoolIndexHasOddNumber) && ForbidIndexHasOddNumber)
                        {
                            index += 1;
                        }
                    }

                    var newAddress = new SignalAddress(dbIndex, index, offset)
                    {
                        HideOffset = usedItem.IndexSize != 0 || signal.Value is Holding
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

        private IEnumerable<IEnumerable<SignalEditObj>> AssembleSignalByAddress(IEnumerable<SignalEditObj> target)
        {
            Dictionary<int, List<SignalEditObj>> signalGroupByDbIndex = new Dictionary<int, List<SignalEditObj>>();
            int preDbIndex = -1;
            foreach (var signal in target)
            {
                if (signal.Value.Address == null)
                {
                    if (preDbIndex != -1)
                    {
                        signalGroupByDbIndex[preDbIndex].Add(signal);
                    }
                    continue;
                }

                if (signalGroupByDbIndex.TryGetValue(signal.Value.Address.DbIndex, out var dbSignals))
                {
                    dbSignals.Add(signal);
                }
                else
                {
                    signalGroupByDbIndex.Add(signal.Value.Address.DbIndex, new List<SignalEditObj> { signal });
                }
                preDbIndex = signal.Value.Address.DbIndex;
            }
            return signalGroupByDbIndex.Values;
        }

        [RelayCommand]
        private void UpdateAddressFromFirst()
        {
            Grid.CommitEdit(DataGridEditingUnit.Row, true);

            UndoRedoManager.StartTransaction();

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(Signals);
                dbSignals.Each(UpdateAddress);
            }
            else
            {
                UpdateAddress(Signals);
            }

            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
        }

        [RelayCommand]
        private void UpdateAddressFromFirtSelected()
        {
            if (Grid.SelectedItems.Count == 0)
            {
                return;
            }

            Grid.CommitEdit(DataGridEditingUnit.Row, true);

            var signals = Signals.Skip(Signals.IndexOf(Grid.SelectedItems.Cast<SignalEditObj>().OrderBy(Signals.IndexOf).First()));

            UndoRedoManager.StartTransaction();

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(signals);
                dbSignals.Each(UpdateAddress);
            }
            else
            {
                UpdateAddress(signals);
            }

            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
        }

        [RelayCommand]
        private void UpdateAddressFromSelectedItems()
        {
            var signals = Grid.SelectedItems.Cast<SignalEditObj>();
            if (!signals.Any())
            {
                return;
            }

            Grid.CommitEdit(DataGridEditingUnit.Row, true);

            UndoRedoManager.StartTransaction();

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(signals);
                dbSignals.Each(UpdateAddress);
            }
            else
            {
                UpdateAddress(signals);
            }

            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
        }

        [RelayCommand]
        private void ClearAddress()
        {
            Grid.CommitEdit(DataGridEditingUnit.Row, true);

            UndoRedoManager.StartTransaction();

            Signals.Each(s =>
            {
                if (s.Value.Address != null)
                {
                    var command = new ValueChangedCommand<SignalAddress>(address =>
                    {
                        s.Value.Address = address;
                    }, s.Value.Address, null);
                    UndoRedoManager.Run(command);
                }
            });

            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
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
        CancellationTokenSource watchCancelSource;
        Task watchTask;
        public bool IsInWatch => watchTask != null && watchCancelSource != null && !watchCancelSource.IsCancellationRequested;
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
            if (watchCancelSource != null && !watchCancelSource.IsCancellationRequested)
            {
                EndWatch();
            }

            watchCancelSource = new CancellationTokenSource();
            watchTask = WatchTask(watchCancelSource.Token);
        }

        private void EndWatch()
        {
            if (watchCancelSource != null)
            {
                try
                {
                    watchCancelSource.Cancel();
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
