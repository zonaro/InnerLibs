using System;
using System.Linq;
using System.Text;

namespace InnerLibs
{
    /// <summary>
    /// Classe para encodar IDs numéricos em hashs curtas
    /// </summary>
    public class AlphabetType
    {
        public AlphabetType()
        {
        }

        public AlphabetType(string Seed)
        {
            if (Seed.IsNotBlank())
            {
                for (int index = 1, loopTo = Seed.Length; index <= loopTo; index++)
                {
                    Alphabet = PredefinedArrays.AlphaNumericChars.OrderBy(x => Encoding.ASCII.GetBytes(x.ToString()).FirstOrDefault() ^ Encoding.ASCII.GetBytes(Seed[index - 1].ToString()).FirstOrDefault()).JoinString("");
                }

                this.Seed = Seed;
            }
        }

        public readonly string Alphabet;

        public readonly string Seed = null;

        public string RandomHash() => Encode(Generate.RandomNumber());

        public string Encode(int i)
        {
            if (i == 0)
            {
                return Alphabet[0].ToString();
            }

            string s = string.Empty;
            while (i > 0)
            {
                s += Convert.ToString(Alphabet[i % Alphabet.Length]);
                i = i / Alphabet.Length;
            }

            while (s.Length < 4)
                s += "0";
            return string.Join(string.Empty, s.Reverse());
        }

        public int Decode(string s)
        {
            int i = 0;
            try
            {
                foreach (var c in s.Trim().Trim('0'))
                    i = i * Alphabet.Length + Alphabet.IndexOf(c);
                return i;
            }
            catch
            {
                return -1;
            }
        }

        public override string ToString() => Alphabet;

        /// <summary>
        /// Gera um link com a hash
        /// </summary>
        /// <param name="ID">Valor da Hash</param>
        /// <returns></returns>
        public Uri CreateLink(string UrlPattern, int ID) => new Uri(UrlPattern.Inject(new { id = ID, hash = Encode(ID) }));
    }
}