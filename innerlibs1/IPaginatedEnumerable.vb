Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Linq.Expressions
Imports System.Web

Namespace LINQ

    Public Class PropertyFilter(Of ClassType As Class)

        Friend Sub New(LB As PaginationFilter(Of ClassType))
            _config = LB
        End Sub

        ''' <summary>
        ''' Operador usado nesse filtro
        ''' </summary>
        ''' <returns></returns>
        Public Property [Operator] As String = "="

        ''' <summary>
        ''' Comparara o valor do filtro com TRUE ou FALSE
        ''' </summary>
        ''' <returns></returns>
        Public Property [Is] As Boolean = True

        ''' <summary>
        ''' Valores a serem testados por esse filtro
        ''' </summary>
        ''' <returns></returns>
        Public Property PropertyValues As IEnumerable(Of IComparable)

        ''' <summary>
        ''' Parametro da expressão lambda
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Parameter As ParameterExpression
            Get
                Return _config.Parameter
            End Get
        End Property

        ''' <summary>
        ''' Expressão do membro utilizado no filtro
        ''' </summary>
        ''' <returns></returns>
        Property Member As MemberExpression

        ''' <summary>
        ''' retorna o lambdafilter deste Filtro
        ''' </summary>
        ''' <returns></returns>
        Function LambdaFilter() As PaginationFilter(Of ClassType)
            Return Me._config
        End Function



        ''' <summary>
        ''' Seta varios valores para esse filtro testar. Substitui os valores antigos
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns></returns>
        Function SetValues(Of T As IComparable)(ParamArray Values As T()) As PropertyFilter(Of ClassType)
            PropertyValues = If(Values, {})
            Return Me
        End Function

        ''' <summary>
        ''' Seta varios valores para esse filtro testar. Substitui os valores antigos
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns></returns>
        Function SetValues(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            PropertyValues = If(Values, {})
            Return Me
        End Function

        ''' <summary>
        ''' Adciona varios valores para esse filtro testar.
        ''' </summary>
        ''' <param name="Values"></param>
        ''' <returns></returns>
        Function AddValues(Of T As IComparable)(ParamArray Values As T()) As PropertyFilter(Of ClassType)
            PropertyValues = If(PropertyValues, {}).Union(If(Values, {}))
            Return Me
        End Function

        ''' <summary>
        ''' Seta um unico valor para esse filtro testar. Substitui os antigos
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <returns></returns>
        Function SetValue(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            PropertyValues = {Value}
            Return Me
        End Function

        ''' <summary>
        ''' Sete um membro para ser utilizado neste filtro. É ignorado quando seus Values estão nulos ou vazios
        ''' </summary>
        ''' <param name="PropertySelector"></param>
        ''' <returns></returns>
        Function SetMember(Of T)(PropertySelector As Expression(Of Func(Of ClassType, T))) As PropertyFilter(Of ClassType)
            Return SetMember(PropertySelector.Body.ToString().Split(".").Skip(1).Join("."))
        End Function

        ''' <summary>
        ''' Sete um membro para ser utilizado neste filtro. É ignorado quando seus Values estão nulos ou vazios
        ''' </summary>
        ''' <param name="PropertyName"></param>
        ''' <returns></returns>
        Function SetMember(PropertyName As String) As PropertyFilter(Of ClassType)
            Dim prop As Expression = Parameter
            For Each name In PropertyName.SplitAny(".", "/")
                prop = Expression.[Property](prop, name)
            Next
            Member = prop
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador utilizado nesse filtro
        ''' </summary>
        ''' <param name="[Operator]"></param>
        ''' <returns></returns>
        Function SetOperator([Operator] As String) As PropertyFilter(Of ClassType)
            Me.Operator = [Operator]
            Return Me
        End Function

        ''' <summary>
        ''' Seta o comparador (TRUE/FALSE) para o resultado do filtro. Quando false, o resultado do filtro é negado
        ''' </summary>
        ''' <param name="TrueOrFalse"></param>
        ''' <returns></returns>
        Function CompareWith(TrueOrFalse As Boolean) As PropertyFilter(Of ClassType)
            Me.Is = TrueOrFalse
            Return Me
        End Function

        ''' <summary>
        ''' Seta TRUE para o comparador  para o resultado do filtro. Quando false, o resultado do filtro é negado
        ''' </summary>
        ''' <returns></returns>
        Function CompareTrue() As PropertyFilter(Of ClassType)
            Return CompareWith(True)
        End Function

        ''' <summary>
        ''' Seta FALSE para o comparador  para o resultado do filtro. Quando false, o resultado do filtro é negado
        ''' </summary>
        ''' <returns></returns>
        Function CompareFalse() As PropertyFilter(Of ClassType)
            Return CompareWith(False)
        End Function

        ''' <summary>
        ''' Seta o operador para Contains e o Valor para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function Contains(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("contains")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para Contains e os Valores para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function Contains(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("contains")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para = e o Valor para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function Equal(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("=")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para = e os Valores para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function Equal(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("=")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para > e o Valor para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function GreaterThan(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator(">")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para > e os Valores para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function GreaterThan(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator(">")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para &lt; e o Valor para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function LessThan(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("<")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para   &lt; e os Valores para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function LessThan(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("<")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para  >= e o Valor para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function GreaterThanOrEqual(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator(">=")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para  >= e os Valores para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function GreaterThanOrEqual(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator(">=")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para   &lt;= e o Valor para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function LessThanOrEqual(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("<=")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para   &lt; e os Valores para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function LessThanOrEqual(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("<=")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para  != e o Valor para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function NotEqual(Of T As IComparable)(Value As T) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("<>")
            Return Me
        End Function

        ''' <summary>
        ''' Seta o operador para  != e os Valores para este filtro
        ''' </summary>
        ''' <returns></returns>
        Function NotEqual(Of T As IComparable)(Values As IEnumerable(Of T)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("<>")
            Return Me
        End Function

        ''' <summary>
        ''' Expressão binaria deste filtro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Filter As BinaryExpression
            Get
                Dim exp As Expression = Nothing
                Dim v = If(PropertyValues, {})
                If Not UseNullValues Then
                    v = v.Where(Function(x) x IsNot Nothing)
                End If

                Return GetOperatorExpression(Member, [Operator], v, False)
            End Get
        End Property

        ''' <summary>
        ''' Configura este filtro para utilização de valores nulos na query
        ''' </summary>
        ''' <returns></returns>
        Property UseNullValues As Boolean = False

        ''' <summary>
        ''' Permite que valores nulos sejam adcionados ao filtro
        ''' </summary>
        ''' <returns></returns>
        Function AllowNull() As PropertyFilter(Of ClassType)
            UseNullValues = True
            Return Me
        End Function

        ''' <summary>
        ''' Impede que valores nulos sejam adcionados ao filtro
        ''' </summary>
        ''' <returns></returns>
        Function IgnoreNull() As PropertyFilter(Of ClassType)
            UseNullValues = False
            Return Me
        End Function

        Friend _config As PaginationFilter(Of ClassType)

    End Class

    ''' <summary>
    ''' Classe para criação de filtros dinâmicos para listas de classes
    ''' </summary>
    ''' <typeparam name="ClassType"></typeparam>
    Public Class PaginationFilter(Of ClassType As Class)

        Friend _filters As New List(Of PropertyFilter(Of ClassType))

        ''' <summary>
        ''' Cria uma nova instancia e seta a exclusividade de filtro
        ''' </summary>
        ''' <param name="Exclusive"></param>
        Sub New(Optional Exclusive As Boolean = True)
            Me.Exclusive = Exclusive
        End Sub

        ''' <summary>
        ''' Configura este LambdaFilter
        ''' </summary>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Function Config(options As Action(Of PaginationFilter(Of ClassType))) As PaginationFilter(Of ClassType)
            options(Me)
            Return Me
        End Function

        ''' <summary>
        ''' Configura este LambDafilter para utilizar uma querystring como Filtro.
        ''' </summary>
        ''' <param name="Query">QueryString</param>
        ''' <remarks> Utiliza os names como propriedade e os values como valores do filtro. Propriedade que não existirem na classe serão ignoradas. Valores nulos serão ignorados por padrão</remarks>
        ''' <returns></returns>
        Function UseQueryString(Query As String, Optional CompareWith As Boolean = True, Optional DefaultOperator As String = "=") As PaginationFilter(Of ClassType)
            If Query.IsNotBlank Then
                UseNameValueCollection(HttpUtility.ParseQueryString(Query), CompareWith, DefaultOperator)
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Configura este LambDafilter para utilizar um Dictionary como Filtro.
        ''' </summary>
        ''' <param name="Collection">Collection</param>
        ''' <remarks> Utiliza os names como propriedade e os values como valores do filtro. Propriedade que não existirem na classe serão ignoradas. Valores nulos serão ignorados por padrão</remarks>
        ''' <returns></returns>
        Function UseArrayDictionary(Collection As IDictionary(Of String, IComparable()), Optional CompareWith As Boolean = True, Optional DefaultOperator As String = "=") As PaginationFilter(Of ClassType)
            Collection = If(Collection, New NameValueCollection)
            For Each K In Collection.Keys
                Dim t = GetType(ClassType)
                Dim l = t.GetProperties()
                If l.Any(Function(x) x.Name = K) Then
                    Me.SetMember(K).SetValues(Collection(K).ToArray()).CompareWith(CompareWith).SetOperator(DefaultOperator)
                End If
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Configura este LambDafilter para utilizar um Dictionary como Filtro.
        ''' </summary>
        ''' <param name="Collection">Collection</param>
        ''' <remarks> Utiliza os names como propriedade e os values como valores do filtro. Propriedade que não existirem na classe serão ignoradas. Valores nulos serão ignorados por padrão</remarks>
        ''' <returns></returns>
        Function UseDictionary(Collection As IDictionary(Of String, IComparable), Optional CompareWith As Boolean = True, Optional DefaultOperator As String = "=") As PaginationFilter(Of ClassType)
            Collection = If(Collection, New NameValueCollection)
            For Each K In Collection.Keys
                Dim t = GetType(ClassType)
                Dim l = t.GetProperties()
                If l.Any(Function(x) x.Name = K) Then
                    Me.SetMember(K).SetValue(Collection(K)).CompareWith(CompareWith).SetOperator(DefaultOperator)
                End If
            Next
            Return Me
        End Function

        Public Function IsExclusive(Optional Exclusive As Boolean = True) As PaginationFilter(Of ClassType)
            Me.Exclusive = Exclusive
            Return Me
        End Function

        Public Function SetData(List As IEnumerable(Of ClassType)) As PaginationFilter(Of ClassType)
            Data = List
            Return Me
        End Function

        Public Function UseNameValueCollection(Collection As NameValueCollection, Optional CompareWith As Boolean = True, Optional DefaultOperator As String = "=") As PaginationFilter(Of ClassType)
            Collection = If(Collection, New NameValueCollection)
            For Each K In Collection.AllKeys
                Dim t = GetType(ClassType)
                Dim l = t.GetProperties()
                If l.Any(Function(x) x.Name = K) Then
                    If Collection(K).IsNotBlank() AndAlso Collection.GetValues(K).Any() Then
                        Me.SetMember(K).SetValues(Collection.GetValues(K)).CompareWith(CompareWith).SetOperator(DefaultOperator)
                    End If
                End If
            Next
            Return Me
        End Function

        Public Property Data As IEnumerable(Of ClassType)

        Default ReadOnly Property Page(PageNumber As Integer) As PaginationInfo(Of ClassType)
            Get
                Return GetPage(PageNumber)
            End Get
        End Property

        Public Function GetPage(PageNumber As Integer) As PaginationInfo(Of ClassType)
            Return CreatePaginationInfo(PageNumber, PageSize, PaginationOffset)
        End Function

        Public Property PageSize As Integer = 10
        Public Property PaginationOffset As Integer = 3

        Public ReadOnly Property Filters As IEnumerable(Of PropertyFilter(Of ClassType))
            Get
                Return _filters
            End Get

        End Property

        Friend param As ParameterExpression = Expression.Parameter(GetType(ClassType), GetType(ClassType).Name.CamelSplit.SelectJoin(Function(x) x.FirstOrDefault().IfBlank(Of Char)(""), "").ToLower())

        ''' <summary>
        ''' Parametro utilizado na contrução da expressão lambda
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Parameter As ParameterExpression
            Get
                Return param
            End Get
        End Property

        ''' <summary>
        ''' Indica se os filtros são exclusivos.
        ''' </summary>
        ''' <remarks>Valores exclusivos precisam cumprir todos os filtros para retornarem na busca. Filtros não exclusivos retornam se um ou mais forem cumpridos</remarks>
        ''' <returns></returns>
        Public Property Exclusive As Boolean = True

        ''' <summary>
        ''' Expressão binária contendo todos os filtros
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Filter As BinaryExpression
            Get
                Dim exp As Expression = Nothing
                For Each valor In Filters.Select(Function(x) x.Filter)
                    If valor IsNot Nothing Then
                        If exp Is Nothing Then
                            exp = valor
                        Else
                            If Exclusive Then
                                exp = Expression.AndAlso(valor, exp)
                            Else
                                exp = Expression.OrElse(valor, exp)
                            End If
                        End If
                    End If
                Next

                Return exp
            End Get
        End Property

        Function SetPagination(PageSize As Integer, PaginationOffset As Integer) As PaginationFilter(Of ClassType)
            Me.PageSize = PageSize
            Me.PaginationOffset = PaginationOffset
            Return Me
        End Function

        Function SetPagination(PageSize As Integer) As PaginationFilter(Of ClassType)
            Me.PageSize = PageSize
            Return Me
        End Function

        ''' <summary>
        ''' Configura um novo membro para este filtro
        ''' </summary>
        ''' <param name="PropertyName"></param>
        ''' <returns></returns>
        Function SetMember(Of T)(PropertyName As Expression(Of Func(Of ClassType, T))) As PropertyFilter(Of ClassType)
            Dim f = New PropertyFilter(Of ClassType)(Me)
            f.SetMember(PropertyName)
            _filters.Add(f)
            Return f
        End Function

        ''' <summary>
        ''' Configura um novo membro para este filtro
        ''' </summary>
        ''' <param name="PropertyName"></param>
        ''' <returns></returns>
        Function SetMember(PropertyName As String) As PropertyFilter(Of ClassType)
            Dim f = New PropertyFilter(Of ClassType)(Me)
            f.SetMember(PropertyName)
            _filters.Add(f)
            Return f
        End Function

        ''' <summary>
        ''' Retorna os dados de <see cref="Data"/> filtrados pelo <see cref="Filter"/>
        ''' </summary>
        ''' <returns></returns>
        Public Function Apply() As IEnumerable(Of ClassType)
            If LambdaExpression IsNot Nothing Then
                If TypeOf Me.Data Is IQueryable(Of ClassType) Then
                    Dim d = CType(Me.Data, IQueryable(Of ClassType)).Where(LambdaExpression)
                    Return d
                Else
                    Return Me.Data.Where(LambdaExpression.Compile())
                End If
            End If
            Return Me.Data
        End Function

        ''' <summary>
        ''' Expressão lambda deste filtro
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property LambdaExpression As Expression(Of Func(Of ClassType, Boolean))
            Get
                Dim exp As Expression(Of Func(Of ClassType, Boolean)) = Nothing
                If Filter IsNot Nothing Then
                    exp = Expression.Lambda(Of Func(Of ClassType, Boolean))(Filter, param)
                    While exp.CanReduce
                        exp = exp.Reduce()
                    End While
                End If
                Return exp
            End Get
        End Property

        ''' <summary>
        ''' Cria uma pagina com os resultados deste filtro
        ''' </summary>
        ''' <param name="PageNumber"></param>
        ''' <param name="PageSize"></param>
        ''' <param name="PaginationOffset"></param>
        ''' <returns></returns>
        Public Function CreatePaginationInfo(PageNumber As Integer, PageSize As Integer, Optional PaginationOffset As Integer = 3) As PaginationInfo(Of ClassType)
            Return CreatePaginationInfo(Me.Data, PageNumber, PageSize, PaginationOffset)
        End Function

        ''' <summary>
        ''' Cria uma pagina com os resultados deste filtro usando uma outra lista
        ''' </summary>
        ''' <param name="PageNumber"></param>
        ''' <param name="PageSize"></param>
        ''' <param name="PaginationOffset"></param>
        ''' <returns></returns>
        Public Function CreatePaginationInfo(List As IEnumerable(Of ClassType), PageNumber As Integer, PageSize As Integer, Optional PaginationOffset As Integer = 3) As PaginationInfo(Of ClassType)
            Return New PaginationInfo(Of ClassType)(List, PageNumber, PageSize, PaginationOffset, LambdaExpression)
        End Function

    End Class

    ''' <summary>
    ''' Classe para manipulação de coleções de forma paginada
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class PaginationInfo(Of T)
        Inherits ReadOnlyCollection(Of T)
        Implements IEnumerable, IEnumerable(Of T)

        ''' <summary>
        ''' Filtro aplicado a lista
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Filter As Expression(Of Func(Of T, Boolean))

        ''' <summary>
        ''' Numero desta pagina
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PageNumber As Integer = 1

        ''' <summary>
        ''' Numero da ultima pagina
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property LastPage As Integer
            Get
                Return PageCount
            End Get
        End Property

        ''' <summary>
        ''' Numero da primeira pagina
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property FirstPage As Integer
            Get
                Return 1
            End Get
        End Property

        ''' <summary>
        ''' Numero da proxima pagina
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property NextPage As Integer
            Get
                Dim pp = PageNumber + 1
                If pp > LastPage Then
                    pp = FirstPage
                End If
                Return pp
            End Get
        End Property

        ''' <summary>
        ''' Numero da pagina anterior
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PreviousPage As Integer
            Get
                Dim pp = PageNumber - 1
                If pp < 1 Then
                    pp = LastPage
                End If
                Return pp
            End Get
        End Property

        ''' <summary>
        ''' Retorna true se esta pagina é a primeira
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsFirstPage As Boolean
            Get
                Return PageNumber = FirstPage
            End Get
        End Property

        ''' <summary>
        ''' Retorna true se esta pagina é a ultima
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsLastPage As Boolean
            Get
                Return PageNumber = LastPage
            End Get
        End Property

        ''' <summary>
        ''' Retorna true se existir mais de uma pagina
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsNecessary As Boolean
            Get
                Return PageCount > 1
            End Get
        End Property

        ''' <summary>
        ''' Retorna true se existir o botão de primeira página for necessário
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsFirstPageNecessary As Boolean
            Get
                Return PageNumber > FirstPage
            End Get
        End Property

        ''' <summary>
        ''' Retorna true se existir o botão de primeira página for necessário
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsLastPageNecessary As Boolean
            Get
                Return PageNumber < LastPage
            End Get
        End Property

        ''' <summary>
        ''' Retorna true se existir o botão de pagina anterior for necessário
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsPreviousPageNecessary As Boolean
            Get
                Return IsFirstPageNecessary
            End Get
        End Property

        ''' <summary>
        ''' Retorna true se existir o botão de proxima pagina for necessário
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property IsNextPageNecessary As Boolean
            Get
                Return IsLastPageNecessary
            End Get
        End Property

        ''' <summary>
        ''' Total de itens da Lista
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property Total As Integer

        ''' <summary>
        ''' Quantidade de itens por página
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PageSize As Integer

        ''' <summary>
        ''' Quantidade de páginas
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PageCount As Integer
            Get
                Return (Total / PageSize).Ceil
            End Get
        End Property

        Sub New(List As IEnumerable(Of T), PageNumber As Integer, PageSize As Integer, Optional PaginationOffset As Integer = 3, Optional Filter As Expression(Of Func(Of T, Boolean)) = Nothing)
            MyBase.New(MakeList(List, PageNumber, PageSize, PaginationOffset, Filter))
            Me.PageNumber = PageNumber
            Me.PageSize = PageSize
            Me.Filter = Filter
            Me.PaginationOffset = PaginationOffset
            Me.Total = FilterList(List, Filter).Count()
        End Sub

        Private Shared Function MakeList(List As IEnumerable(Of T), PageNumber As Integer, PageSize As Integer, PaginationOffset As Integer, Filter As Expression(Of Func(Of T, Boolean))) As IEnumerable(Of T)
            Return PageList(FilterList(List, Filter), PageNumber, PageSize).ToList()
        End Function

        Private Shared Function FilterList(List As IEnumerable(Of T), Filter As Expression(Of Func(Of T, Boolean))) As IEnumerable(Of T)
            If Filter IsNot Nothing Then
                If TypeOf List Is IQueryable(Of T) Then
                    Dim d = CType(List, IQueryable(Of T)).Where(Filter)
                    Return d
                Else
                    Return List.Where(Filter.Compile())
                End If
            End If
            Return List
        End Function

        Private Shared Function PageList(List As IEnumerable(Of T), PageNumber As Integer, PageSize As Integer) As IEnumerable(Of T)
            If PageNumber > 0 AndAlso PageSize > 0 Then
                List = List.Skip((PageNumber - 1) * PageSize).Take(PageSize)
            End If
            Return List
        End Function

        Sub New(List As IEnumerable(Of T), PageNumber As Integer, PageSize As Integer, Total As Integer, Optional PaginationOffset As Integer = 3)
            MyBase.New(List)
            Me.PageNumber = PageNumber
            Me.PageSize = PageSize
            Me.Total = Total
            Me.PaginationOffset = PaginationOffset
        End Sub

        ''' <summary>
        ''' Quantidade média de "botões de paginação" contidas no <see cref="PageRange"/>
        ''' </summary>
        ''' <returns></returns>
        Property PaginationOffset As Integer = 3

        ''' <summary>
        ''' Retorna um range de páginas a partir da pagina atual
        ''' </summary>
        ''' <returns></returns>
        ReadOnly Property PageRange As Integer()
            Get
                Dim frange = 1
                Dim lrange = 1
                If PageCount > 1 Then
                    Dim midrange = Math.Ceiling(PaginationOffset / 2)
                    frange = {(PageNumber - midrange), 1}.Max()
                    lrange = {(PageNumber + midrange), PageCount}.Min()
                End If
                Dim arr = New List(Of Integer)
                For index = frange To lrange
                    arr.Add(index)
                Next
                Return arr.ToArray
            End Get
        End Property

        Public Function IsCurrentPage(Index As Integer) As Boolean
            Return Index = PageNumber
        End Function

    End Class

End Namespace