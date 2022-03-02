using InnerLibs.TimeMachine;
using System;

namespace InnerLibs.TimeMachine
{
    public class WeekInfo
    {
        public WeekInfo(DateTime DateAndTime)
        {
            DateAndTime = DateAndTime.Date;
            var firstMonthDay = DateAndTime.GetFirstDayOfMonth();
            var firstMonthMonday = firstMonthDay.AddDays(((int)DayOfWeek.Monday + 7 - (int)firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > DateAndTime)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays(((int)DayOfWeek.Monday + 7 - (int)firstMonthDay.DayOfWeek) % 7);
            }

            Week = (int)Math.Round((DateAndTime - firstMonthMonday).Days / 7d + 1d);
            Month = firstMonthDay.Month;
            Year = firstMonthDay.Year;
        }

        public int Week { get; private set; }
        public int Month { get; private set; }

        public int Year { get; private set; }

        public int this[int Index]
        {
            get
            {
                switch (Index)
                {
                    case 0: return this.Week;
                    case 1: return this.Month;
                    case 2: return this.Year;
                    default: return -1;
                }
            }
        }

        public static implicit operator int[](WeekInfo Info) => new int[] { Info?.Week ?? -1, Info?.Month ?? -1, Info?.Year ?? -1 };

        public override string ToString()
        {
            return $"{Week}/{Month}/{Year}";
        }
    }
}