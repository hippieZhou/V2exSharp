using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using V2exSharp.Constants;
using V2exSharp.Exceptions;
using V2exSharp.Extensions;
using V2exSharp.Models;

namespace V2exSharp;

public class V2ExApiClient(HttpClient httpClient, ILogger<V2ExApiClient> logger) : IV2ExApiClient
{
    public async Task<DailyHotInfo?> GetDailyHotAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/api/topics/hot.json";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.ReadFromJson<DailyHotInfo>();
    }

    public async Task<NodeInfo?> GetNodeInfoAsync(string nodeName, CancellationToken cancellationToken = default)
    {
        var url = $"/api/nodes/show.json?name={nodeName}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.ReadFromJson<NodeInfo>();
    }

    public async Task<NodePageInfo?> GetNodePageInfoAsync(string nodeName, int page = 1,
        CancellationToken cancellationToken = default)
    {
        var url = $"/go/{nodeName}?p={page}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.ReadFromJson<NodePageInfo>();
    }

    public async Task<NodesInfo?> GetNodesInfoAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/api/nodes/s2.json";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.ReadFromJson<NodesInfo>();
    }

    public async Task<NodesInfo?> GetNodesInfo2Async(CancellationToken cancellationToken = default)
    {
        const string url = "api/nodes/list.json?fields=name,title,topics,aliases&sort_by=topics&reverse=1";
        var response = await httpClient.GetAsync(url, cancellationToken);
        return await response.ReadFromJson<NodesInfo>();
    }

    public async Task<MemberInfo?> GetMemberInfoAsync(string username, CancellationToken cancellationToken = default)
    {
        var url = $"/api/members/show.json?username={username}";
        var response = await httpClient.GetAsync(url, cancellationToken);
        return await response.ReadFromJson<MemberInfo>();
    }

    public async Task<SoV2EXSearchResultInfo?> SearchAsync(string keyword, int from = 0, string sort = "created",
        CancellationToken cancellationToken = default)
    {
        // https://github.com/bynil/sov2ex/blob/v2/API.md
        var queryString = new Dictionary<string, string>
        {
            { "q", keyword },
            { "from", from.ToString() },
            { "sort", sort },
            { "size", 50.ToString() }
        };

        var url = $"https://www.sov2ex.com/api/search?{EncodeQuerystring(queryString)}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.ReadFromJson<SoV2EXSearchResultInfo>();

        string EncodeQuerystring(Dictionary<string, string> queryStrings) =>
            string.Join("&", queryStrings.Select(x => $"{x.Key}={x.Value}"));
    }

    public async Task<NewsInfo> GetTabTopicsAsync(string? tab = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/?tab=" + tab);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/");
        var response = await httpClient.SendAsync(request, cancellationToken);

        return await response.GetEncapsulatedData<NewsInfo>(logger);
    }

    public async Task<NewsInfo> GetRecentTopicsAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/recent";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.GetEncapsulatedData<NewsInfo>(logger);
    }

    public async Task<TagInfo> GetTagInfoAsync(string tagName, int page = 1,
        CancellationToken cancellationToken = default)
    {
        var url = $"/tag/{tagName}?p={page}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.GetEncapsulatedData<TagInfo>(logger);
    }

    public async Task<LoginParameters> GetLoginParametersAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/signin?next=/";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.GetEncapsulatedData<LoginParameters, RestrictedProblem>((error) =>
        {
            if (error.IsRestricted())
            {
                throw new InvalidOperationException(error.RestrictedContent);
            }
        }, logger);
    }

    public async Task<byte[]> GetCaptchaImageAsync(LoginParameters loginParameters,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var url = $"{loginParameters.Captcha}?once={loginParameters.Once}&now={now}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public async Task<NewsInfo> LoginAsync(
        LoginParameters loginParameters,
        string username,
        string password,
        string captcha, CancellationToken cancellationToken = default)
    {
        const string url = "/signin";
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { loginParameters.NameParameter, username },
                { loginParameters.PasswordParameter, password },
                { loginParameters.CaptchaParameter, captcha },
                { "once", loginParameters.Once },
                { "next", "/" },
            })
        };

        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/signin");
        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Found
            && response.Headers.Location != request.RequestUri)
        {
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            response = await httpClient.SendAsync(redirectRequest, cancellationToken);
        }
        else
        {
            var problem = await response.GetEncapsulatedData<LoginProblem>(logger);
            throw new InvalidOperationException(string.Join(" ", problem.Errors));
        }

        return await response.GetEncapsulatedData<NewsInfo>(logger);
    }


    public async Task<TopicInfo> GetTopicDetailAsync(string topicId, int page = 1,
        CancellationToken cancellationToken = default)
    {
        var url = $"/t/{topicId}?p={page}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.GetEncapsulatedData<TopicInfo>(logger);
    }

    public async Task<NotificationInfo?> GetNotificationsAsync(int page = 1,
        CancellationToken cancellationToken = default)
    {
        var url = $"/notifications?p={page}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await httpClient.SendAsync(request, cancellationToken);

        return await response.GetEncapsulatedData<NotificationInfo>(logger);
    }


    public async Task<FollowingInfo?> GetFollowingInfoAsync(int page = 1, CancellationToken cancellationToken = default)
    {
        var url = $"/my/following?p={page}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await httpClient.SendAsync(request, cancellationToken);

        return await response.GetEncapsulatedData<FollowingInfo>(logger);
    }

    public async Task<FavoriteTopicsInfo?> GetFavoriteTopicsAsync(int page = 1,
        CancellationToken cancellationToken = default)
    {
        var url = $"/my/topics?p={page}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await httpClient.SendAsync(request, cancellationToken);

        return await response.GetEncapsulatedData<FavoriteTopicsInfo>(logger);
    }

    public async Task<FavoriteNodeInfo> GetFavoriteNodesAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/my/nodes";
        var response = await httpClient.GetAsync(url, cancellationToken);

        var nodeInfo = await response.GetEncapsulatedData<FavoriteNodeInfo>(logger);

        foreach (var item in nodeInfo.Items)
        {
            item.Image = UrlUtilities.CompleteUrl(item.Image);
        }

        return nodeInfo;
    }

    public async Task<NodesNavInfo> GetNodesNavInfoAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/";
        var response = await httpClient.GetAsync(url, cancellationToken);
        return await response.GetEncapsulatedData<NodesNavInfo>(logger);
    }

    public async Task<MemberPageInfo> GetUserPageInfoAsync(string username,
        CancellationToken cancellationToken = default)
    {
        var url = $"/member/{username}";

        var response = await httpClient.GetAsync(url, cancellationToken);
        return await response.GetEncapsulatedData<MemberPageInfo>(logger);
    }

    public Task<BingSearchResultInfo> BingSearchAsync(string url)
    {
        throw new NotImplementedException();
    }

    public async Task<CreateTopicParameter> GetCreateTopicParameterAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/new";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.GetEncapsulatedData<CreateTopicParameter>(logger);
    }

    public async Task<TopicInfo> PostTopicAsync(string title, string content,
        string nodeId, string once, CancellationToken cancellationToken = default)
    {
        const string url = "/new";
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "title", title },
                { "syntax", "default" },
                { "content", content },
                { "node_name", nodeId },
                { "once", once },
            })
        };
        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Found
            && response.Headers.Location != request.RequestUri)
        {
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            response = await httpClient.SendAsync(redirectRequest, cancellationToken);
        }
        else if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            var problem = await response.GetEncapsulatedData<Problem>(logger);
            throw new CreateTopicException(problem);
        }
        else
        {
            throw new InvalidOperationException(response.ReasonPhrase);
        }

        return await response.GetEncapsulatedData<TopicInfo>(logger);
    }

    public async Task<AppendTopicParameter> GetAppendTopicParameterAsync(string topicId,
        CancellationToken cancellationToken = default)
    {
        var url = $"/t/{topicId}/append";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/t/{topicId}");
        var response = await httpClient.SendAsync(request, cancellationToken);
        return await response.GetEncapsulatedData<AppendTopicParameter>(logger);
    }

    public async Task<TopicInfo> AppendTopicAsync(string topicId,
        string once,
        string content, CancellationToken cancellationToken = default)
    {
        var url = $"/t/{topicId}/append";
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "content", content },
                { "once", once },
            })
        };
        var response = await httpClient.SendAsync(request, cancellationToken);

        return await response.GetEncapsulatedData<TopicInfo>(logger);
    }

    public async Task<UnitInfo> ThanksReplierAsync(string replyId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/thank/reply/{replyId}?once={once}";
        var response = await httpClient.PostAsync(url, null, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return UnitInfo.Parse(content);
    }

    public async Task<ThanksResult?> ThankCreatorAsync(string topicId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/thank/topic/{topicId}?once={once}";
        var response = await httpClient.PostAsync(url, null, cancellationToken);

        return await response.ReadFromJson<ThanksResult>();
    }

    public async Task<ThanksInfo> ThanksMoneyAsync(CancellationToken cancellationToken = default)
    {
        const string url = "/ajax/money";
        var response = await httpClient.PostAsync(url, null, cancellationToken);

        return await response.GetEncapsulatedData<ThanksInfo>(logger);
    }

    public async Task<TopicInfo> IgnoreTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/ignore/topic/{topicId}?once={once}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/t/{topicId}");

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Found
            && response.Headers.Location != null)
        {
            var topicUrl = $"{UrlUtilities.BASE_URL}/t/{topicId}";
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, topicUrl);
            response = await httpClient.SendAsync(redirectRequest, cancellationToken);
        }
        else
        {
            //todo: handle the reply error.
            throw new InvalidOperationException("Can not reply the topic.");
        }

        return await response.GetEncapsulatedData<TopicInfo>(logger);
    }

    public async Task<TopicInfo> UnignoreTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/unignore/topic/{topicId}?once={once}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Found
            && response.Headers.Location != null)
        {
            var topicUrl = $"{UrlUtilities.BASE_URL}/t/{topicId}";
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, topicUrl);
            response = await httpClient.SendAsync(redirectRequest, cancellationToken);
        }
        else
        {
            //todo: handle the reply error.
            throw new InvalidOperationException("Can not reply the topic.");
        }

        return await response.GetEncapsulatedData<TopicInfo>(logger);
    }

    public async Task<UnitInfo> IgnoreReplyAsync(string replyId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/ignore/reply/{replyId}?once={once}";
        var response = await httpClient.GetAsync(url, cancellationToken);
        return await response.GetEncapsulatedData<UnitInfo>(logger);
    }

    public async Task<NodeTopicInfo> IgnoreNodeAsync(string nodeId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/settings/ignore/node/{nodeId}?once={once}";
        var response = await httpClient.GetAsync(url, cancellationToken);
        return await response.GetEncapsulatedData<NodeTopicInfo>(logger);
    }

    public async Task<NodeTopicInfo> UnignoreNodeAsync(string nodeId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/settings/ignore/node/{nodeId}?once={once}";
        var response = await httpClient.GetAsync(url, cancellationToken);

        return await response.GetEncapsulatedData<NodeTopicInfo>(logger);
    }

    public async Task<TopicInfo> UnfavoriteTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/unfavorite/topic/{topicId}?once={once}";

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/t/{topicId}");

        var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Found
            && response.Headers.Location != null)
        {
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            response = await httpClient.SendAsync(redirectRequest, cancellationToken);
        }
        else
        {
            //todo: handle the reply error.
            throw new InvalidOperationException("Can not reply the topic.");
        }

        return await response.GetEncapsulatedData<TopicInfo>(logger);
    }

    public async Task<TopicInfo> FavoriteTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/favorite/topic/{topicId}?once={once}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/t/{topicId}");

        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Found
            && response.Headers.Location != null)
        {
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            response = await httpClient.SendAsync(redirectRequest, cancellationToken);
        }
        else
        {
            //todo: handle the reply error.
            throw new InvalidOperationException("Can not reply the topic.");
        }

        return await response.GetEncapsulatedData<TopicInfo>(logger);
    }

    public async Task<UnitInfo> UpTopicAsync(string topicId, string once, CancellationToken cancellationToken = default)
    {
        var url = $"/up/topic/{topicId}?once={once}";
        var response = await httpClient.GetAsync(url, cancellationToken);
        return await response.GetEncapsulatedData<UnitInfo>(logger);
    }

    public async Task<UnitInfo> DownTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/down/topic/{topicId}?once={once}";
        var response = await httpClient.GetAsync(url, cancellationToken);
        var result = await response.GetEncapsulatedData<UnitInfo>(logger);
        return result;
    }

    public async Task<TopicInfo> ReplyTopicAsync(string topicId, string content, string once,
        CancellationToken cancellationToken = default)
    {
        var url = $"/t/{topicId}";
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "content", content },
                { "once", once },
            })
        };
        var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Found
            && response.Headers.Location != request.RequestUri)
        {
            var redirectRequest = new HttpRequestMessage(HttpMethod.Get, response.Headers.Location);
            response = await httpClient.SendAsync(redirectRequest, cancellationToken);
        }
        else
        {
            //todo: handle the reply error.
            throw new InvalidOperationException("Can not reply the topic.");
        }

        var result = await response.GetEncapsulatedData<TopicInfo>(logger);
        return result;
    }

    public async Task<UnitInfo> BlockUserAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(url, cancellationToken);
        var result = await response.GetEncapsulatedData<UnitInfo>(logger);
        return result;
    }

    public async Task<MemberPageInfo> FollowUserAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(url, cancellationToken);
        var result = await response.GetEncapsulatedData<MemberPageInfo>(logger);
        return result;
    }

    public async Task<UnitInfo> FavoriteNodeAsync(string url, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/mission/daily");
        var response = await httpClient.SendAsync(request, cancellationToken);
        var result = await response.GetEncapsulatedData<UnitInfo>(logger);
        return result;
    }

    public async Task<DailyInfo> GetDailyInfoAsync(CancellationToken cancellationToken = default)
    {
        var url = "/mission/daily";
        var response = await httpClient.GetAsync(url, cancellationToken);
        var result = await response.GetEncapsulatedData<DailyInfo>(logger);
        return result;
    }

    public async Task<DailyInfo> CheckInAsync(string once, CancellationToken cancellationToken = default)
    {
        var url = $"/mission/daily/redeem?once={once}";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/mission/daily");
        var response = await httpClient.SendAsync(request, cancellationToken);
        var result = await response.GetEncapsulatedData<DailyInfo>(logger);
        return result;
    }

    public async Task<NewsInfo> SignInTwoStepAsync(string code, string once,
        CancellationToken cancellationToken = default)
    {
        var url = "/2fa?next=/mission/daily";

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "code", code },
                { "once", once },
            })
        };
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/mission/daily");
        var response = await httpClient.SendAsync(request, cancellationToken);
        var result = await response.GetEncapsulatedData<NewsInfo>(logger);
        return result;
    }

    public async Task<DailyInfo> RequestByUrlAsync(string url, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Referer", $"{UrlUtilities.BASE_URL}/mission/daily");
        var response = await httpClient.SendAsync(request, cancellationToken);
        var result = await response.GetEncapsulatedData<DailyInfo>(logger);
        return result;
    }

    public async Task<TopicInfo> FadeTopicAsync(string url, CancellationToken cancellationToke = default)
    {
        var response = await httpClient.GetAsync(url, cancellationToke);
        var result = await response.GetEncapsulatedData<TopicInfo>(logger);
        return result;
    }

    public async Task<TopicInfo> StickyTopicAsync(string url, CancellationToken cancellationToke = default)
    {
        var response = await httpClient.GetAsync(url, cancellationToke);
        var result = await response.GetEncapsulatedData<TopicInfo>(logger);
        return result;
    }
}