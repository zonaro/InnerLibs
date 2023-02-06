using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace InnerLibs
{
    /// <summary>
    /// Modulo de Criptografia
    /// </summary>
    /// <remarks></remarks>
    public static partial class Util
    {
        /// <summary>
        /// Criptografa um Texto em MD5
        /// </summary>
        /// <param name="Text">Texto a ser Criptografado</param>
        /// <returns>Uma String MD5</returns>

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
    }
}