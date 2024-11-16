using OneLonDataMigration.Db;
using OneLonDataMigration.File;

namespace OneLonDataMigration;

public class Program
{
    private static void Main(string[] args)
    {
        var isThrowError = false;
        ISpinner spinner = new Spinner();
        try
        {
            DisplayHeader();

            if (args.Length > 0 && args[0].ToLower() == "help")
            {
                DisplayHelp();
                return;
            }

            var isCheck = args.Length > 0 && args[0].ToLower() == "check";
            isThrowError = Array.Exists(args, arg => arg.ToLower() == "--throw-error");
            var isForce = Array.Exists(args, arg => arg.ToLower() == "--force");

            var config = new ConfigReader().ReadConfigFromJson();
            DisplayEnvironmentInfo(config);

            IFileFolder fileFolder = new FileFolder();
            IScriptFileManager scriptFileManager = new ScriptFileManager(config, fileFolder);
            IDbClient dbClient = new DbClientPostgres(config);
            IScriptManager scriptManager = new ScriptManager(dbClient, scriptFileManager);
            IScriptLogger scriptLogger = new ScriptFileLogger(config, fileFolder);


            if (isCheck)
            {
                DisplayActionBanner("Checking Scripts");
                var scriptChecker = new ScriptChecker(scriptManager, spinner);
                scriptChecker.CheckAllScripts();
            }
            else
            {
                DisplayActionBanner("Executing Scripts");
                var scriptExecutor = new ScriptExecutor(scriptManager, scriptLogger, spinner);
                scriptExecutor.RunAllScripts(isForce);
            }

            DisplaySuccess("Operation completed successfully!");
        }
        catch (Exception e)
        {
            DisplayError(e.Message);
        }
        finally
        {
            spinner.Stop();
        }

        if (!isThrowError)
        {
            DisplayFooter();
            Console.ReadKey();
        }
    }

    private static void DisplayHeader()
    {
        Console.Clear();
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(@"
   ____              __                        
  / __ \____  ___   / /   ____  ____ ___  
 / / / / __ \/ _ \ / /   / __ \/    `__ \
/ /_/ / / / /  __// /___/ /_/ / __  / / /  
\____/_/ /_/\___//_____/\____/_/ /_/ /_/
                                              
        Data Migration Tool");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void DisplayHelp()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("╔════════════════════════════════════════╗");
        Console.WriteLine("║             Available Options          ║");
        Console.WriteLine("╠════════════════════════════════════════╣");
        Console.WriteLine("║  help         : Show this help         ║");
        Console.WriteLine("║  check        : Check all scripts      ║");
        Console.WriteLine("║  default      : Run all scripts        ║");
        Console.WriteLine("║  --throw-error: Throw error on fail    ║");
        Console.WriteLine("║  --force      : Skip confirmation      ║");
        Console.WriteLine("╚════════════════════════════════════════╝");
        Console.ResetColor();
    }

    private static void DisplayEnvironmentInfo(Config config)
    {
        var env = config.ConnectionString.Split(";")
            .Where(x => !x.ToLower().Contains("password"))
            .Aggregate((x, y) => x + ";" + y);
        
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("┌────────────────────────────────────────────────┐");
        Console.WriteLine($"│ {config.GreetingTitle,-46} │");
        Console.WriteLine($"│ Version: {typeof(Program).Assembly.GetName().Version,-37} │");
        Console.WriteLine($"│ {config.GreetingMessage} {new string(' ', 20)} │");
        Console.WriteLine("└────────────────────────────────────────────────┘");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"Connection: {env}");
        Console.ResetColor();
        Console.WriteLine();
    }

    private static void DisplayActionBanner(string action)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"▶ {action}");
        Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Console.ResetColor();
    }

    private static void DisplaySuccess(string message)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✔ {message}");
        Console.ResetColor();
    }

    private static void DisplayError(string message)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"╔{new string('═', message.Length + 4)}╗");
        Console.WriteLine($"║  {message}  ║");
        Console.WriteLine($"╚{new string('═', message.Length + 4)}╝");
        Console.ResetColor();
    }

    private static void DisplayFooter()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("────────────────────────────────────────────────");
        Console.WriteLine("Press any key to exit");
        Console.ResetColor();
    }
}