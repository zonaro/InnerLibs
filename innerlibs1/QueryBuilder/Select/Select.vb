﻿Imports System
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Text

Namespace QueryLibrary

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
        Friend _desc As Boolean

        ''' <summary>
        ''' Class that aids building a SELECT clause.
        ''' </summary>
        ''' <paramname="columns">Columns to be selected</param>
        Public Sub New(ParamArray columns As String())
            If columns Is Nothing Then Throw New ArgumentNullException(NameOf(columns))

            If _columns Is Nothing Then
                _columns = New List(Of String)(columns)
            Else
                _columns.AddRange(columns)
            End If
        End Sub

        ''' <summary>
        ''' Sets the FROM clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be selected from</param>
        ''' <returns></returns>
        Public Function From(ByVal table As String) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If Not Equals(_from, Nothing) Then Throw New SelectBuildingException("FROM clause already set.")
            _from = table
            Return Me
        End Function

        ''' <summary>
        ''' Sets a JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Join(ByVal table As String, ByVal [on] As String) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If Equals([on], Nothing) Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.None, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Join(ByVal table As String, ByVal [on] As Condition) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If [on] Is Nothing Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.None, table, [on])
        End Function

        ''' <summary>
        ''' Sets a LEFT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function LeftOuterJoin(ByVal table As String, ByVal [on] As String) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If Equals([on], Nothing) Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.LeftOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a LEFT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function LeftOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If [on] Is Nothing Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.LeftOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function RightOuterJoin(ByVal table As String, ByVal [on] As String) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If Equals([on], Nothing) Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.RightOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a RIGHT OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function RightOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If [on] Is Nothing Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.RightOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a FULL OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function FullOuterJoin(ByVal table As String, ByVal [on] As String) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If Equals([on], Nothing) Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.FullOuterJoin, table, New Condition([on]))
        End Function

        ''' <summary>
        ''' Sets a FULL OUTER JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <paramname="on">Condition of the join (ON clause)</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function FullOuterJoin(ByVal table As String, ByVal [on] As Condition) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            If [on] Is Nothing Then Throw New ArgumentNullException(NameOf([on]))
            Return _Join(JoinType.FullOuterJoin, table, [on])
        End Function

        ''' <summary>
        ''' Sets a CROSS JOIN clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="table">Table to be join</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function CrossJoin(ByVal table As String) As [Select]
            If Equals(table, Nothing) Then Throw New ArgumentNullException(NameOf(table))
            Return _Join(JoinType.CrossJoin, table, Nothing)
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function Where(ByVal condition As String) As [Select]
            If Equals(condition, Nothing) Then Throw New ArgumentNullException(NameOf(condition))
            If _where IsNot Nothing Then Throw New SelectBuildingException("WHERE clause already set.")
            _where = New Condition(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' Throws exception if WHERE is already set.
        ''' </summary>
        ''' <paramname="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        ''' <exceptioncref="SelectBuildingException">WHERE clause is already set</exception>
        Public Function Where(ByVal condition As Condition) As [Select]
            If condition Is Nothing Then Throw New ArgumentNullException(NameOf(condition))
            If _where IsNot Nothing Then Throw New SelectBuildingException("WHERE clause is already set.")
            _where = New Condition(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <paramname="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function WhereAnd(ByVal condition As String) As [Select]
            If Equals(condition, Nothing) Then Throw New ArgumentNullException(NameOf(condition))
            If _where Is Nothing Then Return Where(condition)
            _where.And(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an AND clause.
        ''' </summary>
        ''' <paramname="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function WhereAnd(ByVal condition As Condition) As [Select]
            If condition Is Nothing Then Throw New ArgumentNullException(NameOf(condition))
            If _where Is Nothing Then Return Where(condition)
            _where.And(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an OR clause.
        ''' </summary>
        ''' <paramname="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function WhereOr(ByVal condition As String) As [Select]
            If Equals(condition, Nothing) Then Throw New ArgumentNullException(NameOf(condition))
            If _where Is Nothing Then Return Where(condition)
            _where.Or(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Sets the WHERE clause in the SELECT being built.
        ''' If WHERE is already set, appends the condition with an OR clause.
        ''' </summary>
        ''' <paramname="condition">Condition of the WHERE clause</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function WhereOr(ByVal condition As Condition) As [Select]
            If condition Is Nothing Then Throw New ArgumentNullException(NameOf(condition))
            If _where Is Nothing Then Return Where(condition)
            _where.Or(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Sets the GROUP BY clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="columns">Columns to be grouped by</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function GroupBy(ParamArray columns As String()) As [Select]
            If columns Is Nothing Then Throw New ArgumentNullException(NameOf(columns))

            If _groupBy Is Nothing Then
                _groupBy = New List(Of String)(columns)
            Else
                _groupBy.AddRange(columns)
            End If

            Return Me
        End Function

        ''' <summary>
        ''' Sets the HAVING clause in the SELECT being built.
        ''' Throws exception if HAVING is already set.
        ''' </summary>
        ''' <paramname="condition">Condition to set</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        ''' <exceptioncref="SelectBuildingException">HAVING clause is already set</exception>
        Public Function Having(ByVal condition As String) As [Select]
            If Equals(condition, Nothing) Then Throw New ArgumentNullException(NameOf(condition))
            If Not Equals(_having, Nothing) Then Throw New SelectBuildingException("HAVING clause is already set.")
            _having = condition
            Return Me
        End Function

        ''' <summary>
        ''' Sets the ORDER BY clause in the SELECT being built.
        ''' </summary>
        ''' <paramname="columns">Columns to be ordered by</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function OrderBy(ParamArray columns As String()) As [Select]
            If columns Is Nothing Then Throw New ArgumentNullException(NameOf(columns))

            If _orderBy Is Nothing Then
                _orderBy = New List(Of String)(columns)
            Else
                _orderBy.AddRange(columns)
            End If

            _desc = False
            Return Me
        End Function

        ''' <summary>
        ''' Sets the ORDER BY clause in the SELECT being built with DESC.
        ''' </summary>
        ''' <paramname="columns">Columns to be ordered by</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function OrderByDesc(ParamArray columns As String()) As [Select]
            If columns Is Nothing Then Throw New ArgumentNullException(NameOf(columns))

            If _orderBy Is Nothing Then
                _orderBy = New List(Of String)(columns)
            Else
                _orderBy.AddRange(columns)
            End If

            _desc = True
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
            Return $"({ToString()})"
        End Function

        ''' <summary>
        ''' Returns the SELECT statement as a SQL query.
        ''' </summary>
        ''' <returns>The SELECT statement as a SQL query</returns>
        Public Overrides Function ToString() As String
            Dim sql = New StringBuilder("SELECT ")
            sql.Append(String.Join(", ", _columns))

            If Not Equals(_from, Nothing) Then
                sql.Append(" FROM ")
                sql.Append(_from)
            End If

            If _joins IsNot Nothing Then
                For Each join In _joins
                    sql.Append(String.Format(CultureInfo.InvariantCulture, " {0}", join))
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
                If _desc Then sql.Append(" DESC")
            End If

            Return sql.ToString()
        End Function

        Private Function _Join(ByVal type As JoinType, ByVal table As String, ByVal [on] As Condition) As [Select]
            Dim join = New Join With {
                .Type = type,
                .Table = table,
                .[On] = [on]
            }

            If _joins Is Nothing Then
                _joins = New List(Of Join) From {
                    join
                }
            Else
                _joins.Add(join)
            End If

            Return Me
        End Function
    End Class
End Namespace