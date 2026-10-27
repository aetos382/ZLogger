using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Shouldly;
using ZLogger.Providers;

namespace ZLogger.Tests;

public class BuilderTest
{
    [Fact]
    public void LogLevelConfiguration()
    {
        var configuration = new ConfigurationManager()
            .AddInMemoryCollection(new KeyValuePair<string, string>[]
            {
                new("Logging:LogLevel:Default", "Error")
            })
            .Build();
        
        using var loggerFactory = LoggerFactory.Create(x =>
        {
            x.AddConfiguration(configuration.GetSection("Logging"));
            x.AddZLoggerConsole();
        });

        var logger = loggerFactory.CreateLogger<BuilderTest>();

        logger.IsEnabled(LogLevel.Information).ShouldBeFalse();
        logger.IsEnabled(LogLevel.Warning).ShouldBeFalse();
        logger.IsEnabled(LogLevel.Error).ShouldBeTrue();
    }

    [Fact]
    public void LogLevelConfiguration_ProviderAlias()
    {
        var messages = new List<string>();
        
        var configuration = new ConfigurationManager()
            .AddInMemoryCollection(new KeyValuePair<string, string>[]
            {
                new("Logging:ZLoggerConsole:LogLevel:Default", "Warning"),
                new("Logging:ZLoggerInMemory:LogLevel:Default", "Error")
            })
            .Build();

        using var loggerFactory = LoggerFactory.Create(x => x
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddZLoggerConsole()
            .AddZLoggerInMemory(
                "Foo",
                (options, services) => { },
                processor =>
                {
                    processor.MessageReceived += msg => messages.Add(msg);
                }));

        var logger = loggerFactory.CreateLogger<BuilderTest>();

        logger.IsEnabled(LogLevel.Information).ShouldBeFalse();
        logger.IsEnabled(LogLevel.Warning).ShouldBeTrue();
        logger.IsEnabled(LogLevel.Error).ShouldBeTrue();
        
        logger.LogWarning($"hoge");
        messages.ShouldBeEmpty();
    }
}