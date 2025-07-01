using DynamicData;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using S7Svr.Simulator;
using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
                ViewModel.Grid = signalGrid;
                ViewModel.AfterDragEvent += ViewModel_AfterDragEvent;
                signalGrid.ContextMenu.PlacementTarget = signalGrid;
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

        private void ViewModel_AfterDragEvent(IEnumerable<SignalEditObj> dragObjs)
        {
            signalGrid.UnselectAll();
            foreach (var item in dragObjs)
            {
                signalGrid.SelectedItems.Add(item);
            }
            ViewModel.IsDragSignals = false;
            signalGrid.ContextMenu.IsOpen = false;
        }

        private void DataGridRow_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        private void DataGridRow_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(List<SignalEditObj>)) is List<SignalEditObj> draggedItems &&
                sender is DataGridRow targetRow && targetRow.DataContext is SignalEditObj targetItem)
            {
                targetRow.Background = Brushes.Transparent;

                if (draggedItems.Contains(targetItem))
                {
                    return;
                }

                ViewModel.DragTargetSignal = targetItem;
                ViewModel.IsDragSignals = true;

                signalGrid.ContextMenu.IsOpen = true;
            }
        }

        private void DrapButton_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Button icon && icon.DataContext is SignalEditObj item)
            {
                var list = new List<SignalEditObj>();
                if (signalGrid.SelectedItems.Count > 1 && signalGrid.SelectedItems.Contains(item))
                {
                    list.AddRange(signalGrid.SelectedItems.Cast<SignalEditObj>());
                }
                else
                {
                    signalGrid.UnselectAll();
                    signalGrid.SelectedItem = item;
                    list.Add(item);
                }

                ViewModel.DragSignals.Clear();
                ViewModel.DragSignals.AddRange(list);
                DragDrop.DoDragDrop(icon, list, DragDropEffects.Move);
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

        private void SignalGridContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            ViewModel.IsDragSignals = false;
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox box)
            {
                var binding = BindingOperations.GetBindingExpression(box, ComboBox.TextProperty);
                binding?.UpdateSource();
            }
        }
    }
}
