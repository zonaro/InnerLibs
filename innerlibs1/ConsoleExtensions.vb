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
        <Extension()> Public Function ConsoleWrite(Text As String, CustomColoredWords As Dictionary(Of String, ConsoleColor)) As String
            Return ConsoleWrite(Text, CustomColoredWords, StringComparison.InvariantCultureIgnoreCase)
        End Function

        ''' <summary>
        ''' Escreve no console colorindo palavras especificas
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>
        ''' <param name="Comparison">Tipo de comparação</param>
        <Extension()> Public Function ConsoleWrite(Text As String, CustomColoredWords As Dictionary(Of String, ConsoleColor), Comparison As StringComparison, Optional BreakLines As Integer = 0) As String

            Dim lastcolor = System.Console.ForegroundColor
            Dim substrings As String() = Text.Split(" ")
            For Each substring As String In substrings
                For Each cw In CustomColoredWords
                    If substring.Equals(cw.Key, Comparison) Then
                        System.Console.ForegroundColor = cw.Value
                        Exit For
                    End If
                Next
                System.Console.Write(substring & " ")
                System.Console.ForegroundColor = lastcolor
            Next
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
            ConsoleWrite(Text, CustomColoredWords, Lines.SetMinValue(1))
            Return Text
        End Function

        ''' <summary>
        ''' Escreve uma linha no console usando uma cor especifica
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="Color">Cor</param>

        <Extension()> Public Function ConsoleWriteLine(Text As String, Optional Color As ConsoleColor = ConsoleColor.White, Optional Lines As Integer = 1) As String
            ConsoleWrite(Text, Color)
            ConsoleBreakLine(Lines.SetMinValue(1))
            Return Text
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