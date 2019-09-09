Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls

Public Class DropDownListAdapter
    Inherits System.Web.UI.WebControls.Adapters.WebControlAdapter

    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        Dim list As DropDownList = TryCast(Me.Control, DropDownList)
        Dim currentOptionGroup As String
        Dim renderedOptionGroups As List(Of String) = New List(Of String)()

        For Each item As ListItem In list.Items

            If item.Attributes("OptionGroup") Is Nothing Then
                RenderListItem(item, writer)
            Else
                currentOptionGroup = item.Attributes("OptionGroup")

                If renderedOptionGroups.Contains(currentOptionGroup) Then
                    RenderListItem(item, writer)
                Else

                    If renderedOptionGroups.Count > 0 Then
                        RenderOptionGroupEndTag(writer)
                    End If

                    RenderOptionGroupBeginTag(currentOptionGroup, writer)
                    renderedOptionGroups.Add(currentOptionGroup)
                    RenderListItem(item, writer)
                End If
            End If
        Next

        If renderedOptionGroups.Count > 0 Then
            RenderOptionGroupEndTag(writer)
        End If
    End Sub

    Private Sub RenderOptionGroupBeginTag(ByVal name As String, ByVal writer As HtmlTextWriter)
        writer.WriteBeginTag("optgroup")
        writer.WriteAttribute("label", name)
        writer.Write(HtmlTextWriter.TagRightChar)
        writer.WriteLine()
    End Sub

    Private Sub RenderOptionGroupEndTag(ByVal writer As HtmlTextWriter)
        writer.WriteEndTag("optgroup")
        writer.WriteLine()
    End Sub

    Private Sub RenderListItem(ByVal item As ListItem, ByVal writer As HtmlTextWriter)
        writer.WriteBeginTag("option")
        writer.WriteAttribute("value", item.Value, True)

        If item.Selected Then
            writer.WriteAttribute("selected", "selected", False)
        End If

        For Each key As String In item.Attributes.Keys
            writer.WriteAttribute(key, item.Attributes(key))
        Next

        writer.Write(HtmlTextWriter.TagRightChar)
        HttpUtility.HtmlEncode(item.Text, writer)
        writer.WriteEndTag("option")
        writer.WriteLine()
    End Sub
End Class
