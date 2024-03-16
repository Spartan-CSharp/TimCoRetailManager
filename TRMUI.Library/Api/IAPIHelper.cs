using TRMCommon.Library.Authentication;

namespace TRMUI.Library.Api
{
	public interface IAPIHelper
	{
		HttpClient ApiClient { get; }
		void LogOffUser();
		Task<AuthenticatedUser> Authenticate(string username, string password);
		Task GetLoggedInUserInfo(string token);
	}
}