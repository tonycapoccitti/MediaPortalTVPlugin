using System;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Entities;

namespace MediaBrowser.Plugins.MediaPortal.Services.Proxies
{
    /// <summary>
    ///  Provides access to the MP Streaming service
    /// </summary>
    public class StreamingServiceProxy : ProxyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingServiceProxy"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="serialiser">The serialiser.</param>
        public StreamingServiceProxy(IHttpClient httpClient, IJsonSerializer serialiser) : base(httpClient, serialiser)
        {
        }

        protected override string EndPointSuffix
        {
            get { return "StreamingService/stream"; }
        }

        public string GetRecordingImageUrl(String recordingId)
        {
            return GetUrl("ExtractImage?type={0}&provider={1}&position={2}&itemid={3}",
                WebMediaType.Recording,
                0,
                300,
                recordingId);
        }

        public String GetChannelLogoUrl(int channelId)
        {
            return GetUrl("GetArtworkResized?channelId={0}&artworktype={1}&offset=0&mediatype={2}&maxWidth=160&maxHeight=160",
                    channelId, (Int32)WebFileType.Logo, (Int32)WebMediaType.TV);
        }
    }
}