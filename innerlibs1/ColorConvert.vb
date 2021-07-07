Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

''' <summary>
''' Modulo de Conversão de Cores
''' </summary>
''' <remarks></remarks>
Public Module ColorConvert

    Public Function GrayscalePallete(Amount As Integer) As IEnumerable(Of Color)
        Return MonochromaticPallete(Color.White, Amount)
    End Function

    Public Function MonochromaticPallete(Color As Color, Amount As Integer) As IEnumerable(Of Color)

        Dim t = New RuleOfThree(Amount, 100, 1, Nothing)
        Dim Percent = t.SecondEquation.Y

        Color = Color.White.MergeWith(Color)

        Dim l As New List(Of Color)
        For index = 1 To Amount
            Color = Color.MakeDarker(Percent)
            l.Add(Color)
        Next
        Return l
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
    Public Function GetContrastColor(TheColor As Color, Optional Percent As Single = 0.7) As Color
        Dim d As Integer = 0
        Dim a As Double = 1 - (0.299 * TheColor.R + 0.587 * TheColor.G + 0.114 * TheColor.B) / 255
        d = If(a < 0.5, 0, 255)
        Return TheColor.MergeWith(Color.FromArgb(d, d, d), Percent)
    End Function

    ''' <summary>
    ''' Verifica se uma cor é escura
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <returns></returns>
    <Extension>
    Public Function IsDark(TheColor As Color) As Boolean
        Dim Y = 0.2126 * TheColor.R + 0.7152 * TheColor.G + 0.0722 * TheColor.B
        Return Y < 128
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
    ''' Mescal duas cores a partir de uma porcentagem
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
    Public Function MakeDarker(TheColor As Color, Optional percent As Single = 50) As Color
        Return TheColor.MergeWith(Color.Black, percent)
    End Function

    ''' <summary>
    ''' Clareia a cor mistuando ela com branco
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <param name="percent">Porcentagem de mesclagem</param>
    ''' <returns></returns>
    <Extension>
    Public Function MakeLighter(TheColor As Color, Optional percent As Single = 50) As Color
        Return TheColor.MergeWith(Color.White, percent)
    End Function

    ''' <summary>
    ''' Mescla duas cores usando Lerp
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <param name="[to]">Outra cor</param>
    ''' <param name="amount">Indice de mesclagem</param>
    ''' <returns></returns>
    <Extension>
    Public Function Lerp(TheColor As Color, [to] As Color, amount As Single) As Color
        ' start colours as lerp-able floats
        Dim sr As Single = TheColor.R, sg As Single = TheColor.G, sb As Single = TheColor.B
        ' end colours as lerp-able floats
        Dim er As Single = [to].R, eg As Single = [to].G, eb As Single = [to].B
        ' lerp the colours to get the difference
        Dim r As Byte = CByte(sr.Lerp(er, amount)), g As Byte = CByte(sg.Lerp(eg, amount)), b As Byte = CByte(sb.Lerp(eb, amount))
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
        Dim Recolor = Color.R.ToString("X2") & Color.G.ToString("X2") & Color.B.ToString("X2")
        Return If(Hash, "#" & Recolor, Recolor)
    End Function

    ''' <summary>
    ''' Converte uma cor de sistema para CSS RGB
    ''' </summary>
    ''' <param name="Color">Cor do sistema</param>
    ''' <returns>String contendo a cor em RGB</returns>

    <Extension()>
    Public Function ToRGB(Color As System.Drawing.Color) As String
        Return "RGB(" & Color.R.ToString() & "," & Color.G.ToString() & "," & Color.B.ToString() & ")"
    End Function

    <Extension()>
    Public Function ToRGBA(Color As System.Drawing.Color) As String
        Return "rgba(" & Color.R.ToString() & "," & Color.G.ToString() & "," & Color.B.ToString() & "," & Color.A.ToString() & ")"
    End Function

    <Extension> Public Function IsHexaDecimal(ByVal Text As String) As Boolean
        Dim i As Int32
        Return Int32.TryParse(Text, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, i)
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
            Return RandomColor()
        End If

        If Text.IsIn([Enum].GetNames(GetType(KnownColor)), StringComparer.InvariantCultureIgnoreCase) Then Return Color.FromName(Text)

        If Text.IsNumber Then
            Return Color.FromArgb(Text.ToInteger())
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
    Public Function RandomColor(Optional Red As Integer = -1, Optional Green As Integer = -1, Optional Blue As Integer = -1) As Color
        Red = If(Red < 0, RandomNumber(0, 255), Red).LimitRange(0, 255)
        Green = If(Green < 0, RandomNumber(0, 255), Green).LimitRange(0, 255)
        Blue = If(Blue < 0, RandomNumber(0, 255), Blue).LimitRange(0, 255)
        Dim cor = Color.FromArgb(Red, Green, Blue)
        Return cor
    End Function

End Module

Public Class HSVColor

    Private _h, _s, _v As Double

    ''' <summary>
    ''' Hue (Matiz)
    ''' </summary>
    ''' <returns></returns>
    Property H As Double
        Get
            Return _h
        End Get
        Set(value As Double)
            If _h <> value Then
                _h = value
                SetColor()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Saturation (Saturação)
    ''' </summary>
    ''' <returns></returns>
    Property S As Double
        Get
            Return _s
        End Get
        Set(value As Double)
            If _s <> value Then
                _s = value
                SetColor()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Value (Brilho)
    ''' </summary>
    ''' <returns></returns>
    Property V As Double
        Get
            Return _v
        End Get
        Set(value As Double)
            If _v <> value Then
                _v = value
                SetColor()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Red (Vermelho)
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property R As Integer
        Get
            Return _scolor.R
        End Get
    End Property

    ''' <summary>
    ''' Green (Verde)
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property G As Integer
        Get
            Return _scolor.G
        End Get
    End Property

    ''' <summary>
    ''' Blue (Azul)
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property B As Integer
        Get
            Return _scolor.B
        End Get
    End Property

    ''' <summary>
    ''' Alpha (Transparencia)
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property A As Byte
        Get
            Return _scolor.A
        End Get
    End Property

    Private Sub SetColor()

        Dim H, S, V As Double
        Dim alpha = _scolor.A

        H = Me.H
        S = Me.S
        V = Me.V

        While H < 0
            H += 360
        End While

        While H > 360
            H -= 360
        End While

        H = H / 360
        Dim MAX As Byte = 255

        If S > 0 Then
            If H >= 1 Then H = 0
            H = 6 * H
            Dim hueFloor As Integer = CInt(Math.Floor(H))
            Dim a As Byte = CByte(Math.Round(MAX * V * (1.0 - S)))
            Dim b As Byte = CByte(Math.Round(MAX * V * (1.0 - (S * (H - hueFloor)))))
            Dim c As Byte = CByte(Math.Round(MAX * V * (1.0 - (S * (1.0 - (H - hueFloor))))))
            Dim d As Byte = CByte(Math.Round(MAX * V))

            Select Case hueFloor
                Case 0
                    _scolor = Color.FromArgb(alpha, d, c, a)
                Case 1
                    _scolor = Color.FromArgb(alpha, b, d, a)
                Case 2
                    _scolor = Color.FromArgb(alpha, a, d, c)
                Case 3
                    _scolor = Color.FromArgb(alpha, a, b, d)
                Case 4
                    _scolor = Color.FromArgb(alpha, c, a, d)
                Case 5
                    _scolor = Color.FromArgb(alpha, d, a, b)
                Case Else
                    _scolor = Color.FromArgb(0, 0, 0, 0)
            End Select
        Else
            Dim d As Byte = CByte((V * MAX))
            _scolor = Color.FromArgb(255, d, d, d)
        End If

    End Sub

    ''' <summary>
    ''' Valor hexadecimal desta cor
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Hexadecimal As String
        Get
            Return _scolor.ToHexadecimal()
        End Get
    End Property

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> aleatória
    ''' </summary>
    Sub New()
        Me.New(RandomColor())
    End Sub

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> a partir de seus valores de Matiz, Saturação, Brilho e Nome de cor
    ''' </summary>
    ''' <param name="H"></param>
    ''' <param name="S"></param>
    ''' <param name="V"></param>
    ''' <param name="Name"></param>
    Sub New(H As Double, S As Double, V As Double, Optional Name As String = Nothing, Optional Alpha As Integer = 255)
        _h = H
        _s = S
        _v = V
        SetColor()
        _name = Name
    End Sub

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> a partir dos valores RGB
    ''' </summary>
    ''' <param name="R">Vermelho</param>
    ''' <param name="G">Verde</param>
    ''' <param name="B">Azul</param>
    ''' <param name="A">Transparencia</param>
    ''' <param name="Name">Nome da cor</param>
    Sub New(R As Integer, G As Integer, B As Integer, Optional A As Integer = 255, Optional Name As String = Nothing)
        Me.New(Color.FromArgb(A, R, G, B))
        _name = Name
    End Sub

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> a partir de uma <see cref="System.Drawing.Color"/>
    ''' </summary>
    ''' <param name="Color">Cor do sistema</param>
    Sub New(Color As Color)
        _scolor = Color
        Me._name = _scolor.Name

        Dim r As Double = Color.R / 255
        Dim g As Double = Color.G / 255
        Dim b As Double = Color.B / 255

        Dim min As Double = Math.Min(Math.Min(r, g), b)
        Dim max As Double = Math.Max(Math.Max(r, g), b)
        _v = max
        Dim delta = max - min
        If max = 0 OrElse delta = 0 Then
            _s = 0
            _h = 0
        Else
            _s = delta / max
            If (r = max) Then
                'entre amarelo e magenta
                _h = (g - b) / delta
            ElseIf (g = max) Then
                'Entre ciano e amarelo
                _h = 2 + (b - r) / delta
            Else
                'entre magenta e ciano
                _h = 4 + (r - g) / delta
            End If

            _h *= 60
            If _h < 0 Then
                _h += 360
            End If
        End If
    End Sub

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> a partir de uma string de cor (colorname, hexadecimal ou string aleatoria) e um Nome
    ''' </summary>
    ''' <param name="Color">Cor</param>
    Sub New(Color As String)
        Me.New(Color.ToColor())
        _name = Color
    End Sub

    ''' <summary>
    ''' Instancia uma nova HSVColor a partir de uma string de cor (colorname, hexadecimal ou  string aleatoria) e um Nome
    ''' </summary>
    ''' <param name="Color">Cor</param>
    ''' <param name="Name">Nome da cor</param>
    Sub New(Color As String, Name As String)
        Me.New(Color.ToColor())
        _name = Name.IfBlank(Color)
    End Sub

    ''' <summary>
    ''' Instancia uma nova HSVColor a partir de uma <see cref="System.Drawing.Color"/> e um Nome
    ''' </summary>
    ''' <param name="Color">Cor</param>
    ''' <param name="Name">Nome da cor</param>
    Sub New(Color As Color, Name As String)
        Me.New(Color)
        _name = Name
    End Sub

    Private _name As String

    ''' <summary>
    ''' Nome atribuido a esta cor
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
        Get
            Return _name.IfBlank(ColorName)
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    ''' <summary>
    ''' Nome original desta cor
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ColorName As String
        Get
            Return _scolor.Name
        End Get
    End Property

    ''' <summary>
    ''' Descricao desta cor
    ''' </summary>
    ''' <returns></returns>
    Public Property Description As String

    ''' <summary>
    ''' Retorna uma <see cref="System.Drawing.Color"/> desta <see cref="HSVColor"/>
    ''' </summary>
    ''' <returns></returns>
    Public Function ToSystemColor() As Color
        Return _scolor
    End Function

    Private _scolor As Color

    ''' <summary>
    ''' Verifica se uma cor é legivel sobre outra cor
    ''' </summary>
    ''' <param name="BackgroundColor"></param>
    ''' <param name="Size"></param>
    ''' <returns></returns>
    Public Function IsReadable(BackgroundColor As HSVColor, Optional Size As Integer = 10) As Boolean
        If BackgroundColor IsNot Nothing Then
            Dim diff = BackgroundColor.R * 0.299 + BackgroundColor.G * 0.587 + BackgroundColor.B * 0.114 - Me.R * 0.299 - Me.G * 0.587 - Me.B * 0.114
            Return Not ((diff < (1.5 + 141.162 * Math.Pow(0.975, Size)))) AndAlso (diff > (-0.5 - 154.709 * Math.Pow(0.99, Size)))
        End If
        Return True
    End Function

    ''' <summary>
    ''' Retorna uma cópia desta cor
    ''' </summary>
    ''' <returns></returns>
    Public Function Clone() As HSVColor
        Return New HSVColor(Me._scolor, Me.Name)
    End Function

    ''' <summary>
    ''' Retorna a combinação de 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Combine(Color As HSVColor) As HSVColor
        If Color IsNot Nothing Then
            Return New HSVColor(Me.R Xor Color.R, Me.G Xor Color.G, Me.B Xor Color.B, Me.A)
        End If
        Return Me.Clone()
    End Function

    ''' <summary>
    ''' Retorna a cor media entre 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Average(Color As HSVColor) As HSVColor
        If Color IsNot Nothing Then
            Return New HSVColor(R:={Me.R, Color.R}.Average(), G:={Me.G, Color.G}.Average(), B:={Me.B, Color.B}.Average(), Me.A)
        End If
        Return Me.Clone()
    End Function

    ''' <summary>
    ''' Cria uma paleta de cores usando esta cor como base e um metodo especifico
    ''' </summary>
    ''' <param name="PalleteType"></param>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    Public Function CreatePallete(PalleteType As String, Optional Amount As Integer = 4) As HSVColor()
        Dim rl = New List(Of HSVColor)
        For Each item In Me.Monochromatic(Amount)
            Dim c = CType(item.GetType().GetMethod(PalleteType).Invoke(item, {False}), HSVColor())
            rl.AddRange(c)
        Next
        Return rl.ToArray()
    End Function

    ''' <summary>
    ''' Retorna  novas HSVColor a partir da cor atual, movendo ela N graus na roda de cores
    ''' </summary>
    ''' <param name="excludeMe">Inclui esta cor no array</param>
    ''' <param name="Degrees">Lista contendo os graus que serão movidos na roda de cores.</param>
    ''' <returns></returns>
    Public Function ModColor(ExcludeMe As Boolean, ParamArray Degrees As Integer()) As HSVColor()
        If Not ExcludeMe Then
            Return {Me}.ToArray().Union(ModColor(If(Degrees, {})).ToArray()).ToArray()
        End If
        Return ModColor(If(Degrees, {})).ToArray()
    End Function

    ''' <summary>
    ''' Retorna  novas HSVColor a partir da cor atual, movendo ela N graus na roda de cores
    ''' </summary>
    ''' <param name="Degrees">Lista contendo os graus que serão movidos na roda de cores.</param>
    ''' <returns></returns>
    Public Function ModColor(ParamArray Degrees As Integer()) As HSVColor()
        Return If(Degrees, {}).Select(Function(x) New HSVColor((Me.H + x) Mod 360, Me.S, Me.V)).OrderBy(Function(x) x.H).ToArray()
    End Function

    Public Overrides Function ToString() As String
        Return Me.Name
    End Function

    ''' <summary>
    ''' Retorna as cores Quadraadas (tetradicas) desta cor
    ''' </summary>
    ''' <param name="ExcludeMe"></param>
    ''' <returns></returns>
    Public Function Tetradic(Optional ExcludeMe As Boolean = False) As HSVColor()
        Return Square(ExcludeMe)
    End Function

    ''' <summary>
    ''' Retorna as cores análogas desta cor
    ''' </summary>
    ''' <param name="ExcludeMe"></param>
    ''' <returns></returns>
    Public Function Analogous(Optional ExcludeMe As Boolean = False) As HSVColor()
        Return ModColor(ExcludeMe, 45, -45)
    End Function

    ''' <summary>
    ''' Retorna as cores Quadraadas (tetradicas) desta cor
    ''' </summary>
    ''' <param name="ExcludeMe"></param>
    ''' <returns></returns>
    Public Function Square(Optional ExcludeMe As Boolean = False) As HSVColor()
        Return ModColor(ExcludeMe, 90, 180, 260)
    End Function

    ''' <summary>
    ''' Retorna as cores triadicas desta cor
    ''' </summary>
    ''' <param name="ExcludeMe"></param>
    ''' <returns></returns>
    Public Function Triadic(Optional ExcludeMe As Boolean = False) As HSVColor()
        Return ModColor(ExcludeMe, 120, -120)
    End Function

    ''' <summary>
    ''' Retorna as cores complementares desta cor
    ''' </summary>
    ''' <param name="ExcludeMe"></param>
    ''' <returns></returns>
    Public Function Complementary(Optional ExcludeMe As Boolean = False) As HSVColor()
        Return ModColor(ExcludeMe, 180)
    End Function

    ''' <summary>
    '''  Retorna as cores split-complementares desta cor
    ''' </summary>
    ''' <param name="IncludeMe"></param>
    ''' <returns></returns>
    Public Function SplitComplementary(Optional IncludeMe As Boolean = False) As HSVColor()
        Return ModColor(IncludeMe, 150, 210)
    End Function

    ''' <summary>
    ''' Retorna <paramref name="Amount"/> variacoes cores a partir da cor atual
    ''' </summary>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    Public Function Monochromatic(Optional Amount As Decimal = 4) As HSVColor()
        Return MonochromaticPallete(ToSystemColor, Amount).Select(Function(x) New HSVColor(x)).ToArray()
    End Function

    ''' <summary>
    ''' Retorna uma paleta de cores tetradica (Monochromatica + Tetradica)
    ''' </summary>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    Public Function TetradicPallete(Optional Amount As Integer = 3) As HSVColor()
        Return Me.Monochromatic(Amount).SelectMany(Function(item) item.Tetradic()).ToArray()
    End Function

    ''' <summary>
    ''' Retorna uma paleta de cores triadica (Monochromatica + Triadica)
    ''' </summary>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    Public Function TriadicPallete(Optional Amount As Integer = 3) As HSVColor()
        Return Me.Monochromatic(Amount).SelectMany(Function(item) item.Triadic()).ToArray()

    End Function

    ''' <summary>
    ''' Retorna uma paleta de cores complementares (complementares + monocromatica)
    ''' </summary>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    Public Function ComplementaryPallete(Optional Amount As Integer = 3) As HSVColor()
        Return Me.Monochromatic(Amount).SelectMany(Function(item) item.Complementary()).ToArray()
    End Function

    ''' <summary>
    ''' Retorna uma paleta de cores split-complementares (split-complementares + monocromatica)
    ''' </summary>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    Public Function SplitComplementaryPallete(Optional Amount As Integer = 3) As HSVColor()
        Return Me.Monochromatic(Amount).SelectMany(Function(item) item.SplitComplementary()).ToArray()
    End Function

End Class