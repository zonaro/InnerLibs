using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using InnerLibs.LINQ;
using InnerLibs.Locations;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{

    /// <summary>
/// Geradores de conteudo
/// </summary>
/// <remarks></remarks>
    public static class Generate
    {

        /// <summary>
    /// Gera uma palavra aleatória com o numero de caracteres
    /// </summary>
    /// <param name="Length">Tamanho da palavra</param>
    /// <returns>Uma string contendo uma palavra aleatória</returns>
        public static string RandomWord(int Length = 0)
        {
            Length = Length < 1 ? RandomNumber(2, 15) : Length;
            string word = "";
            if (Length == 1)
            {
                return Text.RandomItem(Arrays.Consonants.Union(Arrays.Vowels).ToArray());
            }

            // Generate the word in consonant / vowel pairs
            while (word.Length < Length)
            {

                // Add the consonant
                string consonant = Arrays.Consonants.GetRandomItem();
                if (consonant == "q" && word.Length + 3 <= Length)
                {
                    // check +3 because we'd add 3 characters in this case, the "qu" and the vowel.  Change 3 to 2 to allow words that end in "qu"
                    word += "qu";
                }
                else
                {
                    while (consonant == "q")
                        // ReplaceFrom an orphaned "q"
                        consonant = Arrays.Consonants.GetRandomItem();
                    if (word.Length + 1 <= Length)
                    {
                        // Only add a consonant if there's enough room remaining
                        word += consonant;
                    }
                }

                if (word.Length + 1 <= Length)
                {
                    // Only add a vowel if there's enough room remaining
                    word += Arrays.Vowels.GetRandomItem();
                }
            }

            return word;
        }

        public static object RandomString(int Len)
        {
            return RandomWord(Len);
        }

        /// <summary>
    /// Gera uma palavra aleatória a partir de uma outra palavra
    /// </summary>
    /// <param name="BaseText">Texto base</param>
    /// <returns></returns>
        public static string RandomWord(string BaseText)
        {
            return BaseText.ToArray().Shuffle().Join("");
        }

        /// <summary>
    /// Gera uma senha
    /// </summary>
    /// <returns></returns>
        public static string Password(int AlphaLenght, int NumberLenght, int SpecialLenght)
        {
            string pass = "";
            if (AlphaLenght > 0)
            {
                string ss = "";
                while (ss.Length < AlphaLenght)
                    ss = ss.Append(Arrays.AlphaChars.TakeRandom(1).FirstOrDefault());
                pass = pass.Append(ss);
            }

            if (NumberLenght > 0)
            {
                string ss = "";
                while (ss.Length < NumberLenght)
                    ss = ss.Append(Arrays.NumberChars.TakeRandom(1).FirstOrDefault());
                pass = pass.Append(ss);
            }

            if (SpecialLenght > 0)
            {
                string ss = "";
                while (ss.Length < SpecialLenght)
                    ss = ss.Append(Arrays.PasswordSpecialChars.TakeRandom(1).FirstOrDefault());
                pass = pass.Append(ss);
            }

            return pass.Shuffle();
        }

        public static string Password(int Lenght = 8)
        {
            double @base = Lenght / 3d;
            return Password(@base.CeilInt(), @base.FloorInt(), @base.FloorInt()).PadRight(Lenght, Conversions.ToChar(Arrays.AlphaChars.TakeRandom(1).FirstOrDefault())).GetFirstChars(Lenght);
        }


        /// <summary>
    /// Gera uma URL do google MAPs baseado na localização
    /// </summary>
    /// <param name="local">Uma variavel do tipo InnerLibs.Location onde estão as informações como endereço e as coordenadas geográficas</param>
    /// <param name="LatLong">Gerar URL baseado na latitude e Longitude. Padrão FALSE retorna a URL baseada no Logradouro</param>
    /// <returns>Uma URI do Google Maps</returns>

        public static Uri ToGoogleMapsURL(this AddressInfo local, bool LatLong = false)
        {
            string s;
            if (LatLong == true && local.Latitude.HasValue && local.Longitude.HasValue)
            {
                s = Uri.EscapeUriString(local.LatitudeLongitude().AdjustWhiteSpaces());
            }
            else
            {
                s = Uri.EscapeUriString(local.FullAddress.AdjustWhiteSpaces());
            }

            return new Uri("https://www.google.com.br/maps/search/" + s);
        }

        /// <summary>
    /// Gera um valor boolean aleatorio considerando uma porcentagem de chance
    /// </summary>
    /// <returns>TRUE ou FALSE.</returns>
        public static bool RandomBoolean(int Percent)
        {
            return RandomBoolean(x => x <= Percent, 0L, 100L);
        }

        /// <summary>
    /// Gera um valor boolean aleatorio considerando uma condiçao
    /// </summary>
    /// <param name="Min">Numero minimo, Padrão 0 </param>
    /// <param name="Max">Numero Maximo, Padrão 999999</param>
    /// <returns>TRUE ou FALSE</returns>
        public static bool RandomBoolean(Func<long, bool> Condition, long Min = 0L, long Max = 999999L)
        {
            return Condition(init_rnd.Next((int)Min, (int)(Max + 1L)));
        }

        /// <summary>
    /// Gera um valor boolean aleatorio
    /// </summary>
    /// <returns>TRUE ou FALSE</returns>
        public static bool RandomBoolean()
        {
            return init_rnd.Next(0, 1).ToBoolean();
        }

        /// <summary>
    /// Gera um numero Aleatório entre 2 números
    /// </summary>
    /// <param name="Min">Numero minimo, Padrão 0 </param>
    /// <param name="Max">Numero Maximo, Padrão 999999</param>
    /// <returns>Um numero Inteiro (Integer ou Int)</returns>
        public static int RandomNumber(int Min = 0, int Max = 999999)
        {
            return init_rnd.Next(Min, Max + 1);
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
        public static List<Color> RandomColorList(int Quantity, int Red = -1, int Green = -1, int Blue = -1)
        {
            var l = new List<Color>();
            if (Red == Green && Green == Blue && Blue != -1)
            {
                l.Add(Color.FromArgb(Red, Green, Blue));
                return l;
            }

            int errorcount = 0;
            while (l.Count < Quantity)
            {
                var r = ColorExtensions.RandomColor(Red, Green, Blue);
                if (l.Any(x => (x.ToHexadecimal() ?? "") == (r.ToHexadecimal() ?? "")))
                {
                    errorcount = errorcount + 1;
                    if (errorcount == 5)
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

        private static Random init_rnd = new Random();

        /// <summary>
    /// Gera um texto aleatorio
    /// </summary>
    /// <param name="ParagraphCount">Quantidade de paragrafos</param>
    /// <param name="SentenceCount">QUantidade de sentecas por paragrafo</param>
    /// <param name="MinWordCount"></param>
    /// <param name="MaxWordCount"></param>
    /// <returns></returns>
        public static StructuredText RandomIpsum(int ParagraphCount = 5, int SentenceCount = 3, int MinWordCount = 10, int MaxWordCount = 50, int IdentSize = 0, int BreakLinesBetweenParagraph = 0)
        {
            return new StructuredText(Enumerable.Range(1, ParagraphCount.SetMinValue(1)).SelectJoin(pp => Enumerable.Range(1, SentenceCount.SetMinValue(1)).SelectJoin(s => Enumerable.Range(1, RandomNumber(MinWordCount.SetMinValue(1), MaxWordCount.SetMinValue(1))).SelectJoin(p => RandomBoolean(20).AsIf(RandomWord(RandomNumber(2, 6)).ToUpper(), RandomWord()) + RandomBoolean(30).AsIf(","), " "), Arrays.EndOfSentencePunctuation.FirstRandom() + " "), Environment.NewLine)) { Ident = IdentSize, BreakLinesBetweenParagraph = BreakLinesBetweenParagraph };
        }

        /// <summary>
    /// Converte uma String para um QR Code usando uma API (Nescessita de Internet)
    /// </summary>
    /// <param name="Data">Informações do QR Code</param>
    /// <param name="Size">Tamanho do QR code</param>
    /// <returns>Um componente Image() com o QR code</returns>

        public static byte[] ToQRCode(this string Data, int Size = 100)
        {
            Data = Data.IsURL() ? Data.UrlEncode() : Data;
            string URL = "https://chart.googleapis.com/chart?cht=qr&chl=" + Data.UrlEncode() + "&chs=" + Size + "x" + Size;
            return Web.GetFile(URL);
        }
    }
}