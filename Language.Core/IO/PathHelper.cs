#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Language.IO {

    public static class PathHelper {

        [CanBeNull]
        [ContractAnnotation("path:null => null")]
        public static string NormalizePath(string path) {

            if (String.IsNullOrWhiteSpace(path)) {
                return null;
            }

            try {
                return GetFullPathNoThrow(new Uri(path).LocalPath).ToLowerInvariant();
            } catch (UriFormatException) {
                return path;
            }
        }

        [ContractAnnotation("=> true, fileInfo: notnull; => false, fileInfo: null")]
        public static bool TryGetFileInfo(string candidate, out FileInfo fileInfo) {
            fileInfo = default;

            if (String.IsNullOrEmpty(candidate)) {
                return false;
            }

            candidate = candidate.Trim('"', '\'');

            try {
                fileInfo = new FileInfo(candidate);
                return fileInfo.Exists;
            } catch (ArgumentException) {
            } catch (NotSupportedException) {
            } catch (IOException) {
            }

            return false;
        }

        [ContractAnnotation("=> true, directoryInfo: notnull; => false, directoryInfo: null")]
        public static bool TryGetDirectoryinfo(string path, out DirectoryInfo directoryInfo) {
            directoryInfo = default;
            try {

                directoryInfo = new DirectoryInfo(path);
                return true;
            } catch (NotSupportedException) {
            } catch (ArgumentException) {
            } catch (IOException) {
            }

            return false;
        }

        [ContractAnnotation("=> true, combinedPath: notnull; => false, combinedPath: null")]
        public static bool TryCombinePath(string path1, string path2, out string combinedPath) {
            combinedPath = default;
            try {
                combinedPath = Path.Combine(path1, path2);
                return true;
            } catch (ArgumentException) {
                return false;
            }
        }

        public static bool? TryGetIsPathRooted(string path) {
            try {
                return Path.IsPathRooted(path);
            } catch (ArgumentException) {
                return null;
            }
        }

        public static IEnumerable<FileInfo> TryEnumerateFiles(this DirectoryInfo directoryInfo, string searchPattern, SearchOption searchOption) {
            try {
                return directoryInfo.EnumerateFiles(searchPattern, searchOption);
            } catch (IOException) {
            } catch (ArgumentException) {
            }

            return Enumerable.Empty<FileInfo>();
        }

        public static IEnumerable<DirectoryInfo> TryEnumerateDirectories(this DirectoryInfo directoryInfo) {
            try {
                return directoryInfo.EnumerateDirectories();
            } catch (IOException) {
            } catch (ArgumentException) {
            }

            return Enumerable.Empty<DirectoryInfo>();
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path.</returns>
        /// <exception cref="System.InvalidOperationException"><paramref name="fromPath"/> or <paramref name="fromPath"/> is <c>null</c>.</exception>
        /// <exception cref="System.UriFormatException"></exception>
        /// <exception cref="System.UriFormatException"></exception>
        public static string GetRelativePath(string fromPath, string toPath) {
            if (String.IsNullOrEmpty(fromPath)) {
                throw new ArgumentNullException(nameof(fromPath));
            }

            if (String.IsNullOrEmpty(toPath)) {
                throw new ArgumentNullException(nameof(toPath));
            }

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri   = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme) {
                return toPath;
            }

            Uri    relativeUri  = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (String.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)) {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        static string AppendDirectorySeparatorChar(string path) {
            // Append a slash only if the path is a directory and does not have a slash.
            if (!Path.HasExtension(path) &&
                !path.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                return path + Path.DirectorySeparatorChar;
            }

            return path;
        }

        public static string GetFullPathNoThrow(string path) {
            try {
                path = Path.GetFullPath(path);
            } catch (Exception e) when (IsIoRelatedException(e)) {
            }

            return path;
        }

        internal static bool IsIoRelatedException(Exception e) =>
            e is UnauthorizedAccessException                          ||
            e is NotSupportedException                                ||
            (e is ArgumentException && !(e is ArgumentNullException)) ||
            e is SecurityException                                    ||
            e is IOException;

    }

}