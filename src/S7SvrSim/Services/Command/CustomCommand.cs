using System;
using System.Diagnostics.CodeAnalysis;

namespace S7SvrSim.Services.Command
{
    public class CustomCommand<T> : IHistoryCommand
    {
        private readonly Action<T> execute;
        private readonly Action<T> undo;
        private readonly T oldItem;
        private readonly T newItem;

        public event EventHandler AfterUndo;
        public event EventHandler AfterExecute;

        public CustomCommand(T newItem, [NotNull] Action<T> execute, T oldItem, [NotNull] Action<T> undo)
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            if (undo == null) throw new ArgumentNullException(nameof(undo));

            this.execute = execute;
            this.undo = undo;
            this.oldItem = oldItem;
            this.newItem = newItem;
        }

        public CustomCommand(T item, [NotNull] Action<T> execute, [NotNull] Action<T> undo) : this(item, execute, item, undo)
        {
        }

        public void Execute()
        {
            execute(newItem);
            AfterExecute?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            undo(oldItem);
            AfterUndo?.Invoke(this, EventArgs.Empty);
        }
    }
}
