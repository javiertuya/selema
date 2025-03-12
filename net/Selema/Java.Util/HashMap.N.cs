using System.Collections.Generic;
using System.Linq;

namespace Java.Util
{
    /// <summary>
    /// Implmentation of a HashMap in C#:
    /// Modifies the behaviour or some methods to match the Java implementation
    /// (e.g. get returns null instead of failing if key is not found)
    /// and adds some methods specified in the Map interface.
    /// 
    /// To use this class after conversion with JavaToCSharp you only need
    /// to rename Dictionary by Map (it does not need namespace because the Map
    /// implementation in C# and this class are in the Java.Util namespace).
    /// </summary>
    public class HashMap<K, V> : Dictionary<K, V>, Map<K, V>
    {
        public HashMap() : base() { }

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
