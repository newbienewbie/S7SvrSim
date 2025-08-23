using S7SvrSim.Shared;
using S7SvrSim.ViewModels.Signals.SetBoxVM;
using Splat;
using System;
using System.Windows;
using System.Windows.Input;

namespace S7SvrSim.UserControls.Signals
{
    /// <summary>
    /// SetSignalValueWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetSignalValueWindow : Window, IViewFor<SetBoxVMBase>
    {
#nullable enable
        private class ViewLocator : IViewLocator
        {
            public IViewFor ResolveView<T>(T? viewModel, string? contract = null)
            {
                if (viewModel == null)
                {
                    throw new ArgumentNullException(nameof(viewModel));
                }

                var viewType = typeof(IViewFor<>).MakeGenericType(viewModel.GetType());
                var view = Locator.Current.GetService(viewType);
                if (view == null)
                {
                    throw new InvalidOperationException($"Not regist view type: {viewType}");
                }

                var viewFor = (IViewFor)view;
                viewFor.ViewModel = viewModel;

                return viewFor;
            }
        }
#nullable restore

        private readonly IViewLocator viewLocator;
        public SetSignalValueWindow()
        {
            InitializeComponent();

            viewLocator = new ViewLocator();

            this.WhenActivated(d =>
            {
                if (ViewModel != null)
                {
                    ViewModel.AfterSetValue += ViewModel_AfterSetValue;
                }

                viewModelHost.Content = viewLocator.ResolveView(ViewModel);
                if (viewModelHost.Content is UIElement box)
                {
                    FocusManager.SetFocusedElement(this, box);
                }

                this.OneWayBind(ViewModel, vm => vm.SetValueCmd, v => v.btnOk.Command).DisposeWith(d);
            });
        }

        private void ViewModel_AfterSetValue()
        {
            Close();
        }

        public SetBoxVMBase ViewModel { get => (SetBoxVMBase)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (SetBoxVMBase)value; }
        public readonly static DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(SetBoxVMBase), typeof(SetSignalValueWindow), new PropertyMetadata(null));

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            var point = System.Windows.Forms.Control.MousePosition.ToWinPoint(this);
            point.X = point.X - ActualWidth / 2.0 > 0 ? point.X - ActualWidth / 2.0 : 0;
            point.Y = point.Y - (ActualHeight + 20) > 0 ? point.Y - (ActualHeight + 20) : 0;

            Left = point.X;
            Top = point.Y;

            Opacity = 100;

            FocusManager.SetFocusedElement(this, viewModelHost);
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
