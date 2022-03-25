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
        /// Subtracts the given <see cref="DateRange"/> from a <see cref="TimeSpan"/> and returns
        /// resulting <see cref="TimeSpan"/>.
        /// </summary>
        public static TimeSpan SubtractDateRange(this TimeSpan timeSpan, DateRange DateRange) => timeSpan - DateRange.TimeSpan;

        /// <summary>
        /// Convert a <see cref="TimeSpan"/> to a human readable string.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert</param>
        /// <returns>A human readable string for <paramref name="timeSpan"/></returns>
        public static string ToDisplayString(this TimeSpan timeSpan) => new DateRange(timeSpan).ToString();

        /// <summary>
        /// Rounds <paramref name="timeSpan"/> to the nearest <see cref="RoundTo"/>.
        /// </summary>
        /// <returns>The rounded <see cref="TimeSpan"/>.</returns>
        public static TimeSpan Round(this TimeSpan timeSpan, RoundTo rt)
        {
            TimeSpan rounded;

            switch (rt)
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
                default:
                    {
                        rounded = new TimeSpan(timeSpan.Ticks);
                        break;
                    }
            }

            return rounded;
        }
    }
}