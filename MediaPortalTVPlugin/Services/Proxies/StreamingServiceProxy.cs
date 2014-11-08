using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;

namespace MediaPortalTVPlugin.Services.Proxies
{
    public class StreamingServiceProxy : ProxyBase
    {
        public StreamingServiceProxy(IHttpClient httpClient, IJsonSerializer serialiser) : base(httpClient, serialiser)
        {
        }

        protected override string EndPointSuffix
        {
            get { return "StreamingService/stream/"; }
        }
    }
}
