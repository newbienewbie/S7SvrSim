using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Windows;
using System.Windows.Controls;

namespace S7SvrSim.UserControls
{
    /// <summary>
    /// SignalWatchCtrl.xaml 的交互逻辑
    /// </summary>
    public partial class SignalWatchCtrl : UserControl, IViewFor<SignalWatchVM>
    {
        private readonly IMediator mediator;
        public SignalWatchCtrl()
        {
            InitializeComponent();

            mediator = ((App)Application.Current).ServiceProvider.GetRequiredService<IMediator>();

            this.WhenActivated(d =>
            {
                ViewModel = Locator.Current.GetRequiredService<SignalWatchVM>();
            });
        }

        public SignalWatchVM ViewModel { get => (SignalWatchVM)GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
        object IViewFor.ViewModel { get => ViewModel; set => ViewModel = (SignalWatchVM)value; }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(SignalWatchVM), typeof(SignalWatchCtrl), new PropertyMetadata(null));

        private ValidationResult EventValidation_ValidateEvent(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return null;
            }

            if (value is string strVal)
            {
                try
                {
                    _ = new SignalAddress(strVal);
                }
                catch (FormatException)
                {
                    return new ValidationResult(false, "格式错误");
                }
            }
            else if (value is SignalAddress)
            {
                return null;
            }
            else
            {
                return new ValidationResult(false, "不支持的值类型");
            }
            return null;
        }
    }

    public class DataGridMaxLenTextColumn : DataGridTextColumn
    {
        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            if (dataItem is SignalEditObj editObj && editObj.Value is S7Signal.String)
            {
                return base.GenerateEditingElement(cell, dataItem);
            }
            else
            {
                return null;
            }
        }
    }
}
