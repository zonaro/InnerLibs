using InnerLibs;
using InnerLibs.Console;
using InnerLibs.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InnerLibs
{
    public static class Ext
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
            string s = Ext.EmptyString;
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

        public static string GetFileNameWithoutExtension(this FileInfo Info) => Info != null ? Path.GetFileNameWithoutExtension(Info.Name) : Ext.EmptyString;

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
            var IP = InnerLibs.Ext.EmptyString;
            Ext.TryExecute(() => IP = DownloadString("https://ipv4.icanhazip.com/")).ConsoleWriteError();
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
                CSS = Regex.Replace(CSS, @"[\n\r]+\s*", InnerLibs.Ext.EmptyString);
                CSS = Regex.Replace(CSS, @"\s+", " ");
                CSS = Regex.Replace(CSS, @"\s?([:,;{}])\s?", "$1");
                CSS = CSS.Replace(";}", "}");
                CSS = Regex.Replace(CSS, @"([\s:]0)(px|pt|%|em)", "$1");
                // Remove comments from CSS
                if (PreserveComments == false)
                {
                    CSS = Regex.Replace(CSS, @"/\*[\d\D]*?\*/", InnerLibs.Ext.EmptyString);
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
                URL = Regex.Replace(URL, @"{([^:]+)\s*:\s*(.+?)(?<!\\)}", InnerLibs.Ext.EmptyString);
                URL = URL.TrimLastEqual("/");
            }
            return URL;
        }

        public static string RemoveUrlParameters(Uri URL) => RemoveUrlParameters(URL?.ToString());

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



        #region Public Methods

        /// <summary>
        /// Calcula a distancia entre 2 locais
        /// </summary>
        /// <param name="FirstLocation">Primeiro Local</param>
        /// <param name="SecondLocation">Segundo Local</param>
        /// <returns>A distancia em kilometros</returns>
        public static double CalculateDistance(this AddressInfo FirstLocation, AddressInfo SecondLocation)
        {
            double distance = 0.0d;
            if (FirstLocation?.Latitude != null && FirstLocation?.Longitude != null && SecondLocation?.Latitude != null && SecondLocation?.Longitude != null && (FirstLocation.Latitude != SecondLocation.Latitude || FirstLocation.Longitude != SecondLocation.Longitude))
            {
                // Calculate radians
                double latitude1Rad = FirstLocation.Latitude?.ToDouble().ToRadians() ?? 0;
                double longitude1Rad = FirstLocation.Longitude?.ToDouble().ToRadians() ?? 0;
                double latitude2Rad = SecondLocation.Latitude?.ToDouble().ToRadians() ?? 0;
                double longitude2Rad = SecondLocation.Longitude?.ToDouble().ToRadians() ?? 0;
                double longitudeDiff = Math.Abs(longitude1Rad - longitude2Rad);
                if (longitudeDiff > Math.PI)
                {
                    longitudeDiff = 2.0d * Math.PI - longitudeDiff;
                }

                double angleCalculation = Math.Acos(Math.Sin(latitude2Rad) * Math.Sin(latitude1Rad) + Math.Cos(latitude2Rad) * Math.Cos(latitude1Rad) * Math.Cos(longitudeDiff));
                distance = InnerLibs.Ext.EarthCircumference * angleCalculation / (2.0d * Math.PI);
            }
            return distance;
        }

        public static double[,] GetDistanceMatrix(this IEnumerable<AddressInfo> locations) => GetDistanceMatrix(locations?.ToArray() ?? Array.Empty<AddressInfo>());

        public static double[,] GetDistanceMatrix(params AddressInfo[] locations)
        {
            // Util the distance matrix
            double[,] distanceMatrix = new double[locations.Length, locations.Length];
            for (int i = 0; i < locations.Length; i++)
            {
                for (int j = 0; j < locations.Length; j++)
                {
                    // Set the distance in the distance matrix
                    distanceMatrix[i, j] = CalculateDistance(locations[i], locations[j]);
                }
            }
            return distanceMatrix;
        }

        /// <summary>
        /// Return a <see cref="IEnumerable{AddressInfo}"/> sorted according to the distance between
        /// locations (Traveler Salesman Problem)
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <remarks>
        /// The first item in <paramref name="address"/> will be used as the start of travel and end
        /// of travel (if <paramref name="returnToStart"/> is true)
        /// </remarks>
        public static IEnumerable<AddressInfo> OrderByNearestNeighbor(this IEnumerable<AddressInfo> address, bool returnToStart = true) => OrderByNearestNeighbor(address, null, returnToStart);

        public static IEnumerable<AddressInfo> OrderByNearestNeighbor(this IEnumerable<AddressInfo> address, double[,] distanceMatrix, bool returnToStart = true)
        {
            var locations = address.ToArray();
            distanceMatrix = distanceMatrix ?? address.GetDistanceMatrix();
            List<int> tour = new List<int>();
            if (distanceMatrix.Length > 0)
            {
                int numLocations = distanceMatrix.GetLength(0);

                // Start the tour at the first location
                tour.Add(0);

                int currentLocation = 0;
                while (tour.Count < numLocations)
                {
                    int nextLocation = -1;
                    double minDistance = double.MaxValue;

                    // Find the nearest unvisited location
                    for (int i = 0; i < numLocations; i++)
                    {
                        if (i == currentLocation || tour.Contains(i))
                        {
                            continue;
                        }

                        double distance = distanceMatrix[currentLocation, i];
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            nextLocation = i;
                        }
                    }

                    tour.Add(nextLocation);
                    currentLocation = nextLocation;
                }

                // Return to the starting location
                if (returnToStart) tour.Add(0);

                locations = tour.Select(x => locations[x]).ToArray();
            }

            return locations;
        }

        #endregion Public Methods

        #region Public Methods



        #region Public Methods

        #region Public Fields

        /// <summary>
        /// Earth's circumference at the equator in km, considering the earth is a globe, not flat
        /// </summary>
        public const double EarthCircumference = 40075d;

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Retorna uma progressão Aritmética com N numeros
        /// </summary>
        /// <param name="FirstNumber"></param>
        /// <param name="[Constant]"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static IEnumerable<int> ArithmeticProgression(this int FirstNumber, int Constant, int Length)
        {
            Length--;
            yield return FirstNumber;
            do
            {
                FirstNumber += Constant;
                yield return FirstNumber;
                Length--;
            } while (Length > 0);
        }

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static decimal Average(params decimal[] Values) => Values.Average();

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static double Average(params double[] Values) => Values.Average();

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static double Average(params int[] Values) => Values.Average();

        /// <summary>
        /// Tira a média de todos os números de um Array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo</returns>
        public static double Average(params long[] Values) => Values.Average();

        /// <summary>
        /// Retorna uma sequencia de bytes de N entradas
        /// </summary>
        /// <param name="Length">Quantidade de numeros da sequencia</param>
        /// <returns>Lista com a sequencia de bytes</returns>
        public static IEnumerable<int> ByteSequence(this int Length)
        {
            var lista = Enumerable.Range(1, Length.SetMinValue(2)).ToList();
            for (int i = 1; i < lista.Count; i++)
            {
                lista[i] = lista[i - 1] * 2;
            }
            return lista;
        }

        /// <summary>
        /// Calcula Juros compostos
        /// </summary>
        /// <param name="Capital">Capital</param>
        /// <param name="Rate">Taxa</param>
        /// <param name="Time">Tempo</param>
        /// <returns></returns>
        public static double CalculateCompoundInterest(this double Capital, double Rate, double Time) => Capital * Math.Pow(1 + Rate, Time);

        /// <inheritdoc cref="CalculateCompoundInterest(double,double,double)"/>
        public static decimal CalculateCompoundInterest(this decimal Capital, decimal Rate, decimal Time) => CalculateCompoundInterest((double)Capital, (double)Rate, (double)Time).ToDecimal();

        /// <summary>
        /// Calcula a porcentagem de cada valor em um dicionario em relação a sua totalidade
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public static Dictionary<TKey, decimal> CalculatePercent<TKey, TValue>(this Dictionary<TKey, TValue> Dic) where TValue : struct
        {
            decimal total = Dic.Sum(x => x.Value.ChangeType<decimal>());
            return Dic.Select(x => new KeyValuePair<TKey, decimal>(x.Key, x.Value.ChangeType<decimal>().CalculatePercent(total))).ToDictionary();
        }

        /// <summary>
        /// Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static Dictionary<TKey, decimal> CalculatePercent<TObject, TKey, TValue>(this IEnumerable<TObject> Obj, Expression<Func<TObject, TKey>> KeySelector, Expression<Func<TObject, TValue>> ValueSelector) where TValue : struct => Obj.ToDictionary(KeySelector.Compile(), ValueSelector.Compile()).CalculatePercent();

        /// <summary>
        /// Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
        /// </summary>
        public static Dictionary<TObject, decimal> CalculatePercent<TObject, TValue>(this IEnumerable<TObject> Obj, Expression<Func<TObject, TValue>> ValueSelector) where TValue : struct => Obj.CalculatePercent(x => x, ValueSelector);

        /// <summary>
        /// Calcula a porcentagem de cada valor de uma classe em relação a sua totalidade em uma lista
        /// </summary>
        public static Dictionary<TValue, decimal> CalculatePercent<TValue>(this IEnumerable<TValue> Obj) where TValue : struct => Obj.DistinctCount().CalculatePercent();

        public static decimal CalculatePercent(this decimal Value, decimal Total) => Total > 0 ? Convert.ToDecimal(100m * Value / Total) : 0;

        public static decimal CalculatePercent(this decimal Value, decimal Total, int DecimalPlaces) => CalculatePercent(Value, Total).RoundDecimal(DecimalPlaces);

        /// <summary>
        /// Calcula a porcentagem de objetos que cumprem um determinado critério em uma lista
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Obj"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static decimal CalculatePercentCompletion<TValue>(this IEnumerable<TValue> Obj, Expression<Func<TValue, bool>> selector)
        {
            var total = Obj.Count();
            if (selector == null)
            {
                selector = x => true;
            }
            var part = Obj.Count(selector.Compile());
            return CalculatePercent(part.ToDecimal(), total.ToDecimal());
        }

        /// <summary>
        /// Calcula a variação percentual entre 2 valores
        /// </summary>
        /// <param name="StartValue"></param>
        /// <param name="EndValue"></param>
        /// <returns></returns>
        public static decimal CalculatePercentVariation(this decimal StartValue, decimal EndValue) => StartValue == 0m ? EndValue > 0m ? 100m : 0m : (EndValue / StartValue - 1m) * 100m;

        /// <summary>
        /// Calcula a variação percentual entre 2 valores
        /// </summary>
        /// <param name="StartValue"></param>
        /// <param name="EndValue"></param>
        /// <returns></returns>
        public static decimal CalculatePercentVariation(this int StartValue, int EndValue) => StartValue.ToDecimal().CalculatePercentVariation(EndValue.ToDecimal());

        /// <summary>
        /// Calcula a variação percentual entre 2 valores
        /// </summary>
        /// <param name="StartValue"></param>
        /// <param name="EndValue"></param>
        /// <returns></returns>
        public static decimal CalculatePercentVariation(this long StartValue, long EndValue) => StartValue.ToDecimal().CalculatePercentVariation(EndValue.ToDecimal());

        /// <summary>
        /// Calcula os Juros simples
        /// </summary>
        /// <param name="Capital">Capital</param>
        /// <param name="Rate">Taxa</param>
        /// <param name="Time">Tempo</param>
        /// <returns></returns>
        public static decimal CalculateSimpleInterest(this decimal Capital, decimal Rate, decimal Time) => Capital * Rate * Time;

        public static decimal CalculateValueFromPercent(this string Percent, decimal Total) => Percent.ReplaceNone("%").ToDecimal().CalculateValueFromPercent(Total);

        public static decimal CalculateValueFromPercent(this int Percent, decimal Total) => Percent.ToDecimal().CalculateValueFromPercent(Total);

        public static decimal CalculateValueFromPercent(this decimal Percent, decimal Total) => Percent * Total / 100m;

        /// <summary>
        /// Retorna todas as possiveis combinações de Arrays do mesmo tipo (Produto Cartesiano)
        /// </summary>
        /// <param name="Sets">Lista de Arrays para combinar</param>
        /// <returns>Produto Cartesiano</returns>
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(params IEnumerable<T>[] Sets)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new IEnumerable<T>[] { Enumerable.Empty<T>() };
            var c = Sets.Aggregate(emptyProduct, (accumulator, sequence) => (from accseq in accumulator from item in sequence select accseq.Concat(new T[] { item })));
            var aa = new List<IEnumerable<T>>();
            foreach (var item in c)
            {
                aa.Add(item);
            }

            return aa;
        }

        public static decimal Ceil(this decimal Number) => Math.Ceiling(Number);

        public static double Ceil(this double Number) => Math.Ceiling(Number);

        public static decimal CeilDecimal(this double Number) => Number.Ceil().ToDecimal();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro</returns>
        public static decimal CeilDecimal(this decimal Number) => Number.Ceil().ToDecimal();

        public static double CeilDouble(this double Number) => Number.Ceil().ToDouble();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro</returns>
        public static double CeilDouble(this decimal Number) => Number.Ceil().ToDouble();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int CeilInt(this double Number) => Number.Ceil().ToInt();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int CeilInt(this decimal Number) => Number.Ceil().ToInt();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long CeilLong(this double Number) => Number.Ceil().ToLong();

        /// <summary>
        /// Arredonda um numero para cima. Ex.: 4,5 -&gt; 5
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long CeilLong(this decimal Number) => Number.Ceil().ToLong();

        /// <summary>
        /// The Collatz conjecture is one of the most famous unsolved problems in mathematics. The
        /// conjecture asks whether repeating two simple arithmetic operations will eventually
        /// transform every positive integer into 1
        /// </summary>
        /// <param name="n">Natural number greater than zero</param>
        /// <returns>an <see cref="IEnumerable{decimal}"/> with all steps until 1</returns>
        /// <exception cref="ArgumentException"></exception>
        public static IEnumerable<decimal> CollatzConjecture(this int n)
        {
            if (n < 1)
            {
                throw new ArgumentException("n must be a natural number greater than zero.", nameof(n));
            }

            yield return n;

            decimal _n = n; //n precisa ser decimal

            while (_n > 1)
            {
                if (_n.IsEven())
                {
                    _n /= 2;
                }
                else
                {
                    _n = _n * 3 + 1;
                }

                yield return _n;
            }
        }

        public static double DegreesToRadians(this double degrees) => degrees * Math.PI / 180;

        /// <summary>
        /// Retorna a diferença entre 2 numeros se o valor maximo for menor que o total
        /// </summary>
        /// <param name="Total"></param>
        /// <param name="MaxValue"></param>
        /// <returns></returns>
        public static int DifferenceIfMax(this int Total, int MaxValue) => Total > MaxValue ? MaxValue - Total : 0;

        /// <summary>
        /// Retorna a diferença entre 2 numeros se o valor minimo for maior que o total
        /// </summary>
        /// <param name="Total"></param>
        /// <param name="MinValue"></param>
        /// <returns></returns>
        public static int DifferenceIfMin(this int Total, int MinValue) => Total < MinValue ? MinValue - Total : 0;

        /// <summary>
        /// Calcula o fatorial de um numero
        /// </summary>
        /// <param name="Number">Numero inteiro maior que zero</param>
        /// <returns>fatorial do numero inteiro</returns>
        /// <remarks>Numeros negativos serão tratados como numeros positivos</remarks>
        public static int Factorial(this int Number)
        {
            Number = Number.ForcePositive();
            if (Number == 0)
            {
                return Number;
            }
            else
            {
                int fact = Number;
                int counter = Number - 1;
                while (counter > 0)
                {
                    fact *= counter;
                    counter--;
                }

                return fact;
            }
        }

        /// <summary>
        /// Retorna uma sequencia Fibonacci de N numeros
        /// </summary>
        /// <param name="Length">Quantidade de numeros da sequencia</param>
        /// <returns>Lista com a sequencia Fibonacci</returns>
        public static IEnumerable<int> Fibonacci(this int Length)
        {
            var lista = new List<int>();
            lista.AddRange(new[] { 0, 1 });
            for (int index = 2, loopTo = Length - 1; index <= loopTo; index++)
            {
                lista.Add(lista[index - 1] + lista[index - 2]);
            }

            return lista;
        }

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static decimal Floor(this decimal Number) => Math.Floor(Number);

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static double Floor(this double Number) => Math.Floor(Number);

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int FloorInt(this double Number) => Number.Floor().ToInt();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static int FloorInt(this decimal Number) => Number.Floor().ToInt();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long FloorLong(this double Number) => Number.Floor().ToLong();

        /// <summary>
        /// Arredonda um numero para baixo. Ex.: 4,5 -&gt; 4
        /// </summary>
        /// <param name="Number">Numero a ser arredondado</param>
        /// <returns>Um numero inteiro (Integer ou Int)</returns>
        public static long FloorLong(this decimal Number) => Number.Floor().ToLong();

        public static decimal ForceNegative(this decimal Value) => Value > 0m ? -Value : Value;

        public static int ForceNegative(this int Value) => Value > 0 ? -Value : Value;

        public static long ForceNegative(this long Value) => Value > 0L ? -Value : Value;

        public static double ForceNegative(this double Value) => Value > 0d ? -Value : Value;

        public static float ForceNegative(this float Value) => Value > 0f ? -Value : Value;

        public static short ForceNegative(this short Value) => (short)(Value > 0 ? -Value : Value);

        public static decimal ForcePositive(this decimal Value) => Value < 0m ? -Value : Value;

        public static int ForcePositive(this int Value) => Value < 0 ? -Value : Value;

        public static long ForcePositive(this long Value) => Value < 0L ? -Value : Value;

        public static double ForcePositive(this double Value) => Value < 0d ? -Value : Value;

        public static float ForcePositive(this float Value) => Value < 0f ? -Value : Value;

        public static short ForcePositive(this short Value) => (short)(Value < 0 ? -Value : Value);

        /// <summary>
        /// Retorna uma Progressão Gemoétrica com N numeros
        /// </summary>
        /// <param name="FirstNumber"></param>
        /// <param name="[Constant]"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static IEnumerable<int> GeometricProgression(this int FirstNumber, int Constant, int Length)
        {
            Length--;
            yield return FirstNumber;
            do
            {
                FirstNumber *= Constant;
                yield return FirstNumber;
                Length--;
            } while (Length > 0);
        }

        public static int GetDecimalLength(this decimal number) => BitConverter.GetBytes(decimal.GetBits(number)[3])[2];

        public static int GetDecimalLength(this double number) => number.ToDecimal().GetDecimalLength();

        /// <summary> Get the Decimal Part of <see cref="decimal" /> as <see cref="long"> </summary>
        /// <param name="Value"></param> <param name="Length"></param> <returns></returns>
        public static long GetDecimalPart(this decimal Value, int Length = 0)
        {
            Value = Value.ForcePositive();
            Value -= Value.Floor();
            while (Value.HasDecimalPart())
            {
                Value *= 10m;
            }

            return $"{Value}".GetFirstChars(Length).ToLong();
        }

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this int Number) => Number.ToLong().GetOrdinal();

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this decimal Number) => Number.ToLong().GetOrdinal();

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this short Number) => Number.ToLong().GetOrdinal();

        /// <inheritdoc cref="GetOrdinal(long)"/>
        public static string GetOrdinal(this double Number) => Number.ToLong().GetOrdinal();

        /// <summary>
        /// Returns the ordinal suffix for given <paramref name="Number"/>
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string GetOrdinal(this long Number)
        {
            switch (Number)
            {
                case 1L:
                case -1L: return $"st";
                case 2L:
                case -2L: return $"nd";
                case 3L:
                case -3L: return $"rd";
                default: return $"th";
            }
        }

        /// <summary>
        /// Check if number has decimal part
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool HasDecimalPart(this decimal Value) => !(Value.ForcePositive() % 1m == 0m) && Value.ForcePositive() > 0m;

        /// <summary>
        /// Verifica se um numero possui parte decimal
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool HasDecimalPart(this double Value) => Value.ToDecimal().HasDecimalPart();

        /// <summary>
        /// Verifica se um numero é inteiro (não possui casas decimais)
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this decimal Number) => !Number.HasDecimalPart();

        /// <summary>
        /// Verifica se um numero é inteiro (não possui casas decimais)
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static bool IsWholeNumber(this double Number) => !Number.HasDecimalPart();

        /// <summary>
        /// Realiza um calculo de interpolação Linear
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="End"></param>
        /// <param name="Amount"></param>
        /// <returns></returns>
        public static float Lerp(this float Start, float End, float Amount)
        {
            float difference = End - Start;
            float adjusted = difference * Amount;
            return Start + adjusted;
        }

        public static int LimitIndex<T>(this int Int, IEnumerable<T> Collection) => Int.LimitRange<int>(0, Collection.Count() - 1);

        public static long LimitIndex<T>(this long Lng, IEnumerable<T> Collection) => Lng.LimitRange<long>(0, Collection.LongCount() - 1L);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static T LimitRange<T>(this IComparable Number, IComparable MinValue = null, IComparable MaxValue = null) where T : IComparable
        {
            if (MaxValue != null)
            {
                Number = Number.IsLessThan(MaxValue.ChangeType<T>()) ? Number.ChangeType<T>() : MaxValue.ChangeType<T>();
            }

            if (MinValue != null)
            {
                Number = Number.IsGreaterThan(MinValue.ChangeType<T>()) ? Number.ChangeType<T>() : MinValue.ChangeType<T>();
            }

            return (T)Number;
        }

        /// <summary>
        /// Limita um range para um caractere
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static string LimitRange(this string Number, string MinValue = null, string MaxValue = null) => Number.LimitRange<string>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um caractere
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static char LimitRange(this char Number, char? MinValue = null, char? MaxValue = null) => Number.LimitRange<char>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static float LimitRange(this float Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<float>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static int LimitRange(this int Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<int>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static decimal LimitRange(this decimal Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<decimal>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static long LimitRange(this double Number, IComparable MinValue = null, IComparable MaxValue = null) => (long)Math.Round(Number.LimitRange<double>(MinValue, MaxValue));

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static long LimitRange(this long Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<long>(MinValue, MaxValue);

        /// <summary>
        /// Limita um range para um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Minimo para o numero</param>
        /// <param name="MaxValue">Valor máximo para o numero</param>
        /// <returns></returns>
        public static DateTime LimitRange(this DateTime Number, IComparable MinValue = null, IComparable MaxValue = null) => Number.LimitRange<DateTime>(MinValue, MaxValue);

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static decimal RoundDecimal(this decimal Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number, Decimals.Value.ForcePositive()) : Math.Round(Number);

        public static decimal RoundDecimal(this double Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number.ToDecimal(), Decimals.Value.ForcePositive()) : Math.Round(Number.ToDecimal());

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static double RoundDouble(this double Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number, Decimals.Value.ForcePositive()) : Math.Round(Number);

        public static double RoundDouble(this decimal Number, int? Decimals = default) => Decimals.HasValue ? Math.Round(Number.ToDouble(), Decimals.Value.ForcePositive()) : Math.Round(Number.ToDouble());

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static int RoundInt(this decimal Number) => Math.Round(Number).ToInt();

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static int RoundInt(this double Number) => Math.Round(Number).ToInt();

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static long RoundLong(this decimal Number) => Math.Round(Number).ToLong();

        /// <summary>
        /// Arredonda um numero para o valor inteiro mais próximo
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static long RoundLong(this double Number) => Math.Round(Number).ToLong();

        /// <summary>
        /// Limita o valor Maximo de um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MaxValue">Valor Maximo</param>
        /// <returns></returns>
        public static T SetMaxValue<T>(this T Number, T MaxValue) where T : IComparable => Number.LimitRange<T>(MaxValue: MaxValue);

        /// <summary>
        /// Limita o valor minimo de um numero
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <param name="MinValue">Valor Maximo</param>
        /// <returns></returns>
        public static T SetMinValue<T>(this T Number, T MinValue) where T : IComparable => Number.LimitRange<T>(MinValue: MinValue);

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static double Sum(params double[] Values) => Values.Sum();

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static long Sum(params long[] Values) => Values.Sum();

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static int Sum(params int[] Values) => Values.Sum();

        /// <summary>
        /// Soma todos os números de um array
        /// </summary>
        /// <param name="Values">Array de números</param>
        /// <returns>Decimal contendo a soma de todos os valores</returns>
        public static decimal Sum(params decimal[] Values) => Values.Sum();

        public static string ToDecimalString(this float number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this short number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this double number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this long number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this int number, int Decimals = -1, CultureInfo culture = null) => number.ToDecimal().ToDecimalString(Decimals, culture);

        public static string ToDecimalString(this decimal number, int Decimals = -1, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.CurrentCulture;
            Decimals = Decimals < 0 ? GetDecimalLength(number) : Decimals;
            Decimals = Decimals < 0 ? culture.NumberFormat.NumberDecimalDigits : Decimals;
            return number.ToString("0".AppendIf(culture.NumberFormat.NumberDecimalSeparator + "0".Repeat(Decimals), Decimals > 0), culture);
        }

        #region Public Methods

        /// <summary>
        /// Retorna o Mime T a partir da extensão de um arquivo
        /// </summary>
        /// <param name="Extension">extensão do arquivo</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetFileType(this string Extension) => FileType.GetFileType(Extension).GetMimeTypesOrDefault();

        /// <summary>
        /// Retorna o Mime T a partir de um arquivo
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetFileType(this FileInfo File) => File.Extension.GetFileType();

        /// <summary>
        /// Retorna o Mime T a partir de de um formato de Imagem
        /// </summary>
        /// <param name="RawFormat">Formato de Imagem</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetFileType(this ImageFormat RawFormat)
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
            catch
            {
            }

            return GetFileType(".png");
        }

        /// <summary>
        /// Retorna o Mime T a partir de de uma Imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>string mime type</returns>
        public static IEnumerable<string> GetFileType(this Image Image) => Image.RawFormat.GetFileType();

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
            catch
            {
                return SystemIcons.WinLogo;
            }
        }

        /// <summary>
        /// Retorna um Objeto FileType a partir de uma string MIME T, Nome ou Extensão de Arquivo
        /// </summary>
        /// <param name="MimeTypeOrExtensionOrPathOrDataURI"></param>
        /// <returns></returns>
        public static FileType ToFileType(this string MimeTypeOrExtensionOrPathOrDataURI) => new FileType(MimeTypeOrExtensionOrPathOrDataURI);

        #endregion Public Methods

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this int Number) => Number.ToLong().ToOrdinalNumber();

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this long Number) => $"{Number}{Number.GetOrdinal()}";

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this short Number) => Number.ToInt().ToOrdinalNumber();

        /// <summary>
        /// retorna o numeor em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this double Number) => Number.FloorInt().ToOrdinalNumber();

        /// <summary>
        /// Retorna o numero em sua forma ordinal (inglês)
        /// </summary>
        /// <param name="Number">Numero</param>
        /// <returns></returns>
        public static string ToOrdinalNumber(this decimal Number) => Number.FloorInt().ToOrdinalNumber();

        /// <summary>
        /// COnverte graus para radianos
        /// </summary>
        /// <param name="Degrees"></param>
        /// <returns></returns>
        public static double ToRadians(this double Degrees) => Degrees * Math.PI / 180.0d;

        #endregion Public Methods

        public static string ToAsciiArt(this Bitmap image, int ratio)
        {
            image = (Bitmap)image.Negative();
            bool toggle = false;
            var sb = new StringBuilder();
            int h = 0;
            while (h < image.Height)
            {
                int w = 0;
                while (w < image.Width)
                {
                    var pixelColor = image.GetPixel(w, h);
                    int red, green, blue;
                    red = (int)Math.Round((pixelColor.R.ToInt() + pixelColor.G.ToInt() + pixelColor.B.ToInt()) / 3d);
                    green = red;
                    blue = green;
                    var grayColor = Color.FromArgb(red, green, blue);
                    if (!toggle)
                    {
                        int index = (int)Math.Round(grayColor.R * 10 / 255d);
                        sb.Append(PredefinedArrays.AsciiArtChars.ToArray()[index]);
                    }

                    w += ratio;
                }

                if (!toggle)
                {
                    sb.AppendLine();
                    toggle = true;
                }
                else
                {
                    toggle = false;
                }

                h += ratio;
            }

            return sb.ToString();
        }

        #region Public Fields

        public const string DoubleQuoteChar = "\"";
        public const string EmptyString = "";
        public const string SingleQuoteChar = "\'";
        public const string WhitespaceChar = " ";

        #endregion Public Fields

        #region Public Methods

        /// <summary>
        /// Retorna uma string em ordem afabética baseada em uma outra string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Alphabetize(this string Text)
        {
            var a = Text.IfBlank(EmptyString).ToCharArray();
            Array.Sort(a);
            return a.SelectJoinString(EmptyString);
        }

        /// <summary>
        /// Adiciona texto ao fim de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string Append(this string Text, string AppendText)
        {
            Text = Text ?? string.Empty;
            AppendText = AppendText ?? string.Empty;
            Text += AppendText;
            return Text;
        }

        /// <summary>
        /// Adiciona um digito verificador calulado com Mod10 ao <paramref name="Code"/>
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public static string AppendBarcodeCheckSum(this string Code) => Code.Append(Ext.BarcodeCheckSum(Code));

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendIf(this string Text, string AppendText, bool Test)
        {
            Text = Text ?? string.Empty;
            AppendText = AppendText ?? string.Empty;
            return Test ? Text.Append(AppendText) : Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendIf(this string Text, string AppendText, Func<string, bool> Test) => AppendIf(Text, AppendText, (Test ?? (x => false))(Text));

        /// <summary>
        /// Adiciona texto ao final de uma string com uma quebra de linha no final do <paramref name="AppendText"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        public static string AppendLine(this string Text, string AppendText) => Text.Append(AppendText).Append(Environment.NewLine);

        public static string AppendUrlParameter(this string Url, string Key, params string[] Value)
        {
            if (Url.IsURL())
            {
                Url.ParseQueryString();
                foreach (var v in Value ?? Array.Empty<string>())
                {
                    Url += $"&{Key}={v?.UrlEncode()}";
                }
                return Url;
            }
            throw new ArgumentException("string is not a valid URL", nameof(Url));
        }

        /// <summary>
        /// Adiciona texto ao final de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="AppendText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string AppendWhile(this string Text, string AppendText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);
            while (Test(Text))
            {
                Text = Text.Append(AppendText);
            }

            return Text;
        }

        /// <summary>
        /// Aplica espacos em todos os caracteres de encapsulamento
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ApplySpaceOnWrapChars(this string Text)
        {
            Text = Text ?? EmptyString;
            foreach (var c in PredefinedArrays.WordWrappers)
            {
                Text = Text.Replace(c, WhitespaceChar + c + WhitespaceChar);
            }

            return Text;
        }

        /// <summary>
        /// Encapsula um texto em uma caixa. Funciona somente com fonte monoespaçadas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxText(this string Text, char BoxChar = '*')
        {
            var Lines = Text.SplitAny(PredefinedArrays.BreakLineChars.ToArray()).ToList();
            string linha_longa = string.Empty;
            int charcount = Lines.Max(x => x.Length);
            if (charcount.IsEven())
            {
                charcount++;
            }

            for (int i = 0, loopTo = Lines.Count - 1; i <= loopTo; i++)
            {
                Lines[i] = Lines[i].PadRight(charcount);
            }

            for (int i = 0, loopTo1 = Lines.Count - 1; i <= loopTo1; i++)
            {
                Lines[i] = $"{BoxChar} {Lines[i]} {BoxChar}";
            }

            charcount = Lines.Max(x => x.Length);
            while (linha_longa.Length < charcount)
            {
                linha_longa += $"{BoxChar} ";
            }

            linha_longa = linha_longa.Trim();
            Lines.Insert(0, linha_longa);
            Lines.Add(linha_longa);
            string box = Lines.SelectJoinString(Environment.NewLine);
            return box;
        }

        /// <summary>
        /// Encapsula um texto em uma caixa incorporado em comentários CSS
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string BoxTextCSS(this string Text) => $"/*{Text.BoxText().Wrap(Environment.NewLine)}*/";

        /// <summary>
        /// Encapsula um tento entre 2 caracteres (normalmente parentesis, chaves, aspas ou
        /// colchetes) é um alias de <see cref="Quote(String, Char)"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BracketChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Brackfy(this string Text, char BracketChar = '{') => Text.Quote(BracketChar);

        /// <summary>
        /// Censura as palavras de um texto substituindo as palavras indesejadas por * (ou outro
        /// caractere desejado) e retorna um valor indicando se o texto precisou ser censurado
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BadWords">Lista de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        /// <returns>
        /// TRUE se a frase precisou ser censurada, FALSE se a frase não precisou de censura
        /// </returns>
        public static (string Text, bool IsCensored) Censor(this string Text, IEnumerable<string> BadWords, char CensorshipCharacter)
        {
            var words = Text.Split(WhitespaceChar, StringSplitOptions.None);
            BadWords = BadWords ?? Array.Empty<string>();
            var IsCensored = false;
            if (words.ContainsAny(BadWords))
            {
                foreach (var bad in BadWords)
                {
                    string censored = string.Empty;
                    for (int index = 1, loopTo = bad.Length; index <= loopTo; index++)
                    {
                        censored += CensorshipCharacter;
                    }

                    for (int index = 0, loopTo1 = words.Length - 1; index <= loopTo1; index++)
                    {
                        if ((words[index].RemoveDiacritics().RemoveAny(PredefinedArrays.WordSplitters.ToArray()).ToLowerInvariant() ?? string.Empty) == (bad.RemoveDiacritics().RemoveAny(PredefinedArrays.WordSplitters.ToArray()).ToLowerInvariant() ?? string.Empty))
                        {
                            words[index] = words[index].ToLowerInvariant().Replace(bad, censored);
                            IsCensored = true;
                        }
                    }
                }

                Text = words.SelectJoinString(WhitespaceChar);
            }
            return (Text, IsCensored);
        }

        /// <summary>
        /// Retorna um novo texto censurando as palavras de um texto substituindo as palavras
        /// indesejadas por um caractere desejado)
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="BadWords">Array de palavras indesejadas</param>
        /// <param name="CensorshipCharacter">Caractere que será aplicado nas palavras censuradas</param>
        public static (string Text, bool IsCensored) Censor(this string Text, char CensorshipCharacter, params string[] BadWords) => Text.Censor((BadWords ?? Array.Empty<string>()).ToList(), CensorshipCharacter);

        /// <summary>
        /// Verifica se um texto contém outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool Contains(this string Text, string OtherText, StringComparison StringComparison) => Text.IndexOf(OtherText, StringComparison) > -1;

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsAll(this string Text, params string[] Values) => Text.ContainsAll(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma String contém todos os valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <param name="ComparisonType">Tipo de comparacao</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAll(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            Values = Values ?? Array.Empty<string>();
            if (Values.Any())
            {
                foreach (string value in Values)
                {
                    if (Text == null || Text.IndexOf(value, ComparisonType) == -1)
                    {
                        return false;
                    }
                }

                return true;
            }

            return Text.IsBlank();
        }

        public static bool ContainsAllWords(this string Text, params string[] Words) => Text.ContainsAllWords(null, Words);

        public static bool ContainsAllWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words) => Text.GetWords().ContainsAll(Words, Comparer);

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter algum valor, false se não</returns>
        public static bool ContainsAny(this string Text, params string[] Values) => Text.ContainsAny(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma String contém qualquer um dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <param name="ComparisonType">Tipo de comparacao</param>
        /// <returns>True se conter algum valor, false se não</returns>
        /// <remarks>
        /// Caso <paramref name="Values"/> for nulo ou vazio, retorna <b>true</b> se <paramref
        /// name="Text"/> não estiver em branco,caso contrário, <b>false</b>
        /// </remarks>
        public static bool ContainsAny(this string Text, StringComparison ComparisonType, params string[] Values)
        {
            Values = (Values ?? Array.Empty<string>()).Where(x => x != null && x != string.Empty).ToArray();
            if (Values.Any())
            {
                foreach (string value in Values)
                {
                    if (Text != null && Text.IndexOf(value, ComparisonType) != -1)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                return Text.IsNotBlank();
            }
        }

        public static bool ContainsAnyWords(this string Text, params string[] Words) => Text.ContainsAnyWords(null, Words);

        public static bool ContainsAnyWords(this string Text, IEqualityComparer<string> Comparer, params string[] Words) => Text.GetWords().ContainsAny(Words, Comparer);

        /// <summary>
        /// Verifica se uma string contém caracteres de digito
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsDigit(this string Text) => (Text ?? string.Empty).ToArray().Any(char.IsDigit);

        /// <summary>
        /// Verifica se uma string contém caracteres em minusculo
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsLower(this string Text) => (Text ?? string.Empty).ToArray().Any(char.IsLower);

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter a maioria dos valores, false se não</returns>
        public static bool ContainsMost(this string Text, StringComparison ComparisonType, params string[] Values) => (Values ?? Array.Empty<string>()).Most(value => Text != null && Text.Contains(value, ComparisonType));

        /// <summary>
        /// Verifica se uma string contém a maioria dos valores especificados
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Values">Lista de valores</param>
        /// <returns>True se conter todos os valores, false se não</returns>
        public static bool ContainsMost(this string Text, params string[] Values) => Text.ContainsMost(StringComparison.InvariantCultureIgnoreCase, Values);

        /// <summary>
        /// Verifica se uma string contém caracteres em maiúsculo
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool ContainsUpper(this string Text) => (Text ?? InnerLibs.Ext.EmptyString).ToArray().Any(char.IsUpper);

        /// <summary>
        /// Conta os caracters especificos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Character">Caractere</param>
        /// <returns></returns>
        public static int CountCharacter(this string Text, char Character) => Text.Count((c) => c == Character);

        /// <summary>
        /// Retorna as plavaras contidas em uma frase em ordem alfabética e sua respectiva quantidade
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="RemoveDiacritics">indica se os acentos devem ser removidos das palavras</param>
        /// <param name="Words">
        /// Desconsidera outras palavras e busca a quantidade de cada palavra especificada em um array
        /// </param>
        /// <returns></returns>
        public static Dictionary<string, long> CountWords(this string Text, bool RemoveDiacritics = true, string[] Words = null)
        {
            if (Words == null)
            {
                Words = Array.Empty<string>();
            }

            var palavras = Text.Split(PredefinedArrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToArray();
            if (Words.Any())
            {
                palavras = palavras.Where(x => Words.Select(y => y.ToLowerInvariant()).Contains(x.ToLowerInvariant())).ToArray();
            }

            if (RemoveDiacritics)
            {
                palavras = palavras.Select(p => p.RemoveDiacritics()).ToArray();
                Words = Words.Select(p => p.RemoveDiacritics()).ToArray();
            }

            var dic = palavras.DistinctCount();
            foreach (var w in Words.Where(x => !dic.Keys.Contains(x)))
            {
                dic.Add(w, 0L);
            }

            return dic;
        }

        /// <summary>
        /// Verifica se um texto contém outro ou vice versa
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OtherText"></param>
        /// <returns></returns>
        public static bool CrossContains(this string Text, string OtherText, StringComparison StringComparison = StringComparison.InvariantCultureIgnoreCase) => Text.Contains(OtherText, StringComparison) || OtherText.Contains(Text, StringComparison);

        /// <summary>
        /// Remove uma linha especifica de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="LineIndex">Numero da linha</param>
        /// <returns></returns>
        public static string DeleteLine(this string Text, int LineIndex)
        {
            LineIndex = LineIndex.SetMinValue(0);
            var parts = Text.Split(Environment.NewLine).ToList();

            if (parts.Count > LineIndex)
            {
                parts.RemoveAt(LineIndex);
            }

            return parts.SelectJoinString(Environment.NewLine);
        }

        /// <summary>
        /// Cria um dicionário com as palavras de uma lista e a quantidade de cada uma.
        /// </summary>
        /// <param name="List">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(params string[] List) => List.ToList().DistinctCount();

        /// <summary>
        /// Cria um dicionário com as palavras de uma frase e sua respectiva quantidade.
        /// </summary>
        /// <param name="Text">Lista de palavras</param>
        /// <returns></returns>
        public static Dictionary<string, long> DistinctCount(this string Text) => Text.Split(WhitespaceChar).ToList().DistinctCount();

        /// <summary>
        /// Verifica se uma string termina com alguma outra string de um array
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool EndsWithAny(this string Text, StringComparison comparison, params string[] Words) => Words.Any(p => Text.EndsWith(p, comparison));

        public static bool EndsWithAny(this string Text, params string[] Words) => EndsWithAny(Text, default, Words);

        public static bool EqualsIgnoreCase(this string Text, string CompareText) => string.Equals(Text ?? "", CompareText ?? "", StringComparison.OrdinalIgnoreCase);

        public static bool EqualsIgnoreCaseAndAccents(this string Text, string CompareText) => EqualsIgnoreCase(Text.RemoveAccents(), CompareText.RemoveAccents());

        /// <summary>
        /// Prepara uma string com aspas simples para uma Query TransactSQL
        /// </summary>
        /// <param name="Text">Texto a ser tratado</param>
        /// <returns>String pronta para a query</returns>
        public static string EscapeQuotesToQuery(this string Text, bool AlsoQuoteText = false) => Text.Replace(InnerLibs.Ext.SingleQuoteChar, "''").QuoteIf(AlsoQuoteText, '\'');

        /// <summary>
        /// Extrai emails de uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> ExtractEmails(this string Text) => Text.IfBlank(string.Empty).SplitAny(PredefinedArrays.InvisibleChars.Union(PredefinedArrays.BreakLineChars).ToArray()).Where(x => x.IsEmail()).Select(x => x.ToLowerInvariant()).Distinct().ToArray();

        /// <summary>
        /// Procura CEPs em uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static string[] FindByRegex(this string Text, string Regex, RegexOptions RegexOptions = RegexOptions.None)
        {
            var textos = new List<string>();
            foreach (Match m in new Regex(Regex, RegexOptions).Matches(Text))
            {
                textos.Add(m.Value);
            }

            return textos.ToArray();
        }

        /// <summary>
        /// Procura CEPs em uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string[] FindCEP(this string Text) => Text.FindByRegex(@"\d{5}-\d{3}").Union(Text.FindNumbers().Where(x => x.Length == 8)).ToArray();

        /// <summary>
        /// Procura numeros em uma string e retorna um array deles
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> FindNumbers(this string Text)
        {
            var l = new List<string>();
            var numbers = Regex.Split(Text, @"\D+");
            foreach (var value in numbers)
            {
                if (!value.IsBlank())
                {
                    l.Add(value);
                }
            }

            return l;
        }

        public static string FixCapitalization(this string Text)
        {
            Text = Text.Trim().GetFirstChars(1).ToUpperInvariant() + Text.RemoveFirstChars(1);
            var dots = new[] { "...", ". ", "? ", "! " };
            List<string> sentences;
            foreach (var dot in dots)
            {
                sentences = Text.Split(dot, StringSplitOptions.None).ToList();
                for (int index = 0, loopTo = sentences.Count - 1; index <= loopTo; index++)
                {
                    sentences[index] = string.Empty + sentences[index].Trim().GetFirstChars(1).ToUpperInvariant() + sentences[index].RemoveFirstChars(1);
                }

                Text = sentences.SelectJoinString(dot);
            }

            sentences = Text.Split(WhitespaceChar).ToList();
            Text = InnerLibs.Ext.EmptyString;
            foreach (var c in sentences)
            {
                string palavra = c;
                if (palavra.EndsWith(".") && palavra.Length == 2)
                {
                    palavra = palavra.ToUpperInvariant();
                    Text += palavra;
                    string proximapalavra = sentences.IfNoIndex(sentences.IndexOf(c) + 1, InnerLibs.Ext.EmptyString);
                    if (!(proximapalavra.EndsWith(".") && palavra.Length == 2))
                    {
                        Text += WhitespaceChar;
                    }
                }
                else
                {
                    Text += c + WhitespaceChar;
                }
            }

            return Text.RemoveLastChars(1);
        }

        /// <summary>
        /// Transforma quebras de linha HTML em quebras de linha comuns ao .net
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <returns>String fixada</returns>
        public static string FixHTMLBreakLines(this string Text)
        {
            Text = Text.ReplaceMany(Environment.NewLine, "<br/>", "<br />", "<br>");
            return Text.Replace("&nbsp;", WhitespaceChar);
        }

        /// <summary>
        /// Ajusta um caminho colocando as barras corretamente e substituindo caracteres inválidos
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string FixPath(this string Text, bool AlternativeChar = false)
        {
            return Text.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries).Where(x => x.IsNotBlank()).Select((x, i) =>
            {
                if (i == 0 && x.Length == 2 && x.EndsWith(":"))
                {
                    return x;
                }

                return x.ToFriendlyPathName();
            }).SelectJoinString(AlternativeChar.AsIf(Path.AltDirectorySeparatorChar.ToString(), Path.DirectorySeparatorChar.ToString())).TrimEndAny(Path.DirectorySeparatorChar.ToString(), Path.AltDirectorySeparatorChar.ToString());
        }

        /// <summary>
        /// Adciona pontuação ao final de uma string se a mesma não terminar com alguma pontuacao.
        /// </summary>
        /// <param name="Text">Frase, Texto a ser pontuado</param>
        /// <param name="Punctuation">
        /// Ponto a ser adicionado na frase se a mesma não estiver com pontuacao
        /// </param>
        /// <returns>Frase corretamente pontuada</returns>
        public static string FixPunctuation(this string Text, string Punctuation = ".", bool ForceSpecificPunctuation = false)
        {
            Text = Text.TrimEndAny(true, ",", WhitespaceChar);
            var pts = new[] { ".", "!", "?", ":", ";" };
            if (ForceSpecificPunctuation)
            {
                Text = Text.TrimEndAny(true, pts).Trim() + Punctuation;
            }
            else if (!Text.EndsWithAny(pts))
            {
                Text += Punctuation;
            }

            return Text;
        }

        /// <summary>
        /// Arruma a ortografia do texto captalizando corretamente, adcionando pontuação ao final de
        /// frase caso nescessário e removendo espaços excessivos ou incorretos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string FixText(this string Text, int Ident = 0, int BreakLinesBetweenParagraph = 0)
        {
            Text = Text.IfBlank("");
            var removedot = !Text.Trim().EndsWith(".");
            var addComma = Text.Trim().EndsWith(",");
            Text = new TextStructure(Text) { Ident = Ident, BreakLinesBetweenParagraph = BreakLinesBetweenParagraph }.ToString();
            if (removedot)
            {
                Text = Text.TrimEnd().TrimEndAny(".");
            }
            if (addComma)
            {
                Text = Text.TrimEnd().TrimEndAny(".").Append(",");
            }
            return Text.Trim().TrimBetween();
        }

        /// <summary>
        /// Executa uma ação para cada linha de um texto
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        public static string ForEachLine(this string Text, Expression<Func<string, string>> Action)
        {
            if (Text.IsNotBlank() && Action != null)
            {
                Text = Text.SplitAny(PredefinedArrays.BreakLineChars.ToArray()).Select(x => Action.Compile().Invoke(x)).SelectJoinString(Environment.NewLine);
            }

            return Text;
        }

        /// <summary>
        /// Extension Method para <see cref="String.Format(String,Object())"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Args">Objetos de substituição</param>
        /// <returns></returns>
        public static string FormatString(this string Text, params string[] Args) => string.Format(Text, Args);

        /// <summary>
        /// Retorna um texto posterior a outro
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Value">Texto Posterior</param>
        /// <returns>Uma string com o valor posterior ao valor especificado.</returns>
        public static string GetAfter(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            Value = Value.IfBlank(InnerLibs.Ext.EmptyString);

            return Text.IsBlank() || Text.IndexOf(Value) == -1
                ? WhiteIfNotFound ? EmptyString : $"{Text}"
                : Text.Substring(Text.IndexOf(Value) + Value.Length);
        }

        /// <summary>
        /// Retorna todas as ocorrencias de um texto entre dois textos
        /// </summary>
        /// <param name="Text">T texto correspondente</param>
        /// <param name="Before">T texto Anterior</param>
        /// <param name="After">T texto Posterior</param>
        /// <returns>Uma String com o texto entre o texto anterior e posterior</returns>
        public static string[] GetAllBetween(this string Text, string Before, string After = EmptyString)
        {
            var lista = new List<string>();
            string regx = Before.RegexEscape() + "(.*?)" + After.IfBlank(Before).RegexEscape();
            var mm = new Regex(regx, (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Matches(Text);
            foreach (Match a in mm)
            {
                lista.Add(a.Value.TrimFirstEqual(Before).TrimLastEqual(After));
            }

            return lista.ToArray();
        }

        /// <summary>
        /// Retorna um texto anterior a outro
        /// </summary>
        /// <param name="Text">Texto correspondente</param>
        /// <param name="Value">Texto Anterior</param>
        /// <returns>Uma string com o valor anterior ao valor especificado.</returns>
        public static string GetBefore(this string Text, string Value, bool WhiteIfNotFound = false)
        {
            Value = Value.IfBlank(InnerLibs.Ext.EmptyString);
            return Text.IsBlank() || Text.IndexOf(Value) == -1 ? WhiteIfNotFound ? InnerLibs.Ext.EmptyString : $"{Text}" : Text.Substring(0, Text.IndexOf(Value));
        }

        /// <summary>
        /// Retorna o texto entre dois textos
        /// </summary>
        /// <param name="Text">T texto correspondente</param>
        /// <param name="Before">T texto Anterior</param>
        /// <param name="After">T texto Posterior</param>
        /// <returns>Uma String com o texto entre o texto anterior e posterior</returns>
        public static string GetBetween(this string Text, string Before, string After)
        {
            if (Text.IsNotBlank())
            {
                int beforeStartIndex = Text.IndexOf(Before);
                int startIndex = beforeStartIndex + Before.Length;
                int afterStartIndex = Text.IndexOf(After, startIndex);
                return beforeStartIndex < 0 || afterStartIndex < 0 ? Text : Text.Substring(startIndex, afterStartIndex - startIndex);
            }
            return InnerLibs.Ext.EmptyString;
        }

        /// <summary>
        /// Pega o dominio principal de uma URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomain(this Uri URL, bool RemoveFirstSubdomain = false)
        {
            string d = URL.Authority;
            if (RemoveFirstSubdomain)
            {
                d = d.Split(".").Skip(1).SelectJoinString(".");
            }

            return d;
        }

        /// <summary>
        /// Pega o dominio principal de uma URL ou email
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomain(this string URL, bool RemoveFirstSubdomain = false)
        {
            if (URL.IsEmail())
            {
                URL = $"http://{URL.GetAfter("@")}";
            }

            if (!URL.IsURL())
            {
                URL.Prepend("http://");
            }

            return new Uri(URL).GetDomain(RemoveFirstSubdomain);
        }

        /// <summary>
        /// Pega o protocolo e o dominio principal de uma URL
        /// </summary>
        /// <param name="URL">URL</param>
        /// <returns>nome do dominio</returns>
        public static string GetDomainAndProtocol(this string URL) => $"{new Uri(URL.PrependIf("http://", x => x.IsURL() == false)).GetLeftPart(UriPartial.Authority)}";

        public static string GetFirstChars(this string Text, int Number = 1) => Text.IsNotBlank() ? Text.Length < Number || Number < 0 ? Text : Text.Substring(0, Number) : InnerLibs.Ext.EmptyString;

        public static string GetLastChars(this string Text, int Number = 1) => Text.IsNotBlank() ? Text.Length < Number || Number < 0 ? Text : Text.Substring(Text.Length - Number) : InnerLibs.Ext.EmptyString;

        /// <summary>
        /// Retorna N caracteres de uma string a partir do caractere encontrado no centro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetMiddleChars(this string Text, int Length)
        {
            Text = Text.IfBlank(InnerLibs.Ext.EmptyString);
            if (Text.Length >= Length)
            {
                if (Text.Length % 2 != 0)
                {
                    try
                    {
                        return Text.Substring((int)Math.Round(Text.Length / 2d - 1d), Length);
                    }
                    catch
                    {
                        return Text.GetMiddleChars(Length - 1);
                    }
                }
                else
                {
                    return Text.RemoveLastChars(1).GetMiddleChars(Length);
                }
            }

            return Text;
        }

        /// <summary>
        /// Retorna o caractere de encapsulamento oposto ao caractere indicado
        /// </summary>
        /// <param name="Text">Caractere</param>
        /// <returns></returns>
        public static string GetOppositeWrapChar(this string Text)
        {
            switch (Text.GetFirstChars() ?? EmptyString)
            {
                case DoubleQuoteChar: return DoubleQuoteChar;
                case SingleQuoteChar: return SingleQuoteChar;
                case "(": return ")";
                case ")": return "(";
                case "[": return "]";
                case "]": return "[";
                case "{": return "}";
                case "}": return "{";
                case "<": return ">";
                case ">": return "<";
                case @"\": return "/";
                case "/": return @"\";
                case "¿": return "?";
                case "?": return "¿";
                case "!": return "¡";
                case "¡": return "!";
                case ".": return ".";
                case ":": return ":";
                case ";": return ";";
                case "_": return "_";
                case "*": return "*";
                default: return Text;
            }
        }

        public static char GetOppositeWrapChar(this char c) => $"{c}".GetOppositeWrapChar().FirstOrDefault();

        /// <summary>
        /// Sorteia um item da Lista
        /// </summary>
        /// <typeparam name="T">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static T GetRandomItem<T>(this T[] Array) => Array == null || Array.Length == 0 ? default(T) : Array[Ext.RandomNumber(0, Array.Length - 1)];

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this Uri URL, bool WithQueryString = true) => WithQueryString ? URL.PathAndQuery : URL.AbsolutePath;

        /// <summary>
        /// Retorna o caminho relativo da url
        /// </summary>
        /// <param name="URL">Url</param>
        /// <returns></returns>
        public static string GetRelativeURL(this string URL, bool WithQueryString = true) => URL.IsURL() ? new Uri(URL).GetRelativeURL(WithQueryString) : null;

        /// <summary>
        /// Corta um texto para exibir um numero máximo de caracteres ou na primeira quebra de linha.
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="TextLength"></param>
        /// <param name="Ellipsis"></param>
        /// <returns></returns>
        public static string GetTextPreview(this string Text, int TextLength, string Ellipsis = "...", bool BeforeNewLine = true)
        {
            if (Text.IsBlank() || Text?.Length <= TextLength || TextLength <= 0)
            {
                return Text;
            }
            else
            {
                if (BeforeNewLine)
                {
                    Text = Text.TrimCarriage().GetBefore(Environment.NewLine);
                    if (TextLength == 0) return Text;
                }

                return $"{Text.GetFirstChars(TextLength)}{Ellipsis ?? ""}";
            }
        }

        /// <summary>
        /// Retorna uma lista de palavras encontradas no texto em ordem alfabetica
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<string> GetWords(this string Text)
        {
            var txt = new List<string>();
            var palavras = Text.TrimBetween().FixHTMLBreakLines().ToLowerInvariant().RemoveHTML().Split(PredefinedArrays.WordSplitters.ToArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var w in palavras)
            {
                txt.Add(w);
            }

            return txt.Distinct().OrderBy(x => x);
        }

        /// <summary>
        /// Captura todas as sentenças que estão entre aspas ou parentesis ou chaves ou colchetes em
        /// um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string[] GetWrappedText(this string Text, string Character = DoubleQuoteChar, bool ExcludeWrapChars = true)
        {
            var lista = new List<string>();
            string regx = $"{Character.RegexEscape()}(.*?){Character.GetOppositeWrapChar().RegexEscape()}";
            var mm = new Regex(regx, (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).Matches(Text);
            foreach (Match a in mm)
            {
                if (ExcludeWrapChars)
                {
                    lista.Add(a.Value.TrimFirstEqual(Character).TrimLastEqual(Character.GetOppositeWrapChar()));
                }
                else
                {
                    lista.Add(a.Value);
                }
            }

            return lista.ToArray();
        }

        public static bool HasLength(this string Text, int Length) => Text != null && Text.Length == Length;

        public static bool HasMaxLength(this string Text, int Length) => Text != null && Text.Length <= Length;

        public static bool HasMinLength(this string Text, int Length) => Text != null && Text.Length >= Length;

        /// <summary>
        /// Retorna um texto com entidades HTML convertidas para caracteres e tags BR em breaklines
        /// </summary>
        /// <param name="Text">string HTML</param>
        /// <returns>String HTML corrigido</returns>
        public static string HtmlDecode(this string Text) => System.Net.WebUtility.HtmlDecode(InnerLibs.Ext.EmptyString + Text).ReplaceMany(Environment.NewLine, "<br/>", "<br />", "<br>");

        /// <summary>
        /// Escapa o texto HTML
        /// </summary>
        /// <param name="Text">string HTML</param>
        /// <returns>String HTML corrigido</returns>
        public static string HtmlEncode(this string Text) => System.Net.WebUtility.HtmlEncode(InnerLibs.Ext.EmptyString + Text.ReplaceMany("<br>", PredefinedArrays.BreakLineChars.ToArray()));

        /// <summary>
        /// Inject the property values of <typeparamref name="T"/> into <paramref name="TemplatedString"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatString"></param>
        /// <param name="injectionObject"></param>
        /// <returns></returns>
        public static string Inject<T>(this T Obj, string TemplatedString, bool IsSQL = false) => TemplatedString.IfBlank(InnerLibs.Ext.EmptyString).Inject(Obj, IsSQL);

        /// <summary>
        /// Inject the property values of <typeparamref name="T"/> into <see cref="String"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="formatString"></param>
        /// <param name="injectionObject"></param>
        /// <returns></returns>
        public static string Inject<T>(this string formatString, T injectionObject, bool IsSQL = false)
        {
            if (injectionObject != null)
            {
                return injectionObject.IsDictionary()
                    ? formatString.Inject(new Hashtable((IDictionary)injectionObject), IsSQL)
                    : formatString.Inject(Ext.GetPropertyHash(injectionObject), IsSQL);
            }

            return formatString;
        }

        /// <summary>
        /// Inject a <see cref="Hashtable"/> into <see cref="String"/>
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static string Inject(this string formatString, Hashtable attributes, bool IsSQL = false)
        {
            string result = formatString;
            if (attributes != null && formatString != null)
            {
                foreach (string attributeKey in attributes.Keys)
                {
                    result = result.InjectSingleValue(attributeKey, attributes[attributeKey], IsSQL);
                }
            }

            return result;
        }

        /// <summary>
        /// Replace te found <paramref name="key"/> with <paramref name="replacementValue"/>
        /// </summary>
        /// <param name="formatString"></param>
        /// <param name="key"></param>
        /// <param name="replacementValue"></param>
        /// <returns></returns>
        public static string InjectSingleValue(this string formatString, string key, object replacementValue, bool IsSQL = false, CultureInfo cultureInfo = null)
        {
            string result = formatString ?? "";
            var attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");
            foreach (Match m in attributeRegex.Matches(formatString))
            {
                string replacement = m.ToString();
                if (m.Groups[2].Length > 0)
                {
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
                    replacement = string.Format(cultureInfo ?? CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
                }
                else
                {
                    replacement = (replacementValue ?? default).ToString();
                }

                if (IsSQL)
                {
                    replacement = Ext.ToSQLString(replacement);
                }

                result = result.Replace(m.ToString(), replacement);
            }

            return result;
        }

        public static string Interpolate(this string Text, params string[] Texts)
        {
            Text = Text.IfBlank(InnerLibs.Ext.EmptyString);
            Texts = Texts ?? Array.Empty<string>();

            var s = Texts.ToList();
            s.Insert(0, Text);

            var ns = @InnerLibs.Ext.EmptyString;
            var len = s.Max(x => x.Length);
            for (int i = 0; i < len; i++)
            {
                foreach (var item in s)
                {
                    ns += item.AsEnumerable().IfNoIndex(i, WhitespaceChar.FirstOrDefault());
                }
            }

            return ns;
        }

        /// <summary>
        /// Verifica se uma palavra é um Anagrama de outra palavra
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="AnotherText"></param>
        /// <returns></returns>
        public static bool IsAnagramOf(this string Text, string AnotherText)
        {
            var char1 = Text?.ToLowerInvariant().ToCharArray() ?? Array.Empty<char>();
            var char2 = AnotherText?.ToLowerInvariant().ToCharArray() ?? Array.Empty<char>();
            Array.Sort(char1);
            Array.Sort(char2);
            string NewWord1 = new string(char1);
            string NewWord2 = new string(char2);
            return NewWord1 == NewWord2;
        }

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, params string[] Texts) => Text.IsAny(default, Texts);

        /// <summary>
        /// Compara se uma string é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsAny(this string Text, StringComparison Comparison, params string[] Texts) => (Texts ?? Array.Empty<string>()).Any(x => Text.Equals(x, Comparison));

        public static bool IsCloseWrapChar(this string Text) => Text.GetFirstChars().IsIn(PredefinedArrays.CloseWrappers);

        public static bool IsCloseWrapChar(this char c) => IsCloseWrapChar($"{c}");

        public static bool IsCrossLikeAny(this string Text, IEnumerable<string> Patterns) => (Patterns ?? Array.Empty<string>()).Any((Func<string, bool>)(x => Ext.Like(Text.IfBlank(InnerLibs.Ext.EmptyString), x) || Ext.Like(x, Text)));

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, IEnumerable<string> Patterns) => (Patterns ?? Array.Empty<string>()).Any((Func<string, bool>)(x => Ext.Like(Text.IfBlank(InnerLibs.Ext.EmptyString), x)));

        /// <summary>
        /// Verifica se um texto existe em uma determinada lista usando comparação com caratere curinga
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Patterns"></param>
        /// <returns></returns>
        public static bool IsLikeAny(this string Text, params string[] Patterns) => Text.IsLikeAny((Patterns ?? Array.Empty<string>()).AsEnumerable());

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se nenhuma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, params string[] Texts) => !Text.IsAny(Texts);

        /// <summary>
        /// Compara se uma string nao é igual a outras strings
        /// </summary>
        /// <param name="Text">string principal</param>
        /// <param name="Texts">strings para comparar</param>
        /// <returns>TRUE se alguma das strings for igual a principal</returns>
        public static bool IsNotAny(this string Text, StringComparison Comparison, params string[] Texts) => !Text.IsAny(Comparison, Texts);

        /// <summary>
        /// Retorna o caractere de encapsulamento oposto ao caractere indicado
        /// </summary>
        /// <param name="Text">Caractere</param>
        /// <returns></returns>
        public static bool IsOpenWrapChar(this string Text) => Text.GetFirstChars().IsIn(PredefinedArrays.OpenWrappers);

        public static bool IsOpenWrapChar(this char c) => IsOpenWrapChar($"{c}");

        /// <summary>
        /// Verifica se uma palavra ou frase é idêntica da direita para a esqueda bem como da
        /// esqueda para direita
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="IgnoreWhiteSpaces">Ignora os espaços na hora de comparar</param>
        /// <returns></returns>
        public static bool IsPalindrome(this string Text, bool IgnoreWhiteSpaces = true)
        {
            Text = Text ?? EmptyString;
            if (IgnoreWhiteSpaces)
            {
                Text = Text.RemoveAny(WhitespaceChar);
            }

            return Text == Text.ToCharArray().Reverse().SelectJoinString();
        }

        public static bool IsWrapped(this string Text) => PredefinedArrays.OpenWrappers.Any(x => IsWrapped(Text, x.FirstOrDefault()));

        public static bool IsWrapped(this string Text, string OpenWrapText, string CloseWrapText = null) => IsWrapped(Text, StringComparison.CurrentCultureIgnoreCase, OpenWrapText, CloseWrapText);

        public static bool IsWrapped(this string Text, StringComparison stringComparison, string OpenWrapText, string CloseWrapText = null)
        {
            if (Text.IsNotBlank())
            {
                OpenWrapText = OpenWrapText.IfBlank("");
                CloseWrapText = CloseWrapText.IfBlank("");
                if (OpenWrapText.Length == 1 && (CloseWrapText.IsBlank() || CloseWrapText.Length == 1))
                {
                    return CloseWrapText.IsBlank()
                        ? IsWrapped(Text, OpenWrapText.FirstOrDefault())
                        : IsWrapped(Text, OpenWrapText.FirstOrDefault(), CloseWrapText.FirstOrDefault());
                }
                else
                    return Text.StartsWith(OpenWrapText, stringComparison) && Text.EndsWith(CloseWrapText, stringComparison);
            }
            return false;
        }

        public static bool IsWrapped(this string Text, char OpenWrapChar) => IsWrapped(Text, OpenWrapChar, OpenWrapChar.GetOppositeWrapChar());

        public static bool IsWrapped(this string Text, char OpenWrapChar, char CloseWrapChar)
        {
            Text = Text?.Trim() ?? "";
            OpenWrapChar = OpenWrapChar.IsCloseWrapChar() ? OpenWrapChar.GetOppositeWrapChar() : OpenWrapChar;
            return Text.FirstOrDefault() == OpenWrapChar && Text.LastOrDefault() == CloseWrapChar;
        }

        /// <summary>
        /// Computa a distancia de Levenshtein entre 2 strings. Distancia Levenshtein representa um
        /// numero de operações de acréscimo, remoção ou substituição de caracteres para que uma
        /// string se torne outra
        /// </summary>
        public static int LevenshteinDistance(this string Text1, string Text2)
        {
            Text1 = Text1 ?? InnerLibs.Ext.EmptyString;
            Text2 = Text2 ?? InnerLibs.Ext.EmptyString;
            int n = Text1.Length;
            int m = Text2.Length;
            var d = new int[n + 1 + 1, m + 1 + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0, loopTo = n; i <= loopTo; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0, loopTo1 = m; j <= loopTo1; j++)
            {
                d[0, j] = j;
            }

            // Step 3
            for (int i = 1, loopTo2 = n; i <= loopTo2; i++)
            {
                // Step 4
                for (int j = 1, loopTo3 = m; j <= loopTo3; j++)
                {
                    // Step 5
                    int cost = Text2[j - 1] == Text1[i - 1] ? 0 : 1;
                    // Step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        /// <summary>
        /// compara 2 strings usando wildcards
        /// </summary>
        /// <param name="source"></param>
        /// <param name="Pattern"></param>
        /// <returns></returns>
        public static bool Like(this string source, string Pattern) => new Like(Pattern).Matches(source);

        /// <summary>
        /// Adciona caracteres ao inicio e final de uma string enquanto o <see
        /// cref="string.Length"/> de <paramref name="Text"/> for menor que <paramref name="TotalLength"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="TotalLength">Tamanho total</param>
        /// <param name="PaddingChar">Caractere</param>
        /// <returns></returns>
        public static string Pad(this string Text, int TotalLength, char PaddingChar = ' ')
        {
            if (Text.Length < TotalLength)
            {
                while (Text.Length < TotalLength)
                {
                    Text = Text.Wrap(PaddingChar.ToString());
                }

                if (Text.Length > TotalLength)
                {
                    Text = Text.RemoveLastChars();
                }
            }

            return Text;
        }

        /// <summary>
        /// limpa um texto deixando apenas os caracteres alfanumericos.
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ParseAlphaNumeric(this string Text)
        {
            var l = new List<string>();
            foreach (var item in Text.Split(WhitespaceChar, StringSplitOptions.RemoveEmptyEntries))
            {
                l.Add(Regex.Replace(item, "[^A-Za-z0-9]", EmptyString));
            }

            return l.SelectJoinString(WhitespaceChar);
        }

        /// <summary>
        /// Parseia uma ConnectionString em um <see cref="ConnectionStringParser"/>
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        public static ConnectionStringParser ParseConnectionString(this string ConnectionString) => new ConnectionStringParser(ConnectionString);

        /// <summary>
        /// Remove caracteres não numéricos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string ParseDigits(this string Text, CultureInfo Culture = null)
        {
            Culture = Culture ?? CultureInfo.CurrentCulture;
            string strDigits = EmptyString;
            if (string.IsNullOrEmpty(Text))
            {
                return strDigits;
            }

            foreach (char c in Text.ToCharArray())
            {
                if (char.IsDigit(c) || c == Convert.ToChar(Culture.NumberFormat.NumberDecimalSeparator))
                {
                    strDigits += $"{c}";
                }
            }

            return strDigits;
        }

        /// <summary>
        /// Remove caracteres não numéricos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static T ParseDigits<T>(this string Text, CultureInfo Culture = null) where T : IConvertible => Text.ParseDigits(Culture).ChangeType<T>();

        /// <summary>
        /// Transforma uma <see cref="string"/> em um <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="QueryString">string contendo uma querystring valida</param>
        /// <param name="Keys">Quando especificado, inclui apenas estas entradas no <see cref="NameValueCollection"/></param>
        /// <returns></returns>
        public static NameValueCollection ParseQueryString(this string QueryString, params string[] Keys)
        {
            if (QueryString.IsURL())
            {
                return ParseQueryString(new Uri(QueryString).Query, Keys);
            }
            else
            {
                Keys = Keys ?? Array.Empty<string>();
                var queryParameters = new NameValueCollection();
                var querySegments = QueryString?.Split('&') ?? Array.Empty<string>();
                foreach (string segment in querySegments)
                {
                    var parts = segment.Split('=');
                    if (parts.Any())
                    {
                        string key = parts.First().TrimStartAny(WhitespaceChar, "?");
                        string val = EmptyString;
                        if (parts.Skip(1).Any())
                        {
                            val = parts[1].Trim().UrlDecode();
                        }
                        if (Keys.Contains(key) || Keys.Any() == false)
                        {
                            queryParameters.Add(key, val);
                        }
                    }
                }

                return queryParameters;
            }
        }

        public static HtmlTag ParseTag(this string HtmlStringOrUrl) => HtmlTag.ParseTag(HtmlStringOrUrl);

        public static IEnumerable<HtmlTag> ParseTags(this string HtmlStringOrUrl) => HtmlTag.Parse(HtmlStringOrUrl);

        /// <summary>
        /// Separa as palavras de um texto CamelCase a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string PascalCaseAdjust(this string Text)
        {
            Text = Text.IfBlank(EmptyString);
            var chars = Text.ToArray();
            Text = EmptyString;
            int uppercount = 0;
            foreach (var c in chars)
            {
                if (char.IsUpper(c))
                {
                    if (!(uppercount > 0))
                    {
                        Text += WhitespaceChar;
                    }

                    uppercount++;
                }
                else
                {
                    if (uppercount > 1)
                    {
                        Text += WhitespaceChar;
                    }

                    uppercount = 0;
                }

                Text += $"{c}";
            }

            return Text.Trim();
        }

        /// <summary>
        /// Transforma um texto em CamelCase em um array de palavras a partir de suas letras maíusculas
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static IEnumerable<string> PascalCaseSplit(this string Text) => Text.PascalCaseAdjust().Split(WhitespaceChar);

        /// <summary>
        /// Retorna uma string em sua forma poop
        /// </summary>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static string[] Poopfy(params string[] Words)
        {
            var p = new List<string>();
            foreach (var Text in Words)
            {
                decimal l = (decimal)(Text.Length / 2d);
                l = l.Floor();
                if (!Text.GetFirstChars((int)Math.Round(l)).Last().ToString().ToLowerInvariant().IsIn(PredefinedArrays.LowerVowels))
                {
                    l = l.ToInt() - 1;
                }

                p.Add(Text.GetFirstChars((int)Math.Round(l)).Trim() + Text.GetFirstChars((int)Math.Round(l)).Reverse().ToList().SelectJoinString().ToLowerInvariant().Trim() + Text.RemoveFirstChars((int)Math.Round(l)).TrimStartAny(PredefinedArrays.LowerConsonants.ToArray()));
            }

            return p.ToArray();
        }

        /// <summary>
        /// Retorna uma string em sua forma poop
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string Poopfy(this string Text) => Poopfy(Text.Split(WhitespaceChar)).SelectJoinString(WhitespaceChar);

        /// <summary>
        /// Return a Idented XML string
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        public static string PreetyPrint(this XmlDocument Document)
        {
            string Result = InnerLibs.Ext.EmptyString;
            var mStream = new MemoryStream();
            var writer = new XmlTextWriter(mStream, Encoding.Unicode);
            try
            {
                writer.Formatting = Formatting.Indented;

                // Write the XML into a formatting XmlTextWriter
                Document.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                // Have to rewind the MemoryStream in order to read its contents.
                mStream.Position = 0L;

                // Read MemoryStream contents into a StreamReader.
                var sReader = new StreamReader(mStream);

                // Extract the text from the StreamReader.
                Result = sReader.ReadToEnd();
            }
            catch (XmlException)
            {
            }
            finally
            {
                mStream?.Close();
                writer?.Close();
                mStream?.Dispose();
                writer?.Dispose();
            }

            return Result;
        }

        /// <summary>
        /// Adiciona texto ao começo de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        public static string Prepend(this string Text, string PrependText)
        {
            Text = Text ?? InnerLibs.Ext.EmptyString;
            PrependText = PrependText ?? InnerLibs.Ext.EmptyString;
            Text = PrependText + Text;
            return Text;
        }

        /// <summary>
        /// Adiciona texto ao final de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependIf(this string Text, string PrependText, Func<string, bool> Test = null)
        {
            Text = Text ?? InnerLibs.Ext.EmptyString;
            PrependText = PrependText ?? InnerLibs.Ext.EmptyString;
            return Text.PrependIf(PrependText, (Test ?? (x => false))(Text));
        }

        /// <summary>
        /// Adiciona texto ao começo de uma string se um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependIf(this string Text, string PrependText, bool Test)
        {
            Text = Text ?? InnerLibs.Ext.EmptyString;
            PrependText = PrependText ?? InnerLibs.Ext.EmptyString;
            return Test ? Text.Prepend(PrependText) : Text;
        }

        /// <summary>
        /// Adiciona texto ao inicio de uma string com uma quebra de linha no final do <paramref name="PrependText"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        public static string PrependLine(this string Text, string PrependText) => Text.Prepend(Environment.NewLine).Prepend(PrependText);

        /// <summary>
        /// Adiciona texto ao inicio de uma string enquanto um criterio for cumprido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="PrependText">Texto adicional</param>
        /// <param name="Test">Teste</param>
        public static string PrependWhile(this string Text, string PrependText, Func<string, bool> Test)
        {
            Test = Test ?? (x => false);

            while (Test(Text))
            {
                Text = Text.Prepend(PrependText);
            }

            return Text;
        }

        /// <summary>
        /// Retorna a string especificada se o valor boolean for verdadeiro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="BooleanValue"></param>
        /// <returns></returns>
        public static string PrintIf(this string Text, bool BooleanValue) => BooleanValue ? Text : InnerLibs.Ext.EmptyString;

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico encontrado no primeiro parametro.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <returns></returns>
        /// <example>texto = $"{2} pães"</example>
        public static string QuantifyText(this FormattableString PluralText)
        {
            if (PluralText.IsNotBlank() && PluralText.ArgumentCount > 0)
            {
                decimal numero = 0m;
                string str = PluralText.Format.QuantifyText(PluralText.GetArguments().FirstOrDefault(), ref numero);
                str = str.Replace("{0}", $"{numero}");
                for (int index = 1, loopTo = PluralText.GetArguments().Length - 1; index <= loopTo; index++)
                {
                    str = str.Replace($"{{{index}}}", $"{PluralText.GetArgument(index)}");
                }

                return str;
            }

            return $"{PluralText}";
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this string PluralText, object Quantity)
        {
            decimal d = 0m;
            return PluralText.QuantifyText(Quantity, ref d);
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com uma quantidade
        /// determinada em uma lista ou um valor numérico.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="QuantityOrListOrBoolean">Quantidade de Itens</param>
        /// <param name="OutQuantity">Devolve a quantidade encontrada em <paramref name="QuantityOrListOrBoolean"/></param>
        /// <returns></returns>
        public static string QuantifyText(this string PluralText, object QuantityOrListOrBoolean, ref decimal OutQuantity)
        {
            switch (true)
            {
                case object _ when QuantityOrListOrBoolean is null:
                    {
                        OutQuantity = 0m;
                        break;
                    }

                case object _ when QuantityOrListOrBoolean.GetType() == typeof(bool):
                    {
                        OutQuantity = Ext.ToDecimal(QuantityOrListOrBoolean);
                        return PluralText.Singularize(); // de acordo com as normas do portugues, quando a quantidade esperada maxima for 1, zero também é singular.
                    }

                case object _ when QuantityOrListOrBoolean.IsNumber():
                    {
                        OutQuantity = Convert.ToDecimal(QuantityOrListOrBoolean);
                        break;
                    }

                case object _ when typeof(IList).IsAssignableFrom(QuantityOrListOrBoolean.GetType()):
                    {
                        OutQuantity = ((IList)QuantityOrListOrBoolean).Count;
                        break;
                    }

                case object _ when typeof(IDictionary).IsAssignableFrom(QuantityOrListOrBoolean.GetType()):
                    {
                        OutQuantity = ((IDictionary)QuantityOrListOrBoolean).Count;
                        break;
                    }

                case object _ when typeof(Array).IsAssignableFrom(QuantityOrListOrBoolean.GetType()):
                    {
                        OutQuantity = ((Array)QuantityOrListOrBoolean).Length;
                        break;
                    }

                default:
                    {
                        OutQuantity = Convert.ToDecimal(QuantityOrListOrBoolean);
                        break;
                    }
            }

            return OutQuantity.Floor() == 1m || OutQuantity.Floor() == -1 ? PluralText.Singularize() : PluralText;
        }

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="List">Lista com itens</param>
        /// <returns></returns>
        public static string QuantifyText<T>(this IEnumerable<T> List, string PluralText) => PluralText.QuantifyText(List ?? Array.Empty<T>());

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this int Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this decimal Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this short Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this long Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Retorna o texto a na sua forma singular ou plural de acordo com um numero determinado.
        /// </summary>
        /// <param name="PluralText">Texto no plural</param>
        /// <param name="Quantity">Quantidade de Itens</param>
        /// <returns></returns>
        public static string QuantifyText(this double Quantity, string PluralText) => PluralText.QuantifyText(Quantity);

        /// <summary>
        /// Encapsula um texto entre 2 caracteres (normalmente parentesis, chaves, aspas ou colchetes)
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="OpenQuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Quote(this string Text, char OpenQuoteChar = '"')
        {
            if (Convert.ToBoolean(OpenQuoteChar.ToString().IsCloseWrapChar()))
            {
                OpenQuoteChar = OpenQuoteChar.GetOppositeWrapChar();
            }

            return $"{OpenQuoteChar}{Text}{OpenQuoteChar.GetOppositeWrapChar()}";
        }

        /// <summary>
        /// Encapsula um tento entre 2 textos (normalmente parentesis, chaves, aspas ou colchetes)
        /// se uma condição for cumprida
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="QuoteChar">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string QuoteIf(this string Text, bool Condition, char QuoteChar = '"') => Condition ? Text.Quote(QuoteChar) : Text;

        /// <summary>
        /// Sorteia um item da Matriz
        /// </summary>
        /// <typeparam name="Type">Tipo da Matriz</typeparam>
        /// <param name="Array">Matriz</param>
        /// <returns>Um valor do tipo especificado</returns>
        public static Type RandomItem<Type>(params Type[] Array) => Array.GetRandomItem();

        public static IEnumerable<string> ReduceToDifference(this IEnumerable<string> Texts, bool FromStart = false, string BreakAt = null) => ReduceToDifference(Texts, out _, FromStart, BreakAt);

        public static IEnumerable<string> ReduceToDifference(this IEnumerable<string> Texts, out string RemovedPart, bool FromStart = false, string BreakAt = null)
        {
            RemovedPart = InnerLibs.Ext.EmptyString;
            Texts = Texts ?? Array.Empty<string>();
            var arr = Texts.WhereNotBlank().ToArray();
            while (arr.Distinct().Count() > 1 && !arr.Any(x => BreakAt.IsNotBlank() && (FromStart ? x.StartsWith(BreakAt) : x.EndsWith(BreakAt))) && arr.All(x => FromStart ? x.StartsWith(arr.FirstOrDefault().GetFirstChars()) : x.EndsWith(arr.FirstOrDefault().GetLastChars())))
            {
                arr = arr.Select(x => FromStart ? x.RemoveFirstChars() : x.RemoveLastChars()).ToArray();
            }

            if (BreakAt.IsNotBlank())
            {
                arr = arr.Select(x => FromStart ? x.TrimStartAny(false, BreakAt) : x.TrimEndAny(false, BreakAt)).ToArray();
                //Difference = FromStart ? Difference.Prepend(BreakAt) : Difference.Append(BreakAt);
            }

            RemovedPart = FromStart ? RemovedPart.Prepend(Texts.FirstOrDefault().TrimEndAny(arr.FirstOrDefault())) : RemovedPart.Append(Texts.FirstOrDefault().TrimStartAny(arr.FirstOrDefault()));

            return arr;
        }

        /// <summary>
        /// Escapa caracteres exclusivos de uma regex
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string RegexEscape(this string Text)
        {
            string newstring = InnerLibs.Ext.EmptyString;
            foreach (var c in Text.ToArray())
            {
                if (c.IsIn(PredefinedArrays.RegexChars))
                {
                    newstring += @"\" + c;
                }
                else
                {
                    newstring += Convert.ToString(c);
                }
            }

            return newstring;
        }

        /// <summary>
        /// Remove os acentos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String sem os acentos</returns>
        public static string RemoveAccents(this string Text)
        {
            if (Text == null)
            {
                return Text;
            }

            string s = Text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            int k = 0;
            while (k < s.Length)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(s[k]);
                }

                k++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Remove várias strings de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Values">Strings a serem removidas</param>
        /// <returns>Uma string com os valores removidos</returns>
        public static string RemoveAny(this string Text, params string[] Values) => Text.ReplaceMany(InnerLibs.Ext.EmptyString, Values ?? Array.Empty<string>());

        public static string RemoveAny(this string Text, params char[] Values) => Text.RemoveAny(Values.Select(x => x.ToString()).ToArray());

        /// <summary>
        /// Remove os acentos de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String sem os acentos</returns>
        public static string RemoveDiacritics(this string Text) => Text.RemoveAccents();

        /// <summary>
        /// Remove os X primeiros caracteres
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Quantity">Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveFirstChars(this string Text, int Quantity = 1) => Text.IsNotBlank() && Text.Length > Quantity && Quantity > 0 ? Text.Remove(0, Quantity) : InnerLibs.Ext.EmptyString;

        public static string RemoveHTML(this string Text)
        {
            if (Text.IsNotBlank())
            {
                return Regex.Replace(Text.ReplaceMany(Environment.NewLine, "<br/>", "<br>", "<br />"), "<.*?>", InnerLibs.Ext.EmptyString).HtmlDecode();
            }

            return Text;
        }

        /// <summary>
        /// Remove os X ultimos caracteres
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Quantity">Quantidade de Caracteres</param>
        /// <returns></returns>
        public static string RemoveLastChars(this string Text, int Quantity = 1) => Text.IsNotBlank() && Text.Length > Quantity && Quantity > 0 ? Text.Substring(0, Text.Length - Quantity) : InnerLibs.Ext.EmptyString;

        public static string RemoveMask(this string MaskedText, params char[] AllowCharacters)
        {
            if (MaskedText.IsNotBlank())
            {
                AllowCharacters = "".ToArray();
                string ns = "";
                foreach (char c in MaskedText)
                {
                    if (c.ToString().IsNumber() || c.IsIn(AllowCharacters))
                    {
                        ns += c;
                    }
                }
                return ns;
            }
            return MaskedText;
        }

        public static int RemoveMaskInt(this string MaskedText, params char[] AllowCharacters) => RemoveMask(MaskedText, AllowCharacters).ToInt();

        public static long RemoveMaskLong(this string MaskedText, params char[] AllowCharacters) => RemoveMask(MaskedText, AllowCharacters).ToLong();

        /// <summary>
        /// Remove caracteres não prantáveis de uma string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>String corrigida</returns>
        public static string RemoveNonPrintable(this string Text)
        {
            foreach (char c in Text.ToCharArray())
            {
                if (char.IsControl(c))
                {
                    Text = Text.ReplaceNone(Convert.ToString(c));
                }
            }

            return Text.Trim();
        }

        /// <summary>
        /// Repete uma string um numero determinado de vezes
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public static string Repeat(this string Text, int Times = 2)
        {
            var ns = InnerLibs.Ext.EmptyString;
            while (Times > 0)
            {
                ns += Text;
                Times--;
            }
            return ns;
        }

        /// <summary>
        /// Faz uma busca em todos os elementos do array e aplica um ReplaceFrom comum
        /// </summary>
        /// <param name="Strings">Array de strings</param>
        /// <param name="OldValue">Valor antigo que será substituido</param>
        /// <param name="NewValue">Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE
        /// realiza um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
        /// </param>
        /// <returns></returns>
        public static string[] Replace(this string[] Strings, string OldValue, string NewValue, bool ReplaceIfEquals = true)
        {
            var NewArray = Strings;
            for (int index = 0, loopTo = Strings.Length - 1; index <= loopTo; index++)
            {
                if (ReplaceIfEquals)
                {
                    if ((NewArray[index] ?? InnerLibs.Ext.EmptyString) == (OldValue ?? InnerLibs.Ext.EmptyString))
                    {
                        NewArray[index] = NewValue;
                    }
                }
                else
                {
                    NewArray[index] = NewArray[index].Replace(OldValue, NewValue);
                }
            }

            return NewArray;
        }

        /// <summary>
        /// Faz uma busca em todos os elementos de uma lista e aplica um ReplaceFrom comum
        /// </summary>
        /// <param name="Strings">Array de strings</param>
        /// <param name="OldValue">Valor antigo que será substituido</param>
        /// <param name="NewValue">Valor utilizado para substituir o valor antigo</param>
        /// <param name="ReplaceIfEquals">
        /// Se TRUE, realiza o replace se o valor no array for idêntico ao Valor antigo, se FALSE
        /// realiza um ReplaceFrom em quaisquer valores antigos encontrados dentro do valor do array
        /// </param>
        /// <returns></returns>
        public static IEnumerable<string> Replace(this IEnumerable<string> Strings, string OldValue, string NewValue, bool ReplaceIfEquals = true) => Strings.ToArray().Replace(OldValue, NewValue, ReplaceIfEquals).ToList();

        /// <summary>
        /// Substitui a primeira ocorrencia de um texto por outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OldText"></param>
        /// <param name="NewText"></param>
        /// <returns></returns>
        public static string ReplaceFirst(this string Text, string OldText, string NewText = EmptyString)
        {
            if (Text.Contains(OldText))
            {
                Text = Text.Insert(Text.IndexOf(OldText), NewText);
                Text = Text.Remove(Text.IndexOf(OldText), 1);
            }

            return Text;
        }

        /// <summary>
        /// Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string, string> Dic)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    Text = Text.Replace(p.Key, p.Value);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica varios replaces a um texto a partir de um <see cref="IDictionary"/>
        /// </summary>
        public static string ReplaceFrom<T>(this string Text, IDictionary<string, T> Dic)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    switch (true)
                    {
                        case object _ when p.Value.IsDictionary():
                            {
                                Text = Text.ReplaceFrom((IDictionary<string, object>)p.Value);
                                break;
                            }

                        case object _ when typeof(T).IsAssignableFrom(typeof(Array)):
                            {
                                foreach (var item in Ext.ForceArray(p.Value, typeof(T)))
                                {
                                    Text = Text.ReplaceMany(p.Key, Ext.ForceArray(p.Value, typeof(T)).Cast<string>().ToArray());
                                }

                                break;
                            }

                        default:
                            {
                                Text = Text.Replace(p.Key, p.Value.ToString());
                                break;
                            }
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string, string[]> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    Text = Text.SensitiveReplace(p.Key, p.Value, Comparison);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string[], string> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    Text = Text.SensitiveReplace(p.Value, p.Key.ToArray(), Comparison);
                }
            }

            return Text;
        }

        /// <summary>
        /// Aplica um replace a um texto baseando-se em um <see cref="IDictionary"/>.
        /// </summary>
        public static string ReplaceFrom(this string Text, IDictionary<string[], string[]> Dic, StringComparison Comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Dic != null && Text.IsNotBlank())
            {
                foreach (var p in Dic)
                {
                    var froms = p.Key.ToList();
                    var tos = p.Value.ToList();
                    while (froms.Count > tos.Count)
                    {
                        tos.Add(InnerLibs.Ext.EmptyString);
                    }

                    for (int i = 0, loopTo = froms.Count - 1; i <= loopTo; i++)
                    {
                        Text = Text.SensitiveReplace(froms[i], tos[i], Comparison);
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Substitui a ultima ocorrencia de um texto por outro
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="OldText"></param>
        /// <param name="NewText"></param>
        /// <returns></returns>
        public static string ReplaceLast(this string Text, string OldText, string NewText = EmptyString)
        {
            if (Text != null)
                if (Text.Contains(OldText))
                {
                    Text = Text.Insert(Text.LastIndexOf(OldText), NewText);
                    Text = Text.Remove(Text.LastIndexOf(OldText), 1);
                }

            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por um novo valor.
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="NewValue">Novo Valor</param>
        /// <param name="OldValues">Valores a serem substituido por um novo valor</param>
        /// <returns></returns>
        public static string ReplaceMany(this string Text, string NewValue, params string[] OldValues)
        {
            Text = Text ?? InnerLibs.Ext.EmptyString;
            foreach (var word in (OldValues ?? Array.Empty<string>()).Where(x => x.Length > 0))
            {
                Text = Text.Replace(word, NewValue);
            }

            return Text;
        }

        /// <summary>
        /// Retorna uma nova sequência na qual todas as ocorrências de uma String especificada são
        /// substituídas por vazio.
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="OldValue">Valor a ser substituido por vazio</param>
        /// <returns>String corrigida</returns>
        public static string ReplaceNone(this string Text, string OldValue) => Text.Replace(OldValue, InnerLibs.Ext.EmptyString);

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="Array">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string SelectJoinString<Type>(string Separator, params Type[] Array) => Array.SelectJoinString(Separator);

        /// <summary>
        /// Une todos os valores de um objeto em uma unica string
        /// </summary>
        /// <param name="List">Objeto com os valores</param>
        /// <param name="Separator">Separador entre as strings</param>
        /// <returns>string</returns>
        public static string SelectJoinString<Type>(this List<Type> List, string Separator = EmptyString) => List.ToArray().SelectJoinString(Separator);

        public static IEnumerable<String> SelectLike(this IEnumerable<String> source, String Pattern) => source.Where(x => x.Like(Pattern));

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="NewValue"></param>
        /// <param name="OldValue"></param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string OldValue, string NewValue, StringComparison ComparisonType = StringComparison.InvariantCulture) => Text.SensitiveReplace(NewValue, new[] { OldValue }, ComparisonType);

        /// <summary>
        /// Realiza um replace em uma string usando um tipo especifico de comparacao
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="NewValue"></param>
        /// <param name="OldValues"></param>
        /// <param name="ComparisonType"></param>
        /// <returns></returns>
        public static string SensitiveReplace(this string Text, string NewValue, IEnumerable<string> OldValues, StringComparison ComparisonType = StringComparison.InvariantCulture)
        {
            if (Text.IsNotBlank())
            {
                foreach (var oldvalue in OldValues ?? new[] { InnerLibs.Ext.EmptyString })
                {
                    NewValue = NewValue ?? InnerLibs.Ext.EmptyString;
                    if (!oldvalue.Equals(NewValue, ComparisonType))
                    {
                        int foundAt;
                        do
                        {
                            foundAt = Text.IndexOf(oldvalue, 0, ComparisonType);
                            if (foundAt > -1)
                            {
                                Text = Text.Remove(foundAt, oldvalue.Length).Insert(foundAt, NewValue);
                            }
                        }
                        while (foundAt != -1);
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Randomiza a ordem dos itens de um Array
        /// </summary>
        /// <typeparam name="Type">Tipo do Array</typeparam>
        /// <param name="Array">Matriz</param>
        public static Type[] Shuffle<Type>(this Type[] Array) => Array.OrderByRandom().ToArray();

        /// <summary>
        /// Randomiza a ordem dos itens de uma Lista
        /// </summary>
        /// <typeparam name="Type">Tipo de Lista</typeparam>
        /// <param name="List">Matriz</param>
        public static List<Type> Shuffle<Type>(this List<Type> List) => List.OrderByRandom().ToList();

        /// <summary>
        /// Aleatoriza a ordem das letras de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string Shuffle(this string Text) => Text.OrderByRandom().SelectJoinString();

        /// <summary>
        /// Retorna a frase ou termo especificado em sua forma singular
        /// </summary>
        /// <param name="Text">Texto no plural</param>
        /// <returns></returns>
        public static string Singularize(this string Text)
        {
            var phrase = Text.ApplySpaceOnWrapChars().Split(WhitespaceChar);
            for (int index = 0, loopTo = phrase.Count() - 1; index <= loopTo; index++)
            {
                string endchar = phrase[index].GetLastChars();
                if (endchar.IsAny(StringComparison.CurrentCultureIgnoreCase, PredefinedArrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index].TrimLastEqual(endchar);
                }

                switch (true)
                {
                    case object _ when phrase[index].IsNumber() || phrase[index].IsEmail() || phrase[index].IsURL() || phrase[index].IsIP() || phrase[index].IsIn(PredefinedArrays.WordSplitters):
                        {
                            // nao alterar estes tipos
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ões"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("ões") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ãos"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("ãos") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ães"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("ães") + "ão";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("ais"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("ais") + "al";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("eis"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("eis") + "il";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("éis"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("éis") + "el";

                            break;
                        }

                    case object _ when phrase[index].EndsWith("ois"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("ois") + "ol";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("uis"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("uis") + "ul";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("es"):
                        {
                            if (phrase[index].TrimLastEqual("es").EndsWithAny("z", "r"))
                            {
                                phrase[index] = phrase[index].TrimLastEqual("es");
                            }
                            else
                            {
                                phrase[index] = phrase[index].TrimLastEqual("s");
                            }

                            break;
                        }

                    case object _ when phrase[index].EndsWith("ns"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("ns") + "m";
                            break;
                        }

                    case object _ when phrase[index].EndsWith("s"):
                        {
                            phrase[index] = phrase[index].TrimLastEqual("s");
                            break;
                        }

                    default:
                        {
                            break;
                        }
                        // ja esta no singular
                }

                if (endchar.IsAny(StringComparison.CurrentCultureIgnoreCase, PredefinedArrays.WordSplitters.ToArray()))
                {
                    phrase[index] = phrase[index] + endchar;
                }
            }

            return phrase.SelectJoinString(WhitespaceChar).TrimBetween();
        }

        /// <summary>
        /// Separa um texto em um array de strings a partir de uma outra string
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="Separator">Texto utilizado como separador</param>
        /// <returns></returns>
        public static string[] Split(this string Text, string Separator, StringSplitOptions Options = StringSplitOptions.RemoveEmptyEntries) => (Text ?? InnerLibs.Ext.EmptyString).Split(new[] { Separator }, Options);

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, params string[] SplitText) => Text?.SplitAny(StringSplitOptions.RemoveEmptyEntries, SplitText);

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, StringSplitOptions SplitOptions, params string[] SplitText) => Text?.Split(SplitText ?? Array.Empty<string>(), SplitOptions);

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, IEnumerable<string> SplitText) => Text?.SplitAny(SplitText.ToArray());

        /// <summary>
        /// Separa uma string em varias partes a partir de varias strings removendo as entradas em branco
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SplitText"></param>
        /// <returns></returns>
        public static string[] SplitAny(this string Text, StringSplitOptions SplitOptions, IEnumerable<string> SplitText) => Text?.SplitAny(SplitOptions, SplitText.ToArray());

        /// <summary>
        /// Verifica se uma string começa com alguma outra string de um array
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Words"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string Text, StringComparison comparison, params string[] Words) => Words.Any(p => Text.IfBlank(InnerLibs.Ext.EmptyString).StartsWith(p, comparison));

        public static bool StartsWithAny(this string Text, params string[] Words) => StartsWithAny(Text, StringComparison.InvariantCultureIgnoreCase, Words);

        /// <summary>
        /// Alterna maiusculas e minusculas para cada letra de uma string
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToAlternateCase(this string Text)
        {
            var ch = Text.ToArray();
            for (int index = 0, loopTo = ch.Length - 1; index <= loopTo; index++)
            {
                char antec = ch.IfNoIndex(index - 1, '\0');
                if (antec.ToString().IsBlank() || char.IsLower(antec) || antec.ToString() == null)
                {
                    ch[index] = char.ToUpper(ch[index]);
                }
                else
                {
                    ch[index] = char.ToLower(ch[index]);
                }
            }

            return new string(ch);
        }

        /// <summary>
        /// Retorna um anagrama de um texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string ToAnagram(this string Text) => Shuffle(Text);

        public static IEnumerable<int> ToAsc(this string c) => c.ToArray().Select(x => x.ToAsc());

        public static int ToAsc(this char c)
        {
            int converted = c;
            if (converted >= 0x80)
            {
                byte[] buffer = new byte[2];
                // if the resulting conversion is 1 byte in length, just use the value
                if (Encoding.Default.GetBytes(new char[] { c }, 0, 1, buffer, 0) == 1)
                {
                    converted = buffer[0];
                }
                else
                {
                    // byte swap bytes 1 and 2;
                    converted = buffer[0] << 16 | buffer[1];
                }
            }
            return converted;
        }

        public static byte ToAscByte(this char c) => (byte)c.ToAsc();

        /// <summary>
        /// Returns a CSV String from <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="Separator"></param>
        /// <param name="IncludeHeader"></param>
        /// <returns></returns>
        public static string ToCSV(this IEnumerable<Dictionary<string, object>> Items, string Separator = ",", bool IncludeHeader = false)
        {
            Separator = Separator.IfBlank(",");
            var str = $"sep={Separator}{Environment.NewLine}";
            if (Items != null && Items.Any())
            {
                Items = Items.MergeKeys();

                if (IncludeHeader && Items.All(x => x.Keys.Any()))
                {
                    str += $"{Items.FirstOrDefault()?.Keys.SelectJoinString(Separator)}";
                }
                str += $"{Items.SelectJoinString(x => x.Values.SelectJoinString(Separator), Environment.NewLine)}";
            }

            return str;
        }

        public static string ToCSV<T>(this IEnumerable<T> Items, string Separator = ",", bool IncludeHeader = false) where T : class => (Items ?? Array.Empty<T>()).Select(x => x.CreateDictionary()).ToCSV(Separator, IncludeHeader);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this byte[] Size, int DecimalPlaces = -1) => Size.LongLength.ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this FileInfo Size, int DecimalPlaces = -1) => Size.Length.ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this double Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this int Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB, GB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this long Size, int DecimalPlaces = -1) => Size.ToDecimal().ToFileSizeString(DecimalPlaces);

        /// <summary>
        /// Retorna o uma string representando um valor em bytes, KB, MB ou TB
        /// </summary>
        /// <param name="Size">Tamanho</param>
        /// <returns>String com o tamanho + unidade de medida</returns>
        public static string ToFileSizeString(this decimal Size, int DecimalPlaces = -1) => UnitConverter.CreateFileSizeConverter().Abreviate(Size, DecimalPlaces);

        public static FormattableString ToFormattableString(this string Text, params object[] args) => FormattableStringFactory.Create(Text, args ?? Array.Empty<object>());

        public static FormattableString ToFormattableString<T>(IEnumerable<T> args, string Text) => ToFormattableString(Text, args);

        public static FormattableString ToFormattableString(this string Text, IEnumerable<object[]> args) => ToFormattableString(Text, args);

        /// <summary>
        /// Prepara uma string para se tornar uma caminho amigavel (remove caracteres nao permitidos)
        /// </summary>
        /// <param name="Text"></param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyPathName(this string Text) => Text.RemoveAny(Path.GetInvalidPathChars()).TrimBetween();

        /// <summary>
        /// Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e
        /// troca espacos por hifen)
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="UseUnderscore">
        /// Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
        /// </param>
        /// <returns>string amigavel para URL</returns>
        public static string ToFriendlyURL(this string Text, bool UseUnderscore = false) => Text.ReplaceMany(UseUnderscore ? "_" : "-", "_", "-", WhitespaceChar).RemoveAny("(", ")", ".", ",", "#").ToFriendlyPathName().RemoveAccents().ToLowerInvariant();

        /// <summary>
        /// Converte um texto para Leet (1337)
        /// </summary>
        /// <param name="text">TExto original</param>
        /// <param name="degree">Grau de itensidade (0 a 7)</param>
        /// <returns>Texto em 1337</returns>
        public static string ToLeet(this string Text, int Degree = 7)
        {
            // Adjust degree between 0 - 100
            Degree = Degree.LimitRange(0, 7);
            // No Leet Translator
            if (Degree == 0)
            {
                return Text;
            }
            // StringBuilder to store result.
            var sb = new StringBuilder();
            foreach (char c in Text.AsEnumerable())
            {
                switch (Degree)
                {
                    case 1:
                        switch (c)
                        {
                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 2:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 3:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 4:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 5:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('6');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    case 6:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('6');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'r':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'R':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'b':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'B':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'q':
                                {
                                    sb.Append('Q');
                                    break;
                                }

                            case 'Q':
                                {
                                    sb.Append("Q¸");
                                    break;
                                }

                            case 'x':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            case 'X':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;

                    default:
                        switch (c)
                        {
                            case 'a':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'e':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'i':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'o':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 'A':
                                {
                                    sb.Append('4');
                                    break;
                                }

                            case 'E':
                                {
                                    sb.Append('3');
                                    break;
                                }

                            case 'I':
                                {
                                    sb.Append('1');
                                    break;
                                }

                            case 'O':
                                {
                                    sb.Append('0');
                                    break;
                                }

                            case 's':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'S':
                                {
                                    sb.Append('$');
                                    break;
                                }

                            case 'g':
                                {
                                    sb.Append('9');
                                    break;
                                }

                            case 'G':
                                {
                                    sb.Append('6');
                                    break;
                                }

                            case 'l':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'L':
                                {
                                    sb.Append('£');
                                    break;
                                }

                            case 'c':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 'C':
                                {
                                    sb.Append('(');
                                    break;
                                }

                            case 't':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'T':
                                {
                                    sb.Append('7');
                                    break;
                                }

                            case 'z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'Z':
                                {
                                    sb.Append('2');
                                    break;
                                }

                            case 'y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'Y':
                                {
                                    sb.Append('¥');
                                    break;
                                }

                            case 'U':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'u':
                                {
                                    sb.Append('µ');
                                    break;
                                }

                            case 'f':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'F':
                                {
                                    sb.Append('ƒ');
                                    break;
                                }

                            case 'd':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'D':
                                {
                                    sb.Append('Ð');
                                    break;
                                }

                            case 'n':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'N':
                                {
                                    sb.Append(@"|\|");
                                    break;
                                }

                            case 'w':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'W':
                                {
                                    sb.Append(@"\/\/");
                                    break;
                                }

                            case 'h':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'H':
                                {
                                    sb.Append("|-|");
                                    break;
                                }

                            case 'v':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'V':
                                {
                                    sb.Append(@"\/");
                                    break;
                                }

                            case 'k':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'K':
                                {
                                    sb.Append("|{");
                                    break;
                                }

                            case 'r':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'R':
                                {
                                    sb.Append('®');
                                    break;
                                }

                            case 'm':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'M':
                                {
                                    sb.Append(@"|\/|");
                                    break;
                                }

                            case 'b':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'B':
                                {
                                    sb.Append('ß');
                                    break;
                                }

                            case 'j':
                                {
                                    sb.Append("_|");
                                    break;
                                }

                            case 'J':
                                {
                                    sb.Append("_|");
                                    break;
                                }

                            case 'P':
                                {
                                    sb.Append("|°");
                                    break;
                                }

                            case 'q':
                                {
                                    sb.Append('¶');
                                    break;
                                }

                            case 'Q':
                                {
                                    sb.Append("¶¸");
                                    break;
                                }

                            case 'x':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            case 'X':
                                {
                                    sb.Append(")(");
                                    break;
                                }

                            default:
                                {
                                    sb.Append(c);
                                    break;
                                }
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Pega um texto em "PascalCase" ou "snake_case" e o retorna na forma "normal case"
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToNormalCase(this string Text) => Text.Replace("_", WhitespaceChar).PascalCaseAdjust();

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this decimal Number, int Decimals = -1)
        {
            if (Decimals > -1)
            {
                Number = Number.RoundDecimal(Decimals);
            }

            return $"{Number}%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this int Number) => $"{Number}%";

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this double Number, int Decimals = -1)
        {
            if (Decimals > -1)
            {
                Number = Number.RoundDouble(Decimals);
            }

            return $"{Number}%";
        }

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this short Number) => $"{Number}%";

        /// <summary>
        /// Retorna um numero com o sinal de porcentagem
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public static string ToPercentString(this long Number) => $"{Number}%";

        /// <summary>
        /// Concatena todos os itens de uma lista, utilizando a palavra <paramref name="And"/> antes
        /// da ultima ocorrencia.
        /// </summary>
        /// <param name="Texts">
        /// Lista com itens que serão convertidos em <see cref="string"/> e concatenados
        /// </param>
        /// <param name="And">
        /// palavra correspondente ao "e", utilizada para concatena ro ultimo elemento da lista.
        /// Quando null ou branco, <paramref name="Separator"/> é utilizado em seu lugar.
        /// </param>
        /// <param name="Separator">caractere correspondente a virgula</param>
        /// <param name="EmptyValue">
        /// Valor que será apresentado caso <paramref name="Texts"/> esteja vazio ou nulo. Quando
        /// <see cref="null"/>, omite o <paramref name="PhraseStart"/> da string final
        /// </param>
        /// <returns></returns>
        public static string ToPhrase<TSource>(this IEnumerable<TSource> Texts, string PhraseStart = EmptyString, string And = "and", string EmptyValue = null, char Separator = ',')
        {
            Separator = Separator.IfBlank(',');
            PhraseStart = PhraseStart.IfBlank(Ext.EmptyString);

            Texts = (Texts ?? Array.Empty<TSource>()).WhereNotBlank();

            if (PhraseStart.IsNotBlank() && !PhraseStart.EndsWith(WhitespaceChar, StringComparison.InvariantCultureIgnoreCase))
            {
                PhraseStart += WhitespaceChar;
            }

            switch (Texts.Count())
            {
                case 0:
                    if (EmptyValue != null)
                    {
                        PhraseStart += EmptyValue;
                    }
                    else
                    {
                        PhraseStart = Ext.EmptyString;
                    }
                    break;

                case 1:
                    PhraseStart += $"{Texts.FirstOrDefault()}";
                    break;

                default:
                    PhraseStart += Texts.SkipLast().SelectJoinString($"{Separator} ");
                    PhraseStart += $" {And.IfBlank($"{Separator}")}";
                    PhraseStart += $" {Texts.Last()}";
                    break;
            }

            return PhraseStart;
        }

        ///<inheritdoc cref="ToPhrase{TSource}(IEnumerable{TSource}, string, string, string, char)"/>
        public static string ToPhrase(string And, params string[] Texts) => (Texts ?? Array.Empty<string>()).ToPhrase(InnerLibs.Ext.EmptyString, And);

        /// <summary>
        /// Coloca o texto em TitleCase
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToProperCase(this string Text, bool ForceCase = false)
        {
            if (Text.IsBlank())
            {
                return Text;
            }

            if (ForceCase)
            {
                Text = Text.ToLowerInvariant();
            }

            var l = Text.Split(WhitespaceChar, StringSplitOptions.None).ToList();
            for (int index = 0, loopTo = l.Count - 1; index <= loopTo; index++)
            {
                string pal = l[index];
                bool artigo = index > 0 && Ext.IsIn(pal, "o", "a", "os", "as", "um", "uma", "uns", "umas", "de", "do", "dos", "das", "e", "ou");
                if (pal.IsNotBlank())
                {
                    if (ForceCase || artigo == false)
                    {
                        char c = pal.First();
                        if (!char.IsUpper(c))
                        {
                            pal = char.ToUpper(c, CultureInfo.InvariantCulture) + pal.RemoveFirstChars(1);
                        }

                        l[index] = pal;
                    }
                }
            }

            return l.SelectJoinString(WhitespaceChar);
        }

        /// <summary>
        /// Coloca a string em Randomcase (aleatoriamente letras maiusculas ou minusculas)
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Times">Numero de vezes que serão sorteados caracteres.</param>
        /// <returns></returns>
        public static string ToRandomCase(this string Text, int Times = 0)
        {
            var ch = Text.ToArray();
            Times = Times.SetMinValue(ch.Length);
            for (int index = 1, loopTo = Times; index <= loopTo; index++)
            {
                int newindex = Ext.RandomNumber(0, ch.Length - 1);
                if (char.IsUpper(ch[newindex]))
                {
                    ch[newindex] = char.ToLower(ch[newindex], CultureInfo.InvariantCulture);
                }
                else
                {
                    ch[newindex] = char.ToUpper(ch[newindex], CultureInfo.InvariantCulture);
                }
            }

            return new string(ch);
        }

        /// <summary>
        /// Prepara uma string para se tornar uma URL amigavel (remove caracteres nao permitidos e
        /// troca espacos por hifen). É um alias para <see cref="ToFriendlyURL(String, Boolean)"/>
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="UseUnderscore">
        /// Indica se os espacos serão substituidos por underscores (underline). Use FALSE para hifens
        /// </param>
        /// <returns>string amigavel para URL</returns>
        public static string ToSlugCase(this string Text, bool UseUnderscore = false) => Text.ToFriendlyURL(UseUnderscore);

        /// <summary>
        /// Retorna uma string em Snake_Case
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string Text) => Text.Replace(WhitespaceChar, "_").ToLowerInvariant();

        /// <summary>
        /// Cria um <see cref="Stream"/> a partir de uma string
        /// </summary>
        /// <param name="TExt"></param>
        /// <returns></returns>
        public static Stream ToStream(this string Text)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(Text);
            writer.Flush();
            stream.Position = 0L;
            return stream;
        }

        /// <summary>
        /// Transforma um texto em titulo
        /// </summary>
        /// <param name="Text">Texto a ser manipulado</param>
        /// <param name="ForceCase">
        /// Se FALSE, apenas altera o primeiro caractere de cada palavra como UPPERCASE, dexando os
        /// demais intactos. Se TRUE, força o primeiro caractere de casa palavra como UPPERCASE e os
        /// demais como LOWERCASE
        /// </param>
        /// <returns>Uma String com o texto em nome próprio</returns>
        public static string ToTitle(this string Text, bool ForceCase = false) => Text?.ToProperCase(ForceCase);

        /// <summary>
        /// Transforma um XML Document em string
        /// </summary>
        /// <param name="XML">Documento XML</param>
        /// <returns></returns>
        public static string ToXMLString(this XmlDocument XML)
        {
            using (var stringWriter = new StringWriter())
            using (var xmlTextWriter = XmlWriter.Create(stringWriter))
            {
                XML.WriteTo(xmlTextWriter);
                xmlTextWriter.Flush();
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="StringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, bool ContinuouslyRemove, params string[] StringTest)
        {
            if (Text.IsNotBlank())
            {
                Text = Text.TrimStartAny(ContinuouslyRemove, StringTest);
                Text = Text.TrimEndAny(ContinuouslyRemove, StringTest);
            }

            return Text;
        }

        /// <summary>
        /// Remove do começo e do final de uma string qualquer valor que estiver no conjunto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimAny(this string Text, params string[] StringTest) => Text.TrimAny(true, StringTest);

        public static IEnumerable<string> TrimBetween(this IEnumerable<string> Texts) => Texts.Select(x => x.TrimBetween());

        public static string TrimBetween(this string Text)
        {
            Text = Text.IfBlank(EmptyString);
            if (Text.IsNotBlank())
            {
                var arr = Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                Text = arr.SelectJoinString(Environment.NewLine);
                arr = Text.Split(new string[] { WhitespaceChar }, StringSplitOptions.RemoveEmptyEntries);
                Text = arr.SelectJoinString(WhitespaceChar);
                Text = Text.TrimAny(WhitespaceChar, Environment.NewLine).Trim();
            }

            return Text;
        }

        /// <summary>
        /// Remove continuamente caracteres em branco do começo e fim de uma string incluindo breaklines
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static string TrimCarriage(this string Text) => Text.TrimAny(PredefinedArrays.InvisibleChars.ToArray());

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, bool ContinuouslyRemove, StringComparison comparison, params string[] EndStringTest)
        {
            Text = Text ?? "";
            EndStringTest = EndStringTest ?? Array.Empty<string>();
            while (Text.EndsWithAny(comparison, EndStringTest))
            {
                foreach (var item in EndStringTest)
                {
                    if (Text.EndsWith(item, comparison))
                    {
                        Text = Text.TrimLastEqual(item, comparison);
                        if (!ContinuouslyRemove)
                        {
                            return Text;
                        }
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, params string[] EndStringTest) => Text.TrimEndAny(true, default, EndStringTest);

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <param name="ContinuouslyRemove">Remove continuamente as strings</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, bool ContinuouslyRemove, params string[] EndStringTest) => Text.TrimEndAny(ContinuouslyRemove, default, EndStringTest);

        /// <summary>
        /// Remove continuamente o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimEndAny(this string Text, StringComparison comparison, params string[] EndStringTest) => Text.TrimEndAny(true, comparison, EndStringTest);

        /// <summary>
        /// Remove um texto do inicio de uma string se ele for um outro texto especificado
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Texto inicial que será comparado</param>
        public static string TrimFirstEqual(this string Text, string StartStringTest, StringComparison comparison = default)
        {
            Text = Text ?? "";
            StartStringTest = StartStringTest ?? "";
            if (Text.StartsWith(StartStringTest, comparison))
            {
                Text = Text.RemoveFirstChars(StartStringTest.Length);
            }

            return Text;
        }

        /// <summary>
        /// Remove um texto do final de uma string se ele for um outro texto
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="EndStringTest">Texto final que será comparado</param>
        public static string TrimLastEqual(this string Text, string EndStringTest, StringComparison comparison = default)
        {
            Text = Text ?? "";
            EndStringTest = EndStringTest ?? "";
            if (Text.EndsWith(EndStringTest, comparison))
            {
                Text = Text.RemoveLastChars(EndStringTest.Length);
            }

            return Text;
        }

        /// <summary>
        /// Remove o final de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="ContinuouslyRemove">
        /// Parametro que indica se a string deve continuar sendo testada até que todas as
        /// ocorrencias sejam removidas
        /// </param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, bool ContinuouslyRemove, StringComparison comparison, params string[] StartStringTest)
        {
            Text = Text ?? "";
            StartStringTest = StartStringTest ?? Array.Empty<string>();
            while (Text.StartsWithAny(comparison, StartStringTest))
            {
                foreach (var item in StartStringTest)
                {
                    if (Text.StartsWith(item, comparison))
                    {
                        Text = Text.TrimFirstEqual(item, comparison);
                        if (!ContinuouslyRemove)
                        {
                            return Text;
                        }
                    }
                }
            }

            return Text;
        }

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, StringComparison comparison, params string[] StartStringTest) => Text.TrimStartAny(true, comparison, StartStringTest);

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, params string[] StartStringTest) => Text.TrimStartAny(true, default, StartStringTest);

        /// <summary>
        /// Remove continuamente o começo de uma string se ela for igual a qualquer um dos valores correspondentes
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="StartStringTest">Conjunto de textos que serão comparados</param>
        /// <returns></returns>
        public static string TrimStartAny(this string Text, bool ContinuouslyRemove, params string[] StartStringTest) => Text.TrimStartAny(ContinuouslyRemove, default, StartStringTest);

        public static string UnBrackfy(this string Text) => Text.UnBrackfy('{', true);

        public static string UnBrackfy(this string Text, char OpenBracketChar, bool ContinuouslyRemove = false) => Text.UnQuote(OpenBracketChar, ContinuouslyRemove);

        public static string UnQuote(this string Text) => Text.UnQuote(char.MinValue, true);

        public static string UnQuote(this string Text, char OpenQuoteChar, bool ContinuouslyRemove = false)
        {
            if ($"{OpenQuoteChar}".RemoveNonPrintable().IsBlank())
            {
                while (Text.EndsWithAny(PredefinedArrays.CloseWrappers.ToArray()) || Text.StartsWithAny(PredefinedArrays.OpenWrappers.ToArray()))
                {
                    Text = Text.TrimAny(ContinuouslyRemove, PredefinedArrays.WordWrappers.ToArray());
                }
            }
            else
            {
                if (OpenQuoteChar.ToString().IsCloseWrapChar())
                {
                    OpenQuoteChar = OpenQuoteChar.ToString().GetOppositeWrapChar().FirstOrDefault();
                }

                Text = Text.TrimAny(ContinuouslyRemove, $"{OpenQuoteChar}", OpenQuoteChar.ToString().GetOppositeWrapChar());
            }

            return Text;
        }

        public static string UnWrap(this string Text, string WrapText = DoubleQuoteChar, bool ContinuouslyRemove = false) => Text.TrimAny(ContinuouslyRemove, WrapText);

        /// <summary>
        /// Decoda uma string de uma transmissão por URL
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string UrlDecode(this string Text) => Text.IsNotBlank() ? System.Net.WebUtility.UrlDecode(Text) : EmptyString;

        /// <summary>
        /// Encoda uma string para transmissão por URL
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string UrlEncode(this string Text) => Text.IsNotBlank() ? System.Net.WebUtility.UrlEncode(Text) : EmptyString;

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <param name="WrapText">Caractere de encapsulamento</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string WrapText = DoubleQuoteChar) => Text.Wrap(WrapText, WrapText);

        #region Private Fields

        private const int ERROR_LOCK_VIOLATION = 33;

        private const int ERROR_SHARING_VIOLATION = 32;

        private static readonly Expression<Func<string, bool>>[] passwordValidations = new Expression<Func<string, bool>>[]
        {
            x => x.ToUpperInvariant().ToArray().Distinct().Count() >= 4,
            x => x.ToUpperInvariant().ToArray().Distinct().Count() >= 6,
            x => x.Length >= 8,
            x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.PasswordSpecialChars.ToArray()),
            x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.NumberChars.ToArray()),
            x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.AlphaUpperChars.ToArray()),
            x => x.ContainsAny(StringComparison.InvariantCulture, PredefinedArrays.AlphaLowerChars.ToArray())
        };

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Verifica se o valor é um numero ou pode ser convertido em numero
        /// </summary>
        /// <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
        /// <returns>TRUE se for um numero, FALSE se não for um numero</returns>
        public static bool CanBeNumber(this object Value)
        {
            try
            {
                Convert.ToDecimal(Value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Check the strength of given password
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static PasswordLevel CheckPassword(this string Password)
        {
            var points = Password.ValidateCount(passwordValidations);
            if (points < PasswordLevel.VeryWeak.ToInt())
            {
                return PasswordLevel.VeryWeak;
            }
            else if (points > PasswordLevel.VeryStrong.ToInt())
            {
                return PasswordLevel.VeryStrong;
            }
            else
            {
                return (PasswordLevel)points;
            }
        }

        /// <summary>
        /// Verifica se uma variavel está vazia, em branco ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValueIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static T IfBlank<T>(this object Value, T ValueIfBlank = default) => Value.IsBlank() ? ValueIfBlank : Ext.ChangeType<T>(Value);

        /// <summary>
        /// Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um
        /// valor default se o index nao existir ou seu valor for branco ou null
        /// </summary>
        /// <typeparam name="T">Tipo do IEnumerable e do valor</typeparam>
        /// <param name="Arr">Array</param>
        /// <param name="Index">Posicao</param>
        /// <param name="ValueIfBlankOrNoIndex">Valor se o mesmo nao existir</param>
        /// <returns></returns>
        public static T IfBlankOrNoIndex<T>(this IEnumerable<T> Arr, int Index, T ValueIfBlankOrNoIndex) => (Arr ?? Array.Empty<T>()).IfNoIndex(Index, ValueIfBlankOrNoIndex).IfBlank(ValueIfBlankOrNoIndex);

        /// <summary>
        /// Tenta retornar um valor de um IEnumerable a partir de um Index especifico. retorna um
        /// valor default se o index nao existir
        /// </summary>
        /// <typeparam name="T">Tipo do IEnumerable e do valor</typeparam>
        /// <param name="Arr">Array</param>
        /// <param name="Index">Posicao</param>
        /// <param name="ValueIfNoIndex">Valor se o mesmo nao existir</param>
        /// <returns></returns>
        public static T IfNoIndex<T>(this IEnumerable<T> Arr, int Index, T ValueIfNoIndex = default)
        {
            var item = (Arr ?? Array.Empty<T>()).ElementAtOrDefault(Index);
            return item == null ? ValueIfNoIndex : item;
        }

        /// <summary>
        /// Executa uma função para uma variavel se a mesma nao estiver em branco ( <see cref="IsBlank{T}())"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="ExpressionIfBlank"></param>
        /// <returns></returns>
        public static T IfNotBlank<T>(this T Value, Expression<Func<T, T>> ExpressionIfBlank)
        {
            if (Value.IsNotBlank())
            {
                if (ExpressionIfBlank != null)
                {
                    try
                    {
                        return ExpressionIfBlank.Compile().Invoke(Value);
                    }
                    catch { }
                }
            }
            return Value;
        }

        /// <summary>
        /// Verifica se um array está vazio ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValuesIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static T[] IfNullOrEmpty<T>(this object[] Value, params T[] ValuesIfBlank) => Value == null || !Value.Any() ? ValuesIfBlank ?? Array.Empty<T>() : Value.ChangeArrayType<T, object>();

        /// <summary>
        /// Verifica se um array está vazio ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValuesIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static IEnumerable<T> IfNullOrEmpty<T>(this IEnumerable<object[]> Value, params T[] ValuesIfBlank) => Value != null && Value.Any() ? Value.ChangeIEnumerableType<T, object[]>() : ValuesIfBlank;

        /// <summary>
        /// Verifica se um array está vazio ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValueIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static IEnumerable<T> IfNullOrEmpty<T>(this IEnumerable<object[]> Value, IEnumerable<T> ValueIfBlank) => Value != null && Value.Any() ? Value.ChangeIEnumerableType<T, object[]>() : ValueIfBlank;

        public static bool IsArray<T>(T Obj)
        {
            try
            {
                var ValueType = Obj.GetType();
                return !(ValueType == typeof(string)) && ValueType.IsArray; //  GetType(T).IsAssignableFrom(ValueType.GetElementType())
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se uma variavel está em branco. (nula, vazia ou somente brancos para string,
        /// null ou 0 para tipos primitivos, null ou ToString() em branco para tipos de referencia.
        /// Null ou vazio para arrays)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool IsBlank(this object Value)
        {
            try
            {
                if (Value != null)
                {
                    var tipo = Value.GetTypeOf();

                    if (tipo.IsNumericType())
                    {
                        return Value.ChangeType<decimal>() == 0;
                    }
                    else if (Value is FormattableString fs)
                    {
                        return IsBlank($"{fs}".ToUpperInvariant());
                    }
                    else if (Value is bool b)
                    {
                        return !b;
                    }
                    else if (Value is string s)
                    {
                        return string.IsNullOrWhiteSpace($"{s}".RemoveAny(PredefinedArrays.BreakLineChars.ToArray()));
                    }
                    else if (Value is char c)
                    {
                        return string.IsNullOrWhiteSpace($"{c}".RemoveAny(PredefinedArrays.BreakLineChars.ToArray()));
                    }
                    else if (Value is DateTime time)
                    {
                        return time.Equals(DateTime.MinValue);
                    }
                    else if (Value is TimeSpan span)
                    {
                        return span.Equals(TimeSpan.MinValue);
                    }
                    else if (Value is DateTimeOffset off)
                    {
                        return off.Equals(DateTimeOffset.MinValue);
                    }
                    else if (Value.IsEnumerable())
                    {
                        IEnumerable enumerable = (IEnumerable)Value;
                        foreach (object item in enumerable)
                        {
                            if (item.IsNotBlank())
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Ext.WriteDebug(ex);
            }
            return true;
        }

        /// <summary>
        /// Verifica se uma String está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>TRUE se estivar vazia ou em branco, caso contrario FALSE</returns>
        public static bool IsBlank(this FormattableString Text) => Text == null || $"{Text}".IsBlank();

        public static bool IsBool<T>(this T Obj) => Ext.GetNullableTypeOf(Obj) == typeof(bool) || $"{Obj}".ToLowerInvariant().IsIn("true", "false");

        public static bool IsDate(this string Obj)
        {
            try
            {
                return DateTime.TryParse(Obj, out _);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDate<T>(this T Obj) => Ext.GetNullableTypeOf(Obj) == typeof(DateTime) || $"{Obj}".IsDate();

        /// <summary>
        /// Verifica se uma string é um caminho de diretório válido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>TRUE se o caminho for válido</returns>
        public static bool IsDirectoryPath(this string Text)
        {
            if (Text.IsBlank())
            {
                return false;
            }

            Text = Text.Trim();
            try
            {
                if (Directory.Exists(Text))
                {
                    return true;
                }

                if (File.Exists(Text))
                {
                    return false;
                }
            }
            catch { }

            try
            {
                // if has trailing slash then it's a directory

                if (new string[] { Convert.ToString(Path.DirectorySeparatorChar, CultureInfo.InvariantCulture), Convert.ToString(Path.AltDirectorySeparatorChar, CultureInfo.InvariantCulture) }.Any(x => Text.EndsWith(x, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return true;
                }
                // ends with slash if has extension then its a file; directory otherwise
                return string.IsNullOrWhiteSpace(Path.GetExtension(Text));
            }
            catch
            {
                return false;
            }
        }

        public static bool IsDomain(this string Text) => Text.IsNotBlank() && $"http://{Text}".IsURL();

        /// <summary>
        /// Verifica se um determinado texto é um email
        /// </summary>
        /// <param name="Text">Texto a ser validado</param>
        /// <returns>TRUE se for um email, FALSE se não for email</returns>
        public static bool IsEmail(this string Text) => Text.IsNotBlank() && new Regex(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|Util.Empty(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*Util.Empty)@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])").IsMatch(Text);

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this decimal Value) => Value % 2m == 0m;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this int Value) => Value % 2 == 0;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this short Value) => Value % 2 == 0;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this float Value) => Value % 2f == 0f;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this long Value) => Value % 2L == 0L;

        /// <summary>
        /// Verifica se um numero é par
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsEven(this double Value) => Value % 2d == 0d;

        /// <summary>
        /// Verifica se uma string é um caminho de arquivo válido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>TRUE se o caminho for válido</returns>
        public static bool IsFilePath(this string Text)
        {
            if (Text.IsBlank())
            {
                return false;
            }

            Text = Text.Trim();
            try
            {
                if (Directory.Exists(Text))
                {
                    return false;
                }

                if (File.Exists(Text))
                {
                    return true;
                }
            }
            catch
            {
            }
            try
            {
                // if has extension then its a file; directory otherwise
                return !Text.EndsWith(Convert.ToString(Path.DirectorySeparatorChar, CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase) && Path.GetExtension(Text).IsNotBlank();
            }
            catch { return false; }
        }

        public static bool IsInUse(this FileInfo File)
        {
            //Try-Catch so we dont crash the program and can check the exception
            try
            {
                if (File.Exists)
                {
                    using (FileStream fileStream = System.IO.File.Open(File.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        fileStream?.Close();
                    }
                }
            }
            catch (IOException ex)
            {
                //THE FUNKY MAGIC - TO SEE IF THIS FILE REALLY IS LOCKED!!!

                int errorCode = Marshal.GetHRForException(ex) & ((1 << 16) - 1);

                if (errorCode == ERROR_SHARING_VIOLATION || errorCode == ERROR_LOCK_VIOLATION)
                {
                    return true;
                }
            }
            finally
            { }
            return false;
        }

        /// <summary>
        /// Verifica se a string é um endereço IP válido
        /// </summary>
        /// <param name="IP">Endereco IP</param>
        /// <returns>TRUE ou FALSE</returns>
        public static bool IsIP(this string IP) => Regex.IsMatch(IP, @"\b((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$\b");

        /// <summary>
        /// Verifica se uma String não está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>FALSE se estiver nula, vazia ou em branco, caso contrario TRUE</returns>
        public static bool IsNotBlank(this object Value) => !IsBlank(Value);

        /// <summary>
        /// Verifica se uma String não está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>FALSE se estiver nula, vazia ou em branco, caso contrario TRUE</returns>
        public static bool IsNotBlank(this FormattableString Text) => Text != null && IsNotBlank(FormattableString.Invariant(Text));

        /// <summary>
        /// Verifica se o valor não é um numero
        /// </summary>
        /// <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
        /// <returns>FALSE se for um numero, TRUE se não for um numero</returns>
        public static bool IsNotNumber(this object Value) => !Value.IsNumber();

        /// <summary>
        /// Verifica se o valor é um numero
        /// </summary>
        /// <param name="Value">Valor a ser verificado, pode ser qualquer objeto</param>
        /// <returns>TRUE se for um numero, FALSE se não for um numero</returns>
        public static bool IsNumber(this object Value)
        {
            try
            {
                Convert.ToDecimal(Value, CultureInfo.InvariantCulture);
                return Value != null && $"{Value}".IsIP() == false && ((Value.GetType() == typeof(DateTime)) == false);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this decimal Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this int Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this long Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this short Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se um numero é impar
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <returns></returns>
        public static bool IsOdd(this float Value) => !Value.IsEven();

        /// <summary>
        /// Verifica se uma string é um caminho de arquivo ou diretório válido
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns>TRUE se o caminho for válido</returns>
        public static bool IsPath(this string Text) => Text.IsDirectoryPath() || Text.IsFilePath();

        /// <summary>
        /// Verifica se um determinado texto é uma URL válida
        /// </summary>
        /// <param name="Text">Texto a ser verificado</param>
        /// <returns>TRUE se for uma URL, FALSE se não for uma URL válida</returns>
        public static bool IsURL(this string Text) => Text.IsNotBlank() && Uri.TryCreate(Text.Trim(), UriKind.Absolute, out _) && !Text.Trim().Contains(" ");

        /// <summary>
        /// Verifica se o dominio é válido (existe) em uma URL ou email
        /// </summary>
        /// <param name="DomainOrEmail">Uma String contendo a URL ou email</param>
        /// <returns>TRUE se o dominio existir, FALSE se o dominio não existir</returns>
        /// <remarks>Retornara sempre false quando nao houver conexao com a internet</remarks>
        public static bool IsValidDomain(this string DomainOrEmail)
        {
            System.Net.IPHostEntry ObjHost;
            if (DomainOrEmail.IsEmail() == true)
            {
                DomainOrEmail = "http://" + DomainOrEmail.GetAfter("@");
            }
            if (DomainOrEmail.IsURL())
            {
                try
                {
                    string HostName = new Uri(DomainOrEmail).Host;
                    ObjHost = System.Net.Dns.GetHostEntry(HostName);
                    return (ObjHost.HostName ?? InnerLibs.Ext.EmptyString) == (HostName ?? InnerLibs.Ext.EmptyString);
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// Verifica se um numero é um EAN válido
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static bool IsValidEAN(this string Code)
        {
            if (Code == null || Code.IsNotNumber() || Code.Length < 3)
            {
                return false;
            }

            var bar = Code.RemoveLastChars();
            var ver = Code.GetLastChars();
            return Ext.BarcodeCheckSum(bar) == ver;
        }

        public static bool IsValidEAN(this int Code) => Code.ToString(CultureInfo.InvariantCulture).PadLeft(12, '0').ToString().IsValidEAN();

        /// <summary>
        /// Anula o valor de um objeto se ele for igual a outro objeto
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestExpression">Outro Objeto</param>
        /// <returns></returns>
        public static T NullIf<T>(this T Value, Func<T, bool> TestExpression)
        {
            if (TestExpression != null && TestExpression.Invoke(Value))
            {
                return default;
            }

            return Value;
        }

        /// <summary>
        /// Anula o valor de um objeto se ele for igual a outro objeto
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestValue">Outro Objeto</param>
        /// <returns></returns>
        public static T NullIf<T>(this T Value, T TestValue) where T : class
        {
            if (Value != null && Value.Equals(TestValue))
            {
                return null;
            }

            return Value;
        }

        /// <summary>
        /// Anula o valor de um objeto se ele for igual a outro objeto
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestValue">Outro Objeto</param>
        /// <returns></returns>
        public static T? NullIf<T>(this T? Value, T? TestValue) where T : struct
        {
            if (Value.HasValue && Value.Equals(TestValue))
            {
                Value = default;
            }

            return Value;
        }

        /// <summary>
        /// Anula o valor de uma string se ela for igual a outra string
        /// </summary>
        /// <param name="Value">Valor</param>
        /// <param name="TestValue">Outro Objeto</param>
        /// <returns></returns>
        public static string NullIf(this string Value, string TestValue, StringComparison ComparisonType = StringComparison.InvariantCultureIgnoreCase)
        {
            if (Value == null || Value.Equals(TestValue, ComparisonType))
            {
                Value = null;
            }

            return Value;
        }

        /// <summary>
        /// Returns true if all logical operations return true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool Validate<T>(this T Value, params Expression<Func<T, bool>>[] Tests) => Validate(Value, 0, Tests);

        /// <summary>
        /// Returns true if a certain minimum number of logical operations return true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool Validate<T>(this T Value, int MinPoints, params Expression<Func<T, bool>>[] Tests)
        {
            Tests = Tests ?? Array.Empty<Expression<Func<T, bool>>>();
            if (MinPoints < 1)
            {
                MinPoints = Tests.Length;
            }
            return ValidateCount(Value, Tests) >= MinPoints.LimitRange(1, Tests.Length);
        }

        /// <summary>
        /// Returns the count of true logical operations on a given value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static int ValidateCount<T>(this T Value, params Expression<Func<T, bool>>[] Tests)
        {
            var count = 0;
            Tests = Tests ?? Array.Empty<Expression<Func<T, bool>>>();
            foreach (var item in Tests)
            {
                if (item.Compile().Invoke(Value))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Lança uma <see cref="Exception"/> do tipo <typeparamref name="TE"/> se um teste falhar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Test"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static T ValidateOr<T, TE>(this T Value, Expression<Func<T, bool>> Test, TE Exception) where TE : Exception
        {
            if (Test != null)
            {
                if (Test.Compile().Invoke(Value) == false)
                {
                    Value = default;
                    if (Exception != null)
                    {
                        throw Exception;
                    }
                }
            }
            return Value;
        }

        public static T ValidateOr<T>(this T Value, Expression<Func<T, bool>> Test) => ValidateOr(Value, Test, default);

        public static T ValidateOr<T>(this T Value, Expression<Func<T, bool>> Test, T defaultValue)
        {
            try
            {
                return ValidateOr(Value, Test, new Exception("Validation fail"));
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool ValidatePassword(this string Password, PasswordLevel PasswordLevel = PasswordLevel.Strong) => PasswordLevel == PasswordLevel.None || Password.CheckPassword().ToInt() >= PasswordLevel.ToInt();

        /// <summary>
        /// Bloqueia a Thread atual enquanto um arquivo estiver em uso
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <param name="Seconds">intervalo, em segundo entre as tentativas de acesso</param>
        /// <param name="MaxFailCount">
        /// Numero maximo de tentativas falhas,quando nulo, verifica infinitamente
        /// </param>
        /// <param name="OnAttemptFail">ação a ser executado em caso de falha</param>
        /// <returns>TRUE se o arquivo puder ser utilizado</returns>
        public static bool WaifForFile(this FileInfo File, int Seconds = 1, int? MaxFailCount = null, Action<int> OnAttemptFail = null)
        {
            if (File == null)
            {
                return false;
            }
            if (File.Exists == false)
            {
                return true;
            }

            while (IsInUse(File))
            {
                Thread.Sleep(Seconds * 1000);

                if (File.Exists == false)
                {
                    return true;
                }

                if (MaxFailCount.HasValue)
                {
                    MaxFailCount = MaxFailCount.Value - 1;
                }

                if (MaxFailCount.HasValue && MaxFailCount.Value <= 0)
                {
                    return false;
                }

                OnAttemptFail?.Invoke(MaxFailCount ?? -1);
            }
            return true;
        }

        /// <summary>
        /// Executa uma função com um determinado arquivo caso seja possível sua leitura
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <param name="OnSuccess">Função a ser executada ao abrir o arquivo</param>
        /// <param name="OnFail">
        /// Função a ser executada após um numero determinado de tentativas falharem
        /// </param>
        /// <param name="OnAttemptFail">Função a ser executada a cada tentativa falhada</param>
        /// <param name="Seconds">Tempo de espera em segundos entre uma tentativa e outra</param>
        /// <param name="MaxFailCount">Numero máximo de tentativas, infinito se null</param>
        /// <returns>
        /// TRUE após <paramref name="OnSuccess"/> ser executada com sucesso. FALSE em qualquer
        /// outra situação
        /// </returns>
        public static bool WithFile(this FileInfo File, Action<FileInfo> OnSuccess, Action<FileInfo> OnFail, Action<int> OnAttemptFail = null, int Seconds = 1, int? MaxFailCount = null)
        {
            if (File != null)
            {
                try
                {
                    if (WaifForFile(File, Seconds, MaxFailCount, OnAttemptFail))
                    {
                        OnSuccess?.Invoke(File);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ex.WriteDebug("Execution of OnSuccess failed");

                    try
                    {
                        OnFail?.Invoke(File);
                    }
                    catch (Exception exf)
                    {
                        exf.WriteDebug("Execution of OnFail failed");
                    }
                }
            }
            return false;
        }

        #endregion Public Methods

        /// <summary>
        /// Encapsula um tento entre 2 textos
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static string Wrap(this string Text, string OpenWrapText, string CloseWrapText) => $"{OpenWrapText}{Text}{CloseWrapText.IfBlank(OpenWrapText)}";

        public static HtmlTag WrapInTag(this IEnumerable<HtmlTag> Tags, string TagName) => new HtmlTag(TagName).AddChildren(Tags);

        public static HtmlTag WrapInTag(this HtmlTag Tag, string TagName) => new HtmlTag(TagName).AddChildren(Tag);

        public static HtmlTag WrapInTag(this string Text, string TagName) => new HtmlTag() { InnerHtml = Text, TagName = TagName };

        #endregion Public Methods

        #endregion Public Methods

        public static int ToArabic(this string RomanNumber)
        {
            RomanNumber = $"{RomanNumber}".ToUpper(CultureInfo.InvariantCulture).Trim();
            if (RomanNumber == "N" || RomanNumber.IsBlank())
            {
                return 0;
            }

            // Os numerais que representam números que começam com um '5'(TV, L e D) podem aparecer
            // apenas uma vez em cada numeral romano. Esta regra permite XVI, mas não VIV.
            if (RomanNumber.Split('V').Length > 2 || RomanNumber.Split('L').Length > 2 || RomanNumber.Split('D').Length > 2)
            {
                throw new ArgumentException("Roman number with invalid numerals. The number has a TV, L or D number repeated.");
            }

            // Uma única letra pode ser repetida até três vezes consecutivamente sendo que cada
            // ocorrência será somanda. Isto significa que I é um, II e III significa dois é três.
            // No entanto, IIII não é permitido.
            int contador = 1;
            char ultimo = 'Z';
            foreach (char numeral in RomanNumber)
            {
                // caractere inválido ?
                if ("IVXLCDM".IndexOf(numeral) == -1)
                {
                    throw new ArgumentException("Roman number with invalid positioning.");
                }

                // Duplicado?
                if (numeral == ultimo)
                {
                    contador += 1;
                    if (contador == 4)
                    {
                        throw new ArgumentException("A Roman number can not be repeated more than 3 times in the same number.");
                    }
                }
                else
                {
                    contador = 1;
                    ultimo = numeral;
                }
            }

            // Cria um ArrayList contendo os valores
            int ptr = 0;
            var valores = new ArrayList();
            int digitoMaximo = 1000;
            while (ptr < RomanNumber.Length)
            {
                // valor base do digito
                char numeral = RomanNumber[ptr];
                int digito = Convert.ToInt32(Enum.Parse(typeof(RomanDigit), numeral.ToString()));

                // Um numeral de pequena valor pode ser colocado à esquerda de um valor maior Quando
                // isto ocorre, por exemplo IX, o menor número é subtraído do maior T dígito
                // subtraído deve ser de pelo menos um décimo do valor do maior numeral e deve ser
                // ou I, X ou C Valores como MCMD ou CMC não são permitidos
                if (digito > digitoMaximo)
                {
                    throw new ArgumentException("Roman number with invalid positioning.");
                }

                if (ptr < RomanNumber.Length - 1)
                {
                    char proximoNumeral = RomanNumber[ptr + 1];
                    // proximo digito
                    int proximoDigito = Convert.ToInt32(Enum.Parse(typeof(RomanDigit), proximoNumeral.ToString()));
                    if (proximoDigito > digito)
                    {
                        if ("IXC".IndexOf(numeral) == -1 || proximoDigito > digito * 10 || RomanNumber.Split(numeral).Length > 3)
                        {
                            throw new ArgumentException("Rule 3");
                        }

                        digitoMaximo = digito - 1;
                        digito = proximoDigito - digito;
                        ptr += 1;
                    }
                }

                valores.Add(digito);

                // proximo digito
                ptr += 1;
            }

            // Outra regra é a que compara o tamanho do valor de cada numeral lido a partir da
            // esquerda para a direita. T valor nunca deve aumentar a partir de uma letra para a
            // próxima. Onde houver um numeral subtrativo, esta regra se aplica ao valor combinado
            // dos dois algarismos envolvidos na subtração quando comparado com a letra anterior.
            // Isto significa que XIX é aceitável, mas XIM e IIV não são.
            for (int i = 0, loopTo = valores.Count - 2; i <= loopTo; i++)
            {
                if (Convert.ToInt32(valores[i]) < Convert.ToInt32(valores[i + 1]))
                {
                    throw new ArgumentException("Invalid Roman number.In this case the digit can not be greater than the previous one.");
                }
            }

            // Numerais maiores devem ser colocados à esquerda dos números menores para continuar a
            // combinação aditiva. Assim VI é igual a seis e MDCLXI é 1.661.
            int total = 0;
            foreach (int digito in valores)
                total += digito;
            return total;
        }

        #region Private Fields

        private static readonly Random init_rnd = new Random();

        #endregion Private Fields

        #region Public Methods

        /// <inheritdoc cref="BarcodeCheckSum(string)"/>
        public static string BarcodeCheckSum(long Code) => BarcodeCheckSum(Code.ToString(CultureInfo.InvariantCulture));

        /// <inheritdoc cref="BarcodeCheckSum(string)"/>
        public static string BarcodeCheckSum(int Code) => BarcodeCheckSum(Code.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Gera um digito verificador usando Mod10 em um numero
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        /// <exception cref="FormatException"></exception>
        public static string BarcodeCheckSum(string Code)
        {
            if (Code.IsNotNumber())
            {
                throw new FormatException("Code is not number");
            }

            int i = 0;
            int j;
            int p = 0;
            int T;
            T = Code.Length;
            for (j = 1; j <= T; j++)
            {
                if ((j & ~-2) == 0)
                {
                    p += Code.Substring(j - 1, 1).ToInt();
                }
                else
                {
                    i += Code.Substring(j - 1, 1).ToInt();
                }
            }
            if (T == 7 | T == 11)
            {
                i = i * 3 + p;
                p = Ext.ToInt((i + 9) / 10) * 10;
                T = p - i;
            }
            else
            {
                p = p * 3 + i;
                i = Ext.ToInt((p + 9) / 10) * 10;
                T = i - p;
            }
            return T.ToString(CultureInfo.InvariantCulture);
        }

        public static string EAN(int ContryCode, int ManufacturerCode, int ProductCode) => EANFromNumbers(ContryCode, ManufacturerCode, ProductCode);

        /// <summary>
        /// Gera um numero de EAN válido a aprtir da combinação de vários numeros
        /// </summary>
        /// <param name="Numbers"></param>
        /// <returns></returns>
        public static string EANFromNumbers(params string[] Numbers) => Numbers.Where(x => x.IsNumber()).SelectJoinString(InnerLibs.Ext.EmptyString).AppendBarcodeCheckSum();

        /// <inheritdoc cref="EANFromNumbers(string[])"/>
        public static string EANFromNumbers(params int[] Numbers) => EANFromNumbers(Numbers.Select(x => x.ToString()).ToArray());

        /// <summary>
        /// Util a password with specific lenght for each char type
        /// </summary>
        /// <param name="AlphaLenght"></param>
        /// <param name="NumberLenght"></param>
        /// <param name="SpecialLenght"></param>
        /// <returns></returns>
        public static string Password(int AlphaLenght, int NumberLenght, int SpecialLenght) => Password((AlphaLenght / 2d).CeilInt(), (AlphaLenght / 2d).FloorInt(), NumberLenght, SpecialLenght);

        /// <summary>
        /// Util a password with specific lenght for each char type
        /// </summary>
        /// <returns></returns>
        public static string Password(int AlphaUpperLenght, int AlphaLowerLenght, int NumberLenght, int SpecialLenght)
        {
            string pass = InnerLibs.Ext.EmptyString;
            if (AlphaLowerLenght > 0)
            {
                string ss = InnerLibs.Ext.EmptyString;
                while (ss.Length < AlphaLowerLenght)
                {
                    ss = ss.Append(PredefinedArrays.AlphaLowerChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            if (AlphaUpperLenght > 0)
            {
                string ss = InnerLibs.Ext.EmptyString;
                while (ss.Length < AlphaUpperLenght)
                {
                    ss = ss.Append(PredefinedArrays.AlphaUpperChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            if (NumberLenght > 0)
            {
                string ss = InnerLibs.Ext.EmptyString;
                while (ss.Length < NumberLenght)
                {
                    ss = ss.Append(PredefinedArrays.NumberChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            if (SpecialLenght > 0)
            {
                string ss = InnerLibs.Ext.EmptyString;
                while (ss.Length < SpecialLenght)
                {
                    ss = ss.Append(PredefinedArrays.PasswordSpecialChars.RandomItem());
                }

                pass = pass.Append(ss);
            }

            return pass.Shuffle();
        }

        /// <summary>
        /// Util a password with specific <paramref name="Lenght"/>
        /// </summary>
        /// <param name="Lenght"></param>
        /// <returns></returns>
        public static string Password(int Lenght = 8)
        {
            var basenumber = Lenght / 3d;
            return Password(basenumber.CeilInt(), basenumber.FloorInt(), basenumber.FloorInt()).PadRight(Lenght, Convert.ToChar(PredefinedArrays.AlphaChars.RandomItem())).GetFirstChars(Lenght);
        }

        /// <summary>
        /// Gera um valor boolean aleatorio considerando uma porcentagem de chance
        /// </summary>
        /// <returns>TRUE ou FALSE.</returns>
        public static bool RandomBool(int Percent) => RandomBool(x => x <= Percent, 0, 100);

        /// <summary>
        /// Gera um valor boolean aleatorio considerando uma condição de comparação com um numero
        /// gerado aleatóriamente
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão 999999</param>
        /// <returns>TRUE ou FALSE</returns>
        public static bool RandomBool(Func<int, bool> Condition, int Min = 0, int Max = int.MaxValue) => Condition(RandomNumber(Min, Max));

        /// <summary>
        /// Gera um valor boolean aleatorio
        /// </summary>
        /// <returns>TRUE ou FALSE</returns>
        public static bool RandomBool() => RandomNumber(0, 1).ToBool();

        /// <summary>
        /// Gera uma lista com <paramref name="Quantity"/> cores diferentes
        /// </summary>
        /// <param name="Quantity">Quantidade máxima de cores</param>
        /// <param name="Red"></param>
        /// <param name="Green"></param>
        /// <param name="Blue"></param>
        /// <remarks></remarks>
        /// <returns></returns>
        public static List<Color> RandomColorList(int Quantity, int Red = -1, int Green = -1, int Blue = -1)
        {
            Red = Red.SetMinValue(-1);
            Green = Green.SetMinValue(-1);
            Blue = Blue.SetMinValue(-1);

            var l = new List<Color>();
            if (Red == Green && Green == Blue && Blue != -1)
            {
                l.Add(Color.FromArgb(Red, Green, Blue));
                return l;
            }

            int errorcount = 0;
            while (l.Count < Quantity)
            {
                var r = ColorExtensions.RandomColor(Red, Green, Blue);
                if (l.Any(x => (x.ToHexadecimal() ?? InnerLibs.Ext.EmptyString) == (r.ToHexadecimal() ?? InnerLibs.Ext.EmptyString)))
                {
                    errorcount++;
                    if (errorcount == Quantity)
                    {
                        return l;
                    }
                }
                else
                {
                    errorcount = 0;
                    l.Add(r);
                }
            }

            return l;
        }

        /// <summary>
        /// Gera uma data aleatória a partir de componentes nulos de data
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static DateTime RandomDateTime(int? Year = null, int? Month = null, int? Day = null, int? Hour = null, int? Minute = null, int? Second = null)
        {
            Year = (Year ?? RandomNumber(DateTime.MinValue.Year, DateTime.MaxValue.Year)).ForcePositive().LimitRange(DateTime.MinValue.Year, DateTime.MaxValue.Year);
            Month = (Month ?? RandomNumber(DateTime.MinValue.Month, DateTime.MaxValue.Month)).ForcePositive().LimitRange(1, 12);
            Day = (Day ?? RandomNumber(DateTime.MinValue.Day, DateTime.MaxValue.Day)).ForcePositive().LimitRange(1, 31);
            Hour = (Hour ?? RandomNumber(DateTime.MinValue.Hour, DateTime.MaxValue.Hour)).ForcePositive().LimitRange(1, 31);
            Minute = (Minute ?? RandomNumber(DateTime.MinValue.Minute, DateTime.MaxValue.Minute)).ForcePositive().LimitRange(0, 59);
            Second = (Second ?? RandomNumber(DateTime.MinValue.Second, DateTime.MaxValue.Second)).ForcePositive().LimitRange(0, 59);

            DateTime randomCreated = DateTime.Now;
            while (Ext.TryExecute(() => randomCreated = new DateTime(Year.Value, Month.Value, Day.Value, Hour.Value, Minute.Value, Second.Value)) != null)
            {
                Day--;
            }

            return randomCreated;
        }

        /// <summary>
        /// Gera uma data aleatória entre 2 datas
        /// </summary>
        /// <param name="Min">Data Minima</param>
        /// <param name="Max">Data Maxima</param>
        /// <returns>Um numero Inteiro</returns>
        public static DateTime RandomDateTime(DateTime? MinDate, DateTime? MaxDate = null)
        {
            var Min = (MinDate ?? RandomDateTime()).Ticks;
            var Max = (MaxDate ?? RandomDateTime()).Ticks;
            Ext.FixOrder(ref Min, ref Max);
            return new DateTime(RandomNumber(Min, Max));
        }

        /// <summary>
        /// Gera um EAN aleatório com digito verificador válido
        /// </summary>
        /// <param name="Len"></param>
        /// <returns></returns>
        public static string RandomEAN(int Len) => RandomFixLenghtNumber(Len.SetMinValue(2) - 1).ToString().AppendBarcodeCheckSum();

        /// <summary>
        /// Gera um numero aleatório de comprimento fixo
        /// </summary>
        /// <param name="Len"></param>
        /// <returns></returns>
        public static string RandomFixLenghtNumber(int Len = 8)
        {
            var n = InnerLibs.Ext.EmptyString;
            for (int i = 0; i < Len; i++)
            {
                n += PredefinedArrays.NumberChars.RandomItem();
            }
            return n;
        }

        /// <summary>
        /// Gera um texto aleatorio
        /// </summary>
        /// <param name="ParagraphCount">Quantidade de paragrafos</param>
        /// <param name="SentenceCount">QUantidade de sentenças por paragrafo</param>
        /// <param name="MinWordCount"></param>
        /// <param name="MaxWordCount"></param>
        /// <returns></returns>
        public static TextStructure RandomIpsum(int ParagraphCount = 5, int SentenceCount = 3, int MinWordCount = 10, int MaxWordCount = 50, int IdentSize = 0, int BreakLinesBetweenParagraph = 0) => new TextStructure(Enumerable.Range(1, ParagraphCount.SetMinValue(1)).SelectJoinString(pp => Enumerable.Range(1, SentenceCount.SetMinValue(1)).SelectJoinString(s => Enumerable.Range(1, RandomNumber(MinWordCount.SetMinValue(1), MaxWordCount.SetMinValue(1))).SelectJoinString(p => RandomBool(20).AsIf(RandomWord(RandomNumber(2, 6)).ToUpperInvariant(), RandomWord()) + RandomBool(30).AsIf(","), " "), PredefinedArrays.EndOfSentencePunctuation.TakeRandom() + " "), Environment.NewLine)) { Ident = IdentSize, BreakLinesBetweenParagraph = BreakLinesBetweenParagraph };

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static int RandomNumber(int Min = 0, int Max = int.MaxValue)
        {
            Ext.FixOrder(ref Min, ref Max);
            return Min == Max ? Min : init_rnd.Next(Min, Max == int.MaxValue ? int.MaxValue : Max + 1);
        }

        /// <summary>
        /// Gera um numero Aleatório entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="long.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static long RandomNumber(long Min, long Max = long.MaxValue)
        {
            Ext.FixOrder(ref Min, ref Max);
            if (Min == Max)
            {
                return Min;
            }
            else
            {
                Max = Max == long.MaxValue ? long.MaxValue : Max + 1;
                byte[] buf = new byte[8];
                init_rnd.NextBytes(buf);
                long longRand = BitConverter.ToInt64(buf, 0);
                return Math.Abs(longRand % (Max - Min)) + Min;
            }
        }

        /// <summary>
        /// Gera uma Lista com numeros Aleatórios entre 2 números
        /// </summary>
        /// <param name="Min">Numero minimo, Padrão 0</param>
        /// <param name="Max">Numero Maximo, Padrão <see cref="int.MaxValue"/></param>
        /// <returns>Um numero Inteiro</returns>
        public static IEnumerable<int> RandomNumberList(int Count, int Min = 0, int Max = int.MaxValue, bool UniqueNumbers = true)
        {
            if (Count > 0)
            {
                if (Max == Min) return new int[] { Min };
                if (UniqueNumbers)
                {
                    if (Max < int.MaxValue) Max++;
                    Ext.FixOrder(ref Min, ref Max);
                    var l = Enumerable.Range(Min, Max - Min).OrderByRandom().ToList();
                    while (l.Count > Count) l.RemoveAt(0);
                    return l;
                }
                else
                {
                    return Enumerable.Range(1, Count).Select(e => RandomNumber(Min, Max));
                }
            }
            return Array.Empty<int>();
        }

        /// <summary>
        /// Gera uma palavra aleatória com o numero de caracteres entre <paramref name="MinLength"/>
        /// e <paramref name="MaxLenght"/>
        /// </summary>
        /// <returns>Uma string contendo uma palavra aleatória</returns>
        public static string RandomWord(int MinLength, int MaxLenght) => RandomWord(RandomNumber(MinLength.SetMinValue(1), MaxLenght.SetMinValue(1)));

        /// <summary>
        /// Gera uma palavra aleatória com o numero de caracteres
        /// </summary>
        /// <param name="Length">Tamanho da palavra</param>
        /// <returns>Uma string contendo uma palavra aleatória</returns>
        public static string RandomWord(int Length = 0)
        {
            Length = Length < 1 ? RandomNumber(2, 15) : Length;
            string word = Ext.EmptyString;
            if (Length == 1)
            {
                return Ext.RandomItem(PredefinedArrays.Vowels.ToArray());
            }

            // Util the word in consonant / vowel pairs
            while (word.Length < Length)
            {
                // Add the consonant
                string consonant = PredefinedArrays.LowerConsonants.RandomItem();
                if (consonant == "q" && word.Length + 3 <= Length)
                {
                    // check +3 because we'd add 3 characters in this case, the "qu" and the vowel.
                    // Change 3 to 2 to allow words that end in "qu"
                    word += "qu";
                }
                else
                {
                    while (consonant == "q")
                    {
                        // ReplaceFrom an orphaned "q"
                        consonant = PredefinedArrays.LowerConsonants.RandomItem();
                    }

                    if (word.Length + 1 <= Length)
                    {
                        // Only add a consonant if there's enough room remaining
                        word += consonant;
                    }
                }

                if (word.Length + 1 <= Length)
                {
                    // Only add a vowel if there's enough room remaining
                    word += PredefinedArrays.LowerVowels.RandomItem();
                }
            }

            return word;
        }

        #region Private Methods

        /// <summary>
        /// Aplica um borrão a uma determinada parte da imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="BlurSize"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        private static unsafe Image Blur(this Image Img, int BlurSize, Rectangle rectangle)
        {
            Bitmap blurred = new Bitmap(Img.Width, Img.Height);

            // make an exact copy of the bitmap provided
            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(Img, new Rectangle(0, 0, Img.Width, Img.Height),
                    new Rectangle(0, 0, Img.Width, Img.Height), GraphicsUnit.Pixel);

            // Lock the bitmap's bits
            BitmapData blurredData = blurred.LockBits(new Rectangle(0, 0, Img.Width, Img.Height), ImageLockMode.ReadWrite, blurred.PixelFormat);

            // Get bits per pixel for current PixelFormat
            int bitsPerPixel = Image.GetPixelFormatSize(blurred.PixelFormat);

            // Get pointer to first line
            byte* scan0 = (byte*)blurredData.Scan0.ToPointer();

            // look at every pixel in the blur rectangle
            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // average the color of the red, green and blue for each pixel in the blur size
                    // while making sure you don't go outside the image bounds
                    for (int x = xx; (x < xx + BlurSize && x < Img.Width); x++)
                    {
                        for (int y = yy; (y < yy + BlurSize && y < Img.Height); y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            avgB += data[0]; // Blue
                            avgG += data[1]; // Green
                            avgR += data[2]; // Red

                            blurPixelCount++;
                        }
                    }

                    avgR /= blurPixelCount;
                    avgG /= blurPixelCount;
                    avgB /= blurPixelCount;

                    // now that we know the average for the blur size, set each pixel to that color
                    for (int x = xx; x < xx + BlurSize && x < Img.Width && x < rectangle.Width; x++)
                    {
                        for (int y = yy; y < yy + BlurSize && y < Img.Height && y < rectangle.Height; y++)
                        {
                            // Get pointer to RGB
                            byte* data = scan0 + y * blurredData.Stride + x * bitsPerPixel / 8;

                            // Change values
                            data[0] = (byte)avgB;
                            data[1] = (byte)avgG;
                            data[2] = (byte)avgR;
                        }
                    }
                }
            }

            // Unlock the bits
            blurred.UnlockBits(blurredData);

            return blurred;
        }

        private static bool draw_adjusted_image(Image img, ColorMatrix cm)
        {
            try
            {
                var bmp = new Bitmap(img); // create a copy of the source image
                var imgattr = new ImageAttributes();
                var rc = new Rectangle(0, 0, img.Width, img.Height);
                var g = Graphics.FromImage(img);

                // associate the ColorMatrix object with an ImageAttributes object
                imgattr.SetColorMatrix(cm);

                // draw the copy of the source image back over the original image, applying the ColorMatrix
                g.DrawImage(bmp, rc, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgattr);
                g.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion Private Methods

        #region Public Properties

        /// <summary>
        /// Lista com todos os formatos de imagem
        /// </summary>
        /// <returns></returns>
        public static ImageFormat[] ImageTypes { get; private set; } = new[] { ImageFormat.Bmp, ImageFormat.Emf, ImageFormat.Exif, ImageFormat.Gif, ImageFormat.Icon, ImageFormat.Jpeg, ImageFormat.Png, ImageFormat.Tiff, ImageFormat.Wmf };

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Aplica um borrão a imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="BlurSize"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        public static Image Blur(this Image Img, int BlurSize = 5) => Blur(Img, BlurSize, new Rectangle(0, 0, Img.Width, Img.Height));

        public static Image BrightnessContrastAndGamma(this Image originalimage, float Brightness, float Contrast, float Gamma)
        {
            var adjustedImage = new Bitmap(originalimage);
            Gamma = Gamma.SetMinValue(1.0f);
            Contrast = Contrast.SetMinValue(1.0f);
            float adjustedBrightness = Brightness.SetMinValue(1.0f) - 1.0f;
            var ptsArray = new[] { new float[] { Contrast, 0f, 0f, 0f, 0f }, new float[] { 0f, Contrast, 0f, 0f, 0f }, new float[] { 0f, 0f, Contrast, 0f, 0f }, new float[] { 0f, 0f, 0f, 1.0f, 0f }, new float[] { adjustedBrightness, adjustedBrightness, adjustedBrightness, 0f, 1f } };
            var imageAttributes = new ImageAttributes();
            imageAttributes.ClearColorMatrix();
            imageAttributes.SetColorMatrix(new ColorMatrix(ptsArray), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            imageAttributes.SetGamma(Gamma, ColorAdjustType.Bitmap);
            var g = Graphics.FromImage(adjustedImage);
            g.DrawImage(originalimage, new Rectangle(0, 0, adjustedImage.Width, adjustedImage.Height), 0, 0, originalimage.Width, originalimage.Height, GraphicsUnit.Pixel, imageAttributes);
            return adjustedImage;
        }

        /// <summary>
        /// Retorna uma lista com as cores utilizadas na imagem
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static Dictionary<HSVColor, int> ColorPallette(this Image Img, int PixelateSize = 0)
        {
            Bitmap image;
            if (PixelateSize > 0)
            {
                image = (Bitmap)Img.Pixelate(PixelateSize);
            }
            else
            {
                image = new Bitmap(Img);
            }

            var dctColorIncidence = new Dictionary<int, int>();
            using (image)
                if (image != null && image.Width > 0 && image.Height > 0)
                {
                    int coluna = 0;
                    while (coluna < image.Size.Width)
                    {
                        int linha = 0;
                        while (linha < image.Size.Height)
                        {
                            int pixelColor = image.GetPixel(coluna, linha).ToArgb();
                            if (dctColorIncidence.ContainsKey(pixelColor))
                            {
                                dctColorIncidence[pixelColor] = dctColorIncidence[pixelColor] + 1;
                            }
                            else
                            {
                                dctColorIncidence.Add(pixelColor, 1);
                            }

                            linha++;
                        }

                        coluna++;
                    }
                }

            return dctColorIncidence.OrderByDescending(x => x.Value).ToDictionary(x => new HSVColor(Color.FromArgb(x.Key)), x => x.Value);
        }

        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Lista de Imagens para combinar</param>
        /// <param name="VerticalFlow">
        /// Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as
        /// imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)
        /// </param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens da Lista</returns>
        public static Bitmap CombineImages(bool VerticalFlow, params Image[] Images)
        {
            return Images.CombineImages(VerticalFlow);
        }

        /// <summary>
        /// Combina 2 ou mais imagens em uma única imagem
        /// </summary>
        /// <param name="Images">Array de Imagens para combinar</param>
        /// <param name="VerticalFlow">
        /// Se TRUE, combina as Imagens verticalmente (Uma em baixo da outra), caso contrario as
        /// imagens serão combinadas horizontalmente (Uma do lado da outra da esquerda para a direita)
        /// </param>
        /// <returns>Um Bitmap com a combinaçao de todas as imagens do Array</returns>
        public static Bitmap CombineImages(this IEnumerable<Image> Images, bool VerticalFlow = false)
        {
            int width = 0;
            int height = 0;
            foreach (var image in Images ?? Array.Empty<Image>())
            {
                // cria um bitmap a partir do arquivo e o inclui na lista
                var bitmap = new Bitmap(image);

                // atualiza o tamanho da imagem bitmap final
                if (VerticalFlow)
                {
                    height += bitmap.Height;
                    width = bitmap.Width > width ? bitmap.Width : width;
                }
                else
                {
                    width += bitmap.Width;
                    height = bitmap.Height > height ? bitmap.Height : height;
                }
                bitmap.Dispose();
            }

            // cria um bitmap para tratar a imagem combinada
            Bitmap imagemFinal = new Bitmap(width, height);

            // Obtem o objeto gráfico da imagem
            using (var g = Graphics.FromImage(imagemFinal))
            {
                // define a cor de fundo
                g.Clear(Color.White);

                // percorre imagem por imagem e gera uma unica imagem final
                int offset = 0;
                foreach (Bitmap image in Images.Cast<Bitmap>())
                {
                    if (VerticalFlow)
                    {
                        g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                    else
                    {
                        g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                        offset += image.Width;
                    }
                }
            }

            return imagemFinal;
        }

        public static bool CompareARGB(this Color Color1, Color Color2, bool IgnoreAlpha = true) => Color1.CompareARGB(IgnoreAlpha, Color2);

        public static bool CompareARGB(this Color Color1, bool IgnoreAlpha, params Color[] Colors) => (Colors = Colors ?? Array.Empty<Color>()).Any(Color2 => Color1.R == Color2.R && Color1.G == Color2.G && Color1.B == Color2.B && (IgnoreAlpha || Color1.A == Color2.A));

        public static Image CreateSolidImage(this Color Color, int Width, int Height)
        {
            var Bmp = new Bitmap(Width, Height);
            using (var gfx = Graphics.FromImage(Bmp))
            {
                using (var brush = new SolidBrush(Color))
                {
                    gfx.FillRectangle(brush, 0, 0, Width, Height);
                }
            }

            return Bmp;
        }

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="SizeExpression">Tamanho</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, string SizeExpression) => Image?.Crop(SizeExpression.ParseSize());

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="Size">Tamanho</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, Size Size) => Image?.Crop(Size.Width, Size.Height);

        /// <summary>
        /// Cropa uma imagem a patir do centro
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="maxWidth">Largura maxima</param>
        /// <param name="maxHeight">Altura maxima</param>
        /// <returns></returns>
        public static Image Crop(this Image Image, int MaxWidth, int MaxHeight)
        {
            var jpgInfo = ImageCodecInfo.GetImageEncoders().Where(codecInfo => codecInfo.MimeType == "image/png").First();
            Image finalImage = new Bitmap(Image);
            Bitmap bitmap = null;
            int left = 0;
            int top = 0;
            int srcWidth = MaxWidth;
            int srcHeight = MaxHeight;
            bitmap = new Bitmap(MaxWidth, MaxHeight);
            double croppedHeightToWidth = MaxHeight / (double)MaxWidth;
            double croppedWidthToHeight = MaxWidth / (double)MaxHeight;
            if (Image.Width > Image.Height)
            {
                srcWidth = (int)Math.Round(Math.Round(Image.Height * croppedWidthToHeight));
                if (srcWidth < Image.Width)
                {
                    srcHeight = Image.Height;
                    left = (int)Math.Round((Image.Width - srcWidth) / 2d);
                }
                else
                {
                    srcHeight = (int)Math.Round(Math.Round(Image.Height * (Image.Width / (double)srcWidth)));
                    srcWidth = Image.Width;
                    top = (int)Math.Round((Image.Height - srcHeight) / 2d);
                }
            }
            else
            {
                srcHeight = (int)Math.Round(Math.Round(Image.Width * croppedHeightToWidth));
                if (srcHeight < Image.Height)
                {
                    srcWidth = Image.Width;
                    top = (int)Math.Round((Image.Height - srcHeight) / 2d);
                }
                else
                {
                    srcWidth = (int)Math.Round(Math.Round(Image.Width * (Image.Height / (double)srcHeight)));
                    srcHeight = Image.Height;
                    left = (int)Math.Round((Image.Width - srcWidth) / 2d);
                }
            }

            using (var g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(Image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(left, top, srcWidth, srcHeight), GraphicsUnit.Pixel);
                finalImage = bitmap;
            }

            using (var encParams = new EncoderParameters(1))
            {
                encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100);
                finalImage.RotateFlip(Image.GetRotateFlip());
                return finalImage;
            }
        }

        #region Private Fields

        private static readonly MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

        private static readonly MethodInfo endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });

        private static readonly MethodInfo equalMethod = typeof(string).GetMethod("Equals", new[] { typeof(string) });

        private static readonly MethodInfo startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });

        #endregion Private Fields

        #region Public Methods

        /// <summary>
        /// Retorna TRUE se a todos os testes em uma lista retornarem FALSE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool AllFalse(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).All(x => x == false);

        /// <summary>
        /// Retorna TRUE se a todos os testes em uma lista retornarem TRUE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool AllTrue(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).All(x => x == true);

        /// <summary> Concatena uma expressão com outra usando o operador And (&&) </summary>
        /// <typeparam name="T"></typeparam> <param name="FirstExpression"></param> <param
        /// name="OtherExpressions"></param> <returns></returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> FirstExpression, params Expression<Func<T, bool>>[] OtherExpressions)
        {
            FirstExpression = FirstExpression ?? true.CreateWhereExpression<T>();
            foreach (var item in OtherExpressions ?? Array.Empty<Expression<Func<T, bool>>>())
            {
                if (item != null)
                {
                    var invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast<Expression>());
                    FirstExpression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(FirstExpression.Body, invokedExpr), FirstExpression.Parameters);
                }
            }

            return FirstExpression;
        }

        public static Expression<Func<T, bool>> AndSearch<T>(this Expression<Func<T, bool>> FirstExpression, IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? true.CreateWhereExpression<T>()).And(Text.SearchExpression(Properties));

        public static Expression<Func<T, bool>> AndSearch<T>(this Expression<Func<T, bool>> FirstExpression, string Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? true.CreateWhereExpression<T>()).And(Text.SearchExpression(Properties));

        public static Expression<Func<TModel, TToProperty>> ConvertParameterType<TModel, TFromProperty, TToProperty>(this Expression<Func<TModel, TFromProperty>> expression)
        {
            Expression converted = Expression.Convert(expression.Body, typeof(TToProperty));

            return Expression.Lambda<Func<TModel, TToProperty>>(converted, expression.Parameters);
        }

        /// <summary>
        /// Cria uma constante a partir de um valor para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static ConstantExpression CreateConstant(Expression Member, IComparable Value) => CreateConstant(Member.Type, Value);

        public static ConstantExpression CreateConstant(Type Type, IComparable Value) => Value == null ? Expression.Constant(null, Type) : Expression.Constant(Value.ChangeType(Type));

        /// <summary>
        /// Cria uma constante a partir de um tipo para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        /// <summary>
        /// Cria uma constante a partir de um tipo genérico para ser usada em expressões lambda
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static ConstantExpression CreateConstant<Type>(IComparable Value) => CreateConstant(typeof(Type), Value);

        /// <summary>
        /// Retorna um <see cref="PaginationFilter{T})"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T> CreateFilter<T>(this IEnumerable<T> List) where T : class => (PaginationFilter<T>)new PaginationFilter<T>().SetData(List);

        /// <summary>
        /// Retorna um <see cref="PaginationFilter{T,T})"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T> CreateFilter<T>(this IEnumerable<T> List, Action<PaginationFilter<T, T>> Configuration) where T : class => (PaginationFilter<T>)new PaginationFilter<T>(Configuration).SetData(List);

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, R> CreateFilter<T, R>(this IEnumerable<T> List, Func<T, R> RemapExpression, Action<PaginationFilter<T, R>> Configuration) where T : class => new PaginationFilter<T, R>(RemapExpression, Configuration).SetData(List);

        /// <summary>
        /// Retorna um <see cref="PaginationFilter(Of T,T)"/> para a lista especificada
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static PaginationFilter<T, R> CreateFilter<T, R>(this IEnumerable<T> List, Func<T, R> RemapExpression) where T : class => new PaginationFilter<T, R>(RemapExpression).SetData(List);

        public static Type CreateNullableType(this Type type)
        {
            if (type.IsValueType && (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(object)))
            {
                return typeof(object).MakeGenericType(type);
            }

            return type;
        }

        public static MemberExpression CreatePropertyExpression<T, V>(this Expression<Func<T, V>> Property) => Expression.Property(Property.Parameters.FirstOrDefault() ?? typeof(T).GenerateParameterExpression(), Property.GetPropertyInfo());

        /// <summary>
        /// Cria uma <see cref="Expression"/> condicional a partir de um valor <see cref="Boolean"/>
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="DefaultReturnValue">Valor padrão</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateWhereExpression<T>(this bool DefaultReturnValue) => (f => DefaultReturnValue);

        /// <summary>
        /// Cria uma <see cref="Expression"/> condicional a partir de uma outra expressão
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="predicate">Valor padrão</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> CreateWhereExpression<T>(Expression<Func<T, bool>> predicate) => predicate ?? false.CreateWhereExpression<T>();

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <param name="OrderBy">
        /// Criterio que indica qual o objeto que deverá ser preservado na lista se encontrado mais
        /// de um
        /// </param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey, TOrder>(this IEnumerable<T> Items, Func<T, TKey> Property, Func<T, TOrder> OrderBy, bool Descending = false)
        => Items.GroupBy(Property).Select(x => (Descending ? x.OrderByDescending(OrderBy) : x.OrderBy(OrderBy)).FirstOrDefault());

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> Items, Func<T, TKey> Property)
        => Items.GroupBy(Property).Select(x => x.FirstOrDefault());

        /// <summary>
        /// Distingui os items de uma lista a partir de uma propriedade da classe
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <typeparam name="TKey">Tipo da propriedade</typeparam>
        /// <param name="items">Lista</param>
        /// <param name="[property]">Propriedade</param>
        /// <returns></returns>
        public static IQueryable<T> DistinctBy<T, TKey>(this IQueryable<T> Items, Expression<Func<T, TKey>> Property)
        => Items.GroupBy(Property).Select(x => x.First());

        public static IEnumerable<T> Each<T>(this IEnumerable<T> items, Action<T> Action)
        {
            if (items != null && Action != null)
            {
                foreach (var item in items)
                {
                    Action(item);
                }
            }
            return items;
        }

        /// <summary>
        /// Constroi uma expressão "igual a"
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression Equal(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.Equal(MemberExpression, ValueExpression);
        }

        public static IQueryable<T> FilterRange<T, V>(this IQueryable<T> List, Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, IEnumerable<V> Values) => List.Where(MinProperty.IsBetweenOrEqual(MaxProperty, Values));

        public static IQueryable<T> FilterRange<T, V>(this IQueryable<T> List, Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, params V[] Values) => FilterRange(List, MinProperty, MaxProperty, (Values ?? Array.Empty<V>()).AsEnumerable());

        public static T FirstOr<T>(this IEnumerable<T> source, params T[] Alternate) => source?.Any() ?? false ? source.First() : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

        /// <summary>
        /// Retorna o primeiro objeto de uma lista ou um objeto especifico se a lista estiver vazia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="Alternate"></param>
        /// <returns></returns>
        public static T FirstOr<T>(this IEnumerable<T> source, Func<T, bool> predicade, params T[] Alternate) => source?.Any(predicade) ?? false ? source.First(predicade) : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

        /// <summary>
        /// Busca em um <see cref="IQueryable{T}"/> usando uma expressao lambda a partir do nome de
        /// uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="T"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static T FirstOrDefaultExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, object PropertyValue, bool Is = true) => List.FirstOrDefault(WhereExpression<T>(PropertyName, Operator, (IEnumerable<IComparable>)PropertyValue, Is));

        /// <summary>
        /// Ajusta o tipo da expressão da direita para o tipo da esquerda, quando anulavel
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        public static void FixNullable(ref Expression e1, ref Expression e2)
        {
            var e1type = e1.Type;
            var e2type = e2.Type;
            try
            {
                e1type = ((LambdaExpression)e1).ReturnType;
            }
            catch
            {
            }

            try
            {
                e2type = ((LambdaExpression)e2).ReturnType;
            }
            catch
            {
            }

            if (e1type.IsNullableType() && !e2type.IsNullableType())
            {
                e2 = Expression.Convert(e2, e1type);
            }

            if (!e1type.IsNullableType() && e2type.IsNullableType())
            {
                e1 = Expression.Convert(e1, e2type);
            }

            if (e1.NodeType == ExpressionType.Lambda)
            {
                e1 = Expression.Invoke(e1, ((LambdaExpression)e1).Parameters);
            }

            if (e2.NodeType == ExpressionType.Lambda)
            {
                e2 = Expression.Invoke(e2, ((LambdaExpression)e2).Parameters);
            }
        }

        /// <summary>
        /// Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        /// </summary>
        /// <returns></returns>
        public static ParameterExpression GenerateParameterExpression<ClassType>() => typeof(ClassType).GenerateParameterExpression();

        /// <summary>
        /// Cria uma ParameterExpression utilizando o tipo para gerar um nome amigável
        /// </summary>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static ParameterExpression GenerateParameterExpression(this Type Type) => Expression.Parameter(Type, Type.GenerateParameterName());

        public static string GenerateParameterName(this Type Type)
        {
            if (Type != null)
            {
                return Type.Name.PascalCaseSplit().SelectJoinString(x => x.FirstOrDefault().IfBlank(InnerLibs.Ext.EmptyString), InnerLibs.Ext.EmptyString).ToLowerInvariant();
            }

            return "p";
        }

        public static FieldInfo GetFieldInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (!(GetMemberInfo(propertyLambda) is FieldInfo propInfo))
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a property, not a field.");
            }

            return propInfo;
        }

        public static MemberInfo GetMemberInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            MemberExpression member;
            switch (propertyLambda.Body)
            {
                case UnaryExpression unaryExpression:
                    member = (MemberExpression)unaryExpression.Operand;
                    break;

                default:
                    member = propertyLambda.Body as MemberExpression;
                    break;
            }
            if (member is null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
            }

            return member.Member;
        }

        /// <summary>
        /// Retorna uma expressão de comparação para um ou mais valores e uma ou mais propriedades
        /// </summary>
        /// <param name="Member"></param>
        /// <param name="[Operator]"></param>
        /// <param name="PropertyValues"></param>
        /// <param name="Conditional"></param>
        /// <returns></returns>
        public static BinaryExpression GetOperatorExpression(Expression Member, string Operator, IEnumerable<IComparable> PropertyValues, FilterConditional Conditional = FilterConditional.Or)
        {
            PropertyValues = PropertyValues ?? Array.Empty<IComparable>();
            bool comparewith = !Operator.StartsWithAny("!");
            if (comparewith == false)
            {
                Operator = Operator.TrimStartAny(false, "!");
            }

            BinaryExpression body = null;
            // Dim body As Expression = Nothing
            switch (Operator.ToLowerInvariant().IfBlank("equal"))
            {
                case "blank":
                case "compareblank":
                case "isblank":
                case "isempty":
                case "empty":
                    {
                        foreach (var item in PropertyValues)
                        {
                            var exp = Expression.Equal(Member, Expression.Constant(Ext.EmptyString, Member?.Type));
                            switch (body)
                            {
                                case null:
                                    body = exp;
                                    break;

                                default:
                                    if (Conditional == FilterConditional.And)
                                    {
                                        body = Expression.AndAlso(body, exp);
                                    }
                                    else
                                    {
                                        body = Expression.OrElse(body, exp);
                                    }

                                    break;
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "isnull":
                case "comparenull":
                case "null":
                case "nothing":
                case "isnothing":
                    {
                        foreach (var item in PropertyValues)
                        {
                            var exp = Expression.Equal(Member, Expression.Constant(null, Member.Type));
                            if (body == null)
                            {
                                body = exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "=":
                case "==":
                case "equal":
                case "===":
                case "equals":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = Member.Equal(CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case ">=":
                case "greaterthanorequal":
                case "greaterorequal":
                case "greaterequal":
                case "greatequal":
                    {
                        foreach (var ii in PropertyValues)
                        {
                            var item = ii;
                            if (!(item.GetNullableTypeOf() == typeof(DateTime)) && item.IsNotNumber() && item.ToString().IsNotBlank())
                            {
                                item = item.ToString().Length;
                            }

                            object exp = null;
                            try
                            {
                                exp = GreaterThanOrEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<=":
                case "lessthanorequal":
                case "lessorequal":
                case "lessequal":
                    {
                        foreach (var ii in PropertyValues)
                        {
                            var item = ii;
                            if (!ReferenceEquals(item.GetNullableTypeOf(), typeof(DateTime)) && item.IsNotNumber() && item.ToString().IsNotBlank())
                            {
                                item = item.ToString().Length;
                            }

                            object exp = null;
                            try
                            {
                                exp = LessThanOrEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                exp = Expression.Constant(false);
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case ">":
                case "greaterthan":
                case "greater":
                case "great":
                    {
                        foreach (var item in PropertyValues)
                        {
                            Expression exp = null;
                            try
                            {
                                exp = GreaterThan(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal(exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<":
                case "lessthan":
                case "less":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = LessThan(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "<>":
                case "notequal":
                case "different":
                    {
                        foreach (var item in PropertyValues)
                        {
                            object exp = null;
                            try
                            {
                                exp = NotEqual(Member, CreateConstant(Member, item));
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, (Expression)exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, (Expression)exp);
                            }

                            if (comparewith == false)
                            {
                                body = Expression.Equal((Expression)exp, Expression.Constant(false));
                            }
                        }

                        break;
                    }

                case "betweenequal":
                case "betweenorequal":
                case "btweq":
                case "=><=":
                    {
                        if (PropertyValues.Count() > 1)
                        {
                            if (Member.Type == typeof(string))
                            {
                                body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", !comparewith), new[] { PropertyValues.First() }, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", !comparewith), new[] { PropertyValues.Last() }, Conditional));
                            }
                            else
                            {
                                var ge = GetOperatorExpression(Member, "greaterequal".PrependIf("!", !comparewith), new[] { PropertyValues.Min() }, Conditional);
                                var le = GetOperatorExpression(Member, "lessequal".PrependIf("!", !comparewith), new[] { PropertyValues.Max() }, Conditional);
                                body = Expression.And(ge, le);
                            }
                        }
                        else
                        {
                            body = GetOperatorExpression(Member, "=".PrependIf("!", !comparewith), PropertyValues, Conditional);
                        }

                        break;
                    }

                case "><":
                case "startend":
                case "startends":
                case "btw":
                case "between":
                    {
                        if (PropertyValues.Count() > 1)
                        {
                            switch (Member.Type)
                            {
                                case var case1 when case1 == typeof(string):
                                    {
                                        body = Expression.And(GetOperatorExpression(Member, "starts".PrependIf("!", !comparewith), new[] { PropertyValues.First() }, Conditional), GetOperatorExpression(Member, "ends".PrependIf("!", !comparewith), new[] { PropertyValues.Last() }, Conditional));
                                        break;
                                    }

                                default:
                                    {
                                        body = Expression.And(GetOperatorExpression(Member, "greater".PrependIf("!", !comparewith), new[] { PropertyValues.Min() }, Conditional), GetOperatorExpression(Member, "less".PrependIf("!", !comparewith), new[] { PropertyValues.Max() }, Conditional));
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            body = GetOperatorExpression(Member, "=".PrependIf("!", !comparewith), PropertyValues, Conditional);
                        }

                        break;
                    }

                case "starts":
                case "start":
                case "startwith":
                case "startswith":
                    {
                        switch (Member.Type)
                        {
                            case var case2 when case2 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, startsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, ">=", PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "ends":
                case "end":
                case "endwith":
                case "endswith":
                    {
                        switch (Member.Type)
                        {
                            case var case3 when case3 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, endsWithMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, "lessequal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "like":
                case "contains":
                    {
                        switch (Member.Type)
                        {
                            case var case4 when case4 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Member, containsMethod, Expression.Constant(item.ToString())), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "isin":
                case "inside":
                    {
                        switch (Member.Type)
                        {
                            case var case5 when case5 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    // 'TODO: implementar busca de array de inteiro,data etc
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                case "cross":
                case "crosscontains":
                case "insidecontains":
                    {
                        switch (Member.Type)
                        {
                            case var case6 when case6 == typeof(string):
                                {
                                    foreach (var item in PropertyValues)
                                    {
                                        object exp = null;
                                        try
                                        {
                                            exp = Expression.Equal(Expression.OrElse(Expression.Call(Expression.Constant(item.ToString()), containsMethod, Member), Expression.Call(Member, containsMethod, Expression.Constant(item.ToString()))), Expression.Constant(comparewith));
                                        }
                                        catch
                                        {
                                            continue;
                                        }

                                        if (body == null)
                                        {
                                            body = (BinaryExpression)exp;
                                        }
                                        else if (Conditional == FilterConditional.And)
                                        {
                                            body = Expression.AndAlso(body, (Expression)exp);
                                        }
                                        else
                                        {
                                            body = Expression.OrElse(body, (Expression)exp);
                                        }

                                        if (comparewith == false)
                                        {
                                            body = Expression.Equal((Expression)exp, Expression.Constant(false));
                                        }
                                    }

                                    break;
                                }

                            default:
                                {
                                    //TODO: implementar busca de array de inteiro,data etc
                                    body = GetOperatorExpression(Member, "equal".PrependIf("!", !comparewith), PropertyValues, Conditional);
                                    break;
                                }
                        }

                        break;
                    }

                default: // Executa um metodo com o nome definido pelo usuario que retorna uma expression compativel
                    {
                        try
                        {
                            var metodo = Member.Type.GetMethods().FirstOrDefault(x => (x.Name.ToLowerInvariant() ?? InnerLibs.Ext.EmptyString) == (Operator.ToLowerInvariant() ?? InnerLibs.Ext.EmptyString));
                            Expression exp = (Expression)metodo.Invoke(null, new[] { PropertyValues });
                            exp = Expression.Equal(Expression.Invoke(exp, new[] { Member }), Expression.Constant(comparewith));
                            if (body == null)
                            {
                                body = (BinaryExpression)exp;
                            }
                            else if (Conditional == FilterConditional.And)
                            {
                                body = Expression.AndAlso(body, exp);
                            }
                            else
                            {
                                body = Expression.OrElse(body, exp);
                            }
                        }
                        catch
                        {
                        }

                        break;
                    }
            }

            return body;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var propInfo = GetMemberInfo(propertyLambda) as PropertyInfo;
            if (propInfo is null)
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()));
            }

            return propInfo;
        }

        /// <summary>
        /// Retorna as informacoes de uma propriedade a partir de um seletor
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this TSource source, Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);
            if (!(propertyLambda.Body is MemberExpression member))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda.ToString()));
            }

            if (!(member.Member is PropertyInfo propInfo))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()));
            }

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
            {
                throw new ArgumentException(string.Format("Expression '{0}' refers to a property that is not from type {1}.", propertyLambda.ToString(), type));
            }

            return propInfo;
        }

        /// <summary>
        /// Constroi uma expressão Maior que
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression GreaterThan(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.GreaterThan(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Constroi uma expressão Maior ou Igual
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression GreaterThanOrEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.GreaterThanOrEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        /// </summary>
        /// <typeparam name="Tsource"></typeparam>
        /// <param name="source"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Dictionary<long, IEnumerable<Tsource>> GroupByPage<Tsource>(this IQueryable<Tsource> source, int PageSize) => source.AsEnumerable().GroupByPage(PageSize);

        /// <summary>
        /// Criar um <see cref="Dictionary"/> agrupando os itens em páginas de um tamanho especifico
        /// </summary>
        /// <typeparam name="Tsource"></typeparam>
        /// <param name="source"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static Dictionary<long, IEnumerable<Tsource>> GroupByPage<Tsource>(this IEnumerable<Tsource> source, int PageSize) => source.Select((item, index) => new { item, Page = index / (double)PageSize.SetMinValue(1) }).GroupBy(g => g.Page.FloorLong() + 1L, x => x.item).ToDictionary();

        public static Expression<Func<T, bool>> IsEqual<T, V>(this Expression<Func<T, V>> Property, V Value) => WhereExpression(Property, "equal", new[] { (IComparable)Value });

        /// <summary>
        /// Retorna o ultimo objeto de uma lista ou um objeto especifico se a lista estiver vazia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="Alternate"></param>
        /// <returns></returns>
        public static T LastOr<T>(this IEnumerable<T> source, params T[] Alternate) => source?.Any() ?? false ? source.Last() : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

        public static T LastOr<T>(this IEnumerable<T> source, Func<T, bool> predicade, params T[] Alternate) => source?.Any(predicade) ?? false ? source.Last(predicade) : (Alternate ?? Array.Empty<T>()).AsEnumerable().NullCoalesce();

        /// <summary>
        /// Constroi uma expressão Menor que
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression LessThan(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.LessThan(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Constroi uma expressão Menor ou igual
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression LessThanOrEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.LessThanOrEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool Most<T>(this IEnumerable<T> List, Func<T, bool> predicate, bool Result = true) => List.Select(predicate).Most(Result);

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem o valor correspondente
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool Most(this IEnumerable<bool> List, bool Result = true)
        {
            if (List.Count() > 0)
            {
                var arr = List.DistinctCount();
                if (arr.ContainsKey(true) && arr.ContainsKey(false))
                {
                    return arr[Result] > arr[!Result];
                }
                else
                {
                    return arr.First().Key == Result;
                }
            }

            return false == Result;
        }

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem false
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool MostFalse<T>(this IEnumerable<T> List, Func<T, bool> predicate) => MostFalse(List.Select(predicate).ToArray());

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem FALSE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool MostFalse(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).Most(false);

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem true
        /// </summary>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool MostTrue<T>(this IEnumerable<T> List, Func<T, bool> predicate) => MostTrue(List.Select(predicate).ToArray());

        /// <summary>
        /// Retorna TRUE se a maioria dos testes em uma lista retornarem TRUE
        /// </summary>
        /// <param name="Tests"></param>
        /// <returns></returns>
        public static bool MostTrue(params bool[] Tests) => (Tests ?? Array.Empty<bool>()).Most(true);

        /// <summary>
        /// Constroi uma expressão diferente de
        /// </summary>
        /// <param name="MemberExpression"></param>
        /// <param name="ValueExpression"></param>
        /// <returns></returns>
        public static BinaryExpression NotEqual(this Expression MemberExpression, Expression ValueExpression)
        {
            FixNullable(ref MemberExpression, ref ValueExpression);
            return Expression.NotEqual(MemberExpression, ValueExpression);
        }

        /// <summary>
        /// Concatena uma expressão com outra usando o operador OR (||)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstExpression"></param>
        /// <param name="OtherExpressions"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> FirstExpression, params Expression<Func<T, bool>>[] OtherExpressions)
        {
            FirstExpression = FirstExpression ?? false.CreateWhereExpression<T>();
            foreach (var item in OtherExpressions ?? Array.Empty<Expression<Func<T, bool>>>())
            {
                if (item != null)
                {
                    var invokedExpr = Expression.Invoke(item, FirstExpression.Parameters.Cast<Expression>());
                    FirstExpression = Expression.Lambda<Func<T, bool>>(Expression.OrElse(FirstExpression.Body, invokedExpr), FirstExpression.Parameters);
                }
            }

            return FirstExpression;
        }

        /// <summary>
        /// Orderna uma lista a partir da aproximação de um deerminado campo com uma string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="PropertySelector"></param>
        /// <param name="Ascending"></param>
        /// <param name="Searches"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, params string[] Searches) where T : class => items.ThenByLike(PropertySelector, Ascending, Searches);

        public static IOrderedEnumerable<T> OrderByMany<T>(this IEnumerable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(true, Selectors);

        public static IOrderedEnumerable<T> OrderByMany<T>(this IEnumerable<T> Data, bool Ascending, params Expression<Func<T, object>>[] Selectors)
        {
            Selectors = Selectors ?? Array.Empty<Expression<Func<T, object>>>();
            if (Selectors.IsNullOrEmpty())
            {
                Expression<Func<T, object>> dd = x => true;
                Selectors = new[] { dd };
            }
            foreach (var Selector in Selectors)
            {
                if (Selector != null)
                {
                    if (Data is IOrderedEnumerable<T> datav)
                    {
                        Data = Ascending ? datav.ThenBy(Selector.Compile()) : (IEnumerable<T>)datav.ThenByDescending(Selector.Compile());
                    }
                    else if (Data is IEnumerable<T>)
                    {
                        Data = Ascending ? Data.OrderBy(Selector.Compile()) : (IEnumerable<T>)Data.OrderByDescending(Selector.Compile());
                    }
                }
            }

            return (IOrderedEnumerable<T>)Data;
        }

        public static IOrderedQueryable<T> OrderByMany<T>(this IQueryable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(true, Selectors);

        public static IOrderedQueryable<T> OrderByMany<T>(this IQueryable<T> Data, bool Ascending, params Expression<Func<T, object>>[] Selectors)
        {
            Selectors = Selectors ?? Array.Empty<Expression<Func<T, object>>>();
            if (Selectors.IsNullOrEmpty())
            {
                Expression<Func<T, object>> dd = x => true;
                Selectors = new[] { dd };
            }
            foreach (var Selector in Selectors)
            {
                if (Selector != null)
                {
                    if (Data is IOrderedQueryable<T> datav)
                    {
                        Data = Ascending ? datav.ThenBy(Selector) : (IQueryable<T>)datav.ThenByDescending(Selector);
                    }
                    else if (Data is IQueryable<T>)
                    {
                        Data = Ascending ? Data.OrderBy(Selector) : (IQueryable<T>)Data.OrderByDescending(Selector);
                    }
                }
            }

            return (IOrderedQueryable<T>)Data;
        }

        public static IOrderedEnumerable<T> OrderByManyDescending<T>(this IEnumerable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(false, Selectors);

        public static IOrderedQueryable<T> OrderByManyDescending<T>(this IQueryable<T> Data, params Expression<Func<T, object>>[] Selectors) => Data.OrderByMany(false, Selectors);

        /// <summary>
        /// Order a list following another list order
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="Items"></param>
        /// <param name="Property"></param>
        /// <param name="Order"></param>
        /// <returns></returns>
        public static IEnumerable<T> OrderByPredefinedOrder<T, TOrder>(this IEnumerable<T> Source, Expression<Func<T, TOrder>> PropertySelector, params TOrder[] order)
        {
            Source = Source ?? Array.Empty<T>();
            if (PropertySelector == null) throw new ArgumentException("Property is null");
            var p = PropertySelector.Compile();
            var lookup = Source.ToLookup(p, t => t);
            if (order.IsNotNullOrEmpty())
            {
                foreach (var id in order)
                {
                    foreach (var t in lookup[id])
                    {
                        yield return t;
                    }
                }
            }
            else
            {
                foreach (var item in Source)
                {
                    yield return item;
                }
            }
        }

        public static IOrderedQueryable<T> OrderByProperty<T>(this IQueryable<T> source, string[] SortProperty, bool Ascending = true) => ThenByProperty(source, SortProperty, Ascending);

        public static IOrderedEnumerable<T> OrderByProperty<T>(this IEnumerable<T> source, string[] SortProperty, bool Ascending = true) => ThenByProperty(source, SortProperty, Ascending);

        /// <summary>
        /// Randomiza a ordem de um <see cref="IEnumerable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByRandom<T>(this IEnumerable<T> items) => items.OrderBy(x => Guid.NewGuid());

        /// <summary>
        /// Randomiza a ordem de um <see cref="IQueryable"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByRandom<T>(this IQueryable<T> items) => items.OrderBy(x => Guid.NewGuid());

        /// <summary>
        /// Ordena um <see cref="IEnumerable"/> priorizando valores especificos a uma condição no
        /// inicio da coleção e então segue a ordem padrão para os outros.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="DefaultOrderItem"></typeparam>
        /// <param name="items">colecao</param>
        /// <param name="Priority">Seletores que define a prioridade da ordem dos itens</param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> OrderByWithPriority<T>(this IEnumerable<T> items, params Func<T, bool>[] Priority)
        {
            if (items != null)
            {
                Priority = Priority ?? Array.Empty<Func<T, bool>>();

                return items.OrderByDescending(x => Priority.Sum(y => y(x).ToInt()));
            }
            return null;
        }

        public static Expression<Func<T, bool>> OrSearch<T>(this Expression<Func<T, bool>> FirstExpression, IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? false.CreateWhereExpression<T>()).Or(Text.SearchExpression(Properties));

        public static Expression<Func<T, bool>> OrSearch<T>(this Expression<Func<T, bool>> FirstExpression, string Text, params Expression<Func<T, string>>[] Properties)
               => (FirstExpression ?? false.CreateWhereExpression<T>()).Or(Text.SearchExpression(Properties));

        /// <summary>
        /// Reduz um <see cref="IQueryable"/> em uma página especifica
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> Source, int PageNumber, int PageSize) => PageNumber <= 0 ? Source : Source.Skip((PageNumber - 1) * PageSize).Take(PageSize);

        /// <summary>
        /// Reduz um <see cref="IEnumerable"/> em uma página especifica
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> Source, int PageNumber, int PageSize)
        {
            if (PageNumber <= 0)
            {
                return Source;
            }

            return Source.Skip((PageNumber - 1).SetMinValue(0) * PageSize).Take(PageSize);
        }

        public static IEnumerable<(T, T)> PairUp<T>(this IEnumerable<T> source)
        {
            if (source != null)
                using (var iterator = source.GetEnumerator())
                    while (iterator.MoveNext())
                        yield return (iterator.Current, iterator.MoveNext() ? iterator.Current : default);
        }

        public static Expression PropertyExpression(this ParameterExpression Parameter, string PropertyName)
        {
            Expression prop = Parameter;
            if (PropertyName.IfBlank("this") != "this")
            {
                foreach (var name in PropertyName.SplitAny(".", "/"))
                {
                    prop = Expression.Property(prop, name);
                }
            }

            return prop;
        }

        public static T RandomItem<T>(this IEnumerable<T> l) => l.RandomItemOr();

        public static T RandomItem<T>(this IEnumerable<T> l, Func<T, bool> predicade) => l.RandomItemOr(predicade);

        public static T RandomItemOr<T>(this IEnumerable<T> l, params T[] Alternate) => l.TakeRandom().FirstOr(Alternate);

        public static T RandomItemOr<T>(this IEnumerable<T> l, Func<T, bool> predicade, params T[] Alternate) => l.TakeRandom(predicade).FirstOr(Alternate);

        /// <summary>
        /// Rankeia um <see cref="IEnumerable{TObject}"/> a partir de uma propriedade definida por
        /// <paramref name="ValueSelector"/> guardando sua posição no <paramref name="RankSelector"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TRank"></typeparam>
        /// <param name="values"></param>
        /// <param name="ValueSelector"></param>
        /// <param name="RankSelector"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<TObject> Rank<TObject, TValue, TRank>(this IEnumerable<TObject> values, Expression<Func<TObject, TValue>> ValueSelector, Expression<Func<TObject, TRank>> RankSelector) where TObject : class where TValue : IComparable where TRank : IComparable
        {
            if (values != null)
            {
                if (ValueSelector != null)
                {
                    values = values.OrderByDescending(ValueSelector.Compile());

                    if (RankSelector != null)
                    {
                        var filtered = values.Select(ValueSelector.Compile()).Distinct().ToList();
                        foreach (TObject item in values)
                        {
                            item.SetPropertyValue(RankSelector, (filtered.IndexOf((ValueSelector.Compile().Invoke(item))) + 1).ChangeType<TRank>());
                        }
                        return values.OrderBy(RankSelector.Compile());
                    }
                    return values as IOrderedEnumerable<TObject>;
                }
                return values.OrderBy(x => 0);
            }
            return null;
        }

        public static List<T> RemoveWhere<T>(this List<T> list, Expression<Func<T, bool>> predicate)
        {
            if (list != null)
            {
                if (predicate != null)
                {
                    while (true)
                    {
                        var obj = list.FirstOrDefault(predicate.Compile());
                        if (obj != null)
                        {
                            list.Remove(obj);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Retorna um <see cref="IQueryable{T}"/> procurando em varios campos diferentes de uma entidade
        /// </summary>
        /// <typeparam name="T">Tipo da Entidade</typeparam>
        /// <param name="Table">Tabela da Entidade</param>
        /// <param name="SearchTerms">Termos da pesquisa</param>
        /// <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        /// <returns></returns>
        public static IQueryable<T> Search<T>(this IQueryable<T> Table, IEnumerable<string> SearchTerms, params Expression<Func<T, string>>[] Properties) where T : class
        {
            Properties = Properties ?? Array.Empty<Expression<Func<T, string>>>();
            SearchTerms = SearchTerms ?? Array.Empty<string>().AsEnumerable();
            return Table.Where(SearchTerms.SearchExpression(Properties));
        }

        public static IQueryable<T> Search<T>(this IQueryable<T> Table, string SearchTerm, params Expression<Func<T, string>>[] Properties) where T : class => Search(Table, new[] { SearchTerm }, Properties);

        public static Expression<Func<T, bool>> SearchExpression<T>(this IEnumerable<string> Text, params Expression<Func<T, string>>[] Properties)
        {
            Properties = Properties ?? Array.Empty<Expression<Func<T, string>>>();
            Text = Text?.WhereNotBlank() ?? Array.Empty<string>();

            var predi = (!Text.Any()).CreateWhereExpression<T>();

            foreach (var prop in Properties)
            {
                foreach (var s in Text)
                {
                    if (s.IsNotBlank())
                    {
                        var param = prop.Parameters.First();
                        var con = Expression.Constant(s);
                        var lk = Expression.Call(prop.Body, containsMethod, con);
                        var lbd = Expression.Lambda<Func<T, bool>>(lk, param);
                        predi = predi.Or(lbd);
                    }
                }
            }

            return predi;
        }

        public static Expression<Func<T, bool>> SearchExpression<T>(this string Text, params Expression<Func<T, string>>[] Properties)
        => (new[] { Text }).SearchExpression(Properties);

        /// <summary>
        /// Retorna um <see cref="IQueryable(Of ClassType)"/> procurando em varios campos diferentes
        /// de uma entidade
        /// </summary>
        /// <typeparam name="TClass">Tipo da Entidade</typeparam>
        /// <param name="Table">Tabela da Entidade</param>
        /// <param name="SearchTerms">Termos da pesquisa</param>
        /// <param name="Properties">Propriedades onde <paramref name="SearchTerms"/> serão procurados</param>
        /// <returns></returns>
        public static IOrderedEnumerable<TClass> SearchInOrder<TClass>(this IEnumerable<TClass> Table, IEnumerable<string> SearchTerms, bool Ascending, params Expression<Func<TClass, string>>[] Properties) where TClass : class
        {
            IOrderedEnumerable<TClass> SearchRet = default;
            Properties = Properties ?? Array.Empty<Expression<Func<TClass, string>>>();
            SearchTerms = SearchTerms ?? Array.Empty<string>().AsEnumerable();
            SearchRet = null;
            Table = Table.Where(SearchTerms.SearchExpression(Properties).Compile());
            foreach (var prop in Properties)
            {
                SearchRet = (SearchRet ?? Table.OrderBy(x => true)).ThenByLike(prop.Compile(), Ascending, SearchTerms.ToArray());
            }

            return SearchRet;
        }

        public static IOrderedEnumerable<TClass> SearchInOrder<TClass>(this IEnumerable<TClass> Table, IEnumerable<string> SearchTerms, params Expression<Func<TClass, string>>[] Properties) where TClass : class => SearchInOrder(Table, SearchTerms, true, Properties);

        public static IOrderedQueryable<TClass> SearchInOrder<TClass>(this IQueryable<TClass> Table, IEnumerable<string> SearchTerms, params Expression<Func<TClass, string>>[] Properties) where TClass : class
        {
            var SearchRet = Table.Search(SearchTerms, Properties).OrderBy(x => true);
            foreach (var prop in Properties)
            {
                SearchRet = SearchRet.ThenByLike(SearchTerms, prop);
            }

            return SearchRet;
        }

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectJoinString<TSource>(this IEnumerable<TSource> Source, string Separator = InnerLibs.Ext.EmptyString) => Source.SelectJoinString(null, Separator);

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectJoinString<TSource>(this IEnumerable<TSource> Source, Func<TSource, string> Selector, string Separator = InnerLibs.Ext.EmptyString)
        {
            Selector = Selector ?? (x => $"{x}");
            Source = Source ?? Array.Empty<TSource>();
            return Source.Any() ? String.Join(Separator, Source.Select(Selector).ToArray()) : InnerLibs.Ext.EmptyString;
        }

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos enumeraveis
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectManyJoinString<TSource>(this IEnumerable<TSource> Source, Func<TSource, IEnumerable<string>> Selector = null, string Separator = InnerLibs.Ext.EmptyString) => SelectJoinString(Source.SelectMany(Selector ?? (x => (new[] { x.ToString() }))), Separator);

        /// <summary>
        /// Seleciona e une em uma unica string varios elementos enumeraveis
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="Source"></param>
        /// <param name="Selector"></param>
        /// <param name="Separator"></param>
        /// <returns></returns>
        public static string SelectManyJoinString<TSource>(this IQueryable<TSource> Source, Func<TSource, IEnumerable<string>> Selector = null, string Separator = InnerLibs.Ext.EmptyString) => Source.AsEnumerable().SelectManyJoinString(Selector, Separator);

        /// <summary>
        /// Busca em um <see cref="IQueryable{T}"/> usando uma expressao lambda a partir do nome de
        /// uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="T"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static T SingleOrDefaultExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, object PropertyValue, bool Is = true) => List.SingleOrDefault(WhereExpression<T>(PropertyName, Operator, (IEnumerable<IComparable>)PropertyValue, Is));

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> l, int Count = 1)
        => l.Take(l.Count() - Count.LimitRange(0, l.Count()));

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> l, int Count = 1) => l.Reverse().Take(Count).Reverse();

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> l, int Count = 1) => l.OrderByRandom().Take(Count);

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> l, Func<T, bool> predicade, int Count = 1) => l.Where(predicade).OrderByRandom().Take(Count);

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximação de uma ou mais <see
        /// cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="Searches"></param>
        /// <param name="SortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByLike<T>(this IQueryable<T> items, string[] Searches, string SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            Searches = Searches ?? Array.Empty<string>();
            if (Searches.Any())
            {
                foreach (var t in Searches)
                {
                    var property = type.GetProperty(SortProperty);
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);
                    var testes = new[] { Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)), Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)), Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)), Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t)) };
                    foreach (var exp in testes)
                    {
                        var nv = Expression.Lambda<Func<T, bool>>(exp, parameter);
                        if (Ascending)
                        {
                            if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                            {
                                items = items.OrderByDescending(nv);
                            }
                            else
                            {
                                items = ((IOrderedQueryable<T>)items).ThenByDescending(nv);
                            }
                        }
                        else if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                        {
                            items = items.OrderBy(nv);
                        }
                        else
                        {
                            items = ((IOrderedQueryable<T>)items).ThenBy(nv);
                        }
                    }
                }
            }
            else
            {
                items = items.OrderBy(x => 0);
            }

            return (IOrderedQueryable<T>)items;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximação de uma ou mais <see
        /// cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="Searches"></param>
        /// <param name="SortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByLike<T>(this IQueryable<T> items, IEnumerable<string> Searches, Expression<Func<T, string>> SortProperty, bool Ascending = true)
        {
            var type = typeof(T);
            Searches = Searches ?? Array.Empty<string>();
            if (items != null)
            {
                if (Searches.Any() && SortProperty != null)
                {
                    foreach (var t in Searches)
                    {
                        MemberExpression mem = SortProperty.Body as MemberExpression;
                        var property = mem.Member;
                        var parameter = SortProperty.Parameters.First();
                        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                        var orderByExp = Expression.Lambda(propertyAccess, parameter);
                        var tests = new[] { Expression.Call(propertyAccess, equalMethod, Expression.Constant(t)), Expression.Call(propertyAccess, startsWithMethod, Expression.Constant(t)), Expression.Call(propertyAccess, containsMethod, Expression.Constant(t)), Expression.Call(propertyAccess, endsWithMethod, Expression.Constant(t)) };
                        foreach (var exp in tests)
                        {
                            var nv = Expression.Lambda<Func<T, bool>>(exp, parameter);
                            if (Ascending)
                            {
                                if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                                {
                                    items = items.OrderByDescending(nv);
                                }
                                else
                                {
                                    items = ((IOrderedQueryable<T>)items).ThenByDescending(nv);
                                }
                            }
                            else if (!ReferenceEquals(items.GetType(), typeof(IOrderedQueryable<T>)))
                            {
                                items = items.OrderBy(nv);
                            }
                            else
                            {
                                items = ((IOrderedQueryable<T>)items).ThenBy(nv);
                            }
                        }
                    }
                }
                else
                {
                    items = items.OrderBy(x => 0);
                }
                return (IOrderedQueryable<T>)items;
            }
            throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir da aproximação de uma ou mais <see
        /// cref="String"/> com o valor de um determinado campo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="PropertySelector"></param>
        /// <param name="Ascending"></param>
        /// <param name="Searches"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, params string[] Searches) where T : class => ThenByLike(items, PropertySelector, Ascending, StringComparison.InvariantCultureIgnoreCase, Searches);

        public static IOrderedEnumerable<T> ThenByLike<T>(this IEnumerable<T> items, Func<T, string> PropertySelector, bool Ascending, StringComparison Comparison, params string[] Searches) where T : class
        {
            Searches = Searches ?? Array.Empty<string>();
            if (items != null)
            {
                IOrderedEnumerable<T> newitems = items is IOrderedEnumerable<T> oitems ? oitems : items.OrderBy(x => 0);

                if (Searches.Any())
                {
                    Searches.Each(s =>
                    {
                        bool func(T x) => PropertySelector.Invoke(x).Equals(s, Comparison);
                        newitems = Ascending ? newitems.ThenByDescending(func) : newitems.ThenBy(func);
                    });

                    Searches.Each(s =>
                    {
                        int func(T x) => PropertySelector.Invoke(x).IndexOf(s, Comparison);
                        newitems = Ascending ? newitems.ThenBy(func) : newitems.ThenByDescending(func);
                    });
                }

                return newitems;
            }
            throw new ArgumentNullException(nameof(items));
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable(Of T)"/> a partir de outra lista do mesmo tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source"></param>
        /// <param name="OrderSource"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByList<T>(this IOrderedEnumerable<T> Source, params T[] OrderSource)
        => Source.ThenBy(d => Array.IndexOf(OrderSource, d));

        /// <summary>
        /// Ordena um <see cref="IQueryable(Of T)"/> a partir do nome de uma ou mais propriedades
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByProperty<T>(this IQueryable<T> source, string[] SortProperty, bool Ascending = true)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var type = typeof(T);
            SortProperty = SortProperty ?? Array.Empty<string>();
            foreach (var prop in SortProperty)
            {
                var property = type.FindProperty(prop);
                var parameter = Expression.Parameter(type, "p");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var typeArguments = new Type[] { type, property.PropertyType };
                string methodname = source.GetType() == typeof(IOrderedQueryable<T>)
                    ? Ascending ? "ThenBy" : "ThenByDescending"
                    : Ascending ? "OrderBy" : "OrderByDescending";
                var resultExp = Expression.Call(typeof(Queryable), methodname, typeArguments, source.Expression, Expression.Quote(orderByExp));
                source = source.Provider.CreateQuery<T>(resultExp);
            }
            return source.GetType() != typeof(IOrderedQueryable<T>) ? source.OrderBy(x => true) : (IOrderedQueryable<T>)source;
        }

        /// <summary>
        /// Ordena um <see cref="IEnumerable{T}"/> a partir do nome de uma ou mais propriedades
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortProperty"></param>
        /// <param name="Ascending"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<T> ThenByProperty<T>(this IEnumerable<T> source, string[] SortProperty, bool Ascending = true)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var type = typeof(T);
            SortProperty = SortProperty ?? Array.Empty<string>();
            foreach (var prop in SortProperty)
            {
                var propInfo = Ext.FindProperty(typeof(T), prop);

                if (propInfo != null)
                {
                    object exp(T x) => propInfo.GetValue(x);
                    if (source.GetType() == typeof(IOrderedEnumerable<T>))
                    {
                        source = Ascending ? ((IOrderedEnumerable<T>)source).ThenBy(exp) : ((IOrderedEnumerable<T>)source).ThenByDescending(exp);
                    }
                    else
                    {
                        source = Ascending ? source.OrderBy(exp) : source.OrderByDescending(exp);
                    }
                }
            }

            return source.GetType() != typeof(IOrderedEnumerable<T>) ? source.OrderBy(x => true) : (IOrderedEnumerable<T>)source;
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Items">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> ChildSelector)
        {
            var stack = new Stack<T>(items.Reverse());
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                foreach (var child in ChildSelector(next))
                {
                    stack.Push(child);
                }
            }
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ParentSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this T item, Func<T, T> ParentSelector)
        {
            if (item != null)
            {
                var current = item;
                do
                {
                    yield return current;
                    current = ParentSelector(current);
                }
                while (current != null);
            }
        }

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<T> Traverse<T>(this T Item, Func<T, IEnumerable<T>> ChildSelector, bool IncludeMe = false)
        => ChildSelector(Item).Union(IncludeMe ? (new[] { Item }) : Array.Empty<T>()).Traverse(ChildSelector);

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, P> PropertySelector, bool IncludeMe = false) => Item.Traverse(ChildSelector, IncludeMe).Select(PropertySelector);

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, IEnumerable<P>> PropertySelector, bool IncludeMe = false) => Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector);

        /// <summary>
        /// Percorre uma Lista de objetos que possuem como propriedade objetos do mesmo tipo e as
        /// unifica recursivamente expondo uma outra propriedade
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto</typeparam>
        /// <param name="Item">Itens</param>
        /// <param name="ChildSelector">Seletor das propriedades filhas</param>
        /// <returns></returns>
        public static IEnumerable<P> Traverse<T, P>(this T Item, Func<T, IEnumerable<T>> ChildSelector, Func<T, IQueryable<P>> PropertySelector, bool IncludeMe = false) => Item.Traverse(ChildSelector, IncludeMe).SelectMany(PropertySelector);

        /// <summary>
        /// Gera uma expressao lambda a partir do nome de uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="Type">Tipo do objeto acessado</typeparam>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="Type"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="Type"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="Type"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static Expression<Func<Type, bool>> WhereExpression<Type>(string PropertyName, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, FilterConditional Conditional = FilterConditional.Or)
        {
            var parameter = GenerateParameterExpression<Type>();
            var member = parameter.PropertyExpression(PropertyName);
            Expression body = GetOperatorExpression(member, Operator, PropertyValue, Conditional);
            body = Expression.Equal(body, Expression.Constant(Is));
            var finalExpression = Expression.Lambda<Func<Type, bool>>(body, parameter);
            return finalExpression;
        }

        public static Expression<Func<Type, bool>> WhereExpression<Type, V>(Expression<Func<Type, V>> PropertySelector, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, FilterConditional Conditional = FilterConditional.Or)
        {
            var parameter = GenerateParameterExpression<Type>();
            string member = String.Join(".", PropertySelector.Body.ToString().Split(".").Skip(1));
            var prop = parameter.PropertyExpression(member);
            Expression body = GetOperatorExpression(prop, Operator, PropertyValue, Conditional);
            body = Expression.Equal(body, Expression.Constant(Is));
            var finalExpression = Expression.Lambda<Func<Type, bool>>(body, parameter);
            return finalExpression;
        }

        /// <summary>
        /// Busca em um <see cref="IQueryable(Of T)"/> usando uma expressao lambda a partir do nome
        /// de uma propriedade, uma operacao e um valor
        /// </summary>
        /// <typeparam name="T">Tipo do objeto acessado</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="PropertyName">Propriedade do objeto <typeparamref name="T"/></param>
        /// <param name="[Operator]">
        /// Operador ou método do objeto <typeparamref name="T"/> que retorna um <see cref="Boolean"/>
        /// </param>
        /// <param name="PropertyValue">
        /// Valor da propriedade comparado com o <paramref name="Operator"/> ou como o primeiro
        /// argumento do método de mesmo nome definido em <typeparamref name="T"/>
        /// </param>
        /// <param name="[Is]">Compara o resultado com TRUE ou FALSE</param>
        /// <returns></returns>
        public static IQueryable<T> WhereExpression<T>(this IQueryable<T> List, string PropertyName, string Operator, IEnumerable<IComparable> PropertyValue, bool Is = true, bool Exclusive = true) => List.Where(WhereExpression<T>(PropertyName, Operator, PropertyValue, Is));

        public static IEnumerable<T> WhereNotBlank<T>(this IEnumerable<T> List) => List.Where(x => x != null && $"{x}".IsNotBlank());

        public static IQueryable<T> WhereNotNull<T>(this IQueryable<T> List) => List.Where(x => x != null);

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> List) => List.Where(x => x != null);

        public static IEnumerable<Type> WhereType<BaseType, Type>(this IEnumerable<BaseType> List) where Type : BaseType => List.Where(x => x is Type).Cast<Type>();

        #endregion Public Methods

        #region FilterDateRange

        public static IEnumerable<T> FilterDateRange<T>(this IEnumerable<T> List, Expression<Func<T, DateTime>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null)
        => List.Where(x => Range.Contains(Property.Compile()(x), FilterBehavior ?? Range.FilterBehavior));

        public static IEnumerable<T> FilterDateRange<T>(this IEnumerable<T> List, Expression<Func<T, DateTime?>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null)
        => List.Where(x => Range.Contains(Property.Compile()(x), FilterBehavior ?? Range.FilterBehavior));

        public static IQueryable<T> FilterDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null) => List.Where(Property.IsInDateRange(Range));

        public static IQueryable<T> FilterDateRange<T>(this IQueryable<T> List, Expression<Func<T, DateTime?>> Property, DateRange Range, DateRangeFilterBehavior? FilterBehavior = null) => List.Where(Property.IsInDateRange(Range));

        #endregion FilterDateRange

        #region IsBetween

        public static Expression<Func<T, bool>> IsBetween<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, IEnumerable<V> Values)
        {
            var exp = false.CreateWhereExpression<T>();
            foreach (var item in Values ?? Array.Empty<V>())
            {
                exp = exp.Or(WhereExpression(MinProperty, "<", new[] { (IComparable)item }).And(WhereExpression(MaxProperty, ">", new[] { (IComparable)item })));
            }

            return exp;
        }

        public static Expression<Func<T, bool>> IsBetween<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, params V[] Values)
        => MinProperty.IsBetween(MaxProperty, Values.AsEnumerable());

        public static Expression<Func<T, bool>> IsBetween<T, V>(this Expression<Func<T, V>> Property, V MinValue, V MaxValue)
        {
            if (MinValue.Equals(MaxValue))
            {
                return IsEqual(Property, MinValue);
            }
            else
            {
                return WhereExpression(Property, "between", new[] { (IComparable)MinValue, (IComparable)MaxValue });
            }
        }

        public static Expression<Func<T, bool>> IsBetween<T>(this Expression<Func<T, DateTime>> Property, DateRange DateRange)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, DateRange.StartDate);
            }
            else
            {
                return WhereExpression(Property, "between", new IComparable[] { DateRange.StartDate, DateRange.EndDate });
            }
        }

        public static Expression<Func<T, bool>> IsBetween<T>(this Expression<Func<T, DateTime?>> Property, DateRange DateRange)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, DateRange.StartDate);
            }
            else
            {
                return WhereExpression(Property, "between", new IComparable[] { (DateTime?)DateRange.StartDate, (DateTime?)DateRange.EndDate });
            }
        }

        #endregion IsBetween

        #region IsBetweenOrEqual

        public static Expression<Func<T, bool>> IsBetweenOrEqual<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, IEnumerable<V> Values)
        {
            var exp = false.CreateWhereExpression<T>();
            foreach (var item in Values ?? Array.Empty<V>())
            {
                exp = exp.Or(WhereExpression(MinProperty, "<=", new[] { (IComparable)item }).And(WhereExpression(MaxProperty, ">=", new[] { (IComparable)item })));
            }

            return exp;
        }

        public static Expression<Func<T, bool>> IsBetweenOrEqual<T, V>(this Expression<Func<T, V>> MinProperty, Expression<Func<T, V>> MaxProperty, params V[] Values)
        => MinProperty.IsBetweenOrEqual(MaxProperty, Values.AsEnumerable());

        public static Expression<Func<T, bool>> IsBetweenOrEqual<T, V>(this Expression<Func<T, V>> Property, V MinValue, V MaxValue)
        {
            if (MinValue.Equals(MaxValue))
            {
                return IsEqual(Property, MinValue);
            }
            else
            {
                return WhereExpression(Property, "betweenorequal", new IComparable[] { (IComparable)MinValue, (IComparable)MaxValue });
            }
        }

        public static Expression<Func<T, bool>> IsInDateRange<T>(this Expression<Func<T, DateTime>> Property, DateRange DateRange, DateRangeFilterBehavior? FilterBehavior = null)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, DateRange.StartDate);
            }

            var icomp = new IComparable[] { DateRange.StartDate, DateRange.EndDate };
            switch (FilterBehavior ?? DateRange.FilterBehavior)
            {
                case DateRangeFilterBehavior.BetweenExclusive: return WhereExpression(Property, "between", icomp);
                case DateRangeFilterBehavior.Between: return WhereExpression(Property, "between", icomp).Or(WhereExpression(Property, "equal", new IComparable[] { DateRange.StartDate }));
                case DateRangeFilterBehavior.BetweenOrEqual:
                default: return WhereExpression(Property, "betweenorequal", icomp);
            }
        }

        public static Expression<Func<T, bool>> IsInDateRange<T>(this Expression<Func<T, DateTime?>> Property, DateRange DateRange, DateRangeFilterBehavior? FilterBehavior = null)
        {
            if (DateRange.IsSingleDateTime())
            {
                return IsEqual(Property, (DateTime?)DateRange.StartDate);
            }

            var icomp = new IComparable[] { (DateTime?)DateRange.StartDate, (DateTime?)DateRange.EndDate };
            switch (FilterBehavior ?? DateRange.FilterBehavior)
            {
                case DateRangeFilterBehavior.BetweenExclusive: return WhereExpression(Property, "between", icomp);
                case DateRangeFilterBehavior.Between: return WhereExpression(Property, "between", icomp).Or(IsEqual(Property, (DateTime?)DateRange.StartDate));
                case DateRangeFilterBehavior.BetweenOrEqual:
                default: return WhereExpression(Property, "betweenorequal", icomp);
            }
        }

        #endregion IsBetweenOrEqual

        /// <summary>
        /// Corta a imagem em um circulo
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <param name="Background">Cor do fundo</param>
        /// <returns></returns>
        public static Image CropToCircle(this Image Img, Color? Background = default) => Img.CropToSquare()?.CropToEllipsis(Background);

        /// <summary>
        /// Corta a imagem em uma elipse
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <param name="Background">Cor do fundo</param>
        /// <returns></returns>
        public static Image CropToEllipsis(this Image Img, Color? Background = default)
        {
            var dstImage = new Bitmap(Img.Width, Img.Height);
            var g = Graphics.FromImage(dstImage);
            Background = Background ?? Color.Transparent;
            using (Brush br = new SolidBrush((Color)Background))
            {
                g.FillRectangle(br, 0, 0, dstImage.Width, dstImage.Height);
            }

            var path = new GraphicsPath();
            path.AddEllipse(0, 0, dstImage.Width, dstImage.Height);
            g.SetClip(path);
            g.DrawImage(Img, 0, 0);
            return dstImage;
        }

        /// <summary>
        /// Corta uma imagem para um quadrado perfeito a partir do centro
        /// </summary>
        /// <param name="img">Imagem</param>
        /// <param name="WidthHeight">Tamanho do quadrado em pixels</param>
        /// <returns></returns>
        public static Image CropToSquare(this Image Img, int WidthHeight = 0)
        {
            if (WidthHeight < 1)
            {
                WidthHeight = Img.Height > Img.Width ? Img.Width : Img.Height;
            }

            return Img.Crop(WidthHeight, WidthHeight);
        }

        public static Image DrawString(this Image img, string Text, Font Font = null, Color? Color = default, int X = -1, int Y = -1)
        {
            var bitmap = new Bitmap(img);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                Font = Font ?? new Font("Arial", (float)(bitmap.Width / 10d));
                var tamanho = graphics.MeasureString(Text, Font, new Size(bitmap.Width, bitmap.Height));
                X = X.LimitRange(-1, img.Width);
                Y = Y.LimitRange(-1, img.Height);
                if (X == -1)
                {
                    X = (int)Math.Round(bitmap.Width / 2d - tamanho.Width / 2f);
                }

                if (Y == -1)
                {
                    Y = (int)Math.Round(bitmap.Height / 2d - tamanho.Height / 2f);
                }

                Color = Color ?? bitmap.GetPixel(X, Y).GetContrastColor(50f);
                var B = new SolidBrush((Color)Color);
                graphics.DrawString(Text, Font, B, X, Y);
            }

            return bitmap;
        }

        /// <summary>
        /// Pega o encoder a partir de um formato de imagem
        /// </summary>
        /// <param name="RawFormat">Image format</param>
        /// <returns>image codec info.</returns>
        public static ImageCodecInfo GetEncoderInfo(this ImageFormat RawFormat)
        {
            return ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == RawFormat.Guid).FirstOr(ImageCodecInfo.GetImageDecoders().Where(c => c.FormatID == ImageFormat.Png.Guid).First());
        }

        /// <summary>
        /// Retorna o formato da imagem correspondente a aquela imagem
        /// </summary>
        /// <param name="OriginalImage"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(this Image OriginalImage)
        {
            return ImageTypes.Where(p => p.Guid == OriginalImage.RawFormat.Guid).FirstOr(ImageFormat.Png);
        }

        /// <summary>
        /// Retorna uma lista com as N cores mais utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image, int Count)
        {
            return new Bitmap(Image).GetMostUsedColors().Take(Count);
        }

        /// <summary>
        /// Retorna uma lista com as cores utilizadas na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>uma lista de Color</returns>
        public static IEnumerable<HSVColor> GetMostUsedColors(this Image Image)
        {
            return Image.ColorPallette().Keys;
        }

        public static RotateFlipType GetRotateFlip(this Image Img)
        {
            var rft = RotateFlipType.RotateNoneFlipNone;
            foreach (PropertyItem p in Img.PropertyItems)
            {
                if (p.Id == 274)
                {
                    short orientation = BitConverter.ToInt16(p.Value, 0);
                    switch (orientation)
                    {
                        case 1:
                            {
                                rft = RotateFlipType.RotateNoneFlipNone;
                                break;
                            }

                        case 3:
                            {
                                rft = RotateFlipType.Rotate180FlipNone;
                                break;
                            }

                        case 6:
                            {
                                rft = RotateFlipType.Rotate90FlipNone;
                                break;
                            }

                        case 8:
                            {
                                rft = RotateFlipType.Rotate270FlipNone;
                                break;
                            }
                    }
                }
            }

            return rft;
        }

        /// <summary>
        /// Converte uma Imagem para Escala de cinza
        /// </summary>
        /// <param name="img">imagem original</param>
        /// <returns></returns>
        public static Image Grayscale(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { 0.299f, 0.299f, 0.299f, 0f, 0f }, new float[] { 0.587f, 0.587f, 0.587f, 0f, 0f }, new float[] { 0.114f, 0.114f, 0.114f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            draw_adjusted_image(copia, cm);
            return copia;
        }

        /// <summary>
        /// Insere uma imagem em outra imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="InsertedImage">Imagem de Marca Dagua</param>
        /// <param name="X">Posição X</param>
        /// <param name="Y">Posição Y</param>
        /// <returns></returns>
        public static Image Insert(this Image Image, Image InsertedImage, int X = -1, int Y = -1)
        {
            var bm_Resultado = new Bitmap(Image);
            var bm_marcaDagua = new Bitmap(InsertedImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5));
            if (X < 0)
                X = (bm_Resultado.Width - bm_marcaDagua.Width) / 2;
            if (Y < 0)
                Y = (bm_Resultado.Height - bm_marcaDagua.Height) / 2;
            var gr = Graphics.FromImage(bm_Resultado);
            gr.DrawImage(bm_marcaDagua, X, Y);
            return bm_Resultado;
        }

        public static Image MakeDarker(this Image img, float percent = 50f)
        {
            var lockedBitmap = new Bitmap(img);
            for (int y = 0, loopTo = lockedBitmap.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = lockedBitmap.Width - 1; x <= loopTo1; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    if (!oldColor.CompareARGB(true, Color.Transparent, Color.Black, Color.White))
                    {
                        var newColor = oldColor.MakeDarker(percent);
                        lockedBitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            return lockedBitmap;
        }

        public static Image MakeLighter(this Image img, float percent = 50f)
        {
            var lockedBitmap = new Bitmap(img);
            for (int y = 0, loopTo = lockedBitmap.Height - 1; y <= loopTo; y++)
            {
                for (int x = 0, loopTo1 = lockedBitmap.Width - 1; x <= loopTo1; x++)
                {
                    var oldColor = lockedBitmap.GetPixel(x, y);
                    if (!oldColor.CompareARGB(true, Color.Transparent, Color.Black, Color.White))
                    {
                        var newColor = oldColor.MakeLighter(percent);
                        lockedBitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            return lockedBitmap;
        }

        public static Image Monochrome(this Image Image, Color Color, float Alpha = 0f) => Image.Grayscale().Translate(Color.R, Color.G, Color.B, Alpha);

        /// <summary>
        /// Inverte as cores de uma imagem
        /// </summary>
        /// <param name="Img"></param>
        /// <returns></returns>
        public static Image Negative(this Image img)
        {
            var copia = new Bitmap(img);
            var cm = new ColorMatrix(new float[][] { new float[] { -1, 0f, 0f, 0f, 0f }, new float[] { 0f, -1, 0f, 0f, 0f }, new float[] { 0f, 0f, -1, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { 0f, 0f, 0f, 0f, 1f } });
            draw_adjusted_image(copia, cm);
            return copia;
        }

        /// <summary>
        /// Interpreta uma string de diversas formas e a transforma em um <see cref="Size"/>
        /// </summary>
        /// <param name="Text">Texto</param>
        /// <returns></returns>
        public static Size ParseSize(this string Text)
        {
            var s = new Size();
            Text = Text.ReplaceMany(" ", "px", " ", ";", ":").ToLowerInvariant().Trim();
            Text = Text.Replace("largura", "width");
            Text = Text.Replace("altura", "height");
            Text = Text.Replace("l ", "w ");
            Text = Text.Replace("a ", "h ");
            try
            {
                switch (true)
                {
                    case object _ when Text.IsNumber():
                        {
                            s.Width = Convert.ToInt32(Text);
                            s.Height = s.Width;
                            break;
                        }

                    case object _ when Text.Like("width*") && !Text.Like("*height*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("width"));
                            s.Height = Convert.ToInt32(Text.GetAfter("width"));
                            break;
                        }

                    case object _ when Text.Like("height*") && !Text.Like("*width*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("height"));
                            s.Height = Convert.ToInt32(Text.GetAfter("height"));
                            break;
                        }

                    case object _ when Text.Like("w*") && !Text.Like("*h*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("w"));
                            s.Height = Convert.ToInt32(Text.GetAfter("w"));
                            break;
                        }

                    case object _ when Text.Like("h*") && !Text.Like("*w*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetAfter("h"));
                            s.Height = Convert.ToInt32(Text.GetAfter("h"));
                            break;
                        }

                    case object _ when Text.Like("width*height*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetBetween("width", "height"));
                            s.Height = Convert.ToInt32(Text.GetAfter("height"));
                            break;
                        }

                    case object _ when Text.Like("height*width*"):
                        {
                            s.Height = Convert.ToInt32(Text.GetBetween("height", "width"));
                            s.Width = Convert.ToInt32(Text.GetAfter("width"));
                            break;
                        }

                    case object _ when Text.Like("w*h*"):
                        {
                            s.Width = Convert.ToInt32(Text.GetBetween("w", "h"));
                            s.Height = Convert.ToInt32(Text.GetAfter("h"));
                            break;
                        }

                    case object _ when Text.Like("h*w*"):
                        {
                            s.Height = Convert.ToInt32(Text.GetBetween("h", "w"));
                            s.Width = Convert.ToInt32(Text.GetAfter("w"));
                            break;
                        }

                    case object _ when Text.Like("*x*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "x" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "x" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*by*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "by" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "by" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*por*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "por" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "por" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*,*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*-*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "-" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    case object _ when Text.Like("*_*"):
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }

                    default:
                        {
                            s.Width = Convert.ToInt32(Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0]);
                            s.Height = Convert.ToInt32(Text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[1]);
                            break;
                        }
                }
            }
            catch
            {
            }

            return s;
        }

        /// <summary>
        /// Pixeliza uma imagem
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="PixelateSize"></param>
        /// <returns></returns>
        public static Image Pixelate(this Image Image, int PixelateSize = 1)
        {
            if (Image == null) return null;
            var rectangle = new Rectangle(0, 0, Image.Width, Image.Height);
            PixelateSize++;
            var pixelated = new Bitmap(Image.Width, Image.Height);
            using (var graphics = Graphics.FromImage(pixelated))
            {
                graphics.DrawImage(Image, new Rectangle(0, 0, Image.Width, Image.Height), new Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
            }

            int xx = rectangle.X;
            while (xx < rectangle.X + rectangle.Width && xx < Image.Width)
            {
                int yy = rectangle.Y;
                while (yy < rectangle.Y + rectangle.Height && yy < Image.Height)
                {
                    int offsetX = (int)Math.Round(PixelateSize / 2d);
                    int offsetY = (int)Math.Round(PixelateSize / 2d);
                    while (xx + offsetX >= Image.Width)
                        offsetX -= 1;
                    while (yy + offsetY >= Image.Height)
                        offsetY -= 1;
                    var pixel = pixelated.GetPixel(xx + offsetX, yy + offsetY);
                    int x = xx;
                    while (x < xx + PixelateSize && x < Image.Width)
                    {
                        int y = yy;
                        while (y < yy + PixelateSize && y < Image.Height)
                        {
                            pixelated.SetPixel(x, y, pixel);
                            y += 1;
                        }

                        x += 1;
                    }

                    yy += PixelateSize;
                }

                xx += PixelateSize;
            }

            return pixelated;
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="ResizeExpression">uma string contendo uma expressão de tamanho</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, string ResizeExpression, bool OnlyResizeIfWider = true)
        {
            if (ResizeExpression.Contains("%"))
            {
                return Original.ResizePercent(ResizeExpression, OnlyResizeIfWider);
            }
            else
            {
                var s = ResizeExpression.ParseSize();
                return Original.Resize(s, OnlyResizeIfWider);
            }
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="Size">Tamanho</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, Size Size, bool OnlyResizeIfWider = true)
        {
            return Original.Resize(Size.Width, Size.Height, OnlyResizeIfWider);
        }

        /// <summary>
        /// Redimensiona e converte uma Imagem
        /// </summary>
        /// <param name="Original">Imagem Original</param>
        /// <param name="NewWidth">Nova Largura</param>
        /// <param name="MaxHeight">Altura máxima</param>
        /// <param name="OnlyResizeIfWider">
        /// Indica se a imagem deve ser redimensionada apenas se sua largura for maior que a especificada
        /// </param>
        /// <returns></returns>
        public static Image Resize(this Image Original, int NewWidth, int MaxHeight, bool OnlyResizeIfWider = true)
        {
            Image fullsizeImage = new Bitmap(Original);
            if (OnlyResizeIfWider)
            {
                if (fullsizeImage.Width <= NewWidth)
                {
                    NewWidth = fullsizeImage.Width;
                }
            }

            int newHeight = (int)Math.Round(fullsizeImage.Height * NewWidth / (double)fullsizeImage.Width);
            if (newHeight > MaxHeight)
            {
                // Resize with height instead
                NewWidth = (int)Math.Round(fullsizeImage.Width * MaxHeight / (double)fullsizeImage.Height);
                newHeight = MaxHeight;
            }

            fullsizeImage = fullsizeImage.GetThumbnailImage(NewWidth, newHeight, null, IntPtr.Zero);
            fullsizeImage.RotateFlip(Original.GetRotateFlip());
            return fullsizeImage;
        }

        /// <summary>
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height)
        {
            return Image.Resize(Width, Height, false).Crop(Width, Height);
        }

        /// <summary>
        /// redimensiona e Cropa uma imagem, aproveitando a maior parte dela
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public static Image ResizeCrop(this Image Image, int Width, int Height, bool OnlyResizeIfWider)
        {
            return Image.Resize(Width, Height, OnlyResizeIfWider).Crop(Width, Height);
        }

        /// <summary>
        /// Redimensiona uma imagem para o tamanho definido por uma porcentagem
        /// </summary>
        /// <param name="Original"></param>
        /// <param name="Percent">Porcentagem ( no formato '30% 'ou '20% x 10%')</param>
        /// <param name="OnlyResizeIfWider"></param>
        /// <returns></returns>
        public static Image ResizePercent(this Image Original, string Percent, bool OnlyResizeIfWider = true)
        {
            var size = new Size();
            if (Percent.Contains("x"))
            {
                var parts = Percent.Split("x");
                if (parts[0].TrimBetween().EndsWith("%"))
                {
                    parts[0] = parts[0].TrimBetween().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString();
                }

                if (parts[1].TrimBetween().EndsWith("%"))
                {
                    parts[1] = parts[1].TrimBetween().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString();
                }

                size = new Size(parts[0].ToInt(), parts[1].ToInt());
            }
            else
            {
                if (Percent.TrimBetween().EndsWith("%"))
                {
                    Percent = Percent.Trim('%').TrimBetween();
                }

                if (Percent.IsNumber())
                {
                    size.Width = Convert.ToInt32(Percent.ToInt().CalculateValueFromPercent(Original.Width).RoundDecimal().ToString());
                    size.Height = Convert.ToInt32(Percent.ToInt().CalculateValueFromPercent(Original.Height).RoundDecimal().ToString());
                }
            }

            return Original.Resize(size, OnlyResizeIfWider);
        }

        public static Image ResizePercent(this Image Original, decimal Percent, bool OnlyResizeIfWider = true)
        {
            return Original.ResizePercent(Percent.ToPercentString(), OnlyResizeIfWider);
        }

        /// <summary>
        /// Rotaciona uma imagem para sua posição original caso ela já tenha sido rotacionada (EXIF)
        /// </summary>
        /// <param name="Img">Imagem</param>
        /// <returns>TRUE caso a imagem ja tenha sido rotacionada</returns>
        public static bool TestAndRotate(this Image Img)
        {
            var rft = Img.GetRotateFlip();
            if (rft != RotateFlipType.RotateNoneFlipNone)
            {
                Img.RotateFlip(rft);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retorna uma <see cref="Bitmap"/> a partir de um Image
        /// </summary>
        /// <param name="Image"></param>
        /// <returns></returns>
        public static Bitmap ToBitmap(this Image Image) => new Bitmap(Image);

        /// <summary>
        /// Transforma uma imagem em array de bytes
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns></returns>
        public static byte[] ToBytes(this Image Image, ImageFormat Format = null)
        {
            using (var ms = Image.ToStream(Format))
            {
                return ms.ToBytes();
            }
        }

        /// <summary>
        /// Transforma uma imagem em um stream
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns></returns>
        public static Stream ToStream(this Image Image, ImageFormat Format = null)
        {
            Stream s = new MemoryStream();
            Image.Save(s, Format ?? ImageFormat.Png);
            s.Position = 0L;
            return s;
        }

        public static Image Translate(this Image img, Color Color, float Alpha = 0f) => img.Translate(Color.R, Color.G, Color.B, Alpha);

        public static Image Translate(this Image img, float Red, float Green, float Blue, float Alpha = 0f)
        {
            float sr, sg, sb, sa;
            var copia = new Bitmap(img);
            // normalize the color components to 1
            sr = Red / 255f;
            sg = Green / 255f;
            sb = Blue / 255f;
            sa = Alpha / 255f;

            // create the color matrix
            var cm = new ColorMatrix(new float[][] { new float[] { 1f, 0f, 0f, 0f, 0f }, new float[] { 0f, 1f, 0f, 0f, 0f }, new float[] { 0f, 0f, 1f, 0f, 0f }, new float[] { 0f, 0f, 0f, 1f, 0f }, new float[] { sr, sg, sb, sa, 1f } });

            // apply the matrix to the image
            draw_adjusted_image(copia, cm);
            return copia;
        }

        /// <summary>
        /// Remove os excessos de uma cor de fundo de uma imagem deixando apenas seu conteudo
        /// </summary>
        /// <param name="Img"></param>
        /// <param name="Color"></param>
        /// <returns></returns>
        public static Image Trim(this Image Img, Color? Color = default)
        {
            var bitmap = new Bitmap(Img);
            Color = Color ?? bitmap.GetPixel(0, 0);
            int w = bitmap.Width;
            int h = bitmap.Height;
            bool IsAllWhiteRow(int row)
            {
                for (int i = 0, loopTo = w - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(i, row).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            }
            bool IsAllWhiteColumn(int col)
            {
                for (int i = 0, loopTo = h - 1; i <= loopTo; i++)
                {
                    if (bitmap.GetPixel(col, i).ToArgb() != Color.Value.ToArgb())
                    {
                        return false;
                    }
                }

                return true;
            }
            int leftMost = 0;
            for (int col = 0, loopTo = w - 1; col <= loopTo; col++)
            {
                if (IsAllWhiteColumn(col))
                {
                    leftMost = col + 1;
                }
                else
                {
                    break;
                }
            }

            int rightMost = w - 1;
            for (int col = rightMost; col >= 1; col -= 1)
            {
                if (IsAllWhiteColumn(col))
                {
                    rightMost = col - 1;
                }
                else
                {
                    break;
                }
            }

            int topMost = 0;
            for (int row = 0, loopTo1 = h - 1; row <= loopTo1; row++)
            {
                if (IsAllWhiteRow(row))
                {
                    topMost = row + 1;
                }
                else
                {
                    break;
                }
            }

            int bottomMost = h - 1;
            for (int row = bottomMost; row >= 1; row -= 1)
            {
                if (IsAllWhiteRow(row))
                {
                    bottomMost = row - 1;
                }
                else
                {
                    break;
                }
            }

            if (rightMost == 0 && bottomMost == 0 && leftMost == w && topMost == h)
            {
                return bitmap;
            }

            int croppedWidth = rightMost - leftMost + 1;
            int croppedHeight = bottomMost - topMost + 1;
            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (var g = Graphics.FromImage(target))
                {
                    g.DrawImage(bitmap, new RectangleF(0f, 0f, croppedWidth, croppedHeight), new RectangleF(leftMost, topMost, croppedWidth, croppedHeight), GraphicsUnit.Pixel);
                }

                return target;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Values are top={0} bottom={1} left={2} right={3}", topMost, bottomMost, leftMost, rightMost), ex);
            }
        }

        /// <summary>
        /// Insere uma imagem de marca d'água na imagem
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <param name="WaterMarkImage">Imagem de Marca d'água</param>
        /// <param name="X">Posição X</param>
        /// <param name="Y">Posição Y</param>
        /// <returns></returns>
        public static Image Watermark(this Image Image, Image WaterMarkImage, int X = -1, int Y = -1)
        {
            // a imagem onde iremos aplicar a marca d'água
            var bm_Resultado = new Bitmap(Image);

            // a imagem que será usada como marca d'agua
            var bm_marcaDagua = new Bitmap(WaterMarkImage.Resize(bm_Resultado.Width - 5, bm_Resultado.Height - 5));
            if (X < 0)
                X = (bm_Resultado.Width - bm_marcaDagua.Width) / 2;   // centraliza a marca d'agua
            if (Y < 0)
                Y = (bm_Resultado.Height - bm_marcaDagua.Height) / 2;   // centraliza a marca d'agua
            const byte ALPHA = 128;
            // Define o componente Alpha do pixel
            Color clr;
            for (int py = 0, loopTo = bm_marcaDagua.Height - 1; py <= loopTo; py++)
            {
                for (int px = 0, loopTo1 = bm_marcaDagua.Width - 1; px <= loopTo1; px++)
                {
                    clr = bm_marcaDagua.GetPixel(px, py);
                    bm_marcaDagua.SetPixel(px, py, Color.FromArgb(ALPHA, clr.R, clr.G, clr.B));
                }
            }
            // Define a marca d'agua como transparente
            bm_marcaDagua.MakeTransparent(bm_marcaDagua.GetPixel(0, 0));
            // Copia o resultado na imagem
            var gr = Graphics.FromImage(bm_Resultado);
            gr.DrawImage(bm_marcaDagua, X, Y);
            return bm_Resultado;
        }

        #endregion Public Methods

        /// <summary>
        /// Gera uma URL do google MAPs baseado na localização
        /// </summary>
        /// <param name="local">
        /// Uma variavel do tipo InnerLibs.Location onde estão as informações como endereço e as
        /// coordenadas geográficas
        /// </param>
        /// <returns>Uma URI do Google Maps</returns>
        public static Uri ToGoogleMapsURL(this AddressInfo local, params AddressPart[] Parts) => local != null ? new Uri($"https://www.google.com.br/maps/search/{Uri.EscapeUriString(local.ToString(Parts))}") : null;

        #endregion Public Methods

        public static string ToRoman(this int ArabicNumber)
        {
            ArabicNumber = ArabicNumber.ForcePositive();
            // valida : aceita somente valores entre 1 e 3999
            if (ArabicNumber < 1 || ArabicNumber > 3999)
            {
                throw new ArgumentException("The numeric value must be between 1 and 3999.", nameof(ArabicNumber));
            }

            var algarismosArabicos = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
            var algarismosRomanos = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "TV", "IV", "I" };

            // inicializa o string builder
            string resultado = Ext.EmptyString;

            // percorre os valores nos arrays
            for (int i = 0; i <= 12; i++)
            {
                // se o numero a ser convertido é menor que o valor então anexa o numero
                // correspondente ou o par ao resultado
                while (ArabicNumber >= algarismosArabicos[i])
                {
                    ArabicNumber -= algarismosArabicos[i];
                    resultado += algarismosRomanos[i];
                }
            }

            // retorna o resultado
            return resultado.ToString();
        }

        #endregion Public Methods

        #region Public Enums

        public enum RomanDigit
        {
            /// <summary>
            /// Valor correspondente
            /// </summary>
            I = 1,

            /// <summary>
            /// Valor correspondente
            /// </summary>
            V = 5,

            /// <summary>
            /// Valor correspondente
            /// </summary>
            X = 10,

            /// <summary>
            /// Valor correspondente
            /// </summary>
            L = 50,

            /// <summary>
            /// Valor correspondente
            /// </summary>
            C = 100,

            /// <summary>
            /// Valor correspondente
            /// </summary>
            D = 500,

            /// <summary>
            /// Valor correspondente
            /// </summary>
            M = 1000
        }

        #endregion Public Enums

        #region Public Methods

        /// <summary>
        /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo
        /// </summary>
        /// <param name="Extension">Arquivo</param>
        /// <returns></returns>
        public static string GetFontAwesomeIconByFileExtension(this string Extension) => GetFontAwesomeIconByFileExtension(new[] { Extension });

        public static string GetFontAwesomeIconByFileExtension(this string[] Extensions)
        {
            foreach (var Extension in Extensions ?? Array.Empty<string>())
                if (Extension.IsNotBlank())
                    switch (Extension.RemoveAny(".").ToLowerInvariant())
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
            if (File != null && File.Attributes == FileAttributes.Device)
            {
                return "fa-plug";
            }
            else if (File != null && File.Attributes == FileAttributes.Directory)
            {
                return DirectoryOpen ? "fa-folder-open" : "fa-folder";
            }
            else
            {
                return GetFontAwesomeIconByFileExtension(File?.Extension);
            }
        }

        /// <summary>
        /// Retorna a classe do icone do FontAwesome que representa melhor o arquivo
        /// </summary>
        /// <param name="MIME">MIME T do Arquivo</param>
        /// <returns></returns>
        public static string GetIconByFileType(this FileType MIME) => GetFontAwesomeIconByFileExtension(MIME?.Extensions.ToArray() ?? Array.Empty<string>());

        #region Public Methods

        /// <summary>
        /// Retorna true se <paramref name="Value"/> não estiver em branco, for diferente de NULL,
        /// 'null' '0', 'not', 'nao', '!' ou 'false'
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool AsBool(this string Value)
        {
            if (Value == null || Value.IsBlank())
            {
                return false;
            }

            Value = Value.TrimBetween().ToUpperInvariant().RemoveAccents();
            switch (Value)
            {
                case "!":
                case "0":
                case "FALSE":
                case "NULL":
                case "NOT":
                case "NAO":
                    return false;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Converte um array de um ToType para outro
        /// </summary>
        /// <typeparam name="TTo">Tipo do array</typeparam>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static TTo[] ChangeArrayType<TTo, TFrom>(this TFrom[] Value) => Value.AsEnumerable().ChangeIEnumerableType<TTo, TFrom>().ToArray();

        /// <summary>
        /// Converte um array de um ToType para outro
        /// </summary>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static object[] ChangeArrayType<TFrom>(this TFrom[] Value, Type Type) => Value.ChangeIEnumerableType(Type).ToArray();

        /// <summary>
        /// Converte um IEnumerable de um ToType para outro
        /// </summary>
        /// <typeparam name="TTo">Tipo do array</typeparam>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static IEnumerable<TTo> ChangeIEnumerableType<TTo, TFrom>(this IEnumerable<TFrom> Value) => (IEnumerable<TTo>)Value.ChangeIEnumerableType(typeof(TTo));

        /// <summary>
        /// Converte um IEnumerable de um ToType para outro
        /// </summary>
        /// <param name="Value">Array com elementos</param>
        /// <returns>Array convertido em novo ToType</returns>
        public static IEnumerable<object> ChangeIEnumerableType<TFrom>(this IEnumerable<TFrom> Value, Type ToType) => (Value ?? Array.Empty<TFrom>()).Select(el => el.ChangeType(ToType));

        /// <summary>
        /// Converte um ToType para outro. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType ou null se a conversão falhar</returns>
        public static T ChangeType<T>(this object Value)
        {
            try
            {
                var tp = typeof(T).GetNullableTypeOf() ?? typeof(T);
                if (Value != null)
                {
                    return (T)Value.ChangeType(tp);
                }
                return default;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Converte um ToType para outro. Retorna Nothing (NULL) ou DEFAULT se a conversão falhar
        /// </summary>
        /// <typeparam name="TFrom">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType ou null (ou default) se a conversão falhar</returns>
        public static object ChangeType<TFrom>(this TFrom Value, Type ToType)
        {
            if (ToType == null)
            {
                Ext.WriteDebug($"ToType is null, using {typeof(TFrom).Name}");
                return Value;
            }

            Ext.WriteDebug($"Try changing from {typeof(TFrom).Name} to {ToType.Name}");

            try
            {
                var met = Value?.GetType().GetNullableTypeOf().GetMethods().FirstOrDefault(x => x.Name == $"To{ToType.Name}" && x.ReturnType == ToType && x.IsPublic && x.GetParameters().Any() == false);
                if (met != null)
                {
                    Ext.WriteDebug($"Trying internal method {met.Name}");
                    return met.Invoke(Value, Array.Empty<object>());
                }
            }
            catch
            {
            }

            try
            {
                ToType = Ext.GetNullableTypeOf(ToType);
                if (Value == null)
                {
                    Ext.WriteDebug($"Value is null");
                    if (!ToType.IsValueType() || ToType.IsNullableType())
                    {
                        return null;
                    }
                    else
                    {
                        return default;
                    }
                }

                if (ToType == typeof(Guid))
                {
                    Ext.WriteDebug($"Parsing Guid");
                    return Guid.Parse(Value.ToString());
                }

                if (ToType.IsEnum)
                {
                    if (Value is string Name && Name.IsNotBlank())
                    {
                        Name = Name.RemoveAccents().ToUpperInvariant();
                        foreach (var x in Enum.GetValues(ToType))
                        {
                            var entryName = Enum.GetName(ToType, x)?.RemoveAccents().ToUpperInvariant();
                            var entryValue = $"{(int)x}";

                            if (Name == entryName || Name == entryValue)
                            {
                                Ext.WriteDebug($"{ToType.Name} value ({Name}) found ({entryName})");
                                return Convert.ChangeType(x, ToType);
                            }
                        }

                        Ext.WriteDebug($"{ToType.Name} value ({Name}) not found");
                        return Activator.CreateInstance(ToType);
                    }
                }

                if (ToType.IsValueType())
                {
                    Ext.WriteDebug($"{ToType.Name} is value type");
                    var Converter = TypeDescriptor.GetConverter(ToType);
                    if (Converter.CanConvertFrom(typeof(TFrom)))
                    {
                        try
                        {
                            return Converter.ConvertTo(Value, ToType);
                        }
                        catch
                        {
                            return Convert.ChangeType(Value, ToType);
                        }
                    }
                    else
                    {
                        return Convert.ChangeType(Value, ToType);
                    }
                }
                else
                {
                    return Convert.ChangeType(Value, ToType);
                }
            }
            catch (Exception ex)
            {
                Ext.WriteDebug(ex.ToFullExceptionString(), "Error on change type");
                Ext.WriteDebug("Returning null");
                return null;
            }
        }

        public static object CreateOrSetObject(this Dictionary<string, object> Dic, object Obj, Type Type, params object[] args)
        {
            var tipo = Type.GetNullableTypeOf();
            if (tipo.IsValueType())
            {
                return (Dic?.Values.FirstOrDefault()).ChangeType(tipo);
            }

            if (Obj == null)
            {
                if ((args ?? Array.Empty<object>()).Any())
                {
                    Obj = Activator.CreateInstance(tipo, args);
                }
                else
                {
                    Obj = Activator.CreateInstance(tipo);
                }
            }

            if (tipo == typeof(Dictionary<string, object>))
            {
                if (Dic != null)
                {
                    return Dic.AsEnumerable().ToDictionary();
                }

                return null;
            }
            else if (tipo == typeof(Dictionary<string, string>))
            {
                if (Dic != null)
                {
                    return Dic.AsEnumerable().ToDictionary(x => x.Key, x => x.Value?.ToString());
                }

                return null;
            }

            if (Dic != null && Dic.Any())
            {
                foreach (var k in Dic)
                {
                    k.Key.PropertyNamesFor();
                    string propname1 = k.Key.Trim().Replace(" ", "_").Replace("-", "_").Replace("~", "_");
                    string propname3 = k.Key.Trim().Replace(" ", InnerLibs.Ext.EmptyString).Replace("-", InnerLibs.Ext.EmptyString).Replace("~", InnerLibs.Ext.EmptyString);
                    string propname2 = propname1.RemoveAccents();
                    string propname4 = propname3.RemoveAccents();
                    var prop = Ext.NullCoalesce(tipo.GetProperty(propname1), tipo.GetProperty(propname2), tipo.GetProperty(propname3), tipo.GetProperty(propname4));
                    if (prop != null)
                    {
                        if (prop.CanWrite)
                        {
                            if (k.Value.GetType() == typeof(DBNull))
                            {
                                prop.SetValue(Obj, null);
                            }
                            else
                            {
                                prop.SetValue(Obj, k.Value.ChangeType(prop.PropertyType));
                            }
                        }
                    }
                    else
                    {
                        var fiif = Ext.NullCoalesce(tipo.GetField(propname1), tipo.GetField(propname2), tipo.GetField(propname3), tipo.GetField(propname4));
                        if (fiif != null)
                        {
                            if (k.Value.GetType() == typeof(DBNull))
                            {
                                prop.SetValue(Obj, null);
                            }
                            else
                            {
                                prop.SetValue(Obj, k.Value.ChangeType(fiif.FieldType));
                            }
                        }
                    }
                }
            }

            return Obj;
        }

        /// <summary>
        /// Cria uma lista vazia usando um objeto como o ToType da lista. Util para tipos anonimos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ObjectForDefinition">Objeto que definirá o ToType da lista</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remover o parâmetro não utilizado", Justification = "<Pendente>")]
        public static List<T> DefineEmptyList<T>(this T ObjectForDefinition) => new List<T>();

        /// <summary>
        /// Verifica se <paramref name="Obj"/> é um array e retorna este array. Se negativo, retorna
        /// um array contendo o valor de <paramref name="Obj"/> ou um array vazio se <paramref
        /// name="Obj"/> for nulo
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <returns></returns>
        public static object[] ForceArray(this object Obj, Type Type)
        {
            Type = Type ?? typeof(object);
            if (Obj != null)
            {
                if (Ext.IsArray(Obj))
                {
                    var aobj = ((Array)Obj).Cast<object>().ToArray();
                    return Ext.ChangeArrayType(aobj, Type).ToArray();
                }
                else if (!Obj.IsTypeOf<string>() && Obj.IsEnumerable())
                {
                    var aobj = (IEnumerable<object>)Obj;
                    return Ext.ChangeIEnumerableType(aobj, Type).ToArray();
                }
                else
                {
                    return (new[] { Obj }).ChangeArrayType(Type);
                }
            }

            return Array.Empty<object>().ChangeArrayType(Type);
        }

        /// <summary>
        /// Verifica se um objeto é um array, e se não, cria um array com este obejeto
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <returns></returns>
        public static T[] ForceArray<T>(this object Obj) => ForceArray(Obj, typeof(T)).Cast<T>().ToArray();

        /// <summary>
        /// Mescla varios dicionarios em um unico dicionario. Quando uma key existir em mais de um
        /// dicionario os valores sao agrupados em arrays
        /// </summary>
        /// <typeparam name="T">Tipo da Key, Deve ser igual para todos os dicionarios</typeparam>
        /// <param name="FirstDictionary">Dicionario Principal</param>
        /// <param name="Dictionaries">Outros dicionarios</param>
        /// <returns></returns>
        public static Dictionary<T, object> Merge<T>(this Dictionary<T, object> FirstDictionary, params Dictionary<T, object>[] Dictionaries)
        {
            // dicionario que está sendo gerado a partir dos outros
            var result = new Dictionary<T, object>();

            Dictionaries = Dictionaries ?? Array.Empty<Dictionary<T, object>>();
            // adiciona o primeiro dicionario ao array principal e exclui dicionarios vazios
            Dictionaries = Dictionaries.Union(new[] { FirstDictionary }).Where(x => x.Count > 0).ToArray();

            // cria um array de keys unicas a partir de todos os dicionarios
            var keys = Dictionaries.SelectMany(x => x.Keys.ToArray()).Distinct();

            // para cada chave encontrada
            foreach (var key in keys)
            {
                // para cada dicionario a ser mesclado
                foreach (var dic in Dictionaries)
                {
                    // dicionario tem a chave?
                    if (dic.ContainsKey(key))
                    {
                        // resultado ja tem a chave atual adicionada?
                        if (result.ContainsKey(key))
                        {
                            // lista que vai mesclar tudo
                            var lista = new List<object>();

                            // chave do resultado é um array?
                            if (Ext.IsArray(result[key]))
                            {
                                lista.AddRange((IEnumerable<object>)result[key]);
                            }
                            else
                            {
                                lista.Add(result[key]);
                            }
                            // chave do dicionario é um array?
                            if (Ext.IsArray(dic[key]))
                            {
                                lista.AddRange((IEnumerable<object>)dic[key]);
                            }
                            else
                            {
                                lista.Add(dic[key]);
                            }

                            // transforma a lista em um resultado
                            if (lista.Count > 0)
                            {
                                if (lista.Count > 1)
                                {
                                    result[key] = lista.ToArray();
                                }
                                else
                                {
                                    result[key] = lista.First();
                                }
                            }
                        }
                        else if (dic[key].GetType() != typeof(string) && (Ext.IsArray(dic[key]) || dic[key].IsList()))
                        {
                            result.Add(key, dic[key].ChangeType<object[]>());
                        }
                        else
                        {
                            result.Add(key, dic[key]);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Aplica as mesmas keys a todos os dicionarios de uma lista
        /// </summary>
        /// <typeparam name="TKey">Tipo da key</typeparam>
        /// <typeparam name="TValue">Tipo do Valor</typeparam>
        /// <param name="Dics">Dicionarios</param>
        /// <param name="AditionalKeys">
        /// Chaves para serem incluidas nos dicionários mesmo se não existirem em nenhum deles
        /// </param>
        public static IEnumerable<Dictionary<TKey, TValue>> MergeKeys<TKey, TValue>(this IEnumerable<Dictionary<TKey, TValue>> Dics, params TKey[] AditionalKeys)
        {
            AditionalKeys = AditionalKeys ?? Array.Empty<TKey>();
            Dics = Dics ?? Array.Empty<Dictionary<TKey, TValue>>();
            var chave = Dics.SelectMany(x => x.Keys).Distinct().Union(AditionalKeys);
            foreach (var dic in Dics)
            {
                if (dic != null)
                    foreach (var key in chave)
                    {
                        if (!dic.ContainsKey(key))
                        {
                            dic[key] = default;
                        }
                    }
            }
            return Dics.Select(x => x.OrderBy(y => y.Key).ToDictionary());
        }

        public static T SetValuesIn<T>(this Dictionary<string, object> Dic) => (T)Dic.CreateOrSetObject(null, typeof(T));

        /// <summary>
        /// Seta as propriedades de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <summary>
        /// Seta as propriedades e campos de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Obj"></param>
        public static T SetValuesIn<T>(this Dictionary<string, object> Dic, T obj, params object[] args) => (T)Dic.CreateOrSetObject(obj, typeof(T), args);

        /// <summary>
        /// Seta as propriedades e campos de uma classe a partir de um dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Obj"></param>
        public static T SetValuesIn<T>(this Dictionary<string, object> Dic, T obj) => (T)Dic.CreateOrSetObject(obj, typeof(T), null);

        /// <summary>
        /// Cria uma <see cref="List{T}"/> e adciona um objeto a ela. Util para tipos anonimos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> StartList<T>(this T ObjectForDefinition)
        {
            var d = DefineEmptyList(ObjectForDefinition);
            if (ObjectForDefinition != null)
            {
                d.Add(ObjectForDefinition);
            }

            return d;
        }

        /// <summary>
        /// Converte um ToType para Boolean. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static bool ToBool<T>(this T Value) => Value.ChangeType<bool>();

        /// <summary>
        /// Converte um ToType para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static DateTime ToDateTime<T>(this T Value) => Value.ChangeType<DateTime>();

        /// <summary>
        /// Converte um ToType para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static DateTime ToDateTime<T>(this T Value, string CultureInfoName) => Value.ToDateTime(new CultureInfo(CultureInfoName));

        /// <summary>
        /// Converte um ToType para DateTime. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static DateTime ToDateTime<T>(this T Value, CultureInfo CultureInfo) => Convert.ToDateTime(Value, CultureInfo);

        /// <summary>
        /// Converte um ToType para Decimal. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="T">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static decimal ToDecimal<T>(this T Value) => Value.ChangeType<decimal>();

        /// <summary>
        /// Retorna um <see cref="Dictionary"/> a partir de um <see cref="IGrouping(Of TKey, TElement)"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="groupings"></param>
        /// <returns></returns>
        public static Dictionary<TKey, IEnumerable<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> groupings) => groupings.ToDictionary(group => group.Key, group => group.AsEnumerable());

        /// <summary>
        /// Transforma uma lista de pares em um Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items, params TKey[] Keys) => items.Where(x => Keys == null || Keys.Any() == false || x.Key.IsIn(Keys)).DistinctBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);

        /// <summary>
        /// Converte um NameValueCollection para um <see cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="[NameValueCollection]">Formulario</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this NameValueCollection NameValueCollection, params string[] Keys)
        {
            var result = new Dictionary<string, object>();
            Keys = Keys ?? Array.Empty<string>();
            if (Keys.Any() == false)
            {
                Keys = NameValueCollection.AllKeys;
            }

            foreach (string key in NameValueCollection.Keys)
            {
                if (key.IsNotBlank() && key.IsLikeAny(Keys))
                {
                    var values = NameValueCollection.GetValues(key);
                    if (result.ContainsKey(key))
                    {
                        var l = new List<object>();
                        if (Ext.IsArray(result[key]))
                        {
                            foreach (var v in (IEnumerable)result[key])
                            {
                                switch (true)
                                {
                                    case object _ when v.IsNumber():
                                        {
                                            l.Add(Convert.ToDouble(v));
                                            break;
                                        }

                                    case object _ when Ext.IsDate(v):
                                        {
                                            l.Add(Convert.ToDateTime(v));
                                            break;
                                        }

                                    default:
                                        {
                                            l.Add(v);
                                            break;
                                        }
                                }
                            }
                        }
                        else
                        {
                            switch (true)
                            {
                                case object _ when result[key].IsNumber():
                                    {
                                        l.Add(Convert.ToDouble(result[key]));
                                        break;
                                    }

                                case object _ when Ext.IsDate(result[key]):
                                    {
                                        l.Add(Convert.ToDateTime(result[key]));
                                        break;
                                    }

                                default:
                                    {
                                        l.Add(result[key]);
                                        break;
                                    }
                            }
                        }

                        if (l.Count == 1)
                        {
                            result[key] = l[0];
                        }
                        else
                        {
                            result[key] = l.ToArray();
                        }
                    }
                    else if (values.Length == 1)
                    {
                        switch (true)
                        {
                            case object _ when values[0].IsNumber():
                                {
                                    result.Add(key, Convert.ToDouble(values[0]));
                                    break;
                                }

                            case object _ when values[0].IsDate():
                                {
                                    result.Add(key, Convert.ToDateTime(values[0]));
                                    break;
                                }

                            default:
                                {
                                    result.Add(key, values[0]);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        var ar = new List<object>();
                        foreach (var v in values)
                        {
                            switch (true)
                            {
                                case object _ when v.IsNumber():
                                    {
                                        ar.Add(Convert.ToDouble(v));
                                        break;
                                    }

                                case object _ when v.IsDate():
                                    {
                                        ar.Add(Convert.ToDateTime(v));
                                        break;
                                    }

                                default:
                                    {
                                        ar.Add(v);
                                        break;
                                    }
                            }
                        }

                        result.Add(key, ar.ToArray());
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Converte um ToType para Double. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static double ToDouble<FromType>(this FromType Value) => Value.ChangeType<double>();

        /// <summary>
        /// Converte um ToType para Integer. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static int ToInt<FromType>(this FromType Value) => Value.ChangeType<int>();

        /// <summary>
        /// Converte um ToType para Integer. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static long ToLong<FromType>(this FromType Value) => Value.ChangeType<long>();

        /// <summary>
        /// Converte um ToType para short. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static double ToShort<FromType>(this FromType Value) => Value.ChangeType<short>();

        /// <summary>
        /// Converte um ToType para Single. Retorna Nothing (NULL) se a conversão falhar
        /// </summary>
        /// <typeparam name="FromType">Tipo de origem</typeparam>
        /// <param name="Value">Variavel com valor</param>
        /// <returns>Valor convertido em novo ToType</returns>
        public static float ToSingle<FromType>(this FromType Value) => Value.ChangeType<float>();

        #endregion Public Methods

        #region Public Methods

        /// <summary>
        /// Descriptografa uma string
        /// </summary>
        /// <param name="Text">Texto Criptografado</param>
        /// <returns></returns>
        public static string Decrypt(this string Text, string Key = null)
        {
            if (Text.IsNotBlank())
            {
                byte[] Results = default;
                var UTF8 = new UTF8Encoding();
                var HashProvider = new MD5CryptoServiceProvider();
                var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("1234567890")));
                var TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                var DataToDecrypt = Convert.FromBase64String(Text);
                try
                {
                    var Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                TDESAlgorithm.Dispose();
                HashProvider.Dispose();
                return UTF8.GetString(Results);
            }

            return Text;
        }

        /// <summary>
        /// Descriptografa uma string
        /// </summary>
        /// <param name="Text">Texto Criptografado</param>
        /// <returns></returns>
        public static string Decrypt(this string text, string Key, string IV)
        {
            if (text.IsNotBlank())
            {
                var aes = new AesCryptoServiceProvider
                {
                    BlockSize = 128,
                    KeySize = 256,
                    IV = new UTF8Encoding(false).GetBytes(IV),
                    Key = new UTF8Encoding(false).GetBytes(Key),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                byte[] src;
                try
                {
                    src = Convert.FromBase64String(text.FixBase64());
                }
                catch
                {
                    src = Convert.FromBase64String(text);
                }

                using (var ddecrypt = aes.CreateDecryptor())
                {
                    try
                    {
                        var dest = ddecrypt.TransformFinalBlock(src, 0, src.Length);
                        return new UTF8Encoding(false).GetString(dest);
                    }
                    catch
                    {
                    }
                }
            }

            return text;
        }

        public static FileInfo DecryptRSA(this FileInfo File, string Key) => File?.ToBytes().DecryptRSA(Key).WriteToFile(File.FullName);

        /// <summary>
        /// Descriptografa um array de bytes encriptada em RSA
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static byte[] DecryptRSA(this byte[] bytes, string Key)
        {
            var cspp = new CspParameters() { KeyContainerName = Key };
            var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true };
            bytes = rsa.Decrypt(bytes, true);
            rsa.Dispose();
            return bytes;
        }

        /// <summary>
        /// Criptografa uma string
        /// </summary>
        /// <param name="Text">Texto descriptografado</param>
        /// <returns></returns>
        public static string Encrypt(this string Text, string Key = null)
        {
            if (Text.IsNotBlank())
            {
                byte[] Results = default;
                var UTF8 = new UTF8Encoding();
                var HashProvider = new MD5CryptoServiceProvider();
                var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("1234567890")));
                var TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                var DataToEncrypt = UTF8.GetBytes(Text);
                try
                {
                    var Encryptor = TDESAlgorithm.CreateEncryptor();
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                TDESAlgorithm.Dispose();
                HashProvider.Dispose();
                return Convert.ToBase64String(Results);
            }

            return Text;
        }

        /// <summary>
        /// Criptografa uma string
        /// </summary>
        /// <param name="Text">Texto descriptografado</param>
        /// <returns></returns>
        public static string Encrypt(this string text, string Key, string IV)
        {
            if (text.IsNotBlank())
            {
                var aes = new AesCryptoServiceProvider
                {
                    BlockSize = 128,
                    KeySize = 256,
                    IV = new UTF8Encoding(false).GetBytes(IV),
                    Key = new UTF8Encoding(false).GetBytes(Key),
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7
                };
                var src = new UTF8Encoding(false).GetBytes(text);
                using (var eencrypt = aes.CreateEncryptor())
                {
                    var dest = eencrypt.TransformFinalBlock(src, 0, src.Length);
                    return Convert.ToBase64String(dest);
                }
            }

            return text;
        }

        /// <summary>
        /// Criptografa um string em RSA
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string EncryptRSA(this string Text, string Key)
        {
            var cspp = new CspParameters() { KeyContainerName = Key };
            var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true };
            var bytes = rsa.Encrypt(new UTF8Encoding(false).GetBytes(Text), true);
            rsa.Dispose();
            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// Criptografa um array de bytes em RSA
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static byte[] EncryptRSA(this byte[] bytes, string Key)
        {
            var cspp = new CspParameters() { KeyContainerName = Key };
            using (var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true })
            {
                return rsa.Encrypt(bytes, true);
            }
        }

        public static FileInfo EncryptRSA(this FileInfo File, string Key) => File?.ToBytes().EncryptRSA(Key).WriteToFile(File.FullName);

        public static string ToMD5String(this string Text)
        {
            if (Text.IsNotBlank())
            {
                var md5 = MD5.Create();
                var inputBytes = Encoding.ASCII.GetBytes(Text);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (int i = 0, loopTo = hash.Length - 1; i <= loopTo; i++)
                {
                    sb.Append(hash[i].ToString("X2", CultureInfo.InvariantCulture));
                }

                return sb.ToString();
            }

            return Text;
        }

        #endregion Public Methods

        #endregion Public Methods

        #region Public Methods

        /// <summary>
        /// Format a file path using a <see cref="DateTime"/>
        /// </summary>
        /// <remarks>
        /// You can use any Datetime format (from <see cref="DateTime.ToString(string)"/>) or:
        /// <list type="table">
        /// <term>#timestamp#</term>
        /// <description>Will be replaced with <see cref="DateTime.Ticks"/></description>
        /// <br/>
        /// <term>#datedir#</term>
        /// <description>Will be replaced with a directory path <b>year\month\day</b></description>
        /// <br/>
        /// </list>
        /// </remarks>
        /// <param name="DateAndTime"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static string FormatPath(this DateTime? DateAndTime, string FilePath, bool AlternativeChar = false)
        {
            DateAndTime = DateAndTime ?? DateTime.Now;
            FilePath = FilePath.Replace($"#timestamp#", DateAndTime.Value.Ticks.ToString());
            FilePath = FilePath.Replace($"#datedir#", $@"{DateAndTime.Value.Year}\{DateAndTime.Value.Month}\{DateAndTime.Value.Day}");

            foreach (string item in new[] { "d", "dd", "ddd", "dddd", "hh", "HH", "m", "mm", "M", "MM", "MMM", "MMMM", "s", "ss", "t", "tt", "Y", "YY", "YYY", "YYYY", "f", "ff", "fff", "ffff", "fffff", "ffffff", "fffffff" })
            {
                FilePath = FilePath.SensitiveReplace($"#{item}#", DateAndTime.Value.ToString(item));
            }

            return FilePath.FixPath(AlternativeChar);
        }

        /// <summary>
        /// Retorna o nome do diretorio onde o arquivo se encontra
        /// </summary>
        /// <param name="Path">Caminho do arquivo</param>
        /// <returns>o nome do diretório sem o caminho</returns>
        public static string GetLatestDirectoryName(this FileInfo Path) => System.IO.Path.GetDirectoryName(Path.DirectoryName);

        /// <summary>
        /// Retorna o conteudo de um arquivo de texto
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string ReadAllText(this FileInfo File, Encoding encoding = null) => File != null && File.Exists ? System.IO.File.ReadAllText(File.FullName, encoding ?? Encoding.UTF8) : InnerLibs.Ext.EmptyString;

        /// <summary>
        /// Renomeia um arquivo e retorna um <see cref="FileInfo"/> do arquivo renomeado
        /// </summary>
        /// <param name="File"></param>
        /// <param name="Name"></param>
        /// <param name="KeepOriginalExtension"></param>
        /// <returns></returns>
        public static FileInfo Rename(this FileInfo File, string Name, bool KeepOriginalExtension = false)
        {
            if (File != null && Name.IsNotBlank())
            {
                if (KeepOriginalExtension)
                {
                    Name = $"{Path.GetFileNameWithoutExtension(Name)}.{File.Extension.Trim('.')}";
                }

                var pt = Path.Combine(File.DirectoryName, Name);
                File.MoveTo(pt);
                File = new FileInfo(pt);
            }
            return File;
        }

        /// <summary>
        /// Salva um anexo para um diretório
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="Directory"></param>
        /// <returns></returns>
        public static FileInfo SaveMailAttachment(this System.Net.Mail.Attachment attachment, DirectoryInfo Directory, DateTime? DateAndTime = null) => attachment.SaveMailAttachment(Directory.FullName, DateAndTime);

        /// <summary>
        /// Salva um anexo para um caminho
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static FileInfo SaveMailAttachment(this System.Net.Mail.Attachment attachment, string FilePath, DateTime? DateAndTime = null)
        {
            if (attachment != null)
            {
                if (FilePath.IsDirectoryPath())
                {
                    FilePath = FilePath + @"\" + attachment.Name.IfBlank(attachment.ContentId);
                }

                return attachment.ToBytes().WriteToFile(FilePath, DateAndTime);
            }

            return null;
        }

        /// <summary>
        /// Salva um anexo para Byte()
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this System.Net.Mail.Attachment attachment) => attachment?.ContentStream.ToBytes() ?? Array.Empty<byte>();

        /// <summary>
        /// Converte um stream em Bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this Stream stream)
        {
            if (stream == null)
            {
                return Array.Empty<byte>();
            }

            var pos = stream.Position;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                stream.Position = pos;
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converte o conteúdo de um <see cref="FileInfo"/> em <see cref="byte[]"/>
        /// </summary>
        /// <param name="File"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this FileInfo File)
        {
            if (File != null)
            {
                using (var fStream = new FileStream(File.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (var br = new BinaryReader(fStream))
                    {
                        return br.ReadBytes((int)File.Length);
                    }
                }
            }
            return Array.Empty<byte>();
        }

        /// <summary>
        /// salva um <see cref="Stream"/> em um arquivo
        /// </summary>
        /// <param name="Stream">stream a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos FilePath. default é <see cref="DateTime.Now"/>
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this Stream Stream, string FilePath, DateTime? DateAndTime = null) => Stream.ToBytes().WriteToFile(FilePath, DateAndTime);

        /// <summary>
        /// Salva um Array de Bytes em um arquivo
        /// </summary>
        /// <param name="Bytes">A MAtriz com os Bytes a ser escrita</param>
        /// <param name="FilePath">Caminho onde o arquivo será gravado</param>
        /// <param name="DateAndTime">
        /// DateTime utilizado como <see cref="FileSystemInfo.LastWriteTime"/> e como objeto de
        /// substituição nos FilePath. default é <see cref="DateTime.Now"/> ( <see cref="FormatPath"/>)
        /// </param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this byte[] Bytes, string FilePath, DateTime? DateAndTime = null)
        {
            Bytes = Bytes ?? Array.Empty<byte>();
            DateAndTime = DateAndTime ?? DateTime.Now;

            FilePath = DateAndTime.FormatPath(FilePath);

            if (FilePath.IsFilePath())
            {
                FilePath.CreateDirectoryIfNotExists();
                if (Bytes.Any())
                {
                    File.WriteAllBytes(FilePath, Bytes);
                    Ext.WriteDebug(FilePath, "File Written");
                }
                else
                {
                    Ext.WriteDebug("Bytes array is empty", "File not Written");
                }

                return new FileInfo(FilePath).With(x => { x.LastWriteTime = DateAndTime.Value; });
            }
            else
            {
                throw new ArgumentException($"FilePath is not a valid file FilePath: {FilePath}");
            }
        }

        /// <summary>
        /// Salva um array de bytes em um arquivo
        /// </summary>
        /// <param name="File">T arquivo a ser convertido</param>
        /// <returns>Um array do tipo Byte()</returns>
        /// <summary>
        /// Salva um texto em um arquivo
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="FilePath">Caminho do arquivo</param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, string FilePath, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null)
        {
            var s = new StreamWriter(FilePath, Append, Enconding ?? new UTF8Encoding(false));
            try
            {
                DateAndTime = DateAndTime ?? DateTime.Now;
                FilePath = DateAndTime.FormatPath(FilePath);
                FilePath.CreateDirectoryIfNotExists();
                s.Write(Text);
                s.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Can't write to file:", ex);
            }
            finally
            {
                s.Dispose();
            }

            return new FileInfo(FilePath);
        }

        /// <summary>
        /// Salva um texto em um arquivo
        /// </summary>
        /// <param name="Text">TExto</param>
        /// <param name="File">Arquivo</param>
        /// <returns>Um Fileinfo contendo as informações do arquivo criado</returns>
        public static FileInfo WriteToFile(this string Text, FileInfo File, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(File.FullName, Append, Enconding, DateAndTime);

        public static FileInfo WriteToFile(this string Text, DirectoryInfo Directory, string FileName, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(Path.Combine(Directory?.FullName, Path.GetFileName(FileName)), Append, Enconding, DateAndTime);

        public static FileInfo WriteToFile(this string Text, DirectoryInfo Directory, string SubDirectory, string FileName, bool Append = false, Encoding Enconding = null, DateTime? DateAndTime = null) => Text.WriteToFile(Path.Combine(Directory?.FullName, SubDirectory, Path.GetFileName(FileName)), Append, Enconding, DateAndTime);

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Dicionario com os <see cref="Type"/> e seu <see cref="DbType"/> correspondente
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Type, DbType> DbTypes => new Dictionary<Type, DbType>()
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char[])] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(byte[])] = DbType.Binary
        };

        /// <summary>
        /// Quando Configurado, escreve os parametros e queries executadas no <see
        /// cref="TextWriter"/> específico
        /// </summary>
        /// <returns></returns>
        public static TextWriter LogWriter { get; set; } = new DebugTextWriter();

        #endregion Public Properties

        #region Public Methods

        public static string AsSQLColumns(this IDictionary<string, object> obj, char Quote = '[') => obj.Select(x => x.Key.ToString().Quote(Quote)).SelectJoinString(",");

        public static string AsSQLColumns<T>(this T obj, char Quote = '[') where T : class => obj.GetNullableTypeOf().GetProperties().SelectJoinString(x => x.Name.Quote(Quote), ",");

        public static string AsSQLColumns(this NameValueCollection obj, char Quote = '[', params string[] Keys) => obj.ToDictionary(Keys).AsSQLColumns(Quote);

        /// <summary>
        /// Valida se uma conexao e um comando nao sao nulos. Valida se o texto do comando esta em
        /// branco e associa este comando a conexao especifica. Escreve o comando no <see
        /// cref="LogWriter"/> e retorna o mesmo
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DbCommand BeforeRunCommand(ref DbConnection Connection, ref DbCommand Command, TextWriter LogWriter = null)
        {
            Connection = Connection ?? Command?.Connection;
            if (Command == null || Command.CommandText.IsBlank())
            {
                throw new ArgumentException("Command is null or blank");
            }

            Command.Connection = Connection ?? throw new ArgumentException("Connection is null");
            if (!Connection.IsOpen())
            {
                Connection.Open();
            }

            return Command.LogCommand(LogWriter);
        }

        public static IEnumerable<string> ColumnsFromClass<T>()
        {
            var PropInfos = typeof(T).GetProperties().Select(y => y.GetAttributeValue<ColumnNameAttribute, string>(x => x.Names.FirstOrDefault()).IfBlank(y.Name));
            var FieldInfos = typeof(T).GetProperties().Select(y => y.GetAttributeValue<ColumnNameAttribute, string>(x => x.Names.FirstOrDefault()).IfBlank(y.Name)).Where(x => x.IsNotIn(PropInfos));

            return PropInfos.Union(FieldInfos);
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de um arquivo SQL e um objeto,
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand<T>(this DbConnection Connection, FileInfo SQLFile, T obj, DbTransaction Transaction = null) => CreateCommand(Connection, SQLFile.Exists ? SQLFile.ReadAllText() : InnerLibs.Ext.EmptyString, obj, Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um objeto,
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand<T>(DbConnection connection, string SQL, T obj, DbTransaction transaction = null) => CreateCommand(connection, SQL.Inject(obj, true).ToFormattableString(), transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see cref="Dictionary(Of
        /// String, Object)"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, Dictionary<string, object> Parameters, DbTransaction Transaction = null) => SQLFile != null && SQLFile.Exists ? CreateCommand(Connection, SQLFile.ReadAllText(), Parameters, Transaction) : null;

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see
        /// cref="NameValueCollection"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, NameValueCollection Parameters, DbTransaction Transaction = null) => Connection.CreateCommand(SQLFile, Parameters.ToDictionary(), Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see
        /// cref="NameValueCollection"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, NameValueCollection Parameters, DbTransaction Transaction = null) => Connection.CreateCommand(SQL, Parameters.ToDictionary(), Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string SQL e um <see
        /// cref="Dictionary{TKey, TValue}"/>, tratando os parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, Dictionary<string, object> Parameters, DbTransaction Transaction = null)
        {
            if (Connection != null && SQL.IsNotBlank())
            {
                var command = Connection.CreateCommand();
                command.CommandText = SQL;
                if (Parameters != null && Parameters.Any())
                {
                    foreach (var p in Parameters.Keys)
                    {
                        var v = Parameters.GetValueOr(p);
                        var arr = Ext.ForceArray(v, typeof(object)).ToList();
                        for (int index = 0, loopTo = arr.Count - 1; index <= loopTo; index++)
                        {
                            var param = command.CreateParameter();
                            if (arr.Count == 1)
                            {
                                param.ParameterName = $"__{p}";
                            }
                            else
                            {
                                param.ParameterName = $"__{p}_{index}";
                            }

                            param.Value = arr[index] ?? DBNull.Value;
                            command.Parameters.Add(param);
                        }
                    }
                }
                if (Transaction != null)
                {
                    command.Transaction = Transaction;
                }
                return command;
            }

            return null;
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string , tratando os parametros {p}
        /// desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, params string[] Args) => CreateCommand(Connection, SQL, null, Args);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string , tratando os parametros {p}
        /// desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, string SQL, DbTransaction Transaction, params string[] Args)
        {
            if (SQL.IsNotBlank())
            {
                return Connection.CreateCommand(SQL.ToFormattableString(Args), Transaction);
            }

            return null;
        }

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de um arquivo SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, params string[] Args) => CreateCommand(Connection, SQLFile.ReadAllText().ToFormattableString(Args));

        public static DbCommand CreateCommand(this DbConnection Connection, FileInfo SQLFile, DbTransaction Transaction, params string[] Args) => CreateCommand(Connection, SQLFile.ReadAllText().ToFormattableString(Args), Transaction);

        /// <summary>
        /// Cria um <see cref="DbCommand"/> a partir de uma string interpolada, tratando os
        /// parametros desta string como parametros SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static DbCommand CreateCommand(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
        {
            if (SQL != null && Connection != null && SQL.IsNotBlank())
            {
                var cmd = Connection.CreateCommand();
                if (SQL.ArgumentCount > 0)
                {
                    cmd.CommandText = SQL.Format;
                    for (int index = 0, loopTo = SQL.ArgumentCount - 1; index <= loopTo; index++)
                    {
                        var valores = SQL.GetArgument(index);
                        var v = Ext.ForceArray(valores, typeof(object)).ToList();
                        var param_names = new List<string>();
                        for (int v_index = 0, loopTo1 = v.Count() - 1; v_index <= loopTo1; v_index++)
                        {
                            var param = cmd.CreateParameter();
                            if (v.Count == 1)
                            {
                                param.ParameterName = $"__p{index}";
                            }
                            else
                            {
                                param.ParameterName = $"__p{index}_{v_index}";
                            }

                            param.Value = v[v_index] ?? DBNull.Value;
                            cmd.Parameters.Add(param);
                            param_names.Add("@" + param.ParameterName);
                        }

                        cmd.CommandText = cmd.CommandText.Replace("{" + index + "}", param_names.SelectJoinString(",").IfBlank("NULL").UnQuote('(', true).Quote('('));
                    }
                }
                else
                {
                    cmd.CommandText = SQL.ToString();
                }

                if (Transaction != null)
                {
                    cmd.Transaction = Transaction;
                }

                return cmd;
            }

            return null;
        }

        /// <summary>
        /// Cria comandos de INSERT para cada objeto do tipo <typeparamref name="T"/> em uma lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="obj"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static IEnumerable<DbCommand> CreateINSERTCommand<T>(this DbConnection Connection, IEnumerable<T> obj, string TableName = null, DbTransaction Transaction = null) where T : class => (obj ?? Array.Empty<T>()).Select(x => Connection.CreateINSERTCommand(x, TableName, Transaction));

        /// <summary>
        /// Cria um comando de INSERT para o objeto do tipo <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// <typeparamref name="T"/> pode ser uma classe, <see cref="NameValueCollection"/> ou <see
        /// cref="Dictionary{TKey, TValue}"/>
        /// </remarks>
        public static DbCommand CreateINSERTCommand<T>(this DbConnection Connection, T obj, string TableName = null, DbTransaction Transaction = null) where T : class
        {
            var d = typeof(T);
            var dic = new Dictionary<string, object>();
            if (obj != null && Connection != null)
            {
                dic = obj.CreateDictionary();

                var cmd = Connection.CreateCommand();
                cmd.CommandText = string.Format($"INSERT INTO {TableName.IfBlank(d.Name)} ({{0}}) values ({{1}})", dic.Keys.SelectJoinString(","), dic.Keys.SelectJoinString(x => $"@__{x}", ","));
                foreach (var k in dic.Keys)
                {
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"__{k}";
                    param.Value = dic.GetValueOr(k, DBNull.Value);
                    cmd.Parameters.Add(param);
                }
                if (Transaction != null)
                {
                    cmd.Transaction = Transaction;
                }
                return cmd;
            }

            return null;
        }

        public static SQLResponse<object> CreateSQLQuickResponse(this DbConnection Connection, FormattableString Command, string DataSetType, bool IncludeCommandText = false) => CreateSQLQuickResponse(Connection.CreateCommand(Command), DataSetType, IncludeCommandText);

        /// <summary>
        /// Executa um <paramref name="Command"/> e retorna uma <see cref="SQLResponse{object}"/> de
        /// acordo com o formato especificado em <paramref name="DataSetType"/>
        /// </summary>
        /// <remarks>
        /// Utilize as constantes de <see cref="DataSetType"/> no parametro <paramref name="DataSetType"/>
        /// </remarks>
        /// <param name="Command">Comando SQL com a <see cref="DbCommand.Connection"/> ja setada</param>
        /// <param name="DataSetType">Tipo da resposta. Ver <see cref="DataSetType"/></param>
        /// <returns></returns>
        public static SQLResponse<object> CreateSQLQuickResponse(this DbCommand Command, string DataSetType, bool IncludeCommandText = false)
        {
            var resp = new SQLResponse<object>();
            try
            {
                DataSetType = DataSetType.IfBlank("default").ToLowerInvariant();
                var Connection = Command?.Connection;
                if (Connection == null)
                {
                    resp.Status = "ERROR";
                    resp.Message = "Command or Connection is null";
                    return resp;
                }
                resp.SQL = IncludeCommandText.AsIf(Command.CommandText);
                if (DataSetType.IsAny("value", "single", "id", "key"))
                {
                    //primeiro valor da primeira linha do primeiro set
                    var part = Connection.RunSQLValue(Command);
                    resp.Status = (part == DBNull.Value).AsIf("NULL_VALUE", (part == null).AsIf("ZERO_RESULTS", "OK"));
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("one", "first", "row"))
                {
                    //primeiro do primeiro set (1 linha como objeto)
                    var part = Connection.RunSQLRow(Command);
                    resp.Status = (part == null).AsIf("ZERO_RESULTS", "OK");
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("array", "values", "list"))
                {
                    //primeira coluna do primeiro set como array
                    var part = Connection.RunSQLArray(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("pair", "pairs", "dictionary", "associative"))
                {
                    //primeira e ultima coluna do primeiro set como dictionary
                    var part = Connection.RunSQLPairs(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
                }
                else if (DataSetType.IsAny("many", "sets"))
                {
                    //varios sets
                    var part = Connection.RunSQLMany(Command);
                    resp.Status = (part?.Any(x => x.Any())).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
                }
                else
                {
                    //tudo do primeiro set (lista de objetos)
                    var part = Connection.RunSQLSet(Command);
                    resp.Status = (part?.Any()).AsIf("OK", "ZERO_RESULTS");
                    resp.Data = part;
                }
            }
            catch (Exception ex)
            {
                resp.Status = "ERROR";
                resp.Message = ex.ToFullExceptionString();
            }
            return resp;
        }

        /// <summary>
        /// Cria um comando de UPDATE para o objeto do tipo <typeparamref name="T"/>
        /// </summary>
        /// <remarks>
        /// <typeparamref name="T"/> pode ser uma classe, <see cref="NameValueCollection"/> ou <see
        /// cref="Dictionary{TKey, TValue}"/>
        /// </remarks>
        public static DbCommand CreateUPDATECommand<T>(this DbConnection Connection, T obj, FormattableString WhereClausule, string TableName = null, DbTransaction Transaction = null) where T : class
        {
            var d = typeof(T);
            Dictionary<string, object> dic;

            if (obj != null && Connection != null)
            {
                dic = obj.CreateDictionary();

                var cmd = Connection.CreateCommand();
                cmd.CommandText = $"UPDATE " + TableName.IfBlank(d.Name) + " set" + Environment.NewLine;
                foreach (var k in dic.Keys)
                {
                    cmd.CommandText += $"{k} = @__{k}, {Environment.NewLine}";
                    var param = cmd.CreateParameter();
                    param.ParameterName = $"__{k}";
                    param.Value = dic.GetValueOr(k, DBNull.Value);
                    cmd.Parameters.Add(param);
                }

                cmd.CommandText = cmd.CommandText.TrimAny(Environment.NewLine, ",", " ");

                if (WhereClausule.IsNotBlank())
                {
                    var wherecmd = Connection.CreateCommand(WhereClausule);
                    var wheretxt = wherecmd.CommandText.Trim();
                    foreach (DbParameter item in wherecmd.Parameters)
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = item.ParameterName;
                        param.Value = item.Value;
                        param.DbType = item.DbType;
                        cmd.Parameters.Add(param);
                    }
                    cmd.CommandText += $"{Environment.NewLine}{wheretxt.PrependIf("WHERE ", x => !x.StartsWith("WHERE"))}";
                    wherecmd.Dispose();
                }

                if (Transaction != null)
                {
                    cmd.Transaction = Transaction;
                }

                return cmd;
            }

            return null;
        }

        /// <summary>
        /// Formata o nome de uma coluna SQL adicionando <paramref name="QuoteChar"/> as <paramref
        /// name="ColumnNameParts"/> e as unindo com <b>.</b>
        /// </summary>
        /// <param name="QuoteChar"></param>
        /// <param name="ColumnNameParts"></param>
        /// <returns></returns>
        public static string FormatSQLColumn(char QuoteChar, params string[] ColumnNameParts) => ColumnNameParts.WhereNotBlank().SelectJoinString(x => x.UnQuote(QuoteChar).Quote(QuoteChar), ".");

        /// <inheritdoc cref="FormatSQLColumn(char, string[])"/>
        public static string FormatSQLColumn(params string[] ColumnNameParts) => FormatSQLColumn('[', ColumnNameParts);

        /// <summary>
        /// Retorna um <see cref="DbType"/> a partir do <see cref="Type"/> do <paramref name="obj"/>
        /// </summary>
        public static DbType GetDbType<T>(this T obj, DbType DefaultType = DbType.Object) => DbTypes.GetValueOr(Ext.GetNullableTypeOf(obj), DefaultType);

        public static DataRow GetFirstRow(this DataSet Data) => Data.GetFirstTable()?.GetFirstRow();

        public static DataRow GetFirstRow(this DataTable Table) => Table != null && Table.Rows.Count > 0 ? Table.Rows[0] : null;

        public static DataTable GetFirstTable(this DataSet Data) => Data != null && Data.Tables.Count > 0 ? Data.Tables[0] : null;

        public static T GetSingleValue<T>(this DataSet data, string ColumnNameOrIndex)
        {
            var row = data?.GetFirstRow();
            return row != null ? row.GetValue<T>(ColumnNameOrIndex) : default;
        }

        public static T GetSingleValue<T>(this DataSet data, int ColumnIndex = 0)
        {
            var row = data?.GetFirstRow();
            if (row != null)
                return row.GetValue<T>(ColumnIndex);
            return default;
        }

        public static T GetSingleValue<T>(this DataTable table, string ColumnNameOrIndex)
        {
            var row = table?.GetFirstRow();
            if (row != null)
                return row.GetValue<T>(ColumnNameOrIndex);
            return default;
        }

        public static T GetSingleValue<T>(this DataTable table, int ColumnIndex = 0)
        {
            var row = table?.GetFirstRow();
            if (row != null)
                return row.GetValue<T>(ColumnIndex);
            return default;
        }

        /// <summary>
        /// Retorna um <see cref="Type"/> de um <see cref="DbType"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <param name="DefaultType"></param>
        /// <returns></returns>
        public static Type GetTypeFromDb(this DbType Type, Type DefaultType = null) => DbTypes.Where(x => x.Value == Type).Select(x => x.Key).FirstOrDefault() ?? DefaultType ?? typeof(object);

        public static T GetValue<T>(this DataRow row, int ColumnIndex = 0)
        {
            try
            {
                return Ext.ChangeType<T>(row != null ? row[ColumnIndex] : default);
            }
            catch
            {
                return default;
            }
        }

        public static T GetValue<T>(this DataRow row, string ColumnNameOrIndex)
        {
            try
            {
                return Ext.ChangeType<T>(row != null ? row[ColumnNameOrIndex] : default);
            }
            catch
            {
                if (ColumnNameOrIndex.IsNumber())
                {
                    return GetValue<T>(row, ColumnNameOrIndex.ToInt());
                }
            }
            return default;
        }

        /// <inheritdoc cref="GetValue{T}(DataRow, string, Expression{Func{object, object}})"/>
        public static string GetValue(this DataRow row, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(row, Name, valueParser);

        /// <summary>
        /// Retorna o valor da coluna <paramref name="Name"/> de uma <see cref="DataRow"/>
        /// convertido para <typeparamref name="T"/> e previamente tratado pela função <paramref name="valueParser"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="Name"></param>
        /// <param name="valueParser"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DataRow row, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            try
            {
                if (row == null)
                {
                    throw new ArgumentException("Row is null");
                }

                object v = null;

                if (Name.IsNotBlank() && Name.IsNotNumber())
                {
                    v = row[Name];
                }
                else
                {
                    v = row[Name.IfBlank(0)];
                }

                if (v == null || v == DBNull.Value)
                {
                    return default;
                }

                if (valueParser != null)
                {
                    v = valueParser.Compile().Invoke(v);
                }

                return typeof(T).IsEnum ? v.ToString().GetEnumValue<T>() : v.ChangeType<T>();
            }
            catch (Exception ex)
            {
                LogWriter.WriteLine(ex.ToFullExceptionString());
                return default;
            }
        }

        public static string GetValue(this DataTable Table, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(Table, Name, valueParser);

        public static string GetValue(this DataSet Data, string Name = null, Expression<Func<object, object>> valueParser = null) => GetValue<string>(Data, Name, valueParser);

        public static T GetValue<T>(this DataSet Data, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            var r = Data.GetFirstRow();
            return r == null ? default : r.GetValue<T>(Name, valueParser);
        }

        public static T GetValue<T>(this DataTable Table, string Name = null, Expression<Func<object, object>> valueParser = null)
        {
            var r = Table.GetFirstRow();
            return r == null ? default : r.GetValue<T>(Name, valueParser);
        }

        public static bool IsBroken(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Broken);

        public static bool IsClosed(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Closed);

        public static bool IsConnecting(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Connecting);

        public static bool IsExecuting(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Executing);

        public static bool IsOpen(this DbConnection Connection) => Connection != null && (Connection.State == ConnectionState.Open);

        /// <summary>
        /// Utiliza o <see cref="TextWriter"/> especificado em <see cref="LogWriter"/> para escrever
        /// o comando
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static DbCommand LogCommand(this DbCommand Command, TextWriter LogWriter = null)
        {
            Ext.LogWriter = Ext.LogWriter ?? new DebugTextWriter();
            LogWriter = LogWriter ?? Ext.LogWriter;
            LogWriter.WriteLine(Environment.NewLine);
            LogWriter.WriteLine("=".Repeat(10));
            if (Command != null)
            {
                foreach (DbParameter item in Command.Parameters)
                {
                    string bx = $"Parameter: @{item.ParameterName}{Environment.NewLine}Value: {item.Value}{Environment.NewLine}T: {item.DbType}{Environment.NewLine}Precision/Scale: {item.Precision}/{item.Scale}";
                    LogWriter.WriteLine(bx);
                    LogWriter.WriteLine("-".Repeat(10));
                }

                LogWriter.WriteLine($"Command: {Command.CommandText}");
                LogWriter.WriteLine("/".Repeat(10));

                if (Command.Transaction != null)
                {
                    LogWriter.WriteLine($"Transaction Isolation Level: {Command.Transaction.IsolationLevel}");
                }
                else
                {
                    LogWriter.WriteLine($"No transaction specified");
                }
            }
            else LogWriter.WriteLine("Command is NULL");
            LogWriter.WriteLine("=".Repeat(10));
            LogWriter.WriteLine(Environment.NewLine);

            return Command;
        }

        public static T Map<T>(this DataRow Row, params object[] args) where T : class
        {
            T d;
            if (args.Any())
            {
                d = (T)Activator.CreateInstance(typeof(T), args);
            }
            else
            {
                d = Activator.CreateInstance<T>();
            }

            if (Row?.Table?.Columns != null)
                for (int ii = 0; ii < Row.Table.Columns.Count; ii++)
                {
                    var col = Row.Table.Columns[ii];
                    string name = col.ColumnName;
                    var value = Row.GetValue(name);
                    if (d is Dictionary<string, object> dic)
                    {
                        dic.Set(name, value);
                    }
                    else if (d is NameValueCollection nvc)
                    {
                        nvc.Add(name, $"{value}");
                    }
                    else
                    {
                        var PropInfos = Ext.GetTypeOf(d).FindProperties(name);
                        var FieldInfos = Ext.GetTypeOf(d).FindFields(name).Where(x => x.Name.IsNotIn(PropInfos.Select(y => y.Name)));
                        foreach (var info in PropInfos)
                        {
                            if (info.CanWrite)
                            {
                                if (value == null || ReferenceEquals(value.GetType(), typeof(DBNull)))
                                {
                                    info.SetValue(d, null);
                                }
                                else
                                {
                                    info.SetValue(d, Ext.ChangeType(value, info.PropertyType));
                                }
                            }
                        }

                        foreach (var info in FieldInfos)
                        {
                            if (ReferenceEquals(value.GetType(), typeof(DBNull)))
                            {
                                info.SetValue(d, null);
                            }
                            else
                            {
                                info.SetValue(d, Ext.ChangeType(value, info.FieldType));
                            }
                        }
                    }
                }
            return d;
        }

        public static IEnumerable<T> Map<T>(this DataTable Data, params object[] args) where T : class
        {
            var l = new List<T>();
            args = args ?? Array.Empty<object>();
            if (Data != null)
                for (int i = 0; i < Data.Rows.Count; i++) l.Add(Data.Rows[i].Map<T>(args));

            return l.AsEnumerable();
        }

        /// <summary>
        /// Mapeia o resultado de um <see cref="DbDataReader"/> para um <see cref="object"/>, <see
        /// cref="Dictionary{TKey, TValue}"/> ou <see cref="NameValueCollection"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static IEnumerable<T> Map<T>(this DbDataReader Reader, params object[] args) where T : class
        {
            var l = new List<T>();
            args = args ?? Array.Empty<object>();
            while (Reader != null && Reader.Read())
            {
                T d;
                if (args.Any())
                {
                    d = (T)Activator.CreateInstance(typeof(T), args);
                }
                else
                {
                    d = Activator.CreateInstance<T>();
                }

                for (int i = 0, loopTo = Reader.FieldCount - 1; i <= loopTo; i++)
                {
                    string name = Reader.GetName(i);
                    var value = Reader.GetValue(i);
                    if (typeof(T) == typeof(Dictionary<string, object>))
                    {
                        ((Dictionary<string, object>)(object)d).Set(name, value);
                    }
                    else if (typeof(T) == typeof(NameValueCollection))
                    {
                        ((NameValueCollection)(object)d).Add(name, $"{value}");
                    }
                    else
                    {
                        var propnames = name.PropertyNamesFor().ToList();
                        var PropInfos = Ext.GetTypeOf(d).GetProperties().Where(x => x.GetCustomAttributes<ColumnNameAttribute>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
                        var FieldInfos = Ext.GetTypeOf(d).GetFields().Where(x => x.GetCustomAttributes<ColumnNameAttribute>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase)).Where(x => x.Name.IsNotIn(PropInfos.Select(y => y.Name)));
                        foreach (var info in PropInfos)
                        {
                            if (info.CanWrite)
                            {
                                if (value == null || ReferenceEquals(value.GetType(), typeof(DBNull)))
                                {
                                    info.SetValue(d, null);
                                }
                                else
                                {
                                    info.SetValue(d, Ext.ChangeType(value, info.PropertyType));
                                }
                            }
                        }

                        foreach (var info in FieldInfos)
                        {
                            if (ReferenceEquals(value.GetType(), typeof(DBNull)))
                            {
                                info.SetValue(d, null);
                            }
                            else
                            {
                                info.SetValue(d, Ext.ChangeType(value, info.FieldType));
                            }
                        }
                    }
                }

                l.Add(d);
            }

            return l.AsEnumerable();
        }

        public static T MapFirst<T>(this DataSet Data, params object[] args) where T : class => Data.GetFirstRow().Map<T>(args);

        public static T MapFirst<T>(this DataTable Data, params object[] args) where T : class => Data.GetFirstRow().Map<T>(args);

        /// <summary>
        /// Mapeia a primeira linha de um datareader para uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Reader"></param>
        /// <param name="args">argumentos para o construtor da classe</param>
        /// <returns></returns>
        public static T MapFirst<T>(this DbDataReader Reader, params object[] args) where T : class => Reader.Map<T>(args).FirstOrDefault();

        /// <summary>
        /// Mapeia os resultsets de um datareader para um <see cref="IEnumerable(Of IEnumerable(Of
        /// Dictionary(Of String, Object)))"/>
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> MapMany(this DbDataReader Reader)
        {
            var l = new List<IEnumerable<Dictionary<string, object>>>();
            if (Reader != null)
            {
                do
                {
                    l.Add(Reader.Map<Dictionary<string, object>>());
                }
                while (Reader.NextResult());
            }

            return l.AsEnumerable();
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> MapMany<T1, T2, T3, T4, T5>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            IEnumerable<T3> o3 = null;
            IEnumerable<T4> o4 = null;
            IEnumerable<T5> o5 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }

                if (Reader.NextResult())
                {
                    o3 = Reader.Map<T3>();
                }

                if (Reader.NextResult())
                {
                    o4 = Reader.Map<T4>();
                }

                if (Reader.NextResult())
                {
                    o5 = Reader.Map<T5>();
                }
            }

            return Tuple.Create(o1, o2, o3, o4, o5);
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> MapMany<T1, T2, T3, T4>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            IEnumerable<T3> o3 = null;
            IEnumerable<T4> o4 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }

                if (Reader.NextResult())
                {
                    o3 = Reader.Map<T3>();
                }

                if (Reader.NextResult())
                {
                    o4 = Reader.Map<T4>();
                }
            }

            return Tuple.Create(o1, o2, o3, o4);
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> MapMany<T1, T2, T3>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            IEnumerable<T3> o3 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }

                if (Reader.NextResult())
                {
                    o3 = Reader.Map<T3>();
                }
            }

            return Tuple.Create(o1, o2, o3);
        }

        /// <summary>
        /// Mapeia os resultsets de um datareader para uma tupla de tipos especificos
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> MapMany<T1, T2>(this DbDataReader Reader)
            where T1 : class
            where T2 : class
        {
            IEnumerable<T1> o1 = null;
            IEnumerable<T2> o2 = null;
            if (Reader != null)
            {
                o1 = Reader.Map<T1>();
                if (Reader.NextResult())
                {
                    o2 = Reader.Map<T2>();
                }
            }

            return Tuple.Create(o1, o2);
        }

        public static TConnection OpenConnection<TConnection>(this ConnectionStringParser connection) where TConnection : DbConnection
        {
            if (connection != null)
            {
                TConnection dbcon = Activator.CreateInstance<TConnection>();
                dbcon.ConnectionString = connection.ConnectionString;
                dbcon.Open();
                return dbcon;
            }
            return null;
        }

        /// <summary>
        /// Processa uma propriedade de uma classe marcada com <see cref="FromSQL"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="d"></param>
        /// <param name="PropertyName"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        public static T ProccessSubQuery<T>(this DbConnection Connection, T d, string PropertyName, bool Recursive = false)
        {
            if (d != null)
            {
                var prop = d.GetProperty(PropertyName);
                if (prop != null)
                {
                    var attr = prop.GetCustomAttributes<FromSQLAttribute>(true).FirstOrDefault();
                    string Sql = attr.SQL.Inject(d);
                    bool gen = prop.PropertyType.IsGenericType;
                    bool lista = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
                    bool enume = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(IEnumerable<>));
                    bool cole = gen && prop.PropertyType.GetGenericTypeDefinition().IsAssignableFrom(typeof(ICollection<>));
                    if (lista || enume || cole)
                    {
                        IList baselist = (IList)Activator.CreateInstance(prop.PropertyType);
                        var eltipo = prop.PropertyType.GetGenericArguments().FirstOrDefault();
                        foreach (var x in Connection.RunSQLSet(Sql.ToFormattableString()))
                        {
                            baselist.Add(x.CreateOrSetObject(null, eltipo));
                        }

                        prop.SetValue(d, baselist);
                        if (Recursive)
                        {
                            foreach (var uu in baselist)
                            {
                                Connection.ProccessSubQuery(uu, Recursive);
                            }
                        }

                        return d;
                    }
                    else if (prop.PropertyType.IsClass)
                    {
                        if (prop.GetValue(d) == null)
                        {
                            var oo = Connection.RunSQLRow(Sql.ToFormattableString()).CreateOrSetObject(null, prop.PropertyType);
                            prop.SetValue(d, oo);
                            if (Recursive)
                            {
                                Connection.ProccessSubQuery(oo, Recursive);
                            }
                        }

                        return d;
                    }
                    else if (prop.PropertyType.IsValueType)
                    {
                        if (prop.GetValue(d) == null)
                        {
                            var oo = Connection.RunSQLValue(Sql.ToFormattableString());
                            prop.SetValue(d, Ext.ChangeType(oo, prop.PropertyType));
                            if (Recursive)
                            {
                                Connection.ProccessSubQuery(oo, Recursive);
                            }
                        }

                        return d;
                    }
                }
            }

            return d;
        }

        /// <summary>
        /// Processa todas as propriedades de uma classe marcadas com <see cref="FromSQL"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="d"></param>
        /// <param name="Recursive"></param>
        /// <returns></returns>
        public static T ProccessSubQuery<T>(this DbConnection Connection, T d, bool Recursive = false) where T : class
        {
            foreach (var prop in Ext.GetProperties(d).Where(x => x.HasAttribute<FromSQLAttribute>()))
            {
                Connection.ProccessSubQuery(d, prop.Name, Recursive);
            }

            return d;
        }

        public static string QueryForClass<T>(object InjectionObject = null) => typeof(T).GetAttributeValue<FromSQLAttribute, string>(x => x.SQL).IfBlank($"SELECT * FROM {typeof(T).Name}").Inject(InjectionObject);

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> RunSQLArray<T>(this DbConnection Connection, DbCommand Command) => Connection.RunSQLArray(Command).Select(x => x == null ? default : x.ChangeType<T>());

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array do tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static IEnumerable<T> RunSQLArray<T>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLArray<T>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array
        /// </summary>
        public static IEnumerable<object> RunSQLArray(this DbConnection Connection, DbCommand Command) => Connection.RunSQLSet(Command).Select(x => x.Values.FirstOrDefault());

        /// <summary>
        /// Retorna os resultado da primeira coluna de uma consulta SQL como um array
        /// </summary>
        public static IEnumerable<object> RunSQLArray(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLArray(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em listas de <see
        /// cref="Dictionary{TKey, TValue}"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> RunSQLMany(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLMany(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL e retorna todos os seus resultsets mapeados em uma <see
        /// cref="IEnumerable{IEnumerable{Dictionary{String, Object}}}"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<Dictionary<string, object>>> RunSQLMany(this DbConnection Connection, DbCommand Command)
        {
            IEnumerable<IEnumerable<Dictionary<string, object>>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos específicos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> RunSQLMany<T1, T2, T3, T4, T5>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class => Connection.RunSQLMany<T1, T2, T3, T4, T5>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> RunSQLMany<T1, T2, T3, T4, T5>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2, T3, T4, T5>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> RunSQLMany<T1, T2, T3, T4>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class => Connection.RunSQLMany<T1, T2, T3, T4>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> RunSQLMany<T1, T2, T3, T4>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2, T3, T4>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> RunSQLMany<T1, T2, T3>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class
            where T3 : class => Connection.RunSQLMany<T1, T2, T3>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> RunSQLMany<T1, T2, T3>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
            where T3 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2, T3>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> RunSQLMany<T1, T2>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null)
            where T1 : class
            where T2 : class
        {
            return Connection.RunSQLMany<T1, T2>(Connection.CreateCommand(SQL, Transaction));
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados mapeados em uma tupla de
        /// tipos especificos
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static Tuple<IEnumerable<T1>, IEnumerable<T2>> RunSQLMany<T1, T2>(this DbConnection Connection, DbCommand Command)
            where T1 : class
            where T2 : class
        {
            Tuple<IEnumerable<T1>, IEnumerable<T2>> resposta;
            using (var reader = Connection.RunSQLReader(Command))
            {
                resposta = reader.MapMany<T1, T2>();
            }

            return resposta;
        }

        /// <summary>
        /// Executa um comando SQL e retorna o numero de linhas afetadas
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static int RunSQLNone(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLNone(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o numero de linhas afetadas
        /// </summary>
        public static int RunSQLNone(this DbConnection Connection, DbCommand Command) => BeforeRunCommand(ref Connection, ref Command).ExecuteNonQuery();

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{Object, Object}"/>
        /// </summary>
        public static Dictionary<object, object> RunSQLPairs(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLSet(SQL).DistinctBy(x => x.Values.FirstOrDefault()).ToDictionary(x => x.Values.FirstOrDefault(), x => x.Values.LastOrDefault());

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{object,object}"/>
        /// </summary>
        public static Dictionary<object, object> RunSQLPairs(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLPairs(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{K, V}"/>
        /// </summary>
        public static Dictionary<TK, TV> RunSQLPairs<TK, TV>(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLPairs(SQL).ToDictionary(x => x.Key.ChangeType<TK>(), x => x.Value.ChangeType<TV>());

        /// <summary>
        /// Retorna os resultado das primeiras e ultimas colunas de uma consulta SQL como pares em
        /// um <see cref="Dictionary{K, V}"/>
        /// </summary>
        public static Dictionary<TK, TV> RunSQLPairs<TK, TV>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLPairs<TK, TV>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o <see cref="DbDataReader"/> com os resultados
        /// </summary>
        public static DbDataReader RunSQLReader(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLReader(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Executa um comando SQL e retorna o <see cref="DbDataReader"/> com os resultados
        /// </summary>
        public static DbDataReader RunSQLReader(this DbConnection Connection, DbCommand Command) => BeforeRunCommand(ref Connection, ref Command).ExecuteReader();

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados da primeira linha como um
        /// <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, Select<T> Select, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLRow<T>(Select.CreateDbCommand(Connection, Transaction), WithSubQueries);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// um <see cref="Dictionary{String, Object}"/>
        /// </summary>
        public static Dictionary<string, object> RunSQLRow(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLRow<Dictionary<string, object>>(SQL, false, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// um <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        public static Dictionary<string, object> RunSQLRow(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLRow<Dictionary<string, object>>(SQL);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, DbCommand SQL, bool WithSubQueries = false) where T : class
        {
            var x = Connection.RunSQLSet<T>(SQL, false).FirstOrDefault();
            if (x != null && WithSubQueries)
            {
                Connection.ProccessSubQuery(x, WithSubQueries);
            }

            return x ?? default;
        }

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna o resultado da primeira linha mapeada para
        /// uma classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static T RunSQLRow<T>(this DbConnection Connection, FormattableString SQL, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLRow<T>(Connection.CreateCommand(SQL, Transaction), WithSubQueries);

        public static T RunSQLRow<T>(this DbConnection Connection, bool WithSubQueries = false, DbTransaction Transaction = null, object InjectionObject = null) where T : class => RunSQLRow<T>(Connection, QueryForClass<T>(InjectionObject).ToFormattableString(), WithSubQueries, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <typeparamref name="T"/>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, Select<T> Select, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLSet<T>(Select.CreateDbCommand(Connection, Transaction), WithSubQueries);

        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, bool WithSubQueries = false, DbTransaction Transaction = null, object InjectionObject = null) where T : class => RunSQLSet<T>(Connection, QueryForClass<T>(InjectionObject).ToFormattableString(), WithSubQueries, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> RunSQLSet(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLSet<Dictionary<string, object>>(SQL, false, Transaction);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de <see cref="Dictionary(Of String, Object)"/>
        /// </summary>
        public static IEnumerable<Dictionary<string, object>> RunSQLSet(this DbConnection Connection, DbCommand SQL) => Connection.RunSQLSet<Dictionary<string, object>>(SQL);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, FormattableString SQL, bool WithSubQueries = false, DbTransaction Transaction = null) where T : class => Connection.RunSQLSet<T>(Connection.CreateCommand(SQL, Transaction), WithSubQueries);

        /// <summary>
        /// Executa uma query SQL parametrizada e retorna os resultados do primeiro resultset
        /// mapeados para uma lista de classe POCO do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Connection"></param>
        /// <param name="SQL"></param>
        /// <returns></returns>
        public static IEnumerable<T> RunSQLSet<T>(this DbConnection Connection, DbCommand SQL, bool WithSubQueries = false) where T : class
        {
            return Connection.RunSQLMany(SQL)?.FirstOrDefault()?.Select(x =>
            {
                T v = (T)x.CreateOrSetObject(null, typeof(T));
                if (WithSubQueries)
                {
                    Connection.ProccessSubQuery(v, WithSubQueries);
                }

                return v;
            }).AsEnumerable();
        }

        /// <summary>
        /// Retorna o primeiro resultado da primeira coluna de uma consulta SQL
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static object RunSQLValue(this DbConnection Connection, DbCommand Command) => BeforeRunCommand(ref Connection, ref Command).ExecuteScalar();

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL
        /// </summary>
        public static object RunSQLValue(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLValue(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL como um tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static T RunSQLValue<T>(this DbConnection Connection, DbCommand Command)
        {
            if (!typeof(T).IsValueType())
            {
                throw new ArgumentException("The type param T is not a value type or string");
            }
            var vv = Connection.RunSQLValue(Command);
            return vv != null && vv != DBNull.Value ? vv.ChangeType<T>() : default;
        }

        /// <summary>
        /// Retorna o valor da primeira coluna da primeira linha uma consulta SQL como um tipo
        /// <typeparamref name="T"/>
        /// </summary>
        public static T RunSQLValue<T>(this DbConnection Connection, FormattableString SQL, DbTransaction Transaction = null) => Connection.RunSQLValue<T>(Connection.CreateCommand(SQL, Transaction));

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica para cada item em uma
        /// coleçao. As propriedades do item serao utilizadas como parametros da procedure
        /// </summary>
        /// <param name="Items">Lista de itens que darao origem aos parametros da procedure</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">CHaves de Dicionário que devem ser utilizadas</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static IEnumerable<DbCommand> ToBatchProcedure<T>(this DbConnection Connection, string ProcedureName, IEnumerable<T> Items, DbTransaction Transaction = null, params string[] Keys)
        {
            foreach (var item in Items ?? new List<T>())
            {
                yield return Connection.ToProcedure(ProcedureName, item, Transaction, Keys);
            }
        }

        public static DataSet ToDataSet(this DbDataReader reader) => ToDataSet(reader, null);

        public static DataSet ToDataSet(this DbDataReader reader, string DataSetName, params string[] TableNames)
        {
            DataSet ds = new DataSet(DataSetName.IfBlank("DataSet"));
            TableNames = TableNames ?? Array.Empty<string>();
            var i = 0;
            while (reader != null && !reader.IsClosed)
            {
                ds.Tables.Add(TableNames.IfBlankOrNoIndex(i, $"Table{i}")).Load(reader);
                i++;
            }
            return ds;
        }

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata valores especificos
        /// de um NameValueCollection como parametros da procedure
        /// </summary>
        /// <param name="NVC">Objeto</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">Valores do nameValueCollection o que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static DbCommand ToProcedure(this DbConnection Connection, string ProcedureName, NameValueCollection NVC, DbTransaction Transaction = null, params string[] Keys) => Connection.ToProcedure(ProcedureName, NVC.ToDictionary(Keys), Transaction, Keys);

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata propriedades
        /// específicas de um objeto como parametros da procedure
        /// </summary>
        /// <param name="Obj">Objeto</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">propriedades do objeto que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static DbCommand ToProcedure<T>(this DbConnection Connection, string ProcedureName, T Obj, DbTransaction Transaction = null, params string[] Keys) => Connection.ToProcedure(ProcedureName, Obj?.CreateDictionary() ?? new Dictionary<string, object>(), Transaction, Keys);

        /// <summary>
        /// Monta um Comando SQL para executar uma procedure especifica e trata os valores
        /// específicos de um <see cref="Dictionary{TKey, TValue}"/> como parametros da procedure
        /// </summary>
        /// <param name="Dic">Objeto</param>
        /// <param name="ProcedureName">Nome da Procedure</param>
        /// <param name="Keys">propriedades do objeto que devem ser utilizados</param>
        /// <returns>Um DbCommand parametrizado</returns>
        public static DbCommand ToProcedure(this DbConnection Connection, string ProcedureName, Dictionary<string, object> Dic, DbTransaction Transaction = null, params string[] Keys)
        {
            Dic = Dic ?? new Dictionary<string, object>();
            Keys = Keys ?? Array.Empty<string>();
            if (!Keys.Any())
            {
                Keys = Dic.Keys.ToArray();
            }
            else
            {
                Keys = Dic.Keys.ToArray().Where(x => x.IsLikeAny(Keys)).ToArray();
            }

            string sql = $"{ProcedureName} {Keys.SelectJoinString(key => $" @{key} = @__{key}", ", ")}";

            return Connection.CreateCommand(sql, Dic.ToDictionary(x => x.Key, x => x.Value), Transaction);
        }

        ///<summary> Monta um Comando SQL para executar um SELECT com
        /// filtros a partir de um <see cref="NameValueCollection" />
        /// </summary>
        /// <param name="NVC"> Dicionario</param> <param name="TableName">Nome da Tabela</param>
        public static Select ToSQLFilter(this NameValueCollection NVC, string TableName, string CommaSeparatedColumns, params string[] FilterKeys) => (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(NVC, FilterKeys);

        /// <summary>
        /// Monta um Comando SQL para executar um SELECT com filtros a partir de um <see
        /// cref="Dictionary{string, object}"/>
        /// </summary>
        /// <param name="Dic">Dicionario</param>
        /// <param name="TableName">Nome da Tabela</param>
        /// <param name="FilterKeys">Parametros da URL que devem ser utilizados</param>
        /// <returns>Uma string com o comando montado</returns>
        public static Select ToSQLFilter(this Dictionary<string, object> Dic, string TableName, string CommaSeparatedColumns, LogicConcatenationOperator LogicConcatenation, params string[] FilterKeys) => (Select)new Select(CommaSeparatedColumns.Split(",")).From(TableName).Where(Dic, LogicConcatenation, FilterKeys);

        /// <summary>
        /// Interploa um objeto de tipo <typeparamref name="T"/> em uma <see
        /// cref="FormattableString"/>, e retorna o resultado de <see
        /// cref="ToSQLString(FormattableString, bool)"/>
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static string ToSQLString<T>(this T Obj, bool Parenthesis = true) => ToSQLString("{0}".ToFormattableString(Obj), Parenthesis);

        /// <summary>
        /// Converte uma <see cref="FormattableString"/> para uma string SQL, tratando seus
        /// parametros como parametros da query
        /// </summary>
        /// <param name="Parenthesis">indica se o parametro deve ser encapsulando em parentesis</param>
        public static string ToSQLString(this FormattableString SQL, bool Parenthesis = true)
        {
            if (SQL != null)
            {
                if (SQL.ArgumentCount > 0)
                {
                    string CommandText = SQL.Format.Trim();
                    for (int index = 0, loopTo = SQL.ArgumentCount - 1; index <= loopTo; index++)
                    {
                        var valores = SQL.GetArgument(index);
                        var v = Ext.ForceArray(valores, typeof(object));
                        var paramvalues = new List<object>();

                        for (int v_index = 0, loopTo1 = v.Length - 1; v_index <= loopTo1; v_index++)
                        {
                            paramvalues.Add(v[v_index]);
                        }

                        var pv = paramvalues.Select(x =>
                        {
                            if (x == null)
                            {
                                return "NULL";
                            }
                            else if (Ext.GetNullableTypeOf(x).IsNumericType())
                            {
                                return x.ToString();
                            }
                            else if (Ext.IsDate(x))
                            {
                                return x.ToDateTime().ToSQLDateString().EscapeQuotesToQuery(true);
                            }
                            else if (Ext.IsBool(x))
                            {
                                return x.ToBool().AsIf("1", "0");
                            }
                            else if (x.IsTypeOf<Select>())
                            {
                                return x.ToString();
                            }
                            else
                            {
                                return x.ToString().EscapeQuotesToQuery(true);
                            }
                        }).ToList();
                        CommandText = CommandText.Replace("{" + index + "}", pv.SelectJoinString(",").IfBlank("NULL").UnQuote('(', true).QuoteIf(Parenthesis, '('));
                    }

                    return CommandText;
                }
                else
                {
                    return SQL.ToString(CultureInfo.InvariantCulture);
                }
            }

            return null;
        }

        #endregion Public Methods

        #region Public Enums

        public enum LogicConcatenationOperator
        {
            AND,
            OR
        }

        #endregion Public Enums

        #region Public Methods

        /// <summary>
        /// Decoda uma string em Util
        /// </summary>
        /// <param name="Base"></param>
        /// <param name="Encoding"></param>
        /// <returns></returns>
        public static string Atob(this string Base, Encoding Encoding = null)
        {
            if (Base.IsNotBlank())
            {
                Base = (Encoding ?? new UTF8Encoding(false)).GetString(Convert.FromBase64String(Base));
            }

            return Base;
        }

        /// <summary>
        /// Converte uma DATAURL ou Util String em um array de Bytes
        /// </summary>
        /// <param name="Base64StringOrDataURL">Util String ou DataURL</param>
        /// <returns></returns>
        public static byte[] Base64ToBytes(this string Base64StringOrDataURL) => Convert.FromBase64String(Base64StringOrDataURL.FixBase64());

        /// <summary>
        /// Cria um arquivo fisico a partir de uma Base64 ou DataURL
        /// </summary>
        /// <param name="Base64StringOrDataURL"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static FileInfo Base64ToFile(this string Base64StringOrDataURL, string FilePath) => Base64StringOrDataURL.Base64ToBytes().WriteToFile(FilePath);

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
                    DataUrlOrBase64String = DataUrlOrBase64String.GetAfter(",");
                }

                var imageBytes = Base64ToBytes(DataUrlOrBase64String);
                using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                {
                    ms.Write(imageBytes, 0, imageBytes.Length);
                    if (Width > 0 && Height > 0)
                    {
                        return Image.FromStream(ms, true).Resize(Width, Height, false);
                    }
                    else
                    {
                        return Image.FromStream(ms, true);
                    }
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
                Text = Convert.ToBase64String((Encoding ?? new UTF8Encoding(false)).GetBytes(Text));
            }

            return Text;
        }

        /// <summary>
        /// Arruma os caracteres de uma string Util
        /// </summary>
        /// <param name="Base64StringOrDataUrl">Base64String ou DataURL</param>
        /// <returns>Retorna apenas a Util</returns>
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
        /// Retorna verdadeiro se identificar que a string é base64
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static bool IsBase64(this string base64String)
        {
            // Credit: oybek https://stackoverflow.com/users/794764/oybek
            if (string.IsNullOrWhiteSpace(base64String) ||
                base64String.Length % 4 != 0 ||
                base64String.Contains(" ") ||
                base64String.Contains("\t") ||
                base64String.Contains("\r") ||
                base64String.Contains("\n"))
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                //ignore
            }

            return false;
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
        /// Converte um Array de Bytes em uma string Util
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <returns></returns>
        public static string ToBase64(this byte[] Bytes) => Convert.ToBase64String(Bytes);

        public static string ToBase64(this Image OriginalImage, System.Drawing.Imaging.ImageFormat OriginalImageFormat)
        {
            using (var ms = new MemoryStream())
            {
                OriginalImage.Save(ms, OriginalImageFormat);
                var imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        /// <summary>
        /// Converte uma Imagem para String Util
        /// </summary>
        /// <param name="OriginalImage">
        /// Imagem original, tipo Image() (Picturebox.Image, Picturebox.BackgroundImage etc.)
        /// </param>
        /// <returns>Uma string em formato Util</returns>
        public static string ToBase64(this Image OriginalImage)
        {
            using (var ms = new MemoryStream())
            {
                OriginalImage.Save(ms, OriginalImage.GetImageFormat());
                var imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        /// <summary>
        /// Converte uma Imagem da WEB para String Util
        /// </summary>
        /// <param name="ImageURL">Caminho da imagem</param>
        /// <returns>Uma string em formato Util</returns>
        public static string ToBase64(this Uri ImageURL)
        {
            if (ImageURL != null)
            {
                var imagem = Ext.DownloadImage(ImageURL?.AbsoluteUri);
                using (var m = new MemoryStream())
                {
                    imagem.Save(m, imagem.RawFormat);
                    var imageBytes = m.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
            return null;
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
        public static string ToDataURL(this byte[] Bytes, FileType Type = null) => "data:" + (Type ?? new FileType()).ToString() + ";base64," + Bytes.ToBase64();

        /// <summary>
        /// Converte um Array de Bytes em uma DATA URL Completa
        /// </summary>
        /// <param name="Bytes">Array de Bytes</param>
        /// <param name="MimeType">Tipo de arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this byte[] Bytes, string MimeType) => "data:" + MimeType + ";base64," + Bytes.ToBase64();

        /// <summary>
        /// Converte um arquivo uma DATA URL Completa
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <returns></returns>
        public static string ToDataURL(this FileInfo File) => File.ToBytes().ToDataURL(new FileType(File.Extension));

        /// <summary>
        /// Transforma uma imagem em uma URL Util
        /// </summary>
        /// <param name="Image">Imagem</param>
        /// <returns>Uma DataURI em string</returns>
        public static string ToDataURL(this Image Image) => $"data:{Image.GetFileType().First().ToLowerInvariant().Replace("application/octet-stream", Ext.GetFileType(".png").First())};base64,{Image.ToBase64()}";

        /// <summary>
        /// Converte uma imagem para DataURI trocando o MIME T
        /// </summary>
        /// <param name="OriginalImage">Imagem</param>
        /// <param name="OriginalImageFormat">Formato da Imagem</param>
        /// <returns>Uma data URI com a imagem convertida</returns>
        public static string ToDataURL(this Image OriginalImage, System.Drawing.Imaging.ImageFormat OriginalImageFormat) => OriginalImage.ToBase64(OriginalImageFormat).Base64ToImage().ToDataURL();

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

        #endregion Public Methods

        /// <summary>
        /// Set this flag to true to show InnerLibs Debug messages
        /// </summary>
        public static bool EnableDebugMessages { get; set; }

        /// <summary>
        /// Tenta retornar um index de um IEnumerable a partir de um valor especifico. retorna -1 se
        /// o index nao existir
        /// </summary>
        /// <typeparam name="T">Tipo do IEnumerable e do valor</typeparam>
        /// <param name="Arr">Array</param>
        /// <returns></returns>
        public static int GetIndexOf<T>(this IEnumerable<T> Arr, T item)
        {
            try
            {
                return Arr.ToList().IndexOf(item);
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Write a message using <see cref="Debug.WriteLine(value,category)"/> when <see
        /// cref="Ext.EnableDebugMessages"/> is true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="category"></param>
        public static T WriteDebug<T>(this T value, string category = null)
        {
            if (EnableDebugMessages)
            {
                category = category.IfBlank("InnerLibs Debug");
                Debug.WriteLine(value, category);
            }
            return value;
        }

        #region Public Methods

        public static IEnumerable<TemplateMailAddress<T>> AddAttachmentFromData<T>(this IEnumerable<TemplateMailAddress<T>> recipients, Expression<Func<T, IEnumerable<System.Net.Mail.Attachment>>> AttachmentSelector) where T : class
        {
            if (AttachmentSelector != null)
                foreach (var rec in recipients ?? Array.Empty<TemplateMailAddress<T>>())
                {
                    if (rec?.TemplateData != null)
                    {
                        var att = AttachmentSelector.Compile().Invoke(rec.TemplateData);
                        if (att != null)
                        {
                            rec.AddAttachment(att);
                        }
                    }
                }
            return recipients;
        }

        public static TemplateMailAddress<T> AddAttachmentFromData<T>(this TemplateMailAddress<T> recipient, Expression<Func<T, IEnumerable<System.Net.Mail.Attachment>>> AttachmentSelector) where T : class => AddAttachmentFromData(new[] { recipient }, AttachmentSelector).FirstOrDefault();

        public static TemplateMailAddress<T> AddAttachmentFromData<T>(this TemplateMailAddress<T> recipient, Expression<Func<T, System.Net.Mail.Attachment>> AttachmentSelector) where T : class => AddAttachmentFromData(new[] { recipient }, AttachmentSelector).FirstOrDefault();

        public static IEnumerable<TemplateMailAddress<T>> AddAttachmentFromData<T>(this IEnumerable<TemplateMailAddress<T>> recipients, Expression<Func<T, System.Net.Mail.Attachment>> AttachmentSelector) where T : class
        {
            if (AttachmentSelector != null)
                foreach (var rec in recipients ?? Array.Empty<TemplateMailAddress<T>>())
                {
                    if (rec?.TemplateData != null)
                    {
                        var att = AttachmentSelector.Compile().Invoke(rec.TemplateData);
                        if (att != null)
                        {
                            rec.AddAttachment(att);
                        }
                    }
                }
            return recipients;
        }

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="BoolExp">Expressão de teste de Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static TR AsIf<T, TR>(this T obj, Expression<Func<T, bool>> BoolExp, TR TrueValue, TR FalseValue = default) => obj == null || BoolExp == null ? FalseValue : BoolExp.Compile().Invoke(obj).AsIf(TrueValue, FalseValue);

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool Bool, T TrueValue, T FalseValue = default) => Bool ? TrueValue : FalseValue;

        /// <summary>
        /// Retorna um valor de um tipo especifico de acordo com um valor boolean
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Bool">Valor boolean</param>
        /// <param name="TrueValue">Valor se verdadeiro</param>
        /// <param name="FalseValue">valor se falso</param>
        /// <returns></returns>
        public static T AsIf<T>(this bool? Bool, T TrueValue, T FalseValue = default) => (Bool.HasValue && Bool.Value).AsIf(TrueValue, FalseValue);

        /// <inheritdoc cref="AsIf{T}(bool?, T, T)"/>
        public static T AsIf<T>(this bool? Bool, T TrueValue, T FalseValue, T NullValue)
        {
            if (Bool.HasValue)
                return Bool.Value.AsIf(TrueValue, FalseValue);
            else
                return NullValue;
        }

        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento
        /// que possuir um valor
        /// </summary>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(this string First, params string[] N) => BlankCoalesce(new[] { First }.Union(N ?? Array.Empty<string>()).ToArray());

        /// <summary>
        /// Verifica se dois ou mais string estão nulas ou em branco e retorna o primeiro elemento
        /// que possuir um valor
        /// </summary>
        /// <param name="N">Itens</param>
        /// <returns></returns>
        public static string BlankCoalesce(params string[] N) => (N ?? Array.Empty<string>()).FirstOr(x => x.IsNotBlank(), Ext.EmptyString);

        /// <summary>
        /// Verifica se uma lista, coleção ou array contem todos os itens de outra lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="List1">Lista 1</param>
        /// <param name="List2">Lista2</param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this IEnumerable<T> List1, IEnumerable<T> List2, IEqualityComparer<T> Comparer = null)
        {
            foreach (T value in List2 ?? Array.Empty<T>())
            {
                if (Comparer != null)
                {
                    if (!(List1 ?? Array.Empty<T>()).Contains(value, Comparer))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!(List1 ?? Array.Empty<T>()).Contains(value))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool ContainsAll<T>(this IEnumerable<T> List1, IEqualityComparer<T> Comparer, params T[] List2) => List1.ContainsAll((List2 ?? Array.Empty<T>()).AsEnumerable(), Comparer);

        /// <summary>
        /// Verifica se uma lista, coleção ou array contem um dos itens de outra lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="List1">Lista 1</param>
        /// <param name="List2">Lista2</param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IEnumerable<T> List1, IEnumerable<T> List2, IEqualityComparer<T> Comparer = null)
        {
            foreach (T value in List2.AsEnumerable() ?? Array.Empty<T>())
            {
                if (Comparer == null)
                {
                    if ((List1 ?? Array.Empty<T>()).Contains(value))
                    {
                        return true;
                    }
                }
                else if ((List1 ?? Array.Empty<T>()).Contains(value, Comparer))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converte um objeto para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="T">
        /// Tipo da classe, <see cref="NameValueCollection"/> ou <see cref="Dictionary{TKey, TValue}"/>
        /// </typeparam>
        /// <param name="Obj">valor do objeto</param>
        /// <param name="Keys">Chaves incluidas no dicionario final</param>
        /// <returns></returns>
        public static Dictionary<string, object> CreateDictionary<T>(this T Obj, params string[] Keys)
        {
            if (Obj != null)
            {
                Keys = Keys ?? Array.Empty<string>();
                if (Obj.IsDictionary())
                {
                    return ((Dictionary<string, object>)(object)Obj).ToDictionary(Keys);
                }
                else if (Obj.IsTypeOf<NameValueCollection>())
                {
                    return ((NameValueCollection)(object)Obj).ToDictionary(Keys);
                }
                else
                {
                    return Obj.GetTypeOf().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => (Keys.Any() == false || x.Name.IsLikeAny(Keys)) && x.CanRead).ToDictionary(prop => prop.Name, prop => prop.GetValue(Obj, null));
                }
            }
            return new Dictionary<string, object>();
        }

        /// <summary>
        /// Converte uma classe para um <see cref="Dictionary"/>
        /// </summary>
        /// <typeparam name="T">Tipo da classe</typeparam>
        /// <param name="Obj">Object</param>
        /// <returns></returns>
        public static IEnumerable<Dictionary<string, object>> CreateDictionaryEnumerable<T>(this IEnumerable<T> Obj) => (Obj ?? Array.Empty<T>()).Select(x => x.CreateDictionary());

        /// <summary>
        /// Cria um <see cref="Guid"/> a partir de uma string ou um novo <see cref="Guid"/> se a
        /// conversão falhar
        /// </summary>
        /// <param name="Source"></param>
        /// <returns></returns>
        public static Guid CreateGuidOrDefault(this string Source)
        {
            Guid g;
            if (Source.IsNotBlank() || !Guid.TryParse(Source, out g))
            {
                g = Guid.NewGuid();
            }

            return g;
        }

        public static T CreateObjectFromXML<T>(this string XML) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            T obj;
            using (var reader = new StringReader(XML))
            {
                obj = (T)serializer.Deserialize(reader);
            }

            return obj;
        }

        public static T CreateObjectFromXMLFile<T>(this FileInfo XML) where T : class => File.ReadAllText(XML.FullName).CreateObjectFromXML<T>();

        /// <summary>
        /// Converte um objeto para XML
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="obj">Valor do objeto</param>
        /// <returns>um <see cref="XmlDocument"/></returns>
        public static XmlDocument CreateXML<T>(this T obj) where T : class
        {
            var xs = new XmlSerializer(obj.GetType());
            var doc = new XmlDocument();
            using (var sw = new StringWriter())
            {
                xs.Serialize(sw, obj);
                doc.LoadXml(sw.ToString());
            }

            return doc;
        }

        /// <summary>
        /// Cria um arquivo a partir de qualquer objeto usando o <see cref="Ext.CreateXML()"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FileInfo CreateXmlFile(this object obj, string FilePath) => obj.CreateXML().ToXMLString().WriteToFile(FilePath);

        /// <summary>
        /// Remove um item de uma lista e retorna este item
        /// </summary>
        /// <typeparam name="T">Tipo do item</typeparam>
        /// <param name="List">Lista</param>
        /// <param name="Index">Posicao do item</param>
        /// <returns></returns>
        public static T Detach<T>(this List<T> List, int Index)
        {
            if (List != null && Index.IsBetween(0, List.Count))
            {
                var p = List.ElementAt(Index);
                List.RemoveAt(Index);
                return p;
            }
            return default;
        }

        /// <summary>
        /// Remove itens de uma lista e retorna uma outra lista com estes itens
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="Indexes"></param>
        /// <returns></returns>
        public static IEnumerable<T> DetachMany<T>(this List<T> List, params int[] Indexes)
        {
            var l = new List<T>();
            return List.MoveItems(ref l, Indexes);
        }

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<T, long> DistinctCount<T>(this IEnumerable<T> Arr) => Arr.Distinct().Select(p => new KeyValuePair<T, long>(p, Arr.Where(x => x.Equals(p)).LongCount())).OrderByDescending(p => p.Value).ToDictionary();

        /// <summary>
        /// Conta de maneira distinta items de uma coleçao a partir de uma propriedade
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<TProp, long> DistinctCount<T, TProp>(this IEnumerable<T> Arr, Func<T, TProp> Prop) => Arr.GroupBy(Prop).ToDictionary(x => x.Key, x => x.LongCount()).OrderByDescending(p => p.Value).ToDictionary();

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao e agrupa o resto
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<T, long> DistinctCountTop<T>(this IEnumerable<T> Arr, int Top, T Others)
        {
            var a = Arr.DistinctCount();
            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        /// <summary>
        /// Conta de maneira distinta N items de uma coleçao a partir de uma propriedade e agrupa o
        /// resto em outra
        /// </summary>
        /// <typeparam name="T">TIpo de Objeto</typeparam>
        /// <param name="Arr">colecao</param>
        /// <returns></returns>
        public static Dictionary<TProp, long> DistinctCountTop<T, TProp>(this IEnumerable<T> Arr, Func<T, TProp> Prop, int Top, TProp Others)
        {
            var a = Arr.DistinctCount(Prop);
            if (Top < 1)
            {
                return a;
            }

            var topN = a.TakeTop(Top, Others);
            return topN;
        }

        public static FieldInfo FindField(this Type type, string Name) => FindFields(type, Name).FirstOrDefault();

        public static IEnumerable<FieldInfo> FindFields(this Type type, params string[] Names)
        {
            if (type != null && Names != null)
            {
                var propnames = Names.SelectMany(x => x.PropertyNamesFor()).ToList();
                return type.GetFields().Where(x => x.GetCustomAttributes<ColumnNameAttribute>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
            }
            return Array.Empty<FieldInfo>();
        }

        public static IEnumerable<PropertyInfo> FindProperties(this Type type, params string[] Names)
        {
            if (type != null && Names != null)
            {
                var propnames = Names.SelectMany(x => x.PropertyNamesFor()).ToList();
                return type.GetProperties().Where(x => x.GetCustomAttributes<ColumnNameAttribute>().SelectMany(n => n.Names).Contains(x.Name) || x.Name.IsIn(propnames, StringComparer.InvariantCultureIgnoreCase));
            }
            return Array.Empty<PropertyInfo>();
        }

        public static PropertyInfo FindProperty(this Type type, string Name) => FindProperties(type, Name).FirstOrDefault();

        /// <summary>
        /// T primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstAny<T>(this IEnumerable<T> source, params Expression<Func<T, bool>>[] predicate)
        {
            predicate = predicate ?? Array.Empty<Expression<Func<T, bool>>>();
            for (int index = 0, loopTo = predicate.Length - 1; index <= loopTo; index++)
            {
                var v = source.FirstOrDefault(predicate[index].Compile());
                if (v != null)
                {
                    return v;
                }
            }

            return default;
        }

        /// <summary>
        /// T primeiro valor não nulo de acordo com uma lista de predicados executados nesta lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T FirstAnyOr<T>(this IEnumerable<T> source, T Alternate, params Expression<Func<T, bool>>[] predicate)
        {
            var item = (source ?? Array.Empty<T>()).FirstAny(predicate);
            return (item == null) ? Alternate : item;
        }

        /// <summary>
        /// Troca valor de <paramref name="FirstValue"/> pelo de <paramref name="SecondValue"/> se
        /// <paramref name="FirstValue"/> for maior que <paramref name="SecondValue"/> fazendo com
        /// que <paramref name="FirstValue"/> seja sempre menor que <paramref name="SecondValue"/>.
        /// Util para tratar ranges
        /// </summary>
        /// <remarks>
        /// Caso <paramref name="FirstValue"/> e/ou <paramref name="SecondValue"/> forem
        /// <b>null</b>, nada acontece
        /// </remarks>
        public static (T, T) FixOrder<T>(ref T FirstValue, ref T SecondValue) where T : IComparable
        {
            if (FirstValue != null && SecondValue != null)
            {
                if (FirstValue.IsGreaterThan(SecondValue))
                {
                    return Swap(ref FirstValue, ref SecondValue);
                }
            }

            return (FirstValue, SecondValue);
        }

        /// <summary>
        /// Troca valor de <paramref name="FirstValue"/> pelo de <paramref name="SecondValue"/> se
        /// <paramref name="FirstValue"/> for maior que <paramref name="SecondValue"/> fazendo com
        /// que <paramref name="FirstValue"/> seja sempre menor que <paramref name="SecondValue"/>.
        /// Util para tratar ranges. Se qualquer um dos 2 valores for null, copia o valor da outra
        /// variavel não <b>null</b>. Se ambas forem <b>null</b> nada acontece.
        /// </summary>
        public static (T, T) FixOrderNotNull<T>(ref T FirstValue, ref T SecondValue) where T : IComparable
        {
            if (FirstValue == null && SecondValue != null)
            {
                FirstValue = SecondValue;
            }

            if (SecondValue == null && FirstValue != null)
            {
                SecondValue = FirstValue;
            }

            if (SecondValue == null && FirstValue == null)
            {
                FirstValue = default;
                SecondValue = default;
            }

            return FixOrder(ref FirstValue, ref SecondValue);
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this MemberInfo prop, Expression<Func<TAttribute, TValue>> ValueSelector) where TAttribute : Attribute
        {
            if (prop.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att && att != null)
            {
                return att.GetAttributeValue(ValueSelector);
            }

            return default;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Expression<Func<TAttribute, TValue>> ValueSelector) where TAttribute : Attribute
        {
            if (type != null && type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() is TAttribute att && att != null)
            {
                return att.GetAttributeValue(ValueSelector);
            }

            return default;
        }

        public static TValue GetAttributeValue<TAttribute, TValue>(this TAttribute att, Expression<Func<TAttribute, TValue>> ValueSelector)
        {
            if (att != null)
            {
                if (ValueSelector == null)
                {
                    ValueSelector = x => x.ToString().ChangeType<TValue>();
                }
                try
                {
                    return ValueSelector.Compile()(att);
                }
                catch { }
            }
            return default;
        }

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de uma string </summary> <typeparam name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this string Name) => (T)GetEnumValue(Name, typeof(T));

        public static object GetEnumValue(this string Name, Type EnumType)
        {
            if (EnumType != null && EnumType.IsEnum)
                return Name.ChangeType(EnumType);
            throw new ArgumentException("EnumType is not Enum", nameof(EnumType));
        }

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de um <paramref name="Value"/> inteiro </summary> <typeparam
        /// name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this int? Value) => Value.HasValue ? GetEnumValue<T>($"{Value.Value}") : default(T);

        /// <summary> Traz o valor de uma <see cref="Enum"> do tipo <typeparamref name="T"/> a
        /// partir de um <paramref name="Value"/> inteiro </summary> <typeparam
        /// name="T"></typeparam> <returns></returns>
        public static T GetEnumValue<T>(this int Value) => GetEnumValue<T>($"{Value}");

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this T Value)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("type must be an Enumeration type.", nameof(T));
            return Enum.GetName(typeof(T), Value);
        }

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this string Value) => Value.GetEnumValue<T>().GetEnumValueAsString<T>();

        /// <summary>
        /// Traz a string correspondente ao <paramref name="Value"/> de uma <see cref="Enum"/> do
        /// tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetEnumValueAsString<T>(this int Value) => Value.GetEnumValue<T>().GetEnumValueAsString<T>();

        /// <summary>
        /// Traz todos os Valores de uma enumeração
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumValues<T>()
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("type must be an Enumeration type.", nameof(T));
            return Enum.GetValues(typeof(T)).Cast<T>().AsEnumerable();
        }

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static FieldInfo GetField<T>(this T MyObject, string Name) => MyObject.GetTypeOf().GetFields().SingleOrDefault(x => (x.Name ?? Ext.EmptyString) == (Name ?? Ext.EmptyString));

        public static IEnumerable<FieldInfo> GetFields<T>(this T MyObject, BindingFlags BindAttr) => MyObject.GetTypeOf().GetFields(BindAttr).ToList();

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<FieldInfo> GetFields<T>(this T MyObject) => MyObject.GetTypeOf().GetFields().ToList();

        public static IEnumerable<Type> GetInheritedClasses<T>() where T : class => GetInheritedClasses(typeof(T));

        public static IEnumerable<Type> GetInheritedClasses(this Type MyType)
        {
            //if you want the abstract classes drop the !TheType.IsAbstract but it is probably to instance so its a good idea to keep it.
            return Assembly.GetAssembly(MyType).GetTypes().Where(TheType => TheType.IsClass && !TheType.IsAbstract && TheType.IsSubclassOf(MyType));
        }

        public static string GetMemberName(MemberInfo member)
        {
            if (member != null)
            {
                if (member.IsDefined(typeof(DataMemberAttribute), true))
                {
                    DataMemberAttribute dataMemberAttribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute), true);
                    if (!string.IsNullOrEmpty(dataMemberAttribute.Name))
                        return dataMemberAttribute.Name;
                }

                return member.Name;
            }
            return null;
        }

        /// <summary>
        /// Retorna o <see cref="Type"/> equivalente a <typeparamref name="T"/> ou o <see
        /// cref="Type"/> do objeto <see cref="Nullable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>
        /// o tipo do objeto ou o tipo do objeto anulavel ou o prorio objeto se ele for um <see cref="Type"/>
        /// </returns>
        public static Type GetNullableTypeOf<T>(this T Obj)
        {
            var tt = Obj.GetTypeOf();
            tt = Nullable.GetUnderlyingType(tt) ?? tt;
            return tt;
        }

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this T MyObject, BindingFlags BindAttr) => MyObject.GetTypeOf().GetProperties(BindAttr).ToList();

        /// <summary>
        /// Traz uma Lista com todas as propriedades de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(this T MyObject) => MyObject.GetTypeOf().GetProperties().ToList();

        /// <summary>
        /// Traz uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static PropertyInfo GetProperty<T>(this T MyObject, string Name) => MyObject.GetTypeOf().GetProperties().SingleOrDefault(x => (x.Name ?? Ext.EmptyString) == (Name ?? Ext.EmptyString));

        /// <summary>
        /// Retorna uma <see cref="Hashtable"/> das propriedades de um objeto
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static Hashtable GetPropertyHash<T>(T properties)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                var props = TypeDescriptor.GetProperties(properties);
                foreach (PropertyDescriptor prop in props)
                {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }

            return values;
        }

        /// <summary>
        /// Traz o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <returns></returns>
        public static T GetPropertyValue<T, O>(this O MyObject, string Name) where O : class
        {
            if (MyObject != null)
            {
                var prop = MyObject.GetProperty(Name);
                if (prop != null && prop.CanRead)
                {
                    return (T)prop.GetValue(MyObject);
                }
            }

            return default;
        }

        /// <summary>
        /// Pega os bytes de um arquivo embutido no assembly
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static byte[] GetResourceBytes(this Assembly Assembly, string FileName) => Assembly.GetManifestResourceStream(FileName)?.ToBytes() ?? Array.Empty<byte>();

        public static byte[] GetResourceBytes(string FileName) => GetResourceBytes(Assembly.GetExecutingAssembly(), FileName);

        /// <summary>
        /// Pega o texto de um arquivo embutido no assembly
        /// </summary>
        /// <param name="FileName">Nome do arquivo embutido dentro do assembly (Embedded Resource)</param>
        /// <returns></returns>
        public static string GetResourceFileText(this Assembly Assembly, string FileName)
        {
            string txt = null;
            if (Assembly != null && FileName.IsNotBlank())
            {
                using (var x = Assembly.GetManifestResourceStream(FileName))
                {
                    if (x != null)
                    {
                        using (var r = new StreamReader(x))
                        {
                            txt = r.ReadToEnd();
                        }
                    };
                }
            }

            return txt;
        }

        public static string GetResourceFileText(string FileName) => GetResourceFileText(Assembly.GetExecutingAssembly(), FileName);

        /// <summary>
        /// Retorna o <see cref="Type"/> do objeto mesmo se ele for nulo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns>o tipo do objeto ou o prorio objeto se ele for um <see cref="Type"/></returns>
        public static Type GetTypeOf<T>(this T Obj)
        {
            if (Obj is Type istype)
            {
                return istype;
            }
            else
            {
                try
                {
                    return Obj.GetType();
                }
                catch { }
            }
            return typeof(T);
        }

        /// <summary>
        /// Tries to get a value from <see cref="Dictionary{TKey, TValue}"/>. if fails, return
        /// <paramref name="ReplaceValue"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Key"></param>
        /// <param name="ReplaceValue"></param>
        /// <remarks>
        /// if <paramref name="ReplaceValue"/> is not provided. the default value for type
        /// <typeparamref name="TValue"/> is returned
        /// </remarks>
        /// <returns></returns>
        public static TValue GetValueOr<TKey, TValue>(this IDictionary<TKey, TValue> Dic, TKey Key, TValue ReplaceValue = default) => Dic != null && Dic.ContainsKey(Key) ? Dic[Key] : ReplaceValue;

        /// <summary>
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, long> GroupAndCountBy<Type, Group>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector) => obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, long>(x.Key, x.LongCount())).ToDictionary();

        /// <summary>
        /// Agrupa itens de uma lista a partir de uma propriedade e conta os resultados de cada
        /// grupo a partir de outra propriedade do mesmo objeto
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <typeparam name="Count"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <param name="CountObjectBy"></param>
        /// <returns></returns>
        public static Dictionary<Group, Dictionary<Count, long>> GroupAndCountSubGroupBy<Type, Group, Count>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector, Func<Type, Count> CountObjectBy)
        {
            var dic_of_dic = obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, Dictionary<Count, long>>(x.Key, x.GroupBy(CountObjectBy).ToDictionary(y => y.Key, y => y.LongCount()))).ToDictionary();
            dic_of_dic.Values.MergeKeys();

            return dic_of_dic;
        }

        /// <summary>
        /// Agrupa itens de uma lista a partir de duas propriedades de um objeto resultado em um
        /// grupo com subgrupos daquele objeto
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="Group"></typeparam>
        /// <typeparam name="SubGroup"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <param name="SubGroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<Group, Dictionary<SubGroup, IEnumerable<Type>>> GroupAndSubGroupBy<Type, Group, SubGroup>(this IEnumerable<Type> obj, Func<Type, Group> GroupSelector, Func<Type, SubGroup> SubGroupSelector)
        {
            var dic_of_dic = obj.GroupBy(GroupSelector).Select(x => new KeyValuePair<Group, Dictionary<SubGroup, IEnumerable<Type>>>(x.Key, x.GroupBy(SubGroupSelector).ToDictionary(y => y.Key, y => y.AsEnumerable()))).ToDictionary();
            dic_of_dic.Values.MergeKeys();
            return dic_of_dic;
        }

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <param name="attribType"></param>
        /// <returns></returns>
        public static bool HasAttribute(this PropertyInfo target, Type attribType) => target?.GetCustomAttributes(attribType, false).Any() ?? false;

        /// <summary>
        /// Verifica se um atributo foi definido em uma propriedade de uma classe
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool HasAttribute<T>(this PropertyInfo target) => target?.HasAttribute(typeof(T)) ?? false;

        /// <summary>
        /// Verifica se um tipo possui uma propriedade
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public static bool HasProperty(this Type Type, string PropertyName, bool GetPrivate = false)
        {
            if (Type != null && PropertyName.IsNotBlank())
            {
                var parts = new List<string>();
                bool stop = false;
                string current = Ext.EmptyString;
                for (int i = 0, loopTo = PropertyName.Length - 1; i <= loopTo; i++)
                {
                    if (PropertyName[i] != '.')
                    {
                        current += $"{PropertyName[i]}";
                    }

                    if (PropertyName[i] == '[')
                    {
                        stop = true;
                    }

                    if (PropertyName[i] == ']')
                    {
                        stop = false;
                    }

                    if (PropertyName[i] == '.' && !stop || i == PropertyName.Length - 1)
                    {
                        parts.Add(current.ToString());
                        current = Ext.EmptyString;
                    }
                }

                PropertyInfo prop;
                string propname = parts.First().GetBefore("[");
                if (GetPrivate)
                {
                    prop = Type.GetProperty(propname, (BindingFlags)((int)BindingFlags.Public + (int)BindingFlags.NonPublic + (int)BindingFlags.Instance));
                }
                else
                {
                    prop = Type.GetProperty(propname);
                }

                bool exist = prop != null;
                parts.RemoveAt(0);
                if (exist && parts.Count > 0)
                {
                    exist = prop.PropertyType.HasProperty(parts.First(), GetPrivate);
                }

                return exist;
            }

            return false;
        }

        /// <summary>
        /// Verifica se um tipo possui uma propriedade
        /// </summary>
        /// <param name="Obj"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static bool HasProperty(this object Obj, string Name) => Obj?.GetType().HasProperty(Name, true) ?? false;

        public static bool IsAny<T>(this T obj, params T[] others) => others?.Any(x => x.Equals(obj)) ?? false;

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this Type Type) => Type == typeof(T[]);

        /// <summary>
        /// Verifica se o tipo é um array de um objeto especifico
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsArrayOf<T>(this object Obj) => Obj.GetTypeOf().IsArrayOf<T>();

        /// <summary>
        /// Verifica se <paramref name="Value"/> é igual a <paramref name="MinValue"/> ou está entre
        /// <paramref name="MinValue"/> e <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <b>true</b> se <paramref name="Value"/> for igual a <paramref name="MinValue"/>.
        /// Retorna <b>false</b> se <paramref name="Value"/> for igual a <paramref
        /// name="MaxValue"/>. <br/> Utilize <see cref="IsBetweenOrEqual(IComparable, IComparable,
        /// IComparable)"/> para incluir <paramref name="MaxValue"/> ou <see
        /// cref="IsBetweenExclusive(IComparable, IComparable, IComparable)"/> para excluir
        /// <paramref name="MinValue"/>
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetween(this IComparable Value, IComparable MinValue, IComparable MaxValue)
        {
            FixOrder(ref MinValue, ref MaxValue);
            return MinValue.Equals(MaxValue) ? Value.Equals(MinValue) : Value.IsGreaterThanOrEqual(MinValue) && Value.IsLessThan(MaxValue);
        }

        /// <summary>
        /// Verifica se <paramref name="Value"/> está entre <paramref name="MinValue"/> e <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <see cref="false"/> se <paramref name="Value"/> for igual a <paramref
        /// name="MinValue"/> ou <paramref name="MaxValue"/>. <br/> Utilize <see
        /// cref="IsBetween(IComparable, IComparable, IComparable)"/> para incluir <paramref
        /// name="MinValue"/> ou <see cref="IsBetweenOrEqual(IComparable, IComparable,
        /// IComparable)"/> para incluir ambos
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetweenExclusive(this IComparable Value, IComparable MinValue, IComparable MaxValue)
        {
            FixOrder(ref MinValue, ref MaxValue);
            return MinValue != MaxValue && Value.IsGreaterThan(MinValue) && Value.IsLessThan(MaxValue);
        }

        /// <summary>
        /// Verifica se <paramref name="Value"/> é igual ou está entre <paramref name="MinValue"/> e
        /// <paramref name="MaxValue"/>
        /// </summary>
        /// <remarks>
        /// Retorna <b>true</b> se <paramref name="Value"/> for igual a <paramref name="MinValue"/>
        /// ou <paramref name="MaxValue"/>. <br/> Utilize <see cref="IsBetween(IComparable,
        /// IComparable, IComparable)"/> para excluir <paramref name="MaxValue"/> ou <see
        /// cref="IsBetweenExclusive(IComparable, IComparable, IComparable)"/> para excluir ambos
        /// </remarks>
        /// <param name="Value">Numero</param>
        /// <param name="MinValue">Primeiro comparador</param>
        /// <param name="MaxValue">Segundo comparador</param>
        /// <returns></returns>
        public static bool IsBetweenOrEqual(this IComparable Value, IComparable MinValue, IComparable MaxValue)
        {
            FixOrder(ref MinValue, ref MaxValue);
            return MinValue == MaxValue ? Value == MinValue : Value.IsGreaterThanOrEqual(MinValue) && Value.IsLessThanOrEqual(MaxValue);
        }

        /// <summary>
        /// Verifica se o objeto é um iDictionary
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsDictionary(this object obj) => IsGenericOf(obj, typeof(IDictionary<,>)) || IsGenericOf(obj, typeof(IDictionary));

        /// <summary>
        /// Verifica se o objeto é um enumeravel (lista)
        /// </summary>
        /// <param name="obj"></param>
        /// <remarks>NÃO considera strings (IEnumerable{char}) como true</remarks>
        /// <returns></returns>
        public static bool IsEnumerable(this object obj) => IsGenericOf(obj, typeof(IEnumerable<>)) || IsGenericOf(obj, typeof(IEnumerable));

        public static bool IsEnumerableNotString(this object obj) => IsEnumerable(obj) && GetTypeOf(obj) != typeof(string);

        public static bool IsEqual<T>(this T Value, T EqualsToValue) where T : IComparable => Value.Equals(EqualsToValue);

        public static bool IsGenericOf(this object obj, Type GenericType)
        {
            var type = obj.GetTypeOf();

            if (type == null || GenericType == null) return false;
            if (type == GenericType) return true;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == GenericType) return true;
            if (GenericType.IsGenericType && GenericType.GetGenericTypeDefinition().IsAssignableFrom(type)) return true;
            if (GenericType.IsAssignableFrom(type)) return true;
            if (type.GetInterfaces().Append(type).Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == GenericType)) return true;
            return false;
        }

        /// <summary>
        /// Verifica se um tipo e generico de outro
        /// </summary>
        /// <param name="MainType"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool IsGreaterThan<T>(this T Value, T MinValue) where T : IComparable => Value.CompareTo(MinValue) > 0;

        public static bool IsGreaterThanOrEqual<T>(this T Value, T MinValue) where T : IComparable => Value.IsGreaterThan(MinValue) || Value.IsEqual(MinValue);

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<T>(this T Obj, params T[] List) => Obj.IsIn((List ?? Array.Empty<T>()).ToList());

        public static bool IsIn<T>(this T Obj, IEqualityComparer<T> Comparer = null, params T[] List) => Obj.IsIn((List ?? Array.Empty<T>()).ToList(), Comparer);

        /// <summary>
        /// Verifica se o objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsIn<T>(this T Obj, IEnumerable<T> List, IEqualityComparer<T> Comparer = null) => Comparer is null ? List.Contains(Obj) : List.Contains(Obj, Comparer);

        public static bool IsIn<T>(this T Obj, string Text, StringComparison? Comparer = null) => Comparer == null ? Text?.Contains(Obj.ToString()) ?? false : Text?.Contains(Obj.ToString(), Comparer.Value) ?? false;

        /// <summary>
        /// Verifica se o objeto existe dentro de uma ou mais Listas, coleções ou arrays.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsInAny<T>(this T Obj, IEnumerable<T>[] List, IEqualityComparer<T> Comparer = null) => (List ?? Array.Empty<IEnumerable<T>>()).Any(x => Obj.IsIn(x, Comparer));

        public static bool IsLessThan<T>(this T Value, T MaxValue) where T : IComparable => Value.CompareTo(MaxValue) < 0;

        public static bool IsLessThanOrEqual<T>(this T Value, T MaxValue) where T : IComparable => Value.IsLessThan(MaxValue) || Value.IsEqual(MaxValue);

        /// <summary>
        /// Verifica se o objeto é uma lista
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsList(this object obj) => IsGenericOf(obj, typeof(List<>));

        /// <summary>
        /// Verifica se o não objeto existe dentro de uma Lista, coleção ou array.
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="List">Lista</param>
        /// <returns></returns>
        public static bool IsNotIn<T>(this T Obj, IEnumerable<T> List, IEqualityComparer<T> Comparer = null) => !Obj.IsIn(List, Comparer);

        /// <summary>
        /// Verifica se o objeto não existe dentro de um texto
        /// </summary>
        /// <typeparam name="T">Tipo do objeto</typeparam>
        /// <param name="Obj">objeto</param>
        /// <param name="TExt">Texto</param>
        /// <returns></returns>
        public static bool IsNotIn<T>(this T Obj, string Text, StringComparison? Comparer = null) => !Obj.IsIn(Text, Comparer);

        /// <summary>
        /// Checks if a <paramref name="List"/> is not <b>null</b> and contains at least one item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> List) => (List ?? Array.Empty<T>()).Any();

        public static bool IsNullableType(this Type t) => t != null && t.IsGenericType && Nullable.GetUnderlyingType(t) != null;

        public static bool IsNullableType<T>(this T Obj) => IsNullableType(Obj.GetTypeOf());

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<T>(this object Obj) => Obj.IsNullableTypeOf(typeof(T));

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsNullableTypeOf<T>(this T Obj, Type Type) => Obj.GetNullableTypeOf() == Type.GetNullableTypeOf();

        /// <summary>
        /// Checks if a <paramref name="List"/> is <b>null</b> or empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> List) => !List.IsNotNullOrEmpty();

        /// <summary>
        /// Verifica se o objeto é do tipo numérico.
        /// </summary>
        /// <remarks>Boolean is not considered numeric.</remarks>
        public static bool IsNumericType<T>(this T Obj) => Obj.GetNullableTypeOf().IsIn(PredefinedArrays.NumericTypes);

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsTypeOf<T>(this object Obj) => Obj.IsTypeOf(typeof(T));

        /// <summary>
        /// Verifica se um objeto é de um determinado tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static bool IsTypeOf<T>(this T Obj, Type Type) => Obj.GetTypeOf() == Type.GetTypeOf();

        public static bool IsValueType(this Type T) => T.IsIn(PredefinedArrays.ValueTypes);

        public static bool IsValueType<T>(this T Obj) => Obj.GetNullableTypeOf().IsValueType();

        /// <summary>
        /// Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="Collections"></param>
        /// <returns></returns>
        public static NameValueCollection Merge(this IEnumerable<NameValueCollection> Collections)
        {
            Collections = Collections ?? new List<NameValueCollection>();
            var all = new NameValueCollection();
            foreach (var i in Collections)
            {
                all.Add(i);
            }

            return all;
        }

        /// <summary>
        /// Mescla varios <see cref="NameValueCollection"/> em um unico <see cref="NameValueCollection"/>
        /// </summary>
        /// <param name="OtherCollections"></param>
        /// <returns></returns>
        public static NameValueCollection Merge(this NameValueCollection FirstCollection, params NameValueCollection[] OtherCollections)
        {
            OtherCollections = OtherCollections ?? Array.Empty<NameValueCollection>();
            OtherCollections = new[] { FirstCollection }.Union(OtherCollections).ToArray();
            return OtherCollections.Merge();
        }

        /// <summary>
        /// Move os itens de uma lista para outra
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FromList"></param>
        /// <param name="ToList"></param>
        /// <param name="Indexes"></param>
        /// <returns></returns>
        public static List<T> MoveItems<T>(this List<T> FromList, ref List<T> ToList, params int[] Indexes)
        {
            ToList = ToList ?? new List<T>();
            if (FromList != null)
            {
                Indexes = Indexes?.Where(x => x.IsBetween(0, FromList.Count)).ToArray() ?? Array.Empty<int>();
                foreach (var index in Indexes)
                {
                    var item = FromList.Detach(index);
                    try
                    {
                        ToList.Insert(index, item);
                    }
                    catch
                    {
                        ToList.Add(item);
                    }
                }
            }
            return ToList;
        }

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static T? NullCoalesce<T>(this T? First, params T?[] N) where T : struct => (T?)(T)First ?? N.NullCoalesce<T>();

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="List">Outros itens</param>
        /// <returns></returns>
        public static T? NullCoalesce<T>(this IEnumerable<T?> List) where T : struct => List?.FirstOrDefault(x => x.HasValue) ?? default;

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="First">Primeiro Item</param>
        /// <param name="N">Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this T First, params T[] N) where T : class => First ?? NullCoalesce((N ?? Array.Empty<T>()).AsEnumerable());

        /// <summary>
        /// Verifica se dois ou mais valores são nulos e retorna o primeiro elemento que possuir um valor
        /// </summary>
        /// <typeparam name="T">Tipo</typeparam>
        /// <param name="List">Outros itens</param>
        /// <returns></returns>
        public static T NullCoalesce<T>(this IEnumerable<T> List) => List == null ? default : List.FirstOrDefault(x => x != null);

        /// <summary>
        /// Substitui todas as propriedades nulas de uma classe pelos seus valores Default
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static T NullPropertiesAsDefault<T>(this T Obj, bool IncludeVirtual = false) where T : class
        {
            TryExecute(() => Obj = Obj ?? Activator.CreateInstance<T>());
            if (Obj != null)
                foreach (var item in Obj.GetProperties())
                {
                    if (item.CanRead && item.CanWrite && item.GetValue(Obj) is null)
                    {
                        switch (item.PropertyType)
                        {
                            case var @case when @case == typeof(string):
                                {
                                    item.SetValue(Obj, Ext.EmptyString);
                                    break;
                                }

                            default:
                                {
                                    bool IsVirtual = item.GetAccessors().All(x => x.IsVirtual) && IncludeVirtual;
                                    if (item.IsValueType() || IsVirtual)
                                    {
                                        var o = Activator.CreateInstance(item.PropertyType.GetNullableTypeOf());
                                        item.SetValue(Obj, o);
                                    }

                                    break;
                                }
                        }
                    }
                }

            return Obj;
        }

        /// <summary>
        /// Verifica se somente um unico elemento corresponde a condição
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool OnlyOneOf<T>(this IEnumerable<T> List, Func<T, bool> predicate) => List?.Count(predicate) == 1;

        public static string Peek(this Queue<char> queue, int take) => new String(queue.Take(take).ToArray());

        public static IEnumerable<string> PropertyNamesFor(this string Name)
        {
            var propnames = new List<string>();

            if (Name.IsNotBlank())
            {
                if (Name.StartsWith("_", StringComparison.InvariantCultureIgnoreCase))
                {
                    propnames.Add(Name.TrimStart('_'));
                }
                string propname1 = Name.Trim().Replace(" ", "_").Replace("-", "_").Replace("~", "_");
                string propname3 = Name.Trim().Replace(" ", Ext.EmptyString).Replace("-", Ext.EmptyString).Replace("~", Ext.EmptyString);
                string propname2 = propname1.RemoveAccents();
                string propname4 = propname3.RemoveAccents();
                propnames.AddRange(new[] { Name, propname1, propname2, propname3, propname4 });
                propnames.AddRange(propnames.Select(x => $"_{x}").ToArray());
                return propnames.Distinct();
            }
            return Array.Empty<string>();
        }

        public static HtmlTag QueryLinq(this HtmlTag tags, Func<HtmlTag, bool> query) => QueryLinq(tags.Children, query);

        public static HtmlTag QueryLinq(this IEnumerable<HtmlTag> tags, Func<HtmlTag, bool> query) => QueryLinqAll(tags, query).FirstOrDefault();

        public static IEnumerable<HtmlTag> QueryLinqAll(this HtmlTag tags, Func<HtmlTag, bool> query) => QueryLinqAll(tags?.Children ?? Array.Empty<HtmlTag>(), query);

        public static IEnumerable<HtmlTag> QueryLinqAll(this IEnumerable<HtmlTag> tags, Func<HtmlTag, bool> query) => tags.Traverse(ht => ht.Children).Where(query);

        /// <summary>
        /// Agrupa e conta os itens de uma lista a partir de uma propriedade
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="GroupSelector"></param>
        /// <returns></returns>
        public static Dictionary<T, long> ReduceToTop<T>(this Dictionary<T, long> obj, int First, T OtherLabel)
        {
            var grouped = obj.OrderByDescending(x => x.Value);
            return grouped.Take(First).Union(new[] { new KeyValuePair<T, long>(OtherLabel, grouped.Skip(First).Sum(s => s.Value)) }).ToDictionary();
        }

        public static Dictionary<TGroup, Dictionary<TCount, long>> ReduceToTop<TGroup, TCount>(this Dictionary<TGroup, Dictionary<TCount, long>> Grouped, int First, TCount OtherLabel)
        {
            if (Grouped != null)
            {
                foreach (var item in Grouped.ToArray())
                {
                    var gp = item.Value.OrderByDescending(x => x.Value).ToDictionary();
                    Grouped[item.Key] = gp.Take(First).Union(new[] { new KeyValuePair<TCount, long>(OtherLabel, gp.Skip(First).Sum(s => s.Value)) }).ToDictionary();
                }

                Grouped.Values.MergeKeys();
            }
            return Grouped;
        }

        /// <summary>
        /// Remove de um dicionario as respectivas Keys se as mesmas existirem
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="Keys"></param>
        public static IDictionary<TKey, TValue> RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, params TKey[] Keys)
        {
            if (dic != null)
                foreach (var k in (Keys ?? Array.Empty<TKey>()).Where(x => dic.ContainsKey(x)))
                {
                    dic.Remove(k);
                }

            return dic;
        }

        /// <summary>
        /// Remove de um dicionario os valores encontrados pelo predicate
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="Tvalue"></typeparam>
        /// <param name="dic"></param>
        /// <param name="predicate"></param>
        public static IDictionary<TKey, TValue> RemoveIfExist<TKey, TValue>(this IDictionary<TKey, TValue> dic, Func<KeyValuePair<TKey, TValue>, bool> predicate) => dic.RemoveIfExist(dic.Where(predicate).Select(x => x.Key).ToArray());

        /// <summary>
        /// Remove <paramref name="Count"/> elementos de uma <paramref name="List"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public static List<T> RemoveLast<T>(this List<T> List, int Count = 1)
        {
            if (List != null)
                for (int index = 1, loopTo = Count; index <= loopTo; index++)
                {
                    if (List.Any())
                    {
                        List.RemoveAt(List.Count - 1);
                    }
                }

            return List;
        }

        /// <summary>
        /// Adciona ou substitui um valor a este <see cref="Dictionary(Of TKey, TValue)"/> e retorna
        /// a mesma instancia deste <see cref="Dictionary(Of TKey, TValue)"/>
        /// </summary>
        /// <typeparam name="TKey">Tipo da Key</typeparam>
        /// <typeparam name="TValue">Tipo do valor</typeparam>
        /// <param name="Key">Valor da key</param>
        /// <param name="Value">Valor do Value</param>
        /// <returns>o mesmo objeto do tipo <see cref="Dictionary"/> que chamou este método</returns>
        public static IDictionary<TKey, TValue> Set<TKey, TValue, TK, TV>(this IDictionary<TKey, TValue> Dic, TK Key, TV Value)
        {
            if (Key != null && Dic != null)
            {
                Dic[Key.ChangeType<TKey>()] = Value.ChangeType<TValue>();
            }

            return Dic;
        }

        public static T SetOrRemove<T, TK, TV>(this T Dictionary, KeyValuePair<TK, TV> Pair) where T : IDictionary<TK, TV>
        {
            Dictionary?.SetOrRemove(Pair.Key, Pair.Value);
            return Dictionary;
        }

        public static IDictionary<TKey, string> SetOrRemove<TKey, TK>(this IDictionary<TKey, string> Dic, TK Key, string Value, bool NullIfBlank) => Dic.SetOrRemove(Key, NullIfBlank.AsIf(Value.NullIf(x => x.IsBlank()), Value));

        public static IDictionary<TKey, TValue> SetOrRemove<TKey, TValue, TK, TV>(this IDictionary<TKey, TValue> Dic, TK Key, TV Value)
        {
            if (Dic != null && Key != null)
            {
                if (Value != null)
                {
                    Dic[Key.ChangeType<TKey>()] = Value.ChangeType<TValue>();
                }
                else
                {
                    Dic.RemoveIfExist(Key.ChangeType<TKey>());
                }
            }

            return Dic;
        }

        /// <summary>
        /// Seta o valor de uma propriedade de um objeto
        /// </summary>
        /// <param name="MyObject">Objeto</param>
        /// <param name="PropertyName">Nome da properiedade</param>
        /// <param name="Value">Valor da propriedade definida por <paramref name="PropertyName"/></param>
        /// <typeparam name="T">
        /// Tipo do <paramref name="Value"/> da propriedade definida por <paramref name="PropertyName"/>
        /// </typeparam>
        public static T SetPropertyValue<T>(this T MyObject, string PropertyName, object Value) where T : class
        {
            if (PropertyName.IsNotBlank() && MyObject != null)
            {
                var props = MyObject.GetProperties();

                var prop = props.FirstOrDefault(p => p != null && p.CanWrite && p.Name.IsAny(PropertyNamesFor(PropertyName).ToArray()));

                if (prop != null)
                    if (Value is DBNull)
                    {
                        prop.SetValue(MyObject, null);
                    }
                    else
                    {
                        prop.SetValue(MyObject, Ext.ChangeType(Value, prop.PropertyType));
                    }
            }

            return MyObject;
        }

        public static T SetPropertyValue<T, TProp>(this T obj, Expression<Func<T, TProp>> Selector, TProp Value) where T : class
        {
            obj?.SetPropertyValue(obj.GetPropertyInfo(Selector).Name, Value);
            return obj;
        }

        public static Task SetTimeout(int milliseconds, Action action)
        {
            return Task.Delay(milliseconds).ContinueWith(async (t) =>
                {
                    Ext.TryExecute(action);
                    t.Dispose();
                });
        }

        public static Dictionary<TGroup, Dictionary<TCount, long>> SkipZero<TGroup, TCount>(this Dictionary<TGroup, Dictionary<TCount, long>> Grouped)
        {
            if (Grouped != null)
            {
                foreach (var dic in Grouped.ToArray())
                {
                    Grouped[dic.Key] = dic.Value.Where(x => x.Value > 0).ToDictionary();
                }

                Grouped = Grouped.Where(x => x.Value.Any()).ToDictionary();
            }

            return Grouped;
        }

        public static Dictionary<TCount, long> SkipZero<TCount>(this Dictionary<TCount, long> Grouped)
        {
            Grouped = Grouped?.Where(x => x.Value > 0).ToDictionary();
            return Grouped;
        }

        /// <summary>
        /// Troca o valor de <paramref name="FirstValue"/> pelo valor de <paramref
        /// name="SecondValue"/> e o valor de <paramref name="SecondValue"/> pelo valor de <paramref name="FirstValue"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="FirstValue"></param>
        /// <param name="SecondValue"></param>
        public static (T, T) Swap<T>(ref T FirstValue, ref T SecondValue)
        {
            (SecondValue, FirstValue) = (FirstValue, SecondValue);
            return (FirstValue, SecondValue);
        }

        /// <summary>
        /// Traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> TakeTop<TKey, TValue>(this Dictionary<TKey, TValue> Dic, int Top, TKey GroupOthersLabel)
        {
            if (Dic == null)
            {
                return null;
            }

            if (Top < 1)
            {
                return Dic.ToDictionary();
            }

            var novodic = Dic.Take(Top).ToDictionary();
            if (GroupOthersLabel != null)
            {
                novodic[GroupOthersLabel] = Dic.Values.Skip(Top).Select(x => x.ChangeType<decimal>()).Sum().ChangeType<TValue>();
            }

            return novodic;
        }

        /// <summary>
        /// Traz os top N valores de um dicionario e agrupa os outros
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Dic"></param>
        /// <param name="Top"></param>
        /// <param name="GroupOthersLabel"></param>
        /// <returns></returns>
        public static Dictionary<TKey, IEnumerable<T>> TakeTop<TKey, T>(this Dictionary<TKey, IEnumerable<T>> Dic, int Top, Expression<Func<T, dynamic>> ValueSelector) where T : class
        {
            Dictionary<TKey, IEnumerable<T>> novodic = Dic.ToDictionary();

            if (ValueSelector != null)
            {
                novodic = Dic.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(ValueSelector.Compile()).AsEnumerable());
            }

            if (Top > 0)
            {
                novodic = Dic.ToDictionary(x => x.Key, x => x.Value.TakeTop(Top, ValueSelector));
            }

            return novodic;
        }

        public static IEnumerable<T> TakeTop<T>(this IEnumerable<T> List, int Top, params Expression<Func<T, dynamic>>[] ValueSelector) where T : class => TakeTop<T, object>(List, Top, null, null, ValueSelector?.ToArray());

        public static IEnumerable<T> TakeTop<T, TLabel>(this IEnumerable<T> List, int Top, Expression<Func<T, TLabel>> LabelSelector, TLabel GroupOthersLabel, params Expression<Func<T, dynamic>>[] ValueSelector) where T : class
        {
            ValueSelector = ValueSelector ?? Array.Empty<Expression<Func<T, dynamic>>>();

            if (ValueSelector.WhereNotNull().IsNullOrEmpty())
            {
                throw new ArgumentException("You need at least one value selector", nameof(ValueSelector));
            }

            var newlist = List.OrderByManyDescending(ValueSelector).Take(Top).ToList();

            if (LabelSelector != null && GroupOthersLabel != null)
            {
                var others = Activator.CreateInstance<T>();
                LabelSelector.GetPropertyInfo().SetValue(others, GroupOthersLabel);
                foreach (var v in ValueSelector)
                {
                    var values = List.Skip(Top).Select(x => (v.Compile().Invoke(x) as object).ChangeType<decimal>()).Sum();
                    v.GetPropertyInfo().SetValue(others, values);
                }
                newlist.Add(others);
            }
            return newlist.AsEnumerable();
        }

        public static T[,] To2D<T>(this T[][] source)
        {
            int FirstDim = source.Length;
            int SecondDim = source.GroupBy(row => row.Length).Max().Key;

            var result = new T[FirstDim, SecondDim];
            for (int i = 0; i < FirstDim; ++i)
                for (int j = 0; j < SecondDim; ++j)
                    result[i, j] = source[i].IfNoIndex(j);

            return result;
        }

        public static Attachment ToAttachment(this FileInfo file) => file != null && file.Exists ? new Attachment(file.FullName) : null;

        public static Attachment ToAttachment(this Stream stream, string name) => stream != null && stream.Length > 0 ? new Attachment(stream, name.IfBlank("untitledFile.bin")) : null;

        public static Attachment ToAttachment(this byte[] bytes, string name)
        {
            if (bytes != null && bytes.Any())
                using (var s = new MemoryStream(bytes))
                    return s.ToAttachment(name);
            return null;
        }

        /// <summary>
        /// Concatena todas as <see cref="Exception.InnerException"/> em uma única string
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ToFullExceptionString(this Exception ex, string Separator = " => ") => ex.Traverse(x => x.InnerException).SelectJoinString(x => x.Message, Separator);

        /// <summary>
        /// Alterna uma variavel ente 2 valores diferentes
        /// </summary>
        /// <param name="Current">Objeto contendo o primeiro ou segundo valor</param>
        /// <param name="TrueValue">Primeiro valor</param>
        /// <param name="FalseValue">Segundo Valor</param>
        public static T Toggle<T>(this T Current, T TrueValue, T FalseValue = default) => Current.Equals(TrueValue) ? FalseValue : TrueValue;

        public static T[][] ToJaggedArray<T>(this T[,] inputArray)
        {
            if (inputArray == null || inputArray.Length == 0)
            {
                return Array.Empty<T[]>();
            }

            // Get the number of rows and columns in the input array
            int rows = inputArray.GetLength(0);
            int cols = inputArray.GetLength(1);

            // Create the jagged array with the same number of rows as the input array
            T[][] jaggedArray = new T[rows][];

            // Copy the elements from the input array to the jagged array
            for (int i = 0; i < rows; i++)
            {
                // Create a new sub-array for each row
                jaggedArray[i] = new T[cols];

                // Copy the elements from the input array to the jagged array
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = inputArray[i, j];
                }
            }

            return jaggedArray;
        }

        /// <summary>
        /// Retorna um dicionário em QueryString
        /// </summary>
        /// <param name="Dic"></param>
        /// <returns></returns>
        public static string ToQueryString(this Dictionary<string, string> Dic) => Dic?.Where(x => x.Key.IsNotBlank()).SelectJoinString(x => new[] { x.Key, (x.Value ?? Ext.EmptyString).UrlEncode() }.SelectJoinString("="), "&") ?? Ext.EmptyString;

        /// <summary>
        /// Retorna um <see cref="NameValueCollection"/> em QueryString
        /// </summary>
        /// <param name="NVC"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection NVC) => NVC?.AllKeys.SelectManyJoinString(n => NVC.GetValues(n).Select(v => n + "=" + v).Where(x => x.IsNotBlank() && x != "="), "&");

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de arrays
        /// </summary>
        /// <typeparam name="TGroupKey"></typeparam>
        /// <typeparam name="TSubGroupKey"></typeparam>
        /// <typeparam name="TSubGroupValue"></typeparam>
        /// <param name="Groups"></param>
        /// <returns></returns>
        public static IEnumerable<object> ToTableArray<TGroupKey, TSubGroupKey, TSubGroupValue, THeaderProperty>(this Dictionary<TGroupKey, Dictionary<TSubGroupKey, TSubGroupValue>> Groups, Func<TSubGroupKey, THeaderProperty> HeaderProp)
        {
            var lista = new List<object>();
            var header = new List<object>
            {
                HeaderProp?.Method.GetParameters().First().Name
            };

            Groups?.Values.MergeKeys();
            foreach (var h in Groups.SelectMany(x => x.Value.Keys.ToArray()).Distinct().OrderBy(x => x))
            {
                header.Add(HeaderProp(h));
            }

            lista.Add(header);
            lista.AddRange(Groups.Select(x =>
            {
                var l = new List<object>
                {
                    x.Key // GroupKey
                };
                foreach (var item in x.Value.OrderBy(k => k.Key).Select(v => v.Value))
                {
                    l.Add(item); // SubGroupValue
                }

                return l;
            }));
            return lista;
        }

        /// <summary>
        /// Projeta um unico array os valores sub-agrupados e unifica todos num unico array de
        /// arrays formando uma tabela
        /// </summary>
        public static IEnumerable<object[]> ToTableArray<TGroupKey, TGroupValue>(this Dictionary<TGroupKey, TGroupValue> Groups) => Groups.Select(x => new List<object> { x.Key, x.Value }.ToArray());

        /// <summary>
        /// Run a <see cref="Action"/> inside a Try-catch block and return a <see cref="Exception"/>
        /// if fail
        /// </summary>
        /// <param name="action"></param>
        /// <returns>
        /// A null <see cref="Exception"/> if <paramref name="action"/> runs successfully, otherwise
        /// the captured <see cref="Exception"/>
        /// </returns>
        public static Exception TryExecute(Action action)
        {
            try
            {
                action?.Invoke();
                return null;
            }
            catch (Exception exx)
            {
                return exx;
            }
        }

        ///<inheritdoc cref="With{T}(T, Action{T}, out Exception)"/>
        public static T With<T>(this T Obj, Action<T> Callback) => With(Obj, Callback, out _);

        /// <summary>
        /// Run a <see cref="Action{T}"/> inside a Try-Catch block and return the same <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="Obj">Object</param>
        /// <param name="Callback">The action to execute</param>
        /// <param name="ex">
        /// An out param to capture a <see cref="Exception"/> if <paramref name="Callback"/> fails
        /// </param>
        /// <returns>The same <paramref name="Obj"/></returns>
        public static T With<T>(this T Obj, Action<T> Callback, out Exception ex)
        {
            ex = TryExecute(() => Callback?.Invoke(Obj));
            return Obj;
        }

        #endregion Public Methods
    }
}