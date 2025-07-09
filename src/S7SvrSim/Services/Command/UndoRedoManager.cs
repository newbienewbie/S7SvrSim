using S7SvrSim.Services.Command;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Services
{
    public enum TransactionMode
    {
        /// <summary>
        /// 取消上一个，及将所有的命令单独拿出来存储
        /// </summary>
        CancelLast,
        /// <summary>
        /// 结束上一个，及将上一个命令保存再创建新命令
        /// </summary>
        EndLast,
    }

    public class UndoRedoManager
    {
        private UndoRedoManager()
        {

        }
        public static bool IsInUndoRedo { get; private set; } = false;
        public static int MaxOfCommands { get; set; } = 999;
        private static List<IHistoryCommand> UndoCommands { get; } = [];
        public static int UndoCount => UndoCommands.Count;
        private static List<IHistoryCommand> RedoCommands { get; } = [];
        public static int RedoCount => RedoCommands.Count;

        public delegate void UndoRedoChangedEvent();
        public static event UndoRedoChangedEvent UndoRedoChanged;

        private static TransactionCommand TransactionCommand { get; set; }

        public static void StartTransaction(TransactionMode transactionMode = TransactionMode.CancelLast)
        {
            if (transactionMode == TransactionMode.CancelLast)
            {
                CancelTransaction();
            }
            else if (transactionMode == TransactionMode.EndLast)
            {
                EndTransaction();
            }
            TransactionCommand = new TransactionCommand();
        }

        public static IEnumerable<IHistoryCommand> CancelTransaction()
        {
            if (TransactionCommand != null && TransactionCommand.Any())
            {
                UndoCommands.AddRange(TransactionCommand);
                RedoCommands.Clear();
                if (UndoCommands.Count >= MaxOfCommands)
                {
                    UndoCommands.RemoveRange(0, UndoCount - MaxOfCommands);
                }
                UndoRedoChanged?.Invoke();
            }
            var resCmd = TransactionCommand.AsEnumerable();
            TransactionCommand = null;
            return resCmd;
        }

        public static void RollbackTransaction()
        {
            if (TransactionCommand != null && TransactionCommand.Any())
            {
                IsInUndoRedo = true;
                foreach (var item in TransactionCommand.Reverse())
                {
                    item.Undo();
                }
                IsInUndoRedo = false;
            }

            TransactionCommand = null;
        }

        public static IHistoryCommand EndTransaction()
        {
            if (TransactionCommand != null && TransactionCommand.Any())
            {
                Regist(TransactionCommand);
            }
            var resCmd = TransactionCommand;
            TransactionCommand = null;
            return resCmd;
        }

        private static void AddCommand(List<IHistoryCommand> target, IHistoryCommand command, bool clearRedo = true)
        {
            if (target.Count >= MaxOfCommands)
            {
                target.RemoveAt(0);
            }

            target.Add(command);

            if (clearRedo)
            {
                RedoCommands.Clear();
            }
        }

        public static void Undo()
        {
            if (UndoCommands.Count == 0)
            {
                return;
            }
            IsInUndoRedo = true;
            var command = UndoCommands.Last();
            command.Undo();
            UndoCommands.Remove(command);
            AddCommand(RedoCommands, command, false);
            UndoRedoChanged?.Invoke();
            IsInUndoRedo = false;
        }

        public static void Redo()
        {
            if (RedoCommands.Count == 0)
            {
                return;
            }

            IsInUndoRedo = true;
            var command = RedoCommands.Last();
            command.Execute();
            RedoCommands.Remove(command);
            AddCommand(UndoCommands, command, false);
            UndoRedoChanged?.Invoke();
            IsInUndoRedo = false;
        }

        public static void Regist(IHistoryCommand command)
        {
            if (TransactionCommand != null)
            {
                TransactionCommand.Regist(command);
            }
            else
            {
                AddCommand(UndoCommands, command);
                UndoRedoChanged?.Invoke();
            }
        }

        public static void Run(IHistoryCommand command)
        {
            command.Execute();
            Regist(command);
        }

        public static void Reset()
        {
            UndoCommands.Clear();
            RedoCommands.Clear();
            UndoRedoChanged?.Invoke();
        }

        public static T GetLastUndoCommands<T>()
            where T : IHistoryCommand
        {
            return UndoCommands.Where(c => c is T).Cast<T>().LastOrDefault();
        }

        public static T GetLastRedoCommands<T>()
            where T : IHistoryCommand
        {
            return RedoCommands.Where(c => c is T).Cast<T>().LastOrDefault();
        }

        public static T[] GetUndoCommands<T>()
            where T : IHistoryCommand
        {
            return UndoCommands.Where(c => c is T).Cast<T>().ToArray();
        }

        public static T[] GetRedoCommands<T>()
            where T : IHistoryCommand
        {
            return RedoCommands.Where(c => c is T).Cast<T>().ToArray();
        }
    }
}
