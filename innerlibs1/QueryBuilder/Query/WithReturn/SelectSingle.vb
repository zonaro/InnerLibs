Imports System

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        ''' <summary>
        ''' Runs the given query and returns the queried value.
        ''' Only a single value (first column of the first row) is returned.
        ''' It uses DbCommand.ExecuteScalar underneath.
        ''' </summary>
        ''' <typeparamname="T">Type of the value to be returned</typeparam>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <returns>The first column of the first row queried</returns>
        Public Function SelectSingle(Of T)(ByVal sql As String, ByVal Optional parameters As Object = Nothing) As T
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            Return WithReturn(Of T)(sql, parameters)
        End Function
    End Class
End Namespace
