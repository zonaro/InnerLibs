
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic;

namespace InnerLibs
{

    /// <summary>
/// Arrays de uso comum da biblioteca
/// </summary>
    public static class Arrays
    {

        /// <summary>
    /// Caracteres usado para encapsular palavras em textos
    /// </summary>
    /// <returns></returns>
        public static IEnumerable<string> WordWrappers
        {
            get
            {
                return OpenWrappers.Union(CloseWrappers);
            }
        }

        public static IEnumerable<string> AlphaLowerChars
        {
            get
            {
                return Consonants.Union(Vowels).OrderBy(x => x).AsEnumerable();
            }
        }

        public static IEnumerable<string> AlphaUpperChars
        {
            get
            {
                return AlphaLowerChars.Select(x => x.ToUpper());
            }
        }

        public static IEnumerable<string> AlphaChars
        {
            get
            {
                return AlphaUpperChars.Union(AlphaLowerChars).OrderBy(x => x).AsEnumerable();
            }
        }

        public static IEnumerable<string> AlphaNumericChars
        {
            get
            {
                return AlphaChars.Union(NumberChars).AsEnumerable();
            }
        }

        public static IEnumerable<string> NumberChars
        {
            get
            {
                return new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }.AsEnumerable();
            }
        }

        public static IEnumerable<string> BreakLineChars
        {
            get
            {
                return new[] { Environment.NewLine, Constants.vbCr, Constants.vbLf, Constants.vbCrLf, Constants.vbNewLine }.AsEnumerable();
            }
        }

        public static IEnumerable<string> CloseWrappers
        {
            get
            {
                return new[] { "\"", "'", ")", "}", "]", ">" }.AsEnumerable();
            }
        }

        public static IEnumerable<string> EndOfSentencePunctuation
        {
            get
            {
                return new[] { ".", "?", "!" }.AsEnumerable();
            }
        }

        public static IEnumerable<string> MidSentencePunctuation
        {
            get
            {
                return new[] { ":", ";", "," }.AsEnumerable();
            }
        }

        public static IEnumerable<string> OpenWrappers
        {
            get
            {
                return new[] { "\"", "'", "(", "{", "[", "<" }.AsEnumerable();
            }
        }

        /// <summary>
    /// Caracteres em branco
    /// </summary>
    /// <returns></returns>
        public static IEnumerable<string> WhiteSpaceChars
        {
            get
            {
                return new[] { Environment.NewLine, " ", Constants.vbTab, Constants.vbLf, Constants.vbCr, Constants.vbCrLf }.AsEnumerable();
            }
        }

        /// <summary>
    /// Strings utilizadas para descobrir as palavras em uma string
    /// </summary>
    /// <returns></returns>
        public static IEnumerable<string> WordSplitters
        {
            get
            {
                return new[] { "&nbsp;", "\"", "'", "(", ")", ",", ".", "?", "!", ";", "{", "}", "[", "]", "|", " ", ":", Constants.vbNewLine, "<br>", "<br/>", "<br/>", Environment.NewLine, Constants.vbCr, Constants.vbCrLf }.AsEnumerable();
            }
        }

        public static IEnumerable<string> Consonants
        {
            get
            {
                return new[] { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "z" }.AsEnumerable();
            }
        }

        public static IEnumerable<string> Vowels
        {
            get
            {
                return new[] { "a", "e", "i", "o", "u", "y" }.AsEnumerable();
            }
        }

        public static IEnumerable<string> PasswordSpecialChars
        {
            get
            {
                return new[] { "@", "#", "$", "%", "&" }.Union(WordWrappers).Union(EndOfSentencePunctuation).Union(MidSentencePunctuation).AsEnumerable();
            }
        }

        public static IEnumerable<Type> PrimitiveTypes
        {
            get
            {
                return new[] { typeof(string), typeof(char), typeof(byte), typeof(sbyte), typeof(DateTime) }.Union(PrimitiveNumericTypes);
            }
        }

        public static IEnumerable<Type> PrimitiveNumericTypes
        {
            get
            {
                return new[] { typeof(float), typeof(ushort), typeof(short), typeof(int), typeof(uint), typeof(ulong), typeof(long), typeof(double), typeof(decimal) }.AsEnumerable();
            }
        }
    }
}