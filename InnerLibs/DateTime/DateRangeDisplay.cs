using System.Linq;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// The display configuration for generate human readable strings from a <see cref="DateRange"/>
    /// </summary>
    public class DateRangeDisplay
    {
        public DateRangeDisplay()
        {
            foreach (var item in this.GetProperties().Where(x => x.Name.EndsWith("Word")))
            {
                item.SetValue(this, item.Name.RemoveLastEqual("Word").ToLower());
            }
        }

        public DateRangeDisplay(DateRangeFormatRules FormatRule) : base() => this.FormatRule = FormatRule;

        /// <param name="AndWord">the "And" word, use to concatenate the last item in the string</param>
        /// <param name="YearsWord">'Years'</param>
        /// <param name="MonthsWord">'Months'</param>
        /// <param name="DaysWord">'Days'</param>
        /// <param name="HoursWord">'Hours'</param>
        /// <param name="MinutesWord">'Minutes'</param>
        /// <param name="SecondsWord">'Seconds'</param>
        /// <param name="MillisecondsWord">'Milliseconds'</param>
        /// <param name="FormatRule">Rules for returning the string</param>
        /// <remarks>Keep all string in plural form</remarks>
        public DateRangeDisplay(string AndWord, string MillisecondsWord, string SecondsWord, string MinutesWord, string HoursWord, string DaysWord, string MonthsWord, string YearsWord, DateRangeFormatRules FormatRule = DateRangeFormatRules.FullStringSkipZero)
        {
            this.AndWord = AndWord;
            this.MillisecondsWord = MillisecondsWord;
            this.SecondsWord = SecondsWord;
            this.MinutesWord = MinutesWord;
            this.HoursWord = HoursWord;
            this.DaysWord = DaysWord;
            this.MonthsWord = MonthsWord;
            this.YearsWord = YearsWord;
            this.FormatRule = FormatRule;
        }

        /// <summary>
        /// the "And" word, use to concatenate the last item in the string
        /// </summary>
        public string AndWord { get; set; }

        /// <summary>
        /// The 'Days' word (on plural).
        /// </summary>
        public string DaysWord { get; set; }

        /// <summary>
        /// Rules for returning the string
        /// </summary>
        public DateRangeFormatRules FormatRule { get; set; } = DateRangeFormatRules.FullStringSkipZero;

        /// <summary>
        /// The 'Hours' word (on plural).
        /// </summary>
        public string HoursWord { get; set; }

        /// <summary>
        /// The 'Milliseconds' word (on plural).
        /// </summary>
        public string MillisecondsWord { get; set; }

        /// <summary>
        /// The 'Minutes' word (on plural).
        /// </summary>
        public string MinutesWord { get; set; }

        /// <summary>
        /// The 'Months' word (on plural).
        /// </summary>
        public string MonthsWord { get; set; }

        /// <summary>
        /// The 'Seconds' word (on plural).
        /// </summary>
        public string SecondsWord { get; set; }

        /// <summary>
        /// The 'Years' word (on plural).
        /// </summary>
        public string YearsWord { get; set; }

        public static DateRangeDisplay Default() => new DateRangeDisplay();

        public static DateRangeDisplay DefaultPtBr() => new DateRangeDisplay("e", "milisegundos", "segundos", "minutos", "horas", "dias", "meses", "anos");
    }
}