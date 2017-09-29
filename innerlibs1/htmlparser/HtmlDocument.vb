
Imports System.Text
Imports System.Collections
Imports System.ComponentModel
Imports System.IO

Namespace HtmlParser
    ''' <summary>
    ''' This is the basic HTML document object used to represent a sequence of HTML.
    ''' </summary>

    Public Class HtmlDocument
        Private mNodes As New HtmlNodeCollection(Nothing)
        Private mXhtmlHeader As String = "<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">"

        ''' <summary>
        ''' This will create a new document object by parsing the HTML specified.
        ''' </summary>
        ''' <param name="UrlOrHTMLString">The URL or HTML to parse.</param>

        Public Sub New(UrlOrHTMLString As String, Optional WantSpaces As Boolean = False)
            If UrlOrHTMLString.IsURL Then
                UrlOrHTMLString = AJAX.GET(Of String)(UrlOrHTMLString)
            End If
            Dim parser As New HtmlParser()
            parser.RemoveEmptyElementText = Not WantSpaces
            mNodes = parser.Parse(UrlOrHTMLString)
        End Sub




        <Category("General"), Description("This is the DOCTYPE for XHTML production")>
        Public Property DocTypeXHTML() As String

            Get
                Return mXhtmlHeader
            End Get
            Set

                mXhtmlHeader = Value
            End Set
        End Property

        ''' <summary>
        ''' Return a html string of child nodes
        ''' </summary>
        ''' <returns></returns>
        Public Property InnerHTML As String
            Get
                Dim s = ""
                For Each node As HtmlNode In Nodes
                    s.Append(node.HTML)
                Next
                Return s
            End Get
            Set(value As String)
                Dim d As New HtmlDocument(value)
                Me.Nodes.Clear()
                For Each n As HtmlNode In d.Nodes
                    Me.Nodes.Add(n)
                Next
            End Set
        End Property

        Public Property InnerText As String
            Get
                Return InnerHTML.RemoveHTML
            End Get
            Set(value As String)
                Me.InnerHTML = value.RemoveHTML
            End Set
        End Property

        ''' <summary>
        ''' This is the collection of nodes used to represent this document.
        ''' </summary>
        Public ReadOnly Property Nodes() As HtmlNodeCollection
            Get
                Return mNodes
            End Get
        End Property


        ''' <summary>
        ''' This will return the HTML used to represent this document.
        ''' </summary>
        <Category("Output"), Description("The HTML version of this document")>
        Public ReadOnly Property HTML() As String

            Get
                Dim html__1 As New StringBuilder()

                For Each node As HtmlNode In Nodes
                    html__1.Append(node.HTML)

                Next
                Return html__1.ToString()
            End Get
        End Property

        ''' <summary>
        ''' This will return the XHTML document used to represent this document.
        ''' </summary>
        <Category("Output"), Description("The XHTML version of this document")>
        Public ReadOnly Property XHTML() As String

            Get
                Dim html As New StringBuilder()

                If mXhtmlHeader IsNot Nothing Then
                    html.Append(mXhtmlHeader)

                    html.Append(vbCr & vbLf)
                End If

                For Each node As HtmlNode In Nodes
                    html.Append(node.XHTML)

                Next
                Return html.ToString()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return HTML
        End Function


        Public Function SaveAs(File As FileInfo) As FileInfo
            Me.HTML.WriteToFile(File)
            Return File
        End Function

        Public Function SaveAs(FileName As String) As FileInfo
            Return Me.SaveAs(New FileInfo(FileName))
        End Function

    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
