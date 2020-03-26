Imports System.IO

Public Class JSMin
    Const EOF As Integer = -1
    Private sr As StringReader
    Private sw As String = ""
    Private theA As Integer
    Private theB As Integer
    Private theLookahead As Integer = EOF

    Public Function Minify(ByVal src As String) As String

        Using sr = New StringReader(src)
            jsmin(sr)
            Return sw
        End Using

    End Function

    Private Sub jsmin(sr As StringReader)
        theA = AscW(vbLf)
        action(3, sr)

        While theA <> EOF

            Select Case theA
                Case AscW(" ")

                    If IsAlphanum(theB) Then
                        action(1, sr)
                    Else
                        action(2, sr)
                    End If

                    Exit Select
                Case AscW(vbLf)

                    Select Case theB
                        Case AscW("{"), AscW("["), AscW("("), AscW("+"), AscW("-")
                            action(1, sr)
                            Exit Select
                        Case AscW(" ")
                            action(3, sr)
                            Exit Select
                        Case Else

                            If IsAlphanum(theB) Then
                                action(1, sr)
                            Else
                                action(2, sr)
                            End If

                            Exit Select
                    End Select

                    Exit Select
                Case Else

                    Select Case theB
                        Case AscW(" ")

                            If IsAlphanum(theA) Then
                                action(1, sr)
                                Exit Select
                            End If

                            action(3, sr)
                            Exit Select
                        Case AscW(vbLf)

                            Select Case theA
                                Case AscW("}"), AscW("]"), AscW(")"), AscW("+"), AscW("-"), AscW(""""), AscW("'")
                                    action(1, sr)
                                    Exit Select
                                Case Else

                                    If IsAlphanum(theA) Then
                                        action(1, sr)
                                    Else
                                        action(3, sr)
                                    End If

                                    Exit Select
                            End Select

                            Exit Select
                        Case Else
                            action(1, sr)
                            Exit Select
                    End Select

                    Exit Select
            End Select
        End While
    End Sub

    Private Sub action(ByVal d As Integer, sr As StringReader)
        If d <= 1 Then
            put(theA)
        End If

        If d <= 2 Then
            theA = theB

            If theA = AscW("'") OrElse theA = AscW("""") Then

                While True
                    put(theA)
                    theA = [get](sr)

                    If theA = theB Then
                        Exit While
                    End If

                    If theA <= AscW(vbLf) Then
                        Throw New Exception(String.Format("Error: JSMIN unterminated string literal: {0}" & vbLf, theA))
                    End If

                    If theA = AscW("\") Then
                        put(theA)
                        theA = [get](sr)
                    End If
                End While
            End If
        End If

        If d <= 3 Then
            theB = [next](sr)

            If theB = AscW("/") AndAlso (theA = AscW("(") OrElse theA = AscW(",") OrElse theA = AscW("=") OrElse theA = AscW("[") OrElse theA = AscW("!") OrElse theA = AscW(":") OrElse theA = AscW("&") OrElse theA = AscW("|") OrElse theA = AscW("?") OrElse theA = AscW("{") OrElse theA = AscW("}") OrElse theA = AscW(";") OrElse theA = AscW(vbLf)) Then
                put(theA)
                put(theB)

                While True
                    theA = [get](sr)

                    If theA = AscW("/") Then
                        Exit While
                    ElseIf theA = AscW("\") Then
                        put(theA)
                        theA = [get](sr)
                    ElseIf theA <= AscW(vbLf) Then
                        Throw New Exception(String.Format("Error: JSMIN unterminated Regular Expression literal : {0}." & vbLf, theA))
                    End If

                    put(theA)
                End While

                theB = [next](sr)
            End If
        End If
    End Sub

    Private Function [next](sr As StringReader) As Integer
        Dim c As Integer = [get](sr)

        If c = AscW("/") Then

            Select Case peek(sr)
                Case AscW("/")

                    While True
                        c = [get](sr)

                        If c <= AscW(vbLf) Then
                            Return c
                        End If
                    End While

                Case AscW("*")
                    [get](sr)

                    While True

                        Select Case [get](sr)
                            Case AscW("*")

                                If peek(sr) = AscW("/") Then
                                    [get](sr)
                                    Return AscW(" ")
                                End If

                                Exit Select
                            Case EOF
                                Throw New Exception("Error: JSMIN Unterminated comment." & vbLf)
                        End Select
                    End While

                Case Else
                    Return c
            End Select
        End If

        Return c
    End Function

    Private Function peek(sr As StringReader) As Integer
        theLookahead = [get](sr)
        Return theLookahead
    End Function

    Private Function [get](sr As StringReader) As Integer
        Dim c As Integer = theLookahead
        theLookahead = EOF

        If c = EOF Then
            c = sr.Read()
        End If

        If c >= AscW(" ") OrElse c = AscW(vbLf) OrElse c = EOF Then
            Return c
        End If

        If c = AscW(vbCr) Then
            Return AscW(vbLf)
        End If

        Return AscW(" ")
    End Function

    Private Sub put(ByVal c As Integer)
        sw = sw.Append(ChrW(c))
    End Sub

    Function IsAlphanum(ByVal c As Integer) As Boolean
        Return ((c >= AscW("a") AndAlso c <= AscW("z")) OrElse (c >= AscW("0") AndAlso c <= AscW("9")) OrElse (c >= AscW("A") AndAlso c <= AscW("Z")) OrElse c = AscW("_") OrElse c = AscW("$") OrElse c = AscW("\") OrElse c = AscW("+") OrElse c > 126)
    End Function

End Class