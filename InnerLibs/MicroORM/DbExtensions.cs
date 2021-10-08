using InnerLibs.LINQ;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace InnerLibs.MicroORM
{
    public static class DbExtensions
    {
        private static Dictionary<Type, DbType> typeMap = null;

        /// <summary>
        /// Dicionario com os <see cref="Type"/> e seu <see cref="DbType"/> correspondente
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Type, DbType> DbTypes
        {
            get
            {
                if (typeMap is null)
                {
                    typeMap = new Dictionary<Type, DbType>();
                    typeMap[typeof(byte)] = DbType.Byte;
                    typeMap[typeof(sbyte)] = DbType.SByte;
                    typeMap[typeof(short)] = DbType.Int16;
                    typeMap[typeof(ushort)] = DbType.UInt16;
                    typeMap[typeof(int)] = DbType.Int32;
                    typeMap[typeof(uint)] = DbType.UInt32;
                    typeMap[typeof(long)] = DbType.Int64;
                    typeMap[typeof(ulong)] = DbType.UInt64;
                    typeMap[typeof(float)] = DbType.Single;
                    typeMap[typeof(double)] = DbType.Double;
                    typeMap[typeof(decimal)] = DbType.Decimal;
                    typeMap[typeof(bool)] = DbType.Boolean;
                    typeMap[typeof(string)] = DbType.String;
                    typeMap[typeof(char)] = DbType.StringFixedLength;
                    typeMap[typeof(Guid)] = DbType.Guid;
                    typeMap[typeof(DateTime)] = DbType.DateTime;
                    typeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
                    typeMap[typeof(byte[])] = DbType.Binary;
                }

                return typeMap;
            }
        }

        /// <summary>
        /// Retorna um <see cref="DbType"/> de um <see cref="Type"/>
        /// </summary>
        public static DbType GetDbType<T>(this T obj, DbType Def = DbType.Object)
        {
            return DbTypes.GetValueOr(ClassTools.GetNullableTypeOf(obj), Def);
        }

        /// <summary>
        /// Retorna um <see cref="Type"/> de um <see cref="DbType"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <param name="Def"></param>
        /// <returns></returns>
        public static Type GetTypeFromDb<T>(this DbType Type, Type Def = null)
        {
            var tt = DbTypes.FirstOrDefault(x => x.Value == Type);
            if (!Information.IsNothing(tt))
            {
                return tt.Key;
            }

            return Def ?? typeof(object);
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see cref="NameValueCollection"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, NameValueCollection Parameters)
        {
            return Connection.CreateCommand(SQL, Parameters.ToDictionary());
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see cref="Dictionary(Of String, Object)"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, Dictionary<string, object> Parameters)
        {
            if (Connection != null)
            {
                var command = Connection.CreateCommand();
                command.CommandText = SQL;
                if (Parameters != null && Parameters.Any())
                {
                    foreach (var p in Parameters.Keys)
                    {
                        var v = Parameters.GetValueOr(p);
                        var arr = Converter.ForceArray(v, typeof(object));
                        for (int index = 0, loopTo = arr.Length - 1; index <= loopTo; index++)
                        {
                            var param = command.CreateParameter();
                            if (arr.Count() == 1)
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

                return command;
            }

            return null;
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string ou arquivo SQL, tratando os parametros {p} desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="FilePathOrSQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string FilePathOrSQL, params string[] Args)
        {
            if (FilePathOrSQL != null)
            {
                if (FilePathOrSQL.IsFilePath())
                {
                    if (File.Exists(FilePathOrSQL.ToString()))
                    {
                        return Connection.CreateCommand(File.ReadAllText(FilePathOrSQL).ToFormattableString(Args));
                    }
                    else
                    {
                        return null;
                    }
                }

                return Connection.CreateCommand(FilePathOrSQL.ToFormattableString(Args));
            }

            return null;
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string interpolada, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FormattableString SQL)
        {
            if (SQL != null && Connection != null)
            {
                var cmd = Connection.CreateCommand();
                if (SQL.ArgumentCount > 0)
                {
                    cmd.CommandText = SQL.Format;
                    for (int index = 0, loopTo = SQL.ArgumentCount - 1; index <= loopTo; index++)
                    {
                        var valores = SQL.GetArgument(index);
                        var v = Converter.ForceArray(valores, typeof(object));
                        var param_names = new List<string>();
                        for (int v_index = 0, loopTo1 = v.Count() - 1; v_index <= loopTo1; v_index++)
                        {
                            var param = cmd.CreateParameter();
                            if (v.Count() == 1)
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

                        cmd.CommandText = cmd.CommandText.Replace("{" + index + "}", param_names.Join(",").IfBlank("NULL").QuoteIf(param_names.Count > 1, "("));
                    }
                }
                else
                {
                    cmd.CommandText = SQL.ToString();
                }

                return cmd;
            }

            return null;
        }

        /// <summary>
        /// Converte um objeto para uma string SQL, utilizando o objeto como parametro
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static string ToSQLString(object Obj)
        {
            return ToSQLString($"{Obj}");
        }

        /// <summary>
        /// Converte uma <see cref="FormattableString"/> para uma string SQL, tratando seus parametros como parametros da query
        /// </summary>
        public static string ToSQLString(this FormattableString SQL)
        {
            if (SQL != null)
            {
                if (SQL.ArgumentCount > 0)
                {
                    string CommandText = SQL.Format;
                    for (int index = 0, loopTo = SQL.ArgumentCount - 1; index <= loopTo; index++)
                    {
                        var valores = SQL.GetArgument(index);
                        var v = Converter.ForceArray(valores, typeof(object));
                        var paramvalues = new List<object>();
                        for (int v_index = 0, loopTo1 = v.Count() - 1; v_index <= loopTo1; v_index++)
                            paramvalues.Add(v[v_index]);
                        var pv = paramvalues.Select(x =>
                        {
                            if (x is null)
                            {
                                return "NULL";
                            }

                            if (ClassTools.GetNullableTypeOf(x).IsNumericType() || x.ToString().IsNumber())
                            {
                                return x.ToString();
                            }

                            if (Verify.IsDate(x))
                            {
                                return Conversions.ToDate(x).ToSQLDateString().Quote('\'');
                            }

                            if (Verify.IsBoolean(x))
                            {
                                return Conversions.ToBoolean(x).AsIf(1, 0).ToString();
                            }

                            return x.ToString().Quote('\'');
                        }).ToList();
                        CommandText = CommandText.Replace("{" + index + "}", pv.Join(",").IfBlank("NULL").UnQuote("(", true).QuoteIf(pv.Count > 1, "("));
                    }

                    return CommandText;
                }
                else
                {
                    return SQL.ToString();
                }
            }

            return null;
        }

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata valores especificos de
        /// um NameValueCollection como parametros da procedure
        /// </summary>
        /// <param name="NVC">Objeto</param>
        /// <param name="ProcedureName">  Nome da Procedure</param>
        /// <param name="Keys">Valores do nameValueCollection o que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>

        public static DbCommand ToProcedure(this DbConnection Connection, string ProcedureName, NameValueCollection NVC, params string[] Keys)
        {
            return Connection.ToProcedure(ProcedureName, NVC.ToDictionary(Keys), Keys);
        }

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata propriedades espicificas de
        /// um objeto como parametros da procedure
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <param name="ProcedureName">  Nome da Procedure</param>
        /// <param name="Keys">propriedades do objeto que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static DbCommand ToProcedure<T>(this DbConnection Connection, string ProcedureName, T Obj, params string[] Keys)
        {
            return Connection.ToProcedure(ProcedureName, Obj.CreateDictionary(), Keys);
        }

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e utiliza os pares de um dicionario como parametros da procedure
        /// </summary>
        /// <param name="Dic">Dicionario com os parametros</param>
        /// <param name="ProcedureName">  Nome da Procedure</param>
        /// <param name="Keys">CHaves de Dicionário que devem ser utilizadas</param>
        /// <returns>Um DbCommand parametrizado</returns>

        public static DbCommand ToProcedure(this DbConnection Connection, string ProcedureName, Dictionary<string, object> Dic, params string[] Keys)
        {
            Keys = Keys ?? Array.Empty<string>();
            if (!Keys.Any())
            {
                Keys = Dic.Keys.ToArray();
            }
            else
            {
                Keys = Dic.Keys.ToArray().Where(x => x.IsLikeAny(Keys)).ToArray();
            }

            string sql = "EXEC " + ProcedureName + " " + Keys.SelectJoin(key => " @" + key + " = " + "@__" + key, ", ");
            return Connection.CreateCommand(sql, Dic.ToDictionary(x => x.Key, x => x.Value));
        }

        /// <summary>
        /// Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="NameValueCollection" />
        /// </summary>
        /// <remarks>
        /// NameValueCollection pode usar a seguinte estrutura: &name=value1&or:surname=like:%value2% => WHERE [name] = 'value1' OR [surname] like '%value2%'
        /// </remarks>
        /// <param name="NVC">        Dicionario</param>
        /// <param name="TableName">  Nome da Tabela</param>
        /// <returns>Uma string com o comando montado</returns>
        public static Select ToSQLFilter(this NameValueCollection NVC, string TableName, string CommaSeparatedColumns, params string[] FilterKeys)
        {
            return (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(NVC, FilterKeys);
        }

        /// <summary>
        /// Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        /// <param name="Dic">        Dicionario</param>
        /// <param name="TableName">  Nome da Tabela</param>
        /// <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
        /// <returns>Uma string com o comando montado</returns>

        public static Select ToSQLFilter(this Dictionary<string, object> Dic, string TableName, string CommaSeparatedColumns, LogicConcatenationOperator LogicConcatenation, params string[] FilterKeys)
        {
            return (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(Dic, LogicConcatenation, FilterKeys);
        }

        public enum LogicConcatenationOperator
        {
            AND,
            OR
        }

        /// <summary>
        /// Cria comandos de INSERT para cada objeto do tipo <typeparamref name="T"/> em uma lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="obj"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static IEnumerable<DbCommand> CreateINSERTCommand<T>(this DbConnection Connection, IEnumerable<T> obj, string TableName = null) where T : class
        {
            return (obj ?? Array.Empty<T>()).Select(x => Connection.CreateINSERTCommand(x, TableName));
        }

        /// <summary>
        /// Cria um comando de INSERT para o objeto do tipo <typeparamref name="T"/>
        /// </summary>
        public static DbCommand CreateINSERTCommand<T>(this DbConnection Connection, T obj, string TableName = null) where T : class
        {
            var d = typeof(T);
            var dic = new Dictionary<string, object>();
            if (obj != null && Connection != null)
            {
                if (obj.IsDictionary())
                {
                    dic = (Dictionary<string, object>)(object)obj;
                }
                else if (ReferenceEquals(obj.GetType(), typeof(NameValueCollection)))
                {
                    dic = ((NameValueCollection)(object)obj).ToDictionary();
                }
                else
                {
                    dic = obj.CreateDictionary();
                }

                var cmd = Connection.CreateCommand();
                cmd.CommandText = string.Format($"INSERT INTO " + TableName.BlankCoalesce(d.Name, "#TableName") + " ({0}) values ({1})", dic.Keys.Join(","), dic.Keys.SelectJoin(x => $"@__{x}", ","));
                foreach (var k in dic.Keys)
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"__{k}";
                    param.Value = dic.GetValueOr(k, DBNull.Value);
                    cmd.Parameters.Add(param);
                }

                return cmd;
            }

            return null;
        }

        /// <summary>
        /// Cria um comando de INSERT para o objeto do tipo <typeparamref name="T"/>
        /// </summary>
        public static DbCommand CreateUPDATECommand<T>(this DbConnection Connection, T obj, string WhereClausule, string TableName = null) where T : class
        {
            var d = typeof(T);
            var dic = new Dictionary<string, object>();
            if (obj == null && Connection == null)
            {
                if (obj.IsDictionary())
                {
                    dic = (Dictionary<string, object>)(object)obj;
                }
                else if (ReferenceEquals(obj.GetType(), typeof(NameValueCollection)))
                {
                    dic = ((NameValueCollection)(object)obj).ToDictionary();
                }
                else
                {
                    dic = obj.CreateDictionary();
                }

                var cmd = Connection.CreateCommand();
                cmd.CommandText = string.Format($"UPDATE " + TableName.BlankCoalesce(d.Name, "#TableName") + " set" + Environment.NewLine);
                foreach (var k in dic.Keys)
                {
                    cmd.CommandText += $"set {k} = @__{k}, {Environment.NewLine}";
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"__{k}";
                    param.Value = dic.GetValueOr(k, DBNull.Value);
                    cmd.Parameters.Add(param);
                }

                cmd.CommandText = cmd.CommandText.TrimAny(Environment.NewLine, ",", " ");
                if (WhereClausule.IfBlank("").Trim().StartsWith("where"))
                    WhereClausule = WhereClausule.IfBlank("").RemoveFirstEqual("WHERE").Trim();
                if (WhereClausule.IsNotBlank())
                {
                    cmd.CommandText += $"{Environment.NewLine} WHERE {WhereClausule}";
                }

                return cmd;
            }

            return null;
        }

        /// <summary>
        /// Executa um comando SQL e retorna o numero de linhas afetadas
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static int RunSQLNone(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLNone(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Executa um comando SQL e retorna o numero de linhas afetadas
        /// </summary>
        public static int RunSQLNone(this DbConnection Connection, DbCommand Command)
        {
            if (Connection != null && Command != null)
            {
                if (!(Connection.State == ConnectionState.Open))
                {
                    Connection.Open();
                }

                return Command.LogCommand().ExecuteNonQuery();
            }

            return -1;
        }

        /// <summary>
        /// Retorna o primeiro resultado da primeira coluna de uma consulta SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static object RunSQLValue(this DbConnection Connection, DbCommand Command)
        {
            if (Connection != null && Command != null)
            {
                if (!(Connection.State == ConnectionState.Open))
                {
                    Connection.Open();
                }

                return Command.LogCommand().ExecuteScalar();
            }

            return null;
        }

        /// <summary>
        /// Retorna o primeiro resultado da primeira coluna de uma consulta SQL
        /// </summary>
        public static object RunSQLValue(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLValue(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Retorna o primeiro resultado da primeira coluna de uma consulta SQL como um tipo <typeparamref name="V"/>
        /// </summary>
        public static V? RunSQLValue<V>(this DbConnection Connection, DbCommand Command) where V : struct
        {
            var vv = Connection.RunSQLValue(Command);
            if (vv != null && Operators.ConditionalCompareObjectNotEqual(vv, DBNull.Value, false).ToBoolean())
            {
                return (V)vv;
            }

            return default;
        }

        /// <summary>
        /// Retorna o primeiro resultado da primeira coluna de uma consulta SQL como um tipo <typeparamref name="V"/>
        /// </summary>
        public static V? RunSQLValue<V>(this DbConnection Connection, FormattableString SQL) where V : struct
        {
            return Connection.RunSQLValue<V>(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Quando Configurado, escreve os parametros e queries executadas no TextWriterEspecifico
        /// </summary>
        /// <returns></returns>
        public static TextWriter LogWriter { get; set; } = new DebugTextWriter();

        public static DbCommand LogCommand(this DbCommand Command)
        {
            if (LogWriter != null)
            {
                var oldout = System.Console.Out;
                System.Console.SetOut(LogWriter);
                LogWriter.WriteLine(new string('=', 10));
                if (Command != null)
                {
                    foreach (DbParameter item in Command.Parameters)
                    {
                        string bx = $"Parameter: @{item.ParameterName}{Environment.NewLine}Value: {item.Value}{Environment.NewLine}Type: {item.DbType}{Environment.NewLine}Precision/Scale: {item.Precision}/{item.Scale}";
                        System.Console.WriteLine(bx);
                        System.Console.WriteLine(new string('-', 10));
                    }

                    System.Console.WriteLine(Command.CommandText, "SQL Command");
                }
                else
                {
                    System.Console.WriteLine("Command is NULL");
                }

                System.Console.WriteLine(new string('=', 10));
                System.Console.SetOut(oldout);
            }

            return Command;
        }

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T?> RunSQLArray<T>(this DbConnection Connection, DbCommand Command) where T : struct
        {
            return Connection.RunSQLSet(Command).Select(x => x.Values.FirstOrDefault()?.ChangeType<T>());
        }

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T?> RunSQLArray<T>(this DbConnection Connection, FormattableString SQL) where T : struct
        {
            return Connection.RunSQLArray<T>(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array
        /// </summary>
        public static IEnumerable<object> RunSQLArray(this DbConnection Connection, DbCommand Command)
        {
            return Connection.RunSQLSet(Command).Select(x => x.Values.FirstOrDefault());
        }

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array
        /// </summary>
        public static IEnumerable<object> RunSQLArray(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLArray(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of Object, Object)"/>
        /// </summary>
        public static Dictionary<object, object> RunSQLPairs(this DbConnection Connection, DbCommand SQL)
        {
            return Connection.RunSQLSet(SQL).ToDictionary(x => x.Values.FirstOrDefault(), x => x.Values.LastOrDefault());
        }

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of Object, Object)"/>
        /// </summary>
        public static Dictionary<object, object> RunSQLPairs(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLPairs(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of K, V)"/>
        /// </summary>
        public static Dictionary<K, V> RunSQLPairs<K, V>(this DbConnection Connection, DbCommand SQL)
        {
            return Connection.RunSQLPairs(SQL).ToDictionary(x => x.Key.ChangeType<K>(), x => x.Value.ChangeType<V>());
        }

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em um <see cref="Dictionary(Of K, V)"/>
        /// </summary>
        public static Dictionary<K, V> RunSQLPairs<K, V>(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLPairs<K, V>(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de  <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, Select<T> Select, bool WithSubQueries = false) where T : class
        {
            return Connection.RunSQLSet<T>(Select.CreateDbCommand(Connection), WithSubQueries);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados da primeira linha como um <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, Select<T> Select, bool WithSubQueries = false) where T : class
        {
            return Connection.RunSQLRow<T>(Select.CreateDbCommand(Connection), WithSubQueries);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de  <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> RunSQLSet(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLSet<Dictionary<string, object>>(SQL);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para um  <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        public static Dictionary<string, object> RunSQLRow(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLRow<Dictionary<string, object>>(SQL);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de  <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        public static IEnumerable<Dictionary<string, object>> RunSQLSet(this DbConnection Connection, DbCommand SQL)
        {
            return Connection.RunSQLSet<Dictionary<string, object>>(SQL);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para um  <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        public static Dictionary<string, object> RunSQLRow(this DbConnection Connection, DbCommand SQL)
        {
            return Connection.RunSQLRow<Dictionary<string, object>>(SQL);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, DbCommand SQL, bool WithSubQueries = false)
        {
            var x = Connection.RunSQLSet<T>(SQL, false).FirstOrDefault();
            if (x != null && WithSubQueries)
            {
                Connection.ProccessSubQuery(x, WithSubQueries);
            }

            return default;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, FormattableString SQL, bool WithSubQueries = false)
        {
            return Connection.RunSQLRow<T>(Connection.CreateCommand(SQL), WithSubQueries);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, FormattableString SQL, bool WithSubQueries = false)
        {
            return Connection.RunSQLSet<T>(Connection.CreateCommand(SQL), WithSubQueries);
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, DbCommand SQL, bool WithSubQueries = false)
        {
            return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(x =>
            {
                T v = (T)x.CreateOrSetObject(null, typeof(T));
                if (WithSubQueries)
                {
                    Connection.ProccessSubQuery(v, WithSubQueries);
                }

                return v;
            }).AsEnumerable();
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em listas de <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> RunSQLMany(this DbConnection Connection, FormattableString SQL)
        {
            return Connection.RunSQLMany(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Executa uma query SQL e retorna todos os seus resultsets mapeados em uma <see cref="IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))"/>
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
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> RunSQLMany<T1, T2, T3, T4, T5>(this DbConnection Connection, FormattableString SQL)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            return Connection.RunSQLMany<T1, T2, T3, T4, T5>(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
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
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> RunSQLMany<T1, T2, T3, T4>(this DbConnection Connection, FormattableString SQL)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            return Connection.RunSQLMany<T1, T2, T3, T4>(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
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
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> RunSQLMany<T1, T2, T3>(this DbConnection Connection, FormattableString SQL)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return Connection.RunSQLMany<T1, T2, T3>(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
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
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> RunSQLMany<T1, T2>(this DbConnection Connection, FormattableString SQL)
            where T1 : class
            where T2 : class
        {
            return Connection.RunSQLMany<T1, T2>(Connection.CreateCommand(SQL));
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de tipos especificos
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

        public static DbDataReader RunSQLReader(this DbConnection Connection, FormattableString SQL)
        {
            if (Connection != null)
            {
                return Connection.RunSQLReader(Connection.CreateCommand(SQL));
            }

            return null;
        }

        public static DbDataReader RunSQLReader(this DbConnection Connection, DbCommand Command)
        {
            if (Connection != null && Command != null)
            {
                if (!(Connection.State == ConnectionState.Open))
                {
                    Connection.Open();
                }

                try
                {
                    return Command.LogCommand().ExecuteReader();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            return null;
        }

        /// <summary>
        /// Mapeia os objetos de um datareader para uma classe, Dictionary ou NameValueCollection
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
                        ((NameValueCollection)(object)d).Add(name, Conversions.ToString(value));
                    }
                    else
                    {
                        var propnames = name.PropertyNamesFor();
                        var PropInfos = ClassTools.GetTypeOf(d).GetProperties();
                        var FieldInfos = ClassTools.GetTypeOf(d).GetFields();
                        foreach (var info in PropInfos)
                        {
                            var attrs = info.GetCustomAttributes<ColumnName>().SelectMany(x => x.Names).Union(propnames);
                            if (info.CanWrite && name.IsIn(attrs, StringComparer.InvariantCultureIgnoreCase))
                            {
                                if (ReferenceEquals(value.GetType(), typeof(DBNull)))
                                {
                                    info.SetValue(d, null);
                                }
                                else
                                {
                                    info.SetValue(d, Converter.ChangeType(value, info.PropertyType));
                                }
                            }
                        }

                        foreach (var info in FieldInfos)
                        {
                            var attrs = info.GetCustomAttributes<ColumnName>().SelectMany(x => x.Names).Union(propnames);
                            if (!PropInfos.Select(x => x.Name).ContainsAny(attrs, StringComparer.InvariantCultureIgnoreCase))
                            {
                                if (name.IsIn(attrs, StringComparer.InvariantCultureIgnoreCase))
                                {
                                    if (ReferenceEquals(value.GetType(), typeof(DBNull)))
                                    {
                                        info.SetValue(d, null);
                                    }
                                    else
                                    {
                                        info.SetValue(d, Converter.ChangeType(value, info.FieldType));
                                    }
                                }
                            }
                        }
                    }
                }

                l.Add(d);
            }

            return l.AsEnumerable();
        }

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
                    var attr = prop.GetCustomAttributes<FromSQL>(true).FirstOrDefault();
                    string Sql = attr.SQL.Inject(d);
                    bool gen = prop.PropertyType.IsGenericType;
                    bool lista = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
                    bool enume = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>));
                    bool cole = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>));
                    if (lista || enume || cole)
                    {
                        IList baselist = (IList)Activator.CreateInstance(prop.PropertyType);
                        var eltipo = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                        Connection.RunSQLSet(Sql.ToFormattableString())
                        .Select<Dictionary<string, object>, T>(x =>
                          {
                              baselist.Add(x.CreateOrSetObject(null, eltipo));
                              return default;
                          });
                        prop.SetValue(d, baselist);
                        if (Recursive)
                        {
                            foreach (var uu in baselist) Connection.ProccessSubQuery(uu, Recursive);
                        }

                        return d;
                    }
                    else if (prop.PropertyType.IsClass)
                    {
                        if (prop.GetValue(d) is null)
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
                        if (prop.GetValue(d) is null)
                        {
                            var oo = Connection.RunSQLValue(Sql.ToFormattableString());
                            prop.SetValue(d, Conversion.CTypeDynamic(oo, prop.PropertyType));
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
        public static T ProccessSubQuery<T>(this DbConnection Connection, T d, bool Recursive = false)
        {
            foreach (var prop in ClassTools.GetProperties(d).Where(x => x.HasAttribute<FromSQL>()))
                Connection.ProccessSubQuery(d, prop.Name, Recursive);
            return d;
        }

        /// <summary>
        /// Mapeia a primeira linha de um datareader para uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Reader"></param>
        /// <param name="args">argumentos para o construtor da classe</param>
        /// <returns></returns>
        public static T MapFirst<T>(this DbDataReader Reader, params object[] args) where T : class
        {
            return Reader.Map<T>(args).FirstOrDefault();
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para um <see cref="IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))"/>
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> MapMany(this DbDataReader Reader)
        {
            var l = new List<IEnumerable<Dictionary<string, object>>>();
            if (Reader != null)
            {
                do
                    l.Add(Reader.Map<Dictionary<string, object>>());
                while (Reader != null && Reader.NextResult());
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
    }
}