Imports System

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        ''' <summary>
        ''' Runs the given query giving no return.
        ''' Throws UnexpectedNumberOfRowsAffected if the number of affected rows is less than N.
        ''' It uses DbCommand.ExecuteNonQuery underneath.
        ''' </summary>
        ''' <paramname="n">Minimum of affected rows to ensure</param>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <exceptioncref="UnexpectedNumberOfRowsAffectedException">
        ''' If the number of affected rows is less than N
        ''' </exception>
        Public Sub ChangeNoLessThan(ByVal n As Integer, ByVal sql As String, ByVal Optional parameters As Object = Nothing)
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            WithoutReturn(sql, parameters, CountValidationEnum.NoLessThan, n)
        End Sub
    End Class
End Namespace
