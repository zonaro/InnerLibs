
Public Class Alphabet


    Public Shared ReadOnly Alphabet As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789"
    Public Shared ReadOnly Base As Integer = Alphabet.Length

    Public Shared Function Encode(i As Integer) As String
        If i = 0 Then
            Return Alphabet(0).ToString()
        End If

        Dim s = String.Empty

        While i > 0
            s &= Alphabet(i Mod Base)
            i = i \ Base
        End While

        While s.Length < 4
            s.Append("0")
        End While

        Return String.Join(String.Empty, s.Reverse())

    End Function

    Public Shared Function Decode(s As String) As Integer
        Dim i = 0
        Try
            For Each c In s.Trim().Trim("0")
                i = (i * Base) + Alphabet.IndexOf(c)
            Next

            Return i.SetMinValue(1)
        Catch ex As Exception
            Return -1
        End Try
    End Function

End Class

