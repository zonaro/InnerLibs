Imports System.ComponentModel
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace LINQ

    Public Module LINQExtensions




        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class)(ByVal List As IEnumerable(Of T)) As PaginationFilter(Of T, T)
            Return New PaginationFilter(Of T, T)(False).SetData(List)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class)(ByVal List As IEnumerable(Of T), Configuration As Action(Of PaginationFilter(Of T, T))) As PaginationFilter(Of T, T)
            Return New PaginationFilter(Of T, T)(False).SetData(List).Config(Configuration)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class)(ByVal List As IEnumerable(Of T), Exclusive As Boolean) As PaginationFilter(Of T, T)
            Return New PaginationFilter(Of T, T)(Exclusive).SetData(List)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class)(ByVal List As IEnumerable(Of T), Configuration As Action(Of PaginationFilter(Of T, T)), Exclusive As Boolean) As PaginationFilter(Of T, T)
            Return New PaginationFilter(Of T, T)(Exclusive).SetData(List).Config(Configuration)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class, R As Class)(ByVal List As IEnumerable(Of T), RemapSelector As Func(Of T, R), Optional Exclusive As Boolean = False) As PaginationFilter(Of T, R)
            Return New PaginationFilter(Of T, R)(RemapSelector, Exclusive).SetData(List)
        End Function

        ''' <summary>
        ''' Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="List"></param>
        ''' <returns></returns>
        <Extension>
        Public Function CreateFilter(Of T As Class, R As Class)(ByVal List As IEnumerable(Of T), RemapSelector As Func(Of T, R), Configuration As Action(Of PaginationFilter(Of T, T)), Optional Exclusive As Boolean = False) As PaginationFilter(Of T, R)
            Return New PaginationFilter(Of T, R)(RemapSelector, Exclusive).SetData(List).Config(Configuration)
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
        Public Function WhereExpression(Of Type)(PropertyName As String, [Operator] As String, PropertyValue As IEnumerable(Of IComparable), Optional [Is] As Boolean = True, Optional Exclusive As Boolean = True) As Expression(Of Func(Of Type, Boolean))
            Dim parameter = Expression.Parameter(GetType(Type), GetType(Type).Name.ToLower())
            Dim member = Expression.[Property](parameter, PropertyName)
            Dim body As Expression = GetOperatorExpression(member, [Operator], PropertyValue, Exclusive)
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

        Public Function CreateNullable(ByVal type As Type) As Type
            If type.IsValueType AndAlso (Not type.IsGenericType OrElse type.GetGenericTypeDefinition() <> GetType(Nullable(Of))) Then Return GetType(Nullable(Of)).MakeGenericType(type)
            Return type
        End Function

        Function FixConstant(Of T As IComparable)(Member As Expression, Value As T) As ConstantExpression
            Dim Converter = TypeDescriptor.GetConverter(Member.Type)
            Dim Con = Expression.Constant(Value)
            If Converter.CanConvertFrom(Value.GetType()) Then
                Con = Expression.Constant(Converter.ConvertFrom(Value), Member.Type)
            End If
            Return Con
        End Function

        Function FixConstant(Of T As IComparable)(Value As T, Type As Type) As ConstantExpression
            Dim Converter = TypeDescriptor.GetConverter(Type)
            Dim Con = Expression.Constant(Value)
            If Converter.CanConvertFrom(Value.GetType()) Then
                Con = Expression.Constant(Converter.ConvertFrom(Value), Type)
            End If
            Return Con
        End Function

        Function FixConstant(Of T As IComparable, Type)(Value As T) As ConstantExpression
            Return FixConstant(Value, GetType(Type))
        End Function

        ''' <summary>
        ''' Retorna uma expressão de comparação para um ou mais valores
        ''' </summary>
        ''' <param name="Member"></param>
        ''' <param name="[Operator]"></param>
        ''' <param name="PropertyValues"></param>
        ''' <param name="Exclusive"></param>
        ''' <returns></returns>
        Function GetOperatorExpression(Member As Expression, [Operator] As String, PropertyValues As IEnumerable(Of IComparable), Optional Exclusive As Boolean = False) As BinaryExpression
            PropertyValues = If(PropertyValues, {})
            Dim comparewith As Boolean = Not [Operator].StartsWithAny("!", "not")
            If comparewith = False Then
                [Operator] = [Operator].RemoveFirstAny(False, "!", "Not")
            End If
            Dim body As BinaryExpression = Nothing
            Select Case [Operator].ToLower().IfBlank("equal")
                Case "=", "==", "equal", "===", "equals"
                    For Each item In PropertyValues
                        Dim exp = Expression.Equal(Member, FixConstant(Member, item))
                        exp = Expression.Equal(exp, Expression.Constant(comparewith))
                        If body Is Nothing Then
                            body = exp
                        Else
                            If Exclusive Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                    Next
                Case ">=", "greaterthanorequal", "greaterorequal", "greaterequal", "greatequal"
                    For Each item In PropertyValues
                        Dim exp = Expression.GreaterThanOrEqual(Member, FixConstant(Member, item))
                        exp = Expression.Equal(exp, Expression.Constant(comparewith))
                        If body Is Nothing Then
                            body = exp
                        Else
                            If Exclusive Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                    Next
                Case "<=", "lessthanorequal", "lessorequal", "lessequal"
                    For Each item In PropertyValues
                        Dim exp = Expression.LessThanOrEqual(Member, FixConstant(Member, item))
                        exp = Expression.Equal(exp, Expression.Constant(comparewith))

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Exclusive Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                    Next
                Case ">", "greaterthan", "greater", "great"
                    For Each item In PropertyValues
                        Dim exp = Expression.GreaterThan(Member, FixConstant(Member, item))
                        exp = Expression.Equal(exp, Expression.Constant(comparewith))

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Exclusive Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                    Next
                Case "<", "lessthan", "less"
                    For Each item In PropertyValues
                        Dim exp = Expression.LessThan(Member, FixConstant(Member, item))
                        exp = Expression.Equal(exp, Expression.Constant(comparewith))

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Exclusive Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                    Next
                Case "<>", "notequal", "different"
                    For Each item In PropertyValues
                        Dim exp = Expression.NotEqual(Member, FixConstant(Member, item))
                        exp = Expression.Equal(exp, Expression.Constant(comparewith))

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Exclusive Then
                                body = Expression.AndAlso(body, exp)
                            Else
                                body = Expression.OrElse(body, exp)
                            End If
                        End If
                    Next
                Case "between", "btw"
                    If PropertyValues.Count() > 1 Then
                        Select Case Member.Type
                            Case GetType(String)
                                body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", Not comparewith), {PropertyValues.First()}, Exclusive), GetOperatorExpression(Member, "ends".PrependIf("!", Not comparewith), {PropertyValues.Last()}, Exclusive))
                            Case Else
                                body = Expression.And(GetOperatorExpression(Member, "greaterequal".PrependIf("!", Not comparewith), {PropertyValues.Min()}, Exclusive), GetOperatorExpression(Member, "lessequal".PrependIf("!", Not comparewith), {PropertyValues.Max()}, Exclusive))
                        End Select
                    Else
                        body = GetOperatorExpression(Member, "=".PrependIf("!", Not comparewith), PropertyValues, Exclusive)
                    End If

                Case "starts", "start", "startwith", "startswith"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Expression.Equal(Expression.Call(Member, startsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith))
                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Exclusive Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                            Next
                        Case Else
                            body = GetOperatorExpression(Member, ">=", PropertyValues, Exclusive)
                    End Select
                Case "ends", "end", "endwith", "endswith"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Expression.Equal(Expression.Call(Member, endsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith))
                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Exclusive Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                            Next
                        Case Else
                            body = GetOperatorExpression(Member, "lessequal".PrependIf("!", Not comparewith), PropertyValues, Exclusive)
                    End Select
                Case "like", "contains"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Expression.Equal(Expression.Call(Member, containsMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith))
                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Exclusive Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                            Next
                        Case Else
                            body = GetOperatorExpression(Member, "equal".PrependIf("!", Not comparewith), PropertyValues, Exclusive)
                    End Select
                Case "cross", "crosscontains"
                    Select Case Member.Type
                        Case GetType(String)
                            For Each item In PropertyValues
                                Dim exp = Expression.Equal(Expression.OrElse(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Call(Member, containsMethod, Expression.Constant(item.ToString()))), Expression.Constant(comparewith))
                                If body Is Nothing Then
                                    body = exp
                                Else
                                    If Exclusive Then
                                        body = Expression.AndAlso(body, exp)
                                    Else
                                        body = Expression.OrElse(body, exp)
                                    End If
                                End If
                            Next
                        Case Else
                            ''TODO: implementar busca de array de inteiro,data etc
                            body = GetOperatorExpression(Member, "equal".PrependIf("!", Not comparewith), PropertyValues, Exclusive)
                    End Select
                Case Else
                    Try
                        Dim mettodo = Member.Type.GetMethods().FirstOrDefault(Function(x) x.Name.ToLower() = [Operator].ToLower())

                        Dim exp As Expression = mettodo.Invoke(Nothing, {PropertyValues})
                        exp = Expression.Invoke(exp, Member)
                        exp = Expression.Equal(exp, Expression.Constant(comparewith))

                        If body Is Nothing Then
                            body = exp
                        Else
                            If Exclusive Then
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

        Function GenerateParameterExpression(Of ClassType)(Optional AppendText As String = "") As ParameterExpression
            Return GetType(ClassType).GenerateParameterExpression(AppendText)
        End Function

        <Extension> Function GenerateParameterExpression(Type As Type, Optional AppendText As String = "") As ParameterExpression
            Return Expression.Parameter(Type, Type.Name.CamelSplit.SelectJoin(Function(x) x.FirstOrDefault().IfBlank(Of Char)(""), "").ToLower() + AppendText.IfBlank(""))
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
        ''' <param name="Items">Itens</param>
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

        ''' <summary> Concatena uma expressão com outra usando o operador AND (&&) </summary>
        ''' <typeparam name="T"></typeparam><param name="expr1"></param><param
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
        <Extension()> Public Function DoForEach(Of T)(Items As IEnumerable(Of T), Action As Action(Of T)) As IEnumerable(Of T)
            For Each item In Items
                Action(item)
            Next
            Return Items
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
        <Extension> Public Function OrderByProperty(Of T)(ByVal source As IQueryable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Return source.OrderBy(Function(x) True).ThenByProperty(SortProperty, Ascending)
        End Function

        ''' <summary>
        ''' Ordena um <see cref="IQueryable(Of T)"/> a partir do nome de uma ou mais propriedades
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="source">      </param>
        ''' <param name="sortProperty"></param>
        ''' <param name="Ascending">   </param>
        ''' <returns></returns>
        <Extension> Public Function OrderByProperty(Of T)(ByVal source As IEnumerable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedEnumerable(Of T)
            Return source.OrderBy(Function(x) True).ThenByProperty(SortProperty, Ascending)
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
        ''' Ordena um <see cref="ienumerable(Of T)"/> a partir de outra lista do mesmo tipo
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Source"></param>
        ''' <param name="OrderSource"></param>
        ''' <returns></returns>
        <Extension()> Public Function OrderByList(Of T)(ByVal Source As IOrderedEnumerable(Of T), ParamArray OrderSource As T()) As IOrderedEnumerable(Of T)
            Return Source.OrderBy(Function(x) True).ThenByList(OrderSource)
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
            If PageNumber <= 0 Then
                Return Source
            End If

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
        <Extension()> Public Function Search(Of ClassType As Class)(ByVal Table As IEnumerable(Of ClassType), SearchTerms As String(), ParamArray Properties() As Func(Of ClassType, String)) As IOrderedEnumerable(Of ClassType)
            Properties = If(Properties, {})
            'If Properties.Count = 0 Then
            '    Properties = GetType(ClassType).GetProperties().Where(Function(x) x.PropertyType = GetType(String)).Select(Function(x) Expression.Property(param, x.Name))
            'End If
            Search = Nothing
            Dim predi = CreateExpression(Of ClassType)(False)
            For Each prop In Properties
                For Each s In SearchTerms
                    If Not s = Nothing AndAlso s.IsNotBlank() Then
                        predi = predi.Or(Function(x) prop(x).Contains(s))
                    End If
                Next
            Next
            Table = Table.Where(predi.Compile())
            For Each prop In Properties
                Search = If(Search, Table.OrderBy(Function(x) True)).ThenByLike(prop, True, SearchTerms)
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
        <Extension()> Public Function Search(Of ClassType As Class)(Table As IQueryable(Of ClassType), SearchTerms As String(), ParamArray Properties() As Expression(Of Func(Of ClassType, String))) As IOrderedQueryable(Of ClassType)
            Properties = If(Properties, {})
            If Properties.Count = 0 Then
                Dim s As String() = {}
                'Return Table.Search(SearchTerms, s) 'TODO implementar esse
            End If
            Search = Nothing
            Dim tab = Table.AsQueryable
            Dim predi = CreateExpression(Of ClassType)(False)
            For Each prop In Properties
                For Each s In SearchTerms
                    If Not s = Nothing AndAlso s.IsNotBlank() Then
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
        Public Function ThenByProperty(Of T)(ByVal source As IOrderedQueryable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedQueryable(Of T)
            Dim type = GetType(T)
            For Each prop In SortProperty
                Dim [property] = type.GetProperty(prop)
                Dim parameter = Expression.Parameter(type, "p")
                Dim propertyAccess = Expression.MakeMemberAccess(parameter, [property])
                Dim orderByExp = Expression.Lambda(propertyAccess, parameter)
                Dim typeArguments = New Type() {type, [property].PropertyType}
                Dim methodName = If(Array.IndexOf(SortProperty, prop) > 0, If(Ascending, "ThenBy", "ThenByDescending"), If(Ascending, "OrderBy", "OrderByDescending"))
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
        Public Function ThenByProperty(Of T)(ByVal source As IOrderedEnumerable(Of T), ByVal SortProperty As String(), Optional ByVal Ascending As Boolean = True) As IOrderedEnumerable(Of T)
            Dim type = GetType(T)
            For Each prop In SortProperty
                Dim exp = Function(x As T) x.GetPropertyValue(Of Object)(prop)
                source = source.ThenBy(exp)
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