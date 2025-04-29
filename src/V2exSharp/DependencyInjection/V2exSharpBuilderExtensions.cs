using System;
using Microsoft.Extensions.DependencyInjection;
using V2exSharp.Constants;
using V2exSharp.Handlers;
using V2exSharp.Managers;
using V2exSharp.Options;

namespace V2exSharp.DependencyInjection;

public static class V2ExSharpBuilderExtensions
{
    public static IServiceCollection AddV2ExSharp(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOptions<V2ExApiClientOption>();

        serviceCollection.AddSingleton<CookieContainerManager>();
        serviceCollection.AddSingleton<NetworkProxyManager>();

        serviceCollection.AddTransient<LoggingHandler>();
        serviceCollection
            .AddHttpClient<V2ExApiClient>(client =>
            {
                client.BaseAddress = new Uri(UrlUtilities.BASE_URL);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgentConstants.UserAgent);
            })
            .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<ApiHttpClientHandler>())
            .AddHttpMessageHandler<LoggingHandler>();
        ;
        return serviceCollection;
    }
}