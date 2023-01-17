using InnerLibs.LINQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InnerLibs
{
    /// <summary>
    /// Funções para trabalhar com diretorios
    /// </summary>
    /// <remarks></remarks>
    public static class Directories
    {
        #region Public Methods

        /// <summary>
        /// Remove todos os subdiretorios vazios
        /// </summary>
        /// <param name="TopDirectory">Diretorio da operação</param>
        public static DirectoryInfo CleanDirectory(this DirectoryInfo TopDirectory, bool DeleteTopDirectoryIfEmpty = true)
        {
            if (TopDirectory != null)
            {
                foreach (var diretorio in TopDirectory.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    diretorio.GetDirectories().Each(subdiretorio => subdiretorio.CleanDirectory(true));

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
            return TopDirectory;
        }

        /// <summary>
        /// Copia arquivos para dentro de outro diretório
        /// </summary>
        /// <param name="List">Arquivos</param>
        /// <param name="DestinationDirectory">Diretório de destino</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> CopyTo(this IEnumerable<FileInfo> List, DirectoryInfo DestinationDirectory)
        {
            var lista = new List<FileInfo>();
            if (!DestinationDirectory?.Exists ?? false)
            {
                DestinationDirectory.Create();
            }

            foreach (var file in List ?? new List<FileInfo>())
            {
                lista.Add(file.CopyTo(DestinationDirectory.FullName + Path.DirectorySeparatorChar + file.Name));
            }

            return lista;
        }

        /// <summary>
        /// Cria um diretório se o mesmo nao existir e retorna um <see cref="DirectoryInfo"/> deste diretório
        /// </summary>
        /// <param name="DirectoryName">o nome(s) do(s) diretorio(s) Ex.: "dir1/dir2/dir3"</param>
        /// <returns>Um DirectoryInfo contendo as informacoes do diretório criado</returns>
        /// <remarks>
        /// Caso o <paramref name="DirectoryName"/> for um caminho de arquivo, é utilizado o
        /// diretório deste arquivo.
        /// </remarks>
        public static DirectoryInfo CreateDirectoryIfNotExists(this string DirectoryName)
        {
            if (DirectoryName.IsFilePath())
            {
                DirectoryName = Path.GetDirectoryName(DirectoryName);
            }

            if (DirectoryName.IsDirectoryPath())
            {
                if (Directory.Exists(DirectoryName) == false)
                {
                    Directory.CreateDirectory(DirectoryName);
                }
            }
            else
            {
                throw new ArgumentException("DirectoryName is not a valid path");
            }

            return new DirectoryInfo(DirectoryName + Path.DirectorySeparatorChar);
        }

        public static DirectoryInfo CreateDirectoryIfNotExists(this DirectoryInfo DirectoryName) => DirectoryName?.FullName.CreateDirectoryIfNotExists();

        public static DirectoryInfo CreateDirectoryIfNotExists(this FileInfo FileName) => FileName.FullName.CreateDirectoryIfNotExists();

        /// <summary>
        /// Cria um arquivo em branco se o mesmo nao existir e retorna um Fileinfo deste arquivo
        /// </summary>
        /// <param name="FileName">o nome do arquivo Ex.: "dir1/dir2/dir3/file.txt"</param>
        /// <returns>Um FileInfo contendo as informacoes do arquivo criado</returns>
        public static FileInfo CreateFileIfNotExists(this string FileName, FileType Type = null)
        {
            Type = Type ?? new FileType(Path.GetExtension(FileName));
            FileName = $"{Path.GetFullPath(FileName.TrimAny(Path.GetExtension(FileName)))}{Type.Extensions.FirstOrDefault()}";

            FileName.CreateDirectoryIfNotExists();

            if (File.Exists(FileName) == false)
            {
                File.Create(FileName).Dispose();
            }

            return new FileInfo(FileName);
        }

        /// <summary>
        /// Deleta um arquivo ou diretório se o mesmo existir e retorna true se o arquivo puder ser
        /// criado novamente
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
                        catch
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
                        catch
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
        /// Deleta um arquivo ou diretório se o mesmo existir e retorna TRUE se o arquivo puder ser
        /// criado novamente
        /// </summary>
        /// <param name="Path">Caminho</param>
        /// <returns></returns>
        public static bool DeleteIfExist(this FileSystemInfo Path) => Path?.FullName.DeleteIfExist() ?? false;

        /// <summary>
        /// Verifica se um diretório possui subdiretórios
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool HasDirectories(this DirectoryInfo Directory) => Directory?.GetDirectories().Any() ?? false;

        /// <summary>
        /// Verifica se um diretório possui arquivos
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool HasFiles(this DirectoryInfo Directory) => Directory?.GetFiles().Any() ?? false;

        public static T Hide<T>(this T dir) where T : FileSystemInfo
        {
            if (dir != null && dir.Exists)
            {
                if (!dir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    dir.Attributes |= FileAttributes.Hidden;
                }
            }
            return dir;
        }

        /// <summary>
        /// Verifica se um diretório está vazio
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool IsEmpty(this DirectoryInfo Directory) => !Directory.HasFiles() && !Directory.HasDirectories();

        /// <summary>
        /// Verifica se um diretório não está vazio
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <returns></returns>
        public static bool IsNotEmpty(this DirectoryInfo Directory) => !Directory.IsEmpty();

        public static bool IsVisible<T>(this T dir) where T : FileSystemInfo => dir != null && dir.Exists && dir.Attributes.HasFlag(FileAttributes.Hidden) == false;

        public static IEnumerable<string> ReadManyText(this DirectoryInfo directory, SearchOption Option, params string[] Patterns) => directory.SearchFiles(Option, Patterns).Select(x => x.ReadAllText());

        public static IEnumerable<string> ReadManyText(this DirectoryInfo directory, params string[] Patterns) => directory.ReadManyText(SearchOption.TopDirectoryOnly, Patterns);

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static List<FileSystemInfo> Search(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches)
        {
            var FilteredList = new List<FileSystemInfo>();
            foreach (string pattern in (Searches ?? Array.Empty<string>()).SelectMany(z => z.SplitAny(":", "|")).Where(x => x.IsNotBlank()).DefaultIfEmpty("*"))
            {
                if (Directory != null)
                    FilteredList.AddRange(Directory.GetFileSystemInfos(pattern.Trim(), SearchOption));
            }

            return FilteredList;
        }

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em um ou mais padrões de pesquisas
        /// dentro de um range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static List<FileSystemInfo> SearchBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            Misc.FixOrder(ref FirstDate, ref SecondDate);
            return Directory.Search(SearchOption, Searches).Where(file => file.LastWriteTime >= FirstDate && file.LastWriteTime <= SecondDate).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        /// <summary>
        /// Retorna uma lista de diretórios baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static List<DirectoryInfo> SearchDirectories(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches)
        {
            var FilteredList = new List<DirectoryInfo>();
            foreach (string pattern in (Searches ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).DefaultIfEmpty("*"))
            {
                if (Directory != null)
                    FilteredList.AddRange(Directory.GetDirectories(pattern.Trim(), SearchOption));
            }

            return FilteredList;
        }

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um
        /// range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static List<DirectoryInfo> SearchDirectoriesBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            Misc.FixOrder(ref FirstDate, ref SecondDate);
            return Directory.SearchDirectories(SearchOption, Searches).Where(file => file.LastWriteTime >= FirstDate && file.LastWriteTime <= SecondDate).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> SearchFiles(this DirectoryInfo Directory, SearchOption SearchOption, params string[] Searches) => (Searches ?? Array.Empty<string>()).Where(x => x.IsNotBlank()).DefaultIfEmpty("*").SelectMany(x => Directory.GetFiles(x.Trim(), SearchOption));

        /// <summary>
        /// Retorna uma lista de arquivos baseado em um ou mais padrões de pesquisas dentro de um
        /// range de 2 datas
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">
        /// Especifica se a pesquisa ocorrerá apenas no diretório ou em todos os subdiretórios também
        /// </param>
        /// <param name="Searches">Padrões de pesquisa (*.txt, arquivo.*, *)</param>
        /// <param name="FirstDate">Data Inicial</param>
        /// <param name="SecondDate">Data Final</param>
        /// <returns></returns>
        public static List<FileInfo> SearchFilesBetween(this DirectoryInfo Directory, DateTime FirstDate, DateTime SecondDate, SearchOption SearchOption, params string[] Searches)
        {
            Misc.FixOrder(ref FirstDate, ref SecondDate);
            return Directory.SearchFiles(SearchOption, Searches).Where(file => file.LastWriteTime.IsBetween(FirstDate, SecondDate)).OrderByDescending(f => f.LastWriteTime.Year <= 1601 ? f.CreationTime : f.LastWriteTime).ToList();
        }

        public static T Show<T>(this T dir) where T : FileSystemInfo
        {
            if (dir != null && dir.Exists)
            {
                if (dir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    dir.Attributes &= ~FileAttributes.Hidden;
                }
            }
            return dir;
        }

        public static T ToggleVisibility<T>(this T dir) where T : FileSystemInfo => dir.IsVisible() ? dir.Hide() : dir.Show();

        /// <summary>
        /// Retorna uma lista de arquivos ou diretórios baseado em uma busca usando predicate
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="predicate">Funcao LINQ utilizada para a busca</param>
        /// <param name="SearchOption">
        /// Indica se apenas o diretorio atual ou todos os subdiretorios devem ser percorridos pela busca
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> Where<T>(this DirectoryInfo Directory, Func<T, bool> predicate, SearchOption SearchOption = SearchOption.AllDirectories) where T : FileSystemInfo
        {
            if (Directory != null && Directory.Exists && predicate != null)

                if (typeof(T) == typeof(FileInfo))
                    return Directory.GetFiles("*", SearchOption).Where((Func<FileInfo, bool>)predicate) as IEnumerable<T>;
                else if (typeof(T) == typeof(DirectoryInfo))
                    return Directory.GetDirectories("*", SearchOption).Where((Func<DirectoryInfo, bool>)predicate) as IEnumerable<T>;

                else
                    return Directory.GetFileSystemInfos("*", SearchOption).Where((Func<FileSystemInfo, bool>)predicate) as IEnumerable<T>;

            return Array.Empty<T>();
        }

        #endregion Public Methods
    }
}