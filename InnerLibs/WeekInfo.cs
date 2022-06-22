using System;
using System.Globalization;

namespace InnerLibs.TimeMachine
{
    public struct WeekInfo
    {
        public WeekInfo(int WeekOfMonth, int Month, int Year, CultureInfo culture = null, string stringFormat = null) : this(new DateTime(Year, Month, 1) + WeekOfMonth.Weeks(), culture, stringFormat)
        {

        }
        public WeekInfo(DateTime? DateAndTime, CultureInfo culture = null, string stringFormat = null)
        {
            DateAndTime = DateAndTime.OrNow();
            culture = culture ?? CultureInfo.CurrentCulture;
            stringFormat = stringFormat.IfBlank("{weekofmonth}{monthordinal}/{monthname} - {weekofyear}{yearordinal}/{year}");
            DateTime firstMonthDay = DateAndTime.Value.BeginningOfMonth();



            DateTime firstDayOfWeekOfMonth = firstMonthDay.AddDays((culture.DateTimeFormat.FirstDayOfWeek + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstDayOfWeekOfMonth > DateAndTime.Value)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstDayOfWeekOfMonth = firstMonthDay.AddDays((culture.DateTimeFormat.FirstDayOfWeek + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            WeekOfMonth = (DateAndTime.Value - firstDayOfWeekOfMonth).Days / 7 + 1;
            Month = firstMonthDay.Month;
            Year = firstMonthDay.Year;

            FirstDayOfWeek = DateAndTime.Value.FirstDayOfWeek(culture);
            LastDayOfWeek = DateAndTime.Value.LastDayOfWeek(culture);

            WeekOfYear = culture.Calendar.GetWeekOfYear(DateAndTime.Value, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);

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