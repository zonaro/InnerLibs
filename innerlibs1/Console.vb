

Namespace Console

    ''' <summary>
    ''' Métodos para manipulação de aplicações baseadas em Console (System.Console)
    ''' </summary>
    Public Module Console

        ''' <summary>
        ''' Escreve no console colorindo palavras especificas
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>
        Public Sub Write(Text As String, CustomColoredWords As Dictionary(Of String, ConsoleColor))
            Dim lastcolor = System.Console.ForegroundColor
            Dim substrings As String() = Text.Split(" ")
            For Each substring As String In substrings
                For Each cw In CustomColoredWords
                    If substring = cw.Key Then
                        System.Console.ForegroundColor = cw.Value
                        Exit For
                    End If
                Next
                System.Console.Write(substring & " ")
                System.Console.ForegroundColor = lastcolor
            Next
        End Sub

        ''' <summary>
        ''' Escreve no console usando uma cor especifica
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="Color">Cor</param>
        Public Sub Write(Text As String, Optional Color As ConsoleColor = ConsoleColor.White)
            Dim lastcolor = System.Console.ForegroundColor
            System.Console.ForegroundColor = Color
            System.Console.Write(Text)
            System.Console.ForegroundColor = lastcolor
        End Sub

        ''' <summary>
        ''' Escreve uma linha no console colorindo palavras especificas
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="CustomColoredWords">Lista com as palavras e suas respectivas cores</param>

        Public Sub WriteLine(Text As String, CustomColoredWords As Dictionary(Of String, ConsoleColor))
            Write(Text, CustomColoredWords)
            System.Console.WriteLine("")
        End Sub

        ''' <summary>
        ''' Escreve uma linha no console usando uma cor especifica
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="Color">Cor</param>

        Public Sub WriteLine(Text As String, Optional Color As ConsoleColor = ConsoleColor.White)
            Write(Text, Color)
            System.Console.WriteLine("")
        End Sub
        ''' <summary>
        ''' Pula uma ou mais linhas no console
        ''' </summary>
        ''' <param name="Lines">Numero de linhas</param>
        Public Sub BreakLine(Optional Lines As Integer = 1)
            For index = 1 To Lines.SetMinValue(1)
                System.Console.WriteLine("")
            Next
        End Sub

        ''' <summary>
        ''' Limpa a tela do console
        ''' </summary>
        Public Sub Clear()
            System.Console.Clear()
        End Sub

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
        Public Function Read() As Char
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

