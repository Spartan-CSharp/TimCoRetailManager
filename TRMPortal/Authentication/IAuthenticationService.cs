using TRMCommon.Library.Authentication;

using TRMPortal.Models;

namespace TRMPortal.Authentication
{
	public interface IAuthenticationService
	{
		Task<AuthenticatedUser> Login(AuthenticationUserModel userForAuthentication);
		Task Logout();
	}
}