using System;
using System.Globalization;

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
}