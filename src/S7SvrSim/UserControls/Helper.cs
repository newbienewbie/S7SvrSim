using System;
using System.Windows;
using System.Windows.Controls;

namespace S7SvrSim.UserControls
{
    public static class Helper
    {
        #region
        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }
        #endregion

        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(Helper), new PropertyMetadata(false, AutoScrollPropertyChanged, coerceValueCallback));

        private static object coerceValueCallback(DependencyObject d, object baseValue)
        {
            return baseValue;
        }

        private static void AutoScrollPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToBottom();
            }
        }
    }
}
