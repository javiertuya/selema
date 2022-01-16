using System;

namespace Giis.Selema.Portable
{
    /// <summary>
    /// Platform independent instantiation of Properties object.
    /// </summary>
    public class PropertiesFactory
	{

        public virtual Java.Util.Properties GetPropertiesFromFilename(string fileName)
		{
            try
            {
                Java.Util.Properties prop = new Java.Util.Properties();
                prop.Load(fileName);
                return prop;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
