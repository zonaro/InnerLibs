Imports System.Runtime.CompilerServices

Public Module BeautyStrings

    <Extension()> Public Function BoxTextCSS(Text As String) As String
        Return $"/*{Text.BoxText().Wrap(Environment.NewLine)}*/"
    End Function

    <Extension()> Public Function BoxText(Text As String) As String
        Dim Lines = Text.SplitAny(BreakLineChars.ToArray()).ToList()
        Dim linha_longa = ""
        Dim charcount = Lines.Max(Function(x) x.Length)

        If charcount.IsEven() Then
            charcount = charcount + 1
        End If

        For i As Integer = 0 To Lines.Count() - 1
            Lines(i) = Lines(i).PadRight(charcount)
        Next

        For i As Integer = 0 To Lines.Count() - 1
            Lines(i) = $"* {Lines(i)} *"
        Next

        charcount = Lines.Max(Function(x) x.Length)

        While linha_longa.Length < charcount
            linha_longa = linha_longa & "* "
        End While

        linha_longa = linha_longa.Trim()

        Lines.Insert(0, linha_longa)
        Lines.Add(linha_longa)

        Dim box = Lines.Join(Environment.NewLine)
        Return box
    End Function

End Module