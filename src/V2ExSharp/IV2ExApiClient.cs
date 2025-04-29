using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using V2exSharp.Models;

namespace V2exSharp;

public interface IV2ExApiClient
{
    Task<DailyHotInfo?> GetDailyHotAsync(CancellationToken cancellationToken = default);
    Task<NodeInfo?> GetNodeInfoAsync(string nodeName, CancellationToken cancellationToken = default);

    Task<NodePageInfo?> GetNodePageInfoAsync(string nodeName, int page = 1,
        CancellationToken cancellationToken = default);

    Task<NodesInfo?> GetNodesInfoAsync(CancellationToken cancellationToken = default);
    Task<NodesInfo?> GetNodesInfo2Async(CancellationToken cancellationToken = default);
    Task<MemberInfo?> GetMemberInfoAsync(string username, CancellationToken cancellationToken = default);

    Task<SoV2EXSearchResultInfo?> SearchAsync(string keyword, int from = 0, string sort = "created",
        CancellationToken cancellationToken = default);

    Task<NewsInfo> GetTabTopicsAsync(string? tab = null, CancellationToken cancellationToken = default);
    Task<NewsInfo> GetRecentTopicsAsync(CancellationToken cancellationToken = default);

    Task<TagInfo> GetTagInfoAsync(string tagName, int page = 1,
        CancellationToken cancellationToken = default);

    Task<LoginParameters> GetLoginParametersAsync(CancellationToken cancellationToken = default);

    Task<byte[]> GetCaptchaImageAsync(LoginParameters loginParameters,
        CancellationToken cancellationToken = default);

    Task<NewsInfo> LoginAsync(
        LoginParameters loginParameters,
        string username,
        string password,
        string captcha, CancellationToken cancellationToken = default);

    Task<TopicInfo> GetTopicDetailAsync(string topicId, int page = 1,
        CancellationToken cancellationToken = default);

    Task<NotificationInfo?> GetNotificationsAsync(int page = 1,
        CancellationToken cancellationToken = default);

    Task<FollowingInfo?> GetFollowingInfoAsync(int page = 1, CancellationToken cancellationToken = default);

    Task<FavoriteTopicsInfo?> GetFavoriteTopicsAsync(int page = 1,
        CancellationToken cancellationToken = default);

    Task<FavoriteNodeInfo> GetFavoriteNodesAsync(CancellationToken cancellationToken = default);
    Task<NodesNavInfo> GetNodesNavInfoAsync(CancellationToken cancellationToken = default);

    Task<MemberPageInfo> GetUserPageInfoAsync(string username,
        CancellationToken cancellationToken = default);

    Task<BingSearchResultInfo> BingSearchAsync(string url);
    Task<CreateTopicParameter> GetCreateTopicParameterAsync(CancellationToken cancellationToken = default);

    Task<TopicInfo> PostTopicAsync(string title, string content,
        string nodeId, string once, CancellationToken cancellationToken = default);

    Task<AppendTopicParameter> GetAppendTopicParameterAsync(string topicId,
        CancellationToken cancellationToken = default);

    Task<TopicInfo> AppendTopicAsync(string topicId,
        string once,
        string content, CancellationToken cancellationToken = default);

    Task<UnitInfo> ThanksReplierAsync(string replyId, string once,
        CancellationToken cancellationToken = default);

    Task<ThanksResult?> ThankCreatorAsync(string topicId, string once,
        CancellationToken cancellationToken = default);

    Task<ThanksInfo> ThanksMoneyAsync(CancellationToken cancellationToken = default);

    Task<TopicInfo> IgnoreTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default);

    Task<TopicInfo> UnignoreTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default);

    Task<UnitInfo> IgnoreReplyAsync(string replyId, string once,
        CancellationToken cancellationToken = default);

    Task<NodeTopicInfo> IgnoreNodeAsync(string nodeId, string once,
        CancellationToken cancellationToken = default);

    Task<NodeTopicInfo> UnignoreNodeAsync(string nodeId, string once,
        CancellationToken cancellationToken = default);

    Task<TopicInfo> UnfavoriteTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default);

    Task<TopicInfo> FavoriteTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default);

    Task<UnitInfo> UpTopicAsync(string topicId, string once, CancellationToken cancellationToken = default);

    Task<UnitInfo> DownTopicAsync(string topicId, string once,
        CancellationToken cancellationToken = default);

    Task<TopicInfo> ReplyTopicAsync(string topicId, string content, string once,
        CancellationToken cancellationToken = default);

    Task<UnitInfo> BlockUserAsync(string url, CancellationToken cancellationToken = default);
    Task<MemberPageInfo> FollowUserAsync(string url, CancellationToken cancellationToken = default);
    Task<UnitInfo> FavoriteNodeAsync(string url, CancellationToken cancellationToken = default);
    Task<DailyInfo> GetDailyInfoAsync(CancellationToken cancellationToken = default);
    Task<DailyInfo> CheckInAsync(string once, CancellationToken cancellationToken = default);

    Task<NewsInfo> SignInTwoStepAsync(string code, string once,
        CancellationToken cancellationToken = default);

    Task<DailyInfo> RequestByUrlAsync(string url, CancellationToken cancellationToken = default);
    Task<TopicInfo> FadeTopicAsync(string url, CancellationToken cancellationToke = default);
    Task<TopicInfo> StickyTopicAsync(string url, CancellationToken cancellationToke = default);
}