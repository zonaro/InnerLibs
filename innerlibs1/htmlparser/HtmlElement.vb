Imports System.Text
Imports System.ComponentModel

Namespace HtmlParser

    ''' <summary>
    ''' The HtmlElement object represents any HTML element. An element has a name
    ''' and zero or more attributes.
    ''' </summary>
    Public Class HtmlElement
        Inherits HtmlNode
        Protected mName As String
        Protected mNodes As HtmlNodeCollection
        Protected mAttributes As HtmlAttributeCollection
        Protected mIsTerminated As Boolean
        Protected mIsExplicitlyTerminated As Boolean

        ''' <summary>
        ''' This constructs a new HTML element with the specified tag name.
        ''' </summary>
        ''' <param name="name">The name of this element</param>
        Public Sub New(name As String, Optional InnerHtml As String = "")
            mNodes = New HtmlNodeCollection(Me)
            mAttributes = New HtmlAttributeCollection(Me)
            mName = name
            mIsTerminated = False
            Style = New CssProperties(Me)
            If InnerHtml.IsNotBlank Then
                Me.InnerHTML = InnerHtml
            End If
        End Sub

        Sub Mutate(Element As HtmlElement)
            Me.Attributes.Clear()
            mAttributes = Element.Attributes
            Me.InnerHTML = Element.InnerHTML
            Me.Name = Element.Name
        End Sub

        Sub Mutate(Html As String)
            Dim doc = New HtmlDocument(Html)
            If Html.IsBlank Or doc.Nodes.Count = 0 Then
                Destroy()
            Else
                Me.Mutate(doc.Nodes(0))
            End If
        End Sub

        ''' <summary>
        ''' Verify if this element has an specific attribute
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Public Function HasAttribute(Name As String) As Boolean
            Return Me.Attributes.Where(Function(a) a.Name.ToLower = Name.ToLower).Count > 0
        End Function

        ''' <summary>
        ''' Verify if this element has an specific attribute
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Public Function HasClass(ClassName As String) As Boolean
            Return Me.Class(ClassName)
        End Function

        ''' <summary>
        ''' Remove this element from parten element
        ''' </summary>
        Sub Destroy()
            If Me.Parent IsNot Nothing Then
                Me.Parent.Nodes.Remove(Me)
            End If
        End Sub

        ''' <summary>
        ''' The CSS style of element
        ''' </summary>
        ''' <returns></returns>
        <Category("General"), Description("The CSS style of this element"), TypeConverter(GetType(ExpandableObjectConverter))>
        Public ReadOnly Property Style As CssProperties


        ''' <summary>
        ''' Return the child elements of this element (excluding HtmlText)
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ChildElements As HtmlNodeCollection
            Get
                Dim l As New HtmlNodeCollection(Parent)
                l.AddRange(Nodes.Where(Function(p) TypeOf p Is HtmlElement).Select(Function(p) CType(p, HtmlElement)))
                Return l
            End Get
        End Property

        ''' <summary>
        ''' Return the all classes of this element
        ''' </summary>
        ''' <returns></returns>
        Public Function GetClassList() As IList(Of String)
            Dim attr = Me.Attribute("class")
            If attr.IsNotBlank Then
                Return attr.Split({" "c, ControlChars.Tab}, StringSplitOptions.RemoveEmptyEntries)
            End If
            Return New List(Of String)
        End Function


        <Category("General"), Description("The CSS class of this element")>
        Public Property [Class](ClassName As String) As Boolean
            Get
                Dim s = Me.Attribute("class")
                If s.IsNotBlank Then
                    Dim styledic As New List(Of String)
                    For Each item In s.Split(" ")
                        styledic.Add(item.ToLower)
                    Next
                    If styledic.Contains(ClassName.ToLower) Then
                        Return True
                    Else
                        Return False
                    End If
                End If
                Return False
            End Get
            Set(value As Boolean)
                Dim s = Me.Attribute("class")
                Dim styledic As New List(Of String)
                If s.IsNotBlank Then
                    For Each item In s.Split(" ")
                        styledic.Add(item)
                    Next
                    styledic.Add(ClassName)
                End If
                Dim p = ""
                For Each k In styledic.Distinct
                    p.Append(k.ToLower & " ")
                Next
                Me.Attribute("class") = p
            End Set
        End Property

        ''' <summary>
        ''' Return the value of specific attibute
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        '''
        <Category("General"), Description("An especific attribute of element")>
        Property Attribute(Name As String) As String
            Get
                If Name.IsNotBlank Then
                    Try
                        Return Me.Attributes.Item(Name.ToLower).Value
                    Catch ex As Exception
                        Return ""
                    End Try
                End If
                Return ""
            End Get
            Set(value As String)
                If Name.IsNotBlank Then
                    If Me.Attributes.Where(Function(e) e.Name.ToLower = Name.ToLower).Count > 0 Then
                        Me.Attributes.Item(Name.ToLower).Value = value
                    Else
                        Me.Attributes.Add(New HtmlAttribute(Name.ToLower, value))
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Retorna os nomes dos atributos
        ''' </summary>
        ''' <returns></returns>
        <Category("General"), Description("All attributes names of this element")>
        Public ReadOnly Property AttributesNames As IEnumerable(Of String)
            Get
                Return From itens As HtmlAttribute In Me.Attributes
                       Select itens.Name
            End Get
        End Property

        ''' <summary>
        ''' This is the tag name of the element. e.g. BR, BODY, TABLE etc.
        ''' </summary>
        <Category("General"), Description("The name of the tag/element")>
        Public Property Name() As String
            Get
                Return mName.ToLower
            End Get
            Set
                mName = Value.ToLower
            End Set
        End Property

        ''' <summary>
        ''' This is the collection of all child nodes of this one. If this node is actually
        ''' a text node, this will throw an InvalidOperationException exception.
        ''' </summary>
        <Category("General"), Description("The set of child nodes")>
        Public ReadOnly Property Nodes() As HtmlNodeCollection
            Get
                If IsText() Then
                    Return Nothing
                End If
                Return mNodes
            End Get
        End Property

        Default ReadOnly Property Node(Name As String) As HtmlNode
            Get
                Return Me.Nodes(Name)
            End Get
        End Property

        ''' <summary>
        ''' This is the collection of attributes associated with this element.
        ''' </summary>
        <Category("General"), Description("The set of attributes associated with this element")>
        Public ReadOnly Property Attributes() As HtmlAttributeCollection
            Get
                Return mAttributes
            End Get
        End Property

        ''' <summary>
        ''' This flag indicates that the element is explicitly closed using the "<name/>" method.
        ''' </summary>
        Friend Property IsTerminated() As Boolean
            Get
                If Nodes.Count > 0 Then
                    Return False
                Else
                    Return mIsTerminated Or mIsExplicitlyTerminated
                End If
            End Get
            Set
                mIsTerminated = Value
            End Set
        End Property

        ''' <summary>
        ''' This flag indicates that the element is explicitly closed using the /name method.
        ''' </summary>
        Friend Property IsExplicitlyTerminated As Boolean
            Get
                Return mIsExplicitlyTerminated
            End Get
            Set
                mIsExplicitlyTerminated = Value
            End Set
        End Property

        Friend ReadOnly Property NoEscaping() As Boolean
            Get
                Return "script".Equals(Name.ToLower()) OrElse "style".Equals(Name.ToLower())
            End Get
        End Property

        ''' <summary>
        ''' This will return the HTML representation of this element.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return HTML
        End Function

        ''' <summary>
        ''' This will return the HTML representation of this element.
        ''' </summary>
        ''' <returns></returns>
        <Category("General"), Description("The element representation (tag with attributes)")>
        Public Overrides ReadOnly Property ElementRepresentation As String
            Get
                Dim value As String = Convert.ToString("<") & mName
                For Each attribute As HtmlAttribute In Attributes
                    value += " " + attribute.ToString()
                Next
                value += ">"
                Return value
            End Get

        End Property

        <Category("General"), Description("A concatination of all the text associated with this element")>
        Public ReadOnly Property InnerText As String
            Get
                Dim stringBuilder As New StringBuilder()
                For Each node As HtmlNode In Nodes
                    If TypeOf node Is HtmlText Then
                        stringBuilder.Append(DirectCast(node, HtmlText).Text)
                    Else
                        stringBuilder.Append(DirectCast(node, HtmlElement).InnerText)
                    End If
                Next
                Return stringBuilder.ToString()
            End Get
        End Property

        ''' <summary>
        ''' Return a html string of child nodes
        ''' </summary>
        ''' <returns></returns>
        '''
        <Category("Output"), Description("The string representation of all childnodes")>
        Public Property InnerHTML As String
            Get
                Dim s = ""
                For Each node As HtmlNode In Nodes
                    If TypeOf node Is HtmlElement Then
                        s.Append(node.HTML)
                    Else
                        s.Append(CType(node, HtmlText).Text)
                    End If
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
        ''' This will return the HTML for this element and all subnodes.
        ''' </summary>
        <Category("Output"), Description("The HTML string representation of this element and all childnodes")>
        Public Overrides ReadOnly Property HTML() As String
            Get
                Dim html__1 As New StringBuilder()
                html__1.Append(Convert.ToString("<") & mName)
                For Each attribute As HtmlAttribute In Attributes
                    html__1.Append(" " + attribute.HTML)
                Next
                If Nodes.Count > 0 Then
                    html__1.Append(">")
                    For Each node As HtmlNode In Nodes
                        html__1.Append(node.HTML)
                    Next
                    html__1.Append((Convert.ToString("</") & mName) + ">")
                Else
                    If IsExplicitlyTerminated Then
                        html__1.Append((Convert.ToString("></") & mName) + ">")
                    ElseIf IsTerminated Then
                        html__1.Append("/>")
                    Else
                        html__1.Append(">")
                    End If
                End If
                Return html__1.ToString()
            End Get
        End Property

        ''' <summary>
        ''' This will return the XHTML for this element and all subnodes.
        ''' </summary>
        <Category("Output"), Description("The XHTML string representation of this element and all childnodes")>
        Public Overrides ReadOnly Property XHTML() As String
            Get
                If "html".Equals(mName) AndAlso Me.Attributes("xmlns") Is Nothing Then
                    Attributes.Add(New HtmlAttribute("xmlns", "http://www.w3.org/1999/xhtml"))
                End If
                Dim html As New StringBuilder()
                html.Append("<" + mName.ToLower())
                For Each attribute As HtmlAttribute In Attributes
                    html.Append(" " + attribute.XHTML)
                Next
                If IsTerminated Then
                    html.Append("/>")
                Else
                    If Nodes.Count > 0 Then
                        html.Append(">")
                        For Each node As HtmlNode In Nodes
                            html.Append(node.XHTML)
                        Next
                        html.Append("</" + mName.ToLower() + ">")
                    Else
                        html.Append("/>")
                    End If
                End If
                Return html.ToString()
            End Get
        End Property

        ''' <summary>
        ''' Search elements in document using predicate
        ''' </summary>
        ''' <typeparam name="Type">Tipe of Element (<see cref="HtmlText"/> or <see cref="HtmlElement"/></typeparam>
        ''' <param name="predicate">Predicate</param>
        ''' <param name="SearchChildren">Match all child elements</param>
        ''' <returns></returns>
        Public Function Find(Of Type As HtmlNode)(predicate As Func(Of Type, Boolean), Optional SearchChildren As Boolean = True) As HtmlNodeCollection
            Return Me.Nodes.FindElements(Of Type)(predicate, SearchChildren)
        End Function

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================