Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Drawing.Text
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports InnerLibs.HtmlParser
Imports InnerLibs.LINQ

Public Module ClassTools


    ''' <summary>
    ''' Concatena todas as  <see cref="Exception.InnerException"/> em uma única string
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToFullExceptionString(ex As Exception) As String
        Dim ExceptionString = ex.Message
        While ex.InnerException IsNot Nothing
            ex = ex.InnerException
            ExceptionString &= " >> " & ex.Message
        End While
        Return ExceptionString
    End Function

    ''' <summary>
    ''' Retorna um dicionário em QueryString
    ''' </summary>
    ''' <param name="Dic"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToQueryString(Dic As Dictionary(Of String, String)) As String
        Dim param As String = ""
        For Each k In Dic
            param.Append("&" & k.Key & "=" & HttpUtility.UrlEncode("" & k.Value))
        Next
        Return param
    End Function

    ''' <summary>
    ''' Retorna a propriedade de um objeto como um parametro de query string
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Obj"></param>
    ''' <param name="PropertyNames"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetPropertyAsQueryStringParameter(Of T, TProperty)(Obj As T, ParamArray PropertyNames As Expressions.Expression(Of Func(Of T, TProperty))()) As String
        Dim txt = ""
        PropertyNames = If(PropertyNames, {})
        Dim props = Obj.GetProperties.AsEnumerable
        For Each propertyname In PropertyNames
            Dim prop = Obj.GetPropertyInfo(propertyname)
            txt &= "&" & prop.Name & "=" & ToFlatString(prop.GetValue(Obj)).UrlEncode
        Next
        Return txt
    End Function

    ''' <summary>
    ''' Retorna a propriedade de um objeto como um parametro de query string
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Obj"></param>
    ''' <param name="PropertyNames"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetPropertyAsQueryStringParameter(Of T)(Obj As T, ParamArray PropertyNames As String()) As String
        Dim txt = ""
        PropertyNames = If(PropertyNames, {})
        Dim props = Obj.GetProperties.AsEnumerable
        For Each propertyname In PropertyNames
            If props.Count(Function(x) x.Name = propertyname) > 0 Then
                txt &= "&" & propertyname & "=" & ToFlatString(Obj.GetPropertyValue(propertyname)).UrlEncode
            End If
        Next
        Return txt
    End Function

    ''' <summary>
    ''' Retorna a propriedade de varios objetos em uma lista como um parametro de query string
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Obj"></param>
    ''' <param name="PropertyNames"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetPropertyAsQueryStringParameter(Of T)(Obj As IEnumerable(Of T), ParamArray PropertyNames As String()) As String
        Dim txt = ""
        For Each i In Obj
            txt &= i.GetPropertyAsQueryStringParameter(PropertyNames)
        Next
        Return txt
    End Function




    ''' <summary>
    ''' Remove um item de uma lista e retorna este item
    ''' </summary>
    ''' <typeparam name="T">Tipo do item</typeparam>
    ''' <param name="List">Lista</param>
    ''' <param name="Index">Posicao do item</param>
    ''' <returns></returns>
    <Extension()> Public Function Detach(Of T)(List As List(Of T), Index As Integer) As T
        Dim p = List.IfBlankOrNoIndex(Index, Nothing)
        If p IsNot Nothing Then
            List.RemoveAt(Index)
        End If
        Return p
    End Function

    ''' <summary>
    ''' Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada grupo a partir de outra propriedade deo mesmo objeto
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <typeparam name="Group"></typeparam>
    ''' <typeparam name="Count"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="GroupSelector"></param>
    ''' <param name="CountObjectBy"></param>
    ''' <returns></returns>
    <Extension()> Public Function GroupAndCountBy(Of Type, Group, Count)(obj As IEnumerable(Of Type), GroupSelector As Func(Of Type, Group), CountObjectBy As Func(Of Type, Count)) As Dictionary(Of Group, Dictionary(Of Count, Long))
        Dim dic_of_dic = obj.GroupBy(GroupSelector).Select(Function(x) New KeyValuePair(Of Group, Dictionary(Of Count, Long))(x.Key, x.GroupBy(CountObjectBy).ToDictionary(Function(y) y.Key, Function(y) y.LongCount))).ToDictionary()
        dic_of_dic.Values.Uniform
        Return dic_of_dic
    End Function

    ''' <summary>
    ''' Agrupa itens de uma lista a partir de duas propriedades de um objeto resultado em um grupo com subgrupos daquele objeto
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <typeparam name="Group"></typeparam>
    ''' <typeparam name="SubGroup"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="GroupSelector"></param>
    ''' <param name="SubGroupSelector"></param>
    ''' <returns></returns>
    <Extension()> Public Function GroupAndSubGroupBy(Of Type, Group, SubGroup)(obj As IEnumerable(Of Type), GroupSelector As Func(Of Type, Group), SubGroupSelector As Func(Of Type, SubGroup)) As Dictionary(Of Group, Dictionary(Of SubGroup, IEnumerable(Of Type)))
        Dim dic_of_dic = obj.GroupBy(GroupSelector).Select(Function(x) New KeyValuePair(Of Group, Dictionary(Of SubGroup, IEnumerable(Of Type)))(x.Key, x.GroupBy(SubGroupSelector).ToDictionary(Function(y) y.Key, Function(y) y.AsEnumerable))).ToDictionary()
        dic_of_dic.Values.Uniform
        Return dic_of_dic
    End Function



    ''' <summary>
    ''' Copia os valores de um dicionário para as propriedades de uma classe
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <typeparam name="v"></typeparam>
    ''' <param name="Dic"></param>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    <Extension()> Public Function Map(Of T As Class, v)(Dic As Dictionary(Of String, v), Optional ByRef Obj As T = Nothing) As T
        Obj = If(Obj, Activator.CreateInstance(Of T))
        Dim props = Obj.GetProperties
        For Each k In Dic
            If Obj.HasProperty(k.Key) Then
                Obj.SetPropertyValue(k.Key, Conversion.CTypeDynamic(k.Value, props.SingleOrDefault(Function(i) i.Name = k.Key).PropertyType))
            End If
        Next
        Return Obj
    End Function

    ''' <summary>
    ''' Mapeia os objetos de um datareader para uma classe
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Reader"></param>
    ''' <returns></returns>
    <Extension()> Public Function Map(Of T)(Reader As DbDataReader, ParamArray Params As Object()) As List(Of T)
        Dim l As New List(Of T)
        Params = If(Params, {})
        While Reader.Read
            Dim d As T
            If Params.Count > 0 Then
                d = Activator.CreateInstance(GetType(T), Params)
            Else
                d = Activator.CreateInstance(Of T)()
            End If

            If GetType(T) = GetType(Dictionary(Of String, Object)) Then
                For i As Integer = 0 To Reader.FieldCount - 1
                    CType(CType(d, Object), Dictionary(Of String, Object)).Add(Reader.GetName(i), Reader(Reader.GetName(i)))
                Next
            Else
                For i As Integer = 0 To Reader.FieldCount - 1
                    If d.HasProperty(Reader.GetName(i)) Then
                        d.SetPropertyValue(Reader.GetName(i), Reader(Reader.GetName(i)))
                    End If
                Next
            End If
            l.Add(d)
        End While
        Return l
    End Function

    ''' <summary>
    ''' Adiciona uma fonte a uma PrivateFontCollection a partir de um Resource
    ''' </summary>
    ''' <param name="FontCollection">Colecao</param>
    <Extension()> Public Sub AddFontFromBytes(ByRef FontCollection As PrivateFontCollection, FontBytes As Byte())
        Dim fontData = Marshal.AllocCoTaskMem(FontBytes.Length)
        Marshal.Copy(FontBytes, 0, fontData, FontBytes.Length)
        FontCollection.AddMemoryFont(fontData, FontBytes.Length)
        Marshal.FreeCoTaskMem(fontData)
    End Sub

    ''' <summary>
    ''' Adiciona uma fonte a uma PrivateFontCollection a partir de um Resource
    ''' </summary>
    ''' <param name="FontCollection">Colecao</param>
    ''' <param name="FileName">      Nome do arquivo da fonte</param>
    <Extension()> Public Sub AddFontFromResource(ByRef FontCollection As PrivateFontCollection, Assembly As Assembly, FileName As String)
        Dim fontBytes = Assembly.GetResourceBytes(FileName)
        Dim fontData = Marshal.AllocCoTaskMem(fontBytes.Length)
        Marshal.Copy(fontBytes, 0, fontData, fontBytes.Length)
        FontCollection.AddMemoryFont(fontData, fontBytes.Length)
        Marshal.FreeCoTaskMem(fontData)
    End Sub

    ''' <summary>
    ''' Retorna um valor de um tipo especifico de acordo com um valor boolean
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Bool">      Valor boolean</param>
    ''' <param name="TrueValue"> Valor se verdadeiro</param>
    ''' <param name="FalseValue">valor se falso</param>
    ''' <returns></returns>
    <Extension()>
    Public Function AsIf(Of T)(Bool As Boolean, TrueValue As T, Optional FalseValue As T = Nothing) As T
        Return Bool.Choose(Of T)(TrueValue, FalseValue)
    End Function

    ''' <summary>
    ''' Retorna um valor de um tipo especifico de acordo com um valor boolean
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Bool">      Valor boolean</param>
    ''' <param name="TrueValue"> Valor se verdadeiro</param>
    ''' <param name="FalseValue">valor se falso</param>
    ''' <returns></returns>
    <Extension()>
    Public Function AsIf(Of T)(Bool As Boolean?, TrueValue As T, Optional FalseValue As T = Nothing) As T
        If Bool.HasValue Then
            Return Bool.Value.AsIf(TrueValue, FalseValue)
        Else
            Return FalseValue
        End If
    End Function

    ''' <summary>
    ''' Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que
    ''' possuir um valor
    ''' </summary>
    ''' <param name="First">Primeiro Item</param>
    ''' <param name="N">    Outros itens</param>
    ''' <returns></returns>
    <Extension> Public Function BlankCoalesce(First As String, ParamArray N() As String) As String
        Dim l As New List(Of String)
        l.Add(First)
        l.AddRange(N)
        Return BlankCoalesce(l.ToArray)
    End Function

    ''' <summary>
    ''' Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento que
    ''' possuir um valor
    ''' </summary>
    ''' <param name="N">Itens</param>
    ''' <returns></returns>
    Public Function BlankCoalesce(ParamArray N() As String) As String
        For Each i In If(N, {})
            If i.IsNotBlank Then
                Return i
            End If
        Next
        Return ""
    End Function

    ''' <summary>
    ''' Escolhe um valor de acordo com o resultado de uma variavel booliana
    ''' </summary>
    ''' <param name="BooleanValue"> Resultado da expressão booliana</param>
    ''' <param name="ChooseIfTrue"> Valor retornado se a expressão for verdadeira</param>
    ''' <param name="ChooseIfFalse">Valor retornado se a expressão for falsa</param>
    ''' <returns></returns>
    <Extension()> Public Function Choose(Of T)(BooleanValue As Boolean, ChooseIfTrue As T, ChooseIfFalse As T) As T
        Return If(BooleanValue, ChooseIfTrue, ChooseIfFalse)
    End Function

    ''' <summary>
    ''' Escolhe um valor de acordo com o resultado de uma expressão
    ''' </summary>
    ''' <param name="Expression">   Resultado da expressão booliana</param>
    ''' <param name="ChooseIfTrue"> Valor retornado se a expressão for verdadeira</param>
    ''' <param name="ChooseIfFalse">Valor retornado se a expressão for falsa</param>
    ''' <returns></returns>
    <Extension()> Public Function Choose(Of T)(Expression As String, ChooseIfTrue As T, ChooseIfFalse As T) As T
        Try
            Dim vv = EvaluateExpression(Expression)
            Select Case vv.GetType
                Case GetType(Integer), GetType(Decimal), GetType(Short), GetType(Long)
                    Return CType(vv > 0, Boolean).Choose(ChooseIfTrue, ChooseIfFalse)
                Case GetType(Boolean)
                    CType(vv, Boolean).Choose(ChooseIfTrue, ChooseIfFalse)
                Case GetType(String)
                    Return (vv.ToString.ToLower = "true" OrElse vv.ToString.ChangeType(Of Integer) > 0).Choose(ChooseIfTrue, ChooseIfFalse)
                Case Else
                    Return ChooseIfFalse
            End Select
        Catch ex As Exception
            Return ChooseIfFalse
        End Try
    End Function

    ''' <summary>
    ''' Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array.
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="List1">Lista 1</param>
    ''' <param name="List2">Lista2</param>
    ''' <returns></returns>
    <Extension()> Public Function ContainsAll(Of Type)(List1 As IEnumerable(Of Type), List2 As IEnumerable(Of Type), Optional Comparer As IEqualityComparer(Of Type) = Nothing) As Boolean
        For Each value As Type In List2
            If Comparer Is Nothing Then
                If IsNothing(List1) OrElse IsNothing(List2) OrElse Not List1.Contains(value) Then
                    Return False
                End If
            Else
                If IsNothing(List1) OrElse IsNothing(List2) OrElse Not List1.Contains(value, Comparer) Then
                    Return False
                End If
            End If

        Next
        Return True
    End Function

    ''' <summary>
    ''' Verifica se uma lista, coleção ou array contem um dos itens de outra lista, coleção ou array.
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="List1">Lista 1</param>
    ''' <param name="List2">Lista2</param>
    ''' <returns></returns>
    <Extension()> Public Function ContainsAny(Of Type)(List1 As IEnumerable(Of Type), List2 As IEnumerable(Of Type), Optional Comparer As IEqualityComparer(Of Type) = Nothing) As Boolean
        For Each value As Type In List2
            If Comparer Is Nothing Then
                If Not IsNothing(List1) AndAlso List1.Contains(value) Then
                    Return True
                End If
            Else
                If Not IsNothing(List1) AndAlso List1.Contains(value, Comparer) Then
                    Return True
                End If
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Copia os valores de um <see cref="NameValueCollection"/> para um objeto de um tipo especifico
    ''' </summary>
    ''' <typeparam name="Type">Tipo do Objeto</typeparam>
    ''' <param name="Collection">Colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function CopyToObject(Of Type As Class)(Collection As NameValueCollection, ByRef Obj As Type, ParamArray Keys As String()) As Type
        Dim PROPS = Obj.GetProperties
        If IsNothing(Keys) OrElse Keys.LongCount = 0 Then Keys = Collection.AllKeys
        For Each key As String In Collection.Keys
            If key.IsIn(Keys) And key.IsIn(PROPS.Select(Function(p) p.Name)) Then
                For Each prop In PROPS
                    Try
                        If prop.PropertyType.IsArray Then
                            prop.SetValue(Obj, Conversion.CTypeDynamic(Collection.GetValues(prop.Name), prop.PropertyType))
                        Else
                            prop.SetValue(Obj, Conversion.CTypeDynamic(Collection(prop.Name), prop.PropertyType))
                        End If
                    Catch ex As Exception
                    End Try
                Next
            End If
        Next
        Return Obj
    End Function

    ''' <summary>
    ''' Converte uma classe para um <see cref="Dictionary"/>
    ''' </summary>
    ''' <typeparam name="Type">Tipo da classe</typeparam>
    ''' <param name="Obj">Object</param>
    ''' <returns></returns>
    <Extension()> Public Function CreateDictionary(Of Type)(Obj As Type) As Dictionary(Of String, Object)
        Return Obj.[GetType]().GetProperties(BindingFlags.Instance Or BindingFlags.[Public]).ToDictionary(Function(prop) prop.Name, Function(prop) prop.GetValue(Obj, Nothing))
    End Function

    ''' <summary>
    ''' Cria um objeto de um tipo especifico a partir de um <see cref="NameValueCollection"/>
    ''' </summary>
    ''' <typeparam name="Type">Tipo do Objeto</typeparam>
    ''' <param name="Collection">Colecao</param>
    ''' <returns></returns>
    <Extension()>
    Public Function CreateObject(Of Type As Class)(Collection As NameValueCollection, ParamArray Keys As String()) As Type
        Dim obj = Activator.CreateInstance(Of Type)()
        Return CopyToObject(Of Type)(Collection, obj, Keys)
    End Function

    ''' <summary>
    ''' Conta de maneira distinta items de uma coleçao
    ''' </summary>
    ''' <typeparam name="Type">TIpo de Objeto</typeparam>
    ''' <param name="Arr">colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function DistinctCount(Of Type)(Arr As IEnumerable(Of Type)) As Dictionary(Of Type, Long)
        Return Arr.Distinct.Select(Function(p) New KeyValuePair(Of Type, Long)(p, Arr.Where(Function(x) x.Equals(p)).LongCount)).OrderByDescending(Function(p) p.Value).ToDictionary
    End Function

    ''' <summary>
    ''' Conta de maneira distinta items de uma coleçao a partir de uma propriedade
    ''' </summary>
    ''' <typeparam name="Type">TIpo de Objeto</typeparam>
    ''' <param name="Arr">colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function DistinctCount(Of Type, PropT)(Arr As IEnumerable(Of Type), Prop As Func(Of Type, PropT)) As Dictionary(Of PropT, Long)
        Return Arr.GroupBy(Prop).ToDictionary(Function(x) x.Key, Function(x) x.LongCount).OrderByDescending(Function(p) p.Value).ToDictionary
    End Function

    ''' <summary>
    ''' Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source">   </param>
    ''' <param name="alternate"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FirstOr(Of T)(source As IEnumerable(Of T), Alternate As T) As T
        If source IsNot Nothing AndAlso source.Count > 0 Then
            Return source.First
        Else
            Return Alternate
        End If
    End Function



    ''' <summary>
    ''' Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source">   </param>
    ''' <param name="alternate"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FirstOr(Of T)(source As IEnumerable(Of T), predicate As Func(Of T, Boolean), Alternate As T) As T
        Try
            Return source.First(predicate)
        Catch ex As Exception
            Return Alternate
        End Try
    End Function

    ''' <summary>
    ''' Cria um unico <see cref="NamevalueCollection"/> a partir de um
    ''' <see cref="HttpRequest.QueryString"/> e um <see cref="HttpRequest.Form"/>
    ''' </summary>
    ''' <param name="Request">HttpRequest</param>
    ''' <returns></returns>
    <Extension()> Public Function FlatRequest(Request As HttpRequest) As NameValueCollection
        Return ClassTools.Merge(Request.QueryString, Request.Form)
    End Function

    <Extension()>
    Function GetAttributeValue(Of TAttribute As Attribute, TValue)(ByVal type As Type, ByVal ValueSelector As Func(Of TAttribute, TValue)) As TValue
        Dim att = TryCast(type.GetCustomAttributes(GetType(TAttribute), True).FirstOrDefault(), TAttribute)
        If att IsNot Nothing Then
            Return ValueSelector(att)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Traz o valor de uma enumeração a partir de uma string
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function GetEnumValue(Of T)(Name As String) As T
        If Not GetType(T).IsEnum Then
            Throw New Exception("T must be an Enumeration type.")
        End If
        Dim val As T = DirectCast([Enum].GetValues(GetType(T)), T())(0)
        If Not String.IsNullOrEmpty(Name) Then
            For Each enumValue As T In DirectCast([Enum].GetValues(GetType(T)), T())
                If enumValue.ToString().ToUpper().Equals(Name.ToUpper()) Then
                    val = enumValue
                    Exit For
                End If
            Next
        End If
        Return val
    End Function

    ''' <summary>
    ''' Traz todos os Valores de uma enumeração
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function GetEnumValues(Of T)() As List(Of T)
        If Not GetType(T).IsEnum Then
            Throw New Exception("T must be an Enumeration type.")
        End If
        Return [Enum].GetValues(GetType(T)).Cast(Of T)().ToList
    End Function

    ''' <summary>
    ''' Traz uma Lista com todas as propriedades de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetProperties(MyObject As Object, BindAttr As BindingFlags) As List(Of PropertyInfo)
        If MyObject IsNot Nothing Then
            Return MyObject.GetType().GetProperties(BindAttr).ToList()
        Else
            Return New List(Of PropertyInfo)
        End If
    End Function

    ''' <summary>
    ''' Traz uma Lista com todas as propriedades de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetProperties(MyObject As Object) As List(Of PropertyInfo)
        If MyObject IsNot Nothing Then
            Return MyObject.GetType().GetProperties().ToList()
        Else
            Return New List(Of PropertyInfo)
        End If
    End Function

    ''' <summary>
    ''' Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetPropertyParameterFromString(Of Type)(Text As String) As Object()
        Return GetType(Type).GetPropertyParametersFromString(Text)
    End Function

    ''' <summary>
    ''' Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension> Public Function GetPropertyParametersFromString(Type As Type, Text As String) As Object()
        Dim props = Type.GetProperties(BindingFlags.Public + BindingFlags.NonPublic + BindingFlags.Instance)
        Dim name As String = Text.GetBefore("(")
        Dim params = Regex.Split(Text.RemoveFirstIf(name).RemoveFirstIf("(").RemoveLastIf(")"), ",(?=(?:[^""]*""[^""]*"")*[^""]*$)")

        Dim info = props.Where(Function(x) x.Name.ToLower = name.ToLower AndAlso x.GetIndexParameters.Count = params.Count).FirstOrDefault

        If info IsNot Nothing Then
            Dim oParam = info.GetIndexParameters()
            Dim arr As New List(Of Object)(oParam.Count)
            For index = 0 To oParam.Count - 1
                Try
                    arr.Add(Convert.ChangeType(params(index).GetWrappedText.FirstOr(params(index)), oParam(index).ParameterType))
                Catch ex As Exception
                End Try
            Next
            Return arr.ToArray
        End If
        Return {}
    End Function

    ''' <summary>
    ''' Traz o valor de uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">    Objeto</param>
    ''' <param name="PropertyName">Nome da properiedade</param>
    ''' <param name="Type">        Tipo do Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetPropertyValue(MyObject As Object, PropertyName As String, Type As Type, Optional GetPrivate As Boolean = False)
        Try
            Dim obj = MyObject
            Dim parts = New List(Of String)()
            Dim [stop] = False
            Dim current = New StringBuilder()

            For i As Integer = 0 To PropertyName.Length - 1
                If PropertyName(i) <> "."c Then current.Append(PropertyName(i))
                If PropertyName(i) = "("c Then [stop] = True
                If PropertyName(i) = ")"c Then [stop] = False
                If (PropertyName(i) = "."c AndAlso Not [stop]) OrElse i = PropertyName.Length - 1 Then
                    parts.Add(current.ToString())
                    current.Length = 0
                End If
            Next

            For Each part As String In parts
                If MyObject Is Nothing Then
                    Return Nothing
                End If
                Dim t = obj.[GetType]()
                If t.IsValueType Or t = GetType(String) Then
                    Return MyObject
                End If
                Dim info As PropertyInfo
                If GetPrivate Then
                    info = ClassTools.GetProperties(obj, BindingFlags.Public + BindingFlags.NonPublic + BindingFlags.Instance).Where(Function(x) x.Name.ToLower = part.GetBefore("(").ToLower).FirstOrDefault
                Else
                    info = ClassTools.GetProperties(obj).Where(Function(x) x.Name.ToLower = part.GetBefore("(").ToLower).FirstOrDefault
                End If
                If info Is Nothing Then
                    Return Nothing
                End If

                If part.RemoveFirstIf(info.Name).StartsWith("(") Then
                    Dim allparams = obj.GetType().GetPropertyParametersFromString(part)
                    obj = info.GetValue(obj, allparams)
                Else

                    obj = info.GetValue(obj)
                End If
            Next
            Return Conversion.CTypeDynamic(obj, Type)
        Catch ex As Exception
            Debug.WriteLine(ex)
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Traz o valor de uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">    Objeto</param>
    ''' <param name="PropertyName">Nome da properiedade</param>
    ''' <typeparam name="Type">Tipo do Objeto</typeparam>
    ''' <returns></returns>
    <Extension()>
    Public Function GetPropertyValue(Of Type)(MyObject As Object, PropertyName As String, Optional GetPrivate As Boolean = False) As Type
        Return GetPropertyValue(MyObject, PropertyName, GetType(Type), GetPrivate)
    End Function

    ''' <summary>
    ''' Traz o valor de uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">    Objeto</param>
    ''' <param name="PropertyName">Nome da properiedade</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetPropertyValue(MyObject As Object, PropertyName As String, Optional GetPrivate As Boolean = True) As Object
        Return GetPropertyValue(Of Object)(MyObject, PropertyName, GetPrivate)
    End Function

    ''' <summary>
    ''' Pega os bytes de um arquivo embutido no assembly
    ''' </summary>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetResourceBytes(Assembly As Assembly, FileName As String) As Byte()
        Dim resourceStream = [Assembly].GetManifestResourceStream(FileName)
        If resourceStream Is Nothing Then
            Return Nothing
        End If
        Dim fontBytes = New Byte(resourceStream.Length - 1) {}
        resourceStream.Read(fontBytes, 0, CInt(resourceStream.Length))
        resourceStream.Close()
        Return fontBytes
    End Function

    ''' <summary>
    ''' Pega o texto de um arquivo embutido no assembly
    ''' </summary>
    ''' <param name="FileName">Nome do arquivo embutido dentro do assembly (Embedded Resource)</param>
    ''' <returns></returns>
    <Extension()> Public Function GetResourceFileText(Assembly As Assembly, FileName As String) As String
        Using d As New StreamReader(Assembly.GetManifestResourceStream(FileName))
            Return d.ReadToEnd
        End Using
    End Function

    ''' <summary>
    ''' Pega o texto de um arquivo embutido no assembly
    ''' </summary>
    ''' <param name="FileName">Nome do arquivo embutido dentro do assembly (Embedded Resource)</param>
    ''' <returns></returns>
    <Extension()> Public Function GetResourceHtmlDocument(Assembly As Assembly, FileName As String) As HtmlDocument
        Using d As New StreamReader(Assembly.GetManifestResourceStream(FileName))
            Return New HtmlDocument(d.ReadToEnd)
        End Using
    End Function

    <Extension()>
    Function GetValueOr(Of tkey, Tvalue)(Dic As IDictionary(Of tkey, Tvalue), Key As tkey, Optional ReplaceValue As Tvalue = Nothing) As Tvalue
        Try
            Return Dic(Key)
        Catch ex As Exception
            Return ReplaceValue
        End Try
    End Function

    ''' <summary>
    ''' Verifica se um tipo possui uma propriedade
    ''' </summary>
    ''' <param name="Type">        </param>
    ''' <param name="PropertyName"></param>
    ''' <returns></returns>
    <Extension()> Public Function HasProperty(Type As Type, PropertyName As String, Optional GetPrivate As Boolean = False) As Boolean

        Dim parts = New List(Of String)()
        Dim [stop] = False
        Dim current = New StringBuilder()

        For i As Integer = 0 To PropertyName.Length - 1
            If PropertyName(i) <> "."c Then current.Append(PropertyName(i))
            If PropertyName(i) = "("c Then [stop] = True
            If PropertyName(i) = ")"c Then [stop] = False
            If (PropertyName(i) = "."c AndAlso Not [stop]) OrElse i = PropertyName.Length - 1 Then
                parts.Add(current.ToString())
                current.Length = 0
            End If
        Next

        Dim prop As PropertyInfo
        If GetPrivate Then
            prop = Type.GetProperty(parts.First.GetBefore("("), BindingFlags.Public + BindingFlags.NonPublic + BindingFlags.Instance)
        Else
            prop = Type.GetProperty(parts.First.GetBefore("("))
        End If

        Dim exist As Boolean = prop IsNot Nothing
        parts.RemoveAt(0)
        If exist AndAlso parts.Count > 0 Then
            exist = prop.PropertyType.HasProperty(parts.First, GetPrivate)
        End If
        Return exist
    End Function

    ''' <summary>
    ''' Verifica se um tipo possui uma propriedade
    ''' </summary>
    ''' <param name="Obj"> </param>
    ''' <param name="Name"></param>
    ''' <returns></returns>
    <Extension()> Public Function HasProperty(Obj As Object, Name As String) As Boolean
        Return ClassTools.HasProperty(Obj.GetType, Name, True)
    End Function

    ''' <summary>
    ''' Verifica se o tipo é um array de um objeto especifico
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Type"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsArrayOf(Of T)(Type As Type) As Boolean
        Return Type = GetType(T())
    End Function

    ''' <summary>
    ''' Verifica se o tipo é um array de um objeto especifico
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsArrayOf(Of T)(Obj As Object) As Boolean
        Return Obj.GetType.IsArrayOf(Of T)
    End Function

    ''' <summary>
    ''' Verifica se o objeto é um iDictionary
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsDictionary(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If
        Return TypeOf obj Is IDictionary AndAlso obj.[GetType]().IsGenericType AndAlso obj.[GetType]().GetGenericTypeDefinition().IsAssignableFrom(GetType(Dictionary(Of , )))
    End Function

    ''' <summary>
    ''' Verifica se o objeto existe dentro de uma Lista, coleção ou array.
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="Obj"> objeto</param>
    ''' <param name="List">Lista</param>
    ''' <returns></returns>
    <Extension()> Public Function IsIn(Of Type)(Obj As Type, List As IEnumerable(Of Type), Optional Comparer As IEqualityComparer(Of Type) = Nothing) As Boolean
        If Comparer Is Nothing Then
            Return List.Contains(Obj)
        Else
            Return List.Contains(Obj, Comparer)
        End If
    End Function

    ''' <summary>
    ''' Verifica se o objeto existe dentro de um texto
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="Obj"> objeto</param>
    ''' <param name="TExt">Texto</param>
    ''' <returns></returns>
    <Extension()> Public Function IsIn(Of Type)(Obj As Type, Text As String, Optional Comparer As IEqualityComparer(Of Char) = Nothing) As Boolean
        If Comparer Is Nothing Then
            Return Not IsNothing(Obj) AndAlso Text.Contains(Obj.ToString)
        Else
            Return Not IsNothing(Obj) AndAlso Text.Contains(Obj.ToString, Comparer)
        End If
    End Function

    ''' <summary>
    ''' Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays.
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="Obj"> objeto</param>
    ''' <param name="List">Lista</param>
    ''' <returns></returns>
    <Extension()> Public Function IsInAny(Of Type)(Obj As Type, List As IEnumerable(Of Type)(), Optional Comparer As IEqualityComparer(Of Type) = Nothing) As Boolean
        Return List.Any(Function(x) Obj.IsIn(x, Comparer))
    End Function

    ''' <summary>
    ''' Verifica se o objeto é uma lista
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    <Extension>
    Public Function IsList(obj As Object) As Boolean
        If obj Is Nothing Then
            Return False
        End If
        Return TypeOf obj Is IList AndAlso obj.[GetType]().IsGenericType AndAlso obj.[GetType]().GetGenericTypeDefinition().IsAssignableFrom(GetType(List(Of )))
    End Function

    ''' <summary>
    ''' Verifica se o não objeto existe dentro de uma Lista, coleção ou array.
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="Obj"> objeto</param>
    ''' <param name="List">Lista</param>
    ''' <returns></returns>
    <Extension()> Public Function IsNotIn(Of Type)(Obj As Type, List As IEnumerable(Of Type), Optional Comparer As IEqualityComparer(Of Type) = Nothing) As Boolean
        If Comparer Is Nothing Then
            Return Not List.Contains(Obj)
        Else
            Return Not List.Contains(Obj, Comparer)
        End If
    End Function

    ''' <summary>
    ''' Verifica se o objeto não existe dentro de um texto
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="Obj"> objeto</param>
    ''' <param name="TExt">Texto</param>
    ''' <returns></returns>
    <Extension()> Public Function IsNotIn(Of Type)(Obj As Type, Text As String, Optional Comparer As IEqualityComparer(Of Char) = Nothing) As Boolean
        If Comparer Is Nothing Then
            Return IsNothing(Obj) OrElse Text.Contains(Obj.ToString)
        Else
            Return IsNothing(Obj) OrElse Text.Contains(Obj.ToString, Comparer)
        End If
    End Function

    ''' <summary>
    '''Verifica se o objeto é do tipo numérico.
    ''' </summary>
    ''' <remarks>
    ''' Boolean is not considered numeric.
    ''' </remarks>
    <Extension> Public Function IsNumericType(Obj As Type) As Boolean
        If Obj Is Nothing Then
            Return False
        End If

        Select Case Type.GetTypeCode(Obj)
            Case TypeCode.[Byte], TypeCode.[Decimal], TypeCode.[Double], TypeCode.Int16, TypeCode.Int32, TypeCode.Int64,
            TypeCode.[SByte], TypeCode.[Single], TypeCode.UInt16, TypeCode.UInt32, TypeCode.UInt64
                Return True
            Case TypeCode.[Object]
                If Obj.IsGenericType AndAlso Obj.GetGenericTypeDefinition() = GetType(Nullable(Of )) Then
                    Return IsNumericType(Nullable.GetUnderlyingType(Obj))
                End If
                Return False
        End Select
        Return False
    End Function

    ''' <summary>
    ''' Verifica se um objeto é de um determinado tipo
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    <Extension()> Public Function IsType(Of T)(Obj As [Object]) As Boolean
        Return Obj.GetType() Is GetType(T)
    End Function

    ''' <summary>
    ''' Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source">   </param>
    ''' <param name="alternate"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LastOr(Of T)(source As IEnumerable(Of T), Alternate As T) As T
        If source IsNot Nothing AndAlso source.Count > 0 Then
            Return source.Last
        Else
            Return Alternate
        End If

    End Function

    ''' <summary>
    ''' Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
    ''' </summary>
    ''' <param name="NVC"></param>
    ''' <returns></returns>
    Public Function Merge(ParamArray NVC As NameValueCollection()) As NameValueCollection
        Dim all = New NameValueCollection()
        For Each i In NVC
            all.Add(i)
        Next
        Return all
    End Function

    ''' <summary>
    ''' Mescla varios tipos de objeto em um unico dicionario a partir de suas propriedades
    ''' </summary>
    ''' <param name="Items"></param>
    ''' <returns></returns>
    Public Function MergeProperties(ParamArray Items As Object()) As Dictionary(Of String, Object)
        Dim result As New Dictionary(Of String, Object)
        For Each item In Items
            If IsDictionary(item) Then
                Dim dic = CType(item, IDictionary)
                For Each k In dic.Keys
                    result(k) = dic(k)
                Next
            Else
                For Each fi As PropertyInfo In GetProperties(item)
                    result(fi.Name) = GetPropertyValue(item, fi.Name)
                Next
            End If
        Next
        Return result
    End Function

    ''' <summary>
    ''' Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
    ''' </summary>
    ''' <typeparam name="T">Tipo</typeparam>
    ''' <param name="First">Primeiro Item</param>
    ''' <param name="N">    Outros itens</param>
    ''' <returns></returns>
    <Extension> Public Function NullCoalesce(Of T As Structure)(First As T?, ParamArray N() As T?) As T
        Dim l As New List(Of T?)
        l.Add(First)
        l.AddRange(N)
        If First IsNot Nothing Then Return First
        Return NullCoalesce(Of T)(N)
    End Function

    ''' <summary>
    ''' Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
    ''' </summary>
    ''' <typeparam name="T">Tipo</typeparam>
    ''' <param name="List">Outros itens</param>
    ''' <returns></returns>
    <Extension()> Function NullCoalesce(Of T As Structure)(List As IEnumerable(Of T?)) As T
        For Each item In List
            If item.HasValue Then
                Return item.Value
            End If
        Next
        Return New T?
    End Function

    ''' <summary>
    ''' Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
    ''' </summary>
    ''' <typeparam name="T">Tipo</typeparam>
    ''' <param name="First">Primeiro Item</param>
    ''' <param name="N">    Outros itens</param>
    ''' <returns></returns>
    <Extension> Public Function NullCoalesce(Of T As Class)(First As T, ParamArray N() As T) As T
        Dim l As New List(Of T)
        l.Add(First)
        l.AddRange(N)
        If First IsNot Nothing Then Return First
        Return NullCoalesce(Of T)(N)
    End Function

    ''' <summary>
    ''' Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
    ''' </summary>
    ''' <typeparam name="T">Tipo</typeparam>
    ''' <param name="List">Outros itens</param>
    ''' <returns></returns>
    <Extension()> Function NullCoalesce(Of T As Class)(List As IEnumerable(Of T)) As T
        For Each item In List
            If item IsNot Nothing Then
                Return item
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' Transforma todas as propriedades String em NULL quando suas estiverem em branco
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <param name="Obj"></param>
    ''' <returns></returns>
    <Extension()> Public Function NullifyProperties(Of Type)(Obj As Type) As Type
        For Each prop In Obj.GetProperties
            Try
                If Obj.GetPropertyValue(prop.Name).ToString.IsBlank Then
                    prop.SetValue(Obj, Nothing)
                End If
            Catch ex As Exception
            End Try
        Next
        Return Obj
    End Function

    ''' <summary>
    ''' Remove de um dicionario as respectivas Keys se as mesmas existirem
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="Tvalue"></typeparam>
    ''' <param name="dic"> </param>
    ''' <param name="Keys"></param>
    <Extension()> Public Sub RemoveIfExist(Of TKey, TValue)(dic As IDictionary(Of TKey, TValue), ParamArray Keys As TKey())
        For Each k In Keys
            If dic.ContainsKey(k) Then
                dic.Remove(k)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Remove de um dicionario os valores encontrados pelo predicate
    ''' </summary>
    ''' <typeparam name="TKey"></typeparam>
    ''' <typeparam name="Tvalue"></typeparam>
    ''' <param name="dic">      </param>
    ''' <param name="predicate"></param>
    <Extension()> Public Sub RemoveIfExist(Of TKey, TValue)(dic As IDictionary(Of TKey, TValue), predicate As Func(Of KeyValuePair(Of TKey, TValue), Boolean))
        dic.RemoveIfExist(dic.Where(predicate).Select(Function(x) x.Key).ToArray)
    End Sub

    ''' <summary>
    ''' Seta o valor de uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">    Objeto</param>
    ''' <param name="PropertyName">Nome da properiedade</param>
    ''' <param name="Value">       Valor da propriedade definida por <paramref name="PropertyName"/></param>
    ''' <typeparam name="Type">
    ''' Tipo do <paramref name="Value"/> da propriedade definida por <paramref name="PropertyName"/>
    ''' </typeparam>
    <Extension()>
    Public Sub SetPropertyValue(Of Type)(MyObject As Object, PropertyName As String, Value As Type)
        GetProperties(MyObject).Where(Function(p) p.Name = PropertyName).First.SetValue(MyObject, Value)
    End Sub

    <Extension()>
    Public Sub SetPropertyValueFromCollection(Of Type)(MyObject As Object, PropertyName As String, Collection As CollectionBase)
        GetProperties(MyObject).Where(Function(p) p.Name = PropertyName).First.SetValue(MyObject, Collection(PropertyName))
    End Sub

    ''' <summary>
    ''' Retorna o objeto em seu formato padrão de String, ou serializa o objeto em Json se o mesmo
    ''' não possuir formato em string
    ''' </summary>
    ''' <param name="obj">       </param>
    ''' <param name="DateFormat"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToFlatString(Obj As Object, Optional DateFormat As String = "") As String

        Select Case Obj.GetType
            Case GetType(DateTime), GetType(Date)
                Return CType(Obj, Date).ToString(DateFormat.IfBlank("yyyy-MM-dd HH:mm:ss"))
            Case GetType(String), GetType(Integer), GetType(Long), GetType(Short), GetType(Double), GetType(Decimal), GetType(Money), GetType(HtmlDocument), GetType(HtmlElement)
                Return Obj.ToString
            Case Else
                If (Obj.GetType.GetMethod("ToString").DeclaringType IsNot GetType(Object)) Then
                    Return Obj.ToString
                Else
                    Return Json.SerializeJSON(Obj)
                End If
        End Select
    End Function


End Module