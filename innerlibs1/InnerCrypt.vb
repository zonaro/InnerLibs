Imports System
Imports System.Runtime.CompilerServices

Public Module InnerCrypt

    Private letrasc() As Char = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray
    ''' <summary>
    ''' Criptografa uma suma string usando a logica InnerCrypt
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    <Extension()>
    Public Function InnCrypt(Text As String) As String
        Dim letras = Text.ToArray()
        Dim num As New List(Of String)
        For Each c In letras
            Dim i As Integer = Asc(c).ToString.GetFirstChars(2).ChangeType(Of Integer)
            i = i.LimitRange(1, letrasc.Length - 1)
            num.Add(letrasc(i) & (Asc(c) * Asc(c)))
        Next
        num.Reverse()
        Return num.Join("")
    End Function


    ''' <summary>
    ''' Descriptografa uma string previamente criptografada com InnerCrypt
    ''' </summary>
    ''' <param name="EncryptedText">Texto Criptografado</param>
    ''' <returns></returns>
    <Extension()> Public Function UnnCrypt(EncryptedText As String) As String
        Try
            Dim num = EncryptedText.Split(letrasc).ToList()
            num.Remove("")
            Dim letas As New List(Of Char)
            For Each n In num
                letas.Add(Chr(Math.Sqrt(n)))
            Next
            letas.Reverse()
            Return letas.Join("")
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Module
