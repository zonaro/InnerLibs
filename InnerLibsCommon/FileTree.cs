using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Extensions;

namespace Extensions.Files
{
    [Serializable]
    public class FileTree : FileSystemInfo, IList<FileTree>
    {
        private List<FileTree> _children = new List<FileTree>();

        public FileTree(string Path, FileTree Parent, bool DirectoryOnly = false) : this(Path, Parent, null, DirectoryOnly) { }
        public FileTree(string Path, bool DirectoryOnly = false) : this(Path, null, null, DirectoryOnly) { }
        public FileTree(string Path, IEnumerable<string> FileSearchPatterns, bool DirectoryOnly = false) : this(Path, null, FileSearchPatterns, DirectoryOnly) { }


        public FileTree(string Path, FileTree Parent, IEnumerable<string> FileSearchPatterns, bool DirectoryOnly = false)
        {

            this.OriginalPath = Path;
            this.FullPath = System.IO.Path.GetFullPath(Path);

            if (this.FullPath.IsPath() == false) throw new ArgumentException("Path is not a valid path", nameof(Path));

            this.Parent = Parent;
            FileSearchPatterns = FileSearchPatterns ?? Array.Empty<string>();

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
                        {
                            if (item.IsDirectoryPath() || (item.IsFilePath() && System.IO.Path.GetFileName(item).IsLikeAny(FileSearchPatterns)))
                            {
                                if (item.IsFilePath() && DirectoryOnly)
                                    continue;

                                _children.Add(new FileTree(item, this, FileSearchPatterns));
                            }
                        }
                    }
                }
            }

        }

        public   IEnumerable<string> FileSearchPatterns { get; private set; }
        public Dictionary<string, object> ToDictionary(Func<FileTree, Dictionary<string, object>> aditionalInfo = null)
        {

            var dic = new
            {
                Index,
                IsDirectory,
                IsFile,
                Title,
                Name,
                FullName,
                Size,
                ShortSize,
                TypeDescription,
                Mime,
                Count,
                CheckSum,
                Children = IsDirectory ? _children.Select(x => x.ToDictionary(aditionalInfo)).ToList() : null
            }.CreateDictionary();

            if (aditionalInfo != null)
            {
                foreach (var x in aditionalInfo.Invoke(this))
                    dic[x.Key] = x.Value;
            }
            return dic;
        }

        public string ToJson(Func<FileTree, Dictionary<string, object>> aditionalInfo = null) => ToDictionary(aditionalInfo).ToNiceJson(new JSONParameters()
        {
            SerializeBlankStringsAsNull = true,
            SerializeNullValues = false,
        });
        public string ToJson(JSONParameters parameters, Func<FileTree, Dictionary<string, object>> aditionalInfo = null) => ToDictionary(aditionalInfo).ToNiceJson(parameters);

        public IEnumerable<FileTree> Children => _children.AsEnumerable();


        public bool IsDirectory => Path.IsDirectoryPath();



        public bool IsFile => Path.IsFilePath();


        public string CheckSum => this.GetSHA256Checksum();


        public byte[] GetBytes() => IsFile ? File.ReadAllBytes(this.Path) : null;

        public string GetTextContent() => IsFile ? File.ReadAllText(this.Path) : null;

        /// <summary>
        /// Retorna o tamano em bytes deste arquivo ou diretório
        /// </summary>
        public long Size => this.GetSize();

        /// <summary>
        /// Retorna o tamano em bytes deste arquivo ou diretório usando a maior unidade de medida possivel
        /// </summary>
        public string ShortSize => this.Size.ToFileSizeString();

        /// <summary>
        /// Diretorio pai
        /// </summary>
        public FileTree Parent { get; private set; }

        /// <summary>
        /// Diretório ou arquivo mais alto na hierarquia de arquivos
        /// </summary>
        public FileTree Root => this.TraverseUp(x => x.Parent);

        /// <summary>
        /// Diretório mais alto na hierarquia de arquivos. Se <see cref="Root"/> for um arquivo, esta propriedade retornará o diretorio deste arquivo
        /// </summary>
        public DirectoryInfo RootDirectory
        {
            get
            {
                var p = this.TraverseUp(x => x.Parent);
                return p.IsDirectory ? p : p.Directory;
            }
        }

        public string RelativePath => Util.GetRelativePath(this.Path, Root.Path);

        public DirectoryInfo Directory => new DirectoryInfo(System.IO.Path.GetDirectoryName(Path));
        public string Path => this.FullPath;
        public string Title => Name.FileNameAsTitle();

        public string Mime => IsFile ? GetFileType().ToString() : null;
        public string TypeDescription => this.GetDescription();

        public override string Name => System.IO.Path.GetFileName(this.Path);

        public FileTree Rename(string Name, bool KeepOriginalExtension = false)
        {
            if (Name.IsValid())
            {
                if (this.IsFile)
                    this.FullPath = new FileInfo(Path).Rename(Name, KeepOriginalExtension).FullName;
                else this.FullPath = new DirectoryInfo(Path).Rename(Name).FullName;
            }
            return this;
        }

        public override bool Exists => this.IsDirectory ? System.IO.Directory.Exists(Path) : File.Exists(Path);

        /// <summary>
        /// Retorno a quantidade de arquivos deste diretório
        /// </summary>
        public int Count => IsDirectory ? _children.Count : this.Parent?.Count ?? this.Directory.EnumerateFileSystemInfos().Count();

        public bool IsReadOnly => false;

        public int Index => (this.Parent != this ? this.Parent?.Children.GetIndexOf(this) : null) ?? -1;

        public FileTree this[int index]
        {
            get => Children.FirstOrDefault(x => x.Index == index);
            set => Insert(index, value);

        }

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

        public int IndexOf(FileTree item) => item?.Index ?? -1;

        /// <summary>
        /// Add a item to this Filetree.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, FileTree item)
        {
            if (item != null && item.Exists)
            {
                if (item.IsDirectory)
                {
                    new DirectoryInfo(item.FullName).MoveTo(this.Directory.FullName);
                }
                if (item.IsFile)
                {
                    new FileInfo(item.FullName).MoveTo(System.IO.Path.Combine(this.Directory.FullName, item.Name));
                }
                item.Parent = this;
                _children.Insert(index, item);
            }

        }
        /// <summary>
        /// DeleteCliente and remove a file or directory from tree at especific index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void RemoveAt(int index)
        {
            if (IsDirectory)
            {
                var item = Children.FirstOrDefault(x => x.Index == index);
                if (item != null)
                {
                    if (item.Exists) item.Delete();
                    _children.Remove(item);
                }

            }

        }

        public void Add(FileTree item) => Insert(_children.LastOrDefault()?.Index ?? 0, item);

        public void Clear()
        {
            while (_children.Count > 0)
            {
                RemoveAt(0);
            }
        }

        public bool Contains(FileTree item) => Children.Contains(item);

        public void CopyTo(FileTree[] array, int arrayIndex) => _children.CopyTo(array, arrayIndex);

        /// <summary>
        /// DeleteCliente and remove a file or directory from tree
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(FileTree item)
        {
            RemoveAt(item.Index);
            return item.Exists == false;
        }

        public IEnumerator<FileTree> GetEnumerator() => Children.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator() as IEnumerator;
    }
}