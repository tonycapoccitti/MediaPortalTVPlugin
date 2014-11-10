using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;

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
    }
}
