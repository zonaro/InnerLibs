using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace InnerLibs
{


    /// <summary>
    /// Classe para Extrair informaçoes de uma DATAURL
    /// </summary>
    public class DataURI
    {
        #region Public Constructors

        /// <summary>
        /// Cria um novo DATAURL a aprtir de uma string
        /// </summary>
        /// <param name="DataURI"></param>
        public DataURI(string DataURI)
        {
            try
            {
                var regex = new Regex(@"^data:(?<mimeType>(?<mime>\w+)\/(?<extension>\w+));(?<encoding>\w+),(?<data>.*)", RegexOptions.Compiled);
                var match = regex.Match(DataURI);
                Mime = match.Groups["mime"].Value.ToLowerInvariant();
                Extension = match.Groups["extension"].Value.ToLowerInvariant();
                Encoding = match.Groups["encoding"].Value.ToLowerInvariant();
                Data = match.Groups["data"].Value;
                if (new[] { Mime, Extension, Encoding, Data }.Any(x => x.IsBlank()))
                {
                    throw new Exception("Some parts are blank");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("DataURI not Valid", ex);
            }
        }

        #endregion Public Constructors



        #region Public Properties

        /// <summary>
        /// String Util ou Base32
        /// </summary>
        /// <returns></returns>
        public string Data { get; private set; }

        /// <summary>
        /// Tipo de encoding (32 ou 64)
        /// </summary>
        /// <returns></returns>
        public string Encoding { get; private set; }

        /// <summary>
        /// Extensão do tipo do arquivo
        /// </summary>
        /// <returns></returns>
        public string Extension { get; private set; }

        /// <summary>
        /// MIME type completo
        /// </summary>
        /// <returns></returns>
        public string FullMimeType => Mime + "/" + Extension;

        /// <summary>
        /// Tipo do arquivo encontrado
        /// </summary>
        /// <returns></returns>
        public string Mime { get; private set; }

        #endregion Public Properties



        #region Public Methods

        public static implicit operator byte[](DataURI d) => d?.ToBytes();

        public static implicit operator FileType(DataURI d) => d?.ToFileType();

        public static implicit operator string(DataURI d) => d?.ToString();

        /// <summary>
        /// Converte esta dataURI em Bytes()
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes() => ToString().Base64ToBytes();

        /// <summary>
        /// Informaçoes referentes ao tipo do arquivo
        /// </summary>
        /// <returns></returns>
        public FileType ToFileType() => new FileType(FullMimeType);

        /// <summary>
        /// Retorna uma string da dataURL
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"data:{FullMimeType};{Encoding},{Data}";

        /// <summary>
        /// Transforma este datauri em arquivo
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public FileInfo WriteToFile(string Path, DateTime? dateTime = null) => ToBytes().WriteToFile(Path, dateTime);

        #endregion Public Methods
    }
}