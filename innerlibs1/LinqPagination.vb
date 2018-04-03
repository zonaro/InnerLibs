Imports System.Data.Linq
Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices
Imports System.Web
Imports System.Web.UI.HtmlControls

Namespace LINQ

    Public Module LINQExtensions

        <Extension()> Function Page(Of TSource)(ByVal source As IQueryable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IQueryable(Of TSource)
            Return source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
        End Function


        <Extension()>
        Function Page(Of TSource)(ByVal source As IEnumerable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IEnumerable(Of TSource)
            Return source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
        End Function

        Function CreateExpression(Of T)(Optional DefaultReturnValue As Boolean = True) As Expression(Of Func(Of T, Boolean))
            Return Function(f) DefaultReturnValue
        End Function

        Function CreateExpression(Of T)(predicate As Func(Of T, Boolean)) As Expression(Of Func(Of T, Boolean))
            Return Function(f) predicate(f)
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
        Public Function GetByPrimaryKey(Of T As Class, PKType As Structure)(ByVal Context As DataContext, ByVal ID As PKType, Optional CreateIfNotExists As Boolean = False) As T
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

        ''' <summary>
        ''' Atualiza um objeto de entidade a partir de valores em um Dictionary
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <typeparam name="PKType"></typeparam>
        ''' <param name="Context"></param>
        ''' <param name="Dic"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function UpdateObjectFromDictionary(Of T As Class, PKType As Structure)(ByVal Context As DataContext, Dic As IDictionary(Of String, Object)) As T
            Dim table = Context.GetTable(Of T)()
            Dim mapping = Context.Mapping.GetTable(GetType(T))
            Dim pkfield = mapping.RowType.DataMembers.SingleOrDefault(Function(d) d.IsPrimaryKey)
            If pkfield Is Nothing Then Throw New Exception(String.Format("Table {0} does not contain a Primary Key field", mapping.TableName))
            Dim ID As PKType = CType(Dic(pkfield.Name), PKType)
            Dim param = Expression.Parameter(GetType(T), "e")
            Dim predicate = Expression.Lambda(Of Func(Of T, Boolean))(Expression.Equal(Expression.[Property](param, pkfield.Name), Expression.Constant(ID)), param)
            Dim obj = table.SingleOrDefault(predicate)
            If obj Is Nothing Then
                obj = Activator.CreateInstance(Of T)
                Context.GetTable(Of T).InsertOnSubmit(obj)
            End If
            Dic.SetPropertiesIn(obj)
            Return obj
        End Function

        ''' <summary>
        ''' Atualiza um objeto de entidade a partir de valores em um HttpRequest
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <typeparam name="PKType"></typeparam>
        ''' <param name="Context"></param>
        ''' <param name="Request"></param>
        ''' <param name="Keys"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function UpdateObjectFromRequest(Of T As Class, PKType As Structure)(ByVal Context As DataContext, Request As HttpRequest, ParamArray Keys As String()) As T
            Return Context.UpdateObjectFromDictionary(Of T, PKType)(Request.ToDictionary(Keys))
        End Function



        ''' <summary>
        ''' Aplica os valores encontrados nas propriedades de uma entidade em controles
        ''' com mesmo ID das colunas. Se os conroles não existirem no resultado eles serão ignorados.
        ''' </summary>
        ''' <param name="Controls">Controles que serão Manipulados</param>
        ''' <returns>Um array contendo os inputs manipulados</returns>
        <Extension()> Public Function ApplyToControls(Of T As Class)(Obj As T, ParamArray Controls As System.Web.UI.HtmlControls.HtmlControl()) As System.Web.UI.HtmlControls.HtmlControl()
            For Each c In Controls
                Try
                    Select Case c.TagName.ToLower
                        Case "input"
                            CType(c, HtmlInputControl).Value = Obj.GetPropertyValue(c.ID)
                        Case "select"
                            CType(c, HtmlSelect).SelectValues(Obj.GetPropertyValue(c.ID).ToString)
                        Case Else
                            CType(c, HtmlContainerControl).InnerHtml = Obj.GetPropertyValue(c.ID)
                    End Select
                Catch ex As Exception
                End Try
            Next
            Return Controls
        End Function

    End Module

End Namespace



