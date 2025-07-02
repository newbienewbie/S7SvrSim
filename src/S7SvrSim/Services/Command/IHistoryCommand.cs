using System;

namespace S7SvrSim.Services
{
    public interface IHistoryCommand
    {
        event EventHandler AfterUndo;
        event EventHandler AfterExecute;
        void Undo();
        void Execute();
    }
}
