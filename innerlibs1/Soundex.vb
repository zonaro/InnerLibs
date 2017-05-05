Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Text.RegularExpressions

Public Module SoundEx

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

''' <summary>
''' Implementação da função SoundEX em Portugues
''' </summary>
Public NotInheritable Class Phonetic

    ''' <summary>
    ''' Compara o fonema de uma palavra em portugues com outra palavra
    ''' </summary>
    ''' <param name="Word">Palavra para comparar</param>
    ''' <returns></returns>
    Default ReadOnly Property SoundsLike(Word As String) As Boolean
        Get
            Return New Phonetic(Word).SoundExCode = Me.SoundExCode Or Word = Me.Word
        End Get
    End Property

    ''' <summary>
    ''' Compara o fonema de uma palavra com outra
    ''' </summary>
    ''' <param name="Word1">primeira palavra</param>
    ''' <param name="Word2">segunda palavra</param>
    ''' <returns></returns>
    Shared Operator Like(Word1 As Phonetic, Word2 As String) As Boolean
        Return Word1(Word2)
    End Operator

    ''' <summary>
    ''' Compara o fonema de uma palavra com outra
    ''' </summary>
    ''' <param name="Word1">primeira palavra</param>
    ''' <param name="Word2">segunda palavra</param>
    ''' <returns></returns>
    Shared Operator Like(Word1 As String, Word2 As Phonetic) As Boolean
        Return Word2(Word1)
    End Operator

    ''' <summary>
    ''' Verifica se o fonema atual está presente em alguma frase
    ''' </summary>
    ''' <param name="Text">Texto</param>
    ''' <returns></returns>
    Public Function IsListenedIn(Text As String) As Boolean
        For Each w In Text.Split(" ")
            If Me Like w Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' Palavra Original
    ''' </summary>
    ''' <returns></returns>
    Property Word As String

    ''' <summary>
    ''' Cria um novo Phonetic a partir de uma palavra
    ''' </summary>
    ''' <param name="Word">Palavra</param>
    Sub New(Word As String)
        Try
            Me.Word = Word.Split(" ")(0)
        Catch ex As Exception
            Me.Word = Word
        End Try
    End Sub
    ''' <summary>
    ''' Código SoundExBR que representa o fonema da palavra
    ''' </summary>
    ''' <returns></returns>
    ReadOnly Property SoundExCode As String
        Get
            Dim text = Word
            text = text.Trim.ToUpper
            If text.EndsWith("Z") And text.StartsWith("Z") Then
                text = "Z" & text.Trim("Z").Replace("Z", "S") & "S"
            Else
                If text.StartsWith("Z") Then
                    text = "Z" & text.Replace("Z", "S")
                Else
                    text = text.Replace("Z", "S")
                End If
            End If
            text = text.Replace("Ç", "S")
            text = text.RemoveDiacritics
            text = text.Replace("Y", "I")
            text = text.Replace("AL", "AU")
            text = text.Replace("BR", "B")
            text = text.Replace("BL", "B")
            text = text.Replace("PH", "F")
            text = text.Replace("MG", "G")
            text = text.Replace("NG", "G")
            text = text.Replace("RG", "G")
            text = text.Replace("GE", "J")
            text = text.Replace("GI", "J")
            text = text.Replace("RJ", "J")
            text = text.Replace("MJ", "J")
            text = text.Replace("NJ", "J")
            text = text.Replace("GR", "G")
            text = text.Replace("GL", "G")
            text = text.Replace("CE", "S")
            text = text.Replace("CI", "S")
            text = text.Replace("CH", "X")
            text = text.Replace("CT", "T")
            text = text.Replace("CS", "S")
            text = text.Replace("QU", "K")
            text = text.Replace("Q", "K")
            text = text.Replace("CA", "K")
            text = text.Replace("CO", "K")
            text = text.Replace("CU", "K")
            text = text.Replace("CK", "K")
            text = text.Replace("LH", "LI")
            text = text.Replace("RM", "SM")
            text = text.Replace("N", "M")
            text = text.Replace("GM", "M")
            text = text.Replace("MD", "M")
            text = text.Replace("NH", "N")
            text = text.Replace("PR", "P")
            text = text.Replace("X", "S")
            text = text.Replace("TS", "S")
            text = text.Replace("RS", "S")
            text = text.Replace("TR", "T")
            text = text.Replace("TL", "T")
            text = text.Replace("LT", "T")
            text = text.Replace("RT", "T")
            text = text.Replace("ST", "T")
            text = text.Replace("W", "V")
            text = text.Replace("L", "R")
            text = text.Replace("H", "")
            Dim sb = New StringBuilder(text)

            If text.IsNotBlank Then
                Dim tam As Integer = sb.Length - 1
                If tam > -1 Then
                    If sb(tam) = "S" OrElse sb(tam) = "Z" OrElse sb(tam) = "R" OrElse sb(tam) = "M" OrElse sb(tam) = "N" OrElse sb(tam) = "L" Then
                        sb.Remove(tam, 1)
                    End If
                End If
                tam = sb.Length - 2
                If tam > -1 Then
                    If sb(tam) = "A" AndAlso sb(tam + 1) = "O" Then
                        sb.Remove(tam, 2)
                    End If
                End If

                Dim frasesaida As New StringBuilder()
                Try
                    frasesaida.Append(sb(0))
                Catch ex As Exception
                End Try
                For i As Integer = 1 To sb.Length - 1
                    If frasesaida(frasesaida.Length - 1) <> sb(i) OrElse Char.IsDigit(sb(i)) Then
                        frasesaida.Append(sb(i))
                    End If
                Next
                Return frasesaida.ToString
            Else
                Return ""
            End If
        End Get
    End Property

End Class