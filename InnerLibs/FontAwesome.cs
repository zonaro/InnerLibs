using InnerLibs.LINQ;
using System.IO;

namespace InnerLibs.FontAwesome
{
    public static class FontAwesome
    {
        /// <summary>
        /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo
        /// </summary>
        /// <param name="Extension">Arquivo</param>
        /// <returns></returns>
        public static string GetIconByFileExtension(this string Extension)
        {
            switch (Extension.RemoveAny(".").ToLower() ?? "")
            {
                case "vcf":
                case "vcard":
                    {
                        return "fa-address-card";
                    }
                case "ics":
                case "ical":
                case "ifb":
                case "icalendar":
                    {
                        return "fa-calendar";
                    }
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
                case "webp":
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
                case "css":
                case "aspx":
                case "ascx":
                case "ashx":
                case "config":
                case "json":
                case "jsx":
                case "js":
                case "ts":
                case "vbs":
                case "ps1":
                    {
                        return "fa-file-code";
                    }
                case "apk":
                case "appbundle":
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
                case "gz":
                case "7zip":
                case "7z":
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
                case "smc":
                case "sfc":
                case "wad":
                case "ndc":
                case "gci":
                case "3ds":
                case "nes":
                case "snes":
                case "cia":
                case "gcz":
                    {
                        return "fa-gamepad";
                    }
                case "iso":
                case "ape":
                case "bwt":
                case "ccd":
                case "cdi":
                case "cue":
                case "b5t":
                case "b6t":
                    {
                        return "fa-compact-disc";
                    }

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
                case "db":
                case "sqlite":
                case "litedb":
                case "mdb":
                case "mdf":
                    {
                        return "fa-database";
                    }
                case "bak":
                    {
                        return "fa-copy";
                    }
                case "jar":
                    {
                        return "fa-java";
                    }

                default:
                    {
                        break;
                    }
            }

            return "fa-file";
        }

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
        public static string GetIconByFileType(this FileType MIME) => MIME.Extensions.FirstOr("").GetIconByFileExtension();
    }
}