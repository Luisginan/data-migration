using Newtonsoft.Json;

namespace OneLonDataMigration.File;

public class ConfigReader
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore
    };

    public Config ReadConfigFromJson()
    {
        if (!System.IO.File.Exists("config.json"))
        {
            var newFile = new Config
            {
                PathScripts = "Scripts",
                ConnectionString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=postgres;",
                MinimumOrderNumber = 1
            };
            System.IO.File.WriteAllText("config.json", JsonConvert.SerializeObject(newFile, _jsonSerializerSettings));
            throw new Exception("File json not found, it will generated one for sample");
        }
        var json = System.IO.File.ReadAllText("config.json");
        var config = JsonConvert.DeserializeObject<Config>(json, _jsonSerializerSettings);
        if (config == null)
            throw new Exception("Failed deserialize config");

        return config;
    }
}