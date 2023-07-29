using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;

namespace Extensions.ComplexText
{
    /// <summary>
    /// Class that when inherited by a POCO class implements methods to save the values in an
    /// encrypted Json file
    /// </summary>
    /// <remarks>Your POCO class need a public, parameterless constructor</remarks>
    public abstract class JsonFile
    {
        #region Private Fields

        private string filePath;

        #endregion Private Fields



        #region Public Properties

        /// <summary>
        /// The default file name used to save JsonFiles
        /// </summary>
        /// <remarks>
        /// When <see cref="IsEncrypted"/>, assumes the <see cref="Assembly.Name"/> of current
        /// executing application, otherwise use the default ".json" extension
        /// </remarks>
        [IgnoreDataMember] public string DefaultFileName => $"{this.GetType().Name}.{(IsEncrypted ? this.GetType().Assembly.GetName().Name.ToSnakeCase().ToLower(CultureInfo.InvariantCulture) : "json")}";

        /// <summary>
        /// The default path used to save JsonFiles
        /// </summary>
        /// <remarks>Uses <see cref="Assembly.Location"/> directory and <see cref="DefaultFileName"/></remarks>
        [IgnoreDataMember] public string DefaultFilePath => Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), DefaultFileName);

        /// <summary>
        /// When not blank, encrypt the json file using this string as Key
        /// </summary>
        [IgnoreDataMember] public string EncryptKey { get; set; }

        /// <summary>
        /// <see cref="FileInfo"/> of current JsonFile
        /// </summary>
        /// <returns></returns>
        [IgnoreDataMember] public FileInfo File => new FileInfo(FilePath);

        /// <summary>
        /// The current File of this JsonFile
        /// </summary>
        [IgnoreDataMember]
        public string FilePath
        {
            get
            {
                filePath = filePath.NullIf(x => !x.IsFilePath() && !x.IsDirectoryPath()).BlankCoalesce(filePath, DefaultFilePath).FixPath();
                if (filePath.IsDirectoryPath())
                {
                    filePath = Path.Combine(filePath, DefaultFileName);
                }

                return filePath;
            }
            set => filePath = value;
        }

        /// <summary>
        /// Return if this JsonFile will be encrypted using <see cref="EncryptKey"/>
        /// </summary>
        [IgnoreDataMember] public bool IsEncrypted => EncryptKey.IsNotBlank();

        /// <summary>
        /// Define JSON serialization parameters
        /// </summary>
        [IgnoreDataMember] public JSONParameters JsonParameters { get; set; } = new JSONParameters();

        #endregion Public Properties



        #region Public Methods

        /// <summary>
        /// Load values of a JsonFile into a <typeparamref name="T"/> object
        /// </summary>
        /// <typeparam name="T">Object T</typeparam>
        /// <param name="File">File File</param>
        /// <param name="EncryptKey">Encrypt Key. Leave Null or blank to not encrypt</param>
        /// <returns></returns>
        public static T Load<T>(FileInfo File, string EncryptKey, JSONParameters parameters = null) where T : JsonFile => Load<T>(File?.FullName, EncryptKey, parameters);

        /// <inheritdoc cref="Load{T}(FileInfo,string,JSONParameters)"/>
        public static T Load<T>(DirectoryInfo Directory, string EncryptKey, JSONParameters parameters = null) where T : JsonFile => Load<T>(Directory?.FullName, EncryptKey, parameters);

        /// <inheritdoc cref="Load{T}(FileInfo,string,JSONParameters)"/>
        public static T Load<T>() where T : JsonFile => Load<T>(Util.EmptyString, null);

        /// <inheritdoc cref="Load{T}(FileInfo,string,JSONParameters)"/>
        public static T Load<T>(string FileOrDirectoryPath, string EncryptKey, JSONParameters parameters = null) where T : JsonFile
        {
            T c;
            try
            {
                c = Activator.CreateInstance<T>();
            }
            catch (Exception ex)
            {
                throw new NotSupportedException("Your POCO class need a public, parameterless contructor", ex);
            }

            c.FilePath = FileOrDirectoryPath;
            c.JsonParameters = parameters ?? JSON.Parameters;
            c.EncryptKey = EncryptKey;
            c.Load();

            return c;
        }

        /// <summary>
        /// Delete the file and return TRUE if file can be re-created
        /// </summary>
        /// <returns></returns>
        public bool Delete() => this.File.DeleteIfExist();

        /// <summary>
        /// Get the Json String representation of this file.
        /// </summary>
        /// <returns></returns>
        public string GetJson(bool Minify = false) => Minify ? this.ToJson(this.JsonParameters ?? JSON.Parameters) : this.ToNiceJson(this.JsonParameters ?? JSON.Parameters);

        public void Load()
        {
            if (this.File.Exists)
            {
                string s = this.File.ReadAllText();
                if (EncryptKey.IsNotBlank())
                {
                    s = s.Decrypt(EncryptKey);
                }
                var a = this.FilePath;
                var b = this.JsonParameters ?? JSON.Parameters;
                var c = this.EncryptKey;
                JSON.FillObject(this, s, this.GetType());
                this.FilePath = a;
                this.JsonParameters = b;
                this.EncryptKey = c;
            }
        }

        /// <summary>
        /// Save the current values into a JsonFile
        /// </summary>
        /// <returns></returns>
        public FileInfo Save(bool Minify = false)
        {
            var s = GetJson(Minify || IsEncrypted);
            if (IsEncrypted)
            {
                s = s.Encrypt(this.EncryptKey);
            }

            return s.WriteToFile(this.FilePath);
        }

        #endregion Public Methods
    }
}