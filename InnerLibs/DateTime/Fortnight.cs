using System;
using System.Globalization;

namespace InnerLibs.TimeMachine
{
    public class Fortnight : DateRange
    {
        private static string _format;


        public Fortnight() : this(DateTime.Now)
        {

        }

        /// <summary>
        /// Cria uma instancia de quinzena a partir de uma data que a mesma pode conter
        /// </summary>
        /// <param name="AnyDate">Qualquer data. Se NULL, a data atual é utilizada</param>
        public Fortnight(DateTime? AnyDate) : base(AnyDate.OrNow().FirstDayOfFortnight(), AnyDate.OrNow().LastDayOfFortnight())
        {
        }

        /// <summary>
        /// Define o formato global de uma string de uma <see cref="Fortnight"/>
        /// </summary>
        public static string Format { get => _format.IfBlank("{f}{o} - {mmmm}/{yyyy}"); set => _format = value; }

        /// <summary>
        /// String que identifica a quinzena em uma coleção
        /// </summary>
        /// <returns></returns>
        public string Key => FormatName("{f}@{mm}/{yyyy}");

        /// <summary>
        /// Numero da quinzena (1 ou 2)
        /// </summary>
        /// <returns></returns>
        public int Number => EndDate.Day <= 15 ? 1 : 2;



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
            Format = Format.IfBlank(TimeMachine.Fortnight.Format);

            int dia_inicio = StartDate.Day;
            int dia_fim = EndDate.Day;
            Format = Format.Replace("{s}", dia_inicio.ToString(Culture));
            Format = Format.Replace("{ss}", dia_inicio.ToString(Culture).PadLeft(2, '0'));

            Format = Format.ReplaceMany(dia_fim.ToString(Culture), "{e}", "{ee}");

            Format = Format.ReplaceMany(Number.ToString(Culture), "{f}", "{q}");
            Format = Format.ReplaceMany(Number.ToString(Culture).PadLeft(2, '0'), "{ff}", "{qq}");

            Format = Format.Replace("{m}", EndDate.Month.ToString(Culture));
            Format = Format.Replace("{mm}", EndDate.Month.ToString(Culture).PadLeft(2, '0').GetLastChars(2));
            Format = Format.Replace("{mmm}", EndDate.Month.ToShortMonthName(Culture));
            Format = Format.Replace("{mmmm}", EndDate.Month.ToLongMonthName(Culture));

            Format = Format.ReplaceMany(EndDate.Year.ToString(Culture), "{y}", "{a}");
            Format = Format.ReplaceMany(EndDate.Year.ToString(Culture).PadLeft(2, '0').GetLastChars(2), "{yy}", "{aa}");
            Format = Format.ReplaceMany(EndDate.Year.ToString(Culture).PadLeft(4, '0').GetLastChars(4), "{yyy}", "{aaa}", "{yyyy}", "{aaaa}");

            Format = Format.Replace("{o}", Number.GetOrdinal());
            Format = Format.ReplaceMany(Number.ToOrdinalNumber(), "{on}", "{oq}", "{of}");
            return Format;
        }

        public string FormatName(CultureInfo Culture) => FormatName(null, Culture);

        public override string ToString() => FormatName();

    }
}