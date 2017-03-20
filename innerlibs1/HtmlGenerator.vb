''' <summary>
''' Cria componentes de HTML dinamicos
''' </summary>
Public Module HtmlGenerator

    Public Function Textarea(Name As String, IdName As String, Optional [Class] As String = "", Optional Content As String = "", Optional OtherAttr As String = "", Optional AllowLabel As Boolean = True) As String
        Dim asterisco As String = ""
        For Each item In [Class].Split(Convert.ToChar(" "))
            If item = "obg" Or item = "req" Then
                asterisco = "*"
            End If
        Next


        Dim thelabel As String = If(AllowLabel, "<p>" + Name + asterisco + "<p>", "")

        Return thelabel + "<textarea  id=""" + IdName + """ class=""" + [Class] + """ name=""" + IdName + """  " & OtherAttr & ">" + Content + "</textarea>"
    End Function

    Public Function Input(Name As String, IdName As String, Optional [Class] As String = "", Optional Type As String = "text", Optional Value As String = "", Optional OtherAttr As String = "", Optional AllowLabel As Boolean = True) As String
        Dim asterisco As String = ""
        For Each item In [Class].Split(Convert.ToChar(" "))
            If item = "obg" Or item = "req" Then
                asterisco = "*"
            End If
        Next


        Dim thelabel As String = If(AllowLabel, "<p>" + Name + asterisco + "<p>", "")
        If Type = "hidden" Then
            thelabel = ""
            [Class] = ""
        End If



        Return thelabel + "<input type=""" + Type + """ id=""" + IdName + """ class=""" + [Class] + """ name=""" + IdName + """ value=""" + Value + """  " & OtherAttr & "/>"
    End Function

    Public Function Options(Names As String(), Values As String(), Optional [Default] As String = "") As String
        Dim opt As String = ""
        Dim i As Integer = 0
        While i < Names.Length
            If Not String.IsNullOrWhiteSpace(Names(i)) Then
                Values(i) = If((String.IsNullOrWhiteSpace(Values(i))), Names(i), Values(i))
                If [Default] = Values(i) Then
                    opt += "<option value='" + Values(i) + "' selected='selected'>" + Names(i) + "</option>"
                Else
                    opt += "<option value='" + Values(i) + "'>" + Names(i) + "</option>"

                End If
            End If
            System.Math.Max(System.Threading.Interlocked.Increment(i), i - 1)
        End While

        Return opt
    End Function


    Public Function SelectBox(Name As String, IdName As String, OptionList As String, Optional [Class] As String = "", Optional DefaultItemName As String = "Selecione...", Optional DefaultValue As String = "", Optional OtherAttr As String = "", Optional AllowLabel As Boolean = True) As String

        Dim asterisco As String = ""
        For Each item In [Class].Split(Convert.ToChar(" "))
            If item = "obg" Or item = "req" Then
                asterisco = "*"
            End If
        Next
        Dim thelabel As String = If(AllowLabel, "<p>" + Name + asterisco + "<p>", "")
        Return thelabel + "<select  " & OtherAttr & " id=""" + IdName + """ class=""" + [Class] + """ name=""" + IdName + """ >" + "<option value='" + DefaultValue + "'>" + DefaultItemName + "</option>" + OptionList + "</select>"
    End Function

    ''' <summary>
    ''' Cria uma linha de tabela html com diversas colunas (td)
    ''' </summary>
    ''' <param name="Tds">strings contendo o conteudo de cada TD</param>
    ''' <returns>Uma TR</returns>
    Public Function TableRow(ID As String, ParamArray Tds() As String) As String
        Dim retorno As String = "<tr id='" & ID & "'>"
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
    ''' <param name="Rows">Linhas da tabela</param>
    ''' <param name="ID">id da tabela</param>
    ''' <param name="[Class]">atributo class da tabela</param>
    ''' <returns>uma strig com a table</returns>
    Public Function Table(TableHeader As String, Rows As String, Optional ID As String = "", Optional [Class] As String = "") As String
        Dim retorno As String = "<table class='" & [Class] & "' id='" & ID & "'>"
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