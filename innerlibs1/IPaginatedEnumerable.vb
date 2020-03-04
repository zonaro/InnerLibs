
Imports InnerLibs.LINQ

''' <summary>
''' Classe para manipulaçcao de coleções de forma paginada
''' </summary>
''' <typeparam name="T"></typeparam>
''' <typeparam name="ListType"></typeparam>
Public Class PaginationInfo(Of T, ListType As IEnumerable(Of T))

    ReadOnly Property Data As ListType
    ReadOnly Property PageNumber As Integer
    ReadOnly Property Total As Integer

    ReadOnly Property PageSize As Integer


    ReadOnly Property PageCount As Integer
        Get
            Return (Total / PageSize).Ceil
        End Get
    End Property

    Sub New(ByVal List As IQueryable(Of T), PageNumber As Integer, PageSize As Integer)
        Me.Total = List.Count()
        Me.PageNumber = PageNumber
        Me.PageSize = PageSize
        Data = List.Page(PageNumber, PageSize)
    End Sub

    Sub New(ByVal List As IEnumerable(Of T), PageNumber As Integer, PageSize As Integer)
        Me.Total = List.Count()
        Me.PageNumber = PageNumber
        Me.PageSize = PageSize
        Data = List.Page(PageNumber, PageSize)
    End Sub
End Class



