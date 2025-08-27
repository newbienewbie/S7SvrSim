using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Services;
using Splat;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows;

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
                this.OneWayBind(ViewModel, vm => vm.RunningVM.RunningStatus, v => v.activeBlock.Visibility, isRun => isRun ? Visibility.Visible : Visibility.Collapsed).DisposeWith(d);
                this.ViewModel.NeedSaveChangedEventObservable
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Subscribe(evt => Title = GetTitle(evt.EventArgs.NeedSave))
                    .DisposeWith(d);
                Title = GetTitle(ViewModel.SaveNotifier.NeedSave);
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

        private string GetTitle(bool? needSave)
        {
            if (needSave== true)
            {
                return $"* {ViewModel.ProjectVM.ProjectName} - Siemens PLC 通讯模拟器";
            }
            else
            {
                return $"{ViewModel.ProjectVM.ProjectName} - Siemens PLC 通讯模拟器";
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
