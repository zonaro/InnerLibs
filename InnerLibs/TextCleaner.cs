using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InnerLibs.LINQ;


namespace InnerLibs
{
    public class Paragraph : List<Sentence>
    {
        internal Paragraph(string Text, StructuredText StructuredText)
        {
            this.StructuredText = StructuredText;
            if (Text.IsNotBlank())
            {
                char sep0 = '.';
                char sep1 = '!';
                char sep2 = '?';
                string pattern = string.Format("[{0}{1}{2}]|[^{0}{1}{2}]+", sep0, sep1, sep2);
                var regex = new Regex(pattern);
                var matches = regex.Matches(Text);
                foreach (Match match in matches)
                    Add(new Sentence(match.ToString(), this));
            }
        }


        public static implicit operator string(Paragraph paragraph) => paragraph.ToString();


        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int Ident)
        {
            string ss = "";
            foreach (var s in this)
                ss += s.ToString() + " ";
            ss = ss.AdjustBlankSpaces();
            return ss.PadLeft(ss.Length + Ident);
        }

        public StructuredText StructuredText { get; set; }

        public IEnumerable<string> Words
        {
            get
            {
                return this.SelectMany(x => x.Words);
            }
        }

        public int WordCount
        {
            get
            {
                return Words.Count();
            }
        }
    }

    /// <summary>
    /// Sentença de um texto (uma frase ou oração)
    /// </summary>
    public class Sentence : List<SentencePart>
    {
        public IEnumerable<string> Words
        {
            get
            {
                return this.Where(x => x.IsWord()).Select(x => x.Text);
            }
        }

        public int WordCount
        {
            get
            {
                return Words.Count();
            }
        }

        internal Sentence(string Text, Paragraph Paragraph)
        {
            this.Paragraph = Paragraph;
            if (Text.IsNotBlank())
            {
                var charlist = Text.Trim().ToArray().ToList();
                string palavra = "";
                var listabase = new List<string>();

                // remove quaisquer caracteres nao desejados do inicio da frase
                while (charlist.Count > 0 && charlist.First().ToString().IsIn(Arrays.EndOfSentencePunctuation))
                    charlist.Remove(charlist.First());

                // processa caractere a caractere
                foreach (var p in charlist)
                {
                    switch (true)
                    {
                        // caso for algum tipo de pontuacao, wrapper ou virgula
                        case object _ when Arrays.OpenWrappers.Contains(Convert.ToString(p)):
                        case object _ when Arrays.CloseWrappers.Contains(Convert.ToString(p)):
                        case object _ when Arrays.EndOfSentencePunctuation.Contains(Convert.ToString(p)):
                        case object _ when Arrays.MidSentencePunctuation.Contains(Convert.ToString(p)):
                            {
                                if (palavra.IsNotBlank())
                                {
                                    listabase.Add(palavra); // adiciona a plavra atual
                                    palavra = "";
                                }

                                listabase.Add(Convert.ToString(p)); // adiciona a virgula, wrapper ou pontuacao
                                break;
                            }
                        // caso for espaco
                        case object _ when Convert.ToString(p) == " ":
                            {
                                if (palavra.IsNotBlank())
                                {
                                    listabase.Add(palavra); // adiciona a plavra atual
                                    palavra = "";
                                }
                                // senao, adiciona o proximo caractere a palavra atual
                                palavra = "";
                                break;
                            }

                        default:
                            {
                                palavra += Convert.ToString(p);
                                break;
                            }
                    }
                }

                // e entao adiciona ultima palavra se existir
                if (palavra.IsNotBlank())
                {
                    listabase.Add(palavra);
                    palavra = "";
                }

                if (listabase.Count > 0)
                {
                    if (listabase.Last() == ",") // se a ultima sentenca for uma virgula, substituimos ela por ponto
                    {
                        listabase.RemoveAt(listabase.Count - 1);
                        listabase.Add(".");
                    }

                    // se a ultima sentecao nao for nenhum tipo de pontuacao, adicionamos um ponto a ela
                    if (!listabase.Last().IsInAny(new[] { Arrays.EndOfSentencePunctuation, Arrays.MidSentencePunctuation }))
                    {
                        listabase.Add(".");
                    }

                    // processar palavra a palavra
                    for (int index = 0, loopTo = listabase.Count - 1; index <= loopTo; index++)
                    {
                        if (listabase[index].IsNotBlank())
                        {
                            Add(new SentencePart(listabase[index], this));
                        }
                    }
                }
                else
                {
                    this.Paragraph.Remove(this);
                }
            }
        }

        public override string ToString()
        {
            string sent = "";
            foreach (var s in this)
            {
                sent += s.ToString();
                if (s.Next() != null)
                {
                    if (s.NeedSpaceOnNext())
                    {
                        sent += " ";
                    }
                }
            }

            return sent;
        }

        public static implicit operator string(Sentence sentence) => sentence.ToString();

        public Paragraph Paragraph { get; private set; }
    }

    /// <summary>
    /// Parte de uma sentença. Pode ser uma palavra, pontuaçao ou qualquer caractere de encapsulamento
    /// </summary>
    public class SentencePart
    {
        /// <summary>
        /// Retorna TRUE se esta parte de senteça for uma palavra
        /// </summary>
        /// <returns></returns>
        public bool IsWord()
        {
            return !IsNotWord();
        }

        /// <summary>
        /// Retorna TRUE se esta parte de senteça não for uma palavra
        /// </summary>
        /// <returns></returns>
        public bool IsNotWord()
        {
            return IsOpenWrapChar() || IsCloseWrapChar() || IsComma() || IsEndOfSentencePunctuation() || IsMidSentencePunctuation();
        }

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de abertura de encapsulamento
        /// </summary>
        /// <returns></returns>
        public bool IsOpenWrapChar()
        {
            return Arrays.OpenWrappers.Contains(Text);
        }

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de fechamento de encapsulamento
        /// </summary>
        /// <returns></returns>
        public bool IsCloseWrapChar()
        {
            return Arrays.CloseWrappers.Contains(Text);
        }

        /// <summary>
        /// Retorna TRUE se esta parte de sentença é uma vírgula
        /// </summary>
        /// <returns></returns>
        public bool IsComma()
        {
            return Text == ",";
        }

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de encerramento de frase (pontuaçao)
        /// </summary>
        /// <returns></returns>
        public bool IsEndOfSentencePunctuation()
        {
            return Arrays.EndOfSentencePunctuation.Contains(Text);
        }

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de de meio de sentença (dois pontos ou ponto e vírgula)
        /// </summary>
        /// <returns></returns>
        public bool IsMidSentencePunctuation()
        {
            return Arrays.MidSentencePunctuation.Contains(Text);
        }

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for qualquer tipo de pontuaçao
        /// </summary>
        /// <returns></returns>
        public bool IsPunctuation()
        {
            return IsEndOfSentencePunctuation() | IsMidSentencePunctuation();
        }

        internal SentencePart(string Text, Sentence Sentence)
        {
            this.Text = Text.Trim();
            this.Sentence = Sentence;
        }

        public Sentence Sentence { get; private set; }

        /// <summary>
        /// Texto desta parte de sentença
        /// </summary>
        /// <returns></returns>
        public string Text { get; set; }

        public static implicit operator string(SentencePart sentencePart) => sentencePart.ToString();

        public override string ToString()
        {
            int indexo = Sentence.IndexOf(this);
            if (indexo < 0)
            {
                return "";
            }

            if (indexo == 0 || indexo == 1 && Arrays.OpenWrappers.Contains(Sentence[0].Text))
            {
                return Text.ToProperCase();
            }

            return Text;
        }

        /// <summary>
        /// Parte de sentença anterior
        /// </summary>
        /// <returns></returns>
        public SentencePart Previous()
        {
            return Sentence.IfNoIndex(Sentence.IndexOf(this) - 1);
        }

        /// <summary>
        /// Parte da próxima sentença
        /// </summary>
        /// <returns></returns>
        public SentencePart Next()
        {
            return Sentence.IfNoIndex(Sentence.IndexOf(this) + 1);
        }

        /// <summary>
        /// Retorna true se é nescessário espaço andes da proxima sentença
        /// </summary>
        /// <returns></returns>
        public bool NeedSpaceOnNext()
        {
            return Next() != null && (Next().IsWord() || Next().IsOpenWrapChar());
        }
    }

    /// <summary>
    /// Texto estruturado (Dividido em parágrafos)
    /// </summary>
    public class StructuredText : List<Paragraph>
    {
        public int Ident { get; set; } = 0;
        public int BreakLinesBetweenParagraph { get; set; } = 0;

        public string OriginalText
        {
            get
            {
                return _originaltext;
            }
        }

        public string Text
        {
            get
            {
                return ToString();
            }

            set
            {
                _originaltext = value;
                Clear();
                if (OriginalText.IsNotBlank())
                {
                    foreach (var p in OriginalText.Split(Arrays.BreakLineChars.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (p.IsNotBlank())
                        {
                            Add(new Paragraph(p, this));
                        }
                    }
                }
            }
        }

        private string _originaltext = "";

        /// <summary>
        /// Retorna o texto corretamente formatado
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.SelectJoinString(parag => parag.ToString(Ident), Enumerable.Range(1, 1 + BreakLinesBetweenParagraph.SetMinValue(0)).SelectJoinString(x => Environment.NewLine));
        }


        public Paragraph GetParagraph(int Index) => this.IfNoIndex(Index, null);
        public IEnumerable<Sentence> GetSentences() => this.SelectMany(x => x.AsEnumerable());

        public Sentence GetSentence(int Index) => GetSentences().IfNoIndex(Index, null);


        /// <summary>
        /// Cria um novo texto estruturado (dividido em paragrafos, sentenças e palavras)
        /// </summary>
        /// <param name="OriginalText"></param>
        public StructuredText(string OriginalText)
        {
            Text = OriginalText;
        }

        public static implicit operator string(StructuredText s)
        {
            return s.ToString();
        }

        public static implicit operator int(StructuredText s)
        {
            return s.Count;
        }

        public static implicit operator long(StructuredText s)
        {
            return s.LongCount();
        }

        public static StructuredText operator +(StructuredText a, StructuredText b)
        {
            return new StructuredText(a.ToString() + b.ToString());
        }

        public IEnumerable<string> Words
        {
            get
            {
                return this.SelectMany(x => x.Words);
            }
        }

        public int WordCount
        {
            get
            {
                return Words.Count();
            }
        }
    }
}