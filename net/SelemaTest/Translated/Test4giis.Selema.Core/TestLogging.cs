using NUnit.Framework;
using NLog;
using Giis.Selema.Manager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Test4giis.Selema.Core
{
    /// <summary>
    /// Otras pruebas unitarias necesarias, cada uno con su propio selenium manager,
    /// no se sigue el ciclo de vida ni se instancia ningun driver
    /// </summary>
    public class TestLogging
    {
        [Test]
        public virtual void TestLoggersAreIndependent()
        {

            //dos loggers de sm y uno adicional, no se mezclan los mensajes
            //usa la carpeta de log por defecto, por lo que no apareceran bajo selema como el resto de tests.
            SeleManager sm0 = new SeleManager().SetManagerDelegate(new Config4test());
            LogReader lr0 = new LogReader(new SelemaConfig().GetReportDir());
            sm0.GetLogger().Info("test log1 a principal");
            lr0.AssertBegin();
            lr0.AssertContains("test log1 a principal");
            SeleManager sm1 = new SeleManager(new SelemaConfig().SetName("independent")).SetManagerDelegate(new Config4test());
            LogReader lr1 = new LogReader(new SelemaConfig().GetReportDir(), "selema-log-independent.html");
            sm1.GetLogger().Info("test log1 a secundario");
            lr1.AssertBegin();
            lr1.AssertContains("test log1 a secundario");
            lr0.AssertBegin();
            lr0.AssertEnd(); //no se ha escrito nada aqui

            //compruebo que se puede volver a escribir en el primero
            sm0.GetLogger().Info("test log2 a principal");
            lr0.AssertBegin();
            lr0.AssertContains("test log2 a principal");
            lr1.AssertBegin();
            lr1.AssertEnd(); //no se ha escrito nada aqui

            //otro logger no de sm
            Logger lg = LogManager.GetLogger("test4giis.selema.core");
            lg.Info("test log1 a otro logger");
            lr0.AssertBegin();
            lr0.AssertEnd();
            lr1.AssertBegin();
            lr1.AssertEnd();
        }
    }
}