using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaBrowser.Plugins.MediaPortal.Services.Entities
{
    public class ActiveTunerCard
    {
        public int BitRateMode { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string Device { get; set; }
        public bool Enabled { get; set; }
        public int GetTimeshiftStoppedReason { get; set; }
        public bool GrabTeletext { get; set; }
        public bool HasTeletext { get; set; }
        public int Id { get; set; }
        public bool IsGrabbingEpg { get; set; }
        public bool IsRecording { get; set; }
        public bool IsScanning { get; set; }
        public bool IsScrambled { get; set; }
        public bool IsTimeShifting { get; set; }
        public bool IsTunerLocked { get; set; }
        public int MaxChannel { get; set; }
        public int MinChannel { get; set; }
        public string Name { get; set; }
        public int QualityType { get; set; }
        public string RTSPUrl { get; set; }
        public string RecordingFileName { get; set; }
        public string RecordingFolder { get; set; }
        public int RecordingFormat { get; set; }
        public int RecordingScheduleId { get; set; }
        public DateTime RecordingStarted { get; set; }
        public string RemoteServer { get; set; }
        public int SignalLevel { get; set; }
        public int SignalQuality { get; set; }
        public string TimeShiftFileName { get; set; }
        public object TimeShiftFolder { get; set; }
        public DateTime TimeShiftStarted { get; set; }
        public int Type { get; set; }
        public User User { get; set; }
    }
}
