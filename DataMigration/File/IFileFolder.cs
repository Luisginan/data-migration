namespace OneLonDataMigration.File;

public interface IFileFolder
{
    string[] GetFiles(string path);
    string GetFileName(string path);
    
    string ReadAllText(string path);
    void WriteAllText(string path, string content);
    bool IsFolderExists(string outputFolder);
    void CreateFolder(string outputFolder);
}