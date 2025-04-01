using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dapper;
using Extensions;
using Extensions.Databases;
using static Extensions.Util;
/*
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* This file add overloads for each Dapper.SQLMapper method, allowing the use of FormattableString     *
* for building parametrized queries. Each parameter of string will be converted into a SQL parameter. *
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
*/

namespace Extensions.Databases
{
    /// <inheritdoc cref="SqlMapper"/>
    /// <summary>
    /// Main class for Dapper.SimpleCRUD extensions
    /// </summary>
    public static partial class SimpleCRUD

    {


        ///<summary> Monta um Comando SQL para executar um SELECT com
        /// filtros a partir de um <see cref="NameValueCollection" />
        /// </summary>
        /// <param name="NVC"> Dicionario</param> <param name="TableName">Nome da Tabela</param>
        public static Select ToSQLFilter(this NameValueCollection NVC, string TableName, string CommaSeparatedColumns, params string[] FilterKeys) => (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(NVC, FilterKeys);

        /// <summary>
        /// Monta um Comando SQL para executar um SELECT com filtros a partir de um <see
        /// cref="Dictionary{string, object}"/>
        /// </summary>
        /// <param name="Dic">Dicionario</param>
        /// <param name="TableName">Nome da Tabela</param>
        /// <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
        /// <returns>Uma string com o comando montado</returns>
        public static Select ToSQLFilter(this Dictionary<string, object> Dic, string TableName, string CommaSeparatedColumns, LogicConcatenationOperator LogicConcatenation, params string[] FilterKeys) => (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(Dic, LogicConcatenation, FilterKeys);


        public static T Map<T>(this DataRow Row, params object[] args) where T : class
        {
            T d;
            if (args.Any())
            {
                d = (T)Activator.CreateInstance(typeof(T), args);
            }
            else
            {
                d = Activator.CreateInstance<T>();
            }

            if (Row?.Table?.Columns != null)
                for (int ii = 0; ii < Row.Table.Columns.Count; ii++)
                {
                    var col = Row.Table.Columns[ii];
                    string name = col.ColumnName;
                    var value = Row.GetValue(name);
                    if (d is Dictionary<string, object> dic)
                    {
                        dic.Set(name, value);
                    }
                    else if (d is NameValueCollection nvc)
                    {
                        nvc.Add(name, $"{value}");
                    }
                    else
                    {
                        var PropInfos = Util.GetTypeOf(d).FindProperties(name);
                        var FieldInfos = Util.GetTypeOf(d).FindFields(name).Where(x => x.Name.IsNotIn(PropInfos.Select(y => y.Name)));
                        foreach (var info in PropInfos)
                        {
                            if (info.CanWrite)
                            {
                                if (value == null || ReferenceEquals(value.GetType(), typeof(DBNull)))
                                {
                                    info.SetValue(d, null);
                                }
                                else
                                {
                                    info.SetValue(d, Util.ChangeType(value, info.PropertyType));
                                }
                            }
                        }

                        foreach (var info in FieldInfos)
                        {
                            if (ReferenceEquals(value.GetType(), typeof(DBNull)))
                            {
                                info.SetValue(d, null);
                            }
                            else
                            {
                                info.SetValue(d, Util.ChangeType(value, info.FieldType));
                            }
                        }
                    }
                }
            return d;
        }



        public static IEnumerable<T> Map<T>(this DataTable Data, params object[] args) where T : class
        {
            var l = new List<T>();
            args = args ?? Array.Empty<object>();
            if (Data != null)
                for (int i = 0; i < Data.Rows.Count; i++) l.Add(Data.Rows[i].Map<T>(args));

            return l.AsEnumerable();
        }

        /// <summary>
        /// Mapeia o resultado de um <see cref="DbDataReader"/> para um <see cref="object"/>, <see
        /// cref="Dictionary{TKey, TValue}"/> ou <see cref="NameValueCollection"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this DbDataReader Reader, params object[] args) where T : class
        {
            var l = new List<T>();
            args = args ?? Array.Empty<object>();
            while (Reader != null && Reader.Read())
            {
                T d;
                if (args.Any())
                {
                    d = (T)Activator.CreateInstance(typeof(T), args);
                }
                else
                {
                    d = Activator.CreateInstance<T>();
                }

                for (int i = 0, loopTo = Reader.FieldCount - 1; i <= loopTo; i++)
                {
                    string name = Reader.GetName(i);
                    var value = Reader.GetValue(i);
                    if (typeof(T) == typeof(Dictionary<string, object>))
                    {
                        ((Dictionary<string, object>)(object)d).Set(name, value);
                    }
                    else if (typeof(T) == typeof(NameValueCollection))
                    {
                        ((NameValueCollection)(object)d).Add(name, $"{value}");
                    }
                    else
                    {
                        var propnames = name.PropertyNamesFor().ToList();
                        var PropInfos = Util.GetTypeOf(d).GetProperties().Where(x => x.GetCustomAttributes<ColumnAttribute>().Select(n => n.Name).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
                        var FieldInfos = Util.GetTypeOf(d).GetFields().Where(x => x.GetCustomAttributes<ColumnAttribute>().Select(n => n.Name).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase)).Where(x => x.Name.IsNotIn(PropInfos.Select(y => y.Name)));
                        foreach (var info in PropInfos)
                        {
                            if (info.CanWrite)
                            {
                                if (value == null || ReferenceEquals(value.GetType(), typeof(DBNull)))
                                {
                                    info.SetValue(d, null);
                                }
                                else
                                {
                                    info.SetValue(d, Util.ChangeType(value, info.PropertyType));
                                }
                            }
                        }

                        foreach (var info in FieldInfos)
                        {
                            if (ReferenceEquals(value.GetType(), typeof(DBNull)))
                            {
                                info.SetValue(d, null);
                            }
                            else
                            {
                                info.SetValue(d, Util.ChangeType(value, info.FieldType));
                            }
                        }
                    }
                }

                l.Add(d);
            }

            return l.AsEnumerable();
        }

        public static T MapFirst<T>(this DataSet Data, params object[] args) where T : class => Data.GetFirstRow().Map<T>(args);

        public static T MapFirst<T>(this DataTable Data, params object[] args) where T : class => Data.GetFirstRow().Map<T>(args);

        /// <summary>
        /// Mapeia a primeira linha de um datareader para uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Reader"></param>
        /// <param name="args">argumentos para o construtor da classe</param>
        /// <returns></returns>
        public static T MapFirst<T>(this DbDataReader Reader, params object[] args) where T : class => Reader.Map<T>(args).FirstOrDefault();

        /// <summary>
        /// Mapeia os resultsets de um datareader para um <see cref="IEnumerable{IEnumerable{  Dictionary{String, Object}}}"/>
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> MapMany(this DbDataReader Reader)
        {
            var l = new List<IEnumerable<Dictionary<string, object>>>();
            if (Reader != null)
            {
                do
                {
                    l.Add(Reader.Map<Dictionary<string, object>>());
                }
                while (Reader.NextResult());
            }

            return l.AsEnumerable();
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> MapMany<T1, T2, T3, T4, T5>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            IEnumerable<T3> o3 = null;
            IEnumerable<T4> o4 = null;
            IEnumerable<T5> o5 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }

                if (Reader.NextResult())
                {
                    o3 = Reader.Map<T3>();
                }

                if (Reader.NextResult())
                {
                    o4 = Reader.Map<T4>();
                }

                if (Reader.NextResult())
                {
                    o5 = Reader.Map<T5>();
                }
            }

            return Tuple.Create(o1, o2, o3, o4, o5);
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> MapMany<T1, T2, T3, T4>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            IEnumerable<T3> o3 = null;
            IEnumerable<T4> o4 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }

                if (Reader.NextResult())
                {
                    o3 = Reader.Map<T3>();
                }

                if (Reader.NextResult())
                {
                    o4 = Reader.Map<T4>();
                }
            }

            return Tuple.Create(o1, o2, o3, o4);
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> MapMany<T1, T2, T3>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            IEnumerable<T3> o3 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }

                if (Reader.NextResult())
                {
                    o3 = Reader.Map<T3>();
                }
            }

            return Tuple.Create(o1, o2, o3);
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> MapMany<T1, T2>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }
            }

            return Tuple.Create(o1, o2);
        }


        public static (PropertyInfo, string, Object) ParseExpression<T>(Expression<Action<T>> expression)
        {
            if (expression is LambdaExpression lambdaExpression)
            {
                return ParseExpression(lambdaExpression.Body);
            }
            else
            {
                throw new NotSupportedException($"Expression '{expression}' is not supported.");
            }
        }
        public static (PropertyInfo, string, Object) ParseExpression(Expression expression)
        {
            if (expression is BinaryExpression binaryExpression)
            {
                return ParseExpression(binaryExpression);
            }
            else if (expression is MemberExpression memberExpression)
            {
                var memberInfo = memberExpression.Member as PropertyInfo;
                return (memberInfo, "=", true);
            }
            else
            {
                throw new NotSupportedException($"Expression '{expression}' is not supported.");
            }
        }
        public static (PropertyInfo, string, object) ParseExpression(BinaryExpression binaryExpression)
        {
            var memberExpression = binaryExpression.Left as MemberExpression;
            var valueExpression = binaryExpression.Right;

            var value = Expression.Lambda(valueExpression).Compile().DynamicInvoke();
            var memberInfo = memberExpression.Member as PropertyInfo;

            return (memberInfo, GetSQLOperator(binaryExpression.NodeType), value);
        }

        public static string GetSQLOperator(this ExpressionType expressionType)
        {
            if (expressionType == ExpressionType.NotEqual)
            {
                return "<>";
            }
            else if (expressionType == ExpressionType.GreaterThan)
            {
                return ">";
            }
            else if (expressionType == ExpressionType.GreaterThanOrEqual)
            {
                return ">=";
            }
            else if (expressionType == ExpressionType.LessThan)
            {
                return "<";
            }
            else if (expressionType == ExpressionType.LessThanOrEqual)
            {
                return "<=";
            }
            else if (expressionType == ExpressionType.Equal)
            {
                return "=";
            }
            else if (expressionType == ExpressionType.AndAlso)
            {
                return "AND";
            }
            else if (expressionType == ExpressionType.OrElse)
            {
                return "OR";
            }
            else
            {
                throw new NotSupportedException($"Operator '{expressionType}' is not supported.");
            }
        }

        internal const string DefaultParameterPrefix = "@__p";

        public static int Execute(this DbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.Execute(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static IEnumerable<T> Query<T>(this DbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) => cnn.Query<T>(sql.Query, sql.Parameters, transaction, buffered, commandTimeout, commandType);
        public static T QueryFirst<T>(this DbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QueryFirst<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static T QueryFirstOrDefault<T>(this DbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QueryFirstOrDefault<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static T QuerySingle<T>(this DbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QuerySingle<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        public static T QuerySingleOrDefault<T>(this DbConnection cnn, InterpolatedQuery sql, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null) => cnn.QuerySingleOrDefault<T>(sql.Query, sql.Parameters, transaction, commandTimeout, commandType);

        /// <summary>
        /// Converts a FormattableString to a InterpolatedQuery. It contains the Query string and DynamicParameters.
        /// </summary>
        /// <param name="sql">The FormattableString to convert.</param>
        /// <param name="parameterPrefix">the prefix used for each parameter</param>
        /// <param name="aditionalParameters">Append aditional parameters using <see cref="DynamicParameters.AddDynamicParams(object)"/></param>
        /// <returns>A tuple containing the query string and DynamicParameters.</returns>
        public static InterpolatedQuery ToInterpolatedQuery(this FormattableString sql, string parameterPrefix = null, object aditionalParameters = null, CultureInfo culture = null) => new InterpolatedQuery(sql, parameterPrefix, aditionalParameters, culture);

        private static readonly ConcurrentDictionary<string, string> ColumnNames = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<string, string> StringBuilderCacheDict = new ConcurrentDictionary<string, string>();

        private static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();

        private static IColumnNameResolver _columnNameResolver = new ColumnNameResolver();

        private static Dialect _dialect = Dialect.SQLServer;

        private static string _encapsulation;

        private static string _getIdentitySql;

        private static string _getPagedListSql;

        private static ITableNameResolver _tableNameResolver = new TableNameResolver();

        private static bool StringBuilderCacheEnabled = true;

        public static SQLResponse<object> CreateSQLQuickResponse(this DbConnection Connection, FormattableString Command, string DataSetType, params string[] SetNames) => CreateSQLQuickResponse(Connection.CreateCommand(Command), DataSetType, SetNames);

        /// <summary>
        /// Executa um <paramref name="Command"/> e retorna uma <see cref="SQLResponse{object}"/> de
        /// acordo com o formato especificado em <paramref name="DataSetType"/>
        /// </summary>
        /// <remarks>
        /// Utilize as constantes de <see cref="DataSetType"/> no parametro <paramref name="DataSetType"/>
        /// </remarks>
        /// <param name="Command">Comando SQL com a <see cref="DbCommand.Connection"/> ja setada</param>
        /// <param name="DataSetType">Tipo da resposta. Ver <see cref="DataSetType"/></param>
        /// <returns></returns>
        public static SQLResponse<object> CreateSQLQuickResponse(this DbCommand Command, string DataSetType, params string[] SetNames)
        {
            var resp = new SQLResponse<object>();
            try
            {
                DataSetType = DataSetType.IfBlank("table").ToLowerInvariant();
                var Connection = (Command?.Connection) ?? throw new Exception("Command or Connection is null");
                resp.SQL = Command.CommandText;
                resp.DataSetType = DataSetType;

                if (DataSetType.IsAny("value", "id", "key", "singlevalue"))
                {
                    //primeiro valor da primeira linha do primeiro set
                    var v = Connection.QueryFirst(Command)?.FirstOrDefault();
                    resp.Status = (part == DBNull.Value).AsIf("NULL_VALUE", (part == null).AsIf("empty", "OK"));
                    resp.Data = part;
                    resp.DataSetType = "value";
                }
                else if (DataSetType.IsAny("one", "first", "row", "single"))
                {
                    //primeiro do primeiro set (1 linha como objeto)
                    var part = Connection.RunSQLRow(Command);
                    resp.Status = (part == null).AsIf("empty", "OK");
                    resp.Data = part;
                    resp.DataSetType = "row";
                }
                else if (DataSetType.IsAny("array", "values", "list"))
                {
                    //primeira coluna do primeiro set como array
                    var part = Connection.RunSQLArray(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "empty");
                    resp.Data = part;
                    resp.DataSetType = "array";
                }
                else if (DataSetType.IsAny("pair", "pairs", "dictionary", "associative"))
                {
                    //primeira e ultima coluna do primeiro set como dictionary
                    var part = Connection.RunSQLPairs(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "empty");
                    resp.Data = part;
                    resp.DataSetType = "pairs";
                }
                else if (DataSetType.IsAny("many", "sets", "datasets", "namedsets"))
                {
                    //varios sets
                    var part = Connection.RunSQLMany(Command);
                    resp.Status = (part?.Any(x => x.Any())).AsIf("OK", "empty");
                    if (DataSetType == "namedsets")
                    {
                        foreach (var k in part.Select((x, i) => new KeyValuePair<string, object>(SetNames.IfBlankOrNoIndex(i, $"set{i}"), x)).ToDictionary())
                        {
                            resp[k.Key] = k.Value;
                        }

                        resp.DataSetType = "namedsets";
                    }
                    else
                    {
                        resp.Data = part;
                        resp.DataSetType = "sets";
                    }
                }
                else
                {
                    //tudo do primeiro set (lista de objetos)
                    var part = Connection.RunSQLSet(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "empty");
                    resp.Data = part;
                    resp.DataSetType = "table";
                }
            }
            catch (Exception ex)
            {
                resp.Status = "ERROR";
                resp.Message = ex.ToFullExceptionString();
            }
            return resp;
        }

        /// <summary>
        /// Formata o nome de uma coluna SQL adicionando <paramref name="QuoteChar"/> as <paramref
        /// name="ColumnNameParts"/> e as unindo com <b>.</b>
        /// </summary>
        /// <param name="QuoteChar"></param>
        /// <param name="ColumnNameParts"></param>
        /// <returns></returns>
        public static string FormatSQLColumn(char QuoteChar, params string[] ColumnNameParts) => ColumnNameParts.WhereNotBlank().SelectJoinString(x => x.UnQuote(QuoteChar).Quote(QuoteChar), ".");

        /// <inheritdoc cref="FormatSQLColumn(char, string[])"/>
        public static string FormatSQLColumn(params string[] ColumnNameParts) => FormatSQLColumn('[', ColumnNameParts);

        /// <summary>
        /// Cria um comando de UPDATE para o objeto do tipo <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// <typeparamref name="T"/> pode ser uma classe, <see cref="NameValueCollection"/> ou <see
        /// cref="Dictionary{TKey, TValue}"/>
        /// </remarks>
        public static DbCommand CreateUPDATECommand<T>(this DbConnection Connection, T obj, FormattableString WhereClausule, string TableName = null, DbTransaction Transaction = null) where T : class
        {
            var d = typeof(T);
            Dictionary<string, object> dic;

            if (obj != null && Connection != null)
            {
                dic = obj.CreateDictionary();

                var cmd = Connection.CreateCommand();
                cmd.CommandText = $"UPDATE " + TableName.IfBlank(d.GetTableName()) + " set" + Environment.NewLine;
                foreach (var k in dic.Keys)
                {
                    var col = GetColumnName(d, k).IfBlank(Encapsulate(k));
                    cmd.CommandText += $"{col} = @__{k}, {Environment.NewLine}";
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"__{k}";
                    param.Value = dic.GetValueOr(k, DBNull.Value);
                    cmd.Parameters.Add(param);
                }

                cmd.CommandText = cmd.CommandText.TrimAny(Environment.NewLine, ",", " ");

                if (WhereClausule.IsNotBlank())
                {
                    var wherecmd = Connection.CreateCommand(WhereClausule);
                    var wheretxt = wherecmd.CommandText.Trim();
                    foreach (DbParameter item in wherecmd.Parameters)
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = item.ParameterName;
                        param.Value = item.Value;
                        param.DbType = item.DbType;
                        cmd.Parameters.Add(param);
                    }
                    cmd.CommandText += $"{Environment.NewLine}{wheretxt.PrependIf(" WHERE ", x => !x.Trim().StartsWith("WHERE"))}";
                    wherecmd.Dispose();
                }

                if (Transaction != null)
                {
                    cmd.Transaction = Transaction;
                }

                return cmd;
            }

            return null;
        }



        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> RunSQLArray<T>(this DbConnection Connection, DbCommand Command) => Connection.RunSQLArray(Command).Select(x => x == null ? default : x.ChangeType<T>());

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> RunSQLArray<T>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLArray<T>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array
        /// </summary>
        public static IEnumerable<object> RunSQLArray(this DbConnection Connection, DbCommand Command) => Connection.RunSQLSet(Command).Select(x => x.Values.FirstOrDefault());

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array
        /// </summary>
        public static IEnumerable<object> RunSQLArray(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLArray(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em listas de <see
        /// cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> RunSQLMany(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLMany(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL e retorna todos os seus resultsets mapeados em uma <see
        /// cref="IEnumerable{IEnumerable{Dictionary{String, Object}}}"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> RunSQLMany(this DbConnection Connection, DbCommand Command)
        {
            IEnumerable<IEnumerable<Dictionary<string, object>>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos específicos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> RunSQLMany<T1, T2, T3, T4, T5>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class => Connection.RunSQLMany<T1, T2, T3, T4, T5>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> RunSQLMany<T1, T2, T3, T4, T5>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2, T3, T4, T5>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> RunSQLMany<T1, T2, T3, T4>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class => Connection.RunSQLMany<T1, T2, T3, T4>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> RunSQLMany<T1, T2, T3, T4>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2, T3, T4>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> RunSQLMany<T1, T2, T3>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class => Connection.RunSQLMany<T1, T2, T3>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> RunSQLMany<T1, T2, T3>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2, T3>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> RunSQLMany<T1, T2>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class => Connection.RunSQLMany<T1, T2>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> RunSQLMany<T1, T2>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa um comando SQL e retorna o numero de linhas afetadas
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static int RunSQLNone(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLNone(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o numero de linhas afetadas
        /// </summary>
        public static int RunSQLNone(this DbConnection Connection, DbCommand Command) => BeforeRunCommand(ref Connection, ref Command).ExecuteNonQuery();

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{Object, Object}"/>
        /// </summary>
        public static Dictionary<object, object> RunSQLPairs(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLSet(SQL).DistinctBy(x => x.Values.FirstOrDefault()).ToDictionary(x => x.Values.FirstOrDefault(), x => x.Values.LastOrDefault());

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{object,object}"/>
        /// </summary>
        public static Dictionary<object, object> RunSQLPairs(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLPairs(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{K, V}"/>
        /// </summary>
        public static Dictionary<TK, TV> RunSQLPairs<TK, TV>(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLPairs(SQL).ToDictionary(x => x.Key.ChangeType<TK>(), x => x.Value.ChangeType<TV>());

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{K, V}"/>
        /// </summary>
        public static Dictionary<TK, TV> RunSQLPairs<TK, TV>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLPairs<TK, TV>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o <see cref="DbDataReader"/> com os resultados
        /// </summary>
        public static DbDataReader RunSQLReader(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLReader(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o <see cref="DbDataReader"/> com os resultados
        /// </summary>
        public static DbDataReader RunSQLReader(this DbConnection Connection, DbCommand Command) => BeforeRunCommand(ref Connection, ref Command).ExecuteReader();

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados da primeira linha como um
        /// <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, Select<T> Select, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLRow<T>(Select.CreateDbCommand(Connection, Transaction), WithSubQueries);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// um <see cref="Dictionary{String, Object}"/>
        /// </summary>
        public static Dictionary<string, object> RunSQLRow(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLRow<Dictionary<string, object>>(SQL, false, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// um <see cref="Dictionary{String, Object}"/>
        /// </summary>
        public static Dictionary<string, object> RunSQLRow(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLRow<Dictionary<string, object>>(SQL);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, DbCommand SQL, bool WithSubQueries = false) where T : class
        {
            var x = Connection.RunSQLSet<T>(SQL, false).FirstOrDefault();
            if (x != null && WithSubQueries)
            {
                Connection.ProccessSubQuery(x, WithSubQueries);
            }

            return x ?? default;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, FormattableString SQL, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLRow<T>(Connection.CreateCommand(SQL, Transaction), WithSubQueries);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, Select<T> Select, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLSet<T>(Select.CreateDbCommand(Connection, Transaction), WithSubQueries);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <see cref="Dictionary{String, Object}"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> RunSQLSet(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLSet<Dictionary<string, object>>(SQL, false, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        public static IEnumerable<Dictionary<string, object>> RunSQLSet(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLSet<Dictionary<string, object>>(SQL);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, FormattableString SQL, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLSet<T>(Connection.CreateCommand(SQL, Transaction), WithSubQueries);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, DbCommand SQL, bool WithSubQueries = false) where T : class
            => Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(x =>
            {
                T v = (T)x.CreateOrSetObject(null, typeof(T));
                if (WithSubQueries)
                {
                    Connection.ProccessSubQuery(v, WithSubQueries);
                }
                return v;
            }).AsEnumerable();

        /// <summary>
        /// Processa uma propriedade de uma classe marcada com <see cref="FromSQL"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="d"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        public static T ProccessSubQuery<T>(this DbConnection Connection, T d, string PropertyName, bool Recursive = false)
        {
            if (d != null)
            {
                var prop = d.GetProperty(PropertyName);
                if (prop != null)
                {
                    var attr = prop.GetCustomAttributes<FromSQLAttribute>(true).FirstOrDefault();
                    string Sql = attr.SQL.Inject(d);
                    bool gen = prop.PropertyType.IsGenericType;
                    bool lista = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
                    bool enume = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>));
                    bool cole = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>));
                    if (lista || enume || cole)
                    {
                        IList baselist = (IList)Activator.CreateInstance(prop.PropertyType);
                        var eltipo = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                        foreach (var x in Connection.RunSQLSet(Sql.ToFormattableString()))
                        {
                            baselist.Add(x.CreateOrSetObject(null, eltipo));
                        }

                        prop.SetValue(d, baselist);
                        if (Recursive)
                        {
                            foreach (var uu in baselist)
                            {
                                Connection.ProccessSubQuery(uu, Recursive);
                            }
                        }

                        return d;
                    }
                    else if (prop.PropertyType.IsClass)
                    {
                        if (prop.GetValue(d) == null)
                        {
                            var oo = Connection.RunSQLRow(Sql.ToFormattableString()).CreateOrSetObject(null, prop.PropertyType);
                            prop.SetValue(d, oo);
                            if (Recursive)
                            {
                                Connection.ProccessSubQuery(oo, Recursive);
                            }
                        }

                        return d;
                    }
                    else if (prop.PropertyType.IsValueType)
                    {
                        if (prop.GetValue(d) == null)
                        {
                            var oo = Connection.RunSQLValue(Sql.ToFormattableString());
                            prop.SetValue(d, Util.ChangeType(oo, prop.PropertyType));
                            if (Recursive)
                            {
                                Connection.ProccessSubQuery(oo, Recursive);
                            }
                        }

                        return d;
                    }
                }
            }

            return d;
        }

        /// <summary>
        /// Processa todas as propriedades de uma classe marcadas com <see cref="FromSQL"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="d"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        public static T ProccessSubQuery<T>(this DbConnection Connection, T d, bool Recursive = false) where T : class
        {
            foreach (var prop in Util.GetProperties(d).Where(x => x.HasAttribute<FromSQLAttribute>()))
            {
                Connection.ProccessSubQuery(d, prop.Name, Recursive);
            }

            return d;
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de um arquivo SQL e um objeto,
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand<T>(this DbConnection Connection, FileInfo SQLFile, T obj, DbTransaction Transaction = null) => CreateCommand(Connection, SQLFile.Exists ? SQLFile.ReadAllText() : string.Empty, obj, Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um objeto,
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand<T>(DbConnection connection, string SQL, T obj, DbTransaction transaction = null) => CreateCommand(connection, SQL.InjectSQL(obj).ToFormattableString(), transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see cref="Dictionary{
        /// String, Object}"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, Dictionary<string, object> Parameters, DbTransaction Transaction = null) => SQLFile != null && SQLFile.Exists ? CreateCommand(Connection, SQLFile.ReadAllText(), Parameters, Transaction) : null;

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see
        /// cref="NameValueCollection"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, NameValueCollection Parameters, DbTransaction Transaction = null) => Connection.CreateCommand(SQLFile, Parameters.ToDictionary(), Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see
        /// cref="NameValueCollection"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, NameValueCollection Parameters, DbTransaction Transaction = null) => Connection.CreateCommand(SQL, Parameters.ToDictionary(), Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see
        /// cref="Dictionary{TKey, TValue}"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, Dictionary<string, object> Parameters, DbTransaction Transaction = null)
        {
            if (Connection != null && SQL.IsValid())
            {
                var command = Connection.CreateCommand();
                command.CommandText = SQL;
                if (Parameters != null && Parameters.Any())
                {
                    foreach (var p in Parameters.Keys)
                    {
                        var v = Parameters.GetValueOr(p);
                        var arr = Util.ForceArray(v, typeof(object)).ToList();
                        for (int index = 0, loopTo = arr.Count - 1; index <= loopTo; index++)
                        {
                            var param = command.CreateParameter();
                            if (arr.Count == 1)
                            {
                                param.ParameterName = $"__{p}";
                            }
                            else
                            {
                                param.ParameterName = $"__{p}_{index}";
                            }

                            param.Value = arr[index] ?? DBNull.Value;
                            command.Parameters.Add(param);
                        }
                    }
                }
                if (Transaction != null)
                {
                    command.Transaction = Transaction;
                }
                return command;
            }

            return null;
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string , tratando os parametros {p}
        /// desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, params string[] Args) => CreateCommand(Connection, SQL, null, Args);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string , tratando os parametros {p}
        /// desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, DbTransaction Transaction, params string[] Args)
        {
            if (SQL.IsValid())
            {
                return Connection.CreateCommand(SQL.ToFormattableString(Args), Transaction);
            }

            return null;
        }

        public static TConnection OpenConnection<TConnection>(this ConnectionStringParser connection) where TConnection : DbConnection
        {
            if (connection != null)
            {
                TConnection dbcon = Activator.CreateInstance<TConnection>();
                dbcon.ConnectionString = connection.ConnectionString;
                dbcon.Open();
                return dbcon;
            }
            return null;
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de um arquivo SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, params string[] Args) => CreateCommand(Connection, SQLFile.ReadAllText().ToFormattableString(Args));

        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, DbTransaction Transaction, params string[] Args) => CreateCommand(Connection, SQLFile.ReadAllText().ToFormattableString(Args), Transaction);
        public static bool IsOpen(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Open);
        /// <summary>
        /// Valida se uma conexao e um comando nao sao nulos. Valida se o texto do comando esta em
        /// branco e associa este comando a conexao especifica. Escreve o comando no <see
        /// cref="LogWriter"/> e retorna o mesmo
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DbCommand BeforeRunCommand(ref DbConnection Connection, ref DbCommand Command, TextWriter LogWriter = null)
        {
            Connection = Connection ?? Command?.Connection;
            if (Command == null || Command.CommandText.IsNotValid())
            {
                throw new ArgumentException("Command is null or blank");
            }

            Command.Connection = Connection ?? throw new ArgumentException("Connection is null");
            if (!Connection.IsOpen())
            {
                Connection.Open();
            }

            return Command.LogCommand(LogWriter);
        }
        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica para cada item em uma
        /// coleçao. As propriedades do item serao utilizadas como parametros da procedure
        /// </summary>
        /// <param name="Items">Lista de itens que darao origem aos parametros da procedure</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">CHaves de Dicionário que devem ser utilizadas</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static IEnumerable<DbCommand> ToBatchProcedure<T>(this DbConnection Connection, string ProcedureName, IEnumerable<T> Items, DbTransaction Transaction = null, params string[] Keys)
        {
            foreach (var item in Items ?? new List<T>())
            {
                yield return Connection.ToProcedure(ProcedureName, item, Transaction, Keys);
            }
        }

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata valores especificos
        /// de um NameValueCollection como parametros da procedure
        /// </summary>
        /// <param name="NVC">Objeto</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">Valores do nameValueCollection o que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static DbCommand ToProcedure(this DbConnection Connection, string ProcedureName, NameValueCollection NVC, DbTransaction Transaction = null, params string[] Keys) => Connection.ToProcedure(ProcedureName, NVC.ToDictionary(Keys), Transaction, Keys);

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata propriedades
        /// específicas de um objeto como parametros da procedure
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">propriedades do objeto que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static DbCommand ToProcedure<T>(this DbConnection Connection, string ProcedureName, T Obj, DbTransaction Transaction = null, params string[] Keys) => Connection.ToProcedure(ProcedureName, Obj?.CreateDictionary() ?? new Dictionary<string, object>(), Transaction, Keys);

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata os valores
        /// específicos de um <see cref="Dictionary{TKey, TValue}"/> como parametros da procedure
        /// </summary>
        /// <param name="Dic">Objeto</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">propriedades do objeto que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static DbCommand ToProcedure(this DbConnection Connection, string ProcedureName, Dictionary<string, object> Dic, DbTransaction Transaction = null, params string[] Keys)
        {
            var sql = ProcedureName.ToProcedure(Dic, Keys);

            return Connection.CreateCommand(sql, Dic.ToDictionary(x => x.Key, x => x.Value), Transaction);
        }

        public static bool IsConnecting(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Connecting);

        public static bool IsBroken(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Broken);
        public static bool IsExecuting(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Executing);

        public static bool IsClosed(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Closed);
        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string interpolada, tratando os
        /// parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
        {
            if (SQL != null && Connection != null && SQL.IsNotBlank())
            {
                var cmd = Connection.CreateCommand();
                if (SQL.ArgumentCount > 0)
                {
                    cmd.CommandText = SQL.Format;
                    for (int index = 0, loopTo = SQL.ArgumentCount - 1; index <= loopTo; index++)
                    {
                        var valores = SQL.GetArgument(index);
                        var v = Util.ForceArray(valores, typeof(object)).ToList();
                        var param_names = new List<string>();
                        for (int v_index = 0, loopTo1 = v.Count - 1; v_index <= loopTo1; v_index++)
                        {
                            var param = cmd.CreateParameter();
                            if (v.Count == 1)
                            {
                                param.ParameterName = $"__p{index}";
                            }
                            else
                            {
                                param.ParameterName = $"__p{index}_{v_index}";
                            }

                            param.Value = v[v_index] ?? DBNull.Value;
                            cmd.Parameters.Add(param);
                            param_names.Add("@" + param.ParameterName);
                        }

                        cmd.CommandText = cmd.CommandText.Replace("{" + index + "}", param_names.SelectJoinString(",").IfBlank("NULL").UnQuote('(', true).Quote('('));
                    }
                }
                else
                {
                    cmd.CommandText = SQL.ToString();
                }

                if (Transaction != null)
                {
                    cmd.Transaction = Transaction;
                }

                return cmd;
            }

            return null;
        }



        /// <summary>
        /// Retorna o primeiro resultado da primeira coluna de uma consulta SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static object RunSQLValue(this DbConnection Connection, DbCommand Command) => BeforeRunCommand(ref Connection, ref Command).ExecuteScalar();

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL
        /// </summary>
        public static object RunSQLValue(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLValue(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL como um tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static T RunSQLValue<T>(this DbConnection Connection, DbCommand Command)
        {
            if (!typeof(T).IsSimpleType())
            {
                throw new ArgumentException("The type param TEntity is not a value type or string");
            }
            var vv = Connection.RunSQLValue(Command);
            return vv != null && vv != DBNull.Value ? vv.ChangeType<T>() : default;
        }

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL como um tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static T RunSQLValue<T>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLValue<T>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Cria comandos de INSERT para cada objeto do tipo <typeparamref name="T"/> em uma lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="obj"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static IEnumerable<DbCommand> CreateINSERTCommand<T>(this DbConnection Connection, IEnumerable<T> obj, string TableName = null, DbTransaction Transaction = null) where T : class => (obj ?? Array.Empty<T>()).Select(x => Connection.CreateINSERTCommand(x, TableName, Transaction));

        /// <summary>
        /// Cria um comando de INSERT para o objeto do tipo <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// <typeparamref name="T"/> pode ser uma classe, <see cref="NameValueCollection"/> ou <see
        /// cref="Dictionary{TKey, TValue}"/>
        /// </remarks>
        public static DbCommand CreateINSERTCommand<T>(this DbConnection Connection, T obj, string TableName = null, DbTransaction Transaction = null) where T : class
        {
            var d = typeof(T);
            var dic = new Dictionary<string, object>();
            if (obj != null && Connection != null)
            {
                dic = obj.CreateDictionary();

                var cmd = Connection.CreateCommand();
                cmd.CommandText = string.Format($"INSERT INTO {TableName.IfBlank(GetTableName<T>())} ({{0}}) values ({{1}})", dic.Keys.SelectJoinString(","), dic.Keys.SelectJoinString(x => $"@__{x}", ","));
                foreach (var k in dic.Keys)
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"__{k}";
                    param.Value = dic.GetValueOr(k, DBNull.Value);
                    cmd.Parameters.Add(param);
                }
                if (Transaction != null)
                {
                    cmd.Transaction = Transaction;
                }
                return cmd;
            }

            return null;
        }

        public static V GeneratePrimaryKey<T, V>(this DbConnection connection, Expression<Func<T, V>> column, object whereConditions = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class => GeneratePrimaryKey<T, V>(connection, GetColumnName(column), whereConditions, transaction, commandTimeout);

        /// <summary>
        /// Generate a new ID for the entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="connection"></param>
        /// <param name="keyName"></param>
        /// <param name="whereConditions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static V GeneratePrimaryKey<T, V>(this DbConnection connection, string keyName = null, object whereConditions = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var t = typeof(T);
            var sb = new StringBuilder();
            var keys = GetIdProperties(t);

            if (typeof(V) == typeof(Guid))
            {
                var vv = SequentialGuid();
                if (vv is V g)
                {
                    return g;
                }
            }
            else
            {
                if (keys.Any() == false)
                    throw new ArgumentException("Entity must have at least one [Key] or Id property");

                if (string.IsNullOrWhiteSpace(keyName))
                {
                    if (keys.Count(x => x.PropertyType == typeof(V)) > 1)
                    {
                        throw new ArgumentException("Entity has multiple keys, please specify the keyName");
                    }

                    keyName = GetColumnName(keys.FirstOrDefault(x => x.PropertyType == typeof(V)));
                }
                else
                {
                    keyName = t.GetColumnName(keyName);
                }

                var tableName = GetTableName(t);

                sb.Append($"SELECT MAX({keyName}) FROM {tableName} ");

                if (whereConditions is string s && string.IsNullOrWhiteSpace(s) == false)
                {
                    if (s.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        sb.Append(" WHERE ");
                    }

                    sb.Append(s);
                }
                else
                {
                    if (whereConditions != null)
                    {
                        sb.Append(" WHERE ");
                        var wheres = new List<string>();
                        if (whereConditions is DynamicParameters pp)
                        {
                            wheres = pp.ParameterNames.Select(x => $"{typeof(T).GetColumnName(x)} = @{x}").ToList();
                        }
                        else if (whereConditions is Dictionary<string, object> dic)
                        {
                            wheres = dic.Select(x => $"{typeof(T).GetColumnName(x.Key)} = @{x.Key}").ToList();
                            whereConditions = new DynamicParameters(dic);
                        }
                        else
                        {
                            wheres = GetAllProperties(whereConditions).Select(x => $"{typeof(T).GetColumnName(x.Name)} = @{x.Name}").ToList();
                        }
                        sb.Append(string.Join(" AND ", wheres));
                    }
                }

                var sql = sb.ToString();

                var value = connection.QueryFirstOrDefault<V>(sql, whereConditions, transaction, commandTimeout);

                if (value == null)
                {
                    if (connection.RecordCount<T>(whereConditions) == 0)
                    {
                        return (V)Convert.ChangeType(1, typeof(V));
                    }
                    else
                    {
                        throw new Exception("Cannot get the max value to increment");
                    }
                }

                if (value is long v1)
                {
                    return (V)Convert.ChangeType(v1 + 1, typeof(V));
                }
                else

                if (value is int v2)
                {
                    return (V)Convert.ChangeType(v2 + 1, typeof(V));
                }

                if (value is string ss)
                {
                    if (decimal.TryParse(value.ToString(), out decimal v3))
                    {
                        return (V)Convert.ChangeType(v3 + 1, typeof(V));
                    }
                    else
                    {
                        return (V)Convert.ChangeType(SequentialGuid().ToString(), typeof(V));
                    }
                }

                if (value is V v)
                {
                    throw new Exception($"Cannot generate a primary key for {typeof(T).Name}");
                }
            }
            return default;
        }

        //build insert parameters which include all properties in the class that are not:
        //marked with the Editable(false) attribute
        //marked with the [Key] attribute
        //marked with [IgnoreInsert]
        //named Id
        //marked with [NotMapped]
        private static void BuildInsertParameters<T>(StringBuilder masterSb)
        {
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_BuildInsertParameters", sb =>
            {
                var props = GetScaffoldableProperties<T>().ToArray();

                for (var i = 0; i < props.Count(); i++)
                {
                    var property = props.ElementAt(i);

                    if (property.PropertyType != typeof(Guid) && property.PropertyType != typeof(string)
                        && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                           && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                        continue;

                    if (property.GetCustomAttributes(true).Any(attr =>
                        attr.GetType().Name == typeof(IgnoreInsertAttribute).Name ||
                        attr.GetType().Name == typeof(NotMappedAttribute).Name ||
                        attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))) continue;

                    if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                    sb.Append(GetColumnName(property));
                    if (i < props.Count() - 1)
                        sb.Append(", ");
                }
                if (sb.ToString().EndsWith(", "))
                    sb.Remove(sb.Length - 2, 2);
            });
        }

        //build insert values which include all properties in the class that are:
        //Not named Id
        //Not marked with the Editable(false) attribute
        //Not marked with the [Key] attribute (without required attribute)
        //Not marked with [IgnoreInsert]
        //Not marked with [NotMapped]
        private static void BuildInsertValues<T>(StringBuilder masterSb)
        {
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_BuildInsertValues", sb =>
            {
                var props = GetScaffoldableProperties<T>().ToArray();
                for (var i = 0; i < props.Count(); i++)
                {
                    var property = props.ElementAt(i);
                    if (property.PropertyType != typeof(Guid) && property.PropertyType != typeof(string)
                        && property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)
                           && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name))
                        continue;
                    if (property.GetCustomAttributes(true).Any(attr =>
                        attr.GetType().Name == typeof(IgnoreInsertAttribute).Name ||
                        attr.GetType().Name == typeof(NotMappedAttribute).Name ||
                        attr.GetType().Name == typeof(ReadOnlyAttribute).Name && IsReadOnly(property))
                       ) continue;

                    if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.GetCustomAttributes(true).All(attr => attr.GetType().Name != typeof(RequiredAttribute).Name) && property.PropertyType != typeof(Guid)) continue;

                    sb.AppendFormat("@{0}", property.Name);
                    if (i < props.Count() - 1)
                        sb.Append(", ");
                }
                if (sb.ToString().EndsWith(", "))
                    sb.Remove(sb.Length - 2, 2);
            });
        }

        //build select clause based on list of properties skipping ones with the IgnoreSelect and NotMapped attribute
        private static void BuildSelect(StringBuilder masterSb, IEnumerable<PropertyInfo> props)
        {
            StringBuilderCache(masterSb, $"{props.CacheKey()}_BuildSelect", sb =>
            {
                var propertyInfos = props as IList<PropertyInfo> ?? props.ToList();
                var addedAny = false;
                for (var i = 0; i < propertyInfos.Count(); i++)
                {
                    var property = propertyInfos.ElementAt(i);

                    if (property.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreSelectAttribute).Name || attr.GetType().Name == typeof(NotMappedAttribute).Name)) continue;

                    if (addedAny)
                        sb.Append(",");
                    sb.Append(GetColumnName(property));
                    //if there is a custom column name add an "as customcolumnname" to the item so it maps properly
                    if (property.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) != null)
                        sb.Append(" as " + Encapsulate(property.Name));
                    addedAny = true;
                }
            });
        }

        //build update statement based on list on an entity
        private static void BuildUpdateSet<T>(T entityToUpdate, StringBuilder masterSb)
        {
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_BuildUpdateSet", sb =>
            {
                var nonIdProps = GetUpdateableProperties(entityToUpdate).ToArray();

                for (var i = 0; i < nonIdProps.Length; i++)
                {
                    var property = nonIdProps[i];

                    sb.AppendFormat("{0} = @{1}", GetColumnName(property), property.Name);
                    if (i < nonIdProps.Length - 1)
                        sb.AppendFormat(", ");
                }
            });
        }

        private static void BuildWhere<TEntity>(StringBuilder sb, IEnumerable<PropertyInfo> idProps, object whereConditions = null)
        {
            var propertyInfos = idProps.ToArray();
            for (var i = 0; i < propertyInfos.Count(); i++)
            {
                var useIsNull = false;

                //match up generic properties to source entity properties to allow fetching of the column attribute
                //the anonymous object used for search doesn'classType have the custom attributes attached to them so this allows us to build the correct where clause
                //by converting the model type to the database column name via the column attribute
                var propertyToUse = propertyInfos.ElementAt(i);
                var sourceProperties = GetScaffoldableProperties<TEntity>().ToArray();
                for (var x = 0; x < sourceProperties.Count(); x++)
                {
                    if (sourceProperties.ElementAt(x).Name == propertyToUse.Name)
                    {
                        if (whereConditions != null && propertyToUse.CanRead && (propertyToUse.GetValue(whereConditions, null) == null || propertyToUse.GetValue(whereConditions, null) == DBNull.Value))
                        {
                            useIsNull = true;
                        }
                        propertyToUse = sourceProperties.ElementAt(x);
                        break;
                    }
                }
                sb.AppendFormat(
                    useIsNull ? "{0} is null" : "{0} = @{1}",
                    GetColumnName(propertyToUse),
                    propertyToUse.Name);

                if (i < propertyInfos.Count() - 1)
                    sb.AppendFormat(" and ");
            }
        }

        //Get all properties in an entity
        private static IEnumerable<PropertyInfo> GetAllProperties<T>() where T : class => typeof(T).GetProperties();

        private static IEnumerable<PropertyInfo> GetAllProperties<T>(T entity) where T : class
        {
            if (entity == null) return Array.Empty<PropertyInfo>();
            if (entity is Type type)
            {
                return type.GetProperties();
            }

            return GetAllProperties(entity.GetType());
        }

        private static IEnumerable<PropertyInfo> GetIdProperties<T>() where T : class
        => typeof(T).GetIdProperties();

        //Get all properties that are named Id or have the Key attribute
        //For Inserts and updates we have a whole entity so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties<T>(this T entity) where T : class
        {
            if (entity is Type type)
                return GetIdProperties(type);
            return GetIdProperties(entity.GetType());
        }

        //Get all properties that are named Id or have the Key attribute
        //For Get(id) and Delete(id) we don'classType have an entity, just the type so this method is used
        private static IEnumerable<PropertyInfo> GetIdProperties(this Type type)
        {
            var tp = type.GetProperties().Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name)).ToList();
            return tp.Any() ? tp : type.GetProperties().Where(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
        }

        //Get all properties that are not decorated with the Editable(false) attribute

        private static IEnumerable<PropertyInfo> GetScaffoldableProperties<T>() => typeof(T).GetScaffoldableProperties();

        private static IEnumerable<PropertyInfo> GetScaffoldableProperties(this Type type)
        {


            IEnumerable<PropertyInfo> props = type.GetProperties();

            props = props.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(EditableAttribute).Name && !IsEditable(p)) == false);

            return props.Where(p => p.PropertyType.IsSimpleType() || IsEditable(p));
        }

        //Get all properties that are:
        //Not named Id
        //Not marked with the Key attribute
        //Not marked ReadOnly
        //Not marked IgnoreInsert
        //Not marked NotMapped
        private static IEnumerable<PropertyInfo> GetUpdateableProperties<T>(T entity)
        {
            var updateableProperties = GetScaffoldableProperties<T>();
            //remove ones with ID
            updateableProperties = updateableProperties.Where(p => !p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));
            //remove ones with key attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(KeyAttribute).Name) == false);
            //remove ones that are readonly
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => (attr.GetType().Name == typeof(ReadOnlyAttribute).Name) && IsReadOnly(p)) == false);
            //remove ones with IgnoreUpdate attribute
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(IgnoreUpdateAttribute).Name) == false);
            //remove ones that are not mapped
            updateableProperties = updateableProperties.Where(p => p.GetCustomAttributes(true).Any(attr => attr.GetType().Name == typeof(NotMappedAttribute).Name) == false);

            return updateableProperties;
        }

        //Determine if the Attribute has an AllowEdit key and return its boolean state
        //fake the funk and try to mimic EditableAttribute in System.ComponentModel.DataAnnotations
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsEditable(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(EditableAttribute).Name);
                if (write != null)
                {
                    return write.AllowEdit;
                }
            }
            return false;
        }

        //Determine if the Attribute has an IsReadOnly key and return its boolean state
        //fake the funk and try to mimic ReadOnlyAttribute in System.ComponentModel
        //This allows use of the DataAnnotations property in the model and have the SimpleCRUD engine just figure it out without a reference
        private static bool IsReadOnly(PropertyInfo pi)
        {
            var attributes = pi.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                dynamic write = attributes.FirstOrDefault(x => x.GetType().Name == typeof(ReadOnlyAttribute).Name);
                if (write != null)
                {
                    return write.IsReadOnly;
                }
            }
            return false;
        }

        /// <summary>
        /// Append a Cached version of a strinbBuilderAction result based on a cacheKey
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="cacheKey"></param>
        /// <param name="stringBuilderAction"></param>
        private static void StringBuilderCache(StringBuilder sb, string cacheKey, Action<StringBuilder> stringBuilderAction)
        {
            if (StringBuilderCacheEnabled && StringBuilderCacheDict.TryGetValue(cacheKey, out string value))
            {
                sb.Append(value);
                return;
            }

            StringBuilder newSb = new StringBuilder();
            stringBuilderAction(newSb);
            value = newSb.ToString();
            StringBuilderCacheDict.AddOrUpdate(cacheKey, value, (t, v) => value);
            sb.Append(value);
        }

        static SimpleCRUD()
        {
            SetDialect(_dialect);
        }

        /// <summary>
        /// <para>Deletes a record or records in the database that match the object passed in</para>
        /// <para>-By default deletes records in the table matching the class name</para>
        /// <para>Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns the number of records affected</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToDelete"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int Delete<T>(this DbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_Delete", sb =>
            {
                var idProps = entityToDelete.GetIdProperties().ToList();

                if (!idProps.Any())
                    throw new ArgumentException("Entity must have at least one [Key] or Id property");

                var name = GetTableName(entityToDelete);

                sb.AppendFormat("delete from {0}", name);

                sb.Append(" where ");
                BuildWhere<T>(sb, idProps, entityToDelete);

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("Delete: {0}", sb));
            });
            return connection.Execute(masterSb.ToString(), entityToDelete, transaction, commandTimeout);
        }

        public static int Delete<T>(this DbConnection connection, IEnumerable<T> entities, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return entities.Sum(entity => Delete<T>(connection, entity, transaction, commandTimeout));
        }

        /// <summary>
        /// <para>Deletes a record or records in the database by ID</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// Deletes records where the Id property and properties with the [Key] attribute match
        /// those in the database
        /// </para>
        /// <para>The number of records affected</para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int Delete<T>(this DbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Delete<TEntity> only supports an entity with a [Key] or Id property");

            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.AppendFormat("Delete from {0} where ", name);

            for (var i = 0; i < idProps.Count; i++)
            {
                if (i > 0)
                    sb.Append(" and ");
                sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
            }

            var dynParms = new DynamicParameters();
            if (idProps.Count == 1)
                dynParms.Add("@" + idProps.First().Name, id);
            else
            {
                foreach (var prop in idProps)
                    dynParms.Add("@" + prop.Name, id.GetType().GetProperty(prop.Name).GetValue(id, null));
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Delete<{0}> {1}", currenttype, sb));

            return connection.Execute(sb.ToString(), dynParms, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Deletes a list of records in the database</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Deletes records where that match the where clause</para>
        /// <para>
        /// whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}
        /// </para>
        /// <para>The number of records affected</para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int DeleteList<T>(this DbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_DeleteWhere{whereConditions?.GetType()?.FullName}", sb =>
            {
                var currenttype = typeof(T);
                var name = GetTableName(currenttype);

                var whereprops = GetAllProperties(whereConditions).ToArray();
                sb.AppendFormat("Delete from {0}", name);
                if (whereprops.Any())
                {
                    sb.Append(" where ");
                    BuildWhere<T>(sb, whereprops);
                }

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("DeleteList<{0}> {1}", currenttype, sb));
            });
            return connection.Execute(masterSb.ToString(), whereConditions, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Deletes a list of records in the database</para>
        /// <para>By default deletes records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Deletes records where that match the where clause</para>
        /// <para>conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age"</para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of records affected</returns>
        public static int DeleteList<T>(this DbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(T).FullName}_DeleteWhere{conditions}", sb =>
            {
                if (string.IsNullOrEmpty(conditions))
                    throw new ArgumentException("DeleteList<TEntity> requires a where clause");
                if (!conditions.ToLower().Contains("where"))
                    throw new ArgumentException("DeleteList<TEntity> requires a where clause and must contain the WHERE keyword");

                var currenttype = typeof(T);
                var name = GetTableName(currenttype);

                sb.AppendFormat("Delete from {0}", name);
                sb.Append(" " + conditions);

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("DeleteList<{0}> {1}", currenttype, sb));
            });
            return connection.Execute(masterSb.ToString(), parameters, transaction, commandTimeout);
        }

        /// <summary>
        /// Encapsulates a string with the characters used for encapsulating table and column names
        /// </summary>
        /// <param name="databaseword"></param>
        /// <returns></returns>
        public static string Encapsulate(string databaseword)
        {
            return string.Format(_encapsulation, databaseword);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>By default filters on the Id column</para>
        /// <para>
        /// -Id column name can be overridden by adding an attribute on your primary key property [Key]
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a single entity by a single id from table TEntity</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a single entity by a single id from table TEntity.</returns>
        public static T Get<T>(this DbConnection connection, object id, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);

            var idProps = GetIdProperties(currenttype).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Get<TEntity> only supports an entity with a [Key] or Id property");

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0} where ", name);

            for (var i = 0; i < idProps.Count; i++)
            {
                if (i > 0)
                    sb.Append(" and ");
                sb.AppendFormat("{0} = @{1}", GetColumnName(idProps[i]), idProps[i].Name);
            }

            var dynParms = new DynamicParameters();

            if (idProps.Count == 1)
            {
                if (id.GetType().IsSimpleType())
                {
                    dynParms.Add("@" + idProps.First().Name, id);
                }
                else if (id is IDictionary<string, object> dic)
                {
                    var prop = idProps.First();

                    if (dic.ContainsKey(prop.Name))
                    {
                        dynParms.Add("@" + prop.Name, dic[prop.Name]);
                    }
                }
                else
                {
                    var idProp = id.GetType().GetProperty(idProps.First().Name);
                    var v = idProp.GetValue(id, null);
                    dynParms.Add("@" + idProps.First().Name, v);
                }
            }
            else
            {
                if (id.GetType().IsSimpleType()) throw new ArgumentException($"{typeof(T).Name} needs a object with following properties: ${idProps.Select(x => x.Name).SelectJoinString(", ")}");

                if (id is IDictionary<string, object> dic)
                {
                    foreach (var prop in idProps)
                    {
                        if (dic.ContainsKey(prop.Name))
                        {
                            dynParms.Add("@" + prop.Name, dic[prop.Name]);
                        }
                    }
                }
                else
                {
                    foreach (var prop in idProps)
                    {
                        var idProp = id.GetType().GetProperty(prop.Name);
                        var v = idProp.GetValue(id, null);
                        dynParms.Add("@" + prop.Name, v);
                    }
                }
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Get<{0}>: {1} with Id: {2}", currenttype, sb, id));

            return connection.QueryFirstOrDefault<T>(sb.ToString(), dynParms, transaction, commandTimeout);
        }


        public static ColumnAttribute GetColumnAttribute(this PropertyInfo propertyInfo)
        {
            var aa = propertyInfo.GetCustomAttributes(true).FirstOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name);
            if (aa != null && aa is ColumnAttribute ca)
            {
                return ca;
            }
            return new ColumnAttribute(propertyInfo.Name);
        }

        public static ColumnAttribute GetColumnAttribute<T, V>(this Expression<Func<T, V>> expression) => GetProperty(expression).GetColumnAttribute();

        public static PropertyInfo GetProperty<T, V>(this Expression<Func<T, V>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member as PropertyInfo;
            }
            throw new ArgumentException("Expression must be a property expression");
        }

        public static string GetColumnName(this Type classType, string searchName) => GetScaffoldableProperties(classType).FirstOrDefault(x => x.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase) || x.GetColumnAttribute().Name == searchName)?.Name ?? searchName;

        public static string GetColumnName<T>(PropertyInfo propertyInfo) => GetColumnName(propertyInfo, typeof(T));
        public static string GetColumnName(PropertyInfo propertyInfo) => GetColumnName(propertyInfo, null);
        public static string GetColumnName(PropertyInfo propertyInfo, Type classType)
        {
            classType = classType ?? propertyInfo.DeclaringType;
            string columnName, key = string.Format("{0}.{1}", classType, propertyInfo.Name);

            if (ColumnNames.TryGetValue(key, out columnName))
            {
                return columnName;
            }
            if (propertyInfo.DeclaringType != classType)
            {
                propertyInfo = classType.GetProperties().FirstOrDefault(x => x.Name == propertyInfo.Name);
                if (propertyInfo == null)
                {
                    throw new ArgumentException($"{classType.Name} wont have a property with name {propertyInfo.Name}", nameof(propertyInfo));
                }
            }

            columnName = _columnNameResolver.ResolveColumnName(propertyInfo);

            ColumnNames.AddOrUpdate(key, columnName, (t, v) => columnName);

            return columnName;
        }

        public static string GetColumnName<T, V>(Expression<Func<T, V>> propertyExpression) => GetColumnName(GetProperty(propertyExpression));

        public static string GetColumnName<T, V>(this T obj, Expression<Func<T, V>> propertyExpression) => GetColumnName(propertyExpression);

        public static ColumnAttribute GetColumnAttribute<T, V>(this T obj, Expression<Func<T, V>> propertyExpression) => propertyExpression.GetColumnAttribute<T, V>();

        /// <summary>
        /// Returns the current dialect name
        /// </summary>
        /// <returns></returns>
        public static string GetDialect()
        {
            return _dialect.ToString();
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetList<T>(this DbConnection connection, object whereConditions = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            if (whereConditions == null) whereConditions = new { };

            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0}", name);

            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere<T>(sb, whereprops, whereConditions);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetList<{0}>: {1}", currenttype, sb));

            return connection.Query<T>(sb.ToString(), whereConditions, transaction, true, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// conditions is an SQL where clause and/or order by clause ex: "where name='bob'" or
        /// "where age&gt;=@Age"
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a list of entities with optional SQL where conditions</returns>
        public static IEnumerable<T> GetList<T>(this DbConnection connection, string conditions, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            sb.Append("Select ");
            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            sb.AppendFormat(" from {0} ", name);
            sb.Append(" ");

            if (string.IsNullOrWhiteSpace(conditions) == false)
            {
                conditions = conditions.Trim();

                if (!conditions.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" where ");
                }

                sb.Append(conditions);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetList<{0}>: {1}", currenttype, sb));

            return connection.Query<T>(sb.ToString(), parameters, transaction, true, commandTimeout);
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age" - not required
        /// </para>
        /// <para>
        /// orderby is a column or list of columns to order by ex: "lastname, age desc" - not
        /// required - default is by primary key
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="pageNumber">pageNumber (starts with 0)</param>
        /// <param name="rowsPerPage"></param>
        /// <param name="conditions"></param>
        /// <param name="orderby"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a paged list of entities with optional exact match where conditions</returns>
        /// <returns></returns>
        public static PageResult<T> GetPage<T>(this DbConnection connection, int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, bool IncludeInsertPlaceholder = false) where T : class
        {
            var result = new PageResult<T>();

            //antes de setar o range, verifica se explicitamente foi a primeira pagina
            if (IncludeInsertPlaceholder && pageNumber == 0 && rowsPerPage >= 1)
            {
                try
                {
                    result.InsertPlaceholder = Activator.CreateInstance<T>();
                }
                catch
                {
                    result.InsertPlaceholder = default(T);
                }
            }

            var TotalItems = connection.RecordCount<T>(conditions, parameters, transaction, commandTimeout);

            var TotalPages = 0;

            if (rowsPerPage > 0)
            {
                TotalPages = (int)Math.Ceiling(TotalItems / (decimal)rowsPerPage);
            }

            if (TotalPages < 0)
            {
                TotalPages = 1;
            }

            if (pageNumber < 0) pageNumber = 0;
            if (pageNumber > TotalPages) pageNumber = TotalPages;

            if (rowsPerPage == 0)
            {
                rowsPerPage = TotalItems;
            }
            else if (rowsPerPage < 0)
            {
                //count only
            }

            if (rowsPerPage > 0)
            {
                result.TotalPages = TotalPages;
                result.PageSize = rowsPerPage;
                result.PageNumber = pageNumber;
                result.Items = connection.GetListPaged<T>(pageNumber, rowsPerPage, conditions, orderby, parameters, transaction, commandTimeout);
            }

            result.TotalItems = TotalItems;

            return result;
        }

        /// <inheritdoc cref="GetPage{T}(DbConnection, int, int, string, string, object, IDbTransaction, int?, bool)"/>
        public static PageResult<T> GetPage<T, V>(this DbConnection connection, int pageNumber, int rowsPerPage, string conditions, Expression<Func<T, V>> orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null, bool IncludeInsertPlaceholder = false) where T : class
            => GetPage<T>(connection, pageNumber, rowsPerPage, conditions, GetColumnName(orderby), parameters, transaction, commandTimeout, IncludeInsertPlaceholder);

        /// <inheritdoc cref="GetListPaged{T}(DbConnection, int, int, string, string, object, IDbTransaction, int?)"/>
        public static IEnumerable<T> GetListPaged<T, V>(this DbConnection connection, int pageNumber, int rowsPerPage, string conditions, Expression<Func<T, V>> orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class => GetListPaged<T>(connection, pageNumber, rowsPerPage, conditions, GetColumnName(orderby), parameters, transaction, commandTimeout);

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age" - not required
        /// </para>
        /// <para>
        /// orderby is a column or list of columns to order by ex: "lastname, age desc" - not
        /// required - default is by primary key
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns a list of entities that match where conditions</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="pageNumber">pageNumber (starts with 0)</param>
        /// <param name="rowsPerPage"></param>
        /// <param name="conditions"></param>
        /// <param name="orderby"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Gets a paged list of entities with optional exact match where conditions</returns>
        public static IEnumerable<T> GetListPaged<T>(this DbConnection connection, int pageNumber, int rowsPerPage, string conditions, string orderby, object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (string.IsNullOrEmpty(_getPagedListSql))
                throw new Exception("GetListPage is not supported with the current SQL Dialect");

            if (pageNumber < 0)
                pageNumber = 1;

            pageNumber++;

            var currenttype = typeof(T);
            var idProps = GetIdProperties(currenttype).ToList();

            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            var query = _getPagedListSql;
            if (string.IsNullOrEmpty(orderby))
            {
                var props = idProps.FirstOrDefault() ?? GetAllProperties<T>().FirstOrDefault();
                if (props == null) throw new ArgumentException("Entity must have at least one [Key] or [Column] property");
                orderby = GetColumnName(props);
            }

            //create a new empty instance of the type to get the base properties
            BuildSelect(sb, GetScaffoldableProperties<T>().ToArray());
            query = query.Replace("{SelectColumns}", sb.ToString());
            query = query.Replace("{TableName}", name);
            query = query.Replace("{pageNumber}", pageNumber.ToString());
            query = query.Replace("{RowsPerPage}", rowsPerPage.ToString());
            query = query.Replace("{OrderBy}", orderby);
            query = query.Replace("{WhereClause}", conditions);
            query = query.Replace("{Offset}", ((pageNumber - 1) * rowsPerPage).ToString());

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("GetListPaged<{0}>: {1}", currenttype, query));

            return connection.Query<T>(query, parameters, transaction, true, commandTimeout);
        }

        //Gets the table name for this type
        //For Get(id) and Delete(id) we don'classType have an entity, just the type so this method is used
        //Use dynamic type to be able to handle both our Table-attribute and the DataAnnotation
        //Uses class name by default and overrides if the class has a Table attribute
        public static string GetTableName(Type type)
        {
            string tableName;

            if (TableNames.TryGetValue(type, out tableName))
                return tableName;

            tableName = _tableNameResolver.ResolveTableName(type);

            TableNames.AddOrUpdate(type, tableName, (t, v) => tableName);

            return tableName;
        }

        public static string GetTableName<T, V>(this Expression<Func<T, V>> exp) => GetTableName(typeof(T));

        public static string GetTableName<T>(this T entity)
        {
            if (entity is Type t) return GetTableName(t);
            return GetTableName(entity.GetType());
        }

        public static string GetTableName<T>() => GetTableName(typeof(T));

        /// <summary>
        /// <para>Inserts a row into the database</para>
        /// <para>By default inserts into the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// Returns the ID (primary key) of the newly inserted record if it is identity using the
        /// int? type, otherwise null
        /// </para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>
        /// The ID (primary key) of the newly inserted record if it is identity using the int? type,
        /// otherwise null
        /// </returns>
        public static TEntity Insert<TEntity>(this DbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class => Insert<TEntity, TEntity>(connection, entityToInsert, transaction, commandTimeout);

        /// <summary>
        /// <para>Inserts a row into the database, using ONLY the properties defined by TEntity</para>
        /// <para>By default inserts into the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Insert filters out Id column and any columns with the [Key] attribute</para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// Returns the ID (primary key) of the newly inserted record if it is identity using the
        /// defined type, otherwise null
        /// </para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToInsert"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>
        /// The ID (primary key) of the newly inserted record if it is identity using the defined
        /// type, otherwise null
        /// </returns>
        public static TKey Insert<TKey, TEntity>(this DbConnection connection, TEntity entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            if (typeof(TEntity).IsInterface) //FallBack to BaseType Generic Method : https://stackoverflow.com/questions/4101784/calling-a-generic-method-with-a-dynamic-type
            {
                return (TKey)typeof(SimpleCRUD)
                    .GetMethods().Where(methodInfo => methodInfo.Name == nameof(Insert) && methodInfo.GetGenericArguments().Count() == 2).Single()
                    .MakeGenericMethod(new Type[] { typeof(TKey), entityToInsert.GetType() })
                    .Invoke(null, new object[] { connection, entityToInsert, transaction, commandTimeout });
            }
            var idProps = GetIdProperties(entityToInsert).ToList();

            if (!idProps.Any())
                throw new ArgumentException("Insert<TEntity> only supports an entity with a [Key] or Id property");

            var name = GetTableName(entityToInsert);
            var sb = new StringBuilder();
            sb.AppendFormat("insert into {0}", name);
            sb.Append(" (");
            BuildInsertParameters<TEntity>(sb);
            sb.Append(") ");
            sb.Append("values");
            sb.Append(" (");
            BuildInsertValues<TEntity>(sb);
            sb.Append(")");

            var keyHasPredefinedValue = false;
            var baseType = typeof(TKey);
            var underlyingType = Nullable.GetUnderlyingType(baseType);
            var keytype = underlyingType ?? baseType;

            if (keytype != typeof(TEntity) && keytype != typeof(int) && keytype != typeof(uint) && keytype != typeof(long) && keytype != typeof(ulong) && keytype != typeof(short) && keytype != typeof(ushort) && keytype != typeof(Guid) && keytype != typeof(string))
            {
                throw new Exception("Invalid return type");
            }

            if (keytype == typeof(Guid))
            {
                var guidvalue = (Guid)idProps.First().GetValue(entityToInsert, null);
                if (guidvalue == Guid.Empty)
                {
                    var newguid = SequentialGuid();
                    idProps.First().SetValue(entityToInsert, newguid, null);
                }
                else
                {
                    keyHasPredefinedValue = true;
                }
                sb.Append(";select '" + idProps.First().GetValue(entityToInsert, null) + "' as id");
            }

            try
            {
                if ((keytype == typeof(int) || keytype == typeof(long)) && Convert.ToInt64(idProps.First().GetValue(entityToInsert, null)) == 0)
                {
                    sb.Append(";" + _getIdentitySql);
                }
                else
                {
                    keyHasPredefinedValue = true;
                }
            }
            catch
            {
                keyHasPredefinedValue = true;
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("Insert: {0}", sb));

            var r = connection.Query<TKey>(sb.ToString(), entityToInsert, transaction, true, commandTimeout).FirstOrDefault();

            if (r != null) return r;

            if (keytype == typeof(TEntity))
            {
                var ee = connection.Get<TEntity>(entityToInsert, transaction, commandTimeout);
                if (ee is TKey kk)
                {
                    return kk;
                }
            }

            if (keytype == typeof(Guid) || keyHasPredefinedValue)
            {
                var v = idProps.First().GetValue(entityToInsert, null);
                if (v is TKey key)
                {
                    return key;
                }
                else
                {
                    return (TKey)Convert.ChangeType(v, keytype);
                }
            }

            throw new DataException("The entity must have a [Key] or Id property");
        }

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Returns a number of records entity by a single id from table TEntity</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// conditions is an SQL where clause ex: "where name='bob'" or "where age&gt;=@Age" - not required
        /// </para>
        /// <para>
        /// parameters is an anonymous type to pass in named parameter values: new { Age = 15 }
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount<T>(this DbConnection connection, string conditions = "", object parameters = null, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);
            var sb = new StringBuilder();
            sb.Append("Select count(0)");
            sb.AppendFormat(" from {0} ", name);

            if (!string.IsNullOrWhiteSpace(conditions))
            {
                conditions = conditions.Trim();
                if (!conditions.Trim().StartsWith("where", StringComparison.OrdinalIgnoreCase))
                {
                    sb.Append(" where ");
                }
                sb.Append(conditions);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("RecordCount<{0}>: {1}", currenttype, sb));

            return connection.ExecuteScalar<int>(sb.ToString(), parameters, transaction, commandTimeout);
        }

        public static bool Exists<T>(this DbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
            => RecordCount<T>(connection, whereConditions, transaction, commandTimeout) > 0;

        /// <summary>
        /// <para>By default queries the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>Returns a number of records entity by a single id from table TEntity</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>
        /// whereConditions is an anonymous type to filter the results ex: new {Category = 1, SubCategory=2}
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="whereConditions"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>Returns a count of records.</returns>
        public static int RecordCount<T>(this DbConnection connection, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var currenttype = typeof(T);
            var name = GetTableName(currenttype);

            var sb = new StringBuilder();
            var whereprops = GetAllProperties(whereConditions).ToArray();
            sb.Append("Select count(0)");
            sb.AppendFormat(" from {0}", name);
            if (whereprops.Any())
            {
                sb.Append(" where ");
                BuildWhere<T>(sb, whereprops);
            }

            if (Debugger.IsAttached)
                Trace.WriteLine(String.Format("RecordCount<{0}>: {1}", currenttype, sb));

            return connection.ExecuteScalar<int>(sb.ToString(), whereConditions, transaction, commandTimeout);
        }

        /// <summary>
        /// Generates a GUID based on the current date/time http://stackoverflow.com/questions/1752004/sequential-guid-generator-c-sharp
        /// </summary>
        /// <returns></returns>
        public static Guid SequentialGuid()
        {
            var tempGuid = Guid.NewGuid();
            var bytes = tempGuid.ToByteArray();
            var time = DateTime.Now;
            bytes[3] = (byte)time.Year;
            bytes[2] = (byte)time.Month;
            bytes[1] = (byte)time.Day;
            bytes[0] = (byte)time.Hour;
            bytes[5] = (byte)time.Minute;
            bytes[4] = (byte)time.Second;
            return new Guid(bytes);
        }

        /// <summary>
        /// Sets the column name resolver
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a column name</param>
        public static void SetColumnNameResolver(IColumnNameResolver resolver)
        {
            _columnNameResolver = resolver;
        }

        /// <summary>
        /// Sets the database dialect
        /// </summary>
        /// <param name="dialect"></param>
        public static void SetDialect(Dialect dialect)
        {
            switch (dialect)
            {
                case Dialect.PostgreSQL:
                    _dialect = Dialect.PostgreSQL;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT LASTVAL() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({pageNumber}-1) * {RowsPerPage})";
                    break;

                case Dialect.SQLite:
                    _dialect = Dialect.SQLite;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT LAST_INSERT_ROWID() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {RowsPerPage} OFFSET (({pageNumber}-1) * {RowsPerPage})";
                    break;

                case Dialect.MySQL:
                    _dialect = Dialect.MySQL;
                    _encapsulation = "`{0}`";
                    _getIdentitySql = string.Format("SELECT LAST_INSERT_ID() AS id");
                    _getPagedListSql = "Select {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy} LIMIT {Offset},{RowsPerPage}";
                    break;

                case Dialect.Oracle:
                    _dialect = Dialect.Oracle;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = "";
                    _getPagedListSql = "SELECT * FROM (SELECT ROWNUM PagedNUMBER, u.* FROM(SELECT {SelectColumns} from {TableName} {WhereClause} Order By {OrderBy}) u) WHERE PagedNUMBER BETWEEN (({pageNumber}-1) * {RowsPerPage} + 1) AND ({pageNumber} * {RowsPerPage})";
                    break;

                case Dialect.DB2:
                    _dialect = Dialect.DB2;
                    _encapsulation = "\"{0}\"";
                    _getIdentitySql = string.Format("SELECT CAST(IDENTITY_VAL_LOCAL() AS DEC(31,0)) AS \"id\" FROM SYSIBM.SYSDUMMY1");
                    _getPagedListSql = "Select * from (Select {SelectColumns}, row_number() over(order by {OrderBy}) as PagedNumber from {TableName} {WhereClause} Order By {OrderBy}) as classType where classType.PagedNumber between (({pageNumber}-1) * {RowsPerPage} + 1) AND ({pageNumber} * {RowsPerPage})";
                    break;

                default:
                    _dialect = Dialect.SQLServer;
                    _encapsulation = "[{0}]";
                    _getIdentitySql = string.Format("SELECT CAST(SCOPE_IDENTITY()  AS BIGINT) AS [id]");
                    _getPagedListSql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY {OrderBy}) AS PagedNumber, {SelectColumns} FROM {TableName} {WhereClause}) AS u WHERE PagedNumber BETWEEN (({pageNumber}-1) * {RowsPerPage} + 1) AND ({pageNumber} * {RowsPerPage})";
                    break;
            }
        }

        /// <summary>
        /// Sets the table name resolver
        /// </summary>
        /// <param name="resolver">The resolver to use when requesting the format of a table name</param>
        public static void SetTableNameResolver(ITableNameResolver resolver)
        {
            _tableNameResolver = resolver;
        }

        /// <summary>
        /// <para>Updates a record or records in the database with only the properties of TEntity</para>
        /// <para>By default updates records in the table matching the class name</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")]</para>
        /// <para>
        /// Updates records where the Id property and properties with the [Key] attribute match
        /// those in the database.
        /// </para>
        /// <para>Properties marked with attribute [Editable(false)] and complex types are ignored</para>
        /// <para>Supports transaction and command timeout</para>
        /// <para>Returns number of rows affected</para>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns>The number of affected records</returns>
        public static int Update<TEntity>(this DbConnection connection, TEntity entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            if (typeof(TEntity).IsInterface) //FallBack to BaseType Generic Method: https://stackoverflow.com/questions/4101784/calling-a-generic-method-with-a-dynamic-type
            {
                return (int)typeof(SimpleCRUD)
                    .GetMethods().Where(methodInfo => methodInfo.Name == nameof(Update) && methodInfo.GetGenericArguments().Count() == 1).Single()
                    .MakeGenericMethod(new Type[] { entityToUpdate.GetType() })
                    .Invoke(null, new object[] { connection, entityToUpdate, transaction, commandTimeout });
            }
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(TEntity).FullName}_Update", sb =>
            {
                var idProps = GetIdProperties(entityToUpdate).ToList();

                if (!idProps.Any())
                    throw new ArgumentException("Entity must have at least one [Key] or Id property");

                var name = GetTableName(entityToUpdate);

                sb.AppendFormat("update {0}", name);

                sb.AppendFormat(" set ");
                BuildUpdateSet(entityToUpdate, sb);
                sb.Append(" where ");
                BuildWhere<TEntity>(sb, idProps, entityToUpdate);

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("Update: {0}", sb));
            });
            return connection.Execute(masterSb.ToString(), entityToUpdate, transaction, commandTimeout);
        }

        public static int UpdateWhere<TEntity, Tup>(this DbConnection connection, TEntity entityToUpdate, Tup whereConditions, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class where Tup : class

        {
            if (typeof(TEntity).IsInterface) //FallBack to BaseType Generic Method: https://stackoverflow.com/questions/4101784/calling-a-generic-method-with-a-dynamic-type
            {
                return (int)typeof(SimpleCRUD)
                    .GetMethods().Where(methodInfo => methodInfo.Name == nameof(UpdateWhere) && methodInfo.GetGenericArguments().Count() == 2).Single()
                    .MakeGenericMethod(new Type[] { entityToUpdate.GetType() })
                    .Invoke(null, new object[] { connection, entityToUpdate, whereConditions, transaction, commandTimeout });
            }
            var masterSb = new StringBuilder();
            var para = new DynamicParameters();
            StringBuilderCache(masterSb, $"{typeof(TEntity).FullName}_Update", sb =>
            {
                var whereProps = GetAllProperties(whereConditions).ToList();

                var name = GetTableName(entityToUpdate);

                sb.AppendFormat("update {0}", name);

                sb.AppendFormat(" set ");
                BuildUpdateSet(entityToUpdate, sb);

                GetUpdateableProperties(entityToUpdate).ToList().ForEach(property =>
                {
                    para.Add(property.Name, property.GetValue(entityToUpdate));
                });

                if (whereProps.Any())
                {
                    sb.Append(" where ");
                    BuildWhere<TEntity>(sb, whereProps, whereProps);
                    foreach (var prop in whereProps)
                    {
                        para.Add(prop.Name, prop.GetValue(whereConditions));
                    }
                }

                if (Debugger.IsAttached)
                    Trace.WriteLine(String.Format("Update: {0}", sb));
            });
            return connection.Execute(masterSb.ToString(), para, transaction, commandTimeout);
        }

        public static int UpdateWhere<TEntity>(this DbConnection connection, TEntity fieldsToUpdate, string whereConditions, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            var masterSb = new StringBuilder();
            StringBuilderCache(masterSb, $"{typeof(TEntity).FullName}_UpdateWhere", sb =>
            {
                var name = GetTableName<TEntity>();
                sb.AppendFormat("update {0}", name);
                sb.Append(" set ");
                BuildUpdateSet(fieldsToUpdate, sb);
                sb.Append(" ");

                if (string.IsNullOrWhiteSpace(whereConditions) == false)
                {
                    whereConditions = whereConditions.Trim();
                    if (whereConditions.StartsWith("where", StringComparison.OrdinalIgnoreCase) == false)
                    {
                        sb.Append("where ");
                    }

                    sb.Append(whereConditions);
                }
            });
            return connection.Execute(masterSb.ToString(), fieldsToUpdate, transaction, commandTimeout);
        }

        public static int UpdateWhere<TEntity>(this DbConnection connection, TEntity entityToUpdate, Expression<Func<TEntity, bool>> whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            return UpdateWhere(connection, entityToUpdate, new[] { whereConditions }, transaction, commandTimeout);
        }
        public static int UpdateWhere<TEntity>(this DbConnection connection, TEntity entityToUpdate, Expression<Func<TEntity, bool>>[] whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var sb = new StringBuilder();
            sb.Append("update ");
            sb.Append(GetTableName(entityToUpdate));
            sb.Append(" set ");
            BuildUpdateSet(entityToUpdate, sb);
            var parameters = new DynamicParameters();
            for (int i = 0; i < whereConditions.Length; i++)
            {
                var condition = whereConditions[i];
                if (condition.Body is BinaryExpression binaryExpression)
                {
                    MemberExpression left = binaryExpression.Left as MemberExpression;
                    if (left != null)
                    {
                        var columnName = GetColumnName(left.Member as PropertyInfo);
                        var parameterName = left.Member.Name;
                        Expression right = binaryExpression.Right as ConstantExpression;
                        object parameterValue;
                        if (right is ConstantExpression c)
                        {
                            parameterValue = c.Value;
                        }
                        else if (right is MemberExpression m)
                        {
                            parameterValue = Expression.Lambda(m).Compile().DynamicInvoke();
                        }
                        else
                        {
                            continue;
                        }
                        parameters.Add("@" + parameterName, parameterValue);
                        sb.AppendFormat("{0} = @{1}", columnName, parameterName);
                    }
                }
                if (i < whereConditions.Length - 1)
                {
                    sb.Append(" AND ");
                }
            }

            return connection.Execute(sb.ToString(), entityToUpdate, transaction, commandTimeout);
        }

        public static int UpdateColumn<TEntity, V>(this DbConnection connection, Expression<Func<TEntity, object>> column, V value, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null) => UpdateColumn(connection, new[] { column }, value, whereConditions, transaction, commandTimeout);

        public static int UpdateColumn<TEntity, V>(this DbConnection connection, IEnumerable<Expression<Func<TEntity, object>>> column, V value, object whereConditions, IDbTransaction transaction = null, int? commandTimeout = null)
        {
            var sb = new StringBuilder();
            DynamicParameters para = new DynamicParameters();

            var name = GetTableName(typeof(TEntity));
            sb.AppendFormat("update {0}", name);
            sb.AppendFormat(" set ", name);
            foreach (var col in column)
            {
                var pname = $"value_{DateTime.Now.Ticks}";
                para.Add(pname, value);
                sb.AppendFormat(" {0} = @{1}, ", GetColumnName(col), pname);
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" ");
            if (whereConditions != null)
            {
                sb.Append(" WHERE ");

                var whereprops = GetAllProperties(whereConditions).ToArray();
                BuildWhere<TEntity>(sb, whereprops);
                foreach (var prop in whereprops)
                {
                    para.Add(prop.Name, prop.GetValue(whereConditions));
                }
            }

            return connection.Execute(sb.ToString(), para, transaction, commandTimeout);
        }

        /// <summary>
        /// <para>Inserts or updates a record in the database.</para>
        /// <para>If the record exists, it updates the record.</para>
        /// <para>If the record does not exist, it inserts a new record.</para>
        /// <para>By default, it uses the table matching the class name.</para>
        /// <para>-Table name can be overridden by adding an attribute on your class [Table("YourTableName")].</para>
        /// <para>Supports transaction and command timeout.</para>
        /// <para>Returns TRUE if new record was inserted, otherwise returns FALSE.</para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="connection">The database connection.</param>
        /// <param name="entity">The entity to insert or update.</param>
        /// <param name="transaction">The transaction to use, or null.</param>
        /// <param name="commandTimeout">The command timeout, or null.</param>
        /// <returns>TRUE if the record was inserted, FALSE if the record was updated.</returns>
        public static bool Upsert<TEntity>(this DbConnection connection, TEntity entity, IDbTransaction transaction = null, int? commandTimeout = null) where TEntity : class
        {
            var exists = Get<TEntity>(connection, entity, transaction, commandTimeout);
            if (exists == null)
            {
                Insert<TEntity, TEntity>(connection, entity, transaction, commandTimeout);
                return true;
            }
            else
            {
                Update(connection, entity, transaction, commandTimeout);
                return false;
            }
        }

        public static string CacheKey(this IEnumerable<PropertyInfo> props) => string.Join(",", props.Select(p => p.DeclaringType.FullName + "." + p.Name).ToArray());

        /// <summary>
        /// Database server dialects
        /// </summary>
        public enum Dialect
        {
            SQLServer,
            PostgreSQL,
            SQLite,
            MySQL,
            Oracle,
            DB2
        }

        public class ColumnNameResolver : IColumnNameResolver
        {
            public virtual string ResolveColumnName(PropertyInfo propertyInfo)
            {
                string columnName;

                if (GetDialect() == Dialect.DB2.ToString())
                {
                    columnName = propertyInfo.Name;
                }
                else
                {
                    columnName = Encapsulate(propertyInfo.Name);
                }

                var columnattr = propertyInfo.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(ColumnAttribute).Name) as dynamic;
                if (columnattr != null)
                {
                    columnName = Encapsulate(columnattr.Name);
                    if (Debugger.IsAttached)
                        Trace.WriteLine(String.Format("Column name for type overridden from {0} to {1}", propertyInfo.Name, columnName));
                }
                return columnName;
            }
        }

        public class TableNameResolver : ITableNameResolver
        {
            public virtual string ResolveTableName(Type type)
            {
                string tableName;

                if (GetDialect() == Dialect.DB2.ToString())
                {
                    tableName = type.Name;
                }
                else
                {
                    tableName = Encapsulate(type.Name);
                }

                var tableattr = type.GetCustomAttributes(true).SingleOrDefault(attr => attr.GetType().Name == typeof(TableAttribute).Name) as dynamic;
                if (tableattr != null)
                {
                    tableName = Encapsulate(tableattr.Name);
                    try
                    {
                        if (!String.IsNullOrEmpty(tableattr.Schema))
                        {
                            string schemaName = Encapsulate(tableattr.Schema);
                            tableName = String.Format("{0}.{1}", schemaName, tableName);
                        }
                    }
                    catch
                    {
                        //Schema doesn't exist on this attribute.
                    }
                }

                return tableName;
            }
        }

        public interface IColumnNameResolver
        {
            string ResolveColumnName(PropertyInfo propertyInfo);
        }

        public interface ITableNameResolver
        {
            string ResolveTableName(Type type);
        }

        /// <summary>
        /// Validate an entity for required fields and string lengths
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to validate.</param>
        /// <param name="throwError">When true, throws a ValidationException if validation fails.</param>
        /// <returns>An IEnumerable of strings containing the validation errors.</returns>
        /// <exception cref="ValidationException"></exception>
        public static IEnumerable<string> ValidateEntity<TEntity>(this TEntity entity, bool throwError = false) where TEntity : class
        {
            var errors = new List<string>();
            var properties = GetScaffoldableProperties<TEntity>().Union(GetIdProperties<TEntity>()).ToList();
            foreach (var prop in properties)
            {
                var value = prop.GetValue(entity);

                //required
                if (value == null && prop.GetCustomAttributes(true).Any(x => x.GetType().Name == typeof(RequiredAttribute).Name))
                {
                    var attr = prop.GetCustomAttributes(true).SingleOrDefault(x => x.GetType().Name == typeof(RequiredAttribute).Name);
                    var message = "";
                    if (attr.GetType().GetProperty(nameof(RequiredAttribute.ErrorMessage)) is PropertyInfo pi)
                    {
                        message = pi.GetValue(attr) as string;
                    }
                    if (string.IsNullOrEmpty(message))
                    {
                        message = $"{prop.Name} is required.";
                    }
                    errors.Add(message);
                }

                if (value is string s)
                {
                    //maxlength
                    if (prop.GetCustomAttributes(true).Any(x => x.GetType().Name == typeof(MaxLengthAttribute).Name))
                    {
                        var attr = prop.GetCustomAttributes(true).SingleOrDefault(x => x.GetType().Name == typeof(MaxLengthAttribute).Name);
                        if (attr.GetType().GetProperty(nameof(MaxLengthAttribute.Length)) is PropertyInfo pi)
                        {
                            if (pi.GetValue(attr) is int length)
                            {
                                if (s.Length > length)
                                {
                                    var message = "";
                                    if (attr.GetType().GetProperty(nameof(MaxLengthAttribute.ErrorMessage)) is PropertyInfo pi2)
                                    {
                                        message = pi2.GetValue(attr) as string;
                                    }
                                    if (string.IsNullOrWhiteSpace(message))
                                    {
                                        message = $"{prop.Name} must be less than {length} characters.";
                                    }
                                    message += $" ({s})";
                                    errors.Add(message);
                                }
                            }
                        }
                    }

                    //minlength
                    if (prop.GetCustomAttributes(true).Any(x => x.GetType().Name == typeof(MinLengthAttribute).Name))
                    {
                        var attr = prop.GetCustomAttributes(true).SingleOrDefault(x => x.GetType().Name == typeof(MinLengthAttribute).Name);
                        if (attr.GetType().GetProperty(nameof(MinLengthAttribute.Length)) is PropertyInfo pi)
                        {
                            if (pi.GetValue(attr) is int length)
                            {
                                if (s.Length < length)

                                {
                                    var message = "";
                                    if (attr.GetType().GetProperty(nameof(MinLengthAttribute.ErrorMessage)) is PropertyInfo pi2)
                                    {
                                        message = pi2.GetValue(attr) as string;
                                    }
                                    if (string.IsNullOrWhiteSpace(message))
                                    {
                                        message = $"{prop.Name} must be more than {length} characters.";
                                    }
                                    message += $" ({s})";
                                    errors.Add(message);
                                }
                            }
                        }
                    }
                }
            }
            errors = errors.Distinct().ToList();

            if (throwError && errors.Any())
            {
                ValidationException ex = new ValidationException(string.Join(Environment.NewLine, errors.Select(x => $" - {x}")));
                ex.Data.Add("ValidationErrors", errors);
                throw ex;
            }
            return errors;
        }

        public static string GenerateWhereClause<T>(Expression<Func<T, bool>> expression)
        {
            var visitor = new WhereClauseVisitor();
            visitor.Visit(expression);
            return visitor.WhereClause.ToString();
        }

        private class WhereClauseVisitor : ExpressionVisitor
        {
            private StringBuilder _whereClause;

            public WhereClauseVisitor()
            {
                _whereClause = new StringBuilder();
            }

            public StringBuilder WhereClause => _whereClause;

            protected override Expression VisitBinary(BinaryExpression node)
            {
                _whereClause.Append("(");
                Visit(node.Left);
                _whereClause.Append(node.NodeType.GetSQLOperator());
                Visit(node.Right);
                _whereClause.Append(")");
                return node;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                _whereClause.Append(SimpleCRUD.GetColumnName(node.Member.DeclaringType, node.Member.Name));
                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Type.IsSimpleType())
                {
                    _whereClause.Append(node.Value.ToSQLString());
                }
                else
                {
                    _whereClause.Append(node.Value);
                }
                return node;
            }
        }

        /// <summary>
        /// Optional Editable attribute. You can use the System.ComponentModel.DataAnnotations version
        /// in its place to specify the properties that are editable
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class EditableAttribute : Attribute
        {
            /// <summary>
            /// Optional Editable attribute.
            /// </summary>
            /// <param name="iseditable"></param>
            public EditableAttribute(bool iseditable)
            {
                AllowEdit = iseditable;
            }

            /// <summary>
            /// Does this property persist to the database?
            /// </summary>
            public bool AllowEdit { get; private set; }
        }

        /// <summary>
        /// Optional IgnoreInsert attribute. Custom for Dapper.SimpleCRUD to exclude a property from
        /// Insert methods
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class IgnoreInsertAttribute : Attribute
        {
        }

        /// <summary>
        /// Optional IgnoreSelect attribute. Custom for Dapper.SimpleCRUD to exclude a property from
        /// Select methods
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class IgnoreSelectAttribute : Attribute
        {
        }

        /// <summary>
        /// Optional IgnoreUpdate attribute. Custom for Dapper.SimpleCRUD to exclude a property from
        /// Update methods
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class IgnoreUpdateAttribute : Attribute
        {
        }

        /// <summary>
        /// Represent a Paginated Result of a query with pagination information
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        public class PageResult<TEntity>
        {
            internal PageResult() { }

            public int? PreviousPage
            {
                get
                {
                    if (PageNumber.HasValue)
                    {
                        var pg = PageNumber - 1;

                        if (pg < 1)
                        {
                            pg = LastPage;
                        }
                        return pg;
                    }

                    return null;
                }
            }
            public int? NextPage
            {
                get
                {
                    if (PageNumber.HasValue)
                    {
                        var pg = PageNumber + 1;
                        if (pg > LastPage)
                        {
                            pg = 1;
                        }
                        return pg;
                    }
                    return null;
                }
            }

            public bool IsFirstPage => PageNumber == 0;
            public bool IsLastPage => PageNumber == LastPage;

            public int? LastPage => TotalPages;

            /// <summary>
            /// Current Page Number
            /// </summary>
            public int? PageNumber { get; internal set; }
            /// <summary>
            /// Items in the current page
            /// </summary>
            public IEnumerable<TEntity> Items { get; internal set; }

            /// <summary>
            /// Page Size (items per page). Can be null if the query is not paged
            /// </summary>
            public int? PageSize { get; internal set; }

            /// <summary>
            /// Total Items in the query
            /// </summary>
            public int TotalItems { get; internal set; }

            /// <summary>
            /// Total Pages in the query. Can be null if the query is not paged
            /// </summary>
            public int? TotalPages { get; internal set; }

            /// <summary>
            /// Intance of TEntity to be inserted in the first position of the list, used to represent a
            /// empty item with default values. This is only inserted in the first page.
            /// </summary>
            public TEntity InsertPlaceholder { get; internal set; }

            /// <summary>
            /// Return a List of TEntity. Includes the InsertPlaceholder if it is the first page and it
            /// is not null.
            /// </summary>
            /// <returns></returns>
            public List<TEntity> GetList()
            {
                var l = Items?.ToList() ?? new List<TEntity>();
                if (InsertPlaceholder != null)
                {
                    l.Insert(0, InsertPlaceholder);
                }
                return l;
            }

            public IEnumerable<int> GetPages() => Enumerable.Range(0, TotalPages ?? 0);

            /// <summary>
            /// Return a Dictionary with the page number and the page number button text
            /// </summary>
            /// <param name="PaginationOffset"></param>
            /// <returns></returns>
            public Dictionary<string, int> GetPaginationButtons(int PaginationOffset)
            {
                if (TotalPages.HasValue)
                {
                    var pages = GetPages(PaginationOffset).Select(x => (Button: x.ToString(), Number: x)).ToList();
                    if (pages.Any(x => x.Number == 0) == false)
                    {
                        pages.Insert(0, ("«", 0));
                        pages.Insert(1, ("‹", PreviousPage.Value));
                    }

                    if (pages.Any(x => x.Number == TotalPages.Value) == false)
                    {
                        pages.Add(("›", NextPage.Value));
                        pages.Add(("»", TotalPages.Value));
                    }

                    return pages.ToDictionary(x => x.Button, x => x.Number);
                }

                return new Dictionary<string, int>();
            }

            public IEnumerable<int> GetPages(int PaginationOffset)
            {
                var arr = new List<int>();
                if (PageNumber.HasValue && TotalPages.HasValue)
                {
                    int frange = 1;
                    int lrange = 1;
                    if (TotalPages > 1)
                    {
                        frange = new[] { PageNumber.Value - PaginationOffset, 0 }.Max();
                        lrange = new[] { PageNumber.Value + PaginationOffset, TotalPages.Value }.Min();
                    }

                    for (int index = frange, loopTo = lrange; index <= loopTo; index++)
                    {
                        arr.Add(index);
                    }
                }

                return arr;
            }
        }
    }

    public class InterpolatedQuery
    {
        public InterpolatedQuery(string parameterPrefix = null, CultureInfo culture = null)
        {
            this.ParameterPrefix = parameterPrefix;
            this.Culture = culture;
            defaults();
        }

        internal void processQuery(FormattableString sql, object aditionalParameters = null)
        {
            this.defaults();

            if (sql == null)
            {
                return;
            }

            var format = sql.Format;

            for (int i = 0; i < sql.ArgumentCount; i++)
            {
                var arg = sql.GetArgument(i);
                var index = "{" + i + ":";
                var f = format.IndexOf(index);

                if (f >= 0)
                {
                    if (arg is IFormattable farg && farg != null)
                    {
                        var argumentFormat = format.Substring(f + index.Length);
                        argumentFormat = argumentFormat.Substring(0, argumentFormat.IndexOf("}"));
                        arg = farg.ToString(argumentFormat, this.Culture);
                    }
                    else
                    {
                        arg = arg?.ToString();
                    }

                    this.Parameters.Add($"{this.ParameterPrefix}{i}", arg);
                }
                else
                {
                    if (arg is Type t)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.GetTableName(t));
                    }
                    else if (arg is TableAttribute table)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.Encapsulate(table.Name));
                    }
                    else if (arg is PropertyInfo member)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.GetColumnName(member));
                    }
                    else if (arg is ColumnAttribute c)
                    {
                        format = format.Replace("{" + i + "}", SimpleCRUD.Encapsulate(c.Name));
                    }
                    else if (arg is Condition cn)
                    {
                        format = format.Replace("{" + i + "}", cn.ParenthesisToString().Wrap(" "));
                    }
                    else if (arg.GetType().IsEnumerableNotString())
                    {
                        var list = arg as IEnumerable<object>;

                        var parameters = "";
                        if (list is IEnumerable<BinaryExpression> exps)
                        {
                            //TODO: precisa testar
                            var binExps = exps.Where(x => x is BinaryExpression);
                            parameters = string.Join(" AND ", exps.Select((x, j) =>
                            {
                                var exp = SimpleCRUD.ParseExpression(x);
                                var column = SimpleCRUD.GetColumnName(exp.Item1);
                                var paramName = $"{this.ParameterPrefix}{i}_{j}";
                                this.Parameters.Add(paramName, x);
                                return $"{column} {exp.Item2.Replace("!=", "<>")} @{paramName}";
                            }));
                        }
                        else
                        {
                            parameters = string.Join(", ", list.Select((x, j) =>
                            {
                                this.Parameters.Add($"{this.ParameterPrefix}{i}_{j}", x);
                                return $"{this.ParameterPrefix}{i}_{j}";
                            }));
                        }
                        format = format.Replace("{" + i + "}", parameters);
                    }
                    else
                    {
                        format = format.Replace("{" + i + "}", $"{this.ParameterPrefix}{i}");
                        this.Parameters.Add($"{this.ParameterPrefix}{i}", arg);
                    }
                }
            }

            if (aditionalParameters != null)
            {
                this.Parameters.AddDynamicParams(aditionalParameters);
            }

            this.Query += format;
        }

        internal void defaults()
        {
            this.Parameters = this.Parameters ?? new DynamicParameters();
            this.ParameterPrefix = this.ParameterPrefix ?? SimpleCRUD.DefaultParameterPrefix;
            this.Culture = this.Culture ?? CultureInfo.InvariantCulture;
            this.Query = this.Query ?? "";
        }

        public InterpolatedQuery(FormattableString sql, string parameterPrefix = null, object aditionalParameters = null, CultureInfo culture = null) : this(parameterPrefix, culture)
        {
            processQuery(sql, aditionalParameters);
        }

        public string Query { get; private set; }
        public DynamicParameters Parameters { get; private set; }

        public string ParameterPrefix { get; private set; }
        public CultureInfo Culture { get; private set; }

        public void Deconstruct(out string query, out DynamicParameters parameters)
        {
            query = Query;
            parameters = Parameters;
        }

        public InterpolatedQuery Append(InterpolatedQuery query)
        {
            defaults();
            this.Query += query.Query;
            Parameters.AddDynamicParams(query.Parameters);
            return this;
        }

        public InterpolatedQuery Append(FormattableString query, object aditionalParameters = null)
        {
            return Append(query.ToInterpolatedQuery(this.ParameterPrefix, aditionalParameters, this.Culture));
        }

        public InterpolatedQuery AppendIf(bool condition, FormattableString query)
        {
            if (condition) Append(query);
            return this;
        }

        public static implicit operator InterpolatedQuery(FormattableString query) => new InterpolatedQuery(query);

        public static implicit operator InterpolatedQuery((FormattableString query, DynamicParameters parameters) tuple) => new InterpolatedQuery(tuple.query, null, tuple.parameters, null);
    }

    internal class Join
    {
        #region Private Properties

        private string JoinString
        {
            get
            {
                switch (Type)
                {
                    case JoinType.Inner:
                        {
                            return "INNER JOIN";
                        }

                    case JoinType.LeftOuterJoin:
                        {
                            return "LEFT OUTER JOIN";
                        }

                    case JoinType.RightOuterJoin:
                        {
                            return "RIGHT OUTER JOIN";
                        }

                    case JoinType.FullOuterJoin:
                        {
                            return "FULL OUTER JOIN";
                        }

                    case JoinType.CrossJoin:
                        {
                            return "CROSS JOIN";
                        }

                    case JoinType.CrossApply:
                        {
                            return "CROSS APPLY";
                        }

                    default:
                        {
                            return "JOIN";
                        }
                }
            }
        }

        #endregion Private Properties

        #region Internal Properties

        internal Condition On { get; set; }
        internal string Table { get; set; }
        internal JoinType Type { get; set; }

        #endregion Internal Properties

        #region Public Methods

        public override string ToString() => On == null ? string.Format(CultureInfo.InvariantCulture, "{0} {1}", JoinString, Table) : string.Format(CultureInfo.InvariantCulture, "{0} {1} On {2}", JoinString, Table, On);

        #endregion Public Methods
    }


    /// <summary>
    /// A condition with optional AND and OR clauses that can be used in WHERE or JOIN ON statements.
    /// </summary>
    public class Condition
    {
        #region Internal Fields

        internal readonly List<string> _tokens = new List<string>();

        #endregion Internal Fields

        #region Public Constructors

        public Condition(string LogicOperator, params FormattableString[] Conditions)
        {
            foreach (var condition in Conditions ?? Array.Empty<FormattableString>())
            {
                if (condition != null && condition.ToString().IsValid())
                {
                    if (LogicOperator.ToLowerInvariant() == "or")
                    {
                        Or(condition);
                    }
                    else
                    {
                        And(condition);
                    }
                }
            }
        }

        public Condition(string LogicOperator, params Condition[] Conditions)
        {
            foreach (var condition in Conditions ?? Array.Empty<Condition>())
            {
                if (condition != null && condition.ToString().IsValid())
                {
                    if (LogicOperator.ToLowerInvariant() == "or")
                    {
                        Or(condition);
                    }
                    else
                    {
                        And(condition);
                    }
                }
            }
        }

        /// <summary>
        /// Select class constructor
        /// </summary>
        /// <param name="condition">Condition to set in this instance</param>
        public Condition(FormattableString condition)
        {
            if (condition.IsNotBlank())
            {
                _tokens.Add(condition.ToSQLString());
            }
        }

        /// <summary>
        /// Select class constructor
        /// </summary>
        /// <param name="condition">Copies to the condition being constructed</param>
        public Condition(Condition condition)
        {
            if (condition != null && condition.ToString().IsValid())
            {
                _tokens.Add(condition.ParenthesisToString());
            }
        }

        /// <summary>
        /// condition class constructor
        /// </summary>
        public Condition(string Column, object Value, string Operator = "=")
        {
            if (Column.IsValid())
            {
                if (Value == null)
                {
                    switch (Operator)
                    {
                        case "=":
                            _tokens.Add($"{Column} IS NULL");
                            return;

                        case "!=":
                        case "<>":
                            _tokens.Add($"{Column} IS NOT NULL");
                            return;

                        default:
                            Value = default;
                            break;
                    }
                }

                _tokens.Add($"{Column} {Operator.IfBlank("=")} {Util.ToSQLString(Value)}");
            }
        }

        public Condition((string, string, string) condition) : this(condition.Item1, condition.Item2, condition.Item3)
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public static Condition AndMany(params FormattableString[] conditions) => new Condition("And", conditions);

        public static Condition OrMany(params FormattableString[] conditions) => new Condition("Or", conditions);

        /// <summary>
        /// Appends the given condition with AND in this condition.
        /// </summary>
        /// <param name="condition">Condition to be appended</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Condition And(FormattableString condition)
        {
            if (!(condition == null) && condition.ToString().IsValid())
            {
                if (_tokens.Any())
                {
                    _tokens.Add("And");
                }

                _tokens.Add(condition.ToSQLString());
            }

            return this;
        }

        /// <summary>
        /// Appends the given condition with AND in this condition.
        /// </summary>
        /// <param name="condition">Condition to be appended</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Condition And(Condition condition) => And(condition.ParenthesisToString().ToFormattableString());

        public Condition AndAll(params FormattableString[] Conditions) => And(AndMany(Conditions));

        public Condition AndAny(params FormattableString[] Conditions) => And(OrAny(Conditions));

        /// <summary>
        /// Appends the given condition with OR in this condition.
        /// </summary>
        /// <param name="condition">Condition to be appended</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Condition Or(FormattableString condition)
        {
            if (!(condition == null) && condition.ToString().IsValid())
            {
                if (_tokens.Any())
                {
                    _tokens.Add("Or");
                }

                _tokens.Add(condition.ToSQLString());
            }

            return this;
        }

        /// <summary>
        /// Appends the given condition with OR in this condition.
        /// </summary>
        /// <param name="condition">Condition to be appended</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Condition Or(Condition condition) => Or(condition.ParenthesisToString().ToFormattableString());

        public Condition OrAll(params FormattableString[] Conditions) => Or(AndMany(Conditions));

        public Condition OrAny(params FormattableString[] Conditions) => Or(OrMany(Conditions));

        /// <summary>
        /// Returns the condition statement as a SQL query in parenthesis.
        /// </summary>
        /// <returns>The condition statement as a SQL query in parenthesis</returns>
        public string ParenthesisToString() => ToString().Quote('(');

        /// <summary>
        /// Returns the condition statement as a SQL query.
        /// </summary>
        /// <returns>The condition statement as a SQL query</returns>
        public override string ToString() => string.Join(" ", _tokens).QuoteIf(_tokens.Count > 2, '(');

        #endregion Public Methods
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FromSQLAttribute : Attribute
    {
        #region Private Fields

        private string sql;

        #endregion Private Fields

        #region Public Constructors

        public FromSQLAttribute() : base()
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Arquivo contendo as instruções sql para esta classe
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// QueryInterpolated SQL para esta classe
        /// </summary>
        public string SQL
        {
            get
            {
                if (sql.IsValid())
                {
                    return sql;
                }

                if (File.IsValid())
                {
                    if (File.IsFilePath())
                    {
                        if (System.IO.File.Exists(File))
                        {
                            return System.IO.File.ReadAllText(File).ValidateOr(x => x.IsValid(), new ArgumentException("No file content"));
                        }
                        else
                        {
                            throw new System.IO.FileNotFoundException("File not exists");
                        }
                    }
                    else
                    {
                        throw new ArgumentException("File is not a file path");
                    }
                }

                return $"SELECT * FROM {TableName.ValidateOr(x => x.IsValid(), new ArgumentException("No table name defined"))}";
            }

            set
            {
                if (value.IsValid() && value != sql)
                {
                    sql = value;
                }
            }
        }

        /// <summary>
        /// Nome da tabela, gera automaticamente uma query padrão para esta classe
        /// </summary>
        public string TableName { get; set; }

        #endregion Public Properties
    }

    public class Select : Select<Dictionary<string, object>>
    {
        #region Public Constructors

        public Select() : base()
        {
        }

        public Select(params string[] columns) : base(columns)
        {
        }

        public Select(Dictionary<string, object> Obj) : base(Obj)
        {
        }

        #endregion Public Constructors
    }

    /// <summary>
    /// Class that aids building a SELECT clause using <see cref="Dictionary{TKey, TValue}"/> or
    /// POCO Classes.
    /// </summary>
    public class Select<T> : ISelect where T : class
    {
        #region Internal Fields

        internal List<string> _columns;

        internal string _from;

        internal ISelect _fromsub;

        internal string _fromsubname;

        internal List<string> _groupBy;

        internal string _having;

        internal List<Join> _joins;
        internal string _offset;
        internal List<string> _orderBy;
        internal string _top;
        internal Condition _where;

        #endregion Internal Fields

        #region Public Constructors

        /// <summary>
        /// Class that aids building a SELECT clause.
        /// </summary>
        public Select()
        {
            try
            {
                SetColumns<T>();
                From<T>();
            }
            catch
            {
                SetColumns("*");
            }
        }

        /// <summary>
        /// Class that aids building a SELECT clause.
        /// </summary>
        public Select(T obj)
        {
            SetColumns<T>();
            From<T>();
            WhereObject(obj);
        }

        /// <summary>
        /// Class that aids building a SELECT clause.
        /// </summary>
        /// <param name="columns">Columns to be selected</param>
        public Select(params string[] columns)
        {
            SetColumns(columns);
            From<T>();
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// The final QueryInterpolated string generated by this instance
        /// </summary>
        public string Query => ToString();

        public char QuoteChar { get; set; } = '[';

        #endregion Public Properties

        #region Public Methods

        public static FormattableString CreateSearch(IEnumerable<string> Values, params string[] Columns)
        {
            Values = (Values ?? Array.Empty<string>()).WhereNotBlank().ToArray();
            Columns = (Columns ?? Array.Empty<string>()).WhereNotBlank().ToArray();
            return Columns.SelectMany(col => Values.Select(valor => $"{col} LIKE {valor.Wrap("%").ToSQLString(false)}"))
                .SelectJoinString(" OR ").ToFormattableString();
        }

        /// <summary>
        /// Operator overload that allows using the class wherever a string is expected.
        /// </summary>
        public static implicit operator FormattableString(Select<T> select) => (select?.ToString()).ToFormattableString();

        /// <summary>
        /// Operator overload that allows using the class wherever a string is expected.
        /// </summary>
        public static implicit operator string(Select<T> select) => select?.ToString();

        public Select<T> AddColumns<TO>(TO Obj = null) where TO : class
        {
            var eltipo = typeof(TO).GetNullableTypeOf();
            if (eltipo == typeof(Dictionary<string, object>))
            {
                if (Obj != null)
                {
                    AddColumns(((Dictionary<string, object>)(object)Obj).Keys.ToArray());
                }
            }
            else if (eltipo == typeof(NameValueCollection))
            {
                AddColumns(((NameValueCollection)(object)Obj).AllKeys.ToArray());
            }
            else
            {
                var props = eltipo.GetProperties().Select(x => x.GetAttributeValue<ColumnAttribute, string>(y => y.Name).IfBlank(x.Name));

                AddColumns(props.ToArray());
            }

            return this;
        }

        public Select<T> AddColumns(params string[] Columns)
        {
            Columns = (Columns ?? Array.Empty<string>()).SelectMany(x => x.Split(",")).Distinct().Where(x => x.IsValid()).ToArray();
            _columns = _columns ?? new List<string>();
            _columns.AddRange(Columns);
            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> And(IEnumerable<FormattableString> conditions) => And((conditions ?? Array.Empty<FormattableString>()).Select(x => new Condition(x)).ToArray());

        public Select<T> And(FormattableString condition) => And(new Condition(condition));

        public Select<T> And(string Column, object Value, string Operator = "=") => And(new Condition(Column, Value, Operator));

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> And(params Condition[] conditions)
        {
            foreach (var condition in conditions ?? Array.Empty<Condition>())
            {
                if (condition != null && condition.ToString().IsValid())
                {
                    if (_where is null)
                    {
                        Where(condition);
                    }
                    else
                    {
                        _where.And(condition);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> AndAll(params FormattableString[] conditions)
        {
            conditions = conditions ?? Array.Empty<FormattableString>();
            if (conditions.Any())
            {
                if (_where is null)
                {
                    _where = new Condition("AND", conditions);
                }
                else
                {
                    _where.AndAll(conditions);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> AndAny(params FormattableString[] conditions)
        {
            conditions = conditions ?? Array.Empty<FormattableString>();
            if (conditions.Any())
            {
                if (_where is null)
                {
                    _where = new Condition("OR", conditions);
                }
                else
                {
                    _where.AndAny(conditions);
                }
            }

            return this;
        }

        public Select<T> AndIn<TO>(string Column, params TO[] Items) => And(Util.ToFormattableString(Column + " in " + Items.ToSQLString()));

        public Select<T> AndObject<TO>(TO Obj) where TO : class => WhereObject(Obj, "AND");

        public Select<T> AndSearch(string Value, params string[] Columns) => AndSearch(new[] { Value }, Columns);

        public Select<T> AndSearch(IEnumerable<string> Values, params string[] Columns) => And(CreateSearch(Values, Columns));

        /// <summary>
        /// Sets the <see cref="QuoteChar"/> and apply it to all columns
        /// </summary>
        /// <param name="QuoteChar"></param>
        /// <returns></returns>
        public Select<T> ColumnQuote(char? QuoteChar = null, bool WithTableName = false)
        {
            var _nova = new List<string>();
            this.QuoteChar = QuoteChar ?? this.QuoteChar;

            foreach (var item in _columns ?? new List<string>())
            {
                var c = item;
                if (item != "*")
                {
                    c = item.UnQuote().Split(".", StringSplitOptions.RemoveEmptyEntries).SelectJoinString(x => SimpleCRUD.FormatSQLColumn(this.QuoteChar, WithTableName.AsIf(this.GetTableOrSubQuery()), x.UnQuote()));
                }
                _nova.Add(c);
            }

            SetColumns(_nova.ToArray());
            return this;
        }

        public DbCommand CreateDbCommand(DbConnection Connection, Dictionary<string, object> dic, DbTransaction Transaction = null) => Connection.CreateCommand(ToString(), dic, Transaction);

        public DbCommand CreateDbCommand(DbConnection Connection, DbTransaction Transaction = null) => CreateDbCommand(Connection, null, Transaction);

        /// <summary>
        /// Sets a CROSS APPLY clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> CrossApply(string table) => Join(JoinType.CrossApply, table, null);

        /// <summary>
        /// Sets a CROSS JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> CrossJoin(string table) => Join(JoinType.CrossJoin, table, null);

        /// <summary>
        /// Return the full name of <paramref name="ColumnName"/> using current table or alias
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="QuoteChar"></param>
        /// <returns></returns>
        public string FormatColumnName(string ColumnName) => SimpleCRUD.FormatSQLColumn(this.QuoteChar, GetTableOrSubQuery(), ColumnName);

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="TableOrSubQuery">Table to be selected from</param>
        /// <returns></returns>
        public Select<T> From(string TableOrSubQuery)
        {
            if (TableOrSubQuery.IsValid())
            {
                _from = TableOrSubQuery.QuoteIf(TableOrSubQuery.StartsWith("SELECT "), '(');
                _fromsub = null;
            }

            return this;
        }

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="SubQuery">Subquery to be selected from</param>
        /// <returns></returns>
        public Select<T> From<TO>(Select<TO> SubQuery, string SubQueryAlias = null) where TO : class
        {
            if (SubQuery != null && SubQuery.ToString(true).IsValid() && !ReferenceEquals(SubQuery, this))
            {
                _from = null;
                _fromsub = SubQuery;
                _fromsubname = SubQueryAlias.IfBlank(typeof(TO).Name + "_" + DateTime.Now.Ticks.ToString());
            }

            return this;
        }

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="SubQuery">Subquery to be selected from</param>
        /// <returns></returns>
        public Select<T> From<TO>(Action<Select<TO>> SubQuery, string SubQueryAlias = null) where TO : class
        {
            if (SubQuery != null)
            {
                var sl = new Select<TO>();
                SubQuery(sl);
                From(sl, SubQueryAlias);
            }

            return this;
        }

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="SubQuery">Subquery to be selected from</param>
        /// <returns></returns>
        public Select<T> From(Action<Select> SubQuery)
        {
            if (SubQuery != null)
            {
                From((Action<Select<Dictionary<string, object>>>)SubQuery);
            }

            return this;
        }

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <returns></returns>
        public Select<T> From<TO>()
        {
            From(typeof(TO).GetNullableTypeOf().Name);

            return this;
        }

        /// <summary>
        /// Sets a FULL OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> FullOuterJoin(string table, FormattableString on) => Join(JoinType.FullOuterJoin, table, new Condition(on));

        /// <summary>
        /// Sets a FULL OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> FullOuterJoin(string table, Condition on) => Join(JoinType.FullOuterJoin, table, on);

        public Select<T> FullOuterJoin(string table, string ThisColumn, string ForeignColumn) => FullOuterJoin(table, Util.ToFormattableString(SimpleCRUD.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + SimpleCRUD.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

        /// <summary>
        /// GetCliente the table name or subquery alias used in this select
        /// </summary>
        /// <returns></returns>
        public string GetTableOrSubQuery() => _fromsubname.IfBlank(_from);

        /// <summary>
        /// Sets the GROUP BY clause in the SELECT being built.
        /// </summary>
        /// <param name="columns">Columns to be grouped by</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> GroupBy(params string[] columns)
        {
            columns = columns ?? Array.Empty<string>();
            if (_groupBy is null)
            {
                _groupBy = new List<string>(columns);
            }
            else
            {
                _groupBy.AddRange(columns);
            }

            return this;
        }

        /// <summary>
        /// Sets or overwrite the HAVING clause in the SELECT being built.
        /// </summary>
        /// <param name="condition">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Having(string condition)
        {
            if (condition.IsValid())
            {
                _having = condition;
            }

            return this;
        }

        /// <summary>
        /// Sets a INNER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> InnerJoin(string table, FormattableString on) => InnerJoin(table, new Condition(on));

        public Select<T> InnerJoin(string table, string ThisColumn, string ForeignColumn) => InnerJoin(table, Util.ToFormattableString(SimpleCRUD.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + SimpleCRUD.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

        /// <summary>
        /// Sets a INNER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> InnerJoin(string table, Condition on) => Join(JoinType.Inner, table, on);

        /// <summary>
        /// Sets a JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Join(string table, FormattableString on) => Join(table, new Condition(on));

        /// <summary>
        /// Sets a JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Join(string table, Condition on) => Join(JoinType.Join, table, on);

        public Select<T> Join(JoinType JoinType, string Table, Condition on)
        {
            if (Table.IsValid() && (JoinType == JoinType.CrossApply || (!(on == null) && on.ToString().IsValid())))
            {
                _joins = _joins ?? new List<Join>();
                _joins.Add(new Join()
                {
                    Type = JoinType,
                    Table = Table,
                    On = on
                });
            }

            return this;
        }

        /// <summary>
        /// Sets a LEFT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> LeftOuterJoin(string table, FormattableString on) => LeftOuterJoin(table, new Condition(on));

        public Select<T> LeftOuterJoin(string table, string ThisColumn, string ForeignColumn) => LeftOuterJoin(table, Util.ToFormattableString(SimpleCRUD.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + SimpleCRUD.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

        /// <summary>
        /// Sets a LEFT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> LeftOuterJoin(string table, Condition on) => Join(JoinType.LeftOuterJoin, table, on);

        public Select<T> OffSet(int Page, int PageSize)
        {
            if (Page < 0 || PageSize < 0)
            {
                _offset = null;
            }
            else
            {
                PageSize = PageSize.SetMinValue(0);
                _offset = $"OFFSET {Page} ROWS FETCH NEXT {PageSize} ROWS ONLY";
            }

            return this;
        }

        public Select<T> Or(string Column, object Value, string Operator = "=") => Or(new Condition(Column, Value, Operator));

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an OR clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Or(IEnumerable<FormattableString> conditions) => Or((conditions ?? Array.Empty<FormattableString>()).Select(x => new Condition(x)).ToArray());

        public Select<T> Or(FormattableString condition) => Or(new Condition(condition));

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an OR clause.
        /// </summary>
        /// <param name="conditions">Condition of the WHERE clause</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Or(params Condition[] conditions)
        {
            foreach (var condition in conditions ?? Array.Empty<Condition>())
            {
                if (condition != null && condition.ToString().IsValid())
                {
                    if (_where is null)
                    {
                        Where(condition);
                    }
                    else
                    {
                        _where.Or(condition);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> OrAll(params FormattableString[] conditions)
        {
            conditions = conditions ?? Array.Empty<FormattableString>();
            if (conditions.Any())
            {
                if (_where is null)
                {
                    _where = new Condition("AND", conditions);
                }
                else
                {
                    _where.OrAll(conditions);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> OrAny(params FormattableString[] conditions)
        {
            conditions = conditions ?? Array.Empty<FormattableString>();
            if (conditions.Any())
            {
                if (_where is null)
                {
                    _where = new Condition("OR", conditions);
                }
                else
                {
                    _where.OrAny(conditions);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the ORDER BY clause in the SELECT being built.
        /// </summary>
        /// <param name="columns">Columns to be ordered by</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> OrderBy(params string[] columns)
        {
            columns = columns ?? Array.Empty<string>();
            if (_orderBy is null)
            {
                _orderBy = new List<string>(columns);
            }
            else
            {
                _orderBy.AddRange(columns);
            }

            return this;
        }

        public Select<T> OrIn<TO>(string Column, params TO[] Items) => Or(Util.ToFormattableString(Column + " in " + Items.ToSQLString()));

        public Select<T> OrObject<TO>(TO Obj) where TO : class => WhereObject(Obj, "OR");

        public Select<T> OrSearch(string Value, params string[] Columns) => OrSearch(new[] { Value }, Columns);

        public Select<T> OrSearch(IEnumerable<string> Values, params string[] Columns) => Or(CreateSearch(Values, Columns));

        public Select<T> RemoveColumns(params string[] Columns)
        {
            if (_columns != null)
            {
                _columns = _columns.Where(x => x.IsNotIn(Columns ?? Array.Empty<string>())).ToList();
            }

            return this;
        }

        public Select<T> RightOuterJoin(string table, string ThisColumn, string ForeignColumn) => RightOuterJoin(table, Util.ToFormattableString(SimpleCRUD.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + SimpleCRUD.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

        /// <summary>
        /// Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> RightOuterJoin(string table, FormattableString on) => RightOuterJoin(table, new Condition(on));

        /// <summary>
        /// Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> RightOuterJoin(string table, Condition on) => Join(JoinType.RightOuterJoin, table, on);

        public Select<T> SetColumns(params string[] Columns)
        {
            _columns = null;
            AddColumns(Columns);
            return this;
        }

        public Select<T> SetColumns<TO>(TO Obj = null) where TO : class
        {
            _columns = null;
            AddColumns(Obj);
            return this;
        }

        /// <summary>
        /// Adds a TOP clausule to this SELECT
        /// </summary>
        /// <param name="Top"></param>
        /// <param name="Percent"></param>
        /// <returns></returns>
        public Select<T> Top(int Top, bool Percent = false)
        {
            if (Top > 0)
            {
                _top = $"TOP({Top}) {Percent.AsIf("PERCENT")}";
            }
            else
            {
                _top = null;
            }
            return this;
        }

        /// <summary>
        /// Returns the SELECT statement as a SQL query.
        /// </summary>
        /// <returns>The SELECT statement as a SQL query</returns>
        public override string ToString() => ToString(false);

        /// <summary>
        /// Returns the SELECT statement as a SQL query.
        /// </summary>
        /// <param name="AsSubquery">when TRUE, prevent uncompatibble statements for subqueries</param>
        /// <returns>The SELECT statement as a SQL query</returns>
        public string ToString(bool AsSubquery)
        {
            var sql = new StringBuilder("SELECT ");

            if (_top?.IsValid() ?? false)
            {
                sql.Append($"{_top}");
            }
            var cols = (_columns?.Distinct().SelectJoinString(",") ?? Util.EmptyString).IfBlank(" * ");
            sql.Append(cols);
            if (_fromsub != null && _fromsub.ToString().IsValid())
            {
                _from = _fromsub.ToString(true).Quote('(') + " as " + _fromsubname;
            }

            if (_from?.IsValid() ?? false)
            {
                sql.Append(" FROM ");
                sql.Append(_from);
            }

            if (_joins != null && _joins.Any())
            {
                sql.Append(_joins.SelectJoinString(j => string.Format(CultureInfo.InvariantCulture, " {0}", j), " "));
            }

            if (_where != null)
            {
                sql.Append(" WHERE ");
                sql.Append(_where);
            }

            if (_groupBy != null)
            {
                sql.Append(" GROUP BY ");
                sql.Append(string.Join(", ", _groupBy));
            }

            if (!Equals(_having, null))
            {
                sql.Append(" HAVING ");
                sql.Append(_having);
            }

            if (_orderBy != null && AsSubquery == false)
            {
                sql.Append(" ORDER BY ");
                sql.Append(string.Join(", ", _orderBy));
            }

            if (_offset?.IsValid() ?? false && AsSubquery == false)
            {
                sql.Append($" {_offset} ");
            }

            return sql.ToString();
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// </summary>
        /// <param name="condition">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(FormattableString condition)
        {
            if (condition.IsNotBlank())
            {
                if (_where != null)
                {
                    And(new Condition(condition));
                }
                else
                {
                    _where = new Condition(condition);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(string LogicOperator, IEnumerable<FormattableString> conditions)
        {
            LogicOperator = LogicOperator.IfBlank("and");
            foreach (var condition in conditions ?? Array.Empty<FormattableString>())
            {
                if (condition != null && condition.ToString().IsValid())
                {
                    if (LogicOperator.ToLowerInvariant().IsAny("||", "|", "or"))
                    {
                        Or(condition);
                    }
                    else
                    {
                        And(condition);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(string LogicOperator, IEnumerable<Condition> conditions)
        {
            LogicOperator = LogicOperator.IfBlank("and");
            foreach (var condition in conditions ?? Array.Empty<Condition>())
            {
                if (condition != null)
                {
                    if (LogicOperator.ToLowerInvariant().IsAny("||", "|", "or"))
                    {
                        Or(condition);
                    }
                    else
                    {
                        And(condition);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built using a lambda expression. This method
        /// is experimental
        /// </summary>
        /// <param name="predicate">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                var p = predicate.Body.ToString();
                var pName = predicate.Parameters.First();
                //predicate.Body.NodeType

                var rp = new Dictionary<string, string>()
                {
                    { $"{pName.Name}.", Util.EmptyString},
                    {"==", "="},
                    {"!=", "<>"},
                    {"AndAlso", " AND "},
                    {" && ", " AND "},
                    {" & ", " AND "},
                    {" || ", " OR "},
                    {" | ", " OR "},
                    {"OrElse", " OR "} ,
                    {" like ", " LIKE "} ,
                    {".Contains", " LIKE "},
                    {".Like", " LIKE "},
                    {".Equal", " = "},
                    {".IsIn", " in "},
                    {Util.DoubleQuoteChar, Util.SingleQuoteChar}
                };
                Where(p.ReplaceFrom(rp).ToFormattableString());
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// </summary>
        public Select<T> Where(string Column, object Value, string Operator = "=") => Where(new Condition(Column, Value, Operator));

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// </summary>
        /// <param name="condition">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(Condition condition)
        {
            if (condition != null && condition.ToString().IsValid())
            {
                if (_where != null)
                {
                    And(condition);
                }
                else
                {
                    _where = new Condition(condition);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built. If WHERE is already set, appends the
        /// condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Conditions to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(params Condition[] conditions) => And(conditions);

        public Select<T> Where(params (string, string, string)[] items)
        {
            items = items ?? Array.Empty<(string, string, string)>();
            return Where(items.Select(x => new Condition(x)).ToArray());
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built using a <see cref="Dictionary{string,
        /// object}"/> as column/value
        /// </summary>
        /// <param name="Dic"></param>
        /// <param name="FilterKeys"></param>
        /// <returns></returns>
        public object Where(Dictionary<string, object> Dic, Util.LogicConcatenationOperator LogicConcatenation, params string[] FilterKeys)
        {
            FilterKeys = FilterKeys ?? Array.Empty<string>();
            if (FilterKeys.Any())
            {
                FilterKeys = Dic.Keys.ToArray().Where(x => x.IsLikeAny(FilterKeys)).ToArray();
            }
            else
            {
                FilterKeys = Dic.Keys.ToArray();
            }

            FilterKeys = FilterKeys.Where(x => Dic[x] != null && Dic[x].ToString().IsValid()).ToArray();
            if (FilterKeys.Any())
            {
                foreach (var f in FilterKeys)
                {
                    if (LogicConcatenation == Util.LogicConcatenationOperator.OR)
                    {
                        Or(f, Dic[f]);
                    }
                    else
                    {
                        And(f, Dic[f]);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built using a <see
        /// cref="NameValueCollection"/> as column/operator/value
        /// </summary>
        /// <param name="NVC"></param>
        /// <param name="FilterKeys"></param>
        /// <returns></returns>
        public Select<T> Where(NameValueCollection NVC, params string[] FilterKeys)
        {
            FilterKeys = FilterKeys ?? Array.Empty<string>();
            NVC = NVC ?? new NameValueCollection();
            foreach (var k in NVC.AllKeys)
            {
                if (k.IsValid())
                {
                    string col = k.UrlDecode();
                    if (!FilterKeys.Any() || col.IsLikeAny(FilterKeys))
                    {
                        var values = NVC.GetValues(k) ?? Array.Empty<string>();
                        foreach (var v in values)
                        {
                            string logic = col.GetBefore(":", true).IfBlank("AND");
                            string op = v.GetBefore(":", true).IfBlank("=");
                            col = col.GetAfter(":");
                            col = col.Contains(" ").AsIf(col.UnQuote(QuoteChar, true).Quote(QuoteChar), col);
                            string valor = v.GetAfter(":").NullIf("null", StringComparison.InvariantCultureIgnoreCase);

                            if (valor == "'null'")
                            {
                                valor = null;
                            }

                            var cond = new Condition(col, valor, op);
                            Where(logic, new[] { cond });
                        }
                    }
                }
            }

            return this;
        }

        public Select<T> WhereIn<TO>(string Column, params TO[] Items) => AndIn(Column, Items);

        public Select<T> WhereObject<TO>(TO Obj) where TO : class => AndObject(Obj);

        public Select<T> WhereObject<TO>(TO Obj, Util.LogicConcatenationOperator LogicOperator) where TO : class => WhereObject(Obj, LogicOperator.GetEnumValueAsString());

        public Select<T> WhereObject<TO>(TO Obj, string LogicOperator) where TO : class
        {
            if (Obj != null)
            {
                foreach (var item in Obj.GetNullableTypeOf().GetProperties().Where(x => x.CanRead))
                {
                    if (LogicOperator.ToLowerInvariant().IsIn("or", "||", "|"))
                    {
                        Or(item.Name, item.GetValue(Obj));
                    }
                    else
                    {
                        And(item.Name, item.GetValue(Obj));
                    }
                }
            }

            return this;
        }

        #endregion Public Methods
    }

    public enum JoinType
    {
        Join,
        Inner,
        LeftOuterJoin,
        RightOuterJoin,
        FullOuterJoin,
        CrossJoin,
        CrossApply
    }

    public interface ISelect
    {
        #region Public Methods

        string ToString();

        string ToString(bool SubQuery);

        #endregion Public Methods
    }
}