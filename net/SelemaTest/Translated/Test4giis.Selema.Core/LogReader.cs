/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Text;
using Giis.Portable.Util;
using Giis.Selema.Manager;
using Giis.Selema.Services.Impl;
using Sharpen;

namespace Test4giis.Selema.Core
{
	/// <summary>Utilidad para examinar el log del test y realizar asserts del contenido.</summary>
	public class LogReader
	{
		private string logFile;

		private IList<string> logLines;

		private IList<string[]> assertItems;

		public LogReader(string reportDir)
			: this(reportDir, "selema-log.html")
		{
		}

		public LogReader(string reportDir, string logFile)
		{
			//log en forma de lista de lineas
			//log por defecto
			//log especificando un fichero concreto
			this.logFile = FileUtil.GetPath(reportDir, logFile);
		}

		private IList<string> ReadLogFile()
		{
			try
			{
				return FileUtil.FileReadLines(logFile);
			}
			catch (PortableException e)
			{
				//assume a non existing log file as empty (regression #425)
				if (e.Message.StartsWith("Error reading file"))
				{
					return new List<string>();
				}
				throw (e);
			}
		}

		public virtual int GetLogSize()
		{
			return ReadLogFile().Count;
		}

		public virtual void AssertBegin()
		{
			logLines = ReadLogFile();
			assertItems = new List<string[]>();
		}

		public virtual void AssertContains(params string[] expected)
		{
			assertItems.Add(expected);
		}

		public virtual void AssertEnd()
		{
			AssertEnd(0);
		}

		public virtual void AssertEnd(int offsetFromLast)
		{
			if (assertItems.Count > logLines.Count)
			{
				throw new SelemaException("Log file has less lines than expected");
			}
			StringBuilder sb = new StringBuilder();
			//compares all assertItems at the end of logLines
			for (int i = 0; i < assertItems.Count; i++)
			{
				int offset = logLines.Count - assertItems.Count - offsetFromLast;
				string actual = SelemaLogger.ReplaceTags(logLines[offset + i]);
				for (int j = 0; j < assertItems[i].Length; j++)
				{
					//each of the items that must be included in the current log line
					string expected = assertItems[i][j];
					if (!actual.ToLower().Contains(expected.ToLower()))
					{
						sb.Append(GetAssertMessage(offset + i, i, expected, actual));
					}
				}
			}
			if (sb.Length > 0)
			{
				throw new SelemaException("LogReader has differences:\n" + sb.ToString());
			}
		}

		private string GetAssertMessage(int logLine, int assertItemLine, string expected, string actual)
		{
			return "Log line " + logLine + " item " + assertItemLine + "\n  Expected '" + expected + "'\n  Not contained in: '" + actual + "'";
		}

		//para compatibilidad con el orden de los argumentos entre pataformas
		public virtual void AssertIsTrue(bool value, string message)
		{
			if (!value)
			{
				NUnit.Framework.Legacy.ClassicAssert.Fail(message);
			}
		}
	}
}
