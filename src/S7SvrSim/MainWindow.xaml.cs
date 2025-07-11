using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Commands;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using Splat;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace S7Svr.Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainVM>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.ViewModel = Locator.Current.GetRequiredService<MainVM>();
                this.DataContext = this.ViewModel;
                this.OneWayBind(ViewModel, vm => vm.ProjectVM.NeedSave, w => w.Title, GetTitle).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ProjectVM.UndoCount, win => win.undoBadged.Badge).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.ProjectVM.RedoCount, win => win.redoBadged.Badge).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.RunningVM.RunningStatus, w => w.activeBlock.Visibility, isRun => isRun ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);

                this.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, (_, _) => ViewModel.ProjectVM.NewProject(), NotRunningStatus_CanExecute));
                this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, (_, _) => ViewModel.ProjectVM.LoadProject(), NotRunningStatus_CanExecute));
                this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, (_, _) => ViewModel.ProjectVM.SaveProject(), CanExecuteTrue));
                this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, (_, _) => ViewModel.ProjectVM.SaveProjectAs(), CanExecuteTrue));
                this.CommandBindings.Add(new CommandBinding(AppCommands.OpenFolder, (_, _) => ViewModel.ProjectVM.OpenProjectFolder(), CanExecuteTrue));
                this.InputBindings.Add(new KeyBinding(ViewModel.CmdStartServer, new KeyGesture(Key.F5)));
                this.InputBindings.Add(new KeyBinding(ViewModel.CmdStopServer, new KeyGesture(Key.F5, ModifierKeys.Shift)));
                this.InputBindings.Add(new KeyBinding(ViewModel.CmdRestartServer, new KeyGesture(Key.F5, ModifierKeys.Control)));
                this.InputBindings.Add(new KeyBinding(ViewModel.ProjectVM.UndoCommand, new KeyGesture(Key.Z, ModifierKeys.Control)));
                this.InputBindings.Add(new KeyBinding(ViewModel.ProjectVM.RedoCommand, new KeyGesture(Key.Y, ModifierKeys.Control)));
            });
        }

        #region
        public MainVM ViewModel
        {
            get { return (MainVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (MainVM)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainVM), typeof(MainWindow), new PropertyMetadata(null));
        #endregion

        private string GetTitle(bool needSave)
        {
            if (needSave)
            {
                return $"* {ViewModel.ProjectVM.ProjectName} - Siemens PLC 通讯模拟器";
            }
            else
            {
                return $"{ViewModel.ProjectVM.ProjectName} - Siemens PLC 通讯模拟器";
            }
        }

        private void NotRunningStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ViewModel?.RunningVM != null)
            {
                e.CanExecute = !ViewModel.RunningVM.RunningStatus;
                e.Handled = true;
            }
        }

        private void CanExecuteTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void AreaConfigGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            if (e.NewItem is AreaConfigVM areaConfigVM)
            {
                var command = ListChangedCommand<AreaConfigVM>.Add(ViewModel.ConfigVM.AreaConfigs, [areaConfigVM]);
                ViewModel.ConfigVM.CommandEventRegist(command);
                UndoRedoManager.Regist(command);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel.ProjectVM.NotifyIfSave() == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

        public void SwitchTab(int index)
        {
            if (index < tabControl.Items.Count)
            {
                tabControl.SelectedIndex = index;
            }
        }
    }
}
