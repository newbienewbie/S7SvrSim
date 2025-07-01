using System.IO;

namespace S7SvrSim.Services.Project
{
    public class ProjectFractory : IProjectFactory
    {
        private IProject GetProjectByPath(string path)
        {
            return path == null ? new DefaultProject() : new SoftwareProject(path);
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

        public IProject MoveProject(string path, string newPath)
        {
            throw new System.NotImplementedException();
        }
    }
}
