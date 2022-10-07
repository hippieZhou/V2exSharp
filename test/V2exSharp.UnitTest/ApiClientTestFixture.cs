using Microsoft.Extensions.DependencyInjection;

namespace V2exSharp.UnitTest;

public class ApiClientTestFixture
{
    [Fact]
    public void should_get_api_client_failed_when_not_register_into_ioc()
    {
        var container = new ServiceCollection()
            .BuildServiceProvider();
        var instance = container.GetService<IV2exApiClient>();
        Assert.Null(instance);
    }

    [Fact]
    public void should_get_api_client_success_when_register_into_ioc()
    {
        var container = new ServiceCollection()
            .AddV2exSharp(opt => { opt.AccessToken = "152e5a32-16c3-4851-ba88-43d717b7e012"; })
            .BuildServiceProvider();
        var instance = container.GetService<IV2exApiClient>();
        Assert.NotNull(instance);
    }
}