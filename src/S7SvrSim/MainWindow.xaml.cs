using S7Svr.Simulator.ViewModels;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using Splat;
using System;
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

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.New)
            {
            }
            else if (e.Command == ApplicationCommands.Open)
            {
            }
            else if (e.Command == ApplicationCommands.Save)
            {
            }
            else if (e.Command == ApplicationCommands.SaveAs)
            {
            }
            else if (e.Command == ApplicationCommands.Undo)
            {
                UndoRedoManager.Undo();
            }
            else if (e.Command == ApplicationCommands.Redo)
            {
                UndoRedoManager.Redo();
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

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ViewModel?.RunningVM != null)
            {
                e.CanExecute = UndoRedoManager.UndoCount > 0 && !ViewModel.RunningVM.RunningStatus;
                e.Handled = true;
            }
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ViewModel?.RunningVM != null)
            {
                e.CanExecute = UndoRedoManager.RedoCount > 0 && !ViewModel.RunningVM.RunningStatus;
                e.Handled = true;
            }
        }

        private void AreaConfigGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            if (e.NewItem is AreaConfigVM areaConfigVM)
            {
                var command = new IListChangedCommand<AreaConfigVM>(ViewModel.ConfigVM.AreaConfigs, ChangedType.Add, areaConfigVM);
                ViewModel.ConfigVM.CommandEventRegist(command);
                UndoRedoManager.Regist(command);
            }
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
