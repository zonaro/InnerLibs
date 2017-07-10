Imports System.Text.RegularExpressions

Namespace Templatizer

    ''' <summary>
    ''' Extrai conteudo especifico de paginas HTML usando as instruçoes do <see cref="Rule"/>
    ''' </summary>
    Public Class HtmlContentExtractor
        Inherits Dictionary(Of String, String())

        Public Class Rule

            ''' <summary>
            ''' Tag que será extraida do HTML. É permitido o uso de <see cref="Regex"/>
            ''' </summary>
            ''' <returns></returns>
            ReadOnly Property Tag As HtmlTag

            ''' <summary>
            ''' Nome da propriedade no qual o resultdado extraído será guardado
            ''' </summary>
            ''' <returns></returns>
            ReadOnly Property Key As String

            ''' <summary>
            ''' Instruçao que indica se todos os atirbutos deve ser idênticos para o conteúdo ser extraído.
            ''' </summary>
            ''' <returns></returns>
            ReadOnly Property MatchAllAttributtes As Boolean

            ''' <summary>
            ''' Key do atributo que será utilizado para extrair o conteúdo
            ''' </summary>
            ''' <returns></returns>
            ReadOnly Property ContentAttribute As String = "InnerText"

            ''' <summary>
            ''' Cria uma nova instância de regra
            ''' </summary>
            ''' <param name="Key">Nome da propriedade no qual o resultdado extraído será guardado</param>
            ''' <param name="HtmlString">Tag que será extraida do HTML. É permitido o uso de <see cref="Regex"/></param>
            ''' <param name="MatchAllAttributtes">Instruçao que indica se todos os atirbutos deve ser idênticos para o conteúdo ser extraído.</param>
            ''' <param name="ContentAttribute">Key do atributo que será utilizado para extrair o conteúdo.</param>
            Sub New(Key As String, HtmlString As String, MatchAllAttributtes As Boolean, Optional ContentAttribute As String = "InnerText")
                Me.Key = Key
                Me.Tag = New HtmlTag(HtmlString.Replace("{%}", "^(?!\s*$).+"))
                Me.MatchAllAttributtes = MatchAllAttributtes
                Me.ContentAttribute = ContentAttribute
            End Sub

        End Class

        ''' <summary>
        ''' Declara uma instancia de HtmlContentExtractor
        ''' </summary>
        ''' <param name="HTML">Conteído HTML</param>
        ''' <param name="Rules">Regras para Extração</param>
        Sub New(HTML As String, ParamArray Rules As Rule())
            For Each cm In Rules.ToList.Distinct
                Dim content As New List(Of String)
                For Each element As HtmlTag In HTML.GetElementsByTagName(cm.Tag.TagName)
                    Dim MatchList As New List(Of Boolean)

                    If cm.MatchAllAttributtes Then
                        If cm.Tag.AttributesKeys.ContainsAll(element.AttributesKeys) Then
                            For Each attr In element.AttributesKeys
                                MatchList.Add(New Regex(cm.Tag.Attribute(attr), RegexOptions.IgnoreCase + RegexOptions.Singleline).IsMatch(element.Attribute(attr)) And If(element.Attribute(attr).IsBlank, cm.Tag.Attribute(attr).IsBlank, False))
                            Next
                        End If
                    Else
                        If cm.Tag.AttributesKeys.ContainsAny(element.AttributesKeys.ToList) Then
                            For Each attr In element.AttributesKeys
                                MatchList.Add(New Regex(cm.Tag.Attribute(attr), RegexOptions.IgnoreCase + RegexOptions.Singleline).IsMatch(element.Attribute(attr)) And element.AttributesKeys.Contains(attr))
                            Next
                        End If

                    End If
                    If Not MatchList.Contains(False) Then
                        content.Add(element.Attribute(cm.ContentAttribute))
                    End If
                Next
                If Me.ContainsKey(cm.Key) Then
                    Dim l = Me.Item(cm.Key).ToList
                    l.AddRange(content.ToArray)
                    Me.Item(cm.Key) = l.ToArray
                Else
                    Me.Add(cm.Key, content.ToArray)
                End If
            Next
        End Sub

        ''' <summary>
        ''' Declara uma instancia de HtmlContentExtractor
        ''' </summary>
        ''' <param name="URL">URL de requisição</param>
        ''' <param name="Rules">Regras para Extração</param>
        Sub New(URL As Uri, ParamArray Rules As Rule())
            Me.New(AJAX.[GET](Of String)(URL.AbsoluteUri), Rules)
        End Sub

    End Class

End Namespace