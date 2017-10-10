Imports System.IO
Imports InnerLibs
Imports WeifenLuo.WinFormsUI.Docking

Public Class Files
    Inherits DockContent



    Friend Sub PopulateTreeView(folder As DirectoryInfo)

        If MainForm.ProjectFolder IsNot Nothing Then
            TreeView1.Nodes.Clear()
            Dim rootNode As TreeNode
            If folder.Exists Then
                rootNode = New TreeNode(MainForm.ProjectFolder.Name)
                rootNode.Tag = MainForm.ProjectFolder

                Dim chars As New TreeNode("Personagens")
                For Each i In (Mainform.CharPath.GetFiles("prop.json", SearchOption.AllDirectories))
                    Dim c As New TreeNode(i.Directory.Name) With {.Tag = "char"}

                    chars.Nodes.Add(c)

                Next

                rootNode.Nodes.Add(chars)


                TreeView1.Nodes.Add(rootNode)
            End If
        End If

    End Sub



    Private Sub OpenToolStripButton_Click(sender As Object, e As EventArgs) Handles OpenToolStripButton.Click
        Using d As New FolderBrowserDialog
            d.ShowNewFolderButton = True
            d.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            If d.ShowDialog = DialogResult.OK Then
                MainForm.Instance.ProjectFolder = New DirectoryInfo(d.SelectedPath)
                PopulateTreeView(d.SelectedPath.ToDirectory)
            End If
            MainForm.Instance.MenuStrip1.Enabled = Not MainForm.Instance.ProjectFolder Is Nothing
        End Using

    End Sub


    Sub openthing()
        Select Case TreeView1.SelectedNode.Tag
            Case "char"
                Dim d As New CharacterEditor(TreeView1.SelectedNode.Text)
                d.Show(MainForm.Instance.DockPanel, DockState.Document)
            Case Else

        End Select
    End Sub

    Sub deletething()
        If Confirm("Deseja realmente apagar " & TreeView1.SelectedNode.Text & "?") Then
            Select Case TreeView1.SelectedNode.Tag
                Case "char"
                    For Each d In MainForm.Instance.CharPath.GetDirectories(TreeView1.SelectedNode.Text)
                        d.Delete(True)
                    Next
                Case Else
            End Select
        End If
    End Sub

    Private Sub TreeView1_NodeMouseDoubleClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles TreeView1.NodeMouseDoubleClick
        openthing()
    End Sub

    Private Sub TreeView1_KeyDown(sender As Object, e As KeyEventArgs) Handles TreeView1.KeyDown
        Select Case e.KeyCode
            Case Keys.Enter
                openthing()
            Case Keys.Delete
                deletething()
            Case Else

        End Select
    End Sub
End Class
