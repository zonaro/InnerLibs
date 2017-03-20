Imports System.Drawing.Text
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Module ClassManager

    ''' <summary>
    ''' Traz uma Lista com todas as propriedades de um objeto
    ''' </summary>
    ''' <param name="MyObject">Objeto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function GetProperties(MyObject As Object) As List(Of PropertyInfo)
        Return MyObject.GetType().GetProperties().ToList()
    End Function


    Public Function GetValues(Of T)() As List(Of T)
        Return [Enum].GetValues(GetType(T)).Cast(Of T)().ToList
    End Function

    ''' <summary>
    ''' Verifica se o objeto existe dentro de uma Lista, coleção ou array.
    ''' </summary>
    ''' <typeparam name="Type"></typeparam>
    ''' <param name="Obj"></param>
    ''' <param name="List"></param>
    ''' <returns></returns>
    <Extension()> Public Function IsIn(Of Type)(Obj As Type, List As IEnumerable(Of Type)) As Boolean
        Return List.Contains(Obj)
    End Function

    ''' <summary>
    ''' Pega o texto de um arquivo embutido no assembly
    ''' </summary>
    ''' <param name="FileName">Nome do arquivo embutido dentro do assembly (Embedded Resource)</param>
    ''' <returns></returns>
    <Extension()> Public Function GetResourceFileText(Assembly As Assembly, FileName As String) As String
        Return New StreamReader(Assembly.GetManifestResourceStream(FileName)).ReadToEnd
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
