Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.Linq.Expressions
Imports System.Web

Namespace LINQ

    Public Class PropertyFilter(Of ClassType As Class)

        Friend Sub New(LB As LambdaFilter(Of ClassType))
            _config = LB
        End Sub

        Public Property [Operator] As String = "="
        Public Property [Is] As Boolean = True



        Public Property PropertyValues As IEnumerable(Of IComparable)

        Public ReadOnly Property Parameter As ParameterExpression
            Get
                Return _config.Parameter
            End Get
        End Property

        Property Member As MemberExpression

        Function LambdaFilter() As LambdaFilter(Of ClassType)
            Return Me._config
        End Function

        Function SetValues(ParamArray Values As IComparable()) As PropertyFilter(Of ClassType)
            PropertyValues = If(Values, {})
            Return Me
        End Function

        Function AddValues(ParamArray Values As IComparable()) As PropertyFilter(Of ClassType)
            PropertyValues = If(PropertyValues, {}).Union(If(Values, {}))
            Return Me
        End Function

        Function SetValue(Value As IComparable) As PropertyFilter(Of ClassType)
            PropertyValues = {Value}
            Return Me
        End Function

        Function SetMember(PropertySelector As Expression(Of Func(Of ClassType, IComparable))) As PropertyFilter(Of ClassType)
            Return SetMember(PropertySelector.Body.ToString().Split(".").Skip(1).Join("."))
        End Function

        Function SetMember(PropertyName As String) As PropertyFilter(Of ClassType)
            Dim prop As Expression = Parameter
            For Each name In PropertyName.SplitAny(".", "/")
                prop = Expression.[Property](prop, name)
            Next
            Member = prop
            Return Me
        End Function

        Function SetOperator([Operator] As String) As PropertyFilter(Of ClassType)
            Me.Operator = [Operator]
            Return Me
        End Function

        Function CompareWith(TrueOrFalse As Boolean) As PropertyFilter(Of ClassType)
            Me.Is = TrueOrFalse
            Return Me
        End Function

        Function CompareTrue() As PropertyFilter(Of ClassType)
            Return CompareWith(True)
        End Function

        Function CompareFalse() As PropertyFilter(Of ClassType)
            Return CompareWith(False)
        End Function

        Function Contains(Value As IComparable) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("contains")
            Return Me
        End Function
        Function Contains(Values As IEnumerable(Of IComparable)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("contains")
            Return Me
        End Function

        Function Equal(Value As IComparable) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("=")
            Return Me
        End Function
        Function Equal(Values As IEnumerable(Of IComparable)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("=")
            Return Me
        End Function

        Function GreaterThan(Value As IComparable) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator(">")
            Return Me
        End Function
        Function GreaterThan(Values As IEnumerable(Of IComparable)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator(">")
            Return Me
        End Function

        Function LessThan(Value As IComparable) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("<")
            Return Me
        End Function

        Function LessThan(Values As IEnumerable(Of IComparable)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("<")
            Return Me
        End Function

        Function GreaterThanOrEqual(Value As IComparable) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator(">=")
            Return Me
        End Function

        Function GreaterThanOrEqual(Values As IEnumerable(Of IComparable)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator(">=")
            Return Me
        End Function

        Function LessThanOrEqual(Value As IComparable) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("<=")
            Return Me
        End Function

        Function LessThanOrEqual(Values As IEnumerable(Of IComparable)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("<=")
            Return Me
        End Function

        Function NotEqual(Value As IComparable) As PropertyFilter(Of ClassType)
            Me.SetValue(Value)
            Me.SetOperator("<>")
            Return Me
        End Function

        Function NotEqual(Values As IEnumerable(Of IComparable)) As PropertyFilter(Of ClassType)
            Me.SetValues(Values)
            Me.SetOperator("<>")
            Return Me
        End Function


        ReadOnly Property Filter As BinaryExpression
            Get
                Dim exp As Expression = Nothing
                For Each valor In If(PropertyValues, {})
                    Dim corpo = GetOperatorExpression(Of ClassType)(Member, [Operator], valor)

                    corpo = Expression.Equal(corpo, Expression.Constant([Is]))

                    If exp Is Nothing Then
                        exp = corpo
                    Else
                        exp = Expression.OrElse(exp, corpo)
                    End If
                Next
                Return exp
            End Get
        End Property

        Friend _config As LambdaFilter(Of ClassType)

    End Class

    Public Class LambdaFilter(Of ClassType As Class)
        Friend _filters As New List(Of PropertyFilter(Of ClassType))

        Sub New(Optional Exclusive As Boolean = True)
            Me.Exclusive = Exclusive
        End Sub

        Sub New(options As Action(Of LambdaFilter(Of ClassType)))
            Config(options)
        End Sub

        Function Config(options As Action(Of LambdaFilter(Of ClassType))) As LambdaFilter(Of ClassType)
            options(Me)
            Return Me
        End Function

        Sub New(Collection As IDictionary(Of String, IComparable), Optional Exclusive As Boolean = True, Optional DefaultOperator As String = "=", Optional CompareWith As Boolean = True)
            Collection = If(Collection, New NameValueCollection)
            For Each K In Collection.Keys
                If GetType(ClassType).HasProperty(K) Then
                    Me.SetMember(K).SetValue(Collection(K)).CompareWith(CompareWith).SetOperator(DefaultOperator)
                End If
            Next
        End Sub

        Function UseQueryString(Query As String) As LambdaFilter(Of ClassType)
            If Query.IsNotBlank Then
                UseNameValueCollection(HttpUtility.ParseQueryString(Query))
            End If
            Return Me
        End Function

        Function UseArrayDictionary(Collection As IDictionary(Of String, IComparable()), Optional CompareWith As Boolean = True, Optional DefaultOperator As String = "=") As LambdaFilter(Of ClassType)
            Collection = If(Collection, New NameValueCollection)
            For Each K In Collection.Keys
                If GetType(ClassType).HasProperty(K) Then
                    Me.SetMember(K).SetValues(Collection(K).ToArray()).CompareWith(CompareWith).SetOperator(DefaultOperator)
                End If
            Next
            Return Me
        End Function

        Function UseDictionary(Collection As IDictionary(Of String, IComparable), Optional CompareWith As Boolean = True, Optional DefaultOperator As String = "=") As LambdaFilter(Of ClassType)
            Collection = If(Collection, New NameValueCollection)
            For Each K In Collection.Keys
                If GetType(ClassType).HasProperty(K) Then
                    Me.SetMember(K).SetValue(Collection(K)).CompareWith(CompareWith).SetOperator(DefaultOperator)
                End If
            Next
            Return Me
        End Function


        Public Function IsExclusive(Optional Exclusive As Boolean = True) As LambdaFilter(Of ClassType)
            Me.Exclusive = Exclusive
            Return Me
        End Function

        Public Function SetData(List As IEnumerable(Of ClassType)) As LambdaFilter(Of ClassType)
            Data = List
            Return Me
        End Function

        Public Function UseNameValueCollection(Collection As NameValueCollection, Optional CompareWith As Boolean = True, Optional DefaultOperator As String = "=") As LambdaFilter(Of ClassType)
            Collection = If(Collection, New NameValueCollection)
            For Each K In Collection.AllKeys
                If GetType(ClassType).HasProperty(K) Then
                    Me.SetMember(K).SetValues(Collection.GetValues(K)).CompareWith(CompareWith).SetOperator(DefaultOperator)
                End If
            Next
            Return Me
        End Function

        Public Property Data As IEnumerable(Of ClassType)

        Public ReadOnly Property Filters As IEnumerable(Of PropertyFilter(Of ClassType))
            Get
                Return _filters
            End Get

        End Property

        Friend param As ParameterExpression = Expression.Parameter(GetType(ClassType), GetType(ClassType).Name.ToLower())
        Public ReadOnly Property Parameter As ParameterExpression
            Get
                Return param
            End Get
        End Property

        Public Property Exclusive As Boolean = True

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



        Function SetMember(PropertyName As Expression(Of Func(Of ClassType, IComparable))) As PropertyFilter(Of ClassType)
            Dim f = New PropertyFilter(Of ClassType)(Me)
            f.SetMember(PropertyName)
            _filters.Add(f)
            Return f
        End Function

        Function SetMember(PropertyName As String) As PropertyFilter(Of ClassType)
            Dim f = New PropertyFilter(Of ClassType)(Me)
            f.SetMember(PropertyName)
            _filters.Add(f)
            Return f
        End Function

        Public Function CreatePaginationInfo(PageNumber As Integer, PageSize As Integer, Optional PaginationOffset As Integer = 3) As PaginationInfo(Of ClassType)
            Return CreatePaginationInfo(Me.Data, PageNumber, PageSize, PaginationOffset)
        End Function

        Public Function CreatePaginationInfo(List As IEnumerable(Of ClassType), PageNumber As Integer, PageSize As Integer, Optional PaginationOffset As Integer = 3) As PaginationInfo(Of ClassType)
            Dim exp As Expression(Of Func(Of ClassType, Boolean)) = Nothing
            If Filter IsNot Nothing Then
                exp = Expression.Lambda(Of Func(Of ClassType, Boolean))(Filter, param)
                While exp.CanReduce
                    exp = exp.Reduce()
                End While
            End If
            Return New PaginationInfo(Of ClassType)(List, PageNumber, PageSize, PaginationOffset, exp)
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