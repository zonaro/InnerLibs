
Imports HtmlParser
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.PseudoClassSelectors
    <PseudoClassName("nth-child")> _
    Friend Class NthChildPseudoClass

        Inherits PseudoClass

        Protected Overrides Function CheckNode(node As HtmlElement, parameter As String) As Boolean

            Return node.Index = Integer.Parse(parameter) - 1
        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
