using InnerLibs.Console;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace InnerLibs
{
    /// <summary>
    /// Modulo WebExtensions
    /// </summary>
    /// <remarks></remarks>
    public static partial class Util
    {
        #region Public Methods

        /// <summary>
        /// Adciona um parametro a Query String de uma URL
        /// </summary>
        /// <param name="Url">Uri</param>
        /// <param name="Key">Nome do parâmetro</param>
        /// <param name="Values">Valor do Parâmetro</param>
        /// <returns></returns>
        public static Uri AddParameter(this Uri Url, string Key, bool Append, params string[] Values)
        {
            var UriBuilder = new UriBuilder(Url);
            var query = UriBuilder.Query.ParseQueryString();
            if (Values is null || Append == false)
            {
                if (query.AllKeys.Contains(Key))
                {
                    query.Remove(Key);
                }
            }

            foreach (var v in Values ?? Array.Empty<string>())
            {
                query.Add(Key, v);
            }

            UriBuilder.Query = query.ToString();
            Url = new Uri(UriBuilder.ToString());
            return Url;
        }

        /// <summary>
        /// Adciona um parametro a Query String de uma URL
        /// </summary>
        /// <param name="Url">Uri</param>
        /// <param name="Key">Nome do parâmetro</param>
        /// <param name="Values">Valor do Parâmetro</param>
        /// <returns></returns>
        public static Uri AddParameter(this Uri Url, string Key, params string[] Values) => Url.AddParameter(Key, true, Values);

        public static byte[] DownloadFile(string URL, NameValueCollection Headers = null, Encoding Encoding = null)
        {
            byte[] s = null;
            using (var c = new WebClient())
            {
                c.Encoding = Encoding ?? new UTF8Encoding(false);

                if (Headers != null)
                    c.Headers.Add(Headers);
                if (URL.IsURL()) s = c.DownloadData(URL);
            }

            return s;
        }

        public static byte[] DownloadFile(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadFile($"{URL}", Headers, Encoding);

        public static System.Drawing.Image DownloadImage(string URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadFile(URL, Headers, Encoding).ToImage();

        public static System.Drawing.Image DownloadImage(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadImage($"{URL}", Headers, Encoding);

        public static T DownloadJson<T>(string URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadString(URL, Headers, Encoding).FromJson<T>();

        public static object DownloadJson(string URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadString(URL, Headers, Encoding).FromJson();

        public static T DownloadJson<T>(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadJson<T>($"{URL}", Headers, Encoding);

        public static object DownloadJson(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadJson($"{URL}", Headers, Encoding);

        public static string DownloadString(string URL, NameValueCollection Headers = null, Encoding Encoding = null)
        {
            string s = Util.EmptyString;
            using (var c = new WebClient())
            {
                c.Encoding = Encoding ?? new UTF8Encoding(false);

                if (Headers != null)
                    c.Headers.Add(Headers);

                if (URL.IsURL()) s = $"{c.DownloadString(URL)}";
            }

            return s;
        }

        public static string DownloadString(this Uri URL, NameValueCollection Headers = null, Encoding Encoding = null) => DownloadString($"{URL}", Headers, Encoding);

        /// <summary>
        /// Retorna o Titulo do arquivo a partir do nome do arquivo
        /// </summary>
        /// <param name="Info">Arquivo ou Diretório</param>
        /// <returns></returns>
        public static string FileNameAsTitle(this FileSystemInfo Info, bool ForceCase = false) => Path.GetFileNameWithoutExtension(Info?.Name).ToNormalCase().ToTitle(ForceCase);

        /// <summary>
        /// Retorna o Titulo do arquivo a partir do nome do arquivo
        /// </summary>
        /// <param name="FilePath">Arquivo ou Diretório</param>
        /// <returns></returns>
        public static string FileNameAsTitle(this string FilePath, bool ForceCase = false) => Path.GetFileNameWithoutExtension(FilePath).ToNormalCase().ToTitle(ForceCase);

        /// <summary>
        /// Captura o Username ou UserID de uma URL do Facebook
        /// </summary>
        /// <param name="URL">URL do Facebook</param>
        /// <returns></returns>
        public static string GetFacebookUsername(this string URL) => URL.IsURL() && URL.GetDomain().ToLowerInvariant().IsAny("facebook.com", "fb.com")
               ? Regex.Match(URL.Replace("fb.com", "facebook.com"), @"(?:(?:http|https):\/\/)?(?:www.)?facebook.com\/(?:(?:\w)*#!\/)?(?:pages\/)?(?:[?\w\-]*\/)?(?:profile.php\?id=(?=\d.*))?([\w\-]*)?").Groups[1].Value
                 : throw new ArgumentException("Invalid Facebook URL", nameof(URL));

        /// <summary>
        /// Captura o Username ou UserID de uma URL do Facebook
        /// </summary>
        /// <param name="URL">URL do Facebook</param>
        /// <returns></returns>
        public static string GetFacebookUsername(this Uri URL) => URL?.AbsoluteUri.GetFacebookUsername();

        public static string GetFileNameWithoutExtension(this FileInfo Info) => Info != null ? Path.GetFileNameWithoutExtension(Info.Name) : Util.EmptyString;

        /// <summary>
        /// Retorna um novo <see cref="FileInfo"/> substituindo a extensão original por <paramref name="Extension"/>
        /// </summary>
        /// <param name="Info"></param>
        /// <param name="Extension"></param>
        /// <returns></returns>
        public static FileInfo ReplaceExtension(this FileInfo Info, string Extension)
        {
            if (Info != null)
                return new FileInfo(Path.Combine(Info.DirectoryName, Info.GetFileNameWithoutExtension() + "." + Extension.IfBlank("bin").TrimStart('.')).FixPath());
            return null;
        }

        public static IEnumerable<string> GetIPs() => GetLocalIP().Union(new[] { GetPublicIP() });

        public static IEnumerable<string> GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    yield return ip.ToString().Trim();
                }
            }
        }

        public static string GetPublicIP()
        {
            var IP = InnerLibs.Util.EmptyString;
            Util.TryExecute(() => IP = DownloadString("https://ipv4.icanhazip.com/")).ConsoleWriteError();
            IP = IP.Trim().NullIf(x => !x.IsIP());
            return IP.Trim();
        }

        /// <summary>
        /// Retorna os segmentos de uma url
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetURLSegments(this string URL)
        {
            var l = new List<string>();
            var p = new Regex(@"(?<!\?.+)(?<=\/)[\w-.]+(?=[/\r\n?]|$)", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase));
            var gs = p.Matches(URL);
            foreach (Match g in gs)
            {
                l.Add(g.Value);
            }

            return l;
        }

        /// <summary>
        /// Captura o ID de um video do YOUTUBE ou VIMEO em uma URL
        /// </summary>
        /// <param name="URL">URL do video</param>
        /// <returns>ID do video do youtube ou Vimeo</returns>
        public static string GetVideoID(this Uri URL) => GetVideoID(URL.AbsoluteUri);

        /// <summary>
        /// Captura o ID de um video do youtube em uma URL
        /// </summary>
        /// <param name="URL">URL do video</param>
        /// <returns>ID do video do youtube</returns>
        public static string GetVideoID(this string URL)
        {
            if (URL.IsURL())
            {
                if (URL.GetDomain().ContainsAny("youtube", "youtu"))
                {
                    return Regex.Match(URL.ReplaceNone("&feature=youtu.be"), @"(?:https?:\/\/)?(?:www\.)?youtu(?:.be\/|be\.com\/watch\?v=|be\.com\/v\/)(.{8,})").Groups[1].Value;
                }
                else if (URL.GetDomain().ContainsAny("vimeo"))
                {
                    return Regex.Match(URL, @"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)").Groups[1].Value;
                }
            }

            throw new ArgumentException("Invalid Youtube or Vimeo URL", nameof(URL));
        }

        /// <summary>
        /// Captura a Thumbnail de um video do youtube
        /// </summary>
        /// <param name="URL">Url do Youtube</param>
        /// <returns></returns>
        public static byte[] GetYoutubeThumbnail(string URL) => DownloadFile($"http://img.youtube.com/vi/{GetVideoID(URL)}/hqdefault.jpg");

        /// <summary>
        /// Captura a Thumbnail de um video do youtube
        /// </summary>
        /// <param name="URL">Url do Youtube</param>
        /// <returns></returns>
        public static byte[] GetYoutubeThumbnail(this Uri URL) => GetYoutubeThumbnail(URL?.AbsoluteUri);

        /// <summary>
        /// Verifica se o computador está conectado com a internet
        /// </summary>
        /// <returns></returns>
        public static bool IsConnected(string Test = "http://google.com")
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(Test))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Minifica uma folha de estilo CSS
        /// </summary>
        /// <param name="CSS">String contendo o CSS</param>
        /// <returns></returns>
        public static string MinifyCSS(this string CSS, bool PreserveComments = false)
        {
            if (CSS.IsNotBlank())
            {
                CSS = Regex.Replace(CSS, "[a-zA-Z]+#", "#");
                CSS = Regex.Replace(CSS, @"[\n\r]+\s*", InnerLibs.Util.EmptyString);
                CSS = Regex.Replace(CSS, @"\s+", " ");
                CSS = Regex.Replace(CSS, @"\s?([:,;{}])\s?", "$1");
                CSS = CSS.Replace(";}", "}");
                CSS = Regex.Replace(CSS, @"([\s:]0)(px|pt|%|em)", "$1");
                // Remove comments from CSS
                if (PreserveComments == false)
                {
                    CSS = Regex.Replace(CSS, @"/\*[\d\D]*?\*/", InnerLibs.Util.EmptyString);
                }
            }

            return CSS;
        }

        public static NameValueCollection ParseQueryString(this Uri URL) => URL?.Query.ParseQueryString();

        /// <summary>
        /// Remove um parametro da Query String de uma URL
        /// </summary>
        /// <param name="Url">Uri</param>
        /// <param name="Key">Nome do parâmetro</param>
        /// <param name="Values">Valor do Parâmetro</param>
        /// <returns></returns>
        public static Uri RemoveParameter(this Uri Url, params string[] Keys)
        {
            var UriBuilder = new UriBuilder(Url);
            var query = UriBuilder.Query.ParseQueryString();
            Keys = Keys != null && Keys.Any() ? Keys : query.AllKeys;
            foreach (var k in Keys)
            {
                try
                {
                    query.Remove(k);
                }
                catch
                {
                }
            }

            UriBuilder.Query = query.ToQueryString();
            return UriBuilder.Uri;
        }

        public static string RemoveUrlParameters(this string URL)
        {
            if ((URL.IsURL()))
            {
                URL = Regex.Replace(URL, @"{([^:]+)\s*:\s*(.+?)(?<!\\)}", InnerLibs.Util.EmptyString);
                URL = URL.TrimLastEqual("/");
            }
            return URL;
        }

        public static string RemoveUrlParameters(Uri URL) => RemoveUrlParameters(URL?.ToString());

        /// <summary>
        /// Substitui os parametros de rota de uma URL por valores de um objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string ReplaceUrlParameters<T>(this string URL, T obj)
        {
            if (URL.IsURL())
            {
                URL = Regex.Replace(URL, @"{([^:]+)\s*:\s*(.+?)(?<!\\)}", "{$1}");
                if (obj != null)
                {
                    URL = URL.Inject(obj);
                }

                URL = URL.TrimLastEqual("/");
            }
            return URL;
        }

        public static string ReplaceUrlParameters<T>(Uri URL, T obj) => ReplaceUrlParameters(URL?.ToString(), obj);

        #endregion Public Methods
    }
}