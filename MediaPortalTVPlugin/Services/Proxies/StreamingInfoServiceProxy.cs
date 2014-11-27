using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Entities;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;

namespace MediaBrowser.Plugins.MediaPortal.Services.Proxies
{
    public class StreamingInfoServiceProxy : ProxyBase
    {
        private readonly INetworkManager _networkManager;
        private const int STREAM_TIMEOUT_DIRECT = 10;
        private const int STREAM_TIMEOUT_PROXY = 300;
        private const int STREAM_TIMEOUT_HTTPLIVE = 60;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingInfoServiceProxy"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="serialiser">The serialiser.</param>
        /// <param name="networkManager">The network manager.</param>
        public StreamingInfoServiceProxy(IHttpClient httpClient, IJsonSerializer serialiser, INetworkManager networkManager)
            : base(httpClient, serialiser)
        {
            _networkManager = networkManager;
        }

        protected override string EndPointSuffix
        {
            get { return "StreamingService/json"; }
        }

        public List<TranscoderProfile> GetTranscoderProfiles(CancellationToken cancellationToken)
        {
            return GetFromService<List<TranscoderProfile>>(cancellationToken, "GetTranscoderProfiles");
        }

        public TranscoderProfile GetTranscoderProfile(CancellationToken cancellationToken, String name)
        {
            return GetFromService<TranscoderProfile>(cancellationToken, "GetTranscoderProfileByName?name={0}", name);
        }

        public StreamingDetails GetRecordingStream(CancellationToken cancellationToken, String recordingId, TimeSpan startPosition)
        {
            return GetStream(cancellationToken, WebMediaType.Recording, recordingId, startPosition);
        }

        public StreamingDetails GetLiveTvStream(CancellationToken cancellationToken, String channelId)
        {
            return GetStream(cancellationToken, WebMediaType.TV, channelId, TimeSpan.Zero);
        }

        public bool CancelStream(CancellationToken cancellationToken, string streamIdentifier)
        {
            return GetFromService<WebBoolResult>(cancellationToken, "StopStream?identifier={0}", streamIdentifier).Result;
        }

        private StreamingDetails GetStream(CancellationToken cancellationToken, WebMediaType webMediaType, string itemId, TimeSpan startPosition)
        {
            var profile = GetTranscoderProfile(cancellationToken, "Direct");

            // var profile = GetTranscoderProfile(cancellationToken, Configuration.LiveStreamingProfileName);
            if (profile == null)
            {
                throw new Exception(String.Format("Cannot find a profile with the name {0}",
                    Configuration.LiveStreamingProfileName));
            }

            var identifier = HttpUtility.UrlEncode(String.Format("{0}-{1}-{2:yyyyMMddHHmmss}", webMediaType, itemId, DateTime.UtcNow));
            var isStreamInitialised = GetFromService<WebBoolResult>(cancellationToken,
                    "InitStream?type={0}&provider={1}&itemId={2}&identifier={3}&idleTimeout={4}&clientDescription={5}",
                    webMediaType,
                    0, // Provider - use 0 for recordings and tv
                    itemId, // itemId
                    identifier, // identifier
                    STREAM_TIMEOUT_DIRECT,
                    identifier).Result; //Idletimoue

            if (!isStreamInitialised)
            {
                throw new Exception(String.Format("Could not initialise the stream. Identifier={0}", identifier));
            }

            // Returns the url for streaming
            var url =
                GetFromService<WebStringResult>(cancellationToken,
                    "StartStream?identifier={0}&profileName={1}&startPosition={2}",
                    identifier,
                    profile.Name, // Provider
                    (Int32)startPosition.TotalSeconds).Result;

            Boolean isAuthorised = true;
            foreach (var ipAddress in _networkManager.GetLocalIpAddresses())
            {
                isAuthorised = isAuthorised && GetFromService<WebBoolResult>(
                    cancellationToken, "AuthorizeRemoteHostForStreaming?host={0}", ipAddress).Result;
            }

            // var isAuthorisedForStreaming = GetFromService<WebBoolResult>(cancellationToken, "AuthorizeStreaming").Result;

            if (!isAuthorised)
            {
                throw new Exception(String.Format("Could not authorise the stream. Identifier={0}", identifier));
            }

            return new StreamingDetails()
            {
                StreamIdentifier = identifier,
                StreamInfo = new ChannelMediaInfo()
                {
                    Path = url,
                    Protocol = MediaProtocol.Http,
                    Id = itemId,
                }
            };
        }
    }
}