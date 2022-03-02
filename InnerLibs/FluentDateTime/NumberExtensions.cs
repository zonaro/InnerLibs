using System;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// Static class containing Fluent <see cref="DateTime"/> extension methods.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Years.
        /// </summary>
        public static DateRange Years(this int years)
        {
            return new DateRange(DateTime.MinValue, DateTime.MinValue.AddYears(1));
        }

        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Quarters.
        /// </summary>
        /// <param name="quarters"></param>
        /// <returns></returns>
        public static DateRange Quarters(this int quarters)
        {
            return new DateRange(DateTime.MinValue, DateTime.MinValue.AddMonths(quarters * 3));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> value for given number of Months.
        /// </summary>
        public static DateRange Months(this int months)
        {
            return new DateRange(DateTime.MinValue, DateTime.MinValue.AddMonths(months));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of weeks * 7).
        /// </summary>
        public static DateRange Weeks(this int weeks)
        {
            return new DateRange(TimeSpan.FromDays(weeks * 7));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of weeks * 7).
        /// </summary>
        public static DateRange Weeks(this double weeks)
        {
            return new DateRange(TimeSpan.FromDays(weeks * 7));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public static DateRange Days(this int days)
        {
            return new DateRange(TimeSpan.FromDays(days));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public static DateRange Days(this double days)
        {
            return new DateRange(TimeSpan.FromDays(days));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public static DateRange Hours(this int hours)
        {
            return new DateRange(TimeSpan.FromHours(hours));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public static DateRange Hours(this double hours)
        {
            return new DateRange(TimeSpan.FromHours(hours));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public static DateRange Minutes(this int minutes)
        {
            return new DateRange(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public static DateRange Minutes(this double minutes)
        {
            return new DateRange(TimeSpan.FromMinutes(minutes));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public static DateRange Seconds(this int seconds)
        {
            return new DateRange(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public static DateRange Seconds(this double seconds)
        {
            return new DateRange(TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public static DateRange Milliseconds(this int milliseconds)
        {
            return new DateRange(TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public static DateRange Milliseconds(this double milliseconds)
        {
            return new DateRange(TimeSpan.FromMilliseconds(milliseconds));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of ticks.
        /// </summary>
        public static DateRange Ticks(this int ticks)
        {
            return new DateRange(TimeSpan.FromTicks(ticks));
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of ticks.
        /// </summary>
        public static DateRange Ticks(this long ticks)
        {
            return new DateRange(TimeSpan.FromTicks(ticks));
        }
    }
}