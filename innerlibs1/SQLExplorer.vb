Imports System.IO
Imports System.Text.RegularExpressions

Namespace SQLExplorer

    Public Module SQLExplorer
        ''' <summary>
        ''' Busca arquivos em um diretório utilizando SQL
        ''' </summary>
        ''' <param name="SQLQuery">Comando SQL</param>
        ''' <returns>Uma Lista com todos os caminhos Encontrados</returns>
        Public Function SQLDir(SQLQuery As String) As List(Of FileSystemInfo)
            SQLQuery = SQLQuery.AdjustWhiteSpaces
            Dim result As New List(Of FileSystemInfo)
            Dim wheres As String = ""
            Dim diretorio As DirectoryInfo
            Dim campos As String
            Dim intofile As String = ""
            Try
                Select Case True
                    Case SQLQuery.StartsWith("SELECT", StringComparison.CurrentCultureIgnoreCase)
                        Dim r As Match
                        If SQLQuery.Contains("WHERE") Then
                            r = New Regex("SELECT\s(.*)FROM\s(.*)WHERE\s([^>]*)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Match(SQLQuery)
                            wheres = r.Groups(3).Value
                            '' fazer o parse do where
                        Else
                            r = New Regex("SELECT\s(.*)FROM\s(.*)", RegexOptions.Singleline + RegexOptions.IgnoreCase).Match(SQLQuery)
                        End If
                        Try
                            intofile = r.Groups(2).Value.Split({" INTO ", " into "}, StringSplitOptions.RemoveEmptyEntries)(1)
                        Catch ex As Exception
                        End Try
                        Try
                            diretorio = New DirectoryInfo(r.Groups(2).Value.Split({" INTO ", " into "}, StringSplitOptions.RemoveEmptyEntries)(0))
                        Catch ex As Exception
                            diretorio = New DirectoryInfo(r.Groups(2).Value)
                        End Try
                        campos = r.Groups(1).Value

                        campos = "" & campos.AdjustWhiteSpaces

                        If campos.StartsWith("DIRECTORY ") Then
                            Dim searches = campos.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                            result.AddRange(diretorio.SearchDirectories(campos))
                        Else
                            result.AddRange(diretorio.SearchDirectories(campos))
                            diretorio.Search(SearchOption.AllDirectories, campos.Split({","}, StringSplitOptions.RemoveEmptyEntries))
                        End If

                    Case SQLQuery.StartsWith("CREATE", StringComparison.CurrentCultureIgnoreCase)
                    Case SQLQuery.StartsWith("DROP", StringComparison.CurrentCultureIgnoreCase)
                    Case SQLQuery.StartsWith("DELETE", StringComparison.CurrentCultureIgnoreCase)
                End Select
            Catch ex As Exception

            End Try
            If intofile.IsNotBlank Then
                Select Case True
                    Case intofile.IsDirectory
                        For Each f As FileSystemInfo In result
                            If f.FullName.IsDirectory Then

                            Else

                            End If
                        Next
                    Case New FileInfo(intofile).Extension = ".json"
                        result.SerializeJSON.WriteToFile(intofile)

                End Select
            End If
            Return result
        End Function

    End Module

End Namespace
