Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports InnerLibs.HtmlParser

Namespace HtmlParser.Selectors

    Friend Class IdSelector
        Inherits CssSelector

        Public Overrides ReadOnly Property Token() As String
            Get
                Return "#"
            End Get
        End Property

        Protected Friend Overrides Function FilterCore(currentNodes As HtmlNodeCollection) As HtmlNodeCollection
            Dim l As New HtmlNodeCollection
            l.Add(currentNodes.Where(Function(node As HtmlElement) node.Attribute("id").Equals(Me.Selector, StringComparison.InvariantCultureIgnoreCase)).ToArray)
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