Imports System.IO
Imports System.Web.Script.Serialization



Public Class FileTree

    <ScriptIgnore>
    ReadOnly Property Parent As FileTree

    ReadOnly Property Name As String
        Get
            Return If(Info IsNot Nothing, Info.Name, "")
        End Get
    End Property

    ReadOnly Property Path As String
        Get
            Return If(Info IsNot Nothing, Info.FullName, "")
        End Get
    End Property

    <ScriptIgnore>
    ReadOnly Property Info As FileSystemInfo

    ReadOnly Property Children As New List(Of FileTree)


    Sub New(Directory As DirectoryInfo, ParamArray FileSearchPatterns As String())
        Me.Info = Directory
        Me.Parent = Nothing
        If FileSearchPatterns Is Nothing OrElse FileSearchPatterns.Count = 0 Then
            FileSearchPatterns = {"*.*"}
        End If
        Me.Children = New List(Of FileTree)({New FileTree(Directory, Me, FileSearchPatterns)}.ToList)
    End Sub

    Friend Sub New(Directory As DirectoryInfo, parent As FileTree, FileSearchPatterns As String())
        Info = Directory
        Me.Parent = parent
        Dim f As New List(Of FileTree)
        For Each d In Directory.GetDirectories
            Dim a = New FileTree(d, Me, FileSearchPatterns)
            If a.Children.Count > 0 Then
                f.Add(a)
            End If
        Next
        For Each pt In FileSearchPatterns
            For Each d In Directory.GetFiles(pt, SearchOption.TopDirectoryOnly)
                f.Add(New FileTree(d, Me))
            Next
        Next
        Me.Children = New List(Of FileTree)(f)
    End Sub

    Friend Sub New(File As FileInfo, parent As FileTree)
        Me.Info = File
        Me.Parent = parent
        Me.Children = New List(Of FileTree)(New List(Of FileTree))
    End Sub


    Public Overrides Function ToString() As String
        Return Info.Name
    End Function


End Class




