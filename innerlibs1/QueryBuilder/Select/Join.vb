Imports System.Globalization

Namespace QueryLibrary
    Friend Class Join
        Friend Property Type As JoinType
        Friend Property Table As String
        Friend Property [On] As Condition

        Private ReadOnly Property JoinString As String
            Get

                Select Case Type
                    Case JoinType.Inner
                        Return "INNER JOIN"
                    Case JoinType.LeftOuterJoin
                        Return "LEFT OUTER JOIN"
                    Case JoinType.RightOuterJoin
                        Return "RIGHT OUTER JOIN"
                    Case JoinType.FullOuterJoin
                        Return "FULL OUTER JOIN"
                    Case JoinType.CrossJoin
                        Return "CROSS JOIN"
                    Case Else
                        Return "JOIN"
                End Select
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return If([On] Is Nothing, String.Format(CultureInfo.InvariantCulture, "{0} {1}", JoinString, Table), String.Format(CultureInfo.InvariantCulture, "{0} {1} ON {2}", JoinString, Table, [On]))
        End Function
    End Class
End Namespace
