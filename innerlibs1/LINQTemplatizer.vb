Imports System.Collections.ObjectModel
Imports System.Data.Common
Imports System.Data.Linq
Imports System.Data.Linq.Mapping
Imports System.IO
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Xml.Serialization
Imports InnerLibs.HtmlParser


Namespace LINQ


    ''' <summary>
    ''' Permite integrar <see cref="Triforce"/> a objetos LINQ to SQL
    ''' </summary>
    ''' <typeparam name="DataContextType">Objeto LINQ to SQL gerado</typeparam>
    Public NotInheritable Class Triforce(Of DataContextType As DataContext)
        Inherits Triforce

        ''' <summary>
        ''' Tipo de <see cref="Data.Linq.DataContext"/> utilizado para a comunicação com o banco
        ''' </summary>
        Friend DataContext As DataContextType

        ''' <summary>
        ''' Instancia um novo <see cref="LINQ"/> a partir de um Diretório
        ''' </summary>
        ''' <param name="TemplateDirectory">Diretório contendo os arquivos HTML</param>
        ''' <param name="DateTimeFormat"></param>
        Sub New(TemplateDirectory As DirectoryInfo, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            MyBase.New(TemplateDirectory, DateTimeFormat)
            Me.DataContext = Activator.CreateInstance(Of DataContextType)
        End Sub


        ''' <summary>
        ''' Instancia um novo <see cref="LINQ"/> a partir de um Assembly
        ''' </summary>
        ''' <param name="ApplicationAssembly">Assembly contendo os arquivos HTML. Os arquivos HTML devem ser marcados como EMBEDDED RESOURCE</param>
        ''' <param name="DateTimeFormat"></param>
        Sub New(ApplicationAssembly As Assembly, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            MyBase.New(ApplicationAssembly, DateTimeFormat)
            Me.DataContext = Activator.CreateInstance(Of DataContextType)
        End Sub




        ''' <summary>
        ''' Aplica um template a uma busca determinada pelo tipo de objeto
        ''' </summary>
        ''' <typeparam name="T">Tipo de objeto</typeparam>
        ''' <param name="PageNumber">Pagina atual</param>
        ''' <param name="PageSize">Numero de itens por pagina</param>
        ''' <param name="predicade">Filtro da busca</param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(Optional predicade As Expression(Of Func(Of T, Boolean)) = Nothing, Optional PageNumber As Integer = 0, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Return ApplyTemplate(Of T)(GetTemplate(Of T), predicade, PageNumber, PageSize)
        End Function
        ''' <summary>
        ''' Aplica um template a uma busca determinada pelo tipo de objeto
        ''' </summary>
        ''' <typeparam name="T">Tipo de objeto</typeparam>
        ''' <param name="PageNumber">Pagina atual</param>
        ''' <param name="PageSize">Numero de itens por pagina</param>
        ''' <param name="predicade">Filtro da busca</param>
        ''' <returns></returns>
        Public Overrides Function ApplyTemplate(Of T As Class)(Template As String, Optional predicade As Expression(Of Func(Of T, Boolean)) = Nothing, Optional PageNumber As Integer = 0, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Me.DataContext = Activator.CreateInstance(Of DataContextType)
            Using Me.DataContext
                Dim d As IQueryable(Of T) = Me.DataContext.GetTable(Of T).AsQueryable
                If predicade IsNot Nothing Then
                    d = d.Where(predicade)
                End If
                Return ApplyTemplate(d, Template, PageNumber, PageSize)
            End Using
        End Function


        ''' <summary>
        ''' Executa uma query SQL e retorna um <see cref="IEnumerable"/> com os resultados (É um wrapper para <see cref="DataContext.ExecuteQuery(Of TResult)(String, Object())"/> porém aplica os templates automaticamente
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="SQLQuery"></param>
        ''' <param name="Parameters"></param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(SQLQuery As String, Template As String, ParamArray Parameters As Object()) As TemplateList(Of T)
            Debug.WriteLine(Environment.NewLine & SQLQuery)
            Dim list As IEnumerable(Of T)
            Me.DataContext = Activator.CreateInstance(Of DataContextType)
            Using Me.DataContext
                If GetType(T) = GetType(Dictionary(Of String, Object)) Then
                    Dim con As DbConnection = Activator.CreateInstance(Me.DataContext.Connection.GetType)
                    con.ConnectionString = Me.DataContext.Connection.ConnectionString
                    con.Open()
                    Dim command As DbCommand = con.CreateCommand()
                    command.CommandText = SQLQuery
                    Using Reader As DbDataReader = command.ExecuteReader()
                        Dim l As New List(Of Dictionary(Of String, Object))
                        While Reader.Read
                            Dim d As New Dictionary(Of String, Object)
                            For i As Integer = 0 To Reader.FieldCount - 1
                                d.Add(Reader.GetName(i), Reader(Reader.GetName(i)))
                            Next
                            l.Add(d)
                        End While
                        list = l.AsEnumerable
                        Reader.Dispose()
                    End Using
                    con.Close()
                Else
                    list = DataContext.ExecuteQuery(Of T)(SQLQuery, If(Parameters, {}))
                End If
                Return ApplyTemplate(Of T)(list, Template, 1, -1)
            End Using
        End Function


        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="IQueryable"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(List As IQueryable(Of T), Optional Template As String = "", Optional PageNumber As Integer = 1, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Dim total = List.Count
            If PageSize < 1 Then PageSize = total
            PageNumber = PageNumber.LimitRange(1, (total / PageSize).Ceil())
            Dim l As New List(Of Template(Of T))
            Dim ll = List.Page(PageNumber, PageSize)
            For Each item As T In ll
                l.Add(ApplyTemplate(Of T)(CType(item, T), Template))
            Next
            Dim tpl = New TemplateList(Of T)(l, PageSize, PageNumber, total)
            Try
                tpl.Head = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "head"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try
            Try
                tpl.Footer = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "footer"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try
            Try
                tpl.Empty = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "empty"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try

            Try
                tpl.Pagination = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "pagination"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try
            Return tpl
        End Function

        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="IQueryable"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(List As IQueryable(Of T), Template As HtmlDocument, PageNumber As Integer, PageSize As Integer) As TemplateList(Of T)
            Return ApplyTemplate(List, Template.ToString, PageNumber, PageSize)
        End Function

        ''' <summary>
        ''' Extrai uma Query SQL de um arquivo e retorna um <see cref="TemplateList"/> com os resultados
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="TemplateName">Nome do Template configurado</param>
        ''' <param name="Parameters"></param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(TemplateName As String, ParamArray Parameters As Object()) As TemplateList(Of T)
            Return ApplyTemplate(Of T)(GetCommand(TemplateName), TemplateName, Parameters)
        End Function

        ''' <summary>
        ''' Extrai o Comando SQL e o Template HTML definidos para o objeto do tipo <typeparamref name="T"/> e retorna um <see cref="TemplateList(Of T)"/> com os resultados 
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)() As TemplateList(Of T)
            Return ApplyTemplate(Of T)(GetCommand(GetType(T).Name), GetTemplate(Of T))
        End Function

        ''' <summary>
        ''' Aplica um template HTML a um unico objeto
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="Item">Objeto</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(Item As T, Optional Template As String = "") As Template(Of T)
            If Template.IsBlank Then
                Template = Me.GetTemplate(Of T)
            End If
            Me.doc = New HtmlDocument(GetTemplateContent(Template))

            'replace nos valores
            TravesseAndReplace(doc.Nodes, Item)
            TravesseAndReplace(doc.Nodes, CustomValues)

            'processa subtemplates
            ProccessSubClass(Item)
            ProcessSubTemplate(Item)

            doc = New HtmlDocument(ClearValues(doc.ToString))

            'processar logica
            ProccessConditions(Item)
            ProccessSwitch(Item)
            ProccessIf(Item)
            ProcessRepeat()

            Return New Template(Of T)(Item, doc.ToString)
        End Function


        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="Data.Linq.Table(Of TEntity)"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(List As Table(Of T), Optional Template As String = "", Optional PageNumber As Integer = 1, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Return ApplyTemplate(List.AsQueryable, Template, PageNumber, PageSize)
        End Function

        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="Data.Linq.Table(Of TEntity)"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(List As Table(Of T), Template As HtmlDocument, Optional PageNumber As Integer = 1, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Return ApplyTemplate(List, Template.ToString, PageNumber, PageSize)
        End Function


        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="ISingleResult"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(List As ISingleResult(Of T), Optional Template As String = "", Optional PageNumber As Integer = 1, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Return ApplyTemplate(List.AsQueryable, Template, PageNumber, PageSize)
        End Function

        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="ISingleResult"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Overloads Function ApplyTemplate(Of T As Class)(List As ISingleResult(Of T), Template As HtmlDocument, Optional PageNumber As Integer = 1, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Return ApplyTemplate(List, Template.ToString, PageNumber, PageSize)
        End Function




#Region "Processors"
        Friend Sub ProcessSubTemplate(Of T As Class)(item As T)

            Dim listat = doc.Nodes.GetElementsByTagName("template", True)
            For index = 0 To listat.Count - 1
                Dim templatetag As HtmlElement = listat(index)
                If templatetag.HasAttribute("disabled") Then
                    templatetag.Destroy()
                    Continue For
                End If
                Try
                    If templatetag.Name = "template" Then
                        Dim sql = ""
                        Dim el_sql = CType(templatetag.Nodes.GetElementsByTagName("sql").First, HtmlElement)
                        If el_sql.HasAttribute("file") AndAlso el_sql.Attribute("file").IsNotBlank Then
                            sql = GetCommand(el_sql.Attribute("file"))
                        Else
                            sql = el_sql.InnerHTML.HtmlDecode
                        End If
                        Dim conteudo = ""
                        Dim el_cont = CType(templatetag.Nodes.GetElementsByTagName("content").First, HtmlElement)
                        If el_cont.HasAttribute("file") AndAlso el_cont.Attribute("file").IsNotBlank Then
                            conteudo = GetTemplateContent(el_cont.Attribute("file"))
                        Else
                            conteudo = el_cont.InnerHTML.HtmlDecode
                        End If

                        Dim lista = ApplyTemplate(Of Dictionary(Of String, Object))(sql, conteudo, {})
                        If lista.Count > 0 Then
                            conteudo = lista.ToString
                            templatetag.Mutate(conteudo)
                        Else
                            Throw New Exception("Empty results")
                        End If
                    End If
                Catch ex As Exception
                    Dim plc As HtmlElement = Nothing
                    Try
                        plc = CType(templatetag.FindElements(Of HtmlElement)(Function(x) x.Name.ToLower = "nocontent").FirstOrDefault, HtmlElement)
                    Catch
                    End Try
                    If plc IsNot Nothing Then
                        Dim conteudo = ""
                        If plc.HasAttribute("file") AndAlso plc.Attribute("file").IsNotBlank Then
                            conteudo = GetTemplateContent(plc.Attribute("file"))
                        Else
                            conteudo = plc.InnerHTML.HtmlDecode
                        End If
                        templatetag.Mutate(conteudo)
                    Else
                        templatetag.Destroy()
                        ' index.Decrement
                    End If
                End Try
            Next
        End Sub

#End Region



        ''' <summary>
        ''' Pega o comando SQL de um arquivo ou resource
        ''' </summary>
        ''' <param name="CommandFile">Nome do arquivo ou resource</param>
        ''' <returns></returns>
        Public Function GetCommand(CommandFile As String) As String
            CommandFile = Path.GetFileNameWithoutExtension(CommandFile) & ".sql"
            Select Case True
                Case IsNothing(ApplicationAssembly) And Not IsNothing(TemplateDirectory)
                    Dim filefound = TemplateDirectory.SearchFiles(SearchOption.TopDirectoryOnly, CommandFile).First
                    If Not filefound.Exists Then Throw New FileNotFoundException(CommandFile.Quote & "  not found in " & TemplateDirectory.Name.Quote)
                    Using file As StreamReader = filefound.OpenText
                        Return file.ReadToEnd
                    End Using
                Case Not IsNothing(ApplicationAssembly) And IsNothing(TemplateDirectory)
                    Try
                        Return GetResourceFileText(ApplicationAssembly, ApplicationAssembly.GetName.Name & "." & CommandFile)
                    Catch ex As Exception
                        Throw New FileNotFoundException(CommandFile.Quote & "  not found in " & ApplicationAssembly.GetName.Name.Quote & " resources. Check if Build Action is marked as ""Embedded Resource"" in File Properties.")
                    End Try
                Case Else
                    Throw New Exception("ApplicationAssembly or TemplateDirectory is not configured!")
            End Select
        End Function



    End Class


    ''' <summary>
    ''' Gerador de HTML dinâmico a partir de objetos LINQ e arquivos HTML.
    ''' </summary>
    Public Class Triforce
        ''' <summary>
        ''' Instancia um novo <see cref="LINQ"/> a partir de um Assembly
        ''' </summary>
        ''' <param name="ApplicationAssembly">Assembly contendo os arquivos HTML. Os arquivos HTML devem ser marcados como EMBEDDED RESOURCE</param>
        ''' <param name="DateTimeFormat"></param>
        Sub New(ApplicationAssembly As Assembly, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            Me.DateTimeFormat = If(DateTimeFormat, "dd/MM/yyyy hh:mm:ss")
            Me.ApplicationAssembly = ApplicationAssembly
        End Sub

        ''' <summary>
        ''' Instancia um novo <see cref="LINQ"/> a partir de um Assembly
        ''' </summary>
        ''' <param name="TemplateDirectory">Diretorio contendo os arquivos HTML</param>
        ''' <param name="DateTimeFormat"></param>
        Sub New(TemplateDirectory As DirectoryInfo, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            Me.DateTimeFormat = If(DateTimeFormat, "dd/MM/yyyy hh:mm:ss")
            Me.TemplateDirectory = TemplateDirectory
        End Sub



        ''' <summary>
        ''' Pasta contendo os arquivos HTML e SQL utilizados como template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property TemplateDirectory As DirectoryInfo = Nothing

        ''' <summary>
        ''' Aplicaçao contendo os Resources (arquivos compilados internamente) dos arquivos HTML e SQL
        ''' utilizados como template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ApplicationAssembly As Assembly = Nothing

        ''' <summary>
        ''' Mapeia um template para um tipo
        ''' </summary>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        Default Public Property MapType(Type As Type) As String
            Get
                If TemplateMap.ContainsKey(Type) Then
                    Return TemplateMap(Type)
                Else
                    Return ""
                End If
            End Get
            Set(value As String)
                TemplateMap(Type) = value
            End Set
        End Property


        ''' <summary>
        ''' Valores adicionados ao processamento do template que não vem do banco de dados ou do objeto. Particulamente Util para dados de sessão.
        ''' </summary>
        ''' <returns></returns>
        Public Property CustomValues As New Dictionary(Of String, Object)

        ''' <summary>
        ''' Formato das datas apresentadas no template
        ''' </summary>
        ''' <returns></returns>
        Public Property DateTimeFormat As String
            Get
                Return _datetimeformat.IfBlank("dd/MM/yyyy hh:mm:ss")
            End Get
            Set(value As String)
                _datetimeformat = value.IfBlank("dd/MM/yyyy hh:mm:ss")
            End Set
        End Property

        Friend _datetimeformat As String = "dd/MM/yyyy hh:mm:ss"
        Friend sel As String() = {"##", "##"}
        Friend doc As HtmlDocument 'documento em processamento
        Friend TemplateMap As New Dictionary(Of Type, String)

        ''' <summary>
        ''' Retorna o nome do arquivo de template
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Function GetTemplate(Of T As Class)(Optional ProcessFile As Boolean = False) As String
            Return If(ProcessFile, GetTemplateContent(MapType(GetType(T))), MapType(GetType(T)))
        End Function

        ''' <summary>
        ''' Configura um arquivo de template para um tipo especifico de objeto. 
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Template"></param>
        ''' <returns></returns>
        Function SetTemplate(Of T As Class)(Template As String) As Triforce
            MapType(GetType(T)) = Template
            Return Me
        End Function





        ''' <summary>
        ''' Aplica um template a uma busca determinada pelo tipo de objeto
        ''' </summary>
        ''' <typeparam name="T">Tipo de objeto</typeparam>
        ''' <param name="PageNumber">Pagina atual</param>
        ''' <param name="PageSize">Numero de itens por pagina</param>
        ''' <param name="predicade">Filtro da busca</param>
        ''' <returns></returns>
        Public Overridable Function ApplyTemplate(Of T As Class)(Template As String, Optional predicade As Expression(Of Func(Of T, Boolean)) = Nothing, Optional PageNumber As Integer = 0, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Throw New NotImplementedException("This function works only with LINQ to SQL")
        End Function

        ''' <summary>
        ''' Aplica um template HTML a um unico objeto
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="Item">Objeto</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Item As T, Template As HtmlDocument) As Template(Of T)
            Return ApplyTemplate(Item, Template.ToString)
        End Function


        ''' <summary>
        ''' Aplica um template HTML a um unico objeto
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="Item">Objeto</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Item As T, Optional Template As String = "") As Template(Of T)
            If Template.IsBlank Then
                Template = Me.GetTemplate(Of T)
            End If
            Me.doc = New HtmlDocument(GetTemplateContent(Template))

            'replace nos valores
            TravesseAndReplace(doc.Nodes, Item)
            TravesseAndReplace(doc.Nodes, CustomValues)

            'processa subtemplates
            'este nao tem 
            ProccessSubClass(Item)

            doc = New HtmlDocument(ClearValues(doc.ToString))
            'processar logica
            ProccessConditions(Item)
            ProccessSwitch(Item)
            ProccessIf(Item)
            ProcessRepeat()

            Return New Template(Of T)(Item, doc.ToString)
        End Function



        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="IEnumerable"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(List As IEnumerable(Of T), Optional Template As String = "", Optional PageNumber As Integer = 1, Optional PageSize As Integer = 0) As TemplateList(Of T)
            Dim total = List.Count
            If PageSize < 1 Then PageSize = total
            If total > 0 Then
                PageNumber = PageNumber.LimitRange(1, (total / PageSize).Ceil())
                Dim l As New List(Of Template(Of T))
                For Each item As T In List.Page(PageNumber, PageSize)
                    l.Add(ApplyTemplate(Of T)(CType(item, T), Template))
                Next
            End If

            Dim tpl = New TemplateList(Of T)(l, PageSize, PageNumber, total)
            Try
                tpl.Head = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "head"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try
            Try
                tpl.Footer = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "footer"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try
            Try
                tpl.Empty = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "empty"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try

            Try
                tpl.Pagination = ReplaceValues(Me.CustomValues, GetTemplateContent(Template, "pagination"))
            Catch ex As Exception
                Debug.WriteLine(ex)
            End Try
            Return tpl
        End Function


        ''' <summary>
        ''' Aplica um template HTML a um objeto <see cref="IEnumerable"/>
        ''' </summary>
        ''' <typeparam name="T">TIpo de objeto usado como fonte dos dados</typeparam>
        ''' <param name="List">Lista de objetos</param>
        ''' <param name="Template">Template HTML ou nome do template HTML previamente configurado pelo metodo (<see cref="SetTemplate"/></param>)
        ''' <param name="PageNumber">Pagina que será processada.</param>
        ''' <param name="PageSize">Quantidade de itens por página. Passar o valor 0 para trazer todos os itens</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(List As IEnumerable(Of T), Template As HtmlDocument, PageNumber As Integer, PageSize As Integer) As TemplateList(Of T)
            Return ApplyTemplate(List, Template.ToString, PageNumber, PageSize)
        End Function

        ''' <summary>
        ''' Retorna o conteudo estático de um arquivo de template
        ''' </summary>
        ''' <param name="Templatefile">Nome do arquivo do template</param>
        ''' <param name="Tag">Indica qual tag dve ser capturada, head, body ou footer</param>
        ''' <returns></returns>
        Public Function GetTemplateContent(TemplateFile As String, Optional Tag As String = "body") As String
            If TemplateFile.IsNotBlank Then
                If TemplateFile.ContainsAny("<", ">") Then
                    Try
                        Return New HtmlDocument(TemplateFile).HTML
                    Catch ex As Exception
                        Throw New Exception("Error on parsing Template String: " & ex.Message)
                    End Try
                Else
                    TemplateFile = Path.GetFileNameWithoutExtension(TemplateFile) & ".html"
                    If IsNothing(ApplicationAssembly) Then
                        Dim filefound = TemplateDirectory.SearchFiles(SearchOption.TopDirectoryOnly, TemplateFile).First
                        If Not filefound.Exists Then Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & TemplateDirectory.Name.Quote)
                        Using file As StreamReader = filefound.OpenText
                            Try
                                Return CType(New HtmlDocument(file.ReadToEnd).Nodes.GetElementsByTagName(Tag)(0), HtmlElement).InnerHTML
                            Catch ex As Exception
                                Throw New Exception(Tag & " not found in " & filefound.Name)
                            End Try
                        End Using
                    Else
                        Try
                            Dim txt = GetResourceFileText(ApplicationAssembly, ApplicationAssembly.GetName.Name & "." & TemplateFile)
                            Try
                                Return CType(New HtmlDocument(txt).Nodes.GetElementsByTagName(Tag)(0), HtmlElement).InnerHTML
                            Catch ex As Exception
                                Throw New Exception(Tag & " not found in " & ApplicationAssembly.GetName.Name & "." & TemplateFile)
                            End Try
                        Catch ex As Exception
                            Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & ApplicationAssembly.GetName.Name.Quote & " resources. Check if Build Action is marked as ""Embedded Resource"" in File Properties.")
                        End Try
                    End If
                End If
            End If
            Return ""
        End Function
        ''' <summary>
        ''' Seletor dos campos do template
        ''' </summary>
        ''' <returns></returns>
        Function GetFieldSelector() As String()
            Return sel
        End Function

        ''' <summary>
        ''' Aplica os seletores deste Triforce a uma string (nome do campo)
        ''' </summary>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        Public Function ApplySelector(Name As String) As String
            Return sel(0) & Name & sel(1)
        End Function

        ''' <summary>
        ''' Altera os seletores padrão dos campos
        ''' </summary>
        ''' <param name="OpenSelector"> Seletor</param>
        ''' <param name="CloseSelector"></param>
        Public Sub SetSelector(OpenSelector As String, CloseSelector As String)
            sel(0) = If(OpenSelector.IsBlank, "##", OpenSelector)
            sel(1) = If(CloseSelector.IsBlank, "##", CloseSelector)
        End Sub

#Region "Processors"


        Friend Sub ProccessSubClass(Of T As Class)(item As T)
            Dim lista = doc.Nodes.GetElementsByAttributeName("triforce-source", True)
            For index = 0 To lista.Count - 1
                Dim conditiontag As HtmlElement = lista(index)
                If conditiontag.HasAttribute("disabled") Then
                    conditiontag.Destroy()
                    Continue For
                End If
                Try
                    Dim classe As IEnumerable(Of Object) = ClassTools.GetPropertyValue(item, conditiontag.Attribute("triforce-source"))
                    Dim template = conditiontag.InnerHTML
                    ApplyTemplate(Of Object)(classe, template, 0, 0)
                Catch ex As Exception
                    conditiontag.Destroy()
                End Try
            Next
        End Sub

        Friend Sub ProccessConditions(Of T As Class)(Item As T)

            Dim lista = doc.Nodes.GetElementsByTagName("condition", True)
            For index = 0 To lista.Count - 1
                Dim conditiontag As HtmlElement = lista(index)
                If conditiontag.HasAttribute("disabled") Then
                    conditiontag.Destroy()
                    Continue For
                End If
                Try
                    If conditiontag.Name = "condition" Then
                        Dim expression = CType(conditiontag.Nodes.GetElementsByTagName("expression", False)(0), HtmlElement).InnerHTML.HtmlDecode
                        Dim contenttag = CType(conditiontag.Nodes.GetElementsByTagName("content", False)(0), HtmlElement).InnerHTML.HtmlDecode
                        Dim resultexp = EvaluateExpression(expression)

                        If resultexp = True Or resultexp > 0 Then
                            conditiontag.Mutate(contenttag)
                        Else
                            conditiontag.Destroy()
                        End If
                    End If
                Catch ex As Exception
                    conditiontag.Destroy()
                End Try
            Next
        End Sub

        Friend Sub ProccessIf(Of T As Class)(Item As T)
            Dim lista = doc.Nodes.GetElementsByTagName("IF", True)
            For index = 0 To lista.Count - 1
                Dim conditiontag As HtmlElement = lista(index)
                If conditiontag.HasAttribute("disabled") Then
                    conditiontag.Destroy()
                    Continue For
                End If
                Try
                    If conditiontag.Name = "if" Then
                        Dim expression = CType(conditiontag.Nodes.GetElementsByTagName("expression")(0), HtmlElement).InnerHTML.HtmlDecode
                        Dim truetag = CType(conditiontag.Nodes.GetElementsByTagName("true", False)(0), HtmlElement).InnerHTML.HtmlDecode
                        Dim falsetag = ""
                        Try
                            falsetag = CType(conditiontag.Nodes.GetElementsByTagName("false", False)(0), HtmlElement).InnerHTML.HtmlDecode
                        Catch
                        End Try
                        Dim resultexp = EvaluateExpression(expression)
                        If resultexp = True OrElse resultexp > 0 Then
                            conditiontag.Mutate(truetag)
                        Else
                            If falsetag.IsNotBlank Then
                                conditiontag.Mutate(falsetag)
                            Else
                                conditiontag.Destroy()
                            End If
                        End If
                    End If
                Catch ex As Exception
                    conditiontag.Destroy()
                End Try
            Next
        End Sub

        Friend Sub ProccessSwitch(Of T As Class)(Item As T)
            Dim lista = doc.Nodes.GetElementsByTagName("switch", True)
            For index = 0 To lista.Count - 1
                Dim conditiontag As HtmlElement = lista(index)
                If conditiontag.HasAttribute("disabled") Then
                    conditiontag.Destroy()
                    Continue For
                End If
                Try
                    If conditiontag.Name = "switch" Then

                        Dim othertag = conditiontag.FindElements(Function(n As HtmlElement)
                                                                     Dim value1 = conditiontag.Attribute("value").HtmlDecode
                                                                     Dim value2 = n.Attribute("value").HtmlDecode
                                                                     Dim op = n.Attribute("operator").HtmlDecode.IfBlank("=")
                                                                     value1 = value1.QuoteIf(Not value1.IsNumber)
                                                                     value2 = value2.QuoteIf(Not value2.IsNumber)
                                                                     Return n.Name = "case" AndAlso (EvaluateExpression(value1 & op & value2) = True)
                                                                 End Function, False)
                        Dim html = ""
                        If othertag.Count > 0 Then
                            For Each node As HtmlElement In othertag
                                html.Append(node.InnerHTML)
                                If node.HasAttribute("break") Then
                                    Exit For
                                End If
                            Next
                        Else
                            othertag = conditiontag.FindElements(Function(n As HtmlElement) n.Name = "else", False)
                            If othertag.Count = 1 Then
                                html = CType(othertag.First, HtmlElement).InnerHTML
                            End If
                        End If

                        If html.IsNotBlank Then
                            conditiontag.Mutate(html)
                        Else
                            conditiontag.Destroy()
                        End If
                    End If

                Catch ex As Exception
                    conditiontag.Destroy()
                End Try
            Next
        End Sub

        Friend Sub ProcessRepeat()
            Dim lista = doc.Nodes.GetElementsByTagName("repeat", True)
            For index = 0 To lista.Count - 1
                Dim repeattag As HtmlElement = lista(index)
                If repeattag.HasAttribute("disabled") Then
                    repeattag.Destroy()
                    Continue For
                End If
                If repeattag.HasAttribute("value") Then
                    Dim base = repeattag.InnerHTML
                    repeattag.InnerHTML = ""
                    Dim n = repeattag.Attribute("value")
                    If Not n.IsNumber Then n = n.Length
                    For index2 = 1 To n.ChangeType(Of Integer)
                        repeattag.InnerHTML &= base.Replace("_index", index2)
                    Next
                Else
                    repeattag.Destroy()
                End If
            Next
        End Sub
#End Region





        Friend Function GetRegexPattern() As String
            Dim open = Me.sel(0).ToArray.Join("\").Prepend("\")
            Dim close = Me.sel(1).ToArray.Join("\").Prepend("\")
            Return open & "(.*?)" & close
        End Function


        Friend Sub TravesseAndReplace(Of T As Class)(nodes As HtmlNodeCollection, item As T)
            For index_el = 0 To nodes.Count - 1
                Dim el As HtmlNode = nodes(index_el)
                If TypeOf el Is HtmlElement Then
                    Dim cel = CType(el, HtmlElement)
                    'Replace no nome da tag
                    cel.Name = ReplaceValues(item, cel.Name)

                    'replace dos atributos
                    For Each at In cel.Attributes
                        at.Name = ReplaceValues(item, at.Name)
                        at.Value = ReplaceValues(item, at.Value)
                    Next

                    If cel.Nodes.Count > 0 Then
                        TravesseAndReplace(CType(el, HtmlElement).Nodes, item)
                    End If
                Else
                    Dim ctx = CType(el, HtmlText).Text
                    Dim txt = ReplaceValues(item, ctx)
                    nodes.ReplaceElement(el, New HtmlParser.HtmlParser().Parse(txt))
                End If
            Next
        End Sub


        Friend Function ReplaceValues(Of T As Class)(Item As T, StringToReplace As String) As String

            If StringToReplace.IsNotBlank Then
                Dim ff As MatchEvaluator
                Dim pt = GetRegexPattern()
                Dim re As Regex = New Regex(pt, RegexOptions.Compiled)
                If GetType(T) = GetType(Dictionary(Of String, Object)) AndAlso CType(CType(Item, Object), Dictionary(Of String, Object)).Count > 0 Then
                    ff = Function(match)
                             Dim s As String
                             Try
                                 s = CType(Item, IDictionary)(match.Groups(1).Value)
                                 If s Is Nothing Then Throw New KeyNotFoundException(ApplySelector(match.Groups(1).Value) & " not found")
                             Catch ex As Exception
                                 s = ApplySelector(match.Groups(1).Value)
                             End Try
                             Return s
                         End Function
                Else
                    ff = Function(match As Match) As String
                             Dim val = Item.GetPropertyValue(Of Object)(match.Groups(1).Value, True)
                             If val Is Nothing Then Return ApplySelector(match.Groups(1).Value)
                             If val.GetType.IsIn({GetType(Date), GetType(Date?)}) Then
                                 Dim d As Date? = val
                                 If d.HasValue Then
                                     Return d.Value.ToString(DateTimeFormat)
                                 Else
                                     Return ""
                                 End If
                             Else
                                 Return val.ToString
                             End If
                         End Function
                End If
                StringToReplace = re.Replace(StringToReplace, ff)
            End If
            Return StringToReplace
        End Function


        Friend Function ClearValues(StringToClear As String) As String
            If StringToClear.IsNotBlank Then
                Dim ff As MatchEvaluator
                Dim pt = GetRegexPattern()
                Dim re As Regex = New Regex(pt, RegexOptions.Compiled)
                ff = Function(match)
                         Return ""
                     End Function
                StringToClear = re.Replace(StringToClear, ff)
            End If
            Return StringToClear
        End Function
    End Class


    ''' <summary>
    ''' Estrutura de template do Triforce
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class Template(Of T)

        ''' <summary>
        ''' Cria um novo template
        ''' </summary>
        ''' <param name="Data"></param>
        ''' <param name="ProcessedTemplate"></param>
        Friend Sub New(Data As T, ProcessedTemplate As String)
            Me.Data = Data
            Me.ProcessedTemplate = New HtmlDocument(ProcessedTemplate)
        End Sub

        ''' <summary>
        ''' Objeto de onde são extraidos as informações do template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Data As T
        ''' <summary>
        ''' Template processado
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ProcessedTemplate As HtmlDocument

        ''' <summary>
        ''' Nome do template utilizado para este processamento.
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property TemplateName As String = ""

    End Class


    ''' <summary>
    ''' Lista paginada contendo <see cref="Template(Of T)"/> previamente processados
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <remarks>Apenas os templates da pagina atual são processados</remarks>
    Public Class TemplateList(Of T As Class)
        Inherits ReadOnlyCollection(Of Template(Of T))

        Friend Sub New(lista As List(Of Template(Of T)), Pagesize As Integer, PageNumber As Integer, Total As Integer)
            MyBase.New(lista)
            Me.PageSize = Pagesize
            Me.PageNumber = PageNumber
            Me.Total = Total
        End Sub

        ''' <summary>
        ''' HTML da paginaçao dos itens
        ''' </summary>
        ''' <returns></returns>
        Property Pagination As String
            Get
                Dim paginationdoc As New HtmlDocument(_pagination)
                Dim p As New HtmlDocument(_pagination)
                Dim first As HtmlElement = Nothing
                Dim last As HtmlElement = Nothing
                Dim page As HtmlElement = Nothing
                Dim active As HtmlElement = Nothing
                Dim back As HtmlElement = Nothing
                Dim nex As HtmlElement = Nothing
                Try
                    page = p.Nodes.GetElementsByTagName("page").First
                Catch ex As Exception
                    Return ""
                End Try
                Try
                    active = p.Nodes.GetElementsByTagName("active").First
                Catch ex As Exception
                    active = p.Nodes.GetElementsByTagName("page").First
                End Try
                Try
                    first = p.Nodes.GetElementsByTagName("first").First
                Catch ex As Exception
                    first = p.Nodes.GetElementsByTagName("page").First

                End Try
                Try
                    last = p.Nodes.GetElementsByTagName("last").First
                Catch ex As Exception
                    last = p.Nodes.GetElementsByTagName("page").First
                End Try
                Try
                    back = p.Nodes.GetElementsByTagName("back").First
                Catch ex As Exception
                End Try
                Try
                    nex = p.Nodes.GetElementsByTagName("next").First
                Catch ex As Exception
                End Try

                If Me.PageCount > 1 Then
                    Dim limit = page.Attribute("limit")

                    If Not limit.IsNumber Then
                        limit = PageCount - 2
                    End If

                    Dim dic As New Dictionary(Of String, String)
                    dic("##ACTIVEPAGE##") = PageNumber
                    dic("##PAGE##") = PageNumber
                    dic("##COUNT##") = PageCount
                    dic("##SIZE##") = PageSize
                    dic("##TOTAL##") = Total

                    Dim beforeafter As Decimal = (limit - 1) / 2
                    If beforeafter.IsOdd Then beforeafter = (beforeafter + 1).Floor
                    Dim before = PageNumber
                    Dim after = PageNumber
                    Dim pagestring = active.InnerHTML.Replace(dic)

                    For c = 0 To beforeafter
                        before = before - 1
                        after = after + 1

                        If before > 1 And before < PageCount Then
                            dic("##PAGE##") = before
                            pagestring.Prepend(page.InnerHTML.Replace(dic))
                        End If

                        If after > 1 And after < PageCount Then
                            dic("##PAGE##") = after
                            pagestring.Append(page.InnerHTML.Replace(dic))
                        End If
                    Next
                    dic("##PAGE##") = PageNumber - 1
                    pagestring.PrependIf(back.InnerHTML.Replace(dic), back IsNot Nothing AndAlso PageNumber > 1)

                    dic("##PAGE##") = 1
                    pagestring.PrependIf(first.InnerHTML.Replace(dic), PageNumber > 1)

                    dic("##PAGE##") = PageNumber + 1
                    pagestring.AppendIf(nex.InnerHTML.Replace(dic), nex IsNot Nothing AndAlso PageNumber < PageCount)

                    dic("##PAGE##") = PageCount
                    pagestring.AppendIf(last.InnerHTML.Replace(dic), PageNumber < PageCount)

                    paginationdoc.Nodes.GetElementsByTagName("page").First.Parent.InnerHTML = pagestring
                    Return paginationdoc.InnerHTML.Replace(dic)
                End If
                Return ""
            End Get
            Set(value As String)
                _pagination = value
            End Set
        End Property
        Friend _pagination As String = ""


        ''' <summary>
        ''' HTML retornado quando não houver itens na lista ou na página atual
        ''' </summary>
        ''' <returns></returns>
        Property Empty As String = ""

        ''' <summary>
        ''' Html adcionado antes do template
        ''' </summary>
        ''' <returns></returns>
        Property Head As String = ""

        ''' <summary>
        ''' html adicionado após os template
        ''' </summary>
        ''' <returns></returns>
        Property Footer As String = ""

        ''' <summary>
        ''' Numero de Itens por pagina
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PageSize As Integer

        ''' <summary>
        ''' Pagina atual. Corresponde ao grupo de itens que foram processados
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PageNumber As Integer

        ''' <summary>
        ''' Total de Itens encontrados na <see cref="IQueryable"/> ou <see cref="IEnumerable"/>
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Total As Integer

        ''' <summary>
        ''' Numero de Paginas deste template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PageCount As Integer
            Get
                Return (Total / PageSize).ChangeType(Of Decimal).Ceil
            End Get
        End Property

        ''' <summary>
        ''' Retorna o HTML da pagina atual da lista de templates 
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return BuildHtml.ToString()
        End Function

        ''' <summary>
        ''' Retorna o HTML da pagina atual da lista de templates 
        ''' </summary>
        ''' <returns></returns>
        Public Function BuildHtml() As HtmlDocument
            Dim html As String = ""
            For Each i In Me
                html &= i.ProcessedTemplate.ToString
            Next
            Dim pg = "" & Pagination
            Return New HtmlDocument(Head.Replace("#_PAGINATION_#", pg) & html.IfBlank(Empty) & Footer.Replace("#_PAGINATION_#", pg))
        End Function

        Public Shared Widening Operator CType(v As TemplateList(Of T)) As String
            Return v.ToString
        End Operator
    End Class


End Namespace


