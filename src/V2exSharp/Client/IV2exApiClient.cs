using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using V2exSharp.Models;

namespace V2exSharp.Client
{
    public interface IV2exApiClient
    {
        /// <summary>
        /// 获取节点列表
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<V2Node>> GetNodeListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取最热主题（相当于首页右侧的 10 大每天的内容。）
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<V2Topic>> GetHotTopicsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取最新主题（相当于首页的“全部”这个 tab 下的最新内容。）
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<V2Topic>> GetLatestTopicsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取节点信息（获得指定节点的名字，简介，URL 及头像图片的地址。）
        /// </summary>
        /// <param name="name"> 节点名（V2EX 的节点名全是半角英文或者数字）</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Node> GetNodesShowAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取用户主页（获得指定用户的自我介绍，及其登记的社交网站信息。）
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="id">用户在 V2EX 的数字 ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Member> GetMemberShowAsync(string username, int? id = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取指定节点下的主题
        /// </summary>
        /// <param name="nodeName">节点名，如 "swift"</param>
        /// <param name="page">分页页码，默认为 1</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Response<IEnumerable<V2Topic>>> GetTopicsAsync(string nodeName, int page = 1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取指定主题
        /// </summary>
        /// <param name="topicId">主题ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Response<V2Topic>> GetTopicAsync(int topicId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取指定节点
        /// </summary>
        /// <param name="nodeName">节点名</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Response<V2Node>> GetNodeAsync(string nodeName, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 获取最新的提醒
        /// </summary>
        /// <param name="page">分页页码，默认为 1</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Response<IEnumerable<V2Notification>>> GetNotificationAsync(int page = 1,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 删除指定的提醒
        /// </summary>
        /// <param name="notificationId">提醒ID</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Response<object>> DeleteNotificationAsync(int notificationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取自己的 Profile
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Response<V2Member>> GetMemberAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 创建新的令牌（你可以在系统中最多创建 10 个 Personal Access Token。）
        /// </summary>
        /// <param name="expiration">可支持的值：2592000，5184000，7776000 或者 15552000，即 30 天，60 天，90 天或者 180 天的秒数</param>
        /// <param name="scope">可选 everything 或者 regular，如果是 regular 类型的 Token 将不能用于进一步创建新的 token</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<V2Response<V2Token>> CreateTokenAsync(int expiration,string scope, CancellationToken cancellationToken = default);
    }
}