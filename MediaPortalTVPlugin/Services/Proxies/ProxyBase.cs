using System;
using System.Threading;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Serialization;

namespace MediaPortalTVPlugin.Services.Proxies
{
    /// <summary>
    /// Provides base methods for proxy classes
    /// </summary>
    public abstract class ProxyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyBase" /> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="serialiser">The serialiser.</param>
        protected ProxyBase(IHttpClient httpClient, IJsonSerializer serialiser)
        {
            HttpClient = httpClient;
            Serialiser = serialiser;
        }

        /// <summary>
        /// Gets the HTTP client.
        /// </summary>
        protected IHttpClient HttpClient { get; private set; }

        public IJsonSerializer Serialiser { get; private set; }

        protected abstract String EndPointSuffix { get; }

        public static String GetBaseUrl()
        {
            var configuration = Plugin.Instance.Configuration;
            return String.Format("http://{0}:{1}/MPExtended/", configuration.ApiIpAddress, configuration.ApiPortNumber);
        }

        /// <summary>
        /// Creates the request.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        protected HttpRequestOptions CreateRequest(String action, params object[] args)
        {
            var configuration = Plugin.Instance.Configuration;
            var baseUrl = String.Concat(GetBaseUrl(), EndPointSuffix);

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

        protected TResult GetFromService<TResult>(CancellationToken cancellationToken, String action, params object[] args)
        {
            var request = CreateRequest(cancellationToken, action, args);
            using (var stream = HttpClient.Get(request).Result)
            {
                return Serialiser.DeserializeFromStream<TResult>(stream);
            }
        }

        private HttpRequestOptions CreateRequest(CancellationToken cancellationToken, String action,
            params object[] args)
        {
            var request = CreateRequest(action, args);
            request.CancellationToken = cancellationToken;
            return request;
        }
    }
}