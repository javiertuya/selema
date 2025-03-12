using System.Collections.Generic;

namespace Java.Util
{
    /// <summary>
    /// Use this interface to implement a java map in C#.
    /// Completes the dictionary interface with some methods needed to implement a java map.
    /// </summary>
    public interface Map<K, V> : IDictionary<K, V>
    {
        void Put(K key, V value);
        void PutAll(Map<K, V> map);
        ICollection<K> KeySet();
        bool IsEmpty();
    }
}
