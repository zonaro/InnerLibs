
Imports System.Data.Common
Imports System.IO
Imports System.Text.RegularExpressions

Public NotInheritable Class Templetizer

    Sub New(DataBase As DataBase, TemplateFolder As DirectoryInfo)
        Me.TemplateFolder = TemplateFolder
        Me.DataBase = DataBase
    End Sub

    Property DataBase As DataBase

    Property TemplateFolder As DirectoryInfo


    Function Templetize(ProcedureName As String) As String
        Dim response As String = ""
        Dim template As String = ""
        Dim header As String = ""
        Using file As StreamReader = New FileInfo(ProcedureName).OpenText
            Dim content As String = file.ReadToEnd
            header = content.GetTagContent("head")
            template = content.GetTagContent("body")
        End Using

        Using reader As DbDataReader = DataBase.RunSQL(ProcedureName)
            While reader.Read
                Dim copia As String = template

                For Each proc As String In New Regex("<procedure>([^>]*)<\/procedure>").Matches(copia)
                    copia = copia.Replace("<procedure>" & proc & "</procedure>", Templetize(proc))
                Next
                For Each col In reader.GetColumns
                    copia = copia.Replace("##" & col & "##", reader(col))
                Next
                response.Append(copia)
            End While
        End Using
        response.Append(header)
        Return response
    End Function

End Class
