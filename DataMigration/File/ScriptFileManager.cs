namespace OneLonDataMigration;

public class ScriptFileManager(Config config) : IScriptFileManager
{
    public List<FileScript> GetFileScripts()
    {
        var listFileScripts = new List<FileScript>();
        var files = Directory.GetFiles(config.PathScripts, "*.sql");
        foreach (var file in files)
        {
            var fileScript = new FileScript
            {
                FullName = file,
                ScriptName = Path.GetFileName(file),
                ScriptContent = File.ReadAllText(file)
            };
            fileScript.OrderNumber = int.Parse(fileScript.ScriptName.Split(' ')[0].Replace('.', ' ').Trim());
            fileScript.version = fileScript.ScriptName.Split(' ')[2].Replace(".sql", "").Trim();
            listFileScripts.Add(fileScript);
        }
        return listFileScripts.OrderBy(x => x.OrderNumber).ToList();
    }
}