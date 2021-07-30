Imports System.Data.Common
Imports QueryLibrary.Query

Namespace QueryLibrary

    ''' <summary>
    ''' Exception thrown when a query selects an unexpected number of rows.
    ''' </summary>
    Public Class UnexpectedNumberOfRowsSelectedException
        Inherits QueryException

        Friend Sub New(ByVal command As DbCommand, ByVal n As Integer)
            MyBase.New($"The following query selected an unexpected number of rows ({n}): {FormatSql(command)}")
        End Sub
    End Class
End Namespace
