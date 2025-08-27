using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace S7SvrSim.UserControls
{
    public class CustomTag : ContentControl
    {
        public static readonly DependencyProperty MarkProperty = DependencyProperty.Register("Mark", typeof(object), typeof(CustomTag), new PropertyMetadata("#"));
        public static readonly DependencyProperty MarkForegroundProperty = DependencyProperty.Register("MarkForeground", typeof(Brush), typeof(CustomTag), new PropertyMetadata(Brushes.DodgerBlue));
        public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register("Suffix", typeof(object), typeof(CustomTag), new PropertyMetadata(null));
        public static readonly DependencyProperty SuffixVisibilityProperty = DependencyProperty.Register("SuffixVisibility", typeof(Visibility), typeof(CustomTag), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty SuffixForegroundProperty = DependencyProperty.Register("SuffixForeground", typeof(Brush), typeof(CustomTag), new PropertyMetadata(Brushes.Gray));
        public static readonly DependencyProperty UnderlineProperty = DependencyProperty.Register("Underline", typeof(bool), typeof(CustomTag), new PropertyMetadata(true));

        public object Mark
        {
            get => GetValue(MarkProperty);
            set => SetValue(MarkProperty, value);
        }

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

        public Visibility SuffixVisibility
        {
            get => (Visibility)GetValue(SuffixVisibilityProperty);
            set => SetValue(SuffixVisibilityProperty, value);
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
