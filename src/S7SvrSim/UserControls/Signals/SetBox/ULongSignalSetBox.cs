using S7SvrSim.ViewModels.Signals.SetBoxVM;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace S7SvrSim.UserControls.Signals.SetBox
{
    [TemplatePart(Name = nameof(PART_ValueBox), Type = typeof(TextBox))]
    public class ULongSignalSetBox : Control, IViewFor<ULongSignalSetBoxVM>
    {
        private TextBox PART_ValueBox;

        public ULongSignalSetBox()
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
                ViewModel.ErrorReFocus = () =>
                {
                    Focus();
                };
            });
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            FocusManager.SetFocusedElement(this, PART_ValueBox);
        }

        public ULongSignalSetBoxVM ViewModel { get => (ULongSignalSetBoxVM)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (ULongSignalSetBoxVM)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ULongSignalSetBoxVM), typeof(ULongSignalSetBox), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            control.DataContext = e.NewValue;
        }
    }
}
