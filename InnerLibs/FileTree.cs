using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace InnerLibs
{
    public class FileTree
    {
        private List<FileTree> _children = new List<FileTree>();

        private FileSystemInfo info;

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

        //TODO: construtor que permite aninhar arquivos relacionados

        public FileTree(DirectoryInfo Directory, params string[] FileSearchPatterns)
        {
            info = Directory;
            Parent = null;
            FileSearchPatterns = FileSearchPatterns ?? Array.Empty<string>();
            if (!FileSearchPatterns.Any())
            {
                FileSearchPatterns = new[] { "*" };
            }

            _children = new List<FileTree>(new[] { new FileTree(Directory, this, FileSearchPatterns) }.ToList());
        }

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

        public FileType GetFileType()
        {
            if (Path.IsFilePath())
            {
                return FileType.GetFileType(Path);
            }

            return null;
        }

        public Bitmap GetIcon() => info.GetIcon().ToBitmap();

        public override string ToString() => info.Name;
    }
}