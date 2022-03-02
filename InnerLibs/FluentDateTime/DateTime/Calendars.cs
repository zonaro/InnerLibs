using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using InnerLibs;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// Static class containing Fluent <see cref="DateTime"/> extension methods.
    /// </summary>
    public static class Calendars
    {
        /// <summary>
        /// Returns a new <see cref="DateTime"/> that adds the value of the specified <see cref="DateRange"/> to the value of this instance.
        /// </summary>
        public static DateTime AddDateRange(this DateTime dateTime, DateRange timeSpan) => dateTime.AddTicks(timeSpan.Ticks);

        /// <summary>
        /// Returns a new <see cref="DateTime"/> that subtracts the value of the specified <see cref="DateRange"/> to the value of this instance.
        /// </summary>
        public static DateTime SubtractDateRange(this DateTime dateTime, DateRange timeSpan) => dateTime.AddTicks(-timeSpan.Ticks);

        /// <summary>
        /// Returns the very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
        /// </summary>
        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);
        }

        /// <summary>
        /// Returns the timezone-adjusted very end of the given day (the last millisecond of the last hour for the given <see cref="DateTime"/>).
        /// </summary>
        public static DateTime EndOfDay(this DateTime date, int timeZoneOffset)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind)
                .AddHours(timeZoneOffset);
        }

        /// <summary>
        /// Returns the last day of the week changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T23:59:59.999
        /// </summary>
        public static DateTime EndOfWeek(this DateTime date)
        {
            return date.LastDayOfWeek().EndOfDay();
        }

        /// <summary>
        /// Returns the last day of the week changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T23:59:59.999
        /// </summary>
        public static DateTime EndOfWeek(this DateTime date, int timeZoneOffset)
        {
            return date.LastDayOfWeek().EndOfDay(timeZoneOffset);
        }

        /// <summary>
        /// Returns the last day of the month changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfMonth(this DateTime date)
        {
            return date.LastDayOfMonth().EndOfDay();
        }

        /// <summary>
        /// Returns the last day of the month changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfMonth(this DateTime date, int timeZoneOffset)
        {
            return date.LastDayOfMonth().EndOfDay(timeZoneOffset);
        }

        /// <summary>
        /// Returns the last day of the quarter changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfQuarter(this DateTime date)
        {
            return date.LastDayOfQuarter().EndOfDay();
        }

        /// <summary>
        /// Returns the last day of the quarter changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfQuarter(this DateTime date, int timeZoneOffset)
        {
            return date.LastDayOfQuarter().EndOfDay(timeZoneOffset);
        }

        /// <summary>
        /// Returns the last day of the year changing the time to the very end of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfYear(this DateTime date)
        {
            return date.LastDayOfYear().EndOfDay();
        }

        /// <summary>
        /// Returns the last day of the year changing the time to the very end of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfYear(this DateTime date, int timeZoneOffset)
        {
            return date.LastDayOfYear().EndOfDay(timeZoneOffset);
        }

        /// <summary>
        /// Returns the Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public static DateTime BeginningOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);
        }

        /// <summary>
        /// Returns the timezone-adjusted Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public static DateTime BeginningOfDay(this DateTime date, int timezoneOffset)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind)
                .AddHours(timezoneOffset);
        }

        /// <summary>
        /// Returns the Start day of the week changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-19T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfWeek(this DateTime date)
        {
            return date.FirstDayOfWeek().BeginningOfDay();
        }

        /// <summary>
        /// Returns the Start day of the week changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-19T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfWeek(this DateTime date, int timezoneOffset)
        {
            return date.FirstDayOfWeek().BeginningOfDay(timezoneOffset);
        }

        /// <summary>
        /// Returns the Start day of the month changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-12-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfMonth(this DateTime date)
        {
            return date.FirstDayOfMonth().BeginningOfDay();
        }

        /// <summary>
        /// Returns the Start day of the month changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-12-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfMonth(this DateTime date, int timezoneOffset)
        {
            return date.FirstDayOfMonth().BeginningOfDay(timezoneOffset);
        }

        /// <summary>
        /// Returns the Start day of the quarter changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfQuarter(this DateTime date)
        {
            return date.FirstDayOfQuarter().BeginningOfDay();
        }

        /// <summary>
        /// Returns the Start day of the quarter changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfQuarter(this DateTime date, int timezoneOffset)
        {
            return date.FirstDayOfQuarter().BeginningOfDay(timezoneOffset);
        }

        /// <summary>
        /// Returns the Start day of the year changing the time to the very start of the day. Eg, 2011-12-24T06:40:20.005 => 2011-01-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfYear(this DateTime date)
        {
            return date.FirstDayOfYear().BeginningOfDay();
        }

        /// <summary>
        /// Returns the Start day of the year changing the time to the very start of the day with timezone-adjusted. Eg, 2011-12-24T06:40:20.005 => 2011-01-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfYear(this DateTime date, int timezoneOffset)
        {
            return date.FirstDayOfYear().BeginningOfDay(timezoneOffset);
        }

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc) in the next calendar year.
        /// If that day does not exist in next year in same month, number of missing days is added to the last day in same month next year.
        /// </summary>
        public static DateTime NextYear(this DateTime start)
        {
            var nextYear = start.Year + 1;
            var numberOfDaysInSameMonthNextYear = DateTime.DaysInMonth(nextYear, start.Month);

            if (numberOfDaysInSameMonthNextYear < start.Day)
            {
                var differenceInDays = start.Day - numberOfDaysInSameMonthNextYear;
                var dateTime = new DateTime(nextYear, start.Month, numberOfDaysInSameMonthNextYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
                return dateTime + differenceInDays.Days();
            }
            return new DateTime(nextYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
        }

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc) in the previous calendar year.
        /// If that day does not exist in previous year in same month, number of missing days is added to the last day in same month previous year.
        /// </summary>
        public static DateTime PreviousYear(this DateTime start)
        {
            var previousYear = start.Year - 1;
            var numberOfDaysInSameMonthPreviousYear = DateTime.DaysInMonth(previousYear, start.Month);

            if (numberOfDaysInSameMonthPreviousYear < start.Day)
            {
                var differenceInDays = start.Day - numberOfDaysInSameMonthPreviousYear;
                var dateTime = new DateTime(previousYear, start.Month, numberOfDaysInSameMonthPreviousYear, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
                return dateTime + differenceInDays.Days();
            }
            return new DateTime(previousYear, start.Month, start.Day, start.Hour, start.Minute, start.Second, start.Millisecond, start.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> increased by 24 hours ie Next Day.
        /// </summary>
        public static DateTime NextDay(this DateTime start)
        {
            return start + 1.Days();
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> decreased by 24h period ie Previous Day.
        /// </summary>
        public static DateTime PreviousDay(this DateTime start)
        {
            return start - 1.Days();
        }

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public static DateTime Next(this DateTime start, DayOfWeek day)
        {
            do
            {
                start = start.NextDay();
            } while (start.DayOfWeek != day);

            return start;
        }

        /// <summary>
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public static DateTime Previous(this DateTime start, DayOfWeek day)
        {
            do
            {
                start = start.PreviousDay();
            } while (start.DayOfWeek != day);

            return start;
        }

        /// <summary>
        /// Increases supplied <see cref="DateTime"/> for 7 days ie returns the Next Week.
        /// </summary>
        public static DateTime WeekAfter(this DateTime start)
        {
            return start + 1.Weeks();
        }

        /// <summary>
        /// Decreases supplied <see cref="DateTime"/> for 7 days ie returns the Previous Week.
        /// </summary>
        public static DateTime WeekEarlier(this DateTime start)
        {
            return start - 1.Weeks();
        }

        /// <summary>
        /// Increases the <see cref="DateTime"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public static DateTime IncreaseTime(this DateTime startDate, TimeSpan toAdd)
        {
            return startDate + toAdd;
        }

        /// <summary>
        /// Decreases the <see cref="DateTime"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public static DateTime DecreaseTime(this DateTime startDate, TimeSpan toSubtract)
        {
            return startDate - toSubtract;
        }

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour part changed to supplied hour parameter.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour and Minute parts changed to supplied hour and minute parameters.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour, int minute)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour, Minute and Second parts changed to supplied hour, minute and second parameters.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour, int minute, int second)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, originalDate.Millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour, Minute, Second and Millisecond parts changed to supplied hour, minute, second and millisecond parameters.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour, int minute, int second, int millisecond)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Hour part.
        /// </summary>
        public static DateTime SetHour(this DateTime originalDate, int hour)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Minute part.
        /// </summary>
        public static DateTime SetMinute(this DateTime originalDate, int minute)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Second part.
        /// </summary>
        public static DateTime SetSecond(this DateTime originalDate, int second)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, second, originalDate.Millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Millisecond part.
        /// </summary>
        public static DateTime SetMillisecond(this DateTime originalDate, int millisecond)
        {
            return new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, originalDate.Second, millisecond, originalDate.Kind);
        }

        /// <summary>
        /// Returns original <see cref="DateTime"/> value with time part set to midnight (alias for <see cref="BeginningOfDay(DateTime)"/> method).
        /// </summary>
        public static DateTime Midnight(this DateTime value)
        {
            return value.BeginningOfDay();
        }

        /// <summary>
        /// Returns original <see cref="DateTime"/> value with time part set to Noon (12:00:00h).
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> find Noon for.</param>
        /// <returns>A <see cref="DateTime"/> value with time part set to Noon (12:00:00h).</returns>
        public static DateTime Noon(this DateTime value)
        {
            return value.SetTime(12, 0, 0, 0);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year part.
        /// </summary>
        public static DateTime SetDate(this DateTime value, int year)
        {
            return new DateTime(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year and Month part.
        /// </summary>
        public static DateTime SetDate(this DateTime value, int year, int month)
        {
            return new DateTime(year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year, Month and Day part.
        /// </summary>
        public static DateTime SetDate(this DateTime value, int year, int month, int day)
        {
            return new DateTime(year, month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year part.
        /// </summary>
        public static DateTime SetYear(this DateTime value, int year)
        {
            return new DateTime(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Month part.
        /// </summary>
        public static DateTime SetMonth(this DateTime value, int month)
        {
            return new DateTime(value.Year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Day part.
        /// </summary>
        public static DateTime SetDay(this DateTime value, int day)
        {
            return new DateTime(value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> is before then current value.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="toCompareWith">Value to compare with.</param>
        /// <returns>
        /// 	<c>true</c> if the specified current is before; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBefore(this DateTime current, DateTime toCompareWith)
        {
            return current < toCompareWith;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is After then current value.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="toCompareWith">Value to compare with.</param>
        /// <returns>
        /// 	<c>true</c> if the specified current is after; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAfter(this DateTime current, DateTime toCompareWith)
        {
            return current > toCompareWith;
        }

        /// <summary>
        /// Returns the given <see cref="DateTime"/> with hour and minutes set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <returns><see cref="DateTime"/> with hour and minute set to given values.</returns>
        public static DateTime At(this DateTime current, int hour, int minute)
        {
            return current.SetTime(hour, minute);
        }

        /// <summary>
        /// Returns the given <see cref="DateTime"/> with hour and minutes and seconds set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <returns><see cref="DateTime"/> with hour and minutes and seconds set to given values.</returns>
        public static DateTime At(this DateTime current, int hour, int minute, int second)
        {
            return current.SetTime(hour, minute, second);
        }

        /// <summary>
        /// Returns the given <see cref="DateTime"/> with hour and minutes and seconds and milliseconds set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <param name="milliseconds">The milliseconds to set time to.</param>
        /// <returns><see cref="DateTime"/> with hour and minutes and seconds set to given values.</returns>
        public static DateTime At(this DateTime current, int hour, int minute, int second, int milliseconds)
        {
            return current.SetTime(hour, minute, second, milliseconds);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the first day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html
        /// </summary>
        /// <param name="current"></param>
        /// <returns>given <see cref="DateTime"/> with the day part set to the first day in the quarter.</returns>
        public static DateTime FirstDayOfQuarter(this DateTime current)
        {
            var currentQuarter = (current.Month - 1) / 3 + 1;
            var firstDay = new DateTime(current.Year, 3 * currentQuarter - 2, 1);

            return current.SetDate(firstDay.Year, firstDay.Month, firstDay.Day);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the first day in that month.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <returns>given <see cref="DateTime"/> with the day part set to the first day in that month.</returns>
        public static DateTime FirstDayOfMonth(this DateTime current)
        {
            return current.SetDay(1);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the last day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html
        /// </summary>
        /// <param name="current"></param>
        /// <returns>given <see cref="DateTime"/> with the day part set to the last day in the quarter.</returns>
        public static DateTime LastDayOfQuarter(this DateTime current)
        {
            var currentQuarter = (current.Month - 1) / 3 + 1;
            var firstDay = current.SetDate(current.Year, 3 * currentQuarter - 2, 1);
            return firstDay.SetMonth(firstDay.Month + 2).LastDayOfMonth();
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the last day in that month.
        /// </summary>
        /// <param name="current">The current DateTime to be changed.</param>
        /// <returns>given <see cref="DateTime"/> with the day part set to the last day in that month.</returns>
        public static DateTime LastDayOfMonth(this DateTime current)
        {
            return current.SetDay(DateTime.DaysInMonth(current.Year, current.Month));
        }

        /// <summary>
        /// Adds the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be added.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime AddBusinessDays(this DateTime current, int days, params DayOfWeek[] DaysOff)
        {
            DaysOff = DaysOff ?? new DayOfWeek[] { };
            if (!DaysOff.Any()) { DaysOff = new DayOfWeek[] { DayOfWeek.Saturday, DayOfWeek.Sunday }; }
            var sign = Math.Sign(days);
            var unsignedDays = Math.Abs(days);
            for (var i = 0; i < unsignedDays; i++)
            {
                do
                {
                    current = current.AddDays(sign);
                } while (current.DayOfWeek.IsAny(DaysOff));
            }
            return current;
        }

        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime SubtractBusinessDays(this DateTime current, int days)
        {
            return AddBusinessDays(current, -days);
        }

        /// <summary>
        /// Determine if a <see cref="DateTime"/> is in the future.
        /// </summary>
        /// <param name="dateTime">The date to be checked.</param>
        /// <returns><c>true</c> if <paramref name="dateTime"/> is in the future; otherwise <c>false</c>.</returns>
        public static bool IsInFuture(this DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }

        /// <summary>
        /// Determine if a <see cref="DateTime"/> is in the past.
        /// </summary>
        /// <param name="dateTime">The date to be checked.</param>
        /// <returns><c>true</c> if <paramref name="dateTime"/> is in the past; otherwise <c>false</c>.</returns>
        public static bool IsInPast(this DateTime dateTime)
        {
            return dateTime < DateTime.Now;
        }

        /// <summary>
        /// Rounds <paramref name="dateTime"/> to the nearest <see cref="RoundTo"/>.
        /// </summary>
        /// <returns>The rounded <see cref="DateTime"/>.</returns>
        public static DateTime Round(this DateTime dateTime, RoundTo rt)
        {
            DateTime rounded;

            switch (rt)
            {
                case RoundTo.Second:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
                        if (dateTime.Millisecond >= 500)
                        {
                            rounded = rounded.AddSeconds(1);
                        }
                        break;
                    }
                case RoundTo.Minute:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, dateTime.Kind);
                        if (dateTime.Second >= 30)
                        {
                            rounded = rounded.AddMinutes(1);
                        }
                        break;
                    }
                case RoundTo.Hour:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, dateTime.Kind);
                        if (dateTime.Minute >= 30)
                        {
                            rounded = rounded.AddHours(1);
                        }
                        break;
                    }
                case RoundTo.Day:
                    {
                        rounded = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, dateTime.Kind);
                        if (dateTime.Hour >= 12)
                        {
                            rounded = rounded.AddDays(1);
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException("rt");
                    }
            }

            return rounded;
        }

        /// <summary>
        /// Returns a DateTime adjusted to the beginning of the week.
        /// </summary>
        /// <param name="dateTime">The DateTime to adjust</param>
        /// <returns>A DateTime instance adjusted to the beginning of the current week</returns>
        /// <remarks>the beginning of the week is controlled by the current Culture</remarks>
        public static DateTime FirstDayOfWeek(this DateTime dateTime, CultureInfo currentCulture = null)
        {
            currentCulture = currentCulture ?? CultureInfo.CurrentCulture;
            var firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
            var offset = dateTime.DayOfWeek - firstDayOfWeek < 0 ? 7 : 0;
            var numberOfDaysSinceBeginningOfTheWeek = dateTime.DayOfWeek + offset - firstDayOfWeek;

            return dateTime.AddDays(-numberOfDaysSinceBeginningOfTheWeek);
        }

        /// <summary>
        /// Returns the first day of the year keeping the time component intact. Eg, 2011-02-04T06:40:20.005 => 2011-01-01T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime FirstDayOfYear(this DateTime current)
        {
            return current.SetDate(current.Year, 1, 1);
        }

        /// <summary>
        /// Returns the last day of the week keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-25T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime current)
        {
            return current.FirstDayOfWeek().AddDays(6);
        }

        /// <summary>
        /// Returns the last day of the year keeping the time component intact. Eg, 2011-12-24T06:40:20.005 => 2011-12-31T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime LastDayOfYear(this DateTime current)
        {
            return current.SetDate(current.Year, 12, 31);
        }

        /// <summary>
        /// Returns the previous month keeping the time component intact. Eg, 2010-01-20T06:40:20.005 => 2009-12-20T06:40:20.005
        /// If the previous month doesn't have that many days the last day of the previous month is used. Eg, 2009-03-31T06:40:20.005 => 2009-02-28T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime PreviousMonth(this DateTime current)
        {
            var year = current.Month == 1 ? current.Year - 1 : current.Year;

            var month = current.Month == 1 ? 12 : current.Month - 1;

            var firstDayOfPreviousMonth = current.SetDate(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfPreviousMonth.LastDayOfMonth().Day;

            var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

            return firstDayOfPreviousMonth.SetDay(day);
        }

        /// <summary>
        /// Returns the next month keeping the time component intact. Eg, 2012-12-05T06:40:20.005 => 2013-01-05T06:40:20.005
        /// If the next month doesn't have that many days the last day of the next month is used. Eg, 2013-01-31T06:40:20.005 => 2013-02-28T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime NextMonth(this DateTime current)
        {
            var year = current.Month == 12 ? current.Year + 1 : current.Year;

            var month = current.Month == 12 ? 1 : current.Month + 1;

            var firstDayOfNextMonth = current.SetDate(year, month, 1);

            var lastDayOfPreviousMonth = firstDayOfNextMonth.LastDayOfMonth().Day;

            var day = current.Day > lastDayOfPreviousMonth ? lastDayOfPreviousMonth : current.Day;

            return firstDayOfNextMonth.SetDay(day);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same day (day + month + year) then current
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="date">Value to compare with</param>
        /// <returns>
        /// 	<c>true</c> if the specified date is exactly the same year then current; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameDay(this DateTime current, DateTime date)
        {
            return current.Date == date.Date;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same month (month + year) then current. Eg, 2015-12-01 and 2014-12-01 => False
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="date">Value to compare with</param>
        /// <returns>
        /// 	<c>true</c> if the specified date is exactly the same month and year then current; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameMonth(this DateTime current, DateTime date)
        {
            return current.Month == date.Month && current.Year == date.Year;
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same year then current. Eg, 2015-12-01 and 2015-01-01 => True
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="date">Value to compare with</param>
        /// <returns>
        /// 	<c>true</c> if the specified date is exactly the same date then current; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameYear(this DateTime current, DateTime date)
        {
            return current.Year == date.Year;
        }

        public static DateTime ClearMilliseconds(this DateTime Date)
        {
            return Date.AddTicks(-(Date.Ticks % TimeSpan.TicksPerSecond));
        }

        public static DateRange CreateDateRange<T>(this IEnumerable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateTime? StartDate = default, DateTime? EndDate = default) where T : class
        {
            var Period = new DateRange
            {
                ForceFirstAndLastMoments = true
            };

            StartDate = StartDate ?? List.Min(PropertyExpression.Compile());
            EndDate = EndDate ?? List.Max(PropertyExpression.Compile());

            if (StartDate.HasValue)
            {
                Period.StartDate = StartDate.Value;
            }

            if (EndDate.HasValue)
            {
                Period.StartDate = EndDate.Value;
            }

            return Period;
        }

        /// <summary>
        /// Cria um <see cref="DateRange"/> a partir de 2 datas e/ou uma propriedade do tipo <see cref="DateTime" /> de <paramref name="T" /> como filtro de uma lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns>Um DateRange a partir das datas Iniciais e finais especificadas ou um daterange a partir da menor e maior datas de uma lista</returns>
        //public static DateRange CreateDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateTime? StartDate = default, DateTime? EndDate = default) where T : class
        //{
        //    var Period = new DateRange
        //    {
        //        ForceFirstAndLastMoments = true
        //    };

        //    StartDate = StartDate ?? List.Min(PropertyExpression);
        //    EndDate = EndDate ?? List.Max(PropertyExpression);

        //    if (StartDate.HasValue)
        //    {
        //        Period.StartDate = StartDate.Value;
        //    }

        //    if (EndDate.HasValue)
        //    {
        //        Period.StartDate = EndDate.Value;
        //    }

        //    return Period;
        //}

        /// <summary>
        /// Pega o numero da semana do mês a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetWeekNumberOfMonth(this DateTime DateAndTime)
        {
            return DateAndTime.GetWeekInfoOfMonth().Week;
        }

        /// <summary>
        /// Pega o numero da semana, do mês e ano pertencente
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static WeekInfo GetWeekInfoOfMonth(this DateTime DateAndTime)
        {
            return new WeekInfo(DateAndTime);
        }

        /// <summary>
        /// Pega o numero do Bimestre a partir de uma data
        /// </summary>
        /// <param name="DateAndtime"></param>
        /// <returns></returns>
        public static int GetDoubleMonthOfYear(this DateTime DateAndtime)
        {
            if (DateAndtime.Month <= 2)
            {
                return 1;
            }
            else if (DateAndtime.Month <= 4)
            {
                return 2;
            }
            else if (DateAndtime.Month <= 6)
            {
                return 3;
            }
            else if (DateAndtime.Month <= 8)
            {
                return 4;
            }
            else if (DateAndtime.Month <= 10)
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }

        /// <summary>
        /// Pega o numero do trimestre a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetQuarterOfYear(this DateTime DateAndTime)
        {
            if (DateAndTime.Month <= 3)
            {
                return 1;
            }
            else if (DateAndTime.Month <= 6)
            {
                return 2;
            }
            else if (DateAndTime.Month <= 9)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        /// <summary>
        /// Pega o numero do semestre a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetHalfOfYear(this DateTime DateAndTime)
        {
            if (DateAndTime.Month <= 6)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// Retorna a idade
        /// </summary>
        /// <param name="BirthDate"></param>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime BirthDate, DateTime? FromDate = default)
        {
            FromDate = FromDate ?? DateTime.Now;
            int age = FromDate.Value.Year - BirthDate.Year;
            if (BirthDate > DateTime.Today.AddYears(-age))
                age -= 1;
            return age;
        }

        /// <summary>
        /// Converte um <see cref="Date"/> para um timezone Especifico
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneUtc(this DateTime Date, TimeZoneInfo TimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(Date, TimeZone);
        }

        /// <summary>
        /// Converte um <see cref="Date"/> para um timezone Especifico
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="TimeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZone(this DateTime Date, string TimeZoneId)
        {
            return TimeZoneInfo.ConvertTime(Date, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));
        }

        /// <summary>
        /// Converte uma string em datetime a partir de um formato especifico
        /// </summary>
        /// <param name="DateString">String original</param>
        /// <param name="Format"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static DateTime ConvertDateString(this string DateString, string Format, CultureInfo Culture = null)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            return DateTime.ParseExact(DateString, Format, Culture);
        }

        /// <summary>
        /// Converte uma string de data para outra string de data com formato diferente
        /// </summary>
        /// <param name="DateString">String original</param>
        /// <param name="InputFormat"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string ChangeFormat(this string DateString, string InputFormat, string OutputFormat, CultureInfo Culture = null)
        {
            return DateString.ConvertDateString(InputFormat, Culture).ToString(OutputFormat);
        }

        /// <summary>
        /// Pula para a data inicial da proxima quinzena
        /// </summary>
        /// <param name="FromDate">Data de partida</param>
        /// <param name="Num">Numero de quinzenas para adiantar</param>
        /// <returns></returns>
        public static DateTime NextFortnight(this DateTime FromDate, int Num = 1)
        {
            return new FortnightGroup(FromDate, Num).Last().Period.StartDate;
        }

        /// <summary>
        /// Retorna o primeiro dia da semana da data especificada
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(this DateTime Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday)
        {
            while (Date.DayOfWeek > FirstDayOfWeek)
                Date = Date.AddDays(-1);
            return Date;
        }

        /// <summary>
        /// Retorna o ultimo dia da semana da data especificada
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfWeek(this DateTime Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday)
        {
            return Date.GetFirstDayOfWeek(FirstDayOfWeek).AddDays(6d);
        }

        /// <summary>
        /// Retorna um DateRange equivalente a semana de uma data especifica
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é domingo)</param>
        /// <returns></returns>
        public static DateRange GetWeek(this DateTime Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday)
        {
            return new DateRange(Date.GetFirstDayOfWeek(FirstDayOfWeek), Date.GetLastDayOfWeek(FirstDayOfWeek));
        }

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, DateTime.DaysInMonth(Date.Year, Date.Month), Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="MonthNumber">Data</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this int MonthNumber, int? Year = default)
        {
            Year = (Year ?? DateTime.Now.Month).SetMinValue(DateTime.MinValue.Month);
            return new DateTime((int)Year, MonthNumber, 1).GetLastDayOfMonth();
        }

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="MonthNumber">Data</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this int MonthNumber, int? Year = default)
        {
            Year = (Year ?? DateTime.Now.Month).SetMinValue(DateTime.MinValue.Month);
            return new DateTime((int)Year, MonthNumber, 1);
        }

        /// <summary>
        /// Retorna a primeira data do mes a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, 1, Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna a primeira data da quinzena a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfFortnight(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day <= 15 ? 1 : 16, Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna a ultima data da quinzena a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfFortnight(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day <= 15 ? 15 : Date.GetLastDayOfMonth().Day, Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna o primeiro dia de um ano especifico de outra data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfYear(this DateTime Date)
        {
            return new DateTime(Date.Year, 1, 1).Date;
        }

        /// <summary>
        /// Retorna o ultimo dia de um ano especifico de outra data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfYear(this DateTime Date)
        {
            return new DateTime(Date.Year, 12, 31).Date;
        }

        /// <summary>
        /// Retorna o primeiro dia de um semestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfHalf(this DateTime Date)
        {
            if (Date.GetHalfOfYear() == 1)
            {
                return Date.GetFirstDayOfYear();
            }
            else
            {
                return new DateTime(Date.Year, 7, 1).Date;
            }
        }

        /// <summary>
        /// Retorna o ultimo dia de um semestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfHalf(this DateTime Date)
        {
            if (Date.GetHalfOfYear() == 1)
            {
                return new DateTime(Date.Year, 6, 1).GetLastDayOfMonth();
            }
            else
            {
                return Date.GetLastDayOfYear();
            }
        }

        /// <summary>
        /// Retorna o ultimo dia de um trimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfQuarter(this DateTime Date)
        {
            if (Date.GetQuarterOfYear() == 1)
                return new DateTime(Date.Year, 3, 1).GetLastDayOfMonth();
            if (Date.GetQuarterOfYear() == 2)
                return new DateTime(Date.Year, 6, 1).GetLastDayOfMonth();
            if (Date.GetQuarterOfYear() == 3)
                return new DateTime(Date.Year, 9, 1).GetLastDayOfMonth();
            return new DateTime(Date.Year, 12, 1).GetLastDayOfMonth();
        }

        /// <summary>
        /// Retorna o ultimo dia de um trimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfQuarter(this DateTime Date)
        {
            if (Date.GetQuarterOfYear() == 1)
                return new DateTime(Date.Year, 1, 1).Date;
            if (Date.GetQuarterOfYear() == 2)
                return new DateTime(Date.Year, 4, 1).Date;
            if (Date.GetQuarterOfYear() == 3)
                return new DateTime(Date.Year, 7, 1).Date;
            return new DateTime(Date.Year, 10, 1).Date;
        }

        /// <summary>
        /// Retorna o ultimo dia de um bimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfBimester(this DateTime Date)
        {
            if (Date.GetDoubleMonthOfYear() == 1)
                return new DateTime(Date.Year, 2, 1).GetLastDayOfMonth();
            else if (Date.GetDoubleMonthOfYear() == 2)
                return new DateTime(Date.Year, 4, 1).GetLastDayOfMonth();
            else if (Date.GetDoubleMonthOfYear() == 3)
                return new DateTime(Date.Year, 6, 1).GetLastDayOfMonth();
            else if (Date.GetDoubleMonthOfYear() == 4)
                return new DateTime(Date.Year, 8, 1).GetLastDayOfMonth();
            else if (Date.GetDoubleMonthOfYear() == 5)
                return new DateTime(Date.Year, 10, 1).GetLastDayOfMonth();
            else
                return new DateTime(Date.Year, 12, 1).GetLastDayOfMonth();
        }

        /// <summary>
        /// Retorna o primeiro dia de um bimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfBimester(this DateTime Date)
        {
            if (Date.GetDoubleMonthOfYear() == 1)
                return new DateTime(Date.Year, 1, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 2)
                return new DateTime(Date.Year, 3, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 3)
                return new DateTime(Date.Year, 5, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 4)
                return new DateTime(Date.Year, 7, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 5)
                return new DateTime(Date.Year, 9, 1).Date;
            else
                return new DateTime(Date.Year, 11, 1).Date;
        }

        /// <summary>
        /// Retorna o numero da semana relativa ao ano
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <param name="FirstDayOfWeek"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime Date, CultureInfo Culture = null, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday, CalendarWeekRule Rule = CalendarWeekRule.FirstFourDayWeek)
        {
            return (Culture ?? CultureInfo.InvariantCulture).Calendar.GetWeekOfYear(Date, Rule, FirstDayOfWeek);
        }

        /// <summary>
        /// Retorna o ultimo momento do dia
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static DateTime GetLastMoment(this DateTime Date) => Date.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

        /// <summary>
        /// Verifica se uma data é do mesmo mês e ano que outra data
        /// </summary>
        /// <param name="[Date]">Primeira data</param>
        /// <param name="AnotherDate">Segunda data</param>
        /// <returns></returns>
        public static bool IsSameMonthAndYear(this DateTime Date, DateTime AnotherDate) => Date.IsBetween(AnotherDate.GetFirstDayOfMonth().Date, AnotherDate.GetLastDayOfMonth().GetLastMoment());

        /// <summary>
        /// Verifica se a Data de hoje é um aniversário
        /// </summary>
        /// <param name="BirthDate">  Data de nascimento</param>
        /// <returns></returns>
        public static bool IsAnniversary(this DateTime BirthDate, DateTime? CompareWith = default)
        {
            CompareWith = CompareWith ?? DateTime.Today;
            return (BirthDate.Day == CompareWith.Value.Day) && (BirthDate.Month == CompareWith.Value.Month);
        }

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetLongMonthName(this DateTime Date, CultureInfo Culture = null) => Date.ToString("MMMM", Culture ?? CultureInfo.CurrentCulture);

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetShortMonthName(this DateTime Date, CultureInfo Culture = null) => Date.ToString("MM", Culture ?? CultureInfo.CurrentCulture);

        /// <summary>
        /// COnverte um datetime para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this DateTime Date) => $"{Date.Year}-{Date.Month}-{Date.Day} {Date.Hour}:{Date.Minute}:{Date.Second}.{Date.Millisecond}";

        /// <summary>
        /// Converte uma string dd/mm/aaaa hh:mm:ss.llll para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this string Date, string FromCulture = "pt-BR") => Convert.ToDateTime(Date, new CultureInfo(FromCulture, false).DateTimeFormat).ToSQLDateString();

        /// <summary>
        /// Retorna uma <see cref="DateRange"/> com a diferença entre 2 Datas
        /// </summary>
        /// <param name="InitialDate"></param>
        /// <param name="SecondDate"> </param>
        /// <returns></returns>
        public static DateRange GetDifference(this DateTime InitialDate, DateTime SecondDate) => new DateRange(InitialDate, SecondDate);

        /// <summary>
        /// Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate
        /// sempre seja uma data menor que a EndDate, prevenindo que o calculo entre 2 datas resulte em um
        /// <see cref="TimeSpan"/> negativo
        /// </summary>
        /// <param name="StartDate">Data Inicial</param>
        /// <param name="EndDate">  Data Final</param>
        public static void FixDateOrder(ref DateTime StartDate, ref DateTime EndDate) => ClassTools.FixOrder(ref StartDate, ref EndDate);

        /// <summary>
        /// Verifica se uma data se encontra entre 2 datas
        /// </summary>
        /// <param name="MidDate">   Data</param>
        /// <param name="StartDate"> Data Inicial</param>
        /// <param name="EndDate">   Data final</param>
        /// <param name="IgnoreTime">Indica se o tempo deve ser ignorado na comparação</param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime MidDate, DateTime StartDate, DateTime EndDate, bool IgnoreTime = false)
        {
            FixDateOrder(ref StartDate, ref EndDate);

            if (IgnoreTime)
            {
                if (StartDate.Date == EndDate.Date) return MidDate.Date == StartDate.Date;
                else return StartDate.Date <= MidDate.Date && MidDate.Date <= EndDate.Date;
            }
            else
            {
                if (StartDate == EndDate) return MidDate == StartDate;
                else return StartDate <= MidDate && MidDate <= EndDate;
            }
        }

        /// <summary>
        /// Retorna as datas entre um periodo
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="DaysOfWeek"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetDaysBetween(this DateTime StartDate, DateTime EndDate, params DayOfWeek[] DaysOfWeek)
        {
            var l = new List<DateTime>() { StartDate.Date };
            DaysOfWeek = DaysOfWeek ?? Array.Empty<DayOfWeek>();
            if (DaysOfWeek.Length == 0)
            {
                DaysOfWeek = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            }

            var curdate = StartDate.Date;
            while (curdate.Date < EndDate.Date)
            {
                curdate = curdate.AddDays(1d);
                l.Add(curdate);
            }

            l.Add(EndDate.Date);
            return l.Distinct().Where(x => x.DayOfWeek.IsIn(DaysOfWeek));
        }

        /// <summary>
        /// Remove o tempo de todas as datas de uma lista e retorna uma nova lista
        /// </summary>
        /// <param name="List">Lista que será alterada</param>
        public static IEnumerable<DateTime> ClearTime(this IEnumerable<DateTime> List) => List.Select(x => x.Date);

        /// <summary>
        /// Retorna uma String baseado no numero do Mês Ex.: 1 -&gt; Janeiro
        /// </summary>
        /// <param name="MonthNumber">Numero do Mês</param>
        /// <returns>String com nome do Mês</returns>

        public static string ToLongMonthName(this int MonthNumber, CultureInfo Culture = null) => new DateTime(DateTime.Now.Year, MonthNumber, 1).GetLongMonthName(Culture);

        /// <summary>
        /// Retorna uma String curta baseado no numero do Mês Ex.: 1 -&gt; Jan
        /// </summary>
        /// <param name="MonthNumber">Numero do Mês</param>
        /// <returns>String com nome curto do Mês</returns>

        public static string ToShortMonthName(this int MonthNumber, CultureInfo Culture = null) => new DateTime(DateTime.Now.Year, MonthNumber, 1).GetShortMonthName(Culture);

        /// <summary>
        /// Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Segunda-Feira
        /// </summary>
        /// <param name="DayNumber">Numero do Dia</param>
        /// <returns>String com nome do Dia</returns>

        public static string ToLongDayOfWeekName(this int DayNumber) => DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)DayNumber);

        /// <summary>
        /// Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Seg
        /// </summary>
        /// <param name="DayNumber">Numero do Dia</param>
        /// <returns>String com nome curto do Dia</returns>

        public static string ToShortDayOfWeekName(this int DayNumber) => DayNumber.ToLongDayOfWeekName().GetFirstChars(3);

        /// <summary>
        /// Retorna a data de amanhã
        /// </summary>
        /// <returns>Data de amanhã</returns>

        public static DateTime Tomorrow => DateTime.Now.AddDays(1d);

        /// <summary>
        /// Retorna a data de ontem
        /// </summary>
        /// <returns>Data de ontem</returns>

        public static DateTime Yesterday => DateTime.Now.AddDays(-1);

        public static DateTime BrazilianNow => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public static DateTime BrazilianTomorrow => BrazilianNow.AddDays(1d);

        public static DateTime BrazilianYesterday => BrazilianNow.AddDays(-1);

        /// <summary>
        /// Retorna o ultimo domingo
        /// </summary>
        /// <returns></returns>
        public static DateTime LastSunday(DateTime? FromDate = null) => LastDay(DayOfWeek.Sunday, FromDate);

        /// <summary>
        /// Retorna o proximo domingo
        /// </summary>
        /// <returns></returns>
        public static DateTime NextSunday(DateTime? FromDate = default) => NextDay(DayOfWeek.Sunday, FromDate);

        /// <summary>
        /// Retorna o ultimo dia referente a um dia da semana
        /// </summary>
        /// <param name="DayOfWeek"></param>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public static DateTime LastDay(DayOfWeek DayOfWeek, DateTime? FromDate = default)
        {
            FromDate = FromDate ?? DateTime.Now;
            while (FromDate.Value.DayOfWeek != DayOfWeek)
                FromDate = FromDate.Value.AddDays(-1);
            return (DateTime)FromDate;
        }

        /// <summary>
        /// Retorna o proximo dia referente a um dia da semana
        /// </summary>
        /// <param name="DayOfWeek"></param>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public static DateTime NextDay(DayOfWeek DayOfWeek, DateTime? FromDate = default)
        {
            FromDate = FromDate ?? DateTime.Now;
            while (FromDate.Value.DayOfWeek != DayOfWeek)
                FromDate = FromDate.Value.AddDays(1d);
            return (DateTime)FromDate;
        }

        /// <summary>
        /// Verifica se o dia se encontra no fim de semana
        /// </summary>
        /// <param name="YourDate">Uma data qualquer</param>
        /// <returns>TRUE se for sabado ou domingo, caso contrario FALSE</returns>

        public static bool IsWeekend(this DateTime YourDate) => YourDate.DayOfWeek == DayOfWeek.Sunday | YourDate.DayOfWeek == DayOfWeek.Saturday;

        private static string ToGreetingFarewell(this DateTime Time, string Language = "pt", bool Farewell = false)
        {
            string bomdia = "Bom dia";
            string boatarde = "Boa tarde";
            string boanoite = "Boa noite";
            string boanoite_despedida = boanoite;
            string seDespedidaManha = "tenha um ótimo dia";
            string seDespedidaTarde = "tenha uma ótima tarde";
            switch (Language.ToLower() ?? "")
            {
                case "en":
                case "eng":
                case "ingles":
                case "english":
                case "inglés":
                    {
                        bomdia = "Good morning";
                        boatarde = "Good afternoon";
                        boanoite = "Good evening";
                        boanoite_despedida = "Good night";
                        seDespedidaManha = "have a nice day";
                        seDespedidaTarde = "have a great afternoon";
                        break;
                    }

                case "es":
                case "esp":
                case "espanhol":
                case "espanol":
                case "español":
                case "spanish":
                    {
                        bomdia = "Buenos días";
                        boatarde = "Buenas tardes";
                        boanoite = "Buenas noches";
                        boanoite_despedida = boanoite;
                        seDespedidaManha = "que tengas un buen día";
                        seDespedidaTarde = "que tengas una buena tarde";
                        break;
                    }
            }

            if (Time.Hour < 12)
            {
                return Farewell ? seDespedidaManha : bomdia;
            }
            else if (Time.Hour >= 12 && Time.Hour < 18)
            {
                return Farewell ? seDespedidaTarde : boatarde;
            }
            else
            {
                return Farewell ? boanoite_despedida : boanoite;
            }
        }

        /// <summary>
        /// Transforma um DateTime em uma despedida (Bom dia, Boa tarde, Boa noite)
        /// </summary>
        /// <param name="Time">    Horario</param>
        /// <param name="Language">Idioma da saudação (pt, en, es)</param>
        /// <returns>Uma string com a despedida</returns>
        public static string ToFarewell(this DateTime Time, string Language = "pt") => Time.ToGreetingFarewell(Language, true);

        /// <summary>
        /// Transforma um DateTime em uma saudação (Bom dia, Boa tarde, Boa noite)
        /// </summary>
        /// <param name="Time">    Horario</param>
        /// <param name="Language">Idioma da saudação (pt, en, es)</param>
        /// <returns>Uma string com a despedida</returns>
        public static string ToGreeting(this DateTime Time, string Language = "pt") => Time.ToGreetingFarewell(Language, false);

        /// <summary>
        /// Returna uma lista dupla com os meses
        /// </summary>
        /// <param name="ValueType">Apresentação dos meses no valor</param>
        /// <param name="TextType">Apresentação dos meses no texto</param>

        public static List<KeyValuePair<string, string>> GetMonthList(CalendarFormat TextType = CalendarFormat.LongName, CalendarFormat ValueType = CalendarFormat.Number)
        {
            List<KeyValuePair<string, string>> MonthsRet = new List<KeyValuePair<string, string>>();
            for (int i = 1; i <= 12; i++)
            {
                string key = "";
                string value = "";
                switch (TextType)
                {
                    case CalendarFormat.LongName:
                        {
                            key = i.ToLongMonthName();
                            break;
                        }

                    case CalendarFormat.ShortName:
                        {
                            key = i.ToShortMonthName();
                            break;
                        }

                    default:
                        {
                            key = i.ToString();
                            break;
                        }
                }

                switch (ValueType)
                {
                    case CalendarFormat.LongName:
                        {
                            value = i.ToLongMonthName();
                            break;
                        }

                    case CalendarFormat.ShortName:
                        {
                            value = i.ToShortMonthName();
                            break;
                        }

                    default:
                        {
                            value = i.ToString();
                            break;
                        }
                }

                MonthsRet.Add(new KeyValuePair<string, string>(key, value));
            }

            return MonthsRet;
        }

        /// <summary>
        /// Returna uma lista dupla com os meses
        /// </summary>
        /// <param name="ValueType">Apresentação dos meses no valor</param>
        /// <param name="TextType">Apresentação dos meses no texto</param>

        public static List<KeyValuePair<string, string>> GetWeekDays(CalendarFormat TextType = CalendarFormat.LongName, CalendarFormat ValueType = CalendarFormat.Number)
        {
            List<KeyValuePair<string, string>> WeekDaysRet = default;
            WeekDaysRet = new List<KeyValuePair<string, string>>();
            for (int i = 1; i <= 7; i++)
            {
                string key = "";
                string value = "";
                switch (TextType)
                {
                    case CalendarFormat.LongName:
                        {
                            key = i.ToLongDayOfWeekName();
                            break;
                        }

                    case CalendarFormat.ShortName:
                        {
                            key = i.ToShortDayOfWeekName();
                            break;
                        }

                    default:
                        {
                            key = i.ToString();
                            break;
                        }
                }

                switch (ValueType)
                {
                    case CalendarFormat.LongName:
                        {
                            value = i.ToLongDayOfWeekName();
                            break;
                        }

                    case CalendarFormat.ShortName:
                        {
                            value = i.ToShortDayOfWeekName();
                            break;
                        }

                    default:
                        {
                            value = i.ToString();
                            break;
                        }
                }

                WeekDaysRet.Add(new KeyValuePair<string, string>(key, value));
            }

            return WeekDaysRet;
        }

        /// <summary>
        /// Tipo de Apresentação dos Meses/Dias da Semana/Estado
        /// </summary>

        /// <summary>
        /// Calcula a porcentagem de diferenca entre duas datas de acordo com a data inicial especificada
        /// </summary>
        /// <param name="MidDate">  Data do meio a ser verificada (normalmente Now)</param>
        /// <param name="StartDate">Data Inicial</param>
        /// <param name="EndDate">  Data Final</param>
        /// <returns></returns>
        public static decimal CalculateTimelinePercent(this DateTime MidDate, DateTime StartDate, DateTime EndDate)
        {
            FixDateOrder(ref StartDate, ref EndDate);
            if (MidDate < StartDate)
                return 0m;
            if (MidDate > EndDate)
                return 100m;
            return (decimal)((MidDate - StartDate).Ticks * 100L / (double)(EndDate - StartDate).Ticks);
        }
    }

    public enum CalendarFormat
    {
        /// <summary>
        /// Numerico
        /// </summary>
        Number = 2,

        /// <summary>
        /// Abreviado
        /// </summary>

        ShortName = 1,
        /// <summary>
        /// Completo
        /// </summary>

        LongName = 0
    }

    /// <summary>
    /// Elemento do calendário
    /// </summary>
    public enum CalendarType
    {
        Weekdays,
        Months
    }
}