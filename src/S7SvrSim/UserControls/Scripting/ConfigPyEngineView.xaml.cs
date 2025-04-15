using S7SvrSim.ViewModels;
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

namespace S7SvrSim.UserControls.Scripting
{
    /// <summary>
    /// Interaction logic for ConfigPyEngineView.xaml
    /// </summary>
    public partial class ConfigPyEngineView : UserControl, IViewFor<ConfigPyEngineVM>
    {
        public ConfigPyEngineView()
        {
            InitializeComponent();
            this.WhenActivated(d => {
                this.ViewModel = Locator.Current.GetRequiredService<ConfigPyEngineVM>();
                this.DataContext = this.ViewModel;
                this.OneWayBind(this.ViewModel, vm => vm.PyEngineSearchPaths, v => v.itemsPyEngineSearchPaths.ItemsSource).DisposeWith(d);
                this.BindCommand(this.ViewModel, vm => vm.CmdSelectModulePath, v => v.btnSelectModulePath).DisposeWith(d);
                this.Bind(this.ViewModel, vm => vm.SelectedModulePath, v => v.txtSelectedModulePath.Text).DisposeWith(d);
                this.BindCommand(this.ViewModel, vm => vm.CmdSubmitSelectPath, v => v.btnSubmitSelectPath).DisposeWith(d);
            });
        }

        #region


        public ConfigPyEngineVM ViewModel
        {
            get { return (ConfigPyEngineVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (ConfigPyEngineVM)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ConfigPyEngineVM), typeof(ConfigPyEngineView), new PropertyMetadata(null));



        #endregion
    }
}
