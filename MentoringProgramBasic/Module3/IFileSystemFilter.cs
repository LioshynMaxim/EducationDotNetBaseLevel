namespace Module3_ClassLibrary;

public interface IFileSystemFilter
{
    bool ShouldInclude(string path, bool isDirectory);
}
