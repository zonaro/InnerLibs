Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

''' <summary>
''' Modulo de Conversão de Cores
''' </summary>
''' <remarks></remarks>
Public Module ColorExtensions


    ''' <summary>
    ''' Retorna a <see cref="ConsoleColor"/> mais proxima de uma <see cref="Color"/>
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    <Extension> Public Function ToConsoleColor(ByVal Color As System.Drawing.Color) As System.ConsoleColor
        Dim index As Integer = If((Color.R > 128 Or Color.G > 128 Or Color.B > 128), 8, 0)
        index = index Or If((Color.R > 64), 4, 0)
        index = index Or If((Color.G > 64), 2, 0)
        index = index Or If((Color.B > 64), 1, 0)
        Return CType(index, System.ConsoleColor)
    End Function

    ''' <summary>
    ''' Retorna a <see cref="Color"/> a partir de uma <see cref="ConsoleColor"/>
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    <Extension> Public Function ToColor(ByVal Color As System.ConsoleColor) As HSVColor
        Dim cColors As Integer() = {&H0, &H80, &H8000, &H8080, &H800000, &H800080, &H808000, &HC0C0C0, &H808080, &HFF, &HFF00, &HFFFF, &HFF0000, &HFF00FF, &HFFFF00, &HFFFFFF}
        Return New HSVColor(cColors(CInt(Color))) With {.Alpha = 255}
    End Function

    <Extension()> Public Function ToHSVColorList(ColorList As IEnumerable(Of Color)) As IEnumerable(Of HSVColor)
        Return ColorList?.Select(Function(x) New HSVColor(x))
    End Function
    Public Function GrayscalePallete(Amount As Integer) As IEnumerable(Of HSVColor)
        Return MonochromaticPallete(Color.White, Amount)
    End Function

    ''' <summary>
    ''' Gera uma paleta de cores monocromatica com <paramref name="Amount"/> amostras a partir de uma <paramref name="Color"/> base.
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    ''' <remarks>A distancia entre as cores será maior se a quantidade de amostras for pequena</remarks>
    Public Function MonochromaticPallete(Color As Color, Amount As Integer) As IEnumerable(Of HSVColor)

        Dim t = New RuleOfThree(Amount, 100, 1, Nothing)

        Dim Percent = t.UnknowValue?.ToSingle()

        Color = Color.White.MergeWith(Color)

        Dim l As New List(Of Color)
        For index = 1 To Amount
            Color = Color.MakeDarker(Percent)
            l.Add(Color)
        Next
        Return l.ToHSVColorList
    End Function

    ''' <summary>
    ''' Retorna  a cor negativa de uma cor
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetNegativeColor(TheColor As Color) As Color
        Return Color.FromArgb(255 - TheColor.R, 255 - TheColor.G, 255 - TheColor.B)
    End Function

    ''' <summary>
    ''' Retorna uma cor de contraste baseado na iluminacao da primeira cor: Uma cor clara se a primeira for escura. Uma cor escura se a primeira for clara
    ''' </summary>
    ''' <param name="TheColor">Primeira cor</param>
    ''' <param name="Percent">Grau de mesclagem da cor escura ou clara</param>
    ''' <returns>Uma cor clara se a primeira cor for escura, uma cor escura se a primeira for clara</returns>
    <Extension()>
    Public Function GetContrastColor(TheColor As Color, Optional Percent As Single = 70) As Color
        Dim a As Double = 1 - (0.299 * TheColor.R + 0.587 * TheColor.G + 0.114 * TheColor.B) / 255
        Dim d = If(a < 0.5, 0, 255)
        Return TheColor.MergeWith(Color.FromArgb(d, d, d), Percent)
    End Function

    ''' <summary>
    ''' Verifica se uma cor é escura
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <returns></returns>
    <Extension>
    Public Function IsDark(TheColor As Color) As Boolean
        Return New HSVColor(TheColor).IsDark
    End Function

    ''' <summary>
    ''' Verifica se uma clor é clara
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <returns></returns>
    <Extension>
    Public Function IsLight(TheColor As Color) As Boolean
        Return Not TheColor.IsDark()
    End Function

    ''' <summary>
    ''' Mescla duas cores a partir de uma porcentagem
    ''' </summary>
    ''' <param name="TheColor">Cor principal</param>
    ''' <param name="AnotherColor">Cor de mesclagem</param>
    ''' <param name="percent">Porcentagem de mescla</param>
    ''' <returns></returns>
    <Extension>
    Public Function MergeWith(TheColor As Color, AnotherColor As Color, Optional Percent As Single = 50) As Color
        Return TheColor.Lerp(AnotherColor, Percent / 100)
    End Function

    ''' <summary>
    ''' Escurece a cor mesclando ela com preto
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <param name="percent">porcentagem de mesclagem</param>
    ''' <returns></returns>
    <Extension>
    Public Function MakeDarker(TheColor As Color, Optional Percent As Single = 50) As Color
        Return TheColor.MergeWith(Color.Black, Percent)
    End Function

    ''' <summary>
    ''' Clareia a cor mistuando ela com branco
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <param name="percent">Porcentagem de mesclagem</param>
    ''' <returns></returns>
    <Extension>
    Public Function MakeLighter(TheColor As Color, Optional Percent As Single = 50) As Color
        Return TheColor.MergeWith(Color.White, Percent)
    End Function

    ''' <summary>
    ''' Mescla duas cores usando Lerp
    ''' </summary>
    ''' <param name="FromColor">Cor</param>
    ''' <param name="ToColor">Outra cor</param>
    ''' <param name="amount">Indice de mesclagem</param>
    ''' <returns></returns>
    <Extension>
    Public Function Lerp(FromColor As Color, ToColor As Color, Amount As Single) As Color
        ' start colours as lerp-able floats
        Dim sr As Single = FromColor.R, sg As Single = FromColor.G, sb As Single = FromColor.B
        ' end colours as lerp-able floats
        Dim er As Single = ToColor.R, eg As Single = ToColor.G, eb As Single = ToColor.B
        ' lerp the colours to get the difference
        Dim r As Byte = CByte(sr.Lerp(er, Amount)), g As Byte = CByte(sg.Lerp(eg, Amount)), b As Byte = CByte(sb.Lerp(eb, Amount))
        ' return the new colour
        Return Color.FromArgb(r, g, b)
    End Function

    ''' <summary>
    ''' Converte uma cor de sistema para hexadecimal
    ''' </summary>
    ''' <param name="Color">Cor do sistema</param>
    ''' <param name="Hash">parametro indicando se a cor deve ser retornada com ou sem hashsign (#)</param>
    ''' <returns>string contendo o hexadecimal da cor</returns>

    <Extension()>
    Public Function ToHexadecimal(Color As System.Drawing.Color, Optional Hash As Boolean = True) As String
        Return (Color.R.ToString("X2") & Color.G.ToString("X2") & Color.B.ToString("X2")).PrependIf("#", Hash)
    End Function

    ''' <summary>
    ''' Converte uma cor de sistema para CSS RGB
    ''' </summary>
    ''' <param name="Color">Cor do sistema</param>
    ''' <returns>String contendo a cor em RGB</returns>

    <Extension()>
    Public Function ToCssRGB(Color As System.Drawing.Color) As String
        Return "rgb(" & Color.R.ToString() & "," & Color.G.ToString() & "," & Color.B.ToString() & ")"
    End Function

    <Extension()>
    Public Function ToCssRGBA(Color As System.Drawing.Color) As String
        Return "rgba(" & Color.R.ToString() & "," & Color.G.ToString() & "," & Color.B.ToString() & "," & Color.A.ToString() & ")"
    End Function

    <Extension()> Public Function IsHexaDecimalColor(ByVal Text As String) As Boolean
        Text = Text.RemoveFirstEqual("#")
        Dim myRegex As Regex = New Regex("^[a-fA-F0-9]+$")
        Return Text.IsNotBlank AndAlso myRegex.IsMatch(Text)
    End Function




    ''' <summary>
    ''' Gera uma cor a partir de uma palavra
    ''' </summary>
    ''' <param name="Text">Pode ser um texto em branco (Cor aleatória), uma <see cref="KnownColor"/> (retorna aquela cor exata) ou uma palavra qualquer (gera proceduralmente uma cor)</param>
    ''' <returns></returns>
    <Extension> Public Function ToColor(Text As String) As Color

        If Text.IsBlank() Then
            Return Color.Transparent
        End If

        If Text.IsIn("random", "rand") Then
            Return RandomColor()
        End If



        If Text.IsNumber Then
            Return Color.FromArgb(Text.ToInteger())
        End If

        If Text.IsIn(KnowColors.Select(Function(x) x.Name), StringComparer.InvariantCultureIgnoreCase) Then
            Return KnowColors.FirstOrDefault(Function(x) x.Name.ToLower() = Text.ToLower())
        End If

        If Text.IsHexaDecimalColor Then
            Return ColorTranslator.FromHtml("#" & Text.RemoveFirstEqual("#").IfBlank("000000"))
        End If

        Dim coresInt = Text.GetWords.Select(Function(p) p.ToCharArray().Sum(Function(a) AscW(a) ^ 2 * p.Length)).Sum()

        Return Color.FromArgb(255, Color.FromArgb(coresInt))

    End Function

    ''' <summary>
    ''' Gera uma cor aleatória misturandoo ou não os canais RGB
    ''' </summary>
    ''' <param name="Red">-1 para Random ou de 0 a 255 para especificar o valor</param>
    ''' <param name="Green">-1 para Random ou de 0 a 255 para especificar o valor</param>
    ''' <param name="Blue">-1 para Random ou de 0 a 255 para especificar o valor</param>
    ''' <returns></returns>
    Public Function RandomColor(Optional Red As Integer = -1, Optional Green As Integer = -1, Optional Blue As Integer = -1, Optional Alpha As Integer = 255) As Color
        Red = If(Red < 0, RandomNumber(0, 255), Red).LimitRange(Of Integer)(0, 255)
        Green = If(Green < 0, RandomNumber(0, 255), Green).LimitRange(Of Integer)(0, 255)
        Blue = If(Blue < 0, RandomNumber(0, 255), Blue).LimitRange(Of Integer)(0, 255)
        Alpha = Alpha.LimitRange(Of Integer)(0, 255)
        Return Color.FromArgb(Alpha, Red, Green, Blue)
    End Function

    ''' <summary>
    ''' Lista com todas as <see cref="KnownColor"/> convertidas em <see cref="System.Drawing.Color"/> (Igonora as systemcolors)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property KnowColors As IEnumerable(Of Color)
        Get
            Return [Enum].GetValues(GetType(KnownColor)).Cast(Of KnownColor).Select(Function(x) Color.FromKnownColor(x)).Where(Function(x) x.IsSystemColor = False)
        End Get
    End Property

    ''' <summary>
    ''' Retorna uma <see cref="KnownColor"/> mais proxima de outra cor
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetClosestKnowColor(Color As Color) As Color
        Dim closest_distance As Double = Double.MaxValue
        Dim closest As Color = Color.White
        For Each kc In KnowColors
            Dim d = EuclideanDistance(Color, kc)
            If d < closest_distance Then
                closest_distance = d
                closest = kc
            End If
        Next
        Return closest
    End Function

    ''' <summary>
    ''' Retorna a distancia euclideana
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <param name="OtherColor"></param>
    ''' <returns></returns>
    <Extension> Public Function EuclideanDistance(Color As Color, OtherColor As Color) As Double
        Dim r_dist_sqrd As Double = Math.Pow(CDbl(Color.R) - CDbl(OtherColor.R), 2)
        Dim g_dist_sqrd As Double = Math.Pow(CDbl(Color.G) - CDbl(OtherColor.G), 2)
        Dim b_dist_sqrd As Double = Math.Pow(CDbl(Color.B) - CDbl(OtherColor.B), 2)
        Return Math.Sqrt(r_dist_sqrd + g_dist_sqrd + b_dist_sqrd)
    End Function

    ''' <summary>
    ''' Retorna o nome comum mais proximo a esta cor
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetClosestColorName(Color As Color) As String
        Return Color.GetClosestKnowColor().Name
    End Function

    ''' <summary>
    ''' Retorna o nome da cor
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetColorName(Color As Color) As String
        For Each namedColor In KnowColors
            If Color.CompareARGB(namedColor) Then Return namedColor.Name
        Next
        Return Color.Name
    End Function

    ''' <summary>
    ''' Verifica se uma cor é legivel sobre outra
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <param name="BackgroundColor"></param>
    ''' <param name="Size"></param>
    ''' <returns></returns>
    <Extension()> Public Function IsReadable(Color As Color, BackgroundColor As Color, Optional Size As Integer = 10) As Boolean
        If Color.A = 0 Then Return False
        If BackgroundColor.A = 0 Then Return True
        Dim diff = BackgroundColor.R * 0.299 + BackgroundColor.G * 0.587 + BackgroundColor.B * 0.114 - Color.R * 0.299 - Color.G * 0.587 - Color.B * 0.114
        Return Not ((diff < (1.5 + 141.162 * Math.Pow(0.975, Size)))) AndAlso (diff > (-0.5 - 154.709 * Math.Pow(0.99, Size)))
    End Function

End Module

