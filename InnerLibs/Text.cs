using InnerLibs.LINQ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace InnerLibs
{
    /// <summary>
    /// Modulo de manipulação de Texto
    /// </summary>
    /// <remarks></remarks>
    public static class Text
    {
        public static IEnumerable<string> TrimBetween(this IEnumerable<string> Texts) => Texts.TrimBetween();

        /// <summary>
        /// Arruma a ortografia do texto captalizando corretamente, adcionando pontuação ao final de
        /// frase caso nescessário e removendo espaços excessivos ou incorretos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string FixText(this string Text, int Ident = 0, int BreakLinesBetweenParagraph = 0)
        {
            var removedot = !Text.Trim().EndsWith(".");
            var addComma = Text.Trim().EndsWith(",");
            Text = new TextStructure(Text) { Ident = Ident, BreakLinesBetweenParagraph = BreakLinesBetweenParagraph }.ToString();
            if (removedot)
            {
                Text = Text.TrimEnd().RemoveLastAny(".");
            }
            if (addComma)
            {
                Text = Text.TrimEnd().RemoveLastAny(".").Append(",");
            }
            return Text.Trim();
        }

        public static string TrimBetween(this string Text)
        {
            Text = Text.IfBlank("");
            if (Text.IsNotBlank())
            {
                var arr = Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Text = arr.JoinString(Environment.NewLine);
                arr = Text.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                Text = arr.JoinString(" ");
            }

            return Text.TrimAny(" ", Environment.NewLine);
        }


        /// <summary>
        /// Retorna uma string em ordem afabética baseada em uma outra string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Alphabetize(this string Text)
        {
            var a = Text.ToCharArray();
            Array.Sort(a);
            return a.JoinString("");
        }

        /// <summary>
        /// Adiciona texto ao fim de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string Append(this string Text, string AppendText)
        {
            Text = Text ?? "";
            AppendText = AppendText ?? "";
            Text += AppendText;
            return Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendIf(this string Text, string AppendText, bool Test)
        {
            Text = Text ?? "";
            AppendText = AppendText ?? "";
            return Test ? Text.Append(AppendText) : Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendIf(this string Text, string AppendText, Func<string, bool> Test) => AppendIf(Text, AppendText, (Test ?? (x => false))(Text));

        /// <summary>
        /// Adiciona texto ao final de uma string com uma quebra de linha no final do <paramref name="AppendText"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string AppendLine(this string Text, string AppendText) => Text.Append(AppendText).Append(Environment.NewLine);

        public static string AppendUrlParameter(this string Url, string Key, params string[] Value)
        {
            foreach (var v in Value ?? Array.Empty<string>())
            {
                Url += string.Format("&{0}={1}", Key, v.UrlEncode().IfBlank(""));
            }

            return Url;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendWhile(this string Text, string AppendText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);
            while (Test(Text))
            {
                Text = Text.Append(AppendText);
            }

            return Text;
        }

        /// <summary>
        /// Aplica espacos em todos os caracteres de encapsulamento
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ApplySpaceOnWrapChars(this string Text)
        {
            foreach (var c in PredefinedArrays.WordWrappers)
            {
                Text = Text.Replace(c, " " + c + " ");
            }

            return Text;
        }

        /// <summary>
        /// Encapsula um texto em uma caixa
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxText(this string Text)
        {
            var Lines = Text.SplitAny(PredefinedArrays.BreakLineChars.ToArray()).ToList();
            string linha_longa = "";
            int charcount = Lines.Max(x => x.Length);
            if (charcount.IsEven())
            {
                charcount++;
            }

            for (int i = 0, loopTo = Lines.Count - 1; i <= loopTo; i++)
            {
                Lines[i] = Lines[i].PadRight(charcount);
            }

            for (int i = 0, loopTo1 = Lines.Count - 1; i <= loopTo1; i++)
            {
                Lines[i] = $"* {Lines[i]} *";
            }

            charcount = Lines.Max(x => x.Length);
            while (linha_longa.Length < charcount)
            {
                linha_longa += "* ";
            }

            linha_longa = linha_longa.Trim();
            Lines.Insert(0, linha_longa);
            Lines.Add(linha_longa);
            string box = Lines.JoinString(Environment.NewLine);
            return box;
        }

        /// <summary>
        /// Encapsula um texto em uma caixa incorporado em comentários CSS
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxTextCSS(this string Text) => $"/*{Text.BoxText().Wrap(Environment.NewLine)}*/";

        /// <summary>
        /// Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou
        /// colchetes) é um alias de <see cref="Quote(String, Char)"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BracketChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Brackfy(this string Text, char BracketChar = '{') => Text.Quote(BracketChar);

        /// <summary>
        /// Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro
        /// caractere desejado) e retorna um valor indicando se o texto precisou ser censurado
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BadWords">Lista de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        /// <returns>
        /// TRUE se a frase precisou ser censurada, FALSE se a frase não precisou de censura
        /// </returns>
        public static (string Text, bool IsCensored) Censor(this string Text, IEnumerable<string> BadWords, char CensorshipCharacter)
        {
            var words = Text.Split(" ", StringSplitOptions.None);
            BadWords = BadWords ?? Array.Empty<string>();
            var IsCensored = false;
            if (words.ContainsAny(BadWords))
            {
                foreach (var bad in BadWords)
                {
                    string censored = "";
                    for (int index = 1, loopTo = bad.Length; index <= loopTo; index++)
                    {
                        censored += CensorshipCharacter;
                    }

                    for (int index = 0, loopTo1 = words.Length - 1; index <= loopTo1; index++)
                    {
                        if ((words[index].RemoveDiacritics().RemoveAny(PredefinedArrays.WordSplitters.ToArray()).ToLower() ?? "") == (bad.RemoveDiacritics().RemoveAny(PredefinedArrays.WordSplitters.ToArray()).ToLower() ?? ""))
                        {
                            words[index] = words[index].ToLower().Replace(bad, censored);
                            IsCensored = true;
                        }
                    }
                }

                Text = words.JoinString(" ");
            }
            return (Text, IsCensored);
        }

        /// <summary>
        /// Retorna um novo texto censurando as palavras de um texto substituindo as palavras
        /// indesejadas por um caractere desejado)
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BadWords">Array de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        public static (string Text, bool IsCensored) Censor(this string Text, char CensorshipCharacter, params string[] BadWords) => Text.Censor((BadWords ?? Array.Empty<string>()).ToList(), CensorshipCharacter);

        /// <summary>
        /// Verifica se um texto contém outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool Contains(this string Text, string OtherText, StringComparison StringComparison) => Text.IndexOf(OtherText, StringComparison) > -1;

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsAll(this string Text, params string[] Values) => Text.ContainsAll(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <param name="ComparisonType">Tipo de comparacao</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAll(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            Values = Values ?? Array.Empty<string>();
            if (Values.Any())
            {
                foreach (string value in Values)
                {
                    if (Text == null || Text.IndexOf(value, ComparisonType) == -1)
                    {
                        return false;
                    }
                }

                return true;
            }

            return Text.IsBlank();
        }

        public static bool ContainsAllWords(this string Text, params string[] Words) => Text.ContainsAllWords(null, Words);

        public static bool ContainsAllWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words) => Text.GetWords().ContainsAll(Words, Comparer);

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAny(this string Text, params string[] Values) => Text.ContainsAny(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <param name="ComparisonType">Tipo de comparacao</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAny(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            Values = Values ?? Array.Empty<string>();
            if (Values.Any())
            {
                foreach (string value in Values ?? Array.Empty<string>())
                {
                    if (Text != null && Text.IndexOf(value, ComparisonType) != -1)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return Text.IsNotBlank();
            }
        }

        public static bool ContainsAnyWords(this string Text, params string[] Words) => Text.ContainsAnyWords(null, Words);

        public static bool ContainsAnyWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words) => Text.GetWords().ContainsAny(Words, Comparer);

        /// <summary>
        /// Verifica se uma string contém caracteres de digito
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsDigit(this string Text) => (Text ?? "").ToArray().Any(char.IsDigit);

        /// <summary>
        /// Verifica se uma string contém caracteres em minusculo
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsLower(this string Text) => (Text ?? "").ToArray().Any(char.IsLower);

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter a maioria dos valores, false se não</returns>
        public static bool ContainsMost(this string Text, StringComparison ComparisonType, params string[] Values) => (Values ?? Array.Empty<string>()).Most(value => Text != null && Text.Contains(value, ComparisonType));

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsMost(this string Text, params string[] Values) => Text.ContainsMost(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma string contém caracteres em maiúsculo
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsUpper(this string Text) => (Text ?? "").ToArray().Any(char.IsUpper);

        /// <summary>
        /// Conta os caracters especificos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Character">Caractere</param>
        /// <returns></returns>
        public static int CountCharacter(this string Text, char Character) => Text.Count((c) => c == Character);

        /// <summary>
        /// Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="RemoveDiacritics">indica se os acentos devem ser removidos das palavras</param>
        /// <param name="Words">
        /// Desconsidera outras palavras e busca a quantidade de cada palavra especificada em um array
        /// </param>
        /// <returns></returns>
        public static Dictionary<string, long> CountWords(this string Text, bool RemoveDiacritics = true, string[] Words = null)
        {
            if (Words == null)
            {
                Words = Array.Empty<string>();
            }

            var palavras = Text.Split(PredefinedArrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (Words.Any())
            {
                palavras = palavras.Where(x => Words.Select(y => y.ToLower()).Contains(x.ToLower())).ToArray();
            }

            if (RemoveDiacritics)
            {
                palavras = palavras.Select(p => p.RemoveDiacritics()).ToArray();
                Words = Words.Select(p => p.RemoveDiacritics()).ToArray();
            }

            var dic = palavras.DistinctCount();
            foreach (var w in Words.Where(x => !dic.Keys.Contains(x)))
            {
                dic.Add(w, 0L);
            }

            return dic;
        }

        /// <summary>
        /// Verifica se um texto contém outro ou vice versa
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool CrossContains(this string Text, string OtherText, StringComparison StringComparison = StringComparison.InvariantCultureIgnoreCase) => Text.Contains(OtherText, StringComparison) || OtherText.Contains(Text, StringComparison);

        /// <summary>
        /// Remove uma linha especifica de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="LineIndex">Numero da linha</param>
        /// <returns></returns>
        public static string DeleteLine(this string Text, int LineIndex)
        {
            LineIndex = LineIndex.SetMinValue(0);
            var parts = Text.Split(Environment.NewLine).ToList();

            if (parts.Count > LineIndex)
            {
                parts.RemoveAt(LineIndex);
            }

            return parts.JoinString(Environment.NewLine);
        }

        /// <summary>
        /// Cria um dicionário com as palavras de uma lista e a quantidade de cada uma.
        /// </summary>
        /// <param name="List">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(params string[] List) => List.ToList().DistinctCount();

        /// <summary>
        /// Cria um dicionário com as palavras de uma frase e sua respectiva quantidade.
        /// </summary>
        /// <param name="Text">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(this string Text) => Text.Split(" ").ToList().DistinctCount();

        /// <summary>
        /// Verifica se uma string termina com alguma outra string de um array
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string Text, StringComparison comparison, params string[] Words) => Words.Any(p => Text.EndsWith(p, comparison));

        public static bool EndsWithAny(this string Text, params string[] Words) => EndsWithAny(Text, default, Words);

        /// <summary>
        /// Prepara uma string com aspas simples para uma Query TransactSQL
        /// </summary>
        /// <param name="Text">Texto a ser tratado</param>
        /// <returns>String pronta para a query</returns>
        public static string EscapeQuotesToQuery(this string Text, bool AlsoQuoteText = false) => Text.Replace("'", "''").QuoteIf(AlsoQuoteText, '\'');

        /// <summary>
        /// Extrai emails de uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractEmails(this string Text) => Text.IfBlank("").SplitAny(PredefinedArrays.InvisibleChars.Union(PredefinedArrays.BreakLineChars).ToArray()).Where(x => x.IsEmail()).Select(x => x.ToLower()).Distinct().ToArray();

        /// <summary>
        /// Procura CEPs em uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static string[] FindByRegex(this string Text, string Regex, RegexOptions RegexOptions = RegexOptions.None)
        {
            var textos = new List<string>();
            foreach (Match m in new Regex(Regex, RegexOptions).Matches(Text))
            {
                textos.Add(m.Value);
            }

            return textos.ToArray();
        }

        /// <summary>
        /// Procura CEPs em uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static string[] FindCEP(this string Text) => Text.FindByRegex(@"\d{5}-\d{3}").Union(Text.FindNumbers().Where(x => x.Length == 8)).ToArray();

        /// <summary>
        /// Procura numeros em uma string e retorna um array deles
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindNumbers(this string Text)
        {
            var l = new List<string>();
            var numbers = Regex.Split(Text, @"\D+");
            foreach (var value in numbers)
            {
                if (!value.IsBlank())
                {
                    l.Add(value);
                }
            }

            return l;
        }

        /// <summary>
        /// Procurea numeros de telefone em um texto
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string[] FindTelephoneNumbers(this string Text) => Text.FindByRegex(@"\b[\s()\d-]{6,}\d\b", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Select(x => x.MaskTelephoneNumber()).ToArray();

        public static string FixCapitalization(this string Text)
        {
            Text = Text.Trim().GetFirstChars(1).ToUpper() + Text.RemoveFirstChars(1);
            var dots = new[] { "...", ". ", "? ", "! " };
            List<string> sentences;
            foreach (var dot in dots)
            {
                sentences = Text.Split(dot, StringSplitOptions.None).ToList();
                for (int index = 0, loopTo = sentences.Count - 1; index <= loopTo; index++)
                {
                    sentences[index] = "" + sentences[index].Trim().GetFirstChars(1).ToUpper() + sentences[index].RemoveFirstChars(1);
                }

                Text = sentences.JoinString(dot);
            }

            sentences = Text.Split(" ").ToList();
            Text = "";
            foreach (var c in sentences)
            {
                string palavra = c;
                if (palavra.EndsWith(".") && palavra.Length == 2)
                {
                    palavra = palavra.ToUpper();
                    Text += palavra;
                    string proximapalavra = sentences.IfNoIndex(sentences.IndexOf(c) + 1, "");
                    if (!(proximapalavra.EndsWith(".") && palavra.Length == 2))
                    {
                        Text += " ";
                    }
                }
                else
                {
                    Text += c + " ";
                }
            }

            return Text.RemoveLastChars(1);
        }

        /// <summary>
        /// Transforma quebras de linha HTML em quebras de linha comuns ao .net
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <returns>String fixada</returns>
        public static string FixHTMLBreakLines(this string Text)
        {
            Text = Text.ReplaceMany(Environment.NewLine, "<br/>", "<br />", "<br>");
            return Text.Replace("&nbsp;", " ");
        }

        /// <summary>
        /// Ajusta um caminho colocando as barras corretamente e substituindo caracteres inválidos
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string FixPath(this string Text, bool AlternativeChar = false)
        {
            return Text.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.IsNotBlank()).Select((x, i) =>
            {
                if (i == 0 && x.Length == 2 && x.EndsWith(":"))
                {
                    return x;
                }

                return x.ToFriendlyPathName();
            }).JoinString(AlternativeChar.AsIf(Path.AltDirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString())).RemoveLastAny(Path.DirectorySeparatorChar.ToString(), Path.AltDirectorySeparatorChar.ToString());
        }

        /// <summary>
        /// Adciona pontuação ao final de uma string se a mesma não terminar com alguma pontuacao.
        /// </summary>
        /// <param name="Text">Frase, Texto a ser pontuado</param>
        /// <param name="Punctuation">
        /// Ponto a ser adicionado na frase se a mesma não estiver com pontuacao
        /// </param>
        /// <returns>Frase corretamente pontuada</returns>
        public static string FixPunctuation(this string Text, string Punctuation = ".", bool ForceSpecificPunctuation = false)
        {
            Text = Text.RemoveLastAny(true, ",", " ");
            var pts = new[] { ".", "!", "?", ":", ";" };
            if (ForceSpecificPunctuation)
            {
                Text = Text.RemoveLastAny(true, pts).Trim() + Punctuation;
            }
            else if (!Text.EndsWithAny(pts))
            {
                Text += Punctuation;
            }

            return Text;
        }



        /// <summary>
        /// Executa uma ação para cada linha de um texto
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public static string ForEachLine(this string Text, Expression<Func<string, string>> Action)
        {
            if (Text.IsNotBlank() && Action != null)
            {
                Text = Text.SplitAny(PredefinedArrays.BreakLineChars.ToArray()).Select(x => Action.Compile().Invoke(x)).JoinString(Environment.NewLine);
            }

            return Text;
        }

        /// <summary>
        /// Formata um numero para CNPJ
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static string FormatCNPJ(this long CNPJ) => string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJ);

        /// <summary>
        /// Formata um numero para CNPJ
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static string FormatCNPJ(this string CNPJ)
        {
            if (CNPJ.IsValidCNPJ())
            {
                if (CNPJ.IsNumber())
                {
                    CNPJ = CNPJ.ToLong().FormatCNPJ();
                }
            }
            else
            {
                throw new FormatException("String is not a valid CNPJ");
            }

            return CNPJ;
        }

        /// <summary>
        /// Formata um numero para CPF
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public static string FormatCPF(this long CPF) => string.Format(@"{0:000\.000\.000\-00}", CPF);

        /// <summary>
        /// Formata um numero para CPF
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public static string FormatCPF(this string CPF)
        {
            if (CPF.IsValidCPF())
            {
                if (CPF.IsNumber())
                {
                    CPF = CPF.ToLong().FormatCPF();
                }
            }
            else
            {
                throw new FormatException("String is not a valid CPF");
            }

            return CPF;
        }

        /// <summary>
        /// Formata um numero para CNPJ ou CNPJ se forem validos
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string FormatCPFOrCNPJ(this long Document)
        {
            if (Document.ToString().IsValidCPF())
            {
                return Document.FormatCPF();
            }
            else if (Document.ToString().IsValidCNPJ())
            {
                return Document.FormatCNPJ();
            }
            else
            {
                return Document.ToString();
            }
        }

        /// <summary>
        /// Formata um numero para CNPJ ou CNPJ se forem validos
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string FormatCPFOrCNPJ(this string Document)
        {
            if (Document.IsValidCPF())
            {
                return Document.FormatCPF();
            }
            else if (Document.IsValidCNPJ())
            {
                return Document.FormatCNPJ();
            }
            else
            {
                return Document;
            }
        }

        /// <summary>
        /// Extension Method para <see cref="String.Format(String,Object())"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Args">Objetos de substituição</param>
        /// <returns></returns>
        public static string FormatString(this string Text, params string[] Args) => string.Format(Text, Args);

        /// <summary>
        /// Retorna um texto posterior a outro
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Value">Texto Posterior</param>
        /// <returns>Uma string com o valor posterior ao valor especificado.</returns>
        public static string GetAfter(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            if (string.IsNullOrEmpty(Value))
            {
                Value = "";
            }

            if (string.IsNullOrEmpty(Text) || Text.IndexOf(Value) == -1)
            {
                if (WhiteIfNotFound)
                {
                    return "";
                }
                else
                {
                    return $"{Text}";
                }
            }

            return Text.Substring(Text.IndexOf(Value) + Value.Length);
        }

        /// <summary>
        /// Retorna todas as ocorrencias de um texto entre dois textos
        /// </summary>
        /// <param name="Text">O texto correspondente</param>
        /// <param name="Before">O texto Anterior</param>
        /// <param name="After">O texto Posterior</param>
        /// <returns>Uma String com o texto entre o texto anterior e posterior</returns>
        public static string[] GetAllBetween(this string Text, string Before, string After = "")
        {
            var lista = new List<string>();
            string regx = Before.RegexEscape() + "(.*?)" + After.IfBlank(Before).RegexEscape();
            var mm = new Regex(regx, (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Matches(Text);
            foreach (Match a in mm)
            {
                lista.Add(a.Value.RemoveFirstEqual(Before).RemoveLastEqual(After));
            }

            return lista.ToArray();
        }

        /// <summary>
        /// Retorna um texto anterior a outro
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Value">Texto Anterior</param>
        /// <returns>Uma string com o valor anterior ao valor especificado.</returns>
        public static string GetBefore(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            if (Value == null)
            {
                Value = "";
            }

            if (Text == null || Text.IndexOf(Value) == -1)
            {
                if (WhiteIfNotFound)
                {
                    return "";
                }
                else
                {
                    return "" + Text;
                }
            }

            return Text.Substring(0, Text.IndexOf(Value));
        }

        /// <summary>
        /// Retorna o texto entre dois textos
        /// </summary>
        /// <param name="Text">O texto correspondente</param>
        /// <param name="Before">O texto Anterior</param>
        /// <param name="After">O texto Posterior</param>
        /// <returns>Uma String com o texto entre o texto anterior e posterior</returns>
        public static string GetBetween(this string Text, string Before, string After)
        {
            if (Text.IsBlank())
            {
                return "";
            }

            int beforeStartIndex = Text.IndexOf(Before);
            int startIndex = beforeStartIndex + Before.Length;
            int afterStartIndex = Text.IndexOf(After, startIndex);
            if (beforeStartIndex < 0 || afterStartIndex < 0)
            {
                return Text;
            }

            return Text.Substring(startIndex, afterStartIndex - startIndex);
        }

        /// <summary>
        /// Pega o dominio principal de uma URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomain(this Uri URL, bool RemoveFirstSubdomain = false)
        {
            string d = URL.GetLeftPart(UriPartial.Authority).RemoveAny("http://", "https://", "www.");
            if (RemoveFirstSubdomain)
            {
                var parts = d.Split(".").ToList();
                parts.Remove(parts[0]);
                d = parts.JoinString(".");
            }

            return d;
        }

        /// <summary>
        /// Pega o dominio principal de uma URL ou email
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomain(this string URL, bool RemoveFirstSubdomain = false)
        {
            if (URL.IsEmail())
            {
                URL = $"http://{URL.GetAfter("@")}";
            }

            return new Uri(URL).GetDomain(RemoveFirstSubdomain);
        }

        /// <summary>
        /// Pega o protocolo e o dominio principal de uma URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomainAndProtocol(this string URL) => $"{new Uri(URL).GetLeftPart(UriPartial.Authority)}";

        public static string GetFirstChars(this string Text, int Number = 1)
        {
            if (Text.IsNotBlank())
            {
                if (Text.Length < Number | Number < 1)
                {
                    return Text;
                }
                else
                {
                    return Text.Substring(0, Number);
                }
            }

            return "";
        }

        public static string GetLastChars(this string Text, int Number = 1)
        {
            if (Text.IsNotBlank())
            {
                if (Text.Length < Number | Number < 1)
                {
                    return Text;
                }
                else
                {
                    return Text.Substring(Text.Length - Number);
                }
            }

            return "";
        }

        /// <summary>
        /// Retorna N caracteres de uma string a partir do caractere encontrado no centro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetMiddleChars(this string Text, int Length)
        {
            Text = Text.IfBlank("");
            if (Text.Length >= Length)
            {
                if (Text.Length % 2 != 0)
                {
                    try
                    {
                        return Text.Substring((int)Math.Round(Text.Length / 2d - 1d), Length);
                    }
                    catch
                    {
                        return Text.GetMiddleChars(Length - 1);
                    }
                }
                else
                {
                    return Text.RemoveLastChars(1).GetMiddleChars(Length);
                }
            }

            return Text;
        }

        /// <summary>
        /// Retorna o caractere de encapsulamento oposto ao caractere indicado
        /// </summary>
        /// <param name="Text">Caractere</param>
        /// <returns></returns>
        public static string GetOppositeWrapChar(this string Text)
        {
            switch (Text.GetFirstChars() ?? "")
            {
                case "\"": return "\"";
                case "'": return "'";
                case "(": return ")";
                case ")": return "(";
                case "[": return "]";
                case "]": return "[";
                case "{": return "}";
                case "}": return "{";
                case "<": return ">";
                case ">": return "<";
                case @"\": return "/";
                case "/": return @"\";
                case "¿": return "?";
                case "?": return "¿";
                case "!": return "¡";
                case "¡": return "!";
                case ".": return ".";
                case ":": return ":";
                case ";": return ";";
                case "_": return "_";
                case "*": return "*";
                default: return Text;
            }
        }

        public static char GetOppositeWrapChar(this char Char) => $"{Char}".GetOppositeWrapChar().FirstOrDefault();

        /// <summary>
        /// Sorteia um item da Lista
        /// </summary>
        /// <typeparam name="Type">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static Type GetRandomItem<Type>(this Type[] Array) => Array[Generate.RandomNumber(0, Array.Count() - 1)];

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this Uri URL, bool WithQueryString = true) => WithQueryString ? URL.PathAndQuery : URL.AbsolutePath;

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this string URL, bool WithQueryString = true) => URL.IsURL() ? new Uri(URL).GetRelativeURL(WithQueryString) : null;

        /// <summary>
        /// Retorna uma lista de palavras encontradas no texto em ordem alfabetica
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<string> GetWords(this string Text)
        {
            var txt = new List<string>();
            var palavras = Text.TrimBetween().FixHTMLBreakLines().ToLower().RemoveHTML().Split(PredefinedArrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var w in palavras)
            {
                txt.Add(w);
            }

            return txt.Distinct().OrderBy(x => x);
        }

        /// <summary>
        /// Captura todas as sentenças que estão entre aspas ou parentesis ou chaves ou colchetes em
        /// um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string[] GetWrappedText(this string Text, string Character = "\"", bool ExcludeWrapChars = true)
        {
            var lista = new List<string>();
            string regx = Character.RegexEscape() + "(.*?)" + Character.ToString().GetOppositeWrapChar().RegexEscape();
            var mm = new Regex(regx, (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Matches(Text);
            foreach (Match a in mm)
            {
                if (ExcludeWrapChars)
                {
                    lista.Add(a.Value.RemoveFirstEqual(Character).RemoveLastEqual(Character.GetOppositeWrapChar()));
                }
                else
                {
                    lista.Add(a.Value);
                }
            }

            return lista.ToArray();
        }

        /// <summary>
        /// Retorna um texto com entidades HTML convertidas para caracteres e tags BR em breaklines
        /// </summary>
        /// <param name="Text">string HTML</param>
        /// <returns>String HTML corrigido</returns>
        public static string HtmlDecode(this string Text) => System.Net.WebUtility.HtmlDecode("" + Text).ReplaceMany(Environment.NewLine, "<br/>", "<br />", "<br>");

        /// <summary>
        /// Escapa o texto HTML
        /// </summary>
        /// <param name="Text">string HTML</param>
        /// <returns>String HTML corrigido</returns>
        public static string HtmlEncode(this string Text) => System.Net.WebUtility.HtmlEncode("" + Text.ReplaceMany("<br>", PredefinedArrays.BreakLineChars.ToArray()));

        /// <summary>
        /// Inject the property values of <typeparamref name="T"/> into <paramref name="TemplatedString"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatString"></param>
        /// <param name="injectionObject"></param>
        /// <returns></returns>
        public static string Inject<T>(this T Obj, string TemplatedString, bool IsSQL = false) => TemplatedString.IfBlank("").Inject(Obj, IsSQL);

        /// <summary>
        /// Inject the property values of <typeparamref name="T"/> into <see cref="String"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatString"></param>
        /// <param name="injectionObject"></param>
        /// <returns></returns>
        public static string Inject<T>(this string formatString, T injectionObject, bool IsSQL = false)
        {
            if (injectionObject != null)
            {
                return injectionObject.IsDictionary()
                    ? formatString.Inject(new Hashtable((IDictionary)injectionObject), IsSQL)
                    : formatString.Inject(Misc.GetPropertyHash(injectionObject), IsSQL);
            }

            return formatString;
        }

        /// <summary>
        /// Inject a <see cref="Hashtable"/> into <see cref="String"/>
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static string Inject(this string formatString, Hashtable attributes, bool IsSQL = false)
        {
            string result = formatString;
            if (attributes != null && formatString != null)
            {
                foreach (string attributeKey in attributes.Keys)
                {
                    result = result.InjectSingleValue(attributeKey, attributes[attributeKey], IsSQL);
                }
            }

            return result;
        }

        /// <summary>
        /// Replace te found <paramref name="key"/> with <paramref name="replacementValue"/>
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="key"></param>
        /// <param name="replacementValue"></param>
        /// <returns></returns>
        public static string InjectSingleValue(this string formatString, string key, object replacementValue, bool IsSQL = false, CultureInfo cultureInfo = null)
        {
            string result = formatString;
            var attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");
            foreach (Match m in attributeRegex.Matches(formatString))
            {
                string replacement = m.ToString();
                if (m.Groups[2].Length > 0)
                {
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
                    replacement = string.Format(cultureInfo ?? CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
                }
                else
                {
                    replacement = (replacementValue ?? default).ToString();
                }

                if (IsSQL)
                {
                    replacement = MicroORM.DbExtensions.ToSQLString(replacement);
                }

                result = result.Replace(m.ToString(), replacement);
            }

            return result;
        }

        public static string Interpolate(this string Text, params string[] Texts)
        {
            Text = Text.IfBlank("");
            Texts = Texts ?? Array.Empty<string>();

            var s = Texts.ToList();
            s.Insert(0, Text);

            var ns = @"";
            var len = s.Max(x => x.Length);
            for (int i = 0; i < len; i++)
            {
                foreach (var item in s)
                {
                    ns += item.AsEnumerable().IfNoIndex(i, ' ');
                }
            }

            return ns;
        }

        /// <summary>
        /// Verifica se uma palavra é um Anagrama de outra palavra
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="AnotherText"></param>
        /// <returns></returns>
        public static bool IsAnagramOf(this string Text, string AnotherText)
        {
            var char1 = Text.ToLower().ToCharArray();
            var char2 = AnotherText.ToLower().ToCharArray();
            Array.Sort(char1);
            Array.Sort(char2);
            string NewWord1 = new string(char1);
            string NewWord2 = new string(char2);
            return (NewWord1 ?? "") == (NewWord2 ?? "");
        }

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, params string[] Texts) => Text.IsAny(StringComparison.CurrentCultureIgnoreCase, Texts);

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, StringComparison Comparison, params string[] Texts) => (Texts ?? Array.Empty<string>()).Any(x => Text.Equals(x, Comparison));

        public static bool IsCloseWrapChar(this string Text) => Text.GetFirstChars().IsIn(PredefinedArrays.CloseWrappers);

        public static bool IsCloseWrapChar(this char Char) => IsCloseWrapChar($"{Char}");

        public static bool IsCrossLikeAny(this string Text, IEnumerable<string> Patterns) => (Patterns ?? Array.Empty<string>()).Any(x => Text.IfBlank("").Like(x) || x.Like(Text));

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, IEnumerable<string> Patterns) => (Patterns ?? Array.Empty<string>()).Any(x => Text.IfBlank("").Like(x));

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, params string[] Patterns) => Text.IsLikeAny((Patterns ?? Array.Empty<string>()).AsEnumerable());

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se nenhuma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, params string[] Texts) => !Text.IsAny(Texts);

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, StringComparison Comparison, params string[] Texts) => !Text.IsAny(Comparison, Texts);

        /// <summary>
        /// Retorna o caractere de encapsulamento oposto ao caractere indicado
        /// </summary>
        /// <param name="Text">Caractere</param>
        /// <returns></returns>
        public static bool IsOpenWrapChar(this string Text) => Text.GetFirstChars().IsIn(PredefinedArrays.OpenWrappers);

        public static bool IsOpenWrapChar(this char Char) => IsOpenWrapChar($"{Char}");

        /// <summary>
        /// Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da
        /// esqueda para direita
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="IgnoreWhiteSpaces">Ignora os espaços na hora de comparar</param>
        /// <returns></returns>
        public static bool IsPalindrome(this string Text, bool IgnoreWhiteSpaces = true)
        {
            if (IgnoreWhiteSpaces)
            {
                Text = Text.RemoveAny(" ");
            }

            return Text == Text.ToCharArray().Reverse().JoinString();
        }

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Items">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(this IEnumerable<Type> Items, string Separator = "")
        {
            Items = Items ?? Array.Empty<Type>().AsEnumerable();
            return string.Join(Separator, Items.Select(x => $"{x}").ToArray());
        }

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Array">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(this Type[] Array, string Separator = "") => JoinString(Array.AsEnumerable(), Separator);

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Array">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(string Separator, params Type[] Array) => JoinString(Array, Separator);

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="List">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(this List<Type> List, string Separator = "") => List.ToArray().JoinString(Separator);

        /// <summary>
        /// Computa a distancia de Levenshtein entre 2 strings. Distancia Levenshtein representa um
        /// numero de operações de acréscimo, remoção ou substituição de caracteres para que uma
        /// string se torne outra
        /// </summary>
        public static int LevenshteinDistance(this string Text1, string Text2)
        {
            Text1 = Text1 ?? "";
            Text2 = Text2 ?? "";
            int n = Text1.Length;
            int m = Text2.Length;
            var d = new int[n + 1 + 1, m + 1 + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0, loopTo = n; i <= loopTo; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0, loopTo1 = m; j <= loopTo1; j++)
            {
                d[0, j] = j;
            }

            // Step 3
            for (int i = 1, loopTo2 = n; i <= loopTo2; i++)
            {
                // Step 4
                for (int j = 1, loopTo3 = m; j <= loopTo3; j++)
                {
                    // Step 5
                    int cost = Text2[j - 1] == Text1[i - 1] ? 0 : 1;
                    // Step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// compara 2 strings usando wildcards
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public static bool Like(this String source, String Pattern) => new Like(Pattern).Matches(source);

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string MaskTelephoneNumber(this string Number)
        {
            Number = Number ?? "";
            Number = Number.ParseDigits().RemoveAny(",", ".").ToLong().ToString();
            if (Number.IsBlank())
            {
                return "";
            }

            string mask;
            switch (Number.Length)
            {
                case var fourOrLess when fourOrLess <= 4:
                    {
                        mask = "{0:####}";
                        break;
                    }

                case var eightOrLess when eightOrLess <= 8:
                    {
                        mask = "{0:####-####}";
                        break;
                    }

                case 9:
                    {
                        mask = "{0:#####-####}";
                        break;
                    }

                case 10:
                    {
                        mask = "{0:(##) ####-####}";
                        break;
                    }

                case 11:
                    {
                        mask = "{0:(##) #####-####}";
                        break;
                    }

                case 12:
                    {
                        mask = "{0:+## (##) ####-####}";
                        break;
                    }

                case 13:
                    {
                        mask = "{0:+## (##) #####-####}";
                        break;
                    }

                default:
                    {
                        return Number.ToString();
                    }
            }

            return string.Format(mask, long.Parse(Number));
        }

        /// <inheritdoc cref="MaskTelephoneNumber(int)"/>
        public static string MaskTelephoneNumber(this long Number) => Number.ToString().MaskTelephoneNumber();

        /// <inheritdoc cref="MaskTelephoneNumber(string)"/>
        public static string MaskTelephoneNumber(this int Number) => Number.ToString().MaskTelephoneNumber();

        /// <inheritdoc cref="MaskTelephoneNumber(int)"/>
        public static string MaskTelephoneNumber(this decimal Number) => Number.ToString().MaskTelephoneNumber();

        /// <inheritdoc cref="MaskTelephoneNumber(int)"/>
        public static string MaskTelephoneNumber(this double Number) => Number.ToString().MaskTelephoneNumber();

        /// <summary>
        /// Adciona caracteres ao inicio e final de uma string enquanto o <see
        /// cref="string.Length"/> de <paramref name="Text"/> for menor que <paramref name="TotalLength"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="TotalLength">Tamanho total</param>
        /// <param name="PaddingChar">Caractere</param>
        /// <param name="Trim">
        /// Quando TRUE, elimina caracteres do começo e fim da string se ela for maior que <paramref name="TotalLength"/>
        /// </param>
        /// <returns></returns>
        public static string Pad(this string Text, int TotalLength, char PaddingChar = ' ')
        {
            if (Text.Length < TotalLength)
            {
                if (Text.IsNotBlank())
                {
                    Text = Text.Wrap(" ");
                }

                while (Text.Length < TotalLength)
                {
                    Text = Text.Wrap(PaddingChar.ToString());
                }

                if (Text.Length > TotalLength)
                {
                    Text = Text.RemoveLastChars();
                }
            }

            return Text;
        }

        /// <summary>
        /// limpa um texto deixando apenas os caracteres alfanumericos.
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ParseAlphaNumeric(this string Text)
        {
            var l = new List<string>();
            foreach (var item in Text.Split(" ", StringSplitOptions.RemoveEmptyEntries))
            {
                l.Add(Regex.Replace(item, "[^A-Za-z0-9]", ""));
            }

            return l.JoinString(" ");
        }

        /// <summary>
        /// Parseia uma ConnectionString em um <see cref="ConnectionStringParser"/>
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static ConnectionStringParser ParseConnectionString(this string ConnectionString) => new ConnectionStringParser(ConnectionString);

        /// <summary>
        /// Remove caracteres não numéricos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string ParseDigits(this string Text, CultureInfo Culture = null)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            string strDigits = "";
            if (string.IsNullOrEmpty(Text))
            {
                return strDigits;
            }

            foreach (char c in Text.ToCharArray())
            {
                if (char.IsDigit(c) || c == Convert.ToChar(Culture.NumberFormat.NumberDecimalSeparator))
                {
                    strDigits += Convert.ToString(c);
                }
            }

            return strDigits;
        }

        /// <summary>
        /// Remove caracteres não numéricos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static Type ParseDigits<Type>(this string Text, CultureInfo Culture = null) where Type : IConvertible => Text.ParseDigits(Culture).ChangeType<Type>();

        /// <summary>
        /// Transforma uma <see cref="string"/> em um <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="QueryString">string contendo uma querystring valida</param>
        /// <param name="Keys">Quando especificado, inclui apenas estas entradas no <see cref="NameValueCollection"/></param>
        /// <returns></returns>
        public static NameValueCollection ParseQueryString(this string QueryString, params string[] Keys)
        {
            Keys = Keys ?? Array.Empty<string>();
            var queryParameters = new NameValueCollection();
            var querySegments = QueryString.Split('&');
            foreach (string segment in querySegments)
            {
                var parts = segment.Split('=');
                if (parts.Any())
                {
                    string key = parts.First().RemoveFirstAny(" ", "?");
                    string val = "";
                    if (parts.Skip(1).Any())
                    {
                        val = parts[1].Trim().UrlDecode();
                    }
                    if (Keys.Contains(key) || Keys.Any() == false)
                    {
                        queryParameters.Add(key, val);
                    }
                }
            }

            return queryParameters;
        }

        /// <summary>
        /// Separa as palavras de um texto CamelCase a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string PascalCaseAdjust(this string Text)
        {
            Text = Text.IfBlank("");
            var chars = Text.ToArray();
            Text = "";
            int uppercount = 0;
            foreach (var c in chars)
            {
                if (char.IsUpper(c))
                {
                    if (!(uppercount > 0))
                    {
                        Text += " ";
                    }

                    uppercount++;
                }
                else
                {
                    if (uppercount > 1)
                    {
                        Text += " ";
                    }

                    uppercount = 0;
                }

                Text += Convert.ToString(c);
            }

            return Text.Trim();
        }

        /// <summary>
        /// Transforma um texto em CamelCase em um array de palavras a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> PascalCaseSplit(this string Text) => Text.PascalCaseAdjust().Split(" ");

        /// <summary>
        /// Retorna uma string em sua forma poop
        /// </summary>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static string[] Poopfy(params string[] Words)
        {
            var p = new List<string>();
            foreach (var Text in Words)
            {
                decimal l = (decimal)(Text.Length / 2d);
                l = l.Floor();
                if (!Text.GetFirstChars((int)Math.Round(l)).Last().ToString().ToLower().IsIn(PredefinedArrays.LowerVowels))
                {
                    l = l.ToInt() - 1;
                }

                p.Add(Text.GetFirstChars((int)Math.Round(l)).Trim() + Text.GetFirstChars((int)Math.Round(l)).Reverse().ToList().JoinString().ToLower().Trim() + Text.RemoveFirstChars((int)Math.Round(l)).RemoveFirstAny(PredefinedArrays.LowerConsonants.ToArray()));
            }

            return p.ToArray();
        }

        /// <summary>
        /// Retorna uma string em sua forma poop
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Poopfy(this string Text) => Poopfy(Text.Split(" ")).JoinString(" ");

        /// <summary>
        /// Return a Idented XML string
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string PreetyPrint(this XmlDocument Document)
        {
            string Result = "";
            var mStream = new MemoryStream();
            var writer = new XmlTextWriter(mStream, Encoding.Unicode);
            try
            {
                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                Document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read its contents.
                mStream.Position = 0L;

                // Read MemoryStream contents into a StreamReader.
                var sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                Result = sReader.ReadToEnd();
            }
            catch (XmlException)
            {
            }
            finally
            {
                mStream?.Close();
                writer?.Close();
                mStream?.Dispose();
                writer?.Dispose();
            }

            return Result;
        }

        /// <summary>
        /// Adiciona texto ao começo de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        public static string Prepend(this string Text, string PrependText)
        {
            Text = Text ?? "";
            PrependText = PrependText ?? "";
            Text = PrependText + Text;
            return Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependIf(this string Text, string PrependText, Func<string, bool> Test = null)
        {
            Text = Text ?? "";
            PrependText = PrependText ?? "";
            return Text.PrependIf(PrependText, (Test ?? (x => false))(Text));
        }

        /// <summary>
        /// Adiciona texto ao começo de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependIf(this string Text, string PrependText, bool Test)
        {
            Text = Text ?? "";
            PrependText = PrependText ?? "";
            return Test ? Text.Prepend(PrependText) : Text;
        }

        /// <summary>
        /// Adiciona texto ao inicio de uma string com uma quebra de linha no final do <paramref name="PrependText"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        public static string PrependLine(this string Text, string PrependText) => Text.Prepend(Environment.NewLine).Prepend(PrependText);

        /// <summary>
        /// Adiciona texto ao inicio de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependWhile(this string Text, string PrependText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);

            while (Test(Text))
            {
                Text = Text.Prepend(PrependText);
            }

            return Text;
        }

        /// <summary>
        /// Retorna a string especificada se o valor boolean for verdadeiro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="BooleanValue"></param>
        /// <returns></returns>
        public static string PrintIf(this string Text, bool BooleanValue) => BooleanValue ? Text : "";

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico encontrado no primeiro parametro.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <returns></returns>
        /// <example>texto = $"{2} pães"</example>
        public static string QuantifyText(this FormattableString PluralText)
        {
            if (PluralText.IsNotBlank() && PluralText.ArgumentCount > 0)
            {
                decimal numero = 0m;
                string str = PluralText.Format.QuantifyText(PluralText.GetArguments().FirstOrDefault(), ref numero);
                str = str.Replace("{0}", numero.ToString());
                for (int index = 1, loopTo = PluralText.GetArguments().Count() - 1; index <= loopTo; index++)
                {
                    str = str.Replace($"{{{index}}}", Convert.ToString(PluralText.GetArgument(index)));
                }

                return str;
            }

            return PluralText?.ToString();
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this string PluralText, object Quantity)
        {
            decimal d = 0m;
            return PluralText.QuantifyText(Quantity, ref d);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="QuantityOrListOrBoolean">Quantidade de Itens</param>
        /// <param name="OutQuantity">Devolve a quantidade encontrada em <paramref name="QuantityOrListOrBoolean"/></param>
        /// <returns></returns>
        public static string QuantifyText(this string PluralText, object QuantityOrListOrBoolean, ref decimal OutQuantity)
        {
            switch (true)
            {
                case object _ when QuantityOrListOrBoolean is null:
                    {
                        OutQuantity = 0m;
                        break;
                    }

                case object _ when QuantityOrListOrBoolean.GetType() == typeof(bool):
                    {
                        OutQuantity = Converter.ToDecimal(QuantityOrListOrBoolean);
                        return PluralText.Singularize(); // de acordo com as normas do portugues, quando a quantidade esperada maxima for 1, zero também é singular.
                    }

                case object _ when QuantityOrListOrBoolean.IsNumber():
                    {
                        OutQuantity = Convert.ToDecimal(QuantityOrListOrBoolean);
                        break;
                    }

                case object _ when typeof(IList).IsAssignableFrom(QuantityOrListOrBoolean.GetType()):
                    {
                        OutQuantity = ((IList)QuantityOrListOrBoolean).Count;
                        break;
                    }

                case object _ when typeof(IDictionary).IsAssignableFrom(QuantityOrListOrBoolean.GetType()):
                    {
                        OutQuantity = ((IDictionary)QuantityOrListOrBoolean).Count;
                        break;
                    }

                case object _ when typeof(Array).IsAssignableFrom(QuantityOrListOrBoolean.GetType()):
                    {
                        OutQuantity = ((Array)QuantityOrListOrBoolean).Length;
                        break;
                    }

                default:
                    {
                        OutQuantity = Convert.ToDecimal(QuantityOrListOrBoolean);
                        break;
                    }
            }

            if (OutQuantity.Floor() == 1m || OutQuantity.Floor() == -1)
            {
                return PluralText.Singularize();
            }

            return PluralText;
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="List">Lista com itens</param>
        /// <returns></returns>
        public static string QuantifyText<T>(this IEnumerable<T> List, string PluralText) => PluralText.QuantifyText(List ?? Array.Empty<T>());

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this int Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this decimal Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this short Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this long Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this double Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes)
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="OpenQuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Quote(this string Text, char OpenQuoteChar = '"')
        {
            if (Convert.ToBoolean(OpenQuoteChar.ToString().IsCloseWrapChar()))
            {
                OpenQuoteChar = OpenQuoteChar.GetOppositeWrapChar();
            }

            return $"{OpenQuoteChar}{Text}{OpenQuoteChar.GetOppositeWrapChar()}";
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes)
        /// se uma condição for cumprida
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="QuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string QuoteIf(this string Text, bool Condition, char QuoteChar = '"') => Condition ? Text.Quote(QuoteChar) : Text;

        /// <summary>
        /// Sorteia um item da Matriz
        /// </summary>
        /// <typeparam name="Type">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static Type RandomItem<Type>(params Type[] Array) => Array.GetRandomItem();

        /// <summary>
        /// Escapa caracteres exclusivos de uma regex
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string RegexEscape(this string Text)
        {
            string newstring = "";
            foreach (var c in Text.ToArray())
            {
                if (c.IsIn(PredefinedArrays.RegexChars))
                {
                    newstring += @"\" + c;
                }
                else
                {
                    newstring += Convert.ToString(c);
                }
            }

            return newstring;
        }

        /// <summary>
        /// Remove os acentos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String sem os acentos</returns>
        public static string RemoveAccents(this string Text)
        {
            string s = Text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            int k = 0;
            while (k < s.Length)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }

                k++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Remove várias strings de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Values">Strings a serem removidas</param>
        /// <returns>Uma string com os valores removidos</returns>
        public static string RemoveAny(this string Text, params string[] Values) => Text.ReplaceMany("", Values ?? Array.Empty<string>());

        public static string RemoveAny(this string Text, params char[] Values) => Text.RemoveAny(Values.Select(x => x.ToString()).ToArray());

        /// <summary>
        /// Remove os acentos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String sem os acentos</returns>
        public static string RemoveDiacritics(this string Text) => Text.RemoveAccents();

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveFirstAny(this string Text, bool ContinuouslyRemove, StringComparison comparison, params string[] StartStringTest)
        {
            string re = Text;
            while (re.StartsWithAny(comparison, StartStringTest))
            {
                foreach (var item in StartStringTest)
                {
                    if (re.StartsWith(item, comparison))
                    {
                        re = re.RemoveFirstEqual(item, comparison);
                        if (!ContinuouslyRemove)
                        {
                            return re;
                        }
                    }
                }
            }

            return re;
        }

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string RemoveFirstAny(this string Text, StringComparison comparison, params string[] StartStringTest) => Text.RemoveFirstAny(true, comparison, StartStringTest);

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveFirstAny(this string Text, params string[] StartStringTest) => Text.RemoveFirstAny(true, default, StartStringTest);

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveFirstAny(this string Text, bool ContinuouslyRemove, params string[] StartStringTest) => Text.RemoveFirstAny(ContinuouslyRemove, default, StartStringTest);

        /// <summary>
        /// Remove os X primeiros caracteres
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Quantity">Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveFirstChars(this string Text, int Quantity = 1)
        {
            if (Text.Length > Quantity && Quantity > 0)
            {
                return Text.Remove(0, Quantity);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Remove um texto do inicio de uma string se ele for um outro texto especificado
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Texto inicial que será comparado</param>
        public static string RemoveFirstEqual(this string Text, string StartStringTest, StringComparison comparison = default)
        {
            if (Text.StartsWith(StartStringTest, comparison))
            {
                Text = Text.RemoveFirstChars(StartStringTest.Length);
            }

            return Text;
        }

        public static string RemoveHTML(this string Text)
        {
            if (Text.IsNotBlank())
            {
                return Regex.Replace(Text.ReplaceMany(Environment.NewLine, "<br/>", "<br>", "<br />"), "<.*?>", string.Empty).HtmlDecode();
            }

            return Text;
        }

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveLastAny(this string Text, bool ContinuouslyRemove, StringComparison comparison, params string[] EndStringTest)
        {
            string re = Text;
            while (re.EndsWithAny(comparison, EndStringTest))
            {
                foreach (var item in EndStringTest)
                {
                    if (re.EndsWith(item, comparison))
                    {
                        re = re.RemoveLastEqual(item, comparison);
                        if (!ContinuouslyRemove)
                        {
                            return re;
                        }
                    }
                }
            }

            return re;
        }

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveLastAny(this string Text, params string[] EndStringTest) => Text.RemoveLastAny(true, default, EndStringTest);

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <param name="ContinuouslyRemove">Remove continuamente as strings</param>
        /// <returns></returns>
        public static string RemoveLastAny(this string Text, bool ContinuouslyRemove, params string[] EndStringTest) => Text.RemoveLastAny(ContinuouslyRemove, default, EndStringTest);

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveLastAny(this string Text, StringComparison comparison, params string[] EndStringTest) => Text.RemoveLastAny(true, comparison, EndStringTest);

        /// <summary>
        /// Remove os X ultimos caracteres
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Quantity">Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveLastChars(this string Text, int Quantity = 1)
        {
            if (Text.Length > Quantity && Quantity > 0)
            {
                return Text.Substring(0, Text.Length - Quantity);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Remove um texto do final de uma string se ele for um outro texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Texto final que será comparado</param>
        public static string RemoveLastEqual(this string Text, string EndStringTest, StringComparison comparison = default)
        {
            if (Text.EndsWith(EndStringTest, comparison))
            {
                Text = Text.RemoveLastChars(EndStringTest.Length);
            }

            return Text;
        }

        /// <summary>
        /// Remove caracteres não prantáveis de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String corrigida</returns>
        public static string RemoveNonPrintable(this string Text)
        {
            foreach (char c in Text.ToCharArray())
            {
                if (char.IsControl(c))
                {
                    Text = Text.ReplaceNone(Convert.ToString(c));
                }
            }

            return Text.Trim();
        }

        /// <summary>
        /// Repete uma string um numero determinado de vezes
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public static string Repeat(this string Text, int Times = 2)
        {
            var ns = "";
            while (Times > 0)
            {
                ns += Text;
                Times--;
            }
            return ns;
        }

        /// <summary>
        /// Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum
        /// </summary>
        /// <param name="Strings">Array de strings</param>
        /// <param name="OldValue">Valor antigo que será substituido</param>
        /// <param name="NewValue">Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE
        /// realiza um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
        /// </param>
        /// <returns></returns>
        public static string[] Replace(this string[] Strings, string OldValue, string NewValue, bool ReplaceIfEquals = true)
        {
            var NewArray = Strings;
            for (int index = 0, loopTo = Strings.Length - 1; index <= loopTo; index++)
            {
                if (ReplaceIfEquals)
                {
                    if ((NewArray[index] ?? "") == (OldValue ?? ""))
                    {
                        NewArray[index] = NewValue;
                    }
                }
                else
                {
                    NewArray[index] = NewArray[index].Replace(OldValue, NewValue);
                }
            }

            return NewArray;
        }

        /// <summary>
        /// Faz uma busca em todos os elementos de uma lista e aplica um ReplaceFrom comum
        /// </summary>
        /// <param name="Strings">Array de strings</param>
        /// <param name="OldValue">Valor antigo que será substituido</param>
        /// <param name="NewValue">Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE
        /// realiza um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
        /// </param>
        /// <returns></returns>
        public static IEnumerable<string> Replace(this IEnumerable<string> Strings, string OldValue, string NewValue, bool ReplaceIfEquals = true) => Strings.ToArray().Replace(OldValue, NewValue, ReplaceIfEquals).ToList();

        /// <summary>
        /// Substitui a primeira ocorrencia de um texto por outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OldText"></param>
        /// <param name="NewText"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string Text, string OldText, string NewText = "")
        {
            if (Text.Contains(OldText))
            {
                Text = Text.Insert(Text.IndexOf(OldText), NewText);
                Text = Text.Remove(Text.IndexOf(OldText), 1);
            }

            return Text;
        }

        /// <summary>
        /// Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string, string> Dic)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    Text = Text.Replace(p.Key, p.Value);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
        /// </summary>
        public static string ReplaceFrom<T>(this string Text, IDictionary<string, T> Dic)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    switch (true)
                    {
                        case object _ when p.Value.IsDictionary():
                            {
                                Text = Text.ReplaceFrom((IDictionary<string, object>)p.Value);
                                break;
                            }

                        case object _ when typeof(T).IsAssignableFrom(typeof(Array)):
                            {
                                foreach (var item in Converter.ForceArray(p.Value, typeof(T)))
                                {
                                    Text = Text.ReplaceMany(p.Key, Converter.ForceArray(p.Value, typeof(T)).Cast<string>().ToArray());
                                }

                                break;
                            }

                        default:
                            {
                                Text = Text.Replace(p.Key, p.Value.ToString());
                                break;
                            }
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string, string[]> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    Text = Text.SensitiveReplace(p.Key, p.Value, Comparison);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string[], string> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    Text = Text.SensitiveReplace(p.Value, p.Key.ToArray(), Comparison);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string[], string[]> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    var froms = p.Key.ToList();
                    var tos = p.Value.ToList();
                    while (froms.Count > tos.Count)
                    {
                        tos.Add(string.Empty);
                    }

                    for (int i = 0, loopTo = froms.Count - 1; i <= loopTo; i++)
                    {
                        Text = Text.SensitiveReplace(froms[i], tos[i], Comparison);
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Substitui a ultima ocorrencia de um texto por outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OldText"></param>
        /// <param name="NewText"></param>
        /// <returns></returns>
        public static string ReplaceLast(this string Text, string OldText, string NewText = "")
        {
            if (Text.Contains(OldText))
            {
                Text = Text.Insert(Text.LastIndexOf(OldText), NewText);
                Text = Text.Remove(Text.LastIndexOf(OldText), 1);
            }

            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por um novo valor.
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="NewValue">Novo Valor</param>
        /// <param name="OldValues">Valores a serem substituido por um novo valor</param>
        /// <returns></returns>
        public static string ReplaceMany(this string Text, string NewValue, params string[] OldValues)
        {
            Text = Text ?? "";
            foreach (var word in (OldValues ?? Array.Empty<string>()).Where(x => x.Length > 0))
            {
                Text = Text.Replace(word, NewValue);
            }

            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por vazio.
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="OldValue">Valor a ser substituido por vazio</param>
        /// <returns>String corrigida</returns>
        public static string ReplaceNone(this string Text, string OldValue) => Text.Replace(OldValue, string.Empty);

        public static IEnumerable<String> SelectLike(this IEnumerable<String> source, String Pattern) => source.Where(x => x.Like(Pattern));

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="NewValue"></param>
        /// <param name="OldValue"></param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string OldValue, string NewValue, StringComparison ComparisonType = StringComparison.InvariantCulture) => Text.SensitiveReplace(NewValue, new[] { OldValue }, ComparisonType);

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="NewValue"></param>
        /// <param name="OldValues"></param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string NewValue, IEnumerable<string> OldValues, StringComparison ComparisonType = StringComparison.InvariantCulture)
        {
            if (Text.IsNotBlank())
            {
                foreach (var oldvalue in OldValues ?? new[] { string.Empty })
                {
                    NewValue = NewValue ?? string.Empty;
                    if (!oldvalue.Equals(NewValue, ComparisonType))
                    {
                        int foundAt;
                        do
                        {
                            foundAt = Text.IndexOf(oldvalue, 0, ComparisonType);
                            if (foundAt > -1)
                            {
                                Text = Text.Remove(foundAt, oldvalue.Length).Insert(foundAt, NewValue);
                            }
                        }
                        while (foundAt != -1);
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Randomiza a ordem dos itens de um Array
        /// </summary>
        /// <typeparam name="Type">Tipo do Array</typeparam>
        /// <param name="Array">Matriz</param>
        public static Type[] Shuffle<Type>(this Type[] Array) => Array.OrderByRandom().ToArray();

        /// <summary>
        /// Randomiza a ordem dos itens de uma Lista
        /// </summary>
        /// <typeparam name="Type">Tipo de Lista</typeparam>
        /// <param name="List">Matriz</param>
        public static List<Type> Shuffle<Type>(this List<Type> List) => List.OrderByRandom().ToList();

        /// <summary>
        /// Aleatoriza a ordem das letras de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string Shuffle(this string Text) => Text.OrderByRandom().JoinString();

        /// <summary>
        /// Retorna a frase ou termo especificado em sua forma singular
        /// </summary>
        /// <param name="Text">Texto no plural</param>
        /// <returns></returns>
        public static string Singularize(this string Text)
        {
            var phrase = Text.ApplySpaceOnWrapChars().Split(" ");
            for (int index = 0, loopTo = phrase.Count() - 1; index <= loopTo; index++)
            {
                string endchar = phrase[index].GetLastChars();
                if (endchar.IsAny(PredefinedArrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index].RemoveLastEqual(endchar);
                }

                switch (true)
                {
                    case object _ when phrase[index].IsNumber() || phrase[index].IsEmail() || phrase[index].IsURL() || phrase[index].IsIP() || phrase[index].IsIn(PredefinedArrays.WordSplitters):
                        {
                            // nao alterar estes tipos
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ões"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ões") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ãos"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ãos") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ães"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ães") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ais"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ais") + "al";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("eis"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("eis") + "il";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("éis"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("éis") + "el";

                            break;
                        }

                    case object _ when phrase[index].EndsWith("ois"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ois") + "ol";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("uis"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("uis") + "ul";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("es"):
                        {
                            if (phrase[index].RemoveLastEqual("es").EndsWithAny("z", "r"))
                            {
                                phrase[index] = phrase[index].RemoveLastEqual("es");
                            }
                            else
                            {
                                phrase[index] = phrase[index].RemoveLastEqual("s");
                            }

                            break;
                        }

                    case object _ when phrase[index].EndsWith("ns"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("ns") + "m";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("s"):
                        {
                            phrase[index] = phrase[index].RemoveLastEqual("s");
                            break;
                        }

                    default:
                        {
                            break;
                        }
                        // ja esta no singular
                }

                if (endchar.IsAny(PredefinedArrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index] + endchar;
                }
            }

            return phrase.JoinString(" ").TrimBetween();
        }

        /// <summary>
        /// Corta um texto para exibir um numero máximo de caracteres ou na primeira quebra de linha.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="TextLength"></param>
        /// <param name="Ellipsis"></param>
        /// <returns></returns>
        public static string Slice(this string Text, int TextLength = 0, string Ellipsis = "...", bool TrimCarriage = false)
        {
            if (Text.IsBlank() || Text.Length <= TextLength || TextLength < 1)
            {
                return Text;
            }
            else
            {
                if (TrimCarriage)
                {
                    Text = Text.TrimCarriage().GetBefore(Environment.NewLine);
                }

                return $"{Text.GetFirstChars(TextLength)}{Ellipsis}";
            }
        }

        /// <summary>
        /// Separa um texto em um array de strings a partir de uma outra string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Separator">Texto utilizado como separador</param>
        /// <returns></returns>
        public static string[] Split(this string Text, string Separator, StringSplitOptions Options = StringSplitOptions.RemoveEmptyEntries) => (Text ?? "").Split(new[] { Separator }, Options);

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, params string[] SplitText)
        {
            SplitText = SplitText ?? Array.Empty<string>();
            return Text?.Split(SplitText, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, IEnumerable<string> SplitText) => Text.SplitAny(SplitText.ToArray());

        /// <summary>
        /// Verifica se uma string começa com alguma outra string de um array
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string Text, StringComparison comparison, params string[] Words) => Words.Any(p => Text.IfBlank("").StartsWith(p, comparison));

        public static bool StartsWithAny(this string Text, params string[] Words) => StartsWithAny(Text, StringComparison.InvariantCultureIgnoreCase, Words);

        /// <summary>
        /// Alterna maiusculas e minusculas para cada letra de uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToAlternateCase(this string Text)
        {
            var ch = Text.ToArray();
            for (int index = 0, loopTo = ch.Length - 1; index <= loopTo; index++)
            {
                char antec = ch.IfNoIndex(index - 1, Convert.ToChar(""));
                if (antec.ToString().IsBlank() || char.IsLower(antec) || antec.ToString() == null)
                {
                    ch[index] = char.ToUpper(ch[index]);
                }
                else
                {
                    ch[index] = char.ToLower(ch[index]);
                }
            }

            return new string(ch);
        }

        /// <summary>
        /// Retorna um anagrama de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string ToAnagram(this string Text) => Shuffle(Text);

        public static IEnumerable<int> ToAsc(this string c) => c.ToArray().Select(x => x.ToAsc());

        public static int ToAsc(this char c)
        {
            int converted = c;
            if (converted >= 0x80)
            {
                byte[] buffer = new byte[2];
                // if the resulting conversion is 1 byte in length, just use the value
                if (Encoding.Default.GetBytes(new char[] { c }, 0, 1, buffer, 0) == 1)
                {
                    converted = buffer[0];
                }
                else
                {
                    // byte swap bytes 1 and 2;
                    converted = buffer[0] << 16 | buffer[1];
                }
            }
            return converted;
        }

        public static byte ToAscByte(this char c) => (byte)c.ToAsc();

        /// <summary>
        /// Returns a CSV String from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="Separator"></param>
        /// <param name="IncludeHeader"></param>
        /// <returns></returns>
        public static string ToCSV(this IEnumerable<Dictionary<string, object>> Items, string Separator = ",", bool IncludeHeader = false)
        {
            Separator = Separator.IfBlank(",");
            var str = $"sep={Separator}{Environment.NewLine}";
            if (Items != null && Items.Any())
            {
                Items = Items.MergeKeys();

                if (IncludeHeader && Items.All(x => x.Keys.Any()))
                {
                    str += $"{Items.FirstOrDefault()?.Keys.SelectJoinString(Separator)}";
                }
                str += $"{Items.SelectJoinString(x => x.Values.SelectJoinString(Separator), Environment.NewLine)}";
            }

            return str;
        }

        public static string ToCSV<T>(this IEnumerable<T> Items, string Separator = ",", bool IncludeHeader = false) where T : class => (Items ?? Array.Empty<T>()).Select(x => x.CreateDictionary()).ToCSV(Separator, IncludeHeader);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this byte[] Size, int DecimalPlaces = -1) => Size.LongLength.ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this FileInfo Size, int DecimalPlaces = -1) => Size.Length.ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this double Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this int Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this long Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this decimal Size, int DecimalPlaces = -1) => UnitConverter.CreateFileSizeConverter().Abreviate(Size, DecimalPlaces);

        public static FormattableString ToFormattableString(this string Text, params object[] args) => FormattableStringFactory.Create(Text, args ?? Array.Empty<object>());

        public static FormattableString ToFormattableString<T>(IEnumerable<T> args, string Text) => ToFormattableString(Text, args);

        public static FormattableString ToFormattableString(this string Text, IEnumerable<object[]> args) => ToFormattableString(Text, args);

        /// <summary>
        /// Prepara uma string para se tornar uma caminho amigavel (remove caracteres nao permitidos)
        /// </summary>
        /// <param name="Text"></param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyPathName(this string Text) => Text.RemoveAny(Path.GetInvalidPathChars()).TrimBetween();

        /// <summary>
        /// Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e
        /// troca espacos por hifen)
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="UseUnderscore">
        /// Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
        /// </param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyURL(this string Text, bool UseUnderscore = false) => Text.ReplaceMany(UseUnderscore ? "_" : "-", "_", "-", " ").RemoveAny("(", ")", ".", ",", "#").ToFriendlyPathName().RemoveAccents().ToLower();

        /// <summary>
        /// Converte um texto para Leet (1337)
        /// </summary>
        /// <param name="text">TExto original</param>
        /// <param name="degree">Grau de itensidade (0 a 7)</param>
        /// <returns>Texto em 1337</returns>
        public static string ToLeet(this string Text, int Degree = 7)
        {
            // Adjust degree between 0 - 100
            Degree = Degree.LimitRange(0, 7);
            // No Leet Translator
            if (Degree == 0)
            {
                return Text;
            }
            // StringBuilder to store result.
            var sb = new StringBuilder();
            foreach (char c in Text)
            {
                switch (Degree)
                {
                    case 1:
                        switch (c)
                        {
                            case 'e':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 2:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 3:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 4:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append("9");
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append("9");
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 5:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append("9");
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append("6");
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 6:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append("9");
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append("6");
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'r':
                                {
                                    sb.Append("®");
                                    break;
                                }

                            case 'R':
                                {
                                    sb.Append("®");
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'b':
                                {
                                    sb.Append("ß");
                                    break;
                                }

                            case 'B':
                                {
                                    sb.Append("ß");
                                    break;
                                }

                            case 'q':
                                {
                                    sb.Append("Q");
                                    break;
                                }

                            case 'Q':
                                {
                                    sb.Append("Q¸");
                                    break;
                                }

                            case 'x':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            case 'X':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    default:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append("4");
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append("3");
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append("1");
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append("0");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append("$");
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append("9");
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append("6");
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append("£");
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append("(");
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append("7");
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append("2");
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append("¥");
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append("µ");
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append("ƒ");
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append("Ð");
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'r':
                                {
                                    sb.Append("®");
                                    break;
                                }

                            case 'R':
                                {
                                    sb.Append("®");
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'b':
                                {
                                    sb.Append("ß");
                                    break;
                                }

                            case 'B':
                                {
                                    sb.Append("ß");
                                    break;
                                }

                            case 'j':
                                {
                                    sb.Append("_|");
                                    break;
                                }

                            case 'J':
                                {
                                    sb.Append("_|");
                                    break;
                                }

                            case 'P':
                                {
                                    sb.Append("|°");
                                    break;
                                }

                            case 'q':
                                {
                                    sb.Append("¶");
                                    break;
                                }

                            case 'Q':
                                {
                                    sb.Append("¶¸");
                                    break;
                                }

                            case 'x':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            case 'X':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Pega um texto em "PascalCase" ou "snake_case" e o retorna na forma "normal case"
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToNormalCase(this string Text) => Text.PascalCaseAdjust().Replace("_", " ");

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this decimal Number, int Decimals = -1)
        {
            if (Decimals > -1)
            {
                Number = decimal.Round(Number, Decimals);
            }

            return $"{Number}%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this int Number) => $"{Number}%";

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this double Number, int Decimals = -1)
        {
            if (Decimals > -1)
            {
                Number = Number.RoundDouble(Decimals);
            }

            return $"{Number}%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this short Number) => $"{Number}%";

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this long Number) => $"{Number}%";

        /// <summary>
        /// Concatena todos as strings em uma lista, utilizando a palavra <paramref name="And"/> na
        /// ultima ocorrencia.
        /// </summary>
        /// <param name="Texts"></param>
        /// <param name="And"></param>
        /// <returns></returns>
        public static string ToPhrase(this IEnumerable<string> Texts, string And = "and")
        {
            if (Texts != null && Texts.Count() > 1)
            {
                return TrimBetween($"{Texts.SkipLast(1).JoinString(", ")} {And.IfBlank(",")} {Texts.Last()}");
            }
            else
            {
                return TrimBetween(Texts?.FirstOrDefault());
            }
        }

        /// <summary>
        /// Concatena todos as strings em uma lista, utilizando a palavra <paramref name="And"/> na
        /// ultima ocorrencia.
        /// </summary>
        /// <param name="Texts"></param>
        /// <param name="And"></param>
        /// <returns></returns>
        public static string ToPhrase(string And, params string[] Texts) => (Texts ?? Array.Empty<string>()).ToPhrase(And);

        /// <summary>
        /// Coloca o texto em TitleCase
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToProperCase(this string Text, bool ForceCase = false)
        {
            if (Text.IsBlank())
            {
                return Text;
            }

            if (ForceCase)
            {
                Text = Text.ToLower();
            }

            var l = Text.Split(" ", StringSplitOptions.None).ToList();
            for (int index = 0, loopTo = l.Count - 1; index <= loopTo; index++)
            {
                string pal = l[index];
                bool artigo = index > 0 && Misc.IsIn(pal, "o", "a", "os", "as", "um", "uma", "uns", "umas", "de", "do", "dos", "das", "e");
                if (pal.IsNotBlank())
                {
                    if (ForceCase || artigo == false)
                    {
                        char c = pal.First();
                        if (!char.IsUpper(c))
                        {
                            pal = char.ToUpper(c) + pal.RemoveFirstChars(1);
                        }

                        l[index] = pal;
                    }
                }
            }

            return l.SelectJoinString(" ");
        }

        /// <summary>
        /// Coloca a string em Randomcase (aleatoriamente letras maiusculas ou minusculas)
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Times">Numero de vezes que serão sorteados caracteres.</param>
        /// <returns></returns>
        public static string ToRandomCase(this string Text, int Times = 0)
        {
            var ch = Text.ToArray();
            Times = Times.SetMinValue(ch.Length);
            for (int index = 1, loopTo = Times; index <= loopTo; index++)
            {
                int newindex = Generate.RandomNumber(0, ch.Length - 1);
                if (char.IsUpper(ch[newindex]))
                {
                    ch[newindex] = char.ToLower(ch[newindex]);
                }
                else
                {
                    ch[newindex] = char.ToUpper(ch[newindex]);
                }
            }

            return new string(ch);
        }

        public static string ToSentence(this IEnumerable<string> Texts, string And = "and") => ToPhrase(Texts, And);

        /// <summary>
        /// Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e
        /// troca espacos por hifen). É um alias para <see cref="ToFriendlyURL(String, Boolean)"/>
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="UseUnderscore">
        /// Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
        /// </param>
        /// <returns>string amigavel para URL</returns>
        public static string ToSlugCase(this string Text, bool UseUnderscore = false) => Text.ToFriendlyURL(UseUnderscore);

        /// <summary>
        /// Retorna uma string em Snake_Case
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string Text) => Text.Replace(" ", "_").ToLower();

        /// <summary>
        /// Cria um <see cref="Stream"/> a partir de uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static Stream ToStream(this string Text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(Text);
            writer.Flush();
            stream.Position = 0L;
            return stream;
        }

        /// <summary>
        /// Transforma um texto em titulo
        /// </summary>
        /// <param name="Text">Texto a ser manipulado</param>
        /// <param name="ForceCase">
        /// Se FALSE, apenas altera o primeiro caractere de cada palavra como UPPERCASE, dexando os
        /// demais intactos. Se TRUE, força o primeiro caractere de casa palavra como UPPERCASE e os
        /// demais como LOWERCASE
        /// </param>
        /// <returns>Uma String com o texto em nome próprio</returns>
        public static string ToTitle(this string Text, bool ForceCase = false) => Text.ToProperCase(ForceCase);

        /// <summary>
        /// Transforma um XML Document em string
        /// </summary>
        /// <param name="XML">Documento XML</param>
        /// <returns></returns>
        public static string ToXMLString(this XmlDocument XML)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                XML.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="StringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, bool ContinuouslyRemove, params string[] StringTest)
        {
            if (Text.IsNotBlank())
            {
                Text = Text.RemoveFirstAny(ContinuouslyRemove, StringTest);
                Text = Text.RemoveLastAny(ContinuouslyRemove, StringTest);
            }

            return Text;
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, params string[] StringTest) => Text.TrimAny(true, StringTest);

        /// <summary>
        /// Remove continuamente caracteres em branco do começo e fim de uma string incluindo breaklines
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string TrimCarriage(this string Text) => Text.TrimAny(PredefinedArrays.InvisibleChars.ToArray());

        public static string UnBrackfy(this string Text) => Text.UnBrackfy(char.MinValue, true);

        public static string UnBrackfy(this string Text, char BracketChar, bool ContinuouslyRemove = false) => Text.UnQuote(BracketChar, ContinuouslyRemove);

        public static string UnQuote(this string Text) => Text.UnQuote(char.MinValue, true);

        public static string UnQuote(this string Text, char OpenQuoteChar, bool ContinuouslyRemove = false)
        {
            if ($"{OpenQuoteChar}".RemoveNonPrintable().IsBlank())
            {
                while (Text.EndsWithAny(PredefinedArrays.CloseWrappers.ToArray()) || Text.StartsWithAny(PredefinedArrays.OpenWrappers.ToArray()))
                {
                    Text = Text.TrimAny(ContinuouslyRemove, PredefinedArrays.WordWrappers.ToArray());
                }
            }
            else
            {
                if (OpenQuoteChar.ToString().IsCloseWrapChar())
                {
                    OpenQuoteChar = OpenQuoteChar.ToString().GetOppositeWrapChar().FirstOrDefault();
                }

                Text = Text.TrimAny(ContinuouslyRemove, $"{OpenQuoteChar}", OpenQuoteChar.ToString().GetOppositeWrapChar());
            }

            return Text;
        }

        public static string UnWrap(this string Text, string WrapText = "\"", bool ContinuouslyRemove = false) => Text.TrimAny(ContinuouslyRemove, WrapText);

        /// <summary>
        /// Decoda uma string de uma transmissão por URL
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string UrlDecode(this string Text)
        {
            if (Text.IsNotBlank())
            {
                return System.Net.WebUtility.UrlDecode(Text);
            }

            return "";
        }

        /// <summary>
        /// Encoda uma string para transmissão por URL
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string UrlEncode(this string Text)
        {
            if (Text.IsNotBlank())
            {
                return System.Net.WebUtility.UrlEncode(Text);
            }

            return "";
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="WrapText">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string WrapText = "\"") => Text.Wrap(WrapText, WrapText);

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string OpenWrapText, string CloseWrapText) => $"{OpenWrapText}{Text}{CloseWrapText.IfBlank(OpenWrapText)}";

        public static HtmlTag WrapInTag(this string Text, string TagName) => new HtmlTag() { InnerHtml = Text, TagName = TagName };
    }
}