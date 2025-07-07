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

        [Reactive]
        private bool ValidateTrigger { get; set; }

        public event Func<string, bool> ValidationEvent;

        public ICommand AcceptCommand { get; }

        public DialogViewModel(string title, string errorText = null)
        {
            Title = title;
            ErrorText = errorText;

            this.ValidationRule(vm => vm.Text,
                this.WhenAnyValue(vm => vm.Text, vm => vm.ValidateTrigger, (text, _) => text),
                text => ValidationEvent?.Invoke(text) ?? true,
                _ => ErrorText ?? "Error");

            AcceptCommand = ReactiveCommand.Create(Accept);
        }

        private void TriggerValidate()
        {
            ValidateTrigger = !ValidateTrigger;
        }

        private void Accept()
        {
            TriggerValidate();

            if (!ValidationContext.GetIsValid()) return;

            DialogHost.Close("MainWindowDialogHost", Text);
        }
    }
}
