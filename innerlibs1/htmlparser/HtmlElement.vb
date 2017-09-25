
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
            Me.InnerHTML = InnerHtml

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
        ''' Remove this element from parten element
        ''' </summary>
        Sub Destroy()
            Me.Parent.Nodes.Remove(Me)
        End Sub


        Public Property Style(Key As String) As String
            Get
                Dim s = Me.Attribute("style")
                If s.IsNotBlank Then
                    Dim styledic As New Dictionary(Of String, String)
                    For Each item In s.Split(";")
                        Dim n = item.Split(":")
                        styledic.Add(n(0).ToLower, n(1).ToLower)
                    Next
                    If styledic.ContainsKey(Key.ToLower) Then
                        Return styledic(Key.ToLower)
                    End If
                End If
                Return ""
            End Get
            Set(value As String)
                Dim s = Me.Attribute("style")
                Dim styledic As New Dictionary(Of String, String)

                If s.IsNotBlank Then
                    For Each item In s.Split(";")
                        Dim n = item.Split(":")
                        styledic.Add(n(0).ToLower, n(1).ToLower)
                    Next
                    If styledic.ContainsKey(Key.ToLower) Then
                        styledic(Key.ToLower) = value
                    End If
                End If
                Dim p = ""
                For Each k In styledic
                    p.Append(k.Key.ToLower & ":" & k.Value.ToLower & ";")
                Next
                Me.Attribute("style") = p

            End Set
        End Property


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
        ''' Return de value of specific attibute
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Default Property Attribute(Name As String) As String
            Get
                If Me.Attributes.IndexOf(Name) > -1 Then
                    Return Me.Attributes.Item(Name).Value
                Else
                    Return ""
                End If
            End Get
            Set(value As String)
                If Me.Attributes.IndexOf(Name) > -1 Then
                    Me.Attributes.Item(Name).Value = value
                Else
                    Me.Attributes.Add(New HtmlAttribute(Name, value))
                End If
            End Set
        End Property


        ''' <summary>
        ''' Retorna os nomes dos atributos
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property AttributesNames As IEnumerable(Of String)
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
                Return mName
            End Get
            Set
                mName = Value
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
                    Throw New InvalidOperationException("An HtmlText node does not have child nodes")
                End If
                Return mNodes
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
        ''' This flag indicates that the element is explicitly closed using the "</name>" method.
        ''' </summary>
        Friend Property IsExplicitlyTerminated() As Boolean
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
            Dim value As String = Convert.ToString("<") & mName
            For Each attribute As HtmlAttribute In Attributes
                value += " " + attribute.ToString()
            Next
            value += ">"
            Return value
        End Function




        <Category("General"), Description("A concatination of all the text associated with this element")>
        Public ReadOnly Property Text() As String
            Get
                Dim stringBuilder As New StringBuilder()
                For Each node As HtmlNode In Nodes
                    If TypeOf node Is HtmlText Then
                        stringBuilder.Append(DirectCast(node, HtmlText).Text)
                    End If
                Next
                Return stringBuilder.ToString()
            End Get
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
        ''' This will return the HTML for this element and all subnodes.
        ''' </summary>
        <Category("Output")>
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
        <Category("Output")>
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
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
