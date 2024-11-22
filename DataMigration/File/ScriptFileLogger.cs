using System.Text;
using Newtonsoft.Json;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public class ScriptFileLogger(Config config, IFileFolder fileFolder) : IScriptLogger
{
    public void LogScript(List<ScriptData> listScriptDif)
    {
        var json = JsonConvert.SerializeObject(listScriptDif, Formatting.Indented);
        var fileName = DateTime.Now.ToString("yyyyMMdd_HHmm");
        if (!fileFolder.IsFolderExists("log"))
            fileFolder.CreateFolder("log");
        
        fileFolder.WriteAllText($"log/script_log_{fileName}.json", json);
    }

    
    public void WriteScriptChanges(List<ScriptData> listScriptDif)
    {
        var contentListFileString = new StringBuilder();
        foreach (var scriptData in listScriptDif)
        {
            contentListFileString.AppendLine($"{scriptData.FullName}");
        }
        
        if (!fileFolder.IsFolderExists("log"))
            fileFolder.CreateFolder("log");
        
        var firstScript = listScriptDif.FirstOrDefault();
        var lastScript = listScriptDif.LastOrDefault();
        if (firstScript != null && lastScript != null)
            fileFolder.WriteAllText($"log/script_changes_{firstScript.FileOrderNumber}-{lastScript.FileOrderNumber}.txt",
                contentListFileString.ToString());
        else
        {
            Console.WriteLine("No new script to write");
        }
    }
}