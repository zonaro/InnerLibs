using System;
using System.Collections.Generic;
using System.Linq;

namespace InnerLibs
{
    [Obsolete("Experimental Cryptography. Insecure")]
    public static class InnerCrypt
    {
        #region Private Properties

        private static string[] Array_letras => PredefinedArrays.AlphaChars.ToArray();

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Criptografa uma suma string usando a logica InnerCrypt
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string InnCrypt(this string Text, int Seed = 1)
        {
            var letras = Text.ToArray();
            var num = new List<string>();
            foreach (var c in letras)
            {
                var casc = c.ToAsc();
                string ll = casc.ToString();
                int i = ll.GetFirstChars(1).IfBlank(1) + ll.GetLastChars(1).IfBlank(1);
                i = i.LimitRange(0, Array_letras.Length - 1);
                num.Add(Array_letras[i] + Math.Pow(casc, 3 * Seed.SetMinValue(1)));
            }

            num.Reverse();
            return num.SelectJoinString(string.Empty);
        }

        /// <summary>
        /// Descriptografa uma string previamente criptografada com InnerCrypt
        /// </summary>
        /// <param name="EncryptedText">Texto Criptografado</param>
        /// <returns></returns>
        public static string UnnCrypt(this string EncryptedText, int Seed = 1)
        {
            try
            {
                var num = EncryptedText.Split(Array_letras, StringSplitOptions.RemoveEmptyEntries);
                var letras = new List<char>();
                foreach (var n in num) letras.Add((char)Math.Pow(n.ToDouble(), 1d / (double)(3 * Seed.SetMinValue(1))));
                letras.Reverse();
                return letras.SelectJoinString(InnerLibs.Text.Empty);
            }
            catch
            {
                return null;
            }
        }

        #endregion Public Methods
    }
}