using System.IO;

namespace ZLogger.Tests;

public class BackgroundBufferFullModeTest
{
    [Fact]
    public void Wait()
    {
        var path = Path.GetTempFileName();
        
        if (File.Exists(path)) File.Delete(path);
        
        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerFile(path, options =>
            {
                options.BackgroundBufferCapacity = 1;
                options.FullMode = BackgroundBufferFullMode.Block;
            });
        });

        var logger = loggerFactory.CreateLogger("mytest");

        for (var i = 0; i < 10; i++)
        {
            logger.ZLogInformation($"log {i}");
        }
        loggerFactory.Dispose();

        using var fs = File.OpenText(path);
        for (var i = 0; i < 10; i++)
        {
            fs.ReadLine().ShouldBe($"log {i}");
        }
        fs.ReadLine().ShouldBeNull();
    }
    
    [Fact]
    public void Drop()
    {
        var path = Path.GetTempFileName();
        
        if (File.Exists(path)) File.Delete(path);
        
        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerFile(path, options =>
            {
                options.BackgroundBufferCapacity = 1;
                options.FullMode = BackgroundBufferFullMode.Drop;
            });
        });

        var logger = loggerFactory.CreateLogger("mytest");

        for (var i = 0; i < 10; i++)
        {
            logger.ZLogInformation($"log {i}");
        }
        loggerFactory.Dispose();

        using var fs = File.OpenText(path);
        fs.ReadLine().ShouldBe("log 0");

        var lastLine = fs.ReadLine(); 
        while (lastLine != null)
        {
            lastLine = fs.ReadLine();
        }
        lastLine.ShouldNotBe("log 9");
    }
}