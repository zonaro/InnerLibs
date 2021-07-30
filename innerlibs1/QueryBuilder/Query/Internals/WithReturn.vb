Imports System.Data

Namespace QueryLibrary
    Public NotInheritable Partial Class Query
        Private Function WithReturn(Of tT)(ByVal sql As String, ByVal parameters As Object) As tT
            Using command = _parameterSetter.GetCommand(OpenConnection, sql, parameters)

                Try
                    ' No need to open a transaction, only to use an open one if exists
                    ' That's why _currentTransaction and not OpenTransaction (unlike the on writes)
                    command.Transaction = _currentTransaction
                    Dim t = CType(command.ExecuteScalar(), tT)
                    CloseIfNeeded()
                    Return t
                Catch
                    CloseRegardless(True)
                    Throw
                End Try
            End Using
        End Function

        Private Function WithReturn(ByVal sql As String, ByVal parameters As Object, ByVal countValidation As CountValidationEnum, ByVal n As Integer) As DataTable
            Using command = _parameterSetter.GetCommand(OpenConnection, sql, parameters)

                Try
                    ' No need to open a transaction, only to use an open one if exists
                    ' That's why _currentTransaction and not OpenTransaction (unlike the on writes)
                    command.Transaction = _currentTransaction
                    Dim dt = FillDataTable(command)
                    Dim selected = dt.Rows.Count

                    Select Case countValidation
                        Case CountValidationEnum.Exactly
                            If selected <> n Then Throw New UnexpectedNumberOfRowsSelectedException(command, selected)
                        Case CountValidationEnum.NoLessThan
                            If selected < n Then Throw New UnexpectedNumberOfRowsSelectedException(command, selected)
                        Case CountValidationEnum.NoMoreThan
                            If selected > n Then Throw New UnexpectedNumberOfRowsSelectedException(command, selected)
                    End Select

                    CloseIfNeeded()
                    Return dt
                Catch
                    CloseRegardless(True)
                    Throw
                End Try
            End Using
        End Function
    End Class
End Namespace
