using V2exSharp.Client;
using V2exSharp.Option;

namespace Microsoft.Extensions.DependencyInjection;

public static class V2exSharpBuilderExtensions
{
    public static IServiceCollection AddV2exSharp(this IServiceCollection serviceCollection, V2exSharpOption option)
    {
        serviceCollection.AddSingleton<IV2exApiClient, V2exApiClient>(sp => new V2exApiClient(option));
        return serviceCollection;
    }
}