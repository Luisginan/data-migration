namespace OneLonDataMigration;

public interface IDbClient
{
    List<HistoryScript> GetHistoryScripts();
    void ExecuteScript(string scriptDataScriptContent);
    void InsertHistoryScript(ScriptData scriptData);
}