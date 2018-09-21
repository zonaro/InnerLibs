Imports System
Imports System.Runtime.CompilerServices

Public Module InnerCrypt

    Private letrasc() As Char = "AaBbCcDdEeFfGgHhIiJjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz".ToArray

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
            Dim ll = Asc(c).ToString
            Dim i As Integer = ll.GetFirstChars(1).IfBlank(1) + ll.GetLastChars(1).IfBlank(1)
            i = i.LimitRange(0, letrasc.Length - 1)
            num.Add(letrasc(i) & Math.Pow(Asc(c), 3))
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
            Dim num = EncryptedText.Split(letrasc, StringSplitOptions.RemoveEmptyEntries)
            Dim letas As New List(Of Char)
            For Each n In num
                letas.Add(Chr(n ^ (1 / 3)))
            Next
            letas.Reverse()
            Return letas.Join("")
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Module
