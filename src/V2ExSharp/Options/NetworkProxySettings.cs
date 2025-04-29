using System;

namespace V2exSharp.Options;

public enum ProxyType
{
    None,
    Socks5,
    Http
}

public class NetworkProxySettings
{
    public ProxyType ProxyType { get; set; } = ProxyType.None;
    public string Hostname { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 1080;
    public string? Username { get; set; }
    public string? Password { get; set; }

    public string? GetProxyUrl()
    {
        return ProxyType switch
        {
            ProxyType.None => null,
            ProxyType.Socks5 => string.IsNullOrEmpty(Username)
                ? $"socks5://{Hostname}:{Port}"
                : $"socks5://{Username}:{Password}@:{Hostname}:{Port}",
            ProxyType.Http => string.IsNullOrEmpty(Username)
                ? $"http://{Hostname}:{Port}"
                : $"http://{Username}:{Password}@:{Hostname}:{Port}",
            _ => throw new NotImplementedException()
        };
    }
}
