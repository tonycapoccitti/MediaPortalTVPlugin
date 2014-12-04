using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using MediaBrowser.Controller.Net;
using MediaBrowser.Plugins.MediaPortal.Services.Exceptions;

using ServiceStack;

namespace MediaBrowser.Plugins.MediaPortal
{
    [Route("/MediaPortalPlugin/Profiles", "GET", Summary = "Gets Token")]
    public class GetProfiles : IReturn<List<String>>
    {
    }

    public class ServerApiEndpoints : IRestfulService
    {
        public object Get(GetProfiles request)
        {
            List<String> profiles = new List<string>();
            try
            {
                profiles =
                    Plugin.StreamingProxy.GetTranscoderProfiles(new CancellationToken()).Select(p => p.Name).ToList();
            }
            catch (ServiceAuthenticationException exception)
            {
                // Do nothing, allow an empty list to be passed out
            }
            catch (Exception exception)
            {
                Plugin.Logger.ErrorException("There was an issue retrieving transcoding profiles", exception);
            }

            return profiles;
        }
    }
}
