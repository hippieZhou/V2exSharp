using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using V2exSharp.Helpers;
using V2exSharp.Options;

namespace V2exSharp.Managers;

public partial class NetworkProxyManager(IOptions<V2ExApiClientOption> options, ILogger<NetworkProxyManager> logger)
{
    private const string NetworkProxySettingsKey = "network-proxy-settings";

    public Task<NetworkProxySettings?> GetAsync()
    {
        return Task.FromResult(Get(NetworkProxySettingsKey, new NetworkProxySettings()));
    }

    public Task SaveAsync(NetworkProxySettings settings)
    {
        Set(NetworkProxySettingsKey, settings);
        return Task.CompletedTask;
    }

    [UnsupportedOSPlatform("browser")]
    public async Task<bool> TestAsync(NetworkProxySettings settings)
    {
        try
        {
            var proxyUrl = settings.GetProxyUrl();
            var httpClient = proxyUrl == null
                ? new HttpClient()
                : new HttpClient(new HttpClientHandler
                {
                    Proxy = new WebProxy(proxyUrl)
                });
            var response = await httpClient.GetAsync("http://www.v2ex.com");
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch (Exception)
        {
            return false;
        }
    }
}

public partial class NetworkProxyManager
{
    private string FilePath(string key) => Path.Combine(options.Value.LocalStoragePath, key);

    private T? Get<T>(string key, T? defaultValue)
    {
        var filePath = FilePath(key);
        return FileStorageHelper.Get(filePath, defaultValue, logger);
    }

    private void Set<T>(string key, T value)
    {
        var filePath = FilePath(key);
        FileStorageHelper.Set(filePath, value, logger);
    }
}