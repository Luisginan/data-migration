using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public interface IScriptFileManager
{
    List<FileScript> GetFileScripts(int lastOrderNumber);
}