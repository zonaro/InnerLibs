Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Web

''' <summary>
''' Módulo para criação de arquivos baseados em Array de Bytes()
''' </summary>
''' <remarks></remarks>
Public Module Files


    ''' <summary>
    ''' Salva um anexo para um diretório    
    ''' </summary>
    ''' <param name="attachment"></param>
    ''' <param name="Directory"></param>
    ''' <returns></returns>
    <Extension()> Public Function SaveMailAttachment(ByVal attachment As System.Net.Mail.Attachment, Directory As DirectoryInfo) As FileInfo
        Directory = Directory.FullName.ToDirectoryInfo
        Return SaveMailAttachment(attachment, Directory.FullName & "\" & attachment.Name.IfBlank(attachment.ContentId))
    End Function

    ''' <summary>
    ''' Salva um anexo para um caminho    
    ''' </summary>
    ''' <param name="attachment"></param>
    ''' <param name="Path"></param>
    ''' <returns></returns>
    <Extension()> Public Function SaveMailAttachment(ByVal attachment As System.Net.Mail.Attachment, Path As String) As FileInfo
        Path.ToDirectoryInfo
        If Path.IsDirectoryPath Then
            Path = Path & "\" & attachment.Name.IfBlank(attachment.ContentId)
        End If
        Dim writer As BinaryWriter = New BinaryWriter(New FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        writer.Write(attachment.ToBytes)
        writer.Close()
        Return New FileInfo(Path)
    End Function


    ''' <summary>
    ''' Salva um anexo para Byte()    
    ''' </summary>
    ''' <param name="attachment"></param>
    ''' <returns></returns>
    <Extension()> Public Function ToBytes(ByVal attachment As System.Net.Mail.Attachment) As Byte()
        Return attachment.ContentStream.ToBytes
    End Function

    ''' <summary>
    ''' Converte um stream em Bytes
    ''' </summary>
    ''' <param name="stream"></param>
    ''' <returns></returns>
    <Extension()>
    Public Function ToBytes(ByVal stream As Stream) As Byte()
        Dim totalbytes As Byte() = Nothing
        Dim iBytesRead As Integer
        Dim Totalread As Int64 = 0
        ReDim totalbytes(stream.Length)
        Do
            iBytesRead = stream.Read(totalbytes, Totalread, Totalread + 1048576)
            Totalread += iBytesRead
        Loop While Not iBytesRead = 0
        stream.Close()
        Return totalbytes
    End Function

    ''' <summary>
    ''' Transforma um  Array de Bytes em um arquivo
    ''' </summary>
    ''' <param name="Bytes">A MAtriz com os Bytes  a ser escrita</param>
    ''' <param name="FilePath">Caminho onde o arquivo será gravado</param>
    ''' <returns>Um Fileinfo contendo as informações do arquivo criado</returns>

    <Extension()>
    Public Function WriteToFile(Bytes As Byte(), FilePath As String) As FileInfo
        Dim p = New FileInfo(FilePath)
        p.Directory.FullName.ToDirectoryInfo()
        File.WriteAllBytes(p.FullName, Bytes)
        Return p
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
        Using fStream As New FileStream(File.FullName, FileMode.Open, FileAccess.Read)
            Using br As New BinaryReader(fStream)
                Return br.ReadBytes(CInt(fInfo.Length))
            End Using
        End Using
    End Function

    ''' <summary>
    ''' Transforma um HttpPostedFile em array de bytes
    ''' </summary>
    ''' <param name="File">Arquivo postado</param>
    ''' <returns>Um array do tipo Byte()</returns>
    <Extension> Public Function ToBytes(File As HttpPostedFile) As Byte()
        Try
            Dim br As New BinaryReader(File.InputStream)
            Return br.ReadBytes(CInt(File.InputStream.Length))
        Catch ex As Exception
            Return {}
        End Try
    End Function

    ''' <summary>
    ''' Salva um texto em um arquivo
    ''' </summary>
    ''' <param name="Text">TExto</param>
    ''' <param name="FilePath">Caminho do arquivo</param>
    ''' <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
    <Extension()>
    Public Function WriteToFile(Text As String, FilePath As String, Optional Append As Boolean = False) As FileInfo
        Path.GetDirectoryName(FilePath).ToDirectoryInfo()
        Using s As New StreamWriter(FilePath, Append, System.Text.Encoding.UTF8)
            s.Write(Text)
            s.Close()
        End Using
        Return New FileInfo(FilePath)
    End Function

    ''' <summary>
    ''' Salva um texto em um arquivo
    ''' </summary>
    ''' <param name="Text">TExto</param>
    ''' <param name="File">Arquivo</param>
    ''' <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
    <Extension()>
    Public Function WriteToFile(Text As String, File As FileInfo, Optional Append As Boolean = False) As FileInfo
        Return Text.WriteToFile(File.FullName, Append)
    End Function

    ''' <summary>
    ''' Retorna o nome do diretorio onde o arquivo se encontra
    ''' </summary>
    ''' <param name="Path">Caminho do arquivo</param>
    ''' <returns>o nome do diretório sem o caminho</returns>
    <Extension()> Public Function GetLatestDirectoryName(Path As FileInfo) As String
        Return IO.Path.GetDirectoryName(Path.DirectoryName)
    End Function




End Module