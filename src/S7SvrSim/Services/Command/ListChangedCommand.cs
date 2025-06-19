using DynamicData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Services.Command
{
    internal class ListChangedCommand<T> : ICommand
        where T : class
    {
        protected class ListItem
        {
            public int Index { get; set; } = -1;
            public T Item { get; set; }
        }

        protected readonly IList<T> list;
        protected readonly ChangedType changedType;
        protected readonly IList<ListItem> newItems;
        protected readonly IList<ListItem> oldItems;
        private ListChangedCommand(IList<T> list, ChangedType changedType, IEnumerable<T> newItems, IEnumerable<T> oldItems)
        {
            this.list = list;
            this.changedType = changedType;
            this.newItems = newItems?.Select(item => new ListItem() { Index = list.IndexOf(item), Item = item }).ToList() ?? [];
            this.oldItems = oldItems?.Select(item => new ListItem() { Index = list.IndexOf(item), Item = item }).ToList() ?? [];
        }

        public static ListChangedCommand<T> Add(IList<T> list, IEnumerable<T> added) => new ListChangedCommand<T>(list, ChangedType.Add, added.Except(list).Where(item => item != null).ToList(), null);
        public static ListChangedCommand<T> Remove(IList<T> list, IEnumerable<T> remove) => new ListChangedCommand<T>(list, ChangedType.Remove, null, remove.Intersect(list).Where(item => item != null).ToList());
        public static ListChangedCommand<T> Clear(IList<T> list) => new ListChangedCommand<T>(list, ChangedType.Clear, null, null);
        public static ListChangedCommand<T> Replace(IList<T> list, IEnumerable<(T OldItem, T NewItem)> pairs) => new ListChangedCommand<T>(list, ChangedType.Replace, pairs.IntersectBy(list, p => p.OldItem).Where(item => item.OldItem != null && item.NewItem != null).Select(p => p.NewItem).ToList(), pairs.Select(p => p.OldItem).ToList());

        public event EventHandler AfterUndo;
        public event EventHandler AfterExecute;

        private void Add()
        {
            if (newItems.Count == 0)
            {
                return;
            }

            var first = newItems[0].Index;
            if (first >= 0)
            {
                list.AddRange(newItems.Select(item => item.Item), first);
            }
            else
            {
                list.AddRange(newItems.Select(item => item.Item));
                newItems[0].Index = list.IndexOf(newItems[0].Item);
            }
        }

        private void AddBack()
        {
            if (newItems.Count == 0)
            {
                return;
            }

            list.RemoveMany(newItems.Select(item => item.Item));
        }

        private void Remove()
        {
            if (oldItems.Count == 0)
            {
                return;
            }

            list.RemoveMany(oldItems.Select(item => item.Item));
        }

        private void RemoveBack()
        {
            if (oldItems.Count == 0)
            {
                return;
            }

            foreach (var item in oldItems)
            {
                list.Insert(item.Index, item.Item);
            }
        }

        private void Replace(IEnumerable<ListItem> @from, IEnumerable<ListItem> to)
        {
            var pair = from f in @from.Select((f, i) => (Item: f, Index: i))
                       join t in to.Select((t, i) => (Item: t, Index: i)) on f.Index equals t.Index
                       select (Old: f.Item, New: t.Item);


            foreach (var (oldItem, newItem) in pair)
            {
                if (oldItem.Index == -1 || list.Count <= oldItem.Index)
                {
                    newItem.Index = -1;
                    continue;
                }

                list[oldItem.Index] = newItem.Item;
                if (newItem.Index != oldItem.Index)
                {
                    newItem.Index = oldItem.Index;
                }
            }
        }

        private void Clear()
        {
            if (oldItems.Count == 0)
            {
                oldItems.AddRange(list.Select(item => new ListItem() { Item = item }));
            }
            list.Clear();
        }

        private void ClearBack()
        {
            list.AddRange(oldItems.Select(item => item.Item));
        }

        public virtual void Execute()
        {
            switch (changedType)
            {
                case ChangedType.Add:
                    Add();
                    break;
                case ChangedType.Remove:
                    Remove();
                    break;
                case ChangedType.Replace:
                    Replace(oldItems, newItems);
                    break;
                case ChangedType.Clear:
                    Clear();
                    break;
                default:
                    break;
            }

            AfterExecute?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Undo()
        {
            switch (changedType)
            {
                case ChangedType.Add:
                    AddBack();
                    break;
                case ChangedType.Remove:
                    RemoveBack();
                    break;
                case ChangedType.Replace:
                    Replace(newItems, oldItems);
                    break;
                case ChangedType.Clear:
                    ClearBack();
                    break;
                default:
                    break;
            }
            AfterUndo?.Invoke(this, EventArgs.Empty);
        }
    }

    internal static class ListChangedCommand
    {
        public static ListChangedCommand<T> Add<T>(IList<T> list, IEnumerable<T> added) where T : class => ListChangedCommand<T>.Add(list, added);
        public static ListChangedCommand<T> Remove<T>(IList<T> list, IEnumerable<T> remove) where T : class => ListChangedCommand<T>.Remove(list, remove);
        public static ListChangedCommand<T> Clear<T>(IList<T> list) where T : class => ListChangedCommand<T>.Clear(list);
        public static ListChangedCommand<T> Replace<T>(IList<T> list, IEnumerable<(T OldItem, T NewItem)> pairs) where T : class => ListChangedCommand<T>.Replace(list, pairs);
    }
}
