using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace InnerLibs
{
    public class FileTree
    {
        public FileTree Parent { get; private set; }

        public string Name
        {
            get
            {
                return Info is object ? Info.Name : "";
            }
        }

        public Bitmap Icon
        {
            get
            {
                return Info.GetIcon().ToBitmap();
            }
        }

        public string Path
        {
            get
            {
                return Info is object ? Info.FullName : "";
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

        public bool IsFile
        {
            get
            {
                return Path.IsFilePath();
            }
        }

        public bool IsDirectory
        {
            get
            {
                return Path.IsDirectoryPath();
            }
        }

        public FileSystemInfo Info { get; private set; }

        private List<FileTree> _children = new List<FileTree>();

        public IEnumerable<FileTree> Children
        {
            get
            {
                return _children.AsEnumerable();
            }
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

        public override string ToString()
        {
            return Info.Name;
        }
    }
}