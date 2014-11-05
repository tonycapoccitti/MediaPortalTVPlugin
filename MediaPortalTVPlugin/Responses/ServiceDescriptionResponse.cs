using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPortalTVPlugin.Responses
{
    public class ServiceDescriptionResponse
    {
        public ServiceDescriptionResponse(Stream stream, IJsonSerializer json)
        {
            Result = json.DeserializeFromStream<ServiceDescription>(stream);
        }

        public ServiceDescription Result { get; set; }
    }

    public class ServiceDescription
    {
        public int ApiVersion { get; set; }
        public bool HasConnectionToTVServer { get; set; }
        public string ServiceVersion { get; set; }
    }
}
