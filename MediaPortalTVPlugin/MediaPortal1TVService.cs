using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Helpers;
using MediaBrowser.Plugins.MediaPortal.Interfaces;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;
using MediaBrowser.Plugins.MediaPortal.Services.Proxies;

namespace MediaBrowser.Plugins.MediaPortal
{
    /// <summary>
    /// Provides MP (v1) integration for MB3
    /// </summary>
    public class MediaPortal1TvService : ILiveTvService
    {
        private readonly INetworkManager _networkManager;
        private readonly IUserManager _userManager;
        private readonly IPluginLogger _logger;
        private readonly TvServiceProxy _tasProxy;
        private readonly StreamingServiceProxy _wssProxy;
        private readonly StreamingInfoServiceProxy _wssInfoProxy;
        private static StreamingDetails _currentStreamDetails;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPortal1TvService" /> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="jsonSerialiser">The json serialiser.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="networkManager">The network manager.</param>
        /// <param name="userManager">The user manager.</param>
        public MediaPortal1TvService(IHttpClient httpClient, IJsonSerializer jsonSerialiser, ILogger logger,
            INetworkManager networkManager, IUserManager userManager)
        {
            _networkManager = networkManager;
            _userManager = userManager;
            _logger = new PluginLogger(logger);
            _wssProxy = new StreamingServiceProxy(httpClient, jsonSerialiser);
            _wssInfoProxy = new StreamingInfoServiceProxy(httpClient, jsonSerialiser, _networkManager);
            _tasProxy = new TvServiceProxy(httpClient, jsonSerialiser, _wssProxy);
        }

        public string HomePageUrl
        {
            get { return "http://www.team-mediaportal.com/"; }
        }

        public string Name
        {
            get { return "Media Portal V1 Live TV Service"; }
        }

        public Task CancelSeriesTimerAsync(string timerId, CancellationToken cancellationToken)
        {
            _tasProxy.DeleteSchedule(cancellationToken, timerId);
            return Task.Delay(0, cancellationToken);
        }

        public Task CancelTimerAsync(string timerId, CancellationToken cancellationToken)
        {
            _tasProxy.DeleteSchedule(cancellationToken, timerId);
            return Task.Delay(0, cancellationToken);
        }

        public Task CreateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            _tasProxy.CreateSeriesSchedule(cancellationToken, info);
            return Task.Delay(0, cancellationToken);
        }

        public Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            _tasProxy.CreateSchedule(cancellationToken, info);
            return Task.Delay(0, cancellationToken);
        }

        public Task DeleteRecordingAsync(string recordingId, CancellationToken cancellationToken)
        {
            _tasProxy.DeleteRecording(cancellationToken, recordingId);
            return Task.Delay(0, cancellationToken);
        }

        public Task<StreamResponseInfo> GetChannelImageAsync(string channelId, CancellationToken cancellationToken)
        {
            // This is not required as the channel image is set in the GetChannels method
            throw new NotSupportedException();
        }

        public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
        {
            return _tasProxy.GetChannels(cancellationToken);
        }

        public Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(CancellationToken cancellationToken, ProgramInfo program = null)
        {
            var configuration = Plugin.Instance.Configuration;
            
            return Task.FromResult(new SeriesTimerInfo()
            {
                IsPostPaddingRequired = configuration.PostRecordPaddingInSecs > 0,
                IsPrePaddingRequired = configuration.PreRecordPaddingInSecs > 0,
                PostPaddingSeconds = configuration.PostRecordPaddingInSecs,
                PrePaddingSeconds = configuration.PreRecordPaddingInSecs,
            });
        }

        public Task<StreamResponseInfo> GetProgramImageAsync(string programId, string channelId, CancellationToken cancellationToken)
        {
            return Task.FromResult<StreamResponseInfo>(null);
        }

        public Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken)
        {
            return Task.FromResult(_tasProxy.GetPrograms(channelId, startDateUtc, endDateUtc, cancellationToken));
        }

        public Task<StreamResponseInfo> GetRecordingImageAsync(string recordingId, CancellationToken cancellationToken)
        {
            return Task.FromResult<StreamResponseInfo>(null);
        }

        public Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_tasProxy.GetRecordings(cancellationToken));
        }

        public Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_tasProxy.GetSeriesSchedules(cancellationToken));
        }

        public Task<LiveTvServiceStatusInfo> GetStatusInfoAsync(CancellationToken cancellationToken)
        {
            LiveTvServiceStatusInfo result;

            var configurationValidationResult = Plugin.Instance.Configuration.Validate();

            // Validate configuration first
            if (!configurationValidationResult.IsValid)
            {
                result = new LiveTvServiceStatusInfo()
                {
                    HasUpdateAvailable = false,
                    Status = Model.LiveTv.LiveTvServiceStatus.Unavailable,
                    StatusMessage = configurationValidationResult.Summary,
                    Tuners = new List<LiveTvTunerInfo>(),
                    Version = "1.0"
                };                    
            }
            else
            {
                try
                {
                    // Connect to TAS
                    var response = _tasProxy.GetStatusInfo(cancellationToken);

                    result = new LiveTvServiceStatusInfo()
                    {
                        HasUpdateAvailable = false,
                        Status = Model.LiveTv.LiveTvServiceStatus.Ok,
                        StatusMessage = String.Format("MPExtended Service Version: {0} - API Version : {1}", response.ServiceVersion, response.ApiVersion),
                        Tuners = new List<LiveTvTunerInfo>(),
                        Version = "1.0"
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Exception occured getting the MP service status");

                    result = new LiveTvServiceStatusInfo()
                    {
                        HasUpdateAvailable = false,
                        Status = Model.LiveTv.LiveTvServiceStatus.Unavailable,
                        StatusMessage = "Unable to establish a connection with MediaPortal API - check your settings",
                        Tuners = new List<LiveTvTunerInfo>(),
                        Version = "1.0"
                    };
                }
            }

            return Task.FromResult(result);
        }

        public Task<IEnumerable<TimerInfo>> GetTimersAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_tasProxy.GetSchedules(cancellationToken));
        }

        public Task<ChannelMediaInfo> GetRecordingStream(string recordingId, CancellationToken cancellationToken)
        {
            // Cancel the existing stream if present
            if (_currentStreamDetails != null)
            {
                _wssInfoProxy.CancelStream(cancellationToken, _currentStreamDetails.StreamIdentifier);
            }

            // Start a new one and store it away
            _currentStreamDetails = _wssInfoProxy.GetRecordingStream(cancellationToken, recordingId, TimeSpan.Zero);
            return Task.FromResult(_currentStreamDetails.StreamInfo);
        }
        
        public Task ResetTuner(string id, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task RecordLiveStream(string id, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task CloseLiveStream(string id, CancellationToken cancellationToken)
        {
            if (_currentStreamDetails.StreamInfo.Id == id)
            {
                _wssInfoProxy.CancelStream(cancellationToken, _currentStreamDetails.StreamIdentifier);
                return Task.Delay(0);
            }
            
            throw new Exception(String.Format("Unknown stream id requested for close: {0}", id));
        }

        public Task<ChannelMediaInfo> GetChannelStream(string channelId, CancellationToken cancellationToken)
        {
            // Cancel the existing stream if present
            if (_currentStreamDetails != null)
            {
                _wssInfoProxy.CancelStream(cancellationToken, _currentStreamDetails.StreamIdentifier);
            }

            // Start a new one and store it away
            _currentStreamDetails = _wssInfoProxy.GetRecordingStream(cancellationToken, channelId, TimeSpan.Zero);
            return Task.FromResult(_currentStreamDetails.StreamInfo);
        }

        public Task UpdateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            _tasProxy.DeleteSchedule(cancellationToken, info.Id);
            return CreateSeriesTimerAsync(info, cancellationToken);
        }

        public Task UpdateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            _tasProxy.DeleteSchedule(cancellationToken, info.Id);
            return CreateTimerAsync(info, cancellationToken);
        }

        public event EventHandler<RecordingStatusChangedEventArgs> RecordingStatusChanged;

        public event EventHandler DataSourceChanged;
    }
}