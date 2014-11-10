using System;
using MediaBrowser.Model.Logging;

namespace MediaBrowser.Plugins.MediaPortal.Interfaces
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
