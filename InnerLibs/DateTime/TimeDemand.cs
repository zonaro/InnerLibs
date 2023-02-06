
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InnerLibs
{
    /// <summary>
    /// Dia de Uma Demanda
    /// </summary>
    public class JourneyDay
    {
        #region Private Fields

        private TimeSpan lunch_hour = new TimeSpan(12, 0, 0);

        private TimeSpan start_hour = default;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Inicia uma instancia de dia letivo
        /// </summary>
        public JourneyDay(DayOfWeek Day) => DayOfWeek = Day;

        public JourneyDay(DateTime? Day = null) : this(Day.OrNow().DayOfWeek)
        {
        }

        /// <summary>
        /// Inicia uma instancia de dia letivo
        /// </summary>
        /// <param name="StartDateTime">Hora Inicial</param>
        /// <param name="Journey">Jornada de trabalho</param>
        public JourneyDay(DateTime StartDateTime, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default) : this(StartDateTime) => SetJourney(StartDateTime, Journey, LunchHour, LunchTime);

        #endregion Public Constructors

        #region Public Properties

        public DayOfWeek DayOfWeek { get; private set; }

        public bool HasLunch => LunchTime.Milliseconds > 0;
        public bool IsJourney => JourneyTime.Milliseconds > 0;

        /// <summary>
        /// Hora que se encerra a jornada (inclui hora de almoço)
        /// </summary>
        /// <returns></returns>
        public TimeSpan JourneyEndHour => JourneyStartHour.Add(TotalTime);

        /// <summary>
        /// Hora inicial da jornada
        /// </summary>
        /// <returns></returns>
        public TimeSpan JourneyStartHour
        {
            get => start_hour;

            set => start_hour = value.TimePart();
        }

        /// <summary>
        /// Jornada de Trabalho/Produção
        /// </summary>
        /// <returns></returns>
        public TimeSpan JourneyTime { get; set; } = new TimeSpan(0, 23, 59, 59, 999);

        public TimeSpan LunchEndHour => LunchStartHour.Add(LunchTime);

        /// <summary>
        /// Hora de almoco
        /// </summary>
        /// <returns></returns>
        public TimeSpan LunchStartHour
        {
            get => lunch_hour;

            set => lunch_hour = value.TimePart();
        }

        /// <summary>
        /// Hora de Almoço
        /// </summary>
        /// <returns></returns>
        public TimeSpan LunchTime { get; set; } = default;

        /// <summary>
        /// Jornada + hora de Almoço
        /// </summary>
        /// <returns></returns>
        public TimeSpan TotalTime => JourneyTime + LunchTime;

        #endregion Public Properties

        #region Public Methods

        public DateTime? JourneyEndHourAt(DateTime Day) => Day.DayOfWeek == DayOfWeek && IsJourney ? Day.Date.At(JourneyEndHour) : (DateTime?)null;

        public DateTime? JourneyStartHourAt(DateTime Day) => Day.DayOfWeek == DayOfWeek && IsJourney ? Day.Date.At(JourneyStartHour) : (DateTime?)null;

        public DateTime? LunchEndHourAt(DateTime Day) => Day.DayOfWeek == DayOfWeek && IsJourney && HasLunch ? Day.At(LunchEndHour) : (DateTime?)null;

        public DateTime? LunchStartHourAt(DateTime Day) => Day.DayOfWeek == DayOfWeek && IsJourney && HasLunch ? Day.At(LunchStartHour) : (DateTime?)null;

        /// <summary>
        /// Define a hora inicial e a jornada de trabalho deste dia
        /// </summary>
        /// <param name="StartHour"></param>
        /// <param name="Journey"></param>
        /// <param name="LunchTime">Horas de Almoço</param>
        public JourneyDay SetJourney(DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default) => SetJourney(StartHour.TimeOfDay, Journey, LunchHour.TimeOfDay, LunchTime);

        /// <summary>
        /// Define a hora inicial e a jornada de trabalho deste dia
        /// </summary>
        /// <param name="StartHour"></param>
        /// <param name="Journey"></param>
        /// <param name="LunchTime">Horas de Almoço</param>
        public JourneyDay SetJourney(TimeSpan StartHour, TimeSpan Journey, TimeSpan LunchHour = default, TimeSpan LunchTime = default)
        {
            JourneyStartHour = StartHour;
            JourneyTime = Journey;
            LunchStartHour = LunchHour;
            this.LunchTime = LunchTime;
            return this;
        }

        public override string ToString() => ToString(null);

        public string ToString(CultureInfo culture) => (culture ?? CultureInfo.CurrentCulture)?.DateTimeFormat?.DayNames?.IfNoIndex(DayOfWeek.ToInt()) ?? DayOfWeek.ToString();

        #endregion Public Methods
    }

    /// <summary>
    /// Classe base para calculo de demandas
    /// </summary>
    public class TimeDemand : DateRange
    {


        private List<JourneyDay> _journeys = new List<JourneyDay>();
        private int _quantity = 1;
        private List<DateTime> _holiDays = new List<DateTime>();



        #region Public Constructors

        /// <summary>
        /// inicia uma nova demanda
        /// </summary>
        public TimeDemand() : this(DateTime.Now, TimeSpan.FromHours(1)) { }

        /// <summary>
        /// Inicia uma nova Demanda com as propriedades do item
        /// </summary>
        /// <param name="StartDate">Data Inicial da produção</param>
        /// <param name="Time">Tempo do item</param>
        /// <param name="Quantity">Quantidade de itens</param>
        public TimeDemand(DateTime StartDate, TimeSpan Time) : this(StartDate, Time, 1) { }

        /// <summary>
        /// Inicia uma nova Demanda com as propriedades do item
        /// </summary>
        /// <param name="StartDate">Data Inicial da produção</param>
        /// <param name="Time">Tempo do item</param>
        /// <param name="Quantity">Quantidade de itens</param>
        public TimeDemand(DateTime StartDate, DateTime EndDate) : this(StartDate, EndDate, 1) { }

        /// <summary>
        /// Inicia uma nova Demanda com as propriedades do item
        /// </summary>
        /// <param name="StartDate">Data Inicial da produção</param>
        /// <param name="Time">Tempo do item</param>
        /// <param name="Quantity">Quantidade de itens</param>
        public TimeDemand(DateTime StartDate, TimeSpan Time, int Quantity)
        {
            ItemQuantity = Quantity;
            ItemProductionTime = Time;
            Proccess(StartDate);
        }

        /// <summary>
        /// Inicia uma nova Demanda com as propriedades do item
        /// </summary>
        /// <param name="StartDate">Data Inicial da produção</param>
        /// <param name="EndDate">Tempo do item</param>
        /// <param name="Quantity">Quantidade de itens</param>
        public TimeDemand(DateTime StartDate, DateTime EndDate, int Quantity = 1) : this(StartDate, new TimeSpan(0), Quantity) => ItemProductionTime = GetWorkingTimeUntil(EndDate);

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Data de encerramento da produção
        /// </summary>
        /// <returns></returns>
        public new DateTime EndDate => base.EndDate;

        /// <summary>
        /// Sexta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Friday => GetJourneyDay(DayOfWeek.Friday);

        public bool HasJourney => _journeys.Any(x => x.IsJourney);

        /// <summary>
        /// Feriados, pontos facultativos e/ou datas especificas consideradas não relevantes
        /// </summary>
        /// <returns></returns>
        public List<DateTime> HoliDays
        {
            get
            {
                _holiDays = _holiDays ?? new List<DateTime>();
                return _holiDays;
            }
            set => _holiDays = value ?? new List<DateTime>();
        }

        public string ItemName { get; set; } = "Items";

        /// <summary>
        /// Tempo de produção de 1 item
        /// </summary>
        /// <returns></returns>
        public TimeSpan ItemProductionTime { get; set; } = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Quantidade de itens
        /// </summary>
        /// <returns></returns>
        public int ItemQuantity
        {
            get => _quantity;

            set => _quantity = value.SetMinValue(1);
        }

        /// <summary>
        /// Segunda-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Monday => GetJourneyDay(DayOfWeek.Monday);



        /// <summary>
        /// Tempo total de produção de todos os itens
        /// </summary>
        /// <returns></returns>
        public TimeSpan ProductionTime => new TimeSpan(ItemQuantity * ItemProductionTime.Ticks);

        /// <summary>
        /// Dias relevantes (letivos) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> RelevantDays => GetDays(RelevantDaysOfWeek.ToArray()).ClearTime().Where(x => x.IsNotIn(HoliDays.ClearTime())).OrderBy(x => x);

        /// <summary>
        /// Dias não relevantes (não letivos) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> NonRelevantDays => GetDays(NonRelevantDaysOfWeek.ToArray()).Union(HoliDays.ClearTime()).Distinct().OrderBy(x => x);


        /// <summary>
        /// Dias da semana relevantes
        /// </summary>
        /// <returns></returns>
        public new IEnumerable<DayOfWeek> RelevantDaysOfWeek
        {
            get
            {
                var dias = _journeys.Where(x => x.IsJourney).Select(x => x.DayOfWeek).ToList();

                if (dias.Count == 0)
                {
                    dias.AddRange(new[] { 0, 1, 2, 3, 4, 5, 6 }.Select(x => (DayOfWeek)x));
                }

                return dias;
            }
        }

        /// <summary>
        /// Sábado
        /// </summary>
        /// <returns></returns>
        public JourneyDay Saturday => GetJourneyDay(DayOfWeek.Saturday);

        /// <summary>
        /// Data Inicial da produção
        /// </summary>
        /// <returns></returns>
        public new DateTime StartDate
        {
            get => base.StartDate;
            set => base.StartDate = Proccess(value);
        }

        /// <summary>
        /// Domingo
        /// </summary>
        /// <returns></returns>
        public JourneyDay Sunday => GetJourneyDay(DayOfWeek.Sunday);

        /// <summary>
        /// Quinta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Thursday => GetJourneyDay(DayOfWeek.Thursday);

        /// <summary>
        /// Terça-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Tuesday => GetJourneyDay(DayOfWeek.Tuesday);

        /// <summary>
        /// Quarta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Wednesday => GetJourneyDay(DayOfWeek.Wednesday);

        public TimeSpan WeekJourneyHours => GetWorkTimeBetween(StartDate.BeginningOfWeek(), EndDate.EndOfWeek());

        /// <summary>
        /// Intervalo de horas trabalhadas entre as datas de inicio e fim desta demanda
        /// </summary>
        /// <returns></returns>
        public double WorkHours => WorkTime.TotalHours;

        /// <summary>
        /// Intervalo de tempo trabalhado entre as datas de inicio e fim desta demanda
        /// </summary>
        /// <returns></returns>
        public TimeSpan WorkTime => GetWorkTimeBetween(StartDate, EndDate);

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Cria uma demanda após a demanda atual com as mesmas caracteristicas
        /// </summary>
        /// <param name="DelayTime">Tempo adicionado entre uma demanda e outra</param>
        /// <returns></returns>
        public TimeDemand CloneAndQueue(TimeSpan? DelayTime = null) => new TimeDemand(EndDate.Add(DelayTime ?? new TimeSpan(0L)), ItemProductionTime, ItemQuantity);

        public JourneyDay GetJourneyDay() => GetJourneyDay(DateTime.Now);

        public JourneyDay GetJourneyDay(DayOfWeek DayOfWeek)
        {
            _journeys = _journeys ?? new List<JourneyDay>();

            var jd = _journeys.FirstOr(x => x.DayOfWeek == DayOfWeek, new JourneyDay(DayOfWeek));

            if (_journeys.Any(x => x.DayOfWeek == jd.DayOfWeek) == false)
            {
                _journeys.Add(jd);
            }

            return jd;
        }

        public JourneyDay GetJourneyDay(DateTime Date) => GetJourneyDay(Date.DayOfWeek);

        /// <summary>
        /// Retorna a porcentagem em relacao a posição de uma data entre a data inicial (0%) e final (100%)
        /// </summary>
        /// <param name="MidDate"></param>
        /// <returns></returns>
        public decimal GetPercentCompletion(DateTime MidDate) => MidDate.CalculateTimelinePercent(StartDate, EndDate);

        public decimal GetPercentCompletion() => GetPercentCompletion(DateTime.Now);

        /// <summary>
        /// Dias especificos da semana entre as datas inicial e final da demanda
        /// </summary>
        /// <param name="DaysOfWeek">Dias da semana</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetWeekDays(params DayOfWeek[] DaysOfWeek) => StartDate.GetDaysBetween(EndDate, DaysOfWeek.ToArray());

        public TimeSpan GetWorkingTimeUntil(DateTime EndDate) => GetWorkTimeBetween(StartDate, EndDate);

        /// <summary>
        /// Retorna o intervalo de horas trabalhadas entre duas datas baseado nas configuracoes
        /// desta demanda
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetWorkTimeBetween(DateTime StartDate, DateTime EndDate) => new TimeSpan(RelevantDays.Where(x => x.DayOfWeek.IsIn(RelevantDaysOfWeek) && x.IsBetweenOrEqual(StartDate, EndDate)).Sum(x => GetJourneyDay(x).JourneyTime.Ticks));

        public bool HasLunch(DateTime DateAndTime) => GetJourneyDay(DateAndTime).HasLunch;

        public bool IsJourney(DateTime DateAndTime) => GetJourneyDay(DateAndTime).IsJourney;

        /// <summary>
        /// Retorna a hora final da jornada de uma data acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime? JourneyEndHour(DateTime Date) => GetJourneyDay(Date).JourneyEndHourAt(Date);

        /// <summary>
        /// Retorna a hora inicial da jornada de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime? JourneyStartHour(DateTime Date) => GetJourneyDay(Date).JourneyStartHourAt(Date);

        /// <summary>
        /// Retorna o tempo da jornada de trabalho de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan JourneyTime(DateTime Date) => GetJourneyDay(Date).JourneyTime;

        /// <summary>
        /// Retorna a hora de termino do almoço de uma data de acordo com as configurações desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime? LunchEndHour(DateTime Date) => GetJourneyDay(Date).LunchEndHourAt(Date);

        /// <summary>
        /// Retorno a hora de inicio do almoço de uma data de acordo com as configurações desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime? LunchStartHour(DateTime Date) => GetJourneyDay(Date).LunchStartHourAt(Date);

        /// <summary>
        /// Retorna o tempo de almoço de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan LunchTime(DateTime Date) => GetJourneyDay(Date).LunchTime;

        public DateTime Proccess(DateTime StartDate)
        {
            base.StartDate = PushDateIntoJourney(StartDate);
            base.EndDate = base.StartDate.Add(ProductionTime);

            foreach (var dia in RelevantDays)
            {
                if (!(dia.Date == base.EndDate.Date))
                {
                    base.EndDate = base.EndDate.AddHours(24d - TotalTime(dia).TotalHours);
                }
                else if (HasLunch(dia) && base.EndDate.TimeOfDay > LunchStartHour(dia).Value.TimeOfDay)
                {
                    base.EndDate = base.EndDate.Add(LunchTime(dia));
                }

                if (HasLunch(dia) && base.EndDate.IsBetween(LunchStartHour(dia), LunchEndHour(dia)))
                {
                    base.EndDate = base.EndDate.Add(LunchTime(dia));
                }
            }

            var nrd = NonRelevantDays.ToArray();

            base.EndDate = base.EndDate.AddDays(nrd.Length);
            var lasthours = new TimeSpan();
            var jeh = JourneyEndHour(base.EndDate);
            if (jeh.HasValue && base.EndDate.TimeOfDay > jeh.Value.TimeOfDay)
            {
                lasthours = base.EndDate.TimeOfDay - jeh.Value.TimeOfDay;
            }

            base.EndDate = PushDateIntoJourney(base.EndDate).Add(lasthours);
            return base.EndDate;
        }

        /// <summary>
        /// Empurra a data para dentro da proxima hora disponivel dentro jornada de trabalho
        /// </summary>
        /// <param name="[Date]">Data a ser Verificada</param>
        public DateTime PushDateIntoJourney(DateTime Date)
        {
            if (HasJourney)
            {
                while (IsJourney(Date) && (!Date.IsTimeBetween(JourneyStartHour(Date).Value.TimeOfDay, JourneyEndHour(Date).Value.TimeOfDay) || !RelevantDaysOfWeek.Contains(Date.DayOfWeek) || (HasLunch(Date) && Date.IsTimeBetween(LunchStartHour(Date).Value.TimeOfDay, LunchEndHour(Date).Value.TimeOfDay))))
                {
                    Date = Date.AddMilliseconds(1d);
                }
            }

            return Date;
        }

        /// <summary>
        /// Ajusta a jornada de trabalho de um dia da semana
        /// </summary>
        /// <param name="DayOfWeek"></param>
        /// <param name="StartHour"></param>
        /// <param name="Journey"></param>
        /// <param name="LunchHour"></param>
        /// <param name="LunchTime"></param>
        /// <returns></returns>
        public JourneyDay SetJourney(DayOfWeek DayOfWeek, DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default) => GetJourneyDay(DayOfWeek).SetJourney(StartHour, Journey, LunchHour, LunchTime);

        public JourneyDay SetJourney(DayOfWeek DayOfWeek, DateTime StartHour, int JourneyHours, DateTime LunchHour = default, int LunchHours = default) => SetJourney(DayOfWeek, StartHour, new TimeSpan(JourneyHours, 0, 0), LunchHour, new TimeSpan(LunchHours, 0, 0));

        public TimeDemand SetJourney(IEnumerable<DayOfWeek> DaysOfWeek, DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default)
        {
            foreach (var item in DaysOfWeek ?? Array.Empty<DayOfWeek>())
            {
                SetJourney(item, StartHour, Journey, LunchHour, LunchTime);
            }

            return this;
        }

        public TimeDemand SetJourney(IEnumerable<DayOfWeek> DaysOfWeek, DateTime StartHour, int JourneyHours, DateTime LunchHour = default, int LunchHours = default) => SetJourney(DaysOfWeek, StartHour, new TimeSpan(JourneyHours, 0, 0), LunchHour, new TimeSpan(LunchHours, 0, 0));

        public TimeDemand SetOffJourney(params DayOfWeek[] DaysOfWeek) => SetJourney(DaysOfWeek.AsEnumerable(), DateTime.MinValue, new TimeSpan(0), DateTime.MinValue, new TimeSpan(0));

        /// <summary>
        /// Retorna uma string representado a quantidade de itens e o tempo gasto com a produção
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{ItemQuantity} {ItemQuantity.QuantifyText(ItemName)} - {base.ToString()}";

        /// <summary>
        /// Retorna a jornada de trabalho + hora de almoço de uma data de acordo com as
        /// configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan TotalTime(DateTime Date) => GetJourneyDay(Date).TotalTime;

        #endregion Public Methods
    }
}