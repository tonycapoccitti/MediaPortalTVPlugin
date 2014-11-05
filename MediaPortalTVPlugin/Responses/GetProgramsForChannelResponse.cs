using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortalTVPlugin.Responses
{
    public class GetProgramsForChannelResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetProgramsForChannelResponse"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="json">The json.</param>
        public GetProgramsForChannelResponse(Stream stream, IJsonSerializer json)
        {
            Result = json.DeserializeFromStream<List<Program>>(stream);
        }

        public List<Program> Result { get; set; }
    }

    public class Program
    {
        public int ChannelId { get; set; }
        public string Description { get; set; }
        public int DurationInMinutes { get; set; }
        public DateTime EndTime { get; set; }
        public int Id { get; set; }
        public bool IsScheduled { get; set; }
        public DateTime StartTime { get; set; }
        public string Title { get; set; }
    }
}
