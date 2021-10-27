using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs.TimeMachine
{
    public class FortnightGroup<DataType> : FortnightGroup
    {
        public List<DataType> DataCollection { get; set; } = new List<DataType>();
        public List<Func<DataType, DateTime>> DateSelector { get; set; } = new List<Func<DataType, DateTime>>();

        /// <summary>
        /// Retorna da <see cref="DataCollection"/> os valores correspondentes a quinzena especificada
        /// </summary>
        /// <param name="Fort">Quinzena</param>
        /// <returns></returns>
        public IEnumerable<DataType> this[Fortnight Fort] => this[Fort.Key];

        /// <summary>
        /// Retorna da <see cref="DataCollection"/> os valores correspondentes a quinzena especificada em <paramref name="Key"/>
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
                    var periodo2 = new DateRange(base[Key].Period.StartDate, base[Key].Period.EndDate);
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
        /// Retorna um <see cref="Dictionary(Of String, DataType)"/> com as informações agrupadas por quinzena
        /// </summary>
        /// <returns></returns>
        public Dictionary<Fortnight, IEnumerable<DataType>> ToDataDictionary(bool IncludeFortnightsWithoutData = true)
        {
            var d = new Dictionary<Fortnight, IEnumerable<DataType>>();
            foreach (var k in this)
            {
                var dt = this[k.Key];
                if (dt.Count() > 0 || IncludeFortnightsWithoutData)
                {
                    d.Add(k, dt);
                }
            }

            return d;
        }

        /// <summary>
        /// Cria um <see cref="FortnightGroup(Of DataType)"/> a partir de uma coleção de objetos
        /// </summary>
        /// <param name="Range">Periodo especifico que este grupo irá abranger idependentemente das datas em <paramref name="DateSelector"/></param>
        /// <param name="Data">Coleção de objetos</param>
        /// <param name="DateSelector">Expressão Lambda que indica quais campos do objeto contém uma data que deve ser utilizada</param>
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
        /// <param name="DateSelector">Expressão Lambda que indica quais campos do objeto contém uma data que deve ser utilizada</param>
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
        /// Cria um <see cref="FortnightGroup(Of DataType)"/> a partir de uma data inicial e uma data final
        /// </summary>
        /// <param name="StartDate">Data inicial</param>
        /// <param name="EndDate">Data Final</param>
        /// <returns></returns>
        public static new FortnightGroup<DataType> CreateFromDateRange(DateTime StartDate, DateTime EndDate)
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
        /// Cria um <see cref="FortnightGroup(Of DataType)"/> a partir de uma data inicial e uma data final
        /// </summary>
        /// <param name="StartDate">Data inicial</param>
        /// <param name="EndDate">Data Final</param>
        /// <returns></returns>
        public static new FortnightGroup<DataType> CreateFromDateRange(DateRange DateRange) => CreateFromDateRange(DateRange.StartDate, DateRange.EndDate);


        /// <summary>
        /// Instancia um novo <see cref="FortnightGroup(Of DataType)"/> a partir de uma data inicial e um numero fixo de quinzenas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="FortnightCount"></param>
        public FortnightGroup(DateTime StartDate = default, int FortnightCount = 1) : base(StartDate, FortnightCount)
        {
        }
    }

    public class Fortnight
    {

        /// <summary>
        /// Define o formato global de uma string de uma <see cref="Fortnight"/>
        /// </summary>
        public static string Format { get => _format.IfBlank("{q}{o} - {mmmm}/{yyyy}"); set => _format = value; }

        private static string _format;

        /// <summary>
        /// Cria uma instancia de quinzena a partir de uma data que a mesma pode conter
        /// </summary>
        /// <param name="AnyDate">Qualquer data. Se NULL, a data atual é utilizada</param>
        public Fortnight(DateTime? AnyDate = default)
        {
            AnyDate ??= DateTime.Now;
            AnyDate = new DateTime(AnyDate.Value.Year, AnyDate.Value.Month, AnyDate.Value.Day > 15 ? 16 : 1, AnyDate.Value.Hour, AnyDate.Value.Minute, AnyDate.Value.Second, AnyDate.Value.Millisecond, AnyDate.Value.Kind);
            var EndDate = AnyDate;
            EndDate = EndDate.Value.AddDays(1d);
            if (EndDate.Value.Day <= 15)
            {
                EndDate = new DateTime(EndDate.Value.Year, EndDate.Value.Month, 15, EndDate.Value.Hour, EndDate.Value.Minute, EndDate.Value.Second, EndDate.Value.Millisecond, EndDate.Value.Kind);
            }
            else
            {
                EndDate = EndDate.Value.GetLastDayOfMonth();
            }

            Period = new DateRange((DateTime)AnyDate, (DateTime)EndDate);
        }

        /// <summary>
        /// String que identifica a quinzena em uma coleção
        /// </summary>
        /// <returns></returns>
        public string Key => Number.ToString() + "@" + Period.EndDate.ToString("MM-yyyy");

        private DateRange _period;

        /// <summary>
        /// Periodo que esta quinzena possui
        /// </summary>
        /// <returns></returns>
        public DateRange Period { get => _period.Clone(); private set => _period = value; }

        /// <summary>
        /// Numero da quinzena (1 ou 2)
        /// </summary>
        /// <returns></returns>
        public int Number => Period.EndDate.Day <= 15 ? 1 : 2;

        /// <summary>
        /// Retorna a Key de um <see cref="Fortnight"/> em um formato especifico.
        /// </summary>
        /// <param name="Format">Formato da string</param>
        /// <remarks>
        /// <list type="number">
        /// <listheader>
        /// <term> Marcação </term>
        /// <description> Descrição </description>
        /// </listheader>
        /// <item><term>{f} ou {q}</term><description> Retorna o numero da quinzena com 1 digito. EX.: "1", "2"</description></item>
        /// <item><term>{ff} ou {qq}</term><description> Retorna o numero da quinzena com 2 digitos. EX.: "01", "02"</description></item>
        /// <item><term>{o}</term><description> Retorna sufixo ordinal da quinzena atual. EX.: "st", "nd", "rd", "th"</description></item>
        /// <item><term>{s}</term><description> Retorna o numero do primeiro dia da quinzena com 1 digito. EX.: "1", "2", "30","31"</description></item>
        /// <item><term>{ss}</term><description> Retorna o numero do primeiro dia da quinzena com 2 digitos. EX.: "01", "02","30","31"</description></item>
        /// <item><term>{e} ou {ee}</term><description> Retorna o numero do ultimo dia da quinzena com 1 digito. EX.: "1", "2", "30","31"</description></item>
        /// <item><term>{m}</term><description> Retorna o numero do mês da quinzena com 1 digito. EX.: "1", "2","11","12"</description></item>
        /// <item><term>{mm}</term><description> Retorna o numero do mês da quinzena com 2 digitos. EX.: "01", "02","11","12"</description></item>
        /// <item><term>{mmm}</term><description> Retorna o nome do mês da quinzena abreviado. EX.: "Jan", "Fev","Nov","Dez"</description></item>
        /// <item><term>{mmmm}</term><description> Retorna o nome do mês da quinzena. EX.: "Janeiro", "Fevereiro","Novembro","Dezembro"</description></item>
        /// <item><term>{y} ou {yy} ou {a} ou {aa}</term><description> Retorna os 2 ultimos números do ano da quinzena. EX.: "18", "19","20"</description></item>
        /// <item><term>{yyy} ou {yyyy} ou {aaa} ou {aaaa}</term><description> Retorna o número do ano da quinzena. EX.: "2018", "2019","2020"</description></item>
        /// </list>
        /// </remarks>
        /// <returns>Uma string no formato especificado</returns>
        public string FormatName(string Format = null, CultureInfo Culture = null)
        {
            Culture ??= CultureInfo.CurrentCulture;
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

    /// <summary>
    /// Lista de dias agrupados em quinzenas
    /// </summary>
    public class FortnightGroup : ReadOnlyCollection<Fortnight>
    {
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
        /// Retorna a data inicial do periodo
        /// </summary>
        /// <returns></returns>
        public DateTime StartDate => this.First().Period.StartDate;

        /// <summary>
        /// Retorna a ultima data do periodo
        /// </summary>
        /// <returns></returns>
        public DateTime EndDate => this.Last().Period.EndDate;

        /// <summary>
        /// Retorna uma lista com todos os dias entre as quinzenas
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> AllDays => this.SelectMany(x => x.Period.StartDate.GetDaysBetween(x.Period.EndDate));

        /// <summary>
        /// Retorna um periodo equivalente a este grupo de quinzena
        /// </summary>
        /// <returns></returns>
        public DateRange Period => new DateRange(StartDate, EndDate);

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
        public static FortnightGroup CreateFromDateRange(DateRange Range)
        {
            return CreateFromDateRange(Range.StartDate, Range.EndDate);
        }
    }

    /// <summary>
    /// Classe para comparação entre 2 Datas com possibilidade de validação de dias Relevantes
    /// </summary>
    public class LongTimeSpan
    {
        /// <summary>
        /// Inicia uma instancia de TimeFlow
        /// </summary>
        /// <param name="StartDate">Data inicial</param>
        /// <param name="EndDate">Data Final (data mais recente)</param>
        /// <param name="RelevantDaysOfWeek">Lista de dias da semana que são relevantes (dias letivos)</param>
        public LongTimeSpan(DateTime StartDate, DateTime EndDate, params DayOfWeek[] RelevantDaysOfWeek)
        {
            Calendars.FixDateOrder(ref StartDate, ref EndDate);
            var CurDate = StartDate;
            int years = 0;
            int months = 0;
            int days = 0;
            var _phase = Phase.Years;

            if (!RelevantDaysOfWeek.Any())
            {
                RelevantDaysOfWeek = new DayOfWeek[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday };
            }

            while (CurDate <= EndDate)
            {
                if (RelevantDaysOfWeek.Contains(CurDate.DayOfWeek))
                {
                    RelevantDays.Add(CurDate);
                }
                else
                {
                    NonRelevantDays.Add(CurDate);
                }

                try
                {
                    CurDate = CurDate.AddDays(1d);
                }
                catch
                {
                }
            }

            CurDate = StartDate;
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
                                var timespan = EndDate - CurDate;
                                Years = years;
                                Months = months;
                                Days = days;
                                Hours = timespan.Hours;
                                Minutes = timespan.Minutes;
                                Seconds = timespan.Seconds;
                                Milliseconds = timespan.Milliseconds;
                                this.StartDate = StartDate;
                                this.EndDate = EndDate;
                                this.RelevantDaysOfWeek = RelevantDaysOfWeek.ToList();
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

        /// <summary>
        /// Inicia uma instancia de TimeFlow a partir de um TimeSpan
        /// </summary>
        /// <param name="Span">Intervalo de tempo</param>
        public LongTimeSpan(TimeSpan Span) : this(DateTime.MinValue, Span)
        {
        }

        /// <summary>
        /// Inicia uma instancia de TimeFlow a partir de uma data inicial e um TimeSpan
        /// </summary>
        /// <param name="StartDate">Data Inicial</param>
        /// <param name="Span">Intervalo de tempo</param>
        public LongTimeSpan(DateTime StartDate, TimeSpan Span) : this(StartDate, StartDate.Add(Span))
        {
        }

        /// <summary>
        /// Data Inicial
        /// </summary>
        /// <returns></returns>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Data Final
        /// </summary>
        /// <returns></returns>
        public DateTime EndDate { get; private set; }

        /// <summary>
        /// Dias Relevantes entre as datas Inicial e Final
        /// </summary>
        /// <returns></returns>
        public List<DateTime> RelevantDays { get; private set; } = new List<DateTime>();

        public decimal TotalMilliseconds => (decimal)(EndDate - StartDate).TotalMilliseconds;

        public decimal TotalSeconds => (decimal)(EndDate - StartDate).TotalSeconds;


        public decimal TotalMinutes => (decimal)(EndDate - StartDate).TotalMinutes;


        public decimal TotalHours => (decimal)(EndDate - StartDate).TotalHours;


        public decimal TotalDays => (decimal)(EndDate - StartDate).TotalDays;


        public decimal TotalMonths => (decimal)Math.Round((EndDate - StartDate).TotalDays / (365.25d / 12d), 2);


        public decimal TotalYears => Math.Round(TotalMonths / 12m, 2);

        public decimal TotalWeeks => Math.Round(TotalDays / 7m, 2);


        /// <summary>
        /// Todos os dias entre as datas Inicial e Final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> GetAllDays() => RelevantDays.Union(NonRelevantDays).OrderBy(x => x).AsEnumerable();

        /// <summary>
        /// Dias não relevantes entre as datas Inicial e Final
        /// </summary>
        /// <returns></returns>
        public List<DateTime> NonRelevantDays { get; private set; } = new List<DateTime>();

        /// <summary>
        /// Dias da semana relevantes
        /// </summary>
        /// <returns></returns>
        public List<DayOfWeek> RelevantDaysOfWeek { get; private set; } = new List<DayOfWeek>();

        /// <summary>
        /// Dias da semana não relevantes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayOfWeek> NonRelevantDaysOfWeek
        {
            get
            {
                var lista = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday }.ToList();
                var lista2 = new List<DayOfWeek>();
                lista2.AddRange(lista);
                foreach (DayOfWeek item in lista)
                {
                    if (RelevantDaysOfWeek.Contains(item))
                    {
                        lista2.Remove(item);
                    }
                }

                return lista2.AsEnumerable();
            }
        }

        /// <summary>
        /// Numero de Anos
        /// </summary>
        /// <returns></returns>
        public int Years { get; private set; }

        /// <summary>
        /// Numero de Meses
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
        public int Hours { get; private set; }

        /// <summary>
        /// Numero de Minutos
        /// </summary>
        /// <returns></returns>
        public int Minutes { get; private set; }

        /// <summary>
        /// Numero de Segundos
        /// </summary>
        /// <returns></returns>
        public int Seconds { get; private set; }

        /// <summary>
        /// Numero de milisegundos
        /// </summary>
        /// <returns></returns>
        public int Milliseconds { get; private set; }

        /// <summary>
        /// Retorna uma String no formato "X anos, Y meses e Z dias"
        /// </summary>
        /// <param name="FullString">Parametro que indica se as horas, minutos e segundos devem ser apresentados caso o tempo seja maior que 1 dia</param>
        /// <returns>string</returns>
        public string ToTimeElapsedString(string AndWord, string YearsWord, string MonthsWord, string DaysWord, string HoursWord, string MinutesWord, string SecondsWord, LongTimeSpanString Format = LongTimeSpanString.FullStringSkipZero)
        {

            string ano = Text.QuantifyText(YearsWord, Years).Prepend($"{Years} ").NullIf(x => YearsWord.IsBlank());
            string mes = Text.QuantifyText(MonthsWord, Months).Prepend($"{Months} ").NullIf(x => MonthsWord.IsBlank());
            string dia = Text.QuantifyText(DaysWord, Days).Prepend($"{Days} ").NullIf(x => DaysWord.IsBlank());
            string horas = Text.QuantifyText(HoursWord, Hours).Prepend($"{Hours} ").NullIf(x => HoursWord.IsBlank());
            string minutos = Text.QuantifyText(MinutesWord, Minutes).Prepend($"{Minutes} ").NullIf(x => MinutesWord.IsBlank());
            string segundos = Text.QuantifyText(SecondsWord, Seconds).Prepend($"{Seconds} ").NullIf(x => SecondsWord.IsBlank());


            var flagInt = (int)Format;
            if (flagInt >= 1) //skip zero
            {
                ano = ano.NullIf(x => Years == 0);
                mes = mes.NullIf(x => Months == 0);
                dia = dia.NullIf(x => Days == 0);
                horas = horas.NullIf(x => Hours == 0);
                minutos = minutos.NullIf(x => Minutes == 0);
                segundos = segundos.NullIf(x => Seconds == 0);
            }

            if (flagInt >= 2) // reduce days
            {
                horas = horas.NullIf(x => Days >= 1);
                minutos = minutos.NullIf(x => Days >= 1);
                segundos = segundos.NullIf(x => Days >= 1);
            }

            if (flagInt >= 3) //reduce months
            {

                dia = dia.NullIf(x => Months >= 1);
                horas = horas.NullIf(x => Months >= 1);
                minutos = minutos.NullIf(x => Months >= 1);
                segundos = segundos.NullIf(x => Months >= 1);
            }

            if (flagInt >= 4) // reduce most
            {
                mes = mes.NullIf(x => Years >= 1);
                dia = dia.NullIf(x => Years >= 1);
                horas = horas.NullIf(x => Years >= 1);
                minutos = minutos.NullIf(x => Years >= 1);
                segundos = segundos.NullIf(x => Years >= 1);
            }


            string current = new[] { ano, mes, dia, horas, minutos, segundos }.Where(x => x.IsNotBlank()).ToPhrase(AndWord);

            return current.AdjustWhiteSpaces();
        }


        public string ToTimeElapsedString() => this.ToTimeElapsedString("And", "Years", "Months", "Days", "Hours", "Minutes", "Seconds", LongTimeSpanString.FullStringSkipZero);

        /// <summary>
        /// Retorna uma string com a quantidade de itens e o tempo de produção
        /// </summary>
        /// <returns></returns>
        public override string ToString() => ToTimeElapsedString();



        private enum Phase
        {
            Years,
            Months,
            Days,
            Done
        }
    }

    /// <summary>
    /// Classe base para calculo de demandas
    /// </summary>
    public class TimeDemand
    {
        /// <summary>
        /// Domingo
        /// </summary>
        /// <returns></returns>
        public JourneyDay Sunday { get; set; } = new JourneyDay();

        /// <summary>
        /// Segunda-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Monday { get; set; } = new JourneyDay();

        /// <summary>
        /// Terça-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Tuesday { get; set; } = new JourneyDay();

        /// <summary>
        /// Quarta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Wednesday { get; set; } = new JourneyDay();

        /// <summary>
        /// Quinta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Thursday { get; set; } = new JourneyDay();

        /// <summary>
        /// Sexta-Feira
        /// </summary>
        /// <returns></returns>
        public JourneyDay Friday { get; set; } = new JourneyDay();

        /// <summary>
        /// Sábado
        /// </summary>
        /// <returns></returns>
        public JourneyDay Saturday { get; set; } = new JourneyDay();

        /// <summary>
        /// item da Produção
        /// </summary>
        /// <returns></returns>
        public Item Item { get; set; } = new Item();

        /// <summary>
        /// Data Inicial da produção
        /// </summary>
        /// <returns></returns>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Empurra a data para dentro da proxima hora disponivel dentro jornada de trabalho
        /// </summary>
        /// <param name="[Date]">Data a ser Verificada</param>
        public void PushDateIntoJourney(ref DateTime Date)
        {
            while (Date.TimeOfDay > this.JourneyEndHour(Date).TimeOfDay | Date.TimeOfDay < this.JourneyStartHour(Date).TimeOfDay | !RelevantDaysOfWeek.Contains(Date.DayOfWeek) | Date.IsBetween(LunchStartHour(Date), LunchEndHour(Date)))
                Date = Date.AddSeconds(1d);
        }

        /// <summary>
        /// Data de encerramento da produção
        /// </summary>
        /// <returns></returns>
        public DateTime EndDate
        {
            get
            {
                var argDate = StartDate;
                PushDateIntoJourney(ref argDate);
                StartDate = argDate;
                var FinalDate = StartDate.Add(Item.ProductionTime);
                var t = new LongTimeSpan(StartDate, FinalDate, RelevantDaysOfWeek.ToArray());
                foreach (var dia in t.RelevantDays)
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

                foreach (var feriado in HoliDays.ClearTime())
                {
                    if (!t.NonRelevantDays.ClearTime().Contains(feriado))
                    {
                        t.NonRelevantDays.Add(feriado);
                    }
                }

                FinalDate = FinalDate.AddDays(t.NonRelevantDays.Count);
                var lasthours = new TimeSpan();
                if (FinalDate.TimeOfDay > this.JourneyEndHour(FinalDate).TimeOfDay)
                {
                    lasthours = FinalDate.TimeOfDay - this.JourneyEndHour(FinalDate).TimeOfDay;
                }

                PushDateIntoJourney(ref FinalDate);
                FinalDate = FinalDate.Add(lasthours);
                return FinalDate;
            }
        }

        /// <summary>
        /// Inicia uma nova Demanda com as propriedades do item
        /// </summary>
        /// <param name="StartDate">Data Inicial da produção</param>
        /// <param name="Time">Tempo do item</param>
        /// <param name="Quantity">Quantidade de itens</param>
        public TimeDemand(DateTime StartDate, TimeSpan Time, int Quantity = 1, string MultipleItem = "Items", string SingularItem = null)
        {
            this.StartDate = StartDate;
            Item.Quantity = Quantity;
            Item.Time = Time;
            Item.MultipleItem = MultipleItem;
            Item.SingularItem = SingularItem.IfBlank(MultipleItem.Singularize());

        }

        /// <summary>
        /// Cria uma demanda após a demanda atual com as mesmas caracteristicas
        /// </summary>
        /// <param name="DelayTime">Tempo adicionado entre uma demanda e outra</param>
        /// <returns></returns>
        public TimeDemand CloneAndQueue(TimeSpan DelayTime = default)
        {
            if (DelayTime == default)
            {
                DelayTime = new TimeSpan(0L);
            }

            var enda = EndDate;
            return new TimeDemand(enda.Add(DelayTime), Item.Time, Item.Quantity, Item.SingularItem, Item.MultipleItem);
        }

        /// <summary>
        /// Dias especificos da semana entre as datas inicial e final da demanda
        /// </summary>
        /// <param name="DaysOfWeek">Dias da semana</param>
        /// <returns></returns>
        public IEnumerable<DateTime> GetWorkDays(params DayOfWeek[] DaysOfWeek) => StartDate.GetDaysBetween(EndDate, DaysOfWeek.ToArray());


        /// <summary>
        /// Dias relevantes (letivos) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> RelevantDays => GetWorkDays(RelevantDaysOfWeek.ToArray()).ClearTime().Where(x => x.IsNotIn(HoliDays.ClearTime()));

        /// <summary>
        /// Dias não relevantes (nao letivos e feriados) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> NonRelevantDays => GetWorkDays().ClearTime().Where(x => x.IsNotIn(RelevantDays));

        /// <summary>
        /// Retorna um TimeFlow desta demanda
        /// </summary>
        /// <returns></returns>
        public LongTimeSpan ToLongTimeSpan() => new LongTimeSpan(StartDate, EndDate, RelevantDaysOfWeek.ToArray());

        /// <summary>
        /// Retorna uma string representado a quantidade de itens e o tempo gasto com a produção
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Item.ToString() + " - " + ToTimeElapsedString();

        /// <summary>
        /// Retorna uma String no formato "X anos, Y meses e Z dias"
        /// </summary>
        /// <param name="FullString">Parametro que indica se as horas, minutos e segundos devem ser apresentados caso o tempo seja maior que 1 dia</param>
        /// <returns></returns>
        public string ToTimeElapsedString()
        {
            var data_final = EndDate;
            var data_inicial = StartDate;
            return data_inicial.GetDifference(data_final)
            .ToTimeElapsedString("And", "Years", "Months", "Days", "Hours", "Minutes", "Seconds", LongTimeSpanString.FullStringSkipZero);
        }

        /// <summary>
        /// inicia uma nova demanda
        /// </summary>
        public TimeDemand() => StartDate = DateTime.Now;

        /// <summary>
        /// Retorna a porcentagem em relacao a posição de uma data entre a data inicial (0%) e final (100%)
        /// </summary>
        /// <param name="MidDate"></param>
        /// <returns></returns>
        public decimal GetPercentCompletion(DateTime MidDate) => MidDate.CalculateTimelinePercent(StartDate, EndDate);

        /// <summary>
        /// Dias da semana relevantes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayOfWeek> RelevantDaysOfWeek
        {
            get
            {
                var dias = new List<DayOfWeek>();
                if (Sunday.JourneyTime.TotalHours > 0d)
                    dias.Add(DayOfWeek.Sunday);
                if (Monday.JourneyTime.TotalHours > 0d)
                    dias.Add(DayOfWeek.Monday);
                if (Tuesday.JourneyTime.TotalHours > 0d)
                    dias.Add(DayOfWeek.Tuesday);
                if (Wednesday.JourneyTime.TotalHours > 0d)
                    dias.Add(DayOfWeek.Wednesday);
                if (Thursday.JourneyTime.TotalHours > 0d)
                    dias.Add(DayOfWeek.Thursday);
                if (Friday.JourneyTime.TotalHours > 0d)
                    dias.Add(DayOfWeek.Friday);
                if (Saturday.JourneyTime.TotalHours > 0d)
                    dias.Add(DayOfWeek.Saturday);
                if (dias.Count == 0)
                {
                    dias.AddRange(new[] { (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 });
                }

                return dias.ToArray();
            }
        }

        /// <summary>
        /// Feriados, pontos facultativos e/ou datas especificas consideradas não relevantes
        /// </summary>
        /// <returns></returns>
        public List<DateTime> HoliDays { get; set; } = new List<DateTime>();

        /// <summary>
        /// Dias da semana não relevantes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayOfWeek> NonRelevantDaysOfWeek => new DayOfWeek[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday }.Where(x => x.IsNotIn(RelevantDaysOfWeek));


        public JourneyDay GetJourneyDay(DayOfWeek dayOfWeek) => dayOfWeek switch
        {
            DayOfWeek.Sunday => Sunday,
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            _ => null
        };

        public JourneyDay GetJourneyDay(DateTime Date) => GetJourneyDay(Date.DayOfWeek);


        /// <summary>
        /// Retorna a jornada de trabalho + hora de almoço de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan TotalTime(DateTime Date) => GetJourneyDay(Date).TotalTime;


        /// <summary>
        /// Retorna o tempo da jornada de trabalho de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan JourneyTime(DateTime Date) => GetJourneyDay(Date).JourneyTime;

        /// <summary>
        /// Retorna o tempo de almoço de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan LunchTime(DateTime Date) => GetJourneyDay(Date).LunchTime;

        /// <summary>
        /// Retorna a hora inicial da jornada de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime JourneyStartHour(DateTime Date) => GetJourneyDay(Date).StartHour;

        /// <summary>
        /// Retorna a hora final da jornada de uma data acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime JourneyEndHour(DateTime Date) => GetJourneyDay(Date).EndHour;

        /// <summary>
        /// Retorno a hora de inicio do almoço de uma data de acordo com as configurações desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime LunchStartHour(DateTime Date) => GetJourneyDay(Date).LunchHour;

        /// <summary>
        /// Retorna a hora de termino do almoço de uma data de acordo com as configurações desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime LunchEndHour(DateTime Date) => LunchStartHour(Date).Add(LunchTime(Date));

        /// <summary>
        /// Intervalo de horas trabalhadas entre as datas de inicio e fim desta demanda
        /// </summary>
        /// <returns></returns>
        public TimeSpan WorkTime => GetWorkTimeBetween(StartDate, EndDate);

        /// <summary>
        /// Retorna o intervalo de horas trabalhadas entre duas datas baseado nas configuracoes desta demanda
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetWorkTimeBetween(DateTime StartDate, DateTime EndDate)
        {
            Calendars.FixDateOrder(ref StartDate, ref EndDate);
            double total = 0;
            foreach (var dia in this.RelevantDays.Where(x => x.DayOfWeek.IsIn(RelevantDaysOfWeek)))
                total += GetJourneyDay(dia).JourneyTime.TotalMilliseconds;

            return new TimeSpan(0, 0, 0, 0, total.RoundInt());
        }
    }

    /// <summary>
    /// Dia de Uma Demanda
    /// </summary>
    public class JourneyDay
    {
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
        /// Jornada de Trabalho/Produção
        /// </summary>
        /// <returns></returns>
        public TimeSpan JourneyTime { get; set; } = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Hora de Almoço
        /// </summary>
        /// <returns></returns>
        public TimeSpan LunchTime { get; set; } = new TimeSpan(0, 0, 0);

        /// <summary>
        /// Jornada + hora de Almoço
        /// </summary>
        /// <returns></returns>
        public TimeSpan TotalTime => JourneyTime + LunchTime;

        /// <summary>
        /// Hora inicial da jornada
        /// </summary>
        /// <returns></returns>
        public DateTime StartHour
        {
            get => s;

            set => s = DateTime.MinValue.Add(new TimeSpan(value.TimeOfDay.Ticks));
        }

        private DateTime s;

        /// <summary>
        /// Hora de almoco
        /// </summary>
        /// <returns></returns>
        public DateTime LunchHour
        {
            get => a;

            set => a = DateTime.MinValue.Add(new TimeSpan(value.TimeOfDay.Ticks));
        }

        private DateTime a;

        /// <summary>
        /// Hora que se encerra a jornada (inclui hora de almoço)
        /// </summary>
        /// <returns></returns>
        public DateTime EndHour => StartHour.Add(TotalTime);

        /// <summary>
        /// Define a hora inicial e a jornada de trabalho deste dia
        /// </summary>
        /// <param name="StartHour"></param>
        /// <param name="Journey"></param>
        /// <param name="LunchTime">Horas de Almoço</param>
        public void SetJourney(DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default)
        {
            this.StartHour = StartHour;
            this.JourneyTime = Journey;
            this.LunchHour = LunchHour == default ? DateTime.MinValue.Date.AddHours(12d) : LunchHour;
            this.LunchTime = LunchTime == default ? new TimeSpan(0, 0, 0) : LunchTime;
        }
    }

    /// <summary>
    /// Item de Uma demanda
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Quantidade de itens
        /// </summary>
        /// <returns></returns>
        public int Quantity
        {
            get => q;

            set => q = value.SetMinValue(1);
        }

        private int q = 1;

        /// <summary>
        /// Tempo de produção de 1 item
        /// </summary>
        /// <returns></returns>
        public TimeSpan Time { get; set; } = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Tempo totald e produção de todos os itens
        /// </summary>
        /// <returns></returns>
        public TimeSpan ProductionTime
        {
            get
            {
                return new TimeSpan(Quantity * Time.Ticks);
            }
        }

        /// <summary>
        /// String que representa o item quando sua quantidade é 1
        /// </summary>
        /// <returns></returns>
        public string SingularItem { get; set; } = "Item";

        /// <summary>
        /// string que representa o item quando sua quantidade é maior que 1
        /// </summary>
        /// <returns></returns>
        public string MultipleItem { get; set; } = "Items";

        /// <summary>
        /// Retorna uma string que representa a quantidade do item
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Quantity + " " + (Quantity == 1 ? SingularItem : MultipleItem);
        }
    }




    public enum LongTimeSpanString
    {
        /// <summary>
        /// Retorna a string completa, incluindo os Zeros
        /// </summary>
        FullStringWithZero = 0,

        /// <summary>
        /// Retorna a string completa mas remove os zeros
        /// </summary>
        FullStringSkipZero = 1,

        /// <summary>
        /// Se o valor deste span for maior que 1 dia, descarta a parte de horas, minutos e segundos
        /// </summary>
        ReduceToDays = 2,

        /// <summary>
        /// Se o valor deste span for maior que 1 mes, descarta a parte de dias, horas, minutos e segundos
        /// </summary>
        ReduceToMonths = 3,

        /// <summary>
        /// Retorna somente o valor mais alto deste span
        /// </summary>
        ReduceMost = 4


    }
}