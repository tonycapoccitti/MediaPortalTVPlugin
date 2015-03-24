using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.MediaInfo;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;

namespace MediaBrowser.Plugins.MediaPortal.Services.Proxies
{
    /// <summary>
    /// Provides access to the MP streaming functionality
    /// </summary>
    public class StreamingServiceProxy : ProxyBase
    {
        private readonly INetworkManager _networkManager;

        private String _streamingEndpoint = "StreamingService/stream";

        private const int STREAM_TIMEOUT_DIRECT = 10;
        private const int STREAM_TV_RECORDING_PROVIDER = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingServiceProxy"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="serialiser">The serialiser.</param>
        /// <param name="networkManager">The network manager.</param>
        public StreamingServiceProxy(IHttpClient httpClient, IJsonSerializer serialiser, INetworkManager networkManager)
            : base(httpClient, serialiser)
        {
            _networkManager = networkManager;
        }

        /// <summary>
        /// Gets the end point suffix.
        /// </summary>
        /// <value>
        /// The end point suffix.
        /// </value>
        /// <remarks>
        /// The value appended after "MPExtended" on the service url
        /// </remarks>
        protected override string EndPointSuffix
        {
            get { return "StreamingService/json"; }
        }

        /// <summary>
        /// Gets the status information for the Streaming service
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public ServiceDescription GetStatusInfo(CancellationToken cancellationToken)
        {
            return GetFromService<ServiceDescription>(cancellationToken, "GetServiceDescription");
        }

        /// <summary>
        /// Gets the transcoder profiles supported
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public List<TranscoderProfile> GetTranscoderProfiles(CancellationToken cancellationToken)
        {
            return GetFromService<List<TranscoderProfile>>(cancellationToken, "GetTranscoderProfiles");
        }

        /// <summary>
        /// Gets a single transcoder profile.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public TranscoderProfile GetTranscoderProfile(CancellationToken cancellationToken, String name)
        {
            return GetFromService<TranscoderProfile>(cancellationToken, "GetTranscoderProfileByName?name={0}", name);
        }

        /// <summary>
        /// Gets the video stream for an existing recording
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="recordingId">The recording id.</param>
        /// <param name="startPosition">The start position.</param>
        /// <returns></returns>
        public StreamingDetails GetRecordingStream(CancellationToken cancellationToken, String recordingId, TimeSpan startPosition)
        {
            return GetStream(cancellationToken, WebMediaType.Recording, recordingId, startPosition);
        }

        /// <summary>
        /// Gets a live tv stream.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="channelId">The channel to stream.</param>
        /// <returns></returns>
        public StreamingDetails GetLiveTvStream(CancellationToken cancellationToken, String channelId)
        {
            return GetStream(cancellationToken, WebMediaType.TV, channelId, TimeSpan.Zero);
        }

        
        /// <summary>
        /// Cancels an executing stream.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="streamIdentifier">The stream identifier.</param>
        /// <returns></returns>
        public bool CancelStream(CancellationToken cancellationToken, string streamIdentifier)
        {
            return GetFromService<WebBoolResult>(cancellationToken, "StopStream?identifier={0}", streamIdentifier).Result;
        }

        /// <summary>
        /// Retrieves media information from an established stream.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="mediaType">Type of the media.</param>
        /// <param name="streamIdentifier">The stream identifier.</param>
        /// <returns></returns>
        private WebMediaInfo GetMediaInfoFromStream(CancellationToken cancellationToken, WebMediaType mediaType, String streamIdentifier)
        {
            return GetFromService<WebMediaInfo>(cancellationToken, "GetMediaInfo?type={0}&itemId={1}&provider={2}",
                mediaType, streamIdentifier, STREAM_TV_RECORDING_PROVIDER);
        }

        private StreamingDetails GetStream(CancellationToken cancellationToken, WebMediaType webMediaType, string itemId, TimeSpan startPosition)
        {
            // var profile = GetTranscoderProfile(cancellationToken, "Direct");

            var profile = GetTranscoderProfile(cancellationToken, Configuration.StreamingProfileName);
            if (profile == null)
            {
                throw new Exception(String.Format("Cannot find a profile with the name {0}", Configuration.StreamingProfileName));
            }

            var identifier = HttpUtility.UrlEncode(String.Format("{0}-{1}-{2:yyyyMMddHHmmss}", webMediaType, itemId, DateTime.UtcNow));
            var isStreamInitialised = GetFromService<WebBoolResult>(cancellationToken,
                    "InitStream?type={0}&provider={1}&itemId={2}&identifier={3}&idleTimeout={4}&clientDescription={5}",
                    webMediaType,
                    STREAM_TV_RECORDING_PROVIDER, // Provider - use 0 for recordings and tv
                    itemId, // itemId
                    identifier, // identifier
                    STREAM_TIMEOUT_DIRECT,
                    identifier).Result; //Idletimoue

            if (!isStreamInitialised)
            {
                throw new Exception(String.Format("Could not initialise the stream. Identifier={0}", identifier));
            }

            // Returns the url for streaming
            var url = GetFromService<WebStringResult>(cancellationToken,"StartStream?identifier={0}&profileName={1}&startPosition={2}",
                    identifier,
                    profile.Name, // Provider
                    (Int32)startPosition.TotalSeconds).Result;

            var isAuthorised = true;
            foreach (var ipAddress in _networkManager.GetLocalIpAddresses())
            {
                isAuthorised = isAuthorised && GetFromService<WebBoolResult>(
                    cancellationToken, "AuthorizeRemoteHostForStreaming?host={0}", ipAddress).Result;
            }

            if (!isAuthorised)
            {
                throw new Exception(String.Format("Could not authorise the stream. Identifier={0}", identifier));
            }

            var streamingDetails = new StreamingDetails()
            {
                StreamIdentifier = identifier,
                SourceInfo = new MediaSourceInfo()
                {
                    Path = url,
                    Protocol = MediaProtocol.Http,
                    Id = itemId,
                }
            };

            var mediaInfoId = webMediaType == WebMediaType.Recording ? itemId : identifier;
            var mediaInfo = GetMediaInfoFromStream(cancellationToken, webMediaType, mediaInfoId);
            if (mediaInfo != null)
            {
                streamingDetails.SourceInfo.Container = mediaInfo.Container;
                streamingDetails.SourceInfo.RunTimeTicks = TimeSpan.FromSeconds(mediaInfo.Duration).Ticks;
                
                //streamingDetails.SourceInfo.AudioChannels = mediaInfo.AudioStreams.Count;
                //var defaultAudioStream = mediaInfo.AudioStreams.FirstOrDefault();
                //if (defaultAudioStream != null)
                //{
                //    streamingDetails.SourceInfo.AudioCodec = defaultAudioStream.Codec;
                //}

                //var defaultVideoStream = mediaInfo.VideoStreams.FirstOrDefault();
                //if (defaultVideoStream != null)
                //{
                //    streamingDetails.SourceInfo.VideoCodec = defaultVideoStream.Codec;
                //    streamingDetails.SourceInfo.Height = defaultVideoStream.Height;
                //    streamingDetails.SourceInfo.Width = defaultVideoStream.Width;
                //}
            }

            return streamingDetails;
        }

        /// <summary>
        /// Gets the recording image URL.
        /// </summary>
        /// <param name="recordingId">The recording id.</param>
        /// <returns></returns>
        public String GetRecordingImageUrl(String recordingId)
        {
            return GetUrl(_streamingEndpoint, "ExtractImage?type={0}&provider={1}&position={2}&itemid={3}",
                WebMediaType.Recording,
                STREAM_TV_RECORDING_PROVIDER,
                Configuration.PreviewThumbnailOffsetMinutes * 60,
                recordingId);
        }

        /// <summary>
        /// Gets the channel logo URL.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <returns></returns>
        public String GetChannelLogoUrl(int channelId)
        {
            return GetUrl(_streamingEndpoint, "GetArtworkResized?id={0}&artworktype={1}&offset=0&mediatype={2}&maxWidth=160&maxHeight=160",
                    channelId, (Int32)WebFileType.Logo, (Int32)WebMediaType.TV);
        }
    }
}