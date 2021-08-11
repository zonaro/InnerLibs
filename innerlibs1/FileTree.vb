Imports System.Drawing
Imports System.IO




Public Class FileTree

    ReadOnly Property Parent As FileTree

    ReadOnly Property Name As String
        Get
            Return If(Info IsNot Nothing, Info.Name, "")
        End Get
    End Property

    ReadOnly Property Icon As Bitmap
        Get
            Return GetIcon(Info).ToBitmap()
        End Get
    End Property

    ReadOnly Property Path As String
        Get
            Return If(Info IsNot Nothing, Info.FullName, "")
        End Get
    End Property

    ReadOnly Property FileType As FileType
        Get
            If Path.IsFilePath Then
                Return FileType.GetFileType(Path)
            End If
            Return Nothing
        End Get
    End Property

    ReadOnly Property TypeDescription As String
        Get
            If IsDirectory Then
                Return "Directory"
            End If
            Return FileType?.Description
        End Get
    End Property

    Public ReadOnly Property IsFile As Boolean
        Get
            Return Path.IsFilePath
        End Get
    End Property

    Public ReadOnly Property IsDirectory As Boolean
        Get
            Return Path.IsDirectoryPath
        End Get
    End Property

    ReadOnly Property Info As FileSystemInfo

    Private _children As New List(Of FileTree)

    ReadOnly Property Children As IEnumerable(Of FileTree)
        Get
            Return _children.AsEnumerable()
        End Get
    End Property

    Sub New(Directory As String, ParamArray FileSearchPatterns As String())
        Me.New(New DirectoryInfo(Directory), FileSearchPatterns)
    End Sub

    Sub New(Directory As DirectoryInfo, ParamArray FileSearchPatterns As String())
        Me.Info = Directory
        Me.Parent = Nothing
        If FileSearchPatterns Is Nothing OrElse FileSearchPatterns.Count = 0 Then
            FileSearchPatterns = {"*"}
        End If
        Me._children = New List(Of FileTree)({New FileTree(Directory, Me, FileSearchPatterns)}.ToList)
    End Sub

    Friend Sub New(Directory As DirectoryInfo, parent As FileTree, FileSearchPatterns As String())
        Info = Directory
        Me.Parent = parent
        Dim f As New List(Of FileTree)
        For Each d In Directory.GetDirectories
            Dim a = New FileTree(d, Me, FileSearchPatterns)
            If a._children.Any Then
                f.Add(a)
            End If
        Next
        For Each pt In FileSearchPatterns
            For Each d In Directory.GetFiles(pt, SearchOption.TopDirectoryOnly)
                f.Add(New FileTree(d, Me))
            Next
        Next
        Me._children = New List(Of FileTree)(f)
    End Sub

    Friend Sub New(File As FileInfo, parent As FileTree)
        Me.Info = File
        Me.Parent = parent
        Me._children = New List(Of FileTree)(New List(Of FileTree))
    End Sub


    Public Overrides Function ToString() As String
        Return Info.Name
    End Function


End Class




