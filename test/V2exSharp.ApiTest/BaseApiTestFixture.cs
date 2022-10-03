using Microsoft.Extensions.DependencyInjection;

namespace V2exSharp.ApiTest;

public abstract class BaseApiTestFixture
{
    private readonly ServiceProvider _container;

    protected BaseApiTestFixture()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddV2exSharp(opt => { opt.AccessToken = "152e5a32-16c3-4851-ba88-43d717b7e012"; });
        _container = serviceCollection.BuildServiceProvider();
    }

    protected T GetService<T>() => _container.GetService<T>()!;
}