using System;
using Microsoft.Extensions.Logging;
using V2exSharp;

namespace Microsoft.Extensions.DependencyInjection;

public static class V2exSharpBuilderExtensions
{
    public static IServiceCollection AddV2exSharp(
        this IServiceCollection serviceCollection,
        Action<V2exSharpOptions> configuration)
    {
        serviceCollection.AddHttpClient<IV2exApiClient, V2exApiClient>((client,sp) =>
        {
            var logger = sp.GetService<ILogger<V2exApiClient>>();
            return new V2exApiClient(configuration, client, logger);
        });
        return serviceCollection;
    }
}