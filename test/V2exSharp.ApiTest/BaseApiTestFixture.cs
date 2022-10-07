using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace V2exSharp.ApiTest;

public abstract class BaseApiTestFixture
{
    private readonly ServiceProvider _container;

    protected BaseApiTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _container = new ServiceCollection()
            .AddSingleton(configuration)
            .AddLogging(builder => builder.AddConsole())
            .AddV2exSharp(opt => { opt.AccessToken = configuration.GetSection("AccessToken").Value; })
            .BuildServiceProvider();
    }

    protected T GetService<T>() => _container.GetService<T>()!;
}