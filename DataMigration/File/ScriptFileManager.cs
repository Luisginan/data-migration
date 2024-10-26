using System.Text.RegularExpressions;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public partial class ScriptFileManager(Config config, IFileFolder fileFolder) : IScriptFileManager
{
    public List<FileScript> GetFileScripts(int lastOrderNumber)
    {
        var listFileScripts = new List<FileScript>();
        var files = fileFolder.GetFiles(config.PathScripts);

        var iterationOrderNumber = lastOrderNumber;
        
        var regex = MyRegex();
        
        foreach (var file in files)
        {
            if (!regex.IsMatch(fileFolder.GetFileName(file)))
                throw new Exception($"File {file} have invalid format");
        }   
        foreach (var file in files)
        {
            var orderNumber = int.Parse(fileFolder.GetFileName(file).Split(' ')[0].Replace('.', ' ').Trim());
            if (orderNumber < config.MinimumOrderNumber)
            {
                Console.WriteLine($"Script {file} have order number less than minimum order number. Skipped");
                continue;
            }
            var fileScript = new FileScript
            {
                ScriptName = fileFolder.GetFileName(file).Split(' ')[1].Trim(),
                ScriptFileName = fileFolder.GetFileName(file),
                ScriptVersion = fileFolder.GetFileName(file).Split(' ')[2].Replace(".sql", "").Trim(),
                ScriptContent = fileFolder.ReadAllText(file)
            };
            iterationOrderNumber++;
            fileScript.OrderNumber = iterationOrderNumber;
            fileScript.FileOrderNumber = orderNumber;
            listFileScripts.Add(fileScript);
        }
        return listFileScripts.OrderBy(x => x.FileOrderNumber).ToList();
    }

    [GeneratedRegex(@"^\d+\.\s\w+\sV\d+\.\d+\.sql$")]
    private static partial Regex MyRegex();
}