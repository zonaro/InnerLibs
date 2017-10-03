Namespace HtmlParser
    Partial Public Module HtmlParserExtensionMethods



        <System.Runtime.CompilerServices.Extension>
        Public Function GetClassList(node As HtmlElement) As IList(Of String)
            Dim attr = node.Attribute("class")
            If attr.IsNotBlank Then
                Return attr.Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
            End If
            Return New List(Of String)
        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Function NextSiblingElement(node As HtmlElement) As HtmlNode
            Dim rt = node.Next
            While rt IsNot Nothing AndAlso TypeOf rt Is HtmlElement
                rt = rt.Next
            End While
            Return rt
        End Function

        <System.Runtime.CompilerServices.Extension>
        Public Function PreviousSiblingElement(node As HtmlNode) As HtmlNode

            Dim rt = node.Previous


            While rt IsNot Nothing AndAlso TypeOf rt Is HtmlElement
                rt = rt.Previous
            End While
            Return rt

        End Function

    End Module
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
