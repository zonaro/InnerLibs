
Namespace Select2Data

    Public Class Result
        Public Property id As String
        Public Property text As String
        Public Property otherdata As Object
    End Class

    Public Class Pagination
        Public Property more As Boolean = False
    End Class

    Public Class Group
        Public Property text As String
        Public Property children As Result()
    End Class

    Public Class UngroupedResult
        Inherits ResultType
        Public Shadows Property results As Result()
    End Class

    Public Class GroupedResult
        Inherits ResultType
        Public Shadows Property results As Group()
    End Class

    Public MustInherit Class ResultType
        Public Property results As Object
        Public Property pagination As Pagination
    End Class

End Namespace

