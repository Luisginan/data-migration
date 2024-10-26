﻿using OneLonDataMigration.File;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.Db;

public class ScriptManager(IDbClient dbClient, IScriptFileManager scriptFileManager) : IScriptManager
{
    public List<ScriptData> GetDiffScripts()
    {
        var historyScripts = dbClient.GetHistoryScripts();
        var lastOrderNumber = historyScripts.Any() ? historyScripts.Max(x => x.OrderNumber) : 0;
        var fileScripts = scriptFileManager.GetFileScripts(lastOrderNumber);
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