using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerLibs.TimeMachine
{
    public partial class DateRange
    {
        public static DateRange FromTicks(long Ticks) => new DateRange(TimeSpan.FromTicks(Ticks));

        public static DateRange FromMilliseconds(double Value) => new DateRange(TimeSpan.FromMilliseconds(Value));

        public static DateRange FromSeconds(double Value) => new DateRange(TimeSpan.FromSeconds(Value));

        public static DateRange FromMinutes(double Value) => new DateRange(TimeSpan.FromMinutes(Value));

        public static DateRange FromHours(double Value) => new DateRange(TimeSpan.FromHours(Value));

        public static DateRange FromDays(double Value) => new DateRange(TimeSpan.FromDays(Value));

        public static DateRange Now(DateTime? Date) => new DateRange(Date.OrNow());

        public static DateRange Semester(DateTime? Date) => new DateRange(Date.OrNow().FirstDayOfSemester(), Date.OrNow().LastDayOfSemester());

        public static DateRange Bimester(DateTime? Date) => new DateRange(Date.OrNow().FirstDayOfBimester(), Date.OrNow().LastDayOfBimester());

        public static DateRange Quarter(DateTime? Date) => new DateRange(Date.OrNow().FirstDayOfQuarter(), Date.OrNow().LastDayOfQuarter());

        public static DateRange Year(DateTime? Date) => new DateRange(Date.OrNow().FirstDayOfYear(), Date.OrNow().LastDayOfYear());

        public static DateRange Month(DateTime? Date) => new DateRange(Date.OrNow().FirstDayOfMonth(), Date.OrNow().LastDayOfMonth());

        public static DateRange Week(DateTime? Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday) => new DateRange(Date.OrNow().FirstDayOfWeek(FirstDayOfWeek), Date.OrNow().LastDayOfWeek(FirstDayOfWeek));
    }
}