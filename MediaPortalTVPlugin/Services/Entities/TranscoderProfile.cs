using System.Collections.Generic;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class TranscoderProfile
    {
        public int Bandwidth { get; set; }
        public string Description { get; set; }
        public bool HasVideoStream { get; set; }
        public string MIME { get; set; }
        public int MaxOutputHeight { get; set; }
        public int MaxOutputWidth { get; set; }
        public string Name { get; set; }
        public List<string> Targets { get; set; }
        public string Transport { get; set; }
    }
}