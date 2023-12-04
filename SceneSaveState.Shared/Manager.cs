using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VNEngine;

namespace SceneSaveState
{
    public class Manager<T> where T : IManaged<T>
    {

        private readonly List<T> items = new List<T>();

        protected List<T> Items { get => items; }

        private int _index;

        private string[] _itemNames;

        internal string[] ItemNames
        {
            get
            {
                if (_itemNames != null && _itemNames.Length == Items.Count) return _itemNames;
                _itemNames = RebuildItemNames();
                return _itemNames;
            }
            set
            {
                _itemNames = value;
            }
        }

        internal Manager()
        {
            CurrentIndex = -1;
        }

        internal Manager(List<T> items, int currentIndex = 0)
        {
            this.ImportItems(items);
            this.CurrentIndex = currentIndex;
        }

        internal Manager(List<T> items, string[] itemNames, int currentIndex = 0) : this(items, currentIndex) 
        {
            this.ItemNames = itemNames;
        }

        internal int CurrentIndex
        {
            get
            {
                return _index;
            }
            private set
            {
                if (value >= Items.Count) return;
                if (value == -1 && Items.Count == 0)
                {
                    _index = value;
                }
                else if (value == -1 && Items.Count > 0)
                {
                }
                else
                {
                    _index = value;
                }
            }
        }

        internal T Current
        {
            get => HasItems ? Items[CurrentIndex] : default;
            private set => Items[CurrentIndex] = value;
        }

        internal T this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        internal bool HasItems => CurrentIndex > -1 && Items.Count > 0;


        internal T Add(T i)
        {
            Items.Add(i);
            CurrentIndex = Items.Count - 1;
            return Current;
        }
        internal T Insert(T i)
        {
            return Insert(i, CurrentIndex);
        }

        internal void Prepend(T i)
        {
            Items.Insert(0, i);
            CurrentIndex = 0;
        }

        internal List<T> RemoveUntilEnd(int from)
        {
            var items = Items.GetRange(from, Count - from);
            Items.RemoveRange(from, Count - from);
            return items;
        }

        internal void AddRange(List<T> items)
        {
            Items.AddRange(items);
        }

        internal T Insert(T i, int position)
        {
            Items.Insert(position + 1, i);
            CurrentIndex++;
            return i;
        }

        internal T Remove()
        {
            return Remove(CurrentIndex);
        }

        internal T Remove(int position)
        {
            var item = Items[position];
            if (Items.Count <= 0 || position >= Items.Count || position <= -1) return item;
            Items.RemoveAt(position);
            CurrentIndex--;
            return item;
        }

        internal T Update(T c)
        {
            return Update(CurrentIndex, c);
        }

        internal T Duplicate()
        {
            return Items.Count > 0 ? Insert(Items[CurrentIndex].Copy()) : default;
        }

        internal T[] ExportItems()
        {
            return Items.ToArray();
        }

        internal void ImportItems(T[] items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
            CurrentIndex = 0;
        }

        internal void ImportItems(List<T> items)
        {
            if (items is null) return;
            foreach (var item in items)
            {
                Add(item);
            }
            CurrentIndex = 0;
        }

        internal virtual T Update(int position, T newItem)
        {
            if (position >= Items.Count) return default;
            var oldItem = Items[position];
            Items[position] = newItem;
            return oldItem;
        }

        internal string[] RebuildItemNames()
        {
            return Items.Select((x, i) => x.Name is null ? $"{x.TypeName} {i + 1}" : x.Name).ToArray();
        }

        internal void MoveItemForward()
        {
            if (CurrentIndex >= Items.Count - 1) return;
            var forwardedItem = Current;
            Current = Items[CurrentIndex + 1];
            CurrentIndex += 1;
            Current = forwardedItem;
            _itemNames = RebuildItemNames();
        }

        internal int Count => Items.Count;

        internal void MoveItemBack()
        {
            if (Items.Count <= 1 || this.CurrentIndex == 0) return;
            var backedItem = Current;
            Current = Items[CurrentIndex - 1];
            CurrentIndex -= 1;
            Current = backedItem;
            _itemNames = RebuildItemNames();
        }

        internal bool HasNext => CurrentIndex < Items.Count - 1;

        internal bool HasPrev => CurrentIndex > 0;

        internal T SetCurrent(int index)
        {
            if (!HasItems || index >= Items.Count) return default;
            CurrentIndex = index;
            return Current;
        }

        internal T Next()
        {
            if (!HasNext) return default;
            CurrentIndex++;
            return Current;
        }

        internal T Back()
        {
            if (!HasPrev) return default;
            CurrentIndex--;
            return Current;
        }

        internal T First()
        {
            if (!HasItems) return default;
            CurrentIndex = 0;
            return Current;
        }

        internal T Last()
        {
            if (!HasItems) return default;
            CurrentIndex = Count - 1;
            return Current;
        }

        internal string ExportItemNames()
        {
            var s = "";

            for (var i = 0; i < ItemNames.Length; i++)
            {
                var sceneString = ItemNames[i];
                if (sceneString is null)
                {
                    s += '\n';
                }
                else
                {
                    s += (i == ItemNames.Length - 1) ? sceneString : sceneString + "\n";
                }
            }
            return s;
        }

        internal static string[] DeserializeItemNames(string s)
        {
            var list = s?.Split('\n');
            return list;
        }

        internal void SetName(IManaged<T> item, string name)
        {
            item.Name = name == "" ? null : name;
            ItemNames = RebuildItemNames();
        }

    }
}
