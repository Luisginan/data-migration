namespace OneLonDataMigration;

public class ScriptData
{
    public Int32 OrderNumber { get; set; }
    public string ScriptName { get; set; }
    public string ScriptContent { get; set; }
    public bool isErrored { get; set; }
    public string ErrorMessage { get; set; }
    public string FullName { get; set; }
    public bool IsExecuted { get; set; }
}