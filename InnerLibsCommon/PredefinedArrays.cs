using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;

namespace Extensions
{



    /// <summary>
    /// Classe estática que contém arrays pré-definidos de caracteres e tipos.
    /// </summary>
    public static class PredefinedArrays
    {
        #region Public Properties

        /// <summary>
        /// Gets the collection of ASCII art characters.
        /// </summary>
        public static IEnumerable<string> AsciiArtChars => new[] { "#", "#", "@", "%", "=", "+", "*", ":", "-", ".", Util.WhitespaceChar };

        /// <summary>
        /// Gets the collection of alphabetical characters (both upper and lower case).
        /// </summary>
        public static IEnumerable<string> AlphaChars => AlphaUpperChars.Union(AlphaLowerChars).OrderBy(x => x).AsEnumerable();

        /// <summary>
        /// Gets the collection of lowercase alphabetical characters.
        /// </summary>
        public static IEnumerable<string> AlphaLowerChars => LowerConsonants.Union(LowerVowels).OrderBy(x => x).AsEnumerable();

        /// <summary>
        /// Gets the collection of alphanumeric characters.
        /// </summary>
        public static IEnumerable<string> AlphaNumericChars => AlphaChars.Union(NumberChars).AsEnumerable();

        /// <summary>
        /// Gets the collection of uppercase alphabetical characters.
        /// </summary>
        public static IEnumerable<string> AlphaUpperChars => AlphaLowerChars.Select(x => x.ToUpperInvariant());

        /// <summary>
        /// Gets the collection of characters used for line breaks.
        /// </summary>
        public static IEnumerable<string> BreakLineChars => new[] { Environment.NewLine, "\n", "\r", "\r\n" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used for closing wrappers (quotes, brackets, etc.).
        /// </summary>
        public static IEnumerable<string> CloseWrappers => new[] { Util.DoubleQuoteChar, Util.SingleQuoteChar, ")", "}", "]", ">", "`" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of consonants (both upper and lower case).
        /// </summary>
        public static IEnumerable<string> Consonants => UpperConsonants.Union(LowerConsonants).AsEnumerable();

        /// <summary>
        /// Gets the collection of end-of-sentence punctuation characters.
        /// </summary>
        public static IEnumerable<string> EndOfSentencePunctuation => new[] { ".", "?", "!" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used for indentation.
        /// </summary>
        public static IEnumerable<string> IdentChars => new[] { "\t" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of invisible characters (whitespace, line breaks, indentation).
        /// </summary>
        public static IEnumerable<string> InvisibleChars => WhiteSpaceChars.Union(BreakLineChars).Union(IdentChars).AsEnumerable();

        /// <summary>
        /// Gets the collection of lowercase consonants.
        /// </summary>
        public static IEnumerable<string> LowerConsonants => new[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of lowercase vowels.
        /// </summary>
        public static IEnumerable<string> LowerVowels => new[] { "a", "e", "i", "o", "u", "y" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of mid-sentence punctuation characters.
        /// </summary>
        public static IEnumerable<string> MidSentencePunctuation => new[] { ":", ";", "," }.AsEnumerable();

        /// <summary>
        /// Gets the collection of days of the week from Monday to Friday.
        /// </summary>
        public static IEnumerable<DayOfWeek> MondayToFriday => new[] { 1, 2, 3, 4, 5 }.Select(x => (DayOfWeek)x);

        /// <summary>
        /// Gets the collection of days of the week from Monday to Saturday.
        /// </summary>
        public static IEnumerable<DayOfWeek> MondayToSaturday => new[] { 1, 2, 3, 4, 5, 6 }.Select(x => (DayOfWeek)x);

        /// <summary>
        /// Gets the collection of numeric characters.
        /// </summary>
        public static IEnumerable<string> NumberChars => new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of telephone characters.
        /// </summary>
        public static IEnumerable<string> TelephoneChars => NumberChars.Union(new[] { "+", "(", ")", "-", " " }).AsEnumerable();

        /// <summary>
        /// Gets the collection of numeric types.
        /// </summary>
        public static IEnumerable<Type> NumericTypes => new[] { typeof(float), typeof(ushort), typeof(short), typeof(int), typeof(uint), typeof(ulong), typeof(long), typeof(double), typeof(decimal) }.AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used for opening wrappers (quotes, brackets, etc.).
        /// </summary>
        public static IEnumerable<string> OpenWrappers => new[] { Util.DoubleQuoteChar, Util.SingleQuoteChar, "(", "{", "[", "<", "`" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of special characters used in passwords.
        /// </summary>
        public static IEnumerable<string> PasswordSpecialChars => SpecialChars.Union(WordWrappers).Union(EndOfSentencePunctuation).Union(MidSentencePunctuation).Where(x => x.IsNotAny(Util.DoubleQuoteChar, Util.SingleQuoteChar)).AsEnumerable();

        /// <summary>
        /// Gets the collection of punctuation characters.
        /// </summary>
        public static IEnumerable<string> Punctuation => EndOfSentencePunctuation.Union(MidSentencePunctuation).AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used in regular expressions.
        /// </summary>
        public static IEnumerable<char> RegexChars => new[] { '.', '$', '^', '{', '[', '(', '|', ')', '*', '+', '?', '|' }.AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used for slashes (forward slash, backslash, pipe).
        /// </summary>
        public static IEnumerable<string> Slashes => new[] { "|", @"\", "/" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of special characters.
        /// </summary>
        public static IEnumerable<string> SpecialChars => new[] { "@", "#", "$", "%", "&" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of days of the week from Sunday to Saturday.
        /// </summary>
        public static IEnumerable<DayOfWeek> SundayToSaturday => new[] { 0, 1, 2, 3, 4, 5, 6 }.Select(x => (DayOfWeek)x);

        /// <summary>
        /// Gets the collection of uppercase consonants.
        /// </summary>
        public static IEnumerable<string> UpperConsonants => LowerConsonants.Select(x => x.ToUpperInvariant()).AsEnumerable();

        /// <summary>
        /// Gets the collection of uppercase vowels.
        /// </summary>
        public static IEnumerable<string> UpperVowels => LowerVowels.Select(x => x.ToUpperInvariant()).AsEnumerable();

        /// <summary>
        /// Gets the collection of value types (including numeric types and common types like string, char, DateTime, and Guid).
        /// </summary>
        public static IEnumerable<Type> ValueTypes => new[] { typeof(string), typeof(char), typeof(byte), typeof(sbyte), typeof(DateTime), typeof(Guid) }.Union(NumericTypes);

        /// <summary>
        /// Gets the collection of vowels (both upper and lower case).
        /// </summary>
        public static IEnumerable<string> Vowels => UpperVowels.Union(LowerVowels).AsEnumerable();

        /// <summary>
        /// Gets the collection of whitespace characters.
        /// </summary>
        public static IEnumerable<string> WhiteSpaceChars => new[] { " ", "&nbsp;" }.AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used as word splitters (invisible characters, special characters, punctuation).
        /// </summary>
        public static IEnumerable<string> WordSplitters => InvisibleChars.Union(PasswordSpecialChars).AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used as word wrappers (opening and closing wrappers).
        /// </summary>
        public static IEnumerable<string> WordWrappers => OpenWrappers.Union(CloseWrappers);

        #endregion Public Properties
    }

}