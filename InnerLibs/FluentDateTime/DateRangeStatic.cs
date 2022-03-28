using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnerLibs.TimeMachine
{
    public partial class DateRange
    {
        public static DateRange Semester(DateTime? Date) => new DateRange(Date.OrNow().GetFirstDayOfSemester(), Date.OrNow().GetLastDayOfSemester());

        public static DateRange Bimester(DateTime? Date) => new DateRange(Date.OrNow().GetFirstDayOfBimester(), Date.OrNow().GetLastDayOfBimester());

        public static DateRange Quarter(DateTime? Date) => new DateRange(Date.OrNow().GetFirstDayOfQuarter(), Date.OrNow().GetLastDayOfQuarter());

        public static DateRange Year(DateTime? Date) => new DateRange(Date.OrNow().GetFirstDayOfYear(), Date.OrNow().GetLastDayOfYear());

        public static DateRange Week(DateTime? Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday) => new DateRange(Date.OrNow().GetFirstDayOfWeek(FirstDayOfWeek), Date.OrNow().GetLastDayOfWeek(FirstDayOfWeek));
    }
}