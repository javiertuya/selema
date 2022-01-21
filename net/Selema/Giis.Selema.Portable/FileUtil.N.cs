using System;
using System.Collections.Generic;
using System.IO;

namespace Giis.Selema.Portable
{
    /// <summary>
    /// File management for compatibility Java/C#
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// Throws exception if invalid characters: Although linux is more permisive than windows in characters allowed to filenames,
        /// when running in GitHub Actions, some characters are never allowed in actions such as publish artifacts
        /// </summary>
        /// <param name="name"></param>
        private static void CheckFileName(string name)
        {
            string invalid = "\"<>|*?\r\n";
            foreach (char c in invalid.ToCharArray())
            {
                if (name.IndexOf(c) != -1)
                    throw new Exception("File name contains an invalid character: \" : < > | * ? \\r \\n");
            }
        }
        public static string FileRead(string fileName)
        {
            CheckFileName(fileName);
            return File.ReadAllText(fileName);
        }
        public static List<string> FileReadLines(string fileName)
        {
            try
            {
                string[] linesArray = File.ReadAllLines(fileName);
                return new List<string>(linesArray);
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public static void FileWrite(string fileName, string contents)
        {
            CheckFileName(fileName);
            File.WriteAllText(fileName, contents);
        }
        public static void FileAppend(string fileName, string line)
        {
            CheckFileName(fileName);
            File.AppendAllText(fileName, line);
        }

        //devuelve un array con todos los ficheros de un folder que hacen match con una especificacion de fichero que contiene *
        public static string[] ListFilesMatchingWildcard(string folder, string fileNameWildcard)
        {
            return Directory.GetFiles(folder, fileNameWildcard, SearchOption.TopDirectoryOnly);
        }
        public static string GetPath(params string[] path)
        {
            return Path.Combine(path);
        }
        public static string GetFullPath(string path)
        {
            CheckFileName(path);
            return Path.GetFullPath(path);
        }
        /** 
         * Crea la carpeta indicada como parametro (la carpeta puede existir, o si no existe, debe existir el padre)
         */
        public static void CreateDirectory(string filePath)
        {
            CheckFileName(filePath);
            Directory.CreateDirectory(filePath);
        }

    }
}
