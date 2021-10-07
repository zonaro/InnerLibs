using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public new IEnumerable<DataType> this[Fortnight Fort]
        {
            get
            {
                return this[Fort.Key];
            }
        }

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
        /// Retorna um <see cref="Dictionary(Of String, DataType)"/> com as informaçoes agrupadas por quinzena
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
            if (Data is object && Data.Count() > 0)
            {
                fort.DataCollection = Data.ToList();
            }

            if (DateSelector is object && DateSelector.Count() > 0)
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
            if (DateSelector is null || DateSelector.Count() == 0)
            {
                throw new ArgumentNullException("DateSelector is Nothing or Empty");
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
                fortcount = fortcount + 1;
                fort = new FortnightGroup<DataType>(StartDate, fortcount);
            }

            return fort;
        }

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
                EndDate = EndDate.Value.GetLastDayOfMonth();
            }

            Period = new DateRange((DateTime)AnyDate, (DateTime)EndDate);
        }

        /// <summary>
        /// String que identifica a quinzena em uma coleção
        /// </summary>
        /// <returns></returns>
        public string Key
        {
            get
            {
                if (Period.EndDate.Day <= 15)
                {
                    return Period.EndDate.ToString(@"\1@MM-yyyy");
                }
                else
                {
                    return Period.EndDate.ToString(@"\2@MM-yyyy");
                }
            }
        }

        /// <summary>
        /// Periodo que esta quinzena possui
        /// </summary>
        /// <returns></returns>
        public DateRange Period { get; private set; }

        /// <summary>
        /// Numero da quinzena (1 ou 2)
        /// </summary>
        /// <returns></returns>
        public int Number
        {
            get
            {
                return Key.GetFirstChars(1).ChangeType<int, string>();
            }
        }

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
        public string FormatName(string Format = "{q}{o} - {mmmm}/{yyyy}")
        {
            int quinzena = Conversions.ToInteger(Key.Split("@")[0]);
            int mes = Conversions.ToInteger(Key.Split("@")[1].Split("-")[0]);
            int ano = Conversions.ToInteger(Key.Split("@")[1].Split("-")[1]);
            int dia_inicio = Period.StartDate.Day;
            int dia_fim = Period.EndDate.Day;
            Format = Format.Replace("{s}", dia_inicio.ToString("#"));
            Format = Format.Replace("{ss}", dia_inicio.ToString("##"));
            Format = Format.Replace("{e}", dia_inicio.ToString("#"));
            Format = Format.Replace("{ee}", dia_inicio.ToString("##"));
            Format = Format.Replace("{f}", quinzena.ToString("#"));
            Format = Format.Replace("{ff}", quinzena.ToString("##"));
            Format = Format.Replace("{q}", quinzena.ToString("#"));
            Format = Format.Replace("{qq}", quinzena.ToString("##"));
            Format = Format.Replace("{m}", mes.ToString("#"));
            Format = Format.Replace("{mm}", mes.ToString("##"));
            Format = Format.Replace("{mmm}", mes.ToShortMonthName());
            Format = Format.Replace("{mmmm}", mes.ToLongMonthName());
            Format = Format.Replace("{y}", ano.ToString().GetLastChars(2));
            Format = Format.Replace("{yy}", ano.ToString().GetLastChars(2));
            Format = Format.Replace("{yyy}", ano.ToString());
            Format = Format.Replace("{yyyy}", ano.ToString());
            Format = Format.Replace("{a}", ano.ToString().GetLastChars(2));
            Format = Format.Replace("{aa}", ano.ToString().GetLastChars(2));
            Format = Format.Replace("{aaa}", ano.ToString());
            Format = Format.Replace("{aaaa}", ano.ToString());
            Format = Format.Replace("{o}", quinzena.ToOrdinalNumber(true));
            return Format;
        }

        public override string ToString()
        {
            return FormatName();
        }
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
        public Fortnight this[string Key]
        {
            get
            {
                return base[IndexOf(this.Where(x => (x.Key ?? "") == (Key ?? "")).SingleOrDefault())];
            }
        }

        /// <summary>
        /// Retorna uma quinzena a partir da sua Index
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public new Fortnight this[int Index]
        {
            get
            {
                return base[Index];
            }
        }

        /// <summary>
        /// Retorna a data inicial do periodo
        /// </summary>
        /// <returns></returns>
        public DateTime StartDate
        {
            get
            {
                return this.First().Period.StartDate;
            }
        }

        /// <summary>
        /// Retorna a ultima data do periodo
        /// </summary>
        /// <returns></returns>
        public DateTime EndDate
        {
            get
            {
                return this.Last().Period.EndDate;
            }
        }

        /// <summary>
        /// Retorna uma lista com todos os dias entre as quinzenas
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> AllDays
        {
            get
            {
                return this.SelectMany(x => x.Period.StartDate.GetDaysBetween(x.Period.EndDate));
            }
        }

        /// <summary>
        /// Retorna um periodo equivalente a este grupo de quinzena
        /// </summary>
        /// <returns></returns>
        public DateRange Period
        {
            get
            {
                return new DateRange(StartDate, EndDate);
            }
        }

        /// <summary>
        /// Instancia um novo <see cref="FortnightGroup"/> a partir de uma data e um numero de quinzenas
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="FortnightCount"></param>
        private static List<Fortnight> GerarLista(DateTime StartDate = default, int FortnightCount = 1)
        {
            var l = new List<Fortnight>();
            l.Add(new Fortnight(StartDate));
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
    /// Classe para comapração entre 2 Datas com possibilidade de validação de dias Relevantes
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
            if (RelevantDaysOfWeek.Count() == 0)
            {
                RelevantDaysOfWeek = new[] { (DayOfWeek)0, (DayOfWeek)1, (DayOfWeek)2, (DayOfWeek)3, (DayOfWeek)4, (DayOfWeek)5, (DayOfWeek)6 };
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
                catch (Exception ex)
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
                                years = years + 1;
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
                                months = months + 1;
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
                                days = days + 1;
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

        public decimal TotalMilliseconds
        {
            get
            {
                return (decimal)(EndDate - StartDate).TotalMilliseconds;
            }
        }

        public decimal TotalSeconds
        {
            get
            {
                return (decimal)(EndDate - StartDate).TotalSeconds;
            }
        }

        public decimal TotalMinutes
        {
            get
            {
                return (decimal)(EndDate - StartDate).TotalMinutes;
            }
        }

        public decimal TotalHours
        {
            get
            {
                return (decimal)(EndDate - StartDate).TotalHours;
            }
        }

        public decimal TotalDays
        {
            get
            {
                return (decimal)(EndDate - StartDate).TotalDays;
            }
        }

        public decimal TotalMonths
        {
            get
            {
                return (decimal)Math.Round((EndDate - StartDate).TotalDays / (365.25d / 12d), 2);
            }
        }

        public decimal TotalYears
        {
            get
            {
                return Math.Round(TotalMonths / 12m, 2);
            }
        }

        public decimal TotalWeeks
        {
            get
            {
                return Math.Round(TotalDays / 7m, 2);
            }
        }

        /// <summary>
        /// Todos os dias entre as datas Inicial e Final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> AllDays
        {
            get
            {
                return RelevantDays.Union(NonRelevantDays).OrderBy(x => x).AsEnumerable();
            }
        }

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
        public string ToTimeElapsedString(bool FullString = true)
        {
            string ano = "";
            string mes = "";
            string dia = "";
            string horas = "";
            string minutos = "";
            string segundos = "";
            ano = Years > 0 ? Years == 1 ? Years + " ano " : Years + " anos" : "";
            mes = Months > 0 ? Months == 1 ? Months + " mês " : Months + " meses " : "";
            dia = Days > 0 ? Days == 1 ? Days + " dia " : Days + " dias " : "";
            if (FullString || new[] { ano, mes, dia }.All(x => x.IsBlank()))
            {
                horas = Hours > 0 ? Hours == 1 ? Hours + " hora " : Hours + " horas " : "";
                minutos = Minutes > 0 ? Minutes == 1 ? Minutes + " minuto " : Minutes + " minutos " : "";
                segundos = Seconds > 0 ? Seconds == 1 ? Seconds + " segundo " : Seconds + " segundos " : "";
            }

            ano = ano.AppendIf(",", ano.IsNotBlank() & (mes.IsNotBlank() | dia.IsNotBlank() | horas.IsNotBlank() | minutos.IsNotBlank() | segundos.IsNotBlank()));
            mes = mes.AppendIf(",", mes.IsNotBlank() & (dia.IsNotBlank() | horas.IsNotBlank() | minutos.IsNotBlank() | segundos.IsNotBlank()));
            dia = dia.AppendIf(",", dia.IsNotBlank() & (horas.IsNotBlank() | minutos.IsNotBlank() | segundos.IsNotBlank()));
            horas = horas.AppendIf(",", horas.IsNotBlank() & (minutos.IsNotBlank() | segundos.IsNotBlank()));
            minutos = minutos.AppendIf(",", minutos.IsNotBlank() & segundos.IsNotBlank());

            string current = new[] { ano, mes, dia, horas, minutos, segundos }.Join(" ");
            if (current.Contains(","))
            {
                current = current.ReplaceLast(",", " e ");
            }

            return current.AdjustWhiteSpaces().TrimAny(true, ",", " ", "e");
        }

        /// <summary>
        /// Retorna uma string com a quantidade de itens e o tempo de produção
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToTimeElapsedString(true);
        }

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
        public Day Sunday { get; set; } = new Day();

        /// <summary>
        /// Segunda-Feira
        /// </summary>
        /// <returns></returns>
        public Day Monday { get; set; } = new Day();

        /// <summary>
        /// Terça-Feira
        /// </summary>
        /// <returns></returns>
        public Day Tuesday { get; set; } = new Day();

        /// <summary>
        /// Quarta-Feira
        /// </summary>
        /// <returns></returns>
        public Day Wednesday { get; set; } = new Day();

        /// <summary>
        /// Quinta-Feira
        /// </summary>
        /// <returns></returns>
        public Day Thursday { get; set; } = new Day();

        /// <summary>
        /// Sexta-Feira
        /// </summary>
        /// <returns></returns>
        public Day Friday { get; set; } = new Day();

        /// <summary>
        /// Sábado
        /// </summary>
        /// <returns></returns>
        public Day Saturday { get; set; } = new Day();

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
        public TimeDemand(DateTime StartDate, TimeSpan Time, int Quantity = 1, string SingularItem = "Item", string MultipleItem = "Items")
        {
            this.StartDate = StartDate;
            Item.Quantity = Quantity;
            Item.Time = Time;
            Item.SingularItem = SingularItem;
            Item.MultipleItem = MultipleItem;
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
        public IEnumerable<DateTime> get_WorkDays(params DayOfWeek[] DaysOfWeek)
        {
            return StartDate.GetDaysBetween(EndDate, DaysOfWeek.ToArray());
        }

        /// <summary>
        /// Dias relevantes (letivos) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> RelevantDays
        {
            get
            {
                var dias = get_WorkDays(RelevantDaysOfWeek.ToArray()).ClearTime().ToList();
                foreach (var feriado in HoliDays.ClearTime())
                {
                    if (dias.Contains(feriado))
                    {
                        dias.Remove(feriado);
                    }
                }

                return dias;
            }
        }

        /// <summary>
        /// Dias não relevantes (nao letivos e feriados) entre as datas inicial e final
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DateTime> NonRelevantDays
        {
            get
            {
                var dias = get_WorkDays().ClearTime().ToList();
                foreach (var d in RelevantDays)
                    dias.Remove(d);
                return dias;
            }
        }

        /// <summary>
        /// Retorna um TimeFlow desta demanda
        /// </summary>
        /// <returns></returns>
        public LongTimeSpan BuildTimeFlow()
        {
            return new LongTimeSpan(StartDate, EndDate, RelevantDaysOfWeek.ToArray());
        }

        /// <summary>
        /// Retorna uma string representado a quantidade de itens e o tempo gasto com a produção
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Item.ToString() + " - " + ToTimeElapsedString();
        }

        /// <summary>
        /// Retorna uma String no formato "X anos, Y meses e Z dias"
        /// </summary>
        /// <param name="FullString">Parametro que indica se as horas, minutos e segundos devem ser apresentados caso o tempo seja maior que 1 dia</param>
        /// <returns></returns>
        public string ToTimeElapsedString(bool FullString = true)
        {
            var data_final = EndDate;
            var data_inicial = StartDate;
            return data_inicial.GetDifference(data_final).ToTimeElapsedString(FullString);
        }

        /// <summary>
        /// inicia uma nova demanda
        /// </summary>
        public TimeDemand()
        {
            StartDate = DateTime.Now;
        }

        /// <summary>
        /// Retorna a porcentagem em relacao a posição de uma data entre a data inicial (0%) e final (100%)
        /// </summary>
        /// <param name="MidDate"></param>
        /// <returns></returns>
        public decimal GetPercentCompletion(DateTime MidDate)
        {
            return MidDate.CalculatePercent(StartDate, EndDate);
        }

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
        /// Feriados, pontos facuultativos e/ou datas especificas consideradas não relevantes
        /// </summary>
        /// <returns></returns>
        public List<DateTime> HoliDays { get; set; } = new List<DateTime>();

        /// <summary>
        /// Dias da semana não relevantes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DayOfWeek> NonRelevantDaysOfWeek
        {
            get
            {
                var s = new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday }.ToList();
                foreach (var d in RelevantDaysOfWeek)
                    s.Remove(d);
                return s;
            }
        }

        /// <summary>
        /// Retorna a jornada de trabalho + hora de almoço de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan TotalTime(DateTime Date)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        return Sunday.TotalTime;
                    }

                case DayOfWeek.Monday:
                    {
                        return Monday.TotalTime;
                    }

                case DayOfWeek.Tuesday:
                    {
                        return Tuesday.TotalTime;
                    }

                case DayOfWeek.Wednesday:
                    {
                        return Wednesday.TotalTime;
                    }

                case DayOfWeek.Thursday:
                    {
                        return Thursday.TotalTime;
                    }

                case DayOfWeek.Friday:
                    {
                        return Friday.TotalTime;
                    }

                case DayOfWeek.Saturday:
                    {
                        return Saturday.TotalTime;
                    }
            }

            return default;
        }

        /// <summary>
        /// Retorna o tempo da jornada de trabalho de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan JourneyTime(DateTime Date)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        return Sunday.JourneyTime;
                    }

                case DayOfWeek.Monday:
                    {
                        return Monday.JourneyTime;
                    }

                case DayOfWeek.Tuesday:
                    {
                        return Tuesday.JourneyTime;
                    }

                case DayOfWeek.Wednesday:
                    {
                        return Wednesday.JourneyTime;
                    }

                case DayOfWeek.Thursday:
                    {
                        return Thursday.JourneyTime;
                    }

                case DayOfWeek.Friday:
                    {
                        return Friday.JourneyTime;
                    }

                case DayOfWeek.Saturday:
                    {
                        return Saturday.JourneyTime;
                    }
            }

            return default;
        }

        /// <summary>
        /// Retorna o tempo de almoço de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public TimeSpan LunchTime(DateTime Date)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        return Sunday.LunchTime;
                    }

                case DayOfWeek.Monday:
                    {
                        return Monday.LunchTime;
                    }

                case DayOfWeek.Tuesday:
                    {
                        return Tuesday.LunchTime;
                    }

                case DayOfWeek.Wednesday:
                    {
                        return Wednesday.LunchTime;
                    }

                case DayOfWeek.Thursday:
                    {
                        return Thursday.LunchTime;
                    }

                case DayOfWeek.Friday:
                    {
                        return Friday.LunchTime;
                    }

                case DayOfWeek.Saturday:
                    {
                        return Saturday.LunchTime;
                    }
            }

            return default;
        }

        /// <summary>
        /// Retorna a hora inicial da jornada de uma data de acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime JourneyStartHour(DateTime Date)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        return Sunday.StartHour;
                    }

                case DayOfWeek.Monday:
                    {
                        return Monday.StartHour;
                    }

                case DayOfWeek.Tuesday:
                    {
                        return Tuesday.StartHour;
                    }

                case DayOfWeek.Wednesday:
                    {
                        return Wednesday.StartHour;
                    }

                case DayOfWeek.Thursday:
                    {
                        return Thursday.StartHour;
                    }

                case DayOfWeek.Friday:
                    {
                        return Friday.StartHour;
                    }

                case DayOfWeek.Saturday:
                    {
                        return Saturday.StartHour;
                    }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Retorna a hora final da jornada de uma data acordo com as configuracoes desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime JourneyEndHour(DateTime Date)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        return Sunday.EndHour;
                    }

                case DayOfWeek.Monday:
                    {
                        return Monday.EndHour;
                    }

                case DayOfWeek.Tuesday:
                    {
                        return Tuesday.EndHour;
                    }

                case DayOfWeek.Wednesday:
                    {
                        return Wednesday.EndHour;
                    }

                case DayOfWeek.Thursday:
                    {
                        return Thursday.EndHour;
                    }

                case DayOfWeek.Friday:
                    {
                        return Friday.EndHour;
                    }

                case DayOfWeek.Saturday:
                    {
                        return Saturday.EndHour;
                    }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Retorno a hora de inicio do almoço de uma data de acordo com as configurações desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime LunchStartHour(DateTime Date)
        {
            switch (Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    {
                        return Sunday.LunchHour;
                    }

                case DayOfWeek.Monday:
                    {
                        return Monday.LunchHour;
                    }

                case DayOfWeek.Tuesday:
                    {
                        return Tuesday.LunchHour;
                    }

                case DayOfWeek.Wednesday:
                    {
                        return Wednesday.LunchHour;
                    }

                case DayOfWeek.Thursday:
                    {
                        return Thursday.LunchHour;
                    }

                case DayOfWeek.Friday:
                    {
                        return Friday.LunchHour;
                    }

                case DayOfWeek.Saturday:
                    {
                        return Saturday.LunchHour;
                    }
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// Retorna a hora de termino do almoço de uma data de acordo com as configurações desta demanda
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public DateTime LunchEndHour(DateTime Date)
        {
            return LunchStartHour(Date).Add(LunchTime(Date));
        }

        /// <summary>
        /// Intervalo de horas trabalhadas entre as datas de inicio e fim desta demanda
        /// </summary>
        /// <returns></returns>
        public TimeSpan WorkTime
        {
            get
            {
                return GetWorkTimeBetween(StartDate, EndDate);
            }
        }

        /// <summary>
        /// Retorna o intervalo de horas trabalhadas entre duas datas baseado nas confuguracoes desta demanda
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetWorkTimeBetween(DateTime StartDate, DateTime EndDate)
        {
            Calendars.FixDateOrder(ref StartDate, ref EndDate);
            var dias = RelevantDaysOfWeek;
            int totalhoras = 0;
            var cal = new LongTimeSpan(StartDate, EndDate, dias.ToArray());
            foreach (var dia in cal.RelevantDays)
            {
                switch (dia.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        {
                            totalhoras = (int)Math.Round(totalhoras + Sunday.JourneyTime.TotalHours);
                            break;
                        }

                    case DayOfWeek.Monday:
                        {
                            totalhoras = (int)Math.Round(totalhoras + Monday.JourneyTime.TotalHours);
                            break;
                        }

                    case DayOfWeek.Tuesday:
                        {
                            totalhoras = (int)Math.Round(totalhoras + Tuesday.JourneyTime.TotalHours);
                            break;
                        }

                    case DayOfWeek.Wednesday:
                        {
                            totalhoras = (int)Math.Round(totalhoras + Wednesday.JourneyTime.TotalHours);
                            break;
                        }

                    case DayOfWeek.Thursday:
                        {
                            totalhoras = (int)Math.Round(totalhoras + Thursday.JourneyTime.TotalHours);
                            break;
                        }

                    case DayOfWeek.Friday:
                        {
                            totalhoras = (int)Math.Round(totalhoras + Friday.JourneyTime.TotalHours);
                            break;
                        }

                    case DayOfWeek.Saturday:
                        {
                            totalhoras = (int)Math.Round(totalhoras + Saturday.JourneyTime.TotalHours);
                            break;
                        }
                }
            }

            return new TimeSpan(totalhoras, 0, 0);
        }
    }

    /// <summary>
    /// Dia de Uma Demanda
    /// </summary>
    public class Day
    {

        /// <summary>
        /// Inicia uma instancia de dia letivo
        /// </summary>
        public Day()
        {
            StartHour = DateTime.MinValue;
            LunchHour = DateTime.MinValue.AddHours(12d);
        }

        /// <summary>
        /// Inicia uma instancia de dia letivo
        /// </summary>
        /// <param name="StartHour">Hora Incial</param>
        /// <param name="Journey">Jornada de trabalho</param>
        public Day(DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default)
        {
            SetJourney(StartHour, Journey, LunchHour, LunchTime);
        }

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
        public TimeSpan TotalTime
        {
            get
            {
                return JourneyTime + LunchTime;
            }
        }

        /// <summary>
        /// Hora inicial da jornada
        /// </summary>
        /// <returns></returns>
        public DateTime StartHour
        {
            get
            {
                return s;
            }

            set
            {
                s = DateTime.MinValue.Add(new TimeSpan(value.TimeOfDay.Ticks));
            }
        }

        private DateTime s;

        /// <summary>
        /// Hora de almoco
        /// </summary>
        /// <returns></returns>
        public DateTime LunchHour
        {
            get
            {
                return a;
            }

            set
            {
                a = DateTime.MinValue.Add(new TimeSpan(value.TimeOfDay.Ticks));
            }
        }

        private DateTime a;

        /// <summary>
        /// Hora que se encerra a jornada (inclui hora de almoço)
        /// </summary>
        /// <returns></returns>
        public DateTime EndHour
        {
            get
            {
                return StartHour.Add(TotalTime);
            }
        }

        /// <summary>
        /// Define a hora inicial e a jornada de trabalho deste dia
        /// </summary>
        /// <param name="StartHour"></param>
        /// <param name="Journey"></param>
        /// <param name="LunchTime">Horas de Almoço</param>
        public void SetJourney(DateTime StartHour, TimeSpan Journey, DateTime LunchHour = default, TimeSpan LunchTime = default)
        {
            this.StartHour = StartHour;
            JourneyTime = Journey;
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
            get
            {
                return q;
            }

            set
            {
                q = value.SetMinValue(1);
            }
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
        public string MultipleItem { get; set; } = null;

        /// <summary>
        /// Retorna uma string que representa a quantidade do item
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Quantity + " " + (Quantity == 1 ? SingularItem : MultipleItem);
        }
    }
}