using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace S7SvrSim.Commands
{
    public static class AppCommands
    {
        public static ActionCommand LostFocus { get; } = new ActionCommand(() =>
        {
            LostFocusImpl(null);
        });

        internal static void LostFocusImpl(Type[] includeType)
        {
            if (Keyboard.FocusedElement != null && (includeType == null || includeType.Any(ty => Keyboard.FocusedElement.GetType() == ty)))
            {
                Keyboard.FocusedElement.RaiseEvent(new RoutedEventArgs(UIElement.LostFocusEvent));
                Keyboard.ClearFocus();
            }
        }
    }
}
