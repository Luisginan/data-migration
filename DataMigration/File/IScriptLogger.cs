using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public interface IScriptLogger
{
    void LogScript(List<ScriptData> listScriptDif);
}