using System.Collections.Generic;

namespace Giis.Selema.Portable.Selenium
{
    /// <summary>
    /// A wrapper of a Map (Java) or IDictionary (.NET) to allow compatibility when
    /// converting Java to.NET using JavaToCSharp. 
    /// Default implementation is 
    /// - HashMap(Java) and Dictionary(.NET) for non sorted maps
    /// - TreeMap(Java) and SortedDictionary(.NET) for sorted maps
    /// </summary>
    public class JavaMap<K, V>
    {

        readonly IDictionary<K, V> content; // wrapped dictionary

        /// <summary>
        /// Wraps a dictionary object
        /// </summary>
        public JavaMap(IDictionary<K, V> map)
        {
            this.content = map;
        }

        /// <summary>
        /// Instantiates a dictionary
        /// </summary>
        public JavaMap() : this(false)
        {
        }

        /// <summary>
        /// If sorted is true, instantiates a SortedDictionary, else instantiates a Dictionary
        /// </summary>
        public JavaMap(bool sorted)
        {
            if (sorted)
                this.content = new SortedDictionary<K, V>();
            else
                this.content = new Dictionary<K, V>();
        }

        /// <summary>
        /// Returns the wrapped dictionary
        /// </summary>
        public IDictionary<K, V> Unwrap()
        {
            return this.content;
        }

        // Methods to access the dictionary (java style)

        public V Get(K key)
        {
            return this.content[key];
        }
        public void Put(K key, V value)
        {
            this.content.Add(key, value);
        }

        public void PutAll(IDictionary<K, V> map)
        {
            foreach (K key in map.Keys)
            {
                this.content.Add(key, map[key]);
            }
        }
        public void PutAll(JavaMap<K, V> map)
        {
           this.PutAll(map.Unwrap());
        }

        public ICollection<K> KeySet()
        {
            return this.content.Keys;
        }

        // To allow alternative access to the dictionary using the [] operator
        public V this[K key]
        {
            get { return Get(key); }
            set { Put(key, value); }
        }


    }
}
