Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Globalization
Imports System.Linq.Expressions
Imports System.Runtime.CompilerServices
Imports System.Text
Imports InnerLibs.LINQ

Namespace MicroORM

    Public Class [Select]
        Inherits [Select](Of Object)

        Sub New()
            MyBase.New
        End Sub

    End Class

    Public Class [Select](Of T As Class)

        Sub New()
            SetColumns(Of T)()
            From(Of T)()
        End Sub

        Sub New(obj As T)
            SetColumns(Of T)()
            From(Of T)()
            WhereObject(obj)
        End Sub

        ''' <summary>
        ''' Class that aids building a SELECT clause.
        ''' </summary>
        ''' <param name="columns">Columns to be selected</param>
        Public Sub New(ParamArray columns As String())
            SetColumns(columns)
            From(Of T)()
        End Sub

        Friend _columns As List(Of String)
        Friend _from As String
        Friend _fromsub As [Select](Of T)
        Friend _joins As List(Of Join)
        Friend _where As Condition
        Friend _groupBy As List(Of String)
        Friend _having As String
        Friend _orderBy As List(Of String)
        Friend _offset As String
        Friend _desc As Boolean

        Public Function SetColumns(Of O As Class)(Optional Obj As O = Nothing) As [Select](Of T)
            SetColumns(GetType(O).GetNullableTypeOf().GetProperties().Select(Function(x) x.Name).ToArray())
            Return Me
        End Function

        Public Function SetColumns(ParamArray Columns As String()) As [Select](Of T)
            Columns = If(Columns, {}).Distinct().Where(Function(x) x.IsNotBlank()).ToArray()
            _columns = New List(Of String)()
            _columns.AddRange(Columns)
            Return Me
        End Function

        Public Function AddColumns(ParamArray Columns As String()) As [Select](Of T)
            If _columns Is Nothing Then
                SetColumns(Columns)
            Else
                _columns.AddRange(If(Columns, {}).Distinct().ToArray())
            End If
            Return Me
        End Function

        Public Function AddColumns(Of O As Class)(Optional Obj As O = Nothing) As [Select](Of T)
            AddColumns(GetType(O).GetNullableTypeOf().GetProperties().Select(Function(x) x.Name).ToArray())
            Return Me
        End Function


        Public Function RemoveColumns(ParamArray Columns As String()) As [Select](Of T)
            If _columns IsNot Nothing Then
                Columns = If(Columns, {}).Where(Function(x) x.IsIn(_columns)).ToArray()
                Columns.ToList.ForEach(Sub(x) _columns.Remove(x))
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the FROM clause in the SELECT being built.
        ''' </summary>
        ''' <param name="TableOrSubQuery">Table to be selected from</param>
        ''' <returns></returns>
        Public Function From(ByVal TableOrSubQuery As String) As [Select](Of T)
            If TableOrSubQuery.IsNotBlank Then
                _from = TableOrSubQuery.QuoteIf(TableOrSubQuery.StartsWith("SELECT "), "(")
                _fromsub = Nothing
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the FROM clause in the SELECT being built.
        ''' </summary>
        ''' <param name="SubQuery">Subquery to be selected from</param>
        ''' <returns></returns>
        Public Function From(ByVal SubQuery As [Select](Of T)) As [Select](Of T)
            If SubQuery IsNot Nothing AndAlso SubQuery.ToString().IsNotBlank AndAlso SubQuery IsNot Me Then
                _from = Nothing
                _fromsub = SubQuery
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the FROM clause in the SELECT being built.
        ''' </summary>
        ''' <param name="SubQuery">Subquery to be selected from</param>
        ''' <returns></returns>
        Public Function From(ByVal SubQuery As Action(Of [Select])) As [Select](Of T)
            If SubQuery IsNot Nothing Then
                Dim sl = New [Select]()
                SubQuery(sl)
                Me.From(sl)
                If sl._columns Is Nothing OrElse Not sl._columns.Any() Then
                    sl.SetColumns(Me._columns)
                End If
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the FROM clause in the SELECT being built.
        ''' </summary>
        ''' <returns></returns>
        Public Function From(Of O)() As [Select](Of T)
            From(ClassTools.GetNullableTypeOf(GetType(O)).Name)
            Return Me
        End Function

        ''' <summary>
        ''' Sets a JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Join(ByVal table As String, ByVal [on] As FormattableString) As [Select](Of T)
            Return _Join(JoinType.Join, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a INNER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function InnerJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select](Of T)
            Return _Join(JoinType.Inner, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a INNER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function InnerJoin(ByVal table As String, ByVal [on] As Condition) As [Select](Of T)
            Return _Join(JoinType.Inner, table, [on])
        End Function

        ''' <summary>
        ''' Sets a JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Join(ByVal table As String, ByVal [on] As Condition) As [Select](Of T)
            Return _Join(JoinType.Join, table, [on])
        End Function

        ''' <summary>
        ''' Sets a LEFT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function LeftOuterJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select](Of T)
            Return _Join(JoinType.LeftOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a LEFT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function LeftOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select](Of T)
            Return _Join(JoinType.LeftOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function RightOuterJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select](Of T)
            Return _Join(JoinType.RightOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function RightOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select](Of T)
            Return _Join(JoinType.RightOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a FULL OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function FullOuterJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select](Of T)
            Return _Join(JoinType.FullOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a FULL OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function FullOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select](Of T)
            Return _Join(JoinType.FullOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a CROSS JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function CrossJoin(ByVal table As String) As [Select](Of T)
            Return _Join(JoinType.CrossJoin, table, Nothing)
        End Function

        ''' <summary>
        ''' Sets a CROSS JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function CrossApply(ByVal table As String) As [Select](Of T)
            Return _Join(JoinType.CrossApply, table, Nothing)
        End Function

        Public Function RunSQLValue(Of T)(Connection As DbConnection) As T
            Return Connection.RunSQLValue(Of T)(Me.CreateDbCommand(Connection))
        End Function

        Public Function RunSQLValue(Connection As DbConnection) As Object
            Return Connection.RunSQLValue(Me.CreateDbCommand(Connection))
        End Function

        Public Function RunSQLRow(Of O)(Connection As DbConnection) As O
            Return Connection.RunSQLRow(Of O)(Me.CreateDbCommand(Connection))
        End Function

        Public Function RunSQLRow(Connection As DbConnection) As T
            Return Connection.RunSQLRow(Of T)(Me.CreateDbCommand(Connection))
        End Function

        Public Function RunSQLRowDictionary(Connection As DbConnection) As Dictionary(Of String, Object)
            Return Connection.RunSQLRow(Me.CreateDbCommand(Connection))
        End Function

        Public Function RunSQLSet(Of O)(Connection As DbConnection) As IEnumerable(Of T)
            Return Connection.RunSQLSet(Of O)(Me.CreateDbCommand(Connection))
        End Function

        Public Function RunSQLSet(Connection As DbConnection) As IEnumerable(Of T)
            Return Connection.RunSQLSet(Of T)(Me.CreateDbCommand(Connection))
        End Function

        Public Function RunSQLSetDictionary(Connection As DbConnection) As IEnumerable(Of Dictionary(Of String, Object))
            Return Connection.RunSQLSet(Me.CreateDbCommand(Connection))
        End Function

        Public Function WhereObject(Of O As Class)(Obj As O) As [Select](Of T)
            Return WhereObject(Obj, "AND")
        End Function

        Public Function WhereObject(Of O As Class)(Obj As O, Optional LogicOperator As String = "AND") As [Select](Of T)
            If Obj IsNot Nothing Then
                Where(New Condition(LogicOperator, Obj.GetNullableTypeOf().GetProperties().Where(Function(x) x.CanRead).Select(Function(x) (x.Name & " = {0}").ToFormattableString(x.GetValue(Obj))).ToArray()))
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' </summary>
        ''' <param name="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(ByVal condition As FormattableString) As [Select](Of T)
            If condition.ToString().IsNotBlank Then
                If _where IsNot Nothing Then
                    [And](New Condition(condition))
                Else
                    _where = New Condition(condition)
                End If
            End If
            Return Me
        End Function

        Public Function Where(predicate As Expression(Of Func(Of T, Boolean))) As [Select](Of T)
            If predicate IsNot Nothing Then
                Dim p = New StringBuilder(predicate.Body.ToString())
                Dim pName = predicate.Parameters.First()
                p.Replace(pName.Name + ".", "")
                p.Replace("==", "=")
                p.Replace("!=", "<>")
                p.Replace("AndAlso", "AND")
                p.Replace("OrElse", "OR")
                p.Replace(" like ", " LIKE ")
                p.Replace(".Contains", " LIKE ")
                p.Replace(".Like", " LIKE ")
                p.Replace(".Equal", " = ")
                p.Replace("""", "'")
                Me.Where(p.ToString().ToFormattableString())
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' </summary>
        ''' <param name="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(condition As Condition) As [Select](Of T)
            If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank() Then
                If _where IsNot Nothing Then [And](condition)
                _where = New Condition(condition)
            End If
            Return Me
        End Function

        Public Function Where(ParamArray condition As Condition()) As [Select](Of T)
            Return [And](condition)
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ParamArray conditions As FormattableString()) As [Select](Of T)
            Return [And](If(conditions, {}).Select(Function(x) New Condition(x)).ToArray())
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ParamArray ByVal conditions As Condition()) As [Select](Of T)
            For Each condition In If(conditions, {})
                If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank Then
                    If _where Is Nothing Then Where(condition) Else _where.And(condition)
                End If
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [AndAny](ParamArray ByVal conditions As FormattableString()) As [Select](Of T)
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition("OR", conditions)
                Else
                    _where.[AndAny](conditions)
                End If
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [OrAny](ParamArray ByVal conditions As FormattableString()) As [Select](Of T)
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition("OR", conditions)
                Else
                    _where.[OrAny](conditions)
                End If
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function AndAll(ParamArray ByVal conditions As FormattableString()) As [Select](Of T)
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition("AND", conditions)
                Else
                    _where.AndAll(conditions)
                End If
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function OrAll(ParamArray ByVal conditions As FormattableString()) As [Select](Of T)
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition("AND", conditions)
                Else
                    _where.OrAll(conditions)
                End If
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an OR clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [Or](ParamArray conditions As FormattableString()) As [Select](Of T)
            Return [Or](If(conditions, {}).Select(Function(x) New Condition(x)).ToArray())
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an OR clause.
        ''' </summary>
        ''' <param name="conditions">Condition of the WHERE clause</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [Or](ParamArray conditions As Condition()) As [Select](Of T)
            For Each condition In If(conditions, {})
                If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank Then
                    If _where Is Nothing Then Where(condition) Else _where.Or(condition)
                End If
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Sets the GROUP BY clause in the SELECT being built.
        ''' </summary>
        ''' <param name="columns">Columns to be grouped by</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function GroupBy(ParamArray columns As String()) As [Select](Of T)
            columns = If(columns, {})
            If _groupBy Is Nothing Then
                _groupBy = New List(Of String)(columns)
            Else
                _groupBy.AddRange(columns)
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets or overwrite the HAVING clause in the SELECT being built.
        ''' </summary>
        ''' <param name="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Having(ByVal condition As String) As [Select](Of T)
            If condition.IsNotBlank Then
                _having = condition
            End If
            Return Me
        End Function

        Public Function OffSet(Page As Integer, PageSize As Integer) As [Select](Of T)
            _offset = $"OFFSET {Page} ROWS FETCH NEXT {PageSize} ROWS ONLY"
            Return Me
        End Function

        ''' <summary>
        ''' Sets the ORDER BY clause in the SELECT being built.
        ''' </summary>
        ''' <param name="columns">Columns to be ordered by</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function OrderBy(ParamArray columns As String()) As [Select](Of T)
            columns = If(columns, {})
            If _orderBy Is Nothing Then
                _orderBy = New List(Of String)(columns)
            Else
                _orderBy.AddRange(columns)
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Operator overload that allows using the class wherever a string is expected.
        ''' </summary>
        Public Shared Widening Operator CType(ByVal [select] As [Select](Of T)) As String
            Return If([select] Is Nothing, Nothing, [select].ToString())
        End Operator

        Public Function CreateDbCommand(Connection As DbConnection, dic As Dictionary(Of String, Object)) As DbCommand
            Return Connection.CreateCommand(ToString(), dic)
        End Function

        Public Function CreateDbCommand(Connection As DbConnection) As DbCommand
            Return CreateDbCommand(Connection, Nothing)
        End Function

        Public Function AndSearch(Value As String, ParamArray Columns As String())
            Return Me.AndAny(If(Columns, {}).Select(Function(x) (x & " LIKE {0}").ToFormattableString(Value.ToString().Wrap("%"))).ToArray())
        End Function

        Public Function OrSearch(Value As String, ParamArray Columns As String())
            Return Me.OrAny(If(Columns, {}).Select(Function(x) (x & " LIKE {0}").ToFormattableString(Value.ToString().Wrap("%"))).ToArray())
        End Function

        Public Function OrSearch(Value As IEnumerable(Of String), ParamArray Columns As String())
            For Each item In If(Value, {}).Where(Function(x) x.IsNotBlank)
                Me.OrSearch(item, Columns)
            Next
            Return Me
        End Function

        Public Function AndSearch(Value As IEnumerable(Of String), ParamArray Columns As String())
            For Each item In If(Value, {}).Where(Function(x) x.IsNotBlank)
                Me.AndSearch(item, Columns)
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Returns the SELECT statement as a SQL query.
        ''' </summary>
        ''' <returns>The SELECT statement as a SQL query</returns>
        Public Overrides Function ToString() As String
            Dim sql = New StringBuilder("SELECT ")
            sql.Append(String.Join(", ", _columns.Distinct().ToArray()).IfBlank(" * "))

            If _fromsub IsNot Nothing AndAlso _fromsub.ToString().IsNotBlank Then
                _from = _fromsub.ToString().Quote("(")
            End If

            If _from.IsNotBlank Then
                sql.Append(" FROM ")
                sql.Append(_from)
            End If

            If _joins IsNot Nothing AndAlso _joins.Any() Then
                sql.Append(_joins.Select(Function(j) String.Format(CultureInfo.InvariantCulture, " {0}", j)))
            End If

            If _where IsNot Nothing Then
                sql.Append(" WHERE ")
                sql.Append(_where)
            End If

            If _groupBy IsNot Nothing Then
                sql.Append(" GROUP BY ")
                sql.Append(String.Join(", ", _groupBy))
            End If

            If Not Equals(_having, Nothing) Then
                sql.Append(" HAVING ")
                sql.Append(_having)
            End If

            If _orderBy IsNot Nothing Then

                sql.Append(" ORDER BY ")
                sql.Append(String.Join(", ", _orderBy))

            End If

            If _offset.IsNotBlank Then
                sql.Append($" {_offset} ")

            End If

            Return sql.ToString()
        End Function

        Private Function _Join(ByVal type As JoinType, ByVal table As String, ByVal [on] As Condition) As [Select](Of T)
            If table.IsNotBlank AndAlso Not IsNothing([on]) AndAlso [on].ToString.IsNotBlank Then
                _joins = If(_joins, New List(Of [Join]))
                _joins.Add(New Join With {
                    .Type = type,
                    .Table = table,
                    .[On] = [on]
                })
            End If
            Return Me
        End Function

    End Class

    ''' <summary>
    ''' A condition with optional AND and OR clauses that can be used in WHERE or JOIN ON statements.
    ''' </summary>
    Public Class Condition
        Private ReadOnly _tokens As List(Of String) = New List(Of String)()

        Public Sub New(LogicOperator As String, ParamArray Conditions As FormattableString())
            For Each condition In If(Conditions, {})
                If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank Then
                    If LogicOperator.IsIn("OR", "or") Then
                        [Or](condition)
                    Else
                        [And](condition)
                    End If
                End If
            Next
        End Sub

        Public Sub New(LogicOperator As String, ParamArray Conditions As Condition())
            For Each condition In If(Conditions, {})
                If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank Then
                    If LogicOperator.IsIn("OR", "or") Then
                        [Or](condition)
                    Else
                        [And](condition)
                    End If
                End If
            Next
        End Sub

        ''' <summary>
        ''' Ctor.
        ''' </summary>
        ''' <param name="condition">Condition to set in this instance</param>
        Public Sub New(ByVal condition As FormattableString)
            If condition.IsNotBlank Then
                _tokens.Add(ToSQLString(condition))
            End If
        End Sub

        ''' <summary>
        ''' Ctor.
        ''' </summary>
        ''' <param name="condition">Copies to the condition being constructed</param>
        Public Sub New(condition As Condition)
            If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank Then
                _tokens.Add(condition.ParenthesisToString())
            End If
        End Sub

        Public Shared Function OrMany(ParamArray conditions As FormattableString()) As Condition
            Return New Condition("OR", conditions)
        End Function

        Public Shared Function AndMany(ParamArray conditions As FormattableString()) As Condition
            Return New Condition("AND", conditions)
        End Function

        Public Function OrAll(ParamArray Conditions As FormattableString()) As Condition
            Return [Or](AndMany(Conditions))
        End Function

        Public Function OrAny(ParamArray Conditions As FormattableString()) As Condition
            Return [Or](OrMany(Conditions))
        End Function

        Public Function AndAll(ParamArray Conditions As FormattableString()) As Condition
            Return [And](AndMany(Conditions))
        End Function

        Public Function AndAny(ParamArray Conditions As FormattableString()) As Condition
            Return [And](OrMany(Conditions))
        End Function

        ''' <summary>
        ''' Appends the given condition with AND in this condition.
        ''' </summary>
        ''' <param name="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ByVal condition As FormattableString) As Condition
            If Not IsNothing(condition) AndAlso condition.ToString.IsNotBlank() Then
                If _tokens.Any Then
                    _tokens.Add("AND")
                End If
                _tokens.Add(ToSQLString(condition))
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Appends the given condition with AND in this condition.
        ''' </summary>
        ''' <param name="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ByVal condition As Condition) As Condition
            Return [And](condition.ParenthesisToString().ToFormattableString())
        End Function

        ''' <summary>
        ''' Appends the given condition with OR in this condition.
        ''' </summary>
        ''' <param name="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [Or](ByVal condition As FormattableString) As Condition
            If Not IsNothing(condition) AndAlso condition.ToString.IsNotBlank() Then
                If _tokens.Any Then
                    _tokens.Add("OR")
                End If
                _tokens.Add(condition.ToSQLString())
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Appends the given condition with OR in this condition.
        ''' </summary>
        ''' <param name="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [Or](ByVal condition As Condition) As Condition
            Return [Or](condition.ParenthesisToString().ToFormattableString)
        End Function

        ''' <summary>
        ''' Returns the condition statement as a SQL query in parenthesis.
        ''' </summary>
        ''' <returns>The condition statement as a SQL query in parenthesis</returns>
        Public Function ParenthesisToString() As String
            Return ToString().Quote("(")
        End Function

        ''' <summary>
        ''' Returns the condition statement as a SQL query.
        ''' </summary>
        ''' <returns>The condition statement as a SQL query</returns>
        Public Overrides Function ToString() As String
            Return String.Join(" ", _tokens).QuoteIf(_tokens.Count() > 2, "(")
        End Function

    End Class

    Friend Class Join
        Friend Property Type As JoinType
        Friend Property Table As String
        Friend Property [On] As Condition

        Private ReadOnly Property JoinString As String
            Get

                Select Case Type
                    Case JoinType.Inner
                        Return "INNER JOIN"
                    Case JoinType.LeftOuterJoin
                        Return "LEFT OUTER JOIN"
                    Case JoinType.RightOuterJoin
                        Return "RIGHT OUTER JOIN"
                    Case JoinType.FullOuterJoin
                        Return "FULL OUTER JOIN"
                    Case JoinType.CrossJoin
                        Return "CROSS JOIN"
                    Case JoinType.CrossApply
                        Return "CROSS APPLY"
                    Case Else
                        Return "JOIN"
                End Select
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return If([On] Is Nothing, String.Format(CultureInfo.InvariantCulture, "{0} {1}", JoinString, Table), String.Format(CultureInfo.InvariantCulture, "{0} {1} ON {2}", JoinString, Table, [On]))
        End Function

    End Class

    Friend Enum JoinType
        Join
        Inner
        LeftOuterJoin
        RightOuterJoin
        FullOuterJoin
        CrossJoin
        CrossApply
    End Enum

    Public Module DbExtensions

        Private typeMap As Dictionary(Of Type, DbType)

        Public ReadOnly Property DbTypes As Dictionary(Of Type, DbType)
            Get
                If typeMap Is Nothing Then
                    typeMap = New Dictionary(Of Type, DbType)()
                    typeMap(GetType(Byte)) = DbType.Byte
                    typeMap(GetType(SByte)) = DbType.SByte
                    typeMap(GetType(Short)) = DbType.Int16
                    typeMap(GetType(UShort)) = DbType.UInt16
                    typeMap(GetType(Integer)) = DbType.Int32
                    typeMap(GetType(UInteger)) = DbType.UInt32
                    typeMap(GetType(Long)) = DbType.Int64
                    typeMap(GetType(ULong)) = DbType.UInt64
                    typeMap(GetType(Single)) = DbType.Single
                    typeMap(GetType(Double)) = DbType.Double
                    typeMap(GetType(Decimal)) = DbType.Decimal
                    typeMap(GetType(Boolean)) = DbType.Boolean
                    typeMap(GetType(String)) = DbType.String
                    typeMap(GetType(Char)) = DbType.StringFixedLength
                    typeMap(GetType(Guid)) = DbType.Guid
                    typeMap(GetType(DateTime)) = DbType.DateTime
                    typeMap(GetType(DateTimeOffset)) = DbType.DateTimeOffset
                    typeMap(GetType(Byte())) = DbType.Binary
                    typeMap(GetType(Data.Linq.Binary)) = DbType.Binary
                End If
                Return typeMap
            End Get
        End Property

        <Extension> Public Function GetDbType(Of T)(obj As T) As DbType
            Return DbTypes.GetValueOr(GetNullableTypeOf(Of T)(obj), DbType.Object)
        End Function

        <Extension> Public Function GetTypeFromDb(Of T)(obj As DbType) As Type
            Dim tt = DbTypes.FirstOrDefault(Function(x) x.Value = obj)
            If Not IsNothing(tt) Then
                Return tt.Key
            End If
            Return GetType(Object)
        End Function

        <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As String, Parameters As NameValueCollection) As DbCommand
            Return CreateCommand(Connection, SQL, Parameters.ToDictionary())
        End Function

        <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As String, Parameters As Dictionary(Of String, Object)) As DbCommand
            If Connection IsNot Nothing Then
                Dim command = Connection.CreateCommand()
                command.CommandText = SQL
                If Parameters IsNot Nothing AndAlso Parameters.Any() Then
                    For Each p In Parameters.Keys
                        Dim v = Parameters.GetValueOr(p)
                        Dim arr = ForceArray(v)
                        For index = 0 To arr.Length - 1
                            Dim param As DbParameter = command.CreateParameter()
                            If arr.Count() = 1 Then
                                param.ParameterName = $"__{p}"
                            Else
                                param.ParameterName = $"__{p}_{index}"
                            End If
                            param.Value = If(arr(index), DBNull.Value)
                            command.Parameters.Add(param)
                        Next
                    Next
                End If
                Return command
            End If
            Return Nothing
        End Function

        <Extension()> Public Function CreateCommand(Connection As DbConnection, SQL As FormattableString) As DbCommand
            If SQL IsNot Nothing AndAlso Connection IsNot Nothing Then
                Dim cmd = Connection.CreateCommand()
                If SQL.ArgumentCount > 0 Then
                    cmd.CommandText = SQL.Format
                    For index = 0 To SQL.ArgumentCount - 1
                        Dim valores = SQL.GetArgument(index)
                        Dim v = ForceArray(valores)
                        Dim param_names As New List(Of String)
                        For v_index = 0 To v.Count - 1
                            Dim param = cmd.CreateParameter()
                            If v.Count() = 1 Then
                                param.ParameterName = $"__p{index}"
                            Else
                                param.ParameterName = $"__p{index}_{v_index}"
                            End If
                            param.Value = If(v(v_index), DBNull.Value)
                            cmd.Parameters.Add(param)
                            param_names.Add("@" & param.ParameterName)
                        Next
                        cmd.CommandText = cmd.CommandText.Replace("{" & index & "}", param_names.Join(",").IfBlank("NULL").QuoteIf(param_names.Any(), "("))
                    Next
                Else
                    cmd.CommandText = SQL.ToString()
                End If
                Return cmd
            End If
            Return Nothing
        End Function

        <Extension()> Public Function ToSQLString(SQL As FormattableString) As String
            If SQL IsNot Nothing Then
                If SQL.ArgumentCount > 0 Then
                    Dim CommandText = SQL.Format
                    For index = 0 To SQL.ArgumentCount - 1
                        Dim valores = SQL.GetArgument(index)
                        Dim v = ForceArray(valores)
                        Dim paramvalues As New List(Of Object)
                        For v_index = 0 To v.Count - 1
                            paramvalues.Add(v(v_index))
                        Next
                        Dim pv = paramvalues.Select(Function(x)
                                                        If x Is Nothing Then
                                                            Return "NULL"
                                                        End If
                                                        If GetNullableTypeOf(x).IsNumericType OrElse x.ToString().IsNumber() Then
                                                            Return x.ToString()
                                                        End If
                                                        If IsDate(x) Then
                                                            Return CType(x, Date).ToSQLDateString().Quote("'")
                                                        End If
                                                        If IsBoolean(x) Then
                                                            Return CType(x, Boolean).AsIf(1, 0).ToString()
                                                        End If
                                                        Return x.ToString().Quote("'")
                                                    End Function).ToList()
                        CommandText = CommandText.Replace("{" & index & "}", pv.Join(",").IfBlank("NULL").QuoteIf(paramvalues.Count > 1, "("))
                    Next
                    Return CommandText
                Else
                    Return SQL.ToString()
                End If
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar uma procedure especifica e trata valores espicificos de
        ''' um NameValueCollection como parametros da procedure
        ''' </summary>
        ''' <param name="NVC">Objeto</param>
        ''' <param name="ProcedureName">  Nome da Procedure</param>
        ''' <param name="Keys">Valores do nameValueCollection o que devem ser utilizados</param>
        ''' <returns>Um DbCommand parametrizado</returns>

        <Extension()>
        Public Function ToProcedure(Connection As DbConnection, ByVal ProcedureName As String, NVC As NameValueCollection, ParamArray Keys() As String) As DbCommand
            Return Connection.ToProcedure(ProcedureName, NVC.ToDictionary(Keys), Keys)
        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar uma procedure especifica e trata propriedades espicificas de
        ''' um objeto como parametros da procedure
        ''' </summary>
        ''' <param name="Obj">Objeto</param>
        ''' <param name="ProcedureName">  Nome da Procedure</param>
        ''' <param name="Keys">propriedades do objeto que devem ser utilizados</param>
        ''' <returns>Um DbCommand parametrizado</returns>
        <Extension()>
        Public Function ToProcedure(Of T)(Connection As DbConnection, ByVal ProcedureName As String, Obj As T, ParamArray Keys() As String) As DbCommand
            Return Connection.ToProcedure(ProcedureName, Obj.CreateDictionary(), Keys)
        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar uma procedure especifica e utiliza os pares de um dicionario como parametros da procedure
        ''' </summary>
        ''' <param name="Dic">Dicionario com os parametros</param>
        ''' <param name="ProcedureName">  Nome da Procedure</param>
        ''' <param name="Keys">CHaves de Dicionário que devem ser utilizadas</param>
        ''' <returns>Um DbCommand parametrizado</returns>

        <Extension()>
        Public Function ToProcedure(Connection As DbConnection, ByVal ProcedureName As String, Dic As Dictionary(Of String, Object), ParamArray Keys() As String) As DbCommand
            Keys = If(Keys, {})
            If Not Keys.Any Then
                Keys = Dic.Keys.ToArray()
            Else
                Keys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(Keys)).ToArray
            End If

            Dim sql = "EXEC " & ProcedureName & " " & Keys.SelectJoin(Function(key) " @" & key & " = " & "@__" & key, ", ")

            Return Connection.CreateCommand(sql, Dic.ToDictionary(Function(x) x.Key, Function(x) x.Value))

        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
        ''' uma URL como as colunas da tabela de destino
        ''' </summary>
        ''' <param name="Request">        Requisicao HTTP</param>
        ''' <param name="TableName">  Nome da tabela</param>
        ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
        ''' <returns>Uma string com o comando montado</returns>
        <Extension()> Public Function ToUPDATE(Request As NameValueCollection, ByVal TableName As String, WhereClausule As String, ParamArray Keys As String()) As String
            Keys = If(Keys, {})
            If Keys.Count = 0 Then
                Keys = Request.AllKeys
            Else
                Keys = Request.AllKeys.Where(Function(x) x.IsLikeAny(Keys)).ToArray
            End If
            Dim cmd As String = "UPDATE " & TableName & Environment.NewLine & " set "
            For Each col In Keys
                cmd &= (String.Format(" {0} = {1},", col, UrlDecode(Request(col)).Wrap("'")) & Environment.NewLine)
            Next
            cmd = cmd.TrimAny(Environment.NewLine, " ", ",") & If(WhereClausule.IsNotBlank, " WHERE " & WhereClausule.TrimAny(" ", "where", "WHERE"), "")
            Debug.WriteLine(cmd.Wrap(Environment.NewLine))
            Return cmd
        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="Dictionary(Of String, Object)"/>
        ''' </summary>
        ''' <param name="Dic">        Dicionario</param>
        ''' <param name="TableName">  Nome da Tabela</param>
        ''' <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
        ''' <returns>Uma string com o comando montado</returns>

        <Extension()>
        Public Function ToSQLFilter(Dic As IDictionary(Of String, Object), ByVal TableName As String, CommaSeparatedColumns As String, LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys() As String) As String
            Dim CMD = "SELECT " & CommaSeparatedColumns.IfBlank("*") & " FROM " & TableName
            FilterKeys = If(FilterKeys, {})

            If FilterKeys.Count = 0 Then
                FilterKeys = Dic.Keys.ToArray()
            Else
                FilterKeys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(FilterKeys)).ToArray
            End If

            FilterKeys = FilterKeys.Where(Function(x) Dic(x) IsNot Nothing AndAlso Dic(x).ToString().IsNotBlank()).ToArray()

            If FilterKeys.Count > 0 Then
                CMD = CMD & " WHERE " & FilterKeys.Select(Function(key) " " & key & "=" & UrlDecode("" & Dic(key)).Wrap("'")).ToArray.Join(" " & [Enum].GetName(GetType(LogicConcatenationOperator), LogicConcatenation) & " ")
            End If
            Return CMD
        End Function

        ''' <summary>
        ''' Monta um Comando SQL para executar um SELECT com filtros a partir de um <see cref="NameValueCollection"/>
        ''' </summary>
        ''' <param name="NVC">        Colecao</param>
        ''' <param name="TableName">  Nome da Tabela</param>
        ''' <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
        ''' <returns>Uma string com o comando montado</returns>
        <Extension()>
        Public Function ToSQLFilter(NVC As NameValueCollection, ByVal TableName As String, CommaSeparatedColumns As String, LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys() As String) As String
            Return NVC.ToDictionary.ToSQLFilter(TableName, CommaSeparatedColumns, LogicConcatenation, FilterKeys)
        End Function

        Public Enum LogicConcatenationOperator
            [AND]
            [OR]
        End Enum

        ''' <summary>
        ''' Monta um Comando SQL para executar um INSERT e trata parametros espicificos de
        ''' uma URL como as colunas da tabela de destino
        ''' </summary>
        ''' <param name="Request">        Requisicao HTTP</param>
        ''' <param name="TableName">  Nome da tabela</param>
        ''' <param name="Keys">Parametros da URL que devem ser utilizados</param>
        ''' <returns>Uma string com o comando montado</returns>
        <Extension()> Public Function ToINSERT(Request As NameValueCollection, ByVal TableName As String, ParamArray Keys As String()) As String
            Keys = If(Keys, {})
            If Keys.Count = 0 Then
                Keys = Request.AllKeys
            Else
                Keys = Request.AllKeys.Where(Function(x) x.IsLikeAny(Keys)).ToArray
            End If
            Dim s = String.Format("INSERT INTO " & TableName & " ({0}) values ({1})", Keys.Join(","), Keys.Select(Function(p) Request(p).UrlDecode.Wrap("'")).ToArray.Join(","))
            Debug.WriteLine(s.Wrap(Environment.NewLine))
            Return s
        End Function

        <Extension()> Public Function CreateINSERTCommand(Of T As Class)(Connection As DbConnection, obj As T) As DbCommand
            Dim d = GetType(T)
            Dim dic As New Dictionary(Of String, Object)
            If obj IsNot Nothing AndAlso Connection IsNot Nothing Then
                If obj.IsDictionary Then
                    dic = CType(CType(obj, Object), Dictionary(Of String, Object))
                ElseIf obj.GetType() Is GetType(NameValueCollection) Then
                    dic = CType(CType(obj, Object), NameValueCollection).ToDictionary()
                Else
                    dic = obj.CreateDictionary()
                End If
                Dim cmd = Connection.CreateCommand()
                cmd.CommandText = String.Format($"INSERT INTO " & d.Name & " ({0}) values ({1})", dic.Keys.Join(","), dic.Keys.SelectJoin(Function(x) $"@__{x}", ","))
                For Each k In dic.Keys
                    Dim param = cmd.CreateParameter()
                    param.ParameterName = $"__{k}"
                    param.Value = dic.GetValueOr(k, DBNull.Value)
                    cmd.Parameters.Add(param)
                Next
                Return cmd
            End If
            Return Nothing
        End Function

        <Extension()> Public Function RunSQLValue(Connection As DbConnection, SQL As DbCommand) As Object
            Dim v = RunSQLRow(Connection, SQL)
            If v IsNot Nothing AndAlso v.Any() Then
                Return v.First().Value
            End If
            Return Nothing
        End Function

        <Extension()> Public Function RunSQLValue(Of V)(Connection As DbConnection, SQL As DbCommand) As V
            Return ChangeType(Of V)(RunSQLValue(Connection, SQL))
        End Function

        <Extension()> Public Function RunSQLValue(Connection As DbConnection, SQL As FormattableString) As Object
            Dim v = RunSQLRow(Connection, SQL).FirstOrDefault()
            If Not IsNothing(v) Then
                Return v.Value
            End If
            Return Nothing
        End Function

        <Extension()> Public Function RunSQLValue(Of V)(Connection As DbConnection, SQL As FormattableString) As V
            Return ChangeType(Of V)(RunSQLValue(Connection, SQL))
        End Function

        <Extension()> Public Function RunSQLSet(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of Dictionary(Of String, Object))
            Return RunSQLSet(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        <Extension()> Public Function RunSQLRow(Connection As DbConnection, SQL As FormattableString) As Dictionary(Of String, Object)
            Return RunSQLRow(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        <Extension()> Public Function RunSQLSet(Connection As DbConnection, SQL As DbCommand) As IEnumerable(Of Dictionary(Of String, Object))
            Return RunSQLSet(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        <Extension()> Public Function RunSQLRow(Connection As DbConnection, SQL As DbCommand) As Dictionary(Of String, Object)
            Return RunSQLRow(Of Dictionary(Of String, Object))(Connection, SQL)
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLRow(Of T)(Connection As DbConnection, SQL As DbCommand) As T
            Return Connection.RunSQLSet(Of T)(SQL).FirstOrDefault()
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para uma classe POCO do tipo <see cref="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLRow(Of T)(Connection As DbConnection, SQL As FormattableString) As T
            Return Connection.RunSQLSet(Of T)(SQL).FirstOrDefault()
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLSet(Of T)(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of T)
            Return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(Function(x) x.SetPropertiesIn(Of T))
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset mapeados para uma lista de classe POCO do tipo <see cref="T"/>
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLSet(Of T)(Connection As DbConnection, SQL As DbCommand) As IEnumerable(Of T)
            Return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(Function(x) x.SetPropertiesIn(Of T))
        End Function

        ''' <summary>
        ''' Executa uma query SQL parametrizada e retorna os resultados mapeados em listas de <see cref="Dictionary(Of String, Object)"/>
        ''' </summary>
        ''' <param name="Connection"></param>
        ''' <param name="SQL"></param>
        ''' <returns></returns>
        <Extension()> Public Function RunSQLMany(Connection As DbConnection, SQL As FormattableString) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
            Return Connection.RunSQLMany(Connection.CreateCommand(SQL))
        End Function

        <Extension()> Public Function RunSQLMany(Connection As DbConnection, Command As DbCommand) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
            Dim resposta As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
            If Connection IsNot Nothing AndAlso Command IsNot Nothing Then
                If Not Connection.State = ConnectionState.Open Then
                    Connection.Open()
                End If
                For Each item As DbParameter In Command.Parameters
                    Debug.WriteLine(item.Value, $"Parameter {item.ParameterName}".ToString())
                Next
                Debug.WriteLine(Command.CommandText, "SQL Command")
                Using reader = Command.ExecuteReader()
                    resposta = reader.MapMany()
                End Using
                Return resposta
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Mapeia os objetos de um datareader para uma classe
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="Reader"></param>
        ''' <returns></returns>
        <Extension()> Public Function Map(Of T As Class)(Reader As DbDataReader, ParamArray args As Object()) As List(Of T)
            Dim l = New List(Of T)
            args = If(args, {})
            While Reader IsNot Nothing AndAlso Reader.Read
                Dim d
                If args.Any Then
                    d = Activator.CreateInstance(GetType(T), args)
                Else
                    d = Activator.CreateInstance(Of T)
                End If
                For i As Integer = 0 To Reader.FieldCount - 1
                    Dim name = Reader.GetName(i)
                    Dim value = Reader.GetValue(i)
                    If GetType(T) = GetType(Dictionary(Of String, Object)) Then
                        CType(CType(d, Object), Dictionary(Of String, Object)).Set(name, value)
                    ElseIf GetType(T) = GetType(NameValueCollection) Then
                        CType(CType(d, Object), NameValueCollection).Add(name, value)
                    Else
                        If d.HasProperty(name) AndAlso d.GetProperty(name).CanWrite Then
                            d.SetPropertyValue(name, value)
                        End If
                    End If
                Next
                l.Add(d)
            End While
            Return l
        End Function

        <Extension()> Public Function MapMany(Reader As DbDataReader) As IEnumerable(Of IEnumerable(Of Dictionary(Of String, Object)))
            Dim l As New List(Of IEnumerable(Of Dictionary(Of String, Object)))
            Do
                l.Add(Reader.Map(Of Dictionary(Of String, Object))())
            Loop While Reader IsNot Nothing AndAlso Reader.NextResult()
            Return l
        End Function

        <Extension()> Public Function MapFirst(Of T As Class)(Reader As DbDataReader, ParamArray args As Object()) As T
            Return Reader.Map(Of T)(args).FirstOrDefault()
        End Function

    End Module

End Namespace