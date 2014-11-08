using System;

namespace MediaPortalTVPlugin.Services.Entities
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
    }
}