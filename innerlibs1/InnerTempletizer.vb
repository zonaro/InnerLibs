Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports System.Web

Namespace Templatizer

    ''' <summary>
    ''' Gera HTML dinâmico a partir de uma conexão com banco de dados e um template HTML
    ''' </summary>
    Public NotInheritable Class Templatizer

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

        Public Function ApplySelector(Name As String) As String
            Return sel(0) & Name & sel(1)
        End Function

        ''' <summary>
        ''' Altera os seletores padrão dos campos
        ''' </summary>
        ''' <param name="OpenSelector">Seletor</param>
        ''' <param name="CloseSelector"></param>
        Public Sub SetSelector(OpenSelector As String, CloseSelector As String)
            sel(0) = If(OpenSelector.IsBlank, "##", OpenSelector)
            sel(1) = If(CloseSelector.IsBlank, "##", CloseSelector)
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
        ReadOnly Property TemplateDirectory As DirectoryInfo = Nothing

        ''' <summary>
        ''' Aplicaçao contendo os Resources (arquivos compilados internamente) dos aruqivos HTML utilziados como template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ApplicationAssembly As Reflection.Assembly = Nothing

        ''' <summary>
        ''' Cria uma instancia de Templatizer diretamente de um diretório e uma connectionstring
        ''' </summary>
        ''' <typeparam name="ConnectionType">Tipo de ConnectionString</typeparam>
        ''' <param name="ConnectionString">String de conexão com o banco</param>
        ''' <param name="Directory">Diretório com os arquivos SQL e os arquivos HTML</param>
        ''' <returns></returns>
        Public Shared Function Create(Of ConnectionType As DbConnection)(ConnectionString As String, Directory As DirectoryInfo) As Templatizer
            Return New Templatizer(DataBase.Create(Of ConnectionType)(ConnectionString, Directory), Directory)
        End Function

        ''' <summary>
        ''' Cria uma instancia de Templatizer diretamente de um diretório e uma connectionstring
        ''' </summary>
        ''' <typeparam name="ConnectionType">Tipo de ConnectionString</typeparam>
        ''' <param name="ConnectionString">String de conexão com o banco</param>
        ''' <param name="ApplicationAssembly">Assembly da aplicação onde estão os arquivos html e sql guardados como resources</param>
        ''' <returns></returns>
        Public Shared Function Create(Of ConnectionType As DbConnection)(ConnectionString As String, ApplicationAssembly As Assembly) As Templatizer
            Return New Templatizer(DataBase.Create(Of ConnectionType)(ConnectionString, ApplicationAssembly))
        End Function

        ''' <summary>
        ''' Instancia um Novo Templatizer herdando a <see cref="DataBase.ApplicationAssembly"/>
        ''' </summary>
        ''' <param name="DataBase">Conexão com o banco de dados</param>
        Sub New(DataBase As DataBase)
            Me.DataBase = DataBase
            If Not IsNothing(DataBase.ApplicationAssembly) Then
                Me.ApplicationAssembly = DataBase.ApplicationAssembly
            Else
                Throw New Exception("ApplicationAssembly not found in DataBase. Specify ApplicationAssembly in DataBase constructor or Templatizer constructor.")
            End If
        End Sub

        ''' <summary>
        ''' Instancia um Novo Templatizer utilizando uma Pasta para guardar os templates
        ''' </summary>
        ''' <param name="DataBase">Conexão com o banco de dados</param>
        ''' <param name="TemplateFolder">Pasta com os arquivos HTML dos templates</param>
        Sub New(DataBase As DataBase, TemplateFolder As DirectoryInfo)
            Me.TemplateDirectory = TemplateFolder
            Me.DataBase = DataBase
        End Sub

        ''' <summary>
        ''' Instancia um Novo Templatizer utilizando uma Assembly para guardas os templates como resources
        ''' </summary>
        ''' <param name="DataBase">Conexão com o banco de dados</param>
        ''' <param name="ApplicationAssembly">Assembly da aplicação onde os arquivos HTML dos templates estão compilados</param>
        Sub New(DataBase As DataBase, ApplicationAssembly As Reflection.Assembly)
            Me.DataBase = DataBase
            Me.ApplicationAssembly = ApplicationAssembly
        End Sub
        ''' <summary>
        ''' Traz a lista dos arquivos de template encontrados
        ''' </summary>
        ''' <returns></returns>
        Public Function GetTemplateList() As TemplateList
            Dim l As New TemplateList
            Dim ordenator As Long = 0
            If IsNothing(ApplicationAssembly) Then
                For Each f In TemplateDirectory.GetFiles
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

        ''' <summary>
        ''' Retorna o conteudo estático de um arquivo de template
        ''' </summary>
        ''' <param name="Templatefile">Nome do arquivo do template</param>
        ''' <returns></returns>
        Public Function GetTemplateContent(TemplateFile As String) As String
            TemplateFile = Path.GetFileNameWithoutExtension(TemplateFile) & ".html"
            If IsNothing(ApplicationAssembly) Then
                Dim filefound = TemplateDirectory.SearchFiles(SearchOption.TopDirectoryOnly, TemplateFile).First
                If Not filefound.Exists Then Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & TemplateDirectory.Name.Quote)
                Using file As StreamReader = filefound.OpenText
                    Return file.ReadToEnd
                End Using
            Else
                Try
                    Return GetResourceFileText(ApplicationAssembly, ApplicationAssembly.GetName.Name & "." & TemplateFile)
                Catch ex As Exception
                    Throw New FileNotFoundException(TemplateFile.Quote & "  not found in " & ApplicationAssembly.GetName.Name.Quote & " resources. Check if Build Action is marked as ""Embedded Resource"" in File Properties.")
                End Try
            End If
        End Function

        ''' <summary>
        ''' Processa os comandos SQL de varios templates e cria uma unica lista com todos eles
        ''' </summary>
        ''' <param name="Templates">Templates</param>
        ''' <returns></returns>
        Public Function LoadTemplates(ParamArray Templates As Template()) As TemplateList
            Dim retorno As New List(Of Template)
            For Each t In Templates
                retorno.AddRange(LoadQuery(t.SQLQuery, t.TemplateFile, t.OrderColumn))
            Next
            Return New TemplateList(retorno.ToArray)
        End Function

        ''' <summary>
        ''' Processa o comando SQL e retorna os resultados em HTML utilizando o arquivo de template especificado
        ''' </summary>
        ''' <param name="SQLQuery">Comando SQL</param>
        ''' <param name="TemplateFile">Arquivo do Template</param>
        ''' <param name="OrderColumn">Coluna do resultado que será utilizado como ordem</param>
        ''' <returns></returns>
        Public Function LoadQuery(SQLQuery As String, TemplateFile As String, Optional OrderColumn As String = "") As TemplateList
            Return processar(Of TemplateList)(SQLQuery, TemplateFile, OrderColumn)
        End Function

        ''' <summary>
        ''' Processa o comando de um arquivo SQL e retorna os resultados em HTML utilizando o arquivo de template especificado
        ''' </summary>
        ''' <param name="CommandFile">Arquivo de comando SQL</param>
        ''' <param name="TemplateFile">Arquivo do Template</param>
        ''' <param name="OrderColumn">Coluna do resultado que será utilizado como ordem</param>
        ''' <returns></returns>
        Public Function LoadFile(CommandFile As String, TemplateFile As String, Optional OrderColumn As String = "") As TemplateList
            Return LoadQuery(DataBase.GetCommand(CommandFile), TemplateFile, OrderColumn)
        End Function

        ''' <summary>
        ''' Processa um template configurado
        ''' </summary>
        ''' <param name="TemplateName">Nome do template</param>
        ''' <param name="OrderColumn">Coluna utilizada para ordenar</param>
        ''' <returns></returns>
        Default Public ReadOnly Property Load(TemplateName As String, Optional OrderColumn As String = "") As TemplateList
            Get
                TemplateName = Path.GetFileNameWithoutExtension(TemplateName)
                Return LoadFile(TemplateName & ".sql", TemplateName & ".html", OrderColumn)

            End Get
        End Property

        Private Function processar(Of Type)(SQLQuery As String, TemplateFile As String, Optional OrderColumn As String = "") As Type
            Dim response As Object = Nothing

            Select Case GetType(Type)
                Case GetType(String)
                    response = ""
                Case GetType(TemplateList)
                    response = New TemplateList
            End Select

            Dim template As String = ""
            Dim header As String = ""

            If Not TemplateFile.ContainsAny("<", ">") Then
                template = GetTemplateContent(TemplateFile)
                Try
                    header = template.GetElementsByTagName("head").First.InnerHtml
                Catch ex As Exception
                End Try
                Try
                    Dim bb = template.GetElementsByTagName("body").First
                    template = bb.InnerHtml
                    OrderColumn = If(OrderColumn.IsBlank, bb.Attribute("data-ordercolumn"), OrderColumn)
                Catch ex As Exception
                End Try
            Else
                template = TemplateFile
            End If

            Using reader As DataBase.Reader = DataBase.RunSQL(SQLQuery.ToString)
                Dim ordenator As Int64 = 0
                While reader.Read
                    ordenator.Increment
                    Dim copia As String = template
                    'replace nas strings padrão
                    For Each col In reader.GetColumns
                        copia = copia.Replace(ApplySelector(col), reader(col).ToString())
                    Next
                    'replace nas procedures
                    For Each sqlTag As HtmlTag In copia.GetElementsByTagName("template")
                        sqlTag.FixIn(copia)
                        Dim tp As String = ""
                        Dim novaquery As String = sqlTag("data-sqlquery")
                        Dim parametrossql As String = sqlTag("data-sqlparameters")
                        If novaquery.IsBlank AndAlso sqlTag("data-sqlfile").IsNotBlank Then
                            novaquery = DataBase.GetCommand(sqlTag("data-sqlfile"))
                            If parametrossql.IsNotBlank Then
                                Dim colecaoparam = HttpUtility.ParseQueryString(parametrossql)
                                For Each param In colecaoparam.AllKeys
                                    novaquery.Replace(ApplySelector(param), colecaoparam(param))
                                Next
                            End If
                        End If
                        Dim arquivo As String = sqlTag("data-templatefile")
                        Dim coluna As String = sqlTag("data-ordercolumn")
                        If novaquery.IsNotBlank Then
                            If arquivo.IsBlank Then
                                arquivo = sqlTag.InnerHtml
                            End If
                            tp = processar(Of String)(novaquery, arquivo, coluna)
                        End If
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
                    response.Head = header
            End Select
            Return response
        End Function
    End Class

    ''' <summary>
    ''' Lista de resultados gerados a partir de um ou mais templates
    ''' </summary>
    Public Class TemplateList
        Inherits List(Of Template)

        ''' <summary>
        ''' Conteudo da tag Head do template principal. é adicionado a string após o fim da recursão.
        ''' </summary>
        ''' <returns></returns>
        Property Head As String

        ''' <summary>
        ''' Cria uma lista de templates a partir de varios templates
        ''' </summary>
        ''' <param name="Templates"></param>
        Public Sub New(ParamArray Templates As Template())
            MyBase.New(Templates)
            ReOrder()
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
        End Sub

        ''' <summary>
        ''' Cria uma nova Lista vazia
        ''' </summary>
        Public Sub New()
            MyBase.New()
        End Sub
        ''' <summary>
        ''' Retorna uma string HTML com todos os templates processados e o conteúdo do Head
        ''' </summary>
        ''' <param name="AscendingOrder">Se TRUE, Ordem crescente</param>
        ''' <returns></returns>
        Public Function BuildHtml(Optional AscendingOrder As Boolean = True) As String
            Return ReOrder(AscendingOrder).ToString()
        End Function

        ''' <summary>
        ''' Retorna uma string HTML com todos os templates processados e o conteúdo do Head
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim htmlstring As String = ""
            For Each i In Me
                htmlstring.Append(i.Content)
            Next
            htmlstring.Append(Head)
            Return htmlstring
        End Function

        ''' <summary>
        ''' Reordena a lista de acordo com a propriedade <see cref="Template.Order"/>
        ''' </summary>
        ''' <param name="AscendingOrder">Se TRUE, Ordem crescente</param>
        ''' <returns></returns>
        Function ReOrder(Optional AscendingOrder = True) As TemplateList
            Me.Sort(Function(x, y) x.CompareTo(y))
            If Not AscendingOrder Then Me.Reverse()
            Return Me
        End Function

        ''' <summary>
        ''' Adiciona uma lista de templates á lista na posição correta de acordo com a coluna de ordem
        ''' </summary>
        ''' <param name="TemplateList">Lista de templates</param>
        Public Shadows Sub AddRange(TemplateList As TemplateList)
            MyBase.AddRange(TemplateList)
            ReOrder()
        End Sub

        ''' <summary>
        ''' Adiciona uma lista de templates á lista na posição correta de acordo com a coluna de ordem
        ''' </summary>
        ''' <param name="TemplateList">Lista de templates</param>
        Public Shadows Sub AddRange(TemplateList As IEnumerable(Of Template))
            MyBase.AddRange(TemplateList)
            ReOrder()
        End Sub

        ''' <summary>
        ''' Adiciona um novo template á lista na posição correta de acordo com a coluna de ordem
        ''' </summary>
        ''' <param name="Template">template</param>
        Public Shadows Sub Add(Template As Template)
            MyBase.Add(Template)
            ReOrder()
        End Sub

        ''' <summary>
        ''' Concatena o HTML da lista de templates a string
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="TemplateList">Lista de Template</param>
        ''' <returns></returns>
        Public Shared Operator &(Text As String, TemplateList As TemplateList) As String
            Return Text & TemplateList.ToString
        End Operator

        ''' <summary>
        ''' Adiciona um template á lista
        ''' </summary>
        ''' <param name="List">Lista</param>
        ''' <param name="Template">Template</param>
        ''' <returns></returns>
        Public Shared Operator +(List As TemplateList, Template As Template) As TemplateList
            List.Add(Template)
            Return List
        End Operator

        ''' <summary>
        ''' Adiciona os templates de uma lista á outra lista
        ''' </summary>
        ''' <param name="List">Lista</param>
        ''' <param name="AnotherList">Outra lista</param>
        ''' <returns></returns>
        Public Shared Operator +(List As TemplateList, AnotherList As TemplateList) As TemplateList
            List.AddRange(AnotherList)
            Return List
        End Operator

        ''' <summary>
        ''' Concatena o HTML da lista de templates a string
        ''' </summary>
        ''' <param name="Text">Texto</param>
        ''' <param name="TemplateList">Lista de Template</param>
        ''' <returns></returns>
        Public Shared Operator +(Text As String, TemplateList As TemplateList) As String
            Return Text & TemplateList.ToString
        End Operator
    End Class

    ''' <summary>
    ''' Resultado de uma Query transformada em um template
    ''' </summary>
    Public Class Template
        Implements IComparable
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

        ''' <summary>
        ''' Coluna utilizada na ordenação da lista
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property OrderColumn As String

        ''' <summary>
        ''' Arquivo de onde o conteudo foi extraido e processado
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property TemplateFile As String

        ''' <summary>
        ''' Query SQL aplicada neste template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property SQLQuery As String

        Friend Sub New(TemplateFile, Order, Content, SQLQuery, OrderColumn)
            Me.Content = Content
            Me.Order = Order
            Me.TemplateFile = TemplateFile
            Me.OrderColumn = OrderColumn
            Me.SQLQuery = SQLQuery
        End Sub

        ''' <summary>
        ''' Cria uma nova instrução para contrução de um template
        ''' </summary>
        ''' <param name="SQLQuery">Query SQL</param>
        ''' <param name="TemplateFile">Nome do arquivo de template</param>
        ''' <param name="OrderColumn">Nome da Coluna de Ordem</param>
        Public Sub New(SQLQuery As String, TemplateFile As String, Optional OrderColumn As String = "", Optional IsCommandFile As Boolean = False)
            Me.SQLQuery = SQLQuery
            Me.TemplateFile = TemplateFile
            Me.Content = Nothing
            Me.OrderColumn = OrderColumn
            Me.Order = 0
        End Sub

        ''' <summary>
        ''' Metodo de comparação de ordem dos templates
        ''' </summary>
        ''' <param name="Template">Template</param>
        ''' <returns></returns>
        Public Function CompareTo(Template As Object) As Integer Implements IComparable.CompareTo
            If IsNothing(Template) Then Return -1
            If IsNothing(Template.Order) Then Return -1
            Dim orderToCompare As Template = TryCast(Template, Template)
            If Template.Order.GetType.Equals(orderToCompare.Order.GetType) Then
                Select Case Template.Order.GetType
                    Case GetType(String)
                        Return -1 * String.Compare(orderToCompare.Order, Order, True)
                    Case GetType(DateTime)
                        Return -1 * DateTime.Compare(orderToCompare.Order, Order)
                    Case GetType(Integer), GetType(Decimal), GetType(Short), GetType(Long)
                        Select Case True
                            Case orderToCompare.Order < Order
                                Return -1
                            Case orderToCompare.Order > Order
                                Return 1
                            Case Else
                                Return 0
                        End Select
                    Case GetType(Byte())
                        Select Case True
                            Case orderToCompare.Order.Length < Order.Length
                                Return -1
                            Case orderToCompare.Order.Length > Order.Length
                                Return 1
                            Case Else
                                Return 0
                        End Select
                    Case Else
                        Return -1
                End Select
            Else
                Return -1
            End If
        End Function

    End Class

End Namespace