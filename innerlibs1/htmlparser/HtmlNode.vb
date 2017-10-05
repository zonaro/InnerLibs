Imports System.Collections
Imports System.ComponentModel
Imports System.Linq

Namespace HtmlParser

    ''' <summary>
    ''' The HtmlNode is the base for all objects that may appear in HTML. Currently,
    ''' this implemention only supports HtmlText and HtmlElement node types.
    ''' </summary>
    Public MustInherit Class HtmlNode
        Protected Friend mParent As HtmlElement

        ''' <summary>
        ''' This constructor is used by the subclasses.
        ''' </summary>
        Protected Sub New()
            mParent = Nothing
        End Sub

        ''' <summary>
        ''' This will render the node as it would appear in HTML.
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Overrides Function ToString() As String

        Public MustOverride ReadOnly Property ElementRepresentation As String

        ''' <summary>
        ''' This will return the parent of this node, or null if there is none.
        ''' </summary>
        <Category("Navigation"), Description("The parent node of this one")>
        Public ReadOnly Property Parent() As HtmlElement
            Get
                Return mParent
            End Get
        End Property

        ''' <summary>
        ''' This will return the next sibling node. If this is the last one, it will return null.
        ''' </summary>
        <Category("Navigation"), Description("The next sibling node")>
        Public ReadOnly Property [Next]() As HtmlNode
            Get
                If Index = -1 Then
                    Return Nothing
                Else
                    If Parent.Nodes.Count > Index + 1 Then
                        Return Parent.Nodes(Index + 1)
                    Else
                        Return Nothing
                    End If
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return the previous sibling node. If this is the first one, it will return null.
        ''' </summary>
        <Category("Navigation"), Description("The previous sibling node")>
        Public ReadOnly Property Previous() As HtmlNode
            Get
                If Index = -1 Then
                    Return Nothing
                Else
                    If Index > 0 Then
                        Return Parent.Nodes(Index - 1)
                    Else
                        Return Nothing
                    End If
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return the first child node. If there are no children, this
        ''' will return null.
        ''' </summary>
        <Category("Navigation"), Description("The first child of this node")>
        Public ReadOnly Property FirstChild() As HtmlNode
            Get
                If TypeOf Me Is HtmlElement Then
                    If DirectCast(Me, HtmlElement).Nodes.Count = 0 Then
                        Return Nothing
                    Else
                        Return DirectCast(Me, HtmlElement).Nodes(0)
                    End If
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return the last child node. If there are no children, this
        ''' will return null.
        ''' </summary>
        <Category("Navigation"), Description("The last child of this node")>
        Public ReadOnly Property LastChild() As HtmlNode
            Get
                If TypeOf Me Is HtmlElement Then
                    If DirectCast(Me, HtmlElement).Nodes.Count = 0 Then
                        Return Nothing
                    Else
                        Return DirectCast(Me, HtmlElement).Nodes(DirectCast(Me, HtmlElement).Nodes.Count - 1)
                    End If
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return the index position within the parent's nodes that this one resides.
        ''' If this is not in a collection, this will return -1.
        ''' </summary>
        <Category("Navigation"), Description("The zero-based index of this node in the parent's nodes collection")>
        Public ReadOnly Property Index() As Integer
            Get
                If mParent Is Nothing Then
                    Return -1
                Else
                    Return mParent.Nodes.IndexOf(Me)
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return true if this is a root node (has no parent).
        ''' </summary>
        <Category("Navigation"), Description("Is this node a root node?")>
        Public ReadOnly Property IsRoot() As Boolean
            Get
                Return mParent Is Nothing
            End Get
        End Property

        ''' <summary>
        ''' This will return true if this is a child node (has a parent).
        ''' </summary>
        <Category("Navigation"), Description("Is this node a child of another?")>
        Public ReadOnly Property IsChild() As Boolean
            Get
                Return mParent IsNot Nothing
            End Get
        End Property

        <Category("Navigation"), Description("Does this node have any children?")>
        Public ReadOnly Property IsParent() As Boolean
            Get
                If TypeOf Me Is HtmlElement Then
                    Return DirectCast(Me, HtmlElement).Nodes.Count > 0
                Else
                    Return False
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return true if the node passed is a descendent of this node.
        ''' </summary>
        ''' <param name="node">The node that might be the parent or grandparent (etc.)</param>
        ''' <returns>True if this node is a descendent of the one passed in.</returns>
        <Category("Relationships")>
        Public Function IsDescendentOf(node As HtmlNode) As Boolean
            Dim parent As HtmlNode = mParent
            While parent IsNot Nothing
                If parent Is node Then
                    Return True
                End If
                parent = parent.Parent
            End While
            Return False
        End Function

        ''' <summary>
        ''' This will return true if the node passed is one of the children or grandchildren of this node.
        ''' </summary>
        ''' <param name="node">The node that might be a child.</param>
        ''' <returns>True if this node is an ancestor of the one specified.</returns>
        <Category("Relationships")>
        Public Function IsAncestorOf(node As HtmlNode) As Boolean
            Return node.IsDescendentOf(Me)
        End Function

        ''' <summary>
        ''' This will return the ancstor that is common to this node and the one specified.
        ''' </summary>
        ''' <param name="node">The possible node that is relative</param>
        ''' <returns>The common ancestor, or null if there is none</returns>
        <Category("Relationships")>
        Public Function GetCommonAncestor(node As HtmlNode) As HtmlNode
            Dim thisParent As HtmlNode = Me
            While thisParent IsNot Nothing
                Dim thatParent As HtmlNode = node
                While thatParent IsNot Nothing
                    If thisParent Is thatParent Then
                        Return thisParent
                    End If
                    thatParent = thatParent.Parent
                End While
                thisParent = thisParent.Parent
            End While
            Return Nothing
        End Function

        ''' <summary>
        ''' This will remove this node and all child nodes from the tree. If this
        ''' is a root node, this operation will do nothing.
        ''' </summary>
        <Category("General")>
        Public Sub Remove()
            If mParent IsNot Nothing Then
                mParent.Nodes.RemoveAt(Me.Index)
            End If
        End Sub

        ''' <summary>
        ''' Internal method to maintain the identity of the parent node.
        ''' </summary>
        ''' <param name="parentNode">The parent node of this one</param>
        Friend Sub SetParent(parentNode As HtmlElement)
            mParent = parentNode
        End Sub

        ''' <summary>
        ''' This will return the full HTML to represent this node (and all child nodes).
        ''' </summary>
        <Category("Output"), Description("The HTML that represents this node and all the children")>
        Public MustOverride ReadOnly Property HTML() As String

        ''' <summary>
        ''' This will return the full XHTML to represent this node (and all child nodes)
        ''' </summary>
        <Category("Output"), Description("The XHTML that represents this node and all the children")>
        Public MustOverride ReadOnly Property XHTML() As String

        <Category("General"), Description("This is true if this is a text node")>
        Public Function IsText() As Boolean
            Return TypeOf Me Is HtmlText
        End Function

        <Category("General"), Description("This is true if this is an element node")>
        Public Function IsElement() As Boolean
            Return TypeOf Me Is HtmlElement
        End Function

    End Class

    ''' <summary>
    ''' This object represents a collection of HtmlNodes, which can be either HtmlText
    ''' or HtmlElement objects. The order in which the nodes occur directly corresponds
    ''' to the order in which they appear in the original HTML document.
    ''' </summary>
    Public Class HtmlNodeCollection
        Inherits List(Of HtmlNode)
        Private mParent As HtmlElement

        ' Public constructor to create an empty collection.
        Public Sub New()
            mParent = Nothing
        End Sub

        ''' <summary>
        ''' A collection is usually associated with a parent node (an HtmlElement, actually)
        ''' but you can pass null to implement an abstracted collection.
        ''' </summary>
        ''' <param name="parent">The parent element, or null if it is not appropriate</param>
        Friend Sub New(parent As HtmlElement)
            mParent = parent
        End Sub

        Public Shadows Sub Insert(Index As Integer, Element As HtmlNode)
            Element.mParent = Me.mParent
            MyBase.Insert(Index, Element)
        End Sub

        ''' <summary>
        ''' This will search though this collection of nodes for all elements with the
        ''' specified name. If you want to search the subnodes recursively, you should
        ''' pass True as the parameter in searchChildren. This search is guaranteed to
        ''' return nodes in the order in which they are found in the document.
        ''' </summary>
        ''' <param name="name">The name of the element to find</param>
        ''' <param name="searchChildren">True if you want to search sub-nodes, False to
        ''' only search this collection.</param>
        ''' <returns>A collection of all the nodes that macth.</returns>
        Public Function GetElementsByTagName(Name As String, Optional SearchChildren As Boolean = True) As HtmlNodeCollection
            Dim results As New HtmlNodeCollection(Nothing)
            For Each node As HtmlNode In Me
                If TypeOf node Is HtmlElement Then
                    If DirectCast(node, HtmlElement).Name.ToLower().Equals(Name.ToLower()) Then
                        results.Add(node)
                    End If
                    If SearchChildren Then
                        For Each matchedChild As HtmlNode In DirectCast(node, HtmlElement).Nodes.GetElementsByTagName(Name, SearchChildren)
                            results.Add(matchedChild)
                        Next
                    End If
                End If
            Next
            Return results
        End Function

        ''' <summary>
        ''' This will search though this collection of nodes for all elements with the an
        ''' attribute with the given name.
        ''' </summary>
        ''' <param name="attributeName">The name of the attribute to find</param>
        ''' <param name="searchChildren">True if you want to search sub-nodes, False to
        ''' only search this collection.</param>
        ''' <returns>A collection of all the nodes that macth.</returns>
        Public Function GetElementsByAttributeName(AttributeName As String, Optional SearchChildren As Boolean = True) As HtmlNodeCollection
            Dim results As New HtmlNodeCollection(Nothing)
            For Each node As HtmlNode In Me
                If TypeOf node Is HtmlElement Then
                    For Each attribute As HtmlAttribute In DirectCast(node, HtmlElement).Attributes
                        If attribute.Name.ToLower() = AttributeName.ToLower() Then
                            results.Add(node)
                            Exit For
                        End If
                    Next
                    If SearchChildren Then
                        For Each matchedChild As HtmlNode In DirectCast(node, HtmlElement).Nodes.GetElementsByAttributeName(AttributeName, SearchChildren)
                            results.Add(matchedChild)
                        Next
                    End If
                End If
            Next
            Return results
        End Function

        ''' <summary>
        ''' This will search though this collection of nodes for all elements with the an
        ''' attribute with the given name and value.
        ''' </summary>
        ''' <param name="attributeName">The name of the attribute to find</param>
        ''' <returns>A collection of all the nodes that macth.</returns>
        Public Function GetElementsByAttributeNameValue(AttributeName As String, AttributeValue As String, Optional searchChildren As Boolean = True) As HtmlNodeCollection
            Dim results As New HtmlNodeCollection(Nothing)
            For Each node As HtmlNode In Me
                If TypeOf node Is HtmlElement Then
                    For Each attribute As HtmlAttribute In DirectCast(node, HtmlElement).Attributes
                        If attribute.Name.ToLower().Equals(AttributeName.ToLower()) Then
                            If attribute.Value.ToLower().Equals(AttributeValue.ToLower()) Then
                                results.Add(node)
                            End If
                            Exit For
                        End If
                    Next
                    If searchChildren Then
                        For Each matchedChild As HtmlNode In DirectCast(node, HtmlElement).Nodes.GetElementsByAttributeNameValue(AttributeName, AttributeValue, searchChildren)
                            results.Add(matchedChild)
                        Next
                    End If
                End If
            Next
            Return results
        End Function

        ''' <summary>
        ''' This will search though this collection of nodes for all elements with matchs the predicate.
        ''' </summary>
        ''' <typeparam name="type"></typeparam>
        ''' <param name="predicate"></param>
        ''' <param name="searchChildren"></param>
        ''' <returns></returns>
        Public Function FindElements(Of type As HtmlNode)(predicate As Func(Of type, Boolean), Optional SearchChildren As Boolean = True) As HtmlNodeCollection
            Dim results As New HtmlNodeCollection(Nothing)
            For Each node As HtmlNode In Me
                If TypeOf node Is type Then
                    Dim l As New List(Of type)
                    For Each e In Me.Where(Function(t) TypeOf t Is type)
                        l.Add(e)
                    Next
                    If l.Where(predicate).Contains(node) Then
                        If Not results.Contains(node) Then
                            results.Add(node)
                        End If
                        l.Clear()
                    End If
                End If
                If TypeOf node Is HtmlElement Then
                    If SearchChildren Then
                        For Each matchedChild As HtmlNode In DirectCast(node, HtmlElement).Nodes.FindElements(predicate, SearchChildren)
                            If Not results.Contains(matchedChild) Then
                                results.Add(matchedChild)
                            End If
                        Next
                    End If
                End If

            Next
            Return results
        End Function

        ''' <summary>
        ''' Return elements thats match the current CSS selector
        ''' </summary>
        ''' <param name="CssSelector">CSS selector</param>
        ''' <returns></returns>
        Default Public Overloads ReadOnly Property Item(CssSelector As String) As HtmlNodeCollection
            Get
                Dim l As New HtmlNodeCollection

                If CssSelector Is Nothing OrElse CssSelector.IsBlank Then
                    Return Me
                End If

                If CssSelector.Contains(","c) Then
                    Dim combinedSelectors = CssSelector.Split(","c)
                    Dim rt = Me.Item(combinedSelectors(0))
                    For Each s In combinedSelectors.Skip(1)
                        For Each n In Me.Item(s)
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
                        l = Traverse(Me)

                    End If

                    l = selector.Filter(l)

                    allowTraverse = selector.AllowTraverse

                Next
                Return l
            End Get
        End Property

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

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================