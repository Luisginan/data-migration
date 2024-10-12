using System.Text.Json;

namespace OneLonDataMigration;

public class ScriptFileLogger : IScriptLogger
{
    public void LogScript(List<ScriptData> listScriptDif)
    {
        var json = JsonSerializer.Serialize(listScriptDif);
        File.WriteAllText($"script_log_{DateTime.Now:yyyy-M-d dddd}.json", json);
    }
}