using System;
using System.Collections;
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
    public static partial class Util
    {
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
        public static T IfBlank<T>(this object Value, T ValueIfBlank = default) => Value.IsBlank() ? ValueIfBlank : Util.ChangeType<T>(Value);

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
        /// Verifica se uma variavel está em branco. (nula, vazia ou somente brancos para string, null ou 0 para tipos primitivos, null ou ToString() em branco para tipos de referencia. Null ou vazio para arrays)
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
                Util.WriteDebug(ex);
            }
            return true;
        }

        /// <summary>
        /// Verifica se uma String está em branco
        /// </summary>
        /// <param name="Text">Uma string</param>
        /// <returns>TRUE se estivar vazia ou em branco, caso contrario FALSE</returns>
        public static bool IsBlank(this FormattableString Text) => Text == null || $"{Text}".IsBlank();

        public static bool IsBool<T>(this T Obj) => Util.GetNullableTypeOf(Obj) == typeof(bool) || $"{Obj}".ToLowerInvariant().IsIn("true", "false");

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

        public static bool IsDate<T>(this T Obj) => Util.GetNullableTypeOf(Obj) == typeof(DateTime) || $"{Obj}".IsDate();

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
                    return (ObjHost.HostName ?? InnerLibs.Util.EmptyString) == (HostName ?? InnerLibs.Util.EmptyString);
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
            return Util.BarcodeCheckSum(bar) == ver;
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
        /// <param name="OnFail">Função a ser executada após um numero determinado de tentativas falharem</param>
        /// <param name="OnAttemptFail">Função a ser executada a cada tentativa falhada</param>
        /// <param name="Seconds">Tempo de espera em segundos entre uma tentativa e outra</param>
        /// <param name="MaxFailCount">Numero máximo de tentativas, infinito se null</param>
        /// <returns>
        /// TRUE após <paramref name="OnSuccess"/> ser executada com sucesso. FALSE em qualquer outra situação
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
    }

    public enum PasswordLevel
    {
        None,
        VeryWeak = 2,
        Weak = 3,
        Medium = 4,
        Strong = 5,
        VeryStrong = 6
    }
}