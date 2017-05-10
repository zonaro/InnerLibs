Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices

Public Module Converter
    ''' <summary>
    ''' Converte um tipo para outro
    ''' </summary>
    ''' <typeparam name="T">Tipo</typeparam>
    ''' <param name="Value">Variavel com valor</param>
    ''' <returns>Valor convertido em novo tipo</returns>
    <Extension>
    Public Function [To](Of T)(Value As IConvertible) As T
        Try
            Dim a As Type = GetType(T)
            Dim u As Type = Nullable.GetUnderlyingType(a)

            If Not (u Is Nothing) Then
                If Value Is Nothing OrElse Value.Equals("") Then
                    Return Nothing
                End If
                Return DirectCast(Convert.ChangeType(Value, u), T)
            Else
                If Value Is Nothing OrElse Value.Equals("") Then
                    Return Nothing
                End If

                Return DirectCast(Convert.ChangeType(Value, a), T)
            End If

        Catch
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Converte um NameValueCollection para Dictionary
    ''' </summary>
    ''' <param name="[NameValueCollection]">Formulario</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToDictionary([NameValueCollection] As NameValueCollection) As Dictionary(Of String, Object)
        Dim result = New Dictionary(Of String, Object)()
        For Each key As String In [NameValueCollection].Keys
            Dim values As String() = [NameValueCollection].GetValues(key)
            If values.Length = 1 Then
                result.Add(key, values(0))
            Else
                result.Add(key, [NameValueCollection](key))
            End If
        Next
        Return result
    End Function

    ''' <summary>
    ''' Converte um NameValueCollection para string JSON
    ''' </summary>
    ''' <param name="[NameValueCollection]">Formulário</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToJSON([NameValueCollection] As NameValueCollection, Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
        Return NameValueCollection.ToDictionary.SerializeJSON(DateFormat)
    End Function

    ''' <summary>
    ''' COnverte os Valores de um Formulário enviado por GET ou POST em JSON
    ''' </summary>
    ''' <param name="Request">Request GET ou POST</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToJSON(Request As System.Web.HttpRequest, Optional DateFormat As String = "yyyy-MM-dd hh:mm:ss") As String
        Return Request.Form.ToJSON(DateFormat)
    End Function
End Module