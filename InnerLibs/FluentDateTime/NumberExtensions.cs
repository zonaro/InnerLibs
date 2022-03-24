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
        public static TimeSpan Years(this int years) => DateTime.MinValue.AddYears(years) - DateTime.MinValue;

        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Quarters.
        /// </summary>
        /// <param name="quarters"></param>
        /// <returns></returns>
        public static TimeSpan Quarters(this int quarters) => DateTime.MinValue.AddMonths(quarters * 3) - DateTime.MinValue;

        /// <summary>
        /// Returns <see cref="TimeSpan"/> value for given number of Months.
        /// </summary>
        public static TimeSpan Months(this int months) => DateTime.MinValue.AddMonths(months) - DateTime.MinValue;

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of days * 7).
        /// </summary>
        public static TimeSpan Weeks(this int weeks) => TimeSpan.FromDays(weeks * 7);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of days * 7).
        /// </summary>
        public static TimeSpan Weeks(this double weeks) => TimeSpan.FromDays(weeks * 7);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public static TimeSpan Days(this int days) => TimeSpan.FromDays(days);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public static TimeSpan Days(this double days) => TimeSpan.FromDays(days);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public static TimeSpan Hours(this int hours) => TimeSpan.FromHours(hours);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public static TimeSpan Hours(this double hours) => TimeSpan.FromHours(hours);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public static TimeSpan Minutes(this int minutes) => TimeSpan.FromMinutes(minutes);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public static TimeSpan Minutes(this double minutes) => TimeSpan.FromMinutes(minutes);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public static TimeSpan Seconds(this int seconds) => TimeSpan.FromSeconds(seconds);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public static TimeSpan Seconds(this double seconds) => TimeSpan.FromSeconds(seconds);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public static TimeSpan Milliseconds(this int milliseconds) => TimeSpan.FromMilliseconds(milliseconds);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public static TimeSpan Milliseconds(this double milliseconds) => TimeSpan.FromMilliseconds(milliseconds);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of ticks.
        /// </summary>
        public static TimeSpan Ticks(this int ticks) => TimeSpan.FromTicks(ticks);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of ticks.
        /// </summary>
        public static TimeSpan Ticks(this long ticks) => TimeSpan.FromTicks(ticks);
    }
}