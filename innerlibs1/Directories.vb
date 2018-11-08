Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports System.Web
Imports InnerLibs.LINQ

''' <summary>
''' Funções para trabalhar com diretorios
''' </summary>
''' <remarks></remarks>
Public Module Directories

    ''' <summary>
    ''' Ajusta um caminho de arquivo ou diretório colocando o mesmo <see cref="IO.Path.DirectorySeparatorChar"/> evitando barras duplas ou alternativas
    ''' </summary>
    ''' <param name="Path">String contendo o caminho</param>
    ''' <param name="Alternative">Se TRUE, utiliza <see cref="IO.Path.AltDirectorySeparatorChar"/> ao invés de  <see cref="IO.Path.DirectorySeparatorChar"/> </param>
    ''' <returns></returns>
    <Extension()>
    Public Function FixPathSeparator(Path As String, Optional Alternative As Boolean = False) As String
        Return Path.Split({IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar}, StringSplitOptions.RemoveEmptyEntries).SelectJoin(Function(x) x.Trim, If(Alternative, IO.Path.AltDirectorySeparatorChar, IO.Path.DirectorySeparatorChar))
    End Function

    ''' <summary>
    ''' Deleta um arquivo ou diretório se o mesmo existir e retorna TURE se o arquivo puder ser criado novamente
    ''' </summary>
    ''' <param name="Path">Camingo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function DeleteIfExist(Path As String) As Boolean
        Try
            If Path.IsDirectoryPath Then
                Dim d As New DirectoryInfo(Path)
                If d.Exists Then
                    Try
                        d.Delete(True)
                    Catch ex As Exception
                    End Try
                End If
                Return Not d.Exists
            End If
            If Path.IsFilePath Then
                Dim d As New FileInfo(Path)
                If d.Exists Then
                    Try
                        d.Delete()
                    Catch ex As Exception
                    End Try
                End If
                Return Not d.Exists
            End If
            Return False
        Catch
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Deleta um arquivo ou diretório se o mesmo existir e retorna TRUE se o arquivo puder ser criado novamente
    ''' </summary>
    ''' <param name="Path">Camingo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function DeleteIfExist(Path As FileSystemInfo) As Boolean
        Return DeleteIfExist(Path.FullName)
    End Function

    ''' <summary>
    ''' Cria um diretório se o mesmo nao existir e retorna um DirectoryInfo deste diretório
    ''' </summary>
    ''' <param name="DirectoryName">o nome(s) do(s) diretorio(s) Ex.: "dir1/dir2/dir3" </param>
    ''' <returns>Um DirectoryInfo contendo as informacoes do diretório criado</returns> 
    ''' <remarks>Caso o <paramref name="DirectoryName"/> for um caminho de arquivo, é utilizado o diretório deste aruqivo.</remarks>
    <Extension()>
    Function ToDirectoryInfo(DirectoryName As String) As DirectoryInfo
        If DirectoryName.IsFilePath Then
            DirectoryName = Path.GetDirectoryName(DirectoryName)
        End If
        If Directory.Exists(DirectoryName) = False Then
            Directory.CreateDirectory(DirectoryName)
        End If
        Return New DirectoryInfo(DirectoryName & Path.DirectorySeparatorChar)
    End Function
    ''' <summary>
    ''' Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo
    ''' </summary>
    ''' <param name="FileName">o nome do arquivo Ex.: "dir1/dir2/dir3/file.txt" </param>
    ''' <returns>Um FileInfo contendo as informacoes do arquivo criado</returns>
    <Extension()>
    Function ToFileInfo(FileName As String, Type As FileType) As FileInfo
        Type = If(Type, New FileType(".txt"))
        FileName = Path.GetFullPath(FileName.RemoveAny(Path.GetExtension(FileName))) & Type.Extensions(0)
        If File.Exists(FileName) = False Then
            File.Create(FileName).Dispose()
        End If
        Return New FileInfo(FileName)
    End Function

    ''' <summary>
    ''' Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo
    ''' </summary>
    ''' <param name="FileName">o nome do arquivo Ex.: "dir1/dir2/dir3/file.txt" </param>
    ''' <returns>Um FileInfo contendo as informacoes do arquivo criado</returns>
    <Extension()>
    Function ToFileInfo(FileName As String) As FileInfo
        If File.Exists(FileName) = False Then
            File.Create(FileName).Dispose()
        End If
        Return New FileInfo(FileName)
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
            OutputFile = FilesDirectory.FullName.Replace(FilesDirectory.Name, "") & FilesDirectory.Name
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
            OutputFile = FilesDirectory.FullName.Replace(FilesDirectory.Name, "") & FilesDirectory.Name
        End If
        OutputFile.AppendIf(".zip", Not OutputFile.EndsWith(".zip"))
        For Each arq In FilesDirectory.SearchFiles(SearchOption, Searches)
            Using archive As ZipArchive = ZipFile.Open(OutputFile, If(File.Exists(OutputFile), ZipArchiveMode.Update, ZipArchiveMode.Create))
                Dim arqz = archive.CreateEntryFromFile(arq.FullName, arq.FullName.RemoveAny(FilesDirectory.FullName).ReplaceMany("/", Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), CompressionLevel)
                Debug.WriteLine("Adding: " & arqz.FullName)
            End Using
        Next
        Return New FileInfo(OutputFile)
    End Function

    ''' <summary>
    ''' Extrai um arquivo zip em um diretório
    ''' </summary>
    ''' <param name="File">Arquivo ZIp</param>
    ''' <param name="Directory">Diretório</param>
    ''' <returns></returns>
    <Extension> Function ExtractZipFile(ByVal File As FileInfo, ByVal Directory As DirectoryInfo) As DirectoryInfo
        Directory = (Directory.FullName & Path.DirectorySeparatorChar & Path.GetFileNameWithoutExtension(File.Name) & Path.DirectorySeparatorChar).ToDirectoryInfo
        ZipFile.ExtractToDirectory(File.FullName, Directory.FullName)
        Return Directory.FullName.ToDirectoryInfo
    End Function

    ''' <summary>
    ''' Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
    ''' <returns></returns>
    <Extension>
    Public Function SearchFiles(Directory As DirectoryInfo, SearchOption As SearchOption, ParamArray Searches As String()) As List(Of FileInfo)
        Dim FilteredList As New List(Of FileInfo)
        For Each pattern As String In Searches
            FilteredList.AddRange(Directory.GetFiles(pattern.Trim, SearchOption))
        Next
        Return FilteredList
    End Function

    ''' <summary>
    ''' Retorna uma lista de diretórios baseado em um ou mais padrões de pesquisas
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
    ''' <returns></returns>
    <Extension>
    Public Function SearchDirectories(Directory As DirectoryInfo, SearchOption As SearchOption, ParamArray Searches As String()) As List(Of DirectoryInfo)
        Dim FilteredList As New List(Of DirectoryInfo)
        For Each pattern As String In Searches
            FilteredList.AddRange(Directory.GetDirectories(pattern.Trim, SearchOption))
        Next
        Return FilteredList
    End Function

    ''' <summary>
    ''' Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
    ''' <returns></returns>
    <Extension>
    Public Function Search(Directory As DirectoryInfo, SearchOption As SearchOption, ParamArray Searches As String()) As List(Of FileSystemInfo)
        Dim FilteredList As New List(Of FileSystemInfo)
        For Each pattern As String In Searches
            FilteredList.AddRange(Directory.GetFileSystemInfos(pattern.Trim, SearchOption))
        Next
        Return FilteredList
    End Function

    ''' <summary>
    ''' Retorna uma lista de arquivos ou diretórios baseado em uma busca usando predicate
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="predicate">Funcao LINQ utilizada para a busca</param>
    ''' <param name="SearchOption">Indica se apenas o diretorio atual ou todos os subdiretorios devem ser percorridos pela busca</param>
    ''' <returns></returns>
    <Extension>
    Public Function Find(Of FindType As FileSystemInfo)(Directory As DirectoryInfo, predicate As Func(Of FindType, Boolean), Optional SearchOption As SearchOption = SearchOption.AllDirectories) As IEnumerable(Of FindType)
        Select Case GetType(FindType)
            Case GetType(FileInfo)
                Return Directory.GetFiles("*", SearchOption).Where(predicate)
                Exit Select
            Case GetType(DirectoryInfo)
                Return Directory.GetDirectories("*", SearchOption).Where(predicate)
                Exit Select
            Case Else
                Return Directory.GetFileSystemInfos("*", SearchOption).Where(predicate)
                Exit Select
        End Select
    End Function

    ''' <summary>
    ''' Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
    ''' <param name="FirstDate">Data Inicial</param>
    ''' <param name="SecondDate">Data Final</param>
    ''' <returns></returns>
    <Extension> Function SearchBetween(Directory As DirectoryInfo, FirstDate As DateTime, SecondDate As Date, SearchOption As SearchOption, ParamArray Searches As String()) As List(Of FileSystemInfo)
        FixDateOrder(FirstDate, SecondDate)
        Return Directory.Search(SearchOption, Searches).Where(Function(file) file.LastWriteTime >= FirstDate AndAlso file.LastWriteTime <= SecondDate).OrderByDescending(Function(f) If(f.LastWriteTime.Year <= 1601, f.CreationTime, f.LastWriteTime)).ToList
    End Function

    ''' <summary>
    ''' Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
    ''' <param name="FirstDate">Data Inicial</param>
    ''' <param name="SecondDate">Data Final</param>
    ''' <returns></returns>
    <Extension> Function SearchFilesBetween(Directory As DirectoryInfo, FirstDate As DateTime, SecondDate As Date, SearchOption As SearchOption, ParamArray Searches As String()) As List(Of FileInfo)
        FixDateOrder(FirstDate, SecondDate)
        Return Directory.SearchFiles(SearchOption, Searches).Where(Function(file) file.LastWriteTime.IsBetween(FirstDate, SecondDate)).OrderByDescending(Function(f) If(f.LastWriteTime.Year <= 1601, f.CreationTime, f.LastWriteTime)).ToList
    End Function

    ''' <summary>
    ''' Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
    ''' <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
    ''' <param name="FirstDate">Data Inicial</param>
    ''' <param name="SecondDate">Data Final</param>
    ''' <returns></returns>
    <Extension> Function SearchDirectoriesBetween(Directory As DirectoryInfo, FirstDate As DateTime, SecondDate As Date, SearchOption As SearchOption, ParamArray Searches As String()) As List(Of DirectoryInfo)
        FixDateOrder(FirstDate, SecondDate)
        Return Directory.SearchDirectories(SearchOption, Searches).Where(Function(file) file.LastWriteTime >= FirstDate AndAlso file.LastWriteTime <= SecondDate).OrderByDescending(Function(f) If(f.LastWriteTime.Year <= 1601, f.CreationTime, f.LastWriteTime)).ToList
    End Function




    ''' <summary>
    ''' Copia um diretório para dentro de outro diretório
    ''' </summary>
    ''' <param name="Directory">Diretório</param>
    ''' <param name="DestinationDirectory">Diretório de destino</param>
    ''' <returns></returns>
    <Extension> Public Function CopyTo(Directory As DirectoryInfo, DestinationDirectory As DirectoryInfo) As DirectoryInfo
        If Not DestinationDirectory.Exists Then DestinationDirectory.Create()
        My.Computer.FileSystem.CopyDirectory(Directory.FullName, DestinationDirectory.FullName & Path.DirectorySeparatorChar & Directory.Name)
        Return New DirectoryInfo(DestinationDirectory.FullName & Path.DirectorySeparatorChar & Directory.Name)
    End Function

    ''' <summary>
    ''' Copia arquivos para dentro de outro diretório
    ''' </summary>
    ''' <param name="List">Arquivos</param>
    ''' <param name="DestinationDirectory">Diretório de destino</param>
    ''' <returns></returns>
    <Extension>
    Function CopyTo(List As List(Of FileInfo), DestinationDirectory As DirectoryInfo) As List(Of FileInfo)
        Dim lista As New List(Of FileInfo)
        If Not DestinationDirectory.Exists Then DestinationDirectory.Create()
        For Each file In List
            lista.Add(file.CopyTo(DestinationDirectory.FullName & Path.DirectorySeparatorChar & file.Name))
        Next
        Return lista
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
    Public Sub CleanDirectory(TopDirectory As DirectoryInfo, Optional DeleteTopDirectoryIfEmpty As Boolean = True)
        For Each diretorio In TopDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly)
            For Each subdiretorio In diretorio.GetDirectories()
                CleanDirectory(subdiretorio, True)
            Next
            If diretorio.HasDirectories Then
                CleanDirectory(diretorio, True)
            End If
            If diretorio.IsEmpty Then
                diretorio.Delete()
            End If
        Next
        If DeleteTopDirectoryIfEmpty AndAlso TopDirectory.Exists AndAlso TopDirectory.IsEmpty Then
            TopDirectory.Delete()
        End If
    End Sub

End Module