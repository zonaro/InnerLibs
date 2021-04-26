

''' <summary>
''' Classe para manipulação de coleções de forma paginada
''' </summary>
''' <typeparam name="T"></typeparam>
''' <typeparam name="ListType"></typeparam>
Public Class PaginationInfo(Of T, ListType As IEnumerable(Of T))



    ''' <summary>
    ''' Filtro aplicado previaménte em lista. Guarda qualquer objeto
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Filter As Object

    ''' <summary>
    ''' Registros desta pagina
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property Data As ListType
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

    Sub New(List As IQueryable(Of T), PageNumber As Integer, PageSize As Integer, Optional PaginationOffset As Integer = 3, Optional Filter As Object = Nothing)
        Me.Total = List.Count()
        Me.PageNumber = PageNumber
        Me.PageSize = PageSize
        Me.Filter = Filter
        Me.PaginationOffset = PaginationOffset
        If PageNumber > 0 AndAlso PageSize > 0 Then
            Me.Data = List.Skip((Me.PageNumber - 1) * Me.PageSize).Take(Me.PageSize)
        Else
            Me.Data = List
        End If
    End Sub

    Sub New(List As IEnumerable(Of T), PageNumber As Integer, PageSize As Integer, Optional PaginationOffset As Integer = 3, Optional Filter As Object = Nothing)
        Me.Total = List.Count()
        Me.PageNumber = PageNumber
        Me.PageSize = PageSize
        Me.Filter = Filter
        Me.PaginationOffset = PaginationOffset
        If PageNumber > 0 AndAlso PageSize > 0 Then
            Me.Data = List.Skip((Me.PageNumber - 1) * Me.PageSize).Take(Me.PageSize)
        Else
            Me.Data = List
        End If
    End Sub


    Sub New(List As IEnumerable(Of T), PageNumber As Integer, PageSize As Integer, Total As Integer, Optional PaginationOffset As Integer = 3, Optional Filter As Object = Nothing)
        Me.Data = List
        Me.PageNumber = PageNumber
        Me.PageSize = PageSize
        Me.Total = Total
        Me.PaginationOffset = PaginationOffset
        Me.Filter = Filter
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
    ReadOnly Property PageRange As IEnumerable(Of Integer)
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

