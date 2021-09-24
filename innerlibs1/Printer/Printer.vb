' ***********************************************************************
' Assembly         : InnerLibs
' Author           : Leandro Ferreira / Zonaro
' Created          : 16-03-2019
'
' ***********************************************************************
' <copyright file="Printer.vb" company="InnerCodeTech">

'		        		   The MIT License (MIT)
'	     		    Copyright (c) 2019 InnerCodeTech
'
'	 Permission is hereby granted, free of charge, to any person obtaining
' a copy of this software and associated documentation files (the "Software"),
' to deal in the Software without restriction, including without limitation
' the rights to use, copy, modify, merge, publish, distribute, sublicense,
' and/or sell copies of the Software, and to permit persons to whom the
' Software is furnished to do so, subject to the following conditions:
'	 The above copyright notice and this permission notice shall be
' included in all copies or substantial portions of the Software.
'	 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
' EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
' MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
' IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
' DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
' ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' </copyright>
' ***********************************************************************

Imports System.Globalization
Imports System.IO
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text
Imports InnerLibs.Printer.Command

Namespace Printer

    Public Module PrinterExtension

        <Extension()> Function CreatePrinter(CommandType As IPrintCommand, PrinterName As String, Optional ColsNormal As Integer = 0, Optional ColsCondensed As Integer = 0, Optional ColsExpanded As Integer = 0, Optional Encoding As Encoding = Nothing) As Printer
            Return Printer.CreatePrinter(CommandType, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Function

    End Module





    Public Class Printer
        Private Class PrinterWriter
            Inherits TextWriter

            Private p As Printer

            Public Sub New(p As Printer)
                Me.p = p
            End Sub

            Public Sub New(formatProvider As IFormatProvider, p As Printer)
                MyBase.New(formatProvider)
                Me.p = p
            End Sub

            Public Overrides Sub Write(value As Char)
                Me.Write($"{value}")
            End Sub

            Public Overrides Sub Flush()
                p.PrintDocument()
            End Sub

            Public Overrides ReadOnly Property Encoding As Encoding
                Get
                    Return p.Command?.Encoding
                End Get
            End Property
        End Class

        Private txw = New PrinterWriter(Me)

        ''' <summary>
        ''' TextWriter interno desta Printer
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property TextWriter As TextWriter
            Get
                Return txw
            End Get
        End Property

        Public Shared Function CreatePrinter(Of CommandType As IPrintCommand)(PrinterName As String, Optional ColsNormal As Integer = 0, Optional ColsCondensed As Integer = 0, Optional ColsExpanded As Integer = 0, Optional Encoding As Encoding = Nothing) As Printer
            Return CreatePrinter(GetType(CommandType), PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Function

        Public Shared Function CreatePrinter(CommandType As Type, PrinterName As String, Optional ColsNormal As Integer = 0, Optional ColsCondensed As Integer = 0, Optional ColsExpanded As Integer = 0, Optional Encoding As Encoding = Nothing) As Printer
            Return CreatePrinter(Activator.CreateInstance(CommandType), PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Function

        Public Shared Function CreatePrinter(CommandType As IPrintCommand, PrinterName As String, Optional ColsNormal As Integer = 0, Optional ColsCondensed As Integer = 0, Optional ColsExpanded As Integer = 0, Optional Encoding As Encoding = Nothing) As Printer
            Return New Printer(CommandType, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Function

        Private _ommit As Boolean = False
        Private FontMode As String = "Normal"
        Private Align As String = "Left"

        Public Property DocumentBuffer As Byte()

        Public Property AutoPrint As Boolean = False

        Public ReadOnly Property HTMLDocument As XDocument = XDocument.Parse("<body><link rel='stylesheet' href='Printer.css' /></body>")

        Public Property PrinterName As String

        Public Property ColsNomal As Integer

        Public Property ColsCondensed As Integer

        Public Property ColsExpanded As Integer

        Public ReadOnly Property Command As IPrintCommand

        Public Property Diacritics As Boolean = True

        Public Property RewriteFunction As Func(Of String, String) = Nothing

        Public ReadOnly Property IsItalic As Boolean

        Public ReadOnly Property IsBold As Boolean

        Public ReadOnly Property IsUnderline As Boolean

        Public Property IsCondensed As Boolean
            Get
                Return FontMode = "Condensed"
            End Get
            Set(value As Boolean)
                If value Then
                    FontMode = "Condensed"
                Else
                    FontMode = "Normal"
                End If
            End Set
        End Property

        Public Property IsExpanded As Boolean
            Get
                Return FontMode = "Expanded"
            End Get
            Set(value As Boolean)
                If value Then
                    FontMode = "Expanded"
                Else
                    FontMode = "Normal"
                End If
            End Set
        End Property

        Public Property IsLarge As Boolean
            Get
                Return FontMode = "Double2"
            End Get
            Set(value As Boolean)
                If value Then
                    FontMode = "Double2"
                Else
                    FontMode = "Normal"
                End If
            End Set
        End Property

        Public Property IsLarger As Boolean
            Get
                Return FontMode = "Double3"
            End Get
            Set(value As Boolean)
                If value Then
                    FontMode = "Double3"
                Else
                    FontMode = "Normal"
                End If
            End Set
        End Property

        Public ReadOnly Property IsNormal As Boolean
            Get
                Return Not IsCondensed AndAlso Not IsExpanded
            End Get
        End Property

        Public ReadOnly Property IsLeftAligned As Boolean
            Get
                Return Align = "Left"
            End Get
        End Property

        Public ReadOnly Property IsRightAligned As Boolean
            Get
                Return Align = "Right"
            End Get
        End Property

        Public ReadOnly Property IsCenterAligned As Boolean
            Get
                Return Align = "Center"
            End Get
        End Property



        Public Sub New(ByVal Encoding As Encoding)
            Me.New(Nothing, Nothing, 0, 0, 0, Encoding)
        End Sub

        Public Sub New(ByVal Command As IPrintCommand)
            Me.New(Command, Nothing, 0, 0, 0, Nothing)
        End Sub

        Public Sub New(ByVal Command As IPrintCommand, Encoding As Encoding)
            Me.New(Command, Nothing, 0, 0, 0, Encoding)
        End Sub

        Public Sub New()
            Me.New(Nothing, Nothing, 0, 0, 0, Nothing)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="PrinterName">Printer name, shared name or port of printer install</param>
        ''' <param name="ColsNormal">Number of columns for normal mode print</param>
        ''' <param name="ColsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="ColsExpanded">Number of columns for expanded mode print</param>
        ''' <param name="Encoding">Custom Encoding</param>
        Public Sub New(ByVal PrinterName As String, ByVal ColsNormal As Integer, ByVal ColsCondensed As Integer, ByVal ColsExpanded As Integer, ByVal Encoding As Encoding)
            Me.New(Nothing, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="PrinterName">Printer name, shared name or port of printer install</param>
        ''' <param name="ColsNormal">Number of columns for normal mode print</param>
        ''' <param name="ColsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="ColsExpanded">Number of columns for expanded mode print</param>
        ''' <param name="Encoding">Custom Encoding</param>
        Public Sub New(Command As IPrintCommand, ByVal PrinterName As String, ByVal ColsNormal As Integer, ByVal ColsCondensed As Integer, ByVal ColsExpanded As Integer, ByVal Encoding As Encoding)
            Me.Command = If(Command, New EscPosCommands.EscPos())
            If Encoding IsNot Nothing Then
                Me.Command.Encoding = Encoding
            End If
            If Me.Command.Encoding Is Nothing Then
                Me.Command.Encoding = Encoding.Default
            End If
            Me.PrinterName = PrinterName.IfBlank("temp.prn").Trim()
            Me.ColsNomal = If(ColsNormal <= 0, Me.Command.ColsNomal, ColsNormal)
            Me.ColsCondensed = If(ColsCondensed <= 0, Me.Command.ColsCondensed, ColsCondensed)
            Me.ColsExpanded = If(ColsExpanded <= 0, Me.Command.ColsExpanded, ColsExpanded)
        End Sub

        ''' <summary>
        '''Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="PrinterName">Printer name, shared name or port of printer install</param>
        ''' <param name="ColsNormal">Number of columns for normal mode print</param>
        ''' <param name="ColsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="ColsExpanded">Number of columns for expanded mode print</param>
        Public Sub New(ByVal PrinterName As String, ByVal ColsNormal As Integer, ByVal ColsCondensed As Integer, ByVal ColsExpanded As Integer)
            Me.New(PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Nothing)
        End Sub

        ''' <summary>
        '''Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="PrinterName">Printer name, shared name or port of printer install</param>
        ''' <param name="Encoding">Custom Encoding</param>
        Public Sub New(ByVal PrinterName As String, ByVal Encoding As Encoding)
            Me.New(PrinterName, 0, 0, 0, Encoding)
        End Sub

        ''' <summary>
        '''Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="PrinterName">Printer name, shared name or port of printer install</param>
        Public Sub New(ByVal PrinterName As String)
            Me.New(PrinterName, 0, 0, 0, Nothing)
        End Sub

        'substitui caracteres para caracteres acentuados não disponíveis na fonte DOS
        <Obsolete> Public Shared Function FixAccents(ByVal Lin As String) As String
            Dim T1 As String, T2 As String, i As Integer, p As Integer, C As String
            T1$ = "áéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕàèìòùÈÌÒçÇ" 'tela
            T2$ = "160130161162163181144214224233131136140147150182210215226219198228199229133138141149151212222227135128" ' ASC impressora
            p = 1
            For i = 1 To Len(Lin$)                                      'cada letra
                C$ = Mid$(Lin$, i, 1)                                      'pega o char
                p = InStr(T1$, C$)                                         'tem acento correspondente?
                If p Then                                                  'tem...
                    'troca usando backspace: letra + bs + acento
                    Lin$ = Left$(Lin$, i - 1) & Chr(Val(Mid$(T2$, (p * 3) - 2, 3))) & Mid$(Lin$, i + 1)                                  'troca
                End If
            Next
            Return Lin$
        End Function

        ''' <summary>
        ''' Funcao que reescreve o valor antes de chamar o <see cref="Write(String, Boolean)"/>
        ''' </summary>
        ''' <param name="StringAction"></param>
        ''' <returns></returns>
        Public Function UseRewriteFunction(StringAction As Func(Of String, String)) As Printer
            Me.RewriteFunction = StringAction
            Return Me
        End Function

        ''' <summary>
        ''' Remove a função de reescrita de valor definida pela <see cref="UseRewriteFunction(Func(Of String, String))"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function RemoveRewriteFunction() As Printer
            RewriteFunction = Nothing
            Return Me
        End Function

        ''' <summary>
        ''' Permite a ultilização de acentos nas chamadas <see cref="Write(String, Boolean)"/> posteriores
        ''' </summary>
        ''' <param name="OnOff"></param>
        ''' <returns></returns>
        Public Function UseDiacritics(Optional OnOff As Boolean = True) As Printer
            Diacritics = OnOff
            Return Me
        End Function

        ''' <summary>
        ''' Remove todos os acentod das chamadas <see cref="Write(String, Boolean)"/> posteriores
        ''' </summary>
        ''' <returns></returns>
        Public Function DontUseDiacritics() As Printer
            Return UseDiacritics(False)
        End Function

        ''' <summary>
        ''' Adciona um numero <paramref name="Lines"/> de quebras de linha ao <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="Lines"></param>
        ''' <returns></returns>
        Public Shadows Function NewLine(Optional Lines As Integer = 1) As Printer
            While (Lines > 0)
                Me.Write(Me.Command.Encoding.GetBytes(vbNewLine))
                Lines = Lines - 1
            End While
            Return Me
        End Function

        ''' <summary>
        ''' Adciona um numero <paramref name="Spaces"/> de espaços em branco ao <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="Spaces"></param>
        ''' <returns></returns>
        Public Function Space(Optional Spaces As Integer = 1) As Printer
            While (Spaces > 0)
                Write(Me.Command.Encoding.GetBytes(" "))
                Spaces = Spaces - 1
            End While
            Return Me
        End Function

        ''' <summary>
        ''' Limpa o <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function Clear() As Printer
            DocumentBuffer = Nothing
            HTMLDocument.Root.RemoveAll()
            HTMLDocument.Root.Add(<link rel='stylesheet' href='Printer.css'/>)
            Return Me
        End Function

        ''' <summary>
        ''' Escreve um separador 
        ''' </summary>
        ''' <param name="Character"></param>
        ''' <returns></returns>
        Public Function Separator(Optional Character As Char = "-"c, Optional Columns As Integer? = Nothing) As Printer
            Return WriteLine(New String(Character, If(Columns, GetCurrentColumns())))
        End Function

        ''' <summary>
        ''' Imprime o auto-teste da impressora
        ''' </summary>
        ''' <returns></returns>
        Public Function AutoTest() As Printer
            _ommit = True
            Return Write(Command.AutoTest())
        End Function

        ''' <summary>
        ''' Testa os acentos para esta impressora
        ''' </summary>
        ''' <returns></returns>
        Public Function TestDiacritics() As Printer
            Return WriteLine("áéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕàèìòùÈÌÒçÇ")
        End Function

        Public Function Italic(Optional state As Boolean = True) As Printer
            _ommit = True
            _IsItalic = state
            Return Write(Command.Italic(state))
        End Function

        Public Function NotItalic() As Printer
            Return Italic(False)
        End Function

        Public Function Bold(Optional state As Boolean = True) As Printer
            _ommit = True
            _IsBold = state
            Return Write(Command.Bold(state))
        End Function

        Public Function NotBold() As Printer
            Return Bold(False)
        End Function

        Public Function UnderLine(Optional state As Boolean = True) As Printer
            _ommit = True
            _IsUnderline = state
            Return Write(Command.Underline(state))
        End Function

        Public Function NotUnderline() As Printer
            Return UnderLine(False)
        End Function

        Public Function Expanded(Optional state As Boolean = True) As Printer
            _ommit = True
            IsExpanded = state
            Return Write(Command.Expanded(state))
        End Function

        Public Function NotExpanded() As Printer
            Return Expanded(False)
        End Function

        Public Function Condensed(Optional state As Boolean = True) As Printer
            IsCondensed = state
            _ommit = True
            Return Write(Command.Condensed(state))
        End Function

        Public Function NotCondensed() As Printer
            Return Condensed(False)
        End Function

        Public Function NormalFont() As Printer
            FontMode = "Normal"
            _ommit = True
            Return Write(Command.NormalFont())
        End Function

        Public Function LargeFont() As Printer
            IsLarge = True
            _ommit = True
            Return Write(Command.LargeFont())
        End Function

        Public Function LargerFont() As Printer
            IsLarger = True
            _ommit = True
            Return Write(Command.LargerFont())
        End Function

        Public Function AlignLeft() As Printer
            Align = "Left"
            _ommit = True
            Return Write(Command.Left())
        End Function

        Public Function AlignRight() As Printer
            Align = "Right"
            _ommit = True
            Return Write(Command.Right())
        End Function

        Public Function AlignCenter() As Printer
            Align = "Center"
            _ommit = True
            Return Write(Command.Center())
        End Function

        Public Function FullPaperCut() As Printer
            HTMLDocument.Root.Add(<hr class='FullPaperCut'/>)
            _ommit = True
            Return Write(Command.FullCut())
        End Function

        Public Function PartialPaperCut() As Printer
            HTMLDocument.Root.Add(<hr class='PartialPaperCut'/>)
            _ommit = True
            Return Write(Command.PartialCut())
        End Function

        Public Function OpenDrawer() As Printer
            _ommit = True
            Return Write(Command.OpenDrawer())
        End Function

        Public Function QrCode(ByVal qrData As String) As Printer
            _ommit = True
            Return Write(Command.PrintQrData(qrData))
        End Function

        Public Function QrCode(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Printer
            _ommit = True
            Return Write(Command.PrintQrData(qrData, qrCodeSize))
        End Function

        Public Function Code128(ByVal code As String) As Printer
            _ommit = True
            Return Write(Command.Code128(code))
        End Function

        Public Function Code39(ByVal code As String) As Printer
            _ommit = True
            Return Write(Command.Code39(code))
        End Function

        Public Function Ean13(ByVal code As String) As Printer
            _ommit = True
            Return Write(Command.Ean13(code))
        End Function

        Public Function InitializePrint() As Printer
            RawPrinterHelper.SendBytesToPrinter(PrinterName, Command.Initialize())
            Return Me
        End Function

        ''' <summary>
        ''' Retorna o numero de colunas  do modo atual
        ''' </summary>
        ''' <returns></returns>
        Public Function GetCurrentColumns() As Integer
            If IsCondensed Then
                Return Me.ColsCondensed
            ElseIf IsExpanded Then
                Return Me.ColsExpanded
            Else
                Return Me.ColsNomal
            End If
        End Function

        Public Function GetDotLine(LeftText As String, RightText As String, Optional Columns As Integer? = Nothing, Optional CharLine As Char = "."c) As String
            Columns = If(Columns, GetCurrentColumns())
            If Columns > 0 Then Return New String(CharLine, (Columns.Value - (LeftText.Length + RightText.Length)).LimitRange(0, Columns.Value))
            Return ""
        End Function

        ''' <summary>
        ''' Escreve os bytes contidos em <paramref name="value"/> no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Overloads Function Write(ByVal value As Byte()) As Printer
            If value IsNot Nothing AndAlso value.Any Then
                Dim list = New List(Of Byte)
                If DocumentBuffer IsNot Nothing Then list.AddRange(DocumentBuffer)
                list.AddRange(value)
                DocumentBuffer = list.ToArray
                If _ommit = False Then
                    Try
                        Dim v = Command.Encoding.GetString(value).ReplaceMany("<br/>", BreakLineChars.ToArray())
                        If v = "<br/>" Then
                            HTMLDocument.Root.Add(<br/>)
                        Else
                            HTMLDocument.Root.Add(XElement.Parse($"<span class='align-{Align.ToLower()} font-{FontMode.ToLower()}{IsBold.AsIf(" bold")}{IsItalic.AsIf(" italic")}{IsUnderline.AsIf(" underline")}'>{v.Replace(" ", "&nbsp;")}</span>"))
                        End If
                    Catch ex As Exception
                    End Try
                End If
                _ommit = False

                If AutoPrint Then
                    PrintDocument()
                End If

            End If
            Return Me
        End Function

        ''' <summary>
        ''' Escreve o <paramref name="value"/> no <see cref="DocumentBuffer"/> se <paramref name="Test"/> for TRUE
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="Test"></param>
        ''' <returns></returns>
        Public Overloads Function Write(ByVal value As String, Optional Test As Boolean = True) As Printer
            If Test Then
                If value.ContainsAny(BreakLineChars.ToArray()) Then
                    For Each line In value.SplitAny(BreakLineChars.ToArray())
                        WriteLine(line, Test AndAlso line.IsNotBlank)
                    Next
                Else
                    If value.IsNotBlank Then
                        If Not Diacritics Then
                            value = value.RemoveDiacritics()
                        End If
                        If RewriteFunction IsNot Nothing Then
                            value = RewriteFunction.Invoke(value)
                        End If
                        Write(Me.Command.Encoding.GetBytes(value))
                    End If
                End If
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Escreve o <paramref name="value"/> se <paramref name="Test"/> for TRUE e quebra uma linha
        ''' </summary>
        ''' <param name="value"></param>
        ''' <param name="Test"></param>
        ''' <returns></returns>
        Public Shadows Function WriteLine(ByVal value As String, Test As Boolean) As Printer
            Return If(Test, Write(value, Test).NewLine(), Me)
        End Function


        ''' <summary>
        ''' Escreve o <paramref name="value"/>   e quebra uma linha
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Shadows Function WriteLine(ByVal value As String) As Printer
            Return WriteLine(value, True)
        End Function


        ''' <summary>
        ''' Escreve varias linhas no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Public Shadows Function WriteLine(ParamArray values As String()) As Printer
            values = If(values, {}).Where(Function(x) x.IsNotBlank())
            If values.Any() Then
                WriteLine(values.Join(vbNewLine))
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Escreve um teste de 48 colunas no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function WriteTest() As Printer
            NewLine()
            AlignLeft()
            WriteLine("INNERLIBS TEST PRINTER - 48 COLUMNS")
            WriteLine("....+....1....+....2....+....3....+....4....+...")
            Separator()
            WriteLine("Default Text")
            Italic().WriteLine("Italic Text").NotItalic()
            Bold().WriteLine("Bold Text").NotBold()
            UnderLine.WriteLine("UnderLine Text").NotUnderline()
            Expanded().WriteLine("Expanded Text").WriteLine("....+....1....+....2....").NotExpanded()
            Condensed().WriteLine("Condensed Text").NotCondensed()
            Separator()
            LargeFont()
            WriteLine("Font Size 2")
            LargerFont()
            WriteLine("Font Size 3")
            NormalFont()
            WriteLine("Normal Font Size")
            Separator()
            AlignRight()
            WriteLine("Text on Right")
            AlignCenter()
            WriteLine("Text on Center")
            AlignLeft()
            WriteLine("Text on Left")
            NewLine(3)
            WriteLine("ACENTOS")
            TestDiacritics()
            WriteLine("EOF :)")
            Separator()
            NewLine()
            PartialPaperCut()
            Return Me
        End Function

        ''' <summary>
        ''' Escreve os valores de um Dictionary como pares
        ''' </summary>
        ''' <typeparam name="T1"></typeparam>
        ''' <typeparam name="T2"></typeparam>
        ''' <param name="dics"></param>
        ''' <returns></returns>
        Public Function WriteDictionary(Of T1, T2)(dics As IEnumerable(Of IDictionary(Of T1, T2)), Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteDictionary(PartialCutOnEach, If(dics, {}).ToArray())
        End Function

        ''' <summary>
        ''' Escreve os valores de um Dictionary como pares
        ''' </summary>
        ''' <typeparam name="T1"></typeparam>
        ''' <typeparam name="T2"></typeparam>
        ''' <param name="dic"></param>
        ''' <returns></returns>
        Public Function WriteDictionary(Of T1, T2)(dic As IDictionary(Of T1, T2), Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteDictionary(PartialCutOnEach, {dic})
        End Function

        ''' <summary>
        ''' Escreve os valores de um Dictionary como pares
        ''' </summary>
        ''' <typeparam name="T1"></typeparam>
        ''' <typeparam name="T2"></typeparam>
        ''' <param name="dics"></param>
        ''' <returns></returns>
        Public Function WriteDictionary(Of T1, T2)(ParamArray dics As IDictionary(Of T1, T2)()) As Printer
            Return WriteDictionary(False, dics)
        End Function

        ''' <summary>
        ''' Escreve os valores de um Dictionary como pares
        ''' </summary>
        ''' <typeparam name="T1"></typeparam>
        ''' <typeparam name="T2"></typeparam>
        ''' <param name="dics"></param>
        ''' <returns></returns>
        Public Function WriteDictionary(Of T1, T2)(PartialCutOnEach As Boolean, ParamArray dics As IDictionary(Of T1, T2)()) As Printer
            dics = If(dics, {})
            For Each dic In dics
                If dic IsNot Nothing Then
                    If PartialCutOnEach Then PartialPaperCut() Else Separator()
                    For Each item In dic
                        WritePair($"{item.Key}".ToNormalCase(), item.Value)
                    Next
                    AlignLeft()
                End If
            Next
            If dics.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            Return Me
        End Function

        ''' <summary>
        ''' Escreve uma lista de itens no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="Items"></param>
        ''' <param name="ListOrdenator"></param>
        ''' <returns></returns>
        Public Function WriteList(Items As IEnumerable(Of Object), Optional ListOrdenator As String = Nothing) As Printer
            For index = 0 To Items.Count - 1
                WriteLine($"{If(ListOrdenator, $"{index + 1} ")}{Items(index)}")
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Escreve uma lista de itens no <see cref="DocumentBuffer"/>
        ''' </summary>
        Public Function WriteList(ParamArray Items As Object()) As Printer
            Return WriteList(If(Items, {}).AsEnumerable)
        End Function

        ''' <summary>
        ''' Escreve um par de infomações no <see cref="DocumentBuffer"/>.
        ''' </summary>
        Public Function WritePair(Key As Object, Value As Object, Optional Columns As Integer? = Nothing, Optional CharLine As Char = " "c) As Printer
            Columns = If(Columns, GetCurrentColumns())
            Dim dots = ""
            Dim s1 = $"{Key}"
            Dim s2 = $"{Value}"
            If s2.IsNotBlank() AndAlso Columns.Value > 0 Then
                dots = GetDotLine(s1, s2, Columns, CharLine)
            Else
                dots = " "
            End If
            Dim s = $"{s1}{dots}{s2}"
            Return WriteLine(s, s.IsNotBlank())
        End Function

        ''' <summary>
        ''' Escreve uma linha de preço no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="Description"></param>
        ''' <param name="Price"></param>
        ''' <param name="Culture"></param>
        ''' <param name="Columns"></param>
        ''' <param name="CharLine"></param>
        ''' <returns></returns>
        Public Function WritePriceLine(Description As String, Price As Decimal, Optional Culture As CultureInfo = Nothing, Optional Columns As Integer? = Nothing, Optional CharLine As Char = "."c) As Printer
            Dim sprice = Price.ToString("C", If(Culture, CultureInfo.CurrentCulture))
            Return WritePair(Description, sprice, Columns, CharLine)
        End Function

        ''' <summary>
        ''' Escreve uma lista de preços no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="Culture"></param>
        ''' <param name="Columns"></param>
        ''' <param name="CharLine"></param>
        ''' <returns></returns>
        Public Function WritePriceList(List As IEnumerable(Of Tuple(Of String, Decimal)), Optional Culture As CultureInfo = Nothing, Optional Columns As Integer? = Nothing, Optional CharLine As Char = "."c) As Printer
            For Each item In List.NullAsEmpty()
                WritePriceLine(item.Item1, item.Item2, Culture, Columns, CharLine)
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Escreve uma lista de preços no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <param name="Culture"></param>
        ''' <param name="Columns"></param>
        ''' <param name="CharLine"></param>
        ''' <returns></returns>
        Public Function WritePriceList(Of T)(List As IEnumerable(Of T), Description As Expression(Of Func(Of T, String)), Price As Expression(Of Func(Of T, Decimal)), Optional Culture As CultureInfo = Nothing, Optional Columns As Integer? = Nothing, Optional CharLine As Char = "."c) As Printer
            Return WritePriceList(List.Select(Function(x) New Tuple(Of String, Decimal)(Description.Compile()(x), Price.Compile()(x))), Culture, Columns)
        End Function

        ''' <summary>
        ''' Escreve uma tabela no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function WriteTable(Of T As Class)(Items As IEnumerable(Of T)) As Printer
            Write(ConsoleTables.ConsoleTable.From(Items).ToString())
            Return Me
        End Function

        ''' <summary>
        ''' Escreve uma tabela no <see cref="DocumentBuffer"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function WriteTable(Of T As Class)(ParamArray Items As T()) As Printer
            Return WriteTable(Items.AsEnumerable)
        End Function

        ''' <summary>
        ''' Escreve as Propriedades e valores de uma classe como pares
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="objs"></param>
        ''' <returns></returns>
        Public Function WriteClass(Of T As Class)(ParamArray objs As T()) As Printer
            Return WriteClass(False, objs)
        End Function

        ''' <summary>
        ''' Escreve as Propriedades e valores de uma classe como pares
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="objs"></param>
        ''' <param name="PartialCutOnEach"></param>
        ''' <returns></returns>
        Public Function WriteClass(Of T As Class)(PartialCutOnEach As Boolean, ParamArray objs As T()) As Printer
            objs = If(objs, {})
            For Each obj In objs
                If obj IsNot Nothing Then
                    If PartialCutOnEach Then PartialPaperCut() Else Separator()
                    For Each item In obj.GetNullableTypeOf().GetProperties()
                        If item.CanRead Then
                            WritePair(item.Name.ToNormalCase(), item.GetValue(obj))
                        End If
                    Next
                    AlignLeft()
                End If
            Next
            If objs.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            Return Me
        End Function

        ''' <summary>
        ''' Escreve as Propriedades e valores de uma classe como pares
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="obj"></param>
        ''' <param name="PartialCutOnEach"></param>
        ''' <returns></returns>
        Public Function WriteClass(Of T As Class)(ByVal obj As IEnumerable(Of T), Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteClass(PartialCutOnEach, If(obj, {}).ToArray())
        End Function

        ''' <summary>
        ''' Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="TemplateString"></param>
        ''' <param name="obj"></param>
        ''' <param name="PartialCutOnEach"></param>
        ''' <returns></returns>
        Public Function WriteTemplate(Of T)(TemplateString As String, PartialCutOnEach As Boolean, ParamArray obj As T()) As Printer
            If TemplateString.IsNotBlank Then
                obj = If(obj, {})
                If TemplateString.IsFilePath Then
                    If File.Exists(TemplateString) Then
                        TemplateString = File.ReadAllText(TemplateString)
                    End If
                End If
                For Each item In obj
                    Dim ns = TemplateString.Inject(item)
                    For Each linha In ns.SplitAny(BreakLineChars.ToArray())
                        WriteLine(linha)
                    Next
                Next
                If obj.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="TemplateString"></param>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        Public Function WriteTemplate(Of T As Class)(TemplateString As String, ParamArray obj As T()) As Printer
            Return WriteTemplate(TemplateString, False, obj)
        End Function

        ''' <summary>
        ''' Escreve um template para uma lista substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="TemplateString"></param>
        ''' <param name="obj"></param>
        ''' <param name="PartialCutOnEach"></param>
        ''' <returns></returns>
        Public Function WriteTemplate(Of T As Class)(TemplateString As String, obj As IEnumerable(Of T), Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteTemplate(TemplateString, PartialCutOnEach, If(obj, {}).ToArray())
        End Function

        ''' <summary>
        ''' Escreve um template substituindo as marcações {Propriedade} encontradas pelo valor da propriedade equivalente em <typeparamref name="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="TemplateString"></param>
        ''' <param name="obj"></param>
        ''' <param name="PartialCutOnEach"></param>
        ''' <returns></returns>
        Public Function WriteTemplate(Of T As Class)(TemplateString As String, obj As T, Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteTemplate(TemplateString, PartialCutOnEach, ForceArray(obj))
        End Function

        ''' <summary>
        ''' Escreve uma data usando formato especifico
        ''' </summary>
        ''' <param name="Format"></param>
        ''' <returns></returns>
        Public Function WriteDate([DateAndTime] As Date, Optional Format As String = Nothing)
            If Format.IsNotBlank Then
                Return Write(DateAndTime.ToString(Format))
            Else
                Return Write(DateAndTime.ToString())
            End If
        End Function

        ''' <summary>
        ''' Escreve uma data usando uma Cultura especifica
        ''' </summary>
        ''' <param name="Format"></param>
        ''' <returns></returns>
        Public Function WriteDate([DateAndTime] As Date, Optional Format As CultureInfo = Nothing)
            If Format IsNot Nothing Then
                Return Write(DateAndTime.ToString(Format))
            Else
                Return Write(DateAndTime.ToString())
            End If
        End Function

        ''' <summary>
        ''' Escreve a data atual usando formato especifico
        ''' </summary>
        ''' <param name="Format"></param>
        ''' <returns></returns>
        Public Function WriteDate(Optional Format As String = Nothing)
            Return WriteDate(DateTime.Now, Format)
        End Function

        ''' <summary>
        ''' Escreve a data atual usando uma cultura especifica
        ''' </summary>
        ''' <param name="Format"></param>
        ''' <returns></returns>
        Public Function WriteDate(Optional Format As CultureInfo = Nothing)
            Return WriteDate(DateTime.Now, Format)
        End Function

        ''' <summary>
        ''' Imprime o conteudo do <see cref="DocumentBuffer"/> atual e limpa o buffer
        ''' </summary>
        ''' <param name="Copies"></param>
        ''' <returns></returns>
        Public Function PrintDocument(Optional Copies As Integer = 1) As Printer
            Return PrintDocument(DocumentBuffer, Copies).Clear()
        End Function

        ''' <summary>
        ''' Imprime o conteudo de um arquivo ou o conteudo de todos os arquivos de um diretorio
        ''' </summary>
        ''' <param name="FileOrDirectoryPath"></param>
        ''' <param name="Copies"></param>
        ''' <returns></returns>
        Public Function PrintDocument(FileOrDirectoryPath As String, Optional Copies As Integer = 1) As Printer

            If FileOrDirectoryPath.IsDirectoryPath Then
                If Directory.Exists(FileOrDirectoryPath) Then
                    For Each item In Directory.GetFiles(FileOrDirectoryPath)
                        PrintDocument(item, Copies)
                    Next
                End If
            ElseIf FileOrDirectoryPath.IsFilePath Then
                If File.Exists(FileOrDirectoryPath) Then
                    PrintDocument(File.ReadAllBytes(FileOrDirectoryPath), Copies)
                End If
            Else
                Throw New ArgumentException($"FileOrDirectoryPath Is Not a valid Path: {FileOrDirectoryPath}")

            End If
            Return Me
        End Function

        ''' <summary>
        ''' Envia os Bytes para a impressora ou arquivo
        ''' </summary>
        ''' <param name="Copies"></param>
        ''' <returns></returns>
        Public Function PrintDocument(Bytes As Byte(), Optional Copies As Integer = 1) As Printer
            If Bytes IsNot Nothing AndAlso Bytes.Any Then
                For i = 0 To Copies.SetMinValue(1) - 1
                    If PrinterName.IsFilePath Then
                        SaveFile(PrinterName, False)
                    Else
                        If Not RawPrinterHelper.SendBytesToPrinter(PrinterName, DocumentBuffer.ToArray()) Then Throw New ArgumentException("Não foi possível acessar a impressora: " & PrinterName)
                    End If
                Next
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Escreve um Arquivo com os dados binarios e HTML desta impressao
        ''' </summary>
        ''' <param name="FileOrDirectoryPath"></param>
        ''' <returns></returns>
        Public Function SaveFile(FileOrDirectoryPath As String, Optional IncludeHtmlDoc As Boolean = False) As Printer
            If DocumentBuffer IsNot Nothing AndAlso DocumentBuffer.Count > 0 Then
                If FileOrDirectoryPath.IsDirectoryPath Then
                    FileOrDirectoryPath = $"{FileOrDirectoryPath}\{Me.Command.GetTypeOf().Name}\{Me.PrinterName.ToFriendlyPathName()}\{DateTime.Now.Ticks}.{Me.Command?.GetTypeOf()?.Name.IfBlank("bin")}"
                    FileOrDirectoryPath = FileOrDirectoryPath.AdjustPathChars(True)
                End If
                If FileOrDirectoryPath.IsFilePath Then
                    Dim d = DateTime.Now
                    Dim info = DocumentBuffer.ToArray().WriteToFile(FileOrDirectoryPath, d)
                    If IncludeHtmlDoc Then
                        Dim s = $"{info.Directory.FullName}\{Path.GetFileNameWithoutExtension(info.FullName)}.html"
                        HTMLDocument.Save(s)
                        If Not info.Directory.GetFiles("Printer.css").Any Then
                            [Assembly].GetExecutingAssembly().GetResourceFileText("InnerLibs.Printer.css").Replace("##Cols##", Me.ColsNomal).WriteToFile($"{info.Directory}\Printer.css", False, Encoding.Unicode)
                        End If
                    End If
                Else
                    Throw New ArgumentException($"FileOrDirectoryPath is not a valid Path: {FileOrDirectoryPath}")
                End If
            End If
            Return Me
        End Function

        Public Function Image(ByVal Path As String, Optional highDensity As Boolean = True) As Printer
            If Not Path.IsFilePath Then Throw New FileNotFoundException("Invalid Path")
            If Not File.Exists(Path) Then Throw New FileNotFoundException("Image file not found")
            Dim img = System.Drawing.Image.FromFile(Path)
            Image(img, highDensity)
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal stream As Stream, ByVal Optional HighDensity As Boolean = True) As Printer
            Dim img = System.Drawing.Image.FromStream(stream)
            Image(img, HighDensity)
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal bytes As Byte(), ByVal Optional HighDensity As Boolean = True) As Printer
            Dim img As System.Drawing.Image
            Using ms = New MemoryStream(bytes)
                img = System.Drawing.Image.FromStream(ms)
            End Using
            Image(img, HighDensity)
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal pImage As System.Drawing.Image, ByVal Optional highDensity As Boolean = True) As Printer
            HTMLDocument.Root.Add(XElement.Parse($"<img class='image{highDensity.AsIf(" HighDensity")}'  src='{pImage.ToDataURL()}' />"))
            _ommit = True
            Return Write(Command.PrintImage(pImage, highDensity))
        End Function

    End Class





End Namespace