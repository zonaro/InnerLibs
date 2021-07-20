Imports System.Data
Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports InnerLibs.Locations


''' <summary>
''' Representa um Par X,Y para operaçoes matemáticas
''' </summary>
''' 
Public Class EquationPair

    Sub New()

    End Sub

    Sub New(X As Decimal?, Y As Decimal?)
        Me.X = X
        Me.Y = Y
    End Sub

    Property X As Decimal?
    Property Y As Decimal?


    Function ToArray() As Decimal?()
        Return {X, Y}
    End Function



End Class

Public Class RuleOfThree

    ''' <summary>
    ''' Calcula uma regra de tres
    ''' </summary>
    ''' <param name="FirstEquation"></param>
    ''' <param name="SecondEquation"></param>
    Sub New(FirstEquation As EquationPair, SecondEquation As EquationPair)
        Me.FirstEquation = FirstEquation
        Me.SecondEquation = SecondEquation
        Start()
    End Sub

    Private Sub Start()
        Select Case True
            Case {FirstEquation.Y, SecondEquation.X, SecondEquation.Y}.All(Function(x) x.HasValue)
                FirstEquation.X = SecondEquation.X * FirstEquation.Y / SecondEquation.Y
                Exit Select
            Case {FirstEquation.X, SecondEquation.X, SecondEquation.Y}.All(Function(x) x.HasValue)
                FirstEquation.Y = FirstEquation.X * SecondEquation.Y / FirstEquation.X
                Exit Select

            Case {FirstEquation.X, FirstEquation.Y, SecondEquation.X}.All(Function(x) x.HasValue)
                SecondEquation.Y = SecondEquation.X * FirstEquation.Y / FirstEquation.X
                Exit Select

            Case {FirstEquation.X, FirstEquation.Y, SecondEquation.Y}.All(Function(x) x.HasValue)
                SecondEquation.X = SecondEquation.Y * FirstEquation.X / FirstEquation.Y
                Exit Select
            Case Else
                Throw New NoNullAllowedException("Three numbers need to be known to make a rule of three")
        End Select

    End Sub

    ''' <summary>
    ''' Calcula uma regra de três
    ''' </summary>
    ''' <param name="E1X"></param>
    ''' <param name="E1Y"></param>
    ''' <param name="E2X"></param>
    ''' <param name="E2Y"></param>
    Sub New(E1X As Decimal?, E1Y As Decimal?, E2X As Decimal?, E2Y As Decimal?)
        Me.New(New EquationPair(E1X, E1Y), New EquationPair(E2X, E2Y))
    End Sub

    ''' <summary>
    ''' Calcula uma regra de três
    ''' </summary>
    Sub New(ParamArray Numbers As Decimal?())
        Numbers = If(Numbers, {})
        Select Case True
            Case Numbers.Count < 3
                Throw New NoNullAllowedException("Three numbers need to be known to make a rule of three")
                Exit Select
            Case Numbers.Count = 3
                Me.FirstEquation.X = Numbers(0)
                Me.FirstEquation.Y = Numbers(1)
                Me.SecondEquation.X = Numbers(2)
                Me.SecondEquation.Y = Nothing
            Case Else
                If Numbers.All(Function(x) x.HasValue) Then
                    Throw New NoNullAllowedException("One of numbers must be NULL")
                    Exit Select
                End If

                If Numbers.Count(Function(x) x.HasValue) < 3 Then
                    Throw New NoNullAllowedException("Three numbers need to be known to make a rule of three")
                    Exit Select
                End If

                Me.FirstEquation.X = Numbers.IfNoIndex(0)
                Me.FirstEquation.Y = Numbers.IfNoIndex(1)
                Me.SecondEquation.X = Numbers.IfNoIndex(2)
                Me.SecondEquation.Y = Numbers.IfNoIndex(3)
                Start()
        End Select

    End Sub

    ''' <summary>
    ''' Primeira Equaçao
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property FirstEquation As New EquationPair

    ''' <summary>
    ''' Segunda Equaçao
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SecondEquation As New EquationPair




    Function ToArray() As Decimal?()()
        Return {FirstEquation.ToArray, SecondEquation.ToArray}
    End Function
End Class



''' <summary>
''' Módulo para calculos
''' </summary>
''' <remarks></remarks>
Public Module Mathematic


    ''' <summary>
    ''' Calcula os Juros simples
    ''' </summary>
    ''' <param name="Capital">Capital</param>
    ''' <param name="Rate">Taxa</param>
    ''' <param name="Time">Tempo</param>
    ''' <returns></returns>
    <Extension()> Public Function CalculateSimpleInterest(Capital As Decimal, Rate As Decimal, Time As Decimal)
        Return Capital * Rate * Time
    End Function

    ''' <summary>
    ''' Calcula Juros compostos
    ''' </summary>
    ''' <param name="Capital">Capital</param>
    ''' <param name="Rate">Taxa</param>
    ''' <param name="Time">Tempo</param>
    ''' <returns></returns>
    <Extension()> Public Function CalculateCompoundInterest(Capital As Decimal, Rate As Decimal, Time As Decimal)
        Return Capital * ((1 + Rate) ^ Time)
    End Function

    ''' <summary>
    ''' Verifica se um numero possui parte decimal
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    <Extension> Public Function HasDecimalPart(Value As Decimal) As Boolean
        If Value < 0 Then Value = Value * -1
        Return Not (Value Mod 1) = 0 AndAlso Value > 0
    End Function

    ''' <summary>
    ''' Verifica se um numero possui parte decimal
    ''' </summary>
    ''' <param name="Value"></param>
    ''' <returns></returns>
    <Extension> Public Function HasDecimalPart(Value As Double) As Boolean
        Return Value.ChangeType(Of Decimal).HasDecimalPart()
    End Function

    ''' <summary>
    ''' Retorna o elemento de menor valor de uma coleção
    ''' </summary>
    ''' <typeparam name="T">Tipo do elemento</typeparam>
    ''' <param name="Elements">Lista de elementos</param>
    ''' <returns></returns>
    <Extension()> Function LowerOf(Of T)(Elements As IEnumerable(Of T)) As T
        Return If(Elements, {}).OrderBy(Function(x) x).FirstOrDefault
    End Function

    ''' <summary>
    ''' Retorna o elemento de menor valor de uma coleção
    ''' </summary>
    ''' <typeparam name="T">Tipo do elemento</typeparam>
    ''' <param name="Elements">Lista de elementos</param>
    ''' <returns></returns>
    Function LowerOf(Of T)(ParamArray Elements As T()) As T
        Return If(Elements, {}).LowerOf
    End Function

    ''' <summary>
    ''' Retorna o elemento de maior valor de uma coleção
    ''' </summary>
    ''' <typeparam name="T">Tipo do elemento</typeparam>
    ''' <param name="Elements">Lista de elementos</param>
    ''' <returns></returns>
    <Extension()> Function HigherOf(Of T)(Elements As IEnumerable(Of T)) As T
        Return If(Elements, {}).OrderByDescending(Function(x) x).FirstOrDefault
    End Function

    ''' <summary>
    ''' Retorna o elemento de maior valor de uma coleção
    ''' </summary>
    ''' <typeparam name="T">Tipo do elemento</typeparam>
    ''' <param name="Elements">Lista de elementos</param>
    ''' <returns></returns>
    Function HigherOf(Of T)(ParamArray Elements As T()) As T
        Return If(Elements, {}).HigherOf
    End Function



    ''' <summary>
    ''' Retorna a diferença entre 2 numeros se o valor minimo for maior que o total
    ''' </summary>
    ''' <param name="Total"></param>
    ''' <param name="MinValue"></param>
    ''' <returns></returns>
    <Extension()> Function DifferenceIfMin(Total As Integer, MinValue As Integer) As Integer
        Return If(Total < MinValue, MinValue - Total, 0)
    End Function

    ''' <summary>
    ''' Retorna a diferença entre 2 numeros se o valor maximo for menor que o total
    ''' </summary>
    ''' <param name="Total"></param>
    ''' <param name="MaxValue"></param>
    ''' <returns></returns>
    <Extension()> Function DifferenceIfMax(Total As Integer, MaxValue As Integer) As Integer
        Return If(Total > MaxValue, MaxValue - Total, 0)
    End Function


    ''' <summary>
    ''' retorna o numeor em sua forma ordinal (inglês)
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    <Extension> Public Function ToOrdinalNumber(Number As Integer, Optional ExcludeNumber As Boolean = False) As String
        Return ToOrdinalNumber(CType(Number, Long), ExcludeNumber)
    End Function

    ''' <summary>
    ''' retorna o numeor em sua forma ordinal (inglês)
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    <Extension> Public Function ToOrdinalNumber(Number As Long, Optional ExcludeNumber As Boolean = False) As String
        If Number > 0 Then
            Select Case Number
                Case 1
                    Return If(ExcludeNumber, "", Number) & "st"
                Case 2
                    Return If(ExcludeNumber, "", Number) & "nd"
                Case 3
                    Return If(ExcludeNumber, "", Number) & "rd"
                Case Else
                    Return If(ExcludeNumber, "", Number) & "th"
            End Select
        End If
        Return ""
    End Function

    ''' <summary>
    ''' retorna o numeor em sua forma ordinal (inglês)
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    <Extension> Public Function ToOrdinalNumber(Number As Short) As String
        Return ToOrdinalNumber(CType(Number, Long))

    End Function

    ''' <summary>
    ''' retorna o numeor em sua forma ordinal (inglês)
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    <Extension> Public Function ToOrdinalNumber(Number As Double) As String
        Return ToOrdinalNumber(CType(Number, Long))

    End Function

    ''' <summary>
    ''' retorna o numeor em sua forma ordinal (inglês)
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    <Extension> Public Function ToOrdinalNumber(Number As Decimal) As String
        Return ToOrdinalNumber(CType(Number, Long))
    End Function

    ''' <summary>
    ''' Executa uma Expressão matematica/lógica simples
    ''' </summary>
    ''' <param name="Formula">Expressão matematica</param>
    ''' <returns></returns>
    Public Function EvaluateExpression(Formula As String, Optional Exception As Boolean = False) As Object
        Try
            Return New Evaluator().Parse(Formula).value
        Catch ex As Exception
            If Exception Then Throw ex
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Executa uma Expressão matematica/lógica simples
    ''' </summary>
    ''' <param name="Formula">Expressão matematica</param>
    ''' <returns></returns>
    Public Function EvaluateExpression(Of T As Structure)(Formula As String, Optional Exception As Boolean = False) As T
        Return CType(EvaluateExpression(Formula, Exception), T)
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
    Public Function CartesianProduct(Of T)(ParamArray Sets As T()()) As List(Of T())
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
    ''' Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="Dic"></param>
    ''' <returns></returns>
    <Extension()> Function CalculatePercent(Of TKey, TValue As Structure)(Dic As Dictionary(Of TKey, TValue)) As Dictionary(Of TKey, Decimal)
        Dim total = Dic.Sum(Function(x) x.Value.ChangeType(Of Decimal))
        Return Dic.Select(Function(x) New KeyValuePair(Of TKey, Decimal)(x.Key, CalculatePercent(x.Value.ChangeType(Of Decimal), total))).ToDictionary
    End Function


    ''' <summary>
    ''' Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    <Extension()> Function CalculatePercent(Of TObject, TKey, TValue As Structure)(Obj As IEnumerable(Of TObject), KeySelector As Func(Of TObject, TKey), ValueSelector As Func(Of TObject, TValue)) As Dictionary(Of TKey, Decimal)
        Return CalculatePercent(Obj.ToDictionary(KeySelector, ValueSelector))
    End Function

    ''' <summary>
    ''' Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
    ''' </summary>
    <Extension()> Function CalculatePercent(Of Tobject, Tvalue As Structure)(Obj As IEnumerable(Of Tobject), ValueSelector As Func(Of Tobject, Tvalue)) As Dictionary(Of Tobject, Decimal)
        Return Obj.CalculatePercent(Function(x) x, ValueSelector)
    End Function

    ''' <summary>
    ''' Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
    ''' </summary>
    <Extension()> Function CalculatePercent(Of TValue As Structure)(Obj As IEnumerable(Of TValue)) As Dictionary(Of TValue, Decimal)
        Return Obj.DistinctCount.CalculatePercent()
    End Function

    ''' <summary>
    ''' Calcula a variação percentual entre 2 valores
    ''' </summary>
    ''' <param name="StartValue"></param>
    ''' <param name="EndValue"></param>
    ''' <returns></returns>
    <Extension()> Public Function CalculatePercentVariation(StartValue As Decimal, EndValue As Decimal) As Decimal
        If StartValue = 0 Then
            If EndValue > 0 Then
                Return 100
            Else
                Return 0
            End If
        End If

        Return (EndValue / StartValue - 1) * 100
    End Function

    ''' <summary>
    ''' Calcula a variação percentual entre 2 valores
    ''' </summary>
    ''' <param name="StartValue"></param>
    ''' <param name="EndValue"></param>
    ''' <returns></returns>
    <Extension()> Public Function CalculatePercentVariation(StartValue As Integer, EndValue As Integer) As Decimal
        Return StartValue.ToDecimal().CalculatePercentVariation(EndValue.ToDecimal())
    End Function

    ''' <summary>
    ''' Calcula a variação percentual entre 2 valores
    ''' </summary>
    ''' <param name="StartValue"></param>
    ''' <param name="EndValue"></param>
    ''' <returns></returns>
    <Extension()> Public Function CalculatePercentVariation(StartValue As Long, EndValue As Long) As Decimal
        Return StartValue.ToDecimal().CalculatePercentVariation(EndValue.ToDecimal())
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
    ''' Retorna o valor de um determinado percentual de um valor total
    ''' </summary>
    ''' <param name="Percent">
    ''' Porcentagem, pode ser um numero ou uma string com o sinal de porcento. Ex.: 15 ou 15%
    ''' </param>
    ''' <param name="Total">  Valor Total (Representa 100%)</param>
    ''' <returns>Um numero decimal contendo o valor relativo a porcentagem</returns>

    <Extension()>
    Function CalculateValueFromPercent(Percent As Integer, Total As Decimal) As Decimal
        Return Convert.ToDecimal(Convert.ToDecimal(Percent * Total / 100))
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
    Function CalculateValueFromPercent(Percent As Decimal, Total As Decimal) As Decimal
        Return Convert.ToDecimal(Convert.ToDecimal(Percent * Total / 100))
    End Function

    ''' <summary>
    ''' Corta um numero decimal com a quantidade de casas especiicadas
    ''' </summary>
    ''' <param name="Value"> Numero</param>
    ''' <param name="Places">Numero de casas apos a virgula</param>
    ''' <returns></returns>
    <Extension>
    Public Function Slice(Value As Decimal, Optional Places As Integer = 2) As Decimal
        If Places > -1 Then
            Return Decimal.Round(Value, Places.LimitRange(0, 28))
        End If
        Return Value
    End Function

    ''' <summary>
    ''' Retorna um numero inteiro representando a parte decimal de um numero decimal
    ''' </summary>
    ''' <param name="Value">Valor decimal</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetDecimalPlaces(Value As Decimal, Optional DecimalPlaces As Integer = 0, Optional Culture As CultureInfo = Nothing) As Long
        Culture = If(Culture, CultureInfo.CurrentCulture)
        Dim f = Value.ToString.Split(Culture.NumberFormat.NumberDecimalSeparator)
        Try
            Return If(f(1).IsNotBlank(), f(1).Slice(If(DecimalPlaces > 0, DecimalPlaces, f(1).Length).ChangeType(Of Integer)), 0)
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
        Try
            Return Math.Ceiling(Number)
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
    Public Function Ceil(Number As Double) As Long
        Try
            Return Math.Ceiling(Number)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
    ''' </summary>
    ''' <param name="Number">Numero a ser arredondado</param>
    ''' <returns>Um numero inteiro (Integer ou Int)</returns>
    <Extension()>
    Public Function Floor(Number As Decimal) As Long
        Try
            Return Math.Floor(Number)
        Catch ex As Exception
            Return 0
        End Try
    End Function

    ''' <summary>
    ''' Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
    ''' </summary>
    ''' <param name="Number">Numero a ser arredondado</param>
    ''' <returns>Um numero inteiro (Integer ou Int)</returns>
    <Extension()>
    Public Function Floor(Number As Double) As Long
        Try
            Return Math.Floor(Number)
        Catch ex As Exception
            Return 0
        End Try
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

        If If(MinValue, 0) = If(MaxValue, 0) Then
            Return MinValue
        Else
            If Not Nothing = (MaxValue) Then
                Number = If(Number < MaxValue, Number, MaxValue)
            End If

            If Not Nothing = (MinValue) Then
                Number = If(Number > MinValue, Number, MinValue)
            End If
        End If

        Return Number
    End Function

    <Extension()> Public Function LimitIndex(Of AnyType)(Int As Integer, Collection As IEnumerable(Of AnyType)) As Integer
        Return LimitRange(Int, 0, Collection.Count - 1)
    End Function

    <Extension()> Public Function LimitIndex(Of AnyType)(Lng As Long, Collection As IEnumerable(Of AnyType)) As Long
        Return LimitRange(Lng, 0, Collection.LongCount - 1)
    End Function

    ''' <summary>
    ''' Arredonda um numero para baixo ou para cima de acordo com outro numero
    ''' </summary>
    ''' <param name="Number">      Numero</param>
    ''' <param name="MiddleNumber">Numero Médio</param>
    ''' <returns></returns>
    <Extension()> Public Function Round(Number As Decimal, Optional MiddleNumber As Integer = 5, Optional Culture As CultureInfo = Nothing) As Integer
        MiddleNumber.LimitRange(1, 10)
        Dim split = Number.ToString.Split(If(Culture, CultureInfo.CurrentCulture).NumberFormat.NumberDecimalSeparator)
        If split(1).GetFirstChars(1).ChangeType(Of Integer) > MiddleNumber Then
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
    <Extension()> Public Function CalculateDistance(FirstLocation As AddressInfo, SecondLocation As AddressInfo) As Double
        Dim circumference As Double = 40000.0
        ' Earth's circumference at the equator in km

        Dim distance As Double = 0.0
        If FirstLocation.Latitude = SecondLocation.Latitude AndAlso FirstLocation.Longitude = SecondLocation.Longitude Then
            Return distance
        End If

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
    Public Function CalculateDistanceMatrix(ParamArray Locations As AddressInfo()) As Double
        Dim totalDistance As Double = 0.0

        CartesianProduct(Locations, Locations)



    End Function




End Module