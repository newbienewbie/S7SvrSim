using S7SvrSim.ViewModels.Signals.SetBoxVM;
using System.Windows;
using System.Windows.Controls;

namespace S7SvrSim.UserControls.Signals.SetBox
{
    [TemplatePart(Name = nameof(PART_ValueBox), Type = typeof(TextBox))]
    public class StringSignalSetBox : Control, IViewFor<StringSignalSetBoxVM>
    {
        private TextBox PART_ValueBox;

        public StringSignalSetBox()
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

        public StringSignalSetBoxVM ViewModel { get => (StringSignalSetBoxVM)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (StringSignalSetBoxVM)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(StringSignalSetBoxVM), typeof(StringSignalSetBox), new PropertyMetadata(null, OnViewModelChanged));

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (Control)d;
            control.DataContext = e.NewValue;
        }
    }
}
