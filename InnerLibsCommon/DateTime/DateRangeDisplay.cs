using System.Linq;
using Extensions;
using Extensions.ComplexText;

namespace Extensions.Dates
{


    /// <summary>
    /// The display configuration for generate human readable strings from a <see cref="DateRange"/>
    /// </summary>
    public class DateRangeDisplay
    {
        #region Public Constructors

        public DateRangeDisplay()
        {
            foreach (var item in this.GetProperties().Where(x => x.Name.EndsWith("Word")))
            {
                var w = item.Name.RemoveLastEqual("Word").ToLowerInvariant();
                if (item.PropertyType == typeof(QuantityTextPair))
                {
                    item.SetValue(this, new QuantityTextPair(w));
                }
                else if (item.PropertyType == typeof(string))
                {
                    item.SetValue(this, w);
                }
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

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// the "And" word, use to concatenate the last item in the string
        /// </summary>
        public string AndWord { get; set; }

        /// <summary>
        /// The 'Days' word (on plural).
        /// </summary>
        public QuantityTextPair DaysWord { get; set; }

        /// <summary>
        /// Rules for returning the string
        /// </summary>
        public DateRangeFormatRules FormatRule { get; set; } = DateRangeFormatRules.FullStringSkipZero;

        /// <summary>
        /// The 'Hours' word.
        /// </summary>
        public QuantityTextPair HoursWord { get; set; }

        /// <summary>
        /// The 'Milliseconds' word.
        /// </summary>
        public QuantityTextPair MillisecondsWord { get; set; }

        /// <summary>
        /// The 'Minutes' word.
        /// </summary>
        public QuantityTextPair MinutesWord { get; set; }

        /// <summary>
        /// The 'Months' word.
        /// </summary>
        public QuantityTextPair MonthsWord { get; set; }

        /// <summary>
        /// The 'Seconds' word.
        /// </summary>
        public QuantityTextPair SecondsWord { get; set; }

        /// <summary>
        /// The 'Years' word.
        /// </summary>
        public QuantityTextPair YearsWord { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static DateRangeDisplay Default() => new DateRangeDisplay();

        public static DateRangeDisplay DefaultPtBr() => new DateRangeDisplay("e", "milisegundos", "segundos", "minutos", "horas", "dias", "meses;mes", "anos");

        public override string ToString() => new string[] { YearsWord.Plural, MonthsWord.Plural, DaysWord.Plural, HoursWord.Plural, MinutesWord.Plural, SecondsWord.Plural, MillisecondsWord.Plural }.ToPhrase(Util.EmptyString, AndWord);

        #endregion Public Methods
    }

}