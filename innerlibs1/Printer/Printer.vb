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
Imports System.Runtime.CompilerServices
Imports System.Text
Imports InnerLibs.Printer.Command

Namespace Printer



    Public Class Printer(Of CommandType As IPrintCommand)





        Public Property DocumentBuffer As Byte()
        Public Property PrinterName As String

        Public ReadOnly Property Command As CommandType

        Public ReadOnly Property PrinterCommandType As String
            Get
                Return Command.GetNullableTypeOf().Name
            End Get
        End Property

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="colsNormal">Number of columns for normal mode print</param>
        ''' <param name="colsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="colsExpanded">Number of columns for expanded mode print</param>
        ''' <param name="encoding">Custom encoding</param>
        Public Sub New(ByVal printerName As String, ByVal colsNormal As Integer, ByVal colsCondensed As Integer, ByVal colsExpanded As Integer, ByVal encoding As Encoding)
            Me.New(Nothing, printerName, colsNormal, colsCondensed, colsExpanded, encoding)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="colsNormal">Number of columns for normal mode print</param>
        ''' <param name="colsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="colsExpanded">Number of columns for expanded mode print</param>
        ''' <param name="encoding">Custom encoding</param>
        Friend Sub New(Command As CommandType, ByVal printerName As String, ByVal colsNormal As Integer, ByVal colsCondensed As Integer, ByVal colsExpanded As Integer, ByVal encoding As Encoding)
            Me.Command = If(Command, Activator.CreateInstance(GetType(CommandType)))
            If encoding IsNot Nothing Then
                Me.Command.Encoding = encoding
            End If
            Me.PrinterName = printerName.IfBlank("temp.prn").Trim()
            Me.ColsNomal = If(colsNormal <= 0, Command.ColsNomal, colsNormal)
            Me.ColsCondensed = If(colsCondensed <= 0, Command.ColsCondensed, colsCondensed)
            Me.ColsExpanded = If(colsExpanded <= 0, Command.ColsExpanded, colsExpanded)
        End Sub

        ''' <summary>
        '''Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="colsNormal">Number of columns for normal mode print</param>
        ''' <param name="colsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="colsExpanded">Number of columns for expanded mode print</param>
        Public Sub New(ByVal printerName As String, ByVal colsNormal As Integer, ByVal colsCondensed As Integer, ByVal colsExpanded As Integer)
            Me.New(printerName, colsNormal, colsCondensed, colsExpanded, Nothing)
        End Sub

        ''' <summary>
        '''Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="encoding">Custom encoding</param>
        Public Sub New(ByVal printerName As String, ByVal encoding As Encoding)
            Me.New(printerName, 0, 0, 0, encoding)
        End Sub

        ''' <summary>
        '''Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        Public Sub New(ByVal printerName As String)
            Me.New(printerName, 0, 0, 0, Nothing)
        End Sub

        Public Property ColsNomal As UInteger

        Public Property ColsCondensed As UInteger

        Public Property ColsExpanded As UInteger

        Public Function Write(ByVal value As String) As Printer(Of CommandType)
            Return WriteString(value, False)
        End Function

        Public Function Write(ByVal value As Byte()) As Printer(Of CommandType)
            If value IsNot Nothing AndAlso value.Any Then
                Dim list = New List(Of Byte)
                If DocumentBuffer IsNot Nothing Then list.AddRange(DocumentBuffer)
                list.AddRange(value)
                DocumentBuffer = list.ToArray
            End If
            Return Me
        End Function

        Public Function WriteLine(ByVal value As String) As Printer(Of CommandType)
            Return WriteString(value, True)
        End Function

        Private Function WriteString(ByVal value As String, ByVal useLf As Boolean) As Printer(Of CommandType)
            If value.IsNotBlank Then
                If useLf Then value += vbLf
                Dim list = New List(Of Byte)
                If DocumentBuffer IsNot Nothing Then list.AddRange(DocumentBuffer)
                Dim bytes = _Command.Encoding.GetBytes(value)
                list.AddRange(bytes)
                DocumentBuffer = list.ToArray
            End If
            Return Me
        End Function

        Public Function NewLine(Optional lines As Integer = 1) As Printer(Of CommandType)
            If lines > 0 Then
                For i = 1 To lines - 1
                    Write(vbLf)
                Next
            End If
            Return Me
        End Function

        Public Function Clear() As Printer(Of CommandType)
            DocumentBuffer = Nothing
            Return Me
        End Function

        Public Function Separator() As Printer(Of CommandType)
            Return Write(Command.Separator())
        End Function

        Public Function AutoTest() As Printer(Of CommandType)
            Return Write(Command.AutoTest())
        End Function

        Public Function TestPrinter() As Printer(Of CommandType)
            AlignLeft()
            WriteLine("INNERLIBS TEST PRINTER - 48 COLUMNS")
            WriteLine("....+....1....+....2....+....3....+....4....+...")
            Separator()
            WriteLine("Default Text")
            ItalicMode("Italic Text")
            BoldMode("Bold Text")
            UnderlineMode("UnderLine Text")
            ExpandedMode(PrinterModeState.[On])
            WriteLine("Expanded Text")
            WriteLine("....+....1....+....2....")
            ExpandedMode(PrinterModeState.Off)
            CondensedMode(PrinterModeState.[On])
            WriteLine("Condensed Text")
            CondensedMode(PrinterModeState.Off)
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
            WriteLine("End of Test :)")
            Separator()
            NewLine()
            PartialPaperCut()
            Return Me
        End Function

        Public Function ItalicMode(ByVal value As String) As Printer(Of CommandType)
            Return Write(Command.FontMode.Italic(value))
        End Function

        Public Function ItalicMode(ByVal state As PrinterModeState) As Printer(Of CommandType)
            Return Write(Command.FontMode.Italic(state))
        End Function

        Public Function BoldMode(ByVal value As String) As Printer(Of CommandType)
            Return Write(Command.FontMode.Bold(value))
        End Function

        Public Function BoldMode(ByVal state As PrinterModeState) As Printer(Of CommandType)
            Return Write(Command.FontMode.Bold(state))
        End Function

        Public Function UnderlineMode(ByVal value As String) As Printer(Of CommandType)
            Return Write(Command.FontMode.Underline(value))
        End Function

        Public Function UnderlineMode(ByVal state As PrinterModeState) As Printer(Of CommandType)
            Return Write(Command.FontMode.Underline(state))
        End Function

        Public Function ExpandedMode(ByVal value As String) As Printer(Of CommandType)
            Return Write(Command.FontMode.Expanded(value))
        End Function

        Public Function ExpandedMode(ByVal state As PrinterModeState) As Printer(Of CommandType)
            Return Write(Command.FontMode.Expanded(state))
        End Function

        Public Function CondensedMode(ByVal value As String) As Printer(Of CommandType)
            Return Write(Command.FontMode.Condensed(value))
        End Function

        Public Function CondensedMode(ByVal state As PrinterModeState) As Printer(Of CommandType)
            Return Write(Command.FontMode.Condensed(state))
        End Function

        Public Function NormalWidth() As Printer(Of CommandType)
            Return Write(Command.FontWidth.Normal())
        End Function

        Public Function DoubleWidth2() As Printer(Of CommandType)
            Return Write(Command.FontWidth.DoubleWidth2())
        End Function

        Public Function DoubleWidth3() As Printer(Of CommandType)
            Return Write(Command.FontWidth.DoubleWidth3())
        End Function

        Public Function AlignLeft() As Printer(Of CommandType)
            Return Write(Command.Alignment.Left())
        End Function

        Public Function AlignRight() As Printer(Of CommandType)
            Return Write(Command.Alignment.Right())
        End Function

        Public Function AlignCenter() As Printer(Of CommandType)
            Return Write(Command.Alignment.Center())
        End Function

        Public Function FullPaperCut() As Printer(Of CommandType)
            Return Write(Command.PaperCut.Full())
        End Function

        Public Function FullPaperCut(ByVal predicate As Boolean) As Printer(Of CommandType)
            If predicate Then FullPaperCut()
            Return Me
        End Function

        Public Function PartialPaperCut() As Printer(Of CommandType)
            Return Write(Command.PaperCut.[Partial]())
        End Function

        Public Function PartialPaperCut(ByVal predicate As Boolean) As Printer(Of CommandType)
            If predicate Then PartialPaperCut()
            Return Me
        End Function

        Public Function OpenDrawer() As Printer(Of CommandType)
            Return Write(Command.Drawer.Open())
        End Function

        Public Function QrCode(ByVal qrData As String) As Printer(Of CommandType)
            Return Write(Command.QrCode.Print(qrData))
        End Function

        Public Function QrCode(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Printer(Of CommandType)
            Return Write(Command.QrCode.Print(qrData, qrCodeSize))
        End Function

        Public Function Code128(ByVal code As String) As Printer(Of CommandType)
            Return Write(Command.BarCode.Code128(code))
        End Function

        Public Function Code39(ByVal code As String) As Printer(Of CommandType)
            Return Write(Command.BarCode.Code39(code))
        End Function

        Public Function Ean13(ByVal code As String) As Printer(Of CommandType)
            Return Write(Command.BarCode.Ean13(code))
        End Function

        Public Function InitializePrint() As Printer(Of CommandType)
            RawPrinterHelper.SendBytesToPrinter(PrinterName, Command.InitializePrint.Initialize())
            Return Me
        End Function

        Public Function WriteDictionary(Of T1, T2)(dics As IEnumerable(Of IDictionary(Of T1, T2)), Optional PartialCutOnEach As Boolean = False) As Printer(Of CommandType)
            Return WriteDictionary(PartialCutOnEach, If(dics, {}).ToArray())
        End Function

        Public Function WriteDictionary(Of T1, T2)(ParamArray dics As IDictionary(Of T1, T2)()) As Printer(Of CommandType)
            Return WriteDictionary(False, dics)
        End Function

        Public Function WriteDictionary(Of T1, T2)(PartialCutOnEach As Boolean, ParamArray dics As IDictionary(Of T1, T2)()) As Printer(Of CommandType)
            dics = If(dics, {})
            For Each dic In dics
                If dic IsNot Nothing Then
                    If PartialCutOnEach Then PartialPaperCut() Else Separator()
                    For Each item In dic
                        AlignLeft()
                        Write(item.Key?.ToString().IfBlank("-"))
                        AlignRight()
                        Write(item.Value?.ToString().IfBlank("-"))
                        NewLine()
                    Next
                    AlignLeft()
                End If
            Next
            If dics.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            Return Me
        End Function

        Public Function WriteClass(Of T As Class)(ParamArray objs As T()) As Printer(Of CommandType)
            Return WriteClass(False, objs)
        End Function

        Public Function WriteClass(Of T As Class)(PartialCutOnEach As Boolean, ParamArray objs As T()) As Printer(Of CommandType)
            objs = If(objs, {})
            For Each obj In objs
                If obj IsNot Nothing Then
                    If PartialCutOnEach Then PartialPaperCut() Else Separator()
                    For Each item In obj.GetNullableTypeOf().GetProperties()
                        If item.CanRead Then
                            AlignLeft()
                            Write(item.Name)
                            AlignRight()
                            Write(If(item.GetValue(obj)?.ToString(), "-"))
                            NewLine()
                        End If
                    Next
                    AlignLeft()
                End If
            Next
            If objs.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            Return Me
        End Function

        Public Function WriteClass(Of T As Class)(ByVal obj As IEnumerable(Of T), Optional PartialCutOnEach As Boolean = False) As Printer(Of CommandType)
            Return WriteClass(PartialCutOnEach, If(obj, {}).ToArray())
        End Function

        Public Function WriteTemplate(Of T)(TemplateString As String, PartialCutOnEach As Boolean, ParamArray obj As T()) As Printer(Of CommandType)
            If TemplateString.IsNotBlank Then
                obj = If(obj, {})
                If TemplateString.IsFilePath Then
                    If File.Exists(TemplateString) Then
                        TemplateString = File.ReadAllText(TemplateString)
                    End If
                End If
                For Each item In obj
                    For Each linha In TemplateString.Inject(obj).SplitAny(BreakLineChars.ToArray())
                        WriteLine(linha)
                    Next
                Next
                If obj.Any() Then If PartialCutOnEach Then PartialPaperCut() Else Separator()
            End If

            Return Me
        End Function

        Public Function WriteTemplate(Of T)(TemplateString As String, ParamArray obj As T()) As Printer(Of CommandType)
            Return WriteTemplate(TemplateString, False, obj)
        End Function

        Public Function WriteTemplate(Of T)(TemplateString As String, obj As IEnumerable(Of T), Optional PartiaCutOnEach As Boolean = False) As Printer(Of CommandType)
            Return WriteTemplate(TemplateString, PartiaCutOnEach, If(obj, {}).ToArray())
        End Function

        ''' <summary>
        ''' Imprime o conteudo do <see cref="DocumentBuffer"/> atual
        ''' </summary>
        ''' <param name="Copies"></param>
        ''' <returns></returns>
        Public Function PrintDocument(Optional Copies As Integer = 1) As Printer(Of CommandType)
            Return PrintDocument(DocumentBuffer, Copies)
        End Function

        ''' <summary>
        ''' Imprime o conteudo de um arquivo ou o conteudo de todos os arquivos de um diretorio
        ''' </summary>
        ''' <param name="FileOrDirectoryPath"></param>
        ''' <param name="Copies"></param>
        ''' <returns></returns>
        Public Function PrintDocument(FileOrDirectoryPath As String, Optional Copies As Integer = 1) As Printer(Of CommandType)

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
                Throw New ArgumentException($"FileOrDirectoryPath is not a valid Path: {FileOrDirectoryPath}")

            End If
            Return Me
        End Function

        ''' <summary>
        ''' Imprime os Bytes
        ''' </summary>
        ''' <param name="Copies"></param>
        ''' <returns></returns>
        Public Function PrintDocument(Bytes As Byte(), Optional Copies As Integer = 1) As Printer(Of CommandType)
            If DocumentBuffer IsNot Nothing AndAlso DocumentBuffer.Count > 0 Then
                For i = 0 To Copies.SetMinValue(1) - 1
                    If Not RawPrinterHelper.SendBytesToPrinter(PrinterName, DocumentBuffer.ToArray()) Then Throw New ArgumentException("Não foi possível acessar a impressora: " & PrinterName)
                Next
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Escreve um Arquivo com os dados binarios desta impressao
        ''' </summary>
        ''' <param name="FileOrDirectoryPath"></param>
        ''' <returns></returns>
        Public Function WriteFile(FileOrDirectoryPath As String) As Printer(Of CommandType)
            If DocumentBuffer IsNot Nothing AndAlso DocumentBuffer.Count > 0 Then
                If FileOrDirectoryPath.IsDirectoryPath Then
                    FileOrDirectoryPath = $"{FileOrDirectoryPath}\{GetType(CommandType).Name}\{Me.PrinterName.ToFriendlyPathName()}\{DateTime.Now.Ticks}.{Me.Command?.GetTypeOf()?.Name.IfBlank("bin")}"
                    FileOrDirectoryPath = FileOrDirectoryPath.AdjustPathChars(True)
                End If

                If FileOrDirectoryPath.IsFilePath Then
                    DocumentBuffer.ToArray().WriteToFile(FileOrDirectoryPath)
                Else
                    Throw New ArgumentException($"FileOrDirectoryPath is not a valid Path: {FileOrDirectoryPath}")
                End If
            End If
            Return Me
        End Function

        Public Function Image(ByVal Path As String, Optional highDensity As Boolean = True) As Printer(Of CommandType)
            If Not Path.IsFilePath Then Throw New FileNotFoundException("Invalid Path")
            If Not File.Exists(Path) Then Throw New FileNotFoundException("Image file not found")
            Dim img = System.Drawing.Image.FromFile(Path)
            Write(Command.Image.Print(img, highDensity))
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal stream As Stream, ByVal Optional HighDensity As Boolean = True) As Printer(Of CommandType)
            Dim img = System.Drawing.Image.FromStream(stream)
            Write(Command.Image.Print(img, HighDensity))
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal bytes As Byte(), ByVal Optional HighDensity As Boolean = True) As Printer(Of CommandType)
            Dim img As System.Drawing.Image
            Using ms = New MemoryStream(bytes)
                img = System.Drawing.Image.FromStream(ms)
            End Using
            Write(Command.Image.Print(img, HighDensity))
            img.Dispose()
            Return Me
        End Function

        Public Function Image(ByVal pImage As System.Drawing.Image, ByVal Optional highDensity As Boolean = True) As Printer(Of CommandType)
            Return Write(Command.Image.Print(pImage, highDensity))
        End Function

    End Class

    Public NotInheritable Class BematechPrinter
        Inherits Printer(Of EscBemaCommands.EscBema)




        Public Sub New(printerName As String)
            MyBase.New(printerName)
        End Sub

        Public Sub New(printerName As String, encoding As Encoding)
            MyBase.New(printerName, encoding)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer, encoding As Encoding)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded, encoding)
        End Sub

    End Class

    Public NotInheritable Class EpsonPrinter
        Inherits Printer(Of EscPosCommands.EscPos)

        Public Sub New(printerName As String)
            MyBase.New(printerName)
        End Sub

        Public Sub New(printerName As String, encoding As Encoding)
            MyBase.New(printerName, encoding)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer, encoding As Encoding)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded, encoding)
        End Sub

    End Class

    Public NotInheritable Class DarumaPrinter
        Inherits Printer(Of EscDarumaCommands.EscDaruma)

        Public Sub New(printerName As String)
            MyBase.New(printerName)
        End Sub

        Public Sub New(printerName As String, encoding As Encoding)
            MyBase.New(printerName, encoding)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer, encoding As Encoding)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded, encoding)
        End Sub

    End Class

    Public NotInheritable Class Printer
        Inherits Printer(Of EscPosCommands.EscPos)

        Public Sub New(printerName As String)
            MyBase.New(printerName)
        End Sub

        Public Sub New(printerName As String, encoding As Encoding)
            MyBase.New(printerName, encoding)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded)
        End Sub

        Public Sub New(printerName As String, colsNormal As Integer, colsCondensed As Integer, colsExpanded As Integer, encoding As Encoding)
            MyBase.New(printerName, colsNormal, colsCondensed, colsExpanded, encoding)
        End Sub

    End Class



    Public Module PrinterExtensions
        <Extension()> Public Function CreatePrinter(Of T As IPrintCommand)(Command As T, printerName As String, Optional colsNormal As Integer = 0, Optional colsCondensed As Integer = 0, Optional colsExpanded As Integer = 0, Optional encoding As Encoding = Nothing) As Printer(Of T)
            Return New Printer(Of T)(Command, printerName, colsNormal, colsCondensed, colsExpanded, encoding)
        End Function
    End Module

End Namespace