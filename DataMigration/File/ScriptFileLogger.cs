
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public class ScriptFileLogger(Config config) : IScriptLogger
{
    public void LogScript(List<ScriptData> listScriptDif)
    {
        var json = JsonConvert.SerializeObject(listScriptDif, Newtonsoft.Json.Formatting.Indented);
        // file name format script_log_202410180748.json
        var fileName = DateTime.Now.ToString("yyyyMMdd_HHmm");
        System.IO.File.WriteAllText($"script_log_{fileName}.json", json);
    }

    public void ZipScriptChanges(List<ScriptData> listScriptDif)
    {
        //zip file from listScriptDif
        // file name format script 1.1 2024-10-18.1.zip
        var firstScript = listScriptDif.FirstOrDefault();
        var lastScript = listScriptDif.LastOrDefault();
        // compress file
        if (firstScript != null && lastScript != null)
        {
            var zipFileName = $"script {firstScript.FileOrderNumber}-{lastScript.FileOrderNumber}.zip";
            using var archive = ZipFile.Open(zipFileName, ZipArchiveMode.Create);
            foreach (var scriptData in listScriptDif)
            {
                archive.CreateEntryFromFile(config.PathScripts + "/" + scriptData.FullName, scriptData.FullName);
            }
        }
        else
        {
            Console.WriteLine("No new script to zip");
        }
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