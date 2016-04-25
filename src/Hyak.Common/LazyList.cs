using System.Collections;
using System.Collections.Generic;

namespace Sandboxable.Hyak.Common
{
    public class LazyList<T> : IList<T>, ILazyCollection
    {
        private IList<T> _internalList;

        private IList<T> InternalList
        {
            get
            {
                if (this._internalList == null)
                {
                    this._internalList = new List<T>();
                }
                return this._internalList;
            }
            set
            {
                this._internalList = value;
            }
        }

        public bool IsInitialized => this._internalList != null;

        public T this[int index]
        {
            get
            {
                return this.InternalList[index];
            }
            set
            {
                this.InternalList[index] = value;
            }
        }

        public int Count => this.InternalList.Count;

        public bool IsReadOnly => this.InternalList.IsReadOnly;

        public LazyList()
        {
        }

        public LazyList(IEnumerable<T> collection)
        {
            this.InternalList = new List<T>(collection);
        }

        public LazyList(int capacity)
        {
            this.InternalList = new List<T>(capacity);
        }

        public int IndexOf(T item)
        {
            return this.InternalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.InternalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.InternalList.RemoveAt(index);
        }

        public void Add(T item)
        {
            this.InternalList.Add(item);
        }

        public void Clear()
        {
            this.InternalList.Clear();
        }

        public bool Contains(T item)
        {
            return this.InternalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.InternalList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return this.InternalList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InternalList.GetEnumerator();
        }
    }
}
