Imports System

Namespace QueryLibrary

    ''' <summary>
    ''' Exception thrown when building a SELECT clause.
    ''' </summary>
    Public Class SelectBuildingException
        Inherits Exception

        Friend Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
    End Class
End Namespace
