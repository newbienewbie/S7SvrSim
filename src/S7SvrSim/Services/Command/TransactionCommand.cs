using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Services.Command
{
    internal class TransactionCommand : IHistoryCommand, IEnumerable<IHistoryCommand>
    {
        private readonly IList<IHistoryCommand> _commands;

        public event EventHandler AfterUndo;
        public event EventHandler AfterExecute;

        public TransactionCommand()
        {
            _commands = new List<IHistoryCommand>();
        }

        public void Regist(IHistoryCommand command)
        {
            _commands.Add(command);
        }

        public void Execute()
        {
            foreach (IHistoryCommand command in _commands)
            {
                command.Execute();
            }
            AfterExecute?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            foreach (IHistoryCommand command in _commands.Reverse())
            {
                command.Undo();
            }
            AfterUndo?.Invoke(this, EventArgs.Empty);
        }

        public IEnumerator<IHistoryCommand> GetEnumerator()
        {
            return _commands.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
