Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Web
Imports InnerLibs.HtmlParser

Public Module Converter

    ''' <summary>
    ''' Unidades de medida de yocto a quintilhão
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Units As Dictionary(Of String, Decimal)
        Get
            Dim sizeTypes As New Dictionary(Of String, Decimal)
            sizeTypes.Add("y", 1.0E-24)
            sizeTypes.Add("z", 1.0E-21)
            sizeTypes.Add("a", 1.0E-18)
            sizeTypes.Add("f", 0.000000000000001)
            sizeTypes.Add("p", 0.000000000001)
            sizeTypes.Add("n", 0.000000001)
            sizeTypes.Add("µ", 0.000001)
            sizeTypes.Add("m", 0.001)
            sizeTypes.Add("", 1)
            sizeTypes.Add("K", 1000)
            sizeTypes.Add("M", 1000000)
            sizeTypes.Add("G", 1000000000)
            sizeTypes.Add("T", 1000000000000)
            sizeTypes.Add("P", 1000000000000000)
            sizeTypes.Add("E", 1000000000000000000)
            Return sizeTypes
        End Get
    End Property

    ''' <summary>
    ''' Converte um numero na sua forma abreviada para um tipo numérico
    ''' </summary>
    ''' <param name="Number"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ParseUnitString(Number As String) As Decimal
        Dim i = Number.AdjustWhiteSpaces.GetLastChars(1)
        If Units.ContainsKey(i) Then
            Return Units(i) * Convert.ToDecimal(Number.ParseDigits)
        Else
            Return Number.TrimAny(i)
        End If
    End Function

    ''' <summary>
    ''' Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto
    ''' </summary>
    ''' <param name="Obj">Objeto</param>
    ''' <returns></returns>
    Public Function ForceArray(Obj As Object) As Object()
        Return ForceArray(Of Object)(Obj)
    End Function

    ''' <summary>
    ''' Verifica se um objeto é um array, e se negativo, cria um array de um unico item com o valor do objeto
    ''' </summary>
    ''' <param name="Obj">Objeto</param>
    ''' <returns></returns>
    Public Function ForceArray(Of Type)(ByVal Obj As Object) As Type()
        Dim a As New List(Of Type)
        If IsNothing(Obj) Then Return a.ToArray
        If Not IsArray(Obj) Then
            If Obj.ToString.IsBlank Then Obj = {} Else Obj = {Obj}
        End If
        Return Array.ConvertAll(Of Object, Type)(Obj, Function(x) CType(x, Type))
    End Function

    ''' <summary>
    ''' Converte uma lista de dicionários para uma tabela HTML
    ''' </summary>
    ''' <param name="Table"></param>
    ''' <returns></returns>
    <Extension> Public Function ToHtmlTable(Table As List(Of IDictionary(Of String, Object))) As HtmlElement
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
        Return New HtmlElement(body)
    End Function

    ''' <summary>
    ''' Aplica as mesmas keys a todos os dicionarios de uma lista
    ''' </summary>
    ''' <typeparam name="TKey">Tipo da key</typeparam>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Dics">Dicionarios</param>
    ''' <returns></returns>
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

    ''' <summary>
    ''' Aplica as mesmas keys a todos os dicionarios de uma lista
    ''' </summary>
    ''' <typeparam name="TKey">Tipo da key</typeparam>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Dics">Dicionarios</param>
    ''' <returns></returns>
    Function Uniform(Of TKey, TValue)(ParamArray Dics As IDictionary(Of TKey, TValue)()) As List(Of IDictionary(Of TKey, TValue))
        Dim l As New List(Of IDictionary(Of TKey, TValue))
        l = Uniform(Dics.ToList())
        Return l
    End Function

    ''' <summary>
    ''' Converte um tipo para outro
    ''' </summary>
    ''' <typeparam name="ToType">Tipo</typeparam>
    ''' <param name="Value">Variavel com valor</param>
    ''' <returns>Valor convertido em novo tipo</returns>
    <Extension>
    Public Function ChangeType(Of ToType, FromType)(Value As FromType) As ToType
        Try
            Dim a As Type = GetType(ToType)
            Dim u As Type = Nullable.GetUnderlyingType(a)

            If Not (u Is Nothing) Then
                If Value Is Nothing OrElse Value.Equals("") Then
                    Return Nothing
                End If
                Return CType(Convert.ChangeType(Value, u), ToType)
            Else
                If Value Is Nothing OrElse Value.Equals("") Then
                    Return Nothing
                End If
                Return CType(Convert.ChangeType(Value, a), ToType)
            End If
        Catch ex As Exception
            Debug.WriteLine(ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Converte um array de um tipo para outro
    ''' </summary>
    ''' <typeparam name="ToType">Tipo do array</typeparam>
    ''' <param name="Value">Array com elementos</param>
    ''' <returns>Array convertido em novo tipo</returns>
    <Extension>
    Public Function ChangeArrayType(Of ToType, FromType)(Value As FromType()) As ToType()
        Dim d As New List(Of ToType)
        For Each el As FromType In Value
            d.Add(el.ChangeType(Of ToType))
        Next
        Return d.ToArray
    End Function


    <Extension()> Function Merge(Of Tkey)(Dic1 As Dictionary(Of Tkey, Object), ParamArray Dics As Dictionary(Of Tkey, Object)()) As Dictionary(Of Tkey, Object)
        Dim result = New Dictionary(Of Tkey, Object)()
        Dim keys As New List(Of Tkey)
        For Each k In Dic1.Keys
            keys.Add(k)
        Next
        For Each dic In Dics
            For Each k In dic.Keys
                keys.Add(k)
            Next
        Next

        For Each dic In Dics
            For Each key As Object In keys
                If keys.Contains(key) Then
                    Dim values = dic.Values.ToArray
                    If result.ContainsKey(key) Then
                        Dim l As New List(Of Object)
                        If IsArray(result(key)) Then
                            For Each v In result(key)
                                l.Add(v)
                            Next
                        Else
                            l.Add(result(key))
                        End If
                        If l.Count = 1 Then
                            result(key) = l(0)
                        Else
                            result(key) = l.ToArray
                        End If
                    Else
                        If values.LongCount = 1 Then
                            result.Add(key, values(0))
                        Else
                            Dim ar As New List(Of Object)
                            For Each v In values
                                ar.Add(v)
                            Next
                            result.Add(key, ar.ToArray)
                        End If
                    End If

                End If
            Next
        Next

        Return result

    End Function

    ''' <summary>
    ''' Returna um <see cref=" Dictionary"/> a partir de um <see cref="IGrouping(Of TKey, TElement)"/>
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="groupings"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToDictionary(Of TKey, TValue)(ByVal groupings As IEnumerable(Of IGrouping(Of TKey, TValue))) As Dictionary(Of TKey, List(Of TValue))
        Return groupings.ToDictionary(Function(group) group.Key, Function(group) group.ToList())
    End Function

    ''' <summary>
    ''' Transforma um <see cref="HttpRequest"/> em um <see cref="Dictionary(Of String, Object)"/>
    ''' </summary>
    ''' <param name="Request">HttpRequest</param>
    ''' <param name="Keys">Keys que devem ser incluidas</param>
    ''' <returns></returns>
    <Extension()> Public Function ToDictionary(Request As HttpRequest, ParamArray keys As String()) As Dictionary(Of String, Object)
        If IsNothing(keys) OrElse keys.LongCount = 0 Then
            Dim l As New List(Of String)
            l.AddRange(Request.Form.AllKeys)
            l.AddRange(Request.Files.AllKeys)
            l.AddRange(Request.QueryString.AllKeys)
            keys = l.Distinct.ToArray
        End If
        Dim result = Request.QueryString.ToDictionary(keys)
        Dim result2 = Request.Form.ToDictionary(keys)
        result = result.Merge(result2)
        For Each f As String In Request.Files.AllKeys.Select(Function(k) k.IsIn(keys))
            result(f) = Request.Files(f).ToBytes
        Next
        Return result
    End Function






    ''' <summary>
    ''' Converte um NameValueCollection para um <see cref="Dictionary(Of String, Object)"/>
    ''' </summary>
    ''' <param name="[NameValueCollection]">Formulario</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToDictionary([NameValueCollection] As NameValueCollection, ParamArray Keys As String()) As Dictionary(Of String, Object)
        Dim result = New Dictionary(Of String, Object)()
        If IsNothing(Keys) OrElse Keys.LongCount = 0 Then Keys = NameValueCollection.AllKeys
        For Each key As String In [NameValueCollection].Keys
            If key.IsIn(Keys) Then
                Dim values As String() = [NameValueCollection].GetValues(key)
                If result.ContainsKey(key) Then
                    Dim l As New List(Of Object)
                    If IsArray(result(key)) Then
                        For Each v In result(key)
                            Select Case True
                                Case Verify.IsNumber(v)
                                    l.Add(Convert.ToDouble(v))
                                    Exit Select

                                Case IsDate(v)
                                    l.Add(Convert.ToDateTime(v))
                                    Exit Select

                                Case Else
                                    l.Add(v)
                            End Select
                        Next
                    Else
                        Select Case True
                            Case Verify.IsNumber(result(key))
                                l.Add(Convert.ToDouble(result(key)))
                                Exit Select

                            Case IsDate(result(key))
                                Exit Select

                                l.Add(Convert.ToDateTime(result(key)))
                            Case Else
                                l.Add(result(key))
                        End Select
                    End If
                    If l.Count = 1 Then
                        result(key) = l(0)
                    Else
                        result(key) = l.ToArray
                    End If
                Else
                    If values.Length = 1 Then
                        Select Case True
                            Case Verify.IsNumber(values(0))
                                result.Add(key, Convert.ToDouble(values(0)))
                                Exit Select
                            Case IsDate(values(0))
                                result.Add(key, Convert.ToDateTime(values(0)))
                                Exit Select
                            Case Else
                                result.Add(key, values(0))
                        End Select
                    Else
                        Dim ar As New List(Of Object)
                        For Each v In values
                            Select Case True
                                Case Verify.IsNumber(v)
                                    ar.Add(Convert.ToDouble(v))
                                    Exit Select

                                Case IsDate(v)
                                    ar.Add(Convert.ToDateTime(v))
                                    Exit Select

                                Case Else
                                    ar.Add(v)
                            End Select
                        Next
                        result.Add(key, ar.ToArray)
                    End If
                End If

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