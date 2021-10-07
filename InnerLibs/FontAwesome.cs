using System.IO;

namespace InnerLibs
{
    public static class FontAwesome
    {


        /// <summary>
    /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo ou diretório
    /// </summary>
    /// <param name="File">Arquivo</param>
    /// <returns></returns>
        public static string GetIconByFileType(this FileSystemInfo File, bool DirectoryOpen = false, bool InvertIcon = false)
        {
            if (File.Attributes == FileAttributes.Device)
            {
                return "fa-plug";
            }

            if (File.Attributes == FileAttributes.Directory)
            {
                switch (true)
                {
                    case object _ when DirectoryOpen & InvertIcon:
                        {
                            return "fa-folder-open-o";
                        }

                    case object _ when DirectoryOpen & !InvertIcon:
                        {
                            return "fa-folder-open";
                        }

                    case object _ when !DirectoryOpen & InvertIcon:
                        {
                            return "fa-folder-o";
                        }

                    default:
                        {
                            return "fa-folder";
                        }
                }
            }
            else
            {
                return File.Extension.GetIconByFileExtension();
            }
        }

        /// <summary>
    /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo
    /// </summary>
    /// <param name="MIME">MIME Type do Arquivo</param>
    /// <returns></returns>
        public static string GetIconByFileType(this FileType MIME)
        {
            return MIME.Extensions.FirstOr("").GetIconByFileExtension();
        }

        /// <summary>
    /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo
    /// </summary>
    /// <param name="Extension">Arquivo</param>
    /// <returns></returns>
        public static string GetIconByFileExtension(this string Extension)
        {
            switch (Extension.RemoveAny(".").ToLower() ?? "")
            {
                case "png":
                case "jpg":
                case "gif":
                case "jpeg":
                case "psd":
                case "ai":
                case "drw":
                case "ttf":
                case "svg":
                case "eps":
                case "tiff":
                case "cdr":
                    {
                        return "fa-file-picture-o";
                    }

                case "doc":
                case "docx":
                    {
                        return "fa-file-word-o";
                    }

                case "pdf":
                    {
                        return "fa-file-pdf-o";
                    }

                case "ppt":
                case "pptx":
                    {
                        return "fa-file-powerpoint-o";
                    }

                case "xls":
                case "xlsx":
                    {
                        return "fa-file-excel-o";
                    }

                case "html":
                case "htm":
                case "php":
                case "cpp":
                case "vb":
                case "cs":
                case "jsp":
                case "xml":
                case "js":
                case "css":
                case "aspx":
                case "ascx":
                case "ashx":
                case "config":
                case "ps1":
                    {
                        return "fa-file-code-o";
                    }

                case "apk":
                    {
                        return "fa-android";
                    }

                case "ios":
                case "ipa":
                    {
                        return "fa-apple";
                    }

                case "xap":
                case "appx":
                    {
                        return "fa-windows";
                    }

                case "zip":
                case "rar":
                case "tar":
                case "jar":
                case "gz":
                case "iso":
                case "7zip":
                case "b1":
                case "bar":
                case "rar5":
                case "pk3":
                case "pkg":
                    {
                        return "fa-file-archive-o";
                    }

                case "avi":
                case "mpeg":
                case "mp4":
                case "3gp":
                case "mkv":
                case "wmv":
                case "rmvb":
                case "mov":
                case "webm":
                case "ogv":
                    {
                        return "fa-file-video-o";
                    }

                case "txt":
                case "otf":
                case "otd":
                case var @case when @case == "ttf":
                case "rtf":
                case "csv":
                case "xps":
                case "cfg":
                    {
                        return "fa-file-text-o";
                    }

                case "mp3":
                case "mp2":
                case "wma":
                case "wav":
                case "ogg":
                case "flac":
                case "aac":
                    {
                        return "fa-file-audio-o";
                    }

                case "gb":
                case "gba":
                case "n64":
                case "rom":
                case "z64":
                case "gbc":
                case "wad":
                    {
                        return "fa-gamepad";
                    }

                case "bin":
                case "dll":
                    {
                        return "fa-cog";
                    }

                case "exe":
                case "bat":
                case "msi":
                    {
                        return "fa-window-maximize";
                    }

                case "sql":
                    {
                        return "fa-database";
                    }

                default:
                    {
                        break;
                    }
            }

            return "fa-file-o";
        }

        public const string CDNFontAwesomeCSS = "https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css";
    }
}