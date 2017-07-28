Imports System.Runtime.CompilerServices

''' <summary>
''' Módulo para calculos
''' </summary>
''' <remarks></remarks>
Public Module Mathematic

    ''' <summary>
    ''' Executa uma Expressão matematica simples
    ''' </summary>
    ''' <param name="Formula">Expressão matematica</param>
    ''' <returns></returns>
    Public Function EvaluateExpression(Formula As String) As Double
        Dim SC As New MSScriptControl.ScriptControl
        SC.Language = "VBSCRIPT"
        Try
            Return Convert.ToDouble(SC.Eval(Formula))
        Catch ex As Exception
            Throw New Exception("Formula invalida")
        End Try
    End Function

    ''' <summary>
    ''' Retorna uma progressão Aritmética com N numeros
    ''' </summary>
    ''' <param name="FirstNumber"></param>
    ''' <param name="[Constant]"> </param>
    ''' <param name="Length">     </param>
    ''' <returns></returns>
    Public Function ArithmeticProgression(FirstNumber As Integer, [Constant] As Integer, Length As Integer) As List(Of Integer)
        Dim PA As New List(Of Integer)
        PA.Add(FirstNumber)
        For index = 1 To Length - 1
            PA.Add(PA(index - 1) + [Constant])
        Next
        Return PA
    End Function

    ''' <summary>
    ''' Retorna uma Progressão Gemoétrica com N numeros
    ''' </summary>
    ''' <param name="FirstNumber"></param>
    ''' <param name="[Constant]"> </param>
    ''' <param name="Length">     </param>
    ''' <returns></returns>
    Public Function GeometricProgression(FirstNumber As Integer, [Constant] As Integer, Length As Integer) As List(Of Integer)
        Dim PG As New List(Of Integer)
        PG.Add(FirstNumber)
        For index = 1 To Length - 1
            PG.Add(PG(index - 1) * [Constant])
        Next
        Return PG
    End Function

    ''' <summary>
    ''' Retorna todas as possiveis combinações de Arrays do mesmo tipo (Produto Cartesiano)
    ''' </summary>
    ''' <param name="Sets">Lista de Arrays para combinar</param>
    ''' <returns>Plano Cartesiano</returns>
    Public Function CartesianProduct(Of T)(ByVal ParamArray Sets As T()()) As List(Of T())
        Dim emptyProduct As IEnumerable(Of IEnumerable(Of T)) = New IEnumerable(Of T)() {Enumerable.Empty(Of T)()}
        Dim c = Sets.Aggregate(emptyProduct, Function(accumulator, sequence)
                                                 Return (From accseq In accumulator
                                                         From item In sequence
                                                         Select accseq.Concat(New T() {item}))
                                             End Function)
        Dim aa As New List(Of T())
        For Each item In c
            aa.Add(item.ToArray)
        Next
        Return aa
    End Function

    ''' <summary>
    ''' Retorna uma sequencia Fibonacci de N numeros
    ''' </summary>
    ''' <param name="Length">Quantidade de numeros da sequencia</param>
    ''' <returns>Lista com a sequencia Fibonacci</returns>
    Public Function Fibonacci(Length As Integer) As List(Of Integer)
        Dim lista As New List(Of Integer)
        lista.AddRange({0, 1})
        For index = 2 To Length - 1
            lista.Add(lista(index - 1) + lista(index - 2))
        Next
        Return lista
    End Function

    ''' <summary>
    ''' Calcula o fatorial de um numero
    ''' </summary>
    ''' <param name="Number">Numero inteiro positivo maior que zero</param>
    ''' <returns>fatorial do numero inteiro</returns>
    <Extension>
    Public Function Factorial(Number As Integer) As Integer
        Number = Mathematic.Round(Number)
        If Number = 0 Then Return 0
        If Number < 0 Then
            Number = Number * (-1)
        End If
        Dim fact = Number
        Dim counter = Number - 1
        While counter > 0
            fact = fact * counter
            counter.Decrement
        End While
        Return fact
    End Function

    ''' <summary>
    ''' Retorna o percentual de um valor
    ''' </summary>
    ''' <param name="Value">Valor a ser calculado</param>
    ''' <param name="Total">Valor Total (Representa 100%)</param>
    ''' <returns>Um numero decimal contendo a porcentagem</returns>

    <Extension()>
    Function CalculatePercent(ByVal Value As Decimal, Total As Decimal) As Decimal
        Return Convert.ToDecimal(100 * Value / Total)
    End Function

    ''' <summary>
    ''' Retorna o valor de um determinado percentual de um valor total
    ''' </summary>
    ''' <param name="Percent">
    ''' Porcentagem, pode ser um numero ou uma string com o sinal de porcento. Ex.: 15 ou 15%
    ''' </param>
    ''' <param name="Total">  Valor Total (Representa 100%)</param>
    ''' <returns>Um numero decimal contendo o valor relativo a porcentagem</returns>

    <Extension()>
    Function CalculateValueFromPercent(Percent As String, Total As Decimal) As Decimal
        Return Convert.ToDecimal(Convert.ToDecimal(Percent.Replace("%", "")) * Total / 100)
    End Function

    ''' <summary>
    ''' Corta um numero decimal com a quntdade de casas especiicadas
    ''' </summary>
    ''' <param name="Value"> Numero</param>
    ''' <param name="Places">Numero de casas apos a virgula</param>
    ''' <returns></returns>
    <Extension>
    Public Function Slice(Value As Decimal, Optional Places As Integer = 2) As Decimal
        Try
            Dim splaces = ""
            If Places > 0 Then splaces = "."
            For index = 1 To Places
                splaces.Append("#")
            Next
            Return Value.ToString("###############" & splaces & "}").To(Of Decimal)
        Catch ex As Exception
            Return Value
        End Try
    End Function

    ''' <summary>
    ''' Retorna um numero inteiro representando as casas decimais de um numero decimal
    ''' </summary>
    ''' <param name="Value">Valor decimal</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDecimalPlaces(Value As Decimal, Optional DecimalPlaces As Integer = 0) As Long
        Dim f = Value.ToString.Split(",")
        Try
            Return If(f(1).IsNotBlank(), f(1).Slice(If(DecimalPlaces > 0, DecimalPlaces, f(1).Length).To(Of Integer)), 0)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
    ''' </summary>
    ''' <param name="Number">Numero a ser arredondado</param>
    ''' <returns>Um numero inteiro (Integer ou Int)</returns>

    <Extension()>
    Public Function Ceil(Number As Decimal) As Long
        Return Math.Ceiling(Number)
    End Function

    ''' <summary>
    ''' Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
    ''' </summary>
    ''' <param name="Number">Numero a ser arredondado</param>
    ''' <returns>Um numero inteiro (Integer ou Int)</returns>

    <Extension()>
    Public Function Ceil(Number As Double) As Long
        Return Math.Ceiling(Number)
    End Function

    ''' <summary>
    ''' Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
    ''' </summary>
    ''' <param name="Number">Numero a ser arredondado</param>
    ''' <returns>Um numero inteiro (Integer ou Int)</returns>
    <Extension()>
    Public Function Floor(Number As Decimal) As Long
        Return Math.Floor(Number)
    End Function

    ''' <summary>
    ''' Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
    ''' </summary>
    ''' <param name="Number">Numero a ser arredondado</param>
    ''' <returns>Um numero inteiro (Integer ou Int)</returns>
    <Extension()>
    Public Function Floor(Number As Double) As Long
        Return Math.Floor(Number)
    End Function

    ''' <summary>
    ''' Limita o valor Maximo de um numero
    ''' </summary>
    ''' <param name="Number">  Numero</param>
    ''' <param name="MaxValue">Valor Maximo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function SetMaxValue(Of Type As IConvertible)(ByVal Number As Type, MaxValue As Type) As Type
        Return Number.LimitRange(MaxValue:=MaxValue)
    End Function

    ''' <summary>
    ''' Limita o valor minimo de um numero
    ''' </summary>
    ''' <param name="Number">  Numero</param>
    ''' <param name="MinValue">Valor Maximo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function SetMinValue(Of Type As IConvertible)(ByVal Number As Type, MinValue As Type) As Type
        Return Number.LimitRange(MinValue:=MinValue)
    End Function

    ''' <summary>
    ''' Limita um range para um numero
    ''' </summary>
    ''' <param name="Number">  Numero</param>
    ''' <param name="MinValue">Valor Minimo para o numero</param>
    ''' <param name="MaxValue">Valor máximo para o numero</param>
    ''' <returns></returns>
    <Extension()>
    Public Function LimitRange(Of Type As IConvertible)(ByVal Number As Type, Optional MinValue As Object = Nothing, Optional MaxValue As Object = Nothing) As Type

        If Not IsNothing(MaxValue) Then
            Number = If(Number < MaxValue, Number, MaxValue)
        End If

        If Not IsNothing(MinValue) Then
            Number = If(Number > MinValue, Number, MinValue)
        End If
        Return Number
    End Function

    ''' <summary>
    ''' Arredonda um numero para baixo ou para cima de acordo com outro numero
    ''' </summary>
    ''' <param name="Number">      Numero</param>
    ''' <param name="MiddleNumber">Numero Médio</param>
    ''' <returns></returns>
    <Extension()> Public Function Round(Number As Decimal, Optional MiddleNumber As Integer = 5) As Integer
        MiddleNumber.LimitRange(1, 10)
        Dim split = Number.ToString.Replace(".", ",").Split(",")
        If split(1).GetFirstChars(1).To(Of Integer) > MiddleNumber Then
            Return Number.Ceil
        Else
            Return Number.Floor
        End If
    End Function

    ''' <summary>
    ''' Arredonda um numero para o valor inteiro mais próximo
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    <Extension()> Public Function Round(Number As Decimal) As Integer
        Return Math.Round(Number)
    End Function

    ''' <summary>
    ''' Realiza um calculo de interpolação Linear
    ''' </summary>
    ''' <param name="Start"> </param>
    ''' <param name="[End]"> </param>
    ''' <param name="Amount"></param>
    ''' <returns></returns>
    <Extension>
    Public Function Lerp(Start As Single, [End] As Single, Amount As Single) As Single
        Dim difference As Single = [End] - Start
        Dim adjusted As Single = difference * Amount
        Return Start + adjusted
    End Function

    ''' <summary>
    ''' Soma todos os números de um array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo a soma de todos os valores</returns>
    Public Function Sum(ByVal ParamArray Values As Double()) As Double
        Return Values.Sum
    End Function

    ''' <summary>
    ''' Soma todos os números de um array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo a soma de todos os valores</returns>
    Public Function Sum(ByVal ParamArray Values As Long()) As Long
        Return Values.Sum
    End Function

    ''' <summary>
    ''' Soma todos os números de um array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo a soma de todos os valores</returns>
    Public Function Sum(ByVal ParamArray Values As Integer()) As Integer
        Return Values.Sum
    End Function

    ''' <summary>
    ''' Soma todos os números de um array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo a soma de todos os valores</returns>
    Public Function Sum(ByVal ParamArray Values As Decimal()) As Decimal
        Return Values.Sum
    End Function

    ''' <summary>
    ''' Tira a média de todos os números de um Array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo</returns>
    Public Function Average(ByVal ParamArray Values As Decimal()) As Decimal
        Return Values.Average
    End Function

    ''' <summary>
    ''' Tira a média de todos os números de um Array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo</returns>
    Public Function Average(ByVal ParamArray Values As Double()) As Double
        Return Values.Average
    End Function

    ''' <summary>
    ''' Tira a média de todos os números de um Array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo</returns>
    Public Function Average(ByVal ParamArray Values As Integer()) As Integer
        Return Values.Average
    End Function

    ''' <summary>
    ''' Tira a média de todos os números de um Array
    ''' </summary>
    ''' <param name="Values">Array de números</param>
    ''' <returns>Decimal contendo</returns>
    Public Function Average(ByVal ParamArray Values As Long()) As Long
        Return Values.Average
    End Function

    ''' <summary>
    ''' COnverte graus para radianos
    ''' </summary>
    ''' <param name="Degrees"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ToRadians(Degrees As Double) As Double
        Return Degrees * Math.PI / 180.0
    End Function

    ''' <summary>
    ''' Calcula a distancia entre 2 locais
    ''' </summary>
    ''' <param name="FirstLocation"> Primeiro Local</param>
    ''' <param name="SecondLocation">Segundo Local</param>
    ''' <returns>A distancia em kilometros</returns>
    Public Function CalculateDistance(FirstLocation As Location, SecondLocation As Location) As Double
        Dim circumference As Double = 40000.0
        ' Earth's circumference at the equator in km
        Dim distance As Double = 0.0

        'Calculate radians
        Dim latitude1Rad As Double = ToRadians(FirstLocation.Latitude)
        Dim longitude1Rad As Double = ToRadians(FirstLocation.Longitude)
        Dim latititude2Rad As Double = ToRadians(SecondLocation.Latitude)
        Dim longitude2Rad As Double = ToRadians(SecondLocation.Longitude)

        Dim logitudeDiff As Double = Math.Abs(longitude1Rad - longitude2Rad)

        If logitudeDiff > Math.PI Then
            logitudeDiff = 2.0 * Math.PI - logitudeDiff
        End If

        Dim angleCalculation As Double = Math.Acos(Math.Sin(latititude2Rad) * Math.Sin(latitude1Rad) + Math.Cos(latititude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(logitudeDiff))

        distance = circumference * angleCalculation / (2.0 * Math.PI)

        Return distance
    End Function

    ''' <summary>
    ''' Calcula a distancia passando por todos os pontos
    ''' </summary>
    ''' <param name="Locations">Localizacoes</param>
    ''' <returns></returns>
    Public Function CalculateDistance(ParamArray Locations As Location()) As Double
        Dim totalDistance As Double = 0.0

        For i As Integer = 0 To Locations.Length - 2
            Dim current As Location = Locations(i)
            Dim [next] As Location = Locations(i + 1)

            totalDistance += CalculateDistance(current, [next])
        Next

        Return totalDistance
    End Function

    ''' <summary>
    ''' Verifica se um numero está entre outros 2 números
    ''' </summary>
    ''' <param name="Number">      Numero</param>
    ''' <param name="FirstNumber"> Primeiro numero comparador</param>
    ''' <param name="SecondNumber">Segundo numero comparador</param>
    ''' <returns></returns>
    <Extension()> Public Function IsBetween(Of Type)(Number As Type, FirstNumber As Object, SecondNumber As Object) As Boolean
        FirstNumber = DirectCast(FirstNumber, Type)
        SecondNumber = DirectCast(SecondNumber, Type)
        Select Case True
            Case FirstNumber < SecondNumber
                Return FirstNumber < Number And Number < SecondNumber
            Case FirstNumber > SecondNumber
                Return FirstNumber > Number And Number > SecondNumber
            Case Else
                Return FirstNumber = Number And Number = SecondNumber
        End Select
    End Function

End Module