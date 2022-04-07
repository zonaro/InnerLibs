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
        /// Retorna o nome do diretorio onde o arquivo se encontra
        /// </summary>
        /// <param name="Path">Caminho do arquivo</param>
        /// <returns>o nome do diretório sem o caminho</returns>
        public static string GetLatestDirectoryName(this FileInfo Path) => System.IO.Path.GetDirectoryName(Path.DirectoryName);

        /// <summary>
        /// Retorna o conteudo de um arquivo de texto
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string ReadAllText(this FileInfo File, Encoding encoding = null)
        {
            try
            {
                return System.IO.File.ReadAllText(File.FullName, encoding ?? Encoding.UTF8);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Salva um anexo para um diretório
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="Directory"></param>
        /// <returns></returns>
        public static FileInfo SaveMailAttachment(this System.Net.Mail.Attachment attachment, DirectoryInfo Directory)
        {
            Directory = Directory.FullName.CreateDirectoryIfNotExists();
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
            Path.CreateDirectoryIfNotExists();
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
        public static byte[] ToBytes(this System.Net.Mail.Attachment attachment) => attachment.ContentStream.ToBytes();

        /// <summary>
        /// Converte um stream em Bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            if (stream == null) return Array.Empty<byte>();

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

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
        /// salva um <see cref="Stream"/> em um arquivo
        /// </summary>
        /// <param name="Stream">stream a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos Path. default é <see cref="DateTime.Now"/>
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this Stream Stream, string FilePath, DateTime? DateAndTime = null) => Stream.ToBytes().WriteToFile(FilePath, DateAndTime);

        /// <summary>
        /// alva um Array de Bytes em um arquivo
        /// </summary>
        /// <param name="Bytes">A MAtriz com os Bytes a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos Path. default é <see cref="DateTime.Now"/>
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this byte[] Bytes, string FilePath, DateTime? DateAndTime = null)
        {
            if (FilePath.IsFilePath())
            {
                DateAndTime = DateAndTime ?? DateTime.Now;
                FilePath = FilePath.Replace($"#timestamp#", DateAndTime.Value.Ticks.ToString());
                FilePath = FilePath.Replace($"#datedir#", $@"{DateAndTime.Value.Year}\{DateAndTime.Value.Month}\{DateAndTime.Value.Day}");

                foreach (string item in new[] { "d", "dd", "ddd", "dddd", "hh", "HH", "m", "mm", "M", "MM", "MMM", "MMMM", "s", "ss", "t", "tt", "Y", "YY", "YYY", "YYYY", "f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff" })
                    FilePath = FilePath.SensitiveReplace($"#{item}#", DateAndTime.Value.ToString(item));

                FilePath.CreateDirectoryIfNotExists();
                File.WriteAllBytes(FilePath, Bytes);
                Debug.WriteLine(FilePath, "File Written");
                return new FileInfo(FilePath).With(x => { x.LastWriteTime = DateAndTime.Value; });
            }
            else
            {
                throw new ArgumentException($"FilePath is not a valid file Path: {FilePath}");
            }
        }

        /// <summary>
        /// Transforma um arquivo em um Array de Bytes
        /// </summary>
        /// <param name="File">O arquivo a ser convertido</param>
        /// <returns>Um array do tipo Byte()</returns>
        /// <summary>
        /// Salva um texto em um arquivo
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="FilePath">Caminho do arquivo</param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, string FilePath, bool Append = false, Encoding Enconding = null)
        {
            FilePath.CreateDirectoryIfNotExists();
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
    }
}