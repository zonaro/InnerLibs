using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace InnerLibs
{
    /// <summary>
    /// Verifica determinados valores como Arquivos, Numeros e URLs
    /// </summary>
    /// <remarks></remarks>
    public static class Verify
    {
        #region Private Fields

        private const int ERROR_LOCK_VIOLATION = 33;

        private const int ERROR_SHARING_VIOLATION = 32;

        private static readonly Expression<Func<string, bool>>[] passwordValidations = new Expression<Func<string, bool>>[]
            {
                x => x.ToLowerInvariant().ToArray().Distinct().Count() >= 4,
                x => x.ToLowerInvariant().ToArray().Distinct().Count() >= 6,
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
        /// Verifica se uma variavel está vazia, em branco ou nula e retorna um outro valor caso TRUE
        /// </summary>
        /// <typeparam name="T">Tipo da Variavel</typeparam>
        /// <param name="Value">Valor</param>
        /// <param name="ValueIfBlank">Valor se estiver em branco</param>
        /// <returns></returns>
        public static T IfBlank<T>(this object Value, T ValueIfBlank = default)
        {
            if (Value == null)
            {
                return ValueIfBlank;
            }
            else
            {
                bool blank_flag = false;
                try
                {
                    if (typeof(T).IsNumericType())
                    {
                        blank_flag = Value.ChangeType<decimal>() == 0;
                    }
                    else if (Value is string s)
                    {
                        blank_flag = s.IsBlank();
                    }
                    else if (Value is char c)
                    {
                        blank_flag = $"{c}".IsBlank();
                    }
                    else if (Value is DateTime time)
                    {
                        blank_flag = time.Equals(DateTime.MinValue);
                    }
                    else if (Value is TimeSpan span)
                    {
                        blank_flag = span.Equals(TimeSpan.MinValue);
                    }

                }
                catch
                {
                    blank_flag = false;
                }

                return blank_flag ? ValueIfBlank : Value.ChangeType<T>();
            }
        }

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
        /// Verifica se uma String está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>TRUE se estivar vazia ou em branco, caso contrario FALSE</returns>
        public static bool IsBlank(this string Text) => string.IsNullOrWhiteSpace(Text.RemoveAny(PredefinedArrays.BreakLineChars.ToArray()));

        /// <summary>
        /// Verifica se uma String está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>TRUE se estivar vazia ou em branco, caso contrario FALSE</returns>
        public static bool IsBlank(this FormattableString Text) => Text == null || Text.ToString().IsBlank();

        public static bool IsBool<T>(this T Obj) => Misc.GetNullableTypeOf(Obj) == typeof(bool) || Obj?.ToString().ToLowerInvariant().IsIn<string>("true", "false") == true;

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

        public static bool IsDate<T>(this T Obj) => Misc.GetNullableTypeOf(Obj) == typeof(DateTime) || $"{Obj}".IsDate();

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

                if (new string[] { Convert.ToString(Path.DirectorySeparatorChar), Convert.ToString(Path.AltDirectorySeparatorChar) }.Any(x => Text.EndsWith(x)))
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
        public static bool IsEmail(this string Text) => Text.IsNotBlank() && new Regex(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|InnerLibs.Text.Empty(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*InnerLibs.Text.Empty)@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])").IsMatch(Text);

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
                return !Text.EndsWith(Convert.ToString(Path.DirectorySeparatorChar)) && Path.GetExtension(Text).IsNotBlank();
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
        public static bool IsNotBlank(this string Text) => !IsBlank(Text);

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
        /// Valida se a string é um telefone
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public static bool IsTelephone(this string Text) => new Regex(@"\(?\+[0-9]{1,3}\)? ?-?[0-9]{1,3} ?-?[0-9]{3,5} ?-?[0-9]{4}( ?-?[0-9]{3})? ?(\w{1,10}\s?\d{1,6})?", (RegexOptions)((int)RegexOptions.Singleline + (int)RegexOptions.IgnoreCase)).IsMatch(Text.RemoveAny("(", ")"));

        /// <summary>
        /// Verifica se um determinado texto é uma URL válida
        /// </summary>
        /// <param name="Text">Texto a ser verificado</param>
        /// <returns>TRUE se for uma URL, FALSE se não for uma URL válida</returns>
        public static bool IsURL(this string Text) => Text.IsNotBlank() && Uri.TryCreate(Text.Trim(), UriKind.Absolute, out _) && !Text.Trim().Contains(" ");

        /// <summary>
        /// Verifica se uma string é um cep válido
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static bool IsValidCEP(this string CEP) => new Regex(@"^\d{5}-\d{3}$").IsMatch(CEP) || (CEP.RemoveAny("-").IsNumber() && CEP.RemoveAny("-").Length == 8);

        /// <summary>
        /// Verifica se a string é um CNH válido
        /// </summary>
        /// <param name="Text">CNH</param>
        /// <returns></returns>
        public static bool IsValidCNH(this string CNH)
        {

            // char firstChar = cnh[0];
            if (CNH.IsNotBlank() && CNH.Length == 11 && CNH != new string('1', 11))
            {
                int dsc = 0;
                int v = 0;
                int i = 0;
                int j = 9;
                while (i < 9)
                {
                    v += Convert.ToInt32(CNH[i].ToString()) * j;
                    i += 1;
                    j -= 1;
                }

                int vl1 = v % 11;
                if (vl1 >= 10)
                {
                    vl1 = 0;
                    dsc = 2;
                }

                v = 0;
                i = 0;
                j = 1;
                while (i < 9)
                {
                    v += Convert.ToInt32(CNH[i].ToString()) * j;
                    i += 1;
                    j += 1;
                }

                int x = v % 11;
                int vl2 = x >= 10 ? 0 : x - dsc;
                return $"{vl1}{vl2}" == (CNH.Substring(CNH.Length - 2, 2));
            }

            return false;
        }

        /// <summary>
        /// Verifica se a string é um CNPJ válido
        /// </summary>
        /// <param name="Text">CPF</param>
        /// <returns></returns>
        public static bool IsValidCNPJ(this string Text)
        {
            try
            {
                if (Text.IsNotBlank())
                {
                    var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                    int soma;
                    int resto;
                    string digito;
                    string tempCnpj;
                    Text = Text.Trim();
                    Text = Text.Replace(".", InnerLibs.Text.Empty).Replace("-", InnerLibs.Text.Empty).Replace("/", InnerLibs.Text.Empty);
                    if (Text.Length != 14)
                    {
                        return false;
                    }

                    tempCnpj = Text.Substring(0, 12);
                    soma = 0;
                    for (int i = 0; i <= 12 - 1; i++)
                    {
                        soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
                    }

                    resto = soma % 11;
                    if (resto < 2)
                    {
                        resto = 0;
                    }
                    else
                    {
                        resto = 11 - resto;
                    }

                    digito = resto.ToString();
                    tempCnpj += digito;
                    soma = 0;
                    for (int i = 0; i <= 13 - 1; i++)
                    {
                        soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
                    }

                    resto = soma % 11;
                    if (resto < 2)
                    {
                        resto = 0;
                    }
                    else
                    {
                        resto = 11 - resto;
                    }

                    digito += resto.ToString();
                    return Text.EndsWith(digito);
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Verifica se a string é um CPF válido
        /// </summary>
        /// <param name="Text">CPF</param>
        /// <returns></returns>
        public static bool IsValidCPF(this string Text)
        {
            try
            {
                if (Text.IsNotBlank())
                {
                    Text = Text.RemoveAny(".", "-");
                    string digito = InnerLibs.Text.Empty;
                    int k;
                    int j;
                    int soma;
                    for (k = 0; k <= 1; k++)
                    {
                        soma = 0;
                        var loopTo = 9 + (k - 1);
                        for (j = 0; j <= loopTo; j++)
                        {
                            soma += int.Parse($"{Text[j]}", CultureInfo.InvariantCulture) * (10 + k - j);
                        }

                        digito += $"{(soma % 11 == 0 || soma % 11 == 1 ? 0 : 11 - (soma % 11))}";
                    }

                    return digito[0] == Text[9] & digito[1] == Text[10];
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Verifica se a string é um CPF ou CNPJ válido
        /// </summary>
        /// <param name="Text">CPF ou CNPJ</param>
        /// <returns></returns>
        public static bool IsValidCPFOrCNPJ(this string Text) => Text.IsValidCPF() || Text.IsValidCNPJ();

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
                    return (ObjHost.HostName ?? InnerLibs.Text.Empty) == (HostName ?? InnerLibs.Text.Empty);
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
            if (Code.IsNotNumber() || Code.Length < 3)
            {
                return false;
            }

            var bar = Code.RemoveLastChars();
            var ver = Code.GetLastChars();
            return Generate.BarcodeCheckSum(bar) == ver;
        }

        public static bool IsValidEAN(this int Code) => Code.ToString(CultureInfo.InvariantCulture).PadLeft(12, '0').ToString().IsValidEAN();

        /// <summary>
        /// Verifica se uma string é um PIS válido
        /// </summary>
        /// <param name="CEP"></param>
        /// <returns></returns>
        public static bool IsValidPIS(this string PIS)
        {
            if (PIS.IsBlank())
            {
                return false;
            }

            PIS = Regex.Replace(PIS, "[^0-9]", InnerLibs.Text.Empty).ToString();

            if (PIS.Length != 11)
            {
                return false;
            }

            var count = PIS[0];
            if (PIS.Count(w => w == count) == PIS.Length)
            {
                return false;
            }

            var multiplicador = new int[10] { 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;

            soma = 0;

            for (var i = 0; i < 10; i++)
            {
                soma += int.Parse($"{PIS[i]}", CultureInfo.InvariantCulture) * multiplicador[i];
            }

            resto = soma % 11;

            resto = resto < 2 ? 0 : 11 - resto;

            if (PIS.EndsWith(resto.ToString()))
            {
                return true;
            }

            return false;
        }

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
        /// Lança uma <see cref="Exception"/> do tipo <typeparamref name="E"/> se um teste falhar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Test"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static T ValidateOr<T, E>(this T Value, Expression<Func<T, bool>> Test, E Exception) where E : Exception
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

        public static bool ValidatePassword(this string Password, PasswordLevel PasswordLevel = PasswordLevel.Strong) => Password.CheckPassword().ToInt() >= PasswordLevel.ToInt();

        /// <summary>
        /// Bloqueia a Thread atual enquanto um arquivo estiver em uso
        /// </summary>
        /// <param name="File">Arquivo</param>
        /// <param name="Seconds">intervalo, em segundo entre as tentativas de acesso</param>
        /// <param name="MaxFailCount">
        /// Numero maximo de tentativas falhas,quando negativo, verifica infinitamente
        /// </param>
        /// <param name="OnAttemptFail">ação a ser executado em caso de falha</param>
        /// <returns>TRUE se o arquivo puder ser utilizado</returns>
        public static bool WaifForFile(this FileInfo File, int Seconds = 1, int? MaxFailCount = null, Action<int> OnAttemptFail = null)
        {
            while (IsInUse(File))
            {
                Thread.Sleep(Seconds * 1000);

                if (!File.Exists)
                {
                    return false;
                }

                if (MaxFailCount.HasValue)
                {
                    MaxFailCount = MaxFailCount.Value - 1;
                }

                if (MaxFailCount.HasValue && MaxFailCount.Value < 0)
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
        /// <param name="OnFail">Função a ser executada após um numero determinado de tentativas</param>
        /// <param name="OnAttemptFail"></param>
        /// <param name="Seconds"></param>
        /// <param name="MaxFailCount"></param>
        public static bool WithFile(this FileInfo File, Action<FileInfo> OnSuccess, Action<FileInfo> OnFail, Action<int> OnAttemptFail = null, int Seconds = 1, int? MaxFailCount = null)
        {
            if (File != null)
            {
                if (WaifForFile(File, Seconds, MaxFailCount, OnAttemptFail))
                {
                    OnSuccess?.Invoke(File);
                    return true;
                }
                else
                {
                    OnFail?.Invoke(File);
                }
            }
            return false;
        }

        #endregion Public Methods
    }

    public enum PasswordLevel
    {
        VeryWeak = 2,
        Weak = 3,
        Medium = 4,
        Strong = 5,
        VeryStrong = 6
    }
}