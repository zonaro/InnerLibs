using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Extensions;

namespace Extensions.Files
{
    public class FileTree : FileSystemInfo, IList<FileTree>
    {
        private List<FileTree> _children = new List<FileTree>();

        public FileTree(string Path, FileTree Parent) : this(Path, Parent, null, null) { }
        public FileTree(string Path) : this(Path, null, null, null) { }
        public FileTree(string Path, IEnumerable<string> FileSearchPatterns) : this(Path, null, FileSearchPatterns, null) { }
        public FileTree(string Path, IEnumerable<string> FileSearchPatterns, IEnumerable<string> RelatedFilesSearch) : this(Path, null, FileSearchPatterns, RelatedFilesSearch) { }
        public FileTree(string Path, FileTree Parent, IEnumerable<string> FileSearchPatterns) : this(Path, Parent, FileSearchPatterns, null) { }
        public FileTree(string Path, FileTree Parent, IEnumerable<string> FileSearchPatterns, IEnumerable<string> RelatedFilesSearch)
        {
            this.OriginalPath = Path;
            this.FullPath = System.IO.Path.GetFullPath(Path);
            this.Parent = Parent;
            FileSearchPatterns = FileSearchPatterns ?? Array.Empty<string>();
            RelatedFilesSearch = RelatedFilesSearch ?? Array.Empty<string>();
            if (!FileSearchPatterns.Any())
            {
                FileSearchPatterns = new[] { "*" };
            }

            _children = new List<FileTree>();
            if (Path.IsDirectoryPath())
            {
                if (System.IO.Directory.Exists(Path))
                {
                    foreach (var item in System.IO.Directory.EnumerateFileSystemEntries(Path))
                    {
                        if (!Children.Any(x => x.Children.Any(y => y.FullName == item)))
                            _children.Add(new FileTree(item, this, FileSearchPatterns, RelatedFilesSearch));
                    }
                }
            }
            else if (Path.IsFilePath())
            {
                if (File.Exists(Path))
                {
                    if (RelatedFilesSearch.Any())
                    {
                        var name = System.IO.Path.GetFileNameWithoutExtension(Path);
                        foreach (var item in this.Directory.EnumerateFiles().Where(item => item.FullName != this.FullName))
                        {
                            if (RelatedFilesSearch.Any(x => item.Name.Like(x.Inject(new { name }))))
                            {
                                FileTree ni = this.Parent?._children?.Detach(x => x.FullName == item.FullName) ?? new FileTree(item.FullName, this, Array.Empty<string>(), Array.Empty<string>());
                                ni.Parent = this;
                                _children.Add(ni);
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("File is not valid", nameof(Path));
            }
        }

        public Dictionary<string, object> ToDictionary() => new { Index = this.Parent?.GetIndexOf(this) ?? -1, Title, Name, FullName, Size, ShortSize, TypeDescription, Mime, Children = _children.Select(x => x.ToDictionary()) }.CreateDictionary();

        public string ToJson() => ToDictionary().ToNiceJson();

        public IEnumerable<FileTree> Children => _children.AsEnumerable();

        public bool IsDirectory => Path.IsDirectoryPath();

        public bool IsFile => Path.IsFilePath();

        public long Size => IsFile ? new FileInfo(this.Path).Length : this.Children.Sum(x => x.Size);

        public string ShortSize => this.Size.ToFileSizeString();

        public FileTree Parent { get; private set; }
        public DirectoryInfo Directory => new DirectoryInfo(System.IO.Path.GetDirectoryName(Path));
        public string Path => this.FullPath;
        public string Title => Name.FileNameAsTitle();

        public string Mime => IsFile ? GetFileType().ToString() : null;
        public string TypeDescription => IsDirectory ? "Directory" : (GetFileType()?.Description) ?? "FileOrDirectory";

        public override string Name => System.IO.Path.GetFileName(this.Path);

        public FileTree Rename(string Name, bool KeepOriginalExtension = false)
        {
            if (Name.IsNotBlank())
            {
                if (this.IsFile)
                    this.FullPath = new FileInfo(Path).Rename(Name, KeepOriginalExtension).FullName;
                else this.FullPath = new DirectoryInfo(Path).Rename(Name).FullName;
            }
            return this;
        }

        public override bool Exists => this.IsDirectory ? System.IO.Directory.Exists(Path) : File.Exists(Path);

        public int Count => _children.Count;

        public bool IsReadOnly => false;



        public FileTree this[int index] { get => Children.IfNoIndex(index); set => Add(value); }

        public static implicit operator DirectoryInfo(FileTree Ft) => Ft.IsDirectory ? new DirectoryInfo(Ft.Path) : new FileInfo(Ft.Path).Directory;

        public static implicit operator FileInfo(FileTree Ft) => Ft.IsFile ? new FileInfo(Ft.Path) : null;

        public FileType GetFileType() => IsFile ? FileType.GetFileType(Path) : null;

        public Bitmap GetIcon() => this.GetIcon().ToBitmap();

        public override string ToString() => this.FullName;

        public override void Delete()
        {
            if (this.IsDirectory && Exists)
            {
                System.IO.Directory.Delete(this.Path);
            }

            if (this.IsFile && Exists)
            {
                File.Delete(this.Path);
            }

            this.Parent?.Remove(this);
        }

        public int IndexOf(FileTree item) => _children.IndexOf(item);

        public void Insert(int index, FileTree item) => Add(item);

        public void RemoveAt(int index)
        {
            var t = _children.IfNoIndex(index);
            if (t != null) Remove(t);
        }

        public void Add(FileTree item)
        {
            if (item != null)
            {
                _children.Add(item);
                if (item.IsDirectory)
                {
                    new DirectoryInfo(item.FullName).MoveTo(this.Directory.FullName);
                }
                if (item.IsFile)
                {
                    new FileInfo(item.FullName).MoveTo(System.IO.Path.Combine(this.Directory.FullName, item.Name));
                }
                item.Parent = this;
            }
        }

        public void Clear()
        {
            while (_children.Count > 0)
            {
                RemoveAt(0);
            }
        }

        public bool Contains(FileTree item) => Children.Contains(item);

        public void CopyTo(FileTree[] array, int arrayIndex) => _children.CopyTo(array, arrayIndex);

        public bool Remove(FileTree item)
        {
            if (item.Exists) item.Delete();
            return item.Exists == false && _children.Remove(item);
        }

        public IEnumerator<FileTree> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator() as IEnumerator;
    }
}