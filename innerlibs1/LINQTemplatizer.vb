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
    Public Class Templatizer(Of DataContextType As DataContext)

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


        ''' <summary>
        ''' Valores customizados que serão acrescentados as colunas do Templatizer duranto o processamento
        ''' </summary>
        Public ReadOnly CustomValues As New Dictionary(Of String, Object)


        ''' <summary>
        ''' Adiciona valores customizados ao template
        ''' </summary>
        ''' <param name="Key">Key</param>
        ''' <param name="Value">Valor </param>
        ''' <returns>si proprio</returns>
        Public Function AddCustomValue(Key As String, Value As Object) As Templatizer(Of DataContextType)
            Me.CustomValues(Key) = Value
            Return Me
        End Function

        Sub New(TemplateDirectory As DirectoryInfo)
            Me.TemplateDirectory = TemplateDirectory
            Me.DB = Activator.CreateInstance(Of DataContextType)
        End Sub

        Sub New(ApplicationAssembly As Assembly)
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
        Public Function ExecuteSQL(Of T As Class)(SQLQuery As String, ParamArray Parameters As Object()) As IEnumerable(Of T)
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
        ''' Cria um <see cref="TemplateList"/> a partir de uma Query SQL
        ''' </summary>
        ''' <param name="SQLQuery">Query SQL</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function LoadQuery(Of T As Class)(SQLQuery As String, ByVal Template As String) As TemplateList
            Template = GetTemplateContent(Template)
            Return ApplyTemplate(Of T)(DB.ExecuteQuery(Of T)(SQLQuery), Template)
        End Function


        ''' <summary>
        ''' Cria um <see cref="TemplateList"/> a partir de uma Query SQL
        ''' </summary>
        ''' <param name="SQLQuery">Query SQL</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function LoadQuery(Type As Type, SQLQuery As String, ByVal Template As String) As TemplateList
            Template = GetTemplateContent(Template)
            Return ApplyTemplate(Type, DB.ExecuteQuery(Type, SQLQuery), Template)
        End Function


        ''' <summary>
        ''' Processa um template configurado
        ''' </summary>
        ''' <param name="TemplateName"> Nome do template</param>
        ''' <returns></returns>
        Public Function Load(Of T As Class)(TemplateName As String) As TemplateList
            TemplateName = Path.GetFileNameWithoutExtension(TemplateName)
            Return LoadQuery(Of T)(GetCommand(TemplateName & ".sql"), GetTemplateContent(TemplateName & ".html"))
        End Function

        Public Function Load(Of T As Class)() As String
            Dim d As T
            Return ApplyTemplate(DB.GetTable(Of T), d.GetType().Name)
        End Function

        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="ISingleResult"/> de um objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <param name="Items">Lista de Itens</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Items As ISingleResult(Of T), Template As String) As TemplateList
            Return ApplyTemplate(Of T)(Items.AsQueryable, Template)
        End Function

        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="IQueryable"/> de um objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <param name="Items">Lista de Itens</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Items As IQueryable(Of T), Template As String) As TemplateList
            Template = GetTemplateContent(Template)
            Dim output As New TemplateList
            For Each item In Items
                output.Add(item, ApplyTemplate(Of T)(item, Template))
            Next
            Return output
        End Function

        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="IEnumerable"/> de um objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <param name="Items">Lista de itens</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Items As IEnumerable(Of T), Template As String) As TemplateList
            Template = GetTemplateContent(Template)
            Dim output As New TemplateList
            For Each item As T In Items
                output.Add(item, ApplyTemplate(Of T)(item, Template))
            Next
            Return output
        End Function

        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="IEnumerable"/> 
        ''' </summary>
        ''' <param name="Items">Lista de itens</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Type As Type, Items As IEnumerable, Template As String) As TemplateList
            Template = GetTemplateContent(Template)
            Dim output As New TemplateList
            For Each item In Items
                output.Add(Conversion.CTypeDynamic(item, Type), ApplyTemplate(Conversion.CTypeDynamic(item, Type), Template))
            Next
            Return output
        End Function

        ''' <summary>
        ''' Processa as TAGS de condicoes
        ''' </summary>
        ''' <param name="Template">Template HTML</param>
        ''' <returns></returns>
        Public Function ProcessCondidions(Template As String)
            Template = GetTemplateContent(Template)
            For Each conditionTag As HtmlTag In Template.GetElementsByTagName("condition")
                Dim oldtag = ""
                Try
                    Dim expression = conditionTag.GetElementsByTagName("expression").First
                    expression.ReplaceIn(conditionTag.InnerHtml.Trim)
                    Dim content = conditionTag.GetElementsByTagName("content").First
                    content.ReplaceIn(conditionTag.InnerHtml)
                    conditionTag.ReplaceIn(Template)
                    oldtag = conditionTag.ToString
                    Dim resultexp = EvaluateExpression(expression.InnerHtml)
                    If resultexp = True Or resultexp > 0 Then
                        Template = Template.Replace(oldtag, content.InnerHtml)
                    Else
                        Template = Template.Replace(oldtag, "")
                    End If
                Catch ex As Exception
                    Template = Template.Replace(oldtag, "")
                End Try
            Next
            Return Template
        End Function

        ''' <summary>
        ''' Substitui com os valores de um objeto as keys de um Template
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="Item">Objeto</param>
        ''' <param name="Template">Template HTMl</param>
        ''' <returns></returns>
        Public Function ReplaceValues(Of T)(Item As T, Template As String) As String
            Template = GetTemplateContent(Template)

            If ClassTools.IsDictionary(Item) Then
                Dim dic = CType(Item, IDictionary)
                For Each k In dic.Keys
                    Try
                        If (ClassTools.IsDictionary(dic(k)) OrElse dic(k).GetType.IsClass) AndAlso Not dic(k).GetType.IsIn(ReplaceableTypes) Then
                            Template = ReplaceValues(dic(k), Template)
                        Else
                            Template = Template.Replace(ApplySelector(k), "" & dic(k))
                        End If

                    Catch ex As Exception
                        Template = Template.Replace(Me.ApplySelector(k), "")
                    End Try
                Next
            Else
                If Item.GetType.IsClass Then
                    For Each i As PropertyInfo In Item.GetProperties
                        Try
                            If (i.GetValue(Item).GetType.IsClass OrElse ClassTools.IsDictionary(i.GetValue(Item))) AndAlso Not i.GetValue(Item).GetType.IsIn(ReplaceableTypes) Then
                                Template = ReplaceValues(i.GetValue(Item), Template)
                            Else
                                Template = Template.Replace(ApplySelector(i.Name), "" & i.GetValue(Item))
                            End If
                        Catch ex As Exception
                            Template = Template.Replace(Me.ApplySelector(i.Name), "")
                        End Try
                    Next
                End If
            End If
            Return Template
        End Function

        Dim ReplaceableTypes As Type() = {GetType(String), GetType(DateTime), GetType(Integer), GetType(Long), GetType(Decimal)}


        ''' <summary>
        ''' Aplica um template HTML a uma classe
        ''' </summary>
        ''' <typeparam name="T">Tipo da classe</typeparam>
        ''' <param name="Item">Objeto</param>
        ''' <param name="Template">Arquivo de template ou HTML</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Item As T, ByVal Template As String) As String
            Template = GetTemplateContent(Template)

            Template = ReplaceValues(CustomValues, Template)

            'replace nos valores da class
            Template = ReplaceValues(Of T)(Item, Template)
            'replace nos if
            Template = ProcessCondidions(Template)
            Return Template
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
                    Template.Replace(templateTag.ToString, LoadQuery(tipo, If(sqltags("data-file").IsBlank, sqltags.InnerHtml, GetCommand(sqltags("data-file"))), If(contenttags("data-file").IsBlank, contenttags.InnerHtml, GetTemplateContent(contenttags("data-file")))).BuildHtml)
                Catch ex As Exception
                    templateTag.RemoveIn(Template)
                End Try
            Next
            Return Template
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
                    Return TemplateFile
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



    End Class

    ''' <summary>
    ''' Lista de templates processados
    ''' </summary>

    Public Class TemplateList
        Inherits List(Of Template)

        Sub New()
        End Sub


        ''' <summary>
        ''' Conteudo da tag head que será adiconado após o processamento do template
        ''' </summary>
        ''' <returns></returns>
        Public Property Head As String

        ''' <summary>
        ''' Retorna a string do Template processado
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return Me.BuildHtml()
        End Function

        ''' <summary>
        ''' Adiciona um template a esta lista
        ''' </summary>
        ''' <param name="Data"></param>
        ''' <param name="Content"></param>
        Public Overloads Sub Add(Data As Object, Content As String)
            Me.Add(New Template(Data, Content))
        End Sub

        ''' <summary>
        ''' Retorna a string HTML do Template processado
        ''' </summary>
        ''' <returns></returns>
        Public Function BuildHtml() As String
            Return String.Join("", Me.Select(Function(p) p.Content)) & Head
        End Function

    End Class

    ''' <summary>
    ''' Resultado de uma Query transformada em um template
    ''' </summary>
    Public Class Template

        Public Sub New(Data As Object, Content As String)
            Me.Content = Content
            Me.Data = Data
        End Sub

        ''' <summary>
        ''' Conteudo HTML do Template já processado
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Content As String

        ''' <summary>
        ''' Objeto contendo um resultado da query LINQ
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Data As Object


    End Class


    Module TemplatizerExtensions



    End Module


End Namespace


