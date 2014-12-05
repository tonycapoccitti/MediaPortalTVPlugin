using System;
using System.Collections.Generic;
using System.Linq;
using MediaBrowser.Controller.LiveTv;
using MediaBrowser.Plugins.MediaPortal.Services.Entities;

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

        public static WebScheduleType ToScheduleType(this SeriesTimerInfo info)
        {
            if (info.RecordAnyChannel)
            {
                return WebScheduleType.EveryTimeOnEveryChannel;
            }

            if (info.RecordAnyTime)
            {
                return WebScheduleType.EveryTimeOnThisChannel;
            }

            if (info.Days.Count == 0)
            {
                return WebScheduleType.Once;
            }

            if (info.Days.Count == 1)
            {
                return WebScheduleType.Weekly;
            }

            if (info.Days.IsDaily())
            {
                return WebScheduleType.Daily;
            }         

            if (info.Days.IsWeekends())
            {
                return WebScheduleType.Weekends;
            }

            if (info.Days.IsWorkingDays())
            {
                return WebScheduleType.WorkingDays;
            }

            // if we get here, then the user specified options that are not supported
            // by MP - so specify daily
            return WebScheduleType.Daily;
        }

        public static IEnumerable<TResult> Process<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> func) where TResult : class
        {
            return source.Select(func).Where(result => result != null);
        }

        /// <summary>
        /// Rounds up.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        public static TimeSpan RoundUp(this TimeSpan time, TimeSpan interval)
        {
            Int64 remainder;
            Math.DivRem(time.Ticks, interval.Ticks, out remainder);

            if (remainder == 0)
            {
                return time;
            }

            return TimeSpan.FromTicks(((time.Ticks + interval.Ticks + 1) / interval.Ticks) * interval.Ticks);
        }

        public static Int64 RoundUpMinutes(this TimeSpan time)
        {
            return (Int64)time.RoundUp(TimeSpan.FromMinutes(1)).TotalMinutes;
        }
    }
}
