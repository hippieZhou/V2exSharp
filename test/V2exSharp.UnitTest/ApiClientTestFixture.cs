using Microsoft.Extensions.DependencyInjection;
using V2exSharp.Client;
using V2exSharp.Option;

namespace V2exSharp.UnitTest;

public class ApiClientTestFixture
{
    [Fact]
    public void should_get_api_client_failed_when_not_register_into_ioc()
    {
        var serviceCollection = new ServiceCollection();
        var container = serviceCollection.BuildServiceProvider();
        var instance = container.GetService<IV2exApiClient>();
        Assert.Null(instance);
    }

    [Fact]
    public void should_get_api_client_success_when_register_into_ioc()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddV2exSharp(new V2exSharpOption()
        {
            AccessToken = "hello world"
        });
        var container = serviceCollection.BuildServiceProvider();
        var instance = container.GetService<IV2exApiClient>();
        Assert.NotNull(instance);
    }

    [Fact]
    public void should_throw_exception_when_access_token_is_null_or_empty()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddV2exSharp(new V2exSharpOption());
        var container = serviceCollection.BuildServiceProvider();
        var action = () => container.GetService<IV2exApiClient>();
        Assert.Throws<ArgumentNullException>(action);
    }
}