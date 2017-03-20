

Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
''' <summary>
''' Funções para trabalhar com diretorios
''' </summary>
''' <remarks></remarks>
Public Module Directories
    ''' <summary>
    ''' Cria um diretório se o mesmo nao existir e retorna um DirectoryInfo deste diretório
    ''' </summary>
    ''' <param name="DirectoryName">o nome(s) do(s) diretorio(s) Ex.: "dir1/dir2/dir3" </param>
    ''' <returns>Uma DirectoryInfo contendo as informacoes do diretório criado</returns>

    <Extension()>
    Function ToDirectory(DirectoryName As String) As DirectoryInfo
        DirectoryName = Path.GetFullPath(DirectoryName)
        If Directory.Exists(DirectoryName) = False Then
            Directory.CreateDirectory(DirectoryName)
        End If
        Return New DirectoryInfo(DirectoryName & "\")
    End Function

    ''' <summary>
    ''' Cria um arquivo .ZIP de um diretório
    ''' </summary>
    ''' <param name="FilesDirectory">Um diretório contendo arquivos</param>
    ''' <param name="OutputFile">O caminho onde será exportado o arquivo ZIP, Mesmo caminho do diretório se não especificado</param>
    ''' <param name="CompressionLevel">Nivel de compressão do arquivo Zip</param>
    ''' <returns>Um FileInfo contendo as informações do arquivo gerado</returns>

    <Extension()>
    Public Function ToZipFile(FilesDirectory As DirectoryInfo, OutputFile As String, Optional CompressionLevel As CompressionLevel = CompressionLevel.Optimal) As FileInfo
        If OutputFile.IsBlank Then
            OutputFile = FilesDirectory.FullName.Replace(FilesDirectory.Name, "") + FilesDirectory.Name
        End If
        OutputFile.AppendIf(".zip", Not OutputFile.EndsWith(".zip"))
        ZipFile.CreateFromDirectory(FilesDirectory.FullName, OutputFile, CompressionLevel, True)
        Return New FileInfo(OutputFile)
    End Function

    ''' <summary>
    ''' Cria um arquivo .ZIP a partir de arquivos selecionados de uma pesquisa em um diretório
    ''' </summary>
    ''' <param name="FilesDirectory">Um diretório contendo arquivos</param>
    ''' <param name="OutputFile">O caminho onde será exportado o arquivo ZIP, Mesmo caminho do diretório se não especificado</param>
    ''' <param name="SearchOption">Especifica se a busca ocorrerá apenas no diretório pai ou em subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.extensao, nome.*, *nome*.*, *.*)</param>
    ''' <param name="CompressionLevel">Nivel de compressão do arquivo Zip</param>
    ''' <returns></returns>
    <Extension()> Public Function ToZipFile(FilesDirectory As DirectoryInfo, OutputFile As String, CompressionLevel As CompressionLevel, SearchOption As SearchOption, ParamArray Searches As String()) As FileInfo
        If OutputFile.IsBlank Then
            OutputFile = FilesDirectory.FullName.Replace(FilesDirectory.Name, "") + FilesDirectory.Name
        End If
        OutputFile.AppendIf(".zip", Not OutputFile.EndsWith(".zip"))
        If File.Exists(OutputFile) Then File.Delete(OutputFile)
        For Each arq In FilesDirectory.Search(SearchOption, Searches)
            Using archive As ZipArchive = ZipFile.Open(OutputFile, If(File.Exists(OutputFile), ZipArchiveMode.Update, ZipArchiveMode.Create))
                Dim arqz = archive.CreateEntryFromFile(arq.FullName, arq.FullName.RemoveAny(FilesDirectory.FullName).Replace("\", "/"), CompressionLevel)
                Debug.WriteLine("Adding: " & arqz.FullName)
            End Using
        Next
        Return New FileInfo(OutputFile)
    End Function


    ''' <summary>
    ''' Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
    ''' <returns></returns>
    <Extension>
    Public Function Search(Directory As DirectoryInfo, SearchOption As SearchOption, ParamArray Searches As String()) As List(Of FileInfo)
        Dim FilteredList As New List(Of FileInfo)
        For Each pattern As String In Searches
            FilteredList.AddRange(Directory.GetFiles(pattern.Trim, SearchOption))
        Next
        Return FilteredList
    End Function

    ''' <summary>
    ''' Verifica se um diretório possui arquivos
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <returns></returns>
    <Extension>
    Public Function HasFiles(Directory As DirectoryInfo) As Boolean
        Return Directory.GetFiles.Count > 0
    End Function

    ''' <summary>
    ''' Verifica se um diretório possui subdiretórios
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <returns></returns>
    <Extension>
    Public Function HasDirectories(Directory As DirectoryInfo) As Boolean
        Return Directory.GetDirectories.Count > 0
    End Function

    ''' <summary>
    ''' Verifica se um diretório está vazio
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <returns></returns>
    <Extension>
    Public Function IsEmpty(Directory As DirectoryInfo) As Boolean
        Return Not Directory.HasFiles And Not Directory.HasDirectories
    End Function

    ''' <summary>
    ''' Verifica se um diretório não está vazio
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <returns></returns>
    <Extension>
    Public Function IsNotEmpty(Directory As DirectoryInfo) As Boolean
        Return Not Directory.IsEmpty
    End Function

    ''' <summary>
    ''' Remove todos os subdiretorios vazios
    ''' </summary>
    ''' <param name="TopDirectory">Diretorio da operação</param>
    <Extension>
    Public Sub CleanDirectory(TopDirectory As DirectoryInfo)
        For Each diretorio In TopDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly)
            For Each subdiretorio In diretorio.GetDirectories()
                CleanDirectory(subdiretorio)
            Next
            If diretorio.HasDirectories Then
                CleanDirectory(diretorio)
            End If
            If diretorio.IsEmpty Then
                diretorio.Delete()
            End If
        Next
        If TopDirectory.Exists And TopDirectory.IsEmpty Then
            TopDirectory.Delete()
        End If
    End Sub




End Module