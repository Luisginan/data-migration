using OneLonDataMigration.File;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.Db;

public class ScriptManager(IDbClient dbClient, IScriptFileManager scriptFileManager) : IScriptManager
{
    public List<ScriptData> GetScriptFromDb(string version)
    {
        var historyScripts = dbClient.GetHistoryScriptsWithContent(version);
        var listScriptData = new List<ScriptData>();
        foreach (var historyScript in historyScripts)
        {
            listScriptData.Add(new ScriptData
            {
                ScriptName = historyScript.ScriptName,
                FullName = historyScript.ScriptFileName,
                OrderNumber = historyScript.OrderNumber,
                ScriptContent = historyScript.ScriptContent,
                Version = version
            });
        }

        return listScriptData;
    }

    public List<ScriptData> FindScript(string file)
    {
        var historyScripts =dbClient.FindScript(file);
        var listScriptData = new List<ScriptData>();
        foreach (var historyScript in historyScripts)
        {
            listScriptData.Add(new ScriptData
            {
                ScriptName = historyScript.ScriptName,
                FullName = historyScript.ScriptFileName,
                OrderNumber = historyScript.OrderNumber,
                ScriptContent = historyScript.ScriptContent
            });
        }
        
        return listScriptData; 
    }

    public List<ScriptData> GetDiffScripts()
    {
        var historyScripts = dbClient.GetHistoryScripts();
        var lastOrderNumber = historyScripts.Any() ? historyScripts.Max(x => x.OrderNumber) : 0;
        var fileScripts = scriptFileManager.GetFileScripts(lastOrderNumber);
        ValidateFileScripts(fileScripts);
        
        var listScriptDif = new List<ScriptData>();
        foreach (var fileScript in fileScripts)
        {
            if (historyScripts.All(x => x.ScriptName != fileScript.ScriptName))
            {
                listScriptDif.Add(new ScriptData
                {
                    ScriptName = fileScript.ScriptName,
                    FullName =  fileScript.ScriptFileName,
                    ScriptContent = fileScript.ScriptContent,
                    OrderNumber = fileScript.OrderNumber,
                    Version = fileScript.ScriptVersion,
                    FileOrderNumber = fileScript.FileOrderNumber
                });
            }
            else
            {
                if (historyScripts.Any(x => x.ScriptName == fileScript.ScriptName && x.ScriptFileName != fileScript.ScriptFileName))
                {
                   throw new Exception($"Script {fileScript.ScriptName} exist in database but have different file name. file name:  {fileScript.ScriptFileName}");
                } 
                
            }
        }

        return listScriptDif;
    }

    private void ValidateFileScripts(List<FileScript> fileScripts)
    {
        // throw if have same name
        var listScriptName = fileScripts.Select(x => x.ScriptName).ToList();
        var duplicateScriptName = listScriptName.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
        if (duplicateScriptName.Any())
        {
            throw new Exception($"Duplicate script name: {string.Join(", ", duplicateScriptName)} file name: {string.Join(", ", fileScripts.Where(x => duplicateScriptName.Contains(x.ScriptName)).Select(x => x.ScriptFileName))}");
        }
    }

    public List<ScriptData> ExecuteAllScripts(List<ScriptData> listScriptDif)
    {
        var listScriptExecuted = new List<ScriptData>();
        foreach (var scriptData in listScriptDif)
        {
            try
            {
                dbClient.ExecuteScript(scriptData.ScriptContent);
                Console.WriteLine($"Script {scriptData.FullName} executed successfully");
                dbClient.InsertHistoryScript(scriptData);
                scriptData.isErrored = false;
                scriptData.ErrorMessage = string.Empty;
                scriptData.IsExecuted = true;
                listScriptExecuted.Add(scriptData);
            }
            catch (Exception e)
            {
                scriptData.isErrored = true;
                scriptData.ErrorMessage = e.Message;
                scriptData.IsExecuted = true;
                Console.ResetColor();
                listScriptExecuted.Add(scriptData);
                break;
            }
            
        }

        return listScriptExecuted;  
    }
}