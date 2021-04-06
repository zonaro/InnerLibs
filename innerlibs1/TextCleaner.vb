Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization

Public Class Paragraph
    Inherits List(Of Sentence)

    Friend Sub New(Text As String, StructuredText As StructuredText)
        Me.StructuredText = StructuredText
        Dim sep0 As Char = "."c, sep1 As Char = "!"c, sep2 As Char = "?"c
        Dim pattern As String = String.Format("[{0}{1}{2}]|[^{0}{1}{2}]+", sep0, sep1, sep2)
        Dim regex As Regex = New Regex(pattern)
        Dim matches As MatchCollection = regex.Matches(Text)

        For Each match As Match In matches
            Me.Add(New Sentence(match.ToString(), Me))
        Next

    End Sub

    Property Ident As Integer = 0

    Public Overrides Function ToString() As String
        Dim ss = ""
        For Each s In Me
            ss &= (s.ToString & " ")
        Next
        Return ss.Trim
    End Function

    Property StructuredText As StructuredText

End Class

''' <summary>
''' Sentença de um texto (uma frase ou oração)
''' </summary>
Public Class Sentence
    Inherits List(Of SentencePart)

    Friend Sub New(Text As String, Paragraph As Paragraph)
        Me.Paragraph = Paragraph
        Dim charlist = Text.Trim.ToArray.ToList
        Dim palavra As String = ""
        Dim listabase As New List(Of String)

        'remove quaisquer caracteres nao desejados do inicio da frase
        While charlist.Count > 0 AndAlso charlist.First.ToString.IsIn(EndOfSentencePunctuation)
            charlist.Remove(charlist.First)
        End While

        'processa caractere a caractere
        For Each p In charlist
            Select Case True
                'caso for algum tipo de pontuacao, wrapper ou virgula
                Case OpenWrappers.Contains(p), CloseWrappers.Contains(p), EndOfSentencePunctuation.Contains(p), MidSentencePunctuation.Contains(p)
                    If palavra.IsNotBlank Then
                        listabase.Add(palavra) 'adiciona a plavra atual
                        palavra = ""
                    End If
                    listabase.Add(p) 'adiciona a virgula, wrapper ou pontuacao
                'caso for espaco
                Case p = " "
                    If palavra.IsNotBlank Then
                        listabase.Add(palavra) 'adiciona a plavra atual
                        palavra = ""
                    End If
                    palavra = ""
                    'senao, adiciona o proximo caractere a palavra atual
                Case Else
                    palavra  &=  p
            End Select
        Next

        'e entao adiciona ultima palavra se existir
        If palavra.IsNotBlank Then
            listabase.Add(palavra)
            palavra = ""
        End If

        If listabase.Count > 0 Then
            If listabase.Last = "," Then 'se a ultima sentenca for uma virgula, substituimos ela por ponto
                listabase.RemoveAt(listabase.Count - 1)
                listabase.Add(".")
            End If

            'se a ultima sentecao nao for nenhum tipo de pontuacao, adicionamos um ponto a ela
            If Not listabase.Last.IsInAny({EndOfSentencePunctuation, MidSentencePunctuation}) Then
                listabase.Add(".")
            End If

            'processar palavra a palavra
            For index = 0 To listabase.Count - 1
                Me.Add(New SentencePart(listabase(index), Me))
            Next
        Else
            Me.Paragraph.Remove(Me)
        End If
    End Sub

    Public Overrides Function ToString() As String
        Dim sent = ""
        For Each s In Me
            sent &= (s.ToString)
            If s.Next IsNot Nothing Then
                If s.NeedSpaceOnNext Then
                    sent &= (" ")
                End If
            End If
        Next
        Return sent
    End Function

    ReadOnly Property Paragraph As Paragraph

End Class

''' <summary>
''' Parte de uma sentença. Pode ser uma palavra, pontuaçao ou qualquer caractere de encapsulamento
''' </summary>
Public Class SentencePart

    ''' <summary>
    ''' Retorna TRUE se esta parte de senteça for uma palavra
    ''' </summary>
    ''' <returns></returns>
    Function IsWord() As Boolean
        Return Not IsNotWord()
    End Function

    ''' <summary>
    ''' Retorna TRUE se esta parte de senteça não for uma palavra
    ''' </summary>
    ''' <returns></returns>
    Function IsNotWord() As Boolean
        Return IsOpenWrapChar() OrElse IsCloseWrapChar() OrElse IsComma() OrElse IsEndOfSentencePunctuation() OrElse IsMidSentencePunctuation()
    End Function

    ''' <summary>
    ''' Retorna TRUE se esta parte de senteça for um caractere de abertura de encapsulamento
    ''' </summary>
    ''' <returns></returns>
    Function IsOpenWrapChar() As Boolean
        Return OpenWrappers.Contains(Me.Text)
    End Function

    ''' <summary>
    ''' Retorna TRUE se esta parte de senteça for um caractere de fechamento de encapsulamento
    ''' </summary>
    ''' <returns></returns>
    Function IsCloseWrapChar() As Boolean
        Return CloseWrappers.Contains(Me.Text)
    End Function

    ''' <summary>
    ''' Retorna TRUE se esta parte de sentença é uma vírgula
    ''' </summary>
    ''' <returns></returns>
    Function IsComma() As Boolean
        Return Me.Text = ","
    End Function

    ''' <summary>
    ''' Retorna TRUE se esta parte de senteça for um caractere de encerramento de frase (pontuaçao)
    ''' </summary>
    ''' <returns></returns>
    Function IsEndOfSentencePunctuation() As Boolean
        Return EndOfSentencePunctuation.Contains(Me.Text)
    End Function

    ''' <summary>
    ''' Retorna TRUE se esta parte de senteça for um caractere de de meio de sentença (dois pontos ou ponto e vírgula)
    ''' </summary>
    ''' <returns></returns>
    Function IsMidSentencePunctuation() As Boolean
        Return MidSentencePunctuation.Contains(Me.Text)
    End Function

    ''' <summary>
    ''' Retorna TRUE se esta parte de senteça for qualquer tipo de pontuaçao
    ''' </summary>
    ''' <returns></returns>
    Function IsPunctuation() As Boolean
        Return IsEndOfSentencePunctuation() Or IsMidSentencePunctuation()
    End Function

    Friend Sub New(Text As String, Sentence As Sentence)
        Me.Text = Text.Trim
        Me.Sentence = Sentence
    End Sub

    ReadOnly Property Sentence As Sentence

    ''' <summary>
    ''' Texto desta parte de sentença
    ''' </summary>
    ''' <returns></returns>
    Public Property Text As String

    Public Overrides Function ToString() As String

        Dim indexo = Sentence.IndexOf(Me)

        If indexo < 0 Then
            Return ""
        End If

        If indexo = 0 OrElse (indexo = 1 AndAlso OpenWrappers.Contains(Sentence(0).Text)) Then
            Return Text.ToProperCase
        End If

        Return Text
    End Function

    ''' <summary>
    ''' Parte de sentença anterior
    ''' </summary>
    ''' <returns></returns>
    Public Function Previous() As SentencePart
        Return Sentence.IfNoIndex(Sentence.IndexOf(Me) - 1)
    End Function

    ''' <summary>
    ''' Parte da próxima sentença
    ''' </summary>
    ''' <returns></returns>
    Public Function [Next]() As SentencePart
        Return Sentence.IfNoIndex(Sentence.IndexOf(Me) + 1)
    End Function

    ''' <summary>
    ''' Retorna true se é nescessário espaço andes da proxima sentença
    ''' </summary>
    ''' <returns></returns>
    Public Function NeedSpaceOnNext() As Boolean
        Return Me.Next IsNot Nothing AndAlso (Me.Next.IsWord OrElse Me.Next.IsOpenWrapChar)
    End Function

End Class

''' <summary>
''' Texto estruturado (Dividido em parágrafos)
''' </summary>
Public Class StructuredText
    Inherits List(Of Paragraph)

    Public Overrides Function ToString() As String
        Dim par As String = ""
        For Each p In Me
            For i = 1 To p.Ident
                par &= (vbTab)
            Next
            par &= (p.ToString & Environment.NewLine)
        Next
        Return par
    End Function

    Public Sub New(OriginalText As String)
        For Each p In OriginalText.Split(BreakLineChars, StringSplitOptions.RemoveEmptyEntries)
            Me.Add(New Paragraph(p, Me))
        Next
    End Sub

    Public Shared Widening Operator CType(ByVal s As StructuredText) As String
        Return s.ToString()
    End Operator

    Public Shared Widening Operator CType(ByVal s As StructuredText) As Integer
        Return s.Count
    End Operator

    Public Shared Widening Operator CType(ByVal s As StructuredText) As Long
        Return s.LongCount
    End Operator

    Public Shared Operator &(a As StructuredText, b As StructuredText) As StructuredText
        Return New StructuredText(a.ToString() & b.ToString())
    End Operator

    Public Shared Operator +(a As StructuredText, b As StructuredText) As StructuredText
        Return New StructuredText(a.ToString() & b.ToString())
    End Operator

End Class