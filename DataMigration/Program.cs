﻿// See https://aka.ms/new-console-template for more information

using OneLonDataMigration;
using OneLonDataMigration.Db;
using OneLonDataMigration.File;

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


    IScriptFileManager scriptFileManager = new ScriptFileManager(config);
    IDbClient dbClient= new DbClientPostgres(config);

    IScriptManager scriptManager = new ScriptManager(dbClient, scriptFileManager);
    IScriptLogger scriptLogger = new ScriptFileLogger(config);    

    var scriptExecutor = new ScriptExecutor(scriptManager, scriptLogger);

    scriptExecutor.RunAllScripts();
}
catch (Exception e)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(e.Message);
    Console.ResetColor();
}

Console.WriteLine("====================================");
Console.WriteLine("Press any key to exit");
Console.ReadKey();

