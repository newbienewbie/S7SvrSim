using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace S7SvrSim.UserControls
{
    public class CustomTag : ContentControl
    {
        public static DependencyProperty MarkForegroundProperty = DependencyProperty.Register("MarkForeground", typeof(Brush), typeof(CustomTag), new PropertyMetadata(Brushes.DodgerBlue));
        public static DependencyProperty SuffixProperty = DependencyProperty.Register("Suffix", typeof(object), typeof(CustomTag), new PropertyMetadata(null));
        public static DependencyProperty SuffixForegroundProperty = DependencyProperty.Register("SuffixForeground", typeof(Brush), typeof(CustomTag), new PropertyMetadata(Brushes.DodgerBlue));
        public static DependencyProperty UnderlineProperty = DependencyProperty.Register("Underline", typeof(bool), typeof(CustomTag), new PropertyMetadata(true));

        public Brush MarkForeground
        {
            get => (Brush)GetValue(MarkForegroundProperty);
            set => SetValue(MarkForegroundProperty, value);
        }

        public object Suffix
        {
            get => GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }

        public Brush SuffixForeground
        {
            get => (Brush)GetValue(SuffixForegroundProperty);
            set => SetValue(SuffixForegroundProperty, value);
        }

        public bool Underline
        {
            get => (bool)GetValue(UnderlineProperty);
            set => SetValue(UnderlineProperty, value);
        }

        static CustomTag()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTag), new FrameworkPropertyMetadata(typeof(CustomTag)));
        }
    }
}
