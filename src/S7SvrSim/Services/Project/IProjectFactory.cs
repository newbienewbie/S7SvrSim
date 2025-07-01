namespace S7SvrSim.Services.Project
{
    public interface IProjectFactory
    {
        IProject GetProject(string path);
        IProject CreateProject(string path);
        IProject GetOrCreateProject(string path);
        IProject MoveProject(string path, string newPath);
    }
}
