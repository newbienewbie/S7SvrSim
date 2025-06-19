using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
                ViewModel.Selector = signalGrid;
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

        private void signalGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var currentCell = signalGrid.CurrentCell;
            if ((string)currentCell.Column?.Header == "Value")
            {
                ViewModel.OpenValueSet();
            }
        }

        private void DataGridRow_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void DataGridRow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(SignalEditObj)) is SignalEditObj draggedItem &&
                sender is DataGridRow targetRow && targetRow.DataContext is SignalEditObj targetItem)
            {
                targetRow.Background = Brushes.Transparent;
                ViewModel.ReplaceSignal(draggedItem, targetItem);
            }
        }

        private void DrapButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Button icon && icon.DataContext is SignalEditObj item)
            {
                signalGrid.SelectedItem = item;
                DragDrop.DoDragDrop(icon, item, DragDropEffects.Move);
                e.Handled = true;
            }
        }

        private void DataGridRow_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.Background = Brushes.Gold;
            }
        }

        private void DataGridRow_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                row.Background = Brushes.Transparent;
            }
        }
    }

    public class DataGridMaxLenTextColumn : System.Windows.Controls.DataGridTextColumn
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
