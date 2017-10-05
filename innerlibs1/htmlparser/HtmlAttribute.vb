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
        Public Sub New(name As String, Optional value As String = Nothing)
            mName = name.ToLower
            mValue = value
        End Sub

        ''' <summary>
        ''' The name of the attribute. e.g. WIDTH
        ''' </summary>
        <Category("General"), Description("The name of the attribute")>
        Public Property Name() As String

            Get
                Return mName.ToLower
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
                    Return (mName & Convert.ToString("=""")) & System.Net.WebUtility.HtmlEncode(mValue) & """"
                End If

            End Get

        End Property

        <Category("Output"), Description("The XHTML to represent this attribute")>
        Public ReadOnly Property XHTML() As String

            Get
                If mValue Is Nothing Then

                    Return mName.ToLower()
                Else
                    Return (mName & Convert.ToString("=""")) & System.Net.WebUtility.HtmlEncode(mValue) & """"
                End If

            End Get

        End Property

    End Class

    ''' <summary>
    ''' This is a collection of attributes. Typically, this is associated with a particular
    ''' element. This collection is searchable by both the index and the name of the attribute.
    ''' </summary>
    Public Class HtmlAttributeCollection
        Inherits List(Of HtmlAttribute)
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
        ''' This will search the collection for the named attribute. If it is not found, this
        ''' will return null.
        ''' </summary>
        ''' <param name="name">The name of the attribute to find.</param>
        ''' <returns>The attribute, or null if it wasn't found.</returns>

        Public Function FindByName(Name As String) As HtmlAttribute
            Dim l = Me.Where(Function(a) a.Name.ToLower = Name.ToLower)
            If l.Count > 0 Then
                Return l.First
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' This overload allows you to have direct access to an attribute by providing
        ''' its name. If the attribute does not exist, null is returned.
        ''' </summary>
        Default Public Overloads ReadOnly Property Item(Name As String) As HtmlAttribute

            Get
                Return FindByName(Name.ToLower)
            End Get

        End Property

        ''' <summary>
        ''' Add an attribute to element
        ''' </summary>
        ''' <param name="Name">Name of attribute</param>
        ''' <param name="Value">Value of attribute, fro Wired attributes just leave Nothing</param>
        Public Overloads Sub Add(Name As String, Optional Value As String = Nothing)
            If Me.Contains(Name) Then
                Me.Remove(Name)
            End If
            Me.Add(New HtmlAttribute(Name, Value))
        End Sub

        Public Overloads Function Contains(Name As String) As Boolean
            If Me.Count = 0 Then Return False
            Return Me.Where(Function(p) p.Name.ToLower = Name.ToLower).Count > 0
        End Function

        Public Overloads Sub Remove(Name As String)
            For Each attr In Me
                If attr.Name.ToLower = Name.ToLower Then
                    Me.Remove(attr)
                End If
            Next
        End Sub

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================