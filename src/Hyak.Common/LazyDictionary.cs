using System;
using System.Collections;
using System.Collections.Generic;

namespace Sandboxable.Hyak.Common
{
    public class LazyDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ILazyCollection
    {
        private IDictionary<TKey, TValue> _internalDictionary;

        private IDictionary<TKey, TValue> InternalDictionary
        {
            get
            {
                if (this._internalDictionary == null)
                {
                    this._internalDictionary = new Dictionary<TKey, TValue>();
                }
                return this._internalDictionary;
            }
            set
            {
                this._internalDictionary = value;
            }
        }

        public bool IsInitialized => this._internalDictionary != null;

        public ICollection<TKey> Keys => this.InternalDictionary.Keys;

        public ICollection<TValue> Values => this.InternalDictionary.Values;

        public TValue this[TKey key]
        {
            get
            {
                return this.InternalDictionary[key];
            }
            set
            {
                this.InternalDictionary[key] = value;
            }
        }

        public int Count => this.InternalDictionary.Count;

        public bool IsReadOnly => this.InternalDictionary.IsReadOnly;

        public LazyDictionary()
        {
        }

        public LazyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            this.InternalDictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        public LazyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            this.InternalDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }

        public LazyDictionary(IEqualityComparer<TKey> comparer)
        {
            this.InternalDictionary = new Dictionary<TKey, TValue>(comparer);
        }

        public LazyDictionary(int capacity)
        {
            this.InternalDictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public LazyDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            this.InternalDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public void Add(TKey key, TValue value)
        {
            this.InternalDictionary.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            return this.InternalDictionary.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return this.InternalDictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return this.InternalDictionary.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            this.InternalDictionary.Add(item);
        }

        public void Clear()
        {
            this.InternalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return this.InternalDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.InternalDictionary.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.InternalDictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return this.InternalDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.InternalDictionary.GetEnumerator();
        }
    }
}
