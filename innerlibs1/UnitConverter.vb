
''' <summary>
''' Classe para manipulaçao de numeros e conversão unidades
''' </summary>
Public Class UnitConverter

    ''' <summary>
    ''' Unidades de Medida
    ''' </summary>
    ''' <returns></returns>
    Property Units As New Dictionary(Of Decimal, String)

    ''' <summary>
    ''' Inicia um <see cref="UnitConverter"/> vazio
    ''' </summary>
    Sub New()
    End Sub

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> de Base 1000
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateBase100() As UnitConverter
        Dim c = New UnitConverter(0.000000000000000000000001D, 1000, "y", "z", "a", "f", "p", "n", "µ", "m", "", "K", "M", "G", "T", "P", "E")
        Return c
    End Function

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> utilizando um <see cref="Dictionary(Of Decimal,String)"/> com as marcaçoes de unidade de medida
    ''' </summary>
    ''' <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    ''' <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
    Sub New(Units As Dictionary(Of Decimal, String))
        Me.Units = Units
    End Sub

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> utilizando um <see cref="Dictionary(Of String,Decimal)"/> com as marcaçoes de unidade de medida
    ''' </summary>
    ''' <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    ''' <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
    Sub New(Units As Dictionary(Of String, Decimal))
        Me.Units = Units.ToDictionary(Function(x) x.Value, Function(x) x.Key)
    End Sub

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> utilizando um numero inicial, uma base multiplicadora e um array com as unidades de medida
    ''' </summary>
    ''' <param name="StartAt">Numero Inicial</param>
    ''' <param name="Base">Base multiplicadora, exponencia o numero em <paramref name="StartAt"/> para cada item em <paramref name="Units"/></param>
    ''' <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    ''' <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
    Sub New(StartAt As Decimal, Base As Integer, ParamArray Units As String())
        Units = If(Units, {})
        Me.Units = New Dictionary(Of Decimal, String)
        Me.Units.Add(StartAt, Units.First)
        For index = 1 To Units.Length - 1
            StartAt = StartAt * Base
            Me.Units.Add(StartAt, Units(index))
        Next
    End Sub

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> utilizando uma base multiplicadora e um array com as unidades de medida começando pelo numero 1
    ''' </summary>
    ''' <param name="Base">Base multiplicadora, exponencia o numero 1 para cada item em <paramref name="Units"/></param>
    ''' <param name="Units">Unidades de medida. Permite uso de singular e plural separando as palavras com ";" </param>
    ''' <remarks>Utilize ponto e virgula (;) para separar unidades de medidas com singular;plural (EX.: Centimetro;Centimetros)</remarks>
    Sub New(Base As Integer, ParamArray Units As String())
        Me.New(1, Base, Units)
    End Sub

    ''' <summary>
    ''' Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    Function Abreviate(Number As Decimal, Optional DecimalPlaces As Integer = -1) As String
        Select Case Units.Count
            Case 0
                Return Number.Slice(DecimalPlaces)
            Case Else
                Dim k = Units.OrderBy(Function(x) x.Key).LastOrDefault(Function(x) x.Key <= Number)
                If k.Key = 0 Then
                    k = Units.OrderBy(Function(x) x.Key).First
                End If
                Number = (Number / k.Key).Slice(DecimalPlaces)
                Dim u = k.Value
                If u.Contains(";") Then
                    If Number = 1 Then
                        u = u.Split(";").First
                    Else
                        u = u.Split(";")(1)
                    End If
                End If
                Return Number & " " & u
        End Select
    End Function

    ''' <summary>
    ''' Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    Function Abreviate(Number As Integer) As String
        Return Abreviate(Number.ChangeType(Of Decimal))
    End Function

    ''' <summary>
    ''' Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    Function Abreviate(Number As Short) As String
        Return Abreviate(Number.ChangeType(Of Decimal))
    End Function

    ''' <summary>
    ''' Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    Function Abreviate(Number As Long) As String
        Return Abreviate(Number.ChangeType(Of Decimal))
    End Function

    ''' <summary>
    ''' Retorna o numero decimal a partir de uma string abreviada
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    Public Function Parse(Number As String, Optional DecimalPlaces As Integer = -1) As Decimal
        If Number.IsBlank Then
            Return 0
        End If
        If Not Number.IsNumber Then
            Dim i = Number
            While i.StartsWithAny(1, 2, 3, 4, 5, 6, 7, 8, 9, 0)
                i = i.RemoveFirstChars()
            End While

            Dim p = Units.SingleOrDefault(Function(x) x.Value.Split(";").Any(Function(y) y.Trim = i.Trim))
            If p.Key > 0 Then
                Return (p.Key * System.Convert.ToDecimal(Number.ParseDigits)).Slice(DecimalPlaces)
            Else
                Return Number.TrimAny(i).ChangeType(Of Decimal).Slice(DecimalPlaces)
            End If
        End If
        Return Number.ChangeType(Of Decimal).Slice(DecimalPlaces)
    End Function

    Public Function ParseUnit(Number As String) As String
        Dim i = Number
        While i.StartsWithAny(1, 2, 3, 4, 5, 6, 7, 8, 9, 0)
            i = i.RemoveFirstChars()
        End While
        Dim p = Units.SingleOrDefault(Function(x) x.Value.Split(";").Any(Function(y) y.Trim = i.Trim))
        Dim u = p.Value.IfBlank("")
        If u.Contains(";") Then
            If Number.ParseDigits = 1 Then
                u = u.Split(";").First
            Else
                u = u.Split(";")(1)
            End If
        End If
        Return u
    End Function

    ''' <summary>
    ''' Converte um numero   decimal em outro numero decimal a partir de unidades de medida
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <param name="From">Unidade de Medida de origem</param>
    ''' <param name="[To]">Unidade de medida de destino</param>
    ''' <returns></returns>
    Function Convert(Number As Decimal, From As String, [To] As String) As Decimal
        Return Convert(Number & From, [To])
    End Function

    ''' <summary>
    ''' Converte um numero abreviado em decimal 
    ''' </summary>
    ''' <param name="AbreviatedNumber">Numero abreviado</param>
    ''' <param name="[To]">Unidade de destino</param>
    ''' <returns></returns>
    Function Convert(AbreviatedNumber As String, [To] As String) As Decimal
        Dim nn = Parse(AbreviatedNumber) 'mm 
        Return nn / GetUnitBase([To])
    End Function

    ''' <summary>
    ''' Retorna a base a partir da unidade de medida
    ''' </summary>
    ''' <param name="U">Unidade de medida</param>
    ''' <returns></returns>
    Function GetUnitBase(U As String) As Decimal
        Return Units.SingleOrDefault(Function(x) x.Value.Split(";").Any(Function(y) y.Trim = U.Trim)).Key
    End Function

End Class
