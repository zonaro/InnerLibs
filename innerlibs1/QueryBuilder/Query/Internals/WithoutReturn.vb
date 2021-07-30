Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        Private Sub WithoutReturn(ByVal sql As String, ByVal parameters As Object, ByVal countValidation As CountValidationEnum, ByVal n As Integer)
            Using command = _parameterSetter.GetCommand(OpenConnection, sql, parameters)

                Try
                    command.Transaction = OpenTransaction
                    Dim affected = command.ExecuteNonQuery()

                    Select Case countValidation
                        Case CountValidationEnum.Exactly
                            If affected <> n Then Throw New UnexpectedNumberOfRowsAffectedException(command, affected)
                        Case CountValidationEnum.NoLessThan
                            If affected < n Then Throw New UnexpectedNumberOfRowsAffectedException(command, affected)
                        Case CountValidationEnum.NoMoreThan
                            If affected > n Then Throw New UnexpectedNumberOfRowsAffectedException(command, affected)
                    End Select

                    CloseIfNeeded()
                Catch
                    CloseRegardless(True)
                    Throw
                End Try
            End Using
        End Sub
    End Class
End Namespace
