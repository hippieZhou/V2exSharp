using System.Net.Http;

namespace V2exSharp;

public class V2ExApiClient(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;
}