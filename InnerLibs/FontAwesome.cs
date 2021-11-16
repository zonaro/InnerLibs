using System.IO;

namespace InnerLibs.FontAwesome
{
    public static class FontAwesome
    {


        /// <summary>
        /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo ou diretório
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string GetIconByFileType(this FileSystemInfo File, bool DirectoryOpen = false)
        {
            if (File.Attributes == FileAttributes.Device)
            {
                return "fa-plug";
            }
            else if (File.Attributes == FileAttributes.Directory)
            {
                return DirectoryOpen ? "fa-folder-open" : "fa-folder";
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
                case "svg":
                case "eps":
                case "tiff":
                case "cdr":
                    {
                        return "fa-file-image";
                    }

                case "doc":
                case "docx":
                    {
                        return "fa-file-word";
                    }

                case "pdf":
                    {
                        return "fa-file-pdf";
                    }

                case "ppt":
                case "pptx":
                    {
                        return "fa-file-powerpoint";
                    }

                case "xls":
                case "xlsx":
                    {
                        return "fa-file-excel";
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
                case "json":
                case "ps1":
                    {
                        return "fa-file-code";
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
                        return "fa-file-archive";
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
                        return "fa-file-video";
                    }

                case "txt":
                case "otf":
                case "otd":
                case "ttf":
                case "rtf":
                case "xps":
                case "cfg":
                    {
                        return "fa-file-text";
                    }

                case "csv":
                    {
                        return "fa-file-csv";
                    }

                case "mp3":
                case "mp2":
                case "wma":
                case "wav":
                case "ogg":
                case "flac":
                case "aac":
                    {
                        return "fa-file-audio";
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

            return "fa-file";
        }


    }
}