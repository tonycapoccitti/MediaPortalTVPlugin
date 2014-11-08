using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBrowser.Model.Logging;

namespace MediaPortalTVPlugin.Interfaces
{
    /// <summary>
    /// Provides logging methods for a plugin
    /// </summary>
    public interface IPluginLogger : ILogger
    {
        void Error(Exception exception, string message, params object[] paramList);
        void Fatal(Exception exception, string message, params object[] paramList);
    }
}
