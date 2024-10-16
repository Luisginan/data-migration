namespace OneLonDataMigration.Models;

public class FileScript
{
    public string ScriptName { get; set; }    
    public string ScriptContent { get; set; }
    public int OrderNumber { get; set; }
    public int FileOrderNumber { get; set; }
    public string ScriptFileName { get; set; }
    public string ScriptVersion { get; set; }
}