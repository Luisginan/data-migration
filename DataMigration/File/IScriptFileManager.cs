using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public interface IScriptFileManager
{
    List<FileScript> GetFileScripts(int lastOrderNumber);
    void SaveScript(ScriptData scriptData, string outputFolder);
    bool IsFolderExists(string outputFolder);
    void CreateFolder(string outputFolder);
}