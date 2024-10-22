using OneLonDataMigration.Db;
using OneLonDataMigration.File;
using OneLonDataMigration.Models;

namespace OneLonDataMigration;

public class ScriptExecutor(IScriptManager scriptManager,IScriptLogger scriptLogger)
{
    public void RunAllScripts()
    {
        var listScriptDif = scriptManager.GetDiffScripts();
        foreach (var script in listScriptDif)
        {
            Console.WriteLine(script.FullName);
        }
        if (listScriptDif.Count == 0)
        {
            Console.WriteLine("No new script to execute");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Execute all new script (Y/N) ?");
        Console.ResetColor();
        var key = Console.ReadKey();
        var listScriptExecuted = new List<ScriptData>();
        if (key.Key == ConsoleKey.Y)
        {
            Console.WriteLine();
            listScriptExecuted = scriptManager.ExecuteAllScripts(listScriptDif);
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
            
        }
        else
        {
            Console.WriteLine("Abort executing all new script");
        }
        
        scriptLogger.LogScript(listScriptExecuted);
        scriptLogger.WriteScriptChanges(listScriptExecuted);
        //scriptLogger.ZipScriptChanges(listScriptExecuted);
    }
}