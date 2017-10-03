
Imports HtmlParser
Imports InnerLibs.HtmlParser
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser
    Public Module HtmlParserExtensionMethods




        <System.Runtime.CompilerServices.Extension>
        Public Function QuerySelector(node As HtmlElement, cssSelector As String) As HtmlElement
            Return CType(node.QuerySelectorAll(cssSelector).FirstOrDefault(), HtmlElement)
        End Function



        <System.Runtime.CompilerServices.Extension>
        Public Function QuerySelectorAll(node As HtmlElement, cssSelector As String) As HtmlNodeCollection
            Return node.ChildElements.QuerySelectorAll(cssSelector)
        End Function


        <System.Runtime.CompilerServices.Extension>
        Public Function QuerySelectorAll(nodes As HtmlNodeCollection, CssSelector As String) As HtmlNodeCollection

            If CssSelector Is Nothing Then
                Throw New ArgumentNullException("cssSelector")
            End If

            If CssSelector.Contains(","c) Then
                Dim combinedSelectors = CssSelector.Split(","c)
                Dim rt = nodes.QuerySelectorAll(combinedSelectors(0))
                For Each s In combinedSelectors.Skip(1)
                    For Each n In nodes.QuerySelectorAll(s)
                        If Not rt.Contains(n) Then
                            rt.Add(n)
                        End If

                    Next
                Next


                Return rt

            End If


            CssSelector = CssSelector.Trim()

            Dim selectors = InnerLibs.HtmlParser.CssSelector.Parse(CssSelector)

            Dim allowTraverse As Boolean = True

            For Each selector In selectors

                If allowTraverse AndAlso selector.AllowTraverse Then
                    nodes = Traverse(nodes)

                End If


                nodes = selector.Filter(nodes)

                allowTraverse = selector.AllowTraverse

            Next

            Dim l As New HtmlNodeCollection
            l.AddRange(nodes.Distinct())
            Return l
        End Function


        Private Function Traverse(nodes As HtmlNodeCollection) As HtmlNodeCollection
            Dim l As New HtmlNodeCollection

            For Each node As HtmlElement In nodes
                For Each n As HtmlElement In Traverse(node).Where(Function(i) TypeOf i Is HtmlElement)
                    If n IsNot Nothing Then
                        l.Add(n)
                    End If
                Next

            Next
            Return l
        End Function

        Private Function Traverse(node As HtmlElement) As HtmlNodeCollection

            Dim l As New HtmlNodeCollection
            l.Add(node)

            For Each child As HtmlElement In node.ChildElements
                ''aqui é os node de verdade'
                For Each n As HtmlElement In Traverse(child)
                    l.Add(n)
                Next

            Next
            Return l
        End Function
    End Module

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
