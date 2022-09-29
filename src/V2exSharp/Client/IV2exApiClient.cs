using System.Collections.Generic;
using System.Threading.Tasks;

namespace V2exSharp.Client
{
    public interface IV2exApiClient
    {
        Task<IReadOnlyList<object>> GetNodeListAsync();
    }
}