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
        Public Sub New(Text As String)
            mText = Text
        End Sub

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
        ''' This will return the XHTML to represent this text object.
        ''' </summary>
        Public Overrides ReadOnly Property XHTML() As String
            Get
                Return System.Net.WebUtility.HtmlEncode(Text)
            End Get
        End Property

        Friend ReadOnly Property NoEscaping() As Boolean
            Get
                If mParent Is Nothing Then
                    Return False
                Else
                    Return mParent.NoEscaping
                End If
            End Get
        End Property

        ''' <summary>
        ''' ReplaceFrom Badwords in text.
        ''' </summary>
        ''' <param name="CensorChar"></param>
        ''' <param name="BadWords">  </param>
        ''' <returns></returns>
        Public Overrides Function Censor(CensorChar As Char, ParamArray BadWords As String()) As Boolean
            Dim cs = Me.Text.Censor(CensorChar, BadWords)
            If cs <> Me.Text Then
                Me.Text = cs
                Return True
            End If
            Return False
        End Function

        ''' <summary>
        ''' Fix the punctuation, white spaces and captalization of text
        ''' </summary>
        Public Overrides Sub FixText()
            Me.Text = Me.Text.FixText
        End Sub

        ''' <summary>
        ''' This will return the text for outputting inside an HTML document.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Text
        End Function

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================