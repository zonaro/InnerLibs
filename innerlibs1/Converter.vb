Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Web
Imports InnerLibs.HtmlParser
Imports InnerLibs.LINQ

Public Module Converter


    ''' <summary>
    ''' Cria uma lista vazia usando um objeto como o tipo da lista. Util para tipos anonimos
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="ObjectForDefinition">Objeto que definirá o tipo da lista</param>
    ''' <returns></returns>
    <Extension()> Function DefineEmptyList(Of T)(ObjectForDefinition As T) As List(Of T)
        Return New List(Of T)
    End Function

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
        If Number.IsBlank Then
            Return 0
        End If
        If Not Number.IsNumber Then
            Dim i = Number.AdjustWhiteSpaces.GetLastChars(1)
            If Units.ContainsKey(i) Then
                Return Units(i) * Convert.ToDecimal(Number.ParseDigits)
            Else
                Return Number.TrimAny(i).ChangeType(Of Decimal)
            End If
        End If
        Return Number.ChangeType(Of Decimal)
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
    Public Function ForceArray(Of OutputType)(ByVal Obj As Object) As OutputType()
        Dim a As New List(Of OutputType)
        If IsNothing(Obj) Then Return a.ToArray
        If Not IsArray(Obj) Then
            If Obj.ToString.IsBlank Then Obj = {} Else Obj = {Obj}
        End If
        Return Array.ConvertAll(Of Object, OutputType)(Obj, Function(x) CType(x, OutputType))
    End Function


    ''' <summary>
    ''' Converte uma lista de dicionários para uma tabela HTML
    ''' </summary>
    ''' <param name="Table"></param>
    ''' <returns></returns>
    <Extension> Public Function ToHtmlTable(Table As IEnumerable(Of Dictionary(Of String, Object))) As HtmlElement
        Dim body = ""
        Table.Uniform
        For Each dic In Table
            Dim l As New List(Of String)
            For Each k In dic.Keys.ToArray
                l.Add(dic(k))
            Next
            body.Append(TableGenerator.TableRow("", l.ToArray))
        Next
        body = TableGenerator.Table(TableHeader(Table.First.Keys.ToArray), body)
        Return New HtmlElement("table", body)
    End Function


    ''' <summary>
    ''' Aplica as mesmas keys a todos os dicionarios de uma lista
    ''' </summary>
    ''' <typeparam name="TKey">Tipo da key</typeparam>
    ''' <typeparam name="TValue">Tipo do Valor</typeparam>
    ''' <param name="Dics">Dicionarios</param>
    '''<param name="AditionalKeys">Chaves para serem incluidas nos dicionários mesmo se não existirem em nenhum deles</param>
    <Extension()> Sub Uniform(Of TKey, TValue)(ByRef Dics As IEnumerable(Of Dictionary(Of TKey, TValue)), ParamArray AditionalKeys As TKey())
        AditionalKeys = If(AditionalKeys, {})
        Dim chave = Dics.SelectMany(Function(x) x.Keys).Distinct.Union(AditionalKeys)
        For Each dic In Dics
            For Each key In chave
                If Not dic.ContainsKey(key) Then
                    dic(key) = Nothing
                End If
            Next
        Next
    End Sub

    ''' <summary>
    ''' Converte um tipo para outro. Retorna Nothing (NULL) se a covnersão falhar
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
        If Value.Count > 0 Then
            For Each el As FromType In Value
                d.Add(el.ChangeType(Of ToType))
            Next
            Return d.ToArray
        End If
        Return {}
    End Function

    ''' <summary>
    ''' Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um dicionario os valores sao agrupados em arrays
    ''' </summary>
    ''' <typeparam name="Tkey">Tipo da Key, Deve ser igual para todos os dicionarios</typeparam>
    ''' <param name="FirstDictionary">Dicionario Principal</param>
    ''' <param name="Dictionaries">Outros dicionarios</param>
    ''' <returns></returns>

    <Extension()> Function Merge(Of Tkey)(FirstDictionary As Dictionary(Of Tkey, Object), ParamArray Dictionaries As Dictionary(Of Tkey, Object)()) As Dictionary(Of Tkey, Object)

        'dicionario que está sendo gerado a partir dos outros
        Dim result As New Dictionary(Of Tkey, Object)

        'adiciona o primeiro dicionario ao array principal e exclui dicionarios vazios
        Dictionaries = Dictionaries.Union({FirstDictionary}).Where(Function(x) x.Count > 0).ToArray

        'cria um array de keys unicas a partir de todos os dicionarios
        Dim keys = Dictionaries.SelectMany(Function(x) x.Keys.ToArray).Distinct

        'para cada chave encontrada
        For Each key In keys
            'para cada dicionario a ser mesclado
            For Each dic In Dictionaries
                'dicionario tem a chave?
                If dic.ContainsKey(key) Then
                    'resultado ja tem a chave atual adicionada?
                    If result.ContainsKey(key) Then
                        'lista que vai mesclar tudo
                        Dim lista As New List(Of Object)

                        'chave do resultado é um array?
                        If IsArray(result(key)) Then
                            lista.AddRange(result(key))
                        Else
                            lista.Add(result(key))
                        End If
                        'chave do dicionario é um array?
                        If IsArray(dic(key)) Then
                            lista.AddRange(dic(key))
                        Else
                            lista.Add(dic(key))
                        End If

                        'transforma a lista em um resultado
                        If lista.Count > 0 Then
                            If lista.Count > 1 Then
                                result(key) = lista.ToArray
                            Else
                                result(key) = lista.First
                            End If
                        End If
                    Else
                        If dic(key).GetType IsNot GetType(String) AndAlso (IsArray(dic(key)) OrElse IsList(dic(key))) Then
                            result.Add(key, dic(key).ToArray)
                        Else
                            result.Add(key, dic(key))
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
        For Each f As String In Request.Files.AllKeys.Where(Function(k) k.IsIn(keys))
            If Request.Files(f).ContentLength > 0 Then
                result(f) = Request.Files(f).ToBytes
            End If
        Next
        Return result
    End Function

    ''' <summary>
    ''' Seta as propriedades de uma classe a partir de um dictionary
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Dic"></param>
    ''' <param name="Obj"></param>
    <Extension()>
    Public Sub SetPropertiesIn(Of T As Class)(Dic As IDictionary(Of String, Object), Obj As T)
        For Each k In Dic
            If Obj.HasProperty(k.Key) Then
                Obj.SetPropertyValue(k.Key, k.Value)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Seta as propriedades de uma classe a partir de um HttpRequest
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Request"></param>
    ''' <param name="Obj"></param>
    <Extension()>
    Public Function SetPropertiesIn(Of T As Class)(Request As HttpRequest, ByRef Obj As T, ParamArray Keys As String()) As T
        Obj = If(Obj, Activator.CreateInstance(Of T))
        Request.ToDictionary(Keys).SetPropertiesIn(Of T)(Obj)
        Return Obj
    End Function

    ''' <summary>
    ''' Transforma uma lista de pares em um Dictionary
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="TValue"></typeparam>
    ''' <param name="items"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToDictionary(Of TKey, TValue)(items As IEnumerable(Of KeyValuePair(Of TKey, TValue))) As Dictionary(Of TKey, TValue)
        Return items.DistinctBy(Function(x) x.Key).ToDictionary(Of TKey, TValue)(Function(x) x.Key, Function(x) x.Value)
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
            If key.IsNotBlank AndAlso key.IsIn(Keys) Then
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
    Public Function ToJSON([NameValueCollection] As NameValueCollection) As String
        Return Json.SerializeJSON(NameValueCollection.ToDictionary)
    End Function

    ''' <summary>
    ''' COnverte os Valores de um Formulário enviado por GET ou POST em JSON
    ''' </summary>
    ''' <param name="Request">Request GET ou POST</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToJSON(Request As System.Web.HttpRequest) As String
        Dim d As New Dictionary(Of String, Object)
        d.Add("QueryString", Request.QueryString.ToDictionary)
        d.Add("Form", Request.Form.ToDictionary)
        Return Json.SerializeJSON(d)
    End Function

End Module