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
        ''' Aplicaçao contendo os Resources (arquivos compilados internamente) dos arquivos HTML
        ''' utilizados como template
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property ApplicationAssembly As Reflection.Assembly = Nothing

        ''' <summary>
        ''' Cria uma instancia de Templatizer diretamente de um diretório e uma connectionstring
        ''' </summary>
        ''' <typeparam name="ConnectionType">Tipo de ConnectionString</typeparam>
        ''' <param name="ConnectionString">String de conexão com o banco</param>
        ''' <param name="Directory">       Diretório com os arquivos SQL e os arquivos HTML</param>
        ''' <returns></returns>
        Public Shared Function Create(Of ConnectionType As DbConnection)(ConnectionString As String, Directory As DirectoryInfo) As Templatizer
            Return New Templatizer(DataBase.Create(Of ConnectionType)(ConnectionString, Directory), Directory)
        End Function

        ''' <summary>
        ''' Cria uma instancia de Templatizer diretamente de um diretório e uma connectionstring
        ''' </summary>
        ''' <typeparam name="ConnectionType">Tipo de ConnectionString</typeparam>
        ''' <param name="ConnectionString">   String de conexão com o banco</param>
        ''' <param name="ApplicationAssembly">
        ''' Assembly da aplicação onde estão os arquivos html e sql guardados como resources
        ''' </param>
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
        ''' <param name="DataBase">      Conexão com o banco de dados</param>
        ''' <param name="TemplateFolder">Pasta com os arquivos HTML dos templates</param>
        Sub New(DataBase As DataBase, TemplateFolder As DirectoryInfo)
            Me.TemplateDirectory = TemplateFolder
            Me.DataBase = DataBase
        End Sub

        ''' <summary>
        ''' Instancia um Novo Templatizer utilizando uma Assembly para guardas os templates como resources
        ''' </summary>
        ''' <param name="DataBase">           Conexão com o banco de dados</param>
        ''' <param name="ApplicationAssembly">
        ''' Assembly da aplicação onde os arquivos HTML dos templates estão compilados
        ''' </param>
        Sub New(DataBase As DataBase, ApplicationAssembly As Assembly)
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
                    l.Add(New Template(f.Name, Nothing, "", GetTemplateContent(f.Name), ""))
                Next
            Else
                For Each n In ApplicationAssembly.GetManifestResourceNames()
                    ordenator.Increment
                    l.Add(New Template(n, Nothing, "", GetTemplateContent(n), ""))
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
            If TemplateFile.IsNotBlank Then
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
            End If
            Return ""
        End Function

        ''' <summary>
        ''' Processa os comandos SQL de varios templates e cria uma unica lista com todos eles
        ''' </summary>
        ''' <param name="Templates">Templates</param>
        ''' <returns></returns>
        Public Function LoadTemplates(ParamArray Templates As Template()) As TemplateList
            Dim retorno As New List(Of Template)
            For Each t In Templates
                retorno.AddRange(LoadQuery(t.SQLQuery, t.TemplateFile, t.OrderByColumn))
            Next
            Return New TemplateList(retorno.ToArray)
        End Function

        ''' <summary>
        ''' Processa o comando SQL e retorna os resultados em HTML utilizando o arquivo de template especificado
        ''' </summary>
        ''' <param name="SQLQuery">     Comando SQL</param>
        ''' <param name="TemplateFile"> Arquivo do Template</param>
        ''' <param name="OrderByColumn">Coluna do resultado que será utilizado como ordem</param>
        ''' <returns></returns>
        Public Function LoadQuery(SQLQuery As String, TemplateFile As String, Optional OrderByColumn As String = "") As TemplateList
            Return processar(Of TemplateList)(SQLQuery, TemplateFile, OrderByColumn)
        End Function

        ''' <summary>
        ''' Processa o comando de um arquivo SQL e retorna os resultados em HTML utilizando o arquivo
        ''' de template especificado
        ''' </summary>
        ''' <param name="CommandFile">  Arquivo de comando SQL</param>
        ''' <param name="TemplateFile"> Arquivo do Template</param>
        ''' <param name="OrderByColumn">Coluna do resultado que será utilizado como ordem</param>
        ''' <returns></returns>
        Public Function LoadFile(CommandFile As String, TemplateFile As String, Optional OrderByColumn As String = "") As TemplateList
            Return LoadQuery(DataBase.GetCommand(CommandFile), TemplateFile, OrderByColumn)
        End Function

        ''' <summary>
        ''' Processa um template configurado
        ''' </summary>
        ''' <param name="TemplateName"> Nome do template</param>
        ''' <param name="OrderByColumn">Coluna utilizada para ordenar</param>
        ''' <returns></returns>
        Default Public ReadOnly Property Load(TemplateName As String, Optional OrderByColumn As String = "") As TemplateList
            Get
                TemplateName = Path.GetFileNameWithoutExtension(TemplateName)
                Return LoadFile(TemplateName & ".sql", TemplateName & ".html", OrderByColumn)
            End Get
        End Property

        Friend Function processar(Of Type)(SQLQuery As String, TemplateFile As String, Optional OrderByColumn As String = "", Optional TemplateOrder As TemplateOrder = TemplateOrder.DataBaseDefault) As Type
            Dim response As Object = Nothing


            Select Case GetType(Type)
                Case GetType(String)
                    response = ""
                Case GetType(TemplateList)
                    response = New TemplateList
            End Select

            Dim template As String = ""
            Dim header As String = ""

            If Not TemplateFile.ContainsAny("<", ">") Or TemplateFile.IsBlank Then
                template = GetTemplateContent(TemplateFile)
                Try
                    header = template.GetElementsByTagName("head").First.InnerHtml
                Catch ex As Exception
                End Try
                Try
                    Dim bb = template.GetElementsByTagName("body").First
                    template = bb.InnerHtml
                    OrderByColumn = If(OrderByColumn.IsBlank, bb.Attribute("data-orderbycolumn"), OrderByColumn)
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
                    For Each i In reader.GetColumns
                        Dim v = ""
                        Try
                            v = reader(i).ToString
                        Catch ex As Exception
                        End Try
                        copia = copia.Replace(ApplySelector(i), v)
                    Next





                    'replace nas procedures
                    For Each templateTag As HtmlTag In copia.GetElementsByTagName("template")
                        templateTag.ReplaceIn(copia)
                        Dim tp As String = ""
                        Dim novaquery As String = templateTag("data-sqlquery")
                        Dim parametrossql As String = templateTag("data-sqlparameters")
                        If novaquery.IsBlank AndAlso templateTag("data-sqlfile").IsNotBlank Then
                            novaquery = DataBase.GetCommand(templateTag("data-sqlfile"))
                            If parametrossql.IsNotBlank Then
                                Dim colecaoparam = HttpUtility.ParseQueryString(parametrossql)
                                For Each param In colecaoparam.AllKeys
                                    novaquery.Replace(ApplySelector(param), colecaoparam(param))
                                Next
                            End If
                        End If
                        Dim arquivo As String = templateTag("data-templatefile")
                        Dim coluna As String = templateTag("data-orderbycolumn")
                        If novaquery.IsNotBlank Then
                            If arquivo.IsBlank Then
                                arquivo = templateTag.InnerHtml
                            End If
                            tp = processar(Of String)(novaquery, arquivo, coluna)
                        End If
                        copia = copia.Replace(templateTag.ToString, tp)
                    Next

                    'replace nos if

                    For Each conditionTag As HtmlTag In copia.GetElementsByTagName("condition")
                        Dim oldtag = ""
                        Try
                            Dim expression = conditionTag.GetElementsByTagName("expression").First
                            expression.ReplaceIn(conditionTag.InnerHtml.Trim)
                            Dim content = conditionTag.GetElementsByTagName("content").First
                            content.ReplaceIn(conditionTag.InnerHtml)
                            conditionTag.ReplaceIn(copia)
                            oldtag = conditionTag.ToString
                            Dim resultexp = EvaluateExpression(expression.InnerHtml)
                            If resultexp = True Or resultexp > 0 Then
                                copia = copia.Replace(oldtag, content.InnerHtml)
                            Else
                                copia = copia.Replace(oldtag, "")
                            End If
                        Catch ex As Exception
                            copia = copia.Replace(oldtag, "")
                        End Try

                    Next

                    Select Case GetType(Type)
                        Case GetType(String)
                            response = response & copia
                        Case GetType(TemplateList)
                            If OrderByColumn.IsNotBlank AndAlso OrderByColumn.IsIn(reader.GetColumns) Then
                                response.Add(New Template(TemplateFile, reader.GetCurrentRow, OrderByColumn, copia, SQLQuery))
                            Else
                                response.Add(New Template(TemplateFile, reader.GetCurrentRow, reader.GetColumns()(0), copia, SQLQuery))
                            End If
                    End Select
                End While
            End Using
            Select Case GetType(Type)
                Case GetType(String)
                    response = response & header
                Case GetType(TemplateList)
                    response.Head.Add(header.Split(Environment.NewLine))

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
        Property Head As New List(Of String())

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
        ''' <param name="OrderBy">Se TRUE, Ordem crescente</param>
        ''' <returns></returns>
        Public Function BuildHtml(Optional OrderBy As TemplateOrder = 0) As String
            Return ReOrder(OrderBy).ToString()
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
            For Each cab In Head.Distinct
                For Each cabitem In cab.Distinct
                    htmlstring.Append(cabitem & Environment.NewLine)
                Next
            Next
            Return htmlstring
        End Function

        ''' <summary>
        ''' Procura valores no <see cref="Template.Data"/> que contenham qualquer um dos termos pesquisados
        ''' </summary>
        ''' <param name="SearchTerms">Termos Pesquisados</param>
        ''' <returns></returns>
        Function SearchAny(ParamArray SearchTerms As String()) As TemplateList
            SearchAny = New TemplateList
            For Each t As Template In Me
                For Each k In t.Data.Keys.ToArray
                    If t.Data(k).ToString.ContainsAny(SearchTerms) And Not t.IsIn(SearchAny) Then
                        SearchAny.Add(t)
                    End If
                Next
            Next
            Return SearchAny
        End Function

        ''' <summary>
        ''' Procura valores no <see cref="Template.Data"/> que contenham qualquer um dos termos
        ''' pesquisados usando comparação fonética
        ''' </summary>
        ''' <param name="SearchTerms">Termos Pesquisados</param>
        ''' <returns></returns>
        Function SearchAny(ParamArray SearchTerms As Phonetic()) As TemplateList
            SearchAny = New TemplateList
            For Each t As Template In Me
                For Each k In t.Data.Keys.ToArray
                    For Each p In SearchTerms
                        If Not IsNothing(t.Data(k)) Then
                            If (p.IsListenedIn(t.Data(k).ToString) Or (t.Data(k).ToString.ContainsAny(p.Word))) And Not t.IsIn(SearchAny) Then
                                SearchAny.Add(t)
                            End If
                        End If
                    Next
                Next
            Next
            Return SearchAny
        End Function

        ''' <summary>
        ''' Procura valores nas colunas especificadas do <see cref="Template.Data"/> que contenham
        ''' qualquer um dos termos pesquisados
        ''' </summary>
        ''' <param name="SearchTerms">Termos Pesquisados</param>
        ''' <returns></returns>
        Function SearchAny(SearchTerms As String(), ParamArray Columns As String()) As TemplateList
            SearchAny = New TemplateList
            For Each t As Template In Me
                For Each k In t.Data.Keys.ToArray
                    If k.IsIn(Columns) Then
                        If t.Data(k).ToString.ContainsAny(SearchTerms) And Not t.IsIn(SearchAny) Then
                            SearchAny.Add(t)
                        End If
                    End If
                Next
            Next
            Return SearchAny
        End Function

        ''' <summary>
        ''' Procura valores nas colunas especificadas do <see cref="Template.Data"/> que contenham
        ''' qualquer um dos termos pesquisados usando comparação fonética
        ''' </summary>
        ''' <param name="SearchTerms">Termos Pesquisados</param>
        ''' <returns></returns>
        Function SearchIn(SearchTerms As Phonetic(), ParamArray Columns As String()) As TemplateList
            SearchIn = New TemplateList
            For Each t As Template In Me
                For Each k In t.Data.Keys.ToArray
                    If k.IsIn(Columns) Then
                        For Each p In SearchTerms
                            If Not IsNothing(t.Data(k)) Then
                                If (p.IsListenedIn(t.Data(k).ToString) Or (t.Data(k).ToString.ContainsAny(p.Word))) And Not t.IsIn(SearchIn) Then
                                    SearchIn.Add(t)
                                End If
                            End If
                        Next
                    End If
                Next
            Next
            Return SearchIn
        End Function

        ''' <summary>
        ''' Reordena a lista de acordo com a propriedade <see cref="Template.Order"/>
        ''' </summary>
        ''' <param name="OrderBy">Se TRUE, Ordem crescente</param>
        ''' <returns></returns>
        Function ReOrder(Optional OrderBy As TemplateOrder = 0) As TemplateList
            Select Case OrderBy
                Case TemplateOrder.AscendingOrder, TemplateOrder.DescendingOrder
                    Me.Sort(Function(x, y) If(IsNothing(x), y, x).CompareTo(y))
                Case TemplateOrder.DescendingOrder
                    Me.Reverse()
                Case TemplateOrder.RandomOrder
                    Me.Shuffle
                Case Else
            End Select
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
            If Not IsNothing(Template) Then
                MyBase.Add(Template)
                ReOrder()
            End If
        End Sub

        ''' <summary>
        ''' Concatena o HTML da lista de templates a string
        ''' </summary>
        ''' <param name="Text">        Texto</param>
        ''' <param name="TemplateList">Lista de Template</param>
        ''' <returns></returns>
        Public Shared Operator &(Text As String, TemplateList As TemplateList) As String
            Return Text & TemplateList.ToString
        End Operator

        ''' <summary>
        ''' Adiciona um template á lista
        ''' </summary>
        ''' <param name="List">    Lista</param>
        ''' <param name="Template">Template</param>
        ''' <returns></returns>
        Public Shared Operator +(List As TemplateList, Template As Template) As TemplateList
            Dim l As New TemplateList
            l.Add(Template)
            Return l
        End Operator

        ''' <summary>
        ''' Adiciona os templates de uma lista á outra lista
        ''' </summary>
        ''' <param name="List">       Lista</param>
        ''' <param name="AnotherList">Outra lista</param>
        ''' <returns></returns>
        Public Shared Operator +(List As TemplateList, AnotherList As TemplateList) As TemplateList
            Dim l As New TemplateList
            l.Head.AddRange(List.Head)
            l.Head.AddRange(AnotherList.Head)
            l.AddRange(List)
            l.AddRange(AnotherList)
            Return l
        End Operator

        ''' <summary>
        ''' Concatena o HTML da lista de templates a string
        ''' </summary>
        ''' <param name="Text">        Texto</param>
        ''' <param name="TemplateList">Lista de Template</param>
        ''' <returns></returns>
        Public Shared Operator +(Text As String, TemplateList As TemplateList) As String
            Return Text & TemplateList.ToString
        End Operator

        ''' <summary>
        ''' Concatena o HTML da lista de templates a string
        ''' </summary>
        ''' <param name="Text">        Texto</param>
        ''' <param name="TemplateList">Lista de Template</param>
        ''' <returns></returns>
        Public Shared Operator +(TemplateList As TemplateList, Text As String) As String
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
        ''' Dicionário de onde serão extraidos os valores que definem a ordem de apresentação e o filtro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Data As Dictionary(Of String, Object)

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

        ''' <summary>
        ''' Coluna que define a ordem de apresentação na <see cref="TemplateList"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property OrderByColumn As String

        ''' <summary>
        ''' Valor que define a ordem de apresentação na <see cref="TemplateList"/>
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Order As Object
            Get
                If Not IsNothing(Data) AndAlso Data.ContainsKey(OrderByColumn) Then
                    Return Me.Data(OrderByColumn)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Friend Sub New(TemplateFile As String, Values As Dictionary(Of String, Object), OrderByColumn As String, Content As String, SQLQuery As String)
            Me.Content = Content
            Me.Data = Values
            Me.TemplateFile = TemplateFile
            Me.SQLQuery = SQLQuery
            Me.OrderByColumn = OrderByColumn
        End Sub

        ''' <summary>
        ''' Cria uma nova instrução para contrução de um template
        ''' </summary>
        ''' <param name="SQLQuery">     Query SQL</param>
        ''' <param name="TemplateFile"> Nome do arquivo de template</param>
        ''' <param name="OrderByColumn">Nome da Coluna de Ordem</param>
        Public Sub New(SQLQuery As String, TemplateFile As String, Optional OrderByColumn As String = Nothing)
            Me.SQLQuery = SQLQuery
            Me.TemplateFile = TemplateFile
            Me.Content = Nothing
            Me.OrderByColumn = Me.OrderByColumn
        End Sub

        ''' <summary>
        ''' Metodo de comparação de ordem dos templates
        ''' </summary>
        ''' <param name="Template">Template</param>
        ''' <returns></returns>
        Public Function CompareTo(Template As Object) As Integer Implements IComparable.CompareTo
            If IsNothing(Me.Order) Then Return -1
            If IsNothing(Template) Then Return -1
            If IsNothing(Template.Order) Then Return -1
            If GetType(Template) = Template.GetType Then
                Dim orderToCompare As Template = TryCast(Template, Template)
                If Me.Order.GetType.Equals(orderToCompare.Order.GetType) Then
                    Select Case orderToCompare.Order.GetType
                        Case GetType(String)
                            Return -1 * String.Compare(orderToCompare.Order, Order, True)
                        Case GetType(DateTime)
                            Return -1 * DateTime.Compare(orderToCompare.Order, Order)
                        Case GetType(Integer), GetType(Decimal), GetType(Short), GetType(Long), GetType(Double)
                            Select Case True
                                Case orderToCompare.Order < Order
                                    Return -1
                                Case orderToCompare.Order > Order
                                    Return 1
                                Case Else
                                    Return 0
                            End Select
                        Case GetType(Byte()), GetType(Byte)
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
            Else
                Return -1
            End If
        End Function

        ''' <summary>
        ''' cria uma cópia da <see cref="TemplateList"/> e adiciona o <see cref="Template"/> a ela
        ''' </summary>
        ''' <param name="Template">    Template</param>
        ''' <param name="TemplateList">Lista</param>
        ''' <returns></returns>
        Public Shared Operator +(Template As Template, TemplateList As TemplateList) As TemplateList
            Dim l As New TemplateList
            l.Head = TemplateList.Head
            l.AddRange(TemplateList)
            l.Add(Template)
            Return l
        End Operator

        ''' <summary>
        ''' cria uma cópia da <see cref="TemplateList"/> e adiciona o <see cref="Template"/> a ela
        ''' </summary>
        ''' <param name="Template">    Template</param>
        ''' <param name="TemplateList">Lista</param>
        ''' <returns></returns>
        Public Shared Operator +(TemplateList As TemplateList, Template As Template) As TemplateList
            Return Template + TemplateList
        End Operator

        ''' <summary>
        ''' Verifica se 2 templates processados são parecidos (utilizam o mesmo arquivo de template)
        ''' </summary>
        ''' <param name="Template1"></param>
        ''' <param name="Template2"></param>
        ''' <returns></returns>
        Public Shared Operator Like(Template1 As Template, Template2 As Template) As Boolean
            Return Template1.TemplateFile = Template2.TemplateFile Or Template1.Content = Template2.Content
        End Operator

    End Class

    ''' <summary>
    ''' Define o Comportamento de ordenação dos itens do templatizer
    ''' </summary>
    Public Enum TemplateOrder
        ''' <summary>
        ''' Ordem Default (definida normalmente na clausula "order by" da query SQL)
        ''' </summary>
        DataBaseDefault = 0
        ''' <summary>
        ''' Ordem crescente de acordo com a propriedade <see cref="Template.Order"/>
        ''' </summary>
        AscendingOrder = 1
        ''' <summary>
        ''' Ordem decrescente de acordo com a propriedade <see cref="Template.Order"/>
        ''' </summary>
        DescendingOrder = 2
        ''' <summary>
        ''' Ordem aleatória
        ''' </summary>
        RandomOrder = 3
    End Enum

End Namespace