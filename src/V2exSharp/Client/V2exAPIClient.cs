using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using V2exSharp.Models;
using V2exSharp.Option;

namespace V2exSharp.Client
{
    public class V2exApiClient : IV2exApiClient
    {
        private const string endpointV1 = "https://v2ex.com/api/";
        private const string endpointV2 = "https://www.v2ex.com/api/v2/";

        private readonly V2ExSharpConfiguration _configuration = new();
        private readonly HttpClient _httpClient;
        private readonly ILogger<V2exApiClient> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration">生成参考：https://v2ex.com/help/personal-access-token</param>
        /// <param name="httpClient"></param>
        /// <param name="logger"></param>
        public V2exApiClient(
            Action<V2ExSharpConfiguration> configuration,
            HttpClient httpClient,
            ILogger<V2exApiClient> logger)
        {
            configuration?.Invoke(_configuration);
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<V2Node>> GetNodeListAsync(CancellationToken cancellationToken = default)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("fields", "id,name,title,topics,aliases");
            queryString.Add("sort_by", "topics");
            queryString.Add("reverse", "1");

            return await RequestGetAsync<IEnumerable<V2Node>>(
                endpointV1 + $"nodes/list.json?{queryString}",
                cancellationToken);
        }

        public async Task<IEnumerable<V2Topic>> GetHotTopicsAsync(CancellationToken cancellationToken = default)
        {
            return await RequestGetAsync<IEnumerable<V2Topic>>($"{endpointV1}topics/hot.json", cancellationToken);
        }

        public async Task<IEnumerable<V2Topic>> GetLatestTopicsAsync(CancellationToken cancellationToken = default)
        {
            return await RequestGetAsync<IEnumerable<V2Topic>>($"{endpointV1}topics/latest.json", cancellationToken);
        }

        public async Task<V2Node> GetNodesShowAsync(string name, CancellationToken cancellationToken = default)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("name", name);
            return await RequestGetAsync<V2Node>($"{endpointV1}nodes/show.json?{queryString}", cancellationToken);
        }

        public async Task<V2Member> GetMemberShowAsync(string username, int? id = null,
            CancellationToken cancellationToken = default)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("username", username);
            if (id.HasValue)
            {
                queryString.Add("id", id.ToString());
            }

            return await RequestGetAsync<V2Member>($"{endpointV1}members/show.json?{queryString}", cancellationToken);
        }

        public async Task<V2Response<IEnumerable<V2Topic>>> GetTopicsAsync(string nodeName, int page = 1,
            CancellationToken cancellationToken = default)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("nodeName", nodeName);
            return await RequestGetAsync<V2Response<IEnumerable<V2Topic>>>($"{endpointV2}nodes/{nodeName}/topics",
                cancellationToken);
        }

        public async Task<V2Response<V2Topic>> GetTopicAsync(int topicId, CancellationToken cancellationToken = default)
        {
            return await RequestGetAsync<V2Response<V2Topic>>($"{endpointV2}topics/{topicId}", cancellationToken);
        }

        public async Task<V2Response<V2Node>> GetNodeAsync(string nodeName, CancellationToken cancellationToken = default)
        {
            return await RequestGetAsync<V2Response<V2Node>>($"{endpointV2}nodes/{nodeName}", cancellationToken);
        }

        public async Task<V2Response<IEnumerable<V2Notification>>> GetNotificationAsync(int page = 1,
            CancellationToken cancellationToken = default)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString.Add("p", page.ToString());
            return await RequestGetAsync<V2Response<IEnumerable<V2Notification>>>($"{endpointV2}notifications?{queryString}",
                cancellationToken);
        }

        private async Task<T> RequestGetAsync<T>(string requestUri, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration.AccessToken);
            // request.Content = new StringContent(JsonSerializer.Serialize(new
            // {
            //     fields = "",
            //     sort_by = "",
            //      = "1",
            // }), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true
            });
        }
    }
}