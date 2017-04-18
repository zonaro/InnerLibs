Imports System.IO


Namespace Templatizer

    ''' <summary>
    ''' Gera HTML dinâmico a partir de uma conexão com banco de dados e um template HTML
    ''' </summary>
    Public NotInheritable Class TemplateBuilder


        ''' <summary>
        ''' Instancia um Novo TemplateBuilder utilizando uma Pasta para guardar os templates
        ''' </summary>
        ''' <param name="DataBase">Conexão com o banco de dados</param>
        ''' <param name="TemplateFolder">Pasta com os arquivos HTML dos templates</param>
        Sub New(DataBase As DataBase, TemplateFolder As DirectoryInfo)
            Me.TemplateFolder = TemplateFolder
            Me.DataBase = DataBase
        End Sub

        ''' <summary>
        ''' Instancia um Novo TemplateBuilder
        ''' </summary>
        ''' <param name="DataBase">Conexão com o banco de dados</param>
        ''' <param name="ApplicationAssembly">Assembly da aplicação onde os arquivos HTML dos templates estão compilados</param>
        Sub New(DataBase As DataBase, ApplicationAssembly As Reflection.Assembly)
            Me.DataBase = DataBase
            Me.ApplicationAssembly = ApplicationAssembly
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
        ReadOnly Property TemplateFolder As DirectoryInfo = Nothing

        ''' <summary>
        ''' Aplicaçao contendo os Resources (arquivos compilados internamente) dos aruqivos HTML utilziados como template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ApplicationAssembly As Reflection.Assembly = Nothing

        ''' <summary>
        ''' Processa os comandos SQL de varios templates e cria uma unica lista com todos eles
        ''' </summary>
        ''' <param name="Templates">Templates</param>
        ''' <returns></returns>
        Public Function Build(ParamArray Templates As Template()) As TemplateList
            Dim retorno As New TemplateList(Templates)
            For Each t In Templates
                retorno.AddRange(Build(t.SQLQuery, t.TemplateFile, t.Order))
            Next
            Return retorno
        End Function


        ''' <summary>
        ''' Processa o comando SQL e retorna os resultados em HTML utilizando o arquivo de template especificado
        ''' </summary>
        ''' <param name="SQLQuery">Comando SQL</param>
        ''' <param name="TemplateFile">Arquivo do Template</param>
        ''' <param name="OrderColumn">Coluna do resultado que será utilizado como ordem</param>
        ''' <returns></returns>    
        Public Function Build(SQLQuery As String, TemplateFile As String, Optional OrderColumn As String = "") As TemplateList
            Return Build(Of TemplateList)(SQLQuery, TemplateFile, OrderColumn)
        End Function


        Private Function Build(Of Type)(SQLQuery As String, TemplateFile As String, Optional OrderColumn As String = "") As Type
            Dim response As Object = Nothing

            Select Case GetType(Type)
                Case GetType(String)
                    response = ""
                Case GetType(TemplateList)
                    response = New TemplateList
            End Select


            Dim template As String = GetTemplateContent(TemplateFile)
            Dim header As String = ""

            Try
                header = template.GetElementsByTagName("head").First.Content
            Catch ex As Exception
            End Try
            Try
                template = template.GetElementsByTagName("body").First.Content
            Catch ex As Exception
            End Try

            Using reader As DataBase.Reader = DataBase.RunSQL(SQLQuery)
                Dim ordenator As Int64 = 0
                While reader.Read
                    ordenator.Increment
                    Dim copia As String = template
                    'replace nas strings padrão
                    For Each col In reader.GetColumns
                        copia = copia.Replace("##" & col & "##", reader(col).ToString())
                    Next
                    'replace nas procedures
                    For Each sqlTag As HtmlTag In copia.GetElementsByTagName("sqlquery")
                        sqlTag.FixIn(copia)
                        Dim tp As String = Build(Of String)(sqlTag.Content, sqlTag.Attributes.Item("data-templatefile"))
                        copia = copia.Replace(sqlTag.ToString, tp)
                    Next
                    Select Case GetType(Type)
                        Case GetType(String)
                            response = response & copia
                        Case GetType(TemplateList)
                            If OrderColumn.IsNotBlank AndAlso OrderColumn.IsIn(reader.GetColumns) Then
                                response.Add(New Template(TemplateFile, reader(OrderColumn), copia, SQLQuery, OrderColumn))
                            Else
                                response.Add(New Template(TemplateFile, ordenator, copia, SQLQuery, OrderColumn))
                            End If
                    End Select
                End While
            End Using
            Select Case GetType(Type)
                Case GetType(String)
                    response = response & header
                Case GetType(TemplateList)
                    response.Header = header
            End Select
            Return response
        End Function

        ''' <summary>
        ''' Retorna o conteudo estático de um arquivo de template
        ''' </summary>
        ''' <param name="Templatefile">Nome do arquivo do template</param>
        ''' <returns></returns>
        Public Function GetTemplateContent(TemplateFile As String) As String
            If IsNothing(ApplicationAssembly) Then
                Dim filefound = TemplateFolder.SearchFiles(SearchOption.TopDirectoryOnly, TemplateFile).First
                If Not filefound.Exists Then Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & TemplateFolder.Name.Quote)
                Using file As StreamReader = filefound.OpenText
                    Return file.ReadToEnd.RemoveNonPrintable.AdjustWhiteSpaces
                End Using
            Else
                Try
                    Return GetResourceFileText(ApplicationAssembly, ApplicationAssembly.GetName.Name & "." & TemplateFile).RemoveNonPrintable.AdjustWhiteSpaces
                Catch ex As Exception
                    Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & ApplicationAssembly.GetName.Name.Quote & " resources. Check if Build Action is marked as ""Embedded Resource"" in File Properties.")
                End Try
            End If
        End Function

        ''' <summary>
        ''' Traz a lista dos arquivos de template encontrados
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTemplateList() As TemplateList
            Dim l As New TemplateList
            Dim ordenator As Long = 0
            If IsNothing(ApplicationAssembly) Then
                For Each f In TemplateFolder.GetFiles
                    ordenator.Increment
                    l.Add(New Template(f.Name, ordenator, GetTemplateContent(f.Name), Nothing, "Default"))
                Next
            Else
                For Each n In ApplicationAssembly.GetManifestResourceNames()
                    ordenator.Increment
                    l.Add(New Template(n, ordenator, GetTemplateContent(n), Nothing, "Default"))
                Next
            End If
            Return l
        End Function
    End Class

    ''' <summary>
    ''' Lista de resultados gerados a partir de um ou mais templates
    ''' </summary>
    Public Class TemplateList
        Inherits List(Of Template)

        ''' <summary>
        ''' Adiciona um template á lista
        ''' </summary>
        ''' <param name="List">Lista</param>
        ''' <param name="Template">Template</param>
        ''' <returns></returns>
        Public Shared Operator +(List As TemplateList, Template As Template)
            List.Add(Template)
            Return List
        End Operator

        ''' <summary>
        ''' Adiciona os templates de uma lista á outra lista
        ''' </summary>
        ''' <param name="List">Lista</param>
        ''' <param name="AnotherList">Outra lista</param>
        ''' <returns></returns>
        Public Shared Operator +(List As TemplateList, AnotherList As TemplateList)
            List.AddRange(AnotherList)
            Return List
        End Operator

        ''' <summary>
        ''' Retorna uma string HTML com todos os templates processados e o conteúdo do Head
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim s = ""
            MyBase.Sort(Function(x, y) x.Order.CompareTo(y.Order))
            For Each i In Me
                s.Append(i.Content)
            Next
            s.Append(Head)
            Return s
        End Function

        ''' <summary>
        ''' Adiciona uma lista de templates á lista na posição correta de acordo com a coluna de ordem
        ''' </summary>
        ''' <param name="TemplateList">Lista de templates</param>
        Public Shadows Sub AddRange(TemplateList As TemplateList)
            MyBase.AddRange(TemplateList)
            MyBase.Sort(Function(x, y) x.Order.CompareTo(y.Order))
        End Sub

        ''' <summary>
        ''' Adiciona um novo template á lista na posição correta de acordo com a coluna de ordem
        ''' </summary>
        ''' <param name="Template">template</param>
        Public Shadows Sub Add(Template As Template)
            MyBase.Add(Template)
            MyBase.Sort(Function(x, y) x.Order.CompareTo(y.Order))
        End Sub
        ''' <summary>
        ''' Cria uma lista de templates a partir de varios templates
        ''' </summary>
        ''' <param name="Templates"></param>
        Public Sub New(ParamArray Templates As Template())
            MyBase.New(Templates)
            MyBase.Sort(Function(x, y) x.Order.CompareTo(y.Order))
        End Sub

        ''' <summary>
        ''' Cria uma lista de templates a partir de varias lista de templates
        ''' </summary>
        ''' <param name="Templates">Lista de templates</param>
        Public Sub New(ParamArray Templates As TemplateList())
            MyBase.New()
            For Each t In Templates
                Me.AddRange(t)
            Next
            MyBase.Sort(Function(x, y) x.Order.CompareTo(y.Order))
        End Sub

        ''' <summary>
        ''' Cria uma nova Lista vazia
        ''' </summary>
        Public Sub New()
            MyBase.New()
        End Sub

        ''' <summary>
        ''' Conteudo da tag Head do template principal. é adicionado a string após o fim da recursão.
        ''' </summary>
        ''' <returns></returns>
        Property Head As String
    End Class

    ''' <summary>
    ''' Resultado de uma Query transformada em um template
    ''' </summary>
    Public Class Template
        ''' <summary>
        ''' Conteudo do Template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Content As String

        ''' <summary>
        ''' Objeto que define a ordem de apresentação
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Order As Object

        ReadOnly Property OrderColumn As String

        ''' <summary>
        ''' Arquivo de onde o conteudo foi extraido e processado
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property TemplateFile As String

        ReadOnly Property SQLQuery As String

        Friend Sub New(TemplateFile, Order, Content, SQLQuery, OrderColumn)
            Me.Content = Content
            Me.Order = Order
            Me.TemplateFile = TemplateFile
            Me.OrderColumn = OrderColumn
        End Sub

        ''' <summary>
        ''' Cria uma nova instrução para contrução de um template
        ''' </summary>
        ''' <param name="SQLQuery">Query SQL</param>
        ''' <param name="TemplateFile">Nome do arquivo de template</param>
        ''' <param name="OrderColumn">Nome da Coluna de Ordem</param>
        Public Sub New(SQLQuery As String, TemplateFile As String, Optional OrderColumn As String = "")
            Me.SQLQuery = SQLQuery
            Me.TemplateFile = TemplateFile
            Me.Content = ""
            Me.OrderColumn = OrderColumn
        End Sub
    End Class


End Namespace
