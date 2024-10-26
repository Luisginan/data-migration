using Moq;
using OneLonDataMigration;
using OneLonDataMigration.File;
using OneLonDataMigration.Models;

namespace DataMigrationTest;

public class ScriptFileLoggerTests
{
    [Fact]
    public void LogScript_CreatesCorrectLogFile()
    {
        var config = new Config();
        var fileFolder = new Mock<IFileFolder>();
        var scriptFileLogger = new ScriptFileLogger(config, fileFolder.Object);

        var scripts = new List<ScriptData>
        {
            new() { ScriptName = "Script1", ScriptContent = "Content1", FullName = "Script1.sql" }
        };

        scriptFileLogger.LogScript(scripts);

        fileFolder.Verify(x => x.WriteAllText(It.Is<string>(s => s.StartsWith("script_log_") && s.EndsWith(".json")), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void WriteScriptChanges_CreatesCorrectChangesFile()
    {
        var config = new Config();
        var fileFolder = new Mock<IFileFolder>();
        var scriptFileLogger = new ScriptFileLogger(config, fileFolder.Object);

        var scripts = new List<ScriptData>
        {
            new() { ScriptName = "Script1", ScriptContent = "Content1", FullName = "Script1.sql", FileOrderNumber = 1 },
            new() { ScriptName = "Script2", ScriptContent = "Content2", FullName = "Script2.sql", FileOrderNumber = 2 }
        };

        scriptFileLogger.WriteScriptChanges(scripts);

        fileFolder.Verify(x => x.WriteAllText(It.Is<string>(s => s == "script_changes_1-2.txt"), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void WriteScriptChanges_NoScriptsToWrite()
    {
        var config = new Config();
        var fileFolder = new Mock<IFileFolder>();
        var scriptFileLogger = new ScriptFileLogger(config, fileFolder.Object);

        var scripts = new List<ScriptData>();

        scriptFileLogger.WriteScriptChanges(scripts);

        fileFolder.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}