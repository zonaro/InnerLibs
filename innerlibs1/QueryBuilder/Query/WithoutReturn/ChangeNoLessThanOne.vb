Imports System

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        ''' <summary>
        ''' Runs the given query giving no return.
        ''' Throws UnexpectedNumberOfRowsAffected if less than one row is affected.
        ''' It uses DbCommand.ExecuteNonQuery underneath.
        ''' </summary>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <exceptioncref="UnexpectedNumberOfRowsAffectedException">
        ''' If less than one row is affected
        ''' </exception>
        Public Sub ChangeNoLessThanOne(ByVal sql As String, ByVal Optional parameters As Object = Nothing)
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            ChangeNoLessThan(1, sql, parameters)
        End Sub
    End Class
End Namespace
