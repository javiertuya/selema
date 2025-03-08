package giis.selema.portable.selenium;

import java.util.HashMap;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

/**
 * A wrapper of a Map (Java) or IDictionary (.NET) to allow compatibility when
 * converting Java to .NET using JavaToCSharp. 
 * Default implementation is 
 * - HashMap (Java) and Dictionary (.NET) for non sorted maps
 * - TreeMap (Java) and SortedDictionary (.NET) for sorted maps
 */
public class JavaMap<K, V> {

	Map<K, V> content; // wrapped map

	/**
	 * Wraps a Map object
	 */
	public JavaMap(Map<K, V> map) {
		this.content = map;
	}
	
	/**
	 * Instantiates a HashMap
	 */
	public JavaMap() {
		this(false);
	}
	
	/**
	 * If sorted is true, instantiates a TreeMap, else instantiates a HashMap
	 */
	public JavaMap(boolean sorted) {
		this.content = sorted ? new TreeMap<>() : new HashMap<>();
	}

	/**
	 * Returns the wrapped map
	 */
	public Map<K, V> unwrap() {
		return this.content;
	}

	// Methods to access the map
	
	public V get(K key) {
		return this.content.get(key);
	}
	
	public void put(K key, V value) {
		this.content.put(key, value);
	}
	
	public void putAll(Map<K, V> map) {
		this.content.putAll(map);
	}
	
	public void putAll(JavaMap<K, V> map) {
		this.content.putAll(map.unwrap());
	}
	
	public Set<K> keySet() {
		return this.content.keySet();
	}
}
