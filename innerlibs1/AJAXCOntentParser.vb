Imports System.Text.RegularExpressions

Partial Public Class AJAX

    Public Class ContentMatch
        ReadOnly Property Tag As HtmlTag
        ReadOnly Property Key As String
        ReadOnly Property MatchAllAttributtes As Boolean
        ReadOnly Property ContentAttribute As String = "InnerText"

        Sub New(Key As String, HtmlString As String, MatchAllAttributtes As Boolean, Optional ContentAttribute As String = "InnerText")
            Me.Key = Key
            Me.Tag = New HtmlTag(HtmlString.Replace("{%}", "^(?!\s*$).+"))
            Me.MatchAllAttributtes = MatchAllAttributtes
            Me.ContentAttribute = ContentAttribute
        End Sub

    End Class

    Public Class ContentParser
        Inherits Dictionary(Of String, String())

        Sub New(URL As String, ParamArray ContentMatch As ContentMatch())
            Dim InnerHtml = [GET](Of String)(URL)
            For Each cm In ContentMatch.ToList.Distinct
                Dim content As New List(Of String)
                For Each element In InnerHtml.GetElementsByTagName(cm.Tag.TagName)
                    Dim isMatching As New List(Of Boolean)
                    If cm.MatchAllAttributtes Then
                        isMatching.Add(element.Attributes.Keys.ContainsAll(cm.Tag.Attributes.Keys))
                    End If
                    For Each attr In element.Attributes
                        isMatching.Add(New Regex(cm.Tag.Attribute(attr.Key), RegexOptions.IgnoreCase + RegexOptions.Singleline).IsMatch(element.Attribute(attr.Key)))
                    Next
                    If cm.MatchAllAttributtes Then
                        If Not isMatching.Contains(False) Then
                            content.Add(element.Attribute(cm.ContentAttribute))
                        End If
                    Else
                        If isMatching.Contains(True) Then
                            content.Add(element.Attribute(cm.ContentAttribute))
                        End If
                    End If
                Next
                Me.Add(cm.Key, content.ToArray)
            Next
        End Sub

    End Class
End Class