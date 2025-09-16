using System;
using System.Globalization;
using System.Linq;
using Extensions;
using Extensions.ComplexText;

namespace Extensions.NumberWriters
{
    /// <summary>
    /// Classe para escrever numeros por extenso com suporte até 999 quintilhoes
    /// </summary>
    public class FullNumberWriter
    {

        public string InExtensiveCurrency(decimal Number, int DecimalPlaces = 2, decimal? ceilNumber = null, decimal? floorNumber = null)
        {
            if (ceilNumber.HasValue && floorNumber.HasValue)
            {
                (floorNumber, ceilNumber) = Util.FixOrder(floorNumber.Value, ceilNumber.Value);
            }

            if (ceilNumber.HasValue && Number > ceilNumber.Value)
            {
                return MoreThan + " " + InExtensiveCurrency(ceilNumber.Value, DecimalPlaces);
            }

            if (floorNumber.HasValue && Number > floorNumber.Value)
            {
                return LessThan + " " + InExtensiveCurrency(floorNumber.Value, DecimalPlaces);
            }

            long dec = 0L;
            if (DecimalPlaces > 0 && Number.HasDecimalPart())
            {
                dec = Number.GetDecimalPart(DecimalPlaces.LimitRange(0, 3));
            }

            Number = Number.Floor();
            return $"{InExtensive(Number)}{CurrencyCentsName.ToString(Number).Wrap(" ")}{(dec == 0L | DecimalPlaces == 0 ? Util.EmptyString : $"{And.Wrap(" ")}{InExtensive(dec)}{CurrencyCentsName.ToString(dec).Wrap(" ")}")}".ToLowerInvariant().TrimBetween();

        }


        public string InExtensive(decimal Number, int DecimalPlaces = 3, decimal? ceilNumber = null, decimal? floorNumber = null)
        {
            if (ceilNumber.HasValue && floorNumber.HasValue)
            {
                (floorNumber, ceilNumber) = Util.FixOrder(floorNumber.Value, ceilNumber.Value);
            }

            if (ceilNumber.HasValue && Number > ceilNumber.Value)
            {
                return MoreThan + " " + InExtensive(ceilNumber.Value, DecimalPlaces);
            }

            if (floorNumber.HasValue && Number > floorNumber.Value)
            {
                return LessThan + " " + InExtensive(floorNumber.Value, DecimalPlaces);
            }

            if (DecimalPlaces > 0 && Number.HasDecimalPart())
            {
                long dec = Number.GetDecimalPart(DecimalPlaces.LimitRange(0, 3));
                return $"{InExtensive(Number, 0)} {DecimalSeparator} {InExtensive(dec, 0)}";
            }

            Number = Number.Floor();

            switch (Number)
            {
                case var @case when @case < 0m:
                    {
                        return $"{Minus} {InExtensive(Number * -1)}";
                    }

                case 0m:
                    {
                        return Zero;
                    }

                case var case1 when 1m <= case1 && case1 <= 19m:
                    {
                        var strArray = new string[] { One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven, Twelve, Thirteen, Fourteen, Fifteen, Sixteen, Seventeen, Eighteen, Nineteen };
                        return strArray[(int)Math.Round(Number - 1m)] + " ";
                    }

                case var case2 when 20m <= case2 && case2 <= 99m:
                    {
                        var strArray = new string[] { Twenty, Thirty, Forty, Fifty, Sixty, Seventy, Eighty, Ninety };
                        if (Number % 10m == 0m)
                        {
                            return strArray[(int)((long)Math.Round(Number) / 10L - 2L)];
                        }
                        else
                        {
                            return strArray[(int)((long)Math.Round(Number) / 10L - 2L)] + And.Wrap(" ") + InExtensive(Number % 10m);
                        }
                    }

                case 100m:
                    {
                        return ExactlyOneHundred.IfBlank(OneHundred);
                    }

                case var case3 when 101m <= case3 && case3 <= 999m:
                    {
                        var strArray = new string[] { OneHundred, TwoHundred, ThreeHundred, FourHundred, FiveHundred, SixHundred, SevenHundred, EightHundred, NineHundred };
                        if (Number % 100m == 0m)
                        {
                            return strArray[(int)((long)Math.Round(Number) / 100L - 1L)] + " ";
                        }
                        else
                        {
                            return strArray[(int)((long)Math.Round(Number) / 100L - 1L)] + And.Wrap(" ") + InExtensive(Number % 100m);
                        }
                    }

                case var case4 when 1000m <= case4 && case4 <= 1999m:
                    {
                        switch (Number % 1000m)
                        {
                            case 0m:
                                {
                                    return $"{Thousand} ";
                                }

                            case var case5 when case5 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return Thousand + And.Wrap(" ") + InExtensive(Number % 1000m);
                                }

                            default:
                                {
                                    return Thousand + " " + InExtensive(Number % 1000m);
                                }
                        }
                    }

                case var case6 when 2000m <= case6 && case6 <= 999999m:
                    {
                        switch (Number % 1000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000L) + " " + Thousand;
                                }

                            case var case7 when case7 <= 100m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000L) + " " + Thousand + And.Wrap(" ") + InExtensive(Number % 1000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000L) + " " + Thousand + " " + InExtensive(Number % 1000m);
                                }
                        }
                    }

                #region Milhao

                case var case8 when 1000000m <= case8 && case8 <= 1999999m:
                    {
                        switch (Number % 1000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Million.Singular;
                                }

                            case var case9 when case9 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Million.Singular + And.Wrap(" ") + InExtensive(Number % 1000000m);
                                }

                            default:
                                {
                                    return One + " " + Million.Singular + " " + InExtensive(Number % 1000000m);
                                }
                        }
                    }

                case var case10 when 2000000m <= case10 && case10 <= 999999999m:
                    {
                        switch (Number % 1000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000L) + Million.Plural.Wrap(" ");
                                }

                            case var case11 when case11 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000L) + Million.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000L) + Million.Plural.Wrap(" ") + InExtensive(Number % 1000000m);
                                }
                        }
                    }

                #endregion Milhao

                #region Bilhao

                case var case12 when 1000000000m <= case12 && case12 <= 1999999999m:
                    {
                        switch (Number % 1000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Billion.Singular;
                                }

                            case var case13 when case13 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Billion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000m);
                                }

                            default:
                                {
                                    return One + " " + Billion.Singular + " " + InExtensive(Number % 1000000000m);
                                }
                        }
                    }

                case var case14 when 2000000000m <= case14 && case14 <= 999999999999m:
                    {
                        switch (Number % 1000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000L) + Billion.Plural.Wrap(" ");
                                }

                            case var case15 when case15 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000L) + Billion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000L) + Billion.Plural.Wrap(" ") + InExtensive(Number % 1000000000m);
                                }
                        }
                    }

                #endregion Bilhao

                #region Trilhao

                case var case16 when 1000000000000m <= case16 && case16 <= 1999999999999m:
                    {
                        switch (Number % 1000000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Trillion.Singular;
                                }

                            case var case17 when case17 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Trillion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }

                            default:
                                {
                                    return One + " " + Trillion.Singular + " " + InExtensive(Number % 1000000000000m);
                                }
                        }
                    }
                // 9.223.372.036.854.775.807
                case var case18 when 2000000000000m <= case18 && case18 <= 999999999999999m:
                    {
                        switch (Number % 1000000000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000L) + Trillion.Plural.Wrap(" ");
                                }

                            case var case19 when case19 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000L) + Trillion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000L) + Trillion.Plural.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }
                        }
                    }

                #endregion Trilhao

                #region Quadilhao

                case var case20 when 1000000000000000m <= case20 && case20 <= 1999999999999999m:
                    {
                        switch (Number % 1000000000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Quadrillion.Singular;
                                }

                            case var case21 when case21 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Quadrillion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }

                            default:
                                {
                                    return One + " " + Quadrillion.Singular + " " + InExtensive(Number % 1000000000000m);
                                }
                        }
                    }

                case var case22 when 2000000000000000m <= case22 && case22 <= 999999999999999999m:
                    {
                        switch (Number % 1000000000000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000L) + Quadrillion.Plural.Wrap(" ");
                                }

                            case var case23 when case23 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000L) + Quadrillion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000L) + Quadrillion.Plural.Wrap(" ") + InExtensive(Number % 1000000000000000m);
                                }
                        }
                    }

                #endregion Quadilhao

                #region Quintilhao

                case var case24 when 1000000000000000000m <= case24 && case24 <= 1999999999999999999m:
                    {
                        switch (Number % 1000000000000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Quintillion.Singular;
                                }

                            case var case25 when case25 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Quintillion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000000000000m);
                                }

                            default:
                                {
                                    return One + " " + Quintillion.Singular + " " + InExtensive(Number % 1000000000000000000m);
                                }
                        }
                    }

                case var case26 when 2000000000000000000m <= case26 && case26 <= 999999999999999999999m:
                    {
                        switch (Number % 1000000000000000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000000L) + Quintillion.Plural.Wrap(" ");
                                }

                            case var case27 when case27 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000000L) + Quintillion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000000L) + Quintillion.Plural.Wrap(" ") + InExtensive(Number % 1000000000000000000m);
                                }

                                #endregion Quintilhao
                        }
                    }

                default:
                    {
                        return MoreThan + " " + InExtensive(999999999999999999999m);
                    }
            }
        }






        /// <summary>
        /// Par de strings que representam os centavos desta moeda em sua forma singular ou plural
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair CurrencyCentsName { get; set; } = new QuantityTextPair("cents", "cent");

        /// <summary>
        /// Par de strings que representam os nomes da moeda em sua forma singular ou plural
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair CurrencyName { get; set; } = new QuantityTextPair("dollars", "dollar");



        /// <summary>
        /// Instancia um novo <see cref="FullNumberWriter"/> com as configurações default (inglês)
        /// </summary>
        public FullNumberWriter()
        {
            foreach (var prop in this.GetProperties().Where(x => x.CanWrite))
            {
                switch (prop.Name ?? Util.EmptyString)
                {
                    case nameof(ExactlyOneHundred):
                    case nameof(DecimalSeparator):
                    case nameof(CurrencyCentsName):
                    case nameof(CurrencyName):
                        {

                            ExactlyOneHundred = ExactlyOneHundred ?? "";
                            DecimalSeparator = DecimalSeparator ?? "dot";
                            CurrencyName = CurrencyName ?? new QuantityTextPair("dollars", "dollar");
                            CurrencyCentsName = CurrencyCentsName ?? new QuantityTextPair("cents", "cent");

                            continue;
                        }

                    default:
                        {
                            if (prop.PropertyType == typeof(string) && prop.GetValue(this).IsNotValid())
                            {
                                prop.SetValue(this, prop.Name.ToNormalCase());
                            }
                            else if (prop.PropertyType == typeof(QuantityTextPair) && prop.GetValue(this) == null)
                            {
                                var plural = prop.Name.ToNormalCase() + "s";
                                var singular = prop.Name.ToNormalCase();
                                prop.SetValue(this, new QuantityTextPair(plural, singular));
                            }

                            break;
                        }
                }
            }
        }





        /// <summary>
        /// Escreve um numero por extenso
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public virtual string this[decimal Number, int DecimalPlaces = 2] => InExtensive(Number, DecimalPlaces);


        /// <summary>
        /// String que representa a palavra "e". Utilizada na concatenação de expressões
        /// </summary>
        /// <returns></returns>
        public string And { get; set; }

        /// <summary>
        /// Par de strings que representam os numeros 1 bilhão a 999 bilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Billion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// String utilizada quando um numero possui casa decimais. Normalmente "virgula"
        /// </summary>
        /// <returns></returns>
        public string DecimalSeparator { get; set; }

        /// <summary>
        /// String que representa o numero 8.
        /// </summary>
        /// <returns></returns>
        public string Eight { get; set; }

        /// <summary>
        /// String que representa o numero 18.
        /// </summary>
        /// <returns></returns>
        public string Eighteen { get; set; }

        /// <summary>
        /// String que representa os numeros 800 a 899.
        /// </summary>
        /// <returns></returns>
        public string EightHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 80 a 89.
        /// </summary>
        /// <returns></returns>
        public string Eighty { get; set; }

        /// <summary>
        /// String que representa o numero 11.
        /// </summary>
        /// <returns></returns>
        public string Eleven { get; set; }

        /// <summary>
        /// String que represena o exato numero 100. Em alguns idiomas esta string não é nescessária
        /// </summary>
        /// <returns></returns>
        public string ExactlyOneHundred { get; set; }

        /// <summary>
        /// String que representa o numero 15.
        /// </summary>
        /// <returns></returns>
        public string Fifteen { get; set; }

        /// <summary>
        /// String que representa os numeros 50 a 59.
        /// </summary>
        /// <returns></returns>
        public string Fifty { get; set; }

        /// <summary>
        /// String que representa o numero 5.
        /// </summary>
        /// <returns></returns>
        public string Five { get; set; }

        /// <summary>
        /// String que representa os numeros 500 a 599.
        /// </summary>
        /// <returns></returns>
        public string FiveHundred { get; set; }

        /// <summary>
        /// String que representa o numero 4.
        /// </summary>
        /// <returns></returns>
        public string Four { get; set; }

        /// <summary>
        /// String que representa os numeros 400 a 499.
        /// </summary>
        /// <returns></returns>
        public string FourHundred { get; set; }

        /// <summary>
        /// String que representa o numero 14.
        /// </summary>
        /// <returns></returns>
        public string Fourteen { get; set; }

        /// <summary>
        /// String que representa os numeros 40 a 49.
        /// </summary>
        /// <returns></returns>
        public string Forty { get; set; }

        /// <summary>
        /// Par de strings que representam os numeros 1 milhão a 999 milhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Million { get; set; } = new QuantityTextPair();

        /// <summary>
        /// String que representa a palavra "Menos". Utilizada quando os números são negativos
        /// </summary>
        /// <returns></returns>
        public string Minus { get; set; }

        /// <summary>
        /// String utilizada quando o numero é maior que 999 quintilhões. Retorna uma string "Mais
        /// de 999 quintilhões"
        /// </summary>
        /// <returns></returns>
        public string MoreThan { get; set; }

        public string LessThan { get; set; }

        /// <summary>
        /// String que representa o numero 9.
        /// </summary>
        /// <returns></returns>
        public string Nine { get; set; }

        /// <summary>
        /// String que representa os numeros 900 a 999.
        /// </summary>
        /// <returns></returns>
        public string NineHundred { get; set; }

        /// <summary>
        /// String que representa o numero 19.
        /// </summary>
        /// <returns></returns>
        public string Nineteen { get; set; }

        /// <summary>
        /// String que representa os numeros 90 a 99.
        /// </summary>
        /// <returns></returns>
        public string Ninety { get; set; }

        /// <summary>
        /// String que representa o numero 1.
        /// </summary>
        /// <returns></returns>
        public string One { get; set; }

        /// <summary>
        /// String que representa os numeros 100 a 199.
        /// </summary>
        /// <returns></returns>
        public string OneHundred { get; set; }

        /// <summary>
        /// Par de strings que representam os numeros 1 quadrilhão a 999 quadrilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Quadrillion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// Par de strings que representam os numeros 1 quintilhão a 999 quintilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Quintillion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// String que representa o numero 7.
        /// </summary>
        /// <returns></returns>
        public string Seven { get; set; }

        /// <summary>
        /// String que representa os numeros 700 a 799.
        /// </summary>
        /// <returns></returns>
        public string SevenHundred { get; set; }

        /// <summary>
        /// String que representa o numero 17.
        /// </summary>
        /// <returns></returns>
        public string Seventeen { get; set; }

        /// <summary>
        /// String que representa os numeros 70 a 79.
        /// </summary>
        /// <returns></returns>
        public string Seventy { get; set; }

        /// <summary>
        /// String que representa o numero 6.
        /// </summary>
        /// <returns></returns>
        public string Six { get; set; }

        /// <summary>
        /// String que representa os numeros 600 a 699.
        /// </summary>
        /// <returns></returns>
        public string SixHundred { get; set; }

        /// <summary>
        /// String que representa o numero 16.
        /// </summary>
        /// <returns></returns>
        public string Sixteen { get; set; }

        /// <summary>
        /// String que representa os numeros 60 a 69.
        /// </summary>
        /// <returns></returns>
        public string Sixty { get; set; }

        /// <summary>
        /// String que representa o numero 10.
        /// </summary>
        /// <returns></returns>
        public string Ten { get; set; }

        /// <summary>
        /// String que representa o numero 13.
        /// </summary>
        /// <returns></returns>
        public string Thirteen { get; set; }

        /// <summary>
        /// String que representa os numeros 30 a 39.
        /// </summary>
        /// <returns></returns>
        public string Thirty { get; set; }

        /// <summary>
        /// String que representa os numeros 1000 a 9999
        /// </summary>
        /// <returns></returns>
        public string Thousand { get; set; }

        /// <summary>
        /// String que representa o numero 3.
        /// </summary>
        /// <returns></returns>
        public string Three { get; set; }

        /// <summary>
        /// String que representa os numeros 300 a 399.
        /// </summary>
        /// <returns></returns>
        public string ThreeHundred { get; set; }

        /// <summary>
        /// Par de strings que representam os numeros 1 trilhão a 999 trilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Trillion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// String que representa o numero 12.
        /// </summary>
        /// <returns></returns>
        public string Twelve { get; set; }

        /// <summary>
        /// String que representa os numeros 20 a 29 .
        /// </summary>
        /// <returns></returns>
        public string Twenty { get; set; }

        /// <summary>
        /// String que representa o numero 2.
        /// </summary>
        /// <returns></returns>
        public string Two { get; set; }

        /// <summary>
        /// String que representa os numeros 200 a 299.
        /// </summary>
        /// <returns></returns>
        public string TwoHundred { get; set; }

        /// <summary>
        /// String que representa o numero 0.
        /// </summary>
        /// <returns></returns>
        public string Zero { get; set; }



        public override string ToString() => InExtensive(0m);

       

        public static FullNumberWriter pt_BR()
        {
            return new FullNumberWriter()
            {
                One = "um",
                Two = "dois",
                Three = "três",
                Four = "quatro",
                Five = "cinco",
                Six = "seis",
                Seven = "sete",
                Eight = "oito",
                Nine = "nove",
                Ten = "dez",
                Eleven = "onze",
                Twelve = "doze",
                Thirteen = "treze",
                Fourteen = "quatorze",
                Fifteen = "quinze",
                Sixteen = "dezesseis",
                Seventeen = "dezessete",
                Eighteen = "dezoito",
                Nineteen = "dezenove",
                Twenty = "vinte",
                Thirty = "trinta",
                Forty = "quarenta",
                Fifty = "cinquenta",
                Sixty = "sessenta",
                Seventy = "setenta",
                Eighty = "oitenta",
                Ninety = "noventa",
                ExactlyOneHundred = "cem",
                OneHundred = "cento",
                TwoHundred = "duzentos",
                ThreeHundred = "trezentos",
                FourHundred = "quatrocentos",
                FiveHundred = "quinhentos",
                SixHundred = "seiscentos",
                SevenHundred = "setecentos",
                EightHundred = "oitocentos",
                NineHundred = "novecentos",
                Thousand = "mil",
                Million = ("milhão", "milhões"),
                Billion = ("bilhão", "bilhões"),
                Trillion = ("trilhão", "trilhões"),
                Quadrillion = ("quatrilhão", "quatrilhões"),
                Quintillion = ("quintilhão", "quintilhões"),
                And = "e",
                DecimalSeparator = "vírgula",
                CurrencyName = ("reais", "real"),
                CurrencyCentsName = ("centavos", "centavo"),
                Minus = "menos",
                MoreThan = "mais de",
                LessThan = "menos de"


            };
        }

    }

}