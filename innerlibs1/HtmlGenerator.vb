''' <summary>
''' Assistente de criação de tabelas HTML
''' </summary>
Public Module TableGenerator

    ''' <summary>
    ''' Cria uma linha de tabela html com diversas colunas (td)
    ''' </summary>
    ''' <param name="Tds">strings contendo o conteudo de cada TD</param>
    ''' <returns>Uma TR</returns>
    Public Function TableRow(ID As String, ParamArray Tds() As String) As String
        Dim retorno As String = "<tr id=" & ID.Quote & ">"
        For Each Td In Tds
            retorno.Append("<td>" & Td & "</td>")
        Next
        retorno.Append("</tr>")
        Return retorno
    End Function

    ''' <summary>
    ''' Cria um Table Header (thead) com as colunas especificadas
    ''' </summary>
    ''' <param name="Ths">Colunas</param>
    ''' <returns>String thead</returns>
    Public Function TableHeader(ParamArray Ths() As String) As String
        Dim retorno As String = "<thead><tr>"
        For Each th In Ths
            retorno.Append("<th>" & th & "</th>")
        Next
        retorno.Append("</tr></thead>")
        Return retorno
    End Function

    ''' <summary>
    ''' Cria uma Table HTML a partir de strings geradas
    ''' </summary>
    ''' <param name="TableHeader">Elemento thead com o cabeçalho</param>
    ''' <param name="Rows">       Linhas da tabela</param>
    ''' <param name="ID">         id da tabela</param>
    ''' <param name="[Class]">    atributo class da tabela</param>
    ''' <returns>uma strig com a table</returns>
    Public Function Table(TableHeader As String, Rows As String, Optional ID As String = "", Optional [Class] As String = "") As String
        Dim retorno As String = "<table class=" & [Class].Quote & " id=" & ID.Quote & ">"
        If TableHeader.IsNotBlank Then
            retorno.Append(TableHeader)
        End If
        If Rows.IsNotBlank Then
            retorno.Append("<tbody>" & Rows & "</tbody>")
        End If
        retorno.Append("</table>")
        Return retorno
    End Function

End Module