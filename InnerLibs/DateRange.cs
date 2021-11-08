using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InnerLibs.LINQ;
using InnerLibs.TimeMachine;


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
        public static implicit operator DateRange((DateTime, DateTime) Dates) => new DateRange(Dates.Item1, Dates.Item2);

        public static implicit operator List<DateTime>(DateRange dateRange) => dateRange?.Pair().ToList();

        public static implicit operator DateTime[](DateRange dateRange) => dateRange?.Pair().ToArray();

        public static implicit operator Dictionary<string, DateTime>(DateRange dateRange) => dateRange?.Dictionary();

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
                    _enddate = _enddate.Date.GetLastMoment();
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
        public bool IsDefaultDateRange() => _IsDefault;

        private DateTime _startDate;
        private DateTime _enddate;

        /// <summary>
        /// Instancia um novo periodo do dia de hoje
        /// </summary>
        public DateRange() : this(DateTime.Now, DateTime.Now, true) { _IsDefault = true; EndDate = StartDate; }

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

        public DateRange(IEnumerable<DateTime?> Dates) : this(Dates?.Where(x => x.HasValue).Select(x => x.Value)) { }

        public DateRange(IEnumerable<DateTime> Dates, bool ForceFirstAndLastMoments) : this(Dates) => this.ForceFirstAndLastMoments = ForceFirstAndLastMoments;

        public DateRange(IEnumerable<DateTime?> Dates, bool ForceFirstAndLastMoments) : this(Dates) => this.ForceFirstAndLastMoments = ForceFirstAndLastMoments;

        public DateRange(DateTime StartEndDate) : this(StartEndDate, StartEndDate) => ForceFirstAndLastMoments = true;

        public DateRange(DateTime? StartEndDate) : this(StartEndDate.Value, StartEndDate.Value) => ForceFirstAndLastMoments = true;

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
        public IEnumerable<DateTime> GetDays(params DayOfWeek[] DaysOfWeek) => StartDate.GetDaysBetween(EndDate, DaysOfWeek);

        /// <summary>
        /// Retorna o periodo em um total especificado por <see cref="DateRangeInterval"/>
        /// </summary>
        /// <param name="Interval"></param>
        /// <returns></returns>
        public decimal GetPeriodAs(DateRangeInterval Interval = DateRangeInterval.LessAccurate)
        {
            if (Interval == DateRangeInterval.LessAccurate)
            {
                Interval = GetLessAccurateDateRangeInterval();
            }

            var sd = StartDate;
            var ed = EndDate;

            var range_diferenca = sd.GetDifference(ed);

            switch (Interval)
            {
                case DateRangeInterval.Milliseconds: return range_diferenca.TotalMilliseconds;
                case DateRangeInterval.Seconds: return range_diferenca.TotalSeconds;
                case DateRangeInterval.Minutes: return range_diferenca.TotalMinutes;
                case DateRangeInterval.Hours: return range_diferenca.TotalHours;
                case DateRangeInterval.Days: return range_diferenca.TotalDays;
                case DateRangeInterval.Weeks: return range_diferenca.TotalWeeks;
                case DateRangeInterval.Months: return range_diferenca.TotalMonths;
                case DateRangeInterval.Years: return range_diferenca.TotalYears;
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
        public static DateTime AddInterval(DateTime Datetime, DateRangeInterval Interval, decimal Total)
        {
            switch (Interval)
            {

                case DateRangeInterval.Milliseconds: return Datetime.AddMilliseconds((double)Total);
                case DateRangeInterval.Seconds: return Datetime.AddSeconds((double)Total);
                case DateRangeInterval.Minutes: return Datetime.AddMinutes((double)Total);
                case DateRangeInterval.Hours: return Datetime.AddHours((double)Total);
                case DateRangeInterval.Days: return Datetime.AddDays((double)Total);
                case DateRangeInterval.Weeks: return Datetime.AddDays((double)(Total * 7m));
                case DateRangeInterval.Months: return Datetime.AddMonths((int)Total);
                case DateRangeInterval.Years: return Datetime.AddYears((int)Total);
                default: throw new ArgumentException("You can't use LessAcurate on this scenario. LessAccurate only work for get a DateRange string");
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

            return new DateRange(AddInterval(StartDate, DateRangeInterval, Total), AddInterval(EndDate, DateRangeInterval, Total), ForceFirstAndLastMoments);
        }

        /// <summary>
        /// Clona este DateRange
        /// </summary>
        /// <returns></returns>
        public DateRange Clone() => new DateRange(StartDate, EndDate, ForceFirstAndLastMoments) { _IsDefault = _IsDefault, _Difference = _Difference };

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
        public bool IsSingleDate() => StartDate.Date == EndDate.Date;

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
            if (_Difference == null)
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
        public FortnightGroup CreateFortnightGroup() => FortnightGroup.CreateFromDateRange(StartDate, EndDate);

        /// <summary>
        /// Retorna uma string representando a diferença das datas
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Difference().ToString();

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime>> PropertyExpression) => List.Where(PropertyExpression.IsBetween(this).Compile());

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime>> PropertyExpression) => List.Where(PropertyExpression.IsBetween(this));

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IEnumerable<T> FilterList<T>(IEnumerable<T> List, Expression<Func<T, DateTime?>> PropertyExpression) => List.Where(PropertyExpression.IsBetween(this).Compile());

        /// <summary>
        /// Filtra uma lista considerando o periodo deste DateRange
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <returns></returns>
        public IQueryable<T> FilterList<T>(IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression) => List.Where(PropertyExpression.IsBetween(this));

        /// <summary>
        /// Agrupa itens de uma lista de acordo com uma propriedade e uma expressão de agrupamento de datas
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
        /// Verifica se 2 periodos coincidem datas (interseção, esta dentro de um periodo de ou contém um periodo)
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
        /// Verifica quantos porcento uma data representa  em distancia dentro deste periodo
        /// </summary>
        /// <param name="[Date]">Data correspondente</param>
        /// <returns></returns>
        public decimal CalculatePercent(DateTime? Date = default) => (Date ?? DateTime.Now).CalculateTimelinePercent(StartDate, EndDate);

        public IEnumerable<DateTime> Pair() => new[] { StartDate, EndDate };

        public Dictionary<string, DateTime> Dictionary(string StartDateLabel = null, string EndDateLabel = null) => new Dictionary<string, DateTime>()
        {
            [StartDateLabel.IfBlank("StartDate")] = StartDate,
            [EndDateLabel.IfBlank("EndDate")] = EndDate
        };

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
                curdate = Convert.ToDateTime(AddInterval(curdate, DateRangeInterval, 1m));
                l.Add(curdate);
            }

            l.Add(EndDate);
            return l.Where(x => x <= EndDate).Where(x => x >= StartDate).Distinct();
        }
    }
}