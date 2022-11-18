using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        /// Format a file path using a <see cref="DateTime"/>
        /// </summary>
        /// <remarks>
        /// You can use any Datetime format (from <see cref="DateTime.ToString(string)"/>) or:
        /// <list type="table">
        /// <term>#timestamp#</term>
        /// <description>Will be replaced with <see cref="DateTime.Ticks"/></description>
        /// <br/>
        /// <term>#datedir#</term>
        /// <description>Will be replaced with a directory path <b>year\month\day</b></description>
        /// <br/>
        /// </list>
        /// </remarks>
        /// <param name="DateAndTime"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string FormatPath(this DateTime? DateAndTime, string FilePath, bool AlternativeChar = false)
        {
            DateAndTime = DateAndTime ?? DateTime.Now;
            FilePath = FilePath.Replace($"#timestamp#", DateAndTime.Value.Ticks.ToString());
            FilePath = FilePath.Replace($"#datedir#", $@"{DateAndTime.Value.Year}\{DateAndTime.Value.Month}\{DateAndTime.Value.Day}");

            foreach (string item in new[] { "d", "dd", "ddd", "dddd", "hh", "HH", "m", "mm", "M", "MM", "MMM", "MMMM", "s", "ss", "t", "tt", "Y", "YY", "YYY", "YYYY", "f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff" })
            {
                FilePath = FilePath.SensitiveReplace($"#{item}#", DateAndTime.Value.ToString(item));
            }

            return FilePath.FixPath(AlternativeChar);
        }


        /// <summary>
        /// Renomeia um arquivo e retorna um <see cref="FileInfo"/> do arquivo renomeado
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Name"></param>
        /// <param name="KeepOriginalExtension"></param>
        /// <returns></returns>
        public static FileInfo Rename(this FileInfo File, string Name, bool KeepOriginalExtension = false)
        {
            if (File != null && Name.IsNotBlank())
            {
                if (KeepOriginalExtension)
                {
                    Name = $"{Path.GetFileNameWithoutExtension(Name)}.{File.Extension.Trim('.')}";
                }

                var pt = Path.Combine(File.DirectoryName, Name);
                File.MoveTo(pt);
                File = new FileInfo(pt);
            }
            return File;
        }

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
        public static string ReadAllText(this FileInfo File, Encoding encoding = null) => File != null && File.Exists ? System.IO.File.ReadAllText(File.FullName, encoding ?? Encoding.UTF8) : InnerLibs.Text.Empty;

        /// <summary>
        /// Salva um anexo para um diretório
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="Directory"></param>
        /// <returns></returns>
        public static FileInfo SaveMailAttachment(this System.Net.Mail.Attachment attachment, DirectoryInfo Directory, DateTime? DateAndTime = null) => attachment.SaveMailAttachment(Directory.FullName, DateAndTime);

        /// <summary>
        /// Salva um anexo para um caminho
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static FileInfo SaveMailAttachment(this System.Net.Mail.Attachment attachment, string FilePath, DateTime? DateAndTime = null)
        {
            if (FilePath.IsDirectoryPath())
            {
                FilePath = FilePath + @"\" + attachment.Name.IfBlank(attachment.ContentId);
            }

            return attachment.ToBytes().WriteToFile(FilePath, DateAndTime);
        }

        /// <summary>
        /// Salva um anexo para Byte()
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this System.Net.Mail.Attachment attachment) => attachment?.ContentStream.ToBytes() ?? Array.Empty<byte>();

        /// <summary>
        /// Converte um stream em Bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            if (stream == null)
            {
                return Array.Empty<byte>();
            }

            var pos = stream.Position;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                stream.Position = pos;
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converte o conteúdo de um <see cref="FileInfo"/> em <see cref="byte[]"/>
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this FileInfo File)
        {
            if (File != null)
            {               
                using (var fStream = new FileStream(File.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var br = new BinaryReader(fStream))
                    {
                        return br.ReadBytes((int)File.Length);
                    }
                }
            }
            return Array.Empty<byte>();
        }

        /// <summary>
        /// salva um <see cref="Stream"/> em um arquivo
        /// </summary>
        /// <param name="Stream">stream a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos FilePath. default é <see cref="DateTime.Now"/>
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this Stream Stream, string FilePath, DateTime? DateAndTime = null) => Stream.ToBytes().WriteToFile(FilePath, DateAndTime);

        /// <summary>
        /// Salva um Array de Bytes em um arquivo
        /// </summary>
        /// <param name="Bytes">A MAtriz com os Bytes a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos FilePath. default é <see cref="DateTime.Now"/> ( <see cref="FormatPath"/>)
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this byte[] Bytes, string FilePath, DateTime? DateAndTime = null)
        {
            Bytes = Bytes ?? Array.Empty<byte>();
            DateAndTime = DateAndTime ?? DateTime.Now;

            FilePath = DateAndTime.FormatPath(FilePath);

            if (FilePath.IsFilePath())
            {
                FilePath.CreateDirectoryIfNotExists();
                if (Bytes.Any())
                {
                    File.WriteAllBytes(FilePath, Bytes);
                    Debug.WriteLine(FilePath, "File Written");
                }
                else
                {
                    Debug.WriteLine("Bytes array is empty", "File not Written");
                }

                return new FileInfo(FilePath).With(x => { x.LastWriteTime = DateAndTime.Value; });
            }
            else
            {
                throw new ArgumentException($"FilePath is not a valid file FilePath: {FilePath}");
            }
        }

        /// <summary>
        /// Salva um array de bytes em um arquivo
        /// </summary>
        /// <param name="File">O arquivo a ser convertido</param>
        /// <returns>Um array do tipo Byte()</returns>
        /// <summary>
        /// Salva um texto em um arquivo
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="FilePath">Caminho do arquivo</param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, string FilePath, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null)
        {
            DateAndTime = DateAndTime ?? DateTime.Now;
            FilePath = DateAndTime.FormatPath(FilePath);
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
        public static FileInfo WriteToFile(this string Text, FileInfo File, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(File.FullName, Append, Enconding, DateAndTime);
        public static FileInfo WriteToFile(this string Text, DirectoryInfo Directory, string FileName, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(Path.Combine(Directory?.FullName, Path.GetFileName(FileName)), Append, Enconding, DateAndTime);
        public static FileInfo WriteToFile(this string Text, DirectoryInfo Directory, string SubDirecotry, string FileName, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(Path.Combine(Directory?.FullName, SubDirecotry, Path.GetFileName(FileName)), Append, Enconding, DateAndTime);
    }
}