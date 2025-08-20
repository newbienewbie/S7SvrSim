using Microsoft.Xaml.Behaviors.Core;
using System.Windows;
using System.Windows.Input;

namespace S7SvrSim.Commands
{
    public static class AppCommands
    {
        public static RoutedUICommand OpenFolder { get; } = new RoutedUICommand("打开所在目录", "OpenFolder", typeof(AppCommands));
        public static ActionCommand LostFocus { get; } = new ActionCommand(() =>
        {
            if (Keyboard.FocusedElement != null)
            {
                Keyboard.FocusedElement.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
                Keyboard.ClearFocus();
            }
        });
    }
}
