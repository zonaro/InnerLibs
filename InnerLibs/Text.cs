using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using InnerLibs.LINQ;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{
    /// <summary>
    /// Modulo de manipulação de Texto
    /// </summary>
    /// <remarks></remarks>
    public static class Text
    {
        /// <summary>
        /// Encapsula um texto em uma caixa incorporado em comentários CSS
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxTextCSS(this string Text)
        {
            return $"/*{Text.BoxText().Wrap(Environment.NewLine)}*/";
        }

        /// <summary>
        /// Encapsula um texto em uma caixa
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxText(this string Text)
        {
            var Lines = Text.SplitAny(Arrays.BreakLineChars.ToArray()).ToList();
            string linha_longa = "";
            int charcount = Lines.Max(x => x.Length);
            if (charcount.IsEven())
            {
                charcount = charcount + 1;
            }

            for (int i = 0, loopTo = Lines.Count - 1; i <= loopTo; i++)
                Lines[i] = Lines[i].PadRight(charcount);
            for (int i = 0, loopTo1 = Lines.Count - 1; i <= loopTo1; i++)
                Lines[i] = $"* {Lines[i]} *";
            charcount = Lines.Max(x => x.Length);
            while (linha_longa.Length < charcount)
                linha_longa = linha_longa + "* ";
            linha_longa = linha_longa.Trim();
            Lines.Insert(0, linha_longa);
            Lines.Add(linha_longa);
            string box = Lines.JoinString(Environment.NewLine);
            return box;
        }

        public static FormattableString ToFormattableString(this string Text, params object[] args)
        {
            return FormattableStringFactory.Create(Text, args ?? Array.Empty<object>());
        }

        public static FormattableString ToFormattableString(this string Text, IEnumerable<object[]> args)
        {
            return FormattableStringFactory.Create(Text, args ?? Array.Empty<object[]>());
        }

        /// <summary>
        /// Parseia uma ConnectionString em um Dicionário
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static ConnectionStringParser ParseConnectionString(this string ConnectionString)
        {
            try
            {
                return new ConnectionStringParser(ConnectionString);
            }
            catch (Exception ex)
            {
                return new ConnectionStringParser();
            }
        }

        public static HtmlTag WrapInTag(this string Text, string TagName)
        {
            return new HtmlTag() { InnerHtml = Text, TagName = TagName };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="querystring"></param>
        /// <returns></returns>
        public static NameValueCollection ParseQueryString(this string Querystring)
        {
            var queryParameters = new NameValueCollection();
            var querySegments = Querystring.Split('&');
            foreach (string segment in querySegments)
            {
                var parts = segment.Split('=');
                if (parts.Any())
                {
                    string key = parts[0].Trim(new char[] { '?', ' ' });
                    string val = "";
                    if (parts.Skip(1).Any())
                    {
                        val = parts[1].Trim();
                    }

                    queryParameters.Add(key, val.UrlDecode());
                }
            }

            return queryParameters;
        }

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, IEnumerable<string> Patterns)
        {
            Text = Text.IfBlank("");
            foreach (var item in Patterns ?? Array.Empty<string>())
            {
                if (LikeOperator.LikeString(item, Text, CompareMethod.Binary) | LikeOperator.LikeString(Text, item, CompareMethod.Binary))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, params string[] Patterns)
        {
            return Text.IsLikeAny((Patterns ?? Array.Empty<string>()).AsEnumerable());
        }

        /// <summary>
        /// operador LIKE do VB para C# em forma de extension method
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool Like(this string Text, string OtherText)
        {
            return LikeOperator.LikeString(Text, OtherText, CompareMethod.Binary);
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

            if (Document.ToString().IsValidCNPJ())
            {
                return Document.FormatCNPJ();
            }

            return Document.ToString();
        }

        /// <summary>
        /// Formata um numero para CNPJ
        /// </summary>
        /// <param name="CNPJ"></param>
        /// <returns></returns>
        public static string FormatCNPJ(this long CNPJ)
        {
            return string.Format(@"{0:00\.000\.000\/0000\-00}", CNPJ);
        }

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
                    CNPJ = CNPJ.ToLong().FormatCNPJ();
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
        public static string FormatCPF(this long CPF)
        {
            return string.Format(@"{0:000\.000\.000\-00}", CPF);
        }

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
                    CPF = CPF.ToLong().FormatCPF();
            }
            else
            {
                throw new FormatException("String is not a valid CPF");
            }

            return CPF;
        }

        /// <summary>
        /// Retorna a string especificada se o valor booleano for verdadeiro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="BooleanValue"></param>
        /// <returns></returns>
        public static string PrintIf(this string Text, bool BooleanValue)
        {
            return BooleanValue ? Text : "";
        }

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
        public static string[] SplitAny(this string Text, IEnumerable<string> SplitText)
        {
            return Text.SplitAny(SplitText.ToArray());
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

        public static string AdjustBlankSpaces(this string Text)
        {
            return Text.AdjustWhiteSpaces();
        }

        public static string AdjustWhiteSpaces(this string Text)
        {
            Text = Text.IfBlank("");
            if (Text.IsNotBlank())
            {
                // adiciona espaco quando nescessario
                Text = Text.Replace(")", ") ");
                Text = Text.Replace("]", "] ");
                Text = Text.Replace("}", "} ");
                Text = Text.Replace(">", "> ");
                Text = Text.Replace("(", " (");
                Text = Text.Replace("<", " <");
                Text = Text.Replace("[", " [");
                Text = Text.Replace("{", " {");
                Text = Text.Replace(":", ": ");
                Text = Text.Replace(";", "; ");
                foreach (var item in Arrays.AlphaChars)
                    Text = Text.SensitiveReplace($" -{item}", $" - {item}");
                Text = Text.Replace("- ", " - ");
                Text = Text.Replace("\"", " \"");

                // remove espaco quando nescessario
                Text = Text.Replace(" ,", ",");
                Text = Text.Replace(" .", ".");
                Text = Text.Replace(" !", "!");
                Text = Text.Replace(" ?", "?");
                Text = Text.Replace(" ;", ";");
                Text = Text.Replace(" :", ":");
                Text = Text.Replace(" )", ")");
                Text = Text.Replace(" ]", "]");
                Text = Text.Replace(" }", "}");
                Text = Text.Replace(" >", ">");
                Text = Text.Replace("( ", "(");
                Text = Text.Replace("[ ", "[");
                Text = Text.Replace("{ ", "{");
                Text = Text.Replace("< ", "<");
                Text = Text.Replace("\" ", "\"");
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

        public static string AppendUrlParameter(this string Url, string Key, params string[] Value)
        {
            foreach (var v in Value ?? Array.Empty<string>())
                Url += string.Format("&{0}={1}", Key, v.IfBlank(""));
            return Url;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string com uma quebra de linha no final do <paramref name="AppendText"/>
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string AppendLine(this string Text, string AppendText)
        {
            return Text.Append(AppendText).Append(Environment.NewLine);
        }

        /// <summary>
        /// Adiciona texto ao inicio de uma string com uma quebra de linha no final do <paramref name="AppendText"/>
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string PrependLine(this string Text, string AppendText)
        {
            return Text.Prepend(Environment.NewLine).Prepend(AppendText);
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">      Teste</param>
        public static string AppendIf(this string Text, string AppendText, bool Test)
        {
            if (Test)
            {
                return Text.Append(AppendText);
            }

            return Text ?? "";
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">      Teste</param>
        public static string AppendIf(this string Text, string AppendText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);
            return Text.AppendIf(AppendText, Test(Text));
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">      Teste</param>
        public static string PrependIf(this string Text, string PrependText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);
            return Text.PrependIf(PrependText, Test(Text));
        }

        /// <summary>
        /// Adiciona texto ao inicio de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">      Teste</param>
        public static string PrependWhile(this string Text, string PrependText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);

            while (Test(Text))
                Text = Text.Prepend(PrependText);
            return Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">      Teste</param>
        public static string AppendWhile(this string Text, string AppendText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);
            while (Test(Text))
                Text = Text.Append(AppendText);
            return Text;
        }

        /// <summary>
        /// Aplica espacos em todos os caracteres de encapsulamento
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ApplySpaceOnWrapChars(this string Text)
        {
            foreach (var c in Arrays.WordWrappers)
                Text = Text.Replace(c, " " + c + " ");
            return Text;
        }

        /// <summary>
        /// Transforma um texto em CamelCase em um array de palavras  a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> CamelSplit(this string Text)
        {
            return Text.CamelAdjust().Split(" ");
        }

        /// <summary>
        /// Separa as palavras de um texto CamelCase a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string CamelAdjust(this string Text)
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

                    uppercount = uppercount + 1;
                }
                else
                {
                    if (uppercount > 1)
                    {
                        Text += " ";
                    }

                    uppercount = 0;
                }

                Text += Conversions.ToString(c);
            }

            return Text.Trim();
        }

        /// <summary>
        /// Pega um texto em "CamelCase" ou "Snake_Case" e o retorna na forma "normal case"
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToNormalCase(this string Text)
        {
            return Text.CamelAdjust().Replace("_", " ");
        }

        /// <summary>
        /// Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro
        /// caractere desejado) e retorna um valor indicando se o texto precisou ser censurado
        /// </summary>
        /// <param name="Text">               Texto</param>
        /// <param name="BadWords">           Lista de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        /// <returns>TRUE se a frase precisou ser censurada, FALSE se a frase não precisou de censura</returns>
        public static string Censor(this string Text, IEnumerable<string> BadWords, string CensorshipCharacter, ref bool IsCensored)
        {
            var words = Text.Split(" ", StringSplitOptions.None);
            BadWords = BadWords ?? Array.Empty<string>();
            if (words.ContainsAny(BadWords))
            {
                foreach (var bad in BadWords)
                {
                    string censored = "";
                    for (int index = 1, loopTo = bad.Length; index <= loopTo; index++)
                        censored += CensorshipCharacter;
                    for (int index = 0, loopTo1 = words.Length - 1; index <= loopTo1; index++)
                    {
                        if ((words[index].RemoveDiacritics().RemoveAny(Arrays.WordSplitters.ToArray()).ToLower() ?? "") == (bad.RemoveDiacritics().RemoveAny(Arrays.WordSplitters.ToArray()).ToLower() ?? ""))
                        {
                            words[index] = words[index].ToLower().Replace(bad, censored);
                            IsCensored = true;
                        }
                    }
                }

                Text = words.JoinString(" ");
            }

            return Text;
        }

        /// <summary>
        /// Retorna um novo texto censurando as palavras de um texto substituindo as palavras indesejadas
        /// por um caractere desejado)
        /// </summary>
        /// <param name="Text">               Texto</param>
        /// <param name="BadWords">           Array de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        public static string Censor(this string Text, string CensorshipCharacter, params string[] BadWords)
        {
            bool argIsCensored = false;
            return Text.Censor((BadWords ?? Array.Empty<string>()).ToList(), CensorshipCharacter, IsCensored: ref argIsCensored);
        }

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">  Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter a maioria dos valores, false se não</returns>
        public static bool ContainsMost(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            return (Values ?? Array.Empty<string>()).Most(value => Text != null && Text.Contains(value, ComparisonType));
        }

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">  Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsMost(this string Text, params string[] Values)
        {
            return Text.ContainsMost(StringComparison.InvariantCultureIgnoreCase, Values);
        }

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">  Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsAll(this string Text, params string[] Values)
        {
            return Text.ContainsAll(StringComparison.InvariantCultureIgnoreCase, Values);
        }

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">          Texto correspondente</param>
        /// <param name="Values">        Lista de valores</param>
        /// <param name="ComparisonType">Tipo de comparacao</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAll(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            Values = Values ?? Array.Empty<string>();
            if (Values.Any())
            {
                foreach (string value in Values)
                {
                    if (Text is null || Text.IndexOf(value, ComparisonType) == -1)
                    {
                        return false;
                    }
                }

                return true;
            }

            return Text.IsBlank();
        }

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">  Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAny(this string Text, params string[] Values)
        {
            return Text.ContainsAny(StringComparison.InvariantCultureIgnoreCase, Values);
        }

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">          Texto correspondente</param>
        /// <param name="Values">        Lista de valores</param>
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

        public static bool ContainsAnyWords(this string Text, params string[] Words)
        {
            return Text.ContainsAnyWords(null, Words);
        }

        public static bool ContainsAnyWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words)
        {
            return Text.GetWords().ContainsAny(Words, Comparer);
        }

        public static bool ContainsAllWords(this string Text, params string[] Words)
        {
            return Text.ContainsAllWords(null, Words);
        }

        public static bool ContainsAllWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words)
        {
            return Text.GetWords().ContainsAll(Words, Comparer);
        }

        /// <summary>
        /// Conta os caracters especificos de uma string
        /// </summary>
        /// <param name="Text">     Texto</param>
        /// <param name="Character">Caractere</param>
        /// <returns></returns>
        public static int CountCharacter(this string Text, char Character)
        {
            return Text.Count((c) => c == Character);
        }

        /// <summary>
        /// Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade
        /// </summary>
        /// <param name="Text">            TExto</param>
        /// <param name="RemoveDiacritics">indica se os acentos devem ser removidos das palavras</param>
        /// <param name="Words">
        /// Desconsidera outras palavras e busca a quantidadade de cada palavra especificada em um array
        /// </param>
        /// <returns></returns>
        public static Dictionary<string, long> CountWords(this string Text, bool RemoveDiacritics = true, string[] Words = null)
        {
            if (Words is null)
                Words = Array.Empty<string>();
            var palavras = Text.Split(Arrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
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
                dic.Add(w, 0L);
            return dic;
        }

        /// <summary>
        /// Remove uma linha especifica de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="LineIndex">Numero da linha</param>
        /// <returns></returns>
        public static string DeleteLine(this string Text, int LineIndex)
        {
            var parts = new List<string>();
            var strReader = new StringReader(Text);
            string NewText = "";
            LineIndex = LineIndex.SetMinValue(0);
            while (true)
            {
                NewText = strReader.ReadLine();
                if (NewText is null)
                {
                    break;
                }
                else
                {
                    parts.Add(NewText);
                }
            }

            NewText = "";
            if (parts.Count > LineIndex)
            {
                parts.RemoveAt(LineIndex);
            }

            foreach (var part in parts)
                NewText = NewText + part + Environment.NewLine;
            return NewText;
        }

        /// <summary>
        /// Cria um dicionário com as palavras de uma lista e a quantidade de cada uma.
        /// </summary>
        /// <param name="List">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(params string[] List)
        {
            return List.ToList().DistinctCount();
        }

        /// <summary>
        /// Cria um dicionário com as palavras de uma frase e sua respectiva quantidade.
        /// </summary>
        /// <param name="Phrase">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(this string Phrase)
        {
            return Phrase.Split(" ").ToList().DistinctCount();
        }

        /// <summary>
        /// Verifica se uma string termina com alguma outra string de um array
        /// </summary>
        /// <param name="Text"> </param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string Text, params string[] Words)
        {
            return Words.Any(p => Text.EndsWith(p));
        }

        /// <summary>
        /// Prepara uma string com aspas simples para uma Query TransactSQL
        /// </summary>
        /// <param name="Text">Texto a ser tratado</param>
        /// <returns>String pornta para a query</returns>
        public static string EscapeQuotesToQuery(this string Text)
        {
            return Text.Replace("'", "''");
        }

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
        /// Procura CEPs em uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static string[] FindCEP(this string Text)
        {
            return Text.FindByRegex(@"\d{5}-\d{3}").Union(Text.FindNumbers().Where(x => x.Length == 8)).ToArray();
        }

        /// <summary>
        /// Procura CEPs em uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static string[] FindByRegex(this string Text, string Regex, RegexOptions RegexOptions = RegexOptions.None)
        {
            var textos = new List<string>();
            foreach (Match m in new Regex(Regex, RegexOptions).Matches(Text))
                textos.Add(m.Value);
            return textos.ToArray();
        }

        /// <summary>
        /// Procurea numeros de telefone em um texto
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string[] FindTelephoneNumbers(this string Text)
        {
            return Text.FindByRegex(@"\b[\s()\d-]{6,}\d\b", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Select(x => x.MaskTelephoneNumber()).ToArray();
        }

        /// <summary>
        /// Transforma quebras de linha HTML em quebras de linha comuns ao .net
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <returns>String fixada</returns>
        public static string FixBreakLines(this string Text)
        {
            return Text.ReplaceMany(Constants.vbCr + Constants.vbLf, "<br/>", "<br />", "<br>");
            return Text.Replace("&nbsp;", " ");
        }

        public static string FixCaptalization(this string Text)
        {
            Text = Text.Trim().GetFirstChars(1).ToUpper() + Text.RemoveFirstChars(1);
            var dots = new[] { "...", ". ", "? ", "! " };
            List<string> sentences;
            foreach (var dot in dots)
            {
                sentences = Text.Split(dot, StringSplitOptions.None).ToList();
                for (int index = 0, loopTo = sentences.Count - 1; index <= loopTo; index++)
                    sentences[index] = "" + sentences[index].Trim().GetFirstChars(1).ToUpper() + sentences[index].RemoveFirstChars(1);
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
        /// Adciona pontuaçao ao final de uma string se a mesma não terminar com alguma pontuacao.
        /// </summary>
        /// <param name="Text">       Frase, Texto a ser pontuado</param>
        /// <param name="Punctuation">Ponto a ser adicionado na frase se a mesma não estiver com pontuacao</param>
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
                Text = Text + Punctuation;
            }

            return Text;
        }

        /// <summary>
        /// Arruma a ortografia do texto captalizando corretamente, adcionando pontução ao final de frase
        /// caso nescessário e removendo espaços excessivos ou incorretos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string FixText(this string Text, int Ident = 0, int BreakLinesBetweenParagraph = 0)
        {
            return new StructuredText(Text) { Ident = Ident, BreakLinesBetweenParagraph = BreakLinesBetweenParagraph }.ToString();
        }

        /// <summary>
        /// Extension Method para <see cref="String.Format(String,Object())"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Args">Objetos de substituição</param>
        /// <returns></returns>
        public static string Format(this string Text, params string[] Args)
        {
            return string.Format(Text, Args);
        }

        /// <summary>
        /// Retorna um texto posterior a outro
        /// </summary>
        /// <param name="Text"> Texto correspondente</param>
        /// <param name="Value">Texto Posterior</param>
        /// <returns>Uma string com o valor posterior ao valor especificado.</returns>
        public static string GetAfter(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            if (string.IsNullOrEmpty(Value))
                Value = "";
            if (string.IsNullOrEmpty(Text) || Text.IndexOf(Value) == -1)
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

            return Text.Substring(Text.IndexOf(Value) + Value.Length);
        }

        /// <summary>
        /// Retorna todas as ocorrencias de um texto entre dois textos
        /// </summary>
        /// <param name="Text">  O texto correspondente</param>
        /// <param name="Before">O texto Anterior</param>
        /// <param name="After"> O texto Posterior</param>
        /// <returns>Uma String com o texto entre o texto anterior e posterior</returns>
        public static string[] GetAllBetween(this string Text, string Before, string After = "")
        {
            var lista = new List<string>();
            string regx = Before.RegexEscape() + "(.*?)" + After.IfBlank(Before).RegexEscape();
            var mm = new Regex(regx, (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Matches(Text);
            foreach (Match a in mm)
                lista.Add(a.Value.RemoveFirstEqual(Before).RemoveLastEqual(After));
            return lista.ToArray();
        }

        /// <summary>
        /// Retorna um texto anterior a outro
        /// </summary>
        /// <param name="Text"> Texto correspondente</param>
        /// <param name="Value">Texto Anterior</param>
        /// <returns>Uma string com o valor anterior ao valor especificado.</returns>
        public static string GetBefore(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            if (Value is null)
                Value = "";
            if (Text is null || Text.IndexOf(Value) == -1)
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
        /// <param name="Text">  O texto correspondente</param>
        /// <param name="Before">O texto Anterior</param>
        /// <param name="After"> O texto Posterior</param>
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
        /// Pega o dominio principal de uma URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomain(this string URL, bool RemoveFirstSubdomain = false)
        {
            return new Uri(URL).GetDomain(RemoveFirstSubdomain);
        }

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
        /// <param name="Text">  </param>
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
                    catch (Exception ex)
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
                case "\"":
                    {
                        return "\"";
                    }

                case "'":
                    {
                        return "'";
                    }

                case "(":
                    {
                        return ")";
                    }

                case ")":
                    {
                        return "(";
                    }

                case "[":
                    {
                        return "]";
                    }

                case "]":
                    {
                        return "[";
                    }

                case "{":
                    {
                        return "}";
                    }

                case "}":
                    {
                        return "{";
                    }

                case "<":
                    {
                        return ">";
                    }

                case ">":
                    {
                        return "<";
                    }

                case @"\":
                    {
                        return "/";
                    }

                case "/":
                    {
                        return @"\";
                    }

                case "¿":
                    {
                        return "?";
                    }

                case "?":
                    {
                        return "¿";
                    }

                case "!":
                    {
                        return "¡";
                    }

                case "¡":
                    {
                        return "!";
                    }

                case ".":
                    {
                        return ".";
                    }

                case ":":
                    {
                        return ":";
                    }

                case ";":
                    {
                        return ";";
                    }

                case "_":
                    {
                        return "_";
                    }

                case "*":
                    {
                        return "*";
                    }

                default:
                    {
                        return Text;
                    }
            }
        }

        /// <summary>
        /// Retorna o caractere de encapsulamento oposto ao caractere indicado
        /// </summary>
        /// <param name="Text">Caractere</param>
        /// <returns></returns>
        public static string IsOpenWrapChar(this string Text)
        {
            return Conversions.ToString(Text.GetFirstChars().IsIn(Arrays.OpenWrappers));
        }

        public static string IsCloseWrapChar(this string Text)
        {
            return Conversions.ToString(Text.GetFirstChars().IsIn(Arrays.CloseWrappers));
        }

        /// <summary>
        /// Sorteia um item da Lista
        /// </summary>
        /// <typeparam name="Type">Tipo de lista</typeparam>
        /// <param name="List">Lista</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static Type GetRandomItem<Type>(this IEnumerable<Type> List)
        {
            return List.ToArray()[Generate.RandomNumber(0, List.Count() - 1)];
        }

        /// <summary>
        /// Sorteia um item da Lista
        /// </summary>
        /// <typeparam name="Type">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static Type GetRandomItem<Type>(this Type[] Array)
        {
            return Array[Generate.RandomNumber(0, Array.Count() - 1)];
        }

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this Uri URL)
        {
            return URL.PathAndQuery;
        }

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this string URL)
        {
            if (URL.IsURL())
                return new Uri(URL).GetRelativeURL();
            return null;
        }

        /// <summary>
        /// Retorna uma lista de palavras encontradas no texto em ordem alfabetica
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<string> GetWords(this string Text)
        {
            var txt = new List<string>();
            var palavras = Text.AdjustWhiteSpaces().FixBreakLines().ToLower().RemoveHTML().Split(Arrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var w in palavras)
                txt.Add(w);
            return txt.Distinct().OrderBy(x => x);
        }

        /// <summary>
        /// Captura todas as sentenças que estão entre aspas ou parentesis ou chaves ou colchetes em um texto
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
        public static string HtmlDecode(this string Text)
        {
            return System.Net.WebUtility.HtmlDecode("" + Text).ReplaceMany(Constants.vbCr + Constants.vbLf, "<br/>", "<br />", "<br>");
        }

        /// <summary>
        /// Escapa o texto HTML
        /// </summary>
        /// <param name="Text">string HTML</param>
        /// <returns>String HTML corrigido</returns>
        public static string HtmlEncode(this string Text)
        {
            return System.Net.WebUtility.HtmlEncode("" + Text.ReplaceMany("<br>", Conversions.ToString(Arrays.BreakLineChars)));
        }

        /// <summary>
        /// Verifica se uma palavra é um Anagrama de outra palavra
        /// </summary>
        /// <param name="Text">       </param>
        /// <param name="AnotherText"></param>
        /// <returns></returns>
        public static bool IsAnagramOf(this string Text, string AnotherText)
        {
            var char1 = Text.ToLower().ToCharArray();
            var char2 = Text.ToLower().ToCharArray();
            Array.Sort(char1);
            Array.Sort(char2);
            string NewWord1 = new string(char1);
            string NewWord2 = new string(char2);
            return (NewWord1 ?? "") == (NewWord2 ?? "");
        }

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text"> string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, params string[] Texts)
        {
            return Text.IsAny(StringComparison.CurrentCultureIgnoreCase, Texts);
        }

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text"> string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, StringComparison Comparison, params string[] Texts)
        {
            return (Texts ?? Array.Empty<string>()).Any(x => Text.Equals(x, Comparison));
        }

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text"> string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se nenhuma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, params string[] Texts)
        {
            return !Text.IsAny(Texts);
        }

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text"> string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, StringComparison Comparison, params string[] Texts)
        {
            return !Text.IsAny(Comparison, Texts);
        }

        /// <summary>
        /// Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da esqueda
        /// para direita
        /// </summary>
        /// <param name="Text">             Texto</param>
        /// <param name="IgnoreWhiteSpaces">Ignora os espaços na hora de comparar</param>
        /// <returns></returns>
        public static bool IsPalindrome(this string Text, bool IgnoreWhiteSpaces = true)
        {
            if (IgnoreWhiteSpaces)
                Text = Text.RemoveAny(" ");
            var c = Text.ToArray();
            var p = c;
            Array.Reverse(p);
            return new string(p ?? new char[0]) == new string(c ?? new char[0]);
        }

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Items">    Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(this IEnumerable<Type> Items, string Separator = "") 
        {
            Items ??= Array.Empty<Type>().AsEnumerable();
            return string.Join(Separator, Items.Select(x => $"{x}").ToArray());
        }

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Array">    Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(this Type[] Array, string Separator = "") 
        {
            return JoinString(Array.AsEnumerable(), Separator);
        }

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Array">    Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(string Separator, params Type[] Array) 
        {
            return JoinString(Array, Separator);
        }

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="List">     Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string JoinString<Type>(this List<Type> List, string Separator = "") 
        {
            return List.ToArray().JoinString(Separator);
        }

        /// <summary>
        /// Verifica se um texto contém outro ou vice versa
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool CrossContains(this string Text, string OtherText, StringComparison StringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return Text.Contains(OtherText, StringComparison) || OtherText.Contains(Text, StringComparison);
        }

        /// <summary>
        /// Verifica se um texto contém outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool Contains(this string Text, string OtherText, StringComparison StringComparison)
        {
            return Text.IndexOf(OtherText, StringComparison) > -1;
        }

        /// <summary>
        /// Computa a distancia de Levenshtein entre 2 strings.
        /// </summary>
        public static int LevenshteinDistance(this string Text1, string Text2)
        {
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
                d[i, 0] = i;
            for (int j = 0, loopTo1 = m; j <= loopTo1; j++)
                d[0, j] = j;

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
        /// limpa um texto deixando apenas os caracteres alfanumericos.
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ParseAlphaNumeric(this string Text)
        {
            var l = new List<string>();
            foreach (var item in Text.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                l.Add(Regex.Replace(item, "[^A-Za-z0-9]", ""));
            return l.JoinString(" ");
        }

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
                return strDigits;
            foreach (char c in Text.ToCharArray())
            {
                if (char.IsDigit(c) || c == Convert.ToChar(Culture.NumberFormat.NumberDecimalSeparator))
                {
                    strDigits += Conversions.ToString(c);
                }
            }

            return strDigits;
        }

        public static Type ParseDigits<Type>(this string Text, CultureInfo Culture = null) where Type : IConvertible
        {
            return Text.ParseDigits(Culture).ChangeType<Type, string>();
        }

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
                if (!Text.GetFirstChars((int)Math.Round(l)).Last().ToString().ToLower().IsIn(Arrays.LowerVowels))
                {
                    l = l.ChangeType<int, decimal>() - 1;
                }

                p.Add(Text.GetFirstChars((int)Math.Round(l)).Trim() + Text.GetFirstChars((int)Math.Round(l)).Reverse().ToList().JoinString().ToLower().Trim() + Text.RemoveFirstChars((int)Math.Round(l)).RemoveFirstAny(Arrays.LowerConsonants.ToArray()));
            }

            return p.ToArray();
        }

        /// <summary>
        /// Retorna uma string em sua forma poop
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Poopfy(this string Text)
        {
            return Poopfy(Text.Split(" ")).JoinString(" ");
        }

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
            catch (XmlException generatedExceptionName)
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
        /// <param name="Text">       Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        public static string Prepend(this string Text, string PrependText)
        {
            Text = (PrependText ?? "") + (Text ?? "");
            return Text;
        }

        /// <summary>
        /// Adiciona texto ao fim de uma string
        /// </summary>
        /// <param name="Text">       Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string Append(this string Text, string AppendText)
        {
            Text = (Text ?? "") + (AppendText ?? "");
            return Text;
        }

        /// <summary>
        /// Adiciona texto ao começo de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">       Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">       Teste</param>
        public static string PrependIf(this string Text, string PrependText, bool Test)
        {
            if (Test)
            {
                Text = Text.Prepend(PrependText);
            }

            return Text;
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade determinada em uma lista ou um valor numérico encontrado no primeiro parametro.
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
                    str = str.Replace($"{{{index}}}", Conversions.ToString(PluralText.GetArgument(index)));
                return str;
            }

            return PluralText?.ToString();
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">  Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this string PluralText, object Quantity)
        {
            decimal d = 0m;
            return PluralText.QuantifyText(Quantity, ref d);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="QuantityOrListOrBoolean">  Quantidade de Itens</param>
        /// <param name="OutQuantity">Devolve a quantidade encontrada em <paramref name="QuantityOrListOrBoolean"/> </param>
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
                        break;
                    }

                case object _ when QuantityOrListOrBoolean.IsNumber():
                    {
                        OutQuantity = Conversions.ToDecimal(QuantityOrListOrBoolean);
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
                        OutQuantity = Conversions.ToDecimal(QuantityOrListOrBoolean);
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
        public static string QuantifyText<T>(this IEnumerable<T> List, string PluralText)
        {
            return PluralText.QuantifyText(List ?? Array.Empty<T>());
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this int Quantity, string PluralText)
        {
            return PluralText.QuantifyText(Quantity);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this decimal Quantity, string PluralText)
        {
            return PluralText.QuantifyText(Quantity);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this short Quantity, string PluralText)
        {
            return PluralText.QuantifyText(Quantity);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this long Quantity, string PluralText)
        {
            return PluralText.QuantifyText(Quantity);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this double Quantity, string PluralText)
        {
            return PluralText.QuantifyText(Quantity);
        }

        /// <summary>
        /// Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes)
        /// </summary>
        /// <param name="Text">     Texto</param>
        /// <param name="OpenQuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Quote(this string Text, char OpenQuoteChar = '"')
        {
            if (Conversions.ToBoolean(OpenQuoteChar.ToString().IsCloseWrapChar()))
            {
                OpenQuoteChar = Conversions.ToChar(OpenQuoteChar.ToString().GetOppositeWrapChar());
            }

            return OpenQuoteChar + Text + OpenQuoteChar.ToString().GetOppositeWrapChar();
        }

        public static string UnQuote(this string Text)
        {
            return Text.UnQuote("", true);
        }

        public static string UnQuote(this string Text, string OpenQuoteChar, bool ContinuouslyRemove = false)
        {
            if (OpenQuoteChar.IsBlank())
            {
                while (Text.EndsWithAny(Arrays.CloseWrappers.ToArray()) || Text.StartsWithAny(Arrays.OpenWrappers.ToArray()))
                    Text = Text.TrimAny(ContinuouslyRemove, Arrays.WordWrappers.ToArray());
            }
            else
            {
                if (Conversions.ToBoolean(OpenQuoteChar.ToString().IsCloseWrapChar()))
                {
                    OpenQuoteChar = OpenQuoteChar.ToString().GetOppositeWrapChar();
                }

                Text = Text.TrimAny(ContinuouslyRemove, OpenQuoteChar, OpenQuoteChar.ToString().GetOppositeWrapChar());
            }

            return Text;
        }

        /// <summary>
        /// Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes) é um alias de <see cref="Quote(String, Char)"/>
        /// </summary>
        /// <param name="Text">     Texto</param>
        /// <param name="BracketChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Brackfy(this string Text, char BracketChar = '{')
        {
            return Text.Quote(BracketChar);
        }

        public static string UnBrackfy(this string Text)
        {
            return Text.UnBrackfy("", true);
        }

        public static string UnBrackfy(this string Text, string BracketChar, bool ContinuouslyRemove = false)
        {
            return Text.UnQuote(BracketChar, ContinuouslyRemove);
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes) se uma
        /// condiçao for cumprida
        /// </summary>
        /// <param name="Text">     Texto</param>
        /// <param name="QuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string QuoteIf(this string Text, bool Condition, string QuoteChar = "\"")
        {
            return Condition ? Text.Quote(Conversions.ToChar(QuoteChar)) : Text;
        }

        /// <summary>
        /// Sorteia um item da Matriz
        /// </summary>
        /// <typeparam name="Type">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static Type RandomItem<Type>(params Type[] Array)
        {
            return Array.GetRandomItem();
        }

        /// <summary>
        /// Escapa caracteres exclusivos de uma regex
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string RegexEscape(this string Text)
        {
            var chars = new[] { '.', '$', '^', '{', '[', '(', '|', ')', '*', '+', '?', '|' };
            string newstring = "";
            foreach (var c in Text.ToArray())
            {
                if (c.IsIn(chars))
                {
                    newstring += @"\" + c;
                }
                else
                {
                    newstring += Conversions.ToString(c);
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

                k = k + 1;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Remove várias strings de uma string
        /// </summary>
        /// <param name="Text">  Texto</param>
        /// <param name="Values">Strings a serem removidas</param>
        /// <returns>Uma string com os valores removidos</returns>
        public static string RemoveAny(this string Text, params string[] Values)
        {
            Text = Text.ReplaceMany("", Values ?? Array.Empty<string>());
            return Text;
        }

        /// <summary>
        /// Remove os acentos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String sem os acentos</returns>
        public static string RemoveDiacritics(this string Text)
        {
            Text = Text.RemoveAccents();
            return Text;
        }

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">              Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as ocorrencias
        /// sejam removidas
        /// </param>
        /// <param name="StartStringTest">   Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveFirstAny(this string Text, bool ContinuouslyRemove, params string[] StartStringTest)
        {
            string re = Text;
            while (re.StartsWithAny(StartStringTest))
            {
                foreach (var item in StartStringTest)
                {
                    if (re.StartsWith(item))
                    {
                        re = re.RemoveFirstEqual(item);
                        if (!ContinuouslyRemove)
                            return re;
                    }
                }
            }

            return re;
        }

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">              Texto</param>
        /// <param name="StartStringTest">     Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveFirstAny(this string Text, params string[] StartStringTest)
        {
            return Text.RemoveFirstAny(true, StartStringTest);
        }

        /// <summary>
        /// Remove os X primeiros caracteres
        /// </summary>
        /// <param name="Text">     Texto</param>
        /// <param name="Quantity"> Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveFirstChars(this string Text, int Quantity = 1)
        {
            if (Text.Length > Quantity)
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
        /// <param name="Text">           Texto</param>
        /// <param name="StartStringTest">Texto inicial que será comparado</param>
        public static string RemoveFirstEqual(this string Text, string StartStringTest)
        {
            if (Text.StartsWith(StartStringTest))
            {
                Text = Text.RemoveFirstChars(StartStringTest.Length);
            }

            return Text;
        }

        public static string RemoveHTML(this string Text)
        {
            if (Text.IsNotBlank())
            {
                return Regex.Replace(Text, "<.*?>", string.Empty).HtmlDecode();
            }

            return "";
        }

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">              Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as ocorrencias
        /// sejam removidas
        /// </param>
        /// <param name="EndStringTest">     Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveLastAny(this string Text, bool ContinuouslyRemove, params string[] EndStringTest)
        {
            string re = Text;
            while (re.EndsWithAny(EndStringTest))
            {
                foreach (var item in EndStringTest)
                {
                    if (re.EndsWith(item))
                    {
                        re = re.RemoveLastEqual(item);
                        if (!ContinuouslyRemove)
                            return re;
                    }
                }
            }

            return re;
        }

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">              Texto</param>
        /// <param name="EndStringTest">     Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string RemoveLastAny(this string Text, params string[] EndStringTest)
        {
            return Text.RemoveLastAny(true, EndStringTest);
        }

        /// <summary>
        /// Remove os X ultimos caracteres
        /// </summary>
        /// <param name="Text">    Texto</param>
        /// <param name="Quantity">Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveLastChars(this string Text, int Quantity = 1)
        {
            return Text.Substring(0, Text.Length - Quantity);
        }

        /// <summary>
        /// Remove um texto do final de uma string se ele for um outro texto
        /// </summary>
        /// <param name="Text">         Texto</param>
        /// <param name="EndStringTest">Texto final que será comparado</param>
        public static string RemoveLastEqual(this string Text, string EndStringTest)
        {
            if (Text.EndsWith(EndStringTest))
            {
                Text = Text.RemoveLastChars(EndStringTest.Length);
            }

            return Text;
        }

        /// <summary>
        /// Remove caracteres não printaveis de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String corrigida</returns>
        public static string RemoveNonPrintable(this string Text)
        {
            foreach (char c in Text.ToCharArray())
            {
                if (char.IsControl(c))
                {
                    Text = Text.ReplaceNone(Conversions.ToString(c));
                }
            }

            return Text.Trim();
        }

        /// <summary>
        /// Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum
        /// </summary>
        /// <param name="Strings">        Array de strings</param>
        /// <param name="OldValue">       Valor antigo que será substituido</param>
        /// <param name="NewValue">       Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE realiza
        /// um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
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
        /// <param name="Strings">        Array de strings</param>
        /// <param name="OldValue">       Valor antigo que será substituido</param>
        /// <param name="NewValue">       Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE realiza
        /// um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
        /// </param>
        /// <returns></returns>
        public static List<string> Replace(this List<string> Strings, string OldValue, string NewValue, bool ReplaceIfEquals = true)
        {
            return Strings.ToArray().Replace(OldValue, NewValue, ReplaceIfEquals).ToList();
        }

        /// <summary>
        /// Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string, string> Dic)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                    Text = Text.Replace(p.Key, p.Value);
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
                                    Text = Text.ReplaceMany(p.Key, Converter.ForceArray(p.Value, typeof(T)).Cast<string>().ToArray());
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
                    Text = Text.SensitiveReplace(p.Key, p.Value);
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
                    Text = Text.SensitiveReplace(p.Value, p.Key.ToArray(), Comparison);
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
                        tos.Add(string.Empty);
                    for (int i = 0, loopTo = froms.Count - 1; i <= loopTo; i++)
                        Text = Text.SensitiveReplace(froms[i], tos[i], Comparison);
                }
            }

            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por um novo valor.
        /// </summary>
        /// <param name="Text">     Texto</param>
        /// <param name="NewValue"> Novo Valor</param>
        /// <param name="OldValues">Valores a serem substituido por um novo valor</param>
        /// <returns></returns>
        public static string ReplaceMany(this string Text, string NewValue, params string[] OldValues)
        {
            Text = Text ?? "";
            foreach (var word in (OldValues ?? Array.Empty<string>()).Where(x => x.Length > 0))
                Text = Text.Replace(word, NewValue);
            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por vazio.
        /// </summary>
        /// <param name="Text">    Texto</param>
        /// <param name="OldValue">Valor a ser substituido por vazio</param>
        /// <returns>String corrigida</returns>
        public static string ReplaceNone(this string Text, string OldValue)
        {
            return Text.Replace(OldValue, "");
        }

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text">          </param>
        /// <param name="NewValue">      </param>
        /// <param name="OldValue">      </param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string OldValue, string NewValue, StringComparison ComparisonType = StringComparison.InvariantCulture)
        {
            return Text.SensitiveReplace(NewValue, new[] { OldValue }, ComparisonType);
        }

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text">          </param>
        /// <param name="NewValue">      </param>
        /// <param name="OldValues">     </param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string NewValue, IEnumerable<string> OldValues, StringComparison ComparisonType = StringComparison.InvariantCulture)
        {
            if (Text.IsNotBlank())
            {
                foreach (var oldvalue in OldValues ?? new[] { "" })
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
        public static Type[] Shuffle<Type>(this Type[] Array)
        {
            return Array.OrderByRandom().ToArray();
        }

        /// <summary>
        /// Randomiza a ordem dos itens de uma Lista
        /// </summary>
        /// <typeparam name="Type">Tipo de Lista</typeparam>
        /// <param name="List">Matriz</param>
        public static List<Type> Shuffle<Type>(this List<Type> List)
        {
            return List.OrderByRandom().ToList();
        }

        /// <summary>
        /// Aleatoriza a ordem das letras de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string Shuffle(this string Text)
        {
            return Generate.RandomWord(Text);
        }

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
                if (endchar.IsAny(Arrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index].RemoveLastEqual(endchar);
                }

                switch (true)
                {
                    case object _ when phrase[index].IsNumber() || phrase[index].IsEmail() || phrase[index].IsURL() || phrase[index].IsIP() || phrase[index].IsIn(Arrays.WordSplitters):
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
                            phrase[index] = phrase[index].RemoveLastEqual("eis") + "el";
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

                if (endchar.IsAny(Arrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index] + endchar;
                }
            }

            return phrase.JoinString(" ").AdjustWhiteSpaces();
        }

        /// <summary>
        /// Corta un texto para exibir um numero máximo de caracteres ou na primeira quebra de linha.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="TextLength"></param>
        /// <param name="Ellipsis"></param>
        /// <returns></returns>
        public static string Slice(this string Text, int TextLength = 0, string Ellipsis = "...")
        {
            if (Text.IsBlank() || Text.Length <= TextLength || TextLength < 1)
            {
                return Text;
            }
            else
            {
                Text = Text.GetBefore(Environment.NewLine);
                return Text.GetFirstChars(TextLength) + Ellipsis;
            }
        }

        /// <summary>
        /// Separa um texto em um array de strings a partir de uma outra string
        /// </summary>
        /// <param name="Text">     Texto</param>
        /// <param name="Separator">Texto utilizado como separador</param>
        /// <returns></returns>
        public static string[] Split(this string Text, string Separator, StringSplitOptions Options = StringSplitOptions.RemoveEmptyEntries)
        {
            return (Text ?? "").Split(new[] { Separator }, Options);
        }

        /// <summary>
        /// Verifica se uma string começa com alguma outra string de um array
        /// </summary>
        /// <param name="Text"> </param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string Text, params string[] Words)
        {
            return Words.Any(p => Text.StartsWith(p));
        }

        /// <summary>
        /// Conta as silabas de uma palavra
        /// </summary>
        /// <param name="Word"></param>
        /// <returns></returns>
        public static int SyllableCount(this string Word)
        {
            Word = Word.ToLower().Trim();
            var lastWasVowel = default(bool);
            var vowels = new[] { 'a', 'e', 'i', 'o', 'u', 'y' }.ToList();
            var count = default(int);
            foreach (var c in Word)
            {
                if (vowels.Contains(c))
                {
                    if (!lastWasVowel)
                        count += 1;
                    lastWasVowel = true;
                }
                else
                {
                    lastWasVowel = false;
                }
            }

            if ((Word.EndsWith("e") || Word.EndsWith("es") || Word.EndsWith("ed")) && !Word.EndsWith("le"))
                count -= 1;
            return count;
        }

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
                char antec = ch.IfNoIndex(index - 1, Conversions.ToChar(""));
                if (antec.ToString().IsBlank() || char.IsLower(antec) || Conversions.ToString(antec) == Constants.vbNullChar)
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
        public static string ToAnagram(this string Text)
        {
            string localShuffle() { string argText = Text; var ret = Shuffle(argText); return ret; }

            return localShuffle().AdjustWhiteSpaces();
        }

        /// <summary>
        /// Transforma uma frase em uma palavra CamelCase
        /// </summary>
        /// <param name="Text">Texto a ser manipulado</param>
        /// <returns>Uma String com o texto am CameCase</returns>
        public static string ToCamel(this string Text)
        {
            return Text.ToProperCase().Split(" ", StringSplitOptions.RemoveEmptyEntries).JoinString("");
        }

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this byte[] Size, int DecimalPlaces = -1)
        {
            return Size.LongLength.ToFileSizeString(DecimalPlaces);
        }

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this FileInfo Size, int DecimalPlaces = -1)
        {
            return Size.Length.ToFileSizeString(DecimalPlaces);
        }

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this double Size, int DecimalPlaces = -1)
        {
            return Size.ChangeType<decimal, double>().ToFileSizeString(DecimalPlaces);
        }

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this int Size, int DecimalPlaces = -1)
        {
            return Size.ChangeType<decimal, int>().ToFileSizeString(DecimalPlaces);
        }

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this long Size, int DecimalPlaces = -1)
        {
            return Size.ChangeType<decimal, long>().ToFileSizeString(DecimalPlaces);
        }

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this decimal Size, int DecimalPlaces = -1)
        {
            return UnitConverter.CreateFileSizeConverter().Abreviate(Size, DecimalPlaces);
        }

        /// <summary>
        /// Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca
        /// espacos por hifen)
        /// </summary>
        /// <param name="Text">         </param>
        /// <param name="UseUnderscore">
        /// Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
        /// </param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyURL(this string Text, bool UseUnderscore = false)
        {
            return Text.ReplaceMany(UseUnderscore ? "_" : "-", "_", "-", " ").RemoveAny("(", ")", ".", ",", "#").ToFriendlyPathName().RemoveAccents().ToLower();
        }

        /// <summary>
        /// Prepara uma string para se tornar uma caminho amigavel (remove caracteres nao permitidos)
        /// </summary>
        /// <param name="Text"></param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyPathName(this string Text)
        {
            return Text.Replace("&", "e").Replace("@", "a").RemoveAny(":", "*", "?", "/", @"\", "<", ">", "{", "}", "[", "]", "|", "\"", "'", Constants.vbTab, Environment.NewLine).AdjustBlankSpaces();
        }

        /// <summary>
        /// Ajusta um caminho colocando as barras corretamente e substituindo caracteres inválidos
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string AdjustPathChars(this string Text, bool InvertedBar = false)
        {
            return Text.Split(new[] { "/", @"\" }, StringSplitOptions.RemoveEmptyEntries).Select((x, i) =>
            {
                if (i == 0 && x.Length == 2 && x.EndsWith(":"))
                    return x;
                return x.ToFriendlyPathName();
            }).JoinString(InvertedBar.AsIf(@"\", "/"));
        }

        /// <summary>
        /// Converte um texo para Leet (1337)
        /// </summary>
        /// <param name="text">  TExto original</param>
        /// <param name="degree">Grau de itensidade (0 - 100%)</param>
        /// <returns>Texto em 1337</returns>
        public static string ToLeet(this string Text, int Degree = 30)
        {
            // Adjust degree between 0 - 100
            Degree = Degree.LimitRange(0, 100);
            // No Leet Translator
            if (Degree == 0)
            {
                return Text;
            }
            // StringBuilder to store result.
            var sb = new StringBuilder(Text.Length);
            foreach (char c in Text)
            {
                // #Region "Degree > 0 and < 17"
                if (Degree < 17 && Degree > 0)
                {
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
                }
                // #End Region
                // #Region "Degree > 16 and < 33"
                else if (Degree < 33 && Degree > 16)
                {
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
                }
                // #End Region
                // #Region "Degree > 32 and < 49"
                else if (Degree < 49 && Degree > 32)
                {
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
                }
                // #End Region
                // #Region "Degree > 48 and < 65"
                else if (Degree < 65 && Degree > 48)
                {
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
                }
                // #End Region
                // #Region "Degree > 64 and < 81"
                else if (Degree < 81 && Degree > 64)
                {
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
                }
                // #End Region
                // #Region "Degree < 100 and > 80"
                else if (Degree > 80 && Degree < 100)
                {
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
                }
                // #End Region
                // #Region "Degree 100"
                else if (Degree > 99)
                {
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
                    // #End Region
                }
            }

            return sb.ToString();
            // Return result.
        }

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

            return Number.ToString() + "%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this int Number)
        {
            return Number.ToString() + "%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this double Number, int Decimals = -1)
        {
            if (Decimals > -1)
            {
                Number = (double)decimal.Round(Number.ChangeType<decimal, double>(), Decimals);
            }

            return Number.ToString() + "%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this short Number)
        {
            return Number.ToString() + "%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this long Number)
        {
            return Number.ToString() + "%";
        }

        /// <summary>
        /// Coloca o texto em TitleCase
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToProperCase(this string Text, bool ForceCase = false)
        {
            if (Text.IsBlank())
                return Text;
            if (ForceCase)
                Text = Text.ToLower();
            var l = Text.Split(" ", StringSplitOptions.None).ToList();
            for (int index = 0, loopTo = l.Count - 1; index <= loopTo; index++)
            {
                string pal = l[index];
                bool artigo = index > 0 && pal.IsIn("o", "a", "os", "as", "um", "uma", "uns", "umas", "de", "do", "dos", "das", "e");
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

            return l.SelectJoin(" ");
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

        /// <summary>
        /// Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e troca
        /// espacos por hifen). É um alias para <see cref="ToFriendlyURL(String, Boolean)"/>
        /// </summary>
        /// <param name="Text">         </param>
        /// <param name="UseUnderscore">
        /// Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
        /// </param>
        /// <returns>string amigavel para URL</returns>
        public static string ToSlugCase(this string Text, bool UseUnderscore = false)
        {
            return Text.ToFriendlyURL(UseUnderscore);
        }

        /// <summary>
        /// Retorna uma string em Snake_Case
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string Text)
        {
            return Text.Replace(" ", "_");
        }

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
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string MaskTelephoneNumber(this string Number)
        {
            Number = Number ?? "";
            string mask = "";
            Number = Number.ParseDigits().RemoveAny(",", ".");
            if (Number.IsBlank())
            {
                return "";
            }

            switch (Number.Length)
            {
                case var @case when @case <= 4:
                    {
                        mask = "{0:####}";
                        break;
                    }

                case var case1 when case1 <= 8:
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

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string MaskTelephoneNumber(this long Number)
        {
            return Number.ToString().MaskTelephoneNumber();
        }

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string MaskTelephoneNumber(this int Number)
        {
            return Number.ToString().MaskTelephoneNumber();
        }

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string MaskTelephoneNumber(this decimal Number)
        {
            return Number.ToString().MaskTelephoneNumber();
        }

        /// <summary>
        /// Aplica uma mascara a um numero de telefone
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string MaskTelephoneNumber(this double Number)
        {
            return Number.ToString().MaskTelephoneNumber();
        }

        /// <summary>
        /// Transforma um texto em titulo
        /// </summary>
        /// <param name="Text">Texto a ser manipulado</param>
        /// <param name="ForceCase">Se FALSE, apenas altera o primeiro caractere de cada palavra como UPPERCASE, dexando os demais intactos. Se TRUE, força o primeiro caractere de casa palavra como UPPERCASE e os demais como LOWERCASE</param>
        /// <returns>Uma String com o texto em nome próprio</returns>
        public static string ToTitle(this string Text, bool ForceCase = false)
        {
            return Text.ToProperCase(ForceCase);
        }

        /// <summary>
        /// Transforma um XML Document em string
        /// </summary>
        /// <param name="XML">Documento XML</param>
        /// <returns></returns>
        public static string ToXMLString(this XmlDocument XML)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    XML.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">              Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as ocorrencias
        /// sejam removidas
        /// </param>
        /// <param name="StringTest">        Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, bool ContinuouslyRemove, params string[] StringTest)
        {
            if (Text != null)
            {
                Text = Text.RemoveFirstAny(ContinuouslyRemove, StringTest);
                Text = Text.RemoveLastAny(ContinuouslyRemove, StringTest);
            }

            return Text;
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">      Texto</param>
        /// <param name="StringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, params string[] StringTest)
        {
            return Text.TrimAny(true, StringTest);
        }

        /// <summary>
        /// Remove continuamente caracteres em branco do começo e fim de uma string incluindo breaklines
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string TrimCarriage(this string Text)
        {
            return Text.TrimAny(Arrays.WhiteSpaceChars.ToArray());
        }

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
        /// Executa uma ação para cada linha de um texto
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public static string ForEachLine(this string Text, Expression<Func<string, string>> Action)
        {
            if (Text.IsNotBlank() && Action != null)
            {
                Text = Text.SplitAny(Arrays.BreakLineChars.ToArray()).Select(x => Action.Compile().Invoke(x)).JoinString(Environment.NewLine);
            }

            return Text;
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">    Texto</param>
        /// <param name="WrapText">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string WrapText = "\"")
        {
            return Text.Wrap(WrapText, WrapText);
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">    Texto</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string OpenWrapText, string CloseWrapText)
        {
            return $"{OpenWrapText}{Text}{CloseWrapText.IfBlank(OpenWrapText)}";
        }

        public static string UnWrap(this string Text, string WrapText = "\"", bool ContinuouslyRemove = false)
        {
            return Text.TrimAny(ContinuouslyRemove, WrapText);
        }

        public static string Inject<T>(this string formatString, T injectionObject)
        {
            if (injectionObject != null)
            {
                if (injectionObject.IsDictionary())
                {
                    return formatString.Inject((IDictionary)injectionObject);
                }
                else
                {
                    return formatString.Inject(GetPropertyHash(injectionObject));
                }
            }

            return formatString;
        }

        public static string Inject(this string formatString, IDictionary dictionary)
        {
            return formatString.Inject(new Hashtable(dictionary));
        }

        public static string Inject(this string formatString, Hashtable attributes)
        {
            string result = formatString;
            if (attributes != null && formatString != null)
            {
                foreach (string attributeKey in attributes.Keys)
                    result = result.InjectSingleValue(attributeKey, attributes[attributeKey]);
            }

            return result;
        }

        public static string InjectSingleValue(this string formatString, string key, object replacementValue)
        {
            string result = formatString;
            var attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");
            foreach (Match m in attributeRegex.Matches(formatString))
            {
                string replacement = m.ToString();
                if (m.Groups[2].Length > 0)
                {
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
                    replacement = string.Format(CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
                }
                else
                {
                    replacement = (replacementValue ?? string.Empty).ToString();
                }

                result = result.Replace(m.ToString(), replacement);
            }

            return result;
        }

        private static Hashtable GetPropertyHash<T>(T properties)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                var props = TypeDescriptor.GetProperties(properties);
                foreach (PropertyDescriptor prop in props)
                    values.Add(prop.Name, prop.GetValue(properties));
            }

            return values;
        }
    }

    public class QuantityTextPair
    {
        public QuantityTextPair(string Plural, string Singular = "")
        {
            this.Plural = Plural;
            this.Singular = Singular.IfBlank(Plural.QuantifyText(1));
        }

        public QuantityTextPair()
        {
        }

        public string this[IComparable Number]
        {
            get
            {
                return ToString(Conversions.ToDecimal(Number));
            }

            set
            {
                if (Number.IsNumber() && Conversions.ToDecimal(Number).Floor().IsIn(1m, -1))
                {
                    Singular = value;
                }
                else
                {
                    Plural = value;
                }
            }
        }

        public string Singular { get; set; } = "Item";
        public string Plural { get; set; } = "Items";

        public override string ToString()
        {
            return Plural;
        }

        public string ToString(long Number)
        {
            return Number.IsIn(1L, -1L) ? Singular : Plural;
        }

        public string ToString(decimal Number)
        {
            return Number.Floor().IsIn(1m, -1m) ? Singular : Plural;
        }

        public string ToString(short Number)
        {
            return Number.IsIn((short)1, (short)-1) ? Singular : Plural;
        }

        public string ToString(int Number)
        {
            return Number.IsIn(1, -1) ? Singular : Plural;
        }

        public string ToString(double Number)
        {
            return Number.Floor().IsIn(1d, -1d) ? Singular : Plural;
        }

        public string ToString(float Number)
        {
            return Number.IsIn(1f, -1f) ? Singular : Plural;
        }
    }

    /// <summary>
    /// Classe para escrever numeros por extenso com suporte até 999 quintilhoes
    /// </summary>
    public class FullNumberWriter
    {
        /// <summary>
        /// String que representa a palavra "Menos". Utilizada quando os números são negativos
        /// </summary>
        /// <returns></returns>
        public string Minus { get; set; }

        /// <summary>
        /// String que representa o numero 0.
        /// </summary>
        /// <returns></returns>
        public string Zero { get; set; }

        /// <summary>
        /// String que representa a palavra "e". Utilizada na concatenação de expressões
        /// </summary>
        /// <returns></returns>
        public string And { get; set; }

        /// <summary>
        /// String que representa o numero 1.
        /// </summary>
        /// <returns></returns>
        public string One { get; set; }

        /// <summary>
        /// String que representa o numero 2.
        /// </summary>
        /// <returns></returns>
        public string Two { get; set; }

        /// <summary>
        /// String que representa o numero 3.
        /// </summary>
        /// <returns></returns>
        public string Three { get; set; }

        /// <summary>
        /// String que representa o numero 4.
        /// </summary>
        /// <returns></returns>
        public string Four { get; set; }

        /// <summary>
        /// String que representa o numero 5.
        /// </summary>
        /// <returns></returns>
        public string Five { get; set; }

        /// <summary>
        /// String que representa o numero 6.
        /// </summary>
        /// <returns></returns>
        public string Six { get; set; }

        /// <summary>
        /// String que representa o numero 7.
        /// </summary>
        /// <returns></returns>
        public string Seven { get; set; }

        /// <summary>
        /// String que representa o numero 8.
        /// </summary>
        /// <returns></returns>
        public string Eight { get; set; }

        /// <summary>
        /// String que representa o numero 9.
        /// </summary>
        /// <returns></returns>
        public string Nine { get; set; }

        /// <summary>
        /// String que representa o numero 10.
        /// </summary>
        /// <returns></returns>
        public string Ten { get; set; }

        /// <summary>
        /// String que representa o numero 11.
        /// </summary>
        /// <returns></returns>
        public string Eleven { get; set; }

        /// <summary>
        /// String que representa o numero 12.
        /// </summary>
        /// <returns></returns>
        public string Twelve { get; set; }

        /// <summary>
        /// String que representa o numero 13.
        /// </summary>
        /// <returns></returns>
        public string Thirteen { get; set; }

        /// <summary>
        /// String que representa o numero 14.
        /// </summary>
        /// <returns></returns>
        public string Fourteen { get; set; }

        /// <summary>
        /// String que representa o numero 15.
        /// </summary>
        /// <returns></returns>
        public string Fifteen { get; set; }

        /// <summary>
        /// String que representa o numero 16.
        /// </summary>
        /// <returns></returns>
        public string Sixteen { get; set; }

        /// <summary>
        /// String que representa o numero 17.
        /// </summary>
        /// <returns></returns>
        public string Seventeen { get; set; }

        /// <summary>
        /// String que representa o numero 18.
        /// </summary>
        /// <returns></returns>
        public string Eighteen { get; set; }

        /// <summary>
        /// String que representa o numero 19.
        /// </summary>
        /// <returns></returns>
        public string Nineteen { get; set; }

        /// <summary>
        /// String que representa os numeros 20 a 29 .
        /// </summary>
        /// <returns></returns>
        public string Twenty { get; set; }

        /// <summary>
        /// String que representa os numeros 30 a 39.
        /// </summary>
        /// <returns></returns>
        public string Thirty { get; set; }

        /// <summary>
        /// String que representa os numeros 40 a 49.
        /// </summary>
        /// <returns></returns>
        public string Fourty { get; set; }

        /// <summary>
        /// String que representa os numeros 50 a 59.
        /// </summary>
        /// <returns></returns>
        public string Fifty { get; set; }

        /// <summary>
        /// String que representa os numeros 60 a 69.
        /// </summary>
        /// <returns></returns>
        public string Sixty { get; set; }

        /// <summary>
        /// String que representa os numeros 70 a 79.
        /// </summary>
        /// <returns></returns>
        public string Seventy { get; set; }

        /// <summary>
        /// String que representa os numeros 80 a 89.
        /// </summary>
        /// <returns></returns>
        public string Eighty { get; set; }

        /// <summary>
        /// String que representa os numeros 90 a 99.
        /// </summary>
        /// <returns></returns>
        public string Ninety { get; set; }

        /// <summary>
        /// String que represena o exato numero 100. Em alguns idiomas esta string não é nescessária
        /// </summary>
        /// <returns></returns>
        public string ExactlyOneHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 100 a 199.
        /// </summary>
        /// <returns></returns>
        public string OneHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 200 a 299.
        /// </summary>
        /// <returns></returns>
        public string TwoHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 300 a 399.
        /// </summary>
        /// <returns></returns>
        public string ThreeHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 400 a 499.
        /// </summary>
        /// <returns></returns>
        public string FourHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 500 a 599.
        /// </summary>
        /// <returns></returns>
        public string FiveHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 600 a 699.
        /// </summary>
        /// <returns></returns>
        public string SixHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 700 a 799.
        /// </summary>
        /// <returns></returns>
        public string SevenHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 800 a 899.
        /// </summary>
        /// <returns></returns>
        public string EightHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 900 a 999.
        /// </summary>
        /// <returns></returns>
        public string NineHundred { get; set; }

        /// <summary>
        /// String que representa os numeros 1000 a 9999
        /// </summary>
        /// <returns></returns>
        public string Thousand { get; set; }

        /// <summary>
        /// Par de strings que representam os numeros 1 milhão a 999 milhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Million { get; set; } = new QuantityTextPair();

        /// <summary>
        /// Par de strings que representam os numeros 1 bilhão a 999 bilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Billion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// Par de strings que representam os numeros 1 trilhão a 999 trilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Trillion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// Par de strings que representam os numeros 1 quadrilhão a 999 quadrilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Quadrillion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// Par de strings que representam os numeros 1 quintilhão a 999 quintilhões
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair Quintillion { get; set; } = new QuantityTextPair();

        /// <summary>
        /// String utilizada quando o numero é maior que 999 quintilhões. Retorna uma string "Mais de 999 quintilhões"
        /// </summary>
        /// <returns></returns>
        public string MoreThan { get; set; }

        /// <summary>
        /// String utilizada quando um numero possui casa decimais. Normalmente "virgula"
        /// </summary>
        /// <returns></returns>
        public string DecimalSeparator { get; set; }

        /// <summary>
        /// Instancia um novo <see cref="FullNumberWriter"/> com as configurações default (inglês)
        /// </summary>
        public FullNumberWriter()
        {
            foreach (var prop in this.GetProperties().Where(x => x.CanWrite))
            {
                switch (prop.Name ?? "")
                {
                    case "ExactlyOneHundred":
                    case "DecimalSeparator":
                    case "And":
                        {
                            continue;
                            break;
                        }

                    default:
                        {
                            switch (prop.PropertyType)
                            {
                                case var @case when @case == typeof(string):
                                    {
                                        prop.SetValue(this, prop.Name.ToNormalCase());
                                        break;
                                    }

                                case var case1 when case1 == typeof(QuantityTextPair):
                                    {
                                        if (((QuantityTextPair)prop.GetValue(this)).Plural.IsBlank())
                                        {
                                            prop.SetValue(this, new QuantityTextPair(prop.Name + "s", prop.Name));
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        break;
                                    }
                            }

                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Escreve um numero por extenso
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public virtual string this[decimal Number, int DecimalPlaces = 2]
        {
            get
            {
                return ToString(Number, DecimalPlaces);
            }
        }

        public override string ToString()
        {
            return ToString(0m);
        }

        public virtual string ToString(decimal Number, int DecimalPlaces = 2)
        {
            long dec = Number.GetDecimalPlaces(DecimalPlaces.LimitRange(0, 3));
            long num = (long)Math.Round(Number.Floor());
            return (InExtensive(num) + (dec == 0L | DecimalPlaces == 0 ? "" : DecimalSeparator.Wrap(" ") + InExtensive(dec))).ToLower().AdjustWhiteSpaces();
        }

        internal string InExtensive(decimal Number)
        {
            switch (Number)
            {
                case var @case when @case < 0m:
                    {
                        return Minus + " " + InExtensive(Number * -1);
                    }

                case 0m:
                    {
                        return Zero;
                    }

                case var case1 when 1m <= case1 && case1 <= 19m:
                    {
                        var strArray = new string[] { One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Eleven, Twelve, Thirteen, Fourteen, Fifteen, Sixteen, Seventeen, Eighteen, Nineteen };
                        return strArray[(int)Math.Round(Number - 1m)] + " ";
                    }

                case var case2 when 20m <= case2 && case2 <= 99m:
                    {
                        var strArray = new string[] { Twenty, Thirty, Fourty, Fifty, Sixty, Seventy, Eighty, Ninety };
                        if (Number % 10m == 0m)
                        {
                            return strArray[(int)((long)Math.Round(Number) / 10L - 2L)];
                        }
                        else
                        {
                            return strArray[(int)((long)Math.Round(Number) / 10L - 2L)] + And.Wrap(" ") + InExtensive(Number % 10m);
                        }

                        break;
                    }

                case 100m:
                    {
                        return ExactlyOneHundred.IfBlank(OneHundred);
                    }

                case var case3 when 101m <= case3 && case3 <= 999m:
                    {
                        var strArray = new string[] { OneHundred, TwoHundred, ThreeHundred, FourHundred, FiveHundred, SixHundred, SevenHundred, EightHundred, NineHundred };
                        if (Number % 100m == 0m)
                        {
                            return strArray[(int)((long)Math.Round(Number) / 100L - 1L)] + " ";
                        }
                        else
                        {
                            return strArray[(int)((long)Math.Round(Number) / 100L - 1L)] + And.Wrap(" ") + InExtensive(Number % 100m);
                        }

                        break;
                    }

                case var case4 when 1000m <= case4 && case4 <= 1999m:
                    {
                        switch (Number % 1000m)
                        {
                            case 0m:
                                {
                                    return Thousand + " ";
                                }

                            case var case5 when case5 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return Thousand + And.Wrap(" ") + InExtensive(Number % 1000m);
                                }

                            default:
                                {
                                    return Thousand + " " + InExtensive(Number % 1000m);
                                }
                        }

                        break;
                    }

                case var case6 when 2000m <= case6 && case6 <= 999999m:
                    {
                        switch (Number % 1000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000L) + " " + Thousand;
                                }

                            case var case7 when case7 <= 100m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000L) + " " + Thousand + And.Wrap(" ") + InExtensive(Number % 1000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000L) + " " + Thousand + " " + InExtensive(Number % 1000m);
                                }
                        }

                        break;
                    }

                #region Milhao

                case var case8 when 1000000m <= case8 && case8 <= 1999999m:
                    {
                        switch (Number % 1000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Million.Singular;
                                }

                            case var case9 when case9 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Million.Singular + And.Wrap(" ") + InExtensive(Number % 1000000m);
                                }

                            default:
                                {
                                    return One + " " + Million.Singular + " " + InExtensive(Number % 1000000m);
                                }
                        }

                        break;
                    }

                case var case10 when 2000000m <= case10 && case10 <= 999999999m:
                    {
                        switch (Number % 1000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000L) + Million.Plural.Wrap(" ");
                                }

                            case var case11 when case11 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000L) + Million.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000L) + Million.Plural.Wrap(" ") + InExtensive(Number % 1000000m);
                                }
                        }

                        break;
                    }

                #endregion Milhao

                #region Bilhao

                case var case12 when 1000000000m <= case12 && case12 <= 1999999999m:
                    {
                        switch (Number % 1000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Billion.Singular;
                                }

                            case var case13 when case13 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Billion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000m);
                                }

                            default:
                                {
                                    return One + " " + Billion.Singular + " " + InExtensive(Number % 1000000000m);
                                }
                        }

                        break;
                    }

                case var case14 when 2000000000m <= case14 && case14 <= 999999999999m:
                    {
                        switch (Number % 1000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000L) + Billion.Plural.Wrap(" ");
                                }

                            case var case15 when case15 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000L) + Billion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000L) + Billion.Plural.Wrap(" ") + InExtensive(Number % 1000000000m);
                                }
                        }

                        break;
                    }

                #endregion Bilhao

                #region Trilhao

                case var case16 when 1000000000000m <= case16 && case16 <= 1999999999999m:
                    {
                        switch (Number % 1000000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Trillion.Singular;
                                }

                            case var case17 when case17 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Trillion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }

                            default:
                                {
                                    return One + " " + Trillion.Singular + " " + InExtensive(Number % 1000000000000m);
                                }
                        }

                        break;
                    }
                // 9.223.372.036.854.775.807
                case var case18 when 2000000000000m <= case18 && case18 <= 999999999999999m:
                    {
                        switch (Number % 1000000000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000L) + Trillion.Plural.Wrap(" ");
                                }

                            case var case19 when case19 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000L) + Trillion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000L) + Trillion.Plural.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }
                        }

                        break;
                    }

                #endregion Trilhao

                #region Quadilhao

                case var case20 when 1000000000000000m <= case20 && case20 <= 1999999999999999m:
                    {
                        switch (Number % 1000000000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Quadrillion.Singular;
                                }

                            case var case21 when case21 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Quadrillion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000000m);
                                }

                            default:
                                {
                                    return One + " " + Quadrillion.Singular + " " + InExtensive(Number % 1000000000000m);
                                }
                        }

                        break;
                    }

                case var case22 when 2000000000000000m <= case22 && case22 <= 999999999999999999m:
                    {
                        switch (Number % 1000000000000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000L) + Quadrillion.Plural.Wrap(" ");
                                }

                            case var case23 when case23 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000L) + Quadrillion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000L) + Quadrillion.Plural.Wrap(" ") + InExtensive(Number % 1000000000000000m);
                                }
                        }

                        break;
                    }

                #endregion Quadilhao

                #region Quintilhao

                case var case24 when 1000000000000000000m <= case24 && case24 <= 1999999999999999999m:
                    {
                        switch (Number % 1000000000000000000m)
                        {
                            case 0m:
                                {
                                    return One + " " + Quintillion.Singular;
                                }

                            case var case25 when case25 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return One + " " + Quintillion.Singular + And.Wrap(" ") + InExtensive(Number % 1000000000000000000m);
                                }

                            default:
                                {
                                    return One + " " + Quintillion.Singular + " " + InExtensive(Number % 1000000000000000000m);
                                }
                        }

                        break;
                    }

                case var case26 when 2000000000000000000m <= case26 && case26 <= 999999999999999999999m:
                    {
                        switch (Number % 1000000000000000000m)
                        {
                            case 0m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000000L) + Quintillion.Plural.Wrap(" ");
                                }

                            case var case27 when case27 <= 100m:
                            case 200m:
                            case 300m:
                            case 400m:
                            case 500m:
                            case 600m:
                            case 700m:
                            case 800m:
                            case 900m:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000000L) + Quintillion.Plural.Wrap(" ") + And.Wrap(" ") + InExtensive(Number % 1000000000000000000m);
                                }

                            default:
                                {
                                    return InExtensive((long)Math.Round(Number) / 1000000000000000000L) + Quintillion.Plural.Wrap(" ") + InExtensive(Number % 1000000000000000000m);
                                }

                                #endregion Quintilhao
                        }

                        break;
                    }

                default:
                    {
                        return MoreThan + " " + InExtensive(999999999999999999999m);
                    }
            }
        }
    }

    /// <summary>
    /// Classe para escrever moedas por extenso com suporte até 999 quintilhoes de $$
    /// </summary>
    public class FullMoneyWriter : FullNumberWriter
    {
        /// <summary>
        /// Par de strings que representam os nomes da moeda em sua forma singular ou plural
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair CurrencyName { get; set; } = new QuantityTextPair("dollars", "dollar");

        /// <summary>
        /// Par de strings que representam os centavos desta moeda em sua forma singular ou plural
        /// </summary>
        /// <returns></returns>
        public QuantityTextPair CurrencyCentsName { get; set; } = new QuantityTextPair("cents", "cent");

        /// <summary>
        /// Escreve um numero por extenso
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public override string ToString(decimal Number, int DecimalPlaces = 2)
        {
            long dec = Number.GetDecimalPlaces(DecimalPlaces.LimitRange(0, 3));
            long num = (long)Math.Round(Number.Floor());
            return (InExtensive(num) + CurrencyCentsName[num].Wrap(" ") + (dec == 0L | DecimalPlaces == 0 ? "" : And.Wrap(" ") + InExtensive(dec) + CurrencyCentsName[dec].Wrap(" "))).ToLower().AdjustWhiteSpaces();
        }

        public override string ToString()
        {
            return ToString(0m);
        }
    }

    public class ConnectionStringParser : Dictionary<string, string>
    {
        public ConnectionStringParser() : base()
        {
        }

        public ConnectionStringParser(string ConnectionString) : base()
        {
            Parse(ConnectionString);
        }

        public ConnectionStringParser Parse(string ConnectionString)
        {
            try
            {
                Clear();
                foreach (var ii in ConnectionString.IfBlank("").SplitAny(";").Select(t => t.Split(new char[] { '=' }, 2)).ToDictionary(t => t[0].Trim(), t => t[1].Trim(), StringComparer.InvariantCultureIgnoreCase))
                    this.Set(ii.Key.ToTitle(true), ii.Value);
            }
            catch (Exception ex)
            {
            }

            return this;
        }

        /// <summary>
        /// Retorna a connectionstring deste parser
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.SelectJoin(x => $"{x.Key.ToTitle()}={x.Value}", ";");
        }

        public static implicit operator string(ConnectionStringParser cs)
        {
            return cs.ToString();
        }

        public static implicit operator ConnectionStringParser(string s)
        {
            return new ConnectionStringParser(s);
        }
    }

    /// <summary>
    /// Classe para criação de strings contendo tags HTML
    /// </summary>
    public class HtmlTag
    {
        public string TagName { get; set; } = "div";
        public string InnerHtml { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

        public HtmlTag()
        {
        }

        public HtmlTag(string TagName, string InnerHtml = "")
        {
            this.TagName = TagName.IfBlank("div");
            this.InnerHtml = InnerHtml;
        }

        public string Class
        {
            get
            {
                return Attributes.GetValueOr("class", "");
            }

            set
            {
                Attributes = Attributes ?? new Dictionary<string, string>();
                Attributes["class"] = value;
            }
        }

        public string[] ClassArray
        {
            get
            {
                return Class.Split(" ");
            }

            set
            {
                Class = (value ?? Array.Empty<string>()).JoinString(" ");
            }
        }

        public override string ToString()
        {
            TagName = TagName.RemoveAny("/", @"\");
            Attributes = Attributes ?? new Dictionary<string, string>();
            return $"<{TagName.IfBlank("div")} {Attributes.SelectJoin(x => x.Key.ToLower() + "=" + x.Value.Wrap())}>{InnerHtml}</{TagName.IfBlank("div")}>";
        }

        public static implicit operator string(HtmlTag Tag)
        {
            return Tag?.ToString();
        }
    }
}