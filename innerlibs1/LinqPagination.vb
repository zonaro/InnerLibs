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
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária.
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="context">Datacontext utilizado para conexão com o banco de dados</param>
        ''' <param name="ID">Valor da chave primária</param>
        '''  <param name="CreateIfNotExists">Se true, cria o objeto e coloca o status de INSERT pendente para este</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKey(Of T As Class)(ByVal Context As DataContext, ByVal ID As Object, Optional CreateIfNotExists As Boolean = False) As T
            Dim obj = Context.GetByPrimaryKeys(Of T)({ID}.AsEnumerable).SingleOrDefault
            If obj Is Nothing AndAlso CreateIfNotExists Then
                obj = Activator.CreateInstance(Of T)
                Context.GetTable(Of T).InsertOnSubmit(obj)
            End If
            Return obj
        End Function

        ''' <summary>
        ''' Retorna uma lista de listitem as partir de uma tabela especifcica e suas chaves primárias
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Context"></param>
        ''' <param name="IDs"></param>
        ''' <param name="Text"></param>
        ''' <param name="Value"></param>
        ''' <param name="Selected"></param>
        ''' <returns></returns>
        <Extension()> Public Function GetAsListItems(Of T As Class, TextType, ValueType)(ByVal Context As DataContext, ByVal IDs As IEnumerable, Text As Func(Of T, TextType), Optional Value As Func(Of T, ValueType) = Nothing, Optional Selected As Func(Of T, Boolean) = Nothing) As List(Of UI.WebControls.ListItem)
            If Value Is Nothing Then
                Dim table = Context.GetTable(Of T)()
                Dim mapping = Context.Mapping.GetTable(GetType(T))
                Dim pkfield = mapping.RowType.DataMembers.SingleOrDefault(Function(d) d.IsPrimaryKey)
                If pkfield IsNot Nothing Then
                    Dim param = Expression.Parameter(GetType(T), "e")
                    Value = (Expression.Lambda(Of Func(Of T, ValueType))(Expression.Property(param, pkfield.Name), param)).Compile
                End If
            End If
            Return Context.GetByPrimaryKeys(Of T)(IDs).ToListItems(Text, Value, Selected)
        End Function



        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com sua chave primaria. Pode opcionalmente criar o objeto se o mesmo não existir
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="context">Datacontext utilizado para conexão com o banco de dados</param>
        ''' <param name="IDs">Valor da chave primárias</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKeys(Of T As Class)(ByVal Context As DataContext, ByVal IDs As IEnumerable) As IQueryable(Of T)
            Dim table = Context.GetTable(Of T)()
            Dim mapping = Context.Mapping.GetTable(GetType(T))
            Dim pkfield = mapping.RowType.DataMembers.SingleOrDefault(Function(d) d.IsPrimaryKey)
            If pkfield Is Nothing Then Throw New Exception(String.Format("Table {0} does not contain a Primary Key field", mapping.TableName))
            Dim param = Expression.Parameter(GetType(T), "e")
            Dim l As New List(Of T)
            For Each ID In IDs
                Dim predicate = Expression.Lambda(Of Func(Of T, Boolean))(Expression.Equal(Expression.[Property](param, pkfield.Name), Expression.Constant(CTypeDynamic(ID, pkfield.Type))), param)
                Dim obj = table.SingleOrDefault(predicate)
                If obj IsNot Nothing Then
                    l.Add(obj)
                End If
            Next
            Return l.AsQueryable
        End Function



        Public Function ConvertGeneric(Of TParm, TReturn, TTargetParm, TTargetReturn)(ByVal input As Expression(Of Func(Of TParm, TReturn))) As Expression(Of Func(Of TTargetParm, TTargetReturn))
            Dim parm = Expression.Parameter(GetType(TTargetParm))
            Dim castParm = Expression.Convert(parm, GetType(TParm))
            Dim body = ReplaceExpression(input.Body, input.Parameters(0), castParm)
            body = Expression.Convert(body, GetType(Object))
            body = Expression.Convert(body, GetType(TTargetReturn))
            Return Expression.Lambda(Of Func(Of TTargetParm, TTargetReturn))(body, parm)
        End Function

        Private Function ReplaceExpression(ByVal body As Expression, ByVal source As Expression, ByVal dest As Expression) As Expression
            Dim replacer = New ExpressionReplacer(source, dest)
            Return replacer.Visit(body)
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
            Dim obj As T = Nothing
            If Dic.ContainsKey(pkfield.Name) Then
                Dim ID As PKType = CType(Dic(pkfield.Name), PKType)
                Dim param = Expression.Parameter(GetType(T), "e")
                Dim predicate = Expression.Lambda(Of Func(Of T, Boolean))(Expression.Equal(Expression.[Property](param, pkfield.Name), Expression.Constant(ID)), param)
                obj = table.SingleOrDefault(predicate)
            End If
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

    Friend Class ExpressionReplacer
        Inherits ExpressionVisitor

        Private _source As Expression

        Private _dest As Expression

        Public Sub New(ByVal source As Expression, ByVal dest As Expression)
            _source = source
            _dest = dest
        End Sub

        Public Overrides Function Visit(ByVal node As Expression) As Expression
            If node.Equals(_source) Then Return _dest
            Return MyBase.Visit(node)
        End Function
    End Class

End Namespace



