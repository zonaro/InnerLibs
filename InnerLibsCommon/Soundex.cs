using System;
using System.Text;

namespace Extensions.ComplexText
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
        public static bool SoundsLike(this string FirstText, string SecondText) => (FirstText.SoundEx() ?? Util.EmptyString) == (SecondText.SoundEx() ?? Util.EmptyString);

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
        public bool this[string Word] => (new Phonetic(Word).SoundExCode ?? Util.EmptyString) == (SoundExCode ?? Util.EmptyString) | (Word ?? Util.EmptyString) == (this.Word ?? Util.EmptyString);

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
                string Text = Word;
                Text = Text.Trim().ToUpperInvariant();
                if (Text.EndsWith("Z") & Text.StartsWith("Z"))
                {
                    Text = "Z" + Text.Trim('Z').Replace("Z", "S") + "S";
                }
                else if (Text.StartsWith("Z"))
                {
                    Text = "Z" + Text.TrimStartAny("Z").Replace("Z", "S");
                }
                else
                {
                    Text = Text.Replace("Z", "S");
                }

                Text = Text.Replace("Ç", "S");
                Text = Text.RemoveDiacritics();
                Text = Text.Replace("Y", "I");
                Text = Text.Replace("AL", "AU");
                Text = Text.Replace("BR", "B");
                Text = Text.Replace("BL", "B");
                Text = Text.Replace("PH", "F");
                Text = Text.Replace("MG", "G");
                Text = Text.Replace("NG", "G");
                Text = Text.Replace("RG", "G");
                Text = Text.Replace("GE", "J");
                Text = Text.Replace("GI", "J");
                Text = Text.Replace("RJ", "J");
                Text = Text.Replace("MJ", "J");
                Text = Text.Replace("NJ", "J");
                Text = Text.Replace("GR", "G");
                Text = Text.Replace("GL", "G");
                Text = Text.Replace("CE", "S");
                Text = Text.Replace("CI", "S");
                Text = Text.Replace("CH", "X");
                Text = Text.Replace("CT", "T");
                Text = Text.Replace("CS", "S");
                Text = Text.Replace("QU", "TK");
                Text = Text.Replace("Q", "TK");
                Text = Text.Replace("CA", "TK");
                Text = Text.Replace("CO", "TK");
                Text = Text.Replace("CU", "TK");
                Text = Text.Replace("CK", "TK");
                Text = Text.Replace("LH", "LI");
                Text = Text.Replace("RM", "SM");
                Text = Text.Replace("N", "M");
                Text = Text.Replace("GM", "M");
                Text = Text.Replace("MD", "M");
                Text = Text.Replace("NH", "N");
                Text = Text.Replace("PR", "P");
                Text = Text.Replace("X", "S");
                Text = Text.Replace("TS", "S");
                Text = Text.Replace("RS", "S");
                Text = Text.Replace("TR", "T");
                Text = Text.Replace("TL", "T");
                Text = Text.Replace("LT", "T");
                Text = Text.Replace("RT", "T");
                Text = Text.Replace("ST", "T");
                Text = Text.Replace("W", "TV");
                Text = Text.Replace("L", "R");
                Text = Text.Replace("H", Util.EmptyString);
                var sb = new StringBuilder(Text);
                if (Text.IsValid())
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

                    string frasesaida = Util.EmptyString;
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
                    return Util.EmptyString;
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