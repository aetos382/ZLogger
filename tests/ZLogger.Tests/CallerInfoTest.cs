using System.IO;

namespace ZLogger.Tests;

public class LogCallerInfoTest
{
    [Fact]
    public void CallerInfo()
    {
        var processor = new TestProcessor(new ZLoggerOptions());

        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.CaptureThreadInfo = true;
                return processor;
            });
        });
        var logger = loggerFactory.CreateLogger("test");

        logger.ZLogInformation($"aaaa");

        var x = processor.Entries.Dequeue();
        x.LogInfo.MemberName.ShouldBe("CallerInfo");
        Path.GetFileName(x.LogInfo.FilePath).ShouldBe("CallerInfoTest.cs");
        x.LogInfo.LineNumber.ShouldBe(23);

        var currentThread = System.Threading.Thread.CurrentThread;

        x.LogInfo.ThreadInfo.ThreadId.ShouldNotBe(-1);
        x.LogInfo.ThreadInfo.ThreadId.ShouldBe(currentThread.ManagedThreadId);
        x.LogInfo.ThreadInfo.IsThreadPoolThread.ShouldBe(currentThread.IsThreadPoolThread);
        x.LogInfo.ThreadInfo.ThreadName.ShouldBe(currentThread.Name);
    }
}

