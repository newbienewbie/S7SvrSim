using Microsoft.Scripting.Hosting;
using S7Server.Simulator.ViewModels;
using S7Svr.Simulator.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
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
                this.OneWayBind(this.ViewModel, vm => vm.ScriptTaskDatas, v => v.datagrid.ItemsSource).DisposeWith(d);
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
            SubjectTaskData
                .SubscribeOn(RxApp.MainThreadScheduler)
                .Subscribe(s =>
                {
                    if (s!=null)
                    {
                        s.Order = ScriptTaskDatas.Count+1;
                        ScriptTaskDatas.Add(s);
                    }
                });

            this.CmdStopTask = ReactiveCommand.Create<ScriptTask>((data) =>
            {
                data.TokenSource.Cancel();
                ScriptTaskDatas.Remove(data);
            });
        }
        public ICommand CmdStopTask {  get;  }
        public Subject<ScriptTask> SubjectTaskData { get; } = new Subject<ScriptTask>();
        public ObservableCollection<ScriptTask > ScriptTaskDatas { get; private set; }  =new ObservableCollection<ScriptTask>();
    }

    public class ScriptTask
    {
        public ScriptTask(Guid taskId, string filePath, CancellationTokenSource tokenSource, ScriptScope pyScope)
        {
            TaskId = taskId;
            FilePath = filePath;
            TokenSource = tokenSource;
            PyScope = pyScope;
        }

        public Guid TaskId { get; set; }
        public string FilePath { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public ScriptScope PyScope { get; set; }
        public int Order { get;set; }
    };
}
