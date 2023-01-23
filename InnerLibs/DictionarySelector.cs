using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs
{
    /// <summary>
    /// Uma estrutura <see cref="IDictionary"/> que utiliza como Key uma propriedade de Value
    /// </summary>
    /// <typeparam name="TK">Tipo da Key</typeparam>
    /// <typeparam name="TClass">Tipo da</typeparam>
    public class SelfKeyDictionary<TK, TClass> : IDictionary<TK, TClass>, IDictionary
        where TK : IComparable
        where TClass : class
    {
        #region Private Fields

        private List<TClass> collection = new List<TClass>();
        private Func<TClass, TK> keyselector;

        #endregion Private Fields

        #region Public Constructors

        public SelfKeyDictionary(Func<TClass, TK> KeySelector)
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

        #endregion Public Constructors

        #region Public Indexers

        public object this[object key] { get => this[((TK)key)]; set => this[((TK)key)] = (TClass)value; }

        public TClass this[TK key]
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

        #endregion Public Indexers

        #region Public Properties

        public int Count => collection.Count;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => true;

        public ICollection<TK> Keys => collection?.Select(x => keyselector(x)).ToArray();

        public object SyncRoot => SyncRoot;

        public ICollection<TClass> Values => collection;

        ICollection IDictionary.Keys => (ICollection)Keys;

        ICollection IDictionary.Values => (ICollection)Values;

        #endregion Public Properties

        #region Public Methods

        public void Add(TK key, TClass value)
        {
            if (value != null)
            {
                if (!ContainsKey(keyselector(value)) && !ContainsKey(key))
                {
                    collection.Add(value);
                }
            }
        }

        public TK Add(TClass Value)
        {
            if (Value != null)
            {
                Add(keyselector(Value), Value);
                return keyselector(Value);
            }

            return default;
        }

        public void Add(object key, object value)
        {
            if (key is TK && value is TClass cvalue)
            {
                collection.Add(cvalue);
            }
        }

        public IEnumerable<TK> AddRange(params TClass[] Values) => AddRange((Values ?? Array.Empty<TClass>()).AsEnumerable());

        public IEnumerable<TK> AddRange(IEnumerable<TClass> Values)
        {
            Values = (Values ?? Array.Empty<TClass>()).Where(x => x != null).AsEnumerable();
            foreach (var value in Values)
            {
                Add(value);
            }

            return Values.Select(x => keyselector(x));
        }

        public void Clear() => collection.Clear();

        public bool Contains(KeyValuePair<TK, TClass> item) => collection.Any(x => keyselector(x).Equals(item.Key));

        public bool Contains(object key) => key != null && key is TK ckey && ContainsKey(ckey);

        public bool ContainsKey(TK key) => Keys.Contains(key);

        public void CopyTo(KeyValuePair<TK, TClass>[] array, int arrayIndex) => collection.Select(x => new KeyValuePair<TK, TClass>(keyselector(x), x)).ToArray().CopyTo(array, arrayIndex);

        public void CopyTo(Array array, int index)
        {

            if (array != null && array is TClass[] ppArray)
            {
                ((ICollection<TClass>)this).CopyTo(ppArray, index);
            }
            else
            {
                throw new ArgumentNullException(nameof(array));
            }

        }

        public IEnumerator<KeyValuePair<TK, TClass>> GetEnumerator() => collection.Select(x => new KeyValuePair<TK, TClass>(keyselector(x), x)).GetEnumerator();

        public bool Remove(TK key) => ContainsKey(key) && Misc.TryExecute(() =>
                    {
                        var ii = collection.FirstOrDefault(x => keyselector(x).Equals(key));

                        if (ii != null)
                        {
                            collection.Remove(ii);
                        }
                    }) == null;

        public bool Remove(TClass Value) => Remove(keyselector(Value));

        public bool Remove(KeyValuePair<TK, TClass> item) => collection.Remove(item.Value);

        public void Remove(object key)
        {
            if (key is TK ckey)
            {
                collection.Remove(this[ckey]);
            }
        }

        public bool TryGetValue(TK key, out TClass value)
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

        void ICollection<KeyValuePair<TK, TClass>>.Add(KeyValuePair<TK, TClass> item) => collection.Add(item.Value);

        void IDictionary<TK, TClass>.Add(TK key, TClass value) => Add(key, value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion Public Methods
    }
}