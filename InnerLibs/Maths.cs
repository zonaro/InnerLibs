using InnerLibs.Locations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace InnerLibs
{
    /// <summary>
    /// Módulo para calculos
    /// </summary>
    /// <remarks></remarks>
    public static class MathExt
    {
        public static string ToDecimalString(this float number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this short number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this double number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this long number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this int number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this decimal number, int Decimals = -1, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            Decimals = Decimals < 0 ? GetDecimalLength(number) : Decimals;
            return number.ToString("0".AppendIf("." + "0".Repeat(Decimals), Decimals > 0), culture);
        }

        public static int GetDecimalLength(this decimal number) => BitConverter.GetBytes(decimal.GetBits(number)[3])[2];

        public static int GetDecimalLength(this double number) => number.ToDecimal().GetDecimalLength();

        /// <summary>
        /// The Collatz conjecture is one of the most famous unsolved problems in mathematics. The conjecture asks whether repeating two simple arithmetic operations will eventually transform every positive integer into 1
        /// </summary>
        /// <param name="n">Natural number greater than zero</param>
        /// <returns>an <see cref="IEnumerable{decimal}" /> with all steps until 1  </returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<decimal> CollatzConjecture(this int n)
        {
            if (n < 1)
            {
                throw new ArgumentException("n must be a natural number greater than zero.", nameof(n));
            }

            yield return n;

            decimal _n = n; //n precisa ser decimal

            while (_n > 1)
            {
                if (_n.IsEven())
                {
                    _n /= 2;
                }
                else
                {
                    _n = _n * 3 + 1;
                }

                yield return _n;
            }
        }

        /// <summary>
        /// Retorna uma progressão Aritmética com N numeros
        /// </summary>
        /// <param name="FirstNumber"></param>
        /// <param name="[Constant]"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static IEnumerable<int> ArithmeticProgression(this int FirstNumber, int Constant, int Length)
        {
            Length--;
            yield return FirstNumber;
            do
            {
                FirstNumber += Constant;
                yield return FirstNumber;
                Length--;
            } while (Length > 0);
        }

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static decimal Average(params decimal[] Values) => Values.Average();

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static double Average(params double[] Values) => Values.Average();

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static double Average(params int[] Values) => Values.Average();

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static double Average(params long[] Values) => Values.Average();

        /// <summary>
        /// Retorna uma sequencia de bytes de N entradas
        /// </summary>
        /// <param name="Length">Quantidade de numeros da sequencia</param>
        /// <returns>Lista com a sequencia de bytes</returns>
        public static IEnumerable<int> ByteSequence(this int Length)
        {
            var lista = Enumerable.Range(1, Length.SetMinValue(2)).ToList();
            for (int i = 1; i < lista.Count; i++)
            {
                lista[i] = lista[i - 1] * 2;
            }
            return lista;
        }

        /// <summary>
        /// Calcula Juros compostos
        /// </summary>
        /// <param name="Capital">Capital</param>
        /// <param name="Rate">Taxa</param>
        /// <param name="Time">Tempo</param>
        /// <returns></returns>
        public static double CalculateCompoundInterest(this double Capital, double Rate, double Time) => Capital * Math.Pow(1 + Rate, Time);

        /// <inheritdoc cref="CalculateCompoundInterest(double,double,double)" />
        public static decimal CalculateCompoundInterest(this decimal Capital, decimal Rate, decimal Time) => CalculateCompoundInterest((double)Capital, (double)Rate, (double)Time).ToDecimal();

        /// <summary>
        /// Earth's circumference at the equator in km, considering the earth is a globe, not flat
        /// </summary>
        public const double EarthCircumference = 40075d;

        /// <summary>
        /// Calcula a distancia entre 2 locais
        /// </summary>
        /// <param name="FirstLocation">Primeiro Local</param>
        /// <param name="SecondLocation">Segundo Local</param>
        /// <returns>A distancia em kilometros</returns>
        public static double CalculateDistance(this AddressInfo FirstLocation, AddressInfo SecondLocation)
        {
            double distance = 0.0d;
            if (FirstLocation?.Latitude != null && FirstLocation?.Longitude != null && SecondLocation?.Latitude != null && SecondLocation?.Longitude != null && (FirstLocation.Latitude != SecondLocation.Latitude || FirstLocation.Longitude != SecondLocation.Longitude))
            {
                // Calculate radians
                double latitude1Rad = FirstLocation.Latitude?.ToDouble().ToRadians() ?? 0;
                double longitude1Rad = FirstLocation.Longitude?.ToDouble().ToRadians() ?? 0;
                double latitude2Rad = SecondLocation.Latitude?.ToDouble().ToRadians() ?? 0;
                double longitude2Rad = SecondLocation.Longitude?.ToDouble().ToRadians() ?? 0;
                double longitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);
                if (longitudeDiff > Math.PI)
                {
                    longitudeDiff = 2.0d * Math.PI - longitudeDiff;
                }

                double angleCalculation = Math.Acos(Math.Sin(latitude2Rad) * Math.Sin(latitude1Rad) + Math.Cos(latitude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(longitudeDiff));
                distance = EarthCircumference * angleCalculation / (2.0d * Math.PI);
            }
            return distance;
        }

        /// <summary>
        /// Calcula a matriz de distancia entre varios pontos
        /// </summary>
        /// <param name="Locations">Localizacoes</param>
        /// <returns></returns>
        public static Tuple<AddressInfo, AddressInfo, double> CalculateDistanceMatrix(params AddressInfo[] Locations)
        {
            return (Tuple<AddressInfo, AddressInfo, double>)CartesianProduct(Locations, Locations).Select(x => new Tuple<AddressInfo, AddressInfo, double>(x.First(), x.Last(), x.First().CalculateDistance(x.Last())));
        }

        /// <summary>
        /// Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public static Dictionary<TKey, decimal> CalculatePercent<TKey, TValue>(this Dictionary<TKey, TValue> Dic) where TValue : struct
        {
            decimal total = Dic.Sum(x => x.Value.ChangeType<decimal>());
            return Dic.Select(x => new KeyValuePair<TKey, decimal>(x.Key, x.Value.ChangeType<decimal>().CalculatePercent(total))).ToDictionary();
        }

        /// <summary>
        /// Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static Dictionary<TKey, decimal> CalculatePercent<TObject, TKey, TValue>(this IEnumerable<TObject> Obj, Expression<Func<TObject, TKey>> KeySelector, Expression<Func<TObject, TValue>> ValueSelector) where TValue : struct => Obj.ToDictionary(KeySelector.Compile(), ValueSelector.Compile()).CalculatePercent();

        /// <summary>
        /// Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
        /// </summary>
        public static Dictionary<TObject, decimal> CalculatePercent<TObject, TValue>(this IEnumerable<TObject> Obj, Expression<Func<TObject, TValue>> ValueSelector) where TValue : struct => Obj.CalculatePercent(x => x, ValueSelector);

        /// <summary>
        /// Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
        /// </summary>
        public static Dictionary<TValue, decimal> CalculatePercent<TValue>(this IEnumerable<TValue> Obj) where TValue : struct => Obj.DistinctCount().CalculatePercent();

        public static decimal CalculatePercent(this decimal Value, decimal Total) => Total > 0 ? Convert.ToDecimal(100m * Value / Total) : 0;

        public static decimal CalculatePercent(this decimal Value, decimal Total, int DecimalPlaces) => CalculatePercent(Value, Total).RoundDecimal(DecimalPlaces);

        /// <summary>
        /// Calcula a porcentagem de objetos que cumprem um determinado critério em uma lista
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Obj"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static decimal CalculatePercentCompletion<TValue>(this IEnumerable<TValue> Obj, Expression<Func<TValue, bool>> selector)
        {
            var total = Obj.Count();
            if (selector == null)
            {
                selector = x => true;
            }
            var part = Obj.Count(selector.Compile());
            return CalculatePercent(part.ToDecimal(), total.ToDecimal());
        }

        /// <summary>
        /// Calcula a variação percentual entre 2 valores
        /// </summary>
        /// <param name="StartValue"></param>
        /// <param name="EndValue"></param>
        /// <returns></returns>
        public static decimal CalculatePercentVariation(this decimal StartValue, decimal EndValue) => StartValue == 0m ? EndValue > 0m ? 100m : 0m : (EndValue / StartValue - 1m) * 100m;

        /// <summary>
        /// Calcula a variação percentual entre 2 valores
        /// </summary>
        /// <param name="StartValue"></param>
        /// <param name="EndValue"></param>
        /// <returns></returns>
        public static decimal CalculatePercentVariation(this int StartValue, int EndValue) => StartValue.ToDecimal().CalculatePercentVariation(EndValue.ToDecimal());

        /// <summary>
        /// Calcula a variação percentual entre 2 valores
        /// </summary>
        /// <param name="StartValue"></param>
        /// <param name="EndValue"></param>
        /// <returns></returns>
        public static decimal CalculatePercentVariation(this long StartValue, long EndValue) => StartValue.ToDecimal().CalculatePercentVariation(EndValue.ToDecimal());

        /// <summary>
        /// Calcula os Juros simples
        /// </summary>
        /// <param name="Capital">Capital</param>
        /// <param name="Rate">Taxa</param>
        /// <param name="Time">Tempo</param>
        /// <returns></returns>
        public static decimal CalculateSimpleInterest(this decimal Capital, decimal Rate, decimal Time) => Capital * Rate * Time;

        public static decimal CalculateValueFromPercent(this string Percent, decimal Total) => Percent.ReplaceNone("%").ToDecimal().CalculateValueFromPercent(Total);

        public static decimal CalculateValueFromPercent(this int Percent, decimal Total) => Percent.ToDecimal().CalculateValueFromPercent(Total);

        public static decimal CalculateValueFromPercent(this decimal Percent, decimal Total) => Percent * Total / 100m;

        /// <summary>
        /// Retorna todas as possiveis combinações de Arrays do mesmo tipo (Produto Cartesiano)
        /// </summary>
        /// <param name="Sets">Lista de Arrays para combinar</param>
        /// <returns>Produto Cartesiano</returns>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(params IEnumerable<T>[] Sets)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new IEnumerable<T>[] { Enumerable.Empty<T>() };
            var c = Sets.Aggregate(emptyProduct, (accumulator, sequence) => (from accseq in accumulator from item in sequence select accseq.Concat(new T[] { item })));
            var aa = new List<IEnumerable<T>>();
            foreach (var item in c)
            {
                aa.Add(item);
            }

            return aa;
        }

        public static decimal Ceil(this decimal Number) => Math.Ceiling(Number);

        public static double Ceil(this double Number) => Math.Ceiling(Number);


        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int CeilInt(this double Number) => Number.Ceil().ToInt();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int CeilInt(this decimal Number) => Number.Ceil().ToInt();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long CeilLong(this double Number) => Number.Ceil().ToLong();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long CeilLong(this decimal Number) => Number.Ceil().ToLong();

        public static decimal CeilDecimal(this double Number) => Number.Ceil().ToDecimal();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro </returns>
        public static decimal CeilDecimal(this decimal Number) => Number.Ceil().ToDecimal();

        public static double CeilDouble(this double Number) => Number.Ceil().ToDouble();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro  </returns>
        public static double CeilDouble(this decimal Number) => Number.Ceil().ToDouble();

        /// <summary>
        /// Retorna a diferença entre 2 numeros se o valor maximo for menor que o total
        /// </summary>
        /// <param name="Total"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public static int DifferenceIfMax(this int Total, int MaxValue) => Total > MaxValue ? MaxValue - Total : 0;

        /// <summary>
        /// Retorna a diferença entre 2 numeros se o valor minimo for maior que o total
        /// </summary>
        /// <param name="Total"></param>
        /// <param name="MinValue"></param>
        /// <returns></returns>
        public static int DifferenceIfMin(this int Total, int MinValue) => Total < MinValue ? MinValue - Total : 0;

        /// <summary>
        /// Calcula o fatorial de um numero
        /// </summary>
        /// <param name="Number">Numero inteiro maior que zero</param>
        /// <returns>fatorial do numero inteiro</returns>
        /// <remarks>Numeros negativos serão tratados como numeros positivos</remarks>
        public static int Factorial(this int Number)
        {
            Number = Number.ForcePositive();
            if (Number == 0)
            {
                return Number;
            }
            else
            {
                int fact = Number;
                int counter = Number - 1;
                while (counter > 0)
                {
                    fact *= counter;
                    counter--;
                }

                return fact;
            }
        }

        /// <summary>
        /// Retorna uma sequencia Fibonacci de N numeros
        /// </summary>
        /// <param name="Length">Quantidade de numeros da sequencia</param>
        /// <returns>Lista com a sequencia Fibonacci</returns>
        public static IEnumerable<int> Fibonacci(this int Length)
        {
            var lista = new List<int>();
            lista.AddRange(new[] { 0, 1 });
            for (int index = 2, loopTo = Length - 1; index <= loopTo; index++)
            {
                lista.Add(lista[index - 1] + lista[index - 2]);
            }

            return lista;
        }

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static decimal Floor(this decimal Number) => Math.Floor(Number);

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static double Floor(this double Number) => Math.Floor(Number);

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int FloorInt(this double Number) => Number.Floor().ToInt();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int FloorInt(this decimal Number) => Number.Floor().ToInt();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long FloorLong(this double Number) => Number.Floor().ToLong();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long FloorLong(this decimal Number) => Number.Floor().ToLong();

        public static decimal ForceNegative(this decimal Value) => Value > 0m ? -Value : Value;

        public static int ForceNegative(this int Value) => Value > 0 ? -Value : Value;

        public static long ForceNegative(this long Value) => Value > 0L ? -Value : Value;

        public static double ForceNegative(this double Value) => Value > 0d ? -Value : Value;

        public static float ForceNegative(this float Value) => Value > 0f ? -Value : Value;

        public static short ForceNegative(this short Value) => (short)(Value > 0 ? -Value : Value);

        public static decimal ForcePositive(this decimal Value) => Value < 0m ? -Value : Value;

        public static int ForcePositive(this int Value) => Value < 0 ? -Value : Value;

        public static long ForcePositive(this long Value) => Value < 0L ? -Value : Value;

        public static double ForcePositive(this double Value) => Value < 0d ? -Value : Value;

        public static float ForcePositive(this float Value) => Value < 0f ? -Value : Value;

        public static short ForcePositive(this short Value) => (short)(Value < 0 ? -Value : Value);

        /// <summary>
        /// Retorna uma Progressão Gemoétrica com N numeros
        /// </summary>
        /// <param name="FirstNumber"></param>
        /// <param name="[Constant]"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static IEnumerable<int> GeometricProgression(this int FirstNumber, int Constant, int Length)
        {
            Length--;
            yield return FirstNumber;
            do
            {
                FirstNumber *= Constant;
                yield return FirstNumber;
                Length--;
            } while (Length > 0);
        }

        /// <summary>
        /// Get the Decimal Part of <see cref="decimal" /> as <see cref="long">
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static long GetDecimalPart(this decimal Value, int Length = 0)
        {
            Value = Value.ForcePositive();
            Value -= Value.Floor();
            while (Value.HasDecimalPart())
            {
                Value *= 10m;
            }

            return $"{Value}".GetFirstChars(Length).ToLong();
        }

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this int Number) => Number.ToLong().GetOrdinal();

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this decimal Number) => Number.ToLong().GetOrdinal();

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this short Number) => Number.ToLong().GetOrdinal();

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this double Number) => Number.ToLong().GetOrdinal();

        /// <summary>
        /// Returns the ordinal suffix for given <paramref name="Number"/>
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string GetOrdinal(this long Number)
        {
            switch (Number)
            {
                case 1L:
                case -1L: return $"st";
                case 2L:
                case -2L: return $"nd";
                case 3L:
                case -3L: return $"rd";
                default: return $"th";
            }
        }

        /// <summary>
        /// Check if number has decimal part
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool HasDecimalPart(this decimal Value) => !(Value.ForcePositive() % 1m == 0m) && Value.ForcePositive() > 0m;

        /// <summary>
        /// Verifica se um numero possui parte decimal
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool HasDecimalPart(this double Value) => Value.ToDecimal().HasDecimalPart();

        /// <summary>
        /// Verifica se um numero é inteiro (não possui casas decimais)
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this decimal Number) => !Number.HasDecimalPart();

        /// <summary>
        /// Verifica se um numero é inteiro (não possui casas decimais)
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this double Number) => !Number.HasDecimalPart();

        /// <summary>
        /// Realiza um calculo de interpolação Linear
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public static float Lerp(this float Start, float End, float Amount)
        {
            float difference = End - Start;
            float adjusted = difference * Amount;
            return Start + adjusted;
        }

        public static int LimitIndex<T>(this int Int, IEnumerable<T> Collection) => Int.LimitRange<int>(0, Collection.Count() - 1);

        public static long LimitIndex<T>(this long Lng, IEnumerable<T> Collection) => Lng.LimitRange<long>(0, Collection.LongCount() - 1L);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static T LimitRange<T>(this IComparable Number, IComparable MinValue = null, IComparable MaxValue = null) where T : IComparable
        {
            if (MaxValue != null)
            {
                Number = Number.IsLessThan(MaxValue.ChangeType<T>()) ? Number.ChangeType<T>() : MaxValue.ChangeType<T>();
            }

            if (MinValue != null)
            {
                Number = Number.IsGreaterThan(MinValue.ChangeType<T>()) ? Number.ChangeType<T>() : MinValue.ChangeType<T>();
            }

            return (T)Number;
        }

        /// <summary>
        /// Limita um range para um caractere
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static string LimitRange(this string Number, string MinValue = null, string MaxValue = null) => Number.LimitRange<string>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um caractere
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static char LimitRange(this char Number, char? MinValue = null, char? MaxValue = null) => Number.LimitRange<char>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static float LimitRange(this float Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<float>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static int LimitRange(this int Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<int>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static decimal LimitRange(this decimal Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<decimal>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static long LimitRange(this double Number, IComparable MinValue = null, IComparable MaxValue = null) => (long)Math.Round(Number.LimitRange<double>(MinValue, MaxValue));

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static long LimitRange(this long Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<long>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static DateTime LimitRange(this DateTime Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<DateTime>(MinValue, MaxValue);

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static decimal RoundDecimal(this decimal Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number, Decimals.Value.ForcePositive()) : Math.Round(Number);

        public static decimal RoundDecimal(this double Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number.ToDecimal(), Decimals.Value.ForcePositive()) : Math.Round(Number.ToDecimal());

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static double RoundDouble(this double Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number, Decimals.Value.ForcePositive()) : Math.Round(Number);

        public static double RoundDouble(this decimal Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number.ToDouble(), Decimals.Value.ForcePositive()) : Math.Round(Number.ToDouble());

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static int RoundInt(this decimal Number) => Math.Round(Number).ToInt();

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static int RoundInt(this double Number) => Math.Round(Number).ToInt();

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static long RoundLong(this decimal Number) => Math.Round(Number).ToLong();

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static long RoundLong(this double Number) => Math.Round(Number).ToLong();

        /// <summary>
        /// Limita o valor Maximo de um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MaxValue">Valor Maximo</param>
        /// <returns></returns>
        public static T SetMaxValue<T>(this T Number, T MaxValue) where T : IComparable => Number.LimitRange<T>(MaxValue: MaxValue);

        /// <summary>
        /// Limita o valor minimo de um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Maximo</param>
        /// <returns></returns>
        public static T SetMinValue<T>(this T Number, T MinValue) where T : IComparable => Number.LimitRange<T>(MinValue: MinValue);

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static double Sum(params double[] Values) => Values.Sum();

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static long Sum(params long[] Values) => Values.Sum();

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static int Sum(params int[] Values) => Values.Sum();

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static decimal Sum(params decimal[] Values) => Values.Sum();

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this int Number) => Number.ToLong().ToOrdinalNumber();

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this long Number) => $"{Number}{Number.GetOrdinal()}";

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this short Number) => Number.ToInt().ToOrdinalNumber();

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this double Number) => Number.FloorInt().ToOrdinalNumber();

        /// <summary>
        /// Retorna o numero em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this decimal Number) => Number.FloorInt().ToOrdinalNumber();

        /// <summary>
        /// COnverte graus para radianos
        /// </summary>
        /// <param name="Degrees"></param>
        /// <returns></returns>
        public static double ToRadians(this double Degrees) => Degrees * Math.PI / 180.0d;
    }
}