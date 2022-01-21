package giis.selema.portable;

import java.io.File;
import java.io.FileFilter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.commons.io.FilenameUtils; //no usa java.nio.file.Paths por compatibilidad con java 1.6
import org.apache.commons.io.FileUtils;
import org.apache.commons.io.filefilter.WildcardFileFilter;

/**
 * File management for compatibility Java/C#
 */
public class FileUtil {
	private static final String UTF_8 = "UTF-8";
	private FileUtil() {
	    throw new IllegalAccessError("Utility class");
	  }
	
	/**
	 * Checks invalid characters: Although linux is more permisive than windows in characters allowed to filenames,
	 * when running in GitHub Actions, some characters are never allowed in actions such as publish artifacts
	 */
	public static void checkFileName(String name) throws IOException {
		String invalid="\"<>|*?\r\n";
	    for (char c : invalid.toCharArray()) {
			if (name.indexOf(c) != -1)
				throw new IOException("File name contains an invalid character: \" : < > | * ? \\r \\n");
	    }
	}
	public static String fileRead(String fileName) {
		try {
			checkFileName(fileName);
			File f=new File(fileName);
			return FileUtils.readFileToString(f, UTF_8);
		} catch (IOException e) {
			throw new SelemaException(e);
		}
	}
	public static List<String> fileReadLines(String fileName) {
		try {
			return FileUtils.readLines(new File(fileName),UTF_8);
		} catch (IOException e) {
			return new ArrayList<>();
		}
	}

	public static void fileWrite(String fileName, String contents) {
		try {
			checkFileName(fileName);
			FileUtils.writeStringToFile(new File(fileName), contents, UTF_8);
		} catch (IOException e) {
			throw new SelemaException("Error writing file "+fileName, e);
		}
	}
	public static void fileAppend(String fileName, String line) {
		try {
			checkFileName(fileName);
			FileUtils.writeStringToFile(new File(fileName), line, UTF_8, true);
		} catch (IOException e) {
			throw new SelemaException("Error appending to file "+fileName, e);
		}
	}
	public static void copyFile(File source, File dest) {
		try {
			checkFileName(dest.getName());
			FileUtils.copyFile(source,dest);
		} catch (IOException e) {
			throw new SelemaException("Error copying files", e);
		}
	}
	//devuelve un array con todos los ficheros de un folder que hacen match con una especificacion de fichero que contiene *
	public static File[] listFilesMatchingWildcard(String folder, String fileNameWildcard) {
		File dir = new File(folder);
		FileFilter fileFilter = new WildcardFileFilter(fileNameWildcard);
		return dir.listFiles(fileFilter);
	}

	public static String getPath(String first, String... more) {
		String result=first;
		//El primer componente no puede empezar de forma relativa como .. (concat devuelve null), por lo que busca el full path primero
		if (result.startsWith("."))
			result=getFullPath(result);
		for (int i=0; i<more.length; i++)
			result=FilenameUtils.concat(result, more[i]);
        return result;
    }

	public static String getFullPath(String path)
    {
        try {
			checkFileName(path);
			return new File(path).getCanonicalPath();
		} catch (IOException e) {
			throw new SelemaException("Error getting full path of "+path, e);
		}
    }

	public static void createDirectory(String path) {
		try {
			checkFileName(path);
			FileUtils.forceMkdir(new File(path));
		} catch (IOException e) {
			throw new SelemaException(e);
		}
	}

}
