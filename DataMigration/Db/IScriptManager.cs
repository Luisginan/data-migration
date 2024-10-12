namespace OneLonDataMigration;

public interface IScriptManager
{
    List<ScriptData> GetDiffScripts();
    List<ScriptData> ExecuteAllScripts(List<ScriptData> listScriptDif);
}