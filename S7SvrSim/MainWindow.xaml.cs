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

namespace S7Svr.Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(MainVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as MainVM;
            vm.CmdStartServer.Execute(null);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
