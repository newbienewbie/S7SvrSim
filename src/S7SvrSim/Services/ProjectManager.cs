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
    public class ProjectManager : IDisposable
    {
        private const string DEFAULT_FILENAME = "unamed";
        public const string FILE_EXTENSION = ".s7proj";

        private ConfigSnap7ServerVM configS7Model;
        private ConfigPyEngineVM pyConfigModel;
        private string projectPath = null;
        private bool disposedValue;

        public string ProjectPath
        {
            get
            {
                if (projectPath == null)
                {
                    return Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), DEFAULT_FILENAME, $"{DEFAULT_FILENAME}{FILE_EXTENSION}");
                }
                return projectPath;
            }
        }

        public ProjectManager()
        {
            configS7Model = Locator.Current.GetRequiredService<ConfigSnap7ServerVM>();
            pyConfigModel = Locator.Current.GetRequiredService<ConfigPyEngineVM>();

            try
            {
                if (File.Exists(ProjectPath))
                {
                    Load(ProjectPath, false);
                }
                else
                {
                    New();
                }
            }
            catch (Exception)
            {
                New();
            }
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
            configS7Model.SetIpAddress(string.IsNullOrEmpty(project.IpAddress) ? "127.0.0.1" : project.IpAddress);

            pyConfigModel.PyEngineSearchPaths.AddRange(project.SearchPaths);
        }

        public void New()
        {
            var project = new ProjectFile();
            project.DefaultInit();
            SetSoftware(project);
        }

        private ProjectFile BuildFromApp()
        {
            return new ProjectFile()
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
        }

        public void Save()
        {
            SaveAs(ProjectPath);
        }

        public void Save(string path)
        {
            SaveAs(path);
            projectPath = path;
        }

        public void SaveAs(string path)
        {
            var project = BuildFromApp();

            project.Save(path);
        }

        public void Load(string path)
        {
            Load(path, true);
        }

        private void Load(string path, bool replacePath)
        {
            if (!path.EndsWith(FILE_EXTENSION))
            {
                throw new FileLoadException("文件后缀名不正确", path);
            }

            var project = ProjectFile.Load(path) ?? throw new NullReferenceException("反序列化文件内容结果为空");

            SetSoftware(project);

            if (replacePath)
            {
                projectPath = path;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (projectPath == null)
                    {
                        Save();
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
