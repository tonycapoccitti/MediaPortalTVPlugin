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
        TasProxy _tasWrapper;

        public MediaPortal1TVService(IHttpClient httpClient, IJsonSerializer jsonSerialiser)
        {
            _tasWrapper = new TasProxy(httpClient, jsonSerialiser, Plugin.Logger);
            // _tasWrapper.ValidateConnectivity();
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
            //var result = await _tasWrapper.GetChannels(cancellationToken).ConfigureAwait(false);
            //_logger.Info("TAS Result: {0}", String.Concat(",", result));

            return new ChannelInfo[] {
                new ChannelInfo(){ Id = "1", Name = "BBC 1", ChannelType = MediaBrowser.Model.LiveTv.ChannelType.TV, Number = "1" },
                new ChannelInfo(){ Id = "2", Name = "BBC 2", ChannelType = MediaBrowser.Model.LiveTv.ChannelType.TV, Number = "2" },
                new ChannelInfo(){ Id = "3", Name = "BBC 3", ChannelType = MediaBrowser.Model.LiveTv.ChannelType.TV, Number = "3" },
            };
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
            return Task.FromResult<IEnumerable<ProgramInfo>>(new List<ProgramInfo>());
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
            return Task.FromResult<LiveTvServiceStatusInfo>(null);
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