Imports System.Data.Common
Imports System.Data.Linq
Imports System.Data.Linq.Mapping
Imports System.IO
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports InnerLibs.HtmlParser

Namespace Templatizer

    ''' <summary>
    ''' Gera HTML dinâmico a partir de uma conexão com banco de dados e um template HTML. Utiliza <see cref="Data.Linq.DataContext"/> como ponto de entrada para conexões.
    ''' </summary>
    ''' <typeparam name="DataContextType">Tipo da conexão com o banco de dados</typeparam>
    Public Class Templatizer(Of DataContextType As DataContext)

        ''' <summary>
        ''' Tipo de <see cref="Data.Linq.DataContext"/> utilizado para a comunicação com o banco
        ''' </summary>
        ''' <returns></returns>
        Property DataContext As DataContextType

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
        ''' Lista contento as associações das classes aso seus arquivos de template
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TemplateMap As New Dictionary(Of Type, String)

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


        Private CustomValues As Dictionary(Of String, Object)

        Public Sub WithCustomValue(Key As String, Value As Object)
            CustomValues(Key) = Value
        End Sub


        ''' <summary>
        ''' Retorna o nome do arquivo de template
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <returns></returns>
        Function GetTemplate(Of T As Class)(Optional ProcessFile As Boolean = False) As String
            Return If(ProcessFile, GetTemplateContent(MapType(GetType(T))), MapType(GetType(T)))
        End Function



        ''' <summary>
        ''' Aplica um arquivo de template a um tipo
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Template"></param>
        ''' <returns></returns>
        Function SetTemplate(Of T As Class)(Template As String) As Templatizer(Of DataContextType)
            MapType(GetType(T)) = Template
            Return Me
        End Function


        Sub New(TemplateDirectory As DirectoryInfo, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            Me.DateTimeFormat = If(DateTimeFormat, "dd/MM/yyyy hh:mm:ss")
            Me.TemplateDirectory = TemplateDirectory
            Me.DataContext = Activator.CreateInstance(Of DataContextType)
        End Sub

        Sub New(ApplicationAssembly As Assembly, Optional DateTimeFormat As String = "dd/MM/yyyy hh:mm:ss")
            Me.DateTimeFormat = If(DateTimeFormat, "dd/MM/yyyy hh:mm:ss")
            Me.ApplicationAssembly = ApplicationAssembly
            Me.DataContext = Activator.CreateInstance(Of DataContextType)
        End Sub

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
        ''' Executa uma query SQL e retorna um <see cref="IEnumerable"/> com os resultados (É um wrapper para <see cref="datacontext.ExecuteQuery(Of TResult)(String, Object())"/> porém aplica os templates automaticamente
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="SQLQuery"></param>
        ''' <param name="Parameters"></param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(SQLQuery As String, Optional Template As String = "", Optional Parameters As Object() = Nothing) As List(Of Template(Of T))
            Dim list As IEnumerable(Of T)
            If GetType(T) = GetType(Dictionary(Of String, Object)) Then
                Dim con = Activator.CreateInstance(Me.DataContext.Connection.GetType)
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
                End Using
            Else
                list = DataContext.ExecuteQuery(Of T)(SQLQuery, If(Parameters, {}))
            End If
            Return ApplyTemplate(Of T)(list, Template)
        End Function

        ''' <summary>
        ''' Aplica um template a um unico item
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Item"></param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(Item As T, Optional Template As String = "") As Template(Of T)
            Dim content = ""
            If Template.IsBlank Then
                Template = Me.GetTemplate(Of T)
            End If
            content = "" & GetTemplateContent(Template)
            If content.IsNotBlank Then
                content = ReplaceValues(Item, content)
                content = ReplaceValues(CustomValues, content)
                content = ProccessConditions(Item, content)
                content = ProccessSubTemplate(Item, content)
            End If
            Return New Template(Of T)(Item, content)
        End Function


        Public Function ApplyTemplate(Of T As Class)(List As IQueryable(Of T), Optional Template As String = "")
            Return ApplyTemplate(List.AsEnumerable, Template)
        End Function

        Public Function ApplyTemplate(Of T As Class)(List As Linq.Table(Of T), Optional Template As String = "")
            Return ApplyTemplate(List.AsEnumerable, Template)
        End Function

        Public Function ApplyTemplate(Of T As Class)(List As ISingleResult(Of T), Optional Template As String = "")
            Return ApplyTemplate(List.AsEnumerable, Template)
        End Function

        ''' <summary>
        ''' Aplica um template a uma lista
        ''' </summary>
        ''' <typeparam name="T">Tipo do item</typeparam>
        ''' <param name="List">Lista</param>
        ''' <param name="Template">Nome ou thml do template</param>
        ''' <returns></returns>
        Public Function ApplyTemplate(Of T As Class)(List As IEnumerable(Of T), Optional Template As String = "")
            Dim l As New List(Of Template(Of T))
            For Each item As T In List
                l.Add(ApplyTemplate(Of T)(CType(item, T), Template))
            Next
            CustomValues.Clear()
            Return l
        End Function


        ''' <summary>
        ''' Retorna o conteudo estático de um arquivo de template
        ''' </summary>
        ''' <param name="Templatefile">Nome do arquivo do template</param>
        ''' <param name="HeadTag">TRUE para trazer o conteudo fixo do template (head), FALSE para trazer o conteudo dinamico do template (body)</param>
        ''' <returns></returns>
        Public Function GetTemplateContent(TemplateFile As String, Optional Headtag As Boolean = False) As String
            If TemplateFile.IsNotBlank Then
                If TemplateFile.ContainsAny("<", ">") Then
                    Try
                        Return New HtmlDocument(TemplateFile).HTML
                    Catch ex As Exception
                        Throw New Exception("Error on parsing Template String")
                    End Try
                Else
                    TemplateFile = Path.GetFileNameWithoutExtension(TemplateFile) & ".html"
                    If IsNothing(ApplicationAssembly) Then
                        Dim filefound = TemplateDirectory.SearchFiles(SearchOption.TopDirectoryOnly, TemplateFile).First
                        If Not filefound.Exists Then Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & TemplateDirectory.Name.Quote)
                        Using file As StreamReader = filefound.OpenText
                            Try
                                Return CType(New HtmlDocument(file.ReadToEnd).Nodes.FindByName(If(Headtag, "head", "body"))(0), HtmlElement).InnerHTML
                            Catch ex As Exception
                                Throw New Exception(If(Headtag, "head", "body") & " not found in " & filefound.Name)
                            End Try
                        End Using
                    Else
                        Try
                            Dim txt = GetResourceFileText(ApplicationAssembly, ApplicationAssembly.GetName.Name & "." & TemplateFile)
                            Try
                                Return CType(New HtmlDocument(txt).Nodes.FindByName(If(Headtag, "head", "body"))(0), HtmlElement).InnerHTML
                            Catch ex As Exception
                                Throw New Exception(If(Headtag, "head", "body") & " not found in " & ApplicationAssembly.GetName.Name & "." & TemplateFile)
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
        Private sel As String() = {"##", "##"}

        ''' <summary>
        ''' Seletor dos campos do template
        ''' </summary>
        ''' <returns></returns>
        Function GetFieldSelector() As String()
            Return sel
        End Function

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


        Friend Function ProccessConditions(Of t As Class)(item As t, Template As String) As String
            Dim doc As New HtmlDocument(Template)
            For Each conditionTag As HtmlElement In doc.Nodes.GetElementsByTagName("condition", True)
                Try
                    Dim expression = CType(conditionTag.Nodes.FindByName("expression")(0), HtmlElement).InnerHTML.HtmlDecode
                    Dim contenttag = CType(conditionTag.Nodes.FindByName("content")(0), HtmlElement).InnerHTML.HtmlDecode
                    Dim resultexp = EvaluateExpression(expression)

                    If resultexp = True Or resultexp > 0 Then
                        conditionTag.Mutate(contenttag)
                    Else
                        conditionTag.Destroy()
                    End If
                Catch ex As Exception
                    conditionTag.Destroy()
                End Try
            Next
            Template = doc.InnerHTML
            Return Template
        End Function

        Friend Function ProccessSubTemplate(Of t As Class)(item As t, Template As String) As String
            Dim doc As New HtmlDocument(Template)
            For Each templatetag As HtmlElement In doc.Nodes.GetElementsByTagName("template", True)
                Try
                    Dim sql = CType(templatetag.Nodes.FindByName("sql")(0), HtmlElement).InnerHTML.HtmlDecode
                    Dim conteudo = CType(templatetag.Nodes.FindByName("content")(0), HtmlElement).InnerHTML.HtmlDecode
                    Dim lista = ApplyTemplate(Of Dictionary(Of String, Object))(sql, conteudo)
                    conteudo = lista.BuildHtml
                    templatetag.Mutate(conteudo)
                Catch ex As Exception
                    templatetag.Destroy()
                End Try
            Next
            Template = doc.InnerHTML
            Return Template
        End Function

        Friend Function ReplaceValues(Of T As Class)(Item As T, Template As String) As String
            Template = GetTemplateContent(Template)
            If Template.IsNotBlank Then
                If GetType(T) = GetType(Dictionary(Of String, Object)) Then
                    For Each i In CType(CType(Item, Object), Dictionary(Of String, Object)).Keys.ToArray
                        Try
                            If CType(CType(Item, Object), Dictionary(Of String, Object))(i).GetType.IsIn({GetType(DateTime)}) Then
                                Template = Template.Replace(ApplySelector(i), CType(CType(CType(Item, Object), Dictionary(Of String, Object))(i), Date).ToString(Item.GetPropertyValue("DateTimeFormat")))
                            Else
                                Template = Template.Replace(ApplySelector(i), CType(CType(Item, Object), Dictionary(Of String, Object))(i).ToString())
                            End If
                        Catch ex As Exception
                            Template = Template.Replace(Me.ApplySelector(i), "")
                        End Try
                    Next
                Else
                    For Each i As PropertyInfo In Item.GetProperties
                        Try
                            If i.GetValue(Item).GetType.IsIn({GetType(DateTime)}) Then
                                Template = Template.Replace(ApplySelector(i.Name), CType(i.GetValue(Item), Date).ToString(Item.GetPropertyValue("DateTimeFormat")))
                            Else
                                Template = Template.Replace(ApplySelector(i.Name), i.GetValue(Item).ToString())
                            End If
                        Catch ex As Exception
                            Template = Template.Replace(Me.ApplySelector(i.Name), "")
                        End Try
                    Next
                End If
            End If
            Return Template
        End Function



    End Class




    ''' <summary>
    ''' Estrutura de template do templatizer
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class Template(Of T)
        ''' <summary>
        ''' Cria um novo template
        ''' </summary>
        ''' <param name="Data"></param>
        ''' <param name="ProcessedTemplate"></param>
        Sub New(Data As T, ProcessedTemplate As String)
            Me.Data = Data
            Me.ProcessedTemplate = ProcessedTemplate
        End Sub

        ''' <summary>
        ''' Informação do template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Data As T
        ''' <summary>
        ''' Template processado
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ProcessedTemplate As String
    End Class


    Public Module TemplatizerExtensions




        ''' <summary>
        ''' Retorna o HTML de uma lista de templates
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function BuildHtml(Of T As Class)(List As List(Of Template(Of T))) As String
            Dim html As String = ""
            For Each i In List
                html &= i.ProcessedTemplate
            Next
            Return html
        End Function




    End Module

End Namespace


