using DynamicData;
using DynamicData.Binding;
using MediatR;
using Microsoft.Win32;
using ReactiveUI.Fody.Helpers;
using S7SvrSim.Messages;
using S7SvrSim.Services;
using S7SvrSim.Services.Project;
using S7SvrSim.Services.Recent;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace S7SvrSim.ViewModels
{
    public class ProjectVM : ReactiveObject
    {
        private readonly IProjectFactory projectFactory;
        private readonly RecentFilesCollection recentFiles;
        private readonly IMediator mediator;
        private IProject currentProject;

        [Reactive]
        public bool NeedSave { get; set; }

        [Reactive]
        public int UndoCount { get; set; }

        [Reactive]
        public int RedoCount { get; set; }

        public ReadOnlyObservableCollection<RecentFile> RecentFiles { get; }

        public bool CanOpenRecent => RecentFiles.Count > 0;

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand RenameCommand { get; }
        public ICommand OpenRecentCommand { get; }
        public ICommand RemoveRecentCommand { get; }
        /// <summary>
        /// 项目文件名
        /// </summary>
        public string ProjectName => Path.GetFileName(currentProject.Path);

        public ProjectVM(IProjectFactory projectFactory, RecentFilesCollection recentFilesVM, IMediator mediator)
        {
            this.projectFactory = projectFactory;
            this.recentFiles = recentFilesVM;
            this.mediator = mediator;
            currentProject = projectFactory.GetOrCreateProject(null);

            UndoRedoManager.UndoRedoChanged += () =>
            {
                UndoCount = UndoRedoManager.UndoCount;
                RedoCount = UndoRedoManager.RedoCount;
                NeedSave = true;
            };

            recentFiles.Files.Connect()
                .Sort(SortExpressionComparer<RecentFile>.Descending(file => file.OpenTime))
                .Bind(out var data)
                .Subscribe(files => this.RaisePropertyChanged(nameof(CanOpenRecent)));

            RecentFiles = data;

            UndoCommand = ReactiveCommand.Create(UndoRedoManager.Undo, this.WhenAnyValue(vm => vm.UndoCount).Select(c => c > 0));
            RedoCommand = ReactiveCommand.Create(UndoRedoManager.Redo, this.WhenAnyValue(vm => vm.RedoCount).Select(c => c > 0));
            RenameCommand = ReactiveCommand.CreateFromTask(RenameProject);
            OpenRecentCommand = ReactiveCommand.Create<RecentFile>(OpenRecentFile);
            RemoveRecentCommand = ReactiveCommand.Create<RecentFile>(RemoveRecentFile);
        }

        private void CallbackNeedSave()
        {
            NeedSave = true;
            NeedSave = false;
        }

        public MessageBoxResult? NotifyIfSave()
        {
            if (NeedSave)
            {
                var result = MessageBox.Show("当前项目未保存，是否保存？", "未保存项目", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    SaveProject();
                }
                return result;
            }
            return null;
        }

        public void OpenProjectFolder()
        {
            Process.Start("explorer.exe", $"/select,{currentProject.Path}");
        }

        public void NewProject()
        {
            if (NotifyIfSave() == MessageBoxResult.Cancel)
            {
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "选择新项目保存路径",
                Filter = $"S7模拟项目|*{ProjectConst.FILE_EXTENSION}",
                FileName = Path.GetFileName(currentProject.Path),
                RestoreDirectory = true,
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            currentProject = projectFactory.CreateProject(saveFileDialog.FileName);
            UndoRedoManager.Reset();
        }

        private void OpenProject(string file)
        {
            if (file == currentProject.Path)
            {
                return;
            }

            IProject project;

            try
            {
                project = projectFactory.GetProject(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "打开项目文件失败", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            currentProject = project;
            UndoRedoManager.Reset();

            CallbackNeedSave();
        }

        public void LoadProject()
        {
            if (NotifyIfSave() == MessageBoxResult.Cancel)
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "选择项目文件",
                Filter = $"S7模拟项目|*{ProjectConst.FILE_EXTENSION}",
                Multiselect = false,
                RestoreDirectory = true,
            };

            if (openFileDialog.ShowDialog() != true)
            {
                return;
            }

            OpenProject(openFileDialog.FileName);

            recentFiles.AddFile(new RecentFile(openFileDialog.FileName, DateTime.Now));
        }

        public void SaveProject()
        {
            currentProject.Save();
            NeedSave = false;
        }

        public void SaveProjectAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                Title = "选择另存路径",
                Filter = $"S7模拟项目|*{ProjectConst.FILE_EXTENSION}",
                FileName = Path.GetFileName(currentProject.Path),
                RestoreDirectory = true,
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            currentProject.SaveAs(saveFileDialog.FileName);
            currentProject = projectFactory.GetProject(saveFileDialog.FileName);

            CallbackNeedSave();
        }
        private static readonly char[] InvalidCharsForPath = ['\\', '/', ':', '*', '?', '"', '<', '>', '|'];
        public async Task RenameProject()
        {
            var viewModel = new DialogViewModel("Rename Project", "名称不能为空")
            {
                Text = Path.GetFileNameWithoutExtension(ProjectName),
                SuffixText = ProjectConst.FILE_EXTENSION,
            };
            viewModel.ValidationEvent += (name) => !string.IsNullOrEmpty(name);
            var result = await mediator.Send(new ShowDialogRequest(viewModel));

            if (result.IsCancel || string.IsNullOrEmpty(result.Result)) return;

            var newName = new string(result.Result.Select(c => InvalidCharsForPath.Contains(c) ? ' ' : c).ToArray());
            newName = $"{newName}{ProjectConst.FILE_EXTENSION}";

            if (newName == ProjectName) return;

            var newPath = Path.Combine(Path.GetDirectoryName(currentProject.Path), newName);
            if (File.Exists(newPath) &&
                MessageBox.Show("新名称下已有存在的文件，是否覆盖？", "目标文件已存在", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                return;

            var oldFile = RecentFiles.FirstOrDefault(f => f.Path == currentProject.Path);
            if (oldFile != null)
            {
                recentFiles.RemoveFile(oldFile);
            }

            currentProject.Move(newPath);
            recentFiles.AddFile(new RecentFile(currentProject.Path, DateTime.Now));

            CallbackNeedSave();
        }

        private void OpenRecentFile(RecentFile file)
        {
            if (NotifyIfSave() == MessageBoxResult.Cancel)
            {
                return;
            }

            if (!File.Exists(file.Path))
            {
                if (MessageBox.Show("是否从列表中移除？", "文件不存在", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    recentFiles.RemoveFile(file);
                    return;
                }
            }
            else
            {
                OpenProject(file.Path);
                recentFiles.AddFile(new RecentFile(file.Path, DateTime.Now));
            }
        }

        private void RemoveRecentFile(RecentFile file)
        {
            recentFiles.RemoveFile(file);
        }
    }
}
