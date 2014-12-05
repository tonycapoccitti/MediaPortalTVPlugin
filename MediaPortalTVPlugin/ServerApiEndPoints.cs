using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using MediaBrowser.Controller.Net;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;
using MediaBrowser.Plugins.MediaPortal.Services.Exceptions;

using ServiceStack;

namespace MediaBrowser.Plugins.MediaPortal
{
    [Route("/MediaPortalPlugin/Profiles", "GET", Summary = "Gets a list of streaming profiles")]
    public class GetProfiles : IReturn<List<String>>
    {
    }

    [Route("/MediaPortalPlugin/ChannelGroups", "GET", Summary = "Gets a list of channel groups")]
    public class GetChannelGroups : IReturn<List<ChannelGroup>>
    {
    }

    public class ServerApiEndpoints : IRestfulService
    {
        public object Get(GetProfiles request)
        {
            var profiles = new List<string>();
            try
            {
                profiles = Plugin.StreamingProxy.GetTranscoderProfiles(new CancellationToken()).Select(p => p.Name).ToList();
            }
            catch (ServiceAuthenticationException)
            {
                // Do nothing, allow an empty list to be passed out
            }
            catch (Exception exception)
            {
                Plugin.Logger.ErrorException("There was an issue retrieving transcoding profiles", exception);
            }

            return profiles;
        }

        public object Get(GetChannelGroups request)
        {
            var channelGroups = new List<ChannelGroup>();
            try
            {
                channelGroups = Plugin.TvProxy.GetChannelGroups(new CancellationToken());
            }
            catch (ServiceAuthenticationException)
            {
                // Do nothing, allow an empty list to be passed out
            }
            catch (Exception exception)
            {
                Plugin.Logger.ErrorException("There was an issue retrieving channel groups", exception);
            }

            return channelGroups;
        }
    }
}
