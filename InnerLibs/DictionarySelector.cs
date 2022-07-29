using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs
{
    /// <summary>
    /// Uma estrutura <see cref="IDictionary"/> que utiliza como Key uma propriedade de Value
    /// </summary>
    /// <typeparam name="KeyType">Tipo da Key</typeparam>
    /// <typeparam name="ClassType">Tipo da</typeparam>
    public class SelfKeyDictionary<KeyType, ClassType> : IDictionary<KeyType, ClassType>, IDictionary
        where KeyType : IComparable
        where ClassType : class
    {
        private List<ClassType> collection = new List<ClassType>();
        private Func<ClassType, KeyType> keyselector;

        public void Add(KeyType key, ClassType value)
        {
            if (value != null)
            {
                if (!ContainsKey(keyselector(value)) && !ContainsKey(key))
                {
                    collection.Add(value);
                }
            }
        }




        private IEnumerator IEnumerable_GetEnumerator() => ((IEnumerable)this).GetEnumerator();

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

        public int Count => collection.Count;

        public bool IsReadOnly => false;

        public ICollection<KeyType> Keys => collection?.Select(x => keyselector(x)).ToArray();

        public ICollection<ClassType> Values => collection;

        ICollection IDictionary.Keys => (ICollection)Keys;

        ICollection IDictionary.Values => (ICollection)Values;

        public bool IsFixedSize => false;

        public object SyncRoot => SyncRoot;

        public bool IsSynchronized => true;

        public object this[object key] { get => this[((KeyType)key)]; set => this[((KeyType)key)] = (ClassType)value; }

        public ClassType this[KeyType key]
        {
            get => collection.Where(x => keyselector(x).Equals(key)).SingleOrDefault();

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

        public KeyType Add(ClassType Value)
        {
            if (Value != null)
            {
                Add(keyselector(Value), Value);
                return keyselector(Value);
            }

            return default;
        }

        public IEnumerable<KeyType> AddRange(params ClassType[] Values) => AddRange((Values ?? Array.Empty<ClassType>()).AsEnumerable());

        public IEnumerable<KeyType> AddRange(IEnumerable<ClassType> Values)
        {
            Values = (Values ?? Array.Empty<ClassType>()).Where(x => x != null).AsEnumerable();
            foreach (var value in Values)
            {
                Add(value);
            }

            return Values.Select(x => keyselector(x));
        }

        public void Clear() => collection.Clear();

        public bool Contains(KeyValuePair<KeyType, ClassType> item) => collection.Any(x => keyselector(x).Equals(item.Key));

        public bool ContainsKey(KeyType key) => Keys.Contains(key);

        public void CopyTo(KeyValuePair<KeyType, ClassType>[] array, int arrayIndex) => collection.Select(x => new KeyValuePair<KeyType, ClassType>(keyselector(x), x)).ToArray().CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<KeyType, ClassType>> GetEnumerator() => collection.Select(x => new KeyValuePair<KeyType, ClassType>(keyselector(x), x)).GetEnumerator();

        public bool Remove(KeyType key)
        {
            if (ContainsKey(key))
            {

                var success = Misc.TryExecute(() =>
                  {
                      var ii = collection.FirstOrDefault(x => keyselector(x).Equals(key));

                      if (ii != null)
                      {
                          collection.Remove(ii);
                      }
                  }) == null;

                return success;

            }

            return false;

        }

        public bool Remove(ClassType Value) => Remove(keyselector(Value));

        public bool Remove(KeyValuePair<KeyType, ClassType> item) => collection.Remove(item.Value);

        public bool TryGetValue(KeyType key, out ClassType value)
        {

            try
            {
                value = collection.SingleOrDefault(x => keyselector(x).Equals(key));
                return value != null;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        void ICollection<KeyValuePair<KeyType, ClassType>>.Add(KeyValuePair<KeyType, ClassType> item) => collection.Add(item.Value);

        void IDictionary<KeyType, ClassType>.Add(KeyType key, ClassType value) => Add(key, value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(object key) => key != null && key is KeyType ckey && ContainsKey(ckey);

        public void Add(object key, object value)
        {
            if (key is KeyType && value is ClassType cvalue)
            {
                collection.Add(cvalue);
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            if (key is KeyType ckey)
            {
                collection.Remove(this[ckey]);
            }
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            ClassType[] ppArray = array as ClassType[];
            if (ppArray == null)
            {
                throw new ArgumentException();
            } ((ICollection<ClassType>)this).CopyTo(ppArray, index);
        }
    }
}