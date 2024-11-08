using S7Server.Simulator.ViewModels;
using S7Svr.Simulator.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace S7SvrSim.UserControls
{
    /// <summary>
    /// ScriptTaskWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ScriptTaskWindow : Window,IViewFor<ScriptTaskWindowVM>
    {
        public ScriptTaskWindow()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(this.ViewModel, vm => vm.ScriptTaskDataGrids, v => v.datagrid.ItemsSource).DisposeWith(d);
            });
        }

        #region


        public ScriptTaskWindowVM ViewModel
        {
            get { return (ScriptTaskWindowVM)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel { get => this.ViewModel; set => this.ViewModel = (ScriptTaskWindowVM)value; }

        // Using a DependencyProperty as the backing store for ViewModel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ScriptTaskWindowVM), typeof(ScriptTaskWindow), new PropertyMetadata(null));
        #endregion
    }
    public class ScriptTaskWindowVM :ReactiveObject
    {
        public ScriptTaskWindowVM()
        {
            SubjectTaskData.Subscribe(s =>
            {
                if (s!=null)
                {
                    ScriptTaskDataGrids.Add(s);

                }
            });

            this.StopTask = ReactiveCommand.Create<ScriptTaskData>((data) =>
            {
                var needremovetask = ScriptTaskDataGrids.First(o => o.FilePath == data.FilePath);
                data.TaskDisposable.Cancel();
                ScriptTaskDataGrids.Remove(needremovetask);
            });
        }
        public ICommand StopTask {  get;  }
        public Subject<ScriptTaskData> SubjectTaskData { get; } = new Subject<ScriptTaskData>();
        public ObservableCollection<ScriptTaskData > ScriptTaskDataGrids { get; set; }  =new ObservableCollection<ScriptTaskData>();
    }

    public class ScriptTaskData
    {
        public string FilePath { get; set; }
        public CancellationTokenSource TaskDisposable { get; set; }
    }
}
