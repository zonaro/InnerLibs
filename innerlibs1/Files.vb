
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Web
''' <summary>
''' Módulo para criação de arquivos baseados em Array de Bytes() 
''' </summary>
''' <remarks></remarks>
Public Module Files
    ''' <summary>
    ''' Transforma um  Array de Bytes em um arquivo
    ''' </summary>
    ''' <param name="Bytes">A MAtriz com os Bytes  a ser escrita</param>
    ''' <param name="FilePath">Caminho onde o arquivo será gravado</param>
    ''' <returns>Um Fileinfo contendo as informações do arquivo criado</returns>

    <Extension()>
    Public Function WriteToFile(Bytes As Byte(), FilePath As String) As FileInfo
        Path.GetDirectoryName(FilePath).ToDirectory()
        File.WriteAllBytes(FilePath, Bytes)
        Return New FileInfo(FilePath)
    End Function

    ''' <summary>
    ''' Retorna o conteudo de um arquivo de texto
    ''' </summary>
    ''' <param name="File">Arquivo</param>
    ''' <returns></returns>
    <Extension()>
    Public Function ReadText(File As FileInfo) As String
        Using f = File.OpenText
            Return f.ReadToEnd
        End Using
    End Function

    ''' <summary>
    ''' Transforma um arquivo em um Array de Bytes
    ''' </summary>
    ''' <param name="File">O arquivo a ser convertido</param>
    ''' <returns>Um array do tipo Byte()</returns>

    <Extension()>
    Public Function ToBytes(File As FileInfo) As Byte()
        Dim fInfo As New FileInfo(File.FullName)
        Dim numBytes As Long = fInfo.Length
        Dim fStream As New FileStream(File.FullName, FileMode.Open, FileAccess.Read)
        Dim br As New BinaryReader(fStream)
        Return br.ReadBytes(CInt(numBytes))
        br.Close()
        fStream.Close()
    End Function

    ''' <summary>
    ''' Transforma um HttpPostedFile em array de bytes
    ''' </summary>
    ''' <param name="File">Arquivo postado</param>
    ''' <returns>Um array do tipo Byte()</returns>
    <Extension> Public Function ToBytes(File As HttpPostedFile) As Byte()
        Dim br As New BinaryReader(File.InputStream)
        Return br.ReadBytes(CInt(File.InputStream.Length))
    End Function





    ''' <summary>
    ''' Salva um texto em um arquivo
    ''' </summary>
    ''' <param name="Text">TExto</param>
    ''' <param name="FilePath">Caminho do arquivo</param>
    ''' <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
    <Extension()>
    Public Function WriteToFile(Text As String, FilePath As String, Optional Append As Boolean = False) As FileInfo
        Path.GetDirectoryName(FilePath).ToDirectory()
        Using s As New StreamWriter(FilePath, Append, System.Text.Encoding.UTF8)
            s.Write(Text)
            s.Close()
        End Using
        Return New FileInfo(FilePath)
    End Function

    ''' <summary>
    ''' Retorna o nome do diretorio onde o arquivo se encontra
    ''' </summary>
    ''' <param name="Path">Caminho do arquivo</param>
    ''' <returns>o nome do diretório sem o caminho</returns>
    <Extension()> Public Function GetLatestDirectoryName(Path As FileInfo)
        Return IO.Path.GetDirectoryName(Path.DirectoryName)
    End Function
End Module