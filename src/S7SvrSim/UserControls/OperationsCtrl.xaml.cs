using S7Server.Simulator.ViewModels;
using S7Svr.Simulator.ViewModels;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class OperationsCtrl : UserControl, IViewFor<OperationVM>
    {
        public OperationsCtrl()
        {
            InitializeComponent();


            

            this.WhenActivated(d => {
                this.ViewModel = Locator.Current.GetRequiredService<OperationVM>();
                this.BindCommand(this.ViewModel, vm => vm.RwTargetVM.CmdRunScript, v => v.btnRunScript).DisposeWith(d);
                this.BindCommand(this.ViewModel, vm => vm.RwTargetVM.CmdTaskList, v => v.btnTaskList).DisposeWith(d);
                
                this.Bind(this.ViewModel, vm => vm.RwTargetVM.TargetDBNumber, v => v.txtTargetDbNumber.Text).DisposeWith(d);
                this.Bind(this.ViewModel, vm => vm.RwTargetVM.TargetPos, v => v.txtTargetPos.Text).DisposeWith(d);

                this.OneWayBind(this.ViewModel, vm => vm.RwBitVM, v => v.bitOps.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.RwByteVM, v => v.byteOps.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.RwShortVM, v => v.shortOps.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.RwUInt32VM, v => v.uintOps.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.RwUInt64VM, v => v.ulongOps.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.RwRealVM, v => v.realOps.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.RwLRealVM, v => v.lRealOps.ViewModel).DisposeWith(d);
                this.OneWayBind(this.ViewModel, vm => vm.RwStringVM, v => v.stringOps.ViewModel).DisposeWith(d);
                //this.OneWayBind(this.ViewModel, vm => vm.TaskViewModle, v => v.Task.ViewModel).DisposeWith(d);

                this.ViewModel.RwTargetVM.CmdRunScript.ThrownExceptions
                    .Subscribe(e => MessageBox.Show(e.Message));
            });
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
        #region


        public OperationVM ViewModel
        {
            get { return (OperationVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (OperationVM)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(OperationVM), typeof(OperationsCtrl), new PropertyMetadata(null));
        #endregion

    }
}
