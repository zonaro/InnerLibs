Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices

Public Module Converter

    ''' <summary>
    ''' Converte uma lista de dicionários para uma tabela HTML
    ''' </summary>
    ''' <param name="Table"></param>
    ''' <returns></returns>
    <Extension> Public Function ToHtmlTable(Table As List(Of IDictionary(Of String, Object))) As HtmlTag
        Dim body = ""
        Table = Table.Uniform
        For Each dic In Table
            Dim l As New List(Of String)
            For Each k In dic.Keys.ToArray
                l.Add(dic(k))
            Next
            body.Append(TableGenerator.TableRow("", l.ToArray))
        Next
        body = TableGenerator.Table(TableHeader(Table.First.Keys.ToArray), body)
        Return New HtmlTag(body)
    End Function

    <Extension()> Function Uniform(Of TKey, TValue)(Dics As List(Of IDictionary(Of TKey, TValue))) As List(Of IDictionary(Of TKey, TValue))
        Dim templist = New List(Of IDictionary(Of TKey, TValue))(Dics)
        Dim colunas As New List(Of TKey)
        For Each dic In templist
            colunas.AddRange(dic.Keys.ToArray)
        Next
        colunas = colunas.Distinct.ToList
        For index = 0 To templist.LongCount - 1
            Dim tempdic As New SortedDictionary(Of TKey, TValue)
            For Each col In colunas
                Try
                    tempdic(col) = templist(index)(col)
                Catch ex As Exception
                    tempdic(col) = Nothing
                End Try
            Next
            templist(index) = New SortedDictionary(Of TKey, TValue)(tempdic)
        Next
        Return templist
    End Function

    Function Uniform(Of TKey, TValue)(ParamArray Dics As IDictionary(Of TKey, TValue)()) As List(Of IDictionary(Of TKey, TValue))
        Dim l As New List(Of IDictionary(Of TKey, TValue))
        l = Uniform(Dics.ToList())
        Return l
    End Function

    ''' <summary>
    ''' Converte um tipo para outro
    ''' </summary>
    ''' <typeparam name="T">Tipo</typeparam>
    ''' <param name="Value">Variavel com valor</param>
    ''' <returns>Valor convertido em novo tipo</returns>
    <Extension>
    Public Function [To](Of T)(Value As Object) As T
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
        Dim d As New Dictionary(Of String, Object)
        d.Add("QueryString", Request.QueryString.ToDictionary)
        d.Add("Form", Request.Form.ToDictionary)
        Return d.SerializeJSON(DateFormat)
    End Function

End Module