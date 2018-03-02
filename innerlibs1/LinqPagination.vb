Imports System.Data.Linq
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





        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com sua chave primaria. Pode opcionalmente criar o objeto se o mesmo não existir
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="context">Datacontext utilizado para conexão com o banco de dados</param>
        ''' <param name="ID">Valor da chave primária</param>
        ''' <param name="CreateIfNotExists">Se true, cria o objeto e coloca o status de INSERT pendente para este</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKey(Of T As Class)(ByVal Context As DataContext, ByVal ID As Object, Optional CreateIfNotExists As Boolean = False) As T
            Dim table = Context.GetTable(Of T)()
            Dim mapping = Context.Mapping.GetTable(GetType(T))
            Dim pkfield = mapping.RowType.DataMembers.SingleOrDefault(Function(d) d.IsPrimaryKey)
            If pkfield Is Nothing Then Throw New Exception(String.Format("Table {0} does not contain a Primary Key field", mapping.TableName))
            Dim param = Expression.Parameter(GetType(T), "e")
            Dim predicate = Expression.Lambda(Of Func(Of T, Boolean))(Expression.Equal(Expression.[Property](param, pkfield.Name), Expression.Constant(ID)), param)
            Dim obj = table.SingleOrDefault(predicate)
            If obj Is Nothing AndAlso CreateIfNotExists Then
                obj = Activator.CreateInstance(Of T)
                Context.GetTable(Of T).InsertOnSubmit(obj)
            End If
            Return obj
        End Function


    End Module

End Namespace



