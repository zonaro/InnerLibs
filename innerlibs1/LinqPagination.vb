Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices

Namespace LINQ

    Public Module LINQExtensions

        <Extension()> Function Page(Of TSource)(ByVal source As IQueryable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IQueryable(Of TSource)
            Return source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
        End Function


        <Extension()>
        Function Page(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IEnumerable(Of TSource)
            Return source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
        End Function

        Function CreateExpression(Of T)(Optional DefaultReturnValue As Boolean = False) As Expression(Of Func(Of T, Boolean))
            If DefaultReturnValue Then
                Return Function(f) True
            Else
                Return Function(f) False
            End If
        End Function

        <Extension()>
        Function [Or](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
            Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
            Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[OrElse](expr1.Body, invokedExpr), expr1.Parameters)
        End Function

        <Extension()>
        Function [And](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
            Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
            Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[AndAlso](expr1.Body, invokedExpr), expr1.Parameters)
        End Function
    End Module

End Namespace



