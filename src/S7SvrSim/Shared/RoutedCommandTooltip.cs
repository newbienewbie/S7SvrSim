using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace S7SvrSim.Shared
{
    /// <summary>
    /// <para>When <see cref="GetEnabled(DependencyObject)"/> is <see langword="true"/> and <see cref="FrameworkElement.ToolTip"/> is <see langword="null"/>, it will try to get <c>Command</c> and set the tooltip if <c>Command</c> <see langword="is"/> <see cref="RoutedCommand"/> or <see cref="RoutedUICommand"/></para>
    /// </summary>
    public static class RoutedCommandTooltip
    {
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(RoutedCommandTooltip), new PropertyMetadata(false, OnEnabledChanged));

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
            {
                return;
            }

            if (d is FrameworkElement element)
            {
                var elementType = element.GetType();
                var commandPropertyField = elementType.GetField("CommandProperty", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
                var commandPropertyValue = commandPropertyField?.GetValue(d);

                if (commandPropertyValue is DependencyProperty commandProperty)
                {
                    var descriptor = DependencyPropertyDescriptor.FromProperty(commandProperty, elementType);
                    if ((bool)e.NewValue)
                    {
                        descriptor.AddValueChanged(d, OnCommandChanged);
                        UpdateTooltip(element);
                    }
                    else
                    {
                        descriptor.RemoveValueChanged(d, OnCommandChanged);
                    }
                }
            }
        }

        private static void OnCommandChanged(object sender, EventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                UpdateTooltip(element);
            }
        }

        private static void UpdateTooltip(FrameworkElement element)
        {
            if (element.ToolTip == null)
            {
                var elementType = element.GetType();
                var commandPropertyField = elementType.GetField("CommandProperty", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField);
                var commandPropertyValue = commandPropertyField?.GetValue(element);

                if (commandPropertyValue is DependencyProperty commandProperty && element.GetValue(commandProperty) is RoutedCommand command)
                {
                    string tooltipText = "";

                    if (command is RoutedUICommand uiCommand)
                    {
                        tooltipText = uiCommand.Text.Trim('"', '“', '”', ' ');
                    }

                    if (command.InputGestures.Count > 0 && command.InputGestures[0] is KeyGesture gesture)
                    {
                        tooltipText = string.IsNullOrEmpty(tooltipText) ? $"{gesture.DisplayString}" : $"{tooltipText} ({gesture.DisplayString})";
                    }

                    element.ToolTip = tooltipText;
                }
            }
        }
    }
}
