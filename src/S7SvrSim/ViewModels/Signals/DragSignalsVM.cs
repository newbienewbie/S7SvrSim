using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ReactiveUI.Fody.Helpers;
using S7Svr.Simulator;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace S7SvrSim.ViewModels.Signals
{
    public partial class DragSignalsVM : ViewModelBase
    {
        private readonly SignalsCollection signalsCollection;
        private IList<SignalEditObj> Signals => signalsCollection.Signals;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanMoveAfter))]
        [NotifyPropertyChangedFor(nameof(CanMoveBefore))]
        private SignalEditObj dragTargetSignal;

        [Reactive]
        public bool IsDragSignals { get; set; }
        public ObservableCollection<SignalEditObj> DragSignals { get; } = [];

        public event Action<IEnumerable<SignalEditObj>> AfterDragEvent;

        public DragSignalsVM()
        {
            signalsCollection = Locator.Current.GetRequiredService<SignalsCollection>();

            DragSignals.CollectionChanged += (_, _) =>
            {
                OnPropertyChanged(nameof(DragSignalsIsOne));
                OnPropertyChanged(nameof(CanMoveAfter));
                OnPropertyChanged(nameof(CanMoveBefore));
            };
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
                MoveSignals(Signals[(int)(Signals.IndexOf((SignalEditObj)this.DragTargetSignal) + 1)], dragItems);
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
    }
}
