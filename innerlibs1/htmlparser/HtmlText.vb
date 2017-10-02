
Imports System.ComponentModel

Namespace HtmlParser
    ''' <summary>
    ''' The HtmlText node represents a simple piece of text from the document.
    ''' </summary>
    Public Class HtmlText
        Inherits HtmlNode
        Protected mText As String

        ''' <summary>
        ''' This constructs a new node with the given text content.
        ''' </summary>
        ''' <param name="text"></param>
        Public Sub New(text As String)
            mText = text
        End Sub

        ''' <summary>
        ''' This is the text associated with this node.
        ''' </summary>
        <Category("General"), Description("The text located in this text node")>
        Public Property Text() As String
            Get
                Return mText
            End Get
            Set
                mText = Value
            End Set
        End Property

        ''' <summary>
        ''' This will return the text for outputting inside an HTML document.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Text
        End Function


        ''' <summary>
        ''' This will return the text for outputting inside an HTML document.
        ''' </summary>
        ''' <returns></returns>
        <Category("General"), Description("The text associated with this element")>
        Public Overrides ReadOnly Property ElementRepresentation As String
            Get
                Return ToString()
            End Get
        End Property


        Friend ReadOnly Property NoEscaping() As Boolean
            Get
                If mParent Is Nothing Then
                    Return False
                Else
                    Return DirectCast(mParent, HtmlElement).NoEscaping
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return the HTML to represent this text object.
        ''' </summary>
        Public Overrides ReadOnly Property HTML() As String
            Get
                If NoEscaping Then
                    Return Text
                Else
                    Return System.Net.WebUtility.HtmlEncode(Text)
                End If
            End Get
        End Property

        ''' <summary>
        ''' This will return the XHTML to represent this text object.
        ''' </summary>
        Public Overrides ReadOnly Property XHTML() As String
            Get
                Return System.Net.WebUtility.HtmlEncode(Text)
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
