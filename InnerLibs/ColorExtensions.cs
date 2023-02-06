using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace InnerLibs
{
    /// <summary>
    /// Modulo de Conversão de Cores
    /// </summary>
    /// <remarks></remarks>
    public static class ColorExtensions
    {
        #region Public Properties

        /// <summary>
        /// Retorna uma lista com todas as <see cref="KnowColor"/> convertidas em <see cref="System.Drawing.Color"/>
        /// </summary>
        public static IEnumerable<Color> KnowColors => Ext.GetEnumValues<KnownColor>().Select(x => Color.FromKnownColor(x));

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Procura uma cor na tabela de cores <see cref="HSVColor.NamedColors"/>
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static HSVColor FindColor(this string Text) => HSVColor.NamedColors
               .FirstOrDefault(x => x.Name.ToLowerInvariant().Replace("grey", "gray").RemoveAny(PredefinedArrays.PasswordSpecialChars.Union(new[] { " " }).ToArray()) == Text.ToLowerInvariant().Replace("grey", "gray").RemoveAny(PredefinedArrays.PasswordSpecialChars.Union(new[] { " " }).ToArray()));

        /// <summary>
        /// Retorna o nome comum mais proximo a esta cor
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static string GetClosestColorName(this Color Color) => Color.GetClosestKnowColor().Name;

        /// <summary>
        /// Retorna uma cor conhecida mais proxima de outra cor
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static Color GetClosestKnowColor(this Color Color)
        {
            double closest_distance = double.MaxValue;
            var closest = Color.White;
            foreach (var kc in KnowColors)
            {
                // Calculate Euclidean Distance
                double d = new HSVColor(kc).GetEuclideanDistance(Color);
                if (d < closest_distance)
                {
                    closest_distance = d;
                    closest = kc;
                }
            }

            return closest;
        }

        /// <summary>
        /// Retorna o nome da cor
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static string GetColorName(this Color Color)
        {
            foreach (var namedColor in HSVColor.NamedColors) if (namedColor.ARGB == Color.ToArgb()) return namedColor.Name;
            return Color.Name;
        }

        /// <summary>
        /// Retorna uma cor de contraste baseado na iluminacao da primeira cor: Uma cor clara se a
        /// primeira for escura. Uma cor escura se a primeira for clara
        /// </summary>
        /// <param name="TheColor">Primeira cor</param>
        /// <param name="Percent">Grau de mesclagem da cor escura ou clara</param>
        /// <returns>
        /// Uma cor clara se a primeira cor for escura, uma cor escura se a primeira for clara
        /// </returns>
        public static Color GetContrastColor(this Color TheColor, float Percent = 70f)
        {
            double a = 1d - (0.299d * TheColor.R + 0.587d * TheColor.G + 0.114d * TheColor.B) / 255d;
            int d = a < 0.5d ? 0 : 255;
            return TheColor.MergeWith(Color.FromArgb(d, d, d), Percent);
        }

        /// <summary>
        /// Retorna a cor negativa de uma cor
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <returns></returns>
        public static Color GetNegativeColor(this Color TheColor) => Color.FromArgb(255 - TheColor.R, 255 - TheColor.G, 255 - TheColor.B);

        public static IEnumerable<HSVColor> GrayscalePallette(int Amount) => MonochromaticPallette(Color.White, Amount);

        /// <summary>
        /// Verifica se uma cor é escura
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <returns></returns>
        public static bool IsDark(this Color TheColor) => new HSVColor(TheColor).IsDark();

        public static bool IsHexaDecimalColor(this string Text)
        {
            Text = Text.TrimFirstEqual("#");
            var myRegex = new Regex("^[a-fA-F0-9]+$");
            return Text.IsNotBlank() && myRegex.IsMatch(Text);
        }

        /// <summary>
        /// Verifica se uma cor é clara
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <returns></returns>
        public static bool IsLight(this Color TheColor) => !TheColor.IsDark();

        /// <summary>
        /// Verifica se uma cor é legivel sobre outra
        /// </summary>
        /// <param name="Color"></param>
        /// <param name="BackgroundColor"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public static bool IsReadable(this Color Color, Color BackgroundColor, int Size = 10)
        {
            if (Color.A == 0)
                return false;
            if (BackgroundColor.A == 0)
                return true;
            double diff = BackgroundColor.R * 0.299d + BackgroundColor.G * 0.587d + BackgroundColor.B * 0.114d - Color.R * 0.299d - Color.G * 0.587d - Color.B * 0.114d;
            return !(diff < 1.5d + 141.162d * Math.Pow(0.975d, Size)) && diff > -0.5d - 154.709d * Math.Pow(0.99d, Size);
        }

        /// <summary>
        /// Mescla duas cores usando <see cref="Lerp"/>
        /// </summary>
        /// <param name="FromColor">Cor</param>
        /// <param name="ToColor">Outra cor</param>
        /// <param name="amount">Indice de mesclagem</param>
        /// <returns></returns>
        public static Color Lerp(this Color FromColor, Color ToColor, float Amount)
        {
            // start colours as lerp-able floats
            float sr = FromColor.R;
            float sg = FromColor.G;
            float sb = FromColor.B;
            // end colours as lerp-able floats
            float er = ToColor.R;
            float eg = ToColor.G;
            float eb = ToColor.B;
            // lerp the colours to get the difference
            byte r = (byte)Math.Round(sr.Lerp(er, Amount));
            byte g = (byte)Math.Round(sg.Lerp(eg, Amount));
            byte b = (byte)Math.Round(sb.Lerp(eb, Amount));
            // return the new colour
            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Escurece a cor mesclando ela com preto
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <param name="percent">porcentagem de mesclagem</param>
        /// <returns></returns>
        public static Color MakeDarker(this Color TheColor, float Percent = 50f) => TheColor.MergeWith(Color.Black, Percent);

        /// <summary>
        /// Clareia a cor misturando ela com branco
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <param name="percent">Porcentagem de mesclagem</param>
        /// <returns></returns>
        public static Color MakeLighter(this Color TheColor, float Percent = 50f) => TheColor.MergeWith(Color.White, Percent);

        /// <summary>
        /// Mescla duas cores a partir de uma porcentagem
        /// </summary>
        /// <param name="TheColor">Cor principal</param>
        /// <param name="AnotherColor">Cor de mesclagem</param>
        /// <param name="percent">Porcentagem de mescla</param>
        /// <returns></returns>
        public static Color MergeWith(this Color TheColor, Color AnotherColor, float Percent = 50f) => TheColor.Lerp(AnotherColor, Percent / 100f);

        /// <summary>
        /// Gera uma paleta de cores monocromatica com <paramref name="Amount"/> amostras a partir
        /// de uma <paramref name="Color"/> base.
        /// </summary>
        /// <param name="Color"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        /// <remarks>A distancia entre as cores será maior se a quantidade de amostras for pequena</remarks>
        public static IEnumerable<HSVColor> MonochromaticPallette(Color Color, int Amount)
        {
            var t = new RuleOfThree<int>(Amount, 100, 1, default);
            var Percent = t.UnknownValue?.ToSingle();
            Color = Color.White.MergeWith(Color);
            var l = new List<Color>();
            for (int index = 1, loopTo = Amount; index <= loopTo; index++)
            {
                Color = Color.MakeDarker((float)Percent);
                l.Add(Color);
            }

            return l.ToHSVColorList();
        }

        /// <summary>
        /// Gera uma cor aleatória misturando ou não os canais RGB
        /// </summary>
        /// <param name="Red">-1 para Random ou de 0 a 255 para especificar o valor</param>
        /// <param name="Green">-1 para Random ou de 0 a 255 para especificar o valor</param>
        /// <param name="Blue">-1 para Random ou de 0 a 255 para especificar o valor</param>
        /// <returns></returns>
        public static Color RandomColor(int Red = -1, int Green = -1, int Blue = -1, int Alpha = 255)
        {
            Red = Red.SetMinValue(-1);
            Green = Green.SetMinValue(-1);
            Blue = Blue.SetMinValue(-1);

            Red = (Red < 0 ? Ext.RandomNumber(0, 255) : Red).LimitRange<int>(0, 255);
            Green = (Green < 0 ? Ext.RandomNumber(0, 255) : Green).LimitRange<int>(0, 255);
            Blue = (Blue < 0 ? Ext.RandomNumber(0, 255) : Blue).LimitRange<int>(0, 255);
            Alpha = Alpha.LimitRange<int>(0, 255);
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        /// <summary>
        /// Retorna a <see cref="Color"/> a partir de uma <see cref="ConsoleColor"/>
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static HSVColor ToColor(this ConsoleColor Color) => new HSVColor(new[] { 0x0, 0x80, 0x8000, 0x8080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0xFF, 0xFF00, 0xFFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF }[(int)Color]) { Alpha = 255 };

        /// <summary>
        /// Gera uma cor a partir de uma palavra
        /// </summary>
        /// <param name="Text">
        /// Pode ser um texto em branco (Transparent), uma <see cref="NamedColors"/> (retorna aquela
        /// cor exata), uma palavra qualquer (gera proceduralmente uma cor) ou uma expressão de cor
        /// (Red+Blue, Red-Blue,Green*Red etc).
        /// </param>
        /// <returns></returns>
        public static Color ToColor(this string Text)
        {
            if (Text.IsBlank()) return Color.Transparent;

            if (Text == "random") return RandomColor();

            if (Text.IsNumber()) return Color.FromArgb(Text.ToInt());

            if (Text.IsHexaDecimalColor()) return ColorTranslator.FromHtml("#" + Text.TrimFirstEqual("#"));

            var maybecolor = FindColor(Text);
            if (maybecolor != null)
            {
                return maybecolor.ToDrawingColor();
            }

            if (Text.Contains("+"))
            {
                var various = Text.Split("+");

                if (various.Any())
                {
                    return various.Select(x => new HSVColor(x.Trim())).Aggregate((a, b) => a + b);
                }
            }

            if (Text.Contains("-"))
            {
                var various = Text.Split("-");
                if (various.Any())
                {
                    return various.Select(x => new HSVColor(x.Trim())).Aggregate((a, b) => a - b);
                }
            }
            if (Text.Contains("*"))
            {
                var various = Text.Split("*");

                if (various.Any())
                {
                    return various.Select(x => new HSVColor(x.Trim())).Aggregate((a, b) => a * b);
                }
            }

            var coresInt = Text.GetWords().Select(p => p.ToCharArray().Sum(a => Math.Pow(a.ToAsc(), 2d) * p.Length)).Sum().RoundInt();
            return Color.FromArgb(255, Color.FromArgb(coresInt));
        }

        /// <summary>
        /// Retorna a <see cref="ConsoleColor"/> mais proxima de uma <see cref="Color"/>
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static ConsoleColor ToConsoleColor(this Color Color)
        {
            int index = Color.R > 128 | Color.G > 128 | Color.B > 128 ? 8 : 0;
            index |= Color.R > 64 ? 4 : 0;
            index |= Color.G > 64 ? 2 : 0;
            index |= Color.B > 64 ? 1 : 0;
            return (ConsoleColor)index;
        }

        public static string ToCssRGB(this Color Color) => "rgb(" + Color.R.ToString() + "," + Color.G.ToString() + "," + Color.B.ToString() + ")";

        /// <summary>
        /// Converte uma cor de sistema para CSS RGB
        /// </summary>
        /// <param name="Color">Cor do sistema</param>
        /// <returns>String contendo a cor em RGB</returns>
        public static string ToCssRGBA(this Color Color) => "rgba(" + Color.R.ToString() + "," + Color.G.ToString() + "," + Color.B.ToString() + "," + Color.A.ToString() + ")";

        public static string ToHexadecimal(this Color Color, bool Hash = true) => (Color.R.ToString("X2") + Color.G.ToString("X2") + Color.B.ToString("X2")).PrependIf("#", Hash);

        public static IEnumerable<HSVColor> ToHSVColorList(this IEnumerable<Color> ColorList) => ColorList?.Select(x => new HSVColor(x));

        #endregion Public Methods

        /// <summary>
        /// Converte uma cor de sistema para hexadecimal
        /// </summary>
        /// <param name="Color">Cor do sistema</param>
        /// <param name="Hash">
        /// parametro indicando se a cor deve ser retornada com ou sem hashsign (#)
        /// </param>
        /// <returns>string contendo o hexadecimal da cor</returns>
    }
}