using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Drawing;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;

namespace MediaBrowser.Plugins.MediaPortal
{
    /// <summary>
    /// Provides MP (v1) integration for MB3
    /// </summary>
    public class MediaPortal1TvService : ILiveTvService
    {
        private static StreamingDetails _currentStreamDetails;

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
            Plugin.TvProxy.DeleteSchedule(cancellationToken, timerId);
            return Task.Delay(0, cancellationToken);
        }

        public Task CancelTimerAsync(string timerId, CancellationToken cancellationToken)
        {
            Plugin.TvProxy.DeleteSchedule(cancellationToken, timerId);
            return Task.Delay(0, cancellationToken);
        }

        public Task CreateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            Plugin.TvProxy.CreateSeriesSchedule(cancellationToken, info);
            return Task.Delay(0, cancellationToken);
        }

        public Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            Plugin.TvProxy.CreateSchedule(cancellationToken, info);
            return Task.Delay(0, cancellationToken);
        }

        public Task DeleteRecordingAsync(string recordingId, CancellationToken cancellationToken)
        {
            Plugin.TvProxy.DeleteRecording(cancellationToken, recordingId);
            return Task.Delay(0, cancellationToken);
        }

        public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
        {
            var channels = Plugin.TvProxy.GetChannels(cancellationToken);
            return channels;
        }

        public Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(CancellationToken cancellationToken, ProgramInfo program = null)
        {
            var scheduleDefaults = Plugin.TvProxy.GetScheduleDefaults(cancellationToken);
            return Task.FromResult(new SeriesTimerInfo()
            {
                IsPostPaddingRequired = scheduleDefaults.PostRecordInterval.Ticks > 0,
                IsPrePaddingRequired = scheduleDefaults.PreRecordInterval.Ticks > 0,
                PostPaddingSeconds = (Int32)scheduleDefaults.PostRecordInterval.TotalSeconds,
                PrePaddingSeconds = (Int32)scheduleDefaults.PreRecordInterval.TotalSeconds,
            });
        }

        public Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken)
        {
            return Task.FromResult(Plugin.TvProxy.GetPrograms(channelId, startDateUtc, endDateUtc, cancellationToken));
        }

        public Task<ImageStream> GetProgramImageAsync(string programId, string channelId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Plugin.TvProxy.GetRecordings(cancellationToken));
        }

        public Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Plugin.TvProxy.GetSeriesSchedules(cancellationToken));
        }

        public Task<LiveTvServiceStatusInfo> GetStatusInfoAsync(CancellationToken cancellationToken)
        {
            LiveTvServiceStatusInfo result;

            var configurationValidationResult = Plugin.Instance.Configuration.Validate();
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            // Validate configuration first
            if (!configurationValidationResult.IsValid)
            {
                result = new LiveTvServiceStatusInfo()
                {
                    HasUpdateAvailable = false,
                    Status = Model.LiveTv.LiveTvServiceStatus.Unavailable,
                    StatusMessage = configurationValidationResult.Summary,
                    Tuners = new List<LiveTvTunerInfo>(),
                    Version = version
                };
            }
            else
            {
                try
                {

                    // Test connections to both the streaming and tv proxy
                    var response = Plugin.StreamingProxy.GetStatusInfo(cancellationToken);
                    response = Plugin.TvProxy.GetStatusInfo(cancellationToken);

                    var activeCards = Plugin.TvProxy.GetActiveCards(cancellationToken);
                    var cards = Plugin.TvProxy.GetTunerCards(cancellationToken).Where(c => c.Enabled).Select(c =>
                    {
                        var activeDetails = activeCards.SingleOrDefault(ac => ac.Id == c.Id);
                        var tunerInfo = new LiveTvTunerInfo()
                        {
                            Id = c.Id.ToString(CultureInfo.InvariantCulture),
                            Name = c.Name,
                        };

                        if (activeDetails != null)
                        {
                            tunerInfo.Clients = new List<string>() { activeDetails.User.Name };
                            tunerInfo.Status = 
                                activeDetails.IsRecording ? LiveTvTunerStatus.RecordingTv :
                                activeDetails.IsTunerLocked ? LiveTvTunerStatus.LiveTv : LiveTvTunerStatus.Available;
                        }
                        return tunerInfo;

                    }).ToList();

                    result = new LiveTvServiceStatusInfo()
                    {
                        HasUpdateAvailable = false,
                        Status = LiveTvServiceStatus.Ok,
                        StatusMessage = String.Format("MPExtended Service Version: {0} - API Version : {1}", response.ServiceVersion, response.ApiVersion),
                        Tuners = cards,
                        Version = version
                    };

                }
                catch (Exception ex)
                {
                    Plugin.Logger.Error(ex, "Exception occured getting the MP service status");

                    result = new LiveTvServiceStatusInfo()
                    {
                        HasUpdateAvailable = false,
                        Status = LiveTvServiceStatus.Unavailable,
                        StatusMessage = "Unable to establish a connection with MediaPortal API - check your settings",
                        Tuners = new List<LiveTvTunerInfo>(),
                        Version = version
                    };
                }
            }

            return Task.FromResult(result);
        }

        public Task<IEnumerable<TimerInfo>> GetTimersAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Plugin.TvProxy.GetSchedules(cancellationToken));
        }

        public Task<ChannelMediaInfo> GetRecordingStream(string recordingId, CancellationToken cancellationToken)
        {
            // Cancel the existing stream if present
            if (_currentStreamDetails != null)
            {
                Plugin.StreamingProxy.CancelStream(cancellationToken, _currentStreamDetails.StreamIdentifier);
            }

            // Start a new one and store it away
            _currentStreamDetails = Plugin.StreamingProxy.GetRecordingStream(cancellationToken, recordingId, TimeSpan.Zero);
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
                Plugin.StreamingProxy.CancelStream(cancellationToken, _currentStreamDetails.StreamIdentifier);
                return Task.Delay(0);
            }
            
            throw new Exception(String.Format("Unknown stream id requested for close: {0}", id));
        }

        public Task<ChannelMediaInfo> GetChannelStream(string channelId, CancellationToken cancellationToken)
        {
            // Cancel the existing stream if present
            if (_currentStreamDetails != null)
            {
                Plugin.StreamingProxy.CancelStream(cancellationToken, _currentStreamDetails.StreamIdentifier);
            }

            // Start a new one and store it away
            _currentStreamDetails = Plugin.StreamingProxy.GetLiveTvStream(cancellationToken, channelId);
            return Task.FromResult(_currentStreamDetails.StreamInfo);
        }

        public Task UpdateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            Plugin.TvProxy.DeleteSchedule(cancellationToken, info.Id);
            return CreateSeriesTimerAsync(info, cancellationToken);
        }

        public Task<ImageStream> GetChannelImageAsync(string channelId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ImageStream> GetRecordingImageAsync(string recordingId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            Plugin.TvProxy.DeleteSchedule(cancellationToken, info.Id);
            return CreateTimerAsync(info, cancellationToken);
        }

        public event EventHandler<RecordingStatusChangedEventArgs> RecordingStatusChanged;

        public event EventHandler DataSourceChanged;
    }
}