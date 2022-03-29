using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace InnerLibs
{
    public class FileTree
    {
        private List<FileTree> _children = new List<FileTree>();

        internal FileTree(DirectoryInfo Directory, FileTree parent, string[] FileSearchPatterns)
        {
            Info = Directory;
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
                    f.Add(new FileTree(d, this));
            }

            _children = new List<FileTree>(f);
        }

        internal FileTree(FileInfo File, FileTree parent)
        {
            Info = File;
            Parent = parent;
            _children = new List<FileTree>(new List<FileTree>());
        }

        public FileTree(string Directory, params string[] FileSearchPatterns) : this(new DirectoryInfo(Directory), FileSearchPatterns)
        {
        }

        public FileTree(DirectoryInfo Directory, params string[] FileSearchPatterns)
        {
            Info = Directory;
            Parent = null;
            if (FileSearchPatterns is null || FileSearchPatterns.Count() == 0)
            {
                FileSearchPatterns = new[] { "*" };
            }

            _children = new List<FileTree>(new[] { new FileTree(Directory, this, FileSearchPatterns) }.ToList());
        }

        public IEnumerable<FileTree> Children
        {
            get
            {
                return _children.AsEnumerable();
            }
        }

        public FileType FileType
        {
            get
            {
                if (Path.IsFilePath())
                {
                    return FileType.GetFileType(Path);
                }

                return null;
            }
        }

        public Bitmap Icon
        {
            get
            {
                return Info.GetIcon().ToBitmap();
            }
        }

        public FileSystemInfo Info { get; private set; }

        public bool IsDirectory
        {
            get
            {
                return Path.IsDirectoryPath();
            }
        }

        public bool IsFile
        {
            get
            {
                return Path.IsFilePath();
            }
        }

        public string Name
        {
            get
            {
                return Info != null ? Info.Name : "";
            }
        }

        public FileTree Parent { get; private set; }

        public string Path
        {
            get
            {
                return Info != null ? Info.FullName : "";
            }
        }

        public string TypeDescription
        {
            get
            {
                if (IsDirectory)
                {
                    return "Directory";
                }

                return FileType?.Description;
            }
        }

        public override string ToString()
        {
            return Info.Name;
        }
    }
}