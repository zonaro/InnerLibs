Imports System.Collections.Specialized
Imports System.Drawing.Text
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Public Module ClassTools


    ''' <summary>
    ''' Determines if a type is numeric.  Nullable numeric types are considered numeric.
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
    ''' Conta de maneira distinta items de uma coleçao
    ''' </summary>
    ''' <typeparam name="Type">TIpo de Objeto</typeparam>
    ''' <param name="Arr">colecao</param>
    ''' <returns></returns>
    <Extension()> Public Function DistinctCount(Of Type)(Arr As IEnumerable(Of Type)) As IDictionary(Of Type, Long)
        Return Arr.Distinct.Select(Function(p) New KeyValuePair(Of Type, Long)(p, Arr.Where(Function(x) x.Equals(p)).LongCount)).OrderByDescending(Function(p) p.Value).ToDictionary(Function(p) p.Key, Function(p) p.Value)
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
    ''' Traz uma Lista com todas as propriedades de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetProperties(MyObject As Object) As List(Of PropertyInfo)
        Return MyObject.GetType().GetProperties().ToList()
    End Function

    ''' <summary>
    ''' Traz o valor de uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <param name="PropertyName">Nome da properiedade</param>
    ''' <typeparam name="Type">Tipo do Objeto</typeparam>
    ''' <returns></returns>
    <Extension()>
    Public Function GetPropertyValue(Of Type)(MyObject As Object, PropertyName As String) As Type
        Try
            Return CType(GetProperties(MyObject).Where(Function(p) p.Name = PropertyName).First.GetValue(MyObject), Type)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Traz o valor de uma propriedade de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <param name="PropertyName">Nome da properiedade</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetPropertyValue(MyObject As Object, PropertyName As String) As Object
        Return GetPropertyValue(Of Object)(MyObject, PropertyName)
    End Function

    ''' <summary>
    ''' Traz todos os Valores de uma enumeração
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <returns></returns>
    Public Function GetEnumValues(Of T)() As List(Of T)
        Return [Enum].GetValues(GetType(T)).Cast(Of T)().ToList
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
    ''' Verifica se o objeto existe dentro de uma Lista, coleção ou array.
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="Obj">objeto</param>
    ''' <param name="List">Lista</param>
    ''' <returns></returns>
    <Extension()> Public Function IsIn(Of Type)(Obj As Type, List As IEnumerable(Of Type)) As Boolean
        Return List.Contains(Obj)
    End Function

    ''' <summary>
    ''' Verifica se o objeto existe dentro de um texto
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="Obj">objeto</param>
    ''' <param name="TExt">Texto</param>
    ''' <returns></returns>
    <Extension()> Public Function IsIn(Of Type)(Obj As Type, Text As String) As Boolean
        Return Not IsNothing(Obj) AndAlso Text.Contains(Obj.ToString)
    End Function

    ''' <summary>
    ''' Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array.
    ''' </summary>
    ''' <typeparam name="Type">Tipo do objeto</typeparam>
    ''' <param name="List1">Lista 1</param>
    ''' <param name="List2">Lista2</param>
    ''' <returns></returns>
    <Extension()> Public Function ContainsAll(Of Type)(List1 As IEnumerable(Of Type), List2 As IEnumerable(Of Type)) As Boolean
        For Each value As Type In List2
            If IsNothing(List1) OrElse IsNothing(List2) OrElse Not List1.Contains(value) Then
                Return False
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
    <Extension()> Public Function ContainsAny(Of Type)(List1 As IEnumerable(Of Type), List2 As IEnumerable(Of Type)) As Boolean
        For Each value As Type In List2
            If Not IsNothing(List1) AndAlso List1.Contains(value) Then
                Return True
            End If
        Next
        Return False
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
    ''' Adiciona uma fonte a uma PrivateFontCollection a partir de um Resource
    ''' </summary>
    ''' <param name="FontCollection">Colecao</param>
    ''' <param name="FileName">Nome do arquivo da fonte</param>
    <Extension()> Public Sub AddFontFromResource(ByRef FontCollection As PrivateFontCollection, Assembly As Assembly, FileName As String)
        Dim fontBytes = Assembly.GetResourceBytes(FileName)
        Dim fontData = Marshal.AllocCoTaskMem(fontBytes.Length)
        Marshal.Copy(fontBytes, 0, fontData, fontBytes.Length)
        FontCollection.AddMemoryFont(fontData, fontBytes.Length)
        Marshal.FreeCoTaskMem(fontData)
    End Sub

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
    ''' Cria um objeto de um tipo especifico a partir de um <see cref="NameValueCollection"/>
    ''' </summary>
    ''' <typeparam name="Type">Tipo do Objeto</typeparam>
    ''' <param name="Collection">Colecao</param>
    ''' <returns></returns>
    <Extension()>
    Public Function CreateObject(Of Type)(Collection As NameValueCollection, ParamArray Keys As String()) As Type
        Dim obj = Activator.CreateInstance(Of Type)()
        Dim PROPS = obj.GetProperties
        If IsNothing(Keys) OrElse Keys.LongCount = 0 Then Keys = Collection.AllKeys
        For Each key As String In Collection.Keys
            If key.IsIn(Keys) And key.IsIn(PROPS.Select(Function(p) p.Name)) Then
                For Each prop In PROPS
                    If prop.PropertyType.IsArray Then
                        prop.SetValue(obj, Conversion.CTypeDynamic(Collection.GetValues(prop.Name), prop.GetType))
                    Else
                        prop.SetValue(obj, Conversion.CTypeDynamic(Collection(prop.Name), prop.GetType))
                    End If
                Next
            End If
        Next
        Return obj
    End Function

End Module