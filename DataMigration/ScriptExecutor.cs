using OneLonDataMigration.Db;
using OneLonDataMigration.File;
using OneLonDataMigration.Models;

namespace OneLonDataMigration;

public class ScriptExecutor(IScriptManager scriptManager,IScriptLogger scriptLogger, ISpinner spinner)
{
    public void RunAllScripts(bool isForce = false)
    {
        spinner.Start();
        var listScriptDif = scriptManager.GetDiffScripts();
        spinner.Stop();
        foreach (var script in listScriptDif)
        {
            Console.WriteLine(script.FullName);
        }
        if (listScriptDif.Count == 0)
        {
            Console.WriteLine("No new script to execute");
            return;
        }

        var listScriptExecuted = new List<ScriptData>();
        if (!isForce)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Execute all new script (Y/N) ?");
            Console.ResetColor();
            var key = Console.ReadKey();
           
       
            if (key.Key == ConsoleKey.Y)
            {
                spinner.Start();
                listScriptExecuted = ScriptExecute(listScriptDif);
                spinner.Stop();
            }
            else
            {
                Console.WriteLine("Abort executing all new script");
            } 
            
        }
        else
        {
            Console.WriteLine("Force execute all new script");
            listScriptExecuted = ScriptExecute(listScriptDif);
        }
        
        scriptLogger.LogScript(listScriptExecuted);
        scriptLogger.WriteScriptChanges(listScriptExecuted);
        
    }

    private List<ScriptData> ScriptExecute(List<ScriptData> listScriptDif)
    {
        Console.WriteLine();
        var listScriptExecuted = scriptManager.ExecuteAllScripts(listScriptDif);
        if (listScriptExecuted.Any(x => x.isErrored))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Some script is errored");
            foreach (var scriptData in listScriptExecuted.Where(x => x.isErrored))
            {
                Console.WriteLine($"Script {scriptData.ScriptName} is errored with message {scriptData.ErrorMessage}");
            }
            Console.ResetColor();
        }
        else
        {
            Console.WriteLine("All script executed successfully");
        }

        return listScriptExecuted;
    }
}