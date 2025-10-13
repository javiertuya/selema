package giis.selema.portable.selenium;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.StringWriter;

import org.apache.commons.io.FileUtils;

import giis.portable.util.PortableException;

public class CommandLine {

	/**
	 * Runs a shell/cmd command and returns a string with the standard output
	 */
	public static String runCommand(String command) {
		String output;
		Process proc;
		try {
			proc = Runtime.getRuntime().exec(command);
		} catch (IOException e) {
			throw new PortableException("Can't execute command: " + command);
		}

		InputStreamReader isr = new InputStreamReader(proc.getInputStream());
		BufferedReader br = new BufferedReader(isr);
		StringWriter sw = new StringWriter();
		try {
			while ((output = br.readLine()) != null)
				sw.append(output + "\n");
		} catch (IOException e) {
			throw new PortableException("Can't get standard output from command: " + command);
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
				throw new PortableException("File to delete does not exist: " + fileName);
		} catch (IOException e) {
			throw new PortableException(e);
		}
	}

	public static void fileCopy(String fileFrom, String fileTo) {
		try {
			FileUtils.copyFile(new File(fileFrom), new File(fileTo));
		} catch (IOException e) {
			throw new PortableException("Can't copy " + fileFrom + " to " + fileTo);
		}
	}

}
