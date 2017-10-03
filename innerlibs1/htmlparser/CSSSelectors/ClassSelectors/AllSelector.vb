
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.Selectors
    Friend Class AllSelector
        Inherits CssSelector
        Public Overrides ReadOnly Property Token() As String
            Get
                Return "*"
            End Get
        End Property

        Protected Friend Overrides Function FilterCore(currentNodes As HtmlNodeCollection) As HtmlNodeCollection
            Return currentNodes
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
