Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.Selectors

    Friend Class TagNameSelector

        Inherits CssSelector

        Public Overrides ReadOnly Property Token() As String
            Get
                Return String.Empty
            End Get
        End Property

        Protected Friend Overrides Function FilterCore(currentNodes As HtmlNodeCollection) As HtmlNodeCollection
            Dim l As New HtmlNodeCollection
            l.AddRange(
            currentNodes.Where(Function(node As HtmlElement) node.Name.Equals(Me.Selector, StringComparison.InvariantCultureIgnoreCase)))
            Return l
        End Function

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================