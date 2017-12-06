Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices

Public Module LINQExtensions

    <Extension()> Function Page(Of TSource)(ByVal source As IQueryable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IQueryable(Of TSource)
        Return source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
    End Function


    <Extension()>
    Function Page(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IEnumerable(Of TSource)
        Return source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
    End Function

End Module



