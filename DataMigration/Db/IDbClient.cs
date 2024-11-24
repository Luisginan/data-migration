using OneLonDataMigration.Models;

namespace OneLonDataMigration.Db;

public interface IDbClient
{
    List<HistoryScript> GetHistoryScripts();
    List<HistoryScript> GetHistoryScriptsWithContent(string version);
    void ExecuteScript(string scriptDataScriptContent);
    void InsertHistoryScript(ScriptData scriptData);
    List<HistoryScript> FindScript(string file);
}