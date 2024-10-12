// See https://aka.ms/new-console-template for more information

using OneLonDataMigration;
using OneLonDataMigration.Db;

try
{
    var configReader = new ConfigReader();
    var config = configReader.ReadConfigFromJson();

    var env = config.ConnectionString.Split(";").Where(x => !x.ToLower().Contains("Password".ToLower())).Aggregate((x, y) => x + ";" + y);    
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(config.GreetingTitle);
    Console.WriteLine("Version " + typeof(Program).Assembly.GetName().Version); 
    Console.WriteLine("Developed by OneLoan Team");
    Console.WriteLine("Env : " + env);
    Console.WriteLine("====================================");
    Console.ResetColor();

    

    if (config == null)
    {
        Console.WriteLine("Config is null");
        return;
    }

    IScriptFileManager scriptFileManager = new ScriptFileManager(config);
    IDbClient dbClient= new DbClientPostgres(config);

    IScriptManager scriptManager = new ScriptManager(dbClient, scriptFileManager);
    IScriptLogger scriptLogger = new ScriptFileLogger();    

    var scriptExecutor = new ScriptExecutor(scriptManager, scriptLogger);

    scriptExecutor.RunAllScripts();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

Console.WriteLine("Press any key to exit");
Console.ReadKey();

