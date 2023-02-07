using System;
using System.Globalization;

namespace InnerLibs
{
    public struct WeekInfo : IEquatable<WeekInfo>
    {
        #region Public Constructors

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

            WeekString = stringFormat.Inject(new { weekofyear = WeekOfYear, weekofmonth = WeekOfMonth, monthordinal = WeekOfMonth.GetOrdinal(), yearordinal = WeekOfYear.GetOrdinal(), year = Year, month = FirstDayOfWeek.Month, monthname = FirstDayOfWeek.Month.ToLongMonthName(culture), shortmonthname = FirstDayOfWeek.Month.ToShortMonthName(culture), shortyear = FirstDayOfWeek.Year.ToString(culture).GetLastChars(2) });
        }



        #endregion Public Constructors

        #region Public Indexers

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

        #endregion Public Indexers

        #region Public Properties

        public DateTime FirstDayOfWeek { get; private set; }
        public DateTime LastDayOfWeek { get; private set; }
        public int Month { get; private set; }
        public int WeekOfMonth { get; private set; }
        public int WeekOfYear { get; private set; }
        public string WeekString { get; private set; }
        public int Year { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public static implicit operator int[](WeekInfo Info) => Info.ToInt32Array();

        public static implicit operator string(WeekInfo Info) => Info.ToString();

        public static bool operator !=(WeekInfo left, WeekInfo right)
        {
            return !(left == right);
        }

        public static bool operator ==(WeekInfo left, WeekInfo right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (obj is WeekInfo info)
            {
                return Equals(info);
            }
            else if (obj is int i)
            {
                return GetHashCode().Equals(i);
            }
            else if (obj is int[] ia)
            {
                return GetHashCode().Equals(ia.GetHashCode());
            }
            return false;
        }

        public bool Equals(WeekInfo other) => other != null && this.GetHashCode() == other.GetHashCode();

        public override int GetHashCode() => ((int[])this).GetHashCode();

        public override string ToString() => WeekString;

        public int[] ToInt32Array() => new int[] { this.WeekOfMonth, this.Month, this.Year, this.WeekOfYear };

        #endregion Public Methods
    }
}