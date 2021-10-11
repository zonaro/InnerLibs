using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using InnerLibs.LINQ;
using InnerLibs.TimeMachine;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{
    public enum DateRangeInterval
    {
        LessAccurate = -1,
        Milliseconds = 0,
        Seconds = 1,
        Minutes = 2,
        Hours = 3,
        Days = 4,
        Weeks = 5,
        Months = 6,
        Years = 7
    }

    /// <summary>
    /// Classe que representa um periodo entre 2 datas
    /// </summary>
    public class DateRange
    {
        public static implicit operator DateRange((DateTime, DateTime) Dates)
        {
            return new DateRange(Dates.Item1, Dates.Item2);
        }

        public DateTime StartDate
        {
            get
            {
                Calendars.FixDateOrder(ref _startDate, ref _enddate);
                if (ForceFirstAndLastMoments)
                {
                    _startDate = _startDate.Date;
                }

                return _startDate;
            }

            set
            {
                _IsDefault = false;
                if (_startDate != value)
                {
                    _Difference = null;
                    _startDate = value;
                }
            }
        }

        public DateTime EndDate
        {
            get
            {
                Calendars.FixDateOrder(ref _startDate, ref _enddate);
                if (ForceFirstAndLastMoments)
                {
                    _enddate = _enddate.Date.AddHours(23d).AddMinutes(59d).AddSeconds(59d);
                }

                return _enddate;
            }

            set
            {
                _IsDefault = false;
                if (_enddate != value)
                {
                    _Difference = null;
                    _enddate = value;
                }
            }
        }

        /// <summary>
        /// Se true, ajusta as horas de <see cref="StartDate"/> para o primeiro momento do dia e as horas de <see cref="EndDate"/> para o último momento do dia
        /// </summary>
        public bool ForceFirstAndLastMoments { get; set; } = true;

        private bool _IsDefault = false;

        /// <summary>
        /// Indica se este <see cref="DateRange"/> foi construido sem nenhuma data definida
        /// </summary>
        /// <returns></returns>
        public bool IsDefaultDateRange()
        {
            return _IsDefault;
        }

        private DateTime _startDate;
        private DateTime _enddate;

        /// <summary>
        /// Instancia um novo periodo do dia de hoje
        /// </summary>
        public DateRange() : this(DateTime.Now, DateTime.Now, true)
        {
            _IsDefault = true;
        }

        public DateRange(IEnumerable<DateTime> Dates)
        {
            if (Dates is null || !Dates.Any())
            {
                throw new ArgumentException("Argument 'Dates' is null or empty");
            }

            StartDate = Dates.Min();
            EndDate = Dates.Max();
            ForceFirstAndLastMoments = GetLessAccurateDateRangeInterval() > DateRangeInterval.Hours;
            _IsDefault = false;
        }

        public DateRange(IEnumerable<DateTime?> Dates)
        {
            Dates = Dates ?? Array.Empty<DateTime?>();
            Dates = Dates.Where(x => x.HasValue);
            if (Dates.Any())
            {
                StartDate = Dates.Min().Value;
                EndDate = Dates.Max().Value;
                ForceFirstAndLastMoments = GetLessAccurateDateRangeInterval() > DateRangeInterval.Hours;
                _IsDefault = false;
            }
            else
            {
                throw new ArgumentException("Argument 'Dates' is null or empty");
            }
        }

        public DateRange(IEnumerable<DateTime> Dates, bool ForceFirstAndLastMoments) : this(Dates)
        {
            this.ForceFirstAndLastMoments = ForceFirstAndLastMoments;
        }

        public DateRange(IEnumerable<DateTime?> Dates, bool ForceFirstAndLastMoments) : this(Dates)
        {
            this.ForceFirstAndLastMoments = ForceFirstAndLastMoments;
        }

        public DateRange(DateTime StartEndDate) : this(StartEndDate, StartEndDate)
        {
            ForceFirstAndLastMoments = true;
        }

        public DateRange(DateTime? StartEndDate) : this(StartEndDate.Value, StartEndDate.Value)
        {
            ForceFirstAndLastMoments = true;
        }

        /// <summary>
        /// Instancia um novo periodo entre 2 datas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        public DateRange(DateTime StartDate, DateTime EndDate)
        {
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            ForceFirstAndLastMoments = GetLessAccurateDateRangeInterval() > DateRangeInterval.Hours;
            _IsDefault = false;
        }

        /// <summary>
        /// Instancia um novo periodo entre 2 datas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="ForceFirstAndLastMoments"> Ajusta as horas de <see cref="StartDate"/> para o primeiro momento do dia e as horas de <see cref="EndDate"/> para o último momento do dia</param>
        public DateRange(DateTime StartDate, DateTime EndDate, bool ForceFirstAndLastMoments)
        {
            this.StartDate = StartDate;
            this.EndDate = EndDate;
            this.ForceFirstAndLastMoments = ForceFirstAndLastMoments;
            _IsDefault = false;
        }

        /// <summary>
        /// Retorna uma lista de dias entre <see cref="StartDate"/> e <see cref="EndDate"/>
        /// </summary>
        /// <param name="DaysOfWeek"></param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetDays(params DayOfWeek[] DaysOfWeek)
        {
            return StartDate.GetDaysBetween(EndDate, DaysOfWeek);
        }

        /// <summary>
        /// Retorna o periodo em um total especificado por <see cref="DateRangeInterval"/>
        /// </summary>
        /// <param name="DateRangeInterval"></param>
        /// <returns></returns>
        public decimal GetPeriodAs(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            if (DateRangeInterval == DateRangeInterval.LessAccurate)
            {
                DateRangeInterval = GetLessAccurateDateRangeInterval();
            }

            var sd = StartDate;
            var ed = EndDate;
            if (ForceFirstAndLastMoments)
            {
                ed = ed.AddHours(1d).Date;
            }

            var range_diferenca = sd.GetDifference(ed);
            switch (DateRangeInterval)
            {
                case DateRangeInterval.Milliseconds:
                    {
                        return range_diferenca.TotalMilliseconds;
                    }

                case DateRangeInterval.Seconds:
                    {
                        return range_diferenca.TotalSeconds;
                    }

                case DateRangeInterval.Minutes:
                    {
                        return range_diferenca.TotalMinutes;
                    }

                case DateRangeInterval.Hours:
                    {
                        return range_diferenca.TotalHours;
                    }

                case DateRangeInterval.Days:
                    {
                        return range_diferenca.TotalDays;
                    }

                case DateRangeInterval.Weeks:
                    {
                        return range_diferenca.TotalWeeks;
                    }

                case DateRangeInterval.Months:
                    {
                        return range_diferenca.TotalMonths;
                    }

                case DateRangeInterval.Years:
                    {
                        return range_diferenca.TotalYears;
                    }
            }

            return -1;
        }

        public static object AddInterval(DateTime Datetime, DateRangeInterval DateRangeInterval, decimal Total)
        {
            if (DateRangeInterval == DateRangeInterval.LessAccurate)
            {
                throw new ArgumentException("You cant use LessAcurate on this scenario. LessAccurate only work inside DateRanges");
            }

            switch (DateRangeInterval)
            {
                case DateRangeInterval.Seconds:
                    {
                        return Datetime.AddSeconds((double)Total);
                    }

                case DateRangeInterval.Minutes:
                    {
                        return Datetime.AddMinutes((double)Total);
                    }

                case DateRangeInterval.Hours:
                    {
                        return Datetime.AddHours((double)Total);
                    }

                case DateRangeInterval.Days:
                    {
                        return Datetime.AddDays((double)Total);
                    }

                case DateRangeInterval.Weeks:
                    {
                        return Datetime.AddDays((double)(Total * 7m));
                    }

                case DateRangeInterval.Months:
                    {
                        return Datetime.AddMonths((int)Math.Round(Total));
                    }

                case DateRangeInterval.Years:
                    {
                        return Datetime.AddYears((int)Math.Round(Total));
                    }

                default:
                    {
                        return Datetime.AddMilliseconds((double)Total);
                    }
            }
        }

        /// <summary>
        /// Move um periodo a partir de um <paramref name="Total"/> especificado por <paramref name="DateRangeInterval"/>
        /// </summary>
        /// <param name="DateRangeInterval"></param>
        /// <param name="Total"></param>
        /// <returns></returns>
        public DateRange MovePeriod(DateRangeInterval DateRangeInterval, decimal Total)
        {
            if (DateRangeInterval == DateRangeInterval.LessAccurate)
            {
                DateRangeInterval = GetLessAccurateDateRangeInterval();
            }

            return new DateRange(Conversions.ToDate(AddInterval(StartDate, DateRangeInterval, Total)), Conversions.ToDate(AddInterval(EndDate, DateRangeInterval, Total)), ForceFirstAndLastMoments);
        }

        /// <summary>
        /// Clona este DateRange
        /// </summary>
        /// <returns></returns>
        public DateRange Clone()
        {
            return new DateRange(StartDate, EndDate, ForceFirstAndLastMoments) { _IsDefault = _IsDefault, _Difference = _Difference };
        }

        /// <summary>
        /// Pula um determinado numero de periodos
        /// </summary>
        /// <returns></returns>
        public DateRange JumpPeriod(int Amount, DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            if (Amount == 0)
            {
                return Clone();
            }

            return MovePeriod(DateRangeInterval, GetPeriodAs(DateRangeInterval) * Amount);
        }

        /// <summary>
        /// Move para o periodo equivalente anterior
        /// </summary>
        /// <returns></returns>
        public DateRange PreviousPeriod(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            return MovePeriod(DateRangeInterval, -GetPeriodAs(DateRangeInterval));
        }

        /// <summary>
        /// Move para ao proximo periodo equivalente
        /// </summary>
        /// <returns></returns>
        public DateRange NextPeriod(DateRangeInterval DateRangeInterval = DateRangeInterval.LessAccurate)
        {
            return MovePeriod(DateRangeInterval, GetPeriodAs(DateRangeInterval));
        }

        /// <summary>
        /// Retorna o <see cref="DateRangeInterval"/> menos preciso para calcular periodos
        /// </summary>
        /// <returns></returns>
        public DateRangeInterval GetLessAccurateDateRangeInterval()
        {
            var sd = StartDate;
            var ed = EndDate;
            if (ForceFirstAndLastMoments)
            {
                ed = ed.AddHours(1d).Date;
            }

            var t = sd.GetDifference(ed);
            if (t.TotalYears >= 1m || t.TotalYears <= -1)
            {
                return DateRangeInterval.Years;
            }

            if (t.TotalMonths >= 1m || t.TotalMonths <= -1)
            {
                return DateRangeInterval.Months;
            }

            if (t.TotalWeeks >= 1m || t.TotalWeeks <= -1)
            {
                return DateRangeInterval.Weeks;
            }

            if (t.TotalDays >= 1m || t.TotalDays <= -1)
            {
                return DateRangeInterval.Days;
            }

            if (t.TotalHours >= 1m || t.TotalHours <= -1)
            {
                return DateRangeInterval.Hours;
            }

            if (t.TotalMinutes >= 1m || t.TotalMinutes <= -1)
            {
                return DateRangeInterval.Minutes;
            }

            if (t.TotalSeconds >= 1m || t.TotalSeconds <= -1)
            {
                return DateRangeInterval.Seconds;
            }

            return DateRangeInterval.Milliseconds;
        }

        /// <summary>
        /// Retorna TRUE se a data de inicio e fim for a mesma
        /// </summary>
        /// <returns></returns>
        public bool IsSingleDate()
        {
            return StartDate.Date == EndDate.Date;
        }

        /// <summary>
        /// Retorna TRUE se a data e hora de inicio e fim for a mesma
        /// </summary>
        /// <returns></returns>
        public bool IsSingleDateTime()
        {
            return StartDate == EndDate;
        }

        /// <summary>
        /// Retorna um <see cref="LongTimeSpan"/> contendo a diferença entre as datas
        /// </summary>
        /// <returns></returns>
        public LongTimeSpan Difference()
        {
            if (_Difference is null)
            {
                if (ForceFirstAndLastMoments)
                {
                    _Difference = StartDate.GetDifference(EndDate.AddSeconds(1d));
                }
                else
                {
                    _Difference = StartDate.GetDifference(EndDate);
                }
            }

            return _Difference;
        }

        private LongTimeSpan _Difference = null;

        /// <summary>
        /// Cria um grupo de quinzenas que contenham este periodo
        /// </summary>
        /// <returns></returns>
        public FortnightGroup CreateFortnightGroup()
        {
            return FortnightGroup.CreateFromDateRange(StartDate, EndDate);
        }

        /// <summary>
        /// Retorna uma strin representando a diferença das datas
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Difference().ToString();
        }

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime>> PropertyExpression)
        {
            return List.Where(PropertyExpression.IsBetween(this).Compile());
        }

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime>> PropertyExpression)
        {
            return List.Where(PropertyExpression.IsBetween(this));
        }

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime?>> PropertyExpression)
        {
            return List.Where(PropertyExpression.IsBetween(this).Compile());
        }

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression)
        {
            return List.Where(PropertyExpression.IsBetween(this));
        }

        /// <summary>
        /// Agrupa itens de uma lista de acordo com uma propriedade e uma expressão de agrugrupamento de datas
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
                } ((List<T>)dic[k]).AddRange(gp.ElementAtOrDefault(Conversions.ToInteger(k)).AsEnumerable());
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
        public bool Overlaps(DateRange Period)
        {
            var argStartDate = StartDate;
            var argEndDate = EndDate;
            Calendars.FixDateOrder(ref argStartDate, ref argEndDate);
            StartDate = argStartDate;
            EndDate = argEndDate;
            switch (true)
            {
                case object _ when Period.StartDate <= EndDate & Period.StartDate >= StartDate:
                    {
                        return true;
                    }

                case object _ when StartDate <= Period.EndDate & StartDate >= Period.StartDate:
                    {
                        return true;
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Verifica se 2 periodos coincidem datas (interseção, esta dentro de um periodo de ou contém um periodo)
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool MatchAny(DateRange Period)
        {
            var argStartDate = StartDate;
            var argEndDate = EndDate;
            Calendars.FixDateOrder(ref argStartDate, ref argEndDate);
            StartDate = argStartDate;
            EndDate = argEndDate;
            return Overlaps(Period) | Contains(Period) | IsIn(Period);
        }

        /// <summary>
        /// Verifica se este periodo contém um outro periodo
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool Contains(DateRange Period)
        {
            var argStartDate = StartDate;
            var argEndDate = EndDate;
            Calendars.FixDateOrder(ref argStartDate, ref argEndDate);
            StartDate = argStartDate;
            EndDate = argEndDate;
            return StartDate <= Period.StartDate & Period.EndDate <= EndDate;
        }

        /// <summary>
        /// Verifica se este periodo contém uma data
        /// </summary>
        /// <param name="Day"></param>
        /// <returns></returns>
        public bool Contains(DateTime Day)
        {
            var argStartDate = StartDate;
            var argEndDate = EndDate;
            Calendars.FixDateOrder(ref argStartDate, ref argEndDate);
            StartDate = argStartDate;
            EndDate = argEndDate;
            return StartDate <= Day & Day <= EndDate;
        }

        /// <summary>
        /// Verifica se hoje está dentro deste periodo
        /// </summary>
        /// <returns></returns>
        public bool IsNow()
        {
            return Contains(DateTime.Now);
        }

        /// <summary>
        /// Verifica se este periodo está dentro de outro periodo
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public bool IsIn(DateRange Period)
        {
            var argStartDate = StartDate;
            var argEndDate = EndDate;
            Calendars.FixDateOrder(ref argStartDate, ref argEndDate);
            StartDate = argStartDate;
            EndDate = argEndDate;
            return Period.Contains(this);
        }

        /// <summary>
        /// Verifica quantos porcento uma data representa  em distancia dentro deste periodo
        /// </summary>
        /// <param name="[Date]">Data correspondente</param>
        /// <returns></returns>
        public decimal CalculatePercent(DateTime? Date = default)
        {
            return (Date ?? DateTime.Now).CalculatePercent(StartDate, EndDate);
        }

        public IEnumerable<DateTime> Pair()
        {
            return new[] { StartDate, EndDate };
        }

        /// <summary>
        /// Retorna uma lista com as datas entre <see cref="StartDate"/> e <see cref="EndDate"/> utilizando um Intervalo
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
                curdate = Conversions.ToDate(AddInterval(curdate, DateRangeInterval, 1m));
                l.Add(curdate);
            }

            l.Add(EndDate);
            return l.Where(x => x <= EndDate).Where(x => x >= StartDate).Distinct();
        }
    }

    /// <summary>
    /// Modulo para manipulação de calendário
    /// </summary>
    /// <remarks></remarks>
    public static class Calendars
    {
        public static DateTime ClearMiliseconds(this DateTime Date)
        {
            return Date.AddTicks(-(Date.Ticks % TimeSpan.TicksPerSecond));
        }

        public static DateRange CreateDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateTime? StartDate = default, DateTime? EndDate = default) where T : class
        {
            var Period = new DateRange();
            Period.ForceFirstAndLastMoments = true;
            StartDate = StartDate ?? List.Min(PropertyExpression);
            EndDate = EndDate ?? List.Max(PropertyExpression);
            if (StartDate.HasValue)
            {
                Period.StartDate = StartDate.Value;
            }

            if (EndDate.HasValue)
            {
                Period.StartDate = EndDate.Value;
            }

            return Period;
        }

        /// <summary>
        /// Pega o numero da semana a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetWeekNumberOfMonth(this DateTime DateAndTime)
        {
            return DateAndTime.GetWeekInfoOfMonth().FirstOrDefault();
        }

        /// <summary>
        /// Pega o numero da semana, do mês e ano pertencente
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int[] GetWeekInfoOfMonth(this DateTime DateAndTime)
        {
            DateAndTime = DateAndTime.Date;
            var firstMonthDay = DateAndTime.GetFirstDayOfMonth();
            var firstMonthMonday = firstMonthDay.AddDays(((int)DayOfWeek.Monday + 7 - (int)firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > DateAndTime)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays(((int)DayOfWeek.Monday + 7 - (int)firstMonthDay.DayOfWeek) % 7);
            }

            return new int[] { (int)Math.Round((DateAndTime - firstMonthMonday).Days / 7d + 1d), firstMonthDay.Month, firstMonthDay.Year };
        }

        /// <summary>
        /// Pega o numero do Bimestre a partir de uma data
        /// </summary>
        /// <param name="DateAndtime"></param>
        /// <returns></returns>
        public static int GetDoubleMonthOfYear(this DateTime DateAndtime)
        {
            if (DateAndtime.Month <= 2)
            {
                return 1;
            }
            else if (DateAndtime.Month <= 4)
            {
                return 2;
            }
            else if (DateAndtime.Month <= 6)
            {
                return 3;
            }
            else if (DateAndtime.Month <= 8)
            {
                return 4;
            }
            else if (DateAndtime.Month <= 10)
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }

        /// <summary>
        /// Pega o numero do trimestre a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetQuarterOfYear(this DateTime DateAndTime)
        {
            if (DateAndTime.Month <= 3)
            {
                return 1;
            }
            else if (DateAndTime.Month <= 6)
            {
                return 2;
            }
            else if (DateAndTime.Month <= 9)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        /// <summary>
        /// Pega o numero do semestre a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetHalfOfYear(this DateTime DateAndTime)
        {
            if (DateAndTime.Month <= 6)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// Retorna a idade
        /// </summary>
        /// <param name="BirthDate"></param>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public static int GetAge(this DateTime BirthDate, DateTime? FromDate = default)
        {
            FromDate = FromDate ?? DateTime.Now;
            int age;
            age = FromDate.Value.Year - BirthDate.Year;
            if (BirthDate > DateTime.Today.AddYears(-age))
                age -= 1;
            return age;
        }

        /// <summary>
        /// Converte um <see cref="Date"/> para um timezone Especifico
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="TimeZone"></param>
        /// <returns></returns>
        public static DateTime ToTimeZoneUtc(this DateTime Date, TimeZoneInfo TimeZone)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(Date, TimeZone);
        }

        /// <summary>
        /// Converte um <see cref="Date"/> para um timezone Especifico
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="TimeZoneId"></param>
        /// <returns></returns>
        public static DateTime ToTimeZone(this DateTime Date, string TimeZoneId)
        {
            return TimeZoneInfo.ConvertTime(Date, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));
        }

        /// <summary>
        /// Converte uma string em datetime a partir de um formato especifico
        /// </summary>
        /// <param name="DateString">String original</param>
        /// <param name="Format"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static DateTime ConvertDateString(this string DateString, string Format, CultureInfo Culture = null)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            return DateTime.ParseExact(DateString, Format, Culture);
        }

        /// <summary>
        /// Converte uma string de data para outra string de data com formato diferente
        /// </summary>
        /// <param name="DateString">String original</param>
        /// <param name="InputFormat"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string ChangeFormat(this string DateString, string InputFormat, string OutputFormat, CultureInfo Culture = null)
        {
            return DateString.ConvertDateString(InputFormat, Culture).ToString(OutputFormat);
        }

        /// <summary>
        /// Pula para a data inicial da proxima quinzena
        /// </summary>
        /// <param name="FromDate">Data de partida</param>
        /// <param name="Num">Numero de quinzenas para adiantar</param>
        /// <returns></returns>
        public static DateTime NextFortnight(this DateTime FromDate, int Num = 1)
        {
            return new FortnightGroup(FromDate, Num).Last().Period.StartDate;
        }

        /// <summary>
        /// Retorna o primeiro dia da semana da data especificada
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfWeek(this DateTime Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday)
        {
            while (Date.DayOfWeek > FirstDayOfWeek)
                Date = Date.AddDays(-1);
            return Date;
        }

        /// <summary>
        /// Retorna o ultimo dia da semana da data especificada
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é Domingo)</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfWeek(this DateTime Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday)
        {
            return Date.GetFirstDayOfWeek(FirstDayOfWeek).AddDays(6d);
        }

        /// <summary>
        /// Retorna um DateRange equivalente a semana de uma data especifica
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <param name="FirstDayOfWeek">Primeiro dia da semana (DEFAULT é domingo)</param>
        /// <returns></returns>
        public static DateRange GetWeek(this DateTime Date, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday)
        {
            return new DateRange(Date.GetFirstDayOfWeek(FirstDayOfWeek), Date.GetLastDayOfWeek(FirstDayOfWeek));
        }

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, DateTime.DaysInMonth(Date.Year, Date.Month), Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="MonthNumber">Data</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfMonth(this int MonthNumber, int? Year = default)
        {
            Year = (Year ?? DateTime.Now.Month).SetMinValue(DateTime.MinValue.Month);
            return new DateTime((int)Year, MonthNumber, 1).GetLastDayOfMonth();
        }

        /// <summary>
        /// Retorna a ultima data do mes a partir de uma outra data
        /// </summary>
        /// <param name="MonthNumber">Data</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this int MonthNumber, int? Year = default)
        {
            Year = (Year ?? DateTime.Now.Month).SetMinValue(DateTime.MinValue.Month);
            return new DateTime((int)Year, MonthNumber, 1);
        }

        /// <summary>
        /// Retorna a primeira data do mes a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, 1, Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna a primeira data da quinzena a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfFortnight(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day <= 15 ? 1 : 16, Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna a ultima data da quinzena a partir de uma outra data
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static DateTime GetLastDayOfFortnight(this DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day <= 15 ? 15 : Date.GetLastDayOfMonth().Day, Date.Hour, Date.Minute, Date.Second, Date.Millisecond, Date.Kind);
        }

        /// <summary>
        /// Retorna o prmeiro dia de um ano especifico de outra data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfYear(this DateTime Date)
        {
            return new DateTime(Date.Year, 1, 1).Date;
        }

        /// <summary>
        /// Retorna o ultimo dia de um ano especifico de outra data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfYear(this DateTime Date)
        {
            return new DateTime(Date.Year, 12, 31).Date;
        }

        /// <summary>
        /// Retorna o primeiro dia de um semestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfHalf(this DateTime Date)
        {
            if (Date.GetHalfOfYear() == 1)
            {
                return Date.GetFirstDayOfYear();
            }
            else
            {
                return new DateTime(Date.Year, 7, 1).Date;
            }
        }

        /// <summary>
        /// Retorna o ultimo dia de um semestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfHalf(this DateTime Date)
        {
            if (Date.GetHalfOfYear() == 1)
            {
                return new DateTime(Date.Year, 6, 1).GetLastDayOfMonth();
            }
            else
            {
                return Date.GetLastDayOfYear();
            }
        }

        /// <summary>
        /// Retorna o ultimo dia de um trimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfQuarter(this DateTime Date)
        {
            if (Date.GetQuarterOfYear() == 1)
                return new DateTime(Date.Year, 3, 1).GetLastDayOfMonth();
            if (Date.GetQuarterOfYear() == 2)
                return new DateTime(Date.Year, 6, 1).GetLastDayOfMonth();
            if (Date.GetQuarterOfYear() == 3)
                return new DateTime(Date.Year, 9, 1).GetLastDayOfMonth();
            return new DateTime(Date.Year, 12, 1).GetLastDayOfMonth();
        }

        /// <summary>
        /// Retorna o ultimo dia de um trimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfQuarter(this DateTime Date)
        {
            if (Date.GetQuarterOfYear() == 1)
                return new DateTime(Date.Year, 1, 1).Date;
            if (Date.GetQuarterOfYear() == 2)
                return new DateTime(Date.Year, 4, 1).Date;
            if (Date.GetQuarterOfYear() == 3)
                return new DateTime(Date.Year, 7, 1).Date;
            return new DateTime(Date.Year, 10, 1).Date;
        }

        /// <summary>
        /// Retorna o ultimo dia de um bimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetLastDayOfDoubleMonth(this DateTime Date)
        {
            if (Date.GetDoubleMonthOfYear() == 1)
                return new DateTime(Date.Year, 2, 1).GetLastDayOfMonth();
            if (Date.GetDoubleMonthOfYear() == 2)
                return new DateTime(Date.Year, 4, 1).GetLastDayOfMonth();
            if (Date.GetDoubleMonthOfYear() == 3)
                return new DateTime(Date.Year, 6, 1).GetLastDayOfMonth();
            if (Date.GetDoubleMonthOfYear() == 4)
                return new DateTime(Date.Year, 8, 1).GetLastDayOfMonth();
            if (Date.GetDoubleMonthOfYear() == 5)
                return new DateTime(Date.Year, 10, 1).GetLastDayOfMonth();
            return new DateTime(Date.Year, 12, 1).GetLastDayOfMonth();
        }

        /// <summary>
        /// Retorna o ultimo dia de um bimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfDoubleMonth(this DateTime Date)
        {
            if (Date.GetDoubleMonthOfYear() == 1)
                return new DateTime(Date.Year, 1, 1).Date;
            if (Date.GetDoubleMonthOfYear() == 2)
                return new DateTime(Date.Year, 3, 1).Date;
            if (Date.GetDoubleMonthOfYear() == 3)
                return new DateTime(Date.Year, 5, 1).Date;
            if (Date.GetDoubleMonthOfYear() == 4)
                return new DateTime(Date.Year, 7, 1).Date;
            if (Date.GetDoubleMonthOfYear() == 5)
                return new DateTime(Date.Year, 9, 1).Date;
            return new DateTime(Date.Year, 11, 1).Date;
        }

        /// <summary>
        /// Retorna o numero da semana relativa ao ano
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <param name="FirstDayOfWeek"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime Date, CultureInfo Culture = null, DayOfWeek FirstDayOfWeek = DayOfWeek.Sunday)
        {
            return (Culture ?? CultureInfo.InvariantCulture).Calendar.GetWeekOfYear(Date, CalendarWeekRule.FirstFourDayWeek, FirstDayOfWeek);
        }

        /// <summary>
        /// Verifica se uma data é do mesmo mês e ano que outra data
        /// </summary>
        /// <param name="[Date]">Primeira data</param>
        /// <param name="AnotherDate">Segunda data</param>
        /// <returns></returns>
        public static bool IsSameMonthAndYear(this DateTime Date, DateTime AnotherDate)
        {
            return Date.IsBetween(AnotherDate.GetFirstDayOfMonth(), AnotherDate.GetLastDayOfMonth());
        }

        /// <summary>
        /// Verifica se a Data de hoje é um aniversário
        /// </summary>
        /// <param name="BirthDate">  Data de nascimento</param>
        /// <returns></returns>
        public static bool IsAnniversary(this DateTime BirthDate, DateTime? CompareWith = default)
        {
            if (!CompareWith.HasValue)
                CompareWith = DateTime.Today;
            return (BirthDate.Day + "/" + BirthDate.Month ?? "") == (CompareWith.Value.Day + "/" + CompareWith.Value.Month ?? "");
        }

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetLongMonthName(this DateTime Date, CultureInfo Culture = null)
        {
            return Date.ToString("MMMM", Culture ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetShortMonthName(this DateTime Date, CultureInfo Culture = null)
        {
            return Date.ToString("MM", Culture ?? CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// COnverte um datetime para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this DateTime Date)
        {
            return Date.Year + "-" + Date.Month + "-" + Date.Day + " " + Date.Hour + ":" + Date.Minute + ":" + Date.Second + "." + Date.Millisecond;
        }

        /// <summary>
        /// Converte uma string dd/mm/aaaa hh:mm:ss.llll para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this string Date, string FromCulture = "pt-BR")
        {
            return Convert.ToDateTime(Date, new CultureInfo(FromCulture, false).DateTimeFormat).ToSQLDateString();
        }

        /// <summary>
        /// Retorna uma <see cref="LongTimeSpan"/> com a diferença entre 2 Datas
        /// </summary>
        /// <param name="InitialDate"></param>
        /// <param name="SecondDate"> </param>
        /// <returns></returns>
        public static LongTimeSpan GetDifference(this DateTime InitialDate, DateTime SecondDate)
        {
            FixDateOrder(ref InitialDate, ref SecondDate);
            return new LongTimeSpan(InitialDate, SecondDate);
        }

        /// <summary>
        /// Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate
        /// sempre seja uma data menor que a EndDate, prevenindo que o calculo entre 2 datas resulte em um
        /// <see cref="TimeSpan"/> negativo
        /// </summary>
        /// <param name="StartDate">Data Inicial</param>
        /// <param name="EndDate">  Data Final</param>
        public static void FixDateOrder(ref DateTime StartDate, ref DateTime EndDate)
        {
            ClassTools.FixOrder(ref StartDate, ref EndDate);
        }

        /// <summary>
        /// Verifica se uma data se encontra entre 2 datas
        /// </summary>
        /// <param name="MidDate">   Data</param>
        /// <param name="StartDate"> Data Inicial</param>
        /// <param name="EndDate">   Data final</param>
        /// <param name="IgnoreTime">Indica se o tempo deve ser ignorado na comparação</param>
        /// <returns></returns>
        public static bool IsBetween(this DateTime MidDate, DateTime StartDate, DateTime EndDate, bool IgnoreTime = false)
        {
            FixDateOrder(ref StartDate, ref EndDate);
            if (IgnoreTime)
            {
                return StartDate.Date <= MidDate.Date & MidDate.Date <= EndDate.Date;
            }
            else
            {
                return StartDate <= MidDate & MidDate <= EndDate;
            }
        }

        /// <summary>
        /// Retorna as datas entre um periodo
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="DaysOfWeek"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> GetDaysBetween(this DateTime StartDate, DateTime EndDate, params DayOfWeek[] DaysOfWeek)
        {
            var l = new List<DateTime>() { StartDate.Date };
            DaysOfWeek = DaysOfWeek ?? Array.Empty<DayOfWeek>();
            if (DaysOfWeek.Length == 0)
            {
                DaysOfWeek = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            }

            var curdate = StartDate.Date;
            while (curdate.Date < EndDate.Date)
            {
                curdate = curdate.AddDays(1d);
                l.Add(curdate);
            }

            l.Add(EndDate.Date);
            return l.Distinct().Where(x => x.DayOfWeek.IsIn(DaysOfWeek));
        }

        /// <summary>
        /// Remove o tempo de todas as datas de uma lista e retorna uma nova lista
        /// </summary>
        /// <param name="List">Lista que será alterada</param>
        public static IEnumerable<DateTime> ClearTime(this IEnumerable<DateTime> List)
        {
            return List.Select(x => x.Date);
        }

        /// <summary>
        /// Retorna uma String no formato "W dias, X horas, Y minutos e Z segundos"
        /// </summary>
        /// <param name="TimeElapsed">TimeSpan com o intervalo</param>
        /// <returns>string</returns>
        public static string ToTimeElapsedString(this TimeSpan TimeElapsed, string DayWord = "dia", string HourWord = "hora", string MinuteWord = "minuto", string SecondWord = "segundo")
        {
            string dia = TimeElapsed.Days > 0 ? TimeElapsed.Days == 1 ? TimeElapsed.Days + " " + DayWord + " " : TimeElapsed.Days + " " + DayWord + "s " : "";
            string horas = TimeElapsed.Hours > 0 ? TimeElapsed.Hours == 1 ? TimeElapsed.Hours + " " + HourWord + " " : TimeElapsed.Hours + " " + HourWord + "s " : "";
            string minutos = TimeElapsed.Minutes > 0 ? TimeElapsed.Minutes == 1 ? TimeElapsed.Minutes + " " + MinuteWord + " " : TimeElapsed.Minutes + " " + MinuteWord + "s " : "";
            string segundos = TimeElapsed.Seconds > 0 ? TimeElapsed.Seconds == 1 ? TimeElapsed.Seconds + " " + SecondWord + " " : TimeElapsed.Seconds + " " + SecondWord + "s " : "";

            dia = dia.AppendIf(",", dia.IsNotBlank() && (horas.IsNotBlank() | minutos.IsNotBlank() | segundos.IsNotBlank()));
            horas = horas.AppendIf(",", horas.IsNotBlank() && (minutos.IsNotBlank() | segundos.IsNotBlank()));
            minutos = minutos.AppendIf(",", minutos.IsNotBlank() && segundos.IsNotBlank());

            string current = dia + " " + horas + " " + minutos + " " + segundos;
            if (current.Contains(","))
            {
                current = current.Insert(current.LastIndexOf(","), " e ");
                current = current.Remove(current.LastIndexOf(","), 1);
            }

            return current.AdjustWhiteSpaces();
        }

        /// <summary>
        /// Retorna uma String baseado no numero do Mês Ex.: 1 -&gt; Janeiro
        /// </summary>
        /// <param name="MonthNumber">Numero do Mês</param>
        /// <returns>String com nome do Mês</returns>

        public static string ToLongMonthName(this int MonthNumber)
        {
            return new DateTime(DateTime.Now.Year, MonthNumber, 1).TolongMonthName();
        }

        /// <summary>
        /// Retorna uma String com o nome do mes baseado na data
        /// </summary>
        /// <param name="DateTime">Data</param>
        /// <returns>String com nome do Mês</returns>
        public static string TolongMonthName(this DateTime DateTime)
        {
            return DateTime.ToString("MMMM");
        }

        /// <summary>
        /// Retorna uma String curta baseado no numero do Mês Ex.: 1 -&gt; Jan
        /// </summary>
        /// <param name="MonthNumber">Numero do Mês</param>
        /// <returns>String com nome curto do Mês</returns>

        public static string ToShortMonthName(this int MonthNumber)
        {
            return MonthNumber.ToLongMonthName().GetFirstChars(3);
        }

        /// <summary>
        /// Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Segunda-Feira
        /// </summary>
        /// <param name="DayNumber">Numero do Dia</param>
        /// <returns>String com nome do Dia</returns>

        public static string ToLongDayOfWeekName(this int DayNumber)
        {
            return DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)DayNumber);
        }

        /// <summary>
        /// Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Seg
        /// </summary>
        /// <param name="DayNumber">Numero do Dia</param>
        /// <returns>String com nome curto do Dia</returns>

        public static string ToShortDayOfWeekName(this int DayNumber)
        {
            return DayNumber.ToLongDayOfWeekName().GetFirstChars(3);
        }

        /// <summary>
        /// Retorna a data de amanhã
        /// </summary>
        /// <returns>Data de amanhã</returns>

        public static DateTime Tomorrow
        {
            get
            {
                return DateTime.Now.AddDays(1d);
            }
        }

        /// <summary>
        /// Retorna a data de ontem
        /// </summary>
        /// <returns>Data de ontem</returns>

        public static DateTime Yesterday
        {
            get
            {
                return DateTime.Now.AddDays(-1);
            }
        }

        public static DateTime BrazilianNow
        {
            get
            {
                return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
            }
        }

        public static DateTime BrazilianTomorrow
        {
            get
            {
                return BrazilianNow.AddDays(1d);
            }
        }

        public static DateTime BrazilianYesterday
        {
            get
            {
                return BrazilianNow.AddDays(-1);
            }
        }

        /// <summary>
        /// Retorna o ultimo domingo
        /// </summary>
        /// <returns></returns>
        public static DateTime LastSunday(DateTime? FromDate = null)
        {
            return LastDay(DayOfWeek.Sunday, FromDate);
        }

        /// <summary>
        /// Retorna o proximo domingo
        /// </summary>
        /// <returns></returns>
        public static DateTime NextSunday(DateTime? FromDate = default)
        {
            return NextDay(DayOfWeek.Sunday, FromDate);
        }

        public static DateTime LastDay(DayOfWeek DayOfWeek, DateTime? FromDate = default)
        {
            FromDate = FromDate ?? DateTime.Now;
            while (FromDate.Value.DayOfWeek != DayOfWeek)
                FromDate = FromDate.Value.AddDays(-1);
            return (DateTime)FromDate;
        }

        public static DateTime NextDay(DayOfWeek DayOfWeek, DateTime? FromDate = default)
        {
            FromDate = FromDate ?? DateTime.Now;
            while (FromDate.Value.DayOfWeek != DayOfWeek)
                FromDate = FromDate.Value.AddDays(1d);
            return (DateTime)FromDate;
        }

        /// <summary>
        /// Verifica se o dia se encontra no fim de semana
        /// </summary>
        /// <param name="YourDate">Uma data qualquer</param>
        /// <returns>TRUE se for sabado ou domingo, caso contrario FALSE</returns>

        public static bool IsWeekend(this DateTime YourDate)
        {
            return YourDate.DayOfWeek == DayOfWeek.Sunday | YourDate.DayOfWeek == DayOfWeek.Saturday;
        }

        private static string ToGreetingFarewell(this DateTime Time, string Language = "pt", bool Farewell = false)
        {
            string bomdia = "Bom dia";
            string boatarde = "Boa tarde";
            string boanoite = "Boa noite";
            string boanoite_despedida = boanoite;
            string seDespedidaManha = "tenha um ótimo dia";
            string seDespedidaTarde = "tenha uma ótima tarde";
            switch (Language.ToLower() ?? "")
            {
                case "en":
                case "eng":
                case "ingles":
                case "english":
                case "inglés":
                    {
                        bomdia = "Good morning";
                        boatarde = "Good afternoon";
                        boanoite = "Good evening";
                        boanoite_despedida = "Good night";
                        seDespedidaManha = "have a nice day";
                        seDespedidaTarde = "have a great afternoon";
                        break;
                    }

                case "es":
                case "esp":
                case "espanhol":
                case "espanol":
                case "español":
                case "spanish":
                    {
                        bomdia = "Buenos días";
                        boatarde = "Buenas tardes";
                        boanoite = "Buenas noches";
                        boanoite_despedida = boanoite;
                        seDespedidaManha = "que tengas un buen día";
                        seDespedidaTarde = "que tengas una buena tarde";
                        break;
                    }
            }

            if (Time.Hour < 12)
            {
                return Farewell ? seDespedidaManha : bomdia;
            }
            else if (Time.Hour >= 12 && Time.Hour < 18)
            {
                return Farewell ? seDespedidaTarde : boatarde;
            }
            else
            {
                return Farewell ? boanoite_despedida : boanoite;
            }
        }

        /// <summary>
        /// Transforma um DateTime em uma despedida (Bom dia, Boa tarde, Boa noite)
        /// </summary>
        /// <param name="Time">    Horario</param>
        /// <param name="Language">Idioma da saudação (pt, en, es)</param>
        /// <returns>Uma string com a despedida</returns>
        public static string ToFarewell(this DateTime Time, string Language = "pt")
        {
            return Time.ToGreetingFarewell(Language, true);
        }

        /// <summary>
        /// Transforma um DateTime em uma saudação (Bom dia, Boa tarde, Boa noite)
        /// </summary>
        /// <param name="Time">    Horario</param>
        /// <param name="Language">Idioma da saudação (pt, en, es)</param>
        /// <returns>Uma string com a despedida</returns>
        public static string ToGreeting(this DateTime Time, string Language = "pt")
        {
            return Time.ToGreetingFarewell(Language, false);
        }

        /// <summary>
        /// Retorna uma saudação
        /// </summary>
        /// <param name="Language">Idioma da saudação (pt, en, es)</param>
        /// <returns>Uma string com a saudação</returns>
        public static string get_Greeting(string Language = "pt")
        {
            return DateTime.Now.ToGreetingFarewell(Language);
        }

        /// <summary>
        /// Retorna uma despedida
        /// </summary>
        /// <param name="Language">Idioma da despedida (pt, en, es)</param>
        /// <returns>Uma string com a despedida</returns>
        public static string get_Farewell(string Language = "pt")
        {
            return DateTime.Now.ToGreetingFarewell(Language, true);
        }

        /// <summary>
        /// Returna uma lista dupla com os meses
        /// </summary>
        /// <param name="ValueType">Apresentação dos meses no valor</param>
        /// <param name="TextType">Apresentação dos meses no texto</param>

        public static List<KeyValuePair<string, string>> get_Months(TypeOfFill TextType = TypeOfFill.LongName, TypeOfFill ValueType = TypeOfFill.Number)
        {
            List<KeyValuePair<string, string>> MonthsRet = default;
            MonthsRet = new List<KeyValuePair<string, string>>();
            for (int i = 1; i <= 12; i++)
            {
                string key = "";
                string value = "";
                switch (TextType)
                {
                    case TypeOfFill.LongName:
                        {
                            key = i.ToLongMonthName();
                            break;
                        }

                    case TypeOfFill.ShortName:
                        {
                            key = i.ToShortMonthName();
                            break;
                        }

                    default:
                        {
                            key = i.ToString();
                            break;
                        }
                }

                switch (ValueType)
                {
                    case TypeOfFill.LongName:
                        {
                            value = i.ToLongMonthName();
                            break;
                        }

                    case TypeOfFill.ShortName:
                        {
                            value = i.ToShortMonthName();
                            break;
                        }

                    default:
                        {
                            value = i.ToString();
                            break;
                        }
                }

                MonthsRet.Add(new KeyValuePair<string, string>(key, value));
            }

            return MonthsRet;
        }

        /// <summary>
        /// Returna uma lista dupla com os meses
        /// </summary>
        /// <param name="ValueType">Apresentação dos meses no valor</param>
        /// <param name="TextType">Apresentação dos meses no texto</param>

        public static List<KeyValuePair<string, string>> get_WeekDays(TypeOfFill TextType = TypeOfFill.LongName, TypeOfFill ValueType = TypeOfFill.Number)
        {
            List<KeyValuePair<string, string>> WeekDaysRet = default;
            WeekDaysRet = new List<KeyValuePair<string, string>>();
            for (int i = 1; i <= 7; i++)
            {
                string key = "";
                string value = "";
                switch (TextType)
                {
                    case TypeOfFill.LongName:
                        {
                            key = i.ToLongDayOfWeekName();
                            break;
                        }

                    case TypeOfFill.ShortName:
                        {
                            key = i.ToShortDayOfWeekName();
                            break;
                        }

                    default:
                        {
                            key = i.ToString();
                            break;
                        }
                }

                switch (ValueType)
                {
                    case TypeOfFill.LongName:
                        {
                            value = i.ToLongDayOfWeekName();
                            break;
                        }

                    case TypeOfFill.ShortName:
                        {
                            value = i.ToShortDayOfWeekName();
                            break;
                        }

                    default:
                        {
                            value = i.ToString();
                            break;
                        }
                }

                WeekDaysRet.Add(new KeyValuePair<string, string>(key, value));
            }

            return WeekDaysRet;
        }

        /// <summary>
        /// Tipo de Apresentação dos Meses/Dias da Semana/Estado
        /// </summary>

        public enum TypeOfFill
        {

            /// <summary>
            /// Numerico
            /// </summary>
            Number = 2,

            /// <summary>
            /// Abreviado
            /// </summary>

            ShortName = 1,
            /// <summary>
            /// Completo
            /// </summary>

            LongName = 0
        }

        /// <summary>
        /// Elemento do calendário
        /// </summary>
        public enum CalendarType
        {
            Weekdays,
            Months
        }

        /// <summary>
        /// Calcula a porcentagem de diferenca entre duas datas de acordo com a data inicial especificada
        /// </summary>
        /// <param name="MidDate">  Data do meio a ser verificada (normalmente Now)</param>
        /// <param name="StartDate">Data Inicial</param>
        /// <param name="EndDate">  Data Final</param>
        /// <returns></returns>
        public static decimal CalculatePercent(this DateTime MidDate, DateTime StartDate, DateTime EndDate)
        {
            FixDateOrder(ref StartDate, ref EndDate);
            if (MidDate < StartDate)
                return 0m;
            if (MidDate > EndDate)
                return 100m;
            return (decimal)((MidDate - StartDate).Ticks * 100L / (double)(EndDate - StartDate).Ticks);
        }
    }
}