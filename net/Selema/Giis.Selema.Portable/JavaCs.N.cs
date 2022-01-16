using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Giis.Selema.Portable
{
    /// <summary>
    /// Miscelaneous utilities for compatibility Java/C#
    /// </summary>
    public static class JavaCs
	{
		public static bool EqualsIgnoreCase(string thisString, string anotherString)
		{
            return thisString.Equals(anotherString, System.StringComparison.CurrentCultureIgnoreCase);
        }

        public static string Substring(string fromString, int beginIndex)
		{
            return fromString.Substring(beginIndex);
        }

        public static string Substring(string fromString, int beginIndex, int endIndex)
		{
            return fromString.Substring(beginIndex, endIndex-beginIndex);
        }

        public static string IntToString(int value)
        {
            return value.ToString();
        }

        public static string Join(string separator, string[] values)
        {
            return String.Join(separator, values);
        }
        public static string[] ToArray(List<string> lst)
		{
            return lst.ToArray();
        }
        public static string DeepToString(string[] strArray)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
                sb.Append((i == 0 ? "" : ", ") + strArray[i]);
            return "[" + sb.ToString() + "]";
        }

        public static void PutAll(IDictionary<string, object> targetMap, IDictionary<string, object> mapToAdd)
        {
            foreach (string key in mapToAdd.Keys)
                targetMap.Add(key, mapToAdd[key]);
        }

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string ReplaceAll(string str, string regex, string replacement)
        {
            return Regex.Replace(str, regex, replacement);
        }
        /**
        * Remplazo usando una expresion regular,
        * necesario porque en java es replaceAll pero en .net se debe usar
        * regex y sharpen lo traduce por un simple replace.
        */
        public static string ReplaceRegex(string str, string regex, string replacement)
        {
            return Regex.Replace(str, regex, replacement);
        }
        public static string[] SplitByDot(string str)
        {
            return str.Split('.');
        }

        public static void AddAll(List<string> thisList, List<string> listToAdd)
		{
            foreach (string item in listToAdd)
                thisList.Add(item);
        }
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }

        public static void LoggerError(Logger logger, string message, Exception e)
        {
            logger.Error(e, message);
        }

        public static DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }
        public static string GetTime(DateTime date)
        {
            return date.ToString("hh:mm:ss");
        }
        public static long CurrentTimeMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }
        public static void Sleep(int millis)
        {
            System.Threading.Thread.Sleep(millis);
        }
        public static string GetUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
