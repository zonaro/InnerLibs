
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace InnerLibs
{
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

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public sealed class ColumnNameAttribute : Attribute
    {
        #region Public Constructors

        public ColumnNameAttribute(string ColumnName, params string[] AlternativeNames)
        {
            if (ColumnName.IsBlank())
            {
                throw new ArgumentException("ColumnName is null or blank");
            }
            var l = ColumnName.StartList();
            l.AddRange(AlternativeNames ?? Array.Empty<string>());

            Names = l.Select(x => x.UnQuote()).SelectMany(x => x.Split(",")).ToArray();
        }

        #endregion Public Constructors

        #region Public Properties

        public string[] Names { get; private set; }

        #endregion Public Properties
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
                if (condition != null && condition.ToString().IsNotBlank())
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
                if (condition != null && condition.ToString().IsNotBlank())
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
            if (condition != null && condition.ToString().IsNotBlank())
            {
                _tokens.Add(condition.ParenthesisToString());
            }
        }

        /// <summary>
        /// condition class constructor
        /// </summary>
        public Condition(string Column, object Value, string Operator = "=")
        {
            if (Column.IsNotBlank())
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

                _tokens.Add($"{Column} {Operator.IfBlank("=")} {Ext.ToSQLString(Value)}");
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
            if (!(condition == null) && condition.ToString().IsNotBlank())
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
            if (!(condition == null) && condition.ToString().IsNotBlank())
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
        /// Query SQL para esta classe
        /// </summary>
        public string SQL
        {
            get
            {
                if (sql.IsNotBlank())
                {
                    return sql;
                }

                if (File.IsNotBlank())
                {
                    if (File.IsFilePath())
                    {
                        if (System.IO.File.Exists(File))
                        {
                            return System.IO.File.ReadAllText(File).ValidateOr(x => x.IsNotBlank(), new ArgumentException("No file content"));
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

                return $"SELECT * FROM {TableName.ValidateOr(x => x.IsNotBlank(), new ArgumentException("No table name defined"))}";
            }

            set
            {
                if (value.IsNotBlank() && value != sql)
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
        /// The final Query string generated by this instance
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
                var props = eltipo.GetProperties().Select(x => x.GetAttributeValue<ColumnNameAttribute, string>(y => y.Names.FirstOrDefault()).IfBlank(x.Name));

                AddColumns(props.ToArray());
            }

            return this;
        }

        public Select<T> AddColumns(params string[] Columns)
        {
            Columns = (Columns ?? Array.Empty<string>()).SelectMany(x => x.Split(",")).Distinct().Where(x => x.IsNotBlank()).ToArray();
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
                if (condition != null && condition.ToString().IsNotBlank())
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

        public Select<T> AndIn<TO>(string Column, params TO[] Items) => And(Ext.ToFormattableString(Column + " in " + Items.ToSQLString()));

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
                    c = item.UnQuote().Split(".", StringSplitOptions.RemoveEmptyEntries).SelectJoinString(x => Ext.FormatSQLColumn(this.QuoteChar, WithTableName.AsIf(this.GetTableOrSubQuery()), x.UnQuote()));
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
        public string FormatColumnName(string ColumnName) => Ext.FormatSQLColumn(this.QuoteChar, GetTableOrSubQuery(), ColumnName);

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="TableOrSubQuery">Table to be selected from</param>
        /// <returns></returns>
        public Select<T> From(string TableOrSubQuery)
        {
            if (TableOrSubQuery.IsNotBlank())
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
            if (SubQuery != null && SubQuery.ToString(true).IsNotBlank() && !ReferenceEquals(SubQuery, this))
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

        public Select<T> FullOuterJoin(string table, string ThisColumn, string ForeignColumn) => FullOuterJoin(table, Ext.ToFormattableString(Ext.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + Ext.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

        /// <summary>
        /// Get the table name or subquery alias used in this select
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
            if (condition.IsNotBlank())
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

        public Select<T> InnerJoin(string table, string ThisColumn, string ForeignColumn) => InnerJoin(table, Ext.ToFormattableString(Ext.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + Ext.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

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
            if (Table.IsNotBlank() && (JoinType == JoinType.CrossApply || (!(on == null) && on.ToString().IsNotBlank())))
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

        public Select<T> LeftOuterJoin(string table, string ThisColumn, string ForeignColumn) => LeftOuterJoin(table, Ext.ToFormattableString(Ext.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + Ext.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

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
                if (condition != null && condition.ToString().IsNotBlank())
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

        public Select<T> OrIn<TO>(string Column, params TO[] Items) => Or(Ext.ToFormattableString(Column + " in " + Items.ToSQLString()));

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

        public Select<T> RightOuterJoin(string table, string ThisColumn, string ForeignColumn) => RightOuterJoin(table, Ext.ToFormattableString(Ext.FormatSQLColumn(QuoteChar, GetTableOrSubQuery(), ThisColumn) + " = " + Ext.FormatSQLColumn(QuoteChar, table, ForeignColumn.IfBlank(ThisColumn))));

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

            if (_top?.IsNotBlank() ?? false)
            {
                sql.Append($"{_top}");
            }
            var cols = (_columns?.Distinct().SelectJoinString(",") ?? InnerLibs.Ext.EmptyString).IfBlank(" * ");
            sql.Append(cols);
            if (_fromsub != null && _fromsub.ToString().IsNotBlank())
            {
                _from = _fromsub.ToString(true).Quote('(') + " as " + _fromsubname;
            }

            if (_from?.IsNotBlank() ?? false)
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

            if (_offset?.IsNotBlank() ?? false && AsSubquery == false)
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
                if (condition != null && condition.ToString().IsNotBlank())
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
                    { $"{pName.Name}.", InnerLibs.Ext.EmptyString},
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
                    {Ext.DoubleQuoteChar, InnerLibs.Ext.SingleQuoteChar}
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
            if (condition != null && condition.ToString().IsNotBlank())
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
        public object Where(Dictionary<string, object> Dic, Ext.LogicConcatenationOperator LogicConcatenation, params string[] FilterKeys)
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

            FilterKeys = FilterKeys.Where(x => Dic[x] != null && Dic[x].ToString().IsNotBlank()).ToArray();
            if (FilterKeys.Any())
            {
                foreach (var f in FilterKeys)
                {
                    if (LogicConcatenation == Ext.LogicConcatenationOperator.OR)
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
                if (k.IsNotBlank())
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

        public Select<T> WhereObject<TO>(TO Obj, Ext.LogicConcatenationOperator LogicOperator) where TO : class => WhereObject(Obj, LogicOperator.GetEnumValueAsString());

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