using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Extensions;

namespace Extensions
{



    /// <summary>
    /// Classe estática que contém arrays pré-definidos de caracteres e tipos.
    /// </summary>
    public static class PredefinedArrays
    {

        public static string[] ToStringArray(this IEnumerable<char> chars) => chars.Select(x => x.ToString()).ToArray();

        #region Public Properties

        /// <summary>
        /// Gets the collection of ASCII art characters.
        /// </summary>
        public static IEnumerable<char> AsciiArtChars => $"#@%=+*:-.{Util.WhitespaceChar}".ToCharArray();

        /// <summary>
        /// Gets the collection of alphabetical characters (both upper and lower case).
        /// </summary>
        public static IEnumerable<char> AlphaChars => AlphaUpperChars.Union(AlphaLowerChars).OrderBy(x => x).AsEnumerable();

        /// <summary>
        /// Gets the collection of lowercase alphabetical characters.
        /// </summary>
        public static IEnumerable<char> AlphaLowerChars => LowerConsonants.Union(LowerVowels).OrderBy(x => x).AsEnumerable();

        /// <summary>
        /// Gets the collection of alphanumeric characters.
        /// </summary>
        public static IEnumerable<char> AlphaNumericChars => AlphaChars.Union(NumberChars).AsEnumerable();

        /// <summary>
        /// Gets the collection of uppercase alphabetical characters.
        /// </summary>
        public static IEnumerable<char> AlphaUpperChars => AlphaLowerChars.Select(x => char.ToUpper(x));

        /// <summary>
        /// Gets the collection of characters used for line breaks.
        /// </summary>
        public static IEnumerable<char> BreakLineChars => "\r\n".ToCharArray().AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used for closing wrappers (quotes, brackets, etc.).
        /// </summary>
        public static IEnumerable<char> CloseWrappers => $"{Util.DoubleQuoteChar}{Util.SingleQuoteChar})}}]>`".ToCharArray().AsEnumerable();

        /// <summary>
        /// Gets the collection of consonants (both upper and lower case).
        /// </summary>
        public static IEnumerable<char> Consonants => UpperConsonants.Union(LowerConsonants).AsEnumerable();

        /// <summary>
        /// Gets the collection of end-of-sentence punctuation characters.
        /// </summary>
        public static IEnumerable<char> EndOfSentencePunctuation => ".?!";

        /// <summary>
        /// Gets the collection of characters used for indentation.
        /// </summary>
        public static IEnumerable<char> IdentChars => "\t".ToCharArray().AsEnumerable();

        /// <summary>
        /// Gets the collection of invisible characters (whitespace, line breaks, indentation).
        /// </summary>
        public static IEnumerable<char> InvisibleChars => WhiteSpaceChars.Union(BreakLineChars).Union(IdentChars).AsEnumerable();

        /// <summary>
        /// Gets the collection of lowercase consonants.
        /// </summary>
        public static IEnumerable<char> LowerConsonants => "bcdfghjklmnpqrstvwxz".ToCharArray().AsEnumerable();

        /// <summary>
        /// Gets the collection of lowercase vowels.
        /// </summary>
        public static IEnumerable<char> LowerVowels => "aeiouy".ToCharArray().AsEnumerable();

        /// <summary>
        /// Gets the collection of mid-sentence punctuation characters.
        /// </summary>
        public static IEnumerable<char> MidSentencePunctuation => ":;,".ToCharArray().AsEnumerable();

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
        public static IEnumerable<char> NumberChars => "0123456789".ToCharArray().AsEnumerable();

        public static IEnumerable<char> NumberMaskChars(CultureInfo Culture=null)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            return (Culture.NumberFormat.NumberDecimalSeparator + Culture.NumberFormat.NumberGroupSeparator).ToArray();
        }

        public static IEnumerable<char> MaskedNumberChars(CultureInfo Culture=null) => PredefinedArrays.NumberChars.Union(NumberMaskChars(Culture));

        /// <summary>
        /// Gets the collection of telephone characters.
        /// </summary>
        public static IEnumerable<char> TelephoneChars => NumberChars.Union(TelephoneMaskChars).AsEnumerable();
        public static IEnumerable<char> TelephoneMaskChars => new char[] { ' ', '+', '(', ')', '-' };

        /// <summary>
        /// Gets the collection of numeric types.
        /// </summary>
        public static IEnumerable<Type> NumericTypes => new[] { typeof(float), typeof(ushort), typeof(short), typeof(int), typeof(uint), typeof(ulong), typeof(long), typeof(double), typeof(decimal) }.AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used for opening wrappers (quotes, brackets, etc.).
        /// </summary>
        public static IEnumerable<char> OpenWrappers => $"{Util.DoubleQuoteChar}{Util.SingleQuoteChar}({{[`".ToCharArray();

        /// <summary>
        /// Gets the collection of special characters used in passwords.
        /// </summary>
        public static IEnumerable<char> PasswordSpecialChars => SpecialChars.Union(WordWrappers).Union(EndOfSentencePunctuation).Union(MidSentencePunctuation).JoinString().RemoveAny(Util.DoubleQuoteChar, Util.SingleQuoteChar).ToCharArray();

        /// <summary>
        /// Gets the collection of punctuation characters.
        /// </summary>
        public static IEnumerable<char> Punctuation => EndOfSentencePunctuation.Union(MidSentencePunctuation).AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used in regular expressions.
        /// </summary>
        public static IEnumerable<char> RegexChars => ".$^{[(|)*+?|".ToCharArray();

        /// <summary>
        /// Gets the collection of characters used for slashes (forward slash, backslash, pipe).
        /// </summary>
        public static IEnumerable<char> Slashes => @"\|/".ToCharArray();

        /// <summary>
        /// Gets the collection of special characters.
        /// </summary>
        public static IEnumerable<char> SpecialChars => "@#$%&".ToCharArray();

        /// <summary>
        /// Gets the collection of days of the week from Sunday to Saturday.
        /// </summary>
        public static IEnumerable<DayOfWeek> SundayToSaturday => new[] { 0, 1, 2, 3, 4, 5, 6 }.Select(x => (DayOfWeek)x);

        /// <summary>
        /// Gets the collection of uppercase consonants.
        /// </summary>
        public static IEnumerable<char> UpperConsonants => LowerConsonants.Select(x => char.ToUpper(x)).AsEnumerable();

        /// <summary>
        /// Gets the collection of uppercase vowels.
        /// </summary>
        public static IEnumerable<char> UpperVowels => LowerVowels.Select(x => char.ToUpper(x)).AsEnumerable();

        /// <summary>
        /// Gets the collection of value types (including numeric types and common types like string, char, DateTime, and Guid).
        /// </summary>
        public static IEnumerable<Type> ValueTypes => new[] { typeof(string), typeof(char), typeof(byte), typeof(sbyte), typeof(DateTime), typeof(Guid) }.Union(NumericTypes);

        /// <summary>
        /// Gets the collection of vowels (both upper and lower case).
        /// </summary>
        public static IEnumerable<char> Vowels => UpperVowels.Union(LowerVowels).AsEnumerable();

        /// <summary>
        /// Gets the collection of whitespace characters.
        /// </summary>
        public static IEnumerable<char> WhiteSpaceChars => new[] { ' ', '\t', '\n', '\r', '\v', '\f' };

        /// <summary>
        /// Gets the collection of characters used as word splitters (invisible characters, special characters, punctuation).
        /// </summary>
        public static IEnumerable<char> WordSplitters => InvisibleChars.Union(PasswordSpecialChars).AsEnumerable();

        /// <summary>
        /// Gets the collection of characters used as word wrappers (opening and closing wrappers).
        /// </summary>
        public static IEnumerable<char> WordWrappers => OpenWrappers.Union(CloseWrappers);

        public static IEnumerable<string> EmailDomains => new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "live.com", "aol.com", "icloud.com", "mail.com", "zoho.com", "protonmail.com" }.AsEnumerable();

        #endregion Public Properties
    }

}