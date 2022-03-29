using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace InnerLibs.TimeMachine
{
    public class Fortnight
    {
        private static string _format;

        private DateRange _period;

        /// <summary>
        /// Cria uma instancia de quinzena a partir de uma data que a mesma pode conter
        /// </summary>
        /// <param name="AnyDate">Qualquer data. Se NULL, a data atual é utilizada</param>
        public Fortnight(DateTime? AnyDate = default)
        {
            AnyDate = AnyDate ?? DateTime.Now;
            AnyDate = new DateTime(AnyDate.Value.Year, AnyDate.Value.Month, AnyDate.Value.Day > 15 ? 16 : 1, AnyDate.Value.Hour, AnyDate.Value.Minute, AnyDate.Value.Second, AnyDate.Value.Millisecond, AnyDate.Value.Kind);
            var EndDate = AnyDate;
            EndDate = EndDate.Value.AddDays(1d);
            if (EndDate.Value.Day <= 15)
            {
                EndDate = new DateTime(EndDate.Value.Year, EndDate.Value.Month, 15, EndDate.Value.Hour, EndDate.Value.Minute, EndDate.Value.Second, EndDate.Value.Millisecond, EndDate.Value.Kind);
            }
            else
            {
                EndDate = EndDate.Value.LastDayOfMonth();
            }

            Period = new DateRange(AnyDate.Value, EndDate.Value);
        }

        /// <summary>
        /// Define o formato global de uma string de uma <see cref="Fortnight"/>
        /// </summary>
        public static string Format { get => _format.IfBlank("{q}{o} - {mmmm}/{yyyy}"); set => _format = value; }

        /// <summary>
        /// String que identifica a quinzena em uma coleção
        /// </summary>
        /// <returns></returns>
        public string Key => Number.ToString() + "@" + Period.EndDate.ToString("MM-yyyy");

        /// <summary>
        /// Numero da quinzena (1 ou 2)
        /// </summary>
        /// <returns></returns>
        public int Number => Period.EndDate.Day <= 15 ? 1 : 2;

        /// <summary>
        /// Periodo que esta quinzena possui
        /// </summary>
        /// <returns></returns>
        public DateRange Period { get => _period.Clone(); private set => _period = value; }

        /// <summary>
        /// Retorna a Key de um <see cref="Fortnight"/> em um formato especifico.
        /// </summary>
        /// <param name="Format">Formato da string</param>
        /// <remarks>
        /// <list type="number">
        /// <listheader>
        /// <term>Marcação</term>
        /// <description>Descrição</description>
        /// </listheader>
        /// <item>
        /// <term>{f} ou {q}</term>
        /// <description>Retorna o numero da quinzena com 1 digito. EX.: "1", "2"</description>
        /// </item>
        /// <item>
        /// <term>{ff} ou {qq}</term>
        /// <description>Retorna o numero da quinzena com 2 digitos. EX.: "01", "02"</description>
        /// </item>
        /// <item>
        /// <term>{o}</term>
        /// <description>
        /// Retorna sufixo ordinal da quinzena atual. EX.: "st", "nd", "rd", "th"
        /// </description>
        /// </item>
        /// <item>
        /// <term>{s}</term>
        /// <description>
        /// Retorna o numero do primeiro dia da quinzena com 1 digito. EX.: "1", "2", "30","31"
        /// </description>
        /// </item>
        /// <item>
        /// <term>{ss}</term>
        /// <description>
        /// Retorna o numero do primeiro dia da quinzena com 2 digitos. EX.: "01", "02","30","31"
        /// </description>
        /// </item>
        /// <item>
        /// <term>{e} ou {ee}</term>
        /// <description>
        /// Retorna o numero do ultimo dia da quinzena com 1 digito. EX.: "1", "2", "30","31"
        /// </description>
        /// </item>
        /// <item>
        /// <term>{m}</term>
        /// <description>Retorna o numero do mês da quinzena com 1 digito. EX.: "1", "2","11","12"</description>
        /// </item>
        /// <item>
        /// <term>{mm}</term>
        /// <description>Retorna o numero do mês da quinzena com 2 digitos. EX.: "01", "02","11","12"</description>
        /// </item>
        /// <item>
        /// <term>{mmm}</term>
        /// <description>Retorna o nome do mês da quinzena abreviado. EX.: "Jan", "Fev","Nov","Dez"</description>
        /// </item>
        /// <item>
        /// <term>{mmmm}</term>
        /// <description>Retorna o nome do mês da quinzena. EX.: "Janeiro", "Fevereiro","Novembro","Dezembro"</description>
        /// </item>
        /// <item>
        /// <term>{y} ou {yy} ou {a} ou {aa}</term>
        /// <description>Retorna os 2 ultimos números do ano da quinzena. EX.: "18", "19","20"</description>
        /// </item>
        /// <item>
        /// <term>{yyy} ou {yyyy} ou {aaa} ou {aaaa}</term>
        /// <description>Retorna o número do ano da quinzena. EX.: "2018", "2019","2020"</description>
        /// </item>
        /// </list>
        /// </remarks>
        /// <returns>Uma string no formato especificado</returns>
        public string FormatName(string Format = null, CultureInfo Culture = null)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            Format = Format.IfBlank(Fortnight.Format);

            int dia_inicio = Period.StartDate.Day;
            Format = Format.Replace("{s}", dia_inicio.ToString("#", Culture));
            Format = Format.Replace("{ss}", dia_inicio.ToString("##", Culture));
            Format = Format.Replace("{e}", _period.EndDate.Day.ToString("#", Culture));
            Format = Format.Replace("{ee}", _period.EndDate.Day.ToString("##", Culture));
            Format = Format.Replace("{f}", Number.ToString("#", Culture));
            Format = Format.Replace("{ff}", Number.ToString("##", Culture));
            Format = Format.Replace("{q}", Number.ToString("#", Culture));
            Format = Format.Replace("{qq}", Number.ToString("##", Culture));
            Format = Format.Replace("{m}", _period.EndDate.Month.ToString("#", Culture));
            Format = Format.Replace("{mm}", _period.EndDate.Month.ToString("##", Culture));
            Format = Format.Replace("{mmm}", _period.EndDate.Month.ToShortMonthName(Culture));
            Format = Format.Replace("{mmmm}", _period.EndDate.Month.ToLongMonthName(Culture));
            Format = Format.Replace("{y}", _period.EndDate.Year.ToString(Culture).GetLastChars(2));
            Format = Format.Replace("{yy}", _period.EndDate.Year.ToString(Culture).GetLastChars(2));
            Format = Format.Replace("{yyy}", _period.EndDate.Year.ToString(Culture));
            Format = Format.Replace("{yyyy}", _period.EndDate.Year.ToString(Culture));
            Format = Format.Replace("{a}", _period.EndDate.Year.ToString(Culture).GetLastChars(2));
            Format = Format.Replace("{aa}", _period.EndDate.Year.ToString(Culture).GetLastChars(2));
            Format = Format.Replace("{aaa}", _period.EndDate.Year.ToString(Culture));
            Format = Format.Replace("{aaaa}", _period.EndDate.Year.ToString(Culture));
            Format = Format.Replace("{o}", Number.ToOrdinalNumber(true));
            return Format;
        }

        public string FormatName(CultureInfo Culture) => FormatName(null, Culture);

        public override string ToString() => FormatName();
    }

    public class FortnightGroup<DataType> : FortnightGroup
    {
        /// <summary>
        /// Instancia um novo <see cref="FortnightGroup(Of DataType)"/> a partir de uma data inicial
        /// e um numero fixo de quinzenas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="FortnightCount"></param>
        public FortnightGroup(DateTime StartDate = default, int FortnightCount = 1) : base(StartDate, FortnightCount)
        {
        }

        public List<DataType> DataCollection { get; set; } = new List<DataType>();
        public List<Func<DataType, DateTime>> DateSelector { get; set; } = new List<Func<DataType, DateTime>>();

        /// <summary>
        /// Retorna da <see cref="DataCollection"/> os valores correspondentes a quinzena especificada
        /// </summary>
        /// <param name="Fort">Quinzena</param>
        /// <returns></returns>
        public IEnumerable<DataType> this[Fortnight Fort] => this[Fort.Key];

        /// <summary>
        /// Retorna da <see cref="DataCollection"/> os valores correspondentes a quinzena
        /// especificada em <paramref name="Key"/>
        /// </summary>
        /// <param name="Key">Key da quinzena q@MM-YYYY</param>
        /// <returns></returns>
        public new IEnumerable<DataType> this[string Key]
        {
            get
            {
                var lista = new List<DataType>();
                foreach (var ii in DataCollection)
                {
                    var datas = new List<DateTime>();
                    foreach (var sel in DateSelector)
                        datas.Add(sel(ii));
                    var periodo1 = new DateRange(datas.Min(), datas.Max());
                    var periodo2 = new DateRange((DateTime)base[(string)Key].Period.StartDate, (DateTime)base[(string)Key].Period.EndDate);
                    if (periodo1.MatchAny(periodo2))
                    {
                        lista.Add(ii);
                    }
                }

                if (lista.Count > 0)
                {
                    lista = lista.OrderBy(x => DateSelector.NullCoalesce()).Distinct().ToList();
                }

                return lista;
            }
        }

        /// <summary>
        /// Cria um <see cref="FortnightGroup(Of DataType)"/> a partir de uma coleção de objetos
        /// </summary>
        /// <param name="Range">
        /// Periodo especifico que este grupo irá abranger idependentemente das datas em <paramref name="DateSelector"/>
        /// </param>
        /// <param name="Data">Coleção de objetos</param>
        /// <param name="DateSelector">
        /// Expressão Lambda que indica quais campos do objeto contém uma data que deve ser utilizada
        /// </param>
        /// <returns></returns>
        public static FortnightGroup<DataType> CreateFromDataGroup(IEnumerable<DataType> Data, DateRange Range, params Func<DataType, DateTime>[] DateSelector)
        {
            FortnightGroup<DataType> fort;
            fort = CreateFromDateRange(Range.StartDate, Range.EndDate);
            if (Data != null && Data.Any())
            {
                fort.DataCollection = Data.ToList();
            }

            if (DateSelector != null && DateSelector.Any())
            {
                fort.DateSelector = DateSelector.ToList();
            }

            return fort;
        }

        /// <summary>
        /// Cria um <see cref="FortnightGroup(Of DataType)"/> a partir de uma coleção de objetos
        /// </summary>
        /// <param name="Data">Coleção de objetos</param>
        /// <param name="DateSelector">
        /// Expressão Lambda que indica quais campos do objeto contém uma data que deve ser utilizada
        /// </param>
        /// <returns></returns>
        public static FortnightGroup<DataType> CreateFromDataGroup(IEnumerable<DataType> Data, params Func<DataType, DateTime>[] DateSelector)
        {
            var datas = new List<DateTime?>();
            if (DateSelector == null || !DateSelector.Any())
            {
                throw new ArgumentNullException("DateSelector is null or empty");
            }
            else
            {
                foreach (var dd in DateSelector)
                {
                    datas.Add(Data.OrderBy(dd).Select(dd).First());
                    datas.Add(Data.OrderBy(dd).Select(dd).Last());
                }

                datas = datas.Distinct().Where(x => x.HasValue).OrderBy(x => x).ToList();
                return CreateFromDataGroup(Data, new DateRange((DateTime)datas.First(), (DateTime)datas.Last()), DateSelector);
            }
        }

        /// <summary>
        /// Cria um <see cref="FortnightGroup(Of DataType)"/> a partir de uma data inicial e uma
        /// data final
        /// </summary>
        /// <param name="StartDate">Data inicial</param>
        /// <param name="EndDate">Data Final</param>
        /// <returns></returns>
        public new static FortnightGroup<DataType> CreateFromDateRange(DateTime StartDate, DateTime EndDate)
        {
            Calendars.FixDateOrder(ref StartDate, ref EndDate);
            int fortcount = 1;
            var fort = new FortnightGroup<DataType>(StartDate, fortcount);
            while (fort.EndDate < EndDate)
            {
                fortcount++;
                fort = new FortnightGroup<DataType>(StartDate, fortcount);
            }

            return fort;
        }

        /// <summary>
        /// Cria um <see cref="FortnightGroup(Of DataType)"/> a partir de uma data inicial e uma
        /// data final
        /// </summary>
        /// <param name="StartDate">Data inicial</param>
        /// <param name="EndDate">Data Final</param>
        /// <returns></returns>
        public new static FortnightGroup<DataType> CreateFromDateRange(DateRange DateRange) => CreateFromDateRange((DateTime)DateRange.StartDate, (DateTime)DateRange.EndDate);

        /// <summary>
        /// Retorna um <see cref="Dictionary(Of String, DataType)"/> com as informações agrupadas
        /// por quinzena
        /// </summary>
        /// <returns></returns>
        public Dictionary<Fortnight, IEnumerable<DataType>> ToDataDictionary(bool IncludeFortnightsWithoutData = true)
        {
            var d = new Dictionary<Fortnight, IEnumerable<DataType>>();
            foreach (var k in this)
            {
                var dt = this[k.Key];
                if (dt.Any() || IncludeFortnightsWithoutData)
                {
                    d.Add(k, dt);
                }
            }

            return d;
        }
    }

    /// <summary>
    /// Lista de dias agrupados em quinzenas
    /// </summary>
    public class FortnightGroup : ReadOnlyCollection<Fortnight>
    {
        /// <summary>
        /// Instancia um novo <see cref="FortnightGroup"/> a partir de uma data e um numero de quinzenas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="FortnightCount"></param>
        private static List<Fortnight> GerarLista(DateTime StartDate = default, int FortnightCount = 1)
        {
            var l = new List<Fortnight>
            {
                new Fortnight(StartDate)
            };
            for (int index = 2, loopTo = FortnightCount.SetMinValue(1); index <= loopTo; index++)
            {
                StartDate = l.Last().Period.EndDate.AddDays(1d);
                l.Add(new Fortnight(StartDate));
            }

            return l;
        }

        /// <summary>
        /// Instancia um novo <see cref="FortnightGroup"/> a partir de uma data e um numero de quinzenas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="FortnightCount"></param>
        public FortnightGroup(DateTime StartDate = default, int FortnightCount = 1) : base(GerarLista(StartDate, FortnightCount))
        {
        }

        /// <summary>
        /// Retorna uma lista com todos os dias entre as quinzenas
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> AllDays => this.SelectMany(x => x.Period.StartDate.GetDaysBetween(x.Period.EndDate));

        /// <summary>
        /// Retorna a ultima data do periodo
        /// </summary>
        /// <returns></returns>
        public DateTime EndDate => this.Last().Period.EndDate;

        /// <summary>
        /// Retorna um periodo equivalente a este grupo de quinzena
        /// </summary>
        /// <returns></returns>
        public DateRange Period => new DateRange(StartDate, EndDate);

        /// <summary>
        /// Retorna a data inicial do periodo
        /// </summary>
        /// <returns></returns>
        public DateTime StartDate => this.First().Period.StartDate;

        /// <summary>
        /// Retorna uma quinzena a partir da sua Key
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Fortnight this[string Key] => base[IndexOf(this.Where(x => (x.Key ?? "") == (Key ?? "")).SingleOrDefault())];

        /// <summary>
        /// Retorna uma quinzena a partir da sua Index
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public new Fortnight this[int Index] => base[Index];

        /// <summary>
        /// Cria um grupo de quinzenas entre 2 datas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        public static FortnightGroup CreateFromDateRange(DateTime StartDate, DateTime EndDate)
        {
            Calendars.FixDateOrder(ref StartDate, ref EndDate);
            int fortcount = 1;
            var fort = new FortnightGroup(StartDate, fortcount);
            while (fort.EndDate < EndDate)
            {
                fortcount = fortcount + 1;
                fort = new FortnightGroup(StartDate, fortcount);
            }

            return fort;
        }

        /// <summary>
        /// Cria um grupo de quinzenas entre 2 datas
        /// </summary>
        /// <param name="Range">Periodo</param>
        /// <returns></returns>
        public static FortnightGroup CreateFromDateRange(DateRange Range) => CreateFromDateRange(Range.StartDate, Range.EndDate);
    }

    /// <summary>
    /// Dia de Uma Demanda
    /// </summary>
    public class JourneyDay
    {
        private DateTime lunch_hour;

        private DateTime start_hour;

        /// <summary>
        /// Inicia uma instancia de dia letivo
        /// </summary>
        public JourneyDay()
        {
            StartHour = DateTime.MinValue;
            LunchHour = DateTime.MinValue.AddHours(12d);
        }

        /// <summary>
        /// Inicia uma instancia de dia letivo
        /// </summary>
        /// <param name="StartHour">Hora Inicial</param>
        /// <param name="Journey">Jornada de trabalho</param>
        public JourneyDay(DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default) => SetJourney(StartHour, Journey, LunchHour, LunchTime);

        /// <summary>
        /// Hora que se encerra a jornada (inclui hora de almoço)
        /// </summary>
        /// <returns></returns>
        public DateTime EndHour => StartHour.Add(TotalTime);

        public bool IsJourney => this.JourneyTime.Milliseconds > 0;

        /// <summary>
        /// Jornada de Trabalho/Produção
        /// </summary>
        /// <returns></returns>
        public TimeSpan JourneyTime { get; set; } = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Hora de almoco
        /// </summary>
        /// <returns></returns>
        public DateTime LunchHour
        {
            get => lunch_hour;

            set => lunch_hour = DateTime.MinValue.Add(new TimeSpan(value.TimeOfDay.Ticks));
        }

        /// <summary>
        /// Hora de Almoço
        /// </summary>
        /// <returns></returns>
        public TimeSpan LunchTime { get; set; } = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Hora inicial da jornada
        /// </summary>
        /// <returns></returns>
        public DateTime StartHour
        {
            get => start_hour;

            set => start_hour = DateTime.MinValue.Add(new TimeSpan(value.TimeOfDay.Ticks));
        }

        /// <summary>
        /// Jornada + hora de Almoço
        /// </summary>
        /// <returns></returns>
        public TimeSpan TotalTime => JourneyTime + LunchTime;

        /// <summary>
        /// Define a hora inicial e a jornada de trabalho deste dia
        /// </summary>
        /// <param name="StartHour"></param>
        /// <param name="Journey"></param>
        /// <param name="LunchTime">Horas de Almoço</param>
        public JourneyDay SetJourney(DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default)
        {
            this.StartHour = StartHour;
            this.JourneyTime = Journey;
            this.LunchHour = LunchHour == default ? DateTime.MinValue.Date.AddHours(12d) : LunchHour;
            this.LunchTime = LunchTime == default ? new TimeSpan(0, 0, 0) : LunchTime;
            return this;
        }
    }

    /// <summary>
    /// Classe base para calculo de demandas
    /// </summary>
    public class TimeDemand
    {
        private DateRange _dateRange;

        private DateTime? _EndDate = null;

        private JourneyDay _friday = new JourneyDay();

        private JourneyDay _monday = new JourneyDay();

        private int _quantity = 1;

        private JourneyDay _saturday = new JourneyDay();

        private JourneyDay _sunday = new JourneyDay();

        private JourneyDay _thursday = new JourneyDay();

        private JourneyDay _tuesday = new JourneyDay();

        private JourneyDay _wednesday = new JourneyDay();

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
            this.ItemQuantity = Quantity;
            this.ItemProductionTime = Time;
            this.Proccess(StartDate);
        }

        /// <summary>
        /// Inicia uma nova Demanda com as propriedades do item
        /// </summary>
        /// <param name="StartDate">Data Inicial da produção</param>
        /// <param name="EndDate">Tempo do item</param>
        /// <param name="Quantity">Quantidade de itens</param>
        public TimeDemand(DateTime StartDate, DateTime EndDate, int Quantity = 1) : this(StartDate, new TimeSpan(0), Quantity) => this.ItemProductionTime = GetWorkingTimeUntil(EndDate);

        public DateRange DateRange
        {
            get
            {
                _dateRange = _dateRange ?? new DateRange() { ForceFirstAndLastMoments = false };
                return _dateRange;
            }
        }

        /// <summary>
        /// Data de encerramento da produção
        /// </summary>
        /// <returns></returns>
        public DateTime EndDate
        {
            get
            {
                if (_EndDate == null)
                {
                    _EndDate = Proccess(this.StartDate);
                }

                return _EndDate.Value;
            }
        }

        /// <summary>
        /// Sexta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Friday
        {
            get
            {
                _friday = _friday ?? new JourneyDay();
                return _friday;
            }
            set
            {
                _friday = value ?? new JourneyDay();
            }
        }

        /// <summary>
        /// Feriados, pontos facultativos e/ou datas especificas consideradas não relevantes
        /// </summary>
        /// <returns></returns>
        public List<DateTime> HoliDays { get; set; } = new List<DateTime>();

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
        public JourneyDay Monday
        {
            get
            {
                _monday = _monday ?? new JourneyDay();
                return _monday;
            }
            set
            {
                _monday = value ?? new JourneyDay();
            }
        }

        /// <summary>
        /// Dias não relevantes (nao letivos e feriados) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> NonRelevantDays => GetWorkDays().ClearTime().Where(x => x.IsNotIn(RelevantDays));

        /// <summary>
        /// Dias da semana não relevantes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayOfWeek> NonRelevantDaysOfWeek => new DayOfWeek[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday }.Where(x => x.IsNotIn(RelevantDaysOfWeek));

        /// <summary>
        /// Tempo totald e produção de todos os itens
        /// </summary>
        /// <returns></returns>
        public TimeSpan ProductionTime => new TimeSpan(ItemQuantity * ItemProductionTime.Ticks);

        /// <summary>
        /// Dias relevantes (letivos) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> RelevantDays => GetWorkDays(RelevantDaysOfWeek.ToArray()).ClearTime().Where(x => x.IsNotIn(HoliDays.ClearTime()));

        /// <summary>
        /// Dias da semana relevantes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayOfWeek> RelevantDaysOfWeek
        {
            get
            {
                var dias = new List<DayOfWeek>();
                if (Sunday.JourneyTime.TotalMilliseconds > 0d) dias.Add(DayOfWeek.Sunday);
                if (Monday.JourneyTime.TotalMilliseconds > 0d) dias.Add(DayOfWeek.Monday);
                if (Tuesday.JourneyTime.TotalMilliseconds > 0d) dias.Add(DayOfWeek.Tuesday);
                if (Wednesday.JourneyTime.TotalMilliseconds > 0d) dias.Add(DayOfWeek.Wednesday);
                if (Thursday.JourneyTime.TotalMilliseconds > 0d) dias.Add(DayOfWeek.Thursday);
                if (Friday.JourneyTime.TotalMilliseconds > 0d) dias.Add(DayOfWeek.Friday);
                if (Saturday.JourneyTime.TotalMilliseconds > 0d) dias.Add(DayOfWeek.Saturday);
                if (dias.Count == 0) dias.AddRange(new[] { 0, 1, 2, 3, 4, 5, 6 }.Select(x => (DayOfWeek)x));
                return dias.ToArray();
            }
        }

        /// <summary>
        /// Sábado
        /// </summary>
        /// <returns></returns>
        public JourneyDay Saturday
        {
            get
            {
                _saturday = _saturday ?? new JourneyDay();
                return _saturday;
            }
            set
            {
                _saturday = value ?? new JourneyDay();
            }
        }

        /// <summary>
        /// Data Inicial da produção
        /// </summary>
        /// <returns></returns>
        public DateTime StartDate
        {
            get => DateRange.StartDate;
            set => Proccess(value);
        }

        /// <summary>
        /// Domingo
        /// </summary>
        /// <returns></returns>
        public JourneyDay Sunday
        {
            get
            {
                _sunday = _sunday ?? new JourneyDay();
                return _sunday;
            }
            set
            {
                _sunday = value ?? new JourneyDay();
            }
        }

        /// <summary>
        /// Quinta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Thursday
        {
            get
            {
                _thursday = _thursday ?? new JourneyDay();
                return _thursday;
            }
            set
            {
                _thursday = value ?? new JourneyDay();
            }
        }

        /// <summary>
        /// Terça-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Tuesday
        {
            get
            {
                _tuesday = _tuesday ?? new JourneyDay();
                return _tuesday;
            }
            set
            {
                _tuesday = value ?? new JourneyDay();
            }
        }

        /// <summary>
        /// Quarta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Wednesday
        {
            get
            {
                _wednesday = _wednesday ?? new JourneyDay();
                return _wednesday;
            }
            set
            {
                _wednesday = value ?? new JourneyDay();
            }
        }

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

        /// <summary>
        /// Cria uma demanda após a demanda atual com as mesmas caracteristicas
        /// </summary>
        /// <param name="DelayTime">Tempo adicionado entre uma demanda e outra</param>
        /// <returns></returns>
        public TimeDemand CloneAndQueue(TimeSpan? DelayTime = null) => new TimeDemand(EndDate.Add(DelayTime ?? new TimeSpan(0L)), this.ItemProductionTime, ItemQuantity);

        public JourneyDay GetJourneyDay(DayOfWeek DoW)
        {
            switch (DoW)
            {
                case DayOfWeek.Sunday: return Sunday;
                case DayOfWeek.Monday: return Monday;
                case DayOfWeek.Tuesday: return Tuesday;
                case DayOfWeek.Wednesday: return Wednesday;
                case DayOfWeek.Thursday: return Thursday;
                case DayOfWeek.Friday: return Friday;
                case DayOfWeek.Saturday: return Saturday;
                default: return null;
            }
        }

        public JourneyDay GetJourneyDay(DateTime Date) => GetJourneyDay(Date.DayOfWeek);

        /// <summary>
        /// Retorna a porcentagem em relacao a posição de uma data entre a data inicial (0%) e final (100%)
        /// </summary>
        /// <param name="MidDate"></param>
        /// <returns></returns>
        public decimal GetPercentCompletion(DateTime MidDate) => MidDate.CalculateTimelinePercent(StartDate, EndDate);

        /// <summary>
        /// Dias especificos da semana entre as datas inicial e final da demanda
        /// </summary>
        /// <param name="DaysOfWeek">Dias da semana</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetWorkDays(params DayOfWeek[] DaysOfWeek) => StartDate.GetDaysBetween(EndDate, DaysOfWeek.ToArray());

        public TimeSpan GetWorkingTimeUntil(DateTime EndDate) => this.GetWorkTimeBetween(StartDate, EndDate);

        /// <summary>
        /// Retorna o intervalo de horas trabalhadas entre duas datas baseado nas configuracoes
        /// desta demanda
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetWorkTimeBetween(DateTime StartDate, DateTime EndDate) => new TimeSpan(this.RelevantDays.Where(x => x.DayOfWeek.IsIn(RelevantDaysOfWeek) && x.IsBetweenOrEqual(StartDate, EndDate)).Sum(x => GetJourneyDay(x).JourneyTime.Ticks));

        public bool IsJourney(DateTime DateAndTime) => GetJourneyDay(DateAndTime).JourneyTime.TotalMilliseconds > 0;

        /// <summary>
        /// Retorna a hora final da jornada de uma data acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime JourneyEndHour(DateTime Date) => GetJourneyDay(Date).EndHour;

        /// <summary>
        /// Retorna a hora inicial da jornada de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime JourneyStartHour(DateTime Date) => GetJourneyDay(Date).StartHour;

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
        public DateTime LunchEndHour(DateTime Date) => LunchStartHour(Date).Add(LunchTime(Date));

        /// <summary>
        /// Retorno a hora de inicio do almoço de uma data de acordo com as configurações desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime LunchStartHour(DateTime Date) => GetJourneyDay(Date).LunchHour;

        /// <summary>
        /// Retorna o tempo de almoço de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan LunchTime(DateTime Date) => GetJourneyDay(Date).LunchTime;

        public DateTime Proccess(DateTime StartDate)
        {
            StartDate = PushDateIntoJourney(StartDate);
            var FinalDate = StartDate.Add(ProductionTime);
            _dateRange = _dateRange ?? new DateRange(StartDate, FinalDate, RelevantDaysOfWeek.ToArray());
            _dateRange.StartDate = StartDate;
            _dateRange.EndDate = FinalDate;

            foreach (var dia in _dateRange.GetRelevantDays())
            {
                if (!(dia.Date == FinalDate.Date))
                {
                    FinalDate = FinalDate.AddHours(24d - this.TotalTime(dia).TotalHours);
                }
                else if (FinalDate.TimeOfDay > this.LunchStartHour(dia).TimeOfDay)
                {
                    FinalDate = FinalDate.Add(LunchTime(dia));
                }

                if (FinalDate.IsBetween(LunchStartHour(dia), LunchEndHour(dia)))
                {
                    FinalDate = FinalDate.Add(this.LunchEndHour(dia).TimeOfDay - dia.TimeOfDay);
                }
            }

            var nrd = HoliDays.Union(_dateRange.GetNonRelevantDays()).ClearTime().Distinct();

            FinalDate = FinalDate.AddDays(nrd.Count());
            var lasthours = new TimeSpan();
            if (FinalDate.TimeOfDay > this.JourneyEndHour(FinalDate).TimeOfDay)
            {
                lasthours = FinalDate.TimeOfDay - this.JourneyEndHour(FinalDate).TimeOfDay;
            }

            FinalDate = PushDateIntoJourney(FinalDate);
            FinalDate = FinalDate.Add(lasthours);
            _dateRange.EndDate = FinalDate;

            return FinalDate;
        }

        /// <summary>
        /// Empurra a data para dentro da proxima hora disponivel dentro jornada de trabalho
        /// </summary>
        /// <param name="[Date]">Data a ser Verificada</param>
        public DateTime PushDateIntoJourney(DateTime Date)
        {
            while (Date.TimeOfDay > this.JourneyEndHour(Date).TimeOfDay || Date.TimeOfDay < this.JourneyStartHour(Date).TimeOfDay || !RelevantDaysOfWeek.Contains(Date.DayOfWeek) || Date.IsBetween(LunchStartHour(Date), LunchEndHour(Date)))
                Date = Date.AddMilliseconds(1d);

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
        public JourneyDay SetJourney(DayOfWeek DayOfWeek, DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default)
        {
            switch (DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    Sunday = Sunday ?? new JourneyDay();
                    return Sunday.SetJourney(StartHour, Journey, LunchHour, LunchTime);

                case DayOfWeek.Tuesday:
                    Tuesday = Tuesday ?? new JourneyDay();
                    return Tuesday.SetJourney(StartHour, Journey, LunchHour, LunchTime);

                case DayOfWeek.Wednesday:
                    Wednesday = Wednesday ?? new JourneyDay();
                    return Wednesday.SetJourney(StartHour, Journey, LunchHour, LunchTime);

                case DayOfWeek.Thursday:
                    Thursday = Thursday ?? new JourneyDay();
                    return Thursday.SetJourney(StartHour, Journey, LunchHour, LunchTime);

                case DayOfWeek.Friday:
                    Friday = Friday ?? new JourneyDay();
                    return Friday.SetJourney(StartHour, Journey, LunchHour, LunchTime);

                case DayOfWeek.Saturday:
                    Saturday = Saturday ?? new JourneyDay();
                    return Saturday.SetJourney(StartHour, Journey, LunchHour, LunchTime);

                case DayOfWeek.Monday:
                default:
                    Monday = Monday ?? new JourneyDay();
                    return Monday.SetJourney(StartHour, Journey, LunchHour, LunchTime);
            }
        }

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
        public override string ToString() => ItemQuantity.ToString() + " " + ItemQuantity.QuantifyText(ItemName) + " - " + this.DateRange.ToString();

        /// <summary>
        /// Retorna a jornada de trabalho + hora de almoço de uma data de acordo com as
        /// configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan TotalTime(DateTime Date) => GetJourneyDay(Date).TotalTime;
    }
}