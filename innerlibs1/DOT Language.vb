
Imports System.Runtime.CompilerServices

Public Module DotNodeExtensions

    <Extension> Public Function Compile(List As List(Of DotNode)) As String
        Return "digraph " & List.Select(Function(n) n.ToString & Environment.NewLine).ToArray.Join("").Replace(";;", ";").Wrap("{")
    End Function


End Module

Public Class DotNode
    Inherits Dictionary(Of String, Func(Of Object, String))



    Sub New(Reference As Object, ID As Func(Of Object, Object))
        Me.Reference = Reference
        Me.ID = ID
    End Sub

    Property Reference As Object

    Property ID As Func(Of Object, Object)

    Property Children As New List(Of DotNode)

    Public Overrides Function ToString() As String
        Dim dotstring = ""
        For Each prop In Me
            dotstring &= ID(Reference).ToString.ToSlug(True) & " " & (prop.Key & "=" & prop.Value(Reference).Quote).Wrap("[").Append(";" & Environment.NewLine)
        Next
        For Each c In Children
            dotstring.Append(ID(Reference).ToString.ToSlug(True) & " -> " & c.ID(c.Reference).ToString.ToSlug(True) & ";" & Environment.NewLine)
            dotstring.Append(c.ToString)
        Next
        Return dotstring
    End Function



End Class
