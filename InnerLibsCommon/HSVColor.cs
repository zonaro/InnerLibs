
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;

namespace Extensions.Colors
{


    /// <summary>
    /// Representa uma cor no formato HSV e RGBA com metodos para manipulação de valores
    /// </summary>
    public class HSVColor : IComparable<int>, IComparable<HSVColor>, IComparable<Color>, IComparable, ICloneable
    {
        #region Private Fields

        private static readonly List<HSVColor> staticNamedColors = new List<HSVColor>();
        private double _h, _s, _v;
        private string _name;
        private Color _scolor;

        #endregion Private Fields

        #region Private Methods

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

        private void SetColor()
        {
            double H, S, V;
            byte alpha = _scolor.A;
            H = Hue;
            S = Saturation;
            V = Brightness;
            H /= 360d;
            byte MAX = 255;
            if (S > 0d)
            {
                if (H >= 1d)
                {
                    H = 0d;
                }

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

        #endregion Private Methods

        #region Public Constructors

        /// <summary>
        /// Instancia uma nova <see cref="HSVColor"/> a partir de seu ARGB
        /// </summary>
        public HSVColor(int ARGB) : this(Color.FromArgb(ARGB)) { }

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
        /// Instancia uma nova <see cref="HSVColor"/> a partir de uma <see cref="Color"/>
        /// </summary>
        /// <param name="Color">Cor do sistema</param>
        public HSVColor(Color Color) => _loadColor(Color);

        /// <summary>
        /// Instancia uma nova <see cref="HSVColor"/> a partir de uma string de cor (colorname,
        /// hexadecimal ou string aleatoria)
        /// </summary>
        /// <param name="Color">Cor</param>
        public HSVColor(string Color) : this(Color.ToColor()) => _name = Color;

        /// <summary>
        /// Instancia uma nova HSVColor a partir de uma string de cor (colorname, hexadecimal ou
        /// string aleatoria) e um Nome
        /// </summary>
        /// <param name="Color">Cor</param>
        /// <param name="Name">Nome da cor</param>
        public HSVColor(string Color, string Name) : this(Color.ToColor()) => _name = Name.IfBlank(Color);

        /// <summary>
        /// Instancia uma nova HSVColor a partir de uma <see cref="System.Drawing.Color"/> e um Nome
        /// </summary>
        /// <param name="Color">Cor</param>
        /// <param name="Name">Nome da cor</param>
        public HSVColor(Color Color, string Name) : this(Color) => _name = Name;

        public static HSVColor FromKnownColor(KnownColor color) => new HSVColor(Color.FromKnownColor(color), Util.GetEnumValueAsString(color));


        #endregion Public Constructors

        #region Public Properties



        /// <summary>
        /// Lista com todas as <see cref="HSVColor"/> com nomes oficiais
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<HSVColor> NamedColors
        {
            get
            {
                if (!staticNamedColors.Any())
                {
                    string s = Assembly.GetExecutingAssembly().GetResourceFileText("Colors.xml");
                    var doc = new XmlDocument();
                    doc.LoadXml(s);
                    foreach (XmlNode node in doc["colors"].ChildNodes)
                    {
                        staticNamedColors.Add(new HSVColor(ColorTranslator.FromHtml(node["hexadecimal"].InnerText), node["name"].InnerText));
                    }
                }
                return staticNamedColors.AsEnumerable();
            }
        }

        /// <summary>
        /// Alpha (Transparencia)
        /// </summary>
        /// <returns></returns>
        public byte Alpha
        {
            get => _scolor.A;

            set => _loadColor(Color.FromArgb(value.LimitRange<byte>(0, 255), Red, Green, Blue));
        }

        /// <summary>
        /// Retorna ou seta o valor ARGB de 32 bits dessa cor
        /// </summary>
        /// <returns></returns>
        public int ARGB
        {
            get => _scolor.ToArgb();

            set => _loadColor(Color.FromArgb(value));
        }

        /// <summary>
        /// Blue (Azul)
        /// </summary>
        /// <returns></returns>
        public int Blue
        {
            get => _scolor.B;

            set => _loadColor(Color.FromArgb(Alpha, Red, Green, value.LimitRange<int>(0, 255)));
        }

        /// <summary>
        /// Brilho
        /// </summary>
        /// <returns></returns>
        public double Brightness
        {
            get => _v;

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
        /// Nome original mais proximo desta cor
        /// </summary>
        /// <returns></returns>
        public string ClosestColorName => _scolor.GetClosestColorName();

        /// <summary>
        /// Valor RGBA() desta cor
        /// </summary>
        /// <returns></returns>
        public string CSS
        {
            get
            {
                if (Alpha == 255)
                {
                    return _scolor.ToCssRGB();
                }
                else
                {
                    return _scolor.ToCssRGBA();
                }
            }
        }


        public string Description { get; set; }

        public int DominantValue => new[] { Red, Green, Blue }.Max();

        /// <summary>
        /// Green (Verde)
        /// </summary>
        /// <returns></returns>
        public int Green
        {
            get => _scolor.G;

            set => _loadColor(Color.FromArgb(Alpha, Red, value.LimitRange<int>(0, 255), Blue));
        }

        /// <summary>
        /// Valor hexadecimal desta cor
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public string ToHexadecimal(bool hash = true) => _scolor.ToHexadecimal(hash);

        /// <summary>
        /// Valor hexadecimal desta cor
        /// </summary>
        /// <returns></returns>
        public string Hexadecimal
        {
            get => ToHexadecimal();

            set
            {
                if (value.IsHexaDecimalColor())
                {
                    _loadColor(value.ToColor());
                }
            }
        }

        /// <summary>
        /// Hue (Matiz)
        /// </summary>
        /// <returns></returns>
        public double Hue
        {
            get => _h;

            set
            {
                if (_h != value)
                {
                    _h = value;
                    while (_h < 0d)
                    {
                        _h += 360d;
                    }

                    while (_h > 360d)
                    {
                        _h -= 360d;
                    }

                    SetColor();
                }
            }
        }

        public Bitmap ImageSample => GetImageSample();
        public Bitmap GetImageSample(string size = "") => (Bitmap)CreateSolidImage(size).DrawString(Name);

        /// <summary>
        /// Luminância
        /// </summary>
        /// <returns></returns>
        public double Luminance => 0.2126d * Red + 0.7152d * Green + 0.0722d * Blue;

        public bool IsMostRed() => Red >= Blue && Red >= Green;

        public bool IsMostGreen() => Green >= Red && Green >= Blue;

        public bool IsMostBlue() => Blue >= Red && Blue >= Green;

        public bool IsLowRed() => Red < Blue && Red < Green;

        public bool IsLowGreen() => Green < Red && Green < Blue;

        public bool IsLowBlue() => Blue < Red && Blue < Green;

        public bool IsLowLuminance() => Luminance >= 250d;

        public bool IsHighLuminance() => Luminance <= 15d;


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
                    m |= ColorMood.MediumDark;
                }
                else
                {
                    m |= ColorMood.MediumLight;
                }


                m |= Temperature;
                m |= Visibility;


                if (IsSad())
                {
                    m |= ColorMood.Sad;
                }
                else
                {
                    m |= ColorMood.Happy;
                }



                if (IsLowLuminance())
                {
                    m |= ColorMood.LowLuminance;
                }
                else if (IsHighLuminance())
                {
                    m |= ColorMood.HighLuminance;
                }

                if (Red == 255)
                {
                    m |= ColorMood.Red;
                }

                if (Green == 255)
                {
                    m |= ColorMood.Green;
                }

                if (Blue == 255)
                {
                    m |= ColorMood.Blue;
                }

                if (Red == 0)
                {
                    m |= ColorMood.NoRed;
                }

                if (Green == 0)
                {
                    m |= ColorMood.NoGreen;
                }

                if (Blue == 0)
                {
                    m |= ColorMood.NoBlue;
                }

                if (IsMostRed())
                {
                    m |= ColorMood.MostRed;
                }

                if (IsMostGreen())
                {
                    m |= ColorMood.MostGreen;
                }

                if (IsMostBlue())
                {
                    m |= ColorMood.MostBlue;
                }

                if (IsLowRed())
                {
                    m |= ColorMood.LowRed;
                }

                if (IsLowGreen())
                {
                    m |= ColorMood.LowGreen;
                }

                if (IsLowBlue())
                {
                    m |= ColorMood.LowBlue;
                }

                return m;
            }
        }




        /// <summary>
        /// Nome atribuido a esta cor
        /// </summary>
        /// <returns></returns>
        public string Name
        {
            get => _name.IfBlank(ClosestColorName);

            set => _name = value;
        }

        /// <summary>
        /// Opacidade (de 1 a 100%)
        /// </summary>
        /// <returns></returns>
        public decimal Opacity
        {
            get => Alpha.ToDecimal().CalculatePercent(255m);

            set => Alpha = decimal.ToByte(value.LimitRange(0, 100).CalculateValueFromPercent(255).LimitRange(0, 255));
        }

        /// <summary>
        /// Red (Vermelho)
        /// </summary>
        /// <returns></returns>
        public int Red
        {
            get => _scolor.R;

            set => _loadColor(Color.FromArgb(Alpha, value.LimitRange<int>(0, 255), Green, Blue));
        }

        /// <summary>
        /// Saturation (Saturação)
        /// </summary>
        /// <returns></returns>
        public double Saturation
        {
            get => _s;

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

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Retorna uma lista de cores criadas a partir de strings
        /// </summary>
        /// <param name="Colors"></param>
        /// <returns></returns>
        public static IEnumerable<HSVColor> CreateColors(params string[] Colors) => Colors.IfNullOrEmpty(RandomColor().Hexadecimal).Select(x => new HSVColor(x));

        /// <summary>
        /// Retorna a cor vibrante de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Reduce"></param>
        /// <returns></returns>
        public static HSVColor FromImage(Image Img, int Reduce = 16) => Util.ColorPallette(Img, Reduce).Keys.FirstOr(new HSVColor());

        public static implicit operator Color(HSVColor Value) => Value.ToDrawingColor();

        public static implicit operator HSVColor(int Value) => new HSVColor(Color.FromArgb(Value));

        public static implicit operator HSVColor(Color Value) => new HSVColor(Value);

        public static implicit operator HSVColor(string Value) => new HSVColor(Value);

        public static implicit operator int(HSVColor Color) => Color.ARGB;

        public static implicit operator string(HSVColor Value) => Value.Hexadecimal;

        public static HSVColor operator -(HSVColor Color1, HSVColor Color2) => Color1.Difference(Color2);

        public static HSVColor operator -(Color Color1, HSVColor Color2) => new HSVColor(Color1).Difference(Color2);

        public static HSVColor operator -(HSVColor Color1, Color Color2) => new HSVColor(Color2).Difference(Color1);

        public static bool operator !=(HSVColor Color1, HSVColor Color2) => !(Color1 == Color2);

        public static HSVColor operator %(HSVColor Color, int Degrees) => Color.ModColor(true, Degrees).FirstOrDefault();

        public static HSVColor operator *(HSVColor Color1, HSVColor Color2) => Color1.Multiply(Color2);

        public static HSVColor operator *(Color Color1, HSVColor Color2) => new HSVColor(Color1).Multiply(Color2);

        public static HSVColor operator *(HSVColor Color1, Color Color2) => new HSVColor(Color2).Multiply(Color1);

        public static HSVColor operator +(HSVColor Color1, HSVColor Color2) => Color1.Combine(Color2);

        public static HSVColor operator +(Color Color1, HSVColor Color2) => new HSVColor(Color1).Combine(Color2);

        public static HSVColor operator +(HSVColor Color1, Color Color2) => new HSVColor(Color2).Combine(Color1);

        public static bool operator <(HSVColor Color1, HSVColor Color2) => Color1 != null && Color2 != null && Color1.CompareTo(Color2) < 0;

        public static bool operator <(HSVColor left, int right) => left.CompareTo(right) < 0;

        public static bool operator <(HSVColor left, Color right) => left.CompareTo(right) < 0;

        public static bool operator <=(HSVColor Color1, HSVColor Color2) => Color1 != null && Color2 != null && Color1.CompareTo(Color2) <= 0;

        public static bool operator <=(HSVColor left, int right) => left.CompareTo(right) <= 0;

        public static bool operator <=(HSVColor left, Color right) => left.CompareTo(right) <= 0;

        public static bool operator ==(HSVColor Color1, HSVColor Color2) => Color1?.ARGB == Color2?.ARGB;

        public static bool operator >(HSVColor Color1, HSVColor Color2) => Color1 != null && Color2 != null && Color1.CompareTo(Color2) > 0;

        public static bool operator >(HSVColor left, int right) => left.CompareTo(right) > 0;

        public static bool operator >(HSVColor left, Color right) => left.CompareTo(right) > 0;

        public static bool operator >=(HSVColor Color1, HSVColor Color2) => Color1 != null && Color2 != null && Color1.CompareTo(Color2) >= 0;

        public static bool operator >=(HSVColor left, int right) => left.CompareTo(right) >= 0;

        public static bool operator >=(HSVColor left, Color right) => left.CompareTo(right) >= 0;

        /// <summary>
        /// Retorna uma cor aleatoria a partir da paleta de cores de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Reduce"></param>
        /// <returns></returns>
        public static HSVColor RandomColor(Image Img, int Reduce = 16) => Img.ColorPallette(Reduce).Keys.RandomItem();

        /// <summary>
        /// Retorna uma cor aleatória a partir de uma lista de cores
        /// </summary>
        /// <param name="Colors"></param>
        /// <returns></returns>
        public static HSVColor RandomColor(IEnumerable<Color> Colors) => new HSVColor(Colors?.RandomItem() ?? Color.Transparent);

        /// <summary>
        /// Gera uma <see cref="HSVColor"/> opaca aleatoria
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static HSVColor RandomColor(string Name = null) => new HSVColor(Util.RandomColor(), Name);

        /// <summary>
        /// Gera uma <see cref="HSVColor"/> opaca aleatoria dentro de um Mood especifico
        /// </summary>
        /// <returns></returns>
        public static HSVColor RandomColor(ColorMood Mood) => RandomColorList(1, Mood).FirstOrDefault();

        /// <summary>
        /// Gera uma <see cref="HSVColor"/> opaca aleatoria dentro de uma especificacao
        /// </summary>
        /// <returns></returns>
        public static HSVColor RandomColor(Expression<Func<HSVColor, bool>> predicate) => RandomColorList(1, predicate).FirstOrDefault();

        /// <summary>
        /// Gera uma lista com <see cref="HSVColor"/> aleatorias
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
                {
                    c = RandomColor();
                }
                while (!c.Mood.HasFlag(Mood));
                if (!l.Any(x => x.ARGB == c.ARGB))
                {
                    l.Add(c);
                }
            }

            return l;
        }

        /// <summary>
        /// Gera uma lista com <paramref name="Quantity"/><see cref="HSVColor"/> aleatorias que
        /// cumprem um requisito especificado em <paramref name="predicate"/>
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
                {
                    c = new[] { RandomColor() }.FirstOrDefault(predicate.Compile());
                }
                while (c == null);
                if (!l.Any(x => x.ARGB == c.ARGB))
                {
                    l.Add(c);
                }
            }

            return l;
        }

        /// <summary>
        /// Gera uma <see cref="HSVColor"/> aleatoria com transparencia
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static HSVColor RandomTransparentColor(string Name = null) => RandomColor().With(x => x.Opacity = Util.RandomInt(0, 100)).With(x => x.Name = Name);

        /// <summary>
        /// Retorna uma nova cor a partir da mistura aditiva de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Addictive(HSVColor Color)
        {
            var n = CreateCopy();
            if (Color != null)
            {
                n.Red = (n.Red + Color.Red).LimitRange(0, 255);
                n.Green = (n.Green + Color.Green).LimitRange(0, 255);
                n.Blue = (n.Blue + Color.Blue).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Retorna as cores análogas desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Analogous(bool ExcludeMe = false) => ModColor(ExcludeMe, 45, -45);

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

            return CreateCopy();
        }

        public HSVColor BluePart() => new HSVColor(Color.FromArgb(0, 0, Blue));

        /// <summary>
        /// Retorna uma cópia desta cor
        /// </summary>
        /// <returns></returns>
        public object Clone() => CreateCopy();

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

            return CreateCopy();
        }

        public int CompareTo(int other) => ARGB.CompareTo(other);

        public int CompareTo(HSVColor other) => ARGB.CompareTo(other?.ARGB);

        public int CompareTo(Color other) => CompareTo(new HSVColor(other));

        public int CompareTo(object obj) => CompareTo(new HSVColor(obj?.ToString()));

        /// <summary>
        /// Retorna as cores complementares desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Complementary(bool ExcludeMe = false) => ModColor(ExcludeMe, 180);

        /// <summary>
        /// Retorna uma paleta de cores complementares (complementares + monocromatica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] ComplementaryPallete(int Amount = 3) => Monochromatic(Amount).SelectMany(item => item.Complementary()).ToArray();

        public HSVColor ContrastColor() => new HSVColor(_scolor.GetContrastColor());

        /// <summary>
        /// Retorna uma cópia desta cor
        /// </summary>
        /// <returns></returns>
        public HSVColor CreateCopy() => new HSVColor(_scolor, Name) { Description = Description };

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

        public Bitmap CreateSolidImage(int Width, int Height) => new Bitmap(_scolor.CreateSolidImage(Width, Height));

        public Bitmap CreateSolidImage(Size Size) => CreateSolidImage(Size.Width, Size.Height);
        public Bitmap CreateSolidImage(string Size = Util.EmptyString)
        {
            var s = Size.IfBlank("100").ParseSize();
            return CreateSolidImage(s.Width, s.Height);
        }

        /// <summary>
        /// Retorna uma nova cor a partir da diferença de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Difference(HSVColor Color)
        {
            var n = CreateCopy();
            if (Color != null)
            {
                n.Red = (n.Red - Color.Red).LimitRange(0, 255);
                n.Green = (n.Green - Color.Green).LimitRange(0, 255);
                n.Blue = (n.Blue - Color.Blue).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Retorna a distancia entre 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public double Distance(HSVColor Color) => Math.Sqrt(3 * (Color.Red - Red) * (Color.Red - Red) + 4 * (Color.Green - Green) * (Color.Green - Green) + 2 * (Color.Blue - Blue) * (Color.Blue - Blue));

        public override bool Equals(object obj)
        {
            try
            {
                return (HSVColor)obj == this;
            }
            catch
            {
                return false;
            }
        }

        public HSVColor GetDominantColor()
        {
            if (Mood.HasFlag(ColorMood.MostRed))
            {
                return RedPart();
            }
            else
            if (Mood.HasFlag(ColorMood.MostGreen))
            {
                return GreenPart();
            }
            else
            if (Mood.HasFlag(ColorMood.MostBlue))
            {
                return BluePart();
            }
            else
            {
                return this;
            }
        }

        public double GetEuclideanDistance(HSVColor Color)
        {
            double r_dist_sqrd = Math.Pow(Color.Red - (double)Red, 2d);
            double g_dist_sqrd = Math.Pow(Color.Green - (double)Green, 2d);
            double b_dist_sqrd = Math.Pow(Color.Blue - (double)Blue, 2d);
            return Math.Sqrt(r_dist_sqrd + g_dist_sqrd + b_dist_sqrd);
        }

        public override int GetHashCode() => ARGB;

        public HSVColor GreenPart() => new HSVColor(Color.FromArgb(0, Green, 0));

        /// <summary>
        /// Extrai o cinza desta cor
        /// </summary>
        /// <returns></returns>
        public HSVColor Grey()
        {
            double v = 0.35d + 13 * (Red + Green + Blue) / 60d;
            return new HSVColor(Color.FromArgb((int)Math.Round(v), (int)Math.Round(v), (int)Math.Round(v)));
        }

        public bool HasAnyMood(params ColorMood[] Mood) => Mood?.Any(x => this.Mood.HasFlag(x)) == true;

        public bool HasMood(params ColorMood[] Mood) => Mood?.All(x => this.Mood.HasFlag(x)) == true;

        public bool IsCold() => HasMood(ColorMood.Cold);

        public bool IsVeryCold() => HasMood(ColorMood.VeryCold);

        /// <summary>
        /// Verifica se uma cor e considerada escura
        /// </summary>
        /// <returns></returns>
        public bool IsDark() => Luminance.IsLessThan(70.0d);

        /// <summary>
        /// Verifica se uma cor e considerada clara
        /// </summary>
        /// <returns></returns>
        public bool IsLight() => Luminance.IsGreaterThan(160.0d);

        /// <summary>
        /// Verifica se uma cor e considerada média
        /// </summary>
        /// <returns></returns>
        public bool IsMedium() => Luminance.IsBetweenOrEqual(70.0d, 160.0d);

        /// <summary>
        /// Verifica se uma cor e considerada Medio Escura
        /// </summary>
        /// <returns></returns>
        public bool IsMediumDark() => !IsMediumLight();

        /// <summary>
        /// Verifica se uma cor e considerada Medio Clara
        /// </summary>
        /// <returns></returns>
        public bool IsMediumLight() => Luminance > 255d / 2d;

        /// <summary>
        /// Verifica se uma cor é legivel sobre outra cor
        /// </summary>
        /// <param name="BackgroundColor"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public bool IsReadable(HSVColor BackgroundColor, int Size = 10) => _scolor.IsReadable(BackgroundColor._scolor, Size);

        public bool IsSad() => Saturation.IsLessThan(0.5d) || Brightness.IsLessThan(0.75d);

        public bool IssHappy() => !IsSad();

        public bool IsHot() => Temperature.HasFlag(ColorMood.Hot);

        public bool IsVeryHot() => Temperature.HasFlag(ColorMood.VeryHot);


        public ColorMood Visibility
        {
            get
            {
                var m = ColorMood.None;

                if (Opacity < 15m)
                {
                    m |= ColorMood.Invisible;
                }
                else if (Opacity < 60m)
                {
                    m |= ColorMood.SemiVisible;
                }
                else
                {
                    m |= ColorMood.Visible;
                }
                return m;
            }
        }
        public ColorMood Temperature
        {
            get
            {
                ColorMood c = ColorMood.None;

                if (Hue >= 0 && Hue <= 10)
                {
                    c |= ColorMood.Hot | ColorMood.VeryHot;
                }
                else if (Hue > 10 && Hue <= 45)
                {
                    c |= ColorMood.Hot;
                }
                else if (Hue > 45 && Hue <= 160)
                {
                    c |= ColorMood.Cold;
                }
                else if (Hue > 160 && Hue <= 270)
                {
                    c |= ColorMood.Cold | ColorMood.VeryCold;
                }
                return c;

            }
        }

        /// <summary>
        /// Retorna uma cor mais escura a partir desta cor
        /// </summary>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public HSVColor MakeDarker(float Percent = 50f) => new HSVColor(_scolor.MakeDarker(Percent));

        /// <summary>
        /// Retorna uma cor mais clara a partir desta cor
        /// </summary>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public HSVColor MakeLighter(float Percent = 50f) => new HSVColor(_scolor.MakeLighter(Percent));

        /// <summary>
        /// Retorna novas <see cref="HSVColor"/> a partir da cor atual, movendo ela N graus na roda
        /// de cores
        /// </summary>
        /// <param name="excludeMe">Inclui esta cor no array</param>
        /// <param name="Degrees">Lista contendo os graus que serão movidos na roda de cores.</param>
        /// <returns></returns>
        public HSVColor[] ModColor(bool ExcludeMe, params int[] Degrees)
        {
            var arr = ModColor(Degrees ?? Array.Empty<int>()).ToArray();
            return !ExcludeMe
                ? new[] { this }.ToArray().Union(arr).ToArray()
                : arr;
        }

        /// <summary>
        /// Retorna novas <see cref="HSVColor"/> a partir da cor atual, movendo ela N graus na roda
        /// de cores
        /// </summary>
        /// <param name="Degrees">Lista contendo os graus que serão movidos na roda de cores.</param>
        /// <returns></returns>
        public HSVColor[] ModColor(params int[] Degrees) => (Degrees ?? Array.Empty<int>()).Select(x => new HSVColor() { Hue = (Hue + x) % 360d, Saturation = Saturation, Brightness = Brightness }).OrderBy(x => x.Hue).ToArray();

        /// <summary>
        /// Retorna <paramref name="Amount"/> variacoes cores a partir da cor atual
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] Monochromatic(decimal Amount = 4m) => Util.MonochromaticPallette(_scolor, (int)Math.Round(Amount)).ToArray();

        /// <summary>
        /// Retorna uma nova cor a partir da mistura multiplicativa de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Multiply(HSVColor Color)
        {
            var n = CreateCopy();
            if (Color != null)
            {
                n.Red = (int)(Red / 255d * Color.Red).LimitRange(0, 255);
                n.Green = (int)(Green / 255d * Color.Green).LimitRange(0, 255);
                n.Blue = (int)(Blue / 255d * Color.Blue).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Extrai a cor negativa desta cor
        /// </summary>
        /// <returns></returns>
        public HSVColor Negative() => new HSVColor(_scolor.GetNegativeColor());

        public bool NotHasMood(params ColorMood[] Mood) => Mood?.All(x => this.Mood.HasFlag(x) == false) == true;

        public HSVColor RedPart() => new HSVColor(Color.FromArgb(Red, 0, 0));

        /// <summary>
        /// Extrai os tons marrons de uma cor (filtro sépia)
        /// </summary>
        /// <returns></returns>
        public HSVColor Sepia()
        {
            var c = CreateCopy();
            c.Red = Util.RoundInt(Red * 0.393d + Green * 0.769d + Blue * 0.189d);
            c.Green = Util.RoundInt(Red * 0.349d + Green * 0.686d + Blue * 0.168d);
            c.Blue = Util.RoundInt(Red * 0.272d + Green * 0.534d + Blue * 0.131d);
            return c;
        }

        /// <summary>
        /// Retorna as cores split-complementares desta cor
        /// </summary>
        /// <param name="IncludeMe"></param>
        /// <returns></returns>
        public HSVColor[] SplitComplementary(bool IncludeMe = false) => ModColor(IncludeMe, 150, 210);

        /// <summary>
        /// Retorna uma paleta de cores split-complementares (split-complementares + monocromatica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] SplitComplementaryPallete(int Amount = 3) => Monochromatic(Amount).SelectMany(item => item.SplitComplementary()).ToArray();

        /// <summary>
        /// Retorna as cores Quadraadas (tetradicas) desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Square(bool ExcludeMe = false) => ModColor(ExcludeMe, 90, 180, 260);

        /// <summary>
        /// Retorna uma nova cor a partir da mistura subtrativa de 2 cores
        /// </summary>
        /// <param name="Color"></param>
        /// <returns></returns>
        public HSVColor Subtractive(HSVColor Color)
        {
            var n = CreateCopy();
            if (Color != null)
            {
                n.Red = (n.Red + (Color.Red - 255)).LimitRange(0, 255);
                n.Green = (n.Green + (Color.Green - 255)).LimitRange(0, 255);
                n.Blue = (n.Blue + (Color.Blue - 255)).LimitRange(0, 255);
            }

            return n;
        }

        /// <summary>
        /// Retorna as cores Quadraadas (tetradicas) desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Tetradic(bool ExcludeMe = false) => Square(ExcludeMe);

        /// <summary>
        /// Retorna uma paleta de cores tetradica (Monocromatica + Tetradica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] TetradicPallete(int Amount = 3) => Monochromatic(Amount).SelectMany(item => item.Tetradic()).ToArray();

        /// <summary>
        /// Retorna uma <see cref="System.Drawing.Color"/> desta <see cref="HSVColor"/>
        /// </summary>
        /// <returns></returns>
        public Color ToDrawingColor() => Color.FromArgb(Alpha, Red, Green, Blue);

        public override string ToString() => Name;

        /// <summary>
        /// Retorna as cores triadicas desta cor
        /// </summary>
        /// <param name="ExcludeMe"></param>
        /// <returns></returns>
        public HSVColor[] Triadic(bool ExcludeMe = false) => ModColor(ExcludeMe, 120, -120);

        /// <summary>
        /// Retorna uma paleta de cores triadica (Monocromatica + Triadica)
        /// </summary>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public HSVColor[] TriadicPallete(int Amount = 3) => Monochromatic(Amount).SelectMany(item => item.Triadic()).ToArray();

        #endregion Public Methods
    }

    [Flags]
    public enum ColorMood
    {
        None = 0,
        Dark = 1,
        MediumDark = 2,
        Medium = 4,
        MediumLight = 8,
        Light = 16,

        Sad = 32,
        Happy = 64,

        VeryCold = 128,
        Cold = 256,
        Hot = 512,
        VeryHot = 1024,

        Invisible = 2048,
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

        LowRed = 4194304,
        LowGreen = 8388608,
        LowBlue = 16777216,

        NoRed = ~Red,
        NoGreen = ~Green,
        NoBlue = ~Blue,

        FullRed = NoGreen | NoBlue,
        FullGreen = NoRed | NoBlue,
        FullBlue = NoRed | NoGreen,

        Love = MostRed | NoGreen | Happy,
        Nature = MostGreen | LowBlue | Happy,
        Water = NoRed | Medium | MostBlue,
        Ice = Blue | NoRed | VeryCold,
        Fire = Red | NoBlue | VeryHot,
    }

}

