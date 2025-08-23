using S7SvrSim.ViewModels.Signals.SetBoxVM;
using System.Windows;
using System.Windows.Controls;

namespace S7SvrSim.UserControls.Signals.SetBox
{
    [TemplatePart(Name = nameof(PART_ValueBox), Type = typeof(TextBox))]
    public class UIntSignalSetBox : Control, IViewFor<UIntSignalSetBoxVM>
    {
        private TextBox PART_ValueBox;

        public UIntSignalSetBox()
        {
            this.WhenActivated(d =>
            {
                PART_ValueBox = (TextBox)GetTemplateChild(nameof(PART_ValueBox));
                ViewModel.HasValidationError = () =>
                {
                    if (PART_ValueBox != null)
                    {
                        return Validation.GetHasError(PART_ValueBox);
                    }
                    return false;
                };
            });
        }

        public UIntSignalSetBoxVM ViewModel { get => (UIntSignalSetBoxVM)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (UIntSignalSetBoxVM)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(UIntSignalSetBoxVM), typeof(UIntSignalSetBox), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            control.DataContext = e.NewValue;
        }
    }
}
