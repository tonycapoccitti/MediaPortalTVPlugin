using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Configuration;
using MediaBrowser.Plugins.MediaPortal.Services.Exceptions;

namespace MediaBrowser.Plugins.MediaPortal.Services.Proxies
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
        /// Gets the plugin configuration.
        /// </summary>
        /// <value>
        /// The plugin configuration.
        /// </value>
        public PluginConfiguration Configuration { get { return Plugin.Instance.Configuration;  } }

        protected IHttpClient HttpClient { get; private set; }
        public IJsonSerializer Serialiser { get; private set; }

        /// <summary>
        /// Gets the end point suffix.
        /// </summary>
        /// <value>
        /// The end point suffix.
        /// </value>
        /// <remarks>The value appended after "MPExtended" on the service url</remarks>
        protected abstract String EndPointSuffix { get; }

        /// <summary>
        /// Retrieves a URL for a given action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        protected String GetUrl(String action, params object[] args)
        {
            return GetUrl(EndPointSuffix, action, args);
        }

        /// <summary>
        /// Retrieves a URL for a given action, allows the endpoint to be overriden
        /// </summary>
        /// <param name="endPointSuffixOverride">The endpoint .</param>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        protected String GetUrl(String endPointSuffixOverride, String action, params object[] args)
        {
            var baseUrl = String.Format("http://{0}:{1}/MPExtended/{2}/", Configuration.ApiHostName, Configuration.ApiPortNumber, endPointSuffixOverride);
            return String.Concat(baseUrl, String.Format(action, args));
        }

        /// <summary>
        /// Retrieves data from the service for a given action
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        /// <exception cref="MediaBrowser.Plugins.MediaPortal.Services.Exceptions.ServiceAuthenticationException">There was a problem authenticating with the MP service</exception>
        protected TResult GetFromService<TResult>(CancellationToken cancellationToken, String action, params object[] args)
        {
            var request = CreateRequest(cancellationToken, action, args);
            Task<Stream> task;
            try
            {
                task = HttpClient.Get(request);
                using (var stream = task.Result)
                {
                    return Serialiser.DeserializeFromStream<TResult>(stream);
                }
            }
            catch (AggregateException aggregateException)
            {
                var exception = aggregateException.Flatten().InnerExceptions.OfType<HttpException>().FirstOrDefault();
                if (exception != null && exception.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new ServiceAuthenticationException("There was a problem authenticating with the MP service", exception);    
                }

                throw;
            }
        }

        /// <summary>
        /// Creates a Http request object to be passed into the 
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="args">The arguments.</param>
        /// <returns></returns>
        private HttpRequestOptions CreateRequest(String action, params object[] args)
        {
            var configuration = Plugin.Instance.Configuration;
            var request = new HttpRequestOptions()
            {
                Url = GetUrl(action, args),
                RequestContentType = "application/json",
                LogErrorResponseBody = true,
                LogRequest = true,
            };

            if (configuration.RequiresAuthentication)
            {
                // Add headers?
                string authInfo = String.Format("{0}:{1}", configuration.UserName, configuration.Password);
                authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
                request.RequestHeaders["Authorization"] = "Basic " + authInfo;
            }

            return request;
        }

        private HttpRequestOptions CreateRequest(CancellationToken cancellationToken, String action, params object[] args)
        {
            var request = CreateRequest(action, args);
            request.CancellationToken = cancellationToken;
            return request;
        }
    }
}