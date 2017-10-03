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
            Dim parser As New HtmlParser
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

        ''' <summary>
        ''' Returns the text of all child nodes
        ''' </summary>
        ''' <returns></returns>
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
                Dim html1 As New StringBuilder()

                For Each node As HtmlNode In Nodes
                    html1.Append(node.HTML)

                Next
                Return html1.ToString()
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

        ''' <summary>
        ''' Return the HTML of this document
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return HTML
        End Function

        ''' <summary>
        ''' Save the document as file
        ''' </summary>
        ''' <param name="File">File</param>
        ''' <returns></returns>
        Public Function SaveAs(File As FileInfo) As FileInfo
            Me.HTML.WriteToFile(File)
            Return File
        End Function

        ''' <summary>
        ''' Save the document as file
        ''' </summary>
        ''' <param name="FileName">Filename</param>
        ''' <returns></returns>
        Public Function SaveAs(FileName As String) As FileInfo
            Return Me.SaveAs(New FileInfo(FileName))
        End Function

        ''' <summary>
        ''' Load the document in <see cref="Windows.Forms.WebBrowser"/> control
        ''' </summary>
        ''' <param name="WebBrowser"></param>
        Public Sub LoadInto(ByRef WebBrowser As Windows.Forms.WebBrowser)
            WebBrowser.Navigate("about:blank")
            WebBrowser.Document.Write(Me.ToString)
        End Sub

        ''' <summary>
        ''' Load the nodes of document in <see cref="Windows.Forms.Treeview"/> control
        ''' </summary>
        ''' <param name="TreeView"></param>
        Public Sub LoadInto(ByRef TreeView As Windows.Forms.TreeView)
            TreeView.Nodes.Clear()
            BuildTree(Me.Nodes, TreeView.Nodes)
        End Sub


        Private Sub BuildTree(ByVal nodes As HtmlNodeCollection, ByVal treeNodes As Windows.Forms.TreeNodeCollection)
            Dim value As String = ""

            Dim node As HtmlNode
            For Each node In nodes
                Dim treeNode As New Windows.Forms.TreeNode(node.ElementRepresentation)
                treeNode.Tag = node ' Keep the HtmlNode object in the tag (for when the user clicks on it)
                treeNodes.Add(treeNode)
                If TypeOf (node) Is HtmlElement Then
                    treeNode.SelectedImageIndex = 0
                    treeNode.ImageIndex = 0
                    Me.BuildTree(CType(node, HtmlElement).Nodes, treeNode.Nodes)
                Else
                    treeNode.Text = "(text)" ' This probably has carriage returns in, so don't render the actual HTML here
                    treeNode.SelectedImageIndex = 1
                    treeNode.ImageIndex = 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' Search elements in document using predicate
        ''' </summary>
        ''' <typeparam name="Type">Tipe of Element (<see cref="HtmlText"/> or <see cref="HtmlElement"/></typeparam>
        ''' <param name="predicate">Predicate</param>
        ''' <param name="SearchChildren">Match all child elements</param>
        ''' <returns></returns>
        Public Function FindElements(Of Type As HtmlNode)(predicate As Func(Of Type, Boolean), Optional SearchChildren As Boolean = True) As HtmlNodeCollection
            Return Me.Nodes.FindElements(Of Type)(predicate, SearchChildren)
        End Function

        ''' <summary>
        ''' Return the body element if exist
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Body As HtmlElement
            Get
                Return Me.FindElements(Function(p As HtmlElement) p.Name.ToLower = "body").FirstOrDefault(Function(p) p.Index = 0)
            End Get
        End Property

        ''' <summary>
        ''' Return the head element if exist
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Head As HtmlElement
            Get
                Return Me.FindElements(Function(p As HtmlElement) p.Name.ToLower = "head").FirstOrDefault(Function(p) p.Index = 0)
            End Get
        End Property

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================