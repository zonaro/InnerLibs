Imports System
Imports System.Collections.Generic
Imports System.Data
Imports ObjectLibrary

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        ''' <summary>
        ''' Runs the given query and returns the queried values.
        ''' Throws UnexpectedNumberOfRowsSelected if the number of selected rows is greater than N.
        ''' It uses DbDataAdapter.Fill underneath.
        ''' </summary>
        ''' <paramname="n">Number of selected rows to ensure</param>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <returns>A DataTable with the queried values</returns>
        ''' <exceptioncref="UnexpectedNumberOfRowsSelectedException">
        ''' If the number of selected rows is greater than N
        ''' </exception>
        Public Function SelectNoMoreThan(ByVal n As Integer, ByVal sql As String, ByVal Optional parameters As Object = Nothing) As DataTable
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            Return WithReturn(sql, parameters, CountValidationEnum.NoMoreThan, n)
        End Function

        ''' <summary>
        ''' Runs the given query and returns the queried values.
        ''' Throws UnexpectedNumberOfRowsSelected if the number of selected rows is greater than N.
        ''' It uses DbDataAdapter.Fill underneath.
        ''' </summary>
        ''' <paramname="n">Number of selected rows to ensure</param>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <returns>The queried objects</returns>
        ''' <exceptioncref="UnexpectedNumberOfRowsSelectedException">
        ''' If the number of selected rows is greater than N
        ''' </exception>
        ''' <exceptioncref="MismatchedTypesException">
        ''' The corresponding type in the given class is different than the one found in the DataTable
        ''' </exception>
        ''' <exceptioncref="PropertyNotFoundException">
        ''' A column of the DataTable doesn't match any in the given class
        ''' </exception>
        Public Function SelectNoMoreThan(Of T As New)(ByVal n As Integer, ByVal sql As String, ByVal Optional parameters As Object = Nothing) As IEnumerable(Of T)
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            Return If(_safe, SelectNoMoreThan(n, sql, parameters).ToObjectSafe(Of T)(), SelectNoMoreThan(n, sql, parameters).ToObject(Of T)())
        End Function
    End Class
End Namespace
