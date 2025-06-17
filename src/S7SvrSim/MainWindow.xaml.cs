using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Commands;
using S7SvrSim.Project;
using S7SvrSim.Services;
using S7SvrSim.Services.Command;
using Splat;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace S7Svr.Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewFor<MainVM>
    {
        private readonly ProjectManager projectManager;

        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.ViewModel = Locator.Current.GetRequiredService<MainVM>();
                this.DataContext = this.ViewModel;
                this.OneWayBind(ViewModel, vm => vm.NeedSave, w => w.Title, GetTitle);
            });

            projectManager = ((App)Application.Current).ServiceProvider.GetRequiredService<ProjectManager>();
            UndoRedoManager.UndoRedoChanged += () =>
            {
                ViewModel.NeedSave = true;
            };
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

        private string GetTitle(bool needSave)
        {
            if (needSave)
            {
                return $"* {Path.GetFileName(projectManager.ProjectPath)} - Siemens PLC 通讯模拟器";
            }
            else
            {
                return $"{Path.GetFileName(projectManager.ProjectPath)} - Siemens PLC 通讯模拟器";
            }
        }

        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.New)
            {
                SaveDefaultProject();
                NewProject();
            }
            else if (e.Command == ApplicationCommands.Open)
            {
                SaveDefaultProject();
                OpenProject();
            }
            else if (e.Command == ApplicationCommands.Save)
            {
                SaveProject();
            }
            else if (e.Command == ApplicationCommands.Undo)
            {
                UndoRedoManager.Undo();
            }
            else if (e.Command == ApplicationCommands.Redo)
            {
                UndoRedoManager.Redo();
            }
            else if (e.Command == AppCommands.OpenFolder)
            {
                Process.Start("explorer.exe", $"/select,{projectManager.ProjectPath}");
            }
        }

        private void SaveDefaultProject()
        {
            if (projectManager.IsDefaultProject)
            {
                projectManager.Save();
            }
        }

        private void NewProject()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "选择保存路径",
                Filter = $"S7模拟项目|*{ProjectManager.FILE_EXTENSION}",
                FileName = Path.GetFileName(projectManager.ProjectPath),
                RestoreDirectory = true,
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            projectManager.New(saveFileDialog.FileName);
            UndoRedoManager.Reset();

            ViewModel.NeedSave = false;
        }

        private void SaveProject()
        {
            string savePath = null;
            if (projectManager.IsDefaultProject)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog()
                {
                    Title = "选择保存路径",
                    Filter = $"S7模拟项目|*{ProjectManager.FILE_EXTENSION}",
                    FileName = Path.GetFileName(projectManager.ProjectPath),
                    RestoreDirectory = true,
                };

                if (saveFileDialog.ShowDialog() != true)
                {
                    return;
                }

                savePath = saveFileDialog.FileName;
            }

            try
            {
                if (savePath != null)
                {
                    projectManager.Save(savePath);
                    new ProjectFile().Save(ProjectManager.DefaultProjectPath);
                }
                else
                {
                    projectManager.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "保存项目文件失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ViewModel.NeedSave = false;
        }

        private void OpenProject()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "选择项目文件",
                Filter = $"S7模拟项目|*{ProjectManager.FILE_EXTENSION}",
                Multiselect = false,
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                projectManager.Load(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "打开项目文件失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            UndoRedoManager.Reset();
            ViewModel.NeedSave = false;
        }

        private void NotRunningStatus_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ViewModel?.RunningVM != null)
            {
                e.CanExecute = !ViewModel.RunningVM.RunningStatus;
                e.Handled = true;
            }
        }

        private void CanExecuteTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void Undo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ViewModel?.RunningVM != null)
            {
                e.CanExecute = UndoRedoManager.UndoCount > 0 && !ViewModel.RunningVM.RunningStatus;
                e.Handled = true;
            }
        }

        private void Redo_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ViewModel?.RunningVM != null)
            {
                e.CanExecute = UndoRedoManager.RedoCount > 0 && !ViewModel.RunningVM.RunningStatus;
                e.Handled = true;
            }
        }

        private void AreaConfigGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            if (e.NewItem is AreaConfigVM areaConfigVM)
            {
                var command = ListChangedCommand<AreaConfigVM>.Add(ViewModel.ConfigVM.AreaConfigs, [areaConfigVM]);
                ViewModel.ConfigVM.CommandEventRegist(command);
                UndoRedoManager.Regist(command);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (ViewModel.NeedSave && !projectManager.IsDefaultProject)
            {
                var result = MessageBox.Show("当前项目未保存，是否保存？", "未保存项目", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProject();
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            base.OnClosing(e);
        }

        public void SwitchTab(int index)
        {
            if (index < tabControl.Items.Count)
            {
                tabControl.SelectedIndex = index;
            }
        }
    }
}
