using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using InnerLibs.LINQ;

namespace InnerLibs
{
    public class HSVColor : IComparable<int>, IComparable<HSVColor>, IComparable<Color>, IComparable
    {
        private double _h, _s, _v;
        private string _name;
        private Color _scolor;

        /// <summary>
        /// Retorna a cor vibrante de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Reduce"></param>
        /// <returns></returns>
        public static HSVColor FromImage(Image Img, int Reduce = 16)
        {
            return Images.ColorPallette(Img, Reduce).Keys.FirstOr(new HSVColor());
        }

        /// <summary>
        /// Retorna uma cor aleatoria a partir da paleta de cores de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Reduce"></param>
        /// <returns></returns>
        public static HSVColor RandomColor(Image Img, int Reduce = 16)
        {
            return Img.ColorPallette(Reduce).Keys.FirstRandom();
        }

        /// <summary>
        /// Retorna uma cor aleatória a partir de uma lista de cores
        /// </summary>
        /// <param name="Colors"></param>
        /// <returns></returns>
        public static HSVColor RandomColor(IEnumerable<Color> Colors)
        {
            return new HSVColor((Color)Colors?.OrderByRandom().FirstOr(Color.Transparent));
        }

        /// <summary>
        /// Gera uma <see cref="HSVColor"/> opaca aleatoria
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static HSVColor RandomColor(string Name = null)
        {
            return new HSVColor(ColorExtensions.RandomColor(), Name);
        }

        /// <summary>
        /// Gera uma <see cref="HSVColor"/> opaca aleatoria dentro de um Mood especifico
        /// </summary>
        /// <returns></returns>
        public static HSVColor RandomColor(ColorMood Mood)
        {
            return RandomColorList(1, Mood).FirstOrDefault();
        }

        /// <summary>
        /// Gera uma <see cref="HSVColor"/> opaca aleatoria dentro de uma especificacao
        /// </summary>
        /// <returns></returns>
        public static HSVColor RandomColor(Expression<Func<HSVColor, bool>> predicate)
        {
            return RandomColorList(1, predicate).FirstOrDefault();
        }

        /// <summary>
        /// Gera uma lista com <see cref="HSVColor"/>   aleatorias
        /// </summary>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public static IEnumerable<HSVColor> RandomColorList(int Quantity, ColorMood Mood)
        {
            var l = new List<HSVColor>();
            while (l.Count < Quantity)
            {
                HSVColor c;
                do
                    c = RandomColor();
                while (!c.Mood.HasFlag(Mood));
                if (!l.Any(x => x.ARGB == c.ARGB))
                {
                    l.Add(c);
                }
            }

            return l;
        }

        /// <summary>
        /// Gera uma lista com <see cref="HSVColor"/>   aleatorias
        /// </summary>
        /// <param name="Quantity"></param>
        /// <returns></returns>
        public static IEnumerable<HSVColor> RandomColorList(int Quantity, Expression<Func<HSVColor, bool>> predicate)
        {
            var l = new List<HSVColor>();
            while (l.Count < Quantity)
            {
                HSVColor c = null;
                do
                    c = new[] { RandomColor() }.FirstOrDefault(predicate.Compile());
                while (c is null);
                if (!l.Any(x => x.ARGB == c.ARGB))
                {
                    l.Add(c);
                }
            }

            return l;
        }

        /// <summary>
        /// Gera uma <see cref="HSVColor"/>  aleatoria com transparencia
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static HSVColor RandomTransparentColor(string Name = null)
        {
            return RandomColor().With(x => x.Opacity = Generate.RandomNumber(0, 100)).With(x => x.Name = Name);
        }

        /// <summary>
        /// Instancia uma nova <see cref="HSVColor"/> a aprtir de seu ARGB
        /// </summary>
        public HSVColor(int ARGB) : this(Color.FromArgb(ARGB))
        {
        }

        public HSVColor(int R, int G, int B) : this(255, R, G, B)
        {
        }

        public HSVColor(int A, int R, int G, int B) : this(Color.FromArgb(A, R, G, B))
        {
        }

        /// <summary>
        /// Instancia uma nova <see cref="HSVColor"/> transparente
        /// </summary>
        public HSVColor() : this(Color.Transparent)
        {
        }

        /// <summary>
        /// Instancia uma nova <see cref="HSVColor"/> a partir de uma <see cref="System.Drawing.Color"/>
        /// </summary>
        /// <param name="Color">Cor do sistema</param>
        public HSVColor(Color Color)
        {
            _loadColor(Color);
        }

        /// <summary>
        /// Instancia uma nova <see cref="HSVColor"/> a partir de uma string de cor (colorname, hexadecimal ou string aleatoria)
        /// </summary>
        /// <param name="Color">Cor</param>
        public HSVColor(string Color) : this(Color.ToColor())
        {
            _name = Color;
        }

        /// <summary>
        /// Instancia uma nova HSVColor a partir de uma string de cor (colorname, hexadecimal ou  string aleatoria) e um Nome
        /// </summary>
        /// <param name="Color">Cor</param>
        /// <param name="Name">Nome da cor</param>
        public HSVColor(string Color, string Name) : this(Color.ToColor())
        {
            _name = Name.IfBlank(Color);
        }

        /// <summary>
        /// Instancia uma nova HSVColor a partir de uma <see cref="System.Drawing.Color"/> e um Nome
        /// </summary>
        /// <param name="Color">Cor</param>
        /// <param name="Name">Nome da cor</param>
        public HSVColor(Color Color, string Name) : this(Color)
        {
            _name = Name;
        }

        /// <summary>
        /// Retorna ou seta o valor ARGB de 32 bits dessa cor
        /// </summary>
        /// <returns></returns>
        public int ARGB
        {
            get
            {
                return _scolor.ToArgb();
            }

            set
            {
                _scolor = Color.FromArgb(value);
                _loadColor(_scolor);
            }
        }

        /// <summary>
        /// Hue (Matiz)
        /// </summary>
        /// <returns></returns>
        public double Hue
        {
            get
            {
                return _h;
            }

            set
            {
                if (_h != value)
                {
                    _h = value;
                    while (_h < 0d)
                        _h += 360d;
                    while (_h > 360d)
                        _h -= 360d;
                    SetColor();
                }
            }
        }

        /// <summary>
        /// Saturation (Saturação)
        /// </summary>
        /// <returns></returns>
        public double Saturation
        {
            get
            {
                return _s;
            }

            set
            {
                value = value.LimitRange(0.0d, 1.0d);
                if (_s != value)
                {
                    _s = value;
                    SetColor();
                }
            }
        }

        /// <summary>
        /// Luminância
        /// </summary>
        /// <returns></returns>
        public double Luminance
        {
            get
            {
                return 0.2126d * Red + 0.7152d * Green + 0.0722d * Blue;
            }
        }

        /// <summary>
        /// Brilho
        /// </summary>
        /// <returns></returns>
        public double Brightness
        {
            get
            {
                return _v;
            }

            set
            {
                value = value.LimitRange(0.0d, 1.0d);
                if (_v != value)
                {
                    _v = value;
                    SetColor();
                }
            }
        }

        /// <summary>
        /// Red (Vermelho)
        /// </summary>
        /// <returns></returns>
        public int Red
        {
            get
            {
                return _scolor.R;
            }

            set
            {
                _scolor = Color.FromArgb(Alpha, value.LimitRange<int>(0, 255), Green, Blue);
                _loadColor(_scolor);
            }
        }

        /// <summary>
        /// Green (Verde)
        /// </summary>
        /// <returns></returns>
        public int Green
        {
            get
            {
                return _scolor.G;
            }

            set
            {
                _scolor = Color.FromArgb(Alpha, Red, value.LimitRange<int>(0, 255), Blue);
                _loadColor(_scolor);
            }
        }

        /// <summary>
        /// Blue (Azul)
        /// </summary>
        /// <returns></returns>
        public int Blue
        {
            get
            {
                return _scolor.B;
            }

            set
            {
                _scolor = Color.FromArgb(Alpha, Red, Green, value.LimitRange<int>(0, 255));
                _loadColor(_scolor);
            }
        }

        /// <summary>
        /// Alpha (Transparencia)
        /// </summary>
        /// <returns></returns>
        public byte Alpha
        {
            get
            {
                return _scolor.A;
            }

            set
            {
                _scolor = Color.FromArgb(value.LimitRange<byte>(0, 255), Red, Green, Blue);
                _loadColor(_scolor);
            }
        }

        /// <summary>
        /// Opacidade (de 1 a 100%)
        /// </summary>
        /// <returns></returns>
        public decimal Opacity
        {
            get
            {
                return Alpha.ToDecimal().CalculatePercent(255m);
            }

            set
            {
                Alpha = decimal.ToByte(value.LimitRange(0, 100).CalculateValueFromPercent(255).LimitRange(0, 255));
            }
        }

        /// <summary>
        /// Valor hexadecimal desta cor
        /// </summary>
        /// <returns></returns>
        public string Hexadecimal
        {
            get
            {
                return _scolor.ToHexadecimal();
            }

            set
            {
                if (value.IsHexaDecimalColor())
                {
                    _scolor = value.ToColor();
                    _loadColor(_scolor);
                }
            }
        }

        /// <summary>
        /// Valor RGBA() desta cor
        /// </summary>
        /// <returns></returns>
        public string CSS
        {
            get
            {
                if (Alpha == 255)
                    return _scolor.ToCssRGB();
                else
                    return _scolor.ToCssRGBA();
            }
        }

        /// <summary>
        /// Mood da cor
        /// </summary>
        /// <returns></returns>
        public ColorMood Mood
        {
            get
            {
                ColorMood m;
                if (IsDark())
                {
                    m = ColorMood.Dark;
                }
                else if (IsLight())
                {
                    m = ColorMood.Light;
                }
                else
                {
                    m = ColorMood.Medium;
                }

                if (IsMediumDark())
                {
                    m = m | ColorMood.MediumDark;
                }
                else
                {
                    m = m | ColorMood.MediumLight;
                }

                if (IsWarmer())
                {
                    m = m | ColorMood.Warmer;
                }

                if (IsWarm())
                {
                    m = m | ColorMood.Warm;
                }

                if (IsCool())
                {
                    m = m | ColorMood.Cool;
                }

                if (IsCooler())
                {
                    m = m | ColorMood.Cooler;
                }

                if (IsSad())
                {
                    m = m | ColorMood.Sad;
                }
                else
                {
                    m = m | ColorMood.Happy;
                }

                if (Opacity < 15m)
                {
                    m = m | ColorMood.Unvisible;
                }
                else if (Opacity < 60m)
                {
                    m = m | ColorMood.SemiVisible;
                }
                else
                {
                    m = m | ColorMood.Visible;
                }

                if (Luminance >= 250d)
                {
                    m = m | ColorMood.LowLuminance;
                }
                else if (Luminance <= 15d)
                {
                    m = m | ColorMood.HighLuminance;
                }

                if (Red == 255)
                {
                    m = m | ColorMood.Red;
                }

                if (Green == 255)
                {
                    m = m | ColorMood.Green;
                }

                if (Blue == 255)
                {
                    m = m | ColorMood.Blue;
                }

                if (Red == 0)
                {
                    m = m | ColorMood.NoRed;
                }

                if (Green == 0)
                {
                    m = m | ColorMood.NoGreen;
                }

                if (Blue == 0)
                {
                    m = m | ColorMood.NoBlue;
                }

                if (Red > Blue && Red > Green)
                {
                    m = m | ColorMood.MostRed;
                }

                if (Green > Red && Green > Blue)
                {
                    m = m | ColorMood.MostGreen;
                }

                if (Blue > Red && Blue > Green)
                {
                    m = m | ColorMood.MostBlue;
                }

                if (_scolor.IsKnownColor)
                {
                    m = m | ColorMood.KnowColor;
                }

                return m;
            }
        }

        public bool HasMood(params ColorMood[] Mood)
        {
            return Mood?.All(x => this.Mood.HasFlag(x)) == true;
        }

        public bool HasAnyMood(params ColorMood[] Mood)
        {
            return Mood?.Any(x => this.Mood.HasFlag(x)) == true;
        }

        public bool NotHasMood(params ColorMood[] Mood)
        {
            return Mood?.All(x => this.Mood.HasFlag(x) == false) == true;
        }

        public object DominantValue
        {
            get
            {
                return new[] { Red, Green, Blue }.Max();
            }
        }

        public HSVColor GetDominantColor()
        {
            if (Mood.HasFlag(ColorMood.MostRed))
            {
                return RedPart();
            }

            if (Mood.HasFlag(ColorMood.MostGreen))
            {
                return GreenPart();
            }

            if (Mood.HasFlag(ColorMood.MostBlue))
            {
                return BluePart();
            }

            return this;
        }

        public Bitmap CreateSolidImage(int Width, int Height)
        {
            return new Bitmap(_scolor.CreateSolidImage(Width, Height));
        }

        public Bitmap CreateSolidImage(string Size = "")
        {
            var s = Size.IfBlank("100").ParseSize();
            return CreateSolidImage(s.Width, s.Height);
        }

        public Bitmap ImageSample
        {
            get
            {
                return (Bitmap)CreateSolidImage().DrawString(Name);
            }
        }

        /// <summary>
        /// Nome atribuido a esta cor
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get
            {
                return _name.IfBlank(ClosestColorName);
            }

            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Nome original mais proximo desta cor
        /// </summary>
        /// <returns></returns>
        public string ClosestColorName
        {
            get
            {
                return _scolor.GetClosestColorName();
            }
        }

        /// <summary>
        /// Descricao desta cor
        /// </summary>
        /// <returns></returns>
        public string Description { get; set; }

        private void SetColor()
        {
            double H, S, V;
            byte alpha = _scolor.A;
            H = Hue;
            S = Saturation;
            V = Brightness;
            H = H / 360d;
            byte MAX = 255;
            if (S > 0d)
            {
                if (H >= 1d)
                    H = 0d;
                H = 6d * H;
                int hueFloor = (int)Math.Round(Math.Floor(H));
                byte a = (byte)Math.Round(Math.Round(MAX * V * (1.0d - S)));
                byte b = (byte)Math.Round(Math.Round(MAX * V * (1.0d - S * (H - hueFloor))));
                byte c = (byte)Math.Round(Math.Round(MAX * V * (1.0d - S * (1.0d - (H - hueFloor)))));
                byte d = (byte)Math.Round(Math.Round(MAX * V));
                switch (hueFloor)
                {
                    case 0:
                        {
                            _scolor = Color.FromArgb(alpha, d, c, a);
                            break;
                        }

                    case 1:
                        {
                            _scolor = Color.FromArgb(alpha, b, d, a);
                            break;
                        }

                    case 2:
                        {
                            _scolor = Color.FromArgb(alpha, a, d, c);
                            break;
                        }

                    case 3:
                        {
                            _scolor = Color.FromArgb(alpha, a, b, d);
                            break;
                        }

                    case 4:
                        {
                            _scolor = Color.FromArgb(alpha, c, a, d);
                            break;
                        }

                    case 5:
                        {
                            _scolor = Color.FromArgb(alpha, d, a, b);
                            break;
                        }

                    default:
                        {
                            _scolor = Color.FromArgb(0, 0, 0, 0);
                            break;
                        }
                }
            }
            else
            {
                byte d = (byte)Math.Round(V * MAX);
                _scolor = Color.FromArgb(alpha, d, d, d);
            }

            _s = S.LimitRange(0d, 1.0d);
            _v = V.LimitRange(0d, 1.0d);
        }

        private void _loadColor(Color Color)
        {
            _scolor = Color;
            _name = _scolor.Name;
            double r = Color.R / 255d;
            double g = Color.G / 255d;
            double b = Color.B / 255d;
            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            _v = max;
            double delta = max - min;
            if (max == 0d || delta == 0d)
            {
                _s = 0d;
                _h = 0d;
            }
            else
            {
                _s = delta / max;
                if (r == max)
                {
                    // entre amarelo e magenta
                    _h = (g - b) / delta;
                }
                else if (g == max)
                {
                    // Entre ciano e amarelo
                    _h = 2d + (b - r) / delta;
                }
                else
                {
                    // entre magenta e ciano
                    _h = 4d + (r - g) / delta;
                }

                _h *= 60d;
                if (_h < 0d)
                {
                    _h += 360d;
                }
            }
        }

        /// <summary>
        /// Retorna uma <see cref="System.Drawing.Color"/> desta <see cref="HSVColor"/>
        /// </summary>
        /// <returns></returns>
        public Color ToSystemColor()
        {
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        /// <summary>
        /// Verifica se uma cor é legivel sobre outra cor
        /// </summary>
        /// <param name="BackgroundColor"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public bool IsReadable(HSVColor BackgroundColor, int Size = 10)
        {
            return _scolor.IsReadable(BackgroundColor._scolor, Size);
        }

        /// <summary>
        /// Retorna uma cor mais clara a partir desta cor
        /// </summary>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public HSVColor MakeLighter(float Percent = 50f)
        {
            return new HSVColor(_scolor.MakeLighter(Percent));
        }

        /// <summary>
        /// Retorna uma cor mais escura a partir desta cor
        /// </summary>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public HSVColor MakeDarker(float Percent = 50f)
        {
            return new HSVColor(_scolor.MakeDarker(Percent));
        }

        /// <summary>
        /// Verifica se uma cor e considerada clara
        /// </summary>
        /// <returns></returns>
        public bool IsLight()
        {
            return Luminance.IsGreaterThan(160.0d);
        }

        /// <summary>
        /// Verifica se uma cor e considerada escura
        /// </summary>
        /// <returns></returns>
        public bool IsDark()
        {
            return Luminance.IsLessThan(70.0d);
        }

        /// <summary>
        /// Verifica se uma cor e considerada Medio Clara
        /// </summary>
        /// <returns></returns>
        public bool IsMediumLight()
        {
            return Luminance > 255d / 2d;
        }

        /// <summary>
        /// Verifica se uma cor e considerada Medio Escura
        /// </summary>
        /// <returns></returns>
        public bool IsMediumDark()
        {
            return !IsMediumLight();
        }

        /// <summary>
        /// Verifica se uma cor e considerada média
        /// </summary>
        /// <returns></returns>
        public bool IsMedium()
        {
            return Luminance.IsBetweenOrEqual(70.0d, 160.0d);
        }

        public bool IsWarm()
        {
            return Hue.IsLessThan(90.0d) || Hue.IsGreaterThan(270.0d);
        }

        public bool IsWarmer()
        {
            return Hue.IsLessThan(45.0d) || Hue.IsGreaterThan(315.0d);
        }

        public bool IsCool()
        {
            return !IsWarm();
        }

        public bool IsCooler()
        {
            return Hue.IsLessThan(225.0d) || Hue.IsGreaterThan(135.0d);
        }

        public bool IsSad()
        {
            return Saturation.IsLessThan(0.5d) || Brightness.IsLessThan(0.75d);
        }

        public bool IssHappy()
        {
            return !IsSad();
        }

        /// <summary>
        /// Retorna uma cópia desta cor
        /// </summary>
        /// <returns></returns>
        public HSVColor Clone()
        {
            return new HSVColor(_scolor, Name) { Description = Description };
        }

        /// <summary>
        /// Retorna a combinação de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Combine(HSVColor Color)
        {
            if (Color != null)
            {
                return new HSVColor() { Red = Red ^ Color.Red, Green = Green ^ Color.Green, Blue = Blue ^ Color.Blue, Alpha = Alpha };
            }

            return Clone();
        }

        /// <summary>
        /// Retorna a distancia entre 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public double Distance(HSVColor Color)
        {
            return Math.Sqrt(3 * (Color.Red - Red) * (Color.Red - Red) + 4 * (Color.Green - Green) * (Color.Green - Green) + 2 * (Color.Blue - Blue) * (Color.Blue - Blue));
        }

        /// <summary>
        /// Retorna uma nova cor a partir da mistura multiplicativa de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Multiply(HSVColor Color)
        {
            var n = Clone();
            if (Color != null)
            {
                n.Red = (int)(Red / 255d * Color.Red).LimitRange(0, 255);
                n.Green = (int)(Green / 255d * Color.Green).LimitRange(0, 255);
                n.Blue = (int)(Blue / 255d * Color.Blue).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Retorna uma nova cor a partir da mistura subtrativa de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Subtractive(HSVColor Color)
        {
            var n = Clone();
            if (Color != null)
            {
                n.Red = (n.Red + (Color.Red - 255)).LimitRange(0, 255);
                n.Green = (n.Green + (Color.Green - 255)).LimitRange(0, 255);
                n.Blue = (n.Blue + (Color.Blue - 255)).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Retorna uma nova cor a partir da mistura aditiva de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Addictive(HSVColor Color)
        {
            var n = Clone();
            if (Color != null)
            {
                n.Red = (n.Red + Color.Red).LimitRange(0, 255);
                n.Green = (n.Green + Color.Green).LimitRange(0, 255);
                n.Blue = (n.Blue + Color.Blue).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Retorna uma nova cor a partir da diferença de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Difference(HSVColor Color)
        {
            var n = Clone();
            if (Color != null)
            {
                n.Red = (n.Red - Color.Red).LimitRange(0, 255);
                n.Green = (n.Green - Color.Green).LimitRange(0, 255);
                n.Blue = (n.Blue - Color.Blue).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Retorna a cor media entre 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Average(HSVColor Color)
        {
            if (Color != null)
            {
                return new HSVColor() { Red = (int)Math.Round(new[] { Red, Color.Red }.Average()), Green = (int)Math.Round(new[] { Green, Color.Green }.Average()), Blue = (int)Math.Round(new[] { Blue, Color.Blue }.Average()), Alpha = Alpha };
            }

            return Clone();
        }

        /// <summary>
        /// Extrai os tons marrons de uma cor (filtro sépia)
        /// </summary>
        /// <returns></returns>
        public HSVColor Sepia()
        {
            var c = Clone();
            c.Red = (int)Math.Round(Math.Round(Red * 0.393d + Green * 0.769d + Blue * 0.189d));
            c.Green = (int)Math.Round(Math.Round(Red * 0.349d + Green * 0.686d + Blue * 0.168d));
            c.Blue = (int)Math.Round(Math.Round(Red * 0.272d + Green * 0.534d + Blue * 0.131d));
            return c;
        }

        public HSVColor ContrastColor()
        {
            return new HSVColor(_scolor.GetContrastColor());
        }

        /// <summary>
        /// Extrai a cor negativa desta cor
        /// </summary>
        /// <returns></returns>
        public HSVColor Negative()
        {
            return new HSVColor(_scolor.GetNegativeColor());
        }

        /// <summary>
        /// Extrai o cinza desta cor
        /// </summary>
        /// <returns></returns>
        public HSVColor Grey()
        {
            double v = 0.35d + 13 * (Red + Green + Blue) / 60d;
            return new HSVColor(Color.FromArgb((int)Math.Round(v), (int)Math.Round(v), (int)Math.Round(v)));
        }

        public HSVColor RedPart()
        {
            return new HSVColor(Color.FromArgb(Red, 0, 0));
        }

        public HSVColor GreenPart()
        {
            return new HSVColor(Color.FromArgb(0, Green, 0));
        }

        public HSVColor BluePart()
        {
            return new HSVColor(Color.FromArgb(0, 0, Blue));
        }

        /// <summary>
        /// Cria uma paleta de cores usando esta cor como base e um metodo especifico
        /// </summary>
        /// <param name="PalleteType"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] CreatePallete(string PalleteType, int Amount = 4)
        {
            var rl = new List<HSVColor>();
            foreach (var item in Monochromatic(Amount))
            {
                HSVColor[] c = (HSVColor[])item.GetType().GetMethod(PalleteType).Invoke(item, new[] { (object)false });
                rl.AddRange(c);
            }

            return rl.ToArray();
        }

        /// <summary>
        /// Retorna  novas HSVColor a partir da cor atual, movendo ela N graus na roda de cores
        /// </summary>
        /// <param name="excludeMe">Inclui esta cor no array</param>
        /// <param name="Degrees">Lista contendo os graus que serão movidos na roda de cores.</param>
        /// <returns></returns>
        public HSVColor[] ModColor(bool ExcludeMe, params int[] Degrees)
        {
            if (!ExcludeMe)
            {
                return new[] { this }.ToArray().Union(ModColor(Degrees ?? Array.Empty<int>()).ToArray()).ToArray();
            }

            return ModColor(Degrees ?? Array.Empty<int>()).ToArray();
        }

        /// <summary>
        /// Retorna  novas HSVColor a partir da cor atual, movendo ela N graus na roda de cores
        /// </summary>
        /// <param name="Degrees">Lista contendo os graus que serão movidos na roda de cores.</param>
        /// <returns></returns>
        public HSVColor[] ModColor(params int[] Degrees)
        {
            return (Degrees ?? Array.Empty<int>()).Select(x => new HSVColor() { Hue = (Hue + x) % 360d, Saturation = Saturation, Brightness = Brightness }).OrderBy(x => x.Hue).ToArray();
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Retorna as cores Quadraadas (tetradicas) desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Tetradic(bool ExcludeMe = false)
        {
            return Square(ExcludeMe);
        }

        /// <summary>
        /// Retorna as cores análogas desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Analogous(bool ExcludeMe = false)
        {
            return ModColor(ExcludeMe, 45, -45);
        }

        /// <summary>
        /// Retorna as cores Quadraadas (tetradicas) desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Square(bool ExcludeMe = false)
        {
            return ModColor(ExcludeMe, 90, 180, 260);
        }

        /// <summary>
        /// Retorna as cores triadicas desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Triadic(bool ExcludeMe = false)
        {
            return ModColor(ExcludeMe, 120, -120);
        }

        /// <summary>
        /// Retorna as cores complementares desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Complementary(bool ExcludeMe = false)
        {
            return ModColor(ExcludeMe, 180);
        }

        /// <summary>
        /// Retorna as cores split-complementares desta cor
        /// </summary>
        /// <param name="IncludeMe"></param>
        /// <returns></returns>
        public HSVColor[] SplitComplementary(bool IncludeMe = false)
        {
            return ModColor(IncludeMe, 150, 210);
        }

        /// <summary>
        /// Retorna <paramref name="Amount"/> variacoes cores a partir da cor atual
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] Monochromatic(decimal Amount = 4m)
        {
            return ColorExtensions.MonochromaticPallete(_scolor, (int)Math.Round(Amount)).ToArray();
        }

        /// <summary>
        /// Retorna uma paleta de cores tetradica (Monochromatica + Tetradica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] TetradicPallete(int Amount = 3)
        {
            return Monochromatic(Amount).SelectMany(item => item.Tetradic()).ToArray();
        }

        /// <summary>
        /// Retorna uma paleta de cores triadica (Monochromatica + Triadica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] TriadicPallete(int Amount = 3)
        {
            return Monochromatic(Amount).SelectMany(item => item.Triadic()).ToArray();
        }

        /// <summary>
        /// Retorna uma paleta de cores complementares (complementares + monocromatica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] ComplementaryPallete(int Amount = 3)
        {
            return Monochromatic(Amount).SelectMany(item => item.Complementary()).ToArray();
        }

        /// <summary>
        /// Retorna uma paleta de cores split-complementares (split-complementares + monocromatica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] SplitComplementaryPallete(int Amount = 3)
        {
            return Monochromatic(Amount).SelectMany(item => item.SplitComplementary()).ToArray();
        }

        public int CompareTo(int other)
        {
            return ARGB.CompareTo(other);
        }

        public int CompareTo(HSVColor other)
        {
            return ARGB.CompareTo(other?.ARGB);
        }

        public int CompareTo(Color other)
        {
            return CompareTo(new HSVColor(other));
        }

        public int CompareTo(object obj)
        {
            return CompareTo(new HSVColor(obj?.ToString()));
        }

        public static HSVColor operator +(HSVColor Color1, HSVColor Color2)
        {
            return Color1.Combine(Color2);
        }

        public static HSVColor operator +(Color Color1, HSVColor Color2)
        {
            return new HSVColor(Color1).Combine(Color2);
        }

        public static HSVColor operator +(HSVColor Color1, Color Color2)
        {
            return new HSVColor(Color2).Combine(Color1);
        }

        public static HSVColor operator %(HSVColor Color, int Degrees)
        {
            return Color.ModColor(true, Degrees).FirstOrDefault();
        }

        public static bool operator >(HSVColor Color1, HSVColor Color2)
        {
            return Color1.CompareTo(Color2) > 0;
        }

        public static bool operator <(HSVColor Color1, HSVColor Color2)
        {
            return Color1.CompareTo(Color2) < 0;
        }

        public static bool operator >=(HSVColor Color1, HSVColor Color2)
        {
            return Color1.CompareTo(Color2) >= 0;
        }

        public static bool operator <=(HSVColor Color1, HSVColor Color2)
        {
            return Color1.CompareTo(Color2) <= 0;
        }

        public static bool operator ==(HSVColor Color1, HSVColor Color2)
        {
            return Color1.CompareTo(Color2) == 0;
        }

        public static bool operator !=(HSVColor Color1, HSVColor Color2)
        {
            return Color1.CompareTo(Color2) != 0;
        }

        public static HSVColor operator -(HSVColor Color1, HSVColor Color2)
        {
            return Color1.Difference(Color2);
        }

        public static HSVColor operator -(Color Color1, HSVColor Color2)
        {
            return new HSVColor(Color1).Difference(Color2);
        }

        public static HSVColor operator -(HSVColor Color1, Color Color2)
        {
            return new HSVColor(Color2).Difference(Color1);
        }

        public static HSVColor operator *(HSVColor Color1, HSVColor Color2)
        {
            return Color1.Multiply(Color2);
        }

        public static HSVColor operator *(Color Color1, HSVColor Color2)
        {
            return new HSVColor(Color1).Multiply(Color2);
        }

        public static HSVColor operator *(HSVColor Color1, Color Color2)
        {
            return new HSVColor(Color2).Multiply(Color1);
        }

        public static implicit operator int(HSVColor Color)
        {
            return Color.ARGB;
        }

        public static implicit operator HSVColor(int Value)
        {
            return new HSVColor(Color.FromArgb(Value));
        }

        public static implicit operator HSVColor(Color Value)
        {
            return new HSVColor(Value);
        }

        public static implicit operator Color(HSVColor Value)
        {
            return Value.ToSystemColor();
        }

        public static implicit operator HSVColor(string Value)
        {
            return new HSVColor(Value);
        }

        public static implicit operator string(HSVColor Value)
        {
            return Value.Hexadecimal;
        }
    }

    [Flags]
    public enum ColorMood
    {
        Dark = 1,
        MediumDark = 2,
        Medium = 4,
        MediumLight = 8,
        Light = 16,
        Sad = 32,
        Happy = 64,
        Love = MostRed | NoGreen | Happy,
        Nature = MostGreen | Happy,
        Water = ~Red | Medium,
        Cooler = 128,
        Cool = 256,
        Warm = 512,
        Warmer = 1024,
        Ice = Blue | NoRed | Cooler,
        Fire = Red | NoBlue | Warmer,
        Unvisible = 2048,
        SemiVisible = 4096,
        Visible = 8192,
        LowLuminance = 16384,
        HighLuminance = 32768,
        Red = 65536,
        Green = 131072,
        Blue = 262144,
        MostRed = 524288,
        MostGreen = 1048576,
        MostBlue = 2097152,
        NoRed = 4194304,
        NoGreen = 8388608,
        NoBlue = 16777216,
        FullRed = NoGreen | NoBlue,
        FullGreen = NoRed | NoBlue,
        FullBlue = NoRed | NoGreen,
        KnowColor = 33554432
    }
}