
Imports HtmlParser
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.PseudoClassSelectors
    <PseudoClassName("contains")>
    Friend Class ContainsPseudoClass

        Inherits PseudoClass

        Protected Overrides Function CheckNode(node As HtmlElement, parameter As String) As Boolean
            If parameter Is Nothing Then
                Return node.InnerText.IsNotBlank
            End If
            Dim params = parameter.ParseJSON()
            params = ForceArray(Of Object)(params)
            Select Case params.Length
                Case 0
                    Return node.InnerText.IsNotBlank
                Case 1
                    Return node.InnerText.Contains(params.First)
                Case Else
                    If params(1).IsAny({"False", "false", "0", 0, False}) Then
                        Return node.InnerText.ToLower.Contains(params.First.ToString.ToLower)
                    Else
                        Return node.InnerText.Contains(params.First)
                    End If
            End Select
        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
