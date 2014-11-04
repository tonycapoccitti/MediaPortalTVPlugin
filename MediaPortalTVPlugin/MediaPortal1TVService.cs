using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaPortalTVPlugin.Utilities;
using MediaBrowser.Model.Logging;

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

        public Task CancelSeriesTimerAsync(string timerId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CancelTimerAsync(string timerId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CloseLiveStream(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateSeriesTimerAsync(SeriesTimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateTimerAsync(TimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public event EventHandler DataSourceChanged;

        public Task DeleteRecordingAsync(string recordingId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<StreamResponseInfo> GetChannelImageAsync(string channelId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<MediaBrowser.Controller.Channels.ChannelMediaInfo> GetChannelStream(string channelId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ChannelInfo>> GetChannelsAsync(System.Threading.CancellationToken cancellationToken)
        {
            //var result = await _tasWrapper.GetChannels(cancellationToken).ConfigureAwait(false);
            //_logger.Info("TAS Result: {0}", String.Concat(",", result));

            return new ChannelInfo[] {
                new ChannelInfo(){ Id = "1", Name = "BBC 1", ChannelType = MediaBrowser.Model.LiveTv.ChannelType.TV, Number = "1" },
                new ChannelInfo(){ Id = "2", Name = "BBC 2", ChannelType = MediaBrowser.Model.LiveTv.ChannelType.TV, Number = "2" },
                new ChannelInfo(){ Id = "3", Name = "BBC 3", ChannelType = MediaBrowser.Model.LiveTv.ChannelType.TV, Number = "3" },
            };
        }

        public Task<SeriesTimerInfo> GetNewTimerDefaultsAsync(System.Threading.CancellationToken cancellationToken, ProgramInfo program = null)
        {
            throw new NotImplementedException();
        }

        public Task<StreamResponseInfo> GetProgramImageAsync(string programId, string channelId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProgramInfo>> GetProgramsAsync(string channelId, DateTime startDateUtc, DateTime endDateUtc, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<StreamResponseInfo> GetRecordingImageAsync(string recordingId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<MediaBrowser.Controller.Channels.ChannelMediaInfo> GetRecordingStream(string recordingId, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<RecordingInfo>> GetRecordingsAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SeriesTimerInfo>> GetSeriesTimersAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<LiveTvServiceStatusInfo> GetStatusInfoAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TimerInfo>> GetTimersAsync(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public string HomePageUrl
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "Media Portal V1 Live TV Service"; }
        }

        public Task RecordLiveStream(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public event EventHandler<RecordingStatusChangedEventArgs> RecordingStatusChanged;

        public Task ResetTuner(string id, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSeriesTimerAsync(SeriesTimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTimerAsync(TimerInfo info, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
