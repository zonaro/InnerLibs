using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs
{
    /// <summary>
    /// Arrays de uso comum da biblioteca
    /// </summary>
    public static class PredefinedArrays
    {
        public static IEnumerable<string> AlphaChars => AlphaUpperChars.Union(AlphaLowerChars).OrderBy(x => x).AsEnumerable();
        public static IEnumerable<string> AlphaLowerChars => LowerConsonants.Union(LowerVowels).OrderBy(x => x).AsEnumerable();
        public static IEnumerable<string> AlphaNumericChars => AlphaChars.Union(NumberChars).AsEnumerable();
        public static IEnumerable<string> AlphaUpperChars => AlphaLowerChars.Select(x => x.ToUpper());
        public static IEnumerable<string> BreakLineChars => new[] { Environment.NewLine, "\t", "\n", "\r", "\r\n", "<br>", "<br/>", "<br />" }.AsEnumerable();
        public static IEnumerable<string> CloseWrappers => new[] { "\"", "'", ")", "}", "]", ">" }.AsEnumerable();
        public static IEnumerable<string> Consonants => UpperConsonants.Union(LowerConsonants).AsEnumerable();
        public static IEnumerable<string> EndOfSentencePunctuation => new[] { ".", "?", "!" }.AsEnumerable();
        public static IEnumerable<string> InvisibleChars => WhiteSpaceChars.Union(BreakLineChars).AsEnumerable();
        public static IEnumerable<string> LowerConsonants => new[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z" }.AsEnumerable();
        public static IEnumerable<string> LowerVowels => new[] { "a", "e", "i", "o", "u", "y" }.AsEnumerable();
        public static IEnumerable<string> MidSentencePunctuation => new[] { ":", ";", "," }.AsEnumerable();
        public static IEnumerable<DayOfWeek> MondayToFriday => new[] { 1, 2, 3, 4, 5 }.Cast<DayOfWeek>();
        public static IEnumerable<DayOfWeek> MondayToSaturday => new[] { 1, 2, 3, 4, 5, 6 }.Cast<DayOfWeek>();
        public static IEnumerable<string> NumberChars => new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }.AsEnumerable();
        public static IEnumerable<Type> NumericTypes => new[] { typeof(float), typeof(ushort), typeof(short), typeof(int), typeof(uint), typeof(ulong), typeof(long), typeof(double), typeof(decimal) }.AsEnumerable();
        public static IEnumerable<string> OpenWrappers => new[] { "\"", "'", "(", "{", "[", "<" }.AsEnumerable();
        public static IEnumerable<string> PasswordSpecialChars => SpecialChars.Union(WordWrappers).Union(EndOfSentencePunctuation).Union(MidSentencePunctuation).Where(x => x.IsNotAny("\"", "\'")).AsEnumerable();
        public static IEnumerable<char> RegexChars => new[] { '.', '$', '^', '{', '[', '(', '|', ')', '*', '+', '?', '|' }.AsEnumerable();
        public static IEnumerable<string> Slashes => new[] { "|", @"\", "/" }.AsEnumerable();
        public static IEnumerable<string> SpecialChars => new[] { "@", "#", "$", "%", "&" }.AsEnumerable();
        public static IEnumerable<DayOfWeek> SundayToSaturday => new[] { 0, 1, 2, 3, 4, 5, 6 }.Cast<DayOfWeek>();
        public static IEnumerable<string> UpperConsonants => LowerConsonants.Select(x => x.ToUpper()).AsEnumerable();

        public static IEnumerable<string> UpperVowels => LowerVowels.Select(x => x.ToUpper()).AsEnumerable();

        public static IEnumerable<Type> ValueTypes => new[] { typeof(string), typeof(char), typeof(byte), typeof(sbyte), typeof(DateTime) }.Union(NumericTypes);

        public static IEnumerable<string> Vowels => UpperVowels.Union(LowerVowels).AsEnumerable();

        /// <summary>
        /// Caracteres em branco
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> WhiteSpaceChars => new[] { " ", "&nbsp;" }.AsEnumerable();

        /// <summary>
        /// Strings utilizadas para descobrir as palavras em uma string
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> WordSplitters => InvisibleChars.Union(PasswordSpecialChars).AsEnumerable();

        /// <summary>
        /// Caracteres usado para encapsular palavras em textos
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> WordWrappers => OpenWrappers.Union(CloseWrappers);
    }
}