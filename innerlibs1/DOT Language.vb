
Imports System.Runtime.CompilerServices

Public Module DotNodeExtensions

    <Extension> Public Function Compile(List As List(Of DotNode), Optional Type As String = "digraph", Optional Name As String = "") As String
        Dim s = List.Select(Function(n) n.ToString & Environment.NewLine).ToArray.Join("")
        s = s.Split(Environment.NewLine).Distinct.Join(Environment.NewLine) & Environment.NewLine
        Return Type & " " & Name.ToSlug(True) & " " & s.Wrap("{")
    End Function


End Module

''' <summary>
''' Representa um nó de um grafico em DOT Language
''' </summary>
Public Class DotNode
    Inherits Dictionary(Of String, String)

    Sub New(ID As String)
        Me.ID = ID
    End Sub



    Public Property ID As String

    Public Property Relations As New Dictionary(Of DotEge, DotNode)

    Public Overrides Function ToString() As String
        Dim dotstring As String = ""
        For Each prop In Me
            dotstring &= ID.ToString.ToSlug(True) & " " & (prop.Key & "=" & prop.Value.Quote).Wrap("[").Append(";" & Environment.NewLine)
        Next
        For Each c In Relations
            dotstring.Append(ID.ToSlug(True) & If(c.Key.Oriented, " -> ", " -- ") & c.Value.ID.ToSlug(True) & " " & c.Key.ToString & ";")
            dotstring.Append(Environment.NewLine)
            dotstring.Append(c.Value.ToString)
        Next
        Return dotstring
    End Function



End Class

''' <summary>
''' Representa uma ligação entre nós de um grafico em DOT Language
''' </summary>
Public Class DotEge
    Inherits Dictionary(Of String, String)

    Sub New(Optional Oriented As Boolean = True)
        Me.Oriented = Oriented
    End Sub

    Property Oriented As Boolean = True

    Property Group As Boolean = True

    Public Overrides Function ToString() As String
        Dim propstring = ""
        If Me.Count > 0 Then
            For Each prop In Me
                propstring &= prop.Key & "=" & prop.Value.Quote & " "
            Next
            propstring = propstring.Trim.Wrap("[")
        End If
        Return propstring
    End Function

End Class
