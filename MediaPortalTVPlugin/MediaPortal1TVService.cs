using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaPortalTVPlugin.Utilities;
using MediaBrowser.Model.Logging;
using MediaBrowser.Controller.Channels;

namespace MediaPortalTVPlugin
{
    public class MediaPortal1TVService : ILiveTvService
    {
        ILogger _logger;
        IHttpClient _httpClient;
        IJsonSerializer _jsonSerialiser;

        public MediaPortal1TVService(IHttpClient httpClient, IJsonSerializer jsonSerialiser, ILogger logger)
        {
            _httpClient = httpClient;
            _jsonSerialiser = jsonSerialiser;
            _logger = logger;
        }

        private String GetBaseUrl()
        {
            var configuration = Plugin.Instance.Configuration;
            return String.Format("http://{0}:{1}/MPExtended/", configuration.ApiIpAddress, configuration.ApiPortNumber);
        }

        private HttpRequestOptions GenerateTasRequest(String action, params object[] args)
        {
            var configuration = Plugin.Instance.Configuration;
            var baseUrl = String.Concat(GetBaseUrl(), "TVAccessService/json/");

            var request = new HttpRequestOptions()
            {
                Url = String.Concat(baseUrl, String.Format(action, args)),
                RequestContentType = "application/json",
                LogErrorResponseBody = true,
                LogRequest = true,
            };

            if (configuration.IsAuthenticated)
            {
                // Add headers?
            }

            return request;
        }

        private HttpRequestOptions GenerateWssRequest(String action, params object[] args)
        {
            var configuration = Plugin.Instance.Configuration;
            var baseUrl = String.Format("http://{0}:{1}/MPExtended/StreamingService/stream/", configuration.ApiIpAddress, configuration.ApiPortNumber);
            var request = new HttpRequestOptions()
            {
                Url = String.Concat(baseUrl, String.Format(action, args)),
                RequestContentType = "application/json",
                LogErrorResponseBody = true,
                LogRequest = true,
            };

            if (configuration.IsAuthenticated)
            {
                // Add headers?
            }

            return request;
        }

        public Task CancelSeriesTimerAsync(string timerId, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task CancelTimerAsync(string timerId, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task CloseLiveStream(string id, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task CreateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task CreateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public event EventHandler DataSourceChanged;

        public Task DeleteRecordingAsync(string recordingId, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task<StreamResponseInfo> GetChannelImageAsync(string channelId, CancellationToken cancellationToken)
        {
            return Task.FromResult<StreamResponseInfo>(null);
        }

        public Task<ChannelMediaInfo> GetChannelStream(string channelId, CancellationToken cancellationToken)
        {
            return Task.FromResult<ChannelMediaInfo>(null);
        }

        public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(CancellationToken cancellationToken)
        {
            var options = GenerateTasRequest("GetChannelsBasic");

            List<Responses.Channel> response;
            using (var stream =  await _httpClient.Get(options))
            {
                response = new Responses.GetChannelsResponse(stream, _jsonSerialiser).Result;
            }

            return response.Select(c => new ChannelInfo()
            {
                Id = c.Id.ToString(),
                ChannelType = c.IsTv ? MediaBrowser.Model.LiveTv.ChannelType.TV : MediaBrowser.Model.LiveTv.ChannelType.Radio,
                Name = c.Title,
                Number = c.Id.ToString(),
                ImageUrl = String.Format(
                    "{0}StreamingService/stream/GetArtworkResized?id={1}&artworktype={2}&offset=0&mediatype={3}&maxWidth=160&maxHeight=160",
                    GetBaseUrl(),
                    c.Id, 5, 12)
            });
        }

        public Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(CancellationToken cancellationToken, ProgramInfo program = null)
        {
            return Task.FromResult<SeriesTimerInfo>(null);
        }

        public Task<StreamResponseInfo> GetProgramImageAsync(string programId, string channelId, CancellationToken cancellationToken)
        {
            return Task.FromResult<StreamResponseInfo>(null);
        }

        public Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, CancellationToken cancellationToken)
        {
            // 2014-11-01T23:59:59
            var options = GenerateTasRequest(
                "GetProgramsBasicForChannel?channelId={0}&starttime={1}&endtime={2}",
                channelId,
                startDateUtc.ToString("s"),
                endDateUtc.ToString("s"));

            List<Responses.Program> response;
            using (var stream = _httpClient.Get(options).Result)
            {
                response = new Responses.GetProgramsForChannelResponse(stream, _jsonSerialiser).Result;
            }

            var programs = response.Select(p => new ProgramInfo()
            {
                ChannelId = channelId,
                StartDate = p.StartTime,
                EndDate = p.EndTime,
                // EpisodeTitle = p.Description,
                Id = p.Id.ToString(),
                IsMovie = false,
                Name = p.Title,
                Overview = p.Description,
                Genres = new List<string> { "My Category" }
            });

            return Task.FromResult<IEnumerable<ProgramInfo>>(programs);
        }

        public Task<StreamResponseInfo> GetRecordingImageAsync(string recordingId, CancellationToken cancellationToken)
        {
            return Task.FromResult<StreamResponseInfo>(null);
        }

        public Task<ChannelMediaInfo> GetRecordingStream(string recordingId, CancellationToken cancellationToken)
        {
            return Task.FromResult<ChannelMediaInfo>(null);
        }

        public Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<RecordingInfo>>(new List<RecordingInfo>());
        }

        public Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<SeriesTimerInfo>>(new List<SeriesTimerInfo>());
        }

        public Task<LiveTvServiceStatusInfo> GetStatusInfoAsync(CancellationToken cancellationToken)
        {
            var options = GenerateTasRequest("GetServiceDescription");

            LiveTvServiceStatusInfo result;
            try
            {
                Responses.ServiceDescription response;
                using (var stream = _httpClient.Get(options).Result)
                {
                    response = new Responses.ServiceDescriptionResponse(stream, _jsonSerialiser).Result;
                }

                result = new LiveTvServiceStatusInfo()
                {
                    HasUpdateAvailable = false,
                    Status = MediaBrowser.Model.LiveTv.LiveTvServiceStatus.Ok,
                    StatusMessage = String.Format("MPExtended Service Version: {0} - API Version : {1}", response.ServiceVersion, response.ApiVersion),
                    Tuners = new List<LiveTvTunerInfo>(),
                    Version = "1.0"
                };
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);

                result = new LiveTvServiceStatusInfo()
                {
                    HasUpdateAvailable = false,
                    Status = MediaBrowser.Model.LiveTv.LiveTvServiceStatus.Unavailable,
                    StatusMessage = "Unable to establish a connection with MediaPortal API - check your settings",
                    Tuners = new List<LiveTvTunerInfo>(),
                    Version = "1.0"
                };
            }

            return Task.FromResult<LiveTvServiceStatusInfo>(result);
        }

        public Task<IEnumerable<TimerInfo>> GetTimersAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<TimerInfo>>(new List<TimerInfo>());

        }

        public string HomePageUrl
        {
            get { return "http://www.mediaportal.com/"; }
        }

        public string Name
        {
            get { return "Media Portal V1 Live TV Service"; }
        }

        public Task RecordLiveStream(string id, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public event EventHandler<RecordingStatusChangedEventArgs> RecordingStatusChanged;

        public Task ResetTuner(string id, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task UpdateSeriesTimerAsync(SeriesTimerInfo info, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }

        public Task UpdateTimerAsync(TimerInfo info, CancellationToken cancellationToken)
        {
            return Task.Delay(0);
        }
    }
}