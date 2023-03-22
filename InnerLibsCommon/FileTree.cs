using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Extensions.Files
{
    public class FileTree : FileSystemInfo
    {
        private List<FileTree> _children = new List<FileTree>();

        internal FileTree(DirectoryInfo Directory, FileTree parent, string[] FileSearchPatterns)
        {
            this.OriginalPath = Directory.FullName;
            this.FullPath = Directory.FullName;
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
            this.OriginalPath = File.FullName;
            this.FullPath = File.FullName;
            Parent = parent;
            _children = new List<FileTree>(new List<FileTree>());
        }

        //TODO: construtor que permite aninhar arquivos relacionados

        internal void Construct(DirectoryInfo Directory, params string[] FileSearchPatterns)
        {
            this.OriginalPath = Directory.FullName;
            this.FullPath = Directory.FullName;
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

        public FileTree(string Path, params string[] FileSearchPatterns)
        {
            this.OriginalPath = Path;

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
                throw new ArgumentException("Path is not valid", nameof(Path));
            }
        }

        public FileTree(DirectoryInfo Directory, params string[] FileSearchPatterns) => Construct(Directory, FileSearchPatterns);

        public IEnumerable<FileTree> Children => _children.AsEnumerable();

        public bool IsDirectory => Path.IsDirectoryPath();

        public bool IsFile => Path.IsFilePath();

        public FileTree Parent { get; private set; }
        public string Path => this.FullPath;
        public string Title => Name.FileNameAsTitle();

        public string TypeDescription => IsDirectory ? "Directory" : (GetFileType()?.Description) ?? "File";

        public override string Name => System.IO.Path.GetFileName(this.Path);

        public override bool Exists => this.IsDirectory ? Directory.Exists(Path) : File.Exists(Path);

        public static implicit operator DirectoryInfo(FileTree Ft) => Ft.IsDirectory ? new DirectoryInfo(Ft.Path) : new FileInfo(Ft.Path).Directory;

        public static implicit operator FileInfo(FileTree Ft) => Ft.IsFile ? new FileInfo(Ft.Path) : null;

        public FileType GetFileType() => IsFile ? FileType.GetFileType(Path) : null;

        public Bitmap GetIcon() => this.GetIcon().ToBitmap();

        public override string ToString() => this.FullName;

        public override void Delete()
        {
            if (this.IsDirectory && Exists)
            {
                Directory.Delete(Path);
            }

            if (this.IsFile && Exists)
            {
                File.Delete(Path);
            }
        }
    }
}