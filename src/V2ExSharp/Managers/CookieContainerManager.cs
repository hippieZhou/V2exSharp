using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using V2exSharp.Helpers;
using V2exSharp.Models;
using V2exSharp.Options;

namespace V2exSharp.Managers;

public partial class CookieContainerManager
{
    private const string CookiesFileName = "cookies.json";
    private const string UserKey = "user.json";
    public CookieContainer Container { get; } = new();
    
    public UserInfo? User { get; private set; }

    private readonly IOptions<V2ExApiClientOption> _options;
    private readonly ILogger<CookieContainerManager> _logger;

    public CookieContainerManager(IOptions<V2ExApiClientOption> options, ILogger<CookieContainerManager> logger)
    {
        _options = options;
        _logger = logger;
        Initialize();
    }

    private void Initialize()
    {
        try
        {
            var cookies = Get(CookiesFileName, Array.Empty<Cookie>());
            foreach (var cookie in cookies)
            {
                Container.Add(cookie);
            }

            User = Get<UserInfo>(UserKey, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }

    public void Logout()
    {
        foreach (Cookie cookie in Container.GetAllCookies())
        {
            cookie.Expires = DateTime.Now.AddDays(-1);
        }
        User = null;
        
        Set(CookiesFileName, Array.Empty<Cookie>());
        Set<UserInfo>(UserKey, null);
    }

    public void Login(UserInfo userInfo)
    {
        User = userInfo;
        var cookies = Container.GetAllCookies()
            .Cast<Cookie>()
            .Select(x => new { x.Name, x.Value, x.Domain, x.Path, x.Expires, x.Secure, x.HttpOnly })
            .ToArray();
        
        Set(CookiesFileName, cookies);
        Set<UserInfo>(UserKey, userInfo);
    }
}

public partial class CookieContainerManager
{
    private string FilePath(string key) => Path.Combine(_options.Value.LocalStoragePath, key);

    private T? Get<T>(string key, T? defaultValue)
    {
        var filePath = FilePath(key);
        return FileStorageHelper.Get(filePath, defaultValue, _logger);
    }

    private void Set<T>(string key, T value)
    {
        var filePath = FilePath(key);
        FileStorageHelper.Set(filePath, value, _logger);
    }
}