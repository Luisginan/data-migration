using OneLonDataMigration.Models;

namespace OneLonDataMigration.Db;

public interface IScriptManager
{
    List<ScriptData> GetDiffScripts();
    List<ScriptData> ExecuteAllScripts(List<ScriptData> listScriptDif);
    List<ScriptData> GetScriptFromDb(string version);
}