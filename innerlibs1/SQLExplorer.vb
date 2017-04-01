Imports System.Collections.ObjectModel
Imports System.IO
Imports System.IO.Compression
Imports System.Text.RegularExpressions

Namespace SQLExplorer

    ''' <summary>
    ''' Busca arquivos em um diretório utilizando SQL
    ''' </summary>
    Public Class FileSystemSQL
        Inherits List(Of FileSystemInfo)

        Public Sub New(SQLQuery As String)
            SQLQuery = SQLQuery.AdjustWhiteSpaces
            Dim wheres As String = ""
            Dim diretorio As DirectoryInfo
            Dim intofile As String = ""
            Try
                Select Case True
                    Case SQLQuery.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase)
                        Dim r As Match
                        Select Case True
                            Case SQLQuery.ContainsAny(" WHERE ", " where ") And SQLQuery.ContainsAny(" INTO ".Quote, " into ".Quote)
                                r = New Regex("SELECT\s(.*)FROM\s(.*)INTO\s(.*)WHERE\s([^>]*)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Match(SQLQuery)
                                intofile = r.Groups(3).Value.GetQuotedText(True).First
                                diretorio = New DirectoryInfo(r.Groups(2).Value.GetQuotedText(True).First)
                                wheres = r.Groups(4).Value
                                '' fazer o parse do where
                                Exit Select
                            Case SQLQuery.ContainsAny(" INTO ".Quote, " into ".Quote)
                                r = New Regex("SELECT\s(.*)FROM\s(.*)INTO\s(.*)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Match(SQLQuery)
                                intofile = r.Groups(3).Value.GetQuotedText(True).First
                                diretorio = New DirectoryInfo(r.Groups(2).Value.GetQuotedText(True).First)
                                Exit Select
                            Case SQLQuery.ContainsAny(" WHERE ", " where ")
                                r = New Regex("SELECT\s(.*)FROM\s(.*)WHERE\s([^>]*)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Match(SQLQuery)
                                diretorio = New DirectoryInfo(r.Groups(2).Value.GetQuotedText(True).First)
                                wheres = r.Groups(3).Value
                                '' fazer o parse do where
                                Exit Select
                            Case Else
                                r = New Regex("SELECT\s(.*)FROM\s(.*)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Match(SQLQuery)
                                diretorio = New DirectoryInfo(r.Groups(2).Value.GetQuotedText(True).First)
                        End Select

                        Dim searching As SearchOption
                        Dim method As Integer
                        Select Case True
                            Case r.Groups(1).Value.AdjustWhiteSpaces = "ONLY TOP FILE"
                                searching = SearchOption.TopDirectoryOnly
                                method = 1
                            Case r.Groups(1).Value.AdjustWhiteSpaces = "ONLY TOP DIRECTORY"
                                searching = SearchOption.TopDirectoryOnly
                                method = 2
                            Case r.Groups(1).Value.AdjustWhiteSpaces = "ONLY TOP EVERYTHING", "ONLY TOP *"
                                searching = SearchOption.TopDirectoryOnly
                                method = 3
                            Case r.Groups(1).Value.AdjustWhiteSpaces = "ALL FILE"
                                searching = SearchOption.AllDirectories
                                method = 1
                            Case r.Groups(1).Value.AdjustWhiteSpaces = "ALL DIRECTORY"
                                searching = SearchOption.AllDirectories
                                method = 2
                            Case r.Groups(1).Value.AdjustWhiteSpaces = "ALL EVERYTHING", "*", "ALL *"
                                searching = SearchOption.AllDirectories
                                method = 3
                            Case Else
                                Throw New Exception("Wrong syntax at " & r.Groups(1).Value.AdjustWhiteSpaces.Quote)
                        End Select

                        Dim campos = ""

                        Select Case method
                            Case 1
                                Me.AddRange(diretorio.SearchFiles(searching, campos))
                            Case 2
                                Me.AddRange(diretorio.SearchDirectories(searching, campos))
                            Case 3
                                Me.AddRange(diretorio.Search(searching, campos))
                        End Select

                        wheres = wheres.AdjustWhiteSpaces.Replace("'", """")

                        For Each m As Match In New Regex("(.*?) = ""(.*?)""").Matches(wheres)
                            Dim param_name = m.Groups(1).Value
                            Dim param_value = m.Groups(2).Value
                            Dim templist As New List(Of FileSystemInfo)
                            'terminar

                        Next





                        If intofile.IsNotBlank Then
                            Select Case True
                                Case intofile.IsDirectory
                                    For Each f As FileSystemInfo In Me
                                        If f.FullName.IsDirectory Then
                                            Directories.CopyTo(New DirectoryInfo(f.FullName), intofile.ToDirectory)
                                        Else
                                            File.Copy(f.FullName, intofile.ToDirectory.FullName & "\" & f.Name)
                                        End If
                                    Next
                                Case New FileInfo(intofile).Extension.IsAny(".zip")
                                    Using memoryStream = New MemoryStream()
                                        Using archive = New ZipArchive(memoryStream, ZipArchiveMode.Create, True)
                                            For Each f As FileSystemInfo In Me
                                                If Not f.FullName.IsDirectory Then
                                                    Dim arqz = archive.CreateEntryFromFile(f.FullName, f.FullName.RemoveAny(diretorio.FullName).Replace("\", "/"), CompressionLevel.Fastest)
                                                End If
                                            Next
                                        End Using
                                        Using fileStream = New FileStream(intofile, FileMode.Create)
                                            memoryStream.Seek(0, SeekOrigin.Begin)
                                            memoryStream.CopyTo(fileStream)
                                        End Using
                                    End Using
                                Case New FileInfo(intofile).Extension.IsAny(".json", ".txt")
                                    Me.SerializeJSON.WriteToFile(intofile)
                            End Select
                        End If
                    Case SQLQuery.StartsWith("DROP", StringComparison.CurrentCultureIgnoreCase), SQLQuery.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase)

                End Select
            Catch ex As Exception

            End Try
        End Sub




    End Class

End Namespace
