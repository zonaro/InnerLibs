using System;
using System.Globalization;
using System.IO;

namespace InnerLibs.JSONParser
{
    /// <summary>
    /// Class that when inherited by a POCO class implements methods to save the values in an encrypted Json file
    /// </summary>
    public abstract class JsonFile
    {
        private string file_path;
        private string encrypt_key;
        private bool exclude_null;
        private CultureInfo culture;

        /// <summary>
        /// Return a <see cref="FileInfo"/> of current JsonFile
        /// </summary>
        /// <returns></returns>
        public FileInfo GetFile() => new FileInfo(file_path.BlankCoalesce(DefaultFileName));

        /// <summary>
        /// The default path used to save JsonFiles
        /// </summary>
        public static string DefaultFileName => $"{Environment.CurrentDirectory}\\config.json";

        /// <summary>
        /// Load values of a JsonFile into a <typeparamref name="T"/> object
        /// </summary>
        /// <typeparam name="T">Object Type</typeparam>
        /// <param name="FilePath">File Path</param>
        /// <param name="EncryptKey">Encrypt Key. Leave Null or blank to not encrypt</param>
        /// <param name="ExcludeNull">When true, exclude properties with null values from serialization</param>
        /// <returns></returns>
        public static T Load<T>(FileInfo FilePath, string EncryptKey, bool ExcludeNull = false) where T : JsonFile => Load<T>(FilePath?.FullName, EncryptKey, ExcludeNull);

        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>(FileInfo FilePath) where T : JsonFile => Load<T>(FilePath, null);
        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>(string FilePath) where T : JsonFile => Load<T>(FilePath, null);
        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>() where T : JsonFile => Load<T>(DefaultFileName, null);
        /// <inheritdoc cref="Load{T}"/>     
        public static T Load<T>(string FilePath, string EncryptKey, bool ExcludeNull = false, CultureInfo culture = null) where T : JsonFile
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            FilePath = FilePath.IfBlank(DefaultFileName).FixPath();
            T c = Activator.CreateInstance<T>();

            if (File.Exists(c.file_path))
            {
                string s = File.ReadAllText(c.file_path);
                if (EncryptKey.IsNotBlank())
                {
                    s = s.Decrypt(EncryptKey);
                }

                c = s.FromJson<T>(culture);
            }

            c.SetFilePath(FilePath);
            c.SetEncryptKey(EncryptKey);
            c.ExcludeNullValues(ExcludeNull);
            c.SetCulture(culture);
            c.Save();
            return c;
        }



        /// <summary>
        /// Save the current values into a JsonFile
        /// </summary>
        /// <returns></returns>     
        public FileInfo Save()
        {
            this.file_path = this.file_path.BlankCoalesce(DefaultFileName).FixPath();
            var s = this.ToJson(!exclude_null, culture);
            if (this.encrypt_key.IsNotBlank())
            {
                s = s.Encrypt(this.encrypt_key);
            }

            s.WriteToFile(this.file_path);
            return new FileInfo(this.file_path);
        }

        /// <summary>
        /// Set the <see cref="CultureInfo"/> used during serialization
        /// </summary>
        /// <param name="culture"></param>
        public void SetCulture(CultureInfo culture) => this.culture = culture;

        /// <summary>
        /// Set the Encrypt key using to encrypt the content of JsonFile after serialization
        /// </summary>
        /// <param name="EncryptKey"></param>
        public void SetEncryptKey(string EncryptKey) => this.encrypt_key = EncryptKey.NullIf(x => x.IsBlank());

        /// <summary>
        /// Change the filepath used to <see cref="Save"/>
        /// </summary>
        /// <param name="FilePath"></param>
        public void SetFilePath(string FilePath) => this.file_path = FilePath.BlankCoalesce(this.file_path, DefaultFileName);

        /// <summary>
        /// When true, exclude properties with null values from serialization
        /// </summary>
        /// <param name="Exclude"></param>
        public void ExcludeNullValues(bool Exclude) => this.exclude_null = Exclude;


    }
}
