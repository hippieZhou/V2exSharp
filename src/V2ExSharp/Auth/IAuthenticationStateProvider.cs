using System.Threading.Tasks;
using V2exSharp.Models;

namespace V2exSharp.Auth;

public interface IAuthenticationStateProvider
{
    Task LoginAsync(UserInfo userInfo);
    Task LogoutAsync();
}