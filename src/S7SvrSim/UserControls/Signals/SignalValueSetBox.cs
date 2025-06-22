using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace S7SvrSim.UserControls.Signals
{
    [TemplatePart(Name = CONTENT_TEMPLATE_NAME, Type = typeof(ContentPresenter))]
    public class SignalValueSetBox : ContentControl
    {
        private const string CONTENT_TEMPLATE_NAME = "PART_Content";
        public static DependencyProperty ValueTypeProperty = DependencyProperty.Register("ValueType", typeof(Type), typeof(SignalValueSetBox), new PropertyMetadata(null, ValueTypeChangedCallback));
        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(SignalValueSetBox), new PropertyMetadata(null));
        public static DependencyProperty TextBoxWidthProperty = DependencyProperty.Register("TextBoxWidth", typeof(int), typeof(SignalValueSetBox), new PropertyMetadata(-1));
        public static DependencyProperty HasValidationErrorProperty = DependencyProperty.Register("HasValidationError", typeof(bool), typeof(SignalValueSetBox), new PropertyMetadata(false));
        public static DependencyProperty TextBoxStyleProperty = DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(SignalValueSetBox), new FrameworkPropertyMetadata(OnTextBoxStyleChanged));



        public Type ValueType
        {
            get => (Type)GetValue(ValueTypeProperty);
            set => SetValue(ValueTypeProperty, value);
        }

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int TextBoxWidth
        {
            get => (int)GetValue(TextBoxWidthProperty);
            set => SetValue(TextBoxWidthProperty, value);
        }

        public bool HasValidationError
        {
            get => (bool)GetValue(HasValidationErrorProperty);
            set => SetValue(HasValidationErrorProperty, value);
        }

        public Style TextBoxStyle
        {
            get => (Style)GetValue(TextBoxStyleProperty);
            set => SetValue(TextBoxStyleProperty, value);
        }

        private ContentPresenter contentPresenter;
        private CheckBox checkBox;
        private TextBox textBox;

        private FrameworkElement currentDisplayControl;

        public SignalValueSetBox()
        {
            checkBox = new CheckBox();
            textBox = new TextBox();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            FocusManager.SetFocusedElement(this, currentDisplayControl);
            if (currentDisplayControl is TextBox textbox)
            {
                textbox.SelectAll();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentPresenter = (ContentPresenter)GetTemplateChild(CONTENT_TEMPLATE_NAME);

            BindingBase binding = BindingOperations.GetBinding(this, ValueProperty);
            BindingOperations.SetBinding(checkBox, System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, binding);

            SetContentElement(ValueType);
            textBox.Style = TextBoxStyle;
        }

        private void SetContentElement(Type newType)
        {
            FrameworkElement newContent;
            if (newType == typeof(bool))
            {
                newContent = checkBox;
            }
            else
            {
                if (TextBoxWidth != -1)
                {
                    textBox.Width = TextBoxWidth;
                }
                newContent = textBox;
                var valueBinding = BindingOperations.GetBinding(this, ValueProperty);
                if (valueBinding != null)
                {
                    var binding = new Binding();
                    binding.Path = valueBinding.Path;
                    if (valueBinding.Source != null)
                    {
                        binding.Source = valueBinding.Source;
                    }
                    else
                    {
                        binding.RelativeSource = valueBinding.RelativeSource;
                    }
                    binding.Mode = valueBinding.Mode;
                    binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                    if (newType != typeof(string))
                    {
                        binding.Converter = new StringToTargetType()
                        {
                            TargetType = newType
                        };
                        var validationRule = new StringToTargetTypeValidation()
                        {
                            TargetType = newType,
                        };
                        validationRule.AfterValidate += (res) => HasValidationError = !res.IsValid;
                        binding.ValidationRules.Add(validationRule);
                    }
                    BindingOperations.SetBinding(textBox, TextBox.TextProperty, binding);
                }
            }

            if (contentPresenter != null)
            {
                contentPresenter.Content = newContent;
                currentDisplayControl = newContent;
            }
            HasValidationError = false;
        }

        private class StringToTargetType : IValueConverter
        {
            public Type TargetType { get; set; }
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string str && TargetType != null)
                {
                    try
                    {
                        return System.Convert.ChangeType(str, TargetType);
                    }
                    catch (Exception)
                    {
                        return Binding.DoNothing;
                    }
                }

                return Binding.DoNothing;
            }
        }

        private class StringToTargetTypeValidation : ValidationRule
        {
            public Type TargetType { get; set; }
            public event Action<ValidationResult> AfterValidate;
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                ValidationResult result = new ValidationResult(true, "");
                if (value is string str)
                {
                    try
                    {
                        _ = Convert.ChangeType(str, TargetType);
                    }
                    catch (Exception)
                    {
                        result = new ValidationResult(false, $"不能转换为\"{TargetType.Name}\"");
                    }
                }
                else
                {
                    if (TargetType != null)
                    {
                        var isValid = value != null;
                        result = new ValidationResult(isValid, isValid ? "" : "值不能为空");
                    }
                }

                AfterValidate?.Invoke(result);
                return result;
            }
        }

        private static void ValueTypeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SignalValueSetBox box)
            {
                box.SetContentElement((Type)e.NewValue);
            }
        }

        private static void OnTextBoxStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SignalValueSetBox box)
            {
                box.textBox.Style = (Style)e.NewValue;
            }
        }
    }
}
