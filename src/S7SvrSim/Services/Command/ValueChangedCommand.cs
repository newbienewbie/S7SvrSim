using System;

namespace S7SvrSim.Services.Command
{
    internal class ValueChangedCommand<T> : ICommand
    {
        private readonly Action<T> set;
        private readonly T oldValue;
        private readonly T newValue;

        public ValueChangedCommand(Action<T> set, T oldValue, T newValue)
        {
            this.set = set;
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        public event EventHandler AfterUndo;
        public event EventHandler AfterExecute;

        public void Execute()
        {
            set(newValue);
            AfterExecute?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            set(oldValue);
            AfterUndo?.Invoke(this, EventArgs.Empty);
        }
    }

    internal class ValueChangedCommand(Action<object> set, object oldValue, object newValue)
        : ValueChangedCommand<object>(set, oldValue, newValue)
    {
    }
}
