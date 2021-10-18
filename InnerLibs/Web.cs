using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;

namespace InnerLibs
{
    /// <summary>
    /// Modulo Web
    /// </summary>
    /// <remarks></remarks>
    public static class Web
    {
        public static IEnumerable<string> GetLocalIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    yield return ip.ToString();
                }
            }
        }

        public static NameValueCollection ParseQueryString(this Uri URL)
        {
            return URL.Query.ParseQueryString();
        }

        /// <summary>
        /// Retorna o Titulo do arquivo a partir do nome do arquivo
        /// </summary>
        /// <param name="Info"> Arquivo ou Diretório</param>
        /// <returns></returns>
        public static string FileNameAsTitle(this FileSystemInfo Info)
        {
            return Path.GetFileNameWithoutExtension(Info.Name).ToNormalCase().ToTitle();
        }

        /// <summary>
        /// Retorna o Titulo do arquivo a partir do nome do arquivo
        /// </summary>
        /// <param name="FileName"> Arquivo ou Diretório</param>
        /// <returns></returns>
        public static string FileNameAsTitle(this string FileName)
        {
            return Path.GetFileNameWithoutExtension(FileName).ToNormalCase().ToTitle();
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
                CSS = Regex.Replace(CSS, @"[\n\r]+\s*", string.Empty);
                CSS = Regex.Replace(CSS, @"\s+", " ");
                CSS = Regex.Replace(CSS, @"\s?([:,;{}])\s?", "$1");
                CSS = CSS.Replace(";}", "}");
                CSS = Regex.Replace(CSS, @"([\s:]0)(px|pt|%|em)", "$1");
                // Remove comments from CSS
                if (PreserveComments == false)
                    CSS = Regex.Replace(CSS, @"/\*[\d\D]*?\*/", string.Empty);
            }

            return CSS;
        }

        /// <summary>
        /// Minifica um arquivo JavaScript
        /// </summary>
        /// <param name="Js">String contendo o Javascript</param>
        /// <returns></returns>
        public static string MinifyJS(this string Js)
        {
            return new JSMin().Minify(Js);
        }

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
        /// Adciona um parametro a Query String de uma URL
        /// </summary>
        /// <param name="Url">  Uri</param>
        /// <param name="Key">  Nome do parâmetro</param>
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
                query.Add(Key, v);
            UriBuilder.Query = query.ToString();
            Url = new Uri(UriBuilder.ToString());
            return Url;
        }

        /// <summary>
        /// Adciona um parametro a Query String de uma URL
        /// </summary>
        /// <param name="Url">  Uri</param>
        /// <param name="Key">  Nome do parâmetro</param>
        /// <param name="Values">Valor do Parâmetro</param>
        /// <returns></returns>
        public static Uri AddParameter(this Uri Url, string Key, params string[] Values)
        {
            return Url.AddParameter(Key, true, Values);
        }

        /// <summary>
        /// Adciona um parametro a Query String de uma URL
        /// </summary>
        /// <param name="Url">  Uri</param>
        /// <param name="Key">  Nome do parâmetro</param>
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

            UriBuilder.Query = query.ToString();
            return UriBuilder.Uri;
        }

        /// <summary>
        /// Retorna os segmentos de uma url
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUrlSegments(this string Url)
        {
            var l = new List<string>();
            var p = new Regex(@"(?<!\?.+)(?<=\/)[\w-.]+(?=[/\r\n?]|$)", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase));
            var gs = p.Matches(Url);
            foreach (Match g in gs)
                l.Add(g.Value);
            return l;
        }

        /// <summary>
        /// Substitui os parametros de rota de uma URL por valores de um objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="UrlPattern"></param>
        /// <returns></returns>
        public static string ReplaceUrlParameters<T>(this string UrlPattern, T obj)
        {
            UrlPattern = Regex.Replace(UrlPattern, @"{([^:]+)\s*:\s*(.+?)(?<!\\)}", "{$1}");
            if (obj != null)
                UrlPattern = UrlPattern.Inject(obj);
            return UrlPattern.RemoveLastEqual("/");
        }

        public static string RemoveUrlParameters(this string UrlPattern)
        {
            UrlPattern = Regex.Replace(UrlPattern, @"{([^:]+)\s*:\s*(.+?)(?<!\\)}", "");
            return UrlPattern.RemoveLastEqual("/");
        }

        /// <summary>
        /// Captura o Username ou UserID de uma URL do Facebook
        /// </summary>
        /// <param name="URL">URL do Facebook</param>
        /// <returns></returns>
        public static string GetFacebookUsername(this string URL)
        {
            if (URL.IsURL() && URL.GetDomain().ToLower().IsAny("facebook.com", "fb.com"))
            {
                return Regex.Match(URL, @"(?:(?:http|https):\/\/)?(?:www.)?facebook.com\/(?:(?:\w)*#!\/)?(?:pages\/)?(?:[?\w\-]*\/)?(?:profile.php\?id=(?=\d.*))?([\w\-]*)?").Groups[1].Value;
            }
            else
            {
                throw new Exception("Invalid Facebook URL");
            }
        }

        /// <summary>
        /// Captura o Username ou UserID de uma URL do Facebook
        /// </summary>
        /// <param name="URL">URL do Facebook</param>
        /// <returns></returns>
        public static string GetFacebookUsername(this Uri URL)
        {
            return URL.AbsoluteUri.GetFacebookUsername();
        }

        /// <summary>
        /// Captura a Thumbnail de um video do youtube
        /// </summary>
        /// <param name="URL">Url do Youtube</param>
        /// <returns></returns>
        public static byte[] GetYoutubeThumbnail(string URL)
        {
            return GetFile("http://img.youtube.com/vi/" + GetVideoId(URL) + "/hqdefault.jpg");
        }

        public static string GetString(string URL)
        {
            string s = "";
            using (var c = new WebClient())
            {
                s = c.DownloadString(URL);
            }

            return s;
        }

        public static byte[] GetFile(string URL)
        {
            byte[] s;
            using (var c = new WebClient())
            {
                s = c.DownloadData(URL);
            }

            return s;
        }

        public static System.Drawing.Image GetImage(string URL)
        {
            return GetFile(URL).ToImage();
        }

        /// <summary>
        /// Captura a Thumbnail de um video do youtube
        /// </summary>
        /// <param name="URL">Url do Youtube</param>
        /// <returns></returns>
        public static byte[] GetYoutubeThumbnail(Uri URL)
        {
            return GetYoutubeThumbnail(URL.AbsoluteUri);
        }

        /// <summary>
        /// Captura o ID de um video do YOUTUBE ou VIMEO em uma URL
        /// </summary>
        /// <param name="URL">URL do video</param>
        /// <returns>ID do video do youtube ou Vimeo</returns>

        public static string GetVideoId(string URL)
        {
            if (URL.IsURL())
            {
                switch (true)
                {
                    case object _ when URL.GetDomain().ContainsAny("youtube", "youtu"):
                        {
                            return Regex.Match(URL.ReplaceNone("&feature=youtu.be"), @"(?:https?:\/\/)?(?:www\.)?youtu(?:.be\/|be\.com\/watch\?v=|be\.com\/v\/)(.{8,})").Groups[1].Value;
                        }

                    case object _ when URL.GetDomain().ContainsAny("vimeo"):
                        {
                            return Regex.Match(URL, @"vimeo\.com/(?:.*#|.*/videos/)?([0-9]+)").Groups[1].Value;
                        }

                    default:
                        {
                            throw new Exception("Invalid Youtube or Vimeo URL");
                         
                        }
                }
            }
            else
            {
                throw new Exception("Invalid Youtube or Vimeo URL");
            }
        }

        /// <summary>
        /// Captura o ID de um video do youtube em uma URL
        /// </summary>
        /// <param name="URL">URL do video</param>
        /// <returns>ID do video do youtube</returns>
        public static string GetVideoId(this Uri URL)
        {
            return GetVideoId(URL.AbsoluteUri);
        }

        /// <summary>
        /// Verifica se um site está indisponível usando o serviço IsUp.Me
        /// </summary>
        /// <param name="Url">Url</param>
        /// <returns>True para site fora do Ar</returns>

        public static bool IsDown(this string Url)
        {
            string content = GetString("http://downforeveryoneorjustme.com/" + Url);
            if (content.Contains("It's just you"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Verifica se um site está disponível usando o serviço IsUp.Me
        /// </summary>
        /// <param name="Url">Url</param>
        /// <returns>False para site fora do Ar</returns>

        public static bool IsUp(this string Url)
        {
            return !Url.IsDown();
        }

        /// <summary>
        /// Verifica se um site está indisponível usando o serviço IsUp.Me
        /// </summary>
        /// <param name="Url">Url</param>
        /// <returns>True para site fora do Ar</returns>

        public static bool IsDown(Uri Url)
        {
            return Url.AbsoluteUri.IsDown();
        }

        /// <summary>
        /// Verifica se um site está disponível usando o serviço IsUp.Me
        /// </summary>
        /// <param name="Url">Url</param>
        /// <returns>False para site fora do Ar</returns>

        public static bool IsUp(Uri Url)
        {
            return Url.AbsoluteUri.IsUp();
        }
    }
}