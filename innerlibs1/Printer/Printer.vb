' ***********************************************************************
' Assembly         :
' Author           : Leandro Ferreira
' Created          : 16-03-2019
'
' ***********************************************************************
' <copyright file="Printer.vb" company="VIP Soluções">
'		        		   The MIT License (MIT)
'	     		    Copyright (c) 2019 VIP Soluções
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
' <summary></summary>
' ***********************************************************************

Imports System.IO
Imports System.Linq.Expressions
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

        Public Shared Function CreatePrinter(Of CommandType As IPrintCommand)(PrinterName As String, Optional ColsNormal As Integer = 0, Optional ColsCondensed As Integer = 0, Optional ColsExpanded As Integer = 0, Optional Encoding As Encoding = Nothing) As Printer
            Return CreatePrinter(GetType(CommandType), PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Function

        Public Shared Function CreatePrinter(CommandType As Type, PrinterName As String, Optional ColsNormal As Integer = 0, Optional ColsCondensed As Integer = 0, Optional ColsExpanded As Integer = 0, Optional Encoding As Encoding = Nothing) As Printer
            Return CreatePrinter(Activator.CreateInstance(CommandType), PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Function

        Public Shared Function CreatePrinter(CommandType As IPrintCommand, PrinterName As String, Optional ColsNormal As Integer = 0, Optional ColsCondensed As Integer = 0, Optional ColsExpanded As Integer = 0, Optional Encoding As Encoding = Nothing) As Printer
            Return New Printer(CommandType, PrinterName, ColsNormal, ColsCondensed, ColsExpanded, Encoding)
        End Function

        Public Property DocumentBuffer As Byte()

        Public Property PrinterName As String

        Public Property ColsNomal As Integer

        Public Property ColsCondensed As Integer

        Public Property ColsExpanded As Integer

        Public ReadOnly Property Command As IPrintCommand

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

        Public Function Write(ByVal value As String, Optional Test As Boolean = True) As Printer
            Return If(Test, WriteString(value, False), Me)
        End Function

        Public Function Write(ByVal value As Byte()) As Printer
            If value IsNot Nothing AndAlso value.Any Then
                Dim list = New List(Of Byte)
                If DocumentBuffer IsNot Nothing Then list.AddRange(DocumentBuffer)
                list.AddRange(value)
                DocumentBuffer = list.ToArray
            End If
            Return Me
        End Function

        Public Function WriteLine(ByVal value As String, Optional Test As Boolean = True) As Printer
            Return If(Test, WriteString(value, True), Me)
        End Function

        Private Function WriteString(ByVal value As String, ByVal useLf As Boolean) As Printer
            If value.IsNotBlank Then
                If useLf Then value += vbLf
                Dim list = New List(Of Byte)
                If DocumentBuffer IsNot Nothing Then list.AddRange(DocumentBuffer)
                Dim bytes = _Command.Encoding.GetBytes(value)
                list.AddRange(bytes)
                DocumentBuffer = list.ToArray
                Debug.Write(value)
            End If
            Return Me
        End Function

        Public Function NewLine(Optional lines As Integer = 1) As Printer
            While (lines > 0)
                Write(vbLf)
                lines = lines - 1
            End While
            Return Me
        End Function

        Public Function Clear() As Printer
            DocumentBuffer = Nothing
            Return Me
        End Function

        Public Function Separator(Optional Character As Char = "-") As Printer
            Return Write(Command.Separator(Character))
        End Function

        Public Function AutoTest() As Printer
            Return Write(Command.AutoTest())
        End Function

        Public Function WriteTest() As Printer
            AlignLeft()
            WriteLine("INNERLIBS TEST PRINTER - 48 COLUMNS")
            WriteLine("....+....1....+....2....+....3....+....4....+...")
            Separator()
            WriteLine("Default Text")
            ItalicMode("Italic Text")
            BoldMode("Bold Text")
            UnderlineMode("UnderLine Text")
            ExpandedMode(True)
            WriteLine("Expanded Text")
            WriteLine("....+....1....+....2....")
            ExpandedMode(False)
            CondensedMode(True)
            WriteLine("Condensed Text")
            CondensedMode(False)
            Separator()
            DoubleWidth2()
            WriteLine("Font Size 2")
            DoubleWidth3()
            WriteLine("Fonte Size 3")
            NormalWidth()
            WriteLine("Normal Font Size")
            Separator()
            AlignRight()
            WriteLine("Text on Right")
            AlignCenter()
            WriteLine("Text on Center")
            AlignLeft()
            WriteLine("Text on Left")
            NewLine(3)
            WriteLine("EOF :)")
            Separator()
            NewLine()
            PartialPaperCut()
            Return Me
        End Function

        Public Function ItalicMode(ByVal value As String) As Printer
            Return Write(Command.Italic(value))
        End Function

        Public Function ItalicMode(ByVal state As Boolean) As Printer
            Return Write(Command.Italic(state))
        End Function

        Public Function BoldMode(ByVal value As String) As Printer
            Return Write(Command.Bold(value))
        End Function

        Public Function BoldMode(ByVal state As Boolean) As Printer
            Return Write(Command.Bold(state))
        End Function

        Public Function UnderlineMode(ByVal value As String) As Printer
            Return Write(Command.Underline(value))
        End Function

        Public Function UnderlineMode(ByVal state As Boolean) As Printer
            Return Write(Command.Underline(state))
        End Function

        Public Function ExpandedMode(ByVal value As String) As Printer
            Return Write(Command.Expanded(value))
        End Function

        Public Function ExpandedMode(ByVal state As Boolean) As Printer
            Return Write(Command.Expanded(state))
        End Function

        Public Function CondensedMode(ByVal value As String) As Printer
            Return Write(Command.Condensed(value))
        End Function

        Public Function CondensedMode(ByVal state As Boolean) As Printer
            Return Write(Command.Condensed(state))
        End Function

        Public Function NormalWidth() As Printer
            Return Write(Command.NormalWidth())
        End Function

        Public Function DoubleWidth2() As Printer
            Return Write(Command.DoubleWidth2())
        End Function

        Public Function DoubleWidth3() As Printer
            Return Write(Command.DoubleWidth3())
        End Function

        Public Function AlignLeft() As Printer
            Return Write(Command.Left())
        End Function

        Public Function AlignRight() As Printer
            Return Write(Command.Right())
        End Function

        Public Function AlignCenter() As Printer
            Return Write(Command.Center())
        End Function

        Public Function FullPaperCut() As Printer
            Debug.WriteLine($"✂{New String("-", Me.ColsNomal - 1)}")
            Return Write(Command.FullCut())
        End Function

        Public Function PartialPaperCut() As Printer
            Debug.WriteLine($"✂{New String("-", Me.ColsNomal - (Me.ColsNomal / 3).RoundInt())}//")
            Return Write(Command.PartialCut())
        End Function

        Public Function OpenDrawer() As Printer
            Return Write(Command.OpenDrawer())
        End Function

        Public Function QrCode(ByVal qrData As String) As Printer
            Return Write(Command.PrintQrData(qrData))
        End Function

        Public Function QrCode(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Printer
            Return Write(Command.PrintQrData(qrData, qrCodeSize))
        End Function

        Public Function Code128(ByVal code As String) As Printer
            Return Write(Command.Code128(code))
        End Function

        Public Function Code39(ByVal code As String) As Printer
            Return Write(Command.Code39(code))
        End Function

        Public Function Ean13(ByVal code As String) As Printer
            Return Write(Command.Ean13(code))
        End Function

        Public Function InitializePrint() As Printer
            RawPrinterHelper.SendBytesToPrinter(PrinterName, Command.Initialize())
            Return Me
        End Function

        Public Function WriteDictionary(Of T1, T2)(dics As IEnumerable(Of IDictionary(Of T1, T2)), Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteDictionary(PartialCutOnEach, If(dics, {}).ToArray())
        End Function

        Public Function WritePair(Key As Object, Value As Object) As Printer
            AlignLeft()
            Write($"{Key}")
            AlignRight()
            Write($"{Value}")
            AlignLeft()
            Return Me
        End Function

        Public Function WriteList(Items As IEnumerable(Of Object), Optional ListOrdenator As String = Nothing) As Printer
            For index = 0 To Items.Count - 1
                WriteLine($"{ListOrdenator.IfBlank($"{index + 1}) ")}{Items}")
            Next
            Return Me
        End Function

        Public Function WriteList(ParamArray Items As Object()) As Printer
            Return WriteList(If(Items, {}).AsEnumerable)
        End Function

        Public Function WritePairLine(Key As Object, Value As Object) As Printer
            Return WritePair(Key, Value).NewLine()
        End Function

        Public Function WritePriceLine(Description As String, Price As Decimal) As Printer
            Dim sprice = Price.RoundDecimal(2).ToString()
            Dim dots = New String("."c, (ColsNomal - (Description.Length + sprice.Length)).LimitRange(0, ColsNomal))
            Dim s = $"{Description}{dots}{sprice}"
            WriteLine(s)
            Return Me
        End Function

        Public Function WritePriceList(List As IEnumerable(Of Tuple(Of String, Decimal))) As Printer
            For Each item In List.NullAsEmpty()
                WritePriceLine(item.Item1, item.Item2)
            Next
            Return Me
        End Function

        Public Function WritePriceList(Of T)(List As IEnumerable(Of T), Description As Expression(Of Func(Of T, String)), Price As Expression(Of Func(Of T, Decimal))) As Printer
            Return WritePriceList(List.Select(Function(x) New Tuple(Of String, Decimal)(Description.Compile()(x), Price.Compile()(x))))
        End Function

        Public Function WriteDictionary(Of T1, T2)(ParamArray dics As IDictionary(Of T1, T2)()) As Printer
            Return WriteDictionary(False, dics)
        End Function

        Public Function WriteDictionary(Of T1, T2)(PartialCutOnEach As Boolean, ParamArray dics As IDictionary(Of T1, T2)()) As Printer
            dics = If(dics, {})
            For Each dic In dics
                If dic IsNot Nothing Then
                    If PartialCutOnEach Then PartialPaperCut() Else Separator()
                    For Each item In dic
                        WritePairLine(item.Key?.ToString(), item.Value?.ToString())
                    Next
                    AlignLeft()
                End If
            Next
            If dics.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            Return Me
        End Function

        Public Function WriteClass(Of T As Class)(ParamArray objs As T()) As Printer
            Return WriteClass(False, objs)
        End Function

        Public Function WriteClass(Of T As Class)(PartialCutOnEach As Boolean, ParamArray objs As T()) As Printer
            objs = If(objs, {})
            For Each obj In objs
                If obj IsNot Nothing Then
                    If PartialCutOnEach Then PartialPaperCut() Else Separator()
                    For Each item In obj.GetNullableTypeOf().GetProperties()
                        If item.CanRead Then
                            WritePairLine(item.Name, item.GetValue(obj)?.ToString())
                        End If
                    Next
                    AlignLeft()
                End If
            Next
            If objs.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            Return Me
        End Function

        Public Function WriteClass(Of T As Class)(ByVal obj As IEnumerable(Of T), Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteClass(PartialCutOnEach, If(obj, {}).ToArray())
        End Function

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

        Public Function WriteTemplate(Of T)(TemplateString As String, ParamArray obj As T()) As Printer
            Return WriteTemplate(TemplateString, False, obj)
        End Function

        Public Function WriteTemplate(Of T)(TemplateString As String, obj As IEnumerable(Of T), Optional PartiaCutOnEach As Boolean = False) As Printer
            Return WriteTemplate(TemplateString, PartiaCutOnEach, If(obj, {}).ToArray())
        End Function

        Public Function WriteDate([DateAndTime] As Date, Optional Format As String = Nothing)
            If Format.IsNotBlank Then
                Return Write(DateAndTime.ToString(Format))
            Else
                Return Write(DateAndTime.ToString())
            End If
        End Function

        Public Function WriteDate(Optional Format As String = Nothing)
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
        ''' Imprime os Bytes
        ''' </summary>
        ''' <param name="Copies"></param>
        ''' <returns></returns>
        Public Function PrintDocument(Bytes As Byte(), Optional Copies As Integer = 1) As Printer
            If Bytes IsNot Nothing AndAlso Bytes.Any Then
                For i = 0 To Copies.SetMinValue(1) - 1
                    If PrinterName.IsFilePath Then
                        SaveFile(PrinterName)
                    Else
                        If Not RawPrinterHelper.SendBytesToPrinter(PrinterName, DocumentBuffer.ToArray()) Then Throw New ArgumentException("Não foi possível acessar a impressora: " & PrinterName)
                    End If
                Next
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Escreve um Arquivo com os dados binarios desta impressao
        ''' </summary>
        ''' <param name="FileOrDirectoryPath"></param>
        ''' <returns></returns>
        Public Function SaveFile(FileOrDirectoryPath As String) As Printer
            If DocumentBuffer IsNot Nothing AndAlso DocumentBuffer.Count > 0 Then
                If FileOrDirectoryPath.IsDirectoryPath Then
                    FileOrDirectoryPath = $"{FileOrDirectoryPath}\{GetType(CommandType).Name}\{Me.PrinterName.ToFriendlyPathName()}\{DateTime.Now.Ticks}.{Me.Command?.GetTypeOf()?.Name.IfBlank("bin")}"
                    FileOrDirectoryPath = FileOrDirectoryPath.AdjustPathChars(True)
                End If
                If FileOrDirectoryPath.IsFilePath Then
                    DocumentBuffer.ToArray().WriteToFile(FileOrDirectoryPath, DateTime.Now)
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
            Write(Command.PrintImage(img, highDensity))
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal stream As Stream, ByVal Optional HighDensity As Boolean = True) As Printer
            Dim img = System.Drawing.Image.FromStream(stream)
            Write(Command.PrintImage(img, HighDensity))
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal bytes As Byte(), ByVal Optional HighDensity As Boolean = True) As Printer
            Dim img As System.Drawing.Image
            Using ms = New MemoryStream(bytes)
                img = System.Drawing.Image.FromStream(ms)
            End Using
            Write(Command.PrintImage(img, HighDensity))
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal pImage As System.Drawing.Image, ByVal Optional highDensity As Boolean = True) As Printer
            Return Write(Command.PrintImage(pImage, highDensity))
        End Function

    End Class

End Namespace