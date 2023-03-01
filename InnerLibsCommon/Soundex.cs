using System;
using System.Globalization;
using System.Text;

namespace InnerLibs
{
    public static class SoundExType
    {
        #region Private Methods

        /// <summary>
        /// Gera um código SOUNDEX para comparação de fonemas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>Um código soundex</returns>
        private static string SoundEx(string Text, int Length)
        {
            Text = Text ?? string.Empty;

            // Value to return
            string Value = string.Empty;
            // Size of the word to process
            int Size = Text.Length;
            // Make sure the word is at least two characters in length
            if (Size > 1)
            {
                // Convert the word to all uppercase
                Text = Text.ToUpperInvariant();
                // Convert to the word to a character array for faster processing
                var Chars = Text.ToCharArray();
                // Buffer to build up with character codes
                string Buffer = string.Empty;

                // The current and previous character codes
                int PrevCode = 0;
                int CurrCode = 0;
                // Append the first character to the buffer
                Buffer += Convert.ToString(Chars[0]);
                // Prepare variables for loop
                int i;
                int LoopLimit = Size - 1;
                // Loop through all the characters and convert them to the proper character code
                var loopTo = LoopLimit;
                for (i = 1; i <= loopTo; i++)
                {
                    switch (Chars[i])
                    {
                        case 'A':
                        case 'E':
                        case 'I':
                        case 'O':
                        case 'U':
                        case 'H':
                        case 'W':
                        case 'Y':
                            {
                                CurrCode = 0;
                                break;
                            }

                        case 'B':
                        case 'F':
                        case 'P':
                        case 'V':
                            {
                                CurrCode = 1;
                                break;
                            }

                        case 'C':
                        case 'G':
                        case 'J':
                        case 'K':
                        case 'Q':
                        case 'S':
                        case 'X':
                        case 'Z':
                            {
                                CurrCode = 2;
                                break;
                            }

                        case 'D':
                        case 'T':
                            {
                                CurrCode = 3;
                                break;
                            }

                        case 'L':
                            {
                                CurrCode = 4;
                                break;
                            }

                        case 'M':
                        case 'N':
                            {
                                CurrCode = 5;
                                break;
                            }

                        case 'R':
                            {
                                CurrCode = 6;
                                break;
                            }
                    }
                    // Check to see if the current code is the same as the last one
                    if (CurrCode != PrevCode)
                    {
                        // Check to see if the current code is 0 (a vowel); do not proceed
                        if (CurrCode != 0)
                        {
                            Buffer += CurrCode.ToString();
                        }
                    }
                    // If the buffer size meets the length limit, then exit the loop
                    if (Buffer.Length == Length)
                    {
                        break;
                    }
                }
                // Padd the buffer if required
                Size = Buffer.Length;
                if (Size < Length)
                {
                    Buffer = Buffer.PadLeft(Size, '0');
                }
                // Set the return value
                Value = Buffer.ToString();
            }
            // Return the computed soundex
            return Value;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Gera um código SOUNDEX para comparação de fonemas
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>Um código soundex</returns>
        public static string SoundEx(this string Text) => SoundEx(Text, 4);

        /// <summary>
        /// Compara 2 palavras e verifica se elas possuem fonema parecido
        /// </summary>
        /// <param name="FirstText">Primeira palavra</param>
        /// <param name="SecondText">Segunda palavra</param>
        /// <returns>TRUE se possuirem o mesmo fonema</returns>
        public static bool SoundsLike(this string FirstText, string SecondText) => (FirstText.SoundEx() ?? InnerLibs.Ext.EmptyString) == (SecondText.SoundEx() ?? InnerLibs.Ext.EmptyString);

        #endregion Public Methods
    }

    /// <summary>
    /// Implementação da função SoundEX em Portugues
    /// </summary>
    public sealed class Phonetic
    {
        #region Public Constructors

        /// <summary>
        /// Cria um novo Phonetic a partir de uma palavra
        /// </summary>
        /// <param name="Word">Palavra</param>
        public Phonetic(string Word)
        {
            try
            {
                this.Word = Word.Split(" ")[0] ?? string.Empty;
            }
            catch
            {
                this.Word = Word ?? string.Empty;
            }
        }

        #endregion Public Constructors

        #region Public Indexers

        /// <summary>
        /// Compara o fonema de uma palavra em portugues com outra palavra
        /// </summary>
        /// <param name="Word">Palavra para comparar</param>
        /// <returns></returns>
        public bool this[string Word] => (new Phonetic(Word).SoundExCode ?? InnerLibs.Ext.EmptyString) == (SoundExCode ?? InnerLibs.Ext.EmptyString) | (Word ?? InnerLibs.Ext.EmptyString) == (this.Word ?? InnerLibs.Ext.EmptyString);

        #endregion Public Indexers

        #region Public Properties

        /// <summary>
        /// Código SoundExBR que representa o fonema da palavra
        /// </summary>
        /// <returns></returns>
        public string SoundExCode
        {
            get
            {
                string text = Word;
                text = text.Trim().ToUpperInvariant();
                if (text.EndsWith("Z") & text.StartsWith("Z"))
                {
                    text = "Z" + text.Trim('Z').Replace("Z", "S") + "S";
                }
                else if (text.StartsWith("Z"))
                {
                    text = "Z" + text.TrimStartAny("Z").Replace("Z", "S");
                }
                else
                {
                    text = text.Replace("Z", "S");
                }

                text = text.Replace("Ç", "S");
                text = text.RemoveDiacritics();
                text = text.Replace("Y", "I");
                text = text.Replace("AL", "AU");
                text = text.Replace("BR", "B");
                text = text.Replace("BL", "B");
                text = text.Replace("PH", "F");
                text = text.Replace("MG", "G");
                text = text.Replace("NG", "G");
                text = text.Replace("RG", "G");
                text = text.Replace("GE", "J");
                text = text.Replace("GI", "J");
                text = text.Replace("RJ", "J");
                text = text.Replace("MJ", "J");
                text = text.Replace("NJ", "J");
                text = text.Replace("GR", "G");
                text = text.Replace("GL", "G");
                text = text.Replace("CE", "S");
                text = text.Replace("CI", "S");
                text = text.Replace("CH", "X");
                text = text.Replace("CT", "T");
                text = text.Replace("CS", "S");
                text = text.Replace("QU", "TK");
                text = text.Replace("Q", "TK");
                text = text.Replace("CA", "TK");
                text = text.Replace("CO", "TK");
                text = text.Replace("CU", "TK");
                text = text.Replace("CK", "TK");
                text = text.Replace("LH", "LI");
                text = text.Replace("RM", "SM");
                text = text.Replace("N", "M");
                text = text.Replace("GM", "M");
                text = text.Replace("MD", "M");
                text = text.Replace("NH", "N");
                text = text.Replace("PR", "P");
                text = text.Replace("X", "S");
                text = text.Replace("TS", "S");
                text = text.Replace("RS", "S");
                text = text.Replace("TR", "T");
                text = text.Replace("TL", "T");
                text = text.Replace("LT", "T");
                text = text.Replace("RT", "T");
                text = text.Replace("ST", "T");
                text = text.Replace("W", "TV");
                text = text.Replace("L", "R");
                text = text.Replace("H", InnerLibs.Ext.EmptyString);
                var sb = new StringBuilder(text);
                if (text.IsNotBlank())
                {
                    int tam = sb.Length - 1;
                    if (tam > -1)
                    {
                        if (Convert.ToString(sb[tam]) == "S" || Convert.ToString(sb[tam]) == "Z" || Convert.ToString(sb[tam]) == "R" || Convert.ToString(sb[tam]) == "M" || Convert.ToString(sb[tam]) == "N" || Convert.ToString(sb[tam]) == "L")
                        {
                            sb.Remove(tam, 1);
                        }
                    }

                    tam = sb.Length - 2;
                    if (tam > -1)
                    {
                        if (Convert.ToString(sb[tam]) == "A" && Convert.ToString(sb[tam + 1]) == "T")
                        {
                            sb.Remove(tam, 2);
                        }
                    }

                    string frasesaida = InnerLibs.Ext.EmptyString;
                    try
                    {
                        frasesaida += Convert.ToString(sb[0]);
                    }
                    catch
                    {
                    }

                    for (int i = 1, loopTo = sb.Length - 1; i <= loopTo; i++)
                    {
                        if (frasesaida[frasesaida.Length - 1] != sb[i] || char.IsDigit(sb[i]))
                        {
                            frasesaida += Convert.ToString(sb[i]);
                        }
                    }

                    return frasesaida.ToString();
                }
                else
                {
                    return InnerLibs.Ext.EmptyString;
                }
            }
        }

        /// <summary>
        /// Palavra Original
        /// </summary>
        /// <returns></returns>
        public string Word { get; set; }

        #endregion Public Properties

        #region Public Methods

        public override string ToString() => SoundExCode;

        #endregion Public Methods
    }
}