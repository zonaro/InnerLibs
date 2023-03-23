using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Extensions;

namespace Extensions.Files

{
    /// <summary>
    /// Classe que representa um MIME NodeType
    /// </summary>
    public class FileType
    {
        private static FileTypeList BaseList = new FileTypeList();

        internal void Build(string Extension, FileTypeList FileTypeList = null)
        {
            var item = GetFileType(Extension, FileTypeList);
            Extensions = item.Extensions;
            MimeTypes = item.MimeTypes;
            Description = item.Description.ToProperCase();
        }

        /// <summary>
        /// Constroi um MIME T Default
        /// </summary>
        public FileType()
        {
        }

        /// <summary>
        /// Constroi um File T a partir de um Arquivo (FileInfo)
        /// </summary>
        /// <param name="File">Fileinfo com o Arquivo</param>
        public FileType(FileInfo File, FileTypeList FileTypeList = null) => Build(File?.Extension ?? ".bin", FileTypeList);

        /// <summary>
        /// Constroi um File T a partir da extensão ou MIME T de um Arquivo
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI">Extensão do arquivo</param>
        public FileType(string MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null) => Build(MimeTypeOrExtensionOrPathOrDataURI?.ToLowerInvariant(), FileTypeList);

        /// <summary>
        /// Descrição do tipo de arquivo
        /// </summary>
        /// <returns></returns>
        public string Description { get; set; } = "Unknown File";

        /// <summary>
        /// Extensão do arquivo
        /// </summary>
        /// <returns></returns>
        public List<string> Extensions { get; set; } = new List<string>();

        /// <summary>
        /// Tipo do arquivo (MIME T String)
        /// </summary>
        /// <returns></returns>
        public List<string> MimeTypes { get; set; } = new List<string>();

        /// <summary>
        /// Retorna o subtipo do MIME T (depois da barra)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> SubTypes => GetMimeTypesOrDefault().Select(p => p.ToLowerInvariant().Trim().GetAfter("/")).Distinct();

        /// <summary>
        /// Retorna o tipo do MIME T (antes da barra)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Types => GetMimeTypesOrDefault().Select(p => p.ToLowerInvariant().Trim().GetBefore("/")).Distinct();

        /// <summary>
        /// Traz uma lista de extensões de acordo com o MIME type especificado
        /// </summary>
        /// <param name="MIME">MIME T String</param>
        /// <returns></returns>
        public static IEnumerable<string> GetExtensions(string MIME, FileTypeList FileTypeList = null)
        {
            foreach (FileType item in FileTypeList ?? GetFileTypeList())
            {
                if (item.GetMimeTypesOrDefault().Contains(MIME))
                {
                    return item.Extensions;
                }
            }

            return new List<string>();
        }

        public static FileTypeList GetFileType(IEnumerable<string> MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null) => new FileTypeList(MimeTypeOrExtensionOrPathOrDataURI.Select(x => GetFileType(x)).ToArray());

        /// <summary>
        /// Retorna um objeto FileType a partir de uma extensão de Arquivo ou FileType string
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
        /// <returns></returns>
        public static FileType GetFileType(string MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null)
        {
            bool ismime = true;
            try
            {
                return new Web.DataURI(MimeTypeOrExtensionOrPathOrDataURI).ToFileType();
            }
            catch
            {
            }

            try
            {
                string newmime = Path.GetExtension(MimeTypeOrExtensionOrPathOrDataURI);
                if (newmime.IsNotBlank())
                {
                    MimeTypeOrExtensionOrPathOrDataURI = newmime;
                    ismime = false;
                }
            }
            catch
            {
            }

            if (!ismime)
            {
                MimeTypeOrExtensionOrPathOrDataURI = "." + MimeTypeOrExtensionOrPathOrDataURI.TrimAny(true, " ", ".");
            }

            return (FileTypeList ?? GetFileTypeList()).FirstOr(x => x.Extensions.ToArray().Union(x.GetMimeTypesOrDefault().ToArray()).Contains(MimeTypeOrExtensionOrPathOrDataURI, StringComparer.InvariantCultureIgnoreCase), new FileType());
        }

        /// <summary>
        /// Retorna uma Lista com todos os MIME Types suportados
        /// </summary>
        /// <returns></returns>
        public static FileTypeList GetFileTypeList(bool Reset = false)
        {
            if (Reset || BaseList == null || BaseList.Any() == false)
            {
                string r = Util.GetResourceFileText(Assembly.GetExecutingAssembly(), "mimes.xml");
                if (r.IsNotBlank())
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(r);
                    BaseList = new FileTypeList();
                    foreach (XmlNode node in doc["mimes"].ChildNodes)
                    {
                        var ft = BaseList.FirstOr(x => (x.Description ?? Util.EmptyString) == (node["Description"].InnerText.TrimBetween() ?? Util.EmptyString), new FileType());
                        ft.Description = node["Description"].InnerText.TrimBetween();

                        foreach (XmlNode item in node["MimeTypes"].ChildNodes)
                        {
                            ft.MimeTypes.Add(item.InnerText.TrimBetween());
                        }

                        foreach (XmlNode item in node["Extensions"].ChildNodes)
                        {
                            ft.Extensions.Add(item.InnerText.TrimBetween());
                        }

                        ft.MimeTypes = ft.MimeTypes.Distinct().ToList();
                        ft.Extensions = ft.Extensions.Distinct().ToList();

                        if (!BaseList.Contains(ft))
                        {
                            BaseList.Add(ft);
                        }
                    }
                }
            }

            return BaseList;
        }

        /// <summary>
        /// Retorna uma lista de strings contendo todos os MIME Types
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetFileTypeStringList(FileTypeList FileTypeList = null) => (FileTypeList ?? GetFileTypeList()).SelectMany(x => x.GetMimeTypesOrDefault()).Distinct();

        public IEnumerable<string> GetMimeTypesOrDefault() => (MimeTypes ?? new List<string>()).DefaultIfEmpty("application/octet-stream");

        /// <summary>
        /// Verifica se Tipo de arquivo é de audio
        /// </summary>
        /// <returns></returns>
        public bool IsApplication() => IsType("application");

        /// <summary>
        /// Verifica se Tipo de arquivo é de audio
        /// </summary>
        /// <returns></returns>
        public bool IsAudio() => IsType("audio");

        /// <summary>
        /// Verifica se Tipo de arquivo é fonte
        /// </summary>
        /// <returns></returns>
        public bool IsFont() => IsType("font");

        /// <summary>
        /// Verifica se Tipo de arquivo é de imagem
        /// </summary>
        /// <returns></returns>
        public bool IsImage() => IsType("image");

        /// <summary>
        /// Verifica se Tipo de arquivo é fonte
        /// </summary>
        /// <returns></returns>
        public bool IsMessage() => IsType("message");

        /// <summary>
        /// Verifica se Tipo de arquivo é de audio
        /// </summary>
        /// <returns></returns>
        public bool IsText() => IsType("Text");

        /// <summary>
        /// Verifica se Tipo de arquivo é do tipo <paramref name="Type"/>
        /// </summary>
        /// <returns></returns>
        public bool IsType(string Type) => Types.Contains(Type);

        /// <summary>
        /// Verifica se Tipo de arquivo é de audio
        /// </summary>
        /// <returns></returns>
        public bool IsVideo() => IsType("video");

        public IEnumerable<FileInfo> SearchFiles(DirectoryInfo Directory, SearchOption SearchOption = SearchOption.AllDirectories) => Directory.SearchFiles(SearchOption, Extensions.Select(ext => "*" + ext.PrependIf(".", !ext.StartsWith("."))).ToArray());

        /// <summary>
        /// Retorna uma string representando um filtro de caixa de dialogo WinForms
        /// </summary>
        /// <returns></returns>
        public string ToFilterString() => $"{Description}|{Extensions.SelectJoinString(ext => $"*{ext}", ";")}";

        /// <summary>
        /// Retorna uma string com o primeiro MIME TYPE do arquivo
        /// </summary>
        /// <returns></returns>
        public override string ToString() => GetMimeTypesOrDefault().First();
    }

    /// <summary>
    /// Lista com Tipos de arquivo utilizada para filtro e validação
    /// </summary>
    public class FileTypeList : List<FileType>
    {
        /// <summary>
        /// Cria uma nova lista vazia
        /// </summary>
        public FileTypeList()
        {
        }

        /// <summary>
        /// Cria uma nova lista a partir de mime types, caminhos ou extensoes
        /// </summary>
        /// <param name="FileTypes">Tipos de Arquivos</param>
        public FileTypeList(params string[] FileTypes) : this((FileTypes ?? Array.Empty<string>()).Select(x => new FileType(x)).ToArray())
        {
        }

        /// <summary>
        /// Cria uma nova lista a partir de tipos de arquivos
        /// </summary>
        /// <param name="FileTypes">Tipos de Arquivos</param>
        public FileTypeList(params FileType[] FileTypes) => AddRange(FileTypes ?? Array.Empty<FileType>());

        /// <summary>
        /// Cria uma nova lista a partir de uma lista de tipos de arquivos
        /// </summary>
        /// <param name="FileTypeList">Tipos de Arquivos</param>
        public FileTypeList(IEnumerable<FileType> FileTypeList) : this((FileTypeList ?? new List<FileType>()).ToArray())
        {
        }

        /// <summary>
        /// Cria uma nova lista a partir de um critério de filtro
        /// </summary>
        /// <param name="predicate">Criterio de busca</param>
        public FileTypeList(Func<FileType, bool> predicate) : this(null, predicate)
        {
        }

        /// <summary>
        /// Cria uma nova lista a partir de um critério de filtro
        /// </summary>
        /// <param name="predicate">Criterio de busca</param>
        public FileTypeList(FileTypeList FileTypeList, Func<FileType, bool> predicate) : this((FileTypeList ?? FileType.GetFileTypeList()).Where(predicate).ToArray())
        {
        }

        public IEnumerable<string> Descriptions => (IEnumerable<string>)this.SelectMany(x => x.Description).Distinct();

        /// <summary>
        /// Retorna todas as extensões da lista
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Extensions => this.SelectMany(x => x.Extensions).Distinct();

        public IEnumerable<string> FirstTypes => this.SelectMany(x => x.Types).Distinct();

        /// <summary>
        /// Retorna todas os MIME Types da lista
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> MimeTypes => this.SelectMany(x => x.GetMimeTypesOrDefault()).Distinct();

        public IEnumerable<string> SubTypes => this.SelectMany(x => x.SubTypes).Distinct();

        /// <summary>
        /// Busca arquivos que correspondam com as extensões desta lista
        /// </summary>
        /// <param name="Directory">Diretório</param>
        /// <param name="SearchOption">Tipo de busca</param>
        /// <returns></returns>
        public IEnumerable<FileInfo> SearchFiles(DirectoryInfo Directory, SearchOption SearchOption = SearchOption.AllDirectories) => Directory.SearchFiles(SearchOption, Extensions.Select(ext => "*" + ext.PrependIf(".", !ext.StartsWith("."))).ToArray());

        /// <summary>
        /// Retorna uma string representando um filtro de caixa de dialogo WinForms
        /// </summary>
        /// <returns></returns>
        public string ToFilterString() => this.SelectJoinString(x => x.ToFilterString(), "|");
    }
}