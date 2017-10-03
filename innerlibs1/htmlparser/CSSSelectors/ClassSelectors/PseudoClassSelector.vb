
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser.Selectors
    Friend Class PseudoClassSelector

        Inherits CssSelector

        Public Overrides ReadOnly Property Token() As String
            Get

                Return ":"

            End Get

        End Property


        Protected Friend Overrides Function FilterCore(currentNodes As HtmlNodeCollection) As HtmlNodeCollection

            Dim values As String() = Me.Selector.TrimEnd(")"c).Split({"("c}, 2)

            Dim pseudoClass__1 = PseudoClass.GetPseudoClass(values(0))

            Dim value As String = If(values.Length > 1, values(1), Nothing)


            Return pseudoClass__1.Filter(currentNodes, value)

        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
