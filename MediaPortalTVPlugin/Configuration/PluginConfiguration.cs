using System;
using MediaBrowser.Model.Plugins;

namespace MediaBrowser.Plugins.MediaPortal.Configuration
{
    /// <summary>
    /// Class PluginConfiguration
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginConfiguration" /> class.
        /// </summary>
        public PluginConfiguration()
        {
            ApiPortNumber = 4322;
            LiveStreamingProfileName = "Direct";
        }

        public string ApiHostName { get; set; }
        public Int32 ApiPortNumber { get; set; }
        public bool RequiresAuthentication { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Int32 PreRecordPaddingInSecs { get; set; }
        public Int32 PostRecordPaddingInSecs { get; set; }
        public String LiveStreamingProfileName { get; set; }

        public ValidationResult Validate()
        {
            if (String.IsNullOrEmpty(ApiHostName))
            {
                return new ValidationResult(false, "Please specify an API HostName (the box MPExtended is installed on)");
            }

            if (ApiPortNumber < 1)
            {
                return new ValidationResult(false, "Please specify an API Port Number (usually 4322)");
            }

            if (RequiresAuthentication)
            {
                if (String.IsNullOrEmpty(UserName))
                {
                    return new ValidationResult(false, "Please specify a UserName (check MPExtended - Authentication");
                }

                if (String.IsNullOrEmpty(Password))
                {
                    return new ValidationResult(false, "Please specify an Password (check MPExtended - Authentication");
                }
            }

            return new ValidationResult(true, String.Empty);
        }
    }

    public class ValidationResult
    {
        public ValidationResult(Boolean isValid, String summary)
        {
            IsValid = isValid;
            Summary = summary;
        }

        public Boolean IsValid { get; set; }
        public String Summary { get; set; }
    }
}
