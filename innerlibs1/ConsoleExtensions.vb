Imports System.Runtime.CompilerServices

Namespace Console

    ''' <summary>
    ''' Métodos para manipulação de aplicações baseadas em Console (System.Console)
    ''' </summary>
    Public Module ConsoleExtensions

        ''' <summary>
        ''' Escreve no console colorindo palavras especificas
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>
        <Extension()> Public Function ConsoleWrite(Text As String, CustomColoredWords As Dictionary(Of String, ConsoleColor), Optional Lines As Integer = 0) As String
            Return ConsoleWrite(Text, CustomColoredWords, StringComparison.InvariantCultureIgnoreCase, Lines)
        End Function

        ''' <summary>
        ''' Escreve no console colorindo palavras especificas
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>
        ''' <param name="Comparison">Tipo de comparação</param>
        <Extension()> Public Function ConsoleWrite(Text As String, CustomColoredWords As Dictionary(Of String, ConsoleColor), Comparison As StringComparison, Optional BreakLines As Integer = 0) As String
            CustomColoredWords = If(CustomColoredWords, New Dictionary(Of String, ConsoleColor))

            CustomColoredWords = CustomColoredWords.SelectMany(Function(x) x.Key.Split(" ").Distinct().ToDictionary(Function(y) y, Function(y) x.Value)).ToDictionary()

            Dim lastcolor = System.Console.ForegroundColor
            Dim maincolor = CustomColoredWords.GetValueOr("", lastcolor)
            If Text.IsNotBlank Then
                Dim substrings As String() = Text.Split(" ")
                For Each substring As String In substrings
                    System.Console.ForegroundColor = maincolor
                    System.Console.ForegroundColor = CustomColoredWords.Where(Function(x) x.Key.Equals(substring, Comparison)).Select(Function(x) x.Value).FirstOr(maincolor)
                    System.Console.Write(substring & " ")
                Next
            End If
            System.Console.ForegroundColor = lastcolor
            ConsoleBreakLine(BreakLines)
            Return Text
        End Function

        ''' <summary>
        ''' Escreve no console usando uma cor especifica
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="Color">Cor</param>
        <Extension()> Public Function ConsoleWrite(Text As String, Optional Color As ConsoleColor = ConsoleColor.White, Optional BreakLines As Integer = 0) As String
            Dim lastcolor = System.Console.ForegroundColor
            System.Console.ForegroundColor = Color
            System.Console.Write(Text)
            System.Console.ForegroundColor = lastcolor
            ConsoleBreakLine(BreakLines)
            Return Text
        End Function

        ''' <summary>
        ''' Escreve uma linha no console colorindo palavras especificas
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>

        <Extension()> Public Function ConsoleWriteLine(Text As String, CustomColoredWords As Dictionary(Of String, ConsoleColor), Optional Lines As Integer = 1) As String
            Lines = Lines.SetMinValue(1)
            ConsoleWrite(Text, CustomColoredWords, Lines)
            Return Text
        End Function

        ''' <summary>
        ''' Escreve uma linha no console usando uma cor especifica
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="Color">Cor</param>

        <Extension()> Public Function ConsoleWriteLine(Text As String, Optional Color As ConsoleColor = ConsoleColor.White, Optional Lines As Integer = 1) As String
            Lines = Lines.SetMinValue(1)
            ConsoleWrite(Text, Color, Lines)
            Return Text
        End Function


        ''' <summary>
        ''' Escreve o texto de uma exception no console
        ''' </summary>
        ''' <param name="Exception">Texto</param>
        ''' <param name="Color">Cor</param>

        <Extension()> Public Function ConsoleWriteError(Of T As Exception)(Exception As T, Optional Color As ConsoleColor = ConsoleColor.Red, Optional Lines As Integer = 1) As T
            Lines = Lines.SetMinValue(1)
            ConsoleWrite(Exception.ToFullExceptionString, Color, Lines)
            Return Exception
        End Function

        ''' <summary>
        ''' Pula uma ou mais linhas no console
        ''' </summary>
        ''' <param name="Lines">Numero de linhas</param>
        Public Sub ConsoleBreakLine(Optional Lines As Integer = 1)
            If Lines > 0 Then
                For index = 1 To Lines
                    System.Console.WriteLine("")
                Next
            End If
        End Sub

        ''' <summary>
        ''' Pula uma ou mais linhas no console e retorna a mesma string (usada como chaining)
        ''' </summary>
        ''' <param name="Lines">Numero de linhas</param>
        <Extension()> Public Function ConsoleBreakLine(Text As String, Optional Lines As Integer = 1) As String
            ConsoleBreakLine(Lines)
            Return Text
        End Function

        ''' <summary>
        ''' Le a proxima linha inserida no console pelo usuário
        ''' </summary>
        ''' <returns></returns>
        Public Function ReadLine() As String
            Return System.Console.ReadLine
        End Function

        ''' <summary>
        ''' Le o proximo caractere inserido no console pelo usuário
        ''' </summary>
        ''' <returns></returns>
        Public Function ReadChar() As Char
            Return System.Console.ReadKey.KeyChar
        End Function


        ''' <summary>
        ''' Le a proxima tecla pressionada pelo usuário
        ''' </summary>
        ''' <returns></returns>
        Public Function ReadKey() As ConsoleKey
            Return System.Console.ReadKey.Key
        End Function

        ''' <summary>
        ''' Toca um Beep
        ''' </summary>
        ''' <param name="Times">Numero de beeps</param>
        Public Sub Beep(Optional Times As Integer = 1)
            For index = 1 To Times.SetMinValue(1)
                System.Console.Beep()
            Next
        End Sub

        ''' <summary>
        ''' Toca um beep especifico
        ''' </summary>
        ''' <param name="Frequency">Frequencia</param>
        ''' <param name="Duration">Duracao em milisegundos</param>
        Public Sub Beep(Frequency As Integer, Duration As Integer)
            System.Console.Beep(Frequency.LimitRange(37, 32767), Duration)
        End Sub

        ''' <summary>
        ''' Titulo da janela do console
        ''' </summary>
        ''' <returns></returns>
        Public Property Title As String
            Get
                Return System.Console.Title
            End Get
            Set(value As String)
                System.Console.Title = value
            End Set
        End Property

    End Module

End Namespace