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
    /// <summary>
    /// Provides access to the MP tv service functionality
    /// </summary>
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

        public List<TunerCard> GetTunerCards(CancellationToken cancellationToken)
        {
            return GetFromService<List<TunerCard>>(cancellationToken, "GetCards");
        }

        public List<ActiveTunerCard> GetActiveCards(CancellationToken cancellationToken)
        {
            return GetFromService<List<ActiveTunerCard>>(cancellationToken, "GetActiveCards");
        }

        public List<ChannelGroup> GetChannelGroups(CancellationToken cancellationToken)
        {
            return GetFromService<List<ChannelGroup>>(cancellationToken, "GetGroups").OrderBy(g => g.SortOrder).ToList();
        }
        
        public IEnumerable<ChannelInfo> GetChannels(CancellationToken cancellationToken)
        {
            var builder = new StringBuilder("GetChannelsDetailed");
            if (Configuration.DefaultChannelGroup > 0)
            {
                // This is the only way to get out the channels in the same order that MP displays them.
                // No idea why you sepecify Year as the sort field!
                builder.AppendFormat("?groupId={0}", Configuration.DefaultChannelGroup);
            }

            var response = GetFromService<List<Channel>>(cancellationToken, builder.ToString());
            IEnumerable<Channel> query = response;

            switch (Configuration.DefaultChannelSortOrder)
            {
                case ChannelSorting.ChannelName:
                    query = query.OrderBy(q => q.Title);
                    break;
                case ChannelSorting.ChannelNumber:
                    query = query.OrderBy(q => q.Id);
                    break;
            }

            return query.Where(c => c.VisibleInGuide).Select(c => new ChannelInfo()
            {
                Id = c.Id.ToString(CultureInfo.InvariantCulture),
                ChannelType = c.IsTv ? ChannelType.TV : ChannelType.Radio,
                Name = c.Title,
                Number = c.ExternalId,
                ImageUrl = _wssProxy.GetChannelLogoUrl(c.Id)
            });
        }

        private Program GetProgram(CancellationToken cancellationToken, String programId)
        {
            return GetFromService<Program>(cancellationToken, "GetProgramDetailedById?programId={0}", programId);
        }

        public IEnumerable<ProgramInfo> GetPrograms(string channelId, DateTime startDateUtc, DateTime endDateUtc,
            CancellationToken cancellationToken)
        {
            var response = GetFromService<List<Program>>(
                cancellationToken,
                "GetProgramsDetailedForChannel?channelId={0}&starttime={1}&endtime={2}",
                channelId,
                startDateUtc.ToLocalTime().ToUrlDate(),
                endDateUtc.ToLocalTime().ToUrlDate());

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
                    //IsSeries = !String.IsNullOrEmpty(p.SeriesNum) || !String.IsNullOrEmpty(p.EpisodeNum),
                    IsSeries = true, // Set this to allow series scheduling for all programs
                    Name = p.Title,
                    Overview = p.Description,
                    // OriginalAirDate = p.OriginalAirDate
                };
                
                if (!String.IsNullOrEmpty(p.Genre))
                {
                    program.Genres.Add(p.Genre);
                    program.IsMovie = p.Genre == "Film" || p.Genre == "Movie"; // HACK: Replace with a lookup in the MPGenres
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
                    ImageUrl = _wssProxy.GetRecordingImageUrl(r.Id.ToString()),
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
            var schedulingType = (WebScheduleType)schedule.ScheduleType;

            // Initialise
            seriesTimerInfo.Days = new List<DayOfWeek>();
            seriesTimerInfo.RecordAnyChannel = false;
            seriesTimerInfo.RecordAnyTime = false;
            seriesTimerInfo.RecordNewOnly = false;

            switch (schedulingType)
            {
                case WebScheduleType.EveryTimeOnThisChannel:
                case WebScheduleType.EveryTimeOnEveryChannel:
                    seriesTimerInfo.RecordAnyTime = schedulingType == WebScheduleType.EveryTimeOnThisChannel;
                    seriesTimerInfo.RecordAnyChannel = schedulingType == WebScheduleType.EveryTimeOnEveryChannel;
                    break;

                case WebScheduleType.Daily:
                case WebScheduleType.WorkingDays:
                case WebScheduleType.Weekends:
                case WebScheduleType.Weekly:

                    if (schedulingType == WebScheduleType.Weekly)
                    {
                        seriesTimerInfo.Days.Add(schedule.StartTime.DayOfWeek);
                    }

                    if (schedulingType == WebScheduleType.Daily || schedulingType == WebScheduleType.WorkingDays)
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

                    if (schedulingType == WebScheduleType.Daily || schedulingType == WebScheduleType.Weekends)
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
            var programData = GetProgram(cancellationToken, schedule.ProgramId);
            if (programData == null)
            {
                throw ExceptionHelper.CreateArgumentException("schedule.ProgramId", "The program id {0} could not be found", schedule.ProgramId);
            }

            var builder = new StringBuilder("AddScheduleDetailed?");
            builder.AppendFormat("channelid={0}&", programData.ChannelId);
            builder.AppendFormat("title={0}&", programData.Title);
            builder.AppendFormat("starttime={0}&", programData.StartTime.ToLocalTime().ToUrlDate());
            builder.AppendFormat("endtime={0}&", programData.EndTime.ToLocalTime().ToUrlDate());
            builder.AppendFormat("scheduletype={0}&", (Int32)schedule.ToScheduleType());

            if (schedule.IsPrePaddingRequired & schedule.PrePaddingSeconds > 0)
            {
                builder.AppendFormat("preRecordInterval={0}&", TimeSpan.FromSeconds(schedule.PrePaddingSeconds).RoundUpMinutes());
            }

            if (schedule.IsPostPaddingRequired & schedule.PostPaddingSeconds > 0)
            {
                builder.AppendFormat("postRecordInterval={0}&", TimeSpan.FromSeconds(schedule.PostPaddingSeconds).RoundUpMinutes());
            }

            builder.Remove(builder.Length - 1, 1);

            Plugin.Logger.Info("Creating series scheule with StartTime: {0}, EndTime: {1}, ProgramData from MP: {2}",
                schedule.StartDate, schedule.EndDate, builder.ToString());

            var response = GetFromService<WebBoolResult>(cancellationToken, builder.ToString());
            if (!response.Result)
            {
                throw new LiveTvConflictException();
            }
        }

        public void CreateSchedule(CancellationToken cancellationToken, TimerInfo timer)
        {
            var programData = GetProgram(cancellationToken, timer.ProgramId);
            if (programData == null)
            {
                throw ExceptionHelper.CreateArgumentException("timer.ProgramId", "The program id {0} could not be found", timer.ProgramId);
            }

            var builder = new StringBuilder("AddScheduleDetailed?");
            builder.AppendFormat("channelid={0}&", programData.ChannelId);
            builder.AppendFormat("title={0}&", programData.Title);
            builder.AppendFormat("starttime={0}&", programData.StartTime.ToLocalTime().ToUrlDate());
            builder.AppendFormat("endtime={0}&", programData.EndTime.ToLocalTime().ToUrlDate());
            builder.AppendFormat("scheduletype={0}&", (Int32)WebScheduleType.Once);

            if (timer.IsPrePaddingRequired & timer.PrePaddingSeconds > 0)
            {
                builder.AppendFormat("preRecordInterval={0}&", timer.PrePaddingSeconds / 60);
            }

            if (timer.IsPostPaddingRequired & timer.PostPaddingSeconds > 0)
            {
                builder.AppendFormat("postRecordInterval={0}&", timer.PostPaddingSeconds / 60);
            }

            builder.Remove(builder.Length - 1, 1);

            Plugin.Logger.Info("Creating scheule with StartTime: {0}, EndTime: {1}, ProgramData from MP: {2}",
                timer.StartDate, timer.EndDate, builder.ToString());

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

        #region Other Methods

        public ScheduleDefaults GetScheduleDefaults(CancellationToken cancellationToken)
        {
            Int32 preRecordSecs;
            Int32 postRecordSecs;

            if (!Int32.TryParse(ReadSettingFromDatabase(cancellationToken, "preRecordInterval"), out preRecordSecs))
            {
                Plugin.Logger.Warn("Unable to read the setting 'preRecordInterval' from MP");
            }

            if (!Int32.TryParse(ReadSettingFromDatabase(cancellationToken, "postRecordInterval"), out postRecordSecs))
            {
                Plugin.Logger.Warn("Unable to read the setting 'postRecordInterval' from MP");
            }

            return new ScheduleDefaults()
            {
                PreRecordInterval = TimeSpan.FromMinutes(preRecordSecs),
                PostRecordInterval = TimeSpan.FromMinutes(postRecordSecs),
            };
        }

        public String ReadSettingFromDatabase(CancellationToken cancellationToken, String name)
        {
            return GetFromService<WebStringResult>(cancellationToken, "ReadSettingFromDatabase?tagName={0}", name).Result;
        }

        #endregion
    }
}