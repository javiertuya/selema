using System.Collections.Generic;
using System.Linq;

namespace Java.Util
{
    /// <summary>
    /// Implmentation of a TreeMap in C#:
    /// Duplicates the HashMap, but inherits from SortedDictionary instead of Dictionary.
    /// TODO: Avoid duplicating using default interfaces implementations when moving to netstandard2.1.
    /// </summary>
    public class TreeMap<K, V> : SortedDictionary<K, V>, Map<K, V>
    {
        public TreeMap() : base() { }

        // Methods of the IDictionary interface with modified behaviour

        public new V this[K key]
        {
            // get should return null instead of failing if key is not found
            get { return ContainsKey(key) ? base[key] : default; }
            set { base[key] = value; }
        }

        public override string ToString()
        {
            return "{" + string.Join(", ", this.Select(m => $"{m.Key}={m.Value}")) + "}";
        }

        // Methods added by the Map interface

        public void Put(K key, V value)
        {
            this[key] = value; // updates if already exists
        }

        public void PutAll(Map<K, V> map)
        {
            foreach (K key in map.Keys)
                this[key] = map[key];
        }

        public ICollection<K> KeySet()
        {
            return Keys;
        }

        public bool IsEmpty()
        {
            return Count == 0;
        }

    }
}
