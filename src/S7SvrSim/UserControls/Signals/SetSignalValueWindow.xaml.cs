using S7SvrSim.Shared;
using S7SvrSim.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace S7SvrSim.UserControls.Signals
{
    /// <summary>
    /// SetSignalValueWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetSignalValueWindow : Window, IViewFor<SetSignalValueVM>
    {
        public SetSignalValueWindow()
        {
            InitializeComponent();
        }

        public SetSignalValueVM ViewModel { get => viewModel; set => viewModel = value; }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (SetSignalValueVM)value; }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var point = System.Windows.Forms.Control.MousePosition.ToWinPoint(this);
            point.X = point.X - ActualWidth / 2.0 > 0 ? point.X - ActualWidth / 2.0 : 0;
            point.Y = point.Y - (ActualHeight + 20) > 0 ? point.Y - (ActualHeight + 20) : 0;

            Left = point.X;
            Top = point.Y;

            Opacity = 100;

            FocusManager.SetFocusedElement(this, setValueBox);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CanExecuteTrue(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void CloseCommand_Execute(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
