using System;
using System.Globalization;

namespace InnerLibs.TimeMachine
{
    public struct WeekInfo
    {
        public WeekInfo(DateTime? DateAndTime, CultureInfo culture = null, string stringFormat = null)
        {
            LastDayOfWeek = DateAndTime.OrNow();
            culture = culture ?? CultureInfo.CurrentCulture;
            stringFormat = stringFormat.IfBlank("{weekofmonth}{monthordinal}/{monthname} - {weekofyear}{yearordinal}/{year}");
            DateTime beginningOfMonth = LastDayOfWeek.BeginningOfMonth();
            Month = beginningOfMonth.Month;
            Year = beginningOfMonth.Year;
            while (LastDayOfWeek.Date.AddDays(1).DayOfWeek != culture.DateTimeFormat.FirstDayOfWeek)
            {
                LastDayOfWeek = LastDayOfWeek.AddDays(1);
            }

            FirstDayOfWeek = LastDayOfWeek.FirstDayOfWeek(culture);

            WeekOfMonth = (int)Math.Truncate((double)DateAndTime.Value.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
            WeekOfYear = culture.Calendar.GetWeekOfYear(LastDayOfWeek, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);

            WeekString = stringFormat.Inject(new { weekofyear = WeekOfYear, weekofmonth = WeekOfMonth, monthordinal = WeekOfMonth.GetOrdinal(), yearordinal = Year.GetOrdinal(), year = Year, month = FirstDayOfWeek.Month, monthname = FirstDayOfWeek.Month.ToLongMonthName(culture), shortmonthname = FirstDayOfWeek.Month.ToShortMonthName(culture), shortyear = FirstDayOfWeek.Year.ToString().GetLastChars(2) });
        }


        public int Month { get; private set; }
        public int WeekOfMonth { get; private set; }
        public int WeekOfYear { get; private set; }
        public int Year { get; private set; }
        public string WeekString { get; private set; }
        public DateTime FirstDayOfWeek { get; private set; }
        public DateTime LastDayOfWeek { get; private set; }

        public int this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return WeekOfMonth;
                    case 1: return Month;
                    case 2: return Year;
                    case 3: return WeekOfYear;
                    default: return -1;
                }
            }
        }

        public static implicit operator int[](WeekInfo Info) => new int[] { Info.WeekOfMonth, Info.Month, Info.Year, Info.WeekOfYear };
        public static implicit operator string(WeekInfo Info) => Info.ToString();


        public override string ToString() => WeekString;
    }
}