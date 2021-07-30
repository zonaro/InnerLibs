Imports System
Imports System.Collections.Generic

Namespace QueryLibrary

    ''' <summary>
    ''' A condition with optional AND and OR clauses that can be used in WHERE or JOIN ON statements.
    ''' </summary>
    Public Class Condition
        Private ReadOnly _tokens As List(Of String) = New List(Of String)()

        ''' <summary>
        ''' Ctor.
        ''' </summary>
        ''' <paramname="condition">Condition to set in this instance</param>
        Public Sub New(ByVal condition As String)
            If Equals(condition, Nothing) Then Throw New ArgumentNullException(NameOf(condition))
            _tokens.Add(condition)
        End Sub

        ''' <summary>
        ''' Ctor.
        ''' </summary>
        ''' <paramname="condition">Copies to the condition being constructed</param>
        Public Sub New(ByVal condition As Condition)
            If condition Is Nothing Then Throw New ArgumentNullException(NameOf(condition))
            _tokens.Add(condition.ParenthesisToString())
        End Sub

        ''' <summary>
        ''' Appends the given condition with AND in this condition.
        ''' </summary>
        ''' <paramname="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ByVal condition As String) As Condition
            If Equals(condition, Nothing) Then Throw New ArgumentNullException(NameOf(condition))
            _tokens.Add("AND")
            _tokens.Add(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Appends the given condition with AND in this condition.
        ''' </summary>
        ''' <paramname="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [And](ByVal condition As Condition) As Condition
            If condition Is Nothing Then Throw New ArgumentNullException(NameOf(condition))
            Return [And](condition.ParenthesisToString())
        End Function

        ''' <summary>
        ''' Appends the given condition with OR in this condition.
        ''' </summary>
        ''' <paramname="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [Or](ByVal condition As String) As Condition
            If Equals(condition, Nothing) Then Throw New ArgumentNullException(NameOf(condition))
            _tokens.Add("OR")
            _tokens.Add(condition)
            Return Me
        End Function

        ''' <summary>
        ''' Appends the given condition with OR in this condition.
        ''' </summary>
        ''' <paramname="condition">Condition to be appended</param>
        ''' <returns>This instance, so you can use it in a fluent fashion</returns>
        Public Function [Or](ByVal condition As Condition) As Condition
            If condition Is Nothing Then Throw New ArgumentNullException("condition")
            Return [Or](condition.ParenthesisToString())
        End Function

        ''' <summary>
        ''' Returns the condition statement as a SQL query in parenthesis.
        ''' </summary>
        ''' <returns>The condition statement as a SQL query in parenthesis</returns>
        Public Function ParenthesisToString() As String
            Return $"({ToString()})"
        End Function

        ''' <summary>
        ''' Returns the condition statement as a SQL query.
        ''' </summary>
        ''' <returns>The condition statement as a SQL query</returns>
        Public Overrides Function ToString() As String
            Return String.Join(" ", _tokens)
        End Function
    End Class
End Namespace
