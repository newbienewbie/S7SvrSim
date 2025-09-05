using DynamicData;
using ReactiveUI.Fody.Helpers;
using S7SvrSim.Shared;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace S7SvrSim.ViewModels.Signals
{
    public partial class DragSignalsVM : ReactiveObject
    {
        private readonly SignalsCollection signalsCollection;
        private IList<SignalEditObj> Signals => signalsCollection.Signals;

        [Reactive]
        public SignalEditObj DragTargetSignal { get; set; }

        [Reactive]
        public bool IsDragSignals { get; set; }

        [ObservableAsProperty]
        public bool DragSignalsIsOne { get; }

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

        public ObservableCollection<SignalEditObj> DragSignals { get; } = [];

        public ICommand ReplaceSignalCommand { get; }
        public ICommand MoveSignlsBeforeCommand { get; }
        public ICommand MoveSignalsAfterCommand { get; }

        public DragSignalsVM()
        {
            signalsCollection = Locator.Current.GetRequiredService<SignalsCollection>();

            DragSignals.CollectionChanged += (_, _) =>
            {
                this.RaisePropertyChanged(nameof(CanMoveAfter));
                this.RaisePropertyChanged(nameof(CanMoveBefore));
            };

            this.WhenAnyValue(vm => vm.DragTargetSignal).Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(CanMoveAfter));
                this.RaisePropertyChanged(nameof(CanMoveBefore));
            });

            DragSignals.WhenAnyValue(signals => signals.Count)
                       .Select(count => count == 1)
                       .ToPropertyEx(this, vm => vm.DragSignalsIsOne, scheduler: RxApp.MainThreadScheduler);

            var watchDragSignalsIsOne = this.WhenAnyValue(vm => vm.DragSignalsIsOne);
            var watchCanMoveBefore = this.WhenAnyValue(vm => vm.CanMoveBefore);
            var watchCanMoveAfter = this.WhenAnyValue(vm => vm.CanMoveAfter);

            ReplaceSignalCommand = ReactiveCommand.Create(ReplaceSignal, watchDragSignalsIsOne);
            MoveSignlsBeforeCommand = ReactiveCommand.Create(MoveSignlsBefore, watchCanMoveBefore);
            MoveSignalsAfterCommand = ReactiveCommand.Create(MoveSignalsAfter, watchCanMoveAfter);
        }

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
                Signals.Swap(oldItem, newItem);
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

        private void MoveSignlsBefore()
        {
            var dragItems = DragSignals.OrderBy(Signals.IndexOf).ToArray();
            MoveSignals(DragTargetSignal, dragItems);
        }

        private void MoveSignalsAfter()
        {
            var dragItems = DragSignals.OrderBy(Signals.IndexOf).ToArray();
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

            Signals.RemoveMany(moved);

            var indexOfSignal = Signals.IndexOf(signal);
            Signals.AddOrInsertRange(moved, indexOfSignal);
        }
    }
}
