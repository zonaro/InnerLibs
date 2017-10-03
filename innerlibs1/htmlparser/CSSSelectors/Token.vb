
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace HtmlParser
    Public Class Token

        Public Property Filter() As String
            Get

                Return m_Filter

            End Get

            Set
                m_Filter = Value

            End Set

        End Property

        Private m_Filter As String

        Public Property SubTokens() As IList(Of Token)
            Get

                Return m_SubTokens

            End Get

            Set
                m_SubTokens = Value

            End Set

        End Property

        Private m_SubTokens As IList(Of Token)

        Public Sub New(word As String)

            If String.IsNullOrEmpty(word) Then
                Throw New ArgumentNullException("word")

            End If


            Dim tokens = SplitTokens(word).ToList()


            Me.Filter = tokens.First()
            Me.SubTokens = tokens.Skip(1).[Select] (Function(i) New Token(i)).ToList()

        End Sub


        Private Shared Function SplitTokens(token As String) As IList(Of String)

            Dim isNameToken As Func(Of Char, Boolean) = Function(c) Char.IsLetterOrDigit(c) OrElse c = "-"C OrElse c = "_"C
            Dim rt = New List(Of String)()

            Dim start As Integer = 0
			Dim isPrefix As Boolean = True

            Dim isOpeningBracket As Boolean = False

            Dim closeBracket As Char = ControlChars.NullChar

            For i As Integer = 0 To token.Length - 1
				If isOpeningBracket AndAlso token(i) <> closeBracket Then

                    Continue For

                End If


                isOpeningBracket = False


                If token(i) = "("C Then

                    closeBracket = ")"C
                    isOpeningBracket = True

                ElseIf token(i) = "["C Then

                    closeBracket = "]"C
                    If i<> start Then
                        rt.Add(token.Substring(start, i - start))
						start = i
                    End If
                    isOpeningBracket = True

                ElseIf i = token.Length - 1 Then
                    rt.Add(token.Substring(start, i - start + 1))
				ElseIf Not isNameToken(token(i)) AndAlso Not isPrefix Then

                    rt.Add(token.Substring(start, i - start))
					start = i
                ElseIf isNameToken(token(i)) Then
                    isPrefix = False

                End If

            Next

            Return rt
        End Function
    End Class
End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik
'Facebook: facebook.com/telerik
'=======================================================
