using System;
using System.Linq;
using System.Text;

namespace InnerLibs
{



    /// <summary>
    /// Classe para encodar IDs numéricos em hashs curtas
    /// </summary>
    /// <remarks>Não utilize isto como criptografia de segurança</remarks>
    public class ShortLinkGenerator
    {
        public readonly string Seed = null;
        public readonly string Token;
        private string urlPattern;

        public string UrlPattern { get => urlPattern; set => urlPattern = value.ValidateOr(x => x.IsURL(), UrlPattern); }
        public ShortLinkGenerator() : this(null) => Seed = "";


        public ShortLinkGenerator(string Seed, string UrlPattern) : this(Seed) => this.UrlPattern = UrlPattern;

        public ShortLinkGenerator(string Seed)
        {
            if (Seed.IsNotBlank())
            {
                for (int index = 1, loopTo = Seed.Length; index <= loopTo; index++)
                {
                    Token = PredefinedArrays.AlphaNumericChars.OrderBy(x => Encoding.ASCII.GetBytes(x.ToString()).FirstOrDefault() ^ Encoding.ASCII.GetBytes(Seed[index - 1].ToString()).FirstOrDefault()).JoinString();
                }

                this.Seed = Seed;
            }
            else
            {
                this.Seed = null;
                Token = PredefinedArrays.AlphaNumericChars.JoinString();
            }

            if (Seed.IsURL() && UrlPattern.IsBlank())
            {
                UrlPattern = Seed;
            }
        }

        /// <summary>
        /// Gera um link com a hash
        /// </summary>
        /// <param name="ID">Valor da Hash</param>
        /// <returns></returns>
        public Uri CreateLink(string UrlPattern, int ID) => new Uri(UrlPattern.BlankCoalesce(urlPattern).Inject(new { seed = Seed, id = ID, hash = Encode(ID) }).NullIf(x => x.IsBlank()));

        /// <summary>
        /// Gera um link com a hash
        /// </summary>
        /// <param name="ID">Valor da Hash</param>
        /// <returns></returns>
        public Uri CreateLink(int ID) => CreateLink(urlPattern, ID);

        public int Decode(string s)
        {
            int i = 0;
            try
            {
                foreach (var c in s.Trim().Trim('0'))
                {
                    i = i * Token.Length + Token.IndexOf(c);
                }

                return i;
            }
            catch
            {
                return -1;
            }
        }

        public string Encode(int i)
        {
            if (i == 0)
            {
                return Token[0].ToString();
            }

            string s = string.Empty;
            while (i > 0)
            {
                s += Convert.ToString(Token[i % Token.Length]);
                i /= Token.Length;
            }

            while (s.Length < 4)
            {
                s += "0";
            }

            return string.Join(string.Empty, s.Reverse());
        }

        public string RandomHash() => Encode(Generate.RandomNumber());

        public string RandomHash(int Min, int Max) => Encode(Generate.RandomNumber(Min, Max));

        public override string ToString() => Token;
    }
}