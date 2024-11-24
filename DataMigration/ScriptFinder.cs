using OneLonDataMigration.Db;

namespace OneLonDataMigration;

public class ScriptFinder(IScriptManager scriptManager, ISpinner spinner)
{
    public void FindScript()
    {
        Console.Write("File to find: ");
        var file = Console.ReadLine();
        if (string.IsNullOrEmpty(file))
        {
            throw new Exception("File is required.");
        }
        
        spinner.Start();
        var scripts = scriptManager.FindScript(file);
        if (scripts.Count == 0)
        {
            Console.WriteLine("No script found.");
        }
        else
        {
            foreach (var script in scripts)
            {
                Console.WriteLine(script.FullName);
            }
        }
        spinner.Stop();
    }
}