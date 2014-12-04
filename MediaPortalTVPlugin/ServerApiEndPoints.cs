using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using MediaBrowser.Controller.Net;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;

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
                profiles = Plugin.StreamingProxy.GetTranscoderProfiles(new CancellationToken()).Select(p => p.Name).ToList();
            }
            catch (Exception exception)
            {
                
            }
            return profiles;
        }
    }
}
