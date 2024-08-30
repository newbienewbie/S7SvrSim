using S7Svr.Simulator.ViewModels;
using Splat;
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
    public partial class MainWindow : Window, IViewFor<MainVM>
    {

        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(d => {
                this.ViewModel = Locator.Current.GetRequiredService<MainVM>();
                this.DataContext = this.ViewModel;
            });
        }

        #region
        public MainVM ViewModel
        {
            get { return (MainVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (MainVM)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(MainVM), typeof(MainWindow), new PropertyMetadata(null));


        #endregion
    }
}
