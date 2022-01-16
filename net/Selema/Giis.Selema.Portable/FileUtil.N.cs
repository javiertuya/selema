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
        public static string FileRead(string fileName, bool throwIfNotExists)
        {
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);
            if (throwIfNotExists)
                throw new SelemaException("File does not exist " + fileName);
            else
                return null;
        }
        public static string FileRead(string fileName)
        {
            return FileRead(fileName, true);
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

        public static IList<string> GetFileListInDirectory(String path)
        {
            //En java se tiene solo nombre, pero .net devuelve path y nombre
            IList<string> fileNames = new List<String>();
            IList<string> filesPath = Directory.GetFiles(path);
            foreach (String filePath in filesPath)
                fileNames.Add(GetFileNameOnly(filePath));
            return fileNames;
        }
        private static string GetFileNameOnly(string fileWithPath)
        {
            //puee haber mezcla de separadores / \, busca el ultimo de ellos
            int first = -1; //si no se encuentram obtendra desde fist+1, es decir, desde cero
            for (int i = 0; i < fileWithPath.Length; i++)
                if (fileWithPath[i] == '/' || fileWithPath[i] == '\\')
                    first = i;
            return fileWithPath.Substring(first + 1, fileWithPath.Length - first - 1);
        }
        public static void DeleteFilesInDirectory(string path)
        {
            //la lista de ficheros viene sin path por compatibilidad con java, por lo que al borrar debe anyadir el path
            IList<string> files = GetFileListInDirectory(path);
            foreach (string fileName in files)
                File.Delete(Path.Combine(path, fileName));
        }

        /**
          * Obtiene una propiedad de un fichero properties (estilo Java).
          * Si se especifica valor por defecto, usa este cuando no encuentra la propiedad.
          * Si no se especifica (es null) causa excepcion
          */
        public static string GetProperty(string propFileName, string propName, string defaultValue)
        {
            string[] lines;
            try
            {
                lines = File.ReadAllLines(propFileName);
            }
            catch (FileNotFoundException e)
            {
                throw new SelemaException("Can't load properties file " + propFileName, e);
            }
            //busca la propiedad en las lineas del fichero
            foreach (string line in lines)
            {
                if (line.Trim() == "" || line.Trim().Substring(0, 1) == "#") //ignora comentarios
                    continue;
                //Parte en nombre de propiedad y valor. Asume que no hay ningun caracter = en el valor
                System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex("=");
                string[] comp = rg.Split(line.Trim(), 2);
                if (comp.Length != 2)
                    throw new SelemaException("getProperty: Invalid property specification:" + line);
                //busco si existe la propiedad (case sensitive)
                if (comp[0].Trim().Equals(propName))
                    return comp[1].Trim();
            }
            //si no lo ha encontrado, excepcion si no hay valor por defecto
            if (defaultValue == null)
                throw new SelemaException("Can't read property " + propName);
            else
                return defaultValue.Trim();
        }
        public static string GetProperty(string propFileName, string propName)
        {
            return GetProperty(propFileName, propName, null);
        }
    }
}
