using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using InnerLibs.LINQ;
using Microsoft.VisualBasic;

namespace InnerLibs.MicroORM
{
    public interface ISelect
    {
        string ToString();

        string ToString(bool SubQuery);
    }

    public class Select : Select<Dictionary<string, object>>
    {
        public Select() : base()
        {
        }

        public Select(params string[] columns) : base(columns)
        {
        }

        public Select(Dictionary<string, object> Obj) : base(Obj)
        {
        }
    }

    public class Select<T> : ISelect where T : class
    {
        public Select()
        {
            SetColumns<T>();
            From<T>();
        }

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

        public Select<T> ColumnQuote(char QuoteChar)
        {
            var _nova = new List<string>();
            foreach (var item in _columns ?? new List<string>())
                _nova.Add(item.UnQuote().Split(".", StringSplitOptions.RemoveEmptyEntries).SelectJoin(x => x.UnQuote().Quote(QuoteChar), "."));
            SetColumns(_nova.ToArray());
            return this;
        }

        internal List<string> _columns;
        internal string _from;
        internal ISelect _fromsub;
        internal string _fromsubname;
        internal List<Join> _joins;
        internal Condition _where;
        internal List<string> _groupBy;
        internal string _having;
        internal List<string> _orderBy;
        internal string _offset;
        internal bool _desc;

        public Select<T> AddColumns<O>(O Obj = null) where O : class
        {
            var eltipo = typeof(O).GetNullableTypeOf();
            if (eltipo == typeof(Dictionary<string, object>))
            {
                if (Obj != null)
                {
                    AddColumns(((Dictionary<string, object>)(object)Obj).Keys.ToArray());
                }
            }
            else
            {
                AddColumns(eltipo.GetProperties().Select(x => x.Name).ToArray());
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

        public Select<T> SetColumns(params string[] Columns)
        {
            _columns = null;
            AddColumns(Columns);
            return this;
        }

        public Select<T> SetColumns<O>(O Obj = null) where O : class
        {
            _columns = null;
            AddColumns(Obj);
            return this;
        }

        public Select<T> RemoveColumns(params string[] Columns)
        {
            if (_columns != null)
            {
                _columns = _columns.Where(x => x.IsNotIn(Columns ?? Array.Empty<string>())).ToList();
            }

            return this;
        }

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="TableOrSubQuery">Table to be selected from</param>
        /// <returns></returns>
        public Select<T> From(string TableOrSubQuery)
        {
            if (TableOrSubQuery.IsNotBlank())
            {
                _from = TableOrSubQuery.QuoteIf(TableOrSubQuery.StartsWith("SELECT "), "(");
                _fromsub = null;
            }

            return this;
        }

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="SubQuery">Subquery to be selected from</param>
        /// <returns></returns>
        public Select<T> From<O>(Select<O> SubQuery, string SubQueryAlias = null) where O : class
        {
            if (SubQuery != null && SubQuery.ToString(true).IsNotBlank() && !ReferenceEquals(SubQuery, this))
            {
                _from = null;
                _fromsub = SubQuery;
                _fromsubname = SubQueryAlias.IfBlank(typeof(O).Name + "_" + DateAndTime.Now.Ticks);
            }

            return this;
        }

        /// <summary>
        /// Sets the FROM clause in the SELECT being built.
        /// </summary>
        /// <param name="SubQuery">Subquery to be selected from</param>
        /// <returns></returns>
        public Select<T> From<O>(Action<Select<O>> SubQuery, string SubQueryAlias = null) where O : class
        {
            if (SubQuery != null)
            {
                var sl = new Select<O>();
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
        public Select<T> From<O>()
        {
            From(typeof(O).GetNullableTypeOf().Name);
            return this;
        }

        /// <summary>
        /// Sets a JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Join(string table, FormattableString on)
        {
            return _Join(JoinType.Join, table, new Condition(on));
        }

        /// <summary>
        /// Sets a INNER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> InnerJoin(string table, FormattableString on)
        {
            return _Join(JoinType.Inner, table, new Condition(on));
        }

        /// <summary>
        /// Sets a INNER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> InnerJoin(string table, Condition on)
        {
            return _Join(JoinType.Inner, table, on);
        }

        /// <summary>
        /// Sets a JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Join(string table, Condition on)
        {
            return _Join(JoinType.Join, table, on);
        }

        /// <summary>
        /// Sets a LEFT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> LeftOuterJoin(string table, FormattableString on)
        {
            return _Join(JoinType.LeftOuterJoin, table, new Condition(on));
        }

        /// <summary>
        /// Sets a LEFT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> LeftOuterJoin(string table, Condition on)
        {
            return _Join(JoinType.LeftOuterJoin, table, on);
        }

        /// <summary>
        /// Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> RightOuterJoin(string table, FormattableString on)
        {
            return _Join(JoinType.RightOuterJoin, table, new Condition(on));
        }

        /// <summary>
        /// Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> RightOuterJoin(string table, Condition on)
        {
            return _Join(JoinType.RightOuterJoin, table, on);
        }

        /// <summary>
        /// Sets a FULL OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> FullOuterJoin(string table, FormattableString on)
        {
            return _Join(JoinType.FullOuterJoin, table, new Condition(on));
        }

        /// <summary>
        /// Sets a FULL OUTER JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <param name="on">Condition of the join (ON clause)</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> FullOuterJoin(string table, Condition on)
        {
            return _Join(JoinType.FullOuterJoin, table, on);
        }

        /// <summary>
        /// Sets a CROSS JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> CrossJoin(string table)
        {
            return _Join(JoinType.CrossJoin, table, null);
        }

        /// <summary>
        /// Sets a CROSS JOIN clause in the SELECT being built.
        /// </summary>
        /// <param name="table">Table to be join</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> CrossApply(string table)
        {
            return _Join(JoinType.CrossApply, table, null);
        }

        public Select<T> WhereObject<O>(O Obj) where O : class
        {
            return WhereObject(Obj, "AND");
        }

        public Select<T> WhereObject<O>(O Obj, string LogicOperator = "AND") where O : class
        {
            if (Obj != null)
            {
                Where(new Condition(LogicOperator, Obj.GetNullableTypeOf().GetProperties().Where(x => x.CanRead).Select(x => (x.Name + " = {0}").ToFormattableString(x.GetValue(Obj))).ToArray()));
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// </summary>
        /// <param name="condition">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(FormattableString condition)
        {
            if (condition.ToString().IsNotBlank())
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
            foreach (var condition in conditions ?? Array.Empty<FormattableString>())
            {
                if (condition != null && condition.ToString().IsNotBlank())
                {
                    if (LogicOperator.IsIn("OR", "or"))
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
            foreach (var condition in conditions ?? Array.Empty<Condition>())
            {
                if (condition != null)
                {
                    if (LogicOperator.IsIn("OR", "or"))
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
        /// Sets the WHERE clause in the SELECT being built using a lambda expression. This method is experimental
        /// </summary>
        /// <param name="predicate">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                var p = new StringBuilder(predicate.Body.ToString());
                var pName = predicate.Parameters.First();
                p.Replace(pName.Name + ".", "");
                p.Replace("==", "=");
                p.Replace("!=", "<>");
                p.Replace("AndAlso", "AND");
                p.Replace("OrElse", "OR");
                p.Replace(" like ", " LIKE ");
                p.Replace(".Contains", " LIKE ");
                p.Replace(".Like", " LIKE ");
                p.Replace(".Equal", " = ");
                p.Replace("\"", "'");
                Where(p.ToString().ToFormattableString());
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// </summary>
        /// <param name="condition">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(Condition condition)
        {
            if (condition != null && condition.ToString().IsNotBlank())
            {
                if (_where != null) And(condition); else _where = new Condition(condition);
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Conditions to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Where(params Condition[] conditions)
        {
            return And(conditions);
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built using a <see cref="Dictionary(Of String, Object)"/> as column/value
        /// </summary>
        /// <param name="Dic"></param>
        /// <param name="FilterKeys"></param>
        /// <returns></returns>
        public object Where(Dictionary<string, object> Dic, DbExtensions.LogicConcatenationOperator LogicConcatenation, params string[] FilterKeys)
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
                    if (LogicConcatenation == DbExtensions.LogicConcatenationOperator.OR)
                    {
                        Or(new Condition(f, Dic[f]));
                    }
                    else
                    {
                        And(new Condition(f, Dic[f]));
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built using a <see cref="NameValueCollection"/> as column/operator/value
        /// </summary>
        /// <param name="NVC"></param>
        /// <param name="FilterKeys"></param>
        /// <returns></returns>
        public Select<T> Where(NameValueCollection NVC, params string[] FilterKeys)
        {
            FilterKeys = FilterKeys ?? Array.Empty<string>();
            foreach (var k in NVC.AllKeys)
            {
                if (k.IsNotBlank())
                {
                    string col = k.UrlDecode();
                    if (!FilterKeys.Any() || col.IsLikeAny(FilterKeys))
                    {
                        var values = NVC.GetValues(k) ?? new string[] { };
                        foreach (var v in values)
                        {
                            string logic = col.GetBefore(":", true).IfBlank("AND");
                            string op = v.GetBefore(":", true).IfBlank("=");
                            col = col.GetAfter(":");
                            col = col.Contains(" ").AsIf(col.UnQuote("[", true).Quote('['), col);
                            string valor = v.GetAfter(":").NullIf("null", StringComparison.InvariantCultureIgnoreCase);
                            if (valor is null)
                            {
                                op = "is";
                            }

                            if (valor == "'null'")
                            {
                                valor = "null";
                            }

                            var cond = new Condition(col, valor, op);
                            Where(logic, new[] { cond });
                        }
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an AND clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> And(params FormattableString[] conditions)
        {
            return And((conditions ?? Array.Empty<FormattableString>()).Select(x => new Condition(x)).ToArray());
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an AND clause.
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
                        Where(condition);
                    else
                        _where.And(condition);
                }
            }

            return this;
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an AND clause.
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

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an AND clause.
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
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an AND clause.
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
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an AND clause.
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
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an OR clause.
        /// </summary>
        /// <param name="conditions">Condition to set</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Select<T> Or(params FormattableString[] conditions)
        {
            return Or((conditions ?? Array.Empty<FormattableString>()).Select(x => new Condition(x)).ToArray());
        }

        /// <summary>
        /// Sets the WHERE clause in the SELECT being built.
        /// If WHERE is already set, appends the condition with an OR clause.
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
                        Where(condition);
                    else
                        _where.Or(condition);
                }
            }

            return this;
        }

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

        public Select<T> OffSet(int Page, int PageSize)
        {
            if (Page < 0)
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

        /// <summary>
        /// Operator overload that allows using the class wherever a string is expected.
        /// </summary>
        public static implicit operator string(Select<T> select)
        {
            return select is null ? null : select.ToString();
        }

        /// <summary>
        /// Operator overload that allows using the class wherever a string is expected.
        /// </summary>
        public static implicit operator FormattableString(Select<T> select)
        {
            return (select is null ? null : select.ToString()).ToFormattableString();
        }

        public DbCommand CreateDbCommand(DbConnection Connection, Dictionary<string, object> dic)
        {
            return Connection.CreateCommand(ToString(), dic);
        }

        public DbCommand CreateDbCommand(DbConnection Connection)
        {
            return CreateDbCommand(Connection, null);
        }

        public Select<T> AndSearch(string Value, params string[] Columns)
        {
            return AndAny((Columns ?? Array.Empty<string>()).Select(x => (x + " LIKE {0}").ToFormattableString(Value.ToString().Wrap("%"))).ToArray());
        }

        public Select<T> OrSearch(string Value, params string[] Columns)
        {
            return OrAny((Columns ?? Array.Empty<string>()).Select(x => (x + " LIKE {0}").ToFormattableString(Value.ToString().Wrap("%"))).ToArray());
        }

        public Select<T> OrSearch(IEnumerable<string> Value, params string[] Columns)
        {
            foreach (var item in (Value ?? Array.Empty<string>()).Where(x => x.IsNotBlank()))
                OrSearch(item, Columns);
            return this;
        }

        public Select<T> AndSearch(IEnumerable<string> Value, params string[] Columns)
        {
            foreach (var item in (Value ?? Array.Empty<string>()).Where(x => x.IsNotBlank()))
                AndSearch(item, Columns);
            return this;
        }

        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Returns the SELECT statement as a SQL query.
        /// </summary>
        /// <returns>The SELECT statement as a SQL query</returns>
        public string ToString(bool AsSubquery)
        {
            var sql = new StringBuilder("SELECT ");
            sql.Append(string.Join(", ", _columns.Distinct().ToArray()).IfBlank(" * "));
            if (_fromsub != null && _fromsub.ToString().IsNotBlank())
            {
                _from = _fromsub.ToString(true).Quote('(') + " as " + _fromsubname;
            }

            if (_from.IsNotBlank())
            {
                sql.Append(" FROM ");
                sql.Append(_from);
            }

            if (_joins != null && _joins.Any())
            {
                sql.Append(_joins.SelectJoin(j => string.Format(CultureInfo.InvariantCulture, " {0}", j), " "));
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

            if (_offset.IsNotBlank() && AsSubquery == false)
            {
                sql.Append($" {_offset} ");
            }

            return sql.ToString();
        }

        private Select<T> _Join(JoinType type, string table, Condition on)
        {
            if (table.IsNotBlank() && !Information.IsNothing(on) && on.ToString().IsNotBlank())
            {
                _joins = _joins ?? new List<Join>();
                _joins.Add(new Join()
                {
                    Type = type,
                    Table = table,
                    On = on
                });
            }

            return this;
        }
    }

    /// <summary>
    /// A condition with optional AND and OR clauses that can be used in WHERE or JOIN ON statements.
    /// </summary>
    public class Condition
    {
        private readonly List<string> _tokens = new List<string>();

        public Condition(string LogicOperator, params FormattableString[] Conditions)
        {
            foreach (var condition in Conditions ?? Array.Empty<FormattableString>())
            {
                if (condition != null && condition.ToString().IsNotBlank())
                {
                    if (LogicOperator.IsIn("Or", "OR"))
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
                    if (LogicOperator.IsIn("Or", "OR"))
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

        public Condition(DbExtensions.LogicConcatenationOperator LogicOperator, params Condition[] Conditions) : this(LogicOperator.AsIf(x => x == DbExtensions.LogicConcatenationOperator.OR, "OR", "AND"), Conditions)
        {
        }

        public Condition(DbExtensions.LogicConcatenationOperator LogicOperator, params FormattableString[] Conditions) : this(LogicOperator.AsIf(x => x == DbExtensions.LogicConcatenationOperator.OR, "OR", "AND"), Conditions)
        {
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
        /// Select class constructor
        /// </summary>
        public Condition(string Column, object Value, string Operator = "=")
        {
            if (Column.IsNotBlank())
            {
                _tokens.Add($"{Column} {Operator.IfBlank("=")} {DbExtensions.ToSQLString(Value)}");
            }
        }

        public static Condition OrMany(params FormattableString[] conditions)
        {
            return new Condition("Or", conditions);
        }

        public static Condition AndMany(params FormattableString[] conditions)
        {
            return new Condition("And", conditions);
        }

        public Condition OrAll(params FormattableString[] Conditions)
        {
            return Or(AndMany(Conditions));
        }

        public Condition OrAny(params FormattableString[] Conditions)
        {
            return Or(OrMany(Conditions));
        }

        public Condition AndAll(params FormattableString[] Conditions)
        {
            return And(AndMany(Conditions));
        }

        public Condition AndAny(params FormattableString[] Conditions)
        {
            return And(OrMany(Conditions));
        }

        /// <summary>
        /// Appends the given condition with AND in this condition.
        /// </summary>
        /// <param name="condition">Condition to be appended</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Condition And(FormattableString condition)
        {
            if (!Information.IsNothing(condition) && condition.ToString().IsNotBlank())
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
        public Condition And(Condition condition)
        {
            return And(condition.ParenthesisToString().ToFormattableString());
        }

        /// <summary>
        /// Appends the given condition with OR in this condition.
        /// </summary>
        /// <param name="condition">Condition to be appended</param>
        /// <returns>This instance, so you can use it in a fluent fashion</returns>
        public Condition Or(FormattableString condition)
        {
            if (!Information.IsNothing(condition) && condition.ToString().IsNotBlank())
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
        public Condition Or(Condition condition)
        {
            return Or(condition.ParenthesisToString().ToFormattableString());
        }

        /// <summary>
        /// Returns the condition statement as a SQL query in parenthesis.
        /// </summary>
        /// <returns>The condition statement as a SQL query in parenthesis</returns>
        public string ParenthesisToString()
        {
            return ToString().Quote('(');
        }

        /// <summary>
        /// Returns the condition statement as a SQL query.
        /// </summary>
        /// <returns>The condition statement as a SQL query</returns>
        public override string ToString()
        {
            return string.Join(" ", _tokens).QuoteIf(_tokens.Count > 2, "(");
        }
    }

    internal class Join
    {
        internal JoinType Type { get; set; }
        internal string Table { get; set; }
        internal Condition On { get; set; }

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

        public override string ToString()
        {
            return On is null ? string.Format(CultureInfo.InvariantCulture, "{0} {1}", JoinString, Table) : string.Format(CultureInfo.InvariantCulture, "{0} {1} On {2}", JoinString, Table, On);
        }
    }

    internal enum JoinType
    {
        Join,
        Inner,
        LeftOuterJoin,
        RightOuterJoin,
        FullOuterJoin,
        CrossJoin,
        CrossApply
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class FromSQL : Attribute
    {
        public FromSQL(string QueryOrFilePath)
        {
            if (QueryOrFilePath.IsFilePath() && File.Exists(QueryOrFilePath))
            {
                SQL = File.ReadAllText(QueryOrFilePath);
            }
            else
            {
                SQL = QueryOrFilePath;
            }
        }

        public string SQL { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ColumnName : Attribute
    {
        public ColumnName(params string[] Name)
        {
            Names = (string[])(Name ?? Array.Empty<string>()).Select(x => x.UnQuote());
        }

        public string[] Names { get; private set; }
    }
}