using S7Svr.Simulator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace S7Svr.Simulator.UserControls
{
    /// <summary>
    /// Interaction logic for OperationsCtrl.xaml
    /// </summary>
    public partial class OperationsCtrl : UserControl
    {
        public OperationsCtrl()
        {
            InitializeComponent();
        }


        public int DBNumber
        {
            get { return (int)GetValue(DBNumberProperty); }
            set { SetValue(DBNumberProperty, value); }
        }
        public static readonly DependencyProperty DBNumberProperty = DependencyProperty.Register(nameof(DBNumber), typeof(int), typeof(OperationsCtrl), new PropertyMetadata(0));


        public int DBSize
        {
            get { return (int)GetValue(DBSizeProperty); }
            set { SetValue(DBSizeProperty, value); }
        }
        public static readonly DependencyProperty DBSizeProperty = DependencyProperty.Register(nameof(DBSize), typeof(int), typeof(OperationsCtrl), new PropertyMetadata(0));


    }
}
