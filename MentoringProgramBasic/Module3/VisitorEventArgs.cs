namespace Module3_ClassLibrary
{
    public class VisitorEventArgs(string path) : EventArgs
    {
        public string Path { get; } = path;
        public bool Abort { get; set; } = false;
        public bool Exclude { get; set; } = false;
    }
}