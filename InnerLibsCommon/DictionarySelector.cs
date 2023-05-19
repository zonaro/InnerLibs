using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;



namespace Extensions.Dictionaries
{


    /// <summary>
    /// Uma estrutura <see cref="IDictionary"/> que utiliza como Key uma propriedade de Value
    /// </summary>
    /// <typeparam name="TKey">Tipo da Key</typeparam>
    /// <typeparam name="TClass">Tipo da</typeparam>
    public class SelfKeyDictionary<TKey, TClass> : IDictionary<TKey, TClass>, IDictionary
            where TKey : IComparable
            where TClass : class
    {
        #region Private Fields

        private List<TClass> collection = new List<TClass>();
        private Func<TClass, TKey> keyselector;

        #endregion Private Fields

        #region Public Constructors

        public SelfKeyDictionary(Func<TClass, TKey> KeySelector, Func<TClass, IComparable> AutoSortProperty) : this(KeySelector)
        {
            this.AutoSortProperty = AutoSortProperty;
        }

        public SelfKeyDictionary(Func<TClass, TKey> KeySelector)
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

        public Func<TClass, IComparable> AutoSortProperty { get; set; }



        public object this[object key]
        {
            get => this[((TKey)key)];
            set => this[((TKey)key)] = (TClass)value;
        }

        public TClass this[TKey key]
        {
            get => collection.SingleOrDefault(x => keyselector(x).Equals(key));

            set
            {
                int indexo = collection.IndexOf(value ?? this[key]);
                if (indexo > -1)
                {
                    collection.RemoveAt(indexo);
                    if (value != null)
                    {
                        collection.Insert(indexo, value);
                    }
                }
                else
                {
                    if (value != null && !ContainsKey(keyselector(value)) && !ContainsKey(key))
                    {
                        collection.Add(value);
                    }
                }

                if (AutoSortProperty != null)
                {
                    collection = collection.OrderBy(x => AutoSortProperty.Invoke(x)).ToList();
                }

            }
        }

        #endregion Public Indexers

        #region Public Properties

        public int Count => collection.Count;

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public bool IsSynchronized => true;

        public ICollection<TKey> Keys => collection?.Select(x => keyselector(x)).ToArray();

        public object SyncRoot { get; }

        public ICollection<TClass> Values => collection.ToArray();

        ICollection IDictionary.Keys => (ICollection)Keys;

        ICollection IDictionary.Values => (ICollection)Values;

        #endregion Public Properties

        #region Public Methods

        public void Add(TKey key, TClass value) => this[key] = value;

        public TKey Add(TClass Value)
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
            if (key is TKey && value is TClass cvalue)
            {
                Add(cvalue);
            }
        }

        public IEnumerable<TKey> AddRange(params TClass[] Values) => AddRange((Values ?? Array.Empty<TClass>()).AsEnumerable());

        public IEnumerable<TKey> AddRange(IEnumerable<TClass> Values)
        {
            Values = (Values ?? Array.Empty<TClass>()).Where(x => x != null).AsEnumerable();
            Values.Each(value => Add(value));
            return Values.Select(x => keyselector(x));
        }

        public void Clear() => collection.Clear();

        public bool Contains(KeyValuePair<TKey, TClass> item) => collection.Any(x => keyselector(x).Equals(item.Key));

        public bool Contains(object key)
        {
            if (key != null)
            {
                if (key is TKey ckey)
                {
                    return ContainsKey(ckey);
                }
                else if (key is TClass cvalue)
                {
                    return ContainsKey(this.keyselector(cvalue));
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key) => Keys.Contains(key);

        public void CopyTo(KeyValuePair<TKey, TClass>[] array, int arrayIndex) => collection.Select(x => new KeyValuePair<TKey, TClass>(keyselector(x), x)).ToArray().CopyTo(array, arrayIndex);

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

        public IEnumerator<KeyValuePair<TKey, TClass>> GetEnumerator() => collection.Select(x => new KeyValuePair<TKey, TClass>(keyselector(x), x)).GetEnumerator();

        public bool Remove(TKey key)
        {
            try
            {
                if (ContainsKey(key))
                {
                    var ii = this[key];

                    if (ii != null)
                    {
                        return collection.Remove(ii);
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        public bool Remove(TClass Value) => Remove(keyselector(Value));

        public bool Remove(KeyValuePair<TKey, TClass> item) => Remove(item.Key);

        public void Remove(object key)
        {
            if (key is TKey ckey)
            {
                Remove(ckey);
            }
        }

        public bool TryGetValue(TKey key, out TClass value)
        {
            try
            {
                value = this[key];
                return value != null;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        void ICollection<KeyValuePair<TKey, TClass>>.Add(KeyValuePair<TKey, TClass> item) => Add(item.Value);

        void IDictionary<TKey, TClass>.Add(TKey key, TClass value) => Add(key, value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => (this.ToDictionary(x => x.Key, x => x.Value) as IDictionary).GetEnumerator();

        #endregion Public Methods
    }

}