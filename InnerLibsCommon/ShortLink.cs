﻿
using System;
using System.Linq;
using System.Text;
using Extensions;


namespace Exttensions.Web
{


    /// <summary>
    /// Classe para encodar IDs numéricos em hashs curtas
    /// </summary>
    /// <remarks>Não utilize isto como criptografia de segurança</remarks>
    public class ShortLinkGenerator
    {
        #region Private Fields

        private string urlPattern;

        #endregion Private Fields

        #region Public Fields

        public readonly string Seed = null;
        public readonly string Token;

        #endregion Public Fields

        #region Public Constructors

        public ShortLinkGenerator() : this(null) => Seed = Util.EmptyString;

        public ShortLinkGenerator(string Seed, string UrlPattern) : this(Seed) => this.UrlPattern = UrlPattern;

        public ShortLinkGenerator(string Seed)
        {
            if (Seed.IsValid())
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

            if (Seed.IsURL() && UrlPattern.IsNotValid())
            {
                UrlPattern = Seed;
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public string UrlPattern { get => urlPattern; set => urlPattern = value.ValidateOr( UrlPattern, x => x.IsURL().ToString()); }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Gera um link com a hash
        /// </summary>
        /// <param name="ID">Valor da Hash</param>
        /// <returns></returns>
        public Uri CreateLink(string UrlPattern, int ID) => new Uri(UrlPattern.BlankCoalesce(urlPattern).Inject(new { seed = Seed, id = ID, hash = Encode(ID) }).NullIf(x => x.IsNotValid()));

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

            string s = Util.EmptyString;
            while (i > 0)
            {
                s += Convert.ToString(Token[i % Token.Length]);
                i /= Token.Length;
            }

            while (s.Length < 4)
            {
                s += "0";
            }

            return string.Join(Util.EmptyString, s.Reverse());
        }

        public string RandomHash() => Encode(Util.RandomInt());

        public string RandomHash(int Min, int Max) => Encode(Util.RandomInt(Min, Max));

        public override string ToString() => Token;

        #endregion Public Methods
    }

}