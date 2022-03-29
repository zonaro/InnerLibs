using System;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// Static class containing Fluent <see cref="TimeSpan"/> extension methods.
    /// </summary>
    public static partial class TimeSpanExtensions
    {
        /// <summary>
        /// Adds the given <see cref="DateRange"/> from a <see cref="TimeSpan"/> and returns
        /// resulting <see cref="TimeSpan"/>.
        /// </summary>
        public static TimeSpan AddDateRange(this TimeSpan timeSpan, DateRange DateRange) => timeSpan + DateRange.TimeSpan;

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
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public static TimeSpan Milliseconds(this int milliseconds) => TimeSpan.FromMilliseconds(milliseconds);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Milliseconds.
        /// </summary>
        public static TimeSpan Milliseconds(this double milliseconds) => TimeSpan.FromMilliseconds(milliseconds);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public static TimeSpan Minutes(this int minutes) => TimeSpan.FromMinutes(minutes);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Minutes.
        /// </summary>
        public static TimeSpan Minutes(this double minutes) => TimeSpan.FromMinutes(minutes);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> value for given number of Months.
        /// </summary>
        public static TimeSpan Months(this int months, DateTime? fromDateTime = null) => fromDateTime.OrNow().AddMonths(months) - fromDateTime.OrNow();

        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Quarters.
        /// </summary>
        /// <param name="quarters"></param>
        /// <returns></returns>
        public static TimeSpan Quarters(this int quarters, DateTime? fromDateTime = null) => fromDateTime.OrNow().Date.AddMonths(quarters * 3) - fromDateTime.OrNow().Date;

        /// <summary>
        /// Rounds <paramref name="timeSpan"/> to the nearest <see cref="RoundTo"/>.
        /// </summary>
        /// <returns>The rounded <see cref="TimeSpan"/>.</returns>
        public static TimeSpan Round(this TimeSpan timeSpan, RoundTo round = RoundTo.Second)
        {
            TimeSpan rounded;

            switch (round)
            {
                case RoundTo.Second:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                        if (timeSpan.Milliseconds >= 500)
                        {
                            rounded = rounded + 1.Seconds();
                        }

                        break;
                    }
                case RoundTo.Minute:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
                        if (timeSpan.Seconds >= 30)
                        {
                            rounded = rounded + 1.Minutes();
                        }

                        break;
                    }
                case RoundTo.Hour:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
                        if (timeSpan.Minutes >= 30)
                        {
                            rounded = rounded + 1.Hours();
                        }

                        break;
                    }
                case RoundTo.Day:
                    {
                        rounded = new TimeSpan(timeSpan.Days, 0, 0, 0);
                        if (timeSpan.Hours >= 12)
                        {
                            rounded = rounded + 1.Days();
                        }

                        break;
                    }

                case RoundTo.None:
                default:
                    rounded = new TimeSpan(timeSpan.Ticks);
                    break;
            }

            return rounded;
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public static TimeSpan Seconds(this int seconds) => TimeSpan.FromSeconds(seconds);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Seconds.
        /// </summary>
        public static TimeSpan Seconds(this double seconds) => TimeSpan.FromSeconds(seconds);

        /// <summary>
        /// Subtracts the given <see cref="DateRange"/> from a <see cref="TimeSpan"/> and returns
        /// resulting <see cref="TimeSpan"/>.
        /// </summary>
        public static TimeSpan SubtractDateRange(this TimeSpan timeSpan, DateRange DateRange) => timeSpan - DateRange.TimeSpan;

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of ticks.
        /// </summary>
        public static TimeSpan Ticks(this int ticks) => TimeSpan.FromTicks(ticks);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of ticks.
        /// </summary>
        public static TimeSpan Ticks(this long ticks) => TimeSpan.FromTicks(ticks);

        /// <summary>
        /// Convert a <see cref="TimeSpan"/> to a human readable string.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert</param>
        /// <returns>A human readable string for <paramref name="timeSpan"/></returns>
        public static string ToDisplayString(this TimeSpan timeSpan) => new DateRange(timeSpan).ToString();

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of days * 7).
        /// </summary>
        public static TimeSpan Weeks(this int weeks) => TimeSpan.FromDays(weeks * 7);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of days * 7).
        /// </summary>
        public static TimeSpan Weeks(this double weeks) => TimeSpan.FromDays(weeks * 7);

        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Years.
        /// </summary>
        public static TimeSpan Years(this int years, DateTime? fromDateTime = null) => fromDateTime.OrNow().Date.AddYears(years) - fromDateTime.OrNow().Date;
    }
}