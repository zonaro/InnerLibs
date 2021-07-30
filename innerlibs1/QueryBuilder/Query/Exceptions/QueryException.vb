Imports System

Namespace QueryLibrary

    ''' <summary>
    ''' Query's base exception class.
    ''' </summary>
    Public MustInherit Class QueryException
        Inherits Exception

        Friend Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
    End Class
End Namespace
