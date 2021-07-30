Imports System.Data.Common
Imports QueryLibrary.Query

Namespace QueryLibrary

    ''' <summary>
    ''' Exception thrown when a query affects an unexpected number of rows.
    ''' </summary>
    Public Class UnexpectedNumberOfRowsAffectedException
        Inherits QueryException

        Friend Sub New(ByVal command As DbCommand, ByVal n As Integer)
            MyBase.New($"The following query was rolled back because it affected {n} rows: {FormatSql(command)}")
        End Sub
    End Class
End Namespace
