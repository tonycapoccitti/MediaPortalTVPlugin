using System;
using System.Collections.Generic;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class Channel
    {
        public int Id { get; set; }
        public bool IsRadio { get; set; }
        public bool IsTv { get; set; }
        public string Title { get; set; }
        public Program CurrentProgram { get; set; }
        public bool EpgHasGaps { get; set; }
        public string ExternalId { get; set; }
        public int FreeToAir { get; set; }
        public bool GrabEpg { get; set; }
        public List<string> GroupNames { get; set; }
        public bool IsChanged { get; set; }
        public DateTime LastGrabTime { get; set; }
        public Program NextProgram { get; set; }
        public int TimesWatched { get; set; }
        public DateTime TotalTimeWatched { get; set; }
        public bool VisibleInGuide { get; set; }
    }
}