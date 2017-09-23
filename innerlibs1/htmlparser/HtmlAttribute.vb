
Imports System.Collections
Imports System.ComponentModel

Namespace HtmlParser
    ''' <summary>
    ''' The HtmlAttribute object represents a named value associated with an HtmlElement.
    ''' </summary>

    Public Class HtmlAttribute
        Protected mName As String
        Protected mValue As String

        Public Sub New()
            mName = "Unnamed"

            mValue = ""

        End Sub

        ''' <summary>
        ''' This constructs an HtmlAttribute object with the given name and value. For wierd
        ''' HTML attributes that don't have a value (e.g. "NOWRAP"), specify null as the value.
        ''' </summary>
        ''' <param name="name">The name of the attribute</param>
        ''' <param name="value">The value of the attribute</param>
        Public Sub New(name As String, value As String)

            mName = name
            mValue = value

        End Sub

        ''' <summary>
        ''' The name of the attribute. e.g. WIDTH
        ''' </summary>
        <Category("General"), Description("The name of the attribute")>
        Public Property Name() As String

            Get
                Return mName
            End Get
            Set

                mName = Value
            End Set
        End Property

        ''' <summary>
        ''' The value of the attribute. e.g. 100%
        ''' </summary>
        <Category("General"), Description("The value of the attribute")>
        Public Property Value() As String

            Get
                Return mValue
            End Get
            Set

                mValue = Value
            End Set
        End Property

        ''' <summary>
        ''' This will return an HTML-formatted version of this attribute. NB. This is
        ''' not SGML or XHTML safe, as it caters for null-value attributes such as "NOWRAP".
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String

            If mValue Is Nothing Then
                Return mName
            Else

                Return (Convert.ToString(mName & Convert.ToString("=""")) & mValue) + """"
            End If

        End Function

        <Category("Output"), Description("The HTML to represent this attribute")>
        Public ReadOnly Property HTML() As String

            Get
                If mValue Is Nothing Then

                    Return mName

                Else
                    Return (mName & Convert.ToString("=""")) + HtmlEncoder.EncodeValue(mValue) + """"
                End If

            End Get

        End Property

        <Category("Output"), Description("The XHTML to represent this attribute")>
        Public ReadOnly Property XHTML() As String

            Get
                If mValue Is Nothing Then

                    Return mName.ToLower()

                Else
                    Return (mName & Convert.ToString("=""")) + HtmlEncoder.EncodeValue(mValue.ToLower()) + """"
                End If

            End Get

        End Property

    End Class

    ''' <summary>
    ''' This is a collection of attributes. Typically, this is associated with a particular
    ''' element. This collection is searchable by both the index and the name of the attribute.
    ''' </summary>
    Public Class HtmlAttributeCollection
        Inherits CollectionBase
        Private mElement As HtmlElement

        Public Sub New()

            mElement = Nothing
        End Sub

        ''' <summary>
        ''' This will create an empty collection of attributes.
        ''' </summary>
        ''' <param name="element"></param>
        Friend Sub New(element As HtmlElement)

            mElement = element
        End Sub

        ''' <summary>
        ''' This will add an element to the collection.
        ''' </summary>
        ''' <param name="attribute">The attribute to add.</param>
        ''' <returns>The index at which it was added.</returns>
        Public Function Add(attribute As HtmlAttribute) As Integer

            Return MyBase.List.Add(attribute)

        End Function

        ''' <summary>
        ''' This provides direct access to an attribute in the collection by its index.
        ''' </summary>
        Default Public Property Item(index As Integer) As HtmlAttribute

            Get
                Return DirectCast(MyBase.List(index), HtmlAttribute)

            End Get

            Set
                MyBase.List(index) = Value
            End Set
        End Property

        ''' <summary>
        ''' This will search the collection for the named attribute. If it is not found, this
        ''' will return null.
        ''' </summary>
        ''' <param name="name">The name of the attribute to find.</param>
        ''' <returns>The attribute, or null if it wasn't found.</returns>

        Public Function FindByName(name As String) As HtmlAttribute

            Dim index As Integer = IndexOf(name)

            If index = -1 Then
                Return Nothing
            Else

                Return Me(IndexOf(name))

            End If

        End Function

        ''' <summary>
        ''' This will return the index of the attribute with the specified name. If it is
        ''' not found, this method will return -1.
        ''' </summary>
        ''' <param name="name">The name of the attribute to find.</param>
        ''' <returns>The zero-based index, or -1.</returns>
        Public Function IndexOf(name As String) As Integer

            For index As Integer = 0 To Me.List.Count - 1
                If Me(index).Name.ToLower().Equals(name.ToLower()) Then
                    Return index
                End If
            Next

            Return -1
        End Function

        ''' <summary>
        ''' This overload allows you to have direct access to an attribute by providing
        ''' its name. If the attribute does not exist, null is returned.
        ''' </summary>
        Default Public ReadOnly Property Item(name As String) As HtmlAttribute

            Get
                Return FindByName(name)

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
