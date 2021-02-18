Imports System.Drawing
Imports System.Net
Imports System.Runtime.CompilerServices
Imports System.Web
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
        Length = If(Length < 1, RandomNumber(4, 15), Length)
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
    ''' Gera uma Imagem com uma fonte específica do site Dafont (Muito Impreciso, API em fase experimental)
    ''' </summary>
    ''' <param name="Text">Um texto qualquer a ser usado como exemplo</param>
    ''' <param name="FontName">Nome da fonte (verifique no site)</param>
    ''' <param name="FontSize">Tamanho da fonte</param>
    ''' <returns>Uma (<see cref="System.Drawing.Image"/>)</returns>
    <Obsolete("Muito Impreciso, API em fase experimental")>
    Public Function DaFontLogo(Text As String, FontName As String, Optional FontSize As String = "") As Image
        Dim Client As New WebClient
        Dim URL As String = "http://img.dafont.com/preview.php?text=" & Text & "&ttf=" & FontName.Replace(" ", "_") & "0&ext=1&size=68&psize=" & FontSize & "&y=70"
        Return Image.FromStream(Client.OpenRead(URL))

    End Function
    ''' <summary>
    ''' Tira uma screenshot de um site usando o servico ATS
    ''' </summary>
    ''' <param name="Url">Url do site</param>
    ''' <returns>Um objeto Image() contendo o screenshot do site</returns>
    <Extension>
    Function ScreenshotFromWebsite(Url As String, AccessKey As String, Optional FullPage As Boolean = True, Optional Delay As Integer = 1, Optional Viewport As String = "1440x900", Optional ImageWidth As Integer = 500) As Image
        If Url.IsURL Then
            Return AJAX.GET(Of Image)("http://api.screenshotlayer.com/api/capture?access_key=" & AccessKey & "&delay=" & Delay & "&url=" & Url & "&fullpage=" & If(FullPage, 1, 0) & "&viewport=" & Viewport & "&width=" & ImageWidth)
        Else
            Throw New Exception("Url inválida")
        End If
    End Function


    ''' <summary>
    ''' Gera uma URL do google MAPs baseado na localização
    ''' </summary>
    ''' <param name="local">Uma variavel do tipo InnerLibs.Location onde estão as informações como endereço e as coordenadas geográficas</param>
    ''' <param name="LatLong">Gerar URL baseado na latitude e Longitude. Padrão FALSE retorna a URL baseada no Logradouro</param>
    ''' <returns>Uma URI do Google Maps</returns>

    <Extension()>
    Public Function ToGoogleMapsURL(local As Location, Optional LatLong As Boolean = False) As Uri
        Dim s As String
        If LatLong = True Then
            s = Uri.EscapeUriString(AdjustWhiteSpaces(local.LatitudeLongitude))
        Else
            s = Uri.EscapeUriString(AdjustWhiteSpaces(local.Address))
        End If

        Return New Uri("https://www.google.com.br/maps/search/" & s)

    End Function
    ''' <summary>
    ''' Cria um Mapa estatico utilizando a API do google Maps
    ''' </summary>
    ''' <param name="Location">Objeto contendo as Coordenadas</param>
    ''' <param name="Width">Largura do Mapa</param>
    ''' <param name="Height">Altura do Mapa</param>
    ''' <param name="Zoom">Numero correspondente a aproximação da vizualização do Mapa</param>
    ''' <param name="Scale">Escala do mapa (qualidade)</param>
    ''' <param name="Maptype">Tipo do Mapa (roadmap, satellite, hybrid, ou terrain)</param>
    ''' <returns>Um componente Image() com o mapa</returns>

    <Extension()>
    Public Function ToStaticGoogleMap(Location As Location, Optional Width As Integer = 400, Optional Height As Integer = 400, Optional Zoom As Integer = 16, Optional Scale As Integer = 2, Optional Maptype As MapType = Generate.MapType.RoadMap) As Image
        Dim thecenter = If(Location.Latitude.IsBlank, Location.Address.Replace(" ", "+"), Location.LatitudeLongitude)
        Dim mapstring = ""
        Select Case Maptype
            Case 1
                mapstring = "Satellite"
            Case 2
                mapstring = "Hybrid"
            Case 3
                mapstring = "Terrain"
            Case Else
                mapstring = "RoadMap"
        End Select
        Dim URL As String = "http//maps.googleapis.com/maps/api/staticmap?center=" & thecenter & "&zoom=" & Zoom & "&size=" & Width & "x" & Height & "&scale=" & Scale & "&maptype=" & mapstring.ToLower
        Return AJAX.GET(Of Image)(URL)
    End Function
    ''' <summary>
    ''' Tipo de mapa do Google Maps
    ''' </summary>
    '''

    Enum MapType
        ''' <summary>
        ''' Rotas
        ''' </summary>

        RoadMap = 0
        ''' <summary>
        ''' Visao de satelite
        ''' </summary>

        Satellite = 1
        ''' <summary>
        ''' Hibrido (Rotas + Satelite)
        ''' </summary>

        Hybrid = 2
        ''' <summary>
        ''' Terreno/Relevo
        ''' </summary>

        Terrain = 3
    End Enum

    ''' <summary>
    ''' Gera um numero Aleatório entre 2 números
    ''' </summary>
    ''' <param name="Min">Numero minimo, Padrão 0 </param>
    ''' <param name="Max">Numero Maximo, Padrão 999999</param>
    ''' <returns>Um numero Inteiro (Integer ou Int)</returns>
    Function RandomNumber(Optional Min As Long = 0, Optional Max As Long = 999999) As Integer
        Return init_rnd.Next(Min, Max + 1)
    End Function

    ''' <summary>
    ''' Gera uma cor a partir de uma palavra
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension> Public Function WordToColor(Text As String) As Color
        If Text.IsBlank() Then
            Return RandomColor()
        End If
        If Text.IsNumber Then
            Return Color.FromArgb(Text.ToInteger())
        End If
        Dim coresInt = Text.GetWords.Select(Function(p) p.ToCharArray().Sum(Function(a) AscW(a) ^ 2 * p.Length)).Sum()

        Return Color.FromArgb(coresInt)
    End Function

    ''' <summary>
    ''' Gera uma cor aleatória misturandoo ou não os canais RGB
    ''' </summary>
    ''' <param name="Red">-1 para Random ou de 0 a 255 para especificar o valor</param>
    ''' <param name="Green">-1 para Random ou de 0 a 255 para especificar o valor</param>
    ''' <param name="Blue">-1 para Random ou de 0 a 255 para especificar o valor</param>
    ''' <returns></returns>
    Public Function RandomColor(Optional Red As Integer = -1, Optional Green As Integer = -1, Optional Blue As Integer = -1) As Color
        Red = If(Red < 0, RandomNumber(0, 255), Red).LimitRange(0, 255)
        Green = If(Green < 0, RandomNumber(0, 255), Green).LimitRange(0, 255)
        Blue = If(Blue < 0, RandomNumber(0, 255), Blue).LimitRange(0, 255)
        Dim cor = Color.FromArgb(Red, Green, Blue)
        Return cor
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

    Private init_rnd = New Random()

    ''' <summary>
    ''' Gera um InnerIpsum (InnerIpsum é uma modificação do classico Lorem Ipsum)
    ''' </summary>
    ''' <param name="ParagraphNumber">Quantidade de parágrafos do texto</param>
    ''' <returns>Uma String contendo diversos paragrafos com texto aleatório</returns>

    Function InnerIpsum(Optional ParagraphNumber As Integer = 5) As String
        Dim paragraphs(0 To 4) As String
        Dim loremipusm As String = ""
        paragraphs(0) = "Null Set aliquam est. display none efficitur Inner vitae augue imperdiet scelerisque. Vivamus fermentum arcu pulvinar fermentum laoreet. Phasellus id ante _POST. Praesent at blandit null, at ornare nisl. Quisque cursus non mi vitae facilisis. Maecenas command sem _POST, send tincidunt felis cursus Set. Return at fakepath enim, malesuada consequat justo. Mauris blandit egestas Inner, Get amet interdum eros ullamcorper Set. Fusce mollis, risus id rutrum efficitur, magna risus rhoncus release, id #000000 lacus magna AJAX sapien. null facilisi."
        paragraphs(1) = "Python sagittis orci est, quis egestas cakePHP request quis. Pellentesque sodales suscipit consequat. Fusce semper nunc quis leo porttitor, eu command est imperdiet. Integer euismod fringilla aliquet. Etiam nisl risus, tincidunt quis fakepath vel, venenatis ut tellus. Vivamus hendrerit gravida imperdiet. display none get amet hendrerit ante. Donuts tincidunt turpis get amet split viverra sodales."
        paragraphs(2) = "Praesent vitae vehicula arcu. Interdum et malesuada fames ac ante ipsum primis in fakepath. null get amet ante get amet nisi drag n drop gravida in quis null. Curabitur arcu code, lacinia In nisi congue, getElementById condimentum lectus. In eu sapien vitae nunc imperdiet blandit vel get amet quam. display: none vel drag n drop massa, eu scelerisque est. Quisque vel purus sem."
        paragraphs(3) = "Inner ipsum code GET amet, getElementById adipiscing split. Mauris finibus, felis id command dapibus, nisl ex #000000 nisl, id euismod mauris magna vel purus. send luctus augue euismod sapien rhoncus, et mattis lectus malesuada. Dispose fermentum turpis a getElementById accumsan. Aliquam send orci nibh. In code sem, pulvinar ultrices drop id, ultrices eu release. Ut tempor metus urna. Return Get amet imperdiet turpis. Integer command ac ipsum send tempor. Aenean AJAX mollis null, volutpat euismod eros. _POSTs vitae ante Set _POST luctus viverra condimentum id _POST. Cras vehicula congue ante, fakepath viverra quam rutrum Set. Return non finibus ipsum, send ultrices lectus. Donuts tempus convallis purus ut ornare. Nam vel split sem."
        paragraphs(4) = "Interdum et malesuada fames ac ante ipsum primis In fakepath. send efficitur, leo Get amet accumsan drag n drop, urna Inner pulvinar erat, AJAX dapibus arcu mauris vel felis. Nam in ante get amet vsplit drag n drop tincidunt. display: none at arcu at quam gravida fringilla. Proin et vehicula lacus. nullm leo turpis, auctor ac volutpat euismod, posuere quis felis. nullm dapibus diam vel #000000 facilisis. Curabitur at purus in ante aliquet porta."

        For i = 1 To ParagraphNumber
            loremipusm &= paragraphs(RandomNumber(0, paragraphs.Length)) & vbNewLine & vbNewLine
        Next
        Return loremipusm
    End Function

    ''' <summary>
    ''' Gera um InnerIpsum (InnerIpsum é uma modificação do classico Lorem Ipsum)
    ''' </summary>
    ''' <param name="ParagraphNumber">Quantidade de parágrafos do texto</param>
    ''' <returns>Uma String contendo diversos paragrafos com texto aleatório</returns>
    '''
    Function LoremIpsum(Optional ParagraphNumber As Integer = 5) As String
        Return InnerIpsum(ParagraphNumber)
    End Function

    ''' <summary>
    ''' Converte uma String para um QR Code usando uma API (Nescessita de Internet)
    ''' </summary>
    ''' <param name="Data">Informações do QR Code</param>
    ''' <param name="Size">Tamanho do QR code</param>
    ''' <returns>Um componente Image() com o QR code</returns>

    <Extension()>
    Public Function ToQRCode(Data As String, Optional Size As Integer = 100) As Image
        Data = If(Data.IsURL, HttpUtility.UrlEncode(Data), Data)
        Dim URL As String = "https://chart.googleapis.com/chart?cht=qr&chl=" & Data.UrlEncode & "&chs=" & Size & "x" & Size
        Return AJAX.GET(Of Image)(URL)
    End Function

End Module