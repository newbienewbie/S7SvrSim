using System;
using System.Collections.Generic;

namespace S7SvrSim.Services.Command
{
    internal class IListChangedCommand<T> : ICommand
    {
        protected readonly IList<T> list;
        protected readonly ChangedType changedType;
        protected readonly T obj;
        protected int? index = null;
        public IListChangedCommand(IList<T> list, ChangedType changedType, T obj)
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
                if (index != null)
                {
                    list.Insert(index.Value, obj);
                }
                else
                {
                    list.Add(obj);
                }
            }
            else
            {
                index = list.IndexOf(obj);
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
                if (index != null)
                {
                    list.Insert(index.Value, obj);
                }
                else
                {
                    list.Add(obj);
                }
            }
            AfterUndo?.Invoke(this, EventArgs.Empty);
        }
    }
}
