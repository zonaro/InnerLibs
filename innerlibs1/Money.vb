Imports System.Globalization

Imports InnerLibs

''' <summary>
''' Estrutura que representa valores em dinheiro de uma determinada <see cref="CultureInfo"/>.
''' </summary>
Public Structure Money

    Public ReadOnly Property Value As Decimal

    ''' <summary>
    ''' Cria uma nova instancia de moeda
    ''' </summary>
    ''' <param name="Value">Valor</param>
    Sub New(Optional Value As Decimal = 0)
        Me.New(Value, CultureInfo.CurrentCulture)
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de moeda
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="Culture">Cultura</param>
    Public Sub New(Value As Decimal, Culture As CultureInfo)
        Me.Value = Value
        Me.Culture = Culture
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de moeda
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="Culture">Cultura</param>
    Public Sub New(Value As Decimal, Culture As String)
        Me.New(Value, CultureInfo.GetCultureInfo(Culture.IfBlank(CultureInfo.CurrentCulture.Name)))
    End Sub

    ''' <summary>
    ''' String do valor formatado como moeda, é um alias para <see cref="MoneyString"/>
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Return MoneyString
    End Function

    ''' <summary>
    ''' String do valor formatado como moeda
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property MoneyString As String
        Get
            Return Me.ToString(-1)
        End Get
    End Property

    ''' <summary>
    ''' String do valor formatado como moeda
    ''' </summary>
    ''' <param name="Precision">Precisao de casas decimais</param>
    ''' <returns></returns>
    Public Overloads Function ToString(Precision As Integer) As String
        If Precision >= 0 Then
            Return Value.ToString($"C{Precision.LimitRange(0, 99)}", Me.Culture)
        Else
            Return Value.ToString($"C", Me.Culture)
        End If
    End Function

    ''' <summary>
    ''' Traz uma lista de <see cref="CultureInfo"/> que utilizam uma determinada moeda de acordo com o simbolo, simbolo ISO ou
    ''' </summary>
    ''' <param name="Currency">Moeda</param>
    ''' <returns></returns>
    Public Shared Function GetCultureInfosByCurrencySymbol(Currency As String) As List(Of CultureInfo)
        If Currency Is Nothing OrElse Currency.IsBlank Then
            Throw New ArgumentNullException("Currency is blank")
        End If
        Dim l = CultureInfo.GetCultures(CultureTypes.SpecificCultures) _
        .Where(Function(x) (New RegionInfo(x.LCID).ISOCurrencySymbol.Trim = Currency.Trim Or New RegionInfo(x.LCID).CurrencySymbol.Trim = Currency.Trim Or x.Name.Trim = Currency.Trim)).ToList
        If l.Count = 0 Then l.Add(CultureInfo.InvariantCulture)
        Return l
    End Function

    ''' <summary>
    ''' Região correspondente a essa moeda
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Region As RegionInfo
        Get
            If reg Is Nothing Then
                reg = New RegionInfo(Culture.Name)
            End If
            Return reg
        End Get
    End Property

    Private reg As RegionInfo

    ''' <summary>
    ''' Cultura correspondente a esta moeda
    ''' </summary>
    ''' <returns></returns>

    Property Culture As CultureInfo
        Get
            Return _culture
        End Get
        Set(value As CultureInfo)
            If value.Equals(CultureInfo.InvariantCulture) Then
                value = Nothing
            End If
            _culture = If(value, CultureInfo.CurrentCulture)
            reg = Nothing
        End Set
    End Property

    Private _culture As CultureInfo

    Public Shared Operator &(Text As String, Value As Money) As String
        Return Text & Value.MoneyString
    End Operator

    Public Shared Operator &(Value As Money, Text As String) As String
        Return Value.MoneyString & Text
    End Operator

    Public Shared Operator Not(Value As Money)
        Return New Money(Value.Value * -1, Value.Culture)
    End Operator

    Public Shared Operator +(Text As String, Value As Money) As String
        If Text.IsNumber Then
            Return (Text.ChangeType(Of Decimal) + Value.Value).ToString
        Else
            Return Text & Value.MoneyString
        End If
    End Operator

    Public Shared Operator +(Value As Money, Text As String) As String
        Return Text + Value
    End Operator

    Public Shared Operator +(Value1 As Double, Value2 As Money) As Money
        Return New Money(Value1 + Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator +(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 + Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator +(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 + Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator +(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 + Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator +(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 + Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator +(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 + Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator +(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 + Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value1 As Double, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 - Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 - Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 - Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator -(Value1 As Money, Value2 As Money) As Money
        Return New Money(Value1.Value - Value2.Value, Value1.Culture)
    End Operator

    Public Shared Operator *(Value1 As Double, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator *(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 * Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator *(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator *(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 * Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator *(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator *(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 * Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator *(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator /(Value1 As Double, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator /(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator /(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator /(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator /(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator /(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator /(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator =(Value1 As Double, Value2 As Money) As Boolean
        Return Value1 = Value2.Value
    End Operator

    Public Shared Operator =(Value2 As Money, Value1 As Double) As Boolean
        Return Value1 = Value2.Value

    End Operator

    Public Shared Operator =(Value1 As Decimal, Value2 As Money) As Boolean
        Return Value1 = Value2.Value

    End Operator

    Public Shared Operator =(Value2 As Money, Value1 As Decimal) As Boolean
        Return Value1 = Value2.Value
    End Operator

    Public Shared Operator =(Value1 As Integer, Value2 As Money) As Boolean
        Return Value1 = Value2.Value

    End Operator

    Public Shared Operator =(Value2 As Money, Value1 As Integer) As Boolean
        Return Value1 = Value2.Value

    End Operator

    Public Shared Operator =(Value1 As Long, Value2 As Money) As Boolean
        Return Value1 = Value2.Value

    End Operator

    Public Shared Operator <>(Value2 As Money, Value1 As Money) As Boolean
        Return Not Value1 = Value2
    End Operator

    Public Shared Operator =(Value2 As Money, Value1 As Money) As Boolean
        Return Not Value1.Value = Value2.Value
    End Operator

    Public Shared Operator <>(Value1 As Double, Value2 As Money) As Boolean
        Return Not Value1 = Value2.Value
    End Operator

    Public Shared Operator <>(Value2 As Money, Value1 As Double) As Boolean
        Return Not Value1 = Value2.Value
    End Operator

    Public Shared Operator <>(Value1 As Decimal, Value2 As Money) As Boolean
        Return Not Value1 = Value2.Value
    End Operator

    Public Shared Operator <>(Value2 As Money, Value1 As Decimal) As Boolean
        Return Not Value1 = Value2.Value

    End Operator

    Public Shared Operator <>(Value1 As Integer, Value2 As Money) As Boolean
        Return Not Value1 = Value2.Value

    End Operator

    Public Shared Operator <>(Value2 As Money, Value1 As Integer) As Boolean
        Return Not Value1 = Value2.Value

    End Operator

    Public Shared Operator <>(Value1 As Long, Value2 As Money) As Boolean
        Return Not Value1 = Value2.Value
    End Operator

    Public Shared Operator >=(Value1 As Double, Value2 As Money) As Boolean
        Return Value1 >= Value2.Value
    End Operator

    Public Shared Operator >=(Value2 As Money, Value1 As Double) As Boolean
        Return Value1 >= Value2.Value
    End Operator

    Public Shared Operator >=(Value1 As Decimal, Value2 As Money) As Boolean
        Return Value1 >= Value2.Value
    End Operator

    Public Shared Operator >=(Value2 As Money, Value1 As Decimal) As Boolean
        Return Value1 >= Value2.Value
    End Operator

    Public Shared Operator >=(Value1 As Integer, Value2 As Money) As Boolean
        Return Value1 >= Value2.Value

    End Operator

    Public Shared Operator >=(Value2 As Money, Value1 As Integer) As Boolean
        Return Value1 >= Value2.Value

    End Operator

    Public Shared Operator >=(Value1 As Long, Value2 As Money) As Boolean
        Return Value1 >= Value2.Value

    End Operator

    Public Shared Operator <=(Value1 As Double, Value2 As Money) As Boolean
        Return Value1 <= Value2.Value
    End Operator

    Public Shared Operator <=(Value2 As Money, Value1 As Double) As Boolean
        Return Value1 <= Value2.Value

    End Operator

    Public Shared Operator <=(Value1 As Decimal, Value2 As Money) As Boolean
        Return Value1 <= Value2.Value

    End Operator

    Public Shared Operator <=(Value2 As Money, Value1 As Decimal) As Boolean
        Return Value1 <= Value2.Value
    End Operator

    Public Shared Operator <=(Value1 As Integer, Value2 As Money) As Boolean
        Return Value1 <= Value2.Value

    End Operator

    Public Shared Operator <=(Value2 As Money, Value1 As Integer) As Boolean
        Return Value1 <= Value2.Value

    End Operator

    Public Shared Operator <=(Value1 As Long, Value2 As Money) As Boolean
        Return Value1 <= Value2.Value

    End Operator

    Public Shared Operator >(Value1 As Double, Value2 As Money) As Boolean
        Return Value1 > Value2.Value
    End Operator

    Public Shared Operator >(Value2 As Money, Value1 As Double) As Boolean
        Return Value1 > Value2.Value

    End Operator

    Public Shared Operator >(Value1 As Decimal, Value2 As Money) As Boolean
        Return Value1 > Value2.Value

    End Operator

    Public Shared Operator >(Value2 As Money, Value1 As Decimal) As Boolean
        Return Value1 > Value2.Value
    End Operator

    Public Shared Operator >(Value1 As Integer, Value2 As Money) As Boolean
        Return Value1 > Value2.Value

    End Operator

    Public Shared Operator >(Value2 As Money, Value1 As Integer) As Boolean
        Return Value1 > Value2.Value

    End Operator

    Public Shared Operator >(Value1 As Long, Value2 As Money) As Boolean
        Return Value1 > Value2.Value
    End Operator

    Public Shared Operator <(Value1 As Double, Value2 As Money) As Boolean
        Return Value1 < Value2.Value
    End Operator

    Public Shared Operator <(Value2 As Money, Value1 As Double) As Boolean
        Return Value1 < Value2.Value

    End Operator

    Public Shared Operator <(Value1 As Decimal, Value2 As Money) As Boolean
        Return Value1 < Value2.Value

    End Operator

    Public Shared Operator <(Value2 As Money, Value1 As Decimal) As Boolean
        Return Value1 < Value2.Value
    End Operator

    Public Shared Operator <(Value1 As Integer, Value2 As Money) As Boolean
        Return Value1 < Value2.Value

    End Operator

    Public Shared Operator <(Value2 As Money, Value1 As Integer) As Boolean
        Return Value1 < Value2.Value

    End Operator

    Public Shared Operator <(Value1 As Long, Value2 As Money) As Boolean
        Return Value1 < Value2.Value
    End Operator

    ''' <summary>
    ''' Verifica se 2 valores sao da mesma moeda
    ''' </summary>
    ''' <param name="Value1"></param>
    ''' <param name="Value2"></param>
    ''' <returns></returns>
    Public Shared Operator Like(Value1 As Money, Value2 As Money) As Boolean
        Return Value1.Region.ISOCurrencySymbol = Value2.Region.CurrencySymbol
    End Operator

    Public Shared Widening Operator CType(v As Money) As Decimal
        Return v.Value
    End Operator

    Public Shared Widening Operator CType(v As Decimal) As Money
        Return New Money(v)
    End Operator

    Public Shared Widening Operator CType(v As Integer) As Money
        Return New Money(v)
    End Operator

    Public Shared Widening Operator CType(v As Short) As Money
        Return New Money(v)
    End Operator

    Public Shared Widening Operator CType(v As Long) As Money
        Return New Money(v)
    End Operator

    Public Shared Widening Operator CType(v As Double) As Money
        Return New Money(v)
    End Operator

    Public Shared Widening Operator CType(v As Single) As Money
        Return New Money(v)
    End Operator

    ''' <summary>
    ''' Compara se 2 valores são iguais (mesmo valor e moeda)
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj.GetType = GetType(Money) Then
            Return Me.Value = CType(obj, Money).Value AndAlso Me.Region.ISOCurrencySymbol = CType(obj, Money).Region.ISOCurrencySymbol
        Else
            Return False
        End If
    End Function

    Public Function ConvertToCurrency(Base As Decimal, Culture As String) As Money
        Return New Money(Value / Base, Culture)
    End Function

    Public Function ConvertFromCurrency(Base As Decimal, Culture As String) As Money
        Return New Money(Base * Value, Culture)
    End Function

    Public Function ConvertToCurrency(Base As Decimal, Culture As CultureInfo) As Money
        Return New Money(Value / Base, Culture)
    End Function

    Public Function ConvertFromCurrency(Base As Decimal, Culture As CultureInfo) As Money
        Return New Money(Base * Value, Culture)
    End Function

End Structure