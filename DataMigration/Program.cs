// See https://aka.ms/new-console-template for more information

using OneLonDataMigration;
using OneLonDataMigration.Db;
using OneLonDataMigration.File;

var isThrowError = false;
try
{
    // check argument for help
    
    if (args.Length > 0 && args[0].ToLower() == "help")
    {
        Console.WriteLine("OneLoan Data Migration Tool");
        Console.WriteLine("Usage: DataMigration [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  help : Show this help");
        Console.WriteLine("  check : Check all scripts");
        Console.WriteLine("  default : Run all scripts");
        Console.WriteLine("  --throw-error : Throw error when error occurs");
        Console.WriteLine("  --force : no need confirm");
        return;
    }
    
    var isCheck = args.Length > 0 && args[0].ToLower() == "check";
    
    isThrowError = Array.Exists(args, arg => arg.ToLower() == "--throw-error");
    
    var isForce = Array.Exists(args, arg => arg.ToLower() == "--force");
    
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


    IFileFolder fileFolder = new FileFolder();
    IScriptFileManager scriptFileManager = new ScriptFileManager(config, fileFolder);
    IDbClient dbClient= new DbClientPostgres(config);

    IScriptManager scriptManager = new ScriptManager(dbClient, scriptFileManager);
    IScriptLogger scriptLogger = new ScriptFileLogger(config, fileFolder);    

    var scriptExecutor = new ScriptExecutor(scriptManager, scriptLogger);

    if (isCheck)
    {
        var scriptChecker = new ScriptChecker(scriptManager, scriptLogger);
        scriptChecker.CheckAllScripts();
    }
    else
    {
        scriptExecutor.RunAllScripts(isForce);
    }
    
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(e.Message);
    Console.ResetColor();
}

if (!isThrowError)
{
    Console.WriteLine("====================================");
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();

}

