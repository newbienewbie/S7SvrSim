using DynamicData;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Project;
using S7SvrSim.S7Signal;
using S7SvrSim.ViewModels;
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

        public ProjectFile ProjectFile { get; private set; }
        public string Path { get; }

        public SoftwareProject(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;

            configS7Model = Locator.Current.GetRequiredService<ConfigSnap7ServerVM>();
            pyConfigModel = Locator.Current.GetRequiredService<ConfigPyEngineVM>();
            signalWatchModel = Locator.Current.GetRequiredService<SignalWatchVM>();
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
        }

        public void SaveAs(string path)
        {
            var project = BuildFromApp();
            project.Save(path);
        }

        public void Load()
        {
            if (!Path.EndsWith(ProjectConst.FILE_EXTENSION))
            {
                throw new FileLoadException("文件后缀名不正确", Path);
            }

            ProjectFile = ProjectFile.Load(Path) ?? throw new NullReferenceException("反序列化文件内容结果为空");
            SetSoftware();
        }

        /// <summary>
        /// 用文件数据去配置软件数据
        /// </summary>
        /// <param name="project"></param>
        public void SetSoftware()
        {
            configS7Model.AreaConfigs.Clear();
            pyConfigModel.PyEngineSearchPaths.Clear();
            signalWatchModel.Signals.Clear();

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
                searchPaths = ProjectFile.DefaultPaths.Concat([IOPath.GetDirectoryName(Path)]).Concat(ProjectFile.SearchPaths.Where(s => !s.Equals(ProjectFile.DEFAULT_PATH_KEY, StringComparison.OrdinalIgnoreCase)));
            }
            else
            {
                searchPaths = ProjectFile.SearchPaths;
            }

            pyConfigModel.PyEngineSearchPaths.AddRange(searchPaths);

            signalWatchModel.SetScanSpan(ProjectFile.ScanSpan);

            signalWatchModel.Signals.AddRange(ProjectFile.Signals.Select(signalCfg =>
            {
                var signalType = signalWatchModel.SignalTypes.First(ty => ty.Name == signalCfg.Type);
                var signal = (SignalBase)Activator.CreateInstance(signalType);
                signal.CopyFromSignalItem(signalCfg);

                return new SignalEditObj(signalType)
                {
                    Value = signal
                };
            }));
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
                Signals = signalWatchModel.Signals.Select(signal => signal.Value.ToSignalItem()).ToList(),
                ScanSpan = signalWatchModel.ScanSpan,
            };
        }
    }
}
