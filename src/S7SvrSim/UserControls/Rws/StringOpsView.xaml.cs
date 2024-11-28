using S7SvrSim.ViewModels.Rw;
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

namespace S7SvrSim.UserControls.Rws
{
    /// <summary>
    /// Interaction logic for StringOpsView.xaml
    /// </summary>
    public partial class StringOpsView : UserControl, IViewFor<RwStringVM>
    {

        public StringOpsView()
        {
            InitializeComponent();
            this.WhenActivated(d => {
                this.Bind(this.ViewModel, vm => vm.ValueRead, v => v.txtValueRead.Text).DisposeWith(d);
                this.Bind(this.ViewModel, vm => vm.ToBeWritten, v => v.txtValueWritten.Text).DisposeWith(d);
                this.Bind(this.ViewModel, vm => vm.StringArrayMaxLength, v => v.txtMaxLength.Text).DisposeWith(d);
                this.BindCommand(this.ViewModel, vm => vm.CmdRead, v => v.btnRead).DisposeWith(d);
                this.BindCommand(this.ViewModel, vm => vm.CmdWrite, v => v.btnWrite).DisposeWith(d);

                this.ViewModel.CmdRead.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message));
                this.ViewModel.CmdWrite.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message));
            });
        }

        #region
        public RwStringVM ViewModel
        {
            get { return (RwStringVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (RwStringVM)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(RwStringVM), typeof(StringOpsView), new PropertyMetadata(null));
        #endregion
    }
}
