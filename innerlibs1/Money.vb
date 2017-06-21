Imports System.Globalization

Public Structure Money

    Public ReadOnly Property Value As Double
    Private Culture As CultureInfo

    ''' <summary>
    ''' Cria uma nova instancia de moeda
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="Culture">Cultura</param>
    Public Sub New(Value As Double, Culture As CultureInfo)
        Me.Value = Value
        Me.Culture = Culture
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de moeda
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="Symbol">Simbolo de moeda, ISO ou nome da cultura</param>
    Public Sub New(Value As Double, Symbol As String)
        Me.Value = Value
        Me.Culture = GetCultureInfosByCurrencySymbol(Symbol).FirstOrDefault
    End Sub

    ''' <summary>
    ''' Converte de uma moeda para a outra utilizando a api http://fixer.io
    ''' </summary>
    ''' <param name="Symbol">Simbolo de moeda, ISO ou nome da cultura</param>
    ''' <returns></returns>
    Function ConvertCurrency(Symbol As String) As Money
        Return ConvertCurrency(GetCultureInfosByCurrencySymbol(Symbol).FirstOrDefault)
    End Function

    ''' <summary>
    ''' Converte de uma moeda para a outra utilizando a api http://fixer.io
    ''' </summary>
    ''' <param name="Culture">Cultura</param>
    ''' <returns></returns>
    Function ConvertCurrency(Culture As CultureInfo) As Money
        If Culture.Name = Me.Culture.Name Then Return New Money(Me.Value, Me.Culture)
        If Not IsConnected() Then
            Throw New Exception("Internet is not available to convert currency.")
        End If
        Dim rep = AJAX.GET(Of Object)("http://api.fixer.io/latest?base=" & Me.ISOCurrencySymbol)
        Dim dic As New Dictionary(Of String, Double)
        Try
            For Each item In rep("rates")
                dic.Add(item.Key, item.Value)
            Next
        Catch ex As Exception
            Throw New Exception("Fixer.io can't convert from currency " & Me.ISOCurrencySymbol.Quote)
        End Try
        Try
            Return New Money(Me.Value * dic.Where(Function(y) y.Key = New RegionInfo(Culture.Name).ISOCurrencySymbol).Select(Function(x) x.Value).First, Culture)
        Catch ex As Exception
            Throw New Exception("Fixer.io can't convert to currency " & New RegionInfo(Culture.Name).ISOCurrencySymbol)
        End Try
    End Function

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
            Return String.Format(Me.Culture, "{0:C}", Me.Value)
        End Get
    End Property

    ''' <summary>
    ''' Simbolo de moeda
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CurrencySymbol As String
        Get
            Return Me.Culture.NumberFormat.CurrencySymbol
        End Get
    End Property

    ''' <summary>
    ''' Simbolo de moeda utilizada em cambio (ISO)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ISOCurrencySymbol As String
        Get
            Return New RegionInfo(Me.Culture.Name).ISOCurrencySymbol
        End Get
    End Property

    ''' <summary>
    ''' Traz uma lista de <see cref="CultureInfo"/> que utilizam uma determinada moeda de acordo com o simbolo, simbolo ISO ou
    ''' </summary>
    ''' <param name="Currency">Moeda</param>
    ''' <returns></returns>
    Public Shared Function GetCultureInfosByCurrencySymbol(Currency As String) As List(Of CultureInfo)
        If Currency Is Nothing Or Currency.IsBlank Then
            Throw New ArgumentNullException("Currency is blank")
        End If
        Return CultureInfo.GetCultures(CultureTypes.SpecificCultures) _
        .Where(Function(x) (New RegionInfo(x.LCID).ISOCurrencySymbol.Trim = Currency.Trim Or New RegionInfo(x.LCID).CurrencySymbol.Trim = Currency.Trim Or x.Name.Trim = Currency.Trim)).ToList
    End Function

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
        Return Text & Value.MoneyString
    End Operator

    Public Shared Operator +(Value As Money, Text As String) As String
        Return Value.MoneyString & Text
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

    Public Shared Operator +(Value2 As Money, Value1 As Money) As Money
        If Not Value1.Culture.Name = Value2.Culture.Name Then
            Value1 = Value1.ConvertCurrency(Value2.Culture)
        End If
        Return New Money(Value1.Value + Value2.Value, Value2.Culture)
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

    Public Shared Operator *(Value2 As Money, Value1 As Money) As Money
        If Not Value1.Culture.Name = Value2.Culture.Name Then
            Value1 = Value1.ConvertCurrency(Value2.Culture)
        End If
        Return New Money(Value1.Value * Value2.Value, Value2.Culture)
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

    Public Shared Operator /(Value2 As Money, Value1 As Money) As Money
        If Not Value1.Culture.Name = Value2.Culture.Name Then
            Value1 = Value1.ConvertCurrency(Value2.Culture)
        End If
        Return New Money(Value1.Value / Value2.Value, Value2.Culture)
    End Operator

    Public Shared Operator =(Value2 As Money, Value1 As Money) As Boolean
        Return Value1.Value = Value2.ConvertCurrency(Value1.Culture)
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

    Public Shared Operator >=(Value2 As Money, Value1 As Money) As Boolean
        Return Value1.Value >= Value2.ConvertCurrency(Value1.Culture).Value
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

    Public Shared Operator <=(Value2 As Money, Value1 As Money) As Boolean
        Return Value1.Value <= Value2.ConvertCurrency(Value1.Culture).Value
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

    Public Shared Operator >(Value2 As Money, Value1 As Money) As Boolean
        Return Value1.Value > Value2.ConvertCurrency(Value1.Culture).Value
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

    Public Shared Operator <(Value2 As Money, Value1 As Money) As Boolean
        Return Value1.Value < Value2.ConvertCurrency(Value1.Culture).Value
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

End Structure