
Imports HtmlParser
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.PseudoClassSelectors
    <PseudoClassName("not")>
    Friend Class NotPseudoClass

        Inherits PseudoClass

        Protected Overrides Function CheckNode(node As HtmlElement, parameter As String) As Boolean

            Dim selectors = CssSelector.Parse(parameter)

            Dim nodes = New HtmlNodeCollection

            nodes.Add(node)

            For Each selector In selectors
                If selector.FilterCore(nodes).Count() = 1 Then
                    Return False
                End If
            Next


            Return True

        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
