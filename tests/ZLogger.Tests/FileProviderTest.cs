using Shouldly;
using Microsoft.Extensions.Logging;
using System.IO;

namespace ZLogger.Tests;

public class FileProviderTest
{
    [Fact]
    public void CreateAppend()
    {
        const string Path = "ZLoggerTest.log";
        if (File.Exists(Path)) File.Delete(Path);
        {
            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerFile(Path);
            });

            var logger = loggerFactory.CreateLogger("mytest");

            logger.LogDebug("foo");
            logger.LogDebug("bar");
            logger.LogDebug("baz");

            loggerFactory.Dispose();

            using (var fs = File.OpenText(Path))
            {
                fs.ReadLine().ShouldBe("foo");
                fs.ReadLine().ShouldBe("bar");
                fs.ReadLine().ShouldBe("baz");
                fs.ReadLine().ShouldBeNull();
            }
        }

        {
            // append check

            var loggerFactory = LoggerFactory.Create(x =>
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddZLoggerFile(Path);
            });

            var logger = loggerFactory.CreateLogger("mytest");

            logger.LogDebug("abc");
            logger.LogDebug("def");
            logger.LogDebug("ghi");

            loggerFactory.Dispose();

            using (var fs = File.OpenText(Path))
            {
                fs.ReadLine().ShouldBe("foo");
                fs.ReadLine().ShouldBe("bar");
                fs.ReadLine().ShouldBe("baz");
                fs.ReadLine().ShouldBe("abc");
                fs.ReadLine().ShouldBe("def");
                fs.ReadLine().ShouldBe("ghi");
                fs.ReadLine().ShouldBeNull();
            }
        }
    }
}