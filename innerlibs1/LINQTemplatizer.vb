Imports System.Data.Common
Imports System.Data.Linq
Imports System.IO
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Templatizer

    ''' <summary>
    ''' Gera HTML dinâmico a partir de uma conexão com banco de dados e um template HTML. Utiliza <see cref="DataContext"/> como ponto de entrada para conexões.
    ''' </summary>
    ''' <typeparam name="DataContextType">Tipo da conexão com o banco de dados</typeparam>
    Public Class TemplatizerConfig(Of DataContextType As DataContext)



        ''' <summary>
        ''' Tipo de <see cref="DataContext"/> utilizado para a comunicação com o banco
        ''' </summary>
        ''' <returns></returns>
        Property DB As DataContextType

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



        Sub New(TemplateDirectory As DirectoryInfo, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            Me.DateTimeFormat = If(DateTimeFormat, "dd/MM/yyyy hh:mm:ss")
            Me.TemplateDirectory = TemplateDirectory
            Me.DB = Activator.CreateInstance(Of DataContextType)
        End Sub

        Sub New(ApplicationAssembly As Assembly, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            Me.DateTimeFormat = If(DateTimeFormat, "dd/MM/yyyy hh:mm:ss")
            Me.ApplicationAssembly = ApplicationAssembly
            Me.DB = Activator.CreateInstance(Of DataContextType)
        End Sub


        ''' <summary>
        ''' Executa uma query SQL e retorna um <see cref="IEnumerable"/> com os resultados (É um alias para <see cref="datacontext.ExecuteQuery(Of TResult)(String, Object())"/>
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="SQLQuery"></param>
        ''' <param name="Parameters"></param>
        ''' <returns></returns>
        Public Function ExecuteSQL(Of T As Template)(SQLQuery As String, ParamArray Parameters As Object()) As IEnumerable(Of T)
            Return DB.ExecuteQuery(Of T)(SQLQuery, Parameters)
        End Function

        ''' <summary>
        ''' Executa um comando diretamente no banco de dados sem retornar resultado (é um alias para <see cref="datacontext.ExecuteCommand(String, Object())"/>)
        ''' </summary>
        ''' <param name="Command"></param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        Default ReadOnly Property ExecuteCommand(Command As String, ParamArray Parameters As Object()) As Integer
            Get
                Return DB.ExecuteCommand(Command, Parameters)
            End Get
        End Property


        ''' <summary>
        ''' Cria um <see cref="IEnumerable"/> a partir de uma Query SQL
        ''' </summary>
        ''' <param name="SQLQuery">Query SQL</param>
        ''' <returns></returns>
        Public Function LoadQuery(Of T As Template)(SQLQuery As String) As IEnumerable(Of T)
            Return ApplyTemplate(Of T)(DB.ExecuteQuery(Of T)(SQLQuery))
        End Function

        ''' <summary>
        ''' Carrega um template a partir de uma classe configurada
        ''' </summary>
        ''' <typeparam name="T">Tipo da classe</typeparam>
        ''' <returns></returns>
        Public Function Load(Of T As Template)() As IQueryable(Of T)
            Return ApplyTemplate(Of T)(DB.GetTable(Of T))
        End Function

        ''' <summary>
        ''' Retorna o Template configurado em uma classe de entidade LINQ
        ''' </summary>
        ''' <param name="Type">Tipo da Classe</param>
        ''' <returns></returns>
        Function GetTemplateContent(Type As Type)
            Dim d = Activator.CreateInstance(Type)
            Dim prop = ClassTools.GetProperties(d).Where(Function(p) p.Name = "TemplateFile")
            If prop.Count > 0 Then
                Return GetTemplateContent(prop.First.GetValue(d))
            Else
                Return d.GetType().Name
            End If
        End Function

        ''' <summary>
        ''' Retorna o Template configurado em uma classe de entidade LINQ
        ''' </summary>
        ''' <typeparam name="T">Tipo da Classe</typeparam>
        ''' <returns></returns>
        Function GetTemplateContent(Of T As Template)() As String
            Dim d As T = Activator.CreateInstance(Of T)
            Return GetTemplateContent(d.GetType)
        End Function

        ''' <summary>
        ''' Pega o comando SQL de um arquivo ou resource
        ''' </summary>
        ''' <param name="CommandFile">Nome do arquivo ou resource</param>
        ''' <returns></returns>
        Function GetCommand(CommandFile As String) As String
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
                    Throw New Exception("ApplicationAssembly or CommandDirectory is not configured!")
            End Select
        End Function

        ''' <summary>
        ''' Retorna o conteudo estático de um arquivo de template
        ''' </summary>
        ''' <param name="Templatefile">Nome do arquivo do template</param>
        ''' <param name="HeadTag">TRUE para trazer o conteudo fixo do template (head), FALSE para trazer o conteudo dinamico do template (body)</param>
        ''' <returns></returns>
        Public Function GetTemplateContent(TemplateFile As String, Optional HeadTag As Boolean = False) As String
            If TemplateFile.IsNotBlank Then
                If TemplateFile.ContainsAny("<", ">") Then
                    If HeadTag Then
                        Try
                            Return TemplateFile.GetElementsByTagName("head").First.InnerHtml
                        Catch ex As Exception
                            Return ""
                        End Try
                    Else
                        Return TemplateFile
                    End If
                Else
                    TemplateFile = Path.GetFileNameWithoutExtension(TemplateFile) & ".html"
                    If IsNothing(ApplicationAssembly) Then
                        Dim filefound = TemplateDirectory.SearchFiles(SearchOption.TopDirectoryOnly, TemplateFile).First
                        If Not filefound.Exists Then Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & TemplateDirectory.Name.Quote)
                        Using file As StreamReader = filefound.OpenText
                            Try
                                Return file.ReadToEnd.GetElementsByTagName(If(HeadTag, "head", "body")).First.InnerHtml
                            Catch ex As Exception
                                Throw New Exception(If(HeadTag, "head", "body") & " not found in " & filefound.Name)
                            End Try
                        End Using
                    Else
                        Try
                            Dim txt = GetResourceFileText(ApplicationAssembly, ApplicationAssembly.GetName.Name & "." & TemplateFile)
                            Try
                                Return txt.GetElementsByTagName(If(HeadTag, "head", "body")).First.InnerHtml
                            Catch ex As Exception
                                Throw New Exception(If(HeadTag, "head", "body") & " not found in " & ApplicationAssembly.GetName.Name & "." & TemplateFile)
                            End Try
                        Catch ex As Exception
                            Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & ApplicationAssembly.GetName.Name.Quote & " resources. Check if Build Action is marked as ""Embedded Resource"" in File Properties.")
                        End Try
                    End If
                End If
            End If
            Return TemplateFile
        End Function

        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="ISingleResult"/> de um objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <param name="Items">Lista de Itens</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Template)(Items As ISingleResult(Of Template)) As IQueryable(Of T)
            Return ApplyTemplate(Of T)(Items.AsQueryable)
        End Function

        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="IQueryable"/> de um objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <param name="Items">Lista de Itens</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Template)(Items As IQueryable(Of Template)) As IQueryable(Of T)
            For Each item As Template In Items
                ApplyTemplate(item)
            Next
            Return Items
        End Function

        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="IEnumerable"/> de um objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <param name="Items">Lista de itens</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Template)(Items As IEnumerable(Of Template)) As IQueryable(Of T)
            Return ApplyTemplate(Of T)(Items.AsQueryable)
        End Function

        ''' <summary>
        ''' Aplica um template a um unico item
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Item"></param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Template)(Item As T) As T
            Item._content = GetTemplateContent(ClassTools.GetPropertyValue(Of String)(Item, "TemplateFile"))
            Item._content = Item.ReplaceValues(Item)
            Return Item
        End Function




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

        Private _datetimeformat As String = "dd/MM/yyyy hh:mm:ss"

        ''' <summary>
        ''' Retorna o formato de data configurado em uma classe ou o default
        ''' </summary>
        ''' <typeparam name="T">Tipo da classe</typeparam>
        ''' <param name="Item">Item</param>
        ''' <returns></returns>
        Public Function GetDatetimeFormat(Of T As Template)(Item As Template)
            Try
                Return If(Item.DateTimeFormat, Me.DateTimeFormat)
            Catch ex As Exception
                Return Me.DateTimeFormat
            End Try
        End Function



        ''' <summary>
        ''' Processa as TAGs de subtemplate
        ''' </summary>
        ''' <param name="Template">Template HTML</param>
        ''' <returns></returns>
        Public Function ProcessSubTemplates(Template As String) As String
            Template = GetTemplateContent(Template)
            'replace nas procedures
            For Each templateTag As HtmlTag In Template.GetElementsByTagName("template")
                templateTag.ReplaceIn(Template)
                Try
                    Dim sqltags = templateTag.GetElementsByTagName("sql").First
                    sqltags.ReplaceIn(templateTag.InnerHtml)
                    Dim contenttags = templateTag.GetElementsByTagName("content").First
                    contenttags.ReplaceIn(templateTag.InnerHtml)
                    Dim tipo As Type = Type.GetType(templateTag("type"))
                    ' Template.Replace(templateTag.ToString, LoadQuery(tipo, If(sqltags("data-file").IsBlank, sqltags.InnerHtml, GetCommand(sqltags("data-file"))), If(contenttags("data-file").IsBlank, contenttags.InnerHtml, GetTemplateContent(contenttags("data-file")))).BuildHtml)
                Catch ex As Exception
                    templateTag.RemoveIn(Template)
                End Try
            Next
            Return Template
        End Function
    End Class




    ''' <summary>
    ''' Resultado de uma Query transformada em um template
    ''' </summary>
    Public MustInherit Class Template

        Sub New()

        End Sub

        ''' <summary>
        ''' Conteudo HTML do Template já processado ou o nome do template se o mesmo ainda não foi processado
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ProcessedTemplate As String
            Get
                Return _content.IfBlank(TemplateFile)
            End Get
        End Property

        Friend _content As String = ""

        ''' <summary>
        ''' Formato de data utilizado neste template
        ''' </summary>
        ''' <returns></returns>
        Public Property DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss"

        ''' <summary>
        ''' Template HTML utilizado neste Template
        ''' </summary>
        ''' <returns></returns>
        Public Property TemplateFile As String

        Private sel As String() = {"##", "##"}

        ''' <summary>
        ''' Seletor dos campos do template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property FieldSelector As String()
            Get
                Return sel
            End Get
        End Property

        ''' <summary>
        ''' Aplica um seletor ao nome do campo
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





        ''' <summary>
        ''' Processa as TAGS de condicoes
        ''' </summary>
        ''' <returns></returns>
        Public Function ProcessCondidions() As String
            For Each conditionTag As HtmlTag In _content.GetElementsByTagName("condition")
                Dim oldtag = ""
                Try
                    Dim expression = conditionTag.GetElementsByTagName("expression").First
                    expression.ReplaceIn(conditionTag.InnerHtml.Trim)
                    Dim content = conditionTag.GetElementsByTagName("content").First
                    content.ReplaceIn(conditionTag.InnerHtml)
                    conditionTag.ReplaceIn(_content)
                    oldtag = conditionTag.ToString
                    Dim resultexp = EvaluateExpression(expression.InnerHtml)
                    If resultexp = True Or resultexp > 0 Then
                        _content = _content.Replace(oldtag, _content)
                    Else
                        _content = _content.Replace(oldtag, "")
                    End If
                Catch ex As Exception
                    _content = _content.Replace(oldtag, "")
                End Try
            Next
            Return _content
        End Function

        Friend Function ReplaceValues(Item As Template) As String
            Dim content = ProcessedTemplate
            For Each i As PropertyInfo In Item.GetProperties
                Try
                    If i.GetValue(Item).GetType.IsIn({GetType(DateTime)}) Then
                        content = content.Replace(ApplySelector(i.Name), CType(i.GetValue(Item), Date).ToString(Item.GetPropertyValue("DateTimeFormat")))
                    Else
                        content = content.Replace(ApplySelector(i.Name), i.GetValue(Item).ToString())
                    End If
                Catch ex As Exception
                    content = content.Replace(Me.ApplySelector(i.Name), "")
                End Try
            Next
            Return content
        End Function


    End Class


    Public Module TemplatizerExtensions

        <Extension()>
        Public Function BuildHtml(List As IEnumerable(Of Template)) As String
            Return String.Join("", List.Select(Function(p) p.ProcessedTemplate))
        End Function

        <Extension()>
        Public Function BuildHtml(List As IQueryable(Of Template)) As String
            Return String.Join("", List.Select(Function(p) p.ProcessedTemplate))
        End Function
    End Module



End Namespace


