using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Extensions.Files;

namespace Extensions
{
    public static class IconUtil
    {
        #region Windows API Imports
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
            ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool DestroyIcon(IntPtr handle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
            public uint dwAttributes;
        }

        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
        #endregion

        /// <summary>
        /// Obtém o ícone de um arquivo ou pasta
        /// </summary>
        /// <param name="fileSystemInfo">O arquivo ou pasta</param>
        /// <param name="largeIcon">True para ícone grande, false para pequeno</param>
        /// <returns>Icon do arquivo/pasta ou null se não conseguir obter</returns>
        public static Icon GetIcon(this FileSystemInfo fileSystemInfo, bool largeIcon = true) => GetIcon(fileSystemInfo?.FullName, fileSystemInfo is DirectoryInfo, largeIcon);

        /// <summary>
        /// Obtém o ícone de um arquivo ou pasta pelo caminho
        /// </summary>
        /// <param name="path">Caminho do arquivo ou pasta</param>
        /// <param name="isDirectory">True se for uma pasta</param>
        /// <param name="largeIcon">True para ícone grande, false para pequeno</param>
        /// <returns>Icon do arquivo/pasta ou null se não conseguir obter</returns>
        public static Icon GetIcon(this string path, bool isDirectory = false, bool largeIcon = true)
        {
            if (string.IsNullOrEmpty(path))
                return SystemIcons.Question;

            try
            {
                SHFILEINFO shinfo = new SHFILEINFO();
                uint flags = SHGFI_ICON | (largeIcon ? SHGFI_LARGEICON : SHGFI_SMALLICON);

                // Se o arquivo não existir, usa atributos de arquivo
                if (isDirectory || !File.Exists(path))
                {
                    flags |= SHGFI_USEFILEATTRIBUTES;

                    if (isDirectory)
                        flags |= FILE_ATTRIBUTE_DIRECTORY;
                }

                IntPtr hImgSmall = SHGetFileInfo(path, 0, ref shinfo,
                    (uint)Marshal.SizeOf(shinfo), flags);

                if (hImgSmall != IntPtr.Zero && shinfo.hIcon != IntPtr.Zero)
                {
                    Icon icon = Icon.FromHandle(shinfo.hIcon);
                    // Cria uma cópia do ícone para que possamos destruir o handle original
                    Icon iconCopy = (Icon)icon.Clone();
                    DestroyIcon(shinfo.hIcon);
                    return iconCopy;
                }
            }
            catch (Exception)
            {

            }
            return SystemIcons.Error;

        }

        /// <summary>
        /// Obtém o ícone como Bitmap
        /// </summary>
        /// <param name="fileSystemInfo">O arquivo ou pasta</param>
        /// <param name="largeIcon">True para ícone grande, false para pequeno</param>
        /// <returns>Bitmap do ícone ou null se não conseguir obter</returns>
        public static Bitmap GetIconAsBitmap(this FileSystemInfo fileSystemInfo, bool largeIcon = true)
        {
            using (Icon icon = fileSystemInfo.GetIcon(largeIcon))
            {
                return icon?.ToBitmap();
            }
        }

        public static Bitmap GetThumbnail(this FileSystemInfo fileSystemInfo)
        {
            if (fileSystemInfo != null && fileSystemInfo.Exists)
            {
                try
                {

                    if (fileSystemInfo is FileInfo fileInfo)
                    {
                        var type = FileType.GetFileType(fileInfo.FullName);

                        if (type.IsImage())
                        {
                            return Bitmap.FromFile(fileInfo.FullName).ToBitmap();
                        }
                    }
                }
                catch
                {

                }
            }
            return fileSystemInfo.GetIconAsBitmap();
        }


        /// <summary>
        /// Obtém o ícone como Bitmap pelo caminho
        /// </summary>
        /// <param name="path">Caminho do arquivo ou pasta</param>
        /// <param name="isDirectory">True se for uma pasta</param>
        /// <param name="largeIcon">True para ícone grande, false para pequeno</param>
        /// <returns>Bitmap do ícone ou null se não conseguir obter</returns>
        public static Bitmap GetIconAsBitmap(this string path, bool isDirectory = false, bool largeIcon = true)
        {
            using (Icon icon = path.GetIcon(isDirectory, largeIcon))
            {
                return icon?.ToBitmap();
            }
        }
    }
}

