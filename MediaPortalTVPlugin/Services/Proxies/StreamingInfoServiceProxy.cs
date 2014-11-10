using System.Collections.Generic;
using System.Threading;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;

namespace MediaBrowser.Plugins.MediaPortal.Services.Proxies
{
    public class StreamingInfoServiceProxy : ProxyBase
    {
        public StreamingInfoServiceProxy(IHttpClient httpClient, IJsonSerializer serialiser)
            : base(httpClient, serialiser)
        {
        }

        protected override string EndPointSuffix
        {
            get { return "StreamingService/json"; }
        }

        public List<TranscoderProfile> GetTranscoderProfiles(CancellationToken cancellationToken)
        {
            return GetFromService<List<TranscoderProfile>>(cancellationToken, "GetTranscoderProfiles");
        }
    }
}