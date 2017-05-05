Imports System.Windows.Forms
Imports System.Xml

''' <summary>
''' Estrutura de uma TAG HTML
''' </summary>
Public Class HtmlTag

    Friend stringoriginal As String
    Friend selfclosing As Boolean
    Friend htmlcontent As String

    ''' <summary>
    ''' String que originou essa classe
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property OriginalTagString As String
        Get
            Return stringoriginal
        End Get
    End Property

    ''' <summary>
    ''' Indica se a tag atual é uma tag sem conteúdo
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property IsSelfClosingTag As Boolean
        Get
            Return selfclosing
        End Get
    End Property

    ''' <summary>
    ''' Retorna o valor de um atributo da tag
    ''' </summary>
    ''' <param name="Key">nome do atributo</param>
    ''' <returns></returns>
    Default Property Attribute(Key As String) As String
        Get
            If Key.IsNotBlank AndAlso Attributes.ContainsKey(Key) Then
                Return Attributes(Key)
            Else
                Return ""
            End If
        End Get
        Set(value As String)
            If Key.IsNotBlank AndAlso Attributes.ContainsKey(Key) Then
                Attributes.Item(Key) = value
            Else
                If Key.IsNotBlank Then Attributes.Add(Key, value)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Nome da Tag
    ''' </summary>
    ''' <returns></returns>
    '''
    Public Property TagName As String
    ''' <summary>
    ''' Atributos da Tag
    ''' </summary>
    ''' <returns></returns>
    Public Property Attributes As New Dictionary(Of String, String)

    ''' <summary>
    ''' Conteudo da Tag
    ''' </summary>
    ''' <returns></returns>
    Public Property InnerHtml As String
        Get
            Return htmlcontent
        End Get
        Set(value As String)
            If value.IsNotBlank Then
                selfclosing = False
                htmlcontent = value
            End If
        End Set
    End Property

    ''' <summary>
    ''' Conteudo da Tag. É um alias para <see cref="InnerHtml"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property Content As String
        Get
            Return InnerHtml
        End Get
        Set(value As String)
            InnerHtml = value
        End Set
    End Property

    ''' <summary>
    ''' Retorna a string da tag
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property OuterHtml As String
        Get
            Return ToString()
        End Get
    End Property

    ''' <summary>
    ''' Retorna a string da tag
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function ToString() As String
        Dim attribs = ""
        For Each a In Attributes
            If a.Value.IsBlank Then
                attribs.Append(" " & a.Key)
            Else
                attribs.Append(" " & a.Key & "=" & a.Value.Replace("'", "\'").Replace("""", "\""").Quote)
            End If
        Next
        If IsSelfClosingTag Then
            Return "<" & TagName & attribs & "/>"
        Else
            Return "<" & TagName & attribs & ">" & InnerHtml & "</" & TagName & ">"
        End If
    End Function

    ''' <summary>
    ''' Subistitui a Tag String original em um texto pela nova
    ''' </summary>
    ''' <param name="HtmlText">Texto HTML com a string original</param>
    Public Sub ReplaceIn(ByRef HtmlText As String)
        If stringoriginal.IsNotBlank Then
            HtmlText = HtmlText.Replace(stringoriginal, Me.ToString)
        End If
    End Sub

    ''' <summary>
    ''' Cria uma HtmlTagInfo a partir de uma String
    ''' </summary>
    ''' <param name="TagString">String contendo a tag</param>
    Public Sub New(Optional TagString As String = "")
        If TagString.IsNotBlank Then
            stringoriginal = TagString
            Me.TagName = TagString.AdjustWhiteSpaces.GetBefore(" ").RemoveFirstIf("<")
            Dim t As HtmlTag = TagString.GetElementsByTagName(Me.TagName).FirstOrDefault
            Me.selfclosing = t.IsSelfClosingTag
            Me.Attributes = t.Attributes
            Me.InnerHtml = t.InnerHtml
        End If
    End Sub
End Class