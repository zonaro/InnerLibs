using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace InnerLibs
{
    public class FileTree
    {
        #region Private Fields

        private List<FileTree> _children = new List<FileTree>();

        private FileSystemInfo info;

        #endregion Private Fields

        #region Internal Constructors

        internal FileTree(DirectoryInfo Directory, FileTree parent, string[] FileSearchPatterns)
        {
            info = Directory;
            Parent = parent;
            var f = new List<FileTree>();
            foreach (var d in Directory.GetDirectories())
            {
                var a = new FileTree(d, this, FileSearchPatterns);
                if (a._children.Any())
                {
                    f.Add(a);
                }
            }

            foreach (var pt in FileSearchPatterns)
            {
                foreach (var d in Directory.GetFiles(pt, SearchOption.TopDirectoryOnly))
                {
                    f.Add(new FileTree(d, this));
                }
            }

            _children = new List<FileTree>(f);
        }

        internal FileTree(FileInfo File, FileTree parent)
        {
            info = File;
            Parent = parent;
            _children = new List<FileTree>(new List<FileTree>());
        }

        #endregion Internal Constructors

        //TODO: construtor que permite aninhar arquivos relacionados

        #region Internal Methods

        internal void Construct(DirectoryInfo Directory, params string[] FileSearchPatterns)
        {
            info = Directory;
            Parent = null;
            FileSearchPatterns = FileSearchPatterns ?? Array.Empty<string>();
            if (!FileSearchPatterns.Any())
            {
                FileSearchPatterns = new[] { "*" };
            }
            if (Directory.Exists)
            {
                _children = new List<FileTree>(new[] { new FileTree(Directory, this, FileSearchPatterns) }.ToList());
            }
        }

        #endregion Internal Methods

        #region Public Constructors

        public FileTree(string Path, params string[] FileSearchPatterns)
        {
            if (Path.IsDirectoryPath())
            {
                Construct(new DirectoryInfo(Path), FileSearchPatterns);
            }
            else if (Path.IsFilePath())
            {
                Construct(new FileInfo(Path).Directory, FileSearchPatterns);
            }
            else
            {
                throw new ArgumentException("Path is not valid");
            }
        }

        public FileTree(DirectoryInfo Directory, params string[] FileSearchPatterns) => Construct(Directory, FileSearchPatterns);

        #endregion Public Constructors

        #region Public Properties

        public IEnumerable<FileTree> Children => _children.AsEnumerable();

        public DateTime CreationTime => info.CreationTime;

        public string Extension => info.Extension;
        public bool IsDirectory => info is DirectoryInfo;

        public bool IsFile => info is FileInfo;

        public DateTime LastAccessTime => info.LastAccessTime;

        public DateTime LastWriteTime => info.LastWriteTime;

        public string Name => info.Name;
        public FileTree Parent { get; private set; }
        public string Path => info.FullName;
        public string Title => info.FileNameAsTitle();

        public string TypeDescription => IsDirectory ? "Directory" : (GetFileType()?.Description) ?? "File";

        #endregion Public Properties

        #region Public Methods

        public static implicit operator DirectoryInfo(FileTree Ft) => Ft.IsDirectory ? new DirectoryInfo(Ft.Path) : new FileInfo(Ft.Path).Directory;

        public static implicit operator FileInfo(FileTree Ft) => Ft.IsFile ? new FileInfo(Ft.Path) : null;

        public FileType GetFileType()
        {
            if (IsFile)
            {
                return FileType.GetFileType(Path);
            }

            return null;
        }

        public Bitmap GetIcon() => info.GetIcon().ToBitmap();

        public override string ToString() => info.Name;

        #endregion Public Methods
    }
}