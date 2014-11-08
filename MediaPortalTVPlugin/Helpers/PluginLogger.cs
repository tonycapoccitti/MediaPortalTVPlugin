using System;
using System.Text;
using MediaBrowser.Model.Logging;
using MediaPortalTVPlugin.Interfaces;

namespace MediaPortalTVPlugin.Helpers
{
    /// <summary>
    /// Wrapper class for the MB logging manager
    /// </summary>
    public class PluginLogger : IPluginLogger
    {
        readonly ILogger _logger;

        public PluginLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Debug(string message, params object[] paramList)
        {
            _logger.Debug("[MP TV Plugin] - {0}", String.Format(message, paramList));
        }

        public void Error(string message, params object[] paramList)
        {
            _logger.Error("[MP TV Plugin] - {0}", String.Format(message, paramList));
        }

        public void ErrorException(string message, Exception exception, params object[] paramList)
        {
            _logger.FatalException("[MP TV Plugin] - {0}", exception, String.Format(message, paramList));
        }

        public void Error(Exception exception, string message, params object[] paramList)
        {
            _logger.FatalException("[MP TV Plugin] - {0}", exception, String.Format(message, paramList));
        }

        public void Fatal(string message, params object[] paramList)
        {
            _logger.Fatal("[MP TV Plugin] - {0}", String.Format(message, paramList));
        }

        public void FatalException(string message, Exception exception, params object[] paramList)
        {
            _logger.FatalException("[MP TV Plugin] - {0}", exception, String.Format(message, paramList));
        }

        public void Fatal(Exception exception, string message, params object[] paramList)
        {
            _logger.FatalException("[MP TV Plugin] - {0}", exception, String.Format(message, paramList));
        }

        public void Info(string message, params object[] paramList)
        {
            _logger.Info("[MP TV Plugin] - {0}", String.Format(message, paramList));
        }

        public void Log(LogSeverity severity, string message, params object[] paramList)
        {
            _logger.Log(severity, "[MP TV Plugin] - {0}", String.Format(message, paramList));
        }

        public void LogMultiline(string message, LogSeverity severity, StringBuilder additionalContent)
        {
            _logger.LogMultiline(String.Format("[MP TV Plugin] - {0}", message), severity, additionalContent);
        }

        public void Warn(string message, params object[] paramList)
        {
            _logger.Warn("[MP TV Plugin] - {0}", String.Format(message, paramList));
        }
    }
}
