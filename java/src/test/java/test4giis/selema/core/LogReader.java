package test4giis.selema.core;

import static org.junit.Assert.fail;

import java.util.ArrayList;
import java.util.List;

import giis.selema.portable.SelemaException;
import giis.selema.services.impl.SelemaLogger;
import giis.selema.portable.FileUtil;

/**
 * Utilidad para examinar el log del test y realizar asserts del contenido.
 */
public class LogReader {
	private String logFile;
	private List<String> logLines; //log en forma de lista de lineas
	private List<String[]> assertItems;
	
	//log por defecto
	public LogReader(String reportDir) {
		this(reportDir, "selema-log.html");
	}
	//log especificando un fichero concreto
	public LogReader(String reportDir, String logFile) {
		this.logFile=FileUtil.getPath(reportDir, logFile);
	}
	public int getLogSize() {
		return FileUtil.fileReadLines(logFile).size();
	}
	public void assertBegin() {
		logLines=FileUtil.fileReadLines(logFile);
		assertItems=new ArrayList<String[]>();
	}
	public void assertContains(String expected) {
		assertItems.add(new String[] {expected, null} );
	}
	public void assertContains(String expected1, String expected2) {
		assertItems.add(new String[] {expected1, expected2} );
	}
	public void assertEnd() {
		if (assertItems.size()>logLines.size())
			throw new SelemaException("Log file has less lines than expected");
		StringBuilder sb=new StringBuilder();
		//compares all assertItems at the end of logLines
		for (int i=0; i<assertItems.size(); i++) {
			int offset=logLines.size()-assertItems.size();
			String actual=SelemaLogger.replaceTags(logLines.get(offset+i));
			String expected=assertItems.get(i)[0];
			if (!actual.toLowerCase().contains(expected.toLowerCase()))
				sb.append(getAssertMessage(offset+i, i, expected, actual));
			expected=assertItems.get(i)[1];
			if (expected!=null && !actual.toLowerCase().contains(expected.toLowerCase()))
				sb.append(getAssertMessage(offset+i, i, expected, actual));
		}
		if (sb.length()>0)
			throw new SelemaException("LogReader has differences:\n" + sb.toString());
	}
	private String getAssertMessage(int logLine, int assertItemLine, String expected, String actual) {
		return "Log line " + logLine + " item " + assertItemLine
				+ "\n  Expected '"+expected
				+ "'\n  Not contained in: '"+actual+"'";
	}
	
	//para compatibilidad con el orden de los argumentos entre pataformas
	public void assertIsTrue(boolean value, String message) {
		if (!value)
			fail(message);		
	}

}

