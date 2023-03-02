using System;
using System.Globalization;
using Extensions;

namespace Dates
{
    public partial class DateRange
    {
        #region Public Methods

        public static DateRange Bimester(DateTime? Date = null) => new DateRange(Date.OrNow().BeginningOfBimester(), Date.OrNow().EndOfBimester());

        public static DateRange Day(DateTime? Date = null) => new DateRange(Date.OrNow().BeginningOfDay(), Date.OrNow().EndOfDay());

        public static DateRange Fortnight(DateTime? Date = null) => new DateRange(Date.OrNow().BeginningOfFortnight(), Date.OrNow().EndOfFortnight());

        public static DateRange FromDate(DateTime? Date = null) => new DateRange(Date.OrNow());

        public static DateRange FromDays(double Value) => new DateRange(TimeSpan.FromDays(Value));

        public static DateRange FromDays(DateTime StartDate, double Value) => new DateRange(StartDate, TimeSpan.FromDays(Value));

        public static DateRange FromHours(double Value) => new DateRange(TimeSpan.FromHours(Value));

        public static DateRange FromHours(DateTime StartDate, double Value) => new DateRange(StartDate, TimeSpan.FromHours(Value));

        public static DateRange FromMilliseconds(double Value) => new DateRange(TimeSpan.FromMilliseconds(Value));

        public static DateRange FromMilliseconds(DateTime StartDate, double Value) => new DateRange(StartDate, TimeSpan.FromMilliseconds(Value));

        public static DateRange FromMinutes(double Value) => new DateRange(TimeSpan.FromMinutes(Value));

        public static DateRange FromMinutes(DateTime StartDate, double Value) => new DateRange(StartDate, TimeSpan.FromMinutes(Value));

        public static DateRange FromSeconds(double Value) => new DateRange(TimeSpan.FromSeconds(Value));

        public static DateRange FromSeconds(DateTime StartDate, double Value) => new DateRange(StartDate, TimeSpan.FromSeconds(Value));

        public static DateRange FromTicks(long Ticks) => new DateRange(TimeSpan.FromTicks(Ticks));

        public static DateRange FromTicks(DateTime StartDate, long Ticks) => new DateRange(StartDate, TimeSpan.FromTicks(Ticks));

        public static DateRange Month(DateTime? Date = null) => new DateRange(Date.OrNow().BeginningOfMonth(), Date.OrNow().EndOfMonth());

        public static DateRange Now() => new DateRange();

        public static DateRange Quarter(DateTime? Date = null) => new DateRange(Date.OrNow().BeginningOfQuarter(), Date.OrNow().EndOfQuarter());

        public static DateRange Semester(DateTime? Date = null) => new DateRange(Date.OrNow().BeginningOfSemester(), Date.OrNow().EndOfSemester());

        public static DateRange Week(DateTime? Date = null) => Week(Date, null);

        public static DateRange Week(DateTime? Date, DayOfWeek FirstDayOfWeek) => new DateRange(Date.OrNow().FirstDayOfWeek(FirstDayOfWeek), Date.OrNow().LastDayOfWeek(FirstDayOfWeek));

        public static DateRange Week(DateTime? Date, CultureInfo culture = null) => new DateRange(Date.OrNow().FirstDayOfWeek(culture), Date.OrNow().LastDayOfWeek(culture));

        public static DateRange Year(DateTime? Date = null) => new DateRange(Date.OrNow().BeginningOfYear(), Date.OrNow().EndOfYear());

        #endregion Public Methods
    }
}
