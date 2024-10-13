
using Newtonsoft.Json;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public class ScriptFileLogger : IScriptLogger
{
    public void LogScript(List<ScriptData> listScriptDif)
    {
        var json = JsonConvert.SerializeObject(listScriptDif, Newtonsoft.Json.Formatting.Indented);
        System.IO.File.WriteAllText($"script_log_{Guid.NewGuid()}.json", json);
    }
}