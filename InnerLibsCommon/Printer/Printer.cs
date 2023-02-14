// *********************************************************************** Assembly : InnerLibs
// Author : Leandro Ferreira / Zonaro Created : 16-03-2019
//
// ***********************************************************************
// <copyright file="Printer.vb" company="InnerCodeTech">
//     The MIT License (MIT) Copyright (c) 2019 InnerCodeTech
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//     and associated documentation files (the "Software"), to deal in the Software without
//     restriction, including without limitation the rights to use, copy, modify, merge, publish,
//     distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
//     Software is furnished to do so, subject to the following conditions: The above copyright
//     notice and this permission notice shall be included in all copies or substantial portions of
//     the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A
//     PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//     LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
//     OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//     DEALINGS IN THE SOFTWARE.
// </copyright>
// ***********************************************************************

using InnerLibs.Printer.Command;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace InnerLibs.Printer
{
    public static class PrinterExtension
    {
        #region Public Methods

        public static Printer CreatePrinter(this InnerLibs.Printer.Command.IPrintCommand CommandType, string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null)
        {
            return Printer.CreatePrinter(CommandType, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }

        #endregion Public Methods
    }

    public class Printer
    {
        #region Private Fields

        private bool _ommit = false;

        private string Align = "Left";

        private string FontMode = "Normal";

        private string FontSize = "Normal";

        private TextWriter txw;

        #endregion Private Fields

        #region Private Classes

        private class PrinterWriter : TextWriter
        {
            #region Private Fields

            private Printer p;

            #endregion Private Fields

            #region Public Constructors

            public PrinterWriter(Printer p)
            {
                this.p = p;
            }

            public PrinterWriter(IFormatProvider formatProvider, Printer p) : base(formatProvider)
            {
                this.p = p;
            }

            #endregion Public Constructors

            #region Public Properties

            public override Encoding Encoding => p.Command?.Encoding;

            #endregion Public Properties

            #region Public Methods

            public override void Flush()
            {
                p.PrintDocument();
            }

            public override void Write(char value)
            {
                Write($"{value}");
            }

            #endregion Public Methods
        }

        #endregion Private Classes

        #region Internal Methods

        internal string GetDotLine(string LeftText, string RightText, int? Columns = default, char CharLine = ' ')
        {
            Columns = Columns ?? GetCurrentColumns();
            if (CharLine.ToString().IsBlank())
            {
                CharLine = ' ';
            }

            if (Columns > 0 == true)
            {
                return new string(CharLine, (Columns.Value - (LeftText.Length + RightText.Length)).LimitRange(0, Columns.Value));
            }

            return InnerLibs.Ext.EmptyString;
        }

        internal string GetPair(string LeftText, string RightText, int? Columns = default, char CharLine = ' ')
        {
            Columns = Columns ?? GetCurrentColumns();
            string dots = InnerLibs.Ext.EmptyString;
            string s1 = $"{LeftText}";
            string s2 = $"{RightText}";
            if (s2.IsNotBlank() && Columns.Value > 0)
            {
                dots = GetDotLine(s1, s2, Columns, CharLine);
            }
            else
            {
                dots = $"{CharLine}";
            }

            return $"{s1}{dots}{s2}";
        }

        #endregion Internal Methods

        #region Public Constructors

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
            if (PrinterName.IsBlank())
            {
                throw new ArgumentException("Printername cannot be null or empty", "PrinterName");
            }

            this.Command = Command ?? new InnerLibs.EscPosCommands.EscPos();
            if (Encoding != null)
            {
                this.Command.Encoding = Encoding;
            }

            if (this.Command.Encoding == null)
            {
                this.Command.Encoding = Encoding.Default;
            }

            this.PrinterName = PrinterName;
            ColumnsNormal = ColsNormal <= 0 ? this.Command.ColsNormal : ColsNormal;
            ColumnsCondensed = ColsCondensed <= 0 ? this.Command.ColsCondensed : ColsCondensed;
            ColumnsExpanded = ColsExpanded <= 0 ? this.Command.ColsExpanded : ColsExpanded;
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

        #endregion Public Constructors

        #region Public Properties

        public bool AutoPrint { get; set; } = false;

        public int ColumnsCondensed { get; set; }

        public int ColumnsExpanded { get; set; }

        public int ColumnsNormal { get; set; }

        public IPrintCommand Command { get; private set; }

        public bool Diacritics { get; set; } = true;

        public byte[] DocumentBuffer { get; set; }

        public XDocument HTMLDocument { get; private set; } = XDocument.Parse("<body><link rel='stylesheet' href='Printer.css' /></body>");

        public bool IsBold { get; private set; }

        public bool IsCenterAligned => Align == "Center";

        public bool IsCondensed
        {
            get => FontMode == "Condensed";

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
            get => FontMode == "Expanded";

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

        public bool IsItalic { get; private set; }

        public bool IsLarge
        {
            get => FontSize == "Large";

            set
            {
                if (value)
                {
                    FontSize = "Large";
                }
                else
                {
                    FontSize = "Normal";
                }
            }
        }

        public bool IsLeftAligned => Align == "Left";

        public bool IsMedium
        {
            get => FontSize == "Medium";

            set
            {
                if (value)
                {
                    FontSize = "Medium";
                }
                else
                {
                    FontSize = "Normal";
                }
            }
        }

        public bool IsNormal => !IsCondensed && !IsExpanded;

        public bool IsRightAligned => Align == "Right";

        public bool IsUnderline { get; private set; }

        public bool OnOff { get; set; } = true;

        public string PrinterName { get; set; }

        public Func<string, string> RewriteFunction { get; set; } = null;

        /// <summary>
        /// TextWriter interno desta Printer
        /// </summary>
        /// <returns></returns>
        public TextWriter TextWriter => txw;

        #endregion Public Properties

        #region Public Methods

        public static Printer CreatePrinter<CommandType>(string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null) where CommandType : InnerLibs.Printer.Command.IPrintCommand
        {
            return CreatePrinter(typeof(CommandType), PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }

        public static Printer CreatePrinter(Type CommandType, string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null)
        {
            IPrintCommand tp = (IPrintCommand)Activator.CreateInstance(CommandType);
            return CreatePrinter(tp, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }

        public static Printer CreatePrinter(IPrintCommand CommandType, string PrinterName, int ColsNormal = 0, int ColsCondensed = 0, int ColsExpanded = 0, Encoding Encoding = null)
        {
            return new Printer(CommandType, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding);
        }

        public Printer AlignCenter()
        {
            Align = "Center";
            _ommit = true;
            return Write(Command.Center());
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

        /// <summary>
        /// Imprime o auto-teste da impressora
        /// </summary>
        /// <returns></returns>
        public Printer AutoTest()
        {
            _ommit = true;
            return Write(Command.AutoTest());
        }

        public Printer Bold(bool state = true)
        {
            _ommit = true;
            IsBold = state;
            return Write(Command.Bold(state));
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

        public Printer Condensed(bool state = true)
        {
            IsCondensed = state;
            _ommit = true;
            return Write(Command.Condensed(state));
        }

        /// <summary>
        /// Remove todos os acentos das chamadas <see cref="Write(String, Boolean)"/> posteriores
        /// </summary>
        /// <returns></returns>
        public Printer DontUseDiacritics() => UseDiacritics(false);

        public Printer Ean13(string code)
        {
            _ommit = true;
            return Write(Command.Ean13(code));
        }

        public Printer Expanded(bool state = true)
        {
            _ommit = true;
            IsExpanded = state;
            return Write(Command.Expanded(state));
        }

        public Printer FullPaperCut()
        {
            HTMLDocument.Root.Add("<hr class='FullPaperCut'/>");
            _ommit = true;
            return Write(Command.FullCut());
        }

        /// <summary>
        /// Retorna o numero de colunas do modo atual
        /// </summary>
        /// <returns></returns>
        public int GetCurrentColumns()
        {
            if (IsCondensed)
            {
                if (IsLarge)
                {
                    return (int)Math.Round(ColumnsCondensed / 3d);
                }

                if (IsMedium)
                {
                    return (int)Math.Round(ColumnsCondensed / 2d);
                }

                return ColumnsCondensed;
            }
            else if (IsExpanded)
            {
                if (IsLarge)
                {
                    return (int)Math.Round(ColumnsExpanded / 3d);
                }

                if (IsMedium)
                {
                    return (int)Math.Round(ColumnsExpanded / 2d);
                }

                return ColumnsExpanded;
            }
            else
            {
                if (IsLarge)
                {
                    return (int)Math.Round(ColumnsNormal / 3d);
                }

                if (IsMedium)
                {
                    return (int)Math.Round(ColumnsNormal / 2d);
                }

                return ColumnsNormal;
            }
        }

        public Printer Image(string Path, bool HighDensity = true)
        {
            if (!Path.IsFilePath())
            {
                throw new FileNotFoundException("Invalid Path");
            }

            if (!File.Exists(Path))
            {
                throw new FileNotFoundException("Image file not found");
            }

            var img = System.Drawing.Image.FromFile(Path);
            Image(img, HighDensity);
            img.Dispose();
            return this;
        }

        public Printer Image(Stream Stream, bool HighDensity = true)
        {
            var img = System.Drawing.Image.FromStream(Stream);
            Image(img, HighDensity);
            img.Dispose();
            return this;
        }

        public Printer Image(byte[] Bytes, bool HighDensity = true)
        {
            System.Drawing.Image img;
            using (var ms = new MemoryStream(Bytes))
            {
                img = System.Drawing.Image.FromStream(ms);
            }

            Image(img, HighDensity);
            img.Dispose();
            return this;
        }

        public Printer Image(System.Drawing.Image Img, bool HighDensity = true)
        {
            HTMLDocument.Root.Add(XElement.Parse($"<img class='image{HighDensity.AsIf(" HighDensity")}'  src='{Img.ToDataURL()}' />"));
            _ommit = true;
            return Write(Command.PrintImage(Img, HighDensity));
        }

        public Printer Initialize()
        {
            if (OnOff)
            {
                RawPrinterHelper.SendBytesToPrinter(PrinterName, Command.Initialize());
            }

            return this;
        }

        public Printer Italic(bool state = true)
        {
            _ommit = true;
            IsItalic = state;
            return Write(Command.Italic(state));
        }

        public Printer LargeFontSize()
        {
            IsLarge = true;
            _ommit = true;
            return Write(Command.LargerFont());
        }

        public Printer MediumFontSize()
        {
            IsMedium = true;
            _ommit = true;
            return Write(Command.LargeFont());
        }

        /// <summary>
        /// Adciona um numero <paramref name="Lines"/> de quebras de linha ao <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Lines"></param>
        /// <returns></returns>
        public Printer NewLine(int Lines = 1)
        {
            while (Lines > 0)
            {
                Write(Environment.NewLine);
                Lines--;
            }

            return this;
        }

        public Printer NormalFontSize()
        {
            FontMode = "Normal";
            _ommit = true;
            return Write(Command.NormalFont());
        }

        public Printer NormalFontStretch() => NotCondensed().NotExpanded();

        public Printer NormalFontStyle() => NotBold().NotItalic().NotUnderline();

        public Printer NotBold() => Bold(false);

        public Printer NotCondensed() => Condensed(false);

        public Printer NotExpanded() => Expanded(false);

        public Printer NotItalic() => Italic(false);

        public Printer NotUnderline() => UnderLine(false);

        public Printer OpenDrawer()
        {
            _ommit = true;
            return Write(Command.OpenDrawer());
        }

        public Printer PartialPaperCut()
        {
            HTMLDocument.Root.Add("<hr class='PartialPaperCut'/>");
            _ommit = true;
            return Write(Command.PartialCut());
        }

        /// <summary>
        /// Imprime o conteudo do <see cref="DocumentBuffer"/> atual e limpa o buffer
        /// </summary>
        /// <param name="Copies"></param>
        /// <returns></returns>
        public Printer PrintDocument(int Copies = 1, bool Clear = true)
        {
            PrintDocument(DocumentBuffer, Copies);
            if (Clear)
            {
                this.Clear();
            }

            return this;
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
                    {
                        PrintDocument(item, Copies);
                    }
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
            if (OnOff && Copies > 0)
            {
                if (Bytes != null && Bytes.Any())
                {
                    for (int i = 0, loopTo = Copies - 1; i <= loopTo; i++)
                    {
                        if (PrinterName.IsFilePath())
                        {
                            SaveFile(PrinterName, false);
                        }
                        else if (!RawPrinterHelper.SendBytesToPrinter(PrinterName, DocumentBuffer.ToArray()))
                        {
                            Ext.WriteDebug("Não foi possível acessar a impressora: " + PrinterName);
                        }
                    }
                }
            }

            return this;
        }

        public Printer QrCode(string qrData)
        {
            _ommit = true;
            return Write(Command.PrintQrData(qrData));
        }

        public Printer QrCode(string qrData, InnerLibs.Printer.QrCodeSize qrCodeSize)
        {
            _ommit = true;
            return Write(Command.PrintQrData(qrData, qrCodeSize));
        }

        /// <summary>
        /// Remove a função de reescrita de valor definida pela <see
        /// cref="UseRewriteFunction(Func(Of String, String))"/>
        /// </summary>
        /// <returns></returns>
        public Printer RemoveRewriteFunction()
        {
            RewriteFunction = null;
            return this;
        }

        /// <summary>
        /// Alinha a esquerda, remove formatação (italico, negrito, sublinhado) e retorna a fonte ao
        /// seu tamanho normal
        /// </summary>
        /// <returns></returns>
        public Printer ResetFont() => NormalFontSize().NormalFontStretch().NormalFontStyle();

        /// <summary>
        /// Escreve um Arquivo com os dados binarios e HTML desta impressao
        /// </summary>
        /// <param name="FileOrDirectoryPath"></param>
        /// <returns></returns>
        public Printer SaveFile(string FileOrDirectoryPath, bool IncludeHtmlDoc = false)
        {
            if (DocumentBuffer != null && DocumentBuffer.Count() > 0)
            {
                if (FileOrDirectoryPath.IsDirectoryPath())
                {
                    FileOrDirectoryPath = $@"{FileOrDirectoryPath}\{Command.GetTypeOf().Name}\{PrinterName.ToFriendlyPathName()}\{DateTime.Now.Ticks}.{Command?.GetTypeOf()?.Name.IfBlank("bin")}";
                    FileOrDirectoryPath = FileOrDirectoryPath.FixPath(true);
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
                            Ext.GetResourceFileText(Assembly.GetExecutingAssembly(), "InnerLibs.Printer.css").Replace("##Cols##", ColumnsNormal.ToString()).WriteToFile($@"{info.Directory}\Printer.css", false, Encoding.Unicode);
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
        /// Adciona um numero <paramref name="Spaces"/> de espaços em branco ao <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Spaces"></param>
        /// <returns></returns>
        public Printer Space(int Spaces = 1)
        {
            if (Spaces > 0)
            {
                Write(new string(' ', Spaces));
            }

            return this;
        }

        /// <summary>
        /// Testa os acentos para esta impressora
        /// </summary>
        /// <returns></returns>
        public Printer TestDiacritics()
        {
            bool ud = Diacritics;
            return UseDiacritics(true).WriteLine("áéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕàèìòùÈÌÒçÇ").UseDiacritics(ud);
        }

        public Printer UnderLine(bool state = true)
        {
            _ommit = true;
            IsUnderline = state;
            return Write(Command.Underline(state));
        }

        /// <summary>
        /// Permite a utilização de acentos nas chamadas <see cref="Write(String, Boolean)"/> posteriores
        /// </summary>
        /// <param name="OnOff"></param>
        /// <returns></returns>
        public Printer UseDiacritics(bool OnOff = true)
        {
            Diacritics = OnOff;
            return this;
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
                {
                    list.AddRange(DocumentBuffer);
                }

                list.AddRange(value);
                DocumentBuffer = list.ToArray();
                if (_ommit == false)
                {
                    try
                    {
                        string v = Command.Encoding.GetString(value).ReplaceMany("<br/>", InnerLibs.PredefinedArrays.BreakLineChars.ToArray());
                        if (v == "<br/>")
                        {
                            HTMLDocument.Root.Add("<br/>");
                        }
                        else
                        {
                            HTMLDocument.Root.Add(XElement.Parse($"<span class='align-{Align.ToLowerInvariant()} font-{FontMode.ToLowerInvariant()}{IsBold.AsIf(" bold")}{IsItalic.AsIf(" italic")}{IsUnderline.AsIf(" underline")}'>{v.Replace(" ", "&#160;")}</span>"));
                        }
                    }
                    catch
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
        /// Escreve o <paramref name="value"/> no <see cref="DocumentBuffer"/> se <paramref
        /// name="Test"/> for TRUE
        /// </summary>
        /// <param name="value"></param>
        /// <param name="Test"></param>
        /// <returns></returns>
        public Printer Write(string value, bool Test = true)
        {
            if (Test)
            {
                if (value.ContainsAny(PredefinedArrays.BreakLineChars.ToArray()))
                {
                    foreach (var line in value.SplitAny(PredefinedArrays.BreakLineChars.ToArray()))
                    {
                        WriteLine(line, Test && line.IsNotBlank());
                    }
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

        public Printer Write(string value, Expression<Func<string, bool>> Test) => Write(value, Test == null || Test.Compile().Invoke(value));

        /// <summary>
        /// Escreve as Propriedades e valores de uma classe como pares
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public Printer WriteClass<T>(params T[] Objects) where T : class => WriteClass(false, Objects);

        /// <summary>
        /// Escreve as Propriedades e valores de uma classe como pares
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Objects"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteClass<T>(bool PartialCutOnEach, params T[] Objects) where T : class
        {
            Objects = Objects ?? Array.Empty<T>();
            foreach (var obj in Objects)
            {
                if (obj != null)
                {
                    if (PartialCutOnEach)
                    {
                        PartialPaperCut();
                    }
                    else
                    {
                        Separator();
                    }

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

            if (Objects.Any())
            {
                if (PartialCutOnEach)
                {
                    PartialPaperCut();
                }
                else
                {
                    Separator();
                }
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
        public Printer WriteClass<T>(IEnumerable<T> obj, bool PartialCutOnEach = false) where T : class => WriteClass(PartialCutOnEach, (obj ?? Array.Empty<T>()).ToArray());

        /// <summary>
        /// Escreve uma data usando formato especifico
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public Printer WriteDate(DateTime DateAndTime, string Format = null) => Format.IsNotBlank() ? Write(DateAndTime.ToString(Format)) : Write(DateAndTime.ToString());

        /// <summary>
        /// Escreve uma data usando uma Cultura especifica
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public Printer WriteDate(DateTime DateAndTime, CultureInfo Format = null) => Format != null ? Write(DateAndTime.ToString(Format)) : Write(DateAndTime.ToString());

        /// <summary>
        /// Escreve a data atual usando formato especifico
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public Printer WriteDate(string Format = null) => WriteDate(DateTime.Now, Format);

        /// <summary>
        /// Escreve a data atual usando uma cultura especifica
        /// </summary>
        /// <param name="Format"></param>
        /// <returns></returns>
        public Printer WriteDate(CultureInfo Format = null) => WriteDate(DateTime.Now, Format);

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Dictionaries"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(IEnumerable<IDictionary<T1, T2>> Dictionaries, bool PartialCutOnEach = false) => WriteDictionary(PartialCutOnEach, (Dictionaries ?? Array.Empty<IDictionary<T1, T2>>()).ToArray());

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(IDictionary<T1, T2> dic, bool PartialCutOnEach = false) => WriteDictionary(PartialCutOnEach, new[] { dic });

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Dictionaries"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(params IDictionary<T1, T2>[] Dictionaries) => WriteDictionary(false, Dictionaries);

        /// <summary>
        /// Escreve os valores de um Dictionary como pares
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="Dictionaries"></param>
        /// <returns></returns>
        public Printer WriteDictionary<T1, T2>(bool PartialCutOnEach, params IDictionary<T1, T2>[] Dictionaries)
        {
            Dictionaries = Dictionaries ?? Array.Empty<IDictionary<T1, T2>>();
            foreach (var dic in Dictionaries)
            {
                if (dic != null)
                {
                    if (PartialCutOnEach)
                    {
                        PartialPaperCut();
                    }
                    else
                    {
                        Separator();
                    }

                    foreach (var item in dic)
                    {
                        WritePair($"{item.Key}".ToNormalCase(), item.Value);
                    }

                    AlignLeft();
                }
            }

            if (Dictionaries.Any())
            {
                if (PartialCutOnEach)
                {
                    PartialPaperCut();
                }
                else
                {
                    Separator();
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
        public Printer WriteLine(string value, Expression<Func<string, bool>> Test) => WriteLine(value, Test == null || Test.Compile().Invoke(value));

        public Printer WriteLine(string value, bool Test) => Test ? Write(value, Test).NewLine() : this;

        /// <summary>
        /// Escreve o <paramref name="value"/> e quebra uma linha
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Printer WriteLine(string value) => WriteLine(value, true);

        /// <summary>
        /// Escreve varias linhas no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public Printer WriteLine(params string[] values)
        {
            values = (values ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).ToArray();
            if (values.Any())
            {
                WriteLine(values.SelectJoinString(Environment.NewLine));
            }

            return this;
        }

        /// <summary>
        /// Escreve uma lista de itens no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="ListOrdenator"></param>
        /// <returns></returns>
        public Printer WriteList(IEnumerable<object> Items, int ListOrdenator = 1)
        {
            Items = (Items ?? Array.Empty<object>()).AsEnumerable();
            for (int index = 0, loopTo = Items.Count() - 1; index <= loopTo; index++)
            {
                WriteLine($"{index + ListOrdenator} {Items.ElementAtOrDefault(index)}");
            }

            return this;
        }

        public Printer WriteList(IEnumerable<object> Items, string ListOrdenator)
        {
            Items = (Items ?? Array.Empty<object>()).AsEnumerable();
            for (int index = 0, loopTo = Items.Count() - 1; index <= loopTo; index++)
            {
                WriteLine($"{ListOrdenator} {Items.ElementAtOrDefault(index)}");
            }

            return this;
        }

        /// <summary>
        /// Escreve uma lista de itens no <see cref="DocumentBuffer"/>
        /// </summary>
        public Printer WriteList(params object[] Items) => WriteList((Items ?? Array.Empty<object>()).AsEnumerable());

        /// <summary>
        /// Escreve um par de informações no <see cref="DocumentBuffer"/>.
        /// </summary>
        public Printer WritePair(object Key, object Value, int? Columns = default, char CharLine = ' ') => WriteLine(GetPair($"{Key}", $"{Value}", Columns, CharLine), x => x.IsNotBlank());

        /// <summary>
        /// Escreve uma linha de preço no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="Price"></param>
        /// <param name="Culture"></param>
        /// <param name="Columns"></param>
        /// <param name="CharLine"></param>
        /// <returns></returns>
        public Printer WritePriceLine(string Description, decimal Price, CultureInfo Culture = null, int? Columns = default, char CharLine = '.') => WritePair(Description, Price.ToString("C", Culture ?? CultureInfo.CurrentCulture), Columns, CharLine);

        /// <summary>
        /// Escreve uma lista de preços no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Culture"></param>
        /// <param name="Columns"></param>
        /// <param name="CharLine"></param>
        /// <returns></returns>
        public Printer WritePriceList(IEnumerable<Tuple<string, decimal>> List, CultureInfo Culture = null, int? Columns = default, char CharLine = '.')
        {
            foreach (var item in List ?? new List<Tuple<string, decimal>>())
            {
                WritePriceLine(item.Item1, item.Item2, Culture, Columns, CharLine);
            }

            return this;
        }

        /// <summary>
        /// Escreve uma lista de preços no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <param name="Culture"></param>
        /// <param name="Columns"></param>
        /// <param name="CharLine"></param>
        /// <returns></returns>
        public Printer WritePriceList<T>(IEnumerable<T> List, Expression<Func<T, string>> Description, Expression<Func<T, decimal>> Price, CultureInfo Culture = null, int? Columns = default, char CharLine = '.') => WritePriceList(List.Select(x => new Tuple<string, decimal>(Description.Compile()(x), Price.Compile()(x))), Culture, Columns);

        public Printer WriteScriptLine(int? Columns = default, string Name = InnerLibs.Ext.EmptyString)
        {
            ResetFont();
            NewLine(5);
            AlignCenter();
            Separator('_', Columns);
            WriteLine(Name, Name.IsNotBlank());
            ResetFont();
            return this;
        }

        /// <summary>
        /// Escreve uma tabela no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <returns></returns>
        public Printer WriteTable<T>(IEnumerable<T> Items) where T : class => Write(Console.ConsoleTable.From(Items).ToString());

        /// <summary>
        /// Escreve uma tabela no <see cref="DocumentBuffer"/>
        /// </summary>
        /// <returns></returns>
        public Printer WriteTable<T>(params T[] Items) where T : class => WriteTable(Items.AsEnumerable());

        /// <summary>
        /// Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas
        /// pelo valor da propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="Objects"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, bool PartialCutOnEach, params T[] Objects)
        {
            if (TemplateString.IsNotBlank())
            {
                Objects = Objects ?? Array.Empty<T>();
                if (TemplateString.IsFilePath())
                {
                    if (File.Exists(TemplateString))
                    {
                        TemplateString = File.ReadAllText(TemplateString);
                    }
                }

                foreach (var item in Objects)
                {
                    string ns = TemplateString.Inject(item);
                    WriteLine(ns.SplitAny(PredefinedArrays.BreakLineChars.ToArray()));
                    if (PartialCutOnEach)
                    {
                        PartialPaperCut();
                    }
                    else
                    {
                        Separator();
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas
        /// pelo valor da propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, params T[] obj) where T : class => WriteTemplate(TemplateString, false, obj);

        /// <summary>
        /// Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas
        /// pelo valor da propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="obj"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, IEnumerable<T> obj, bool PartialCutOnEach = false) where T : class => WriteTemplate(TemplateString, PartialCutOnEach, (obj ?? Array.Empty<T>()).ToArray());

        /// <summary>
        /// Escreve um template substituindo as marcações {Propriedade} encontradas pelo valor da
        /// propriedade equivalente em <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TemplateString"></param>
        /// <param name="obj"></param>
        /// <param name="PartialCutOnEach"></param>
        /// <returns></returns>
        public Printer WriteTemplate<T>(string TemplateString, T obj, bool PartialCutOnEach = false) where T : class => WriteTemplate(TemplateString, PartialCutOnEach, InnerLibs.Ext.ForceArray(obj, typeof(T)));

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
            MediumFontSize();
            WriteLine("Font Size 2");
            LargeFontSize();
            WriteLine("Font Size 3");
            NormalFontSize();
            WriteLine("Normal Font Size");
            Separator();
            AlignRight();
            WriteLine("AlignRight");
            AlignCenter();
            WriteLine("AlignCenter");
            AlignLeft();
            WriteLine("AlignLeft");
            NewLine(3);
            WriteLine("Accents");
            TestDiacritics();
            WriteLine("EOF :)");
            Separator();
            PartialPaperCut();
            return this;
        }

        #endregion Public Methods
    }
}

namespace InnerLibs.Printer.XmlTemplates
{
    public class XmlTemplatePrinter : Printer
    {
        #region Public Methods

        /// <summary>
        /// Escreve um template de um <see cref="XDocument"/> para cada entrada em uma lista
        /// substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public Printer WriteXmlTemplate<T>(T obj, XmlDocument Xml)
        {
            return WriteXmlTemplate(obj, Xml.OuterXml);
        }

        /// <summary>
        /// Escreve um template de um <see cref="XDocument"/> para cada entrada em uma lista
        /// substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public Printer WriteXmlTemplate<T>(IEnumerable<T> Item, XmlDocument Xml) => WriteXmlTemplate(Item, Xml.OuterXml);

        /// <summary>
        /// Escreve um template de um <see cref="XDocument"/> para cada entrada em uma lista
        /// substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public Printer WriteXmlTemplate<T>(T Item, XmlNode Xml) => WriteXmlTemplate(Item, Xml.OuterXml);

        public Printer WriteXmlTemplate<T>(IEnumerable<T> Item, XmlNode Xml) => WriteXmlTemplate(Item, Xml.OuterXml);

        /// <summary>
        /// Escreve um template de um <see cref="XDocument"/> para cada entrada em uma lista
        /// substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Item"></param>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public Printer WriteXmlTemplate<T>(T Item, XDocument Xml) => WriteXmlTemplate(Item, Xml.Root);

        public Printer WriteXmlTemplate<T>(IEnumerable<T> Item, XDocument Xml) => WriteXmlTemplate(Item, Xml.Root);

        /// <summary>
        /// Escreve um template de um <see cref="XmlNode"/> para cada entrada em uma lista
        /// substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente
        /// </summary>
        public Printer WriteXmlTemplate<T>(IEnumerable<T> Items, XElement Xml)
        {
            foreach (var item in Items ?? Array.Empty<T>())
            {
                WriteXmlTemplate(item, Xml);
            }

            return this;
        }

        public Printer WriteXmlTemplate<T>(IEnumerable<T> Items, string Xml)
        {
            foreach (var item in Items ?? Array.Empty<T>())
            {
                WriteXmlTemplate(item, Xml);
            }

            return this;
        }

        public Printer WriteXmlTemplate<T>(T Item, string Xml)
        {
            var n = XDocument.Parse(Xml);
            return WriteXmlTemplate(Item, n);
        }

        /// <summary>
        /// Escreve um template de um <see cref="XmlNode"/> para o objeto designado substituindo as
        /// marcações {Propriedade} encontradas pelo valor da propriedade equivalente
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Item"></param>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public Printer WriteXmlTemplate<T>(T Item, XElement Xml)
        {
            int lines = 0;

            string name = Xml.Name.LocalName.ToLowerInvariant();
            if (name.IsIn("br"))
            {
                try
                {
                    lines = Xml.Attribute("count")?.Value.ToInt() ?? 1;
                }
                catch
                {
                    lines = 1;
                }

                return NewLine(lines);
            }

            if (name.IsIn("partialcut", "partialpapercut"))
            {
                return PartialPaperCut();
            }

            if (name.IsIn("cut", "fullcut", "fullpapercut", "hr"))
            {
                return FullPaperCut();
            }

            if (name.IsIn("sep", "separator"))
            {
                string sep = "-";
                foreach (var attr in Xml.Attributes())
                {
                    if (attr.Name.LocalName.ToLowerInvariant() == "char")
                    {
                        try
                        {
                            sep = attr.Value;
                        }
                        catch
                        {
                            sep = "-";
                        }
                    }
                }

                return Separator(sep.FirstOrDefault());
            }

            if (name.IsIn("list"))
            {
                string v = Xml.Attribute("property")?.Value;
                if (v.IsNotBlank())
                {
                    var prop = typeof(T).GetProperty(v);
                    if (prop != null && Item != null)
                    {
                        object[] itens = (object[])(prop.GetValue(Item) ?? new object[] { });
                        if (itens.Any())
                        {
                            if (Xml.HasElements)
                            {
                                foreach (XElement node in Xml.Nodes().WhereType<XNode, XElement>())
                                {
                                    WriteXmlTemplate(itens.AsEnumerable(), node);
                                }
                            }
                            else
                            {
                                WriteXmlTemplate(itens.AsEnumerable(), Xml.Value);
                            }
                        }
                    }
                }

                return this;
            }

            if (name.IsIn("pair"))
            {
                string dotchar = Xml.Attribute("char")?.Value;
                dotchar = dotchar.GetFirstChars().IfBlank(" ");
                var left = Xml.Descendants().FirstOrDefault(x => x.Name == "left");
                var right = Xml.Descendants().FirstOrDefault(x => x.Name == "right");
                string ltxt = left?.Value;
                string rtxt = right?.Value;
                if (Item != null)
                {
                    ltxt = ltxt.Inject(Item);
                    rtxt = rtxt.Inject(Item);
                }

                return WritePair(ltxt, rtxt, default, dotchar.FirstOrDefault());
            }

            if (Xml.HasElements)
            {
                foreach (XElement node in Xml.Nodes().WhereType<XNode, XElement>())
                {
                    WriteXmlTemplate(Item, node);
                }

                return this;
            }
            // se chegou aqui, é so tratar como texto mesmo
            if (name.IsIn("line", "writeline", "ln", "printl", "title", "h1", "h2", "h3", "h4", "h5", "h6"))
            {
                lines = 1;
            }

            foreach (var attr in Xml.Attributes())
            {
                string atname = attr.Name.LocalName.ToLowerInvariant();
                if (atname == "bold")
                {
                    Bold($"{attr.Value}".ToLowerInvariant().IfBlank("true").ToBool());
                }

                if (atname == "italic")
                {
                    Italic($"{attr.Value}".ToLowerInvariant().IfBlank("true").ToBool());
                }

                if (atname == "underline")
                {
                    UnderLine($"{attr.Value}".ToLowerInvariant().IfBlank("true").ToBool());
                }

                if (atname == "lines")
                {
                    try
                    {
                        lines = (int)attr.Value?.ToInt();
                    }
                    catch
                    {
                        lines = 0;
                    }
                }

                if (atname == "align")
                {
                    switch (attr.Value?.ToLowerInvariant() ?? InnerLibs.Ext.EmptyString)
                    {
                        case "right":
                            {
                                AlignRight();
                                break;
                            }

                        case "center":
                            {
                                AlignCenter();
                                break;
                            }

                        default:
                            {
                                AlignLeft();
                                break;
                            }
                    }
                }

                if (atname == "font-size")
                {
                    switch (attr.Value?.ToLowerInvariant() ?? InnerLibs.Ext.EmptyString)
                    {
                        case "2":
                        case "medium":
                            {
                                MediumFontSize();
                                break;
                            }

                        case "3":
                        case "large":
                            {
                                LargeFontSize();
                                break;
                            }

                        default:
                            {
                                NormalFontSize();
                                break;
                            }
                    }
                }

                if (atname == "font-stretch")
                {
                    switch (attr.Value?.ToLowerInvariant() ?? InnerLibs.Ext.EmptyString)
                    {
                        case "2":
                        case "condensed":
                            {
                                Condensed();
                                break;
                            }

                        case "3":
                        case "expanded":
                            {
                                Expanded();
                                break;
                            }

                        default:
                            {
                                NotCondensed().NotExpanded();
                                break;
                            }
                    }
                }
            }

            string txt = Xml.Value;
            if (Item != null)
            {
                txt = txt.Inject(Item);
            }

            Write(txt);
            if (lines > 0)
            {
                NewLine(lines);
            }

            AlignLeft();
            ResetFont();
            return this;
        }

        #endregion Public Methods
    }
}