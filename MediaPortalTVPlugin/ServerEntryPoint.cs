using System.Threading.Tasks;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.ScheduledTasks;
using MediaBrowser.Common.Security;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Plugins.MediaPortal.Configuration;
using MediaBrowser.Plugins.MediaPortal.Helpers;

namespace MediaBrowser.Plugins.MediaPortal
{
    /// <summary>
    /// Class ServerEntryPoint
    /// </summary>
    public class ServerEntryPoint : IServerEntryPoint, IRequiresRegistration
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ServerEntryPoint Instance { get; private set; }

        /// <summary>
        /// The _task manager
        /// </summary>
        private readonly ITaskManager _taskManager;

        /// <summary>
        /// Access to the LibraryManager of MB Server
        /// </summary>
        public ILibraryManager LibraryManager { get; private set; }

        /// <summary>
        /// Access to the SecurityManager of MB Server
        /// </summary>
        public ISecurityManager PluginSecurityManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerEntryPoint" /> class.
        /// </summary>
        /// <param name="taskManager">The task manager.</param>
        /// <param name="libraryManager">The library manager.</param>
        /// <param name="appPaths">The app paths.</param>
        /// <param name="logManager">The log manager.</param>
        /// <param name="securityManager">The security manager.</param>
        public ServerEntryPoint(ITaskManager taskManager, ILibraryManager libraryManager, 
            IApplicationPaths appPaths, ILogManager logManager, ISecurityManager securityManager)
        {
            _taskManager = taskManager;

            LibraryManager = libraryManager;
            PluginSecurityManager = securityManager;

            // Inject our bespoke logger to get tailored messages
            Plugin.Logger = new PluginLogger(logManager.GetLogger(Plugin.Instance.Name));

            Instance = this;
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
        }

        /// <summary>
        /// Called when [configuration updated].
        /// </summary>
        /// <param name="oldConfig">The old config.</param>
        /// <param name="newConfig">The new config.</param>
        public void OnConfigurationUpdated(PluginConfiguration oldConfig, PluginConfiguration newConfig)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Loads our registration information
        ///
        /// </summary>
        /// <returns></returns>
        public async Task LoadRegistrationInfoAsync()
        {
            //Plugin.Instance.Registration = await PluginSecurityManager.GetRegistrationStatus("MediaBrowser.Plugins.MediaPortal", "[**MB2CompatibleFeature**]").ConfigureAwait(false);
        }
    }
}
