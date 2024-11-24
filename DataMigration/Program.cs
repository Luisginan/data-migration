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
            var config = new ConfigReader().ReadConfigFromJson();

            if (args.Length == 0)
            {
                ShowMenu(config);
                return;
            }

            if (args.Length > 0 && args[0].ToLower() == "help")
            {
                DisplayEnvironmentInfo(config);
                DisplayHelp();
                return;
            }

            var isCheck = args.Length > 0 && args[0].ToLower() == "check";
            var isExtract = args.Length > 0 && args[0].ToLower() == "extract";
            var isFind = args.Length > 0 && args[0].ToLower() == "find";
            var isRun = args.Length > 0 && args[0].ToLower() == "run";
            isThrowError = Array.Exists(args, arg => arg.ToLower() == "--throw-error");
            var isForce = Array.Exists(args, arg => arg.ToLower() == "--force");


            IFileFolder fileFolder = new FileFolder();
            IScriptFileManager scriptFileManager = new ScriptFileManager(config, fileFolder);
            IDbClient dbClient = new DbClientPostgres(config);
            IScriptManager scriptManager = new ScriptManager(dbClient, scriptFileManager);
            IScriptLogger scriptLogger = new ScriptFileLogger(config, fileFolder);

            if (isCheck)
            {
                DisplayEnvironmentInfo(config);
                DisplayActionBanner("Checking Scripts");
                var scriptChecker = new ScriptChecker(scriptManager, spinner);
                scriptChecker.CheckAllScripts();
            }
            else if (isExtract)
            {
                DisplayEnvironmentInfo(config);
                DisplayActionBanner("Extracting Scripts");
                var scriptExtractor = new ScriptExtractor(scriptManager, scriptFileManager, spinner);
                scriptExtractor.ExtractScripts();
            }
            else if (isFind)
            {
                DisplayEnvironmentInfo(config);
                DisplayActionBanner("Finding Script");
                var scriptFinder = new ScriptFinder(scriptManager, spinner);
                scriptFinder.FindScript();
            }
            else if (isRun)
            {
                DisplayEnvironmentInfo(config);
                DisplayActionBanner("Executing Scripts");
                var scriptExecutor = new ScriptExecutor(scriptManager, scriptLogger, spinner);
                scriptExecutor.RunAllScripts(isForce);
            }

            DisplaySuccess("Operation completed successfully!");
        }
        catch (Exception e)
        {
            DisplayError(e.Message);
            if (isThrowError)
            {
                throw;
            }
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

    private static void ShowMenu(Config config)
    {
        while (true)
        {
            Console.Clear();
            DisplayHeader();
            DisplayEnvironmentInfo(config);
            Console.WriteLine("Select an option:");
            Console.WriteLine("1. Run all scripts");
            Console.WriteLine("2. Check all scripts");
            Console.WriteLine("3. Extract all scripts");
            Console.WriteLine("4. Find a script by name");
            Console.WriteLine("5. Show help");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Main(["run"]);
                    break;
                case "2":
                    Main(["check"]);
                    break;
                case "3":
                    Main(["extract"]);
                    break;
                case "4":
                    Main(["find"]);
                    break;
                case "5":
                    Main(["help"]);
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

                    Console.WriteLine("Press any key to continue...");
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
        Console.WriteLine("║  extract      : Extract all scripts    ║");
        Console.WriteLine("║  find <name>  : Find a script by name  ║");
        Console.WriteLine("║  run          : Run all scripts        ║");
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
        Console.WriteLine("└───��────���────────────────────────────────────┘");
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
        Console.WriteLine("───────────────────────────────────────────────");
        Console.WriteLine("Press any key to exit");
        Console.ResetColor();
    }
}