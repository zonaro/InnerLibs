
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.Selectors
    Friend Class SiblingSelector

        Inherits CssSelector

        Public Overrides ReadOnly Property AllowTraverse() As Boolean
            Get

                Return False

            End Get

        End Property


        Public Overrides ReadOnly Property Token() As String
            Get

                Return "~"

            End Get

        End Property


        Protected Friend Overrides Function FilterCore(currentNodes As HtmlNodeCollection) As HtmlNodeCollection
            Dim l As New HtmlNodeCollection
            l.AddRange(currentNodes.SelectMany(Function(node)
                                                   Dim idx = node.Index
                                                   Return node.Parent.Nodes.Where(Function(i) TypeOf i Is HtmlElement).Skip(idx + 1)
                                               End Function).ToArray)
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
