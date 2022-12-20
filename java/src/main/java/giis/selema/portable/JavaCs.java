package giis.selema.portable;

import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Date;
import java.util.Map;

/**
 * Miscelaneous utilities for compatibility Java/C#
 */
public class JavaCs {
	private JavaCs() {
	    throw new IllegalAccessError("Utility class");
	}
	public static String substring(String fromString, int beginIndex) {
		return fromString.substring(beginIndex);
	}
	public static String substring(String fromString, int beginIndex, int endIndex) {
		return fromString.substring(beginIndex, endIndex);
	}
	public static String intToString(int value) {
		return Integer.toString(value);
	}
	public static String deepToString(String[] strArray) {
		return Arrays.deepToString(strArray);
	}
	public static void putAll(Map<String,Object> targetMap, Map<String,Object> mapToAdd) {
		targetMap.putAll(mapToAdd);
	}
	public static boolean isEmpty(String str) {
		return str==null || "".equals(str.trim());
	}
	/**
	 * Remplazo usando una expresion regular,
	 * necesario porque en java es replaceAll pero en .net se debe usar
	 * regex y sharpen lo traduce por un simple replace.
	 */
	public static String replaceRegex(String str, String regex, String replacement) {
		return str.replaceAll(regex, replacement);
	}
	//necesario porque la traduccion a c# no interpreta el argumento \\. como expresion regular
	public static String[] splitByDot(String str) {
		return str.split("\\.");
	}
	
	public static String getEnvironmentVariable(String name) {
		return System.getenv(name); //NOSONAR
	}
	
	public static Date getCurrentDate() {
		return new Date();
	}
	public static String getTime(Date date) {
		return new SimpleDateFormat("HH:mm:ss").format(date);
	}
	public static long currentTimeMillis() {
		return System.currentTimeMillis();
	}
	public static void sleep(long millis) {
		try {
			Thread.sleep(millis);
		} catch (Exception e1) { //NOSONAR
			throw new SelemaException("Exception in Thread.Sleep",e1);
		}
	}
}
