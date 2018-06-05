
Imports HtmlParser
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.PseudoClassSelectors
    <PseudoClassName("read-only")>
    Friend Class ReadOnlyPseudoClass

        Inherits PseudoClass

        Protected Overrides Function CheckNode(node As HtmlElement, parameter As String) As Boolean

            Dim selectors = CssSelector.Parse(parameter)

            Dim nodes = New HtmlNodeCollection

            nodes.Add(node)

            For Each selector In selectors
                If selector.FilterCore(nodes).AsEnumerable.Count(Function(x) x.AsElement.HasAttribute("readonly")) Then
                    Return True
                End If
            Next
            Return False

        End Function

    End Class

    <PseudoClassName("read-write")>
    Friend Class ReadWritePseudoClass

        Inherits PseudoClass

        Protected Overrides Function CheckNode(node As HtmlElement, parameter As String) As Boolean

            Dim selectors = CssSelector.Parse(parameter)

            Dim nodes = New HtmlNodeCollection

            nodes.Add(node)

            For Each selector In selectors
                If selector.FilterCore(nodes).AsEnumerable.Count(Function(x) Not x.AsElement.HasAttribute("readonly")) Then
                    Return True
                End If
            Next
            Return False

        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
