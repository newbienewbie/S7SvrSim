using System;
using System.Collections.Generic;

namespace S7SvrSim.Services.Command
{
    internal class CollectionChangedCommand<T> : ICommand
    {
        protected readonly ICollection<T> list;
        protected readonly ChangedType changedType;
        protected readonly T obj;

        public CollectionChangedCommand(ICollection<T> list, ChangedType changedType, T obj)
        {
            this.list = list;
            this.changedType = changedType;
            this.obj = obj;
        }

        public event EventHandler AfterUndo;
        public event EventHandler AfterExecute;

        public virtual void Execute()
        {
            if (changedType == ChangedType.Add)
            {
                list.Add(obj);
            }
            else
            {
                list.Remove(obj);
            }

            AfterExecute?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Undo()
        {
            if (changedType == ChangedType.Add)
            {
                list.Remove(obj);
            }
            else
            {
                list.Add(obj);
            }
            AfterUndo?.Invoke(this, EventArgs.Empty);
        }
    }
}
