Imports System.Runtime.CompilerServices

Public Module BeautyStrings

    <Extension()> Public Function ListInFrameCssComment(Lines As List(Of String)) As String
        Return Lines.ListInFrame().Wrap(Environment.NewLine).CSSComment()
    End Function

    <Extension()> Public Function CSSComment(Text As String) As String
        Return $"/* {Text} */"
    End Function

    <Extension> Public Function PropertiesInFrame(Of T As Class)(obj As T) As String
        Dim l As New List(Of String)
        If obj IsNot Nothing Then

            For Each item In obj.GetProperties()
                If item.CanRead Then
                    l.Add($"{item.Name}: {item.GetValue(obj)}")
                End If
            Next

        End If
        Return l.ListInFrame()
    End Function

    <Extension> Public Function FieldsInFrame(Of T As Class)(obj As T) As String
        Dim l As New List(Of String)
        If obj IsNot Nothing Then
            For Each item In GetNullableTypeOf(obj).GetFields()
                l.Add($"{item.Name}: {item.GetValue(obj)}")
            Next

        End If
        Return l.ListInFrame()
    End Function

    <Extension> Public Function PairsInFrame(Of K, V)(obj As IDictionary(Of K, V)) As String
        Dim l As New List(Of String)
        If obj IsNot Nothing Then
            For Each item In obj
                l.Add($"{item.Key}: {item.Value}")
            Next
        End If
        Return l.ListInFrame()
    End Function

    <Extension()> Public Function ListInFrame(Lines As List(Of String)) As String

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