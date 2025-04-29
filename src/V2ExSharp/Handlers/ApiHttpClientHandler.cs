using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using V2exSharp.Managers;

namespace V2exSharp.Handlers;

public sealed class ApiHttpClientHandler : HttpClientHandler
{
    private readonly NetworkProxyManager _networkProxyManager;

    public ApiHttpClientHandler(
        CookieContainerManager cookieContainerManager,
        NetworkProxyManager networkProxyManager)
    {
        this.CookieContainer = cookieContainerManager.Container;
        this._networkProxyManager = networkProxyManager;
        this.UseCookies = true;
        this.UseDefaultCredentials = false;
        this.AllowAutoRedirect = false;
        this.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

#if WINDOWS
        var networkSettings = this._networkProxyManager.GetAsync().Result;
        var proxyUrl = networkSettings.GetProxyUrl();
        if (proxyUrl == null)
        {
            this.Proxy = null;
        }
        else
        {
            this.Proxy = new WebProxy(proxyUrl);
        }
#endif
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await base.SendAsync(request, cancellationToken);
    }
}
