namespace Module3_ClassLibrary
{
    public class TraversalErrorEventArgs(string path, Exception exception) : EventArgs
    {
        public string Path { get; } = path;
        public Exception Exception { get; } = exception;
    }
}
