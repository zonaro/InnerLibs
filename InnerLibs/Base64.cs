using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InnerLibs
{
    /// <summary>
    /// Modulo para manipulação de imagens e Strings Base64
    /// </summary>
    /// <remarks></remarks>
    public static class Base64
    {
        /// <summary>
        /// Decoda uma string em Base64
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        public static string Atob(this string Base, Encoding Encoding = null)
        {
            if (Base.IsNotBlank())
            {
                return (Encoding ?? new UTF8Encoding(false)).GetString(Convert.FromBase64String(Base));
            }

            return null;
        }

        /// <summary>
        /// Converte uma DATAURL ou Base64 String em um array de Bytes
        /// </summary>
        /// <param name="Base64StringOrDataURL">Base64 String ou DataURL</param>
        /// <returns></returns>
        public static byte[] Base64ToBytes(this string Base64StringOrDataURL)
        {
            return Convert.FromBase64String(Base64StringOrDataURL.FixBase64());
        }

        public static Image Base64ToImage(this string DataUrlOrBase64String, int Width = 0, int Height = 0)
        {
            try
            {
                if (DataUrlOrBase64String.IsBlank())
                {
                    return null;
                }

                if (DataUrlOrBase64String.Contains(","))
                {
                    DataUrlOrBase64String = DataUrlOrBase64String.Split(",")[1];
                }

                var imageBytes = Convert.FromBase64String(DataUrlOrBase64String.FixBase64());
                var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                if (Width > 0 & Height > 0)
                {
                    return Image.FromStream(ms, true).Resize(Width, Height, false);
                }
                else
                {
                    return Image.FromStream(ms, true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Invalid Base64 or DataURL string or Base64 format is not an Image", ex);
            }
        }

        /// <summary>
        /// Encoda uma string em Base64
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        public static string Btoa(this string Text, Encoding Encoding = null)
        {
            if (Text.IsNotBlank())
            {
                return Convert.ToBase64String((Encoding ?? new UTF8Encoding(false)).GetBytes(Text));
            }

            return null;
        }

        /// <summary>
        /// Cria um arquivo fisico a partir de uma Base64 ou DataURL
        /// </summary>
        /// <param name="Base64StringOrDataURL"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static FileInfo CreateFileFromDataURL(this string Base64StringOrDataURL, string FilePath)
        {
            return Base64StringOrDataURL.Base64ToBytes().WriteToFile(FilePath);
        }

        /// <summary>
        /// Arruma os caracteres de uma string Base64
        /// </summary>
        /// <param name="Base64StringOrDataUrl">Base64String ou DataURL</param>
        /// <returns>Retorna apenas a Base64</returns>
        public static string FixBase64(this string Base64StringOrDataUrl)
        {
            string dummyData = Base64StringOrDataUrl.GetAfter(",").Trim().Replace(" ", "+");
            if (dummyData.Length % 4 > 0)
            {
                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
            }

            return dummyData;
        }

        /// <summary>
        /// Retorna TRUE se o texto for um dataurl valido
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool IsDataURL(this string Text)
        {
            try
            {
                return new DataURI(Text).ToString().IsNotBlank();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converte um Array de Bytes em uma string Base64
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <returns></returns>
        public static string ToBase64(this byte[] Bytes)
        {
            return Convert.ToBase64String(Bytes);
        }

        public static string ToBase64(this Image OriginalImage, System.Drawing.Imaging.ImageFormat OriginalImageFormat)
        {
            using (var ms = new MemoryStream())
            {
                OriginalImage.Save(ms, OriginalImageFormat);
                var imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public static string ToBase64(this Image OriginalImage)
        {
            using (var ms = new MemoryStream())
            {
                OriginalImage.Save(ms, OriginalImage.GetImageFormat());
                var imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        public static string ToBase64(this Uri ImageURL)
        {
            var imagem = Web.GetImage(ImageURL.AbsoluteUri);
            using (var m = new MemoryStream())
            {
                imagem.Save(m, imagem.RawFormat);
                var imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static string ToBase64(this string ImageURL, System.Drawing.Imaging.ImageFormat OriginalImageFormat)
        {
            var imagem = Image.FromStream(System.Net.WebRequest.Create(string.Format(ImageURL)).GetResponse().GetResponseStream());
            using (var m = new MemoryStream())
            {
                imagem.Save(m, OriginalImageFormat);
                var imageBytes = m.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        /// <summary>
        /// Converte um Array de Bytes em uma DATA URL Completa
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <param name="Type">Tipo de arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this byte[] Bytes, FileType Type = null)
        {
            return "data:" + (Type ?? new FileType()).ToString() + ";base64," + Bytes.ToBase64();
        }

        /// <summary>
        /// Converte um Array de Bytes em uma DATA URL Completa
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <param name="MimeType">Tipo de arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this byte[] Bytes, string MimeType)
        {
            return "data:" + MimeType + ";base64," + Bytes.ToBase64();
        }

        /// <summary>
        /// Converte um arquivo uma DATA URL Completa
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this FileInfo File)
        {
            return File.ToBytes().ToDataURL(new FileType(File.Extension));
        }

        /// <summary>
        /// Transforma uma imagem em uma URL Base64
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>Uma DataURI em string</returns>
        public static string ToDataURL(this Image Image)
        {
            return "data:" + Image.GetFileType().First().ToLower().Replace("application/octet-stream", FileTypeExtensions.GetFileType(".png").First()) + ";base64," + Image.ToBase64();
        }

        /// <summary>
        /// Converte uma Imagem para String Base64
        /// </summary>
        /// <param name="OriginalImage">
        /// Imagem original, tipo Image() (Picturebox.Image, Picturebox.BackgroundImage etc.)
        /// </param>
        /// <param name="OriginalImageFormat">
        /// Formato da imagem de acordo com sua extensão (JPG, PNG, GIF etc.)
        /// </param>
        /// <returns>Uma string em formato Base64</returns>
        /// <summary>
        /// Converte uma imagem para DataURI trocando o MIME Type
        /// </summary>
        /// <param name="OriginalImage">Imagem</param>
        /// <param name="OriginalImageFormat">Formato da Imagem</param>
        /// <returns>Uma data URI com a imagem convertida</returns>
        public static string ToDataURL(this Image OriginalImage, System.Drawing.Imaging.ImageFormat OriginalImageFormat)
        {
            return OriginalImage.ToBase64(OriginalImageFormat).Base64ToImage().ToDataURL();
        }

        /// <summary>
        /// Converte uma Imagem para String Base64
        /// </summary>
        /// <param name="OriginalImage">
        /// Imagem original, tipo Image() (Picturebox.Image, Picturebox.BackgroundImage etc.)
        /// </param>
        /// <returns>Uma string em formato Base64</returns>
        /// <summary>
        /// Converte uma Imagem da WEB para String Base64
        /// </summary>
        /// <param name="ImageURL">Caminho da imagem</param>
        /// <returns>Uma string em formato Base64</returns>
        /// <summary>
        /// Converte uma Imagem da WEB para String Base64
        /// </summary>
        /// <param name="ImageURL">Caminho da imagem</param>
        /// <param name="OriginalImageFormat">
        /// Formato da imagem de acordo com sua extensão (JPG, PNG, GIF etc.)
        /// </param>
        /// <returns>Uma string em formato Base64</returns>
        /// <summary>
        /// Converte uma String DataURL ou Base64 para Imagem
        /// </summary>
        /// <param name="DataUrlOrBase64String">A string Base64 a ser convertida</param>
        /// <param name="Width">
        /// Altura da nova imagem (não preencher retorna o tamanho original da imagem)
        /// </param>
        /// <param name="Height">
        /// Largura da nova imagem (não preencher retorna o tamanho original da imagem)
        /// </param>
        /// <returns>Uma imagem (componente Image)</returns>
        /// <summary>
        /// Converte um array de bytes para imagem
        /// </summary>
        /// <param name="Bytes">Bytes</param>
        /// <returns></returns>
        public static Image ToImage(this byte[] Bytes)
        {
            using (var s = new MemoryStream(Bytes))
            {
                return Image.FromStream(s);
            }
        }
    }

    /// <summary>
    /// Classe para Extrair informaçoes de uma DATAURL
    /// </summary>
    public class DataURI
    {
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
                Mime = match.Groups["mime"].Value.ToLower();
                Extension = match.Groups["extension"].Value.ToLower();
                Encoding = match.Groups["encoding"].Value.ToLower();
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

        /// <summary>
        /// String Base64 ou Base32
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
        public override string ToString() => "data:" + FullMimeType + ";" + Encoding + "," + Data;

        /// <summary>
        /// Transforma este datauri em arquivo
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public FileInfo WriteToFile(string Path, DateTime? dateTime = null) => ToBytes().WriteToFile(Path, dateTime);

        public static implicit operator byte[](DataURI d) => d.ToBytes();
        public static implicit operator string(DataURI d) => d.ToString();
        public static implicit operator FileType(DataURI d) => d.ToFileType();

    }
}