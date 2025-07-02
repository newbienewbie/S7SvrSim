using DynamicData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace S7SvrSim.Services.Command
{
    internal class ListChangedCommand<T> : IHistoryCommand
        where T : class
    {
        protected class ListItem
        {
            public int Index { get; set; } = -1;
            public T Item { get; set; }
        }

        protected readonly IList<T> list;
        private readonly int startIndex;
        protected readonly ChangedType changedType;
        protected readonly IList<ListItem> newItems;
        protected readonly IList<ListItem> oldItems;

        private ListChangedCommand(IList<T> list, int startIndex, ChangedType changedType, IEnumerable<T> newItems, IEnumerable<T> oldItems)
        {
            this.list = list;
            this.startIndex = startIndex;
            this.changedType = changedType;
            this.newItems = newItems?.Select(item => new ListItem() { Index = list.IndexOf(item), Item = item }).ToList() ?? [];
            this.oldItems = oldItems?.Select(item => new ListItem() { Index = list.IndexOf(item), Item = item }).ToList() ?? [];
        }

        public static ListChangedCommand<T> Add(IList<T> list, IEnumerable<T> added) => new ListChangedCommand<T>(list, -1, ChangedType.Add, added.Except(list).Where(item => item != null).ToList(), null);
        public static ListChangedCommand<T> Insert(IList<T> list, int index, IEnumerable<T> added) => new ListChangedCommand<T>(list, index, ChangedType.Add, added.Except(list).Where(item => item != null).ToList(), null);
        public static ListChangedCommand<T> Remove(IList<T> list, IEnumerable<T> remove) => new ListChangedCommand<T>(list, -1, ChangedType.Remove, null, remove.Intersect(list).Where(item => item != null).ToList());
        public static ListChangedCommand<T> Clear(IList<T> list) => new ListChangedCommand<T>(list, -1, ChangedType.Clear, null, null);
        public static ListChangedCommand<T> Replace(IList<T> list, IEnumerable<(T OldItem, T NewItem)> pairs) => new ListChangedCommand<T>(list, -1, ChangedType.Replace, pairs.IntersectBy(list, p => p.OldItem).Where(item => item.OldItem != null && item.NewItem != null).Select(p => p.NewItem).ToList(), pairs.Select(p => p.OldItem).ToList());
        public static ListChangedCommand<T> Move(IList<T> list, T before, IEnumerable<T> moved) => new ListChangedCommand<T>(list, -1, ChangedType.Move, before == null ? null : [before], moved);
        public static ListChangedCommand<T> OrderBy<TKey>(IList<T> list, Func<T, TKey> keySelector) => new ListChangedCommand<T>(list, -1, ChangedType.Order, list.OrderBy(keySelector), list);
        public static ListChangedCommand<T> OrderByDescending<TKey>(IList<T> list, Func<T, TKey> keySelector) => new ListChangedCommand<T>(list, -1, ChangedType.Order, list.OrderByDescending(keySelector), list);

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
                list.AddOrInsertRange(newItems.Select(item => item.Item), first);
            }
            else
            {
                list.AddOrInsertRange(newItems.Select(item => item.Item), startIndex);
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

            foreach (var item in oldItems.OrderBy(o => o.Index))
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

        private void Move()
        {
            var moveItems = oldItems.OrderBy(o => o.Index).Select(o => o.Item);
            list.RemoveMany(moveItems);
            list.AddOrInsertRange(moveItems, newItems.Count > 0 ? list.IndexOf(newItems[0].Item) : -1);
        }

        private void MoveBack()
        {
            list.RemoveMany(oldItems.Select(o => o.Item));
            foreach (var item in oldItems.OrderBy(o => o.Index))
            {
                list.Insert(item.Index, item.Item);
            }
        }

        private void Order()
        {
            var items = newItems.Select(item => item.Item);
            ArrayList.Adapter((IList)list).Sort(Comparer<T>.Create((item1, item2) =>
            {
                return items.IndexOf(item1) - items.IndexOf(item2);
            }));
        }

        private void OrderBack()
        {
            var items = oldItems.Select(item => item.Item);
            ArrayList.Adapter((IList)list).Sort(Comparer<T>.Create((item1, item2) =>
            {
                return items.IndexOf(item1) - items.IndexOf(item2);
            }));
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
                case ChangedType.Move:
                    Move();
                    break;
                case ChangedType.Order:
                    Order();
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
                case ChangedType.Move:
                    MoveBack();
                    break;
                case ChangedType.Order:
                    OrderBack();
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
        public static ListChangedCommand<T> Insert<T>(IList<T> list, int index, IEnumerable<T> added) where T : class => ListChangedCommand<T>.Insert(list, index, added);
        public static ListChangedCommand<T> Remove<T>(IList<T> list, IEnumerable<T> remove) where T : class => ListChangedCommand<T>.Remove(list, remove);
        public static ListChangedCommand<T> Clear<T>(IList<T> list) where T : class => ListChangedCommand<T>.Clear(list);
        public static ListChangedCommand<T> Replace<T>(IList<T> list, IEnumerable<(T OldItem, T NewItem)> pairs) where T : class => ListChangedCommand<T>.Replace(list, pairs);
        public static ListChangedCommand<T> Move<T>(IList<T> list, T before, IEnumerable<T> moved) where T : class => ListChangedCommand<T>.Move(list, before, moved);
        public static ListChangedCommand<T> OrderBy<T, TKey>(IList<T> list, Func<T, TKey> keySelector) where T : class => ListChangedCommand<T>.OrderBy(list, keySelector);
        public static ListChangedCommand<T> OrderByDescending<T, TKey>(IList<T> list, Func<T, TKey> keySelector) where T : class => ListChangedCommand<T>.OrderByDescending(list, keySelector);
    }
}
