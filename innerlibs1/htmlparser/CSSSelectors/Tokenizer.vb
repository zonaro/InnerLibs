Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser

    Public Class Tokenizer

        Public Shared Function GetTokens(cssFilter As String) As IEnumerable(Of Token)
            Dim tk As New List(Of Token)

            Dim reader = New System.IO.StringReader(cssFilter)
            While True
                Dim v As Integer = reader.Read()

                If v < 0 Then

                    Exit While

                End If

                Dim c As Char = Chr(v)

                If c = ">"c Then

                    tk.Add(New Token(">"))

                    Continue While

                End If

                If c = " "c OrElse c = ControlChars.Tab Then

                    Continue While

                End If

                Dim word As String = c & ReadWord(reader)

                tk.Add(New Token(word))

            End While
            Return tk
        End Function

        Private Shared Function ReadWord(reader As System.IO.StringReader) As String

            Dim sb = ""

            While True

                Dim v As Integer = reader.Read()

                If v < 0 Then
                    Exit While
                End If

                Dim c As Char = Chr(v)

                If c = " "c OrElse c = ControlChars.Tab Then

                    Exit While

                End If

                sb &= (c)
            End While

            Return sb.ToString()
        End Function

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================