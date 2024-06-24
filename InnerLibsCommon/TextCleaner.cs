
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Extensions;

namespace Extensions.ComplexText
{


    public class Paragraph : List<Sentence>
    {
        #region Internal Constructors

        internal Paragraph(string Text, StructuredText StructuredText)
        {
            this.StructuredText = StructuredText;
            if (Text.IsValid())
            {
                char sep0 = '.';
                char sep1 = '!';
                char sep2 = '?';
                string pattern = string.Format("[{0}{1}{2}]|[^{0}{1}{2}]+", sep0, sep1, sep2);
                var regex = new Regex(pattern);
                var matches = regex.Matches(Text);
                foreach (Match match in matches)
                {
                    if (match.ToString().IsValid())
                        Add(new Sentence(match.ToString(), this));
                }
            }
        }

        #endregion Internal Constructors

        #region Public Properties

        public StructuredText StructuredText { get; set; }

        public int WordCount => Words.Count();

        public IEnumerable<string> Words => this.SelectMany(x => x.Words);

        #endregion Public Properties

        #region Public Methods

        public static implicit operator string(Paragraph paragraph) => paragraph.ToString();

        public override string ToString() => ToString(0);

        public string ToString(int Ident) => this.SelectJoinString(Util.WhitespaceChar).Prepend(Util.WhitespaceChar.Repeat(Ident));

        #endregion Public Methods
    }

    /// <summary>
    /// Sentença de um Texto (uma frase ou oração)
    /// </summary>
    public class Sentence : List<SentencePart>
    {
        #region Internal Constructors

        internal Sentence(string Text, Paragraph Paragraph)
        {
            this.Paragraph = Paragraph;
            if (Text.IsValid())
            {
                var charlist = Text.Trim().ToArray().ToList();
                string palavra = Util.EmptyString;
                var listabase = new List<string>();

                // remove quaisquer caracteres nao desejados do inicio da frase
                while (charlist.Count > 0 && charlist.First().ToString().IsIn(PredefinedArrays.EndOfSentencePunctuation))
                {
                    charlist.Remove(charlist.First());
                }

                // processa caractere a caractere
                foreach (var p in charlist)
                {
                    switch (true)
                    {
                        // caso for algum tipo de pontuacao, wrapper ou virgula
                        case object _ when PredefinedArrays.OpenWrappers.Contains(Convert.ToString(p)):
                        case object _ when PredefinedArrays.CloseWrappers.Contains(Convert.ToString(p)):
                        case object _ when PredefinedArrays.EndOfSentencePunctuation.Contains(Convert.ToString(p)):
                        case object _ when PredefinedArrays.MidSentencePunctuation.Contains(Convert.ToString(p)):
                            {
                                if (palavra.IsValid())
                                {
                                    listabase.Add(palavra); // adiciona a plavra atual
                                    palavra = Util.EmptyString;
                                }

                                listabase.Add(Convert.ToString(p)); // adiciona a virgula, wrapper ou pontuacao
                                break;
                            }
                        // caso for espaco
                        case object _ when Convert.ToString(p) == Util.WhitespaceChar:
                            {
                                if (palavra.IsValid())
                                {
                                    listabase.Add(palavra); // adiciona a plavra atual                                   
                                }

                                palavra = Util.EmptyString;
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
                if (palavra.IsValid())
                {
                    listabase.Add(palavra);
                }

                if (listabase.Count > 0)
                {
                    if (listabase.Last() == ",") // se a ultima sentenca for uma virgula, substituimos ela por ponto
                    {
                        listabase.RemoveAt(listabase.Count - 1);
                        listabase.Add(".");
                    }

                    // se a ultima senteca nao for nenhum tipo de pontuacao, adicionamos um ponto a ela
                    if (!listabase.Last().IsInAny(new[] { PredefinedArrays.EndOfSentencePunctuation, PredefinedArrays.MidSentencePunctuation }))
                    {
                        listabase.Add(".");
                    }

                    // processar palavra a palavra
                    for (int index = 0, loopTo = listabase.Count - 1; index <= loopTo; index++)
                    {
                        if (listabase[index].IsValid())
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

        #endregion Internal Constructors

        #region Public Properties

        public Paragraph Paragraph { get; private set; }

        public int WordCount => Words.Count();

        public IEnumerable<string> Words => this.Where(x => x.IsWord).Select(x => x.Text);

        #endregion Public Properties

        #region Public Methods

        public static implicit operator string(Sentence sentence) => sentence.ToString();

        public override string ToString()
        {
            string sent = Util.EmptyString;
            foreach (var s in this)
            {
                if (s.IsClosingQuote)
                {
                    sent += s.GetMatchQuote().ToString();
                }
                else
                {
                    sent += s.ToString();
                }

                if (s.NeedSpaceOnNext)
                {
                    sent += Util.WhitespaceChar;
                }
            }

            return sent.Trim();
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Parte de uma sentença. Pode ser uma palavra, pontuaçao ou qualquer caractere de encapsulamento
    /// </summary>
    public class SentencePart
    {
        #region Internal Constructors

        internal SentencePart(string Text, Sentence Sentence)
        {
            this.Text = Text.Trim();
            this.Sentence = Sentence;
        }

        #endregion Internal Constructors

        #region Public Properties

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de fechamento de encapsulamento
        /// </summary>
        /// <returns></returns>
        public bool IsCloseWrapChar => PredefinedArrays.CloseWrappers.Contains(Text) && !IsOpeningQuote;

        public bool IsClosingQuote => IsQuote && Sentence.Where(x => x.IsQuote).GetIndexOf(this).IsOdd();

        /// <summary>
        /// Retorna TRUE se esta parte de sentença é uma vírgula
        /// </summary>
        /// <returns></returns>
        public bool IsComma => Text == ",";

        public bool IsDoubleQuote => Text == Util.DoubleQuoteChar;

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de encerramento de frase (pontuaçao)
        /// </summary>
        /// <returns></returns>
        public bool IsEndOfSentencePunctuation => PredefinedArrays.EndOfSentencePunctuation.Contains(Text);

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de de meio de sentença (dois
        /// pontos ou ponto e vírgula)
        /// </summary>
        /// <returns></returns>
        public bool IsMidSentencePunctuation => PredefinedArrays.MidSentencePunctuation.Contains(Text);

        /// <summary>
        /// Retorna TRUE se esta parte de senteça não for uma palavra
        /// </summary>
        /// <returns></returns>
        public bool IsNotWord => IsOpenWrapChar || IsCloseWrapChar || IsComma || IsEndOfSentencePunctuation || IsMidSentencePunctuation || IsQuote;

        public bool IsOpeningQuote => IsQuote && Sentence.Where(x => x.IsQuote).GetIndexOf(this).IsEven();

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for um caractere de abertura de encapsulamento
        /// </summary>
        /// <returns></returns>
        public bool IsOpenWrapChar => PredefinedArrays.OpenWrappers.Contains(Text) && !IsClosingQuote;

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for qualquer tipo de pontuaçao
        /// </summary>
        /// <returns></returns>
        public bool IsPunctuation => IsEndOfSentencePunctuation || IsMidSentencePunctuation;

        public bool IsQuote => IsSingleQuote || IsDoubleQuote;
        public bool IsSingleQuote => Text == Util.SingleQuoteChar;

        /// <summary>
        /// Retorna TRUE se esta parte de senteça for uma palavra
        /// </summary>
        /// <returns></returns>
        public bool IsWord => !IsNotWord;

        /// <summary>
        /// Retorna true se é nescessário espaço andes da proxima sentença
        /// </summary>
        /// <returns></returns>
        public bool NeedSpaceOnNext => GetNextPart() != null && !IsOpeningQuote && !IsOpenWrapChar && !GetNextPart().IsClosingQuote && (IsClosingQuote || IsCloseWrapChar || GetNextPart().IsWord || GetNextPart().IsOpenWrapChar);

        public Sentence Sentence { get; private set; }

        /// <summary>
        /// Texto desta parte de sentença
        /// </summary>
        /// <returns></returns>
        public string Text { get; set; }

        #endregion Public Properties

        #region Public Methods

        public static implicit operator string(SentencePart sentencePart) => sentencePart.ToString();

        public SentencePart GetMatchQuote()
        {
            var quotes = Sentence.Where(x => x.IsQuote).ToList();

            if (IsOpeningQuote)
            {
                return quotes.IfNoIndex(quotes.GetIndexOf(this) + 1);
            }

            if (IsClosingQuote)
            {
                return quotes.IfNoIndex(quotes.GetIndexOf(this) - 1);
            }

            return null;
        }

        /// <summary>
        /// Parte da próxima sentença
        /// </summary>
        /// <returns></returns>
        public SentencePart GetNextPart() => Sentence.IfNoIndex(Sentence.IndexOf(this) + 1);

        /// <summary>
        /// Parte de sentença anterior
        /// </summary>
        /// <returns></returns>
        public SentencePart GetPreviousPart() => Sentence.IfNoIndex(Sentence.IndexOf(this) - 1);

        public override string ToString()
        {
            int indexo = Sentence.IndexOf(this);
            if (indexo < 0)
            {
                return Util.EmptyString;
            }

            if (indexo == 0 || indexo == 1 && PredefinedArrays.OpenWrappers.Contains(Sentence[0].Text))
            {
                return Text.ToProperCase();
            }

            return Text;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Texto estruturado (Dividido em parágrafos)
    /// </summary>
    public class StructuredText : List<Paragraph>
    {
        #region Private Fields

        private string _originalText = Util.EmptyString;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Cria um novo Texto estruturado (dividido em paragrafos, sentenças e palavras)
        /// </summary>
        /// <param name="OriginalText"></param>
        public StructuredText(string OriginalText)
        {
            Text = OriginalText;
        }

        #endregion Public Constructors

        #region Public Properties

        public int BreakLinesBetweenParagraph { get; set; } = 0;
        public int Ident { get; set; } = 0;

        public string OriginalText => _originalText;

        public string Text
        {
            get => ToString();

            set
            {
                _originalText = value;
                Clear();
                if (OriginalText.IsValid())
                {
                    foreach (var p in OriginalText.Split(PredefinedArrays.BreakLineChars.ToArray(), StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (p.IsValid())
                        {
                            Add(new Paragraph(p, this));
                        }
                    }
                }
            }
        }

        public int WordCount => Words.Count();

        public IEnumerable<string> Words => this.SelectMany(x => x.Words);

        #endregion Public Properties

        #region Public Methods

        public static implicit operator int(StructuredText s) => s.Count;

        public static implicit operator long(StructuredText s) => s.Count;

        public static implicit operator string(StructuredText s) => s.ToString();

        public static StructuredText operator +(StructuredText a, StructuredText b) => new StructuredText($"{a}{Environment.NewLine}{b}");

        public Paragraph GetParagraph(int Index) => this.IfNoIndex(Index, null);

        public Sentence GetSentence(int Index) => GetSentences().IfNoIndex(Index, null);

        public IEnumerable<Sentence> GetSentences() => this.SelectMany(x => x.AsEnumerable());

        /// <summary>
        /// Retorna o Texto corretamente formatado
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.SelectJoinString(parag => parag.ToString(Ident), Enumerable.Range(1, 1 + BreakLinesBetweenParagraph.SetMinValue(0)).SelectJoinString(x => Environment.NewLine));

        #endregion Public Methods
    }

}