using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs.LINQ
{
    public static class LINQExtensions
    {
        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> l, int Count)
        {
            return l.OrderByRandom().Take(Count);
        }

        public static T FirstRandom<T>(this IEnumerable<T> l)
        {
            return l.OrderByRandom().FirstOrDefault();
        }

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, T> CreateFilter<T>(this IEnumerable<T> List) where T : class
        {
            return new PaginationFilter<T, T>().SetData(List);
        }

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, T> CreateFilter<T>(this IEnumerable<T> List, Action<PaginationFilter<T, T>> Configuration) where T : class
        {
            return new PaginationFilter<T, T>(Configuration).SetData(List);
        }

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, R> CreateFilter<T, R>(this IEnumerable<T> List, Func<T, R> RemapExpression, Action<PaginationFilter<T, R>> Configuration) where T : class
        {
            return new PaginationFilter<T, R>(RemapExpression, Configuration).SetData(List);
        }

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, R> CreateFilter<T, R>(this IEnumerable<T> List, Func<T, R> RemapExpression) where T : class
        {
            return new PaginationFilter<T, R>(RemapExpression).SetData(List);
        }

        public static Expression PropertyExpression(this ParameterExpression Parameter, string PropertyName)
        {
            Expression prop = Parameter;
            if (PropertyName.IfBlank("this") != "this")
            {
                foreach (var name in PropertyName.SplitAny(".", "/"))
                    prop = Expression.Property(prop, name);
            }

            return prop;
        }

        /// <summary>
        /// Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto acessado</typeparam>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="Type"/></param>
        /// <param name="[Operator]">Operador ou método do objeto <typeparamref name="Type"/> que retorna um <see cref="Boolean"/></param>
        /// <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="Type"/></param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static Expression<Func<Type, bool>> WhereExpression<Type>(string PropertyName, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, FilterConditional Conditional = FilterConditional.Or)
        {
            var parameter = GenerateParameterExpression<Type>();
            var member = parameter.PropertyExpression(PropertyName);
            Expression body = GetOperatorExpression(member, Operator, PropertyValue, Conditional);
            body = Expression.Equal(body, Expression.Constant(Is));
            var finalExpression = Expression.Lambda<Func<Type, bool>>(body, parameter);
            return finalExpression;
        }

        public static Expression<Func<Type, bool>> WhereExpression<Type, V>(Expression<Func<Type, V>> PropertySelector, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, FilterConditional Conditional = FilterConditional.Or)
        {
            var parameter = GenerateParameterExpression<Type>();
            string member = PropertySelector.Body.ToString().Split(".").Skip(1).JoinString(".");
            var prop = parameter.PropertyExpression(member);
            Expression body = GetOperatorExpression(prop, Operator, PropertyValue, Conditional);
            body = Expression.Equal(body, Expression.Constant(Is));
            var finalExpression = Expression.Lambda<Func<Type, bool>>(body, parameter);
            return finalExpression;
        }

        /// <summary>
        /// Busca em um <see cref="IQueryable(Of T)"/> usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/></param>
        /// <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="T"/></param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static IQueryable<T> WhereExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, bool Exclusive = true)
        {
            return List.Where(WhereExpression<T>(PropertyName, Operator, PropertyValue, Is));
        }

        public static Type CreateNullableType(this Type type)
        {
            if (type.IsValueType && (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(object)))
                return typeof(object).MakeGenericType(type);
            return type;
        }

        public static IQueryable<T> FilterDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime>> Property, DateRange Range)
        {
            return List.Where(Property.IsBetween(Range.StartDate, Range.EndDate));
        }

        public static IQueryable<T> FilterDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime?>> Property, DateRange Range)
        {
            return List.Where(Property.IsBetween(Range.StartDate, Range.EndDate));
        }

        public static IQueryable<T> FilterDateRange<T, V>(this IQueryable<T> List, Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, IEnumerable<V> Values)
        {
            return List.Where(MinProperty.IsBetween(MaxProperty, Values));
        }

        public static IQueryable<T> FilterDateRange<T, V>(this IQueryable<T> List, Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, params V[] Values)
        {
            return List.Where(MinProperty.IsBetween(MaxProperty, Values));
        }

        public static Expression<Func<T, bool>> IsBetween<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, IEnumerable<V> Values)
        {
            var exp = false.CreateWhereExpression<T>();
            foreach (var item in Values ?? Array.Empty<V>())
                exp = exp.Or(WhereExpression(MinProperty, "<=", new[] { (IComparable)item }).And(WhereExpression(MaxProperty, ">=", new[] { (IComparable)item })));
            return exp;
        }

        public static Expression<Func<T, bool>> IsBetween<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, params V[] Values)
        {
            return MinProperty.IsBetween(MaxProperty, Values.AsEnumerable());
        }

        public static Expression<Func<T, bool>> IsBetween<T, V>(this Expression<Func<T, V>> Property, V MinValue, V MaxValue)
        {
            return WhereExpression(Property, "between", new[] { (IComparable)MinValue, (IComparable)MaxValue });
        }

        public static Expression<Func<T, bool>> IsBetween<T>(this Expression<Func<T, DateTime>> Property, DateRange DateRange)
        {
            return WhereExpression(Property, "between", new[] { DateRange.StartDate, (IComparable)DateRange.EndDate });
        }

        public static Expression<Func<T, bool>> IsBetween<T>(this Expression<Func<T, DateTime?>> Property, DateRange DateRange)
        {
            return WhereExpression(Property, "between", new[] { (DateTime?)DateRange.StartDate, (IComparable)(DateTime?)DateRange.EndDate });
        }

        public static MemberExpression CreatePropertyExpression<T, V>(this Expression<Func<T, V>> Property)
        {
            return Expression.Property(Property.Parameters.FirstOrDefault() ?? typeof(T).GenerateParameterExpression(), Property.GetPropertyInfo());
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);
            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member is null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda.ToString()));
            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo is null)
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()));
            return propInfo;
        }

        public static void FixNullable(ref Expression e1, ref Expression e2)
        {
            var e1type = e1.Type;
            var e2type = e2.Type;
            try
            {
                e1type = ((LambdaExpression)e1).ReturnType;
            }
            catch
            {
            }

            try
            {
                e2type = ((LambdaExpression)e2).ReturnType;
            }
            catch
            {
            }

            if (e1type.IsNullableType() && !e2type.IsNullableType())
            {
                e2 = Expression.Convert(e2, e1type);
            }

            if (!e1type.IsNullableType() && e2type.IsNullableType())
            {
                e1 = Expression.Convert(e1, e2type);
            }

            if (e1.NodeType == ExpressionType.Lambda)
            {
                e1 = Expression.Invoke(e1, ((LambdaExpression)e1).Parameters);
            }

            if (e2.NodeType == ExpressionType.Lambda)
            {
                e2 = Expression.Invoke(e2, ((LambdaExpression)e2).Parameters);
            }
        }

        /// <summary>
        /// Constroi uma expressão Maior ou Igual
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression GreaterThanOrEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.GreaterThanOrEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Constroi uma expressão Menor ou igual
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression LessThanOrEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.LessThanOrEqual(MemberExpression, ValueExpression);
        }
        /// <summary>
        /// Constroi uma expressão Maior que
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression GreaterThan(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.GreaterThan(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Constroi uma expressão Menor que
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression LessThan(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.LessThan(MemberExpression, ValueExpression);
        }
        /// <summary>
        /// Constroi uma expressão igual a
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression Equal(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.Equal(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Constroi uma expressão diferente de
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression NotEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.NotEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Cria uma constante a partir de um valor para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static ConstantExpression CreateConstant(Expression Member, IComparable Value) => CreateConstant(Member.Type, Value);


        /// <summary>
        /// Cria uma constante a partir de um tipo para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>

        public static ConstantExpression CreateConstant(Type Type, IComparable Value)
        {
            if (Value == null)
            {
                return Expression.Constant(null, Type);
            }

            return Expression.Constant(Value.ChangeType(Type));
        }
        /// <summary>
        /// Cria uma constante a partir de um tipo genérico para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static ConstantExpression CreateConstant<Type>(IComparable Value) => CreateConstant(typeof(Type), Value);

        /// <summary>
        /// Retorna uma expressão de comparação para um ou mais valores e uma ou mais propriedades
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="[Operator]"></param>
        /// <param name="PropertyValues"></param>
        /// <param name="Conditional"></param>
        /// <returns></returns>
        public static BinaryExpression GetOperatorExpression(Expression Member, string Operator, IEnumerable<IComparable> PropertyValues, FilterConditional Conditional = FilterConditional.Or)
        {
            PropertyValues = PropertyValues ?? Array.Empty<IComparable>();
            bool comparewith = !Operator.StartsWithAny("!");
            if (comparewith == false)
            {
                Operator = Operator.RemoveFirstAny(false, "!");
            }

            BinaryExpression body = null;
            // Dim body As Expression = Nothing
            switch (Operator.ToLower().IfBlank("equal") ?? "")
            {
                case "blank":
                case "compareblank":
                case "isblank":
                case "isempty":
                case "empty":
                    {
                        foreach (var item in PropertyValues)
                        {
                            var exp = Expression.Equal(Member, Expression.Constant(string.Empty, Member.Type));
                            if (body is null)
                            {
                                body = exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "isnull":
                case "comparenull":
                case "null":
                case "nothing":
                case "isnothing":
                    {
                        foreach (var item in PropertyValues)
                        {
                            var exp = Expression.Equal(Member, Expression.Constant(null, Member.Type));
                            if (body is null)
                            {
                                body = exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "=":
                case "==":
                case "equal":
                case "===":
                case "equals":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = Member.Equal(CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body is null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case ">=":
                case "greaterthanorequal":
                case "greaterorequal":
                case "greaterequal":
                case "greatequal":
                    {
                        foreach (var ii in PropertyValues)
                        {
                            var item = ii;
                            if (!(item.GetNullableTypeOf() == typeof(DateTime)) && item.IsNotNumber() && item.ToString().IsNotBlank())
                            {
                                item = item.ToString().Length;
                            }

                            object exp = null;
                            try
                            {
                                exp = GreaterThanOrEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body is null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<=":
                case "lessthanorequal":
                case "lessorequal":
                case "lessequal":
                    {
                        foreach (var ii in PropertyValues)
                        {
                            var item = ii;
                            if (!ReferenceEquals(item.GetNullableTypeOf(), typeof(DateTime)) && item.IsNotNumber() && item.ToString().IsNotBlank())
                            {
                                item = item.ToString().Length;
                            }

                            object exp = null;
                            try
                            {
                                exp = LessThanOrEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body is null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case ">":
                case "greaterthan":
                case "greater":
                case "great":
                    {
                        foreach (var item in PropertyValues)
                        {
                            Expression exp = null;
                            try
                            {
                                exp = GreaterThan(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<":
                case "lessthan":
                case "less":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = LessThan(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body is null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<>":
                case "notequal":
                case "different":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = NotEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body is null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "betweenequal":
                case "betweenorequal":
                case "btweq":
                case "=><=":
                    {
                        if (PropertyValues.Count() > 1)
                        {
                            switch (Member.Type)
                            {
                                case var @case when @case == typeof(string):
                                    {
                                        body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", !comparewith), new[] { PropertyValues.First() }, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", !comparewith), new[] { PropertyValues.Last() }, Conditional));
                                        break;
                                    }

                                default:
                                    {
                                        var ge = GetOperatorExpression(Member, "greaterequal".PrependIf("!", !comparewith), new[] { PropertyValues.Min() }, Conditional);
                                        var le = GetOperatorExpression(Member, "lessequal".PrependIf("!", !comparewith), new[] { PropertyValues.Max() }, Conditional);
                                        body = Expression.And(ge, le);
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            body = GetOperatorExpression(Member, "=".PrependIf("!", !comparewith), PropertyValues, Conditional);
                        }

                        break;
                    }

                case "><":
                case "startend":
                case "startends":
                case "btw":
                case "between":
                    {
                        if (PropertyValues.Count() > 1)
                        {
                            switch (Member.Type)
                            {
                                case var case1 when case1 == typeof(string):
                                    {
                                        body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", !comparewith), new[] { PropertyValues.First() }, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", !comparewith), new[] { PropertyValues.Last() }, Conditional));
                                        break;
                                    }

                                default:
                                    {
                                        body = Expression.And(GetOperatorExpression(Member, "greater".PrependIf("!", !comparewith), new[] { PropertyValues.Min() }, Conditional), GetOperatorExpression(Member, "less".PrependIf("!", !comparewith), new[] { PropertyValues.Max() }, Conditional));
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            body = GetOperatorExpression(Member, "=".PrependIf("!", !comparewith), PropertyValues, Conditional);
                        }

                        break;
                    }

                case "starts":
                case "start":
                case "startwith":
                case "startswith":
                    {
                        switch (Member.Type)
                        {
                            case var case2 when case2 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, startsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body is null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, ">=", PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "ends":
                case "end":
                case "endwith":
                case "endswith":
                    {
                        switch (Member.Type)
                        {
                            case var case3 when case3 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, endsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body is null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, "lessequal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "like":
                case "contains":
                    {
                        switch (Member.Type)
                        {
                            case var case4 when case4 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, containsMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body is null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "isin":
                case "inside":
                    {
                        switch (Member.Type)
                        {
                            case var case5 when case5 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body is null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    // 'TODO: implementar busca de array de inteiro,data etc
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "cross":
                case "crosscontains":
                case "insidecontains":
                    {
                        switch (Member.Type)
                        {
                            case var case6 when case6 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.OrElse(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Call(Member, containsMethod, Expression.Constant(item.ToString()))), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body is null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    //TODO: implementar busca de array de inteiro,data etc
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                default: // Executa um metodo com o nome definido pelo usuario que retorna uma expression compativel
                    {
                        try
                        {
                            var metodo = Member.Type.GetMethods().FirstOrDefault(x => (x.Name.ToLower() ?? "") == (Operator.ToLower() ?? ""));
                            Expression exp = (Expression)metodo.Invoke(null, new[] { PropertyValues });
                            exp = Expression.Equal(Expression.Invoke(exp, new[] { Member }), Expression.Constant(comparewith));
                            if (body is null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }
                        }
                        catch
                        {
                        }

                        break;
                    }
            }

            return body;
        }

        /// <summary>
        /// Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        /// </summary>
        /// <returns></returns>
        public static ParameterExpression GenerateParameterExpression<ClassType>() => typeof(ClassType).GenerateParameterExpression();

        /// <summary>
        /// Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static ParameterExpression GenerateParameterExpression(this Type Type) => Expression.Parameter(Type, Type.GenerateParameterName());

        public static string GenerateParameterName(this Type Type)
        {
            if (Type != null)
            {
                return Type.Name.CamelSplit().SelectJoinString(x => x.FirstOrDefault().IfBlank(""), "").ToLower();
            }

            return "p";
        }

        /// <summary>
        /// Busca em um <see cref="IQueryable(Of T)"/> usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/></param>
        /// <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="T"/></param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static T FirstOrDefaultExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, object PropertyValue, bool Is = true) => List.FirstOrDefault(WhereExpression<T>(PropertyName, Operator, (IEnumerable<IComparable>)PropertyValue, Is));

        /// <summary>
        /// Busca em um <see cref="IQueryable(Of T)"/> usando uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/></param>
        /// <param name="PropertyValue">Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro argumento do método de mesmo nome definido em <typeparamref name="T"/></param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static T SingleOrDefaultExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, object PropertyValue, bool Is = true)
        {
            return List.SingleOrDefault(WhereExpression<T>(PropertyName, Operator, (IEnumerable<IComparable>)PropertyValue, Is));
        }

        /// <summary>
        /// Retorna as informacoes de uma propriedade a partir de um seletor
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);
            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda.ToString()));
            if (!(member.Member is PropertyInfo propInfo))
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()));
            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format("Expression '{0}' refers to a property that is not from type {1}.", propertyLambda.ToString(), type));
            return propInfo;
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Items">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> ChildSelector)
        {
            var stack = new Stack<T>(items);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in ChildSelector(next))
                    stack.Push(child);
            }
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ParentSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this T item, Func<T, T> ParentSelector)
        {
            if (item != null)
            {
                var current = item;
                do
                {
                    yield return current;
                    current = ParentSelector(current);
                }
                while (current != null);
            }
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this T Item, Func<T, IEnumerable<T>> ChildSelector, bool IncludeMe = false)
        {
            return ChildSelector(Item).Union(IncludeMe ? (new[] { Item }) : Array.Empty<T>()).Traverse(ChildSelector);
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, P> PropertySelector, bool IncludeMe = false)
        {
            return Item.Traverse(ChildSelector, IncludeMe).Select(PropertySelector);
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, IEnumerable<P>> PropertySelector, bool IncludeMe = false)
        {
            return Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector);
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, IQueryable<P>> PropertySelector, bool IncludeMe = false)
        {
            return Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector);
        }

        private static MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
        private static MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
        private static MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
        private static MethodInfo equalMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });

        /// <summary>
        /// Cria uma <see cref="Expression"/> condicional a partir de um valor <see cref="Boolean"/>
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="DefaultReturnValue">Valor padrão</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateWhereExpression<T>(this bool DefaultReturnValue)
        {
            return f => DefaultReturnValue;
        }

        /// <summary>
        /// Concatena uma expressão com outra usando o operador And (&&)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstExpression"></param>
        /// <param name="OtherExpressions"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> FirstExpression, params Expression<Func<T, bool>>[] OtherExpressions)
        {
            FirstExpression = FirstExpression ?? true.CreateWhereExpression<T>();
            foreach (var item in OtherExpressions ?? Array.Empty<Expression<Func<T, bool>>>())
            {
                if (item != null)
                {
                    var invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast<Expression>());
                    FirstExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(FirstExpression.Body, invokedExpr), FirstExpression.Parameters);
                }
            }

            return FirstExpression;
        }

        /// <summary>
        /// Concatena uma expressão com outra usando o operador OR (||)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstExpression"></param>
        /// <param name="OtherExpressions"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> FirstExpression, params Expression<Func<T, bool>>[] OtherExpressions)
        {
            FirstExpression ??= false.CreateWhereExpression<T>();
            foreach (var item in OtherExpressions ?? Array.Empty<Expression<Func<T, bool>>>())
            {
                if (item != null)
                {
                    var invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast<Expression>());
                    FirstExpression = Expression.Lambda<Func<T, bool>>(Expression.OrElse(FirstExpression.Body, invokedExpr), FirstExpression.Parameters);
                }
            }

            return FirstExpression;
        }

        public static Expression<Func<T, bool>> OrSearch<T>(this Expression<Func<T, bool>> FirstExpression, IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
        {
            return (FirstExpression ?? false.CreateWhereExpression<T>()).Or(Text.SearchExpression(Properties));
        }

        public static Expression<Func<T, bool>> OrSearch<T>(this Expression<Func<T, bool>> FirstExpression, string Text, params Expression<Func<T, string>>[] Properties)
        {
            return (FirstExpression ?? false.CreateWhereExpression<T>()).Or(Text.SearchExpression(Properties));
        }

        public static Expression<Func<T, bool>> AndSearch<T>(this Expression<Func<T, bool>> FirstExpression, IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
        {
            return (FirstExpression ?? true.CreateWhereExpression<T>()).And(Text.SearchExpression(Properties));
        }

        public static Expression<Func<T, bool>> AndSearch<T>(this Expression<Func<T, bool>> FirstExpression, string Text, params Expression<Func<T, string>>[] Properties)
        {
            return (FirstExpression ?? true.CreateWhereExpression<T>()).And(Text.SearchExpression(Properties));
        }

        public static Expression<Func<T, bool>> SearchExpression<T>(this IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
        {
            Properties ??= Array.Empty<Expression<Func<T, string>>>();
            var predi = false.CreateWhereExpression<T>();
            foreach (var prop in Properties)
            {
                foreach (var s in Text)
                {
                    if (s != default && s.IsNotBlank())
                    {
                        var param = prop.Parameters.First();
                        var con = Expression.Constant(s);
                        var lk = Expression.Call(prop.Body, containsMethod, con);
                        var lbd = Expression.Lambda<Func<T, bool>>(lk, param);
                        predi = predi.Or(lbd);
                    }
                }
            }

            return predi;
        }

        public static Expression<Func<T, bool>> SearchExpression<T>(this string Text, params Expression<Func<T, string>>[] Properties)
        {
            return (new[] { Text }).SearchExpression(Properties);
        }

        /// <summary>
        /// Cria uma <see cref="Expression"/> condicional a partir de uma outra expressão
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="predicate">Valor padrão</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateWhereExpression<T>(Expression<Func<T, bool>> predicate)
        {
            return predicate ?? false.CreateWhereExpression<T>();
        }

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">     Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <param name="OrderBy">Criterio que indica qual o objeto que deverá ser preservado na lista se encontrado mais de um</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey, TOrder>(this IEnumerable<T> Items, Func<T, TKey> Property, Func<T, TOrder> OrderBy, bool Descending = false)
        {
            return Items.GroupBy(Property).Select(x => (Descending ? x.OrderByDescending(OrderBy) : x.OrderBy(OrderBy)).FirstOrDefault());
        }

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">     Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> Items, Func<T, TKey> Property)
        {
            return Items.GroupBy(Property).Select(x => x.FirstOrDefault());
        }

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">     Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <returns></returns>
        public static IQueryable<T> DistinctBy<T, TKey>(this IQueryable<T> Items, Expression<Func<T, TKey>> Property)
        {
            return Items.GroupBy(Property).Select(x => x.First());
        }

        /// <summary>
        /// Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        /// </summary>
        /// <typeparam name="Tsource"></typeparam>
        /// <param name="source">  </param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Dictionary<long, IEnumerable<Tsource>> GroupByPage<Tsource>(this IQueryable<Tsource> source, int PageSize)
        {
            return source.AsEnumerable().GroupByPage(PageSize);
        }

        /// <summary>
        /// Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        /// </summary>
        /// <typeparam name="Tsource"></typeparam>
        /// <param name="source">  </param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Dictionary<long, IEnumerable<Tsource>> GroupByPage<Tsource>(this IEnumerable<Tsource> source, int PageSize)
        {
            PageSize = PageSize.SetMinValue(1);
            return source.Select((item, index) => new { item, Page = index / (double)PageSize }).GroupBy(g => g.Page.FloorLong() + 1L, x => x.item).ToDictionary();
        }

        /// <summary>
        /// Orderna uma lista a partir da aproximação de um deerminado campo com uma string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">           </param>
        /// <param name="PropertySelector"></param>
        /// <param name="Ascending">       </param>
        /// <param name="Searches">        </param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, params string[] Searches) where T : class => items.ThenByLike(PropertySelector, Ascending, Searches);

        /// <summary>
        /// Randomiza a ordem de um <see cref="IEnumerable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByRandom<T>(this IEnumerable<T> items) => items.OrderBy(x => Guid.NewGuid());

        /// <summary>
        /// Randomiza a ordem de um <see cref="IQueryable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByRandom<T>(this IQueryable<T> items) => items.OrderBy(x => Guid.NewGuid());

        /// <summary>
        /// Reduz um <see cref="IQueryable"/> em uma página especifica
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source">    </param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize">  </param>
        /// <returns></returns>
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> Source, int PageNumber, int PageSize) => PageNumber <= 0 ? Source : Source.Skip((PageNumber - 1) * PageSize).Take(PageSize);

        /// <summary>
        /// Reduz um <see cref="IEnumerable"/> em uma página especifica
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source">    </param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize">  </param>
        /// <returns></returns>
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> Source, int PageNumber, int PageSize)
        {
            if (PageNumber <= 0)
            {
                return Source;
            }

            return Source.Skip((PageNumber - 1).SetMinValue(0) * PageSize).Take(PageSize);
        }

        /// <summary>
        /// Retorna um <see cref="IQueryable(Of ClassType)"/> procurando em varios campos diferentes de uma entidade
        /// </summary>
        /// <typeparam name="ClassType">Tipo da Entidade</typeparam>
        /// <param name="Table">Tabela da Entidade</param>
        /// <param name="SearchTerms">Termos da pesquisa</param>
        /// <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        /// <returns></returns>
        public static IOrderedEnumerable<ClassType> Search<ClassType>(this IEnumerable<ClassType> Table, IEnumerable<string> SearchTerms, params Expression<Func<ClassType, string>>[] Properties) where ClassType : class
        {
            IOrderedEnumerable<ClassType> SearchRet = default;
            Properties ??= Array.Empty<Expression<Func<ClassType, string>>>();
            SearchTerms ??= Array.Empty<string>().AsEnumerable();
            SearchRet = null;
            Table = Table.Where(SearchTerms.SearchExpression(Properties).Compile());
            foreach (var prop in Properties)
                SearchRet = (SearchRet ?? Table.OrderBy(x => true)).ThenByLike(prop.Compile(), true, SearchTerms.ToArray());
            return SearchRet;
        }

        /// <summary>
        /// Retorna um <see cref="IQueryable(Of ClassType)"/> procurando em varios campos diferentes de uma entidade
        /// </summary>
        /// <typeparam name="ClassType">Tipo da Entidade</typeparam>
        /// <param name="Table">Tabela da Entidade</param>
        /// <param name="SearchTerms">Termos da pesquisa</param>
        /// <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        /// <returns></returns>
        public static IOrderedQueryable<ClassType> Search<ClassType>(this IQueryable<ClassType> Table, IEnumerable<string> SearchTerms, params Expression<Func<ClassType, string>>[] Properties) where ClassType : class
        {
            IOrderedQueryable<ClassType> SearchRet = default;
            Properties ??= Array.Empty<Expression<Func<ClassType, string>>>();
            SearchTerms ??= Array.Empty<string>().AsEnumerable();
            SearchRet = Table.Where(SearchTerms.SearchExpression(Properties)).OrderBy(x => true);
            foreach (var prop in Properties)
                SearchRet = SearchRet.ThenByLike((string[])SearchTerms, prop);
            return SearchRet;
        }

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source">   </param>
        /// ''' <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectJoinString<TSource>(this IEnumerable<TSource> Source, string Separator = "") => Source.SelectJoinString(null, Separator);

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source">   </param>
        /// <param name="Selector"> </param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectJoinString<TSource>(this IEnumerable<TSource> Source, Func<TSource, string> Selector = null, string Separator = "")
        {
            Selector ??= (x => x.ToString());
            return Source.Select(Selector).JoinString(Separator);
        }

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source">   </param>
        /// <param name="Selector"> </param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectJoinString<TSource>(this IQueryable<TSource> Source, Func<TSource, string> Selector = null, string Separator = "") => Source.AsEnumerable().SelectJoinString(Selector, Separator);

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos enumeraveis
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source">   </param>
        /// <param name="Selector"> </param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectManyJoinString<TSource>(this IEnumerable<TSource> Source, Func<TSource, IEnumerable<string>> Selector = null, string Separator = "")
        {
            Selector ??= (x => new[] { x.ToString() });
            return Source.SelectMany(Selector).JoinString(Separator);
        }

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos enumeraveis
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source">   </param>
        /// <param name="Selector"> </param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectManyJoinString<TSource>(this IQueryable<TSource> Source, Func<TSource, IEnumerable<string>> Selector = null, string Separator = "") => Source.AsEnumerable().SelectManyJoinString(Selector, Separator);

        /// <summary>
        /// Ordena um <see cref="IEnumerable"/> priorizando valores especificos a uma condição no
        /// inicio da coleção e então segue uma ordem padrão para os outros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="DefaultOrderType"></typeparam>
        /// <param name="items">       colecao</param>
        /// <param name="Priority">    Seletor que define a prioridade</param>
        /// <param name="DefaultOrder">ordenacao padrao para os outros itens</param>
        /// <returns></returns>
        public static IEnumerable<T> OrderByWithPriority<T, DefaultOrderType>(this IEnumerable<T> items, Func<T, bool> Priority, Func<T, DefaultOrderType> DefaultOrder = null)
        {
            if (DefaultOrder == null)
            {
                foreach (var item in items) yield return item;
            }
            else
            {
                foreach (var item in items.Where(Priority)) yield return item;
                foreach (var item in items.Where(i => !Priority(i)).OrderBy(i => i)) yield return item;
            }
        }



        /// <summary>
        /// Ordena um <see cref="IQueryable(Of T)"/> a partir do nome de uma ou mais propriedades
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">      </param>
        /// <param name="sortProperty"></param>
        /// <param name="Ascending">   </param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByProperty<T>(this IQueryable<T> source, string[] SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            SortProperty = SortProperty ?? Array.Empty<string>();
            foreach (var prop in SortProperty)
            {
                var property = type.GetProperty(prop);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var typeArguments = new Type[] { type, property.PropertyType };
                string methodname = "OrderBy";
                if (!ReferenceEquals(source.GetType(), typeof(IOrderedQueryable<T>)))
                {
                    methodname = Array.IndexOf(SortProperty, prop) > 0 ? Ascending ? "ThenBy" : "ThenByDescending" : Ascending ? "OrderBy" : "OrderByDescending";
                }
                else
                {
                    methodname = Ascending ? "ThenBy" : "ThenByDescending";
                }

                var resultExp = Expression.Call(typeof(Queryable), methodname, typeArguments, source.Expression, Expression.Quote(orderByExp));
                source = source.Provider.CreateQuery<T>(resultExp);
            }

            return (IOrderedQueryable<T>)source;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir do nome de uma ou mais propriedades
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">      </param>
        /// <param name="sortProperty"></param>
        /// <param name="Ascending">   </param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByProperty<T>(this IEnumerable<T> source, string[] SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            SortProperty = SortProperty ?? Array.Empty<string>();
            foreach (var prop in SortProperty)
            {
                object exp(T x) => x.GetPropertyValue<object, object>(prop);
                if (source.GetType() != typeof(IOrderedEnumerable<T>))
                {
                    source = Array.IndexOf(SortProperty, prop) > 0 ? Ascending ? ((IOrderedEnumerable<T>)source).ThenBy(exp) : ((IOrderedEnumerable<T>)source).ThenByDescending(exp) : Ascending ? source.OrderBy(exp) : source.OrderByDescending(exp);
                }
                else
                {
                    source = Ascending ? ((IOrderedEnumerable<T>)source).ThenBy(exp) : ((IOrderedEnumerable<T>)source).ThenByDescending(exp);
                }
            }

            return (IOrderedEnumerable<T>)source;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir de outra lista do mesmo tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="OrderSource"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByList<T>(this IOrderedEnumerable<T> Source, params T[] OrderSource)
        {
            return Source.ThenBy(d => Array.IndexOf(OrderSource, d));
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximação de uma ou mais
        /// <see cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">       </param>
        /// <param name="Searches">    </param>
        /// <param name="SortProperty"></param>
        /// <param name="Ascending">   </param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByLike<T>(this IQueryable<T> items, string[] Searches, string SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            Searches = Searches ?? Array.Empty<string>();
            if (Searches.Any())
            {
                foreach (var t in Searches)
                {
                    var property = type.GetProperty(SortProperty);
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    var testes = new[] { Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)), Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)), Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)), Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t)) };
                    foreach (var exp in testes)
                    {
                        var nv = Expression.Lambda<Func<T, bool>>(exp, parameter);
                        if (Ascending)
                        {
                            if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                            {
                                items = items.OrderByDescending(nv);
                            }
                            else
                            {
                                items = ((IOrderedQueryable<T>)items).ThenByDescending(nv);
                            }
                        }
                        else if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                        {
                            items = items.OrderBy(nv);
                        }
                        else
                        {
                            items = ((IOrderedQueryable<T>)items).ThenBy(nv);
                        }
                    }
                }
            }
            else
            {
                items = items.OrderBy(x => 0);
            }

            return (IOrderedQueryable<T>)items;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximação de uma ou mais
        /// <see cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">       </param>
        /// <param name="Searches">    </param>
        /// <param name="SortProperty"></param>
        /// <param name="Ascending">   </param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByLike<T>(this IQueryable<T> items, string[] Searches, Expression<Func<T, string>> SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            Searches = Searches ?? Array.Empty<string>();
            if (Searches.Any())
            {
                foreach (var t in Searches)
                {
                    MemberExpression mem = (MemberExpression)SortProperty.Body;
                    var property = mem.Member;
                    var parameter = SortProperty.Parameters.First();
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    var tests = new[] { Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)), Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)), Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)), Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t)) };
                    foreach (var exp in tests)
                    {
                        var nv = Expression.Lambda<Func<T, bool>>(exp, parameter);
                        if (Ascending)
                        {
                            if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                            {
                                items = items.OrderByDescending(nv);
                            }
                            else
                            {
                                items = ((IOrderedQueryable<T>)items).ThenByDescending(nv);
                            }
                        }
                        else if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                        {
                            items = items.OrderBy(nv);
                        }
                        else
                        {
                            items = ((IOrderedQueryable<T>)items).ThenBy(nv);
                        }
                    }
                }
            }
            else
            {
                items = items.OrderBy(x => 0);
            }

            return (IOrderedQueryable<T>)items;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximação de uma ou mais
        /// <see cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">           </param>
        /// <param name="PropertySelector"></param>
        /// <param name="Ascending">       </param>
        /// <param name="Searches">        </param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, params string[] Searches) where T : class
        {
            IOrderedEnumerable<T> newitems = null;
            Searches = Searches ?? Array.Empty<string>();
            if (items.GetType() != typeof(IOrderedEnumerable<T>))
            {
                items = items.OrderBy(x => 0);
            }
            else
            {
                newitems = (IOrderedEnumerable<T>)items;
            }

            if (Searches.Any())
            {
                foreach (var t in Searches)
                {
                    var arr = new[] {
                         new Func<T, bool>(x => (PropertySelector(x) ?? "") == (t ?? "")),
                         new Func<T, bool>(x => PropertySelector(x).StartsWith(t)),
                         new Func<T, bool>(x => PropertySelector(x).Contains(t)),
                         new Func<T, bool>(x => PropertySelector(x).EndsWith(t))
                    };

                    foreach (var exp in arr)
                    {
                        if (Ascending)
                        {
                            newitems = newitems.ThenByDescending(exp);
                        }
                        else
                        {
                            newitems = newitems.ThenBy(exp);
                        }
                    }
                }
            }

            return newitems;
        }

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool Most<T>(this IEnumerable<T> List, Func<T, bool> predicate, bool Result = true) => List.Select(predicate).Most(Result);

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem true
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool MostTrue<T>(this IEnumerable<T> List, Func<T, bool> predicate) => MostTrue(List.Select(predicate).ToArray());

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem false
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool MostFalse<T>(this IEnumerable<T> List, Func<T, bool> predicate) => MostFalse(List.Select(predicate).ToArray());

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool Most(this IEnumerable<bool> List, bool Result = true)
        {
            if (List.Count() > 0)
            {
                var arr = List.DistinctCount();
                if (arr.ContainsKey(true) && arr.ContainsKey(false))
                {
                    return arr[Result] > arr[!Result];
                }
                else
                {
                    return arr.First().Key == Result;
                }
            }

            return false == Result;
        }

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem TRUE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool MostTrue(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).Most(true);

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem FALSE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool MostFalse(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).Most(false);

        /// <summary>
        /// Retorna TRUE se a todos os testes em uma lista retornarem TRUE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool AllTrue(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).All(x => x == true);

        /// <summary>
        /// Retorna TRUE se a todos os testes em uma lista retornarem FALSE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool AllFalse(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).All(x => x == false);


    }

    internal class ExpressionReplacer : ExpressionVisitor
    {
        private Expression _dest;
        private Expression _source;

        public ExpressionReplacer(Expression source, Expression dest)
        {
            _source = source;
            _dest = dest;
        }

        public override Expression Visit(Expression node)
        {
            if (node.Equals(_source))
                return _dest;
            return base.Visit(node);
        }
    }
}