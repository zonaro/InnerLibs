Imports System.Globalization

''' <summary>
''' Classe para manipulaçao de numeros e conversão unidades
''' </summary>
Public Class UnitConverter

    Private Units As New Dictionary(Of Decimal, String)

    Property UnitComparisonType As StringComparison = StringComparison.Ordinal

    Property Culture As CultureInfo = CultureInfo.InvariantCulture

    ''' <summary>
    ''' Inicia um <see cref="UnitConverter"/> vazio
    ''' </summary>
    Sub New()
    End Sub

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> de Base 1000 (de y a E)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateBase1000Converter() As UnitConverter
        Return New UnitConverter(1000, 0.000000000000000000000001D, "y", "z", "a", "f", "p", "n", "µ", "m", "", "K", "M", "G", "T", "P", "E")
    End Function

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> de de Massa (peso) complexos de base 10 (de mg a kg)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateComplexMassConverter() As UnitConverter
        Return New UnitConverter(10, "mg", "cg", "dg", "g", "dag", "hg", "kg") With {.UnitComparisonType = StringComparison.OrdinalIgnoreCase}
    End Function

    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> de de Massa (peso) simples de base 1000 (de mg a T)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateSimpleMassConverter() As UnitConverter
        Return New UnitConverter(1000, "mg", "g", "kg", "T") With {.UnitComparisonType = StringComparison.OrdinalIgnoreCase}
    End Function



    ''' <summary>
    ''' Cria um <see cref="UnitConverter"/> de Base 1024 (Bytes) de (B a EB)
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function CreateFileSizeConverter() As UnitConverter
        Return New UnitConverter(1024, "B", "KB", "MB", "GB", "TB", "PB", "EB") With {.UnitComparisonType = StringComparison.OrdinalIgnoreCase}
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
    Sub New(Base As Decimal, StartAt As Decimal, ParamArray Units As String())
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
        Me.New(Base, 1, Units)
    End Sub

    ''' <summary>
    ''' Abrevia um numero com a unidade mais alta encontrada dentro do conversor
    ''' </summary>
    ''' <param name="Number">Numero</param>
    ''' <returns></returns>
    Function Abreviate(Number As Decimal, DecimalPlaces As Integer) As String
        Select Case Units.Count
            Case 0
                Return Number.Slice(DecimalPlaces).ToString
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
                Dim abr = Number.ToString(Culture)
                If abr.Contains(Culture.NumberFormat.NumberDecimalSeparator) Then
                    abr = abr.Trim("0")
                    abr = abr.Trim(Culture.NumberFormat.NumberDecimalSeparator)
                End If
                Return (abr & " " & u).Trim
        End Select
    End Function

    Function Abreviate(Number As Decimal) As String
        Return Abreviate(Number, Culture.NumberFormat.NumberDecimalDigits)
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
            While i.StartsWithAny(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, ",", ".")
                i = i.RemoveFirstChars()
            End While
            Dim p = GetUnit(i)
            If p.Key > 0 Then
                Return (p.Key * Number.ParseDigits.ChangeType(Of Decimal)).Slice(DecimalPlaces)
            Else
                Return Number.ParseDigits.ChangeType(Of Decimal).Slice(DecimalPlaces)
            End If
        End If
        Return Number.ChangeType(Of Decimal).Slice(DecimalPlaces)
    End Function

    ''' <summary>
    ''' Extrai a Unidade utilizada a partir de um numero abreviado
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    Public Function ParseUnit(Number As String) As String
        Dim i = Number
        While i.StartsWithAny(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, ",", ".")
            i = i.RemoveFirstChars()
        End While
        Dim p = GetUnit(i)
        Dim u = p.Value.IfBlank("")
        If u.Contains(";") Then
            If Number.ParseDigits.IfBlank(1) = 1 Then
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
    Function Convert(Number As Decimal, [To] As String, From As String) As Decimal
        Return Convert(Number & From, [To])
    End Function

    ''' <summary>
    ''' Converte um numero abreviado em decimal
    ''' </summary>
    ''' <param name="AbreviatedNumber">Numero abreviado</param>
    ''' <param name="[To]">Unidade de destino</param>
    ''' <returns></returns>
    Function Convert(AbreviatedNumber As String, [To] As String) As Decimal
        Dim nn = Parse(AbreviatedNumber)
        Return nn / GetUnit([To]).Key
    End Function

    ''' <summary>
    ''' Converte um numero abreviado em outro numero abreviado de outra unidade
    ''' </summary>
    ''' <param name="AbreviatedNumber">Numero abreviado</param>
    ''' <param name="[To]">Unidade de destino</param>
    ''' <returns></returns>
    Function ConvertAbreviate(AbreviatedNumber As String, [To] As String) As String
        Dim nn = Parse(AbreviatedNumber)
        Return ((nn / GetUnit([To]).Key) & " " & [To]).Trim
    End Function

    ''' <summary>
    ''' Retorna a unidade e a base a partir do nome da unidade
    ''' </summary>
    ''' <param name="U"></param>
    ''' <returns></returns>
    Private Function GetUnit(U As String) As KeyValuePair(Of Decimal, String)
        If U.IsBlank Then
            Return Units.SingleOrDefault(Function(x) x.Value.IsBlank)
        Else
            Return Units.SingleOrDefault(Function(x) x.Value.Split(";").Any(Function(y) y.Trim.Equals(U.Trim, UnitComparisonType)))
        End If
    End Function

End Class