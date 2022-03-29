using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// Works like a positive <see cref="System.TimeSpan"/> with validation of Relevant (Business)
    /// Days and many other <see cref="DateTime"/> functions
    /// </summary>
    public partial class DateRange : IEquatable<DateRange>, IComparable<TimeSpan>, IComparable<DateRange>, ICloneable
    {
        /// <summary>
        /// Create a new <see cref="DateRange"/> for today (from 00:00:00.000 to 23:59:59.999)
        /// </summary>
        public DateRange() : this(DateTime.Now, DateTime.Now, true) { }

        public DateRange(RoundTo RoundTo) : this(DateTime.Now, RoundTo)
        {
        }

        public DateRange(DateTime StartDate, RoundTo RoundTo) : this(StartDate, StartDate, true, RoundTo)
        {
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> to <paramref name="EndDate"/>
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="ForceFirstAndLastMoments">
        /// Force <paramref name="StartDate"/> to the first moment of day (Midnight) and <paramref
        /// name="EndDate"/> to last moment of day (23:59:59.999)
        /// </param>
        public DateRange(DateTime StartDate, DateTime EndDate, bool ForceFirstAndLastMoments) : this(StartDate, EndDate) => this.ForceFirstAndLastMoments = ForceFirstAndLastMoments;

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> to <paramref name="EndDate"/>
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="ForceFirstAndLastMoments">
        /// Force <see cref="StartDate"/> to the first moment of day (Midnight) and <see
        /// cref="EndDate"/> to last moment of day (23:59:59.999)
        /// </param>
        public DateRange(DateTime StartDate, DateTime EndDate, params DayOfWeek[] RelevantDaysOfWeek) : this(StartDate, EndDate, false, RoundTo.None, RelevantDaysOfWeek)
        {
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> to <paramref name="EndDate"/>
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="ForceFirstAndLastMoments">
        /// Force <see cref="StartDate"/> to the first moment of day (Midnight) and <see
        /// cref="EndDate"/> to last moment of day (23:59:59.999)
        /// </param>
        public DateRange(DateTime StartDate, DateTime EndDate, RoundTo RoundTo, params DayOfWeek[] RelevantDaysOfWeek) : this(StartDate, EndDate, false, RoundTo, RelevantDaysOfWeek)
        {
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> to <paramref name="EndDate"/>
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="ForceFirstAndLastMoments">
        /// Force <see cref="StartDate"/> to the first moment of day (Midnight) and <see
        /// cref="EndDate"/> to last moment of day (23:59:59.999)
        /// </param>
        /// <param name="RelevantDaysOfWeek">Business Days of Week</param>
        public DateRange(DateTime StartDate, DateTime EndDate, bool ForceFirstAndLastMoments, RoundTo RoundTo, params DayOfWeek[] RelevantDaysOfWeek)
        {
            if (RelevantDaysOfWeek == null || !RelevantDaysOfWeek.Any())
            {
                RelevantDaysOfWeek = PredefinedArrays.SundayToSaturday.ToArray();
            }

            this.RelevantDaysOfWeek = RelevantDaysOfWeek.ToList();
            _roundTo = RoundTo;
            _forceFirstAndLastMoments = ForceFirstAndLastMoments;
            _startDate = StartDate;
            _endDate = EndDate;
            CalcRange();
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <see cref="DateTime.MinValue"/> plus <paramref name="Span"/>
        /// </summary>
        /// <param name="Span"></param>
        public DateRange(TimeSpan Span) : this(DateTime.MinValue, Span)
        {
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> plus <paramref name="Span"/>
        /// </summary>
        /// <param name="StartDate">Start date</param>
        /// <param name="Span">Interval</param>
        public DateRange(DateTime StartDate, TimeSpan Span) : this(StartDate, StartDate.Add(Span))
        {
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> plus <paramref name="Span"/>
        /// </summary>
        /// <param name="StartDate">Start date</param>
        /// <param name="Span">Interval</param>
        public DateRange(DateTime StartDate, TimeSpan Span, RoundTo RoundTo) : this(StartDate, StartDate.Add(Span), RoundTo)
        {
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> plus <paramref name="Span"/>
        /// </summary>
        /// <param name="StartDate">Start date</param>
        /// <param name="Span">Interval</param>
        public DateRange(DateTime StartDate, TimeSpan Span, bool ForceFirstAndLastMoments) : this(StartDate, StartDate.Add(Span), ForceFirstAndLastMoments)
        {
        }

        /// <summary>
        /// Create a new <see cref="DateRange"/> from <paramref name="StartDate"/> plus <paramref name="Span"/>
        /// </summary>
        /// <param name="StartDate">Start date</param>
        /// <param name="Span">Interval</param>
        public DateRange(DateTime StartDate, TimeSpan Span, bool ForceFirstAndLastMoments, RoundTo RoundTo) : this(StartDate, StartDate.Add(Span), ForceFirstAndLastMoments, RoundTo)
        {
        }

        /// <summary>
        /// Create a new instance of <see cref="DateRange"/> using the smallest and largest date
        /// from a list
        /// </summary>
        /// <param name="Dates"></param>
        public DateRange(IEnumerable<DateTime?> Dates) : this(Dates?.Where(x => x.HasValue).Select(x => x.Value))
        {
        }

        /// <summary>
        /// create a new instance of <see cref="DateRange"/> using the smallest and largest date
        /// from a list
        /// </summary>
        /// <param name="Dates"></param>
        public DateRange(IEnumerable<DateTime?> Dates, bool ForceFirstAndLastMoments) : this(Dates) => this.ForceFirstAndLastMoments = ForceFirstAndLastMoments;

        public DateRange(DateTime StartEndDate) : this(StartEndDate, StartEndDate, true)
        {
        }

        public DateRange(DateTime? StartEndDate) : this(StartEndDate.OrNow())
        {
        }

        /// <summary>
        /// create a new instance of <see cref="DateRange"/> using the smallest and largest date
        /// from a list
        /// </summary>
        /// <param name="Dates"></param>
        public DateRange(IEnumerable<DateTime> Dates) : this(Dates, false)
        {
        }

        /// <summary>
        /// create a new instance of <see cref="DateRange"/> using the smallest and largest date
        /// from a list
        /// </summary>
        /// <param name="Dates"></param>
        public DateRange(IEnumerable<DateTime> Dates, bool ForceFirstAndLastMoments) : this(Dates.Min(), Dates.Max(), ForceFirstAndLastMoments)
        { }

        private void CalcRange()
        {
            Calendars.FixDateOrder(ref _startDate, ref _endDate);

            int days = 0;
            int years = 0;
            int months = 0;
            var _phase = Phase.Years;

            if (ForceFirstAndLastMoments)
            {
                _startDate = _startDate.Date;
                _endDate = _endDate.EndOfDay();
            }

            this._timeSpanBase = _endDate.Round(this.RoundTo) - _startDate.Round(this.RoundTo);

            var CurDate = _startDate.Round(this.RoundTo);

            while (_phase != Phase.Done)
            {
                switch (_phase)
                {
                    case Phase.Years:
                        {
                            if (CurDate.AddYears(years + 1) > _endDate.Round(this.RoundTo))
                            {
                                _phase = Phase.Months;
                                CurDate = CurDate.AddYears(years);
                            }
                            else
                            {
                                years++;
                            }

                            break;
                        }

                    case Phase.Months:
                        {
                            if (CurDate.AddMonths(months + 1) > _endDate.Round(this.RoundTo))
                            {
                                _phase = Phase.Days;
                                CurDate = CurDate.AddMonths(months);
                            }
                            else
                            {
                                months++;
                            }

                            break;
                        }

                    case Phase.Days:
                        {
                            if (CurDate.AddDays(days + 1) > _endDate.Round(this.RoundTo))
                            {
                                CurDate = CurDate.AddDays(days);

                                Years = years;
                                Months = months;
                                Days = days;
                                //Hours = timespan.Hours;
                                //Minutes = timespan.Minutes;
                                //Seconds = timespan.Seconds;
                                //Milliseconds = timespan.Milliseconds;

                                _phase = Phase.Done;
                            }
                            else
                            {
                                days++;
                            }

                            break;
                        }
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;

            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    CalcRange();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;

            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    CalcRange();
                }
            }
        }

        private bool _forceFirstAndLastMoments = false;
        private RoundTo _roundTo = RoundTo.None;

        /// <summary>
        /// When <b>TRUE</b>, force <see cref="StartDate"/> to the first moment of day (Midnight)
        /// and <see cref="EndDate"/> to last moment of day (23:59:59.999)
        /// </summary>
        public bool ForceFirstAndLastMoments
        {
            get => _forceFirstAndLastMoments;
            set { _forceFirstAndLastMoments = value; CalcRange(); }
        }

        public RoundTo RoundTo
        {
            get => _roundTo;
            set { _roundTo = value; CalcRange(); }
        }

        /// <summary>
        /// Total Milliseconds between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalMilliseconds => TimeSpan.TotalMilliseconds;

        /// <summary>
        /// Total Seconds between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalSeconds => TimeSpan.TotalSeconds;

        /// <summary>
        /// Total Minutes between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalMinutes => TimeSpan.TotalMinutes;

        /// <summary>
        /// Total Hours between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalHours => TimeSpan.TotalHours;

        /// <summary>
        /// Total Days between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalDays => TimeSpan.TotalDays;

        /// <summary>
        /// Total Weeks between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalWeeks => (TotalDays / 7d);

        /// <summary>
        /// Total Months between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalMonths => TotalDays / (365.25 / 12d);

        /// <summary>
        /// Total Bimesters between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalBimesters => TotalMonths / 2d;

        /// <summary>
        /// Total Quarters between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalQuarters => TotalMonths / 3d;

        /// <summary>
        /// Total Semesters between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalSemesters => TotalMonths / 6d;

        /// <summary>
        /// Total Years between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public double TotalYears => (TotalMonths / 12d);

        /// <summary>
        /// Total Ticks between <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public long Ticks => TimeSpan.Ticks;

        /// <summary>
        /// Relevant days of week (Business Days)
        /// </summary>
        /// <returns></returns>
        public List<DayOfWeek> RelevantDaysOfWeek { get; private set; } = new List<DayOfWeek>();

        /// <summary>
        /// Non relevant days of week (not Business Days)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayOfWeek> NonRelevantDaysOfWeek => PredefinedArrays.SundayToSaturday.Where(x => x.IsNotIn(RelevantDaysOfWeek));

        /// <summary>
        /// Years
        /// </summary>
        /// <returns></returns>
        public int Years { get; private set; }

        /// <summary>
        /// Months
        /// </summary>
        /// <returns></returns>
        public int Months { get; private set; }

        /// <summary>
        /// Numero de Dias
        /// </summary>
        /// <returns></returns>
        public int Days { get; private set; }

        /// <summary>
        /// Numero de Horas
        /// </summary>
        /// <returns></returns>
        public int Hours => TimeSpan.Hours;

        /// <summary>
        /// Numero de Minutos
        /// </summary>
        /// <returns></returns>
        public int Minutes => TimeSpan.Minutes;

        /// <summary>
        /// Numero de Segundos
        /// </summary>
        /// <returns></returns>
        public int Seconds => TimeSpan.Seconds;

        /// <summary>
        /// Numero de milisegundos
        /// </summary>
        /// <returns></returns>
        public int Milliseconds => TimeSpan.Milliseconds;

        /// <summary>
        /// Dias relevantes entre as datas Inicial e Final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetRelevantDays() => GetDays(RelevantDaysOfWeek);

        /// <summary>
        /// Dias não relevantes entre as datas Inicial e Final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetNonRelevantDays() => GetDays(NonRelevantDaysOfWeek);

        /// <summary>
        /// Convert a <see cref="DateRange"/> to a human readable string using default <see cref="DateRangeDisplay"/>.
        /// </summary>
        /// <returns></returns>
        public string ToDisplayString() => ToDisplayString(null);

        /// <summary>
        /// Convert a <see cref="DateRange"/> to a human readable string.
        /// </summary>
        ///<param name="display">The display configuration. If <b>null</b>, uses <see cref="Display"/></param>
        /// <returns></returns>
        public string ToDisplayString(DateRangeDisplay display)
        {
            display = display ?? _display;

            string ano = Text.QuantifyText(display.YearsWord, Years).Prepend($"{Years} ").NullIf(x => display.YearsWord.IsBlank());
            string mes = Text.QuantifyText(display.MonthsWord, Months).Prepend($"{Months} ").NullIf(x => display.MonthsWord.IsBlank());
            string dia = Text.QuantifyText(display.DaysWord, Days).Prepend($"{Days} ").NullIf(x => display.DaysWord.IsBlank());
            string horas = Text.QuantifyText(display.HoursWord, Hours).Prepend($"{Hours} ").NullIf(x => display.HoursWord.IsBlank());
            string minutos = Text.QuantifyText(display.MinutesWord, Minutes).Prepend($"{Minutes} ").NullIf(x => display.MinutesWord.IsBlank());
            string segundos = Text.QuantifyText(display.SecondsWord, Seconds).Prepend($"{Seconds} ").NullIf(x => display.SecondsWord.IsBlank());
            string milisegundos = Text.QuantifyText(display.MillisecondsWord, Milliseconds).Prepend($"{Milliseconds} ").NullIf(x => display.MillisecondsWord.IsBlank());

            var flagInt = (int)display.FormatRule;
            if (flagInt >= 1) //skip zero
            {
                ano = ano.NullIf(x => Years == 0);
                mes = mes.NullIf(x => Months == 0);
                dia = dia.NullIf(x => Days == 0);
                horas = horas.NullIf(x => Hours == 0);
                minutos = minutos.NullIf(x => Minutes == 0);
                segundos = segundos.NullIf(x => Seconds == 0);
                milisegundos = milisegundos.NullIf(x => Milliseconds == 0);
            }

            if (flagInt >= 2) // reduce days
            {
                horas = horas.NullIf(x => Days >= 1);
                minutos = minutos.NullIf(x => Days >= 1);
                segundos = segundos.NullIf(x => Days >= 1);
                milisegundos = milisegundos.NullIf(x => Days >= 1);
            }

            if (flagInt >= 3) //reduce months
            {
                dia = dia.NullIf(x => Months >= 1);
                horas = horas.NullIf(x => Months >= 1);
                minutos = minutos.NullIf(x => Months >= 1);
                segundos = segundos.NullIf(x => Months >= 1);
                milisegundos = milisegundos.NullIf(x => Months >= 1);
            }

            if (flagInt >= 4) // reduce years
            {
                mes = mes.NullIf(x => Years >= 1);
                dia = dia.NullIf(x => Years >= 1);
                horas = horas.NullIf(x => Years >= 1);
                minutos = minutos.NullIf(x => Years >= 1);
                segundos = segundos.NullIf(x => Years >= 1);
                milisegundos = milisegundos.NullIf(x => Years >= 1);
            }

            string current = new[] { ano, mes, dia, horas, minutos, segundos, milisegundos }.Where(x => x.IsNotBlank()).ToPhrase(display.AndWord);

            return current.AdjustWhiteSpaces();
        }

        private DateRangeDisplay _display = null;

        /// <summary>
        /// String configurations for <see cref="DateRange.ToDisplayString(DateRangeDisplay)"/>
        /// </summary>
        public DateRangeDisplay Display
        {
            get
            {
                if (_display == null)
                {
                    _display = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.AsIf(x => x.ToLower() == "pt", DateRangeDisplay.DefaultPtBr(), DateRangeDisplay.Default());
                }
                return _display;
            }
        }

        /// <summary>
        /// Convert a <see cref="DateRange"/> to a human readable string using default <see cref="DateRangeDisplay"/>.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToDisplayString();

        private enum Phase
        {
            Years,
            Months,
            Days,
            Done
        }

        /// <summary>
        /// Adds two <see cref="DateRange"/> according operator +.
        /// </summary>
        /// <param name="number">The number to add to this <see cref="DateRange"/>.</param>
        /// <returns>The result of the addition.</returns>
        public DateRange Add(DateRange number) => AddInternal(this, number);

        /// <summary>
        /// Returns a new <see cref="DateRange"/> that adds the value of the specified <see
        /// cref="System.TimeSpan"/> to the value of this instance.
        /// </summary>
        /// <param name="timeSpan">The <see cref="System.TimeSpan"/> to add to this <see cref="DateRange"/>.</param>
        /// <returns>The result of the addition.</returns>
        public DateRange Add(TimeSpan timeSpan) => AddInternal(this, timeSpan);

        /// <summary>
        /// Subtracts the number according operator -.
        /// </summary>
        /// <param name="DateRange">The matrix to subtract from this <see cref="DateRange"/>.</param>
        /// <returns>The result of the subtraction.</returns>
        public DateRange Subtract(DateRange DateRange) => SubtractInternal(this, DateRange);

        /// <summary>
        /// Returns a new <see cref="DateRange"/> that subtracts the value of the specified <see
        /// cref="System.TimeSpan"/> to the value of this instance.
        /// </summary>
        /// <param name="timeSpan">
        /// The <see cref="System.TimeSpan"/> to subtract from this <see cref="DateRange"/>.
        /// </param>
        /// <returns>The result of the subtraction.</returns>
        public DateRange Subtract(TimeSpan timeSpan) => SubtractInternal(this, timeSpan);

        /// <summary>
        /// Overload of the operator +
        /// </summary>
        /// <param name="left">The left hand <see cref="DateRange"/>.</param>
        /// <param name="right">The right hand <see cref="DateRange"/>.</param>
        /// <returns>The result of the addition.</returns>
        public static DateRange operator +(DateRange left, DateRange right) => AddInternal(left, right);

        public static DateRange operator +(DateRange left, TimeSpan right) => AddInternal(left, right);

        public static DateRange operator +(TimeSpan left, DateRange right) => AddInternal(left, right);

        /// <summary>
        /// Overload of the operator -
        /// </summary>
        /// <param name="left">The left hand <see cref="DateRange"/>.</param>
        /// <param name="right">The right hand <see cref="DateRange"/>.</param>
        /// <returns>The result of the subtraction.</returns>
        public static DateRange operator -(DateRange left, DateRange right) => SubtractInternal(left, right);

        public static DateRange operator -(TimeSpan left, DateRange right) => SubtractInternal(left, right);

        public static DateRange operator -(DateRange left, TimeSpan right) => SubtractInternal(left, right);

        /// <summary>
        /// Equals operator.
        /// </summary>
        /// <param name="left">The left hand side.</param>
        /// <param name="right">The right hand side.</param>
        /// <returns>
        /// <c>true</c> is <paramref name="left"/> is equal to <paramref name="right"/>; otherwise <c>false</c>.
        /// </returns>
        public static bool operator ==(DateRange left, DateRange right)
        {
            return left.Years == right.Years &&
                   left.Months == right.Months &&
                   left.Days == right.Days &&
                   left.Hours == right.Hours &&
                   left.Minutes == right.Minutes &&
                   left.Seconds == right.Seconds &&
                   left.Milliseconds == right.Milliseconds;
        }

        public static bool operator ==(TimeSpan left, DateRange right) => (DateRange)left == right;

        public static bool operator ==(DateRange left, TimeSpan right) => left == (DateRange)right;

        /// <summary>
        /// Not Equals operator.
        /// </summary>
        /// <param name="left">The left hand side.</param>
        /// <param name="right">The right hand side.</param>
        /// <returns>
        /// <c>true</c> is <paramref name="left"/> is not equal to <paramref name="right"/>;
        /// otherwise <c>false</c>.
        /// </returns>
        public static bool operator !=(DateRange left, DateRange right) => !(left == right);

        public static bool operator !=(TimeSpan left, DateRange right) => !(left == right);

        public static bool operator !=(DateRange left, TimeSpan right) => !(left == right);

        public static TimeSpan operator -(DateRange value) => value.Negate();

        /// <summary>
        /// Return a negative <see cref="TimeSpan"/> with the interval of this <see cref="DateRange"/>
        /// </summary>
        /// <returns></returns>
        public TimeSpan Negate() => TimeSpan.Negate();

        public static bool operator <(DateRange left, DateRange right) => (TimeSpan)left < (TimeSpan)right;

        public static bool operator <(DateRange left, TimeSpan right) => (TimeSpan)left < right;

        public static bool operator <(TimeSpan left, DateRange right) => left < (TimeSpan)right;

        public static bool operator <=(DateRange left, DateRange right) => (TimeSpan)left <= (TimeSpan)right;

        public static bool operator <=(DateRange left, TimeSpan right) => (TimeSpan)left <= right;

        public static bool operator <=(TimeSpan left, DateRange right) => left <= (TimeSpan)right;

        public static bool operator >(DateRange left, DateRange right) => (TimeSpan)left > (TimeSpan)right;

        public static bool operator >(DateRange left, TimeSpan right) => (TimeSpan)left > right;

        public static bool operator >(TimeSpan left, DateRange right) => left > (TimeSpan)right;

        public static bool operator >=(DateRange left, DateRange right) => (TimeSpan)left >= (TimeSpan)right;

        public static bool operator >=(DateRange left, TimeSpan right) => (TimeSpan)left >= right;

        public static bool operator >=(TimeSpan left, DateRange right) => left >= (TimeSpan)right;

        private TimeSpan? _timeSpanBase = null;

        /// <summary>
        /// The base <see cref="System.TimeSpan"/> used in this <see cref="DateRange"/>
        /// </summary>
        public TimeSpan TimeSpan
        {
            get
            {
                if (_timeSpanBase == null)
                {
                    _timeSpanBase = (EndDate - StartDate);
                    if (ForceFirstAndLastMoments)
                    {
                        _timeSpanBase = _timeSpanBase.Value.Add(TimeSpan.FromMilliseconds(1));
                    }
                }
                return _timeSpanBase.Value;
            }
        }

        /// <summary>
        /// Performs an explicit conversion from <see cref="DateRange"/> to <see cref="System.TimeSpan"/>.
        /// </summary>
        /// <param name="DateRange">The DateRange.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator TimeSpan(DateRange DateRange) => DateRange.TimeSpan;

        /// <summary>
        /// Performs an implicit conversion from a <see cref="System.TimeSpan"/> to <see cref="DateRange"/>.
        /// </summary>
        /// <param name="timeSpan">The <see cref="System.TimeSpan"/> that will be converted.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator DateRange(TimeSpan timeSpan) => new DateRange(timeSpan);

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;

                case DateRange dr:
                    return this == dr;

                case TimeSpan ts:
                    return TimeSpan == ts;

                case int i:
                    return GetHashCode() == i;

                case long l:
                    return Ticks == l;

                default:
                    return false;
            }
        }

        public override int GetHashCode() => Months.GetHashCode() ^ Years.GetHashCode() ^ Days.GetHashCode() ^ Hours.GetHashCode() ^ Minutes.GetHashCode() ^ Seconds.GetHashCode() ^ Milliseconds.GetHashCode();

        private static DateRange AddInternal(DateRange left, TimeSpan right) => new DateRange(left.StartDate, right);

        private static DateRange SubtractInternal(DateRange left, TimeSpan right) => new DateRange(left.StartDate, left.EndDate.Subtract(right));

        private static DateRange AddInternal(DateRange left, DateRange right) => AddInternal(left, right.TimeSpan);

        private static DateRange SubtractInternal(DateRange left, DateRange right) => SubtractInternal(left, right.TimeSpan);

        public int CompareTo(TimeSpan other) => TimeSpan.CompareTo(other);

        public int CompareTo(object value)
        {
            if (value is TimeSpan timeSpan)
            {
                return this.TimeSpan.CompareTo(timeSpan);
            }

            throw new ArgumentException("Value must be a TimeSpan or DateRange", "value");
        }

        public int CompareTo(DateRange value) => this.TimeSpan.CompareTo(value?.TimeSpan ?? TimeSpan.Zero);

        public static implicit operator DateRange((DateTime, DateTime) Dates) => new DateRange(Dates.Item1, Dates.Item2);

        public static implicit operator List<DateTime>(DateRange dateRange) => dateRange?.PairArray().ToList();

        public static implicit operator DateTime[](DateRange dateRange) => dateRange?.PairArray().ToArray();

        public static implicit operator Dictionary<string, DateTime>(DateRange dateRange) => dateRange?.Dictionary();

        /// <summary>
        /// Indica se este <see cref="DateRange"/> foi construido sem nenhuma data definida
        /// </summary>
        /// <returns></returns>

        private DateTime _startDate;
        private DateTime _endDate;

        /// <summary>
        /// Retorna uma lista de dias entre <see cref="StartDate"/> e <see cref="EndDate"/>
        /// </summary>
        /// <param name="DaysOfWeek"></param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetDays(params DayOfWeek[] DaysOfWeek) => GetDays(DaysOfWeek.AsEnumerable());

        public IEnumerable<DateTime> GetDays(IEnumerable<DayOfWeek> DaysOfWeek) => StartDate.GetDaysBetween(EndDate, DaysOfWeek.ToArray());

        /// <summary>
        /// Retorna o periodo em um total especificado por <see cref="DateRangeInterval"/>
        /// </summary>
        /// <param name="Interval"></param>
        /// <returns></returns>
        public double GetPeriodAs(DateRangeInterval Interval = DateRangeInterval.LessAccurate)
        {
            if (Interval == DateRangeInterval.LessAccurate)
            {
                Interval = GetLessAccurateDateRangeInterval();
            }

            switch (Interval)
            {
                case DateRangeInterval.Years: return TotalYears;
                case DateRangeInterval.Months: return TotalMonths;
                case DateRangeInterval.Weeks: return TotalWeeks;
                case DateRangeInterval.Days: return TotalDays;
                case DateRangeInterval.Hours: return TotalHours;
                case DateRangeInterval.Minutes: return TotalMinutes;
                case DateRangeInterval.Seconds: return TotalSeconds;
                case DateRangeInterval.Milliseconds:
                case DateRangeInterval.LessAccurate:
                default: return TotalMilliseconds;
            };
        }

        /// <summary>
        /// Return a new instance os <see cref="DateRange"/> by adding a <see
        /// cref="DateRangeInterval"/> multiplied by <paramref name="Amount"/> to this <see cref="DateRange"/>
        /// </summary>
        /// <returns></returns>

        public DateRange AddInterval(DateRangeInterval DateRangeInterval, double Amount)
        {
            if (DateRangeInterval == DateRangeInterval.LessAccurate)
            {
                DateRangeInterval = GetLessAccurateDateRangeInterval();
            }

            return new DateRange(StartDate.AddInterval(DateRangeInterval, Amount), EndDate.AddInterval(DateRangeInterval, Amount), ForceFirstAndLastMoments);
        }

        /// <summary>
        /// Return a new identical instance of this <see cref="DateRange"/>
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone() => this.Clone();

        /// <summary>
        /// Return a new identical instance of this <see cref="DateRange"/>
        /// </summary>
        /// <returns></returns>
        public DateRange Clone() => new DateRange(StartDate, EndDate, ForceFirstAndLastMoments);

        /// <summary>
        /// Return a new <see cref="DateRange"/> with equivalent number of <see
        /// cref="DateRangeInterval"/> multiplied by <paramref name="Amount"/>
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Negative <paramref name="Amount"/> return a period before <see cref="StartDate"/>,
        /// Positive <paramref name="Amount"/> return a period after <see cref="EndDate"/>
        /// </remarks>
        public DateRange JumpPeriod(int Amount, DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate) => Amount == 0 ? Clone() : AddInterval(DateRangeInterval, GetPeriodAs(DateRangeInterval) * Amount);

        /// <summary>
        /// Return a new <see cref="DateRange"/> with equivalent number of <see
        /// cref="DateRangeInterval"/> before <see cref="StartDate"/>
        /// </summary>
        /// <returns></returns>
        public DateRange PreviousPeriod(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate) => AddInterval(DateRangeInterval, GetPeriodAs(DateRangeInterval).ForceNegative());

        /// <summary>
        /// Return a new <see cref="DateRange"/> with equivalent number of <see
        /// cref="DateRangeInterval"/> after <see cref="EndDate"/>
        /// </summary>
        /// <returns></returns>
        public DateRange NextPeriod(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate) => AddInterval(DateRangeInterval, GetPeriodAs(DateRangeInterval).ForcePositive());

        /// <summary>
        /// Return the less accurate <see cref="DateRangeInterval"/> by checking the most hight
        /// non-zero total propety of this <see cref="DateRange"/>
        /// </summary>
        /// <returns></returns>
        public DateRangeInterval GetLessAccurateDateRangeInterval()
        {
            if (TotalYears.ForcePositive() >= 1d) return DateRangeInterval.Years;
            else if (TotalMonths.ForcePositive() >= 1d) return DateRangeInterval.Months;
            else if (TotalWeeks.ForcePositive() >= 1d) return DateRangeInterval.Weeks;
            else if (TotalDays.ForcePositive() >= 1d) return DateRangeInterval.Days;
            else if (TotalHours.ForcePositive() >= 1d) return DateRangeInterval.Hours;
            else if (TotalMinutes.ForcePositive() >= 1d) return DateRangeInterval.Minutes;
            else if (TotalSeconds.ForcePositive() >= 1d) return DateRangeInterval.Seconds;
            else return DateRangeInterval.Milliseconds;
        }

        /// <summary>
        /// Retorna TRUE se a data de inicio e fim for a mesma
        /// </summary>
        /// <returns></returns>
        public bool IsSingleDate() => StartDate.Date == EndDate.Date;

        /// <summary>
        /// Retorna TRUE se a data e hora de inicio e fim for a mesma
        /// </summary>
        /// <returns></returns>
        public bool IsSingleDateTime() => StartDate == EndDate;

        /// <summary>
        /// Cria um grupo de quinzenas que contenham este periodo
        /// </summary>
        /// <returns></returns>
        public FortnightGroup CreateFortnightGroup() => FortnightGroup.CreateFromDateRange(this);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime>> PropertyExpression, DateRangeFilterBehavior FilterBehavior) => List.FilterDateRange(PropertyExpression, this, FilterBehavior);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime>> PropertyExpression) => List.FilterDateRange(PropertyExpression, this);

        /// <summary>
        /// Returns a new <see cref="IQueryable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime>> PropertyExpression, DateRangeFilterBehavior FilterBehavior) => List.FilterDateRange(PropertyExpression, this, FilterBehavior);

        /// <summary>
        /// Returns a new <see cref="IQueryable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime>> PropertyExpression) => List.FilterDateRange(PropertyExpression, this);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateRangeFilterBehavior FilterBehavior) => List.FilterDateRange(PropertyExpression, this, FilterBehavior);

        /// <summary>
        /// Returns a new <see cref="IEnumerable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime?>> PropertyExpression) => List.FilterDateRange(PropertyExpression, this);

        /// <summary>
        /// Returns a new <see cref="IQueryable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateRangeFilterBehavior FilterBehavior) => List.FilterDateRange(PropertyExpression, this, FilterBehavior);

        /// <summary>
        /// Returns a new <see cref="IQueryable{T}"/> filtered from a property where its value is
        /// tested through the function <see cref="Contains(DateTime, DateRangeFilterBehavior)"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression) => List.FilterDateRange(PropertyExpression, this);

        /// <summary>
        /// Agrupa itens de uma lista de acordo com uma propriedade e uma expressão de agrupamento
        /// de datas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <param name="GroupByExpression"></param>
        /// <param name="DateRangeInterval"></param>
        /// <returns></returns>
        public Dictionary<string, IEnumerable<T>> GroupList<T>(IEnumerable<T> List, Func<T, DateTime> PropertyExpression, Func<DateTime, string> GroupByExpression, DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            var keys = GetDates(DateRangeInterval).Select(GroupByExpression).Distinct();
            var gp = List.GroupBy(x => GroupByExpression(PropertyExpression(x)));
            var dic = new Dictionary<string, IEnumerable<T>>();
            foreach (var k in keys)
            {
                if (!dic.ContainsKey(k))
                {
                    dic[k] = new List<T>();
                } ((List<T>)dic[k]).AddRange(gp.ElementAtOrDefault(k.ToInt()).AsEnumerable());
            }

            return dic;
        }

        public Dictionary<string, IEnumerable<T>> GroupList<T>(IEnumerable<T> List, Func<T, DateTime?> PropertyExpression, Func<DateTime?, string> GroupByExpression, DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            var keys = GetDates(DateRangeInterval).Cast<DateTime?>().Select(GroupByExpression).Distinct();
            var gp = List.GroupBy(x => GroupByExpression(PropertyExpression(x))).ToDictionary();
            var dic = new Dictionary<string, IEnumerable<T>>();
            foreach (var k in keys)
            {
                if (!dic.ContainsKey(k))
                {
                    dic[k] = new List<T>();
                }

                List<T> l = (List<T>)dic[k];
                if (gp.ContainsKey(k))
                {
                    l.AddRange(gp[k].AsEnumerable());
                }

                dic[k] = l;
            }

            return dic;
        }

        /// <summary>
        /// Check if this <see cref="DateRange"/> overlaps another <see cref="DateRange"/>
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool Overlaps(DateRange Period) => (Period.StartDate <= EndDate && Period.StartDate >= StartDate) || (StartDate <= Period.EndDate && StartDate >= Period.StartDate);

        /// <summary>
        /// Check if 2 dateranges maches any date (Intersect, Contains or Overlaps)
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool MatchAny(DateRange Period) => Overlaps(Period) || Contains(Period) || IsIn(Period);

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains another <see cref="DateRange"/>
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool Contains(DateRange Period) => StartDate <= Period.StartDate && Period.EndDate <= EndDate;

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains a <see cref="DateTime"/> using a
        /// <paramref name="FilterBehavior"/>
        /// </summary>
        /// <param name="Day"></param>
        /// <returns></returns>
        public bool Contains(DateTime Day, DateRangeFilterBehavior FilterBehavior)
        {
            if (IsSingleDateTime())
            {
                return StartDate == Day;
            }
            else
            {
                switch (FilterBehavior)
                {
                    case DateRangeFilterBehavior.Between: return Day.IsBetween(StartDate, EndDate);
                    case DateRangeFilterBehavior.BetweenOrEqualExcludeEnd: return Day.IsBetweenOrEqualExcludeMax(StartDate, EndDate);
                    case DateRangeFilterBehavior.BetweenOrEqual:
                    default: return Day.IsBetweenOrEqual(StartDate, EndDate);
                }
            }
        }

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains a <see cref="DateTime"/> using a
        /// <paramref name="FilterBehavior"/>
        /// </summary>
        /// <param name="Day"></param>
        /// <returns></returns>
        public bool Contains(DateTime? Day, DateRangeFilterBehavior FilterBehavior) => Day.HasValue && Contains(Day.Value, FilterBehavior);

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains a <see cref="DateTime"/> using the
        /// configured <see name="FilterBehavior"/>
        /// </summary>
        /// <param name="Day"></param>
        /// <returns></returns>
        public bool Contains(DateTime Day) => Contains(Day, this.FilterBehavior);

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains a <see cref="DateTime"/> using the
        /// configured <see name="FilterBehavior"/>
        /// </summary>
        /// <param name="Day"></param>
        /// <returns></returns>
        public bool Contains(DateTime? Day) => Contains(Day, this.FilterBehavior);

        /// <summary>
        /// Configuration to compare a <see cref="DateTime"/> to <see cref="StartDate"/> and <see cref="EndDate"/>
        /// </summary>
        public DateRangeFilterBehavior FilterBehavior { get; set; } = DateRangeFilterBehavior.BetweenOrEqual;

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains <see cref="DateTime.Now"/> using the
        /// configured <see cref="FilterBehavior"/>
        /// </summary>
        /// <returns></returns>
        public bool IsNow() => IsNow(this.FilterBehavior);

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains <see cref="DateTime.Now"/> using the
        /// giving <paramref name="FilterBehavior"/>
        /// </summary>
        /// <returns></returns>
        public bool IsNow(DateRangeFilterBehavior FilterBehavior) => Contains(DateTime.Now, FilterBehavior);

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains <see cref="DateTime.Today"/> using the
        /// configured <see cref="FilterBehavior"/>
        /// </summary>
        /// <returns></returns>
        public bool IsToday() => IsToday(this.FilterBehavior);

        /// <summary>
        /// Check if this <see cref="DateRange"/> contains <see cref="DateTime.Today"/> using the
        /// giving <paramref name="FilterBehavior"/>
        /// </summary>
        /// <returns></returns>
        public bool IsToday(DateRangeFilterBehavior FilterBehavior) => Contains(DateTime.Today, FilterBehavior);

        /// <summary>
        /// Verifica se este periodo está dentro de outro periodo
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool IsIn(DateRange Period) => Period.Contains(this);

        /// <summary>
        /// Verifica quantos porcento uma data representa em distancia dentro deste periodo
        /// </summary>
        /// <param name="[Date]">Data correspondente</param>
        /// <returns></returns>
        public decimal CalculatePercent(DateTime? Date = default) => (Date ?? DateTime.Now).CalculateTimelinePercent(StartDate, EndDate);

        /// <summary>
        /// Return the <see cref="StartDate"/> and <see cref="EndDate"/> inside a <see cref="IEnumerable{DateTime}"/>
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> PairArray() => new[] { StartDate, EndDate };

        /// <summary>
        /// Return the <see cref="StartDate"/> and <see cref="EndDate"/> as <see
        /// cref="Tuple{DateTime, DateTime}"/>
        /// </summary>
        /// <returns></returns>
        public Tuple<DateTime, DateTime> Tuple() => new Tuple<DateTime, DateTime>(StartDate, EndDate);

        // <summary>
        /// Return the <see cref="StartDate"/> and <see cref="EndDate"/> as <see
        /// cref="Dictionary{string, DateTime}"/> </summary> <returns></returns>
        public Dictionary<string, DateTime> Dictionary(string StartDateLabel = null, string EndDateLabel = null) => new Dictionary<string, DateTime>()
        {
            [StartDateLabel.IfBlank("StartDate")] = StartDate,
            [EndDateLabel.IfBlank("EndDate")] = EndDate
        };

        /// <summary>
        /// Return a <see cref="IEnumerable{T}"/> of <see cref="DateTime"/> between <see
        /// cref="StartDate"/> and <see cref="EndDate"/> using a <see cref="DateRangeInterval"/>
        /// between each <see cref="DateTime"/> and configured <see cref="FilterBehavior"/>
        /// </summary>
        /// <param name="DateRangeInterval"></param>
        /// <returns></returns>

        public IEnumerable<DateTime> GetDates(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate) => GetDates(this.FilterBehavior, DateRangeInterval);

        /// <summary>
        /// Return a <see cref="IEnumerable{T}"/> of <see cref="DateTime"/> between <see
        /// cref="StartDate"/> and <see cref="EndDate"/> using a <see cref="DateRangeInterval"/>
        /// between each <see cref="DateTime"/> and giving <paramref name="FilterBehavior"/>
        /// </summary>
        /// <param name="DateRangeInterval"></param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetDates(DateRangeFilterBehavior FilterBehavior, DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            DateRangeInterval = DateRangeInterval == DateRangeInterval.LessAccurate ? GetLessAccurateDateRangeInterval() : DateRangeInterval;

            var l = new List<DateTime>() { StartDate, EndDate };
            var curdate = StartDate;
            while (curdate <= EndDate)
            {
                curdate = curdate.AddInterval(DateRangeInterval, 1d);
                if (!l.Contains(curdate)) l.Add(curdate);
            }

            l.RemoveAll(x => !this.Contains(x, FilterBehavior));
            l.Sort();
            return l;
        }

        public bool Equals(DateRange other) => this.Ticks.Equals(other.TimeSpan.Round().Ticks);
    }

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

        public DateRangeDisplay(DateRangeString FormatRule) : base() => this.FormatRule = FormatRule;

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
        public DateRangeDisplay(string AndWord, string MillisecondsWord, string SecondsWord, string MinutesWord, string HoursWord, string DaysWord, string MonthsWord, string YearsWord, DateRangeString FormatRule = DateRangeString.FullStringSkipZero)
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
        /// The 'Milliseconds' word (on plural).
        /// </summary>
        public string MillisecondsWord { get; set; }

        /// <summary>
        /// The 'Seconds' word (on plural).
        /// </summary>
        public string SecondsWord { get; set; }

        /// <summary>
        /// The 'Minutes' word (on plural).
        /// </summary>
        public string MinutesWord { get; set; }

        /// <summary>
        /// The 'Hours' word (on plural).
        /// </summary>
        public string HoursWord { get; set; }

        /// <summary>
        /// The 'Days' word (on plural).
        /// </summary>
        public string DaysWord { get; set; }

        /// <summary>
        /// The 'Months' word (on plural).
        /// </summary>
        public string MonthsWord { get; set; }

        /// <summary>
        /// The 'Years' word (on plural).
        /// </summary>
        public string YearsWord { get; set; }

        /// <summary>
        /// Rules for returning the string
        /// </summary>
        public DateRangeString FormatRule { get; set; } = DateRangeString.FullStringSkipZero;

        public static DateRangeDisplay DefaultPtBr() => new DateRangeDisplay("e", "milisegundos", "segundos", "minutos", "horas", "dias", "meses", "anos");

        public static DateRangeDisplay Default() => new DateRangeDisplay();
    }

    /// <summary>
    /// Intervals used <see cref="DateRange.AddInterval(DateRangeInterval, double)(DateRangeInterval)"/>
    /// </summary>
    public enum DateRangeInterval
    {
        /// <summary>
        /// <see cref="DateRange"/> will use the most hight <see cref="DateRangeInterval"/> avaible.
        /// </summary>
        LessAccurate = -1,

        /// <summary>
        /// Milliseconds
        /// </summary>
        Milliseconds = 0,

        /// <summary>
        /// Seconds
        /// </summary>
        Seconds = 1,

        /// <summary>
        /// Minutes
        /// </summary>
        Minutes = 2,

        /// <summary>
        /// Hours
        /// </summary>
        Hours = 3,

        /// <summary>
        /// Days
        /// </summary>
        Days = 4,

        /// <summary>
        /// Weeks
        /// </summary>
        Weeks = 5,

        /// <summary>
        /// Months
        /// </summary>
        Months = 6,

        /// <summary>
        /// Years
        /// </summary>
        Years = 7
    }

    /// <summary>
    /// Behavior for <see cref="DateRange.FilterBehavior"/>
    /// </summary>
    public enum DateRangeFilterBehavior
    {
        /// <summary>
        /// Check if <see cref="DateTime"/> is between <see cref="DateRange.StartDate"/> and <see
        /// cref="DateRange.EndDate"/> but not equal them
        /// </summary>
        Between,

        /// <summary>
        /// Check if <see cref="DateTime"/> is Between or equal <see cref="DateRange.StartDate"/>
        /// and <see cref="DateRange.EndDate"/>
        /// </summary>
        BetweenOrEqual,

        /// <summary>
        /// Check if <see cref="DateTime"/> is greater than or equal <see
        /// cref="DateRange.StartDate"/> and less than <see cref="DateRange.EndDate"/>
        /// </summary>
        BetweenOrEqualExcludeEnd,
    }

    /// <summary>
    /// Format rules for <see cref="DateRangeDisplay"/>
    /// </summary>
    public enum DateRangeString
    {
        /// <summary>
        /// Return the full string, including Zeros
        /// </summary>
        FullStringWithZero = 0,

        /// <summary>
        /// Return the full string, but skip zeros. This is the DEFAULT behavior
        /// </summary>
        FullStringSkipZero = 1,

        /// <summary>
        /// If <see cref="DateRange"/> value is greater than 1 day, the hours, minutes and seconds
        /// are ignored
        /// </summary>
        ReduceToDays = 2,

        /// <summary>
        /// If <see cref="DateRange"/> value is greater than 1 month, the days, hours, minutes and
        /// seconds are ignored
        /// </summary>
        ReduceToMonths = 3,

        /// <summary>
        /// If <see cref="DateRange"/> value is greater than 1 year, the months, days, hours,
        /// minutes and seconds are ignored
        /// </summary>
        ReduceToYears = 4
    }
}