using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// Static class containing <see cref="DateTime"/> extension methods.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region Public Properties

        public static DateTime BrazilianNow => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public static DateTime BrazilianTomorrow => BrazilianNow.AddDays(1);

        public static DateTime BrazilianYesterday => BrazilianNow.AddDays(-1);

        public static DateTime Tomorrow => DateTime.Now.AddDays(1);

        public static DateTime Yesterday => DateTime.Now.AddDays(-1);

        #endregion Public Properties

        #region Public Methods

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
                } while (current.DayOfWeek.IsIn(DaysOff));
            }
            return current;
        }

        /// <summary>
        /// Returns a new <see cref="DateTime"/> that adds the value of the specified <see
        /// cref="DateRange"/> to the value of this instance.
        /// </summary>
        public static DateTime AddDateRange(this DateTime dateTime, DateRange timeSpan) => dateTime.AddTicks(timeSpan.Ticks);

        /// <summary>
        /// Adds the given <see cref="DateRange"/> from a <see cref="TimeSpan"/> and returns
        /// resulting <see cref="TimeSpan"/>.
        /// </summary>
        public static TimeSpan AddDateRange(this TimeSpan timeSpan, DateRange DateRange) => timeSpan + DateRange.TimeSpan;

        /// <summary>
        /// Adciona um intervalo a um <see cref="DateTime"/>
        /// </summary>
        /// <param name="Datetime"></param>
        /// <param name="Interval"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        /// <remarks>
        /// <see cref="DateRangeInterval.LessAccurate"/> is equivalent of <see
        /// cref="DateRangeInterval.Milliseconds"/> on this scenario. When <paramref
        /// name="Interval"/> is <see cref="DateRangeInterval.Months"/> or <see
        /// cref="DateRangeInterval.Years"/>, <paramref name="Total"/> is rounded to next <see
        /// cref="int"/> value
        /// </remarks>
        public static DateTime AddInterval(this DateTime Datetime, DateRangeInterval Interval, double Total)
        {
            switch (Interval)
            {
                case DateRangeInterval.Years: return Datetime.AddYears(Total.RoundInt());
                case DateRangeInterval.Months: return Datetime.AddMonths(Total.RoundInt());
                case DateRangeInterval.Weeks: return Datetime.AddDays(Total * 7d);
                case DateRangeInterval.Days: return Datetime.AddDays(Total);
                case DateRangeInterval.Hours: return Datetime.AddHours(Total);
                case DateRangeInterval.Minutes: return Datetime.AddMinutes(Total);
                case DateRangeInterval.Seconds: return Datetime.AddSeconds(Total);
                case DateRangeInterval.Milliseconds:
                case DateRangeInterval.LessAccurate:
                default: return Datetime.AddMilliseconds(Total);
            }
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from current date ( <see cref="DateTime.Now"/>)
        /// and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTime Ago(this TimeSpan from) => from.Before(DateTime.Now);

        /// <summary>
        /// Subtracts given <see cref="DateRange"/> from current date ( <see cref="DateTime.Now"/>)
        /// and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTime Ago(this DateRange from) => from.Before(DateTime.Now);

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTime Ago(this TimeSpan from, DateTime originalValue) => from.Before(originalValue);

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTime Ago(this DateRange from, DateTime originalValue) => from.Before(originalValue);

        /// <summary>
        /// Returns the given <see cref="DateTime"/> with hour and minutes set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <returns><see cref="DateTime"/> with hour and minute set to given values.</returns>
        public static DateTime At(this DateTime current, int hour, int minute) => current.SetTime(hour, minute);

        /// <summary>
        /// Returns the given <see cref="DateTime"/> with hour and minutes and seconds set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <returns><see cref="DateTime"/> with hour and minutes and seconds set to given values.</returns>
        public static DateTime At(this DateTime current, int hour, int minute, int second) => current.SetTime(hour, minute, second);

        /// <summary>
        /// Returns the given <see cref="DateTime"/> with hour and minutes and seconds and
        /// milliseconds set At given values.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <param name="hour">The hour to set time to.</param>
        /// <param name="minute">The minute to set time to.</param>
        /// <param name="second">The second to set time to.</param>
        /// <param name="milliseconds">The milliseconds to set time to.</param>
        /// <returns><see cref="DateTime"/> with hour and minutes and seconds set to given values.</returns>
        public static DateTime At(this DateTime current, int hour, int minute, int second, int milliseconds) => current.SetTime(hour, minute, second, milliseconds);

        public static DateTime At(this DateTime current, TimeSpan Time) => current.SetTime(Time);

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTime Before(this TimeSpan from, DateTime originalValue) => originalValue - from;

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTime Before(this DateRange from, DateTime originalValue) => originalValue.Add(-from);

        /// <summary>
        /// Returns the Start day of the bimester changing the time to the very start of the day.
        /// Eg, 2011-12-24T06:40:20.005 =&gt; 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfBimester(this DateTime date) => date.FirstDayOfBimester().BeginningOfDay();

        /// <summary>
        /// Returns the timezone-adjusted Start day of the bimester changing the time to the very
        /// start of the day. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfBimester(this DateTime date, int timezoneOffset) => date.FirstDayOfBimester().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start of the given day (the first millisecond of the given <see cref="DateTime"/>).
        /// </summary>
        public static DateTime BeginningOfDay(this DateTime date) => new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind);

        /// <summary>
        /// Returns the timezone-adjusted Start of the given day (the first millisecond of the given
        /// <see cref="DateTime"/>).
        /// </summary>
        public static DateTime BeginningOfDay(this DateTime date, int timezoneOffset) => new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0, date.Kind).AddHours(timezoneOffset);

        public static DateTime BeginningOfFortnight(this DateTime date) => date.FirstDayOfFortnight().Date;

        /// <summary>
        /// Returns the Start day of the month changing the time to the very start of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfMonth(this DateTime date) => date.FirstDayOfMonth().BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the month changing the time to the very start of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfMonth(this DateTime date, int timezoneOffset) => date.FirstDayOfMonth().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the quarter changing the time to the very start of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfQuarter(this DateTime date) => date.FirstDayOfQuarter().BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the quarter changing the time to the very start of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfQuarter(this DateTime date, int timezoneOffset) => date.FirstDayOfQuarter().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the timezone-adjusted Start day of the semester changing the time to the very
        /// start of the day. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfSemester(this DateTime date, int timezoneOffset) => date.FirstDayOfSemester().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the semester changing the time to the very start of the day.
        /// Eg, 2011-12-24T06:40:20.005 =&gt; 2011-10-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfSemester(this DateTime date) => date.FirstDayOfSemester().BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the week changing the time to the very start of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-19T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfWeek(this DateTime date, CultureInfo culture = null) => date.FirstDayOfWeek(culture).BeginningOfDay();

        public static DateTime BeginningOfWeek(this DateTime date, DayOfWeek FirstDayOfWeek) => date.FirstDayOfWeek(FirstDayOfWeek).BeginningOfDay();

        public static DateTime BeginningOfWeek(this DateTime date, DayOfWeek FirstDayOfWeek, int timezoneOffset) => date.FirstDayOfWeek(FirstDayOfWeek).BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the week changing the time to the very start of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-19T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfWeek(this DateTime date, int timezoneOffset, CultureInfo culture = null) => date.FirstDayOfWeek(culture).BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Returns the Start day of the year changing the time to the very start of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-01-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfYear(this DateTime date) => date.FirstDayOfYear().BeginningOfDay();

        /// <summary>
        /// Returns the Start day of the year changing the time to the very start of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-01-01T00:00:00.000. <see cref="DateTime"/>
        /// </summary>
        public static DateTime BeginningOfYear(this DateTime date, int timezoneOffset) => date.FirstDayOfYear().BeginningOfDay(timezoneOffset);

        /// <summary>
        /// Calcula a porcentagem de diferenca entre duas datas de acordo com a data inicial especificada
        /// </summary>
        /// <param name="MidDate">Data do meio a ser verificada (normalmente Now)</param>
        /// <param name="StartDate">Data Inicial</param>
        /// <param name="EndDate">Data Final</param>
        /// <returns></returns>
        public static decimal CalculateTimelinePercent(this DateTime MidDate, DateTime StartDate, DateTime EndDate)
        {
            Misc.FixOrder(ref StartDate, ref EndDate);
            if (MidDate < StartDate)
            {
                return 0m;
            }

            if (MidDate > EndDate)
            {
                return 100m;
            }

            return (decimal)((MidDate - StartDate).Ticks * 100L / (double)(EndDate - StartDate).Ticks);
        }


        /// <summary>
        /// Clear Milliseconds from <see cref="DateTime"/>
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static DateTime ClearMilliseconds(this DateTime Date) => Date.AddTicks(-(Date.Ticks % TimeSpan.TicksPerSecond));

        /// <summary>
        /// Remove o tempo de todas as datas de uma lista e retorna uma nova lista
        /// </summary>
        /// <param name="List">Lista que será alterada</param>
        public static IEnumerable<DateTime> ClearTime(this IEnumerable<DateTime> List) => List.Select(x => x.Date);

        /// <summary>
        /// Converte uma string em datetime a partir de um formato especifico
        /// </summary>
        /// <param name="DateString">String original</param>
        /// <param name="Format"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static DateTime ConvertDateString(this string DateString, string Format, CultureInfo Culture = null) => DateTime.ParseExact(DateString, Format, Culture ?? CultureInfo.CurrentCulture);

        /// <summary>
        /// Create a <see cref="DateRange"/> from giving <paramref name="List"/> and/or <paramref
        /// name="StartDate"/>/ <paramref name="EndDate"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
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
                Period.EndDate = EndDate.Value;
            }

            return Period;
        }

        /// <summary>
        /// Cria um <see cref="DateRange"/> a partir de 2 datas e/ou uma propriedade do tipo <see
        /// cref="DateTime"/> de <paramref name="T"/> como filtro de uma lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns>
        /// Um DateRange a partir das datas Iniciais e finais especificadas ou um daterange a partir
        /// da menor e maior datas de uma lista
        /// </returns>
        public static DateRange CreateDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateTime? StartDate = default, DateTime? EndDate = default) where T : class
        {
            var Period = new DateRange
            {
                ForceFirstAndLastMoments = true
            };

            StartDate = StartDate ?? List.Min(PropertyExpression);
            EndDate = EndDate ?? List.Max(PropertyExpression);

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
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public static TimeSpan Days(this int days) => TimeSpan.FromDays(days);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Days.
        /// </summary>
        public static TimeSpan Days(this double days) => TimeSpan.FromDays(days);

        /// <summary>
        /// Decreases the <see cref="DateTime"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public static DateTime DecreaseTime(this DateTime startDate, TimeSpan toSubtract) => startDate - toSubtract;

        /// <summary>
        /// Returns the last day of the bimester changing the time to the very end of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfBimester(this DateTime date) => date.LastDayOfBimester().EndOfDay();

        /// <summary>
        /// Returns the last day of the bimester changing the time to the very end of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfBimester(this DateTime date, int timeZoneOffset) => date.LastDayOfBimester().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the very end of the given day (the last millisecond of the last hour for the
        /// given <see cref="DateTime"/>).
        /// </summary>
        public static DateTime EndOfDay(this DateTime date) => new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind);

        /// <summary>
        /// Returns the timezone-adjusted very end of the given day (the last millisecond of the
        /// last hour for the given <see cref="DateTime"/>).
        /// </summary>
        public static DateTime EndOfDay(this DateTime date, int timeZoneOffset) => new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999, date.Kind).AddHours(timeZoneOffset);

        public static DateTime EndOfFortnight(this DateTime date, int timeZoneOffset) => date.LastDayOfFortnight().EndOfDay(timeZoneOffset);

        public static DateTime EndOfFortnight(this DateTime date) => date.LastDayOfFortnight().EndOfDay();

        /// <summary>
        /// Returns the last day of the month changing the time to the very end of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfMonth(this DateTime date) => date.LastDayOfMonth().EndOfDay();

        /// <summary>
        /// Returns the last day of the month changing the time to the very end of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfMonth(this DateTime date, int timeZoneOffset) => date.LastDayOfMonth().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the quarter changing the time to the very end of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfQuarter(this DateTime date) => date.LastDayOfQuarter().EndOfDay();

        /// <summary>
        /// Returns the last day of the quarter changing the time to the very end of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfQuarter(this DateTime date, int timeZoneOffset) => date.LastDayOfQuarter().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the bimester changing the time to the very end of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfSemester(this DateTime date) => date.LastDayOfSemester().EndOfDay();

        /// <summary>
        /// Returns the last day of the bimester changing the time to the very end of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfSemester(this DateTime date, int timeZoneOffset) => date.LastDayOfSemester().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the week changing the time to the very end of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-25T23:59:59.999
        /// </summary>
        public static DateTime EndOfWeek(this DateTime date) => date.LastDayOfWeek().EndOfDay();
        public static DateTime EndOfWeek(this DateTime date, CultureInfo culture) => date.LastDayOfWeek(culture).EndOfDay();

        public static DateTime EndOfWeek(this DateTime date, DayOfWeek FirstDayOfWeek) => date.LastDayOfWeek(FirstDayOfWeek).EndOfDay();

        public static DateTime EndOfWeek(this DateTime date, DayOfWeek FirstDayOfWeek, int timeZoneOffset) => date.LastDayOfWeek(FirstDayOfWeek).EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the week changing the time to the very end of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-25T23:59:59.999
        /// </summary>
        public static DateTime EndOfWeek(this DateTime date, int timeZoneOffset) => date.LastDayOfWeek().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Returns the last day of the year changing the time to the very end of the day. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfYear(this DateTime date) => date.LastDayOfYear().EndOfDay();

        /// <summary>
        /// Returns the last day of the year changing the time to the very end of the day with
        /// timezone-adjusted. Eg, 2011-12-24T06:40:20.005 =&gt; 2011-12-31T23:59:59.999
        /// </summary>
        public static DateTime EndOfYear(this DateTime date, int timeZoneOffset) => date.LastDayOfYear().EndOfDay(timeZoneOffset);

        /// <summary>
        /// Retorna o primeiro dia de um bimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfBimester(this DateTime Date)
        {
            switch (Date.GetBimesterOfYear())
            {
                case 1: return new DateTime(Date.Year, 1, 1).Date;
                case 2: return new DateTime(Date.Year, 3, 1).Date;
                case 3: return new DateTime(Date.Year, 5, 1).Date;
                case 4: return new DateTime(Date.Year, 7, 1).Date;
                case 5: return new DateTime(Date.Year, 9, 1).Date;
                default: return new DateTime(Date.Year, 11, 1).Date;
            }
        }

        /// <summary>
        /// Returns the first day of the fortnight
        /// </summary>
        public static DateTime FirstDayOfFortnight(this DateTime date) => new DateTime(date.Year, date.Month, date.Day > 15 ? 16 : 1, date.Hour, date.Minute, date.Second, date.Millisecond, date.Kind);

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the first day in that month.
        /// </summary>
        /// <param name="current">The current <see cref="DateTime"/> to be changed.</param>
        /// <returns>
        /// given <see cref="DateTime"/> with the day part set to the first day in that month.
        /// </returns>
        public static DateTime FirstDayOfMonth(this DateTime current) => current.SetDay(1);

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="MonthNumber">Data</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(this int MonthNumber, int? Year = default)
        {
            Year = (Year ?? DateTime.Now.Year).SetMinValue(DateTime.MinValue.Year);
            return new DateTime(Year.Value, MonthNumber, 1);
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the first day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html
        /// </summary>
        /// <param name="current"></param>
        /// <returns>
        /// given <see cref="DateTime"/> with the day part set to the first day in the quarter.
        /// </returns>
        public static DateTime FirstDayOfQuarter(this DateTime current)
        {
            var currentQuarter = (current.Month - 1) / 3 + 1;
            var firstDay = new DateTime(current.Year, 3 * currentQuarter - 2, 1);

            return current.SetDate(firstDay.Year, firstDay.Month, firstDay.Day);
        }

        /// <summary>
        /// Retorna o primeiro dia de um semestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime FirstDayOfSemester(this DateTime Date) => Date.GetSemesterOfYear() == 1 ? Date.FirstDayOfYear() : new DateTime(Date.Year, 7, 1).Date;

        /// <summary>
        /// Returns a DateTime adjusted to the beginning of the week.
        /// </summary>
        /// <param name="dateTime">The DateTime to adjust</param>
        /// <returns>A DateTime instance adjusted to the beginning of the current week</returns>
        /// <remarks>the beginning of the week is controlled by the current Culture</remarks>
        public static DateTime FirstDayOfWeek(this DateTime dateTime, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            return dateTime.FirstDayOfWeek(firstDayOfWeek);
        }

        /// <summary>
        /// Retorna o primeiro dia da semana da data especificada
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
        /// <returns></returns>
        public static DateTime FirstDayOfWeek(this DateTime Date, DayOfWeek FirstDayOfWeek)
        {
            while (Date.DayOfWeek > FirstDayOfWeek)
            {
                Date = Date.AddDays(-1);
            }

            return Date;
        }

        /// <summary>
        /// Returns the first day of the year keeping the time component intact. Eg,
        /// 2011-02-04T06:40:20.005 =&gt; 2011-01-01T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime FirstDayOfYear(this DateTime current) => current.SetDate(current.Year, 1, 1);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTime From(this TimeSpan from, DateTime originalValue) => originalValue + from;

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTime From(this DateRange from, DateTime originalValue) => originalValue.Add(from);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns
        /// resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTime FromNow(this TimeSpan from) => from.From(DateTime.Now);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns
        /// resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTime FromNow(this DateRange from) => from.From(DateTime.Now);

        public static DateTime FromToday(this TimeSpan from) => from.From(DateTime.Today);

        public static DateTime FromToday(this DateRange from) => from.From(DateTime.Today);

        /// <summary>
        /// Return the age at giving date
        /// </summary>
        /// <param name="BirthDate">Birth date</param>
        /// <param name="AtDate"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime BirthDate, DateTime? AtDate = null)
        {
            int age = 0;
            AtDate = AtDate.OrNow();
            if (BirthDate <= AtDate)
            {
                age = AtDate.Value.Year - BirthDate.Year;
                if (BirthDate > AtDate?.AddYears(-age))
                {
                    age -= 1;
                }
            }

            return age;
        }

        /// <summary>
        /// Return the age at giving day, month and year
        /// </summary>
        /// <param name="BirthDate">Birth date</param>
        /// <param name="AtDate"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime BirthDate, int Day, int Month, int Year) => GetAge(BirthDate, new DateTime(Year, Month, Day));

        /// <summary>
        /// Return the age at giving month and year
        /// </summary>
        /// <param name="BirthDate">Birth date</param>
        /// <param name="AtDate"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime BirthDate, int Month, int Year) => GetAge(BirthDate, DateTime.Today.SetMonth(Month).SetYear(Year));

        /// <summary>
        /// Return the age at giving year
        /// </summary>
        /// <param name="BirthDate">Birth date</param>
        /// <param name="AtDate"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime BirthDate, int Year) => GetAge(BirthDate, DateTime.Today.SetYear(Year));

        public static int GetBimesterOfYear(this DateTime DateAndtime) => GetBimesterOfYear(DateAndtime.Month);

        /// <summary>
        /// Pega o numero do Bimesre a partir de uma data
        /// </summary>
        /// <param name="DateAndtime"></param>
        /// <returns></returns>
        public static int GetBimesterOfYear(this int Month)
        {
            if (Month <= 2)
            {
                return 1;
            }
            else if (Month <= 4)
            {
                return 2;
            }
            else if (Month <= 6)
            {
                return 3;
            }
            else if (Month <= 8)
            {
                return 4;
            }
            else if (Month <= 10)
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }

        public static DateRange GetBimesterRange(this DateTime Date) => DateRange.Bimester(Date);

        public static DateTime GetDateFromDayOfYear(this int DayOfYear, int? year = null, int? hours = null, int? minutes = null, int? seconds = null, int? milliseconds = null) => new DateTime(year ?? DateTime.Now.Year, 1, 1, hours ?? DateTime.Now.Hour, minutes ?? DateTime.Now.Minute, seconds ?? DateTime.Now.Second, milliseconds ?? DateTime.Now.Millisecond).AddDays(DayOfYear - 1);

        public static DateRange GetDayRange(this DateTime Date) => DateRange.Day(Date);

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
            if (!DaysOfWeek.Any())
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

        public static int GetFortnightNumber(this DateTime datetime) => datetime.Day <= 15 ? 1 : 2;

        public static DateRange GetFortnightRange(this DateTime Date) => DateRange.Fortnight(Date);

        public static Expression<Func<T, string>> GetGroupByPeriodExpression<T>(this IEnumerable<T> data, string Group, Expression<Func<T, DateTime>> prop, PeriodFormat formats = null)
        {
            data = data ?? Array.Empty<T>();
            return (formats ?? new PeriodFormat()).GroupByPeriodExpression(Group, prop);
        }

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetLongMonthName(this DateTime Date, CultureInfo Culture = null) => Date.ToString("MMMM", Culture ?? CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a dictionary with all months on especific format
        /// </summary>
        /// <param name="Culture"></param>
        /// <param name="TextType"></param>
        /// <param name="ValueType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetMonthList(CalendarFormat TextType = CalendarFormat.LongName, CalendarFormat ValueType = CalendarFormat.Number) => CultureInfo.CurrentCulture.GetMonthList(TextType, ValueType);

        /// <summary>
        /// Returns a dictionary with all months on especific format
        /// </summary>
        /// <param name="Culture"></param>
        /// <param name="TextType"></param>
        /// <param name="ValueType"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetMonthList(this CultureInfo Culture, CalendarFormat TextType = CalendarFormat.LongName, CalendarFormat ValueType = CalendarFormat.Number)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            var MonthsRet = new Dictionary<string, string>();
            for (int i = 1; i <= 12; i++)
            {
                string key;
                switch (TextType)
                {
                    case CalendarFormat.LongName:
                        {
                            key = i.ToLongMonthName(Culture);
                            break;
                        }

                    case CalendarFormat.ShortName:
                        {
                            key = i.ToShortMonthName(Culture);
                            break;
                        }

                    default:
                        {
                            key = i.ToString(Culture);
                            break;
                        }
                }

                string value;
                switch (ValueType)
                {
                    case CalendarFormat.LongName:
                        {
                            value = i.ToLongMonthName(Culture);
                            break;
                        }

                    case CalendarFormat.ShortName:
                        {
                            value = i.ToShortMonthName(Culture);
                            break;
                        }

                    default:
                        {
                            value = i.ToString(Culture);
                            break;
                        }
                }

                MonthsRet[key] = value;
            }

            return MonthsRet;
        }

        public static DateRange GetMonthRange(this DateTime Date) => DateRange.Month(Date);

        public static int GetQuarterOfYear(this DateTime DateAndTime) => GetQuarterOfYear(DateAndTime.Month);

        /// <summary>
        /// Pega o numero do trimestre a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetQuarterOfYear(this int Month)
        {
            if (Month <= 3)
            {
                return 1;
            }
            else if (Month <= 6)
            {
                return 2;
            }
            else if (Month <= 9)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        public static DateRange GetQuarterRange(this DateTime Date) => DateRange.Quarter(Date);

        /// <summary>
        /// Pega o numero do semestre a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetSemesterOfYear(this DateTime DateAndTime) => GetSemesterOfYear(DateAndTime.Month);

        public static int GetSemesterOfYear(this int Month) => Month <= 6 ? 1 : 2;

        public static DateRange GetSemesterRange(this DateTime Date) => DateRange.Semester(Date);

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetShortMonthName(this DateTime Date, CultureInfo Culture = null) => Date.ToString("MMM", Culture ?? CultureInfo.CurrentCulture);


        public static string GetWeekDay(this DateTime DateTime, CalendarFormat Type = CalendarFormat.LongName, CultureInfo Culture = default) => GetWeekDay(DateTime.DayOfWeek.ToInt(), Type, Culture);


        public static string GetWeekDay(this int WeekDay, CalendarFormat Type = CalendarFormat.LongName, CultureInfo Culture = default)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;

            switch (Type)
            {
                case CalendarFormat.LongName: return WeekDay.ToLongDayOfWeekName(Culture);
                case CalendarFormat.ShortName: return WeekDay.ToShortDayOfWeekName(Culture);
                default: return WeekDay.ToString(Culture);
            }
        }

        public static Dictionary<string, string> GetWeekDays(this CultureInfo Culture, CalendarFormat TextType = CalendarFormat.LongName, CalendarFormat ValueType = CalendarFormat.Number)
        {
            var WeekDaysRet = new Dictionary<string, string>();

            for (int weekday = 1; weekday <= 7; weekday++)
            {
                WeekDaysRet[weekday.GetWeekDay(TextType, Culture)] = weekday.GetWeekDay(ValueType, Culture);
            }

            return WeekDaysRet;
        }

        /// <summary>
        /// Pega o numero da semana, do mês e ano pertencente
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static WeekInfo GetWeekInfo(this DateTime DateAndTime, CultureInfo culture = null, string stringFormat = null) => new WeekInfo(DateAndTime, culture, stringFormat);

        /// <summary>
        /// Pega o numero da semana do mês a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetWeekNumberOfMonth(this DateTime DateAndTime) => DateAndTime.GetWeekInfo().WeekOfMonth;

        public static int GetWeekNumberOfYear(this DateTime DateAndTime) => DateAndTime.GetWeekInfo().WeekOfYear;

        /// <summary>
        /// Retorna o numero da semana relativa ao ano
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <param name="FirstDayOfWeek"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime Date, CultureInfo Culture = null, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday, CalendarWeekRule Rule = CalendarWeekRule.FirstFourDayWeek) => (Culture ?? CultureInfo.InvariantCulture).Calendar.GetWeekOfYear(Date, Rule, FirstDayOfWeek);

        /// <summary>
        /// Retorna um DateRange equivalente a semana de uma data especifica
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é domingo)</param>
        /// <returns></returns>
        public static DateRange GetWeekRange(this DateTime Date, DayOfWeek FirstDayOfWeek) => DateRange.Week(Date, FirstDayOfWeek);

        public static DateRange GetWeekRange(this DateTime Date, CultureInfo culture = null) => DateRange.Week(Date, culture);

        public static DateRange GetYearRange(this DateTime Date) => DateRange.Year(Date);

        public static IEnumerable<IGrouping<string, T>> GroupByPeriod<T>(this IEnumerable<T> data, string Group, Expression<Func<T, DateTime>> prop, PeriodFormat formats = null)
        {
            data = data ?? Array.Empty<T>();
            return data.GroupBy((formats ?? new PeriodFormat()).GroupByPeriodExpression(Group, prop).Compile());
        }

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public static TimeSpan Hours(this int hours) => TimeSpan.FromHours(hours);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Hours.
        /// </summary>
        public static TimeSpan Hours(this double hours) => TimeSpan.FromHours(hours);

        /// <summary>
        /// Increases the <see cref="DateTime"/> object with given <see cref="TimeSpan"/> value.
        /// </summary>
        public static DateTime IncreaseTime(this DateTime startDate, TimeSpan toAdd) => startDate + toAdd;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is After then current value.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="toCompareWith">Value to compare with.</param>
        /// <returns><c>true</c> if the specified current is after; otherwise, <c>false</c>.</returns>
        public static bool IsAfter(this DateTime current, DateTime toCompareWith) => current > toCompareWith;

        /// <summary>
        /// Verifica se a Data de hoje é um aniversário
        /// </summary>
        /// <param name="BirthDate">Data de nascimento</param>
        /// <returns></returns>
        public static bool IsAnniversary(this DateTime BirthDate, DateTime? CompareWith = default)
        {
            CompareWith = CompareWith ?? DateTime.Today;
            return (BirthDate.Day == CompareWith.Value.Day) && (BirthDate.Month == CompareWith.Value.Month);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> is before then current value.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="toCompareWith">Value to compare with.</param>
        /// <returns><c>true</c> if the specified current is before; otherwise, <c>false</c>.</returns>
        public static bool IsBefore(this DateTime current, DateTime toCompareWith) => current < toCompareWith;

        /// <summary>
        /// Determine if a <see cref="DateTime"/> is in the future.
        /// </summary>
        /// <param name="dateTime">The date to be checked.</param>
        /// <returns><c>true</c> if <paramref name="dateTime"/> is in the future; otherwise <c>false</c>.</returns>
        public static bool IsInFuture(this DateTime dateTime) => dateTime > DateTime.Now;

        /// <summary>
        /// Determine if a <see cref="DateTime"/> is in the past.
        /// </summary>
        /// <param name="dateTime">The date to be checked.</param>
        /// <returns><c>true</c> if <paramref name="dateTime"/> is in the past; otherwise <c>false</c>.</returns>
        public static bool IsInPast(this DateTime dateTime) => dateTime < DateTime.Now;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same day
        /// (day + month + year) then current
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="date">Value to compare with</param>
        /// <returns>
        /// <c>true</c> if the specified date is exactly the same year then current; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameDay(this DateTime current, DateTime date) => current.Date == date.Date;

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same month
        /// (month + year) then current. Eg, 2015-12-01 and 2014-12-01 =&gt; False
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="date">Value to compare with</param>
        /// <returns>
        /// <c>true</c> if the specified date is exactly the same month and year then current;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameMonth(this DateTime current, DateTime date) => current.Month == date.Month && current.Year == date.Year;

        /// <summary>
        /// Verifica se uma data é do mesmo mês e ano que outra data
        /// </summary>
        /// <param name="[Date]">Primeira data</param>
        /// <param name="AnotherDate">Segunda data</param>
        /// <returns></returns>
        public static bool IsSameMonthAndYear(this DateTime Date, DateTime AnotherDate) => Date.IsBetweenOrEqual(AnotherDate.FirstDayOfMonth().Date, AnotherDate.EndOfMonth());

        /// <summary>
        /// Determines whether the specified <see cref="DateTime"/> value is exactly the same year
        /// then current. Eg, 2015-12-01 and 2015-01-01 =&gt; True
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="date">Value to compare with</param>
        /// <returns>
        /// <c>true</c> if the specified date is exactly the same date then current; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSameYear(this DateTime current, DateTime date) => current.Year == date.Year;

        /// <summary>
        /// Check if <see cref="DateTime.TimeOfDay"/> is between <paramref name="TimeBegin"/> and
        /// <paramref name="TimeEnd"/> (Uses <see cref="TimePart(TimeSpan)"/> on both parameters)
        /// </summary>
        /// <param name="Date"></param>
        /// <param name="TimeBegin"></param>
        /// <param name="TimeEnd"></param>
        /// <remarks><inheritdoc cref="Misc.IsBetween(IComparable, IComparable, IComparable)"/></remarks>
        /// <returns></returns>
        public static bool IsTimeBetween(this DateTime Date, TimeSpan TimeBegin, TimeSpan TimeEnd)
        {
            TimeBegin = TimeBegin.TimePart();
            TimeEnd = TimeEnd.TimePart();
            Misc.FixOrder(ref TimeBegin, ref TimeEnd);
            return Date.TimeOfDay.IsBetween(TimeBegin, TimeEnd);
        }

        public static bool IsWeekend(this DateTime YourDate) => YourDate.DayOfWeek == DayOfWeek.Sunday | YourDate.DayOfWeek == DayOfWeek.Saturday;

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
            {
                FromDate = FromDate.Value.AddDays(-1);
            }

            return (DateTime)FromDate;
        }

        /// <summary>
        /// Retorna o ultimo dia de um bimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime LastDayOfBimester(this DateTime Date)
        {
            switch (Date.GetBimesterOfYear())
            {
                case 1: return new DateTime(Date.Year, 2, 1).LastDayOfMonth();
                case 2: return new DateTime(Date.Year, 4, 1).LastDayOfMonth();
                case 3: return new DateTime(Date.Year, 6, 1).LastDayOfMonth();
                case 4: return new DateTime(Date.Year, 8, 1).LastDayOfMonth();
                case 5: return new DateTime(Date.Year, 10, 1).LastDayOfMonth();
                default: return new DateTime(Date.Year, 12, 1).LastDayOfMonth();
            }
        }

        /// <summary>
        /// Retorna a ultima data da quinzena a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime LastDayOfFortnight(this DateTime Date) => new DateTime(Date.Year, Date.Month, Date.Day <= 15 ? 15 : Date.LastDayOfMonth().Day, Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the last day in that month.
        /// </summary>
        /// <param name="current">The current DateTime to be changed.</param>
        /// <returns>
        /// given <see cref="DateTime"/> with the day part set to the last day in that month.
        /// </returns>
        public static DateTime LastDayOfMonth(this DateTime current) => current.SetDay(DateTime.DaysInMonth(current.Year, current.Month));

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="MonthNumber">Data</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(this int MonthNumber, int? Year = default)
        {
            Year = (Year ?? DateTime.Now.Year).SetMinValue(DateTime.MinValue.Year);
            return new DateTime(Year.Value, MonthNumber, 1).LastDayOfMonth();
        }

        /// <summary>
        /// Sets the day of the <see cref="DateTime"/> to the last day in that calendar quarter.
        /// credit to http://www.devcurry.com/2009/05/find-first-and-last-day-of-current.html
        /// </summary>
        /// <param name="current"></param>
        /// <returns>
        /// given <see cref="DateTime"/> with the day part set to the last day in the quarter.
        /// </returns>
        public static DateTime LastDayOfQuarter(this DateTime current)
        {
            var currentQuarter = (current.Month - 1) / 3 + 1;
            var firstDay = current.SetDate(current.Year, 3 * currentQuarter - 2, 1);
            return firstDay.SetMonth(firstDay.Month + 2).LastDayOfMonth();
        }

        /// <summary>
        /// Retorna o ultimo dia de um semestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime LastDayOfSemester(this DateTime Date) => Date.GetSemesterOfYear() == 1 ? new DateTime(Date.Year, 6, 1).LastDayOfMonth() : Date.LastDayOfYear();

        /// <summary>
        /// Returns the last day of the week keeping the time component intact. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-25T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime current) => current.FirstDayOfWeek().AddDays(6);

        /// <summary>
        /// Retorna o ultimo dia da semana da data especificada
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
        /// <returns></returns>
        public static DateTime LastDayOfWeek(this DateTime Date, DayOfWeek FirstDayOfWeek) => Date.FirstDayOfWeek(FirstDayOfWeek).AddDays(6d);

        public static DateTime LastDayOfWeek(this DateTime Date, CultureInfo culture) => Date.FirstDayOfWeek(culture).AddDays(6d);

        /// <summary>
        /// Returns the last day of the year keeping the time component intact. Eg,
        /// 2011-12-24T06:40:20.005 =&gt; 2011-12-31T06:40:20.005
        /// </summary>
        /// <param name="current">The DateTime to adjust</param>
        /// <returns></returns>
        public static DateTime LastDayOfYear(this DateTime current) => current.SetDate(current.Year, 12, 31);

        /// <summary>
        /// Return the last sunday
        /// </summary>
        /// <returns></returns>
        public static DateTime LastSunday(DateTime? FromDate = null) => LastDay(DayOfWeek.Sunday, FromDate);

        /// <summary>
        /// Returns original <see cref="DateTime"/> value with time part set to midnight (alias for
        /// <see cref="BeginningOfDay(DateTime)"/> method).
        /// </summary>
        public static DateTime Midnight(this DateTime value) => value.BeginningOfDay();

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
        /// Returns first next occurrence of specified <see cref="DayOfWeek"/>.
        /// </summary>
        public static DateTime NextDay(this DateTime start, DayOfWeek day)
        {
            do
            {
                start = start.NextDay();
            } while (start.DayOfWeek != day);

            return start;
        }

        /// <summary>
        /// Returns <see cref="DateTime"/> increased by 24 hours ie GetNextPart Day.
        /// </summary>
        public static DateTime NextDay(this DateTime start) => start + 1.Days();

        /// <summary>
        /// Retorna o proximo dia referente a um dia da semana
        /// </summary>
        /// <param name="DayOfWeek"></param>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public static DateTime NextDay(this DayOfWeek DayOfWeek, DateTime? FromDate = default)
        {
            FromDate = FromDate ?? DateTime.Now;
            while (FromDate.Value.DayOfWeek != DayOfWeek)
            {
                FromDate = FromDate.Value.AddDays(1d);
            }

            return (DateTime)FromDate;
        }

        /// <summary>
        /// Pula para a data inicial da proxima quinzena
        /// </summary>
        /// <param name="FromDate">Data de partida</param>
        /// <param name="Num">Numero de quinzenas para adiantar</param>
        /// <returns></returns>
        public static DateTime NextFortnight(this DateTime FromDate, int Num = 1) => new FortnightGroup(FromDate, Num.SetMinValue(1) + 1).Last().StartDate;

        /// <summary>
        /// Returns the next month keeping the time component intact. Eg, 2012-12-05T06:40:20.005
        /// =&gt; 2013-01-05T06:40:20.005 If the next month doesn't have that many days the last day
        /// of the next month is used. Eg, 2013-01-31T06:40:20.005 =&gt; 2013-02-28T06:40:20.005
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
        /// Retorna o proximo domingo
        /// </summary>
        /// <returns></returns>
        public static DateTime NextSunday(DateTime? FromDate = default) => NextDay(DayOfWeek.Sunday, FromDate);

        /// <summary>
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc) in the next calendar
        /// year. If that day does not exist in next year in same month, number of missing days is
        /// added to the last day in same month next year.
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
        /// Returns original <see cref="DateTime"/> value with time part set to Noon (12:00:00h).
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> find Noon for.</param>
        /// <returns>A <see cref="DateTime"/> value with time part set to Noon (12:00:00h).</returns>
        public static DateTime Noon(this DateTime value) => value.SetTime(12, 0, 0, 0);

        /// <summary>
        /// Return the current value of <paramref name="Date"/> or <see cref="DateTime.Now"/> if
        /// <paramref name="Date"/> is <b><see cref="null"/></b>
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static DateTime OrNow(this DateTime? Date) => Date ?? DateTime.Now;

        /// <summary>
        /// Return the current value of <paramref name="Date"/> or <see cref="DateTime.Today"/> if
        /// <paramref name="Date"/> is <b><see cref="null"/></b>
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static DateTime OrToday(this DateTime? Date) => Date ?? DateTime.Today;

        public static DateRange PeriodRange(this DateTime date, string Group, CultureInfo culture) => PeriodRange(date, date, Group, (culture ?? CultureInfo.CurrentCulture).DateTimeFormat.FirstDayOfWeek);

        public static DateRange PeriodRange(this DateRange Dates, string Group, CultureInfo culture) => PeriodRange(Dates, Group, (culture ?? CultureInfo.CurrentCulture).DateTimeFormat.FirstDayOfWeek);

        public static DateRange PeriodRange(this DateTime date, string Group) => PeriodRange(date, Group, CultureInfo.CurrentCulture);

        public static DateRange PeriodRange(this DateRange dates, string Group) => PeriodRange(dates, Group, CultureInfo.CurrentCulture);

        public static DateRange PeriodRange(this DateRange dates, string Group, DayOfWeek FirstDayOfWeek) => PeriodRange(dates.StartDate, dates.EndDate, Group, FirstDayOfWeek);

        public static DateRange PeriodRange(this DateTime StartDate, DateTime? EndDate, string Group, CultureInfo culture) => PeriodRange(StartDate, EndDate, Group, (culture ?? CultureInfo.CurrentCulture).DateTimeFormat.FirstDayOfWeek);

        public static DateRange PeriodRange(this DateTime StartDate, DateTime? EndDate, string Group) => PeriodRange(StartDate, EndDate, Group, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a group-defined period from a date. The dates will be adjusted for the beginning
        /// and end of this period
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Group"></param>
        /// <param name="FirstDayOfWeek"></param>
        /// <returns></returns>
        public static DateRange PeriodRange(this DateTime StartDate, DateTime? EndDate, string Group, DayOfWeek FirstDayOfWeek)
        {
            var end = EndDate ?? StartDate;
            (StartDate, EndDate) = Misc.FixOrder(ref StartDate, ref end);
            switch (Group.ToLowerInvariant().RemoveAccents())
            {
                case "month":
                case "mensal":
                case "mes":
                    StartDate = StartDate.BeginningOfMonth();
                    EndDate = EndDate.Value.EndOfMonth();
                    break;

                case "semanal":
                case "week":
                case "semana":
                    StartDate = StartDate.BeginningOfWeek(FirstDayOfWeek);
                    EndDate = EndDate.Value.EndOfWeek(FirstDayOfWeek);
                    break;

                case "quinzenal":
                case "fortnight":
                case "quinzena":
                    StartDate = StartDate.BeginningOfFortnight();
                    EndDate = EndDate.Value.EndOfFortnight();
                    break;

                case "bimestral":
                case "bimester":
                case "bimestre":
                    StartDate = StartDate.BeginningOfBimester();
                    EndDate = EndDate.Value.EndOfBimester();
                    break;

                case "trimestral":
                case "quarter":
                case "trimestre":
                    StartDate = StartDate.BeginningOfQuarter();
                    EndDate = EndDate.Value.EndOfQuarter();
                    break;

                case "semestral":
                case "halfyear":
                case "semester":
                case "semestre":
                    StartDate = StartDate.BeginningOfSemester();
                    EndDate = EndDate.Value.EndOfSemester();
                    break;

                case "anual":
                case "year":
                case "ano":
                    StartDate = StartDate.BeginningOfYear();
                    EndDate = EndDate.Value.EndOfYear();
                    break;

                case "diario":
                case "day":
                case "dia":
                    StartDate = StartDate.BeginningOfDay();
                    EndDate = EndDate.Value.EndOfDay();
                    break;

                default:
                    break;
            }
            return new DateRange(StartDate, EndDate.Value);
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
        /// Returns <see cref="DateTime"/> decreased by 24h period ie GetPreviousPart Day.
        /// </summary>
        public static DateTime PreviousDay(this DateTime start) => start - 1.Days();

        /// <summary>
        /// Returns the previous month keeping the time component intact. Eg,
        /// 2010-01-20T06:40:20.005 =&gt; 2009-12-20T06:40:20.005 If the previous month doesn't have
        /// that many days the last day of the previous month is used. Eg, 2009-03-31T06:40:20.005
        /// =&gt; 2009-02-28T06:40:20.005
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
        /// Returns the same date (same Day, Month, Hour, Minute, Second etc) in the previous
        /// calendar year. If that day does not exist in previous year in same month, number of
        /// missing days is added to the last day in same month previous year.
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
        /// Generates <see cref="TimeSpan"/> value for given number of Quarters.
        /// </summary>
        /// <param name="quarters"></param>
        /// <returns></returns>
        public static TimeSpan Quarters(this int quarters, DateTime? fromDateTime = null) => fromDateTime.OrNow().Date.AddMonths(quarters * 3) - fromDateTime.OrNow().Date;

        /// <summary>
        /// Rounds <paramref name="dateTime"/> to the nearest <see cref="RoundTo"/>.
        /// </summary>
        /// <returns>The rounded <see cref="DateTime"/>.</returns>
        public static DateTime Round(this DateTime dateTime, RoundTo round = RoundTo.Second)
        {
            DateTime rounded;

            switch (round)
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
                        rounded = dateTime;
                        break;
                    }
            }

            return rounded;
        }

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
                            rounded += 1.Seconds();
                        }

                        break;
                    }
                case RoundTo.Minute:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
                        if (timeSpan.Seconds >= 30)
                        {
                            rounded += 1.Minutes();
                        }

                        break;
                    }
                case RoundTo.Hour:
                    {
                        rounded = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
                        if (timeSpan.Minutes >= 30)
                        {
                            rounded += 1.Hours();
                        }

                        break;
                    }
                case RoundTo.Day:
                    {
                        rounded = new TimeSpan(timeSpan.Days, 0, 0, 0);
                        if (timeSpan.Hours >= 12)
                        {
                            rounded += 1.Days();
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
        /// Returns <see cref="DateTime"/> with changed Year part.
        /// </summary>
        public static DateTime SetDate(this DateTime value, int year) => new DateTime(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year and Month part.
        /// </summary>
        public static DateTime SetDate(this DateTime value, int year, int month) => new DateTime(year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year, Month and Day part.
        /// </summary>
        public static DateTime SetDate(this DateTime value, int year, int month, int day) => new DateTime(year, month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Day part.
        /// </summary>
        public static DateTime SetDay(this DateTime value, int day) => new DateTime(value.Year, value.Month, day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Hour part.
        /// </summary>
        public static DateTime SetHour(this DateTime originalDate, int hour) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Millisecond part.
        /// </summary>
        public static DateTime SetMillisecond(this DateTime originalDate, int millisecond) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, originalDate.Second, millisecond, originalDate.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Minute part.
        /// </summary>
        public static DateTime SetMinute(this DateTime originalDate, int minute) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Month part.
        /// </summary>
        public static DateTime SetMonth(this DateTime value, int month) => new DateTime(value.Year, month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Second part.
        /// </summary>
        public static DateTime SetSecond(this DateTime originalDate, int second) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, originalDate.Hour, originalDate.Minute, second, originalDate.Millisecond, originalDate.Kind);

        public static DateTime SetTime(this DateTime current, TimeSpan Time) => current.SetTime(Time.Hours, Time.Minutes, Time.Seconds, Time.Milliseconds);

        public static DateTime SetTime(this DateTime current, DateTime Time) => current.SetTime(Time.Hour, Time.Minute, Time.Second, Time.Millisecond);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour part changed to supplied hour parameter.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, originalDate.Minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour and Minute parts changed to
        /// supplied hour and minute parameters.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour, int minute) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, originalDate.Second, originalDate.Millisecond, originalDate.Kind);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour, Minute and Second parts changed
        /// to supplied hour, minute and second parameters.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour, int minute, int second) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, originalDate.Millisecond, originalDate.Kind);

        /// <summary>
        /// Returns the original <see cref="DateTime"/> with Hour, Minute, Second and Millisecond
        /// parts changed to supplied hour, minute, second and millisecond parameters.
        /// </summary>
        public static DateTime SetTime(this DateTime originalDate, int hour, int minute, int second, int millisecond) => new DateTime(originalDate.Year, originalDate.Month, originalDate.Day, hour, minute, second, millisecond, originalDate.Kind);

        /// <summary>
        /// Returns <see cref="DateTime"/> with changed Year part.
        /// </summary>
        public static DateTime SetYear(this DateTime value, int year) => new DateTime(year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Kind);

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(TimeSpan, DateTime)"/>
        /// <remarks>Synonym of <see cref="From(TimeSpan, DateTime)"/> method.</remarks>
        public static DateTime Since(this TimeSpan from, DateTime originalValue) => From(from, originalValue);

        /// <summary>
        /// Adds given <see cref="DateRange"/> to supplied <paramref name="originalValue"/><see
        /// cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(DateRange, DateTime)"/>
        /// <remarks>Synonym of <see cref="From(DateRange, DateTime)"/> method.</remarks>
        public static DateTime Since(this DateRange from, DateTime originalValue) => From(from, originalValue);

        /// <summary>
        /// Subtracts the given number of business days to the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="current">The date to be changed.</param>
        /// <param name="days">Number of business days to be subtracted.</param>
        /// <returns>A <see cref="DateTime"/> increased by a given number of business days.</returns>
        public static DateTime SubtractBusinessDays(this DateTime current, int days, params DayOfWeek[] DaysOff) => AddBusinessDays(current, days.ForceNegative(), DaysOff);

        /// <summary>
        /// Returns a new <see cref="DateTime"/> that subtracts the value of the specified <see
        /// cref="DateRange"/> to the value of this instance.
        /// </summary>
        public static DateTime SubtractDateRange(this DateTime dateTime, DateRange timeSpan) => dateTime.Add(-timeSpan.TimeSpan);

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
        /// Return a <see cref="TimeSpan"/> with the timepart of another <see cref="TimeSpan"/>
        /// (basically, exclude <see cref="TimeSpan.Days"/>)
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        public static TimeSpan TimePart(this TimeSpan Time) => new TimeSpan(Time.Hours, Time.Minutes, Time.Seconds);

        /// <summary>
        /// Returns a <see cref="DateRange"/> from <paramref name="InitialDate"/> to <paramref name="SecondDate"/>
        /// </summary>
        /// <param name="InitialDate"></param>
        /// <param name="SecondDate"></param>
        /// <returns></returns>
        public static DateRange To(this DateTime InitialDate, DateTime SecondDate) => new DateRange(InitialDate, SecondDate);

        /// <summary>
        /// Convert a <see cref="TimeSpan"/> to a human readable string.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert</param>
        /// <returns>A human readable string for <paramref name="timeSpan"/></returns>
        public static string ToDisplayString(this TimeSpan timeSpan) => new DateRange(timeSpan).ToString();

        /// <summary>
        /// Transforma um DateTime em uma saudação (Bom dia, Boa tarde, Boa noite)
        /// </summary>
        /// <param name="Time">Horario</param> 
        /// <returns>Uma string com a saudação</returns>
        public static string ToGreeting(this DateTime Time, string Morning, string Afternoon, string EveningOrNight)
        {
            if (Time.Hour < 12)
            {
                return Morning;
            }
            else if (Time.Hour >= 12 && Time.Hour < 18)
            {
                return Afternoon;
            }
            else
            {
                return EveningOrNight;
            }
        }

        public static string ToLongDayOfWeekName(this int DayNumber, CultureInfo Culture = null) => (Culture ?? CultureInfo.CurrentCulture).DateTimeFormat.GetDayName((DayOfWeek)DayNumber.LimitRange(0, 6));

        public static string ToLongMonthName(this int MonthNumber, CultureInfo Culture = null) => new DateTime(DateTime.Now.Year, MonthNumber.LimitRange(1, 12), 1).GetLongMonthName(Culture);

        public static string ToShortDayOfWeekName(this int DayNumber, CultureInfo Culture = null) => (Culture ?? CultureInfo.CurrentCulture).DateTimeFormat.GetShortestDayName((DayOfWeek)DayNumber.LimitRange(0, 6));

        public static string ToShortMonthName(this int MonthNumber, CultureInfo Culture = null) => new DateTime(DateTime.Now.Year, MonthNumber.LimitRange(1, 12), 1).GetShortMonthName(Culture);

        /// <summary>
        /// Converte uma string de data para outra string de data com formato diferente
        /// </summary>
        /// <param name="DateString">String original</param>
        /// <param name="InputFormat"></param>     
        /// <returns></returns>     
        public static string ChangeDateFormat(this string Date, string FromFormat, string ToFormat)
        {
            try
            {
                return DateTime.ParseExact(Date, FromFormat, CultureInfo.InvariantCulture).ToString(ToFormat, CultureInfo.InvariantCulture);

            }
            catch
            {
                return Date;
            }
        }


        /// <summary>
        /// COnverte um datetime para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this DateTime Date) => Date.ToString("yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);

        /// <summary>
        /// Converte uma string dd/mm/aaaa hh:mm:ss.llll para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this string Date, string FromCulture = "pt-BR") => Date.ToSQLDateString(new CultureInfo(FromCulture, false));
        /// <summary>
        /// Converte uma string dd/mm/aaaa hh:mm:ss.llll para o formato de string do SQL server ou Mysql
        /// </summary>
        public static string ToSQLDateString(this string Date, CultureInfo FromCulture) => Date.IsNotBlank() ? Convert.ToDateTime(Date, (FromCulture ?? CultureInfo.CurrentCulture).DateTimeFormat).ToSQLDateString() : Date;

        /// <summary>
        /// Converte um <see cref="Date"/> para um timezone Especifico
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="TimeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZone(this DateTime Date, string TimeZoneId) => TimeZoneInfo.ConvertTime(Date, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));

        /// <summary>
        /// Converte um <see cref="Date"/> para um timezone Especifico
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneUtc(this DateTime Date, TimeZoneInfo TimeZone) => TimeZoneInfo.ConvertTimeFromUtc(Date, TimeZone);

        /// <summary>
        /// Increases supplied <see cref="DateTime"/> for 7 days ie returns the GetNextPart Week.
        /// </summary>
        public static DateTime WeekAfter(this DateTime start) => start + 1.Weeks();

        /// <summary>
        /// Decreases supplied <see cref="DateTime"/> for 7 days ie returns the GetPreviousPart Week.
        /// </summary>
        public static DateTime WeekEarlier(this DateTime start) => start - 1.Weeks();

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of days * 7).
        /// </summary>
        public static TimeSpan Weeks(this int weeks) => TimeSpan.FromDays(weeks * 7);

        public static TimeSpan Weeks(this short weeks) => weeks.ToInt().Weeks();

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of days * 7).
        /// </summary>
        public static TimeSpan Weeks(this double weeks) => TimeSpan.FromDays(weeks * 7);

        /// <summary>
        /// Returns <see cref="TimeSpan"/> for given number of Weeks (number of days * 7).
        /// </summary>
        public static TimeSpan Weeks(this decimal weeks) => weeks.ToDouble().Weeks();

        /// <summary>
        /// Generates <see cref="TimeSpan"/> value for given number of Years.
        /// </summary>
        public static TimeSpan Years(this int years, DateTime? fromDateTime = null) => fromDateTime.OrNow().Date.AddYears(years) - fromDateTime.OrNow().Date;

        #endregion Public Methods

        #region DateStrings

        public static string BimesterString(this DateTime datetime, string format = null) => format.IfBlank("{number}{ordinal} B/{year}").Inject(new { number = datetime.GetBimesterOfYear(), ordinal = datetime.GetBimesterOfYear().GetOrdinal(), year = datetime.Year, shortyear = datetime.Year.ToString().GetLastChars(2) });

        public static string DayString(this DateTime datetime, string format = null, CultureInfo culture = null) => format.IfBlank("{day}/{monthname}/{year}").Inject(new { day = datetime.Day, year = datetime.Year, month = datetime.Month, monthname = datetime.GetLongMonthName(culture), shortmonthname = datetime.GetShortMonthName(culture), shortyear = datetime.Year.ToString().GetLastChars(2) });

        public static string FortnightString(this DateTime datetime, string format = null, CultureInfo culture = null) => format.IfBlank("{number}{ordinal} F/{monthname}/{year}").Inject(new { number = datetime.GetFortnightNumber(), ordinal = datetime.GetFortnightNumber().GetOrdinal(), shortyear = datetime.Year.ToString().GetLastChars(2), year = datetime.Year, month = datetime.Month, monthname = datetime.GetLongMonthName(culture), shortmonthname = datetime.GetShortMonthName(culture) });

        public static string MonthString(this DateTime datetime, string format = null, CultureInfo culture = null) => format.IfBlank("{monthname}/{year}").Inject(new { year = datetime.Year, month = datetime.Month, monthname = datetime.GetLongMonthName(culture), shortmonthname = datetime.GetShortMonthName(culture), shortyear = datetime.Year.ToString().GetLastChars(2) });

        public static string QuarterString(this DateTime datetime, string format = null) => format.IfBlank("{number}{ordinal} Q/{year}").Inject(new { number = datetime.GetQuarterOfYear(), ordinal = datetime.GetQuarterOfYear().GetOrdinal(), year = datetime.Year, shortyear = datetime.Year.ToString().GetLastChars(2) });

        public static string SemesterString(this DateTime datetime, string format = null) => format.IfBlank("{number}{ordinal} S/{year}").Inject(new { number = datetime.GetSemesterOfYear(), ordinal = datetime.GetSemesterOfYear().GetOrdinal(), year = datetime.Year, shortyear = datetime.Year.ToString().GetLastChars(2) });

        public static string WeekString(this DateTime datetime, string format = null, CultureInfo culture = null) => datetime.GetWeekInfo(culture, format).WeekString;

        public static string YearString(this DateTime datetime, string format = null) => format.IfBlank("{year}").Inject(new { year = datetime.Year, shortyear = datetime.Year.ToString().GetLastChars(2) });

        #endregion DateStrings
    }

    public class PeriodFormat
    {
        #region Public Properties

        public string BimesterFormat { get; set; }
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;
        public string DayFormat { get; set; }
        public string FortnightFormat { get; set; }
        public string MonthFormat { get; set; }
        public string QuarterFormat { get; set; }
        public string SemesterFormat { get; set; }
        public string WeekFormat { get; set; }
        public string YearFormat { get; set; }

        #endregion Public Properties

        #region Public Methods

        public string Format(DateRange Date, string Group, string format, bool simplify = false) => Format(Date.StartDate, Date.EndDate, Group, format, simplify);

        public string Format(DateTime StartDate, DateTime EndDate, string Group, string format, bool simplify = false)
        {
            Misc.FixOrder(ref StartDate, ref EndDate);
            var labels = new string[] { Format(StartDate, Group), Format(EndDate, Group) };
            if (simplify) { labels = labels.ReduceToDifference().DefaultIfEmpty(Group).ToArray(); }
            var oo = new { start = labels.FirstOrDefault(), end = labels.LastOrDefault() };
            return oo.start == oo.end ? oo.start : format.IfBlank("from {start} to {end}").Inject(oo);
        }

        public string Format(DateTime Date, string Group)
        {
            switch (Group?.ToLowerInvariant().RemoveAccents())
            {
                case "month":
                case "mensal":
                case "mes":
                    return Date.MonthString(MonthFormat);

                case "semanal":
                case "week":
                case "semana":
                    return Date.WeekString(WeekFormat, Culture);

                case "quinzenal":
                case "fortnight":
                case "quinzena":
                    return Date.FortnightString(FortnightFormat);

                case "bimestral":
                case "bimester":
                case "bimestre":
                    return Date.BimesterString(BimesterFormat);

                case "trimestral":
                case "quarter":
                case "trimestre":
                    return Date.QuarterString(QuarterFormat);

                case "semestral":
                case "halfyear":
                case "semester":
                case "semestre":
                    return Date.SemesterString(SemesterFormat);

                case "anual":
                case "year":
                case "ano":
                    return Date.YearString(YearFormat);

                case "diario":
                case "day":
                case "dia":
                default:
                    return Date.DayString(DayFormat, Culture);
            }
        }

        public string Get(string Group)
        {
            switch (Group?.ToLowerInvariant().RemoveAccents())
            {
                case "month":
                case "mensal":
                case "mes":
                    return MonthFormat;

                case "semanal":
                case "week":
                case "semana":
                    return WeekFormat;

                case "quinzenal":
                case "fortnight":
                case "quinzena":
                    return FortnightFormat;

                case "bimestral":
                case "bimester":
                case "bimestre":
                    return BimesterFormat;

                case "trimestral":
                case "quarter":
                case "trimestre":
                    return QuarterFormat;

                case "semestral":
                case "halfyear":
                case "semester":
                case "semestre":
                    return SemesterFormat;

                case "anual":
                case "year":
                case "ano":
                    return YearFormat;

                case "diario":
                case "day":
                case "dia":
                default:
                    return DayFormat;
            }
        }

        public Expression<Func<T, string>> GroupByPeriodExpression<T>(string Group, Expression<Func<T, DateTime>> prop)
        {
            var param = prop.Parameters.First();
            Group = Group.IfBlank(prop.Name);
            MethodInfo method = typeof(DateTime).GetMethod(nameof(DateTime.ToString), new Type[] { });
            MethodCallExpression exp = Expression.Call(prop.Body, method);
            Expression c = Expression.Constant(Culture ?? CultureInfo.CurrentCulture);
            switch (Group.ToLowerInvariant().RemoveAccents())
            {
                case "month":
                case "mensal":
                case "mes":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.MonthString), new[] { typeof(DateTime), typeof(string), typeof(CultureInfo) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(MonthFormat), c);
                    break;

                case "semanal":
                case "week":
                case "semana":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.WeekString), new[] { typeof(DateTime), typeof(string), typeof(CultureInfo) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(WeekFormat), c);
                    break;

                case "quinzenal":
                case "fortnight":
                case "quinzena":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.FortnightString), new[] { typeof(DateTime), typeof(string), typeof(CultureInfo) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(FortnightFormat), c);
                    break;

                case "bimestral":
                case "bimester":
                case "bimestre":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.BimesterString), new[] { typeof(DateTime), typeof(string) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(BimesterFormat));
                    break;

                case "trimestral":
                case "quarter":
                case "trimestre":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.QuarterString), new[] { typeof(DateTime), typeof(string) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(QuarterFormat));
                    break;

                case "semestral":
                case "halfyear":
                case "semester":
                case "semestre":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.SemesterString), new[] { typeof(DateTime), typeof(string) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(SemesterFormat));
                    break;

                case "anual":
                case "year":
                case "ano":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.YearString), new[] { typeof(DateTime), typeof(string) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(YearFormat));
                    break;

                case "diario":
                case "day":
                case "dia":
                    method = typeof(DateTimeExtensions).GetMethod(nameof(DateTimeExtensions.DayString), new[] { typeof(DateTime), typeof(string), typeof(CultureInfo) });
                    exp = Expression.Call(null, method, prop.Body, Expression.Constant(DayFormat), c);
                    break;

                default:
                    return Expression.Lambda<Func<T, string>>(Expression.Constant(Group), param);
            }

            return Expression.Lambda<Func<T, string>>(exp, param);
        }

        #endregion Public Methods
    }
}