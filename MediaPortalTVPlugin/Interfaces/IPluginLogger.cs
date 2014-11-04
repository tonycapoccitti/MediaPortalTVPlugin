using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortalTVPlugin.Interfaces
{
    /// <summary>
    /// Provides plugin specific implementations for logging
    /// </summary>
    public interface IPluginLogger
    {
        /// <summary>
        /// Logs an exception, with a message and detail
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        void LogError(Exception ex, String message, params Object[] args);
    }
}
