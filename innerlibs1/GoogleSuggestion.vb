Imports System.Xml
Imports InnerLibs
Public Module GoogleSuggestion
    ''' <summary>
    ''' Retorna uma lista de sugestões de pesquisa do google baseado em um texto
    ''' </summary>
    ''' <param name="Text">Texto da pesquisa</param>
    ''' <param name="Language">Sigla do Idioma</param>
    ''' <returns></returns>
    <Runtime.CompilerServices.Extension()>
    Public Function GetGoogleSuggestions(Text As String, Optional Language As String = "pt") As List(Of String)
        GetGoogleSuggestions = New List(Of String)
        Try
            If Text.Length > 1 Then
                Dim xml As XmlDocument = (AJAX.Request(Of XmlDocument)("http://suggestqueries.google.com/complete/search?q=" & Text & "&client=toolbar&hl=" & Language))
                For Each n As XmlNode In xml.SelectNodes("//toplevel/CompleteSuggestion/suggestion")
                    GetGoogleSuggestions.Add(n.Attributes("data").Value)
                Next
            End If
        Catch ex As Exception
        End Try
    End Function
End Module
