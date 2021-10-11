using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using InnerLibs.LINQ;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{

    /// <summary>
    /// Funções para trabalhar com diretorios
    /// </summary>
    /// <remarks></remarks>
    public static class Directories
    {

        /// <summary>
        /// Ajusta um caminho de arquivo ou diretório colocando o mesmo <see cref="IO.Path.DirectorySeparatorChar"/> evitando barras duplas ou alternativas
        /// </summary>
        /// <param name="Path">String contendo o caminho</param>
        /// <param name="Alternative">Se TRUE, utiliza <see cref="IO.Path.AltDirectorySeparatorChar"/> ao invés de  <see cref="IO.Path.DirectorySeparatorChar"/> </param>
        /// <returns></returns>
        public static string FixPathSeparator(this string Path, bool Alternative = false)
        {
            return Path.Split(new[] { System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).SelectJoin(x => x.Trim(), Conversions.ToString(Alternative ? System.IO.Path.AltDirectorySeparatorChar : System.IO.Path.DirectorySeparatorChar));
        }

        /// <summary>
        /// Deleta um arquivo ou diretório se o mesmo existir e retorna TURE se o arquivo puder ser criado novamente
        /// </summary>
        /// <param name="Path">Camingo</param>
        /// <returns></returns>
        public static bool DeleteIfExist(this string Path)
        {
            try
            {
                if (Path.IsDirectoryPath())
                {
                    var d = new DirectoryInfo(Path);
                    if (d.Exists)
                    {
                        try
                        {
                            d.Delete(true);
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    return !d.Exists;
                }

                if (Path.IsFilePath())
                {
                    var d = new FileInfo(Path);
                    if (d.Exists)
                    {
                        try
                        {
                            d.Delete();
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    return !d.Exists;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deleta um arquivo ou diretório se o mesmo existir e retorna TRUE se o arquivo puder ser criado novamente
        /// </summary>
        /// <param name="Path">Camingo</param>
        /// <returns></returns>
        public static bool DeleteIfExist(this FileSystemInfo Path)
        {
            return Path.FullName.DeleteIfExist();
        }

        /// <summary>
        /// Cria um diretório se o mesmo nao existir e retorna um DirectoryInfo deste diretório
        /// </summary>
        /// <param name="DirectoryName">o nome(s) do(s) diretorio(s) Ex.: "dir1/dir2/dir3" </param>
        /// <returns>Um DirectoryInfo contendo as informacoes do diretório criado</returns>
        /// <remarks>Caso o <paramref name="DirectoryName"/> for um caminho de arquivo, é utilizado o diretório deste arquivo.</remarks>
        public static DirectoryInfo ToDirectoryInfo(this string DirectoryName)
        {
            if (DirectoryName.IsFilePath())
            {
                DirectoryName = Path.GetDirectoryName(DirectoryName);
            }

            if (Directory.Exists(DirectoryName) == false)
            {
                Directory.CreateDirectory(DirectoryName);
            }

            return new DirectoryInfo(DirectoryName + Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo
        /// </summary>
        /// <param name="FileName">o nome do arquivo Ex.: "dir1/dir2/dir3/file.txt" </param>
        /// <returns>Um FileInfo contendo as informacoes do arquivo criado</returns>
        public static FileInfo ToFileInfo(this string FileName, FileType Type)
        {
            Type = Type ?? new FileType(".txt");
            FileName = Path.GetFullPath(FileName.RemoveAny(Path.GetExtension(FileName))) + Type.Extensions[0];
            if (File.Exists(FileName) == false)
            {
                File.Create(FileName).Dispose();
            }

            return new FileInfo(FileName);
        }

        /// <summary>
        /// Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo
        /// </summary>
        /// <param name="FileName">o nome do arquivo Ex.: "dir1/dir2/dir3/file.txt" </param>
        /// <returns>Um FileInfo contendo as informacoes do arquivo criado</returns>
        public static FileInfo ToFileInfo(this string FileName)
        {
            if (File.Exists(FileName) == false)
            {
                File.Create(FileName).Dispose();
            }

            return new FileInfo(FileName);
        }

        /// <summary>
        /// Cria um arquivo .ZIP de um diretório
        /// </summary>
        /// <param name="FilesDirectory">Um diretório contendo arquivos</param>
        /// <param name="OutputFile">O caminho onde será exportado o arquivo ZIP, Mesmo caminho do diretório se não especificado</param>
        /// <param name="CompressionLevel">Nivel de compressão do arquivo Zip</param>
        /// <returns>Um FileInfo contendo as informações do arquivo gerado</returns>

        public static FileInfo ToZipFile(this DirectoryInfo FilesDirectory, string OutputFile, CompressionLevel CompressionLevel = CompressionLevel.Optimal)
        {
            if (OutputFile.IsBlank())
            {
                OutputFile = FilesDirectory.FullName.Replace(FilesDirectory.Name, "") + FilesDirectory.Name;
            }

            OutputFile = !OutputFile.EndsWith(".zip") ? OutputFile + ".zip" : OutputFile;
            ZipFile.CreateFromDirectory(FilesDirectory.FullName, OutputFile, CompressionLevel, true);
            return new FileInfo(OutputFile);
        }

        /// <summary>
        /// Cria um arquivo .ZIP a partir de arquivos selecionados de uma pesquisa em um diretório
        /// </summary>
        /// <param name="FilesDirectory">Um diretório contendo arquivos</param>
        /// <param name="OutputFile">O caminho onde será exportado o arquivo ZIP, Mesmo caminho do diretório se não especificado</param>
        /// <param name="SearchOption">Especifica se a busca ocorrerá apenas no diretório pai ou em subdiretórios também</param>
        /// <param name="Searches">Padrões de pesquisa (*.extensao, nome.*, *nome*.*, *.*)</param>
        /// <param name="CompressionLevel">Nivel de compressão do arquivo Zip</param>
        /// <returns></returns>
        public static FileInfo ToZipFile(this DirectoryInfo FilesDirectory, string OutputFile, CompressionLevel CompressionLevel, SearchOption SearchOption, params string[] Searches)
        {
            if (OutputFile.IsBlank())
            {
                OutputFile = FilesDirectory.FullName.Replace(FilesDirectory.Name, "") + FilesDirectory.Name;
            }

            OutputFile = !OutputFile.EndsWith(".zip") ? OutputFile + ".zip" : OutputFile;
            foreach (var arq in FilesDirectory.SearchFiles(SearchOption, Searches))
            {
                using (var archive = ZipFile.Open(OutputFile, File.Exists(OutputFile) ? ZipArchiveMode.Update : ZipArchiveMode.Create))
                {
                    var arqz = archive.CreateEntryFromFile(arq.FullName, arq.FullName.RemoveAny(FilesDirectory.FullName).ReplaceMany("/", Conversions.ToString(Path.DirectorySeparatorChar), Conversions.ToString(Path.AltDirectorySeparatorChar)), CompressionLevel);
                    Debug.WriteLine("Adding: " + arqz.FullName);
                }
            }

            return new FileInfo(OutputFile);
        }

        /// <summary>
        /// Extrai um arquivo zip em um diretório
        /// </summary>
        /// <param name="File">Arquivo ZIp</param>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static DirectoryInfo ExtractZipFile(this FileInfo File, DirectoryInfo Directory)
        {
            Directory = (Directory.FullName + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(File.Name) + Path.DirectorySeparatorChar).ToDirectoryInfo();
            ZipFile.ExtractToDirectory(File.FullName, Directory.FullName);
            return Directory.FullName.ToDirectoryInfo();
        }

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches)
        {
            return (Searches ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).DefaultIfEmpty("*").SelectMany(x => Directory.GetFiles(x.Trim(), SearchOption));
        }

        /// <summary>
        /// Retorna uma lista de diretórios baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static List<DirectoryInfo> SearchDirectories(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches)
        {
            var FilteredList = new List<DirectoryInfo>();
            foreach (string pattern in (Searches ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).DefaultIfEmpty("*"))
                FilteredList.AddRange(Directory.GetDirectories(pattern.Trim(), SearchOption));
            return FilteredList;
        }

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static List<FileSystemInfo> Search(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches)
        {
            var FilteredList = new List<FileSystemInfo>();
            foreach (string pattern in (Searches ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).DefaultIfEmpty("*"))
                FilteredList.AddRange(Directory.GetFileSystemInfos(pattern.Trim(), SearchOption));
            return FilteredList;
        }

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em uma busca usando predicate
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="predicate">Funcao LINQ utilizada para a busca</param>
        /// <param name="SearchOption">Indica se apenas o diretorio atual ou todos os subdiretorios devem ser percorridos pela busca</param>
        /// <returns></returns>
        public static IEnumerable<FindType> Find<FindType>(this DirectoryInfo Directory, Func<FindType, bool> predicate, SearchOption SearchOption = SearchOption.AllDirectories) where FindType : FileSystemInfo
        {
            switch (typeof(FindType))
            {
                case var @case when @case == typeof(FileInfo):
                    {
                        return (IEnumerable<FindType>)Directory.GetFiles("*", SearchOption).Where((Func<FileInfo, bool>)predicate);
                        break;
                    }

                case var case1 when case1 == typeof(DirectoryInfo):
                    {
                        return (IEnumerable<FindType>)Directory.GetDirectories("*", SearchOption).Where((Func<DirectoryInfo, bool>)predicate);
                        break;
                    }

                default:
                    {
                        return (IEnumerable<FindType>)Directory.GetFileSystemInfos("*", SearchOption).Where((Func<FileSystemInfo, bool>)predicate);
                        break;
                    }
            }
        }

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static List<FileSystemInfo> SearchBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            Calendars.FixDateOrder(ref FirstDate, ref SecondDate);
            return Directory.Search(SearchOption, Searches).Where(file => file.LastWriteTime >= FirstDate && file.LastWriteTime <= SecondDate).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static List<FileInfo> SearchFilesBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            Calendars.FixDateOrder(ref FirstDate, ref SecondDate);
            return Directory.SearchFiles(SearchOption, Searches).Where(file => file.LastWriteTime.IsBetween(FirstDate, SecondDate)).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também</param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static List<DirectoryInfo> SearchDirectoriesBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            Calendars.FixDateOrder(ref FirstDate, ref SecondDate);
            return Directory.SearchDirectories(SearchOption, Searches).Where(file => file.LastWriteTime >= FirstDate && file.LastWriteTime <= SecondDate).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        /// <summary>
        /// Copia arquivos para dentro de outro diretório
        /// </summary>
        /// <param name="List">Arquivos</param>
        /// <param name="DestinationDirectory">Diretório de destino</param>
        /// <returns></returns>
        public static List<FileInfo> CopyTo(this List<FileInfo> List, DirectoryInfo DestinationDirectory)
        {
            var lista = new List<FileInfo>();
            if (!DestinationDirectory.Exists)
                DestinationDirectory.Create();
            foreach (var file in List)
                lista.Add(file.CopyTo(DestinationDirectory.FullName + Path.DirectorySeparatorChar + file.Name));
            return lista;
        }

        /// <summary>
        /// Verifica se um diretório possui arquivos
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool HasFiles(this DirectoryInfo Directory)
        {
            return Directory.GetFiles().Any();
        }

        /// <summary>
        /// Verifica se um diretório possui subdiretórios
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool HasDirectories(this DirectoryInfo Directory)
        {
            return Directory.GetDirectories().Any();
        }

        /// <summary>
        /// Verifica se um diretório está vazio
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool IsEmpty(this DirectoryInfo Directory)
        {
            return !Directory.HasFiles() & !Directory.HasDirectories();
        }

        /// <summary>
        /// Verifica se um diretório não está vazio
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool IsNotEmpty(this DirectoryInfo Directory)
        {
            return !Directory.IsEmpty();
        }

        /// <summary>
        /// Remove todos os subdiretorios vazios
        /// </summary>
        /// <param name="TopDirectory">Diretorio da operação</param>
        public static void CleanDirectory(this DirectoryInfo TopDirectory, bool DeleteTopDirectoryIfEmpty = true)
        {
            foreach (var diretorio in TopDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                foreach (var subdiretorio in diretorio.GetDirectories())
                    subdiretorio.CleanDirectory(true);
                if (diretorio.HasDirectories())
                {
                    diretorio.CleanDirectory(true);
                }

                if (diretorio.IsEmpty())
                {
                    diretorio.Delete();
                }
            }

            if (DeleteTopDirectoryIfEmpty && TopDirectory.Exists && TopDirectory.IsEmpty())
            {
                TopDirectory.Delete();
            }
        }
    }
}