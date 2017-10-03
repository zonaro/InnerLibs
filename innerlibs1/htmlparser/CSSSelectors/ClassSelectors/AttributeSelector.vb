Imports System.Globalization

Namespace HtmlParser.Selectors
    Friend Class AttributeSelector
        Inherits CssSelector
        Public Overrides ReadOnly Property Token() As String
            Get
                Return "["
            End Get
        End Property

        Protected Friend Overrides Function FilterCore(currentNodes As HtmlNodeCollection) As HtmlNodeCollection
            Return currentNodes.FindElements(Me.GetFilter())
        End Function

        Private Function GetFilter() As Func(Of HtmlElement, Boolean)
            Dim filter As String = Me.Selector.Trim("["c, "]"c)

            Dim idx As Integer = filter.IndexOf("="c)

            If idx = 0 Then
                Throw New InvalidOperationException("Uso inválido de seletor por atributo: " + Me.Selector)
            End If

            If idx < 0 Then
                Return Function(node As HtmlElement) node.HasAttribute(filter)
            End If

            Dim operation = GetOperation(filter(idx - 1))

            If Not Char.IsLetterOrDigit(filter(idx - 1)) Then
                filter = filter.Remove(idx - 1, 1)
            End If

            Dim values As String() = filter.Split({"="c}, 2)
            filter = values(0)
            Dim value As String = values(1)

            If value(0) = value(value.Length - 1) AndAlso (value(0) = """"c OrElse value(0) = "'"c) Then
                value = value.Substring(1, value.Length - 2)
            End If

            Return Function(node As HtmlElement) node.HasAttribute(filter) AndAlso operation(node.Attribute(filter), value)
        End Function

        Shared s_Culture As CultureInfo = CultureInfo.GetCultureInfo("en")
        Private Function GetOperation(value As Char) As Func(Of String, String, Boolean)
            If Char.IsLetterOrDigit(value) Then
                Return Function(attr, v) attr = v
            End If

            Select Case value
                Case "*"c
                    Return Function(attr, v) attr = v OrElse attr.Contains(v)
                Case "^"c
                    Return Function(attr, v) attr.StartsWith(v)
                Case "$"c
                    Return Function(attr, v) attr.EndsWith(v)
                Case "~"c
                    Return Function(attr, v) attr.Split(" "c).Contains(v)
            End Select

            Throw New NotSupportedException("Uso inválido de seletor por atributo: " + Me.Selector)
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
