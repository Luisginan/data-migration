using OneLonDataMigration.Db;
using OneLonDataMigration.File;

namespace OneLonDataMigration;

public class ScriptChecker(IScriptManager scriptManager,IScriptLogger scriptLogger)
{
    public void CheckAllScripts()
    {
        var listScriptDif = scriptManager.GetDiffScripts();
        foreach (var script in listScriptDif)
        {
            Console.WriteLine(script.FullName);
        }
    }
}