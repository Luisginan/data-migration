using Moq;
using OneLonDataMigration.Db;
using OneLonDataMigration.File;
using OneLonDataMigration.Models;

namespace DataMigrationTest;

public class ScriptManagerTests
{
    [Fact]
    public void GetDiffScripts_ReturnsCorrectDiffScripts()
    {
        var dbClientMock = new Mock<IDbClient>();
        var scriptFileManagerMock = new Mock<IScriptFileManager>();

        var historyScripts = new List<HistoryScript>
        {
            new() { ScriptName = "Script1", OrderNumber = 1, ScriptFileName = "Script1.sql" }
        };
        var fileScripts = new List<FileScript>
        {
            new() { ScriptName = "Script2", ScriptContent = "Content2", OrderNumber = 2, FileOrderNumber = 2, ScriptFileName = "Script2.sql", ScriptVersion = "1.0" }
        };

        dbClientMock.Setup(x => x.GetHistoryScripts()).Returns(historyScripts);
        scriptFileManagerMock.Setup(x => x.GetFileScripts(It.IsAny<int>())).Returns(fileScripts);

        var scriptManager = new ScriptManager(dbClientMock.Object, scriptFileManagerMock.Object);

        var result = scriptManager.GetDiffScripts();

        Assert.Single(result);
        Assert.Equal("Script2", result[0].ScriptName);
    }

    [Fact]
    public void GetDiffScripts_ThrowsExceptionForDifferentFileName()
    {
        var dbClientMock = new Mock<IDbClient>();
        var scriptFileManagerMock = new Mock<IScriptFileManager>();

        var historyScripts = new List<HistoryScript>
        {
            new() { ScriptName = "Script1", ScriptFileName = "Script1.sql", OrderNumber = 1 }
        };
        var fileScripts = new List<FileScript>
        {
            new() { ScriptName = "Script1", ScriptFileName = "Script1_v2.sql", ScriptContent = "Content1", OrderNumber = 1, FileOrderNumber = 1, ScriptVersion = "1.0" }
        };

        dbClientMock.Setup(x => x.GetHistoryScripts()).Returns(historyScripts);
        scriptFileManagerMock.Setup(x => x.GetFileScripts(It.IsAny<int>())).Returns(fileScripts);

        var scriptManager = new ScriptManager(dbClientMock.Object, scriptFileManagerMock.Object);

        Assert.Throws<Exception>(() => scriptManager.GetDiffScripts());
        var ex = Assert.Throws<Exception>(() => scriptManager.GetDiffScripts());
        Assert.Equal("Script Script1 exist in database but have different file name. file name:  Script1_v2.sql", ex.Message);
    }

    [Fact]
    public void ExecuteAllScripts_ExecutesScriptsSuccessfully()
    {
        var dbClientMock = new Mock<IDbClient>();
        var scriptFileManagerMock = new Mock<IScriptFileManager>();

        var scriptsToExecute = new List<ScriptData>
        {
            new() { ScriptName = "Script1", ScriptContent = "Content1", FullName = "Script1.sql" }
        };

        var scriptManager = new ScriptManager(dbClientMock.Object, scriptFileManagerMock.Object);

        var result = scriptManager.ExecuteAllScripts(scriptsToExecute);

        Assert.Single(result);
        Assert.False(result[0].isErrored);
        Assert.True(result[0].IsExecuted);
    }

    [Fact]
    public void ExecuteAllScripts_HandlesExecutionError()
    {
        var dbClientMock = new Mock<IDbClient>();
        var scriptFileManagerMock = new Mock<IScriptFileManager>();

        var scriptsToExecute = new List<ScriptData>
        {
            new() { ScriptName = "Script1", ScriptContent = "Content1", FullName = "Script1.sql" }
        };

        dbClientMock.Setup(x => x.ExecuteScript(It.IsAny<string>())).Throws(new Exception("Execution error"));

        var scriptManager = new ScriptManager(dbClientMock.Object, scriptFileManagerMock.Object);

        var result = scriptManager.ExecuteAllScripts(scriptsToExecute);

        Assert.Single(result);
        Assert.True(result[0].isErrored);
        Assert.Equal("Execution error", result[0].ErrorMessage);
        Assert.True(result[0].IsExecuted);
    }
}