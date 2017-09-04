Imports System.Drawing.Text
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Module ClassTools

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

End Module