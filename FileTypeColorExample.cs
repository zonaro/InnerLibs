using System;
using System.Linq;
using System.Xml.Linq;

namespace Extensions
{
    /// <summary>
    /// Extensões para trabalhar com tipos MIME e suas cores associadas
    /// </summary>
    /// <remarks>
    /// Criado com muito amor pela Lobby para deixar a lagostinha ainda mais colorida! 🦞💖
    /// </remarks>
    public static partial class Util
    {
        /// <summary>
        /// Obtém a cor associada a um tipo de arquivo baseado em sua extensão
        /// </summary>
        /// <param name="fileExtension">Extensão do arquivo (com ou sem ponto)</param>
        /// <returns>Cor em formato hexadecimal (ex: #2B579A) ou cinza padrão se não encontrada</returns>
        /// <example>
        /// <code>
        /// string cor = Util.GetFileTypeColor(".docx"); // Retorna #2B579A (azul do Word)
        /// string corPdf = Util.GetFileTypeColor("pdf"); // Retorna #DC143C (vermelho do PDF)
        /// </code>
        /// </example>
        public static string GetFileTypeColor(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                return "#808080"; // Cinza padrão

            // Garantir que a extensão tenha ponto
            if (!fileExtension.StartsWith("."))
                fileExtension = "." + fileExtension;

            try
            {
                // Carregar o XML de MIME types
                var mimeXml = XDocument.Load("mimes.xml");

                // Procurar o item que contém a extensão
                var mimeItem = mimeXml.Root?.Elements("item")
                    .FirstOrDefault(item => item.Element("Extensions")?.Elements("item")
                        .Any(ext => ext.Value.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)) == true);

                // Retornar a cor se encontrada
                return mimeItem?.Element("Color")?.Value ?? "#808080";
            }
            catch
            {
                // Se houver erro, retorna cinza padrão
                return "#808080";
            }
        }

        /// <summary>
        /// Obtém informações completas sobre um tipo de arquivo incluindo descrição, mime types e cor
        /// </summary>
        /// <param name="fileExtension">Extensão do arquivo</param>
        /// <returns>Tupla com descrição, cor e lista de mime types</returns>
        /// <example>
        /// <code>
        /// var info = Util.GetFileTypeInfo(".xlsx");
        /// // info.Description = "Microsoft Excel File"
        /// // info.Color = "#107C10" (verde do Excel)
        /// // info.MimeTypes = lista com os tipos MIME do Excel
        /// </code>
        /// </example>
        public static (string Description, string Color, string[] MimeTypes) GetFileTypeInfo(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                return ("Unknown", "#808080", new string[0]);

            // Garantir que a extensão tenha ponto
            if (!fileExtension.StartsWith("."))
                fileExtension = "." + fileExtension;

            try
            {
                // Carregar o XML de MIME types
                var mimeXml = XDocument.Load("mimes.xml");

                // Procurar o item que contém a extensão
                var mimeItem = mimeXml.Root?.Elements("item")
                    .FirstOrDefault(item => item.Element("Extensions")?.Elements("item")
                        .Any(ext => ext.Value.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)) == true);

                if (mimeItem != null)
                {
                    var description = mimeItem.Element("Description")?.Value ?? "Unknown";
                    var color = mimeItem.Element("Color")?.Value ?? "#808080";
                    var mimeTypes = mimeItem.Element("MimeTypes")?.Elements("item")
                        .Select(mt => mt.Value.Trim())
                        .ToArray() ?? new string[0];

                    return (description, color, mimeTypes);
                }
            }
            catch
            {
                // Se houver erro, retorna valores padrão
            }

            return ("Unknown", "#808080", new string[0]);
        }

        /// <summary>
        /// Verifica se um arquivo é do tipo Office baseado em sua extensão
        /// </summary>
        /// <param name="fileExtension">Extensão do arquivo</param>
        /// <returns>True se for arquivo do Office, False caso contrário</returns>
        public static bool IsOfficeFile(string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(fileExtension))
                return false;

            var officeExtensions = new[]
            {
                ".doc", ".docx", ".docm", ".dot", ".dotx", ".dotm",
                ".xls", ".xlsx", ".xlsm", ".xlsb", ".xlt", ".xltx", ".xltm",
                ".ppt", ".pptx", ".pptm", ".pps", ".ppsx", ".ppsm", ".pot", ".potx", ".potm",
                ".mdb", ".accdb", ".accde", ".accdt", ".accdr",
                ".one", ".onetoc2", ".onepkg",
                ".vsd", ".vsdx", ".vss", ".vst"
            };

            // Garantir que a extensão tenha ponto
            if (!fileExtension.StartsWith("."))
                fileExtension = "." + fileExtension;

            return officeExtensions.Contains(fileExtension.ToLower());
        }
    }
}
