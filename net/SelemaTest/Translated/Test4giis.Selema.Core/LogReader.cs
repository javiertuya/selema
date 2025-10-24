using Giis.Portable.Util;
using Giis.Selema.Manager;
using Giis.Selema.Services.Impl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    /// <summary>
    /// Utilidad para examinar el log del test y realizar asserts del contenido.
    /// </summary>
    public class LogReader
    {
        private string logFile;
        private IList<string> logLines; //log en forma de lista de lineas
        private List<String[]> assertItems;
        //log por defecto
        public LogReader(string reportDir) : this(reportDir, "selema-log.html")
        {
        }

        //log especificando un fichero concreto
        public LogReader(string reportDir, string logFile)
        {
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
                    return new List<string>();
                throw;
            }
        }

        public virtual int GetLogSize()
        {
            return ReadLogFile().Count;
        }

        public virtual void AssertBegin()
        {
            logLines = ReadLogFile();
            assertItems = new List<String[]>();
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
                throw new SelemaException("Log file has less lines than expected");
            StringBuilder sb = new StringBuilder();

            //compares all assertItems at the offset from the end of logLines
            for (int i = 0; i < assertItems.Count; i++)
            {
                int offset = logLines.Count - assertItems.Count - offsetFromLast;
                string actual = SelemaLogger.ReplaceTags(logLines[offset + i]);
                for (int j = 0; j < assertItems[i].Length; j++)
                {

                    //each of the items that must be included in the current log line
                    string expected = assertItems[i][j];
                    if (!actual.ToLower().Contains(expected.ToLower()))
                        sb.Append(sb.Length > 0 ? "\n" : "").Append(GetAssertMessage(offsetFromLast, offset + i, j, expected, actual));
                }
            }

            if (sb.Length > 0)
                throw new SelemaException("LogReader has differences:\n" + sb.ToString());
        }

        private string GetAssertMessage(int offsetFromLast, int logLine, int assertItemLine, string expected, string actual)
        {
            return "Log line " + logLine + " (row " + offsetFromLast + " before end), expected item " + assertItemLine + "\n  Expected '" + expected + "'\n  Not contained in: '" + actual + "'";
        }
    }
}