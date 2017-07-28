Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Xml

''' <summary>
''' Estrutura de uma TAG HTML
''' </summary>
Public Class HtmlTag

    Friend stringoriginal As String = ""
    Friend selfclosing As Boolean = False
    Friend htmlcontent As String = ""

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
            Select Case True
                Case Key.ToLower = "tagname"
                    Return TagName & ""
                Case Key.ToLower = "innerhtml"
                    Return InnerHtml & ""
                Case Key.ToLower = "innertext"
                    Return InnerText & ""
                Case Key.ToLower = "class"
                    Return Me.Class.Join(" ")
                Case Key.ToLower = "id"
                    Return Me.ID & ""
                Case Key.ToLower = "style"
                    Dim r As String = ""
                    For Each item In Style
                        r.Append(item.Key & ":" & item.Value & ";")
                    Next
                    Return r & ""
                Case Key.IsNotBlank AndAlso Attributes.ContainsKey(Key)
                    Return Attributes(Key) & ""
                Case Else
                    Return ""
            End Select
        End Get
        Set(value As String)
            Select Case True
                Case Key.ToLower = "id"
                    Me.ID = value
                Case Key.ToLower = "tagname"
                    TagName = value.ToLower
                Case Key.ToLower = "innerhtml"
                    InnerHtml = value
                Case Key.ToLower = "innertext"
                    InnerText = value
                Case Key.ToLower = "class"
                    Me.Class = value.Split(" ").ToList
                Case Key.ToLower = "style"
                    Dim d As New Dictionary(Of String, String)
                    Try
                        For Each prop In value.Split(";")
                            d.Add(prop.Split(":")(0), prop.Split(":")(1))
                        Next
                    Catch ex As Exception
                    End Try
                    Style = New SortedDictionary(Of String, String)(d)
                Case Key.IsNotBlank AndAlso Attributes.ContainsKey(Key)
                    Attributes.Item(Key) = value
                Case Else
                    If Key.IsNotBlank Then Attributes.Add(Key, "" & value)
            End Select
        End Set
    End Property

    ''' <summary>
    ''' Atributo Class da tag HTML
    ''' </summary>
    ''' <returns></returns>
    Property [Class] As New List(Of String)

    ''' <summary>
    ''' Estilos CSS da tag
    ''' </summary>
    ''' <returns></returns>
    Property Style As New SortedDictionary(Of String, String)

    ''' <summary>
    ''' Retorna elementos desta tag por nome da tag
    ''' </summary>
    ''' <param name="TagName"></param>
    ''' <returns></returns>
    Function GetElementsByTagName(ParamArray TagName As String()) As List(Of HtmlTag)
        Return InnerHtml.GetElementsByTagName(TagName)
    End Function

    ''' <summary>
    ''' Nome da Tag
    ''' </summary>
    ''' <returns></returns>
    '''
    Public Property TagName As String

    ''' <summary>
    ''' ID da tag
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String

    ''' <summary>
    ''' Atributos da Tag
    ''' </summary>
    ''' <returns></returns>
    Public Property Attributes As SortedDictionary(Of String, String)
        Get
            Return cs
        End Get
        Set(value As SortedDictionary(Of String, String))
            cs = New SortedDictionary(Of String, String)
            For Each k In value.Keys
                Me.Attribute(k) = value(k)
            Next
        End Set
    End Property

    Private cs As New SortedDictionary(Of String, String)

    ''' <summary>
    ''' Retorna todas as Keys dos atributos incluindo id, class e style se estes possuirem algum valor
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AttributesKeys As List(Of String)
        Get
            Dim l As New List(Of String)(Attributes.Keys)
            If ID.IsNotBlank Then l.Add("id")
            If Style.Count > 0 Then l.Add("style")
            If Me.Class.Count > 0 Then l.Add("class")
            Return l
        End Get
    End Property

    ''' <summary>
    ''' String que representa os atributos da tag incluindo class, id e style se estes possuirem algum valor
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property AttributesString As String
        Get
            Dim attribs = ""
            If [Class].Count > 0 Then
                attribs.Append(" class=" & Attribute("class").Quote)
            End If

            If Style.Count > 0 Then
                attribs.Append(" style=" & Attribute("style").Quote)
            End If

            If ID.IsNotBlank Then
                attribs.Append(" id=" & Attribute("id").Quote)
            End If

            For Each a In Attributes
                If a.Key.IsNotBlank And Not a.Key.IsIn({"innerhtml", "innertext", "id", "style", "class"}) Then
                    If a.Value.IsBlank Then
                        attribs.Append(" " & a.Key)
                    Else
                        attribs.Append(" " & a.Key & "=" & a.Value.Replace("'", "\'").Replace("""", "\""").Quote)
                    End If
                End If
            Next

            Return attribs.Trim
        End Get
    End Property

    ''' <summary>
    ''' COnteudo da Tag sem HTML
    ''' </summary>
    ''' <returns></returns>
    Public Property InnerText As String
        Set(value As String)
            Me.InnerHtml = value.HtmlEncode
        End Set
        Get
            Return InnerHtml.RemoveHTML
        End Get
    End Property

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
    ''' Retorna a string da tag. É um alias para <see cref="ToString()"/>
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

        If IsSelfClosingTag Then
            Return "<" & TagName & " " & AttributesString & "/>"
        Else
            Return "<" & TagName & " " & AttributesString & ">" & InnerHtml & "</" & TagName & ">"
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
    ''' Remove a Tag String original em um texto
    ''' </summary>
    ''' <param name="HtmlText">Texto HTML com a string original</param>
    Public Sub RemoveIn(ByRef HtmlText As String)
        If stringoriginal.IsNotBlank Then
            HtmlText = HtmlText.Replace(stringoriginal, "")
        End If
    End Sub

    ''' <summary>
    ''' Cria uma HtmlTag a partir de uma String
    ''' </summary>
    ''' <param name="TagString">String contendo a tag</param>
    Public Sub New(Optional TagString As String = "")
        If TagString.IsNotBlank Then
            Me.stringoriginal = TagString
            Me.TagName = TagString.Trim.GetBetween("<", ">").GetBefore(" ")
            Dim t As HtmlTag = TagString.GetElementsByTagName(Me.TagName).FirstOrDefault
            Me.TagName = t.TagName
            Me.selfclosing = t.IsSelfClosingTag
            Me.Attributes = t.Attributes
            Me.Class = t.Class
            Me.Style = t.Style
            Me.ID = t.ID
            Me.InnerHtml = t.InnerHtml
        End If
    End Sub

    Public Sub New(Tag As XElement)
        Me.New(Tag.ToString)
    End Sub

End Class