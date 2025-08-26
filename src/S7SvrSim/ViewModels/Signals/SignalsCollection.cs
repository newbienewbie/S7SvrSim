using DynamicData;
using ReactiveUI.Fody.Helpers;
using S7SvrSim.Messages;
using S7SvrSim.S7Signal;
using S7SvrSim.Services;
using S7SvrSim.Services.Settings;
using S7SvrSim.Shared;
using S7SvrSim.UserControls.Signals;
using S7SvrSim.ViewModels.Signals.SetBoxVM;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace S7SvrSim.ViewModels.Signals
{
    public partial class SignalsCollection : ReactiveObject
    {
        private readonly IMemCache<WatchState> watchState;
        private readonly SignalsHelper signalsHelper;
        private readonly MediatR.IMediator mediator;

        public DataGrid Grid { get; set; }

        [Reactive]
        public string GroupName { get; set; }

        public ObservableCollection<SignalEditGroup> SignalGroups { get; } = new ObservableCollection<SignalEditGroup>();

        public ObservableCollection<SignalEditObj> Signals => SignalGroups.Where(sg => sg.Name == GroupName).FirstOrDefault()?.Signals;

        public bool IsSignalsNotNull => Signals != null;

        [Reactive]
        public SignalEditObj SelectedEditObj { get; set; }

        public bool UpdateAddressByDbIndex { get; set; }

        [Reactive]
        public string NewGroupName { get; set; }

        public ICommand AddGroupCommand { get; }
        public ReactiveCommand<string, Unit> SwitchGroupCommand { get; }
        public ReactiveCommand<SignalEditGroup, Unit> DeleteGroupCommand { get; }
        public ReactiveCommand<SignalEditGroup, Unit> RenameGroupCommand { get; }
        public ReactiveCommand<SignalEditGroup, Unit> CopyGroupCommand { get; }

        public ICommand NewSignalCommand { get; }
        public ICommand InsertSignalCommand { get; }
        public ICommand RemoveSelectedSignalsCommand { get; }
        public ICommand RemoveSignalCommand { get; }
        public ICommand ClearSignalsCommand { get; }
        public ICommand CopySignalsCommand { get; }
        public ICommand PasteSignalsCommand { get; }

        public ICommand UpdateAddressFromFirstCommand { get; }
        public ICommand UpdateAddressFromFirstSelectedCommand { get; }
        public ICommand UpdateAddressFromSelectedItemsCommand { get; }
        public ICommand ClearAddressCommand { get; }

        public SignalsCollection(IMemCache<WatchState> watchState, ISetting<UpdateAddressOptions> setting, SignalsHelper signalsHelper, MediatR.IMediator mediator)
        {
            this.watchState = watchState;
            this.signalsHelper = signalsHelper;
            this.mediator = mediator;

            setting.Value.Subscribe(options =>
            {
                UpdateAddressByDbIndex = options.UpdateAddressByDbIndex;
            });

            var watchGroupName = this.WhenAnyValue(vm => vm.GroupName);
            watchGroupName.Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(GroupName));
                this.RaisePropertyChanged(nameof(Signals));
            });

            var watchCanEditSignal = watchGroupName.Select(name => !string.IsNullOrEmpty(name));
            var watchCanAddGroup = this.WhenAnyValue(vm => vm.NewGroupName).Select(name => !string.IsNullOrWhiteSpace(name));

            AddGroupCommand = ReactiveCommand.Create(AddGroup, watchCanAddGroup);
            SwitchGroupCommand = ReactiveCommand.Create<string>(SwitchGroup);
            DeleteGroupCommand = ReactiveCommand.Create<SignalEditGroup>(DeleteGroup);
            RenameGroupCommand = ReactiveCommand.CreateFromTask<SignalEditGroup>(RenameGroup);
            CopyGroupCommand = ReactiveCommand.CreateFromTask<SignalEditGroup>(CopyGroup);

            NewSignalCommand = ReactiveCommand.Create<Type>(NewSignal, watchCanEditSignal);
            InsertSignalCommand = ReactiveCommand.Create<Type>(InsertSignal, watchCanEditSignal);
            RemoveSelectedSignalsCommand = ReactiveCommand.Create(RemoveSelectedSignals, watchCanEditSignal);
            RemoveSignalCommand = ReactiveCommand.Create<SignalEditObj>(RemoveSignal, watchCanEditSignal);
            ClearSignalsCommand = ReactiveCommand.Create(ClearSignals, watchCanEditSignal);
            CopySignalsCommand = ReactiveCommand.Create(CopySignals, watchCanEditSignal);
            PasteSignalsCommand = ReactiveCommand.Create(PasteSignals, watchCanEditSignal);

            UpdateAddressFromFirstCommand = ReactiveCommand.Create(UpdateAddressFromFirst, watchCanEditSignal);
            UpdateAddressFromFirstSelectedCommand = ReactiveCommand.Create(UpdateAddressFromFirstSelected, watchCanEditSignal);
            UpdateAddressFromSelectedItemsCommand = ReactiveCommand.Create(UpdateAddressFromSelectedItems, watchCanEditSignal);
            ClearAddressCommand = ReactiveCommand.Create(ClearAddress, watchCanEditSignal);
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

        #region Group Edit
        private async Task<ShowDialogResult> GetNewGroupName(string title, string oldName, bool checkOldName = false)
        {
            var dialogViewModel = new DialogViewModel(title, "名称不能为空或重复")
            {
                Text = oldName,
            };
            dialogViewModel.ValidationEvent += (rawValue) =>
            {
                if (rawValue is string value)
                {
                    if (string.IsNullOrEmpty(value)) return false;

                    if (checkOldName) return !SignalGroups.Any(s => s.Name == value);
                    else return !SignalGroups.Where(s => s.Name != oldName).Any(s => s.Name == value);
                }

                return true;
            };

            var renameResult = await mediator.Send(new ShowDialogRequest(dialogViewModel));
            return renameResult;
        }

        private void AddGroup()
        {
            if (string.IsNullOrEmpty(NewGroupName) || SignalGroups.Any(sg => sg.Name == NewGroupName)) return;

            var newGroupName = NewGroupName;
            var currentGroupName = GroupName;

            SignalGroups.Add(new SignalEditGroup(newGroupName, []));

            NewGroupName = "";
            this.RaisePropertyChanged(nameof(NewGroupName));
        }

        private void SwitchGroup(string name)
        {
            GroupName = name;
        }

        private void DeleteGroup(SignalEditGroup sg)
        {
            if (MessageBox.Show("是否删除？", "请确认", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

            if (sg == null) return;

            SignalGroups.Remove(sg);

            var deleteName = sg.Name;
            if (deleteName == GroupName || GroupName == null)
            {
                GroupName = SignalGroups.FirstOrDefault()?.Name;
            }
        }

        private async Task RenameGroup(SignalEditGroup sg)
        {
            if (sg == null) return;

            var oldName = sg.Name;

            var renameResult = await GetNewGroupName("Rename Group", oldName);
            if (renameResult.IsCancel || string.IsNullOrEmpty(renameResult.Result)) return;

            var newName = renameResult.Result;
            if (oldName == newName) return;

            sg.Name = newName;
        }

        private async Task CopyGroup(SignalEditGroup sg)
        {
            if (sg == null) return;

            var renameResult = await GetNewGroupName("Rename Group", sg.Name, true);
            if (renameResult.IsCancel || string.IsNullOrEmpty(renameResult.Result)) return;

            var newSg = new SignalEditGroup(renameResult.Result, sg.Signals);
            SignalGroups.Insert(SignalGroups.IndexOf(sg) + 1, newSg);
        }
        #endregion

        #region Signal Edit
        public void OpenValueSet()
        {
            if (SelectedEditObj == null || SelectedEditObj.Value.Address == null || !watchState.Value.IsInWatch || SelectedEditObj.Value is HoldingSignal)
            {
                return;
            }
            var valueType = GetSignalValueType(SelectedEditObj.Value);
            var viewModelType = typeof(SetBoxVMBase<>).MakeGenericType(valueType);
            var viewModel = (SetBoxVMBase)Locator.Current.GetService(viewModelType);
            viewModel.Signal = SelectedEditObj.Value;

            if (viewModel == null) return;

            var setWindow = new SetSignalValueWindow() { ViewModel = viewModel };

            setWindow.ShowDialog();
        }

        private Type GetSignalValueType(SignalBase signal)
        {
            var signalType = signal.GetType();
            var signalVaueTypeAttribute = signalType.GetCustomAttribute<SignalVaueTypeAttribute>();
            if (signalVaueTypeAttribute == null)
            {
                return null;
            }

            return signalVaueTypeAttribute.ValueType;
        }

        private void NewSignal(Type signalType)
        {
            var newSignal = new SignalEditObj(signalType);
            Signals.Add(newSignal);
        }

        private void InsertSignals(IEnumerable<SignalEditObj> objs)
        {
            var indexInsert = (Grid.SelectedItems.Count == 0) ? -1 : Signals.IndexOf(Grid.SelectedItems.Cast<SignalEditObj>().OrderBy(Signals.IndexOf).Last()) + 1;
            Signals.AddOrInsertRange(objs, indexInsert);
            SetGridSelectedItems(objs);
        }

        private void InsertSignal(Type signalType)
        {
            var newSignal = new SignalEditObj(signalType);
            InsertSignals([newSignal]);
        }

        private void RemoveSignal(SignalEditObj signal)
        {
            Signals.Remove(signal);
        }

        private void RemoveSelectedSignals()
        {
            if (Signals.Count == 0 || Grid.SelectedItems.Count == 0)
            {
                return;
            }
            var removed = Grid.SelectedItems.Cast<SignalEditObj>().ToArray();
            Signals.Remove(removed);
        }

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

            Signals.Clear();
        }

        private void CopySignals()
        {
            if (Grid.SelectedItems.Count == 0) return;

            var signals = Grid.SelectedItems.Cast<SignalEditObj>().OrderBy(Signals.IndexOf);
            Clipboard.SetText(signals.ToXml(), TextDataFormat.Xaml);
        }

        private void PasteSignals()
        {
            var clipData = Clipboard.GetText(TextDataFormat.Xaml);
            IEnumerable<SignalEditObj> signals;
            try
            {
                signals = clipData.FromXml(signalsHelper);
            }
            catch (Exception)
            {
                return;
            }
            InsertSignals(signals);
        }
        #endregion

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

        private void UpdateAddressFromFirst()
        {
            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(Signals);
                dbSignals.Each(signals => signalsHelper.UpdateAddress(signals));
            }
            else
            {
                signalsHelper.UpdateAddress(Signals.Select(s => s.Value));
            }
        }

        private void UpdateAddressFromFirstSelected()
        {
            if (Grid.SelectedItems.Count == 0)
            {
                return;
            }

            var signals = Signals.Skip(Signals.IndexOf(Grid.SelectedItems.Cast<SignalEditObj>().OrderBy(Signals.IndexOf).First()));

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(signals);
                dbSignals.Each(signals => signalsHelper.UpdateAddress(signals));
            }
            else
            {
                signalsHelper.UpdateAddress(signals.Select(s => s.Value));
            }
        }

        private void UpdateAddressFromSelectedItems()
        {
            var signals = Grid.SelectedItems.Cast<SignalEditObj>().OrderBy(Signals.IndexOf);
            if (!signals.Any())
            {
                return;
            }

            if (UpdateAddressByDbIndex)
            {
                var dbSignals = AssembleSignalByAddress(signals);
                dbSignals.Each(signals => signalsHelper.UpdateAddress(signals));
            }
            else
            {
                signalsHelper.UpdateAddress(signals.Select(s => s.Value));
            }
        }

        private void ClearAddress()
        {
            Signals.Each(s =>
            {
                if (s.Value.Address != null)
                {
                    s.Value.Address = null;
                }
            });
        }
        #endregion
    }
}
