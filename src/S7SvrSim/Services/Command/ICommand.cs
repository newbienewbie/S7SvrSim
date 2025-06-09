namespace S7SvrSim.Services
{
    public interface ICommand
    {
        void Undo();
        void Execute();
    }
}
