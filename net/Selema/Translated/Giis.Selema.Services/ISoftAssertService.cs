using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Selema.Services
{
    /// <summary>
    /// Management of SoftVisualAssert comparisons between long strings, managing the html diff files and links from the log
    /// </summary>
    public interface ISoftAssertService
    {
        /// <summary>
        /// Configures this service, called on attaching the service to a SeleManager
        /// </summary>
        ISoftAssertService Configure(ISelemaLogger logger, bool local, string projectRoot, string reportSubdir);
        /// <summary>
        /// General soft assertion, intended to be used from a proxy in the SeleManager class
        /// </summary>
        void AssertEquals(string expected, string actual, string message, IMediaContext context, string testName);
        /// <summary>
        /// Throws and exception if at least one assertion failed including all assertion messages
        /// </summary>
        void AssertAll();
        /// <summary>
        /// Resets the current failure messages that are stored
        /// </summary>
        void AssertClear();
    }
}