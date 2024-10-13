using System.Text.RegularExpressions;
using OneLonDataMigration.Models;

namespace OneLonDataMigration.File;

public class ScriptFileManager(Config config) : IScriptFileManager
{
    public List<FileScript> GetFileScripts()
    {
        var listFileScripts = new List<FileScript>();
        var files = Directory.GetFiles(config.PathScripts, "*.sql");
        // validate every file have valid format
        // format : 1. name version.sql
        // example : 1. create_table_user V1.2.sql
        // example : 2. create_table_role V1.2.sql
        // example : 3. create_table V1.2.sql
        // example : 4. create V1.2.sql
        // example : 5. create_1 V1.2.sql
        // example : 6. select_2 V1.2.sql
        // example : 7. select_3 V1.sql
        // example : 8. select_4 V1.sql
        
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
            var fileScript = new FileScript
            {
                ScriptName = Path.GetFileName(file).Split(' ')[1].Trim(),
                ScriptFileName = Path.GetFileName(file),
                ScriptVersion = Path.GetFileName(file).Split(' ')[2].Replace(".sql", "").Trim(),
                ScriptContent = System.IO.File.ReadAllText(file)
            };
            fileScript.OrderNumber = int.Parse(fileScript.ScriptFileName.Split(' ')[0].Replace('.', ' ').Trim());
            listFileScripts.Add(fileScript);
        }
        return listFileScripts.OrderBy(x => x.OrderNumber).ToList();
    }
}