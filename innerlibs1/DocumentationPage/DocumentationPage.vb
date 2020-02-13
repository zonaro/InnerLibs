Imports System.IO
Imports System.Runtime.Serialization
Imports InnerLibs
Imports InnerLibs.HtmlParser

Public MustInherit Class DocumentationPage
    Inherits System.Web.UI.Page

    Public Property SourceDirectory As String = "/"
    Public Property ProductName As String = ""
    Public Property LoadingMessage As String = "Loading file..."
    Public Property SearchPlaceholder As String = "Search..."

    Friend Sub LoadFiles(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.LoadComplete
        Dim html = Reflection.Assembly.GetExecutingAssembly.GetResourceFileText("InnerLibs.DocumentationPage.html")
        Response.Clear()
        html = html.Replace("##LOADING_MESSAGE##", LoadingMessage)
        html = html.Replace("##PRODUCT_NAME##", ProductName)
        html = html.Replace("##SEARCH_PLACEHOLDER##", SearchPlaceholder)
        html = html.Replace("##TREEDATA##", MontarMenu)
        Response.WriteEnd(html)
    End Sub

    Friend Function nodeadd(x As FileTree) As Node
        Dim n As New Node
        n.text = Path.GetFileNameWithoutExtension(x.Name)
        If x.Info.GetType = GetType(FileInfo) Then
            Dim tipo = New FileType(CType(x.Info, FileInfo))
            Dim fname = ""
            Select Case True
                Case tipo.IsImage
                    fname = "OpenImage"
                Case tipo.Extensions.Contains(".md")
                    fname = "OpenMD"
                Case Else
                    fname = "OpenHtml"
            End Select
            n.href = "javascript:" & fname & "('/" & x.Path.ReplaceNone(Request.PhysicalApplicationPath) & "');"
            n.href = n.href.Replace("\", "/").Replace(" ", "%20")
            While n.href.Contains("//")
                n.href = n.href.Replace("//", "/")
            End While
        End If
        n.icon = "fa " & x.Info.GetIconByFileType
        If x.Children.Count > 0 Then
            For Each file In x.Children
                n.nodes.Add(nodeadd(file))
            Next
        Else
            n.nodes = Nothing
        End If

        Return n
    End Function



    Friend Function MontarMenu() As String
        Dim menu As New Node()
        menu.state.expanded = True
        Dim caminho = (Request.PhysicalApplicationPath & "/" & SourceDirectory).ToDirectoryInfo
        Dim files As New FileTree(caminho, "*.html", "*.png,", "*.jpg", "*.md")
        Dim lista As New List(Of Node)
        For Each n In files.Children
            lista.Add(nodeadd(n))
        Next
        Return JsonReader.JsonReader.Serialize(lista)
    End Function

    Friend Class State
        Public Property checked As Boolean
        Public Property disabled As Boolean
        Public Property expanded As Boolean
        Public Property selected As Boolean
    End Class

    Friend Class Node
        Public Property text As String
        Public Property icon As String
        Public Property selectedIcon As String
        Public Property color As String
        Public Property backColor As String
        Public Property href As String
        Public Property selectable As Boolean
        Public Property state As New State
        Public Property tags As String()
        <DataMember(IsRequired:=False)>
        Public Property nodes As New List(Of Node)
    End Class


End Class
