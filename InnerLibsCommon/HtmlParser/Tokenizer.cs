using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions.Web
{
    public class Tokenizer
    {
        public class Token
        {
            public string Filter { get; set; }
            public IList<Token> SubTokens { get; set; }

            public Token(string word)
            {
                if (string.IsNullOrEmpty(word))
                    throw new ArgumentNullException("word");

                var tokens = SplitTokens(word).ToList();

                this.Filter = tokens.First();
                this.SubTokens = tokens.Skip(1).Select(i => new Token(i)).ToList();
            }

            private static IList<string> SplitTokens(string token)
            {
                Func<char, bool> isNameToken = (c) => char.IsLetterOrDigit(c) || c == '-' || c == '_';
                var rt = new List<string>();

                int start = 0;
                bool isPrefix = true;
                bool isOpeningBracket = false;
                char closeBracket = '\0';
                for (int i = 0; i < token.Length; i++)
                {
                    if (isOpeningBracket && token[i] != closeBracket)
                        continue;

                    isOpeningBracket = false;

                    if (token[i] == '(')
                    {
                        closeBracket = ')';
                        isOpeningBracket = true;
                    }
                    else if (token[i] == '[')
                    {
                        closeBracket = ']';
                        if (i != start)
                        {
                            rt.Add(token.Substring(start, i - start));
                            start = i;
                        }
                        isOpeningBracket = true;
                    }
                    else if (i == token.Length - 1)
                    {
                        rt.Add(token.Substring(start, i - start + 1));
                    }
                    else if (!isNameToken(token[i]) && !isPrefix)
                    {
                        rt.Add(token.Substring(start, i - start));
                        start = i;
                    }
                    else if (isNameToken(token[i]))
                        isPrefix = false;
                }

                return rt;
            }
        }
        public static IEnumerable<Token> GetTokens(string cssFilter)
        {
            var reader = new System.IO.StringReader(cssFilter);
            while (true)
            {
                int v = reader.Read();

                if (v < 0)
                    yield break;

                char c = (char)v;

                if (c == '>')
                {
                    yield return new Token(">");
                    continue;
                }

                if (c == ' ' || c == '\t')
                    continue;

                string word = c + ReadWord(reader);
                yield return new Token(word);
            }
        }

        private static string ReadWord(System.IO.StringReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (true)
            {
                int v = reader.Read();

                if (v < 0)
                    break;

                char c = (char)v;

                if (c == ' ' || c == '\t')
                    break;

                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
