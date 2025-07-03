using System;
using System.IO;

namespace S7SvrSim.Services.Project
{
    public class ProjectFractory : IProjectFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ProjectFractory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        private IProject GetProjectByPath(string path)
        {
            return path == null ? new DefaultProject(serviceProvider) : new SoftwareProject(path, serviceProvider);
        }

        public IProject CreateProject(string path)
        {
            var project = GetProjectByPath(path);

            project.New();
            project.Save();

            return project;
        }

        public IProject GetOrCreateProject(string path)
        {
            var project = GetProjectByPath(path);
            path = project.Path;

            if (File.Exists(path))
            {
                project.Load();
            }
            else
            {
                project.New();
                project.Save();
            }

            return project;
        }

        public IProject GetProject(string path)
        {
            var project = GetProjectByPath(path);
            project.Load();
            return project;
        }
    }
}
