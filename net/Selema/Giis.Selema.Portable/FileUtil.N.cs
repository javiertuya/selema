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
        public static string FileRead(string fileName)
        {
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
            File.WriteAllText(fileName, contents);
        }
        public static void FileAppend(string fileName, string line)
        {
            File.AppendAllText(fileName, line);
        }
        public static void CopyFile(string source, string dest)
        {
            throw (new NotImplementedException());
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
            return Path.GetFullPath(path);
        }
        /** 
         * Crea la carpeta indicada como parametro (la carpeta puede existir, o si no existe, debe existir el padre)
         */
        public static void CreateDirectory(string filePath)
        {
            Directory.CreateDirectory(filePath);
        }

    }
}
