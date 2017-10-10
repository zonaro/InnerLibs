Imports System.IO
Imports InnerLibs
Imports WeifenLuo.WinFormsUI.Docking

Public Class Files
    Inherits DockContent


    Private Sub PopulateTreeView()

        If MainForm.ProjectFolder IsNot Nothing Then
            TreeView1.Nodes.Clear()
            Dim rootNode As TreeNode
            If MainForm.ProjectFolder.Exists Then
                rootNode = New TreeNode(MainForm.ProjectFolder.Name)
                rootNode.Tag = MainForm.ProjectFolder
                GetDirectories(MainForm.ProjectFolder.GetDirectories(), rootNode)
                GetFiles(MainForm.ProjectFolder, rootNode)
                TreeView1.Nodes.Add(rootNode)
            End If
        End If

    End Sub

    Private Sub GetFiles(ByVal dir As DirectoryInfo, ByVal nodeToAddTo As TreeNode)

        For Each file In dir.Search(SearchOption.TopDirectoryOnly, MainForm.Filetypes.Extensions.ToArray)
            Dim aNode As TreeNode
            aNode = New TreeNode(file.Name, 0, 0)
            aNode.Tag = file
            aNode.ImageKey = "file"
            nodeToAddTo.Nodes.Add(aNode)
        Next file



    End Sub

    Private Sub GetDirectories(ByVal subDirs() As DirectoryInfo, ByVal nodeToAddTo As TreeNode)

        Dim aNode As TreeNode
        Dim subSubDirs() As DirectoryInfo
        Dim subDir As DirectoryInfo
        For Each subDir In subDirs
            aNode = New TreeNode(subDir.Name, 0, 0)
            aNode.Tag = subDir
            aNode.ImageKey = "folder"
            subSubDirs = subDir.GetDirectories()
            If subSubDirs.Length <> 0 Then
                GetDirectories(subSubDirs, aNode)
            End If
            nodeToAddTo.Nodes.Add(aNode)
            GetFiles(subDir, nodeToAddTo)
        Next subDir



    End Sub
    Private Sub Files_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateTreeView()
    End Sub

    Private Sub OpenToolStripButton_Click(sender As Object, e As EventArgs) Handles OpenToolStripButton.Click
        Dim d As New FolderBrowserDialog
        If d.ShowDialog = DialogResult.OK Then
            CType(Me.ParentForm, MainForm).ProjectFolder = New DirectoryInfo(d.SelectedPath)
        End If
    End Sub
End Class
