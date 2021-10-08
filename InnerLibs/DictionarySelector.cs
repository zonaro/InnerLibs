/// <summary>
/// Uma estrutura <see cref="IDictionary"/> que utiliza como Key uma propriedade de Value
/// </summary>
/// <typeparam name="KeyType">Tipo da Key</typeparam>
/// <typeparam name="ClassType">Tipo da</typeparam>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs
{
    public class SelfKeyDictionary<KeyType, ClassType> : IDictionary<KeyType, ClassType>
        where KeyType : IComparable
        where ClassType : class
    {
        private Func<ClassType, KeyType> keyselector;

        public SelfKeyDictionary(Func<ClassType, KeyType> KeySelector)
        {
            if (KeySelector != null)
            {
                keyselector = KeySelector;
            }
            else
            {
                throw new ArgumentException("KeySelector cannot be NULL");
            }
        }

        public bool ContainsKey(KeyType key)
        {
            return Keys.Contains(key);
        }

        private void Add(KeyType key, ClassType value)
        {
            if (value != null)
            {
                if (!ContainsKey(keyselector(value)))
                {
                    collection.Add(value);
                }
            }
        }

        public KeyType Add(ClassType Value)
        {
            if (Value != null)
            {
                this.Add(keyselector(Value), Value);
                return keyselector(Value);
            }

            return default;
        }

        public IEnumerable<KeyType> AddRange(params ClassType[] Values)
        {
            return AddRange((Values ?? Array.Empty<ClassType>()).AsEnumerable());
        }

        public IEnumerable<KeyType> AddRange(IEnumerable<ClassType> Values)
        {
            Values = (Values ?? Array.Empty<ClassType>()).Where(x => x != null).AsEnumerable();
            foreach (var value in Values)
                Add(value);
            return Values.Select(x => keyselector(x));
        }

        public bool Remove(KeyType key)
        {
            var toremove = new List<ClassType>();
            foreach (var ii in collection.Where(x => keyselector(x).Equals(key)))
                toremove.Add(ii);
            foreach (var i in toremove)
                collection.Remove(i);
            return ContainsKey(key);
        }

        public bool Remove(ClassType Value)
        {
            return Remove(keyselector(Value));
        }

        public bool TryGetValue(KeyType key, out ClassType value)
        {
            try
            {
                value = collection.Where(x => keyselector(x).Equals(key)).Single();
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        void ICollection<KeyValuePair<KeyType, ClassType>>.Add(KeyValuePair<KeyType, ClassType> item)
        {
            collection.Add(item.Value);
        }

        public void Clear()
        {
            collection.Clear();
        }

        public bool Contains(KeyValuePair<KeyType, ClassType> item)
        {
            return collection.Any(x => keyselector(x).Equals(item.Key));
        }

        public void CopyTo(KeyValuePair<KeyType, ClassType>[] array, int arrayIndex)
        {
            collection.Select(x => new KeyValuePair<KeyType, ClassType>(keyselector(x), x)).ToArray().CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<KeyType, ClassType> item)
        {
            return collection.Remove(item.Value);
        }

        public IEnumerator<KeyValuePair<KeyType, ClassType>> GetEnumerator()
        {
            return collection.Select(x => new KeyValuePair<KeyType, ClassType>(keyselector(x), x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerator IEnumerable_GetEnumerator() => ((IEnumerable)this).GetEnumerator();

        void IDictionary<KeyType, ClassType>.Add(KeyType key, ClassType value)
        {
            this.Add(key, value);
        }

        private List<ClassType> collection = new List<ClassType>();

        public ClassType this[KeyType key]
        {
            get
            {
                return collection.Where(x => keyselector(x).Equals(key)).SingleOrDefault();
            }

            set
            {
                int indexo = collection.IndexOf(this[key]);
                if (indexo > -1)
                {
                    collection.RemoveAt(indexo);
                    collection.Insert(indexo, value);
                }
                else
                {
                    Add(value);
                }
            }
        }

        public ICollection<KeyType> Keys
        {
            get
            {
                return collection?.Select(x => keyselector(x)).ToArray();
            }
        }

        public ICollection<ClassType> Values
        {
            get
            {
                return collection;
            }
        }

        public int Count
        {
            get
            {
                return collection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
    }
}