Imports System.Collections.Specialized
Imports System.Data.Linq
Imports System.Data.Linq.Mapping
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Web
Imports System.Web.UI.HtmlControls

Namespace LINQ

    Public Module LINQExtensions

        ''' <summary>
        ''' Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        ''' </summary>
        ''' <typeparam name="Type">Tipo do objeto acessado</typeparam>
        ''' <param name="PropertyName">Propriedade do objeto <typeparamref name="Type"/></param>
        ''' <param name="[Operator]">Operador ou método do objeto <typeparamref name="Type"/> que retorna um <see cref="Boolean"/></param>
        ''' <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="Type"/></param>
        ''' <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        ''' <returns></returns>
        Public Function WhereExpression(Of Type)(PropertyName As String, [Operator] As String, PropertyValue As Object, Optional [Is] As Boolean = True) As Expression(Of Func(Of Type, Boolean))
            Dim parameter = Expression.Parameter(GetType(Type), GetType(Type).Name.ToLower())
            Dim member = Expression.[Property](parameter, PropertyName)
            Dim constant = Expression.Constant(PropertyValue)
            Dim body As Expression
            Select Case [Operator].ToLower()
                Case "=", "==", "equal"
                    body = Expression.Equal(member, constant)
                Case ">=", "greaterthanorequal", "greaterorequal"
                    body = Expression.GreaterThanOrEqual(member, constant)
                Case "<=", "lessthanorequal", "lessorequal"
                    body = Expression.LessThanOrEqual(member, constant)
                Case ">", "greaterthan", "greater"
                    body = Expression.GreaterThan(member, constant)
                Case "<", "lessthan", "less"
                    body = Expression.LessThan(member, constant)
                Case "<>", "!=", "notequal"
                    body = Expression.NotEqual(member, constant)
                Case "like", "contains"
                    body = Expression.Call(member, containsMethod, constant)
                Case Else
                    body = Expression.Call(member, GetType(Type).GetMethod([Operator], {PropertyValue.GetType()}), constant)
            End Select

            body = Expression.Equal(body, Expression.Constant([Is]))
            Dim finalExpression = Expression.Lambda(Of Func(Of Type, Boolean))(body, parameter)
            Return finalExpression
        End Function

        ''' <summary>
        ''' Busca em um <see cref="IQueryable(Of T)"/> usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto acessado</typeparam>
        ''' <param name="List">Lista</param>
        ''' <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        ''' <param name="[Operator]">Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/></param>
        ''' <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="T"/></param>
        ''' <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        ''' <returns></returns>
        <Extension()> Function WhereExpression(Of T)(List As IQueryable(Of T), PropertyName As String, [Operator] As String, PropertyValue As Object, Optional [Is] As Boolean = True) As IQueryable(Of T)
            Return List.Where(WhereExpression(Of T)(PropertyName, [Operator], PropertyValue, [Is]))
        End Function

        ''' <summary>
        ''' Busca em um <see cref="IQueryable(Of T)"/> usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto acessado</typeparam>
        ''' <param name="List">Lista</param>
        ''' <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        ''' <param name="[Operator]">Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/></param>
        ''' <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="T"/></param>
        ''' <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        ''' <returns></returns>
        <Extension()> Function FirstOrDefaultExpression(Of T)(List As IQueryable(Of T), PropertyName As String, [Operator] As String, PropertyValue As Object, Optional [Is] As Boolean = True) As T
            Return List.FirstOrDefault(WhereExpression(Of T)(PropertyName, [Operator], PropertyValue, [Is]))
        End Function

        ''' <summary>
        ''' Busca em um <see cref="IQueryable(Of T)"/> usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto acessado</typeparam>
        ''' <param name="List">Lista</param>
        ''' <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        ''' <param name="[Operator]">Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/></param>
        ''' <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="T"/></param>
        ''' <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        ''' <returns></returns>
        <Extension()> Function SingleOrDefaultExpression(Of T)(List As IQueryable(Of T), PropertyName As String, [Operator] As String, PropertyValue As Object, Optional [Is] As Boolean = True) As T
            Return List.SingleOrDefault(WhereExpression(Of T)(PropertyName, [Operator], PropertyValue, [Is]))
        End Function

        ''' <summary>
        '''Atualiza objetos de entidade usando <see cref="Data.Linq.RefreshMode.KeepChanges"/> e envia as alteraçoes ao banco de dados
        ''' </summary>
        ''' <param name="Entities">Entidades</param>
        <Extension()> Public Sub RefreshAndSubmitChanges(Context As DataContext, ParamArray Entities As Object())
            Entities = If(Entities, {})
            If Entities.Count > 0 Then
                Context.Refresh(Data.Linq.RefreshMode.KeepChanges, Entities)
                Context.SubmitChanges()
            End If
        End Sub

        ''' <summary>
        ''' Aplica o mesmo valor de uma propriedade a todos os objetos de uma coleção a partir de uma ordem especificada
        ''' </summary>
        ''' <typeparam name="TObject">Tipo do objeto</typeparam>
        ''' <typeparam name="TOrder">Tipo do objeto de ordenacao</typeparam>
        ''' <param name="objs">colecao de objetos</param>
        ''' <param name="PropertyName">seletor da propriedade</param>
        ''' <param name="Order">seletor da ordem</param>
        ''' <param name="Ascending">ordem ascendente ou descendente</param>
        ''' <returns></returns>
        <Extension()>
        Public Function MergeProperty(Of TObject, TOrder)(objs As IEnumerable(Of TObject), PropertyName As String, Order As Func(Of TObject, TOrder), Optional Ascending As Boolean = True) As IEnumerable(Of TObject)
            objs = If(objs, {})
            objs = If(Ascending, objs.OrderBy(Order), objs.OrderByDescending(Order)).ToArray
            Dim value = objs.First().GetPropertyValue(PropertyName)
            For Each obj In objs
                obj.SetPropertyValue(PropertyName, value)
            Next
            Return objs
        End Function

        ''' <summary>
        ''' Retorna as informacoes de uma propriedade a partir de um seletor
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <typeparam name="TProperty"></typeparam>
        ''' <param name="source"></param>
        ''' <param name="propertyLambda"></param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetPropertyInfo(Of TSource, TProperty)(ByVal source As TSource, ByVal propertyLambda As Expression(Of Func(Of TSource, TProperty))) As PropertyInfo
            Dim type As Type = GetType(TSource)
            Dim member As MemberExpression = TryCast(propertyLambda.Body, MemberExpression)
            If member Is Nothing Then Throw New ArgumentException(String.Format("Expression '{0}' refers to a method, not a property.", propertyLambda.ToString()))
            Dim propInfo As PropertyInfo = TryCast(member.Member, PropertyInfo)
            If propInfo Is Nothing Then Throw New ArgumentException(String.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()))
            If type <> propInfo.ReflectedType AndAlso Not type.IsSubclassOf(propInfo.ReflectedType) Then Throw New ArgumentException(String.Format("Expression '{0}' refers to a property that is not from type {1}.", propertyLambda.ToString(), type))
            Return propInfo
        End Function

        ''' <summary>
        ''' Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="Items">Itens</param>
        ''' <param name="ChildSelector">Seletor das propriedades filhas</param>
        ''' <returns></returns>
        <Extension()>
        Public Iterator Function Traverse(Of T)(ByVal items As IEnumerable(Of T), ByVal ChildSelector As Func(Of T, IEnumerable(Of T))) As IEnumerable(Of T)
            Dim stack = New Stack(Of T)(items)
            While stack.Any()
                Dim [next] = stack.Pop()
                Yield [next]
                For Each child In ChildSelector([next])
                    stack.Push(child)
                Next
            End While
        End Function

        ''' <summary>
        ''' Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="Item">Itens</param>
        ''' <param name="ChildSelector">Seletor das propriedades filhas</param>
        ''' <returns></returns>
        <Extension()>
        Public Function Traverse(Of T)(ByVal Item As T, ByVal ChildSelector As Func(Of T, IEnumerable(Of T)), Optional IncludeMe As Boolean = False) As IEnumerable(Of T)
            Return ChildSelector(Item).Union(If(IncludeMe, {Item}, {})).Traverse(ChildSelector)
        End Function

        ''' <summary>
        ''' Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente expondo uma outra propriedade
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="Item">Itens</param>
        ''' <param name="ChildSelector">Seletor das propriedades filhas</param>
        ''' <returns></returns>
        <Extension()>
        Public Function Traverse(Of T, P)(ByVal Item As T, ByVal ChildSelector As Func(Of T, IEnumerable(Of T)), PropertySelector As Func(Of T, P), Optional IncludeMe As Boolean = False) As IEnumerable(Of P)
            Return Item.Traverse(ChildSelector, IncludeMe).Select(PropertySelector)
        End Function

        ''' <summary>
        ''' Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente expondo uma outra propriedade
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="Item">Itens</param>
        ''' <param name="ChildSelector">Seletor das propriedades filhas</param>
        ''' <returns></returns>
        <Extension()>
        Public Function Traverse(Of T, P)(ByVal Item As T, ByVal ChildSelector As Func(Of T, IEnumerable(Of T)), PropertySelector As Func(Of T, IEnumerable(Of P)), Optional IncludeMe As Boolean = False) As IEnumerable(Of P)
            Return Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector)
        End Function

        ''' <summary>
        ''' Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente expondo uma outra propriedade
        ''' </summary>
        ''' <typeparam name="T">Tipo do Objeto</typeparam>
        ''' <param name="Item">Itens</param>
        ''' <param name="ChildSelector">Seletor das propriedades filhas</param>
        ''' <returns></returns>
        <Extension()>
        Public Function Traverse(Of T, P)(ByVal Item As T, ByVal ChildSelector As Func(Of T, IEnumerable(Of T)), PropertySelector As Func(Of T, IQueryable(Of P)), Optional IncludeMe As Boolean = False) As IEnumerable(Of P)
            Return Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector)
        End Function

        Private containsMethod As MethodInfo = GetType(String).GetMethod("Contains")

        Private endsWithMethod As MethodInfo = GetType(String).GetMethod("EndsWith", {GetType(String)})

        Private startsWithMethod As MethodInfo = GetType(String).GetMethod("StartsWith", {GetType(String)})

        Private equalMethod As MethodInfo = GetType(String).GetMethod("Equals", {GetType(String)})

        ''' <summary> Concatena uma expressão com outra usando o operador AND (&&) </summary>
        ''' <typeparam name="T"></typeparam> <param name="expr1"></param> <param
        ''' name="expr2"></param> <returns></returns>
        <Extension()>
        Function [And](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
            Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
            Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[AndAlso](expr1.Body, invokedExpr), expr1.Parameters)
        End Function

        ''' <summary>
        ''' Concatena uma expressão com outra usando o operador OR (||)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="expr1"></param>
        ''' <param name="expr2"></param>
        ''' <returns></returns>
        <Extension()> Function [Or](Of T)(ByVal expr1 As Expression(Of Func(Of T, Boolean)), ByVal expr2 As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
            Dim invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast(Of Expression)())
            Return Expression.Lambda(Of Func(Of T, Boolean))(Expression.[OrElse](expr1.Body, invokedExpr), expr1.Parameters)
        End Function

        ''' <summary>
        ''' Aplica os valores encontrados nas propriedades de uma entidade em controles com mesmo ID
        ''' das colunas. Se os conroles não existirem no resultado eles serão ignorados.
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
                        Case "img"
                            CType(c, HtmlImage).Src = Obj.GetPropertyValue(c.ID).ToString
                        Case Else
                            CType(c, HtmlContainerControl).InnerHtml = Obj.GetPropertyValue(c.ID)
                    End Select
                Catch ex As Exception
                End Try
            Next
            Return Controls
        End Function

        <Extension()> Public Function ApplyToControls(Obj As NameValueCollection, ParamArray Controls As System.Web.UI.HtmlControls.HtmlControl()) As System.Web.UI.HtmlControls.HtmlControl()
            For Each c In Controls
                Try
                    Select Case c.TagName.ToLower
                        Case "input"
                            CType(c, HtmlInputControl).Value = Obj(c.ID).ToString
                        Case "select"
                            CType(c, HtmlSelect).SelectValues(Obj(c.ID))
                        Case "img"
                            CType(c, HtmlImage).Src = Obj(c.ID).ToString
                        Case Else
                            CType(c, HtmlContainerControl).InnerHtml = Obj(c.ID)
                    End Select
                Catch ex As Exception
                End Try
            Next
            Return Controls
        End Function

        ''' <summary>
        ''' Retorna uma expressão genérica a partir de uma expressão tipada
        ''' </summary>
        ''' <typeparam name="TParm"></typeparam>
        ''' <typeparam name="TReturn"></typeparam>
        ''' <typeparam name="TTargetParm"></typeparam>
        ''' <typeparam name="TTargetReturn"></typeparam>
        ''' <param name="input"></param>
        ''' <returns></returns>
        <Extension()> Public Function ConvertGeneric(Of TParm, TReturn, TTargetParm, TTargetReturn)(ByVal input As Expression(Of Func(Of TParm, TReturn))) As Expression(Of Func(Of TTargetParm, TTargetReturn))
            Dim parm = Expression.Parameter(GetType(TTargetParm))
            Dim castParm = Expression.Convert(parm, GetType(TParm))
            Dim body = ReplaceExpression(input.Body, input.Parameters(0), castParm)
            body = Expression.Convert(body, GetType(Object))
            body = Expression.Convert(body, GetType(TTargetReturn))
            Return Expression.Lambda(Of Func(Of TTargetParm, TTargetReturn))(body, parm)
        End Function

        ''' <summary>
        ''' Cria uma <see cref="Expression"/> condicional a partir de um valor <see cref="Boolean"/>
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="DefaultReturnValue">Valor padrão</param>
        ''' <returns></returns>
        Function CreateExpression(Of T)(Optional DefaultReturnValue As Boolean = True) As Expression(Of Func(Of T, Boolean))
            Return Function(f) DefaultReturnValue
        End Function

        ''' <summary>
        ''' Cria uma <see cref="Expression"/> condicional a partir de uma outra expressão
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="predicate">Valor padrão</param>
        ''' <returns></returns>
        Function CreateExpression(Of T)(predicate As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
            Return predicate
        End Function

        ''' <summary>
        ''' Cria uma Expression a partir de uma outra Expression
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <typeparam name="T2"></typeparam>
        ''' <param name="predicate"></param>
        ''' <returns></returns>
        <Extension()> Function CreateExpression(Of T, T2)(predicate As Expression(Of Func(Of T, T2))) As Expression(Of Func(Of T, T2))
            Return predicate
        End Function

        ''' <summary>
        ''' Distingui os items de uma lista a partir de uma propriedade da classe
        ''' </summary>
        ''' <typeparam name="T">Tipo da classe</typeparam>
        ''' <typeparam name="TKey">Tipo da propriedade</typeparam>
        ''' <param name="items">     Lista</param>
        ''' <param name="[property]">Propriedade</param>
        ''' <param name="OrderBy">Criterio que indica qual o objeto que deverá ser preservado na lista se encontrado mais de um</param>
        ''' <returns></returns>
        <Extension()> Public Function DistinctBy(Of T, TKey, TOrder)(ByVal Items As IEnumerable(Of T), ByVal [Property] As Func(Of T, TKey), OrderBy As Func(Of T, TOrder), Optional Descending As Boolean = False) As IEnumerable(Of T)
            Return Items.GroupBy([Property]).Select(Function(x) If(Descending, x.OrderByDescending(OrderBy), x.OrderBy(OrderBy)).FirstOrDefault())
        End Function

        ''' <summary>
        ''' Distingui os items de uma lista a partir de uma propriedade da classe
        ''' </summary>
        ''' <typeparam name="T">Tipo da classe</typeparam>
        ''' <typeparam name="TKey">Tipo da propriedade</typeparam>
        ''' <param name="items">     Lista</param>
        ''' <param name="[property]">Propriedade</param>
        ''' <returns></returns>
        <Extension()> Public Function DistinctBy(Of T, TKey)(ByVal Items As IEnumerable(Of T), ByVal [Property] As Func(Of T, TKey)) As IEnumerable(Of T)
            Return Items.GroupBy([Property]).Select(Function(x) x.FirstOrDefault())
        End Function

        ''' <summary>
        ''' Distingui os items de uma lista a partir de uma propriedade da classe
        ''' </summary>
        ''' <typeparam name="T">Tipo da classe</typeparam>
        ''' <typeparam name="TKey">Tipo da propriedade</typeparam>
        ''' <param name="items">     Lista</param>
        ''' <param name="[property]">Propriedade</param>
        ''' <returns></returns>
        <Extension()> Public Function DistinctBy(Of T, TKey)(ByVal Items As IQueryable(Of T), ByVal [Property] As Expression(Of Func(Of T, TKey))) As IQueryable(Of T)
            Return Items.GroupBy([Property]).[Select](Function(x) x.First())
        End Function

        ''' <summary>
        ''' Realiza uma acão para cada item de uma lista.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Items"> </param>
        ''' <param name="Action"></param>
        <Extension()> Public Function ForEach(Of T)(Items As IEnumerable(Of T), Action As Action(Of T)) As IEnumerable(Of T)
            For Each item In Items
                Action(item)
            Next
            Return Items
        End Function

        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária.  Pode
        ''' opcionalmente criar o objeto se o mesmo não existir
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="Table">Tabela da entidade</param>
        ''' <param name="IDs">Valores das chaves primárias</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKeys(Of T As Class)(ByVal Table As Table(Of T), ByVal IDs As Object) As IEnumerable(Of T)
            Dim mapping = Table.Context.Mapping.GetTable(GetType(T))
            Dim pkfield = mapping.RowType.DataMembers.SingleOrDefault(Function(d) d.IsPrimaryKey)
            If pkfield Is Nothing Then Throw New Exception(String.Format("Table {0} does not contain a Primary Key field or is a composite primary key", mapping.TableName))
            Dim param = Expression.Parameter(GetType(T), "e")
            Dim l As New List(Of T)
            For Each ID In IDs
                Dim predicate = Expression.Lambda(Of Func(Of T, Boolean))(Expression.Equal(Expression.[Property](param, pkfield.Name), Expression.Constant(CTypeDynamic(ID, pkfield.Type))), param)
                Dim obj = Table.SingleOrDefault(predicate)
                If obj IsNot Nothing Then
                    l.Add(obj)
                End If
            Next
            Return l.AsEnumerable
        End Function

        ''' <summary>
        ''' Retorna um array  de objetos de uma tabela especifica de acordo com uma coleção de chaves primárias.
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="context">Datacontext utilizado para conexão com o banco de dados</param>
        ''' <param name="IDs">    Valor da chave primárias</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKeys(Of T As Class)(ByVal Context As DataContext, ParamArray IDs As Object()) As IEnumerable(Of T)
            Return Context.GetTable(Of T)().GetByPrimaryKeys(IDs)
        End Function

        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária.  Pode
        ''' opcionalmente criar o objeto se o mesmo não existir
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="context">          Datacontext utilizado para conexão com o banco de dados</param>
        ''' <param name="ID">               Valor da chave primária</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKey(Of T As Class)(ByVal Context As DataContext, ByVal ID As Object) As T
            Return Context.GetByPrimaryKey(Of T)(ID, False)
        End Function

        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária.  Pode
        ''' opcionalmente criar o objeto se o mesmo não existir
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="Table">          Tabela da entidade</param>
        ''' <param name="ID">               Valor da chave primária</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKey(Of T As Class)(ByVal Table As Table(Of T), ByVal ID As Object) As T
            Return Table.Context.GetByPrimaryKey(Of T)(ID, False)
        End Function

        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária. é um alias de <see cref="GetByPrimaryKey(Of T)(Data.Linq.Table(Of T), Object)"/>
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="Table">          Tabela da entidade</param>
        ''' <param name="ID">               Valor da chave primária</param>
        ''' <returns></returns>
        <Extension> Public Function GetPK(Of T As Class)(ByVal Table As Table(Of T), ByVal ID As Object) As T
            Return Table.GetByPrimaryKey(ID)
        End Function

        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária.  Pode
        ''' opcionalmente criar o objeto se o mesmo não existir. é um alias para <see cref="GetByPrimaryKey(Of T)(Data.Linq.Table(Of T), Object, Boolean, ByRef Boolean)"/>
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="Table">          Tabela da entidade</param>
        ''' <param name="ID">               Valor da chave primária</param>
        ''' <returns></returns>
        <Extension> Public Function GetPK(Of T As Class)(ByVal Table As Table(Of T), ByVal ID As Object, CreateIfNotExists As Boolean, Optional ByRef IsNew As Boolean = False) As T
            Return Table.GetByPrimaryKey(ID, CreateIfNotExists, IsNew)
        End Function

        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária.  Pode
        ''' opcionalmente criar o objeto se o mesmo não existir
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="Table">          Tabela da entidade</param>
        ''' <param name="ID">               Valor da chave primária</param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKey(Of T As Class)(ByVal Table As Table(Of T), ByVal ID As Object, CreateIfNotExists As Boolean, Optional ByRef IsNew As Boolean = False) As T
            Return Table.Context.GetByPrimaryKey(Of T)(ID, CreateIfNotExists, IsNew)
        End Function

        ''' <summary>
        ''' Retorna um objeto de uma tabela especifica de acordo com uma chave primária.
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="context">          Datacontext utilizado para conexão com o banco de dados</param>
        ''' <param name="ID">               Valor da chave primária</param>
        ''' <param name="CreateIfNotExists">
        ''' Se true, cria o objeto e coloca o status de INSERT pendente para este
        ''' </param>
        ''' <returns></returns>
        <Extension()>
        Public Function GetByPrimaryKey(Of T As Class)(ByVal Context As DataContext, ByVal ID As Object, CreateIfNotExists As Boolean, Optional ByRef IsNew As Boolean = False) As T
            Dim obj = Nothing
            Try
                obj = Context.GetByPrimaryKeys(Of T)({ID}.ToArray).SingleOrDefault
                IsNew = False
            Catch ex As Exception
            End Try
            If obj Is Nothing AndAlso CreateIfNotExists Then
                IsNew = True
                obj = Activator.CreateInstance(Of T)
                Context.GetTable(Of T).InsertOnSubmit(obj)
            End If
            Return obj
        End Function

        ''' <summary>
        ''' Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        ''' </summary>
        ''' <typeparam name="Tsource"></typeparam>
        ''' <param name="source">  </param>
        ''' <param name="PageSize"></param>
        ''' <returns></returns>
        <Extension> Public Function GroupByPage(Of Tsource)(source As IQueryable(Of Tsource), ByVal PageSize As Integer) As Dictionary(Of Long, IEnumerable(Of Tsource))
            Return source.AsEnumerable.GroupByPage(PageSize)
        End Function

        ''' <summary>
        ''' Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        ''' </summary>
        ''' <typeparam name="Tsource"></typeparam>
        ''' <param name="source">  </param>
        ''' <param name="PageSize"></param>
        ''' <returns></returns>
        <Extension> Public Function GroupByPage(Of Tsource)(source As IEnumerable(Of Tsource), ByVal PageSize As Integer) As Dictionary(Of Long, IEnumerable(Of Tsource))
            PageSize = PageSize.SetMinValue(1)
            Return source.Select(Function(item, index) New With {item, Key .Page = index / PageSize}).GroupBy(Function(g) g.Page.Floor + 1, Function(x) x.item).ToDictionary
        End Function

        ''' <summary>
        ''' Verifica se uma instancia de uma classe possui propriedades especificas com valores igual
        ''' as de outra instancia da mesma classe
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Obj1">      Instancia 1</param>
        ''' <param name="Obj2">      Instancia 2</param>
        ''' <param name="Properties">Propriedades</param>
        ''' <returns></returns>
        <Extension()>
        Public Function HasSamePropertyValues(Of T As Class)(Obj1 As T, Obj2 As T, ParamArray Properties As Func(Of T, Object)())
            Return Properties.All(Function(x) x(Obj1).Equals(x(Obj2)))
        End Function

        ''' <summary>
        ''' Ordena um <see cref="IQueryable(Of T)"/> a partir do nome de uma ou mais propriedades
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source">      </param>
        ''' <param name="sortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension> Public Function OrderBy(Of T)(ByVal source As IQueryable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Return source.OrderBy(Function(x) True).ThenBy(SortProperty, Ascending)
        End Function


        ''' <summary>
        ''' Ordena um <see cref="IQueryable(Of T)"/> a partir do nome de uma ou mais propriedades
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source">      </param>
        ''' <param name="sortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension> Public Function OrderBy(Of T)(ByVal source As IEnumerable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Return source.OrderBy(Function(x) True).ThenBy(SortProperty, Ascending)
        End Function




        ''' <summary>
        ''' Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximaçao de uma ou mais
        ''' <see cref="String"/> com o valor de um determinado campo
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="items">       </param>
        ''' <param name="Searches">    </param>
        ''' <param name="SortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension()> Public Function OrderByLike(Of T)(ByVal items As IQueryable(Of T), ByVal Searches As String(), SortProperty As String, Optional ByVal Ascending As Boolean = True)
            Return items.OrderBy(Function(x) True).ThenByLike(Searches, SortProperty, Ascending)
        End Function

        ''' <summary>
        ''' Orderna uma lista a partir da aproximaçao de um deerminado campo com uma string
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="items">           </param>
        ''' <param name="PropertySelector"></param>
        ''' <param name="Ascending">       </param>
        ''' <param name="Searches">        </param>
        ''' <returns></returns>
        <Extension()> Public Function OrderByLike(Of T As Class)(ByVal items As IEnumerable(Of T), PropertySelector As Func(Of T, String), Ascending As Boolean, ParamArray Searches As String()) As IOrderedEnumerable(Of T)
            Return items.OrderBy(Function(x) True).ThenByLike(PropertySelector, Ascending, Searches)
        End Function

        ''' <summary>
        ''' Randomiza a ordem de um <see cref="IEnumerable"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="items"></param>
        ''' <returns></returns>
        <Extension()> Public Function OrderByRandom(Of T)(items As IEnumerable(Of T)) As IOrderedEnumerable(Of T)
            Return items.OrderBy(Function(x) Guid.NewGuid)
        End Function

        ''' <summary>
        ''' Randomiza a ordem de um <see cref="IQueryable"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="items"></param>
        ''' <returns></returns>
        <Extension()> Public Function OrderByRandom(Of T)(items As IQueryable(Of T)) As IOrderedQueryable(Of T)
            Return items.OrderBy(Function(x) Guid.NewGuid)
        End Function

        ''' <summary>
        ''' Reduz um <see cref="IQueryable"/> em uma página especifica
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Source">    </param>
        ''' <param name="PageNumber"></param>
        ''' <param name="PageSize">  </param>
        ''' <returns></returns>
        <Extension()> Function Page(Of TSource)(ByVal Source As IQueryable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IQueryable(Of TSource)
            Return Source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
        End Function

        ''' <summary>
        ''' Reduz um <see cref="IEnumerable"/> em uma página especifica
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Source">    </param>
        ''' <param name="PageNumber"></param>
        ''' <param name="PageSize">  </param>
        ''' <returns></returns>
        <Extension()>
        Function Page(Of TSource)(ByVal Source As IEnumerable(Of TSource), ByVal PageNumber As Integer, ByVal PageSize As Integer) As IEnumerable(Of TSource)
            If PageNumber < 0 Then
                Return Source
            End If
            Return Source.Skip((PageNumber - 1) * PageSize).Take(PageSize)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="IQueryable(Of T)"/> procurando em varios campos diferentes de uma entidade
        ''' </summary>
        ''' <typeparam name="ClassType"></typeparam>
        ''' <param name="Context">   </param>
        ''' <param name="SearchTerm"></param>
        ''' <param name="Properties"></param>
        ''' <returns></returns>
        <Extension()> Public Function Search(Of ClassType As Class)(Context As DataContext, SearchTerm As String, ParamArray Properties() As String) As IOrderedQueryable(Of ClassType)
            Return Context.Search(Of ClassType)({SearchTerm}, Properties)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="IQueryable(Of T)"/> procurando em varios campos diferentes de uma entidade
        ''' </summary>
        ''' <typeparam name="ClassType"></typeparam>
        ''' <param name="Context">    </param>
        ''' <param name="SearchTerms"></param>
        ''' <param name="Properties"> </param>
        ''' <returns></returns>
        <Extension()> Public Function Search(Of ClassType As Class)(Context As DataContext, SearchTerms As String(), ParamArray Properties() As String) As IOrderedQueryable(Of ClassType)
            Search = Nothing
            Dim tab = Context.GetTable(Of ClassType).AsQueryable
            Dim predi = CreateExpression(Of ClassType)(False)
            Dim mapping = Context.Mapping.GetTable(GetType(ClassType))
            Properties = If(Properties, {})
            If Properties.Count = 0 Then
                Properties = GetType(ClassType).GetProperties.Where(Function(x) x.PropertyType.Equals(GetType(String)) AndAlso x.GetCustomAttribute(Of ColumnAttribute) IsNot Nothing).Select(Function(x) x.Name).ToArray
            End If
            Properties = Properties.Select(Function(x) x.ToLower).Distinct.ToArray
            For Each prop In Properties
                Dim field = mapping.RowType.DataMembers.SingleOrDefault(Function(d) d.Name.ToLower = prop)
                If field IsNot Nothing Then
                    For Each s In SearchTerms
                        If Not IsNothing(s) Then
                            Dim param = Expression.Parameter(GetType(ClassType), "e")
                            Dim ac = Expression.[Property](param, field.Name)
                            Dim con = Expression.Constant(s)
                            Dim lk = Expression.Call(ac, containsMethod, con)
                            Dim lbd = Expression.Lambda(Of Func(Of ClassType, Boolean))(lk, param)
                            predi = predi.Or(lbd)
                        End If
                    Next
                End If
            Next
            tab = tab.Where(predi)
            For Each prop In Properties
                Dim field = mapping.RowType.DataMembers.SingleOrDefault(Function(d) d.Name.ToLower = prop)
                If field IsNot Nothing Then
                    Search = If(Search, tab.OrderBy(Function(x) True)).ThenByLike(SearchTerms, field.Name)
                End If
            Next
            Return Search
        End Function

        ''' <summary>
        ''' Retorna um <see cref="IQueryable(Of T)"/> procurando em varios campos diferentes de uma entidade
        ''' </summary>
        ''' <typeparam name="ClassType"></typeparam>
        ''' <param name="Context">    </param>
        ''' <param name="SearchTerm"></param>
        ''' <param name="Properties"> </param>
        ''' <returns></returns>
        <Extension()> Public Function Search(Of ClassType As Class)(Context As DataContext, SearchTerm As String, ParamArray Properties() As Expression(Of Func(Of ClassType, String))) As IOrderedQueryable(Of ClassType)
            Return Context.Search({SearchTerm}, Properties)
        End Function

        <Extension()> Public Function Search(Of ClassType As Class)(Table As Table(Of ClassType), SearchTerm As String, ParamArray Properties() As Expression(Of Func(Of ClassType, String))) As IOrderedQueryable(Of ClassType)
            Return Table.Search({SearchTerm}, Properties)
        End Function

        <Extension()> Public Function Search(Of ClassType As Class)(Table As Table(Of ClassType), SearchTerm As String, ParamArray Properties() As String) As IOrderedQueryable(Of ClassType)
            Return Table.Context.Search(Of ClassType)({SearchTerm}, Properties)
        End Function

        <Extension()> Public Function Search(Of ClassType As Class)(Table As Table(Of ClassType), SearchTerms As String(), ParamArray Properties() As String) As IOrderedQueryable(Of ClassType)
            Return Table.Context.Search(Of ClassType)(SearchTerms, Properties)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="IQueryable(Of T)"/> procurando em varios campos diferentes de uma entidade
        ''' </summary>
        ''' <typeparam name="ClassType"></typeparam>
        ''' <param name="Context">    </param>
        ''' <param name="SearchTerms"></param>
        ''' <param name="Properties"> </param>
        ''' <returns></returns>
        <Extension()> Public Function Search(Of ClassType As Class)(Context As DataContext, SearchTerms As String(), ParamArray Properties() As Expression(Of Func(Of ClassType, String))) As IOrderedQueryable(Of ClassType)
            Return Context.GetTable(Of ClassType).Search(SearchTerms, Properties)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="IQueryable(Of ClassType)"/> procurando em varios campos diferentes de uma entidade
        ''' </summary>
        ''' <typeparam name="ClassType">Tipo da Entidade</typeparam>
        ''' <param name="Table">Tabela da Entidade</param>
        ''' <param name="SearchTerms">Termos da pesquisa</param>
        ''' <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        ''' <returns></returns>
        <Extension()> Public Function Search(Of ClassType As Class)(Table As Table(Of ClassType), SearchTerms As String(), ParamArray Properties() As Expression(Of Func(Of ClassType, String))) As IOrderedQueryable(Of ClassType)
            Search = Nothing
            Dim tab = Table.AsQueryable
            Dim predi = CreateExpression(Of ClassType)(False)
            Dim mapping = Table.Context.Mapping.GetTable(GetType(ClassType))
            Properties = If(Properties, {})
            For Each prop In Properties
                For Each s In SearchTerms
                    If Not IsNothing(s) AndAlso s.IsNotBlank() Then
                        Dim param = prop.Parameters.First
                        Dim con = Expression.Constant(s)
                        Dim lk = Expression.Call(prop.Body, containsMethod, con)
                        Dim lbd = Expression.Lambda(Of Func(Of ClassType, Boolean))(lk, param)
                        predi = predi.Or(lbd)
                    End If
                Next
            Next
            tab = tab.Where(predi)
            For Each prop In Properties
                Search = If(Search, tab.OrderBy(Function(x) True)).ThenByLike(SearchTerms, prop)
            Next
            Return Search
        End Function

        ''' <summary>
        ''' Seleciona e une em uma unica string varios elementos
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Source">   </param>
        ''' ''' <param name="Separator"></param>
        ''' <returns></returns>
        <Extension()> Function SelectJoin(Of TSource)(ByVal Source As IEnumerable(Of TSource), Optional Separator As String = "") As String
            Return Source.SelectJoin(Nothing, Separator)
        End Function

        ''' <summary>
        ''' Seleciona e une em uma unica string varios elementos
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Source">   </param>
        ''' <param name="Selector"> </param>
        ''' <param name="Separator"></param>
        ''' <returns></returns>
        <Extension()> Function SelectJoin(Of TSource)(ByVal Source As IEnumerable(Of TSource), Optional Selector As Func(Of TSource, String) = Nothing, Optional Separator As String = "") As String
            Selector = If(Selector, Function(x) x.ToString)
            Return Source.Select(Selector).Join(Separator)
        End Function

        ''' <summary>
        ''' Seleciona e une em uma unica string varios elementos
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Source">   </param>
        ''' <param name="Selector"> </param>
        ''' <param name="Separator"></param>
        ''' <returns></returns>
        <Extension()> Function SelectJoin(Of TSource)(ByVal Source As IQueryable(Of TSource), Optional Selector As Func(Of TSource, String) = Nothing, Optional Separator As String = "") As String
            Return Source.AsEnumerable.SelectJoin(Selector, Separator)
        End Function

        ''' <summary>
        ''' Seleciona e une em uma unica string varios elementos enumeraveis
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Source">   </param>
        ''' <param name="Selector"> </param>
        ''' <param name="Separator"></param>
        ''' <returns></returns>
        <Extension()> Function SelectManyJoin(Of TSource)(ByVal Source As IEnumerable(Of TSource), Optional Selector As Func(Of TSource, IEnumerable(Of String)) = Nothing, Optional Separator As String = "") As String
            Selector = If(Selector, Function(x) {x.ToString})
            Return Source.SelectMany(Selector).Join(Separator)
        End Function

        ''' <summary>
        ''' Seleciona e une em uma unica string varios elementos enumeraveis
        ''' </summary>
        ''' <typeparam name="TSource"></typeparam>
        ''' <param name="Source">   </param>
        ''' <param name="Selector"> </param>
        ''' <param name="Separator"></param>
        ''' <returns></returns>
        <Extension()> Function SelectManyJoin(Of TSource)(ByVal Source As IQueryable(Of TSource), Optional Selector As Func(Of TSource, IEnumerable(Of String)) = Nothing, Optional Separator As String = ";") As String
            Return Source.AsEnumerable.SelectManyJoin(Selector, Separator)
        End Function

        ''' <summary>
        ''' Ordena um <see cref="IEnumerable"/> priorizando valores especificos a uma condição no
        ''' inicio da coleção e então segue uma ordem padrão para os outros.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <typeparam name="DefaultOrderType"></typeparam>
        ''' <param name="items">       colecao</param>
        ''' <param name="Priority">    Seletor que define a prioridade</param>
        ''' <param name="DefaultOrder">ordenacao padrao para os outros itens</param>
        ''' <returns></returns>
        <Extension()>
        Public Iterator Function TakeAndOrder(Of T, DefaultOrderType)(ByVal items As IEnumerable(Of T), ByVal Priority As Func(Of T, Boolean), Optional DefaultOrder As Func(Of T, DefaultOrderType) = Nothing) As IEnumerable(Of T)
            DefaultOrder = If(DefaultOrder, Function(x) True)
            For Each item In items.Where(Priority)
                Yield item
            Next

            For Each item In items.Where(Function(i) Not Priority(i)).OrderBy(Function(i) i)
                Yield item
            Next
        End Function

        ''' <summary>
        ''' Ordena um <see cref="IQueryable(Of T)"/> a partir do nome de uma ou mais propriedades
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source">      </param>
        ''' <param name="sortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension()>
        Public Function ThenBy(Of T)(ByVal source As IOrderedQueryable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Dim type = GetType(T)
            For Each prop In SortProperty
                Dim [property] = type.GetProperty(prop)
                Dim parameter = Expression.Parameter(type, "p")
                Dim propertyAccess = Expression.MakeMemberAccess(parameter, [property])
                Dim orderByExp = Expression.Lambda(propertyAccess, parameter)
                Dim typeArguments = New Type() {type, [property].PropertyType}
                Dim methodName = If(Ascending, "OrderBy", "OrderByDescending")
                Dim resultExp = Expression.[Call](GetType(Queryable), methodName, typeArguments, source.Expression, Expression.Quote(orderByExp))
                source = source.Provider.CreateQuery(Of T)(resultExp)
            Next
            Return source
        End Function

        ''' <summary>
        ''' Ordena um <see cref="ienumerable(Of T)"/> a partir do nome de uma ou mais propriedades
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source">      </param>
        ''' <param name="sortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension()>
        Public Function ThenBy(Of T)(ByVal source As IOrderedEnumerable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedEnumerable(Of T)
            Dim type = GetType(T)
            For Each prop In SortProperty
                source = source.ThenBy(Function(x) x.GetPropertyValue(prop))
            Next
            Return source
        End Function

        ''' <summary>
        ''' Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximaçao de uma ou mais
        ''' <see cref="String"/> com o valor de um determinado campo
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="items">       </param>
        ''' <param name="Searches">    </param>
        ''' <param name="SortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension()> Public Function ThenByLike(Of T)(ByVal items As IOrderedQueryable(Of T), Searches As String(), SortProperty As String, Optional Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Dim type = GetType(T)
            Searches = If(Searches, {})
            For Each t In Searches
                Dim [property] = type.GetProperty(SortProperty)
                Dim parameter = Expression.Parameter(type, "p")
                Dim propertyAccess = Expression.MakeMemberAccess(parameter, [property])
                Dim orderByExp = Expression.Lambda(propertyAccess, parameter)

                Dim testes As MethodCallExpression() = {
                        Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)),
                        Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)),
                        Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)),
                        Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t))
                        }

                For Each exp In testes
                    Dim nv = Expression.Lambda(Of Func(Of T, Boolean))(exp, parameter)
                    If Ascending Then
                        items = items.ThenByDescending(nv)
                    Else
                        items = items.ThenBy(nv)
                    End If
                Next

            Next
            Return items
        End Function

        ''' <summary>
        ''' Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximaçao de uma ou mais
        ''' <see cref="String"/> com o valor de um determinado campo
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="items">       </param>
        ''' <param name="Searches">    </param>
        ''' <param name="SortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension()> Public Function ThenByLike(Of T)(ByVal items As IOrderedQueryable(Of T), Searches As String(), SortProperty As Expression(Of Func(Of T, String)), Optional Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Dim type = GetType(T)
            Searches = If(Searches, {})
            For Each t In Searches
                Dim mem As MemberExpression = SortProperty.Body
                Dim [property] = mem.Member
                Dim parameter = SortProperty.Parameters.First
                Dim propertyAccess = Expression.MakeMemberAccess(parameter, [property])
                Dim orderByExp = Expression.Lambda(propertyAccess, parameter)

                Dim tests As MethodCallExpression() = {
                        Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)),
                        Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)),
                        Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)),
                        Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t))
                        }

                For Each exp In tests
                    Dim nv = Expression.Lambda(Of Func(Of T, Boolean))(exp, parameter)
                    If Ascending Then
                        items = items.ThenByDescending(nv)
                    Else
                        items = items.ThenBy(nv)
                    End If
                Next

            Next
            Return items
        End Function

        ''' <summary>
        ''' Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximaçao de uma ou mais
        ''' <see cref="String"/> com o valor de um determinado campo
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="items">           </param>
        ''' <param name="PropertySelector"></param>
        ''' <param name="Ascending">       </param>
        ''' <param name="Searches">        </param>
        ''' <returns></returns>
        <Extension()> Public Function ThenByLike(Of T As Class)(ByVal items As IOrderedEnumerable(Of T), PropertySelector As Func(Of T, String), Ascending As Boolean, ParamArray Searches As String()) As IOrderedEnumerable(Of T)
            Searches = If(Searches, {})
            If Searches.Count > 0 Then
                For Each t In Searches
                    For Each exp In {
                        Function(x) PropertySelector(x) = t,
                        Function(x) PropertySelector(x).StartsWith(t),
                        Function(x) PropertySelector(x).Contains(t),
                        Function(x) PropertySelector(x).EndsWith(t)
                      }

                        If Ascending Then
                            items = items.ThenByDescending(exp)
                        Else
                            items = items.ThenBy(exp)
                        End If
                    Next
                Next
            End If

            Return items
        End Function

        ''' <summary>
        ''' Coloca todos os abjetos que atendem a um predicado em um estado de PENDING DELETE
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="Table">Tabela</param>
        ''' <param name="predicate">predicado</param>
        <Extension()> Public Sub DeleteAllOnSubmitWhere(Of T As Class)(Table As Table(Of T), predicate As Func(Of T, Boolean))
            Table.DeleteAllOnSubmit(Table.Where(predicate))
        End Sub

        ''' <summary>
        ''' Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        ''' </summary>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension()> Public Function Most(List As IEnumerable(Of Boolean), Optional Result As Boolean = True) As Boolean
            If List.Count > 0 Then
                Dim arr = List.DistinctCount
                If arr.ContainsKey(True) AndAlso arr.ContainsKey(False) Then
                    Return arr(Result) > arr(Not Result)
                Else
                    Return arr.First.Key = Result
                End If
            End If
            Return False = Result
        End Function

        ''' <summary>
        ''' Retorna TRUE se a maioria dos testes em uma lista retornarem TRUE
        ''' </summary>
        ''' <param name="Tests"></param>
        ''' <returns></returns>
        Public Function MostTrue(ParamArray Tests As Boolean()) As Boolean
            Return If(Tests, {}).Most(True)
        End Function

        ''' <summary>
        ''' Retorna TRUE se a maioria dos testes em uma lista retornarem FALSE
        ''' </summary>
        ''' <param name="Tests"></param>
        ''' <returns></returns>
        Public Function MostFalse(ParamArray Tests As Boolean()) As Boolean
            Return If(Tests, {}).Most(False)
        End Function

        ''' <summary>
        ''' Retorna TRUE se a todos os testes em uma lista retornarem TRUE
        ''' </summary>
        ''' <param name="Tests"></param>
        ''' <returns></returns>
        Public Function AllTrue(ParamArray Tests As Boolean()) As Boolean
            Return If(Tests, {}).All(Function(x) x = True)
        End Function

        ''' <summary>
        ''' Retorna TRUE se a todos os testes em uma lista retornarem FALSE
        ''' </summary>
        ''' <param name="Tests"></param>
        ''' <returns></returns>
        Public Function AllFalse(ParamArray Tests As Boolean()) As Boolean
            Return If(Tests, {}).All(Function(x) x = False)
        End Function

        ''' <summary>
        ''' Atualiza um objeto de entidade a partir de valores em um Dictionary
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <typeparam name="PKType"></typeparam>
        ''' <param name="Context"></param>
        ''' <param name="Dic">    </param>
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
        ''' <param name="Keys">   </param>
        ''' <returns></returns>
        <Extension()>
        Public Function UpdateObjectFromRequest(Of T As Class, PKType As Structure)(ByVal Context As DataContext, Request As HttpRequest, ParamArray Keys As String()) As T
            Return Context.UpdateObjectFromDictionary(Of T, PKType)(Request.ToDictionary(Keys))
        End Function

        Private Function ReplaceExpression(ByVal body As Expression, ByVal source As Expression, ByVal dest As Expression) As Expression
            Dim replacer = New ExpressionReplacer(source, dest)
            Return replacer.Visit(body)
        End Function

    End Module

    Friend Class ExpressionReplacer
        Inherits ExpressionVisitor

        Private _dest As Expression
        Private _source As Expression

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