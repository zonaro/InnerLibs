using InnerLibs.Locations;
using System;
using System.Collections.Generic;
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
        /// <summary>
        /// Calcula os Juros simples
        /// </summary>
        /// <param name="Capital">Capital</param>
        /// <param name="Rate">Taxa</param>
        /// <param name="Time">Tempo</param>
        /// <returns></returns>
        public static object CalculateSimpleInterest(this decimal Capital, decimal Rate, decimal Time) => Capital * Rate * Time;

        /// <summary>
        /// Calcula Juros compostos
        /// </summary>
        /// <param name="Capital">Capital</param>
        /// <param name="Rate">Taxa</param>
        /// <param name="Time">Tempo</param>
        /// <returns></returns>
        public static object CalculateCompoundInterest(this decimal Capital, decimal Rate, decimal Time) => (double)Capital * Math.Pow((double)(1m + Rate), (double)Time);

        public static decimal ForceNegative(this decimal Value)
        {
            if (Value > 0m)
                Value = Value * -1;
            return Value;
        }

        public static int ForceNegative(this int Value)
        {
            if (Value > 0)
                Value = Value * -1;
            return Value;
        }

        public static long ForceNegative(this long Value)
        {
            if (Value > 0)
                Value = Value * -1;
            return Value;
        }

        public static double ForceNegative(this double Value)
        {
            if (Value > 0d)
                Value = Value * -1;
            return Value;
        }

        public static float ForceNegative(this float Value)
        {
            if (Value > 0f)
                Value = Value * -1;
            return Value;
        }

        public static short ForceNegative(this short Value)
        {
            if (Value > 0)
                Value = ((short)(Value * -1));
            return Value;
        }

        public static decimal ForcePositive(this decimal Value)
        {
            if (Value < 0m)
                Value = Value * -1;
            return Value;
        }

        public static int ForcePositive(this int Value)
        {
            if (Value < 0)
                Value = Value * -1;
            return Value;
        }

        public static long ForcePositive(this long Value)
        {
            if (Value < 0)
                Value = Value * -1;
            return Value;
        }

        public static double ForcePositive(this double Value)
        {
            if (Value < 0d)
                Value = Value * -1;
            return Value;
        }

        public static float ForcePositive(this float Value)
        {
            if (Value < 0f)
                Value = Value * -1;
            return Value;
        }

        public static short ForcePositive(this short Value)
        {
            if (Value < 0)
                Value = ((short)(Value * -1));
            return Value;
        }

        /// <summary>
        /// Verifica se um numero possui parte decimal
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool HasDecimalPart(this decimal Value) => !(Value.ForcePositive() % 1m == 0m) && Value.ForcePositive() > 0m;

        /// <summary>
        /// Verifica se um numero possui parte decimal
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool HasDecimalPart(this double Value) => Value.ChangeType<decimal, double>().HasDecimalPart();

        /// <summary>
        /// Retorna a diferença entre 2 numeros se o valor minimo for maior que o total
        /// </summary>
        /// <param name="Total"></param>
        /// <param name="MinValue"></param>
        /// <returns></returns>
        public static int DifferenceIfMin(this int Total, int MinValue) => Total < MinValue ? MinValue - Total : 0;

        /// <summary>
        /// Retorna a diferença entre 2 numeros se o valor maximo for menor que o total
        /// </summary>
        /// <param name="Total"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public static int DifferenceIfMax(this int Total, int MaxValue) => Total > MaxValue ? MaxValue - Total : 0;

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this int Number, bool ExcludeNumber = false) => Number.ToLong().ToOrdinalNumber(ExcludeNumber);

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this long Number, bool ExcludeNumber = false)
        {
            var suffix = "";

            var negative = Number < 0;
            switch (Number.ForcePositive())
            {
                case 1L:
                    {
                        suffix = "st";
                        break;
                    }

                case 2L:
                    {
                        suffix = "nd";
                        break;
                    }

                case 3L:
                    {
                        suffix = "rd";
                        break;
                    }

                default:
                    {
                        suffix = "th";
                        break;
                    }
            }
            Number = negative.AsIf(-Number, Number);

            return $"{Number}{suffix}";
        }

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
        /// Retorna uma progressão Aritmética com N numeros
        /// </summary>
        /// <param name="FirstNumber"></param>
        /// <param name="[Constant]"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static IEnumerable<int> ArithmeticProgression(this int FirstNumber, int Constant, int Length)
        {
            var PA = new List<int>();
            PA.Add(FirstNumber);
            for (int index = 1, loopTo = Length - 1; index <= loopTo; index++)
                PA.Add(PA.Last() + Constant);
            return PA;
        }

        /// <summary>
        /// Retorna uma Progressão Gemoétrica com N numeros
        /// </summary>
        /// <param name="FirstNumber"></param>
        /// <param name="[Constant]"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static IEnumerable<int> GeometricProgression(this int FirstNumber, int Constant, int Length)
        {
            var PG = new List<int>();
            PG.Add(FirstNumber);
            for (int index = 1, loopTo = Length - 1; index <= loopTo; index++)
                PG.Add(PG.Last() * Constant);
            return PG;
        }

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
                aa.Add(item);
            return aa;
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
                lista.Add(lista[index - 1] + lista[index - 2]);
            return lista;
        }

        /// <summary>
        /// Retorna uma sequencia Fibonacci de N numeros
        /// </summary>
        /// <param name="Length">Quantidade de numeros da sequencia</param>
        /// <returns>Lista com a sequencia Fibonacci</returns>
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
        /// Calcula o fatorial de um numero
        /// </summary>
        /// <param name="Number">Numero inteiro positivo maior que zero</param>
        /// <returns>fatorial do numero inteiro</returns>
        public static int Factorial(this int Number)
        {
            Number = Number.ForcePositive();
            if (Number == 0) return 0;
            int fact = Number;
            int counter = Number - 1;
            while (counter > 0)
            {
                fact = fact * counter;
                counter = counter - 1;
            }

            return fact;
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
            decimal total = Dic.Sum(x => x.Value.ChangeType<decimal, TValue>());
            return Dic.Select(x => new KeyValuePair<TKey, decimal>(x.Key, x.Value.ChangeType<decimal, TValue>().CalculatePercent(total))).ToDictionary();
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
        public static Dictionary<Tobject, decimal> CalculatePercent<Tobject, Tvalue>(this IEnumerable<Tobject> Obj, Expression<Func<Tobject, Tvalue>> ValueSelector) where Tvalue : struct => Obj.CalculatePercent(x => x, ValueSelector);

        /// <summary>
        /// Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
        /// </summary>
        public static Dictionary<TValue, decimal> CalculatePercent<TValue>(this IEnumerable<TValue> Obj) where TValue : struct => Obj.DistinctCount().CalculatePercent();

        /// <summary>
        /// Calcula a porcentagem a partir da quantidade de valores verdadeiros em uma lista
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Obj"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static decimal CalculatePercentCompletion<TValue>(this IEnumerable<TValue> Obj, Expression<Func<TValue, bool>> selector)
        {
            var total = Obj.Count();
            var part = Obj.Count(selector.Compile());
            return CalculatePercent(part.ToDecimal(), total.ToDecimal());
        }

        /// <summary>
        /// Calcula a variação percentual entre 2 valores
        /// </summary>
        /// <param name="StartValue"></param>
        /// <param name="EndValue"></param>
        /// <returns></returns>
        public static decimal CalculatePercentVariation(this decimal StartValue, decimal EndValue)
        {
            if (StartValue == 0m)
            {
                if (EndValue > 0m)
                {
                    return 100m;
                }
                else
                {
                    return 0m;
                }
            }

            return (EndValue / StartValue - 1m) * 100m;
        }

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
        /// Retorna o percentual de um valor
        /// </summary>
        /// <param name="Value">Valor a ser calculado</param>
        /// <param name="Total">Valor Total (Representa 100%)</param>
        /// <returns>Um numero decimal contendo a porcentagem</returns>

        public static decimal CalculatePercent(this decimal Value, decimal Total) => Total > 0 ? Convert.ToDecimal(100m * Value / Total) : 0;

        /// <summary>
        /// Retorna o percentual de um valor
        /// </summary>
        /// <param name="Value">Valor a ser calculado</param>
        /// <param name="Total">Valor Total (Representa 100%)</param>
        /// <returns>Um numero decimal contendo a porcentagem</returns>

        public static decimal CalculatePercent(this decimal Value, decimal Total, int DecimalPlaces) => CalculatePercent(Value, Total).RoundDecimal(DecimalPlaces);

        /// <summary>
        /// Retorna o valor de um determinado percentual de um valor total
        /// </summary>
        /// <param name="Percent">
        /// Porcentagem, pode ser um numero ou uma string com o sinal de porcento. Ex.: 15 ou 15%
        /// </param>
        /// <param name="Total">Valor Total (Representa 100%)</param>
        /// <returns>Um numero decimal contendo o valor relativo a porcentagem</returns>

        public static decimal CalculateValueFromPercent(this string Percent, decimal Total) => Convert.ToDecimal(Convert.ToDecimal(Percent.Replace("%", "")) * Total / 100m);

        /// <summary>
        /// Retorna o valor de um determinado percentual de um valor total
        /// </summary>
        /// <param name="Percent">
        /// Porcentagem, pode ser um numero ou uma string com o sinal de porcento. Ex.: 15 ou 15%
        /// </param>
        /// <param name="Total">Valor Total (Representa 100%)</param>
        /// <returns>Um numero decimal contendo o valor relativo a porcentagem</returns>

        public static decimal CalculateValueFromPercent(this int Percent, decimal Total) => Convert.ToDecimal(Convert.ToDecimal(Percent * Total / 100m));

        /// <summary>
        /// Retorna o valor de um determinado percentual de um valor total
        /// </summary>
        /// <param name="Percent">
        /// Porcentagem, pode ser um numero ou uma string com o sinal de porcento. Ex.: 15 ou 15%
        /// </param>
        /// <param name="Total">Valor Total (Representa 100%)</param>
        /// <returns>Um numero decimal contendo o valor relativo a porcentagem</returns>

        public static decimal CalculateValueFromPercent(this decimal Percent, decimal Total) => Convert.ToDecimal(Convert.ToDecimal(Percent * Total / 100m));

        /// <summary>
        /// Retorna um numero inteiro representando a parte decimal de um numero decimal
        /// </summary>
        /// <param name="Value">Valor decimal</param>
        /// <returns></returns>
        public static long GetDecimalPlaces(this decimal Value, int DecimalPlaces = 0)
        {
            if (Value < 0m)
                Value = -Value;
            Value = Value - Value.Floor();
            while (Value.HasDecimalPart())
                Value = Value * 10m;
            if (DecimalPlaces > 0)
            {
                Value.ToString().GetFirstChars(DecimalPlaces).ToLong();
            }

            return Value.ToLong();
        }

        public static bool IsWholeNumber(this decimal Number) => !Number.HasDecimalPart();

        public static bool IsWholeNumber(this double Number) => !Number.HasDecimalPart();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>

        public static decimal Ceil(this decimal Number)
        {
            try
            {
                return Math.Ceiling(Number);
            }
            catch
            {
                return 0m;
            }
        }

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>

        public static double Ceil(this double Number)
        {
            try
            {
                return Math.Ceiling(Number);
            }
            catch
            {
                return 0d;
            }
        }

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static decimal Floor(this decimal Number)
        {
            try
            {
                return Math.Floor(Number);
            }
            catch
            {
                return 0m;
            }
        }

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static double Floor(this double Number)
        {
            try
            {
                return Math.Floor(Number);
            }
            catch
            {
                return 0d;
            }
        }

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long FloorLong(this double Number) => Number.Floor().ToLong();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long CeilLong(this double Number) => Number.Ceil().ToLong();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long FloorLong(this decimal Number) => Number.Floor().ToLong();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long CeilLong(this decimal Number) => Number.Ceil().ToLong();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int FloorInt(this double Number) => Number.Floor().ToInt();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int CeilInt(this double Number) => Number.Ceil().ToInt();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int FloorInt(this decimal Number) => Number.Floor().ToInt();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int CeilInt(this decimal Number) => Number.Ceil().ToInt();

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
                Number = Number.IsLessThan(MaxValue.ChangeType<T>()) ? Number : MaxValue.ChangeType<T>();
            }

            if (MinValue != null)
            {
                Number = Number.IsGreaterThan(MinValue.ChangeType<T>()) ? Number : MinValue.ChangeType<T>();
            }

            return (T)Number;
        }

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

        public static int LimitIndex<AnyType>(this int Int, IEnumerable<AnyType> Collection) => Int.LimitRange<int>(0, Collection.Count() - 1);

        public static long LimitIndex<AnyType>(this long Lng, IEnumerable<AnyType> Collection) => Lng.LimitRange<int>(0, Collection.LongCount() - 1L);

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static decimal RoundDecimal(this decimal Number, int? Decimals = default) => Convert.ToBoolean(Decimals) ? Math.Round(Number, Decimals.Value) : Math.Round(Number);

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static double RoundDouble(this double Number, int? Decimals = default) => Convert.ToBoolean(Decimals) ? Math.Round(Number, Decimals.Value) : Math.Round(Number);

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
        public static int Average(params int[] Values) => (int)Math.Round(Values.Average());

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static long Average(params long[] Values) => (long)Math.Round(Values.Average());

        /// <summary>
        /// COnverte graus para radianos
        /// </summary>
        /// <param name="Degrees"></param>
        /// <returns></returns>
        public static double ToRadians(this double Degrees) => Degrees * Math.PI / 180.0d;

        /// <summary>
        /// Calcula a distancia entre 2 locais
        /// </summary>
        /// <param name="FirstLocation">Primeiro Local</param>
        /// <param name="SecondLocation">Segundo Local</param>
        /// <returns>A distancia em kilometros</returns>
        public static double CalculateDistance(this AddressInfo FirstLocation, AddressInfo SecondLocation)
        {
            double circumference = 40000.0d;
            // Earth's circumference at the equator in km

            double distance = 0.0d;
            if (FirstLocation.Latitude == SecondLocation.Latitude == true && FirstLocation.Longitude == SecondLocation.Longitude)
            {
                return distance;
            }

            // Calculate radians
            double latitude1Rad = ((double)FirstLocation.Latitude).ToRadians();
            double longitude1Rad = ((double)FirstLocation.Longitude).ToRadians();
            double latitude2Rad = ((double)SecondLocation.Latitude).ToRadians();
            double longitude2Rad = ((double)SecondLocation.Longitude).ToRadians();
            double longitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);
            if (longitudeDiff > Math.PI)
            {
                longitudeDiff = 2.0d * Math.PI - longitudeDiff;
            }

            double angleCalculation = Math.Acos(Math.Sin(latitude2Rad) * Math.Sin(latitude1Rad) + Math.Cos(latitude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(longitudeDiff));
            distance = circumference * angleCalculation / (2.0d * Math.PI);
            return distance;
        }

        /// <summary>
        /// Calcula a distancia passando por todos os pontos
        /// </summary>
        /// <param name="Locations">Localizacoes</param>
        /// <returns></returns>
        public static Tuple<AddressInfo, AddressInfo, decimal> CalculateDistanceMatrix(params AddressInfo[] Locations) => (Tuple<AddressInfo, AddressInfo, decimal>)CartesianProduct(Locations, Locations).Select(x => new Tuple<AddressInfo, AddressInfo, decimal>(x.First(), x.Last(), (decimal)x.First().CalculateDistance(x.Last())));
    }
}