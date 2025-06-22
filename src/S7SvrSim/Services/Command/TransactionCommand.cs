using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Services.Command
{
    internal class TransactionCommand : ICommand, IEnumerable<ICommand>
    {
        private readonly IList<ICommand> _commands;

        public event EventHandler AfterUndo;
        public event EventHandler AfterExecute;

        public TransactionCommand()
        {
            _commands = new List<ICommand>();
        }

        public void Regist(ICommand command)
        {
            _commands.Add(command);
        }

        public void Execute()
        {
            foreach (ICommand command in _commands)
            {
                command.Execute();
            }
            AfterExecute?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            foreach (ICommand command in _commands.Reverse())
            {
                command.Undo();
            }
            AfterUndo?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerator<ICommand> GetEnumerator()
        {
            return _commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
