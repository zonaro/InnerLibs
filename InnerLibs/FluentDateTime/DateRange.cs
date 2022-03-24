using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace InnerLibs.TimeMachine
{
    /// <summary>
    /// Works like a positive <see cref="System.TimeSpan"/> with validation of Relevant (Business)
    /// Days and many other <see cref="DateTime"/> functions
    /// </summary>
    public class DateRange : IEquatable<DateRange>, IComparable<TimeSpan>, IComparable<DateRange>
    {
        /// <summary>
        /// Create a new <see cref="DateRange"/> for today (from 00:00:00.000 to 23:59:59.999)
        /// </summary>
        public DateRange() : this(DateTime.Now, DateTime.Now, true) { _IsDefault = true; }

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
        public DateRange(DateTime StartDate, DateTime EndDate, params DayOfWeek[] RelevantDaysOfWeek) : this(StartDate, EndDate, false, RelevantDaysOfWeek)
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
        public DateRange(DateTime StartDate, DateTime EndDate, bool ForceFirstAndLastMoments, params DayOfWeek[] RelevantDaysOfWeek)
        {
            if (RelevantDaysOfWeek == null || !RelevantDaysOfWeek.Any())
            {
                RelevantDaysOfWeek = PredefinedArrays.SundayToSaturday.ToArray();
            }

            this.RelevantDaysOfWeek = RelevantDaysOfWeek.ToList();
            _forceFirstAndLastMoments = ForceFirstAndLastMoments;
            _IsDefault = false;
            _startDate = StartDate;
            _enddate = EndDate;
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

        public DateRange(DateTime? StartEndDate) : this(StartEndDate.Value, StartEndDate.Value, true)
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
            Calendars.FixDateOrder(ref _startDate, ref _enddate);

            if (ForceFirstAndLastMoments)
            {
                _startDate = _startDate.Date;
                _enddate = _enddate.EndOfDay();
            }

            this._timeSpanBase = _enddate - _startDate;
            var CurDate = _startDate;
            int years = 0;
            int months = 0;
            int days = 0;
            var _phase = Phase.Years;

            while (_phase != Phase.Done)
            {
                switch (_phase)
                {
                    case Phase.Years:
                        {
                            if (CurDate.AddYears(years + 1) > EndDate)
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
                            if (CurDate.AddMonths(months + 1) > EndDate)
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
                            if (CurDate.AddDays(days + 1) > EndDate)
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
                    _IsDefault = false;
                    _startDate = value;
                    CalcRange();
                }
            }
        }

        public DateTime EndDate
        {
            get => _enddate;

            set
            {
                if (_enddate != value)
                {
                    _IsDefault = false;
                    _enddate = value;
                    CalcRange();
                }
            }
        }

        private bool _forceFirstAndLastMoments = false;

        /// <summary>
        /// When <b>TRUE</b>, force <see cref="StartDate"/> to the first moment of day (Midnight)
        /// and <see cref="EndDate"/> to last moment of day (23:59:59.999)
        /// </summary>
        public bool ForceFirstAndLastMoments
        {
            get => _forceFirstAndLastMoments;
            set { _forceFirstAndLastMoments = value; CalcRange(); }
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
        /// Convert a <see cref="DateRange"/> to a human readable string.
        /// </summary>
        /// <param name="AndWord">the "And" word, use to concatenate the last item in the string</param>
        /// <param name="YearsWord">'Years'</param>
        /// <param name="MonthsWord">'Months'</param>
        /// <param name="DaysWord">'Days'</param>
        /// <param name="HoursWord">'Hours'</param>
        /// <param name="MinutesWord">'Minutes'</param>
        /// <param name="SecondsWord">'Seconds'</param>
        /// <param name="MillisecondsWord">'Milliseconds'</param>
        /// <param name="Format">Rules for returning the string</param>
        /// <returns></returns>
        public string ToDisplayString(DateRangeDisplay display)
        {
            display = display ?? DateRangeDisplay.Default();

            string ano = Text.QuantifyText(display.YearsWord, Years).Prepend($"{Years} ").NullIf(x => display.YearsWord.IsBlank());
            string mes = Text.QuantifyText(display.MonthsWord, Months).Prepend($"{Months} ").NullIf(x => display.MonthsWord.IsBlank());
            string dia = Text.QuantifyText(display.DaysWord, Days).Prepend($"{Days} ").NullIf(x => display.DaysWord.IsBlank());
            string horas = Text.QuantifyText(display.HoursWord, Hours).Prepend($"{Hours} ").NullIf(x => display.HoursWord.IsBlank());
            string minutos = Text.QuantifyText(display.MinutesWord, Minutes).Prepend($"{Minutes} ").NullIf(x => display.MinutesWord.IsBlank());
            string segundos = Text.QuantifyText(display.SecondsWord, Seconds).Prepend($"{Seconds} ").NullIf(x => display.SecondsWord.IsBlank());
            string milisegundos = Text.QuantifyText(display.MinutesWord, Milliseconds).Prepend($"{Milliseconds} ").NullIf(x => display.MillisecondsWord.IsBlank());

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

        /// <summary>
        /// Convert a <see cref="System.TimeSpan"/> to a human readable string.
        /// </summary>
        /// <param name="timeSpan">The <see cref="System.TimeSpan"/> to convert</param>
        /// <returns>A human readable string for <paramref name="timeSpan"/></returns>

        private DateRangeDisplay _display = DateRangeDisplay.Default();

        public DateRangeDisplay Display
        {
            get
            {
                if (_display == null)
                {
                    _display = DateRangeDisplay.Default();
                }
                return _display;
            }
        }

        /// <summary>
        /// Convert a <see cref="DateRange"/> to a human readable string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToDisplayString(null);

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
                }
                if (ForceFirstAndLastMoments)
                {
                    _timeSpanBase = _timeSpanBase.Value.Add(TimeSpan.FromMilliseconds(1));
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

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var type = obj.GetType();
            if (type == typeof(DateRange))
            {
                return this == (DateRange)obj;
            }
            if (type == typeof(TimeSpan))
            {
                return this == (TimeSpan)obj;
            }
            if (type == typeof(int))
            {
                return this.GetHashCode() == type.ToInt();
            }

            if (type == typeof(long))
            {
                return this.Ticks == type.ToLong();
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => Months.GetHashCode() ^ Years.GetHashCode() ^ Days.GetHashCode() ^ Hours.GetHashCode() ^ Minutes.GetHashCode() ^ Seconds.GetHashCode() ^ Milliseconds.GetHashCode();

        private static DateRange AddInternal(DateRange left, TimeSpan right) => new DateRange(left.StartDate, right);

        private static DateRange SubtractInternal(DateRange left, TimeSpan right) => new DateRange(left.StartDate, left.EndDate.Subtract(right));

        private static DateRange AddInternal(DateRange left, DateRange right) => AddInternal(left, right.TimeSpan);

        private static DateRange SubtractInternal(DateRange left, DateRange right) => SubtractInternal(left, right.TimeSpan);

        public int CompareTo(TimeSpan other) => ((TimeSpan)this).CompareTo(other);

        public int CompareTo(object value)
        {
            if (value is TimeSpan timeSpan)
            {
                return ((TimeSpan)this).CompareTo(timeSpan);
            }

            throw new ArgumentException("Value must be a TimeSpan or DateRange", "value");
        }

        public int CompareTo(DateRange value) => ((TimeSpan)this).CompareTo(value);

        public static implicit operator DateRange((DateTime, DateTime) Dates) => new DateRange(Dates.Item1, Dates.Item2);

        public static implicit operator List<DateTime>(DateRange dateRange) => dateRange?.Pair().ToList();

        public static implicit operator DateTime[](DateRange dateRange) => dateRange?.Pair().ToArray();

        public static implicit operator Dictionary<string, DateTime>(DateRange dateRange) => dateRange?.Dictionary();

        private bool _IsDefault = false;

        /// <summary>
        /// Indica se este <see cref="DateRange"/> foi construido sem nenhuma data definida
        /// </summary>
        /// <returns></returns>
        public bool IsDefaultDateRange => _IsDefault;

        private DateTime _startDate;
        private DateTime _enddate;

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
                case DateRangeInterval.Milliseconds: return TotalMilliseconds;
                case DateRangeInterval.Seconds: return TotalSeconds;
                case DateRangeInterval.Minutes: return TotalMinutes;
                case DateRangeInterval.Hours: return TotalHours;
                case DateRangeInterval.Days: return TotalDays;
                case DateRangeInterval.Weeks: return TotalWeeks;
                case DateRangeInterval.Months: return TotalMonths;
                case DateRangeInterval.Years: return TotalYears;
                default: return -1;
            };
        }

        /// <summary>
        /// Adciona um intervalo a um <see cref="DateTime"/>
        /// </summary>
        /// <param name="Datetime"></param>
        /// <param name="Interval"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DateTime AddInterval(DateTime Datetime, DateRangeInterval Interval, double Total)
        {
            switch (Interval)
            {
                case DateRangeInterval.Milliseconds: return Datetime.AddMilliseconds(Total);
                case DateRangeInterval.Seconds: return Datetime.AddSeconds(Total);
                case DateRangeInterval.Minutes: return Datetime.AddMinutes(Total);
                case DateRangeInterval.Hours: return Datetime.AddHours(Total);
                case DateRangeInterval.Days: return Datetime.AddDays(Total);
                case DateRangeInterval.Weeks: return Datetime.AddDays(Total * 7d);
                case DateRangeInterval.Months: return Datetime.AddMonths(Total.RoundInt());
                case DateRangeInterval.Years: return Datetime.AddYears(Total.RoundInt());
                default: throw new ArgumentException("You can't use LessAcurate on this scenario. LessAccurate only work for get a DateRange string");
            }
        }

        /// <summary>
        /// Move um periodo a partir de um <paramref name="Total"/> especificado por <paramref name="DateRangeInterval"/>
        /// </summary>
        /// <param name="DateRangeInterval"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        public DateRange MovePeriod(DateRangeInterval DateRangeInterval, double Total)
        {
            if (DateRangeInterval == DateRangeInterval.LessAccurate)
            {
                DateRangeInterval = GetLessAccurateDateRangeInterval();
            }

            return new DateRange(AddInterval(StartDate, DateRangeInterval, Total), AddInterval(EndDate, DateRangeInterval, Total), ForceFirstAndLastMoments);
        }

        /// <summary>
        /// Clona este DateRange
        /// </summary>
        /// <returns></returns>
        public DateRange Clone() => new DateRange(StartDate, EndDate, ForceFirstAndLastMoments) { _IsDefault = _IsDefault };

        /// <summary>
        /// Pula um determinado numero de periodos
        /// </summary>
        /// <returns></returns>
        public DateRange JumpPeriod(int Amount, DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate) => Amount == 0 ? Clone() : MovePeriod(DateRangeInterval, GetPeriodAs(DateRangeInterval) * Amount);

        /// <summary>
        /// Move para o periodo equivalente anterior
        /// </summary>
        /// <returns></returns>
        public DateRange PreviousPeriod(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate) => MovePeriod(DateRangeInterval, -GetPeriodAs(DateRangeInterval));

        /// <summary>
        /// Move para ao proximo periodo equivalente
        /// </summary>
        /// <returns></returns>
        public DateRange NextPeriod(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate) => MovePeriod(DateRangeInterval, GetPeriodAs(DateRangeInterval));

        /// <summary>
        /// Retorna o <see cref="DateRangeInterval"/> menos preciso para calcular periodos
        /// </summary>
        /// <returns></returns>
        public DateRangeInterval GetLessAccurateDateRangeInterval()
        {
            if (TotalYears >= 1d || TotalYears <= -1)
            {
                return DateRangeInterval.Years;
            }

            if (TotalMonths >= 1d || TotalMonths <= -1)
            {
                return DateRangeInterval.Months;
            }

            if (TotalWeeks >= 1d || TotalWeeks <= -1)
            {
                return DateRangeInterval.Weeks;
            }

            if (TotalDays >= 1d || TotalDays <= -1)
            {
                return DateRangeInterval.Days;
            }

            if (TotalHours >= 1d || TotalHours <= -1)
            {
                return DateRangeInterval.Hours;
            }

            if (TotalMinutes >= 1d || TotalMinutes <= -1)
            {
                return DateRangeInterval.Minutes;
            }

            if (TotalSeconds >= 1d || TotalSeconds <= -1)
            {
                return DateRangeInterval.Seconds;
            }

            return DateRangeInterval.Milliseconds;
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
        public FortnightGroup CreateFortnightGroup() => FortnightGroup.CreateFromDateRange(StartDate, EndDate);

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime>> PropertyExpression) => List.FilterDateRange(PropertyExpression, this);

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime>> PropertyExpression) => List.FilterDateRange(PropertyExpression, this);

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime?>> PropertyExpression) => List.FilterDateRange(PropertyExpression, this);

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
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
            var keys = GetBetween(DateRangeInterval).Select(GroupByExpression).Distinct();
            var gp = List.GroupBy(x => GroupByExpression(PropertyExpression(x)));
            var dic = new Dictionary<string, IEnumerable<T>>();
            foreach (var k in keys)
            {
                if (!dic.ContainsKey(k))
                {
                    dic[k] = new List<T>();
                } ((List<T>)dic[k]).AddRange(gp.ElementAtOrDefault(Convert.ToInt16(k)).AsEnumerable());
            }

            return dic;
        }

        public Dictionary<string, IEnumerable<T>> GroupList<T>(IEnumerable<T> List, Func<T, DateTime?> PropertyExpression, Func<DateTime?, string> GroupByExpression, DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            var keys = GetBetween(DateRangeInterval).Cast<DateTime?>().Select(GroupByExpression).Distinct();
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
                    foreach (var item in gp[k].AsEnumerable())
                        l.Add(item);
                }

                dic[k] = l;
            }

            return dic;
        }

        /// <summary>
        /// Verifica se 2 periodos possuem interseção de datas
        /// </summary>
        /// <param name="Period">Periodo</param>
        /// <returns></returns>
        public bool Overlaps(DateRange Period) => (Period.StartDate <= EndDate && Period.StartDate >= StartDate) || (StartDate <= Period.EndDate && StartDate >= Period.StartDate);

        /// <summary>
        /// Verifica se 2 periodos coincidem datas (interseção, esta dentro de um periodo de ou
        /// contém um periodo)
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool MatchAny(DateRange Period) => Overlaps(Period) || Contains(Period) || IsIn(Period);

        /// <summary>
        /// Verifica se este periodo contém um outro periodo
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool Contains(DateRange Period) => StartDate <= Period.StartDate && Period.EndDate <= EndDate;

        /// <summary>
        /// Verifica se este periodo contém uma data
        /// </summary>
        /// <param name="Day"></param>
        /// <returns></returns>
        public bool Contains(DateTime Day) => StartDate <= Day && Day <= EndDate;

        /// <summary>
        /// Verifica se hoje está dentro deste periodo
        /// </summary>
        /// <returns></returns>
        public bool IsNow() => Contains(DateTime.Now);

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

        public IEnumerable<DateTime> Pair() => new[] { StartDate, EndDate };

        public Tuple<DateTime, DateTime> Tuple() => new Tuple<DateTime, DateTime>(StartDate, EndDate);

        public Dictionary<string, DateTime> Dictionary(string StartDateLabel = null, string EndDateLabel = null) => new Dictionary<string, DateTime>()
        {
            [StartDateLabel.IfBlank("StartDate")] = StartDate,
            [EndDateLabel.IfBlank("EndDate")] = EndDate
        };

        /// <summary>
        /// Retorna uma lista com as datas entre <see cref="StartDate"/> e <see cref="EndDate"/>
        /// utilizando um Intervalo
        /// </summary>
        /// <param name="DateRangeInterval"></param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetBetween(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            if (DateRangeInterval == DateRangeInterval.LessAccurate)
            {
                DateRangeInterval = GetLessAccurateDateRangeInterval();
            }

            var l = new List<DateTime>() { StartDate };
            var curdate = StartDate;
            while (curdate < EndDate)
            {
                curdate = Convert.ToDateTime(AddInterval(curdate, DateRangeInterval, 1d));
                l.Add(curdate);
            }

            l.Add(EndDate);
            return l.Where(x => x.IsBetweenOrEqual(StartDate, EndDate)).Distinct();
        }

        public bool Equals(DateRange other) => this.Equals(other.TimeSpan);
    }

    public class DateRangeDisplay
    {
        public string AndWord { get; set; }
        public string MillisecondsWord { get; set; }
        public string SecondsWord { get; set; }
        public string MinutesWord { get; set; }
        public string HoursWord { get; set; }
        public string DaysWord { get; set; }
        public string WeeksWord { get; set; }
        public string MonthsWord { get; set; }
        public string YearsWord { get; set; }

        public DateRangeString FormatRule { get; set; } = DateRangeString.FullStringSkipZero;

        public static DateRangeDisplay Default()
        {
            var d = new DateRangeDisplay();
            foreach (var item in d.GetProperties().Where(x => x.Name.EndsWith("Word")))
            {
                item.SetValue(d, item.Name.RemoveLastEqual("Word"));
            }
            return d;
        }
    }
}