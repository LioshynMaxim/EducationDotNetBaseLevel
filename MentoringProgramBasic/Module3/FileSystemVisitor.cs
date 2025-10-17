using System;
using System.Collections.Generic;
using System.IO;

namespace Module3_ClassLibrary
{
    public partial class FileSystemVisitor : IFileSystemVisitor
    {
        private readonly string _rootPath;
        private readonly Func<string, bool> _filter;
        private bool _abort;

        public event EventHandler<EventArgs> Start;
        public event EventHandler<EventArgs> Finish;
        public event EventHandler<VisitorEventArgs> FileFound;
        public event EventHandler<VisitorEventArgs> DirectoryFound;
        public event EventHandler<VisitorEventArgs> FilteredFileFound;
        public event EventHandler<VisitorEventArgs> FilteredDirectoryFound;
        public event EventHandler<TraversalErrorEventArgs> TraversalError;

        public int MaxDepth { get; set; } = int.MaxValue;
        public bool FollowSymbolicLinks { get; set; } = false;

        public FileSystemVisitor(string rootPath)
        {
            _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            _filter = null;
        }

        public FileSystemVisitor(string rootPath, Func<string, bool> filter)
        {
            _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
            _filter = filter;
        }

        public FileSystemVisitor(string rootPath, IFileSystemFilter filter)
        {
            _rootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));

            if (filter != null)
            {
                _filter = path => filter.ShouldInclude(path, Directory.Exists(path));
            }
        }

        public IEnumerable<string> Traverse()
        {
            _abort = false;
            Start?.Invoke(this, EventArgs.Empty);

            try
            {
                foreach (var item in TraverseInternal(_rootPath, 0))
                {
                    if (_abort)
                    {
                        break;
                    }
                    yield return item;
                }
            }
            finally
            {
                Finish?.Invoke(this, EventArgs.Empty);
            }
        }

        private IEnumerable<string> TraverseInternal(string currentPath, int depth)
        {
            if (depth > MaxDepth)
            {
                yield break;
            }

            foreach (var item in ProcessItems(currentPath, true, depth))
            {
                yield return item;
            }

            foreach (var item in ProcessItems(currentPath, false, depth))
            {
                yield return item;
            }
        }

        private IEnumerable<string> ProcessItems(string path, bool isDirectory, int depth)
        {
            IEnumerable<string> items;

            try
            {
                items = isDirectory
                    ? Directory.EnumerateDirectories(path)
                    : Directory.EnumerateFiles(path);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException ||
                                      ex is DirectoryNotFoundException ||
                                      ex is IOException)
            {
                TraversalError?.Invoke(this, new TraversalErrorEventArgs(path, ex));
                yield break;
            }

            foreach (var item in items)
            {
                if (_abort) yield break;

                if (!FollowSymbolicLinks && IsSymbolicLink(item))
                {
                    continue;
                }

                var itemArgs = new VisitorEventArgs(item);
                var eventHandler = isDirectory ? DirectoryFound : FileFound;
                eventHandler?.Invoke(this, itemArgs);

                if (itemArgs.Abort)
                {
                    _abort = true;
                    yield break;
                }

                if (itemArgs.Exclude)
                {
                    continue;
                }

                bool passedFilter = _filter == null || _filter(item);
                if (passedFilter)
                {
                    var filteredArgs = new VisitorEventArgs(item);
                    var filteredEventHandler = isDirectory ? FilteredDirectoryFound : FilteredFileFound;
                    filteredEventHandler?.Invoke(this, filteredArgs);

                    if (filteredArgs.Abort)
                    {
                        _abort = true;
                        yield break;
                    }

                    if (!filteredArgs.Exclude)
                    {
                        yield return item;
                    }
                }

                if (isDirectory)
                {
                    foreach (var subItem in TraverseInternal(item, depth + 1))
                    {
                        if (_abort) yield break;
                        yield return subItem;
                    }
                }
            }
        }

        private bool IsSymbolicLink(string path)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                return fileInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
            }
            catch
            {
                return false;
            }
        }
    }

}