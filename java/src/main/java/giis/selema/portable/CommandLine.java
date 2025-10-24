package giis.selema.portable;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringWriter;

import org.apache.commons.io.FileUtils;

public class CommandLine {

	private CommandLine() {
		throw new IllegalStateException("Utility class");
	}

	/**
	 * Runs a shell/cmd command and returns a string with the standard output and error
	 */
	public static String runCommand(String command) {
		String output;
		Process proc;
		try {
			proc = Runtime.getRuntime().exec(command);
		} catch (IOException e) {
			throw new VideoControllerException("Can't execute command: " + command);
		}

		InputStreamReader isr = new InputStreamReader(proc.getInputStream());
		BufferedReader br = new BufferedReader(isr);
		InputStreamReader eisr = new InputStreamReader(proc.getErrorStream());
		BufferedReader ebr = new BufferedReader(eisr);
		StringWriter sw = new StringWriter();
		try {
			while ((output = br.readLine()) != null)
				sw.append(output + "\n");
			while ((output = ebr.readLine()) != null)
				sw.append(output + "\n");
		} catch (IOException e) {
			throw new VideoControllerException("Can't get standard output from command: " + command);
		}
		return sw.toString();
	}

	// Temporal methods, to be included later in the portable component

	public static void fileDelete(String fileName, boolean throwIfNotExists) {
		try {
			File f = new File(fileName);
			if (f.exists())
				FileUtils.delete(f);
			else if (throwIfNotExists)
				throw new VideoControllerException("File to delete does not exist: " + fileName);
		} catch (IOException e) {
			throw new VideoControllerException(e);
		}
	}

	public static void fileCopy(String fileFrom, String fileTo) {
		try {
			FileUtils.copyFile(new File(fileFrom), new File(fileTo));
		} catch (IOException e) {
			throw new VideoControllerException("Can't copy " + fileFrom + " to " + fileTo);
		}
	}

	public static boolean fileExists(String fileName) {
		File f = new File(fileName);
		return f.exists();
	}

	public static boolean isAbsolute(String fileName) {
		return fileName.startsWith("/")
				|| fileName.length() >= 3 && fileName.charAt(1) == ':' && fileName.charAt(2) == '/';
	}

}
