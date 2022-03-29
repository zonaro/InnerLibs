using System;

namespace InnerLibs.TimeMachine
{
    public partial class DateRange
    {
        public static DateRange Bimester(DateTime? Date = null) => new DateRange(Date.OrNow().FirstDayOfBimester(), Date.OrNow().LastDayOfBimester());

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

        public static DateRange Month(DateTime? Date = null) => new DateRange(Date.OrNow().FirstDayOfMonth(), Date.OrNow().LastDayOfMonth());

        public static DateRange Now() => new DateRange();

        public static DateRange Quarter(DateTime? Date = null) => new DateRange(Date.OrNow().FirstDayOfQuarter(), Date.OrNow().LastDayOfQuarter());

        public static DateRange Semester(DateTime? Date = null) => new DateRange(Date.OrNow().FirstDayOfSemester(), Date.OrNow().LastDayOfSemester());

        public static DateRange Week(DateTime? Date = null, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday) => new DateRange(Date.OrNow().FirstDayOfWeek(FirstDayOfWeek), Date.OrNow().LastDayOfWeek(FirstDayOfWeek));

        public static DateRange Year(DateTime? Date = null) => new DateRange(Date.OrNow().FirstDayOfYear(), Date.OrNow().LastDayOfYear());
    }
}