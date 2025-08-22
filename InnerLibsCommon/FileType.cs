using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Extensions;
using Extensions.Colors;

namespace Extensions.Files

{
    /// <summary>
    /// Classe que representa um MIME NodeType
    /// </summary>
    public class FileType : IComparable<FileType>, IEquatable<FileType>
    {
        private static FileTypeList BaseList = new FileTypeList();

        /// <summary>
        /// Compara este FileType com outro FileType
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(FileType other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.GetHashCode() == other.GetHashCode();
        }


        /// <summary>
        /// Compara este FileType com outro objeto que pode ser um FileType, string (MIME Type ou extensão) ou FileInfo
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is FileType other)
            {
                return Equals(other);
            }
            else if (obj is string str)
            {
                return this.Extensions.Any(ext => ext.FlatEqual(str)) || this.MimeTypes.Any(mime => mime.FlatEqual(str));
            }
            else if (obj is FileInfo fileInfo)
            {
                return GetFileType(fileInfo) == this;
            }

            return false;
        }

        public int CompareTo(FileType other)
        {
            if (other == null) return 1;
            return string.Compare(Description, other.Description, StringComparison.OrdinalIgnoreCase);
        }

        internal void Build(string Extension, FileTypeList FileTypeList = null)
        {
            var item = GetFileType(Extension, FileTypeList);
            Extensions = item.Extensions;
            MimeTypes = item.MimeTypes;
            Description = item.Description.ToProperCase();
            Color = item.Color;
            Categories = item.Categories;
        }

        /// <summary>
        /// Constroi um MIME TEntity Default
        /// </summary>
        public FileType()
        {
            this.Description = "Unknown File";
            this.Extensions = new List<string>();
            this.MimeTypes = new List<string> { "application/octet-stream" };
            this.Categories = new List<string>();
            this.Color = new HSVColor("bbbbbb");
        }

        /// <summary>
        /// Constroi um File TEntity a partir de um Arquivo (FileInfo)
        /// </summary>
        /// <param name="File">Fileinfo com o Arquivo</param>
        public FileType(FileInfo File, FileTypeList FileTypeList = null) => Build(File?.Extension ?? ".bin", FileTypeList);

        /// <summary>
        /// Constroi um File Type a partir da extensão ou MIME Type de um Arquivo
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI">Extensão do arquivo</param>
        public FileType(string MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null) => Build(MimeTypeOrExtensionOrPathOrDataURI?.ToLowerInvariant(), FileTypeList);

        /// <summary>
        /// Descrição do tipo de arquivo
        /// </summary>
        /// <returns></returns>
        public string Description { get; internal set; } = "Unknown File";

        /// <summary>
        /// Extensão do arquivo
        /// </summary>
        /// <returns></returns>
        public List<string> Extensions { get; internal set; } = new List<string>();

        /// <summary>
        /// Tipo do arquivo (MIME TEntity String)
        /// </summary>
        /// <returns></returns>
        public List<string> MimeTypes { get; internal set; } = new List<string>();

        /// <summary>
        /// Categorias do tipo de arquivo
        /// </summary>
        /// <returns></returns>
        public List<string> Categories { get; internal set; } = new List<string>();

        /// <summary>
        /// Retorna uma lista de URLs do site fileinfo.com para este tipo de arquivo
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Uri> AboutUrls => Extensions?.Select(c => new Uri($"https://fileinfo.com/extension/{c.TrimStart('.')}/")) ?? new List<Uri>();

        /// <summary>
        /// Retorna uma cor relacionada a este tipo de arquivo
        /// </summary>
        public HSVColor Color
        {
            get => _c;
            internal set => _c = value ?? new HSVColor("bbbbbb");
        }

        private HSVColor _c = new HSVColor("bbbbbb");

        /// <summary>
        /// Retorna o subtipo do MIME TEntity (depois da barra)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> SubTypes => GetMimeTypesOrDefault().Select(p => p.ToLowerInvariant().Trim().GetAfter("/")).Distinct();

        /// <summary>
        /// Retorna o tipo do MIME TEntity (antes da barra)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Types => GetMimeTypesOrDefault().Select(p => p.ToLowerInvariant().Trim().GetBefore("/")).Distinct();

        /// <summary>
        /// Traz uma lista de extensões de acordo com o MIME type especificado
        /// </summary>
        /// <param name="MIME">MIME TEntity String</param>
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



        public static string GetDescription(string MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null)
        {
            return GetFileType(MimeTypeOrExtensionOrPathOrDataURI, FileTypeList).Description;

        }


        /// <summary>
        /// Retorna o MIME Type a partir de uma string que pode ser um MIME Type, extensão, caminho ou Data URI
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
        /// <param name="FileTypeList"></param>
        /// <returns></returns>
        public static string GetMimeType(string MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null)
        {
            return GetFileType(MimeTypeOrExtensionOrPathOrDataURI, FileTypeList).ToString();
        }

        /// <summary>
        /// Retorna a cor do tipo de arquivo a partir de um arquivo (FileInfo)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="FileTypeList"></param>
        /// <returns></returns>
        public static HSVColor GetColor(FileInfo info, FileTypeList FileTypeList = null)
        {
            return GetFileType(info, FileTypeList).Color;
        }

        /// <summary>
        /// Retorna a cor do tipo de arquivo a partir de uma string que pode ser um MIME Type, extensão, caminho ou Data URI
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
        /// <param name="FileTypeList"></param>
        /// <returns></returns>
        public static HSVColor GetColor(string MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null)
        {
            return GetFileType(MimeTypeOrExtensionOrPathOrDataURI, FileTypeList).Color;
        }



        /// <summary>
        /// Retorna um objeto FileType a partir de uma lista de MIME Types, extensões, caminhos ou Data URIs
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
        /// <param name="FileTypeList"></param>
        /// <returns></returns>
        public static FileTypeList GetFileType(IEnumerable<string> MimeTypeOrExtensionOrPathOrDataURI) => new FileTypeList(MimeTypeOrExtensionOrPathOrDataURI.Select(x => GetFileType(x)).ToArray());

        /// <summary>
        /// Retorna um objeto FileType a partir de um arquivo (FileInfo)
        /// </summary>
        /// <param name="info"></param>
        /// <param name="FileTypeList"></param>
        /// <returns></returns>
        public static FileType GetFileType(FileInfo info, FileTypeList FileTypeList = null) => info == null ? new FileType() : GetFileType(info.Extension, FileTypeList);


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
                if (MimeTypeOrExtensionOrPathOrDataURI.IsDataURL())
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
                if (r.IsValid())
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(r);
                    BaseList = new FileTypeList();
                    foreach (XmlNode node in doc["mimes"].ChildNodes)
                    {
                        var ft = BaseList.FirstOr(x => (x.Description ?? Util.EmptyString) == (node["Description"].InnerText.TrimBetween() ?? Util.EmptyString), new FileType());
                        ft.Description = node["Description"].InnerText.TrimBetween().ToTitle();

                        foreach (XmlNode item in node["MimeTypes"].ChildNodes)
                        {
                            ft.MimeTypes.Add(item.InnerText.TrimBetween().ToLower());
                        }

                        foreach (XmlNode item in node["Extensions"].ChildNodes)
                        {
                            ft.Extensions.Add(item.InnerText.TrimBetween().ToLower());
                        }

                        foreach (XmlNode item in node["Categories"].ChildNodes)
                        {
                            ft.Categories.Add(item.InnerText.TrimBetween().ToTitle());
                        }

                        ft.Color = new HSVColor(Util.BlankCoalesce(node["Color"]?.InnerText ?? Util.EmptyString, ft.Description, "#808080"));

                        ft.MimeTypes = ft.MimeTypes.Distinct().ToList();
                        ft.Extensions = ft.Extensions.Distinct().ToList();
                        ft.Categories = ft.Categories.Distinct().ToList();

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


        public static IEnumerable<FileType> GetByCategory(string category) => GetFileTypeList().Where(x => x.Categories.Contains(category, StringComparer.InvariantCultureIgnoreCase));

        public static IEnumerable<string> GetCategories() => GetFileTypeList().SelectMany(x => x.Categories).Distinct();


        public static Dictionary<string, IEnumerable<FileType>> GroupByCategories() => GetFileTypeList().SelectMany(x => x.Categories, (x, category) => new { x, category })
            .GroupBy(x => x.category, x => x.x)
            .ToDictionary();

        public static IEnumerable<FileType> GetImageTypes() => GetFileTypeList().Where(x => x.IsImage());


        public static IEnumerable<FileType> GetTextTypes() => GetFileTypeList().Where(x => x.IsText());

        public static IEnumerable<FileType> GetVideoTypes() => GetFileTypeList().Where(x => x.IsVideo());

        public static IEnumerable<FileType> GetAudioTypes() => GetFileTypeList().Where(x => x.IsAudio());

        public static IEnumerable<FileType> GetApplicationTypes() => GetFileTypeList().Where(x => x.IsApplication());

        public static IEnumerable<FileType> GetFontTypes() => GetFileTypeList().Where(x => x.IsFont());

        public static IEnumerable<FileType> GetMessageTypes() => GetFileTypeList().Where(x => x.IsMessage());

        public static IEnumerable<FileType> GetMarkdownTypes() => GetFileTypeList().Where(x => x.IsMarkdown());

        public static IEnumerable<FileType> GetHtmlTypes() => GetFileTypeList().Where(x => x.IsHtml());

        public bool IsCategory(params string[] categories) => Categories.ContainsAll(categories, StringComparer.InvariantCultureIgnoreCase);
        public bool IsAnyCategory(params string[] categories) => categories.Any(category => Categories.Contains(category, StringComparer.InvariantCultureIgnoreCase));


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
        public bool IsSubType(string Type) => SubTypes.Contains(Type);

        /// <summary>
        /// Verifica se Tipo de arquivo é de audio
        /// </summary>
        /// <returns></returns>
        public bool IsVideo() => IsType("video");


        public bool IsMarkdown()
        {
            return IsType("text") && IsSubType("markdown")
             || IsType("application") && IsSubType("markdown")
                || IsType("text") && IsSubType("x-markdown")
                || IsType("application") && IsSubType("x-markdown")
                || IsType("text") && IsSubType("x-gfm")
                || IsType("application") && IsSubType("x-gfm")
                || IsType("text") && IsSubType("x-markdown+html")
                || IsType("application") && IsSubType("x-markdown+html")
                || IsType("text") && IsSubType("x-markdown+plain")
                || IsType("application") && IsSubType("x-markdown+plain")
             ;

        }

        public bool IsHtml()
        {
            return IsType("text") && IsSubType("html")
                || IsType("application") && IsSubType("html")
                || IsType("text") && IsSubType("x-html")
                || IsType("application") && IsSubType("x-html");
        }

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

        public override int GetHashCode() => Extensions.OrderBy(ext => ext).Union(MimeTypes.OrderBy(x => x)).SelectJoinString().GetHashCode();

        public static bool operator ==(FileType left, FileType right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }

            return left.Equals(right);
        }

        public static bool operator !=(FileType left, FileType right) => !(left == right);

        public static bool operator <(FileType left, FileType right) => ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;

        public static bool operator <=(FileType left, FileType right) => ReferenceEquals(left, null) || left.CompareTo(right) <= 0;

        public static bool operator >(FileType left, FileType right) => !ReferenceEquals(left, null) && left.CompareTo(right) > 0;

        public static bool operator >=(FileType left, FileType right) => ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
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
        public FileTypeList(params FileType[] FileTypes)
        {
            var defaultType = new FileType();
            foreach (var item in FileTypes ?? Array.Empty<FileType>())
            {
                if (item == null || item.Extensions == null || item.Extensions.Count == 0 || item.MimeTypes == null || item.MimeTypes.Count == 0)
                {
                    continue;
                }

                var existing = this.FirstOrDefault(x => x == item || x.Description.FlatEqual(item.Description));

                if (existing != null)
                {
                    // if description is default change to new one
                    existing.Extensions = existing.Extensions.Union(item.Extensions).Distinct().ToList();
                    existing.MimeTypes = existing.MimeTypes.Union(item.MimeTypes).Distinct().ToList();
                    existing.Categories = existing.Categories.Union(item.Categories).Distinct().ToList();


                    if (existing.Description == defaultType.Description || item.Description.IsBlank())
                    {
                        existing.Description = item.Description;
                    }


                    // if color is default change to new one
                    if (item.Color != null && item.Color != defaultType.Color && !item.Color.IsGray())
                    {
                        existing.Color = item.Color;
                    }
                }
                else
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Cria uma nova lista a partir de uma lista de tipos de arquivos
        /// </summary>
        /// <param name="FileTypes">Tipos de Arquivos</param>
        public FileTypeList(IEnumerable<FileType> FileTypes) : this(FileTypes?.ToArray() ?? Array.Empty<FileType>())
        {
        }


        /// <summary>
        /// Cria uma nova lista a partir de uma lista de tipos de arquivos
        /// </summary>
        /// <param name="Files">Tipos de Arquivos</param>
        public FileTypeList(IEnumerable<FileInfo> Files) : this(Files?.Select(x => new FileType(x)))
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

        public IEnumerable<string> Descriptions => this.Select(x => x.Description).Distinct(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Retorna todas as extensões da lista
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> Extensions => this.SelectMany(x => x.Extensions).Distinct(StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<string> FirstTypes => this.SelectMany(x => x.Types).Distinct(StringComparer.InvariantCultureIgnoreCase);

        public IEnumerable<string> Categories => this.SelectMany(x => x.Categories).Distinct(StringComparer.InvariantCultureIgnoreCase);

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