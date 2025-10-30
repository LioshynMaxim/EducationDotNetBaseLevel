using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module3_ClassLibrary
{
    public interface IFileSystemVisitor
    {
        event EventHandler<EventArgs> Start;
        event EventHandler<EventArgs> Finish;
        event EventHandler<VisitorEventArgs> FileFound;
        event EventHandler<VisitorEventArgs> DirectoryFound;
        event EventHandler<VisitorEventArgs> FilteredFileFound;
        event EventHandler<VisitorEventArgs> FilteredDirectoryFound;
        event EventHandler<TraversalErrorEventArgs> TraversalError;

        int MaxDepth { get; set; }
        bool FollowSymbolicLinks { get; set; }

        IEnumerable<string> Traverse();
    }
}
