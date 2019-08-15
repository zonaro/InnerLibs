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
                    basenode.ForeColor = Color.Magenta
                    Exit Select
                Case Else
                    Select Case True
                        Case IsDictionary(Item)
                            basenode.ForeColor = Color.Red
                            basenode.Text = Item.GetType().Name
                            basenode.Tag = Item
                            For Each i In CType(Item, Dictionary(Of String, Object))
                                Dim thenode = If(basenode.Parent, basenode)
                                thenode.Nodes.Add(CreateNode(i))
                            Next
                            If basenode.Parent IsNot Nothing Then
                                basenode.Remove()
                            End If
                            Exit Select
                        Case GetType(KeyValuePair(Of String, Object)).IsAssignableFrom(Item.GetType)
                            Dim keytem = CType(Item, KeyValuePair(Of String, Object))
                            basenode.Text = keytem.Key
                            basenode.Nodes.Add(CreateNode(keytem.Value))
                            Exit Select
                        Case IsArray(Item) OrElse GetType(IEnumerable).IsAssignableFrom(Item.GetType)
                            For Each i In Item
                                basenode.ForeColor = Color.DarkGray
                                basenode.Text = Item.GetType().Name & Item.Length.ToString.Quote("[")
                                basenode.Nodes.Add(CreateNode(i))
                            Next
                            Exit Select
                        Case Else
                            For Each p In GetProperties(Item)
                                If p.CanRead AndAlso Not (Attribute.IsDefined(p, GetType(NonSerializedAttribute)) OrElse Attribute.IsDefined(p, GetType(ScriptIgnoreAttribute)) OrElse Attribute.IsDefined(p, GetType(XmlIgnoreAttribute))) Then
                                    Dim newnode As TreeNode = CreateNode(GetPropertyValue(Item, p.Name))
                                    newnode.Nodes.Add(New TreeNode(newnode.Text) With {.Tag = newnode.Text})
                                    newnode.Text = p.Name.ToTitle
                                    newnode.Tag = GetPropertyValue(Item, p.Name)
                                    basenode.Nodes.Add(newnode)
                                End If
                            Next
                    End Select
            End Select
        Else
            basenode.ForeColor = Color.LightBlue
            basenode.Text = "null"
            basenode.Tag = Nothing
        End If
        Return basenode
    End Function

End Module