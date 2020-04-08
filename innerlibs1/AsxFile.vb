

Imports System.IO
Imports System.Text
Imports System.Web

Public Class AsxFile

    Property Title As String

    Property Entries As New List(Of AsxEntry)

    Public Sub New(Title As String)
        Me.Title = Title
    End Sub


    Function AddEntry(Title As String, Url As Uri, Optional Author As String = "", Optional Copyright As String = "") As AsxEntry
        Dim e As New AsxEntry
        e.Title = Title
        e.URL = Url
        e.Author = Author
        e.Copyright = Copyright
        Entries.Add(e)
        Return e
    End Function

    Public Overrides Function ToString() As String
        Dim Variable As String = ""
        Variable  &=  "<asx version=""3.0"">"
        Variable  &=  "  <title>" & Me.Title & "</title>"
        For Each item In Entries
            Variable  &=  "  <entry>"
            Variable  &=  "    <title>" & item.Title & "</title>"
            Variable  &=  "    <ref href=" & item.URL.ToString & " />"
            'Dim nv = HttpUtility.ParseQueryString(item.URL.Query)
            'For Each p As String In nv
            '    Variable  &=  "    <param name=" & p.Quote & " value=" & nv(p).Quote & " />"
            'Next
            If item.Author.IsNotBlank Then
                Variable  &=  "  <author>" & item.Author & "</author>"
            End If
            If item.Copyright.IsNotBlank Then
                Variable  &=  "  <copyright>" & item.Copyright & "</copyright>"
            End If
            Variable  &=  "  </entry>"
        Next
        Variable  &=  "</asx>"
        Variable  &=  ""
        Return Variable
    End Function

    Function AsBytes(Optional Encoding As Encoding = Nothing) As Byte()
        Encoding = If(Encoding, Encoding.UTF8)
        Return Encoding.GetBytes(ToString)
    End Function

    Function WriteFile(FilePath As String) As FileInfo
        Return AsBytes.WriteToFile(FilePath)
    End Function

End Class

Public Class AsxEntry
    Property Title As String
    Property URL As Uri
    Property Author As String
    Property Copyright As String
End Class
