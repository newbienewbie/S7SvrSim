using MaterialDesignThemes.Wpf;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using System;
using System.Windows.Input;

namespace S7SvrSim.ViewModels
{
    public class DialogViewModel : ReactiveValidationObject
    {
        public string Title { get; }
        public string ErrorText { get; }
        public string SuffixText { get; set; }

        [Reactive]
        public string Text { get; set; }

        public event Func<string, bool> ValidationEvent;

        public ICommand AcceptCommand { get; }

        public DialogViewModel(string title, string errorText = null)
        {
            Title = title;
            ErrorText = errorText;

            this.ValidationRule(vm => vm.Text, text => ValidationEvent?.Invoke(text) ?? true, ErrorText ?? "error");

            AcceptCommand = ReactiveCommand.Create(Accept);
        }

        private void Accept()
        {
            if (!ValidationContext.GetIsValid()) return;

            DialogHost.Close("MainWindowDialogHost", Text);
        }
    }
}
