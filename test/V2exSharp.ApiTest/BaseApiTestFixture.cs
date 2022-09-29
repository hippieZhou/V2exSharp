using Microsoft.Extensions.DependencyInjection;

namespace V2exSharp.ApiTest;

public abstract class BaseApiTestFixture
{
    public BaseApiTestFixture()
    {
        var serviceCollection = new ServiceCollection();
        var container = serviceCollection.BuildServiceProvider();
    }
}