using System;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class Schedule
    {
        public int BitRateMode { get; set; }
        public DateTime Canceled { get; set; }
        public int ChannelId { get; set; }
        public string Directory { get; set; }
        public bool DoesUseEpisodeManagement { get; set; }
        public DateTime EndTime { get; set; }
        public int Id { get; set; }
        public bool IsChanged { get; set; }
        public bool IsManual { get; set; }
        public DateTime KeepDate { get; set; }
        public int KeepMethod { get; set; }
        public long MaxAirings { get; set; }
        public int ParentScheduleId { get; set; }
        public int PostRecordInterval { get; set; }
        public int PreRecordInterval { get; set; }
        public int Priority { get; set; }
        public int Quality { get; set; }
        public int QualityType { get; set; }
        public int RecommendedCard { get; set; }
        public int ScheduleType { get; set; }
        public bool Series { get; set; }
        public DateTime StartTime { get; set; }
        public string Title { get; set; }
    }
}