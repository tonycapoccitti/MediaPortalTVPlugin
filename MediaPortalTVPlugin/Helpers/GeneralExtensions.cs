using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Plugins.MediaPortal.Entities;

namespace MediaBrowser.Plugins.MediaPortal.Helpers
{
    public static class GeneralExtensions
    {
        public static String ToUrlDate(this DateTime value)
        {
            return value.ToString("s");
        }

        public static Boolean IsDaily(this List<DayOfWeek> value)
        {
            return value.Contains(DayOfWeek.Monday) &&
                   value.Contains(DayOfWeek.Tuesday) &&
                   value.Contains(DayOfWeek.Wednesday) &&
                   value.Contains(DayOfWeek.Thursday) &&
                   value.Contains(DayOfWeek.Friday) &&
                   value.Contains(DayOfWeek.Saturday) &&
                   value.Contains(DayOfWeek.Sunday);
        }

        public static Boolean IsWorkingDays(this List<DayOfWeek> value)
        {
            return value.Count == 5 &&
                   value.Contains(DayOfWeek.Monday) &&
                   value.Contains(DayOfWeek.Tuesday) &&
                   value.Contains(DayOfWeek.Wednesday) &&
                   value.Contains(DayOfWeek.Thursday) &&
                   value.Contains(DayOfWeek.Friday);
        }

        public static Boolean IsWeekends(this List<DayOfWeek> value)
        {
            return value.Count == 2 && 
                   value.Contains(DayOfWeek.Saturday) &&
                   value.Contains(DayOfWeek.Sunday);
        }

        public static ScheduleType ToScheduleType(this SeriesTimerInfo info)
        {
            if (info.RecordAnyChannel)
            {
                return ScheduleType.EveryTimeOnEveryChannel;
            }

            if (info.RecordAnyTime)
            {
                return ScheduleType.EveryTimeOnThisChannel;
            }

            if (info.Days.Count == 0)
            {
                return ScheduleType.Once;
            }

            if (info.Days.Count == 1)
            {
                return ScheduleType.Weekly;
            }

            if (info.Days.IsDaily())
            {
                return ScheduleType.Daily;
            }         

            if (info.Days.IsWeekends())
            {
                return ScheduleType.Weekends;
            }

            if (info.Days.IsWorkingDays())
            {
                return ScheduleType.WorkingDays;
            }

            throw new InvalidOperationException();
        }

        public static IEnumerable<TResult> Process<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> func) where TResult : class
        {
            return source.Select(func).Where(result => result != null);
        }
    }
}
