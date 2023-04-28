namespace MbTilesManager3.FileManagement;

public class FilePathUtilityExample
{
    public void Example()
    {
        string path = FilePathUtility.Combine("C:\\", "Users", "james", "Desktop", "mbtiles", "test.mbtiles");
        string? directoryName = FilePathUtility.GetDirectoryName(path);
        string extension = FilePathUtility.GetExtension(path);
        string fileName = FilePathUtility.GetFileName(path);
        string fullPath = FilePathUtility.GetFullPath(path);
        bool isPathRooted = FilePathUtility.IsPathRooted(path);
    }
}

public static class FilePathUtility
{
    public static string Combine(params string[] paths)
    {
        return Path.Combine(paths);
    }

    public static string? GetDirectoryName(string path)
    {
        return Path.GetDirectoryName(path);
    }

    public static string GetExtension(string path)
    {
        return Path.GetExtension(path);
    }

    public static string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public static string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }

    public static bool IsPathRooted(string path)
    {
        return Path.IsPathRooted(path);
    }
}