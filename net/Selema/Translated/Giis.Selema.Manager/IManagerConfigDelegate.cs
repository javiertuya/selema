using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    /// <summary>
    /// A delegate used to provide additional configurations to the SeleManager
    /// </summary>
    public interface IManagerConfigDelegate
    {
        void Configure(SeleManager sm);
    }
}