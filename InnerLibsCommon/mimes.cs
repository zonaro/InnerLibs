
using System.Collections.Generic;
using Extensions.Colors;

namespace Extensions.Files
{
    public partial class FileType
    {

        public static   FileTypeList AllTypes  { get; private set; } = new FileTypeList()
            {
                new FileType
                {
                    Description = "Internet Telephony",
                    Extensions = new string[] { ".323" },
                    MimeTypes = new string[] { "application/octet-stream", "text/h323" },
                    Categories = new string[] { "Media", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Adaptive Multi-Rate ACELP Codec",
                    Extensions = new string[] { ".amr" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/amr" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Aac Audio File",
                    Extensions = new string[] { ".aac" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/aac" },
                    Categories = new string[] { "Audio", "Media", "Music" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Abiword Document",
                    Extensions = new string[] { ".abw" },
                    MimeTypes = new string[] { "application/octet-stream", "application/abiword" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Atari St Executable",
                    Extensions = new string[] { ".acx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/internet-property-stream" },
                    Categories = new string[] { "Software" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Adobe Illustrator File",
                    Extensions = new string[] { ".ai" },
                    MimeTypes = new string[] { "application/octet-stream", "application/illustrator" },
                    Categories = new string[] { "Design", "Adobe", "Media", "Picture", "Vector" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Aiff Audio File",
                    Extensions = new string[] { ".aif", ".aifc", ".aiff" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/aiff", "audio/aifc", "audio/x-aiff" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Windows Media File",
                    Extensions = new string[] { ".asf", ".asr", ".asx", ".wmf" },
                    MimeTypes = new string[] { "application/octet-stream", "video/x-ms-asf", "image/x-wmf", "application/wmf", "application/x-msmetafile", "application/x-wmf", "image/wmf", "image/x-win-metafile" },
                    Categories = new string[] { "Media", "Video", "Windows", "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Asp Source File",
                    Extensions = new string[] { ".asp" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-asp", "text/asp" },
                    Categories = new string[] { "Source Code", "Web", "Windows" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Audio File",
                    Extensions = new string[] { ".au", ".snd" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/basic", "audio/au", "audio/x-au", "audio/x-basic" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Avi Video File",
                    Extensions = new string[] { ".avi" },
                    MimeTypes = new string[] { "application/octet-stream", "video/avi", "application/x-troff-msvideo", "image/avi", "video/msvideo", "video/x-msvideo", "video/xmpg2" },
                    Categories = new string[] { "Media", "Video" },
                    Color = new HSVColor("#4B0082")
                },
                new FileType
                {
                    Description = "Activex Script",
                    Extensions = new string[] { ".axs" },
                    MimeTypes = new string[] { "application/octet-stream", "application/olescript" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Basic Source Code",
                    Extensions = new string[] { ".bas" },
                    MimeTypes = new string[] { "application/octet-stream", "text/plain" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Binary File",
                    Extensions = new string[] { ".bin" },
                    MimeTypes = new string[] { "application/octet-stream", "application/bin", "application/binary", "application/x-msdownload" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Bitmap Image",
                    Extensions = new string[] { ".bmp", ".dib", ".xbm" },
                    MimeTypes = new string[] { "application/octet-stream", "image/bmp", "application/bmp", "application/x-bmp", "image/ms-bmp", "image/x-bitmap", "image/x-bmp", "image/x-ms-bmp", "image/x-win-bitmap", "image/x-windows-bmp", "image/x-xbitmap" },
                    Categories = new string[] { "Media", "Picture", "Windows" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Compressed Bzip2 File",
                    Extensions = new string[] { ".bz2" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-bzip2", "application/bzip2", "application/x-bz2", "application/x-bzip" },
                    Categories = new string[] { "Archive", "Compressed", "Linux" },
                    Color = new HSVColor("#8B4513")
                },
                new FileType
                {
                    Description = "C Source File",
                    Extensions = new string[] { ".c" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-csrc" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "C++ Source File",
                    Extensions = new string[] { ".c++", ".cp", ".cpp" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-c++src" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Microsoft Cabinet Archive",
                    Extensions = new string[] { ".cab" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-cab-compressed", "application/cab", "application/x-cabinet" },
                    Categories = new string[] { "Archive", "Compressed", "Windows", "Microsoft" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Security Catalog",
                    Extensions = new string[] { ".cat" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-pkiseccat" },
                    Categories = new string[] { "Security", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Adobe Director File",
                    Extensions = new string[] { ".cct", ".cst", ".cxt", ".dcr", ".dir", ".dxr", ".fqd", ".swa", ".w3d" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-director" },
                    Categories = new string[] { "Adobe", "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Channel Definition Format",
                    Extensions = new string[] { ".cdf" },
                    MimeTypes = new string[] { "application/octet-stream", "application/cdf", "application/x-cdf", "application/netcdf", "application/x-netcdf", "text/cdf", "text/x-cdf" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Internet Security Certificate File",
                    Extensions = new string[] { ".cer", ".crt", ".der", ".p12" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-x509-ca-cert", "application/pkix-cert", "application/x-pkcs12", "application/keychain_access" },
                    Categories = new string[] { "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Coldfusion Source File",
                    Extensions = new string[] { ".cfc", ".cfm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-cfm" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Java Bytecode File",
                    Extensions = new string[] { ".class" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-java", "application/java", "application/java-byte-code", "application/java-vm", "application/x-java-applet", "application/x-java-bean", "application/x-java-class", "application/x-java-vm", "application/x-jinit-bean", "application/x-jinit-applet" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Clipboard/Picture",
                    Extensions = new string[] { ".clp" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-msclip" },
                    Categories = new string[] { "Media", "Picture", "Windows" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Presentation Exchange Image",
                    Extensions = new string[] { ".cmx" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-cmx", "application/cmx", "application/x-cmx", "drawing/cmx" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Cis-Cod File",
                    Extensions = new string[] { ".cod" },
                    MimeTypes = new string[] { "application/octet-stream", "image/cis-cod" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Unix Cpio Archive",
                    Extensions = new string[] { ".cpio" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-cpio" },
                    Categories = new string[] { "Archive", "Compressed", "Linux" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Cardfile",
                    Extensions = new string[] { ".crd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-mscardfile" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Certificate Revocation List",
                    Extensions = new string[] { ".crl" },
                    MimeTypes = new string[] { "application/octet-stream", "application/pkix-crl" },
                    Categories = new string[] { "Security", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Certificate File",
                    Extensions = new string[] { ".crt" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-x509-ca-cert", "application/pkix-cert", "application/keychain_access" },
                    Categories = new string[] { "Security", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "C Shell File",
                    Extensions = new string[] { ".csh" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-csh" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Cascading Stylesheet File",
                    Extensions = new string[] { ".css" },
                    MimeTypes = new string[] { "application/octet-stream", "text/css", "application/css-stylesheet" },
                    Categories = new string[] { "Source Code", "Web" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Comma-Delimited File",
                    Extensions = new string[] { ".csv" },
                    MimeTypes = new string[] { "application/octet-stream", "text/csv", "application/csv", "text/comma-separated-values", "text/x-comma-separated-values" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#696969")
                },
                new FileType
                {
                    Description = "Patch Source File",
                    Extensions = new string[] { ".diff", ".patch" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-patch" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Dynamic Link Library",
                    Extensions = new string[] { ".dll" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-msdownload", "application/x-msdos-program" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Word Document",
                    Extensions = new string[] { ".doc", ".docm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-word", "application/doc", "application/msword", "application/msword-doc", "application/vnd.msword", "application/winword", "application/word", "application/x-msw6", "application/x-msword", "application/x-msword-doc", "application/vnd.ms-word.document.macroenabled.12" },
                    Categories = new string[] { "Document", "Office", "Microsoft" },
                    Color = new HSVColor("#2B579A")
                },
                new FileType
                {
                    Description = "Microsoft Word File",
                    Extensions = new string[] { ".docx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.ms-word.document.12", "application/vnd.openxmlformats-officedocument.word" },
                    Categories = new string[] { "Document", "Microsoft", "Office" },
                    Color = new HSVColor("#2B579A")
                },
                new FileType
                {
                    Description = "Word Document Template",
                    Extensions = new string[] { ".dot" },
                    MimeTypes = new string[] { "application/octet-stream", "application/msword" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#2B579A")
                },
                new FileType
                {
                    Description = "Microsoft Word Template File",
                    Extensions = new string[] { ".dotm", ".dotx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-word.template.macroenabled.12", "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
                    Categories = new string[] { "Document", "Microsoft", "Office" },
                    Color = new HSVColor("#2B579A")
                },
                new FileType
                {
                    Description = "Stata Data File",
                    Extensions = new string[] { ".dta" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-stata" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Digital Video File",
                    Extensions = new string[] { ".dv" },
                    MimeTypes = new string[] { "application/octet-stream", "video/x-dv" },
                    Categories = new string[] { "Media", "Video" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Dvi File",
                    Extensions = new string[] { ".dvi" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-dvi" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Autocad Drawing",
                    Extensions = new string[] { ".dwg", ".dxf" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-dwg", "application/acad", "application/autocad_dwg", "application/dwg", "application/x-acad", "application/x-autocad", "application/x-dwg", "image/vnd.dwg", "application/dxf", "application/x-dxf", "drawing/x-dxf", "image/vnd.dxf", "image/x-autocad", "image/x-dxf" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Emacs Source File",
                    Extensions = new string[] { ".elc" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-elc" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Web Archive File",
                    Extensions = new string[] { ".eml", ".mail", ".mht", ".mhtml", ".nws" },
                    MimeTypes = new string[] { "application/octet-stream", "message/rfc822" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Endnote Library File",
                    Extensions = new string[] { ".enl", ".enz", ".lib" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-endnote-library", "application/x-endnote-refer" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Postscript File",
                    Extensions = new string[] { ".eps", ".ps" },
                    MimeTypes = new string[] { "application/octet-stream", "application/postscript", "application/eps", "application/x-eps", "image/eps", "image/x-eps" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "SeText (Structure Enhanced Text)",
                    Extensions = new string[] { ".etx" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-setext", "text/anytext" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Envoy Document",
                    Extensions = new string[] { ".evy" },
                    MimeTypes = new string[] { "application/octet-stream", "application/envoy", "application/x-envoy" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Executable File",
                    Extensions = new string[] { ".exe" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-msdos-program", "application/dos-exe", "application/exe", "application/msdos-windows", "application/x-sdlc", "application/x-exe", "application/x-winexe" },
                    Categories = new string[] { "Software", "Windows" },
                    Color = new HSVColor("#000000")
                },
                new FileType
                {
                    Description = "Fractal Image Format",
                    Extensions = new string[] { ".fif" },
                    MimeTypes = new string[] { "application/octet-stream", "application/fractals", "image/fif" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Virtual Reality Modeling Language File",
                    Extensions = new string[] { ".flr", ".vrml" },
                    MimeTypes = new string[] { "application/octet-stream", "x-world/x-vrml" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Framemaker File",
                    Extensions = new string[] { ".fm", ".mif" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.framemaker", "application/framemaker", "application/maker", "application/vnd.mif", "application/x-framemaker", "application/x-maker", "application/x-mif" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Gif Image",
                    Extensions = new string[] { ".gif" },
                    MimeTypes = new string[] { "application/octet-stream", "image/gif" },
                    Categories = new string[] { "Media", "Picture", "Web" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Compressed Tar File",
                    Extensions = new string[] { ".gtar", ".tar" },
                    MimeTypes = new string[] { "application/octet-stream", "application/tar", "application/x-gtar", "application/x-tar" },
                    Categories = new string[] { "Archive", "Compressed", "Linux" },
                    Color = new HSVColor("#8B4513")
                },
                new FileType
                {
                    Description = "Compressed Gzip Archive",
                    Extensions = new string[] { ".gz", ".tgz" },
                    MimeTypes = new string[] { "application/octet-stream", "application/gzip", "application/gzip-compressed", "application/gzipped", "application/x-gunzip", "application/x-gzip" },
                    Categories = new string[] { "Archive", "Compressed", "Linux" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "C Header File",
                    Extensions = new string[] { ".h" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-chdr" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Hierarchical Data Format File",
                    Extensions = new string[] { ".hdf" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-hdf" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Help File",
                    Extensions = new string[] { ".hlp" },
                    MimeTypes = new string[] { "application/octet-stream", "application/winhlp", "application/x-helpfile", "application/x-winhelp" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Binhex Archive",
                    Extensions = new string[] { ".hqx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/binhex", "application/mac-binhex", "application/mac-binhex40" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Html Application File",
                    Extensions = new string[] { ".hta" },
                    MimeTypes = new string[] { "application/octet-stream", "application/hta" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Html Component File",
                    Extensions = new string[] { ".htc" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-component" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Html File",
                    Extensions = new string[] { ".htm", ".html", ".shtml", ".xhtml" },
                    MimeTypes = new string[] { "application/octet-stream", "text/html", "application/xhtml+xml" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "HyperText Template File",
                    Extensions = new string[] { ".htt" },
                    MimeTypes = new string[] { "application/octet-stream", "text/webviewhtml" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Favicon, Icon File",
                    Extensions = new string[] { ".ico" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-ico" },
                    Categories = new string[] { "Media", "Picture", "Windows" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Calendar File",
                    Extensions = new string[] { ".ics" },
                    MimeTypes = new string[] { "application/octet-stream", "text/calendar" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Ief Image File",
                    Extensions = new string[] { ".ief" },
                    MimeTypes = new string[] { "application/octet-stream", "image/ief" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Intel Iphone Compatible File",
                    Extensions = new string[] { ".iii" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-iphone" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Adobe Indesign File",
                    Extensions = new string[] { ".indd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-indesign" },
                    Categories = new string[] { "Adobe", "Design", "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Iis Internet Communications Settings File",
                    Extensions = new string[] { ".ins" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-internet-signup" },
                    Categories = new string[] { "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Iis Internet Service Provider Settings File",
                    Extensions = new string[] { ".isp" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-internet-signup" },
                    Categories = new string[] { "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Java Application Descriptor",
                    Extensions = new string[] { ".jad" },
                    MimeTypes = new string[] { "application/octet-stream", "text/vnd.sun.j2me.app-descriptor" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Java Archive File",
                    Extensions = new string[] { ".jar" },
                    MimeTypes = new string[] { "application/octet-stream", "application/java-archive" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Java Source File",
                    Extensions = new string[] { ".java" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-java", "java/*", "text/java", "text/x-java-source" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Jpeg Image",
                    Extensions = new string[] { ".jpeg", ".jpg", ".jfif", ".jpe" },
                    MimeTypes = new string[] { "application/octet-stream", "image/jpeg", "image/pjpeg" },
                    Categories = new string[] { "Media", "Photo", "Picture" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Javascript Source File",
                    Extensions = new string[] { ".js" },
                    MimeTypes = new string[] { "application/octet-stream", "text/javascript", "application/javascript", "application/x-javascript", "application/x-js" },
                    Categories = new string[] { "Source Code", "Web" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Kml File",
                    Extensions = new string[] { ".kml" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.google-earth.kml+xml" },
                    Categories = new string[] { "Google", "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Compressed Kml File",
                    Extensions = new string[] { ".kmz" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.google-earth.kmz" },
                    Categories = new string[] { "Google", "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Latex File",
                    Extensions = new string[] { ".latex" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-latex", "text/x-latex" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Compressed Archive File",
                    Extensions = new string[] { ".lha", ".lzh" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-lha", "application/lha", "application/lzh", "application/x-lzh", "application/x-lzh-archive" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Labview Application File",
                    Extensions = new string[] { ".llb" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-labview", "application/x-labview-vi" },
                    Categories = new string[] { "Software" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Log Text File",
                    Extensions = new string[] { ".log" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-log" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#696969")
                },
                new FileType
                {
                    Description = "Streaming Audio/Video File",
                    Extensions = new string[] { ".lsf", ".lsx" },
                    MimeTypes = new string[] { "application/octet-stream", "video/x-la-asf" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Labview Cal Simulation File",
                    Extensions = new string[] { ".lvx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-labview-exec" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Objective C Source File",
                    Extensions = new string[] { ".m" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-objcsrc" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Mpeg Video File",
                    Extensions = new string[] { ".m1v", ".m2v", ".mpe", ".mpeg", ".mpg" },
                    MimeTypes = new string[] { "application/octet-stream", "video/mpeg" },
                    Categories = new string[] { "Media", "Video" },
                    Color = new HSVColor("#4B0082")
                },
                new FileType
                {
                    Description = "Mp3 Playlist File",
                    Extensions = new string[] { ".m3u", ".pls" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/x-mpegurl", "application/x-winamp-playlist", "audio/mpegurl", "audio/mpeg-url", "audio/playlist", "audio/scpls", "audio/x-scpls" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Mpeg-4 Audio File",
                    Extensions = new string[] { ".m4a" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/m4a", "audio/x-m4a" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Mpeg-4 Video File",
                    Extensions = new string[] { ".m4v", ".mp4" },
                    MimeTypes = new string[] { "application/octet-stream", "video/mp4", "video/mpeg4", "video/x-m4v" },
                    Categories = new string[] { "Media", "Video" },
                    Color = new HSVColor("#4B0082")
                },
                new FileType
                {
                    Description = "Mathematica File",
                    Extensions = new string[] { ".ma", ".nb" },
                    MimeTypes = new string[] { "application/octet-stream", "application/mathematica" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Troff With Man Macros File",
                    Extensions = new string[] { ".man" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-troff-man" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Mathcad File",
                    Extensions = new string[] { ".mcd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-mathcad", "application/mcad" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Access File",
                    Extensions = new string[] { ".mdb" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-access", "application/mdb", "application/msaccess", "application/vnd.msaccess", "application/x-mdb", "application/x-msaccess" },
                    Categories = new string[] { "Database", "Office", "Microsoft", "Windows" },
                    Color = new HSVColor("#A4373A")
                },
                new FileType
                {
                    Description = "Troff With Me Macros File",
                    Extensions = new string[] { ".me" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-troff-me" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Adobe Flash File",
                    Extensions = new string[] { ".mfp", ".spl", ".swf" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-shockwave-flash", "application/futuresplash" },
                    Categories = new string[] { "Adobe", "Games", "Video", "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Free Lossless Audio Codec",
                    Extensions = new string[] { ".flac" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/flac" },
                    Categories = new string[] { "Audio", "Lossless" },
                    Color = new HSVColor("#009688")
                },
                new FileType
                {
                    Description = "Midi Audio File",
                    Extensions = new string[] { ".mid", ".midi" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/x-midi", "application/x-midi", "audio/mid", "audio/midi", "audio/soundtrack" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Money Data File",
                    Extensions = new string[] { ".mny" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-msmoney" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Quicktime Video File",
                    Extensions = new string[] { ".mov", ".mqv", ".qt" },
                    MimeTypes = new string[] { "application/octet-stream", "video/quicktime" },
                    Categories = new string[] { "Media", "Video" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Mpeg Layer 2 Audio File",
                    Extensions = new string[] { ".mp2" },
                    MimeTypes = new string[] { "application/octet-stream", "video/mpeg", "audio/x-mpeg", "audio/x-mpeg-2", "video/x-mpeg", "video/x-mpeq2a" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Mp3 Audio File",
                    Extensions = new string[] { ".mp3" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/mpeg", "audio/mp3", "audio/mpeg3", "audio/mpg", "audio/x-mp3", "audio/x-mpeg", "audio/x-mpeg3", "audio/x-mpg" },
                    Categories = new string[] { "Audio", "Media", "Music" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Mpeg Audio Stream",
                    Extensions = new string[] { ".mpa", ".mpv2" },
                    MimeTypes = new string[] { "application/octet-stream", "video/mpeg" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Project File",
                    Extensions = new string[] { ".mpp" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-project", "application/mpp", "application/msproj", "application/msproject", "application/x-dos_ms_project", "application/x-ms-project", "application/x-msproject" },
                    Categories = new string[] { "Microsoft", "Document", "Office", "Diagram", "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Troff With Ms Macros File",
                    Extensions = new string[] { ".ms" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-troff-ms" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Multimedia Viewer",
                    Extensions = new string[] { ".mvb" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-msmediaview" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Maple Worksheet File",
                    Extensions = new string[] { ".mws" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-maple", "application/maple-v-r4" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Oda Document",
                    Extensions = new string[] { ".oda" },
                    MimeTypes = new string[] { "application/octet-stream", "application/oda" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Openoffice Formula File",
                    Extensions = new string[] { ".odf" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.oasis.opendocument.formula" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Openoffice Graphics File",
                    Extensions = new string[] { ".odg" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.oasis.opendocument.graphics" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Openoffice Presentation File",
                    Extensions = new string[] { ".odp" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.oasis.opendocument.presentation" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Openoffice Spreadsheet File",
                    Extensions = new string[] { ".ods" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.oasis.opendocument.spreadsheet" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Openoffice Document",
                    Extensions = new string[] { ".odt" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.oasis.opendocument.text" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Ogg Audio File",
                    Extensions = new string[] { ".ogg" },
                    MimeTypes = new string[] { "application/octet-stream", "application/ogg", "application/x-ogg", "audio/x-ogg" },
                    Categories = new string[] { "Audio", "Media", "Music" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Microsoft Onenote File",
                    Extensions = new string[] { ".one" },
                    MimeTypes = new string[] { "application/octet-stream", "application/msonenote" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#7719AA")
                },
                new FileType
                {
                    Description = "Portable Bitmap Image",
                    Extensions = new string[] { ".pbm" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-portable-bitmap", "image/pbm", "image/portable-bitmap", "image/x-pbm" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Kodak Photo Cd File",
                    Extensions = new string[] { ".pcd" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-photo-cd", "image/pcd" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Macintosh Quickdraw Image",
                    Extensions = new string[] { ".pct", ".pic", ".pict" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-pict", "image/pict", "image/x-macpict" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Adobe Acrobat Document",
                    Extensions = new string[] { ".pdf" },
                    MimeTypes = new string[] { "application/octet-stream", "application/pdf", "application/acrobat", "application/nappdf", "application/x-pdf", "application/vnd.pdf", "text/pdf", "text/x-pdf" },
                    Categories = new string[] { "Document", "Office", "Adobe" },
                    Color = new HSVColor("#DC143C")
                },
                new FileType
                {
                    Description = "Personal Information Exchange File",
                    Extensions = new string[] { ".pfx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-pkcs12" },
                    Categories = new string[] { "Security", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Portable Graymap Image",
                    Extensions = new string[] { ".pgm" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-portable-graymap", "image/x-pgm" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Php Source File",
                    Extensions = new string[] { ".php" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-php", "application/php", "text/php", "text/x-php" },
                    Categories = new string[] { "Source Code", "Web" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Perl Source File",
                    Extensions = new string[] { ".pl", ".pm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-perl", "text/x-perl" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Publickey Security Object",
                    Extensions = new string[] { ".pko" },
                    MimeTypes = new string[] { "application/octet-stream", "application/ynd.ms-pkipko" },
                    Categories = new string[] { "Security", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Performance Monitor File",
                    Extensions = new string[] { ".pmc" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-perfmon" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Portable Network Graphics Image",
                    Extensions = new string[] { ".png" },
                    MimeTypes = new string[] { "application/octet-stream", "image/png", "image/x-png" },
                    Categories = new string[] { "Media", "Picture", "Web" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Portable Any Map Graphic Bitmap",
                    Extensions = new string[] { ".pnm" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-portable-anymap" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Perl Documentation File",
                    Extensions = new string[] { ".pod" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-pod" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Powerpoint Template File",
                    Extensions = new string[] { ".potm", ".potx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-powerpoint.template.macroenabled.12", "application/vnd.openxmlformats-officedocument.presentationml.template" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#D83B01")
                },
                new FileType
                {
                    Description = "Microsoft Powerpoint File",
                    Extensions = new string[] { ".ppam", ".pps", ".ppsm", ".ppsx", ".ppt", ".pptm", ".pptx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-powerpoint.addin.macroenabled.12", "application/vnd.ms-powerpoint", "application/ms-powerpoint", "application/mspowerpoint", "application/powerpoint", "application/ppt", "application/vnd-mspowerpoint", "application/vnd_ms-powerpoint", "application/x-mspowerpoint", "application/x-powerpoint", "application/vnd.ms-powerpoint.slideshow.macroenabled.12", "application/vnd.openxmlformats-officedocument.presentationml.slideshow", "application/vnd.ms-powerpoint.presentation.macroenabled.12", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#D83B01")
                },
                new FileType
                {
                    Description = "Portable Pixmap Image",
                    Extensions = new string[] { ".ppm" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-portable-pixmap", "application/ppm", "application/x-ppm", "image/x-ppm" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Pics Rules File",
                    Extensions = new string[] { ".prf" },
                    MimeTypes = new string[] { "application/octet-stream", "application/pics-rules" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Adobe Photoshop File",
                    Extensions = new string[] { ".psd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/photoshop", "application/psd", "application/x-photoshop", "image/photoshop", "image/psd", "image/x-photoshop", "image/x-psd" },
                    Categories = new string[] { "Asset", "Design", "Media", "Adobe", "Picture" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Microsoft Publisher File",
                    Extensions = new string[] { ".pub" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-publisher", "application/x-mspublisher" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Python Source File",
                    Extensions = new string[] { ".py" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-python" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Realaudio File",
                    Extensions = new string[] { ".ra", ".ram", ".rpm" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/vnd.rn-realaudio", "audio/vnd.pn-realaudio", "audio/x-pn-realaudio", "audio/x-pn-realaudio-plugin", "audio/x-pn-realvideo", "audio/x-realaudio" },
                    Categories = new string[] { "Audio", "Media" },
                    Color = new HSVColor("#000000")
                },
                new FileType
                {
                    Description = "Compressed Archive",
                    Extensions = new string[] { ".rar" },
                    MimeTypes = new string[] { "application/octet-stream", "application/rar", "application/x-rar-compressed" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#8B4513")
                },
                new FileType
                {
                    Description = "Raster Image",
                    Extensions = new string[] { ".ras" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-cmu-raster" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Iris Image",
                    Extensions = new string[] { ".rgb" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-rgb", "image/rgb" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Realmedia File",
                    Extensions = new string[] { ".rm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.rn-realmedia" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Radio Midi File",
                    Extensions = new string[] { ".rmi" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/mid" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Troff File",
                    Extensions = new string[] { ".roff", ".t", ".tr" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-troff" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Rich Text Format File",
                    Extensions = new string[] { ".rtf", ".rtx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/rtf", "application/richtext", "application/x-rtf", "text/richtext", "text/rtf" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Realvideo File",
                    Extensions = new string[] { ".rv" },
                    MimeTypes = new string[] { "application/octet-stream", "video/vnd.rn-realvideo", "video/x-pn-realvideo" },
                    Categories = new string[] { "Media", "Video" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Sas File",
                    Extensions = new string[] { ".sas" },
                    MimeTypes = new string[] { "application/octet-stream", "application/sas", "application/x-sas", "application/x-sas-data", "application/x-sas-log", "application/x-sas-output" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Spss File",
                    Extensions = new string[] { ".sav", ".sd2", ".spo" },
                    MimeTypes = new string[] { "application/octet-stream", "application/spss" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Schedule Data",
                    Extensions = new string[] { ".scd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-msschedule" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Scheme File",
                    Extensions = new string[] { ".scm" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-script.scheme", "text/x-scheme" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Script Component",
                    Extensions = new string[] { ".sct" },
                    MimeTypes = new string[] { "application/octet-stream", "text/scriptlet" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Self-Extracting Archive",
                    Extensions = new string[] { ".sea" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-sea" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Shell Script File",
                    Extensions = new string[] { ".sh" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-sh", "application/x-shellscript" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Unix Shar Archive File",
                    Extensions = new string[] { ".shar" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-shar" },
                    Categories = new string[] { "Archive", "Compressed", "Linux" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Stuffit Archive",
                    Extensions = new string[] { ".sit" },
                    MimeTypes = new string[] { "application/octet-stream", "application/stuffit", "application/x-sit", "application/x-stuffit" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Smil File",
                    Extensions = new string[] { ".smil" },
                    MimeTypes = new string[] { "application/octet-stream", "application/smil", "application/smil+xml" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Sql File",
                    Extensions = new string[] { ".sql" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-sql", "text/sql" },
                    Categories = new string[] { "Database" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Wais Source File",
                    Extensions = new string[] { ".src" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-wais-source" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Certificate Store Crypto Shell Extension",
                    Extensions = new string[] { ".sst" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-pkicertstore" },
                    Categories = new string[] { "Security", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Certificate Trust List",
                    Extensions = new string[] { ".stl" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-pkistl" },
                    Categories = new string[] { "Security", "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Shtml File",
                    Extensions = new string[] { ".stm" },
                    MimeTypes = new string[] { "application/octet-stream", "text/html" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Openoffice.Org Document",
                    Extensions = new string[] { ".sxw" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.sun.xml.writer" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Tcl Source File",
                    Extensions = new string[] { ".tcl" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-tcl", "text/x-script.tcl", "text/x-tcl" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Tex File",
                    Extensions = new string[] { ".tex" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-tex", "text/x-tex" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Truevision Targa Image",
                    Extensions = new string[] { ".tga" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-targa", "application/tga", "application/x-targa", "application/x-tga", "image/targa", "image/tga", "image/x-tga" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Tagged Image File",
                    Extensions = new string[] { ".tif", ".tiff" },
                    MimeTypes = new string[] { "application/octet-stream", "image/tiff", "application/tif", "application/tiff", "application/x-tif", "application/x-tiff", "image/tif", "image/x-tif", "image/x-tiff" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Microsoft Exchange Tnef File",
                    Extensions = new string[] { ".tnef" },
                    MimeTypes = new string[] { "application/octet-stream", "application/ms-tnef" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Terminal Settings",
                    Extensions = new string[] { ".trm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-msterminal" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Tab-Delimited File",
                    Extensions = new string[] { ".tsv" },
                    MimeTypes = new string[] { "application/octet-stream", "text/tsv", "text/tab-separated-values", "text/x-tab-separated-values" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Tableau Workbook File",
                    Extensions = new string[] { ".twb", ".twbx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/twb", "application/twbx", "application/x-twb" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Plain Text File",
                    Extensions = new string[] { ".txt" },
                    MimeTypes = new string[] { "application/octet-stream", "text/plain" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#696969")
                },
                new FileType
                {
                    Description = "Internet Location Service",
                    Extensions = new string[] { ".uls" },
                    MimeTypes = new string[] { "application/octet-stream", "text/iuls" },
                    Categories = new string[] { "Web" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Posix Tar Compressed Archive",
                    Extensions = new string[] { ".ustar" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-ustar" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Vcard File",
                    Extensions = new string[] { ".vcf" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-vcard" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Visio Document",
                    Extensions = new string[] { ".vsd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.visio", "application/visio", "application/visio.drawing", "application/vsd", "application/x-visio", "application/x-vsd", "image/x-vsd" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#3955A3")
                },
                new FileType
                {
                    Description = "Kde Web Archive",
                    Extensions = new string[] { ".war" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-webarchive" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Waveform Audio File",
                    Extensions = new string[] { ".wav" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/wav", "audio/s-wav", "audio/wave", "audio/x-wav" },
                    Categories = new string[] { "Audio", "Media", "Music" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Works File Transmission",
                    Extensions = new string[] { ".wcm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-works" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Works File",
                    Extensions = new string[] { ".wdb", ".wks", ".wps" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-works", "application/x-msworks-wp" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Media Audio File",
                    Extensions = new string[] { ".wma" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/x-ms-wma" },
                    Categories = new string[] { "Audio", "Media", "Music" },
                    Color = new HSVColor("#FF8C00")
                },
                new FileType
                {
                    Description = "Windows Media Video File",
                    Extensions = new string[] { ".wmv" },
                    MimeTypes = new string[] { "application/octet-stream", "video/x-ms-wmv" },
                    Categories = new string[] { "Media", "Video", "Windows" },
                    Color = new HSVColor("#4B0082")
                },
                new FileType
                {
                    Description = "Windows Media Compressed File",
                    Extensions = new string[] { ".wmz" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-ms-wmz" },
                    Categories = new string[] { "Archive", "Compressed", "Windows" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Wordperfect Document",
                    Extensions = new string[] { ".wpd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/wordperfect", "application/wordperf", "application/wpd" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Write Document",
                    Extensions = new string[] { ".wri" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-mswrite" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Vrml 3D File",
                    Extensions = new string[] { ".wrl", ".wrz" },
                    MimeTypes = new string[] { "application/octet-stream", "x-world/x-vrml" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Excel Add-In",
                    Extensions = new string[] { ".xla" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-excel" },
                    Categories = new string[] { "Document", "Office", "Microsoft" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Excel Document",
                    Extensions = new string[] { ".xlam", ".xll", ".xls", ".xlsb", ".xlsm", ".xlsx", ".xltm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-excel.addin.macroenabled.12", "application/vnd.ms-excel", "application/excel", "application/msexcel", "application/msexcell", "application/x-dos_ms_excel", "application/x-excel", "application/x-ms-excel", "application/x-msexcel", "application/x-xls", "application/xls", "application/vnd.ms-excel.sheet.binary.macroenabled.12", "application/vnd.ms-excel.sheet.macroenabled.12", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.ms-excel.template.macroenabled.12" },
                    Categories = new string[] { "Document", "Microsoft", "Office" },
                    Color = new HSVColor("#107C10")
                },
                new FileType
                {
                    Description = "Excel Chart",
                    Extensions = new string[] { ".xlc" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-excel" },
                    Categories = new string[] { "Document", "Microsoft", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Excel Macro File",
                    Extensions = new string[] { ".xlm" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-excel" },
                    Categories = new string[] { "Document", "Microsoft", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Excel Template File",
                    Extensions = new string[] { ".xlt" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-excel" },
                    Categories = new string[] { "Document", "Microsoft", "Office" },
                    Color = new HSVColor("#107C10")
                },
                new FileType
                {
                    Description = "Microsoft Excel Template File",
                    Extensions = new string[] { ".xltx" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.openxmlformats-officedocument.spreadsheetml.template" },
                    Categories = new string[] { "Microsoft", "Document", "Office" },
                    Color = new HSVColor("#107C10")
                },
                new FileType
                {
                    Description = "Excel Workspace File",
                    Extensions = new string[] { ".xlw" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-excel" },
                    Categories = new string[] { "Document", "Microsoft", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Xml File",
                    Extensions = new string[] { ".xml" },
                    MimeTypes = new string[] { "application/octet-stream", "text/xml", "application/x-xml", "application/xml" },
                    Categories = new string[] { "Markup", "Source Code" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Pixmap Image",
                    Extensions = new string[] { ".xpm" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-xpixmap", "image/x-xpm", "image/xpm" },
                    Categories = new string[] { "Media", "Picture" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Microsoft Xps File",
                    Extensions = new string[] { ".xps" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.ms-xpsdocument" },
                    Categories = new string[] { "Binary", "Document" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Xslt Stylesheet File",
                    Extensions = new string[] { ".xsl" },
                    MimeTypes = new string[] { "application/octet-stream", "text/xsl" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "X Windows Dump",
                    Extensions = new string[] { ".xwd" },
                    MimeTypes = new string[] { "application/octet-stream", "image/x-xwindowdump", "image/xwd", "image/x-xwd", "application/xwd", "application/x-xwd" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Unix Compressed Archive File",
                    Extensions = new string[] { ".z" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-compress", "application/z", "application/x-z" },
                    Categories = new string[] { "Archive", "Compressed", "Linux" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Compressed Zip Archive",
                    Extensions = new string[] { ".zip" },
                    MimeTypes = new string[] { "application/octet-stream", "application/zip", "application/x-compress", "application/x-compressed", "application/x-zip", "application/x-zip-compressed", "application/zip-compressed", "application/x-7zip-compressed" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#8B4513")
                },
                new FileType
                {
                    Description = "3Gp Mobile Video",
                    Extensions = new string[] { ".3gp" },
                    MimeTypes = new string[] { "application/octet-stream", "video/3gpp" },
                    Categories = new string[] { "Media", "Video" },
                    Color = new HSVColor("#4B0082")
                },
                new FileType
                {
                    Description = "Iphone Index",
                    Extensions = new string[] { ".m3u8" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-mpegurl" },
                    Categories = new string[] { "Apple", "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Iphone Segment",
                    Extensions = new string[] { ".ts" },
                    MimeTypes = new string[] { "application/octet-stream", "video/mp2t" },
                    Categories = new string[] { "Video", "Binary", "Apple" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Android Application Package",
                    Extensions = new string[] { ".apk" },
                    MimeTypes = new string[] { "application/octet-stream", "application/vnd.android.package-archive" },
                    Categories = new string[] { "Software", "Google", "Android" },
                    Color = new HSVColor("#75BC54")
                },
                new FileType
                {
                    Description = "Web Open Font Format",
                    Extensions = new string[] { ".ttf" },
                    MimeTypes = new string[] { "application/octet-stream", "application/font-woff" },
                    Categories = new string[] { "Web", "Font" },
                    Color = new HSVColor("#FF1493")
                },
                new FileType
                {
                    Description = "Scalable Vector Graphics",
                    Extensions = new string[] { ".svg" },
                    MimeTypes = new string[] { "application/octet-stream", "image/svg+xml" },
                    Categories = new string[] { "Design", "Media", "Picture", "Vector", "Web" },
                    Color = new HSVColor("#1E90FF")
                },
                new FileType
                {
                    Description = "Compressed Scalable Vector Graphics",
                    Extensions = new string[] { ".svgz" },
                    MimeTypes = new string[] { "application/octet-stream", "image/svg+xml" },
                    Categories = new string[] { "Archive", "Compressed" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Windows Phone Application",
                    Extensions = new string[] { ".xap" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-silverlight-app" },
                    Categories = new string[] { "Software", "Windows" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Markdown Text File",
                    Extensions = new string[] { ".md" },
                    MimeTypes = new string[] { "application/octet-stream", "text/markdown", "text/vnd.daringfireball.markdown", "text/x-markdown", "text/x-web-markdown" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#696969")
                },
                new FileType
                {
                    Description = "JavaScript Object Notation",
                    Extensions = new string[] { ".json" },
                    MimeTypes = new string[] { "application/octet-stream", "application/json", "text/json" },
                    Categories = new string[] { "Source Code" },
                    Color = new HSVColor("#228B22")
                },
                new FileType
                {
                    Description = "Textile Text File",
                    Extensions = new string[] { ".textile" },
                    MimeTypes = new string[] { "application/octet-stream", "text/x-web-textile", "text/x-wiki.creole" },
                    Categories = new string[] { "Document", "Office" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Batch File",
                    Extensions = new string[] { ".bat" },
                    MimeTypes = new string[] { "application/octet-stream", "application/bat", "application/x-bat" },
                    Categories = new string[] { "Binary" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Nintendo 64 ROM File",
                    Extensions = new string[] { ".n64", ".v64", ".z64" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-nintendo64-rom" },
                    Categories = new string[] { "Emulation", "Games" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Super Nintendo ROM File",
                    Extensions = new string[] { ".smc", ".sfc" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-snes-rom" },
                    Categories = new string[] { "Emulation", "Games" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "SEGA Genesis ROM File",
                    Extensions = new string[] { ".gen", ".smd" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-sega-genesis-rom" },
                    Categories = new string[] { "Emulation", "Games" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Nintendo GameCube ROM File",
                    Extensions = new string[] { ".gcm", ".gcz", ".ciso" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-gamecube-rom" },
                    Categories = new string[] { "Emulation", "Games", "Disk" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Nintendo 3DS ROM File",
                    Extensions = new string[] { ".3ds", ".cci", ".cxi", ".cia" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-nintendo-3ds-rom" },
                    Categories = new string[] { "Emulation", "Games" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "Nintendo DS ROM File",
                    Extensions = new string[] { ".nds" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-nintendo-ds-rom" },
                    Categories = new string[] { "Emulation", "Games" },
                    Color = new HSVColor("#808080")
                },
                new FileType
                {
                    Description = "ISO Disk Image",
                    Extensions = new string[] { ".iso" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-iso9660-image", "application/x-cd-image", "application/x-dvd-image" },
                    Categories = new string[] { "Disk", "Archive" },
                    Color = new HSVColor("#FFD700")
                },
                new FileType
                {
                    Description = "Compressed Wii/GameCube ROM",
                    Extensions = new string[] { ".rvz" },
                    MimeTypes = new string[] { "application/octet-stream", "application/x-rvz-rom", "application/x-dolphin-rom" },
                    Categories = new string[] { "Emulation", "Games", "Compressed" },
                    Color = new HSVColor("#4169E1")
                },
                new FileType
                {
                    Description = "Apple Lossless Audio File",
                    Extensions = new string[] { ".alac" },
                    MimeTypes = new string[] { "application/octet-stream", "audio/alac", "audio/x-alac" },
                    Categories = new string[] { "Audio", "Apple", "Lossless" },
                    Color = new HSVColor("#FF4500")
                }
            };

    }
}
