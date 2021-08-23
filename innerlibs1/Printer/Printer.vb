' ***********************************************************************
' Assembly         :
' Author           : Leandro Ferreira
' Created          : 16-03-2019
'
' ***********************************************************************
' <copyright file="Printer.cs" company="VIP Soluções">
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
Imports System.Text
Imports InnerLibs.EscBemaCommands
Imports InnerLibs.EscDarumaCommands
Imports InnerLibs.EscPosCommands
Imports InnerLibs.Printer.Command

Namespace Printer

    Public Class Printer
        Implements IPrinter(Of Printer)

        Private _ColsNomal As Integer, _ColsCondensed As Integer, _ColsExpanded As Integer
        Private _buffer As Byte()
        Private ReadOnly _printerName As String
        Private ReadOnly _command As IPrintCommand
        Private ReadOnly _printerType As PrinterType
        Private ReadOnly _encoding As Encoding

        ''' <summary>
        ''' Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="type">Command set of type printer</param>
        ''' <param name="colsNormal">Number of columns for normal mode print</param>
        ''' <param name="colsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="colsExpanded">Number of columns for expanded mode print</param>
        ''' <param name="encoding">Custom encoding</param>
        Public Sub New(ByVal printerName As String, ByVal type As PrinterType, ByVal colsNormal As Integer, ByVal colsCondensed As Integer, ByVal colsExpanded As Integer, ByVal encoding As Encoding)
            _printerName = printerName.IfBlank("temp.prn").Trim()
            _printerType = type
            _encoding = encoding

#Region "Select printer type"

            Select Case type
                Case PrinterType.Epson
                    _command = New EscPos()
                Case PrinterType.Bematech
                    _command = New EscBema()
                Case PrinterType.Daruma
                    _command = New EscDaruma()
            End Select

#End Region

#Region "Configure number columns"

            ColsNomal = If(colsNormal = 0, _command.ColsNomal, colsNormal)
            Me.ColsCondensed = If(colsCondensed = 0, _command.ColsCondensed, colsCondensed)
            Me.ColsExpanded = If(colsExpanded = 0, _command.ColsExpanded, colsExpanded)

#End Region

        End Sub

        ''' <summary>
        '''     Initializes a new instance of the <see cref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="type">Command set of type printer</param>
        ''' <param name="colsNormal">Number of columns for normal mode print</param>
        ''' <param name="colsCondensed">Number of columns for condensed mode print</param>
        ''' <param name="colsExpanded">Number of columns for expanded mode print</param>
        Public Sub New(ByVal printerName As String, ByVal type As PrinterType, ByVal colsNormal As Integer, ByVal colsCondensed As Integer, ByVal colsExpanded As Integer)
            Me.New(printerName, type, colsNormal, colsCondensed, colsExpanded, Nothing)
        End Sub

        ''' <summary>
        '''     Initializes a new instance of the <seecref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="type">Command set of type printer</param>
        ''' <param name="encoding">Custom encoding</param>
        Public Sub New(ByVal printerName As String, ByVal type As PrinterType, ByVal encoding As Encoding)
            Me.New(printerName, type, 0, 0, 0, encoding)
        End Sub

        ''' <summary>
        '''     Initializes a new instance of the <seecref="Printer"/> class.
        ''' </summary>
        ''' <param name="printerName">Printer name, shared name or port of printer install</param>
        ''' <param name="type">>Command set of type printer</param>
        Public Sub New(ByVal printerName As String, ByVal type As PrinterType)
            Me.New(printerName, type, 0, 0, 0, Nothing)
        End Sub

        Public Property ColsNomal As Integer Implements IPrinter(Of Printer).ColsNomal
            Get
                Return _ColsNomal
            End Get
            Private Set(ByVal value As Integer)
                _ColsNomal = value
            End Set
        End Property

        Public Property ColsCondensed As Integer Implements IPrinter(Of Printer).ColsCondensed
            Get
                Return _ColsCondensed
            End Get
            Private Set(ByVal value As Integer)
                _ColsCondensed = value
            End Set
        End Property

        Public Property ColsExpanded As Integer Implements IPrinter(Of Printer).ColsExpanded
            Get
                Return _ColsExpanded
            End Get
            Private Set(ByVal value As Integer)
                _ColsExpanded = value
            End Set
        End Property

        Public Function Write(ByVal value As String) As Printer Implements IPrinter(Of Printer).Write
            Return WriteString(value, False)
        End Function

        Public Function Write(ByVal value As Byte()) As Printer Implements IPrinter(Of Printer).Write
            If value IsNot Nothing Then
                Dim list = New List(Of Byte)()
                If _buffer IsNot Nothing Then list.AddRange(_buffer)
                list.AddRange(value)
                _buffer = list.ToArray()
            End If

            Return Me
        End Function

        Public Function WriteLine(ByVal value As String) As Printer Implements IPrinter(Of Printer).WriteLine
            Return WriteString(value, True)
        End Function

        Private Function WriteString(ByVal value As String, ByVal useLf As Boolean) As Printer
            If String.IsNullOrEmpty(value) Then Return Me
            If useLf Then value += vbLf
            Dim list = New List(Of Byte)()
            If _buffer IsNot Nothing Then list.AddRange(_buffer)
            Dim bytes = If(_encoding IsNot Nothing, _encoding.GetBytes(value), If(_printerType = PrinterType.Bematech, Encoding.GetEncoding(850).GetBytes(value), Encoding.GetEncoding("IBM860").GetBytes(value)))
            list.AddRange(bytes)
            _buffer = list.ToArray()
            Return Me
        End Function

        Public Function NewLine() As Printer Implements IPrinter(Of Printer).NewLine
            Return Write(vbLf)
        End Function

        Public Function NewLines(ByVal lines As Integer) As Printer Implements IPrinter(Of Printer).NewLines
            For i = 1 To lines - 1
                NewLine()
            Next
            Return Me
        End Function

        Public Function Clear() As Printer Implements IPrinter(Of Printer).Clear
            _buffer = Nothing
            Return Me
        End Function

        Public Function Separator() As Printer Implements IPrinter(Of Printer).Separator
            Return Write(_command.Separator())
        End Function

        Public Function AutoTest() As Printer Implements IPrinter(Of Printer).AutoTest
            Return Write(_command.AutoTest())
        End Function

        Public Function TestPrinter() As Printer Implements IPrinter(Of Printer).TestPrinter
            AlignLeft()
            WriteLine("TESTE DE IMPRESSÃO NORMAL - 48 COLUNAS")
            WriteLine("....+....1....+....2....+....3....+....4....+...")
            Separator()
            WriteLine("Texto Normal")
            ItalicMode("Texto Itálico")
            BoldMode("Texto Negrito")
            UnderlineMode("Texto Sublinhado")
            ExpandedMode(PrinterModeState.[On])
            WriteLine("Texto Expandido")
            WriteLine("....+....1....+....2....")
            ExpandedMode(PrinterModeState.Off)
            CondensedMode(PrinterModeState.[On])
            WriteLine("Texto condensado")
            CondensedMode(PrinterModeState.Off)
            Separator()
            DoubleWidth2()
            WriteLine("Largura Fonte 2")
            DoubleWidth3()
            WriteLine("Largura Fonte 3")
            NormalWidth()
            WriteLine("Largura normal")
            Separator()
            AlignRight()
            WriteLine("Texto alinhado à direita")
            AlignCenter()
            WriteLine("Texto alinhado ao centro")
            AlignLeft()
            WriteLine("Texto alinhado à esquerda")
            NewLines(3)
            WriteLine("Final de Teste :)")
            Separator()
            NewLine()
            PartialPaperCut()
            Return Me
        End Function

        Public Function ItalicMode(ByVal value As String) As Printer Implements IPrinter(Of Printer).ItalicMode
            Return Write(_command.FontMode.Italic(value))
        End Function

        Public Function ItalicMode(ByVal state As PrinterModeState) As Printer Implements IPrinter(Of Printer).ItalicMode
            Return Write(_command.FontMode.Italic(state))
        End Function

        Public Function BoldMode(ByVal value As String) As Printer Implements IPrinter(Of Printer).BoldMode
            Return Write(_command.FontMode.Bold(value))
        End Function

        Public Function BoldMode(ByVal state As PrinterModeState) As Printer Implements IPrinter(Of Printer).BoldMode
            Return Write(_command.FontMode.Bold(state))
        End Function

        Public Function UnderlineMode(ByVal value As String) As Printer Implements IPrinter(Of Printer).UnderlineMode
            Return Write(_command.FontMode.Underline(value))
        End Function

        Public Function UnderlineMode(ByVal state As PrinterModeState) As Printer Implements IPrinter(Of Printer).UnderlineMode
            Return Write(_command.FontMode.Underline(state))
        End Function

        Public Function ExpandedMode(ByVal value As String) As Printer Implements IPrinter(Of Printer).ExpandedMode
            Return Write(_command.FontMode.Expanded(value))
        End Function

        Public Function ExpandedMode(ByVal state As PrinterModeState) As Printer Implements IPrinter(Of Printer).ExpandedMode
            Return Write(_command.FontMode.Expanded(state))
        End Function

        Public Function CondensedMode(ByVal value As String) As Printer Implements IPrinter(Of Printer).CondensedMode
            Return Write(_command.FontMode.Condensed(value))
        End Function

        Public Function CondensedMode(ByVal state As PrinterModeState) As Printer Implements IPrinter(Of Printer).CondensedMode
            Return Write(_command.FontMode.Condensed(state))
        End Function

        Public Function NormalWidth() As Printer Implements IPrinter(Of Printer).NormalWidth
            Return Write(_command.FontWidth.Normal())
        End Function

        Public Function DoubleWidth2() As Printer Implements IPrinter(Of Printer).DoubleWidth2
            Return Write(_command.FontWidth.DoubleWidth2())
        End Function

        Public Function DoubleWidth3() As Printer Implements IPrinter(Of Printer).DoubleWidth3
            Return Write(_command.FontWidth.DoubleWidth3())
        End Function

        Public Function AlignLeft() As Printer Implements IPrinter(Of Printer).AlignLeft
            Return Write(_command.Alignment.Left())
        End Function

        Public Function AlignRight() As Printer Implements IPrinter(Of Printer).AlignRight
            Return Write(_command.Alignment.Right())
        End Function

        Public Function AlignCenter() As Printer Implements IPrinter(Of Printer).AlignCenter
            Return Write(_command.Alignment.Center())
        End Function

        Public Function FullPaperCut() As Printer Implements IPrinter(Of Printer).FullPaperCut
            Return Write(_command.PaperCut.Full())
        End Function

        Public Function FullPaperCut(ByVal predicate As Boolean) As Printer Implements IPrinter(Of Printer).FullPaperCut
            If predicate Then FullPaperCut()
            Return Me
        End Function

        Public Function PartialPaperCut() As Printer Implements IPrinter(Of Printer).PartialPaperCut
            Return Write(_command.PaperCut.[Partial]())
        End Function

        Public Function PartialPaperCut(ByVal predicate As Boolean) As Printer Implements IPrinter(Of Printer).PartialPaperCut
            If predicate Then PartialPaperCut()
            Return Me
        End Function

        Public Function OpenDrawer() As Printer Implements IPrinter(Of Printer).OpenDrawer
            Return Write(_command.Drawer.Open())
        End Function

        Public Function QrCode(ByVal qrData As String) As Printer Implements IPrinter(Of Printer).QrCode
            Return Write(_command.QrCode.Print(qrData))
        End Function

        Public Function QrCode(ByVal qrData As String, ByVal qrCodeSize As QrCodeSize) As Printer Implements IPrinter(Of Printer).QrCode
            Return Write(_command.QrCode.Print(qrData, qrCodeSize))
        End Function

        Public Function Code128(ByVal code As String) As Printer Implements IPrinter(Of Printer).Code128
            Return Write(_command.BarCode.Code128(code))
        End Function

        Public Function Code39(ByVal code As String) As Printer Implements IPrinter(Of Printer).Code39
            Return Write(_command.BarCode.Code39(code))
        End Function

        Public Function Ean13(ByVal code As String) As Printer Implements IPrinter(Of Printer).Ean13
            Return Write(_command.BarCode.Ean13(code))
        End Function

        Public Function InitializePrint() As Printer Implements IPrinter(Of Printer).InitializePrint
            RawPrinterHelper.SendBytesToPrinter(_printerName, _command.InitializePrint.Initialize())
            Return Me
        End Function

        Public Function WriteDictionary(Of T1, T2)(dics As IEnumerable(Of IDictionary(Of T1, T2)), Optional PartialCutOnEach As Boolean = False) As Printer
            Return WriteDictionary(PartialCutOnEach, If(dics, {}).ToArray())
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
                    For Each linha In TemplateString.Inject(obj).SplitAny(BreakLineChars.ToArray())
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

        Public Function PrintDocument(Optional copies As Integer = 1) As Printer Implements IPrinter(Of Printer).PrintDocument
            If _buffer IsNot Nothing Then
                For i = 0 To copies.SetMinValue(1) - 1
                    If Not RawPrinterHelper.SendBytesToPrinter(_printerName, _buffer) Then Throw New ArgumentException("Não foi possível acessar a impressora: " & _printerName)
                Next
            End If
            Return Me
        End Function

        Public Function Image(ByVal path As String, Optional highDensity As Boolean = True) As Printer Implements IPrinter(Of Printer).Image
            If Not File.Exists(path) Then Throw New Exception("Image file not found")
            Return Write(_command.Image.Print(System.Drawing.Image.FromFile(path), highDensity))
        End Function

        Public Function Image(ByVal stream As Stream, ByVal Optional highDensity As Boolean = True) As Printer Implements IPrinter(Of Printer).Image
            Dim img = System.Drawing.Image.FromStream(stream)
            Return Write(_command.Image.Print(img, highDensity))
        End Function

        Public Function Image(ByVal bytes As Byte(), ByVal Optional highDensity As Boolean = True) As Printer Implements IPrinter(Of Printer).Image
            Dim img As System.Drawing.Image

            Using ms = New MemoryStream(bytes)
                img = System.Drawing.Image.FromStream(ms)
            End Using

            Return Write(_command.Image.Print(img, highDensity))
        End Function

        Public Function Image(ByVal pImage As System.Drawing.Image, ByVal Optional highDensity As Boolean = True) As Printer Implements IPrinter(Of Printer).Image
            Return Write(_command.Image.Print(pImage, highDensity))
        End Function

    End Class

End Namespace