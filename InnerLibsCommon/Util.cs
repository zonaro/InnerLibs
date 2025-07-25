﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Extensions;
using Extensions.Colors;
using Extensions.ComplexText;
using Extensions.Console;
using Extensions.Converters;
using Extensions.DataBases;
using Extensions.Dates;
using Extensions.DebugWriter;
using Extensions.Equations;
using Extensions.Files;
using Extensions.Locations;
using Extensions.Pagination;
using Extensions.Web;
using Expression = System.Linq.Expressions.Expression;

namespace Extensions
{
    public static partial class Util
    {
        private const int ERROR_LOCK_VIOLATION = 33;

        private const int ERROR_SHARING_VIOLATION = 32;



        private static readonly MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        private static readonly MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        private static readonly MethodInfo equalMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });

        private static readonly Random init_rnd = new Random();

        private static readonly Expression<Func<string, bool>>[] passwordValidations = new Expression<Func<string, bool>>[]
                {
                    x => x.ToUpperInvariant().ToArray().Distinct().Count() >= 4,
                    x => x.ToUpperInvariant().ToArray().Distinct().Count() >= 6,
                    x => x.Length >= 8,
                    x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.PasswordSpecialChars.ToArray()),
                    x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.NumberChars.ToArray()),
                    x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.AlphaUpperChars.ToArray()),
                    x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.AlphaLowerChars.ToArray()),
                    x => x.CountSequentialCharacters() <=3
                };

        private static readonly MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });

        /// <summary>
        /// Gets all constants defined in the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetConstants(this Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly);
        }

               /// <summary>
        /// Gets all constants defined in the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetStringConstants(this Type type)
        {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);

            return fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                             .Select(fi => (string)fi.GetValue(null));
        }

        /// <summary>
        /// Splits the given text into parts based on a character limit specified by an array. Ensures sentences are not split,
        /// and also forces splitting at line breaks when present.
        /// </summary>
        /// <param name="text">The input text to be split.</param>
        /// <param name="limits">An array specifying the maximum number of characters allowed in each part.</param>
        /// <param name="delimiters">Optional delimiters for identifying sentences.</param>
        /// <returns>A list of text parts that comply with the specified character limits.</returns>
        /// <remarks>
        /// Sentences are identified using the delimiters '.', '!', '?', and ';' by default. You can specify the delimiters by passing <paramref name="delimiters"/>. Line breaks ('\n') will
        /// force splitting even if sentences fit within the character limit.
        /// </remarks>
        public static IEnumerable<string> SplitSentencesByLimit(this string text, int[] limits, params char[] delimiters)
        {
            limits = (limits ?? Array.Empty<int>()).Where(x => x > 0).ToArray();

            if (limits.Length == 0) return new[] { text };

            delimiters = delimiters ?? Array.Empty<char>();
            if (delimiters.IsNullOrEmpty())
            {
                delimiters = PredefinedArrays.EndOfSentencePunctuation.Union(new[] { ";" }).SelectJoinString().ToArray();
            }

            List<string> partes = new List<string>();
            string[] linhas = text.SplitAny(PredefinedArrays.BreakLineChars);


            int limitIndex = 0;

            foreach (string linha in linhas)
            {
                string parteAtual = "";
                string[] frases = linha.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                foreach (string frase in frases)
                {
                    string fraseTrim = frase.Trim();
                    int indexDelim = text.IndexOf(fraseTrim) + fraseTrim.Length;
                    char delimitador = text.ToCharArray().IfNoIndex(indexDelim, ' ');

                    string fraseComDelim = fraseTrim + delimitador + (delimitador == '\n' ? "" : " ");
                    int limit = limits[limitIndex % limits.Length]; // Cicla o array de limites



                    if (parteAtual.Length + fraseComDelim.Length <= limit)
                    {
                        parteAtual += fraseComDelim;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(parteAtual))
                        {
                            partes.Add(parteAtual.Trim());
                        }

                        parteAtual = fraseComDelim;
                        limitIndex++; // Move para o próximo limite
                    }
                }

                if (!string.IsNullOrEmpty(parteAtual))
                {
                    partes.Add(parteAtual.Trim());
                    limitIndex++; // Move para o próximo limite para a próxima linha
                }
            }

            return partes;
        }


        /// <summary>
        /// Splits the given text into parts based on a character limit. Ensures sentences are not split,
        /// and also forces splitting at line breaks when present.
        /// </summary>
        /// <param name="text">The input text to be split.</param>
        /// <param name="limit">The maximum number of characters allowed in each part.</param>
        /// <returns>A list of text parts that comply with the specified character limit.</returns>
        /// <example>
        /// var parts = "This is a test. Another test!\nNew line here.".SplitSentencesByLimit(20);
        /// foreach (var part in parts)
        /// {
        ///     Console.WriteLine(part);
        /// }
        /// </example>
        /// <remarks>
        /// Sentences are identified using the delimiters '.', '!', '?', and ';' by default. You can specify the delimiters by passing <paramref name="delimiters"/>. Line breaks ('\n') will
        /// force splitting even if sentences fit within the character limit.
        /// </remarks>
        public static IEnumerable<string> SplitSentencesByLimit(this string text, int limit, params char[] delimiters)
        {
            if (text.IsBlank()) return Array.Empty<string>();
            if (limit <= 0) limit = text.Length;
            return SplitSentencesByLimit(text, new int[] { limit }, delimiters);
        }

        public static int CountSequentialCharacters(this string input)
        {
            if (input.IsBlank()) return 0;
            int somaSequencias = 0;
            int sequenciaAtual = 1;

            for (int i = 1; i < input.Length; i++)
            {
                if (input[i] == input[i - 1] + 1) // Verifica se o caractere atual é sequencial
                {
                    sequenciaAtual++;
                }
                else
                {
                    somaSequencias += sequenciaAtual; // Soma o comprimento da sequência atual
                    sequenciaAtual = 1; // Reinicia para uma nova sequência
                }
            }

            // Adiciona a última sequência à soma
            somaSequencias += sequenciaAtual;

            return somaSequencias;

        }

        /// <summary>
        /// Generates an avatar image based on the provided name.
        /// </summary>
        /// <param name="Name">The name to generate the avatar from.</param>
        /// <param name="Size">The size of the avatar image.</param>
        /// <param name="maxLenght">The maximum length of the initials.</param>
        /// <param name="circle">Whether the avatar should be circular.</param>
        /// <returns>An image representing the avatar.</returns>
        public static Image GenerateAvatarByName(this string Name, string Size = "", int maxLenght = 3, bool circle = true)
        {
            Color? color = null;
            if (Name.IsBlank())
            {
                color = RandomColor();
            }
            else
            {
                color = Name.ToColor();
            }

            color = color?.MakeLighter();

            var fontColor = color?.GetContrastColor();

            var img = color?.CreateSolidImage(Size);

            if (Name.IsNotBlank())
            {
                if (Name.Contains(" "))
                {
                    List<string> parts = new List<string>();
                    parts = Name.GetInitials().ToList();
                    maxLenght = maxLenght.LimitRange(1, parts.Count);
                    if (maxLenght == 1)
                    {
                        Name = parts.FirstOrDefault() ?? "";
                    }
                    else
                    {

                        Name = $"{parts.Detach(0)}{parts.TakeLast(maxLenght - 1).SelectJoinString()}";
                    }
                }
                else
                {
                    if (Name.Length > 3)
                    {
                        // manter somente a primeira letra e as consoantes que definem a pronuncia,
                        // por exemplo Evento vira Evnt (maxChar 4) ou Evt se maxChar for 3

                        var firstChar = Name.GetFirstChars(1).ToUpperInvariant();
                        var otherConsonants = Name.GetLastChars(Name.Length - 1).Where(c => c.ToString().IsAny(PredefinedArrays.Consonants.ToArray()));

                        Name = firstChar + otherConsonants.SelectJoinString().ToUpperInvariant();
                        while (Name.Length > maxLenght)
                        {
                            Name = Name.GetFirstChars(-1).Remove(Name.Length - 1);
                        }



                    }
                    Name = Name.GetFirstChars(maxLenght).ToTitle();
                }

                float size = (float)(img.Width * .18);
                img = img.DrawString(Name, new Font("Arial", size, FontStyle.Bold), fontColor);
            }

            if (circle)
            {
                img = img?.CropToCircle();
            }

            return img;

        }

        /// <summary>
        /// Gets the initials of each word in the given text.
        /// </summary>
        /// <param name="Text">The text to get initials from.</param>
        /// <returns>An enumerable of initials from the text.</returns>
        public static IEnumerable<string> GetInitials(this string Text) => Text?.SplitAny(PredefinedArrays.WordSplitters).Select(x => x.GetFirstChars().ToUpper()) ?? Array.Empty<string>();

        /// <summary>
        /// Gets the initials of each word in the given text.
        /// </summary>
        /// <param name="Text">The text to get initials from.</param>
        /// <returns>An string of initials from the text.</returns>
        public static string GetInitialsString(this string Text) => Text.GetInitials().SelectJoinString();

        /// <summary>
        /// Check if one File is a copy of another file
        /// </summary>
        /// <param name="file1"></param>
        /// <param name="file2"></param>
        /// <returns></returns>
        public static bool IsCopyOf(FileInfo file1, FileInfo file2)
        {
            // verifica se os arquivos existem, se possuem o mesmo tamanho e se não são o mesmo arquivo (o mesmo arquivo não pode ser copia dele mesmo)
            if (!file1.Exists || !file2.Exists || file1.Length != file2.Length || file1.FullName == file2.FullName)
                return false;

            using (var sha256 = SHA256.Create())
            {
                using (var stream1 = file1.OpenRead())
                using (var stream2 = file2.OpenRead())
                {
                    byte[] hash1 = sha256.ComputeHash(stream1);
                    byte[] hash2 = sha256.ComputeHash(stream2);

                    for (int i = 0; i < hash1.Length; i++)
                    {
                        if (hash1[i] != hash2[i])
                            return false;
                    }

                    return true;
                }
            }
        }

        /// <summary>
        /// Orders the elements of a sequence according to the specified indexes.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the sequence.</typeparam>
        /// <param name="list">The sequence of elements to order.</param>
        /// <param name="indexes">The indexes to order the elements by.</param>
        /// <returns>An ordered sequence of elements.</returns>
        public static IEnumerable<T> OrderByIndexes<T>(this IEnumerable<T> list, params int[] indexes)
        {
            if (indexes != null && indexes.Length > 0 && list != null && list.Any())
            {
                var l = list.ToList();
                List<T> orderedList = indexes.Where(index => index < list.Count()).Select(index => l[index]).ToList();

                if (list.Count() > indexes.Length)
                {
                    orderedList.AddRange(list.Where((item, index) => !indexes.Contains(index)));
                }

                return orderedList.AsEnumerable();
            }
            return list.AsEnumerable();
        }

        /// <summary>
        /// Inject a <see cref="Hashtable"/> into <see cref="String"/>
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="attributes"></param>  
        /// <returns></returns>
        private static string InjectBase(string formatString, Hashtable attributes)
        {
            string result = formatString;
            if (attributes != null && formatString != null)
            {
                foreach (string attributeKey in attributes.Keys)
                {
                    result = result.InjectSingleValue(attributeKey, attributes[attributeKey]);
                }
            }

            return result;
        }

        /// <summary>
        /// Replace te found <paramref name="key"/> with <paramref name="replacementValue"/>
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="key"></param>
        /// <param name="replacementValue"></param>
        /// <returns></returns>
        public static string InjectSingleValue(this string formatString, string key, object replacementValue, CultureInfo cultureInfo = null)
        {
            string result = formatString ?? "";
            var attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");
            foreach (Match m in attributeRegex.Matches(formatString))
            {
                string replacement = m.ToString();
                if (m.Groups[2].Length > 0)
                {
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
                    replacement = string.Format(cultureInfo ?? CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
                }
                else
                {
                    replacement = (replacementValue ?? default).ToString();
                }



                result = result.Replace(m.ToString(), replacement);
            }

            return result;
        }

        /// <summary>
        /// Represents a double quote character.
        /// </summary>
        public const string DoubleQuoteChar = "\"";

        /// <summary>
        /// Earth's circumference at the equator in km, considering the earth is a globe, not flat
        /// </summary>
        public const double EarthCircumference = 40075d;

        public const string EmptyString = "";

        public const string SingleQuoteChar = "\'";

        public const string TabChar = "\t";

        public const string WhitespaceChar = " ";



        /// <summary>
        /// Set this flag to true to show InnerLibs Debug messages (<see cref="WriteDebug"/>)
        /// </summary>
        public static bool EnableDebugMessages { get; set; }


        /// <summary>
        /// Lista com todos os formatos de imagem
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ImageFormat> ImageTypes { get; private set; } = new[] { ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Exif, ImageFormat.Gif, ImageFormat.Icon, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff, ImageFormat.Wmf }.AsEnumerable();

        /// <summary>
        /// Retorna uma lista com todas as <see cref="KnowColor"/> convertidas em <see cref="Color"/>
        /// </summary>
        public static IEnumerable<Color> KnowColors => GetEnumValues<KnownColor>().Select(x => Color.FromKnownColor(x));

        /// <summary>
        /// Gets a collection of predefined HSV (Hue, Saturation, Value) colors.
        /// </summary>
        /// <remarks>The collection is derived from the known colors defined in the <see
        /// cref="KnownColor"/> enumeration. Each color is converted to its HSV representation.</remarks>
        public static IEnumerable<HSVColor> KnownHSVColors => GetEnumValues<KnownColor>().Select(x => HSVColor.FromKnownColor(x));

        /// <summary>
        /// Quando Configurado, escreve os parametros e queries executadas no <see
        /// cref="TextWriter"/> específico
        /// </summary>
        /// <returns></returns>
        public static TextWriter LogWriter { get; set; } = new DebugTextWriter();

        /// <summary>
        /// Retorna o ano atual
        /// </summary>
        public static int ThisYear => DateTime.Now.Year;
        /// <summary>
        /// Gets the current month as an integer, where January is 1 and December is 12.
        /// </summary>
        public static int ThisMonth => DateTime.Now.Month;

        /// <summary>
        /// Generates a sequence of mid-ranges between a start and end value, divided into a
        /// specified number of ranges.
        /// </summary>
        /// <typeparam name="T">The type of the start and end values.</typeparam>
        /// <param name="ranges">The number of ranges to generate.</param>
        /// <param name="start">The start value.</param>
        /// <param name="end">The end value.</param>
        /// <returns>
        /// A sequence of tuples representing the mid-ranges between the start and end values.
        /// </returns>
        public static IEnumerable<(T, T)> GetMidRanges<T>(this int ranges, T start, T end) where T : IComparable
        {
            var startDecimal = start.ToDecimal();
            var endDecimal = end.ToDecimal();
            FixOrder(ref startDecimal, ref endDecimal);
            var rangeSize = (endDecimal - startDecimal) / ranges;
            var rangeStart = startDecimal;
            for (int i = 0; i < ranges; i++)
            {
                var rangeEnd = rangeStart + rangeSize;
                if (i == ranges - 1)
                {
                    rangeEnd = endDecimal;
                }
                yield return (rangeStart.ChangeType<T>(), rangeEnd.ChangeType<T>());
                rangeStart = rangeEnd + 1;
            }
        }

        /// <summary>
        /// Adciona um parametro a QueryInterpolated String de uma URL
        /// </summary>
        /// <param name="Url">Uri</param>
        /// <param name="Key">Nome do parâmetro</param>
        /// <param name="Values">Valor do Parâmetro</param>
        /// <returns></returns>
        public static Uri AddParameter(this Uri Url, string Key, bool Append, params string[] Values)
        {
            var UriBuilder = new UriBuilder(Url);
            var query = UriBuilder.Query.ParseQueryString();
            if (Values is null || Append == false)
            {
                query = query ?? new NameValueCollection();
                if (query.AllKeys.Contains(Key))
                {
                    query.Remove(Key);
                }
            }

            foreach (var v in Values ?? Array.Empty<string>())
            {
                query.Add(Key, v);
            }

            UriBuilder.Query = query.ToString();
            Url = UriBuilder.Uri;
            return Url;
        }

        /// <summary>
        /// Adciona um parametro a QueryInterpolated String de uma URL
        /// </summary>
        /// <param name="Url">Uri</param>
        /// <param name="Key">Nome do parâmetro</param>
        /// <param name="Values">Valor do Parâmetro</param>
        /// <returns></returns>
        public static Uri AddParameter(this Uri Url, string Key, params string[] Values) => Url.AddParameter(Key, true, Values);

        /// <summary>
        /// Extends the <see cref="Uri"/> class to support scroll-to-text fragments,
        /// enabling the highlighting of specific text on supported browsers like Chrome.
        /// </summary>
        /// <param name="uri">The base <see cref="Uri"/> where the text will be highlighted.</param>
        /// <param name="startText">
        /// The beginning of the text fragment to be highlighted.
        /// This must match text exactly as it appears on the target web page.
        /// </param>
        /// <param name="endText">
        /// The end of the text fragment to be highlighted.
        /// The browser will highlight text between <paramref name="startText"/> and <paramref name="endText"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="Uri"/> instance including the scroll-to-text fragment appended to the original URL.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="uri"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="startText"/> or <paramref name="endText"/> is <c>null</c>, empty, or whitespace.
        /// </exception>
        /// <remarks>
        /// This method constructs a URL with the format: 
        /// <c>#~:text=startText,endText</c>
        /// which is supported in Chrome and Chromium-based browsers.
        /// </remarks>
        /// <example>
        /// <code>
        /// Uri article = new Uri("https://example.com/page");
        /// Uri highlight = article.WithHighlightedText("Introduction", "Conclusion");
        /// Console.WriteLine(highlight);
        /// // Output: https://example.com/page#:~:text=Introduction,Conclusion
        /// </code>
        /// </example>
        public static Uri AddHighlightedText(this Uri uri, string text, string finalText = null)
        {
            if (uri == null) return null;

            finalText = finalText ?? string.Empty;
            text = text ?? string.Empty;

            var b = new UriBuilder(uri);

            if (finalText.IsNotBlank() && text.IsBlank())
            {
                text = finalText;
                finalText = string.Empty;
            }


            if (text.IsNotBlank())
            {
                // Codifica o texto para uso seguro em URL
                text = Uri.EscapeDataString(text);
                finalText = Uri.EscapeDataString(finalText);

                if (finalText.IsNotBlank())
                    b.Fragment = $"#:~:text={Uri.EscapeDataString(text)},{Uri.EscapeDataString(finalText)}";
                else
                    b.Fragment = $"#:~:text={Uri.EscapeDataString(text)}";
            }


            return b.Uri;
        }


        /// <summary>
        /// Retorna TRUE se a todos os testes em uma lista retornarem FALSE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool AllFalse(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).All(x => x == false);

        /// <summary>
        /// Retorna TRUE se a todos os testes em uma lista retornarem TRUE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool AllTrue(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).All(x => x == true);

        /// <summary>
        /// Retorna uma string em ordem afabética baseada em uma outra string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Alphabetize(this string Text)
        {
            var a = Text.IfBlank(EmptyString).ToCharArray();
            Array.Sort(a);
            return a.SelectJoinString(EmptyString);
        }

        /// <summary> Concatena uma expressão com outra usando o operador And (&&) </summary>
        /// <typeparam name="T"></typeparam> <param name="FirstExpression"></param> <param
        /// name="OtherExpressions"></param> <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> FirstExpression, params Expression<Func<T, bool>>[] OtherExpressions)
        {
            FirstExpression = FirstExpression ?? true.CreateWhereExpression<T>();
            foreach (var item in OtherExpressions ?? Array.Empty<Expression<Func<T, bool>>>())
            {
                if (item != null)
                {
                    var invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast<Expression>());
                    FirstExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(FirstExpression.Body, invokedExpr), FirstExpression.Parameters);
                }
            }

            return FirstExpression;
        }

        public static Expression<Func<T, bool>> AndSearch<T>(this Expression<Func<T, bool>> FirstExpression, IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? true.CreateWhereExpression<T>()).And(Text.SearchExpression(Properties));

        public static Expression<Func<T, bool>> AndSearch<T>(this Expression<Func<T, bool>> FirstExpression, string Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? true.CreateWhereExpression<T>()).And(Text.SearchExpression(Properties));

        /// <summary>
        /// Adiciona texto ao fim de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string Append(this string Text, string AppendText)
        {
            Text = Text ?? string.Empty;
            AppendText = AppendText ?? string.Empty;
            Text += AppendText;
            return Text;
        }

        /// <summary>
        /// Adiciona um digito verificador calulado com Mod10 ao <paramref name="Code"/>
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static string AppendBarcodeCheckSum(this string Code) => Code.Append(GenerateBarcodeCheckSum(Code));

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendIf(this string Text, string AppendText, bool Test)
        {
            Text = Text ?? string.Empty;
            AppendText = AppendText ?? string.Empty;
            return Test ? Text.Append(AppendText) : Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendIf(this string Text, string AppendText, Func<string, bool> Test) => AppendIf(Text, AppendText, (Test ?? (x => false))(Text));

        /// <summary>
        /// Adiciona texto ao final de uma string com uma quebra de linha no final do <paramref name="AppendText"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string AppendLine(this string Text, string AppendText) => Text.Append(AppendText).Append(Environment.NewLine);

        /// <summary>
        /// Adiciona texto ao final de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendWhile(this string Text, string AppendText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);
            while (Test(Text))
            {
                Text = Text.Append(AppendText);
            }

            return Text;
        }

        /// <summary>
        /// Applies a color matrix to an image.
        /// </summary>
        /// <param name="img">The source image.</param>
        /// <param name="cm">The color matrix to apply.</param>
        /// <returns>A new image with the color matrix applied, or null if an error occurs.</returns>
        public static Image ApplyColorMatrix(Image img, ColorMatrix cm)
        {
            try
            {
                var bmp = new Bitmap(img); // create a copy of the source image
                var imgattr = new ImageAttributes();
                var rc = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
                var g = Graphics.FromImage(img);

                // associate the ColorMatrix object with an ImageAttributes object
                imgattr.SetColorMatrix(cm);

                // draw the copy of the source image back over the original image, applying the ColorMatrix
                g.DrawImage(bmp, rc, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgattr);
                g.Dispose();
                imgattr.Dispose();
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Aplica espacos em todos os caracteres de encapsulamento
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ApplySpaceOnWrapChars(this string Text)
        {
            Text = Text ?? EmptyString;
            foreach (var c in PredefinedArrays.WordWrappers)
            {
                Text = Text.Replace(c, WhitespaceChar + c + WhitespaceChar);
            }

            return Text;
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
        /// Converts an object to a nullable boolean.
        /// </summary>
        /// <param name="Value">The object to be converted.</param>
        /// <param name="defaultReturn">
        /// A flag indicating whether to return true for any value not explicitly mapped to false.
        /// Defaults to true.
        /// </param>
        /// <returns>
        /// A nullable boolean representing the converted value of the object. The following string
        /// values are considered:
        /// - "NULL", "CANCEL", "CANCELAR": Returns null.
        /// - "", "!", "0", "FALSE", "NOT", "NAO", "NO", "NOP", "DISABLED", "DISABLE", "OFF",
        ///   "DESATIVADO", "DESATIVAR", "DESATIVO", "N": Returns false.
        /// - "1", "OK", "TRUE", "YES", "YEP", "SIM", "ENABLED", "ENABLE", "ON", "Y", "ATIVO",
        ///   "ATIVAR", "ATIVADO": Returns true. Any other value: Returns true if EverythingIsTrue
        /// is set to true, otherwise throws an ArgumentException.
        /// </returns>

        public static bool? AsNullableBool(this object Value, bool? defaultReturn = null)
        {
            if (Value == null) return null;
            else switch ($"{Value}".ToUpperInvariant().RemoveDiacritics())
                {
                    case "NULL":
                    case "CANCEL":
                    case "CANCELAR":
                        return null;

                    case "":
                    case "!":
                    case "0":
                    case "FALSE":
                    case "NOT":
                    case "NAO":
                    case "NO":
                    case "NOP":
                    case "DISABLED":
                    case "DISABLE":
                    case "OFF":
                    case "DESATIVADO":
                    case "DESATIVAR":
                    case "DESATIVO":
                    case "N":
                        return false;

                    case "1":
                    case "OK":
                    case "TRUE":
                    case "YES":
                    case "YEP":
                    case "SIM":
                    case "S":
                    case "ENABLED":
                    case "ENABLE":
                    case "ON":
                    case "Y":
                    case "ATIVO":
                    case "ATIVAR":
                    case "ATIVADO":
                        return true;

                    default:
                        return defaultReturn;
                }
        }

        /// <summary>
        /// Converts an object to a boolean.
        /// </summary>
        /// <param name="Value">The object to be converted.</param>

        /// A flag indicating whether to return true for any value not explicitly mapped to false.
        /// Defaults to true.
        /// </param>
        /// <returns>
        /// A boolean representing the converted value of the object. The following string values
        /// are considered:
        /// - "NULL", "CANCEL", "CANCELAR": Returns false as the function is non-nullable.
        /// - "", "!", "0", "FALSE", "NOT", "NAO", "NO", "NOP", "DISABLED", "DISABLE", "OFF",
        ///   "DESATIVADO", "DESATIVAR", "DESATIVO", "N": Returns false.
        /// - "1", "OK", "TRUE", "YES", "YEP", "SIM", "ENABLED", "ENABLE", "ON", "Y", "ATIVO",
        ///   "ATIVAR", "ATIVADO": Returns true. Any other value: Returns true if EverythingIsTrue
        /// is set to true, otherwise returns false.
        /// </returns>
        public static bool AsBool(this object Value, bool defaultReturn = true) => AsNullableBool(Value, defaultReturn) ?? defaultReturn;

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="BoolExp">Expressão de teste de Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static TR AsIf<T, TR>(this T obj, Expression<Func<T, bool>> BoolExp, TR TrueValue, TR FalseValue = default) => obj == null || BoolExp == null ? FalseValue : BoolExp.Compile().Invoke(obj).AsIf(TrueValue, FalseValue);
        public static TR AsIf<TR>(this string obj, TR TrueValue, TR FalseValue = default) => AsIf<string, TR>(obj, x => x.AsBool(true), TrueValue, FalseValue);

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool Bool, T TrueValue, T FalseValue = default) => Bool ? TrueValue : FalseValue;

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool? Bool, T TrueValue, T FalseValue = default) => (Bool.HasValue && Bool.Value).AsIf(TrueValue, FalseValue);

        /// <inheritdoc cref="AsIf{T}(bool?, T, T)"/>
        public static T AsIf<T>(this bool? Bool, T TrueValue, T FalseValue, T NullValue)
        {
            if (Bool.HasValue)
                return Bool.Value.AsIf(TrueValue, FalseValue);
            else
                return NullValue;
        }



        /// <summary>
        /// Decoda uma string em Base64
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        public static string Atob(this string Base, Encoding Encoding = null)
        {
            if (Base.IsValid())
            {
                Base = (Encoding ?? new UTF8Encoding(false)).GetString(Convert.FromBase64String(Base));
            }

            return Base;
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
        /// Converte uma DATAURL ou Base64 String em um array de Bytes
        /// </summary>
        /// <param name="Base64StringOrDataURL">Base64 String ou DataURL</param>
        /// <returns></returns>
        public static byte[] Base64ToBytes(this string Base64StringOrDataURL) => Convert.FromBase64String(Base64StringOrDataURL.FixBase64());

        /// <summary>
        /// Cria um arquivo fisico a partir de uma Base64 ou DataURL
        /// </summary>
        /// <param name="Base64StringOrDataURL"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static FileInfo Base64ToFile(this string Base64StringOrDataURL, string FilePath) => Base64StringOrDataURL.Base64ToBytes().WriteToFile(FilePath);

        public static Image Base64ToImage(this string DataUrlOrBase64String, int Width = 0, int Height = 0)
        {
            try
            {
                if (DataUrlOrBase64String.IsNotValid())
                {
                    return null;
                }

                if (DataUrlOrBase64String.Contains(","))
                {
                    DataUrlOrBase64String = DataUrlOrBase64String.GetAfter(",");
                }

                var imageBytes = Base64ToBytes(DataUrlOrBase64String);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    if (Width > 0 && Height > 0)
                    {
                        return Image.FromStream(ms, true).Resize(Width, Height, false);
                    }
                    else
                    {
                        return Image.FromStream(ms, true);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Invalid Base64 or DataURL string or Base64 format is not an Image", ex);
            }
        }



        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento
        /// que possuir um valor
        /// </summary>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(this string First, params string[] N) => BlankCoalesce(new[] { First }.Union(N ?? Array.Empty<string>()).ToArray());

        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento
        /// que possuir um valor
        /// </summary>
        /// <param name="N">Itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(params string[] N) => (N ?? Array.Empty<string>()).FirstOr(x => x.IsValid(), EmptyString);

        /// <summary>
        /// Aplica um borrão a imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="BlurSize"></param>
        /// <returns></returns>
        public static Image Blur(this Image Img, int BlurSize = 5) => Blur(Img, BlurSize, new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height));

        /// <summary>
        /// Aplica um borrão a uma determinada parte da imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="BlurSize"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static unsafe Image Blur(this Image Img, int BlurSize, System.Drawing.Rectangle rectangle)
        {
            Bitmap blurred = new Bitmap(Img.Width, Img.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(Img, new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height),
                    new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new System.Drawing.Rectangle(0, 0, Img.Width, Img.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // GetCliente bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

            // GetCliente pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the blur size
                    // while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + BlurSize && x < Img.Width); x++)
                    {
                        for (int y = yy; (y < yy + BlurSize && y < Img.Height); y++)
                        {
                            // GetCliente pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0]; // Blue
                            avgG += data[1]; // Green
                            avgR += data[2]; // Red

                            blurPixelCount++;
                        }
                    }

                    avgR /= blurPixelCount;
                    avgG /= blurPixelCount;
                    avgB /= blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + BlurSize && x < Img.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + BlurSize && y < Img.Height && y < rectangle.Height; y++)
                        {
                            // GetCliente pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }

        /// <summary>
        /// Remove the background of an image.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <param name="background">The background color.</param>
        /// <returns>The image with the background removed.</returns>
        public static Image RemoveBackground(this Image original, Color? background = null)
        {
            if (original != null)
            {
                Bitmap bmp = new Bitmap(original.Width, original.Height);

                if (background == null || background.Value == Color.Transparent)
                {
                    // Detect the background color automatically.
                    background = DetectBackgroundColor(original);
                }

                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        Color pixelColor = ((Bitmap)original).GetPixel(j, i);

                        if (pixelColor != background)
                        {
                            bmp.SetPixel(j, i, pixelColor);
                        }
                    }
                }

                return bmp;
            }
            return null;
        }

        /// <summary>
        /// Detects the background color of an image.
        /// </summary>
        /// <param name="original">The original image.</param>
        /// <returns>The detected background color.</returns>
        private static Color DetectBackgroundColor(this Image original)
        {
            using (Bitmap bmp = new Bitmap(original.Width, original.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(original, 0, 0);
                }

                return bmp.GetPixel(0, 0);
            }
        }

        /// <summary>
        /// Inverte as cores de uma imagem.
        /// </summary>
        /// <param name="original">A imagem original.</param>
        /// <returns>A imagem com as cores invertidas.</returns>
        public static Image InvertColors(this Image original)
        {
            if (original != null)
            {
                Bitmap bmp = new Bitmap(original.Width, original.Height);

                for (int i = 0; i < bmp.Height; i++)
                {
                    for (int j = 0; j < bmp.Width; j++)
                    {
                        Color pixelColor = ((Bitmap)original).GetPixel(j, i);
                        Color newColor = Color.FromArgb(pixelColor.A,
                                                         255 - pixelColor.R,
                                                         255 - pixelColor.G,
                                                         255 - pixelColor.B);
                        bmp.SetPixel(j, i, newColor);
                    }
                }
                return bmp;
            }
            return null;
        }

        /// <summary>
        /// Encapsula um texto em uma caixa. Funciona somente com fonte monoespaçadas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxText(this string Text, char BoxChar = '*')
        {
            var Lines = Text.SplitAny(PredefinedArrays.BreakLineChars.ToArray()).ToList();
            string linha_longa = string.Empty;
            int charcount = Lines.Max(x => x.Length);
            if (charcount.IsEven())
            {
                charcount++;
            }

            for (int i = 0, loopTo = Lines.Count - 1; i <= loopTo; i++)
            {
                Lines[i] = Lines[i].PadRight(charcount);
            }

            for (int i = 0, loopTo1 = Lines.Count - 1; i <= loopTo1; i++)
            {
                Lines[i] = $"{BoxChar} {Lines[i]} {BoxChar}";
            }

            charcount = Lines.Max(x => x.Length);
            while (linha_longa.Length < charcount)
            {
                linha_longa += $"{BoxChar} ";
            }

            linha_longa = linha_longa.Trim();
            Lines.Insert(0, linha_longa);
            Lines.Add(linha_longa);
            string box = Lines.SelectJoinString(Environment.NewLine);
            return box;
        }

        /// <summary>
        /// Encapsula um texto em uma caixa incorporado em comentários CSS
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxTextCSS(this string Text) => $"/*{Text.BoxText().Wrap(Environment.NewLine)}*/";

        /// <summary>
        /// Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou
        /// colchetes) é um alias de <see cref="Quote(String, Char)"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BracketChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Brackfy(this string Text, char BracketChar = '{') => Text.Quote(BracketChar);

        public static Image BrightnessContrastAndGamma(this Image originalimage, float Brightness, float Contrast, float Gamma)
        {
            var adjustedImage = new Bitmap(originalimage);
            Gamma = Gamma.SetMinValue(1.0f);
            Contrast = Contrast.SetMinValue(1.0f);
            float adjustedBrightness = Brightness.SetMinValue(1.0f) - 1.0f;
            var ptsArray = new[] { new float[] { Contrast, 0f, 0f, 0f, 0f }, new float[] { 0f, Contrast, 0f, 0f, 0f }, new float[] { 0f, 0f, Contrast, 0f, 0f }, new float[] { 0f, 0f, 0f, 1.0f, 0f }, new float[] { adjustedBrightness, adjustedBrightness, adjustedBrightness, 0f, 1f } };
            var imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(Gamma, ColorAdjustType.Bitmap);
            var g = Graphics.FromImage(adjustedImage);
            g.DrawImage(originalimage, new System.Drawing.Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, originalimage.Width, originalimage.Height, GraphicsUnit.Pixel, imageAttributes);
            return adjustedImage;
        }

        /// <summary>
        /// Encoda uma string em Base64
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        public static string Btoa(this string Text, Encoding Encoding = null)
        {
            if (Text.IsValid())
            {
                Text = Convert.ToBase64String((Encoding ?? new UTF8Encoding(false)).GetBytes(Text));
            }

            return Text;
        }

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

        /// <inheritdoc cref="CalculateCompoundInterest(double,double,double)"/>
        public static decimal CalculateCompoundInterest(this decimal Capital, decimal Rate, decimal Time) => CalculateCompoundInterest((double)Capital, (double)Rate, (double)Time).ToDecimal();

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

        public static decimal CalculateValueFromPercent(this string Percent, decimal Total)
        {
            Percent = Percent.FindNumbers().FirstOrDefault();
            if (Percent.IsNumber())
            {
                return Percent.ToDecimal().CalculateValueFromPercent(Total);
            }
            else throw new ArgumentException("Percent is not a number");
        }

        public static decimal CalculateValueFromPercent(this int Percent, decimal Total) => Percent.ToDecimal().CalculateValueFromPercent(Total);

        public static decimal CalculateValueFromPercent(this decimal Percent, decimal Total) => Percent / 100m * Total;

        public static string OnlyNumbers(this string Text) => Text?.ToArray().Where(x => char.IsDigit(x)).SelectJoinString() ?? "";

        public static int OnlyNumbersInt(this string Text) => Text?.OnlyNumbers().ToInt() ?? 0;

        public static long OnlyNumbersLong(this string Text) => Text?.OnlyNumbers().ToLong() ?? 0;

        /// <summary>
        /// Verifica se o valor é um numero ou pode ser convertido em numero
        /// </summary>
        /// <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
        /// <returns>TRUE se for um numero, FALSE se não for um numero</returns>
        public static bool CanBeNumber(this object Value)
        {
            try
            {
                Convert.ToDecimal(Value);
                return true;
            }
            catch
            {
                return false;
            }
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
            {
                aa.Add(item);
            }

            return aa;
        }

        public static decimal Ceil(this decimal Number) => Math.Ceiling(Number);

        public static double Ceil(this double Number) => Math.Ceiling(Number);

        public static decimal CeilDecimal(this double Number) => Number.Ceil().ToDecimal();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro</returns>
        public static decimal CeilDecimal(this decimal Number) => Number.Ceil().ToDecimal();

        public static double CeilDouble(this double Number) => Number.Ceil().ToDouble();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro</returns>
        public static double CeilDouble(this decimal Number) => Number.Ceil().ToDouble();

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

        /// <summary>
        /// Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro
        /// caractere desejado) e retorna um valor indicando se o texto precisou ser censurado
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BadWords">Lista de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        /// <returns>
        /// TRUE se a frase precisou ser censurada, FALSE se a frase não precisou de censura
        /// </returns>
        public static (string Text, bool IsCensored) Censor(this string Text, IEnumerable<string> BadWords, char CensorshipCharacter)
        {
            var words = Text.Split(WhitespaceChar, StringSplitOptions.None);
            BadWords = BadWords ?? Array.Empty<string>();
            var IsCensored = false;
            if (words.ContainsAny(BadWords))
            {
                foreach (var bad in BadWords)
                {
                    string censored = string.Empty;
                    for (int index = 1, loopTo = bad.Length; index <= loopTo; index++)
                    {
                        censored += CensorshipCharacter;
                    }

                    for (int index = 0, loopTo1 = words.Length - 1; index <= loopTo1; index++)
                    {
                        if ((words[index].RemoveDiacritics().RemoveAny(PredefinedArrays.WordSplitters.ToArray()).ToLowerInvariant() ?? string.Empty) == (bad.RemoveDiacritics().RemoveAny(PredefinedArrays.WordSplitters.ToArray()).ToLowerInvariant() ?? string.Empty))
                        {
                            words[index] = words[index].ToLowerInvariant().Replace(bad, censored);
                            IsCensored = true;
                        }
                    }
                }

                Text = words.SelectJoinString(WhitespaceChar);
            }
            return (Text, IsCensored);
        }

        /// <summary>
        /// Retorna um novo texto censurando as palavras de um texto substituindo as palavras
        /// indesejadas por um caractere desejado)
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BadWords">Array de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        public static (string Text, bool IsCensored) Censor(this string Text, char CensorshipCharacter, params string[] BadWords) => Text.Censor((BadWords ?? Array.Empty<string>()).ToList(), CensorshipCharacter);

        /// <summary>
        /// Converte um array de um ToType para outro
        /// </summary>
        /// <typeparam name="TTo">Tipo do array</typeparam>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static TTo[] ChangeArrayType<TTo, TFrom>(this TFrom[] Value) => Value.AsEnumerable().ChangeIEnumerableType<TTo, TFrom>().ToArray();

        /// <summary>
        /// Converte um array de um ToType para outro
        /// </summary>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static object[] ChangeArrayType<TFrom>(this TFrom[] Value, Type Type) => Value.ChangeIEnumerableType(Type).ToArray();

        /// <summary>
        /// Converte um IEnumerable de um ToType para outro
        /// </summary>
        /// <typeparam name="TTo">Tipo do array</typeparam>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static IEnumerable<TTo> ChangeIEnumerableType<TTo, TFrom>(this IEnumerable<TFrom> Value) => (IEnumerable<TTo>)Value.ChangeIEnumerableType(typeof(TTo));

        /// <summary>
        /// Converte um IEnumerable de um ToType para outro
        /// </summary>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static IEnumerable<object> ChangeIEnumerableType<TFrom>(this IEnumerable<TFrom> Value, Type ToType) => (Value ?? Array.Empty<TFrom>()).Select(el => el.ChangeType(ToType)).ToList().AsEnumerable();

        /// <summary>
        /// Converte um ToType para outro. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType ou null se a conversão falhar</returns>
        public static T ChangeType<T>(this object Value)
        {
            try
            {

                var v = Util.ChangeType(Value, typeof(T));
                if (v == null && typeof(T).IsSimpleType()) return default;
                return (T)v;

            }
            catch (Exception ex)
            {
                return default;
            }
        }

        /// <summary>
        /// Converte um ToType para outro. Retorna Nothing (NULL) ou DEFAULT se a conversão falhar
        /// </summary>
        /// <typeparam name="TFrom">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType ou null (ou default) se a conversão falhar</returns>
        public static object ChangeType<TFrom>(this TFrom Value, Type ToType)
        {
            if (ToType == null)
            {
                WriteDebug($"ToType is null, no conversion performed");
                return Value;
            }

            if (typeof(TFrom) == ToType)
            {
                return Value;
            }

            if (ToType.IsAssignableFrom(typeof(TFrom)))
            {
                return Value;
            }


            if ((Value == null || (ToType.IsSimpleType() && Value is string ss && ss.IsBlank()))) return default;

            WriteDebug($"Try changing from {typeof(TFrom).Name} to {ToType.Name}");

            try
            {
                var met = Value?.GetType().GetNullableTypeOf().GetMethods().FirstOrDefault(x => x.Name == $"To{ToType.Name}" && x.ReturnType == ToType && x.IsPublic && x.GetParameters().Any() == false);
                if (met != null)
                {
                    WriteDebug($"Trying internal method {met.Name}");
                    return met.Invoke(Value, Array.Empty<object>());
                }
            }
            catch
            {
            }



            try
            {
                ToType = GetNullableTypeOf(ToType);
                if (Value == null)
                {
                    WriteDebug($"Value is null");
                    if (!ToType.IsSimpleType() || ToType.IsNullableType())
                    {
                        return null;
                    }
                    else
                    {
                        return default;
                    }
                }

                if (ToType == typeof(Guid))
                {
                    WriteDebug($"Parsing Guid");
                    return Guid.Parse(Value.ToString());
                }

                if (ToType.IsEnum)
                {
                    if (Value is string Name && Name.IsValid())
                    {
                        Name = Name.RemoveAccents().ToUpperInvariant();
                        foreach (var x in Enum.GetValues(ToType))
                        {
                            var entryName = Enum.GetName(ToType, x)?.RemoveAccents().ToUpperInvariant();
                            var entryValue = $"{(int)x}";

                            if (Name == entryName || Name == entryValue)
                            {
                                WriteDebug($"{ToType.Name} value ({Name}) found ({entryName})");
                                return Convert.ChangeType(x, ToType);
                            }
                        }

                        WriteDebug($"{ToType.Name} value ({Name}) not found");
                        return Activator.CreateInstance(ToType);
                    }
                }

                if (ToType.IsSimpleType())
                {
                    WriteDebug($"{ToType.Name} is value type");
                    var Converter = TypeDescriptor.GetConverter(ToType);
                    if (Converter.CanConvertFrom(typeof(TFrom)))
                    {
                        try
                        {
                            return Converter.ConvertTo(Value, ToType);
                        }
                        catch
                        {
                            return Convert.ChangeType(Value, ToType);
                        }
                    }
                    else
                    {
                        return Convert.ChangeType(Value, ToType);
                    }
                }
                else
                {
                    return Convert.ChangeType(Value, ToType);
                }
            }
            catch (Exception ex)
            {
                WriteDebug(ex.ToFullExceptionString(), "Error on change type");
                WriteDebug("Returning null");
                return null;
            }
        }

        /// <summary>
        /// Check the strength of given password
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static PasswordLevel CheckPassword(this string Password)
        {
            var points = Password.ValidateCount(passwordValidations);
            if (points < PasswordLevel.VeryWeak.ToInt())
            {
                return PasswordLevel.VeryWeak;
            }
            else if (points > PasswordLevel.VeryStrong.ToInt())
            {
                return PasswordLevel.VeryStrong;
            }
            else
            {
                return (PasswordLevel)points;
            }
        }

        /// <summary>
        /// Remove todos os subdiretorios vazios
        /// </summary>
        /// <param name="TopDirectory">Diretorio da operação</param>
        public static IEnumerable<DirectoryInfo> CleanDirectory(this DirectoryInfo TopDirectory, bool DeleteTopDirectoryIfEmpty = true)
        {
            var l = new List<DirectoryInfo>();
            if (TopDirectory != null && TopDirectory.Exists)
            {
                foreach (var subdir in TopDirectory.GetDirectories())
                {
                    if (subdir.Exists)
                    {
                        if (subdir.HasDirectories())
                        {
                            l.AddRange(subdir.CleanDirectory(true));
                        }
                        else
                        {
                            if (subdir.IsEmpty())
                            {
                                subdir.Delete();
                                l.Add(subdir);
                            }
                        }
                    }
                }

                if (DeleteTopDirectoryIfEmpty && TopDirectory.Exists && TopDirectory.IsEmpty())
                {
                    TopDirectory.Delete();
                    l.Add(TopDirectory);
                }
            }
            return l;
        }

        /// <summary>
        /// The Collatz conjecture is one of the most famous unsolved problems in mathematics. The
        /// conjecture asks whether repeating two simple arithmetic operations will eventually
        /// transform every positive integer into 1
        /// </summary>
        /// <param name="n">Natural number greater than zero</param>
        /// <returns>an <see cref="IEnumerable{decimal}"/> with all steps until 1</returns>
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
        /// Retorna uma lista com as cores utilizadas na imagem
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static Dictionary<HSVColor, int> ColorPallette(this Image Img, int PixelateSize = 0)
        {
            Bitmap image;
            if (PixelateSize > 0)
            {
                image = (Bitmap)Img.Pixelate(PixelateSize);
            }
            else
            {
                image = new Bitmap(Img);
            }

            var dctColorIncidence = new Dictionary<int, int>();
            using (image)
                if (image != null && image.Width > 0 && image.Height > 0)
                {
                    int coluna = 0;
                    while (coluna < image.Size.Width)
                    {
                        int linha = 0;
                        while (linha < image.Size.Height)
                        {
                            int pixelColor = image.GetPixel(coluna, linha).ToArgb();
                            if (dctColorIncidence.ContainsKey(pixelColor))
                            {
                                dctColorIncidence[pixelColor] = dctColorIncidence[pixelColor] + 1;
                            }
                            else
                            {
                                dctColorIncidence.Add(pixelColor, 1);
                            }

                            linha++;
                        }

                        coluna++;
                    }
                }

            return dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => new HSVColor(Color.FromArgb(x.Key)), x => x.Value);
        }

        public static IEnumerable<string> ColumnsFromClass<T>()
        {
            var PropInfos = typeof(T).GetProperties().Select(y => y.GetAttributeValue<ColumnAttribute, string>(x => x.Name).IfBlank(y.Name));
            var FieldInfos = typeof(T).GetProperties().Select(y => y.GetAttributeValue<ColumnAttribute, string>(x => x.Name).IfBlank(y.Name)).Where(x => x.IsNotIn(PropInfos));

            return PropInfos.Union(FieldInfos);
        }

        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Lista de Imagens para combinar</param>
        /// <param name="VerticalFlow">
        /// Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as
        /// imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)
        /// </param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens da Lista</returns>
        public static Bitmap CombineImages(bool VerticalFlow, params Image[] Images) => Images.CombineImages(VerticalFlow);

        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Array de Imagens para combinar</param>
        /// <param name="VerticalFlow">
        /// Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as
        /// imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)
        /// </param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens do Array</returns>
        public static Bitmap CombineImages(this IEnumerable<Image> Images, bool VerticalFlow = false)
        {
            int width = 0;
            int height = 0;
            foreach (var image in Images ?? Array.Empty<Image>())
            {
                // cria um bitmap a partir do arquivo e o inclui na lista
                var bitmap = new Bitmap(image);

                // atualiza o tamanho da imagem bitmap final
                if (VerticalFlow)
                {
                    height += bitmap.Height;
                    width = bitmap.Width > width ? bitmap.Width : width;
                }
                else
                {
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;
                }
                bitmap.Dispose();
            }

            // cria um bitmap para tratar a imagem combinada
            Bitmap imagemFinal = new Bitmap(width, height);

            // Obtem o objeto gráfico da imagem
            using (var g = Graphics.FromImage(imagemFinal))
            {
                // define a cor de fundo
                g.Clear(Color.White);

                // percorre imagem por imagem e gera uma unica imagem final
                int offset = 0;
                foreach (Bitmap image in Images.Cast<Bitmap>())
                {
                    if (VerticalFlow)
                    {
                        g.DrawImage(image, new System.Drawing.Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                    else
                    {
                        g.DrawImage(image, new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }
            }

            return imagemFinal;
        }

        public static bool CompareARGB(this Color Color1, Color Color2, bool IgnoreAlpha = true) => Color1.CompareARGB(IgnoreAlpha, Color2);

        public static bool CompareARGB(this Color Color1, bool IgnoreAlpha, params Color[] Colors) => (Colors = Colors ?? Array.Empty<Color>()).Any(Color2 => Color1.R == Color2.R && Color1.G == Color2.G && Color1.B == Color2.B && (IgnoreAlpha || Color1.A == Color2.A));

        /// <summary>
        /// Verifica se um texto contém outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool Contains(this string Text, string OtherText, StringComparison StringComparison) => Text.IndexOf(OtherText, StringComparison) > -1;

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsAll(this string Text, params string[] Values) => Text.ContainsAll(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <param name="ComparisonType">Tipo de comparacao</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAll(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            Values = Values ?? Array.Empty<string>();
            if (Values.Any())
            {
                foreach (string value in Values)
                {
                    if (Text == null || Text.IndexOf(value, ComparisonType) == -1)
                    {
                        return false;
                    }
                }

                return true;
            }

            return Text.IsNotValid();
        }

        /// <summary>
        /// Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="List1">Lista 1</param>
        /// <param name="List2">Lista2</param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> List1, IEnumerable<T> List2, IEqualityComparer<T> Comparer = null)
        {
            foreach (T value in List2 ?? Array.Empty<T>())
            {
                if (Comparer != null)
                {
                    if (!(List1 ?? Array.Empty<T>()).Contains(value, Comparer))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!(List1 ?? Array.Empty<T>()).Contains(value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> List1, IEqualityComparer<T> Comparer, params T[] List2) => List1.ContainsAll((List2 ?? Array.Empty<T>()).AsEnumerable(), Comparer);

        public static bool ContainsAllWords(this string Text, params string[] Words) => Text.ContainsAllWords(null, Words);

        public static bool ContainsAllWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words) => Text.GetWords().ContainsAll(Words, Comparer);

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAny(this string Text, params string[] Values) => Text.ContainsAny(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <param name="ComparisonType">Tipo de comparacao</param>
        /// <returns>True se conter algum valor, false se não</returns>
        /// <remarks>
        /// Caso <paramref name="Values"/> for nulo ou vazio, retorna <b>true</b> se <paramref
        /// name="Text"/> não estiver em branco,caso contrário, <b>false</b>
        /// </remarks>
        public static bool ContainsAny(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            Values = (Values ?? Array.Empty<string>()).Where(x => x != null && x != string.Empty).ToArray();
            if (Values.Any())
            {
                foreach (string value in Values)
                {
                    if (Text != null && Text.IndexOf(value, ComparisonType) != -1)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return Text.IsValid();
            }
        }

        public static bool ContainsAny<T>(this IEnumerable<T> List1, IEqualityComparer<T> Comparer, params T[] List2) => ContainsAny(List1, List2);

        public static bool ContainsAny<T>(this IEnumerable<T> List1, params T[] List2) => ContainsAny(List1, List2.AsEnumerable());

        /// <summary>
        /// Verifica se uma lista, coleção ou array contem um dos itens de outra lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="List1">Lista 1</param>
        /// <param name="List2">Lista2</param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> List1, IEnumerable<T> List2, IEqualityComparer<T> Comparer = null)
        {
            foreach (T value in List2.AsEnumerable() ?? Array.Empty<T>())
            {
                if (Comparer == null)
                {
                    if ((List1 ?? Array.Empty<T>()).Contains(value))
                    {
                        return true;
                    }
                }
                else if ((List1 ?? Array.Empty<T>()).Contains(value, Comparer))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool ContainsAnyWords(this string Text, params string[] Words) => Text.ContainsAnyWords(null, Words);

        public static bool ContainsAnyWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words) => Text.GetWords().ContainsAny(Words, Comparer);

        /// <summary>
        /// Verifica se uma string contém caracteres de digito
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsDigit(this string Text) => (Text ?? string.Empty).ToArray().Any(char.IsDigit);

        /// <summary>
        /// Verifica se uma string contém caracteres em minusculo
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsLower(this string Text) => (Text ?? string.Empty).ToArray().Any(char.IsLower);

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter a maioria dos valores, false se não</returns>
        public static bool ContainsMost(this string Text, StringComparison ComparisonType, params string[] Values) => (Values ?? Array.Empty<string>()).Most(value => Text != null && Text.Contains(value, ComparisonType));

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsMost(this string Text, params string[] Values) => Text.ContainsMost(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma string contém caracteres em maiúsculo
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsUpper(this string Text) => (Text ?? EmptyString).ToArray().Any(char.IsUpper);

        public static Expression<Func<TModel, TToProperty>> ConvertParameterType<TModel, TFromProperty, TToProperty>(this Expression<Func<TModel, TFromProperty>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(TToProperty));

            return Expression.Lambda<Func<TModel, TToProperty>>(converted, expression.Parameters);
        }

        /// <summary>
        /// Copia arquivos para dentro de outro diretório
        /// </summary>
        /// <param name="List">Arquivos</param>
        /// <param name="DestinationDirectory">Diretório de destino</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> CopyTo(this IEnumerable<FileInfo> List, DirectoryInfo DestinationDirectory, bool overwrite = true)
        {
            if (DestinationDirectory != null)
            {
                DestinationDirectory.CreateDirectoryIfNotExists();
                foreach (var file in List ?? new List<FileInfo>())
                {
                    if (file != null && file.Exists)
                        yield return file.CopyTo(Path.Combine(DestinationDirectory.FullName, file.Name), overwrite);
                }
            }
        }

        /// <summary>
        /// Conta os caracters especificos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Character">Caractere</param>
        /// <returns></returns>
        public static int CountCharacter(this string Text, char Character) => Text.Count((c) => c == Character);

        /// <summary>
        /// Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="RemoveDiacritics">indica se os acentos devem ser removidos das palavras</param>
        /// <param name="Words">
        /// Desconsidera outras palavras e busca a quantidade de cada palavra especificada em um array
        /// </param>
        /// <returns></returns>
        public static Dictionary<string, long> CountWords(this string Text, bool RemoveDiacritics = true, string[] Words = null)
        {
            if (Words == null)
            {
                Words = Array.Empty<string>();
            }

            var palavras = Text.Split(PredefinedArrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (Words.Any())
            {
                palavras = palavras.Where(x => Words.Select(y => y.ToLowerInvariant()).Contains(x.ToLowerInvariant())).ToArray();
            }

            if (RemoveDiacritics)
            {
                palavras = palavras.Select(p => p.RemoveDiacritics()).ToArray();
                Words = Words.Select(p => p.RemoveDiacritics()).ToArray();
            }

            var dic = palavras.DistinctCount();
            foreach (var w in Words.Where(x => !dic.Keys.Contains(x)))
            {
                dic.Add(w, 0L);
            }

            return dic;
        }


        /// <summary>
        /// Cria uma constante a partir de um valor para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static ConstantExpression CreateConstant(Expression Member, IComparable Value) => CreateConstant(Member.Type, Value);

        public static ConstantExpression CreateConstant(Type Type, IComparable Value) => Value == null ? Expression.Constant(null, Type) : Expression.Constant(Value.ChangeType(Type));

        /// <summary>
        /// Cria uma constante a partir de um tipo para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        /// <summary>
        /// Cria uma constante a partir de um tipo genérico para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static ConstantExpression CreateConstant<Type>(IComparable Value) => CreateConstant(typeof(Type), Value);

        /// <summary>
        /// Converte um objeto para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="T">
        /// Tipo da classe, <see cref="NameValueCollection"/> ou <see cref="Dictionary{TKey, TValue}"/>
        /// </typeparam>
        /// <param name="Obj">valor do objeto</param>
        /// <param name="Keys">Chaves incluidas no dicionario final</param>
        /// <returns></returns>
        public static Dictionary<string, object> CreateDictionary(this object Obj, params string[] Keys)
        {
            if (Obj != null)
            {
                Keys = Keys ?? Array.Empty<string>();

                if (Obj.GetTypeOf() == typeof((string, object)))
                {
                    (string, object) t = Obj.ChangeType<(string, object)>();
                    return new Dictionary<string, object>()
                    {
                        { t.Item1, t.Item2 }
                    };
                }

                if (Obj.GetTypeOf() == typeof(IEnumerable<(string, object)>))
                {
                    var oo = Obj.ChangeType<IEnumerable<(string, object)>>();
                    return CreateDictionary(oo.ToDictionary(x => x.Item1, x => x.Item2), Keys);
                }

                if (Obj.IsDictionary())
                {
                    var po = Obj as IDictionary;
                    var l = new List<KeyValuePair<string, object>>();
                    foreach (DictionaryEntry kv in po)
                    {

                        l.Add(new KeyValuePair<string, object>($"{kv.Key}", (object)kv.Value));
                    }
                    return l.ToDictionary(Keys);
                }
                else if (Obj.IsTypeOf<NameValueCollection>())
                {
                    return ((NameValueCollection)Obj).ToDictionary(Keys);
                }
                else
                {
                    return Obj.GetTypeOf().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => (Keys.Any() == false || x.Name.IsLikeAny(Keys)) && x.CanRead).ToDictionary(prop => prop.Name, prop => prop.GetValue(Obj, null));
                }
            }
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// Converte uma classe para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <param name="Obj">Object</param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> CreateDictionaryEnumerable<T>(this IEnumerable<T> Obj, params string[] Keys) => (Obj ?? Array.Empty<T>()).Select(x => x.CreateDictionary(Keys));


        /// <summary>
        /// Cria um diretório se ele não existir e retorna um <see cref="DirectoryInfo"/> correspondente.
        /// </summary>
        /// <param name="DirectoryName">O nome ou caminho do diretório a ser criado. Pode ser um caminho de arquivo, caso em que o diretório do arquivo será utilizado.</param>
        /// <param name="DateAndTime">(Opcional) Data e hora para criar subdiretórios no formato yyyy/MM/dd, se fornecido.</param>
        /// <returns>Um objeto <see cref="DirectoryInfo"/> representando o diretório criado ou existente.</returns>
        /// <remarks>
        /// Caso o <paramref name="DirectoryName"/> seja um caminho de arquivo, o diretório do arquivo será utilizado.
        /// Se o diretório já existir, apenas retorna o <see cref="DirectoryInfo"/> correspondente.
        /// </remarks>
        public static DirectoryInfo CreateDirectoryIfNotExists(this string DirectoryName, DateTime? DateAndTime = null)
        {
            DirectoryName = DirectoryName.ToFriendlyPathName();
            DirectoryName = DateAndTime.FormatPath(DirectoryName);

            if (DirectoryName.IsFilePath())
            {
                DirectoryName = Path.GetDirectoryName(DirectoryName);
            }

            if (DirectoryName.IsDirectoryPath())
            {
                if (Directory.Exists(DirectoryName) == false)
                {
                    DirectoryName = Directory.CreateDirectory(DirectoryName).FullName;
                }
            }
            else
            {
                throw new ArgumentException("DirectoryName is not a valid path");
            }

            return new DirectoryInfo(DirectoryName);
        }

        /// <summary>
        /// Cria um diretório a partir de um <see cref="DirectoryInfo"/> se ele não existir e retorna o <see cref="DirectoryInfo"/> correspondente.
        /// </summary>
        /// <param name="DirectoryName">O objeto <see cref="DirectoryInfo"/> representando o diretório a ser criado.</param>
        /// <param name="DateAndTime">(Opcional) Data e hora para criar subdiretórios no formato yyyy/MM/dd, se fornecido.</param>
        /// <returns>Um objeto <see cref="DirectoryInfo"/> representando o diretório criado ou existente.</returns>
        public static DirectoryInfo CreateDirectoryIfNotExists(this DirectoryInfo DirectoryName, DateTime? DateAndTime = null) => DirectoryName?.FullName.CreateDirectoryIfNotExists(DateAndTime);

        /// <summary>
        /// Cria um diretório a partir de um <see cref="FileInfo"/> se ele não existir e retorna o <see cref="DirectoryInfo"/> correspondente ao diretório do arquivo.
        /// </summary>
        /// <param name="FileName">O objeto <see cref="FileInfo"/> representando o arquivo cujo diretório será criado.</param>
        /// <param name="DateAndTime">(Opcional) Data e hora para criar subdiretórios no formato yyyy/MM/dd, se fornecido.</param>
        /// <returns>Um objeto <see cref="DirectoryInfo"/> representando o diretório criado ou existente.</returns>
        public static DirectoryInfo CreateDirectoryIfNotExists(this FileInfo FileName, DateTime? DateAndTime = null) => FileName?.FullName.CreateDirectoryIfNotExists(DateAndTime);


        /// <summary>
        /// Cria um diretório a partir de múltiplas partes de caminho (string, DateTime, FileInfo ou DirectoryInfo) e retorna o <see cref="DirectoryInfo"/> correspondente.
        /// </summary>
        /// <param name="PathParts">Partes do caminho que podem ser string, DateTime, FileInfo ou DirectoryInfo.</param>
        /// <returns>Um objeto <see cref="DirectoryInfo"/> representando o diretório criado ou existente.</returns>
        public static DirectoryInfo CreateDirectoryIfNotExists(params object[] PathParts)
        {
            var pt = PathParts.SelectMany((object x, int i) =>
              {
                  if (x is DateTime dt)
                  {
                      return new string[] { dt.ToString("yyyy"), dt.ToString("MM"), dt.ToString("dd") };

                  }
                  else if (x is FileInfo fi)
                  {
                      return fi.Directory.FullName.SplitAny($"{Path.DirectorySeparatorChar}", $"{Path.AltDirectorySeparatorChar}").Skip(i);
                  }
                  else if (x is DirectoryInfo di)
                  {
                      return di.FullName.SplitAny($"{Path.DirectorySeparatorChar}", $"{Path.AltDirectorySeparatorChar}").Skip(i);

                  }
                  else
                  {
                      return x.ChangeType<string>().SplitAny($"{Path.DirectorySeparatorChar}", $"{Path.AltDirectorySeparatorChar}");
                  }
              }).SelectJoinString($"{Path.DirectorySeparatorChar}");

            return pt.CreateDirectoryIfNotExists();
        }

        /// <summary>
        /// Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo
        /// </summary>
        /// <param name="FileName">o nome do arquivo Ex.: "dir1/dir2/dir3/file.txt"</param>
        /// <returns>Um FileInfo contendo as informacoes do arquivo criado</returns>
        public static FileInfo CreateFileIfNotExists(this string FileName, FileType Type = null, DateTime? DateAndTime = null)
        {
            FileName = DateAndTime.FormatPath(FileName);
            Type = Type ?? new FileType(Path.GetExtension(FileName));
            FileName = $"{Path.GetFullPath(FileName.TrimAny(Path.GetExtension(FileName)))}{Type.Extensions.FirstOrDefault()}";

            FileName.CreateDirectoryIfNotExists(DateAndTime);

            if (File.Exists(FileName) == false)
            {
                File.Create(FileName).Dispose();
            }

            return new FileInfo(FileName);
        }

        /// <summary>
        /// Retorna um <see cref="PaginationFilter{T})"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T> CreateFilter<T>(this IEnumerable<T> List) where T : class => (PaginationFilter<T>)new PaginationFilter<T>().SetData(List);

        /// <summary>
        /// Retorna um <see cref="PaginationFilter{T,T})"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T> CreateFilter<T>(this IEnumerable<T> List, Action<PaginationFilter<T, T>> Configuration) where T : class => (PaginationFilter<T>)new PaginationFilter<T>(Configuration).SetData(List);

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, R> CreateFilter<T, R>(this IEnumerable<T> List, Func<T, R> RemapExpression, Action<PaginationFilter<T, R>> Configuration) where T : class => new PaginationFilter<T, R>(RemapExpression, Configuration).SetData(List);

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, R> CreateFilter<T, R>(this IEnumerable<T> List, Func<T, R> RemapExpression) where T : class => new PaginationFilter<T, R>(RemapExpression).SetData(List);

        /// <summary>
        /// Cria um <see cref="Guid"/> a partir de uma string ou um novo <see cref="Guid"/> se a
        /// conversão falhar
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Guid CreateGuidOrDefault(this string Source)
        {
            if (Source.IsBlank() || !Guid.TryParse(Source, out Guid g))
            {
                g = Guid.NewGuid();
            }

            return g;
        }

        public static Type CreateNullableType(this Type type)
        {
            if (type.IsValueType && (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(object)))
            {
                return typeof(object).MakeGenericType(type);
            }

            return type;
        }

        /// <summary>
        /// Creates an object of type TEntity from XML string.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="XML">The XML string.</param>
        /// <returns>The created object.</returns>
        public static T CreateObjectFromXML<T>(this string XML) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            T obj;
            using (var reader = new StringReader(XML))
            {
                obj = (T)serializer.Deserialize(reader);
            }

            return obj;
        }

        /// <summary>
        /// Creates an object of type TEntity from an XML file.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="XML">The XML file.</param>
        /// <returns>The created object.</returns>
        public static T CreateObjectFromXMLFile<T>(this FileInfo XML) where T : class => XML.ReadAllText().CreateObjectFromXML<T>();

        public static T SetObject<T>(this Dictionary<string, object> Dictionary, T Obj, params object[] args) => (T)CreateOrSetObject(Dictionary, Obj, typeof(T), args);

        /// <summary>
        /// Create a new instance of <paramref name="Type"/> and set the properties values from
        /// <paramref name="Dictionary"/>
        /// </summary>
        /// <param name="Dictionary"></param>
        /// <param name="Obj"></param>
        /// <param name="Type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateOrSetObject(this Dictionary<string, object> Dictionary, object Obj, Type Type, params object[] args)
        {
            var tipo = Type.GetNullableTypeOf();
            if (tipo.IsSimpleType())
            {
                return (Dictionary?.Values.FirstOrDefault()).ChangeType(tipo);
            }

            if (Obj == null)
            {
                if ((args ?? Array.Empty<object>()).Any())
                {
                    Obj = Activator.CreateInstance(tipo, args);
                }
                else
                {
                    Obj = Activator.CreateInstance(tipo);
                }
            }

            if (tipo == typeof(Dictionary<string, object>))
            {
                if (Dictionary != null)
                {
                    return Dictionary.AsEnumerable().ToDictionary();
                }

                return null;
            }
            else if (tipo == typeof(Dictionary<string, string>))
            {
                if (Dictionary != null)
                {
                    return Dictionary.AsEnumerable().ToDictionary(x => x.Key, x => x.Value?.ToString());
                }

                return null;
            }

            if (Dictionary != null && Dictionary.Any())
            {
                foreach (var k in Dictionary)
                {
                    var names = k.Key.PropertyNamesFor();

                    var prop = NullCoalesce(names.Select(x => tipo.GetProperty(x)));
                    if (prop != null)
                    {
                        if (prop.CanWrite)
                        {
                            if (k.Value.GetType() == typeof(DBNull))
                            {
                                prop.SetValue(Obj, null);
                            }
                            else
                            {
                                prop.SetValue(Obj, k.Value.ChangeType(prop.PropertyType));
                            }
                        }
                    }
                    else
                    {
                        var fiif = NullCoalesce(names.Select(x => tipo.GetField(x)));
                        if (fiif != null)
                        {
                            if (k.Value.GetType() == typeof(DBNull))
                            {
                                prop.SetValue(Obj, null);
                            }
                            else
                            {
                                prop.SetValue(Obj, k.Value.ChangeType(fiif.FieldType));
                            }
                        }
                    }
                }
            }

            return Obj;
        }

        public static MemberExpression CreatePropertyExpression<T, V>(this Expression<Func<T, V>> Property) => Expression.Property(Property.Parameters.FirstOrDefault() ?? typeof(T).GenerateParameterExpression(), Property.GetPropertyInfo());

        /// <summary>
        /// Create a <paramref name="Width"/> x <paramref name="Height"/> solid image from <paramref name="Color"/>
        /// </summary>
        /// <param name="Color"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image CreateSolidImage(this Color Color, int Width, int Height)
        {
            if (Width <= 0 && Height <= 0) return null;
            if (Width <= 0) Width = Height;
            if (Height <= 0) Height = Width;
            var Bmp = new Bitmap(Width, Height);
            using (var gfx = Graphics.FromImage(Bmp))
            {
                using (var brush = new SolidBrush(Color))
                {
                    gfx.FillRectangle(brush, 0, 0, Width, Height);
                }
            }

            return Bmp;
        }

        /// <inheritdoc cref="CreateSolidImage(Color, int, int)"/>
        public static Image CreateSolidImage(this Color Color, string WidthHeight) => CreateSolidImage(Color, WidthHeight.ParseSize());

        /// <inheritdoc cref="CreateSolidImage(Color, int, int)"/>
        public static Image CreateSolidImage(this Color Color, Size Size) => CreateSolidImage(Color, Size.Width, Size.Height);

        /// <summary>
        /// Cria uma <see cref="Expression"/> condicional a partir de um valor <see cref="Boolean"/>
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="DefaultReturnValue">Valor padrão</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateWhereExpression<T>(this bool DefaultReturnValue) => (f => DefaultReturnValue);

        /// <summary>
        /// Cria uma <see cref="Expression"/> condicional a partir de uma outra expressão
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="predicate">Valor padrão</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateWhereExpression<T>(Expression<Func<T, bool>> predicate) => predicate ?? false.CreateWhereExpression<T>();

        /// <summary>
        /// Converte um objeto para XML
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="obj">Valor do objeto</param>
        /// <returns>um <see cref="XmlDocument"/></returns>
        public static XmlDocument CreateXML<T>(this T obj) where T : class
        {
            if (obj is XmlDocument tdoc)
            {
                return tdoc;
            }
            else
            {
                var xs = new XmlSerializer(typeof(T));
                var doc = new XmlDocument();
                using (var sw = new StringWriter())
                {
                    xs.Serialize(sw, obj);
                    doc.LoadXml(sw.ToString());
                }

                return doc;
            }
        }

        /// <summary>
        /// Cria um arquivo a partir de qualquer objeto usando o <see cref="Util.CreateXML()"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FileInfo CreateXmlFile<T>(this T obj, string FilePath, DateTime? DateAndTime = null) where T : class => obj.CreateXML<T>().ToXMLString().WriteToFile(FilePath, DateAndTime: DateAndTime);

        public static FileInfo CreateXmlFile<T>(this T obj, DirectoryInfo DirectoryPath, string FileName, DateTime? DateAndTime = null) where T : class => obj.CreateXML<T>().ToXMLString().WriteToFile(DirectoryPath, FileName, DateAndTime: DateAndTime);

        public static FileInfo CreateXmlFile<T>(this T obj, DirectoryInfo DirectoryPath, string SubDirectory, string FileName, DateTime? DateAndTime = null) where T : class => obj.CreateXML<T>().ToXMLString().WriteToFile(DirectoryPath, SubDirectory, FileName, DateAndTime: DateAndTime);

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="SizeExpression">Tamanho</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, string SizeExpression) => Image?.Crop(SizeExpression.ParseSize());

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="Size">Tamanho</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, Size Size) => Image?.Crop(Size.Width, Size.Height);

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="MaxWidth">Largura maxima</param>
        /// <param name="MaxHeight">Altura maxima</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, int MaxWidth, int MaxHeight)
        {
            var jpgInfo = ImageCodecInfo.GetImageEncoders().Where(codecInfo => codecInfo.MimeType == "image/png").First();
            Image finalImage = new Bitmap(Image);
            Bitmap bitmap = null;
            int left = 0;
            int top = 0;
            int srcWidth = MaxWidth;
            int srcHeight = MaxHeight;
            bitmap = new Bitmap(MaxWidth, MaxHeight);
            double croppedHeightToWidth = MaxHeight / (double)MaxWidth;
            double croppedWidthToHeight = MaxWidth / (double)MaxHeight;
            if (Image.Width > Image.Height)
            {
                srcWidth = (int)Math.Round(Math.Round(Image.Height * croppedWidthToHeight));
                if (srcWidth < Image.Width)
                {
                    srcHeight = Image.Height;
                    left = (int)Math.Round((Image.Width - srcWidth) / 2d);
                }
                else
                {
                    srcHeight = (int)Math.Round(Math.Round(Image.Height * (Image.Width / (double)srcWidth)));
                    srcWidth = Image.Width;
                    top = (int)Math.Round((Image.Height - srcHeight) / 2d);
                }
            }
            else
            {
                srcHeight = (int)Math.Round(Math.Round(Image.Width * croppedHeightToWidth));
                if (srcHeight < Image.Height)
                {
                    srcWidth = Image.Width;
                    top = (int)Math.Round((Image.Height - srcHeight) / 2d);
                }
                else
                {
                    srcWidth = (int)Math.Round(Math.Round(Image.Width * (Image.Height / (double)srcHeight)));
                    srcHeight = Image.Height;
                    left = (int)Math.Round((Image.Width - srcWidth) / 2d);
                }
            }

            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(Image, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), new System.Drawing.Rectangle(left, top, srcWidth, srcHeight), GraphicsUnit.Pixel);
                finalImage = bitmap;
            }

            using (var encParams = new EncoderParameters(1))
            {
                encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100);
                finalImage.RotateFlip(Image.GetRotateFlip());
                return finalImage;
            }
        }

        /// <summary>
        /// Corta a imagem em um circulo
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <param name="Background">Cor do fundo</param>
        /// <returns></returns>
        public static Image CropToCircle(this Image Img, Color? Background = default) => Img.CropToSquare()?.CropToEllipsis(Background);

        /// <summary>
        /// Corta a imagem em uma elipse
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <param name="Background">Cor do fundo</param>
        /// <returns></returns>
        public static Image CropToEllipsis(this Image Img, Color? Background = default)
        {
            var dstImage = new Bitmap(Img.Width, Img.Height);
            var g = Graphics.FromImage(dstImage);
            Background = Background ?? Color.Transparent;
            using (Brush br = new SolidBrush((Color)Background))
            {
                g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
            }

            var path = new GraphicsPath();
            path.AddEllipse(0, 0, dstImage.Width, dstImage.Height);
            g.SetClip(path);
            g.DrawImage(Img, 0, 0);
            return dstImage;
        }

        /// <summary>
        /// Corta uma imagem para um quadrado perfeito a partir do centro
        /// </summary>
        /// <param name="img">Imagem</param>
        /// <param name="WidthHeight">Tamanho do quadrado em pixels</param>
        /// <returns></returns>
        public static Image CropToSquare(this Image Img, int WidthHeight = 0)
        {
            if (WidthHeight < 1)
            {
                WidthHeight = Img.Height > Img.Width ? Img.Width : Img.Height;
            }

            return Img.Crop(WidthHeight, WidthHeight);
        }

        /// <summary>
        /// Verifica se um texto contém outro ou vice versa
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool CrossContains(this string Text, string OtherText, StringComparison StringComparison = StringComparison.InvariantCultureIgnoreCase) => Text.Contains(OtherText, StringComparison) || OtherText.Contains(Text, StringComparison);

        /// <summary>
        /// Descriptografa uma string
        /// </summary>
        /// <param name="Text">Texto Criptografado</param>
        /// <returns></returns>
        public static string Decrypt(this string Text, string Key = null)
        {
            if (Text.IsValid())
            {
                byte[] Results = default;
                var UTF8 = new UTF8Encoding();
                var HashProvider = new MD5CryptoServiceProvider();
                var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("1234567890")));
                var TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                var DataToDecrypt = Convert.FromBase64String(Text);
                try
                {
                    var Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                TDESAlgorithm.Dispose();
                HashProvider.Dispose();
                return UTF8.GetString(Results);
            }

            return Text;
        }

        public static string DecryptOrReturn(this string Text, string Key = null)
        {
            try
            {
                return Text.Decrypt(Key);
            }
            catch (Exception)
            {
                return Text;
            }
        }

        /// <summary>
        /// Descriptografa uma string
        /// </summary>
        /// <param name="Text">Texto Criptografado</param>
        /// <returns></returns>
        public static string Decrypt(this string text, string Key, string IV)
        {
            if (text.IsValid())
            {
                var aes = new AesCryptoServiceProvider
                {
                    BlockSize = 128,
                    KeySize = 256,
                    IV = new UTF8Encoding(false).GetBytes(IV),
                    Key = new UTF8Encoding(false).GetBytes(Key),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                byte[] src;
                try
                {
                    src = Convert.FromBase64String(text.FixBase64());
                }
                catch
                {
                    src = Convert.FromBase64String(text);
                }

                using (var ddecrypt = aes.CreateDecryptor())
                {
                    try
                    {
                        var dest = ddecrypt.TransformFinalBlock(src, 0, src.Length);
                        return new UTF8Encoding(false).GetString(dest);
                    }
                    catch
                    {
                    }
                }
            }

            return text;
        }

        public static FileInfo DecryptRSA(this FileInfo File, string Key) => File?.ToBytes().DecryptRSA(Key).WriteToFile(File.FullName);

        /// <summary>
        /// Descriptografa um array de bytes encriptada em RSA
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static byte[] DecryptRSA(this byte[] bytes, string Key)
        {
            var cspp = new CspParameters() { KeyContainerName = Key };
            var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true };
            bytes = rsa.Decrypt(bytes, true);
            rsa.Dispose();
            return bytes;
        }

        /// <summary>
        /// Cria uma lista vazia usando um objeto como o ToType da lista. Util para tipos anonimos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjectForDefinition">Objeto que definirá o ToType da lista</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remover o parâmetro não utilizado", Justification = "<Pendente>")]
        public static List<T> DefineEmptyList<T>(this T ObjectForDefinition) => new List<T>();

        public static double DegreesToRadians(this double degrees) => degrees * Math.PI / 180;

        /// <summary>
        /// Deleta um arquivo ou diretório se o mesmo existir e retorna true se o arquivo puder ser
        /// criado novamente
        /// </summary>
        /// <param name="Path">Camingo</param>
        /// <returns></returns>
        public static bool DeleteIfExist(this string Path)
        {
            try
            {
                if (Path.IsDirectoryPath())
                {
                    var d = new DirectoryInfo(Path);
                    if (d.Exists)
                    {
                        try
                        {
                            d.Delete(true);
                        }
                        catch
                        {
                        }
                    }

                    return !d.Exists;
                }

                if (Path.IsFilePath())
                {
                    var d = new FileInfo(Path);
                    if (d.Exists)
                    {
                        try
                        {
                            d.Delete();
                        }
                        catch
                        {
                        }
                    }

                    return !d.Exists;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deleta um arquivo ou diretório se o mesmo existir e retorna TRUE se o arquivo puder ser
        /// criado novamente
        /// </summary>
        /// <param name="Path">Caminho</param>
        /// <returns></returns>
        public static bool DeleteIfExist(this FileSystemInfo Path) => Path?.FullName.DeleteIfExist() ?? false;

        /// <summary>
        /// Remove uma linha especifica de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="LineIndex">Numero da linha</param>
        /// <returns></returns>
        public static string DeleteLine(this string Text, int LineIndex)
        {
            var parts = Text.Split(Environment.NewLine).ToList();
            LineIndex = LineIndex.LimitIndex(parts);

            if (parts.Count > LineIndex)
            {
                parts.RemoveAt(LineIndex);
            }

            return parts.SelectJoinString(Environment.NewLine);
        }

        /// <summary>
        /// Remove um item de uma lista e retorna este item
        /// </summary>
        /// <typeparam name="T">Tipo do item</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="Index">Posicao do item</param>
        /// <returns></returns>
        public static T Detach<T>(this List<T> List, int Index)
        {
            if (List != null && Index.IsBetween(0, List.Count))
            {
                var p = List.ElementAt(Index);
                List.RemoveAt(Index);
                return p;
            }
            return default;
        }

        public static T Detach<T>(this List<T> List, Expression<Func<T, bool>> predicate)
        {
            if (List != null && predicate != null)
            {
                var l = List.FirstOrDefault(predicate.Compile());
                if (l != null)
                    List.Remove(l);
                return l;
            }
            return default;
        }

        public static IEnumerable<T> DetachMany<T>(this List<T> List, Expression<Func<T, bool>> predicate)
        {
            if (List != null && predicate != null)
            {
                var l = List.Where(predicate.Compile());
                if (l != null && l.Any())
                    List.RemoveWhere(predicate);
                return l;
            }
            return default;
        }

        /// <summary>
        /// Remove itens de uma lista e retorna uma outra lista com estes itens
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="Indexes"></param>
        /// <returns></returns>
        public static IEnumerable<T> DetachMany<T>(this List<T> List, params int[] Indexes)
        {
            var l = new List<T>();
            return List.MoveItems(ref l, Indexes);
        }

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
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <param name="OrderBy">
        /// Criterio que indica qual o objeto que deverá ser preservado na lista se encontrado mais
        /// de um
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey, TOrder>(this IEnumerable<T> Items, Func<T, TKey> Property, Func<T, TOrder> OrderBy, bool Descending = false)
        => Items.GroupBy(Property).Select(x => (Descending ? x.OrderByDescending(OrderBy) : x.OrderBy(OrderBy)).FirstOrDefault());

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> Items, Func<T, TKey> Property)
        => Items.GroupBy(Property).Select(x => x.FirstOrDefault());

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <returns></returns>
        public static IQueryable<T> DistinctBy<T, TKey>(this IQueryable<T> Items, Expression<Func<T, TKey>> Property)
        => Items.GroupBy(Property).Select(x => x.First());

        /// <summary>
        /// Cria um dicionário com as palavras de uma lista e a quantidade de cada uma.
        /// </summary>
        /// <param name="List">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(params string[] List) => List.ToList().DistinctCount();

        /// <summary>
        /// Cria um dicionário com as palavras de uma frase e sua respectiva quantidade.
        /// </summary>
        /// <param name="Text">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(this string Text) => Text.Split(WhitespaceChar).ToList().DistinctCount();

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<T, long> DistinctCount<T>(this IEnumerable<T> Arr) => Arr.Distinct().Select(p => new KeyValuePair<T, long>(p, Arr.Where(x => x.Equals(p)).LongCount())).OrderByDescending(p => p.Value).ToDictionary();

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao a partir de uma propriedade
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<TProp, long> DistinctCount<T, TProp>(this IEnumerable<T> Arr, Func<T, TProp> Prop) => Arr.GroupBy(Prop).ToDictionary(x => x.Key, x => x.LongCount()).OrderByDescending(p => p.Value).ToDictionary();

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao e agrupa o resto
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<T, long> DistinctCountTop<T>(this IEnumerable<T> Arr, int Top, T Others)
        {
            var a = Arr.DistinctCount();
            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao a partir de uma propriedade e agrupa o
        /// resto em outra
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<TProp, long> DistinctCountTop<T, TProp>(this IEnumerable<T> Arr, Func<T, TProp> Prop, int Top, TProp Others)
        {
            var a = Arr.DistinctCount(Prop);
            if (Top < 1)
            {
                return a;
            }

            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        public static byte[] DownloadFile(string URL, NameValueCollection Headers = null, Encoding Encoding = null)
        {
            byte[] s = null;
            if (URL.IsURL())
                using (var c = new WebClient())
                {
                    c.Encoding = Encoding ?? new UTF8Encoding(false);

                    if (Headers != null)
                        c.Headers.Add(Headers);
                    s = c.DownloadData(URL);
                }

            return s;
        }

        public static byte[] DownloadFile(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadFile($"{URL}", Headers, Encoding);

        public static Image DownloadImage(string URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadFile(URL, Headers, Encoding).ToImage();

        public static Image DownloadImage(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadImage($"{URL}", Headers, Encoding);

        public static T DownloadJson<T>(string URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadString(URL, Headers, Encoding).FromJson<T>();

        public static object DownloadJson(string URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadString(URL, Headers, Encoding).FromJson();

        public static T DownloadJson<T>(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadJson<T>($"{URL}", Headers, Encoding);

        public static object DownloadJson(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadJson($"{URL}", Headers, Encoding);

        public static string DownloadString(string URL, NameValueCollection Headers = null, Encoding Encoding = null)
        {
            string s = EmptyString;
            if (URL.IsURL())
                using (var c = new WebClient())
                {
                    c.Encoding = Encoding ?? new UTF8Encoding(false);

                    if (Headers != null)
                        c.Headers.Add(Headers);

                    s = $"{c.DownloadString(URL)}";
                }

            return s;
        }

        public static string DownloadString(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadString($"{URL}", Headers, Encoding);

        public static Image DrawImage(this string text, int Width, int Height, Font Font = null, Color? Color = default, int X = -1, int Y = -1)
        {
            return text.DrawImage(new Size(Width, Height), Font, Color, X, Y);
        }
        public static Image DrawImage(this string text, Size imageSize, Font Font = null, Color? Color = default, int X = -1, int Y = -1)
        {
            var b = new Bitmap(imageSize.Width, imageSize.Height);
            return b.DrawString(text, Font, Color, X, Y);
        }
        public static Image DrawString(this Image img, string Text, Font Font = null, Color? Color = default, int X = -1, int Y = -1)
        {
            var bitmap = new Bitmap(img);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                Font = Font ?? new Font("Arial", (float)(bitmap.Width / 10d));
                var tamanho = graphics.MeasureString(Text, Font, new Size(bitmap.Width, bitmap.Height));
                X = X.LimitRange(-1, img.Width);
                Y = Y.LimitRange(-1, img.Height);
                if (X == -1)
                {
                    X = RoundInt(bitmap.Width / 2d - tamanho.Width / 2f);
                }

                if (Y == -1)
                {
                    Y = RoundInt(bitmap.Height / 2d - tamanho.Height / 2f);
                }

                Color = Color ?? bitmap.GetPixel(X, Y).GetContrastColor(50f);
                var B = new System.Drawing.SolidBrush((Color)Color);
                graphics.DrawString(Text, Font, B, X, Y);
            }

            return bitmap;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> items, Action<T> Action)
        {
            if (items != null && Action != null)
            {
                foreach (var item in items)
                {
                    Action(item);
                }
            }
            return items;
        }

        /// <summary>
        /// Criptografa uma string
        /// </summary>
        /// <param name="Text">Texto descriptografado</param>
        /// <returns></returns>
        public static string Encrypt(this string Text, string Key = null)
        {
            if (Text.IsValid())
            {
                byte[] Results = default;
                var UTF8 = new UTF8Encoding();
                var HashProvider = new MD5CryptoServiceProvider();
                var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("1234567890")));
                var TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                var DataToEncrypt = UTF8.GetBytes(Text);
                try
                {
                    var Encryptor = TDESAlgorithm.CreateEncryptor();
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                TDESAlgorithm.Dispose();
                HashProvider.Dispose();
                return Convert.ToBase64String(Results);
            }

            return Text;
        }

        /// <summary>
        /// Criptografa uma string
        /// </summary>
        /// <param name="Text">Texto descriptografado</param>
        /// <returns></returns>
        public static string Encrypt(this string text, string Key, string IV)
        {
            if (text.IsValid())
            {
                var aes = new AesCryptoServiceProvider
                {
                    BlockSize = 128,
                    KeySize = 256,
                    IV = new UTF8Encoding(false).GetBytes(IV),
                    Key = new UTF8Encoding(false).GetBytes(Key),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                var src = new UTF8Encoding(false).GetBytes(text);
                using (var eencrypt = aes.CreateEncryptor())
                {
                    var dest = eencrypt.TransformFinalBlock(src, 0, src.Length);
                    return Convert.ToBase64String(dest);
                }
            }

            return text;
        }

        /// <summary>
        /// Criptografa um string em RSA
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string EncryptRSA(this string Text, string Key)
        {
            var cspp = new CspParameters() { KeyContainerName = Key };
            var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true };
            var bytes = rsa.Encrypt(new UTF8Encoding(false).GetBytes(Text), true);
            rsa.Dispose();
            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// Criptografa um array de bytes em RSA
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static byte[] EncryptRSA(this byte[] bytes, string Key)
        {
            var cspp = new CspParameters() { KeyContainerName = Key };
            using (var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true })
            {
                return rsa.Encrypt(bytes, true);
            }
        }

        public static FileInfo EncryptRSA(this FileInfo File, string Key) => File?.ToBytes().EncryptRSA(Key).WriteToFile(File.FullName);

        /// <summary>
        /// Verifica se uma string termina com alguma outra string de um array
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string Text, StringComparison comparison, params string[] Words) => Words.Any(p => Text.EndsWith(p, comparison));

        public static bool EndsWithAny(this string Text, params string[] Words) => EndsWithAny(Text, default, Words);

        /// <summary>
        /// Constroi uma expressão "igual a"
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression Equal(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.Equal(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Compara uma string com outra ou mais strings ignorando a diferença entre maiusculas e minusculas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="CompareText">Um ou mais textos para comapração</param>
        /// <returns></returns>
        public static bool InsensitiveEqual(this string Text, params string[] CompareText) => CompareText?.Any(x => x?.Equals(Text ?? "", StringComparison.OrdinalIgnoreCase) ?? false) ?? false;

        /// <summary>
        /// Compara uma string com outra ou mais strings ignorando a diferença entre maiusculas e minusculas e acentos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="CompareText">Um ou mais textos para comapração</param>
        /// <returns></returns>
        public static bool FlatEqual(this string Text, params object[] CompareText) => CompareText?.Any(x => InsensitiveEqual(Text.RemoveAccents(), x.ChangeType<string>().RemoveAccents())) ?? false;


        public static bool FlatContains(this string Text, params object[] CompareText) => Text.RemoveAccents().ContainsAny(StringComparison.OrdinalIgnoreCase, CompareText.Select(y => y.ChangeType<string>().RemoveAccents()).ToArray());



        /// <summary>
        /// Prepara uma string com aspas simples para uma string TransactSQL
        /// </summary>
        /// <param name="Text">Texto a ser tratado</param>
        /// <returns>String pronta para a query</returns>
        public static string EscapeQuotesToQuery(this string Text, bool AlsoQuoteText = false) => Text.Replace(SingleQuoteChar, SingleQuoteChar.Repeat(2)).QuoteIf(AlsoQuoteText, '\'');

        /// <summary>
        /// Extrai emails de uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractEmails(this string Text) => Text.IfBlank(string.Empty).SplitAny(PredefinedArrays.InvisibleChars.Union(PredefinedArrays.BreakLineChars).Union(new[] { ":", ";" }).ToArray()).Where(x => x.IsEmail()).Select(x => x.ToLowerInvariant()).Distinct().ToArray();

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
        /// Retorna o Titulo do arquivo a partir do nome do arquivo
        /// </summary>
        /// <param name="Info">Arquivo ou Diretório</param>
        /// <returns></returns>
        public static string FileNameAsTitle(this FileSystemInfo Info, bool ForceCase = false) => Info.FullName.FileNameAsTitle(ForceCase);

        /// <summary>
        /// Retorna o Titulo do arquivo a partir do nome do arquivo
        /// </summary>
        /// <param name="FilePath">Arquivo ou Diretório</param>
        /// <returns></returns>
        public static string FileNameAsTitle(this string FilePath, bool ForceCase = false) => Path.GetFileNameWithoutExtension(FilePath).ToNormalCase().ToTitle(ForceCase);

        public static IEnumerable<T> FilterDateRange<T>(this IEnumerable<T> List, Expression<Func<T, DateTime>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null)
               => List.Where(x => Range.Contains(Property.Compile()(x), FilterBehavior ?? Range.FilterBehavior));

        public static IEnumerable<T> FilterDateRange<T>(this IEnumerable<T> List, Expression<Func<T, DateTime?>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null)
               => List.Where(x => Range.Contains(Property.Compile()(x), FilterBehavior ?? Range.FilterBehavior));

        public static IQueryable<T> FilterDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null) => List.Where(Property.IsInDateRange(Range, FilterBehavior));

        public static IQueryable<T> FilterDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime?>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null) => List.Where(Property.IsInDateRange(Range, FilterBehavior));

        public static IQueryable<T> FilterRange<T, V>(this IQueryable<T> List, Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, IEnumerable<V> Values) => List.Where(MinProperty.IsBetweenOrEqual(MaxProperty, Values));

        public static IQueryable<T> FilterRange<T, V>(this IQueryable<T> List, Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, params V[] Values) => FilterRange(List, MinProperty, MaxProperty, (Values ?? Array.Empty<V>()).AsEnumerable());

        /// <summary>
        /// Procura valores em uma string usnado expressões regulares
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindByRegex(this string Text, string Regex, RegexOptions RegexOptions = RegexOptions.None)
        {
            foreach (Match m in new Regex(Regex, RegexOptions).Matches(Text))
            {
                yield return m.Value;
            }
        }

        /// <summary>
        /// Procura CEPs em uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string[] FindCEP(this string Text) => Text.FindByRegex(@"\d{5}-\d{3}").Union(Text.FindNumbers().Where(x => x.Length == 8)).ToArray();

        /// <summary>
        /// Procura uma cor na tabela de cores <see cref="HSVColor.NamedColors"/>
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static HSVColor FindColor(this string Text) => HSVColor.NamedColors.FirstOrDefault(x => x.Name.ToLowerInvariant().Replace("grey", "gray").RemoveAny(PredefinedArrays.PasswordSpecialChars.Union(new[] { " " }).ToArray()) == Text.ToLowerInvariant().Replace("grey", "gray").RemoveAny(PredefinedArrays.PasswordSpecialChars.Union(new[] { " " }).ToArray()));

        public static FieldInfo FindField(this Type type, string Name) => FindFields(type, Name).FirstOrDefault();

        public static IEnumerable<FieldInfo> FindFields(this Type type, params string[] Names)
        {
            if (type != null && Names != null)
            {
                var propnames = Names.SelectMany(x => x.PropertyNamesFor()).ToList();
                return type.GetFields().Where(x => x.GetCustomAttributes<ColumnAttribute>().Select(n => n.Name).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
            }
            return Array.Empty<FieldInfo>();
        }

        /// <summary>
        /// Procura numeros em uma string e retorna um array deles
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindNumbers(this string Text)
        {
            var l = new List<string>();
            var numbers = Regex.Split(Text, @"\D+");
            foreach (var value in numbers)
            {
                if (!value.IsNotValid())
                {
                    l.Add(value);
                }
            }

            return l;
        }

        public static IEnumerable<PropertyInfo> FindProperties(this Type type, params string[] Names)
        {
            if (type != null && Names != null)
            {
                var propnames = Names.SelectMany(x => x.PropertyNamesFor()).ToList();
                return type.GetProperties().Where(x => x.GetCustomAttributes<ColumnAttribute>().Select(n => n.Name).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
            }
            return Array.Empty<PropertyInfo>();
        }

        public static PropertyInfo FindProperty(this Type type, string Name) => FindProperties(type, Name).FirstOrDefault();

        /// <summary>
        /// TEntity primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstAny<T>(this IEnumerable<T> source, params Expression<Func<T, bool>>[] predicate)
        {
            predicate = predicate ?? Array.Empty<Expression<Func<T, bool>>>();
            for (int index = 0, loopTo = predicate.Length - 1; index <= loopTo; index++)
            {
                var v = source.FirstOrDefault(predicate[index].Compile());
                if (v != null)
                {
                    return v;
                }
            }

            return default;
        }

        /// <summary>
        /// TEntity primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstAnyOr<T>(this IEnumerable<T> source, T Alternate, params Expression<Func<T, bool>>[] predicate)
        {
            var item = (source ?? Array.Empty<T>()).FirstAny(predicate);
            return (item == null) ? Alternate : item;
        }

        public static T FirstOr<T>(this IEnumerable<T> source, params T[] Alternate) => source?.Any() ?? false ? source.First() : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

        /// <summary>
        /// Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="Alternate"></param>
        /// <returns></returns>
        public static T FirstOr<T>(this IEnumerable<T> source, Func<T, bool> predicade, params T[] Alternate) => source?.Any(predicade) ?? false ? source.First(predicade) : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

        /// <summary>
        /// Busca em um <see cref="IQueryable{T}"/> usando uma expressao lambda a partir do nome de
        /// uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="T"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static T FirstOrDefaultExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, object PropertyValue, bool Is = true) => List.FirstOrDefault(WhereExpression<T>(PropertyName, Operator, (IEnumerable<IComparable>)PropertyValue, Is));

        /// <summary>
        /// Arruma os caracteres de uma string Base64
        /// </summary>
        /// <param name="Base64StringOrDataUrl">Base64String ou DataURL</param>
        /// <returns>Retorna apenas a Base64</returns>
        public static string FixBase64(this string Base64StringOrDataUrl)
        {
            string dummyData = Base64StringOrDataUrl.GetAfter(",").Trim().Replace(" ", "+");
            if (dummyData.Length % 4 > 0)
            {
                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
            }

            return dummyData;
        }

        public static string FixedLenght(this int Number, int Lenght) => Number.ToLong().FixedLenght(Lenght);

        public static string FixedLenght(this long Number, int Lenght) => Number.PadZero(Lenght).GetLastChars(Lenght);

        public static string FixedLenghtByLeft(this string Text, int Lenght, char PaddingChar = '0') => Text.PadLeft(Lenght, PaddingChar).GetLastChars(Lenght);

        public static string FixedLenghtByRight(this string Text, int Lenght, char PaddingChar = '0') => Text.PadRight(Lenght, PaddingChar).GetFirstChars(Lenght);

        /// <summary>
        /// Transforma quebras de linha HTML em quebras de linha comuns ao .net
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <returns>String fixada</returns>
        public static string FixHTMLBreakLines(this string Text) => Text.ReplaceMany(Environment.NewLine, "<br/>", "<br />", "<br>").Replace("&nbsp;", WhitespaceChar);

        /// <summary>
        /// Ajusta o tipo da expressão da direita para o tipo da esquerda, quando anulavel
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        public static void FixNullable(ref Expression e1, ref Expression e2)
        {
            var e1type = e1.Type;
            var e2type = e2.Type;
            try
            {
                e1type = ((LambdaExpression)e1).ReturnType;
            }
            catch
            {
            }

            try
            {
                e2type = ((LambdaExpression)e2).ReturnType;
            }
            catch
            {
            }

            if (e1type.IsNullableType() && !e2type.IsNullableType())
            {
                e2 = Expression.Convert(e2, e1type);
            }

            if (!e1type.IsNullableType() && e2type.IsNullableType())
            {
                e1 = Expression.Convert(e1, e2type);
            }

            if (e1.NodeType == ExpressionType.Lambda)
            {
                e1 = Expression.Invoke(e1, ((LambdaExpression)e1).Parameters);
            }

            if (e2.NodeType == ExpressionType.Lambda)
            {
                e2 = Expression.Invoke(e2, ((LambdaExpression)e2).Parameters);
            }
        }

        /// <summary>
        /// Troca valor de <paramref name="FirstValue"/> pelo de <paramref name="SecondValue"/> se
        /// <paramref name="FirstValue"/> for maior que <paramref name="SecondValue"/> fazendo com
        /// que <paramref name="FirstValue"/> seja sempre menor que <paramref name="SecondValue"/>.
        /// Util para tratar ranges
        /// </summary>
        /// <remarks>
        /// Caso <paramref name="FirstValue"/> e/ou <paramref name="SecondValue"/> forem
        /// <b>null</b>, nada acontece
        /// </remarks>
        public static (T, T) FixOrder<T>(ref T FirstValue, ref T SecondValue) where T : IComparable
        {
            if (FirstValue != null && SecondValue != null)
            {
                if (FirstValue.IsGreaterThan(SecondValue))
                {
                    return Swap(ref FirstValue, ref SecondValue);
                }
            }

            return (FirstValue, SecondValue);
        }

        /// <summary>
        /// Troca valor de <paramref name="FirstValue"/> pelo de <paramref name="SecondValue"/> se
        /// <paramref name="FirstValue"/> for maior que <paramref name="SecondValue"/> fazendo com
        /// que <paramref name="FirstValue"/> seja sempre menor que <paramref name="SecondValue"/>.
        /// Util para tratar ranges. Se qualquer um dos 2 valores for null, copia o valor da outra
        /// variavel não <b>null</b>. Se ambas forem <b>null</b> nada acontece.
        /// </summary>
        public static (T, T) FixOrderNotNull<T>(ref T FirstValue, ref T SecondValue) where T : IComparable
        {
            if (FirstValue == null && SecondValue != null)
            {
                FirstValue = SecondValue;
            }
            else if (SecondValue == null && FirstValue != null)
            {
                SecondValue = FirstValue;
            }
            else if (SecondValue == null && FirstValue == null)
            {
                FirstValue = default;
                SecondValue = default;
            }

            return FixOrder(ref FirstValue, ref SecondValue);
        }

        /// <summary>
        /// Return <see cref="Path.DirectorySeparatorChar"/> or <see
        /// cref="Path.AltDirectorySeparatorChar"/> based on the number of ocurrences of each in the string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static char MostCommonPathChar(this string Text)
        {
            if (Text == null) return Path.DirectorySeparatorChar;
            return (Text.Count(x => x == Path.AltDirectorySeparatorChar) > Text.Count(x => x == Path.DirectorySeparatorChar))
           .AsIf(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public static IEnumerable<string> SplitPath(this string Text)
        {
            var PathParts = Text?.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }).Where(x => x.IsNotBlank())?.ToArray() ?? Array.Empty<string>();
            if (PathParts.Count() == 1 && PathParts.First().EndsWith(":"))
            {
                PathParts[0] = PathParts[0] + Path.DirectorySeparatorChar.ToString();
            }

            return PathParts;
        }

        /// <summary>
        /// Extracts the portion of a file path that is relative to a specified base path.
        /// </summary>
        /// <remarks>This method ensures that the returned path is normalized using the specified or
        /// default directory separator.</remarks>
        /// <param name="fullPath">The full file path to process. Must not be null or empty.</param>
        /// <param name="basePath">The base path to which the <paramref name="fullPath"/> is compared. Must not be null or empty.</param>
        /// <param name="AlternativeChar">An optional character to use as the directory separator in the returned path. If <see langword="null"/>, the
        /// default directory separator character is used.</param>
        /// <returns>A string representing the portion of <paramref name="fullPath"/> that is relative to <paramref
        /// name="basePath"/>. If <paramref name="fullPath"/> does not start with <paramref name="basePath"/>, the
        /// original <paramref name="fullPath"/> is returned.</returns>
        public static string GetLeftPathPart(this string fullPath, string basePath, bool? AlternativeChar = null)
        {
            var pt1 = fullPath.FixPath(false);
            var pt2 = basePath.IfBlank(Path.GetPathRoot(fullPath) ?? fullPath ?? "").FixPath(false) + Path.AltDirectorySeparatorChar;

            if (pt1.IsNotBlank() && pt2.IsNotBlank() && pt1.StartsWith(pt2, StringComparison.OrdinalIgnoreCase))
            {
                var relativePath = pt1.Substring(pt2.Length).TrimStartAny(Path.DirectorySeparatorChar.ToString(), Path.AltDirectorySeparatorChar.ToString());
                relativePath = relativePath.FixPath(AlternativeChar);
                return $"{MostCommonPathChar(relativePath)}{relativePath}";
            }

            return fullPath.FixPath(AlternativeChar);
        }

        /// <inheritdoc cref="GetLeftPathPart(string, string, bool?)"/>
        public static string GetLeftPathPart(this FileSystemInfo fullPath, string basePath, bool? AlternativeChar = null) => fullPath?.FullName.GetLeftPathPart(basePath, AlternativeChar) ?? string.Empty;


        /// <summary>
        /// Ajusta um caminho colocando as barras corretamente e substituindo caracteres inválidos
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string FixPath(this string Text, bool? AlternativeChar = null)
        {
            Text = Text ?? "";
            var c = AlternativeChar == null ? Text.MostCommonPathChar().ToString() : AlternativeChar.AsIf(Path.AltDirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString());
            var PathParts = Text?.UrlDecode()?.SplitPath()?.ToArray() ?? Array.Empty<string>();
            var NewPath = PathParts.SelectJoinString(c)
            .Replace($"{c}{c}", c); // Remove duplicadas
            if (NewPath.IsBlank())
            {
                return c;
            }
            return NewPath;
        }

        /// <summary>
        /// Ajusta um caminho unidos partes, colocando as barras corretamente e substituindo
        /// caracteres inválidos
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string FixPath(this string[] PathParts, bool AlternativeChar = false) => PathParts.SelectJoinString(new string(AlternativeChar ? Path.AltDirectorySeparatorChar : Path.DirectorySeparatorChar, 1)).FixPath(AlternativeChar);

        /// <summary>
        /// Adciona pontuação ao final de uma string se a mesma não terminar com alguma pontuacao.
        /// </summary>
        /// <param name="Text">Frase, Texto a ser pontuado</param>
        /// <param name="Punctuation">
        /// Ponto a ser adicionado na frase se a mesma não estiver com pontuacao
        /// </param>
        /// <returns>Frase corretamente pontuada</returns>
        public static string FixPunctuation(this string Text, string Punctuation = ".", bool ForceSpecificPunctuation = false)
        {
            Text = Text.TrimEndAny(true, ",", WhitespaceChar);
            var pts = new[] { ".", "!", "?", ":", ";" };
            if (ForceSpecificPunctuation)
            {
                Text = Text.TrimEndAny(true, pts).Trim() + Punctuation;
            }
            else if (!Text.EndsWithAny(pts))
            {
                Text += Punctuation;
            }

            return Text;
        }

        /// <summary>
        /// Arruma a ortografia do texto captalizando corretamente, adcionando pontuação ao final de
        /// frase caso nescessário e removendo espaços excessivos ou incorretos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string FixText(this string Text, int Ident = 0, int BreakLinesBetweenParagraph = 0)
        {
            Text = Text.IfBlank("");
            var removedot = !Text.Trim().EndsWith(".");
            var addComma = Text.Trim().EndsWith(",");
            Text = new StructuredText(Text) { Ident = Ident, BreakLinesBetweenParagraph = BreakLinesBetweenParagraph }.ToString();
            if (removedot)
            {
                Text = Text.TrimEnd().TrimEndAny(".");
            }
            if (addComma)
            {
                Text = Text.TrimEnd().TrimEndAny(".").Append(",");
            }
            return Text.Trim().TrimBetween();
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

        /// <summary>
        /// Verifica se <paramref name="Obj"/> é um array e retorna este array. Se negativo, retorna
        /// um array contendo o valor de <paramref name="Obj"/> ou um array vazio se <paramref
        /// name="Obj"/> for nulo
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <returns></returns>
        public static object[] ForceArray(this object Obj, Type Type)
        {
            Type = Type ?? typeof(object);
            if (Obj != null)
            {
                if (IsArray(Obj))
                {
                    var aobj = ((Array)Obj).Cast<object>().ToArray();
                    return ChangeArrayType(aobj, Type).ToArray();
                }
                else if (!Obj.IsTypeOf<string>() && Obj.IsEnumerable())
                {
                    var aobj = (IEnumerable<object>)Obj;
                    return ChangeIEnumerableType(aobj, Type).ToArray();
                }
                else
                {
                    return (new[] { Obj }).ChangeArrayType(Type);
                }
            }

            return Array.Empty<object>().ChangeArrayType(Type);
        }

        /// <summary>
        /// Verifica se um objeto é um array, e se não, cria um array com este obejeto
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <returns></returns>
        public static T[] ForceArray<T>(this object Obj) => ForceArray(Obj, typeof(T)).Cast<T>().ToArray();

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
        /// Executa uma ação para cada linha de um texto
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public static string ForEachLine(this string Text, Func<string, string> Action, bool RemoveBlankLines = false)
        {
            if (Text.IsValid())
            {
                if (Action == null)
                {
                    Text = Text.SplitAny(PredefinedArrays.BreakLineChars.ToArray()).Where(x => !RemoveBlankLines || x.IsValid()).SelectJoinString(Environment.NewLine);
                }
                else
                {
                    Text = Text.SplitAny(PredefinedArrays.BreakLineChars.ToArray()).Select(x => Action.Invoke(x)).Where(x => !RemoveBlankLines || x.IsValid()).SelectJoinString(Environment.NewLine);
                }
            }

            return Text;
        }

        /// <summary>
        /// Format a file path using a <see cref="DateTime"/>
        /// </summary>
        /// <remarks>
        /// You can use any Datetime format (from <see cref="DateTime.ToString(string)"/>) or:
        /// <list type="table">
        /// <term>#timestamp#</term>
        /// <description>Will be replaced with <see cref="DateTime.Ticks"/></description>
        /// <br/>
        /// <term>#datedir#</term>
        /// <description>Will be replaced with a directory path <b>year\month\day</b></description>
        /// <br/>
        /// </list>
        /// </remarks>
        /// <param name="DateAndTime"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string FormatPath(this DateTime? DateAndTime, string FilePath, bool AlternativeChar = false)
        {
            DateAndTime = DateAndTime ?? DateTime.Now;
            FilePath = FilePath.Replace($"#timestamp#", DateAndTime.Value.Ticks.ToString());
            FilePath = FilePath.Replace($"#datedir#", $@"{DateAndTime.Value.Year}\{DateAndTime.Value.Month}\{DateAndTime.Value.Day}");
            FilePath = FilePath.Replace($"#datetimedir#", $@"{DateAndTime.Value.Year}\{DateAndTime.Value.Month}\{DateAndTime.Value.Day}\{DateAndTime.Value.Hour}-{DateAndTime.Value.Minute}-{DateAndTime.Value.Second}\");

            foreach (string item in new[] { "d", "dd", "ddd", "dddd", "hh", "HH", "m", "mm", "M", "MM", "MMM", "MMMM", "s", "ss", "t", "tt", "Y", "YY", "YYY", "YYYY", "f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff" })
            {
                FilePath = FilePath.SensitiveReplace($"#{item}#", DateAndTime.Value.ToString(item));
            }

            return FilePath.FixPath(AlternativeChar);
        }

        public static string SimilarityPercentCaseInsensitive(this string Text1, string Text2, int Decimals = -1) => SimilarityCaseInsensitive(Text1, Text2).ToPercentString(Decimals, true);

        public static string SimilarityPercent(this string Text1, string Text2, int Decimals = -1) => Similarity(Text1, Text2).ToPercentString(Decimals, true);

        public static double Similarity(this string Text1, string Text2) => (1.0 - ((double)Text1.LevenshteinDistance(Text2) / (double)Math.Max(Text1.Length, Text2.Length)));

        public static double SimilarityCaseInsensitive(this string Text1, string Text2) => (1.0 - ((double)Text1.LevenshteinDistanceCaseInsensitive(Text2) / (double)Math.Max(Text1.Length, Text2.Length)));
        public static double SimilarityFlat(this string Text1, string Text2) => SimilarityCaseInsensitive(Text1.RemoveAccents(), Text2.RemoveAccents());

        public static int LevenshteinDistanceCaseInsensitive(this string Text1, string Text2) => Text1.ToLower().LevenshteinDistance(Text2.ToLower());
        public static int LevenshteinDistanceFlat(this string Text1, string Text2) => Text1.ToLower().RemoveAccents().LevenshteinDistance(Text2.ToLower().RemoveAccents());



        /// <summary>
        /// Extension Method para <see cref="string.Format(string,object)"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Args">Objetos de substituição</param>
        /// <returns></returns>
        public static string FormatString(this string Text, params string[] Args) => string.Format(Text, Args);



        /// <inheritdoc cref="GenerateBarcodeCheckSum(string)"/>
        public static string GenerateBarcodeCheckSum(long Code) => GenerateBarcodeCheckSum(Code.ToString(CultureInfo.InvariantCulture));

        /// <inheritdoc cref="GenerateBarcodeCheckSum(string)"/>
        public static string GenerateBarcodeCheckSum(int Code) => GenerateBarcodeCheckSum(Code.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Gera um digito verificador usando Mod10 em um numero
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static string GenerateBarcodeCheckSum(string Code)
        {
            if (Code.IsNotNumber())
            {
                throw new ArgumentException("Code is not number", nameof(Code));
            }

            int i = 0;
            int j;
            int p = 0;
            int T;
            T = Code.Length;
            for (j = 1; j <= T; j++)
            {
                if ((j & ~-2) == 0)
                {
                    p += Code.Substring(j - 1, 1).ToInt();
                }
                else
                {
                    i += Code.Substring(j - 1, 1).ToInt();
                }
            }
            if (T == 7 | T == 11)
            {
                i = i * 3 + p;
                p = ToInt((i + 9) / 10) * 10;
                T = p - i;
            }
            else
            {
                p = p * 3 + i;
                i = ToInt((p + 9) / 10) * 10;
                T = i - p;
            }
            return T.ToString(CultureInfo.InvariantCulture);
        }


        /// <summary>
        /// Gera um numero de EAN válido a aprtir da combinação de vários numeros
        /// </summary>
        /// <param name="Numbers"></param>
        /// <returns></returns>
        public static string GenerateEAN(params string[] Numbers) => Numbers.Where(x => x.IsNumber()).SelectJoinString(EmptyString).AppendBarcodeCheckSum();

        /// <inheritdoc cref="GenerateEAN(string[])"/>
        public static string GenerateEAN(params int[] Numbers) => GenerateEAN(Numbers.Select(x => x.ToString()).ToArray());

        public static string GenerateLicenseKey(this Assembly product) => product.GetName().Name.GenerateLicenseKey();

        /// <summary>
        /// Gera uma chave de licença para um produto
        /// </summary>
        /// <param name="productIdentifier"></param>
        /// <returns></returns>
        public static string GenerateLicenseKey(this string productIdentifier)
        {
            if (productIdentifier.IsNotValid()) productIdentifier = Guid.NewGuid().ToString();
            var enc = Encoding.Unicode.GetEncoder();
            byte[] unicodeText = new byte[productIdentifier.Length * 2];
            enc.GetBytes(productIdentifier.ToCharArray(), 0, productIdentifier.Length, unicodeText, 0, true);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(unicodeText);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            productIdentifier = sb.ToString().Substring(0, 28).ToUpper();
            char[] serialArray = productIdentifier.ToCharArray();
            StringBuilder licenseKey = new StringBuilder();

            int j;
            for (int i = 0; i < 28; i++)
            {
                for (j = i; j < 4 + i; j++)
                {
                    licenseKey.Append(serialArray[j]);
                }
                if (j == 28)
                {
                    break;
                }
                else
                {
                    i = (j) - 1;
                    licenseKey.Append('-');
                }
            }
            return licenseKey.ToString();
        }

        /// <summary>
        /// Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        /// </summary>
        /// <returns></returns>
        public static ParameterExpression GenerateParameterExpression<ClassType>() => typeof(ClassType).GenerateParameterExpression();

        /// <summary>
        /// Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static ParameterExpression GenerateParameterExpression(this Type Type) => Expression.Parameter(Type, Type.GenerateParameterName());

        public static string GenerateParameterName(this Type Type)
        {
            if (Type != null)
            {
                return Type.Name.PascalCaseSplit().SelectJoinString(x => x.FirstOrDefault().IfBlank(EmptyString), EmptyString).ToLowerInvariant();
            }

            return "p";
        }

        /// <summary>
        /// Util a password with specific lenght for each char type
        /// </summary>
        /// <param name="AlphaLenght"></param>
        /// <param name="NumberLenght"></param>
        /// <param name="SpecialLenght"></param>
        /// <returns></returns>
        public static string GeneratePassword(int AlphaLenght, int NumberLenght, int SpecialLenght) => GeneratePassword((AlphaLenght / 2d).CeilInt(), (AlphaLenght / 2d).FloorInt(), NumberLenght, SpecialLenght);

        /// <summary>
        /// Util a password with specific lenght for each char type
        /// </summary>
        /// <returns></returns>
        public static string GeneratePassword(int AlphaUpperLenght, int AlphaLowerLenght, int NumberLenght, int SpecialLenght)
        {
            string pass = EmptyString;
            if (AlphaLowerLenght > 0)
            {
                string ss = EmptyString;
                while (ss.Length < AlphaLowerLenght)
                {
                    ss = ss.Append(PredefinedArrays.AlphaLowerChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            if (AlphaUpperLenght > 0)
            {
                string ss = EmptyString;
                while (ss.Length < AlphaUpperLenght)
                {
                    ss = ss.Append(PredefinedArrays.AlphaUpperChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            if (NumberLenght > 0)
            {
                string ss = EmptyString;
                while (ss.Length < NumberLenght)
                {
                    ss = ss.Append(PredefinedArrays.NumberChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            if (SpecialLenght > 0)
            {
                string ss = EmptyString;
                while (ss.Length < SpecialLenght)
                {
                    ss = ss.Append(PredefinedArrays.PasswordSpecialChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            return pass.Shuffle();
        }

        /// <summary>
        /// Util a password with specific <paramref name="Lenght"/>
        /// </summary>
        /// <param name="Lenght"></param>
        /// <returns></returns>
        public static string GeneratePassword(int Lenght = 8)
        {
            var basenumber = Lenght / 3d;
            return GeneratePassword(basenumber.CeilInt(), basenumber.FloorInt(), basenumber.FloorInt()).PadRight(Lenght, Convert.ToChar(PredefinedArrays.AlphaChars.RandomItem())).GetFirstChars(Lenght);
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
        /// Retorna um texto posterior a outro
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Value">Texto Posterior</param>
        /// <returns>Uma string com o valor posterior ao valor especificado.</returns>
        public static string GetAfter(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            Value = Value.IfBlank(EmptyString);

            return Text.IsNotValid() || Text.IndexOf(Value) == -1
                ? WhiteIfNotFound ? EmptyString : $"{Text}"
                : Text.Substring(Text.IndexOf(Value) + Value.Length);
        }

        /// <summary>
        /// Retorna todas as ocorrencias de um texto entre dois textos
        /// </summary>
        /// <param name="Text">  texto correspondente</param>
        /// <param name="Before"> texto Anterior</param>
        /// <param name="After"> texto Posterior</param>
        /// <returns>Uma String com o texto entre o texto anterior e posterior</returns>
        public static string[] GetAllBetween(this string Text, string Before, string After = EmptyString)
        {
            var lista = new List<string>();
            string regx = Before.RegexEscape() + "(.*?)" + After.IfBlank(Before).RegexEscape();
            var mm = new Regex(regx, (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Matches(Text);
            foreach (Match a in mm)
            {
                lista.Add(a.Value.RemoveFirstEqual(Before).RemoveLastEqual(After));
            }

            return lista.ToArray();
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this MemberInfo prop, Expression<Func<TAttribute, TValue>> ValueSelector) where TAttribute : Attribute
        {
            if (prop.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att)
            {
                return att.GetAttributeValue(ValueSelector);
            }

            return default;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Expression<Func<TAttribute, TValue>> ValueSelector) where TAttribute : Attribute
        {
            if (type != null && type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att)
            {
                return att.GetAttributeValue(ValueSelector);
            }

            return default;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this TAttribute att, Expression<Func<TAttribute, TValue>> ValueSelector) where TAttribute : Attribute
        {
            if (att != null)
            {
                if (ValueSelector == null)
                {
                    ValueSelector = x => x.ToString().ChangeType<TValue>();
                }
                try
                {
                    return ValueSelector.Compile()(att);
                }
                catch { }
            }
            return default;
        }

        /// <summary>
        /// Retorna um texto anterior a outro
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Value">Texto Anterior</param>
        /// <returns>Uma string com o valor anterior ao valor especificado.</returns>
        public static string GetBefore(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            Value = Value.IfBlank(EmptyString);
            return Text.IsNotValid() || Text.IndexOf(Value) == -1 ? WhiteIfNotFound ? EmptyString : $"{Text}" : Text.Substring(0, Text.IndexOf(Value));
        }

        /// <summary>
        /// Retorna o texto entre dois textos
        /// </summary>
        /// <param name="Text">TEntity texto correspondente</param>
        /// <param name="Before">TEntity texto Anterior</param>
        /// <param name="After">TEntity texto Posterior</param>
        /// <returns>Uma String com o texto entre o texto anterior e posterior</returns>
        public static string GetBetween(this string Text, string Before, string After)
        {
            if (Text.IsValid())
            {
                int beforeStartIndex = Text.IndexOf(Before);
                int startIndex = beforeStartIndex + Before.Length;
                int afterStartIndex = Text.IndexOf(After, startIndex);
                return beforeStartIndex < 0 || afterStartIndex < 0 ? Text : Text.Substring(startIndex, afterStartIndex - startIndex);
            }
            return EmptyString;
        }

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


        public static int GetDecimalLength(this decimal number) => BitConverter.GetBytes(decimal.GetBits(number)[3])[2];

        public static int GetDecimalLength(this double number) => number.ToDecimal().GetDecimalLength();

        /// <summary> GetCliente the Decimal Part of <see cref="decimal" /> as <see cref="long"> </summary>
        /// <param name="Value"></param> <param name="Length"></param> <returns></returns>
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

        public static double[,] GetDistanceMatrix(this IEnumerable<AddressInfo> locations) => GetDistanceMatrix(locations?.ToArray() ?? Array.Empty<AddressInfo>());

        public static double[,] GetDistanceMatrix(params AddressInfo[] locations)
        {
            // Util the distance matrix
            double[,] distanceMatrix = new double[locations.Length, locations.Length];
            for (int i = 0; i < locations.Length; i++)
            {
                for (int j = 0; j < locations.Length; j++)
                {
                    // Set the distance in the distance matrix
                    distanceMatrix[i, j] = CalculateDistance(locations[i], locations[j]);
                }
            }
            return distanceMatrix;
        }

        /// <summary>
        /// Pega o dominio principal de uma URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomain(this Uri URL, bool RemoveFirstSubdomain = false)
        {
            string d = URL.Authority;
            if (RemoveFirstSubdomain)
            {
                d = d.Split(".").Skip(1).SelectJoinString(".");
            }

            return d;
        }

        /// <summary>
        /// Pega o dominio principal de uma URL ou email
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomain(this string URL, bool RemoveFirstSubdomain = false)
        {
            if (URL.IsEmail())
            {
                URL = $"http://{URL.GetAfter("@")}";
            }

            if (!URL.IsURL())
            {
                URL.Prepend("http://");
            }

            return new Uri(URL).GetDomain(RemoveFirstSubdomain);
        }

        /// <summary>
        /// Pega o protocolo e o dominio principal de uma URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomainAndProtocol(this string URL) => $"{new Uri(URL.PrependIf("http://", x => x.IsURL() == false)).GetLeftPart(UriPartial.Authority)}";

        /// <summary>
        /// Pega o encoder a partir de um formato de imagem
        /// </summary>
        /// <param name="RawFormat">Image format</param>
        /// <returns>image codec info.</returns>
        public static ImageCodecInfo GetEncoderInfo(this ImageFormat RawFormat) => ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == RawFormat.Guid).FirstOr(ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == ImageFormat.Png.Guid).First());

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de uma string </summary> <typeparam name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this string Name) => (T)GetEnumValue(Name, typeof(T));

        public static object GetEnumValue(this string Name, Type EnumType)
        {
            if (EnumType != null && EnumType.IsEnum)
                return Name.ChangeType(EnumType);
            throw new ArgumentException("EnumType is not Enum", nameof(EnumType));
        }

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de um <paramref name="Value"/> inteiro </summary> <typeparam
        /// name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this int? Value) => Value.HasValue ? GetEnumValue<T>($"{Value.Value}") : default;

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de um <paramref name="Value"/> inteiro </summary> <typeparam
        /// name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this int Value) => GetEnumValue<T>($"{Value}");

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this T Value)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an Enumeration type.", nameof(T));
            return Enum.GetName(typeof(T), Value);
        }
        /// <summary>
        /// Traz todos os Valores de uma enumeração como string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<string> GetEnumValuesAsString<T>()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an Enumeration type.", nameof(T));
            return Enum.GetValues(typeof(T)).Cast<T>().Select(x => x.GetEnumValueAsString());
        }


        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this string Value) => Value.GetEnumValue<T>().GetEnumValueAsString();

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this int Value) => Value.GetEnumValue<T>().GetEnumValueAsString();

        /// <summary>
        /// Traz todos os Valores de uma enumeração
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumValues<T>()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("type must be an Enumeration type.", nameof(T));
            return Enum.GetValues(typeof(T)).Cast<T>().AsEnumerable();
        }

        public static Dictionary<string, int> GetEnumValuesDictionary<T>() => GetEnumValues<T>().ToDictionary(x => x.GetEnumValueAsString(), x => x.ToInt());

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static FieldInfo GetField<T>(this T MyObject, string Name) => MyObject.GetTypeOf().GetFields().SingleOrDefault(x => (x.Name ?? EmptyString) == (Name ?? EmptyString));

        public static FieldInfo GetFieldInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (!(GetMemberInfo(propertyLambda) is FieldInfo propInfo))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property, not a field.");
            }

            return propInfo;
        }

        public static IEnumerable<FieldInfo> GetFields<T>(this T MyObject, BindingFlags BindAttr) => MyObject.GetTypeOf().GetFields(BindAttr).ToList();

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFields<T>(this T MyObject) => MyObject.GetTypeOf().GetFields().ToList();

        /// <summary>
        /// Retorna o nome do arquivo sem a extensão
        /// </summary>
        /// <param name="Info"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(this FileInfo Info) => Info != null ? Path.GetFileNameWithoutExtension(Info.Name) : EmptyString;

        /// <summary>
        /// Retorna o Mime TEntity a partir de um arquivo
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetMimeType(this FileInfo File) => File.Extension.GetFileType().MimeTypes;

        /// <summary>
        /// Retorna o Mime TEntity a partir de de um formato de Imagem
        /// </summary>
        /// <param name="RawFormat">Formato de Imagem</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetMimeTypes(this ImageFormat RawFormat)
        {
            try
            {
                RawFormat = RawFormat ?? ImageFormat.Png;
                foreach (var img in ImageCodecInfo.GetImageEncoders())
                {
                    if (img.FormatID == RawFormat.Guid)
                    {
                        return img.FilenameExtension.GetFileType().MimeTypes;
                    }
                }
            }
            catch
            {
            }

            return GetFileType(".png").MimeTypes;
        }

        public static string GetMimeType(this ImageFormat RawFormat)
        {
            try
            {
                RawFormat = RawFormat ?? ImageFormat.Png;
                foreach (var img in ImageCodecInfo.GetImageEncoders())
                {
                    if (img.FormatID == RawFormat.Guid)
                    {
                        ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                        return codecs.First(codec => codec.FormatID == RawFormat.Guid).MimeType;
                    }
                }
            }
            catch
            {
            }

            return "image/png";
        }

        /// <summary>
        /// Retorna o Mime TEntity a partir de de uma Imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetMimeTypes(this Image Image) => Image?.RawFormat.GetMimeTypes() ?? Array.Empty<string>();
        public static string GetMimeType(this Image Image) => Image?.RawFormat.GetMimeType() ?? "image/png";

        public static string GetFirstChars(this string Text, int Number = 1)
        {
            while (Number < 0)
            {
                Number = Text.Length + Number;
            }

            return Text.IsValid() ? Text.Length < Number || Number < 0 ? Text : Text.Substring(0, Number) : EmptyString;
        }

        public static DataRow GetFirstRow(this DataSet Data) => Data?.GetFirstTable()?.GetFirstRow();

        public static DataRow GetFirstRow(this DataTable Table) => Table != null && Table.Rows.Count > 0 ? Table.Rows[0] : null;

        public static DataTable GetFirstTable(this DataSet Data) => Data != null && Data.Tables.Count > 0 ? Data.Tables[0] : null;

        /// <summary>
        /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo
        /// </summary>
        /// <param name="Extension">Arquivo</param>
        /// <returns></returns>
        public static string GetFontAwesomeIconByFileExtension(this string Extension) => GetFontAwesomeIconByFileExtension(new[] { Extension ?? "" });

        public static string GetFontAwesomeIconByFileExtension(this string[] Extensions)
        {
            foreach (var Extension in Extensions ?? Array.Empty<string>())
            {
                if (Extension.IsValid())
                {
                    switch (Path.GetExtension(Extension).IfBlank(Extension).RemoveAny(".").ToLowerInvariant())
                    {
                        case "vcf":
                        case "vcard":
                            {
                                return "fa-address-card";
                            }
                        case "ics":
                        case "ical":
                        case "ifb":
                        case "icalendar":
                            {
                                return "fa-calendar";
                            }
                        case "png":
                        case "jpg":
                        case "gif":
                        case "jpeg":
                        case "psd":
                        case "ai":
                        case "drw":
                        case "svg":
                        case "eps":
                        case "tiff":
                        case "webp":
                        case "cdr":
                            {
                                return "fa-file-image";
                            }

                        case "doc":
                        case "docx":
                            {
                                return "fa-file-word";
                            }

                        case "pdf":
                            {
                                return "fa-file-pdf";
                            }

                        case "ppt":
                        case "pptx":
                            {
                                return "fa-file-powerpoint";
                            }

                        case "xls":
                        case "xlsx":
                            {
                                return "fa-file-excel";
                            }

                        case "html":
                        case "htm":
                        case "php":
                        case "cpp":
                        case "vb":
                        case "cs":
                        case "jsp":
                        case "xml":
                        case "css":
                        case "aspx":
                        case "ascx":
                        case "ashx":
                        case "config":
                        case "json":
                        case "jsx":
                        case "js":
                        case "ts":
                        case "vbs":
                        case "ps1":
                        case "lua":
                            {
                                return "fa-file-code";
                            }
                        case "apk":
                        case "appbundle":
                            {
                                return "fa-android";
                            }

                        case "ios":
                        case "ipa":
                            {
                                return "fa-apple";
                            }

                        case "xap":
                        case "appx":
                            {
                                return "fa-windows";
                            }

                        case "zip":
                        case "rar":
                        case "tar":
                        case "gz":
                        case "7zip":
                        case "7z":
                        case "b1":
                        case "bar":
                        case "rar5":
                        case "pk3":
                        case "pkg":
                            {
                                return "fa-file-archive";
                            }

                        case "avi":
                        case "mpeg":
                        case "mp4":
                        case "3gp":
                        case "mkv":
                        case "wmv":
                        case "rmvb":
                        case "mov":
                        case "webm":
                        case "ogv":
                            {
                                return "fa-file-video";
                            }

                        case "txt":
                        case "otf":
                        case "otd":
                        case "ttf":
                        case "rtf":
                        case "xps":
                        case "cfg":
                            {
                                return "fa-file-text";
                            }

                        case "csv":
                            {
                                return "fa-file-csv";
                            }

                        case "mp3":
                        case "mp2":
                        case "wma":
                        case "wav":
                        case "ogg":
                        case "flac":
                        case "aac":
                            {
                                return "fa-file-audio";
                            }

                        case "gb":
                        case "gba":
                        case "n64":
                        case "v64":
                        case "z64":
                        case "rom":
                        case "gbc":
                        case "smc":
                        case "sfc":
                        case "wad":
                        case "ndc":
                        case "nds":
                        case "gci":
                        case "3ds":
                        case "nes":
                        case "snes":
                        case "cia":
                        case "gcz":
                        case "xci":
                        case "nsp":
                            {
                                return "fa-gamepad";
                            }
                        case "iso":
                        case "ape":
                        case "bwt":
                        case "ccd":
                        case "cdi":
                        case "cue":
                        case "b5t":
                        case "b6t":
                            {
                                return "fa-compact-disc";
                            }

                        case "dll":
                            {
                                return "fa-cog";
                            }

                        case "exe":
                        case "bat":
                        case "msi":
                            {
                                return "fa-window-maximize";
                            }

                        case "sql":
                        case "db":
                        case "sqlite":
                        case "litedb":
                        case "mdb":
                        case "mdf":
                            {
                                return "fa-database";
                            }
                        case "bak":
                            {
                                return "fa-copy";
                            }
                        case "jar":
                            {
                                return "fa-java";
                            }

                        default:
                            {
                                break;
                            }
                    }
                }
            }
            return "fa-file";
        }



        /// <summary>
        /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo ou diretório
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string GetIconByFileType(this FileSystemInfo File, bool DirectoryOpen = false)
        {
            if (File != null && File.Attributes == FileAttributes.Device)
            {
                return "fa-plug";
            }
            else if (File != null && File.Attributes == FileAttributes.Directory)
            {
                return DirectoryOpen ? "fa-folder-open" : "fa-folder";
            }
            else
            {
                return GetFontAwesomeIconByFileExtension(File?.Extension);
            }
        }

        /// <summary>
        /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo
        /// </summary>
        /// <param name="MIME">MIME TEntity do Arquivo</param>
        /// <returns></returns>
        public static string GetIconByFileType(this FileType MIME) => GetFontAwesomeIconByFileExtension(MIME?.Extensions.ToArray() ?? Array.Empty<string>());

        /// <summary>
        /// Retorna o formato da imagem correspondente a aquela imagem
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(this Image OriginalImage) => ImageTypes.Where(p => p.Guid == OriginalImage.RawFormat.Guid).FirstOr(ImageFormat.Png);

        /// <summary>
        /// Tenta retornar um index de um IEnumerable a partir de um valor especifico. retorna -1 se
        /// o index nao existir
        /// </summary>
        /// <typeparam name="T">Tipo do IEnumerable e do valor</typeparam>
        /// <param name="Arr">Array</param>
        /// <returns></returns>
        public static IEnumerable<int> GetIndexesOf<T>(this IEnumerable<T> Arr, params T[] items)
        {
            foreach (var i in items ?? Array.Empty<T>())
            {
                yield return Arr.GetIndexOf(i);
            }
        }

        public static int GetIndexOf<T>(this IEnumerable<T> Arr, T item)
        {
            try
            {
                if (Arr != null)
                {
                    if (Arr is IList<T> lista) return lista.IndexOf(item);
                    else if (Arr is IList lista2) return lista2.IndexOf(item);
                    else
                    {
                        int index = 0;
                        foreach (var element in Arr)
                        {
                            if (element.Equals(item))
                                return index;
                            index++;
                        }
                    }
                }
            }
            catch
            {
                try
                {
                    return Arr.ToList().IndexOf(item);
                }
                catch
                {
                }
            }

            return -1;
        }

        public static IEnumerable<Type> GetInheritedClasses<T>() where T : class => GetInheritedClasses(typeof(T));

        public static IEnumerable<Type> GetInheritedClasses(this Type MyType) =>
            //if you want the abstract classes drop the !TheType.IsAbstract but it is probably to instance so its a good idea to keep it.
            Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType));

        public static IEnumerable<string> GetIPs() => GetLocalIP().Union(new[] { GetPublicIP() });

        public static string GetLastChars(this string Text, int Number = 1)
        {
            while (Number < 0)
            {
                Number = Text.Length + Number;
            }

            return Text.IsValid() ? Text.Length < Number || Number < 0 ? Text : Text.Substring(Text.Length - Number) : EmptyString;
        }

        /// <summary>
        /// Retorna o nome do diretorio onde o arquivo se encontra
        /// </summary>
        /// <param name="Path">Caminho do arquivo</param>
        /// <returns>o nome do diretório sem o caminho</returns>
        public static string GetLatestDirectoryName(this FileInfo Path) => System.IO.Path.GetDirectoryName(Path.DirectoryName);

        public static IEnumerable<string> GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    yield return ip.ToString().Trim();
                }
            }
        }

        public static TType GetMemberInfo<TType, TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda) where TType : MemberInfo
        {
            return GetMemberInfo<TSource, TProperty>(propertyLambda) as TType;
        }
        public static MemberInfo GetMemberInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            MemberExpression member;
            switch (propertyLambda.Body)
            {
                case UnaryExpression unaryExpression:
                    member = (MemberExpression)unaryExpression.Operand;
                    break;

                default:
                    member = propertyLambda.Body as MemberExpression;
                    break;
            }
            if (member is null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            return member.Member;
        }

        public static string GetMemberName(MemberInfo member)
        {
            if (member != null)
            {
                if (member.IsDefined(typeof(DataMemberAttribute), true))
                {
                    DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        return dataMemberAttribute.Name;
                }

                return member.Name;
            }
            return null;
        }

        /// <summary>
        /// Retorna N caracteres de uma string a partir do caractere encontrado no centro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetMiddleChars(this string Text, int Length)
        {
            Text = Text.IfBlank(EmptyString);
            if (Text.Length >= Length)
            {
                if (Text.Length % 2 != 0)
                {
                    try
                    {
                        return Text.Substring((int)Math.Round(Text.Length / 2d - 1d), Length);
                    }
                    catch
                    {
                        return Text.GetMiddleChars(Length - 1);
                    }
                }
                else
                {
                    return Text.RemoveLastChars(1).GetMiddleChars(Length);
                }
            }

            return Text;
        }

        /// <summary>
        /// Retorna uma lista com as N cores mais utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image, int Count) => new Bitmap(Image).GetMostUsedColors().Take(Count);

        /// <summary>
        /// Retorna uma lista com as cores utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image) => Image.ColorPallette().Keys;

        /// <summary>
        /// Retorna a cor negativa de uma cor
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <returns></returns>
        public static Color GetNegativeColor(this Color TheColor) => Color.FromArgb(255 - TheColor.R, 255 - TheColor.G, 255 - TheColor.B);

        /// <summary>
        /// Retorna o <see cref="Type"/> equivalente a <typeparamref name="T"/> ou o <see
        /// cref="Type"/> do objeto <see cref="Nullable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>
        /// o tipo do objeto ou o tipo do objeto anulavel ou o prorio objeto se ele for um <see cref="Type"/>
        /// </returns>
        public static Type GetNullableTypeOf<T>(this T Obj)
        {
            var tt = Obj.GetTypeOf();
            tt = Nullable.GetUnderlyingType(tt) ?? tt;
            return tt;
        }

        /// <summary>
        /// Retorna uma expressão de comparação para um ou mais valores e uma ou mais propriedades
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="[Operator]"></param>
        /// <param name="PropertyValues"></param>
        /// <param name="Conditional"></param>
        /// <returns></returns>
        public static BinaryExpression GetOperatorExpression(Expression Member, string Operator, IEnumerable<IComparable> PropertyValues, FilterConditional Conditional = FilterConditional.Or)
        {
            PropertyValues = PropertyValues ?? Array.Empty<IComparable>();
            bool comparewith = !Operator.StartsWithAny("!");
            if (comparewith == false)
            {
                Operator = Operator.TrimStartAny(false, "!");
            }

            BinaryExpression body = null;

            switch (Operator.ToLowerInvariant().IfBlank("equal"))
            {
                case "blank":
                case "compareblank":
                case "isblank":
                case "isempty":
                case "empty":
                    {
                        foreach (var item in PropertyValues)
                        {
                            var exp = Expression.Equal(Member, Expression.Constant(EmptyString, Member?.Type));
                            switch (body)
                            {
                                case null:
                                    body = exp;
                                    break;

                                default:
                                    if (Conditional == FilterConditional.And)
                                    {
                                        body = Expression.AndAlso(body, exp);
                                    }
                                    else
                                    {
                                        body = Expression.OrElse(body, exp);
                                    }

                                    break;
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "isnull":
                case "comparenull":
                case "null":
                case "nothing":
                case "isnothing":
                    {
                        foreach (var item in PropertyValues)
                        {
                            var exp = Expression.Equal(Member, Expression.Constant(null, Member.Type));
                            if (body == null)
                            {
                                body = exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "=":
                case "==":
                case "equal":
                case "===":
                case "equals":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = Member.Equal(CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case ">=":
                case "greaterthanorequal":
                case "greaterorequal":
                case "greaterequal":
                case "greatequal":
                    {
                        foreach (var ii in PropertyValues)
                        {
                            var item = ii;
                            if (!(item.GetNullableTypeOf() == typeof(DateTime)) && item.IsNotNumber() && item.ToString().IsValid())
                            {
                                item = item.ToString().Length;
                            }

                            object exp = null;
                            try
                            {
                                exp = GreaterThanOrEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<=":
                case "lessthanorequal":
                case "lessorequal":
                case "lessequal":
                    {
                        foreach (var ii in PropertyValues)
                        {
                            var item = ii;
                            if (!ReferenceEquals(item.GetNullableTypeOf(), typeof(DateTime)) && item.IsNotNumber() && item.ToString().IsValid())
                            {
                                item = item.ToString().Length;
                            }

                            object exp = null;
                            try
                            {
                                exp = LessThanOrEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case ">":
                case "greaterthan":
                case "greater":
                case "great":
                    {
                        foreach (var item in PropertyValues)
                        {
                            Expression exp = null;
                            try
                            {
                                exp = GreaterThan(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<":
                case "lessthan":
                case "less":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = LessThan(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<>":
                case "notequal":
                case "different":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = NotEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "betweenequal":
                case "betweenorequal":
                case "btweq":
                case "=><=":
                    {
                        if (PropertyValues.Count() > 1)
                        {
                            if (Member.Type == typeof(string))
                            {
                                body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", !comparewith), new[] { PropertyValues.First() }, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", !comparewith), new[] { PropertyValues.Last() }, Conditional));
                            }
                            else
                            {
                                var ge = GetOperatorExpression(Member, "greaterequal".PrependIf("!", !comparewith), new[] { PropertyValues.Min() }, Conditional);
                                var le = GetOperatorExpression(Member, "lessequal".PrependIf("!", !comparewith), new[] { PropertyValues.Max() }, Conditional);
                                body = Expression.And(ge, le);
                            }
                        }
                        else
                        {
                            body = GetOperatorExpression(Member, "=".PrependIf("!", !comparewith), PropertyValues, Conditional);
                        }

                        break;
                    }

                case "><":
                case "startend":
                case "startends":
                case "btw":
                case "between":
                    {
                        if (PropertyValues.Count() > 1)
                        {
                            switch (Member.Type)
                            {
                                case var case1 when case1 == typeof(string):
                                    {
                                        body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", !comparewith), new[] { PropertyValues.First() }, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", !comparewith), new[] { PropertyValues.Last() }, Conditional));
                                        break;
                                    }

                                default:
                                    {
                                        body = Expression.And(GetOperatorExpression(Member, "greater".PrependIf("!", !comparewith), new[] { PropertyValues.Min() }, Conditional), GetOperatorExpression(Member, "less".PrependIf("!", !comparewith), new[] { PropertyValues.Max() }, Conditional));
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            body = GetOperatorExpression(Member, "=".PrependIf("!", !comparewith), PropertyValues, Conditional);
                        }

                        break;
                    }

                case "starts":
                case "start":
                case "startwith":
                case "startswith":
                    {
                        switch (Member.Type)
                        {
                            case var case2 when case2 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, startsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, ">=", PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "ends":
                case "end":
                case "endwith":
                case "endswith":
                    {
                        switch (Member.Type)
                        {
                            case var case3 when case3 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, endsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, "lessequal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "like":
                case "contains":
                    {
                        switch (Member.Type)
                        {
                            case var case4 when case4 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, containsMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "in":
                case "isin":
                case "inside":
                    {
                        switch (Member.Type)
                        {
                            case var case5 when case5 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    // 'TODO: implementar busca de array de inteiro,data etc
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "cross":
                case "crosscontains":
                case "insidecontains":
                    {
                        switch (Member.Type)
                        {
                            case var case6 when case6 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.OrElse(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Call(Member, containsMethod, Expression.Constant(item.ToString()))), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    //TODO: implementar busca de array de inteiro,data etc
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                default: // Executa um metodo com o nome definido pelo usuario que retorna uma expression compativel
                    {
                        try
                        {
                            var metodo = Member.Type.GetMethods().FirstOrDefault(x => (x.Name.ToLowerInvariant() ?? EmptyString) == (Operator.ToLowerInvariant() ?? EmptyString));
                            Expression exp = (Expression)metodo.Invoke(null, new[] { PropertyValues });
                            exp = Expression.Equal(Expression.Invoke(exp, new[] { Member }), Expression.Constant(comparewith));
                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }
                        }
                        catch
                        {
                        }

                        break;
                    }
            }

            return body;
        }

        /// <summary>
        /// Retorna o caractere de encapsulamento oposto ao caractere indicado
        /// </summary>
        /// <param name="Text">Caractere</param>
        /// <returns></returns>
        public static string GetOppositeWrapChar(this string Text)
        {
            switch (Text.GetFirstChars() ?? EmptyString)
            {
                case DoubleQuoteChar: return DoubleQuoteChar;
                case SingleQuoteChar: return SingleQuoteChar;
                case "(": return ")";
                case ")": return "(";
                case "[": return "]";
                case "]": return "[";
                case "{": return "}";
                case "}": return "{";
                case "<": return ">";
                case ">": return "<";
                case @"\": return "/";
                case "/": return @"\";
                case "¿": return "?";
                case "?": return "¿";
                case "!": return "¡";
                case "¡": return "!";
                case ".": return ".";
                case ":": return ":";
                case ";": return ";";
                case "_": return "_";
                case "*": return "*";
                default: return Text;
            }
        }

        public static char GetOppositeWrapChar(this char c) => $"{c}".GetOppositeWrapChar().FirstOrDefault();

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
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this T MyObject, BindingFlags BindAttr) => MyObject.GetTypeOf().GetProperties(BindAttr).ToList();

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this T MyObject) => MyObject.GetTypeOf().GetProperties().ToList();

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<T>(this T MyObject, string Name) => MyObject.GetTypeOf().GetProperties().SingleOrDefault(x => (x.Name ?? EmptyString) == (Name ?? EmptyString));

        /// <summary>
        /// Retorna uma <see cref="Hashtable"/> das propriedades de um objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Hashtable GetPropertyHash<T>(T properties)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                var props = TypeDescriptor.GetProperties(properties);
                foreach (PropertyDescriptor prop in props)
                {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }

            return values;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (!(GetMemberInfo(propertyLambda) is PropertyInfo propInfo))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()));
            }

            return propInfo;
        }

        /// <summary>
        /// Retorna as informacoes de uma propriedade a partir de um seletor
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda == null) return null;

            var type = source.GetTypeOf() ?? typeof(TSource);
            if (!(propertyLambda.Body is MemberExpression member))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda.ToString()));
            }

            if (!(member.Member is PropertyInfo propInfo))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()));
            }

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a property that is not from type {1}.", propertyLambda.ToString(), type));
            }

            return propInfo;
        }

        /// <summary>
        /// Traz o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static T GetPropertyValue<T, O>(this O MyObject, string Name) where O : class
        {
            return (T)MyObject.GetPropertyValue(Name, typeof(T));
        }

        /// <summary>
        /// Traz o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static object GetPropertyValue<O>(this O MyObject, string Name, Type type = null) where O : class
        {
            if (MyObject != null)
            {
                var prop = MyObject.GetProperty(Name);
                if (prop != null && prop.CanRead)
                {
                    var v = prop.GetValue(MyObject);
                    if (type != null)
                        return v.ChangeType(type);
                    return v;
                }
            }

            return default;
        }

        public static string GetPublicIP()
        {
            var IP = EmptyString;
            var z = TryExecute(() => IP = DownloadString("https://ipv4.icanhazip.com/"));
            if (z != null) WriteDebug(z);
            IP = IP.Trim().NullIf(x => !x.IsIP());
            return IP.Trim();
        }

        /// <summary>
        /// Sorteia um item da Lista
        /// </summary>
        /// <typeparam name="T">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static T GetRandomItem<T>(this T[] Array) => Array == null || Array.Length == 0 ? default : Array[RandomInt(0, Array.Length - 1)];

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this Uri URL, bool WithQueryString = true) => WithQueryString ? URL.PathAndQuery : URL.AbsolutePath;

        public static string GetRelativePath(this FileSystemInfo fromPath, FileSystemInfo toPath) => GetRelativePath(fromPath?.FullName, toPath?.FullName);

        public static string GetRelativePath(this String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this string URL, bool WithQueryString = true) => URL.IsURL() ? new Uri(URL).GetRelativeURL(WithQueryString) : null;

        public static DateTime GetLatestCompileTime(this Assembly assembly)
        {
            var filePath = assembly.Location;
            const int c_PeHeaderOffset = 60;
            const int c_LinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                stream.Read(buffer, 0, 2048);

            var offset = BitConverter.ToInt32(buffer, c_PeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + c_LinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            return linkTimeUtc;
        }

        /// <summary>
        /// Pega os bytes de um arquivo embutido no assembly
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static byte[] GetResourceBytes(this Assembly Assembly, string FileName) => Assembly.GetManifestResourceStream(FileName)?.ToBytes() ?? Array.Empty<byte>();

        public static byte[] GetResourceBytes(string FileName) => GetResourceBytes(Assembly.GetExecutingAssembly(), FileName);

        /// <summary>
        /// Pega o texto de um arquivo embutido no assembly
        /// </summary>
        /// <param name="FileName">Nome do arquivo embutido dentro do assembly (Embedded Resource)</param>
        /// <returns></returns>
        public static string GetResourceFileText(this Assembly Assembly, string FileName, bool IsFullQualifiedName = false, Encoding encoding = null)
        {
            string txt = null;
            encoding = encoding ?? Encoding.Default;
            if (Assembly != null && FileName.IsValid())
            {
                if (!IsFullQualifiedName)
                {
                    FileName = $"{Assembly.GetName().Name}.{FileName}";
                }

                using (var x = Assembly.GetManifestResourceStream(FileName))
                {
                    if (x != null)
                    {
                        using (var r = new StreamReader(x, encoding))
                        {
                            txt = r.ReadToEnd();
                        }
                    }
                    else
                    {
                        $"{FileName} not found in assembly ({Assembly.GetName()}){Environment.NewLine}Files:{Environment.NewLine}{Assembly.GetManifestResourceNames().SelectJoinString(s => $" - {s}", Environment.NewLine)}".ConsoleLog();
                    }
                }
            }

            return txt;
        }

        public static RotateFlipType GetRotateFlip(this Image Img)
        {
            var rft = RotateFlipType.RotateNoneFlipNone;
            foreach (PropertyItem p in Img.PropertyItems)
            {
                if (p.Id == 274)
                {
                    short orientation = BitConverter.ToInt16(p.Value, 0);
                    switch (orientation)
                    {
                        case 1:
                            {
                                rft = RotateFlipType.RotateNoneFlipNone;
                                break;
                            }

                        case 3:
                            {
                                rft = RotateFlipType.Rotate180FlipNone;
                                break;
                            }

                        case 6:
                            {
                                rft = RotateFlipType.Rotate90FlipNone;
                                break;
                            }

                        case 8:
                            {
                                rft = RotateFlipType.Rotate270FlipNone;
                                break;
                            }
                    }
                }
            }

            return rft;
        }

        public static T GetSingleValue<T>(this DataRow data, int ColumnIndex = 0) => data != null ? data.GetValue<T>(ColumnIndex) : default;

        public static T GetSingleValue<T>(this DataTable table, int ColumnIndex = 0) => GetSingleValue<T>(table?.GetFirstRow(), ColumnIndex);

        public static T GetSingleValue<T>(this DataSet data, int ColumnIndex = 0) => GetSingleValue<T>(data?.GetFirstRow(), ColumnIndex);

        public static T GetSingleValue<T>(this DataRow data, string ColumnNameOrIndex) => data != null ? data.GetValue<T>(ColumnNameOrIndex) : default;

        public static T GetSingleValue<T>(this DataTable table, string ColumnNameOrIndex) => GetSingleValue<T>(table?.GetFirstRow(), ColumnNameOrIndex);

        public static T GetSingleValue<T>(this DataSet data, string ColumnNameOrIndex) => GetSingleValue<T>(data?.GetFirstRow(), ColumnNameOrIndex);

        /// <summary>
        /// Return the username of most social websites like facebook, tiktok, instagram and youtube
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetSocialUsername(this string url)
        {
            if (url.IsURL())
                return GetSocialUsername(new Uri(url));
            else throw new Exception("Invalid URL");
        }

        public static string GetSocialUsername(this Uri uri)
        {
            if (uri == null) throw new ArgumentException("Uri is null", nameof(uri));
            string username = "";

            var host = uri.Host;
            var segments = uri.Segments;

            username = segments[1];

            if (host.ContainsAny("fb.com", "facebook.com"))
            {
                username = Regex.Match(uri.AbsoluteUri.Replace("fb.com", "facebook.com"), @"(?:(?:http|https):\/\/)?(?:www.)?facebook.com\/(?:(?:\words)*#!\/)?(?:pages\/)?(?:[?\words\-]*\/)?(?:profile.php\?id=(?=\d.*))?([\words\-]*)?").Groups[1].Value;
            }
            else if (host.Contains("dailymotion.com"))
            {
                username = segments[2];
            }
            else if (host.Contains("youtube.com"))
            {
                if (segments.Skip(1).Take(2).ContainsAny("user", "channel"))
                {
                    username = segments[2];
                }
            }

            return username.TrimStart('@').TrimEnd('/');
        }

        /// <summary>
        /// Corta um texto para exibir um numero máximo de caracteres ou na primeira quebra de linha.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="TextLength"></param>
        /// <param name="Ellipsis"></param>
        /// <returns></returns>
        public static string GetTextPreview(this string Text, int TextLength, string Ellipsis = "...", bool BeforeNewLine = true)
        {
            if (Text.IsNotValid() || Text?.Length <= TextLength || TextLength <= 0)
            {
                return Text;
            }
            else
            {
                if (BeforeNewLine)
                {
                    Text = Text.TrimCarriage().GetBefore(Environment.NewLine);
                    if (TextLength == 0) return Text;
                }

                return $"{Text.GetFirstChars(TextLength)}{Ellipsis ?? ""}";
            }
        }


        /// <summary>
        /// Retorna o <see cref="Type"/> do objeto mesmo se ele for nulo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>o tipo do objeto ou o prorio objeto se ele for um <see cref="Type"/></returns>
        public static Type GetTypeOf<T>(this T Obj)
        {
            if (Obj is Type istype)
            {
                return istype;
            }
            else
            {
                try
                {
                    return Obj.GetType();
                }
                catch { }
            }
            return typeof(T);
        }

        /// <summary>
        /// Retorna os segmentos de uma url
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetURLSegments(this string URL)
        {
            var p = new Regex(@"(?<!\?.+)(?<=\/)[\words-.]+(?=[/\r\n?]|$)", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase));
            var gs = p.Matches(URL);
            foreach (Match g in gs)
            {
                yield return g.Value;
            }
        }

        public static T GetValue<T>(this DataRow row, int ColumnIndex = 0)
        {
            try
            {
                return ChangeType<T>(row != null ? row[ColumnIndex] : default);
            }
            catch
            {
                return default;
            }
        }

        public static T GetValue<T>(this DataRow row, string ColumnNameOrIndex)
        {
            try
            {
                return ChangeType<T>(row != null ? row[ColumnNameOrIndex] : default);
            }
            catch
            {
                if (ColumnNameOrIndex.IsNumber())
                {
                    return GetValue<T>(row, ColumnNameOrIndex.ToInt());
                }
            }
            return default;
        }

        /// <inheritdoc cref="GetValue{T}(DataRow, string, Expression{Func{object, object}})"/>
        public static string GetValue(this DataRow row, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(row, Name, valueParser);

        /// <summary>
        /// Retorna o valor da coluna <paramref name="Name"/> de uma <see cref="DataRow"/>
        /// convertido para <typeparamref name="T"/> e previamente tratado pela função <paramref name="valueParser"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="Name"></param>
        /// <param name="valueParser"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRow row, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            try
            {
                if (row == null)
                {
                    throw new ArgumentException("Row is null", nameof(row));
                }

                object v = null;

                if (Name.IsValid() && Name.IsNotNumber())
                {
                    v = row[Name];
                }
                else
                {
                    v = row[Name.IfBlank(0)];
                }

                if (v == null || v == DBNull.Value)
                {
                    return default;
                }

                if (valueParser != null)
                {
                    v = valueParser.Compile().Invoke(v);
                }

                return typeof(T).IsEnum ? v.ToString().GetEnumValue<T>() : v.ChangeType<T>();
            }
            catch (Exception ex)
            {
                LogWriter.WriteLine(ex.ToFullExceptionString());
                return default;
            }
        }

        public static string GetValue(this DataTable Table, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(Table, Name, valueParser);

        public static string GetValue(this DataSet Data, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(Data, Name, valueParser);

        public static T GetValue<T>(this DataSet Data, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            var r = Data.GetFirstRow();
            return r == null ? default : r.GetValue<T>(Name, valueParser);
        }

        public static T GetValue<T>(this DataTable Table, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            var r = Table.GetFirstRow();
            return r == null ? default : r.GetValue<T>(Name, valueParser);
        }

        /// <summary>
        /// Tries to get a value from <see cref="Dictionary{TKey, TValue}"/>. if fails, return
        /// <paramref name="ReplaceValue"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Key"></param>
        /// <param name="ReplaceValue"></param>
        /// <remarks>
        /// if <paramref name="ReplaceValue"/> is not provided. the default value for type
        /// <typeparamref name="TValue"/> is returned
        /// </remarks>
        /// <returns></returns>
        public static TValue GetValueOr<TKey, TValue>(this IDictionary<TKey, TValue> Dic, TKey Key, TValue ReplaceValue = default) => Dic != null && Dic.ContainsKey(Key) ? Dic[Key] : ReplaceValue;

        /// <summary>
        /// Captura o Id de um video do YOUTUBE ou VIMEO em uma URL
        /// </summary>
        /// <param name="URL">URL do video</param>
        /// <returns>ChaveFormatadaTraco do video do youtube ou Vimeo</returns>
        public static string GetVideoID(this Uri URL) => GetVideoID(URL?.AbsoluteUri);

        /// <summary>
        /// Captura o Id de um video do youtube ou vimeo em uma URL
        /// </summary>
        /// <param name="URL">URL do video</param>
        /// <returns>Id do video do youtube ou vimeo</returns>
        public static string GetVideoID(this string URL)
        {
            if (URL.IsURL())
            {
                if (URL.GetDomain().ContainsAny("youtube", "youtu"))
                {
                    return Regex.Match(URL.ReplaceNone("&feature=youtu.be"), @"(?:https?:\/\/)?(?:www\.)?youtu(?:.be\/|be\.com\/watch\?v=|be\.com\/v\/)(.{8,})").Groups[1].Value;
                }
                else if (URL.GetDomain().ContainsAny("vimeo"))
                {
                    return Regex.Match(URL, @"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)").Groups[1].Value;
                }
            }

            throw new ArgumentException("Invalid Youtube or Vimeo URL", nameof(URL));
        }

        /// <summary>
        /// Retorna uma lista de palavras encontradas no texto em ordem alfabetica
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<string> GetWords(this string Text)
        {
            var txt = new List<string>();
            var palavras = Text.TrimBetween().FixHTMLBreakLines().ToLowerInvariant().RemoveHTML().Split(PredefinedArrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var w in palavras)
            {
                txt.Add(w);
            }

            return txt.Distinct().OrderBy(x => x);
        }

        /// <summary>
        /// Captura todas as sentenças que estão entre aspas ou parentesis ou chaves ou colchetes em
        /// um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string[] GetWrappedText(this string Text, string Character = DoubleQuoteChar, bool ExcludeWrapChars = true)
        {
            var lista = new List<string>();
            string regx = $"{Character.RegexEscape()}(.*?){Character.GetOppositeWrapChar().RegexEscape()}";
            var mm = new Regex(regx, (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Matches(Text);
            foreach (Match a in mm)
            {
                if (ExcludeWrapChars)
                {
                    lista.Add(a.Value.RemoveFirstEqual(Character).RemoveLastEqual(Character.GetOppositeWrapChar()));
                }
                else
                {
                    lista.Add(a.Value);
                }
            }

            return lista.ToArray();
        }

        /// <summary>
        /// Captura a Thumbnail de um video do youtube
        /// </summary>
        /// <param name="URL">Url do Youtube</param>
        /// <returns></returns>
        public static Image GetYoutubeThumbnail(string URL) => DownloadImage($"http://img.youtube.com/vi/{GetVideoID(URL)}/hqdefault.jpg");

        /// <summary>
        /// Captura a Thumbnail de um video do youtube
        /// </summary>
        /// <param name="URL">Url do Youtube</param>
        /// <returns></returns>
        public static Image GetYoutubeThumbnail(this Uri URL) => GetYoutubeThumbnail(URL?.AbsoluteUri);

        /// <summary>
        /// Converte uma Imagem para Escala de cinza
        /// </summary>
        /// <param name="img">imagem original</param>
        /// <returns></returns>
        public static Image Grayscale(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f }, new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f }, new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            return ApplyColorMatrix(copia, cm);
        }

        public static IEnumerable<HSVColor> GrayscalePallette(int Amount) => MonochromaticPallette(Color.White, Amount);

        /// <summary>
        /// Constroi uma expressão Maior que
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression GreaterThan(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.GreaterThan(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Constroi uma expressão Maior ou Igual
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression GreaterThanOrEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.GreaterThanOrEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, long> GroupAndCountBy<T, Group>(this IEnumerable<T> obj, Func<T, Group> GroupSelector) => obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, long>(x.Key, x.LongCount())).ToDictionary();

        public static Dictionary<Group, decimal> GroupAndSumBy<T, Group>(this IEnumerable<T> obj, Func<T, Group> GroupSelector, Func<T, decimal> SumSelector) => obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, decimal>(x.Key, x.Sum(SumSelector))).ToDictionary();

        /// <summary>
        /// Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada
        /// grupo a partir de outra propriedade do mesmo objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <typeparam name="Count"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <param name="CountObjectBy"></param>
        /// <returns></returns>
        public static Dictionary<Group, Dictionary<Count, long>> GroupAndCountSubGroupBy<T, Group, Count>(this IEnumerable<T> obj, Func<T, Group> GroupSelector, Func<T, Count> CountObjectBy)
        {
            var dic_of_dic = obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, Dictionary<Count, long>>(x.Key, x.GroupBy(CountObjectBy).ToDictionary(y => y.Key, y => y.LongCount()))).ToDictionary();
            dic_of_dic.Values.MergeKeys();

            return dic_of_dic;
        }

        /// <summary>
        /// Agrupa itens de uma lista a partir de duas propriedades de um objeto resultado em um
        /// grupo com subgrupos daquele objeto
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <typeparam name="SubGroup"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <param name="SubGroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, Dictionary<SubGroup, IEnumerable<Type>>> GroupAndSubGroupBy<Type, Group, SubGroup>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector, Func<Type, SubGroup> SubGroupSelector)
        {
            var dic_of_dic = obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, Dictionary<SubGroup, IEnumerable<Type>>>(x.Key, x.GroupBy(SubGroupSelector).ToDictionary(y => y.Key, y => y.AsEnumerable()))).ToDictionary();
            dic_of_dic.Values.MergeKeys();
            return dic_of_dic;
        }

        /// <summary>
        /// Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Dictionary<long, IEnumerable<T>> GroupByPage<T>(this IQueryable<T> source, int PageSize) => source.AsEnumerable().GroupByPage(PageSize);

        /// <summary>
        /// Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Dictionary<long, IEnumerable<T>> GroupByPage<T>(this IEnumerable<T> source, int PageSize) => source.Select((item, index) => new { item, Page = index / (double)PageSize.SetMinValue(1) }).GroupBy(g => g.Page.FloorLong() + 1L, x => x.item).ToDictionary();

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attribType"></param>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo target, Type attribType) => target?.GetCustomAttributes(attribType, false).Any() ?? false;

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo target) => target?.HasAttribute(typeof(T)) ?? false;

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
        /// Verifica se um diretório possui subdiretórios
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool HasDirectories(this DirectoryInfo Directory) => Directory?.GetDirectories().Any() ?? false;

        /// <summary>
        /// Verifica se um diretório possui arquivos
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool HasFiles(this DirectoryInfo Directory) => Directory?.GetFiles().Any() ?? false;

        public static bool HasLength(this string Text, int Length) => Text != null && Text.Length == Length;

        public static bool HasMaxLength(this string Text, int Length) => Text != null && Text.Length <= Length;

        public static bool HasMinLength(this string Text, int Length) => Text != null && Text.Length >= Length;

        /// <summary>
        /// Verifica se um tipo possui uma propriedade
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type Type, string PropertyName, bool GetPrivate = false)
        {
            if (Type != null && PropertyName.IsValid())
            {
                var parts = new List<string>();
                bool stop = false;
                string current = EmptyString;
                for (int i = 0, loopTo = PropertyName.Length - 1; i <= loopTo; i++)
                {
                    if (PropertyName[i] != '.')
                    {
                        current += $"{PropertyName[i]}";
                    }

                    if (PropertyName[i] == '[')
                    {
                        stop = true;
                    }

                    if (PropertyName[i] == ']')
                    {
                        stop = false;
                    }

                    if (PropertyName[i] == '.' && !stop || i == PropertyName.Length - 1)
                    {
                        parts.Add(current.ToString());
                        current = EmptyString;
                    }
                }

                PropertyInfo prop;
                string propname = parts.First().GetBefore("[");
                if (GetPrivate)
                {
                    prop = Type.GetProperty(propname, (BindingFlags)((int)BindingFlags.Public + (int)BindingFlags.NonPublic + (int)BindingFlags.Instance));
                }
                else
                {
                    prop = Type.GetProperty(propname);
                }

                bool exist = prop != null;
                parts.RemoveAt(0);
                if (exist && parts.Count > 0)
                {
                    exist = prop.PropertyType.HasProperty(parts.First(), GetPrivate);
                }

                return exist;
            }

            return false;
        }

        /// <summary>
        /// Verifica se um tipo possui uma propriedade
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool HasProperty(this object Obj, string Name) => Obj?.GetType().HasProperty(Name, true) ?? false;

        /// <summary>
        /// Check if object is a simple type.
        /// </summary>
        /// <param name="objOrType"></param>
        /// <returns></returns>
        public static bool IsSimpleType(this object objOrType)
        {
            var type = objOrType.GetNullableTypeOf();
            return new List<Type>
                           {
                               typeof(byte),
                               typeof(sbyte),
                               typeof(short),
                               typeof(ushort),
                               typeof(int),
                               typeof(uint),
                               typeof(long),
                               typeof(ulong),
                               typeof(float),
                               typeof(double),
                               typeof(decimal),
                               typeof(bool),
                               typeof(string),
                               typeof(char),
                               typeof(Guid),
                               typeof(DateTime),
                               typeof(DateTimeOffset),
                               typeof(TimeSpan),
                               typeof(byte[])
                           }.Contains(type) || type.IsEnum;
        }

        /// <summary>
        /// Verifica se um valor possui propriedades com os mesmos valores de outro objeto
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="OtherObj"></param>
        /// <returns></returns>
        public static bool HasSamePropertyValues<T, O>(this T Obj, O OtherObj)
        {
            if (Obj != null && OtherObj != null)
            {
                var props = Obj.GetNullableTypeOf().GetProperties();
                var otherProps = OtherObj.GetNullableTypeOf().GetProperties();

                foreach (var prop in props)
                {
                    if (otherProps.Any(x => x.Name == prop.Name))
                    {
                        var val = prop.GetValue(Obj);
                        var otherval = otherProps.First(x => x.Name == prop.Name).GetValue(OtherObj);
                        if (val != otherval)
                        {
                            return false;
                        }
                    }

                }
            }
            return true;
        }

        /// <summary>
        /// Hides the specified directory or file.
        /// </summary>
        /// <typeparam name="T">The type of the directory or file to hide.</typeparam>
        /// <param name="dir">The directory or file to hide.</param>
        /// <returns>The hidden directory or file.</returns>
        public static T Hide<T>(this T dir) where T : FileSystemInfo
        {
            if (dir != null && dir.Exists)
            {
                if (!dir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    dir.Attributes |= FileAttributes.Hidden;
                }
            }
            return dir;
        }

        public static bool IsHidden<T>(this T dir) where T : FileSystemInfo
        {
            return dir != null && dir.Exists && dir.Attributes.HasFlag(FileAttributes.Hidden);
        }


        /// <summary>
        /// Retorna um texto com entidades HTML convertidas para caracteres e tags BR em breaklines
        /// </summary>
        /// <param name="Text">string HTML</param>
        /// <returns>String HTML corrigido</returns>
        public static string HtmlDecode(this string Text) => WebUtility.HtmlDecode(EmptyString + Text).ReplaceMany(Environment.NewLine, "<br/>", "<br />", "<br>");

        /// <summary>
        /// Escapa o texto HTML
        /// </summary>
        /// <param name="Text">string HTML</param>
        /// <returns>String HTML corrigido</returns>
        public static string HtmlEncode(this string Text) => WebUtility.HtmlEncode(Text?.ReplaceMany("<br>", PredefinedArrays.BreakLineChars.ToArray()) ?? EmptyString);

        /// <summary>
        /// Verifica se uma variavel está vazia, em branco ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValueIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static T IfBlank<T>(this object Value, T ValueIfBlank = default) => Value.IsNotValid() ? ValueIfBlank : ChangeType<T>(Value);

        public static string BlankIfNull(this string Text) => IfBlank(Text, "");

        /// <summary>
        /// Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um
        /// valor default se o index nao existir ou seu valor for branco ou null
        /// </summary>
        /// <typeparam name="T">Tipo do IEnumerable e do valor</typeparam>
        /// <param name="Arr">Array</param>
        /// <param name="Index">Posicao</param>
        /// <param name="ValueIfBlankOrNoIndex">Valor se o mesmo nao existir</param>
        /// <returns></returns>
        public static T IfBlankOrNoIndex<T>(this IEnumerable<T> Arr, int Index, T ValueIfBlankOrNoIndex) => (Arr ?? Array.Empty<T>()).IfNoIndex(Index, ValueIfBlankOrNoIndex).IfBlank(ValueIfBlankOrNoIndex);

        /// <summary>
        /// Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um
        /// valor default se o index nao existir
        /// </summary>
        /// <typeparam name="T">Tipo do IEnumerable e do valor</typeparam>
        /// <param name="Arr">Array</param>
        /// <param name="Index">Posicao</param>
        /// <param name="ValueIfNoIndex">Valor se o mesmo nao existir</param>
        /// <returns></returns>
        public static T IfNoIndex<T>(this IEnumerable<T> Arr, int Index, T ValueIfNoIndex = default)
        {
            var item = (Arr ?? Array.Empty<T>()).ElementAtOrDefault(Index);
            return item == null ? ValueIfNoIndex : item;
        }

        /// <summary>
        /// Executa uma função para uma variavel se a mesma nao estiver em branco ( <see cref="IsBlank{T}())"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="ExpressionIfBlank"></param>
        /// <returns></returns>
        public static T IfNotBlank<T>(this T Value, Expression<Func<T, T>> ExpressionIfBlank)
        {
            if (Value.IsValid())
            {
                if (ExpressionIfBlank != null)
                {
                    try
                    {
                        return ExpressionIfBlank.Compile().Invoke(Value);
                    }
                    catch { }
                }
            }
            return Value;
        }

        /// <summary>
        /// Verifica se um array está vazio ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValuesIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static T[] IfNullOrEmpty<T>(this object[] Value, params T[] ValuesIfBlank) => Value == null || !Value.Any() ? ValuesIfBlank ?? Array.Empty<T>() : Value.ChangeArrayType<T, object>();

        /// <summary>
        /// Verifica se um array está vazio ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValuesIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static IEnumerable<T> IfNullOrEmpty<T>(this IEnumerable<object[]> Value, params T[] ValuesIfBlank) => Value != null && Value.Any() ? Value.ChangeIEnumerableType<T, object[]>() : ValuesIfBlank;

        /// <summary>
        /// Verifica se um array está vazio ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValueIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static IEnumerable<T> IfNullOrEmpty<T>(this IEnumerable<object[]> Value, IEnumerable<T> ValueIfBlank) => Value != null && Value.Any() ? Value.ChangeIEnumerableType<T, object[]>() : ValueIfBlank;

        public static int Increment(this int Num, int Inc = 1) => Num + Inc.SetMinValue(0);

        public static long Increment(this long Num, long Inc = 1) => Num + Inc.SetMinValue(0L);

        /// <summary>
        /// Inject the property values of <typeparamref name="T"/> into <see cref="String"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatString"></param>
        /// <param name="injectionObject"></param>
        /// <returns></returns>
        public static string Inject<T>(this string formatString, T injectionObject)
        {
            if (injectionObject != null)
            {
                return injectionObject.IsDictionary()
                    ? formatString.Inject(new Hashtable((IDictionary)injectionObject))
                    : formatString.Inject(GetPropertyHash(injectionObject));
            }

            return formatString;
        }

        public static string Inject(this string formatString, Hashtable attributes) => InjectBase(formatString, attributes);

        /// <summary>
        /// Inject the property values of <typeparamref name="T"/> into <paramref name="TemplatedString"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatString"></param>
        /// <param name="injectionObject"></param>
        /// <returns></returns>
        public static string InjectInto<T>(this T Obj, string TemplatedString) => TemplatedString.IfBlank(EmptyString).Inject(Obj);



        /// <summary>
        /// Inject the property values of <typeparamref name="T"/> into <see cref="String"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatString"></param>
        /// <param name="injectionObject"></param>
        /// <returns></returns>
        public static string InjectSQL<T>(this string formatString, T injectionObject)
        {
            if (injectionObject != null)
            {
                return injectionObject.IsDictionary()
                    ? formatString.InjectSQL(new Hashtable((IDictionary)injectionObject))
                    : formatString.InjectSQL(GetPropertyHash(injectionObject));
            }

            return formatString;
        }




        /// <summary>
        /// Insere uma imagem em outra imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="InsertedImage">Imagem de Marca Dagua</param>
        /// <param name="X">Posição X</param>
        /// <param name="Y">Posição Y</param>
        /// <returns></returns>
        public static Image Insert(this Image Image, Image InsertedImage, int X = -1, int Y = -1)
        {
            var bm_Resultado = new Bitmap(Image);
            var bm_marcaDagua = new Bitmap(InsertedImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5));
            if (X < 0)
                X = (bm_Resultado.Width - bm_marcaDagua.Width) / 2;
            if (Y < 0)
                Y = (bm_Resultado.Height - bm_marcaDagua.Height) / 2;
            var gr = Graphics.FromImage(bm_Resultado);
            gr.DrawImage(bm_marcaDagua, X, Y);
            return bm_Resultado;
        }

        public static string Interpolate(this string Text, params string[] Texts)
        {
            Text = Text.IfBlank(EmptyString);
            Texts = Texts ?? Array.Empty<string>();

            var s = Texts.ToList();
            s.Insert(0, Text);

            var ns = EmptyString;
            var len = s.Max(x => x.Length);
            for (int i = 0; i < len; i++)
            {
                foreach (var item in s)
                {
                    ns += item.AsEnumerable().IfNoIndex(i, WhitespaceChar.FirstOrDefault());
                }
            }

            return ns;
        }

        /// <summary>
        /// Verifica se uma palavra é um Anagrama de outra palavra
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="AnotherText"></param>
        /// <returns></returns>
        public static bool IsAnagramOf(this string Text, string AnotherText)
        {
            var char1 = Text?.ToLowerInvariant().ToCharArray() ?? Array.Empty<char>();
            var char2 = AnotherText?.ToLowerInvariant().ToCharArray() ?? Array.Empty<char>();
            Array.Sort(char1);
            Array.Sort(char2);
            string NewWord1 = new string(char1);
            string NewWord2 = new string(char2);
            return NewWord1 == NewWord2;
        }

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, params string[] Texts) => Text.IsAny(default, Texts);

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, StringComparison Comparison, params string[] Texts) => (Texts ?? Array.Empty<string>()).Any(x => Text.Equals(x, Comparison));

        public static bool IsAny<T>(this T obj, params T[] others) => others?.Any(x => x.Equals(obj)) ?? false;

        public static bool IsArray<T>(T Obj)
        {
            try
            {
                var ValueType = Obj?.GetType() ?? typeof(T);
                return !(ValueType == typeof(string)) && ValueType.IsArray; //  GetType(TEntity).IsAssignableFrom(ValueType.GetElementType())
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this Type Type) => Type == typeof(T[]);

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this object Obj) => Obj.GetTypeOf().IsArrayOf<T>();

        /// <summary>
        /// Retorna verdadeiro se identificar que a string é base64
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static bool IsBase64(this string base64String)
        {
            // Credit: oybek https://stackoverflow.com/users/794764/oybek
            if (string.IsNullOrWhiteSpace(base64String) ||
                base64String.Length % 4 != 0 ||
                base64String.Contains(" ") ||
                base64String.Contains("\t") ||
                base64String.Contains("\r") ||
                base64String.Contains("\n"))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                //ignore
            }

            return false;
        }

        public static Expression<Func<T, bool>> IsBetween<T, TV
            >(this Expression<Func<T, TV>> MinProperty, Expression<Func<T, TV>> MaxProperty, IEnumerable<TV> Values)
        {
            var exp = false.CreateWhereExpression<T>();
            foreach (var item in Values ?? Array.Empty<TV>())
            {
                exp = exp.Or(WhereExpression(MinProperty, "<", new[] { (IComparable)item }).And(WhereExpression(MaxProperty, ">", new[] { (IComparable)item })));
            }

            return exp;
        }

        public static Expression<Func<T, bool>> IsBetween<T, TV>(this Expression<Func<T, TV>> MinProperty, Expression<Func<T, TV>> MaxProperty, params TV[] Values)
               => MinProperty.IsBetween(MaxProperty, Values.AsEnumerable());

        public static Expression<Func<T, bool>> IsBetween<T, TV>(this Expression<Func<T, TV>> Property, TV MinValue, TV MaxValue)
        {
            if (MinValue.Equals(MaxValue))
            {
                return IsEqual(Property, MinValue);
            }
            else
            {
                return WhereExpression(Property, "between", new[] { (IComparable)MinValue, (IComparable)MaxValue });
            }
        }

        public static Expression<Func<T, bool>> IsBetween<T>(this Expression<Func<T, DateTime>> Property, DateRange DateRange)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, DateRange.StartDate);
            }
            else
            {
                return WhereExpression(Property, "between", new IComparable[] { DateRange.StartDate, DateRange.EndDate });
            }
        }

        public static Expression<Func<T, bool>> IsBetween<T>(this Expression<Func<T, DateTime?>> Property, DateRange DateRange)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, DateRange.StartDate);
            }
            else
            {
                return WhereExpression(Property, "between", new IComparable[] { (DateTime?)DateRange.StartDate, (DateTime?)DateRange.EndDate });
            }
        }

        /// <summary>
        /// Verifica se <paramref name="Value"/> é igual a <paramref name="MinValue"/> ou está entre
        /// <paramref name="MinValue"/> e <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <b>true</b> se <paramref name="Value"/> for igual a <paramref name="MinValue"/>.
        /// Retorna <b>false</b> se <paramref name="Value"/> for igual a <paramref
        /// name="MaxValue"/>. <br/> Utilize <see cref="IsBetweenOrEqual(IComparable, IComparable,
        /// IComparable)"/> para incluir <paramref name="MaxValue"/> ou <see
        /// cref="IsBetweenExclusive(IComparable, IComparable, IComparable)"/> para excluir
        /// <paramref name="MinValue"/>
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T Value, T MinValue, T MaxValue) where T : IComparable
        {
            FixOrder(ref MinValue, ref MaxValue);
            return MinValue.IsEqual(MaxValue) ? Value.IsEqual(MinValue) : Value.IsGreaterThanOrEqual(MinValue) && Value.IsLessThan(MaxValue);
        }

        /// <summary>
        /// Verifica se <paramref name="Value"/> está entre <paramref name="MinValue"/> e <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <see cref="false"/> se <paramref name="Value"/> for igual a <paramref
        /// name="MinValue"/> ou <paramref name="MaxValue"/>. <br/> Utilize <see
        /// cref="IsBetween(IComparable, IComparable, IComparable)"/> para incluir <paramref
        /// name="MinValue"/> ou <see cref="IsBetweenOrEqual(IComparable, IComparable,
        /// IComparable)"/> para incluir ambos
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetweenExclusive<T>(this T Value, T MinValue, T MaxValue) where T : IComparable
        {
            FixOrder(ref MinValue, ref MaxValue);
            return !MinValue.IsEqual(MaxValue) && Value.IsGreaterThan(MinValue) && Value.IsLessThan(MaxValue);
        }

        public static Expression<Func<T, bool>> IsBetweenOrEqual<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, IEnumerable<V> Values)
        {
            var exp = false.CreateWhereExpression<T>();
            foreach (var item in Values ?? Array.Empty<V>())
            {
                exp = exp.Or(WhereExpression(MinProperty, "<=", new[] { (IComparable)item }).And(WhereExpression(MaxProperty, ">=", new[] { (IComparable)item })));
            }

            return exp;
        }

        public static Expression<Func<T, bool>> IsBetweenOrEqual<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, params V[] Values)
               => MinProperty.IsBetweenOrEqual(MaxProperty, Values.AsEnumerable());

        public static Expression<Func<T, bool>> IsBetweenOrEqual<T, V>(this Expression<Func<T, V>> Property, V MinValue, V MaxValue)
        {
            if (MinValue.Equals(MaxValue))
            {
                return IsEqual(Property, MinValue);
            }
            else
            {
                return WhereExpression(Property, "betweenorequal", new IComparable[] { (IComparable)MinValue, (IComparable)MaxValue });
            }
        }

        /// <summary>
        /// Verifica se <paramref name="Value"/> é igual ou está entre <paramref name="MinValue"/> e
        /// <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <b>true</b> se <paramref name="Value"/> for igual a <paramref name="MinValue"/>
        /// ou <paramref name="MaxValue"/>. <br/> Utilize <see cref="IsBetween(IComparable,
        /// IComparable, IComparable)"/> para excluir <paramref name="MaxValue"/> ou <see
        /// cref="IsBetweenExclusive(IComparable, IComparable, IComparable)"/> para excluir ambos
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetweenOrEqual<T>(this T Value, T MinValue, T MaxValue) where T : IComparable
        {
            FixOrder(ref MinValue, ref MaxValue);
            return Value.IsGreaterThanOrEqual(MinValue) && Value.IsLessThanOrEqual(MaxValue);
        }


        public static bool IsBlankOrZero(this string text) => text.IsBlank() || (text.IsNumber() && text.ToInt() == 0);
        public static bool IsBlank(this string text) => text.IsNotValid();
        public static bool IsNotBlank(this string text) => text.IsValid();

        /// <summary>
        /// Verifica se o valor não é válido.
        /// </summary>
        /// <param name="Value">O valor a ser verificado.</param>
        /// <returns>True se o valor não for válido, caso contrário, False.</returns>
        public static bool IsNotValid(this object Value)
        {
            try
            {
                if (Value != null)
                {
                    var tipo = Value.GetNullableTypeOf();

                    if (tipo.IsNumericType())
                    {
                        return Value.ChangeType<decimal>() == 0;
                    }
                    else if (Value is FormattableString fs)
                    {
                        return IsNotValid($"{fs}".ToUpperInvariant());
                    }
                    else if (Value is bool b)
                    {
                        return !b;
                    }
                    else if (Value is string s)
                    {
                        return string.IsNullOrWhiteSpace($"{s}".RemoveAny(PredefinedArrays.BreakLineChars.ToArray()));
                    }
                    else if (Value is char c)
                    {
                        return string.IsNullOrWhiteSpace($"{c}".RemoveAny(PredefinedArrays.BreakLineChars.ToArray()));
                    }
                    else if (Value is DateTime time)
                    {
                        return time.Equals(DateTime.MinValue);
                    }
                    else if (Value is TimeSpan span)
                    {
                        return span.Equals(TimeSpan.MinValue);
                    }
                    else if (Value is DateTimeOffset off)
                    {
                        return off.Equals(DateTimeOffset.MinValue);
                    }
                    else if (Value is IDictionary dic)
                    {
                        foreach (DictionaryEntry item in dic)
                        {
                            if (item.Value.IsValid())
                            {
                                return false;
                            }
                        }
                    }
                    else if (Value.IsEnumerableNotString() && Value is IEnumerable enumerable)
                    {
                        foreach (object item in enumerable)
                        {
                            if (item.IsValid())
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebug(ex);
            }
            return true;
        }

        /// <summary>
        /// Verifica se uma String está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>TRUE se estivar vazia ou em branco, caso contrario FALSE</returns>
        public static bool IsBlank(this FormattableString Text) => Text == null || $"{Text}".IsNotValid();

        public static bool IsBool<T>(this T Obj) => GetNullableTypeOf(Obj) == typeof(bool) || $"{Obj}".ToLowerInvariant().IsIn("true", "false");



        public static bool IsCloseWrapChar(this string Text) => Text.GetFirstChars().IsIn(PredefinedArrays.CloseWrappers);

        public static bool IsCloseWrapChar(this char c) => IsCloseWrapChar($"{c}");

        /// <summary>
        /// Verifica se o computador está conectado com a internet
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected(string Test = "http://google.com")
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(Test))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }


        public static bool IsCrossLikeAny(this string Text, IEnumerable<string> Patterns) => (Patterns ?? Array.Empty<string>()).Any(x => Like(Text.IfBlank(EmptyString), x) || Like(x, Text));

        /// <summary>
        /// Verifica se uma cor é escura
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <returns></returns>
        public static bool IsDark(this Color TheColor) => new HSVColor(TheColor).IsDark();

        /// <summary>
        /// Retorna TRUE se o texto for um dataurl valido
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool IsDataURL(this string Text)
        {
            try
            {
                return new Web.DataURI(Text).ToString().IsValid();
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDate(this string Obj)
        {
            try
            {
                return DateTime.TryParse(Obj, out _);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDate<T>(this T Obj) => GetNullableTypeOf(Obj) == typeof(DateTime) || $"{Obj}".IsDate();

        public static bool IsNameValueCollection(this object obj)
        {
            return obj is NameValueCollection;
        }

        /// <summary>
        /// Verifica se o objeto é um iDictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDictionary(this object obj) => IsGenericOf(obj, typeof(IDictionary<,>)) || IsGenericOf(obj, typeof(IDictionary));

        /// <summary>
        /// Verifica se uma string é um caminho de diretório válido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>TRUE se o caminho for válido</returns>
        public static bool IsDirectoryPath(this string Text)
        {
            if (Text.IsNotValid())
            {
                return false;
            }

            Text = Text.Trim();
            try
            {
                if (Directory.Exists(Text))
                {
                    return true;
                }

                if (File.Exists(Text))
                {
                    return false;
                }
            }
            catch { }

            try
            {
                // if has trailing slash then it's a directory

                if (new string[] { Convert.ToString(Path.DirectorySeparatorChar, CultureInfo.InvariantCulture), Convert.ToString(Path.AltDirectorySeparatorChar, CultureInfo.InvariantCulture) }.Any(x => Text.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
                // ends with slash if has extension then its a file; directory otherwise
                return Path.GetExtension(Text).IsNotValid();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates if a string is a valid domain name.
        /// </summary>
        /// <param name="Text">The string to validate.</param>
        /// <returns>True if the string is a valid domain name, otherwise false.</returns>
        public static bool IsDomain(this string Text) => new Regex(@"^([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,}$", RegexOptions.IgnoreCase).IsMatch(Text) && $"http://{Text}".IsURL();

        /// <summary>
        /// Verifica se um texto é uma lista de emails separados por virgula, ponto e virgula ou espaço
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool IsMultiEmail(this string Text) => Text.SplitAny(" ", ",", ";").All(x => x.IsEmail());


        /// <summary>
        /// Verifica se um determinado texto é um email
        /// </summary>
        /// <param name="Text">Texto a ser validado</param>
        /// <returns>TRUE se for um email, FALSE se não for email</returns>
        public static bool IsEmail(this string Text)
        {
            // Trim any leading/trailing spaces
            string trimmedEmail = Text?.Trim();

            // Check for whitespace or an empty string
            if ((trimmedEmail).IsNotValid())
                return false;

            // Use a regular expression to validate the email format This pattern checks for basic
            // email structure You can enhance it further based on your requirements
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(trimmedEmail, pattern);
        }

        /// <summary>
        /// Verifica se um diretório está vazio
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool IsEmpty(this DirectoryInfo Directory) => !Directory.HasFiles() && !Directory.HasDirectories();

        /// <summary>
        /// Verifica se o objeto é um enumeravel (lista)
        /// </summary>
        /// <param name="obj"></param>
        /// <remarks>NÃO considera strings (IEnumerable{char}) como true</remarks>
        /// <returns></returns>
        public static bool IsEnumerableNotString(this object obj) => IsEnumerable(obj) && GetTypeOf(obj) != typeof(string);

        /// <summary>
        /// Verifica se o objeto é um enumeravel (lista)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsEnumerable(this object obj) => IsGenericOf(obj, typeof(IEnumerable<>)) || IsGenericOf(obj, typeof(IEnumerable));

        public static Expression<Func<T, bool>> IsEqual<T, V>(this Expression<Func<T, V>> Property, V Value) => WhereExpression(Property, "equal", new[] { (IComparable)Value });

        public static bool IsEqual<T>(this T Value, T EqualsToValue) where T : IComparable => Value.Equals(EqualsToValue);

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this decimal Value) => Value % 2m == 0m;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this int Value) => Value % 2 == 0;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this short Value) => Value % 2 == 0;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this float Value) => Value % 2f == 0f;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this long Value) => Value % 2L == 0L;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this double Value) => Value % 2d == 0d;


        /// <summary>
        /// Verifica se uma string é um caminho de arquivo válido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>TRUE se o caminho for válido</returns>
        public static bool IsFilePath(this string Text)
        {
            if (Text.IsNotValid())
            {
                return false;
            }

            Text = Text.Trim();
            try
            {
                if (Directory.Exists(Text))
                {
                    return false;
                }

                if (File.Exists(Text))
                {
                    return true;
                }
            }
            catch
            {
            }
            try
            {
                // if has extension then its a file; directory otherwise
                return !Text.EndsWith(Convert.ToString(Path.DirectorySeparatorChar, CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase) && Path.GetExtension(Text).IsValid();
            }
            catch { return false; }
        }

        public static bool IsGenericOf(this object obj, Type GenericType)
        {
            var type = obj.GetTypeOf();

            if (type == null || GenericType == null) return false;
            if (type == GenericType) return true;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == GenericType) return true;
            if (GenericType.IsGenericType && GenericType.GetGenericTypeDefinition().IsAssignableFrom(type)) return true;
            if (GenericType.IsAssignableFrom(type)) return true;
            if (type.GetInterfaces().Append(type).Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == GenericType)) return true;
            return false;
        }

        /// <summary>
        /// Verifica se um valor de tipo generico é maior que outro
        /// </summary>
        /// <returns></returns>
        public static bool IsGreaterThan<T>(this T Value, T MinValue) where T : IComparable => Value.CompareTo(MinValue) > 0;

        public static bool IsGreaterThanOrEqual<T>(this T Value, T MinValue) where T : IComparable => Value.IsGreaterThan(MinValue) || Value.IsEqual(MinValue);

        public static bool IsGuid(this string value)
        {
            return Guid.TryParse(value, out _);
        }

        public static bool IsHexaDecimalColor(this string Text)
        {
            Text = Text.RemoveFirstEqual("#");
            var myRegex = new Regex("^[a-fA-F0-9]+$");
            return Text.IsValid() && myRegex.IsMatch(Text);
        }

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<T>(this T Obj, params T[] List) => Obj.IsIn((List ?? Array.Empty<T>()).ToList());

        public static bool IsIn<T>(this T Obj, IEqualityComparer<T> Comparer = null, params T[] List) => Obj.IsIn((List ?? Array.Empty<T>()).ToList(), Comparer);

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<T>(this T Obj, IEnumerable<T> List, IEqualityComparer<T> Comparer = null) => Comparer is null ? List.Contains(Obj) : List.Contains(Obj, Comparer);

        public static bool IsIn<T>(this T Obj, string Text, StringComparison? Comparer = null) => Comparer == null ? Text?.Contains(Obj.ToString()) ?? false : Text?.Contains(Obj.ToString(), Comparer.Value) ?? false;

        /// <summary>
        /// Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsInAny<T>(this T Obj, IEnumerable<T>[] List, IEqualityComparer<T> Comparer = null) => (List ?? Array.Empty<IEnumerable<T>>()).Any(x => Obj.IsIn(x, Comparer));

        public static Expression<Func<T, bool>> IsInDateRange<T>(this Expression<Func<T, DateTime>> Property, DateRange DateRange, DateRangeFilterBehavior? FilterBehavior = null)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, DateRange.StartDate);
            }

            var icomp = new IComparable[] { DateRange.StartDate, DateRange.EndDate };
            switch (FilterBehavior ?? DateRange.FilterBehavior)
            {
                case DateRangeFilterBehavior.BetweenExclusive: return WhereExpression(Property, "between", icomp);
                case DateRangeFilterBehavior.Between: return WhereExpression(Property, "between", icomp).Or(WhereExpression(Property, "equal", new IComparable[] { DateRange.StartDate }));
                case DateRangeFilterBehavior.BetweenOrEqual:
                default: return WhereExpression(Property, "betweenorequal", icomp);
            }
        }

        public static Expression<Func<T, bool>> IsInDateRange<T>(this Expression<Func<T, DateTime?>> Property, DateRange DateRange, DateRangeFilterBehavior? FilterBehavior = null)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, (DateTime?)DateRange.StartDate);
            }

            var icomp = new IComparable[] { (DateTime?)DateRange.StartDate, (DateTime?)DateRange.EndDate };
            switch (FilterBehavior ?? DateRange.FilterBehavior)
            {
                case DateRangeFilterBehavior.BetweenExclusive: return WhereExpression(Property, "between", icomp);
                case DateRangeFilterBehavior.Between: return WhereExpression(Property, "between", icomp).Or(IsEqual(Property, (DateTime?)DateRange.StartDate));
                case DateRangeFilterBehavior.BetweenOrEqual:
                default: return WhereExpression(Property, "betweenorequal", icomp);
            }
        }

        public static bool IsInUse(this FileInfo File)
        {
            //Try-Catch so we dont crash the program and can check the exception
            try
            {
                if (File.Exists)
                {
                    using (FileStream fileStream = System.IO.File.Open(File.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileStream?.Close();
                    }
                }
            }
            catch (IOException ex)
            {
                //THE FUNKY MAGIC - TO SEE IF THIS FILE REALLY IS LOCKED!!!

                int errorCode = Marshal.GetHRForException(ex) & ((1 << 16) - 1);

                if (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION)
                {
                    return true;
                }
            }
            finally
            { }
            return false;
        }

        /// <summary>
        /// Verifica se a string é um endereço IP válido
        /// </summary>
        /// <param name="IP">Endereco IP</param>
        /// <returns>TRUE ou FALSE</returns>
        public static bool IsIP(this string IP) => IP.IsValid() && Regex.IsMatch(IP, @"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$\b");

        public static bool IsLastIndex<T>(this int index, IEnumerable<T> list) => list.IsLastIndex(index);

        public static bool IsLastIndex<T>(this IEnumerable<T> list, int index) => index == list.Count() - 1;

        public static bool IsLessThan<T>(this T Value, T MaxValue) where T : IComparable => Value.CompareTo(MaxValue) < 0;

        public static bool IsLessThanOrEqual<T>(this T Value, T MaxValue) where T : IComparable => Value.IsLessThan(MaxValue) || Value.IsEqual(MaxValue);

        /// <summary>
        /// Verifica se uma cor é clara
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <returns></returns>
        public static bool IsLight(this Color TheColor) => !TheColor.IsDark();

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, IEnumerable<string> Patterns) => (Patterns ?? Array.Empty<string>()).Any((Func<string, bool>)(x => Like(Text.IfBlank(EmptyString), x)));

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, params string[] Patterns) => Text.IsLikeAny((Patterns ?? Array.Empty<string>()).AsEnumerable());

        /// <summary>
        /// Verifica se o objeto é uma lista
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsList(this object obj) => IsGenericOf(obj, typeof(List<>));

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se nenhuma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, params string[] Texts) => !Text.IsAny(Texts);

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, StringComparison Comparison, params string[] Texts) => !Text.IsAny(Comparison, Texts);

        /// <summary>
        /// Verifica se uma String não está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>FALSE se estiver nula, vazia ou em branco, caso contrario TRUE</returns>
        public static bool IsValid(this object Value) => !IsNotValid(Value);

        /// <summary>
        /// Verifica se uma String não está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>FALSE se estiver nula, vazia ou em branco, caso contrario TRUE</returns>
        public static bool IsNotBlank(this FormattableString Text) => Text != null && IsValid(FormattableString.Invariant(Text));

        /// <summary>
        /// Verifica se um diretório não está vazio
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool IsNotEmpty(this DirectoryInfo Directory) => !Directory.IsEmpty();

        /// <summary>
        /// Verifica se o não objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsNotIn<T>(this T Obj, IEnumerable<T> List, IEqualityComparer<T> Comparer = null) => !Obj.IsIn(List, Comparer);

        /// <summary>
        /// Verifica se o objeto não existe dentro de um texto
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="TExt">Texto</param>
        /// <returns></returns>
        public static bool IsNotIn<T>(this T Obj, string Text, StringComparison? Comparer = null) => !Obj.IsIn(Text, Comparer);

        /// <summary>
        /// Checks if a <paramref name="List"/> is not <b>null</b> and contains at least one item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> List) => (List ?? Array.Empty<T>()).Any();

        /// <summary>
        /// Verifica se o valor não é um numero
        /// </summary>
        /// <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
        /// <returns>FALSE se for um numero, TRUE se não for um numero</returns>
        public static bool IsNotNumber(this object Value) => !Value.IsNumber();

        public static bool IsNullableType(this Type t) => t != null && t.IsGenericType && Nullable.GetUnderlyingType(t) != null;

        public static bool IsNullableType<T>(this T Obj) => IsNullableType(Obj.GetTypeOf());

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<T>(this object Obj) => Obj.IsNullableTypeOf(typeof(T));

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<T>(this T Obj, Type Type) => Obj.GetNullableTypeOf() == Type.GetNullableTypeOf();

        /// <summary>
        /// Checks if a <paramref name="List"/> is <b>null</b> or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> List) => !List.IsNotNullOrEmpty();

        /// <summary>
        /// Verifica se o valor é um numero
        /// </summary>
        /// <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
        /// <returns>TRUE se for um numero, FALSE se não for um numero</returns>
        public static bool IsNumber(this object Value)
        {
            try
            {
                if ($"{Value}".IsValid() && $"{Value}".Trim().ToCharArray().All(x => char.IsNumber(x)))
                {
                    return true;
                }
                Convert.ToDecimal(Value, CultureInfo.InvariantCulture);
                return Value != null && $"{Value}".IsIP() == false && ((Value.GetType() == typeof(DateTime)) == false);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se o objeto é do tipo numérico. Veja <see cref="PredefinedArrays.NumericTypes"/>
        /// </summary>
        /// <remarks>Boolean is not considered numeric.</remarks>
        public static bool IsNumericType<T>(this T Obj) => Obj.GetNullableTypeOf().IsIn(PredefinedArrays.NumericTypes);

        /// <summary>
        /// Verifica se o numero é uma potencia de 2
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(this int x) => (x != 0) && ((x & (x - 1)) == 0);

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this decimal Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this int Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this long Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this short Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this float Value) => !Value.IsEven();


        /// <summary>
        /// Retorna o caractere de encapsulamento oposto ao caractere indicado
        /// </summary>
        /// <param name="Text">Caractere</param>
        /// <returns></returns>
        public static bool IsOpenWrapChar(this string Text) => Text.GetFirstChars().IsIn(PredefinedArrays.OpenWrappers);

        public static bool IsOpenWrapChar(this char c) => IsOpenWrapChar($"{c}");

        /// <summary>
        /// Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da
        /// esqueda para direita
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="IgnoreWhiteSpaces">Ignora os espaços na hora de comparar</param>
        /// <returns></returns>
        public static bool IsPalindrome(this string Text, bool IgnoreWhiteSpaces = true)
        {
            Text = Text ?? EmptyString;
            if (IgnoreWhiteSpaces)
            {
                Text = Text.RemoveAny(WhitespaceChar);
            }

            return Text == Text.Reverse().SelectJoinString();
        }

        /// <summary>
        /// Verifica se uma string é um caminho de arquivo ou diretório válido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>TRUE se o caminho for válido</returns>
        public static bool IsPath(this string Text) => Text.IsDirectoryPath() || Text.IsFilePath();

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
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsTypeOf<T>(this object Obj) => Obj.IsTypeOf(typeof(T));

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsTypeOf<T>(this T Obj, object Type) => Obj.GetTypeOf() == Type.GetTypeOf();

        /// <summary>
        /// Verifica se um determinado texto é uma URL válida
        /// </summary>
        /// <param name="Text">Texto a ser verificado</param>
        /// <returns>TRUE se for uma URL, FALSE se não for uma URL válida</returns>
        public static bool IsURL(this string Text) => Text.IsValid() && Uri.TryCreate(Text.Trim(), UriKind.Absolute, out _) && !Text.Trim().Contains(" ");

        /// <summary>
        /// Verifica se o dominio é válido (existe) em uma URL ou email
        /// </summary>
        /// <param name="DomainOrEmail">Uma String contendo a URL ou email</param>
        /// <returns>TRUE se o dominio existir, FALSE se o dominio não existir</returns>
        /// <remarks>Retornara sempre false quando nao houver conexao com a internet</remarks>
        public static bool IsValidDomain(this string DomainOrEmail)
        {
            IPHostEntry ObjHost;
            if (DomainOrEmail.IsEmail() == true)
            {
                DomainOrEmail = "http://" + DomainOrEmail.GetAfter("@");
            }
            if (DomainOrEmail.IsURL())
            {
                try
                {
                    string HostName = new Uri(DomainOrEmail).Host;
                    ObjHost = Dns.GetHostEntry(HostName);
                    return (ObjHost?.HostName ?? EmptyString) == (HostName ?? EmptyString);
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Verifica se um numero é um EAN válido
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static bool IsValidEAN(this string Code)
        {
            if (Code == null || Code.IsNotNumber() || Code.Length < 3)
            {
                return false;
            }

            var bar = Code.RemoveLastChars();
            var ver = Code.GetLastChars();
            return GenerateBarcodeCheckSum(bar) == ver;
        }

        public static bool IsValidGTIN(this string Code)
        {
            if (Code.FlatEqual("SEM GTIN"))
            {
                return true;
            }

            return Code.IsValidEAN();
        }

        public static bool IsValidEAN(this int Code) => Code.ToString(CultureInfo.InvariantCulture).PadLeft(12, '0').ToString().IsValidEAN();





        public static bool IsVisible<T>(this T info) where T : FileSystemInfo => info != null && info.Exists && info.Attributes.HasFlag(FileAttributes.Hidden) == false;

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

        public static bool IsWrapped(this string Text) => PredefinedArrays.OpenWrappers.Any(x => IsWrapped(Text, x.FirstOrDefault()));

        public static bool IsWrapped(this string Text, string OpenWrapText, string CloseWrapText = null) => IsWrapped(Text, StringComparison.CurrentCultureIgnoreCase, OpenWrapText, CloseWrapText);

        public static bool IsWrapped(this string Text, StringComparison stringComparison, string OpenWrapText, string CloseWrapText = null)
        {
            if (Text.IsValid())
            {
                OpenWrapText = OpenWrapText.IfBlank("");
                CloseWrapText = CloseWrapText.IfBlank("");
                if (OpenWrapText.Length == 1 && (CloseWrapText.IsNotValid() || CloseWrapText.Length == 1))
                {
                    return CloseWrapText.IsNotValid()
                        ? IsWrapped(Text, OpenWrapText.FirstOrDefault())
                        : IsWrapped(Text, OpenWrapText.FirstOrDefault(), CloseWrapText.FirstOrDefault());
                }
                else
                    return Text.StartsWith(OpenWrapText, stringComparison) && Text.EndsWith(CloseWrapText, stringComparison);
            }
            return false;
        }

        public static bool IsWrapped(this string Text, char OpenWrapChar) => IsWrapped(Text, OpenWrapChar, OpenWrapChar.GetOppositeWrapChar());

        public static bool IsWrapped(this string Text, char OpenWrapChar, char CloseWrapChar)
        {
            Text = Text?.Trim() ?? "";
            OpenWrapChar = OpenWrapChar.IsCloseWrapChar() ? OpenWrapChar.GetOppositeWrapChar() : OpenWrapChar;
            return Text.FirstOrDefault() == OpenWrapChar && Text.LastOrDefault() == CloseWrapChar;
        }

        /// <summary>
        /// Retorna o ultimo objeto de uma lista ou um objeto especifico se a lista estiver vazia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="Alternate"></param>
        /// <returns></returns>
        public static T LastOr<T>(this IEnumerable<T> source, params T[] Alternate) => source?.Any() ?? false ? source.Last() : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

        public static T LastOr<T>(this IEnumerable<T> source, Func<T, bool> predicade, params T[] Alternate) => source?.Any(predicade) ?? false ? source.Last(predicade) : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

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

        public static HSVColor Lerp(this HSVColor FromColor, HSVColor ToColor, float Amount) => new HSVColor(Lerp(FromColor.ToDrawingColor(), ToColor.ToDrawingColor(), Amount));

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
        /// Constroi uma expressão Menor que
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression LessThan(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.LessThan(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Constroi uma expressão Menor ou igual
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression LessThanOrEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.LessThanOrEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Computa a distancia de Levenshtein entre 2 strings. Distancia Levenshtein representa um
        /// numero de operações de acréscimo, remoção ou substituição de caracteres para que uma
        /// string se torne outra
        /// </summary>
        public static int LevenshteinDistance(this string Text1, string Text2)
        {
            Text1 = Text1 ?? EmptyString;
            Text2 = Text2 ?? EmptyString;
            int n = Text1.Length;
            int m = Text2.Length;
            var d = new int[n + 1 + 1, m + 1 + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0, loopTo = n; i <= loopTo; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0, loopTo1 = m; j <= loopTo1; j++)
            {
                d[0, j] = j;
            }

            // Step 3
            for (int i = 1, loopTo2 = n; i <= loopTo2; i++)
            {
                // Step 4
                for (int j = 1, loopTo3 = m; j <= loopTo3; j++)
                {
                    // Step 5
                    int cost = Text2[j - 1] == Text1[i - 1] ? 0 : 1;
                    // Step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static IEnumerable<(string, string, int)> LevenshteinDistanceList(this IEnumerable<string> Words)
        {
            var words = Words.ToArray() ?? Array.Empty<string>();
            int n = words.Length;

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    yield return (words[i], words[j], LevenshteinDistance(words[i], words[j]));
                }
            }
        }

        /// <summary>
        /// compara 2 strings usando wildcards
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public static bool Like(this string source, string Pattern) => new Like(Pattern).Matches(source);

        public static int LimitIndex<T>(this int ii, IEnumerable<T> Collection) => ii.LimitRange<int>(0, Collection.Count() - 1);

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



        public static StructuredText LoremIpsum(int ParagraphCount = 5, int SentenceCount = 3, int MinWordCount = 10, int MaxWordCount = 50, int IdentSize = 0, int BreakLinesBetweenParagraph = 0, string[] Words = null)
        {
            var sb = new StringBuilder();
            if (Words == null || Words.Length == 0)
                Words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua", "Ut", "enim", "ad", "minim", "veniam", "quis", "nostrud", "exercitation", "ullamco", "laboris", "nisi", "ut", "aliquip", "ex", "ea", "commodo", "consequat", "Duis", "aute", "irure", "dolor", "in", "reprehenderit", "in", "voluptate", "velit", "esse", "cillum", "dolore", "eu", "fugiat", "nulla", "pariatur" };

            for (int i = 0; i < ParagraphCount.SetMinValue(1); i++)
            {
                sb.Append(WhitespaceChar.Repeat(IdentSize));
                for (int j = 0; j < SentenceCount.SetMinValue(1); j++)
                {
                    for (int k = 0; k < RandomInt(MinWordCount.SetMinValue(1), MaxWordCount.SetMinValue(1)); k++)
                    {
                        string word = Words.RandomItem();
                        if (k == 0)
                        {
                            word = word.ToSentenceCase();
                        }
                        sb.Append(word + WhitespaceChar);
                    }
                    sb.Append(PredefinedArrays.EndOfSentencePunctuation.RandomItem());
                }
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine.Repeat(BreakLinesBetweenParagraph)); // Add more newline character between paragraphs
            }

            return new StructuredText(sb.ToString());
        }

        /// <summary>
        /// Escurece a cor mesclando ela com preto
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <param name="percent">porcentagem de mesclagem</param>
        /// <returns></returns>
        public static Color MakeDarker(this Color TheColor, float Percent = 50f) => TheColor.MergeWith(Color.Black, Percent);

        public static Image MakeDarker(this Image img, float percent = 50f)
        {
            var lockedBitmap = new Bitmap(img);
            for (int y = 0, loopTo = lockedBitmap.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = lockedBitmap.Width - 1; x <= loopTo1; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    if (!oldColor.CompareARGB(true, Color.Transparent, Color.Black, Color.White))
                    {
                        var newColor = oldColor.MakeDarker(percent);
                        lockedBitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            return lockedBitmap;
        }

        /// <summary>
        /// Clareia a cor misturando ela com branco
        /// </summary>
        /// <param name="TheColor">Cor</param>
        /// <param name="percent">Porcentagem de mesclagem</param>
        /// <returns></returns>
        public static Color MakeLighter(this Color TheColor, float Percent = 50f) => TheColor.MergeWith(Color.White, Percent);

        public static Image MakeLighter(this Image img, float percent = 50f)
        {
            var lockedBitmap = new Bitmap(img);
            for (int y = 0, loopTo = lockedBitmap.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = lockedBitmap.Width - 1; x <= loopTo1; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    if (!oldColor.CompareARGB(true, Color.Transparent, Color.Black, Color.White))
                    {
                        var newColor = oldColor.MakeLighter(percent);
                        lockedBitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            return lockedBitmap;
        }

        /// <summary>
        /// Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um
        /// dicionario ela será substituida
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstDictionary"></param>
        /// <param name="Dictionaries"></param>
        /// <returns></returns>
        public static Dictionary<T, object> MergeReplace<T>(this Dictionary<T, object> FirstDictionary, params Dictionary<T, object>[] Dictionaries)
        {
            var result = new Dictionary<T, object>();
            if (FirstDictionary != null && FirstDictionary.IsNotNullOrEmpty())
                foreach (var entry in FirstDictionary)
                {
                    result[entry.Key] = entry.Value;
                }

            foreach (var dic in Dictionaries)
            {
                foreach (var entry in dic)
                    result[entry.Key] = entry.Value;
            }
            return result;
        }


        /// <summary>
        /// Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um
        /// dicionario os valores sao agrupados em arrays
        /// </summary>
        /// <typeparam name="T">Tipo da Key, Deve ser igual para todos os dicionarios</typeparam>
        /// <param name="FirstDictionary">Dicionario Principal</param>
        /// <param name="Dictionaries">Outros dicionarios</param>
        /// <returns></returns>
        public static Dictionary<T, object> Merge<T>(this Dictionary<T, object> FirstDictionary, params Dictionary<T, object>[] Dictionaries)
        {
            // dicionario que está sendo gerado a partir dos outros
            var result = new Dictionary<T, object>();

            Dictionaries = Dictionaries ?? Array.Empty<Dictionary<T, object>>();
            // adiciona o primeiro dicionario ao array principal e exclui dicionarios vazios
            Dictionaries = Dictionaries.Union(new[] { FirstDictionary }).Where(x => x.Count > 0).ToArray();

            // cria um array de keys unicas a partir de todos os dicionarios
            var keys = Dictionaries.SelectMany(x => x.Keys.ToArray()).Distinct();

            // para cada chave encontrada
            foreach (var key in keys)
            {
                // para cada dicionario a ser mesclado
                foreach (var dic in Dictionaries)
                {
                    // dicionario tem a chave?
                    if (dic.ContainsKey(key))
                    {
                        // resultado ja tem a chave atual adicionada?
                        if (result.ContainsKey(key))
                        {
                            // lista que vai mesclar tudo
                            var lista = new List<object>();

                            // chave do resultado é um array?
                            if (IsArray(result[key]))
                            {
                                lista.AddRange((IEnumerable<object>)result[key]);
                            }
                            else
                            {
                                lista.Add(result[key]);
                            }
                            // chave do dicionario é um array?
                            if (IsArray(dic[key]))
                            {
                                lista.AddRange((IEnumerable<object>)dic[key]);
                            }
                            else
                            {
                                lista.Add(dic[key]);
                            }

                            // transforma a lista em um resultado
                            if (lista.Count > 0)
                            {
                                if (lista.Count > 1)
                                {
                                    result[key] = lista.ToArray();
                                }
                                else
                                {
                                    result[key] = lista.First();
                                }
                            }
                        }

                        else
                        {
                            result[key] = dic[key];
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="Collections"></param>
        /// <returns></returns>
        public static NameValueCollection Merge(this IEnumerable<NameValueCollection> Collections)
        {
            Collections = Collections ?? new List<NameValueCollection>();
            var all = new NameValueCollection();
            foreach (var i in Collections)
            {
                all.Add(i);
            }

            return all;
        }

        /// <summary>
        /// Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="OtherCollections"></param>
        /// <returns></returns>
        public static NameValueCollection Merge(this NameValueCollection FirstCollection, params NameValueCollection[] OtherCollections)
        {
            OtherCollections = OtherCollections ?? Array.Empty<NameValueCollection>();
            OtherCollections = new[] { FirstCollection }.Union(OtherCollections).ToArray();
            return OtherCollections.Merge();
        }

        /// <summary>
        /// Aplica as mesmas keys a todos os dicionarios de uma lista
        /// </summary>
        /// <typeparam name="TKey">Tipo da key</typeparam>
        /// <typeparam name="TValue">Tipo do Valor</typeparam>
        /// <param name="Dics">Dicionarios</param>
        /// <param name="AditionalKeys">
        /// Chaves para serem incluidas nos dicionários mesmo se não existirem em nenhum deles
        /// </param>
        public static IEnumerable<Dictionary<TKey, TValue>> MergeKeys<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> Dics, params TKey[] AditionalKeys)
        {
            AditionalKeys = AditionalKeys ?? Array.Empty<TKey>();
            Dics = Dics ?? Array.Empty<Dictionary<TKey, TValue>>();
            var chave = Dics.SelectMany(x => x.Keys).Distinct().Union(AditionalKeys);
            foreach (var dic in Dics)
            {
                if (dic != null)
                    foreach (var key in chave)
                    {
                        if (!dic.ContainsKey(key))
                        {
                            dic[key] = default;
                        }
                    }
            }
            return Dics.Select(x => x.OrderBy(y => y.Key).ToDictionary());
        }

        /// <summary>
        /// Mescla duas cores a partir de uma porcentagem
        /// </summary>
        /// <param name="TheColor">Cor principal</param>
        /// <param name="AnotherColor">Cor de mesclagem</param>
        /// <param name="Percent">Porcentagem de mescla</param>
        /// <returns></returns>
        public static Color MergeWith(this Color TheColor, Color AnotherColor, float Percent = 50f) => TheColor.Lerp(AnotherColor, Percent / 100f);

        /// <summary>
        /// Mescla duas cores a partir de uma porcentagem
        /// </summary>
        /// <param name="TheColor">Cor principal</param>
        /// <param name="AnotherColor">Cor de mesclagem</param>
        /// <param name="Percent">Porcentagem de mescla</param>
        /// <returns></returns>
        public static HSVColor MergeWith(this HSVColor TheColor, HSVColor AnotherColor, float Percent = 50f) => TheColor.Lerp(AnotherColor, Percent / 100f);

        /// <summary>
        /// Minifica uma folha de estilo CSS
        /// </summary>
        /// <param name="CSS">String contendo o CSS</param>
        /// <returns></returns>
        public static string MinifyCSS(this string CSS, bool PreserveComments = false)
        {
            if (CSS.IsValid())
            {
                CSS = Regex.Replace(CSS, "[a-zA-Z]+#", "#");
                CSS = Regex.Replace(CSS, @"[\n\r]+\s*", EmptyString);
                CSS = Regex.Replace(CSS, @"\s+", " ");
                CSS = Regex.Replace(CSS, @"\s?([:,;{}])\s?", "$1");
                CSS = CSS.Replace(";}", "}");
                CSS = Regex.Replace(CSS, @"([\s:]0)(px|pt|%|em)", "$1");
                // Remove comments from CSS
                if (PreserveComments == false)
                {
                    CSS = Regex.Replace(CSS, @"/\*[\d\D]*?\*/", EmptyString);
                }
            }

            return CSS;
        }

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
            var t = new RuleOfThree(Amount, 100, 1, default);
            var Percent = t.UnknownValue?.ToFloat();
            Color = Color.White.MergeWith(Color);
            var l = new List<Color>();
            for (int index = 1, loopTo = Amount; index <= loopTo; index++)
            {
                Color = Color.MakeDarker((float)Percent);
                l.Add(Color);
            }

            return l.ToHSVColorList();
        }

        public static Image Monochrome(this Image Image, Color Color, float Alpha = 0f) => Image.Grayscale().Translate(Color.R, Color.G, Color.B, Alpha);

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool Most<T>(this IEnumerable<T> List, Func<T, bool> predicate, bool Result = true) => List.Select(predicate).Most(Result);

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool Most(this IEnumerable<bool> List, bool Result = true)
        {
            if (List.Any())
            {
                var arr = List.DistinctCount();
                if (arr.ContainsKey(true) && arr.ContainsKey(false))
                {
                    return arr[Result] > arr[!Result];
                }
                else
                {
                    return arr.First().Key == Result;
                }
            }

            return false == Result;
        }

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem false
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool MostFalse<T>(this IEnumerable<T> List, Func<T, bool> predicate) => MostFalse(List.Select(predicate).ToArray());

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem FALSE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool MostFalse(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).Most(false);

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem true
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool MostTrue<T>(this IEnumerable<T> List, Func<T, bool> predicate) => MostTrue(List.Select(predicate).ToArray());

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem TRUE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool MostTrue(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).Most(true);

        public static DirectoryInfo MoveDirectory(DirectoryInfo sourcePath, DirectoryInfo targetPath)
        {
            if (sourcePath.Exists)
            {
                targetPath = targetPath.CreateDirectoryIfNotExists();
                var files = Directory.EnumerateFiles(sourcePath.FullName, "*", SearchOption.AllDirectories).GroupBy(s => Path.GetDirectoryName(s));
                foreach (var folder in files)
                {
                    var targetFolder = folder.Key.Replace(sourcePath.FullName, targetPath.FullName);
                    Directory.CreateDirectory(targetFolder);
                    foreach (var file in folder)
                    {
                        var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                        if (File.Exists(targetFile)) File.Delete(targetFile);
                        File.Move(file, targetFile);
                    }
                }
                Directory.Delete(sourcePath.FullName, true);
            }

            return targetPath;
        }

        /// <summary>
        /// Move os itens de uma lista para outra
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FromList"></param>
        /// <param name="ToList"></param>
        /// <param name="Indexes"></param>
        /// <returns></returns>
        public static List<T> MoveItems<T>(this List<T> FromList, ref List<T> ToList, params int[] Indexes)
        {
            ToList = ToList ?? new List<T>();
            if (FromList != null)
            {
                Indexes = Indexes?.Where(x => x.IsBetween(0, FromList.Count - 1)).ToArray() ?? Array.Empty<int>();
                foreach (var index in Indexes)
                {
                    var item = FromList.Detach(index);
                    try
                    {
                        ToList.Insert(index, item);
                    }
                    catch
                    {
                        ToList.Add(item);
                    }
                }
            }
            return ToList;
        }

        /// <summary>
        /// Inverte as cores de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <returns></returns>
        public static Image Negative(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { -1, 0f, 0f, 0f, 0f }, new float[] { 0f, -1, 0f, 0f, 0f }, new float[] { 0f, 0f, -1, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            return ApplyColorMatrix(copia, cm);
        }

        /// <summary>
        /// Constroi uma expressão diferente de
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression NotEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.NotEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static T? NullCoalesce<T>(this T? First, params T?[] N) where T : struct => (T?)(T)First ?? N.NullCoalesce<T>();

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="List">Outros itens</param>
        /// <returns></returns>
        public static T? NullCoalesce<T>(this IEnumerable<T?> List) where T : struct => List?.FirstOrDefault(x => x.HasValue) ?? default;

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this T First, params T[] N) where T : class => First ?? NullCoalesce((N ?? Array.Empty<T>()).AsEnumerable());

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="List">Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this IEnumerable<T> List) => List == null ? default : List.FirstOrDefault(x => x != null);

        /// <summary>
        /// Anula o valor de um objeto se ele for igual a outro objeto
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestExpression">Outro Objeto</param>
        /// <returns></returns>
        public static T NullIf<T>(this T Value, Func<T, bool> TestExpression) => Value.NullIf(TestExpression != null && TestExpression.Invoke(Value));

        public static T NullIf<T>(this T Value, bool Test) => Test ? default : Value;

        /// <summary>
        /// Anula o valor de um objeto se ele for igual a outro objeto
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestValue">Outro Objeto</param>
        /// <returns></returns>
        public static T NullIf<T>(this T Value, T TestValue) where T : class
        {
            if (Value != null && Value.Equals(TestValue))
            {
                return null;
            }

            return Value;
        }

        /// <summary>
        /// Anula o valor de um objeto se ele for igual a outro objeto
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestValue">Outro Objeto</param>
        /// <returns></returns>
        public static T? NullIf<T>(this T? Value, T? TestValue) where T : struct
        {
            if (Value.HasValue && Value.Equals(TestValue))
            {
                Value = default;
            }

            return Value;
        }

        /// <summary>
        /// Anula o valor de uma string se ela for igual a outra string
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestValue">Outro Objeto</param>
        /// <returns></returns>
        public static string NullIf(this string Value, string TestValue, StringComparison ComparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Value != null && Value.Equals(TestValue, ComparisonType))
            {
                Value = null;
            }

            return Value;
        }

        public static string NullIfBlank(this string Value) => Value.NullIf(x => x.IsBlank());

        /// <summary>
        /// Substitui todas as propriedades nulas de uma classe pelos seus valores Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static T NullPropertiesAsDefault<T>(this T Obj, bool IncludeVirtual = false, params Type[] OnlyTypes) where T : class
        {
            OnlyTypes = OnlyTypes ?? Array.Empty<Type>();
            TryExecute(() => Obj = Obj ?? Activator.CreateInstance<T>());
            if (Obj != null)
                foreach (var item in Obj.GetProperties())
                {
                    if (OnlyTypes.Length == 0 || OnlyTypes.Contains(item.PropertyType.GetTypeOf()))
                        if (item.CanRead && item.CanWrite && item.GetValue(Obj) is null)
                        {
                            switch (item.PropertyType.GetNullableTypeOf())
                            {
                                case var @case when @case == typeof(string):
                                    {
                                        item.SetValue(Obj, EmptyString);
                                        break;
                                    }

                                case var @case when @case == typeof(DateTime):
                                    {
                                        item.SetValue(Obj, DateTime.MinValue);
                                        break;
                                    }

                                case var @case when @case == typeof(byte):
                                    {
                                        item.SetValue(Obj, default(byte));
                                        break;
                                    }

                                case var @case when @case == typeof(short):
                                    {
                                        item.SetValue(Obj, default(short));
                                        break;
                                    }

                                case var @case when @case == typeof(int):
                                    {
                                        item.SetValue(Obj, default(int));
                                        break;
                                    }

                                case var @case when @case == typeof(long):
                                    {
                                        item.SetValue(Obj, default(long));
                                        break;
                                    }

                                case var @case when @case == typeof(double):
                                    {
                                        item.SetValue(Obj, default(double));
                                        break;
                                    }

                                case var @case when @case == typeof(decimal):
                                    {
                                        item.SetValue(Obj, default(decimal));
                                        break;
                                    }

                                default:
                                    {
                                        bool IsVirtual = item.GetAccessors().All(x => x.IsVirtual) && IncludeVirtual;
                                        if (item.IsSimpleType() || IsVirtual)
                                        {
                                            var o = Activator.CreateInstance(item.PropertyType.GetNullableTypeOf());
                                            item.SetValue(Obj, o);
                                        }

                                        break;
                                    }
                            }
                        }
                }

            return Obj;
        }

        /// <summary>
        /// Verifica se somente um unico elemento corresponde a condição
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool OnlyOneOf<T>(this IEnumerable<T> List, Func<T, bool> predicate) => List?.Count(predicate) == 1;



        /// <summary>
        /// Concatena uma expressão com outra usando o operador OR (||)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstExpression"></param>
        /// <param name="OtherExpressions"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> FirstExpression, params Expression<Func<T, bool>>[] OtherExpressions)
        {
            FirstExpression = FirstExpression ?? false.CreateWhereExpression<T>();
            foreach (var item in OtherExpressions ?? Array.Empty<Expression<Func<T, bool>>>())
            {
                if (item != null)
                {
                    var invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast<Expression>());
                    FirstExpression = Expression.Lambda<Func<T, bool>>(Expression.OrElse(FirstExpression.Body, invokedExpr), FirstExpression.Parameters);
                }
            }

            return FirstExpression;
        }

        /// <summary>
        /// Orderna uma lista a partir da aproximação de um deerminado campo com uma string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="PropertySelector"></param>
        /// <param name="Ascending"></param>
        /// <param name="Searches"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, params string[] Searches) where T : class => items.ThenByLike(PropertySelector, Ascending, Searches);

        public static IOrderedEnumerable<T> OrderByMany<T>(this IEnumerable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(true, Selectors);

        public static IOrderedEnumerable<T> OrderByMany<T>(this IEnumerable<T> Data, bool Ascending, params Expression<Func<T, object>>[] Selectors)
        {
            Selectors = Selectors ?? Array.Empty<Expression<Func<T, object>>>();
            if (Selectors.IsNullOrEmpty())
            {
                Expression<Func<T, object>> dd = x => true;
                Selectors = new[] { dd };
            }
            foreach (var Selector in Selectors)
            {
                if (Selector != null)
                {
                    if (Data is IOrderedEnumerable<T> datav)
                    {
                        Data = Ascending ? datav.ThenBy(Selector.Compile()) : (IEnumerable<T>)datav.ThenByDescending(Selector.Compile());
                    }
                    else if (Data is IEnumerable<T>)
                    {
                        Data = Ascending ? Data.OrderBy(Selector.Compile()) : (IEnumerable<T>)Data.OrderByDescending(Selector.Compile());
                    }
                }
            }

            return (IOrderedEnumerable<T>)Data;
        }

        public static IOrderedQueryable<T> OrderByMany<T>(this IQueryable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(true, Selectors);

        public static IOrderedQueryable<T> OrderByMany<T>(this IQueryable<T> Data, bool Ascending, params Expression<Func<T, object>>[] Selectors)
        {
            Selectors = Selectors ?? Array.Empty<Expression<Func<T, object>>>();
            if (Selectors.IsNullOrEmpty())
            {
                Expression<Func<T, object>> dd = x => true;
                Selectors = new[] { dd };
            }
            foreach (var Selector in Selectors)
            {
                if (Selector != null)
                {
                    if (Data is IOrderedQueryable<T> datav)
                    {
                        Data = Ascending ? datav.ThenBy(Selector) : (IQueryable<T>)datav.ThenByDescending(Selector);
                    }
                    else if (Data is IQueryable<T>)
                    {
                        Data = Ascending ? Data.OrderBy(Selector) : (IQueryable<T>)Data.OrderByDescending(Selector);
                    }
                }
            }

            return (IOrderedQueryable<T>)Data;
        }

        public static IOrderedEnumerable<T> OrderByManyDescending<T>(this IEnumerable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(false, Selectors);

        public static IOrderedQueryable<T> OrderByManyDescending<T>(this IQueryable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(false, Selectors);

        /// <summary>
        /// Return a <see cref="IEnumerable{AddressInfo}"/> sorted according to the distance between
        /// locations (Traveler Salesman Problem)
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <remarks>
        /// The first item in <paramref name="address"/> will be used as the start of travel and end
        /// of travel (if <paramref name="returnToStart"/> is true)
        /// </remarks>
        public static IEnumerable<AddressInfo> OrderByNearestNeighbor(this IEnumerable<AddressInfo> address, bool returnToStart = true) => OrderByNearestNeighbor(address, null, returnToStart);

        public static IEnumerable<AddressInfo> OrderByNearestNeighbor(this IEnumerable<AddressInfo> address, double[,] distanceMatrix, bool returnToStart = true)
        {
            var locations = address.ToArray();
            distanceMatrix = distanceMatrix ?? address.GetDistanceMatrix();
            List<int> tour = new List<int>();
            if (distanceMatrix.Length > 0)
            {
                int numLocations = distanceMatrix.GetLength(0);

                // Start the tour at the first location
                tour.Add(0);

                int currentLocation = 0;
                while (tour.Count < numLocations)
                {
                    int nextLocation = -1;
                    double minDistance = double.MaxValue;

                    // Find the nearest unvisited location
                    for (int i = 0; i < numLocations; i++)
                    {
                        if (i == currentLocation || tour.Contains(i))
                        {
                            continue;
                        }

                        double distance = distanceMatrix[currentLocation, i];
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nextLocation = i;
                        }
                    }

                    tour.Add(nextLocation);
                    currentLocation = nextLocation;
                }

                // Return to the starting location
                if (returnToStart) tour.Add(0);

                locations = tour.Select(x => locations[x]).ToArray();
            }

            return locations;
        }

        /// <summary>
        /// Order a list following another list order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="Items"></param>
        /// <param name="Property"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderByPredefinedOrder<T, TOrder>(this IEnumerable<T> Source, Expression<Func<T, TOrder>> PropertySelector, params TOrder[] order)
        {
            Source = Source ?? Array.Empty<T>();
            if (PropertySelector == null) throw new ArgumentException("Property selector is null");
            var p = PropertySelector.Compile();
            var lookup = Source.ToLookup(p, t => t);
            if (order.IsNotNullOrEmpty())
            {
                foreach (var id in order)
                {
                    foreach (var t in lookup[id])
                    {
                        yield return t;
                    }
                }
            }
            else
            {
                foreach (var item in Source)
                {
                    yield return item;
                }
            }
        }

        public static IOrderedQueryable<T> OrderByProperty<T>(this IQueryable<T> source, string[] SortProperty, bool Ascending = true) => ThenByProperty(source, SortProperty, Ascending);

        public static IOrderedEnumerable<T> OrderByProperty<T>(this IEnumerable<T> source, string[] SortProperty, bool Ascending = true) => ThenByProperty(source, SortProperty, Ascending);

        /// <summary>
        /// Randomiza a ordem de um <see cref="IEnumerable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByRandom<T>(this IEnumerable<T> items) => items.OrderBy(x => Guid.NewGuid());

        /// <summary>
        /// Randomiza a ordem de um <see cref="IQueryable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByRandom<T>(this IQueryable<T> items) => items.OrderBy(x => Guid.NewGuid());

        /// <summary>
        /// Ordena um <see cref="IEnumerable"/> priorizando valores especificos a uma condição no
        /// inicio da coleção e então segue a ordem padrão para os outros.
        /// </summary>
        /// <typeparam name="T"></typeparam>

        /// <param name="items">colecao</param>
        /// <param name="Priority">Seletores que define a prioridade da ordem dos itens</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByWithPriority<T>(this IEnumerable<T> items, params Func<T, bool>[] Priority)
        {
            if (items != null)
            {
                Priority = Priority ?? Array.Empty<Func<T, bool>>();

                return items.OrderByDescending(x => Priority.Sum(y => y(x).ToInt()));
            }
            return null;
        }

        public static Expression<Func<T, bool>> OrSearch<T>(this Expression<Func<T, bool>> FirstExpression, IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? false.CreateWhereExpression<T>()).Or(Text.SearchExpression(Properties));

        public static Expression<Func<T, bool>> OrSearch<T>(this Expression<Func<T, bool>> FirstExpression, string Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? false.CreateWhereExpression<T>()).Or(Text.SearchExpression(Properties));

        /// <summary>
        /// Adciona caracteres ao inicio e final de uma string enquanto o <see
        /// cref="string.Length"/> de <paramref name="Text"/> for menor que <paramref name="TotalLength"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="TotalLength">Tamanho total</param>
        /// <param name="PaddingChar">Caractere</param>
        /// <returns></returns>
        public static string Pad(this string Text, int TotalLength, char PaddingChar = ' ')
        {
            Text = Text ?? EmptyString;
            if (Text.Length < TotalLength)
            {
                while (Text.Length < TotalLength)
                {
                    Text = Text.Wrap(PaddingChar.ToString());
                }

                if (Text.Length > TotalLength)
                {
                    Text = Text.RemoveLastChars();
                }
            }

            return Text;
        }

        public static string PadZero(this string Number, int totalWidth) => Number.IfBlank("0").PadLeft(totalWidth, '0');

        public static string PadZero(this int Number, int totalWidth) => Number.ToString(CultureInfo.InvariantCulture).PadZero(totalWidth);

        public static string PadZero(this long Number, int totalWidth) => Number.ToString(CultureInfo.InvariantCulture).PadZero(totalWidth);

        /// <summary>
        /// Reduz um <see cref="IQueryable"/> em uma página especifica
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> Source, int PageNumber, int PageSize) => PageNumber <= 0 ? Source : Source.Skip((PageNumber - 1) * PageSize).Take(PageSize);

        /// <summary>
        /// Reduz um <see cref="IEnumerable"/> em uma página especifica
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> Source, int PageNumber, int PageSize)
        {
            if (PageNumber <= 0)
            {
                return Source;
            }

            return Source.Skip((PageNumber - 1).SetMinValue(0) * PageSize).Take(PageSize);
        }

        /// <summary>
        /// Divide uma lista em pares
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<(T, T)> PairUp<T>(this IEnumerable<T> source)
        {
            if (source != null)
                using (var iterator = source.GetEnumerator())
                    while (iterator.MoveNext())
                        yield return (iterator.Current, iterator.MoveNext() ? iterator.Current : default);
        }

        /// <summary>
        /// limpa um texto deixando apenas os caracteres alfanumericos e espaços
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ParseAlphaNumeric(this string Text)
        {
            var l = new List<string>();
            foreach (var item in Text.Split(WhitespaceChar, StringSplitOptions.RemoveEmptyEntries))
            {
                l.Add(Regex.Replace(item, "[^A-Za-z0-9]", EmptyString));
            }

            return l.SelectJoinString(WhitespaceChar);
        }

        /// <summary>
        /// Parseia uma ConnectionString em um <see cref="ConnectionStringParser"/>
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static ConnectionStringParser ParseConnectionString(this string ConnectionString) => new ConnectionStringParser(ConnectionString);

        /// <summary>
        /// Remove caracteres não numéricos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string ParseDigits(this string Text, CultureInfo Culture = null)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            string strDigits = EmptyString;
            if (string.IsNullOrEmpty(Text))
            {
                return strDigits;
            }

            foreach (char c in Text.ToCharArray())
            {
                if (char.IsDigit(c) || c == Convert.ToChar(Culture.NumberFormat.NumberDecimalSeparator))
                {
                    strDigits += $"{c}";
                }
            }

            return strDigits;
        }

        /// <summary>
        /// Remove caracteres não numéricos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static T ParseDigits<T>(this string Text, CultureInfo Culture = null) where T : IConvertible => Text.ParseDigits(Culture).ChangeType<T>();

        public static NameValueCollection ParseQueryString(this Uri URL, params string[] Keys) => URL?.Query.ParseQueryString(Keys);

        /// <summary>
        /// Transforma uma string em um NameValueCollection de forma segura, tratando falhas e casos de querystring inválida.
        /// </summary>
        /// <param name="QueryString">string contendo uma querystring válida ou URL</param>
        /// <param name="Keys">Quando especificado, inclui apenas estas entradas no NameValueCollection</param>
        /// <returns></returns>
        public static NameValueCollection ParseQueryString(this string QueryString, params string[] Keys)
        {
            var result = new NameValueCollection();
            if (QueryString.IsBlank())
                return result;

            Keys = Keys ?? Array.Empty<string>();
            string query = QueryString;

            // Tenta extrair a query de uma URL, se for o caso
            if (Uri.IsWellFormedUriString(QueryString, UriKind.Absolute))
            {
                try
                {
                    var uri = new Uri(QueryString);
                    query = uri.Query;
                    if (query.StartsWithAny("?"))
                        query = query.Substring(1);
                }
                catch
                {
                    // Se não for uma URL válida, mantém a string original
                    query = QueryString;
                }
            }
            else if (query.StartsWithAny("?"))
            {
                query = query.Substring(1);
            }

            var querySegments = query.Split('&');
            foreach (string qs in querySegments)
            {
                if (qs.IsBlank())
                    continue;
                var segment = qs.UrlDecode();
                var idx = segment.IndexOf('=');
                string key, val;
                if (idx > 0)
                {
                    key = segment.Substring(0, idx).TrimStart(' ', '?');
                    val = segment.Substring(idx + 1);
                }
                else
                {
                    key = segment.TrimStart(' ', '?');
                    val = string.Empty;
                }

                if ((key.IsNotBlank() || Keys.Contains(key)))
                {
                    result.Add(key, val);
                }
            }
            return result;
        }

        /// <summary>
        /// Retorna um novo size mantendo o Aspect ratio, a partir da troca do valor de uma propiedade Width ou Height
        /// </summary>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Size ResizeMaintainingAspect(this Size size, int value, Expression<Func<Size, int>> property)
        {
            var info = property.GetPropertyInfo();

            if (info.Name.FlatEqual(nameof(Size.Height)))
            {
                var aspect = size.Height / (double)size.Width;
                return new Size((int)(value * aspect), value);
            }
            else if (info.Name.FlatEqual(nameof(Size.Width)))
            {
                var aspect = size.Width / (double)size.Height;
                return new Size(value, (int)(value / aspect));
            }
            else
            {
                throw new ArgumentException("Property must be Width or Height");
            }
        }

        /// <summary>
        /// Interpreta uma string de diversas formas e a transforma em um <see cref="Size"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static Size ParseSize(this string Text)
        {
            var s = new Size();
            Text = Text.ReplaceMany(" ", "px", " ", ";", ":").ToLowerInvariant().Trim();
            Text = Text.Replace("largura", "width");
            Text = Text.Replace("altura", "height");
            Text = Text.Replace("l ", "w ");
            Text = Text.Replace("a ", "h ");
            try
            {
                if (Text.IsNumber())
                {
                    s.Width = Text.ToInt();
                    s.Height = s.Width;
                }
                else if (Text.Like("width*") && !Text.Like("*height*"))
                {
                    s.Width = ToInt(Text.GetAfter("width"));
                    s.Height = ToInt(Text.GetAfter("width"));
                }
                else if (Text.Like("height*") && !Text.Like("*width*"))
                {
                    s.Width = ToInt(Text.GetAfter("height"));
                    s.Height = ToInt(Text.GetAfter("height"));
                }
                else if (Text.Like("w*") && !Text.Like("*h*"))
                {
                    s.Width = ToInt(Text.GetAfter("w"));
                    s.Height = ToInt(Text.GetAfter("w"));
                }
                else if (Text.Like("h*") && !Text.Like("*w*"))
                {
                    s.Width = ToInt(Text.GetAfter("h"));
                    s.Height = ToInt(Text.GetAfter("h"));
                }
                else if (Text.Like("width*height*"))
                {
                    s.Width = ToInt(Text.GetBetween("width", "height"));
                    s.Height = ToInt(Text.GetAfter("height"));
                }
                else if (Text.Like("height*width*"))
                {
                    s.Height = ToInt(Text.GetBetween("height", "width"));
                    s.Width = ToInt(Text.GetAfter("width"));
                }
                else if (Text.Like("w*h*"))
                {
                    s.Width = ToInt(Text.GetBetween("w", "h"));
                    s.Height = ToInt(Text.GetAfter("h"));
                }
                else if (Text.Like("h*w*"))
                {
                    s.Height = ToInt(Text.GetBetween("h", "w"));
                    s.Width = ToInt(Text.GetAfter("w"));
                }
                else if (Text.Like("*x*"))
                {
                    s.Width = ToInt(Text.Split(new[] { "x" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { "x" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
                else if (Text.Like("*by*"))
                {
                    s.Width = ToInt(Text.Split(new[] { "by" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { "by" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
                else if (Text.Like("*por*"))
                {
                    s.Width = ToInt(Text.Split(new[] { "por" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { "por" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
                else if (Text.Like("*,*"))
                {
                    s.Width = ToInt(Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
                else if (Text.Like("*-*"))
                {
                    s.Width = ToInt(Text.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
                else if (Text.Like("*_*"))
                {
                    s.Width = ToInt(Text.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
                else if (Text.Like("*:*"))
                {
                    s.Width = ToInt(Text.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
                else
                {
                    s.Width = ToInt(Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault());
                    s.Height = ToInt(Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
                }
            }
            catch
            {
            }

            return s;
        }

        public static HtmlNode ParseTag(this string HtmlString) => HtmlNode.ParseNode(HtmlString);

        public static HtmlNode ParseTag(this FileInfo File) => HtmlNode.ParseNode(File);

        public static HtmlNode ParseTag(this Uri URL) => HtmlNode.ParseNode(URL);

        public static IEnumerable<HtmlNode> ParseTags(this string HtmlString) => HtmlNode.Parse(HtmlString);

        public static IEnumerable<HtmlNode> ParseTags(this FileInfo File) => HtmlNode.Parse(File);

        public static IEnumerable<HtmlNode> ParseTags(this Uri URL) => HtmlNode.Parse(URL);

        public static string CamelCaseAdjust(this string Text) => PascalCaseAdjust(Text);

        /// <summary>
        /// Separa as palavras de um texto PascalCase a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string PascalCaseAdjust(this string Text)
        {
            Text = Text.IfBlank(EmptyString);
            var chars = Text.ToArray();
            Text = EmptyString;
            int uppercount = 0;
            foreach (var c in chars)
            {
                if (char.IsUpper(c))
                {
                    if (!(uppercount > 0))
                    {
                        Text += WhitespaceChar;
                    }

                    uppercount++;
                }
                else
                {
                    if (uppercount > 1)
                    {
                        Text += WhitespaceChar;
                    }

                    uppercount = 0;
                }

                Text += $"{c}";
            }

            return Text.Trim();
        }

        /// <summary>
        /// Transforma um texto em CamelCase em um array de palavras a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> PascalCaseSplit(this string Text) => Text.PascalCaseAdjust().Split(WhitespaceChar);

        /// <summary>
        /// Transforma um texto em CamelCase em um array de palavras a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> CamelCaseSplit(this string Text) => PascalCaseSplit(Text);

        /// <summary>
        /// Retorna os primeiros caracteres de uma fila de caracteres
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public static string Peek(this Queue<char> queue, int take) => new String(queue.Take(take).ToArray());

        /// <summary>
        /// Pixeliza uma imagem
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="PixelateSize"></param>
        /// <returns></returns>
        public static Image Pixelate(this Image Image, int PixelateSize = 1)
        {
            if (Image == null) return null;
            var rectangle = new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height);
            PixelateSize++;
            var pixelated = new Bitmap(Image.Width, Image.Height);
            using (var graphics = Graphics.FromImage(pixelated))
            {
                graphics.DrawImage(Image, new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height), new System.Drawing.Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
            }

            int xx = rectangle.X;
            while (xx < rectangle.X + rectangle.Width && xx < Image.Width)
            {
                int yy = rectangle.Y;
                while (yy < rectangle.Y + rectangle.Height && yy < Image.Height)
                {
                    int offsetX = (int)Math.Round(PixelateSize / 2d);
                    int offsetY = (int)Math.Round(PixelateSize / 2d);
                    while (xx + offsetX >= Image.Width)
                        offsetX -= 1;
                    while (yy + offsetY >= Image.Height)
                        offsetY -= 1;
                    var pixel = pixelated.GetPixel(xx + offsetX, yy + offsetY);
                    int x = xx;
                    while (x < xx + PixelateSize && x < Image.Width)
                    {
                        int y = yy;
                        while (y < yy + PixelateSize && y < Image.Height)
                        {
                            pixelated.SetPixel(x, y, pixel);
                            y += 1;
                        }

                        x += 1;
                    }

                    yy += PixelateSize;
                }

                xx += PixelateSize;
            }

            return pixelated;
        }

        /// <summary>
        /// Retorna uma string em sua forma poop
        /// </summary>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static string[] Poopfy(params string[] Words)
        {
            var p = new List<string>();
            foreach (var Text in Words ?? Array.Empty<string>())
            {
                decimal l = (decimal)(Text.Length / 2d);
                l = l.Floor();
                if (!Text.GetFirstChars((int)Math.Round(l)).Last().ToString().ToLowerInvariant().IsIn(PredefinedArrays.LowerVowels))
                {
                    l = l.ToInt() - 1;
                }

                p.Add(Text.GetFirstChars((int)Math.Round(l)).Trim() + Text.GetFirstChars((int)Math.Round(l)).Reverse().ToList().SelectJoinString().ToLowerInvariant().Trim() + Text.RemoveFirstChars((int)Math.Round(l)).TrimStartAny(PredefinedArrays.LowerConsonants.ToArray()));
            }

            return p.ToArray();
        }

        /// <summary>
        /// Retorna uma string em sua forma poop
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Poopfy(this string Text) => Poopfy(Text.SplitAny(PredefinedArrays.WordSplitters)).SelectJoinString(WhitespaceChar);

        /// <summary>
        /// Return a Idented XML string
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string PreetyPrint(this XmlDocument Document)
        {
            string Result = EmptyString;
            if (Document != null)
            {
                var mStream = new MemoryStream();
                var writer = new XmlTextWriter(mStream, Encoding.Unicode);
                try
                {
                    writer.Formatting = Formatting.Indented;

                    // Write the XML into a formatting XmlTextWriter
                    Document.WriteContentTo(writer);
                    writer.Flush();
                    mStream.Flush();

                    // Have to rewind the MemoryStream in order to read its contents.
                    mStream.Position = 0L;

                    // Read MemoryStream contents into a StreamReader.
                    var sReader = new StreamReader(mStream);

                    // Extract the text from the StreamReader.
                    Result = sReader.ReadToEnd();
                }
                catch (XmlException)
                {
                }
                finally
                {
                    mStream.Close();
                    writer.Close();
                    mStream.Dispose();
                    writer.Dispose();
                }
            }

            return Result;
        }

        /// <summary>
        /// Adiciona texto ao começo de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        public static string Prepend(this string Text, string PrependText)
        {
            Text = Text ?? EmptyString;
            PrependText = PrependText ?? EmptyString;
            Text = PrependText + Text;
            return Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependIf(this string Text, string PrependText, Func<string, bool> Test = null)
        {
            Text = Text ?? EmptyString;
            PrependText = PrependText ?? EmptyString;
            return Text.PrependIf(PrependText, (Test ?? (x => false))(Text));
        }

        /// <summary>
        /// Adiciona texto ao começo de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependIf(this string Text, string PrependText, bool Test)
        {
            Text = Text ?? EmptyString;
            PrependText = PrependText ?? EmptyString;
            return Test ? Text.Prepend(PrependText) : Text;
        }

        /// <summary>
        /// Adiciona texto ao inicio de uma string com uma quebra de linha no final do <paramref name="PrependText"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        public static string PrependLine(this string Text, string PrependText) => Text.Prepend(Environment.NewLine).Prepend(PrependText);

        /// <summary>
        /// Adiciona texto ao inicio de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependWhile(this string Text, string PrependText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);

            while (Test(Text))
            {
                Text = Text.Prepend(PrependText);
            }

            return Text;
        }

        /// <summary>
        /// Retorna a string especificada se o valor boolean for verdadeiro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="BooleanValue"></param>
        /// <returns></returns>
        public static string PrintIf(this string Text, bool BooleanValue) => BooleanValue ? Text : EmptyString;

        /// <summary>
        /// Gera uma lista de nomes de propriedades a partir de um nome base.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static IEnumerable<string> PropertyNamesFor(this string Name)
        {
            var propnames = new List<string>();

            if (Name.IsValid())
            {
                if (Name.StartsWith("_", StringComparison.InvariantCultureIgnoreCase))
                {
                    propnames.Add(Name.TrimStart('_').Replace(" ", "_").Replace("-", "_").Replace("~", "_"));
                }
                string propname1 = Name.Trim().Replace(" ", "_").Replace("-", "_").Replace("~", "_");
                string propname3 = Name.Trim().Replace(" ", EmptyString).Replace("-", EmptyString).Replace("~", EmptyString);
                string propname2 = propname1.RemoveAccents();
                string propname4 = propname3.RemoveAccents();
                propnames.AddRange(new[] { Name, propname1, propname2, propname3, propname4 });
                propnames.AddRange(propnames.Select(x => $"_{x}").ToArray());
                propnames.AddRange(propnames.Select(x => x.ToTitle()).ToArray());
                return propnames.Where(x => x.Contains(" ") == false).Distinct();
            }
            return Array.Empty<string>();
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico encontrado no primeiro parametro.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <returns></returns>
        /// <example>texto = $"{2} pães"</example>
        public static string QuantifyText(this FormattableString PluralText)
        {
            if (PluralText.IsNotBlank() && PluralText.ArgumentCount > 0)
            {
                decimal numero = 0m;
                string str = PluralText.Format.QuantifyText(PluralText.GetArguments().FirstOrDefault(), ref numero);
                str = str.Replace("{0}", $"{numero}");
                for (int index = 1, loopTo = PluralText.GetArguments().Length - 1; index <= loopTo; index++)
                {
                    str = str.Replace($"{{{index}}}", $"{PluralText.GetArgument(index)}");
                }

                return str;
            }

            return $"{PluralText}";
        }




        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this string PluralText, object Quantity)
        {
            decimal d = 0m;
            return PluralText.QuantifyText(Quantity, ref d);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="QuantityOrListOrBoolean">Quantidade de Itens</param>
        /// <param name="OutQuantity">Devolve a quantidade encontrada em <paramref name="QuantityOrListOrBoolean"/></param>
        /// <returns></returns>
        public static string QuantifyText(this string PluralText, object QuantityOrListOrBoolean, ref decimal OutQuantity)
        {
            bool forceSingular = false;
            if (QuantityOrListOrBoolean == null)
            {
                OutQuantity = 0m;
            }
            else if (QuantityOrListOrBoolean is bool b)
            {
                //em portugues, quando a quantidade maxima de itens é 1, zero também é singular
                OutQuantity = b ? 1 : 0;
                forceSingular = true;
            }
            else if (QuantityOrListOrBoolean.IsNumber())
            {
                OutQuantity = (QuantityOrListOrBoolean).ToDecimal();
            }
            else if (typeof(IList).IsAssignableFrom(QuantityOrListOrBoolean.GetType()))
            {
                OutQuantity = ((IList)QuantityOrListOrBoolean).Count;
            }
            else if (typeof(IDictionary).IsAssignableFrom(QuantityOrListOrBoolean.GetType()))
            {
                var dic = (IDictionary)QuantityOrListOrBoolean;
                OutQuantity = dic.Count;
            }
            else if (typeof(Array).IsAssignableFrom(QuantityOrListOrBoolean.GetType()))
            {
                var arr = (Array)QuantityOrListOrBoolean;
                OutQuantity = (arr).Length;
            }
            else
            {
                if (!decimal.TryParse(QuantityOrListOrBoolean.ToString(), out OutQuantity))
                {
                    WriteDebug("Quantity parsing fail");
                }
            }
            return forceSingular || OutQuantity.Floor() == 1m || OutQuantity.Floor() == -1 ? PluralText.Singularize() : PluralText;
        }

        public static string QuantifyText(this decimal Quantity, string PluralText, string SingularText = "")
        {
            PluralText = PluralText.IfBlank("Items");
            return Quantity.Floor() == 1 || Quantity.Floor() == -1 ? SingularText.IfBlank(PluralText.Singularize()) : PluralText;
        }

        public static string QuantifyText(this int Quantity, string PluralText, string SingularText = "")
        => Quantity.ToDecimal().QuantifyText(PluralText, SingularText);

        public static string QuantifyText(this long Quantity, string PluralText, string SingularText = "")
       => Quantity.ToDecimal().QuantifyText(PluralText, SingularText);

        public static string QuantifyText(this short Quantity, string PluralText, string SingularText = "")
       => Quantity.ToDecimal().QuantifyText(PluralText, SingularText);

        public static string QuantifyText(this double Quantity, string PluralText, string SingularText = "")
       => Quantity.ToDecimal().QuantifyText(PluralText, SingularText);


        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="List">Lista com itens</param>
        /// <returns></returns>
        public static string QuantifyText<T>(this IEnumerable<T> List, string PluralText) => PluralText.QuantifyText(List ?? Array.Empty<T>());

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this int Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this decimal Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this short Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this long Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this double Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Encapsula um texto entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes)
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="OpenQuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Quote(this string Text, char OpenQuoteChar = '"')
        {
            if (Convert.ToBoolean(OpenQuoteChar.ToString().IsCloseWrapChar()))
            {
                OpenQuoteChar = OpenQuoteChar.GetOppositeWrapChar();
            }

            return $"{OpenQuoteChar}{Text}{OpenQuoteChar.GetOppositeWrapChar()}";
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes)
        /// se uma condição for cumprida
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="QuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string QuoteIf(this string Text, bool Condition, char QuoteChar = '"') => Condition ? Text.Quote(QuoteChar) : Text;

        /// <summary>
        /// Gera um valor boolean aleatorio considerando uma porcentagem de chance
        /// </summary>
        /// <returns>TRUE ou FALSE.</returns>
        public static bool RandomBool(int Percent) => RandomBool(x => x <= Percent, 0, 100);

        /// <summary>
        /// Gera um valor boolean aleatorio considerando uma condição de comparação com um numero
        /// gerado aleatóriamente
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão 999999</param>
        /// <returns>TRUE ou FALSE</returns>
        public static bool RandomBool(Func<int, bool> Condition, int Min = 0, int Max = int.MaxValue) => Condition(RandomInt(Min, Max));

        /// <summary>
        /// Gera um valor boolean aleatorio
        /// </summary>
        /// <returns>TRUE ou FALSE</returns>
        public static bool RandomBool() => RandomInt(0, 1).ToBool();

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

            Red = (Red < 0 ? RandomInt(0, 255) : Red).LimitRange<int>(0, 255);
            Green = (Green < 0 ? RandomInt(0, 255) : Green).LimitRange<int>(0, 255);
            Blue = (Blue < 0 ? RandomInt(0, 255) : Blue).LimitRange<int>(0, 255);
            Alpha = Alpha.LimitRange<int>(0, 255);
            return Color.FromArgb(Alpha, Red, Green, Blue);
        }

        /// <summary>
        /// Gera uma lista com <paramref name="Quantity"/> cores diferentes
        /// </summary>
        /// <param name="Quantity">Quantidade máxima de cores</param>
        /// <param name="Red"></param>
        /// <param name="Green"></param>
        /// <param name="Blue"></param>
        /// <remarks></remarks>
        /// <returns></returns>
        public static IEnumerable<Color> RandomColorList(int Quantity, int Red = -1, int Green = -1, int Blue = -1)
        {
            Red = Red.SetMinValue(-1);
            Green = Green.SetMinValue(-1);
            Blue = Blue.SetMinValue(-1);

            var l = new List<Color>();
            if (Red == Green && Green == Blue && Blue != -1)
            {
                l.Add(Color.FromArgb(Red, Green, Blue));
                return l;
            }

            int errorcount = 0;
            while (l.Count < Quantity)
            {
                var r = RandomColor(Red, Green, Blue);
                if (l.Any(x => (x.ToHexadecimal() ?? EmptyString) == (r.ToHexadecimal() ?? EmptyString)))
                {
                    errorcount++;
                    if (errorcount == Quantity)
                    {
                        return l;
                    }
                }
                else
                {
                    errorcount = 0;
                    l.Add(r);
                }
            }

            return l;
        }

        /// <summary>
        /// Gera uma data aleatória a partir de componentes nulos de data
        /// </summary>
        /// <returns>Um numero Inteiro</returns>
        public static DateTime RandomDateTime(int? Year = null, int? Month = null, int? Day = null, int? Hour = null, int? Minute = null, int? Second = null)
        {
            Year = (Year ?? RandomInt(DateTime.MinValue.Year, DateTime.MaxValue.Year)).ForcePositive().LimitRange(DateTime.MinValue.Year, DateTime.MaxValue.Year);
            Month = (Month ?? RandomInt(DateTime.MinValue.Month, DateTime.MaxValue.Month)).ForcePositive().LimitRange(1, 12);
            Day = (Day ?? RandomInt(DateTime.MinValue.Day, DateTime.MaxValue.Day)).ForcePositive().LimitRange(1, 31);
            Hour = (Hour ?? RandomInt(DateTime.MinValue.Hour, DateTime.MaxValue.Hour)).ForcePositive().LimitRange(1, 31);
            Minute = (Minute ?? RandomInt(DateTime.MinValue.Minute, DateTime.MaxValue.Minute)).ForcePositive().LimitRange(0, 59);
            Second = (Second ?? RandomInt(DateTime.MinValue.Second, DateTime.MaxValue.Second)).ForcePositive().LimitRange(0, 59);

            DateTime randomCreated = DateTime.Now;
            while (TryExecute(() => randomCreated = new DateTime(Year.Value, Month.Value, Day.Value, Hour.Value, Minute.Value, Second.Value)) != null)
            {
                Day--;
            }

            return randomCreated;
        }

        /// <summary>
        /// Gera uma data aleatória entre 2 datas
        /// </summary>
        /// <param name="Min">Data Minima</param>
        /// <param name="Max">Data Maxima</param>
        /// <returns>Um numero Inteiro</returns>
        public static DateTime RandomDateTime(DateTime? MinDate, DateTime? MaxDate = null)
        {
            var Min = (MinDate ?? RandomDateTime()).Ticks;
            var Max = (MaxDate ?? RandomDateTime()).Ticks;
            FixOrder(ref Min, ref Max);
            return new DateTime(RandomLong(Min, Max));
        }

        /// <summary>
        /// Gera um EAN aleatório com digito verificador válido
        /// </summary>
        /// <param name="Len"></param>
        /// <returns></returns>
        public static string RandomEAN(int Len) => RandomFixLenghtNumber(Len.SetMinValue(2) - 1).ToString().AppendBarcodeCheckSum();

        /// <summary>
        /// Gera um numero aleatório de comprimento fixo
        /// </summary>
        /// <param name="Len"></param>
        /// <returns></returns>
        public static string RandomFixLenghtNumber(int Len = 8)
        {
            var n = EmptyString;
            for (int i = 0; i < Len; i++)
            {
                n += PredefinedArrays.NumberChars.RandomItem();
            }
            return n;
        }

        /// <summary>
        /// Gera um texto aleatorio
        /// </summary>
        /// <param name="ParagraphCount">Quantidade de paragrafos</param>
        /// <param name="SentenceCount">QUantidade de sentenças por paragrafo</param>
        /// <param name="MinWordCount"></param>
        /// <param name="MaxWordCount"></param>
        /// <returns></returns>
        public static StructuredText RandomIpsum(int ParagraphCount = 5, int SentenceCount = 3, int MinWordCount = 10, int MaxWordCount = 50, int IdentSize = 0, int BreakLinesBetweenParagraph = 0, int Words = 300) => LoremIpsum(ParagraphCount, SentenceCount, MinWordCount, MaxWordCount, IdentSize, BreakLinesBetweenParagraph, Enumerable.Range(0, Words).Select(x => RandomWord(2, 14)).ToArray());

        /// <summary>
        /// Sorteia um item da Matriz
        /// </summary>
        /// <typeparam name="Type">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static T RandomItem<T>(params T[] Array) => Array.GetRandomItem();

        public static T RandomItem<T>(this IEnumerable<T> l) => l.RandomItemOr();

        public static T RandomItem<T>(this IEnumerable<T> l, Func<T, bool> predicade) => l.RandomItemOr(predicade);

        public static T RandomItemOr<T>(this IEnumerable<T> l, params T[] Alternate) => l.TakeRandom().FirstOr(Alternate);

        public static T RandomItemOr<T>(this IEnumerable<T> l, Func<T, bool> predicade, params T[] Alternate) => l.TakeRandom(predicade).FirstOr(Alternate);

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo</param>
        /// <param name="Max">Numero Maximo</param>
        /// <returns>Um numero Inteiro</returns>
        public static T Random<T>(this T Min, T Max) where T : IComparable
        {
            FixOrder(ref Min, ref Max);

            if (Min.Equals(Max)) return Min;

            if (Min is int MinI && Max is int MaxI)
            {
                return init_rnd.Next(MinI, MaxI == int.MaxValue ? int.MaxValue : MaxI + 1).ChangeType<T>();
            }
            if (Min is decimal MinT && Max is decimal MaxT)
            {
                return (init_rnd.NextDouble().ToDecimal() * (MaxT - MinT) + MinT).ChangeType<T>();
            }
            else if (Min is double MinD && Max is double MaxD)
            {
                return (init_rnd.NextDouble() * (MaxD - MinD) + MinD).ChangeType<T>();
            }
            else if (Min is long MinL && Max is long MaxL)
            {
                MaxL = MaxL == long.MaxValue ? long.MaxValue : MaxL + 1;
                byte[] buf = new byte[8];
                init_rnd.NextBytes(buf);
                long longRand = BitConverter.ToInt64(buf, 0);
                return (Math.Abs(longRand % (MaxL - MinL)) + MinL).ChangeType<T>();
            }
            else if (Min is DateTime MinDate && Max is DateTime MaxDate)
            {
                return new DateTime(RandomLong(MinDate.Ticks, MaxDate.Ticks)).ChangeType<T>();
            }
            else if (Min is string MinS && Max is string MaxS)
            {
                return RandomWord(MinS.Length, MaxS.Length).ChangeType<T>();
            }
            else if (Min is short MinX && Max is short MaxX)
            {
                return RandomInt(MinX.ToInt(), MaxX.ToInt()).ChangeType<T>();
            }
            else if (Min is bool MinB && Max is bool MaxB)
            {
                return RandomBool().ChangeType<T>();
            }
            else if (Min is char MinC && Max is char MaxC)
            {
                return char.ConvertFromUtf32(RandomInt(MinC.ToInt(), MaxC.ToInt())).ChangeType<T>();
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static short RandomShort(short Min = 0, short Max = short.MaxValue) => Random(Min, Max);

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static int RandomInt(int Min = 0, int Max = int.MaxValue) => Random(Min, Max);

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static double RandomDouble(double Min = 0, double Max = double.MaxValue) => Random(Min, Max);

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static decimal RandomDecimal(decimal Min = 0, decimal Max = decimal.MaxValue) => Random(Min, Max);

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="long.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static long RandomLong(long Min, long Max = long.MaxValue) => Random(Min, Max);

        /// <summary>
        /// Gera uma Lista com numeros Aleatórios entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static IEnumerable<int> RandomNumberList(int Count, int Min = 0, int Max = int.MaxValue, bool UniqueNumbers = true)
        {
            if (Count > 0)
            {
                if (Max == Min) return new int[] { Min };
                if (UniqueNumbers)
                {
                    if (Max < int.MaxValue) Max++;
                    FixOrder(ref Min, ref Max);
                    var l = Enumerable.Range(Min, Max - Min).OrderByRandom().ToList();
                    while (l.Count > Count) l.RemoveAt(0);
                    return l;
                }
                else
                {
                    return Enumerable.Range(1, Count).Select(e => RandomInt(Min, Max));
                }
            }
            return Array.Empty<int>();
        }

        /// <summary>
        /// Gera um nome de usuário aleatório com 5 caracteres e um número entre 1111 e 9999
        /// </summary>
        /// <returns></returns>
        public static string RandomUserName() => RandomUserName(5, 1111);
        /// <summary>
        /// Gera um nome de usuário aleatório com o tamanho de palavra definido por <paramref name="WordLength"/>
        /// e um número entre <paramref name="MinNumber"/> e 9999
        /// </summary>
        /// <param name="WordLength"></param>
        /// <param name="MinNumber"></param>
        /// <returns></returns>
        public static string RandomUserName(int WordLength, int MinNumber) => $"{Util.RandomWord(WordLength)}{Util.RandomInt(MinNumber, 9999)}";

        /// <summary>
        /// Gera uma palavra aleatória com o numero de caracteres entre <paramref name="MinLength"/>
        /// e <paramref name="MaxLenght"/>
        /// </summary>
        /// <returns>Uma string contendo uma palavra aleatória</returns>
        public static string RandomWord(int MinLength, int MaxLenght) => RandomWord(RandomInt(MinLength.SetMinValue(1), MaxLenght.SetMinValue(1)));

        /// <summary>
        /// Gera uma palavra aleatória com o numero de caracteres
        /// </summary>
        /// <param name="Length">Tamanho da palavra</param>
        /// <returns>Uma string contendo uma palavra aleatória</returns>
        public static string RandomWord(int Length = 0)
        {
            Length = Length < 1 ? RandomInt(2, 15) : Length;
            string word = EmptyString;
            if (Length == 1)
            {
                return RandomItem(PredefinedArrays.Vowels.ToArray());
            }

            // Util the word in consonant / vowel pairs
            while (word.Length < Length)
            {
                // Add the consonant
                string consonant = PredefinedArrays.LowerConsonants.RandomItem();
                if (consonant == "q" && word.Length + 3 <= Length)
                {
                    // check +3 because we'd add 3 characters in this case, the "qu" and the vowel.
                    // Change 3 to 2 to allow ww that end in "qu"
                    word += "qu";
                }
                else
                {
                    while (consonant == "q")
                    {
                        // ReplaceFrom an orphaned "q"
                        consonant = PredefinedArrays.LowerConsonants.RandomItem();
                    }

                    if (word.Length + 1 <= Length)
                    {
                        // Only add a consonant if there's enough room remaining
                        word += consonant;
                    }
                }

                if (word.Length + 1 <= Length)
                {
                    // Only add a vowel if there's enough room remaining
                    word += PredefinedArrays.LowerVowels.RandomItem();
                }
            }

            return word;
        }

        public static IOrderedEnumerable<TObject> Rank<TObject, TValue>(this IEnumerable<TObject> values, Expression<Func<TObject, TValue>> ValueSelector) where TObject : class where TValue : IComparable => Rank<TObject, TValue, int>(values, ValueSelector, null);

        /// <summary>
        /// Rankeia um <see cref="IEnumerable{TObject}"/> a partir de uma propriedade definida por
        /// <paramref name="ValueSelector"/> guardando sua posição no <paramref name="RankSelector"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TRank"></typeparam>
        /// <param name="values"></param>
        /// <param name="ValueSelector"></param>
        /// <param name="RankSelector"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TObject> Rank<TObject, TValue, TRank>(this IEnumerable<TObject> values, Expression<Func<TObject, TValue>> ValueSelector, Expression<Func<TObject, TRank>> RankSelector) where TObject : class where TValue : IComparable where TRank : IComparable
        {
            if (values != null)
            {
                if (ValueSelector != null)
                {
                    values = values.OrderByDescending(ValueSelector.Compile());

                    if (RankSelector != null)
                    {
                        var filtered = values.Select(ValueSelector.Compile()).Distinct().ToList();
                        var l = new List<TObject>();
                        foreach (TObject item in values)
                        {
                            var pos = filtered.IndexOf((ValueSelector.Compile().Invoke(item))) + 1;
                            l.Add(item.SetPropertyValue(RankSelector, pos.ChangeType<TRank>()));
                        }
                        return l.OrderBy(RankSelector.Compile());
                    }
                    return values as IOrderedEnumerable<TObject>;
                }
                return values.OrderBy(x => 0);
            }
            return null;
        }

        /// <summary>
        /// Retorna o conteudo de um arquivo de texto
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string ReadAllText(this FileInfo File, Encoding encoding = null) => File != null && File.Exists ? System.IO.File.ReadAllText(File.FullName, encoding ?? Encoding.UTF8) : EmptyString;

        public static IEnumerable<string> ReadManyText(this DirectoryInfo directory, SearchOption Option, params string[] Patterns) => directory.SearchFiles(Option, Patterns).Select(x => x.ReadAllText());

        public static IEnumerable<string> ReadManyText(this DirectoryInfo directory, params string[] Patterns) => directory.ReadManyText(SearchOption.TopDirectoryOnly, Patterns);

        public static IEnumerable<string> ReduceToDifference(this IEnumerable<string> Texts, bool FromStart = false, string BreakAt = null) => ReduceToDifference(Texts, out _, FromStart, BreakAt);

        public static IEnumerable<string> ReduceToDifference(this IEnumerable<string> Texts, out string RemovedPart, bool FromStart = false, string BreakAt = null)
        {
            RemovedPart = EmptyString;
            Texts = Texts ?? Array.Empty<string>();
            var arr = Texts.WhereNotBlank().ToArray();
            while (arr.Distinct().Count() > 1 && !arr.Any(x => BreakAt.IsValid() && (FromStart ? x.StartsWith(BreakAt) : x.EndsWith(BreakAt))) && arr.All(x => FromStart ? x.StartsWith(arr.FirstOrDefault().GetFirstChars()) : x.EndsWith(arr.FirstOrDefault().GetLastChars())))
            {
                arr = arr.Select(x => FromStart ? x.RemoveFirstChars() : x.RemoveLastChars()).ToArray();
            }

            if (BreakAt.IsValid())
            {
                arr = arr.Select(x => FromStart ? x.TrimStartAny(false, BreakAt) : x.TrimEndAny(false, BreakAt)).ToArray();
                //Difference = FromStart ? Difference.Prepend(BreakAt) : Difference.Append(BreakAt);
            }

            RemovedPart = FromStart ? RemovedPart.Prepend(Texts.FirstOrDefault().TrimEndAny(arr.FirstOrDefault())) : RemovedPart.Append(Texts.FirstOrDefault().TrimStartAny(arr.FirstOrDefault()));

            return arr;
        }

        /// <summary>
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<T, long> ReduceToTop<T>(this Dictionary<T, long> obj, int First, T OtherLabel)
        {
            var grouped = obj.OrderByDescending(x => x.Value);
            return grouped.Take(First).Union(new[] { new KeyValuePair<T, long>(OtherLabel, grouped.Skip(First).Sum(s => s.Value)) }).ToDictionary();
        }

        public static Dictionary<TGroup, Dictionary<TCount, long>> ReduceToTop<TGroup, TCount>(this Dictionary<TGroup, Dictionary<TCount, long>> Grouped, int First, TCount OtherLabel)
        {
            if (Grouped != null)
            {
                foreach (var item in Grouped.ToArray())
                {
                    var gp = item.Value.OrderByDescending(x => x.Value).ToDictionary();
                    Grouped[item.Key] = gp.Take(First).Union(new[] { new KeyValuePair<TCount, long>(OtherLabel, gp.Skip(First).Sum(s => s.Value)) }).ToDictionary();
                }

                Grouped.Values.MergeKeys();
            }
            return Grouped;
        }

        /// <summary>
        /// Escapa caracteres exclusivos de uma regex
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string RegexEscape(this string Text)
        {
            string newstring = EmptyString;
            foreach (var c in Text.ToArray())
            {
                if (c.IsIn(PredefinedArrays.RegexChars))
                {
                    newstring += @"\" + c;
                }
                else
                {
                    newstring += Convert.ToString(c);
                }
            }

            return newstring;
        }

        /// <summary>
        /// Remove os acentos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String sem os acentos</returns>
        public static string RemoveAccents(this string Text)
        {
            if (Text == null)
            {
                return Text;
            }

            string s = Text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            int k = 0;
            while (k < s.Length)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }

                k++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Remove várias strings de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Values">Strings a serem removidas</param>
        /// <returns>Uma string com os valores removidos</returns>
        public static string RemoveAny(this string Text, params string[] Values) => Text.ReplaceMany(EmptyString, Values ?? Array.Empty<string>());

        public static string RemoveAny(this string Text, params char[] Values) => Text.RemoveAny(Values.Select(x => x.ToString()).ToArray());

        public static IEnumerable<T> RemoveAny<T>(this IEnumerable<T> Items, params T[] Values) => Items?.Where(x => x.IsNotIn(Values));

        public static IEnumerable<T> RemoveAny<T>(this IEnumerable<T> Items, IEqualityComparer<T> Comparer, params T[] Values) => Items?.Where(x => x.IsNotIn(Values, Comparer));

        /// <summary>
        /// Remove os acentos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String sem os acentos</returns>
        public static string RemoveDiacritics(this string Text) => Text.RemoveAccents();

        /// <summary>
        /// Remove os X primeiros caracteres
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Quantity">Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveFirstChars(this string Text, int Quantity = 1)
        {
            if (Text.IsNotBlank())
            {
                if (Quantity > 0)
                {
                    if (Text.Length >= Quantity) return Text.Remove(0, Quantity);
                }
                else if (Quantity < 0)
                {
                    return Text.Remove(0, Text.Length + Quantity);
                }
                else
                {
                    return Text;
                }
            }
            return Text;
        }

        /// <summary>
        /// Remove um texto do inicio de uma string se ele for um outro texto especificado
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Texto inicial que será comparado</param>
        public static string RemoveFirstEqual(this string Text, string StartStringTest, StringComparison comparison = default)
        {
            Text = Text ?? "";
            StartStringTest = StartStringTest ?? "";
            if (Text.StartsWith(StartStringTest, comparison))
            {
                Text = Text.RemoveFirstChars(StartStringTest.Length);
            }

            return Text;
        }

        public static string RemoveHTML(this string Text)
        {
            if (Text.IsValid())
            {
                return Regex.Replace(Text.ReplaceMany(Environment.NewLine, "<br/>", "<br>", "<br />"), "<.*?>", EmptyString).HtmlDecode();
            }

            return Text;
        }

        /// <summary>
        /// Remove de um dicionario as respectivas Keys se as mesmas existirem
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="Keys"></param>
        public static IDictionary<TKey, TValue> RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, params TKey[] Keys)
        {
            if (dic != null)
                foreach (var k in (Keys ?? Array.Empty<TKey>()).Where(x => dic.ContainsKey(x)))
                {
                    dic.Remove(k);
                }

            return dic;
        }

        /// <summary>
        /// Remove de um dicionario os valores encontrados pelo predicate
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="predicate"></param>
        public static IDictionary<TKey, TValue> RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<KeyValuePair<TKey, TValue>, bool> predicate) => dic.RemoveIfExist(dic.Where(predicate).Select(x => x.Key).ToArray());

        /// <summary>
        /// Remove <paramref name="Count"/> elementos de uma <paramref name="List"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static List<T> RemoveLast<T>(this List<T> List, int Count = 1)
        {
            if (List != null)
                for (int index = 1, loopTo = Count; index <= loopTo; index++)
                {
                    if (List.Any())
                    {
                        List.RemoveAt(List.Count - 1);
                    }
                }

            return List;
        }

        /// <summary>
        /// Remove os X ultimos caracteres
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Quantity">Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveLastChars(this string Text, int Quantity = 1)
        {
            if (Text.IsNotBlank())
            {
                if (Quantity > 0)
                {
                    if (Text.Length >= Quantity) return Text.Substring(0, Text.Length - Quantity);
                }
                else if (Quantity < 0)
                {
                    return Text.Substring(0, Text.Length + Quantity);
                }
                else
                {
                    return Text;
                }
            }
            return Text;

        }

        /// <summary>
        /// Remove um texto do final de uma string se ele for um outro texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Texto final que será comparado</param>
        public static string RemoveLastEqual(this string Text, string EndStringTest, StringComparison comparison = default)
        {
            Text = Text ?? "";
            EndStringTest = EndStringTest ?? "";
            if (Text.EndsWith(EndStringTest, comparison))
            {
                Text = Text.RemoveLastChars(EndStringTest.Length);
            }

            return Text;
        }

        /// <summary>
        /// Remove a máscara de um texto, mantendo apenas números e os caracteres permitidos
        /// </summary>
        /// <param name="MaskedText"></param>
        /// <param name="AllowCharacters"></param>
        /// <returns></returns>
        public static string RemoveMask(this string MaskedText, params char[] AllowCharacters)
        {
            if (MaskedText.IsValid())
            {
                string ns = "";
                foreach (char c in MaskedText)
                {
                    if (c.ToString().IsNumber() || c.IsIn(AllowCharacters))
                    {
                        ns += c;
                    }
                }
                return ns;
            }
            return MaskedText;
        }

        /// <summary>
        /// Remove a máscara de um texto, mantendo apenas números e os caracteres permitidos
        /// Retorna um inteiro
        /// </summary>
        /// <param name="MaskedText"></param>
        /// <param name="AllowCharacters"></param>
        /// <returns></returns>
        public static int RemoveMaskInt(this string MaskedText, params char[] AllowCharacters) => RemoveMask(MaskedText, AllowCharacters).ToInt();

        /// <summary>
        /// Remove a máscara de um texto, mantendo apenas números e os caracteres permitidos
        /// Retorna um long
        /// </summary>
        /// <param name="MaskedText"></param>
        /// <param name="AllowCharacters"></param>
        /// <returns></returns>
        public static long RemoveMaskLong(this string MaskedText, params char[] AllowCharacters) => RemoveMask(MaskedText, AllowCharacters).ToLong();

        /// <summary>
        /// Remove caracteres não prantáveis de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String corrigida</returns>
        public static string RemoveNonPrintable(this string Text)
        {
            foreach (char c in Text.ToCharArray())
            {
                if (char.IsControl(c))
                {
                    Text = Text.ReplaceNone(Convert.ToString(c));
                }
            }

            return Text.Trim();
        }

        /// <summary>
        /// Remove um parametro da QueryInterpolated String de uma URL
        /// </summary>
        /// <param name="Url">Uri</param>
        /// <param name="Key">Nome do parâmetro</param>
        /// <param name="Values">Valor do Parâmetro</param>
        /// <returns></returns>
        public static Uri RemoveParameter(this Uri Url, params string[] Keys)
        {
            var UriBuilder = new UriBuilder(Url);
            var query = UriBuilder.Query.ParseQueryString();
            Keys = Keys != null && Keys.Any() ? Keys : query.AllKeys;
            foreach (var k in Keys)
            {
                try
                {
                    query.Remove(k);
                }
                catch
                {
                }
            }

            UriBuilder.Query = query.ToQueryString();
            return UriBuilder.Uri;
        }

        /// <summary>
        /// Remove os parâmetros de uma URL, deixando apenas o caminho
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string RemoveUrlParameters(this string URL)
        {
            if ((URL.IsURL()))
            {
                URL = Regex.Replace(URL, @"{([^:]+)\s*:\s*(.+?)(?<!\\)}", EmptyString);
                URL = URL.RemoveLastEqual("/");
            }
            return URL;
        }

        public static string RemoveUrlParameters(Uri URL) => RemoveUrlParameters(URL?.ToString());

        /// <summary>
        /// Remove itens de uma lista com base em uma condição
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> RemoveWhere<T>(this List<T> list, Expression<Func<T, bool>> predicate)
        {
            if (list != null)
            {
                if (predicate != null)
                {
                    while (true)
                    {
                        var obj = list.FirstOrDefault(predicate.Compile());
                        if (obj != null)
                        {
                            list.Remove(obj);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Retorna o diretório pai de um <see cref="FileSystemInfo"/>
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static DirectoryInfo GetParent(this FileSystemInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info), "info cannot be null");
            return Path.GetDirectoryName(info.FullName).ToFileSystemInfo() as DirectoryInfo ?? throw new ArgumentException("info must be a FileInfo or DirectoryInfo", nameof(info));

        }

        /// <summary>
        /// Renomeia um <typeparamref name="T"/> e retorna um novo <typeparamref name="T"/>
        /// </summary>
        /// <param name="info">Arquivo ou Diretório</param>
        /// <param name="Name">Novo nome</param>
        /// <param name="KeepOriginalExtension">Se TRUE, mantém a extensão original do arquivo
        /// <returns></returns>
        public static T Rename<T>(this T info, string Name, bool KeepOriginalExtension = false) where T : FileSystemInfo
        {
            if (info != null && info is FileInfo File && Name.IsValid() && File.Exists)
            {
                if (KeepOriginalExtension || Path.GetExtension(Name).IsBlank())
                {
                    Name = $"{Path.GetFileNameWithoutExtension(Name)}{File.Extension}";
                }

                var pt = Path.Combine(File.DirectoryName, Name);
                File.MoveTo(pt);
                info = new FileInfo(pt) as T;
            }
            else if (info != null && info is DirectoryInfo Directory && Name.IsValid() && Directory.Exists)
            {
                var pt = Path.Combine(Directory.Parent.FullName, Name);
                Directory.MoveTo(pt);
                info = new DirectoryInfo(pt) as T;
            }

            return info as T ?? throw new ArgumentException("info must be a FileInfo or DirectoryInfo", nameof(info));
        }


        /// <summary>
        /// Repete uma string um numero determinado de vezes
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public static string Repeat(this string Text, int Times = 2)
        {
            var ns = EmptyString;
            while (Times > 0)
            {
                ns += Text;
                Times--;
            }
            return ns;
        }

        /// <summary>
        /// Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum
        /// </summary>
        /// <param name="Strings">Array de strings</param>
        /// <param name="OldValue">Valor antigo que será substituido</param>
        /// <param name="NewValue">Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE
        /// realiza um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
        /// </param>
        /// <returns></returns>
        public static string[] Replace(this string[] Strings, string OldValue, string NewValue, bool ReplaceIfEquals = true)
        {
            var NewArray = Strings;
            for (int index = 0, loopTo = Strings.Length - 1; index <= loopTo; index++)
            {
                if (ReplaceIfEquals)
                {
                    if ((NewArray[index] ?? EmptyString) == (OldValue ?? EmptyString))
                    {
                        NewArray[index] = NewValue;
                    }
                }
                else
                {
                    NewArray[index] = NewArray[index].Replace(OldValue, NewValue);
                }
            }

            return NewArray;
        }

        /// <summary>
        /// Faz uma busca em todos os elementos de uma lista e aplica um ReplaceFrom comum
        /// </summary>
        /// <param name="Strings">Array de strings</param>
        /// <param name="OldValue">Valor antigo que será substituido</param>
        /// <param name="NewValue">Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE
        /// realiza um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
        /// </param>
        /// <returns></returns>
        public static IEnumerable<string> Replace(this IEnumerable<string> Strings, string OldValue, string NewValue, bool ReplaceIfEquals = true) => Strings.ToArray().Replace(OldValue, NewValue, ReplaceIfEquals).ToList();

        /// <summary>
        /// Retorna um novo <see cref="FileInfo"/> substituindo a extensão original por <paramref name="Extension"/>
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="Extension"></param>
        /// <returns></returns>
        public static FileInfo ReplaceExtension(this FileInfo Info, string Extension)
        {
            if (Info != null)
                return new FileInfo(Path.Combine(Info.DirectoryName, $"{Info.GetFileNameWithoutExtension()}.{Extension.IfBlank("bin").TrimStart('.')}").FixPath());
            return null;
        }

        /// <summary>
        /// Substitui a primeira ocorrencia de um texto por outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OldText"></param>
        /// <param name="NewText"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string Text, string OldText, string NewText = EmptyString, StringComparison Comparison = StringComparison.CurrentCulture)
        {
            if (Text.Contains(OldText))
            {
                Text = Text.Insert(Text.IndexOf(OldText, Comparison), NewText);
                Text = Text.Remove(Text.IndexOf(OldText, Comparison), 1);
            }

            return Text;
        }

        /// <summary>
        /// Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string, string> Dic)
        {
            if (Dic != null && Text.IsValid())
            {
                foreach (var p in Dic)
                {
                    Text = Text.Replace(p.Key, p.Value);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
        /// </summary>
        public static string ReplaceFrom<T>(this string Text, IDictionary<string, T> Dic)
        {
            if (Dic != null && Text.IsValid())
            {
                foreach (var p in Dic)
                {
                    switch (true)
                    {
                        case object _ when p.Value.IsDictionary():
                            {
                                Text = Text.ReplaceFrom((IDictionary<string, object>)p.Value);
                                break;
                            }

                        case object _ when typeof(T).IsAssignableFrom(typeof(Array)):
                            {
                                foreach (var item in ForceArray(p.Value, typeof(T)))
                                {
                                    Text = Text.ReplaceMany(p.Key, ForceArray(p.Value, typeof(T)).Cast<string>().ToArray());
                                }

                                break;
                            }

                        default:
                            {
                                Text = Text.Replace(p.Key, p.Value.ToString());
                                break;
                            }
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string, string[]> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsValid())
            {
                foreach (var p in Dic)
                {
                    Text = Text.SensitiveReplace(p.Key, p.Value, Comparison);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string[], string> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsValid())
            {
                foreach (var p in Dic)
                {
                    Text = Text.SensitiveReplace(p.Value, p.Key.ToArray(), Comparison);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string[], string[]> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsValid())
            {
                foreach (var p in Dic)
                {
                    var froms = p.Key.ToList();
                    var tos = p.Value.ToList();
                    while (froms.Count > tos.Count)
                    {
                        tos.Add(EmptyString);
                    }

                    for (int i = 0, loopTo = froms.Count - 1; i <= loopTo; i++)
                    {
                        Text = Text.SensitiveReplace(froms[i], tos[i], Comparison);
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Substitui a ultima ocorrencia de um texto por outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OldText"></param>
        /// <param name="NewText"></param>
        /// <returns></returns>
        public static string ReplaceLast(this string Text, string OldText, string NewText = EmptyString, StringComparison comparison = StringComparison.CurrentCulture)
        {
            if (Text != null)
                if (Text.Contains(OldText))
                {
                    Text = Text.Insert(Text.LastIndexOf(OldText, comparison), NewText);
                    Text = Text.Remove(Text.LastIndexOf(OldText, comparison), 1);
                }

            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por um novo valor.
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="NewValue">Novo Valor</param>
        /// <param name="OldValues">Valores a serem substituido por um novo valor</param>
        /// <returns></returns>
        public static string ReplaceMany(this string Text, string NewValue, params string[] OldValues)
        {
            Text = Text ?? EmptyString;
            foreach (var word in (OldValues ?? Array.Empty<string>()).Where(x => x.Length > 0))
            {
                Text = Text.Replace(word, NewValue);
            }

            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por vazio.
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="OldValue">Valor a ser substituido por vazio</param>
        /// <returns>String corrigida</returns>
        public static string ReplaceNone(this string Text, string OldValue) => Text.Replace(OldValue, EmptyString);

        /// <summary>
        /// Substitui os parametros de rota de uma URL por valores de um objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string ReplaceUrlParameters<T>(this string URL, T obj)
        {
            if (URL.IsURL())
            {
                URL = Regex.Replace(URL, @"{([^:]+)\s*:\s*(.+?)(?<!\\)}", "{$1}");
                if (obj != null)
                {
                    URL = URL.Inject(obj);
                }

                URL = URL.RemoveLastEqual("/");
            }
            return URL;
        }

        public static string ReplaceUrlParameters<T>(Uri URL, T obj) => ReplaceUrlParameters(URL?.ToString(), obj);

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="ResizeExpression">uma string contendo uma expressão de tamanho</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, string ResizeExpression, bool OnlyResizeIfWider = true)
        {
            if (ResizeExpression.Contains("%"))
            {
                return Original.ResizePercent(ResizeExpression, OnlyResizeIfWider);
            }
            else
            {
                var s = ResizeExpression.ParseSize();
                return Original.Resize(s, OnlyResizeIfWider);
            }
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="Size">Tamanho</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, Size Size, bool OnlyResizeIfWider = true) => Original.Resize(Size.Width, Size.Height, OnlyResizeIfWider);

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="NewWidth">Nova Largura</param>
        /// <param name="MaxHeight">Altura máxima</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, int NewWidth, int MaxHeight, bool OnlyResizeIfWider = true)
        {
            if (Original == null)
            {
                return null;
            }

            Image fullsizeImage = new Bitmap(Original);
            if (OnlyResizeIfWider)
            {
                if (fullsizeImage.Width <= NewWidth)
                {
                    NewWidth = fullsizeImage.Width;
                }
            }

            int newHeight = (int)Math.Round(fullsizeImage.Height * NewWidth / (double)fullsizeImage.Width);
            if (newHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = (int)Math.Round(fullsizeImage.Width * MaxHeight / (double)fullsizeImage.Height);
                newHeight = MaxHeight;
            }

            fullsizeImage = fullsizeImage.GetThumbnailImage(NewWidth, newHeight, null, IntPtr.Zero);
            fullsizeImage.RotateFlip(Original.GetRotateFlip());
            return fullsizeImage;
        }

        /// <summary>
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height) => Image.Resize(Width, Height, false).Crop(Width, Height);

        /// <summary>
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height, bool OnlyResizeIfWider) => Image.Resize(Width, Height, OnlyResizeIfWider).Crop(Width, Height);

        /// <summary>
        /// Redimensiona uma imagem para o tamanho definido por uma porcentagem
        /// </summary>
        /// <param name="Original"></param>
        /// <param name="Percent">Porcentagem ( no formato '30% 'ou '20% x 10%')</param>
        /// <param name="OnlyResizeIfWider"></param>
        /// <returns></returns>
        public static Image ResizePercent(this Image Original, string Percent, bool OnlyResizeIfWider = true)
        {
            var size = new Size();
            if (Percent.Contains("x"))
            {
                var parts = Percent.Split("x");
                if (parts[0].TrimBetween().EndsWith("%"))
                {
                    parts[0] = parts[0].TrimBetween().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString();
                }

                if (parts[1].TrimBetween().EndsWith("%"))
                {
                    parts[1] = parts[1].TrimBetween().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString();
                }

                size = new Size(parts[0].ToInt(), parts[1].ToInt());
            }
            else
            {
                if (Percent.TrimBetween().EndsWith("%"))
                {
                    Percent = Percent.Trim('%').TrimBetween();
                }

                if (Percent.IsNumber())
                {
                    size.Width = Convert.ToInt32(Percent.ToInt().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString());
                    size.Height = Convert.ToInt32(Percent.ToInt().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString());
                }
            }

            return Original.Resize(size, OnlyResizeIfWider);
        }

        public static Image ResizePercent(this Image Original, decimal Percent, bool OnlyResizeIfWider = true) => Original.ResizePercent(Percent.ToPercentString(), OnlyResizeIfWider);

        /// <summary>
        /// Arredonda um numero para um numero especifico de digitos fracionários
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static decimal RoundDecimal(this decimal Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number, Decimals.Value.ForcePositive()) : Math.Round(Number);
        /// <summary>
        /// Arredonda um numero para um numero especifico de digitos fracionários
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static decimal RoundDecimal(this double Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number.ToDecimal(), Decimals.Value.ForcePositive()) : Math.Round(Number.ToDecimal());

        /// <summary>
        /// Arredonda um numero para um numero especifico de digitos fracionários
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static double RoundDouble(this double Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number, Decimals.Value.ForcePositive()) : Math.Round(Number);
        /// <summary>
        /// Arredonda um numero para um numero especifico de digitos fracionários
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
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
        /// <param name="Number"></param>
        /// <returns></returns>
        public static short RoundShort(this decimal Number) => Math.Round(Number).ToShort();

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
        /// Salva um anexo para um diretório
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="Directory"></param>
        /// <returns></returns>
        public static FileInfo SaveMailAttachment(this Attachment attachment, DirectoryInfo Directory, DateTime? DateAndTime = null) => attachment.SaveMailAttachment(Directory.FullName, DateAndTime);

        /// <summary>
        /// Salva um anexo para um caminho
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static FileInfo SaveMailAttachment(this Attachment attachment, string FilePath, DateTime? DateAndTime = null)
        {
            if (attachment != null)
            {
                if (FilePath.IsDirectoryPath())
                {
                    FilePath = FilePath + @"\" + attachment.Name.IfBlank(attachment.ContentId);
                }

                return attachment.ToBytes().WriteToFile(FilePath, DateAndTime);
            }

            return null;
        }

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static IEnumerable<FileSystemInfo> Search(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches)
        {
            var FilteredList = new List<FileSystemInfo>();
            foreach (string pattern in (Searches ?? Array.Empty<string>()).SelectMany(z => z.SplitAny(":", "|")).Where(x => x.IsValid()).DefaultIfEmpty("*"))
            {
                if (Directory != null)
                    try
                    {
                        FilteredList.AddRange(Directory.EnumerateFileSystemInfos(pattern.Trim(), SearchOption));

                    }
                    catch
                    {

                    }
            }

            return FilteredList;
        }

        /// <summary>
        /// Retorna um <see cref="IQueryable{T}"/> procurando em varios campos diferentes de uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo da Entidade</typeparam>
        /// <param name="Table">Tabela da Entidade</param>
        /// <param name="SearchTerms">Termos da pesquisa</param>
        /// <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        /// <returns></returns>
        public static IQueryable<T> Search<T>(this IQueryable<T> Table, IEnumerable<string> SearchTerms, params Expression<Func<T, string>>[] Properties) where T : class
        {
            Properties = Properties ?? Array.Empty<Expression<Func<T, string>>>();
            SearchTerms = SearchTerms ?? Array.Empty<string>().AsEnumerable();
            return Table.Where(SearchTerms.SearchExpression(Properties));
        }

        public static IQueryable<T> Search<T>(this IQueryable<T> Table, string SearchTerm, params Expression<Func<T, string>>[] Properties) where T : class => Search(Table, new[] { SearchTerm }, Properties);

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas
        /// dentro de um range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static IEnumerable<FileSystemInfo> SearchBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            FixOrder(ref FirstDate, ref SecondDate);
            return Directory.Search(SearchOption, Searches).Where(file => file.LastWriteTime >= FirstDate && file.LastWriteTime <= SecondDate).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        /// <summary>
        /// Retorna uma lista de diretórios baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectories(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches)
        {
            var FilteredList = new List<DirectoryInfo>();
            foreach (string pattern in (Searches ?? Array.Empty<string>()).Where(x => x.IsValid()).DefaultIfEmpty("*"))
            {
                if (Directory != null)
                    FilteredList.AddRange(Directory.GetDirectories(pattern.Trim(), SearchOption));
            }

            return FilteredList;
        }

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um
        /// range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> SearchDirectoriesBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            FixOrder(ref FirstDate, ref SecondDate);
            return Directory.SearchDirectories(SearchOption, Searches).Where(file => file.LastWriteTime >= FirstDate && file.LastWriteTime <= SecondDate).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        public static Expression<Func<T, bool>> SearchExpression<T>(this IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
        {
            Properties = Properties ?? Array.Empty<Expression<Func<T, string>>>();
            Text = Text?.WhereNotBlank() ?? Array.Empty<string>();

            var predi = (!Text.Any()).CreateWhereExpression<T>();

            foreach (var prop in Properties)
            {
                foreach (var s in Text)
                {
                    if (s.IsValid())
                    {
                        var param = prop.Parameters.First();
                        var con = Expression.Constant(s);
                        var lk = Expression.Call(prop.Body, containsMethod, con);
                        var lbd = Expression.Lambda<Func<T, bool>>(lk, param);
                        predi = predi.Or(lbd);
                    }
                }
            }

            return predi;
        }

        public static Expression<Func<T, bool>> SearchExpression<T>(this string Text, params Expression<Func<T, string>>[] Properties)
        => (new[] { Text }).SearchExpression(Properties);

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches) => (Searches ?? Array.Empty<string>()).Where(x => x.IsValid()).DefaultIfEmpty("*").SelectMany(x => Directory.GetFiles(x.Trim(), SearchOption));

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um
        /// range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFilesBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            FixOrder(ref FirstDate, ref SecondDate);
            return Directory.SearchFiles(SearchOption, Searches).Where(file => file.LastWriteTime.IsBetween(FirstDate, SecondDate)).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        /// <summary>
        /// Retorna um <see cref="IQueryable{ClassType}"/> procurando em varios campos diferentes de
        /// uma entidade
        /// </summary>
        /// <typeparam name="TClass">Tipo da Entidade</typeparam>
        /// <param name="Table">Tabela da Entidade</param>
        /// <param name="SearchTerms">Termos da pesquisa</param>
        /// <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        /// <returns></returns>
        public static IOrderedEnumerable<TClass> SearchInOrder<TClass>(this IEnumerable<TClass> Table, IEnumerable<string> SearchTerms, bool Ascending, params Expression<Func<TClass, string>>[] Properties) where TClass : class
        {
            IOrderedEnumerable<TClass> SearchRet = default;
            Properties = Properties ?? Array.Empty<Expression<Func<TClass, string>>>();
            SearchTerms = SearchTerms ?? Array.Empty<string>().AsEnumerable();
            SearchRet = null;
            Table = Table.Where(SearchTerms.SearchExpression(Properties).Compile());
            foreach (var prop in Properties)
            {
                SearchRet = (SearchRet ?? Table.OrderBy(x => true)).ThenByLike(prop.Compile(), Ascending, SearchTerms.ToArray());
            }

            return SearchRet;
        }

        public static IOrderedEnumerable<TClass> SearchInOrder<TClass>(this IEnumerable<TClass> Table, string SearchTerms, bool Ascending, params Expression<Func<TClass, string>>[] Properties) where TClass : class
        => SearchInOrder(Table, ForceArray<string>(SearchTerms), Ascending, Properties);

        public static IOrderedEnumerable<TClass> SearchInOrder<TClass>(this IEnumerable<TClass> Table, IEnumerable<string> SearchTerms, params Expression<Func<TClass, string>>[] Properties) where TClass : class => SearchInOrder(Table, SearchTerms, true, Properties);

        public static IOrderedQueryable<TClass> SearchInOrder<TClass>(this IQueryable<TClass> Table, IEnumerable<string> SearchTerms, params Expression<Func<TClass, string>>[] Properties) where TClass : class
        {
            var SearchRet = Table.Search(SearchTerms, Properties).OrderBy(x => true);
            foreach (var prop in Properties)
            {
                SearchRet = SearchRet.ThenByLike(SearchTerms, prop);
            }

            return SearchRet;
        }

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Array">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string SelectJoinString<Type>(string Separator, params Type[] Array) => Array.SelectJoinString(Separator);

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="List">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string SelectJoinString<Type>(this List<Type> List, string Separator = EmptyString) => List.ToArray().SelectJoinString(Separator);

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectJoinString<TSource>(this IEnumerable<TSource> Source, string Separator = EmptyString) => Source.SelectJoinString(null, Separator);

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectJoinString<TSource>(this IEnumerable<TSource> Source, Func<TSource, string> Selector, string Separator = EmptyString)
        {
            Selector = Selector ?? (x => $"{x}");
            Source = Source ?? Array.Empty<TSource>();
            return Source.Any() ? String.Join(Separator, Source.Select(Selector).ToArray()) : EmptyString;
        }

        public static IEnumerable<String> SelectLike(this IEnumerable<String> source, String Pattern) => source.Where(x => x.Like(Pattern));

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos enumeraveis
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectManyJoinString<TSource>(this IEnumerable<TSource> Source, Func<TSource, IEnumerable<string>> Selector = null, string Separator = EmptyString) => SelectJoinString(Source.SelectMany(Selector ?? (x => (new[] { x.ToString() }))), Separator);

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos enumeraveis
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectManyJoinString<TSource>(this IQueryable<TSource> Source, Func<TSource, IEnumerable<string>> Selector = null, string Separator = EmptyString) => Source.AsEnumerable().SelectManyJoinString(Selector, Separator);

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="NewValue"></param>
        /// <param name="OldValue"></param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string OldValue, string NewValue, StringComparison ComparisonType = StringComparison.InvariantCulture) => Text.SensitiveReplace(NewValue, new[] { OldValue }, ComparisonType);

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="NewValue"></param>
        /// <param name="OldValues"></param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string NewValue, IEnumerable<string> OldValues, StringComparison ComparisonType = StringComparison.InvariantCulture)
        {
            if (Text.IsValid())
            {
                foreach (var oldvalue in OldValues ?? new[] { EmptyString })
                {
                    NewValue = NewValue ?? EmptyString;
                    if (!oldvalue.Equals(NewValue, ComparisonType))
                    {
                        int foundAt;
                        do
                        {
                            foundAt = Text.IndexOf(oldvalue, 0, ComparisonType);
                            if (foundAt > -1)
                            {
                                Text = Text.Remove(foundAt, oldvalue.Length).Insert(foundAt, NewValue);
                            }
                        }
                        while (foundAt != -1);
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Adciona ou substitui um valor a este <see cref="Dictionary(Of TKey, TValue)"/> e retorna
        /// a mesma instancia deste <see cref="Dictionary(Of TKey, TValue)"/>
        /// </summary>
        /// <typeparam name="TKey">Tipo da Key</typeparam>
        /// <typeparam name="TValue">Tipo do valor</typeparam>
        /// <param name="Key">Valor da key</param>
        /// <param name="Value">Valor do Value</param>
        /// <returns>o mesmo objeto do tipo <see cref="Dictionary"/> que chamou este método</returns>
        public static IDictionary<TKey, TValue> Set<TKey, TValue, TK, TV>(this IDictionary<TKey, TValue> Dic, TK Key, TV Value)
        {
            if (Key != null && Dic != null)
            {
                Dic[Key.ChangeType<TKey>()] = Value.ChangeType<TValue>();
            }

            return Dic;
        }

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

        public static T SetOrRemove<T, TK, TV>(this T Dictionary, KeyValuePair<TK, TV> Pair) where T : IDictionary<TK, TV>
        {
            Dictionary?.SetOrRemove(Pair.Key, Pair.Value);
            return Dictionary;
        }

        public static IDictionary<TKey, string> SetOrRemove<TKey, TK>(this IDictionary<TKey, string> Dic, TK Key, string Value, bool NullIfBlank) => Dic.SetOrRemove(Key, NullIfBlank.AsIf(Value.NullIf(x => x.IsNotValid()), Value));

        public static IDictionary<TKey, TValue> SetOrRemove<TKey, TValue, TK, TV>(this IDictionary<TKey, TValue> Dic, TK Key, TV Value)
        {
            if (Dic != null && Key != null)
            {
                if (Value != null)
                {
                    Dic[Key.ChangeType<TKey>()] = Value.ChangeType<TValue>();
                }
                else
                {
                    Dic.RemoveIfExist(Key.ChangeType<TKey>());
                }
            }

            return Dic;
        }

        /// <summary>
        /// Seta o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <param name="PropertyName">Nome da properiedade</param>
        /// <param name="Value">Valor da propriedade definida por <paramref name="PropertyName"/></param>
        /// <typeparam name="T">
        /// Tipo do <paramref name="Value"/> da propriedade definida por <paramref name="PropertyName"/>
        /// </typeparam>
        public static T SetPropertyValue<T>(this T MyObject, string PropertyName, object Value) where T : class
        {
            if (PropertyName.IsValid() && MyObject != null)
            {
                var props = MyObject.GetProperties();

                var prop = props.FirstOrDefault(p => p != null && p.CanWrite && p.Name.IsAny(PropertyNamesFor(PropertyName).ToArray()));

                if (prop != null)
                    if (Value is DBNull)
                    {
                        prop.SetValue(MyObject, null);
                    }
                    else
                    {
                        prop.SetValue(MyObject, ChangeType(Value, prop.PropertyType));
                    }
            }

            return MyObject;
        }

        public static T SetPropertyValue<T, TProp>(this T obj, Expression<Func<T, TProp>> Selector, TProp Value) where T : class
        {
            obj?.SetPropertyValue(obj.GetPropertyInfo(Selector).Name, Value);
            return obj;
        }

        public static Task SetTimeout(int milliseconds, Action action) => Task.Delay(milliseconds).ContinueWith((t) =>
                                                                                     {
                                                                                         TryExecute(action);
                                                                                         t.Dispose();
                                                                                     });

        public static T SetValuesIn<T>(this Dictionary<string, object> Dic) => (T)Dic.CreateOrSetObject(null, typeof(T));

        /// <summary>
        /// Seta as propriedades de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <summary>
        /// Seta as propriedades e campos de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Obj"></param>
        public static T SetValuesIn<T>(this Dictionary<string, object> Dic, T obj, params object[] args) => (T)Dic.CreateOrSetObject(obj, typeof(T), args);

        /// <summary>
        /// Seta as propriedades e campos de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Obj"></param>
        public static T SetValuesIn<T>(this Dictionary<string, object> Dic, T obj) => (T)Dic.CreateOrSetObject(obj, typeof(T), null);

        public static T Show<T>(this T dir) where T : FileSystemInfo
        {
            if (dir != null && dir.Exists)
            {
                if (dir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    dir.Attributes &= ~FileAttributes.Hidden;
                }
            }
            return dir;
        }

        /// <summary>
        /// Randomiza a ordem dos itens de um Array
        /// </summary>
        /// <typeparam name="Type">Tipo do Array</typeparam>
        /// <param name="Array">Matriz</param>
        public static Type[] Shuffle<Type>(this Type[] Array) => Array.OrderByRandom().ToArray();

        /// <summary>
        /// Randomiza a ordem dos itens de uma Lista
        /// </summary>
        /// <typeparam name="Type">Tipo de Lista</typeparam>
        /// <param name="List">Matriz</param>
        public static List<Type> Shuffle<Type>(this List<Type> List) => List.OrderByRandom().ToList();

        /// <summary>
        /// Aleatoriza a ordem das letras de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string Shuffle(this string Text) => Text.OrderByRandom().SelectJoinString();

        /// <summary>
        /// Busca em um <see cref="IQueryable{T}"/> usando uma expressao lambda a partir do nome de
        /// uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="T"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static T SingleOrDefaultExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, object PropertyValue, bool Is = true) => List.SingleOrDefault(WhereExpression<T>(PropertyName, Operator, (IEnumerable<IComparable>)PropertyValue, Is));

        /// <summary>
        /// Retorna a frase ou termo especificado em sua forma singular
        /// </summary>
        /// <param name="Text">Texto no plural</param>
        /// <returns></returns>
        public static string Singularize(this string Text)
        {
            var phrase = Text.ApplySpaceOnWrapChars().Split(WhitespaceChar);
            for (int index = 0, loopTo = phrase.Length - 1; index <= loopTo; index++)
            {
                string endchar = phrase[index].GetLastChars();
                if (endchar.IsAny(StringComparison.CurrentCultureIgnoreCase, PredefinedArrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index].RemoveLastEqual(endchar);
                }

                switch (true)
                {
                    case object _ when phrase[index].IsNumber() || phrase[index].IsEmail() || phrase[index].IsURL() || phrase[index].IsIP() || phrase[index].IsIn(PredefinedArrays.WordSplitters):
                        {
                            // nao alterar estes tipos
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ões"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ões") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ãos"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ãos") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ães"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ães") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ais"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ais") + "al";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("eis"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("eis") + "il";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("éis"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("éis") + "el";

                            break;
                        }

                    case object _ when phrase[index].EndsWith("ois"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ois") + "ol";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("uis"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("uis") + "ul";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("es"):
                        {
                            if (phrase[index].RemoveLastEqual("es").EndsWithAny("z", "r"))
                            {
                                phrase[index] = phrase[index].RemoveLastEqual("es");
                            }
                            else
                            {
                                phrase[index] = phrase[index].RemoveLastEqual("s");
                            }

                            break;
                        }

                    case object _ when phrase[index].EndsWith("ns"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ns") + "m";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("s"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("s");
                            break;
                        }

                    default:
                        {
                            break;
                        }
                        // ja esta no singular
                }

                if (endchar.IsAny(StringComparison.CurrentCultureIgnoreCase, PredefinedArrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index] + endchar;
                }
            }

            return phrase.SelectJoinString(WhitespaceChar).TrimBetween();
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> l, int Count = 1)
        => l.Take(l.Count() - Count.LimitRange(0, l.Count()));

        public static Dictionary<TGroup, Dictionary<TCount, long>> SkipZero<TGroup, TCount>(this Dictionary<TGroup, Dictionary<TCount, long>> Grouped)
        {
            if (Grouped != null)
            {
                foreach (var dic in Grouped.ToArray())
                {
                    Grouped[dic.Key] = dic.Value.Where(x => x.Value > 0).ToDictionary();
                }

                Grouped = Grouped.Where(x => x.Value.Any()).ToDictionary();
            }

            return Grouped;
        }

        public static Dictionary<TCount, long> SkipZero<TCount>(this Dictionary<TCount, long> Grouped)
        {
            Grouped = Grouped?.Where(x => x.Value > 0).ToDictionary();
            return Grouped;
        }

        /// <summary>
        /// Separa um texto em um array de strings a partir de uma outra string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Separator">Texto utilizado como separador</param>
        /// <returns></returns>
        public static string[] Split(this string Text, string Separator, StringSplitOptions Options = StringSplitOptions.RemoveEmptyEntries) => (Text ?? EmptyString).Split(new[] { Separator }, Options);

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, params string[] SplitText) => Text?.SplitAny(StringSplitOptions.RemoveEmptyEntries, SplitText);

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, StringSplitOptions SplitOptions, params string[] SplitText) => Text?.Split(SplitText ?? Array.Empty<string>(), SplitOptions);

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, IEnumerable<string> SplitText) => Text?.SplitAny(SplitText.ToArray());

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, StringSplitOptions SplitOptions, IEnumerable<string> SplitText) => Text?.SplitAny(SplitOptions, SplitText.ToArray());

        public static IEnumerable<string> SplitChunk(this string input, params int[] chunkSizes)
        {
            if (input != null)
                while (input.Length > 0)
                {
                    var size = chunkSizes.IfNoIndex(0, input.Length);
                    if (size < 0) size = (input.Length - size).LimitRange(0, input.Length);


                    var chunk = input.GetFirstChars(size);
                    //if (chunk.Length == 0)
                    //{
                    //    if (input.Length > 0)
                    //        yield return input;
                    //    break;
                    //}
                    yield return chunk;
                    input = input.RemoveFirstChars(size);
                    chunkSizes = chunkSizes.Skip(1).ToArray();
                }
        }

        public static IEnumerable<string> SplitFixedChunk(this string inputString, int chunkSize)
        {
            inputString = inputString ?? EmptyString;
            if (chunkSize > 0 && inputString.Length > 0)
            {
                for (int i = 0; i < inputString.Length; i += chunkSize)
                {
                    int remainingLength = inputString.Length - i;
                    yield return inputString.Substring(i, remainingLength < chunkSize ? remainingLength : chunkSize);
                }
            }
            else yield return inputString;
        }

        public static IEnumerable<string> SplitPercentChunk(this string input, params string[] percents)
        {
            if (input != null && input.Length > 0)
            {
                var p = percents.Select(x => x.CalculateValueFromPercent(input.Length.ToDecimal()).RoundInt()).ToArray();

                foreach (var s in input.SplitChunk(p)) yield return s;
            }
        }

        /// <summary>
        /// Cria uma <see cref="List{T}"/> e adciona um objeto a ela. Util para tipos anonimos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> StartList<T>(this T ObjectForDefinition)
        {
            var d = DefineEmptyList(ObjectForDefinition);
            if (ObjectForDefinition != null)
            {
                d.Add(ObjectForDefinition);
            }

            return d;
        }

        /// <summary>
        /// Verifica se uma string começa com alguma outra string de um array
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string Text, StringComparison comparison, params string[] Words) => Words.Any(p => Text.IfBlank(EmptyString).StartsWith(p, comparison));

        public static bool StartsWithAny(this string Text, params string[] Words) => StartsWithAny(Text, StringComparison.InvariantCultureIgnoreCase, Words);

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
        /// Troca o valor de <paramref name="FirstValue"/> pelo valor de <paramref
        /// name="SecondValue"/> e o valor de <paramref name="SecondValue"/> pelo valor de <paramref name="FirstValue"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstValue"></param>
        /// <param name="SecondValue"></param>
        public static (T, T) Swap<T>(ref T FirstValue, ref T SecondValue)
        {
            (SecondValue, FirstValue) = (FirstValue, SecondValue);
            return (FirstValue, SecondValue);
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> l, int Count = 1) => l.Reverse().Take(Count).Reverse();

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> l, int Count = 1) => l.OrderByRandom().Take(Count);

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> l, Func<T, bool> predicade, int Count = 1) => l.Where(predicade).OrderByRandom().Take(Count);

        /// <summary>
        /// Traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> TakeTop<TKey, TValue>(this Dictionary<TKey, TValue> Dic, int Top, TKey GroupOthersLabel)
        {
            if (Dic == null)
            {
                return null;
            }

            if (Top < 1)
            {
                return Dic.ToDictionary();
            }

            var novodic = Dic.Take(Top).ToDictionary();
            if (GroupOthersLabel != null)
            {
                novodic[GroupOthersLabel] = Dic.Values.Skip(Top).Select(x => x.ChangeType<decimal>()).Sum().ChangeType<TValue>();
            }

            return novodic;
        }

        /// <summary>
        /// Traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<TKey, IEnumerable<T>> TakeTop<TKey, T>(this Dictionary<TKey, IEnumerable<T>> Dic, int Top, Expression<Func<T, dynamic>> ValueSelector) where T : class
        {
            Dictionary<TKey, IEnumerable<T>> novodic = Dic.ToDictionary();

            if (ValueSelector != null)
            {
                novodic = Dic.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(ValueSelector.Compile()).AsEnumerable());
            }

            if (Top > 0)
            {
                novodic = Dic.ToDictionary(x => x.Key, x => x.Value.TakeTop(Top, ValueSelector));
            }

            return novodic;
        }

        public static IEnumerable<T> TakeTop<T>(this IEnumerable<T> List, int Top, params Expression<Func<T, dynamic>>[] ValueSelector) where T : class => TakeTop<T, object>(List, Top, null, null, ValueSelector?.ToArray());

        public static IEnumerable<T> TakeTop<T, TLabel>(this IEnumerable<T> List, int Top, Expression<Func<T, TLabel>> LabelSelector, TLabel GroupOthersLabel, params Expression<Func<T, dynamic>>[] ValueSelector) where T : class
        {
            ValueSelector = ValueSelector ?? Array.Empty<Expression<Func<T, dynamic>>>();

            if (ValueSelector.WhereNotNull().IsNullOrEmpty())
            {
                throw new ArgumentException("You need at least one value selector", nameof(ValueSelector));
            }

            var newlist = List.OrderByManyDescending(ValueSelector).Take(Top).ToList();

            if (LabelSelector != null && GroupOthersLabel != null)
            {
                var others = Activator.CreateInstance<T>();
                LabelSelector.GetPropertyInfo().SetValue(others, GroupOthersLabel);
                foreach (var v in ValueSelector)
                {
                    var values = List.Skip(Top).Select(x => (v.Compile().Invoke(x) as object).ChangeType<decimal>()).Sum();
                    v.GetPropertyInfo().SetValue(others, values);
                }
                newlist.Add(others);
            }
            return newlist.AsEnumerable();
        }

        /// <summary>
        /// Rotaciona uma imagem para sua posição original caso ela já tenha sido rotacionada (EXIF)
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <returns>TRUE caso a imagem ja tenha sido rotacionada</returns>
        public static bool TestAndRotate(this Image Img)
        {
            var rft = Img.GetRotateFlip();
            if (rft != RotateFlipType.RotateNoneFlipNone)
            {
                Img.RotateFlip(rft);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable{T}"/> a partir da aproximação de uma ou mais <see
        /// cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="Searches"></param>
        /// <param name="SortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByLike<T>(this IQueryable<T> items, string[] Searches, string SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            Searches = Searches ?? Array.Empty<string>();
            if (Searches.Any())
            {
                foreach (var t in Searches)
                {
                    var property = type.GetProperty(SortProperty);
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    var testes = new[] { Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)), Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)), Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)), Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t)) };
                    foreach (var exp in testes)
                    {
                        var nv = Expression.Lambda<Func<T, bool>>(exp, parameter);
                        if (Ascending)
                        {
                            if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                            {
                                items = items.OrderByDescending(nv);
                            }
                            else
                            {
                                items = ((IOrderedQueryable<T>)items).ThenByDescending(nv);
                            }
                        }
                        else if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                        {
                            items = items.OrderBy(nv);
                        }
                        else
                        {
                            items = ((IOrderedQueryable<T>)items).ThenBy(nv);
                        }
                    }
                }
            }
            else
            {
                items = items.OrderBy(x => 0);
            }

            return (IOrderedQueryable<T>)items;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable{T}"/> a partir da aproximação de uma ou mais <see
        /// cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="Searches"></param>
        /// <param name="SortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByLike<T>(this IQueryable<T> items, IEnumerable<string> Searches, Expression<Func<T, string>> SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            Searches = Searches ?? Array.Empty<string>();
            if (items != null)
            {
                if (Searches.Any() && SortProperty != null)
                {
                    foreach (var t in Searches)
                    {
                        MemberExpression mem = SortProperty.Body as MemberExpression;
                        var property = mem.Member;
                        var parameter = SortProperty.Parameters.First();
                        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                        var orderByExp = Expression.Lambda(propertyAccess, parameter);
                        var tests = new[] { Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)), Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)), Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)), Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t)) };
                        foreach (var exp in tests)
                        {
                            var nv = Expression.Lambda<Func<T, bool>>(exp, parameter);
                            if (Ascending)
                            {
                                if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                                {
                                    items = items.OrderByDescending(nv);
                                }
                                else
                                {
                                    items = ((IOrderedQueryable<T>)items).ThenByDescending(nv);
                                }
                            }
                            else if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                            {
                                items = items.OrderBy(nv);
                            }
                            else
                            {
                                items = ((IOrderedQueryable<T>)items).ThenBy(nv);
                            }
                        }
                    }
                }
                else
                {
                    items = items.OrderBy(x => 0);
                }
                return (IOrderedQueryable<T>)items;
            }
            throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable{T}"/> a partir da aproximação de uma ou mais <see
        /// cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="PropertySelector"></param>
        /// <param name="Ascending"></param>
        /// <param name="Searches"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, params string[] Searches) where T : class => ThenByLike(items, PropertySelector, Ascending, StringComparison.InvariantCultureIgnoreCase, Searches);

        public static IOrderedEnumerable<T> ThenByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, StringComparison Comparison, params string[] Searches) where T : class
        {
            Searches = Searches ?? Array.Empty<string>();
            if (items != null)
            {
                IOrderedEnumerable<T> newitems = items is IOrderedEnumerable<T> oitems ? oitems : items.OrderBy(x => 0);

                if (Searches.Any())
                {
                    Searches.Each(s =>
                    {
                        bool func(T x) => PropertySelector.Invoke(x).Equals(s, Comparison);
                        newitems = Ascending ? newitems.ThenByDescending(func) : newitems.ThenBy(func);
                    });

                    Searches.Each(s =>
                    {
                        int func(T x) => PropertySelector.Invoke(x).IndexOf(s, Comparison);
                        newitems = Ascending ? newitems.ThenBy(func) : newitems.ThenByDescending(func);
                    });
                }

                return newitems;
            }
            throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable{T}"/> a partir de outra lista do mesmo tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="OrderSource"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByList<T>(this IOrderedEnumerable<T> Source, params T[] OrderSource)
        => Source.ThenBy(d => Array.IndexOf(OrderSource, d));

        /// <summary>
        /// Ordena um <see cref="IQueryable{T}"/> a partir do nome de uma ou mais propriedades
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByProperty<T>(this IQueryable<T> source, string[] SortProperty, bool Ascending = true)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var type = typeof(T);
            SortProperty = SortProperty ?? Array.Empty<string>();
            foreach (var prop in SortProperty)
            {
                var property = type.FindProperty(prop);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var typeArguments = new Type[] { type, property.PropertyType };
                string methodname = source.GetType() == typeof(IOrderedQueryable<T>)
                    ? Ascending ? "ThenBy" : "ThenByDescending"
                    : Ascending ? "OrderBy" : "OrderByDescending";
                var resultExp = Expression.Call(typeof(Queryable), methodname, typeArguments, source.Expression, Expression.Quote(orderByExp));
                source = source.Provider.CreateQuery<T>(resultExp);
            }
            return source.GetType() != typeof(IOrderedQueryable<T>) ? source.OrderBy(x => true) : (IOrderedQueryable<T>)source;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable{T}"/> a partir do nome de uma ou mais propriedades
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByProperty<T>(this IEnumerable<T> source, string[] SortProperty, bool Ascending = true)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var type = typeof(T);
            SortProperty = SortProperty ?? Array.Empty<string>();
            foreach (var prop in SortProperty)
            {
                var propInfo = FindProperty(typeof(T), prop);

                if (propInfo != null)
                {
                    object exp(T x) => propInfo.GetValue(x);
                    if (source.GetType() == typeof(IOrderedEnumerable<T>))
                    {
                        source = Ascending ? ((IOrderedEnumerable<T>)source).ThenBy(exp) : ((IOrderedEnumerable<T>)source).ThenByDescending(exp);
                    }
                    else
                    {
                        source = Ascending ? source.OrderBy(exp) : source.OrderByDescending(exp);
                    }
                }
            }

            return source.GetType() != typeof(IOrderedEnumerable<T>) ? source.OrderBy(x => true) : (IOrderedEnumerable<T>)source;
        }

        /// <summary>
        /// Converte uma matriz denteada para uma matriz multidemensional
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T[,] To2D<T>(this T[][] source)
        {
            if (source != null)
            {
                int FirstDim = source.Length;
                int SecondDim = source.GroupBy(row => row.Length).Max().Key;

                var result = new T[FirstDim, SecondDim];
                for (int i = 0; i < FirstDim; ++i)
                    for (int j = 0; j < SecondDim; ++j)
                        result[i, j] = source[i].IfNoIndex(j);

                return result;
            }
            return default;
        }

        /// <summary>
        /// Alterna maiusculas e minusculas para cada letra de uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToAlternateCase(this string Text)
        {
            var ch = Text.ToArray();
            for (int index = 0, loopTo = ch.Length - 1; index <= loopTo; index++)
            {
                char antec = ch.IfNoIndex(index - 1, '\0');
                if (antec.ToString().IsNotValid() || char.IsLower(antec) || antec.ToString() == null)
                {
                    ch[index] = char.ToUpper(ch[index]);
                }
                else
                {
                    ch[index] = char.ToLower(ch[index]);
                }
            }

            return new string(ch);
        }

        /// <summary>
        /// Retorna um anagrama de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string ToAnagram(this string Text) => Shuffle(Text);

        /// <summary>
        /// Return a Arabic number for a Roman number
        /// </summary>
        /// <param name="RomanNumber"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static int ToArabic(this string RomanNumber)
        {
            RomanNumber = $"{RomanNumber}".ToUpper(CultureInfo.InvariantCulture).Replace("IIII", "IV").Trim();
            if (RomanNumber == "N" || RomanNumber.IsNotValid())
            {
                return 0;
            }

            // Os numerais que representam números que começam com um '5'(TV, L e D) podem aparecer
            // apenas uma vez em cada numeral romano. Esta regra permite XVI, mas não VIV.
            if (RomanNumber.Split('V').Length > 2 || RomanNumber.Split('L').Length > 2 || RomanNumber.Split('D').Length > 2)
            {
                throw new ArgumentException("Roman number with invalid numerals. The number has a TV, L or D number repeated.");
            }

            // Uma única letra pode ser repetida até três vezes consecutivamente sendo que cada
            // ocorrência será somanda. Isto significa que I é um, II e III significa dois é três.
            // No entanto, IIII não é permitido.
            int contador = 1;
            char ultimo = 'Z';
            foreach (char numeral in RomanNumber)
            {
                // caractere inválido ?
                if ("IVXLCDM".IndexOf(numeral) == -1)
                {
                    throw new ArgumentException("Roman number with invalid positioning.");
                }

                // Duplicado?
                if (numeral == ultimo)
                {
                    contador += 1;
                    if (contador == 4)
                    {
                        throw new ArgumentException("A Roman number can not be repeated more than 3 times in the same number.");
                    }
                }
                else
                {
                    contador = 1;
                    ultimo = numeral;
                }
            }

            // Cria um ArrayList contendo os valores
            int ptr = 0;
            var valores = new ArrayList();
            int digitoMaximo = 1000;
            while (ptr < RomanNumber.Length)
            {
                // valor base do digito
                char numeral = RomanNumber[ptr];
                int digito = Convert.ToInt32(Enum.Parse(typeof(RomanDigit), numeral.ToString()));

                // Um numeral de pequena valor pode ser colocado à esquerda de um valor maior Quando
                // isto ocorre, por exemplo IX, o menor número é subtraído do maior TEntity dígito
                // subtraído deve ser de pelo menos um décimo do valor do maior numeral e deve ser
                // ou I, X ou C Valores como MCMD ou CMC não são permitidos
                if (digito > digitoMaximo)
                {
                    throw new ArgumentException("Roman number with invalid positioning.");
                }

                if (ptr < RomanNumber.Length - 1)
                {
                    char proximoNumeral = RomanNumber[ptr + 1];
                    // proximo digito
                    int proximoDigito = Convert.ToInt32(Enum.Parse(typeof(RomanDigit), proximoNumeral.ToString()));
                    if (proximoDigito > digito)
                    {
                        if ("IXC".IndexOf(numeral) == -1 || proximoDigito > digito * 10 || RomanNumber.Split(numeral).Length > 3)
                        {
                            throw new ArgumentException("Rule 3");
                        }

                        digitoMaximo = digito - 1;
                        digito = proximoDigito - digito;
                        ptr += 1;
                    }
                }

                valores.Add(digito);

                // proximo digito
                ptr += 1;
            }

            // Outra regra é a que compara o tamanho do valor de cada numeral lido a partir da
            // esquerda para a direita. TEntity valor nunca deve aumentar a partir de uma letra para a
            // próxima. Onde houver um numeral subtrativo, esta regra se aplica ao valor combinado
            // dos dois algarismos envolvidos na subtração quando comparado com a letra anterior.
            // Isto significa que XIX é aceitável, mas XIM e IIV não são.
            for (int i = 0, loopTo = valores.Count - 2; i <= loopTo; i++)
            {
                if (Convert.ToInt32(valores[i]) < Convert.ToInt32(valores[i + 1]))
                {
                    throw new ArgumentException("Invalid Roman number. In this case the digit can not be greater than the previous one.");
                }
            }

            // Numerais maiores devem ser colocados à esquerda dos números menores para continuar a
            // combinação aditiva. Assim VI é igual a seis e MDCLXI é 1.661.
            int total = 0;
            foreach (int digito in valores)
                total += digito;
            return total;
        }

        public static IEnumerable<int> ToAsc(this string c) => c.ToArray().Select(x => x.ToAsc());

        public static int ToAsc(this char c)
        {
            int converted = c;
            if (converted >= 0x80)
            {
                byte[] buffer = new byte[2];
                // if the resulting conversion is 1 byte in length, just use the value
                if (Encoding.Default.GetBytes(new char[] { c }, 0, 1, buffer, 0) == 1)
                {
                    converted = buffer[0];
                }
                else
                {
                    // byte swap bytes 1 and 2;
                    converted = buffer[0] << 16 | buffer[1];
                }
            }
            return converted;
        }

        public static byte ToAscByte(this char c) => (byte)c.ToAsc();

        public static string ToAsciiArt(this Bitmap image, int ratio)
        {
            image = (Bitmap)image.Negative();
            bool toggle = false;
            var sb = new StringBuilder();
            int h = 0;
            while (h < image.Height)
            {
                int w = 0;
                while (w < image.Width)
                {
                    var pixelColor = image.GetPixel(w, h);
                    int red, green, blue;
                    red = (int)Math.Round((pixelColor.R.ToInt() + pixelColor.G.ToInt() + pixelColor.B.ToInt()) / 3d);
                    green = red;
                    blue = green;
                    var grayColor = Color.FromArgb(red, green, blue);
                    if (!toggle)
                    {
                        int index = (int)Math.Round(grayColor.R * 10 / 255d);
                        sb.Append(PredefinedArrays.AsciiArtChars.ToArray()[index]);
                    }

                    w += ratio;
                }

                if (!toggle)
                {
                    sb.AppendLine();
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }

                h += ratio;
            }

            return sb.ToString();
        }

        public static Attachment ToAttachment(this FileInfo file) => file != null && file.Exists ? new Attachment(file.FullName) : null;

        public static Attachment ToAttachment(this Stream stream, string name) => stream != null && stream.Length > 0 ? new Attachment(stream, name.IfBlank("untitledFile.bin")) : null;

        public static Attachment ToAttachment(this byte[] bytes, string name)
        {
            if (bytes != null && bytes.Any())
                using (var s = new MemoryStream(bytes))
                    return s.ToAttachment(name);
            return null;
        }

        /// <summary>
        /// Converte um Array de Bytes em uma string Util
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <returns></returns>
        public static string ToBase64(this byte[] Bytes) => Convert.ToBase64String(Bytes);

        public static string ToBase64(this Image OriginalImage, ImageFormat OriginalImageFormat = null)
        {
            if (OriginalImage != null)
                using (var ms = new MemoryStream())
                {
                    OriginalImage.Save(ms, OriginalImageFormat ?? OriginalImage.GetImageFormat() ?? ImageFormat.Png);
                    var imageBytes = ms.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            return null;
        }

        /// <summary>
        /// Converte uma Imagem da WEB para Base64
        /// </summary>
        /// <param name="ImageURL">Caminho da imagem</param>
        /// <returns>Uma string em formato Util</returns>
        public static string ToBase64(this Uri ImageURL, ImageFormat OriginalImageFormat, NameValueCollection Headers = null, Encoding Encoding = null) => ImageURL?.DownloadImage(Headers, Encoding)?.ToBase64(OriginalImageFormat);



        /// <summary>
        /// Retorna uma <see cref="Bitmap"/> a partir de um Image
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this Image Image) => new Bitmap(Image);

        /// <summary>
        /// Converte um ToType para Boolean. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static bool ToBool<T>(this T Value) => Value.ChangeType<bool>();

        /// <summary>
        /// Transforma uma imagem em array de bytes
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image Image, ImageFormat Format = null)
        {
            using (var ms = Image.ToStream(Format))
            {
                return ms.ToBytes();
            }
        }

        /// <summary>
        /// Salva um anexo para Byte()
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Attachment attachment) => attachment?.ContentStream.ToBytes() ?? Array.Empty<byte>();

        /// <summary>
        /// Converte um stream em Bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            if (stream == null)
            {
                return Array.Empty<byte>();
            }

            if (stream is MemoryStream ms)
            {
                return ms.ToArray();
            }

            var pos = stream.Position;
            using (var ms2 = new MemoryStream())
            {
                stream.CopyTo(ms2);
                stream.Position = pos;
                return ms2.ToArray();
            }
        }

        /// <summary>
        /// Converte o conteúdo de um <see cref="FileInfo"/> em <see cref="byte[]"/>
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this FileInfo File)
        {
            if (File != null && File.Exists)
            {
                using (var fStream = new FileStream(File.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var br = new BinaryReader(fStream))
                    {
                        return br.ReadBytes((int)File.Length);
                    }
                }
            }
            return Array.Empty<byte>();
        }

        /// <summary>
        /// Retrun the string in camelCase form
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToCamelCase(this string Text) => Text.PascalCaseSplit().Select((x, i) => i == 0 ? x.ToLowerInvariant() : x.ToTitle(true)).SelectJoinString("");
        public static string ToPascalCase(this string Text) => Text.PascalCaseSplit().Select(x => x.ToTitle(true)).SelectJoinString("");

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
        /// , Pode ser um texto em branco (Transparent), um valor hexadecimal, um valor int ARGB,
        /// uma <see cref="NamedColors"/> (retorna aquela cor exata), uma palavra qualquer (gera
        /// proceduralmente uma cor) ou uma expressão de cor (Red+Blue, Red-Blue,Green*Red etc). A
        /// palavra 'random' ou 'rnd' gera uma cor aleatória
        /// </param>
        /// <returns></returns>
        public static Color ToColor(this string Text)
        {
            if (Text.IsNotValid()) return Color.Transparent;

            if (Text == "random" || Text == "rnd") return RandomColor();

            if (Text.IsNumber()) return Color.FromArgb(Text.ToInt());

            if (Text.IsHexaDecimalColor()) return ColorTranslator.FromHtml($"#{Text.RemoveFirstEqual("#")}");

            var maybecolor = FindColor(Text);
            if (maybecolor != null)
            {
                return maybecolor.ToDrawingColor();
            }

            if (Text.Contains("*"))
            {
                var various = Text.Split("*");

                if (various.Any())
                {
                    return various.Select(x => new HSVColor(x.Trim())).Aggregate((a, b) => a * b);
                }
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

        public static string ToCssRGB(this Color Color) => $"rgb({Color.R},{Color.G},{Color.B})";

        /// <summary>
        /// Converte uma cor de sistema para CSS RGB
        /// </summary>
        /// <param name="Color">Cor do sistema</param>
        /// <returns>String contendo a cor em RGB</returns>
        public static string ToCssRGBA(this Color Color) => $"rgba({Color.R},{Color.G},{Color.B},{Color.A})";

        /// <summary>
        /// Returns a CSV String from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="Separator"></param>
        /// <param name="IncludeHeader"></param>
        /// <returns></returns>
        public static string ToCSV(this IEnumerable<Dictionary<string, object>> Items, string Separator = ",", bool IncludeHeader = false)
        {
            Separator = Separator.IfBlank(",");
            var str = $"sep={Separator}{Environment.NewLine}";
            if (Items != null && Items.Any())
            {
                Items = Items.MergeKeys();

                if (IncludeHeader && Items.All(x => x.Keys.Any()))
                {
                    str += $"{Items.FirstOrDefault()?.Keys.SelectJoinString(Separator)}{Environment.NewLine}";
                }
                str += $"{Items.SelectJoinString(x => x.Values.SelectJoinString(Separator), Environment.NewLine)}";
            }

            return str;
        }

        public static string ToCSV<T>(this IEnumerable<T> Items, string Separator = ",", bool IncludeHeader = false) where T : class => (Items ?? Array.Empty<T>()).Select(x => x.CreateDictionary()).ToCSV(Separator, IncludeHeader);

        /// <summary>
        /// Converte um Array de Bytes em uma DATA URL Completa
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <param name="Type">Tipo de arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this byte[] Bytes, FileType Type = null) => "data:" + (Type ?? new FileType()).ToString() + ";base64," + Bytes.ToBase64();

        /// <summary>
        /// Converte um Array de Bytes em uma DATA URL Completa
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <param name="MimeType">Tipo de arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this byte[] Bytes, string MimeType) => "data:" + MimeType + ";base64," + Bytes.ToBase64();

        /// <summary>
        /// Converte um arquivo uma DATA URL Completa
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this FileInfo File) => File.ToBytes().ToDataURL(new FileType(File.Extension));

        /// <summary>
        /// Transforma uma imagem em uma URL Util
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>Uma DataURI em string</returns>
        public static string ToDataURL(this Image Image) => $"data:{Image.GetMimeTypes().First().ToLowerInvariant().Replace("application/octet-stream", GetFileType(".png").GetMimeTypesOrDefault().First())};base64,{Image.ToBase64()}";

        /// <summary>
        /// Converte uma imagem para DataURI trocando o MIME TEntity
        /// </summary>
        /// <param name="OriginalImage">Imagem</param>
        /// <param name="OriginalImageFormat">Formato da Imagem</param>
        /// <returns>Uma data URI com a imagem convertida</returns>
        public static string ToDataURL(this Image OriginalImage, ImageFormat OriginalImageFormat) => OriginalImage.ToBase64(OriginalImageFormat).Base64ToImage().ToDataURL();

        /// <summary>
        /// Converte um ToType para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static DateTime ToDateTime<T>(this T Value) => Value.ChangeType<DateTime>();

        /// <summary>
        /// Converte um ToType para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static DateTime ToDateTime<T>(this T Value, string CultureInfoName) => Value.ToDateTime(new CultureInfo(CultureInfoName));

        /// <summary>
        /// Converte um ToType para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static DateTime ToDateTime<T>(this T Value, CultureInfo CultureInfo) => Convert.ToDateTime(Value, CultureInfo);

        /// <summary>
        /// Converte um ToType para Decimal. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static decimal ToDecimal<T>(this T Value) => Value.ChangeType<decimal>();

        public static string ToDecimalString(this float number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this short number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this double number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this long number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this int number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this decimal number, int Decimals = -1, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            Decimals = Decimals < 0 ? GetDecimalLength(number) : Decimals;
            Decimals = Decimals < 0 ? culture.NumberFormat.NumberDecimalDigits : Decimals;
            return number.ToString("0".AppendIf(culture.NumberFormat.NumberDecimalSeparator + "0".Repeat(Decimals), Decimals > 0), culture);
        }

        /// <summary>
        /// Retorna um <see cref="Dictionary"/> a partir de um <see cref="IGrouping(Of TKey, TElement)"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="groupings"></param>
        /// <returns></returns>
        public static Dictionary<TKey, IEnumerable<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings) => groupings.ToDictionary(group => group.Key, group => group.AsEnumerable());

        /// <summary>
        /// Transforma uma lista de pares em um Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items, params TKey[] Keys) => items.Where(x => Keys == null || Keys.Any() == false || x.Key.IsIn(Keys)).DistinctBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        public static bool IsAnonymousType<T>(this T obj)

        {
            var type = obj.GetTypeOf();
            if (type == null)
                throw new ArgumentNullException("type");

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        /// <summary>
        /// Converte um NameValueCollection para um <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="[NameValueCollection]">Formulario</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this NameValueCollection NameValueCollection, params string[] Keys)
        {
            var result = new Dictionary<string, object>();
            Keys = Keys ?? Array.Empty<string>();
            if (Keys.Any() == false)
            {
                Keys = NameValueCollection.AllKeys;
            }

            foreach (string key in NameValueCollection.Keys)
            {
                if (key.IsValid() && key.IsLikeAny(Keys))
                {
                    var values = NameValueCollection.GetValues(key);
                    if (result.ContainsKey(key))
                    {
                        var l = new List<object>();
                        if (IsArray(result[key]))
                        {
                            foreach (var v in (IEnumerable)result[key])
                            {
                                if (v.IsNumber())
                                {
                                    l.Add(v.ToDouble());
                                }
                                else if (v.IsDate())
                                {
                                    l.Add(v.ToDateTime());
                                }
                                else
                                {
                                    l.Add(v);
                                }
                            }
                        }
                        else
                        {
                            var v = result[key];
                            if (v.IsNumber())
                            {
                                l.Add(v.ToDouble());
                            }
                            else if (v.IsDate())
                            {
                                l.Add(v.ToDateTime());
                            }
                            else
                            {
                                l.Add(v);
                            }
                        }

                        if (l.Count == 1)
                        {
                            result[key] = l[0];
                        }
                        else
                        {
                            result[key] = l.ToArray();
                        }
                    }
                    else if (values.Length == 1)
                    {
                        var v = values.FirstOrDefault();
                        if (v.IsNumber())
                        {
                            result.Add(key, v.ToDouble());
                        }
                        else if (v.IsDate())
                        {
                            result.Add(key, v.ToDateTime());
                        }
                        else
                        {
                            result.Add(key, v);
                        }
                    }
                    else
                    {
                        var ar = new List<object>();
                        foreach (var v in values)
                        {
                            if (v.IsNumber())
                            {
                                ar.Add(v.ToDouble());
                            }
                            else if (v.IsDate())
                            {
                                ar.Add(v.ToDateTime());
                            }
                            else
                            {
                                ar.Add(v);
                            }
                        }

                        result.Add(key, ar.ToArray());
                    }
                }
            }

            return result;
        }

        public static DirectoryInfo ToDirectoryInfo(this string PathPart) => ToDirectoryInfo(new[] { PathPart });

        public static DirectoryInfo ToDirectoryInfo(this string[] PathParts)
        {
            var x = ToFileSystemInfo(PathParts);

            return x is FileInfo info ? info.Directory : x as DirectoryInfo;
        }

        /// <summary>
        /// Converte um ToType para Double. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static double ToDouble<T>(this T Value) => Value.ChangeType<double>();

        public static FileInfo ToFileInfo(this string PathPart) => ToFileInfo(new[] { PathPart });

        public static FileInfo ToFileInfo(this string[] PathParts)
        {
            var x = ToFileSystemInfo(PathParts);
            if (x is DirectoryInfo) throw new Exception("File is directory");
            else if (x is FileInfo) return x as FileInfo;
            throw new Exception("File is not a valid file");
        }

        public static long GetSize(this FileSystemInfo info)
        {
            if (info is DirectoryInfo dir)
            {
                return dir.EnumerateFileSystemInfos().Sum(d => GetSize(d));
            }
            else if (info is FileInfo file)
            {
                return file.Length;
            }
            else if (info is FileTree ft)
            {
                if (ft.IsDirectory)
                    return (new DirectoryInfo(ft.Path)).GetSize();
                if (ft.IsFile)
                    return (new FileInfo(ft.Path)).GetSize();
            }
            return 0;
        }
        /// <summary>
        /// Retorna uma string contendo a descrição do tipo arquivo ou diretório
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static string GetDescription(this FileSystemInfo info) => info.FullName.IsDirectoryPath() ? "Directory" : GetFileType(info.FullName)?.Description ?? "FileOrDirectory";

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this byte[] Size, int DecimalPlaces = -1) => (Size?.LongLength ?? 0).ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="info">Diretorio ou Arquivo</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this FileSystemInfo info, int DecimalPlaces = -1) => info.GetSize().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this double Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this int Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this long Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this decimal Size, int DecimalPlaces = -1) => UnitConverter.CreateFileSizeConverter().Abreviate(Size, DecimalPlaces);

        public static FileSystemInfo ToFileSystemInfo(this string PathPart) => ToFileSystemInfo(new[] { PathPart });

        public static FileTree ToFileTree(this string PathPart) => ToFileTree(new[] { PathPart });
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PathParts"></param>
        /// <returns></returns>
        public static FileTree ToFileTree(this string[] PathParts) => new FileTree(PathParts.FixPath());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PathParts"></param>
        /// <param name="AlternativeChar"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static FileSystemInfo ToFileSystemInfo(this string[] PathParts, bool AlternativeChar = false)
        {
            var path = PathParts.FixPath();
            if (path.IsFilePath()) return new FileInfo(path);
            else if (path.IsDirectoryPath()) return new DirectoryInfo(path);
            else throw new ArgumentException("Can't create path from array", nameof(PathParts));
        }


        /// <summary>
        /// Computes the MD5 checksum of the specified file.
        /// </summary>
        /// <remarks>This method extends the <see cref="FileInfo"/> class to provide a convenient way to
        /// compute the MD5 checksum of a file.</remarks>
        /// <param name="file">The <see cref="FileInfo"/> object representing the file for which to compute the MD5 checksum. Must not be
        /// <see langword="null"/>.</param>
        /// <returns>A <see cref="string"/> containing the MD5 checksum of the file in hexadecimal format.  Returns an empty
        /// string if <paramref name="file"/> is <see langword="null"/>.</returns>
        public static string GetMD5Checksum(this FileInfo file) => file?.ToBytes().GetMD5Checksum() ?? string.Empty;



        /// <summary>
        /// Computes the SHA-256 checksum of the specified byte array.
        /// </summary>
        /// <remarks>This method uses the SHA-256 cryptographic hash function to compute a fixed-length checksum for the
        /// input data. The result is returned as a hexadecimal string, which can be used for data integrity verification or
        /// other cryptographic purposes.</remarks>
        /// <param name="data">The byte array for which to compute the SHA-256 checksum. Cannot be null.</param>
        /// <returns>A string representing the SHA-256 checksum of the input data, encoded as a hexadecimal string.</returns>
        public static string GetSHA256Checksum(this byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                return ToHexString(hash);
            }
        }

        /// <summary>
        /// Computes the SHA-256 checksum of the specified text using the provided encoding.
        /// </summary>
        /// <remarks>This method provides a convenient way to compute the SHA-256 checksum of a string. 
        /// If no encoding is specified, UTF-8 is used as the default encoding.</remarks>
        /// <param name="text">The input text for which the SHA-256 checksum will be calculated. Cannot be <see langword="null"/> or empty.</param>
        /// <param name="encoding">The character encoding to use when converting the text to bytes. If <see langword="null"/>, UTF-8 encoding
        /// is used by default.</param>
        /// <returns>A string representing the SHA-256 checksum of the input text, encoded as a hexadecimal string.</returns>
        public static string GetSHA256Checksum(this string text, Encoding encoding = null)
        {
            if (text.IsBlank()) return string.Empty;
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytes = encoding.GetBytes(text);
            return GetSHA256Checksum(bytes);
        }

        /// <summary>
        /// Computes the SHA-256 checksum of all files within the specified directory and its subdirectories.
        /// </summary>
        /// <remarks>The method processes all files in the directory and its subdirectories recursively. 
        /// Files are read in a sorted order based on their full path (case-insensitive) to ensure  consistent results
        /// regardless of file system ordering. The SHA-256 checksum is computed  by combining the hashes of all
        /// files.</remarks>
        /// <param name="dir">The directory for which the SHA-256 checksum is calculated. Must exist.</param>
        /// <returns>A hexadecimal string representing the SHA-256 checksum of the directory's contents.  The checksum is
        /// computed by processing all files in the directory and its subdirectories,  sorted by their full path in a
        /// case-insensitive manner.</returns>
        /// <exception cref="DirectoryNotFoundException">Thrown if the specified <paramref name="dir"/> does not exist.</exception>
        public static string GetSHA256Checksum(this FileSystemInfo info)
        {

            if (info is FileTree tree)
            {
                if (tree.IsDirectory)
                    return GetSHA256Checksum(new DirectoryInfo(tree.FullName));
                if (tree.IsFile)
                    return GetSHA256Checksum(new FileInfo(tree.FullName));
            }
            else if (info is FileInfo file)
            {
                if (!file.Exists)
                    throw new FileNotFoundException("Arquivo não encontrado.", file.FullName);

                using (var sha256 = SHA256.Create())
                {
                    using (var stream = file.OpenRead())
                    {
                        byte[] hash = sha256.ComputeHash(stream);
                        return ToHexString(hash);
                    }
                }
            }
            else if (info is DirectoryInfo dir)
            {
                if (!dir.Exists)
                    throw new DirectoryNotFoundException("Diretório não encontrado.");

                using (var sha256 = SHA256.Create())
                {
                    var allFiles = dir.GetFiles("*", SearchOption.AllDirectories);
                    Array.Sort(allFiles, (f1, f2) => string.Compare(f1.FullName, f2.FullName, StringComparison.OrdinalIgnoreCase));

                    foreach (var f in allFiles)
                    {
                        using (var stream = f.OpenRead())
                        {
                            byte[] fileHash = sha256.ComputeHash(stream);
                            sha256.TransformBlock(fileHash, 0, fileHash.Length, null, 0);
                        }
                    }

                    // Finaliza o hash depois de todos os blocos
                    sha256.TransformFinalBlock(new byte[0], 0, 0);
                    return ToHexString(sha256.Hash);
                }
            }

            throw new ArgumentException("O objeto FileSystemInfo deve ser um arquivo ou diretório válido.", nameof(info));

        }

        // 🔧 Conversão para string hexadecimal
        public static string ToHexString(this byte[] hash)
        {
            var sb = new StringBuilder();
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }


        public static string GetMD5Checksum(this byte[] inputData)
        {
            // Convert byte array to stream
            using (var stream = new System.IO.MemoryStream(inputData))
            {
                // Create an MD5 instance
                using (var md5Instance = System.Security.Cryptography.MD5.Create())
                {
                    // Compute the hash
                    var hashResult = md5Instance.ComputeHash(stream);

                    // Convert the hash to a string (removing dashes and converting to lowercase)
                    return BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        /// <summary>
        /// Retorna um Objeto FileType a partir de uma string MIME TEntity, Nome ou Extensão de Arquivo
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
        /// <returns></returns>
        public static FileType GetFileType(this string MimeTypeOrExtensionOrPathOrDataURI) => new FileType(MimeTypeOrExtensionOrPathOrDataURI);

        public static Uri ToFileUri(this FileSystemInfo File) => new Uri($@"file://{File?.FullName.Replace(" ", "%20")}");

        /// <summary>
        /// Converte um ToType para Single. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static float ToFloat<T>(this T Value) => Value.ChangeType<float>();

        public static FormattableString ToFormattableString(this string Text, params object[] args) => FormattableStringFactory.Create(Text, args ?? Array.Empty<object>());

        /// <summary>
        /// Prepara uma string para se tornar uma caminho amigavel (remove caracteres nao permitidos)
        /// </summary>
        /// <param name="Text"></param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyPathName(this string Text) => Text.RemoveAny(Path.GetInvalidPathChars()).TrimBetween();

        /// <summary>
        /// Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e
        /// troca espacos por hifen)
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="UseUnderscore">
        /// Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
        /// </param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyURL(this string Text, bool UseUnderscore = false) => Text.ReplaceMany(UseUnderscore ? "_" : "-", "_", "-", WhitespaceChar).RemoveAny("(", ")", ".", ",", "#", ":", ";").ToLowerInvariant().Replace("@", "A").Replace("&", "E").RemoveAccents().ToFriendlyPathName();

        /// <summary>
        /// Concatena todas as <see cref="Exception.InnerException"/> em uma única string
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToFullExceptionString(this Exception ex, string Separator = " => ") => ex.Traverse(x => x.InnerException).SelectJoinString(x => x.Message, Separator);

        /// <summary>
        /// Alterna uma variavel ente 2 valores diferentes
        /// </summary>
        /// <param name="Current">Objeto contendo o primeiro ou segundo valor</param>
        /// <param name="TrueValue">Primeiro valor</param>
        /// <param name="FalseValue">Segundo Valor</param>
        public static T Toggle<T>(this T Current, T TrueValue, T FalseValue = default) => Current.Equals(TrueValue) ? FalseValue : TrueValue;

        public static T ToggleVisibility<T>(this T FileOrDir) where T : FileSystemInfo => FileOrDir.IsVisible() ? FileOrDir.Hide() : FileOrDir.Show();

        /// <summary>
        /// Gera uma URL do google MAPs baseado na localização
        /// </summary>
        /// <param name="local">
        /// Uma variavel do tipo AddressInfo onde estão as informações como endereço e as
        /// coordenadas geográficas
        /// </param>
        /// <returns>Uma URI do Google Maps</returns>
        public static Uri ToGoogleMapsURL(this AddressInfo local, params AddressPart[] Parts) => local != null ? new Uri($"https://www.google.com.br/maps/search/{Uri.EscapeUriString(local.ToString(Parts))}") : null;

        public static string ToHexadecimal(this Color Color, bool Hash = true) => (Color.R.ToString("X2") + Color.G.ToString("X2") + Color.B.ToString("X2")).PrependIf("#", Hash);

        public static IEnumerable<HSVColor> ToHSVColorList(this IEnumerable<Color> ColorList) => ColorList?.Select(x => new HSVColor(x));

        public static HSVColor ToHSVColor(this Color color) => new HSVColor(color);
        public static HSVColor ToHSVColor(this string color) => new HSVColor(color);

        /// <summary>
        /// Converte um array de bytes para imagem
        /// </summary>
        /// <param name="Bytes">Bytes</param>
        /// <returns></returns>
        public static Image ToImage(this byte[] Bytes)
        {
            if (Bytes != null && Bytes.IsNotNullOrEmpty())
                using (var s = new MemoryStream(Bytes))
                {
                    return Image.FromStream(s);
                }
            return null;
        }

        /// <summary>
        /// Converte um ToType para Integer. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static int ToInt<FromType>(this FromType Value) => Value.ChangeType<int>();

        public static T[][] ToJaggedArray<T>(this T[,] inputArray)
        {
            if (inputArray == null || inputArray.Length == 0)
            {
                return Array.Empty<T[]>();
            }

            // GetCliente the number of rows and columns in the input array
            int rows = inputArray.GetLength(0);
            int cols = inputArray.GetLength(1);

            // Create the jagged array with the same number of rows as the input array
            T[][] jaggedArray = new T[rows][];

            // Copy the elements from the input array to the jagged array
            for (int i = 0; i < rows; i++)
            {
                // Create a new sub-array for each row
                jaggedArray[i] = new T[cols];

                // Copy the elements from the input array to the jagged array
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = inputArray[i, j];
                }
            }

            return jaggedArray;
        }

        /// <summary>
        /// Converte um texto para Leet (1337)
        /// </summary>
        /// <param name="text">TExto original</param>
        /// <param name="degree">Grau de itensidade (0 a 7)</param>
        /// <returns>Texto em 1337</returns>
        public static string ToLeet(this string Text, int Degree = 7)
        {
            // Adjust degree between 0 - 100
            Degree = Degree.LimitRange(0, 7);
            // No Leet Translator
            if (Degree == 0)
            {
                return Text;
            }
            // StringBuilder to store result.
            var sb = new StringBuilder();
            foreach (char c in Text.AsEnumerable())
            {
                switch (Degree)
                {
                    case 1:
                        switch (c)
                        {
                            case 'e':
                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 2:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 3:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 4:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 5:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('6');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 6:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('6');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'r':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'R':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'b':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'B':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'q':
                                {
                                    sb.Append('Q');
                                    break;
                                }

                            case 'Q':
                                {
                                    sb.Append("Q¸");
                                    break;
                                }

                            case 'x':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            case 'X':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    default:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('6');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'r':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'R':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'b':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'B':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'j':
                                {
                                    sb.Append("_|");
                                    break;
                                }

                            case 'J':
                                {
                                    sb.Append("_|");
                                    break;
                                }

                            case 'P':
                                {
                                    sb.Append("|°");
                                    break;
                                }

                            case 'q':
                                {
                                    sb.Append('¶');
                                    break;
                                }

                            case 'Q':
                                {
                                    sb.Append("¶¸");
                                    break;
                                }

                            case 'x':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            case 'X':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converte um ToType para Integer. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static long ToLong<T>(this T Value) => Value.ChangeType<long>();

        public static string ToMD5String(this string Text)
        {
            if (Text.IsValid())
            {
                var md5 = MD5.Create();
                var inputBytes = Encoding.ASCII.GetBytes(Text);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (int i = 0, loopTo = hash.Length - 1; i <= loopTo; i++)
                {
                    sb.Append(hash[i].ToString("X2", CultureInfo.InvariantCulture));
                }

                return sb.ToString();
            }

            return Text;
        }

        /// <summary>
        /// Pega um texto em "PascalCase" ou "snake_case" e o retorna na forma "normal case"
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToNormalCase(this string Text) => Text.PascalCaseAdjust().Replace("_", WhitespaceChar).TrimBetween();

        /// <summary>
        /// retorna o numero em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this int Number) => Number.ToLong().ToOrdinalNumber();

        /// <summary>
        /// retorna o numero em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this long Number) => $"{Number}{Number.GetOrdinal()}";

        /// <summary>
        /// retorna o numero em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this short Number) => Number.ToInt().ToOrdinalNumber();

        /// <summary>
        /// retorna o numero em sua forma ordinal (inglês)
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
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this decimal Number, int Decimals = -1, bool MultiplyBy100 = false)
        {
            if (MultiplyBy100) Number *= 100;
            if (Decimals > -1)
            {
                Number = Number.RoundDecimal(Decimals);
            }
            return $"{Number}%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this int Number) => $"{Number}%";

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this double Number, int Decimals = -1, bool MultiplyBy100 = false) => Number.ToDecimal().ToPercentString(Decimals, MultiplyBy100);

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this short Number) => $"{Number}%";

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this long Number) => $"{Number}%";

        /// <summary>
        /// Concatena todos os itens de uma lista, utilizando a palavra <paramref name="And"/> antes
        /// da ultima ocorrencia.
        /// </summary>
        /// <param name="Texts">
        /// Lista com itens que serão convertidos em <see cref="string"/> e concatenados
        /// </param>
        /// <param name="And">
        /// palavra correspondente ao "e", utilizada para concatena ro ultimo elemento da lista.
        /// Quando null ou branco, <paramref name="Separator"/> é utilizado em seu lugar.
        /// </param>
        /// <param name="Separator">caractere correspondente a virgula</param>
        /// <param name="EmptyValue">
        /// Valor que será apresentado caso <paramref name="Texts"/> esteja vazio ou nulo. Quando
        /// <see cref="null"/>, omite o <paramref name="PhraseStart"/> da string final
        /// </param>
        /// <returns></returns>
        public static string ToPhrase<T>(this IEnumerable<T> Texts, string PhraseStart = EmptyString, string And = "and", string EmptyValue = null, char Separator = ',')
        {
            Separator = Separator.IfBlank(',');
            PhraseStart = PhraseStart.IfBlank(EmptyString);

            Texts = (Texts ?? Array.Empty<T>()).WhereNotBlank();

            if (PhraseStart.IsValid() && !PhraseStart.EndsWithAny(StringComparison.InvariantCultureIgnoreCase, PredefinedArrays.BreakLineChars.ToArray()) && !PhraseStart.EndsWith(WhitespaceChar, StringComparison.InvariantCultureIgnoreCase))
            {
                PhraseStart += WhitespaceChar;
            }

            switch (Texts.Count())
            {
                case 0:
                    if (EmptyValue != null)
                    {
                        PhraseStart += EmptyValue;
                    }

                    break;

                case 1:
                    PhraseStart += $"{Texts.FirstOrDefault()}";
                    break;

                default:
                    PhraseStart += Texts.SkipLast().SelectJoinString($"{Separator} ");
                    PhraseStart += $" {And.IfBlank($"{Separator}")}";
                    PhraseStart += $" {Texts.Last()}";
                    break;
            }

            return PhraseStart;
        }

        ///<inheritdoc cref="ToPhrase{TSource}(IEnumerable{TSource}, string, string, string, char)"/>
        public static string ToPhrase(string And, params string[] Texts) => (Texts ?? Array.Empty<string>()).ToPhrase(EmptyString, And);



        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata os valores
        /// específicos de um <see cref="Dictionary{TKey, TValue}"/> como parametros da procedure
        /// </summary>
        /// <param name="Dic">Objeto</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">propriedades do objeto que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static string ToProcedure(this string ProcedureName, Dictionary<string, object> Dic, params string[] Keys)
        {
            Dic = Dic ?? new Dictionary<string, object>();
            Keys = Keys ?? Array.Empty<string>();
            if (!Keys.Any())
            {
                Keys = Dic.Keys.ToArray();
            }
            else
            {
                Keys = Dic.Keys.ToArray().Where(x => x.IsLikeAny(Keys)).ToArray();
            }

            return $"{ProcedureName} {Keys.SelectJoinString(key => $" @{key} = @__{key}", ", ")}";
        }

        /// <summary>
        /// Coloca o texto em TitleCase
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToProperCase(this string Text, bool? ForceCase = null)
        {
            if (Text.IsNotValid())
            {
                return Text;
            }

            if (ForceCase == null)
            {
                ForceCase = Text.Where(char.IsLetter).All(char.IsUpper);
            }


            if (ForceCase == true)
            {
                Text = Text.ToLowerInvariant();
            }

            var l = Text.Split(WhitespaceChar, StringSplitOptions.None).ToList();
            for (int index = 0, loopTo = l.Count - 1; index <= loopTo; index++)
            {
                string pal = l[index];
                bool artigo = index > 0 && IsIn(pal, "o", "a", "os", "as", "um", "uma", "uns", "umas", "de", "do", "dos", "das", "e", "ou", "of");
                if (pal.IsValid())
                {
                    if (ForceCase == true || artigo == false)
                    {
                        char c = pal.First();
                        if (!char.IsUpper(c))
                        {
                            pal = char.ToUpper(c, CultureInfo.InvariantCulture) + pal.RemoveFirstChars(1);
                        }

                        l[index] = pal;
                    }
                }
            }

            return l.SelectJoinString(WhitespaceChar);
        }

        public static QuantityTextPair ToQuantityTextPair(this string Text) => (QuantityTextPair)Text;

        public static QuantityTextPair ToQuantityTextPair(this IEnumerable<string> Text) => (QuantityTextPair)(Text?.ToArray());

        /// <summary>
        /// Retorna um dicionário em QueryString
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public static string ToQueryString(this Dictionary<string, string> Dic) => Dic?.Where(x => x.Key.IsValid()).SelectJoinString(x => new[] { x.Key, (x.Value ?? EmptyString).UrlEncode() }.SelectJoinString("="), "&") ?? EmptyString;

        /// <summary>
        /// Retorna um <see cref="NameValueCollection"/> em QueryString
        /// </summary>
        /// <param name="NVC"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection NVC) => NVC?.AllKeys.SelectManyJoinString(n => NVC.GetValues(n).Select(v => n + "=" + v).Where(x => x.IsValid() && x != "="), "&");

        /// <summary>
        /// COnverte graus para radianos
        /// </summary>
        /// <param name="Degrees"></param>
        /// <returns></returns>
        public static double ToRadians(this double Degrees) => Degrees * Math.PI / 180.0d;

        /// <summary>
        /// Coloca a string em Randomcase (aleatoriamente letras maiusculas ou minusculas)
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Times">Numero de vezes que serão sorteados caracteres.</param>
        /// <returns></returns>
        public static string ToRandomCase(this string Text, int Times = 0)
        {
            var ch = Text.ToArray();
            Times = Times.SetMinValue(ch.Length);
            for (int index = 1, loopTo = Times; index <= loopTo; index++)
            {
                int newindex = RandomInt(0, ch.Length - 1);
                if (char.IsUpper(ch[newindex]))
                {
                    ch[newindex] = char.ToLower(ch[newindex], CultureInfo.InvariantCulture);
                }
                else
                {
                    ch[newindex] = char.ToUpper(ch[newindex], CultureInfo.InvariantCulture);
                }
            }

            return new string(ch);
        }

        public static string ToRoman(this int ArabicNumber)
        {
            ArabicNumber = ArabicNumber.ForcePositive();
            // valida : aceita somente valores entre 1 e 3999
            if (ArabicNumber < 1 || ArabicNumber > 3999)
            {
                throw new ArgumentException("The numeric value must be between 1 and 3999.", nameof(ArabicNumber));
            }

            var algarismosArabicos = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            var algarismosRomanos = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "TV", "IV", "I" };

            // inicializa o string builder
            string resultado = EmptyString;

            // percorre os valores nos arrays
            for (int i = 0; i <= 12; i++)
            {
                // se o numero a ser convertido é menor que o valor então anexa o numero
                // correspondente ou o par ao resultado
                while (ArabicNumber >= algarismosArabicos[i])
                {
                    ArabicNumber -= algarismosArabicos[i];
                    resultado += algarismosRomanos[i];
                }
            }

            // retorna o resultado
            return resultado.ToString();
        }

        public static string ToSentenceCase(this string Text)
        {
            Text = Text.Trim().GetFirstChars(1).ToUpperInvariant() + Text.RemoveFirstChars(1);
            var dots = new[] { "...", ". ", "? ", "! " };
            List<string> sentences;
            foreach (var dot in dots)
            {
                sentences = Text.Split(dot, StringSplitOptions.None).ToList();
                for (int index = 0, loopTo = sentences.Count - 1; index <= loopTo; index++)
                {
                    sentences[index] = $"{sentences[index].Trim().GetFirstChars(1)?.ToUpperInvariant()}{sentences[index]?.RemoveFirstChars(1)}";
                }

                Text = sentences.SelectJoinString(dot);
            }

            sentences = Text.Split(WhitespaceChar).ToList();
            Text = EmptyString;
            foreach (var c in sentences)
            {
                string palavra = c;
                if (palavra.EndsWith(".") && palavra.Length == 2)
                {
                    palavra = palavra.ToUpperInvariant();
                    Text += palavra;
                    string proximapalavra = sentences.IfNoIndex(sentences.IndexOf(c) + 1, EmptyString);
                    if (!(proximapalavra.EndsWith(".") && palavra.Length == 2))
                    {
                        Text += WhitespaceChar;
                    }
                }
                else
                {
                    Text += c + WhitespaceChar;
                }
            }

            return Text.RemoveLastChars(1);
        }

        /// <summary>
        /// Generates a GUID based on the current date/time http://stackoverflow.com/questions/1752004/sequential-guid-generator-c-sharp
        /// </summary>
        /// <returns></returns>
        public static Guid SequentialGuid()
        {
            var tempGuid = Guid.NewGuid();
            var bytes = tempGuid.ToByteArray();
            var time = DateTime.Now;
            bytes[3] = (byte)time.Year;
            bytes[2] = (byte)time.Month;
            bytes[1] = (byte)time.Day;
            bytes[0] = (byte)time.Hour;
            bytes[5] = (byte)time.Minute;
            bytes[4] = (byte)time.Second;
            return new Guid(bytes);
        }

        /// <summary>
        /// Converte um ToType para short. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static short ToShort<T>(this T Value) => Value.ChangeType<short>();

        /// <summary>
        /// Converts the specified text to slug case.
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToSlugCase(this string Text) => Text?.Replace(WhitespaceChar, "-").ToLowerInvariant();

        /// <summary>
        /// Converts the specified text to kebab case.
        /// </summary>
        /// <param name="Text">The text to convert.</param>
        /// <returns>The text converted to kebab case.</returns>
        public static string ToKebabCase(this string Text) => Text.ToSlugCase();

        /// <summary>
        /// Retorna uma string em Snake_Case
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string Text) => Text?.Replace(WhitespaceChar, "_").ToLowerInvariant();

        /// <summary>
        /// Cria um <see cref="Stream"/> a partir de uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static Stream ToStream(this string Text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(Text);
            writer.Flush();
            stream.Position = 0L;
            return stream;
        }

        /// <summary>
        /// Transforma uma imagem em um stream
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns></returns>
        public static Stream ToStream(this Image Image, ImageFormat Format = null)
        {
            Format = Format ?? ImageFormat.Png;
            Stream s = new MemoryStream();
            Image.Save(s, Format);
            s.Position = 0L;
            return s;
        }

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays
        /// </summary>
        /// <typeparam name="TGroupKey"></typeparam>
        /// <typeparam name="TSubGroupKey"></typeparam>
        /// <typeparam name="TSubGroupValue"></typeparam>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public static IEnumerable<object> ToTableArray<TGroupKey, TSubGroupKey, TSubGroupValue, THeaderProperty>(this Dictionary<TGroupKey, Dictionary<TSubGroupKey, TSubGroupValue>> Groups, Func<TSubGroupKey, THeaderProperty> HeaderProp)
        {
            var lista = new List<object>();
            var header = new List<object>
            {
                HeaderProp?.Method.GetParameters().First().Name
            };

            Groups?.Values.MergeKeys();
            foreach (var h in Groups.SelectMany(x => x.Value.Keys.ToArray()).Distinct().OrderBy(x => x))
            {
                header.Add(HeaderProp(h));
            }

            lista.Add(header);
            lista.AddRange(Groups.Select(x =>
            {
                var l = new List<object>
                {
                    x.Key // GroupKey
                };
                foreach (var item in x.Value.OrderBy(k => k.Key).Select(v => v.Value))
                {
                    l.Add(item); // SubGroupValue
                }

                return l;
            }));
            return lista;
        }

        /// <summary>
        /// Converts an integer value to its equivalent Radix string representation.
        /// </summary>
        /// <param name="value">The integer value to convert.</param>
        /// <returns>The Radix string representation of the specified integer value.</returns>
        public static string ToRadix(this int value)
        {
            const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (value == 0) return "0";
            int radix = digits.Length;
            string result = "";
            while (value > 0)
            {
                int remainder = value % radix;
                value /= radix;
                result = digits[remainder] + result;
            }
            return result;
        }

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de
        /// arrays formando uma tabela
        /// </summary>
        public static IEnumerable<object[]> ToTableArray<TGroupKey, TGroupValue>(this Dictionary<TGroupKey, TGroupValue> Groups) => Groups.Select(x => new List<object> { x.Key, x.Value }.ToArray());

        /// <summary>
        /// Transforma um texto em titulo
        /// </summary>
        /// <param name="Text">Texto a ser manipulado</param>
        /// <param name="ForceCase">
        /// Se FALSE, apenas altera o primeiro caractere de cada palavra como UPPERCASE, dexando os
        /// demais intactos. Se TRUE, força o primeiro caractere de casa palavra como UPPERCASE e os
        /// demais como LOWERCASE
        /// </param>
        /// <returns>Uma String com o texto em nome próprio</returns>
        public static string ToTitle(this string Text, bool? ForceCase = null) => Text?.ToProperCase(ForceCase);

        /// <summary>
        /// Transforma um XML Document em string
        /// </summary>
        /// <param name="XML">Documento XML</param>
        /// <returns></returns>
        public static string ToXMLString(this XmlDocument XML)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                XML.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        public static Image Translate(this Image img, Color Color, float Alpha = 0f) => img.Translate(Color.R, Color.G, Color.B, Alpha);

        public static Image Translate(this Image img, float Red, float Green, float Blue, float Alpha = 0f)
        {
            float sr, sg, sb, sa;
            var copia = new Bitmap(img);
            // normalize the color components to 1
            sr = Red / 255f;
            sg = Green / 255f;
            sb = Blue / 255f;
            sa = Alpha / 255f;

            // create the color matrix
            var cm = new ColorMatrix(new float[][] { new float[] { 1f, 0f, 0f, 0f, 0f }, new float[] { 0f, 1f, 0f, 0f, 0f }, new float[] { 0f, 0f, 1f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { sr, sg, sb, sa, 1f } });

            // apply the matrix to the image
            return ApplyColorMatrix(copia, cm);
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Items">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> ChildSelector)
        {
            var stack = new Stack<T>(items.Reverse());
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in ChildSelector(next))
                {
                    stack.Push(child);
                }
            }
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ParentSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this T item, Expression<Func<T, T>> ParentSelector, Expression<Func<T, bool>> Filter = null)
        {
            if (item != null && ParentSelector != null)
            {
                var current = item;
                do
                {
                    if (Filter == null || Filter.Compile().Invoke(current))
                        yield return current;
                    current = ParentSelector.Compile().Invoke(current);
                }
                while (current != null);
            }
        }

        public static T TraverseUp<T>(this T node, Func<T, T> parentPredicate) where T : class
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            T current = node;
            while (current != null)
            {
                if (parentPredicate(current) == null)
                    return current;

                current = parentPredicate(current);
            }

            return current; // No matching parent found
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this T Item, Func<T, IEnumerable<T>> ChildSelector, bool IncludeMe = false)
        => ChildSelector(Item).Union(IncludeMe ? (new[] { Item }) : Array.Empty<T>()).Traverse(ChildSelector);

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, P> PropertySelector, bool IncludeMe = false) => Item.Traverse(ChildSelector, IncludeMe).Select(PropertySelector);

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, IEnumerable<P>> PropertySelector, bool IncludeMe = false) => Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector);

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, IQueryable<P>> PropertySelector, bool IncludeMe = false) => Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector);

        public static IEnumerable<T> TraverseAll<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> ChildSelector, Expression<Func<T, bool>> Filter = null) => items.SelectMany(x => Traverse(x, ChildSelector, true)).Where(Filter?.Compile() ?? (x => true));

        /// <summary>
        /// Remove os excessos de uma cor de fundo de uma imagem deixando apenas seu conteudo
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static Image Trim(this Image Img, Color? Color = default)
        {
            var bitmap = new Bitmap(Img);
            Color = Color ?? bitmap.GetPixel(0, 0);
            int w = bitmap.Width;
            int h = bitmap.Height;
            bool IsAllWhiteRow(int row)
            {
                for (int i = 0, loopTo = w - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(i, row).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            }
            bool IsAllWhiteColumn(int col)
            {
                for (int i = 0, loopTo = h - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(col, i).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            }
            int leftMost = 0;
            for (int col = 0, loopTo = w - 1; col <= loopTo; col++)
            {
                if (IsAllWhiteColumn(col))
                {
                    leftMost = col + 1;
                }
                else
                {
                    break;
                }
            }

            int rightMost = w - 1;
            for (int col = rightMost; col >= 1; col -= 1)
            {
                if (IsAllWhiteColumn(col))
                {
                    rightMost = col - 1;
                }
                else
                {
                    break;
                }
            }

            int topMost = 0;
            for (int row = 0, loopTo1 = h - 1; row <= loopTo1; row++)
            {
                if (IsAllWhiteRow(row))
                {
                    topMost = row + 1;
                }
                else
                {
                    break;
                }
            }

            int bottomMost = h - 1;
            for (int row = bottomMost; row >= 1; row -= 1)
            {
                if (IsAllWhiteRow(row))
                {
                    bottomMost = row - 1;
                }
                else
                {
                    break;
                }
            }

            if (rightMost == 0 && bottomMost == 0 && leftMost == w && topMost == h)
            {
                return bitmap;
            }

            int croppedWidth = rightMost - leftMost + 1;
            int croppedHeight = bottomMost - topMost + 1;
            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(bitmap, new RectangleF(0f, 0f, croppedWidth, croppedHeight), new RectangleF(leftMost, topMost, croppedWidth, croppedHeight), GraphicsUnit.Pixel);
                }

                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Values are top={0} bottom={1} left={2} right={3}", topMost, bottomMost, leftMost, rightMost), ex);
            }
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="StringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, bool ContinuouslyRemove, params string[] StringTest)
        {
            if (Text.IsValid())
            {
                Text = Text.TrimStartAny(ContinuouslyRemove, StringTest);
                Text = Text.TrimEndAny(ContinuouslyRemove, StringTest);
            }

            return Text;
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, params string[] StringTest) => Text.TrimAny(true, StringTest);

        public static IEnumerable<string> TrimBetween(this IEnumerable<string> Texts) => Texts.Select(x => x.TrimBetween());

        public static string TrimBetween(this string input)
        {
            if (input.IsBlank())
            {
                return input;
            }

            var lines = input.Split(new[] { "\r\n", "\r", "\n", Environment.NewLine }, StringSplitOptions.None)
                             .Where(line => line.IsNotBlank());

            var result = lines.Select(value =>
            {
                // Remove spaces before any of these chars
                value = Regex.Replace(value, @"\s+([\%:,.;?!\)\]})])", match => match.Groups[1].Value);

                // Remove spaces after any of these chars
                value = Regex.Replace(value, @"([\(\[\{])\s+", match => match.Groups[1].Value);

                // Remove extra spaces between words
                value = Regex.Replace(value, @"\s+", " ");

                return value.Trim();
            });

            return string.Join(Environment.NewLine, result);
        }

        /// <summary>
        /// Remove continuamente caracteres em branco do começo e fim de uma string incluindo breaklines
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string TrimCarriage(this string Text) => Text.TrimAny(PredefinedArrays.InvisibleChars.ToArray());

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, bool ContinuouslyRemove, StringComparison comparison, params string[] EndStringTest)
        {
            Text = Text ?? "";
            EndStringTest = EndStringTest ?? Array.Empty<string>();
            while (Text.EndsWithAny(comparison, EndStringTest))
            {
                foreach (var item in EndStringTest)
                {
                    if (Text.EndsWith(item, comparison))
                    {
                        Text = Text.RemoveLastEqual(item, comparison);
                        if (!ContinuouslyRemove)
                        {
                            return Text;
                        }
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, params string[] EndStringTest) => Text.TrimEndAny(true, default, EndStringTest);

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <param name="ContinuouslyRemove">Remove continuamente as strings</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, bool ContinuouslyRemove, params string[] EndStringTest) => Text.TrimEndAny(ContinuouslyRemove, default, EndStringTest);

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, StringComparison comparison, params string[] EndStringTest) => Text.TrimEndAny(true, comparison, EndStringTest);

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, bool ContinuouslyRemove, StringComparison comparison, params string[] StartStringTest)
        {
            Text = Text ?? "";
            StartStringTest = StartStringTest ?? Array.Empty<string>();
            while (Text.StartsWithAny(comparison, StartStringTest))
            {
                foreach (var item in StartStringTest)
                {
                    if (Text.StartsWith(item, comparison))
                    {
                        Text = Text.RemoveFirstEqual(item, comparison);
                        if (!ContinuouslyRemove)
                        {
                            return Text;
                        }
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, StringComparison comparison, params string[] StartStringTest) => Text.TrimStartAny(true, comparison, StartStringTest);

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, params string[] StartStringTest) => Text.TrimStartAny(true, default, StartStringTest);

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, bool ContinuouslyRemove, params string[] StartStringTest) => Text.TrimStartAny(ContinuouslyRemove, default, StartStringTest);

        /// <summary>
        /// Run a <see cref="Action"/> inside a Try-catch block and return a <see cref="Exception"/>
        /// if fail
        /// </summary>
        /// <param name="action"></param>
        /// <returns>
        /// A null <see cref="Exception"/> if <paramref name="action"/> runs successfully, otherwise
        /// the captured <see cref="Exception"/>
        /// </returns>
        public static Exception TryExecute(Action action)
        {
            try
            {
                action.Invoke();
                return null;
            }
            catch (Exception exx)
            {
                return exx;
            }
        }

        public static string UnBrackfy(this string Text) => Text.UnBrackfy('{', true);

        public static string UnBrackfy(this string Text, char OpenBracketChar, bool ContinuouslyRemove = false) => Text.UnQuote(OpenBracketChar, ContinuouslyRemove);

        public static string UnQuote(this string Text) => Text.UnQuote(char.MinValue, true);

        public static string UnQuote(this string Text, char OpenQuoteChar, bool ContinuouslyRemove = false)
        {
            if ($"{OpenQuoteChar}".RemoveNonPrintable().IsNotValid())
            {
                while (Text.EndsWithAny(PredefinedArrays.CloseWrappers.ToArray()) || Text.StartsWithAny(PredefinedArrays.OpenWrappers.ToArray()))
                {
                    Text = Text.TrimAny(ContinuouslyRemove, PredefinedArrays.WordWrappers.ToArray());
                }
            }
            else
            {
                if (OpenQuoteChar.ToString().IsCloseWrapChar())
                {
                    OpenQuoteChar = OpenQuoteChar.ToString().GetOppositeWrapChar().FirstOrDefault();
                }

                Text = Text.TrimAny(ContinuouslyRemove, $"{OpenQuoteChar}", OpenQuoteChar.ToString().GetOppositeWrapChar());
            }

            return Text;
        }

        public static string UnWrap(this string Text, string WrapText = DoubleQuoteChar, bool ContinuouslyRemove = false) => Text.TrimAny(ContinuouslyRemove, WrapText);

        /// <summary>
        /// Decoda uma string de uma transmissão por URL
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string UrlDecode(this string Text) => Text.IsNotBlank() ? WebUtility.UrlDecode(Text) : EmptyString;

        /// <summary>
        /// Encoda uma string para transmissão por URL
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string UrlEncode(this string Text) => Text.IsNotBlank() ? WebUtility.UrlEncode(Text) : EmptyString;

        /// <summary>
        /// Returns true if all logical operations return true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool Validate<T>(this T Value, params Expression<Func<T, bool>>[] Tests) => Validate(Value, 0, Tests);

        /// <summary>
        /// Returns true if a certain minimum number of logical operations return true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool Validate<T>(this T Value, int MinPoints, params Expression<Func<T, bool>>[] Tests)
        {
            Tests = Tests ?? Array.Empty<Expression<Func<T, bool>>>();
            if (MinPoints < 1)
            {
                MinPoints = Tests.Length;
            }
            return ValidateCount(Value, Tests) >= MinPoints.LimitRange(1, Tests.Length);
        }

        /// <summary>
        /// Returns the count of true logical operations on a given value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static int ValidateCount<T>(this T Value, params Expression<Func<T, bool>>[] Tests)
        {
            var count = 0;
            Tests = Tests ?? Array.Empty<Expression<Func<T, bool>>>();
            foreach (var item in Tests)
            {
                if (item.Compile().Invoke(Value))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Lança uma <see cref="Exception"/> do tipo <typeparamref name="TE"/> se um teste falhar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Test"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static T ValidateOr<T, TE>(this T Value, Expression<Func<T, bool>> Test, TE Exception) where TE : Exception
        {
            if (Test != null)
            {
                if (Test.Compile().Invoke(Value) == false)
                {
                    Value = default;
                    if (Exception != null)
                    {
                        throw Exception;
                    }
                }
            }
            return Value;
        }

        public static T ValidateOr<T>(this T Value, Expression<Func<T, bool>> Test) => ValidateOr(Value, Test, default);

        public static T ValidateOr<T>(this T Value, Expression<Func<T, bool>> Test, T defaultValue)
        {
            try
            {
                return ValidateOr(Value, Test, new Exception("Validation fail"));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool ValidatePassword(this string Password, PasswordLevel PasswordLevel = PasswordLevel.Strong) => PasswordLevel == PasswordLevel.None || Password.CheckPassword().ToInt() >= PasswordLevel.ToInt();

        /// <summary>
        /// Bloqueia a Thread atual enquanto um arquivo estiver em uso
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <param name="millisecondsTimeout">intervalo, em segundo entre as tentativas de acesso</param>
        /// <param name="MaxFailCount">
        /// Numero maximo de tentativas falhas,quando nulo, verifica infinitamente
        /// </param>
        /// <param name="OnAttemptFail">ação a ser executado em caso de falha</param>
        /// <returns>TRUE se o arquivo puder ser utilizado</returns>
        public static bool WaitForFile(this FileInfo File, int millisecondsTimeout = 1000, int? MaxFailCount = null, Action<int> OnAttemptFail = null)
        {
            if (File == null)
            {
                return false;
            }
            if (File.Exists == false)
            {
                return true;
            }

            while (IsInUse(File))
            {
                Thread.Sleep(millisecondsTimeout);

                if (File.Exists == false)
                {
                    return true;
                }

                if (MaxFailCount.HasValue)
                {
                    MaxFailCount = MaxFailCount.Value - 1;
                }

                if (MaxFailCount.HasValue && MaxFailCount.Value <= 0)
                {
                    return false;
                }

                OnAttemptFail?.Invoke(MaxFailCount ?? -1);
            }
            return true;
        }

        /// <summary>
        /// Insere uma imagem de marca d'água na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="WaterMarkImage">Imagem de Marca d'água</param>
        /// <param name="X">Posição X</param>
        /// <param name="Y">Posição Y</param>
        /// <returns></returns>
        public static Image Watermark(this Image Image, Image WaterMarkImage, int X = -1, int Y = -1, Color? transparentColor = null)
        {
            // a imagem onde iremos aplicar a marca d'água
            var bm_Resultado = new Bitmap(Image);

            // a imagem que será usada como marca d'agua
            var bm_marcaDagua = new Bitmap(WaterMarkImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5));
            if (X < 0)
                X = (bm_Resultado.Width - bm_marcaDagua.Width) / 2;   // centraliza a marca d'agua
            if (Y < 0)
                Y = (bm_Resultado.Height - bm_marcaDagua.Height) / 2;   // centraliza a marca d'agua
            const byte ALPHA = 128;
            // Define o componente Alpha do pixel
            Color clr;
            for (int py = 0, loopTo = bm_marcaDagua.Height - 1; py <= loopTo; py++)
            {
                for (int px = 0, loopTo1 = bm_marcaDagua.Width - 1; px <= loopTo1; px++)
                {
                    clr = bm_marcaDagua.GetPixel(px, py);
                    bm_marcaDagua.SetPixel(px, py, Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
                }
            }
            // Define a marca d'agua como transparente
            bm_marcaDagua.MakeTransparent(transparentColor ?? bm_marcaDagua.GetPixel(0, 0));
            // Copia o resultado na imagem
            var gr = Graphics.FromImage(bm_Resultado);
            gr.DrawImage(bm_marcaDagua, X, Y);
            return bm_Resultado;
        }

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em uma busca usando predicate
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="predicate">Funcao LINQ utilizada para a busca</param>
        /// <param name="SearchOption">
        /// Indica se apenas o diretorio atual ou todos os subdiretorios devem ser percorridos pela busca
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> Where<T>(this DirectoryInfo Directory, Func<T, bool> predicate, SearchOption SearchOption = SearchOption.AllDirectories) where T : FileSystemInfo
        {
            if (Directory != null && Directory.Exists && predicate != null)

                if (typeof(T) == typeof(FileInfo))
                    return Directory.GetFiles("*", SearchOption).Where((Func<FileInfo, bool>)predicate) as IEnumerable<T>;
                else if (typeof(T) == typeof(DirectoryInfo))
                    return Directory.GetDirectories("*", SearchOption).Where((Func<DirectoryInfo, bool>)predicate) as IEnumerable<T>;
                else
                    return Directory.GetFileSystemInfos("*", SearchOption).Where((Func<FileSystemInfo, bool>)predicate) as IEnumerable<T>;

            return Array.Empty<T>();
        }

        public static Expression PropertyExpression(this ParameterExpression Parameter, string PropertyName)
        {
            Expression prop = Parameter;
            if (PropertyName.IfBlank("this") != "this")
            {
                foreach (var name in PropertyName.SplitAny(".", "/"))
                {
                    prop = Expression.Property(prop, name);
                }
            }

            return prop;
        }

        /// <summary>
        /// Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto acessado</typeparam>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="Type"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="Type"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="Type"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static Expression<Func<Type, bool>> WhereExpression<Type>(string PropertyName, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, FilterConditional Conditional = FilterConditional.Or)
        {
            var parameter = GenerateParameterExpression<Type>();
            var member = parameter.PropertyExpression(PropertyName);
            Expression body = GetOperatorExpression(member, Operator, PropertyValue, Conditional);
            body = Expression.Equal(body, Expression.Constant(Is));
            var finalExpression = Expression.Lambda<Func<Type, bool>>(body, parameter);
            return finalExpression;
        }

        public static Expression<Func<Type, bool>> WhereExpression<Type, V>(Expression<Func<Type, V>> PropertySelector, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, FilterConditional Conditional = FilterConditional.Or)
        {
            var parameter = GenerateParameterExpression<Type>();
            string member = String.Join(".", PropertySelector.Body.ToString().Split(".").Skip(1));
            var prop = parameter.PropertyExpression(member);
            Expression body = GetOperatorExpression(prop, Operator, PropertyValue, Conditional);
            body = Expression.Equal(body, Expression.Constant(Is));
            var finalExpression = Expression.Lambda<Func<Type, bool>>(body, parameter);
            return finalExpression;
        }

        /// <summary>
        /// Busca em um <see cref="IQueryable{T}"/> usando uma expressao lambda a partir do nome de
        /// uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="T"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static IQueryable<T> WhereExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true) => List.Where(WhereExpression<T>(PropertyName, Operator, PropertyValue, Is));

        public static IEnumerable<T> WhereNotBlank<T>(this IEnumerable<T> List) => List.Where(x => x.IsValid());

        public static IQueryable<T> WhereNotNull<T>(this IQueryable<T> List) => List.Where(x => x != null);

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> List) => List.Where(x => x != null);

        public static IEnumerable<Type> WhereType<BaseType, Type>(this IEnumerable<BaseType> List) where Type : BaseType => List.Where(x => x is Type).Cast<Type>();

        ///<inheritdoc cref="With{T}(T, Action{T}, out Exception)"/>
        public static T With<T>(this T Obj, Action<T> Callback) => With(Obj, Callback, out _);

        /// <summary>
        /// Run a <see cref="Action{T}"/> inside a Try-Catch block and return the same <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Obj">Object</param>
        /// <param name="Callback">The action to execute</param>
        /// <param name="ex">
        /// An out param to capture a <see cref="Exception"/> if <paramref name="Callback"/> fails
        /// </param>
        /// <returns>The same <paramref name="Obj"/></returns>
        public static T With<T>(this T Obj, Action<T> Callback, out Exception ex)
        {
            ex = TryExecute(() => Callback?.Invoke(Obj));
            return Obj;
        }

        /// <summary>
        /// Executa uma função com um determinado arquivo caso seja possível sua leitura
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <param name="OnSuccess">Função a ser executada ao abrir o arquivo</param>
        /// <param name="OnFail">
        /// Função a ser executada após um numero determinado de tentativas falharem
        /// </param>
        /// <param name="OnAttemptFail">Função a ser executada a cada tentativa falhada</param>
        /// <param name="Seconds">Tempo de espera em segundos entre uma tentativa e outra</param>
        /// <param name="MaxFailCount">Numero máximo de tentativas, infinito se null</param>
        /// <returns>
        /// TRUE após <paramref name="OnSuccess"/> ser executada com sucesso. FALSE em qualquer
        /// outra situação
        /// </returns>
        public static bool WithFile(this FileInfo File, Action<FileInfo> OnSuccess, Action<FileInfo> OnFail, Action<int> OnAttemptFail = null, int Seconds = 1, int? MaxFailCount = null)
        {
            if (File != null)
            {
                try
                {
                    if (WaitForFile(File, Seconds, MaxFailCount, OnAttemptFail))
                    {
                        OnSuccess?.Invoke(File);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ex.WriteDebug("Execution of OnSuccess failed");

                    try
                    {
                        OnFail?.Invoke(File);
                    }
                    catch (Exception exf)
                    {
                        exf.WriteDebug("Execution of OnFail failed");
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Return a code for a word based on pronuciation
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static int GetSoundCode(this string word)
        {
            // Converter a palavra para maiúsculas e remover caracteres não-alfabéticos
            word = word.ToUpper();

            // Mapear as letras para seus códigos correspondentes
            string[] letterMappings = {
            "A",
            "EI",
            "OUL",
            "BP",
            "F",
            "VW",
            "CGKQ",
            "SJXZ",
            "DT",
            "MN",
            "RH"
        };

            string code = "";

            foreach (char v in word)
                for (int j = 0; j < letterMappings.Length; j++)
                    if (letterMappings[j].Contains(v))
                    {
                        code += j.ToString();
                        break;
                    }

            // Remover os dígitos repetidos consecutivos
            for (int i = code.Length - 1; i > 0; i--)
            {
                if (code[i] == code[i - 1])
                    code.Remove(i, 1);
            }
            return code.FixedLenghtByRight(4).ToInt();
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="WrapText">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string WrapText = DoubleQuoteChar) => Text.Wrap(WrapText, WrapText);

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string OpenWrapText, string CloseWrapText) => $"{OpenWrapText}{Text}{CloseWrapText.IfBlank(OpenWrapText)}";

        public static HtmlElementNode WrapInTag(this IEnumerable<HtmlElementNode> Tags, string TagName) => new HtmlElementNode(TagName).Add(Tags);

        public static HtmlElementNode WrapInTag(this HtmlElementNode Tag, string TagName) => new HtmlElementNode(TagName).Add(Tag);

        public static HtmlElementNode WrapInTag(this string Text, string TagName) => new HtmlElementNode(TagName, InnerHtml: Text);





        /// <summary>
        /// Write a message using <see cref="Debug.WriteLine(value,category)"/> when <see
        /// cref="EnableDebugMessages"/> is <b>true</b>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="category"></param>
        public static async void WriteDebug<T>(this T value, string category = null)
        {
#if DEBUG
            if (EnableDebugMessages)
            {
                category = $"{new StackFrame(3, true).GetMethod()?.Name.AppendIf(" => ", category.IsValid())} {category}".Trim();
                Debug.WriteLine(value, category);
            }
#endif
        }

        /// <summary>
        /// salva um <see cref="Stream"/> em um arquivo
        /// </summary>
        /// <param name="Stream">stream a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos FilePath. default é <see cref="DateTime.Now"/>
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this Stream Stream, string FilePath, DateTime? DateAndTime = null) => Stream.ToBytes().WriteToFile(FilePath, DateAndTime);

        public static FileInfo WriteToFile(this byte[] Bytes, DirectoryInfo Directory, string SubDirectory, string FileName, DateTime? DateAndTime = null) => WriteToFile(Bytes, Path.Combine(Directory?.FullName, SubDirectory, Path.GetFileName(FileName)), DateAndTime);

        public static FileInfo WriteToFile(this byte[] Bytes, DirectoryInfo Directory, string FileName, DateTime? DateAndTime = null) => WriteToFile(Bytes, Path.Combine(Directory?.FullName, Path.GetFileName(FileName)), DateAndTime);

        /// <summary>
        /// Salva um Array de Bytes em um arquivo
        /// </summary>
        /// <param name="Bytes">A MAtriz com os Bytes a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos FilePath. default é <see cref="DateTime.Now"/> ( <see cref="FormatPath"/>)
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this byte[] Bytes, string FilePath, DateTime? DateAndTime = null)
        {
            Bytes = Bytes ?? Array.Empty<byte>();
            DateAndTime = DateAndTime ?? DateTime.Now;

            FilePath = DateAndTime.FormatPath(FilePath);

            if (FilePath.IsFilePath())
            {
                FilePath.CreateDirectoryIfNotExists(DateAndTime);
                if (Bytes.Any())
                {
                    File.WriteAllBytes(FilePath, Bytes);
                    WriteDebug(FilePath, "File Written");
                }
                else
                {
                    WriteDebug("Bytes array is empty", "File not Written");
                }

                return new FileInfo(FilePath).With(x => { x.LastWriteTime = DateAndTime.Value; });
            }
            else
            {
                throw new ArgumentException($"FilePath is not a valid file file path: {FilePath}");
            }
        }

        /// <summary>
        /// Salva um texto em um arquivo
        /// </summary>
        /// <param name="File">TEntity arquivo a ser convertido</param>
        /// <returns>Um array do tipo Byte()</returns>
        /// <summary>
        /// Salva um texto em um arquivo
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="FilePath">Caminho do arquivo</param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, string FilePath, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null)
        {
            StreamWriter s = null;

            try
            {
                DateAndTime = DateAndTime ?? DateTime.Now;
                FilePath = DateAndTime.FormatPath(FilePath);
                var f = new FileInfo(FilePath);
                f.CreateDirectoryIfNotExists(DateAndTime);
                s = new StreamWriter(FilePath, Append, Enconding ?? new UTF8Encoding(false));
                s.Write(Text);
                s.Close();
                f.LastWriteTime = DateAndTime.Value;
                return f;
            }
            catch (Exception ex)
            {
                throw new Exception("Can't write to file:", ex);
            }
            finally
            {
                s?.Dispose();
            }
        }

        /// <summary>
        /// Salva um texto em um arquivo
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="File">Arquivo</param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, FileInfo File, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(File.FullName, Append, Enconding, DateAndTime);

        public static FileInfo WriteToFile(this string Text, DirectoryInfo Directory, string FileName, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(Path.Combine(Directory?.FullName, Path.GetFileName(FileName)), Append, Enconding, DateAndTime);

        public static FileInfo WriteToFile(this string Text, DirectoryInfo Directory, string SubDirectory, string FileName, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(Path.Combine(Directory?.FullName, SubDirectory, Path.GetFileName(FileName)), Append, Enconding, DateAndTime);

        public enum LogicConcatenationOperator
        {
            AND,
            OR
        }

        public enum RomanDigit
        {
            /// <summary>
            /// Valor correspondente a 1
            /// </summary>
            I = 1,

            /// <summary>
            /// Valor correspondente a 5
            /// </summary>
            V = 5,

            /// <summary>
            /// Valor correspondente a 10
            /// </summary>
            X = 10,

            /// <summary>
            /// Valor correspondente a 50
            /// </summary>
            L = 50,

            /// <summary>
            /// Valor correspondente a 100
            /// </summary>
            C = 100,

            /// <summary>
            /// Valor correspondente a 500
            /// </summary>
            D = 500,

            /// <summary>
            /// Valor correspondente a 1000
            /// </summary>
            M = 1000
        }
    }
}