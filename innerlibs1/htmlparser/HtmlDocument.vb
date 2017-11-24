Imports System.Text
Imports System.Collections
Imports System.ComponentModel
Imports System.IO
Imports System.Xml
Imports System.IO.Packaging

Namespace HtmlParser
    ''' <summary>
    ''' This is the basic HTML document object used to represent a sequence of HTML.
    ''' </summary>

    Public Class HtmlDocument

        Public Const Html5Structure As String = "<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" /><title></title></head><body></body></html>"

        Private mNodes As New HtmlNodeCollection(Nothing)
        Private mXhtmlHeader As String = "<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">"

        ''' <summary>
        ''' This will create a new document object by parsing the HTML specified.
        ''' </summary>
        ''' <param name="UrlOrHTMLString">The URL or HTML to parse.</param>

        Public Sub New(Optional UrlOrHTMLString As String = "", Optional WantSpaces As Boolean = False)
            If UrlOrHTMLString.IsBlank Then
                UrlOrHTMLString = Html5Structure
            End If
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
                Dim txt = ""
                For Each n In Nodes
                    If TypeOf n Is HtmlText Then
                        txt &= CType(n, HtmlText).Text
                    Else
                        txt &= CType(n, HtmlElement).InnerText
                    End If
                Next
                Return txt
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
        ''' Return the <see cref="XmlDocument"/> equivalent to this document
        ''' </summary>
        ''' <returns></returns>
        Function ToXmlDocument() As XmlDocument
            Dim doc As New XmlDocument
            doc.LoadXml(XHTML)
            Return doc
        End Function

        ''' <summary>
        ''' Return the HTML of this document
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return HTML
        End Function


        Public Function GetBytes() As Byte()
            Return Encoding.UTF8.GetBytes(Me.HTML)
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
        ''' This will search though this collection of nodes for all elements with matchs the predicate.
        ''' </summary>
        ''' <typeparam name="NodeType">Type of Node (<see cref="HtmlElement"/> or <see cref="HtmlText"/>)</typeparam>
        ''' <param name="predicate">The predicate to match the nodes</param>
        ''' <param name="SearchChildren">Travesse the child nodes</param>
        ''' <returns></returns>
        Public Function FindElements(Of NodeType As HtmlNode)(predicate As Func(Of NodeType, Boolean), Optional SearchChildren As Boolean = True) As HtmlNodeCollection
            Return Me.Nodes.FindElements(Of NodeType)(predicate, SearchChildren)
        End Function

        ''' <summary>
        ''' Return the body element if exist, otherwise, the first node
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Body As HtmlElement
            Get
                Return Me.FindElements(Function(p As HtmlElement) p.Name.ToLower = "body").FirstOr(Me.Nodes.First)
            End Get
        End Property

        ''' <summary>
        ''' Return the body element if exist, otherwise, the first node
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Head As HtmlElement
            Get
                Return Me.FindElements(Function(p As HtmlElement) p.Name.ToLower = "head").FirstOr(Me.Nodes.First)
            End Get
        End Property

        ''' <summary>
        ''' Travesse DOM with a CSS selector an retireve nodes
        ''' </summary>
        ''' <param name="CssSelector">Teh CSS selector</param>
        ''' <returns>The <see cref="HtmlNodeCollection"/> with matched elements</returns>

        Default ReadOnly Property QuerySelectorAll(CssSelector As String) As HtmlNodeCollection
            Get
                Return Me.Nodes(CssSelector)
            End Get
        End Property

        ''' <summary>
        ''' Execute an <see cref="Action"/> for each <see cref="HtmlElement"/> matched with <paramref name="CssSelector"/>
        ''' </summary>
        ''' <param name="CssSelector">The CSS Selector</param>
        ''' <param name="Actions">Actions</param>
        ''' <returns>The <see cref="HtmlNodeCollection"/> with matched elements</returns>
        Default Public ReadOnly Property QuerySelectorAll(CssSelector As String, ParamArray Actions As Action(Of HtmlElement)()) As HtmlNodeCollection
            Get
                Dim col = Me(CssSelector).GetElements
                For Each action In Actions
                    For Each a In col
                        action(a)
                    Next
                Next
                Dim d As New HtmlNodeCollection
                d.AddRange(col)
                Return d
            End Get
        End Property


        ''' <summary>
        ''' Travesse DOM with a CSS selector an retireve the first node
        ''' </summary>
        ''' <param name="CssSelector">Teh CSS selector</param>
        ''' <returns>The <see cref="HtmlElement"/> matched</returns>
        Public Function QuerySelector(CssSelector As String) As HtmlElement
            Return Me(CssSelector).First
        End Function



    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================