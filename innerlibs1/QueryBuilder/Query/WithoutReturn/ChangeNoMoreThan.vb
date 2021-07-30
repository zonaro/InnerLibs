Imports System

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        ''' <summary>
        ''' Runs the given query giving no return.
        ''' Throws UnexpectedNumberOfRowsAffected if the number of affected rows is greater than N.
        ''' It uses DbCommand.ExecuteNonQuery underneath.
        ''' </summary>
        ''' <paramname="n">Maximum of affected rows to ensure</param>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <exceptioncref="UnexpectedNumberOfRowsAffectedException">
        ''' If the number of affected rows is greater than N
        ''' </exception>
        Public Sub ChangeNoMoreThan(ByVal n As Integer, ByVal sql As String, ByVal Optional parameters As Object = Nothing)
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            WithoutReturn(sql, parameters, CountValidationEnum.NoMoreThan, n)
        End Sub
    End Class
End Namespace
