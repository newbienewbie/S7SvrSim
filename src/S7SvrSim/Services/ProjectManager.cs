using DynamicData;
using S7Svr.Simulator.ViewModels;
using S7SvrSim.Project;
using S7SvrSim.ViewModels;
using Splat;
using System;
using System.IO;
using System.Linq;

namespace S7SvrSim.Services
{
    public class ProjectManager
    {
        private const string DEFAULT_FILENAME = "unamed";
        public const string FILE_EXTENSION = ".s7proj";

        private ConfigSnap7ServerVM configS7Model;
        private ConfigPyEngineVM pyConfigModel;
        private PyScriptRunner pyRunner;
        private string projectPath = null;
        private ProjectFile currentProject = null;

        public string ProjectPath
        {
            get
            {
                if (projectPath == null)
                {
                    return Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), $"{DEFAULT_FILENAME}{FILE_EXTENSION}");
                }
                return projectPath;
            }
        }

        public ProjectManager(PyScriptRunner pyRunner)
        {
            configS7Model = Locator.Current.GetRequiredService<ConfigSnap7ServerVM>();
            pyConfigModel = Locator.Current.GetRequiredService<ConfigPyEngineVM>();
            this.pyRunner = pyRunner;
            Load(ProjectPath);
        }

        private void SetSoftware(ProjectFile project)
        {
            configS7Model.AreaConfigs.Clear();
            pyConfigModel.PyEngineSearchPaths.Clear();

            configS7Model.AreaConfigs.AddRange(project.AreaConfigs.Select(config => new AreaConfigVM()
            {
                AreaKind = config.AreaKind,
                DBNumber = config.BlockNumber,
                DBSize = config.BlockSize,
            }));
            configS7Model.IpAddress = string.IsNullOrEmpty(project.IpAddress) ? "127.0.0.1" : project.IpAddress;

            pyConfigModel.PyEngineSearchPaths.AddRange(project.SearchPaths);
        }

        public void New()
        {
            var project = new ProjectFile();
            project.SearchPaths.Add("$DEFAULT");
            SetSoftware(project);
        }

        public void Save()
        {
            var project = new ProjectFile()
            {
                AreaConfigs = configS7Model.AreaConfigs.Select(config => new AreaConfig()
                {
                    AreaKind = config.AreaKind,
                    BlockNumber = config.DBNumber,
                    BlockSize = config.DBSize,
                }).ToList(),
                SearchPaths = pyConfigModel.PyEngineSearchPaths.ToList(),
                IpAddress = configS7Model.IpAddress,
            };

            project.Save(ProjectPath);
        }

        public void Load(string path)
        {
            if (!path.EndsWith(FILE_EXTENSION))
            {
                throw new FileLoadException("文件后缀名不正确", path);
            }

            var project = ProjectFile.Load(path) ?? throw new NullReferenceException("反序列化文件内容结果为空");

            SetSoftware(project);

            projectPath = path;
        }
    }
}
