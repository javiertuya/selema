package giis.selema.portable;

import java.io.FileInputStream;
import java.io.IOException;

/**
 * Platform independent instantiation of Properties object.
 */
public class PropertiesFactory {
	public java.util.Properties getPropertiesFromFilename(String fileName) {
		try {
			java.util.Properties prop=new java.util.Properties();
			prop.load(new FileInputStream(fileName)); //NOSONAR no usa try-with-resources por compatibilidad java 1.6
			return prop;
		} catch (IOException e) {
			return null;
		}
	}
}
