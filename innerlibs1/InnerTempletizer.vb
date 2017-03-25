
Imports System.Data.Common
Imports System.IO


Namespace Templatizer

    ''' <summary>
    ''' Gera HTML dinâmico a partir de uma conexão com banco de dados e um template HTML
    ''' </summary>
    Public NotInheritable Class TemplateBuilder

        ''' <summary>
        ''' Instancia um Novo TemplateBuilder
        ''' </summary>
        ''' <param name="DataBase">Conexão com o banco de dados</param>
        ''' <param name="TemplateFolder">Pasta com os arquivos HTML dos templates</param>
        Sub New(DataBase As DataBase, TemplateFolder As DirectoryInfo)
            Me.TemplateFolder = TemplateFolder
            Me.DataBase = DataBase
        End Sub

        ''' <summary>
        ''' Conexão genérica de Banco de Dados
        ''' </summary>
        ''' <returns></returns>
        Property DataBase As DataBase

        ''' <summary>
        ''' Pasta contendo os arquivos HTML utilizados como template
        ''' </summary>
        ''' <returns></returns>
        Property TemplateFolder As DirectoryInfo

        ''' <summary>
        ''' Processa os comandos SQL e retorna o resultado em HTML utilizando o template especificado
        ''' </summary>
        ''' <param name="SQLQuery">Comando SQL</param>
        ''' <param name="TemplateFile">Arquivo do Template</param>
        ''' <returns></returns>
        Public Function Build(SQLQuery As String, TemplateFile As String) As String
            Dim response As String = ""
            Dim template As String = ""
            Dim header As String = ""
            Dim filefound = TemplateFolder.Search(SearchOption.TopDirectoryOnly, TemplateFile).First
            If Not filefound.Exists Then Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & TemplateFolder.FullName)
            Using file As StreamReader = filefound.OpenText
                template = file.ReadToEnd.RemoveNonPrintable.AdjustWhiteSpaces
                Try
                    header = template.GetElementsByTagName("head").First.Content
                Catch ex As Exception
                End Try
                Try
                    template = template.GetElementsByTagName("body").First.Content
                Catch ex As Exception
                End Try
            End Using

            Using reader As DbDataReader = DataBase.RunSQL(SQLQuery)
                While reader.Read
                    Dim copia As String = template
                    'replace nas strings padrão
                    For Each col In reader.GetColumns
                        copia = copia.Replace("##" & col & "##", reader(col).ToString())
                    Next

                    'replace nas procedures
                    For Each sqlTag As HtmlTag In copia.GetElementsByTagName("sqlquery")
                        sqlTag.FixIn(copia)
                        Dim tp = Build(sqlTag.Content, sqlTag.Attributes.Item("data-templatefile"))
                        copia = copia.Replace(sqlTag.ToString, tp)
                    Next
                    response.Append(copia)
                End While
            End Using
            response.Append(header)
            Return response
        End Function

    End Class
End Namespace
