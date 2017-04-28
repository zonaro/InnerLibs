Imports System.Windows.Forms
Imports System.Xml

''' <summary>
''' Estrutura de uma TAG HTML
''' </summary>
Public Class HtmlTag

    Friend stringoriginal As String

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
        Return "<" & TagName & attribs & ">" & InnerHtml & "</" & TagName & ">"
    End Function

    Friend Sub FixIn(ByRef HtmlText As String)
        If stringoriginal.IsNotBlank Then
            HtmlText = HtmlText.Replace(stringoriginal, Me.ToString)
        End If
    End Sub

    ''' <summary>
    ''' Retorna um XML Document a partir desta HTML Tag
    ''' </summary>
    ''' <returns></returns>
    Public Function ToXML() As XmlDocument
        Dim xml As New XmlDocument
        xml.LoadXml(Me.ToString)
        Return xml
    End Function

    ''' <summary>
    ''' Cria uma HtmlTagInfo a partir de uma String
    ''' </summary>
    ''' <param name="TagString">String contendo a tag</param>
    Public Sub New(Optional TagString As String = "")
        If TagString.IsNotBlank Then
            stringoriginal = TagString
            Me.TagName = TagString.AdjustWhiteSpaces.GetBefore(" ").RemoveFirstIf("<")
            Dim t As HtmlTag = TagString.GetElementsByTagName(Me.TagName).FirstOrDefault
            Me.Attributes = t.Attributes
            Me.InnerHtml = t.InnerHtml
        End If
    End Sub
End Class