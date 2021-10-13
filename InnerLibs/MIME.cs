﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace InnerLibs
{

    /// <summary>
/// Módulo de manipulaçao de MIME Types
/// </summary>
    public static class FileTypeExtensions
    {

        /// <summary>
    /// Retorna o Mime Type a partir da extensão de um arquivo
    /// </summary>
    /// <param name="Extension">extensão do arquivo</param>
    /// <returns>string mime type</returns>
        public static List<string> GetFileType(this string Extension)
        {
            return (List<string>)FileType.GetFileType(Extension).GetMimeTypesOrDefault();
        }

        /// <summary>
    /// Retorna o Mime Type a partir de um arquivo
    /// </summary>
    /// <param name="File">Arquivo</param>
    /// <returns>string mime type</returns>
        public static List<string> GetFileType(this FileInfo File)
        {
            return File.Extension.GetFileType();
        }

        /// <summary>
    /// Retorna o Mime Type a partir de de um formato de Imagem
    /// </summary>
    /// <param name="RawFormat">Formato de Imagem</param>
    /// <returns>string mime type</returns>
        public static List<string> GetFileType(this ImageFormat RawFormat)
        {
            try
            {
                foreach (var img in ImageCodecInfo.GetImageEncoders())
                {
                    if (img.FormatID == RawFormat.Guid)
                    {
                        return img.FilenameExtension.GetFileType();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return ".png".GetFileType();
        }

        /// <summary>
    /// Retorna o Mime Type a partir de de uma Imagem
    /// </summary>
    /// <param name="Image">Imagem</param>
    /// <returns>string mime type</returns>
        public static List<string> GetFileType(this Image Image)
        {
            return Image.RawFormat.GetFileType();
        }

        /// <summary>
    /// Retorna um Objeto FileType a partir de uma string MIME Type, Nome ou Extensão de Arquivo
    /// </summary>
    /// <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
    /// <returns></returns>
        public static FileType ToFileType(this string MimeTypeOrExtensionOrPathOrDataURI)
        {
            return new FileType(MimeTypeOrExtensionOrPathOrDataURI);
        }

        /// <summary>
    /// Retorna um icone de acordo com o arquivo
    /// </summary>
    /// <param name="File">Arquivo</param>
    /// <returns></returns>
        public static Icon GetIcon(this FileSystemInfo File)
        {
            try
            {
                return Icon.ExtractAssociatedIcon(File.FullName);
            }
            catch (Exception ex)
            {
                return SystemIcons.WinLogo;
            }
        }
    }

    /// <summary>
/// Classe que representa um MIME Type
/// </summary>
    public class FileType
    {

        /// <summary>
    /// Extensão do arquivo
    /// </summary>
    /// <returns></returns>
        public List<string> Extensions { get; set; } = new List<string>();

        /// <summary>
    /// Tipo do arquivo (MIME Type String)
    /// </summary>
    /// <returns></returns>
        public List<string> MimeTypes { get; set; } = new List<string>();

        public IEnumerable<string> GetMimeTypesOrDefault()
        {
            return (MimeTypes ?? new List<string>()).DefaultIfEmpty("application/octet-stream");
        }

        /// <summary>
    /// Descrição do tipo de arquivo
    /// </summary>
    /// <returns></returns>
        public string Description { get; set; } = "Unknown File";

        /// <summary>
    /// Verifica se Tipo de arquivo é de imagem
    /// </summary>
    /// <returns></returns>
        public bool IsImage()
        {
            return FirstTypes.Contains("image");
        }

        /// <summary>
    /// Verifica se Tipo de arquivo é de audio
    /// </summary>
    /// <returns></returns>
        public bool IsAudio()
        {
            return FirstTypes.Contains("audio");
        }

        /// <summary>
    /// Verifica se Tipo de arquivo é de audio
    /// </summary>
    /// <returns></returns>
        public bool IsVideo()
        {
            return FirstTypes.Contains("video");
        }

        /// <summary>
    /// Verifica se Tipo de arquivo é de audio
    /// </summary>
    /// <returns></returns>
        public bool IsText()
        {
            return FirstTypes.Contains("text");
        }

        /// <summary>
    /// Verifica se Tipo de arquivo é de audio
    /// </summary>
    /// <returns></returns>
        public bool IsApplication()
        {
            return FirstTypes.Contains("application");
        }

        /// <summary>
    /// Retorna o tipo do MIME Type (antes da barra)
    /// </summary>
    /// <returns></returns>
        public IEnumerable<string> FirstTypes
        {
            get
            {
                return GetMimeTypesOrDefault().Select(p => p.ToLower().Trim().GetBefore("/")).Distinct();
            }
        }

        /// <summary>
    /// Retorna o subtipo do MIME Type (depois da barra)
    /// </summary>
    /// <returns></returns>
        public IEnumerable<string> SubTypes
        {
            get
            {
                return GetMimeTypesOrDefault().Select(p => p.ToLower().Trim().GetAfter("/")).Distinct();
            }
        }

        private static FileTypeList l = new FileTypeList();

        /// <summary>
    /// Retorna uma Lista com todos os MIME Types suportados
    /// </summary>
    /// <returns></returns>
        public static FileTypeList GetFileTypeList(bool Reset = false)
        {
            if (Reset || l is null || l.Any() == false)
            {
                string r = Assembly.GetExecutingAssembly().GetResourceFileText("InnerLibs.mimes.xml");
                var doc = new XmlDocument();
                doc.LoadXml(r);
                l = new FileTypeList();
                foreach (XmlNode node in doc["mimes"].ChildNodes)
                {
                    var ft = l.FirstOr(x => (x.Description ?? "") == (node["Description"].InnerText.AdjustBlankSpaces() ?? ""), new FileType());
                    ft.Description = node["Description"].InnerText.AdjustBlankSpaces();
                    foreach (XmlNode item in node["MimeTypes"].ChildNodes)
                        ft.MimeTypes.Add(item.InnerText.AdjustBlankSpaces());
                    foreach (XmlNode item in node["Extensions"].ChildNodes)
                        ft.Extensions.Add(item.InnerText.AdjustBlankSpaces());
                    ft.MimeTypes = ft.MimeTypes.Distinct().ToList();
                    ft.Extensions = ft.Extensions.Distinct().ToList();
                    if (!l.Contains(ft))
                        l.Add(ft);
                }
            }

            return l;
        }

        /// <summary>
    /// Retorna uma lista de strings contendo todos os MIME Types
    /// </summary>
    /// <returns></returns>
        public static IEnumerable<string> GetFileTypeStringList(FileTypeList FileTypeList = null)
        {
            return (FileTypeList ?? GetFileTypeList()).SelectMany(x => x.GetMimeTypesOrDefault()).Distinct();
        }

        /// <summary>
    /// Traz uma lista de extensões de acordo com o MIME type especificado
    /// </summary>
    /// <param name="MIME">MIME Type String</param>
    /// <returns></returns>
        public static List<string> GetExtensions(string MIME, FileTypeList FileTypeList = null)
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

        public static FileTypeList GetFileType(IEnumerable<string> MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null)
        {
            return new FileTypeList(MimeTypeOrExtensionOrPathOrDataURI.Select(x => GetFileType(x)).ToArray());
        }

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
                return new DataURI(MimeTypeOrExtensionOrPathOrDataURI).ToFileType();
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
            }

            if (!ismime)
            {
                MimeTypeOrExtensionOrPathOrDataURI = "." + MimeTypeOrExtensionOrPathOrDataURI.TrimAny(true, " ", ".");
            }

            return (FileTypeList ?? GetFileTypeList()).FirstOr(x => x.Extensions.ToArray().Union(x.GetMimeTypesOrDefault().ToArray()).Contains(MimeTypeOrExtensionOrPathOrDataURI, StringComparer.InvariantCultureIgnoreCase), new FileType());
        }

        public IEnumerable<FileInfo> SearchFiles(DirectoryInfo Directory, SearchOption SearchOption = SearchOption.AllDirectories)
        {
            return Directory.SearchFiles(SearchOption, Extensions.Select(ext => "*" + ext.PrependIf(".", !ext.StartsWith("."))).ToArray());
        }

        /// <summary>
    /// Constroi um MIME Type Default
    /// </summary>
        public FileType()
        {
        }

        /// <summary>
    /// Constroi um File Type a partir de um Arquivo (FileInfo)
    /// </summary>
    /// <param name="File">Fileinfo com o Arquivo</param>
        public FileType(FileInfo File, FileTypeList FileTypeList = null)
        {
            Build(File.Extension, FileTypeList);
        }

        /// <summary>
    /// Constroi um File Type a partir da extensão ou MIME Type de um Arquivo
    /// </summary>
    /// <param name="MimeTypeOrExtensionOrPathOrDataURI">Extensão do arquivo</param>
        public FileType(string MimeTypeOrExtensionOrPathOrDataURI, FileTypeList FileTypeList = null)
        {
            Build(MimeTypeOrExtensionOrPathOrDataURI.ToLower(), FileTypeList);
        }

        internal void Build(string Extension, FileTypeList FileTypeList = null)
        {
            var item = GetFileType(Extension, FileTypeList);
            Extensions = item.Extensions;
            MimeTypes = item.MimeTypes;
            Description = item.Description.ToProperCase();
        }

        /// <summary>
    /// Retorna uma string com o primeiro MIME TYPE do arquivo
    /// </summary>
    /// <returns></returns>
        public override string ToString()
        {
            return GetMimeTypesOrDefault().First();
        }

        /// <summary>
    /// Retorna uma string representando um filtro de caixa de dialogo WinForms
    /// </summary>
    /// <returns></returns>
        public string ToFilterString()
        {
            string r = "";
            foreach (var ext in Extensions)
                r += "*" + ext + ";";
            r = Description + "|" + r.RemoveLastEqual(";");
            return r;
        }
    }

    /// <summary>
/// Lista com Tipos de arquivo ultilizada para filtro e validação
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
            AddRange(FileTypes ?? Array.Empty<FileType>());
        }

        /// <summary>
    /// Cria uma nova lista a partir de uma lista de tipos de arquivos
    /// </summary>
    /// <param name="FileTypeList">Tipos de Arquivos</param>
        public FileTypeList(List<FileType> FileTypeList) : this((FileTypeList ?? new List<FileType>()).ToArray())
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

        /// <summary>
    /// Retorna uma string representando um filtro de caixa de dialogo WinForms
    /// </summary>
    /// <returns></returns>
        public string ToFilterString()
        {
            string r = "";
            foreach (var ext in this)
                r += ext.ToFilterString() + "|";
            return r.RemoveLastEqual("|");
        }

        /// <summary>
    /// Busca arquivos que correspondam com as extensões desta lista
    /// </summary>
    /// <param name="Directory">   Diretório</param>
    /// <param name="SearchOption">Tipo de busca</param>
    /// <returns></returns>
        public IEnumerable<FileInfo> SearchFiles(DirectoryInfo Directory, SearchOption SearchOption = SearchOption.AllDirectories)
        {
            return Directory.SearchFiles(SearchOption, Extensions.Select(ext => "*" + ext.PrependIf(".", !ext.StartsWith("."))).ToArray());
        }

        /// <summary>
    /// Retorna todas as extensões da lista
    /// </summary>
    /// <returns></returns>
        public IEnumerable<string> Extensions
        {
            get
            {
                return this.SelectMany(x => x.Extensions).Distinct();
            }
        }

        /// <summary>
    /// Retorna todas os MIME Types da lista
    /// </summary>
    /// <returns></returns>
        public IEnumerable<string> MimeTypes
        {
            get
            {
                return this.SelectMany(x => x.GetMimeTypesOrDefault()).Distinct();
            }
        }

        public IEnumerable<string> SubTypes
        {
            get
            {
                return this.SelectMany(x => x.SubTypes).Distinct();
            }
        }

        public IEnumerable<string> FirstTypes
        {
            get
            {
                return this.SelectMany(x => x.FirstTypes).Distinct();
            }
        }

        public IEnumerable<string> Descriptions
        {
            get
            {
                return (IEnumerable<string>)this.SelectMany(x => x.Description).Distinct();
            }
        }
    }
}