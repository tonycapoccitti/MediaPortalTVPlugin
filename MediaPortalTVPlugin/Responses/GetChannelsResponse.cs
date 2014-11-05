using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortalTVPlugin.Responses
{
    public class GetChannelsResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetChannelsResponse"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="json">The json.</param>
        public GetChannelsResponse(Stream stream, IJsonSerializer json)
        {
            Result = json.DeserializeFromStream<List<Channel>>(stream);
        }

        public List<Channel> Result { get; set; }
    }

    public class Channel
    {
        public int Id { get; set; }
        public bool IsRadio { get; set; }
        public bool IsTv { get; set; }
        public string Title { get; set; }
    }
}
