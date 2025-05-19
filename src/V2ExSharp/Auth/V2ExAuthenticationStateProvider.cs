using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using V2exSharp.Managers;
using V2exSharp.Models;

namespace V2exSharp.Auth;

public class V2ExAuthenticationStateProvider(
    CookieContainerManager cookieContainerManager,
    ILogger<V2ExAuthenticationStateProvider> logger)
    :
        AuthenticationStateProvider, IAuthenticationStateProvider
{
    private readonly CookieContainerManager _cookieContainerManager = cookieContainerManager;
    private readonly ILogger<V2ExAuthenticationStateProvider> _logger = logger;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userInfo = _cookieContainerManager.User;
        var identity = new ClaimsIdentity();
        if (userInfo != null)
        {
            try
            {
                var notifications = userInfo.Notifications;
                var unreadMessageCount = 0;
                if (!string.IsNullOrEmpty(notifications))
                {
                    var splits = notifications.Split(" ");
                    if (splits.Length > 0 && int.TryParse(splits[1], out var count))
                    {
                        unreadMessageCount = count;
                    }
                }

                identity = new ClaimsIdentity([
                    new Claim("sub", userInfo?.Name ?? ""),
                    new Claim("avatar", userInfo?.Avatar ?? ""),
                    new Claim("notifications", unreadMessageCount.ToString()),
                    new Claim("moneyGold", userInfo?.MoneyGold?.Trim().ToString() ?? "0"),
                    new Claim("moneySilver", userInfo?.MoneySilver?.Trim().ToString() ?? "0"),
                    new Claim("moneyBronze", userInfo?.MoneyBronze?.Trim().ToString() ?? "0")
                ], "v2ex");
            }
            catch (Exception)
            {
                _logger.LogError("Failed to create ClaimsIdentity");
            }
        }

        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public Task LoginAsync(UserInfo userInfo)
    {
        _cookieContainerManager.Login(userInfo);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }

    public Task LogoutAsync()
    {
        _cookieContainerManager.Logout();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return Task.CompletedTask;
    }
}