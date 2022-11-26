using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using V2exSharp.Models;

namespace V2exSharp
{
    public class V2exApiClient : IV2exApiClient
    {
        private const string endpointV1 = "https://v2ex.com/api/";
        private const string endpointV2 = "https://www.v2ex.com/api/v2/";

        private readonly Dictionary<string, object> responseCache = new();
        private readonly V2exSharpOptions _options = new();
        private readonly HttpClient _httpClient;
        private readonly ILogger<V2exApiClient> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration">生成参考：https://v2ex.com/help/personal-access-token</param>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public V2exApiClient(
            Action<V2exSharpOptions> configuration,
            HttpClient httpClient,
            ILogger<V2exApiClient> logger)
        {
            configuration?.Invoke(_options);
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<V2Node>> GetNodeListAsync(CancellationToken cancellationToken = default)
        {
            var queryString = new Dictionary<string,string>
            {
                {"fields", "id,name,title,topics,aliases"},
                {"sort_by", "topics"},
                {"reverse", "1"}
            };

            var request = CreateGetRequest($"{endpointV1}nodes/list.json", queryString);
            return await RequestGetAsync<IEnumerable<V2Node>>(request, cancellationToken);
        }

        public async Task<IEnumerable<V2Topic>> GetHotTopicsAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(HttpMethod.Get, $"{endpointV1}topics/hot.json");
            return await RequestGetAsync<IEnumerable<V2Topic>>(request, cancellationToken);
        }

        public async Task<IEnumerable<V2Topic>> GetLatestTopicsAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(HttpMethod.Get, $"{endpointV1}topics/latest.json");
            return await RequestGetAsync<IEnumerable<V2Topic>>(request, cancellationToken);
        }

        public async Task<V2Node> GetNodesShowAsync(string name, CancellationToken cancellationToken = default)
        {
            var queryString = new Dictionary<string,string> {{"name", name}};

            var request = CreateGetRequest($"{endpointV1}nodes/show.json", queryString);
            return await RequestGetAsync<V2Node>(request, cancellationToken);
        }

        public async Task<V2Member> GetMemberShowAsync(string username, int? id = null,
            CancellationToken cancellationToken = default)
        {
            var queryString = new Dictionary<string, string> {{"username", username}};
            if (id.HasValue)
            {
                queryString.Add("id", id.ToString());
            }

            var request = CreateGetRequest($"{endpointV1}members/show.json", queryString);
            return await RequestGetAsync<V2Member>(request, cancellationToken);
        }

        public async Task<V2Response<IEnumerable<V2Topic>>> GetTopicsAsync(string nodeName, int page = 1,
            CancellationToken cancellationToken = default)
        {
            var queryString = new Dictionary<string, string> {{"nodeName", nodeName}};

            var request = CreateGetRequest($"{endpointV2}nodes/{nodeName}/topics", queryString);
            return await RequestGetAsync<V2Response<IEnumerable<V2Topic>>>(request, cancellationToken);
        }

        public async Task<V2Response<V2Topic>> GetTopicAsync(int topicId, CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(HttpMethod.Get, $"{endpointV2}topics/{topicId}");
            return await RequestGetAsync<V2Response<V2Topic>>(request, cancellationToken);
        }

        public async Task<V2Response<V2Node>> GetNodeAsync(string nodeName,
            CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(HttpMethod.Get, $"{endpointV2}nodes/{nodeName}");
            return await RequestGetAsync<V2Response<V2Node>>(request, cancellationToken);
        }

        public async Task<V2Response<IEnumerable<V2Notification>>> GetNotificationAsync(int page = 1,
            CancellationToken cancellationToken = default)
        {
            var queryString = new Dictionary<string, string> {{"p", page.ToString()}};

            var request = CreateGetRequest($"{endpointV2}notifications", queryString);
            return await RequestGetAsync<V2Response<IEnumerable<V2Notification>>>(request, cancellationToken);
        }

        public async Task<V2Response<object>> DeleteNotificationAsync(int notificationId,
            CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(HttpMethod.Delete, $"{endpointV2}notifications/{notificationId}");
            return await RequestGetAsync<V2Response<object>>(request, cancellationToken);
        }

        public async Task<V2Response<V2Member>> GetMemberAsync(CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(HttpMethod.Get, $"{endpointV2}member");
            return await RequestGetAsync<V2Response<V2Member>>(request, cancellationToken);
        }

        public async Task<V2Response<IEnumerable<V2Reply>>> GetRepliesAsync(int topicId, int page = 1,
            CancellationToken cancellationToken = default)
        {
            var request = CreateRequest(HttpMethod.Get, $"{endpointV2}topics/{topicId}/replies?p={page}");
            return await RequestGetAsync<V2Response<IEnumerable<V2Reply>>>(request, cancellationToken);
        }

        public async Task<V2Response<V2Token>> CreateTokenAsync(int expiration, string scope,
            CancellationToken cancellationToken = default)
        {
            var request = CreatePostRequest($"{endpointV2}tokens", new {expiration, scope});
            return await RequestGetAsync<V2Response<V2Token>>(request, cancellationToken);
        }

        private async Task<T> RequestGetAsync<T>(HttpRequestMessage request,
            CancellationToken cancellationToken = default, bool forceRefresh = false)
        {
            T result;
            var uri = request.RequestUri.AbsoluteUri;

            if (forceRefresh || !responseCache.ContainsKey(uri))
            {
                var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
#if DEBUG
                foreach (var header in response.Headers)
                {
                    var key = header.Key;
                    var value = header.Value;
                    _logger.LogInformation("{Key}={Value}", key, value);
                }
#endif
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                result = await Task.Run(() => JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    WriteIndented = true
                }), cancellationToken).ConfigureAwait(false);

                if (responseCache.ContainsKey(uri))
                {
                    responseCache[uri] = result;
                }
                else
                {
                    responseCache.Add(uri, result);
                }
            }
            else
            {
                result = (T) responseCache[uri];
            }

            return result;
        }

        private HttpRequestMessage CreateRequest(HttpMethod httpMethod, string requestUri)
        {
            var request = new HttpRequestMessage(httpMethod, requestUri);
            request.Headers.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
            return request;
        }

        private HttpRequestMessage CreateGetRequest(
            string requestUri,
            IDictionary<string, string> args)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var kv in args)
            {
                queryString.Add(kv.Key, kv.Value);
            }

            return CreateRequest(HttpMethod.Get, $"{requestUri}?{queryString}");
        }

        private HttpRequestMessage CreatePostRequest(
            string requestUri,
            object body)
        {
            var request = CreateRequest(HttpMethod.Post, requestUri);
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
            return request;
        }
    }
}