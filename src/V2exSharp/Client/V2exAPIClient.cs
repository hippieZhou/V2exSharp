using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using V2exSharp.Option;

namespace V2exSharp.Client
{
    public class V2exApiClient : IV2exApiClient
    {
        private const string endpointV1 = "https://v2ex.com/api/";
        private const string endpointV2 = "https://www.v2ex.com/api/v2/";

        private readonly V2exSharpOption _option;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option">生成参考：https://v2ex.com/help/personal-access-token</param>
        public V2exApiClient(V2exSharpOption option)
        {
            if (string.IsNullOrWhiteSpace(option.AccessToken))
            {
                throw new ArgumentNullException(nameof(option.AccessToken));
            }

            this._option = option;
        }

        public Task<IReadOnlyList<object>> GetNodeListAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}