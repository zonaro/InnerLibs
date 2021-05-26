Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Security.Cryptography
Imports System.Text
''' <summary>
''' Modulo de Conversão de Cores
''' </summary>
''' <remarks></remarks>
Public Module ColorConvert


    ''' <summary>
    ''' Gera uma cor de acordo com um texto
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function GenerateColor(Text As String) As Color
        Dim ba = Encoding.Default.GetBytes(Text)
        Dim hash = BitConverter.ToString(ba).RemoveAny("-")
        Return hash.GetMiddleChars(6).ToColor()
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
    Public Function MergeWith(TheColor As Color, AnotherColor As Color, Optional Percent As Single = 0.6) As Color
        Return TheColor.Lerp(AnotherColor, Percent)
    End Function
    ''' <summary>
    ''' Escurece a cor mesclando ela com preto
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <param name="percent">porcentagem de mesclagem</param>
    ''' <returns></returns>
    <Extension>
    Public Function MakeDarker(TheColor As Color, Optional percent As Single = 0.6) As Color
        Return TheColor.MergeWith(Color.Black, percent)
    End Function

    ''' <summary>
    ''' Clareia a cor mistuando ela com branco
    ''' </summary>
    ''' <param name="TheColor">Cor</param>
    ''' <param name="percent">Porcentagem de mesclagem</param>
    ''' <returns></returns>
    <Extension>
    Public Function MakeLighter(TheColor As Color, Optional percent As Single = 0.6) As Color
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

    ''' <summary>
    ''' Converte uma string hexadecimal (HTML) para objeto Color
    ''' </summary>
    ''' <param name="HexadecimalColorString">String Hexadecimal</param>
    ''' <returns>Um objeto color</returns>
    <Extension()>
    Public Function ToColor(HexadecimalColorString As String) As Color
        Return ColorTranslator.FromHtml("#" & HexadecimalColorString.RemoveAny("#").IfBlank("000000"))
    End Function

End Module