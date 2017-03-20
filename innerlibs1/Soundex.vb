Imports System.Runtime.CompilerServices

Public Module Sound


    ''' <summary>
    ''' Gera um código SOUNDEX para comparação de fonemas
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>Um código soundex</returns>
    <Extension>
    Public Function SoundEx(ByVal Text As String) As String
        Return SoundEx(Text, 4)
    End Function

    ''' <summary>
    ''' Compara 2 palavras e verifica se elas possuem fonema parecido
    ''' </summary>
    ''' <param name="FirstText">Primeira palavra</param>
    ''' <param name="SecondText">Segunda palavra</param>
    ''' <returns>TRUE se possuirem o mesmo fonema</returns>
    <Extension>
    Public Function SoundsLike(FirstText As String, SecondText As String) As Boolean
        Return FirstText.SoundEx() = SecondText.SoundEx()
    End Function


    ''' <summary>
    ''' Gera um código SOUNDEX para comparação de fonemas
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns>Um código soundex</returns>
    Private Function SoundEx(ByVal Text As String, ByVal Length As Integer) As String
        ' Value to return
        Dim Value As String = ""
        ' Size of the word to process
        Dim Size As Integer = Text.Length
        ' Make sure the word is at least two characters in length
        If (Size > 1) Then
            ' Convert the word to all uppercase
            Text = Text.ToUpper()
            ' Conver to the word to a character array for faster processing
            Dim Chars() As Char = Text.ToCharArray()
            ' Buffer to build up with character codes
            Dim Buffer As New System.Text.StringBuilder
            Buffer.Length = 0
            ' The current and previous character codes
            Dim PrevCode As Integer = 0
            Dim CurrCode As Integer = 0
            ' Append the first character to the buffer
            Buffer.Append(Chars(0))
            ' Prepare variables for loop
            Dim i As Integer
            Dim LoopLimit As Integer = Size - 1
            ' Loop through all the characters and convert them to the proper character code
            For i = 1 To LoopLimit
                Select Case Chars(i)
                    Case "A", "E", "I", "O", "U", "H", "W", "Y"
                        CurrCode = 0
                    Case "B", "F", "P", "V"
                        CurrCode = 1
                    Case "C", "G", "J", "K", "Q", "S", "X", "Z"
                        CurrCode = 2
                    Case "D", "T"
                        CurrCode = 3
                    Case "L"
                        CurrCode = 4
                    Case "M", "N"
                        CurrCode = 5
                    Case "R"
                        CurrCode = 6
                End Select
                ' Check to see if the current code is the same as the last one
                If (CurrCode <> PrevCode) Then
                    ' Check to see if the current code is 0 (a vowel); do not proceed
                    If (CurrCode <> 0) Then
                        Buffer.Append(CurrCode)
                    End If
                End If
                ' If the buffer size meets the length limit, then exit the loop
                If (Buffer.Length = Length) Then
                    Exit For
                End If
            Next
            ' Padd the buffer if required
            Size = Buffer.Length
            If (Size < Length) Then
                Buffer.Append("0", (Length - Size))
            End If
            ' Set the return value
            Value = Buffer.ToString()
        End If
        ' Return the computed soundex
        Return Value
    End Function
End Module
