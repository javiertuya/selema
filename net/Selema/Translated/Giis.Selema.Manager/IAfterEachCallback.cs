using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Manager
{
    /// <summary>
    /// Only for testing: a callback to check the log results that is fired after all tear down actions
    /// </summary>
    public interface IAfterEachCallback
    {
        void RunAfterCallback(string testName, bool success);
    }
}