using S7SvrSim.ViewModels.Signals.SetBoxVM;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace S7SvrSim.UserControls.Signals.SetBox
{
    [TemplatePart(Name = nameof(PART_ValueBox), Type = typeof(TextBox))]
    public class LRealSignalSetBox : Control, IViewFor<LRealSignalSetBoxVM>
    {
        private TextBox PART_ValueBox;

        public LRealSignalSetBox()
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

        public LRealSignalSetBoxVM ViewModel { get => (LRealSignalSetBoxVM)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (LRealSignalSetBoxVM)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(LRealSignalSetBoxVM), typeof(LRealSignalSetBox), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            control.DataContext = e.NewValue;
        }
    }
}
