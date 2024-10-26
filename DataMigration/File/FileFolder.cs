namespace OneLonDataMigration.File;

public class FileFolder: IFileFolder
{
    public string[] GetFiles(string path) 
    {
        return Directory.GetFiles(path, "*.sql");
    }

    public string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }

    public string ReadAllText(string path)
    {
        return System.IO.File.ReadAllText(path);
    }

    public void WriteAllText(string path, string content)
    {
        System.IO.File.WriteAllText(path, content);
    }
}