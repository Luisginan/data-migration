using OneLonDataMigration.Db;
using OneLonDataMigration.File;

namespace OneLonDataMigration;

public class ScriptExtractor(IScriptManager scriptManager, IScriptFileManager scriptFileManager,ISpinner spinner)
{
    public void ExtractScripts()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
     
        Console.Write("Version to extract: ");
        var version = Console.ReadLine();
        if(string.IsNullOrEmpty(version))
        {
            throw new Exception("Version is required.");
        }
        
        Console.Write("Output folder: ");
        var outputFolder = Console.ReadLine();
        if (string.IsNullOrEmpty(outputFolder))
        {
            throw new Exception("Output folder is required.");
        }
        
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("Extract all scripts ? (Y/N): ");
        
        var extractAll = Console.ReadLine();
        Console.ResetColor();
        if (extractAll == null || !extractAll.Equals("Y", StringComparison.OrdinalIgnoreCase))
        {
           Console.WriteLine("Extracting canceled.");
        }
        else
        {
            var isFolderExists = scriptFileManager.IsFolderExists(outputFolder);
            if (!isFolderExists)
            {
                scriptFileManager.CreateFolder(outputFolder);
                Console.WriteLine($"Folder {outputFolder} created.");
            }
            Console.WriteLine("Extracting all scripts...");
            spinner.Start();
            var listScriptData = scriptManager.GetScriptFromDb(version);
            foreach (var scriptData in listScriptData)
            {
                scriptFileManager.SaveScript(scriptData, outputFolder);
                Console.WriteLine($"Script {scriptData.FullName} extracted.");
            }
            spinner.Stop();
        }
        
    }
}
