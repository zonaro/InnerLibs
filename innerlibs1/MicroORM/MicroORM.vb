Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Globalization
Imports System.IO
Imports System.Linq.Expressions
Imports System.Text
Imports InnerLibs.LINQ

Namespace MicroORM

    Public Interface ISelect

        Overloads Function ToString() As String

        Overloads Function ToString(SubQuery As Boolean) As String

    End Interface

    Public Class [Select]
        Inherits [Select](Of Dictionary(Of String, Object))

        Public Sub New()
            MyBase.New
        End Sub

        Public Sub New(ParamArray columns As String())
            MyBase.New(columns)
        End Sub

        Public Sub New(Obj As Dictionary(Of String, Object))
            MyBase.New(Obj)
        End Sub

    End Class

    Public Class [Select](Of T As Class)
        Implements ISelect

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

        Public Function ColumnQuote(QuoteChar As Char) As [Select](Of T)
            Dim _nova = New List(Of String)
            For Each item In If(_columns, New List(Of String))
                _nova.Add(item.UnQuote().Split(".", StringSplitOptions.RemoveEmptyEntries).SelectJoin(Function(x) x.UnQuote().Quote(QuoteChar), "."))
            Next
            SetColumns(_nova.ToArray())
            Return Me
        End Function

        Friend _columns As List(Of String)
        Friend _from As String
        Friend _fromsub As ISelect
        Friend _fromsubname As String
        Friend _joins As List(Of Join)
        Friend _where As Condition
        Friend _groupBy As List(Of String)
        Friend _having As String
        Friend _orderBy As List(Of String)
        Friend _offset As String
        Friend _desc As Boolean

        Public Function AddColumns(Of O As Class)(Optional Obj As O = Nothing) As [Select](Of T)
            Dim eltipo = GetType(O).GetNullableTypeOf()
            If eltipo Is GetType(Dictionary(Of String, Object)) Then
                If Obj IsNot Nothing Then
                    AddColumns(CType(CType(Obj, Object), Dictionary(Of String, Object)).Keys.ToArray())
                End If
            Else
                AddColumns(eltipo.GetProperties().Select(Function(x) x.Name).ToArray())
            End If
            Return Me
        End Function

        Public Function AddColumns(ParamArray Columns As String()) As [Select](Of T)
            Columns = If(Columns, {}).SelectMany(Function(x) x.Split(",")).Distinct().Where(Function(x) x.IsNotBlank()).ToArray()
            _columns = If(_columns, New List(Of String)())
            _columns.AddRange(Columns)
            Return Me
        End Function

        Public Function SetColumns(ParamArray Columns As String()) As [Select](Of T)
            _columns = Nothing
            AddColumns(Columns)
            Return Me
        End Function

        Public Function SetColumns(Of O As Class)(Optional Obj As O = Nothing) As [Select](Of T)
            _columns = Nothing
            AddColumns(Obj)
            Return Me
        End Function

        Public Function RemoveColumns(ParamArray Columns As String()) As [Select](Of T)
            If _columns IsNot Nothing Then
                _columns = _columns.Where(Function(x) x.IsNotIn(If(Columns, {}))).ToList()
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
        Public Function From(Of O As Class)(ByVal SubQuery As [Select](Of O), Optional SubQueryAlias As String = Nothing) As [Select](Of T)
            If SubQuery IsNot Nothing AndAlso SubQuery.ToString(True).IsNotBlank AndAlso SubQuery IsNot Me Then
                _from = Nothing
                _fromsub = SubQuery
                _fromsubname = SubQueryAlias.IfBlank(GetType(O).Name & "_" & Now.Ticks)
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the FROM clause in the SELECT being built.
        ''' </summary>
        ''' <param name="SubQuery">Subquery to be selected from</param>
        ''' <returns></returns>
        Public Function From(Of O As Class)(ByVal SubQuery As Action(Of [Select](Of O)), Optional SubQueryAlias As String = Nothing) As [Select](Of T)
            If SubQuery IsNot Nothing Then
                Dim sl = New [Select](Of O)()
                SubQuery(sl)
                Me.From(sl, SubQueryAlias)
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
                From(Of Dictionary(Of String, Object))(SubQuery)
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

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(ByVal LogicOperator As String, conditions As IEnumerable(Of FormattableString)) As [Select](Of T)
            For Each condition In If(conditions, {})
                If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank Then
                    If LogicOperator.IsIn("OR", "or") Then
                        [Or](condition)
                    Else
                        [And](condition)
                    End If
                End If
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' </summary>
        ''' <param name="conditions">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(ByVal LogicOperator As String, conditions As IEnumerable(Of Condition)) As [Select](Of T)
            For Each condition In If(conditions, {})
                If condition IsNot Nothing Then
                    If LogicOperator.IsIn("OR", "or") Then
                        [Or](condition)
                    Else
                        [And](condition)
                    End If
                End If
            Next
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built using a lambda expression. This method is experimental
        ''' </summary>
        ''' <param name="predicate">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
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

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <param name="conditions">Conditions to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(ParamArray conditions As Condition()) As [Select](Of T)
            Return [And](conditions)
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built using a <see cref="Dictionary(Of String, Object)"/> as column/value
        ''' </summary>
        ''' <param name="Dic"></param>
        ''' <param name="FilterKeys"></param>
        ''' <returns></returns>
        Public Function Where(Dic As Dictionary(Of String, Object), LogicConcatenation As LogicConcatenationOperator, ParamArray FilterKeys As String())

            FilterKeys = If(FilterKeys, {})

            If FilterKeys.Any Then
                FilterKeys = Dic.Keys.ToArray().Where(Function(x) x.IsLikeAny(FilterKeys)).ToArray
            Else
                FilterKeys = Dic.Keys.ToArray()
            End If

            FilterKeys = FilterKeys.Where(Function(x) Dic(x) IsNot Nothing AndAlso Dic(x).ToString().IsNotBlank()).ToArray()

            If FilterKeys.Any Then
                For Each f In FilterKeys
                    If LogicConcatenation = LogicConcatenationOperator.OR Then
                        Me.Or(New Condition(f, Dic(f)))
                    Else
                        Me.And(New Condition(f, Dic(f)))
                    End If
                Next
            End If
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built using a <see cref="NameValueCollection"/> as column/operator/value
        ''' </summary>
        ''' <param name="NVC"></param>
        ''' <param name="FilterKeys"></param>
        ''' <returns></returns>
        Public Function Where(NVC As NameValueCollection, ParamArray FilterKeys As String()) As [Select](Of T)
            FilterKeys = If(FilterKeys, {})
            For Each k In NVC.AllKeys
                If k.IsNotBlank() Then
                    Dim col = k.UrlDecode()
                    If Not FilterKeys.Any() OrElse col.IsLikeAny(FilterKeys) Then
                        Dim values = If(NVC.GetValues(k), New String() {})
                        For Each v In values
                            Dim logic = col.GetBefore(":", True).IfBlank("AND")
                            Dim op = v.GetBefore(":", True).IfBlank("=")
                            col = col.GetAfter(":")
                            col = col.Contains(" ").AsIf(col.UnQuote("[", True).Quote("["c), col)
                            Dim valor = v.GetAfter(":").NullIf("null", StringComparison.InvariantCultureIgnoreCase)

                            If valor Is Nothing Then
                                op = "is"
                            End If

                            If valor = "'null'" Then
                                valor = "null"
                            End If

                            Dim cond = New Condition(col, valor, op)
                            Where(logic, {cond})
                        Next
                    End If
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
            If Page < 0 Then
                _offset = Nothing
            Else
                PageSize = PageSize.SetMinValue(0)
                _offset = $"OFFSET {Page} ROWS FETCH NEXT {PageSize} ROWS ONLY"
            End If
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

        ''' <summary>
        ''' Operator overload that allows using the class wherever a string is expected.
        ''' </summary>
        Public Shared Widening Operator CType(ByVal [select] As [Select](Of T)) As FormattableString
            Return If([select] Is Nothing, Nothing, [select].ToString()).ToFormattableString()
        End Operator

        Public Function CreateDbCommand(Connection As DbConnection, dic As Dictionary(Of String, Object)) As DbCommand
            Return Connection.CreateCommand(ToString(), dic)
        End Function

        Public Function CreateDbCommand(Connection As DbConnection) As DbCommand
            Return CreateDbCommand(Connection, Nothing)
        End Function

        Public Function AndSearch(Value As String, ParamArray Columns As String()) As [Select](Of T)
            Return Me.AndAny(If(Columns, {}).Select(Function(x) (x & " LIKE {0}").ToFormattableString(Value.ToString().Wrap("%"))).ToArray())
        End Function

        Public Function OrSearch(Value As String, ParamArray Columns As String()) As [Select](Of T)
            Return Me.OrAny(If(Columns, {}).Select(Function(x) (x & " LIKE {0}").ToFormattableString(Value.ToString().Wrap("%"))).ToArray())
        End Function

        Public Function OrSearch(Value As IEnumerable(Of String), ParamArray Columns As String()) As [Select](Of T)
            For Each item In If(Value, {}).Where(Function(x) x.IsNotBlank)
                Me.OrSearch(item, Columns)
            Next
            Return Me
        End Function

        Public Function AndSearch(Value As IEnumerable(Of String), ParamArray Columns As String()) As [Select](Of T)
            For Each item In If(Value, {}).Where(Function(x) x.IsNotBlank)
                Me.AndSearch(item, Columns)
            Next
            Return Me
        End Function

        Public Overrides Function ToString() As String Implements ISelect.ToString
            Return ToString(False)
        End Function

        ''' <summary>
        ''' Returns the SELECT statement as a SQL query.
        ''' </summary>
        ''' <returns>The SELECT statement as a SQL query</returns>
        Public Overloads Function ToString(AsSubquery As Boolean) As String Implements ISelect.ToString
            Dim sql = New StringBuilder("SELECT ")
            sql.Append(String.Join(", ", _columns.Distinct().ToArray()).IfBlank(" * "))

            If _fromsub IsNot Nothing AndAlso _fromsub.ToString().IsNotBlank Then
                _from = _fromsub.ToString(True).Quote("(") & " as " & _fromsubname
            End If

            If _from.IsNotBlank Then
                sql.Append(" FROM ")
                sql.Append(_from)
            End If

            If _joins IsNot Nothing AndAlso _joins.Any() Then
                sql.Append(_joins.SelectJoin(Function(j) String.Format(CultureInfo.InvariantCulture, " {0}", j), " "))
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

            If _orderBy IsNot Nothing AndAlso AsSubquery = False Then

                sql.Append(" ORDER BY ")
                sql.Append(String.Join(", ", _orderBy))

            End If

            If _offset.IsNotBlank AndAlso AsSubquery = False Then
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
                    If LogicOperator.IsIn("Or", "OR") Then
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
                    If LogicOperator.IsIn("Or", "OR") Then
                        [Or](condition)
                    Else
                        [And](condition)
                    End If
                End If
            Next
        End Sub

        Public Sub New(LogicOperator As LogicConcatenationOperator, ParamArray Conditions As Condition())
            Me.New(LogicOperator.AsIf(Function(x) x = LogicConcatenationOperator.OR, "OR", "AND"), Conditions)
        End Sub

        Public Sub New(LogicOperator As LogicConcatenationOperator, ParamArray Conditions As FormattableString())
            Me.New(LogicOperator.AsIf(Function(x) x = LogicConcatenationOperator.OR, "OR", "AND"), Conditions)
        End Sub

        ''' <summary>
        ''' Select class constructor
        ''' </summary>
        ''' <param name="condition">Condition to set in this instance</param>
        Public Sub New(ByVal condition As FormattableString)
            If condition.IsNotBlank Then
                _tokens.Add(ToSQLString(condition))
            End If
        End Sub

        ''' <summary>
        ''' Select class constructor
        ''' </summary>
        ''' <param name="condition">Copies to the condition being constructed</param>
        Public Sub New(condition As Condition)
            If condition IsNot Nothing AndAlso condition.ToString().IsNotBlank Then
                _tokens.Add(condition.ParenthesisToString())
            End If
        End Sub

        ''' <summary>
        ''' Select class constructor
        ''' </summary>
        Public Sub New(Column As String, Value As Object, Optional [Operator] As String = "=")
            If Column.IsNotBlank() Then
                _tokens.Add($"{Column} {[Operator].IfBlank("=")} {ToSQLString(Value)}")
            End If
        End Sub

        Public Shared Function OrMany(ParamArray conditions As FormattableString()) As Condition
            Return New Condition("Or", conditions)
        End Function

        Public Shared Function AndMany(ParamArray conditions As FormattableString()) As Condition
            Return New Condition("And", conditions)
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
                    _tokens.Add("And")
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
                    _tokens.Add("Or")
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
            Return If([On] Is Nothing, String.Format(CultureInfo.InvariantCulture, "{0} {1}", JoinString, Table), String.Format(CultureInfo.InvariantCulture, "{0} {1} On {2}", JoinString, Table, [On]))
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

    <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Field Or AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)>
    Public Class FromSQL
        Inherits Attribute

        Sub New(QueryOrFilePath As String)
            If QueryOrFilePath.IsFilePath AndAlso File.Exists(QueryOrFilePath) Then
                Me.SQL = File.ReadAllText(QueryOrFilePath)
            Else
                Me.SQL = QueryOrFilePath
            End If
        End Sub

        Public ReadOnly Property SQL As String

    End Class

    <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Field, AllowMultiple:=True, Inherited:=True)>
    Public Class ColumnName
        Inherits Attribute
        Sub New(ParamArray Name As String())
            Me.Names = If(Name, {}).Select(Function(x) x.UnQuote())
        End Sub

        Public ReadOnly Property Names As String()

    End Class

End Namespace