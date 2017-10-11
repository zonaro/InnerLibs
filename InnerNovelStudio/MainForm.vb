Imports System.IO
Imports System.Runtime.CompilerServices
Imports InnerLibs
Imports WeifenLuo.WinFormsUI.Docking

Public Class MainForm


    Public Shared ReadOnly Property Instance As MainForm
        Get
            Return CType(Application.OpenForms("MainForm"), MainForm)
        End Get

    End Property

    Property ProjectFolder As DirectoryInfo

    ReadOnly Property CharPath As DirectoryInfo
        Get
            Return (ProjectFolder.FullName & Path.DirectorySeparatorChar & "char").ToDirectory
        End Get
    End Property



    Shared ReadOnly Property Filetypes As FileTypeList
        Get
            Dim lista As New FileTypeList
            lista.AddRange(FileType.GetFileTypeList.Where(Function(p) p.IsAudio Or p.IsImage Or p.IsText Or p.Extensions.ContainsAny({".json", ".txt", ".md", ".js", ".css"})))
            Return lista
        End Get

    End Property

    Property DockPanel As DockPanel = New DockPanel With {.Dock = DockStyle.Fill}

    Property FileMenu As New Files With {.Text = "Arquivos do Projeto", .HideOnClose = 1}
    Property Theme As New VS2015DarkTheme()

    Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        Me.Controls.Add(DockPanel)
        DockPanel.BringToFront()

        DockPanel.Theme = Theme

        Theme.ApplyTo(MenuStrip1)

    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        openfiles()
    End Sub


    Private Sub ToolStripMenuItem3_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem3.Click
        Dim per As New CharacterEditor
        per.Show(DockPanel, DockState.Document)
    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MenuStrip1.Enabled = False
        openfiles()
    End Sub

    Sub openfiles()
        FileMenu.Show(DockPanel, DockState.DockLeft)
        Theme.ApplyTo(FileMenu.ToolStrip1)
    End Sub

    Sub Salvar(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Select Case Me.ActiveMdiChild.GetType
            Case GetType(CharacterEditor)
                CType(Me.ActiveMdiChild, CharacterEditor).Salvar()
        End Select
        Me.ProjectFolder.CleanDirectory()
        FileMenu.PopulateTreeView(ProjectFolder)
    End Sub


End Class

Public Module utils

    Public Function ImageName(img As Image, Name As String)
        Dim tipo = img.

    End Function

End Module
