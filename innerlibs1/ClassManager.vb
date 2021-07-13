Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.IO
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Text.RegularExpressions

Imports System.Xml
Imports System.Xml.Serialization
Imports InnerLibs.LINQ

Public Module ClassTools





    <Extension()> Public Function IsNullOrEmpty(Of T)(ByVal List As IEnumerable(Of T)) As Boolean
        Return Not If(List, {}).Any()
    End Function

    <Extension()> Function ObjectToByteArray(ByVal obj As Object) As Byte()
        If obj Is Nothing Then Return Nothing
        Dim bf As BinaryFormatter = New BinaryFormatter()
        Dim ms As MemoryStream = New MemoryStream()
        bf.Serialize(ms, obj)
        Dim b = ms.ToArray()
        ms.Dispose()
        Return b
    End Function

    Function ByteArrayToObject(ByVal arrBytes As Byte()) As Object
        Return ByteArrayToObject(Of Object)(arrBytes)
    End Function

    <Extension()> Function ByteArrayToObject(Of T)(ByVal arrBytes As Byte()) As T
        Dim memStream As MemoryStream = New MemoryStream()
        Dim binForm As BinaryFormatter = New BinaryFormatter()
        memStream.Write(arrBytes, 0, arrBytes.Length)
        memStream.Seek(0, SeekOrigin.Begin)
        Dim obj = CType(binForm.Deserialize(memStream), T)
        memStream.Dispose()
        Return obj
    End Function

    <Extension>
    Public Function IsPrimitiveType(T As Type) As Boolean
        Return T.IsIn(
           GetType(String),
           GetType(Char),
           GetType(Byte),
           GetType(SByte),
           GetType(UShort),
           GetType(Short),
           GetType(Integer),
           GetType(UInt16),
           GetType(UInt64),
           GetType(UInt32),
           GetType(ULong),
           GetType(Long),
           GetType(Double),
           GetType(Decimal),
           GetType(DateTime))
    End Function

    <Extension>
    Public Function IsPrimitiveType(Of T)(Obj As T) As Boolean
        If Obj.GetType() Is GetType(Type) Then
            Return Obj.ChangeType(Of Type).IsPrimitiveType()
        End If
        Return Obj.GetType().IsPrimitiveType()
    End Function

    <Extension> Public Function RemoveLast(Of T)(List As List(Of T), Optional Count As Integer = 1) As List(Of T)
        For index = 1 To Count
            If List IsNot Nothing AndAlso List.Any() Then
                List.RemoveAt(List.Count() - 1)
            End If
        Next
        Return List
    End Function

    <Extension()>
    Public Function IsEqual(Of T As IComparable)(ByVal Value1 As T, ByVal Value2 As T) As Boolean
        Return Not Value1.IsGreaterThan(Value2) AndAlso Not Value1.IsLessThan(Value2)
    End Function

    <Extension()>
    Public Function IsGreaterThan(Of T As IComparable)(ByVal Value1 As T, ByVal Value2 As T) As Boolean
        Return Value1.CompareTo(Value2) > 0
    End Function

    <Extension()>
    Public Function IsGreaterThanOrEqual(Of T As IComparable)(ByVal Value1 As T, ByVal Value2 As T) As Boolean
        Return Value1.IsGreaterThan(Value2) OrElse Value1.IsEqual(Value2)
    End Function

    <Extension()>
    Public Function IsLessThan(Of T As IComparable)(ByVal Value1 As T, ByVal Value2 As T) As Boolean
        Return Value1.CompareTo(Value2) < 0
    End Function

    <Extension()>
    Public Function IsLessThanOrEqual(Of T As IComparable)(ByVal Value1 As T, ByVal Value2 As T) As Boolean
        Return Value1.IsLessThan(Value2) OrElse Value1.IsEqual(Value2)
    End Function

    ''' <summary>
    ''' Verifica se um valor numerico ou data está entre outros 2 valores
    ''' </summary>
    ''' <param name="Value">      Numero</param>
    ''' <param name="Value1"> Primeiro numero comparador</param>
    ''' <param name="Value2">Segundo numero comparador</param>
    ''' <returns></returns>
    <Extension()> Public Function IsBetween(Value As IComparable, Value1 As IComparable, Value2 As IComparable) As Boolean
        FixOrder(Value1, Value2)
        Return Value.IsLessThan(Value2) AndAlso Value.IsGreaterThan(Value1)
    End Function

    ''' <summary>
    ''' Verifica se um valor numerico ou data está entre outros 2 valores
    ''' </summary>
    ''' <param name="Value">      Numero</param>
    ''' <param name="Value1"> Primeiro numero comparador</param>
    ''' <param name="Value2">Segundo numero comparador</param>
    ''' <returns></returns>
    <Extension()> Public Function IsBetweenOrEqual(Value As IComparable, Value1 As IComparable, Value2 As IComparable) As Boolean
        FixOrder(Value1, Value2)
        Return Value.IsLessThanOrEqual(Value2) AndAlso Value.IsGreaterThanOrEqual(Value1)
    End Function

    ''' <summary>
    ''' Troca ou não a ordem das variaveis de inicio e fim  fazendo com que a Value1
    ''' sempre seja menor que a Value2. Util para tratar ranges
    ''' </summary>

    Public Sub FixOrder(Of T As IComparable)(ByRef Value1 As T, ByRef Value2 As T)
        If Value1 IsNot Nothing AndAlso Value2 IsNot Nothing Then
            If Value1.IsGreaterThan(Value2) Then
                Dim temp = Value1
                Value1 = Value2
                Value2 = temp
            End If
        End If
    End Sub

    <Extension()> Public Function CreateXML(Of Type)(obj As Type) As XmlDocument
        Dim xs As XmlSerializer = New XmlSerializer(obj.GetType())
        Dim sw As System.IO.StringWriter = New System.IO.StringWriter()
        xs.Serialize(sw, obj)
        Dim doc As XmlDocument = New XmlDocument()
        doc.LoadXml(sw.ToString())
        Return doc
    End Function

    <Extension()> Public Function CreateObjectFromXML(Of Type)(XML As String) As Type
        Dim serializer = New XmlSerializer(GetType(Type))
        Dim obj As Type
        Using reader = New StringReader(XML)
            obj = serializer.Deserialize(reader)
        End Using
        Return obj
    End Function

    <Extension()> Public Function CreateObjectFromXMLFile(Of Type)(XML As FileInfo) As Type
        Return File.ReadAllText(XML.FullName).CreateObjectFromXML(Of Type)
    End Function

    ''' <summary>
    ''' Cria um arquivo a partir de qualquer objeto usando o <see cref="CreateObjectFromXML(Object)"/>
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <returns></returns>
    <Extension()> Public Function CreateXmlFile(obj As Object, FilePath As String) As FileInfo
        Return CreateXML(obj).ToXMLString().WriteToFile(FilePath)
    End Function

    ''' <summary>
    ''' Retorna as classes de um Namespace
    ''' </summary>
    ''' <param name="assembly"></param>
    ''' <param name="desiredNamespace"></param>
    ''' <returns></returns>
    <Extension> Public Function GetTypesFromNamespace(assembly As Assembly, desiredNamespace As String) As Type()
        Return assembly.GetTypes().Where(Function(x) x.Namespace = desiredNamespace)
    End Function

    ''' <summary>
    ''' Retorna as classes de um Namespace
    ''' </summary>
    ''' <param name="desiredNamespace"></param>
    ''' <returns></returns>
    Public Function GetTypesFromNamespace(desiredNamespace As String) As Type()
        Return Assembly.GetExecutingAssembly().GetTypesFromNamespace(desiredNamespace)
    End Function

    ''' <summary>
    ''' Cria um <see cref="Guid"/> a partir de uma string ou um novo <see cref="Guid"/> se a conversão falhar
    ''' </summary>
    ''' <param name="Source"></param>
    ''' <returns></returns>
    <Extension()> Public Function CreateGuidOrDefault(Source As String) As Guid
        Dim g = Guid.NewGuid()
        If Source.IsNotBlank() Then
            If Not Guid.TryParse(Source, g) Then
                g = Guid.NewGuid()
            End If
        End If
        Return g
    End Function

    ''' <summary>
    ''' Concatena todas as  <see cref="Exception.InnerException"/> em uma única string
    ''' </summary>
    ''' <param name="ex"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToFullExceptionString(ex As Exception, Optional Separator As String = " >> ") As String
        Return ex.Traverse(Function(x) x.InnerException).SelectJoin(Function(x) x.Message, Separator).AdjustBlankSpaces
    End Function

    ''' <summary>
    ''' Retorna um dicionário em QueryString
    ''' </summary>
    ''' <param name="Dic"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToQueryString(Dic As Dictionary(Of String, String)) As String
        Dim param As String = ""
        If Dic IsNot Nothing Then
            Return Dic.Where(Function(x) x.Key.IsNotBlank()).SelectJoin(Function(x) {x.Key, If(x.Value, "").UrlEncode}.Join("="), "&")
        End If
        Return ""
    End Function

    ''' <summary>
    ''' Retorna um <see cref="NameValueCollection"/> em QueryString
    ''' </summary>
    ''' <param name="NVC"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToQueryString(NVC As NameValueCollection) As String
        Return NVC.AllKeys.SelectManyJoin(Function(n) NVC.GetValues(n).Select(Function(v) n & "=" & v).Where(Function(x) x.IsNotBlank() AndAlso x <> "="), "&")
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
    ''' Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays
    ''' </summary>
    ''' <typeparam name="GroupKey"></typeparam>
    ''' <typeparam name="SubGroupKey"></typeparam>
    ''' <typeparam name="SubGroupValue"></typeparam>
    ''' <param name="Groups"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToTableArray(Of GroupKey, SubGroupKey, SubGroupValue, HeaderProperty)(ByVal Groups As Dictionary(Of GroupKey, Dictionary(Of SubGroupKey, SubGroupValue)), HeaderProp As Func(Of SubGroupKey, HeaderProperty)) As IEnumerable(Of Object)
        Dim lista = New List(Of Object)
        Dim header = New List(Of Object)
        header.Add(HeaderProp.Method.GetParameters.First().Name)

        Groups.Values.MergeKeys()
        For Each h In Groups.SelectMany(Function(x) x.Value.Keys.ToArray()).Distinct().OrderBy(Function(x) x)
            header.Add(HeaderProp(h))
        Next
        lista.Add(header)
        lista.AddRange(Groups.Select(Function(x)
                                         Dim l = New List(Of Object)
                                         l.Add(x.Key) 'GroupKey
                                         For Each item In x.Value.OrderBy(Function(k) k.Key).Select(Function(v) v.Value)
                                             l.Add(item) 'SubGroupValue
                                         Next
                                         Return l
                                     End Function))
        Return lista
    End Function

    ''' <summary>
    ''' Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays
    ''' </summary>
    <Extension()> Public Function ToTableArray(Of GroupKeyType, GroupValueType)(ByVal Groups As Dictionary(Of GroupKeyType, GroupValueType))
        Return Groups.Select(Function(x)
                                 Dim l = New List(Of Object)
                                 l.Add(x.Key)
                                 l.Add(x.Value)
                                 Return l.ToArray()
                             End Function)
    End Function

    ''' <summary>
    ''' Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada grupo a partir de outra propriedade do mesmo objeto
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <typeparam name="Group"></typeparam>
    ''' <typeparam name="Count"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="GroupSelector"></param>
    ''' <param name="CountObjectBy"></param>
    ''' <returns></returns>
    <Extension()> Public Function GroupAndCountSubGroupBy(Of Type, Group, Count)(obj As IEnumerable(Of Type), GroupSelector As Func(Of Type, Group), CountObjectBy As Func(Of Type, Count)) As Dictionary(Of Group, Dictionary(Of Count, Long))
        Dim dic_of_dic = obj.GroupBy(GroupSelector).Select(Function(x) New KeyValuePair(Of Group, Dictionary(Of Count, Long))(x.Key, x.GroupBy(CountObjectBy).ToDictionary(Function(y) y.Key, Function(y) y.LongCount))).ToDictionary()
        dic_of_dic.Values.MergeKeys
        Return dic_of_dic
    End Function

    ''' <summary>
    ''' Agrupa e conta os itens de uma lista a partir de uma propriedade
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <typeparam name="Group"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="GroupSelector"></param>
    ''' <returns></returns>
    <Extension()> Public Function GroupAndCountBy(Of Type, Group)(obj As IEnumerable(Of Type), GroupSelector As Func(Of Type, Group)) As Dictionary(Of Group, Long)
        Return obj.GroupBy(GroupSelector).Select(Function(x) New KeyValuePair(Of Group, Long)(x.Key, x.LongCount())).ToDictionary()
    End Function

    ''' <summary>
    ''' Agrupa e conta os itens de uma lista a partir de uma propriedade
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <typeparam name="Group"></typeparam>
    ''' <param name="obj"></param>
    ''' <param name="GroupSelector"></param>
    ''' <returns></returns>
    <Extension()> Public Function GroupFirstAndCountBy(Of Type, Group)(obj As IEnumerable(Of Type), First As Integer, GroupSelector As Func(Of Type, Group), OtherLabel As Group) As Dictionary(Of Group, Long)
        Dim grouped = obj.GroupBy(GroupSelector).Select(Function(x) New KeyValuePair(Of Group, Long)(x.Key, x.LongCount())).OrderByDescending(Function(x) x.Value)
        Return grouped.Take(First).Union({New KeyValuePair(Of Group, Long)(OtherLabel, grouped.Skip(First).Sum(Function(s) s.Value))}).ToDictionary()
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
        dic_of_dic.Values.MergeKeys
        Return dic_of_dic
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
                If Not List1.Contains(value) Then
                    Return False
                End If
            Else
                If Not List1.Contains(value, Comparer) Then
                    Return False
                End If
            End If

        Next
        Return True
    End Function

    ''' <summary>
    ''' Verifica se somente um unico elemento corresponde a condição
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <param name="List"></param>
    ''' <param name="predicate"></param>
    ''' <returns></returns>
    <Extension()> Public Function OnlyOneOf(Of Type)(List As IEnumerable(Of Type), predicate As Func(Of Type, Boolean)) As Boolean
        Return List.Count(predicate) = 1
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
                If List1.Contains(value) Then
                    Return True
                End If
            Else
                If List1.Contains(value, Comparer) Then
                    Return True
                End If
            End If
        Next
        Return False
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
    ''' Conta de maneira distinta items de uma coleçao
    ''' </summary>
    ''' <typeparam name="Type">TIpo de Objeto</typeparam>
    ''' <param name="Arr">colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function DistinctCount(Of Type)(Arr As IEnumerable(Of Type)) As Dictionary(Of Type, Long)
        Return Arr.Distinct.Select(Function(p) New KeyValuePair(Of Type, Long)(p, Arr.Where(Function(x) x.Equals(p)).LongCount)).OrderByDescending(Function(p) p.Value).ToDictionary
    End Function

    ''' <summary>
    ''' Conta de maneira distinta N items de uma coleçao e agrupa o resto
    ''' </summary>
    ''' <typeparam name="Type">TIpo de Objeto</typeparam>
    ''' <param name="Arr">colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function DistinctCountTop(Of Type)(Arr As IEnumerable(Of Type), Top As Integer, Others As Type) As Dictionary(Of Type, Long)
        Dim a = Arr.DistinctCount()
        Dim topN = a.TakeTop(Top, Others)
        Return topN
    End Function

    ''' <summary>
    ''' traz os top N valores de um dicionario e agrupa os outros
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Dic"></param>
    ''' <param name="Top"></param>
    ''' <param name="GroupOthersLabel"></param>
    ''' <returns></returns>
    <Extension()> Public Function TakeTop(Of K, T As IConvertible)(Dic As IDictionary(Of K, T), Top As Integer, GroupOthersLabel As K) As Dictionary(Of K, T)
        If Top < 1 Then Return Dic
        Dim novodic = Dic.Take(Top).ToDictionary()
        If GroupOthersLabel IsNot Nothing Then
            novodic(GroupOthersLabel) = Dic.Values.Skip(Top).Select(Function(x) x.ChangeType(Of Decimal)).Sum().ChangeType(Of T)
        End If
        Return novodic
    End Function

    ''' <summary>
    ''' traz os top N valores de um dicionario e agrupa os outros
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="Dic"></param>
    ''' <param name="Top"></param>
    ''' <param name="GroupOthersLabel"></param>
    ''' <returns></returns>
    <Extension()> Public Function TakeTop(Of K, T)(Dic As IDictionary(Of K, IEnumerable(Of T)), Top As Integer, GroupOthersLabel As K) As Dictionary(Of K, IEnumerable(Of T))
        If Top < 1 Then Return Dic
        Dim novodic = Dic.Take(Top).ToDictionary()
        If GroupOthersLabel IsNot Nothing Then
            novodic(GroupOthersLabel) = Dic.Values.Skip(Top).SelectMany(Function(x) x).Select(Function(x) x.ChangeType(Of Decimal)).Sum().ChangeType(Of T)
        End If
        Return novodic
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
    ''' Conta de maneira distinta N items de uma coleçao a partir de uma propriedade e agrupa o resto em outra
    ''' </summary>
    ''' <typeparam name="Type">TIpo de Objeto</typeparam>
    ''' <param name="Arr">colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function DistinctCountTop(Of Type, PropT)(Arr As IEnumerable(Of Type), Prop As Func(Of Type, PropT), Top As Integer, Others As PropT) As Dictionary(Of PropT, Long)
        Dim a = Arr.DistinctCount(Prop)
        If Top < 1 Then Return a
        Dim topN = a.TakeTop(Top, Others)
        Return topN
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
        If source IsNot Nothing AndAlso source.Any() Then
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
        Return If(source.FirstOrDefault(predicate), Alternate)

    End Function

    ''' <summary>
    ''' O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="predicate"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FirstAny(Of T)(source As IEnumerable(Of T), ParamArray predicate() As Func(Of T, Boolean)) As T
        For index = 0 To predicate.Length - 1
            Dim v = source.FirstOrDefault(predicate(index))
            If v IsNot Nothing Then
                Return v
            End If
        Next
        Return Nothing
    End Function

    ''' <summary>
    ''' O primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="source"></param>
    ''' <param name="predicate"></param>
    ''' <returns></returns>
    <Extension>
    Public Function FirstAnyOr(Of T)(source As IEnumerable(Of T), Alternate As T, ParamArray predicate() As Func(Of T, Boolean)) As T
        Return If(source.FirstAny(predicate), Alternate)
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
    ''' Verifica se um atributo foi definido em uma propriedade de uma classe
    ''' </summary>
    ''' <param name="target"></param>
    ''' <param name="attribType"></param>
    ''' <returns></returns>
    <Extension()> Public Function HasAttribute(target As PropertyInfo, attribType As Type)
        Dim attribs = target.GetCustomAttributes(attribType, False)
        Return attribs.Length > 0
    End Function

    ''' <summary>
    ''' Verifica se um atributo foi definido em uma propriedade de uma classe
    ''' </summary>
    ''' <param name="target"></param>
    ''' <returns></returns>
    <Extension()> Public Function HasAttribute(Of T)(target As PropertyInfo)
        Return target.HasAttribute(GetType(T))
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
    ''' Traz o valor de uma enumeração a partir de uma string
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    <Extension()> Public Function GetEnumValueAsString(Of T)(Value As T) As String
        If Not GetType(T).IsEnum Then
            Throw New Exception("T must be an Enumeration type.")
        End If
        Return [Enum].GetName(GetType(T), Value)
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
    ''' Traz uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetProperty(MyObject As Object, Name As String) As PropertyInfo
        If MyObject IsNot Nothing Then
            Return MyObject.GetType().GetProperties().SingleOrDefault(Function(x) x.Name = Name)
        Else
            Return Nothing
        End If
    End Function

    ''' <summary>
    ''' Traz uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetPropertyValue(Of T)(MyObject As Object, Name As String) As T
        If MyObject IsNot Nothing Then
            Dim prop = MyObject.GetType().GetProperties().SingleOrDefault(Function(x) x.Name.ToLower = Name.ToLower)
            If prop IsNot Nothing Then
                Return CType(prop.GetValue(MyObject), T)
            End If
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension()> Public Function GetPropertyParameterFromString(Of Type)(Text As String) As Object()
        Return GetType(Type).GetPropertyParametersFromString(Text)
    End Function

    <Extension()> Public Function ParamSplit(Text As String) As String()
        Dim name As String = Text.GetBefore("(")
        Dim params = Regex.Split(Text.RemoveFirstEqual(name).RemoveFirstEqual("(").RemoveLastEqual(")"), ",(?=(?:[^""]*""[^""]*"")*[^""]*$)")
        Return params
    End Function

    ''' <summary>
    ''' Retorna um array de objetos a partir de uma string que representa uma propriedade de uma classe
    ''' </summary>
    ''' <param name="Text"></param>
    ''' <returns></returns>
    <Extension> Public Function GetPropertyParametersFromString(Type As Type, Text As String) As Object()
        Dim props = Type.GetProperties(BindingFlags.Public + BindingFlags.NonPublic + BindingFlags.Instance)
        Dim name As String = Text.GetBefore("(")
        Dim params = Text.ParamSplit()
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

        If PropertyName.IsNotBlank() Then
            Dim parts = New List(Of String)()
            Dim [stop] = False
            Dim current = ""

            For i As Integer = 0 To PropertyName.Length - 1
                If PropertyName(i) <> "."c Then current &= (PropertyName(i))
                If PropertyName(i) = "("c Then [stop] = True
                If PropertyName(i) = ")"c Then [stop] = False
                If (PropertyName(i) = "."c AndAlso Not [stop]) OrElse i = PropertyName.Length - 1 Then
                    parts.Add(current.ToString())
                    current = ""
                End If
            Next

            Dim prop As PropertyInfo
            Dim propname = parts.First.GetBefore("(")
            If GetPrivate Then
                prop = Type.GetProperty(propname, BindingFlags.Public + BindingFlags.NonPublic + BindingFlags.Instance)
            Else
                prop = Type.GetProperty(propname)
            End If

            Dim exist As Boolean = prop IsNot Nothing
            parts.RemoveAt(0)
            If exist AndAlso parts.Count > 0 Then
                exist = prop.PropertyType.HasProperty(parts.First, GetPrivate)
            End If
            Return exist
        End If
        Return False
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
    <Extension()> Public Function IsIn(Of Type)(Obj As Type, ParamArray List As Type()) As Boolean
        Return Obj.IsIn(If(List, {}).ToList)
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
            Return Text.Contains(Obj.ToString)
        Else
            Return Text.Contains(Obj.ToString, Comparer)
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
        Return If(List, {}).Any(Function(x) Obj.IsIn(x, Comparer))
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
            Return Text.Contains(Obj.ToString)
        Else
            Return Text.Contains(Obj.ToString, Comparer)
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
    Public Function SetPropertyValue(Of Type)(MyObject As Type, PropertyName As String, Value As Object) As Type
        Dim prop = GetProperties(MyObject).Where(Function(p) p.Name = PropertyName).FirstOrDefault
        If prop IsNot Nothing Then
            prop.SetValue(MyObject, Convert.ChangeType(Value, prop.PropertyType))
        End If
        Return MyObject
    End Function


    <Extension()>
    Public Function SetPropertyValueFromCollection(Of Type)(MyObject As Type, PropertyName As String, Collection As CollectionBase) As Type
        GetProperties(MyObject).Where(Function(p) p.Name = PropertyName).FirstOrDefault()?.SetValue(MyObject, Collection(PropertyName))
        Return MyObject
    End Function


    <Extension()> Public Function SetPropertyValue(Of Type As Class, Prop)(obj As Type, Selector As Expression(Of Func(Of Type, Prop)), Value As Prop) As Type
        obj.GetPropertyInfo(Selector).SetValue(obj, Value)
        Return obj
    End Function
End Module