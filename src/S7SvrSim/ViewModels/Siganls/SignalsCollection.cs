using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using S7SvrSim.Services.Settings;
using S7SvrSim.Shared;
using S7SvrSim.UserControls.Signals;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace S7SvrSim.ViewModels.Siganls
{
    public partial class SignalsCollection : ViewModelBase
    {
        private readonly IMemCache<WatchState> watchState;
        private readonly SignalsHelper signalsHelper;

        public DataGrid Grid { get; set; }

        [Reactive]
        public string GroupName { get; set; }

        public ObservableCollection<SignalEditGroup> SignalGroups { get; } = new ObservableCollection<SignalEditGroup>();

        public ObservableCollection<SignalEditObj> Signals => SignalGroups.Where(sg => sg.Name == GroupName).FirstOrDefault()?.Signals;

        [ObservableProperty]
        private SignalEditObj selectedEditObj;

        public bool UpdateAddressByDbIndex { get; set; }

        public SignalsCollection(IMemCache<WatchState> watchState, ISetting<UpdateAddressOptions> setting, SignalsHelper signalsHelper)
        {
            this.watchState = watchState;
            this.signalsHelper = signalsHelper;

            this.WhenAnyValue(signals => signals.GroupName).Subscribe(_ => OnPropertyChanged(nameof(Signals)));

            setting.Value.Subscribe(options =>
            {
                UpdateAddressByDbIndex = true;
            });
        }

        private void CommandEventHandle(object _object, EventArgs _args)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).SwitchTab(2);
        }

        private void RegistCommandEventHandle(IHistoryCommand command)
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

        public void OpenValueSet()
        {
            if (SelectedEditObj == null || SelectedEditObj.Value.Address == null || !watchState.Value.IsInWatch || SelectedEditObj.Value is Holding)
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
            var newSignal = new SignalEditObj(signalType);
            var command = ListChangedCommand.Add(Signals, [newSignal]);
            RegistCommandEventHandle(command);
            command.AfterExecute += (_, _) => SetGridSelectedItems([newSignal]);
            UndoRedoManager.Run(command);
        }

        [RelayCommand]
        private void InsertSignal(Type signalType)
        {
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

        private SignalSortBy? lastSignalSoryBy;

        [RelayCommand]
        private void OrderBy(SignalSortBy sortBy)
        {
            IHistoryCommand command;
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

        #region Quick Cal Address
        private IEnumerable<IEnumerable<SignalBase>> AssembleSignalByAddress(IEnumerable<SignalEditObj> target)
        {
            Dictionary<int, List<SignalBase>> signalGroupByDbIndex = new Dictionary<int, List<SignalBase>>();
            int preDbIndex = -1;
            foreach (var signal in target)
            {
                if (signal.Value.Address == null)
                {
                    if (preDbIndex != -1)
                    {
                        signalGroupByDbIndex[preDbIndex].Add(signal.Value);
                    }
                    continue;
                }

                if (signalGroupByDbIndex.TryGetValue(signal.Value.Address.DbIndex, out var dbSignals))
                {
                    dbSignals.Add(signal.Value);
                }
                else
                {
                    signalGroupByDbIndex.Add(signal.Value.Address.DbIndex, new List<SignalBase> { signal.Value });
                }
                preDbIndex = signal.Value.Address.DbIndex;
            }
            return signalGroupByDbIndex.Values;
        }

        [RelayCommand]
        private void UpdateAddressFromFirst()
        {
            UndoRedoManager.StartTransaction();

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(Signals);
                dbSignals.Each(signals => signalsHelper.UpdateAddress(signals));
            }
            else
            {
                signalsHelper.UpdateAddress(Signals.Select(s => s.Value));
            }

            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
        }

        [RelayCommand]
        private void UpdateAddressFromFirstSelected()
        {
            if (Grid.SelectedItems.Count == 0)
            {
                return;
            }

            var signals = Signals.Skip(Signals.IndexOf(Grid.SelectedItems.Cast<SignalEditObj>().OrderBy(Signals.IndexOf).First()));

            UndoRedoManager.StartTransaction();

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(signals);
                dbSignals.Each(signals => signalsHelper.UpdateAddress(signals));
            }
            else
            {
                signalsHelper.UpdateAddress(signals.Select(s => s.Value));
            }

            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
        }

        [RelayCommand]
        private void UpdateAddressFromSelectedItems()
        {
            var signals = Grid.SelectedItems.Cast<SignalEditObj>().OrderBy(Signals.IndexOf);
            if (!signals.Any())
            {
                return;
            }

            UndoRedoManager.StartTransaction();

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(signals);
                dbSignals.Each(signals => signalsHelper.UpdateAddress(signals));
            }
            else
            {
                signalsHelper.UpdateAddress(signals.Select(s => s.Value));
            }

            var command = UndoRedoManager.EndTransaction();
            RegistCommandEventHandle(command);
        }

        [RelayCommand]
        private void ClearAddress()
        {
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
    }
}
