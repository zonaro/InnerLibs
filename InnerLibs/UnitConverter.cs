using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{

    /// <summary>
/// Classe para manipulaçao de numeros e conversão unidades
/// </summary>
    public class UnitConverter
    {
        private Dictionary<decimal, string> Units = new Dictionary<decimal, string>();

        public StringComparison UnitComparisonType { get; set; } = StringComparison.Ordinal;
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
    /// Inicia um <see cref="UnitConverter"/> vazio
    /// </summary>
        public UnitConverter()
        {
        }

        /// <summary>
    /// Cria um <see cref="UnitConverter"/> de Base 1000 (de y a E)
    /// </summary>
    /// <returns></returns>
        public static UnitConverter CreateBase1000Converter()
        {
            return new UnitConverter(1000m, 0.000000000000000000000001m, "y", "z", "a", "f", "p", "n", "µ", "m", "", "K", "M", "G", "T", "P", "E");
        }

        /// <summary>
    /// Cria um <see cref="UnitConverter"/> de de Massa (peso) complexos de base 10 (de mg a kg)
    /// </summary>
    /// <returns></returns>
        public static UnitConverter CreateComplexMassConverter()
        {
            return new UnitConverter(10, "mg", "cg", "dg", "g", "dag", "hg", "kg") { UnitComparisonType = StringComparison.OrdinalIgnoreCase };
        }

        /// <summary>
    /// Cria um <see cref="UnitConverter"/> de de Massa (peso) simples de base 1000 (de mg a T)
    /// </summary>
    /// <returns></returns>
        public static UnitConverter CreateSimpleMassConverter()
        {
            return new UnitConverter(1000, "mg", "g", "kg", "T") { UnitComparisonType = StringComparison.OrdinalIgnoreCase };
        }



        /// <summary>
    /// Cria um <see cref="UnitConverter"/> de Base 1024 (Bytes) de (B a EB)
    /// </summary>
    /// <returns></returns>
        public static UnitConverter CreateFileSizeConverter()
        {
            return new UnitConverter(1024, "B", "KB", "MB", "GB", "TB", "PB", "EB") { UnitComparisonType = StringComparison.OrdinalIgnoreCase };
        }

        /// <summary>
    /// Cria um <see cref="UnitConverter"/> utilizando um <see cref="Dictionary(Of Decimal,String)"/> com as marcaçoes de unidade de medida
    /// </summary>
    /// <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    /// <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
        public UnitConverter(Dictionary<decimal, string> Units)
        {
            this.Units = Units;
        }

        /// <summary>
    /// Cria um <see cref="UnitConverter"/> utilizando um <see cref="Dictionary(Of String,Decimal)"/> com as marcaçoes de unidade de medida
    /// </summary>
    /// <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    /// <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
        public UnitConverter(Dictionary<string, decimal> Units)
        {
            this.Units = Units.ToDictionary(x => x.Value, x => x.Key);
        }

        /// <summary>
    /// Cria um <see cref="UnitConverter"/> utilizando um numero inicial, uma base multiplicadora e um array com as unidades de medida
    /// </summary>
    /// <param name="StartAt">Numero Inicial</param>
    /// <param name="Base">Base multiplicadora, exponencia o numero em <paramref name="StartAt"/> para cada item em <paramref name="Units"/></param>
    /// <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    /// <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
        public UnitConverter(decimal Base, decimal StartAt, params string[] Units)
        {
            Units = Units ?? Array.Empty<string>();
            this.Units = new Dictionary<decimal, string>();
            this.Units.Add(StartAt, Units.First());
            for (int index = 1, loopTo = Units.Length - 1; index <= loopTo; index++)
            {
                StartAt = StartAt * Base;
                this.Units.Add(StartAt, Units[index]);
            }
        }

        /// <summary>
    /// Cria um <see cref="UnitConverter"/> utilizando uma base multiplicadora e um array com as unidades de medida começando pelo numero 1
    /// </summary>
    /// <param name="Base">Base multiplicadora, exponencia o numero 1 para cada item em <paramref name="Units"/></param>
    /// <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    /// <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
        public UnitConverter(int Base, params string[] Units) : this(Base, 1m, Units)
        {
        }

        /// <summary>
    /// Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    /// </summary>
    /// <param name="Number">Numero</param>
    /// <returns></returns>
        public string Abbreviate(decimal Number, int DecimalPlaces)
        {
            switch (Units.Count)
            {
                case 0:
                    {
                        return decimal.Round(DecimalPlaces).ToString();
                    }

                default:
                    {
                        var k = Units.OrderBy(x => x.Key).LastOrDefault(x => x.Key <= Number);
                        if (k.Key == 0m)
                        {
                            k = Units.OrderBy(x => x.Key).First();
                        }

                        Number = (Number / k.Key).RoundDecimal(DecimalPlaces);
                        string u = k.Value;
                        if (u.Contains(";"))
                        {
                            if (Number == 1m)
                            {
                                u = u.Split(";").First();
                            }
                            else
                            {
                                u = u.Split(";")[1];
                            }
                        }

                        string abr = Number.ToString(Culture);
                        if (abr.Contains(Culture.NumberFormat.NumberDecimalSeparator))
                        {
                            abr = abr.Trim('0');
                            abr = abr.Trim(Conversions.ToChar(Culture.NumberFormat.NumberDecimalSeparator));
                        }

                        return (abr + " " + u).Trim();
                    }
            }
        }

        public string Abreviate(decimal Number)
        {
            return Abbreviate(Number, Culture.NumberFormat.NumberDecimalDigits);
        }

        /// <summary>
    /// Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    /// </summary>
    /// <param name="Number">Numero</param>
    /// <returns></returns>
        public string Abreviate(int Number)
        {
            return Abreviate(Number.ChangeType<decimal, int>());
        }

        /// <summary>
    /// Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    /// </summary>
    /// <param name="Number">Numero</param>
    /// <returns></returns>
        public string Abreviate(short Number)
        {
            return Abreviate(Number.ChangeType<decimal, short>());
        }

        /// <summary>
    /// Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    /// </summary>
    /// <param name="Number">Numero</param>
    /// <returns></returns>
        public string Abreviate(long Number)
        {
            return Abreviate(Number.ChangeType<decimal, long>());
        }

        /// <summary>
    /// Retorna o numero decimal a partir de uma string abreviada
    /// </summary>
    /// <param name="Number"></param>
    /// <returns></returns>
        public decimal Parse(string Number, int DecimalPlaces = -1)
        {
            if (Number.IsBlank())
            {
                return 0m;
            }

            if (!Number.IsNumber())
            {
                string i = Number;
                while (i.StartsWithAny(1.ToString(), 2.ToString(), 3.ToString(), 4.ToString(), 5.ToString(), 6.ToString(), 7.ToString(), 8.ToString(), 9.ToString(), 0.ToString(), ",", "."))
                    i = i.RemoveFirstChars();
                var p = GetUnit(i);
                if (p.Key > 0m)
                {
                    return (p.Key * Number.ParseDigits().ToDecimal()).RoundDecimal(DecimalPlaces);
                }
                else
                {
                    return Number.ParseDigits().ToDecimal().RoundDecimal(DecimalPlaces);
                }
            }

            return Number.ToDecimal().RoundDecimal(DecimalPlaces);
        }

        /// <summary>
    /// Extrai a Unidade utilizada a partir de um numero abreviado
    /// </summary>
    /// <param name="Number"></param>
    /// <returns></returns>
        public string ParseUnit(string Number)
        {
            string i = Number;
            while (i.StartsWithAny(1.ToString(), 2.ToString(), 3.ToString(), 4.ToString(), 5.ToString(), 6.ToString(), 7.ToString(), 8.ToString(), 9.ToString(), 0.ToString(), ",", "."))
                i = i.RemoveFirstChars();
            var p = GetUnit(i);
            string u = p.Value.IfBlank("");
            if (u.Contains(";"))
            {
                if (Number.ParseDigits().IfBlank(1) == 1)
                {
                    u = u.Split(";").First();
                }
                else
                {
                    u = u.Split(";")[1];
                }
            }

            return u;
        }

        /// <summary>
    /// Converte um numero   decimal em outro numero decimal a partir de unidades de medida
    /// </summary>
    /// <param name="Number">Numero</param>
    /// <param name="From">Unidade de Medida de origem</param>
    /// <param name="[To]">Unidade de medida de destino</param>
    /// <returns></returns>
        public decimal Convert(decimal Number, string To, string From)
        {
            return Convert(Number + From, To);
        }

        /// <summary>
    /// Converte um numero abreviado em decimal
    /// </summary>
    /// <param name="AbreviatedNumber">Numero abreviado</param>
    /// <param name="[To]">Unidade de destino</param>
    /// <returns></returns>
        public decimal Convert(string AbreviatedNumber, string To)
        {
            decimal nn = Parse(AbreviatedNumber);
            return nn / this.GetUnit(To).Key;
        }

        /// <summary>
    /// Converte um numero abreviado em outro numero abreviado de outra unidade
    /// </summary>
    /// <param name="AbreviatedNumber">Numero abreviado</param>
    /// <param name="[To]">Unidade de destino</param>
    /// <returns></returns>
        public string ConvertAbreviate(string AbreviatedNumber, string To)
        {
            decimal nn = Parse(AbreviatedNumber);
            return (nn / this.GetUnit(To).Key + " " + To).Trim();
        }

        /// <summary>
    /// Retorna a unidade e a base a partir do nome da unidade
    /// </summary>
    /// <param name="U"></param>
    /// <returns></returns>
        private KeyValuePair<decimal, string> GetUnit(string U)
        {
            if (U.IsBlank())
            {
                return Units.SingleOrDefault(x => x.Value.IsBlank());
            }
            else
            {
                return Units.SingleOrDefault(x => x.Value.Split(";").Any(y => y.Trim().Equals(U.Trim(), UnitComparisonType)));
            }
        }
    }
}