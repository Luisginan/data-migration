
using System.Text;
using Newtonsoft.Json;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public class ScriptFileLogger : IScriptLogger
{
    public void LogScript(List<ScriptData> listScriptDif)
    {
        var json = JsonConvert.SerializeObject(listScriptDif, Newtonsoft.Json.Formatting.Indented);
        // file name format script_log_202410180748.json
        var fileName = DateTime.Now.ToString("yyyyMMdd_HHmm");
        System.IO.File.WriteAllText($"script_log_{fileName}.json", json);
    }

    public void WriteScriptChanges(List<ScriptData> listScriptDif)
    {
        var contentListFileString = new StringBuilder();
        foreach (var scriptData in listScriptDif)
        {
            contentListFileString.AppendLine($"{scriptData.FullName}");
        }
        
        
        var firstScript = listScriptDif.FirstOrDefault();
        var lastScript = listScriptDif.LastOrDefault();
        if (firstScript != null && lastScript != null)
            System.IO.File.WriteAllText($"script_changes_{firstScript.FileOrderNumber}-{lastScript.FileOrderNumber}.txt",
                contentListFileString.ToString());
        else
        {
            Console.WriteLine("No new script to write");
        }
    }
}