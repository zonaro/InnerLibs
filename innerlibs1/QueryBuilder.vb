Imports System.Data.Common
Imports System.Globalization
Imports System.Text

Namespace QueryBuilder

    ''' <summary>
    ''' Class that aids building a SELECT clause.
    ''' </summary>
    Public Class [Select]
        Friend _columns As List(Of String)
        Friend _from As String
        Friend _joins As List(Of Join)
        Friend _where As Condition
        Friend _groupBy As List(Of String)
        Friend _having As String
        Friend _orderBy As List(Of String)
        Friend _offset As String
        Friend _desc As Boolean

        ''' <summary>
        ''' Class that aids building a SELECT clause.
        ''' </summary>
        ''' <param name="columns">Columns to be selected</param>
        Public Sub New(ParamArray columns As String())
            columns = If(columns, {})
            If _columns Is Nothing Then
                _columns = New List(Of String)(columns)
            Else
                _columns.AddRange(columns)
            End If
        End Sub

        ''' <summary>
        ''' Sets the FROM clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be selected from</param>
        ''' <returns></returns>
        Public Function From(ByVal table As String) As [Select]
            If table.IsNotBlank Then
                _from = table
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets a JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Join(ByVal table As String, ByVal [on] As FormattableString) As [Select]
            Return _Join(JoinType.Join, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a INNER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function InnerJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select]
            Return _Join(JoinType.Inner, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a INNER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function InnerJoin(ByVal table As String, ByVal [on] As Condition) As [Select]
            Return _Join(JoinType.Inner, table, [on])
        End Function

        ''' <summary>
        ''' Sets a JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Join(ByVal table As String, ByVal [on] As Condition) As [Select]
            Return _Join(JoinType.Join, table, [on])
        End Function

        ''' <summary>
        ''' Sets a LEFT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function LeftOuterJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select]
            Return _Join(JoinType.LeftOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a LEFT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function LeftOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select]
            Return _Join(JoinType.LeftOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function RightOuterJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select]
            Return _Join(JoinType.RightOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function RightOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select]
            Return _Join(JoinType.RightOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a FULL OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function FullOuterJoin(ByVal table As String, ByVal [on] As FormattableString) As [Select]
            Return _Join(JoinType.FullOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a FULL OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <param name="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function FullOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select]
            Return _Join(JoinType.FullOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a CROSS JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function CrossJoin(ByVal table As String) As [Select]
            Return _Join(JoinType.CrossJoin, table, Nothing)
        End Function

        ''' <summary>
        ''' Sets a CROSS JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <param name="table">Table to be join</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function CrossApply(ByVal table As String) As [Select]
            Return _Join(JoinType.CrossApply, table, Nothing)
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' </summary>
        ''' <param name="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(ByVal condition As FormattableString) As [Select]
            If condition.ToString().IsNotBlank Then
                If _where IsNot Nothing Then
                    [And](New Condition(condition))
                Else
                    _where = New Condition(condition)
                End If
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' </summary>
        ''' <param name="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(condition As Condition) As [Select]
            If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank() Then
                If _where IsNot Nothing Then [And](condition)
                _where = New Condition(condition)
            End If
            Return Me
        End Function

        Public Function Where(ParamArray condition As Condition()) As [Select]
            Return [And](condition)
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ParamArray conditions As FormattableString()) As [Select]
            Return [And](If(conditions, {}).Select(Function(x) New Condition(x)).ToArray())
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ParamArray ByVal conditions As Condition()) As [Select]
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
        Public Function [AndAny](ParamArray ByVal conditions As FormattableString()) As [Select]
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition(conditions.First())
                    _where.[AndAny](conditions.Skip(1).ToArray())
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
        Public Function [OrAny](ParamArray ByVal conditions As FormattableString()) As [Select]
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition(conditions.First())
                    _where.[OrAny](conditions.Skip(1).ToArray())
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
        Public Function AndAll(ParamArray ByVal conditions As FormattableString()) As [Select]
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition(conditions.First())
                    _where.AndAll(conditions.Skip(1).ToArray())
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
        Public Function OrAll(ParamArray ByVal conditions As FormattableString()) As [Select]
            conditions = If(conditions, {})
            If conditions.Any Then
                If _where Is Nothing Then
                    _where = New Condition(conditions.First())
                    _where.OrAll(conditions.Skip(1).ToArray())
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
        Public Function [Or](ParamArray conditions As FormattableString()) As [Select]
            Return [Or](If(conditions, {}).Select(Function(x) New Condition(x)).ToArray())
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an OR clause.
        ''' </summary>
        ''' <param name="conditions">Condition of the WHERE clause</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [Or](ParamArray conditions As Condition()) As [Select]
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
        Public Function GroupBy(ParamArray columns As String()) As [Select]
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
        Public Function Having(ByVal condition As String) As [Select]
            If condition.IsNotBlank Then
                _having = condition
            End If
            Return Me
        End Function

        Public Function OffSet(Page As Integer, PageSize As Integer) As [Select]
            _offset = $"OFFSET {Page} ROWS FETCH NEXT {PageSize} ROWS ONLY"
            Return Me
        End Function

        ''' <summary>
        ''' Sets the ORDER BY clause in the SELECT being built.
        ''' </summary>
        ''' <param name="columns">Columns to be ordered by</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function OrderBy(ParamArray columns As String()) As [Select]
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
        Public Shared Widening Operator CType(ByVal [select] As [Select]) As String
            Return If([select] Is Nothing, Nothing, [select].ToString())
        End Operator

        ''' <summary>
        ''' Returns the SELECT statement as a SQL query in parenthesis (subselect).
        ''' </summary>
        ''' <returns>The SELECT statement as a SQL query in parenthesis</returns>
        Public Function ParenthesisToString() As String
            Return ToString.Quote("'")
        End Function

        Public Function CreateDbCommand(Connection As DbConnection, dic As Dictionary(Of String, Object)) As DbCommand
            Return Connection.CreateCommand(ToString(), dic)
        End Function

        Public Function CreateDbCommand(Connection As DbConnection) As DbCommand
            Return CreateDbCommand(Connection, Nothing)
        End Function

        ''' <summary>
        ''' Returns the SELECT statement as a SQL query.
        ''' </summary>
        ''' <returns>The SELECT statement as a SQL query</returns>
        Public Overrides Function ToString() As String
            Dim sql = New StringBuilder("SELECT ")
            sql.Append(String.Join(", ", _columns).IfBlank(" * "))

            If Not Equals(_from, Nothing) Then
                sql.Append(" FROM ")
                sql.Append(_from)
            End If

            If _joins IsNot Nothing Then
                For Each j In _joins
                    sql.Append(String.Format(CultureInfo.InvariantCulture, " {0}", j))
                Next
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

        Private Function _Join(ByVal type As JoinType, ByVal table As String, ByVal [on] As Condition) As [Select]
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



End Namespace