using Moq;
using OneLonDataMigration;
using OneLonDataMigration.File;

namespace DataMigrationTest;

public class ScriptFileManagerTests
{
    [Fact]
    public void GetFileScripts_ReturnsCorrectScripts()
    {
        var config = new Config { PathScripts = "path/to/scripts", MinimumOrderNumber = 1 };
        var fileFolder = new Mock<IFileFolder>();
        fileFolder.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new[] { "1. ScriptName V1.0.sql" });
        fileFolder.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("ScriptContent");
        fileFolder.Setup(x => x.GetFileName(It.IsAny<string>())).Returns("1. ScriptName V1.0.sql");
        var scriptFileManager = new ScriptFileManager(config, fileFolder.Object);

        var result = scriptFileManager.GetFileScripts(0);

        Assert.NotEmpty(result);
        Assert.Equal("ScriptName", result[0].ScriptName);
    }
    
    [Fact]
    public void GetFileScripts_3Digit_ReturnsCorrectScripts()
    {
        var config = new Config { PathScripts = "path/to/scripts", MinimumOrderNumber = 1 };
        var fileFolder = new Mock<IFileFolder>();
        fileFolder.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new[] { "1. ScriptName V1.2.1sql" });
        fileFolder.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("ScriptContent");
        fileFolder.Setup(x => x.GetFileName(It.IsAny<string>())).Returns("1. ScriptName V1.2.1.sql");
        var scriptFileManager = new ScriptFileManager(config, fileFolder.Object);

        var result = scriptFileManager.GetFileScripts(0);

        Assert.NotEmpty(result);
        Assert.Equal("ScriptName", result[0].ScriptName);
    }

    [Fact]
    public void GetFileScripts_ThrowsExceptionForInvalidFileFormat()
    {
        var config = new Config { PathScripts = "path/to/invalid/scripts", MinimumOrderNumber = 1 };
        var fileFolder = new Mock<IFileFolder>();
        var scriptFileManager = new ScriptFileManager(config, fileFolder.Object);
        
        fileFolder.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new[] { "1. Script Name V1.0.sql" });
        fileFolder.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("ScriptContent");
        fileFolder.Setup(x => x.GetFileName(It.IsAny<string>())).Returns("1. Script Name V1.0.sql");

        Assert.Throws<Exception>(() => scriptFileManager.GetFileScripts(0));
        var ex = Assert.Throws<Exception>(() => scriptFileManager.GetFileScripts(0));
        Assert.Equal("File 1. Script Name V1.0.sql have invalid format", ex.Message);
    }

    [Fact]
    public void GetFileScripts_SkipsScriptsWithOrderNumberLessThanMinimum()
    {
        var config = new Config { PathScripts = "path/to/scripts", MinimumOrderNumber = 10 };
        var fileFolder = new Mock<IFileFolder>();
        var scriptFileManager = new ScriptFileManager(config, fileFolder.Object);
        fileFolder.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(["1. ScriptName V1.0.sql", "2. ScriptName2 V1.0.sql"]);
        fileFolder.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("ScriptContent");
        fileFolder.Setup(x => x.GetFileName("1. ScriptName V1.0.sql")).Returns("1. ScriptName V1.0.sql");
        fileFolder.Setup(x => x.GetFileName("2. ScriptName2 V1.0.sql")).Returns("2. ScriptName2 V1.0.sql");

        var result = scriptFileManager.GetFileScripts(0);

        Assert.Empty(result);
    }
    
    [Fact]
    public void GetFileScripts_SkipsScriptsWithOrderNumberLessThanMinimum2()
    {
        var config = new Config { PathScripts = "path/to/scripts", MinimumOrderNumber = 2 };
        var fileFolder = new Mock<IFileFolder>();
        var scriptFileManager = new ScriptFileManager(config, fileFolder.Object);
        fileFolder.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(["1. ScriptName V1.0.sql", "2. ScriptName2 V1.0.sql"]);
        fileFolder.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("ScriptContent");
        fileFolder.Setup(x => x.GetFileName("1. ScriptName V1.0.sql")).Returns("1. ScriptName V1.0.sql");
        fileFolder.Setup(x => x.GetFileName("2. ScriptName2 V1.0.sql")).Returns("2. ScriptName2 V1.0.sql");

        var result = scriptFileManager.GetFileScripts(1);

        Assert.Single(result);
        Assert.Equal("ScriptName2", result[0].ScriptName);
        Assert.Equal(2, result[0].FileOrderNumber);
    }

    [Fact]
    public void GetFileScripts_OrdersScriptsByFileOrderNumber()
    {
        var config = new Config { PathScripts = "path/to/scripts", MinimumOrderNumber = 1 };
        var fileFolder = new Mock<IFileFolder>();
        var scriptFileManager = new ScriptFileManager(config, fileFolder.Object);
        fileFolder.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(["1. ScriptName V1.0.sql", "2. ScriptName2 V1.0.sql"]);
        fileFolder.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns("ScriptContent");
        fileFolder.Setup(x => x.GetFileName("1. ScriptName V1.0.sql")).Returns("1. ScriptName V1.0.sql");
        fileFolder.Setup(x => x.GetFileName("2. ScriptName2 V1.0.sql")).Returns("2. ScriptName2 V1.0.sql");

        var result = scriptFileManager.GetFileScripts(0);

        Assert.True(result[0].FileOrderNumber < result[1].FileOrderNumber);
    }
}