using System;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
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
        public string Classification { get; set; }
        public string EpisodeName { get; set; }
        public string EpisodeNum { get; set; }
        public string EpisodeNumber { get; set; }
        public string EpisodePart { get; set; }
        public string Genre { get; set; }
        public bool HasConflict { get; set; }
        public bool IsChanged { get; set; }
        public bool IsPartialRecordingSeriesPending { get; set; }
        public bool IsRecording { get; set; }
        public bool IsRecordingManual { get; set; }
        public bool IsRecordingOnce { get; set; }
        public bool IsRecordingOncePending { get; set; }
        public bool IsRecordingSeries { get; set; }
        public bool IsRecordingSeriesPending { get; set; }
        public bool Notify { get; set; }
        public DateTime OriginalAirDate { get; set; }
        public int ParentalRating { get; set; }
        public string SeriesNum { get; set; }
        public int StarRating { get; set; }
    }
}