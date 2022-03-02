using System;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// Static class containing Fluent <see cref="TimeSpan"/> extension methods.
    /// </summary>
    public static class TimeSpanOffsetExtensions
    {
        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from current date (<see cref="DateTime.Now"/>) and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this TimeSpan from)
        {
            return from.Before(DateTimeOffset.Now);
        }

        /// <summary>
        /// Subtracts given <see cref="DateRange"/> from current date (<see cref="DateTimeOffset.Now"/>) and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this DateRange from)
        {
            return from.Before(DateTimeOffset.Now);
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this TimeSpan from, DateTimeOffset originalValue)
        {
            return from.Before(originalValue);
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this DateRange from, DateTimeOffset originalValue)
        {
            return from.Before(originalValue);
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Before(this TimeSpan from, DateTimeOffset originalValue)
        {
            return originalValue - from;
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Before(this DateRange from, DateTimeOffset originalValue)
        {
            return originalValue.Add(-from);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset FromNow(this TimeSpan from)
        {
            return from.From(DateTimeOffset.Now);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset FromNow(this DateRange from)
        {
            return from.From(DateTimeOffset.Now);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset From(this TimeSpan from, DateTimeOffset originalValue)
        {
            return originalValue + from;
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset From(this DateRange from, DateTimeOffset originalValue)
        {
            return originalValue.Add(from);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(TimeSpan, DateTimeOffset)"/>
        /// <remarks>
        /// Synonym of <see cref="From(TimeSpan, DateTimeOffset)"/> method.
        /// </remarks>
        public static DateTimeOffset Since(this TimeSpan from, DateTimeOffset originalValue)
        {
            return From(from, originalValue);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(TimeSpan, DateTimeOffset)"/>
        /// <remarks>
        /// Synonym of <see cref="From(TimeSpan, DateTimeOffset)"/> method.
        /// </remarks>
        public static DateTimeOffset Since(this DateRange from, DateTimeOffset originalValue)
        {
            return From(from, originalValue);
        }
    }
}