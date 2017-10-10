Imports System.IO
Imports InnerLibs
Imports WeifenLuo.WinFormsUI.Docking

Public Class MainForm

    Property ProjectFolder As DirectoryInfo

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

    End Sub



    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        FileMenu.Show(DockPanel, DockState.DockLeft)
        Theme.ApplyTo(FileMenu.ToolStrip1)
    End Sub

    Private Sub EditorDePersonagemToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditorDePersonagemToolStripMenuItem.Click

    End Sub

    Private Sub NewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NewToolStripMenuItem.Click

    End Sub
End Class