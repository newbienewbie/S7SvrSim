using S7SvrSim.Project;

namespace S7SvrSim.Services.Project
{
    public interface IProject
    {
        protected const string DEFAULT_FILENAME = "unamed";
        public const string FILE_EXTENSION = ".s7proj";

        ProjectFile ProjectFile { get; }
        string Path { get; }
        void New();
        void Save();
        void SaveAs(string path);
        void Load();
        void SetSoftware();
    }
}
