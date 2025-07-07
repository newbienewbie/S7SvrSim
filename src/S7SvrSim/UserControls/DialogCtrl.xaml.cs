using ReactiveUI.Validation.Extensions;
using S7SvrSim.ViewModels;
using System.Windows;

namespace S7SvrSim.UserControls
{
    /// <summary>
    /// DialogCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class DialogCtrl : IViewFor<DialogViewModel>
    {
        public DialogCtrl(DialogViewModel dialogViewModel)
        {
            InitializeComponent();
            DataContext = dialogViewModel;
            ViewModel = dialogViewModel;

            this.WhenActivated(d =>
            {
                this.BindWithValidation(ViewModel, vm => vm.Text, view => view.inputTextBox.Text).DisposeWith(d);
                //this.BindValidation(ViewModel, view => view.errorText.Text).DisposeWith(d);
                //this.WhenAnyValue(view => view.errorText.Text, text => !string.IsNullOrEmpty(text))
                //    .BindTo(this, view => view.errorText.Visibility)
                //    .DisposeWith(d);
            });
        }

        public DialogViewModel ViewModel { get => (DialogViewModel)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (DialogViewModel)value; }

        public static DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(DialogViewModel), typeof(DialogCtrl), new PropertyMetadata(null));
    }
}
