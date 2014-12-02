using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Model.LiveTv;
using MediaBrowser.Model.Net;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Plugins.MediaPortal.Entities;
using MediaBrowser.Plugins.MediaPortal.Helpers;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;

namespace MediaBrowser.Plugins.MediaPortal.Services.Proxies
{
    public class TvServiceProxy : ProxyBase
    {
        private readonly StreamingServiceProxy _wssProxy;

        public TvServiceProxy(IHttpClient httpClient, IJsonSerializer serialiser, StreamingServiceProxy wssProxy)
            : base(httpClient, serialiser)
        {
            _wssProxy = wssProxy;
        }

        protected override string EndPointSuffix
        {
            get { return "TVAccessService/json"; }
        }

        #region Get Methods

        public ServiceDescription GetStatusInfo(CancellationToken cancellationToken)
        {
            return GetFromService<ServiceDescription>(cancellationToken, "GetServiceDescription");
        }

        public IEnumerable<ChannelInfo> GetChannels(CancellationToken cancellationToken)
        {
            var response = GetFromService<List<Channel>>(cancellationToken, "GetChannelsDetailed");

            return response.Where(c => c.VisibleInGuide).Select(c => new ChannelInfo()
            {
                Id = c.Id.ToString(CultureInfo.InvariantCulture),
                ChannelType = c.IsTv ? ChannelType.TV : ChannelType.Radio,
                Name = c.Title,
                Number = c.ExternalId,
                ImageUrl = _wssProxy.GetChannelLogoUrl(c.Id)
            });
        }

        public IEnumerable<ProgramInfo> GetPrograms(string channelId, DateTime startDateUtc, DateTime endDateUtc,
            CancellationToken cancellationToken)
        {
            var response = GetFromService<List<Program>>(
                cancellationToken,
                "GetProgramsDetailedForChannel?channelId={0}&starttime={1}&endtime={2}",
                channelId,
                startDateUtc.ToUrlDate(),
                endDateUtc.ToUrlDate());

            var programs = response.Select(p =>
            {
                var program = new ProgramInfo()
                {
                    ChannelId = channelId,
                    StartDate = p.StartTime,
                    EndDate = p.EndTime,
                    EpisodeTitle = p.EpisodeName,
                    Genres = new List<String>(),
                    Id = p.Id.ToString(CultureInfo.InvariantCulture),
                    IsMovie = false,
                    IsSeries = !String.IsNullOrEmpty(p.SeriesNum),
                    Name = p.Title,
                    Overview = p.Description,
                    OriginalAirDate = p.OriginalAirDate
                };
                
                if (!String.IsNullOrEmpty(p.Genre))
                {
                    program.Genres.Add(p.Genre);
                }

                return program;
            });

            return programs;
        }

        public IEnumerable<RecordingInfo> GetRecordings(CancellationToken cancellationToken)
        {
            var response = GetFromService<List<Recording>>(cancellationToken, "GetRecordings");

            var recordings = response.Select(r =>
            {
                var recording = new RecordingInfo()
                {
                    ChannelId = r.ChannelId.ToString(CultureInfo.InvariantCulture),
                    EndDate = r.EndTime,
                    EpisodeTitle = r.EpisodeName,
                    Genres = new List<String>(),
                    Id = r.Id.ToString(CultureInfo.InvariantCulture),
                    Name = r.Title,
                    Overview = r.Description,
                    ProgramId = r.ScheduleId.ToString(CultureInfo.InvariantCulture),
                    StartDate = r.StartTime,
                    ImageUrl = _wssProxy.GetRecordingImageUrl(r.Id.ToString())
                };

                if (!String.IsNullOrEmpty(r.Genre))
                {
                    recording.Genres.Add(r.Genre);
                }

                return recording;

            }).ToList();

            return recordings;
        }

        public RecordingInfo GetRecording(CancellationToken cancellationToken, String id)
        {
            var response = GetFromService<Recording>(cancellationToken, "GetRecordingById?id={0}", id);

            var recording = new RecordingInfo()
            {
                ChannelId = response.ChannelId.ToString(CultureInfo.InvariantCulture),
                EndDate = response.EndTime,
                EpisodeTitle = response.EpisodeName,
                Genres = new List<String>(),
                Id = response.Id.ToString(CultureInfo.InvariantCulture),
                Name = response.Title,
                Overview = response.Description,
                ProgramId = response.ScheduleId.ToString(CultureInfo.InvariantCulture),
                StartDate = response.StartTime,
                Path = response.FileName,
            };

            if (!String.IsNullOrEmpty(response.Genre))
            {
                recording.Genres.Add(response.Genre);
            }

            return recording;
        }


        public IEnumerable<SeriesTimerInfo> GetSeriesSchedules(CancellationToken cancellationToken)
        {
            var response = GetFromService<List<Schedule>>(cancellationToken, "GetSchedules");

            var recordings = response.Where(r => r.ScheduleType > 0).Select(r =>
            {
                var seriesTimerInfo = new SeriesTimerInfo()
                {
                    ChannelId = r.ChannelId.ToString(CultureInfo.InvariantCulture),
                    EndDate = r.EndTime,
                    Id = r.Id.ToString(CultureInfo.InvariantCulture),
                    Name = r.Title,
                    IsPostPaddingRequired = (r.PostRecordInterval > 0),
                    IsPrePaddingRequired = (r.PreRecordInterval > 0),
                    PostPaddingSeconds = r.PostRecordInterval * 60,
                    PrePaddingSeconds = r.PreRecordInterval * 60,
                    StartDate = r.StartTime,
                };

                UpdateScheduling(seriesTimerInfo, r);
                
                return seriesTimerInfo;
            });

            return recordings;
        }

        private void UpdateScheduling(SeriesTimerInfo seriesTimerInfo, Schedule schedule)
        {
            var schedulingType = (ScheduleType)schedule.ScheduleType;

            // Initialise
            seriesTimerInfo.Days = new List<DayOfWeek>();
            seriesTimerInfo.RecordAnyChannel = false;
            seriesTimerInfo.RecordAnyTime = false;
            seriesTimerInfo.RecordNewOnly = false;

            switch (schedulingType)
            {
                case ScheduleType.EveryTimeOnThisChannel:
                case ScheduleType.EveryTimeOnEveryChannel:
                    seriesTimerInfo.RecordAnyTime = schedulingType == ScheduleType.EveryTimeOnThisChannel;
                    seriesTimerInfo.RecordAnyChannel = schedulingType == ScheduleType.EveryTimeOnEveryChannel;
                    break;

                case ScheduleType.Daily:
                case ScheduleType.WorkingDays:
                case ScheduleType.Weekends:
                case ScheduleType.Weekly:

                    if (schedulingType == ScheduleType.Weekly)
                    {
                        seriesTimerInfo.Days.Add(schedule.StartTime.DayOfWeek);
                    }

                    if (schedulingType == ScheduleType.Daily || schedulingType == ScheduleType.WorkingDays)
                    {
                        seriesTimerInfo.Days.AddRange(new[]
                        {
                            DayOfWeek.Monday,
                            DayOfWeek.Tuesday,
                            DayOfWeek.Wednesday,
                            DayOfWeek.Thursday,
                            DayOfWeek.Friday,
                        });
                    }

                    if (schedulingType == ScheduleType.Daily || schedulingType == ScheduleType.Weekends)
                    {
                        seriesTimerInfo.Days.AddRange(new[]
                        {
                           DayOfWeek.Saturday,
                           DayOfWeek.Sunday,
                        });
                    }
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Should not be processing scheduling for ScheduleType={0}", schedulingType));
            }
        }

        public IEnumerable<TimerInfo> GetSchedules(CancellationToken cancellationToken)
        {
            var response = GetFromService<List<Schedule>>(cancellationToken, "GetSchedules");

            var recordings = response.Where(r => r.ScheduleType == 0).Select(r => new TimerInfo()
            {
                ChannelId = r.ChannelId.ToString(CultureInfo.InvariantCulture),
                EndDate = r.EndTime,
                Id = r.Id.ToString(CultureInfo.InvariantCulture),
                Name = r.Title,
                IsPostPaddingRequired = (r.PostRecordInterval > 0),
                IsPrePaddingRequired = (r.PreRecordInterval > 0),
                PostPaddingSeconds = r.PostRecordInterval * 60,
                PrePaddingSeconds = r.PreRecordInterval * 60,
                Status = RecordingStatus.Scheduled,
                StartDate = r.StartTime,
            }).ToList();

            return recordings;
        }

        #endregion

        #region Streaming Methods

        public String SwitchTVChannelAndStream(CancellationToken cancellationToken, Int32 channelId)
        {
            var userName = String.Empty;
            return GetFromService<String>(cancellationToken, 
                "SwitchTVServerToChannelAndGetStreamingUrl?userName={0}&channelId={1}",
                userName,
                channelId);
        }

        #endregion

        #region Create Methods

        public void CreateSeriesSchedule(CancellationToken cancellationToken, SeriesTimerInfo schedule)
        {
            var builder = new StringBuilder("AddScheduleDetailed?");
            builder.AppendFormat("channelid={0}&", schedule.ChannelId);
            builder.AppendFormat("title={0}&", schedule.Name);
            builder.AppendFormat("starttime={0}&", schedule.StartDate.ToUrlDate());
            builder.AppendFormat("endtime={0}&", schedule.EndDate.ToUrlDate());
            builder.AppendFormat("scheduletype={0}&", (Int32)schedule.ToScheduleType());

            if (schedule.IsPrePaddingRequired & schedule.PrePaddingSeconds > 0)
            {
                builder.AppendFormat("preRecordInterval={0}&", schedule.PrePaddingSeconds / 60);
            }

            if (schedule.IsPostPaddingRequired & schedule.PostPaddingSeconds > 0)
            {
                builder.AppendFormat("postRecordInterval={0}&", schedule.PostPaddingSeconds / 60);
            }

            builder.Remove(builder.Length - 1, 1);

            var response = GetFromService<WebBoolResult>(cancellationToken, builder.ToString());
            if (!response.Result)
            {
                throw new LiveTvConflictException();
            }
        }

        public void CreateSchedule(CancellationToken cancellationToken, TimerInfo timer)
        {
            var builder = new StringBuilder("AddScheduleDetailed?");
            builder.AppendFormat("channelid={0}&", timer.ChannelId);
            builder.AppendFormat("title={0}&", timer.Name);
            builder.AppendFormat("starttime={0}&", timer.StartDate.ToUrlDate());
            builder.AppendFormat("endtime={0}&", timer.EndDate.ToUrlDate());
            builder.AppendFormat("scheduletype={0}&", (Int32)ScheduleType.Once);

            if (timer.IsPrePaddingRequired & timer.PrePaddingSeconds > 0)
            {
                builder.AppendFormat("preRecordInterval={0}&", timer.PrePaddingSeconds / 60);
            }

            if (timer.IsPostPaddingRequired & timer.PostPaddingSeconds > 0)
            {
                builder.AppendFormat("postRecordInterval={0}&", timer.PostPaddingSeconds / 60);
            }

            builder.Remove(builder.Length - 1, 1);

            var response = GetFromService<WebBoolResult>(cancellationToken, builder.ToString());
            if (!response.Result)
            {
                throw new LiveTvConflictException();
            }
        }

        #endregion

        #region Delete Methods

        public void DeleteSchedule(CancellationToken cancellationToken, string scheduleId)
        {
            var response = GetFromService<WebBoolResult>(cancellationToken,
                "DeleteSchedule?scheduleId={0}",
                scheduleId);

            if (!response.Result)
            {
                throw new LiveTvConflictException();
            }
        }

        public void DeleteRecording(CancellationToken cancellationToken, string programId)
        {
            try
            {
                var response = GetFromService<WebBoolResult>(cancellationToken,
                    "DeleteRecording?id={0}",
                    programId);

                if (!response.Result)
                {
                    throw new LiveTvConflictException();
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.OfType<HttpException>().All(e => e.StatusCode != HttpStatusCode.NotFound))
                {
                    throw;
                }
            }

        }

        #endregion
    }
}