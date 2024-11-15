namespace OneLonDataMigration;

public class Config
{
    public string GreetingTitle { get; set; } = "Oneloan Data Migration Tool";
    public string GreetingMessage { get; set; } = "Developed by OneLoan Team";
    public string PathScripts { get; set; } = "Scripts";
    public string ConnectionString { get; set; }
    public int MinimumOrderNumber { get; set; }
}