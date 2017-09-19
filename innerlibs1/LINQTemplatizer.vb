Imports System.Data.Common
Imports System.Data.Linq
Imports System.IO
Imports System.Reflection

Namespace Templatizer



    Public Class Templatizer(Of DataContextType As DataContext)


        Private sel As String() = {"##", "##"}


        ''' <summary>
        ''' Seletor dos campos
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
        ''' Conexão com o Banco de Dados
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
        ReadOnly Property ApplicationAssembly As Reflection.Assembly = Nothing


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


        Default Public ReadOnly Property ExecuteSQL(SQLQuery As String) As IEnumerable
            Get
                Return RunSQL(Of Object)(SQLQuery)
            End Get
        End Property

        ''' <summary>
        ''' Retorna um <see cref="IEnumerable"/> com os resultados de uma Query SQL
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="SQLQuery"></param>
        ''' <param name="Parameters"></param>
        ''' <returns></returns>
        Public Function RunSQL(Of T As Class)(SQLQuery As String, ParamArray Parameters As Object()) As IEnumerable(Of T)
            Return DB.ExecuteQuery(Of T)(SQLQuery, Parameters)
        End Function


        ''' <summary>
        ''' Cria um <see cref="InnerLibs.Templatizer.TemplateList"/> a partir de uma Query SQL
        ''' </summary>
        ''' <param name="SQLQuery">Query SQL</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function LoadQuery(Of T As Class)(SQLQuery As String, ByVal Template As String) As TemplateList(Of T)
            Template = GetTemplateContent(Template)
            Return ApplyTemplate(Of T)(DB.ExecuteQuery(Of T)(SQLQuery), Template)
        End Function

        ''' <summary>
        ''' Processa um template configurado
        ''' </summary>
        ''' <param name="TemplateName"> Nome do template</param>
        ''' <returns></returns>
        Public Function Load(Of T As Class)(TemplateName As String) As TemplateList(Of T)
            TemplateName = Path.GetFileNameWithoutExtension(TemplateName)
            Return LoadQuery(Of T)(GetCommand(TemplateName & ".sql"), GetTemplateContent(TemplateName & ".html"))
        End Function



        ''' <summary>
        ''' Aplica um Template a uma lista <see cref="IQueryable"/> de um objeto do tipo <typeparamref name="T"/>
        ''' </summary>
        ''' <param name="Items">Lista de Itens</param>
        ''' <param name="Template">Arquivo ou HTML do template</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Items As IQueryable(Of T), Template As String) As TemplateList(Of T)
            Template = GetTemplateContent(Template)
            Dim output As New TemplateList(Of T)
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
        Public Function ApplyTemplate(Of T As Class)(Items As IEnumerable(Of T), Template As String) As TemplateList(Of T)
            Template = GetTemplateContent(Template)
            Dim output As New TemplateList(Of T)
            For Each item As T In Items
                output.Add(item, ApplyTemplate(Of T)(item, Template))
            Next
            Return output
        End Function


        ''' <summary>
        ''' Aplica um template HTML a uma classe
        ''' </summary>
        ''' <typeparam name="T">Tipo da classe</typeparam>
        ''' <param name="Item">Objeto</param>
        ''' <param name="Template">Arquivo de template ou HTML</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Item As T, ByVal Template As String) As String
            Template = GetTemplateContent(Template)

            For Each i In CustomValues
                Try
                    Template = Template.Replace(Me.ApplySelector(i.Key), "" & i.Value)
                Catch ex As Exception
                    Template = Template.Replace(Me.ApplySelector(i.Key), "")
                End Try
            Next
            For Each prop In ClassTools.GetProperties(Item)
                Try
                    Template = Template.Replace(ApplySelector(prop.Name), "" & prop.GetValue(Item))
                Catch ex As Exception
                    Template = Template.Replace(ApplySelector(prop.Name), "")
                End Try
            Next

            'replace nos if
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

            'replace nas procedures
            For Each templateTag As HtmlTag In Template.GetElementsByTagName("template")
                templateTag.ReplaceIn(Template)
                Try
                    Dim sqltags = templateTag.GetElementsByTagName("sql").First
                    sqltags.ReplaceIn(templateTag.InnerHtml)
                    Dim contenttags = templateTag.GetElementsByTagName("content").First
                    contenttags.ReplaceIn(templateTag.InnerHtml)
                    Template.Replace(templateTag.ToString, LoadQuery(Of T)(If(sqltags("data-file").IsBlank, sqltags.InnerHtml, GetCommand(sqltags("data-file"))), If(contenttags("data-file").IsBlank, contenttags.InnerHtml, GetTemplateContent(contenttags("data-file")))).BuildHtml)
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

    Public Class TemplateList(Of T)
        Inherits List(Of Template(Of T))


        Sub New()

        End Sub


        Sub New(ParamArray Templates As TemplateList(Of Object)())
            For Each I In Templates
                Me.AddRange(I)
            Next
        End Sub


        Public Property Head As String

        Public Overrides Function ToString() As String
            Return Me.BuildHtml()
        End Function

        Public Overloads Sub Add(Data As Object, Content As String)
            Me.Add(New Template(Of T)(Data, Content))
        End Sub

        Public Function BuildHtml() As String
            Return String.Join("", Me.Select(Function(p) p.Content)) & Head
        End Function

        Public Shared Widening Operator CType(v As TemplateList(Of Object)) As TemplateList(Of T)
            Dim na As New TemplateList(Of T)
            For Each i In v
                na.Add(New Template(Of T)(CType(i.Data, T), i.Content))
            Next
            Return na
        End Operator
    End Class

    ''' <summary>
    ''' Resultado de uma Query transformada em um template
    ''' </summary>
    Public Class Template(Of T)

        Public Sub New(Data As T, Content As String)
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
        ReadOnly Property Data As T


    End Class



End Namespace


