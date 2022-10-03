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
    }
}