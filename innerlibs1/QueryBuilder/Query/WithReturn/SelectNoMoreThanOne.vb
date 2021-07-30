﻿Imports System
Imports System.Data
Imports System.Linq

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        ''' <summary>
        ''' Runs the given query and returns the queried values.
        ''' Throws UnexpectedNumberOfRowsSelected if more than one row is selected.
        ''' It uses DbDataAdapter.Fill underneath.
        ''' </summary>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <returns>A DataTable with the queried values</returns>
        ''' <exceptioncref="UnexpectedNumberOfRowsSelectedException">
        ''' If more than one row is selected
        ''' </exception>
        Public Function SelectNoMoreThanOne(ByVal sql As String, ByVal Optional parameters As Object = Nothing) As DataTable
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            Return SelectNoMoreThan(1, sql, parameters)
        End Function

        ''' <summary>
        ''' Runs the given query and returns the queried values.
        ''' Throws UnexpectedNumberOfRowsSelected if more than one row is selected.
        ''' It uses DbDataAdapter.Fill underneath.
        ''' </summary>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <returns>The querie objects</returns>
        ''' <exceptioncref="UnexpectedNumberOfRowsSelectedException">
        ''' If more than one row is selected
        ''' </exception>
        ''' <exceptioncref="MismatchedTypesException">
        ''' The corresponding type in the given class is different than the one found in the DataTable
        ''' </exception>
        ''' <exceptioncref="PropertyNotFoundException">
        ''' A column of the DataTable doesn't match any in the given class
        ''' </exception>
        Public Function SelectNoMoreThanOne(Of T As New)(ByVal sql As String, ByVal Optional parameters As Object = Nothing) As T
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            Return SelectNoMoreThan(Of T)(1, sql, parameters).SingleOrDefault()
        End Function
    End Class
End Namespace