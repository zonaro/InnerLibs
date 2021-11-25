using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace InnerLibs
{

    /// <summary>
    /// Modulo de Criptografia
    /// </summary>
    /// <remarks></remarks>
    public static class Encryption
    {

        /// <summary>
        /// Criptografa um Texto em MD5
        /// </summary>
        /// <param name="Text">Texto a ser Criptografado</param>
        /// <returns>Uma String MD5</returns>

        public static string ToMD5String(this string Text)
        {
            if (Text.IsNotBlank())
            {
                var md5 = MD5.Create();
                var inputBytes = Encoding.ASCII.GetBytes(Text);
                var hash = md5.ComputeHash(inputBytes);
                var sb = new StringBuilder();
                for (int i = 0, loopTo = hash.Length - 1; i <= loopTo; i++)
                    sb.Append(hash[i].ToString("X2"));
                return sb.ToString();
            }

            return Text;
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
            return BitConverter.ToString(bytes);
        }


        /// <summary>
        /// Descriptografa uma string encriptada em RSA
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string DecryptRSA(this string Text, string Key)
        {
            var cspp = new CspParameters() { KeyContainerName = Key };
            var rsa = new RSACryptoServiceProvider(cspp) { PersistKeyInCsp = true };
            var decryptArray = Text.Split(new[] { "-" }, StringSplitOptions.None);
            var decryptByteArray = Array.ConvertAll(decryptArray, s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber)));
            var bytes = rsa.Decrypt(decryptByteArray, true);
            return new UTF8Encoding(false).GetString(bytes);
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
                byte[] Results;
                var UTF8 = new UTF8Encoding();
                var HashProvider = new MD5CryptoServiceProvider();
                var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("12345")));
                var TDESAlgorithm = new TripleDESCryptoServiceProvider();
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;
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

                return Convert.ToBase64String(Results);
            }

            return Text;
        }

        /// <summary>
        /// Descriptografa uma string
        /// </summary>
        /// <param name="Text">Texto Criptografado</param>
        /// <returns></returns>
        public static string Decrypt(this string Text, string Key = null)
        {
            if (Text.IsNotBlank())
            {
                byte[] Results;
                var UTF8 = new UTF8Encoding();
                var HashProvider = new MD5CryptoServiceProvider();
                var TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Key.IfBlank("12345")));
                var TDESAlgorithm = new TripleDESCryptoServiceProvider();
                TDESAlgorithm.Key = TDESKey;
                TDESAlgorithm.Mode = CipherMode.ECB;
                TDESAlgorithm.Padding = PaddingMode.PKCS7;
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

                return UTF8.GetString(Results);
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
                var aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = new UTF8Encoding(false).GetBytes(IV);
                aes.Key = new UTF8Encoding(false).GetBytes(Key);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
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
        /// Descriptografa uma string
        /// </summary>
        /// <param name="Text">Texto Criptografado</param>
        /// <returns></returns>
        public static string Decrypt(this string text, string Key, string IV)
        {
            if (text.IsNotBlank())
            {
                var aes = new AesCryptoServiceProvider();
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.IV = new UTF8Encoding(false).GetBytes(IV);
                aes.Key = new UTF8Encoding(false).GetBytes(Key);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
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
    }
}