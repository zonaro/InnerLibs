Imports System.Globalization

Imports InnerLibs



''' <summary>
''' Estrutura que representa valores em dinheiro de uma determinada <see cref="CultureInfo"/>. Utiliza uma API (http://fixer.io) para conversão de moedas.
''' </summary>
Public Structure Money

    Public Property Value As Decimal

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
        Me.Region = New RegionInfo(Culture.Name)
    End Sub

    ''' <summary>
    ''' Cria uma nova instancia de moeda
    ''' </summary>
    ''' <param name="Value">Valor</param>
    ''' <param name="ISOCurrencySymbol">Simbolo de moeda, ISO ou nome da cultura</param>
    Public Sub New(Value As Decimal, ISOCurrencySymbol As String, Optional CurrencySymbol As String = "")
        Me.Value = Value
        Me.Culture = GetCultureInfosByCurrencySymbol(ISOCurrencySymbol).FirstOrDefault
        Me.Region = New RegionInfo(Me.Culture.Name)
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
            Return Me.ToString(0)
        End Get
    End Property

    ''' <summary>
    ''' String do valor formatado como moeda
    ''' </summary>
    ''' <param name="Precision">Precisao de casas decimais</param>
    ''' <returns></returns>
    Public Overloads Function ToString(Precision As Integer) As String
        Dim c = Me.Culture
        If c.Equals(CultureInfo.InvariantCulture) Then
            c = CultureInfo.CurrentCulture
        End If
        Dim ss = CurrencySymbol & " " & If(Precision > 0, Me.Value.Slice(Precision.SetMinValue(2)), Me.Value).ToString(c)
        If Me.Value.HasDecimalPart Then
            ss = ss.TrimEnd("0")
        End If
        Return ss.IfBlank("0")
    End Function

    ''' <summary>
    ''' Simbolo de moeda
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property CurrencySymbol As String
        Get
            Return Me.Region.CurrencySymbol
        End Get
    End Property


    ''' <summary>
    ''' Simbolo de moeda utilizada em cambio (ISO)
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property ISOCurrencySymbol As String
        Get
            Return Me.Region.ISOCurrencySymbol
        End Get
    End Property


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


    Shared DefaultPattern As String = "{value} {money|moneys} and {cent|cents}"

    ''' <summary>
    ''' Região correspondente a essa moeda
    ''' </summary>
    ''' <returns></returns>

    Property Region As RegionInfo
    ''' <summary>
    ''' Cultura correspondente a esta moeda
    ''' </summary>
    ''' <returns></returns>

    Property Culture As CultureInfo


    Public Shared Operator &(Text As String, Value As Money) As String
        Return Text & Value.MoneyString
    End Operator

    Public Shared Operator &(Value As Money, Text As String) As String
        Return Value.MoneyString & Text
    End Operator

    Public Shared Operator Not(Value As Money)
        Return New Money(Value.Value * -1, Value.ISOCurrencySymbol)
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
        Return New Money(Value1 + Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator +(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 + Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator +(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 + Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator +(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 + Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator +(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 + Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator +(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 + Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator +(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 + Value2.Value, Value2.ISOCurrencySymbol)
    End Operator



    Public Shared Operator -(Value1 As Double, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator -(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 - Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator -(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator -(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 - Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator -(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator -(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 - Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator -(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 - Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator -(Value1 As Money, Value2 As Money) As Money
        Return New Money(Value1.Value - Value2.Value, Value1.ISOCurrencySymbol)
    End Operator

    Public Shared Operator *(Value1 As Double, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator *(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 * Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator *(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator *(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 * Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator *(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator *(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 * Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator *(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 * Value2.Value, Value2.ISOCurrencySymbol)
    End Operator



    Public Shared Operator /(Value1 As Double, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator /(Value2 As Money, Value1 As Double) As Money
        Return New Money(Value1 / Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator /(Value1 As Decimal, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator /(Value2 As Money, Value1 As Decimal) As Money
        Return New Money(Value1 / Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator /(Value1 As Integer, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator /(Value2 As Money, Value1 As Integer) As Money
        Return New Money(Value1 / Value2.Value, Value2.ISOCurrencySymbol)
    End Operator

    Public Shared Operator /(Value1 As Long, Value2 As Money) As Money
        Return New Money(Value1 / Value2.Value, Value2.ISOCurrencySymbol)
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
        Return Value1.CurrencySymbol = Value2.CurrencySymbol
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

    ''' <summary>
    ''' Compara se 2 valores são iguais (mesmo valor e moeda)
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    Public Overrides Function Equals(obj As Object) As Boolean
        If obj.GetType = GetType(Money) Then
            Return Me.Value = CType(obj, Money).Value AndAlso Me.ISOCurrencySymbol = CType(obj, Money).ISOCurrencySymbol
        Else
            Return False
        End If
    End Function
End Structure