Imports System.Runtime.CompilerServices
Imports System.Text

''' <summary>
''' Modulo para manipulação de numeros romanos
''' </summary>
''' <remarks></remarks>
Public Module Romanize
    ''' <summary>
    ''' Lista de algarismos romanos
    ''' </summary>

    Public Enum RomanDigit
        ''' <summary>
        ''' Valor correspondente
        ''' </summary>

        I = 1
        ''' <summary>
        ''' Valor correspondente
        ''' </summary>

        V = 5
        ''' <summary>
        ''' Valor correspondente
        ''' </summary>

        X = 10
        ''' <summary>
        ''' Valor correspondente
        ''' </summary>

        L = 50
        ''' <summary>
        ''' Valor correspondente
        ''' </summary>

        C = 100
        ''' <summary>
        ''' Valor correspondente
        ''' </summary>

        D = 500
        ''' <summary>
        ''' Valor correspondente
        ''' </summary>

        M = 1000
    End Enum

    ''' <summary>
    ''' Converte uma String contendo um numero romano para seu valor arabico
    ''' </summary>
    ''' <param name="RomanNumber">Stringo contendo o numero romano</param>
    ''' <returns>Valor em arabico</returns>

    <Extension()>
    Public Function ToArabic(ByVal RomanNumber As String) As Integer

        RomanNumber = RomanNumber.ToUpper().Trim()
        If RomanNumber = "N" Then
            Return 0
        End If

        ' Os numerais que representam números que começam com um '5'(V, L e D) podem
        ' aparecer apenas uma vez em cada numeral romano. Esta regra permite XVI, mas não VIV.
        If RomanNumber.Split("V"c).Length > 2 OrElse RomanNumber.Split("L"c).Length > 2 OrElse RomanNumber.Split("D"c).Length > 2 Then
            Throw New ArgumentException("Número romano com algarismos inválidos. O número possui um algarismo V,L ou D repetido.")
        End If

        ' Uma única letra pode ser repetida até três vezes consecutivamente sendo
        ' que cada ocorrência será somanda. Isto significa que I é um, II e III
        ' significa dois é três. No entanto, IIII não é permitido.
        Dim contador As Integer = 1
        Dim ultimo As Char = "Z"c
        For Each numeral As Char In RomanNumber
            ' caractere inválido ?
            If "IVXLCDM".IndexOf(numeral) = -1 Then
                Throw New ArgumentException("O algarismo com posicionamento inválido.")
            End If

            ' Duplicado?
            If numeral = ultimo Then
                contador += 1
                If contador = 4 Then
                    Throw New ArgumentException("Um algarismo romano não pode ser repetido mais de 3 vezes no mesmo número.")
                End If
            Else
                contador = 1
                ultimo = numeral
            End If
        Next

        ' Cria um ArrayList contendo os valores
        Dim ptr As Integer = 0
        Dim valores As New ArrayList()
        Dim digitoMaximo As Integer = 1000
        While ptr < RomanNumber.Length
            ' valor base do digito
            Dim numeral As Char = RomanNumber(ptr)
            Dim digito As Integer = CInt([Enum].Parse(GetType(RomanDigit), numeral.ToString()))

            ' Um numeral de pequena valor pode ser colocado à esquerda de um valor maior
            ' Quando isto ocorre, por exemplo IX, o menor número é subtraído do maior
            ' O dígito subtraído deve ser de pelo menos um décimo do valor do maior numeral e deve ser ou I, X ou C
            ' Valores como MCMD ou CMC não são permitidos
            If digito > digitoMaximo Then
                Throw New ArgumentException("Algarísmo com posicionamento inválido.")
            End If

            ' proximo digito
            Dim proximoDigito As Integer = 0
            If ptr < RomanNumber.Length - 1 Then
                Dim proximoNumeral As Char = RomanNumber(ptr + 1)
                proximoDigito = CInt([Enum].Parse(GetType(RomanDigit), proximoNumeral.ToString()))

                If proximoDigito > digito Then
                    If "IXC".IndexOf(numeral) = -1 OrElse proximoDigito > (digito * 10) OrElse RomanNumber.Split(numeral).Length > 3 Then
                        Throw New ArgumentException("Rule 3")
                    End If

                    digitoMaximo = digito - 1
                    digito = proximoDigito - digito
                    ptr += 1
                End If
            End If

            valores.Add(digito)

            ' proximo digito
            ptr += 1
        End While

        ' Outra regra é a que compara o tamanho do valor de cada numeral lido a partir da esquerda para a direita.
        ' O valor nunca deve aumentar a partir de uma letra para a próxima.
        ' Onde houver um numeral subtractivo, esta regra se aplica ao valor
        ' combinado dos dois algarismos envolvidos na subtração quando comparado com a letra anterior.
        ' Isto significa que XIX é aceitável, mas XIM e IIV não são.
        For i As Integer = 0 To valores.Count - 2
            If CInt(valores(i)) < CInt(valores(i + 1)) Then
                Throw New ArgumentException("Algarismo romano inválido. Neste caso o algarismo não pode ser maior que o anterior.")
            End If
        Next

        ' Numerais maiores devem ser colocados à esquerda dos números menores para
        ' continuar a combinação aditiva. Assim VI é igual a seis e MDCLXI é 1.661.
        Dim total As Integer = 0
        For Each digito As Integer In valores
            total += digito
        Next
        Return total

    End Function

    ''' <summary>
    ''' Converte um valor numérico arabico para numero romano
    ''' </summary>
    ''' <param name="ArabicNumber">Valor numerico arabico</param>
    ''' <returns>uma string com o numero romano</returns>

    <Extension()>
    Public Function ToRoman(ByVal ArabicNumber As Integer) As String
        ' valida : aceita somente valores entre 1 e 3999
        If ArabicNumber < 1 OrElse ArabicNumber > 3999 Then
            ArabicNumber = ArabicNumber.LimitRange(1, 3999)
            Debug.Write("O valor numérico deve estar entre 1 e 3999.", "ArabicNumber")
        End If

        Dim algarismosArabicos As Integer() = New Integer() {1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1}
        Dim algarismosRomanos As String() = New String() {"M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I"}

        ' inicializa o string builder
        Dim resultado = ""

        ' percorre os valores nos arrays
        For i As Integer = 0 To 12
            ' se o numero a ser convertido é menor que o valor então anexa
            ' o numero correspondente ou o par ao resultado
            While ArabicNumber >= algarismosArabicos(i)
                ArabicNumber -= algarismosArabicos(i)
                resultado &= (algarismosRomanos(i))
            End While
        Next

        ' retorna o resultado
        Return resultado.ToString()

    End Function

End Module