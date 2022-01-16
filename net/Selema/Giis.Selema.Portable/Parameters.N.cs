using System;
using System.Net;

namespace Giis.Selema.Portable
{
    /// <summary>
    /// Platform specific constants
    /// </summary>
    public class Parameters
    {
        public const string PlatformName = "net";
        public static string DefaultProjectRoot = "."; //se establece en un metodo estatico
        public const string DefaultReportSubdir = "reports";
        public const string DefaultJenkinsProjectRoot = ".";

       //Cuando la configuracion de la solucion incluye los proyectos dentro de carpetas (p.e. src, tests)
        //esta variable debe ser true
        private static readonly bool ProjectsUnderFolder = false;

        static Parameters()
        {
            //No hay forma estandar de determinar si se esta ejecutando en entorno hosted (web) o no (consola o test)
            //por lo que primero examina una variable de entorno que si esta configurada determina la ruta relativa a la raiz
            //(usado cuando se ejecuta en containers)
            string envRoot = System.Environment.GetEnvironmentVariable("HOSTED_APP_ROOT");
            if (!string.IsNullOrEmpty(envRoot))
            {
                DefaultProjectRoot = envRoot;
                return;
            }
            //Establece la raiz del proyecto
            DefaultProjectRoot = FileUtil.GetPath("..","..","..",".."); //cuatro niveles arriba (el proyecto se ejecuta en proyecto/bin/debug/netcoreappX.X
            //Cuando la solucion contiene carpetas que albergan los proyectos sera otro nivel mas
            if (ProjectsUnderFolder)
                DefaultProjectRoot = FileUtil.GetPath("..", "..", "..", "..", "..");
        }

         /// <summary>
        /// Direccion ip V4 del equipo donde se ejecuta este programa. Si hay varias devuelve la primera
        /// </summary>
        public static string GetIpV4Address()
        {
            string strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            for (int i = 0; i < addr.Length; i++)
            {
                if (addr[i].AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    return addr[i].ToString();
            }
            throw new SelemaException("GetIpV4Address: No IP V4 address found");
        }
        /// <summary>
        /// Determina si se esta ejecutando en un entorno jenkins por medio de la variable de entorno JOB_NAME
        /// </summary>
        public static bool IsJenkinsJob()
        {
            return !String.IsNullOrEmpty(Environment.GetEnvironmentVariable("JOB_NAME"));
        }

    }
}
