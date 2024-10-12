using System.Text.Json;

namespace OneLonDataMigration;

public class ConfigReader
{
    public Config ReadConfigFromJson()
    {
        if (!File.Exists("config.json"))
        {
            var newFile = new Config
            {
                PathScripts = "C:\\Scripts",
                ConnectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;"
            };
            File.WriteAllText("config.json", JsonSerializer.Serialize(newFile));
            throw new Exception("File json not found, it will generated one for sample");
        }
        var json = File.ReadAllText("config.json");
        var config= JsonSerializer.Deserialize<Config>(json);
        if (config == null)
        {
            throw new Exception("Failed deserialize config");
        }

        return config;
    }
}