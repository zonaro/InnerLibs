Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Web.Script.Serialization
Imports System.Windows.Forms
Imports System.Xml.Serialization

Public Module JsonViewer

    <Extension()> Public Sub LoadObject(TreeView As TreeView, Obj As Object)
        TreeView.Nodes.Clear()
        TreeView.Tag = Obj
        TreeView.Nodes.Add(CreateNode(Obj))
    End Sub

    <Extension()> Function CreateNode(Item As Object) As TreeNode
        Dim basenode As New TreeNode

        If Item IsNot Nothing Then
            basenode.Text = Item.ToString
            basenode.Tag = Item
            Select Case Item.GetType
                Case GetType(Long), GetType(Integer), GetType(Decimal), GetType(Short), GetType(Byte)
                    basenode.ForeColor = Color.DarkGreen
                    Exit Select
                Case GetType(String), GetType(Char)
                    basenode.ForeColor = Color.DarkRed
                    Exit Select
                Case GetType(Boolean)
                    basenode.ForeColor = Color.Blue
                    Exit Select
                Case GetType(Date), GetType(DateTime)
                    basenode.ForeColor = Color.Green
                    Exit Select
                Case Else
                    Select Case True
                        Case IsDictionary(Item)
                            basenode.ForeColor = Color.Red
                            basenode.Text = Item.ToString
                            basenode.Tag = Item
                            For Each i In CType(Item, Dictionary(Of String, Object))
                                basenode.Nodes.Add(CreateNode(i.Value))
                            Next
                            Exit Select
                        Case IsArray(Item) OrElse GetType(IEnumerable).IsAssignableFrom(Item.GetType)
                            For Each i In Item
                                basenode.ForeColor = Color.DarkGray
                                basenode.Text = Item.ToString
                                basenode.Nodes.Add(CreateNode(i))
                            Next
                            Exit Select
                        Case Else
                            For Each p In GetProperties(Item)
                                If p.CanRead AndAlso Not (Attribute.IsDefined(p, GetType(NonSerializedAttribute)) OrElse Attribute.IsDefined(p, GetType(ScriptIgnoreAttribute)) OrElse Attribute.IsDefined(p, GetType(XmlIgnoreAttribute))) Then
                                    Dim newnode As TreeNode = CreateNode(GetPropertyValue(Item, p.Name))
                                    newnode.Text = p.Name & ": " & newnode.Text
                                    newnode.Tag = GetPropertyValue(Item, p.Name)
                                    basenode.Nodes.Add(newnode)
                                End If
                            Next
                    End Select
            End Select
        Else
            basenode.ForeColor = Color.Blue
            basenode.Text = "null"
            basenode.Tag = Nothing
        End If
        Return basenode
    End Function

End Module