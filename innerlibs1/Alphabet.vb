
Imports System.Text

Public Class Alphabet


    Public Sub New()
    End Sub

    Public Sub New(Seed As String)
        If Seed.IsNotBlank Then
            For index = 1 To Seed.Length
                Dim ii = index - 1
                Alphabet = Alphabet.OrderBy(Function(x)
                                                Return Encoding.ASCII.GetBytes(x.ToString).FirstOrDefault() Xor Encoding.ASCII.GetBytes(Seed(ii).ToString).FirstOrDefault()
                                            End Function).Join("")
            Next

        End If
    End Sub

    Public ReadOnly Property Alphabet As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789"

    Public ReadOnly Property Base As Integer
        Get
            Return Alphabet.Length
        End Get
    End Property

    Public ReadOnly OrderSeed As String = Nothing

    Public Function RandomHash() As String
        Return Encode(RandomNumber)
    End Function

    Public Function Encode(i As Integer) As String
        If i = 0 Then
            Return Alphabet(0).ToString()
        End If

        Dim s = String.Empty

        While i > 0
            s &= Alphabet(i Mod Base)
            i = i \ Base
        End While

        While s.Length < 4
            s &= "0"
        End While

        Return String.Join(String.Empty, s.Reverse())

    End Function

    Public Function Decode(s As String) As Integer
        Dim i = 0
        Try
            For Each c In s.Trim().Trim("0")
                i = (i * Base) + Alphabet.IndexOf(c)
            Next
            Return i
        Catch ex As Exception
            Return -1
        End Try
    End Function

End Class

