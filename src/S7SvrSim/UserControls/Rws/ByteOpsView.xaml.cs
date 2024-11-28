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
    /// Interaction logic for ByteOpsView.xaml
    /// </summary>
    public partial class ByteOpsView : UserControl, IViewFor<RwByteVM>
    {
        public ByteOpsView()
        {
            InitializeComponent();
            this.WhenActivated(d => {
                this.Bind(this.ViewModel, vm => vm.ValueRead, v => v.txtValueRead.Text).DisposeWith(d);
                this.Bind(this.ViewModel, vm => vm.ToBeWritten, v => v.txtValueWritten.Text).DisposeWith(d);
                this.BindCommand(this.ViewModel, vm => vm.CmdRead, v => v.btnRead).DisposeWith(d);
                this.BindCommand(this.ViewModel, vm => vm.CmdWrite, v => v.btnWrite).DisposeWith(d);

                this.ViewModel.CmdRead.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message));
                this.ViewModel.CmdWrite.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message));
            });
        }

        #region
        public RwByteVM ViewModel
        {
            get { return (RwByteVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (RwByteVM)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(RwByteVM), typeof(ByteOpsView), new PropertyMetadata(null));
        #endregion
    }
}
