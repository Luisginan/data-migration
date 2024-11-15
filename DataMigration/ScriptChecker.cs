using OneLonDataMigration.Db;
using OneLonDataMigration.File;

namespace OneLonDataMigration;

public class ScriptChecker(IScriptManager scriptManager, ISpinner spinner)
{
    public void CheckAllScripts()
    {
        spinner.Start();
        var listScriptDif = scriptManager.GetDiffScripts();
        if (listScriptDif.Count == 0)
        {
            Console.WriteLine("No scripts to check.");
            spinner.Stop();
            return;
        }

        foreach (var script in listScriptDif)
        {
            Console.WriteLine(script.FullName);
        }
        spinner.Stop();
    }
}