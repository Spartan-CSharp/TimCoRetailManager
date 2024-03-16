using TRMPortal.Models;

namespace TRMPortal.Authentication
{
	public interface IAuthenticationService
	{
		Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication);
		Task Logout();
	}
}