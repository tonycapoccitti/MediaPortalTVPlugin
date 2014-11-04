using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaPortalTVPlugin.Utilities
{
    public class TasProxy
    {
        IHttpClient _httpClient;
        IJsonSerializer _jsonSerialiser;
        ILogger _logger;

        String _baseUrl;

        public TasProxy(IHttpClient httpClient, IJsonSerializer jsonSerialiser, ILogger logger)
        {
            _httpClient = httpClient;
            _jsonSerialiser = jsonSerialiser;
            _logger = logger;

            var configuration = Plugin.Instance.Configuration;
            _baseUrl = String.Format("http://{0}:{1}/MPExtended/TVAccessService/json/", configuration.ApiIpAddress, configuration.ApiPortNumber);
        }

        public async Task<Boolean> ValidateConnectivity()
        {
            var configuration = Plugin.Instance.Configuration;
            if (string.IsNullOrEmpty(configuration.ApiIpAddress))
            {
                _logger.Error("[MediaPortal] Api IP Address must be configured.");
                throw new InvalidOperationException("Api IP Address must be configured.");
            }

            if (configuration.ApiPortNumber == 0)
            {
                _logger.Error("[MediaPortal] API Port Number must be configured.");
                throw new InvalidOperationException("API Port Number must be configured.");
            }

            var request = GenerateRequest("GetServiceDescription");
            request.CancellationToken = new CancellationToken();

            using (var stream = await _httpClient.Get(request).ConfigureAwait(false))
            {
                var result = _jsonSerialiser.DeserializeFromStream<String>(stream);
                return !String.IsNullOrEmpty(result);
            }
        }

        public async Task<List<String>> GetChannels(CancellationToken cancellationToken)
        {
            var request = GenerateRequest("GetServiceDescription");
            request.CancellationToken = new CancellationToken();

            using (var stream = await _httpClient.Get(request).ConfigureAwait(false))
            {
                var result = _jsonSerialiser.DeserializeFromStream<String>(stream);
                return new List<String>();
            }
        }

        private HttpRequestOptions GenerateRequest(String action, params object[] args)
        {
            var configuration = Plugin.Instance.Configuration;
            var request = new HttpRequestOptions()
            {
                Url = String.Concat(_baseUrl, String.Format(action, args)),
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
    }
}
