using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using InnerLibs.TimeMachine;

namespace InnerLibs
{




    /// <summary>
    /// Modulo para manipulação de calendário
    /// </summary>
    /// <remarks></remarks>
    public static class Calendars
    {
        public static DateTime ClearMilliseconds(this DateTime Date)
        {
            return Date.AddTicks(-(Date.Ticks % TimeSpan.TicksPerSecond));
        }


        public static DateRange CreateDateRange<T>(this IEnumerable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateTime? StartDate = default, DateTime? EndDate = default) where T : class
        {
            var Period = new DateRange
            {
                ForceFirstAndLastMoments = true
            };

            StartDate ??= List.Min(PropertyExpression.Compile());
            EndDate ??= List.Max(PropertyExpression.Compile());

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
        /// Cria um <see cref="DateRange"/> a partir de 2 datas e/ou uma propriedade do tipo <see cref="DateTime" /> de <paramref name="T" /> como filtro de uma lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="PropertyExpression"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns>Um DateRange a partir das datas Iniciais e finais especificadas ou um daterange a partir da menor e maior datas de uma lista</returns>
        public static DateRange CreateDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime?>> PropertyExpression, DateTime? StartDate = default, DateTime? EndDate = default) where T : class
        {
            var Period = new DateRange
            {
                ForceFirstAndLastMoments = true
            };

            StartDate ??= List.Min(PropertyExpression);
            EndDate ??= List.Max(PropertyExpression);

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
        /// Pega o numero da semana do mês a partir de uma data
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static int GetWeekNumberOfMonth(this DateTime DateAndTime)
        {
            return DateAndTime.GetWeekInfoOfMonth().Week;
        }

        /// <summary>
        /// Pega o numero da semana, do mês e ano pertencente
        /// </summary>
        /// <param name="DateAndTime"></param>
        /// <returns></returns>
        public static WeekInfo GetWeekInfoOfMonth(this DateTime DateAndTime)
        {
            return new WeekInfo(DateAndTime);
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
        /// Retorna o primeiro dia de um ano especifico de outra data
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
            else if (Date.GetDoubleMonthOfYear() == 2)
                return new DateTime(Date.Year, 4, 1).GetLastDayOfMonth();
            else if (Date.GetDoubleMonthOfYear() == 3)
                return new DateTime(Date.Year, 6, 1).GetLastDayOfMonth();
            else if (Date.GetDoubleMonthOfYear() == 4)
                return new DateTime(Date.Year, 8, 1).GetLastDayOfMonth();
            else if (Date.GetDoubleMonthOfYear() == 5)
                return new DateTime(Date.Year, 10, 1).GetLastDayOfMonth();
            else
                return new DateTime(Date.Year, 12, 1).GetLastDayOfMonth();
        }

        /// <summary>
        /// Retorna o primeiro dia de um bimestre a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfDoubleMonth(this DateTime Date)
        {
            if (Date.GetDoubleMonthOfYear() == 1)
                return new DateTime(Date.Year, 1, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 2)
                return new DateTime(Date.Year, 3, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 3)
                return new DateTime(Date.Year, 5, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 4)
                return new DateTime(Date.Year, 7, 1).Date;
            else if (Date.GetDoubleMonthOfYear() == 5)
                return new DateTime(Date.Year, 9, 1).Date;
            else
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
        /// Retorna o ultimo momento do dia
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public static DateTime GetLastMoment(this DateTime Date) => Date.Date.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999);

        /// <summary>
        /// Verifica se uma data é do mesmo mês e ano que outra data
        /// </summary>
        /// <param name="[Date]">Primeira data</param>
        /// <param name="AnotherDate">Segunda data</param>
        /// <returns></returns>
        public static bool IsSameMonthAndYear(this DateTime Date, DateTime AnotherDate) => Date.IsBetween(AnotherDate.GetFirstDayOfMonth().Date, AnotherDate.GetLastDayOfMonth().GetLastMoment());

        /// <summary>
        /// Verifica se a Data de hoje é um aniversário
        /// </summary>
        /// <param name="BirthDate">  Data de nascimento</param>
        /// <returns></returns>
        public static bool IsAnniversary(this DateTime BirthDate, DateTime? CompareWith = default)
        {
            CompareWith ??= DateTime.Today;
            return (BirthDate.Day == CompareWith.Value.Day) && (BirthDate.Month == CompareWith.Value.Month);
        }

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetLongMonthName(this DateTime Date, CultureInfo Culture = null) => Date.ToString("MMMM", Culture ?? CultureInfo.CurrentCulture);

        /// <summary>
        /// Retorna o nome do mês a partir da data
        /// </summary>
        /// <param name="[Date]"></param>
        /// <param name="Culture"></param>
        /// <returns></returns>
        public static string GetShortMonthName(this DateTime Date, CultureInfo Culture = null) => Date.ToString("MM", Culture ?? CultureInfo.CurrentCulture);

        /// <summary>
        /// COnverte um datetime para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this DateTime Date) => $"{Date.Year}-{Date.Month}-{Date.Day} {Date.Hour}:{Date.Minute}:{Date.Second}.{Date.Millisecond}";


        /// <summary>
        /// Converte uma string dd/mm/aaaa hh:mm:ss.llll para o formato de string do SQL server ou Mysql
        /// </summary>
        /// <param name="[Date]">Data</param>
        /// <returns></returns>
        public static string ToSQLDateString(this string Date, string FromCulture = "pt-BR") => Convert.ToDateTime(Date, new CultureInfo(FromCulture, false).DateTimeFormat).ToSQLDateString();


        /// <summary>
        /// Retorna uma <see cref="LongTimeSpan"/> com a diferença entre 2 Datas
        /// </summary>
        /// <param name="InitialDate"></param>
        /// <param name="SecondDate"> </param>
        /// <returns></returns>
        public static LongTimeSpan GetDifference(this DateTime InitialDate, DateTime SecondDate) => new LongTimeSpan(InitialDate, SecondDate);

        /// <summary>
        /// Troca ou não a ordem das variaveis de inicio e fim de um periodo fazendo com que a StartDate
        /// sempre seja uma data menor que a EndDate, prevenindo que o calculo entre 2 datas resulte em um
        /// <see cref="TimeSpan"/> negativo
        /// </summary>
        /// <param name="StartDate">Data Inicial</param>
        /// <param name="EndDate">  Data Final</param>
        public static void FixDateOrder(ref DateTime StartDate, ref DateTime EndDate) => ClassTools.FixOrder(ref StartDate, ref EndDate);


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
                return StartDate.Date <= MidDate.Date && MidDate.Date <= EndDate.Date;
            }
            else
            {
                return StartDate <= MidDate && MidDate <= EndDate;
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
            DaysOfWeek ??= Array.Empty<DayOfWeek>();
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
        public static IEnumerable<DateTime> ClearTime(this IEnumerable<DateTime> List) => List.Select(x => x.Date);




        /// <summary>
        /// Retorna uma String baseado no numero do Mês Ex.: 1 -&gt; Janeiro
        /// </summary>
        /// <param name="MonthNumber">Numero do Mês</param>
        /// <returns>String com nome do Mês</returns>

        public static string ToLongMonthName(this int MonthNumber, CultureInfo Culture = null) => new DateTime(DateTime.Now.Year, MonthNumber, 1).GetLongMonthName(Culture);




        /// <summary>
        /// Retorna uma String curta baseado no numero do Mês Ex.: 1 -&gt; Jan
        /// </summary>
        /// <param name="MonthNumber">Numero do Mês</param>
        /// <returns>String com nome curto do Mês</returns>

        public static string ToShortMonthName(this int MonthNumber, CultureInfo Culture = null) => new DateTime(DateTime.Now.Year, MonthNumber, 1).GetShortMonthName(Culture);

        /// <summary>
        /// Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Segunda-Feira
        /// </summary>
        /// <param name="DayNumber">Numero do Dia</param>
        /// <returns>String com nome do Dia</returns>

        public static string ToLongDayOfWeekName(this int DayNumber) => DateTimeFormatInfo.CurrentInfo.GetDayName((DayOfWeek)DayNumber);

        /// <summary>
        /// Retorna uma String baseado no numero do Dia da Semana Ex.: 2 -&gt; Seg
        /// </summary>
        /// <param name="DayNumber">Numero do Dia</param>
        /// <returns>String com nome curto do Dia</returns>

        public static string ToShortDayOfWeekName(this int DayNumber) => DayNumber.ToLongDayOfWeekName().GetFirstChars(3);

        /// <summary>
        /// Retorna a data de amanhã
        /// </summary>
        /// <returns>Data de amanhã</returns>

        public static DateTime Tomorrow => DateTime.Now.AddDays(1d);

        /// <summary>
        /// Retorna a data de ontem
        /// </summary>
        /// <returns>Data de ontem</returns>

        public static DateTime Yesterday => DateTime.Now.AddDays(-1);

        public static DateTime BrazilianNow => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));

        public static DateTime BrazilianTomorrow => BrazilianNow.AddDays(1d);

        public static DateTime BrazilianYesterday => BrazilianNow.AddDays(-1);

        /// <summary>
        /// Retorna o ultimo domingo
        /// </summary>
        /// <returns></returns>
        public static DateTime LastSunday(DateTime? FromDate = null) => LastDay(DayOfWeek.Sunday, FromDate);

        /// <summary>
        /// Retorna o proximo domingo
        /// </summary>
        /// <returns></returns>
        public static DateTime NextSunday(DateTime? FromDate = default) => NextDay(DayOfWeek.Sunday, FromDate);

        /// <summary>
        /// Retorna o ultimo dia referente a um dia da semana
        /// </summary>
        /// <param name="DayOfWeek"></param>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public static DateTime LastDay(DayOfWeek DayOfWeek, DateTime? FromDate = default)
        {
            FromDate ??= DateTime.Now;
            while (FromDate.Value.DayOfWeek != DayOfWeek)
                FromDate = FromDate.Value.AddDays(-1);
            return (DateTime)FromDate;
        }

        /// <summary>
        /// Retorna o proximo dia referente a um dia da semana
        /// </summary>
        /// <param name="DayOfWeek"></param>
        /// <param name="FromDate"></param>
        /// <returns></returns>
        public static DateTime NextDay(DayOfWeek DayOfWeek, DateTime? FromDate = default)
        {
            FromDate ??= DateTime.Now;
            while (FromDate.Value.DayOfWeek != DayOfWeek)
                FromDate = FromDate.Value.AddDays(1d);
            return (DateTime)FromDate;
        }

        /// <summary>
        /// Verifica se o dia se encontra no fim de semana
        /// </summary>
        /// <param name="YourDate">Uma data qualquer</param>
        /// <returns>TRUE se for sabado ou domingo, caso contrario FALSE</returns>

        public static bool IsWeekend(this DateTime YourDate) => YourDate.DayOfWeek == DayOfWeek.Sunday | YourDate.DayOfWeek == DayOfWeek.Saturday;

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
        public static string ToFarewell(this DateTime Time, string Language = "pt") => Time.ToGreetingFarewell(Language, true);

        /// <summary>
        /// Transforma um DateTime em uma saudação (Bom dia, Boa tarde, Boa noite)
        /// </summary>
        /// <param name="Time">    Horario</param>
        /// <param name="Language">Idioma da saudação (pt, en, es)</param>
        /// <returns>Uma string com a despedida</returns>
        public static string ToGreeting(this DateTime Time, string Language = "pt") => Time.ToGreetingFarewell(Language, false);



        /// <summary>
        /// Returna uma lista dupla com os meses
        /// </summary>
        /// <param name="ValueType">Apresentação dos meses no valor</param>
        /// <param name="TextType">Apresentação dos meses no texto</param>

        public static List<KeyValuePair<string, string>> GetMonthList(CalendarFormat TextType = CalendarFormat.LongName, CalendarFormat ValueType = CalendarFormat.Number)
        {
            List<KeyValuePair<string, string>> MonthsRet = new List<KeyValuePair<string, string>>();
            for (int i = 1; i <= 12; i++)
            {
                string key = "";
                string value = "";
                switch (TextType)
                {
                    case CalendarFormat.LongName:
                        {
                            key = i.ToLongMonthName();
                            break;
                        }

                    case CalendarFormat.ShortName:
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
                    case CalendarFormat.LongName:
                        {
                            value = i.ToLongMonthName();
                            break;
                        }

                    case CalendarFormat.ShortName:
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

        public static List<KeyValuePair<string, string>> GetWeekDays(CalendarFormat TextType = CalendarFormat.LongName, CalendarFormat ValueType = CalendarFormat.Number)
        {
            List<KeyValuePair<string, string>> WeekDaysRet = default;
            WeekDaysRet = new List<KeyValuePair<string, string>>();
            for (int i = 1; i <= 7; i++)
            {
                string key = "";
                string value = "";
                switch (TextType)
                {
                    case CalendarFormat.LongName:
                        {
                            key = i.ToLongDayOfWeekName();
                            break;
                        }

                    case CalendarFormat.ShortName:
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
                    case CalendarFormat.LongName:
                        {
                            value = i.ToLongDayOfWeekName();
                            break;
                        }

                    case CalendarFormat.ShortName:
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

        public enum CalendarFormat
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
        public static decimal CalculateTimelinePercent(this DateTime MidDate, DateTime StartDate, DateTime EndDate)
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