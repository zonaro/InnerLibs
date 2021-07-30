Imports System

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        ''' <summary>
        ''' Runs the given query giving no return.
        ''' Throws UnexpectedNumberOfRowsAffected if the number of affected rows is different from N.
        ''' It uses DbCommand.ExecuteNonQuery underneath.
        ''' </summary>
        ''' <paramname="n">Number of affected rows to ensure</param>
        ''' <paramname="sql">Query to run</param>
        ''' <paramname="parameters">Parameters names and values pairs</param>
        ''' <exceptioncref="UnexpectedNumberOfRowsAffectedException">
        ''' If the number of affected rows is different from N
        ''' </exception>
        Public Sub ChangeExactly(ByVal n As Integer, ByVal sql As String, ByVal Optional parameters As Object = Nothing)
            If _disposed Then Throw New ObjectDisposedException([GetType]().FullName)
            WithoutReturn(sql, parameters, CountValidationEnum.Exactly, n)
        End Sub
    End Class
End Namespace
