Imports System.Drawing
Imports System.Linq.Expressions
Imports InnerLibs.LINQ

Public Class HSVColor
    Implements IComparable(Of Integer)
    Implements IComparable(Of HSVColor)
    Implements IComparable(Of System.Drawing.Color)
    Implements IComparable
    Private _h, _s, _v As Double
    Private _name As String
    Private _scolor As Color


    ''' <summary>
    ''' Retorna a cor vibrante de uma imagem
    ''' </summary>
    ''' <param name="Img"></param>
    ''' <param name="Reduce"></param>
    ''' <returns></returns>
    Public Shared Function FromImage(Img As Image, Optional Reduce As Integer = 16) As HSVColor
        Return ColorPallette(Img, Reduce).Keys.FirstOr(New HSVColor())
    End Function

    ''' <summary>
    ''' Retorna uma cor aleatoria a partir da paleta de cores de uma imagem
    ''' </summary>
    ''' <param name="Img"></param>
    ''' <param name="Reduce"></param>
    ''' <returns></returns>
    Public Shared Function RandomColor(Img As Image, Optional Reduce As Integer = 16) As HSVColor
        Return Img.ColorPallette(Reduce).Keys.FirstRandom()
    End Function

    ''' <summary>
    ''' Retorna uma cor aleatória a partir de uma lista de cores
    ''' </summary>
    ''' <param name="Colors"></param>
    ''' <returns></returns>
    Public Shared Function RandomColor(Colors As IEnumerable(Of Color)) As HSVColor
        Return New HSVColor(Colors?.OrderByRandom().FirstOr(Color.Transparent))
    End Function

    ''' <summary>
    ''' Gera uma <see cref="HSVColor"/> opaca aleatoria
    ''' </summary>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    Public Shared Function RandomColor(Optional Name As String = Nothing) As HSVColor
        Return New HSVColor(ColorExtensions.RandomColor(), Name)
    End Function

    ''' <summary>
    ''' Gera uma <see cref="HSVColor"/> opaca aleatoria dentro de um Mood especifico
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function RandomColor(Mood As ColorMood) As HSVColor
        Return RandomColorList(1, Mood).FirstOrDefault()
    End Function

    ''' <summary>
    ''' Gera uma <see cref="HSVColor"/> opaca aleatoria dentro de uma especificacao
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function RandomColor(predicate As Expression(Of Func(Of HSVColor, Boolean))) As HSVColor
        Return RandomColorList(1, predicate).FirstOrDefault()
    End Function

    ''' <summary>
    ''' Gera uma lista com <see cref="HSVColor"/>   aleatorias
    ''' </summary>
    ''' <param name="Quantity"></param>
    ''' <returns></returns>
    Public Shared Function RandomColorList(Quantity As Integer, Mood As ColorMood) As IEnumerable(Of HSVColor)
        Dim l As New List(Of HSVColor)
        While l.Count < Quantity
            Dim c As HSVColor
            Do
                c = HSVColor.RandomColor()
            Loop While Not c.Mood.HasFlag(Mood)
            If Not l.Any(Function(x) x.ARGB = c.ARGB) Then
                l.Add(c)
            End If
        End While
        Return l
    End Function

    ''' <summary>
    ''' Gera uma lista com <see cref="HSVColor"/>   aleatorias
    ''' </summary>
    ''' <param name="Quantity"></param>
    ''' <returns></returns>
    Public Shared Function RandomColorList(Quantity As Integer, predicate As Expression(Of Func(Of HSVColor, Boolean))) As IEnumerable(Of HSVColor)
        Dim l As New List(Of HSVColor)
        While l.Count < Quantity
            Dim c As HSVColor = Nothing
            Do
                c = {HSVColor.RandomColor()}.FirstOrDefault(predicate.Compile())
            Loop While c Is Nothing
            If Not l.Any(Function(x) x.ARGB = c.ARGB) Then
                l.Add(c)
            End If
        End While
        Return l
    End Function

    ''' <summary>
    ''' Gera uma <see cref="HSVColor"/>  aleatoria com transparencia
    ''' </summary>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    Public Shared Function RandomTransparentColor(Optional Name As String = Nothing) As HSVColor
        Return RandomColor().With(Sub(x) x.Opacity = Generate.RandomNumber(0, 100)).With(Sub(x) If Name IsNot Nothing Then x.Name = Name)
    End Function


    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> a aprtir de seu ARGB
    ''' </summary>
    Sub New(ARGB As Integer)
        Me.New(Color.FromArgb(ARGB))
    End Sub


    Sub New(R As Integer, G As Integer, B As Integer)
        Me.New(255, R, G, B)
    End Sub

    Sub New(A As Integer, R As Integer, G As Integer, B As Integer)
        Me.New(Color.FromArgb(A, R, G, B))
    End Sub

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> transparente
    ''' </summary>
    Sub New()
        Me.New(Color.Transparent)
    End Sub

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> a partir de uma <see cref="System.Drawing.Color"/>
    ''' </summary>
    ''' <param name="Color">Cor do sistema</param>
    Sub New(Color As Color)
        _loadColor(Color)
    End Sub

    ''' <summary>
    ''' Instancia uma nova <see cref="HSVColor"/> a partir de uma string de cor (colorname, hexadecimal ou string aleatoria)
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

    ''' <summary>
    ''' Retorna ou seta o valor ARGB de 32 bits dessa cor
    ''' </summary>
    ''' <returns></returns>
    Public Property ARGB As Integer
        Get
            Return _scolor.ToArgb()
        End Get
        Set(value As Integer)
            _scolor = Color.FromArgb(value)
            _loadColor(_scolor)
        End Set
    End Property

    ''' <summary>
    ''' Hue (Matiz)
    ''' </summary>
    ''' <returns></returns>
    Property Hue As Double
        Get
            Return _h
        End Get
        Set(value As Double)
            If _h <> value Then
                _h = value

                While _h < 0
                    _h += 360
                End While

                While _h > 360
                    _h -= 360
                End While

                SetColor()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Saturation (Saturação)
    ''' </summary>
    ''' <returns></returns>
    Property Saturation As Double
        Get
            Return _s
        End Get
        Set(value As Double)
            value = value.LimitRange(0.0, 1.0)
            If _s <> value Then
                _s = value
                SetColor()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Luminância
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Luminance As Double
        Get
            Return (0.2126 * Red) + (0.7152 * Green) + (0.0722 * Blue)
        End Get
    End Property

    ''' <summary>
    ''' Brilho
    ''' </summary>
    ''' <returns></returns>
    Property Brightness As Double
        Get
            Return _v
        End Get
        Set(value As Double)
            value = value.LimitRange(0.0, 1.0)
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
    Property Red As Integer
        Get
            Return _scolor.R
        End Get
        Set(value As Integer)
            _scolor = Color.FromArgb(Alpha, value.LimitRange(Of Integer)(0, 255), Green, Blue)
            _loadColor(_scolor)
        End Set
    End Property

    ''' <summary>
    ''' Green (Verde)
    ''' </summary>
    ''' <returns></returns>
    Property Green As Integer
        Get
            Return _scolor.G
        End Get
        Set(value As Integer)
            _scolor = Color.FromArgb(Alpha, Red, value.LimitRange(Of Integer)(0, 255), Blue)
            _loadColor(_scolor)
        End Set
    End Property

    ''' <summary>
    ''' Blue (Azul)
    ''' </summary>
    ''' <returns></returns>
    Property Blue As Integer
        Get
            Return _scolor.B
        End Get
        Set(value As Integer)
            _scolor = Color.FromArgb(Alpha, Red, Green, value.LimitRange(Of Integer)(0, 255))
            _loadColor(_scolor)
        End Set
    End Property

    ''' <summary>
    ''' Alpha (Transparencia)
    ''' </summary>
    ''' <returns></returns>
    Property Alpha As Byte
        Get
            Return _scolor.A
        End Get
        Set(value As Byte)
            _scolor = Color.FromArgb(value.LimitRange(Of Byte)(0, 255), Red, Green, Blue)
            _loadColor(_scolor)
        End Set
    End Property

    ''' <summary>
    ''' Opacidade (de 1 a 100%)
    ''' </summary>
    ''' <returns></returns>
    Property Opacity As Decimal
        Get
            Return Alpha.ToDecimal().CalculatePercent(255)
        End Get
        Set(value As Decimal)
            Alpha = Decimal.ToByte(CalculateValueFromPercent(value.LimitRange(0, 100), 255).LimitRange(0, 255))
        End Set
    End Property

    ''' <summary>
    ''' Valor hexadecimal desta cor
    ''' </summary>
    ''' <returns></returns>
    Public Property Hexadecimal As String
        Get
            Return _scolor.ToHexadecimal()
        End Get
        Set(value As String)
            If value.IsHexaDecimalColor Then
                _scolor = value.ToColor()
                _loadColor(_scolor)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Valor RGBA() desta cor
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CSS As String
        Get
            If Me.Alpha = 255 Then Return _scolor.ToCssRGB() Else Return _scolor.ToCssRGBA()
        End Get
    End Property

    ''' <summary>
    ''' Mood da cor
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Mood As ColorMood
        Get

            Dim m As ColorMood

            If IsDark() Then
                m = ColorMood.Dark
            ElseIf IsLight() Then
                m = ColorMood.Light
            Else
                m = ColorMood.Medium
            End If

            If IsMediumDark() Then
                m = m Or ColorMood.MediumDark
            Else
                m = m Or ColorMood.MediumLight
            End If

            If Me.IsWarmer Then
                m = m Or ColorMood.Warmer
            End If

            If IsWarm() Then
                m = m Or ColorMood.Warm
            End If

            If IsCool() Then
                m = m Or ColorMood.Cool
            End If

            If Me.IsCooler Then
                m = m Or ColorMood.Cooler
            End If

            If IsSad() Then
                m = m Or ColorMood.Sad
            Else
                m = m Or ColorMood.Happy
            End If

            If Opacity < 15 Then
                m = m Or ColorMood.Unvisible
            ElseIf Opacity < 60 Then
                m = m Or ColorMood.SemiVisible
            Else
                m = m Or ColorMood.Visible
            End If

            If Luminance >= 250 Then
                m = m Or ColorMood.LowLuminance
            ElseIf Luminance <= 15 Then
                m = m Or ColorMood.HighLuminance
            End If

            If Red = 255 Then
                m = m Or ColorMood.Red
            End If

            If Green = 255 Then
                m = m Or ColorMood.Green
            End If

            If Blue = 255 Then
                m = m Or ColorMood.Blue
            End If

            If Red = 0 Then
                m = m Or ColorMood.NoRed
            End If

            If Green = 0 Then
                m = m Or ColorMood.NoGreen
            End If

            If Blue = 0 Then
                m = m Or ColorMood.NoBlue
            End If

            If Red > Blue AndAlso Red > Green Then
                m = m Or ColorMood.MostRed
            End If

            If Green > Red AndAlso Green > Blue Then
                m = m Or ColorMood.MostGreen
            End If

            If Blue > Red AndAlso Blue > Green Then
                m = m Or ColorMood.MostBlue
            End If

            If _scolor.IsKnownColor Then
                m = m Or ColorMood.KnowColor
            End If

            Return m
        End Get
    End Property

    Public Function HasMood(ParamArray Mood As ColorMood()) As Boolean
        Return Mood?.All(Function(x) Me.Mood.HasFlag(x))
    End Function

    Public Function HasAnyMood(ParamArray Mood As ColorMood()) As Boolean
        Return Mood?.Any(Function(x) Me.Mood.HasFlag(x))
    End Function

    Public Function NotHasMood(ParamArray Mood As ColorMood()) As Boolean
        Return Mood?.All(Function(x) Me.Mood.HasFlag(x) = False)
    End Function

    Public ReadOnly Property DominantValue
        Get
            Return {Red, Green, Blue}.Max()
        End Get
    End Property

    Public Function GetDominantColor() As HSVColor
        If Mood.HasFlag(ColorMood.MostRed) Then
            Return Me.RedPart
        End If
        If Mood.HasFlag(ColorMood.MostGreen) Then
            Return Me.GreenPart
        End If
        If Mood.HasFlag(ColorMood.MostBlue) Then
            Return Me.BluePart
        End If
        Return Me
    End Function

    Public Function GetEuclideanDistance(Other As HSVColor) As Double
        Return _scolor.EuclideanDistance(Other._scolor)
    End Function

    Public Function CreateSolidImage(Width As Integer, Height As Integer) As Bitmap
        Return New Bitmap(_scolor.CreateSolidImage(Width, Height))
    End Function

    ''' <summary>
    ''' Retorna a cor intermediaria de um gradiente
    ''' </summary>
    ''' <param name="ToColor"></param>
    ''' <param name="Position"></param>
    ''' <param name="Size"></param>
    ''' <returns></returns>
    Public Function GradientLevel(ToColor As HSVColor, Position As Integer, Optional Size As Integer = 100) As HSVColor
        If Size = 0 Then Size = 1
        If Position > Size Then Size = Position
        Dim a = Me.Clone()
        a.Red = a.Red + ((ToColor.Red - a.Red) / Size) * Position
        a.Green = a.Green + ((ToColor.Green - a.Green) / Size) * Position
        a.Blue = a.Blue + ((ToColor.Blue - a.Blue) / Size) * Position
        Return a
    End Function


    Public Iterator Function GradientArray(ToColor As HSVColor, Size As Integer) As IEnumerable(Of HSVColor)
        For index = 1 To Size
            Yield GradientLevel(ToColor, index, Size)
        Next
    End Function



    Public Function WebRound() As HSVColor
        Dim c = Me.Clone
        If (c.Red = c.Red + 51 - c.Red Mod 51) > 255 Then c.Red = 255
        If (c.Green = c.Green + 51 - c.Green Mod 51) > 255 Then c.Green = 255
        If (c.Blue = c.Blue + 51 - c.Blue Mod 51) > 255 Then c.Blue = 255
        Return c
    End Function

    Public Function CreateSolidImage(Optional Size As String = "") As Bitmap
        Dim s = Size.IfBlank("200").ParseSize()
        Return CreateSolidImage(s.Width, s.Height)
    End Function

    Public ReadOnly Property ImageSample As Bitmap
        Get
            Return CreateSolidImage().DrawString(Name)
        End Get
    End Property

    ''' <summary>
    ''' Nome atribuido a esta cor
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
        Get
            Return _name.IfBlank(GetColorName(_scolor))
        End Get
        Set(value As String)
            _name = value
        End Set
    End Property

    ''' <summary>
    ''' Nome original mais proximo desta cor
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ClosestColorName As String
        Get
            Return _scolor.GetClosestColorName()
        End Get
    End Property



    Private Sub SetColor()

        Dim H, S, V As Double
        Dim alpha = _scolor.A

        H = Me.Hue
        S = Me.Saturation
        V = Me.Brightness

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
            _scolor = Color.FromArgb(alpha, d, d, d)
        End If

        _s = S.LimitRange(0R, 1.0R)
        _v = V.LimitRange(0R, 1.0R)

    End Sub

    Private Sub _loadColor(Color As Color)
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
    ''' Retorna uma <see cref="System.Drawing.Color"/> desta <see cref="HSVColor"/>
    ''' </summary>
    ''' <returns></returns>
    Public Function ToSystemColor() As Color
        Return Color.FromArgb(Alpha, Red, Green, Blue)
    End Function

    ''' <summary>
    ''' Verifica se uma cor é legivel sobre outra cor
    ''' </summary>
    ''' <param name="BackgroundColor"></param>
    ''' <param name="Size"></param>
    ''' <returns></returns>
    Public Function IsReadable(BackgroundColor As HSVColor, Optional Size As Integer = 10) As Boolean
        Return Me._scolor.IsReadable(BackgroundColor._scolor, Size)
    End Function

    ''' <summary>
    ''' Retorna uma cor mais clara a partir desta cor
    ''' </summary>
    ''' <param name="Percent"></param>
    ''' <returns></returns>
    Public Function MakeLighter(Optional Percent As Single = 50) As HSVColor
        Return New HSVColor(_scolor.MakeLighter(Percent))
    End Function

    ''' <summary>
    ''' Retorna uma cor mais escura a partir desta cor
    ''' </summary>
    ''' <param name="Percent"></param>
    ''' <returns></returns>
    Public Function MakeDarker(Optional Percent As Single = 50) As HSVColor
        Return New HSVColor(_scolor.MakeDarker(Percent))
    End Function

    ''' <summary>
    ''' Verifica se uma cor e considerada clara
    ''' </summary>
    ''' <returns></returns>
    Public Function IsLight() As Boolean
        Return Luminance.IsGreaterThan(160.0R)
    End Function

    ''' <summary>
    ''' Verifica se uma cor e considerada escura
    ''' </summary>
    ''' <returns></returns>
    Public Function IsDark() As Boolean
        Return Luminance.IsLessThan(70.0R)
    End Function

    ''' <summary>
    ''' Verifica se uma cor e considerada Medio Clara
    ''' </summary>
    ''' <returns></returns>
    Public Function IsMediumLight() As Boolean
        Return Luminance > 255 / 2
    End Function

    ''' <summary>
    ''' Verifica se uma cor e considerada Medio Escura
    ''' </summary>
    ''' <returns></returns>
    Public Function IsMediumDark() As Boolean
        Return Not IsMediumLight()
    End Function

    ''' <summary>
    ''' Verifica se uma cor e considerada média
    ''' </summary>
    ''' <returns></returns>
    Public Function IsMedium() As Boolean
        Return Luminance.IsBetweenOrEqual(70.0R, 160.0R)
    End Function

    Public Function IsWarm() As Boolean
        Return Hue.IsLessThan(90.0R) OrElse Hue.IsGreaterThan(270.0R)
    End Function

    Public Function IsWarmer() As Boolean
        Return Hue.IsLessThan(45.0R) OrElse Hue.IsGreaterThan(315.0R)
    End Function

    Public Function IsCool() As Boolean
        Return Not IsWarm()
    End Function

    Public Function IsCooler() As Boolean
        Return Hue.IsLessThan(225.0R) OrElse Hue.IsGreaterThan(135.0R)
    End Function

    Public Function IsSad() As Boolean
        Return Saturation.IsLessThan(0.5) OrElse Brightness.IsLessThan(0.75)
    End Function

    Public Function IssHappy() As Boolean
        Return Not IsSad()
    End Function

    Public Function IsOpaque() As Boolean
        Return Alpha > 125
    End Function


    Public Function Breed(Color As HSVColor) As HSVColor
        Dim RedGene = {Me.Red, Color.Red, Color.Combine(Me).Red}
        Dim BlueGene = {Me.Blue, Color.Blue, Color.Combine(Me).Blue}
        Dim GreenGene = {Me.Green, Color.Green, Color.Combine(Me).Green}
        Dim r = If(Me.HasMood(ColorMood.MostRed) AndAlso Me.DominantValue > Color.DominantValue, Me.Red, RedGene.FirstRandom())
        Dim g = If(Me.HasMood(ColorMood.MostGreen) AndAlso Me.DominantValue > Color.DominantValue, Me.Green, GreenGene.FirstRandom())
        Dim b = If(Me.HasMood(ColorMood.MostBlue) AndAlso Me.DominantValue > Color.DominantValue, Me.Blue, BlueGene.FirstRandom())
        Dim a = {Me.Alpha, Color.Alpha}.FirstRandom()
        Return New HSVColor(a, r, g, b)
    End Function

    Public Iterator Function Breed(Color As HSVColor, Amount As Integer) As IEnumerable(Of HSVColor)
        For index = 1 To Amount
            Yield Breed(Color)
        Next
    End Function


    ''' <summary>
    ''' Retorna uma cópia desta cor
    ''' </summary>
    ''' <returns></returns>
    Public Function Clone() As HSVColor
        Return New HSVColor(_scolor, Me.Name)
    End Function

    ''' <summary>
    ''' Retorna a combinação de 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Combine(Color As HSVColor) As HSVColor
        If Color IsNot Nothing Then
            Return New HSVColor() With {.Red = Me.Red Xor Color.Red, .Green = Me.Green Xor Color.Green, .Blue = Me.Blue Xor Color.Blue, .Alpha = Me.Alpha}
        End If
        Return Me.Clone()
    End Function

    ''' <summary>
    ''' Retorna a distancia entre 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Distance(Color As HSVColor) As Double
        Return Math.Sqrt(3 * (Color.Red - Me.Red) * (Color.Red - Me.Red) + 4 * (Color.Green - Me.Green) * (Color.Green - Me.Green) + 2 * (Color.Blue - Me.Blue) * (Color.Blue - Me.Blue))
    End Function

    ''' <summary>
    ''' Retorna uma nova cor a partir da mistura multiplicativa de 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Multiply(Color As HSVColor) As HSVColor
        Dim n = Me.Clone()
        If Color IsNot Nothing Then
            n.Red = (Me.Red / 255 * Color.Red).LimitRange(0, 255)
            n.Green = (Me.Green / 255 * Color.Green).LimitRange(0, 255)
            n.Blue = (Me.Blue / 255 * Color.Blue).LimitRange(0, 255)
        End If
        Return n
    End Function

    ''' <summary>
    ''' Retorna uma nova cor a partir da mistura subtrativa de 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Subtractive(Color As HSVColor) As HSVColor
        Dim n = Me.Clone()
        If Color IsNot Nothing Then
            n.Red = (n.Red + (Color.Red - 255)).LimitRange(0, 255)
            n.Green = (n.Green + (Color.Green - 255)).LimitRange(0, 255)
            n.Blue = (n.Blue + (Color.Blue - 255)).LimitRange(0, 255)
        End If
        Return n
    End Function

    ''' <summary>
    ''' Retorna uma nova cor a partir da mistura aditiva de 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Addictive(Color As HSVColor) As HSVColor
        Dim n = Me.Clone()
        If Color IsNot Nothing Then
            n.Red = (n.Red + Color.Red).LimitRange(0, 255)
            n.Green = (n.Green + Color.Green).LimitRange(0, 255)
            n.Blue = (n.Blue + Color.Blue).LimitRange(0, 255)
        End If
        Return n
    End Function

    ''' <summary>
    ''' Retorna uma nova cor a partir da diferença de 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Difference(Color As HSVColor) As HSVColor
        Dim n = Me.Clone()
        If Color IsNot Nothing Then
            n.Red = (n.Red - Color.Red).LimitRange(0, 255)
            n.Green = (n.Green - Color.Green).LimitRange(0, 255)
            n.Blue = (n.Blue - Color.Blue).LimitRange(0, 255)
        End If
        Return n
    End Function

    ''' <summary>
    ''' Retorna a cor media entre 2 cores
    ''' </summary>
    ''' <param name="Color"></param>
    ''' <returns></returns>
    Public Function Average(Color As HSVColor) As HSVColor
        If Color IsNot Nothing Then
            Return New HSVColor() With {.Red = {Me.Red, Color.Red}.Average(), .Green = {Me.Green, Color.Green}.Average(), .Blue = {Me.Blue, Color.Blue}.Average(), .Alpha = Me.Alpha}
        End If
        Return Me.Clone()
    End Function

    ''' <summary>
    ''' Extrai os tons marrons de uma cor (filtro sépia)
    ''' </summary>
    ''' <returns></returns>
    Public Function Sepia() As HSVColor
        Dim c = Me.Clone()
        c.Red = Math.Round(Red * 0.393 + Green * 0.769 + Blue * 0.189)
        c.Green = Math.Round(Red * 0.349 + Green * 0.686 + Blue * 0.168)
        c.Blue = Math.Round(Red * 0.272 + Green * 0.534 + Blue * 0.131)
        Return c
    End Function

    ''' <summary>
    ''' Retorna a cor contrastante desta HSVColor
    ''' </summary>
    ''' <returns></returns>
    Public Function ContrastColor() As HSVColor
        Return New HSVColor(_scolor.GetContrastColor)
    End Function

    ''' <summary>
    ''' Extrai a cor negativa desta cor
    ''' </summary>
    ''' <returns></returns>
    Public Function Negative() As HSVColor
        Return New HSVColor(_scolor.GetNegativeColor())
    End Function

    ''' <summary>
    ''' Extrai o cinza desta cor
    ''' </summary>
    ''' <returns></returns>
    Public Function GrayFilter() As HSVColor
        Dim v = 0.35 + 13 * (Red + Green + Blue) / 60
        Return New HSVColor(Drawing.Color.FromArgb(v, v, v))
    End Function

    Public Function RedPart() As HSVColor
        Return New HSVColor(Color.FromArgb(Red, 0, 0))
    End Function

    Public Function GreenPart() As HSVColor
        Return New HSVColor(Color.FromArgb(0, Green, 0))
    End Function

    Public Function BluePart() As HSVColor
        Return New HSVColor(Color.FromArgb(0, 0, Blue))
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
        Return If(Degrees, {}).Select(Function(x) New HSVColor() With {.Hue = ((Me.Hue + x) Mod 360), .Saturation = Me.Saturation, .Brightness = Me.Brightness}).OrderBy(Function(x) x.Hue).ToArray()
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
        Return MonochromaticPallete(_scolor, Amount).ToArray()
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

    Public Function CompareTo(other As Integer) As Integer Implements IComparable(Of Integer).CompareTo
        Return Me.ARGB.CompareTo(other)
    End Function

    Public Function CompareTo(other As HSVColor) As Integer Implements IComparable(Of HSVColor).CompareTo
        Return Me.ARGB.CompareTo(other?.ARGB)
    End Function

    Public Function CompareTo(other As Color) As Integer Implements IComparable(Of Color).CompareTo
        Return Me.CompareTo(New HSVColor(other))
    End Function

    Public Function CompareTo(obj As Object) As Integer Implements IComparable.CompareTo
        Return Me.CompareTo(New HSVColor(obj?.ToString()))
    End Function

    Public Shared Operator +(Color1 As HSVColor, Color2 As HSVColor) As HSVColor
        Return Color1.Combine(Color2)
    End Operator

    Public Shared Operator +(Color1 As Color, Color2 As HSVColor) As HSVColor
        Return New HSVColor(Color1).Combine(Color2)
    End Operator

    Public Shared Operator +(Color1 As HSVColor, Color2 As Color) As HSVColor
        Return New HSVColor(Color2).Combine(Color1)
    End Operator

    Public Shared Operator Mod(Color As HSVColor, Degrees As Integer) As HSVColor
        Return Color.ModColor(True, Degrees).FirstOrDefault
    End Operator

    Public Shared Operator >(Color1 As HSVColor, Color2 As HSVColor) As Boolean
        Return Color1.CompareTo(Color2) > 0
    End Operator

    Public Shared Operator <(Color1 As HSVColor, Color2 As HSVColor) As Boolean
        Return Color1.CompareTo(Color2) < 0
    End Operator

    Public Shared Operator >=(Color1 As HSVColor, Color2 As HSVColor) As Boolean
        Return Color1.CompareTo(Color2) >= 0
    End Operator

    Public Shared Operator <=(Color1 As HSVColor, Color2 As HSVColor) As Boolean
        Return Color1.CompareTo(Color2) <= 0
    End Operator

    Public Shared Operator =(Color1 As HSVColor, Color2 As HSVColor) As Boolean
        Return Color1.CompareTo(Color2) = 0
    End Operator

    Public Shared Operator <>(Color1 As HSVColor, Color2 As HSVColor) As Boolean
        Return Color1.CompareTo(Color2) <> 0
    End Operator

    Public Shared Operator -(Color1 As HSVColor, Color2 As HSVColor) As HSVColor
        Return Color1.Difference(Color2)
    End Operator

    Public Shared Operator -(Color1 As Color, Color2 As HSVColor) As HSVColor
        Return New HSVColor(Color1).Difference(Color2)
    End Operator

    Public Shared Operator -(Color1 As HSVColor, Color2 As Color) As HSVColor
        Return New HSVColor(Color2).Difference(Color1)
    End Operator

    Public Shared Operator *(Color1 As HSVColor, Color2 As HSVColor) As HSVColor
        Return Color1.Multiply(Color2)
    End Operator

    Public Shared Operator *(Color1 As Color, Color2 As HSVColor) As HSVColor
        Return New HSVColor(Color1).Multiply(Color2)
    End Operator

    Public Shared Operator *(Color1 As HSVColor, Color2 As Color) As HSVColor
        Return New HSVColor(Color2).Multiply(Color1)
    End Operator

    Public Shared Widening Operator CType(Color As HSVColor) As Integer
        Return Color.ARGB
    End Operator

    Public Shared Widening Operator CType(Value As Integer) As HSVColor
        Return New HSVColor(Drawing.Color.FromArgb(Value))
    End Operator

    Public Shared Widening Operator CType(Value As Drawing.Color) As HSVColor
        Return New HSVColor(Value)
    End Operator

    Public Shared Widening Operator CType(Value As HSVColor) As Drawing.Color
        Return Value.ToSystemColor
    End Operator

    Public Shared Widening Operator CType(Value As String) As HSVColor
        Return New HSVColor(Value)
    End Operator

    Public Shared Widening Operator CType(Value As HSVColor) As String
        Return Value.Hexadecimal
    End Operator

End Class

<Flags>
Public Enum ColorMood

    Dark = 1
    MediumDark = 2
    Medium = 4
    MediumLight = 8
    Light = 16

    Sad = 32
    Happy = 64
    Love = ColorMood.MostRed Or ColorMood.NoGreen Or ColorMood.Happy
    Nature = ColorMood.MostGreen Or ColorMood.Happy
    Water = Not ColorMood.Red Or ColorMood.Medium

    Cooler = 128
    Cool = 256
    Warm = 512
    Warmer = 1024
    Ice = Blue Or NoRed Or Cooler
    Fire = Red Or NoBlue Or Warmer

    Unvisible = 2048
    SemiVisible = 4096
    Visible = 8192

    LowLuminance = 16384
    HighLuminance = 32768

    Red = 65536
    Green = 131072
    Blue = 262144

    MostRed = 524288
    MostGreen = 1048576
    MostBlue = 2097152

    NoRed = 4194304
    NoGreen = 8388608
    NoBlue = 16777216

    FullRed = ColorMood.NoGreen Or ColorMood.NoBlue
    FullGreen = ColorMood.NoRed Or ColorMood.NoBlue
    FullBlue = ColorMood.NoRed Or ColorMood.NoGreen

    KnowColor = 33554432

End Enum

Public Module HSVColorPallete

    'Public ReadOnly Property Pokemon As IEnumerable(Of HSVColor)
    '    Get
    '        Return {New HSVColor("#f85801", "FireRed"), New HSVColor("#f85801", "LeafGreen"), New HSVColor("#f85801", "FireRed")}
    '    End Get
    'End Property

End Module