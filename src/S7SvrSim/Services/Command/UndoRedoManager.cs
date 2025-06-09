using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Services
{
    public class UndoRedoManager
    {
        private UndoRedoManager()
        {

        }
        public static int MaxOfCommands { get; set; } = 100;
        private static List<ICommand> UndoCommands { get; } = [];
        public static int UndoCount => UndoCommands.Count;
        private static List<ICommand> RedoCommands { get; } = [];
        public static int RedoCount => RedoCommands.Count;

        public delegate void UndoRedoChangedEvent();
        public static event UndoRedoChangedEvent UndoRedoChanged;

        private static void AddCommand(List<ICommand> target, ICommand command)
        {
            if (target.Count >= 100)
            {
                target.RemoveAt(0);
            }

            target.Add(command);
        }

        public static void Undo()
        {
            if (UndoCommands.Count == 0)
            {
                return;
            }

            var command = UndoCommands.Last();
            command.Undo();
            UndoCommands.Remove(command);
            AddCommand(RedoCommands, command);
            UndoRedoChanged?.Invoke();
        }

        public static void Redo()
        {
            if (RedoCommands.Count == 0)
            {
                return;
            }

            var command = RedoCommands.Last();
            command.Execute();
            RedoCommands.Remove(command);
            AddCommand(UndoCommands, command);
            UndoRedoChanged?.Invoke();
        }

        public static void Regist(ICommand command)
        {
            AddCommand(UndoCommands, command);
            UndoRedoChanged?.Invoke();
        }
    }
}
