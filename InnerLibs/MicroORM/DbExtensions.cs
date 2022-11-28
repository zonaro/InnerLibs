using InnerLibs.LINQ;
using InnerLibs.TimeMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace InnerLibs.MicroORM
{
    /// <summary>
    /// Constantes utilizadas na funçao <see cref="DbExtensions.CreateSQLQuickResponse(DbCommand,
    /// string, bool)"/> e <see cref="DbExtensions.CreateSQLQuickResponse(DbConnection,
    /// FormattableString, string,bool)"/>
    /// </summary>
    public static class DataSetType
    {
        /// <summary>
        /// Coloca todos os datasets no <see cref="SQLResponse{T}.Data"/>.
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "DEFAULT", "SETS"</remarks>
        public const string Many = "MANY";

        /// <summary>
        /// Coloca primeira e ultima coluna do primeiro dataset no <see cref="SQLResponse{T}.Data"/> como um <see cref="Dictionary{string, object}"/>
        /// </summary>
        ///<remarks>
        /// pode tambem ser representada pelas strings "PAIRS", "DICTIONARY", "ASSOCIATIVE",
        ///</remarks>
        public const string Pair = "PAIR";

        /// <summary>
        /// Coloca a primeira coluna do primeiro dataset no <see cref="SQLResponse{T}.Data"/>
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "ONE", "FIRST"</remarks>
        public const string Row = "ROW";

        /// <summary>
        /// Coloca o primeiro valor da primeira linha do primeiro dataset no <see cref="SQLResponse{T}.Data"/>
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "SINGLE", "ID", "KEY"</remarks>
        public const string Value = "VALUE";

        /// <summary>
        /// Coloca todos os valores encontrados na primeira coluna do primeiro dataset no <see cref="SQLResponse{T}.Data"/>
        /// </summary>
        /// <remarks>pode tambem ser representado pelas strings "ARRAY", "LIST"</remarks>
        public const string Values = "VALUES";

        public static IEnumerable<string> ToList() => new List<string>() { Many, Pair, Row, Value, Values };
    }

    /// <summary>
    /// Enxtensões para <see cref="DbConnection"/> e classes derivadas
    /// </summary>
    public static class DbExtensions
    {
        public enum LogicConcatenationOperator
        {
            AND,
            OR
        }

        /// <summary>
        /// Dicionario com os <see cref="Type"/> e seu <see cref="DbType"/> correspondente
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Type, DbType> DbTypes => new Dictionary<Type, DbType>()
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char[])] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary
        };

        /// <summary>
        /// Quando Configurado, escreve os parametros e queries executadas no <see
        /// cref="TextWriter"/> específico
        /// </summary>
        /// <returns></returns>
        public static TextWriter LogWriter { get; set; } = new DebugTextWriter();

        public static string AsSQLColumns(this IDictionary<string, object> obj, char Quote = '[') => obj.Select(x => x.Key.ToString().Quote(Quote)).SelectJoinString(",");

        public static string AsSQLColumns<T>(this T obj, char Quote = '[') where T : class => obj.GetNullableTypeOf().GetProperties().SelectJoinString(x => x.Name.Quote(Quote), ",");

        public static string AsSQLColumns(this NameValueCollection obj, char Quote = '[', params string[] Keys) => obj.ToDictionary(Keys).AsSQLColumns(Quote);

        /// <summary>
        /// Valida se uma conexao e um comando nao sao nulos. Valida se o texto do comando esta em
        /// branco e associa este comando a conexao especifica. Escreve o comando no <see
        /// cref="LogWriter"/> e retorna o mesmo
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DbCommand BeforeRun(ref DbConnection Connection, ref DbCommand Command, TextWriter LogWriter = null)
        {
            Connection = Connection ?? Command?.Connection;
            if (Command == null || Command.CommandText.IsBlank())
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

        public static IEnumerable<string> ColumnsFromClass<T>()
        {
            var PropInfos = typeof(T).GetProperties().Select(y => y.GetAttributeValue<ColumnName, string>(x => x.Names.FirstOrDefault()).IfBlank(y.Name));
            var FieldInfos = typeof(T).GetProperties().Select(y => y.GetAttributeValue<ColumnName, string>(x => x.Names.FirstOrDefault()).IfBlank(y.Name)).Where(x => x.IsNotIn(PropInfos));

            return PropInfos.Union(FieldInfos);
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de um arquivo SQL e um objeto,
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand<T>(this DbConnection Connection, FileInfo SQLFile, T obj, DbTransaction Transaction = null) => CreateCommand(Connection, SQLFile.Exists ? SQLFile.ReadAllText() : InnerLibs.Text.Empty, obj, Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um objeto,
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand<T>(DbConnection connection, string SQL, T obj, DbTransaction transaction = null) => CreateCommand(connection, SQL.Inject(obj, true).ToFormattableString(), transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see cref="Dictionary(Of
        /// String, Object)"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, Dictionary<string, object> Parameters, DbTransaction Transaction = null)
        {
            if (SQLFile != null && SQLFile.Exists)
            {
                return CreateCommand(Connection, SQLFile.ReadAllText(), Parameters, Transaction);
            }
            return null;
        }

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
            if (Connection != null && SQL.IsNotBlank())
            {
                var command = Connection.CreateCommand();
                command.CommandText = SQL;
                if (Parameters != null && Parameters.Any())
                {
                    foreach (var p in Parameters.Keys)
                    {
                        var v = Parameters.GetValueOr(p);
                        var arr = Converter.ForceArray(v, typeof(object)).ToList();
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
            if (SQL.IsNotBlank())
            {
                return Connection.CreateCommand(SQL.ToFormattableString(Args), Transaction);
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
                        var v = Converter.ForceArray(valores, typeof(object)).ToList();
                        var param_names = new List<string>();
                        for (int v_index = 0, loopTo1 = v.Count() - 1; v_index <= loopTo1; v_index++)
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
                cmd.CommandText = string.Format($"INSERT INTO " + TableName.IfBlank(d.Name) + " ({0}) values ({1})", dic.Keys.SelectJoinString(","), dic.Keys.SelectJoinString(x => $"@__{x}", ","));
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

        public static SQLResponse<object> CreateSQLQuickResponse(this DbConnection Connection, FormattableString Command, string DataSetType, bool IncludeCommandText = false) => CreateSQLQuickResponse(Connection.CreateCommand(Command), DataSetType, IncludeCommandText);

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
        public static SQLResponse<object> CreateSQLQuickResponse(this DbCommand Command, string DataSetType, bool IncludeCommandText = false)
        {
            var resp = new SQLResponse<object>();
            try
            {
                DataSetType = DataSetType.IfBlank("default").ToLower();
                var Connection = Command?.Connection;
                if (Connection == null)
                {
                    resp.Status = "ERROR";
                    resp.Message = "Command or Connection is null";
                    return resp;
                }
                resp.SQL = IncludeCommandText.AsIf(Command.CommandText);
                if (DataSetType.IsAny("value", "single", "id", "key"))
                {
                    //primeiro valor da primeira linha do primeiro set
                    var part = Connection.RunSQLValue(Command);
                    resp.Status = (part == DBNull.Value).AsIf("NULL_VALUE", (part == null).AsIf("ZERO_RESULTS", "OK"));
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("one", "first", "row"))
                {
                    //primeiro do primeiro set (1 linha como objeto)
                    var part = Connection.RunSQLRow(Command);
                    resp.Status = (part == null).AsIf("ZERO_RESULTS", "OK");
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("array", "values", "list"))
                {
                    //primeira coluna do primeiro set como array
                    var part = Connection.RunSQLArray(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("pair", "pairs", "dictionary", "associative"))
                {
                    //primeira e ultima coluna do primeiro set como dictionary
                    var part = Connection.RunSQLPairs(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("many", "sets"))
                {
                    //varios sets
                    var part = Connection.RunSQLMany(Command);
                    resp.Status = (part?.Any(x => x.Any())).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
                }
                else
                {
                    //tudo do primeiro set (lista de objetos)
                    var part = Connection.RunSQLSet(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
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
                cmd.CommandText = $"UPDATE " + TableName.IfBlank(d.Name) + " set" + Environment.NewLine;
                foreach (var k in dic.Keys)
                {
                    cmd.CommandText += $"{k} = @__{k}, {Environment.NewLine}";
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
                    cmd.CommandText += $"{Environment.NewLine}{wheretxt.PrependIf("WHERE ", x => !x.StartsWith("WHERE"))}";
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
        /// Retorna um <see cref="DbType"/> a partir do <see cref="Type"/> do <paramref name="obj"/>
        /// </summary>
        public static DbType GetDbType<T>(this T obj, DbType DefaultType = DbType.Object) => DbTypes.GetValueOr(Misc.GetNullableTypeOf(obj), DefaultType);

        public static DataRow GetFirstRow(this DataSet Data) => Data.GetFirstTable()?.GetFirstRow();

        public static DataRow GetFirstRow(this DataTable Table)
        {
            if (Table != null && Table.Rows.Count > 0)
                return Table.Rows[0];
            else
                return null;
        }

        public static DataTable GetFirstTable(this DataSet Data)
        {
            if (Data != null && Data.Tables.Count > 0)
                return Data.Tables[0];
            else
                return null;
        }
        public static T GetValue<T>(this DataRow row, int ColumnIndex = 0)
        {
            try
            {
                return Converter.ChangeType<T>(row != null ? row[ColumnIndex] : default);
            }
            catch
            {
                return default;
            }
        }
        public static T GetValue<T>(this DataRow row, string ColumnNameOrIndex)
        {
            try
            {
                return Converter.ChangeType<T>(row != null ? row[ColumnNameOrIndex] : default);
            }
            catch
            {
                if (ColumnNameOrIndex.IsNumber())
                {
                    return GetValue<T>(row, ColumnNameOrIndex.ToInt());
                }
            }
            return default;
        }

        public static T GetSingleValue<T>(this DataSet data, string ColumnNameOrIndex)
        {
            var row = data?.GetFirstRow();
            if (row != null)
                return row.GetValue<T>(ColumnNameOrIndex);
            return default;
        }

        public static T GetSingleValue<T>(this DataSet data, int ColumnIndex = 0)
        {
            var row = data?.GetFirstRow();
            if (row != null)
                return row.GetValue<T>(ColumnIndex);
            return default;
        }

        public static T GetSingleValue<T>(this DataTable table, string ColumnNameOrIndex)
        {
            var row = table?.GetFirstRow();
            if (row != null)
                return row.GetValue<T>(ColumnNameOrIndex);
            return default;
        }

        public static T GetSingleValue<T>(this DataTable table, int ColumnIndex = 0)
        {
            var row = table?.GetFirstRow();
            if (row != null)
                return row.GetValue<T>(ColumnIndex);
            return default;
        }



        /// <summary>
        /// Retorna um <see cref="Type"/> de um <see cref="DbType"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <param name="DefaultType"></param>
        /// <returns></returns>
        public static Type GetTypeFromDb(this DbType Type, Type DefaultType = null) => DbTypes.Where(x => x.Value == Type).Select(x => x.Key).FirstOrDefault() ?? DefaultType ?? typeof(object);

        /// <inheritdoc cref="GetValue{T}(DataRow, string, Expression{Func{object, object}})"/>
        public static string GetValue(this DataRow row, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(row, Name, valueParser);

        /// <summary>
        /// Retorna o valor da coluna <paramref name="Name"/> de uma <see cref="DataRow"/>
        /// convertido para <typeparamref name="T"/> e previamente tratado pela função <paramref name="valueParser"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="Name"></param>
        /// <param name="valueParser"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRow row, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            try
            {
                if (row == null)
                {
                    throw new ArgumentException("Row is null");
                }

                object v = null;

                if (Name.IsNotBlank() && Name.IsNotNumber())
                {
                    v = row[Name];
                }
                else
                {
                    v = row[Name.IfBlank(0)];
                }

                if (v == null || v == DBNull.Value)
                {
                    throw new Exception("Value is null");
                }

                if (valueParser != null)
                {
                    v = valueParser.Compile().Invoke(v);
                }

                if (typeof(T).IsEnum)
                {
                    return v.ToString().GetEnumValue<T>();
                }
                else
                {
                    return v.ChangeType<T>();
                }
            }
            catch (Exception ex)
            {
                LogWriter.WriteLine(ex.ToFullExceptionString());
                return default;
            }
        }

        public static string GetValue(this DataTable Table, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(Table, Name, valueParser);

        public static string GetValue(this DataSet Data, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(Data, Name, valueParser);

        public static T GetValue<T>(this DataSet Data, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            var r = Data.GetFirstRow();
            return r == null ? default : r.GetValue<T>(Name, valueParser);
        }

        public static T GetValue<T>(this DataTable Table, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            var r = Table.GetFirstRow();
            return r == null ? default : r.GetValue<T>(Name, valueParser);
        }

        public static bool IsBroken(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Broken);

        public static bool IsClosed(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Closed);

        public static bool IsConnecting(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Connecting);

        public static bool IsExecuting(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Executing);

        public static bool IsOpen(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Open);

        /// <summary>
        /// Utiliza o <see cref="TextWriter"/> especificado em <see cref="LogWriter"/> para
        /// escrever o comando
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static DbCommand LogCommand(this DbCommand Command, TextWriter LogWriter = null)
        {
            LogWriter = LogWriter ?? DbExtensions.LogWriter ?? new DebugTextWriter();
            if (LogWriter != null)
            {
                LogWriter.WriteLine(Environment.NewLine);
                LogWriter.WriteLine("=".Repeat(10));
                if (Command != null)
                {
                    foreach (DbParameter item in Command.Parameters)
                    {
                        string bx = $"Parameter: @{item.ParameterName}{Environment.NewLine}Value: {item.Value}{Environment.NewLine}Type: {item.DbType}{Environment.NewLine}Precision/Scale: {item.Precision}/{item.Scale}";
                        LogWriter.WriteLine(bx);
                        LogWriter.WriteLine("-".Repeat(10));
                    }

                    LogWriter.WriteLine($"Command: {Command.CommandText}");
                    LogWriter.WriteLine("/".Repeat(10));

                    if (Command.Transaction != null)
                    {
                        LogWriter.WriteLine($"Transaction Isolation Level: {Command.Transaction.IsolationLevel}");
                    }
                    else
                    {
                        LogWriter.WriteLine($"No transaction specified");
                    }
                }
                else
                {
                    LogWriter.WriteLine("Command is NULL");
                }

                LogWriter.WriteLine("=".Repeat(10));
                LogWriter.WriteLine(Environment.NewLine);
            }

            return Command;
        }

        public static DataSet ToDataSet(this DbDataReader reader) => ToDataSet(reader, null);
        public static DataSet ToDataSet(this DbDataReader reader, string DataSetName, params string[] TableNames)
        {
            DataSet ds = new DataSet(DataSetName.IfBlank("DataSet"));
            TableNames = TableNames ?? Array.Empty<string>();
            var i = 0;
            while (reader != null && !reader.IsClosed)
            {
                ds.Tables.Add(TableNames.IfBlankOrNoIndex(i, $"Table{i}")).Load(reader);
                i++;
            }
            return ds;
        }

        public static T MapFirst<T>(this DataSet Data, params object[] args) where T : class => Data.GetFirstRow().Map<T>(args);
        public static T MapFirst<T>(this DataTable Data, params object[] args) where T : class => Data.GetFirstRow().Map<T>(args);
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
                        var propnames = name.PropertyNamesFor().ToList();
                        var PropInfos = Misc.GetTypeOf(d).GetProperties().Where(x => x.GetCustomAttributes<ColumnName>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
                        var FieldInfos = Misc.GetTypeOf(d).GetFields().Where(x => x.GetCustomAttributes<ColumnName>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase)).Where(x => x.Name.IsNotIn(PropInfos.Select(y => y.Name)));
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
                                    info.SetValue(d, Converter.ChangeType(value, info.PropertyType));
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
                                info.SetValue(d, Converter.ChangeType(value, info.FieldType));
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
                        var PropInfos = Misc.GetTypeOf(d).GetProperties().Where(x => x.GetCustomAttributes<ColumnName>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
                        var FieldInfos = Misc.GetTypeOf(d).GetFields().Where(x => x.GetCustomAttributes<ColumnName>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase)).Where(x => x.Name.IsNotIn(PropInfos.Select(y => y.Name)));
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
                                    info.SetValue(d, Converter.ChangeType(value, info.PropertyType));
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
                                info.SetValue(d, Converter.ChangeType(value, info.FieldType));
                            }
                        }
                    }
                }

                l.Add(d);
            }

            return l.AsEnumerable();
        }

        /// <summary>
        /// Mapeia a primeira linha de um datareader para uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Reader"></param>
        /// <param name="args">argumentos para o construtor da classe</param>
        /// <returns></returns>
        public static T MapFirst<T>(this DbDataReader Reader, params object[] args) where T : class => Reader.Map<T>(args).FirstOrDefault();

        /// <summary>
        /// Mapeia os resultsets de um datareader para um <see cref="IEnumerable(Of IEnumerable(Of
        /// Dictionary(Of String, Object)))"/>
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

        public static ConnectionType OpenConnection<ConnectionType>(this ConnectionStringParser connection) where ConnectionType : DbConnection
        {
            ConnectionType dbcon = Activator.CreateInstance<ConnectionType>();
            dbcon.ConnectionString = connection.ConnectionString;
            dbcon.Open();
            return dbcon;
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
                            prop.SetValue(d, Converter.ChangeType(oo, prop.PropertyType));
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
            foreach (var prop in Misc.GetProperties(d).Where(x => x.HasAttribute<FromSQL>()))
            {
                Connection.ProccessSubQuery(d, prop.Name, Recursive);
            }

            return d;
        }

        public static string QueryForClass<T>(object InjectionObject = null) => typeof(T).GetAttributeValue<FromSQL, string>(x => x.SQL).IfBlank($"SELECT * FROM {typeof(T).Name}").Inject(InjectionObject);

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
            where T2 : class
        {
            return Connection.RunSQLMany<T1, T2>(Connection.CreateCommand(SQL, Transaction));
        }

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
        public static int RunSQLNone(this DbConnection Connection, DbCommand Command) => BeforeRun(ref Connection, ref Command).ExecuteNonQuery();

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
        public static Dictionary<K, V> RunSQLPairs<K, V>(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLPairs(SQL).ToDictionary(x => x.Key.ChangeType<K>(), x => x.Value.ChangeType<V>());

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{K, V}"/>
        /// </summary>
        public static Dictionary<K, V> RunSQLPairs<K, V>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLPairs<K, V>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o <see cref="DbDataReader"/> com os resultados
        /// </summary>
        public static DbDataReader RunSQLReader(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLReader(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o <see cref="DbDataReader"/> com os resultados
        /// </summary>
        public static DbDataReader RunSQLReader(this DbConnection Connection, DbCommand Command) => BeforeRun(ref Connection, ref Command).ExecuteReader();

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
        /// um <see cref="Dictionary(Of String, Object)"/>
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

        public static T RunSQLRow<T>(this DbConnection Connection, bool WithSubQueries = false, DbTransaction Transaction = null, object InjectionObject = null) where T : class => RunSQLRow<T>(Connection, QueryForClass<T>(InjectionObject).ToFormattableString(), WithSubQueries, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, Select<T> Select, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLSet<T>(Select.CreateDbCommand(Connection, Transaction), WithSubQueries);

        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, bool WithSubQueries = false, DbTransaction Transaction = null, object InjectionObject = null) where T : class => RunSQLSet<T>(Connection, QueryForClass<T>(InjectionObject).ToFormattableString(), WithSubQueries, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <see cref="Dictionary(Of String, Object)"/>
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
        /// Retorna o primeiro resultado da primeira coluna de uma consulta SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static object RunSQLValue(this DbConnection Connection, DbCommand Command) => BeforeRun(ref Connection, ref Command).ExecuteScalar();

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL
        /// </summary>
        public static object RunSQLValue(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLValue(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL como um tipo
        /// <typeparamref name="V"/>
        /// </summary>
        public static V RunSQLValue<V>(this DbConnection Connection, DbCommand Command)
        {
            if (!typeof(V).IsValueType())
            {
                throw new ArgumentException("The type param V is not a value type or string");
            }
            var vv = Connection.RunSQLValue(Command);
            return vv != null && vv != DBNull.Value ? vv.ChangeType<V>() : default;
        }

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL como um tipo
        /// <typeparamref name="V"/>
        /// </summary>
        public static V RunSQLValue<V>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLValue<V>(Connection.CreateCommand(SQL, Transaction));

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
            Dic = Dic ?? new Dictionary<string, object>();
            Keys = Keys ?? Array.Empty<string>();
            if (!Keys.Any())
            {
                Keys = Dic.Keys.ToArray();
            }
            else
            {
                Keys = Dic.Keys.ToArray().Where(x => x.IsLikeAny(Keys)).ToArray();
            }

            string sql = $"{ProcedureName} {Keys.SelectJoinString(key => $" @{key} = @__{key}", ", ")}";

            return Connection.CreateCommand(sql, Dic.ToDictionary(x => x.Key, x => x.Value), Transaction);
        }

        ///<summary> Monta um Comando SQL para executar um SELECT com
        /// filtros a partir de um <see cref="NameValueCollection" />
        /// </summary>
        /// <param name="NVC"> Dicionario</param> <param name="TableName">Nome da Tabela</param>
        public static Select ToSQLFilter(this NameValueCollection NVC, string TableName, string CommaSeparatedColumns, params string[] FilterKeys) => (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(NVC, FilterKeys);

        /// <summary>
        /// Monta um Comando SQL para executar um SELECT com filtros a partir de um <see
        /// cref="Dictionary(Of String, Object)"/>
        /// </summary>
        /// <param name="Dic">Dicionario</param>
        /// <param name="TableName">Nome da Tabela</param>
        /// <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
        /// <returns>Uma string com o comando montado</returns>
        public static Select ToSQLFilter(this Dictionary<string, object> Dic, string TableName, string CommaSeparatedColumns, LogicConcatenationOperator LogicConcatenation, params string[] FilterKeys) => (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(Dic, LogicConcatenation, FilterKeys);

        /// <summary>
        /// Interploa um objeto de tipo <typeparamref name="T"/> em uma <see
        /// cref="FormattableString"/>, e retorna o resultado de <see
        /// cref="ToSQLString(FormattableString, bool)"/>
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static string ToSQLString<T>(this T Obj, bool Parenthesis = true) => ToSQLString("{0}".ToFormattableString(Obj), Parenthesis);

        /// <summary>
        /// Converte uma <see cref="FormattableString"/> para uma string SQL, tratando seus
        /// parametros como parametros da query
        /// </summary>
        /// <param name="Parenthesis">indica se o parametro deve ser encapsulando em parentesis</param>
        public static string ToSQLString(this FormattableString SQL, bool Parenthesis = true)
        {
            if (SQL != null)
            {
                if (SQL.ArgumentCount > 0)
                {
                    string CommandText = SQL.Format.TrimBetween();
                    for (int index = 0, loopTo = SQL.ArgumentCount - 1; index <= loopTo; index++)
                    {
                        var valores = SQL.GetArgument(index);
                        var v = Converter.ForceArray(valores, typeof(object));
                        var paramvalues = new List<object>();

                        for (int v_index = 0, loopTo1 = v.Count() - 1; v_index <= loopTo1; v_index++)
                        {
                            paramvalues.Add(v[v_index]);
                        }

                        var pv = paramvalues.Select(x =>
                        {
                            if (x == null)
                            {
                                return "NULL";
                            }
                            else if (Misc.GetNullableTypeOf(x).IsNumericType())
                            {
                                return x.ToString();
                            }
                            else if (Verify.IsDate(x))
                            {
                                return Convert.ToDateTime(x).ToSQLDateString().EscapeQuotesToQuery(true);
                            }
                            else if (Verify.IsBool(x))
                            {
                                return Convert.ToBoolean(x).AsIf(1, 0).ToString();
                            }
                            else if (x.IsTypeOf<Select>())
                            {
                                return x.ToString();
                            }
                            else
                            {
                                return x.ToString().EscapeQuotesToQuery(true);
                            }
                        }).ToList();
                        CommandText = CommandText.Replace("{" + index + "}", pv.SelectJoinString(",").IfBlank("NULL").UnQuote('(', true).QuoteIf(Parenthesis, '('));
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
    }

    public class SQLResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public string SQL { get; set; }
        public string Status { get; set; }
    }
}