Imports System.Drawing
Imports System.Net
Imports System.Runtime.CompilerServices
Imports InnerLibs.LINQ
Imports InnerLibs.Locations

''' <summary>
''' Geradores de conteudo
''' </summary>
''' <remarks></remarks>
Public Module Generate

    ''' <summary>
    ''' Gera uma palavra aleatória com o numero de caracteres
    ''' </summary>
    ''' <param name="Length">Tamanho da palavra</param>
    ''' <returns>Uma string contendo uma palavra aleatória</returns>
    Public Function RandomWord(Optional Length As Integer = 0) As String
        Length = If(Length < 1, RandomNumber(2, 15), Length)
        Dim rnd As New Random()
        Dim consonants As String() = {"b", "c", "d", "f", "g", "h",
        "j", "k", "l", "m", "n", "p",
        "q", "r", "s", "t", "v", "w",
        "x", "y", "z"}
        Dim vowels As String() = {"a", "e", "i", "o", "u"}

        Dim word As String = ""

        ' Generate the word in consonant / vowel pairs
        While word.Length < Length
            If Length <> 1 Then
                ' Add the consonant
                Dim consonant As String = GetRandomLetter(rnd, consonants)

                If consonant = "q" AndAlso word.Length + 3 <= Length Then
                    ' check +3 because we'd add 3 characters in this case, the "qu" and the vowel.  Change 3 to 2 to allow words that end in "qu"
                    word += "qu"
                Else
                    While consonant = "q"
                        ' ReplaceFrom an orphaned "q"
                        consonant = GetRandomLetter(rnd, consonants)
                    End While

                    If word.Length + 1 <= Length Then
                        ' Only add a consonant if there's enough room remaining
                        word += consonant
                    End If
                End If
            End If

            If word.Length + 1 <= Length Then
                ' Only add a vowel if there's enough room remaining
                word += GetRandomLetter(rnd, vowels)
            End If
        End While
        Return word
    End Function

    Public Function RandomString()
        For index = 1 To 10

        Next

        AlphaChars.OrderByRandom()
    End Function

    ''' <summary>
    ''' Gera uma palavra aleatória a partir de uma outra palavra
    ''' </summary>
    ''' <param name="BaseText">Texto base</param>
    ''' <returns></returns>
    Public Function RandomWord(BaseText As String) As String
        Return BaseText.ToArray.Shuffle.Join("")
    End Function

    Private Function GetRandomLetter(rnd As Random, letters As String()) As String
        Return letters(rnd.[Next](0, letters.Length - 1))
    End Function




    ''' <summary>
    ''' Gera uma URL do google MAPs baseado na localização
    ''' </summary>
    ''' <param name="local">Uma variavel do tipo InnerLibs.Location onde estão as informações como endereço e as coordenadas geográficas</param>
    ''' <param name="LatLong">Gerar URL baseado na latitude e Longitude. Padrão FALSE retorna a URL baseada no Logradouro</param>
    ''' <returns>Uma URI do Google Maps</returns>

    <Extension()>
    Public Function ToGoogleMapsURL(local As AddressInfo, Optional LatLong As Boolean = False) As Uri
        Dim s As String
        If LatLong = True AndAlso local.Latitude.HasValue AndAlso local.Longitude.HasValue Then
            s = Uri.EscapeUriString(AdjustWhiteSpaces(local.LatitudeLongitude))
        Else
            s = Uri.EscapeUriString(AdjustWhiteSpaces(local.FullAddress))
        End If

        Return New Uri("https://www.google.com.br/maps/search/" & s)

    End Function




    ''' <summary>
    ''' Gera um valor boolean aleatorio considerando uma porcentagem de chance
    ''' </summary>
    ''' <returns>TRUE ou FALSE.</returns>
    Function RandomBoolean(Percent As Integer) As Boolean
        Return RandomBoolean(Function(x) x <= Percent, 0, 100)
    End Function

    ''' <summary>
    ''' Gera um valor boolean aleatorio considerando uma condiçao
    ''' </summary>
    ''' <param name="Min">Numero minimo, Padrão 0 </param>
    ''' <param name="Max">Numero Maximo, Padrão 999999</param>
    ''' <returns>TRUE ou FALSE</returns>
    Function RandomBoolean(Condition As Func(Of Long, Boolean), Optional Min As Long = 0, Optional Max As Long = 999999) As Boolean
        Return Condition(init_rnd.Next(Min, Max + 1))
    End Function

    ''' <summary>
    ''' Gera um valor boolean aleatorio
    ''' </summary>
    ''' <returns>TRUE ou FALSE</returns>
    Function RandomBoolean() As Boolean
        Return init_rnd.Next(0, 1).ToBoolean()
    End Function



    ''' <summary>
    ''' Gera um numero Aleatório entre 2 números
    ''' </summary>
    ''' <param name="Min">Numero minimo, Padrão 0 </param>
    ''' <param name="Max">Numero Maximo, Padrão 999999</param>
    ''' <returns>Um numero Inteiro (Integer ou Int)</returns>
    Function RandomNumber(Optional Min As Integer = 0, Optional Max As Integer = 999999) As Integer
        Return init_rnd.Next(Min, Max + 1)
    End Function


    ''' <summary>
    ''' Gera uma lista com <paramref name="Quantity"/> cores diferentes
    ''' </summary>
    ''' <param name="Quantity">Quantidade máxima de cores</param>
    ''' <param name="Red"></param>
    ''' <param name="Green"></param>
    ''' <param name="Blue"></param>
    ''' <remarks></remarks>
    ''' <returns></returns>
    Public Function RandomColorList(Quantity As Integer, Optional Red As Integer = -1, Optional Green As Integer = -1, Optional Blue As Integer = -1) As List(Of Color)
        Dim l As New List(Of Color)
        If Red = Green AndAlso Green = Blue AndAlso Blue <> -1 Then
            l.Add(Color.FromArgb(Red, Green, Blue))
            Return l
        End If
        Dim errorcount = 0
        While l.Count < Quantity
            Dim r = RandomColor(Red, Green, Blue)
            If l.Any(Function(x) x.ToHexadecimal = r.ToHexadecimal) Then
                errorcount = errorcount + 1
                If errorcount = 5 Then
                    Return l
                End If
            Else
                errorcount = 0
                l.Add(r)
            End If
        End While
        Return l
    End Function

    Private init_rnd As Random = New Random()

    ''' <summary>
    ''' Gera um texto aleatorio
    ''' </summary>
    ''' <param name="ParagraphCount">Quantidade de paragrafos</param>
    ''' <param name="SentenceCount">QUantidade de sentecas por paragrafo</param>
    ''' <param name="MinWordCount"></param>
    ''' <param name="MaxWordCount"></param>
    ''' <returns></returns>
    Function RandomIpsum(Optional ParagraphCount As Integer = 5, Optional SentenceCount As Integer = 3, Optional MinWordCount As Integer = 10, Optional MaxWordCount As Integer = 50, Optional IdentSize As Integer = 0, Optional BreakLinesBetweenParagraph As Integer = 0) As StructuredText
        Return New StructuredText(Enumerable.Range(1, ParagraphCount.SetMinValue(1)).SelectJoin(Function(pp) Enumerable.Range(1, SentenceCount.SetMinValue(1)).SelectJoin(Function(s) Enumerable.Range(1, RandomNumber(MinWordCount.SetMinValue(1), MaxWordCount.SetMinValue(1))).SelectJoin(Function(p) RandomBoolean(20).AsIf(RandomWord(RandomNumber(2, 6)).ToUpper(), RandomWord()) & RandomBoolean(30).AsIf(","), " "), EndOfSentencePunctuation.FirstRandom() & " "), Environment.NewLine)) With {.Ident = IdentSize, .BreakLinesBetweenParagraph = BreakLinesBetweenParagraph}
    End Function



    ''' <summary>
    ''' Converte uma String para um QR Code usando uma API (Nescessita de Internet)
    ''' </summary>
    ''' <param name="Data">Informações do QR Code</param>
    ''' <param name="Size">Tamanho do QR code</param>
    ''' <returns>Um componente Image() com o QR code</returns>

    <Extension()>
    Public Function ToQRCode(Data As String, Optional Size As Integer = 100) As Byte()
        Data = If(Data.IsURL, UrlEncode(Data), Data)
        Dim URL As String = "https://chart.googleapis.com/chart?cht=qr&chl=" & Data.UrlEncode & "&chs=" & Size & "x" & Size
        Return GetFile(URL)
    End Function

End Module