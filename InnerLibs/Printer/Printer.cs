// ***********************************************************************
// Assembly         : InnerLibs
// Author           : Leandro Ferreira / Zonaro
// Created          : 16-03-2019
//
// ***********************************************************************
// <copyright file="Printer.vb" company="InnerCodeTech">

// The MIT License (MIT)
// Copyright (c) 2019 InnerCodeTech
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using InnerLibs.Printer.Command;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs.Printer
{
    public static class PrinterExtension
    {
        public static Printer CreatePrinter(this IPrintCommand CommandType, string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null)
        {
            return Printer.CreatePrinter(CommandType, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }
    }

    public class Printer
    {
        private class PrinterWriter : TextWriter
        {
            private Printer p;

            public PrinterWriter(Printer p)
            {
                this.p = p;
            }

            public PrinterWriter(IFormatProvider formatProvider, Printer p) : base(formatProvider)
            {
                this.p = p;
            }

            public override void Write(char value)
            {
                Write($"{value}");
            }

            public override void Flush()
            {
                p.PrintDocument();
            }

            public override Encoding Encoding
            {
                get
                {
                    return p.Command?.Encoding;
                }
            }
        }

        private object txw;

        /// <summary>
        /// TextWriter interno desta Printer
        /// </summary>
        /// <returns></returns>
        public TextWriter TextWriter
        {
            get
            {
                return (TextWriter)txw;
            }
        }

        public static Printer CreatePrinter<CommandType>(string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null) where CommandType : IPrintCommand
        {
            return CreatePrinter(typeof(CommandType), PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }

        public static Printer CreatePrinter(Type CommandType, string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null)
        {
            return CreatePrinter((IPrintCommand)Activator.CreateInstance(CommandType), PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }

        public static Printer CreatePrinter(IPrintCommand CommandType, string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null)
        {
            return new Printer(CommandType, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }

        private bool _ommit = false;
        private string FontMode = "Normal";
        private string Align = "Left";

        public byte[] DocumentBuffer { get; set; }
        public bool AutoPrint { get; set; } = false;
        public XDocument HTMLDocument { get; private set; } = XDocument.Parse("<body><link rel='stylesheet' href='Printer.css' /></body>");
        public string PrinterName { get; set; }
        public int ColsNomal { get; set; }
        public int ColsCondensed { get; set; }
        public int ColsExpanded { get; set; }
        public IPrintCommand Command { get; private set; }
        public bool Diacritics { get; set; } = true;
        public Func<string, string> RewriteFunction { get; set; } = null;
        public bool IsItalic { get; private set; }
        public bool IsBold { get; private set; }
        public bool IsUnderline { get; private set; }

        public bool IsCondensed
        {
            get
            {
                return FontMode == "Condensed";
            }

            set
            {
                if (value)
                {
                    FontMode = "Condensed";
                }
                else
                {
                    FontMode = "Normal";
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return FontMode == "Expanded";
            }

            set
            {
                if (value)
                {
                    FontMode = "Expanded";
                }
                else
                {
                    FontMode = "Normal";
                }
            }
        }

        public bool IsLarge
        {
            get
            {
                return FontMode == "Double2";
            }

            set
            {
                if (value)
                {
                    FontMode = "Double2";
                }
                else
                {
                    FontMode = "Normal";
                }
            }
        }

        public bool IsLarger
        {
            get
            {
                return FontMode == "Double3";
            }

            set
            {
                if (value)
                {
                    FontMode = "Double3";
                }
                else
                {
                    FontMode = "Normal";
                }
            }
        }

        public bool IsNormal
        {
            get
            {
                return !IsCondensed && !IsExpanded;
            }
        }

        public bool IsLeftAligned
        {
            get
            {
                return Align == "Left";
            }
        }

        public bool IsRightAligned
        {
            get
            {
                return Align == "Right";
            }
        }

        public bool IsCenterAligned
        {
            get
            {
                return Align == "Center";
            }
        }

        public Printer(Encoding Encoding) : this(null, null, 0, 0, 0, Encoding)
        {
        }

        public Printer(IPrintCommand Command) : this(Command, null, 0, 0, 0, null)
        {
        }

        public Printer(IPrintCommand Command, Encoding Encoding) : this(Command, null, 0, 0, 0, Encoding)
        {
        }

        public Printer() : this(null, null, 0, 0, 0, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Printer"/> class.
        /// </summary>
        /// <param name="PrinterName">Printer name, shared name or port of printer install</param>
        /// <param name="ColsNormal">Number of columns for normal mode print</param>
        /// <param name="ColsCondensed">Number of columns for condensed mode print</param>
        /// <param name="ColsExpanded">Number of columns for expanded mode print</param>
        /// <param name="Encoding">Custom Encoding</param>
        public Printer(string PrinterName, int ColsNormal, int ColsCondensed, int ColsExpanded, Encoding Encoding) : this(null, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Printer"/> class.
        /// </summary>
        /// <param name="PrinterName">Printer name, shared name or port of printer install</param>
        /// <param name="ColsNormal">Number of columns for normal mode print</param>
        /// <param name="ColsCondensed">Number of columns for condensed mode print</param>
        /// <param name="ColsExpanded">Number of columns for expanded mode print</param>
        /// <param name="Encoding">Custom Encoding</param>
        public Printer(IPrintCommand Command, string PrinterName, int ColsNormal, int ColsCondensed, int ColsExpanded, Encoding Encoding)
        {
            txw = new PrinterWriter(this);
            this.Command = Command ?? new EscPosCommands.EscPos();
            if (Encoding != null)
            {
                this.Command.Encoding = Encoding;
            }

            if (this.Command.Encoding is null)
            {
                this.Command.Encoding = Encoding.Default;
            }

            this.PrinterName = PrinterName.IfBlank("temp.prn").Trim();
            ColsNomal = ColsNormal <= 0 ? this.Command.ColsNomal : ColsNormal;
            this.ColsCondensed = ColsCondensed <= 0 ? this.Command.ColsCondensed : ColsCondensed;
            this.ColsExpanded = ColsExpanded <= 0 ? this.Command.ColsExpanded : ColsExpanded;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Printer"/> class.
        /// </summary>
        /// <param name="PrinterName">Printer name, shared name or port of printer install</param>
        /// <param name="ColsNormal">Number of columns for normal mode print</param>
        /// <param name="ColsCondensed">Number of columns for condensed mode print</param>
        /// <param name="ColsExpanded">Number of columns for expanded mode print</param>
        public Printer(string PrinterName, int ColsNormal, int ColsCondensed, int ColsExpanded) : this(PrinterName, ColsNormal, ColsCondensed, ColsExpanded, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Printer"/> class.
        /// </summary>
        /// <param name="PrinterName">Printer name, shared name or port of printer install</param>
        /// <param name="Encoding">Custom Encoding</param>
        public Printer(string PrinterName, Encoding Encoding) : this(PrinterName, 0, 0, 0, Encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Printer"/> class.
        /// </summary>
        /// <param name="PrinterName">Printer name, shared name or port of printer install</param>
        public Printer(string PrinterName) : this(PrinterName, 0, 0, 0, null)
        {
        }

        // substitui caracteres para caracteres acentuados não disponíveis na fonte DOS
        [Obsolete]
        public static string FixAccents(string Lin)
        {
            string T1;
            string T2;
            int i;
            int p;
            string C;
            T1 = "áéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕàèìòùÈÌÒçÇ"; // tela
            T2 = "160130161162163181144214224233131136140147150182210215226219198228199229133138141149151212222227135128"; // ASC impressora
            p = 1;
            var loopTo = Strings.Len(Lin);
            for (i = 1; i <= loopTo; i++)                                      // cada letra
            {
                C = Strings.Mid(Lin, i, 1);                                      // pega o char
                p = Strings.InStr(T1, C);                                         // tem acento correspondente?
                if (Conversions.ToBoolean(p))                                                  // tem...
                {
                    // troca usando backspace: letra + bs + acento
                    Lin = Strings.Left(Lin, i - 1) + Strings.Chr((int)Math.Round(Conversion.Val(Strings.Mid(T2, p * 3 - 2, 3)))) + Strings.Mid(Lin, i + 1);                                  // troca
                }
            }

            return Lin;
        }

        /// <summary>
        /// Funcao que reescreve o valor antes de chamar o <see cref="Write(String, Boolean)"/>
        /// </summary>
        /// <param name="StringAction"></param>
        /// <returns></returns>
        public Printer UseRewriteFunction(Func<string, string> StringAction)
        {
            RewriteFunction = StringAction;
            return this;
        }

        /// <summary>
        /// Remove a função de reescrita de valor definida pela <see cref="UseRewriteFunction(Func(Of String, String))"/>
        /// </summary>
        /// <returns></returns>
        public Printer RemoveRewriteFunction()
        {
            RewriteFunction = null;
            return this;
        }

        /// <summary>
        /// Permite a ultilização de acentos nas chamadas <see cref="Write(String, Boolean)"/> posteriores
        /// </summary>
        /// <param name="OnOff"></param>
        /// <returns></returns>
        public Printer UseDiacritics(bool OnOff = true)
        {
            Diacritics = OnOff;
            return this;
        }

        /// <summary>
        /// Remove todos os acentod das chamadas <see cref="Write(String, Boolean)"/> posteriores
        /// </summary>
        /// <returns></returns>
        public Printer DontUseDiacritics()
        {
            return UseDiacritics(false);
        }

        /// <summary>
        /// Adciona um numero <paramref name="Lines"/> de quebras de linha ao <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Lines"></param>
        /// <returns></returns>
        public new Printer NewLine(int Lines = 1)
        {
            while (Lines > 0)
            {
                Write(Command.Encoding.GetBytes(Constants.vbNewLine));
                Lines = Lines - 1;
            }

            return this;
        }

        /// <summary>
        /// Adciona um numero <paramref name="Spaces"/> de espaços em branco ao <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Spaces"></param>
        /// <returns></returns>
        public Printer Space(int Spaces = 1)
        {
            while (Spaces > 0)
            {
                Write(Command.Encoding.GetBytes(" "));
                Spaces = Spaces - 1;
            }

            return this;
        }

        /// <summary>
        /// Limpa o <see cref="DocumentBuffer"/>
        /// </summary>
        /// <returns></returns>
        public Printer Clear()
        {
            DocumentBuffer = null;
            HTMLDocument.Root.RemoveAll();
            HTMLDocument.Root.Add("<link rel='stylesheet' href='Printer.css'/>");
            return this;
        }

        /// <summary>
        /// Escreve um separador
        /// </summary>
        /// <param name="Character"></param>
        /// <returns></returns>
        public Printer Separator(char Character = '-', int? Columns = default)
        {
            return WriteLine(new string(Character, Columns ?? GetCurrentColumns()));
        }

        /// <summary>
        /// Imprime o auto-teste da impressora
        /// </summary>
        /// <returns></returns>
        public Printer AutoTest()
        {
            _ommit = true;
            return Write(Command.AutoTest());
        }

        /// <summary>
        /// Testa os acentos para esta impressora
        /// </summary>
        /// <returns></returns>
        public Printer TestDiacritics()
        {
            return WriteLine("áéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕàèìòùÈÌÒçÇ");
        }

        public Printer Italic(bool state = true)
        {
            _ommit = true;
            IsItalic = state;
            return Write(Command.Italic(state));
        }

        public Printer NotItalic()
        {
            return Italic(false);
        }

        public Printer Bold(bool state = true)
        {
            _ommit = true;
            IsBold = state;
            return Write(Command.Bold(state));
        }

        public Printer NotBold()
        {
            return Bold(false);
        }

        public Printer UnderLine(bool state = true)
        {
            _ommit = true;
            IsUnderline = state;
            return Write(Command.Underline(state));
        }

        public Printer NotUnderline()
        {
            return UnderLine(false);
        }

        public Printer Expanded(bool state = true)
        {
            _ommit = true;
            IsExpanded = state;
            return Write(Command.Expanded(state));
        }

        public Printer NotExpanded()
        {
            return Expanded(false);
        }

        public Printer Condensed(bool state = true)
        {
            IsCondensed = state;
            _ommit = true;
            return Write(Command.Condensed(state));
        }

        public Printer NotCondensed()
        {
            return Condensed(false);
        }

        public Printer NormalFont()
        {
            FontMode = "Normal";
            _ommit = true;
            return Write(Command.NormalFont());
        }

        public Printer LargeFont()
        {
            IsLarge = true;
            _ommit = true;
            return Write(Command.LargeFont());
        }

        public Printer LargerFont()
        {
            IsLarger = true;
            _ommit = true;
            return Write(Command.LargerFont());
        }

        public Printer AlignLeft()
        {
            Align = "Left";
            _ommit = true;
            return Write(Command.Left());
        }

        public Printer AlignRight()
        {
            Align = "Right";
            _ommit = true;
            return Write(Command.Right());
        }

        public Printer AlignCenter()
        {
            Align = "Center";
            _ommit = true;
            return Write(Command.Center());
        }

        public Printer FullPaperCut()
        {
            HTMLDocument.Root.Add("<hr class='FullPaperCut'/>");
            _ommit = true;
            return Write(Command.FullCut());
        }

        public Printer PartialPaperCut()
        {
            HTMLDocument.Root.Add("<hr class='PartialPaperCut'/>");
            _ommit = true;
            return Write(Command.PartialCut());
        }

        public Printer OpenDrawer()
        {
            _ommit = true;
            return Write(Command.OpenDrawer());
        }

        public Printer QrCode(string qrData)
        {
            _ommit = true;
            return Write(Command.PrintQrData(qrData));
        }

        public Printer QrCode(string qrData, QrCodeSize qrCodeSize)
        {
            _ommit = true;
            return Write(Command.PrintQrData(qrData, qrCodeSize));
        }

        public Printer Code128(string code)
        {
            _ommit = true;
            return Write(Command.Code128(code));
        }

        public Printer Code39(string code)
        {
            _ommit = true;
            return Write(Command.Code39(code));
        }

        public Printer Ean13(string code)
        {
            _ommit = true;
            return Write(Command.Ean13(code));
        }

        public Printer InitializePrint()
        {
            RawPrinterHelper.SendBytesToPrinter(PrinterName, Command.Initialize());
            return this;
        }

        /// <summary>
        /// Retorna o numero de colunas  do modo atual
        /// </summary>
        /// <returns></returns>
        public int GetCurrentColumns()
        {
            if (IsCondensed)
            {
                return ColsCondensed;
            }
            else if (IsExpanded)
            {
                return ColsExpanded;
            }
            else
            {
                return ColsNomal;
            }
        }

        public string GetDotLine(string LeftText, string RightText, int? Columns = default, char CharLine = '.')
        {
            Columns = Columns ?? GetCurrentColumns();
            if (Columns > 0 == true)
                return new string(CharLine, (Columns.Value - (LeftText.Length + RightText.Length)).LimitRange(0, Columns.Value));
            return "";
        }

        /// <summary>
        /// Escreve os bytes contidos em <paramref name="value"/> no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Printer Write(byte[] value)
        {
            if (value != null && value.Any())
            {
                var list = new List<byte>();
                if (DocumentBuffer != null)
                    list.AddRange(DocumentBuffer);
                list.AddRange(value);
                DocumentBuffer = list.ToArray();
                if (_ommit == false)
                {
                    try
                    {
                        string v = Command.Encoding.GetString(value).ReplaceMany("<br/>", Arrays.BreakLineChars.ToArray());
                        if (v == "<br/>")
                        {
                            HTMLDocument.Root.Add("<br/>");
                        }
                        else
                        {
                            HTMLDocument.Root.Add(XElement.Parse($"<span class='align-{Align.ToLower()} font-{FontMode.ToLower()}{IsBold.AsIf(" bold")}{IsItalic.AsIf(" italic")}{IsUnderline.AsIf(" underline")}'>{v.Replace(" ", "&nbsp;")}</span>"));
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                _ommit = false;
                if (AutoPrint)
                {
                    PrintDocument();
                }
            }

            return this;
        }

        /// <summary>
        /// Escreve o <paramref name="value"/> no <see cref="DocumentBuffer"/> se <paramref name="Test"/> for TRUE
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Test"></param>
        /// <returns></returns>
        public Printer Write(string value, bool Test = true)
        {
            if (Test)
            {
                if (value.ContainsAny(Arrays.BreakLineChars.ToArray()))
                {
                    foreach (var line in value.SplitAny(Arrays.BreakLineChars.ToArray()))
                        WriteLine(line, Test && line.IsNotBlank());
                }
                else if (value.IsNotBlank())
                {
                    if (!Diacritics)
                    {
                        value = value.RemoveDiacritics();
                    }

                    if (RewriteFunction != null)
                    {
                        value = RewriteFunction.Invoke(value);
                    }

                    Write(Command.Encoding.GetBytes(value));
                }
            }

            return this;
        }

        /// <summary>
        /// Escreve o <paramref name="value"/> se <paramref name="Test"/> for TRUE e quebra uma linha
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Test"></param>
        /// <returns></returns>
        public new Printer WriteLine(string value, bool Test)
        {
            return Test ? Write(value, Test).NewLine() : this;
        }

        /// <summary>
        /// Escreve o <paramref name="value"/>   e quebra uma linha
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public new Printer WriteLine(string value)
        {
            return WriteLine(value, true);
        }

        /// <summary>
        /// Escreve varias linhas no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public new Printer WriteLine(params string[] values)
        {
            values = (values ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).ToArray();
            if (values.Any())
            {
                WriteLine(values.Join(Constants.vbNewLine));
            }

            return this;
        }

        /// <summary>
        /// Escreve um teste de 48 colunas no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <returns></returns>
        public Printer WriteTest()
        {
            NewLine();
            AlignLeft();
            WriteLine("INNERLIBS TEST PRINTER - 48 COLUMNS");
            WriteLine("....+....1....+....2....+....3....+....4....+...");
            Separator();
            WriteLine("Default Text");
            Italic().WriteLine("Italic Text").NotItalic();
            Bold().WriteLine("Bold Text").NotBold();
            UnderLine().WriteLine("UnderLine Text").NotUnderline();
            Expanded().WriteLine("Expanded Text").WriteLine("....+....1....+....2....").NotExpanded();
            Condensed().WriteLine("Condensed Text").NotCondensed();
            Separator();
            LargeFont();
            WriteLine("Font Size 2");
            LargerFont();
            WriteLine("Font Size 3");
            NormalFont();
            WriteLine("Normal Font Size");
            Separator();
            AlignRight();
            WriteLine("Text on Right");
            AlignCenter();
            WriteLine("Text on Center");
            AlignLeft();
            WriteLine("Text on Left");
            NewLine(3);
            WriteLine("ACENTOS");
            TestDiacritics();
            WriteLine("EOF :)");
            Separator();
            NewLine();
            PartialPaperCut();
            return this;
        }

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dics"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(IEnumerable<IDictionary<T1, T2>> dics, bool PartialCutOnEach = false)
        {
            return WriteDictionary(PartialCutOnEach, (dics ?? Array.Empty<IDictionary<T1, T2>>()).ToArray());
        }

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(IDictionary<T1, T2> dic, bool PartialCutOnEach = false)
        {
            return WriteDictionary(PartialCutOnEach, new[] { dic });
        }

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dics"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(params IDictionary<T1, T2>[] dics)
        {
            return WriteDictionary(false, dics);
        }

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dics"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(bool PartialCutOnEach, params IDictionary<T1, T2>[] dics)
        {
            dics = dics ?? Array.Empty<IDictionary<T1, T2>>();
            foreach (var dic in dics)
            {
                if (dic != null)
                {
                    if (PartialCutOnEach)
                        PartialPaperCut();
                    else
                        Separator();
                    foreach (var item in dic)
                        WritePair($"{item.Key}".ToNormalCase(), item.Value);
                    AlignLeft();
                }
            }

            if (dics.Any())
            {
                if (PartialCutOnEach)
                    PartialPaperCut();
                else
                    Separator();
            }

            return this;
        }

        /// <summary>
        /// Escreve uma lista de itens no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="ListOrdenator"></param>
        /// <returns></returns>
        public Printer WriteList(IEnumerable<object> Items, string ListOrdenator = null)
        {
            for (int index = 0, loopTo = Items.Count() - 1; index <= loopTo; index++)
                WriteLine($"{ListOrdenator ?? $"{index + 1} "}{Items.ElementAtOrDefault(index)}");
            return this;
        }

        /// <summary>
        /// Escreve uma lista de itens no <see cref="DocumentBuffer"/>
        /// </summary>
        public Printer WriteList(params object[] Items)
        {
            return WriteList((Items ?? Array.Empty<object>()).AsEnumerable());
        }

        /// <summary>
        /// Escreve um par de infomações no <see cref="DocumentBuffer"/>.
        /// </summary>
        public Printer WritePair(object Key, object Value, int? Columns = default, char CharLine = ' ')
        {
            Columns = Columns ?? GetCurrentColumns();
            string dots = "";
            string s1 = $"{Key}";
            string s2 = $"{Value}";
            if (s2.IsNotBlank() && Columns.Value > 0)
            {
                dots = GetDotLine(s1, s2, Columns, CharLine);
            }
            else
            {
                dots = " ";
            }

            string s = $"{s1}{dots}{s2}";
            return WriteLine(s, s.IsNotBlank());
        }

        /// <summary>
        /// Escreve uma linha de preço no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="Price"></param>
        /// <param name="Culture"></param>
        /// <param name="Columns"></param>
        /// <param name="CharLine"></param>
        /// <returns></returns>
        public Printer WritePriceLine(string Description, decimal Price, CultureInfo Culture = null, int? Columns = default, char CharLine = '.')
        {
            string sprice = Price.ToString("C", Culture ?? CultureInfo.CurrentCulture);
            return WritePair(Description, sprice, Columns, CharLine);
        }

        /// <summary>
        /// Escreve uma lista de preços no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Culture"></param>
        /// <param name="Columns"></param>
        /// <param name="CharLine"></param>
        /// <returns></returns>
        public Printer WritePriceList(IEnumerable<Tuple<string, decimal>> List, CultureInfo Culture = null, int? Columns = default, char CharLine = '.')
        {
            foreach (var item in List.NullAsEmpty())
                WritePriceLine(item.Item1, item.Item2, Culture, Columns, CharLine);
            return this;
        }

        /// <summary>
        /// Escreve uma lista de preços no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Culture"></param>
        /// <param name="Columns"></param>
        /// <param name="CharLine"></param>
        /// <returns></returns>
        public Printer WritePriceList<T>(IEnumerable<T> List, Expression<Func<T, string>> Description, Expression<Func<T, decimal>> Price, CultureInfo Culture = null, int? Columns = default, char CharLine = '.')
        {
            return WritePriceList(List.Select(x => new Tuple<string, decimal>(Description.Compile()(x), Price.Compile()(x))), Culture, Columns);
        }

        /// <summary>
        /// Escreve uma tabela no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <returns></returns>
        public Printer WriteTable<T>(IEnumerable<T> Items) where T : class
        {
            Write(ConsoleTables.ConsoleTable.From(Items).ToString());
            return this;
        }

        /// <summary>
        /// Escreve uma tabela no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <returns></returns>
        public Printer WriteTable<T>(params T[] Items) where T : class
        {
            return WriteTable(Items.AsEnumerable());
        }

        /// <summary>
        /// Escreve as Propriedades e valores de uma classe como pares
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <returns></returns>
        public Printer WriteClass<T>(params T[] objs) where T : class
        {
            return WriteClass(false, objs);
        }

        /// <summary>
        /// Escreve as Propriedades e valores de uma classe como pares
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objs"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteClass<T>(bool PartialCutOnEach, params T[] objs) where T : class
        {
            objs = objs ?? Array.Empty<T>();
            foreach (var obj in objs)
            {
                if (obj != null)
                {
                    if (PartialCutOnEach)
                        PartialPaperCut();
                    else
                        Separator();
                    foreach (var item in obj.GetNullableTypeOf().GetProperties())
                    {
                        if (item.CanRead)
                        {
                            WritePair(item.Name.ToNormalCase(), item.GetValue(obj));
                        }
                    }

                    AlignLeft();
                }
            }

            if (objs.Any())
            {
                if (PartialCutOnEach)
                    PartialPaperCut();
                else
                    Separator();
            }

            return this;
        }

        /// <summary>
        /// Escreve as Propriedades e valores de uma classe como pares
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteClass<T>(IEnumerable<T> obj, bool PartialCutOnEach = false) where T : class
        {
            return WriteClass(PartialCutOnEach, (obj ?? Array.Empty<T>()).ToArray());
        }

        /// <summary>
        /// Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="obj"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, bool PartialCutOnEach, params T[] obj)
        {
            if (TemplateString.IsNotBlank())
            {
                obj = obj ?? Array.Empty<T>();
                if (TemplateString.IsFilePath())
                {
                    if (File.Exists(TemplateString))
                    {
                        TemplateString = File.ReadAllText(TemplateString);
                    }
                }

                foreach (var item in obj)
                {
                    string ns = TemplateString.Inject(item);
                    foreach (var linha in ns.SplitAny(Arrays.BreakLineChars.ToArray()))
                        WriteLine(linha);
                }

                if (obj.Any())
                {
                    if (PartialCutOnEach)
                        PartialPaperCut();
                    else
                        Separator();
                }
            }

            return this;
        }

        /// <summary>
        /// Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, params T[] obj) where T : class
        {
            return WriteTemplate(TemplateString, false, obj);
        }

        /// <summary>
        /// Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="obj"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, IEnumerable<T> obj, bool PartialCutOnEach = false) where T : class
        {
            return WriteTemplate(TemplateString, PartialCutOnEach, (obj ?? Array.Empty<T>()).ToArray());
        }

        /// <summary>
        /// Escreve um template substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="obj"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, T obj, bool PartialCutOnEach = false) where T : class
        {
            return WriteTemplate(TemplateString, PartialCutOnEach, Converter.ForceArray(obj, typeof(T)));
        }

        /// <summary>
        /// Escreve uma data usando formato especifico
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public object WriteDate(DateTime DateAndTime, string Format = null)
        {
            if (Format.IsNotBlank())
            {
                return Write(DateAndTime.ToString(Format));
            }
            else
            {
                return Write(DateAndTime.ToString());
            }
        }

        /// <summary>
        /// Escreve uma data usando uma Cultura especifica
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public object WriteDate(DateTime DateAndTime, CultureInfo Format = null)
        {
            if (Format != null)
            {
                return Write(DateAndTime.ToString(Format));
            }
            else
            {
                return Write(DateAndTime.ToString());
            }
        }

        /// <summary>
        /// Escreve a data atual usando formato especifico
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public object WriteDate(string Format = null)
        {
            return WriteDate(DateTime.Now, Format);
        }

        /// <summary>
        /// Escreve a data atual usando uma cultura especifica
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public object WriteDate(CultureInfo Format = null)
        {
            return WriteDate(DateTime.Now, Format);
        }

        /// <summary>
        /// Imprime o conteudo do <see cref="DocumentBuffer"/> atual e limpa o buffer
        /// </summary>
        /// <param name="Copies"></param>
        /// <returns></returns>
        public Printer PrintDocument(int Copies = 1)
        {
            return PrintDocument(DocumentBuffer, Copies).Clear();
        }

        /// <summary>
        /// Imprime o conteudo de um arquivo ou o conteudo de todos os arquivos de um diretorio
        /// </summary>
        /// <param name="FileOrDirectoryPath"></param>
        /// <param name="Copies"></param>
        /// <returns></returns>
        public Printer PrintDocument(string FileOrDirectoryPath, int Copies = 1)
        {
            if (FileOrDirectoryPath.IsDirectoryPath())
            {
                if (Directory.Exists(FileOrDirectoryPath))
                {
                    foreach (var item in Directory.GetFiles(FileOrDirectoryPath))
                        PrintDocument(item, Copies);
                }
            }
            else if (FileOrDirectoryPath.IsFilePath())
            {
                if (File.Exists(FileOrDirectoryPath))
                {
                    PrintDocument(File.ReadAllBytes(FileOrDirectoryPath), Copies);
                }
            }
            else
            {
                throw new ArgumentException($"FileOrDirectoryPath Is Not a valid Path: {FileOrDirectoryPath}");
            }

            return this;
        }

        /// <summary>
        /// Envia os Bytes para a impressora ou arquivo
        /// </summary>
        /// <param name="Copies"></param>
        /// <returns></returns>
        public Printer PrintDocument(byte[] Bytes, int Copies = 1)
        {
            if (Bytes != null && Bytes.Any())
            {
                for (int i = 0, loopTo = Copies.SetMinValue(1) - 1; i <= loopTo; i++)
                {
                    if (PrinterName.IsFilePath())
                    {
                        SaveFile(PrinterName, false);
                    }
                    else if (!RawPrinterHelper.SendBytesToPrinter(PrinterName, DocumentBuffer.ToArray()))
                        throw new ArgumentException("Não foi possível acessar a impressora: " + PrinterName);
                }
            }

            return this;
        }

        /// <summary>
        /// Escreve um Arquivo com os dados binarios e HTML desta impressao
        /// </summary>
        /// <param name="FileOrDirectoryPath"></param>
        /// <returns></returns>
        public Printer SaveFile(string FileOrDirectoryPath, bool IncludeHtmlDoc = false)
        {
            if (DocumentBuffer != null && DocumentBuffer.Any())
            {
                if (FileOrDirectoryPath.IsDirectoryPath())
                {
                    FileOrDirectoryPath = $@"{FileOrDirectoryPath}\{Command.GetTypeOf().Name}\{PrinterName.ToFriendlyPathName()}\{DateTime.Now.Ticks}.{Command?.GetTypeOf()?.Name.IfBlank("bin")}";
                    FileOrDirectoryPath = FileOrDirectoryPath.AdjustPathChars(true);
                }

                if (FileOrDirectoryPath.IsFilePath())
                {
                    var d = DateTime.Now;
                    var info = DocumentBuffer.ToArray().WriteToFile(FileOrDirectoryPath, d);
                    if (IncludeHtmlDoc)
                    {
                        string s = $@"{info.Directory.FullName}\{Path.GetFileNameWithoutExtension(info.FullName)}.html";
                        HTMLDocument.Save(s);
                        if (!info.Directory.GetFiles("Printer.css").Any())
                        {
                            Assembly.GetExecutingAssembly().GetResourceFileText("InnerLibs.Printer.css").Replace("##Cols##", ColsNomal.ToString()).WriteToFile($@"{info.Directory}\Printer.css", false, Encoding.Unicode);
                        }
                    }
                }
                else
                {
                    throw new ArgumentException($"FileOrDirectoryPath is not a valid Path: {FileOrDirectoryPath}");
                }
            }

            return this;
        }

        public Printer Image(string Path, bool highDensity = true)
        {
            if (!Path.IsFilePath())
                throw new FileNotFoundException("Invalid Path");
            if (!File.Exists(Path))
                throw new FileNotFoundException("Image file not found");
            var img = System.Drawing.Image.FromFile(Path);
            Image(img, highDensity);
            img.Dispose();
            return this;
        }

        public Printer Image(Stream stream, bool HighDensity = true)
        {
            var img = System.Drawing.Image.FromStream(stream);
            Image(img, HighDensity);
            img.Dispose();
            return this;
        }

        public Printer Image(byte[] bytes, bool HighDensity = true)
        {
            System.Drawing.Image img;
            using (var ms = new MemoryStream(bytes))
            {
                img = System.Drawing.Image.FromStream(ms);
            }

            Image(img, HighDensity);
            img.Dispose();
            return this;
        }

        public Printer Image(System.Drawing.Image pImage, bool highDensity = true)
        {
            HTMLDocument.Root.Add(XElement.Parse($"<img class='image{highDensity.AsIf(" HighDensity")}'  src='{pImage.ToDataURL()}' />"));
            _ommit = true;
            return Write(Command.PrintImage(pImage, highDensity));
        }
    }
}