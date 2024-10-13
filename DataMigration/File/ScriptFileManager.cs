using System.Text.RegularExpressions;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public class ScriptFileManager(Config config) : IScriptFileManager
{
    public List<FileScript> GetFileScripts(int lastOrderNumber)
    {
        var listFileScripts = new List<FileScript>();
        var files = Directory.GetFiles(config.PathScripts, "*.sql");

        var iterationOrderNumber = lastOrderNumber;
        
        var regex = new Regex(@"^\d+\.\s\w+\sV\d+\.\d+\.sql$");
        
        foreach (var file in files)
        {
            if (!regex.IsMatch(Path.GetFileName(file)))
            {
                throw new Exception($"File {file} have invalid format");
            }
        }   
        foreach (var file in files)
        {
            var orderNumber = int.Parse(Path.GetFileName(file).Split(' ')[0].Replace('.', ' ').Trim());
            if (orderNumber < config.MinimumOrderNumber)
            {
                Console.WriteLine($"Script {file} have order number less than minimum order number. Skipped");
                continue;
            }
            var fileScript = new FileScript
            {
                ScriptName = Path.GetFileName(file).Split(' ')[1].Trim(),
                ScriptFileName = Path.GetFileName(file),
                ScriptVersion = Path.GetFileName(file).Split(' ')[2].Replace(".sql", "").Trim(),
                ScriptContent = System.IO.File.ReadAllText(file)
            };
            iterationOrderNumber++;
            fileScript.OrderNumber = iterationOrderNumber;
            listFileScripts.Add(fileScript);
        }
        return listFileScripts.OrderBy(x => x.OrderNumber).ToList();
    }
}