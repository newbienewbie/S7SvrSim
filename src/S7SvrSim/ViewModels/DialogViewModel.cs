using ReactiveUI.Fody.Helpers;
using System.Globalization;
using System.Windows.Controls;
using static Less.Utils.WPF.EventValidation;

namespace S7SvrSim.ViewModels
{
    public class DialogViewModel : ReactiveObject
    {
        public string Title { get; }
        public string SuffixText { get; set; }

        [Reactive]
        public string Text { get; set; }

        public event ValidationEvent ValidationEvent;

        public DialogViewModel(string title)
        {
            Title = title;
        }

        public ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return ValidationEvent?.Invoke(value, cultureInfo);
        }
    }
}
