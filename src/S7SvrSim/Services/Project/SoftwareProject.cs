using Microsoft.Extensions.DependencyInjection;
using Microsoft.Scripting.Utils;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Project;
using S7SvrSim.S7Signal;
using S7SvrSim.Shared;
using S7SvrSim.ViewModels;
using S7SvrSim.ViewModels.Signals;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IOPath = System.IO.Path;

namespace S7SvrSim.Services.Project
{
    public class SoftwareProject : IProject
    {
        private readonly ConfigSnap7ServerVM configS7Model;
        private readonly ConfigPyEngineVM pyConfigModel;
        private readonly SignalWatchVM signalWatchModel;
        private readonly SignalsCollection signalsCollection;
        private readonly MsgLoggerVM msgLoggerVM;
        private readonly IServiceProvider serviceProvider;
        private readonly SignalsHelper signalsHelper;
        private readonly IMemCache<Type[]> signalTypes;

        public ProjectFile ProjectFile { get; private set; }
        public string Path { get; private set; }

        public SoftwareProject(string path, IServiceProvider serviceProvider, SignalsHelper signalsHelper)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.serviceProvider = serviceProvider;
            this.signalsHelper = signalsHelper;
            Path = path;

            signalTypes = serviceProvider.GetRequiredService<IMemCache<Type[]>>();

            configS7Model = Locator.Current.GetRequiredService<ConfigSnap7ServerVM>();
            pyConfigModel = Locator.Current.GetRequiredService<ConfigPyEngineVM>();
            signalWatchModel = Locator.Current.GetRequiredService<SignalWatchVM>();
            signalsCollection = Locator.Current.GetRequiredService<SignalsCollection>();
            msgLoggerVM = Locator.Current.GetRequiredService<MsgLoggerVM>();
        }

        public void New()
        {
            ProjectFile = new ProjectFile();
            ProjectFile.DefaultInit();
            SetSoftware();
        }

        public void Save()
        {
            ProjectFile = BuildFromApp();
            ProjectFile.Save(Path);
            msgLoggerVM.LogInfo("保存成功！");
        }

        public void SaveAs(string path)
        {
            var project = BuildFromApp();
            project.Save(path);
            msgLoggerVM.LogInfo($"保存成功！路径：{path}");
        }

        public void Load()
        {
            if (!Path.EndsWith(ProjectConst.FILE_EXTENSION))
            {
                throw new FileLoadException("文件后缀名不正确", Path);
            }

            ProjectFile = ProjectFile.Load(Path) ?? throw new NullReferenceException("反序列化文件内容结果为空");
            SetSoftware();
            msgLoggerVM.LogInfo($"已加载项目：{Path}");
        }

        public void Move(string newPath)
        {
            if (string.IsNullOrEmpty(newPath)) throw new ArgumentNullException(nameof(newPath));

            Save();

            if (newPath.Equals(Path, StringComparison.OrdinalIgnoreCase)) return;

            var oldPath = Path;
            File.Move(oldPath, newPath, true);
            Path = newPath;

            msgLoggerVM.LogInfo($"移动成功");
        }

        private SignalEditGroup MergeGroup(SignalEditGroup first, IEnumerable<SignalEditGroup> groups)
        {
            first.Signals.AddRange(groups.Select(g => g.Signals).Merge());
            return first;
        }

        private IEnumerable<SignalEditGroup> MergeGroup(IEnumerable<SignalEditGroup> groups)
        {
            return from g in groups
                   group g by g.Name into g
                   let firstGroup = g.FirstOrDefault()
                   where firstGroup != null
                   let sg = MergeGroup(firstGroup, g.Skip(1))
                   select sg;
        }

        /// <summary>
        /// 用文件数据去配置软件数据
        /// </summary>
        /// <param name="project"></param>
        public void SetSoftware()
        {
            configS7Model.AreaConfigs.Clear();
            pyConfigModel.PyEngineSearchPaths.Clear();
            signalsCollection.SignalGroups.Clear();

            configS7Model.AreaConfigs.AddRange(ProjectFile.AreaConfigs.Select(config => new AreaConfigVM()
            {
                AreaKind = config.AreaKind,
                DBNumber = config.BlockNumber,
                DBSize = config.BlockSize,
            }));
            configS7Model.SetIpAddress(string.IsNullOrEmpty(ProjectFile.IpAddress) ? "127.0.0.1" : ProjectFile.IpAddress);

            IEnumerable<string> searchPaths;
            var defaultQuery = ProjectFile.SearchPaths.Where(s => s.Equals(ProjectFile.DEFAULT_PATH_KEY, StringComparison.OrdinalIgnoreCase));
            if (defaultQuery.Any())
            {
                searchPaths = ProjectFile.DefaultPaths.Concat([IOPath.GetDirectoryName(Path)]).Concat(ProjectFile.SearchPaths.Where(s => !s.Equals(ProjectFile.DEFAULT_PATH_KEY, StringComparison.OrdinalIgnoreCase))).Distinct();
            }
            else
            {
                searchPaths = ProjectFile.SearchPaths.Distinct();
            }

            pyConfigModel.PyEngineSearchPaths.AddRange(searchPaths);

            signalWatchModel.SetScanSpan(ProjectFile.ScanSpan);

            IEnumerable<SignalEditGroup> defaultGroup = null;

            if (ProjectFile.Signals.Count > 0)
            {
                defaultGroup = [new SignalEditGroup("default", ProjectFile.Signals.Select(signalsHelper.ItemToEditObj))];
            }

            var groups = ProjectFile.SignalGroups.Select(sg => new SignalEditGroup(sg.Name, sg.SignalItems.Select(signalsHelper.ItemToEditObj)));

            signalsCollection.SignalGroups.AddRange(MergeGroup(defaultGroup != null ? defaultGroup.Concat(groups) : groups));
            signalsCollection.GroupName = null;
            signalsCollection.GroupName = signalsCollection.SignalGroups.FirstOrDefault()?.Name;
        }

        /// <summary>
        /// 使用软件数据生成文件数据
        /// </summary>
        /// <returns></returns>
        private ProjectFile BuildFromApp()
        {
            IEnumerable<string> searchPaths = pyConfigModel.PyEngineSearchPaths;
            var defaultSearchPaths = ProjectFile.DefaultPaths.Concat([IOPath.GetDirectoryName(Path)]);
            if (pyConfigModel.PyEngineSearchPaths.Intersect(defaultSearchPaths).Count() == defaultSearchPaths.Count())
            {
                searchPaths = pyConfigModel.PyEngineSearchPaths.Except(defaultSearchPaths).Concat([ProjectFile.DEFAULT_PATH_KEY]);
            }

            return new ProjectFile()
            {
                AreaConfigs = configS7Model.AreaConfigs.Select(config => new AreaConfig()
                {
                    AreaKind = config.AreaKind,
                    BlockNumber = config.DBNumber,
                    BlockSize = config.DBSize,
                }).ToList(),
                SearchPaths = searchPaths.ToList(),
                IpAddress = configS7Model.IpAddress,
                SignalGroups = signalsCollection.SignalGroups.Select(sg => new SignalGroup()
                {
                    Name = sg.Name,
                    SignalItems = sg.Signals.Select(s => s.Value.ToSignalItem()).ToList()
                }).ToList(),
                ScanSpan = signalWatchModel.ScanSpan,
            };
        }
    }
}
