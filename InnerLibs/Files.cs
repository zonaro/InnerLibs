using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace InnerLibs
{

    /// <summary>
/// Módulo para criação de arquivos baseados em Array de Bytes()
/// </summary>
/// <remarks></remarks>
    public static class Files
    {

        /// <summary>
    /// Salva um anexo para um diretório
    /// </summary>
    /// <param name="attachment"></param>
    /// <param name="Directory"></param>
    /// <returns></returns>
        public static FileInfo SaveMailAttachment(this System.Net.Mail.Attachment attachment, DirectoryInfo Directory)
        {
            Directory = Directory.FullName.ToDirectoryInfo();
            return attachment.SaveMailAttachment(Directory.FullName + @"\" + attachment.Name.IfBlank(attachment.ContentId));
        }

        /// <summary>
    /// Salva um anexo para um caminho
    /// </summary>
    /// <param name="attachment"></param>
    /// <param name="Path"></param>
    /// <returns></returns>
        public static FileInfo SaveMailAttachment(this System.Net.Mail.Attachment attachment, string Path)
        {
            Path.ToDirectoryInfo();
            if (Path.IsDirectoryPath())
            {
                Path = Path + @"\" + attachment.Name.IfBlank(attachment.ContentId);
            }

            var writer = new BinaryWriter(new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None));
            writer.Write(attachment.ToBytes());
            writer.Close();
            return new FileInfo(Path);
        }

        /// <summary>
    /// Salva um anexo para Byte()
    /// </summary>
    /// <param name="attachment"></param>
    /// <returns></returns>
        public static byte[] ToBytes(this System.Net.Mail.Attachment attachment)
        {
            return attachment.ContentStream.ToBytes();
        }

        /// <summary>
    /// Converte um stream em Bytes
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
    /// Transforma um  Array de Bytes em um arquivo
    /// </summary>
    /// <param name="Bytes">A MAtriz com os Bytes  a ser escrita</param>
    /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
    /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>

        public static FileInfo WriteToFile(this byte[] Bytes, string FilePath)
        {
            if (FilePath.IsFilePath())
            {
                var p = new FileInfo(FilePath);
                p.Directory.FullName.ToDirectoryInfo();
                File.WriteAllBytes(p.FullName, Bytes);
                Debug.WriteLine(FilePath, "File Written");
                return p;
            }

            return null;
        }

        /// <summary>
    /// Transforma um  Array de Bytes em um arquivo
    /// </summary>
    /// <param name="Bytes">A MAtriz com os Bytes  a ser escrita</param>
    /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
    /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this byte[] Bytes, string FilePath, DateTime DateTime)
        {
            FilePath = FilePath.Replace($"#timestamp#", DateTime.Ticks.ToString());
            FilePath = FilePath.Replace($"#datedir#", $@"{DateTime.Year}\{DateTime.Month}\{DateTime.Day}");
            foreach (string item in new[] { "d", "dd", "ddd", "dddd", "hh", "HH", "m", "mm", "M", "MM", "MMM", "MMMM", "s", "ss", "t", "tt", "Y", "YY", "YYY", "YYYY", "f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff" })
                FilePath = FilePath.SensitiveReplace($"#{item}#", DateTime.ToString(item));
            return Bytes.WriteToFile(FilePath).With(x => { x.LastWriteTime = DateTime; });
        }

        /// <summary>
    /// Retorna o conteudo de um arquivo de texto
    /// </summary>
    /// <param name="File">Arquivo</param>
    /// <returns></returns>
        public static string ReadText(this FileInfo File)
        {
            try
            {
                using (var f = File.OpenText())
                {
                    return f.ReadToEnd();
                }
            }
            catch  
            {
                return null;
            }
        }

        /// <summary>
    /// Transforma um arquivo em um Array de Bytes
    /// </summary>
    /// <param name="File">O arquivo a ser convertido</param>
    /// <returns>Um array do tipo Byte()</returns>

        public static byte[] ToBytes(this FileInfo File)
        {
            var fInfo = new FileInfo(File.FullName);
            using (var fStream = new FileStream(File.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var br = new BinaryReader(fStream))
                {
                    return br.ReadBytes((int)fInfo.Length);
                }
            }
        }

        /// <summary>
    /// Salva um texto em um arquivo
    /// </summary>
    /// <param name="Text">TExto</param>
    /// <param name="FilePath">Caminho do arquivo</param>
    /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, string FilePath, bool Append = false, Encoding Enconding = null)
        {
            Path.GetDirectoryName(FilePath).ToDirectoryInfo();
            using (var s = new StreamWriter(FilePath, Append, Enconding ?? new UTF8Encoding(false)))
            {
                s.Write(Text);
                s.Close();
            }

            return new FileInfo(FilePath);
        }

        /// <summary>
    /// Salva um texto em um arquivo
    /// </summary>
    /// <param name="Text">TExto</param>
    /// <param name="File">Arquivo</param>
    /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, FileInfo File, bool Append = false, Encoding Enconding = null)
        {
            return Text.WriteToFile(File.FullName, Append, Enconding);
        }

        /// <summary>
    /// Retorna o nome do diretorio onde o arquivo se encontra
    /// </summary>
    /// <param name="Path">Caminho do arquivo</param>
    /// <returns>o nome do diretório sem o caminho</returns>
        public static string GetLatestDirectoryName(this FileInfo Path)
        {
            return System.IO.Path.GetDirectoryName(Path.DirectoryName);
        }
    }
}