Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace LINQ

    Public Module LINQExtensions

        <Extension()> Public Function TakeRandom(Of T)(l As IEnumerable(Of T), Count As Integer) As IEnumerable(Of T)
            Return l.OrderByRandom().Take(Count)
        End Function

        <Extension()> Public Function FirstRandom(Of T)(l As IEnumerable(Of T)) As T
            Return l.OrderByRandom().FirstOrDefault()
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class)(ByVal List As IEnumerable(Of T)) As PaginationFilter(Of T, T)
            Return New PaginationFilter(Of T, T)().SetData(List)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class)(ByVal List As IEnumerable(Of T), Configuration As Action(Of PaginationFilter(Of T, T))) As PaginationFilter(Of T, T)
            Return New PaginationFilter(Of T, T)(Configuration).SetData(List)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class, R)(ByVal List As IEnumerable(Of T), RemapExpression As Func(Of T, R), Configuration As Action(Of PaginationFilter(Of T, R))) As PaginationFilter(Of T, R)
            Return New PaginationFilter(Of T, R)(RemapExpression, Configuration).SetData(List)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class, R)(ByVal List As IEnumerable(Of T), RemapExpression As Func(Of T, R)) As PaginationFilter(Of T, R)
            Return New PaginationFilter(Of T, R)(RemapExpression).SetData(List)
        End Function

        <Extension()> Public Function PropertyExpression(Parameter As ParameterExpression, PropertyName As String) As Expression
            Dim prop As Expression = Parameter
            If PropertyName.IfBlank("this") <> "this" Then
                For Each name In PropertyName.SplitAny(".", "/")
                    prop = Expression.[Property](prop, name)
                Next
            End If
            Return prop
        End Function

        ''' <summary>
        ''' Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        ''' </summary>
        ''' <typeparam name="Type">Tipo do objeto acessado</typeparam>
        ''' <param name="PropertyName">Propriedade do objeto <typeparamref name="Type"/></param>
        ''' <param name="[Operator]">Operador ou método do objeto <typeparamref name="Type"/> que retorna um <see cref="Boolean"/></param>
        ''' <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="Type"/></param>
        ''' <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        ''' <returns></returns>
        Public Function WhereExpression(Of Type)(PropertyName As String, [Operator] As String, PropertyValue As IEnumerable(Of IComparable), Optional [Is] As Boolean = True, Optional Conditional As FilterConditional = FilterConditional.Or) As Expression(Of Func(Of Type, Boolean))
            Dim parameter = GenerateParameterExpression(Of Type)()
            Dim member = PropertyExpression(parameter, PropertyName)
            Dim body As Expression = GetOperatorExpression(member, [Operator], PropertyValue, Conditional)
            body = Expression.Equal(body, Expression.Constant([Is]))
            Dim finalExpression = Expression.Lambda(Of Func(Of Type, Boolean))(body, parameter)
            Return finalExpression
        End Function

        Public Function WhereExpression(Of Type, V)(PropertySelector As Expression(Of Func(Of Type, V)), [Operator] As String, PropertyValue As IEnumerable(Of IComparable), Optional [Is] As Boolean = True, Optional Conditional As FilterConditional = FilterConditional.Or) As Expression(Of Func(Of Type, Boolean))
            Dim parameter = GenerateParameterExpression(Of Type)()
            Dim member = PropertySelector.Body.ToString().Split(".").Skip(1).Join(".")
            Dim prop = PropertyExpression(parameter, member)
            Dim body As Expression = GetOperatorExpression(prop, [Operator], PropertyValue, Conditional)
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
        <Extension()> Function WhereExpression(Of T)(List As IQueryable(Of T), PropertyName As String, [Operator] As String, PropertyValue As IEnumerable(Of IComparable), Optional [Is] As Boolean = True, Optional Exclusive As Boolean = True) As IQueryable(Of T)
            Return List.Where(WhereExpression(Of T)(PropertyName, [Operator], PropertyValue, [Is]))
        End Function

        <Extension> Public Function CreateNullableType(ByVal type As Type) As Type
            If type.IsValueType AndAlso (Not type.IsGenericType OrElse type.GetGenericTypeDefinition() <> GetType(Nullable(Of))) Then Return GetType(Nullable(Of)).MakeGenericType(type)
            Return type
        End Function

        <Extension()> Public Function FilterDateRange(Of T)(List As IQueryable(Of T), ByVal [Property] As Expression(Of Func(Of T, DateTime)), Range As DateRange) As IQueryable(Of T)
            Return List.Where(IsBetween([Property], Range.StartDate, Range.EndDate))
        End Function

        <Extension()> Public Function FilterDateRange(Of T)(List As IQueryable(Of T), ByVal [Property] As Expression(Of Func(Of T, DateTime?)), Range As DateRange) As IQueryable(Of T)
            Return List.Where(IsBetween([Property], Range.StartDate, Range.EndDate))
        End Function

        <Extension()> Public Function FilterDateRange(Of T, V)(List As IQueryable(Of T), ByVal MinProperty As Expression(Of Func(Of T, V)), [MaxProperty] As Expression(Of Func(Of T, V)), Values As IEnumerable(Of V)) As IQueryable(Of T)
            Return List.Where(IsBetween(MinProperty, MaxProperty, Values))
        End Function

        <Extension()> Public Function FilterDateRange(Of T, V)(List As IQueryable(Of T), ByVal MinProperty As Expression(Of Func(Of T, V)), [MaxProperty] As Expression(Of Func(Of T, V)), ParamArray Values As V()) As IQueryable(Of T)
            Return List.Where(IsBetween(MinProperty, MaxProperty, Values))
        End Function

        <Extension()> Public Function IsBetween(Of T, V)(ByVal MinProperty As Expression(Of Func(Of T, V)), [MaxProperty] As Expression(Of Func(Of T, V)), Values As IEnumerable(Of V)) As Expression(Of Func(Of T, Boolean))
            Dim exp = LINQExtensions.CreateWhereExpression(Of T)(False)
            For Each item In If(Values, {})
                exp = exp.Or(WhereExpression(MinProperty, "<=", {item}).And(WhereExpression(MaxProperty, ">=", {item})))
            Next
            Return exp
        End Function

        <Extension()> Public Function IsBetween(Of T, V)(ByVal MinProperty As Expression(Of Func(Of T, V)), [MaxProperty] As Expression(Of Func(Of T, V)), ParamArray Values As V()) As Expression(Of Func(Of T, Boolean))
            Return IsBetween(MinProperty, MaxProperty, Values.AsEnumerable())
        End Function

        <Extension()>
        Public Function IsBetween(Of T, V)(ByVal [Property] As Expression(Of Func(Of T, V)), MinValue As V, MaxValue As V) As Expression(Of Func(Of T, Boolean))
            Return WhereExpression([Property], "between", {MinValue, MaxValue})
        End Function

        <Extension()>
        Public Function IsBetween(Of T)(ByVal [Property] As Expression(Of Func(Of T, DateTime)), DateRange As DateRange) As Expression(Of Func(Of T, Boolean))
            Return WhereExpression([Property], "between", {DateRange.StartDate, DateRange.EndDate})
        End Function

        <Extension()>
        Public Function IsBetween(Of T)(ByVal [Property] As Expression(Of Func(Of T, DateTime?)), DateRange As DateRange) As Expression(Of Func(Of T, Boolean))
            Return WhereExpression([Property], "between", {DateRange.StartDate, DateRange.EndDate})
        End Function

        <Extension> Public Function CreatePropertyExpression(Of T, V)(ByVal [Property] As Expression(Of Func(Of T, V))) As MemberExpression
            Return Expression.[Property](If([Property].Parameters.FirstOrDefault(), GetType(T).GenerateParameterExpression()), GetPropertyInfo([Property]))
        End Function

        <Extension()> Public Function GetPropertyInfo(Of TSource, TProperty)(ByVal propertyLambda As Expression(Of Func(Of TSource, TProperty))) As PropertyInfo
            Dim type As Type = GetType(TSource)
            Dim member As MemberExpression = TryCast(propertyLambda.Body, MemberExpression)
            If member Is Nothing Then Throw New ArgumentException(String.Format("Expression '{0}' refers to a method, not a property.", propertyLambda.ToString()))
            Dim propInfo As PropertyInfo = TryCast(member.Member, PropertyInfo)
            If propInfo Is Nothing Then Throw New ArgumentException(String.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()))
            Return propInfo
        End Function

        <Extension()> Public Sub FixNullable(ByRef e1 As Expression, ByRef e2 As Expression)
            Dim e1type As Type = e1.Type
            Dim e2type As Type = e2.Type

            Try
                e1type = CType(e1, LambdaExpression).ReturnType
            Catch ex As Exception
            End Try

            Try
                e2type = CType(e2, LambdaExpression).ReturnType
            Catch ex As Exception
            End Try

            If e1type.IsNullableType AndAlso Not e2type.IsNullableType Then
                e2 = Expression.Convert(e2, e1type)
            End If

            If Not e1type.IsNullableType AndAlso e2type.IsNullableType Then
                e1 = Expression.Convert(e1, e2type)
            End If

            If e1.NodeType = ExpressionType.Lambda Then
                e1 = Expression.Invoke(e1, CType(e1, LambdaExpression).Parameters)
            End If

            If e2.NodeType = ExpressionType.Lambda Then
                e2 = Expression.Invoke(e2, CType(e2, LambdaExpression).Parameters)
            End If

        End Sub

        <Extension> Public Function GreaterThanOrEqual(ByVal MemberExpression As Expression, ByVal ValueExpression As Expression) As BinaryExpression
            FixNullable(MemberExpression, ValueExpression)
            Return Expression.GreaterThanOrEqual(MemberExpression, ValueExpression)
        End Function

        <Extension> Public Function LessThanOrEqual(ByVal MemberExpression As Expression, ByVal ValueExpression As Expression) As BinaryExpression
            FixNullable(MemberExpression, ValueExpression)
            Return Expression.LessThanOrEqual(MemberExpression, ValueExpression)
        End Function

        <Extension> Public Function GreaterThan(ByVal MemberExpression As Expression, ByVal ValueExpression As Expression) As BinaryExpression
            FixNullable(MemberExpression, ValueExpression)
            Return Expression.GreaterThan(MemberExpression, ValueExpression)
        End Function

        <Extension> Public Function LessThan(ByVal MemberExpression As Expression, ByVal ValueExpression As Expression) As BinaryExpression
            FixNullable(MemberExpression, ValueExpression)
            Return Expression.LessThan(MemberExpression, ValueExpression)
        End Function

        <Extension> Public Function Equal(ByVal MemberExpression As Expression, ByVal ValueExpression As Expression) As BinaryExpression
            FixNullable(MemberExpression, ValueExpression)
            Return Expression.Equal(MemberExpression, ValueExpression)
        End Function

        <Extension> Public Function NotEqual(ByVal MemberExpression As Expression, ByVal ValueExpression As Expression) As BinaryExpression
            FixNullable(MemberExpression, ValueExpression)
            Return Expression.NotEqual(MemberExpression, ValueExpression)
        End Function

        Public Function CreateConstant(Member As Expression, Value As IComparable) As ConstantExpression
            Return CreateConstant(Member.Type, Value)
        End Function

        Public Function CreateConstant(Type As Type, Value As IComparable) As ConstantExpression
            If Value Is Nothing Then
                Return Expression.Constant(Nothing, Type)
            End If
            Return Expression.Constant(Value.ChangeType(Type))
        End Function

        Public Function CreateConstant(Of Type)(Value As IComparable) As ConstantExpression
            Return CreateConstant(GetType(Type), Value)
        End Function

        ''' <summary>
        ''' Retorna uma expressão de comparação para um ou mais valores
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="[Operator]"></param>
        ''' <param name="PropertyValues"></param>
        ''' <param name="Conditional"></param>
        ''' <returns></returns>
        Function GetOperatorExpression(Member As Expression, [Operator] As String, PropertyValues As IEnumerable(Of IComparable), Optional Conditional As FilterConditional = FilterConditional.Or) As BinaryExpression
            PropertyValues = If(PropertyValues, {})
            Dim comparewith As Boolean = Not [Operator].StartsWithAny("!")
            If comparewith = False Then
                [Operator] = [Operator].RemoveFirstAny(False, "!")
            End If
            Dim body As BinaryExpression = Nothing
            'Dim body As Expression = Nothing
            Select Case [Operator].ToLower().IfBlank("equal")
                Case "blank", "compareblank", "isblank", "isempty", "empty"
                    For Each item In PropertyValues
                        Dim exp = Expression.Equal(Member, Expression.Constant(String.Empty, Member.Type))
                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next
                Case "isnull", "comparenull", "null", "nothing", "isnothing"
                    For Each item In PropertyValues
                        Dim exp = Expression.Equal(Member, Expression.Constant(Nothing, Member.Type))
                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next
                Case "=", "==", "equal", "===", "equals"
                    For Each item In PropertyValues
                        Dim exp = Nothing
                        Try
                            exp = LINQExtensions.Equal(Member, CreateConstant(Member, item))
                        Catch ex As Exception
                            exp = Expression.Constant(False)
                            Continue For
                        End Try

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next
                Case ">=", "greaterthanorequal", "greaterorequal", "greaterequal", "greatequal"
                    For Each item In PropertyValues
                        If item.GetNullableTypeOf() IsNot GetType(DateTime) AndAlso item.IsNotNumber() AndAlso item.ToString().IsNotBlank() Then
                            item = item.ToString().Length
                        End If
                        Dim exp = Nothing
                        Try
                            exp = GreaterThanOrEqual(Member, CreateConstant(Member, item))
                        Catch ex As Exception
                            exp = Expression.Constant(False)
                            Continue For
                        End Try

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next
                Case "<=", "lessthanorequal", "lessorequal", "lessequal"
                    For Each item In PropertyValues
                        If item.GetNullableTypeOf() IsNot GetType(DateTime) AndAlso item.IsNotNumber() AndAlso item.ToString().IsNotBlank() Then
                            item = item.ToString().Length
                        End If
                        Dim exp = Nothing
                        Try
                            exp = LINQExtensions.LessThanOrEqual(Member, CreateConstant(Member, item))
                        Catch ex As Exception
                            exp = Expression.Constant(False)
                            Continue For
                        End Try
                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next
                Case ">", "greaterthan", "greater", "great"
                    For Each item In PropertyValues
                        Dim exp = Nothing
                        Try
                            exp = LINQExtensions.GreaterThan(Member, CreateConstant(Member, item))
                        Catch ex As Exception
                            Continue For
                        End Try

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next

                Case "<", "lessthan", "less"
                    For Each item In PropertyValues
                        Dim exp = Nothing
                        Try
                            exp = LINQExtensions.LessThan(Member, CreateConstant(Member, item))
                        Catch ex As Exception
                            Continue For
                        End Try

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next
                Case "<>", "notequal", "different"
                    For Each item In PropertyValues
                        Dim exp = Nothing
                        Try
                            exp = LINQExtensions.NotEqual(Member, CreateConstant(Member, item))
                        Catch ex As Exception
                            Continue For
                        End Try

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                        If comparewith = False Then
                            body = Expression.Equal(exp, Expression.Constant(False))
                        End If
                    Next
                Case "between", "btw", "=><="
                    If PropertyValues.Count() > 1 Then
                        Select Case Member.Type
                            Case GetType(String)
                                body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", Not comparewith), {PropertyValues.First()}, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", Not comparewith), {PropertyValues.Last()}, Conditional))
                            Case Else
                                body = Expression.And(GetOperatorExpression(Member, "greaterequal".PrependIf("!", Not comparewith), {PropertyValues.Min()}, Conditional), GetOperatorExpression(Member, "lessequal".PrependIf("!", Not comparewith), {PropertyValues.Max()}, Conditional))
                        End Select
                    Else
                        body = GetOperatorExpression(Member, "=".PrependIf("!", Not comparewith), PropertyValues, Conditional)
                    End If
                Case "><"
                    If PropertyValues.Count() > 1 Then
                        Select Case Member.Type
                            Case GetType(String)
                                body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", Not comparewith), {PropertyValues.First()}, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", Not comparewith), {PropertyValues.Last()}, Conditional))
                            Case Else
                                body = Expression.And(GetOperatorExpression(Member, "greater".PrependIf("!", Not comparewith), {PropertyValues.Min()}, Conditional), GetOperatorExpression(Member, "less".PrependIf("!", Not comparewith), {PropertyValues.Max()}, Conditional))
                        End Select
                    Else
                        body = GetOperatorExpression(Member, "=".PrependIf("!", Not comparewith), PropertyValues, Conditional)
                    End If

                Case "starts", "start", "startwith", "startswith"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Nothing
                                Try
                                    exp = Expression.Equal(Expression.Call(Member, startsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith))
                                Catch ex As Exception
                                    Continue For
                                End Try

                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Conditional = FilterConditional.And Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                                If comparewith = False Then
                                    body = Expression.Equal(exp, Expression.Constant(False))
                                End If
                            Next
                        Case Else
                            body = GetOperatorExpression(Member, ">=", PropertyValues, Conditional)
                    End Select
                Case "ends", "end", "endwith", "endswith"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Nothing
                                Try
                                    exp = Expression.Equal(Expression.Call(Member, endsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith))
                                Catch ex As Exception
                                    Continue For
                                End Try

                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Conditional = FilterConditional.And Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                                If comparewith = False Then
                                    body = Expression.Equal(exp, Expression.Constant(False))
                                End If
                            Next
                        Case Else
                            body = GetOperatorExpression(Member, "lessequal".PrependIf("!", Not comparewith), PropertyValues, Conditional)
                    End Select
                Case "like", "contains"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Nothing
                                Try
                                    exp = Expression.Equal(Expression.Call(Member, containsMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith))
                                Catch ex As Exception
                                    Continue For
                                End Try

                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Conditional = FilterConditional.And Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                                If comparewith = False Then
                                    body = Expression.Equal(exp, Expression.Constant(False))
                                End If
                            Next
                        Case Else
                            body = GetOperatorExpression(Member, "equal".PrependIf("!", Not comparewith), PropertyValues, Conditional)
                    End Select
                Case "isin", "inside"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Nothing
                                Try
                                    exp = Expression.Equal(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Constant(comparewith))
                                Catch ex As Exception
                                    Continue For
                                End Try

                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Conditional = FilterConditional.And Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                                If comparewith = False Then
                                    body = Expression.Equal(exp, Expression.Constant(False))
                                End If
                            Next
                        Case Else
                            ''TODO: implementar busca de array de inteiro,data etc
                            body = GetOperatorExpression(Member, "equal".PrependIf("!", Not comparewith), PropertyValues, Conditional)
                    End Select
                Case "cross", "crosscontains", "insidecontains"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Nothing
                                Try
                                    exp = Expression.Equal(Expression.OrElse(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Call(Member, containsMethod, Expression.Constant(item.ToString()))), Expression.Constant(comparewith))
                                Catch ex As Exception
                                    Continue For
                                End Try

                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Conditional = FilterConditional.And Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                                If comparewith = False Then
                                    body = Expression.Equal(exp, Expression.Constant(False))
                                End If
                            Next
                        Case Else
                            ''TODO: implementar busca de array de inteiro,data etc
                            body = GetOperatorExpression(Member, "equal".PrependIf("!", Not comparewith), PropertyValues, Conditional)
                    End Select
                Case Else
                    Try
                        Dim mettodo = Member.Type.GetMethods().FirstOrDefault(Function(x) x.Name.ToLower() = [Operator].ToLower())

                        Dim exp As Expression = mettodo.Invoke(Nothing, {PropertyValues})

                        exp = Expression.Equal(Expression.Invoke(exp, {Member}), Expression.Constant(comparewith))
                        If body Is Nothing Then
                            body = exp
                        Else
                            If Conditional = FilterConditional.And Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
            End Select
            Return body
        End Function

        ''' <summary>
        ''' Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        ''' </summary>
        ''' <returns></returns>
        Function GenerateParameterExpression(Of ClassType)() As ParameterExpression
            Return GetType(ClassType).GenerateParameterExpression()
        End Function

        ''' <summary>
        ''' Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        ''' </summary>
        ''' <param name="Type"></param>
        ''' <returns></returns>
        <Extension> Function GenerateParameterExpression(Type As Type) As ParameterExpression
            Return Expression.Parameter(Type, Type.GenerateParameterName())
        End Function

        <Extension> Function GenerateParameterName(Type As Type) As String
            If Type IsNot Nothing Then
                Return Type.Name.CamelSplit.SelectJoin(Function(x) x.FirstOrDefault().IfBlank(Of Char)(""), "").ToLower()
            End If
            Return "p"
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
        ''' <param name="ParentSelector">Seletor das propriedades filhas</param>
        ''' <returns></returns>
        <Extension()>
        Public Iterator Function Traverse(Of T)(ByVal item As T, ByVal ParentSelector As Func(Of T, T)) As IEnumerable(Of T)
            If item IsNot Nothing Then
                Dim current = item
                Do
                    Yield current
                    current = ParentSelector(current)
                Loop While current IsNot Nothing
            End If
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

        Private containsMethod As MethodInfo = GetType(String).GetMethod("Contains", {GetType(String)})

        Private endsWithMethod As MethodInfo = GetType(String).GetMethod("EndsWith", {GetType(String)})

        Private startsWithMethod As MethodInfo = GetType(String).GetMethod("StartsWith", {GetType(String)})

        Private equalMethod As MethodInfo = GetType(String).GetMethod("Equals", {GetType(String)})

        ''' <summary>
        ''' Cria uma <see cref="Expression"/> condicional a partir de um valor <see cref="Boolean"/>
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="DefaultReturnValue">Valor padrão</param>
        ''' <returns></returns>
        <Extension()> Function CreateWhereExpression(Of T)(DefaultReturnValue As Boolean) As Expression(Of Func(Of T, Boolean))
            Return Function(f) DefaultReturnValue
        End Function

        ''' <summary>
        ''' Concatena uma expressão com outra usando o operador And  
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="FirstExpression"></param>
        ''' <param name="OtherExpressions"></param>
        ''' <returns></returns>
        <Extension()>
        Function [And](Of T)(ByVal FirstExpression As Expression(Of Func(Of T, Boolean)), ParamArray OtherExpressions As Expression(Of Func(Of T, Boolean))()) As Expression(Of Func(Of T, Boolean))
            FirstExpression = If(FirstExpression, CreateWhereExpression(Of T)(True))
            For Each item In If(OtherExpressions, {})
                If item IsNot Nothing Then
                    Dim invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast(Of Expression)())
                    FirstExpression = Expression.Lambda(Of Func(Of T, Boolean))(Expression.[AndAlso](FirstExpression.Body, invokedExpr), FirstExpression.Parameters)
                End If
            Next
            Return FirstExpression
        End Function

        ''' <summary>
        ''' Concatena uma expressão com outra usando o operador OR (||)
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="FirstExpression"></param>
        ''' <param name="OtherExpressions"></param>
        ''' <returns></returns>
        <Extension()> Function [Or](Of T)(ByVal FirstExpression As Expression(Of Func(Of T, Boolean)), ParamArray OtherExpressions As Expression(Of Func(Of T, Boolean))()) As Expression(Of Func(Of T, Boolean))
            FirstExpression = If(FirstExpression, CreateWhereExpression(Of T)(False))
            For Each item In If(OtherExpressions, {})
                If item IsNot Nothing Then
                    Dim invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast(Of Expression)())
                    FirstExpression = Expression.Lambda(Of Func(Of T, Boolean))(Expression.[OrElse](FirstExpression.Body, invokedExpr), FirstExpression.Parameters)
                End If
            Next
            Return FirstExpression
        End Function

        <Extension()> Function OrSearch(Of T)(ByVal FirstExpression As Expression(Of Func(Of T, Boolean)), Text As IEnumerable(Of String), ParamArray Properties As Expression(Of Func(Of T, String))()) As Expression(Of Func(Of T, Boolean))
            Return If(FirstExpression, CreateWhereExpression(Of T)(False)).Or(SearchExpression(Text, Properties))
        End Function

        <Extension()> Function OrSearch(Of T)(ByVal FirstExpression As Expression(Of Func(Of T, Boolean)), Text As String, ParamArray Properties As Expression(Of Func(Of T, String))()) As Expression(Of Func(Of T, Boolean))
            Return If(FirstExpression, CreateWhereExpression(Of T)(False)).Or(SearchExpression(Text, Properties))
        End Function

        <Extension()> Function AndSearch(Of T)(ByVal FirstExpression As Expression(Of Func(Of T, Boolean)), Text As IEnumerable(Of String), ParamArray Properties As Expression(Of Func(Of T, String))()) As Expression(Of Func(Of T, Boolean))
            Return If(FirstExpression, CreateWhereExpression(Of T)(True)).And(SearchExpression(Text, Properties))
        End Function

        <Extension()> Function AndSearch(Of T)(ByVal FirstExpression As Expression(Of Func(Of T, Boolean)), Text As String, ParamArray Properties As Expression(Of Func(Of T, String))()) As Expression(Of Func(Of T, Boolean))
            Return If(FirstExpression, CreateWhereExpression(Of T)(True)).And(SearchExpression(Text, Properties))
        End Function

        <Extension()> Function SearchExpression(Of T)(Text As IEnumerable(Of String), ParamArray Properties As Expression(Of Func(Of T, String))()) As Expression(Of Func(Of T, Boolean))
            Properties = Properties.NullAsEmpty()
            Dim predi = CreateWhereExpression(Of T)(False)
            For Each prop In Properties
                For Each s In Text
                    If Not s = Nothing AndAlso s.IsNotBlank() Then
                        Dim param = prop.Parameters.First
                        Dim con = Expression.Constant(s)
                        Dim lk = Expression.Call(prop.Body, containsMethod, con)
                        Dim lbd = Expression.Lambda(Of Func(Of T, Boolean))(lk, param)
                        predi = predi.Or(lbd)
                    End If
                Next
            Next
            Return predi
        End Function

        <Extension()> Function SearchExpression(Of T)(Text As String, ParamArray Properties As Expression(Of Func(Of T, String))()) As Expression(Of Func(Of T, Boolean))
            Return SearchExpression({Text}, Properties)
        End Function

        ''' <summary>
        ''' Cria uma <see cref="Expression"/> condicional a partir de uma outra expressão
        ''' </summary>
        ''' <typeparam name="T">Tipo do objeto</typeparam>
        ''' <param name="predicate">Valor padrão</param>
        ''' <returns></returns>
        Function CreateWhereExpression(Of T)(predicate As Expression(Of Func(Of T, Boolean))) As Expression(Of Func(Of T, Boolean))
            Return If(predicate, CreateWhereExpression(Of T)(False))
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
            Return source.Select(Function(item, index) New With {item, Key .Page = index / PageSize}).GroupBy(Function(g) g.Page.FloorLong() + 1, Function(x) x.item).ToDictionary()
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
            Return Properties.NullAsEmpty().All(Function(x) x(Obj1).Equals(x(Obj2)))
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
            Return items.ThenByLike(PropertySelector, Ascending, Searches)
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
            Return If(PageNumber <= 0, Source, Source.Skip((PageNumber - 1) * PageSize).Take(PageSize))
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
            If PageNumber <= 0 Then
                Return Source
            End If
            Return Source.Skip((PageNumber - 1).SetMinValue(0) * PageSize).Take(PageSize)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="IQueryable(Of ClassType)"/> procurando em varios campos diferentes de uma entidade
        ''' </summary>
        ''' <typeparam name="ClassType">Tipo da Entidade</typeparam>
        ''' <param name="Table">Tabela da Entidade</param>
        ''' <param name="SearchTerms">Termos da pesquisa</param>
        ''' <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        ''' <returns></returns>
        <Extension()> Public Function Search(Of ClassType As Class)(ByVal Table As IEnumerable(Of ClassType), SearchTerms As IEnumerable(Of String), ParamArray Properties() As Expression(Of Func(Of ClassType, String))) As IOrderedEnumerable(Of ClassType)
            Properties = If(Properties, {})
            SearchTerms = SearchTerms.NullAsEmpty()
            Search = Nothing
            Table = Table.Where(SearchExpression(SearchTerms, Properties).Compile())
            For Each prop In Properties
                Search = If(Search, Table.OrderBy(Function(x) True)).ThenByLike(prop.Compile(), True, SearchTerms.ToArray())
            Next
            Return Search
        End Function

        ''' <summary>
        ''' Retorna um <see cref="IQueryable(Of ClassType)"/> procurando em varios campos diferentes de uma entidade
        ''' </summary>
        ''' <typeparam name="ClassType">Tipo da Entidade</typeparam>
        ''' <param name="Table">Tabela da Entidade</param>
        ''' <param name="SearchTerms">Termos da pesquisa</param>
        ''' <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        ''' <returns></returns>
        <Extension()> Public Function Search(Of ClassType As Class)(Table As IQueryable(Of ClassType), SearchTerms As IEnumerable(Of String), ParamArray Properties() As Expression(Of Func(Of ClassType, String))) As IOrderedQueryable(Of ClassType)
            Properties = If(Properties, {})
            SearchTerms = SearchTerms.NullAsEmpty()
            Search = Table.Where(SearchExpression(SearchTerms, Properties)).OrderBy(Function(x) True)
            For Each prop In Properties
                Search = Search.ThenByLike(SearchTerms, prop)
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
        Public Iterator Function OrderByWithPriority(Of T, DefaultOrderType)(ByVal items As IEnumerable(Of T), ByVal Priority As Func(Of T, Boolean), Optional DefaultOrder As Func(Of T, DefaultOrderType) = Nothing) As IEnumerable(Of T)
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
        Public Function ThenByProperty(Of T)(ByVal source As IQueryable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Dim type = GetType(T)
            SortProperty = If(SortProperty, {})
            For Each prop In SortProperty
                Dim [property] = type.GetProperty(prop)
                Dim parameter = Expression.Parameter(type, "p")
                Dim propertyAccess = Expression.MakeMemberAccess(parameter, [property])
                Dim orderByExp = Expression.Lambda(propertyAccess, parameter)
                Dim typeArguments = New Type() {type, [property].PropertyType}
                Dim methodname = "OrderBy"
                If Not source.GetType() Is GetType(IOrderedQueryable(Of T)) Then
                    methodname = If(Array.IndexOf(SortProperty, prop) > 0, If(Ascending, "ThenBy", "ThenByDescending"), If(Ascending, "OrderBy", "OrderByDescending"))
                Else
                    methodname = If(Ascending, "ThenBy", "ThenByDescending")
                End If
                Dim resultExp = Expression.[Call](GetType(Queryable), methodname, typeArguments, source.Expression, Expression.Quote(orderByExp))
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
        Public Function ThenByProperty(Of T)(ByVal source As IEnumerable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedEnumerable(Of T)
            Dim type = GetType(T)
            SortProperty = If(SortProperty, {})
            For Each prop In SortProperty
                Dim exp = Function(x As T) x.GetPropertyValue(Of Object)(prop)
                If Not source.GetType() Is GetType(IOrderedEnumerable(Of T)) Then
                    source = If(Array.IndexOf(SortProperty, prop) > 0, If(Ascending, CType(source, IOrderedEnumerable(Of T)).ThenBy(exp), CType(source, IOrderedEnumerable(Of T)).ThenByDescending(exp)), If(Ascending, source.OrderBy(exp), source.OrderByDescending(exp)))
                Else
                    source = If(Ascending, CType(source, IOrderedEnumerable(Of T)).ThenBy(exp), CType(source, IOrderedEnumerable(Of T)).ThenByDescending(exp))
                End If
            Next
            Return source
        End Function

        ''' <summary>
        ''' Ordena um <see cref="ienumerable(Of T)"/> a partir de outra lista do mesmo tipo
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Source"></param>
        ''' <param name="OrderSource"></param>
        ''' <returns></returns>
        <Extension()> Public Function ThenByList(Of T)(ByVal Source As IOrderedEnumerable(Of T), ParamArray OrderSource As T()) As IOrderedEnumerable(Of T)
            Return Source.ThenBy(Function(d) Array.IndexOf(OrderSource, d))
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
        <Extension()> Public Function ThenByLike(Of T)(ByVal items As IQueryable(Of T), Searches As String(), SortProperty As String, Optional Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Dim type = GetType(T)
            Searches = If(Searches, {})
            If Searches.Any() Then
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
                            If Not items.GetType() Is GetType(IOrderedQueryable(Of T)) Then
                                items = items.OrderByDescending(nv)
                            Else
                                items = CType(items, IOrderedQueryable(Of T)).ThenByDescending(nv)
                            End If
                        Else
                            If Not items.GetType() Is GetType(IOrderedQueryable(Of T)) Then
                                items = items.OrderBy(nv)
                            Else
                                items = CType(items, IOrderedQueryable(Of T)).ThenBy(nv)
                            End If
                        End If
                    Next

                Next
            Else
                items = items.OrderBy(Function(x) 0)
            End If

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
        <Extension()> Public Function ThenByLike(Of T)(ByVal items As IQueryable(Of T), Searches As String(), SortProperty As Expression(Of Func(Of T, String)), Optional Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Dim type = GetType(T)
            Searches = If(Searches, {})
            If Searches.Any Then
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
                            If Not items.GetType() Is GetType(IOrderedQueryable(Of T)) Then
                                items = items.OrderByDescending(nv)
                            Else
                                items = CType(items, IOrderedQueryable(Of T)).ThenByDescending(nv)
                            End If
                        Else
                            If Not items.GetType() Is GetType(IOrderedQueryable(Of T)) Then
                                items = items.OrderBy(nv)
                            Else
                                items = CType(items, IOrderedQueryable(Of T)).ThenBy(nv)
                            End If
                        End If
                    Next

                Next
            Else
                items = items.OrderBy(Function(x) 0)
            End If
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
        <Extension()> Public Function ThenByLike(Of T As Class)(ByVal items As IEnumerable(Of T), PropertySelector As Func(Of T, String), Ascending As Boolean, ParamArray Searches As String()) As IOrderedEnumerable(Of T)
            Searches = If(Searches, {})
            If Not items.GetType() Is GetType(IOrderedEnumerable(Of T)) Then
                items = items.OrderBy(Function(x) 0)
            End If
            If Searches.Any Then
                For Each t In Searches
                    For Each exp In {
                        Function(x) PropertySelector(x) = t,
                        Function(x) PropertySelector(x).StartsWith(t),
                        Function(x) PropertySelector(x).Contains(t),
                        Function(x) PropertySelector(x).EndsWith(t)
                      }

                        If Ascending Then
                            items = CType(items, IOrderedEnumerable(Of T)).ThenByDescending(exp)
                        Else
                            items = CType(items, IOrderedEnumerable(Of T)).ThenBy(exp)
                        End If
                    Next
                Next
            End If

            Return items
        End Function

        ''' <summary>
        ''' Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        ''' </summary>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension()> Public Function Most(Of T)(List As IEnumerable(Of T), predicate As Func(Of T, Boolean), Optional Result As Boolean = True) As Boolean
            Return List.Select(predicate).Most(Result)
        End Function

        ''' <summary>
        ''' Retorna TRUE se a maioria dos testes em uma lista retornarem true
        ''' </summary>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension()> Public Function MostTrue(Of T)(List As IEnumerable(Of T), predicate As Func(Of T, Boolean)) As Boolean
            Return MostTrue(List.Select(predicate).ToArray())
        End Function

        ''' <summary>
        ''' Retorna TRUE se a maioria dos testes em uma lista retornarem false
        ''' </summary>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension()> Public Function MostFalse(Of T)(List As IEnumerable(Of T), predicate As Func(Of T, Boolean)) As Boolean
            Return MostFalse(List.Select(predicate).ToArray())
        End Function

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