using System;
using System.IO;

namespace InnerLibs.JSONParser
{
    public abstract class JsonFile
    {
        private string file_path;
        private string encrypt_key;

        public FileInfo GetFile() => new FileInfo(file_path.BlankCoalesce(DefaultFileName));

        public static string DefaultFileName => $"{Environment.CurrentDirectory}\\config.json";

        public static T Load<T>(FileInfo FilePath, string EncryptKey) where T : JsonFile => Load<T>(FilePath?.FullName, EncryptKey);
        public static T Load<T>(FileInfo FilePath) where T : JsonFile => Load<T>(FilePath, null);
        public static T Load<T>(string FilePath) where T : JsonFile => Load<T>(FilePath, null);
        public static T Load<T>() where T : JsonFile => Load<T>(DefaultFileName, null);
        public static T Load<T>(string FilePath, string EncryptKey) where T : JsonFile
        {

            FilePath = FilePath.IfBlank(DefaultFileName).FixPath();
            T c = Activator.CreateInstance<T>();

            if (File.Exists(c.file_path))
            {
                string s = File.ReadAllText(c.file_path);
                if (EncryptKey.IsNotBlank())
                {
                    s = s.Decrypt(EncryptKey);
                }

                c = s.FromJson<T>();
            }

            c.SetFilePath(FilePath);
            c.SetEncryptKey(EncryptKey);
            c.Save();
            return c;
        }

        public FileInfo Save(FileInfo FilePath) => new FileInfo(Save(FilePath?.FullName));
        public string Save(string FilePath)
        {
            FilePath = FilePath.BlankCoalesce(file_path, DefaultFileName).FixPath();
            var s = this.ToJson();
            if (this.encrypt_key.IsNotBlank())
            {
                s = s.Encrypt(this.encrypt_key);
            }

            s.WriteToFile(FilePath);
            return FilePath;
        }

        public void SetEncryptKey(string EncryptKey) => this.encrypt_key = EncryptKey.NullIf(x => x.IsBlank());
        public void SetFilePath(string FilePath) => this.file_path = FilePath.BlankCoalesce(DefaultFileName);

        public string Save() => Save(file_path);
    }
}
