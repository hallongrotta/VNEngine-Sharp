using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VNEngine;

namespace SceneSaveState
{
    public class Manager<T> where T : IManaged<T>
    {

        protected List<T> Items;

        private int _index;

        private string[] _itemNames;

        public string[] ItemNames
        {
            get
            {
                if (_itemNames != null && _itemNames.Length == Items.Count) return _itemNames;
                RebuildItemNames();
                return _itemNames;
            }
            set
            {
                _itemNames = value;
            }
        }

        public Manager()
        {
            Items = new List<T>();
            CurrentIndex = -1;
        }

        public Manager(List<T> items, int currentIndex = 0)
        {
            this.Items = items;
            this.CurrentIndex = currentIndex;
        }

        public Manager(List<T> items, string[] itemNames, int currentIndex = 0) : this(items, currentIndex) 
        {
            this.ItemNames = itemNames;
        }

        public int CurrentIndex
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

        public T Current
        {
            get => HasItems ? Items[CurrentIndex] : default;
            private set => Items[CurrentIndex] = value;
        }

        public T this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public bool HasItems => CurrentIndex > -1;


        public int Add(T i)
        {
            Items.Add(i);
            CurrentIndex = Items.Count - 1;
            return CurrentIndex;
        }
        public void Insert(T i)
        {
            Insert(i, CurrentIndex);
        }

        public void Prepend(T i)
        {
            Items.Insert(0, i);
            CurrentIndex = 0;
        }

        public List<T> RemoveUntilEnd(int from)
        {
            var items = Items.GetRange(from, Count - from);
            Items.RemoveRange(from, Count - from);
            return items;
        }

        public void AddRange(List<T> items)
        {
            Items.AddRange(items);
        }

        public void Insert(T i, int position)
        {
            Items.Insert(position + 1, i);
            CurrentIndex++;
        }

        public int Remove()
        {
            return Remove(CurrentIndex);
        }

        public int Remove(int position)
        {
            if (Items.Count <= 0 || position >= Items.Count || position <= -1) return CurrentIndex;
            Items.RemoveAt(position);
            CurrentIndex--;
            return CurrentIndex;
        }

        public void Update(T c)
        {
            Update(CurrentIndex, c);
        }

        public void Duplicate()
        {
            if (Items.Count > 0)
            {
                Items.Insert(CurrentIndex, Items[CurrentIndex].Copy());
            }
        }

        public T[] ExportItems()
        {
            return Items.ToArray();
        }

        public virtual T Update(int position, T newItem)
        {
            if (position >= Items.Count) return default;
            var oldItem = Items[position];
            Items[position] = newItem;
            return oldItem;
        }

        internal void RebuildItemNames()
        {
            var newSceneStrArray = new string[Items.Count];
            int id;
            for (id = 0; id < Items.Count; id++)
            {
                if (Items[id].name is null)
                {
                    newSceneStrArray[id] = $"{Items[id].TypeName} {id + 1}";
                }
                else
                {
                    newSceneStrArray[id] = Items[id].name;
                }

            }
            _itemNames = newSceneStrArray;
        }

        public void MoveItemForward()
        {
            if (CurrentIndex >= Items.Count - 1) return;
            var forwardedItem = Current;
            Current = Items[CurrentIndex + 1];
            CurrentIndex += 1;
            Current = forwardedItem;
            RebuildItemNames();
        }

        public int Count => Items.Count;

        public void MoveItemBack()
        {
            if (Items.Count <= 1 || this.CurrentIndex == 0) return;
            var backedItem = Current;
            Current = Items[CurrentIndex - 1];
            CurrentIndex -= 1;
            Current = backedItem;
            RebuildItemNames();
        }

        public bool HasNext => CurrentIndex < Items.Count - 1;

        public bool HasPrev => CurrentIndex > 0;

        public T SetCurrent(int index)
        {
            if (!HasItems || index >= Items.Count) return default;
            CurrentIndex = index;
            return Current;
        }

        public T Next()
        {
            if (!HasNext) return default;
            CurrentIndex++;
            return Current;
        }

        public T Back()
        {
            if (!HasPrev) return default;
            CurrentIndex--;
            return Current;
        }

        public T First()
        {
            if (!HasItems) return default;
            CurrentIndex = 0;
            return Current;
        }

        public T Last()
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

    }
}
