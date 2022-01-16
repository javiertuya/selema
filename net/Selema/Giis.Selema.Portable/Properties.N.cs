using System.IO;
using System;

namespace Java.Util
{
    /// <summary>
    /// Simple implementation of Properties class for the net platform
    /// </summary>
	public class Properties
	{
		private string propFile = string.Empty;
        private string[] lines = new string[0];

        public void Load(string propFileName)
        {
			this.propFile = propFileName;
            try
            {
                this.lines = File.ReadAllLines(propFile);
            }
            catch (FileNotFoundException e)
            {
                throw new ApplicationException("Can't load properties file " + propFile, e);
            }
        }

        /**
         * Lee el valor de una propiedad de un fichero .properties (estilo Java).
         */
        public string GetProperty(string propName)
        {
            foreach (string line in this.lines)
            {
                if (line.Trim() == "" || line.Trim().Substring(0, 1) == "#") //ignora comentarios
                    continue;
                //Parte en nombre de propiedad y valor. Asume que no hay ningun caracter = en el valor
                System.Text.RegularExpressions.Regex rg = new System.Text.RegularExpressions.Regex("=");
                string[] comp = rg.Split(line.Trim(), 2);
                if (comp.Length != 2)
                    throw new ApplicationException("Invalid property specification: " + line);
                //busco si existe la propiedad (case sensitive)
                if (comp[0].Trim().Equals(propName))
                    return comp[1].Trim();
            }
            return null;
        }
        /**
         * Lee una propiedad, si o esxite establece un valor por defecto
         */
         public string GetProperty(string propName, string defaultValue)
        {
            string prop = GetProperty(propName);
            return prop == null ? defaultValue : prop;
        }
    }
}
